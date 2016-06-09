#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
{                                                                   }
{       Copyright (c) 2000-2015 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2015 Developer Express Inc.

using System;
using DevExpress.Utils;
#if ASP
using System.Web.UI;
using System.Web.Configuration;
using System.Web.UI.WebControls;
#else
using DevExpress.vNext;
using DevExpress.vNext.Internal;
#endif
namespace DevExpress.Web.Internal {
	public delegate IStateManager GetStateManagerObject(object obj, bool create);
	public class ViewStateUtils {
		public static IStateManager[] GetMergedStateManagedObjects(IStateManager[] objects1, IStateManager[] objects2) {
			IStateManager[] res = new IStateManager[objects1.Length + objects2.Length];
			Array.Copy(objects1, res, objects1.Length);
			Array.Copy(objects2, 0, res, objects1.Length, objects2.Length);
			return res;
		}
		public static IStateManager[] MergedBaseAndNewStateManagedObjects(IStateManager[] baseObjects, params IStateManager[] newObjects) {
			int count = baseObjects.Length;
			for(int i = 0; i < newObjects.Length; i++) {
				if(newObjects[i] != null) count++;
			}
			IStateManager[] res = new IStateManager[count];
			Array.Copy(baseObjects, res, baseObjects.Length);
			int index = baseObjects.Length;
			for(int i = 0; i < newObjects.Length; i++) {
				if(newObjects[i] != null) {
					res[index++] = newObjects[i];
				}
			}
			return res;
		}
		public static bool IsStateArrayEmpty(object[] stateArray) {
			for (int i = 0; i < stateArray.Length; i++) {
				if (stateArray[i] != null)
					return false;
			}
			return true;
		}
		public static void LoadViewState(object savedState, IStateManager baseStateManager, IStateManager[] objects) {
			if (savedState != null) {
				object[] stateArray = savedState as object[];
				object baseState = (stateArray.Length > 0 && stateArray[0] != null) ? stateArray[0] : null;
				baseStateManager.LoadViewState(baseState);
				LoadObjectsViewState(stateArray, objects);
			}
		}
		public static void LoadViewState(object obj, object savedState, IStateManager baseStateManager, GetStateManagerObject[] delegates) {
			if(savedState != null) {
				object[] stateArray = savedState as object[];
				object baseState = (stateArray.Length > 0 && stateArray[0] != null) ? stateArray[0] : null;
				baseStateManager.LoadViewState(baseState);
				LoadObjectsViewState(obj, stateArray, delegates);
			}
		}
		public static void LoadObjectsViewState(object obj, object[] stateArray, GetStateManagerObject[] delegates) {
			if (delegates != null) {
				for (int i = 0; i < delegates.Length; i++) {
					if ((stateArray.Length > (i + 1)) && (stateArray[i + 1] != null) && delegates[i] != null) {
						IStateManager stateManager = delegates[i](obj, true);
						stateManager.LoadViewState(stateArray[i + 1]);
						if (stateManager is IStateManagerTracker)
							((IStateManagerTracker)stateManager).ViewStateLoaded();
					}
				}
			}
		}
		public static void LoadObjectsViewState(object[] stateArray, IStateManager[] objects) {
			if (objects != null) {
				for (int i = 0; i < objects.Length; i++) {
					if ((stateArray.Length > (i + 1)) && (stateArray[i + 1] != null) && objects[i] != null) {
						objects[i].LoadViewState(stateArray[i + 1]);
						if (objects[i] is IStateManagerTracker)
							((IStateManagerTracker)objects[i]).ViewStateLoaded();
					}
				}
			}
		}
		public static object SaveViewState(object obj, IStateManager baseStateManager, GetStateManagerObject[] delegates) {
			return SaveViewState(obj, baseStateManager.SaveViewState(), delegates);
		}
		public static object SaveViewState(object obj, object baseViewState, GetStateManagerObject[] objects) {
			object[] state = null;
			object currentState = baseViewState;
			if (currentState != null) {
				state = (objects != null) ? new object[objects.Length + 1] : new object[1];
				state[0] = currentState;
			}
			if (objects != null) {
				for (int i = 0; i < objects.Length; i++) {
					IStateManager stateManager = objects[i](obj, false);
					if (stateManager == null)
						continue;
					currentState = stateManager.SaveViewState();
					if (currentState != null) {
						if (state == null)
							state = new object[objects.Length + 1];
						state[i + 1] = currentState;
					}
				}
			}
			return state; 
		}
		public static object SaveViewState(IStateManager baseViewState, IStateManager[] objects) {
			return SaveViewState(baseViewState.SaveViewState(), objects);
		}
		public static object SaveViewState(object baseViewState, IStateManager[] objects) {
			object[] state = null;
			object currentState = baseViewState;
			if (currentState != null) {
				state = (objects != null) ? new object[objects.Length + 1] : new object[1];
				state[0] = currentState;
			}
			if (objects != null) {
				for (int i = 0; i < objects.Length; i++) {
					if (objects[i] == null)
						continue;
					currentState = objects[i].SaveViewState();
					if (currentState != null) {
						if (state == null)
							state = new object[objects.Length + 1];
						state[i + 1] = currentState;
					}
				}
			}
			return state; 
		}
		public static void TrackViewState(object obj, IStateManager baseViewState, GetStateManagerObject[] objects) {
			baseViewState.TrackViewState();
			TrackObjectsViewState(obj, objects);
		}
		public static void TrackObjectsViewState(object obj, GetStateManagerObject[] delegates) {
			if (delegates != null) {
				for (int i = 0; i < delegates.Length; i++) {
					IStateManager stateManager = delegates[i](obj, false);
					if (stateManager != null)
						stateManager.TrackViewState();
				}
			}
		}
		public static void TrackViewState(IStateManager baseViewState, IStateManager[] objects) {
			baseViewState.TrackViewState();
			TrackObjectsViewState(objects);
		}
		public static void TrackObjectsViewState(IStateManager[] objects) {
			if (objects != null) {
				for (int i = 0; i < objects.Length; i++) {
					if (objects[i] != null)
						objects[i].TrackViewState();
				}
			}
		}
		public static void SetPropertiesDirty(object obj, StateBag viewState, GetStateManagerObject[] delegates) {
			if(viewState != null)
				viewState.SetDirty(true);
			if (delegates != null) {
				for (int i = 0; i < delegates.Length; i++) {
					IStateManager stateManager = delegates[i](obj, false);
					if (stateManager is IPropertiesDirtyTracker)
						((IPropertiesDirtyTracker)stateManager).SetPropertiesDirty();
				}
			}
		}
		public static void SetPropertiesDirty(StateBag viewState, IStateManager[] objects) {
			if(viewState != null)
				viewState.SetDirty(true);
			if (objects != null) {
				for (int i = 0; i < objects.Length; i++) {
					if (objects[i] is IPropertiesDirtyTracker)
						((IPropertiesDirtyTracker)objects[i]).SetPropertiesDirty();
				}
			}
		}
		public static object GetObjectProperty(StateBag viewState, string key, object defaultValue) {
			return GetProperty(viewState, key, defaultValue);
		}
		public static void SetObjectProperty(StateBag viewState, string key, object defaultValue, object value) {
			viewState[key] = value;
		}
		public static void SetObjectProperty(StateBag viewState, string key, object value) {
			viewState[key] = value;
		}
		public static object GetEnumProperty(StateBag viewState, string key, object defaultValue) {
			return GetObjectProperty(viewState, key, defaultValue);
		}
		public static void SetEnumProperty(StateBag viewState, string key, object defaultValue, object value) {
			SetObjectProperty(viewState, key, value);
		}
		static object boolTrue = true;
		static object boolFalse = false;
		public static bool GetBoolProperty(StateBag viewState, string key, bool defaultValue) {
			return GetProperty(viewState, key, defaultValue);
		}
		public static void SetBoolProperty(StateBag viewState, string key, bool defaultValue, bool value) {
			SetObjectProperty(viewState, key, value ? boolTrue : boolFalse);
		}
		public static DefaultBoolean GetDefaultBooleanProperty(StateBag viewState, string key, DefaultBoolean defaultValue) {
			return GetProperty(viewState, key, defaultValue);
		}
		public static void SetDefaultBooleanProperty(StateBag viewState, string key, DefaultBoolean defaultValue, DefaultBoolean value) {
			SetObjectProperty(viewState, key, value == DefaultBoolean.Default ? DefaultBoolean.Default : value);
		}
		public static string GetStringProperty(StateBag viewState, string key, string defaultValue) {
			return (string)GetObjectProperty(viewState, key, defaultValue);
		}
		public static void SetStringProperty(StateBag viewState, string key, string defaultValue, string value) {
			SetObjectProperty(viewState, key, value);
		}
		public static char GetCharProperty(StateBag viewState, string key, char defaultValue) {
			return (char)GetObjectProperty(viewState, key, defaultValue);
		}
		public static void SetCharProperty(StateBag viewState, string key, char defaultValue, char value) {
			SetObjectProperty(viewState, key, value);
		}
		static object int0 = 0;
		static object intm1 = -1;
		public static int GetIntProperty(StateBag viewState, string key, int defaultValue) {
			return GetProperty(viewState, key, defaultValue);
		}
		public static void SetIntProperty(StateBag viewState, string key, int defaultValue, int value) {
			SetObjectProperty(viewState, key, value == 0 ? int0 : value == -1 ? intm1 : value);
		}
		static object long0 = 0L;
		static object longm1 = -1L;
		public static long GetLongProperty(StateBag viewState, string key, long defaultValue) {
			return GetProperty(viewState, key, defaultValue);
		}
		public static void SetLongProperty(StateBag viewState, string key, long defaultValue, long value) {
			SetObjectProperty(viewState, key, value == 0L ? long0 : value == -1L ? longm1 : value);
		}
		public static T GetProperty<T>(StateBag viewState, string key, T defaultValue) {
			object value = viewState[key];
			return value == null ? defaultValue : (T)value;
		}
		public static Decimal GetDecimalProperty(StateBag viewState, string key, Decimal defaultValue) {
			return GetProperty(viewState, key, defaultValue);
		}
		public static void SetDecimalProperty(StateBag viewState, string key, Decimal defaultValue, Decimal value) {
			SetObjectProperty(viewState, key, value);
		}
		public static Unit GetUnitProperty(StateBag viewState, string key, Unit defaultValue) {
			object value = viewState[key];
			return value == null ? defaultValue : (Unit)value;
		}
		static object emptyUnit = Unit.Empty;
		public static void SetUnitProperty(StateBag viewState, string key, Unit defaultValue, Unit value) {
			SetObjectProperty(viewState, key, value.IsEmpty ? emptyUnit : value);
		}
		static object emptyColor = System.Drawing.Color.Empty;
		public static System.Drawing.Color GetColorProperty(StateBag viewState, string key, System.Drawing.Color defaultValue) {
			return (System.Drawing.Color)GetObjectProperty(viewState, key, defaultValue.IsEmpty ? emptyColor : defaultValue);
		}
		public static void SetColorProperty(StateBag viewState, string key, System.Drawing.Color defaultValue, System.Drawing.Color value) {
			SetObjectProperty(viewState, key, value.IsEmpty ? emptyColor : value);
		}
	}
}
