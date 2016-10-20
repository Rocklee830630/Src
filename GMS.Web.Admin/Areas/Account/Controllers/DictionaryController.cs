using GMS.Account.Contract;
using GMS.Web.Admin.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GMS.Framework.Utility;
using System.Text;
using System.Data;
using GMS.Store.Contract;

namespace GMS.Web.Admin.Areas.Account.Controllers
{
    [Permission(EnumBusinessPermission.Order_Itembase)]
    public class DictionaryController : AdminControllerBase
    {
        string treeID = null; 
        
        public ActionResult Index(StoreTreeNameRequests request)
        { 
            var result = this.StoreService.GetNodeProperty(request); 
            return View(result);  
        }

        [HttpPost]
        public JsonResult GetSelectTreeID(StoreTreeNameRequests request,FormCollection collection)
        { 
            bool isParent = bool.Parse(collection["isParent"]);
            var dataString = collection["id"].ToString();
            this.treeID = dataString;
            request.treeID = new Guid(dataString);
            request.PageSize = 100;
            //Guid id = new Guid(dataString);
            this.TempData["treeid"] = dataString;
            if (!isParent)
            {
                ModelState.Clear();
                var result = this.StoreService.GetNodeProperty(request);
                ViewData.ModelState.Remove("userPwd");
                ModelState.AddModelError("userPwd", "请输入密码aaaaaaaaaaaaaaa！");
                List<DictionaryProperty> resultList = new List<DictionaryProperty>();
                foreach (DictionaryProperty dp in result)
                {
                    resultList.Add(dp);
                } 
                return Json(resultList, JsonRequestBehavior.AllowGet);
            }

            else
            {
                JsonResult json = new JsonResult
                {
                    Data = "ParentNode"
                };
                return Json(json);
            }
        }

