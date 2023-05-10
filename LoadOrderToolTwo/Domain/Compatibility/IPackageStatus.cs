﻿using System;

namespace LoadOrderToolTwo.Domain.Compatibility;

public interface IPackageStatus<TType> : IGenericPackageStatus where TType : struct, Enum
{
	TType Type { get; set; } 
}

public interface IGenericPackageStatus 
{
	InteractionAction Action { get; set; }
	ulong[]? Packages { get; set; }
	string? Note { get; set; }
#if !API
	NotificationType Notification { get; }
#endif
}
