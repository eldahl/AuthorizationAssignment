
using System.Data.SQLite;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

public class HTTPLoginCredentials {
    public string Username { get; set; }
    public string Password { get; set; }
}

public class HTTPCommentData {
    public string Content { get; set; }
    public int CommentId { get; set; }
}

public class HTTPArticleData {
    public int ArticleId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
}

[Authorize]
[ApiController]
[Route("[controller]")]
public class RoutesController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private SQLiteDatabaseContext _dbcontext;
    public RoutesController(IConfiguration configuration, SQLiteDatabaseContext dbcontext)
    {
        _configuration = configuration;
        _dbcontext = dbcontext;
    }

    [AllowAnonymous]
    [HttpGet("isOnline")]
    public ActionResult<string> GetIsOnline() {
        return "Online";
    }

    // lookup the username in the sqlite database and check if the password is correct
    // if the password is correct, generate a JWT token and return it
    [AllowAnonymous]
    [HttpPost("login")]
    public ActionResult<string> PostLogin([FromBody]HTTPLoginCredentials login ) {

        if(login.Username == null || login.Password == null)
            return BadRequest("Username or password not provided.");
        
        var userData = _dbcontext.GetUser(login.Username);
        string suppliedPasswordHash = Convert.ToBase64String(SHA512.Create().ComputeHash(Encoding.UTF8.GetBytes(login.Password)));
        
        // Check if the user exists and the password is correct
        if(userData == null || !userData.PasswordHash.SequenceEqual(suppliedPasswordHash))
            return Unauthorized("Invalid username or password.");

        // Generate JWT token
        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!);

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer, 
            audience, 
            [ 
                new Claim(ClaimTypes.Role, userData.UserRole),
                new Claim(ClaimTypes.NameIdentifier, userData.UserId.ToString())
            ], 
            expires: DateTime.UtcNow.AddHours(2), 
            signingCredentials: credentials
        );

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtTokenString = tokenHandler.WriteToken(token);
        return Ok(jwtTokenString);
    }

    [Authorize]
    [HttpGet("getRole")]
    public ActionResult<string> GetRole() {
        return HttpContext.User.Claims.First(c => c.Type == ClaimTypes.Role).Value;
    }
    
    [AllowAnonymous]
    [HttpGet("getArticle")]
    public ArticleData GetArticle([FromQuery] int articleID) {
        // Get article from database. 
        return _dbcontext.GetArticleById(articleID);
    }

    [Authorize(Roles = "Registered")]
    [HttpPost("createComment")]
    public ActionResult<string> CreateComment([FromBody] HTTPCommentData comment) {
        // Create comment in database
        _dbcontext.CreateComment(comment.Content, comment.CommentId, Int32.Parse(HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value), DateTime.Now);
        return "Success: Comment created.";
    }

    [Authorize(Roles = "Journalist")]
    [HttpPost("createArticle")]
    public ActionResult<string> CreateArticle([FromBody] HTTPArticleData article) {
        // Create article in database
        _dbcontext.CreateArticle(article.Title, article.Content, Int32.Parse(HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value), DateTime.Now);
        return "Success: Article created.";
    }

    [Authorize(Roles = "Journalist,Editor")]
    [HttpPut("editArticle")]
    public ActionResult<string> EditArticle([FromBody] HTTPArticleData article) {

        ArticleData articleToEdit = _dbcontext.GetArticleById(article.ArticleId);

        // Check if the user is the author of the article
        if(HttpContext.User.Claims.First(c => c.Type == ClaimTypes.Role).Value == "Journalist" && HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value != articleToEdit.AuthorId.ToString())
            return Unauthorized("You are not the author of this article.");

        _dbcontext.UpdateArticle(article.ArticleId, article.Title, article.Content);
        return "Success: Article updated.";
    }

    [Authorize(Roles = "Editor")]
    [HttpDelete("deleteArticle")]
    public ActionResult<string> DeleteArticle([FromQuery] int articleID) {
        _dbcontext.DeleteArticle(articleID);
        return "Success: Article deleted.";
    }

    [Authorize(Roles = "Editor")]
    [HttpPut("editComment")]
    public ActionResult<string> EditComment([FromBody] HTTPCommentData comment) {

        _dbcontext.UpdateComment(comment.CommentId, comment.Content);
        return "Success: Comment updated.";

    }

    [Authorize(Roles = "Editor")]
    [HttpDelete("deleteComment")]
    public ActionResult<string> DeleteComment([FromQuery] int commentID) {
        _dbcontext.DeleteComment(commentID);
        return "Success: Comment deleted.";
    }
}
