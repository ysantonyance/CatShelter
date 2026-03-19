using CatShelter.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using NUnit.Framework;

namespace CatShelterTest.Models
{
    // тестове за модела care
    public class CareTest
    {
        // валиден модел трябва да мине валидация
        [Test]
        public void Care_ShouldBeValid_WhenAllFieldsAreProvided()
        {
            var care = new Care
            {
                CareName = "Food",
                Description = "Food donation"
            };

            var context = new ValidationContext(care);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(care, context, results, true);

            Assert.IsTrue(isValid);
        }
        // липсващо име на грижа трябва да върне грешка
        [Test]
        public void Care_ShouldFail_WhenCareNameIsMissing()
        {
            var care = new Care
            {
                Description = "Food donation"
            };

            var context = new ValidationContext(care);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(care, context, results, true);

            Assert.IsFalse(isValid);
        }
        // липсващо описание трябва да върне грешка
        [Test]
        public void Care_ShouldFail_WhenDescriptionIsMissing()
        {
            var care = new Care
            {
                CareName = "Food"
            };

            var context = new ValidationContext(care);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(care, context, results, true);

            Assert.IsFalse(isValid);
        }
        // всички липсващи задължителни полета трябва да върнат грешки
        [Test]
        public void Care_ShouldFail_WhenAllRequiredFieldsMissing()
        {
            var care = new Care();
            var context = new ValidationContext(care);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(care, context, results, true);

            Assert.IsFalse(isValid);
            Assert.AreEqual(2, results.Count);
            Assert.That(results, Has.Exactly(1).Matches<ValidationResult>(r => r.ErrorMessage.Contains("CareName")));
            Assert.That(results, Has.Exactly(1).Matches<ValidationResult>(r => r.ErrorMessage.Contains("Description")));
        }
    }
}
