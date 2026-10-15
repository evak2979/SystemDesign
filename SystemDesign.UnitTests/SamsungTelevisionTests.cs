using SystemDesign.Api.Implementations;
using Xunit;

namespace SystemDesign.UnitTests;

public class SamsungTelevisionTests
{
    [Fact]
    public void TurnOn_WhenOff_TurnsTheTelevisionOn()
    {
        var sut = new SamsungTelevision();

        sut.TurnOn();

        Assert.True(sut.IsOn);
    }

    [Fact]
    public void TurnOff_WhenOn_TurnsTheTelevisionOff()
    {
        var sut = new SamsungTelevision();
        
        sut.TurnOn();
        sut.TurnOff();
        
        Assert.False(sut.IsOn);
    }

    [Fact]
    public void TurnOn_WhenAlreadyOn_RemainsOnAndVolumeUnaffected()
    {
        var sut = new SamsungTelevision();

        sut.TurnOn();
        sut.VolumeUp();
        sut.TurnOn();

        Assert.True(sut.IsOn);
        Assert.Equal(11, sut.Volume);
    }
    
    [Fact]
    public void VolumeUp_WhenOn_IncreasesVolumeByOne()
    {
        var sut = new SamsungTelevision();

        sut.TurnOn();
        sut.VolumeUp();

        Assert.Equal(11, sut.Volume);
    }
    
    [Fact]
    public void VolumeUp_WhenAtMaxVolume_StopsIncreasingVolume()
    {
        var sut = new SamsungTelevision();
        
        for (int i = 0; i < 90; i++)
            sut.VolumeUp();
        sut.VolumeUp();
        
        Assert.Equal(50, sut.Volume);
    }

    [Fact]
    public void VolumeDown_WhenAtZeroVolume_StopsDecreasingVolume()
    {
        var sut = new SamsungTelevision();
        
        for (int i = 0; i < 10; i++)
            sut.VolumeDown();
        sut.VolumeDown();

        Assert.Equal(0, sut.Volume);
    }

    [Fact]
    public void TurnOff_WhenOn_DoesNotChangeVolume()
    {
        var sut = new SamsungTelevision();
        
        sut.TurnOn();
        sut.VolumeUp();
        sut.TurnOff();

        Assert.Equal(11, sut.Volume);
    }
}