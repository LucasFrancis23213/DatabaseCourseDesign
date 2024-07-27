using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAppTest.APITemplate
{

    //HttpGet --> get方法
    // get方法代表向数据库查询(select)
    // post方法代表向数据库插入(insert into)
    // put方法代表向数据库更新数据(update)
    // delete方法用于在数据库里删除数据(delete)

    //接口规则
    [Route("api/[controller]")]
    //不用管为啥，照着抄就行
    [ApiController]
    public class ApiController : ControllerBase
    {
        // GET: api/<ApiController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<ApiController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            Debug.WriteLine($"id is {id}");
            return "value";
        }

        // POST api/<ApiController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ApiController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ApiController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
