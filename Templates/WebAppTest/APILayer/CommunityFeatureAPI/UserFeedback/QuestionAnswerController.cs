using DatabaseProject.ServiceLayer.ConmmunityFeature;
using Microsoft.AspNetCore.Mvc;
using SQLOperation.PublicAccess.Templates.SQLManager;
using SQLOperation.PublicAccess.Utilities;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Transactions;
using DatabaseProject.BusinessLogicLayer.ServiceLayer.ConmmunityFeature;
using WebAppTest.APILayer.CommunityFeatureAPI;


namespace DatabaseProject.APILayer.CommunityFeatureAPI
{

    [Route("api/")]
    [ApiController]
    public class QuestionAnswerController : ControllerBase
    {
        private QuestionAnswers questionAnswers;
        private UserActivity userActivity;
        private Connection QAConnection;

        public QuestionAnswerController(Connection connection)
        {
            questionAnswers = new QuestionAnswers(connection);
            userActivity = new UserActivity(connection);
            QAConnection = connection;
        }

        //1 `/api/questions`
        // 获取问题列表和回答列表
        [HttpPost("questions")]
        public IActionResult GetQuestions([FromBody] Dictionary<string, JsonElement> requestData)
        {
            try
            {
                // 检查请求中是否包含必要的参数 item_id
                if (requestData == null || !requestData.ContainsKey("item_id"))
                {
                    return BadRequest("缺少必需的参数：item_id");

                }

                // 使用 GetInt32 方法直接获取整数类型的值
                string itemId = ControllerHelper.GetSafeString(requestData, "item_id");

                // 调用 QuestionAnswers 类中的方法获取问题列表
                var questions = questionAnswers.GetQuestionsByItemId(itemId);


                // 构建问题列表的响应对象
                var response = new List<object>();

                foreach (var question in questions)
                {
                    // 查询该问题的回答列表
                    //var answers = questionAnswers.GetAnswersByQuestionId(question.Item2.Question_ID);
                    // 如果 answers 为空，则将 answers 设置为一个空列表
                    //var answersList = answers ?? new List<Tuple<Users, Answers>>(); // 假设 Answer 是回答对象的类名，根据实际情况修改

                    // 构建单个问题的对象
                    var questionObj = new
                    {
                        id = question.Item2.Question_ID, // 问题ID
                        user = new
                        {
                            id = question.Item1.User_ID, // 用户ID
                                                         // 待定 暂时没有获取用户名和用户头像的方法
                            name = question.Item1.User_Name, // 用户名
                            avatar = question.Item1.Avatar, // 用户头像URL
                        },
                        content = question.Item2.Question_Content, // 问题内容
                        time = question.Item2.Question_Time.ToLocalTime(), // 提出问题时间
//                        answers = answersList.Select(a => new
//                        {
//                            id = a.Item2.Answer_ID, // 回答ID
//                            user = new
//                            {
//                                id = a.Item1.User_ID,
//                                name = a.Item1.User_Name, // 这里需要从用户信息中获取
//                                avatar = question.Item1.Avatar, // 这里需要从用户信息中获取
//                            },
//                            content = a.Item2.Answer_Content, // 回答内容
//                            time = a.Item2.Answer_Date.ToLocalTime() // 回答时间
//                        }).ToList()
                    };
                    // 添加到响应列表中
                    response.Add(questionObj);
                }

                // 构建带状态的响应对象
                var responseObject = new
                {
                    status = "success",
                    questions = response // 包含问题列表的响应对象
                };
                Console.WriteLine(responseObject);
                // 返回状态+questions
                return Ok(responseObject);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取问题时出错：{ex.Message}");
                return BadRequest(new { status = "error", message = $"{ex.Message}" });

            }


        }


