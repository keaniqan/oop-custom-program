using System.Numerics;
using Raylib_cs;
#nullable disable

namespace MyApp;


class AffinityMeter
{
    private AffinityType _affinity;
    private int _meterCount;
    private int _maxMeterCount;
    private bool _isActive;

    public AffinityMeter(AffinityType affinity, bool isActive)
    {
        _affinity = affinity;
        _meterCount = 0;
        _maxMeterCount = 10;
        _isActive = isActive;
    }
    public void ActivateAffinityMeter(){
        _isActive = true;
    }
    public void DeactivateAffinityMeter(){
        _isActive = false;
    }
    public void IncreaseMeter(){
        _meterCount++;
    }
    public void ResetMeter(){
        _meterCount = 0;
    }
}
