#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.ComponentModel;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Web {
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class WebApplicationStyleManager {
		private const string IsNewStyleKey = "IsNewStyleKey";
		private const string NewTemplatesThemeNameKey = "TemplatesV2Images";
		private const string LockSwitchNewStyleKey = "LockSwitchNewStyle";
		private const string EnableUpperCaseKey = "EnableUpperCaseKey";
		private const string GroupUpperCaseKey = "GroupUCKey";
		private const string GridColumnsUpperCaseKey = "GridCUCKey";
		private const string NavigationGroupsUpperCaseKey = "NavGUCKey";
		private static bool disableCheckValueManager = false;
		internal static void SwitchToNewStyle(bool isNew) {
			if(!LockSwitchNewStyle) {
				TemplateContentFactory.Instance.NewStyle = isNew;
				IsNewStyle = isNew;
				EnableUpperCase = isNew;
				IValueManager<string> manager = ValueManager.GetValueManager<string>(ImageLoader.ThemeImagesFolderKey);
				manager.Value = isNew ? NewTemplatesThemeNameKey : "";
			}
		}
		private static void CheckValueManager() {
			if(!disableCheckValueManager) {
				if(WebApplication.Instance == null) {
					throw new InvalidOperationException("The WebApplication is not initialized");
				}
				else {
					if(ValueManager.ValueManagerType == null) {
						throw new InvalidOperationException("The ValueManagerType is not initialized");
					}
				}
			}
		}
		public static bool IsNewStyle {
			get {
				IValueManager<bool?> manager = ValueManager.GetValueManager<bool?>(IsNewStyleKey);
				return manager.Value.HasValue ? manager.Value.Value : false;
			}
			private set {
				CheckValueManager();
				IValueManager<bool?> manager = ValueManager.GetValueManager<bool?>(IsNewStyleKey);
				manager.Value = value;
			}
		}
		public static bool EnableUpperCase {
			get {
				IValueManager<bool?> manager = ValueManager.GetValueManager<bool?>(EnableUpperCaseKey);
				return manager.Value.HasValue ? manager.Value.Value : false;
			}
			set {
				CheckValueManager();
				IValueManager<bool?> manager = ValueManager.GetValueManager<bool?>(EnableUpperCaseKey);
				manager.Value = value;
				GroupUpperCase = value;
				GridColumnsUpperCase = value;
				NavigationGroupsUpperCase = value;
			}
		}
		public static bool GroupUpperCase {
			get {
				IValueManager<bool?> manager = ValueManager.GetValueManager<bool?>(GroupUpperCaseKey);
				return manager.Value.HasValue ? manager.Value.Value : false;
			}
			set {
				CheckValueManager();
				IValueManager<bool?> manager = ValueManager.GetValueManager<bool?>(GroupUpperCaseKey);
				manager.Value = value;
			}
		}
		public static bool GridColumnsUpperCase {
			get {
				IValueManager<bool?> manager = ValueManager.GetValueManager<bool?>(GridColumnsUpperCaseKey);
				return manager.Value.HasValue ? manager.Value.Value : false;
			}
			set {
				CheckValueManager();
				IValueManager<bool?> manager = ValueManager.GetValueManager<bool?>(GridColumnsUpperCaseKey);
				manager.Value = value;
			}
		}
		public static bool NavigationGroupsUpperCase {
			get {
				IValueManager<bool?> manager = ValueManager.GetValueManager<bool?>(NavigationGroupsUpperCaseKey);
				return manager.Value.HasValue ? manager.Value.Value : false;
			}
			set {
				CheckValueManager();
				IValueManager<bool?> manager = ValueManager.GetValueManager<bool?>(NavigationGroupsUpperCaseKey);
				manager.Value = value;
			}
		}
		internal static bool LockSwitchNewStyle {
			get {
				IValueManager<bool?> manager = ValueManager.GetValueManager<bool?>(LockSwitchNewStyleKey);
				return manager.Value.HasValue ? manager.Value.Value : false;
			}
			set {
				IValueManager<bool?> manager = ValueManager.GetValueManager<bool?>(LockSwitchNewStyleKey);
				manager.Value = value;
			}
		}
#if DebugTest
		public static void DebugTest_SwitchToNewStyle(bool resetLockSwitchNewStyle) {
			disableCheckValueManager = true;
			if(resetLockSwitchNewStyle) {
				LockSwitchNewStyle = false;
			}
			SwitchToNewStyle(true);
		}
		public static void DebugTest_SwitchToNewStyle() {
			disableCheckValueManager = true;
			DebugTest_SwitchToNewStyle(true);
		}
		public static void DebugTest_ResetIsNewStyle(bool resetLockSwitchNewStyle) {
			disableCheckValueManager = true;
			if(resetLockSwitchNewStyle) {
				LockSwitchNewStyle = false;
			}
			SwitchToNewStyle(false);
		}
		public static void DebugTest_ResetIsNewStyle() {
			disableCheckValueManager = true;
			DebugTest_ResetIsNewStyle(true);
		}
		public static bool DebugTest_LockSwitchNewStyle {
			get {
				return LockSwitchNewStyle;
			}
			set {
				disableCheckValueManager = true;
				LockSwitchNewStyle = value;
			}
		}
#endif
	}
}
