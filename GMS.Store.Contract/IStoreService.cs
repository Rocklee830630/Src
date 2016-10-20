using GMS.Store.Contract;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GMS.Store.Contract
{
    public interface IStoreService
    { 
        //获取树数据
        Object GetDictionaryTreeList(); 

        //添加树节点
        void InsertDictionaryTree(DictionaryTree dt);

        //修改保存树节点
        void SaveDictionaryTree(DictionaryTree dt);

        //获取指定节点属性
        IEnumerable<DictionaryProperty> GetNodeProperty(StoreTreeNameRequests request);

        void RemoveTree(List<Guid> ids);

        void SaveDictionaryProperty(DictionaryProperty dp);

        DictionaryProperty GetDictionaryProperty(Guid id);

        DictionaryProperty GetDicProperty(Guid id,string name);

        void SaveEditProperty(DictionaryProperty dp);

        void DeleteProperty(List<Guid> ids);

        IEnumerable<StoreTable> GetStoreList(StoreTableRequest request = null);

        IEnumerable<DictionaryTree> GetOrderList(OrderRequest request = null);

        string GetTreeName(Guid treeid);

        string GetParentNameByLeafID(Guid id);

        void InsertStoreItem(StoreTable st);

        void OutBoundItem(StoreTable st);

        StoreTable GetStoreItem(Guid id);
        void DeleteStoreItem(List<Guid> ids);

        void InsertInboundRecord(InBoundRecord ibr);

        void InsertOutBoundRecord(OutBoundRecord obr);

        IEnumerable<OutBoundRecord> GetOutBound(OutBoundRequest request);

        IEnumerable<InBoundRecord> GetInBound(InBoundRequest request);

        void DeleteAllDataByLeafId(List<Guid> ids);

        DataTable ReadExcel(string filePath);

        decimal GetPropertyNumber(Guid id);
    }
}
