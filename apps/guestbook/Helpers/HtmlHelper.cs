using Guestbook.Models;

namespace Guestbook.Helpers;

public static class HtmlHelper
{
    public static string RenderMessageList(List<Message> messages)
    {
        var html = "<ul>";
        foreach (var message in messages)
        {
            html += $"<li><strong>{message.CreatedBy}</strong> ({message.CreatedAt:yyyy-MM-dd HH:mm}): {message.Text}</li>";
        }
        html += "</ul>";
        return html;
    }

    public static string RenderAdminPanel()
    {
        return @"
            <div id='admin-section'>
                <h3>Admin Panel</h3>
                <form hx-post='/api/messages' hx-target='#messages-list' hx-swap='innerHTML'>
                    <textarea name='text' maxlength='200' placeholder='Enter message...' required></textarea><br>
                    <button type='submit'>Post Message</button>
                </form>
                <button hx-post='/api/logout' hx-target='#admin-section' hx-swap='outerHTML'>Logout</button>
            </div>";
    }

    public static string RenderLoginForm()
    {
        return @"
            <div id='admin-section'>
                <h3>Admin Login</h3>
                <form hx-post='/api/login' hx-target='#admin-section' hx-swap='outerHTML'>
                    <input type='text' name='username' placeholder='Username' required /><br>
                    <input type='password' name='password' placeholder='Password' required /><br>
                    <button type='submit'>Login</button>
                </form>
            </div>";
    }
}