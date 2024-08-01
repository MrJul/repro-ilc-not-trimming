using CommonLib;

namespace TestTrimming;

public static class Program
{
    public static void Main(string[] args)
    {
        var appBuilder = AppBuilder.Configure<App>().UsePlatformDetect();
        appBuilder.Start((_, _) => Console.WriteLine("Started"), args);
    }
}

public class App : Application;