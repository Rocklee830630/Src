using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GMS.Store.Contract;
using GMS.Store.DAL;
using EntityFramework.Extensions;
using GMS.Framework.Contract;
using System.Data;
using NPOI;
using NPOI.HSSF.UserModel;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace GMS.Store.BLL
{
    public class StoreServices : IStoreService
    {
        #region DictionaryProperty CURD
        #region BoundManager
        /// <summary>
        /// 获取树形数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns>列表信息</returns>
        public Object GetDictionaryTreeList()
        { 
            using (var dbContext = new StoreDbContext())
            {
                IQueryable<DictionaryTree> dpp = dbContext.DictionaryTree.Include("DictionaryTree");
                 
                var list = (from a in dbContext.DictionaryTree select new
                {
                    id = a.dt_id,
                    pId = a.parent_id,
                    name = a.name,
                    isParent = !a.is_leaf,
                    LinkUrl = ""
                }).ToList();
                return list;
            }

        }

        /// <summary>
        /// 获取节点属性
        /// </summary>
        /// <param name="id">节点ID</param>
        /// <returns></returns>
        public IEnumerable<DictionaryProperty> GetNodeProperty(StoreTreeNameRequests request)
        {
            request = request ?? new StoreTreeNameRequests();
            using (var dbContext = new StoreDbContext())
            {
                IQueryable<DictionaryProperty> dps = dbContext.DictionaryProperty;
                if(request.treeID!=null)
                { 
                    dps = dps.Where(u => u.leaf_id == request.treeID); 
                }

                return dps.OrderBy(u => u.CreateTime).ToPagedList(request.PageIndex, request.PageSize);
            }
        }
         

        /// <summary>
        /// 添加节点
        /// </summary>
        /// <param name="dt">树结构</param>
        public void InsertDictionaryTree(DictionaryTree dt)
        { 
            using (var dbContext = new StoreDbContext())
            {
                dbContext.Insert<DictionaryTree>(dt);  
            }
        }

        /// <summary>
        /// 删除节点
        /// </summary>
        /// <param name="ids">节点编号列表</param>
        /// <returns></returns>
        public void RemoveTree(List<Guid> ids)
        {
            using (var dbContext = new StoreDbContext())
            {
                dbContext.OutBoundRecord.Where(u => ids.Contains(u.node_id.Value)).ToList().ForEach(a => { dbContext.OutBoundRecord.Remove(a); });
                IQueryable<DictionaryTree> dpp = dbContext.DictionaryTree.Include("DictionaryTree");
                dbContext.DictionaryTree.Where(u => ids.Contains(u.dt_id)).ToList().ForEach(a => {  dbContext.DictionaryTree.Remove(a); }); 
                dbContext.SaveChanges(); 
            }
        }

        /// <summary>
        /// 保存节点属性
        /// </summary>
        /// <param name="dp">属性</param>
        public void SaveDictionaryProperty(DictionaryProperty dp)
        {
            using (var dbContext = new StoreDbContext())
            {
                dbContext.Insert<DictionaryProperty>(dp);
            }
        }

        /// <summary>
        /// 保存修改节点
        /// </summary>
        /// <param name="id">节点ID</param>
        public void SaveDictionaryTree(DictionaryTree dt)
        {
            using (var dbContext = new StoreDbContext())
            {
                dbContext.Update<DictionaryTree>(dt);
            }
        }

        /// <summary>
        /// 根据树的guid获取属性数据
        /// </summary>
        /// <param name="id">GUID</param>
        /// <returns>属性</returns>
        public DictionaryProperty GetDictionaryProperty(Guid id)
        { 
            using (var dbContext = new StoreDbContext())
            { 
                return dbContext.DictionaryProperty.Include("DictionaryTree").Where(u => u.dpid == id).SingleOrDefault();
            }
        }

        /// <summary>
        /// 保存属性编辑数据
        /// </summary>
        /// <param name="dp">字典属性</param>
        public void SaveEditProperty(DictionaryProperty dp)
        {
            using (var dbContext = new StoreDbContext())
            { 
                dbContext.Update<DictionaryProperty>(dp); 
            }
        }

        /// <summary>
        /// 根据GUID删除字典属性数据
        /// </summary>
        /// <param name="ids"></param>
        public void DeleteProperty(List<Guid> ids)
        {
            using (var dbContext = new StoreDbContext())
            {
                dbContext.InBoundRecord.Where(u => ids.Contains(u.inbound_id.Value)).ToList().ForEach(a => { dbContext.InBoundRecord.Remove(a); });
                dbContext.SaveChanges();
                dbContext.StoreTable.Where(u => ids.Contains(u.store_item_id.Value)).ToList().ForEach(a => { dbContext.StoreTable.Remove(a); });
                dbContext.SaveChanges();
                dbContext.DictionaryProperty.Where(u => ids.Contains(u.dpid)).Delete();
            }
        }

        /// <summary>
        /// 获取库存信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public IEnumerable<StoreTable> GetStoreList(StoreTableRequest request = null)
        {
            request = request ?? new StoreTableRequest();
            using (var dbContext = new StoreDbContext())
            {
                IQueryable<StoreTable> store = dbContext.StoreTable.Include("DictionaryProperty.DictionaryTree");
                if (!string.IsNullOrEmpty(request.orderType)&&request.orderType!= "00000000-0000-0000-0000-000000000000")
                { 
                    store = store.Where(d => d.DictionaryProperty.leaf_id == new Guid(request.orderType));
                }
                if (!string.IsNullOrEmpty(request.clmc))
                {
                    store = store.Where(d => d.DictionaryProperty.clmc.Contains(request.clmc));
                }

                return store.OrderByDescending(u => u.CreateTime).ToPagedList(request.PageIndex, request.PageSize);
            }
        }

        /// <summary>
        /// 获取订单信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public IEnumerable<DictionaryTree> GetOrderList(OrderRequest request = null)
        {
            request = request ?? new OrderRequest();
            using (var dbContext = new StoreDbContext())
            {
                IQueryable<DictionaryTree> dictionaryItem = dbContext.DictionaryTree.Include("DictionaryProperty"); 
                dictionaryItem = dictionaryItem.Where(u => u.is_leaf == true);
           
                return dictionaryItem.OrderByDescending(u => u.dt_id).ToPagedList(request.PageIndex, request.PageSize);
            }
        }

        /// <summary>
        /// 根据id获取名称
        /// </summary>
        /// <param name="treeid"></param>
        /// <returns></returns>
        public string GetTreeName(Guid treeid)
        {
            using (var dbContext = new StoreDbContext())
            { 

                DictionaryTree dt = dbContext.DictionaryTree.Include("DictionaryProperty").FirstOrDefault(u => u.dt_id == treeid);
                return dt.name;
            }
            
        }

        /// <summary>
        /// 入库，如果有增加数量，如果没有新增
        /// </summary>
        /// <param name="st"></param>
        public void InsertStoreItem(StoreTable st)
        {
            using (var dbContext = new StoreDbContext())
            {
                var item = dbContext.StoreTable.Where(u=>u.store_item_id==st.store_item_id).FirstOrDefault();
                
                if (item == null)
                {
                    st.kcid = Guid.NewGuid();
                    dbContext.Insert<StoreTable>(st);
                }
                else
                {
                    decimal totalnumber = item.number.Value;
                    string guid = item.kcid.ToString();
                    Guid tempguid = new Guid(guid);
                    st.number += totalnumber;
                    st.kcid = tempguid;

                    ///问题的原因在于，我们之前已经附加过当前实体，如果再进行Attach的时候，就会报这样的错。
                    // 解决办法：1.销毁之前的上下文，重新开启上下文。（等于白说)
                    //2.更改当前上下文的实体的状态。（这个是问题关键）
                    var entry = dbContext.Set<StoreTable>().Find(tempguid);
                    if (entry != null)
                    {
                        dbContext.Entry<StoreTable>(entry).State = System.Data.EntityState.Detached; //这个是在同一个上下文能修改的关键
                    }
                    dbContext.Update<StoreTable>(st);
                }
                
            }
        }

        /// <summary>
        /// 根据属性ID获取单个属性
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DictionaryProperty GetDicProperty(Guid id,string name)
        {
            using (var dbContext = new StoreDbContext())
            {
                DictionaryProperty dp = dbContext.DictionaryProperty.Where(u => u.dpid == id).FirstOrDefault();
                return dp;
            }
        }

        /// <summary>
        /// 根据叶子节点ID获取父节点名称
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetParentNameByLeafID(Guid id)
        {
            using (var dbContext = new StoreDbContext())
            {
                DictionaryTree dt = dbContext.DictionaryTree.Where(u => u.dt_id == id).FirstOrDefault();
                DictionaryTree pdt = dbContext.DictionaryTree.Where(u => u.dt_id == dt.parent_id).FirstOrDefault();
                return pdt.name;
            }
        }

        /// <summary>
        /// 出库保存
        /// </summary>
        /// <param name="st"></param>
        public void OutBoundItem(StoreTable st)
        {
            using (var dbContext = new StoreDbContext())
            {
                dbContext.Update<StoreTable>(st);
            }
        }

        /// <summary>
        /// 根据Store的主键获取对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public StoreTable GetStoreItem(Guid id)
        {
            using (var dbContext = new StoreDbContext())
            {
                return dbContext.StoreTable.Include("DictionaryProperty.DictionaryTree").Where(u => u.kcid == id).FirstOrDefault();
            }
        }

        /// <summary>
        /// 删除StoreItem
        /// </summary>
        /// <param name="ids"></param>
        public void DeleteStoreItem(List<Guid> ids)
        {
            using (var dbContext = new StoreDbContext())
            { 
                dbContext.StoreTable.Where(u => ids.Contains(u.kcid)).ToList().ForEach(a => { dbContext.StoreTable.Remove(a); });
                dbContext.SaveChanges(); 
            }
        }

        /// <summary>
        /// 插入库存操作记录
        /// </summary>
        /// <param name="ibr"></param>
        public void InsertInboundRecord(InBoundRecord ibr)
        {
            using (var dbContext = new StoreDbContext())
            { 
                StoreTable st = dbContext.StoreTable.Include("DictionaryProperty.DictionaryTree").Where(u => u.store_item_id == ibr.inbound_id).FirstOrDefault();
                 
                if(st==null)
                {
                    ibr.before_number = 0;

                }
                else
                {
                    ibr.before_number = st.number;
                    ibr.khmc = GetParentNameByLeafID(st.DictionaryProperty.leaf_id.Value);
                }
                
                dbContext.Insert<InBoundRecord>(ibr); 
            }
        }

        /// <summary>
        /// 由于按订单出库，因此单独有表记录订单出库数量
        /// </summary>
        /// <param name="obr"></param>
        public void InsertOutBoundRecord(OutBoundRecord obr)
        {
            using (var dbContext = new StoreDbContext())
            { 
                dbContext.Insert<OutBoundRecord>(obr);
            }
        }
        #endregion

        #region OutBoundServices
        public IEnumerable<OutBoundRecord> GetOutBound(OutBoundRequest request)
        {
            request = request ?? new OutBoundRequest();
            using (var dbContext = new StoreDbContext())
            {
                IQueryable<OutBoundRecord> obr = dbContext.OutBoundRecord.Include("DictionaryTree");
                if (!string.IsNullOrEmpty(request.khmc))
                {
                    obr = obr.Where(d => d.DictionaryTree.parent_name.Contains(request.khmc));
                }
                if (!string.IsNullOrEmpty(request.ddmc))
                {
                    obr = obr.Where(d => d.DictionaryTree.name.Contains(request.ddmc));
                }
                if (request.starttime.ToString()!= "0001/1/1 0:00:00" && request.endtime.ToString()== "0001/1/1 0:00:00")
                { //有开始时间和无结束时间
                    obr = obr.Where(d => d.CreateTime>=request.starttime);
                }
                if (request.starttime.ToString() == "0001/1/1 0:00:00" && request.endtime.ToString() != "0001/1/1 0:00:00")
                { //无开始时间和有结束时间
                    obr = obr.Where(m => m.CreateTime <= request.endtime);
                }
                if (request.starttime.ToString() != "0001/1/1 0:00:00" && request.endtime.ToString() != "0001/1/1 0:00:00")
                { //有开始时间和结束时间
                    obr = obr.Where(m => m.CreateTime <= request.endtime).Where(d => d.CreateTime >= request.starttime);
                }
                return obr.OrderByDescending(u => u.CreateTime).ToPagedList(request.PageIndex, request.PageSize);
            }
        }

        public IEnumerable<InBoundRecord> GetInBound(InBoundRequest request)
        {
            request = request ?? new InBoundRequest();
            using (var dbContext = new StoreDbContext())
            { 
                IQueryable<InBoundRecord> ibr = dbContext.InBoundRecord.Include("DictionaryProperty.DictionaryTree");
                //ibr = dbContext.InBoundRecord.Include("DictionaryTree");
                if (!string.IsNullOrEmpty(request.khmc))
                {
                    ibr = ibr.Where(d => d.khmc.Contains(request.khmc));
                }
                if (!string.IsNullOrEmpty(request.ddmc))
                {
                    ibr = ibr.Where(d => d.DictionaryProperty.DictionaryTree.name.Contains(request.ddmc));
                }
                if (!string.IsNullOrEmpty(request.clmc))
                {
                    ibr = ibr.Where(d => d.DictionaryProperty.clmc.Contains(request.clmc));
                }
                if (!string.IsNullOrEmpty(request.czlx))
                {
                    if(request.czlx == "入库"|| request.czlx == "还库材料"|| request.czlx == "出库材料")
                    {
                         ibr = ibr.Where(d => d.boundtype.Contains(request.czlx));
                    } 
                }
                if (request.starttime.ToString() != "0001/1/1 0:00:00" && request.endtime.ToString() == "0001/1/1 0:00:00")
                { //有开始时间和无结束时间
                    ibr = ibr.Where(d => d.CreateTime >= request.starttime);
                }
                if (request.starttime.ToString() == "0001/1/1 0:00:00" && request.endtime.ToString() != "0001/1/1 0:00:00")
                { //无开始时间和有结束时间
                    ibr = ibr.Where(m => m.CreateTime <= request.endtime);
                }
                if (request.starttime.ToString() != "0001/1/1 0:00:00" && request.endtime.ToString() != "0001/1/1 0:00:00")
                { //有开始时间和结束时间
                    ibr = ibr.Where(m => m.CreateTime <= request.endtime).Where(d => d.CreateTime >= request.starttime);
                }
                return ibr.OrderByDescending(u => u.CreateTime).ToPagedList(request.PageIndex, request.PageSize);
            }

        }

        /// <summary>
        /// 根据订单ID删除所有相关数据，包括材料列表、入库记录、出库记录
        /// </summary>
        /// <param name="id"></param>
        public void DeleteAllDataByLeafId(List<Guid> ids)
        { 
            using (var dbContext = new StoreDbContext())
            {
                dbContext.InBoundRecord.Where(u => ids.Contains(u.DictionaryProperty.leaf_id.Value)).ToList().ForEach(a => { dbContext.InBoundRecord.Remove(a); });
                dbContext.StoreTable.Where(u => ids.Contains(u.DictionaryProperty.leaf_id.Value)).ToList().ForEach(a => { dbContext.StoreTable.Remove(a); });
                dbContext.DictionaryProperty.Where(u => ids.Contains(u.leaf_id.Value)).ToList().ForEach(a => { dbContext.DictionaryProperty.Remove(a); });
                dbContext.SaveChanges();
            }
        }

        /// <summary>
        /// 读取EXCEL
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public DataTable ReadExcel(string filePath)
        {
            IWorkbook workbook = null; 
            ISheet sheet = null;//得到里面第一个sheet
            //第一行一般为标题行。
            DataTable table = new DataTable();
            using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                if (filePath.IndexOf(".xlsx") > 0) // 2007版本
                {
                    workbook = new XSSFWorkbook(file);
                    sheet = (XSSFSheet)workbook.GetSheetAt(0);
                }
                else if (filePath.IndexOf(".xls") > 0) // 2003版本 
                {
                    workbook = new HSSFWorkbook(file);
                    sheet = (HSSFSheet)workbook.GetSheetAt(0);
                }
            }
            //根据路径通过已存在的excel来创建HSSFWorkbook，即整个excel文档
            
            //HSSFWorkbook workbook = new HSSFWorkbook(File.Open(filePath, FileMode.Open));
            //HSSFSheet sheet = (HSSFSheet)workbook.GetSheetAt(0);
            //获取excel的第一个sheet


            //获取Excel的最大行数
            int rowsCount = sheet.PhysicalNumberOfRows;
            //为保证Table布局与Excel一样，这里应该取所有行中的最大列数（需要遍历整个Sheet）。
            //为少一交全Excel遍历，提高性能，我们可以人为把第0行的列数调整至所有行中的最大列数。
            int colsCount = sheet.GetRow(1).PhysicalNumberOfCells;

            table.Columns.Add("材料名称");
            table.Columns.Add("品名");
            table.Columns.Add("门幅");
            table.Columns.Add("基数米");
            //for (int i = 0; i < colsCount; i++)
            //{
            //    table.Columns.Add(i.ToString());
            //}

            for (int x = 2; x < rowsCount; x++)
            {
                DataRow dr = table.NewRow();
                for (int y = 0; y < colsCount; y++)
                {
                    dr[y] = sheet.GetRow(x).GetCell(y).ToString();
                }
                table.Rows.Add(dr);
            }

            sheet = null;
            workbook = null;
            return table;
        }

        /// <summary>
        /// 根据材料主键ID获取数量
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public decimal GetPropertyNumber(Guid id)
        {
            using (var dbContext = new StoreDbContext())
            {
                var dpitem = dbContext.StoreTable.Where(u => u.store_item_id.Value == id).FirstOrDefault();
                if (dpitem == null)
                {
                    return 0;//库存里不存在的数据为0
                }
                else
                {
                    return dpitem.number.Value;
                }
            }
        }
        #endregion
        #endregion
    }
}
