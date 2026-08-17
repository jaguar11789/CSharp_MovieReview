using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MovieReviewApi.Data;
using MovieReviewApi.Repositories.Reviews;
using MovieReviewApi.Repositories.User;
using MovieReviewApi.Services.Accounts.Email;
using MovieReviewApi.Services.Accounts.User;
using MovieReviewApi.Services.Auth;
using MovieReviewApi.Services.Movies;
using MovieReviewApi.Services.Reviews;
using MovieReviewApi.Services.Social;
using MovieReviewApi.Services.TV;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWeb", policy =>
    {
        policy.WithOrigins("https://localhost:7185")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // <-- 쿠키 주고받음
    });
});
builder.Services.AddControllers();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer   = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies["accessToken"];

                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly    = true;
    options.Cookie.IsEssential = true;
});

// builder.Services.AddScoped<UserRepository>();
// Repository
builder.Services.AddScoped<IUserRepository,              UserRepository>();
builder.Services.AddScoped<IUserHistoryRepository,       UserHistoryRepository>();
builder.Services.AddScoped<IUserSocialAccountRepository, UserSocialAccountRepository>();
builder.Services.AddScoped<IEmailVerificationRepository, EmailVerificationRepository>();
builder.Services.AddScoped<IReviewsRepository,           ReviewsRepository>();

builder.Services.AddScoped<IReviewHistoryRepository,     ReviewHistoryRepository>();

// Service
builder.Services.AddScoped<IAuthService,              AuthService>();
builder.Services.AddScoped<IUserService,              UserService>();
builder.Services.AddScoped<IEmailService,             EmailService>();
builder.Services.AddScoped<IEmailVerificationService, EmailVerificationService>();
builder.Services.AddScoped<IReviewService,            ReviewService>();

builder.Services.AddHttpClient<IKakaoAuthService,  KakaoAuthService>();
builder.Services.AddHttpClient<INaverAuthService,  NaverAuthService>();
builder.Services.AddHttpClient<IGoogleAuthService, GoogleAuthService>();

builder.Services.AddHttpClient<ITMDBMoviesService, TMDBMoviesService>();
builder.Services.AddHttpClient<ITMDBTvService,     TMDBTvService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name         = "Authorization",
        Type         = SecuritySchemeType.Http,
        Scheme       = "bearer",
        BearerFormat = "JWT",
        In           = ParameterLocation.Header,

        Description  = "Bearer {token} 형식으로 입력하세요."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseSession();

app.UseCors("AllowWeb");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
