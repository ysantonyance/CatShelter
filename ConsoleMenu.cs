using CatShelter.Data;
using CatShelter.Models;
using Microsoft.EntityFrameworkCore;
using CatShelter;

public class ConsoleMenu
{
    private readonly ApplicationDbContext _context;
    private readonly bool _isAdmin;

    public ConsoleMenu(ApplicationDbContext context, bool isAdmin = false)
    {
        _context = context;
        _isAdmin = isAdmin;
    }

    public void Start()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("CAT SHELTER");

            if (_isAdmin)
                Console.WriteLine("1. Admin Menu");
            else
                Console.WriteLine("1. User Menu");

            Console.WriteLine("2. Exit");

            string choice = Console.ReadLine();
            if (choice == "1")
            {
                if (_isAdmin)
                    AdminMenu();
                else
                    UserMenu();
            }
            else if (choice == "2")
            {
                return;
            }
        }
    }

    private void UserMenu()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("USER MENU");
            Console.WriteLine("1. List Cats");
            Console.WriteLine("2. View Cat Details");
            Console.WriteLine("3. Create Adoption Request");
            Console.WriteLine("4. Make Donation (Care)");
            Console.WriteLine("5. Back");

            switch (Console.ReadLine())
            {
                case "1": 
                    ListCats(); 
                    break;
                case "2": 
                    ViewCatDetails(); 
                    break;
                case "3": 
                    CreateAdoption(); 
                    break;
                case "4": 
                    CreateCatCare(); 
                    break;
                case "5": 
                    return;
            }
        }
    }

    private void AdminMenu()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("ADMIN MENU");
            Console.WriteLine("1. Cats");
            Console.WriteLine("2. Breeds");
            Console.WriteLine("3. Adoptions");
            Console.WriteLine("4. Cat Care");
            Console.WriteLine("5. Back");

            switch (Console.ReadLine())
            {
                case "1": 
                    CatsMenu(); 
                    break;
                case "2": 
                    BreedsMenu();
                    break;
                case "3": 
                    AdoptionsMenu(); 
                    break;
                case "4": 
                    CatCareMenu(); 
                    break;
                case "5": 
                    return;
            }
        }
    }

    private void CatsMenu()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("CATS");
            Console.WriteLine("1. List Cats");
            Console.WriteLine("2. Create Cat");
            Console.WriteLine("3. Edit Cat");
            Console.WriteLine("4. Delete Cat");
            Console.WriteLine("5. Back");

            switch (Console.ReadLine())
            {
                case "1": 
                    ListCats(); 
                    break;
                case "2": 
                    CreateCat(); 
                    break;
                case "3": 
                    EditCat();
                    break;
                case "4":
                    DeleteCat(); 
                    break;
                case "5":
                    return;
            }
        }
    }

    private void ListCats()
    {
        Console.Clear();
        var cats = _context.Cat.Include(c => c.Breed).ToList();
        foreach (var c in cats)
            Console.WriteLine($"#{c.Id} - {c.Name} - {c.Breed?.Name} - Adopted: {c.IsAdopted}");
        Console.WriteLine("Press any key...");
        Console.ReadKey();
    }

    private void CreateCat()
    {
        Console.Clear();
        Console.WriteLine("CREATE NEW CAT");

        Console.Write("Name: ");
        string name = Console.ReadLine();

        var breeds = _context.Breed.ToList();
        foreach (var b in breeds)
            Console.WriteLine($"{b.Id}: {b.Name}");
        Console.Write("Breed Id: ");
        int breedId = int.Parse(Console.ReadLine());

        Console.Write("Weight (kg): ");
        decimal kg = decimal.Parse(Console.ReadLine());

        Console.Write("Birthdate (yyyy-mm-dd): ");
        DateOnly birthDate = DateOnly.Parse(Console.ReadLine());

        Console.Write("Gender (Male/Female): ");
        Gender gender = Enum.Parse<Gender>(Console.ReadLine(), true);

        Console.Write("Description: ");
        string description = Console.ReadLine();

        Console.Write("Image (path): ");
        string img = Console.ReadLine();

        Console.Write("Is Healthy? (y/n): ");
        bool isHealthy = Console.ReadLine().ToLower() == "y";

        var cat = new Cat
        {
            Name = name,
            BreedId = breedId,
            Kg = kg,
            BirthDate = birthDate,
            Gender = gender,
            Description = description,
            Img = img,
            IsAdopted = false,
            IsHealthy = isHealthy
        };

        _context.Cat.Add(cat);
        _context.SaveChanges();

        Console.WriteLine("Cat created successfully!");
        Console.ReadKey();
    }

    private void EditCat()
    {
        Console.Write("Cat Id to edit: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
            return;

        var cat = _context.Cat.Find(id);
        if (cat == null)
        {
            Console.WriteLine("Cat not found.");
            Console.ReadKey();
            return;
        }

        while (true)
        {
            Console.Clear();
            Console.WriteLine($"Editing Cat: {cat.Name}");
            Console.WriteLine("1. Name");
            Console.WriteLine("2. Breed Id");
            Console.WriteLine("3. Weight (Kg)");
            Console.WriteLine("4. Birth Date (yyyy-mm-dd)");
            Console.WriteLine("5. Image Path");
            Console.WriteLine("6. Description");
            Console.WriteLine("7. Is Adopted");
            Console.WriteLine("8. Is Healthy");
            Console.WriteLine("9. Done (Save Changes)");
            Console.WriteLine("0. Cancel (Exit without saving)");

            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    Console.Write("New Name (leave empty to skip): ");
                    var name = Console.ReadLine();
                    if (!string.IsNullOrEmpty(name)) cat.Name = name;
                    break;
                case "2":
                    Console.Write("New Breed Id (leave empty to skip): ");
                    var breedInput = Console.ReadLine();
                    if (!string.IsNullOrEmpty(breedInput))
                        cat.BreedId = int.Parse(breedInput);
                    break;
                case "3":
                    Console.Write("New Weight (Kg, leave empty to skip): ");
                    var kgInput = Console.ReadLine();
                    if (!string.IsNullOrEmpty(kgInput))
                        cat.Kg = decimal.Parse(kgInput);
                    break;
                case "4":
                    Console.Write("New Birth Date (yyyy-mm-dd, leave empty to skip): ");
                    var dateInput = Console.ReadLine();
                    if (DateOnly.TryParse(dateInput, out var date))
                        cat.BirthDate = date;
                    break;
                case "5":
                    Console.Write("New Image Path (leave empty to skip): ");
                    var img = Console.ReadLine();
                    if (!string.IsNullOrEmpty(img)) cat.Img = img;
                    break;
                case "6":
                    Console.Write("New Description (leave empty to skip): ");
                    var desc = Console.ReadLine();
                    if (!string.IsNullOrEmpty(desc)) cat.Description = desc;
                    break;
                case "7":
                    Console.Write("Is Adopted? (y/n, leave empty to skip): ");
                    var adopted = Console.ReadLine();
                    if (!string.IsNullOrEmpty(adopted))
                        cat.IsAdopted = adopted.ToLower() == "y";
                    break;
                case "8":
                    Console.Write("Is Healthy? (y/n, leave empty to skip): ");
                    var healthy = Console.ReadLine();
                    if (!string.IsNullOrEmpty(healthy))
                        cat.IsHealthy = healthy.ToLower() == "y";
                    break;
                case "9":
                    _context.SaveChanges();
                    Console.WriteLine("Cat updated!");
                    Console.ReadKey();
                    return;
                case "0":
                    Console.WriteLine("Edit canceled, no changes saved.");
                    Console.ReadKey();
                    return;
            }
        }
    }

    private void DeleteCat()
    {
        Console.Write("Cat Id to delete: ");
        int id = int.Parse(Console.ReadLine());
        var cat = _context.Cat.Find(id);
        if (cat != null)
        {
            _context.Cat.Remove(cat);
            _context.SaveChanges();
        }
        Console.WriteLine("Deleted!");
        Console.ReadKey();
    }

    private void ViewCatDetails()
    {
        Console.Write("Cat Id: ");
        int id = int.Parse(Console.ReadLine());
        var cat = _context.Cat.Include(c => c.Breed).FirstOrDefault(c => c.Id == id);
        if (cat != null)
        {
            Console.WriteLine($"Name: {cat.Name}");
            Console.WriteLine($"Breed: {cat.Breed?.Name}");
            var birth = cat.BirthDate;
            var today = DateOnly.FromDateTime(DateTime.Today);
            var totalDays = today.DayNumber - birth.DayNumber;
            var totalWeeks = totalDays / 7;
            var totalMonths = (today.Year - birth.Year) * 12 + today.Month - birth.Month;
            var totalYears = today.Year - birth.Year;
            if (birth > today.AddYears(-totalYears)) { totalYears--; }
            string ageDisplay = totalDays < 7 ? $"{totalDays} day{(totalDays == 1 ? "" : "s")}"
                : totalDays < 30 ? $"{totalWeeks} week{(totalWeeks == 1 ? "" : "s")}"
                : totalMonths < 12 ? $"{totalMonths} month{(totalMonths == 1 ? "" : "s")}"
                : $"{totalYears} year{(totalYears == 1 ? "" : "s")}";
            Console.WriteLine($"Age: {ageDisplay}");
            Console.WriteLine($"Adopted: {cat.IsAdopted}");
            Console.WriteLine($"Healthy: {cat.IsHealthy}");
            Console.WriteLine($"Description: {cat.Description}");
        }
        Console.WriteLine("Press any key...");
        Console.ReadKey();
    }

    private void BreedsMenu()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("BREEDS");
            Console.WriteLine("1. List Breeds");
            Console.WriteLine("2. Create Breed");
            Console.WriteLine("3. Delete Breed");
            Console.WriteLine("4. Back");

            switch (Console.ReadLine())
            {
                case "1": 
                    ListBreeds(); 
                    break;
                case "2": 
                    CreateBreed();
                    break;
                case "3": 
                    DeleteBreed(); 
                    break;
                case "4":
                    return;
            }
        }
    }

    private void ListBreeds()
    {
        Console.Clear();
        var breeds = _context.Breed.ToList();
        foreach (var b in breeds)
            Console.WriteLine($"#{b.Id} - {b.Name}");
        Console.ReadKey();
    }

    private void CreateBreed()
    {
        Console.Write("Name: ");
        string name = Console.ReadLine();
        Console.Write("Description: ");
        string desc = Console.ReadLine();
        _context.Breed.Add(new Breed { Name = name, Description = desc });
        _context.SaveChanges();
        Console.WriteLine("Created!");
        Console.ReadKey();
    }

    private void DeleteBreed()
    {
        Console.Write("Breed Id to delete: ");
        int id = int.Parse(Console.ReadLine());
        var breed = _context.Breed.Find(id);
        if (breed != null)
        {
            _context.Breed.Remove(breed);
            _context.SaveChanges();
        }
        Console.WriteLine("Deleted!");
        Console.ReadKey();
    }

    private void AdoptionsMenu()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("ADOPTIONS");
            Console.WriteLine("1. List Adoptions");
            Console.WriteLine("2. Approve Adoption");
            Console.WriteLine("3. Deny Adoption");
            Console.WriteLine("4. Delete Adoption");
            Console.WriteLine("5. Back");

            switch (Console.ReadLine())
            {
                case "1": 
                    ListAdoptions();
                    break;
                case "2": 
                    ChangeAdoptionStatus(ApplicationStatus.Approved);
                    break;
                case "3": 
                    ChangeAdoptionStatus(ApplicationStatus.Denied); 
                    break;
                case "4": 
                    DeleteAdoption(); 
                    break;
                case "5":
                    return;
            }
        }
    }

    private void ListAdoptions()
    {
        Console.Clear();
        var list = _context.Adoption.Include(a => a.Cat).Include(a => a.User).ToList();
        foreach (var a in list)
            Console.WriteLine($"#{a.Id} - {a.Cat?.Name} - {a.Status} - User: {a.User?.Email ?? a.UserId}");
        Console.ReadKey();
    }

    private void CreateAdoption()
    {
        Console.Clear();
        var availableCats = _context.Cat.Where(c => !c.IsAdopted).ToList();
        if (!availableCats.Any())
        {
            Console.WriteLine("No cats available for adoption.");
            Console.ReadKey();
            return;
        }
        Console.WriteLine("Available cats:");
        foreach (var c in availableCats)
            Console.WriteLine($"{c.Id}: {c.Name}");

        Console.Write("Cat Id to adopt: ");
        int catId = int.Parse(Console.ReadLine());

        var users = _context.Users.ToList();
        Console.WriteLine("Users:");
        foreach (var u in users)
            Console.WriteLine($"{u.Id}: {u.Email}");
        Console.Write("User Id: ");
        string userId = Console.ReadLine();

        _context.Adoption.Add(new Adoption
        {
            CatId = catId,
            UserId = userId,
            AdoptionDate = DateOnly.FromDateTime(DateTime.Now),
            Status = ApplicationStatus.Pending
        });
        _context.SaveChanges();
        Console.WriteLine("Adoption request created!");
        Console.ReadKey();
    }

    private void ChangeAdoptionStatus(ApplicationStatus status)
    {
        Console.Write("Adoption Id: ");
        int id = int.Parse(Console.ReadLine());
        var adoption = _context.Adoption.Find(id);
        if (adoption != null)
        {
            adoption.Status = status;
            if (status == ApplicationStatus.Approved)
            {
                var cat = _context.Cat.Find(adoption.CatId);
                if (cat != null) cat.IsAdopted = true;
            }
            else if (status == ApplicationStatus.Denied)
            {
                var cat = _context.Cat.Find(adoption.CatId);
                if (cat != null) cat.IsAdopted = false;
            }
            _context.SaveChanges();
            Console.WriteLine($"Status updated to {status}");
        }
        Console.ReadKey();
    }

    private void DeleteAdoption()
    {
        Console.Write("Adoption Id: ");
        int id = int.Parse(Console.ReadLine());
        var adoption = _context.Adoption.Find(id);
        if (adoption != null)
        {
            _context.Adoption.Remove(adoption);
            _context.SaveChanges();
        }
        Console.WriteLine("Deleted!");
        Console.ReadKey();
    }

    private void CatCareMenu()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("CAT CARE");
            Console.WriteLine("1. List Care Records");
            Console.WriteLine("2. Create Care Record (Donation)");
            Console.WriteLine("3. Mark as Satisfied");
            Console.WriteLine("4. Delete Care Record");
            Console.WriteLine("5. Back");

            switch (Console.ReadLine())
            {
                case "1": 
                    ListCatCare(); 
                    break;
                case "2": 
                    CreateCatCare(); 
                    break;
                case "3": 
                    MarkCareSatisfied();
                    break;
                case "4": 
                    DeleteCatCare(); 
                    break;
                case "5": 
                    return;
            }
        }
    }

    private void ListCatCare()
    {
        Console.Clear();
        var list = _context.CatCare.Include(c => c.Cat).Include(c => c.Care).Include(c => c.User).ToList();
        foreach (var c in list)
            Console.WriteLine($"#{c.Id} - {c.Cat?.Name} - {c.Care?.CareName} - {c.Price:C} - Satisfied: {c.IsSatisfied}");
        Console.ReadKey();
    }

    private void CreateCatCare()
    {
        Console.Clear();
        var cats = _context.Cat.ToList();
        Console.WriteLine("Cats:");
        foreach (var c in cats)
            Console.WriteLine($"{c.Id}: {c.Name}");
        Console.Write("Cat Id: ");
        int catId = int.Parse(Console.ReadLine());

        var cares = _context.Care.ToList();
        Console.WriteLine("Care Types:");
        foreach (var c in cares)
            Console.WriteLine($"{c.Id}: {c.CareName}");
        Console.Write("Care Id: ");
        int careId = int.Parse(Console.ReadLine());

        var users = _context.Users.ToList();
        Console.WriteLine("Users:");
        foreach (var u in users)
            Console.WriteLine($"{u.Id}: {u.Email}");
        Console.Write("User Id: ");
        string userId = Console.ReadLine();

        Console.Write("Price: ");
        decimal price = decimal.Parse(Console.ReadLine());

        _context.CatCare.Add(new CatCare
        {
            CatId = catId,
            CareId = careId,
            UserId = userId,
            Price = price,
            IsSatisfied = false
        });
        _context.SaveChanges();
        Console.WriteLine("Care record created!");
        Console.ReadKey();
    }


    private void MarkCareSatisfied()
    {
        Console.Write("Care Id to mark as satisfied: ");
        int id = int.Parse(Console.ReadLine());
        var care = _context.CatCare.Find(id);
        if (care != null)
        {
            care.IsSatisfied = true;
            _context.SaveChanges();
        }
        Console.WriteLine("Marked as satisfied!");
        Console.ReadKey();
    }

    private void DeleteCatCare()
    {
        Console.Write("Care Id to delete: ");
        int id = int.Parse(Console.ReadLine());
        var care = _context.CatCare.Find(id);
        if (care != null)
        {
            _context.CatCare.Remove(care);
            _context.SaveChanges();
        }
        Console.WriteLine("Deleted!");
        Console.ReadKey();
    }
}