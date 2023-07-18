﻿using SkyveApp.Domain.CS1;
using SkyveApp.Systems.CS1.Utilities;

using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace SkyveApp.UserInterface.Lists;

internal partial class ItemListControl<T>
{
	protected override void OnPaintItemGrid(ItemPaintEventArgs<T, ItemListControl<T>.Rectangles> e)
	{
		var localPackage = e.Item.LocalPackage;
		var localParentPackage = localPackage?.LocalParentPackage;
		var workshopInfo = e.Item.GetWorkshopInfo();
		var partialIncluded = false;
		var isPressed = false;
		var isIncluded = (localPackage is not null && _packageUtil.IsIncluded(e.Item.LocalPackage!, out partialIncluded)) || partialIncluded;

		if (e.IsSelected)
		{
			e.BackColor = FormDesign.Design.GreenColor.MergeColor(FormDesign.Design.BackColor);
		}

		if (!IsPackagePage && e.HoverState.HasFlag(HoverState.Hovered) && (e.Rects.CenterRect.Contains(CursorLocation) || e.Rects.IconRect.Contains(CursorLocation)))
		{
			e.BackColor = (e.IsSelected ? e.BackColor : FormDesign.Design.AccentBackColor).MergeColor(FormDesign.Design.ActiveColor, e.HoverState.HasFlag(HoverState.Pressed) ? 0 : 90);

			isPressed = e.HoverState.HasFlag(HoverState.Pressed);
		}

		base.OnPaintItemGrid(e);

		DrawThumbnail(e);
		DrawTitleAndTagsAndVersion(e, localParentPackage, workshopInfo, isPressed);
		DrawIncludedButton(e, e.Rects, isIncluded, partialIncluded, true, localParentPackage, out var activeColor);

		var scoreX = DrawScore(e, workshopInfo);

		if (workshopInfo?.Author is not null)
		{
			DrawAuthor(e, workshopInfo.Author, scoreX);
		}
		else if (e.Item.IsLocal)
		{
			DrawFolderName(e, localParentPackage!, scoreX);
		}

		DrawDividerLine(e);

		var maxTagX = DrawButtons(e, isPressed, localParentPackage);

		DrawTags(e, maxTagX);

		e.Graphics.ResetClip();

		DrawCompatibilityAndStatus(e, out var outerColor);

		if (isIncluded)
		{
			if (outerColor == default)
			{
				outerColor = Color.FromArgb(FormDesign.Design.Type == FormDesignType.Dark ? 65 : 100, activeColor);
			}

			using var pen = new Pen(outerColor, (float)(1.5 * UI.FontScale));

			e.Graphics.DrawRoundedRectangle(pen, e.ClipRectangle.InvertPad(GridPadding - new Padding((int)pen.Width)), (int)(5 * UI.FontScale));
		}
		else if (localPackage is not null)
		{
			using var brush = new SolidBrush(Color.FromArgb(85, BackColor));
			e.Graphics.FillRectangle(brush, e.ClipRectangle.InvertPad(GridPadding));
		}
	}

	private void DrawCompatibilityAndStatus(ItemPaintEventArgs<T, ItemListControl<T>.Rectangles> e, out Color outerColor)
	{
		var compatibilityReport = e.Item.GetCompatibilityInfo();
		var notificationType = compatibilityReport?.GetNotification();
		outerColor = default;

		var height = e.Rects.IconRect.Bottom - Math.Max(e.Rects.TextRect.Bottom, Math.Max(e.Rects.VersionRect.Bottom, e.Rects.DateRect.Bottom)) - GridPadding.Bottom;
		if (notificationType > NotificationType.Info)
		{
			outerColor = notificationType.Value.GetColor();

			e.Rects.CompatibilityRect = e.Graphics.DrawLargeLabel(new(e.ClipRectangle.Right, e.Rects.IconRect.Bottom), LocaleCR.Get($"{notificationType}"), "I_CompatibilityReport", outerColor, ContentAlignment.BottomRight, padding: GridPadding, height: height, cursorLocation: CursorLocation);
		}

		if (GetStatusDescriptors(e.Item, out var text, out var icon, out var color))
		{
			if (!(notificationType > NotificationType.Info))
			{
				outerColor = color;
			}

			e.Rects.DownloadStatusRect = e.Graphics.DrawLargeLabel(new(notificationType > NotificationType.Info ? (e.Rects.CompatibilityRect.X - GridPadding.Left) : e.ClipRectangle.Right, e.Rects.IconRect.Bottom), notificationType > NotificationType.Info ? "" : text, icon!, color, ContentAlignment.BottomRight, padding: GridPadding, height: height, cursorLocation: CursorLocation);
		}

		if (e.IsSelected && outerColor == default)
		{
			outerColor = FormDesign.Design.GreenColor;
		}
	}

