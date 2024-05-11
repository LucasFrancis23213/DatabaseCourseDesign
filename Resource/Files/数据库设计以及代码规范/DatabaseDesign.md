# 遗失物品找回数据库设计方案

设计一个“遗失物品找回”数据库时，我们的目标是创建一个能够有效存储和管理遗失物品、报失者信息、物品找回状态以及可能的匹配者信息的系统。以下是一个可能的设计方案，包括了多个表格以及它们之间的关系。

## I. 基础设施和通用表

### 1. 用户管理
- 用户表 (Users) ✔️
- 用户认证信息表 (AuthInfo) ✔️
- 用户偏好设置表 (UserPreferences) ✔️
- 用户积分表 (UserPoints) ✔️
- 用户评价表 (UserRatings) ✔️
- 用户活跃度表 (UserActivity) ✔️
- 用户消息表 (UserMessages) ✔️
- 用户关系表 (UserRelationships) ✔️
- 用户订阅表 (UserSubscriptions) ✔️

### 2. 物品管理
- 物品类别表 (ItemCategories) ✔️
- 遗失物品表 (LostItems) ✔️
- 找回物品表 (FoundItems) ✔️
- 物品图片表 (ItemImages) ✔️
- 物品标签表 (ItemTags) ✔️
- 物品-标签关联表 (ItemTagLinks) ✔️
- 物品评论表 (ItemComments) ✔️
- 物品状态历史表 (ItemStatusHistory) ✔️
- 物品认领流程表 (ItemClaimProcesses) ✔️

### 3. 系统管理和日志
- 事件日志表 (EventLogs)
- ~~系统设置表 (SystemSettings)~~(*拟弃用，不知道这个表有啥用*)
- 系统日志表 (SystemLogs) ✔️
- 安全事件记录表 (SecurityEvents) ✔️
- API访问日志表 (APIAccessLogs) ✔️
- 定时任务表 (ScheduledTasks) ✔️

## II. 特定角色功能扩展

### 1. 针对丢失物品的用户
- ~~用户反馈表 (UserFeedback)~~(*拟弃用,与“用户评价表”重复*)
- 赏金悬赏表 (RewardOffers) ✔️
- 物品认领流程表 (ItemClaimProcesses) ✔️

### 2. 针对找回物品的用户
- 匹配记录表 (MatchRecords) ✔️
- 物品交换表 (ItemExchanges) ✔️
- 物品归还协议表 (ItemReturnAgreements) ✔️
- 物品认领流程表 (ItemClaimProcesses) ✔️

### 3. 针对网站工作人员
- 通知记录表 (NotificationLogs)
- 用户行为日志表 (UserActivityLogs)
- 推荐系统日志表 (RecommendationLogs)
- 交易记录表 (TransactionLogs)

## III. 交互增强功能

- 社交媒体分享记录表 (SocialMediaShares) ✔️
- 用户动态表 (UserFeeds) ✔️
- 问题表(Questions)
- 回答表(Answers)
- ~~关注列表表 (FollowLists)~~(*拟弃用，与"用户关系表"重复*)
- 物品认领流程表 (ItemClaimProcesses) ✔️

### VIP系统
- 广告信息表(Advertisements)  
- 点击统计表(AdClickStatistics)  
- VIP会员信息表(VIPMembers)  
- VIP等级表(VIPLevels) (*拟弃用，但如果想扩展则保留，也就是做这个表，但只有一条信息*)
- VIP购买订单表（VIPOrders）
- VIP特权表（VIPPrivileges）(*功能拟弃用，但如果想扩展则保留*)

### 审核系统
- 审核状态码表(ReviewCodes)  
- 物品审核状态关联表 (ItemReviewLinks)

# 具体表设计

## I. 基础设施和通用表

### 1. 用户管理
- 用户表 (Users)
    - **用户ID** (UserID): 唯一标识每个用户的主键。
    - **用户名** (Username): 用户的名称。
    - **联系方式** (Contact): 用户的联系方式，可以是电话号码或电子邮件。
    - **注册日期** (RegistrationDate): 用户注册平台的日期。
    - <u>**ip**</u>

