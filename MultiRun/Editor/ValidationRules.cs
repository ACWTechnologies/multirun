using System.Globalization;
using System.Windows.Controls;

namespace MultiRun.Editor
{
    public class TextBoxRequiredRule : ValidationRule
    {
        public string FieldName { get; set; }

        public bool WhitespaceAllowed { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var s = (string)value;
            if (WhitespaceAllowed)
            {
                if (s?.Length < 1)
                {
                    return new ValidationResult(false, FieldName + " is required.");
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(s))
                {
                    return new ValidationResult(false, FieldName + " is required.");
                }
            }

            return new ValidationResult(true, null);
        }
    }
}