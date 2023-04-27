﻿using Extensions;

using LoadOrderToolTwo.Domain;
using LoadOrderToolTwo.Domain.Enums;
using LoadOrderToolTwo.Domain.Interfaces;
using LoadOrderToolTwo.Domain.Steam;
using LoadOrderToolTwo.UserInterface.Panels;
using LoadOrderToolTwo.Utilities;
using LoadOrderToolTwo.Utilities.IO;
using LoadOrderToolTwo.Utilities.Managers;

using SlickControls;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using static CompatibilityReport.CatalogData.Enums;

namespace LoadOrderToolTwo.UserInterface.Lists;
internal class ItemListControl<T> : SlickStackedListControl<T> where T : IPackage
{
	private PackageSorting sorting;
	private readonly Dictionary<DrawableItem<T>, Rectangles> _itemRects = new();

	public event Action<ReportSeverity>? CompatibilityReportSelected;
	public event Action<DownloadStatus>? DownloadStatusSelected;
	public event Action<DateTime>? DateSelected;
	public event Action<TagItem>? TagSelected;
	public event Action<SteamUser>? AuthorSelected;
	public event Action<bool>? FilterByIncluded;
	public event Action<bool>? FilterByEnabled;
	public event Action<string>? AddToSearch;
	public event EventHandler? FilterRequested;

	public ItemListControl()
	{
		HighlightOnHover = true;
		SeparateWithLines = true;

		if (!CentralManager.IsContentLoaded)
		{
			Loading = true;

			CentralManager.ContentLoaded += () => Loading = false;
		}

		sorting = CentralManager.SessionSettings.UserSettings.PackageSorting;
		SortDesc = CentralManager.SessionSettings.UserSettings.PackageSortingDesc;
	}

	public IEnumerable<T> FilteredItems => SafeGetItems().Select(x => x.Item);

	public int FilteredCount => SafeGetItems().Count;

	public bool SortDesc { get; private set; }

	protected override void UIChanged()
	{
		ItemHeight = CentralManager.SessionSettings.UserSettings.LargeItemOnHover ? 64 : 36;

		base.UIChanged();

		Padding = UI.Scale(new Padding(3, 2, 3, 2), UI.FontScale);
	}

	public override void FilterChanged()
	{
		if (!IsHandleCreated)
		{
			base.FilterChanged();
		}
		else
		{
			FilterRequested?.Invoke(this, EventArgs.Empty);
		}
	}

	internal void DoFilterChanged()
	{
		base.FilterChanged();
	}

	public void SetSorting(PackageSorting packageSorting, bool desc)
	{
		if (sorting == packageSorting && SortDesc == desc)
		{
			return;
		}

		SortDesc = desc;
		sorting = packageSorting;

		if (!IsHandleCreated)
		{
			SortingChanged();
		}
		else
		{
			new BackgroundAction(SortingChanged).Run();
		}
	}

	protected override IEnumerable<DrawableItem<T>> OrderItems(IEnumerable<DrawableItem<T>> items)
	{
		items = sorting switch
		{
			PackageSorting.FileSize => items
				.OrderBy(x => x.Item.FileSize),

			PackageSorting.Name => items
				.OrderBy(x => x.Item.ToString()),

			PackageSorting.Author => items
				.OrderBy(x => x.Item.Author?.Name ?? string.Empty),

			PackageSorting.Status => items
				.OrderBy(x => x.Item.Status),

			PackageSorting.UpdateTime => items
				.OrderBy(x => x.Item.ServerTime.If(DateTime.MinValue, x.Item.LocalTime)),

			PackageSorting.SubscribeTime => items
				.OrderBy(x => x.Item.SubscribeTime),

			PackageSorting.Mod => items
				.OrderBy(x => Path.GetFileName(x.Item.Package.Mod?.FileName ?? string.Empty)),

			PackageSorting.CompatibilityReport => items
				.OrderBy(x => x.Item.Package.CompatibilityReport?.Severity ?? default),

			_ => items
				.OrderByDescending(x => x.Item.IsIncluded)
				.ThenByDescending(x => x.Item.Workshop)
				.ThenBy(x => x.Item.ToString())
		};

		if (SortDesc)
		{
			return items.Reverse();
		}

		return items;
	}