- 用户认证信息表 (AuthInfo)
    - **认证ID** (AuthID): 唯一标识每个认证记录的主键。
    - **用户ID** (UserID): 关联的用户ID，外键，关联到Users表的UserID。
    - ~~**认证方式** (AuthMethod): 认证方式，如邮箱验证、手机验证等。~~
    - **认证状态** (AuthStatus): 当前的认证状态，如已验证、待验证等。
    - **认证日期** (AuthDate): 认证完成的日期。

- 用户偏好设置表 (UserPreferences)
    - **偏好ID** (PreferenceID): 唯一标识每个偏好设置的主键。
    - **用户ID** (UserID): 关联的用户ID，外键，关联到Users表的UserID。
    - **偏好类型** (PreferenceType): 偏好设置的类型，例如接收通知的方式、匹配项显示偏好等。
    - **偏好值** (PreferenceValue): 用户设置的偏好值。
    - **更新日期** (UpdateDate): 偏好设置最后更新的日期。

- 用户积分表 (UserPoints)
    - **积分ID** (PointID): 唯一标识每个积分记录的主键。
    - **用户ID** (UserID): 用户的ID，外键，关联到Users表的UserID。
    - **积分来源** (PointSource): 积分获取的来源，如“物品报失登记”、“成功找回物品”等。
    - **积分** (Points): 获得或消耗的积分数。
    - **日期** (Date): 积分变更的日期。

- 用户评价表 (UserRatings)
    - **评价ID** (RatingID): 唯一标识每个评价记录的主键。
    - **发起用户ID** (FromUserID): 发起评价的用户ID，外键，关联到Users表的UserID。
    - **目标用户ID** (ToUserID): 被评价的用户ID，外键，关联到Users表的UserID。
    - **评分** (Score): 给出的评分，例如1到5。
    - **评论** (Comment): 对目标用户的评论。
    - **评价日期** (RatingDate): 发起评价的日期。

- 用户活跃度表 (UserActivity)
    - **活跃度ID** (ActivityID): 唯一标识每条活跃度记录的主键。
    - **用户ID** (UserID): 用户的ID，外键，关联到Users表的UserID。
    - **活跃行为** (ActionID):对于评论、发帖、点赞等行为种类的标识  
	- **活跃分数** (ActivityScore): 基于用户活动计算的活跃度分数。  
	- **整体活跃度** (Points): 当前整体活跃度。(**拟更改，4.25**)  
	- **日期** (Date): 活跃度变更的日期。(**拟更改，4.25**)  

- 用户消息表 (UserMessages)
    - **消息ID** (MessageID): 唯一标识每条消息的主键。
    - **发送者用户ID** (FromUserID): 消息发送者的用户ID，外键，关联到Users表的UserID。
    - **接收者用户ID** (ToUserID): 消息接收者的用户ID，外键，关联到Users表的UserID。
    - **消息内容** (Content): 消息的具体内容。
    - **发送日期** (SentDate): 消息发送的日期和时间。
    - **阅读状态** (ReadStatus): 消息的阅读状态，如已读、未读。

- 用户关系表 (UserRelationships)
    - **关系ID** (RelationshipID): 唯一标识每对用户关系的主键。
    - **用户ID1** (UserID1): 用户1的ID，外键，关联到Users表的UserID。
    - **用户ID2** (UserID2): 用户2的ID，外键，关联到Users表的UserID。
    - **关系类型** (RelationshipType): 用户之间的关系类型，如“朋友”、“黑名单”等。
    - **建立日期** (EstablishedDate): 关系建立的日期。

- 用户订阅表 (UserSubscriptions)
    - **订阅ID** (SubscriptionID): 唯一标识每条订阅记录的主键。
    - **用户ID** (UserID): 订阅用户的ID，外键，关联到Users表的UserID。
    - **订阅类型** (SubscriptionType): 订阅的内容类型，如“特定物品类别的新失物登记”、“用户评价更新”等。
    - **订阅状态** (SubscriptionStatus): 订阅的状态，如“激活”、“暂停”等。
    - **创建日期** (CreationDate): 订阅创建的日期。

