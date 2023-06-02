using ColossalFramework.UI;

using HarmonyLib;

using ICities;

using KianCommons;
using KianCommons.Plugins;

using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;

using static ColossalFramework.Plugins.PluginManager;
using static KianCommons.ReflectionHelpers;

namespace SkyveMod.Util;
public static class HotReloadUtil
{
	public static List<UIComponent> m_Dummies(this OptionsMainPanel optionsMainPanel)
	{
		return GetFieldValue(optionsMainPanel, "m_Dummies") as List<UIComponent>;
	}

	public static UIListBox m_Categories(this OptionsMainPanel optionsMainPanel)
	{
		return GetFieldValue(optionsMainPanel, "m_Categories") as UIListBox;
	}

	public static UITabContainer m_CategoriesContainer(this OptionsMainPanel optionsMainPanel)
	{
		return GetFieldValue(optionsMainPanel, "m_CategoriesContainer") as UITabContainer;
	}


	//static TDelegate CreateDelegate<TDelegate>(object instance, string name) where TDelegate : Delegate {
	//    var method = GetMethod(instance.GetType(), name);
	//    return (TDelegate)Delegate.CreateDelegate(typeof(TDelegate), instance, method);
	//}

	//public static PluginsChangedHandler RefreshPlugins =>
	//    CreateDelegate<PluginsChangedHandler>(optionsMainPanel, "RefreshPlugins");

	//public static PropertyChangedEventHandler<int> OnCategoryChanged =>
	//    CreateDelegate<PropertyChangedEventHandler<int>>(optionsMainPanel, "OnCategoryChanged");


	public static void DropCategory(this OptionsMainPanel optionsMainPanel, string name)
	{
		try
		{
			Log.Called(name);
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}

			var categories = optionsMainPanel.m_Categories();
			var index = (categories.items as IList<string>).IndexOf(name);
			Log.Debug("index = " + index);
			if (index < 0)
			{
				return;
			}

			var selectedIndex = categories.selectedIndex;

			var dummies = optionsMainPanel.m_Dummies();
			var category = dummies.Find(c => c.name == name);
			Log.Debug($"removing category component: " + category);
			dummies.Remove(category);
			GameObject.DestroyImmediate(category);
			categories.items = categories.items.RemoveAt(index);

			categories.selectedIndex = Math.Max(selectedIndex, categories.items.Length - 1);
		}
		catch (Exception ex)
		{
			Log.Exception(ex);
		}
	}


	public static void AddCategory(this OptionsMainPanel optionsMainPanel, PluginInfo p)
	{
		try
		{
			LogCalled();
			if (!p.isEnabled)
			{
				return;
			}

			if (p?.userModInstance is not IUserMod userMod)
			{
				return;
			}

			var name = userMod.Name;
			var mOnSettingsUI = userMod.GetType().GetMethod("OnSettingsUI", BindingFlags.Instance | BindingFlags.Public);
			if (mOnSettingsUI != null)
			{
				Log.Info("Adding category :" + name);
				var category = optionsMainPanel.m_CategoriesContainer().AttachUIComponent(UITemplateManager.GetAsGameObject("OptionsScrollPanelTemplate"));
				category.name = userMod.Name;
				optionsMainPanel.m_Dummies().Add(category);

				mOnSettingsUI.Invoke(userMod, new object[] { new UIHelper(category.Find("ScrollContent")) });
				var categories = optionsMainPanel.m_Categories();
				categories.items = categories.items.AddToArray(name);
				category.zOrder = categories.items.Length - 1;
			}
		}
		catch (Exception ex)
		{
			Log.Exception(ex);

		}
	}

	public static string GetUserModName(this PluginInfo p)
	{
		return p?.GetUserModInstance()?.Name ?? p?.name ?? "<null>";
	}
}
