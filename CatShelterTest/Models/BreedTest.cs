using CatShelter.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using NUnit.Framework;

namespace CatShelterTest.Models
{
    // тестове за модела breed
    public class BreedTest
    {
        // липсващо име трябва да върне грешка
        [Test]
        public void Breed_ShouldFail_WhenNameMissing()
        {
            var breed = new Breed
            {
                Name = null,
                Description = "Some description"
            };

            var context = new ValidationContext(breed);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(breed, context, results, true);

            Assert.IsFalse(isValid);
            Assert.That(results, Has.Exactly(1).Matches<ValidationResult>(r => r.ErrorMessage.Contains("Name")));
        }
        // липсващо описание трябва да върне грешка
        [Test]
        public void Breed_ShouldFail_WhenDescriptionMissing()
        {
            var breed = new Breed
            {
                Name = "Persian",
                Description = null
            };

            var context = new ValidationContext(breed);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(breed, context, results, true);

            Assert.IsFalse(isValid);
            Assert.That(results, Has.Exactly(1).Matches<ValidationResult>(r => r.ErrorMessage.Contains("Description")));
        }
        // валиден модел трябва да мине валидация
        [Test]
        public void Breed_ShouldPass_WhenValid()
        {
            var breed = new Breed
            {
                Name = "Persian",
                Description = "Long-haired cat"
            };

            var context = new ValidationContext(breed);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(breed, context, results, true);

            Assert.IsTrue(isValid);
        }
    }
}
