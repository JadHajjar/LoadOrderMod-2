﻿using SkyveApp.Utilities;

namespace SkyveApp.Domain.Utilities;
internal class CachedModInclusion : CachedSaveItem<Mod, bool>
{
	public CachedModInclusion(Mod key, bool value) : base(key, value)
	{ }

	public override bool CurrentValue => ModsUtil.IsLocallyIncluded(Key);

	protected override void OnSave()
	{
		ModsUtil.SetLocallyIncluded(Key, ValueToSave);
	}
}