	private void DrawTags(ItemPaintEventArgs<T, ItemListControl<T>.Rectangles> e, int maxTagX)
	{
		var tagsRect = new Rectangle(e.ClipRectangle.X, e.Rects.IconRect.Bottom + (GridPadding.Vertical * 2), 0, 0);

		e.Graphics.SetClip(new Rectangle(tagsRect.X, tagsRect.Y, maxTagX - tagsRect.X, e.ClipRectangle.Bottom - tagsRect.Y));

		if (e.Item.Id > 0)
		{
			e.Rects.SteamIdRect = DrawTag(e, maxTagX, ref tagsRect, new TagItem(Domain.CS1.Enums.TagSource.Workshop, e.Item.Id.ToString()));
		}

		foreach (var item in e.Item.GetTags(IsPackagePage))
		{
			DrawTag(e, maxTagX, ref tagsRect, item);
		}

		var seamRect = new Rectangle(maxTagX - (int)(50 * UI.UIScale), (int)e.Graphics.ClipBounds.Y, (int)(50 * UI.UIScale), (int)e.Graphics.ClipBounds.Height);
		using var seamBrush = new LinearGradientBrush(seamRect, Color.Empty, e.BackColor, 0F);

		e.Graphics.FillRectangle(seamBrush, seamRect);
	}

	private void DrawDividerLine(ItemPaintEventArgs<T, ItemListControl<T>.Rectangles> e)
	{
		var lineRect = new Rectangle(e.ClipRectangle.X, e.Rects.IconRect.Bottom + GridPadding.Vertical, e.ClipRectangle.Width, (int)(2 * UI.FontScale));
		using var lineBrush = new LinearGradientBrush(lineRect, default, default, 0F);

		lineBrush.InterpolationColors = new ColorBlend
		{
			Colors = new[] { Color.Empty, FormDesign.Design.AccentColor, FormDesign.Design.AccentColor, Color.Empty, Color.Empty },
			Positions = new[] { 0.0f, 0.15f, 0.6f, 0.75f, 1f }
		};

		e.Graphics.FillRectangle(lineBrush, lineRect);
	}

	private Rectangle DrawTag(ItemPaintEventArgs<T, ItemListControl<T>.Rectangles> e, int maxTagX, ref Rectangle tagsRect, ITag item)
	{
		using var tagIcon = IconManager.GetSmallIcon(item.Icon);

		var tagRect = e.Graphics.DrawLabel(item.Value, tagIcon, Color.FromArgb(200, FormDesign.Design.LabelColor.MergeColor(FormDesign.Design.AccentBackColor, 40)), tagsRect, ContentAlignment.TopLeft, large: true, mousePosition: CursorLocation);

		e.Rects.TagRects[item] = tagRect;

		tagsRect.X += GridPadding.Left + tagRect.Width;

		if (tagsRect.X + (int)(70 * UI.FontScale) > maxTagX)
		{
			tagsRect.X = e.ClipRectangle.X;
			tagsRect.Y += tagRect.Height + GridPadding.Top;
		}

		return tagRect;
	}

	private int DrawButtons(ItemPaintEventArgs<T, Rectangles> e, bool isPressed, ILocalPackageWithContents? package)
	{
		var padding = GridView ? GridPadding : Padding;
		var size = UI.Scale(new Size(28, 28), UI.FontScale);
		var rect = new Rectangle(e.ClipRectangle.Right - size.Width - (GridView ? 0 : Padding.Right), e.ClipRectangle.Bottom - size.Height, size.Width, size.Height);

		if (package is not null)
		{
			using var icon = IconManager.GetIcon("I_Folder", size.Height * 3 / 4);

			SlickButton.DrawButton(e, rect, string.Empty, Font, icon, null, rect.Contains(CursorLocation) ? e.HoverState | (isPressed ? HoverState.Pressed : 0) : HoverState.Normal, backColor: Color.FromArgb(175, FormDesign.Design.BackColor));

			e.Rects.FolderRect = rect;

			rect.X -= rect.Width + padding.Left;
		}

		if (e.Item.GetWorkshopInfo()?.Url is not null)
		{
			using var icon = IconManager.GetIcon("I_Steam", rect.Height * 3 / 4);

			SlickButton.DrawButton(e, rect, string.Empty, Font, icon, null, rect.Contains(CursorLocation) ? e.HoverState | (isPressed ? HoverState.Pressed : 0) : HoverState.Normal, backColor: Color.FromArgb(175, FormDesign.Design.BackColor));

			e.Rects.SteamRect = rect;

			rect.X -= rect.Width + padding.Left;
		}

		if (_compatibilityManager.GetPackageInfo(e.Item)?.Links?.FirstOrDefault(x => x.Type == LinkType.Github) is ILink gitLink)
		{
			using var icon = IconManager.GetIcon("I_Github", rect.Height * 3 / 4);

			SlickButton.DrawButton(e, rect, string.Empty, Font, icon, null, rect.Contains(CursorLocation) ? e.HoverState | (isPressed ? HoverState.Pressed : 0) : HoverState.Normal, backColor: Color.FromArgb(175, FormDesign.Design.BackColor));

			e.Rects.GithubRect = rect;

			rect.X -= rect.Width + padding.Left;
		}

		return rect.X + rect.Width;
	}

