using DatabaseProject.DataAccessLayer.CommunityFeatureDAL;
using Oracle.ManagedDataAccess.Client;
using SQLOperation.PublicAccess.Templates.SQLManager;
using System.Reflection;
using System.Text.Json;


namespace DatabaseProject.BusinessLogicLayer.CommunityFeatureBLL
{
    public class CommunityFeatureBusiness<T> : ICommunityFeatureBusiness<T>
    {
        protected IMergedDAL mergedDAL;
        protected string tableName;
        public CommunityFeatureBusiness(Connection connection)
        {
            mergedDAL = new MergedDAL(connection);
            tableName = typeof(T).Name.ToUpper(); // 获取类型名并转换为大写
        }


        // 打包数据
        public T PackageData(params object[] parameters)
        {

            PropertyInfo[] properties = typeof(T).GetProperties();


            if (parameters.Length != properties.Length)
            {
                throw new Exception($"参数个数 ({parameters.Length}) 与类型 T 中属性个数 ({properties.Length}) 不匹配。");


            }

            // 创建类型 T 的实例并设置属性值
            T instance = Activator.CreateInstance<T>();
            for (int i = 0; i < parameters.Length; i++)
            {
                object value = parameters[i];
                PropertyInfo property = properties[i];

                // 检查参数类型是否与属性类型一致
                if (value != null && !property.PropertyType.IsAssignableFrom(value.GetType()))
                {
                    throw new Exception($"参数类型 ({value.GetType()}) 在索引 {i} 处与属性类型 ({property.PropertyType}) 不匹配。");

                }

                // 设置属性值
                property.SetValue(instance, value);
            }

            return instance;
        }

        // 映射数据
        public T MapDictionaryToObject(Dictionary<string, object> record)
        {
            T instance = Activator.CreateInstance<T>();
            PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            if (record == null)
            {
                return default(T);
            }

            foreach (var kvp in record)
            {
                // 将 record 中的键转换为大写，以便与 T 的属性名匹配
                string propertyName = kvp.Key.ToUpper();
                object value = kvp.Value;


                // 查找与键匹配的属性
                PropertyInfo property = properties.FirstOrDefault(p => p.Name.ToUpper() == propertyName);

                if (property != null && property.CanWrite)
                {
                    // 尝试将 value 转换为属性的类型并设置属性值
                    try
                    {
                        string stringValue = value.ToString();
                        object convertedValue = Convert.ChangeType(stringValue, property.PropertyType);
                        property.SetValue(instance, convertedValue);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"设置属性 {propertyName} 时发生错误：{ex.Message}");

                    }
                }
            }

            return instance;
        }


