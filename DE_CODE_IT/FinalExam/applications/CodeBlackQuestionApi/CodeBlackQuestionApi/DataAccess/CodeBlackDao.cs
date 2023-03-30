using CodeBlackQuestionApi.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using System.Xml;

namespace CodeBlackQuestionApi.DataAccess
{
    public class CodeBlackDao : ICodeBlackDao
    {
        private readonly IConfiguration _configuration;
        private static string? _connectionString;

        public CodeBlackDao(IConfiguration configuration)
        {
            _configuration = configuration;
            //_connectionString = _configuration.GetConnectionString("DEV");
            _connectionString = _configuration.GetConnectionString("UAT"); 
        }


        #region GET (SELECTS)

        /// <summary>
        /// Lookup a single customer
        /// </summary>
        /// <param name="customer"></param>
        /// <returns>IEnumerable of CustomerModel</returns>
        public async Task<IEnumerable<CustomerModel>> GetCustomerAsync(CustomerModel customer)
        {
            try
            {
                using SqlConnection conn = new(_connectionString);
                conn.Open();
                var sql = $@"
                    SELECT * FROM [dbo].[CustomerInfo] (NOLOCK)
                    WHERE firstName = @FirstName
                    AND lastName = @LastName
                ";
                var data = await conn.QueryAsync<CustomerModel>(sql, new
                {
                    FirstName = customer.FirstName,
                    LastName = customer.LastName
                }).ConfigureAwait(false);

                return data;

            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Lookup all customer records
        /// </summary>
        /// <returns>IEnumerable of CustomerModel</returns>
        public async Task<IEnumerable<CustomerModel>> GetCustomersAsync()
        {
            try
            {
                using SqlConnection conn = new (_connectionString);
                conn.Open();
                var sql = "SELECT * FROM [dbo].[CustomerInfo]";
                var data = await conn.QueryAsync<CustomerModel>(sql).ConfigureAwait(false);
                return data;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Lookup All Questions
        /// </summary>
        /// <returns>IEnumerable of QuestionnaireModel</returns>
        public async Task<IEnumerable<QuestionaireModel>> GetAllQuestionsAsync()
        {
            try
            {
                using SqlConnection conn = new (_connectionString);
                conn.Open();
                var sql = $@"
                SELECT * FROM (
	                SELECT ROW_NUMBER() OVER(ORDER BY QUESTIONID) QuestionID, 
                    QuestionText, IsActive, QuestionAddedDate, AddedBy, QuestionDeleteDate, DeletedBy 
	                FROM (
		                SELECT * FROM Questionaire1
		                UNION ALL
		                SELECT * FROM Questionaire2
		                UNION ALL
		                SELECT * FROM Questionaire3
	                ) GetQuestions
                ) AllQuestions
                WHERE IsActive = 1
                ORDER BY QuestionID
                ";
                var data = await conn.QueryAsync<QuestionaireModel>(sql).ConfigureAwait(false);
                return data;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Lookup a specific survey by customer name
        /// </summary>
        /// <returns>IEnumerable of SurveyModel</returns>
        public async Task<IEnumerable<SurveyModel>> GetSurveyInfoAsync(string? firstName, string? lastName)
        {
            try
            {
                using SqlConnection conn = new (_connectionString);
                conn.Open();

                var customer = await this.GetCustomerAsync(new CustomerModel() { FirstName = firstName, LastName = lastName }).ConfigureAwait(false);

                var sql = $@"
                    SELECT SurveyId, CustomerID, SURVEYDATE, 
	                    Q1.QuestionText SurveyQuestion1, ANSWER1, 
	                    Q2.QuestionText SurveyQuestion2, ANSWER2,
	                    Q3.QuestionText SurveyQuestion3, ANSWER3,
	                    ISINTERESTED
                    FROM CBSurveyInfo
	                    INNER JOIN Questionaire1 Q1 ON SURVEYQUESTION1 = Q1.QuestionID
	                    INNER JOIN Questionaire2 Q2 ON SURVEYQUESTION2 = Q2.QuestionID
	                    INNER JOIN Questionaire3 Q3 ON SURVEYQUESTION3 = Q3.QuestionID
                    WHERE CustomerID = @CustomerID
                    ORDER BY SurveyId ASC
                ";
                var data = await conn.QueryAsync<SurveyModel>(sql, new { CustomerID = customer?.FirstOrDefault().CustomerId }).ConfigureAwait(false);
                return data;
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

        #region POST (INSERT)

        /// <summary>
        /// Insert a New Customer
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<CustomerModel>> InsertCustomerAsync(CustomerModel newCustomer)
        {
            try
            {
                using SqlConnection conn = new (_connectionString);
                conn.Open();
                var sql = $@"
                    INSERT INTO [dbo].[CustomerInfo]
                    (
                        [FirstName],[LastName],[Address],[City],[State],[Zipcode],[Email]
                    )
                    VALUES
                    (
                        @FirstName, @LastName, @Address, @City, @State, @Zipcode, @Email
                    )
                ";

                var data = await conn.QueryAsync<CustomerModel>
                    (
                        sql, new
                        {
                            FirstName = newCustomer.FirstName,
                            LastName = newCustomer.LastName,
                            Address = newCustomer.Address,
                            City = newCustomer.City,
                            State = newCustomer.State,
                            Zipcode = newCustomer.Zipcode,
                            Email = newCustomer.Email
                        }
                ).ConfigureAwait(false);

                var returnData = await GetCustomerAsync(newCustomer).ConfigureAwait(false);

                return returnData;

            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Insert a New Question
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<QuestionaireModel>> InsertQuestionAsync(QuestionaireModel newQuestion)
        {
            try
            {
                /*
                 * Gonna try to be a bit slick here.  We're going to have to try to look at 3 different tables, and 
                 * sequentially insert questions into each of them as to not overload one table over others.
                 */
                using SqlConnection conn = new (_connectionString);
                conn.Open();
                var sql = $@"
                    WITH Q1TotalQuestions AS (
	                    SELECT COUNT(*) Q1Quests
	                    FROM Questionaire1
                    ),
                    Q2TotalQuestions AS (
	                    SELECT COUNT(*) Q2Quests 
	                    FROM Questionaire2
                    ),
                    Q3TotalQuestions AS (
	                    SELECT COUNT(*) Q3Quests
	                    FROM Questionaire3
                    ),
                    AllQuestions As(
	                    select Q1Quests, Q2Quests, Q3Quests
	                    FROM Q1TotalQuestions
	                    OUTER APPLY(
		                    SELECT * FROM Q2TotalQuestions
	                    )q2
	                    OUTER APPLY(
		                    SELECT * FROM Q3TotalQuestions
	                    )q3
                    )

                    SELECT Q1Quests, Q2Quests, Q3Quests FROM AllQuestions
                ";

                var data = await conn.QueryAsync<QNumsModel>(sql).ConfigureAwait(false);
                var dataDict = new Dictionary<object, int>();

                dataDict.Add("Q1Quests", data.Select(x => x.Q1Quests).FirstOrDefault());
                dataDict.Add("Q2Quests", data.Select(x => x.Q2Quests).FirstOrDefault());
                dataDict.Add("Q3Quests", data.Select(x => x.Q3Quests).FirstOrDefault());

                var ordered = dataDict.OrderBy(x => x.Value).ToHashSet();

                var tableNum = ordered?.Select(x => x.Key).FirstOrDefault().ToString().Substring(1, 1);

                //Now, grab the first one (which should have the lowest number of questions and add this
                //question to the table so that they try to stay even.

                var insertSql = $@"
                INSERT INTO [dbo].[Questionaire{tableNum}] ([QuestionText],[IsActive]
                                ,[QuestionAddedDate],[AddedBy])
                VALUES(@QuestionText, 1, @QuestionAddedDate, 1)";

                var insertData = await conn.QueryAsync<QuestionaireModel>(insertSql,
                    new
                    {
                        QuestionText = newQuestion.QuestionText,
                        QuestionAddedDate = DateTime.Now.Date
                    });

                var returnData = await GetAllQuestionsAsync().ConfigureAwait(false);

                return returnData;

            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Insert a New Survey.  Inserts a new customer if no customer exists.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<SurveyModel>> InsertSurveyAsync(string? firstName, string? lastName, string? address, string? city, string? state,
            string? zip, int Q1, string? a1, int Q2, string? a2, int Q3, string? a3, string? email = "")
        {
            try
            {
                using SqlConnection conn = new (_connectionString);
                conn.Open();

                int custCustomerID = 0;

                //Do not insert a new customer if one already exists.
                var searchCustomer = await GetCustomerAsync(new CustomerModel() { FirstName = firstName, LastName = lastName }).ConfigureAwait(false);
                var custCnt = searchCustomer.ToList().Count();

                if (custCnt > 0)
                {
                    custCustomerID = searchCustomer.FirstOrDefault().CustomerId;
                }
                else
                {
                    //insert the customer info
                    var custInsertSql = $@"
                    INSERT INTO [dbo].[CustomerInfo] ([FirstName] ,[LastName] ,[Address] ,[City] ,[State] ,[Zipcode] ,[Email])
                    VALUES (@FirstName, @LastName, @Address, @City, @State, @Zipcode, @Email);                    
                ";

                    var custInsert = await conn.ExecuteAsync(custInsertSql, new
                    {
                        FirstName = firstName,
                        LastName = lastName,
                        Address = address,
                        City = city,
                        State = state,
                        Zipcode = zip,
                        Email = email,
                    }).ConfigureAwait(false);

                    //get the customer id inserted
                    custCustomerID = await conn.ExecuteScalarAsync<int>($@"SELECT TOP 1 CustomerId FROM [dbo].[CustomerInfo] ORDER BY CustomerId desc").ConfigureAwait(false);
                }

                //Now insert the survey information
                var survInsertSQL = $@"
                    INSERT INTO [dbo].[CBSurveyInfo] ([CustomerID] ,[SURVEYDATE] ,[SURVEYQUESTION1] ,[ANSWER1] ,[SURVEYQUESTION2]
                                ,[ANSWER2] ,[SURVEYQUESTION3] ,[ANSWER3], [ISINTERESTED])
                    VALUES (@CustomerID, @SurveyDate, @SurveyQuestion1, @Answer1, @SurveyQuestion2, @Answer2, @SurveyQuestion3, 
                    @Answer3, @IsInterested)
                ";

                var surveyInsert = await conn.ExecuteAsync(survInsertSQL, new
                {
                    CustomerID = custCustomerID,
                    SurveyDate = DateTime.Now.Date,
                    SurveyQuestion1 = Q1,
                    Answer1 = a1,
                    SurveyQuestion2 = Q2,
                    Answer2 = a2,
                    SurveyQuestion3 = Q3,
                    Answer3 = a3,
                    IsInterested = true
                }).ConfigureAwait(false);

                //Lastly, lookup the survey you just entered
                //get the customer id inserted
                var surveyID = await conn.ExecuteScalarAsync<int>($@"SELECT TOP 1 SurveyId FROM [dbo].[CBSurveyInfo] ORDER BY SurveyId DESC").ConfigureAwait(false);

                var returnData = await conn.QueryAsync<SurveyModel>($@"SELECT * FROM [dbo].[CBSurveyInfo] WHERE SurveyId = @SurveyID",
                    new { SurveyID = surveyID }).ConfigureAwait(false);

                return returnData;

            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

        #region PUT (UPDATE)

        public async Task<IEnumerable<CustomerModel>> UpdateExistingCustomerAsync(CustomerModel model)
        {
            try
            {
                using SqlConnection conn = new (_connectionString);
                conn.Open();

                var updateCustomer = await conn.QueryAsync<CustomerModel>($@"
                    UPDATE [dbo].[CustomerInfo]
                        SET FirstName = CASE WHEN ISNULL(@FirstName, '') = '' THEN FirstName ELSE @FirstName END,
                            LastName = CASE WHEN ISNULL(@LastName, '') = '' THEN LastName ELSE @LastName END,
                            Address = CASE WHEN ISNULL(@Address, '') = '' THEN Address ELSE @Address END,
                            City = CASE WHEN ISNULL(@City, '') = '' THEN City ELSE @City END,
                            State = CASE WHEN ISNULL(@State, '') = '' THEN State ELSE @State END,
                            Zipcode = CASE WHEN ISNULL(@Zipcode, '') = '' THEN Zipcode ELSE @Zipcode END,
                            Email = CASE WHEN ISNULL(@Email, '') = '' THEN Email ELSE @Email END
                    WHERE FirstName = @FirstName AND LastName = @LastName
                ", new
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Address = model.Address,
                    City = model.City,
                    State = model.State,
                    Zipcode = model.Zipcode,
                    Email = model.Email
                }).ConfigureAwait(false);

                var returnData = await GetCustomerAsync(model).ConfigureAwait(false);

                return returnData;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// UpdateExistingSurveyAsync - Asynchronous method that updates surveys of the individual identified.
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="isInterested"></param>
        /// <returns></returns>
        public async Task<IEnumerable<SurveyModel>> UpdateExistingSurveyAsync(string? firstName, string? lastName, bool isInterested)
        {
            try
            {
                using SqlConnection conn = new(_connectionString);
                conn.Open();

                var custInfo = new CustomerModel() { FirstName = firstName, LastName = lastName };

                var custData = await GetCustomerAsync(custInfo).ConfigureAwait(false);

                var updateSurvey = await conn.ExecuteAsync($@"
                    UPDATE [dbo].[CBSurveyInfo]
                        SET IsInterested = @IsInterested
                    WHERE CustomerID = @CustomerID
                ", new
                {
                    CustomerID = custData?.FirstOrDefault().CustomerId,
                    IsInterested = isInterested
                }).ConfigureAwait(false);

                var returnData = await GetSurveyInfoAsync(firstName, lastName).ConfigureAwait(false);

                return returnData;
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

        #region DELETE (DELETE)

        /* Deleting a customer will automatically delete any surveys the person filled out */

        /// <summary>
        /// DeleteCustomerAsync - Deletes a customer, along with any surveys that the customer filled out.
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <returns>Integer of 0</returns>
        public async Task<int> DeleteCustomerAsync(string? firstName, string? lastName)
        {
            try
            {
                using SqlConnection conn = new(_connectionString);
                conn.Open();

                var custInfo = new CustomerModel() { FirstName = firstName, LastName = lastName };

                var custData = await GetCustomerAsync(custInfo).ConfigureAwait(false);

                var deleteAll = await conn.ExecuteAsync($@"
                    DELETE FROM [dbo].[CBSurveyInfo] WHERE CustomerID = @CustomerID;
                    DELETE FROM [dbo].[CustomerInfo] WHERE CustomerID = @CustomerID;
                ", new { CustomerID = custData?.FirstOrDefault().CustomerId }).ConfigureAwait(false);

                return 0;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        /// <summary>
        /// DeleteSurveyAsync
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <returns>Integer of 0</returns>
        public async Task<int> DeleteSurveyAsync(string? firstName, string? lastName)
        {
            try
            {
                using SqlConnection conn = new(_connectionString);
                conn.Open();

                var custInfo = new CustomerModel() { FirstName = firstName, LastName = lastName };

                var custData = await GetCustomerAsync(custInfo).ConfigureAwait(false);

                var deleteAll = await conn.ExecuteAsync($@"
                    DELETE FROM [dbo].[CBSurveyInfo] WHERE CustomerID = @CustomerID;
                    DELETE FROM [dbo].[CustomerInfo] WHERE CustomerID = @CustomerID;
                ", new { CustomerID = custData?.FirstOrDefault().CustomerId }).ConfigureAwait(false);

                return 0;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        #endregion
    }
}