### 2. 物品管理
- 物品类别表 (ItemCategories)
    - **类别ID** (CategoryID): 唯一标识每个物品类别的主键。
    - **类别名称** (CategoryName): 物品的类别名称，如钱包、电子产品、证件等。

- 遗失物品表 (LostItems)
    - **物品ID** (ItemID): 唯一标识每个遗失物品的主键。
    - **物品名称** (ItemName): 遗失物品的名称。
    - **类别ID** (CategoryID): 物品属于的类别，外键，关联到ItemCategories表的CategoryID。
    - **描述** (Description): 物品的详细描述。
    - **遗失地点** (LostLocation): 物品遗失的地点。
    - **遗失日期** (LostDate): 物品遗失的日期。
    - **用户ID** (UserID): 报失者的ID，外键，关联到Users表的UserID。
    - **遗失状态** (LostStatus): 物品的状态（如：正在寻找、已找到）。
    - **审核状态** (ReviewStatus) 

- 找回物品表 (FoundItems)
    - **物品ID** (ItemID): 唯一标识每个找回物品的主键。
    - **物品名称** (ItemName): 找回物品的名称。
    - **类别ID** (CategoryID): 物品属于的类别，外键，关联到ItemCategories表的CategoryID。
    - **描述** (Description): 物品的详细描述。
    - **找回地点** (FoundLocation): 物品找回的地点。
    - **找回日期** (FoundDate): 物品找回的日期。
    - **用户ID** (UserID): 找回物品者的ID，外键，关联到Users表的UserID。
    - **匹配状态** (MatchStatus): 物品的匹配状态（如：待审核、匹配成功）。
    - **审核状态** (ReviewStatus)

- 物品图片表 (ItemImages)：提供物品图片相关信息
    - **图片ID** (ImageID): 唯一标识每个图片的主键。
    - **物品ID** (ItemID): 关联的遗失或找回物品的ID，外键，关联到LostItems或FoundItems表的ItemID。
    - **图片URL** (ImageUrl): 存储图片的URL地址。
    - **描述** (Description): 图片的简短描述或备注。
    - **审核状态** (ReviewStatus)

- 物品标签表 (ItemTags)：标签可以帮助用户通过不同的关键词快速分类和查找物品。
    - **标签ID** (TagID): 唯一标识每个标签的主键。
    - **标签名称** (TagName): 标签的名称，如“电子设备”、“个人物品”等。

- 物品-标签关联表 (ItemTagLinks)：用于建立物品和标签之间的多对多关系，使得物品可以被多个标签标记。
    - **物品ID** (ItemID): 物品的ID，外键，关联到LostItems或FoundItems表的ItemID。
    - **标签ID** (TagID): 标签的ID，外键，关联到ItemTags表的TagID。
    - **审核状态**(ReviewStatus)  

- 物品评论表 (ItemComments)：为物品提供评论功能，增加用户互动，帮助其他用户了解物品详情和找回历史。
    - **评论ID** (CommentID): 唯一标识每条评论的主键。
    - **物品ID** (ItemID): 评论关联的物品ID，外键，关联到LostItems或FoundItems表的ItemID。
    - **用户ID** (UserID): 评论者的用户ID，外键，关联到Users表的UserID。
    - **评论内容** (Content): 评论的具体内容。
    - **评论日期** (CommentDate): 发表评论的日期。

- 物品状态历史表 (ItemStatusHistory)：记录物品状态的变更历史，便于追踪物品的处理过程和改进物品管理。
    - **历史记录ID** (HistoryID): 唯一标识每条状态变更记录的主键。
    - **物品ID** (ItemID): 状态变更关联的物品ID，外键，关联到LostItems或FoundItems表的ItemID。
    - **前状态** (PreviousStatus): 物品的前一个状态。
    - **后状态** (NewStatus): 物品的新状态。
    - **变更日期** (ChangeDate): 状态变更的日期。

- 物品认领流程表 (ItemClaimProcesses)：管理物品认领流程，确保物品安全归还给失主。
    - **流程ID** (ProcessID): 唯一标识每个认领流程的主键。
    - **物品ID** (ItemID): 被认领的物品ID，外键，关联到LostItems或FoundItems表的ItemID。
    - **认领用户ID** (ClaimantUserID): 申请认领的用户ID，外键，关联到Users表的UserID。
    - **流程状态** (Status): 认领流程的当前状态，如“待审核”、“审核通过”、“审核拒绝”等。
    - **申请日期** (ApplicationDate): 提交认领申请的日期。

