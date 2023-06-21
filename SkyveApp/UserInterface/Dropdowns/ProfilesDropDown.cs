﻿using Extensions;

using SkyveApp.Domain;
using SkyveApp.Services;
using SkyveApp.Utilities;

using SlickControls;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SkyveApp.UserInterface.Dropdowns;
internal class ProfilesDropDown : SlickSelectionDropDown<Profile>
{
	protected override void OnHandleCreated(EventArgs e)
	{
		base.OnHandleCreated(e);

		if (Live)
		{
			Items = ProfileManager.Profiles.ToArray();

			selectedItem = Items[0];
		}
	}

	protected override IEnumerable<Profile> OrderItems(IEnumerable<Profile> items)
	{
		return items.OrderByDescending(x => x.Temporary).ThenByDescending(x => x.LastEditDate);
	}

	protected override void PaintItem(PaintEventArgs e, Rectangle rectangle, Color foreColor, HoverState hoverState, Profile item)
	{
		if (item is null)
		{ return; }

		var text = item.Name;

		if (item.Temporary)
		{
			text = Locale.Unfiltered;
		}

		using var icon = (item.Temporary ? new DynamicIcon("I_Slash") : item.GetIcon()).Get(rectangle.Height - 2).Color(foreColor);

		e.Graphics.DrawImage(icon, rectangle.Align(icon.Size, ContentAlignment.MiddleLeft));

		e.Graphics.DrawString(text, Font, new SolidBrush(foreColor), rectangle.Pad(icon.Width + Padding.Left, 0, 0, 0).AlignToFontSize(Font, ContentAlignment.MiddleLeft, e.Graphics), new StringFormat { Trimming = StringTrimming.EllipsisCharacter });
	}
}
