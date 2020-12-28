using Microsoft.EntityFrameworkCore;
using RecycleSystem.Data.Data.WareHouseDTO;
using RecycleSystem.DataEntity.Entities;
using RecycleSystem.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecycleSystem.Service
{
    public class WareHouseService : IWareHouseService
    {
        private readonly DbContext _dbContext;
        public WareHouseService(DbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IEnumerable<GoodsOutput> GetGoodsInputInfo(int page, int limit, out int count, string queryInfo)
        {
            IQueryable<UserInfo> userInfos = _dbContext.Set<UserInfo>();
            IQueryable<Categorylnfo> categorylnfos = _dbContext.Set<Categorylnfo>();
            IQueryable<InputInfo> inputInfos = _dbContext.Set<InputInfo>();
            count = inputInfos.Count();
            IEnumerable<GoodsOutput> goodsOutputs = (from i in inputInfos
                                                     where i.InstanceId.Contains(queryInfo) || queryInfo == null
                                                     select new GoodsOutput
                                                     {
                                                         Id = i.Id,
                                                         InstanceId = i.InstanceId,
                                                         Category = (from g in categorylnfos where g.CategoryId == i.CategoryId select new { g.CategoryName }).Select(s => s.CategoryName).FirstOrDefault(),
                                                         Name = i.Name,
                                                         Unit = i.Unit,
                                                         Num = i.Num,
                                                         InputUser = (from u in userInfos where u.UserId == i.InputUser select new { u.UserName }).Select(s => s.UserName).FirstOrDefault(),
                                                         AddTime=i.AddTime
                                                     }).OrderBy(o => o.Id).Skip((page - 1) * limit).Take(limit).ToList();
            return goodsOutputs;
        }

        public IEnumerable<WareHouseOutput> GetGoodsOutputs(int page, int limit, out int count, string queryInfo)
        {
            IQueryable<UserInfo> userInfos = _dbContext.Set<UserInfo>();
            IQueryable<Categorylnfo> categorylnfos = _dbContext.Set<Categorylnfo>();
            IQueryable<WareHouseInfo> Infos = _dbContext.Set<WareHouseInfo>();
            count = Infos.Count();
            IEnumerable<WareHouseOutput> goodsOutputs = (from i in Infos
                                                     where i.InstanceId.Contains(queryInfo) || queryInfo == null
                                                     select new WareHouseOutput
                                                     {
                                                         Id = i.Id,
                                                         InstanceId = i.InstanceId,
                                                         CategoryName = (from g in categorylnfos where g.CategoryId == i.CategoryId select new { g.CategoryName }).Select(s => s.CategoryName).FirstOrDefault(),
                                                         Name = i.Name,
                                                         Unit = i.Unit,
                                                         Num = i.Num,
                                                         
                                                         AddTime = i.AddTime
                                                     }).OrderBy(o => o.Id).Skip((page - 1) * limit).Take(limit).ToList();
            return goodsOutputs;
        }

        public bool UploadMaterialaInfo(IEnumerable<GoodsInput> goodsInputs, string userId, string msg)
        {
            //遍历
            foreach (var item in goodsInputs)
            {
                //输入信息表
                InputInfo inputInfo = new InputInfo
                {
                    InstanceId = item.InstanceId,
                    CategoryId = item.CategoryId,
                    InputUser = userId,
                    Name = item.Name,
                    Num = item.Num,
                    Unit = item.Unit,
                    AddTime = DateTime.Now
                };
                _dbContext.Set<InputInfo>().Add(inputInfo);

                //仓库信息表
                string instanceId = "W" + item.InstanceId.Substring(3);
                WareHouseInfo goods = new WareHouseInfo
                {
                    InstanceId = instanceId,
                    CategoryId = item.CategoryId,
                    Name = item.Name,
                    Num = item.Num,
                    Unit = item.Unit,
                    IsPress = false,
                    DelFlag = false,
                    AddTime = DateTime.Now
                };
                _dbContext.Set<WareHouseInfo>().Add(goods);
                //上传的同时要把相应的订单改为已完成
            }
            if (_dbContext.SaveChanges()>0)
            {
                msg = "导入成功！";
                return true;
            }
            msg = "失败！内部错误！";
            return false;
        }
    }
}
