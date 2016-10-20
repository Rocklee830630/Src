using GMS.Account.Contract;
using GMS.Web.Admin.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GMS.Framework.Utility;
using GMS.Store.Contract;
 

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
    }
}