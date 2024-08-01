using System.Reflection;

namespace CommonLib;

internal static class StandardRuntimePlatformServices
{
    public static void Register(Assembly? applicationTypeAssembly)
        => Console.WriteLine($"Assembly is {applicationTypeAssembly}");
}