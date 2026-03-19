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
                IsHealthy = true
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
                IsHealthy = true
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

        [Test]
        public async Task Details_InvalidId_ReturnsNotFound()
        {
            var context = GetDbContext();
            var controller = CreateController(context);

            var result = await controller.Details(999);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

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
                IsHealthy = true
            };

            var result = await controller.Create(newCat) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
            Assert.AreEqual(3, context.Cat.Count());
        }

        [Test]
        public async Task Edit_InvalidId_ReturnsNotFound()
        {
            var context = GetDbContext();
            var controller = CreateController(context);

            var cat = new Cat { Id = 999, Name = "Fake", BreedId = 1, Img = "fake.jpg", Description = "Fake cat" };
            var result = await controller.Edit(999, cat);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

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