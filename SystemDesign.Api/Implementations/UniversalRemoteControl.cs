using SystemDesign.Api.Interfaces;

namespace SystemDesign.Api.Implementations;

public class UniversalRemoteControl(ITelevision television) : IRemoteControl
{
    public bool HasBattery { get; set; }

    public void PressPower()
    {
        if (!HasBattery) return;

        if (television.IsOn)
            television.TurnOff();
        else
            television.TurnOn();
    }

    public void PressVolumeUp()
    {
        if (HasBattery) {
            television.VolumeUp();
        }
    }

    public void PressVolumeDown()
    {
        if (HasBattery) {
            television.VolumeDown();
        }
    }
}