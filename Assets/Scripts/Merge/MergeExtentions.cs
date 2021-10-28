namespace Merge
{
    public static class MergeExtentions
    {
        private static IMerger<EntityData> _merger;

        public static void Initialize(IMerger<EntityData> merger) => _merger = merger;

        public static EntityData Merge(this EntityData @this, EntityData other) => _merger.Merge(@this, other);
        public static bool CanBeMerged(this EntityData @this, EntityData other) => _merger.CanBeMerged(@this, other);
    }
}