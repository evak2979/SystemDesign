using SystemDesign.Api.Interfaces;

namespace SystemDesign.Api.Implementations;

public class LGTelevision : ITelevision
{
    public int Volume { get; private set; }
    public int MaxVolume => 50;
    public bool IsOn { get; set; }

    public void TurnOn()
    {
        if (!IsOn)
        {
            Volume = 0;
        }
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
            Volume += 2;
        }
    }

    public void VolumeDown()
    {
        if (Volume != 0 && IsOn)
        {
            Volume -= 2;
        }
    }
}