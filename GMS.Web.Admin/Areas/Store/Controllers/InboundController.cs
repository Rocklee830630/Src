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
            return View(ibr);
        }

        [HttpPost]
        public ActionResult ExportToExcel()
        {
            NPOI.SS.UserModel.IWorkbook workbook = new NPOI.XSSF.UserModel.XSSFWorkbook();
            NPOI.SS.UserModel.ICell cell;
            NPOI.SS.UserModel.ISheet sheet = workbook.CreateSheet("出入库记录");
            int i = 0;
            int rowLimit = 100;
            DateTime originalTime = DateTime.Now;
            for (i = 0; i < rowLimit; i++)
            {
                cell = sheet.CreateRow(i).CreateCell(0);
                cell.SetCellValue("值" + i.ToString());
            }

            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                var buffer = ms.GetBuffer();
                ms.Close();
                return File(buffer, "application/ms-excel", "test.xlsx");
            }
        }
    }
}