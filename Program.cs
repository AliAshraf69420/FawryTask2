public interface IInventoryItem
{
    string ISBN { get; }
    string Title { get; }
    int PublishYear { get; }
    float Price { get; }
    string Author { get; }
}
public abstract class InventoryItem : IInventoryItem
{
    private string isbn { get; set; }
    private string title { get; set; }
    private int publishYear { get; set; }
    private float price { get; set; }
    private string author { get; set; }

    public string ISBN
    {
        get { return isbn; }
        set { isbn = value; }
    }
    public string Title
    {
        get { return title; }
        set { title = value; }

    }
    public int PublishYear
    {
        get { return publishYear; }
        set { publishYear = value; }
    }
    public float Price
    {
        get { return price; }
        set { price = value; }
    }
    public string Author
    {
        get { return author; }
        set { author = value; }
    }
    public InventoryItem(string isbn, string title, int publishYear, float price, string author)
    {
        this.isbn = isbn;
        this.title = title;
        this.publishYear = publishYear;
        this.price = price;
        this.author = author;
    }

    public abstract void Deliver(string destination);

}

public class PaperBackBook : InventoryItem
{
    private int stock;
    public int Stock
    {
        get { return stock; }
        set { stock = value; }
    }
    public PaperBackBook(string isbn, string title, int publishYear, float price, string author, int stock)
        : base(isbn, title, publishYear, price, author)
    {
        this.stock = stock;
    }
    public override void Deliver(string address)
    {
        Console.WriteLine($"Shipping paper book '{Title}' to {address}.");
    }
}
public class EBook : InventoryItem
{
    private string fileType;
    public string FileType
    {
        get { return fileType; }
        set { fileType = value; }
    }

    public EBook(string isbn, string title, int publishYear, float price, string author, string fileType)
        : base(isbn, title, publishYear, price, author)
    {
        this.fileType = fileType;
    }

    public override void Deliver(string email)
    {
        Console.WriteLine($"Sending ebook '{Title}' to {email}.");
    }
}
public class ShowcaseBook : InventoryItem
{
    public ShowcaseBook(string isbn, string title, int publishYear, float price, string author)
        : base(isbn, title, publishYear, price, author) { }

    public override void Deliver(string address)
    {
        //was looking for a keyword that is similar to Pass in python but decided to just do nothing (kinda like spin waiting except you don't wait)
    }
}
public class Inventory
{
    private Dictionary<string, InventoryItem> items = new();

    public void AddBook(InventoryItem item)
    {
        items[item.ISBN] = item;
        Console.WriteLine($"Added '{item.Title}' to inventory.");
    }

    public InventoryItem? GetBook(string isbn)
    {
        return items.TryGetValue(isbn, out var book) ? book : null;
    }

    public List<InventoryItem> RemoveOutdatedBooks()
    {
        int currentYear = DateTime.Now.Year;
        var outdated = items.Values
                            .Where(book => currentYear - book.PublishYear > 500)
                            .ToList();

        foreach (var book in outdated)
        {
            items.Remove(book.ISBN);
            Console.WriteLine($"Removed outdated book '{book.Title}'.");
        }

        return outdated;
    }

    public float BuyBook(string isbn, int quantity, string email, string address)
    {
        if (!items.TryGetValue(isbn, out var book))
        {
            Console.WriteLine("Can't find Book.");
        }


        if (book is PaperBackBook pb)
        {
            if (pb.Stock < quantity)
            {
                Console.WriteLine("Not enough stock for the requested quantity.");
                return 0;
            }
            pb.Stock -= quantity;
            pb.Deliver(address);
        }
        else if (book is EBook eb)
        {
            eb.Deliver(email);
        }
        else if (book is ShowcaseBook sb)
        {
            sb.Deliver(address); // bad error handling if I'm being honest and in general bad handling of the showcase book case.
        }

        float total = book.Price * quantity;
        Console.WriteLine($"Total amount paid: {total}");
        return total;
    }
}
public class Customer
{
    private string name;
    private string email;
    private string adress;
    public string Name
    {
        get { return name; }
        set { name = value; }
    }
    public string Email
    {
        get { return email; }
        set { email = value; }
    }
    public string Adress
    {
        get { return adress; }
        set { adress = value; }
    }

    public Customer(string name, string email, string adress)
    {
        this.name = name;
        this.email = email;
        this.adress = adress;
    }

    public void BuyBook(InventoryItem book, int quantity, Inventory inventory)
    {
        Console.WriteLine($"{Name} is buying {quantity} copy/copies of book {book.Title}.");
        float paid = inventory.BuyBook(book.ISBN, quantity, email, adress);
        Console.WriteLine($"{Name} has paid {paid}.");
    }
}
public class Testing
{
    public static void Main()
    {
        Inventory inventory = new Inventory();
        PaperBackBook paperBook = new PaperBackBook("PB100", "No Longer Human", 1948, 45.5f, "Osamu Dazai", 5);
        EBook ebook = new EBook("EB100", "The Secret History", 1992, 30.0f, "Donna Tart", "PDF");
        ShowcaseBook demoBook = new ShowcaseBook("SB100", "Demo of The Odyssey", 1500, 0f, "Homer"); //I just realized I didn't handle books released in BCE, only thought of it now after making a reference to the odyssey

        inventory.AddBook(paperBook);
        inventory.AddBook(ebook);
        inventory.AddBook(demoBook);

        Customer customer = new Customer("Ali Ashraf", "ali@example.com", "some address");

        customer.BuyBook(paperBook, 2, inventory);

        customer.BuyBook(ebook, 1, inventory);

        customer.BuyBook(paperBook, 10, inventory);

        customer.BuyBook(demoBook, 1, inventory);

        inventory.RemoveOutdatedBooks();
    }
}
