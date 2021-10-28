using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
#endif

namespace Data
{
    public static class DatabaseEntryExtentions
    {
        public static IEnumerable<IDatabaseEntry> GetInnerEntriesNonRecursive(this IDatabaseEntryReadonly entry)
        {
            var fields = entry.GetType().GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public).Where(x => typeof(IDatabaseEntry).IsAssignableFrom(x.FieldType));
            var lists = entry.GetType().GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public).Where(x => typeof(IEnumerable<IDatabaseEntry>).IsAssignableFrom(x.FieldType));

            return fields.Select(e => e.GetValue(entry) as IDatabaseEntry).Union(lists.Select(e => e.GetValue(entry) as IEnumerable<DatabaseEntry>).SelectMany(x => x));
        }

        public static IEnumerable<IDatabaseEntryReadonly> GetInnerEntriesRecursive(this IDatabaseEntryReadonly entry)
        {
            var entries = GetInnerEntriesNonRecursive(entry);

            return entries.Union(entries.SelectMany(x => x.InnerEntries));
        }

        public static void UpdateChildGuids(this IDatabaseEntryReadonly entry)
        {
            var entries = GetInnerEntriesNonRecursive(entry);

            foreach (var item in entries)
            {
                item.Guid = System.Guid.NewGuid().ToString("N");
            }
        }
    }
}
