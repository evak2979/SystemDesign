namespace SystemDesign.Api.Interfaces;

public interface ITelevision
{
    int Volume { get; }
    bool IsOn { get; }
    
    void TurnOn();
    void TurnOff();
    void VolumeUp();
    void VolumeDown();
}