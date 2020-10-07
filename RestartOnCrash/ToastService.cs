using System.Runtime.InteropServices;
using Windows.UI.Notifications;

namespace RestartOnCrash
{
    public static class ToastService
    {
        public static void Notify(string message)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var template = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01);

                var textNodes = template.GetElementsByTagName("text");
                textNodes.Item(0).InnerText = message;

                var notifier = ToastNotificationManager.CreateToastNotifier("RestartOnCrash");
                var notification = new ToastNotification(template);
                notifier.Show(notification);
            }
        }
    }
}
