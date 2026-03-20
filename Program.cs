using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>() // <-- THIS TURNS ON ROLES
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    // 1. Create role if it doesn't exist
    if (!await roleManager.RoleExistsAsync("InventoryManager"))
    {
        await roleManager.CreateAsync(new IdentityRole("InventoryManager"));
    }

    // 2. List of emails that should get this role
    var managerEmails = new List<string>
    {
        "jainam121005@gmail.com",
        "jainam121005@outlook.com",
        "sagar.shah@example.com"
    };

    // 3. Assign role to each email
    foreach (var email in managerEmails)
    {
        var user = await userManager.FindByEmailAsync(email);

        if (user != null && !await userManager.IsInRoleAsync(user, "InventoryManager"))
        {
            await userManager.AddToRoleAsync(user, "InventoryManager");
        }
    }
}

app.Run();
