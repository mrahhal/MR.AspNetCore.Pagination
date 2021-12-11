using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMvc();

builder.Services.AddPagination(c =>
{
	c.PageQueryParameterName = "p";
});

builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
	c.CustomSchemaIds(x => x.FullName);

	c.TagActionsBy(api => new[] { api.RelativePath });

	c.ConfigurePagination();
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSwagger();

app.UseSwaggerUI(c =>
{
	c.SwaggerEndpoint("/swagger/v1/swagger.json", "Api");
	c.OAuthClientId("swagger");
});

app.UseEndpoints(endpoints =>
{
	endpoints.MapControllers();
});

app.Run();
