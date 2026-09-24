using SystemDesign.Api.Interfaces;

namespace SystemDesign.Api.Implementations;

public class SamsungTelevision : ITelevision
{
    public int Volume { get; private set; } = 10;
    public int MaxVolume => 100;
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
        if (Volume != MaxVolume)
        {
            Volume++;
        }
    }

    public void VolumeDown()
    {
        if (Volume != 0)
        {
            Volume--;
        }
    }
}