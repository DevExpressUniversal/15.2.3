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
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using DevExpress.Utils.Controls;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Native;
using DevExpress.Utils;
namespace DevExpress.XtraScheduler.UI {
	#region RepositoryItemDayOfWeek
	[
	UserRepositoryItem("RegisterWeekDaysEdit"),
	System.Runtime.InteropServices.ComVisible(false)
	]
	public class RepositoryItemDayOfWeek : RepositoryItemImageComboBox {
		static RepositoryItemDayOfWeek() { RegisterWeekDaysEdit(); }
		public RepositoryItemDayOfWeek() {
			InitItems();
		}
		public static void RegisterWeekDaysEdit() {
			Bitmap img = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraScheduler.Bitmaps256.weekDaysEdit.bmp", Assembly.GetExecutingAssembly());
			img.MakeTransparent(Color.Magenta);
			EditorClassInfo editorInfo = new EditorClassInfo(typeof(WeekDaysEdit).Name, typeof(WeekDaysEdit), typeof(RepositoryItemDayOfWeek), typeof(DevExpress.XtraEditors.ViewInfo.ImageComboBoxEditViewInfo), new DevExpress.XtraEditors.Drawing.ImageComboBoxEditPainter(), true, img);
			editorInfo.AllowInplaceEditing = ShowInContainerDesigner.OnlyInBars;
			EditorRegistrationInfo.Default.Editors.Add(editorInfo);
		}
		#region Properties
		internal static string InternalEditorTypeName { get { return typeof(WeekDaysEdit).Name; } }
		public override string EditorTypeName { get { return InternalEditorTypeName; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ImageComboBoxItemCollection Items { get { return base.Items; } }
		#endregion
		protected void InitItems() {
			BeginUpdate();
			try {
				Items.Clear();
				AddDayOfWeek(WeekDays.EveryDay);
				AddDayOfWeek(WeekDays.WorkDays);
				AddDayOfWeek(WeekDays.WeekendDays);
				AddDayOfWeek(WeekDays.Sunday);
				AddDayOfWeek(WeekDays.Monday);
				AddDayOfWeek(WeekDays.Tuesday);
				AddDayOfWeek(WeekDays.Wednesday);
				AddDayOfWeek(WeekDays.Thursday);
				AddDayOfWeek(WeekDays.Friday);
				AddDayOfWeek(WeekDays.Saturday);
			}
			finally {
				CancelUpdate();
			}
		}
		void AddDayOfWeek(WeekDays dayOfWeek) {
			Items.Add(new ImageComboBoxItem(GetDisplayName(dayOfWeek), dayOfWeek, -1));
		}
		static string GetDisplayName(WeekDays val) {
			if (val == WeekDays.EveryDay)
				return SchedulerLocalizer.GetString(SchedulerStringId.Caption_WeekDaysEveryDay);
			else if (val == WeekDays.WeekendDays)
				return SchedulerLocalizer.GetString(SchedulerStringId.Caption_WeekDaysWeekendDays);
			else if (val == WeekDays.WorkDays)
				return SchedulerLocalizer.GetString(SchedulerStringId.Caption_WeekDaysWorkDays);
			else {
				DateTimeFormatInfo dtfi = DateTimeFormatHelper.CurrentUIDateTimeFormat;
				DayOfWeek dayOfWeek = DateTimeHelper.ToDayOfWeek(val);
				return dtfi.GetDayName(dayOfWeek);
			}
		}
	}
	#endregion
	#region WeekDaysEdit
	[
	DXToolboxItem(DXToolboxItemKind.Regular),
	ToolboxTabName(AssemblyInfo.DXTabScheduling),
	ToolboxBitmap(typeof(SchedulerControl), DevExpress.Utils.ControlConstants.BitmapPath + "weekDaysEdit.bmp"),
	System.Runtime.InteropServices.ComVisible(false),
	Designer("DevExpress.XtraScheduler.Design.XtraSchedulerSuiteComboBoxEditDesigner," + AssemblyInfo.SRAssemblySchedulerDesign),
	Description("A combo box control used to select days of the week.")
	]
	public class WeekDaysEdit : ImageComboBoxEdit {
		#region Properties
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public WeekDays DayOfWeek {
			get {
				object val = base.EditValue;
				if (IsNullValue(val))
					return WeekDays.Sunday;
				else
					return (WeekDays)val;
			}
			set { base.EditValue = value; }
		}
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("WeekDaysEditEditorTypeName")]
#endif
		public override string EditorTypeName { get { return GetType().Name; } }
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("WeekDaysEditProperties"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new RepositoryItemDayOfWeek Properties { get { return base.Properties as RepositoryItemDayOfWeek; } }
		#endregion
		static WeekDaysEdit() {
			RepositoryItemDayOfWeek.RegisterWeekDaysEdit();
		}
	}
	#endregion
}
