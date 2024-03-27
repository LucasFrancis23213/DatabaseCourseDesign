# DatabaseCourseDesign
**代码规范**
1. 一个.cs文件只放一个类
2. 每个人的类都要装到自己的namespace命名空间里，不要出现野生的class
3. 所有类名/函数名/变量名都遵循Pascel命名方式**即所有单词的首字母大写**
- 例如： public class UserInteract{  
    public bool IsValid;  
    public static void UserLogin(){}  
}
4. 添加适当的注释，对于实现核心功能的函数（自己把握就行）**必须**写注释（vs2022里输入 /// 似乎会自动生成辅助注释的东西）
5. 将代码合理分配到不同的文件夹里，合理布局

**github使用要求**
1. 代码**禁止**直接传到main分支里，先传到分配给你的branch里，经测试后由测试人员merge到main分支里
2. 往github上提交时除了传代码还要传**更新日志**（内容包括但不限于：提交日期、新建了什么类、更改了以前代码里的类哪些成员/函数**的名字**）
3. 每天晚8点（待定）前提交代码到github仓库

**技术栈**
1. 前端 vue 
![VueSuggestion](ImageResource/VueSuggestion.jpg)
2. 后端C#