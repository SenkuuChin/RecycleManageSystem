using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Senkuu.MaterialSystem.Utility
{
    /// <summary>
    /// json.net帮助类：对json.net的进一步封装，
    /// </summary>
    public class JsonNetHelper
    {
        /// <summary>
        /// 小驼峰命名+格式化日期+序列化
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string SerialzeoJsonForCamelCase(object obj)
        {
            //序列化配置
            var setting = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),//配置小驼峰命名
                DateFormatString = "yyyy-MM-dd HH:mm:ss"//格式化日期
            };
            //序列化核心方法
            string str = JsonConvert.SerializeObject(obj, Formatting.Indented, setting);
            return str;
        }
    }
}
