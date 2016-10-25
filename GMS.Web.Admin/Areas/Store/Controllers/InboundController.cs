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
    [Permission(EnumBusinessPermission.StoreManage_Inbound)]
    public class InboundController:AdminControllerBase
    {

        public ActionResult Index(InBoundRequest request)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "所有类型", Value = "0", Selected = true });
            items.Add(new SelectListItem { Text = "入库", Value = "入库" });
            items.Add(new SelectListItem { Text = "还库材料", Value = "还库材料" });
            items.Add(new SelectListItem { Text = "出库材料", Value = "出库材料" });
            this.ViewData["czlx"] = items;

            this.ViewBag.OperateType = new SelectListItem() { };
            var ibr = this.StoreService.GetInBound(request);

            if(request.export=="export")//导出EXCEL
            {
                request.PageSize = 10000000;
                var ibrAll = this.StoreService.GetInBound(request);
                DataTable dt = new DataTable();
                dt.Columns.Add("客户名称");                
                dt.Columns.Add("订单类别");
                dt.Columns.Add("材料名称");
                dt.Columns.Add("品名");
                dt.Columns.Add("门幅");
                dt.Columns.Add("基数/米");
                dt.Columns.Add("数量");
                dt.Columns.Add("领用人");
                dt.Columns.Add("操作时间");
                dt.Columns.Add("操作类型"); 
                
                foreach(InBoundRecord inBoundItem in ibrAll)
                {
                    DataRow dr = dt.NewRow();
                    dr["客户名称"] = inBoundItem.khmc;
                    dr["订单类别"] = inBoundItem.DictionaryProperty.DictionaryTree.name;
                    dr["材料名称"] = inBoundItem.DictionaryProperty.clmc;
                    dr["品名"] = inBoundItem.DictionaryProperty.pm;
                    dr["门幅"] = inBoundItem.DictionaryProperty.mf;
                    dr["基数/米"] = inBoundItem.DictionaryProperty.js;
                    dr["数量"] = inBoundItem.number;
                    dr["领用人"] = inBoundItem.employment;
                    dr["操作时间"] = inBoundItem.CreateTime; 
                    dr["操作类型"] = inBoundItem.boundtype;
                    dt.Rows.Add(dr);
                }
                byte[] excelByte = this.StoreService.DataTable2Excel(dt, "出入库记录");  
                using (MemoryStream ms = new MemoryStream())
                { 
                    return File(excelByte, "application/ms-excel", "出入库记录.xls");
                }
            }
            return View(ibr);
        } 
    }
}