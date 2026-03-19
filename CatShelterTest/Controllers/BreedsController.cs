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
    public class BreedsControllerTests
    {
        private ApplicationDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);

            context.Breed.AddRange(
                new Breed { Id = 1, Name = "Siamese", Description = "Siamese cat" },
                new Breed { Id = 2, Name = "Persian", Description = "Persian cat" }
            );
            context.SaveChanges();
            return context;
        }

        private ClaimsPrincipal GetUser(string role = "")
        {
            var claims = new List<Claim>();
            if (!string.IsNullOrEmpty(role))
                claims.Add(new Claim(ClaimTypes.Role, role));

            return new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));
        }

        private BreedsController CreateController(ApplicationDbContext context, string role = "")
        {
            var controller = new BreedsController(context);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = GetUser(role)
                }
            };
            return controller;
        }

        [Test]
        public async Task Index_ReturnsAllBreeds()
        {
            var context = GetDbContext();
            var controller = CreateController(context);

            var result = await controller.Index() as ViewResult;
            var model = result.Model as List<Breed>;

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
            var controller = CreateController(context);

            var result = await controller.Details(1) as ViewResult;
            var model = result.Model as Breed;

            Assert.IsNotNull(result);
            Assert.AreEqual(1, model.Id);
            Assert.AreEqual("Siamese", model.Name);
        }

        [Test]
        public async Task CreatePost_ValidModel_RedirectsToIndex()
        {
            var context = GetDbContext();
            var controller = CreateController(context, "Admin");

            var breed = new Breed { Name = "Maine Coon", Description = "Maine Coon cat" };
            var result = await controller.Create(breed) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);

            var saved = context.Breed.FirstOrDefault(b => b.Name == "Maine Coon");
            Assert.IsNotNull(saved);
        }

        [Test]
        public async Task Edit_ValidId_ReturnsView()
        {
            var context = GetDbContext();
            var controller = CreateController(context, "Admin");

            var result = await controller.Edit(1) as ViewResult;
            var model = result.Model as Breed;

            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.Id);
        }

        [Test]
        public async Task Edit_InvalidId_ReturnsNotFound()
        {
            var controller = CreateController(GetDbContext(), "Admin");
            var result = await controller.Edit(999);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Delete_ValidId_ReturnsView()
        {
            var context = GetDbContext();
            var controller = CreateController(context, "Admin");

            var result = await controller.Delete(1) as ViewResult;
            var model = result.Model as Breed;

            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.Id);
        }

        [Test]
        public async Task Delete_InvalidId_ReturnsNotFound()
        {
            var controller = CreateController(GetDbContext(), "Admin");
            var result = await controller.Delete(999);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task DeleteConfirmed_RemovesBreedAndRedirects()
        {
            var context = GetDbContext();
            var controller = CreateController(context, "Admin");

            var result = await controller.DeleteConfirmed(1) as RedirectToActionResult;

            Assert.AreEqual("Index", result.ActionName);
            Assert.IsFalse(context.Breed.Any(b => b.Id == 1));
        }
    }
}