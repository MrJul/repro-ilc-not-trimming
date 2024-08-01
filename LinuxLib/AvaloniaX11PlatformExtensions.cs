using CommonLib;

namespace LinuxLib;

public static class AvaloniaX11PlatformExtensions
{
    public static AppBuilder UseX11(this AppBuilder builder)
        => builder
            .UseStandardRuntimePlatformSubsystem()
            .UseWindowingSubsystem(() => new AvaloniaX11Platform().Initialize(new X11PlatformOptions()), "X11");
}