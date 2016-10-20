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
    [Permission(EnumBusinessPermission.StoreManage_Outbound)]
    public class OutboundController:AdminControllerBase
    {
        public ActionResult Index(OutBoundRequest request)
        {
            var obrlist = this.StoreService.GetOutBound(request);
            return View(obrlist);
        }
    }
}