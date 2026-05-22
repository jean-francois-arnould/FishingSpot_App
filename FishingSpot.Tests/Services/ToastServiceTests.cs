using FishingSpot.PWA.Services;

namespace FishingSpot.Tests.Services;

public class ToastServiceTests
{
    [Fact]
    public void ShowSuccess_RaisesToastWithExpectedTypeAndMessage()
    {
        var service = new ToastService();
        ToastMessage? shownToast = null;

        service.OnShow += toast => shownToast = toast;

        service.ShowSuccess("Saved", 1234);

        Assert.NotNull(shownToast);
        Assert.Equal("Saved", shownToast.Message);
        Assert.Equal(ToastType.Success, shownToast.Type);
        Assert.Equal(1234, shownToast.DurationMs);
    }

    [Fact]
    public void Hide_RaisesHideEventWithId()
    {
        var service = new ToastService();
        string? hiddenId = null;

        service.OnHide += id => hiddenId = id;

        service.Hide("toast-1");

        Assert.Equal("toast-1", hiddenId);
    }
}