	protected override bool IsItemActionHovered(DrawableItem<T> item, Point location)
	{
		var rects = _itemRects.TryGet(item);

		if (rects is null)
		{
			return false;
		}

		if (item.Item.Package.Mod is not null)
		{
			if (rects.IncludedRect.Contains(location))
			{
				setTip($"{Locale.ExcludeInclude}\r\n\r\n{string.Format(Locale.ControlClickTo, Locale.FilterByThisIncludedStatus.ToLower())}", rects.IncludedRect);
			}

			if (rects.EnabledRect.Contains(location))
			{
				setTip($"{Locale.EnableDisable}\r\n\r\n{string.Format(Locale.ControlClickTo, Locale.FilterByThisEnabledStatus.ToLower())}", rects.EnabledRect);
			}

			if (rects.VersionRect.Contains(location))
			{
				setTip(Locale.CopyVersionNumber, rects.VersionRect);
			}
		}
		else
		{
			if (rects.IncludedRect.Contains(location))
			{
				setTip($"{Locale.ExcludeInclude}\r\n\r\n{string.Format(Locale.ControlClickTo, Locale.FilterByThisIncludedStatus.ToLower())}", rects.IncludedRect);
			}
		}

		if (rects.CenterRect.Contains(location) || rects.IconRect.Contains(location))
		{
			setTip(Locale.OpenPackagePage, rects.CenterRect);
		}

		if (rects.FolderRect.Contains(location))
		{
			setTip(Locale.OpenLocalFolder, rects.FolderRect);
		}

		if (rects.CompatibilityRect.Contains(location))
		{
			setTipFilter(Locale.ViewPackageCR, Locale.FilterByThisReportStatus, rects.CompatibilityRect);
		}

		if (rects.DownloadStatusRect.Contains(location))
		{
			setTipFilter(null, Locale.FilterByThisPackageStatus, rects.DownloadStatusRect);
		}

		if (rects.DateRect.Contains(location))
		{
			var date = item.Item.ServerTime.If(DateTime.MinValue, item.Item.LocalTime).ToLocalTime();
			setTipFilter(string.Format(Locale.CopyToClipboard, date.ToString("g")), Locale.FilterSinceThisDate, rects.DateRect);
		}

		foreach (var tag in rects.TagRects)
		{
			if (tag.Value.Contains(location))
			{
				setTipFilter(string.Format(Locale.CopyToClipboard, tag.Key), string.Format(Locale.FilterByThisTag, tag.Key), tag.Value);
			}
		}

		if (item.Item.Workshop)
		{
			if (rects.SteamRect.Contains(location))
			{
				setTip(Locale.ViewOnSteam, rects.SteamRect);
			}

			if (rects.SteamIdRect.Contains(location))
			{
				setTipFilter(string.Format(Locale.CopyToClipboard, item.Item.SteamId), string.Format(Locale.AddToSearch, item.Item.SteamId), rects.SteamIdRect);
			}

			if (rects.AuthorRect.Contains(location))
			{
				setTipFilter(Locale.OpenAuthorPage, Locale.FilterByThisAuthor, rects.AuthorRect);
			}
		}

		else if (rects.SteamIdRect.Contains(location))
		{
			var folder = Path.GetFileName(item.Item.Folder);
			setTipFilter(string.Format(Locale.CopyToClipboard, folder), string.Format(Locale.AddToSearch, folder), rects.SteamIdRect);
		}

		void setTip(string text, Rectangle rectangle) => SlickTip.SetTo(this, text, offset: new Point(rectangle.X, item.Bounds.Y));

		void setTipFilter(string? text, string? alt, Rectangle rectangle)
		{
			var tip = string.Empty;

			if (CentralManager.SessionSettings.UserSettings.FlipItemCopyFilterAction)
			{
				ExtensionClass.Swap(ref text, ref alt);
			}

			if (text is not null)
			{
				tip += text + "\r\n\r\n";
			}

			if (alt is not null)
			{
				tip += string.Format(Locale.ControlClickTo, alt.ToLower());
			}

			SlickTip.SetTo(this, tip.Trim(), offset: new Point(rectangle.X, item.Bounds.Y));
		}

		return rects.Contain(location);
	}

	protected override void OnItemMouseClick(DrawableItem<T> item, MouseEventArgs e)
	{
		base.OnItemMouseClick(item, e);

		if (e.Button == MouseButtons.Right)
		{
			ShowRightClickMenu(item.Item);
			return;
		}

		if (e.Button != MouseButtons.Left)
		{
			return;
		}

		var rects = _itemRects.TryGet(item);
		var filter = ModifierKeys.HasFlag(Keys.Control) != CentralManager.SessionSettings.UserSettings.FlipItemCopyFilterAction;

		if (item.Item.Package.Mod is Mod mod)
		{
			if (rects.IncludedRect.Contains(e.Location))
			{
				if (ModifierKeys.HasFlag(Keys.Control))
				{
					FilterByIncluded?.Invoke(mod.IsIncluded);
				}
				else
				{
					mod.IsIncluded = !mod.IsIncluded;
				}

				return;
			}

			if (rects.EnabledRect.Contains(e.Location))
			{
				if (ModifierKeys.HasFlag(Keys.Control))
				{
					FilterByEnabled?.Invoke(mod.IsEnabled);
				}
				else
				{
					mod.IsEnabled = !mod.IsEnabled;
				}

				return;
			}

			if (rects.VersionRect.Contains(e.Location))
			{
				Clipboard.SetText(item.Item.Package.Mod.Version.GetString());
			}
		}
		else
		{
			if (rects.IncludedRect.Contains(e.Location))
			{
				if (ModifierKeys.HasFlag(Keys.Control))
				{
					FilterByIncluded?.Invoke(item.Item.IsIncluded);
				}
				else
				{
					item.Item.IsIncluded = !item.Item.IsIncluded;
				}

				return;
			}
		}

		if (rects.CenterRect.Contains(e.Location) || rects.IconRect.Contains(e.Location))
		{
			(FindForm() as BasePanelForm)?.PushPanel(null, new PC_PackagePage(item.Item.Package));

			if (CentralManager.SessionSettings.UserSettings.ResetScrollOnPackageClick)
			{
				ScrollTo(item.Item);
			}

			return;
		}

		if (rects.FolderRect.Contains(e.Location))
		{
			OpenFolder(item.Item);
			return;
		}

		if (rects.CompatibilityRect.Contains(e.Location))
		{
			if (filter)
			{
				CompatibilityReportSelected?.Invoke(item.Item.Package.CompatibilityReport?.Severity ?? ReportSeverity.NothingToReport);
			}
			else
			{
				var pc = new PC_PackagePage(item.Item.Package);
				(FindForm() as BasePanelForm)?.PushPanel(null, pc);

				pc.T_CR.Selected = true;

				if (CentralManager.SessionSettings.UserSettings.ResetScrollOnPackageClick)
				{
					ScrollTo(item.Item);
				}
			}
			return;
		}

		if (rects.DownloadStatusRect.Contains(e.Location))
		{
			if (filter)
			{
				DownloadStatusSelected?.Invoke(item.Item.Status);
			}

			return;
		}

		if (rects.DateRect.Contains(e.Location))
		{
			var date = item.Item.ServerTime.If(DateTime.MinValue, item.Item.LocalTime).ToLocalTime();
			if (filter)
			{
				DateSelected?.Invoke(date);
			}
			else
			{
				Clipboard.SetText(date.ToString("g"));
			}
			return;
		}

		foreach (var tag in rects.TagRects)
		{
			if (tag.Value.Contains(e.Location))
			{
				if (filter)
				{
					TagSelected?.Invoke(tag.Key);
				}
				else
				{
					Clipboard.SetText(tag.Key.Value);
				}

				return;
			}
		}

		if (item.Item.Workshop)
		{
			if (rects.SteamRect.Contains(e.Location))
			{
				OpenSteamLink(item.Item.SteamPage);
				return;
			}

			if (rects.SteamIdRect.Contains(e.Location))
			{
				if (filter)
				{
					AddToSearch?.Invoke(item.Item.SteamId.ToString());
				}
				else
				{
					Clipboard.SetText(item.Item.SteamId.ToString());
				}

				return;
			}

			if (rects.AuthorRect.Contains(e.Location) && item.Item.Author is not null)
			{
				if (filter)
				{
					AuthorSelected?.Invoke(item.Item.Author);
				}
				else
				{
					OpenSteamLink($"{item.Item.Author.ProfileUrl}myworkshopfiles");
				}

				return;
			}
		}

		if (rects.SteamIdRect.Contains(e.Location))
		{
			if (filter)
			{
				AddToSearch?.Invoke(Path.GetFileName(item.Item.Folder));
			}
			else
			{
				Clipboard.SetText(Path.GetFileName(item.Item.Folder));
			}

			return;
		}
	}

