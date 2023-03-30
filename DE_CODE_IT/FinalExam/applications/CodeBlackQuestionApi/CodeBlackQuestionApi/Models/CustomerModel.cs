namespace CodeBlackQuestionApi.Models
{
    public class CustomerModel
    {
        /*
            Anjali's table
        */

        public int CustomerId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Zipcode { get; set; }
        public string? Email { get; set; }

    }
}
