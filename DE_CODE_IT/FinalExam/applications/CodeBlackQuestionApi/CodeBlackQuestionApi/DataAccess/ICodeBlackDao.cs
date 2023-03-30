using CodeBlackQuestionApi.Models;
using Microsoft.Data.SqlClient;

namespace CodeBlackQuestionApi.DataAccess
{
    public interface ICodeBlackDao
    {
        #region GET (SELECTS)
        /// <summary>
        /// Lookup a single customer
        /// </summary>
        /// <param name="customer"></param>
        /// <returns>IEnumerable of CustomerModel</returns>
        Task<IEnumerable<CustomerModel>> GetCustomerAsync(CustomerModel customer);

        /// <summary>
        /// Lookup all customer records
        /// </summary>
        /// <returns>IEnumerable of CustomerModel</returns>
        Task<IEnumerable<CustomerModel>> GetCustomersAsync();

        /// <summary>
        /// Lookup all questions
        /// </summary>
        /// <returns>IEnumerable of CustomerModel</returns>
        Task<IEnumerable<QuestionaireModel>> GetAllQuestionsAsync();

        Task<IEnumerable<SurveyModel>> GetSurveyInfoAsync(string? firstName, string? lastName);

        #endregion

        #region POST (INSERT)

        Task<IEnumerable<CustomerModel>> InsertCustomerAsync(CustomerModel newCustomer);
        Task<IEnumerable<QuestionaireModel>> InsertQuestionAsync(QuestionaireModel newQuestion);
        Task<IEnumerable<SurveyModel>> InsertSurveyAsync(string? firstName, string? lastName, string? address, string? city, string? state,
            string? zip, int Q1, string? a1, int Q2, string? a2, int Q3, string? a3, string? email = "");

        #endregion

        #region PUT (UPDATE)

        Task<IEnumerable<CustomerModel>> UpdateExistingCustomerAsync(CustomerModel model);
        Task<IEnumerable<SurveyModel>> UpdateExistingSurveyAsync(string? firstName, string? lastName, bool isInterested);

        #endregion

        #region DELETE (DELETE)

        Task<int> DeleteCustomerAsync(string? firstName, string? lastName);
        Task<int> DeleteSurveyAsync(string? firstName, string? lastName);

        #endregion
    }
}
