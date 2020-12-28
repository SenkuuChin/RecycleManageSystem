using Microsoft.EntityFrameworkCore;
using RecycleSystem.Data.Data.OrderManageDTO;
using RecycleSystem.Data.Data.WareHouseDTO;
using RecycleSystem.Data.Data.WorkFlowDTO;
using RecycleSystem.DataEntity.Entities;
using RecycleSystem.IService;
using RecycleSystem.Ulitity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecycleSystem.Service
{
    public class OrderManageService : IOrderManageService
    {
        private readonly DbContext _dbContext;
        public OrderManageService(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool AcceptOrder(string oid, string userId, out string message)
        {
            IQueryable<DemandOrderInfo> orderInfos = _dbContext.Set<DemandOrderInfo>();
            DemandOrderInfo orderInfo = orderInfos.Where(o => o.Oid == oid).FirstOrDefault();
            if (orderInfo != null)
            {
                try
                {
                    orderInfo.UserId = userId;
                    orderInfo.Status = (int?)TypeEnum.DemendOrderStatus.Accepted;
                    _dbContext.Set<DemandOrderInfo>().Update(orderInfo);
                    //改变需求订单状态的同时，要往订单表中也插入一条数据。方便后续内部管理者查看
                    //查询是否已经有人接取订单（此功能类似抢购，可能同时并发，因现今技术问题，暂时使用此方法来判断）
                    OrderInfo order = _dbContext.Set<OrderInfo>().Where(o => o.OriginalOrderId == orderInfo.Oid).FirstOrDefault();
                    if (order != null)
                    {
                        message = "该订单已被人接取！";
                        return false;
                    }
                    string instanceId = "O" + DateTime.Now.ToString("yyyyMMddHHmmssffff");//要求要日期的时分秒以及毫秒
                    OrderInfo newOrder = new OrderInfo
                    {
                        InstanceId = instanceId,
                        CategoryId = orderInfo.CategoryId,
                        Name = orderInfo.Name,
                        OriginalOrderId = orderInfo.Oid,
                        EnterpriseId = orderInfo.EnterpriseId,
                        Num = orderInfo.Num,
                        Unit = orderInfo.Unit,
                        ReceiverId = orderInfo.UserId,
                        Status = (int)TypeEnum.OrderStatus.Running,
                        AddTime = DateTime.Now,
                        Url = null,
                    };

                    _dbContext.Set<OrderInfo>().Add(newOrder);
                    if (_dbContext.SaveChanges() > 0)
                    {
                        message = "接单成功！";
                        return true;
                    }
                    message = "失败！内部出现异常";
                    return false;
                }
                catch (Exception ex)
                {
                    message = ex.Message + "  具体原因： " + ex.InnerException.Message;
                    return false;

                }


            }
            message = "订单不存在！";
            return false;
        }

        public bool ApproveSpecialOrderWithdrew(DemandOrderInput demandOrderInput, out string msg)
        {
            IQueryable<WorkFlow> workFlows = _dbContext.Set<WorkFlow>();
            IQueryable<WorkFlowStep> workFlowSteps = _dbContext.Set<WorkFlowStep>();
            WorkFlow workFlow = workFlows.Where(o => o.OrderID == demandOrderInput.Oid).FirstOrDefault();
            if (workFlow != null)
            {
                WorkFlowStep workFlowStep = workFlowSteps.Where(w => w.InstanceId == workFlow.InstanceId).FirstOrDefault();
                if (workFlowStep != null)
                {
                    try
                    {
                        if (workFlowStep.ReviewStatus == (int?)TypeEnum.ReviewStatus.Viewed || workFlow.Status != (int?)TypeEnum.WorkFlowStatus.Applying)
                        {
                            msg = "已审批过！";
                            return false;
                        }
                        workFlowStep.ReviewStatus = (int?)TypeEnum.ReviewStatus.Viewed;
                        workFlowStep.ReviewTime = DateTime.Now;
                        workFlowStep.BackContent = demandOrderInput.BackContent;
                        workFlowStep.isRead = true;
                        DemandOrderInfo orderInfo = _dbContext.Set<DemandOrderInfo>().Where(o => o.Oid == demandOrderInput.Oid).FirstOrDefault();
                        if (demandOrderInput.Decide == 1)
                        {
                            workFlow.Status = (int?)TypeEnum.WorkFlowStatus.UnAccept;
                            orderInfo.Status = (int?)TypeEnum.DemendOrderStatus.ForbinCancel;//把需求订单表的状态改为拒绝撤销
                        }
                        if (demandOrderInput.Decide == 2)
                        {
                            workFlow.Status = (int?)TypeEnum.WorkFlowStatus.Allow;
                            orderInfo.Status = (int?)TypeEnum.DemendOrderStatus.Canceled; //需求订单表里改为已撤销
                            OrderInfo order = _dbContext.Set<OrderInfo>().Where(o => o.OriginalOrderId == orderInfo.Oid).FirstOrDefault();
                            order.Status = (int?)TypeEnum.OrderStatus.Canceled; //订单表改为已撤销
                        }
                        if (_dbContext.SaveChanges() > 0)
                        {
                            msg = "审批成功！";
                            return true;
                        }
                        msg = "失败！内部出现异常！";
                        return false;
                    }
                    catch (Exception ex)
                    {
                        msg = "错误信息：" + ex.Message + "<br>错误内容：" + ex.InnerException.Message;
                        return false;

                    }

                }
            }
            msg = "找不到该事件编号！数据库数据可能遭受到了攻击";
            return false;
        }
        /// <summary>
        /// 确认完成订单
        /// </summary>
        /// <param name="order"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool confirmFinish(OrderInput order, out string msg)
        {
            OrderInfo info = _dbContext.Set<OrderInfo>().Where(o => o.InstanceId == order.InstanceId).FirstOrDefault();
            if (info != null)
            {
                try
                {
                    info.Status = (int)TypeEnum.OrderStatus.Finished;
                    string zid = info.InstanceId.Substring(1);//不要订单号O，只要后面的数字
                    decimal? price = _dbContext.Set<Categorylnfo>().Where(c => c.CategoryId == info.CategoryId).FirstOrDefault().CurrentPrice;
                    int pounds = 1;
                    if (info.Unit == "吨")
                    {
                        pounds *= 2000;
                    }
                    if (info.Unit == "千克")
                    {
                        pounds *= 2;
                    }
                    decimal num = Convert.ToDecimal(info.Num) * pounds;
                    decimal? money = num * price;//获取价钱。
                    //往账单表添加数据
                    RevenueBill revenueBill = new RevenueBill
                    {
                        Zid = "DV" + zid,
                        Oid = info.InstanceId,
                        Name = info.Name,
                        Num = info.Num,
                        Unit = info.Unit,
                        Money = money,
                        AddTime = DateTime.Now
                    };
                    //记录审批信息
                    InputInfo inputInfo = new InputInfo
                    {
                        InstanceId = "GSI" + zid,
                        InputUser = order.OperationID,
                        CategoryId = info.CategoryId,
                        Name = info.Name,
                        Num = info.Num,
                        Unit = info.Unit,
                        AddTime = DateTime.Now

                    };
                    _dbContext.Set<RevenueBill>().Add(revenueBill);
                    _dbContext.Set<InputInfo>().Add(inputInfo);
                    if (_dbContext.SaveChanges() > 0)
                    {
                        msg = "确认成功！";
                        return true;
                    }
                    msg = "错误！内部出现异常！";
                }
                catch (Exception ex)
                {
                    msg = "错误信息：" + ex.Message;
                    if (ex.InnerException!=null)
                    {
                        msg += "错误内容：" + ex.InnerException.Message;
                    }
                    return false;
                }
            }
            msg = "找不到订单编号！";
            return false;
        }

        public bool FinishOrder(string oid ,string pathName, string userId, out string msg)
        {
            OrderInfo orderInfo = _dbContext.Set<OrderInfo>().Where(o => o.OriginalOrderId == oid).FirstOrDefault();
            if (orderInfo!=null)
            {
                DemandOrderInfo demandOrderInfo = _dbContext.Set<DemandOrderInfo>().Where(d => d.Oid == oid).FirstOrDefault();
                if (demandOrderInfo!=null)
                {
                    orderInfo.Url = pathName;
                    orderInfo.FinishedTime = DateTime.Now;
                    demandOrderInfo.Status = (int?)TypeEnum.DemendOrderStatus.Finished;
                    OperateLog newLog = new OperateLog
                    {
                        OperatorId = userId,
                        Info = "完成了 " + orderInfo.InstanceId + " 订单 (已上传图片)",
                        AddTime = DateTime.Now
                    };
                    _dbContext.Set<OperateLog>().Add(newLog);
                    try
                    {
                        if (_dbContext.SaveChanges()>0)
                        {
                            msg = "订单完成！";
                            return true;
                        }
                        msg = "错误！内部出现异常！";
                        return false;
                    }
                    catch (Exception ex)
                    {
                        msg = "错误信息："+ex.Message;
                        if (ex.InnerException!=null)
                        {
                            msg += "详细信息：" + ex.InnerException.Message;
                        }
                        return false;
                        
                    }
                }
                msg = "原订单号不存在！请联系部门主管查看！";
                return false;

            }
            msg = "订单好不存在！请联系主管！";
            return false;
        }

        public IEnumerable<DemandOrderOutput> GetAllOrders(int page, int limit, out int count, string queryInfo)
        {
            IQueryable<UserInfo> userInfos = _dbContext.Set<UserInfo>();
            IQueryable<Categorylnfo> categorylnfos = _dbContext.Set<Categorylnfo>();
            IQueryable<DemandOrderInfo> orderInfos = _dbContext.Set<DemandOrderInfo>(); // To Find The order which is applied by me
            count = orderInfos.Count();
            IEnumerable<DemandOrderOutput> orderOutputs = (from a in orderInfos
                                                           where a.Oid.Contains(queryInfo) || queryInfo == null
                                                           select new DemandOrderOutput
                                                           {
                                                               Id = a.Id,
                                                               Oid = a.Oid,
                                                               Name = a.Name,
                                                               Num = a.Num,
                                                               Unit = a.Unit,
                                                               Enterpriser = (from u in userInfos where u.UserId == a.EnterpriseId select new { u.UserName }).Select(s => s.UserName).FirstOrDefault(),
                                                               Status = a.Status,
                                                               AddTime = a.AddTime,
                                                               Category = (from g in categorylnfos where g.CategoryId == a.CategoryId select new { g.CategoryName }).Select(s => s.CategoryName).FirstOrDefault(),
                                                               Receiver = (from n in userInfos where n.UserId == a.UserId select new { n.UserName }).Select(s => s.UserName).FirstOrDefault(),
                                                               EnterpriseName = (from n in userInfos where n.UserId == a.EnterpriseId select new { n.EnterpriseName }).Select(s => s.EnterpriseName).FirstOrDefault(),
                                                               DelFlag = a.DelFlag
                                                           }).OrderBy(o => o.Id).Skip((page - 1) * limit).Take(limit).ToList();
            return orderOutputs;
        }

        /// <summary>
        /// 获取已完成的订单
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="count"></param>
        /// <param name="queryInfo"></param>
        /// <returns></returns>
        public IEnumerable<DemandOrderOutput> GetFinishedOrder(int page, int limit, out int count, string queryInfo)
        {
            IQueryable<UserInfo> userInfos = _dbContext.Set<UserInfo>();
            IQueryable<Categorylnfo> categorylnfos = _dbContext.Set<Categorylnfo>();
            IQueryable<DemandOrderInfo> orderInfos = _dbContext.Set<DemandOrderInfo>().Where(o => o.Status == (int)TypeEnum.DemendOrderStatus.Finished); // To Find The order which has been finished
            count = orderInfos.Count();
            IEnumerable<DemandOrderOutput> orderOutputs = (from a in orderInfos
                                                           where a.Oid.Contains(queryInfo) || queryInfo == null
                                                           select new DemandOrderOutput
                                                           {
                                                               Id = a.Id,
                                                               Oid = a.Oid,
                                                               Name = a.Name,
                                                               Num = a.Num,
                                                               Unit = a.Unit,
                                                               Enterpriser = (from u in userInfos where u.UserId == a.EnterpriseId select new { u.UserName }).Select(s => s.UserName).FirstOrDefault(),
                                                               Status = a.Status,
                                                               AddTime = a.AddTime,
                                                               Category = (from g in categorylnfos where g.CategoryId == a.CategoryId select new { g.CategoryName }).Select(s => s.CategoryName).FirstOrDefault(),
                                                               Receiver = (from n in userInfos where n.UserId == a.UserId select new { n.UserName }).Select(s => s.UserName).FirstOrDefault(),
                                                               EnterpriseName = (from n in userInfos where n.UserId == a.EnterpriseId select new { n.EnterpriseName }).Select(s => s.EnterpriseName).FirstOrDefault(),
                                                           }).OrderBy(o => o.Id).Skip((page - 1) * limit).Take(limit).ToList();
            return orderOutputs;
        }

        /// <summary>
        /// 获取已完成的订单的信息（页面展开查看信息，提供给审核完成订单）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public OrderOutput GetFinishedOrderInfo(int id)
        {
            IQueryable<UserInfo> userInfos = _dbContext.Set<UserInfo>();
            IQueryable<Categorylnfo> categorylnfos = _dbContext.Set<Categorylnfo>();
            IQueryable<OrderInfo> orderInfos = _dbContext.Set<OrderInfo>(); // To query the order which has been accpeted

            OrderOutput orderOutput = (from a in orderInfos
                                       where a.Id == id
                                       select new OrderOutput
                                       {
                                           Id = a.Id,
                                           InstanceId = a.InstanceId,
                                           Name = a.Name,
                                           Num = a.Num,
                                           Unit = a.Unit,
                                           EnterpriseID = (from u in userInfos where u.UserId == a.EnterpriseId select new { u.UserName }).Select(s => s.UserName).FirstOrDefault(),
                                           Status = a.Status,
                                           AddTime = a.AddTime,
                                           CategoryName = (from g in categorylnfos where g.CategoryId == a.CategoryId select new { g.CategoryName }).Select(s => s.CategoryName).FirstOrDefault(),
                                           Receiver = (from n in userInfos where n.UserId == a.ReceiverId select new { n.UserName }).Select(s => s.UserName).FirstOrDefault(),
                                           EnterpriseName = (from n in userInfos where n.UserId == a.EnterpriseId select new { n.EnterpriseName }).Select(s => s.EnterpriseName).FirstOrDefault(),
                                           OriginalOrder = a.OriginalOrderId,
                                           CategoryId = a.CategoryId,
                                           FinishedTime = a.FinishedTime,
                                           Url = a.Url
                                       }).FirstOrDefault();
            return orderOutput;
        }

        public IEnumerable<WorkFlowOutput> GetFlowOutputs(int page, int limit, out int count, string queryInfo, string userId)
        {
            IQueryable<UserInfo> userInfos = _dbContext.Set<UserInfo>();
            IQueryable<DemandOrderInfo> demandOrderInfos = _dbContext.Set<DemandOrderInfo>();
            IQueryable<WorkFlow> workFlows = _dbContext.Set<WorkFlow>().Where(u => u.CurrentReviewer == userId && u.Status == (int)TypeEnum.WorkFlowStatus.Applying);
            count = workFlows.Count();
            IEnumerable<WorkFlowOutput> outputs = (from w in workFlows
                                                   where w.InstanceId.Contains(queryInfo) || queryInfo == null
                                                   select new WorkFlowOutput
                                                   {
                                                       Id = w.Id,
                                                       InstanceId = w.InstanceId,
                                                       OrderID = w.OrderID,
                                                       Reason = w.Reason,
                                                       Status = w.Status,
                                                       TypeId = (int)w.TypeId,
                                                       UserId = w.UserId,
                                                       CurrentReviewer = w.CurrentReviewer,
                                                       AddTime = w.AddTime,
                                                       isRead = (bool)w.isRead,
                                                       EnterpriseName = userInfos.Where(u => u.UserId == w.UserId).FirstOrDefault().EnterpriseName,
                                                       Receiver = demandOrderInfos.Where(d => d.Oid == w.OrderID).FirstOrDefault().UserId
                                                   }).OrderBy(o => o.Id).Skip((page - 1) * limit).Take(limit).ToList();
            return outputs;
        }

        public IEnumerable<DemandOrderOutput> GetMyDemandOrders(int page, int limit, out int count, string queryInfo, string userId)
        {
            IQueryable<UserInfo> userInfos = _dbContext.Set<UserInfo>();
            IQueryable<Categorylnfo> categorylnfos = _dbContext.Set<Categorylnfo>();
            IQueryable<DemandOrderInfo> orderInfos = _dbContext.Set<DemandOrderInfo>().Where(d => d.EnterpriseId == userId); // To Find The order which is applied by me
            count = orderInfos.Count();
            IEnumerable<DemandOrderOutput> orderOutputs = (from a in orderInfos
                                                           where a.Oid.Contains(queryInfo) || queryInfo == null
                                                           select new DemandOrderOutput
                                                           {
                                                               Id = a.Id,
                                                               Oid = a.Oid,
                                                               Name = a.Name,
                                                               Num = a.Num,
                                                               Unit = a.Unit,
                                                               Enterpriser = (from u in userInfos where u.UserId == a.EnterpriseId select new { u.UserName }).Select(s => s.UserName).FirstOrDefault(),
                                                               Status = a.Status,
                                                               AddTime = a.AddTime,
                                                               Category = (from g in categorylnfos where g.CategoryId == a.CategoryId select new { g.CategoryName }).Select(s => s.CategoryName).FirstOrDefault(),
                                                               Receiver = (from n in userInfos where n.UserId == a.UserId select new { n.UserName }).Select(s => s.UserName).FirstOrDefault(),
                                                               EnterpriseName = (from n in userInfos where n.UserId == a.EnterpriseId select new { n.EnterpriseName }).Select(s => s.EnterpriseName).FirstOrDefault(),
                                                           }).OrderBy(o => o.Id).Skip((page - 1) * limit).Take(limit).ToList();
            return orderOutputs;
        }

        public IEnumerable<OrderOutput> GetMyFinishOrder(int page, int limit, out int count, string queryInfo, string userId)
        {
            IQueryable<UserInfo> userInfos = _dbContext.Set<UserInfo>();
            IQueryable<Categorylnfo> categorylnfos = _dbContext.Set<Categorylnfo>();
            IQueryable<OrderInfo> orderInfos = _dbContext.Set<OrderInfo>().Where(o =>o.ReceiverId == userId && o.Status == (int)TypeEnum.OrderStatus.Finished && !string.IsNullOrEmpty(o.Url)); // To query the order which has been accpeted
            count = orderInfos.Count();
            IEnumerable<OrderOutput> orderOutputs = (from a in orderInfos
                                                     where a.InstanceId.Contains(queryInfo) || queryInfo == null
                                                     select new OrderOutput
                                                     {
                                                         Id = a.Id,
                                                         InstanceId = a.InstanceId,
                                                         Name = a.Name,
                                                         Num = a.Num,
                                                         Unit = a.Unit,
                                                         EnterpriseID = (from u in userInfos where u.UserId == a.EnterpriseId select new { u.UserName }).Select(s => s.UserName).FirstOrDefault(),
                                                         Status = a.Status,
                                                         AddTime = a.AddTime,
                                                         CategoryName = (from g in categorylnfos where g.CategoryId == a.CategoryId select new { g.CategoryName }).Select(s => s.CategoryName).FirstOrDefault(),
                                                         Receiver = (from n in userInfos where n.UserId == a.ReceiverId select new { n.UserName }).Select(s => s.UserName).FirstOrDefault(),
                                                         EnterpriseName = (from n in userInfos where n.UserId == a.EnterpriseId select new { n.EnterpriseName }).Select(s => s.EnterpriseName).FirstOrDefault(),
                                                         OriginalOrder = a.OriginalOrderId
                                                     }).OrderBy(o => o.Id).Skip((page - 1) * limit).Take(limit).ToList();
            return orderOutputs;
        }

        public IEnumerable<DemandOrderOutput> GetMyRuningOrders(int page, int limit, out int count, string queryInfo, string userId)
        {
            IQueryable<UserInfo> userInfos = _dbContext.Set<UserInfo>();
            IQueryable<Categorylnfo> categorylnfos = _dbContext.Set<Categorylnfo>();
            IQueryable<DemandOrderInfo> orderInfos = _dbContext.Set<DemandOrderInfo>().Where(d => d.UserId == userId && d.DelFlag == false && d.Status == (int)TypeEnum.DemendOrderStatus.Accepted); // To Find The order which is applied by me and is runing
            count = orderInfos.Count();
            IEnumerable<DemandOrderOutput> orderOutputs = (from a in orderInfos
                                                           where a.Oid.Contains(queryInfo) || queryInfo == null
                                                           select new DemandOrderOutput
                                                           {
                                                               Id = a.Id,
                                                               Oid = a.Oid,
                                                               Name = a.Name,
                                                               Num = a.Num,
                                                               Unit = a.Unit,
                                                               Enterpriser = (from u in userInfos where u.UserId == a.EnterpriseId select new { u.UserName }).Select(s => s.UserName).FirstOrDefault(),
                                                               Status = a.Status,
                                                               AddTime = a.AddTime,
                                                               Category = (from g in categorylnfos where g.CategoryId == a.CategoryId select new { g.CategoryName }).Select(s => s.CategoryName).FirstOrDefault(),
                                                               Receiver = (from n in userInfos where n.UserId == a.UserId select new { n.UserName }).Select(s => s.UserName).FirstOrDefault(),
                                                               EnterpriseName = (from n in userInfos where n.UserId == a.EnterpriseId select new { n.EnterpriseName }).Select(s => s.EnterpriseName).FirstOrDefault(),
                                                           }).OrderBy(o => o.Id).Skip((page - 1) * limit).Take(limit).ToList();
            return orderOutputs;
        }

        public IEnumerable<DemandOrderOutput> GetMyRunningDemandOrders(int page, int limit, out int count, string queryInfo, string userId)
        {
            IQueryable<UserInfo> userInfos = _dbContext.Set<UserInfo>();
            IQueryable<Categorylnfo> categorylnfos = _dbContext.Set<Categorylnfo>();
            IQueryable<DemandOrderInfo> orderInfos = _dbContext.Set<DemandOrderInfo>().Where(d => d.EnterpriseId == userId && d.DelFlag == false && d.Status == (int)TypeEnum.DemendOrderStatus.Accepted); // To Find The order which is applied by me and is runing
            count = orderInfos.Count();
            IEnumerable<DemandOrderOutput> orderOutputs = (from a in orderInfos
                                                           where a.Oid.Contains(queryInfo) || queryInfo == null
                                                           select new DemandOrderOutput
                                                           {
                                                               Id = a.Id,
                                                               Oid = a.Oid,
                                                               Name = a.Name,
                                                               Num = a.Num,
                                                               Unit = a.Unit,
                                                               Enterpriser = (from u in userInfos where u.UserId == a.EnterpriseId select new { u.UserName }).Select(s => s.UserName).FirstOrDefault(),
                                                               Status = a.Status,
                                                               AddTime = a.AddTime,
                                                               Category = (from g in categorylnfos where g.CategoryId == a.CategoryId select new { g.CategoryName }).Select(s => s.CategoryName).FirstOrDefault(),
                                                               Receiver = (from n in userInfos where n.UserId == a.UserId select new { n.UserName }).Select(s => s.UserName).FirstOrDefault(),
                                                               EnterpriseName = (from n in userInfos where n.UserId == a.EnterpriseId select new { n.EnterpriseName }).Select(s => s.EnterpriseName).FirstOrDefault(),
                                                           }).OrderBy(o => o.Id).Skip((page - 1) * limit).Take(limit).ToList();
            return orderOutputs;
        }

        public OrderOutput GetMyRunningOrderInfo(string oid)
        {
            IQueryable<UserInfo> userInfos = _dbContext.Set<UserInfo>();
            IQueryable<Categorylnfo> categorylnfos = _dbContext.Set<Categorylnfo>();
            IQueryable<OrderInfo> orderInfos = _dbContext.Set<OrderInfo>(); // To query the order which has been accpeted

            OrderOutput orderOutput = (from a in orderInfos
                                       where a.OriginalOrderId == oid
                                       select new OrderOutput
                                       {
                                           Id = a.Id,
                                           InstanceId = a.InstanceId,
                                           Name = a.Name,
                                           Num = a.Num,
                                           Unit = a.Unit,
                                           EnterpriseID = (from u in userInfos where u.UserId == a.EnterpriseId select new { u.UserName }).Select(s => s.UserName).FirstOrDefault(),
                                           Status = a.Status,
                                           AddTime = a.AddTime,
                                           CategoryName = (from g in categorylnfos where g.CategoryId == a.CategoryId select new { g.CategoryName }).Select(s => s.CategoryName).FirstOrDefault(),
                                           Receiver = (from n in userInfos where n.UserId == a.ReceiverId select new { n.UserName }).Select(s => s.UserName).FirstOrDefault(),
                                           EnterpriseName = (from n in userInfos where n.UserId == a.EnterpriseId select new { n.EnterpriseName }).Select(s => s.EnterpriseName).FirstOrDefault(),
                                           OriginalOrder = a.OriginalOrderId,
                                           CategoryId = a.CategoryId,
                                           FinishedTime = a.FinishedTime,
                                           Url = a.Url
                                       }).FirstOrDefault();
            return orderOutput;
        }

        public IEnumerable<OrderOutput> GetMyWaittingConfirmOrder(int page, int limit, out int count, string queryInfo, string userId)
        {
            IQueryable<UserInfo> userInfos = _dbContext.Set<UserInfo>();
            IQueryable<Categorylnfo> categorylnfos = _dbContext.Set<Categorylnfo>();
            IQueryable<OrderInfo> orderInfos = _dbContext.Set<OrderInfo>().Where(o =>o.OriginalOrderId==userId && o.Status == (int)TypeEnum.OrderStatus.Running && !string.IsNullOrEmpty(o.Url)); // To query the order which has been accpeted
            count = orderInfos.Count();
            IEnumerable<OrderOutput> orderOutputs = (from a in orderInfos
                                                     where a.InstanceId.Contains(queryInfo) || queryInfo == null
                                                     select new OrderOutput
                                                     {
                                                         Id = a.Id,
                                                         InstanceId = a.InstanceId,
                                                         Name = a.Name,
                                                         Num = a.Num,
                                                         Unit = a.Unit,
                                                         EnterpriseID = (from u in userInfos where u.UserId == a.EnterpriseId select new { u.UserName }).Select(s => s.UserName).FirstOrDefault(),
                                                         Status = a.Status,
                                                         AddTime = a.AddTime,
                                                         CategoryName = (from g in categorylnfos where g.CategoryId == a.CategoryId select new { g.CategoryName }).Select(s => s.CategoryName).FirstOrDefault(),
                                                         Receiver = (from n in userInfos where n.UserId == a.ReceiverId select new { n.UserName }).Select(s => s.UserName).FirstOrDefault(),
                                                         EnterpriseName = (from n in userInfos where n.UserId == a.EnterpriseId select new { n.EnterpriseName }).Select(s => s.EnterpriseName).FirstOrDefault(),
                                                         OriginalOrder = a.OriginalOrderId
                                                     }).OrderBy(o => o.Id).Skip((page - 1) * limit).Take(limit).ToList();
            return orderOutputs;
        }

        public DemandOrderOutput GetOrderByOID(string id)
        {
            IQueryable<UserInfo> userInfos = _dbContext.Set<UserInfo>();
            IQueryable<Categorylnfo> categorylnfos = _dbContext.Set<Categorylnfo>();
            DemandOrderInfo info = _dbContext.Set<DemandOrderInfo>().Where(d => d.Oid == id).FirstOrDefault();
            if (info != null)
            {
                DemandOrderOutput output = new DemandOrderOutput
                {
                    Id = info.Id,
                    Oid = info.Oid,
                    Name = info.Name,
                    Num = info.Num,
                    Unit = info.Unit,
                    Enterpriser = (from u in userInfos where u.UserId == info.EnterpriseId select new { u.UserName }).Select(s => s.UserName).FirstOrDefault(),
                    Status = info.Status,
                    AddTime = info.AddTime,
                    Category = (from g in categorylnfos where g.CategoryId == info.CategoryId select new { g.CategoryName }).Select(s => s.CategoryName).FirstOrDefault(),
                    CategoryId = info.CategoryId,
                    Receiver = (from n in userInfos where n.UserId == info.UserId select new { n.UserName }).Select(s => s.UserName).FirstOrDefault(),
                    EnterpriseName = (from e in userInfos where e.UserId == info.EnterpriseId select new { e.EnterpriseName }).Select(s => s.EnterpriseName).FirstOrDefault()
                };
                return output;
            }
            return null;
        }

        /// <summary>
        /// Get UnAceeptOrder List
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="count"></param>
        /// <param name="queryInfo"></param>
        /// <returns></returns>
        public IEnumerable<DemandOrderOutput> GetOrders(int page, int limit, out int count, string queryInfo)
        {
            IQueryable<UserInfo> userInfos = _dbContext.Set<UserInfo>();
            IQueryable<Categorylnfo> categorylnfos = _dbContext.Set<Categorylnfo>();
            IQueryable<DemandOrderInfo> orderInfos = _dbContext.Set<DemandOrderInfo>().Where(o => o.Status == (int)TypeEnum.DemendOrderStatus.unAccept && o.Status != (int)TypeEnum.DemendOrderStatus.Canceled); //未接受的订单 （没有撤销的订单）
            count = orderInfos.Count();
            IEnumerable<DemandOrderOutput> orderOutputs = (from a in orderInfos
                                                           where a.Oid.Contains(queryInfo) || queryInfo == null
                                                           select new DemandOrderOutput
                                                           {
                                                               Id = a.Id,
                                                               Oid = a.Oid,
                                                               Name = a.Name,
                                                               Num = a.Num,
                                                               Unit = a.Unit,
                                                               Enterpriser = (from u in userInfos where u.UserId == a.EnterpriseId select new { u.UserName }).Select(s => s.UserName).FirstOrDefault(),
                                                               Status = a.Status,
                                                               AddTime = a.AddTime,
                                                               Category = (from g in categorylnfos where g.CategoryId == a.CategoryId select new { g.CategoryName }).Select(s => s.CategoryName).FirstOrDefault(),
                                                               EnterpriseName = (from n in userInfos where n.UserId == a.EnterpriseId select new { n.EnterpriseName }).Select(s => s.EnterpriseName).FirstOrDefault(),
                                                           }).OrderBy(o => o.Id).Skip((page - 1) * limit).Take(limit).ToList();
            return orderOutputs;
        }

        public IEnumerable<DemandOrderOutput> GetRuningOrder(int page, int limit, out int count, string queryInfo)
        {
            IQueryable<UserInfo> userInfos = _dbContext.Set<UserInfo>();
            IQueryable<Categorylnfo> categorylnfos = _dbContext.Set<Categorylnfo>();
            IQueryable<DemandOrderInfo> orderInfos = _dbContext.Set<DemandOrderInfo>().Where(o => o.Status == (int)TypeEnum.DemendOrderStatus.Accepted); // To query the order which has been accpeted
            count = orderInfos.Count();
            IEnumerable<DemandOrderOutput> orderOutputs = (from a in orderInfos
                                                           where a.Oid.Contains(queryInfo) || queryInfo == null
                                                           select new DemandOrderOutput
                                                           {
                                                               Id = a.Id,
                                                               Oid = a.Oid,
                                                               Name = a.Name,
                                                               Num = a.Num,
                                                               Unit = a.Unit,
                                                               Enterpriser = (from u in userInfos where u.UserId == a.EnterpriseId select new { u.UserName }).Select(s => s.UserName).FirstOrDefault(),
                                                               Status = a.Status,
                                                               AddTime = a.AddTime,
                                                               Category = (from g in categorylnfos where g.CategoryId == a.CategoryId select new { g.CategoryName }).Select(s => s.CategoryName).FirstOrDefault(),
                                                               Receiver = (from n in userInfos where n.UserId == a.UserId select new { n.UserName }).Select(s => s.UserName).FirstOrDefault(),
                                                               EnterpriseName = (from n in userInfos where n.UserId == a.EnterpriseId select new { n.EnterpriseName }).Select(s => s.EnterpriseName).FirstOrDefault(),
                                                           }).OrderBy(o => o.Id).Skip((page - 1) * limit).Take(limit).ToList();
            return orderOutputs;
        }

        public IEnumerable<OrderOutput> GetUnVerifyOrderList(int page, int limit, out int count, string queryInfo)
        {
            IQueryable<UserInfo> userInfos = _dbContext.Set<UserInfo>();
            IQueryable<Categorylnfo> categorylnfos = _dbContext.Set<Categorylnfo>();
            IQueryable<OrderInfo> orderInfos = _dbContext.Set<OrderInfo>().Where(o => o.Status == (int)TypeEnum.OrderStatus.Running && !string.IsNullOrEmpty(o.Url)); // To query the order which has been accpeted
            count = orderInfos.Count();
            IEnumerable<OrderOutput> orderOutputs = (from a in orderInfos
                                                     where a.InstanceId.Contains(queryInfo) || queryInfo == null
                                                     select new OrderOutput
                                                     {
                                                         Id = a.Id,
                                                         InstanceId = a.InstanceId,
                                                         Name = a.Name,
                                                         Num = a.Num,
                                                         Unit = a.Unit,
                                                         EnterpriseID = (from u in userInfos where u.UserId == a.EnterpriseId select new { u.UserName }).Select(s => s.UserName).FirstOrDefault(),
                                                         Status = a.Status,
                                                         AddTime = a.AddTime,
                                                         CategoryName = (from g in categorylnfos where g.CategoryId == a.CategoryId select new { g.CategoryName }).Select(s => s.CategoryName).FirstOrDefault(),
                                                         Receiver = (from n in userInfos where n.UserId == a.ReceiverId select new { n.UserName }).Select(s => s.UserName).FirstOrDefault(),
                                                         EnterpriseName = (from n in userInfos where n.UserId == a.EnterpriseId select new { n.EnterpriseName }).Select(s => s.EnterpriseName).FirstOrDefault(),
                                                         OriginalOrder = a.OriginalOrderId
                                                     }).OrderBy(o => o.Id).Skip((page - 1) * limit).Take(limit).ToList();
            return orderOutputs;
        }

        public bool ReleaseOrder(DemandOrderInput demandOrderInput, out string msg)
        {
            DemandOrderInfo demandOrder = new DemandOrderInfo
            {
                Oid = "EO" + DateTime.Now.ToString("yyyyMMddHHmmssffff"),
                EnterpriseId = demandOrderInput.EnterpriseId,
                Name = demandOrderInput.Name,
                Num = demandOrderInput.Num,
                Unit = demandOrderInput.Unit,
                CategoryId = demandOrderInput.CategoryId,
                Status = (int)TypeEnum.DemendOrderStatus.unAccept,
                DelFlag = false,
                AddTime = DateTime.Now
            };
            _dbContext.Set<DemandOrderInfo>().Add(demandOrder);
            if (_dbContext.SaveChanges() > 0)
            {
                msg = "添加成功！";
                return true;
            }
            msg = "错误！内部出现异常！";
            return false;
        }

        public OrderOutput ViewMyOrder(string oid)
        {
            IQueryable<UserInfo> userInfos = _dbContext.Set<UserInfo>();
            IQueryable<Categorylnfo> categorylnfos = _dbContext.Set<Categorylnfo>();
            IQueryable<OrderInfo> orderInfos = _dbContext.Set<OrderInfo>(); // To query the order which has been accpeted

            OrderOutput orderOutput = (from a in orderInfos
                                       where a.OriginalOrderId == oid
                                       select new OrderOutput
                                       {
                                           Id = a.Id,
                                           InstanceId = a.InstanceId,
                                           Name = a.Name,
                                           Num = a.Num,
                                           Unit = a.Unit,
                                           EnterpriseID = (from u in userInfos where u.UserId == a.EnterpriseId select new { u.UserName }).Select(s => s.UserName).FirstOrDefault(),
                                           Status = a.Status,
                                           AddTime = a.AddTime,
                                           CategoryName = (from g in categorylnfos where g.CategoryId == a.CategoryId select new { g.CategoryName }).Select(s => s.CategoryName).FirstOrDefault(),
                                           Receiver = (from n in userInfos where n.UserId == a.ReceiverId select new { n.UserName }).Select(s => s.UserName).FirstOrDefault(),
                                           EnterpriseName = (from n in userInfos where n.UserId == a.EnterpriseId select new { n.EnterpriseName }).Select(s => s.EnterpriseName).FirstOrDefault(),
                                           OriginalOrder = a.OriginalOrderId,
                                           CategoryId = a.CategoryId,
                                           FinishedTime = a.FinishedTime,
                                           Url = a.Url
                                       }).FirstOrDefault();
            return orderOutput;
        }

        public DemandOrderOutput ViewSpecialApplyingOrder(string oid)
        {
            IQueryable<UserInfo> userInfos = _dbContext.Set<UserInfo>();
            IQueryable<Categorylnfo> categorylnfos = _dbContext.Set<Categorylnfo>();
            IQueryable<WorkFlow> workFlows = _dbContext.Set<WorkFlow>();
            DemandOrderInfo info = _dbContext.Set<DemandOrderInfo>().Where(d => d.Oid == oid).FirstOrDefault();
            if (info != null)
            {
                WorkFlow workFlow = workFlows.Where(w => w.OrderID == oid).FirstOrDefault();
                DemandOrderOutput output = new DemandOrderOutput
                {
                    Id = info.Id,
                    Oid = info.Oid,
                    Name = info.Name,
                    Num = info.Num,
                    Unit = info.Unit,
                    Enterpriser = (from u in userInfos where u.UserId == info.EnterpriseId select new { u.UserName }).Select(s => s.UserName).FirstOrDefault(),
                    Status = info.Status,
                    AddTime = info.AddTime,
                    Category = (from g in categorylnfos where g.CategoryId == info.CategoryId select new { g.CategoryName }).Select(s => s.CategoryName).FirstOrDefault(),
                    CategoryId = info.CategoryId,
                    Receiver = (from n in userInfos where n.UserId == info.UserId select new { n.UserName }).Select(s => s.UserName).FirstOrDefault(),
                    EnterpriseName = (from e in userInfos where e.UserId == info.EnterpriseId select new { e.EnterpriseName }).Select(s => s.EnterpriseName).FirstOrDefault(),
                    Reason = workFlow.Reason
                };
                workFlow.isRead = true;
                _dbContext.SaveChanges();

                return output;
            }
            return null;
        }

        public bool WithdrewMyApplication(string oid, out string msg)
        {
            IQueryable<DemandOrderInfo> demandOrders = _dbContext.Set<DemandOrderInfo>();
            DemandOrderInfo orderInfo = demandOrders.Where(d => d.Oid == oid).FirstOrDefault();
            if (orderInfo != null)
            {
                if (orderInfo.Status != (int)TypeEnum.DemendOrderStatus.unAccept)
                {
                    msg = "只有未被接受的订单才可以撤销！进行中的订单如有特殊变故可申请特殊撤销！已完成的订单不可撤销！";
                    return false;
                }
                try
                {
                    orderInfo.Status = (int)TypeEnum.DemendOrderStatus.Canceled;
                    if (_dbContext.SaveChanges() > 0)
                    {
                        msg = "已撤销！";
                        return true;
                    }
                    msg = "错误！内部异常！";
                    return false;
                }
                catch (Exception ex)
                {
                    msg = ex.Message;
                    return false;
                }
            }
            msg = "订单不存在！数据可能被篡改！";
            return true;
        }

        public bool WithdrewMyApplicationBySpecial(DemandOrderInput demandOrderInput, out string msg)
        {
            DemandOrderInfo demandOrder = _dbContext.Set<DemandOrderInfo>().Where(d => d.Oid == demandOrderInput.Oid).FirstOrDefault();
            if (demandOrder != null)
            {
                try
                {
                    //获取到部门主管的Id
                    string manager = _dbContext.Set<DepartmentInfo>().Where(d => d.DepartmentId == "D1003").Select(S => S.LeaderId).FirstOrDefault();
                    string date = "W" + DateTime.Now.ToString("yyyyMMddHHmmssffff");
                    WorkFlow workFlow = new WorkFlow
                    {
                        InstanceId = date,
                        OrderID = demandOrder.Oid,
                        CurrentReviewer = manager,
                        TypeId = (int)TypeEnum.WorkFlowType.SpecialWithdrew,
                        UserId = demandOrderInput.UserId,
                        Status = (int)TypeEnum.WorkFlowStatus.Applying,
                        AddTime = DateTime.Now,
                        Reason = demandOrderInput.Reason,
                        isRead = false
                    };
                    _dbContext.Set<WorkFlow>().Add(workFlow);
                    demandOrder.Status = (int)TypeEnum.DemendOrderStatus.ApplyingCancel;
                    //两重审核(本项目不需要二重，所以写在这里，即是一重)
                    WorkFlowStep workFlowStep = new WorkFlowStep
                    {
                        InstanceId = date,
                        TypeId = (int)TypeEnum.WorkFlowType.SpecialWithdrew,
                        isRead = false,
                        ReviewerId = manager,
                        ReviewStatus = (int)TypeEnum.ReviewStatus.UnView,
                        NextReviewer = "无"
                    };
                    _dbContext.Set<WorkFlowStep>().Add(workFlowStep);
                    if (_dbContext.SaveChanges() > 0)
                    {
                        msg = "申请成功！";
                        return true;
                    }
                    msg = "错误！内部异常！";
                    return false;
                }
                catch (Exception ex)
                {
                    msg = ex.Message;
                    return false;
                }
            }
            msg = "订单不存在！数据可能被篡改！";
            return true;
        }
    }
}
