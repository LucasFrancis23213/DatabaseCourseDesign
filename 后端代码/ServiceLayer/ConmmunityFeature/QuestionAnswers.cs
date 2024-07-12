using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseProject.BusinessLogicLayer.CommunityFeatureBLL;
using SQLOperation.PublicAccess.Templates.SQLManager;
using SQLOperation.PublicAccess.Utilities;

namespace DatabaseProject.ServiceLayer.ConmmunityFeature
{
    public class QuestionAnswers
    {
        private CommunityFeatureBusiness<Questions> QuestionBusiness;
        private CommunityFeatureBusiness<Answers> AnswerBusiness;
       
        private List<string> answerList=new List<string> { "question_id", "user_id", "answer_content", "answer_date" };
        private List<string> questionList = new List<string> { "item_id","user_id","question_content","question_time"};

        // 构造函数
        public QuestionAnswers(Connection connection)
        {
            QuestionBusiness=new CommunityFeatureBusiness<Questions>(connection);
            AnswerBusiness=new CommunityFeatureBusiness<Answers>(connection);
        }

        // 根据物品id获取问题列表 
        public List<Questions> GetQuestionsByItemId(int itemId)
        {
            try
            {
                var result = QuestionBusiness.QueryBusiness(new Dictionary<string, object> { { "item_id", itemId } });
                return result ?? new List<Questions>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching questions for item ID {itemId}: {ex.Message}");
                throw new Exception($"Error fetching questions for item ID {itemId}: {ex.Message}");
            }
        }

        // 根据问题id获取回答列表
        public List<Answers> GetAnswersByQuestionId(int questionId)
        {
            try
            {
                var result = AnswerBusiness.QueryBusiness(new Dictionary<string, object> { { "question_id", questionId } });
                return result ?? new List<Answers>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting answers by question id: {ex.Message}");
                throw new Exception($"Error getting answers by question id: {ex.Message}");
            }
        }

        // 根据问题内容和相关物品id进行提问操作 返回问题id
        public int PostQuestion(string questionContent, int itemId,int userId)
        {
            try
            {
                Questions newQuestion = QuestionBusiness.PackageData(0, itemId, userId, questionContent, DateTime.Now);
                var result = QuestionBusiness.AddBusiness(questionList, "question_id", newQuestion);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error posting question: {ex.Message}");
                throw new Exception($"Error posting question: {ex.Message}");
            }
        }

        // 根据问题id和当前用户撤回问题
        public bool WithdrawQuestion(int questionId,int userId)
        {
            try
            {
                var result = QuestionBusiness.RemoveBusiness(new Dictionary<string, object> { { "question_id", questionId }, { "user_id", userId } });
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error withdrawing question: {ex.Message}");
                throw new Exception($"Error withdrawing question: {ex.Message}");
            }
        }

        // 根据问题id查询问题
        public Questions GetQuestionById(int questionId)
        {
            try
            {
                var result = QuestionBusiness.QueryBusiness(new Dictionary<string, object> { { "question_id", questionId } });
                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting question by id: {ex.Message}");
                throw new Exception($"Error getting question by id: {ex.Message}");
            }
        }

        // 根据问题id和回答内容回答问题
        public int PostAnswer(int questionId, string answerContent, int userId)
        {
            try
            {
                Answers newAnswer = AnswerBusiness.PackageData(0, questionId, userId, answerContent, DateTime.Now);
                var result = AnswerBusiness.AddBusiness(answerList, "answer_id", newAnswer);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error posting answer: {ex.Message}");
                throw new Exception($"Error posting answer: {ex.Message}");
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
                Console.WriteLine($"Error withdrawing answer: {ex.Message}");
                throw new Exception($"Error withdrawing answer: {ex.Message}");
            }
        }

        // 根据回答id查询回答
        public Answers GetAnswerById(int answerId)
        {
            try
            {
                var result = AnswerBusiness.QueryBusiness(new Dictionary<string, object> { { "answer_id", answerId } });
                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting answer by id: {ex.Message}");
                throw new Exception($"Error getting answer by id: {ex.Message}");
            }
        }
    }
}

