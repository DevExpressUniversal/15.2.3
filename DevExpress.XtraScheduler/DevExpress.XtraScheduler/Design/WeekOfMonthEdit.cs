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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using DevExpress.Utils.Controls;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraScheduler.Localization;
using DevExpress.Utils;
namespace DevExpress.XtraScheduler.UI {
	#region RepositoryItemWeekOfMonth
	[
	UserRepositoryItem("RegisterWeekOfMonthEdit"),
	System.Runtime.InteropServices.ComVisible(false)
	]
	public class RepositoryItemWeekOfMonth : RepositoryItemImageComboBox {
		static Hashtable displayNameHT = new Hashtable();
		static RepositoryItemWeekOfMonth() {
			RegisterWeekOfMonthEdit();
			displayNameHT[WeekOfMonth.First] = SchedulerStringId.Caption_WeekOfMonthFirst;
			displayNameHT[WeekOfMonth.Second] = SchedulerStringId.Caption_WeekOfMonthSecond;
			displayNameHT[WeekOfMonth.Third] = SchedulerStringId.Caption_WeekOfMonthThird;
			displayNameHT[WeekOfMonth.Fourth] = SchedulerStringId.Caption_WeekOfMonthFourth;
			displayNameHT[WeekOfMonth.Last] = SchedulerStringId.Caption_WeekOfMonthLast;
		}
		public RepositoryItemWeekOfMonth() {
			InitItems();
		}
		public static void RegisterWeekOfMonthEdit() {
			Bitmap img = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraScheduler.Bitmaps256.weekOfMonthEdit.bmp", Assembly.GetExecutingAssembly());
			img.MakeTransparent(Color.Magenta);
			EditorClassInfo editorInfo = new EditorClassInfo(typeof(WeekOfMonthEdit).Name, typeof(WeekOfMonthEdit), typeof(RepositoryItemWeekOfMonth), typeof(DevExpress.XtraEditors.ViewInfo.ImageComboBoxEditViewInfo), new DevExpress.XtraEditors.Drawing.ImageComboBoxEditPainter(), true, img);
			editorInfo.AllowInplaceEditing = ShowInContainerDesigner.OnlyInBars;
			EditorRegistrationInfo.Default.Editors.Add(editorInfo);
		}
		#region Properties
		internal static string InternalEditorTypeName { get { return typeof(WeekOfMonthEdit).Name; } }
		public override string EditorTypeName { get { return InternalEditorTypeName; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ImageComboBoxItemCollection Items { get { return base.Items; } }
		#endregion
		protected void InitItems() {
			BeginUpdate();
			try {
				Items.Clear();
				WeekOfMonth[] values = (WeekOfMonth[])Enum.GetValues(typeof(WeekOfMonth));
				int count = values.Length;
				for (int i = 0; i < count; i++)
					if (values[i] != WeekOfMonth.None)
						Items.Add(new ImageComboBoxItem(GetDisplayName(values[i]), values[i], -1));
			}
			finally {
				CancelUpdate();
			}
		}
		static string GetDisplayName(WeekOfMonth val) {
			return SchedulerLocalizer.GetString((SchedulerStringId)displayNameHT[val]);
		}
	}
	#endregion
	#region WeekOfMonthEdit
	[
	DXToolboxItem(DXToolboxItemKind.Regular),
	ToolboxTabName(AssemblyInfo.DXTabScheduling),
	ToolboxBitmap(typeof(SchedulerControl), DevExpress.Utils.ControlConstants.BitmapPath + "weekOfMonthEdit.bmp"),
	System.Runtime.InteropServices.ComVisible(false),
	Designer("DevExpress.XtraScheduler.Design.XtraSchedulerSuiteComboBoxEditDesigner," + AssemblyInfo.SRAssemblySchedulerDesign),
	Description("A combo box used to select a week of the month.")
	]
	public class WeekOfMonthEdit : ImageComboBoxEdit {
		#region Properties
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public WeekOfMonth WeekOfMonth {
			get {
				object val = base.EditValue;
				if (IsNullValue(val))
					return WeekOfMonth.First;
				else
					return (WeekOfMonth)val;
			}
			set { base.EditValue = value; }
		}
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("WeekOfMonthEditEditorTypeName")]
#endif
		public override string EditorTypeName { get { return GetType().Name; } }
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("WeekOfMonthEditProperties"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new RepositoryItemWeekOfMonth Properties { get { return base.Properties as RepositoryItemWeekOfMonth; } }
		#endregion
		static WeekOfMonthEdit() {
			RepositoryItemWeekOfMonth.RegisterWeekOfMonthEdit();
		}
	}
	#endregion
}
