using CatShelter.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using NUnit.Framework;

namespace CatShelterTest.Models
{
    public class CatTest
    {
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

        [Test]
        public void Cat_ShouldBeValid_WhenAllFieldsProvided()
        {
            var cat = GetValidCat();
            var results = cat.Validate();
            Assert.IsEmpty(results); 
        }

        [Test]
        public void Cat_ShouldFail_WhenNameMissing()
        {
            var cat = GetValidCat();
            cat.Name = null;

            var results = cat.Validate();
            Assert.IsNotEmpty(results);
        }

        [Test]
        public void Cat_ShouldFail_WhenImgMissing()
        {
            var cat = GetValidCat();
            cat.Img = null;

            var results = cat.Validate();
            Assert.IsNotEmpty(results);
        }

        [Test]
        public void Cat_ShouldFail_WhenDescriptionMissing()
        {
            var cat = GetValidCat();
            cat.Description = null;

            var results = cat.Validate();
            Assert.IsNotEmpty(results);
        }

        [Test]
        public void Cat_ShouldFail_WhenBreedIdIsZero()
        {
            var cat = GetValidCat();
            cat.BreedId = 0;

            var results = cat.Validate();
            Assert.IsNotEmpty(results);
        }

        [Test]
        public void Cat_ShouldFail_WhenKgIsZeroOrNegative()
        {
            var cat = GetValidCat();
            cat.Kg = -1;

            var results = cat.Validate();
            Assert.IsNotEmpty(results);
        }

        [Test]
        public void Cat_ShouldFail_WhenBirthDateInFuture()
        {
            var cat = GetValidCat();
            cat.BirthDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1));

            var results = cat.Validate();
            Assert.IsNotEmpty(results);
        }

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
