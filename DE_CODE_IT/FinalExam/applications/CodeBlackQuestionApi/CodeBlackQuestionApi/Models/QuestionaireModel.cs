namespace CodeBlackQuestionApi.Models
{
    public class QuestionaireModel
    {
        /*
            Erika, Lyndon, and Tim's Tables        
        */
        public int QuestionID { get; set; }
        public string? QuestionText { get; set; }
        public bool IsActive { get; set; }
        public DateTime QuestionAddedDate { get; set; }
        public string? AddedBy { get; set; }
        public DateTime QuestionDeletedDate { get; set; }
        public bool DeletedBy { get; set; }

    }
}