### 3. 系统管理和日志
- 事件日志表 (EventLogs)
    - **事件ID** (EventID): 唯一标识每个事件的主键。
    - **用户ID** (UserID): 事件关联的用户ID，外键，关联到Users表的UserID。
    - **事件类型** (EventType): 事件的类型，如“物品报失”、“物品找回”、“匹配成功”等。
    - **事件日期** (EventDate): 事件发生的日期和时间。
    - **详情** (Details): 事件的详细信息或备注。

- 系统设置表 (SystemSettings)
    - **设置ID** (SettingID): 唯一标识每个设置项的主键。
    - **设置名** (SettingName): 设置项的名称。
    - **设置值** (SettingValue): 设置项的值。
    - **描述** (Description): 设置项的描述或用途说明。

- 系统日志表 (SystemLogs)
    - **日志ID** (LogID): 唯一标识每条系统日志的主键。
    - **操作类型** (OperationType): 发生的操作类型，如系统维护、数据备份、异常报告等。
    - **操作详情** (OperationDetails): 操作的具体详情。
    - **操作用户ID** (UserID): 执行操作的用户ID，可以为空，外键，关联到Users表的UserID。
    - **操作日期** (OperationDate): 操作发生的日期和时间。

- 安全事件记录表 (SecurityEvents)：记录和管理安全相关的事件，确保平台的安全性和用户数据的保护。
    - **事件ID** (EventID): 唯一标识每个安全事件的主键。
    - **事件类型** (EventType): 安全事件的类型，如“账户异常登录”、“数据泄露”等。
    - **事件详情** (EventDetails): 事件的详细信息。
    - **处理状态** (Status): 事件的处理状态，如“已解决”、“待处理”等。
    - **发生日期** (OccurrenceDate): 事件发生的日期。

- API访问日志表 (APIAccessLogs)：通过记录API的访问情况，帮助监控和优化API服务的性能和安全性。
    - **访问ID** (AccessID): 唯一标识每次API访问的主键。
    - **API名称** (APIName): 被访问的API名称或标识。
    - **访问者标识** (AccessorID): 访问者的标识，可能是用户ID或第三方应用ID。
    - **访问时间** (AccessTime): API被访问的时间。
    - **访问结果** (AccessResult): API访问的结果，如“成功”、“失败”、“错误代码”。

- 定时任务表 (ScheduledTasks)：管理系统内部的定时任务，确保任务按计划执行，提高数据管理效率。
    - **任务ID** (TaskID): 唯一标识每个定时任务的主键。
    - **任务类型** (TaskType): 定时任务的类型，如“数据备份”、“报表生成”等。
    - **任务状态** (TaskStatus): 任务的执行状态，如“待执行”、“执行中”、“已完成”等。
    - **执行时间** (ExecutionTime): 定时任务执行的时间点。
    - **任务详情** (TaskDetails): 任务的详细描述或执行参数。

## II. 特定角色功能扩展

### 1. 针对丢失物品的用户
- 用户反馈表 (UserFeedback)
    - **反馈ID** (FeedbackID): 唯一标识每条反馈的主键。
    - **用户ID** (UserID): 提出反馈的用户ID，外键，关联到Users表的UserID。
    - **内容** (Content): 反馈的具体内容。
    - **提交日期** (SubmissionDate): 反馈提交的日期。

- 赏金悬赏表 (RewardOffers)
    - **悬赏ID** (RewardID): 唯一标识每个悬赏记录的主键。
    - **物品ID** (ItemID): 关联的遗失物品ID，外键，关联到LostItems表的ItemID。
    - **用户ID** (UserID): 悬赏者的用户ID，外键，关联到Users表的UserID。
    - **悬赏金额** (RewardAmount): 提供的悬赏金额。
    - **状态** (Status): 悬赏的当前状态，如激活、已领取、取消。
    - **发布日期** (ReleaseDate): 悬赏发布的日期。
    - **截止日期** (Deadline): 悬赏有效的截止日期。

