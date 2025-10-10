using Guestbook.Models;

namespace Guestbook.Helpers;

public static class HtmlHelper
{
    public static string RenderMessages(List<Message> messages, bool isAdmin)
    {
        if (messages == null || messages.Count == 0)
        {
            return "<p>No messages yet. Be the first to post!</p>";
        }

        var html = new System.Text.StringBuilder();

        if (isAdmin)
        {
            html.Append("<form id='delete-form' hx-post='/guestbook/api/messages/delete' hx-target='#messages-list' hx-swap='innerHTML'>");
        }

        html.Append("<ul style='list-style: none; padding: 0;'>");

        foreach (var msg in messages)
        {
            html.Append("<li style='margin-bottom: 10px;'>");

            if (isAdmin)
            {
                html.Append($"<input type='checkbox' name='messageIds' value='{msg.Id}' style='margin-right: 10px;'>");
            }
            else
            {
                html.Append($"<input type='checkbox' disabled style='margin-right: 10px; opacity: 0.3;'>");
            }

            html.Append($"<strong>{msg.CreatedBy}</strong> ({msg.CreatedAt:yyyy-MM-dd HH:mm}): {msg.Text}");
            html.Append("</li>");
        }

        html.Append("</ul>");

        if (isAdmin)
        {
            html.Append("<button type='submit' style='margin-top: 10px;'>Delete Selected</button>");
            html.Append("</form>");
        }

        return html.ToString();
    }

    public static string RenderAdminPanel()
    {
        return @"
            <div id='admin-section'>
                <h3>Admin Panel</h3>
                <form hx-post='/guestbook/api/messages' 
                      hx-target='#messages-list' 
                      hx-swap='innerHTML'
                      hx-on::after-request=""this.reset()"">
                    <textarea name='text' placeholder='Your message' maxlength='200' required 
                          style='width: 400px; height: 80px; resize: both;'></textarea><br>
                    <button type='submit'>Post Message</button>
                </form>
                <form hx-post='/guestbook/api/logout' hx-target='#admin-section' hx-swap='outerHTML' style='margin-top: 10px;'>
                    <button type='submit'>Logout</button>
                </form>
            </div>";
    }

    public static string RenderLoginForm()
    {
        return @"
            <div id='admin-section'>
                <h3>Admin Login</h3>
                <form hx-post='/guestbook/api/login' hx-target='#admin-section' hx-swap='outerHTML'>
                    <input type='text' name='username' placeholder='Username' required /><br>
                    <input type='password' name='password' placeholder='Password' required /><br>
                    <button type='submit'>Login</button>
                </form>
            </div>";
    }
}