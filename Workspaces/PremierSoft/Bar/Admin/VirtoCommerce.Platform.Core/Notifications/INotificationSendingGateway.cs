﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtoCommerce.Platform.Core.Notifications
{
	public interface INotificationSendingGateway
	{
		SendNotificationResult SendNotification(Notification notification);
		bool ValidateNotification(Notification notification);
	}
}
