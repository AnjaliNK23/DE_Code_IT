namespace CodeBlackQuestionApi.Models
{
    public class SurveyModel
    {
        /*
            Erika, Tim, and Lyndon's tables
        */

        public int CustomerID { get; set; }
        public string? SurveyQuestion1 { get; set; }
        public string? Answer1 { get; set; }
        public string? SurveyQuestion2 { get; set; }
        public string? Answer2 { get; set; }
        public string? SurveyQuestion3 { get; set; }
        public string? Answer3 { get; set; }
        public bool IsInterested { get; set; }
    }
}
