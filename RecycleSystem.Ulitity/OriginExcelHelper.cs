using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using RecycleSystem.Data.Data.UserManageDTO;
using RecycleSystem.Data.Data.WareHouseDTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;

namespace Senkuu.MaterialSystem.Utility
{
    public class OriginExcelHelper
    {
        public static DataTable ExcelToDataTable(Stream stream, string fileType, out string strMsg, string sheetName = null)
        {
            //out strMsg 是为了让上级一层能看到错误信息而设立。
            strMsg = "";
            DataTable dataTable = new DataTable();
            ISheet sheet = null;
            IWorkbook workbook = null;
            try
            {
                #region 判断Excel版本
                if (fileType == ".xlsx")
                {
                    workbook = new XSSFWorkbook(stream);
                }
                else if (fileType == ".xls")
                {
                    workbook = new HSSFWorkbook(stream);
                }
                else
                {
                    throw new Exception("传入的不是Excel文件！");
                }
                #endregion
                if (!string.IsNullOrEmpty(sheetName))
                {
                    sheet = workbook.GetSheet(sheetName);
                    if (sheet == null)
                    {
                        sheet = workbook.GetSheetAt(0);
                    }
                }
                else
                {
                    sheet = workbook.GetSheetAt(0);
                }
                if (sheet != null)
                {
                    //获取第一行，作为dataTable数据行标题
                    IRow firstRow = sheet.GetRow(1);
                    int cellCount = firstRow.LastCellNum;
                    for (int i = firstRow.FirstCellNum; i < cellCount; i++)
                    {
                        ICell cell = firstRow.GetCell(i);
                        if (cell != null)
                        {
                            string cellValue = cell.StringCellValue.Trim();
                            if (!string.IsNullOrEmpty(cellValue))
                            {
                                if (cellValue == "用户ID")
                                {
                                    cellValue = "UserId";
                                }
                                if (cellValue == "用户名")
                                {
                                    cellValue = "UserName";
                                }
                                if (cellValue == "性别")
                                {
                                    cellValue = "Gender";
                                }
                                if (cellValue == "电话号码")
                                {
                                    cellValue = "Tel";
                                }
                                if (cellValue == "邮箱地址")
                                {
                                    cellValue = "Email";
                                }
                                DataColumn dataColumn = new DataColumn(cellValue);
                                dataTable.Columns.Add(dataColumn);
                            }
                        }
                    }
                    //获取第二行及后面的数据
                    DataRow dataRow = null;
                    //需从一开始，不然会从第一行开始遍历。把标题又拿一遍下来
                    for (int j = sheet.FirstRowNum + 2; j <= sheet.LastRowNum; j++)
                    {
                        IRow row = sheet.GetRow(j);
                        dataRow = dataTable.NewRow();
                        if (row == null || row.FirstCellNum < 0)
                        {
                            continue;
                        }
                        // row 行。 cell列
                        for (int i = row.FirstCellNum; i < 6; i++)
                        {
                            ICell cellData = row.GetCell(i);
                            if (cellData != null)
                            {
                                if (cellData.CellType == CellType.Numeric)
                                {
                                    if (DateUtil.IsCellDateFormatted(cellData))
                                    {
                                        dataRow[i] = cellData.DateCellValue;
                                    }
                                    else
                                    {
                                        dataRow[i] = cellData.ToString().Trim();
                                    }
                                }
                                else
                                {
                                    //赋上值
                                    dataRow[i] = cellData.ToString().Trim();
                                }
                            }
                        }
                        dataTable.Rows.Add(dataRow);
                    }
                }
                else
                {
                    throw new Exception("没有获取到Excel中的数据表！");
                }
            }
            catch (Exception ex)
            {
                strMsg = ex.Message;
            }
            workbook.Close();
            return dataTable;
        }
        public static List<UserInput> ConvertToList(DataTable dt, out string msg)
        {
            // 定义集合
            List<UserInput> ts = new List<UserInput>();
            // 获得此模型的类型
            Type type = typeof(UserInput);
            //定义一个临时变量
            string tempName = string.Empty;
            //遍历DataTable中所有的数据行
            foreach (DataRow dr in dt.Rows)
            {
                UserInput t = new UserInput();
                // 获得此模型的公共属性
                PropertyInfo[] propertys = t.GetType().GetProperties();
                //遍历该对象的所有属性
                foreach (PropertyInfo pi in propertys)
                {
                    tempName = pi.Name;//将属性名称赋值给临时变量
                    //检查DataTable是否包含此列（列名==对象的属性名）  
                    if (dt.Columns.Contains(tempName))
                    {
                        // 判断此属性是否有Setter
                        if (!pi.CanWrite) continue;//该属性不可写，直接跳出

                        //取值
                        object value = dr[tempName];

                        if (tempName == "Gender")
                        {
                            try
                            {
                                if (value.ToString() == "男")
                                {
                                    value = true;
                                }
                                if (value.ToString() == "女")
                                {
                                    value = false;
                                }
                                //else if (value.ToString().Trim() == null || ((bool)value != true && (bool)value != false))
                                //{
                                //    value = null;
                                //}
                            }
                            catch (Exception ex)
                            {
                                msg = "性别栏处有空缺数据！错误信息：" + ex.Message + "<br>详细信息：" ;
                                return null;
                            }



                        }

                        //如果非空，则赋给对象的属性
                        if (value != DBNull.Value)
                            pi.SetValue(t, value, null);
                    }
                }
                //对象添加到泛型集合中
                ts.Add(t);
            }
            msg = "OK";
            return ts;
        }
    }
}