	private void DrawFolderName(ItemPaintEventArgs<T, ItemListControl<T>.Rectangles> e, ILocalPackageWithContents package, int scoreX)
	{
		var padding = GridView ? GridPadding : Padding;
		var height = e.Rects.IconRect.Bottom - Math.Max(e.Rects.TextRect.Bottom, Math.Max(e.Rects.VersionRect.Bottom, e.Rects.DateRect.Bottom)) - padding.Bottom;
		var folderPoint = new Point(e.Rects.TextRect.X + scoreX, e.Rects.IconRect.Bottom);

		e.Rects.SteamIdRect = e.Graphics.DrawLargeLabel(folderPoint, Path.GetFileName(package.Folder), "I_Folder", alignment: ContentAlignment.BottomLeft, padding: GridView ? GridPadding : Padding, height: height, cursorLocation: CursorLocation);
	}

	private void DrawAuthor(ItemPaintEventArgs<T, ItemListControl<T>.Rectangles> e, IUser author, int scoreX)
	{
		var padding = GridView ? GridPadding : Padding;
		var authorRect = new Rectangle(e.Rects.TextRect.X + scoreX, e.Rects.IconRect.Bottom, 0, 0);
		var authorImg = author.GetUserAvatar();

		var height = e.Rects.IconRect.Bottom - Math.Max(e.Rects.TextRect.Bottom, Math.Max(e.Rects.VersionRect.Bottom, e.Rects.DateRect.Bottom)) - padding.Bottom;

		if (authorImg is null)
		{
			using var authorIcon = Properties.Resources.I_AuthorIcon.Color(FormDesign.Design.IconColor);

			authorRect = e.Graphics.DrawLargeLabel(authorRect.Location, author.Name, authorIcon, alignment: ContentAlignment.BottomLeft, padding: padding, height: height, cursorLocation: CursorLocation);
		}
		else
		{
			authorRect = e.Graphics.DrawLargeLabel(authorRect.Location, author.Name, authorImg, alignment: ContentAlignment.BottomLeft, padding: padding, height: height, cursorLocation: CursorLocation);
		}

		if (_compatibilityManager.IsUserVerified(author))
		{
			var avatarRect = authorRect.Pad(padding).Align(new(authorRect.Height * 3 / 4, authorRect.Height * 3 / 4), ContentAlignment.MiddleLeft);
			var checkRect = avatarRect.Align(new Size(avatarRect.Height / 3, avatarRect.Height / 3), ContentAlignment.BottomRight);

			e.Graphics.FillEllipse(new SolidBrush(FormDesign.Design.GreenColor), checkRect.Pad(-(int)(2 * UI.FontScale)));

			using var img = IconManager.GetIcon("I_Check", checkRect.Height);

			e.Graphics.DrawImage(img.Color(Color.White), checkRect.Pad(0, 0, -1, -1));
		}

		e.Rects.AuthorRect = authorRect;
	}