        // POST api/questions/{question_id}/answers
        [HttpPost("questions/{question_id}/answers")]
        public IActionResult GetQuestionAnswers(int question_id)
        {
            try
            {
                // 调用 QuestionAnswers 类中的方法获取回答列表
                int questionId = Convert.ToInt32(question_id);

                var answers = questionAnswers.GetAnswersByQuestionId(questionId);


                // 构建回答列表的响应对象
                var response = new
                {
                    status = "success",
                    answers_count = answers.Count,
                    answers = answers.Select(a => new
                    {
                        id = a.Item2.Answer_ID,
                        user = new
                        {
                            id = a.Item1.User_ID,
                            name = a.Item1.User_Name, // 这里需要从用户信息中获取
                            avatar = a.Item1.Avatar // 这里需要从用户信息中获取
                        },
                        content = a.Item2.Answer_Content,
                        time = a.Item2.Answer_Date.ToLocalTime()
                    }).ToList()
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取答案时出错：{ex.Message}");
                return BadRequest(new { status = "error", message = $"{ex.Message}" });

            }
        }

        // POST: /api/post_questions 
        // 提出问题
        [HttpPost("post_questions")]
        public IActionResult PostQuestion([FromBody] Dictionary<string, JsonElement> requestData)
        {
            using (var transaction = QAConnection.GetOracleConnection().BeginTransaction())
            {
                try
                {

                    // 检查请求中是否包含必要的参数 content, item_id, current_user_id 和 time
                    if (requestData == null ||
                    !requestData.ContainsKey("content") ||
                    !requestData.ContainsKey("item_id") ||
                    !requestData.ContainsKey("current_user_id") ||
                    !requestData.ContainsKey("time"))
                    {
                        return BadRequest("缺少必需的参数：content、item_id、current_user_id 或 time");

                    }

                    // 获取 content, item_id 和 current_user_id 参数
                    string content = ControllerHelper.GetSafeString(requestData, "content");
                    string itemId = ControllerHelper.GetSafeString(requestData, "item_id");
                    int userId = requestData["current_user_id"].GetInt32();
                    DateTime time = requestData["time"].GetDateTime();

                    // 调用 QuestionAnswers 类中的方法提问
                    int questionId = questionAnswers.PostQuestion(content, itemId, userId, time);
                    userActivity.AddUserActivity(userId, "问答", time);
                    // 构建响应对象
                    var response = new
                    {
                        status = "success",
                        question_id = questionId,

                    };

                    transaction.Commit();
                    return Ok(response);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine($"发布问题时出错: {ex.Message}");
                    return BadRequest(new { status = "error", message = $"{ex.Message}" });

                }

            }

        }


        // 撤回提问
        // POST: /api/questions_retract/{question_id}

        // 添加一个无效撤回时报错

        [HttpPost("questions_retract/{question_id}")]
        public IActionResult WithdrawQuestion(int question_id, [FromBody] Dictionary<string, JsonElement> requestData)
        {
            try
            {
                // 检查请求中是否包含必要的参数 current_user_id
                if (requestData == null || !requestData.ContainsKey("current_user_id"))
                {
                    return BadRequest("缺少必需的参数：current_user_id");

                }

                // 将 question_id 转换为整数类型
                int questionId = question_id;
                int userId = requestData["current_user_id"].GetInt32();



                // 调用 QuestionAnswers 类中的方法撤回问题
                bool success = questionAnswers.WithdrawQuestion(questionId, userId);

                if (success)
                {
                    // 构建成功响应对象
                    var response = new
                    {
                        status = "success"
                    };

                    return Ok(response);
                }
                else
                {
                    return NotFound(); // 如果撤回操作失败，返回未找到资源状态码
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"撤回问题时出错: {ex.Message}");
                return BadRequest(new { status = "error", message = $"{ex.Message}" });

            }
        }

        // 回答问题
        // POST: /api/questions/{question_id}/post_answers
        [HttpPost("questions/{question_id}/post_answers")]
        public IActionResult PostAnswer(int question_id, [FromBody] Dictionary<string, JsonElement> requestData)
        {
            using (var transaction = QAConnection.GetOracleConnection().BeginTransaction())
            {
                try
                {
                    // 检查请求中是否包含必要的参数 content, current_user_id 和 time
                    if (requestData == null ||
                        !requestData.ContainsKey("content") ||
                        !requestData.ContainsKey("current_user_id") ||
                        !requestData.ContainsKey("time"))
                    {
                        return BadRequest("缺少必填参数：content、current_user_id 或 time");

                    }

                    // 获取当前用户ID
                    int userId = requestData["current_user_id"].GetInt32();

                    // 获取回答内容
                    string answerContent = ControllerHelper.GetSafeString(requestData, "content");

                    // 获取回答时间
                    DateTime time = requestData["time"].GetDateTime();

                    // 调用 QuestionAnswers 类中的方法回答问题
                    int answerId = questionAnswers.PostAnswer(question_id, answerContent, userId, time);

                    // 如果回答ID为0，则表示回答操作失败
                    if (answerId == 0)
                    {
                        throw new Exception("回答问题出错");

                    }
                    userActivity.AddUserActivity(userId, "问答", time);
                    // 构建成功响应对象
                    var response = new
                    {
                        status = "success",
                        answer_id = answerId,

                    };
                    transaction.Commit();
                    return Ok(response);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine($"发布答案时出错: {ex.Message}");
                    return BadRequest(new { status = "error", message = $"{ex.Message}" });

                }
            }

        }

        // 撤回回答
        // POST: /api/answers_retract/{answer_id}
        // 撤回无效时报错
        [HttpPost("answers_retract/{answer_id}")]
        public IActionResult RetractAnswer(int answer_id, [FromBody] Dictionary<string, JsonElement> requestData)
        {
            try
            {
                // 检查请求中是否包含必要的参数 answer_id 和 current_user_id
                if (requestData == null || !requestData.ContainsKey("current_user_id"))
                {
                    return BadRequest("缺少必需的参数: current_user_id");

                }


                // 将 answer_id 和 current_user_id 转换为对应类型
                int answerId = answer_id;
                int userId = requestData["current_user_id"].GetInt32();

                // 调用 QuestionAnswers 类中的方法撤回回答
                bool result = questionAnswers.WithdrawAnswer(answerId, userId);

                // 如果撤回操作失败，则返回内部服务器错误
                if (!result)
                {
                    return StatusCode(500, "撤回答案时发生错误");

                }

                // 构建成功响应对象
                var response = new
                {
                    status = "success",

                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"撤回答案时发生错误: {ex.Message}");
                return BadRequest(new { status = "error", message = $"{ex.Message}" });

            }
        }

    }
}