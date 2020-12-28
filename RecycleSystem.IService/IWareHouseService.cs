using RecycleSystem.Data.Data.WareHouseDTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecycleSystem.IService
{
    public interface IWareHouseService
    {
        IEnumerable<GoodsOutput> GetGoodsInputInfo(int page, int limit, out int count, string queryInfo);
        IEnumerable<WareHouseOutput> GetGoodsOutputs(int page, int limit, out int count, string queryInfo);
        bool UploadMaterialaInfo(IEnumerable<GoodsInput> goodsInputs, string userId, string msg);
    }
}
