## **广告功能**：

### 数据库表

- 表名：**Advertisements**（广告信息表）
	
	| 字段名      | 数据类型     | 描述         |
	| ----------- | ------------ | ------------ |
	| AdID        | int          | 主键，广告ID |
	| AdContent   | varchar(255) | 广告内容     |
	| AdURL       | varchar(255) | 广告链接     |
	| AdType      | varchar(50)  | 广告类型     |
	| AdStartDate | datetime     | 广告开始日期 |
	| AdEndDate   | datetime     | 广告结束日期 |
	
- 表名：**ClickStatistics**（点击统计表）

| 字段名    | 数据类型    | 描述                 |
| --------- | ----------- | -------------------- |
| ClickID   | int         | 主键，点击记录ID     |
| AdID      | int         | 外键，被点击的广告ID |
| UserID    | int         | 外键，点击用户的ID   |
| ClickTime | datetime    | 点击时间             |
| IPAddress | varchar(50) | 点击用户的IP地址     |

### 后台逻辑

#### 后台逻辑：

1. 获取广告列表：编写逻辑来从数据库中获取有效期内的广告列表，供前台展示使用。
2. 检查用户VIP状态：编写逻辑来检查用户是否为VIP用户，可以通过查询用户表中的VIP状态字段来实现。
3. 确定广告显示：根据用户VIP状态和广告的类型，决定是否在前台展示广告。对于VIP用户，可能需要不显示广告或者显示VIP专属广告；对于普通用户，则显示普通广告。
4. 管理广告：编写逻辑来管理广告，包括新增、编辑、删除广告等功能。需要确保新增的广告的开始日期在当前日期之后，并且结束日期在开始日期之后。
5. 广告点击统计：如果需要统计广告的点击量，可以在用户点击广告时记录点击信息，并定期汇总统计点击量。

#### 示例代码

- C#代码

```c#
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

public class AdvertisementsService
{
    private string connectionString = "Your_Connection_String";

    public List<Advertisement> GetAdvertisements()
    {
        List<Advertisement> advertisements = new List<Advertisement>();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string query = "SELECT * FROM Advertisements WHERE AdStartDate <= GETDATE() AND AdEndDate >= GETDATE()";
            SqlCommand command = new SqlCommand(query, connection);

            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Advertisement ad = new Advertisement
                {
                    AdID = (int)reader["AdID"],
                    AdContent = (string)reader["AdContent"],
                    AdURL = (string)reader["AdURL"],
                    AdType = (string)reader["AdType"],
                    AdStartDate = (DateTime)reader["AdStartDate"],
                    AdEndDate = (DateTime)reader["AdEndDate"]
                };

                advertisements.Add(ad);
            }
        }

        return advertisements;
    }

    public bool IsUserVIP(int userID)
    {
        bool isVIP = false;

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string query = "SELECT VIPStatus FROM Users WHERE UserID = @UserID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserID", userID);

            connection.Open();
            object result = command.ExecuteScalar();

            if (result != null && result != DBNull.Value)
            {
                isVIP = (bool)result;
            }
        }

        return isVIP;
    }
}

public class Advertisement
{
    public int AdID { get; set; }
    public string AdContent { get; set; }
    public string AdURL { get; set; }
    public string AdType { get; set; }
    public DateTime AdStartDate { get; set; }
    public DateTime AdEndDate { get; set; }
}
```

### **接口设计**

#### 管理员接口

1. **获取广告列表**：
	- **请求方式：** GET
	- **URL：** /api/advertisements
	- **功能：** 获取所有有效期内的广告列表
	- **返回数据：** 包含广告信息的JSON数组
2. **获取单个广告信息**：
	- **请求方式：** GET
	- **URL：** /api/advertisements/{adID}
	- **功能：** 获取指定广告ID的广告信息
	- **返回数据：** 包含广告信息的JSON对象
3. **新增广告**：
	- **请求方式：** POST
	- **URL：** /api/advertisements
	- **功能：** 新增一条广告信息
	- **请求数据：** 包含新增广告信息的JSON对象
	- **返回数据：** 包含新增广告的ID等信息的JSON对象
4. **编辑广告**：
	- **请求方式：** PUT
	- **URL：** /api/advertisements/{adID}
	- **功能：** 编辑指定广告ID的广告信息
	- **请求数据：** 包含更新后广告信息的JSON对象
	- **返回数据：** 包含更新后的广告信息的JSON对象
