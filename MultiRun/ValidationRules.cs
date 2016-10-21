using System.Globalization;
using System.Windows.Controls;

namespace MultiRun.Editor
{
    public class TextBoxRequiredRule : ValidationRule
    {
        private string _fieldName;
        private bool _whitespaceAllowed;

        public string FieldName
        {
            get { return _fieldName; }
            set { _fieldName = value; }
        }

        public bool WhitespaceAllowed
        {
            get { return _whitespaceAllowed; }
            set { _whitespaceAllowed = value; }
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string s = (string)value;
            if (WhitespaceAllowed)
            {
                if (s.Length < 1)
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