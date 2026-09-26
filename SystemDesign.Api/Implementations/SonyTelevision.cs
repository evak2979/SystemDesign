using SystemDesign.Api.Interfaces;

namespace SystemDesign.Api.Implementations;

public class SonyTelevision : ITelevision
{
    public int Volume { get; private set; } = 20;
    private const int MaxVolume = 50;
    public bool IsOn { get; set; }
    
    public void TurnOn()
    {
        IsOn = true;
    }

    public void TurnOff()
    {
        IsOn = false;
    }

    public void VolumeUp()
    {
        if (Volume != MaxVolume && IsOn)
        {
            Volume += 5;
        }
    }

    public void VolumeDown()
    {
        if (Volume != 0 && IsOn)
        {
            Volume -= 5;
        }
    }
}