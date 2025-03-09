
// תמר אלישיב
// full stack1
using ToDoApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
var builder = WebApplication.CreateBuilder(args);
// הוספת cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("http://localhost:3000")
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});
// הזרקת תלות לmySql
builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("ToDoDB"),
    new MySqlServerVersion(new Version(8,0,0))));
var app = builder.Build();
// הפעלת CORS
 app.UseCors("AllowSpecificOrigin");
//  שליפת כל המשימות
app.MapGet("/", async (ToDoDbContext context) =>
{
    var items = await context.Items.ToListAsync();
    return items;
});
// הוספת משימה חדשה
app.MapPost("/one", async ( ToDoDbContext context, Item item) =>
{
    try
    {
        if (item == null)
        {
            return Results.BadRequest("Item cannot be null.");
        }

        context.Items.Add(item);
        await context.SaveChangesAsync();
        return Results.Created($"/one/{item.Id}", item);
    }
    catch (DbUpdateException dbEx)
    {
        return Results.Problem($"Database update error: {dbEx.Message}");
    }
    catch (Exception ex)
    {
        return Results.Problem($"An error occurred: {ex.Message}");
    }
});
// הוספת משימה מותחלת בfalse
app.MapPost("/addItem", async (ToDoDbContext context, [FromBody] string name) =>
{
    Item item = new Item
    {
        Name = name,
        IsComplete = false
    };
    context.Items.Add(item);

    await context.SaveChangesAsync();

    return Results.Created($"/items/{item.Id}", item);
});
// עדכון משימה
app.MapPut("/updateItem", async (ToDoDbContext context, int id, bool? IsComplete) =>
{
    var item = await context.Items.FindAsync(id);
    if (item is null)
    {
        return Results.NotFound();
    }

    if (item.Id == id)
    {
        if (IsComplete != null)
        {
            item.IsComplete = IsComplete;
        }
    }

    await context.SaveChangesAsync();
    return Results.NoContent();

});
// מחיקת משימה
app.MapDelete("/delete/{id}", async (ToDoDbContext context, int id) =>
{
    var delete = await context.Items.FindAsync(id);
    if (delete is null)
    {
        return Results.NotFound();
    }
    context.Items.Remove(delete);
    await context.SaveChangesAsync();
    return Results.Ok(delete);
});

app.MapMethods("/options-or-head", new[] { "OPTIONS", "HEAD" }, 
                          () => "This is an options or head request ");

app.Run();
