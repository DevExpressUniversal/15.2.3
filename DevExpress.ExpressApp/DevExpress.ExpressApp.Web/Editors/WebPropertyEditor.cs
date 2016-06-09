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

using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.ExpressApp.Web.TestScripts;
using DevExpress.Persistent.Base;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.ExpressApp.Web.Utils;
namespace DevExpress.ExpressApp.Web.Editors {
	public interface ISupportExportCustomValue {
		string GetExportedValue();
	}
	public abstract class WebPropertyEditor : PropertyEditor, ISupportViewEditMode, IAppearanceFormat, ISupportToolTip, ISupportImmediatePostData {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public const string processValueChangedScript = @"xaf.ConfirmUnsavedChangedController.EditorValueChanged";
		[Obsolete("This member is not used any longer"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public const string ViewModeControlCssClass = "ValueViewControlClass";
		private string groupName;
		private Table table;
		private WebControl controlContainer;
		private WebControl editor;
		private WebControl inplaceViewModeEditor;
		private ViewEditMode viewEditMode;
		private bool editValueChangedHandling;
		private bool cancelClickEventPropagation = false;
		private bool useEditorErrorCell = false;
#if !SL
	[DevExpressExpressAppWebLocalizedDescription("WebPropertyEditorSupportInlineEdit")]
#endif
		public virtual bool SupportInlineEdit {
			get { return true; }
		}
		protected bool skipEditModeDataBind = false;
		private void table_PreRender(object sender, EventArgs e) {
			Table table = (Table)sender;
			table.PreRender -= new EventHandler(table_PreRender);
			TableCell errorCell = table.Rows[0].Cells[0];
			TableCell editorCell = table.Rows[0].Cells[1];
			if(!string.IsNullOrEmpty(ErrorMessage)) {
				if(Editor is ASPxEdit && useEditorErrorCell) {
					ASPxEdit aspxEdit = (ASPxEdit)Editor;
					aspxEdit.JSProperties["cpValidationText"] = ErrorMessage;
					aspxEdit.JSProperties["cpValidationImage"] = ErrorIcon.ImageUrl;
				}
				else {
					System.Web.UI.WebControls.Image image = new System.Web.UI.WebControls.Image();
					image.AlternateText = "Error";
					ImageInfo imageInfo = ErrorIcon;
					image.ImageUrl = imageInfo.ImageUrl;
					image.Width = imageInfo.Width;
					image.Height = imageInfo.Height;
					image.ToolTip = ErrorMessage;
					image.Style["margin"] = "5px";
					errorCell.Visible = true;
					errorCell.Width = Unit.Pixel(1);
					errorCell.Controls.Add(image);
				}
				editor.CssClass += " ValidationFailed";
			}
			else {
				errorCell.Visible = false;
			}
		}
		private void control_Init(object sender, EventArgs e) {
			System.Web.UI.Control control = (System.Web.UI.Control)sender;
			control.Init -= new EventHandler(control_Init);
			ReadValue();
		}
		private void control_Load(object sender, EventArgs e) {
			System.Web.UI.Control control = (System.Web.UI.Control)sender;
			control.Load -= new EventHandler(control_Load);
			control.DataBind();
			SetProcessValueChangedScriptCore(processValueChangedScript);
		}
		private void control_Unload(object sender, EventArgs e) {
			Control control = sender as Control;
			OnControlInitialized(control);
		}
		private void editor_PreRender(object sender, EventArgs e) {
			editor.PreRender -= new EventHandler(editor_PreRender);
			ReadValue();
		}
		protected void OnControlInitialized(Control control) {
			if(ControlInitialized != null) {
				ControlInitialized(this, new ControlInitializedEventArgs(control));
			}
		}
		public event EventHandler<ControlInitializedEventArgs> ControlInitialized;
		protected virtual WebControl CreateViewModeControlCore() {
			return new Label();
		}
		protected abstract WebControl CreateEditModeControlCore();
		protected virtual string GetPropertyDisplayValue() {
			return ReflectionHelper.GetObjectDisplayText(PropertyValue);
		}
		protected void EditValueChangedHandler(object sender, EventArgs e) {
			try {
				editValueChangedHandling = true;
				if(AllowEdit && !PopupWindow.IsRefreshOnCallback) {
					OnControlValueChanged();
					WriteValue();
				}
			}
			finally {
				editValueChangedHandling = false;
			}
		}
		protected void ReadViewModeValue() {
			ReadViewModeValueCore();
		}
		protected virtual void ReadViewModeValueCore() {
			string displayValue = GetPropertyDisplayValue();
			if(displayValue == null) {
				displayValue = string.Empty;
			}
			else {
				string encodedDisplayValue = System.Web.HttpUtility.HtmlEncode(displayValue);
				string preparedDisplayValue = encodedDisplayValue.Replace("\t", "&nbsp;&nbsp;&nbsp;").Replace("\r", "");
				string[] lines = preparedDisplayValue.Split('\n');
				displayValue = string.Join("<br>", lines);
			}
			if(inplaceViewModeEditor is Label) {
				((Label)inplaceViewModeEditor).Text = (IsPassword) ? new string('*', displayValue.Length) : displayValue;
			}
		}
		protected void ReadEditModeValue() {
			ReadEditModeValueCore();
			RefreshReadOnly();
		}
		protected abstract void ReadEditModeValueCore();
		protected override bool CanReadValue() {
			return base.CanReadValue() && !editValueChangedHandling;
		}
		protected override object CreateControlCore() {
			WebControl control;
			RefreshReadOnly();
			if(ViewEditMode == ViewEditMode.Edit) {
				editor = CreateEditModeControlCore();
				editor.PreRender += new EventHandler(editor_PreRender);
				editor.Load += new EventHandler(editor_Load);
				control = editor;
				ApplyReadOnly();
				if(control.Width.IsEmpty) {
					control.Width = Unit.Percentage(100);
				}
			}
			else {
				inplaceViewModeEditor = CreateViewModeControlCore();
				control = inplaceViewModeEditor;
			}
			control.Unload += new EventHandler(control_Unload);
			control.Load += new EventHandler(control_Load);
			control.Init += new EventHandler(control_Init);
			SetupControl(control);
			return CreateControlContainer(control);
		}
		private object CreateControlContainer(WebControl control) {
			object result;
			if(ViewEditMode == ViewEditMode.Edit) {
				table = RenderHelper.CreateTable();
				table.ID = "WebEditorContainer";
				result = table;
				table.CellPadding = 0;
				table.CellSpacing = 0;
				table.BorderWidth = 0;
				table.Width = Unit.Percentage(100);
				TableRow row = new TableRow();
				TableCell errorMessageCell = new TableCell();
				errorMessageCell.HorizontalAlign = HorizontalAlign.Left;
				TableCell controlCell = new TableCell();
				controlCell.Width = Unit.Percentage(100);
				controlCell.HorizontalAlign = HorizontalAlign.Left;
				controlContainer = controlCell;
				row.Cells.Add(errorMessageCell);
				row.Cells.Add(controlCell);
				table.Rows.Add(row);
				table.PreRender += new EventHandler(table_PreRender);
			}
			else {
				controlContainer = new Panel();
				((System.Web.UI.WebControls.PanelStyle)controlContainer.ControlStyle).HorizontalAlign = HorizontalAlign.Left;
				result = controlContainer;
			}
			controlContainer.CssClass = "WebEditorCell";
			((ISupportToolTip)this).SetToolTip(controlContainer, Model);
			controlContainer.Controls.Add(control);
			return result;
		}
		void editor_Load(object sender, EventArgs e) {
			editor.Load -= new EventHandler(editor_Load);
			if(editor.Page != null) {
				ICallbackManagerHolder holder = editor.Page as ICallbackManagerHolder;
				if(holder != null && holder.CallbackManager != null) {
					holder.CallbackManager.PreRenderInternal += new EventHandler<EventArgs>(CallbackManager_PreRenderInternal);
				}
			}
		}
		void CallbackManager_PreRenderInternal(object sender, EventArgs e) {
			((XafCallbackManager)sender).PreRenderInternal -= new EventHandler<EventArgs>(CallbackManager_PreRenderInternal);
			ReadValue();
		}
		protected virtual void SetImmediatePostDataScript(string script) {
		}
		protected virtual void SetImmediatePostDataCompanionScript(string script) {
		}
		private void SetProcessValueChangedScriptCore(string script) {
			if(((IModelMemberViewItemWeb)Model).TrackPropertyChangesOnClient) {
				SetProcessValueChangedScript(script);
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected virtual void SetProcessValueChangedScript(string script) {
		}
		protected override void OnAllowEditChanged() {
			base.OnAllowEditChanged();
			ApplyReadOnly();
		}
		protected virtual void SetupControl(WebControl control) { }
		protected virtual void ApplyReadOnly() {
			if(editor != null) {
				editor.Enabled = AllowEdit;
			}
		}
		protected override void RefreshReadOnly() {
			base.RefreshReadOnly();
			if(ViewEditMode == ExpressApp.Editors.ViewEditMode.Edit) {
				if(MemberInfo != null) {
					bool hasIntermediateNulls = MemberInfo.GetOwnerInstance(CurrentObject) == null;
					AllowEdit["HasIntermediateNulls"] = !hasIntermediateNulls;
				}
				else {
					AllowEdit.RemoveItem("HasIntermediateNulls");
				}
			}
		}
		protected string GetFormattedValue() {
			object val = PropertyValue;
			if(val != null) {
				if(!string.IsNullOrEmpty(DisplayFormat)) {
					return String.Format(DevExpress.Web.Internal.CommonUtils.GetFormatString(DisplayFormat), val);
				}
				else {
					return val.ToString();
				}
			}
			else {
				return String.Empty;
			}
		}
		protected WebPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) { }
		protected virtual string GetEditorClientId() {
			return editor.ClientID;
		}
		protected virtual string GetInplaceViewModeEditorClientId() {
			return inplaceViewModeEditor.ClientID;
		}
		protected virtual void SetControlAlignmentCore(HorizontalAlign alignment) {
			Style style = controlContainer.ControlStyle;
			if(style is System.Web.UI.WebControls.PanelStyle) {
				((System.Web.UI.WebControls.PanelStyle)style).HorizontalAlign = alignment;
			}
			else {
				if(style is TableItemStyle) {
					((TableItemStyle)style).HorizontalAlign = alignment;
				}
			}
		}
		public void SetControlAlignment(HorizontalAlign alignment) {
			SetControlAlignmentCore(alignment);
		}
		protected internal virtual IJScriptTestControl GetEditorTestControlImpl() {
			return new JSTextBoxTestControl();
		}
		protected internal virtual IJScriptTestControl GetInplaceViewModeEditorTestControlImpl() {
			return new JSLabelTestControl();
		}
		protected virtual string GetTestCaption() {
			return this.Caption;
		}
		protected override void ReadValueCore() {
			if((editor == null && ViewEditMode == ViewEditMode.Edit)
				|| (inplaceViewModeEditor == null && ViewEditMode == ViewEditMode.View))
				return;
			if(ViewEditMode == ViewEditMode.View) {
				ReadViewModeValue();
			}
			else {
				ReadEditModeValue();
			}
		}
		public override void BreakLinksToControl(bool unwireEventsOnly) {
			if(editor != null) {
				editor.PreRender -= new EventHandler(editor_PreRender);
				editor.Load -= new EventHandler(editor_Load);
				editor.Unload -= control_Unload;
				if(editor.Page != null) {
					ICallbackManagerHolder holder = editor.Page as ICallbackManagerHolder;
					if(holder != null && holder.CallbackManager != null) {
						holder.CallbackManager.PreRenderInternal -= new EventHandler<EventArgs>(CallbackManager_PreRenderInternal);
					}
				}
			}
			if(inplaceViewModeEditor != null) {
				inplaceViewModeEditor.Unload -= control_Unload;
			}
			if(table != null) {
				table.PreRender -= new EventHandler(table_PreRender);
			}
			if(!unwireEventsOnly) {
				if(editor != null) {
					editor.Dispose();
					editor = null;
				}
				if(table != null) {
					table.Dispose();
					table = null;
				}
				if(controlContainer != null) {
					controlContainer.Dispose();
					controlContainer = null;
				}
				if(inplaceViewModeEditor != null) {
					inplaceViewModeEditor.Dispose();
					inplaceViewModeEditor = null;
				}
			}
			base.BreakLinksToControl(unwireEventsOnly);
		}
		public string SetControlId(string id) {
			string controlId = id + "_" + viewEditMode.ToString();
			if(viewEditMode == ViewEditMode.Edit) {
				if(Editor != null) {
					SetEditorId(controlId);
				}
			}
			else {
				if(InplaceViewModeEditor != null) {
					InplaceViewModeEditor.ID = controlId;
				}
			}
			return controlId;
		}
		protected virtual void SetEditorId(string controlId) {
			Editor.ID = controlId;
		}
		public override object ControlValue {
			get {
				if(Editor != null) {
					return GetControlValueCore();
				}
				return null;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool UseEditorErrorCell {
			get { return useEditorErrorCell; }
			set { useEditorErrorCell = value; }
		}
#if !SL
	[DevExpressExpressAppWebLocalizedDescription("WebPropertyEditorEditor")]
#endif
		public WebControl Editor {
			get { return editor; }
		}
#if !SL
	[DevExpressExpressAppWebLocalizedDescription("WebPropertyEditorGroupName")]
#endif
		public string GroupName {
			get { return groupName; }
			set { groupName = value; }
		}
#if !SL
	[DevExpressExpressAppWebLocalizedDescription("WebPropertyEditorInplaceViewModeEditor")]
#endif
		public WebControl InplaceViewModeEditor {
			get { return inplaceViewModeEditor; }
		}
#if !SL
	[DevExpressExpressAppWebLocalizedDescription("WebPropertyEditorViewEditMode")]
#endif
		public ViewEditMode ViewEditMode {
			get { return viewEditMode; }
			set { viewEditMode = value; }
		}
#if !SL
	[DevExpressExpressAppWebLocalizedDescription("WebPropertyEditorCancelClickEventPropagation")]
#endif
		public virtual bool CancelClickEventPropagation {
			get { return cancelClickEventPropagation; }
			set { cancelClickEventPropagation = value; }
		}
#if !SL
	[DevExpressExpressAppWebLocalizedDescription("WebPropertyEditorTestControl")]
#endif
		public IJScriptTestControl TestControl {
			get {
				IJScriptTestControl result = null;
				if(Editor != null) {
					result = GetEditorTestControlImpl();
					if(Editor is DevExpress.Web.ASPxEditBase) {
						(Editor as DevExpress.Web.ASPxEditBase).EnableClientSideAPI = true;
					}
				}
				else {
					if(InplaceViewModeEditor != null) {
						result = GetInplaceViewModeEditorTestControlImpl();
						if(InplaceViewModeEditor is DevExpress.Web.ASPxEditBase) {
							(InplaceViewModeEditor as DevExpress.Web.ASPxEditBase).EnableClientSideAPI = true;
						}
					}
				}
				return result;
			}
		}
#if !SL
	[DevExpressExpressAppWebLocalizedDescription("WebPropertyEditorClientId")]
#endif
		public string ClientId {
			get {
				if(Editor != null) {
					return GetEditorClientId();
				}
				else {
					if(InplaceViewModeEditor != null) {
						return GetInplaceViewModeEditorClientId();
					}
				}
				return "";
			}
		}
#if !SL
	[DevExpressExpressAppWebLocalizedDescription("WebPropertyEditorTestCaption")]
#endif
		public string TestCaption {
			get { return GetTestCaption(); }
		}
		public virtual TestControlType TestControlType {
			get {
				return TestControlType.Field;
			}
		}
		public static string EmptyValue {
			get {
				return CaptionHelper.NullValueText;
			}
			set {
				CaptionHelper.NullValueText = value;
			}
		}
		protected virtual WebControl GetActiveControl() {
			if(Editor != null) return editor;
			return Control as WebControl;
		}
		#region IAppearanceFormat Members
		FontStyle IAppearanceFormat.FontStyle {
			get { return FontStyle.Regular; }
			set {
				WebControl activeControl = GetActiveControl();
				if(activeControl != null) {
					RenderHelper.SetFontStyle(activeControl, value);
				}
			}
		}
		Color IAppearanceFormat.FontColor {
			get { return GetActiveControl().ForeColor; }
			set {
				WebControl activeControl = GetActiveControl();
				if(activeControl != null) {
					activeControl.ForeColor = value;
				}
			}
		}
		Color IAppearanceFormat.BackColor {
			get { return GetActiveControl().BackColor; }
			set {
				WebControl activeControl = GetActiveControl();
				if(activeControl != null) {
					activeControl.BackColor = value;
				}
			}
		}
		void IAppearanceFormat.ResetFontStyle() {
			WebControl activeControl = GetActiveControl();
			if(activeControl != null) {
				RenderHelper.ResetFontStyle(activeControl);
			}
		}
		void IAppearanceFormat.ResetFontColor() {
			WebControl activeControl = GetActiveControl();
			if(activeControl != null) {
				activeControl.ForeColor = new Color();
			}
		}
		void IAppearanceFormat.ResetBackColor() {
			WebControl activeControl = GetActiveControl();
			if(activeControl != null) {
				activeControl.BackColor = new Color();
			}
		}
		#endregion
		void ISupportToolTip.SetToolTip(object element, IModelToolTip model) {
			if(View != null) {
				WebControl webControl = element as WebControl;
				if(model != null && webControl != null) {
					if(!string.IsNullOrEmpty(model.ToolTip)) {
						webControl.ToolTip = model.ToolTip;
					}
				}
			}
		}
		void ISupportImmediatePostData.SetImmediatePostDataCompanionScript(string script) {
			SetImmediatePostDataCompanionScript(script);
		}
		void ISupportImmediatePostData.SetImmediatePostDataScript(string script) {
			SetImmediatePostDataScript(script);
		}
		bool ISupportImmediatePostData.ImmediatePostData {
			get { return ImmediatePostData; }
		}
#if DebugTest
		public void DebugTest_SetEditor(DevExpress.Web.ASPxWebControl control) {
			this.editor = control;
		}
#endif
	}
}