- 物品认领流程表 (ItemClaimProcesses)(具体实现上面已经有了)

### 2. 针对找回物品的用户
- 匹配记录表 (MatchRecords)
    - **记录ID** (RecordID): 唯一标识每个匹配记录的主键。
    - **遗失物品ID** (LostItemID): 遗失物品的ID，外键，关联到LostItems表的ItemID。
    - **找回物品ID** (FoundItemID): 找回物品的ID，外键，关联到FoundItems表的ItemID。
    - **匹配日期** (MatchDate): 匹配的日期。
    - **处理状态** (ProcessingStatus): 记录的处理状态（如：待确认、已完成）。

- 物品交换表 (ItemExchanges)：支持用户之间的物品交换功能，增加平台的互动性和物品流通性。
    - **交换ID** (ExchangeID): 唯一标识每个交换记录的主键。
    - **发起物品ID** (InitiatorItemID): 发起交换的物品ID，外键，关联到LostItems或FoundItems表的ItemID。
    - **响应物品ID** (ResponderItemID): 响应交换的物品ID，外键，关联到LostItems或FoundItems表的ItemID。
    - **发起用户ID** (InitiatorUserID): 发起交换的用户ID，外键，关联到Users表的UserID。
    - **交易类型** (TransactionType): 交易的类型，如物品归还、赏金支付等。
    - **响应用户ID** (ResponderUserID): 响应交换的用户ID，外键，关联到Users表的UserID。
    - **交换状态** (ExchangeStatus): 交换的当前状态，如“待响应”、“已接受”、“已拒绝”等。
    - **创建时间** (CreationTime): 交换记录创建的时间。

- 物品归还协议表 (ItemReturnAgreements)：管理物品归还过程中的沟通和协议，确保归还行为有据可依。
    - **协议ID** (AgreementID): 唯一标识每个归还协议的主键。
    - **物品ID** (ItemID): 关联的物品ID，外键，关联到LostItems或FoundItems表的ItemID。
    - **发起用户ID** (FromUserID): 发起协议的用户ID，外键，关联到Users表的UserID。
    - **接收用户ID** (ToUserID): 接受协议的用户ID，外键，关联到Users表的UserID。
    - **协议内容** (AgreementContent): 协议的具体内容和条款。
    - **协议状态** (AgreementStatus): 协议的当前状态，如待确认、已同意、已拒绝。
    - **创建日期** (CreationDate): 协议创建的日期。

- 物品认领流程表 (ItemClaimProcesses)
    - **流程ID** (ProcessID): 唯一标识每个认领流程的主键。
    - **物品ID** (ItemID): 被认领的物品ID，外键，关联到LostItems或FoundItems表的ItemID。
    - **认领用户ID** (ClaimantUserID): 申请认领的用户ID，外键，关联到Users表的UserID。
    - **流程状态** (Status): 认领流程的当前状态，如“待审核”、“审核通过”、“审核拒绝”等。
    - **申请日期** (ApplicationDate): 提交认领申请的日期。

### 3. 针对网站工作人员
- 通知记录表 (NotificationLogs)
    - **通知ID** (NotificationID): 唯一标识每个通知的主键。
    - **用户ID** (UserID): 要通知的用户ID，外键，关联到Users表的UserID。
    - **通知类型** (NotificationType): 通知的类型，如“物品匹配通知”、“系统通知”等。
    - **发送日期** (SentDate): 通知发送的日期和时间。
    - **状态** (Status): 通知的状态，如已读、未读。

- 用户行为日志表 (UserActivityLogs)：通过记录用户的每一次活动，帮助管理员更好地理解用户行为，优化用户体验。
    - **行为日志ID** (ActivityLogID): 唯一标识每条用户行为记录的主键。
    - **用户ID** (UserID): 行为发起者的用户ID，外键，关联到Users表的UserID。
    - **行为类型** (ActivityType): 用户行为的类型，如“登录”、“发布物品”、“评论”等。
    - **发生时间** (Timestamp): 行为发生的具体时间。

