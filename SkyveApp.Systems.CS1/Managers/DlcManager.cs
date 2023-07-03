﻿using Extensions;

using SkyveApp.Domain;
using SkyveApp.Domain.CS1.Legacy;
using SkyveApp.Domain.Systems;
using SkyveApp.Systems.CS1.Utilities;

using SkyveShared;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyveApp.Systems.CS1.Managers;
internal class DlcManager : IDlcManager
{
	private readonly DlcConfig _config;

	public IEnumerable<IDlcInfo> Dlcs => SteamUtil.Dlcs;

	public event Action? DlcsLoaded;

    public DlcManager()
	{
		_config = DlcConfig.Deserialize();

		SteamUtil.DLCsLoaded += DlcsLoaded;
    }

    public bool IsAvailable(uint dlcId)
	{
		return _config.AvailableDLCs.Contains(dlcId);
	}

	public bool IsIncluded(IDlcInfo dlc)
	{
		return !_config.RemovedDLCs.Contains(dlc.Id);
	}

	public void SetExcludedDlcs(IEnumerable<uint> dlcs)
	{
		_config.RemovedDLCs = dlcs.ToList();

		_config.Serialize();
	}

	public void SetIncluded(IDlcInfo dlc, bool value)
	{
		if (value)
		{
			_config.RemovedDLCs.Remove(dlc.Id);
		}
		else
		{
			_config.RemovedDLCs.AddIfNotExist(dlc.Id);
		}

		_config.Serialize();
	}

	public List<uint> GetExcludedDlcs()
	{
		return new(_config.RemovedDLCs);
	}
}
