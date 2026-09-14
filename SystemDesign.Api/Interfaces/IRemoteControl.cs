namespace SystemDesign.Api.Interfaces;

public interface IRemoteControl
{
    bool HasBattery { get; }

    void PressPower();
    void PressVolumeUp();
    void PressVolumeDown();
}