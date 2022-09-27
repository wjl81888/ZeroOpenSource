namespace Recursive
{
    public interface BaseTree<T>
    {
        public string Id { get; set; }

        public string ParentId { get; set; }

        public List<T> ChildList { get; set; }

    }
}