- 推荐系统日志表 (RecommendationLogs)
    - **日志ID** (LogID): 唯一标识每条推荐日志记录的主键。
    - **用户ID** (UserID): 接受推荐的用户ID，外键，关联到Users表的UserID。
    - **推荐类别** (RecommendedContent): 推荐的类型。(拟更改，4.26日，改为推荐类别)  
    - **推荐时间** (RecommendationTime): 推荐发生的时间。
    - **用户反馈** (UserFeedback): 用户对推荐内容的反馈，如“有用”、“无用”。
- 交易记录表 (TransactionLogs)
    - **TransactionID** (交易ID): 主键，唯一标识每笔交易。
    - **FromUserID** (发起方用户ID): 发起交易的用户ID，外键，关联到**Users**表。
    - **ToUserID** (接收方用户ID): 接收交易的用户ID（如果适用），外键，关联到**Users**表。对于系统收费等情况，此字段可能为空或指向一个特定的系统账户。
    - **ItemID** (物品ID): 交易相关的物品ID（如果适用），外键，关联到**LostItems**或**FoundItems**表。不是所有交易都与物品直接相关，因此，这个字段在某些记录中可能为空。
    - **Amount** (金额): 交易金额。根据交易的性质，这可以是正数（如支付赏金、购买服务）。
    - **TransactionType** (交易类型): 描述交易类型的字符串，如"赏金支付"、"服务购买"、"退款"等。
    - **Status** (状态): 交易的当前状态，如"进行中"、"完成"、"失败"等。
    - **TransactionBeginDate** (交易开始日期): 记录交易开始的时间(交易发起，类比淘宝用户付完钱等待收货的状态)。(拟更改，4.26日，改为开始和完成时间)
    - **TransactionEndDate** (交易完成日期): 记录交易完成的时间(交易结束，可能是用户点击确认收获后对应的记录)。(拟更改，4.26日)
    - **Description** (描述): 关于交易的额外信息或备注，例如交易的具体目的或原因。

## III. 交互增强功能

- **社交媒体分享记录表** (SocialMediaShares)：促进物品信息在社交媒体上的传播，增加找回遗失物品的机会。
    - **分享ID** (ShareID): 唯一标识每次分享的主键。
    - **用户ID** (UserID): 进行分享的用户ID，外键，关联到Users表的UserID。
    - **物品ID** (ItemID): 被分享的物品ID，外键，关联到LostItems或FoundItems表的ItemID。
    - **平台** (Platform): 分享到的社交平台，如Facebook、Twitter等。
    - **分享日期** (ShareDate): 分享发生的日期。

- **用户动态表** (UserFeeds)：记录用户发布的动态，如物品找回成功的故事、寻物经验分享等。
   - **动态ID** (FeedID): 唯一标识每条动态的主键。
   - **用户ID** (UserID): 发布动态的用户ID，外键，关联到Users表的UserID。
   - **内容** (Content): 动态的具体内容。
   - **发布日期** (PublishDate): 动态发布的日期。

- **Questions**（问题表）
  - **问题ID** (QuestionID): 主键，唯一标识每个问题。
  - **关联物品ID** (ItemID): 外键，连接到物品信息表的ItemID。
  - **提问用户ID** (AskerUserID): 外键，关联到用户信息表的UserID。
  - **提出问题内容** (Content): 记录提问的文本内容。
  - **提问日期** (AskedDate): 记录提问的日期。
  - **订阅用户ID** (SubscriberUserID): 外键，关联到用户信息表的UserID，标识订阅此问题的用户。

- **Answers**（回答表）
  - **回答ID** (AnswerID): 主键，唯一标识每个回答。
  - **问题ID** (QuestionID): 外键，关联到问题表的QuestionID。
  - **回答用户ID** (ResponderUserID): 外键，关联到用户信息表的UserID。
  - **回答内容** (Content): 记录回答的文本信息。
  - **回答日期** (AnswerDate): 记录回答的日期。


- **关注列表表 (FollowLists)**: 允许用户关注其他用户，获取其更新和动态。
   - **关注ID** (FollowID): 唯一标识每条关注记录的主键。
   - **关注者用户ID** (FollowerUserID): 关注他人的用户ID，外键，关联到Users表的UserID。
   - **被关注者用户ID** (FollowedUserID): 被关注的用户ID，外键，关联到Users表的UserID。
   - **关注日期** (FollowDate): 关注的日期。

