﻿//public class EvenNumberAttribute : ValidationAttribute
//{
//    public PhoneNumberAttribute() : base(() => Resource1.PhoneNumberError) { }
//    public PhoneNumberAttribute(string errorMessage) : base(() => errorMessage) { }

//    protected override ValidationResult IsValid(object value, 
//        ValidationContext validationContext)
//    {
//        if (value == null)
//        {
//            return ValidationResult.Success;
//        }

//        int convertedValue;
//        try
//        {
//            convertedValue = Convert.ToInt32(value);
//        }
//        catch (FormatException)
//        {
//            return new ValidationResult(Resource1.ConversionError);
//        }

//        if (convertedValue % 2 == 0)
//        {
//            return ValidationResult.Success;
//        }
//        else
//        {
//            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
//        }
//    }