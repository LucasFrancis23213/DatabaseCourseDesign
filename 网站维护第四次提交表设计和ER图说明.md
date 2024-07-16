# 安全维护部分表以及E-R图

## 拟定表

### SystemLogs(系统日志表)  

| 标签             | 变量类型      | 说明                       |
|------------------|--------------|----------------------------|
| 日志ID (LogID)         | INT 或 BIGINT         | 通常使用自增主键，确保每条记录唯一  |
| 操作类型 (OperationType)          | VARCHAR(50)         | 存储文本描述的操作类型，如“系统维护”、“数据备份”等|
| 操作详情 (OperationDetails)         |TEXT  | 存储操作的具体详情，这可能是一个较长的文本|
| 操作用户ID (UserID)         | DATETIME    | 存储操作发生的日期和时间     |


### APIAccessLogs (API访问日志表)

| 标签             | 变量类型      | 说明                                      |
|------------------|--------------|-------------------------------------------|
| 访问ID (AccessID)      | INT 或 BIGINT         | 作为主键使用，通常设为自增，以保证唯一性          |
| API名称 (APIName)          | VARCHAR(100)        | 存储被访问API的名称或唯一标识。|
| 访问者标识 (AccessorID)         | varchar(50)  | 存储访问者的用户ID或第三方应用ID。若访问者可以是多种类型实体，此字段需要适当设计以适配这种多态性|
| 访问时间 (AccessTime)         | DATETIME     | 行为发生的具体时间。                      |
|访问结果 (AccessResult) |VARCHAR(50)|存储API访问的结果，如“成功”、“失败”或具体的错误代码
### SecurityEvents (安全事件记录表)

| 标签                       | 变量类型         | 说明                                       |
|----------------------------|-----------------|------------------------------------------|
| 事件ID (EventID)           | INT 或 BIGINT   | 作为表的主键，通常设为自增，保证每个安全事件具有唯一标识 |
| 事件类型 (EventType)       | VARCHAR(100)    | 描述安全事件的类型，例如“账户异常登录”、“数据泄露”等   |
| 事件详情 (EventDetails)    | TEXT            | 提供关于安全事件的详细信息，包括事件的具体描述、可能涉及的系统组件、影响范围等 |
| 处理状态 (Status)          | VARCHAR(50)     | 表示事件的当前处理状态，例如“已解决”、“待处理”等       |
| 发生日期 (OccurrenceDate)  | DATE            | 事件发生的日期                             |
### 审核过渡表

| 标签                 | 变量类型         | 说明                                                     |
|----------------------|-----------------|----------------------------------------------------------|
| 临时ID (TempItemID)  | INT 或 BIGINT   | 主键，系统自动生成，唯一标识每条记录                       |
| 物品ID (ItemID)      | INT 或 BIGINT   | 关联的正式物品ID，为空表示新提交的物品                    |
| 物品名称 (ItemName)  | VARCHAR(255)    | 用户输入的物品名称                                       |
| 类别ID (CategoryID)  | INT             | 用户选择的类别ID，关联到ItemCategories表                 |
| 描述 (Description)   | TEXT            | 用户输入的描述信息                                       |
| 地点 (Location)      | VARCHAR(255)    | 用户输入的地点（遗失地点或找回地点）                       |
| 日期 (Date)          | DATE            | 用户输入的日期（遗失日期或找回日期）                       |
| 用户ID (UserID)      | INT 或 BIGINT   | 提交信息的用户ID，关联到Users表                           |
| 图片URL (ImageURL)   | VARCHAR(255)    | 用户上传的图片URL                                        |
| 审核状态 (Status)    | VARCHAR(50)     | 物品的审核状态，如“待审核”、“审核通过”、“审核拒绝”等       |
| 处理类型 (ProcessType)| VARCHAR(50)    | 标识是“遗失物品”还是“找回物品”                           |
| 提交日期 (SubmissionDate) | DATETIME   | 系统自动生成的提交日期                                   |
| 审核备注 (ReviewComments) | TEXT      | 用于记录审核过程中的任何特别说明或问题                   |

## E-R图设计相关说明
具体ER图见drawio文件
### SystemLogs (系统日志表)

- **实体**: SystemLogs
- **属性**:
  - LogID
  - OperationType
  - OperationDetails
  - UserID
  - OperationDate
- **关系**:
  - 关联到 Users 表 (通过 UserID)，记录管理员的操作。

### APIAccessLogs (API访问日志表)

- **实体**: APIAccessLogs
- **属性**:
  - AccessID
  - APIName
  - AccessorID
  - AccessTime
  - AccessResult
- **关系**:
  - 关联到AuthInfo表，实现身份验证服务API
  - 关联到VIPMembers，实现支付服务API和充值二维码的提供
  

### SecurityEvents (安全事件记录表)

- **实体**: SecurityEvents
- **属性**:
  - EventID
  - EventType
  - EventDetails
  - Status
  - OccurrenceDate
- **关系**:
  - 无直接的外键关系，但可以与用户表间接关联，涉及到用户安全事件（与其他同学设计的功能方面的安全问题输出关联）。

### 审核过渡表

- **实体**: AuditTransition
- **属性**:
  - TempItemID
  - ItemID
  - ItemName
  - CategoryID
  - Description
  - Location
  - Date
  - UserID
  - ImageURL
  - Status
  - ProcessType
  - SubmissionDate
  - ReviewComments
- **关系**:
  - 关联到陆诚彬同学审核部分的所有表 ,实现评论部分审核的过渡，完成后直接对审核状态进行更新
 