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
    [Permission(EnumBusinessPermission.StoreManage)]
    public class BoundManageController: AdminControllerBase
    {
        public ActionResult Index(StoreTableRequest request)
        {
            var OrderList = this.StoreService.GetOrderList(new OrderRequest());
            this.ViewBag.OrederType = new SelectList(OrderList, "dt_id", "name");
            
            var result = this.StoreService.GetStoreList(request);
            //foreach(StoreTable st in result)
            //{
            //    DictionaryTree dt = new DictionaryTree();
            //    dt.dt_id = st.DictionaryProperty.leaf_id.Value;
            //    dt.name = this.StoreService.GetParentNameByLeafID(dt.dt_id);
            //    st.DictionaryProperty.DictionaryTree = dt;
            //} 
            return View(result);
        }

        public ActionResult GetTree()
        {
            var list = this.StoreService.GetDictionaryTreeList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 初始化入库edit框
        /// </summary>
        /// <returns></returns>
        public ActionResult InBoundEdit()
        {
            var OrderList = this.StoreService.GetOrderList(new OrderRequest());
            this.ViewBag.OrderType = new SelectList(OrderList, "dt_id", "name");
            List<string> clmcs = new List<string>(); 
            clmcs.Add("请选择材料..."); 
            this.ViewBag.Materialnames = new SelectList(clmcs); 

            var model = new StoreTable();
            return View("InBoundEdit", model);
        }
           
        /// <summary>
        /// 初始化出库edit框
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            var OrderList = this.StoreService.GetOrderList(new OrderRequest());
            this.ViewBag.OrderType = new SelectList(OrderList, "dt_id", "name");
            var model = new StoreTable();
            return View("OutBoundEdit", model);
        }

        /// <summary>
        /// 出库提交保存
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            if (collection.AllKeys.Contains("DictionaryProperty.dpid"))
            { 
                StoreTableRequest request = new StoreTableRequest();
                request.PageSize = 1000;
                request.Gid = new Guid(collection["DictionaryProperty.dpid"]);
                int OutProductNumber;
                if(!int.TryParse(collection["OutBoundCount"],out OutProductNumber))
                {
                    var model = new StoreTable();
                    return View("OutBoundEdit",model);
                }
                //OutProductNumber = int.Parse(collection["OutBoundCount"]); //出库订单件数
                var result = this.StoreService.GetStoreList(request);
                Guid tempid =new Guid();
                foreach (StoreTable st in result)
                {
                    if(st.DictionaryProperty.leaf_id == request.Gid)
                    { 
                        st.number = st.number - (st.DictionaryProperty.js.Value * Convert.ToDecimal(OutProductNumber));
                        InBoundRecord ibr = new InBoundRecord();
                        //插入数据到入库记录表 
                        ibr.inbound_id = st.store_item_id;
                        ibr.rkid = Guid.NewGuid();
                        ibr.number = st.number;
                        ibr.boundtype = "出库材料";
                        ibr.khmc = this.StoreService.GetParentNameByLeafID(st.DictionaryProperty.leaf_id.Value);
                        this.StoreService.InsertInboundRecord(ibr);

                        tempid = st.DictionaryProperty.leaf_id.Value;
                        //出库
                        this.StoreService.OutBoundItem(st);
                    }
                }
                //插入到出库记录表
                OutBoundRecord obr = new OutBoundRecord();
                obr.ckid = Guid.NewGuid();
                obr.node_id = tempid;
                obr.number = OutProductNumber;
                this.StoreService.InsertOutBoundRecord(obr);
            }
            return this.RefreshParent();
        }

        /// <summary>
        /// 环库EDIT初始化
        /// </summary>
        /// <returns></returns>
        public ActionResult ReturnOrder()
        {
            var OrderList = this.StoreService.GetOrderList(new OrderRequest());
            this.ViewBag.OrderType = new SelectList(OrderList, "dt_id", "name");
            var model = new StoreTable();
            return View("ReturnOrderEdit", model);
        }

        /// <summary>
        /// 还库提交保存
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ReturnOrder(FormCollection collection)
        {
            if (collection.AllKeys.Contains("DictionaryProperty.dpid"))
            {
                StoreTableRequest request = new StoreTableRequest();
                request.PageSize = 1000;
                request.Gid = new Guid(collection["DictionaryProperty.dpid"]);
                int OutProductNumber = int.Parse(collection["OutBoundCount"]); //出库订单件数
                var result = this.StoreService.GetStoreList(request);

                //获取订单下所有材料
                StoreTreeNameRequests stnRequest = new StoreTreeNameRequests();
                stnRequest.PageSize = 1000;
                stnRequest.treeID = request.Gid;
                var clResultList = this.StoreService.GetNodeProperty(stnRequest);
                //循环材料列表，如果在StoreTable中存在更新其数量，如果不在插入数据
                foreach(DictionaryProperty dp in clResultList)
                {
                    StoreTable st = new StoreTable();
                    st.number = 0;
                    st.number = st.number + (dp.js.Value * Convert.ToDecimal(OutProductNumber));
                    st.store_item_id = dp.dpid;
                    
                     
                    InBoundRecord ibr = new InBoundRecord();
                        //插入数据到入库记录表 
                        ibr.inbound_id = st.store_item_id;
                        ibr.rkid = Guid.NewGuid();
                        ibr.number = st.number;
                        ibr.boundtype = "还库材料";
                        ibr.khmc = this.StoreService.GetParentNameByLeafID(dp.leaf_id.Value);
                        this.StoreService.InsertInboundRecord(ibr);
                    this.StoreService.InsertStoreItem(st);
                }
                } 
            return this.RefreshParent();
        }

        /// <summary>
        /// 初始化修改库房数量
        /// </summary>
        /// <returns></returns>
        public ActionResult StoreEdit(string id)
        { 
            StoreTable st = this.StoreService.GetStoreItem(new Guid(id));
            DictionaryProperty dp = this.StoreService.GetDictionaryProperty(st.store_item_id.Value);
            string leafname = this.StoreService.GetTreeName(dp.leaf_id.Value);
            string parentname = this.StoreService.GetParentNameByLeafID(dp.leaf_id.Value);
            StoreWindowModel swm = new StoreWindowModel();
            swm.clmc = dp.clmc;
            swm.ddlx = leafname;
            swm.khmc = parentname;
            swm.pm = dp.pm;
            swm.mf = dp.mf;
            swm.js = dp.js.ToString();
            swm.number = st.number.Value.ToString();

            return View("StoreEdit", swm);
        }

        /// <summary>
        /// 提交修改库存信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult StoreEdit(string id,FormCollection collection)
        {
            StoreTable st = this.StoreService.GetStoreItem(new Guid(id));
            st.number = decimal.Parse(collection["number"]);
            InBoundRecord ibr = new InBoundRecord();
            //插入数据到入库记录表 
            ibr.inbound_id = st.store_item_id;
            ibr.rkid = Guid.NewGuid();
            ibr.number = st.number;
            ibr.boundtype = "库存修改";
            ibr.khmc = this.StoreService.GetParentNameByLeafID(st.DictionaryProperty.leaf_id.Value);
            this.StoreService.InsertInboundRecord(ibr);

            this.StoreService.OutBoundItem(st);
            return this.RefreshParent();
        }

        /// <summary>
        /// 提交入库action
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult InBoundEdit(FormCollection collection)
        { 
            var model = new StoreTable();
            //this.TryUpdateModel<StoreTable>(model);
            InBoundRecord ibr = new InBoundRecord(); 
            if (collection.AllKeys.Contains("DictionaryProperty.dpid"))
            {
                Guid proid = new Guid(collection["clID"]);
                int temp = 0;
                if(!int.TryParse(collection["InBoundCount"],out temp))
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                    return PartialView();
                }
                
                model.number = int.Parse(collection["InBoundCount"]);
                 
                DictionaryProperty dp = this.StoreService.GetDicProperty(proid, collection["Materialnames"]);
                model.store_item_id = dp.dpid;
                
                //插入数据到入库记录表 
                ibr.inbound_id = dp.dpid;
                ibr.rkid = Guid.NewGuid();
                ibr.number= model.number;
                ibr.boundtype = "入库";
                ibr.khmc = this.StoreService.GetParentNameByLeafID(dp.leaf_id.Value);
                this.StoreService.InsertInboundRecord(ibr);
                ///入库
                this.StoreService.InsertStoreItem(model);
            } 
            
            return this.RefreshParent();
        }

        [HttpPost]
        public JsonResult GetInBoundData(FormCollection collection)
        {
            string tempID = collection[0].ToString();
            var OrderList = this.StoreService.GetOrderList(new OrderRequest());
            IboundProperty ibp =new IboundProperty();
            ibp.dpid = new List<string>();
            ibp.clmc = new List<string>();
            ibp.pm = new List<string>();
            ibp.mf = new List<string>();
            ibp.js = new List<string>();
            ibp.numbers = new List<int>();
            ibp.number = 0;
            foreach (DictionaryTree dt in OrderList)
            {
                if (dt.dt_id.ToString() == tempID)
                {
                    ibp.khmc = this.StoreService.GetTreeName(dt.parent_id);
                    ibp.number=0;
                    int flag = 0;
                    foreach (DictionaryProperty dp in dt.DictionaryProperty)
                    {
                        ibp.dpid.Add(dp.dpid.ToString());
                        ibp.clmc.Add(dp.clmc);
                        ibp.pm.Add(dp.pm);
                        ibp.mf.Add(dp.mf);
                        ibp.js.Add(dp.js.Value.ToString());  
                        if(dp.leaf_id.Value.ToString() == tempID)//获取订单最大出库数量
                        {
                            int tempNumber = (Convert.ToDecimal(this.StoreService.GetPropertyNumber(dp.dpid)) / dp.js.Value).ToInt();
                            if (flag == 0)
                            {
                                ibp.number = tempNumber;
                                flag = 1;
                            }

                            if (tempNumber==0||tempNumber < ibp.number)
                            {
                                ibp.number = tempNumber;
                                ibp.minclmc = ibp.minclmc + "\\"+ dp.clmc;
                            } 
                        } 
                    }

                }
            }
            string jsonItem = JsonHelper.JsonSerializer(ibp); 
            return Json(jsonItem); 
        }

        /// <summary>
        /// 删除库存物料
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(List<string> ids)
        {
            List<Guid> list = new List<Guid>();
            foreach(string id in ids )
            {
                list.Add(new Guid(id));
            }

            this.StoreService.DeleteStoreItem(list);
            return RedirectToAction("Index");
        }
    }
}