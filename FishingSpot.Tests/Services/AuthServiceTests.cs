using Xunit;
using Moq;
using FishingSpot.PWA.Services;
using FishingSpot.PWA.Models.Auth;
using Microsoft.JSInterop;
using Microsoft.Extensions.Configuration;
using FluentAssertions;
using System.Net;

namespace FishingSpot.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<HttpClient> _httpClientMock;
        private readonly Mock<IJSRuntime> _jsRuntimeMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _httpClientMock = new Mock<HttpClient>();
            _jsRuntimeMock = new Mock<IJSRuntime>();
            _configurationMock = new Mock<IConfiguration>();

            _configurationMock.Setup(c => c["Supabase:Url"]).Returns("https://test.supabase.co");
            _configurationMock.Setup(c => c["Supabase:Key"]).Returns("test-key");

            _authService = new AuthService(
                _httpClientMock.Object,
                _configurationMock.Object,
                _jsRuntimeMock.Object
            );
        }

        [Fact]
        public void CurrentUser_ShouldBeNull_WhenNotAuthenticated()
        {
            // Act
            var user = _authService.CurrentUser;

            // Assert
            user.Should().BeNull();
        }

        [Fact]
        public void IsAuthenticated_ShouldBeFalse_WhenNoToken()
        {
            // Act
            var isAuthenticated = _authService.IsAuthenticated;

            // Assert
            isAuthenticated.Should().BeFalse();
        }

        [Fact]
        public async Task InitializeAsync_ShouldLoadUserFromLocalStorage()
        {
            // Arrange
            var expectedToken = "test-token";
            var expectedUser = new User { Id = "123", Email = "test@test.com" };

            _jsRuntimeMock
                .Setup(js => js.InvokeAsync<string>("localStorage.getItem", "supabase_token", default))
                .ReturnsAsync(expectedToken);

            // Act
            await _authService.InitializeAsync();

            // Assert
            _jsRuntimeMock.Verify(
                js => js.InvokeAsync<string>("localStorage.getItem", "supabase_token", default),
                Times.Once
            );
        }
    }
}
