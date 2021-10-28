namespace Data
{
    public static class StateExtentions
    {
        private static IContentStorage _contentStorage;

        public static void Initialize(IContentStorage storage) => _contentStorage = storage;

        internal static T GetData<T>(this EntryState<T> entry) where T : IDatabaseEntry => _contentStorage.RequestData<T>(entry._dbGuid);
    }
}