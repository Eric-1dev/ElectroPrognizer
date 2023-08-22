using System.ComponentModel.DataAnnotations;

namespace ElectroPrognizer.FrontOffice.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class DateGreaterThanAttribute : ValidationAttribute
{
    private readonly string _dateToCompareToFieldName;

    public DateGreaterThanAttribute(string dateToCompareToFieldName)
    {
        _dateToCompareToFieldName = dateToCompareToFieldName;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var laterDate = (DateTime?)value;

        if (laterDate == null)
            return ValidationResult.Success;

        var earlierDate = (DateTime?)validationContext.ObjectType.GetProperty(_dateToCompareToFieldName).GetValue(validationContext.ObjectInstance, null);

        if (earlierDate == null)
            return ValidationResult.Success;

        return laterDate >= earlierDate
            ? ValidationResult.Success
            : new ValidationResult("Некорректный диапазон дат");
    }
}
