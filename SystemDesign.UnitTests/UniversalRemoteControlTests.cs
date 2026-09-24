using SystemDesign.Api.Implementations;

namespace SystemDesign.UnitTests;

public class UniversalRemoteControlTests
{
    [Fact]
    public void PressPower_WhenTVIsOffAndHasBattery_TurnTVOn()
    {
        var tv = new SonyTelevision() { IsOn = false };
        var remote = new UniversalRemoteControl(tv) { HasBattery = true };

        remote.PressPower();
        
        Assert.True(tv.IsOn);
    }
    
    [Fact]
    public void PressPower_WhenTVIsOffAndHasNoBattery_DoesNothing()
    {
        var tv = new SonyTelevision();
        var remote = new UniversalRemoteControl(tv){ HasBattery = false };

        remote.PressPower();
        
        Assert.False(tv.IsOn);
    }

    [Fact]
    public void PressPower_WhenTVIsOnAndHasBattery_TurnTVOff()
    {
        var tv = new SonyTelevision() { IsOn = true };
        var remote = new UniversalRemoteControl(tv) { HasBattery = true };

        remote.PressPower();
        
        Assert.False(tv.IsOn);
    }
    
    [Fact]
    public void PressVolumeUp_WhenHasBattery_IncreasesTVVolume()
    {
        var tv = new SonyTelevision();
        var remote = new UniversalRemoteControl(tv){ HasBattery = true };
        
        remote.PressPower();
        remote.PressVolumeUp();
        
        Assert.Equal(25, tv.Volume);
    }
    
    [Fact]
    public void PressVolumeUp_WhenNoBattery_DoNothing()
    {
        var tv = new SonyTelevision();
        var remote = new UniversalRemoteControl(tv){ HasBattery = true };
        
        remote.PressPower();
        remote.HasBattery = false;
        remote.PressVolumeUp();
        
        Assert.Equal(20, tv.Volume);
    }
}