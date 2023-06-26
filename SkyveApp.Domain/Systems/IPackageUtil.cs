﻿using System.Collections.Generic;
using System.Drawing;

namespace SkyveApp.Domain.Systems;
public interface IPackageUtil
{
	string CleanName(IPackage? package, bool keepTags = false);
	string CleanName(IPackage? package, out List<(Color Color, string Text)> tags, bool keepTags = false);
	string GetVersionText(string name);
}
