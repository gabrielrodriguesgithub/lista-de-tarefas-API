using ListaDeTarefasAPI.Dados;
using ListaDeTarefasAPI.EndPoints;
using ListaDeTarefasAPI.Modelos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyAllowSpecificOrigins",
    builder =>
    {
        builder.WithOrigins("http://localhost:4200")
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<ListaDeTarefasContext>((options) => {
    options
        .UseSqlServer(builder.Configuration["ConnectionStrings:ListaDeTarefasAPIContext"])
        .UseLazyLoadingProxies();
});

builder.Services
    .AddIdentityApiEndpoints<PessoaComAcesso>()
    .AddEntityFrameworkStores<ListaDeTarefasContext>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddAuthorization();


builder.Services.AddHealthChecks();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
});

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

var app = builder.Build();

app.UseCors("MyAllowSpecificOrigins");

// Ordem importante: UseAuthentication deve vir antes de UseAuthorization
app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.AddEndPointsTarefas();

app.MapGroup("auth").MapIdentityApi<PessoaComAcesso>().WithTags("Autorização");

app.MapPost("auth/loginn", async (SignInManager<PessoaComAcesso> signInManager, UserManager<PessoaComAcesso> userManager, [FromBody] LoginModel model) =>
{
    try
    {
        var user = await userManager.FindByEmailAsync(model.Email); // Procura o usuário pelo e-mail
        if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Email, user.Email) // Adiciona o e-mail como uma claim
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: builder.Configuration["Jwt:Issuer"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(double.Parse(builder.Configuration["Jwt:ExpiryInMinutes"])),
                signingCredentials: creds);

            return Results.Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });
        }
        return Results.Unauthorized();
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
}).WithTags("Autorização");



app.MapPost("auth/logout", async ([FromServices] SignInManager<PessoaComAcesso> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.Ok();
}).RequireAuthorization().WithTags("Autorização");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
