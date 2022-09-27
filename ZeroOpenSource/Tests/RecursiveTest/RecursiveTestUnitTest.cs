using Recursive;

namespace RecursiveTest
{
    [TestClass]
    public class RecursiveTestUnitTest
    {
        private readonly List<Item> _data;
        public RecursiveTestUnitTest()
        {
            _data = new List<Item> {
                    new Item{ Id="1",ParentId = "0" },
                    new Item{ Id="2",ParentId = "1" },
                    new Item{ Id="3",ParentId = "2" }
            };
        }
        [TestMethod]
        public void TestMethod1()
        {
            #region 数据
            //初始化父级节点对象
            var itemUnfold = new Item { Id = "2", ParentId = "1" };
            //自定义平展规则
            Func<Item, Item, bool> whereFunc = (itemChild, itemParent) => itemChild.ParentId == itemParent.Id;
            #endregion
            //行为
            var result = _data.RecursionUnfold(itemUnfold, whereFunc);
            //断言
            Assert.IsNotNull(result);

            Assert.Equals(1,result.Count);

            Assert.Equals(2,result?.FirstOrDefault()?.ParentId);
            
        }
    }
}