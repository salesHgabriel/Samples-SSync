using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.EntityFrameworkCore;
using PocServerSync.Data;
using PocServerSync.Models;
using PocServerSync.Services;
using PocServerSync.Sync;
using SSync.Server.LitebDB.Abstractions;
using SSync.Server.LitebDB.Engine;
using SSync.Server.LitebDB.Enums;


const string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PocDbContext>();

builder.Services.Configure<JsonOptions>(options =>
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddSSyncSchemaCollection<PocDbContext>(
    optionsPullChanges: (pullChangesConfig) =>
    {
        pullChangesConfig
            .By<UserSync>(User.CollectionName)
            .ThenBy<NoteSync>(Note.CollectionName);
    },
    optionsPushChanges: (pushChangesConfig) =>
    {
        pushChangesConfig
            .By<UserSync>(User.CollectionName)
            .ThenBy<NoteSync>(Note.CollectionName)
            .ThenBy<DocSync>(Doc.CollectionName);
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy => { policy.WithOrigins("*").AllowAnyHeader().AllowAnyMethod(); });
});



builder.Services.AddScoped<IFileService, FileService>();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.UseCors(MyAllowSpecificOrigins);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

var syncGroup = app.MapGroup("api/sync").WithOpenApi();

syncGroup.MapGet("/pull",
    async ([AsParameters] CustomParamenterSync parameter, [FromServices] ISchemaCollection schemaCollection) =>
    {
        var changes = await schemaCollection.PullChangesAsync(parameter);

        return Results.Ok(changes);
    });

syncGroup.MapPost("/push",
    async (HttpContext httpContext, [FromBody] JsonArray changes, [FromServices] ISchemaCollection schemaCollection) =>
    {
        var query = httpContext.Request.Query;

        var sucesso = Guid.TryParse(query["userId"], out var userId);
        var parameter = new CustomParamenterSync
        {
            UserId = sucesso ? userId : null,
            Colletions = query["collections"].ToArray()!,
            Timestamp = DateTime.TryParse(query["timestamp"], out DateTime timestamp) ? timestamp : DateTime.MinValue
        };


        var now = await schemaCollection.PushChangesAsync(changes, parameter);

        return Results.Ok(now);
    });


syncGroup.MapPost("/doc-up", async
([FromForm] IFormFile file1,
    [FromQuery] Guid docId,
    [FromServices] PocDbContext dbContext,
    [FromServices] IFileService fileService) =>
{
    var doc = await dbContext.Docs.FindAsync(docId);

    if (doc == null)
    {
        return Results.NotFound();
    }

    var fileName = $"{doc.Id}{Path.GetExtension(file1.FileName)}";

    if (file1.Length <= 0) return Results.BadRequest("Invalid file.");
    
    doc.Path = await fileService.AddOrUpdateFileAsync(file1, fileName);

    dbContext.Docs.Update(doc);
    
    await dbContext.SaveChangesAsync();

    return Results.Ok(new { FilePath = doc.Path });

}).DisableAntiforgery();


var crudGroup = app.MapGroup("api/crud/users").WithOpenApi();


crudGroup.MapGet("/list", (PocDbContext dbContext) => dbContext.Users.OrderByDescending(b => b.DeletedAt).ToList());

crudGroup.MapPost("/create", async (PocDbContext dbContext) =>
{
    var ageRandon = new Random().Next(0, 100);
    var user = new User(Guid.NewGuid(), Time.UTC)
    {
        Age = ageRandon,
        Name = $"John Doe {ageRandon}",
    };

    dbContext.Users.Add(user);

    return await dbContext.SaveChangesAsync() > 0 ? Results.Ok(user) : Results.NotFound();
});

crudGroup.MapPut("/update/{id:guid}", async (Guid id, PocDbContext dbContext) =>
{
    var user = await dbContext.Users.FindAsync(id);

    user!.Name = $"(updated) John Doe";

    user!.SetUpdatedAt(DateTime.UtcNow);

    dbContext.Users.Update(user);

    return await dbContext.SaveChangesAsync() > 0 ? Results.Ok(user) : Results.NotFound();
});

crudGroup.MapDelete("/delete/{id:guid}", async (Guid id, PocDbContext dbContext) =>
{
    var user = await dbContext.Users.FindAsync(id);

    user!.SetDeletedAt(DateTime.UtcNow);

    dbContext.Users.Update(user);

    return await dbContext.SaveChangesAsync() > 0 ? Results.Ok(user) : Results.NotFound();
});

var crudNotesGroup = app.MapGroup("api/crud/notes").WithOpenApi();


crudNotesGroup.MapGet("/list",
    (PocDbContext dbContext) => dbContext.Notes.OrderByDescending(b => b.DeletedAt).ToList());

crudNotesGroup.MapGet("/users/list",
    (PocDbContext dbContext) => dbContext.Users.Where(u => !u.DeletedAt.HasValue).ToList());

crudNotesGroup.MapPost("/user/{userId:guid}/create", async (Guid userId, PocDbContext dbContext) =>
{
    var note = new Note(Guid.NewGuid(), Time.UTC)
    {
        Completed = false,
        Message = "new note",
        UserId = userId
    };

    dbContext.Notes.Add(note);

    return await dbContext.SaveChangesAsync() > 0 ? Results.Ok(note) : Results.NotFound();
});

crudNotesGroup.MapPut("/update/{id:guid}", async (Guid id, PocDbContext dbContext) =>
{
    var note = await dbContext.Notes.FindAsync(id);

    note!.Message = $"(updated) note";

    note!.SetUpdatedAt(DateTime.UtcNow);

    dbContext.Notes.Update(note);

    return await dbContext.SaveChangesAsync() > 0 ? Results.Ok(note) : Results.NotFound();
});

crudNotesGroup.MapPut("/completed/{id:guid}", async (Guid id, PocDbContext dbContext) =>
{
    var note = await dbContext.Notes.FindAsync(id);

    note!.Completed = !note!.Completed;

    note!.SetUpdatedAt(DateTime.UtcNow);

    dbContext.Notes.Update(note);

    return await dbContext.SaveChangesAsync() > 0 ? Results.Ok(note) : Results.NotFound();
});

crudNotesGroup.MapDelete("/delete/{id:guid}", async (Guid id, PocDbContext dbContext) =>
{
    var note = await dbContext.Notes.FindAsync(id);

    note!.Message = $"(updated) note";

    note!.SetDeletedAt(DateTime.UtcNow);

    dbContext.Notes.Update(note);

    return await dbContext.SaveChangesAsync() > 0 ? Results.Ok(note) : Results.NotFound();
});

var crudDocsGroup = app.MapGroup("api/crud/docs").WithOpenApi();


crudDocsGroup.MapGet("/list", (PocDbContext dbContext, IWebHostEnvironment env, HttpContext httpContext) =>
{
    //
    var request = httpContext.Request;
    return dbContext.Docs.AsNoTracking().Select(d => new
    {
        d.Id,
        d.Name,
        link = $"{request.Scheme}://{request.Host}/temp/uploads-ssync/{d.FileName}"
    });
});


app.UseStaticFiles();

app.Run();