using Xunit;
using FluentAssertions;
using FishingSpot.PWA.Services;

namespace FishingSpot.Tests.Services
{
    public class ToastServiceTests
    {
        private readonly ToastService _toastService;
        private ToastMessage? _lastToastShown;

        public ToastServiceTests()
        {
            _toastService = new ToastService();
            _toastService.OnShow += (toast) => _lastToastShown = toast;
        }

        [Fact]
        public void ShowSuccess_ShouldTriggerOnShowEvent()
        {
            // Act
            _toastService.ShowSuccess("Test message");

            // Assert
            _lastToastShown.Should().NotBeNull();
            _lastToastShown!.Message.Should().Be("Test message");
            _lastToastShown.Type.Should().Be(ToastType.Success);
            _lastToastShown.DurationMs.Should().Be(3000);
        }

        [Fact]
        public void ShowError_ShouldHaveLongerDuration()
        {
            // Act
            _toastService.ShowError("Error message");

            // Assert
            _lastToastShown.Should().NotBeNull();
            _lastToastShown!.Type.Should().Be(ToastType.Error);
            _lastToastShown.DurationMs.Should().Be(5000);
        }

        [Fact]
        public void ShowWarning_ShouldUseWarningType()
        {
            // Act
            _toastService.ShowWarning("Warning message");

            // Assert
            _lastToastShown.Should().NotBeNull();
            _lastToastShown!.Type.Should().Be(ToastType.Warning);
        }

        [Fact]
        public void ShowInfo_ShouldUseInfoType()
        {
            // Act
            _toastService.ShowInfo("Info message");

            // Assert
            _lastToastShown.Should().NotBeNull();
            _lastToastShown!.Type.Should().Be(ToastType.Info);
        }

        [Fact]
        public void ToastMessage_ShouldHaveUniqueId()
        {
            // Act
            _toastService.ShowSuccess("Message 1");
            var id1 = _lastToastShown!.Id;

            _toastService.ShowSuccess("Message 2");
            var id2 = _lastToastShown!.Id;

            // Assert
            id1.Should().NotBe(id2);
        }

        [Fact]
        public void Hide_ShouldTriggerOnHideEvent()
        {
            // Arrange
            string? hiddenId = null;
            _toastService.OnHide += (id) => hiddenId = id;

            // Act
            _toastService.Hide("test-id");

            // Assert
            hiddenId.Should().Be("test-id");
        }
    }
}
