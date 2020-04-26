namespace Sibedge.GraphQlServer
{
    public static class Helper
    {
        public static string ToTypeName(this string str)
        {
            return str.Replace(' ', '_');
        }
    }
}
