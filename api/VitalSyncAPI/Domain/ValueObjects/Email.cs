using System.Text.RegularExpressions;
using VitalSyncAPI.Domain.Exceptions;

namespace VitalSyncAPI.Domain.ValueObjects
{
    public class Email
    {
        private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        public string Value { get; }

        public Email(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ValidationException("Email é obrigatório.");

            if (!EmailRegex.IsMatch(email))
                throw new ValidationException("Email inválido.");

            Value = email.ToLowerInvariant();
        }

        public override bool Equals(object? obj)
        {
            return obj is Email other && Value == other.Value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value);
        }

        public override string ToString() => Value;
    }
}