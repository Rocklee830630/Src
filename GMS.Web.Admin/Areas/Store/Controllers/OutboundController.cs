using GMS.Account.Contract;
using GMS.Web.Admin.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GMS.Framework.Utility;
using GMS.Store.Contract;
using System.IO;
using System.Data;

namespace GMS.Web.Admin.Areas.Store.Controllers
{
    [Permission(EnumBusinessPermission.StoreManage_Outbound)]
    public class OutboundController:AdminControllerBase
    {
        public ActionResult Index(OutBoundRequest request)
        {
            if (request.export == "export")//导出EXCEL
            {
                request.PageSize = 10000000;
                var ibrAll = this.StoreService.GetOutBound(request);
                DataTable dt = new DataTable();
                dt.Columns.Add("客户名称");
                dt.Columns.Add("订单类别");
                dt.Columns.Add("出库数量");
                dt.Columns.Add("领用单位");
                dt.Columns.Add("出库时间"); 

                foreach (OutBoundRecord inBoundItem in ibrAll)
                {
                    DataRow dr = dt.NewRow();
                    dr["客户名称"] = inBoundItem.DictionaryTree.parent_name;
                    dr["订单类别"] = inBoundItem.DictionaryTree.name;
                    dr["出库数量"] = inBoundItem.number;
                    dr["领用单位"] = inBoundItem.employment;
                    dr["出库时间"] = inBoundItem.CreateTime; 
                    dt.Rows.Add(dr);
                }
                byte[] excelByte = this.StoreService.DataTable2Excel(dt, "出入库记录");
                using (MemoryStream ms = new MemoryStream())
                {
                    return File(excelByte, "application/ms-excel", "出库记录.xls");
                }
            }

            var obrlist = this.StoreService.GetOutBound(request);
            return View(obrlist);
        } 
    }
}