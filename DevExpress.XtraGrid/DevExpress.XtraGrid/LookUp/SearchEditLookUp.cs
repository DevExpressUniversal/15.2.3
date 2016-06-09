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

using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Editors;
using DevExpress.XtraGrid.Views.Base;
using System.Windows.Forms;
using DevExpress.XtraEditors.Controls;
using System.Collections;
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Localization;
using System;
namespace DevExpress.XtraEditors.Repository {
	[Designer("DevExpress.XtraGrid.Design.GridLookUpEditRepositoryItemDesigner, " + AssemblyInfo.SRAssemblyGridDesign)
	]
	[DevExpress.Utils.Design.DataAccess.DataAccessMetadata("All", SupportedProcessingModes = "SearchLookUp", EnableDirectBinding = false)]
	[LookupEditCustomBindingProperties("SearchLookUpEdit")]
	public class RepositoryItemSearchLookUpEdit : RepositoryItemGridLookUpEditBase {
		private static readonly object addNewValue = new object();
		SearchEditLookUpPopup lookUpPopup;
		FindMode popupFindMode = FindMode.Default;
		bool showAddNewButton, showClearButton;
		static RepositoryItemSearchLookUpEdit() {
			RegisterSearchLookUpEdit();
		}
		public RepositoryItemSearchLookUpEdit() {
			this.showAddNewButton = false;
			this.showClearButton = true;
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(lookUpPopup != null) lookUpPopup.Dispose();
			}
			base.Dispose(disposing);
		}
		public static void RegisterSearchLookUpEdit() {
			if(EditorRegistrationInfo.Default.Editors.Contains("SearchLookUpEdit")) return;
			Image img = null;
			EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo("SearchLookUpEdit", typeof(SearchLookUpEdit), typeof(RepositoryItemSearchLookUpEdit), typeof(DevExpress.XtraEditors.ViewInfo.SearchLookUpEditBaseViewInfo), new DevExpress.XtraEditors.Drawing.ButtonEditPainter(), true, img, typeof(DevExpress.Accessibility.PopupEditAccessible)));
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new SearchLookUpEdit OwnerEdit { get { return base.OwnerEdit as SearchLookUpEdit; } }
#if !SL
	[DevExpressXtraGridLocalizedDescription("RepositoryItemSearchLookUpEditEditorTypeName")]
#endif
		public override string EditorTypeName { get { return "SearchLookUpEdit"; } }
		[DXCategory(CategoryName.Data), 
#if !SL
	DevExpressXtraGridLocalizedDescription("RepositoryItemSearchLookUpEditPopupFindMode"),
#endif
 DefaultValue(FindMode.Default)]
		public FindMode PopupFindMode {
			get { return popupFindMode; }
			set {
				if(PopupFindMode == value) return;
				popupFindMode = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraGridLocalizedDescription("RepositoryItemSearchLookUpEditShowAddNewButton"),
#endif
 DefaultValue(false)]
		public bool ShowAddNewButton {
			get { return showAddNewButton; }
			set {
				if(ShowAddNewButton == value) return;
				showAddNewButton = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraGridLocalizedDescription("RepositoryItemSearchLookUpEditShowClearButton"),
#endif
 DefaultValue(true)]
		public bool ShowClearButton {
			get { return showClearButton; }
			set {
				if(ShowClearButton == value) return;
				showClearButton = value;
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("RepositoryItemSearchLookUpEditAddNewValue"),
#endif
 Category(CategoryName.Events)]
		public event AddNewValueEventHandler AddNewValue {
			add { this.Events.AddHandler(addNewValue, value); }
			remove { this.Events.RemoveHandler(addNewValue, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new event ProcessNewValueEventHandler ProcessNewValue {
			add {  }
			remove { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool ImmediatePopup {
			get { return false; }
			set { }
		}
		[Category(CategoryName.Behavior), 
#if !SL
	DevExpressXtraGridLocalizedDescription("RepositoryItemSearchLookUpEditTextEditStyle"),
#endif
 DefaultValue(TextEditStyles.DisableTextEditor)]
		public override TextEditStyles TextEditStyle {
			get { return base.TextEditStyle; }
			set {
				if(value == TextEditStyles.Standard) value = TextEditStyles.DisableTextEditor;
				base.TextEditStyle = value;
			}
		}
		protected override bool CancelPopupInputOnButtonClose { get { return true; } }
		object currentDataSource = null;
		protected override bool AllowAssignDataSource { get { return false; } }
		public override void Assign(RepositoryItem item) {
			RepositoryItemSearchLookUpEdit source = item as RepositoryItemSearchLookUpEdit;
			BeginUpdate();
			try {
				if(source != null) {
					this.DisplayMember = source.DisplayMember;
					this.ValueMember = source.ValueMember;
					if(!object.ReferenceEquals(currentDataSource, source.DataSource)) {
						this.currentDataSource = source.DataSource;
						object clonedData = GridControl.GetClonedDataSource(source.DataSource);
						if(clonedData != null) {
							SetDataSource(clonedData, true);
						}
						else {
							SetDataSource(source.DataSource, false);
						}
					}
					this.showClearButton = source.ShowClearButton;
					this.showAddNewButton = source.ShowAddNewButton;
					this.popupFindMode = source.PopupFindMode;
					Events.AddHandler(addNewValue, source.Events[addNewValue]);
				}
				base.Assign(item);
			}
			finally {
				EndUpdate();
			}
		}
		protected internal virtual FindMode GetPopupFindMode() {
			if(PopupFindMode == FindMode.Default) return FindMode.Always;
			return PopupFindMode;
		}
		protected internal SearchEditLookUpPopup LookUpPopup {
			get {
				if(lookUpPopup == null) {
					SetupLookUpPopup();
				}
				return lookUpPopup;
			}
		}
		protected override void Setup(BaseView view) {
			GridView gv = view as GridView;
			if(gv != null) gv.CustomDrawEmptyForeground += new CustomDrawEventHandler(gv_CustomDrawEmptyForeground);
			base.Setup(view);
		}
		void gv_CustomDrawEmptyForeground(object sender, CustomDrawEventArgs e) {
			GridView view = (GridView)sender;
			GridViewInfo viewInfo = view.ViewInfo;
			if(GetPopupFindMode() != FindMode.FindClick || view.DataSource != null) return;
			AppearanceObject app = (AppearanceObject)view.PaintAppearance.Empty.Clone();
			app.TextOptions.HAlignment = HorzAlignment.Center;
			app.TextOptions.VAlignment = VertAlignment.Center;
			app.TextOptions.WordWrap = WordWrap.Wrap;
			app.ForeColor = SystemColors.GrayText;
			app.DrawString(e.Cache, GridLocalizer.Active.GetLocalizedString(GridStringId.SearchLookUpMissingRows), viewInfo.ViewRects.Rows);
		}
		internal void SetupLookUpPopup() {
			if(lookUpPopup != null) return;
			if(Grid == null) return;
			lookUpPopup = CreateLookUpPopup();
			lookUpPopup.SetupGrid(Grid);
		}
		protected virtual SearchEditLookUpPopup CreateLookUpPopup() {
			return new SearchEditLookUpPopup(this);
		}
		internal void ForceActivateGridDataSource() {
			base.ActivateGridDataSource();
		}
		protected internal override void ActivateGridDataSource() {
			if(OwnerEdit == null) return;
			if(GetPopupFindMode() == FindMode.FindClick) return;
			base.ActivateGridDataSource();
			View.DataController.ClearInvalidRowsCache();
		}
		internal override BaseView.DataControllerType CheckDCType(BaseView.DataControllerType dataControllerType) {
			return dataControllerType;
		}
		protected internal override void ResetValuesCache(bool fireRefresh) {
			displayCache.Clear();
			base.ResetValuesCache(fireRefresh);
		}
		Hashtable displayCache = new Hashtable();
		protected internal virtual object GetDisplayTextByKeyValueCore(object keyValue) {
			if(Controller == null)
				return string.Empty;
			object displayText = string.Empty;
			if(keyValue == null || IsNullValue(keyValue)) {
				displayText = NullText;
			}
			else {
				displayText = displayCache[keyValue] as string;
				if(displayText == null) {
					int index = GetIndexByKeyValueCore(keyValue, delegate(object args) {
						int row = (int)args;
						if(row >= 0) Controller.GetRow(row);
					});
					if(index == AsyncServerModeDataController.OperationInProgress) return AsyncServerModeDataController.NoValue;
					displayText = GetDisplayTextCore(index, keyValue, null);
					if(BaseEdit.IsNotLoadedValue(displayText)) return displayText;
				}
			}
			CustomDisplayTextEventArgs e = new CustomDisplayTextEventArgs(keyValue, displayText.ToString());
			RaiseCustomDisplayText(e);
			return e.DisplayText;
		}
		object GetKeyValue(int index) {
			return Controller.GetValueEx(index, ValueMember);
		}
		protected override void OnAsyncTotalsReceived() {
			if(OwnerEdit != null) {
				if(OwnerEdit.EditValue != null) {
					OwnerEdit.requireUpdateDisplayText = true;
					OwnerEdit.OnAsyncRowLoaded(-1);
				}
			}
			else {
				RaiseRefreshRequired(EventArgs.Empty);
			}
		}
		protected override void OnAsyncRowLoaded(int controllerRowHandle) {
			GetDisplayTextCore(controllerRowHandle, GetKeyValue(controllerRowHandle), null);
			if(OwnerEdit != null)
				OwnerEdit.OnAsyncRowLoaded(controllerRowHandle);
			else {
				RaiseRefreshRequired(EventArgs.Empty);
			}
		}
		public override object GetDisplayValueByKeyValue(object keyValue) {
			return GetDisplayValueByKeyValueEx(keyValue);
		}
		public object GetDisplayValueByKeyValueEx(object keyValue, params OperationCompleted[] completed) {
			int index = GetIndexByKeyValueCore(keyValue, delegate(object args) {
				int row = (int)args;
				if(row >= 0) {
					Controller.GetRow(row);
					object x = GetDisplayValue(row, delegate(object args2) {
						if(completed != null) completed[0](args2);
					});
					if(!BaseEdit.IsNotLoadedValue(x) && completed != null) completed[0](x);
				}
			});
			if(index == AsyncServerModeDataController.OperationInProgress) return AsyncServerModeDataController.NoValue;
			object res = GetDisplayValue(index, delegate(object args) {
				if(completed != null) completed[0](args);
			});
			return res;
		}
		public virtual string GetDisplayTextByKeyValue(object keyValue) {
			object res = GetDisplayTextByKeyValueCore(keyValue);
			if(BaseEdit.IsNotLoadedValue(res) || res == null || res == DBNull.Value) return string.Empty;
			return res.ToString();
		}
		protected virtual object GetDisplayTextCore(int index, object keyValue, OperationCompleted completed) {
			if(BaseEdit.IsNotLoadedValue(keyValue)) return keyValue;
			if(index < 0) return string.Empty;
			object displayValue = GetDisplayValue(index, completed);
			if(BaseEdit.IsNotLoadedValue(displayValue)) return displayValue;
			return GetDisplayTextCoreByKeyValue(keyValue, displayValue);
		}
		protected object GetDisplayValue(int index) { return GetDisplayValue(index, null); }
		protected virtual object GetDisplayValue(int index, OperationCompleted completed) {
			if(index < 0) return null;
			return Controller.GetValueEx(index, DisplayMember, completed);
		}
		protected virtual string GetDisplayTextCoreByKeyValue(object keyValue, object displayValue) {
			string text = GetDisplayTextByDisplayValue(displayValue);
			if(keyValue == null) return text; 
			displayCache[keyValue] = text;
			return text;
		}
		protected int GetIndexByKeyValueCore(object keyValue, params OperationCompleted[] completed) {
			try {
				return Controller.FindRowByValue(ValueMember, keyValue, completed);
			}
			catch {
				return -1;
			}
		}
		public override int GetIndexByKeyValue(object keyValue) {
			int res = GetIndexByKeyValueCore(keyValue);
			if(res == DataController.OperationInProgress) return DataController.InvalidRow;
			return res;
		}
		public override string GetDisplayText(FormatInfo format, object editValue) {
			if(DataSource == null && !IsNullValue(editValue)) return GetNullEditText();
			if(IsNullValue(editValue)) return base.GetDisplayText(format, editValue);
			object res = GetDisplayTextByKeyValueCore(editValue);
			if(BaseEdit.IsNotLoadedValue(res)) {
				if(OwnerEdit != null) OwnerEdit.requireUpdateDisplayText = true;
				return string.Empty;
			}
			if(res == null) return string.Empty;
			return res.ToString();
		}
		protected internal virtual void AddNewClick() {
			if(OwnerEdit == null) return;
			OwnerEdit.CancelPopup();
			RaiseAddNewValue(new AddNewValueEventArgs());
		}
		protected internal virtual void ClearClick() {
			if(OwnerEdit != null && OwnerEdit.IsPopupOpen) {
				OwnerEdit.CancelPopup();
				OwnerEdit.EditValue = null;
			}
		}
		internal bool inAddNewValue = false;
		protected virtual void RaiseAddNewValue(AddNewValueEventArgs e) {
			if(inAddNewValue) return;
			AddNewValueEventHandler handler = (AddNewValueEventHandler)Events[addNewValue];
			if(handler == null) return;
			this.inAddNewValue = true;
			try {
				handler(GetEventSender(), e);
				if(e.Cancel) return;
				if(OwnerEdit != null) {
					OwnerEdit.Focus();
					OwnerEdit.EditValue = e.NewValue;
				}
			}
			finally {
				this.inAddNewValue = false;
			}
		}
		protected internal override PopupFilterMode GetFilterMode() {
			return PopupFilterMode;
		}
	}
}
namespace DevExpress.XtraEditors.ViewInfo {
	public class SearchLookUpEditBaseViewInfo : LookUpEditBaseViewInfo {
		public SearchLookUpEditBaseViewInfo(RepositoryItem item) : base(item) { }
		public new SearchLookUpEdit OwnerEdit { get { return base.OwnerEdit as SearchLookUpEdit; } }
		public new RepositoryItemSearchLookUpEdit Item { get { return base.Item as RepositoryItemSearchLookUpEdit; } }
	}
}
namespace DevExpress.XtraEditors {
	[Designer("DevExpress.XtraGrid.Design.GridLookUpEditBaseDesigner, " + AssemblyInfo.SRAssemblyGridDesign),
	 DXToolboxItem(true),
	 Description("Encapsulates lookup functionality using a dropdown GridControl control, and so providing advanced data representation features (sorting, grouping, summary calculation, formatting, etc)."),
	 ToolboxTabName(AssemblyInfo.DXTabData),
	ToolboxBitmap(typeof(DevExpress.XtraGrid.ToolboxIcons.ToolboxIconsRootNS), "SearchLookUpEdit")
	]
	public class SearchLookUpEdit : GridLookUpEditBase {
		static SearchLookUpEdit() {
			RepositoryItemSearchLookUpEdit.RegisterSearchLookUpEdit();
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("SearchLookUpEditProperties"),
#endif
 DXCategory(CategoryName.Properties), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new RepositoryItemSearchLookUpEdit Properties { get { return base.Properties as RepositoryItemSearchLookUpEdit; } }
#if !SL
	[DevExpressXtraGridLocalizedDescription("SearchLookUpEditEditorTypeName")]
#endif
		public override string EditorTypeName { get { return "SearchLookUpEdit"; } }
		protected override PopupBaseForm GetPopupForm() {
			PopupBaseForm form = base.GetPopupForm();
			if(form != null) Properties.SetupLookUpPopup();
			return form;
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("SearchLookUpEditAddNewValue"),
#endif
 Category(CategoryName.Events)]
		public event AddNewValueEventHandler AddNewValue {
			add { Properties.AddNewValue += value; }
			remove { Properties.AddNewValue -= value; }
		}
		public override bool EditorContainsFocus {
			get { return base.EditorContainsFocus || (Properties != null && Properties.inAddNewValue); }
		}
		protected override PopupBaseForm CreatePopupForm() {
			return new PopupSearchLookUpEditForm(this);
		}
		protected new PopupSearchLookUpEditForm PopupForm { get { return base.PopupForm as PopupSearchLookUpEditForm; } }
		protected override bool CheckInputNewValue(bool partial) {
			return false;
		}
		public override object GetSelectedDataRow() {
			return Properties.GetRowByKeyValue(EditValue);
		}
		internal object lastKeyMessage = null;
		protected override void WndProc(ref Message m) {
			lastKeyMessage = DevExpress.XtraEditors.Senders.BaseSender.SaveMessage(ref m, lastKeyMessage);
			base.WndProc(ref m);
		}
		protected override bool AllowPopupTabOut { get { return false; } }
		internal bool requireUpdateDisplayText = false;
		internal void OnAsyncRowLoaded(int controllerRowHandle) {
			if(requireUpdateDisplayText) {
				this.requireUpdateDisplayText = false;
				object text = Properties.GetDisplayTextByKeyValueCore(EditValue);
				if(BaseEdit.IsNotLoadedValue(text)) {
					this.requireUpdateDisplayText = true;
					return;
				}
				if(!object.Equals(ViewInfo.DisplayText, Text)) {
					UpdateDisplayText();
					Invalidate();
				}
			}
		}
		protected override bool CanProcessAutoSearchText { get { return false; } }
		protected override void OnEditorKeyPress(KeyPressEventArgs e) {
			base.OnEditorKeyPress(e);
			if(!e.Handled && !Properties.ReadOnly) {
				if(!IsPopupOpen && char.IsLetterOrDigit(e.KeyChar)) {
					ShowPopup();
					if(PopupForm != null && PopupForm.FindEdit != null && e.KeyChar != 13 && e.KeyChar != 9) {
						PopupForm.FindEdit.SendKey(this.lastKeyMessage, e);
					}
				}
			}
		}
	}
}
