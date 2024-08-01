using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace CommonLib;

public sealed class AppBuilder
{
    private Func<Application>? _appFactory;

    /// <summary>
    /// Gets or sets a method to call the initialize the runtime platform services (e. g. AssetLoader)
    /// </summary>
    public Action? RuntimePlatformServicesInitializer { get; private set; }

    /// <summary>
    /// Gets the name of the currently selected windowing subsystem.
    /// </summary>
    public string? RuntimePlatformServicesName { get; private set; }

    /// <summary>
    /// Gets the <see cref="Application"/> instance being initialized.
    /// </summary>
    public Application? Instance { get; private set; }

    /// <summary>
    /// Gets the type of the Instance (even if it's not created yet)
    /// </summary>
    public Type? ApplicationType { get; private set; }

    /// <summary>
    /// Gets or sets a method to call the initialize the windowing subsystem.
    /// </summary>
    public Action? WindowingSubsystemInitializer { get; private set; }

    /// <summary>
    /// Gets the name of the currently selected windowing subsystem.
    /// </summary>
    public string? WindowingSubsystemName { get; private set; }

    /// <summary>
    /// Gets or sets a method to call the initialize the windowing subsystem.
    /// </summary>
    public Action? RenderingSubsystemInitializer { get; private set; }

    /// <summary>
    /// Gets the name of the currently selected rendering subsystem.
    /// </summary>
    public string? RenderingSubsystemName { get; private set; }

    /// <summary>
    /// Gets a method to call after the <see cref="Application"/> is setup.
    /// </summary>
    public Action<AppBuilder> AfterSetupCallback { get; private set; } = builder => { };

    /// <summary>
    /// Callbacks that are commonly used by backends to initialize avalonia views.
    /// </summary>
    private Action<AppBuilder> AfterApplicationSetupCallback { get; set; } = builder => { };

    public Action<AppBuilder> AfterPlatformServicesSetupCallback { get; private set; } = builder => { };

    /// <summary>
    /// Initializes a new instance of the <see cref="AppBuilder"/> class.
    /// </summary>
    private AppBuilder()
    {
    }

    /// <summary>
    /// Begin configuring an <see cref="Application"/>.
    /// </summary>
    /// <typeparam name="TApp">The subclass of <see cref="Application"/> to configure.</typeparam>
    /// <returns>An <see cref="AppBuilder"/> instance.</returns>
    public static AppBuilder Configure<TApp>()
        where TApp : Application, new()
    {
        return new AppBuilder()
        {
            ApplicationType = typeof(TApp),
            _appFactory = () => new TApp()
        };
    }

    /// <summary>
    /// Begin configuring an <see cref="Application"/>.
    /// </summary>
    /// <param name="appFactory">Factory function for <typeparamref name="TApp"/>.</param>
    /// <typeparam name="TApp">The subclass of <see cref="Application"/> to configure.</typeparam>
    /// <remarks><paramref name="appFactory"/> is useful for passing of dependencies to <typeparamref name="TApp"/>.</remarks>
    /// <returns>An <see cref="AppBuilder"/> instance.</returns>
    public static AppBuilder Configure<TApp>(Func<TApp> appFactory)
        where TApp : Application
    {
        return new AppBuilder()
        {
            ApplicationType = typeof(TApp),
            _appFactory = appFactory
        };
    }

    /// <summary>
    /// Begin configuring an <see cref="Application"/>.
    /// Should only be used for testing and design purposes, as it relies on dynamic code.
    /// </summary>
    /// <param name="entryPointType">
    /// Parameter from which <see cref="AppBuilder"/> should be created.
    /// It either needs to have BuildAvaloniaApp -> AppBuilder method or inherit Application.
    /// </param>
    /// <returns>An <see cref="AppBuilder"/> instance. If can't be created, thrown an exception.</returns>
    internal static AppBuilder Configure(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods |
                                    DynamicallyAccessedMemberTypes.NonPublicMethods |
                                    DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
        Type entryPointType)
    {
        var appBuilderObj = entryPointType
            .GetMethod(
                "BuildAvaloniaApp",
                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy,
                null,
                Array.Empty<Type>(),
                null)?
            .Invoke(null, Array.Empty<object?>());

        if (appBuilderObj is AppBuilder appBuilder)
        {
            return appBuilder;
        }

        if (typeof(Application).IsAssignableFrom(entryPointType))
        {
            return Configure(() => (Application)Activator.CreateInstance(entryPointType)!);
        }

        throw new InvalidOperationException(
            $"Unable to create AppBuilder from type \"{entryPointType.FullName}\". " +
            $"Input type either needs to have BuildAvaloniaApp -> AppBuilder method or inherit Application type.");
    }

