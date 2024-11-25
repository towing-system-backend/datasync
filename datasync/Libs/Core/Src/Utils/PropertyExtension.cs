namespace Datasync.Core
{
    public static class PropertyExtension
    {
        public static T GetProperty<T>(this object obj, string propertyName)
        {
            return (T)obj.GetType().GetProperty(propertyName)!.GetValue(obj)!;
        }
    }
}
