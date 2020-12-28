using System;
using System.Collections.Generic;
using System.Text;

namespace Senkuu.MaterialSystem.Model
{
    /// <summary>
    /// 针对layui的数据返回实体类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataResult<T>
    {
        /// <summary>
        /// 接口状态
        /// </summary>
        public int code { get; set; }

        /// <summary>
        /// 提示文本
        /// </summary>
        public string msg { get; set; }

        /// <summary>
        /// 数据长度
        /// </summary>
        public int count { get; set; }

        /// <summary>
        /// 数据结果
        /// </summary>
        public T data { get; set; }
    }
}