- **物品认领流程表** (ItemClaimProcesses)
    - **流程ID** (ProcessID): 唯一标识每个认领流程的主键。
    - **物品ID** (ItemID): 被认领的物品ID，外键，关联到LostItems或FoundItems表的ItemID。
    - **认领用户ID** (ClaimantUserID): 申请认领的用户ID，外键，关联到Users表的UserID。
    - **流程状态** (Status): 认领流程的当前状态，如“待审核”、“审核通过”、“审核拒绝”等。

### 广告设计
- **Advertisements**（广告信息表）
   - **广告ID** (AdID): 主键，唯一标识每条广告记录。
   - **广告内容** (AdContent): 存储广告的内容。
   - **广告链接** (AdURL): 链接到广告目标的URL。
   - **广告类型** (AdType): 广告的类型。
   - **广告开始日期** (AdStartDate): 广告投放的开始日期。
   - **广告结束日期** (AdEndDate): 广告投放的结束日期。

- **AdClickStatistics**（点击统计表）
   - **点击记录ID** (ClickID): 主键，唯一标识每条点击记录。
   - **被点击的广告ID** (AdID): 外键，关联到Advertisements表的AdID。
   - **点击用户的ID** (UserID): 外键，关联到Users表的UserID。
   - **点击时间** (ClickTime): 记录点击发生的时间。
   - **点击用户的IP地址** (IPAddress): 记录发起点击的用户IP地址。

### VIP会员信息表 (VIPMembers)
- **VIPMembers**（VIP会员信息表）
   - **VIP会员ID** (VIPMemberID): 主键，唯一标识每个VIP会员。
   - **关联用户ID** (UserID): 外键，关联到Users表的UserID。
   - **VIP会员开始日期** (VIPStartDate): 标记VIP会员资格的开始日期。
   - **VIP会员结束日期** (VIPEndDate): 标记VIP会员资格的结束日期。
   - **VIP会员等级** (VIPLevel): 表示会员的级别。
   - **VIP会员状态** (Status): 当前VIP状态（如正常、过期、冻结等）。
   - **创建时间** (CreatedAt): 记录VIP会员信息创建的时间。
   - **更新时间** (UpdatedAt): 记录VIP会员信息最后一次更新的时间。

- **VIPLevels**（VIP等级表）
   - **等级ID** (LevelID): 主键，唯一标识每个等级。
   - **等级名称** (LevelName): 等级的名称。
   - **等级描述** (LevelDescription): 描述等级的详情。
   - **所需积分** (RequiredPoints): 升级到此等级所需的积分。

- **VIPPrivileges**（VIP特权表）
   - **特权ID** (PrivilegeID): 主键，唯一标识每个特权。
   - **特权名称** (PrivilegeName): 特权的名称。
   - **特权描述** (PrivilegeDescription): 描述特权的详情。
   - **所需等级ID** (RequiredLevelID): 外键，关联到VIPLevels表的LevelID，指定获取该特权所需的VIP等级。

- **VIPOrders**（VIP购买订单表）
   - **订单ID** (OrderID): 主键，唯一标识每个订单。
   - **关联用户ID** (UserID): 外键，关联到Users表的UserID。
   - **订单总金额** (TotalAmount): 订单的总金额。
   - **折扣金额** (DiscountAmount): 订单的折扣金额。
   - **积分返还量** (PointReturn): 购买此订单后返还的积分数量。
   - **下单时间** (OrderTime): 记录订单的创建时间。

### 审核系统
- **审核状态码表(ReviewCodes)**
    - **审核状态ID**(ReviewID):物品的审核状态的ID，便于快速确认物品状态 (00, 01, 99)  
    - **审核状态**(ReviewInfo): 物品的审核状态值 (Pending, Approved, Rejected)  

- **物品审核状态关联表 (ItemReviewLinks)**
    - （主码）发布时间码：物品发布/提交修改时的时间码，便于标记同一个物品修改时的情况
    - （主码）物品ID：略
    - 图片ID：略
    - 标签ID：略
    - 审核状态ID：略
