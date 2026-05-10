using VitalSyncAPI.Domain.Exceptions;

namespace VitalSyncAPI.Domain.ValueObjects
{
    public class Password
    {
        public string Value { get; }

        public Password(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ValidationException("Senha é obrigatória.");

            if(password.Length < 6)
                throw new ValidationException("A senha deve ter pelo menos 6 caracteres.");

            
            Value = password;
        }

        public override bool Equals(object? obj)
        {
            return obj is Password other && Value == other.Value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value);
        }

        public override string ToString() => Value;
    }
}