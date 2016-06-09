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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.EditForm;
using DevExpress.XtraGrid.EditForm.Helpers.Controls;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Localization;
using DevExpress.Skins;
namespace DevExpress.XtraGrid.EditForm.Helpers {
	public enum EditFormExitAction { CancelExit, CancelResult, SaveResult }
	public class EditFormData : IDisposable {
		int rowHandle = GridControl.InvalidRowHandle;
		public EditFormContainer RootPanel { get; set; }
		public DXErrorProvider ErrorProvider { get; set; }
		public XtraForm PopupForm { get; set; }
		public int ProposedHeight { get; set; }
		public EditFormBindableControlsCollection BindableControls { get; set; }
		public Control ButtonsPanel { get; set; }
		public Control EditorPanel { get; set; }
		public bool ButtonsPanelVisible { get; set; }
		public ValuesCache Cache { get; set; }
		EditFormDataProxyCore dataProxy;
		public EditFormDataProxyCore DataProxy {
			get { return dataProxy; }
			set {
				if(DataProxy == value) return;
				if(dataProxy != null) dataProxy.Dispose();
				dataProxy = value; 
			}
		}
		public int RowHandle { get { return rowHandle; } set { rowHandle = value; }  }
		public virtual void Dispose() {
			if(RootPanel != null) RootPanel.Dispose();
			if(BindableControls != null) BindableControls.Clear();
			if(ErrorProvider != null) ErrorProvider.Dispose();
			if(Cache != null) Cache.Clear();
			DataProxy = null;
			RootPanel = null;
			ErrorProvider = null;
			PopupForm = null;
		}
	}
	public class EditFormController : IDisposable {
		GridView view;
		LayoutBuilder builder;
		EditFormData editForm;
		EditFormBindingMode bindingMode = EditFormBindingMode.Default;
		bool isModified;
		public event EventHandler CloseEditForm;
		public EditFormController(GridView view) {
			this.view = view;
		}
		public EditFormBindingMode BindingMode {
			get {
				if(bindingMode == EditFormBindingMode.Default) 
					return View.OptionsEditForm.BindingMode;
				return bindingMode;
			}
			set { bindingMode = value; }
		}
		public bool IsModified {
			get { return isModified; }
			set { isModified = value; }
		}
		public GridView View { get { return view; } }
		protected LayoutBuilder Builder {
			get {
				if(builder == null) builder = CreateLayoutBuilder();
				return builder;
			}
		}
		protected virtual void DestroyFormInfo() {
			if(EditForm == null) return;
			HideForm();
			if(View != null && View.OptionsEditForm.CustomEditFormLayout != null) View.OptionsEditForm.CustomEditFormLayout.Parent = null;
			EditForm.Dispose();
			this.editForm = null;
		}
		protected virtual bool IsEditFormCreated {
			get { return editForm != null; }
		}
		protected virtual void EnsureEditForm() {
			if(IsEditFormCreated) return;
			this.editForm = CreateEditFormCore();
		}
		public EditFormBindableControlsCollection BindableControls { get { return EditForm == null ? null : EditForm.BindableControls; } }
		protected internal virtual EditFormData EditForm { get { return editForm; } }
		protected virtual ValuesCache CreateValuesCache() {
			return new ValuesCache();
		}
		protected virtual LayoutBuilder CreateLayoutBuilder() {
			return new LayoutBuilder(View);
		}
		protected virtual bool HasSavedValues { get { return EditForm != null && EditForm.RowHandle != GridControl.InvalidRowHandle; } }
		public virtual XtraForm CreateEditForm(int rowHandle) {
			IsModified = false;
			var panel = PrepareEditFormControlCore(rowHandle, true);
			if(EditForm == null) return null;
			XtraForm form = new XtraForm();
			form.RightToLeft = View.IsRightToLeft ? RightToLeft.Yes : RightToLeft.No; ;
			EditForm.PopupForm = form;
			form.ShowInTaskbar = false;
			form.ShowIcon = false;
			form.Text = PrepareCaption(rowHandle);
			form.LookAndFeel.Assign(View.ElementsLookAndFeel);
			form.ClientSize = new Size(View.OptionsEditForm.PopupEditFormWidth, panel.Height);
			FormClosingEventHandler closingHandler = (s, e) => {
				if(e.CloseReason == CloseReason.UserClosing && form.IsHandleCreated) {
					e.Cancel = true;
					OnCancelButtonClick();
				}
			};
			form.FormClosing += closingHandler;
			panel.Parent = null;
			panel.Visible = true;
			panel.Dock = DockStyle.Fill;
			panel.BindingContext = null;
			form.Controls.Add(panel);
			EventHandler handler = null;
			handler = (s, e) => {
				CloseEditForm -= handler;
				form.FormClosing -= closingHandler;
				Form gridForm = View.GridControl == null ? null : View.GridControl.FindForm();
				form.Controls.Clear();
				form.Close();
				if(gridForm != null && gridForm.IsHandleCreated) {
					gridForm.BeginInvoke(new MethodInvoker(() => {
						form.Dispose();
					}));
				}
				else {
					form.Dispose();
				}
				View.Focus();
			};
			CloseEditForm += handler;
			return form;
		}
		public virtual int ProposedEditFormHeight { get { return EditForm == null ? 0 : EditForm.ProposedHeight; } }
		protected virtual bool ShowButtonsPanel { get { return View.OptionsEditForm.ShowUpdateCancelPanel != DefaultBoolean.False; } }
		protected virtual EditFormData CreateEditFormCore() {
			EditFormContainer rootPanel = new EditFormContainer(this);
			rootPanel.RightToLeft = View.IsRightToLeft ? RightToLeft.Yes : RightToLeft.No;
			rootPanel.Padding = new Padding(16, 10, 16, 0);
			rootPanel.LookAndFeel.Assign(View.ElementsLookAndFeel);
			if(rootPanel.LookAndFeel.ActiveStyle == LookAndFeel.ActiveLookAndFeelStyle.Skin) {
				rootPanel.BackColor = LookAndFeel.LookAndFeelHelper.GetSystemColorEx(rootPanel.LookAndFeel, SystemColors.Control);
			}
			Control editorPanel = null;
			if(View.OptionsEditForm.CustomEditFormLayout != null) 
				editorPanel = View.OptionsEditForm.CustomEditFormLayout;
			else
				editorPanel = Builder.GenerateLayoutControl();
			int proposedHeight = editorPanel.Height;
			editorPanel.Dock = DockStyle.Top;
			if(editorPanel is EditFormUserControl) {
				((EditFormUserControl)editorPanel).SetMenuManager(View.GridControl.MenuManager);
			}
			XtraScrollableControl xs = new XtraScrollableControlX() { Dock = DockStyle.Fill };
			xs.AutoScroll = true;
			xs.Controls.Add(editorPanel);
			rootPanel.Controls.Add(xs);
			int buttonsPanelHeight = 0;
			var pc = CreateButtonsPanel();
			bool buttonsPanelVisible = true;
			rootPanel.Controls.Add(pc);
			xs.Name = "EditorContainer";
			pc.Name = "ButtonPanel";
			if(ShowButtonsPanel) {
				buttonsPanelHeight = pc.Height;
			}
			else {
				pc.Visible = buttonsPanelVisible = false;
			}
			DXErrorProvider provider = CreateErrorProvider();
			provider.ContainerControl = rootPanel;
			EditFormBindableControlsCollection bindableControls = GetBindableControls(editorPanel);
			rootPanel.Disposed += (s, e) => {
				if(editForm != null) {
					editForm = null;
				}
			};
			proposedHeight += buttonsPanelHeight;
			proposedHeight += rootPanel.Margin.Vertical + rootPanel.Padding.Vertical;
			rootPanel.Height = proposedHeight;
			EnsureErrorProvider(bindableControls, provider);
			return new EditFormData() { BindableControls = bindableControls, RootPanel = rootPanel, ErrorProvider = provider, 
				ProposedHeight = proposedHeight, ButtonsPanel = pc, ButtonsPanelVisible = buttonsPanelVisible, EditorPanel = editorPanel};
		}
		public virtual void FocusFirst() {
			if(EditForm == null) return;
			var control = EditForm.BindableControls.Where(x => x.Visible).OrderBy(x => x.TabIndex).FirstOrDefault();
			if(control != null) {
				control.Focus();
				this.View.RaiseEditFormPrepared(EditForm.RootPanel);
				return;
			}
			EditForm.RootPanel.Focus();
			this.View.RaiseEditFormPrepared(EditForm.RootPanel);
		}
		protected virtual void EnsureErrorProvider(EditFormBindableControlsCollection bindableControls, DXErrorProvider provider) {
			foreach(Control c in bindableControls) {
			}
		}
		static Regex captionRegex = new Regex(@"{(?<Field>[\w\s]+)}", RegexOptions.Compiled);
		public string PrepareCaption(int rowHandle) {
			return captionRegex.Replace(View.OptionsEditForm.FormCaptionFormat, new MatchEvaluator((m) => {
				var g = m.Groups["Field"];
				if(g != null && g.Success) return View.GetRowCellDisplayText(rowHandle, g.Value);
				return "";
			}));
		}
		public UserControl PrepareEditFormControl(int rowHandle) {
			return PrepareEditFormControlCore(rowHandle, false);
		}
		protected UserControl PrepareEditFormControlCore(int rowHandle, bool popup) {
			if(!View.IsAllowEditForm(rowHandle)) return null;
			HideForm();
			EnsureEditForm();
			if(popup) {
				if(!EditForm.ButtonsPanelVisible) {
					EditForm.ButtonsPanel.Visible = EditForm.ButtonsPanelVisible = true;
					EditForm.ProposedHeight += EditForm.ButtonsPanel.Height;
					EditForm.RootPanel.Height = EditForm.ProposedHeight;
				}
			}
			else {
				if(!ShowButtonsPanel && EditForm.ButtonsPanelVisible) {
					EditForm.ButtonsPanel.Visible = EditForm.ButtonsPanelVisible = false;
					EditForm.ProposedHeight -= EditForm.ButtonsPanel.Height;
					EditForm.RootPanel.Height = EditForm.ProposedHeight;
				}
			}
			IsModified = false;
			ValuesCache cache = CreateValuesCache();
			cache.SaveValues(View, rowHandle);
			EditForm.PopupForm = null;
			EditForm.ErrorProvider.ClearErrors();
			EditForm.RowHandle = rowHandle;
			EditForm.Cache = cache;
			EditForm.DataProxy = CreateDataProxy(rowHandle);
			var bs = new BindingSource(EditForm.DataProxy, "");
			BindControls(EditForm.EditorPanel, EditForm.BindableControls, bs);
			EditForm.ErrorProvider.DataSource = bs;
			EditForm.ErrorProvider.UpdateBinding();
			if(EditForm.BindableControls.FirstOrDefault() != null) EditForm.BindableControls.FirstOrDefault().Focus();
			EditForm.RootPanel.EnableValidation();
			return EditForm.RootPanel;
		}
		protected virtual EditFormDataProxyCore CreateDataProxy(int rowHandle) {
			if(BindingMode == EditFormBindingMode.Default || BindingMode == EditFormBindingMode.Direct) {
				return new EditFormDataProxyGridDirect(this, rowHandle);
			}
			return new EditFormDataProxyGridBuffered(new ValuesCache().PullValues(View, rowHandle, true), this, rowHandle);
		}
		IEditorFormTagProvider GeTagProvider(Control ownerPanel) {
			IEditorFormTagProvider res = ownerPanel as IEditorFormTagProvider;
			if(res == null) res = DefaultEditFormTagProvider.Default;
			return res;
		}
		protected virtual void BindControls(Control editorsPanel, EditFormBindableControlsCollection bindableControls, object proxy) {
			foreach(var c in bindableControls) {
				string field = GeTagProvider(editorsPanel).GetFieldName(c);
				string property = GeTagProvider(editorsPanel).GetPropertyName(c);
				c.DataBindings.Clear();
				if(View.Columns.ColumnByFieldName(field) == null) continue;
				if(!string.IsNullOrEmpty(field)) c.DataBindings.Add(property, proxy, FieldNameConverter(field), true);
			}
		}
		internal static string FieldNameConverter(string field) {
			if(string.IsNullOrEmpty(field)) return field;
			return field.Replace(".", " dOt ");
		}
		internal static string FromFieldNameConverter(string field) {
			if(string.IsNullOrEmpty(field)) return field;
			return field.Replace(" dOt ", ".");
		}
		protected virtual EditFormBindableControlsCollection GetBindableControls(Control panel) {
			EditFormBindableControlsCollection res = new EditFormBindableControlsCollection();
			return GetBindableControls(panel, res, panel);
		}
		EditFormBindableControlsCollection GetBindableControls(Control editorsPanel, EditFormBindableControlsCollection res, Control panel) {
			IEditorFormTagProvider tag = GeTagProvider(editorsPanel);
			if(panel.HasChildren) {
				foreach(Control c in panel.Controls) {
					GetBindableControls(editorsPanel, res, c);
				}
			}
			string field = tag.GetFieldName(panel);
			if(!string.IsNullOrEmpty(field)) res.AddControlEx(panel, field);
			return res;
		}
		protected virtual DXErrorProvider CreateErrorProvider() {
			return new DXErrorProvider();
		}
		public void CancelValues() {
			if(IsUpdatingValues) return;
			try {
				this.updatingValues++;
				if(!HasSavedValues) return;
				EditForm.RootPanel.DisableValidation();
				EditForm.DataProxy.Enabled = false;
				bool newItemRow = View.IsNewItemRow(EditForm.RowHandle);
				if(!newItemRow) {
					EditForm.Cache.RestoreValues(View, EditForm.RowHandle);
				}
				View.ViewInfo.allowEditFormHeight = false;
				View.EditFormAllowAdjustTopRowIndex = false;
				try {
					View.CancelUpdateCurrentRow();
				}
				finally {
					View.ViewInfo.allowEditFormHeight = true;
					View.EditFormAllowAdjustTopRowIndex = true;
				}
				HideForm();
				View.CheckLayoutEditFormClose();
			}
			finally {
				this.updatingValues--;
			}
		}
		public bool CloseForm() {
			View.BeginLockFocusedRowChange();
			try {
				if(UpdateValues()) {
					HideForm();
					return true;
				}
				return false;
			}
			finally {
				View.EndLockFocusedRowChange();
				View.FocusedRowHandle = View.DataController.CurrentControllerRow;
			}
		}
		public bool Validate() {
			if(EditForm == null) return true;
			if(!EditForm.RootPanel.ValidateChildren()) return false;
			return true;
		}
		int updatingValues = 0;
		protected bool UpdateValues() {
			if(IsUpdatingValues) return false;
			if(EditForm == null) return false;
			bool hasValidateErrors = false;
			try {
				this.updatingValues++;
				RefreshErrorInfo();
				if(!Validate()) {
					hasValidateErrors = true;
					return false;
				}
				return UpdateValuesCore();
			}
			finally {
				if(!hasValidateErrors) RefreshErrorInfo();
				this.updatingValues--;
			}
		}
		bool UpdateValuesCore() {
			View.BeginLockFocusedRowChange();
			try {
				EditForm.DataProxy.PushData();
				if(View.UpdateCurrentRow()) {
					return true;
				}
			}
			finally {
				View.EndLockFocusedRowChange();
				View.FocusedRowHandle = View.DataController.CurrentControllerRow;
			}
			return false;
		}
		protected internal bool IsUpdatingValues { get { return updatingValues != 0; } }
		protected virtual void RefreshErrorInfo() {
			if(EditForm != null) EditForm.ErrorProvider.RefreshControls();
		}
		protected virtual void OnCloseEditForm() {
			if(EditForm != null && View != null && View.GridControl != null) {
				DevExpress.XtraEditors.Container.ContainerHelper.UpdateUnvalidatedControl(EditForm.RootPanel, View.GridControl.GetContainerControl() as ContainerControl, null);
				EditForm.RootPanel.Visible = false;
			}
			if(CloseEditForm != null) CloseEditForm(this, EventArgs.Empty);
		}
		protected virtual Control CreateButtonsPanel() {
			PanelControl pc = new PanelControl() { Dock = DockStyle.Bottom, Height = 40, BorderStyle = XtraEditors.Controls.BorderStyles.NoBorder };
			Control splitter = new LabelControl() { LineLocation = XtraEditors.LineLocation.Center, AutoSizeMode = LabelAutoSizeMode.None, Dock = DockStyle.Top, LineVisible = true };
			pc.Controls.Add(splitter);
			SimpleButton ok;
			SimpleButton cancel;
			int buttonY = splitter.Bottom + 4;
			pc.Controls.Add(ok = new SimpleButton() { Text = Localization.GridLocalizer.Active.GetLocalizedString(GridStringId.EditFormUpdateButton), Left = 30, Top = buttonY, Anchor = AnchorStyles.Right | AnchorStyles.Top, Tag = "OkButton" });
			pc.Controls.Add(cancel = new EditFormCancelButton() { Text = Localization.GridLocalizer.Active.GetLocalizedString(GridStringId.EditFormCancelButton), Left = 120, Top = buttonY, Anchor = AnchorStyles.Right | AnchorStyles.Top, Tag = "CancelButton"});
			Size szOk = ok.CalcBestSize();
			Size szCancel = cancel.CalcBestSize();
			Size bestSize = new Size(Math.Max(szOk.Width, szCancel.Width), Math.Max(szOk.Height, szCancel.Height));
			bestSize = new Size(Math.Max(bestSize.Width, cancel.Width), Math.Max(bestSize.Height, cancel.Height));
			ok.Size = bestSize;
			cancel.Size = bestSize;
			if(cancel.Right + 10 > pc.Width) {
				cancel.Left -= (cancel.Right + 10 - pc.Width);
			}
			if(ok.Right + 10 > cancel.Left) {
				ok.Left -= (ok.Right + 10 - cancel.Left);
			}
			ok.Click += (s, e) => {
				OnUpdateButtonClick();
			};
			cancel.Click += (s, e) => {
				OnCancelButtonClick();
			};
			pc.Height = ok.Height + splitter.Height * 1 + 14;
			return pc;
		}
		protected virtual void OnCancelButtonClick() {
			CancelValues();			
		}
		protected virtual void OnUpdateButtonClick() {
			try {
				CloseForm();
			}
			catch(Exception e) {
				OnUpdateError(e);
			}
		}
		protected virtual void OnUpdateError(Exception e) {
			XtraMessageBox.Show(e.Message, Localization.GridLocalizer.Active.GetLocalizedString(GridStringId.WindowErrorCaption), MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
		protected virtual void HideForm() {
			OnCloseEditForm();
		}
		public virtual void Dispose() {
			this.CloseEditForm = null;
			DestroyFormInfo();
		}
		protected internal virtual void OnModified() {
			IsModified = true;
		}
		public virtual EditFormExitAction AskModifiedCancel() {
			if(IsUpdatingValues) return EditFormExitAction.SaveResult;
			if(!Validate()) return EditFormExitAction.CancelExit;
			if(!IsModified) return EditFormExitAction.CancelResult;
			if(View.OptionsEditForm.ActionOnModifiedRowChange == EditFormModifiedAction.Cancel) {
				return XtraMessageBox.Show(EditForm.RootPanel.FindForm(), Localization.GridLocalizer.Active.GetLocalizedString(GridStringId.EditFormCancelMessage), 
					Localization.GridLocalizer.Active.GetLocalizedString(GridStringId.WindowWarningCaption), MessageBoxButtons.YesNo) == DialogResult.Yes ? EditFormExitAction.CancelResult : EditFormExitAction.CancelExit;
			}
			DialogResult res = XtraMessageBox.Show(EditForm.RootPanel.FindForm(), Localization.GridLocalizer.Active.GetLocalizedString(GridStringId.EditFormSaveMessage),
				Localization.GridLocalizer.Active.GetLocalizedString(GridStringId.WindowWarningCaption), MessageBoxButtons.YesNoCancel);
			if(res == DialogResult.Yes) return EditFormExitAction.SaveResult;
			if(res == DialogResult.Cancel) return EditFormExitAction.CancelExit;
			return EditFormExitAction.CancelResult;
		}
		protected internal virtual void OnModified(GridColumn column) {
		}
		internal EditFormValidateEditorEventArgs RaiseValidateEditor(int row, GridColumn column, object value) {
			EditFormValidateEditorEventArgs veArgs = CreateValidateEventArgs(column, value);
			veArgs.TryValidateViaColumnAnnotationAttributes();
			View.RaiseValidatingEditor(veArgs);
			if(!veArgs.Valid) {
				if(string.IsNullOrEmpty(veArgs.ErrorText)) veArgs.ErrorText = Localizer.Active.GetLocalizedString(StringId.InvalidValueText);
			}
			return veArgs;
		}
		protected virtual EditFormValidateEditorEventArgs CreateValidateEventArgs(GridColumn column, object value) {
			return new EditFormValidateEditorEventArgs(column, value);
		}
		internal void ValidateCachedValue(int row, GridColumn column, object value) {
			EditFormValidateEditorEventArgs bc = RaiseValidateEditor(row, column, value);
			if(!bc.Valid) throw new Exception(bc.ErrorText);
		}
		internal void SetRowCellValue(int row, GridColumn column, object value) {
			EditFormValidateEditorEventArgs bc = RaiseValidateEditor(row, column, value);
			if(!bc.Valid) {
				View.SetColumnError(column, bc.ErrorText);
				RefreshErrorInfo();
				throw new Exception(bc.ErrorText);
			}
			else {
				View.SetColumnError(column, null);
				value = bc.Value;
			}
			View.SetFocusedRowModified();
			View.SetRowCellValue(row, column, value);
			OnModified(column);
			RefreshBinding(column);
		}
		void RefreshBinding(GridColumn column) {
			if(EditForm == null) return;
			foreach(var c in EditForm.BindableControls) {
				foreach(Binding db in c.DataBindings) {
					if(FromFieldNameConverter(db.BindingMemberInfo.BindingField) == column.FieldName) {
						db.ReadValue();
					}
				}
			}
		}
		public void RefreshValues() {
			foreach(GridColumn column in View.Columns) RefreshBinding(column);
		}
	}
	public class EditFormBindableControlsCollection : DXCollectionBase<Control> {
		Dictionary<string, Control> fields = new Dictionary<string, Control>();
		public Control this[GridColumn column] { get { return this[column.FieldName]; } }
		public Control this[string fieldName] {
			get {
				if(fields.ContainsKey(fieldName)) return fields[fieldName];
				return null;
			}
		}
		protected internal void AddControlEx(Control control, string fieldName) {
			fields[fieldName] = control;
			Add(control);
		}
	}
}
namespace DevExpress.XtraGrid.Views.Grid {
	public class EditFormValidateEditorEventArgs : BaseContainerValidateEditorEventArgs {
		GridColumn column;
		public EditFormValidateEditorEventArgs(GridColumn column, object value) : base(value) {
			this.column = column;
		}
		public GridColumn Column { get { return column; } }
		protected internal void TryValidateViaColumnAnnotationAttributes() {
			if(column != null)
				TryValidateViaAnnotationAttributes(Value, column.ColumnAnnotationAttributes);
		}
	}
}
