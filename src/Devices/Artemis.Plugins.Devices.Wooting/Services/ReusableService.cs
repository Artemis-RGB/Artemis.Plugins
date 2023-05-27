using System;
using System.Collections.Generic;
using Artemis.Core.Services;

namespace Artemis.Plugins.Devices.Wooting.Services;

public abstract class ReusableService : IPluginService
{
    private readonly object _lock = new();
    private int _useToken;
    private readonly HashSet<int> _useTokens = new();
    
    protected bool IsActivated { get; private set; }

    internal int RegisterUse()
    {
        ActivateInternal();
        int useToken = ++_useToken;
        _useTokens.Add(useToken);
        return useToken;
    }

    internal void UnregisterUse(int useToken)
    {
        _useTokens.Remove(useToken);
        if (_useTokens.Count == 0)
            DeactivateInternal();
    }
    
    protected abstract void Activate();
    protected abstract void Deactivate();
    
    protected void ActivateInternal()
    {
        lock (_lock)
        {
            if (IsActivated)
                return;
            
            Activate();
            IsActivated = true;
        }
    }
    
    protected void DeactivateInternal()
    {
        lock (_lock)
        {
            if (!IsActivated)
                return;
            
            Deactivate();
            IsActivated = false;
        }
    }
}