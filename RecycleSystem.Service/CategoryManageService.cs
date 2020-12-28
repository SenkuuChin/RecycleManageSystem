using Microsoft.EntityFrameworkCore;
using RecycleSystem.Data.Data.CategoryDTO;
using RecycleSystem.DataEntity.Entities;
using RecycleSystem.IService;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace RecycleSystem.Service
{
    public class CategoryManageService : ICategoryManageService
    {
        private readonly DbContext _dbContext;
        public CategoryManageService(DbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public bool AddCategory(CategoryInput categoryInput, out string msg)
        {
            IQueryable<Categorylnfo> categorylnfos = _dbContext.Set<Categorylnfo>();
            Categorylnfo categorylnfo = categorylnfos.Where(c => c.CategoryId == categoryInput.CategoryId).FirstOrDefault();
            if (categorylnfo == null)
            {
                Categorylnfo categorylnfo1 = categorylnfos.Where(c => c.CategoryName == categoryInput.CategoryName).FirstOrDefault();
                if (categorylnfo1==null)
                {
                    try
                    {
                        Categorylnfo newCategory = new Categorylnfo
                        {
                            CategoryId = categoryInput.CategoryId,
                            CategoryName = categoryInput.CategoryName,
                            CurrentPrice = categoryInput.CurrentPrice,
                            Unit = categoryInput.Unit,
                            DelFlag = false,
                            AddTime = DateTime.Now
                        };
                        _dbContext.Set<Categorylnfo>().Add(newCategory);
                        if (_dbContext.SaveChanges()>0)
                        {
                            msg = "添加成功！";
                            return true;
                        }
                        msg = "失败！内部出现异常！";
                        return false;
                    }
                    catch (Exception ex)
                    {
                        msg = ex.Message;
                        return false;
                        
                    }
                }
                msg = "该类名已存在！";
                return false;
            }
            msg = "类目编号已存在！";
            return false;
        }

        public bool BanCategory(int id, out string msg)
        {
            Categorylnfo categorylnfo = _dbContext.Set<Categorylnfo>().Find(id);
            if (categorylnfo != null)
            {
                try
                {
                    if (categorylnfo.DelFlag == false)
                    {
                        categorylnfo.DelFlag = true;
                        if (_dbContext.SaveChanges() > 0)
                        {
                            msg = "已禁用";
                            return true;
                        }
                        msg = "错误！内部出现异常！";
                        return false;
                    }
                    categorylnfo.DelFlag = false;
                    if (_dbContext.SaveChanges() > 0)
                    {
                        msg = "已开启";
                        return true;
                    }
                    msg = "错误！内部出现异常！";
                    return false;
                }
                catch (Exception ex)
                {
                    msg = ex.Message;
                    return false;
                }
            }
            msg = "找不到该类目信息！数据可能被篡改！";
            return false;
        }
        /// <summary>
        /// 为页面展示数据
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="count"></param>
        /// <param name="queryInfo"></param>
        /// <returns></returns>
        public IEnumerable<CategoryOutput> GetCategories(int page, int limit, out int count, string queryInfo)
        {
            IQueryable<Categorylnfo> categorylnfos = _dbContext.Set<Categorylnfo>();
            count = categorylnfos.Count();
            IEnumerable<CategoryOutput> categories = (from c in categorylnfos
                                                      where c.CategoryName.Contains(queryInfo) || queryInfo == null
                                                      select new CategoryOutput
                                                      {
                                                          Id = c.Id,
                                                          CategoryId = c.CategoryId,
                                                          CategoryName = c.CategoryName,
                                                          CurrentPrice = c.CurrentPrice,
                                                          Unit = c.Unit,
                                                          DelFlag = c.DelFlag,
                                                          AddTime = c.AddTime
                                                      }).OrderBy(o => o.Id).Skip((page - 1) * limit).Take(limit).ToList();
            return categories;
        }
        /// <summary>
        /// 给其它功能提供相应数据
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CategoryOutput> GetCategories()
        {
            IEnumerable<CategoryOutput> categories = (from c in _dbContext.Set<Categorylnfo>()
                                                      where c.DelFlag == false
                                                      select new CategoryOutput
                                                      {
                                                          Id = c.Id,
                                                          CategoryId = c.CategoryId,
                                                          CategoryName = c.CategoryName,
                                                          CurrentPrice = c.CurrentPrice
                                                      }).OrderBy(o => o.Id).ToList();
            return categories;
        }

        public CategoryOutput GetCategory(int id)
        {
            Categorylnfo categorylnfo = _dbContext.Set<Categorylnfo>().Find(id);
            if (categorylnfo != null)
            {
                CategoryOutput output = new CategoryOutput
                {
                    CategoryId = categorylnfo.CategoryId,
                    CategoryName = categorylnfo.CategoryName,
                    CurrentPrice = categorylnfo.CurrentPrice,
                    Unit = categorylnfo.Unit,
                    AddTime = categorylnfo.AddTime
                };
                return output;
            }
            return null;
        }

        public bool UpdateCategoryById(CategoryInput categoryInput, out string msg)
        {
            IQueryable<Categorylnfo> categorylnfos = _dbContext.Set<Categorylnfo>();
            Categorylnfo categorylnfo = categorylnfos.Where(c => c.CategoryId == categoryInput.CategoryId).FirstOrDefault();
            if (categorylnfo != null)
            {
                try
                {
                    Categorylnfo categorylnfo1 = categorylnfos.Where(c => c.CategoryName == categoryInput.CategoryName).FirstOrDefault();
                    if (categorylnfo1 == null || categoryInput.CategoryName == categorylnfo.CategoryName)
                    {
                        categorylnfo.CategoryName = categoryInput.CategoryName;
                        categorylnfo.CurrentPrice = categoryInput.CurrentPrice;
                        if (_dbContext.SaveChanges() > 0)
                        {
                            msg = "修改成功！";
                            return true;
                        }
                        msg = "错误！内部出现异常！";
                        return false;
                    }
                    msg = "该类名已存在！";
                    return false;
                }
                catch (Exception ex)
                {
                    msg = ex.Message;
                    return false;
                }
            }
            msg = "找不到该类目数据！数据可能被篡改！";
            return false;
        }
    }
}
