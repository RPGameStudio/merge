using System.Collections.Generic;

namespace Data
{
    public abstract class ValidateEntry : IValidateHandler
    {
        public virtual void OnValidate()
        {
            var fields = GetType().GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);

            foreach (var item in fields)
            {
                if (typeof(IValidateHandler).IsAssignableFrom(item.FieldType))
                {
                    OnValidateItem(item.GetValue(this) as IValidateHandler);
                }
                else if (typeof(IEnumerable<IValidateHandler>).IsAssignableFrom(item.FieldType))
                {
                    OnValidateIEnumerable(item.GetValue(this) as IEnumerable<IValidateHandler>);
                }
            }
        }

        protected virtual void OnValidateIEnumerable(IEnumerable<IValidateHandler> query)
        {
            if (query == null)
                return;

            foreach (var val in query)
            {
                OnValidateItem(val);
            }
        }

        protected virtual void OnValidateItem(IValidateHandler item) => item?.OnValidate();
    }
}
