using CommonLib;
using LinuxLib;
using WindowsLib;

namespace TestTrimming;

public static class AppBuilderDesktopExtensions
{
    public static AppBuilder UsePlatformDetect(this AppBuilder builder)
    {
        if (OperatingSystem.IsWindows())
        {
            LoadWin32(builder);
        }
        else if (OperatingSystem.IsLinux())
        {
            LoadX11(builder);
        }
        else
        {
            Console.WriteLine("Unsupported platform");
        }

        return builder;
    }

    private static void LoadWin32(AppBuilder builder)
        => builder.UseWin32();

    private static void LoadX11(AppBuilder builder)
        => builder.UseX11();
}