namespace SystemDesign.Api.Interfaces;

public interface ITelevision
{
    int Volume { get; }
    int MaxVolume { get; }
    bool IsOn { get; }
    
    void TurnOn();
    void TurnOff();
    void VolumeUp();
    void VolumeDown();
}