using ConsultantDashboard.Core.DTOs;
using ConsultantDashboard.Services.Implement;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using System.Threading.Tasks;
using ConsultantDashboard.Core.Entities;

public class AuthServiceTests
{
    [Fact]
    public async Task RegisterAsync_ReturnsUserExists_WhenUserWithEmailAlreadyExists()
    {
        //Arrange start

        // Step 1: Mock UserManager
        var mockUserManager = MockUserManager();
        mockUserManager.Setup(x => x.FindByEmailAsync("test@example.com"))
                       .ReturnsAsync(new ApplicationUser()); // simulate user exists

        // Step 2: Mock SignInManager and Configuration
        var mockSignInManager = MockSignInManager();
        var mockConfig = new Mock<IConfiguration>();

        // Step 3: Create instance of AuthService
        var authService = new AuthService(mockUserManager.Object, mockSignInManager.Object, mockConfig.Object);
        
        // Step 4: Fake input
        var request = new RegisterConsultantDTOs { Email = "test@example.com" };

        //Arrange end

        // Act start

        // Step 5: Call the method
        var result = await authService.RegisterAsync(request);

        // Act end

        // Step 6: Assert
        Assert.False(result.Success);
        Assert.Equal("User already exists", result.Message);
    }

    // Helper to mock UserManager
    private Mock<UserManager<ApplicationUser>> MockUserManager()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        return new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
    }

    // Helper to mock SignInManager
    private Mock<SignInManager<ApplicationUser>> MockSignInManager()
    {
        var userManager = MockUserManager().Object;
        var contextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
        var claimsFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
        return new Mock<SignInManager<ApplicationUser>>(userManager, contextAccessor.Object, claimsFactory.Object, null, null, null, null);
    }
}
