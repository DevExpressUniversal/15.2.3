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
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
using DevExpress.Xpf.Core;
using DevExpress.Utils;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Scheduler.Drawing;
using System.ComponentModel;
using System.Collections.ObjectModel;
namespace DevExpress.Xpf.Scheduler.UI {
	[DXToolboxBrowsable, ToolboxTabName(AssemblyInfo.DXTabWpfScheduling)]
	public class TimeZoneEdit : FixedSourceComboBoxEdit {
		public TimeZoneEdit() {
			SetCurrentValue(ValidateOnTextInputProperty, false);
			SetCurrentValue(IsTextEditableProperty, true);
			SetCurrentValue(FilterConditionProperty, DevExpress.Data.Filtering.FilterCondition.Contains);
			SetCurrentValue(ImmediatePopupProperty, true);
			SetCurrentValue(IncrementalFilteringProperty, true);
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("TimeZoneEditTimeZoneId")]
#endif
		public string TimeZoneId {
			get {
				if (EditValue == null)
					return DevExpress.XtraScheduler.TimeZoneId.DateLine;
				else
					return (string)EditValue;
			}
		}
		protected override BaseEditSettings CreateEditorSettings() {
			return new TimeZoneEditSettings();
		}
	}
	public class TimeZoneEditSettings : FixedSourceComboBoxEditSettings {
		static TimeZoneEditSettings() {
			RegisterEditor();
		}
		internal static void RegisterEditor() {
			EditorSettingsProvider.Default.RegisterUserEditor(typeof(TimeZoneEdit), typeof(TimeZoneEditSettings), delegate() { return new TimeZoneEdit(); }, delegate() { return new TimeZoneEditSettings(); });
		}
		protected override void PopulateItems() {
			Items.Clear();
			ReadOnlyCollection<TimeZoneInfo> timeZones = TimeZoneInfo.GetSystemTimeZones();
			int count = timeZones.Count;
			for (int i = 0; i < count; i++) {
				Items.Add(CreateItem(timeZones[i]));
			}
		}
		protected NamedElement CreateItem(TimeZoneInfo timeZone) {
			return new NamedElement(timeZone.Id, timeZone.DisplayName);
		}
	}
}
