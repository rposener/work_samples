namespace copilot_test.Data
{
    public class User
    {
        public required Guid Id { get; init; }
        public required string Email { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }

        public User(string email, string firstName, string lastName)
        {
            Id = Guid.NewGuid();
            Email = email;
            FirstName = firstName;
            LastName = lastName;
        }
    }
}
