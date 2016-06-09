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
using System.IO;
using System.Reflection;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Helpers;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Registrator;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraEditors.ViewInfo;
namespace DevExpress.XtraEditors.Repository {
	[Designer("DevExpress.XtraGrid.Design.GridLookUpEditRepositoryItemDesigner, " + AssemblyInfo.SRAssemblyGridDesign)
	]
	[DevExpress.Utils.Design.DataAccess.DataAccessMetadata("All", SupportedProcessingModes = "GridLookUp", EnableDirectBinding = false)]
	[LookupEditCustomBindingProperties("GridLookUpEdit")]
	public class RepositoryItemGridLookUpEdit : RepositoryItemGridLookUpEditBase {
		static RepositoryItemGridLookUpEdit() {
			RegisterGridLookUpEdit();
		}
		public static void RegisterGridLookUpEdit() {
			if(EditorRegistrationInfo.Default.Editors.Contains("GridLookUpEdit")) return;
			Image img = null;
			EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo("GridLookUpEdit", typeof(GridLookUpEdit), typeof(RepositoryItemGridLookUpEdit), typeof(DevExpress.XtraEditors.ViewInfo.GridLookUpEditBaseViewInfo), new DevExpress.XtraEditors.Drawing.ButtonEditPainter(), true, img, typeof(DevExpress.Accessibility.PopupEditAccessible)));
		}
#if !SL
	[DevExpressXtraGridLocalizedDescription("RepositoryItemGridLookUpEditEditorTypeName")]
#endif
		public override string EditorTypeName { get { return "GridLookUpEdit"; } }	
		bool autoComplete;
		public RepositoryItemGridLookUpEdit() {
			this.autoComplete = true;
		}
		[Browsable(false)]
#if !SL
	[DevExpressXtraGridLocalizedDescription("RepositoryItemGridLookUpEditOwnerEdit")]
#endif
		public new GridLookUpEdit OwnerEdit { get { return base.OwnerEdit as GridLookUpEdit; } }
		[DXCategory(CategoryName.Data), 
#if !SL
	DevExpressXtraGridLocalizedDescription("RepositoryItemGridLookUpEditServerMode"),
#endif
 DefaultValue(false),
		Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Obsolete(ObsoleteText.SRGridControl_ServerMode)]
		public bool ServerMode {
			get { return Controller != null && Controller.IsServerMode; }
			set {
			}
		}
		[DXCategory(CategoryName.Data), 
#if !SL
	DevExpressXtraGridLocalizedDescription("RepositoryItemGridLookUpEditAutoComplete"),
#endif
 DefaultValue(true)]
		public bool AutoComplete {
			get { return autoComplete; }
			set {
				if(AutoComplete == value) return;
				autoComplete = value;
				CheckServerMode();
			}
		}
		public override void Assign(RepositoryItem item) {
			RepositoryItemGridLookUpEdit source = item as RepositoryItemGridLookUpEdit;
			BeginUpdate(); 
			try {
				if(source != null) {
					this.autoComplete = source.AutoComplete;
				}
				base.Assign(item);
			}
			finally {
				EndUpdate();
			}
		}
		protected internal override void ResetValuesCache(bool fireRefresh) {
			if(this.displayCache != null) this.displayCache.Clear();
			base.ResetValuesCache(fireRefresh);
		}
		Hashtable displayCache = new Hashtable();
		public override object GetDisplayValueByKeyValue(object keyValue) {
			int index = GetIndexByKeyValue(keyValue);
			if(index < 0) return null;
			return GetDisplayValue(index);
		}
		public virtual string GetDisplayTextByKeyValue(object keyValue) {
			if(Controller == null)
				return string.Empty;
			string displayText = string.Empty;
			if(keyValue == null || IsNullValue(keyValue)) {
				displayText = NullText;
			}
			else {
				displayText = displayCache[keyValue] as string;
				if(displayText == null) {
					int index = GetIndexByKeyValue(keyValue);
					displayText = GetDisplayTextCore(index, keyValue);
				}
			}
			CustomDisplayTextEventArgs e = new CustomDisplayTextEventArgs(keyValue, displayText);
			RaiseCustomDisplayText(e);
			return e.DisplayText;
		}
		protected internal object GetKeyValueByDisplayValue(object displayValue) {
			int index = Controller.FindRowByValue(DisplayMember, displayValue);
			return GetKeyValue(index);
		}
		protected override bool CancelPopupInputOnButtonClose { get { return true; } }
		protected virtual string GetDisplayTextCore(int index, object keyValue) {
			if(index < 0) return string.Empty;
			return GetDisplayTextCore(keyValue, GetDisplayValue(index));
		}
		protected virtual string GetDisplayTextCore(object keyValue, object displayValue) {
			string text = GetDisplayTextByDisplayValue(displayValue);
			if(keyValue == null) return text; 
			displayCache[keyValue] = text;
			return text;
		}
		public virtual object GetKeyValue(int index) {
			if(index < 0) return null;
			return Controller.GetValueEx(index, ValueMember);
		}
		public virtual string GetDisplayText(int index) {
			return GetDisplayTextCore(index, GetKeyValue(index));
		}
		public virtual object GetDisplayValue(int index) {
			if(index < 0) return null;
			return Controller.GetValueEx(index, DisplayMember);
		}
		protected internal DataController GetController() {
			return Controller;
		}
		protected internal int FindItem(string text, bool partial) {
			if(!IsReady) return -1;
			DataController controller = GetController();
			if(controller.IsServerMode) {
				return ServerFindItem(text, partial);
			}
			int count = controller.VisibleListSourceRowCount;
			text = text.ToLower();
			for(int n = 0; n < count; n++) {
				object val = controller.GetValueEx(n, ValueMember);
				object dispVal = controller.GetValueEx(n, DisplayMember);
				string valText = GetDisplayTextCore(val, dispVal);
				if(valText == null) 
					valText = string.Empty;
				else
					valText = valText.ToLower();
				if(text.Length == 0) {
					if(valText.Length == 0) return n;
					continue;
				}
				if(partial) 
					valText = valText.Substring(0, Math.Min(text.Length, valText.Length));
				if(valText == text) return n;
			}
			return -1;
		}
		int ServerFindItem(string text, bool partial) {
			DataController controller = GetController();
			int res = -1;
			if(string.IsNullOrEmpty(text) || !partial)
				res = controller.FindRowByValue(DisplayMember, text);
			else
				res = controller.FindRowByBeginWith(DisplayMember, text);
			if(res < 0) return -1;
			return res;
		}
		public override string GetDisplayText(FormatInfo format, object editValue) {
			if(OwnerEdit != null && !OwnerEdit.IsDisplayTextValid) {
				if(TextEditStyle == TextEditStyles.Standard) {
					return OwnerEdit.AutoSearchText;
				}
				return base.GetDisplayText(format, editValue);
			}
			if(DataSource == null && !IsNullValue(editValue)) return GetNullEditText();
			if(IsNullValue(editValue)) return base.GetDisplayText(format, editValue);
			string res = GetDisplayTextByKeyValue(editValue);
			if(res == null) return string.Empty;
			return res;
		}
		internal void RaiseProcessNewValueCore(ProcessNewValueEventArgs e) { RaiseProcessNewValue(e); }
		internal bool IsNullInputAllowedCore { get { return IsNullInputAllowed; } }
		protected internal override PopupFilterMode GetFilterMode() {
			if(TextEditStyle != TextEditStyles.Standard) return PopupFilterMode.StartsWith;
			if(PopupFilterMode == PopupFilterMode.Default) {
				return AutoComplete ? PopupFilterMode.StartsWith : PopupFilterMode.Contains;
			}
			return PopupFilterMode;
		}
	}
}
namespace DevExpress.XtraEditors.ViewInfo {
	public class GridLookUpEditBaseViewInfo : LookUpEditBaseViewInfo {
		public GridLookUpEditBaseViewInfo(RepositoryItem item) : base(item) { }
		public new GridLookUpEdit OwnerEdit { get { return base.OwnerEdit as GridLookUpEdit; } }
		public new RepositoryItemGridLookUpEdit Item { get { return base.Item as RepositoryItemGridLookUpEdit; } }
	}
}
namespace DevExpress.XtraEditors {
	[Designer("DevExpress.XtraGrid.Design.GridLookUpEditBaseDesigner, " + AssemblyInfo.SRAssemblyGridDesign),
	 DXToolboxItem(true),
	 Description("Encapsulates lookup functionality using a dropdown GridControl control, and so providing advanced data representation features (sorting, grouping, summary calculation, formatting, etc)."),
	 ToolboxTabName(AssemblyInfo.DXTabData),
	ToolboxBitmap(typeof(DevExpress.XtraGrid.ToolboxIcons.ToolboxIconsRootNS), "GridLookUpEdit")
	]
	public class GridLookUpEdit : GridLookUpEditBase {
		bool isDisplayTextValid = true;
		static GridLookUpEdit() {
			RepositoryItemGridLookUpEdit.RegisterGridLookUpEdit();
		}
#if !SL
	[DevExpressXtraGridLocalizedDescription("GridLookUpEditEditorTypeName")]
#endif
		public override string EditorTypeName { get { return "GridLookUpEdit"; } }	
		protected override PopupBaseForm CreatePopupForm() {
			return new PopupGridLookUpEditForm(this);
		}
		protected internal string GetAutoSearchTextFilterCore() { return GetAutoSearchTextFilter(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridLookUpEditProperties"),
#endif
 DXCategory(CategoryName.Properties), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new RepositoryItemGridLookUpEdit Properties { get { return base.Properties as RepositoryItemGridLookUpEdit; } }
		protected internal new PopupGridLookUpEditForm PopupForm { get { return base.PopupForm as PopupGridLookUpEditForm; } }
		protected override int FindItem(string text, int startIndex) {
			if(!IsReady) return -1;
			int index = Properties.FindItem(text, true);
			return index;
		}
		protected override bool IsClearAutoSearchOnCancelPopup { get { return false; } }
		protected override void FindUpdateEditValueAutoSearchText() { 
			LockEditValueChanged();
			try {
				this.isDisplayTextValid = false;
				base.FindUpdateEditValueAutoSearchText();
			}
			finally {
				UnLockEditValueChanged();
			}
		}
		protected override void OnValidatingCore(CancelEventArgs e) {
			if(!CompareEditValue(EditValue, this.lastChangedEditValue, false)) OnEditValueChanged();
			base.OnValidatingCore(e);
		}
		protected override void DoImmediatePopup(int itemIndex, char pressedKey) {
			if(!IsAutoComplete) itemIndex = -1;
			base.DoImmediatePopup(itemIndex, pressedKey);
		}
		protected override void FindUpdatePopupSelectedItem(int itemIndex) {  
			if(itemIndex >= 0) {
				if(Properties.ReadOnly) return;
				if(!IsAutoComplete) {
					PopupForm.UpdateFocusedIndex(GridControl.InvalidRowHandle);
					return;
				}
				EditValue = Properties.GetController().GetValueEx(itemIndex, Properties.ValueMember);
				if(PopupForm != null) 
					PopupForm.UpdateFocusedIndex(itemIndex);
			}
			else {
				if(Properties.TextEditStyle == TextEditStyles.Standard) {
					PopupForm.UpdateFocusedIndex(-1);
				}
			}
		}
		protected override object GetChangingOldEditValue(object value) {
			if(onAcceptEditValue != null) return onAcceptEditValue;
			return value;
		}
		object onAcceptEditValue = null;
		protected override void AcceptPopupValue(object val) {
			if(val != null) {
				this.onAcceptEditValue = EditValue;
				SetEmptyEditValue(null);
			}
			this.isDisplayTextValid = true;
			base.AcceptPopupValue(val);
			this.onAcceptEditValue = null;
		}
		protected override void OnCancelEditValueChanging() {
			if(onAcceptEditValue != null) {
				object value = this.onAcceptEditValue;
				this.onAcceptEditValue = null;
				SetEmptyEditValue(value);
			}
		}
		protected override bool IsImmediatePopup { get { return !IsAutoComplete || Properties.ImmediatePopup; } }
		protected internal override bool IsAutoComplete { get { return Properties.AutoComplete || !IsMaskBoxAvailable; } }
		protected override void FindUpdateEditValue(int itemIndex, bool jopened) {
			if(!IsAutoComplete) {
				this.isDisplayTextValid = false;
				IsModified = true;
				return;
			}
			this.isDisplayTextValid = itemIndex >= 0;
			if(!IsPopupOpen || jopened) {
				if(Properties.ReadOnly) return;
				if(itemIndex >= 0)
					EditValue = Properties.GetKeyValue(itemIndex);
			}
		}
		protected override void ParseEditorValue() {
			AutoSearchText = string.Empty;
			this.isDisplayTextValid = true;
			if(IsMaskBoxAvailable) {
				if(!CheckInputNewValue(false)) {
					bool isNull = EditValue == null || EditValue == DBNull.Value;
					if(Properties.GetIndexByKeyValue(EditValue) < 0) {
						if(!(Properties.IsNullInputAllowedCore && isNull)) EditValue = OldEditValue;
					}
					UpdateMaskBoxDisplayText();
				}
				UpdateEditValueFromMaskBoxText();
			}
		}
		protected internal virtual bool IsDisplayTextValid { get { return isDisplayTextValid; } }
		protected bool IsReady { get { return Properties.IsReady; } }
		internal void ProcessAutoSearchCharCore(KeyPressEventArgs e) {
			ProcessAutoSearchChar(e);
		}
		protected override void SetEmptyEditValue(object emptyEditValue) {
			this.isDisplayTextValid = true;
			base.SetEmptyEditValue(emptyEditValue);
		}
		internal void ProcessAutoSearchNavKeyCore(KeyEventArgs e) {
			ProcessAutoSearchNavKey(e);
		}
		bool prevHideSelection = true;
		protected override void OnPopupShown() { 
			this.prevHideSelection = Properties.HideSelection;
			UpdateHideSelection(false);
			base.OnPopupShown();
		}
		protected override void OnPopupClosed(PopupCloseMode closeMode) {
			this.isDisplayTextValid = true;
			UpdateHideSelection(prevHideSelection);
			if(closeMode != PopupCloseMode.Immediate) {
				if(EditorContainsFocus && MaskBox != null) MaskBox.Focus();
			}
			base.OnPopupClosed(closeMode);
		}
		internal TextBoxMaskBox MaskBoxCore { get { return MaskBox; } }
		protected internal bool FireProcessNewValue(string text, out object keyValue) {
			keyValue = null;
			object dispVal = text;
			DataColumnInfo info = Properties.Controller.Columns[Properties.DisplayMember];
			if(info != null) {
				try { dispVal = info.ConvertValue(dispVal, true); }
				catch {}
			}
			keyValue = Properties.GetKeyValueByDisplayValue(dispVal);
			if(keyValue != null) return true;
			ProcessNewValueEventArgs e = new ProcessNewValueEventArgs(dispVal);
			Properties.RaiseProcessNewValueCore(e);
			if(e.Handled) {
				Properties.ResetValuesCache();
				keyValue = Properties.GetKeyValueByDisplayValue(e.DisplayValue);
			}
			return e.Handled;
		}
		protected override bool CheckInputNewValue(bool partial) {
			if(!IsMaskBoxAvailable || Properties.FindItem(MaskBox.MaskBoxText, partial) != -1) return true;
			object newValue;
			if(FireProcessNewValue(MaskBox.MaskBoxText, out newValue)) {
				EditValue = newValue;
				return true;
			}
			return false;
		}
		protected override void OnMaskBox_ValueChanged(object sender, EventArgs e) {
			LockEditValueChanged();
			try {
				base.OnMaskBox_ValueChanged(sender, e);
			}
			finally {
				UnLockEditValueChanged();
			}
		}
		private void UpdateEditValueFromMaskBoxText() {
			if(Properties.GetDisplayText(EditValue) == MaskBox.MaskBoxText) return;
			int index = FindItem(MaskBox.MaskBoxText, 0);
			if(index != -1)
				EditValue = Properties.GetKeyValue(index);
		}
		public override object GetSelectedDataRow() {
			return Properties.GetRowByKeyValue(EditValue);
		}
	}
	public enum FindMode { Default, Always, FindClick }
}
namespace DevExpress.XtraEditors.Popup {
	[ToolboxItem(false)]
	public class PopupGridLookUpEditForm : CustomBlogGridPopupForm {
		public PopupGridLookUpEditForm(GridLookUpEdit ownerEdit) : base(ownerEdit) { 
			Controls.Add(Grid);
		}
		protected override void Dispose(bool disposing) {
			Controls.Remove(Properties.Grid);
			base.Dispose(disposing);
		}
		public override void ShowPopupForm() {
			base.ShowPopupForm();
			FocusFormControl(EmbeddedControl);
		}
		protected override void ShowingPopupForm() {
			Grid.SetSelectable(false);
			Grid.CreateHandleCore();
			Grid.ForceInitialize();
			base.ShowingPopupForm();
		}
		protected override Control EmbeddedControl { get { return Grid; } }
		public override bool FormContainsFocus { 
			get { 
				if(base.FormContainsFocus) return true;
				if(Grid.IsFocused) return true;
				return false;
			}
		}
		public override void ProcessKeyDown(KeyEventArgs e) {
			if(!Focused) {
				if(AllowGridKeys) {
					if(Grid.IsNeededKey(e.KeyData)) return;
				} else {
					if(e.KeyData == Keys.Left || e.KeyData == Keys.Right) {
						OwnerEdit.ProcessAutoSearchNavKeyCore(e);
						UpdateDisplayFilter();
						return;
					}
				}
				if(e.KeyData == Keys.Enter) {
					e.Handled = true;
					ClosePopup();
				}
			}
			base.ProcessKeyDown(e);
		}
		protected override object GetResultValue(int rowHandle, bool allowFireNewValue) {
			if(View.IsGroupRow(rowHandle)) rowHandle = View.GetDataRowHandleByGroupRowHandle(rowHandle);
			if(!View.DataController.IsValidControllerRowHandle(rowHandle)) {
				if(!allowFireNewValue) return OldEditValue;
				if(OwnerEdit.MaskBoxCore != null && OwnerEdit.AutoSearchText != string.Empty) {
					int i = OwnerEdit.Properties.FindItem(OwnerEdit.AutoSearchText, false);
					if(i != -1) return Properties.GetKeyValue(i);
					object val;
					if(OwnerEdit.FireProcessNewValue(OwnerEdit.AutoSearchText, out val)) return val;
				}
			}
			if(View.DataController.IsValidControllerRowHandle(rowHandle)) {
				return View.DataController.GetValueEx(rowHandle, Properties.ValueMember);
			}
			return OldEditValue;
		}
		public override object ResultValue {  get {  return GetResultValue(View.FocusedRowHandle, false); }  }
		public override void ProcessKeyPress(KeyPressEventArgs e) {
			base.ProcessKeyPress(e);
			if(e.Handled) return;
			if(!AllowGridKeys) {
				OwnerEdit.ProcessAutoSearchCharCore(e);
				if(e.KeyChar == '\t' || e.KeyChar == 13 || e.KeyChar == 10) return;
				UpdateDisplayFilter();
			}
		}
		protected void UpdateDisplayFilter() {
			UpdateDisplayFilter(OwnerEdit.GetAutoSearchTextFilterCore());
		}
		protected virtual bool AllowGridKeys { get { return (View.State == GridState.Editing || View.FocusedRowHandle == GridControl.AutoFilterRowHandle); } }
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
		protected internal void UpdateFocusedIndex(int index) {
			View.FocusedRowHandle = index < 0 ? GridControl.InvalidRowHandle : index;
		}
		public new RepositoryItemGridLookUpEdit  Properties { get { return OwnerEdit.Properties as RepositoryItemGridLookUpEdit ; } }
		public new GridLookUpEdit OwnerEdit { get { return base.OwnerEdit as GridLookUpEdit; } }
		protected override Size DefaultEmptySize { get { return new Size(300, 300); } }
	}
}
namespace DevExpress.XtraGrid.Helpers {
	public interface IViewController {
		GridEditorContainerHelper EditorHelper { get ; }
		InfoCollection AvailableViews { get ; } 
	}
	public class GridLookUpData : GridColumnData {
		public GridLookUpData(ColumnView view) : base(view) { }
		protected override GridDataColumnInfo CreateColumnInfo(GridColumn column) {
			GridDataColumnInfo info = new GridDataColumnInfo(this, column);
			info.Required = true;
			return info;
		}
		protected override object GetKey(DataColumnInfo column) { 
			if(column == null) return null;
			return column.Name;
		}
	}
}
