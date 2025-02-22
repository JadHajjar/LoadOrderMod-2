using ColossalFramework;
using ColossalFramework.UI;

using KianCommons;

using SkyveMod.Settings;

using UnityEngine;

using static KianCommons.ReflectionHelpers;

namespace SkyveMod.UI;
public class FloatingMonoStatus : UILabel
{
	public static FloatingMonoStatus Instance { get; private set; }
	public static float SavedX
	{
		get => ConfigUtil.Config.StatusX;
		set
		{
			ConfigUtil.Config.StatusX = value;
			ConfigUtil.SaveThread.Dirty = true;
		}
	}

	public static float SavedY
	{
		get => ConfigUtil.Config.StatusY;
		set
		{
			if (SavedY != value)
			{
				ConfigUtil.Config.StatusY = value;
				ConfigUtil.SaveThread.Dirty = true;
			}
		}
	}

	private UIDragHandle drag_ { get; set; }

	#region Life Cycle
	public override void Awake()
	{
		base.Awake();
		textColor = debug ? new Color32(196, 24, 49, 255) : new Color32(132, 205, 245, 255);
		autoSize = true;
		isVisible = Helpers.InStartupMenu;
		objectUserData = 1; //refcount
	}

	bool started_ = false;
	internal bool debug;

	public override void Start()
	{
		LogCalled();
		base.Start();
		Instance = this;
		absolutePosition = new Vector3((UIView.GetAView().fixedWidth - width) / 2, SavedY + 10);
		tooltip = "Controlled by the Skyve App";
		SetupDrag();
		if (isVisible = Helpers.InStartupMenu)
		{
			var chirperSprite = UIView.GetAView().FindUIComponent<UISprite>("Chirper");
			chirperSprite.parent.AttachUIComponent(gameObject);
			relativePosition = new Vector3((chirperSprite.parent.width - width) / 2, 20);
			drag_.enabled = false;
		}
		started_ = true;
	}

	#endregion

	public void SetupDrag()
	{
		var dragHandler = new GameObject("UnifiedUI_FloatingButton_DragHandler");
		dragHandler.transform.parent = transform;
		dragHandler.transform.localPosition = Vector3.zero;
		drag_ = dragHandler.AddComponent<UIDragHandle>();

		drag_.width = width;
		drag_.height = height;
		drag_.enabled = true;
	}

	protected override void OnPositionChanged()
	{
		base.OnPositionChanged();
		Log.DebugWait(ThisMethod + " called",
			id: "OnPositionChanged called".GetHashCode(), seconds: 0.2f, copyToGameLog: false);

		if (!started_)
		{
			return;
		}

		var resolution = GetUIView().GetScreenResolution();

		absolutePosition = new Vector2(
			Mathf.Clamp(absolutePosition.x, 0, resolution.x - width),
			Mathf.Clamp(absolutePosition.y, 0, resolution.y - height));

		var delta = new Vector2(absolutePosition.x - SavedX, absolutePosition.y - SavedY);

		Log.DebugWait(message: "absolutePosition: " + absolutePosition,
			id: "absolutePosition: ".GetHashCode(), seconds: 0.2f, copyToGameLog: false);
	}

	protected override void OnSizeChanged()
	{
		base.OnSizeChanged();
		if (!started_)
		{
			return;
		}

		drag_.width = width;
		drag_.height = height;
	}

	protected override void OnMouseUp(UIMouseEventParameter p)
	{
		base.OnMouseUp(p);
		SavedX = absolutePosition.x;
		SavedY = absolutePosition.y;
	}

	public override string text
	{
		get => base.text;
		set
		{
			base.text = value;
			isVisible = !text.IsNullOrWhiteSpace();
		}
	}
}