    private AppBuilder Self => this;

    public AppBuilder AfterSetup(Action<AppBuilder> callback)
    {
        AfterSetupCallback = (Action<AppBuilder>)Delegate.Combine(AfterSetupCallback, callback);
        return Self;
    }

    public AppBuilder AfterApplicationSetup(Action<AppBuilder> callback)
    {
        AfterApplicationSetupCallback =
            (Action<AppBuilder>)Delegate.Combine(AfterPlatformServicesSetupCallback, callback);
        return Self;
    }

    public AppBuilder AfterPlatformServicesSetup(Action<AppBuilder> callback)
    {
        AfterPlatformServicesSetupCallback =
            (Action<AppBuilder>)Delegate.Combine(AfterPlatformServicesSetupCallback, callback);
        return Self;
    }

    public delegate void AppMainDelegate(Application app, string[] args);

    public void Start(AppMainDelegate main, string[] args)
    {
        Setup();
        main(Instance!, args);
    }

    /// <summary>
    /// Sets up the platform-specific services for the application, but does not run it.
    /// </summary>
    /// <returns></returns>
    public AppBuilder SetupWithoutStarting()
    {
        Setup();
        return Self;
    }

    /// <summary>
    /// Specifies a windowing subsystem to use.
    /// </summary>
    /// <param name="initializer">The method to call to initialize the windowing subsystem.</param>
    /// <param name="name">The name of the windowing subsystem.</param>
    /// <returns>An <see cref="AppBuilder"/> instance.</returns>
    public AppBuilder UseWindowingSubsystem(Action initializer, string name = "")
    {
        WindowingSubsystemInitializer = initializer;
        WindowingSubsystemName = name;
        return Self;
    }

    /// <summary>
    /// Specifies a runtime platform subsystem to use.
    /// </summary>
    /// <param name="initializer">The method to call to initialize the runtime platform subsystem.</param>
    /// <param name="name">The name of the runtime platform subsystem.</param>
    /// <returns>An <see cref="AppBuilder"/> instance.</returns>
    public AppBuilder UseRuntimePlatformSubsystem(Action initializer, string name = "")
    {
        RuntimePlatformServicesInitializer = initializer;
        RuntimePlatformServicesName = name;
        return Self;
    }

    /// <summary>
    /// Specifies a standard runtime platform subsystem to use.
    /// </summary>
    /// <returns>An <see cref="AppBuilder"/> instance.</returns>
    public AppBuilder UseStandardRuntimePlatformSubsystem()
    {
        RuntimePlatformServicesInitializer = () => StandardRuntimePlatformServices.Register(ApplicationType?.Assembly);
        RuntimePlatformServicesName = "StandardRuntimePlatform";
        return Self;
    }

    /// <summary>
    /// Sets up the platform-specific services for the <see cref="Application"/>.
    /// </summary>
    private void Setup()
    {
        if (RuntimePlatformServicesInitializer == null)
        {
            throw new InvalidOperationException("No runtime platform services configured.");
        }

        if (WindowingSubsystemInitializer == null)
        {
            throw new InvalidOperationException("No windowing system configured.");
        }

        if (_appFactory == null)
        {
            throw new InvalidOperationException("No Application factory configured.");
        }

        SetupUnsafe();
    }

    /// <summary>
    /// Setup method that doesn't check for input initalizers being set.
    /// Nor
    /// </summary>
    internal void SetupUnsafe()
    {
        RuntimePlatformServicesInitializer?.Invoke();
        RenderingSubsystemInitializer?.Invoke();
        WindowingSubsystemInitializer?.Invoke();
        AfterPlatformServicesSetupCallback?.Invoke(Self);
        Instance = _appFactory!();
        Instance.RegisterServices();
        Instance.Initialize();
        AfterApplicationSetupCallback?.Invoke(Self);
        AfterSetupCallback?.Invoke(Self);
        Instance.OnFrameworkInitializationCompleted();
    }
}