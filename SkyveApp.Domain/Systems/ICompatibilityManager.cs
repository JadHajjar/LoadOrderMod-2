﻿using SkyveApp.Domain.Enums;

using System;

namespace SkyveApp.Domain.Systems;
public interface ICompatibilityManager
{
	ICompatibilityInfo GetCompatibilityInfo(IPackage package, bool noCache = false);
	IPackageIdentity GetFinalSuccessor(IPackageIdentity item);
	NotificationType GetNotification(ICompatibilityInfo info);
	IPackageCompatibilityInfo? GetPackageInfo(IPackage package);
	bool IsBlacklisted(IPackage package);
	bool IsSnoozed(ICompatibilityItem reportItem);
	void LoadCachedData();
	void ResetCache();
	void ResetSnoozes();
	void ToggleSnoozed(ICompatibilityItem reportItem);
}
