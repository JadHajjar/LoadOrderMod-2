using HarmonyLib;

using KianCommons.Patches;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using static KianCommons.ReflectionHelpers;


namespace SkyveMod.Patches.ContentManager;
public static class OnEnableDisableAllPatch
{
	static readonly MethodInfo mEnabledPackageEvents =
		GetMethod(typeof(ContentManagerPanel), "EnablePackageEvents");
	static readonly MethodInfo mFlush =
		new Action(DelayedEventInvoker.Flush).Method;

	static IEnumerable<MethodBase> TargetMethods()
	{
		yield return GetMethod(typeof(ContentManagerPanel), "OnEnableAll");
		yield return GetMethod(typeof(ContentManagerPanel), "OnDisableAll");
	}

	static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
	{
		var codes = instructions.ToCodeList();
		//Log.Debug("before:\n" + codes.IL2STR());

		var index = codes.FindIndex(_c => _c.Calls(mEnabledPackageEvents));

		// 0:replace call ContentManagerPanel.EnablePackageEvents 
		codes.ReplaceInstruction(index, new CodeInstruction(OpCodes.Call, mFlush));

		// -2:remove ldarg0     
		// -1:remove ldc.i4.1
		codes.RemoveRange(index - 2, 2);

		//Log.Debug("after:\n"+codes.IL2STR());
		return codes;
	}
}
