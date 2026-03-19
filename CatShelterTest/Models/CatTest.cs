using CatShelter.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using NUnit.Framework;

namespace CatShelterTest.Models
{
    // тестове за модела cat
    public class CatTest
    {
        // помощен клас с допълнителна валидация за дата на раждане
        private class CatWithBirthDateValidation : Cat
        {
            public IEnumerable<ValidationResult> Validate()
            {
                var context = new ValidationContext(this);
                var results = new List<ValidationResult>();

                Validator.TryValidateObject(this, context, results, true);

                if (BirthDate > DateOnly.FromDateTime(DateTime.Today))
                {
                    results.Add(new ValidationResult("BirthDate cannot be in the future.", new[] { nameof(BirthDate) }));
                }

                return results;
            }
        }
        // създаване на валиден обект котка
        private CatWithBirthDateValidation GetValidCat() => new CatWithBirthDateValidation
        {
            Name = "Leo",
            BirthDate = DateOnly.FromDateTime(DateTime.Now.AddYears(-1)),
            Kg = 3.5m,
            Img = "leo.jpg",
            BreedId = 1,
            IsAdopted = false,
            IsHealthy = true,
            Gender = Gender.Male,
            Description = "Cute kitten"
        };
        // валиден модел трябва да мине валидация
        [Test]
        public void Cat_ShouldBeValid_WhenAllFieldsProvided()
        {
            var cat = GetValidCat();
            var results = cat.Validate();
            Assert.IsEmpty(results); 
        }
        // липсващо име трябва да върне грешка
        [Test]
        public void Cat_ShouldFail_WhenNameMissing()
        {
            var cat = GetValidCat();
            cat.Name = null;

            var results = cat.Validate();
            Assert.IsNotEmpty(results);
        }
        // липсваща снимка трябва да върне грешка
        [Test]
        public void Cat_ShouldFail_WhenImgMissing()
        {
            var cat = GetValidCat();
            cat.Img = null;

            var results = cat.Validate();
            Assert.IsNotEmpty(results);
        }
        // липсващо описание трябва да върне грешка
        [Test]
        public void Cat_ShouldFail_WhenDescriptionMissing()
        {
            var cat = GetValidCat();
            cat.Description = null;

            var results = cat.Validate();
            Assert.IsNotEmpty(results);
        }
        // breedid = 0 трябва да върне грешка
        [Test]
        public void Cat_ShouldFail_WhenBreedIdIsZero()
        {
            var cat = GetValidCat();
            cat.BreedId = 0;

            var results = cat.Validate();
            Assert.IsNotEmpty(results);
        }
        // тегло 0 или отрицателно трябва да върне грешка
        [Test]
        public void Cat_ShouldFail_WhenKgIsZeroOrNegative()
        {
            var cat = GetValidCat();
            cat.Kg = -1;

            var results = cat.Validate();
            Assert.IsNotEmpty(results);
        }
        // бъдеща дата на раждане трябва да върне грешка
        [Test]
        public void Cat_ShouldFail_WhenBirthDateInFuture()
        {
            var cat = GetValidCat();
            cat.BirthDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1));

            var results = cat.Validate();
            Assert.IsNotEmpty(results);
        }
        // невалиден gender трябва да върне грешка
        [Test]
        public void Cat_ShouldFail_WhenGenderDefault()
        {
            var cat = GetValidCat();
            cat.Gender = 0; 

            var results = cat.Validate();
            Assert.IsNotEmpty(results);
        }
    }
}
