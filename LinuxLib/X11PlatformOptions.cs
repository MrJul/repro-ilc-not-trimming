namespace LinuxLib;

public class X11PlatformOptions
{
    public IReadOnlyList<X11RenderingMode> RenderingMode { get; set; } = [
        X11RenderingMode.Glx,
        X11RenderingMode.Software
    ];
}