using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;
using VirtoCommerce.Platform.Core.Notifications;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.Platform.Data.Notifications
{
    public class SendGridEmailNotificationSendingGateway : IEmailNotificationSendingGateway
    {
        private readonly ISettingsManager _settingsManager;

        private const string _sendGridApiKeySettingName = "VirtoCommerce.Platform.Notifications.SendGrid.ApiKey";

        public SendGridEmailNotificationSendingGateway(ISettingsManager settingsManager)
        {
            if (settingsManager == null)
                throw new ArgumentNullException("settingsManager");

            _settingsManager = settingsManager;
        }

        public SendNotificationResult SendNotification(Notification notification)
        {
            return Task.Run(() => SendNotificationAsync(notification)).Result;
        }


        public bool ValidateNotification(Notification notification)
        {
            var retVal = ValidateNotificationRecipient(notification.Recipient);
            return retVal;
        }


        private async Task<SendNotificationResult> SendNotificationAsync(Notification notification)
        {
            var retVal = new SendNotificationResult();
            var apiKey = _settingsManager.GetSettingByName(_sendGridApiKeySettingName).Value;
            var sendGridClient = new SendGridClient(apiKey);

            var from = new EmailAddress(notification.Sender);
            var to = new EmailAddress(notification.Recipient);
            var mail = new SendGridMessage() { From = from, Subject = notification.Subject, ReplyTo = to, HtmlContent = notification.Body };
            mail.AddTo(to);

            try
            {
                var result = await sendGridClient.SendEmailAsync(mail);
                retVal.IsSuccess = result.StatusCode == HttpStatusCode.Accepted;
                if (!retVal.IsSuccess)
                {
                    retVal.ErrorMessage = result.StatusCode.ToString() + ":" + await result.Body.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                retVal.ErrorMessage = ex.Message;
            }
            return retVal;
        }


        private static bool ValidateNotificationRecipient(string recipient)
        {
            try
            {
                new MailAddress(recipient);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
