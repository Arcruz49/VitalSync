namespace VitalSyncAPI.Domain.ValueObjects
{
    public class Password
    {
        public string Value { get; }

        public Password(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Senha é obrigatória.");

            if(password.Length < 6)
                throw new ArgumentException("A senha deve ter pelo menos 6 caractéres");

            
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