5. **删除广告**：
	- **请求方式：** DELETE
	- **URL：** /api/advertisements/{adID}
	- **功能：** 删除指定广告ID的广告信息
	- **返回数据：** 操作成功或失败的消息

#### 表格

| 接口名称         | 请求方法 | URL                        | 描述               | 请求参数                                                     | 响应数据                     |
| ---------------- | -------- | -------------------------- | ------------------ | ------------------------------------------------------------ | ---------------------------- |
| 获取广告列表     | GET      | /api/advertisements        | 获取所有广告列表   | 无                                                           | 广告对象的JSON数组           |
| 获取单个广告信息 | GET      | /api/advertisements/{adID} | 获取指定广告的信息 | adID: 广告ID                                                 | 单个广告对象的JSON           |
| 新增广告         | POST     | /api/advertisements        | 新增一条广告信息   | AdContent: 广告内容<br>AdURL: 广告链接<br>AdType: 广告类型<br>AdStartDate: 广告开始日期<br>AdEndDate: 广告结束日期 | 新增广告的ID等信息的JSON对象 |
| 编辑广告         | PUT      | /api/advertisements/{adID} | 编辑指定广告的信息 | adID: 广告ID<br>其他广告属性                                 | 更新后的广告信息的JSON对象   |
| 删除广告         | DELETE   | /api/advertisements/{adID} | 删除指定广告       | adID: 广告ID                                                 | 操作成功或失败的消息         |

#### 示例代码

```c#
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AdvertisementAPI.Controllers
{
    public class AdvertisementsController : ApiController
    {
        private static List<Advertisement> advertisements = new List<Advertisement>
        {
            new Advertisement { AdID = 1, AdContent = "Ad 1 Content", AdURL = "http://example.com", AdType = "Banner", AdStartDate = DateTime.Now, AdEndDate = DateTime.Now.AddDays(30) },
            new Advertisement { AdID = 2, AdContent = "Ad 2 Content", AdURL = "http://example.com", AdType = "Popup", AdStartDate = DateTime.Now, AdEndDate = DateTime.Now.AddDays(15) }
        };

        // GET: api/advertisements
        public IHttpActionResult GetAdvertisements()
        {
            return Ok(advertisements);
        }
    }

    public class Advertisement
    {
        public int AdID { get; set; }
        public string AdContent { get; set; }
        public string AdURL { get; set; }
        public string AdType { get; set; }
        public DateTime AdStartDate { get; set; }
        public DateTime AdEndDate { get; set; }
    }
}
```

#### 用户接口

1. **获取广告列表**：
	- **请求方式：** GET
	- **URL：** /api/advertisements
	- **描述：** 获取所有有效期内的广告列表，用于在前台展示广告内容。
	- **请求参数：** 无
	- **响应数据：** 包含广告信息的JSON数组
2. **获取单个广告信息**：
	- **请求方式：** GET
	- **URL：** /api/advertisements/{adID}
	- **描述：** 获取指定广告ID的广告信息，用于在前台展示单个广告内容。
	- **请求参数：** adID: 广告ID
	- **响应数据：** 单个广告对象的JSON
3. **点击广告**：
	- **请求方式：** POST
	- **URL：** /api/advertisements/{adID}/click
	- **描述：** 用户点击广告时调用此接口，用于统计广告点击量或其他相关操作。
	- **请求参数：** adID: 广告ID
	- **响应数据：** 操作成功或失败的消息

#### 表格

| 接口名称         | 请求方法 | URL                              | 描述                     | 请求参数     | 响应数据             |
| ---------------- | -------- | -------------------------------- | ------------------------ | ------------ | -------------------- |
| 获取广告列表     | GET      | /api/advertisements              | 获取所有广告列表         | 无           | 广告对象的JSON数组   |
| 获取单个广告信息 | GET      | /api/advertisements/{adID}       | 获取指定广告的信息       | adID: 广告ID | 单个广告对象的JSON   |
| 点击广告         | POST     | /api/advertisements/{adID}/click | 用户点击广告时调用此接口 | adID: 广告ID | 操作成功或失败的消息 |



### 前台实现

#### 示例代码

