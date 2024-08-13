using System.Text.Json;

namespace WebAppTest.APILayer.CommunityFeatureAPI
{
    public static class ControllerHelper
    {
        /// <summary>
        /// 从字典中安全地获取字符串值，并验证其不为 null 或空。
        /// </summary>
        /// <param name="request">包含请求参数的字典。</param>
        /// <param name="key">要检索的键。</param>
        /// <returns>字符串值。</returns>
        /// <exception cref="ArgumentException">如果键不存在、值为 null 或值为空。</exception>
        public static string GetSafeString(Dictionary<string, JsonElement> request, string key)
        {
            if (request.TryGetValue(key, out var value) && value.ValueKind == JsonValueKind.String)
            {
                return value.GetString() ?? throw new ArgumentException($"{key} 不能为空");
            }
            throw new ArgumentException($"{key} 未找到或格式不正确");
        }

    }
}
