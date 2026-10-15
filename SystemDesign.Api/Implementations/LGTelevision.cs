using SystemDesign.Api.Interfaces;

namespace SystemDesign.Api.Implementations;

public class LGTelevision : ITelevision
{
    public int Volume { get; private set; }
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
        Volume = Math.Min(Volume + 3, MaxVolume);
    }

    public void VolumeDown()
    {
        Volume = Math.Max(Volume - 3, 0);
    }
}