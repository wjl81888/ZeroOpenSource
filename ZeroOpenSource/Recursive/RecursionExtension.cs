using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recursive
{
    /// <summary>
    /// 递归扩展
    /// </summary>
    public static class RecursionExtension
    {
        /// <summary>
        /// 递归平展数据（往下递归） 从传入的item对象以及规则 往下查找，返回整颗树
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">数据源</param>
        /// <param name="parent">父级对象</param>
        /// <param name="whereFunc">条件函数</param>
        /// <returns></returns>
        public static List<T> RecursionUnfold<T>(this List<T> source, T parent, Func<T, T, bool> whereFunc)
        {
            var newList = new List<T>();
            var list = source.Where(x => whereFunc(x, parent)).ToList();
            newList.AddRange(list);
            list.ForEach(x => newList.AddRange(RecursionUnfold(source, x, whereFunc)));
            return newList;
        }


        /// <summary>
        /// 树形数据（往下递归，需要继承） 从传入的pid为顶端往下查找 ，返回整颗树
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">源</param>
        /// <param name="pid">树顶端Id</param>
        /// <returns>整颗树List</returns>
        public static List<T> Recursion<T>(this List<T> source, string pid = "0") where T : BaseTree<T>
        {
            var list = source.Where(x => x.ParentId == pid).ToList();
            list.ForEach(x =>
            {
                var childList = Recursion(source, x.Id);
                if (!childList.Any())
                    return;
                x.ChildList ??= new List<T>();
                x.ChildList.AddRange(childList);
            });
            return list;
        }

        /// <summary>
        /// 反向递归
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">数据源</param>
        /// <param name="idList">子节点Id集合</param>
        /// <param name="whereFunc">条件函数</param>
        /// <param name="selectFuncs">筛选函数</param>
        /// <returns>找出idList里的每一个Id的父级List然后去重合并</returns>
        public static List<T> ReversalRecursion<T>(this List<T> source, List<string> idList, Func<T, List<string>, bool> whereFunc,
            Func<T, string> selectFuncs)
        {
            //去重
            var cidList = idList.Distinct().ToList();
            var list = source.Where(x => whereFunc(x, cidList)).ToList();
            //上级菜单
            var pidList = list.Select(selectFuncs)
                              .Where(x => !string.IsNullOrEmpty(x) && !cidList.Contains(x))
                              .Distinct().ToList();
            if (pidList.Any())
                list.AddRange(ReversalRecursion(source, pidList, whereFunc, selectFuncs));
            return list;
        }
    }
}