```html
<template>
  <div>
    <h1>广告列表</h1>
    <ul>
      <li v-for="ad in advertisements" :key="ad.AdID">
        <a :href="ad.AdURL" target="_blank">{{ ad.AdContent }}</a>
      </li>
    </ul>
  </div>
</template>

<script>
export default {
  data() {
    return {
      advertisements: []
    };
  },
  mounted() {
    this.fetchAdvertisements();
  },
  methods: {
    fetchAdvertisements() {
      fetch('/api/advertisements')
        .then(response => response.json())
        .then(data => {
          this.advertisements = data;
        })
        .catch(error => {
          console.error('Error fetching advertisements:', error);
        });
    }
  }
};
</script>
```

## VIP功能

### 数据库表

- **VIPMembers**（**VIP会员信息表**）

| 字段名       | 数据类型    | 描述                                |
| ------------ | ----------- | ----------------------------------- |
| VIPMemberID  | int         | 主键，VIP会员ID                     |
| UserID       | int         | 外键，关联用户ID                    |
| VIPStartDate | datetime    | VIP会员开始日期                     |
| VIPEndDate   | datetime    | VIP会员结束日期                     |
| VIPLevel     | int         | VIP会员等级                         |
| Status       | varchar(20) | VIP会员状态（如正常、过期、冻结等） |
| CreatedAt    | datetime    | 创建时间                            |
| UpdatedAt    | datetime    | 更新时间                            |

- **VIP等级表（VIPLevels）**：

	| 字段名           | 数据类型     | 描述         |
	| ---------------- | ------------ | ------------ |
	| LevelID          | int          | 主键，等级ID |
	| LevelName        | varchar(50)  | 等级名称     |
	| LevelDescription | varchar(255) | 等级描述     |
	| RequiredPoints   | int          | 所需积分     |

- **VIP特权表（VIPPrivileges）**：

	| 字段名               | 数据类型     | 描述         |
	| -------------------- | ------------ | ------------ |
	| PrivilegeID          | int          | 主键，特权ID |
	| PrivilegeName        | varchar(50)  | 特权名称     |
	| PrivilegeDescription | varchar(255) | 特权描述     |
	| RequiredLevelID      | int          | 所需等级ID   |

- **积分记录表（PointRecords）**：

	| 字段名      | 数据类型    | 描述                  |
	| ----------- | ----------- | --------------------- |
	| RecordID    | int         | 主键，记录ID          |
	| UserID      | int         | 外键，关联用户ID      |
	| RecordType  | varchar(20) | 记录类型（增加/减少） |
	| PointChange | int         | 积分变动值            |
	| RecordTime  | datetime    | 记录时间              |

- **订单表（Orders）**：

	| 字段名         | 数据类型      | 描述             |
	| -------------- | ------------- | ---------------- |
	| OrderID        | int           | 主键，订单ID     |
	| UserID         | int           | 外键，关联用户ID |
	| TotalAmount    | decimal(10,2) | 订单总金额       |
	| DiscountAmount | decimal(10,2) | 折扣金额         |
	| PointReturn    | int           | 积分返还量       |
	| OrderTime      | datetime      | 下单时间         |

### 接口设计

| 接口名称         | 请求方法 | URL                 | 描述                      | 请求参数                                                     | 响应数据                         |
| ---------------- | -------- | ------------------- | ------------------------- | ------------------------------------------------------------ | -------------------------------- |
| 获取VIP会员信息  | GET      | /api/vip/members    | 获取当前用户的VIP会员信息 | 无                                                           | 当前用户的VIP会员信息的JSON对象  |
| 获取VIP等级列表  | GET      | /api/vip/levels     | 获取所有VIP会员等级列表   | 无                                                           | 所有VIP会员等级信息的JSON数组    |
| 获取VIP特权列表  | GET      | /api/vip/privileges | 获取所有VIP特权列表       | 无                                                           | 所有VIP特权信息的JSON数组        |
| 购买VIP会员      | POST     | /api/vip/members    | 购买VIP会员               | UserID: 用户ID<br>LevelID: VIP等级ID<br>StartDate: VIP开始日期<br>EndDate: VIP结束日期 | 购买成功或失败的消息             |
| 获取用户积分记录 | GET      | /api/vip/points     | 获取当前用户的积分记录    | 无                                                           | 当前用户的积分记录信息的JSON数组 |

