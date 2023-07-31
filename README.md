# FluantValidationDemoApp
Fluent Validation ASP.NET Core Web API 6.0
FluentValidation is a very popular .NET library for building strongly typed validation rules. It helps us use validation in very easy manner. It is a small validation library that uses a fluent interface and lambda expressions for building validation rules.
Introduction to FluentValidation
Data Validation is essential for any Application. When it comes to Validating Models, developers usually use Data Annotations. There are few issues with Data Annotations approach:
1. Validation rules are tightly coupled with Entities.
2. Add complexity to Entities/DTOs.
3. Difficult to make dynamic and conditional validations.
4. Difficult to extend and scale.
FluentValidation is a replacement for the existing validation attributes (Data Annotations). It can turn up the validation game to a new level, gives total control. It separates the validation rules and/or logic from the Entity/DTO classes.
It is a open-source library that helps you make validations clean, easy to create, and maintain. It also works on external models that you don’t have access. It makes the model classes clean and readable.

Configure Fluent Validation in ASP.NET Core
1.	NuGet: To use FluentValidation, you need to install below NuGet packages.
PM> Install-Package FluentValidation.AspNetCore
PM> Install-Package FluentValidation.DependencyInjectionExtensions

2.	Configuration: Automatic registration of validators is possible. You can make use of the FluentValidation.DependencyInjectionExtensions package which can be used to automatically find all the validators in a specific assembly using an extension method.

using FluantValidationDemoApp.Data;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDBContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));

});

builder.Services.AddControllers()
            .AddFluentValidation(v =>
            {
                v.ImplicitlyValidateChildProperties = true;
                v.ImplicitlyValidateRootCollectionElements = true;
                v.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
                
            });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

This adds FluentValidation to the pipeline for Controllers.

Validator Implemetaion
To define a set of validation rules for a particular object, you will need to create a class that inherits from AbstractValidator<T>, where T is the type of class that you wish to validate.
The validation rules themselves should be defined in the validator class’s constructor. To specify a validation rule for a particular property, call the RuleFor method, passing a lambda expression that indicates the property that you wish to validate.

using FluantValidationDemoApp.DTOs;
using FluentValidation;

namespace FluantValidationDemoApp.Validations
{
    public class CustomerValidator:AbstractValidator<CustomerDTO>
    {
        public CustomerValidator()
        {
            RuleFor(x => x.Name).NotNull().NotEmpty();
            RuleFor(x => x.Name).Length(20, 250);
            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Please specify a phone number.");
            RuleFor(x => x.Age).InclusiveBetween(18, 60);

            // Complex Properties
            RuleFor(x => x.Address).InjectValidator();

            // Other way
            //RuleFor(x => x.Address).SetValidator(new AddressValidator());

            // Collections of Complex Types
            //RuleForEach(x => x.Addresses).SetValidator(new AddressValidator());
        }
    }
}

using FluantValidationDemoApp.DTOs;
using FluentValidation;

namespace FluantValidationDemoApp.Validations
{
    public class AddressValidator: AbstractValidator<AddressDTO>
    {

        public AddressValidator()
        {
            RuleFor(x => x.State).NotNull().NotEmpty();
            RuleFor(x => x.Country).NotEmpty().WithMessage("Please specify a Country.");

            RuleFor(x => x.Postcode).NotNull();
            RuleFor(x => x.Postcode).Must(BeAValidPostcode).WithMessage("Please specify a valid postcode");
        }

        private bool BeAValidPostcode(string postcode)
        {
            return postcode.Length==6;

        }
    }
}


•	You can use the RuleForEach method to apply the same rule to multiple items in a collection.
•	You can also combine RuleForEach with SetValidator when the collection is of another complex objects.
•	RuleSets allow you to group validation rules together which can be executed together as a group whilst ignoring other rules.
•	Including Rules: You can include rules from other validators provided they validate the same type. This allows you to split rules across multiple classes and compose them together.
•	Validators can be used with any dependency injection library. To inject a validator for a specific model, you should register the validator with the service provider as IValidator<T>. services.AddScoped<IValidator<Customer, CustomerValidator>();



namespace FluantValidationDemoApp.Entites
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public string PhoneNumber { get; set; }=string.Empty;
        public bool IsAdult { get; set; }       
        public Address? Address { get; set; }
    }
}

namespace FluantValidationDemoApp.Entites
{
    public class Address
    {
        public int Id { get; set; }
        public string Line1 { get; set; } = string.Empty;
        public string Line2 { get; set; } =  string.Empty;
        public string Town { get; set; } = string.Empty;
        public string Postcode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
    }
}


3.	Usage: You don’t need to explicitly check the ModelState in controllers to see if the input is valid. The FluentValidation ASP.NET middleware will automatically find our validator, and if validation fails it will prepare the ModelState and our action will return a 400 response.

using FluantValidationDemoApp.Data;
using FluantValidationDemoApp.DTOs;
using FluantValidationDemoApp.Validations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FluantValidationDemoApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ApplicationDBContext _applicationDBContext;

        public CustomerController( ApplicationDBContext applicationDBContext)
        {
            _applicationDBContext = applicationDBContext;
        }
        [HttpPost("AddNewCustomer",Order =0)]
        public IActionResult Add(CustomerDTO customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);               
            }

            _applicationDBContext.Add(customer);
            _applicationDBContext.SaveChanges();

            return Ok();
        }

        [HttpPost("UpdateNewCustomer", Order = 1)]
        public IActionResult Update(CustomerDTO customer)
        {
            CustomerValidator validator = new CustomerValidator();
            var validationResult = validator.Validate(customer);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            _applicationDBContext.Update(customer);
            _applicationDBContext.SaveChanges();

            return Ok();
        }
    }
}


You can also explicitaly validate the models anywhere. The Validate method returns a ValidationResult object. This contains two properties:
•	IsValid - a boolean that says whether the validation suceeded.
•	Errors - a collection of ValidationFailure objects containing details about any validation failures.

FluentValidation Features and Benefits
•	Built-in Validators — ships with several built-in validators like Regular Expression, Email, Credit Card, and many more.
•	Custom Validators — There are several ways to create a custom, reusable validator.
•	Localization — provides translations for the default validation messages in several languages.
•	Test Extensions — provides some extensions that can aid with testing your validator classes.
•	Asynchronous Validation — you can define asynchronous rules, for example when working with an external API.
•	Transforming Values — you can apply a transformation to a property value prior to validation being performed against it.


