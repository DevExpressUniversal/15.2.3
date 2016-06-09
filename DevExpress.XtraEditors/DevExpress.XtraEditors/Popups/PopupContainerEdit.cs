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
using System.Windows.Forms;
using System.Reflection;
using System.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.ToolboxIcons;
namespace DevExpress.XtraEditors.Repository {
	public class RepositoryItemPopupContainerEdit : RepositoryItemPopupBase {
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete(ObsoleteText.SRObsoletePropertiesText)]
		public new RepositoryItemPopupContainerEdit Properties { get { return this; } }
		[Browsable(false)]
		public override string EditorTypeName { get { return "PopupContainerEdit"; } }
		PopupContainerControl _popupControl;
		bool fPopupSizeable, _showPopupCloseButton, _closeOnOuterMouseClick, _closeOnLostFocus;
		bool usePopupControlMinSize, closeActAsOkButton;
		private static readonly object queryResultValue = new object();
		private static readonly object queryDisplayText = new object();
		public RepositoryItemPopupContainerEdit() {
			this.closeActAsOkButton = false;
			this._popupControl = null;
			this.fTextEditStyle = TextEditStyles.DisableTextEditor;
			this.fPopupSizeable = true;
			this._showPopupCloseButton = true;
			this._closeOnLostFocus = true;
			this._closeOnOuterMouseClick = true;
			this.usePopupControlMinSize = false;
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(OwnerEdit == null || OwnerEdit.InplaceType == InplaceType.Standalone) {
					if(!Cloned && PopupControl != null) PopupControl.Dispose();
				}
				PopupControl = null;
			}
			base.Dispose(disposing);
		}
		protected new PopupContainerEdit OwnerEdit { get { return base.OwnerEdit as PopupContainerEdit; } }
		public override void Assign(RepositoryItem item) {
			RepositoryItemPopupContainerEdit source = item as RepositoryItemPopupContainerEdit;
			BeginUpdate(); 
			try {
				base.Assign(item);
				if(source == null) return;
				this.closeActAsOkButton = source.CloseActAsOkButton;
				this._closeOnLostFocus = source.CloseOnLostFocus;
				this._closeOnOuterMouseClick = source.CloseOnOuterMouseClick;
				this._popupControl = source.PopupControl;
				this.fPopupSizeable = source.PopupSizeable;
				this._showPopupCloseButton = source.ShowPopupCloseButton;
				this.usePopupControlMinSize = source.UsePopupControlMinSize;
			}
			finally {
				EndUpdate();
			}
			Events.AddHandler(queryResultValue, source.Events[queryResultValue]);
			Events.AddHandler(queryDisplayText, source.Events[queryDisplayText]);
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemPopupContainerEditCloseOnOuterMouseClick"),
#endif
 DefaultValue(true)]
		public virtual bool CloseOnOuterMouseClick {
			get { return _closeOnOuterMouseClick; }
			set { 
				if(CloseOnOuterMouseClick == value) return;
				_closeOnOuterMouseClick = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemPopupContainerEditCloseOnLostFocus"),
#endif
 DefaultValue(true)]
		public virtual bool CloseOnLostFocus {
			get { return _closeOnLostFocus; }
			set { 
				if(CloseOnLostFocus == value) return;
				_closeOnLostFocus = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemPopupContainerEditUsePopupControlMinSize"),
#endif
 DefaultValue(false)]
		public virtual bool UsePopupControlMinSize {
			get { return usePopupControlMinSize; }
			set {
				if(UsePopupControlMinSize == value) return;
				usePopupControlMinSize = value;
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemPopupContainerEditPopupControl"),
#endif
 DefaultValue(null), DXCategory(CategoryName.Data), SmartTagProperty("Popup Control", "")]
		public virtual PopupContainerControl PopupControl {
			get { return _popupControl; }
			set {
				if(PopupControl == value) return;
				OnPopupControlChanging();
				_popupControl = value;
				if(value != null) PopupControl.OwnerItem = this;
				OnPopupControlChanged();
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemPopupContainerEditPopupSizeable"),
#endif
 DefaultValue(true)]
		public bool PopupSizeable {
			get { return fPopupSizeable; }
			set {
				if(PopupSizeable == value) return;
				fPopupSizeable = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemPopupContainerEditShowPopupCloseButton"),
#endif
 DefaultValue(true)]
		public virtual bool ShowPopupCloseButton {
			get { return _showPopupCloseButton; }
			set {
				if(ShowPopupCloseButton == value) return;
				_showPopupCloseButton = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemPopupContainerEditTextEditStyle"),
#endif
 DefaultValue(TextEditStyles.DisableTextEditor)]
		public override TextEditStyles TextEditStyle {
			get { return base.TextEditStyle; }
			set { base.TextEditStyle = value; }
		}
		protected internal virtual bool CloseActAsOkButton { get { return closeActAsOkButton; } set { closeActAsOkButton = value; } }
		protected virtual void OnPopupControlChanging() {
			if(OwnerEdit != null) {
				OwnerEdit.OnPopupControlChanging();
			}
		}
		protected virtual void OnPopupControlChanged() {
			if(OwnerEdit != null) {
				OwnerEdit.OnPopupControlChanged();
			}
		}
		public override string GetDisplayText(FormatInfo format, object editValue) {
			string res = base.GetDisplayText(format, editValue);
			QueryDisplayTextEventArgs e = new QueryDisplayTextEventArgs(editValue, res);
			RaiseQueryDisplayText(e);
			return e.DisplayText;
		}
		protected internal virtual void PreQueryDisplayText(QueryDisplayTextEventArgs e) { }
		protected internal virtual void PreQueryResultValue(QueryResultValueEventArgs e) { }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemPopupContainerEditQueryResultValue"),
#endif
 DXCategory(CategoryName.Events)]
		public event QueryResultValueEventHandler QueryResultValue {
			add { this.Events.AddHandler(queryResultValue, value); }
			remove { this.Events.RemoveHandler(queryResultValue, value); }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemPopupContainerEditQueryDisplayText"),
#endif
 DXCategory(CategoryName.Events)]
		public event QueryDisplayTextEventHandler QueryDisplayText {
			add { this.Events.AddHandler(queryDisplayText, value); }
			remove { this.Events.RemoveHandler(queryDisplayText, value); }
		}
		protected internal virtual void RaiseQueryResultValue(QueryResultValueEventArgs e) {
			PreQueryResultValue(e);
			QueryResultValueEventHandler handler = (QueryResultValueEventHandler)this.Events[queryResultValue];
			if(handler != null) handler(GetEventSender(), e);
		}
		protected internal virtual void RaiseQueryDisplayText(QueryDisplayTextEventArgs e) {
			PreQueryDisplayText(e);
			QueryDisplayTextEventHandler handler = (QueryDisplayTextEventHandler)this.Events[queryDisplayText];
			if(handler != null) handler(GetEventSender(), e);
		}
		[Browsable(false)]
		public override HorzAlignment DefaultAlignment { get { return HorzAlignment.Near; } }
	}
}
namespace DevExpress.XtraEditors {
	[DXToolboxItem(DXToolboxItemKind.Free), DefaultBindingPropertyEx("EditValue"), Designer("DevExpress.XtraEditors.Design.PopupContainerEditDesigner, " + AssemblyInfo.SRAssemblyEditorsDesign),
	 Description("Allows you to display custom controls in its popup window."),
	 ToolboxTabName(AssemblyInfo.DXTabCommon),
	ToolboxBitmap(typeof(ToolboxIconsRootNS), "PopupContainerEdit")
	]
	public class PopupContainerEdit : PopupBaseEdit {
		public PopupContainerEdit() {
		}
		[Browsable(false)]
		public override string EditorTypeName { get { return "PopupContainerEdit"; } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("PopupContainerEditProperties"),
#endif
 DXCategory(CategoryName.Properties), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), SmartTagSearchNestedProperties]
		public new RepositoryItemPopupContainerEdit Properties { get { return base.Properties as RepositoryItemPopupContainerEdit; } }
		protected override PopupBaseForm CreatePopupForm() {
			if(Properties.PopupControl == null) return null;
			return new PopupContainerForm(this);
		}
		protected internal virtual void OnPopupControlChanged() {
			DestroyPopupForm();
		}
		protected internal virtual void OnPopupControlChanging() {
			Control popupControl = Properties.PopupControl;
			if(PopupForm != null && popupControl != null) {
				ClosePopup();
				popupControl.Parent = null;
			}
		}
		public override bool DoValidate(PopupCloseMode closeMode) {
			if(IsPopupOpen) {
				DoClosePopup(closeMode);
				if(IsPopupOpen) return false;
			}
			return base.DoValidate(closeMode);
		}
		public override void ShowPopup() {
			if(Properties.PopupControl != null) Properties.PopupControl.SetOwnerEdit(this);
			base.ShowPopup();
		}
		protected override void DoShowPopup() {
			if(Properties.PopupControl != null) Properties.PopupControl.SetOwnerEdit(this);
			base.DoShowPopup();
		}
		protected override void DoClosePopup(PopupCloseMode closeMode) {
			base.DoClosePopup(closeMode);
			if(Properties.PopupControl != null) Properties.PopupControl.SetOwnerEdit(null);
		}
		protected internal override bool AllowPopupTabOut { get { return false; } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("PopupContainerEditQueryResultValue"),
#endif
 DXCategory(CategoryName.Events)]
		public event QueryResultValueEventHandler QueryResultValue {
			add { Properties.QueryResultValue += value; }
			remove { Properties.QueryResultValue -= value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("PopupContainerEditQueryDisplayText"),
#endif
 DXCategory(CategoryName.Events)]
		public event QueryDisplayTextEventHandler QueryDisplayText {
			add { Properties.QueryDisplayText += value; }
			remove { Properties.QueryDisplayText -= value; }
		}
	}
	[DXToolboxItem(DXToolboxItemKind.Free),
	 Description("Represents a panel, that can contain any controls, to be displayed as a drop-down window for a PopupContainerEdit control."),
	 ToolboxTabName(AssemblyInfo.DXTabCommon),
	 Designer("DevExpress.XtraEditors.Design.PopupContainerControlDesigner," + AssemblyInfo.SRAssemblyEditorsDesign),
	ToolboxBitmap(typeof(ToolboxIconsRootNS), "PopupContainerControl")
	]
	public class PopupContainerControl : PanelControl, ISupportLookAndFeel {
		RepositoryItemPopupContainerEdit ownerItem;
		PopupContainerEdit ownerEdit;
		public PopupContainerControl() {
			SetStyle(ControlStyles.Selectable, false);
			base.Visible = false;
			this.TabStop = false;
			this.ownerItem = null;
			this.ownerEdit = null;
		}
		bool ISupportLookAndFeel.IgnoreChildren { get { return true; } }
		protected override void OnControlAdded(ControlEventArgs e) {
			if(e.Control is Form && ((Form)e.Control).TopLevel)
				throw new InvalidOperationException("PopupContainerControl can't contains Form");
			base.OnControlAdded(e);
		}
		protected override BorderStyles DefaultBorderStyle { get { return BorderStyles.NoBorder; } }
		protected internal virtual RepositoryItemPopupContainerEdit OwnerItem {
			get { return ownerItem; }
			set {
				if(OwnerItem == value) return;
				RepositoryItemPopupContainerEdit prev = OwnerItem;
				this.ownerItem = null;
				if(prev != null) prev.PopupControl = null;
				this.ownerItem = value;
			}
		}
		[Browsable(false)]
		public virtual RepositoryItemPopupContainerEdit PopupContainerProperties { get { return OwnerItem; } }
		[Browsable(false)]
		public virtual PopupContainerEdit OwnerEdit { 
			get { return OwnerItem == null ? null : ownerEdit; }
		}
		protected internal virtual void SetOwnerEdit(PopupContainerEdit ownerEdit) {
			this.ownerEdit = ownerEdit;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool TabStop {
			get { return base.TabStop; }
			set { base.TabStop = false; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool Visible {
			get { return base.Visible; }
			set { base.Visible = false; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Localizable(false)]
		public override DockStyle Dock {
			get { return DockStyle.None; }
		}
		protected internal virtual void SetVisible(bool newVisible) {
			base.Visible = newVisible;
		}
	}
}
namespace DevExpress.XtraEditors.ViewInfo {
	public class PopupContainerEditViewInfo : PopupBaseEditViewInfo {
		public PopupContainerEditViewInfo(RepositoryItem item) : base(item) {
		}
		public override bool RefreshDisplayText { get { return true; } }
	}
}
namespace DevExpress.XtraEditors.Popup {
	public class PopupContainerForm : CustomBlobPopupForm {
		bool formConstructed = false;
		public PopupContainerForm(PopupContainerEdit ownerEdit) : base(ownerEdit) {
			this.Controls.Add(PopupControl);
			PopupControl.SetVisible(true);
			this.formConstructed = true;
		}
		protected override bool AllowLayoutChanged { get { return base.AllowLayoutChanged && formConstructed; } }
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(Controls.Contains(PopupControl)) Controls.Remove(PopupControl);
			}
			base.Dispose(disposing);
		}
		public override void ProcessKeyDown(KeyEventArgs e) {
			if(!Focused && ActiveControl != null) {
				if((e.KeyData == (Keys.Alt | Keys.Down)) || CheckIsInputKey(ActiveControl, e)) return;
			}
			base.ProcessKeyDown(e);
		}
		bool CheckIsInputKey(Control control, KeyEventArgs e) {
			if(control == null) return false;
			try {
				MethodInfo mi = typeof(Control).GetMethod("IsInputKey", BindingFlags.Instance | BindingFlags.NonPublic);
				if(mi != null) {
					if((bool)mi.Invoke(control, new object[] { e.KeyData })) return true;
				}
			} catch { }
			return false;
		}
		public override bool AllowMouseClick(Control control, Point mousePosition) {
			if(base.AllowMouseClick(control, mousePosition)) return true;
			Form controlForm = control == null ? null : control.FindForm();
			PopupBaseForm popupControlForm = controlForm as PopupBaseForm;
			if(control == OwnerEdit.Parent || (controlForm != null && controlForm.Contains(OwnerEdit))) return false;
			if(popupControlForm != null && popupControlForm.OwnerEdit != null && this.Contains(popupControlForm.OwnerEdit)) return true;
			if(!OwnerEdit.Properties.CloseOnOuterMouseClick) return true;
			return false;
		}
		[DXCategory(CategoryName.Appearance)]
		public override object ResultValue {
			get {
				QueryResultValueEventArgs e = new QueryResultValueEventArgs(OwnerEdit.EditValue);
				OwnerEdit.Properties.RaiseQueryResultValue(e);
				return e.Value;
			}
		}
		protected override bool UseSimpleVisible { get { return OwnerEdit.Properties.CloseOnOuterMouseClick; } }
		[DXCategory(CategoryName.Focus)]
		public override bool FormContainsFocus { 
			get { 
				if(base.FormContainsFocus) return true;
				if(OwnerEdit.Properties.CloseOnLostFocus) return false;
				Form form = OwnerEdit.FindForm();
				if(form != null) {
					if(form.ContainsFocus && !OwnerEdit.ContainsFocus) return false;
				}
				return true;
			}
		}
		protected override void OnCloseButtonClick() {
			if(OwnerEdit.Properties.CloseActAsOkButton)
				OwnerEdit.ClosePopup();
			else
				OwnerEdit.CancelPopup();
		}
		protected override void SetupButtons() {
			this.AllowSizing = OwnerEdit.Properties.PopupSizeable;
			this.fShowOkButton = false;
			this.fCloseButtonStyle = !OwnerEdit.Properties.ShowPopupCloseButton ? BlobCloseButtonStyle.None : BlobCloseButtonStyle.Glyph;
			if(!AllowSizing && CloseButtonStyle == BlobCloseButtonStyle.None) {
				ViewInfo.ShowSizeBar = false;
			}
		}
		protected override Size DefaultMinFormSize { get { return new Size(base.DefaultMinFormSize.Width, 30); } }
		protected override Size UpdateMinFormSize(Size minSize) {
			if(!OwnerEdit.Properties.UsePopupControlMinSize || OwnerEdit.Properties.PopupControl.MinimumSize == Size.Empty) return base.UpdateMinFormSize(minSize);
			Size fs = CalcFormSize(OwnerEdit.Properties.PopupControl.MinimumSize);
			minSize.Width = Math.Max(minSize.Width, fs.Width);
			minSize.Height = Math.Max(minSize.Height, fs.Height);
			return minSize;
		}
		protected override Size CalcFormSizeCore() {
			Size size = CalcFormSize(OwnerEdit.Properties.PopupControl.ClientSize);
			Size desiredSize = OwnerEdit.Properties.GetDesiredPopupFormSize(false);
			if(desiredSize.Width != 0) size.Width = desiredSize.Width;
			if(desiredSize.Height != 0) size.Height = desiredSize.Height;
			size.Width = Math.Max(MinFormSize.Width, size.Width);
			size.Height = Math.Max(MinFormSize.Height, size.Height);
			return size;
		}
		public override void ShowPopupForm() {
			this.Controls.Add(PopupControl);
			PopupControl.SetVisible(true);
			base.ShowPopupForm();
			FocusFormControl(PopupControl);
			SelectNextControl(PopupControl, true, true, true, true);
		}
		protected virtual PopupContainerControl PopupControl { get { return  OwnerEdit.Properties.PopupControl; } }
		protected override Control EmbeddedControl { get { return PopupControl; } }
		[Browsable(false)]
		public new PopupContainerEdit OwnerEdit { get { return base.OwnerEdit as PopupContainerEdit; } }
	}
}
