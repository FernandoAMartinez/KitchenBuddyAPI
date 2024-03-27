using Supabase;
using KitchenBuddyAPI.Models;
using KitchenBuddyAPI.Contracts;
using KitchenBuddyAPI.Helpers;
using Microsoft.AspNetCore.Mvc;
using SQLitePCL;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Azure.Core;

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
#region Auth Endpoints
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
#endregion

// profile endpoints
#region Profile Endpoints
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

app.MapGet("/profile/{id}", async ([FromRoute] long id, Supabase.Client client) =>
{
    var response = await client.From<Profile>().Where(x => x.Id == id).Get();

    if (response is null)
        return Results.NotFound();

    var profile = response.Models.FirstOrDefault();

    return Results.Ok(new ProfileResponse(profile.Id, profile.CreatedAt, profile.Email, profile.FirstName, profile.LastName, profile.Birthday, profile.FollowerCount, profile.FollowingCount));
}).WithTags("Profile");

app.MapPut("/profile/{id}", async ([FromRoute] long id, [FromBody] Profile request, Supabase.Client client) =>
{
    var response = await client.From<Profile>().Where(x => x.Id == id).Get();

    if (response is null)
        return Results.NotFound();

    var update = await client.From<Profile>().Where(x => x.Id == request.Id).Single();

    try
    {
        update.FirstName = request.FirstName;
        update.LastName = request.LastName;
        update.Birthday = request.Birthday;

        await update.Update<Profile>();
    }
    catch
    {
        throw new Exception("Error at updating profile at supabase.");
    }

    return Results.Ok(new ProfileResponse(update.Id, update.CreatedAt, update.Email, update.FirstName, update.LastName, update.Birthday, update.FollowerCount, update.FollowingCount));
}).WithTags("Profile");

app.MapDelete("/profile/{id}", async ([FromRoute] long id, Supabase.Client client) =>
{
    var response = await client.From<FollowProfile>().Where(x => x.ProfileId == id).Get();

    if (response.Models.Count.Equals(0))
        return Results.BadRequest();

    try
    {
        await client.From<FollowProfile>().Where(x => x.ProfileId == id).Delete();
    }
    catch
    {
        throw new Exception("Error at posting follow profile to Supabase.");
    }

    return Results.Ok();
}).WithTags("Profile");
#endregion

// recipes endpoints
#region Recipe Endpoints
app.MapPost("/recipes", async ([FromBody] RecipeRequest request, Supabase.Client client) =>
{
    var response = await client.From<Recipe>().Where(x => x.Id == request.Id).Get();

    if (!response.Models.Count.Equals(0))
        return Results.BadRequest();

    var recipe = new Recipe()
    {
        CreatedAt = DateTime.Now,
        Title = request.Title,
        Description = request.Description,
        CreatedBy = request.CreatedBy,
        Base = request.Base,
        Origin = request.Origin,
        Steps = request.Steps
    };

    try
    {
        var post = await client.From<Recipe>().Insert(recipe);

        if (post is null)
            return Results.BadRequest();

        recipe = post.Models.FirstOrDefault();
    }
    catch
    {
        throw new Exception("Error at creating the recipe in supabase.");
    }

    return Results.Ok(new RecipeResponse(recipe.Id, recipe.CreatedAt, recipe.Title, recipe.Description, recipe.CreatedBy, recipe.Base, recipe.Origin, recipe.Steps));
}).WithTags("Recipes");

#endregion

#region Ingredients
app.MapPost("/ingredients", async ([FromBody] IngredientRequest request, Supabase.Client client) =>
{
    var response = await client.From<Ingredient>().Where(x => x.RecipeId == request.RecipeId && x.IngredientId == request.IngredientId).Get();

    if (!response.Models.Count.Equals(0))
        return Results.BadRequest();

    var ingredient = new Ingredient()
    {
        RecipeId = request.RecipeId,
        Name = request.Name,
        Amount = request.Amount,
        Unit = request.Unit
    };

    try
    {
        var post = await client.From<Ingredient>().Insert(ingredient);

        if (post is null)
            return Results.BadRequest();

        ingredient = post.Models.FirstOrDefault();
    }
    catch
    {
        throw new Exception("Error at creating the ingredient in supabase.");
    }

    return Results.Ok(new IngredientResponse(ingredient.RecipeId, ingredient.IngredientId, ingredient.Name, ingredient.Amount, ingredient.Unit));
}).WithTags("Ingredients");

app.MapGet("/ingredients/{recipeId}", async ([FromRoute] long recipeId, Supabase.Client client) =>
{
    var response = await client.From<Ingredient>().Where(x => x.RecipeId == recipeId).Get();

    if (response.Models.Count.Equals(0))
        return Results.BadRequest();

    var ingredients = new List<IngredientResponse>();

    foreach (var ing in response.Models.ToList())
    {
        ingredients.Add(
            new IngredientResponse(ing.RecipeId, ing.IngredientId, ing.Name, ing.Amount, ing.Unit)
        );
    }

    return Results.Ok(ingredients);
}).WithTags("Ingredients");
#endregion
app.UseHttpsRedirection();

app.Run();
