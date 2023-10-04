using System.Text;
using GuardianGate.Handlers;
using GuardianGate.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//Bearer Token Authentication
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY") ?? builder.Configuration["Jwt:Key"]!)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("BasicPolicy", policy =>
        policy.AddAuthenticationSchemes("BasicAuthentication")
            .RequireAuthenticatedUser());

    options.AddPolicy("JwtPolicy", policy =>
        policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser());
});

builder.Services.AddControllers();
builder.Services.AddSingleton<ICosmosDbService, CosmosDbService>();
builder.Services.AddSingleton<IAuthService, AuthService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//Swagger with Bearer Authentication
builder.Services.AddSwaggerGen(
    c =>
    {
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme() 
        { 
            Name = "Authorization", 
            Type = SecuritySchemeType.ApiKey, 
            Scheme = "Bearer", 
            BearerFormat = "JWT", 
            In = ParameterLocation.Header,
        }); 
        c.AddSecurityRequirement(new OpenApiSecurityRequirement 
        { 
            { 
                new OpenApiSecurityScheme 
                { 
                    Reference = new OpenApiReference 
                    { 
                        Type = ReferenceType.SecurityScheme, 
                        Id = "Bearer" 
                    } 
                }, 
                new string[] {} 
            } 
        }); 
    }
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();