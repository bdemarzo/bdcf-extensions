using Bdcf.Extensions.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System.ComponentModel.DataAnnotations;

namespace Bdcf.Extensions.Web.Razor.Tests;

public class Html5ValidationTagHelperLogicTests
{
	private Html5ValidationTagHelperLogic _logic = new Html5ValidationTagHelperLogic();

	private ModelMetadata CreateMetadata(Type type, params object[] validatorAttributes)
	{
		var identity = ModelMetadataIdentity.ForType(type);
		var metadataProvider = new EmptyModelMetadataProvider();
		var compositeProvider = new FakeCompositeMetadataDetailsProvider();
		return new CustomModelMetadata(metadataProvider, compositeProvider, identity, type, validatorAttributes.ToList());
	}

	private class FakeCompositeMetadataDetailsProvider : ICompositeMetadataDetailsProvider
	{
		public void CreateBindingMetadata(BindingMetadataProviderContext context) { }
		public void CreateDisplayMetadata(DisplayMetadataProviderContext context) { }
		public void CreateValidationMetadata(ValidationMetadataProviderContext context) { }
	}

	private class CustomModelMetadata : DefaultModelMetadata
	{
		private readonly IReadOnlyList<object> _validatorMetadata;

		public CustomModelMetadata(IModelMetadataProvider metadataProvider, ICompositeMetadataDetailsProvider compositeProvider, ModelMetadataIdentity identity, Type modelType, IReadOnlyList<object> validatorMetadata)
			: base(metadataProvider, compositeProvider, new DefaultMetadataDetails(identity, ModelAttributes.GetAttributesForType(modelType)))
		{
			ModelType = modelType;
			_validatorMetadata = validatorMetadata;
		}

		public override IReadOnlyList<object> ValidatorMetadata => _validatorMetadata;
		public new Type ModelType { get; }
	}

	[Fact]
	public void AddsRequiredAttribute()
	{
		var metadata = CreateMetadata(typeof(string), new RequiredAttribute());
		var attributes = new Dictionary<string, string>();
		_logic.ApplyValidationAttributes(metadata, null, attributes);

		Assert.Equal("required", attributes["required"]);
	}

	[Fact]
	public void AddsPatternAttribute()
	{
		var metadata = CreateMetadata(typeof(string), new RegularExpressionAttribute("^[a-z]+$"));
		var attributes = new Dictionary<string, string>();
		_logic.ApplyValidationAttributes(metadata, null, attributes);

		Assert.Equal("^[a-z]+$", attributes["pattern"]);
	}

	[Fact]
	public void AddsDecimalTypeWithPrecision()
	{
		var metadata = CreateMetadata(typeof(decimal), new DataTypeAttribute(DataType.Currency), new PrecisionAttribute(3));
		var attributes = new Dictionary<string, string>();
		_logic.ApplyValidationAttributes(metadata, null, attributes);

		Assert.Equal("number", attributes["type"]);
		Assert.Equal("0.001", attributes["step"]);
		Assert.False(attributes.ContainsKey("min"));
		Assert.False(attributes.ContainsKey("max"));
	}

	[Fact]
	public void AddsDecimalRange_WithIntegerBounds()
	{
		var metadata = CreateMetadata(typeof(decimal), new DataTypeAttribute(DataType.Currency), new RangeAttribute(0, 100));
		var attributes = new Dictionary<string, string>();
		_logic.ApplyValidationAttributes(metadata, null, attributes);

		Assert.Equal("number", attributes["type"]);
		Assert.Equal("0", attributes["min"]);
		Assert.Equal("100", attributes["max"]);
		Assert.False(attributes.ContainsKey("step"));
	}

	[Fact]
	public void AddsDecimalRange_WithDoubleBounds()
	{
		var metadata = CreateMetadata(typeof(decimal), new DataTypeAttribute(DataType.Currency), new RangeAttribute(0.5, 99.5));
		var attributes = new Dictionary<string, string>();
		_logic.ApplyValidationAttributes(metadata, null, attributes);

		Assert.Equal("number", attributes["type"]);
		Assert.Equal("0.5", attributes["min"]);
		Assert.Equal("99.5", attributes["max"]);
		Assert.False(attributes.ContainsKey("step"));
	}

	[Theory]
	[InlineData(0, null)]
	[InlineData(2, "0.01")]
	[InlineData(3, "0.001")]
	public void AddsPrecisionSteps(int decimalPlaces, string? expectedStep)
	{
		var metadata = CreateMetadata(typeof(decimal), new DataTypeAttribute(DataType.Currency), new PrecisionAttribute(decimalPlaces));
		var attributes = new Dictionary<string, string>();
		_logic.ApplyValidationAttributes(metadata, null, attributes);

		Assert.Equal("number", attributes["type"]);
		if (expectedStep is not null)
			Assert.Equal(expectedStep, attributes["step"]);
		else
			Assert.False(attributes.ContainsKey("step"));
	}

	[Fact]
	public void AddsTimeSpanAttributes_Minutes()
	{
		var time = new TimeSpan(2, 30, 0);
		var precision = new PrecisionAttribute(TimeInterval.Minutes);
		var metadata = CreateMetadata(typeof(TimeSpan), precision);
		var attributes = new Dictionary<string, string>();
		_logic.ApplyValidationAttributes(metadata, time, attributes);

		Assert.Equal("text", attributes["type"]);
		Assert.Equal("h:mm", attributes["placeholder"]);
		Assert.Equal("2:30", attributes["value"]);
	}
}
