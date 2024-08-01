namespace LinuxLib;

internal class AvaloniaX11Platform
{
    public void Initialize(X11PlatformOptions options)
        => Console.WriteLine($"X11PlatformOptions: {options}");
}