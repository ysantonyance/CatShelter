using NUnit.Framework;
using Moq;
using CatShelter.Controllers;
using CatShelter.Data;
using CatShelter.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace CatShelterTest.Controllers
{
    public class AdoptionsControllerTests
    {
        private ApplicationDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);

            context.Cat.AddRange(
                new Cat { Id = 1, Name = "Milo", IsAdopted = false, Description = "Cat Milo", Img = "milo.jpg" },
                new Cat { Id = 2, Name = "Luna", IsAdopted = false, Description = "Cat Luna", Img = "luna.jpg" }
            );

            context.SaveChanges();
            return context;
        }

        private UserManager<ApplicationUser> GetUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            return new UserManager<ApplicationUser>(
                store.Object, null, null, null, null, null, null, null, null);
        }

        private ClaimsPrincipal GetUser(string userId = "user1", string role = "")
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };
            if (!string.IsNullOrEmpty(role))
                claims.Add(new Claim(ClaimTypes.Role, role));
            return new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));
        }

        private AdoptionsController CreateController(ApplicationDbContext context, string userId = "user1", string role = "")
        {
            var controller = new AdoptionsController(context, GetUserManager());
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = GetUser(userId, role)
                }
            };
            return controller;
        }

        [Test]
        public async Task Index_ShouldReturnViewForRegularUser()
        {
            var context = GetDbContext();

            context.Users.Add(new ApplicationUser { Id = "user1", Email = "user1@test.com" });
            context.Users.Add(new ApplicationUser { Id = "user2", Email = "user2@test.com" });

            context.Adoption.AddRange(
                new Adoption { Id = 1, CatId = 1, UserId = "user1", Status = ApplicationStatus.Pending },
                new Adoption { Id = 2, CatId = 2, UserId = "user2", Status = ApplicationStatus.Pending }
            );
            context.SaveChanges();

            var controller = CreateController(context, "user1");

            var result = await controller.Index() as ViewResult;
            var model = result.Model as List<Adoption>;

            Assert.IsNotNull(result);
            Assert.AreEqual(1, model.Count);
            Assert.AreEqual("user1", model[0].UserId);
        }

        [Test]
        public async Task Index_ShouldReturnViewForAdmin()
        {
            var context = GetDbContext();

            context.Users.Add(new ApplicationUser { Id = "admin1", Email = "admin@test.com" });
            context.Adoption.AddRange(
                new Adoption { Id = 1, CatId = 1, UserId = "admin1", Status = ApplicationStatus.Pending },
                new Adoption { Id = 2, CatId = 2, UserId = "admin1", Status = ApplicationStatus.Pending }
            );
            context.SaveChanges();

            var controller = CreateController(context, "admin1", "Admin");

            var result = await controller.Index() as ViewResult;
            var model = result.Model as List<Adoption>;

            Assert.IsNotNull(model);
            Assert.AreEqual(2, model.Count);
        }

        [Test]
        public async Task Details_NullId_ReturnsNotFound()
        {
            var controller = CreateController(GetDbContext());
            var result = await controller.Details(null);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Details_ValidId_ReturnsView()
        {
            var context = GetDbContext();

            var user = new ApplicationUser { Id = "user1", Email = "user1@test.com" };
            context.Users.Add(user);
            var cat = context.Cat.Find(1);

            var adoption = new Adoption
            {
                Id = 1,
                CatId = cat.Id,
                Cat = cat,
                UserId = user.Id,
                User = user,
                Status = ApplicationStatus.Pending
            };
            context.Adoption.Add(adoption);
            context.SaveChanges();

            var controller = CreateController(context, "user1");
            var result = await controller.Details(1) as ViewResult;
            var model = result.Model as Adoption;

            Assert.IsNotNull(result);
            Assert.AreEqual(1, model.Id);
            Assert.IsNotNull(model.Cat);
            Assert.IsNotNull(model.User);
        }

        [Test]
        public async Task CreatePost_ValidModel_RedirectsToIndex()
        {
            var context = GetDbContext();
            var controller = CreateController(context);

            var adoption = new Adoption { CatId = 1 };
            var result = await controller.Create(adoption) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);

            var saved = context.Adoption.First();
            Assert.AreEqual(ApplicationStatus.Pending, saved.Status);
            Assert.AreEqual("user1", saved.UserId);
        }

        [Test]
        public async Task CreatePost_InvalidModel_ReturnsView()
        {
            var context = GetDbContext();
            var controller = CreateController(context);

            controller.ModelState.AddModelError("error", "error");

            var result = await controller.Create(new Adoption()) as ViewResult;

            Assert.IsNotNull(result);
        }

        [Test]
        public async Task Approve_ValidId_SetsApprovedAndCatIsAdopted()
        {
            var context = GetDbContext();
            context.Adoption.Add(new Adoption { Id = 1, CatId = 1, UserId = "user1", Status = ApplicationStatus.Pending });
            context.SaveChanges();

            var controller = CreateController(context);
            var result = await controller.Approve(1) as RedirectToActionResult;

            var updated = context.Adoption.Find(1);
            var cat = context.Cat.Find(1);

            Assert.AreEqual(ApplicationStatus.Approved, updated.Status);
            Assert.IsTrue(cat.IsAdopted);
            Assert.AreEqual("Index", result.ActionName);
        }

        [Test]
        public async Task Approve_InvalidId_ReturnsNotFound()
        {
            var controller = CreateController(GetDbContext());
            var result = await controller.Approve(999);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Reject_ValidId_SetsDenied()
        {
            var context = GetDbContext();
            context.Adoption.Add(new Adoption { Id = 2, CatId = 2, UserId = "user1", Status = ApplicationStatus.Pending });
            context.SaveChanges();

            var controller = CreateController(context);
            await controller.Reject(2);

            var updated = context.Adoption.Find(2);
            Assert.AreEqual(ApplicationStatus.Denied, updated.Status);
        }
    }
}