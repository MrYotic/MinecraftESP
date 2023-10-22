﻿namespace Core.Abstracts;
public unsafe abstract class AbstractRenderHook
{
    public AbstractRenderHook(AbstractRender render) => Render = render;

    public AbstractRender Render;

    protected HookFunction[] hooks;

    protected void SetHooks(params HookFunction[] functions) => hooks = functions;

    public void Attach()
    {
        foreach (var hook in hooks)
        {
            hook.Attach();

            var ptr = (nint*)GL.Interface;
            for (int i = 0; i < GLInterface.FunctionsCount; i++)
                if (ptr[i] == hook.Origin.Addr)
                {
                    ((nint*)GL.Interface)[i] = (nint)hook;
                    break;
                }
        }
    }
}   