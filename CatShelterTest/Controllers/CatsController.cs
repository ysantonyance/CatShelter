using NUnit.Framework;
using Moq;
using CatShelter.Controllers;
using CatShelter.Data;
using CatShelter.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Collections.Generic;

namespace CatShelterTest.Controllers
{
    public class CatsControllerTests
    {
        private ApplicationDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);

            var breed = new Breed
            {
                Id = 1,
                Name = "Siamese",
                Description = "Siamese cats are affectionate."
            };
            context.Breed.Add(breed);

            var cat1 = new Cat
            {
                Id = 1,
                Name = "Milo",
                BirthDate = new DateOnly(2021, 5, 12),
                Kg = 4.5m,
                Img = "milo.jpg",
                Description = "Friendly cat",
                BreedId = breed.Id,
                IsAdopted = false,
                IsHealthy = true,
                Gender = Gender.Male  // Added missing Gender
            };

            var cat2 = new Cat
            {
                Id = 2,
                Name = "Luna",
                BirthDate = new DateOnly(2022, 3, 7),
                Kg = 3.2m,
                Img = "luna.jpg",
                Description = "Playful kitten",
                BreedId = breed.Id,
                IsAdopted = false,
                IsHealthy = true,
                Gender = Gender.Female  // Added missing Gender
            };

            context.Cat.AddRange(cat1, cat2);
            context.SaveChanges();

            return context;
        }

        private ClaimsPrincipal GetUser(string userId = "user1")
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };
            return new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));
        }

        private CatsController CreateController(ApplicationDbContext context, string userId = "user1")
        {
            var controller = new CatsController(context);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = GetUser(userId)
                }
            };
            return controller;
        }

        // index връща всички котки
        [Test]
        public async Task Index_ReturnsAllCats()
        {
            var context = GetDbContext();
            var controller = CreateController(context);

            var result = await controller.Index() as ViewResult;
            var model = result.Model as List<Cat>;

            Assert.IsNotNull(result);
            Assert.AreEqual(2, model.Count);
        }

        // details с валиден id връща котка с данни
        [Test]
        public async Task Details_ValidId_ReturnsCat()
        {
            var context = GetDbContext();
            var controller = CreateController(context);

            var result = await controller.Details(1) as ViewResult;
            var model = result.Model as Cat;

            Assert.IsNotNull(result);
            Assert.AreEqual(1, model.Id);
            Assert.AreEqual("Milo", model.Name);
            Assert.IsNotNull(model.Breed);
        }

        // details с невалиден id връща notfound
        [Test]
        public async Task Details_InvalidId_ReturnsNotFound()
        {
            var context = GetDbContext();
            var controller = CreateController(context);

            var result = await controller.Details(999);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        // create post с валиден модел добавя котка и редиректва
        [Test]
        public async Task CreatePost_ValidModel_RedirectsToIndex()
        {
            var context = GetDbContext();
            var controller = CreateController(context);

            var newCat = new Cat
            {
                Name = "Leo",
                BirthDate = DateOnly.FromDateTime(DateTime.Now.AddYears(-1)),
                Kg = 3.5m,
                Img = "leo.jpg",
                Description = "Cute kitten",
                BreedId = 1,
                IsAdopted = false,
                IsHealthy = true,
                Gender = Gender.Male  // Added missing Gender
            };

            var result = await controller.Create(newCat, null) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
            Assert.AreEqual(3, context.Cat.Count());
        }

        // edit с невалиден id връща notfound
        [Test]  // Changed from [TestMethod] to [Test] for NUnit
        public async Task Edit_InvalidId_ReturnsNotFound()
        {
            // Use your helper methods instead of creating new ones
            var context = GetDbContext();
            var controller = CreateController(context);

            var cat = new Cat
            {
                Id = 999,
                Name = "Fake",
                BreedId = 1,
                Gender = Gender.Male,
                BirthDate = new DateOnly(2020, 1, 1),
                Kg = 4.5m,
                Img = "fake.jpg",
                Description = "Fake cat",
                IsAdopted = false,
                IsHealthy = true
            };

            // Act
            var result = await controller.Edit(999, cat, null);

            // Assert - use IsInstanceOf for NUnit
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        // delete confirmed изтрива котка и редиректва към index
        [Test]
        public async Task DeleteConfirmed_RemovesCat()
        {
            var context = GetDbContext();
            var controller = CreateController(context);

            var result = await controller.DeleteConfirmed(1) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
            Assert.AreEqual(1, context.Cat.Count());
        }
    }
}