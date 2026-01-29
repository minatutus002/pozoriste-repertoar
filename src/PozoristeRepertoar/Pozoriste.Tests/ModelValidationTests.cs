using System.ComponentModel.DataAnnotations;
using Pozoriste.Models.Entities;

namespace Pozoriste.Tests;

public class ModelValidationTests
{
    [Fact]
    public void Glumac_PunoIme_IsRequired()
    {
        var model = new Glumac { PunoIme = string.Empty };

        var results = ValidateModel(model);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(Glumac.PunoIme)));
    }

    [Fact]
    public void Glumac_PunoIme_HasMaxLength120()
    {
        var model = new Glumac { PunoIme = new string('A', 121) };

        var results = ValidateModel(model);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(Glumac.PunoIme)));
    }

    private static List<ValidationResult> ValidateModel(object model)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(model);
        Validator.TryValidateObject(model, context, results, validateAllProperties: true);
        return results;
    }
}
