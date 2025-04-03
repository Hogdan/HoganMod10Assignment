using NLog;

string path = Directory.GetCurrentDirectory() + "//nlog.config";

// create instance of Logger
var logger = LogManager.Setup().LoadConfigurationFromFile(path).GetCurrentClassLogger();
logger.Info("Program started");

var db = new DataContext();
var input = "";

Console.WriteLine("Welcome to BlogCity!");
do
{
    Console.WriteLine("\n1) Display all blogs\n2) Add Blog\n3) Create Post\n4) Display Posts\n");
    Console.WriteLine("Please select an option (1-4) or any other key to quit:");
    input = Console.ReadLine();

    switch (input)
    {
        case "1":
            logger.Info("User selected option 1 - Display all blogs");
            DisplayAllBlogs();
            break;
        case "2":
            logger.Info("User selected option 2 - Add Blog");
            AddBlog();
            break;
        case "3":
            logger.Info("User selected option 3 - Create Post");
            break;
        case "4":
            logger.Info("User selected option 4 - Display Posts");
            break;
        default:
            logger.Info("User selected option quit - Exiting program");
            return;
    }
} while (true);

void DisplayAllBlogs()
{
    var query = db.Blogs.OrderBy(b => b.Name);

    Console.WriteLine("Current Blog list:");
    foreach (var item in query)
    {
    Console.WriteLine(item.Name);
    }
}

void AddBlog()
{
    Console.WriteLine("Enter the name of the blog to add:");
    var name = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(name))
    {
        Console.WriteLine("Blog name cannot be empty.");
        return;
    }
    if (db.Blogs.Any(b => b.Name == name))
    {
        Console.WriteLine("Blog with this name already exists.");
        return;
    }
    var blog = new Blog { Name = name };
    db.AddBlog(blog);
    logger.Info("Blog added - {name}", name);
}

void CreatePost()
{
    // TODO
}

void DisplayPosts()
{
    // TODO
}
