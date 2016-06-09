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
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler;
using DevExpress.Xpf.Core;
using DevExpress.Utils;
using DevExpress.Xpf.Scheduler.Drawing;
using System.Windows;
using System.ComponentModel;
namespace DevExpress.Xpf.Scheduler.UI {
	[DXToolboxBrowsable, ToolboxTabName(AssemblyInfo.DXTabWpfScheduling)]
	public class WeekOfMonthEdit : FixedSourceComboBoxEdit {
		#region WeekOfMonth
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("WeekOfMonthEditWeekOfMonth")]
#endif
public WeekOfMonth WeekOfMonth {
			get { return (WeekOfMonth)GetValue(WeekOfMonthProperty); }
			set { SetValue(WeekOfMonthProperty, value); }
		}
		public static readonly DependencyProperty WeekOfMonthProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<WeekOfMonthEdit, WeekOfMonth>("WeekOfMonth", WeekOfMonth.First, (d, e) => d.OnWeekOfMonthChanged(e.OldValue, e.NewValue), null);
		void OnWeekOfMonthChanged(WeekOfMonth oldValue, WeekOfMonth newValue) {
			SelectedItem = Settings.GetItemFromValue(newValue);
		}
		#endregion
		protected override Editors.Settings.BaseEditSettings CreateEditorSettings() {
			return new WeekOfMonthEditSettings();
		}
		protected override void OnSelectedItemChanged(object oldValue, object newValue) {
			base.OnSelectedItemChanged(oldValue, newValue);
			NamedElement element = newValue as NamedElement;
			if(element != null && element.Id != null && element.Id is WeekOfMonth)
				WeekOfMonth = (WeekOfMonth)element.Id;
		}
	}
	public class WeekOfMonthEditSettings: FixedSourceComboBoxEditSettings {
		protected override void PopulateItems() {
			Items.Clear();
			Items.Add(CreateItem(WeekOfMonth.First, SchedulerStringId.Caption_WeekOfMonthFirst));
			Items.Add(CreateItem(WeekOfMonth.Second, SchedulerStringId.Caption_WeekOfMonthSecond));
			Items.Add(CreateItem(WeekOfMonth.Third, SchedulerStringId.Caption_WeekOfMonthThird));
			Items.Add(CreateItem(WeekOfMonth.Fourth, SchedulerStringId.Caption_WeekOfMonthFourth));
			Items.Add(CreateItem(WeekOfMonth.Last, SchedulerStringId.Caption_WeekOfMonthLast));
		}
		protected NamedElement CreateItem(WeekOfMonth val, SchedulerStringId id) {
			string displayName = SchedulerLocalizer.GetString(id);
			return new NamedElement(val, displayName);
		}
	}
}
