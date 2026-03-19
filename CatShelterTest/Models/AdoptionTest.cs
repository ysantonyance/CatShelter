using CatShelter.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using NUnit.Framework;

namespace CatShelterTest.Models
{
    // тестове за модела adoption
    public class AdoptionTest
    {
        // създаване на валиден обект adoption за тестове
        private Adoption GetValidAdoption()
        {
            return new Adoption
            {
                UserId = "user1",
                CatId = 1,
                AdoptionDate = DateOnly.FromDateTime(DateTime.Now),
                Status = ApplicationStatus.Pending
            };
        }
        // валиден модел трябва да мине валидация
        [Test]
        public void Adoption_ShouldPass_WhenAllFieldsProvided()
        {
            var adoption = GetValidAdoption();

            var context = new ValidationContext(adoption);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(adoption, context, results, true);

            Assert.IsTrue(isValid);
            Assert.IsEmpty(results);
        }
        // липсващ userid трябва да върне грешка
        [Test]
        public void Adoption_ShouldFail_WhenUserIdMissing()
        {
            var adoption = GetValidAdoption();
            adoption.UserId = null;

            var context = new ValidationContext(adoption);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(adoption, context, results, true);

            Assert.IsFalse(isValid);
            Assert.IsNotEmpty(results);
        }
        // catid = 0 трябва да върне грешка
        [Test]
        public void Adoption_ShouldFail_WhenCatIdIsZero()
        {
            var adoption = GetValidAdoption();
            adoption.CatId = 0;

            var context = new ValidationContext(adoption);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(adoption, context, results, true);

            Assert.IsFalse(isValid);
            Assert.IsNotEmpty(results);
        }
        // празна дата трябва да върне грешка
        [Test]
        public void Adoption_ShouldFail_WhenAdoptionDateDefault()
        {
            var adoption = GetValidAdoption();
            adoption.AdoptionDate = default;

            var context = new ValidationContext(adoption);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(adoption, context, results, true);

            Assert.IsFalse(isValid);
            Assert.IsNotEmpty(results);
        }
        // няколко липсващи полета трябва да върнат повече грешки
        [Test]
        public void Adoption_ShouldFail_WhenMultipleFieldsMissing()
        {
            var adoption = new Adoption
            {
                UserId = null,
                CatId = 0,
                AdoptionDate = default,
                Status = 0
            };

            var context = new ValidationContext(adoption);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(adoption, context, results, true);

            Assert.IsFalse(isValid);
            Assert.IsTrue(results.Count >= 3); 
        }
    }
}
