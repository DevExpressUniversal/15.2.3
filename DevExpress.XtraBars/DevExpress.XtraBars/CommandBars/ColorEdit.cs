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
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.Office.UI.Internal;
using DevExpress.XtraBars.Localization;
namespace DevExpress.Office.UI {
	interface IRepositoryItemColorEdit {
		Color ConvertToColor(object editValue);
	}
	#region RepositoryItemOfficeColorEditBase (abstract class)
	public abstract class RepositoryItemOfficeColorEditBase : RepositoryItemColorEdit, IRepositoryItemColorEdit {
		[EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool ShowCustomColors { get { return true; } set { base.ShowCustomColors = true; } }
		[EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool ShowSystemColors { get { return false; } set { base.ShowSystemColors = false; } }
		[EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool ShowWebColors { get { return false; } set { base.ShowWebColors = false; } }
		[EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new HorzAlignment ColorAlignment { get { return HorzAlignment.Center; } set { base.ColorAlignment = HorzAlignment.Center; } }
		[EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override HorzAlignment DefaultAlignment { get { return HorzAlignment.Center; } }
		public override BaseEditViewInfo CreateViewInfo() {
			return new OfficeColorEditViewInfo(this);
		}
		protected override object ConvertToEditValue(object val) {
			return (val == null) ? null : base.ConvertToEditValue(val);
		}
		Color IRepositoryItemColorEdit.ConvertToColor(object editValue) {
			return this.ConvertToColor(editValue);
		}
	}
	#endregion
	#region RepositoryItemOfficeColorEdit
	[
	UserRepositoryItem("RegisterRepositoryItemColorEditEx"),
	System.Runtime.InteropServices.ComVisible(false)
	]
	public class RepositoryItemOfficeColorEdit : RepositoryItemOfficeColorEditBase {
		internal static string InternalEditorTypeName { get { return typeof(OfficeColorEdit).Name; } }
		static RepositoryItemOfficeColorEdit() { RegisterRepositoryItemColorEditEx(); }
		public static void RegisterRepositoryItemColorEditEx() {
			EditorClassInfo editorInfo = new EditorClassInfo(InternalEditorTypeName, typeof(OfficeColorEdit), typeof(RepositoryItemOfficeColorEdit), typeof(OfficeColorEditViewInfo), new ColorEditPainter(), true);
			editorInfo.AllowInplaceEditing = ShowInContainerDesigner.OnlyInBars;
			EditorRegistrationInfo.Default.Editors.Add(editorInfo);
		}
	}
	#endregion
	#region OfficeColorEdit
	[ToolboxItem(false)]
	public class OfficeColorEdit : ColorEdit {
		static OfficeColorEdit() {
			RepositoryItemOfficeColorEdit.RegisterRepositoryItemColorEditEx();
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new RepositoryItemOfficeColorEdit Properties { get { return base.Properties as RepositoryItemOfficeColorEdit; } }
		public override string EditorTypeName { get { return GetType().Name; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override object EditValue { get { return base.EditValue; } set { base.EditValue = value; } }
		public OfficePopupColorEditForm PopupControl { get { return (OfficePopupColorEditForm)GetPopupForm(); } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Color? Value { get { return EditValue as Color?; } set { EditValue = value; } }
		protected override PopupBaseForm CreatePopupForm() {
			return new OfficePopupColorEditForm(this);
		}
	}
	#endregion
}
namespace DevExpress.Office.UI.Internal {
	public interface IOfficePopupColorEdit : IPopupColorEdit, IPopupColorPickEdit {
		void OnDefaultButtonClick();
	}
	#region OfficePopupColorBuilder
	public class OfficePopupColorBuilder : PopupColorBuilder {
		#region Fields
		SimpleButton btnDefaultColor;
		PanelControl panel;
		#endregion
		public OfficePopupColorBuilder(IOfficePopupColorEdit owner)
			: base(owner) {
			InitializeControls();
		}
		#region Properties
		public SimpleButton BtnDefaultColor { get { return btnDefaultColor; } }
		public override System.Windows.Forms.Control EmbeddedControl { get { return panel; } }
		#endregion
		protected internal virtual SimpleButton CreateDefaultColorButton() {
			return new SimpleButton();
		}
		private void InitializeControls() {
			InitializeDefaultColorButton();
			InitializePanel();
			TabControl.Dock = System.Windows.Forms.DockStyle.Fill;
		}
		protected virtual void InitializeDefaultColorButton() {
			this.btnDefaultColor = CreateDefaultColorButton();
			this.btnDefaultColor.Dock = System.Windows.Forms.DockStyle.Top;
			this.btnDefaultColor.Text = BarLocalizer.Active.GetLocalizedString(BarString.ColorAuto);
			this.btnDefaultColor.AllowFocus = false;
			this.btnDefaultColor.Click += OnBtnDefaultColorClick;
		}
		protected virtual void InitializePanel() {
			this.panel = new PanelControl();
			this.panel.BorderStyle = BorderStyles.NoBorder;
			this.panel.Size = CalcContentSizeCore();
			this.panel.Controls.Add(TabControl);
			this.panel.Controls.Add(BtnDefaultColor);
		}
		protected virtual void OnBtnDefaultColorClick(object sender, EventArgs e) {
			if (!Owner.IsPopupOpen)
				return;
			((IOfficePopupColorEdit)Owner).OnDefaultButtonClick();
			Owner.ClosePopup();
		}
		public override Size CalcContentSize() {
			return this.panel.Size;
		}
		protected virtual Size CalcContentSizeCore() {
			Size size = base.CalcContentSize();
			size.Height += BtnDefaultColor.Height;
			return size;
		}
	}
	#endregion
	#region OfficePopupColorEditForm
	public class OfficePopupColorEditForm : PopupColorEditForm, IOfficePopupColorEdit {
		bool isDefaultColor;
		public OfficePopupColorEditForm(OfficeColorEdit ownerEdit)
			: base(ownerEdit) {
		}
		OfficePopupColorBuilder OfficePopupColorBuilder { get { return (OfficePopupColorBuilder)PopupColorBuilder; } }
		public DevExpress.XtraEditors.SimpleButton BtnDefaultColor { get { return OfficePopupColorBuilder.BtnDefaultColor; } }
		public override object ResultValue {
			get {
				if (this.isDefaultColor)
					return Color.Empty;
				return base.ResultValue;
			}
		}
		protected override System.Windows.Forms.Control EmbeddedControl { get { return OfficePopupColorBuilder.EmbeddedControl; } }
		protected override void Initialize() {
			this.Controls.Add(OfficePopupColorBuilder.EmbeddedControl);
		}
		protected override PopupColorBuilder CreatePopupColorEditBuilder() {
			return new OfficePopupColorBuilder(this);
		}
		public override void ShowPopupForm() {
			this.isDefaultColor = false;
			base.ShowPopupForm();
		}
		#region IOfficePopupColorEdit Members
		void IOfficePopupColorEdit.OnDefaultButtonClick() {
			this.isDefaultColor = true;
		}
		#endregion
		#region IPopupColorPickEdit Members
		void IPopupColorPickEdit.ClosePopup(PopupCloseMode mode) {
			ClosePopup();
		}
		ColorPickEditBase IPopupColorPickEdit.OwnerEdit {
			get { return OwnerEdit as ColorPickEditBase; }
		}
		void IPopupColorPickEdit.SetSelectedColorItem(ColorItem colorItem) { }
		bool IPopupColorPickEdit.HasBorder { get { return false; } }
		#endregion
	}
	#endregion
	#region OfficeColorEditViewInfo
	public class OfficeColorEditViewInfo : ColorEditViewInfo {
		bool isUndefinedColor;
		public OfficeColorEditViewInfo(RepositoryItem item)
			: base(item) {
		}
		new IRepositoryItemColorEdit Item { get { return base.Item as IRepositoryItemColorEdit; } }
		public bool IsUndefinedColor { get { return isUndefinedColor; } set { isUndefinedColor = value; } }
		protected override void CalcColorBoxRect() {
			base.fColorBoxRect = (IsUndefinedColor || Color.IsEmpty) ? Rectangle.Empty : Rectangle.Inflate(MaskBoxRect, -ColorIndent, -ColorIndent);
		}
		protected override void CalcColorTextRect() {
			base.fColorTextRect = (!IsUndefinedColor && Color.IsEmpty) ? MaskBoxRect : Rectangle.Empty;
			if (Color.IsEmpty)
				SetDisplayText(BarLocalizer.Active.GetLocalizedString(BarString.ColorAuto));
		}
		protected override void OnEditValueChanged() {
			RefreshDisplayText = true;
			this.fColor = Item.ConvertToColor(EditValue);
			this.isUndefinedColor = (EditValue == null);
		}
	}
	#endregion
}
