﻿namespace LoadOrderToolTwo.Domain.Compatibility;

public enum PackageStability
{
	[CRN(NotificationType.None)] Stable = 1,
	[CRN(NotificationType.Info)] NotEnoughInformation = 2,
	[CRN(NotificationType.Warning)] HasIssues = 3,
	[CRN(NotificationType.Unsubscribe)] Broken = 4,

	[CRN(NotificationType.None, false)] AssetNotReviewed = 98,
	[CRN(NotificationType.Info, false)] NotReviewed = 0,
}
