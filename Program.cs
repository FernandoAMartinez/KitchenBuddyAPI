using Supabase;
using KitchenBuddyAPI.Models;
using KitchenBuddyAPI.Contracts;
using KitchenBuddyAPI.Helpers;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();

//Add Supabase Dependencies
builder.Services.AddScoped<Supabase.Client>(_ =>
    new Supabase.Client(
        builder.Configuration["SUPABASE_URL"],
        builder.Configuration["SUPABASE_KEY"],
        new SupabaseOptions()
        {
            AutoRefreshToken = true,
            AutoConnectRealtime = true
        }
    )
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//User Authentication Endpoints
app.MapGet("/hash/{password}", ([FromRoute] string password, IPasswordHasher hashser) => hashser.Hash(password)).WithTags("Test");
// /auth endpoints
app.MapPost("/auth/login", async ([FromBody] AuthRequest request, Supabase.Client client, IPasswordHasher hasher) =>
{
    var response = await client.From<User>().Where(x => x.Email == request.Email).Get();

    if (response is null)
        return Results.NotFound();

    var user = response.Models.FirstOrDefault();

    if (user is null)
        return Results.NotFound();

    var isValid = hasher.Verify(user.Password, request.Password);

    if (!isValid)
        return Results.BadRequest();

    return Results.Ok(new UserResponse(user.Id, user.CreatedAt, user.Email));
}).WithTags("Auth");

app.MapPost("/auth/register", async ([FromBody] AuthRequest request, Supabase.Client client, IPasswordHasher hasher) =>
{
    var response = await client.From<User>().Where(x => x.Email == request.Email).Get();

    if (!response.Models.Count.Equals(0))
        return Results.BadRequest();

    var user = new User()
    {
        CreatedAt = DateTime.Now,
        Email = request.Email,
        Password = hasher.Hash(request.Password)
    };

    try
    {
        var post = await client.From<User>().Insert(user);

        if (post is null)
            return Results.BadRequest();

        user = post.Models.FirstOrDefault();
    }
    catch
    {
        throw new Exception("Error at posting user to Supabase.");
    }

    return Results.Ok(new UserResponse(user.Id, user.CreatedAt, user.Email));
}).WithTags("Auth");

// profile endpoints
app.MapPost("/profile/register", async ([FromBody] ProfileRequest request, Supabase.Client client) =>
{
    var response = await client.From<Profile>().Where(x => x.Email == request.Email).Get();

    if (!response.Models.Count.Equals(0))
        return Results.BadRequest();

    var profile = new Profile()
    {
        CreatedAt = DateTime.Now,
        Email = request.Email,
        FirstName = request.FirstName,
        LastName = request.LastName,
        Birthday = request.Birthday,
        FollowerCount = 0,
        FollowingCount = 0
    };

    try
    {
        var post = await client.From<Profile>().Insert(profile);

        if (post is null)
            return Results.BadRequest();

        profile = post.Models.FirstOrDefault();
    }
    catch
    {
        throw new Exception("Error at posting profile to Supabase.");
    }

    return Results.Ok(new ProfileResponse(profile.Id, profile.CreatedAt, profile.Email, profile.FirstName, profile.LastName, profile.Birthday, profile.FollowerCount, profile.FollowingCount));
}).WithTags("Profile");

app.MapPost("/profile/follow", async ([FromBody] FollowRequest request, Supabase.Client client) =>
{
    var response = await client.From<FollowProfile>().Where(x => x.ProfileId == request.ProfileId && x.FollowerId == request.FollowerId).Get();

    if (!response.Models.Count.Equals(0))
        return Results.BadRequest();

    var follow = new FollowProfile()
    {
        ProfileId = request.ProfileId,
        FollowerId = request.FollowerId
    };

    try
    {
        var post = await client.From<FollowProfile>().Insert(follow);

        if (post is null)
            return Results.BadRequest();
    }
    catch
    {
        throw new Exception("Error at posting follow profile to Supabase.");
    }

    return Results.Ok(new FollowResponse(request.ProfileId, request.FollowerId));
}).WithTags("Profile");

app.MapDelete("/profile/unfollow", async ([FromBody] FollowRequest request, Supabase.Client client) => 
{
    var response = await client.From<FollowProfile>().Where(x => x.ProfileId == request.ProfileId && x.FollowerId == request.FollowerId).Get();

    if (response.Models.Count.Equals(0))
        return Results.BadRequest();

    try
    {
        await client.From<FollowProfile>().Where(x => x.ProfileId == request.ProfileId && x.FollowerId == request.FollowerId).Delete();
    }
    catch
    {
        throw new Exception("Error at posting follow profile to Supabase.");
    }

    return Results.Ok();
}).WithTags("Profile");

app.UseHttpsRedirection();

app.Run();
