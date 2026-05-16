using System.ComponentModel.DataAnnotations;
namespace CinemaDashboard.Validations
{
    public class MinMaxCustomAttribute : ValidationAttribute
    {
        private int MinLength;
        private int MaxLength;
        public MinMaxCustomAttribute(int minLength, int maxLength){ MinLength = minLength; MaxLength = maxLength; }
        public override string FormatErrorMessage(string name) => $"The {name} must be between {MinLength} and {MaxLength} characters.";
        public override bool IsValid(object? value) => value is string name && name.Length >= MinLength && name.Length <= MaxLength;
    }
}
