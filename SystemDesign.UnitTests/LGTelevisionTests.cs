using SystemDesign.Api.Implementations;

namespace SystemDesign.UnitTests;

public class LGTelevisionTests
{
    [Fact]
    public void TurnOn_WhenOff_TurnsTheTelevisionOn()
    {
        var sut = new LGTelevision();

        sut.TurnOn();

        Assert.True(sut.IsOn);
    }

    [Fact]
    public void TurnOff_WhenOn_TurnsTheTelevisionOff()
    {
        var sut = new LGTelevision();
        
        sut.TurnOn();
        sut.TurnOff();
        
        Assert.False(sut.IsOn);
    }

    [Fact]
    public void TurnOn_WhenAlreadyOn_RemainsOnAndVolumeUnaffected()
    {
        var sut = new LGTelevision();

        sut.TurnOn();
        sut.VolumeUp();
        sut.TurnOn();

        Assert.True(sut.IsOn);
        Assert.Equal(3, sut.Volume);
    }
    
    [Fact]
    public void VolumeUp_WhenOn_IncreasesVolumeByThree()
    {
        var sut = new LGTelevision();

        sut.TurnOn();
        sut.VolumeUp();

        Assert.Equal(3, sut.Volume);
    }
    
    [Fact]
    public void VolumeDown_WhenOn_DecreasesVolumeByThree()
    {
        var sut = new LGTelevision();

        sut.TurnOn();
        sut.VolumeUp();
        sut.VolumeUp();
        sut.VolumeDown();

        Assert.Equal(3, sut.Volume);
    }
    
    [Fact]
    public void VolumeUp_WhenAtMaxVolume_StopsIncreasingVolume()
    {
        var sut = new LGTelevision();
        
        sut.TurnOn();
        for (int i = 0; i < 25; i++)
            sut.VolumeUp();
        sut.VolumeUp();
        
        Assert.Equal(50, sut.Volume);
    }

    [Fact]
    public void VolumeDown_WhenAtZeroVolume_StopsDecreasingVolume()
    {
        var sut = new LGTelevision();
        
        sut.TurnOn();
        sut.VolumeDown();
        
        Assert.Equal(0, sut.Volume);
    }
    
    [Fact]
    public void TurnOff_WhenOn_DoesNotChangeVolume()
    {
        var sut = new LGTelevision();
        
        sut.TurnOn();
        sut.VolumeUp();
        sut.TurnOff();

        Assert.Equal(3, sut.Volume);
    }

    [Fact]
    public void VolumeUp_WhenOff_IncreasesVolume()
    {
        var sut = new LGTelevision();
        
        sut.TurnOn();
        sut.VolumeUp(); // volume = 3
        sut.VolumeUp(); // volume = 6
        sut.TurnOff();
        sut.VolumeUp(); // volume = 9
        
        Assert.False(sut.IsOn);
        Assert.Equal(9, sut.Volume);
    }
    
    [Fact]
    public void VolumeDown_WhenOff_DecreasesVolume()
    {
        var sut = new LGTelevision();
        
        sut.TurnOn();
        sut.VolumeUp(); // volume = 3
        sut.VolumeUp(); // volume = 6
        sut.TurnOff();
        sut.VolumeDown(); // volume = 3
        
        Assert.False(sut.IsOn);
        Assert.Equal(3, sut.Volume);
    }
}