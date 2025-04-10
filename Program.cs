using NLog;

string path = Directory.GetCurrentDirectory() + "//nlog.config";

// create instance of Logger
var logger = LogManager.Setup().LoadConfigurationFromFile(path).GetCurrentClassLogger();
logger.Info("Program started");

DataContext db = new();
Blog[] blogs = [];
string? input = "";

Console.WriteLine("====| Welcome to BlogCity! |====");
do
{
    Console.WriteLine("\n====| Main Menu |====\n\n1) Display all blogs\n2) Add Blog\n3) Create Post\n4) Display Posts\n");
    Console.WriteLine("Enter selection or q to exit:");
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
            DisplayPosts();
            break;
        default:
            logger.Info("User selected option quit - Exiting program");
            return;
    }
} while (true);

void DisplayAllBlogs()
{
    blogs = [.. db.Blogs.OrderBy(b => b.Name)];

    Console.WriteLine($"====| BlogCity: Home of {blogs.Length} blogs |====\n");
    foreach (Blog blog in blogs)
    {
        Console.WriteLine($"{blog.Name}");
    }
}

void AddBlog()
{
    Console.WriteLine("====| New Blog |====\nName of blog:");
    string? name = Console.ReadLine();
    Boolean blogExists = db.Blogs.Any(b => b.Name == name);
    if (string.IsNullOrWhiteSpace(name) || blogExists)
    {
        Console.WriteLine("Blog name cannot be empty or already exist.");
        return;
    }

    Blog blog = new() { Name = name };
    db.AddBlog(blog);
    logger.Info("Blog added - {name}", name);
}

void CreatePost()
{
    // prompt for blog
    Console.WriteLine("====| New Post |====\nSelect blog to post to:\n");
    Blog blog = PromptForBlog();
    if (string.IsNullOrEmpty(blog.Name))
    {
        return;
    }

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
    logger.Info($"Post created - {title} in blog {blog.Name}");
    // display success message
    Console.WriteLine($"Post '{title}' created in '{blog.Name}'.");
}

void DisplayPosts()
{
    // prompt for blog name
    Console.WriteLine("====| View Posts |====\nView posts from which blog:\n\n0) All Blogs");
    blogs = [.. db.Blogs.OrderBy(b => b.Name)];
    int count = 1;
    foreach (Blog b in blogs)
    {
        Console.WriteLine($"{count}) {b.Name} - {GetPostCount(b)} posts");
        count++;
    }
    // get response from user
    string? input = Console.ReadLine();
    if (input == "0")
    {
        foreach (Blog b in blogs)
        {
            DisplayPostsFromBlog(b);
        }
        return;
    }
    if (int.TryParse(input, out int i) && i > 0 && i <= blogs.Length)
    {
        logger.Info("User selected blog - {blog}", blogs[i - 1].Name);
        DisplayPostsFromBlog(blogs[i - 1]);
    }
    else
    {
        Console.WriteLine("Blog not found.");
    }
    
}

void DisplayPostsFromBlog(Blog blog)
{
    // get posts from database
    Post[] posts = [.. db.Posts.Where(p => p.BlogId == blog.BlogId)];
    // display posts
    Console.WriteLine($"\nPosts in '{blog.Name}' - {posts.Length}");
    if (posts.Length == 0)
    {
        Console.WriteLine("No posts found.");
        return;
    }
    foreach (Post post in posts)
    {
        Console.WriteLine($"\nBlog: {blog.Name}\nTitle: {post.Title}\nBody: {post.Content}");
    }
}

Blog PromptForBlog()
{
    blogs = [.. db.Blogs.OrderBy(b => b.Name)];
    int count = 1;
    foreach (Blog b in blogs)
    {
        Console.WriteLine($"{count}) {b.Name} - {GetPostCount(b)} posts");
        count++;
    }
    
    string? input = Console.ReadLine();
    if (int.TryParse(input, out int index) && index > 0 && index <= blogs.Length)
    {
        logger.Info("User selected blog - {blog}", blogs[index - 1].Name);
        return blogs[index - 1];
    }
    else
    {
        Console.WriteLine("Blog not found.");
        return new Blog();
    }
}

int GetPostCount(Blog blog)
{
    return db.Posts.Count(p => p.BlogId == blog.BlogId);
}
