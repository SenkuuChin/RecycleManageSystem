using RecycleSystem.Data.Data.OrderManageDTO;
using RecycleSystem.Data.Data.WareHouseDTO;
using RecycleSystem.Data.Data.WorkFlowDTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecycleSystem.IService
{
    public interface IOrderManageService
    {
        IEnumerable<DemandOrderOutput> GetOrders(int page, int limit, out int count, string queryInfo);
        IEnumerable<DemandOrderOutput> GetRuningOrder(int page, int limit, out int count, string queryInfo);
        IEnumerable<DemandOrderOutput> GetFinishedOrder(int page, int limit, out int count, string queryInfo);
        IEnumerable<DemandOrderOutput> GetAllOrders(int page, int limit, out int count, string queryInfo);
        DemandOrderOutput GetOrderByOID(string id);
        bool AcceptOrder(string oid, string userId, out string message);
        IEnumerable<OrderOutput> GetUnVerifyOrderList(int page, int limit, out int count, string queryInfo);
        IEnumerable<DemandOrderOutput> GetMyDemandOrders(int page, int limit, out int count, string queryInfo, string userId);
        bool ReleaseOrder(DemandOrderInput demandOrderInput, out string msg);
        bool WithdrewMyApplication(string oid, out string msg);
        IEnumerable<DemandOrderOutput> GetMyRunningDemandOrders(int page, int limit, out int count, string queryInfo, string userId);
        bool WithdrewMyApplicationBySpecial(DemandOrderInput demandOrderInput,out string msg);
        IEnumerable<WorkFlowOutput> GetFlowOutputs(int page, int limit, out int count, string queryInfo,string userId);
        DemandOrderOutput ViewSpecialApplyingOrder(string oid);
        bool ApproveSpecialOrderWithdrew(DemandOrderInput demandOrderInput,out string msg);
        OrderOutput GetFinishedOrderInfo(int id);
        bool confirmFinish(OrderInput order,out string msg);
        IEnumerable<DemandOrderOutput> GetMyRuningOrders(int page, int limit, out int count, string queryInfo, string userId);
        OrderOutput GetMyRunningOrderInfo(string oid);
        /// <summary>
        /// 完成订单（上传图片）
        /// </summary>
        /// <returns></returns>
        bool FinishOrder(string oid,string pathName,string userId,out string msg);
        IEnumerable<OrderOutput> GetMyWaittingConfirmOrder(int page, int limit, out int count, string queryInfo, string userId);
        IEnumerable<OrderOutput> GetMyFinishOrder(int page, int limit, out int count, string queryInfo, string userId);
        OrderOutput ViewMyOrder(string oid);
    }
}