#### 前台用到的接口

| 接口名称         | 请求方法 | URL                  | 描述                      | 请求参数               | 响应数据                         |
| ---------------- | -------- | -------------------- | ------------------------- | ---------------------- | -------------------------------- |
| 获取VIP会员信息  | GET      | /api/vip/members     | 获取当前用户的VIP会员信息 | 无                     | 当前用户的VIP会员信息的JSON对象  |
| 获取VIP等级列表  | GET      | /api/vip/levels      | 获取所有VIP会员等级列表   | 无                     | 所有VIP会员等级信息的JSON数组    |
| 获取VIP特权列表  | GET      | /api/vip/privileges  | 获取所有VIP特权列表       | 无                     | 所有VIP特权信息的JSON数组        |
| 获取用户积分记录 | GET      | /api/vip/points      | 获取当前用户的积分记录    | 无                     | 当前用户的积分记录信息的JSON数组 |
| 使用优惠券       | POST     | /api/vip/coupons/use | 使用优惠券                | CouponCode: 优惠券代码 | 使用成功或失败的消息             |
| 获取广告列表     | GET      | /api/ads             | 获取广告列表              | 无                     | 包含广告信息的JSON数组           |
| 获取用户个人信息 | GET      | /api/user/profile    | 获取当前用户的个人信息    | 无                     | 当前用户的个人信息的JSON对象     |
| 更新用户个人信息 | PUT      | /api/user/profile    | 更新当前用户的个人信息    | 要更新的用户个人信息   | 更新成功或失败的消息             |
| 获取订单列表     | GET      | /api/orders          | 获取当前用户的订单列表    | 无                     | 当前用户的订单列表的JSON数组     |
| 创建新订单       | POST     | /api/orders          | 创建新订单                | 新订单的详细信息       | 创建成功或失败的消息             |
| 取消订单         | DELETE   | /api/orders/{id}     | 取消指定ID的订单          | 无                     | 取消成功或失败的消息             |

### 后台逻辑（部分）

- VIP会员信息

~~~c#
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace YourApi.Controllers
{
    public class VIPController : ApiController
    {
        // 模拟数据库，存储VIP会员信息
        private List<VIPMember> VIPMembers = new List<VIPMember>()
        {
            new VIPMember { UserID = 1, LevelID = 1, StartDate = DateTime.Now, EndDate = DateTime.Now.AddYears(1) },
            new VIPMember { UserID = 2, LevelID = 2, StartDate = DateTime.Now, EndDate = DateTime.Now.AddYears(1) },
            // 其他VIP会员信息...
        };

        // GET: api/vip/members
        public IHttpActionResult GetVIPMember()
        {
            // 模拟从数据库中获取当前用户的VIP会员信息，这里假设用户ID为1
            var currentUserVIP = VIPMembers.FirstOrDefault(v => v.UserID == 1);
            if (currentUserVIP == null)
            {
                return NotFound(); // 如果用户不存在VIP会员信息，则返回404 Not Found
            }
            return Ok(currentUserVIP); // 返回当前用户的VIP会员信息
        }
    }

    // VIP会员信息模型
    public class VIPMember
    {
        public int UserID { get; set; } // 用户ID
        public int LevelID { get; set; } // VIP等级ID
        public DateTime StartDate { get; set; } // VIP开始日期
        public DateTime EndDate { get; set; } // VIP结束日期
    }
}

~~~

- VIP等级列表信息：

	```c#
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net;
	using System.Net.Http;
	using System.Web.Http;
	
	namespace YourApi.Controllers
	{
	    public class VIPController : ApiController
	    {
	        // 模拟数据库，存储VIP等级信息
	        private List<VIPLevel> VIPLevels = new List<VIPLevel>()
	        {
	            new VIPLevel { LevelID = 1, LevelName = "普通会员", LevelDescription = "享有基本特权" },
	            new VIPLevel { LevelID = 2, LevelName = "高级会员", LevelDescription = "享有高级特权" },
	            // 其他VIP等级信息...
	        };
	
	        // GET: api/vip/levels
	        public IHttpActionResult GetVIPLevels()
	        {
	            return Ok(VIPLevels); // 返回所有VIP等级信息
	        }
	    }
	
	    // VIP等级信息模型
	    public class VIPLevel
	    {
	        public int LevelID { get; set; } // VIP等级ID
	        public string LevelName { get; set; } // VIP等级名称
	        public string LevelDescription { get; set; } // VIP等级描述
	    }
	}
	```

