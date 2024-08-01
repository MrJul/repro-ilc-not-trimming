using System.Runtime.CompilerServices;

namespace CommonLib;

public sealed class ActionExecutor
{
    private Action? _action;

    [MethodImpl(MethodImplOptions.NoInlining)]
    public void SetAction(Action action) => _action = action;

    [MethodImpl(MethodImplOptions.NoInlining)]
    public void Execute() => _action!();
}