	private void DrawTitleAndTagsAndVersion(ItemPaintEventArgs<T, ItemListControl<T>.Rectangles> e, ILocalPackageWithContents? localParentPackage, IWorkshopInfo? workshopInfo, bool isPressed)
	{
		using var font = UI.Font(GridView ? 10.5F : CompactList ? 8.25F : 9F, FontStyle.Bold);
		var mod = e.Item is not IAsset;
		var tags = new List<(Color Color, string Text)>();
		var text = mod ? e.Item.CleanName(out tags) : e.Item.ToString();
		using var brush = new SolidBrush(isPressed ? FormDesign.Design.ActiveForeColor : (e.Rects.CenterRect.Contains(CursorLocation) || e.Rects.IconRect.Contains(CursorLocation)) && e.HoverState.HasFlag(HoverState.Hovered) && !IsPackagePage ? FormDesign.Design.ActiveColor : ForeColor);
		e.Graphics.DrawString(text, font, brush, e.Rects.TextRect, new StringFormat { Trimming = StringTrimming.EllipsisCharacter });

		var isVersion = localParentPackage?.Mod is not null && !e.Item.IsBuiltIn && !IsPackagePage;
		var versionText = isVersion ? "v" + localParentPackage!.Mod!.Version.GetString() : e.Item.IsBuiltIn ? Locale.Vanilla : (e.Item is ILocalPackage lp ? lp.LocalSize.SizeString() : workshopInfo?.ServerSize.SizeString());
		var date = workshopInfo?.ServerTime ?? e.Item.LocalParentPackage?.LocalTime;

		if (!string.IsNullOrEmpty(versionText))
		{
			tags.Insert(0, (isVersion ? FormDesign.Design.YellowColor : FormDesign.Design.YellowColor.MergeColor(FormDesign.Design.AccentBackColor, 40), versionText!));
		}

		var padding = GridView ? GridPadding : Padding;
		var tagRect = new Rectangle(e.Rects.TextRect.X, e.Rects.TextRect.Bottom + padding.Bottom, 0, 0);

		for (var i = 0; i < tags.Count; i++)
		{
			var rect = e.Graphics.DrawLabel(tags[i].Text, null, tags[i].Color, tagRect, ContentAlignment.TopLeft, smaller: true, mousePosition: i == 0 && localParentPackage?.Mod is not null ? CursorLocation : null);

			if (i == 0 && !string.IsNullOrEmpty(versionText))
			{
				e.Rects.VersionRect = rect;
			}

			tagRect.X += padding.Left + rect.Width;
		}

		if (date.HasValue && !IsPackagePage)
		{
			var dateText = _settings.UserSettings.ShowDatesRelatively ? date.Value.ToRelatedString(true, false) : date.Value.ToString("g");

			e.Rects.DateRect = e.Graphics.DrawLabel(dateText, IconManager.GetSmallIcon("I_UpdateTime"), FormDesign.Design.AccentColor, tagRect, ContentAlignment.TopLeft, smaller: true, mousePosition: CursorLocation);
		}
	}

	private void DrawThumbnail(ItemPaintEventArgs<T, ItemListControl<T>.Rectangles> e)
	{
		var thumbnail = e.Item.GetThumbnail();

		if (thumbnail is null)
		{
			using var generic = (e.Item is ILocalPackageWithContents ? Properties.Resources.I_CollectionIcon : e.Item.IsMod ? Properties.Resources.I_ModIcon : Properties.Resources.I_AssetIcon).Color(FormDesign.Design.IconColor);

			drawThumbnail(generic);
		}
		else if (e.Item.IsLocal)
		{
			using var unsatImg = new Bitmap(thumbnail, e.Rects.IconRect.Size).Tint(Sat: 0);

			drawThumbnail(unsatImg);
		}
		else
		{
			drawThumbnail(thumbnail);
		}

		void drawThumbnail(Bitmap generic) => e.Graphics.DrawRoundedImage(generic, e.Rects.IconRect, (int)(5 * UI.FontScale), FormDesign.Design.BackColor);
	}

	private ItemListControl<T>.Rectangles GenerateGridRectangles(T item, Rectangle rectangle)
	{
		var rects = new Rectangles(item)
		{
			IconRect = rectangle.Align(UI.Scale(new Size(64, 64), UI.UIScale), ContentAlignment.TopLeft)
		};

		rects.TextRect = rectangle.Pad(rects.IconRect.Width + GridPadding.Left, 0, 0, rectangle.Height).AlignToFontSize(UI.Font(10.5F, FontStyle.Bold), ContentAlignment.TopLeft);

		rects.IncludedRect = rects.TextRect.Align(new Size(rects.TextRect.Height, rects.TextRect.Height), ContentAlignment.TopRight);

		if (_settings.UserSettings.AdvancedIncludeEnable && item.LocalParentPackage?.Mod is not null)
		{
			rects.EnabledRect = rects.IncludedRect;

			rects.IncludedRect.X -= rects.IncludedRect.Width;
		}

		rects.TextRect.Width = rects.IncludedRect.X - rects.TextRect.X;

		rects.CenterRect = rects.TextRect;

		return rects;
	}
}
