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
using DevExpress.XtraScheduler.Native;
using DevExpress.Utils;
namespace DevExpress.XtraScheduler.UI {
	#region RepositoryItemMonth
	[
	UserRepositoryItem("RegisterMonthEdit"),
	System.Runtime.InteropServices.ComVisible(false)
	]
	public class RepositoryItemMonth : RepositoryItemImageComboBox {
		static RepositoryItemMonth() { RegisterMonthEdit(); }
		public RepositoryItemMonth() {
			InitItems();
		}
		public static void RegisterMonthEdit() {
			Bitmap img = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraScheduler.Bitmaps256.monthEdit.bmp", Assembly.GetExecutingAssembly());
			img.MakeTransparent(Color.Magenta);
			EditorClassInfo editorInfo = new EditorClassInfo(typeof(MonthEdit).Name, typeof(MonthEdit), typeof(RepositoryItemMonth), typeof(DevExpress.XtraEditors.ViewInfo.ImageComboBoxEditViewInfo), new DevExpress.XtraEditors.Drawing.ImageComboBoxEditPainter(), true, img);
			editorInfo.AllowInplaceEditing = ShowInContainerDesigner.OnlyInBars;
			EditorRegistrationInfo.Default.Editors.Add(editorInfo);
		}
		#region Properties
		internal static string InternalEditorTypeName { get { return typeof(MonthEdit).Name; } }
		public override string EditorTypeName { get { return InternalEditorTypeName; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ImageComboBoxItemCollection Items { get { return base.Items; } }
		#endregion
		protected void InitItems() {
			BeginUpdate();
			try {
				Items.Clear();
				DateTimeFormatInfo dtfi = DateTimeFormatHelper.CurrentUIDateTimeFormat;
				int count = 12;
				for (int i = 1; i <= count; i++)
					Items.Add(new ImageComboBoxItem(dtfi.GetMonthName(i), i, -1));
			}
			finally {
				CancelUpdate();
			}
		}
	}
	#endregion
	#region MonthEdit
	[
	DXToolboxItem(DXToolboxItemKind.Regular),
	ToolboxTabName(AssemblyInfo.DXTabScheduling),
	ToolboxBitmap(typeof(SchedulerControl), DevExpress.Utils.ControlConstants.BitmapPath + "monthEdit.bmp"),
	System.Runtime.InteropServices.ComVisible(false),
	Designer("DevExpress.XtraScheduler.Design.XtraSchedulerSuiteComboBoxEditDesigner," + AssemblyInfo.SRAssemblySchedulerDesign),
	Description("A combo box control used to select a month.")
	]
	public class MonthEdit : ImageComboBoxEdit {
		#region Properties
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int Month {
			get {
				object val = base.EditValue;
				if (IsNullValue(val))
					return 1; 
				else
					return (int)val;
			}
			set {
				base.EditValue = value;
			}
		}
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("MonthEditEditorTypeName")]
#endif
		public override string EditorTypeName { get { return GetType().Name; } }
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("MonthEditProperties"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new RepositoryItemMonth Properties { get { return base.Properties as RepositoryItemMonth; } }
		#endregion
		static MonthEdit() {
			RepositoryItemMonth.RegisterMonthEdit();
		}
	}
	#endregion
}
