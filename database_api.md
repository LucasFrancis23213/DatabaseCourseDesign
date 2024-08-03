# API 接口设计

## 用户消息 (User Messages)
### 1. 获取用户消息列表
- **URL**: `/api/messages_get`
- **方法**: `GET`
- **参数**:
  - `user_id` (必需): 用户ID
- **说明**: 获取指定用户的所有消息。
- **对应表**: `用户消息表`

### 2. 发送消息
- **URL**: `/api/messages_send`
- **方法**: `POST`
- **参数**:
  - `sender_user_id` (必需): 发送用户ID
  - `receiver_user_id` (必需): 接收用户ID
  - `message_content` (必需): 消息内容
  - `message_type` (必需): 消息类型
- **说明**: 发送一条新消息。
- **对应表**: `用户消息表`


## 用户动态 (User Posts)
### 1. 获取用户动态
- **URL**: `/api/posts_get`
- **方法**: `GET`
- **参数**:
  - `user_id` (必需): 用户ID
- **说明**: 获取指定用户的所有动态。
- **对应表**: `用户动态表`

### 2. 发表动态
- **URL**: `/api/posts_pose`
- **方法**: `POST`
- **参数**:
  - `user_id` (必需): 用户ID
  - `content` (必需): 动态内容
- **说明**: 发表一条新动态。
- **对应表**: `用户动态表`

## 用户关系 (User Relationships)
### 1. 获取用户关系
- **URL**: `/api/relationships`
- **方法**: `GET`
- **参数**:
  - `user_id` (必需): 用户ID
- **说明**: 获取指定用户的所有关系。
- **对应表**: `用户关系表`


## 关注列表 (Follow List)
### 1. 获取关注列表
- **URL**: `/api/follows_get`
- **方法**: `GET`
- **参数**:
  - `user_id` (必需): 用户ID
- **说明**: 获取指定用户的关注列表。
- **对应表**: `关注列表表`


## 问题 (Questions)
### 1. 获取问题列表
- **URL**: `/api/questions_get`
- **方法**: `POST`
- **参数**:
  - `item_id` (可选): 物品ID
  - `user_id` (可选): 用户ID
- **说明**: 获取所有问题，可以根据物品ID或用户ID进行过滤。
- **对应表**: `问题表`

### 2. 提问
- **URL**: `/api/questions_pose`
- **方法**: `POST`
- **参数**:
  - `user_id` (必需): 用户ID
  - `item_id` (可选): 物品ID
  - `question_content` (必需): 问题内容
- **说明**: 提交一个新问题。
- **对应表**: `问题表`

## 回答 (Answers)
### 1. 获取回答列表
- **URL**: `/api/answers_get`
- **方法**: `GET`
- **参数**:
  - `question_id` (必需): 问题ID
- **说明**: 获取指定问题的所有回答。
- **对应表**: `回答表`

### 2. 提交回答
- **URL**: `/api/answers_pose`
- **方法**: `POST`
- **参数**:
  - `question_id` (必需): 问题ID
  - `user_id` (必需): 用户ID
  - `answer_content` (必需): 回答内容
- **说明**: 提交一个新回答。
- **对应表**: `回答表`

