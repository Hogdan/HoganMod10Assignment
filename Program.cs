using NLog;

string path = Directory.GetCurrentDirectory() + "//nlog.config";

// create instance of Logger
var logger = LogManager.Setup().LoadConfigurationFromFile(path).GetCurrentClassLogger();
logger.Info("Program started");

DataContext db = new();
string? input = "";

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
            CreatePost();
            break;
        case "4":
            logger.Info("User selected option 4 - Display Posts");
            break;
        case "rmv blog":
            logger.Info("User selected rmv blog - Remove Blog");
            DeleteBlog();
            break;
        default:
            logger.Info("User selected option quit - Exiting program");
            return;
    }
} while (true);

void DisplayAllBlogs()
{
    Blog[] query = [.. db.Blogs.OrderBy(b => b.Name)];

    Console.WriteLine("Current Blog list:");
    foreach (Blog blog in query)
    {
        Console.WriteLine($"{blog.Name} ID: {blog.BlogId}");
    }
}

void AddBlog()
{
    Console.WriteLine("Enter the name of the blog to add:");
    string? name = Console.ReadLine();
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
    Blog blog = new() { Name = name };
    db.AddBlog(blog);
    logger.Info("Blog added - {name}", name);
}

void CreatePost()
{
    // prompt for blog name
    Console.WriteLine("Enter the name or id of the blog you want to make a post on:");
    string? input = Console.ReadLine();
    Blog blog = GetBlogByNameOrId(input);
    // prompt for post title
    Console.WriteLine("Enter the title of the post:");
    string? title = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(title))
    {
        Console.WriteLine("Post title cannot be empty.");
        return;
    }
    // prompt for post content
    Console.WriteLine("Enter the content of the post:");
    string? content = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(content))
    {
        Console.WriteLine("Post content cannot be empty.");
        return;
    }
    // create post object
    Post post = new() { Title = title, Content = content, BlogId = blog.BlogId };
    // add post to database
    db.Posts.Add(post);
    db.SaveChanges();
    logger.Info("Post created - {title} in blog {blog}", title, blog.Name);
    // display success message
    Console.WriteLine($"Post '{title}' created in '{blog.Name}'.");
}

void DisplayPosts()
{
    // TODO
}

void DeleteBlog()
{
    Console.WriteLine("Enter the name or id of the blog to delete:");
    string? input = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(input))
    {
        Console.WriteLine("Blog name or id cannot be empty.");
        return;
    }
    Blog? blog = db.Blogs.FirstOrDefault(b => b.Name == input || b.BlogId.ToString() == input);
    if (blog == null)
    {
        Console.WriteLine("Blog not found.");
        return;
    }
    db.Blogs.Remove(blog);
    db.SaveChanges();
    logger.Info("Blog deleted - {name}", blog.Name);
}

Blog GetBlogByNameOrId(string? input)
{
    if (string.IsNullOrWhiteSpace(input))
    {
        throw new ArgumentException("Blog name or id cannot be empty.");
    }
    Blog? blog = db.Blogs.First(b => b.Name == input || b.BlogId.ToString() == input) ?? throw new ArgumentException("Blog not found.");
    return blog;
}