- 后台查看VIP信息

~~~ c#
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace YourApi.Controllers
{
    public class VIPController : ApiController
    {
        // 模拟数据库，存储VIP会员信息
        private List<VIPMember> VIPMembers = new List<VIPMember>();

        // GET: api/vip/members
        public IHttpActionResult GetVIPMember(int userID)
        {
            // 根据用户ID从数据库中获取VIP会员信息
            var currentUserVIP = VIPMembers.FirstOrDefault(v => v.UserID == userID);
            if (currentUserVIP == null)
            {
                return NotFound(); // 如果用户不存在VIP会员信息，则返回404 Not Found
            }
            return Ok(currentUserVIP); // 返回当前用户的VIP会员信息
        }

        // POST: api/vip/members
        public IHttpActionResult PostVIPMember(VIPMember vipMember)
        {
            // 模拟向数据库中添加VIP会员信息
            VIPMembers.Add(vipMember);
            return CreatedAtRoute("DefaultApi", new { id = vipMember.UserID }, vipMember); // 返回创建的VIP会员信息
        }

        // PUT: api/vip/members/{id}
        public IHttpActionResult PutVIPMember(int userID, VIPMember vipMember)
        {
            // 根据用户ID更新数据库中的VIP会员信息
            var existingVIPMember = VIPMembers.FirstOrDefault(v => v.UserID == userID);
            if (existingVIPMember == null)
            {
                return NotFound(); // 如果用户不存在VIP会员信息，则返回404 Not Found
            }
            existingVIPMember.LevelID = vipMember.LevelID;
            existingVIPMember.StartDate = vipMember.StartDate;
            existingVIPMember.EndDate = vipMember.EndDate;
            return Ok(existingVIPMember); // 返回更新后的VIP会员信息
        }

        // DELETE: api/vip/members/{id}
        public IHttpActionResult DeleteVIPMember(int userID)
        {
            // 根据用户ID从数据库中删除VIP会员信息
            var existingVIPMember = VIPMembers.FirstOrDefault(v => v.UserID == userID);
            if (existingVIPMember == null)
            {
                return NotFound(); // 如果用户不存在VIP会员信息，则返回404 Not Found
            }
            VIPMembers.Remove(existingVIPMember);
            return Ok(); // 返回删除成功的消息
        }
    }

    // VIP会员信息模型
    public class VIPMember
    {
        public int UserID { get; set; } // 用户ID
        public int LevelID { get; set; } // VIP等级ID
        public DateTime StartDate { get; set; } // VIP开始日期
        public DateTime EndDate { get; set; } // VIP结束日期
    }
}

~~~

- VIP充值

~~~c#
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace YourApi.Controllers
{
    public class VIPController : ApiController
    {
        // 模拟数据库，存储VIP会员信息
        private List<VIPMember> VIPMembers = new List<VIPMember>();

        // POST: api/vip/recharge
        public IHttpActionResult PostVIPRecharge(VIPRechargeRequest request)
        {
            // 根据请求中的用户ID查找数据库中的VIP会员信息
            var existingVIPMember = VIPMembers.FirstOrDefault(v => v.UserID == request.UserID);
            if (existingVIPMember == null)
            {
                return NotFound(); // 如果用户不存在VIP会员信息，则返回404 Not Found
            }
            
            // 更新VIP会员信息中的结束日期
            existingVIPMember.EndDate = existingVIPMember.EndDate.AddMonths(request.Months);

            return Ok(existingVIPMember); // 返回充值后的VIP会员信息
        }
    }

    // VIP会员信息模型
    public class VIPMember
    {
        public int UserID { get; set; } // 用户ID
        public int LevelID { get; set; } // VIP等级ID
        public DateTime StartDate { get; set; } // VIP开始日期
        public DateTime EndDate { get; set; } // VIP结束日期
    }

    // VIP充值请求模型
    public class VIPRechargeRequest
    {
        public int UserID { get; set; } // 用户ID
        public int Months { get; set; } // 充值月数
    }
}

~~~

