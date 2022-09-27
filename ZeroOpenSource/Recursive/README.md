
##递归扩展

##示例
# //初始化父级节点对象
# var itemUnfold = new Item { Id = "2", ParentId = "1" };
# //数据源
# var sourceList = new List<Item> {
#     new Item{ Id="1",ParentId = "0" },
#     new Item{ Id="2",ParentId = "1" },
#     new Item{ Id="3",ParentId = "2" }
# };
# //初始化子级节点id集合
# var idList = new List<string> { "0", "1", "2" };
# //自定义平展规则
# Func<Item, Item, bool> whereFunc = (itemChild, itemParent) => itemChild.ParentId == itemParent.Id;
# var result1 = sourceList.RecursionUnfold(itemUnfold, whereFunc);
# var result2 = sourceList.Recursion();
# //自定义反向递归数据源的Id规则
# Func<Item, List<string>, bool> whereFunc2 = (item, idList) => idList.Contains(item.Id);
# //自定义ParentId规则
# Func<Item, string> selectFuncs2 = (item) => item.ParentId;
# var result3 = sourceList.ReversalRecursion(idList, whereFunc2, selectFuncs2);
# var options = new JsonSerializerOptions
# {
#     IgnoreNullValues = true,
#     WriteIndented = true
# };
# var jsonString = JsonSerializer.Serialize(result3, options);
# Console.WriteLine(JsonSerializer.Serialize(jsonString));
# Console.ReadKey();