        [HttpPost]
        public ActionResult GetID(StoreTreeNameRequests request)
        {  
            var dataString = "8b0bdde6-7f59-4dc2-acc5-6216dfdc973d";
            request.treeID = new Guid(dataString);
            //Guid id = new Guid(dataString);
            this.TempData["treeid"] = dataString; 
                var result = this.StoreService.GetNodeProperty(request);
                return View(result); 
          
        }
        public ActionResult Delete()
        {
            return View();
        }
        public ActionResult GetTree()
        {
            var list = this.StoreService.GetDictionaryTreeList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditTree(FormCollection collection)
        {
            var dataString = collection[0].ToString();
            DictionaryTree dt = new DictionaryTree();
            TreeProperty treeProperty = JsonHelper.JsonDeserialize<TreeProperty>(dataString);
            dt.dt_id = treeProperty.id;
            dt.is_leaf = treeProperty.type;
            dt.name = treeProperty.name;
            dt.parent_id = treeProperty.pid;
            if(treeProperty.pid.ToString() != "11111111-1111-1111-1111-111111111111")
            {
                dt.parent_name = this.StoreService.GetTreeName(treeProperty.pid);
            }
            
            this.StoreService.SaveDictionaryTree(dt);

            return View();
        }

        [HttpPost]
        public ActionResult AddType(FormCollection collection)
        {
            
            var dataString = collection[0].ToString();
            DictionaryTree dt = new DictionaryTree();
            TreeProperty treeProperty = JsonHelper.JsonDeserialize<TreeProperty>(dataString);
          
            dt.dt_id = treeProperty.id;
            dt.is_leaf = treeProperty.type;
            dt.name = treeProperty.name;
            dt.parent_id = treeProperty.pid;
            if (treeProperty.pid.ToString() != "11111111-1111-1111-1111-111111111111")
            {
                dt.parent_name = this.StoreService.GetTreeName(treeProperty.pid);
            }
            this.StoreService.InsertDictionaryTree(dt);
            //var list = this.StoreService.GetDictionaryTreeList(null);
            return View();
        }

        public ActionResult RemoveTree(FormCollection collection)
        {
            var dataString = collection[0].ToString();
            string[] strarr = dataString.Split(',');
            List<Guid> ids = new List<Guid>();
            foreach(string id in strarr)
            {
                Guid guid = new Guid(id.ToString());
                ids.Add(guid);
            } 
            //删除树形表中内容(由于有关联所以出库记录也删除了)
            this.StoreService.RemoveTree(ids);
            //删除材料表中相关数据、库存表、入库记录
            this.StoreService.DeleteAllDataByLeafId(ids);
             
            return View();
        }
        
        public ActionResult Create()
        {
            var model = new DictionaryProperty(); 
            if(this.TempData.ContainsKey("treeid"))
            {
                
            }
            model.dpid = new Guid("00000000-0000-0000-0000-000000000000");
            return View("Edit",model);
        }
         
        [HttpPost]
        public PartialViewResult DPropertyPartialPage(StoreTreeNameRequests request,FormCollection collection)
        {
            bool isParent = bool.Parse(collection["isParent"]);
            var dataString = collection["id"].ToString();
            this.treeID = dataString;
            request.treeID = new Guid(dataString);
            request.PageSize = 500;
            //Guid id = new Guid(dataString);
            this.TempData["treeid"] = dataString;
            if (!isParent)
            { 
                var result = this.StoreService.GetNodeProperty(request);
                return PartialView("_DPropertyPartialPage", result);
            }
            else
            {
                return PartialView("_DPropertyPartialPage");
            }
           
        }

        /// <summary>
        /// 添加并获取属性数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult AddProperty(StoreTreeNameRequests request,FormCollection collection)
        {
            var model = new DictionaryProperty();
            this.TryUpdateModel<DictionaryProperty>(model);

            string id = null;
            if (this.TempData.ContainsKey("treeid"))
            {
                id = this.TempData["treeid"].ToString();
                //TempData.Remove("treeid");
                request.treeID = new Guid(id.ToString());
            }

            if (id != null)
            {
                Guid treeid = new Guid(id);
                Guid keyid = Guid.NewGuid();
                model.leaf_id = treeid;
                model.dpid = keyid;
            } 
            this.StoreService.SaveDictionaryProperty(model);
            //Dictionary<string, string> dp = new Dictionary<string, string>();
            //dp.Add("clmc", model.clmc);
            //dp.Add("pm", model.pm);
            //dp.Add("mf", model.mf);
            //dp.Add("js", model.js);
            request.PageSize = 500;
            var result = this.StoreService.GetNodeProperty(request);
            return PartialView("_DPropertyPartialPage", result);

        }

        /// <summary>
        /// 点击编辑按钮
        /// </summary>
        /// <param name="id">树节点ID</param>
        /// <returns>返回所选节点属性</returns>
        public ActionResult Edit(string id)
        {
            var model = new DictionaryProperty();
            if (id!=null||id!="")
            {
                Guid treeid = new Guid(id);
                model = this.StoreService.GetDictionaryProperty(treeid); 
            } 
            return View("Edit", model);
        }

        /// <summary>
        /// 编辑提交按钮
        /// </summary>
        /// <param name="request"></param>
        /// <param name="collection"></param>
        /// <returns>返回局部视图全部数据页面</returns>
        [HttpPost]
        public PartialViewResult EditProperty(StoreTreeNameRequests request, FormCollection collection)
        { 
            string id = collection["dpid"].ToString();
            Guid dpid = new Guid(id);
            var model = this.StoreService.GetDictionaryProperty(dpid);
            this.TryUpdateModel<DictionaryProperty>(model);

            this.StoreService.SaveEditProperty(model);
            string leafid = model.leaf_id.ToString();
            request.treeID = new Guid(leafid);
            request.PageSize = 500;
            var result = this.StoreService.GetNodeProperty(request);
            return PartialView("_DPropertyPartialPage", result);
        }
        /// <summary>
        /// 根据材料属性ID列表删除材料
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult Delete(string ids, FormCollection collection)
        { 
            string[] strarr = ids.Split(',');
            List<Guid> dellist = new List<Guid>();
            foreach (var i in strarr)
            {
                if(i.Length==36)
                {
                    Guid delID = new Guid(i);
                    dellist.Add(delID);
                }
                
            }
            this.StoreService.DeleteProperty(dellist);

            string id = collection["treeItemID"].ToString();
            Guid dpid = new Guid(id);
            StoreTreeNameRequests request = new StoreTreeNameRequests();
            request.treeID = dpid;
            request.PageSize = 500;
            var model = this.StoreService.GetNodeProperty(request); 
            return PartialView("_DPropertyPartialPage", model);
        }
        /// <summary>
        /// EXCEL材料导入
        /// </summary>
        /// <param name="request"></param>
        /// <param name="Filedata"></param>
        /// <returns></returns>
        public PartialViewResult UploadFile(StoreTreeNameRequests request,HttpPostedFileBase Filedata,string treeidt)
        {
            
            // 如果没有上传文件
            if (Filedata == null ||
                string.IsNullOrEmpty(Filedata.FileName) ||
                Filedata.ContentLength == 0)
            {
                //return this.HttpNotFound();
            }

            // 保存到 ~/photos 文件夹中，名称不变
            string filename = System.IO.Path.GetFileName(Filedata.FileName); 
            string virtualPath =
                string.Format("~/UploadFiles/{0}", filename);
            // 文件系统不能使用虚拟路径
            string path = this.Server.MapPath(virtualPath); 
            Filedata.SaveAs(path);

            var model = new DictionaryProperty();
            string id = null;
            if (this.TempData.ContainsKey("treeid"))
            {
                id = this.TempData["treeid"].ToString();
                //TempData.Remove("treeid");
                request.treeID = new Guid(id.ToString());
            }

            if (treeidt != null)
            { 
                Guid treeid = new Guid(treeidt);
                request.treeID = treeid;
                model.leaf_id = treeid; 
            }
            DataTable dt = this.StoreService.ReadExcel(path);
            foreach (DataRow row in dt.Rows)
            {//判断exel数据异常返回，不存数据
                var clmc = row[0].ToString();
                var pm = row[1].ToString();
                var mf = row[2].ToString();
                if(row[3].ToString()==""|| row[3].ToString() ==null)
                {
                    row[3] = 1;
                }
                var temp = decimal.Parse(row[3].ToString());
            }
            foreach (DataRow row in dt.Rows)
            {
                Guid keyid = Guid.NewGuid();
                model.dpid = keyid;
                model.clmc = row[0].ToString();
                model.pm = row[1].ToString(); 
                model.mf = row[2].ToString();
                model.js = decimal.Parse(row[3].ToString()) ;
                this.StoreService.SaveDictionaryProperty(model);
            }

            //Dictionary<string, string> dp = new Dictionary<string, string>();
            //dp.Add("clmc", model.clmc);
            //dp.Add("pm", model.pm);
            //dp.Add("mf", model.mf);
            //dp.Add("js", model.js);
            request.PageSize = 500;
            var result = this.StoreService.GetNodeProperty(request);
            return PartialView("_DPropertyPartialPage", result);
        }
    }
}