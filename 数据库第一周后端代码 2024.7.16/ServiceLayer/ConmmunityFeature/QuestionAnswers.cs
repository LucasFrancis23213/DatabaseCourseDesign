using DatabaseProject.BusinessLogicLayer.CommunityFeatureBLL;
using Oracle.ManagedDataAccess.Client;
using SQLOperation.PublicAccess.Templates.SQLManager;
using SQLOperation.PublicAccess.Utilities;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;

namespace DatabaseProject.ServiceLayer.ConmmunityFeature
{
    public class QuestionAnswers
    {
        private CommunityFeatureBusiness<Questions> QuestionBusiness;
        private CommunityFeatureBusiness<Answers> AnswerBusiness;
        private CommunityFeatureBusiness<Users> UserBusiness;


        private List<string> answerList = new List<string> { "question_id", "user_id", "answer_content", "answer_date" };
        private List<string> questionList = new List<string> { "item_id", "user_id", "question_content", "question_time" };

        // 构造函数
        public QuestionAnswers(Connection connection)
        {
            QuestionBusiness = new CommunityFeatureBusiness<Questions>(connection);
            AnswerBusiness = new CommunityFeatureBusiness<Answers>(connection);

            UserBusiness = new CommunityFeatureBusiness<Users>(connection);

        }

        // 根据物品id获取 users和questions
        public List<Tuple<Users, Questions>> GetQuestionsByItemId(int itemId)
        {
            try
            {
                // 定义 FROM 子句
                string fromClause = "QUESTIONS NATURAL JOIN USERS";

                // 定义 WHERE 子句
                string whereClause = "ITEM_ID = :itemId";

                // 定义参数
                OracleParameter[] parameters = new OracleParameter[]
                {
                    new OracleParameter(":itemId", itemId)
                };

                
                // 调用查询方法
                List<Dictionary<string, object>> rowList = QuestionBusiness.QueryTableWithFromAndWhereBusiness(fromClause, whereClause, parameters);


                List<Tuple<Users, Questions>> result = new List<Tuple<Users, Questions>>();


                foreach (var row in rowList)
                {
                    // 将数据映射到 Users 和 Questions 对象
                    Users user = UserBusiness.MapDictionaryToObject(row);
                    Questions question = QuestionBusiness.MapDictionaryToObject(row);

                    result.Add(new Tuple<Users, Questions>(user, question));
                }


                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取物品 ID {itemId} 的问题列表时出错：{ex.Message}");
                throw new Exception($"获取物品 ID {itemId} 的问题列表时出错：{ex.Message}");
            }
        }


        // 限制查询条目获取回答列表
        public List<Answers> GetAnswersLimitedByQuestionId(int questionId,int number)
        {
            try
            {
                // 构建带有位置参数的查询语句
                string whereClause = $"(QUESTION_ID = :questionId) AND ROWNUM <= :LimitNumber";
                // 使用参数化查询
                OracleParameter[] parameters = 
                {
                    new OracleParameter(":questionId", questionId),
                    new OracleParameter(":LimitNumber", number)
                };

                // 调用 QueryTableWithWhere 方法执行查询
                List<Answers> queryResult = AnswerBusiness.QueryTableWithWhereBusiness(whereClause, parameters);

                return queryResult ?? new List<Answers>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"根据问题 ID 获取有限回答列表时出错：{ex.Message}");
                throw new Exception($"根据问题 ID 获取有限回答列表时出错：{ex.Message}");
            }
        }


        // 根据问题id获取回答列表
        public List<Tuple<Users, Answers>> GetAnswersByQuestionId(int questionId)
        {
            try
            {
                // 定义 FROM 子句
                string fromClause = "ANSWERS NATURAL JOIN USERS";

                // 定义 WHERE 子句
                string whereClause = "QUESTION_ID = :questionId";

                // 定义参数
                OracleParameter[] parameters = new OracleParameter[]
                {
                    new OracleParameter(":questionId", questionId)
                };

                // 调用查询方法
                List<Dictionary<string, object>> rowList = AnswerBusiness.QueryTableWithFromAndWhereBusiness(fromClause, whereClause, parameters);

                List<Tuple<Users, Answers>> result = new List<Tuple<Users, Answers>>();

                foreach (var row in rowList)
                {
                    // 将数据映射到 Users 和 Answers 对象
                    Users user = UserBusiness.MapDictionaryToObject(row);
                    Answers answer = AnswerBusiness.MapDictionaryToObject(row);

                    result.Add(new Tuple<Users, Answers>(user, answer));
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"根据问题 ID {questionId} 获取回答列表时出错：{ex.Message}");
                throw new Exception($"根据问题 ID {questionId} 获取回答列表时出错：{ex.Message}");
            }
        }

        // 根据问题内容和相关物品id进行提问操作 返回问题id
        public int PostQuestion(string questionContent, int itemId, int userId, DateTime time)
        {
            try
            {
                Questions newQuestion = QuestionBusiness.PackageData(0, itemId, userId, questionContent, time);
                var result = QuestionBusiness.AddBusiness(questionList, "question_id", newQuestion);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发布问题时出错：{ex.Message}");
                throw new Exception($"发布问题时出错：{ex.Message}");
            }
        }

        // 根据问题id和当前用户撤回问题
        public bool WithdrawQuestion(int questionId, int userId)
        {
            try
            {
                var result = QuestionBusiness.RemoveBusiness(new Dictionary<string, object> { { "question_id", questionId }, { "user_id", userId } });
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"撤回问题时出错：{ex.Message}");
                throw new Exception($"撤回问题时出错：{ex.Message}");

            }
        }

        // 根据问题id查询问题
        public Questions GetQuestionById(int questionId)
        {
            try
            {
                var result = QuestionBusiness.QueryBusiness(new Dictionary<string, object> { { "question_id", questionId } }, "AND");
                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"根据问题 ID 获取问题时出错：{ex.Message}");
                throw new Exception($"根据问题 ID 获取问题时出错：{ex.Message}");

            }
        }

        // 根据问题id和回答内容回答问题
        public int PostAnswer(int questionId, string answerContent, int userId,DateTime time)
        {
            try
            {
                Answers newAnswer = AnswerBusiness.PackageData(0, questionId, userId, answerContent, time);
                var result = AnswerBusiness.AddBusiness(answerList, "answer_id", newAnswer);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发布回答时出错：{ex.Message}");
                throw new Exception($"发布回答时出错：{ex.Message}");

            }
        }

        // 根据回答ID撤回回答
        public bool WithdrawAnswer(int answerId, int userId)
        {
            try
            {
                var result = AnswerBusiness.RemoveBusiness(new Dictionary<string, object> { { "answer_id", answerId }, { "user_id", userId } });
                
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"撤回回答时出错：{ex.Message}");
                throw new Exception($"撤回回答时出错：{ex.Message}");

            }
        }

        // 根据回答id查询回答
        public Answers GetAnswerById(int answerId)
        {
            try
            {
                var result = AnswerBusiness.QueryBusiness(new Dictionary<string, object> { { "answer_id", answerId } }, "AND");
                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"根据回答 ID 获取回答时出错：{ex.Message}");
                throw new Exception($"根据回答 ID 获取回答时出错：{ex.Message}");

            }
        }
    }
}

