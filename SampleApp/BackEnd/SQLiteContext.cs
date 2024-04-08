using System.Data.SQLite;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Net.Mime;
using System.Globalization;

public class UserData {
    public int UserId { get; set; }
    public string Username { get; set; }
    public string UserRole { get; set; }
    public string PasswordHash { get; set; }
}

public class ArticleData {
    public int ArticleId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public int AuthorId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class SQLiteDatabaseContext : DbContext
{
    SQLiteConnection connection;

    public SQLiteDatabaseContext(string _databaseFilePath = "Database.db") {

        connection = new SQLiteConnection("Data Source=" + _databaseFilePath + ";Version=3;");
        connection.Open();

        if(IsNewDatabase())
            CreateDatabase();
    }

    public bool IsNewDatabase()
    {
        using (SQLiteCommand command = new SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table' AND name='users';", connection))
        {
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                return !reader.Read();
            }
        }
    }

    public void CreateDatabase()
    {
        Debug.WriteLine("Creating database.");
        string sql = File.ReadAllText("DatabaseSeed.sql");
        Debug.WriteLine(sql);
        
        // Execute the seed script
        using var cmd = new SQLiteCommand(sql, connection);
        cmd.ExecuteNonQuery();
    }

    public UserData GetUser(string username) {
        string sql = String.Format("SELECT * FROM users WHERE username='{0}';", username);
        Debug.WriteLine(sql);

        using (SQLiteCommand command = new SQLiteCommand(sql, connection))
        { 
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    return new UserData() {
                        UserId = (int)(long)reader["user_id"],
                        Username = (string)reader["username"],
                        UserRole = (string)reader["user_role"],
                        PasswordHash = (string)reader["password_hash"]
                    };
                }
            }
        }
        return null;
    }

    public ArticleData GetArticleById(int articleId) {
        string sql = String.Format("SELECT * FROM articles WHERE article_id={0};", articleId);
        Debug.WriteLine(sql);

        using (SQLiteCommand command = new SQLiteCommand(sql, connection))
        { 
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    return new ArticleData() {
                        ArticleId = (int)(long)reader["article_id"],
                        Title = (string)reader["title"],
                        Content = (string)reader["content"],
                        AuthorId = (int)(long)reader["author_id"],
                        CreatedAt = (DateTime)reader["created_at"]
                    };
                }
            }
        }
        return null;
    }

    public void CreateComment(string content, int commentId, int userId, DateTime createdAt) {
        string sql = String.Format("INSERT INTO comments (content, article_id, user_id, created_at) VALUES ('{0}', {1}, {2}, '{3}');", content, commentId, userId, createdAt);
        Debug.WriteLine(sql);

        using (SQLiteCommand command = new SQLiteCommand(sql, connection))
        {
            command.ExecuteNonQuery();
        }
    }

    public void CreateArticle(string title, string content, int authorId, DateTime createdAt) {
        string sql = String.Format("INSERT INTO articles (title, content, author_id, created_at) VALUES ('{0}', '{1}', {2}, '{3}');", title, content, authorId, createdAt);
        Debug.WriteLine(sql);

        using (SQLiteCommand command = new SQLiteCommand(sql, connection))
        {
            command.ExecuteNonQuery();
        }
    }

    public void UpdateArticle(int articleId, string title, string content) {
        string sql = String.Format("UPDATE articles SET title='{0}', content='{1}' WHERE article_id={2};", title, content, articleId);
        Debug.WriteLine(sql);

        using (SQLiteCommand command = new SQLiteCommand(sql, connection))
        {
            command.ExecuteNonQuery();
        }
    }

    public void DeleteArticle(int articleId) {
        string sql = String.Format("DELETE FROM articles WHERE article_id={0};", articleId);
        Debug.WriteLine(sql);

        using (SQLiteCommand command = new SQLiteCommand(sql, connection))
        {
            command.ExecuteNonQuery();
        }
    }

    public void UpdateComment(int commentId, string content) {
        string sql = String.Format("UPDATE comments SET content='{0}' WHERE comment_id={1};", content, commentId);
        Debug.WriteLine(sql);

        using (SQLiteCommand command = new SQLiteCommand(sql, connection))
        {
            command.ExecuteNonQuery();
        }
    }

    public void DeleteComment(int commentId) {
        string sql = String.Format("DELETE FROM comments WHERE comment_id={0};", commentId);
        Debug.WriteLine(sql);

        using (SQLiteCommand command = new SQLiteCommand(sql, connection))
        {
            command.ExecuteNonQuery();
        }
    }
}