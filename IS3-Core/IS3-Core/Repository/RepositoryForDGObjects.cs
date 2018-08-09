using iS3.Core.Service;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace iS3.Core
{
    public class DGObjectRepository : IRepository<DGObject>
    {
        #region parameters
        private string projectID;
        private string domainType;
        private string dgobjectName;
        #endregion

        #region instance
        private static DGObjectRepository _DGObjectsRepository;
        public static DGObjectRepository Instance(string prjID,string domainType,string dgobjectName)
        {
            if (_DGObjectsRepository == null)
            {
                _DGObjectsRepository = new DGObjectRepository();
            }
            _DGObjectsRepository.projectID = prjID;
            _DGObjectsRepository.domainType = domainType;
            _DGObjectsRepository.dgobjectName = dgobjectName;
            return _DGObjectsRepository;
        }
        private DGObjectRepository() { }
        #endregion

        public DGObject Create(DGObject model)
        {
            throw new NotImplementedException();
        }

        public List<DGObject> BatchCreate(List<DGObject> models)
        {
            throw new NotImplementedException();
        }

        public DGObject Update(DGObject model)
        {
            throw new NotImplementedException();
        }

        public List<DGObject> BatchUpdate(List<DGObject> models)
        {
            throw new NotImplementedException();
        }

        public DGObject Retrieve(Guid guid)
        {
            throw new NotImplementedException();
        }

        public DGObject Retrieve(Expression<Func<DGObject, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public async Task<DGObject> Retrieve(int key)
        {
            try {
    
                    //网络请求 
                    string result = await Task.Run(() =>
                       WebApiCaller.HttpGet(ServiceConfig.BaseURL +
                       string.Format(ServiceConfig.DGObjectByIDFormat, domainType.ToLower(), dgobjectName.ToLower(), projectID, key)));
                    //
                    JObject obj = JObject.Parse(result);
                    string data = obj["data"].ToString();
                    //
                    DGObject objHelper =
                      ObjectHelper.CreateDGObjectFromSubclassName(dgobjectName);
                    var _obj = JsonConvert.DeserializeObject(data, objHelper.GetType());
                    return _obj as DGObject;

            }
            catch (Exception e) { return null; }

        }
        static IEnumerable<Type> subclasses = null;
        //获取对象列表
        public async Task<List<DGObject>> GetAllByObjs(string filter)
        {
            //网络请求 
            string result = await Task.Run(() =>
               WebApiCaller.HttpGet(ServiceConfig.BaseURL +
               string.Format(ServiceConfig.DGObjectListFormat, domainType.ToLower(), dgobjectName.ToLower(), projectID,iS3.Core.APIMessageConverter.EnCode(filter))));
            //
            JObject obj = JObject.Parse(result);
            string data = obj["data"].ToString();
            JArray objList = JArray.Parse(data);
            //
            if (subclasses == null)
            {
                subclasses =
                        from assembly in AppDomain.CurrentDomain.GetAssemblies()
                        from type in assembly.GetTypes()
                        where type.IsSubclassOf(typeof(DGObject))
                        select type;
            }

            // match the subclassName with full name at first
            Type t = subclasses.FirstOrDefault(x => x.FullName == dgobjectName);

            // if not found, match the subclassName with name
            if (t == null)
                t = subclasses.FirstOrDefault(x => x.Name == dgobjectName);

            List<DGObject> list = new List<DGObject>();
            foreach (JToken token in objList)
            {
                var _obj = JsonConvert.DeserializeObject(token.ToString(), t);
                list.Add(_obj as DGObject);
            }
            return list;
        }
        //获取对象列表
        public async Task<List<DGObject>> GetAllAsync()
        {
            //网络请求 
            string result = await Task.Run(() =>
               WebApiCaller.HttpGet(ServiceConfig.BaseURL +
               string.Format(ServiceConfig.DGObjectListFormat, domainType.ToLower(), dgobjectName.ToLower(), projectID,"")));
            //
            JObject obj = JObject.Parse(result);
            string data = obj["data"].ToString();
            JArray objList = JArray.Parse(data);
            //
            if (subclasses == null)
            {
                subclasses =
                        from assembly in AppDomain.CurrentDomain.GetAssemblies()
                        from type in assembly.GetTypes()
                        where type.IsSubclassOf(typeof(DGObject))
                        select type;
            }

            // match the subclassName with full name at first
            Type t = subclasses.FirstOrDefault(x => x.FullName == dgobjectName);

            // if not found, match the subclassName with name
            if (t == null)
                t = subclasses.FirstOrDefault(x => x.Name == dgobjectName);

            List<DGObject> list = new List<DGObject>();
            foreach (JToken token in objList)
            {
                var _obj = JsonConvert.DeserializeObject(token.ToString(), t);
                list.Add(_obj as DGObject);
            }
            return list;
        }
        public List<DGObject> GetAll(Expression<Func<DGObject, bool>> expression, Expression<Func<DGObject, dynamic>> sortPredicate, SortOrder sortOrder, int skip, int take, out int total)
        {
            throw new NotImplementedException();
        }

        public int Delete(Guid guid)
        {
            throw new NotImplementedException();
        }

        public int Delete(int key)
        {
            throw new NotImplementedException();
        }

        public int BatchDelete(IList<Guid> guids)
        {
            throw new NotImplementedException();
        }





    }
}
