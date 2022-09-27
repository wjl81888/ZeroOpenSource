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
            #region ����
            //��ʼ�������ڵ����
            var itemUnfold = new Item { Id = "2", ParentId = "1" };
            //�Զ���ƽչ����
            Func<Item, Item, bool> whereFunc = (itemChild, itemParent) => itemChild.ParentId == itemParent.Id;
            #endregion
            //��Ϊ
            var result = _data.RecursionUnfold(itemUnfold, whereFunc);
            //����
            Assert.IsNotNull(result);

            Assert.Equals(1,result.Count);

            Assert.Equals(2,result?.FirstOrDefault()?.ParentId);
            
        }
    }
}