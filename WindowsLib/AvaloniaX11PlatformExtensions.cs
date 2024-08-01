using CommonLib;

namespace WindowsLib;

public static class Win32PlatformExtensions
{
    public static AppBuilder UseWin32(this AppBuilder builder)
        => builder
            .UseStandardRuntimePlatformSubsystem()
            .UseWindowingSubsystem(() => new AvaloniaWin32Platform().Initialize(new Win32PlatformOptions()), "Win32");
}