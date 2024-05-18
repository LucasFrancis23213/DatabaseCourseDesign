# API接口设计

## VIP充值功能 (VIP Recharge)

### 1. 生成充值二维码
- **URL**: `/api/vip/recharge`
- **方法**: `POST`  
- **参数**:
  - `user_id` (必需): 用户ID
  - `duration_months` (必需): 充值时长(月)
- **说明**: 选择充值时长,生成充值二维码
- **对应表**: `VIPMembers`, `VIPOrders`
- **响应**:
  - `{ qrCodeUrl: string }`

## VIP特权功能 (VIP Privileges)
### 1. 获取VIP会员列表 
- **URL**: `/api/vip/members`
- **方法**: `GET`
- **说明**: 获取所有VIP会员信息(仅管理员可访问)
- **对应表**: `VIPMembers` 
- **响应**:
  - `[ { userId: number, username: string, vipLevel: number, expirationDate: date }, ...]`

## 管理员编辑VIP信息 (Admin Edit VIP)
### 1. 修改VIP用户信息
- **URL**: `/api/admin/vip/:userId`  
- **方法**: `PUT`
- **参数**:
  - `vipLevel` (可选): VIP等级  
  - `expirationDate` (可选): 到期日期
- **对应表**: `VIPMembers`, `User`, `Admin`, `AdminEditVIP`

### 2. 获取VIP编辑记录
- **URL**: `/api/admin/vip/edits`
- **方法**: `GET`  
- **说明**: 获取VIP编辑记录(仅管理员可访问)
- **对应表**: `AdminEditVIP`
- **响应**:
  - `[ { editId: number, userId: number, field: string, oldValue: any, newValue: any, editedAt: date, adminId: number }, ...]`

## 广告呈现 (Ad Rendering)  
### 1. 获取广告列表
- **URL**: `/api/ads`
- **方法**: `GET`
- **说明**: 获取广告列表用于展示
- **对应表**: `Advertisements`
- **响应**:
  - `[ { adId: number, imageUrl: string, link: string, ... }, ...]`

### 2. 记录用户查看广告
- **URL**: `/api/user/:userId/ads/shown`
- **方法**: `POST`
- **参数**:
  - `adIds` (必需): 广告ID列表
- **说明**: 记录用户查看广告  
- **对应表**: `AdShowStatistics`, `User`

## 用户点击广告 (Ad Clicks)
### 1. 记录用户点击广告
- **URL**: `/api/user/:userId/ads/clicked` 
- **方法**: `POST`
- **参数**:
  - `adId` (必需): 广告ID
- **说明**: 记录用户点击广告
- **对应表**: `AdClickStatistics`, `User`  

## 管理员编辑广告 (Admin Edit Ads)
### 1. 获取广告列表
- **URL**: `/api/admin/ads`
- **方法**: `GET`
- **说明**: 获取所有广告信息(仅管理员可访问)
- **对应表**: `Advertisements`
- **响应**:
  - `[ { adId: number, ... }, ...]`

### 2. 新增广告
- **URL**: `/api/admin/ads`
- **方法**: `POST`
- **参数**:
  - `imageUrl` (必需): 广告图片URL
  - `link` (必需): 广告链接
  - ... 其他广告字段
- **对应表**: `Advertisements`, `AdminEditAd`  

### 3. 修改广告
- **URL**: `/api/admin/ads/:adId`
- **方法**: `PUT` 
- **参数**:
  - `imageUrl` (可选): 广告图片URL
  - `link` (可选): 广告链接
  - ... 其他广告字段
- **对应表**: `Advertisements`, `AdminEditAd`

### 4. 删除广告
- **URL**: `/api/admin/ads/:adId`
- **方法**: `DELETE`
- **对应表**: `Advertisements`, `AdminEditAd`  

### 5. 获取广告编辑记录
- **URL**: `/api/admin/ads/edits`
- **方法**: `GET`
- **说明**: 获取广告编辑记录(仅管理员可访问)
- **对应表**: `AdminEditAd`
- **响应**:
  - `[ { editId: number, adId: number, field: string, oldValue: any, newValue: any, editedAt: date, adminId: number }, ...]`

## 用户消息 (User Messages)

### 1. 获取用户消息列表

- **URL**: `/api/messages_get`

- **方法**: `GET`

- 参数

	:

	- `user_id` (必需): 用户ID

- **说明**: 获取指定用户的所有消息。

- **对应表**: `用户消息表`

### 2. 发送消息

- **URL**: `/api/messages_send`

- **方法**: `POST`

- 参数

	:

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

- 参数

	:

	- `user_id` (必需): 用户ID

- **说明**: 获取指定用户的所有动态。

- **对应表**: `用户动态表`

### 2. 发表动态

- **URL**: `/api/posts_pose`

- **方法**: `POST`

- 参数

	:

	- `user_id` (必需): 用户ID
	- `content` (必需): 动态内容

- **说明**: 发表一条新动态。

- **对应表**: `用户动态表`

## 用户关系 (User Relationships)

### 1. 获取用户关系

- **URL**: `/api/relationships`

- **方法**: `GET`

- 参数

	:

	- `user_id` (必需): 用户ID

- **说明**: 获取指定用户的所有关系。

- **对应表**: `用户关系表`

## 关注列表 (Follow List)

### 1. 获取关注列表

- **URL**: `/api/follows_get`

- **方法**: `GET`

- 参数

	:

	- `user_id` (必需): 用户ID

- **说明**: 获取指定用户的关注列表。

- **对应表**: `关注列表表`

## 问题 (Questions)

### 1. 获取问题列表

- **URL**: `/api/questions_get`

- **方法**: `POST`

- 参数

	:

	- `item_id` (可选): 物品ID
	- `user_id` (可选): 用户ID

- **说明**: 获取所有问题，可以根据物品ID或用户ID进行过滤。

- **对应表**: `问题表`

### 2. 提问

- **URL**: `/api/questions_pose`

- **方法**: `POST`

- 参数

	:

	- `user_id` (必需): 用户ID
	- `item_id` (可选): 物品ID
	- `question_content` (必需): 问题内容

- **说明**: 提交一个新问题。

- **对应表**: `问题表`

## 回答 (Answers)

### 1. 获取回答列表

- **URL**: `/api/answers_get`

- **方法**: `GET`

- 参数

	:

	- `question_id` (必需): 问题ID

- **说明**: 获取指定问题的所有回答。

- **对应表**: `回答表`

### 2. 提交回答

- **URL**: `/api/answers_pose`

- **方法**: `POST`

- 参数

	:

	- `question_id` (必需): 问题ID
	- `user_id` (必需): 用户ID
	- `answer_content` (必需): 回答内容

- **说明**: 提交一个新回答。

- **对应表**: `回答表`