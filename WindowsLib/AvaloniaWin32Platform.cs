namespace WindowsLib;

internal class AvaloniaWin32Platform
{
    public void Initialize(Win32PlatformOptions options)
        => Console.WriteLine($"Win32PlatformOptions: {options}");
}