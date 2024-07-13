using DatabaseProject.ServiceLayer.ConmmunityFeature;
using Microsoft.AspNetCore.Mvc;
using SQLOperation.PublicAccess.Utilities;
using System.Transactions;


namespace DatabaseProject.APILayer.CommunityFeatureAPI
{
    [Route("api/")]
    [ApiController]
    public class QuestionAnswerController : ControllerBase
    {
        private readonly QuestionAnswers questionAnswers;

        public QuestionAnswerController(QuestionAnswers q_a)
        {
            questionAnswers = q_a;
        }

        //1 `/api/questions`

        [HttpPost("questions")]
        public IActionResult GetQuestions([FromBody] Dictionary<string, object> requestData)
        {
            try
            {
                // 检查请求中是否包含必要的参数 item_id
                if (requestData == null || !requestData.ContainsKey("item_id"))
                {
                    return BadRequest("Missing required parameter: item_id");
                }

                // 获取 item_id 参数
                int itemId = Convert.ToInt32(requestData["item_id"]);

                // 调用 QuestionAnswers 类中的方法获取问题列表
                var questions = questionAnswers.GetQuestionsByItemId(itemId);

                // 如果 questions 为空，则返回空列表但状态为成功
                if (questions == null)
                {
                    return Ok(new { status = "success", questions = new List<object>() });
                }

                // 构建问题列表的响应对象
                var response = new List<object>();

                foreach (var question in questions)
                {
                    // 查询该问题的回答列表
                    var answers = questionAnswers.GetAnswersLimitedByQuestionId(question.Question_ID, 3);
                    // 如果 answers 为空，则将 answers 设置为一个空列表
                    var answersList = answers ?? new List<Answers>(); // 假设 Answer 是回答对象的类名，根据实际情况修改

                    // 构建单个问题的对象
                    var questionObj = new
                    {
                        id = question.Question_ID, // 问题ID
                        user = new
                        {
                            id = question.User_ID, // 用户ID
                                                   // 待定 暂时没有获取用户名和用户头像的方法
                            name = "default", // 用户名
                            avatar = "" // 用户头像URL
                        },
                        content = question.Question_Content, // 问题内容
                        time = question.Question_Time, // 提出问题时间
                        answers = answersList.Select(a => new
                        {
                            id = a.Answer_ID, // 回答ID
                            content = a.Answer_Content, // 回答内容
                            time = a.Answer_Date // 回答时间
                        }).ToList()
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


                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching questions: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }


        }

        // POST api/questions/{question_id}/answers
        [HttpPost("questions/{question_id}/answers")]
        public IActionResult GetAnswers(string question_id)
        {
            try
            {
                // 调用 QuestionAnswers 类中的方法获取回答列表
                int questionId = Convert.ToInt32(question_id);
                var answers = questionAnswers.GetAnswersByQuestionId(questionId);
                answers=answers ?? new List<Answers>();
                
                // 构建回答列表的响应对象
                var response = new
                {
                    status = "success",
                    answers_count = answers.Count,
                    answers = answers.Select(a => new
                    {
                        id = a.Answer_ID,
                        user = new
                        {
                            id = a.User_ID,
                            name = "default", // 这里需要从用户信息中获取
                            avatar = "" // 这里需要从用户信息中获取
                        },
                        content = a.Answer_Content,
                        time = a.Answer_Date
                    }).ToList()
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching answers: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: /api/post_questions 
        // 提出问题
        [HttpPost("post_questions")]
        public IActionResult PostQuestion([FromBody] Dictionary<string, object> requestData)
        {
            try
            {
                // 检查请求中是否包含必要的参数 content, item_id 和 current_user_id
                if (requestData == null || !requestData.ContainsKey("content") || !requestData.ContainsKey("item_id") || !requestData.ContainsKey("current_user_id"))
                {
                    return BadRequest("Missing required parameters: content, item_id or current_user_id");
                }

                // 获取 content, item_id 和 current_user_id 参数
                string content = Convert.ToString(requestData["content"]);
                int itemId = Convert.ToInt32(requestData["item_id"]);
                int userId = Convert.ToInt32(requestData["current_user_id"]);

                

                // 调用 QuestionAnswers 类中的方法提问
                int questionId = questionAnswers.PostQuestion(content, itemId, userId,DateTime.Now);

                // 构建响应对象
                var response = new
                {
                    status = "success",
                    question_id = questionId,
                    time = DateTime.Now // 当前时间
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error posting question: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        // 撤回提问
        // POST: /api/questions_retract/{question_id}
        [HttpPost("questions_retract/{question_id}")]
        public IActionResult WithdrawQuestion([FromBody] Dictionary<string, object> requestData)
        {
            try
            {
                // 检查请求中是否包含必要的参数 current_user_id
                if (requestData == null || !requestData.ContainsKey("current_user_id") || !requestData.ContainsKey("question_id")) 
                {
                    return BadRequest("Missing required parameter: current_user_id");
                }

                // 将 question_id 转换为整数类型
                int questionId = Convert.ToInt32(requestData["question_id"]);
                int userId = Convert.ToInt32(requestData["current_user_id"]);

                

                // 调用 QuestionAnswers 类中的方法撤回问题
                bool success = questionAnswers.WithdrawQuestion(questionId, userId);

                if (success)
                {
                    // 构建成功响应对象
                    var response = new
                    {
                        status = "success",
                        time = DateTime.Now // 当前时间
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
                Console.WriteLine($"Error withdrawing question: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // 回答问题
        // POST: /api/questions/{question_id}/answers
        [HttpPost("questions/{question_id}/post_answers")]
        public IActionResult PostAnswer([FromBody] Dictionary<string, object> requestData)
        {
            try
            {
                // 检查请求中是否包含必要的参数 content 和 current_user_id
                if (requestData == null || !requestData.ContainsKey("content") || !requestData.ContainsKey("current_user_id")||!requestData.ContainsKey("question_id"))
                {
                    return BadRequest("Missing required parameters: content or current_user_id");
                }

                // 将 question_id 转换为整数类型
                int questionId = Convert.ToInt32(requestData["question_id"]);

                // 获取当前用户ID
                int userId = Convert.ToInt32(requestData["current_user_id"]);

                // 获取回答内容
                string answerContent = requestData["content"].ToString();

                // 调用 QuestionAnswers 类中的方法回答问题
                int answerId = questionAnswers.PostAnswer(questionId, answerContent, userId);

                // 如果回答ID为0，则表示回答操作失败
                if (answerId == 0)
                {
                    return StatusCode(500, "Error posting answer"); // 返回内部服务器错误
                }

                // 构建成功响应对象
                var response = new
                {
                    status = "success",
                    answer_id = answerId.ToString(),
                    time = DateTime.Now // 当前时间
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error posting answer: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        // POST: /api/answers_retract/{answer_id}
        [HttpPost("answers_retract/{answer_id}")]
        public IActionResult RetractAnswer([FromBody] Dictionary<string, object> requestData)
        {
            try
            {
                // 检查请求中是否包含必要的参数 answer_id 和 current_user_id
                if (requestData == null || !requestData.ContainsKey("answer_id") || !requestData.ContainsKey("current_user_id"))
                {
                    return BadRequest("Missing required parameters: answer_id or current_user_id");
                }


                // 将 answer_id 和 current_user_id 转换为对应类型
                int answerId = Convert.ToInt32(requestData["answer_id"]);
                int userId = Convert.ToInt32(requestData["current_user_id"]);

                // 调用 QuestionAnswers 类中的方法撤回回答
                bool result = questionAnswers.WithdrawAnswer(answerId, userId);

                // 如果撤回操作失败，则返回内部服务器错误
                if (!result)
                {
                    return StatusCode(500, "Error withdrawing answer");
                }

                // 构建成功响应对象
                var response = new
                {
                    status = "success",
                    time = DateTime.Now // 当前时间
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error withdrawing answer: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
