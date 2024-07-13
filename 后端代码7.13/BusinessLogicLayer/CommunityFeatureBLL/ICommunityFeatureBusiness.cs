namespace DatabaseProject.BusinessLogicLayer.CommunityFeatureBLL
{
    public interface ICommunityFeatureBusiness<T>
    {
        // 将参数打包成类型 T 的实例
        T PackageData(params object[] parameters);

        // 解析 JSON 并重新封装为 T 类型的列表
        List<T> ParseAndRepackageJsonList(string json);

        // 添加记录，使用指定的列名和目标列，返回插入的 ID 或抛出异常
        int AddBusiness(List<string> columnNames, string targetColumn, T instance);

        // 根据条件查询记录，返回 T 类型的列表或抛出异常
        List<T> QueryBusiness(Dictionary<string, object> condition, string conditionType);

        // 根据条件删除记录，成功返回 true，否则抛出异常
        bool RemoveBusiness(Dictionary<string, object> condition);

        // 更新记录（待定）
        bool UpdateBusiness(Dictionary<string, object> UpdateColumns, Dictionary<string, object> ConditionColumns);

        //
       
    }

}
