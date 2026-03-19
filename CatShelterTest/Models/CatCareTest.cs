using CatShelter.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using NUnit.Framework;

namespace CatShelterTest.Models
{
    // тестове за модела catcare
    public class CatCareTest
    {
        // създаване на валиден обект catcare
        private CatCare GetValidCatCare()
        {
            return new CatCare
            {
                CatId = 1,
                CareId = 1,
                UserId = "user1",
                Price = 10m,
                IsSatisfied = false
            };
        }
        // валиден модел трябва да мине валидация
        [Test]
        public void CatCare_ShouldBeValid_WhenAllFieldsProvided()
        {
            var catCare = GetValidCatCare();

            var context = new ValidationContext(catCare);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(catCare, context, results, true);

            Assert.IsTrue(isValid);
            Assert.IsEmpty(results);
        }
        // catid = 0 трябва да върне грешка
        [Test]
        public void CatCare_ShouldFail_WhenCatIdIsZero()
        {
            var catCare = GetValidCatCare();
            catCare.CatId = 0;

            var context = new ValidationContext(catCare);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(catCare, context, results, true);

            Assert.IsFalse(isValid);
            Assert.IsNotEmpty(results);
        }
        // careid = 0 трябва да върне грешка
        [Test]
        public void CatCare_ShouldFail_WhenCareIdIsZero()
        {
            var catCare = GetValidCatCare();
            catCare.CareId = 0;

            var context = new ValidationContext(catCare);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(catCare, context, results, true);

            Assert.IsFalse(isValid);
            Assert.IsNotEmpty(results);
        }
        // липсващ userid трябва да върне грешка
        [Test]
        public void CatCare_ShouldFail_WhenUserIdMissing()
        {
            var catCare = GetValidCatCare();
            catCare.UserId = null;

            var context = new ValidationContext(catCare);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(catCare, context, results, true);

            Assert.IsFalse(isValid);
            Assert.IsNotEmpty(results);
        }
        // отрицателна цена трябва да върне грешка
        [Test]
        public void CatCare_ShouldFail_WhenPriceIsNegative()
        {
            var catCare = GetValidCatCare();
            catCare.Price = -5m;

            var context = new ValidationContext(catCare);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(catCare, context, results, true);

            Assert.IsFalse(isValid);
            Assert.IsNotEmpty(results);
        }
    }
}
