using Bogus;
using EduPortal.Data.Models.Entites;
using EduPortal.Logic.DTOs.IdentityDTOs;
using EduPortal.Logic.Services.UserService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using NSubstitute;

namespace EduPortal.Tests.Unit.ServiceTests;

[TestFixture]
public class UserServiceTests
{
    private UserManager<User> _userManagerSub;
    private SignInManager<User> _signInManagerSub;
    private RoleManager<IdentityRole<Guid>> _roleManagerSub;
    private IUserService _sut;
    private Faker<RegisterDTO> _registerDTOFaker;


    [SetUp]
    public void Setup()
    {
        var userStoreSub = Substitute.For<IUserStore<User>>();
        _userManagerSub = Substitute.For<UserManager<User>>(
            userStoreSub, null, null, null, null, null, null, null, null);

        var contextAccessorSub = Substitute.For<IHttpContextAccessor>();
        var claimsFactorySub = Substitute.For<IUserClaimsPrincipalFactory<User>>();
        _signInManagerSub = Substitute.For<SignInManager<User>>(
            _userManagerSub, contextAccessorSub, claimsFactorySub, null, null, null, null);

        var roleStoreSub = Substitute.For<IRoleStore<IdentityRole<Guid>>>();
        _roleManagerSub = Substitute.For<RoleManager<IdentityRole<Guid>>>(
            roleStoreSub, null, null, null, null);

        _sut = new UserService(_userManagerSub, _signInManagerSub, _roleManagerSub);

        _registerDTOFaker = new Faker<RegisterDTO>()
            .RuleFor(r => r.UserName, f => f.Internet.UserName())
            .RuleFor(r => r.Email, f => f.Internet.Email())
            .RuleFor(r => r.FirstName, f => f.Name.FirstName())
            .RuleFor(r => r.LastName, f => f.Name.LastName())
            .RuleFor(r => r.DateOfBirth, f => f.Date.Past(30, DateTime.Now.AddYears(-18)))
            .RuleFor(r => r.Password, f => f.Internet.Password(8))
            .RuleFor(r => r.ConfirmPassword, (f, r) => r.Password);
    }

    [Category("Unit")]
    [Test]
    public async Task RegisterUserAsync_RegisterWithInvalidParameters_ReturnsFail()
    {
        //Arrange
        RegisterDTO registerDTO = _registerDTOFaker.Generate();

        _userManagerSub.FindByNameAsync(registerDTO.UserName).Returns((new User { UserName = registerDTO.UserName }));

        //Act
        var result = await _sut.RegisterUserAsync(registerDTO, "Student");

        //Assert
        Assert.IsFalse(result.Succeeded);
        Assert.That(result.Errors.First().Code, Is.EqualTo("DuplicateUserName"));

        await _userManagerSub.DidNotReceive().CreateAsync(Arg.Any<User>(), Arg.Any<string>());
    }

    [Category("Unit")]
    [Test]
    public async Task RegisterUserAsync_RegisterWithValidParameters_ReturnsSuccess()
    {
        //Arrange
        RegisterDTO registerDTO = _registerDTOFaker.Generate();
        string role = "Student";

        _userManagerSub.FindByNameAsync(registerDTO.UserName).Returns((User)null);
        _userManagerSub.FindByEmailAsync(registerDTO.Email).Returns((User)null);

        _userManagerSub.CreateAsync(Arg.Any<User>(), registerDTO.Password).Returns(IdentityResult.Success);
        _roleManagerSub.RoleExistsAsync(role).Returns(true);
        _userManagerSub.AddToRoleAsync(Arg.Any<User>(), role).Returns(IdentityResult.Success);

        //Act
        var result = await _sut.RegisterUserAsync(registerDTO, role);

        //Assert
        Assert.IsTrue(result.Succeeded);

        await _userManagerSub.Received(1).CreateAsync(Arg.Any<User>(), registerDTO.Password);
        await _userManagerSub.Received(1).AddToRoleAsync(Arg.Any<User>(), role);

        await _roleManagerSub.DidNotReceive().CreateAsync(Arg.Any<IdentityRole<Guid>>());
    }

    [TearDown]
    public void TearDown()
    {
        _userManagerSub?.Dispose();
        _roleManagerSub?.Dispose();
    }
}
