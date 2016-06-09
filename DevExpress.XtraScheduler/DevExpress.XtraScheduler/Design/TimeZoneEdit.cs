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
using System.Reflection;
using DevExpress.Utils.Controls;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraScheduler.Native;
using DevExpress.Utils;
using System.Collections.ObjectModel;
using DevExpress.XtraEditors.ListControls;
using DevExpress.Utils.Design;
using DevExpress.Data.Filtering;
using DevExpress.XtraEditors.ViewInfo;
namespace DevExpress.XtraScheduler.UI {
	#region RepositoryItemTimeZone
	[
	UserRepositoryItem("RegisterTimeZoneEdit"),
	System.Runtime.InteropServices.ComVisible(false)
	]
	public class RepositoryItemTimeZone : RepositoryItemLookUpEdit {
		internal static string InternalEditorTypeName { get { return typeof(TimeZoneEdit).Name; } }
		static RepositoryItemTimeZone() { RegisterTimeZoneEdit(); }
		public RepositoryItemTimeZone() {
			InitItems();
		}
		public static void RegisterTimeZoneEdit() {
			Bitmap img = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraScheduler.Bitmaps256.TimeZoneEdit.bmp", Assembly.GetExecutingAssembly());
			img.MakeTransparent(Color.Magenta);
			EditorClassInfo editorInfo = new EditorClassInfo(typeof(TimeZoneEdit).Name, typeof(TimeZoneEdit), typeof(RepositoryItemTimeZone), typeof(DevExpress.XtraEditors.ViewInfo.LookUpEditViewInfo), new DevExpress.XtraEditors.Drawing.ButtonEditPainter(), true, img);
			editorInfo.AllowInplaceEditing = ShowInContainerDesigner.OnlyInBars;
			EditorRegistrationInfo.Default.Editors.Add(editorInfo);
		}
		#region Properties
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new LookUpColumnInfoCollection Columns { get { return base.Columns; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new string DisplayMember { get { return base.DisplayMember; } set { base.DisplayMember = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new string ValueMember { get { return base.ValueMember; } set { base.ValueMember = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool ShowLines { get { return base.ShowLines; } set { base.ShowLines = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new object DataSource { get { return base.DataSource; } set { base.DataSource = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new BestFitMode BestFitMode { get { return base.BestFitMode; } set { base.BestFitMode = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool ShowFooter { get { return false; } set { } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool ShowHeader { get { return false; } set { } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string NullText { get { return String.Empty; } set { } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override TextEditStyles TextEditStyle { get { return TextEditStyles.DisableTextEditor; } set { } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal LookUpListDataAdapter ProtectedDataAdapter { get { return DataAdapter; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal virtual FormatInfo ProtectedActiveFormat { get { return ActiveFormat; } }
		public override string EditorTypeName { get { return InternalEditorTypeName; } }
		#endregion
		protected internal string GetValueDisplayTextProtected(FormatInfo format, object displayValue) {
			return GetValueDisplayText(format, displayValue);
		}
		protected override LookUpListDataAdapter CreateDataAdapter() {
			LookUpListDataAdapter adapter = new TimeZoneAdapter(this);
			return adapter;
		}
		protected void InitItems() {
			BeginUpdate();
			try {
				DataSource = TimeZoneInfo.GetSystemTimeZones();
				ValueMember = "Id";
				DisplayMember = "DisplayName";
				ShowLines = false;
				BestFitMode = BestFitMode.BestFitResizePopup;
				LookUpColumnInfo column = new LookUpColumnInfo(DisplayMember);
				if (Columns.IndexOf(column) == -1)
					Columns.Add(column);
			} finally {
				CancelUpdate();
			}
		}
	}
	#endregion
	#region TimeZoneAdapter
	public class TimeZoneAdapter : LookUpListDataAdapter {
		public TimeZoneAdapter(RepositoryItemLookUpEdit item) : base(item) { }
		protected override string CreateFilterExpression() {
			if (string.IsNullOrEmpty(FilterPrefix))
				return string.Empty;
			return CriteriaOperator.ToString(new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty(FilterField), FilterPrefix));
		}
	}
	#endregion
	#region TimeZoneEdit
	[
	DXToolboxItem(DXToolboxItemKind.Regular),
	ToolboxTabName(AssemblyInfo.DXTabScheduling),
	ToolboxBitmap(typeof(SchedulerControl), DevExpress.Utils.ControlConstants.BitmapPath + "TimeZoneEdit.bmp"),
	System.Runtime.InteropServices.ComVisible(false),
	Designer("DevExpress.XtraScheduler.Design.TimeZoneEditDesigner," + AssemblyInfo.SRAssemblySchedulerDesign),
	Description("A combo box used to specify a time zone.")]
	public class TimeZoneEdit : LookUpEdit, ISmartDesignerActionListOwner {
		static TimeZoneEdit() {   
			RepositoryItemTimeZone.RegisterTimeZoneEdit();
		}
		#region Properties
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("TimeZoneEditEditorTypeName")]
#endif
		public override string EditorTypeName { get { return GetType().Name; } }
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("TimeZoneEditProperties"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new RepositoryItemTimeZone Properties { get { return base.Properties as RepositoryItemTimeZone; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string TimeZoneId {
			get {
				string val = base.EditValue as string;
				if (IsNullValue(val))
					return TimeZoneInfo.Local.Id;
				else
					return val;
			}
			set {
				EditValue = value;
			}
		}
		protected override BaseEditViewInfo EditViewInfo {
			get {
				BaseEditViewInfo viewInfo = base.EditViewInfo;
				viewInfo.MatchedStringUseContains = true;
				return viewInfo;
			}
		}
		#endregion
		protected override int FindItem(string text, bool partialSearch, int startIndex) {
			if (text == null)
				return -1;
			if (text.Length == 0 && partialSearch)
				return -1;
			if (!Properties.CaseSensitiveSearch)
				text = text.ToLower();
			if (startIndex < 0)
				startIndex = 0;
			for (int i = startIndex; i < Properties.ProtectedDataAdapter.ItemCount; i++) {
				string itemText = Properties.GetValueDisplayTextProtected(Properties.ProtectedActiveFormat, Properties.ProtectedDataAdapter.GetValueAtIndex(Properties.DisplayMember, i));
				if (!Properties.CaseSensitiveSearch)
					itemText = itemText.ToLower();
				if (partialSearch) {
					if (itemText.IndexOf(text) != -1)
						return i;
				} else {
					if (text == itemText)
						return i;
				}
			}
			return -1;
		}
		bool ISmartDesignerActionListOwner.AllowSmartTag(IComponent component) {
			return false;
		}
	}
	#endregion
}
