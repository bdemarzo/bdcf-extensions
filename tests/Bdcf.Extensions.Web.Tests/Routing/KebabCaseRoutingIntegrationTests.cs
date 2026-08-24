using System.Net;
using Bdcf.Extensions.Web;
using Bdcf.Extensions.Web.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Bdcf.Extensions.Web.Routing.Tests;

public class KebabCaseRoutingIntegrationTests
{
	[Fact]
	public void AddKebabCaseRouting_RegistersAttributeConventionAndNamedPolicy()
	{
		var baselineServices = new ServiceCollection();
		baselineServices.AddControllersWithViews();
		var mvcConfigureCountBefore = baselineServices.Count(descriptor => descriptor.ServiceType == typeof(IConfigureOptions<MvcOptions>));

		var services = new ServiceCollection();
		services.AddControllersWithViews().AddKebabCaseRouting();
		var mvcConfigureCountAfter = services.Count(descriptor => descriptor.ServiceType == typeof(IConfigureOptions<MvcOptions>));

		Assert.True(mvcConfigureCountAfter > mvcConfigureCountBefore);
		using var provider = services.BuildServiceProvider();

		var routeOptions = provider.GetRequiredService<IOptions<RouteOptions>>().Value;

		Assert.Equal(
			typeof(KebabCaseOutboundParameterTransformer),
			routeOptions.ConstraintMap["kebab"]);
	}

	[Fact]
	public async Task AttributeRouteTokens_AreTransformedAndExplicitLiteralsArePreserved()
	{
		using var host = CreateHost();
		using var client = host.GetTestServer().CreateClient();

		var transformedResponse = await client.GetAsync("/api/attribute-subscription-management/get-all", TestContext.Current.CancellationToken);
		var literalResponse = await client.GetAsync("/ExplicitSegment/explicit-literal/get-all", TestContext.Current.CancellationToken);

		Assert.Equal(HttpStatusCode.OK, transformedResponse.StatusCode);
		Assert.Equal("attribute", await transformedResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
		Assert.Equal(HttpStatusCode.OK, literalResponse.StatusCode);
		Assert.Equal("literal", await literalResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
	}

	[Fact]
	public async Task ConventionalRoute_MatchesKebabCaseControllerAndAction()
	{
		using var host = CreateHost();
		using var client = host.GetTestServer().CreateClient();

		var response = await client.GetAsync("/subscription-management/get-all/42", TestContext.Current.CancellationToken);

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		Assert.Equal("42|none", await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
	}

	[Fact]
	public async Task ConventionalRoute_UsesDefaultsAndOptionalId()
	{
		using var host = CreateHost();
		using var client = host.GetTestServer().CreateClient();

		var defaultResponse = await client.GetAsync("/", TestContext.Current.CancellationToken);
		var optionalIdResponse = await client.GetAsync("/subscription-management/get-all", TestContext.Current.CancellationToken);

		Assert.Equal(HttpStatusCode.OK, defaultResponse.StatusCode);
		Assert.Equal("home", await defaultResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
		Assert.Equal(HttpStatusCode.OK, optionalIdResponse.StatusCode);
		Assert.Equal("none|none", await optionalIdResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
	}

	[Fact]
	public async Task ConventionalRoute_GeneratesLinksThroughUrlActionAndLinkGenerator()
	{
		using var host = CreateHost();
		using var client = host.GetTestServer().CreateClient();

		var response = await client.GetAsync("/link-generation/get-links", TestContext.Current.CancellationToken);
		var links = (await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken)).Split('|');

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		Assert.Equal("/subscription-management/get-all/17?filter=KeepCase", links[0]);
		Assert.Equal("/subscription-management/get-all/17?filter=KeepCase", links[1]);
	}

	[Fact]
	public async Task ConventionalRoute_GeneratesLinksThroughMvcAnchorTagHelper()
	{
		using var host = CreateHost();
		using var client = host.GetTestServer().CreateClient();

		var response = await client.GetAsync("/link-generation/get-anchor", TestContext.Current.CancellationToken);

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		Assert.Equal("/subscription-management/get-all/17?filter=KeepCase", await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
	}

	private static IHost CreateHost()
	{
		var host = new HostBuilder()
			.ConfigureWebHost(webHostBuilder =>
			{
				webHostBuilder
					.UseTestServer()
					.ConfigureServices(services =>
					{
						services.AddRouting();
						services.AddControllersWithViews()
							.AddApplicationPart(typeof(SubscriptionManagementController).Assembly)
							.AddKebabCaseRouting();
					})
					.Configure(app =>
					{
						app.UseRouting();
						app.UseEndpoints(endpoints => endpoints.MapKebabCaseControllerRoute());
					});
			})
			.Build();

		host.Start();
		return host;
	}

}

public class HomeController : Controller
{
	public IActionResult Index() => Content("home");
}

public class SubscriptionManagementController : Controller
{
	public IActionResult GetAll(int? id, string? filter) => Content($"{id?.ToString() ?? "none"}|{filter ?? "none"}");
}

[Route("api/[controller]/[action]")]
public class AttributeSubscriptionManagementController : Controller
{
	[HttpGet]
	public IActionResult GetAll() => Content("attribute");
}

[Route("ExplicitSegment/[controller]/[action]")]
public class ExplicitLiteralController : Controller
{
	[HttpGet]
	public IActionResult GetAll() => Content("literal");
}

public class LinkGenerationController : Controller
{
	public IActionResult GetLinks([FromServices] LinkGenerator linkGenerator)
	{
		var values = new { id = 17, filter = "KeepCase" };
		var urlAction = Url.Action("GetAll", "SubscriptionManagement", values);
		var linkGeneratorPath = linkGenerator.GetPathByAction(HttpContext, "GetAll", "SubscriptionManagement", values);

		return Content($"{urlAction}|{linkGeneratorPath}");
	}

	public IActionResult GetAnchor([FromServices] IHtmlGenerator generator)
	{
		var tagHelper = new Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper(generator)
		{
			Action = "GetAll",
			Controller = "SubscriptionManagement",
			RouteValues = new Dictionary<string, string?>
			{
				["id"] = "17",
				["filter"] = "KeepCase"
			},
			ViewContext = new ViewContext(
				ControllerContext,
				new IntegrationTestView(),
				ViewData,
				new TempDataDictionary(HttpContext, new IntegrationTestTempDataProvider()),
				TextWriter.Null,
				new HtmlHelperOptions())
		};
		var output = new TagHelperOutput(
			"a",
			new TagHelperAttributeList(),
			(_, _) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

		tagHelper.Process(
			new TagHelperContext(new TagHelperAttributeList(), new Dictionary<object, object?>(), "test"),
			output);

		return Content(output.Attributes["href"]?.Value?.ToString() ?? string.Empty);
	}
}

internal sealed class IntegrationTestView : IView
{
	public string Path => "test";

	public Task RenderAsync(ViewContext context)
	{
		return Task.CompletedTask;
	}
}

internal sealed class IntegrationTestTempDataProvider : ITempDataProvider
{
	public IDictionary<string, object> LoadTempData(HttpContext context)
	{
		return new Dictionary<string, object>();
	}

	public void SaveTempData(HttpContext context, IDictionary<string, object> values)
	{
	}
}
