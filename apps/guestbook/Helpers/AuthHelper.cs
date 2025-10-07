namespace Guestbook.Helpers;

public static class AuthHelper
{
    public static bool IsAdmin(HttpContext context)
    {
        return context.Session.GetString("IsAdmin") == "true";
    }
}