        // json拆包并解包1
        public List<T> ParseAndRepackageJsonList(string json)
        {
            try
            {
                // 尝试解析 JSON 数据为 List<Dictionary<string, object>>
                List<Dictionary<string, object>> rowList = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(json);

                List<T> result = new List<T>();

                if (rowList != null)
                {
                    foreach (var record in rowList)
                    {
                        // 创建类型 T 的实例并映射数据

                        T instance = MapDictionaryToObject(record);

                        result.Add(instance);
                    }

                    return result;
                }
                else
                {

                    Console.WriteLine("JSON 数据解析失败.");
                    throw new Exception("JSON 数据解析失败.");
                }
            }
            catch (JsonException jsonEx)
            {

                Console.WriteLine($"JSON 解析错误: {jsonEx.Message}");
                throw new Exception($"JSON 解析错误: {jsonEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"处理 JSON 数据时发生错误: {ex.Message}");
                throw new Exception($"处理 JSON 数据时发生错误: {ex.Message}");
            }
        }
        // 添加事务 返回id 或者false
        public int AddBusiness(List<string> columnNames, string targetColumn, T instance)
        {
            try
            {
                // 将 columnNames 中的所有元素转换为大写
                columnNames = columnNames.Select(name => name.ToUpper()).ToList();

                //获取T中所有属性值并大写
                PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                Dictionary<string, PropertyInfo> propertyDict = properties.ToDictionary(p => p.Name.ToUpper(), p => p);

                List<object> values = new List<object>();

                // 构造列名和对应的值
                foreach (string columnName in columnNames)
                {
                    // 找到与 columnName 对应的属性
                    if (propertyDict.TryGetValue(columnName, out PropertyInfo property))
                    {
                        // 获取属性值
                        object propertyValue = property.GetValue(instance);
                        values.Add(propertyValue);
                    }
                    else
                    {
                        // 如果找不到对应的属性，抛出异常
                        throw new Exception($"Property {columnName} not found in instance of {typeof(T).Name}");
                    }
                }

                // 调用 MergedDAL 插入数据

                Tuple<bool, string> success = mergedDAL.InsertAndGetValue(tableName, targetColumn, columnNames, values);

                // 如果插入成功
                if (success.Item1 == true)
                {
                    int if_success = Convert.ToInt32(success.Item2);

                    return if_success;

                }
                else
                {
                    // 如果找不到对应的属性，抛出异常
                    throw new Exception(success.Item2);
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine($"添加记录时发生错误：{ex.Message}");
                throw new Exception($"添加记录时发生错误：{ex.Message}");

            }
        }

        // 查询事务
        public List<T> QueryBusiness(Dictionary<string, object> condition,string conditionType)
        {
            try
            {
                Tuple<bool, string> success = mergedDAL.QueryTable(tableName, condition,conditionType);

                if (success.Item1 == true)
                {
                    // 为空不报错
                    if (success.Item2 == null)
                    {
                        return new List<T> ();
                    }
                    //这里解析错误都丢出去了

                    List<T> result = ParseAndRepackageJsonList(success.Item2);
                    return result;
                }
                else
                {
                    throw new Exception($"查询表时发生错误：{success.Item2}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }


        }

        // 删除事务
        public bool RemoveBusiness(Dictionary<string, object> condition)
        {
            Tuple<bool, string> success = mergedDAL.DeleteTable(tableName, condition);
            if (success.Item1 == true)
            {
                return true;
            }
            else
            {
                throw new Exception(success.Item2);
            }
        }

        // 更新事务 待定 待定
        public bool UpdateBusiness(Dictionary<string, object> UpdateColumns, Dictionary<string, object> ConditionColumns)
        {
            Tuple<bool, string> success = mergedDAL.UpdateTable(tableName, UpdateColumns, ConditionColumns);
            if (success.Item1 == true)
            {
                return true;
            }
            else
            {
                throw new Exception(success.Item2);
            }
        }

        
        // 任意指定where语句
        public List<T> QueryTableWithWhereBusiness(string whereClause, OracleParameter[] parameters)
        {
            try
            {
                Tuple<bool, string> success = mergedDAL.QueryTableWithWhere(tableName,whereClause,parameters);

                if (success.Item1 == true)
                {
                    // 为空不报错
                    if (success.Item2 == null)
                    {
                        return new List<T>();
                    }
                    //这里解析错误都丢出去了

                    List<T> result = ParseAndRepackageJsonList(success.Item2);
                    return result;
                }
                else
                {
                    throw new Exception($"查询错误: {success.Item2}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        // 自定义from子句和where子句的方法
        public List<Dictionary<string, object>> QueryTableWithFromAndWhereBusiness(string fromClause, string whereClause, OracleParameter[] parameters)
        {
            try
            {
                // 调用 QueryWithCustomFromAndWhere 方法进行查询
                Tuple<bool, string> success = mergedDAL.QueryWithCustomFromAndWhere(fromClause, whereClause, parameters);

                if (success.Item1)
                {
                    // 如果结果为空，不报错，返回空列表
                    if (string.IsNullOrEmpty(success.Item2))
                    {
                        return new List<Dictionary<string, object>>();
                    }

                    // 解析 JSON 结果
                    List<Dictionary<string, object>> rowList = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(success.Item2);
                    return rowList ?? new List<Dictionary<string, object>>();// 如果解析为空 则返回空列表
                }
                else
                {
                    throw new Exception($"查询错误: {success.Item2}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


    }
}
