using System.Security.Cryptography;
using System.Text;

namespace SQLOperation.BusinessLogicLayer.ManagementFeatureBLL
{
    public class PasswordEncryptor
    {
        public static string EncryptPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // 将密码转换为字节数组
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

                // 生成 SHA-256 哈希
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);

                // 将哈希字节数组转换为 Base64 字符串
                string base64Hash = Convert.ToBase64String(hashBytes);

                // 扩展或裁剪 Base64 字符串以确保结果长度为 255
                string extendedHash = ExtendTo255(base64Hash);

                return extendedHash;
            }
        }

        private static string ExtendTo255(string input)
        {
            StringBuilder extendedBuilder = new StringBuilder(input);

            // 通过重复添加自己的内容，确保长度达到 255
            while (extendedBuilder.Length < 255)
            {
                extendedBuilder.Append(input);
            }

            // 裁剪到 255 个字符
            return extendedBuilder.ToString().Substring(0, 255);
        }
    }
}