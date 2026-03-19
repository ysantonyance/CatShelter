using NUnit.Framework;
using Moq;
using CatShelter.Controllers;
using CatShelter.Data;
using CatShelter.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace CatShelterTest.Controllers
{
    // тестове за контролера catcares
    public class CatCaresControllerTests
    {
        private ApplicationDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);

            var breed1 = new Breed { Id = 1, Name = "Siamese", Description = "Siamese cat" };
            var breed2 = new Breed { Id = 2, Name = "Persian", Description = "Persian cat" };
            context.Breed.AddRange(breed1, breed2);

            var cat1 = new Cat { Id = 1, Name = "Milo", Description = "Cute Milo", Img = "milo.jpg", Breed = breed1 };
            var cat2 = new Cat { Id = 2, Name = "Luna", Description = "Lovely Luna", Img = "luna.jpg", Breed = breed2 };
            context.Cat.AddRange(cat1, cat2);

            var user1 = new ApplicationUser { Id = "user1", Email = "user1@test.com" };
            var user2 = new ApplicationUser { Id = "user2", Email = "user2@test.com" };
            context.Users.AddRange(user1, user2);

            var care1 = new Care { Id = 1, CareName = "Vaccination", Description = "Routine vaccination for cats" };
            var care2 = new Care { Id = 2, CareName = "Grooming", Description = "Regular grooming session" };
            context.Care.AddRange(care1, care2);

            context.SaveChanges();
            return context;
        }

        private ClaimsPrincipal GetUser(string userId = "user1", string role = "")
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };
            if (!string.IsNullOrEmpty(role))
                claims.Add(new Claim(ClaimTypes.Role, role));
            return new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));
        }

        private CatCaresController CreateController(ApplicationDbContext context, string userId = "user1", string role = "")
        {
            var controller = new CatCaresController(context);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = GetUser(userId, role)
                }
            };
            return controller;
        }
        // index връща всички записи за грижи
        [Test]
        public async Task Index_ReturnsAllCatCares()
        {
            var context = GetDbContext();
            context.CatCare.Add(new CatCare { Id = 1, CatId = 1, CareId = 1, UserId = "user1", Price = 50 });
            context.CatCare.Add(new CatCare { Id = 2, CatId = 2, CareId = 2, UserId = "user1", Price = 30 });
            context.SaveChanges();

            var controller = CreateController(context);
            var result = await controller.Index() as ViewResult;
            var model = result.Model as List<CatCare>;

            Assert.IsNotNull(model);
            Assert.AreEqual(2, model.Count);
        }
        // details с null id връща notfound
        [Test]
        public async Task Details_NullId_ReturnsNotFound()
        {
            var controller = CreateController(GetDbContext());
            var result = await controller.Details(null);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }
        // details с валиден id връща view с пълни данни
        [Test]
        public async Task Details_ValidId_ReturnsView()
        {
            var context = GetDbContext();
            context.CatCare.Add(new CatCare { Id = 1, CatId = 1, CareId = 1, UserId = "user1", Price = 50 });
            context.SaveChanges();

            var controller = CreateController(context);
            var result = await controller.Details(1) as ViewResult;
            var model = result.Model as CatCare;

            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.Id);
            Assert.IsNotNull(model.Cat);
            Assert.IsNotNull(model.Care);
            Assert.IsNotNull(model.User);
        }
        // create post с валиден модел създава запис и редиректва към index
        [Test]
        public async Task CreatePost_ValidModel_RedirectsToIndex()
        {
            var context = GetDbContext();
            var controller = CreateController(context);

            var catCare = new CatCare { CatId = 1, CareId = 1, Price = 50 };
            var result = await controller.Create(catCare) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);

            var saved = context.CatCare.FirstOrDefault();
            Assert.IsNotNull(saved);
            Assert.AreEqual("user1", saved.UserId);
            Assert.IsFalse(saved.IsSatisfied);
        }
        // delete confirmed изтрива запис и редиректва към index
        [Test]
        public async Task DeleteConfirmed_RemovesCatCareAndRedirects()
        {
            var context = GetDbContext();
            context.CatCare.Add(new CatCare { Id = 1, CatId = 1, CareId = 1, UserId = "user1", Price = 50 });
            context.SaveChanges();

            var controller = CreateController(context);
            var result = await controller.DeleteConfirmed(1) as RedirectToActionResult;

            Assert.AreEqual("Index", result.ActionName);
            Assert.IsFalse(context.CatCare.Any(cc => cc.Id == 1));
        }
        // edit с невалиден id връща notfound
        [Test]
        public async Task Edit_InvalidId_ReturnsNotFound()
        {
            var controller = CreateController(GetDbContext());
            var result = await controller.Edit(999);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }
    }
}