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
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.ExpressApp.Web.Templates.ActionContainers {
	[ToolboxItem(false)]
	public abstract class ParametrizedActionControl : Table, INamingContainer, IDisposableExt {
		private EditButton actButton;
		private ASPxButtonEditBase editor;
		private ASPxLabel label;
		private TableCell labelCell;
		private bool isPrerendered;
		private Boolean isDisposed;
		private bool clientEnabled = true;
		private void UpdateEnabled() {
			if(Editor != null) {
				Editor.ClientEnabled = ClientEnabled;
			}
		}
		protected virtual string GetForceButtonClickScript(string buttonClickHandler) {
			return "function(s, e) {  " + RenderHelper.GetForceButtonClickFunctionName() + "(s, e, " + buttonClickHandler + "); return false;}";
		}
		protected virtual string GetEditorClearButtonClickScript(string clientClickHandler) {
			return "function(s, e) { if(e.buttonIndex === 0) { s.SetText(''); } (" + clientClickHandler + ")(s, e); }";
		}
		protected virtual string GetClientControlClassName() {
			return "ParametrizedActionClientControl";
		}
		protected abstract ASPxButtonEditBase CreateASPxEditor();
		protected override void OnPreRender(EventArgs e) {
			isPrerendered = true;
			base.OnPreRender(e);
		}
		protected override void Render(HtmlTextWriter writer) {
			if(!isPrerendered) {
				OnPreRender(EventArgs.Empty);
			}
			base.Render(writer);
			RenderUtils.WriteScriptHtml(writer, @"window." + ClientID + @" =  new " + GetClientControlClassName() + "('" + ClientID + "');");
		}
		protected virtual void OnClick() {
			if(Click != null) {
				Click(this, EventArgs.Empty);
			}
		}
		public override void Focus() {
			if(Editor != null) {
				Editor.Focus();
			}
			else {
				base.Focus();
			}
		}
		public void SetImage(DevExpress.ExpressApp.Utils.ImageInfo imageInfo, string buttonText) {
			if(!imageInfo.IsEmpty) {
				ASPxImageHelper.SetImageProperties(ActButton.Image, imageInfo);
				ActButton.Text = "";
				CssClass = "ParametrizedActionWithImage";
			}
			else {
				ASPxImageHelper.ClearImageProperties(ActButton.Image);
				ActButton.Text = buttonText;
				CssClass = "ParametrizedAction";
			}
		}
		public ParametrizedActionControl() : this(ActionContainerOrientation.Horizontal) { }
		public ParametrizedActionControl(ActionContainerOrientation orientation) {
			editor = CreateASPxEditor();
			actButton = editor.Buttons.Add();
			actButton.Index = editor.Buttons.Count;
			editor.JSProperties["cpActButtonIndex"] = actButton.Index;
			editor.ID = "Ed";
			editor.EnableClientSideAPI = true;
			label = RenderHelper.CreateASPxLabel();
			label.ID = "L";
			label.Wrap = DevExpress.Utils.DefaultBoolean.False;
			this.CssClass = "ParametrizedActionControl";
			this.ID = "T";
			labelCell = new TableCell();
			TableCell editorCell = new TableCell();
			FillTemplateTable(orientation, this, labelCell, editorCell);
			labelCell.Controls.Add(label);
			labelCell.CssClass = "ControlCaption";
			editorCell.Controls.Add(editor);
			editorCell.CssClass = "Label";
		}
		protected Table FillTemplateTable(ActionContainerOrientation orientation, Table table, TableCell labelCell, TableCell editorCell) {
			if(orientation == ActionContainerOrientation.Horizontal) {
				return FillHTemplateTable(table, labelCell, editorCell);
			}
			else {
				return FillVTemplateTable(table, labelCell, editorCell);
			}
		}
		protected virtual Table FillHTemplateTable(Table table, TableCell labelCell, TableCell editorCell) {
			table.Rows.Add(new TableRow());
			table.Rows[0].Cells.Add(labelCell);
			table.Rows[0].Cells.Add(editorCell);
			return table;
		}
		protected virtual Table FillVTemplateTable(Table table, TableCell labelCell, TableCell editorCell) {
			table.Rows.Add(new TableRow());
			table.Rows[0].Cells.Add(labelCell);
			table.Rows.Add(new TableRow());
			table.Rows[1].Cells.Add(editorCell);
			return table;
		}
		public abstract void SetNullValuePrompt(string nullValuePrompt);
		public override void Dispose() {
			base.Dispose();
			editor = null;
			isDisposed = true;
		}
		public static ParametrizedActionControl CreateControl(Type valueType) {
			return CreateControl(valueType, ActionContainerOrientation.Horizontal);
		}
		public static ParametrizedActionControl CreateControl(Type valueType, ActionContainerOrientation orientation) {
			ParametrizedActionControl result = null;
			if(valueType == typeof(string)) {
				result = new ParametrizedActionTextBoxControl(orientation);
			}
			if(valueType == typeof(int)) {
				result = new ParametrizedActionSpinEditControl(orientation);
			}
			if(valueType == typeof(DateTime)) {
				result = new ParametrizedActionDateEditControl(orientation);
			}
			return result;
		}
		public bool ClientEnabled {
			get { return clientEnabled; }
			set {
				clientEnabled = value;
				UpdateEnabled();
			}
		}
		public override string ToolTip {
			get { return actButton.ToolTip; }
			set { actButton.ToolTip = value; }
		}
		public EditButton ActButton {
			get { return actButton; }
		}
		public ASPxButtonEditBase Editor {
			get {
				Guard.NotDisposed(this);
				return editor;
			}
		}
		public new string Caption {
			get { return label.Text; }
			set {
				label.Text = value;
				CaptionVisible = !String.IsNullOrEmpty(value);
			}
		}
		public bool CaptionVisible {
			get { return labelCell.Visible; }
			set { labelCell.Visible = value; }
		}
		public virtual object Value {
			get { return Editor.Value; }
			set { Editor.Value = value; }
		}
		public event EventHandler Click;
		#region IDisposableExt Members
		public bool IsDisposed {
			get { return isDisposed; }
		}
		#endregion
		public virtual void SetButtonClickScript(string clientClickHandler) {
			Editor.SetClientSideEventHandler("KeyPress", GetForceButtonClickScript(clientClickHandler));
			Editor.SetClientSideEventHandler("ButtonClick", clientClickHandler);
		}
	}
	[ToolboxItem(false)]
	public class ParametrizedActionSpinEditControl : ParametrizedActionControl {
		protected override ASPxButtonEditBase CreateASPxEditor() {
			ASPxSpinEdit editor = new ASPxSpinEdit();
			editor.CustomButtonsPosition = CustomButtonsPosition.Far;
			editor.Width = Unit.Pixel(250);
			editor.NumberType = SpinEditNumberType.Integer;
			return editor;
		}
		public new ASPxSpinEdit Editor {
			get { return (ASPxSpinEdit)base.Editor; }
		}
		public override object Value {
			get { return base.Value == null ? 0 : decimal.ToInt32((decimal)base.Value); }
			set { base.Value = value == null ? 0 : value; }
		}
		public ParametrizedActionSpinEditControl() { }
		public ParametrizedActionSpinEditControl(ActionContainerOrientation orientation) : base(orientation) { }
		public override void SetNullValuePrompt(string nullValuePrompt) {
			Editor.NullText = nullValuePrompt;
		}
	}
	[ToolboxItem(false)]
	public class ParametrizedActionDateEditControl : ParametrizedActionControl {
		protected override ASPxButtonEditBase CreateASPxEditor() {
			ASPxDateEdit editor = new ASPxDateEdit();
			editor.CustomButtonsPosition = CustomButtonsPosition.Far;
			editor.Width = Unit.Pixel(250);
			return editor;
		}
		public new ASPxDateEdit Editor {
			get { return (ASPxDateEdit)base.Editor; }
		}
		public ParametrizedActionDateEditControl() { }
		public ParametrizedActionDateEditControl(ActionContainerOrientation orientation) : base(orientation) { }
		public override void SetNullValuePrompt(string nullValuePrompt) {
			Editor.NullText = nullValuePrompt;
		}
	}
	[ToolboxItem(false)]
	public class ParametrizedActionTextBoxControl : ParametrizedActionControl {
		public override void SetButtonClickScript(string clientClickHandler) {
			base.SetButtonClickScript(clientClickHandler);
			Editor.SetClientSideEventHandler("ButtonClick", GetEditorClearButtonClickScript(clientClickHandler));
		}
		protected override ASPxButtonEditBase CreateASPxEditor() {
			ASPxButtonEdit editor = new ASPxButtonEdit();
			editor.Width = Unit.Pixel(250);
			EditButton button = new EditButton();
			ASPxImageHelper.SetImageProperties(button.Image, ImageLoader.Instance.GetSmallImageInfo("Action_ParametrizedAction_Clear"));
			editor.Buttons.Add(button);
			editor.ClientSideEvents.Init = "function(s,e) { var button = s.GetButton(0); button.style.backgroundColor= 'White'; button.style.backgroundImage = 'none'; button.style.borderStyle='none'; button.style.display = s.GetText() !== '' ? '' : 'none'; }";
			editor.ClientSideEvents.KeyUp = "function(s,e) { s.GetButton(0).style.display = s.GetText() !== '' ? '' : 'none'; }";
			editor.JSProperties["cpClearButtonIndex"] = button.Index;
			return editor;
		}
		public ParametrizedActionTextBoxControl() { }
		public ParametrizedActionTextBoxControl(ActionContainerOrientation orientation) : base(orientation) { }
		public new ASPxButtonEdit Editor {
			get { return (ASPxButtonEdit)base.Editor; }
		}
		public override object Value {
			get { return base.Value == null ? string.Empty : base.Value; }
			set { base.Value = value == null ? string.Empty : value; }
		}
		public override void SetNullValuePrompt(string nullValuePrompt) {
			Editor.NullText = nullValuePrompt;
		}
	}
}
