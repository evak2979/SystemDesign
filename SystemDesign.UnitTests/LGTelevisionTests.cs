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
        Assert.Equal(2, sut.Volume);
    }
    
    [Fact]
    public void VolumeUp_WhenOn_IncreasesVolumeByTwo()
    {
        var sut = new LGTelevision();

        sut.TurnOn();
        sut.VolumeUp();

        Assert.Equal(2, sut.Volume);
    }
    
    [Fact]
    public void VolumeUp_WhenOn_DecreasesVolumeByTwo()
    {
        var sut = new LGTelevision();

        sut.TurnOn();
        sut.VolumeUp();
        sut.VolumeUp();
        sut.VolumeDown();

        Assert.Equal(2, sut.Volume);
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
    public void TurnOn_WhenOff_ResetsVolume()
    {
        var sut = new LGTelevision();
        
        sut.TurnOn();
        sut.VolumeUp();
        sut.TurnOff();
        sut.TurnOn();

        Assert.Equal(0, sut.Volume);
    }

    [Fact]
    public void VolumeUp_WhenOff_DoNothing()
    {
        var sut = new LGTelevision();
        
        sut.TurnOn();
        sut.VolumeUp(); // volume = 2
        sut.VolumeUp(); // volume = 4
        sut.TurnOff();
        sut.VolumeUp();
        
        Assert.False(sut.IsOn);
        Assert.Equal(4, sut.Volume);
    }
    
    [Fact]
    public void VolumeDown_WhenOff_DoNothing()
    {
        var sut = new LGTelevision();
        
        sut.TurnOn();
        sut.VolumeUp(); // volume = 2
        sut.VolumeUp(); // volume = 4
        sut.TurnOff();
        sut.VolumeDown();
        
        Assert.False(sut.IsOn);
        Assert.Equal(4, sut.Volume);
    }
}