	private void ShowRightClickMenu(T item)
	{
		var isPackageIncluded = item.Package.IsIncluded;

		var items = new SlickStripItem[]
		{
			  new (Locale.IncludeAllItemsInThisPackage, "I_Ok", !isPackageIncluded, action: () => { item.Package.IsIncluded = true; Invalidate(); })
			, new (Locale.ExcludeAllItemsInThisPackage, "I_Cancel", isPackageIncluded, action: () => { item.Package.IsIncluded = false; Invalidate(); })
			, new (Locale.ReDownloadPackage, "I_ReDownload", SteamUtil.IsSteamAvailable(), action: () => Redownload(item))
			, new (Locale.MovePackageToLocalFolder, "I_PC",item.Workshop, action: () => ContentUtil.MoveToLocalFolder(item))
			, new (string.Empty)
			, new (!item.Workshop && item is Asset ? Locale.DeleteAsset : Locale.DeletePackage, "I_Disposable", !item.BuiltIn, action: () => AskThenDelete(item))
			, new (Locale.UnsubscribePackage, "I_Steam", item.Workshop && !item.BuiltIn, action: async () => await CitiesManager.Subscribe(new[] { item.SteamId }, true))
			, new (string.Empty)
			, new (Locale.OtherProfiles, "I_ProfileSettings", fade: true)
			, new (Locale.IncludeThisItemInAllProfiles, "I_Ok", tab: 1, action: () => { new BackgroundAction(() => ProfileManager.SetIncludedForAll(item, true)).Run(); item.IsIncluded = true; Invalidate(); })
			, new (Locale.ExcludeThisItemInAllProfiles, "I_Cancel", tab: 1, action: () => { new BackgroundAction(() => ProfileManager.SetIncludedForAll(item, false)).Run(); item.IsIncluded = false; Invalidate(); })
			, new (Locale.Copy, "I_Copy", item.Workshop, fade: true)
			, new (Locale.CopyPackageName, item.Workshop ? null : "I_Copy", tab: item.Workshop ? 1 : 0, action: () => Clipboard.SetText(item.ToString()))
			, new (Locale.CopyWorkshopLink, null, item.Workshop, tab: 1, action: () => Clipboard.SetText(item.SteamPage))
			, new (Locale.CopyWorkshopId, null, item.Workshop, tab: 1,  action: () => Clipboard.SetText(item.SteamId.ToString()))
			, new (string.Empty, show: item.Workshop, tab: 1)
			, new (Locale.CopyAuthorName, null, item.Workshop, tab: 1, action: () => Clipboard.SetText(item.Author?.Name))
			, new (Locale.CopyAuthorLink, null, item.Workshop, tab: 1, action: () => Clipboard.SetText($"{item.Author?.ProfileUrl}myworkshopfiles"))
			, new (Locale.CopyAuthorId, null, item.Workshop, tab: 1, action: () => Clipboard.SetText(item.Author?.ProfileUrl.Split(new[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries).Last()))
			, new (Locale.CopyAuthorSteamId, null, item.Workshop, tab: 1,  action: () => Clipboard.SetText(item.Author?.SteamId))
		};

		this.TryBeginInvoke(() => SlickToolStrip.Show(Program.MainForm, items));
	}

	private void AskThenDelete(T item)
	{
		if (MessagePrompt.Show(Locale.AreYouSure + "\r\n\r\n" + Locale.ActionUnreversible, PromptButtons.YesNo, form: Program.MainForm) == DialogResult.Yes)
		{
			try
			{
				if (!item.Workshop && item is Asset asset)
				{
					ExtensionClass.DeleteFile(asset.FileName);
				}
				else
				{
					ContentUtil.DeleteAll(item.Folder);
				}
			}
			catch (Exception ex) { MessagePrompt.Show(ex, Locale.FailedToDeleteItem); }
		}
	}

	private void Redownload(T item)
	{
		SteamUtil.ReDownload(item);
	}

	private void OpenSteamLink(string? url)
	{
		PlatformUtil.OpenUrl(url);
	}

	private void OpenFolder(T item)
	{
		try
		{
			if (item is Asset asset)
			{
				PlatformUtil.OpenFolder(asset.FileName);
			}
			else
			{
				PlatformUtil.OpenFolder(item.Folder);
			}
		}
		catch { }
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		try
		{
			if (Loading)
			{
				base.OnPaint(e);
			}
			else if (!Items.Any())
			{
				e.Graphics.DrawString(Locale.NoLocalPackagesFound + "\r\n" + Locale.CheckFolderInOptions, UI.Font(9.75F, FontStyle.Italic), new SolidBrush(ForeColor), ClientRectangle, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
			}
			else if (!SafeGetItems().Any())
			{
				e.Graphics.DrawString(Locale.NoPackagesMatchFilters, UI.Font(9.75F, FontStyle.Italic), new SolidBrush(ForeColor), ClientRectangle, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
			}
			else
			{
				base.OnPaint(e);
			}
		}
		catch { }
	}

	protected override void OnPaintItem(ItemPaintEventArgs<T> e)
	{
		var large = CentralManager.SessionSettings.UserSettings.LargeItemOnHover;
		var rects = _itemRects[e.DrawableItem] = GetActionRectangles(e.Graphics, e.ClipRectangle, e.Item, large);
		var inclEnableRect = (rects.EnabledRect == Rectangle.Empty ? rects.IncludedRect : Rectangle.Union(rects.IncludedRect, rects.EnabledRect)).Pad(0, Padding.Vertical, 0, Padding.Vertical);
		var isPressed = e.HoverState.HasFlag(HoverState.Pressed);
		var labelRect = new Rectangle(rects.TextRect.X, rects.CenterRect.Bottom, 0, e.ClipRectangle.Bottom - rects.CenterRect.Bottom);

		if (isPressed && !rects.CenterRect.Contains(CursorLocation) && !rects.IconRect.Contains(CursorLocation))
		{
			e.HoverState &= ~HoverState.Pressed;
		}

		base.OnPaintItem(e);

		var partialIncluded = e.Item is Package package && package.IsPartiallyIncluded();
		var isIncluded = partialIncluded || e.Item.IsIncluded;

		PaintIncludedButton(e, rects, inclEnableRect, isIncluded, partialIncluded, large);

		DrawThumbnailAndTitle(e, rects, large);

		var isVersion = e.Item.Package.Mod is not null && !e.Item.Package.Mod.BuiltIn;
		var versionText = isVersion ? "v" + e.Item.Package.Mod!.Version.GetString() : e.Item.Package.Mod?.BuiltIn ?? false ? Locale.Vanilla : e.Item.FileSize.SizeString();
		rects.VersionRect = DrawLabel(e, versionText, null, isVersion ? FormDesign.Design.YellowColor : FormDesign.Design.YellowColor.MergeColor(FormDesign.Design.BackColor, 40), labelRect, large ? ContentAlignment.TopLeft : ContentAlignment.BottomLeft, isVersion);
		labelRect.X += Padding.Left + rects.VersionRect.Width;

		var date = (e.Item.ServerTime == DateTime.MinValue ? e.Item.LocalTime : e.Item.ServerTime).ToLocalTime();
		var dateText = CentralManager.SessionSettings.UserSettings.ShowDatesRelatively ? date.ToRelatedString(true, false) : date.ToString("g");
		rects.DateRect = DrawLabel(e, dateText, IconManager.GetSmallIcon("I_UpdateTime"), FormDesign.Design.AccentColor.MergeColor(FormDesign.Design.BackColor, 50), labelRect, large ? ContentAlignment.TopLeft : ContentAlignment.BottomLeft, true);
		labelRect.X += Padding.Left + rects.DateRect.Width;

		if (!large || e.Item.Workshop)
		{
			labelRect = DrawStatusDescriptor(e, rects, labelRect, ContentAlignment.TopLeft);
		}

		DrawAuthorAndSteamId(e, large, rects);

		var report = e.Item.Package.CompatibilityReport;
		if (report is not null && report.Severity != ReportSeverity.NothingToReport)
		{
			var labelColor = report.Severity switch
			{
				ReportSeverity.MinorIssues => FormDesign.Design.YellowColor,
				ReportSeverity.MajorIssues => FormDesign.Design.YellowColor.MergeColor(FormDesign.Design.RedColor),
				ReportSeverity.Unsubscribe => FormDesign.Design.RedColor,
				ReportSeverity.Remarks => FormDesign.Design.ButtonColor,
				_ => FormDesign.Design.GreenColor.MergeColor(FormDesign.Design.AccentColor, 20)
			};

			rects.CompatibilityRect = DrawLabel(e, LocaleHelper.GetGlobalText($"CR_{report.Severity}"), IconManager.GetSmallIcon("I_CompatibilityReport"), labelColor, labelRect, large ? ContentAlignment.TopLeft : ContentAlignment.BottomLeft, true);

			labelRect.X += Padding.Left + rects.CompatibilityRect.Width;
		}
		else
		{
			rects.CompatibilityRect = Rectangle.Empty;
		}

		DrawButtons(e, rects, isPressed);

		if (large)
		{
			labelRect.X = rects.TextRect.X;
		}

		if (large && !e.Item.Workshop)
		{
			labelRect = DrawStatusDescriptor(e, rects, labelRect, ContentAlignment.BottomLeft);
		}

		foreach (var item in e.Item.Tags.Distinct(x => x.Value))
		{
			using var tagIcon = IconManager.GetSmallIcon(item.Source switch { TagSource.Workshop => "I_Steam", TagSource.FindIt => "I_Search", _ => "I_Tag" });

			var tagRect = DrawLabel(e, item.Value, tagIcon, FormDesign.Design.ButtonColor, labelRect, ContentAlignment.BottomLeft, true);

			rects.TagRects[item] = tagRect;

			labelRect.X += Padding.Left + tagRect.Width;
		}

		if (!isIncluded) // fade excluded item
		{
			using var fadedBrush = new SolidBrush(Color.FromArgb(e.HoverState.HasFlag(HoverState.Hovered) ? 25 : 75, BackColor));
			var filledRect = e.ClipRectangle.Pad(0, -Padding.Top, 0, -Padding.Bottom);

			e.Graphics.SetClip(filledRect);
			e.Graphics.FillRectangle(fadedBrush, filledRect);
		}
	}

	private Rectangle DrawStatusDescriptor(ItemPaintEventArgs<T> e, ItemListControl<T>.Rectangles rects, Rectangle labelRect, ContentAlignment contentAlignment)
	{
		GetStatusDescriptors(e.Item, out var text, out var icon, out var color);

		if (!string.IsNullOrEmpty(text))
		{
			using (icon)
			{
				rects.DownloadStatusRect = DrawLabel(e, text, icon, color.MergeColor(FormDesign.Design.BackColor, 65), labelRect, contentAlignment, true);
			}

			labelRect.X += Padding.Left + rects.DownloadStatusRect.Width;
		}
		else
		{
			rects.DownloadStatusRect = Rectangle.Empty;
		}

		return labelRect;
	}

	private void DrawButtons(ItemPaintEventArgs<T> e, ItemListControl<T>.Rectangles rects, bool isPressed)
	{
		using (var icon = IconManager.GetIcon("I_Folder", rects.FolderRect.Height / 2))
		{
			SlickButton.DrawButton(e, rects.FolderRect, string.Empty, Font, icon, null, rects.FolderRect.Contains(CursorLocation) ? e.HoverState | (isPressed ? HoverState.Pressed : 0) : HoverState.Normal);
		}

		if (e.Item.Workshop)
		{
			using (var icon = IconManager.GetIcon("I_Steam", rects.SteamRect.Height / 2))
			{
				SlickButton.DrawButton(e, rects.SteamRect, string.Empty, Font, icon, null, rects.SteamRect.Contains(CursorLocation) ? e.HoverState | (isPressed ? HoverState.Pressed : 0) : HoverState.Normal);
			}
		}
	}

	private void DrawAuthorAndSteamId(ItemPaintEventArgs<T> e, bool large, ItemListControl<T>.Rectangles rects)
	{
		if (!e.Item.Workshop)
		{
			rects.SteamIdRect = DrawLabel(e, Path.GetFileName(e.Item.Folder), IconManager.GetSmallIcon("I_Folder"), FormDesign.Design.ActiveColor.MergeColor(FormDesign.Design.ButtonColor, 30), rects.SteamIdRect, large ? ContentAlignment.MiddleRight : ContentAlignment.BottomRight, true);
			rects.AuthorRect = Rectangle.Empty;
			return;
		}

		if (large && e.Item.Author is not null)
		{
			using var font = UI.Font(9.75F);
			var size = e.Graphics.Measure(e.Item.Author.Name, font).ToSize();
			var authorRect = rects.AuthorRect.Align(new Size(size.Width + Padding.Horizontal + Padding.Right + size.Height, size.Height + (Padding.Vertical * 2)), ContentAlignment.MiddleRight);
			authorRect.X -= Padding.Left;
			authorRect.Y += Padding.Top;
			var avatarRect = authorRect.Pad(Padding).Align(new(size.Height, size.Height), ContentAlignment.MiddleLeft);

			using var brush = new SolidBrush(FormDesign.Design.BackColor.Tint(Lum: FormDesign.Design.Type.If(FormDesignType.Dark, 4, -4)).MergeColor(FormDesign.Design.ActiveColor, authorRect.Contains(CursorLocation) ? 65 : 100));
			e.Graphics.FillRoundedRectangle(brush, authorRect, (int)(4 * UI.FontScale));

			using var brush1 = new SolidBrush(FormDesign.Design.ForeColor);
			e.Graphics.DrawString(e.Item.Author.Name, font, brush1, authorRect.Pad(avatarRect.Width + Padding.Horizontal, 0, 0, 0), new StringFormat { LineAlignment = StringAlignment.Center });

			var authorImg = e.Item.AuthorIconImage;

			if (authorImg is null)
			{
				using var authorIcon = Properties.Resources.I_AuthorIcon.Color(FormDesign.Design.IconColor);

				e.Graphics.DrawRoundImage(authorIcon, avatarRect);
			}
			else
			{
				e.Graphics.DrawRoundImage(authorImg, avatarRect);
			}

			rects.AuthorRect = authorRect;
		}
		else
		{
			rects.AuthorRect = DrawLabel(e, e.Item.Author?.Name, IconManager.GetSmallIcon("I_Developer"), FormDesign.Design.ActiveColor.MergeColor(FormDesign.Design.ButtonColor, 30), rects.AuthorRect, ContentAlignment.TopLeft, true);
		}

		rects.SteamIdRect = DrawLabel(e, e.Item.SteamId.ToString(), IconManager.GetSmallIcon("I_Steam"), FormDesign.Design.ActiveColor.MergeColor(FormDesign.Design.ButtonColor, 30), rects.SteamIdRect, large ? ContentAlignment.MiddleRight : ContentAlignment.BottomLeft, true);
	}

	private void DrawThumbnailAndTitle(ItemPaintEventArgs<T> e, ItemListControl<T>.Rectangles rects, bool large)
	{
		var iconRectangle = rects.IconRect;

		var iconImg = e.Item.IconImage;

		if (iconImg is null)
		{
			using var generic = (e.Item is Package ? Properties.Resources.I_CollectionIcon : e.Item is Asset ? Properties.Resources.I_AssetIcon : Properties.Resources.I_ModIcon).Color(FormDesign.Design.IconColor);

			e.Graphics.DrawRoundedImage(generic, iconRectangle, (int)(4 * UI.FontScale), FormDesign.Design.AccentBackColor);
		}
		else
		{
			try
			{ e.Graphics.DrawRoundedImage(iconImg, iconRectangle, (int)(4 * UI.FontScale), FormDesign.Design.AccentBackColor); }
			catch { }
		}

		List<string>? tags = null;

		var mod = e.Item.Package.Mod is not null;
		var text = mod ? e.Item.ToString().RemoveVersionText(out tags) : e.Item.ToString();
		using var font = UI.Font(large ? 11.25F : 9F, FontStyle.Bold);
		var textSize = e.Graphics.Measure(text, font);

		using var brush = new SolidBrush(e.HoverState.HasFlag(HoverState.Pressed) ? FormDesign.Design.ActiveForeColor : (rects.CenterRect.Contains(CursorLocation) || rects.IconRect.Contains(CursorLocation)) && e.HoverState.HasFlag(HoverState.Hovered) ? FormDesign.Design.ActiveColor : base.ForeColor);
		e.Graphics.DrawString(text, font, brush, rects.TextRect, new StringFormat { Trimming = StringTrimming.EllipsisCharacter });

		if (tags is null)
		{
			return;
		}

		var tagRect = new Rectangle(rects.TextRect.X + (int)textSize.Width, rects.TextRect.Y, 1, (int)textSize.Height);

		foreach (var item in tags)
		{
			if (item.ToLower() == "stable")
			{ continue; }

			var color = item.ToLower() switch
			{
				"alpha" or "experimental" => Color.FromArgb(200, FormDesign.Design.YellowColor.MergeColor(FormDesign.Design.RedColor)),
				"beta" or "test" or "testing" => Color.FromArgb(180, FormDesign.Design.YellowColor),
				"deprecated" => Color.FromArgb(225, FormDesign.Design.RedColor),
				_ => (Color?)null
			};

			tagRect.X += Padding.Left + DrawLabel(e, color is null ? item : LocaleHelper.GetGlobalText(item.ToUpper()), null, color ?? FormDesign.Design.ButtonColor, tagRect, ContentAlignment.MiddleLeft, smaller: true).Width;
		}
	}

	private void PaintIncludedButton(ItemPaintEventArgs<T> e, ItemListControl<T>.Rectangles rects, Rectangle inclEnableRect, bool isIncluded, bool partialIncluded, bool large)
	{
		var incl = new DynamicIcon(partialIncluded ? "I_Slash" : isIncluded ? "I_Ok" : "I_Enabled");
		if (CentralManager.SessionSettings.UserSettings.AdvancedIncludeEnable && e.Item.Package.Mod is Mod mod)
		{
			var enabl = new DynamicIcon(mod.IsEnabled ? "I_Checked" : "I_Checked_OFF");
			if (isIncluded)
			{
				using var brush = inclEnableRect.Gradient(Color.FromArgb(inclEnableRect.Contains(CursorLocation) ? 150 : 255, partialIncluded ? FormDesign.Design.YellowColor : mod.IsEnabled ? FormDesign.Design.GreenColor : FormDesign.Design.RedColor), 1.5F);
				e.Graphics.FillRoundedRectangle(brush, inclEnableRect, 4);
			}
			else if (mod.IsEnabled)
			{
				using var brush = inclEnableRect.Gradient(Color.FromArgb(inclEnableRect.Contains(CursorLocation) ? 150 : 255, FormDesign.Design.YellowColor), 1.5F);
				e.Graphics.FillRoundedRectangle(brush, inclEnableRect, 4);
			}
			else if (inclEnableRect.Contains(CursorLocation))
			{
				using var brush = inclEnableRect.Gradient(Color.FromArgb(20, ForeColor), 1.5F);
				e.Graphics.FillRoundedRectangle(brush, inclEnableRect, 4);
			}

			using var includedIcon = (large ? incl.Large : incl.Get(rects.IncludedRect.Height / 2)).Color(rects.IncludedRect.Contains(CursorLocation) ? FormDesign.Design.ActiveColor : isIncluded ? FormDesign.Design.ActiveForeColor : ForeColor);
			using var enabledIcon = (large ? enabl.Large : enabl.Get(rects.IncludedRect.Height / 2)).Color(rects.EnabledRect.Contains(CursorLocation) ? FormDesign.Design.ActiveColor : isIncluded ? FormDesign.Design.ActiveForeColor : base.ForeColor);
			e.Graphics.DrawImage(includedIcon, rects.IncludedRect.CenterR(includedIcon.Size));
			e.Graphics.DrawImage(enabledIcon, rects.EnabledRect.CenterR(enabledIcon.Size));
		}
		else
		{
			if (isIncluded)
			{
				using var brush = inclEnableRect.Gradient(Color.FromArgb(inclEnableRect.Contains(CursorLocation) ? 150 : 255, partialIncluded ? FormDesign.Design.YellowColor : FormDesign.Design.GreenColor), 1.5F);
				e.Graphics.FillRoundedRectangle(brush, inclEnableRect, 4);
			}
			else if (inclEnableRect.Contains(CursorLocation))
			{
				using var brush = inclEnableRect.Gradient(Color.FromArgb(20, ForeColor), 1.5F);
				e.Graphics.FillRoundedRectangle(brush, inclEnableRect, 4);
			}

			using var icon = (large ? incl.Large : incl.Get(rects.IncludedRect.Height / 2)).Color(rects.IncludedRect.Contains(CursorLocation) ? FormDesign.Design.ActiveColor : isIncluded ? FormDesign.Design.ActiveForeColor : ForeColor);
			e.Graphics.DrawImage(icon, inclEnableRect.CenterR(icon.Size));
		}
	}

	private Rectangle DrawLabel(ItemPaintEventArgs<T> e, string? text, Bitmap? icon, Color color, Rectangle rectangle, ContentAlignment alignment, bool action = false, bool smaller = false)
	{
		if (text == null)
		{
			return Rectangle.Empty;
		}

		var large = CentralManager.SessionSettings.UserSettings.LargeItemOnHover;
		using var font = UI.Font((large ? 9F : 7.5F) - (smaller ? 0.5F : 0F));
		var size = e.Graphics.Measure(text, font).ToSize();

		if (icon is not null)
		{
			size.Width += icon.Width + Padding.Left;
		}

		size.Width += Padding.Left;

		rectangle = rectangle.Pad(smaller ? Padding.Left / 2 : Padding.Left).Align(size, alignment);

		if (action && !rectangle.Contains(CursorLocation))
		{
			color = color.MergeColor(FormDesign.Design.BackColor, 50);
		}

		using var backBrush = rectangle.Gradient(color);
		using var foreBrush = new SolidBrush(color.GetTextColor());

		e.Graphics.FillRoundedRectangle(backBrush, rectangle, (int)(3 * UI.FontScale));
		e.Graphics.DrawString(text, font, foreBrush, icon is null ? rectangle : rectangle.Pad(icon.Width + (Padding.Left * 2) - 2, 0, 0, 0), new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

		if (icon is not null)
		{
			e.Graphics.DrawImage(icon.Color(color.GetTextColor()), rectangle.Pad(Padding.Left, 0, 0, 0).Align(icon.Size, ContentAlignment.MiddleLeft));
		}

		return rectangle;
	}

	private void GetStatusDescriptors(T mod, out string text, out Bitmap? icon, out Color color)
	{
		if (!mod.Workshop)
		{
			text = Locale.Local;
			icon = IconManager.GetSmallIcon("I_PC");
			color = FormDesign.Design.YellowColor.MergeColor(FormDesign.Design.AccentColor);
			return;
		}

		switch (mod.Status)
		{
			case DownloadStatus.OK:
				break;
			//text = Locale.UpToDate;
			//icon = Properties.Resources.I_Ok_16;
			//color = FormDesign.Design.GreenColor.MergeColor(FormDesign.Design.AccentColor, 20);
			//return;
			case DownloadStatus.Unknown:
				text = Locale.StatusUnknown;
				icon = IconManager.GetSmallIcon("I_Question");
				color = FormDesign.Design.YellowColor;
				return;
			case DownloadStatus.OutOfDate:
				text = Locale.OutOfDate;
				icon = IconManager.GetSmallIcon("I_OutOfDate");
				color = FormDesign.Design.YellowColor;
				return;
			case DownloadStatus.NotDownloaded:
				text = Locale.ModIsNotDownloaded;
				icon = IconManager.GetSmallIcon("I_Question");
				color = FormDesign.Design.RedColor;
				return;
			case DownloadStatus.PartiallyDownloaded:
				text = Locale.PartiallyDownloaded;
				icon = IconManager.GetSmallIcon("I_Broken");
				color = FormDesign.Design.RedColor;
				return;
			case DownloadStatus.Removed:
				text = Locale.ModIsRemoved;
				icon = IconManager.GetSmallIcon("I_ContentRemoved");
				color = FormDesign.Design.RedColor;
				return;
		}

		text = string.Empty;
		icon = null;
		color = Color.White;
	}

	private Rectangles GetActionRectangles(Graphics g, Rectangle rectangle, T item, bool doubleSize)
	{
		var rects = new Rectangles() { Item = item };
		var includeItemHeight = doubleSize ? (ItemHeight / 2) : ItemHeight;

		if (CentralManager.SessionSettings.UserSettings.AdvancedIncludeEnable && item.Package.Mod is not null)
		{
			rects.IncludedRect = rectangle.Pad(1 * Padding.Left, 0, 0, 0).Align(new Size(includeItemHeight * 9 / 10, rectangle.Height), ContentAlignment.MiddleLeft);
			rects.EnabledRect = rects.IncludedRect.Pad(rects.IncludedRect.Width, 0, -rects.IncludedRect.Width, 0);
		}
		else if (item is Package)
		{
			rects.IncludedRect = rectangle.Pad(1 * Padding.Left, 0, 0, 0).Align(new Size(CentralManager.SessionSettings.UserSettings.AdvancedIncludeEnable ? (includeItemHeight * 2 * 9 / 10) : includeItemHeight + 1, rectangle.Height), ContentAlignment.MiddleLeft);
		}
		else
		{
			rects.IncludedRect = rectangle.Pad(1 * Padding.Left, 0, 0, 0).Align(new Size(includeItemHeight + 1, rectangle.Height), ContentAlignment.MiddleLeft);
		}

		var buttonRectangle = rectangle.Pad(0, 0, Padding.Right, 0).Align(new Size(ItemHeight / (doubleSize ? 2 : 1), ItemHeight / (doubleSize ? 2 : 1)), ContentAlignment.TopRight);
		var iconSize = rectangle.Height - Padding.Vertical;

		rects.FolderRect = buttonRectangle;
		rects.IconRect = rectangle.Pad(Math.Max(rects.IncludedRect.Right, rects.EnabledRect.Right) + (2 * Padding.Left)).Align(new Size(iconSize, iconSize), ContentAlignment.MiddleLeft);
		rects.TextRect = rectangle.Pad(rects.IconRect.X + rects.IconRect.Width + Padding.Left, 0, (item.Workshop ? (2 * Padding.Left) + (2 * buttonRectangle.Width) + (int)(100 * UI.FontScale) : 0) + rectangle.Width - buttonRectangle.X, rectangle.Height / 2);

		if (item.Workshop)
		{
			buttonRectangle.X -= Padding.Left + buttonRectangle.Width;
			rects.SteamRect = buttonRectangle;
		}

		if (doubleSize)
		{
			rects.SteamIdRect = new Rectangle(buttonRectangle.X - (int)(100 * UI.FontScale), rectangle.Y, (int)(100 * UI.FontScale), rectangle.Height / 2);
			rects.AuthorRect = new Rectangle(rectangle.X, rectangle.Y + (rectangle.Height / 2), rectangle.Width, rectangle.Height / 2);
			rects.CenterRect = new Rectangle(rects.IconRect.X - 1, rectangle.Y, rects.SteamIdRect.X - rects.IconRect.X, (int)g.Measure(" ", UI.Font(11.25F, FontStyle.Bold)).Height + Padding.Bottom);
		}
		else
		{
			rects.AuthorRect = new Rectangle(buttonRectangle.X - (int)(100 * UI.FontScale), rectangle.Y + (rectangle.Height / 2), (int)(100 * UI.FontScale), rectangle.Height / 2);
			rects.SteamIdRect = new Rectangle(buttonRectangle.X - (int)(100 * UI.FontScale), rectangle.Y, (int)(100 * UI.FontScale), rectangle.Height / 2);
			rects.CenterRect = new Rectangle(rects.IconRect.X - 1, rectangle.Y, rects.SteamIdRect.X - rects.IconRect.X, rectangle.Height / 2);
		}

		//}
		//else
		//{
		//	rects.CenterRect = new Rectangle(rects.IconRect.X - 1, rectangle.Y, buttonRectangle.X - rects.IconRect.X, rectangle.Height / 2);
		//}

		return rects;
	}

	private class Rectangles
	{
		internal T? Item;

		internal Dictionary<TagItem, Rectangle> TagRects = new();
		internal Rectangle IncludedRect;
		internal Rectangle EnabledRect;
		internal Rectangle FolderRect;
		internal Rectangle IconRect;
		internal Rectangle TextRect;
		internal Rectangle SteamRect;
		internal Rectangle SteamIdRect;
		internal Rectangle CenterRect;
		internal Rectangle AuthorRect;
		internal Rectangle VersionRect;
		internal Rectangle CompatibilityRect;
		internal Rectangle DownloadStatusRect;
		internal Rectangle DateRect;

		internal bool Contain(Point location)
		{
			return
				IncludedRect.Contains(location) ||
				EnabledRect.Contains(location) ||
				FolderRect.Contains(location) ||
				CenterRect.Contains(location) ||
				SteamRect.Contains(location) ||
				AuthorRect.Contains(location) ||
				IconRect.Contains(location) ||
				DownloadStatusRect.Contains(location) ||
				CompatibilityRect.Contains(location) ||
				DateRect.Contains(location) ||
				(VersionRect.Contains(location) && Item?.Package.Mod is not null) ||
				TagRects.Any(x => x.Value.Contains(location)) ||
				SteamIdRect.Contains(location);
		}
	}
}
