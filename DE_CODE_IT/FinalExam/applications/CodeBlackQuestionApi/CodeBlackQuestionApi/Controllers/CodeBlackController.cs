using CodeBlackQuestionApi.DataAccess;
using CodeBlackQuestionApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Reflection;

namespace CodeBlackQuestionAPI.Controllers
{
    [ApiController]
    [Route("apis/[controller]")]
    public class CodeBlackController : Controller
    {
        public ICodeBlackDao? Dao { get; set; }


        public CodeBlackController(IConfiguration configuration)
        {
            this.Dao = new CodeBlackDao(configuration);
        }

        #region Gets
        /// <summary>
        /// Get a collection of customers
        /// </summary>
        /// <returns></returns>
        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<CustomerModel>>> GetCustomers(string? firstName, string? lastName)
        {
            try
            {
                var model = new CustomerModel() { FirstName = firstName, LastName = lastName };
                var data = await Dao.GetCustomersAsync().ConfigureAwait(false);

                return Ok(data);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        /// <summary>
        /// Get a collection of customers
        /// </summary>
        /// <returns></returns>
        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<CustomerModel>>> GetCustomer(string? firstName, string? lastName)
        {
            try
            {
                var model = new CustomerModel() { FirstName = firstName, LastName = lastName };
                var data = await Dao.GetCustomerAsync(model).ConfigureAwait(false);

                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        /// <summary>
        /// Gets a collection of questions
        /// </summary>
        /// <returns></returns>
        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<QuestionaireModel>>> GetQuestions()
        {
            try
            {
                var data = await Dao.GetAllQuestionsAsync().ConfigureAwait(false);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Gets a collection of surveys
        /// </summary>
        /// <returns></returns>
        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<SurveyModel>>> GetSurvey(string? firstName, string? lastName)
        {
            try
            {
                var data = await Dao.GetSurveyInfoAsync(firstName, lastName);

                return Ok(data);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }

        #endregion

        #region Puts

        /// <summary>
        /// Insert a New Customer
        /// </summary>
        /// <returns></returns>
        [HttpPut("[action]")]
        public async Task<ActionResult<IEnumerable<CustomerModel>>> InsertNewCustomer(CustomerModel model)
        {
            try
            {
                var data = await Dao.InsertCustomerAsync(model);

                return Ok(data);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        /// <summary>
        /// Inserts a new question into the question table
        /// </summary>
        /// <returns></returns>
        [HttpPut("[action]")]
        public async Task<ActionResult<IEnumerable<QuestionaireModel>>> InsertQuestion(QuestionaireModel model)
        {
            try
            {
                var data = await Dao.InsertQuestionAsync(model).ConfigureAwait(false);

                return Ok(data);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Inserts a new survey into the database.
        /// </summary>
        /// <returns></returns>
        [HttpPut("[action]")]
        public async Task<ActionResult<IEnumerable<SurveyModel>>> InsertNewSurvey(string? firstName, string? lastName, string? address, string? city, string? state,
            string? zip, int Q1, string? a1, int Q2, string? a2, int Q3, string? a3, string? email = "")
        {
            try
            {
                var data = await Dao.InsertSurveyAsync(firstName, lastName, address, city, state, zip, Q1, a1, Q2, a2, Q3, a3, email);

                return Ok(data);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }

        #endregion

        #region Posts

        /* Update only the Customer Info or the IsInterested flag in the Survey*/

        /// <summary>
        /// Update Existing Customer
        /// </summary>
        /// <returns>IEnumerable of Customer Models</returns>
        [HttpPost("[action]")]
        public async Task<ActionResult<IEnumerable<CustomerModel>>> UpdateCustomer(CustomerModel model)
        {
            try
            {
                var data = await Dao.UpdateExistingCustomerAsync(model);

                return Ok(data);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        /// <summary>
        /// Update Existing Customer
        /// </summary>
        /// <returns>IEnumerable of Customer Models</returns>
        [HttpPost("[action]")]
        public async Task<ActionResult<IEnumerable<CustomerModel>>> UpdateSurvey(string? firstName, string? lastName, bool isInterested)
        {
            try
            {
                var data = await Dao.UpdateExistingSurveyAsync(firstName, lastName, isInterested).ConfigureAwait(false);

                return Ok(data);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        #endregion


        #region Deletes

        /// <summary>
        /// Removes a single customer from the database. Also removes all surveys completed by customer as to not
        /// orphan rows in the database.
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <returns></returns>
        [HttpDelete("[action]")]
        public async Task<ActionResult<int>> DeleteCustomer(string? firstName, string? lastName)
        {
            try
            {
                var data = await Dao.DeleteCustomerAsync(firstName, lastName).ConfigureAwait(false);

                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        /// <summary>
        /// Deletes last survey from the database
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <returns>0</returns>
        [HttpDelete("[action]")]
        public async Task<ActionResult<int>> DeleteSurvey(string? firstName, string? lastName)
        {
            try
            {
                var data = await Dao.DeleteSurveyAsync(firstName, lastName).ConfigureAwait(false);

                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion
    }
}