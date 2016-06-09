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
using System.Text;
using System.ComponentModel;
using DevExpress.XtraEditors.Popup;
using System.Windows.Forms;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.Utils;
using System.Collections;
using DevExpress.Utils.Serializing;
using DevExpress.XtraTreeList.Data;
using DevExpress.XtraTreeList.Nodes.Operations;
using System.Drawing;
namespace DevExpress.XtraEditors {
	[DXToolboxItem(true), ToolboxTabName(AssemblyInfo.DXTabData), ToolboxBitmap(typeof(DevExpress.XtraTreeList.ToolboxIcons.ToolboxIconsRootNS), "TreeListLookUpEdit"), 
	Designer("DevExpress.XtraTreeList.Design.TreeListLookUpEditDesigner, " + AssemblyInfo.SRAssemblyTreeListDesign),
	Description("Encapsulates lookup functionality using a dropdown TreeList control, and so providing advanced data representation features (sorting, filtering, summary calculation, formatting, etc).")
	]
	public class TreeListLookUpEdit : LookUpEditBase {
		static TreeListLookUpEdit() {
			RepositoryItemTreeListLookUpEdit.RegisterTreeListLookUpEdit();
		}
		bool savedHideSelection;
		public TreeListLookUpEdit() {
			IsDisplayTextValid = true;
		}
		[DXCategory(CategoryName.Properties), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new RepositoryItemTreeListLookUpEdit Properties { get { return base.Properties as RepositoryItemTreeListLookUpEdit; } }
		protected internal new TreeListLookUpEditPopupForm PopupForm { get { return base.PopupForm as TreeListLookUpEditPopupForm; } }
		protected virtual bool IsAutoComplete { get { return Properties.AutoComplete || !IsMaskBoxAvailable; } }
		protected override bool IsImmediatePopup { get { return !IsAutoComplete || Properties.ImmediatePopup; } }
		public override string EditorTypeName { get { return RepositoryItemTreeListLookUpEdit.EditorName; } }
		protected internal bool IsDisplayTextValid { get; private set; }
		protected override bool IsAcceptCloseMode(PopupCloseMode closeMode) {
			return closeMode == PopupCloseMode.Normal || closeMode == PopupCloseMode.ButtonClick;
		}
		protected override int FindItem(string text, int startIndex) {
			return Properties.FindRowByDisplayText(text, true);
		}
		protected override void FindUpdateEditValue(int itemIndex, bool jopened) {
			if(!IsAutoComplete) {
				IsDisplayTextValid = false;
				IsModified = true;
				return;
			}
			IsDisplayTextValid = itemIndex > -1;
			if(!IsPopupOpen && IsDisplayTextValid && !Properties.ReadOnly) 
				EditValue = Properties.GetRowValue(itemIndex, Properties.ValueMember);
		}
		protected override void FindUpdateEditValueAutoSearchText() {
			LockEditValueChanged();
			try {
				IsDisplayTextValid = false;
				base.FindUpdateEditValueAutoSearchText();
			}
			finally {
				UnLockEditValueChanged();
			}
		}
		protected override void FindUpdatePopupSelectedItem(int itemIndex) {
			if(itemIndex > -1) {
				if(Properties.ReadOnly) return;
				EditValue = Properties.GetRowValue(itemIndex, Properties.ValueMember);
				Properties.UpdateTreeListFocusedNode(itemIndex);
			}
			else {
				if(Properties.TextEditStyle == TextEditStyles.Standard)
					Properties.UpdateTreeListFocusedNode(itemIndex);
			}
		}
		protected override void OnPopupShown() {
			base.OnPopupShown();
			Properties.TreeList.MenuManager = MenuManager;
			savedHideSelection = Properties.HideSelection;
			UpdateMaskBoxSelection(false);
		}
		void UpdateMaskBoxSelection(bool hideSelection) {
			Properties.BeginUpdate();
			Properties.HideSelection = hideSelection;
			Properties.CancelUpdate();
			UpdateMaskBoxProperties(false);
		}
		protected override void OnPopupClosed(PopupCloseMode closeMode) {
			IsDisplayTextValid = true;
			UpdateMaskBoxSelection(savedHideSelection);
			if(closeMode != PopupCloseMode.Immediate) 
				if(EditorContainsFocus && MaskBox != null) MaskBox.Focus();
			base.OnPopupClosed(closeMode);
		}
		protected override void ParseEditorValue() {
			IsDisplayTextValid = true;
			if(IsMaskBoxAvailable) {
				if(CheckInputNewValue(false)) 
					UpdateMaskBoxDisplayText();
				UpdateEditValueFromMaskBoxText();
			}
			base.ParseEditorValue();
		}
		void UpdateEditValueFromMaskBoxText() {
			if(Properties.GetDisplayText(EditValue) == MaskBox.MaskBoxText) return;
			int index = FindItem(MaskBox.MaskBoxText, 0);
			if(index != -1)
				EditValue = Properties.GetKeyValueByIndex(index);
		}
		protected override void SetEmptyEditValue(object emptyEditValue) {
			IsDisplayTextValid = true;
			base.SetEmptyEditValue(emptyEditValue);
		}
		protected override void AcceptPopupValue(object val) {
			IsDisplayTextValid = true;
			base.AcceptPopupValue(val);
		}
		protected internal string GetAutoSearchTextFilterCore() { return GetAutoSearchTextFilter(); }
		protected override DevExpress.XtraEditors.Popup.PopupBaseForm CreatePopupForm() {
			return new TreeListLookUpEditPopupForm(this);
		}
		public override object GetSelectedDataRow() {
			return Properties.GetRowByKeyValue(EditValue);
		}
		protected override bool CheckInputNewValue(bool partial) {
			if(!IsMaskBoxAvailable || Properties.FindRowByDisplayText(MaskBox.MaskBoxText, partial) != -1) return true;
			object newValue;
			if(Properties.OnProcessInputNewValue(MaskBox.MaskBoxText, out newValue)) {
				EditValue = newValue;
				return true;
			}
			return false;
		}
		internal void ProcessAutoSearchCharCore(KeyPressEventArgs e) {
			ProcessAutoSearchChar(e);
			if(IsPopupOpen)
				Properties.UpdateDisplayFilter();
		}
	}
}
namespace DevExpress.XtraEditors.Repository {
	[Designer("DevExpress.XtraTreeList.Design.TreeListLookUpEditRepositoryItemDesigner, " + AssemblyInfo.SRAssemblyTreeListDesign)
	]
	[DevExpress.Utils.Design.DataAccess.DataAccessMetadata("All", SupportedProcessingModes = "Simple", EnableDirectBinding = false)]
	[LookupEditCustomBindingProperties("TreeListLookUpEdit")]
	public class RepositoryItemTreeListLookUpEdit : RepositoryItemLookUpEditBase {
		protected internal const string TreeListLayoutKeyName = "TreeListLayoutKey";
		public const string EditorName = "TreeListLookUpEdit";
		static RepositoryItemTreeListLookUpEdit() {
			RegisterTreeListLookUpEdit();
		}
		public static void RegisterTreeListLookUpEdit() {
			if(EditorRegistrationInfo.Default.Editors.Contains(EditorName)) return;
			EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(EditorName, typeof(TreeListLookUpEdit), typeof(RepositoryItemTreeListLookUpEdit), typeof(DevExpress.XtraEditors.ViewInfo.TreeListLookUpEditBaseViewInfo), new DevExpress.XtraEditors.Drawing.ButtonEditPainter(), true, null, typeof(DevExpress.Accessibility.PopupEditAccessible)));
		}
		TreeList treeList;
		Dictionary<object, string> displayTextCache;
		bool autoComplete, autoExpandAllNodes, allowSelectOnHover;
		PopupFilterMode popupFilterMode;
		public RepositoryItemTreeListLookUpEdit() {
			this.displayTextCache = new Dictionary<object, string>();
			this.autoComplete = true;
			this.autoExpandAllNodes = true;
			this.allowSelectOnHover = true;
			this.popupFilterMode = PopupFilterMode.Default;
			this.treeList = CreateTreeList();
			this.treeList.OptionsView.ShowIndentAsRowStyle = true;
			this.treeList.OptionsBehavior.EnableFiltering = true;
			InitTreeList(TreeList);
		}
		public override string EditorTypeName { get { return EditorName; } }
		[Browsable(false)]
		public new TreeListLookUpEdit OwnerEdit { get { return base.OwnerEdit as TreeListLookUpEdit; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), TypeConverter(typeof(ExpandableObjectConverter)), Editor("DevExpress.XtraTreeList.Design.TreeListLookUpEditor, " + AssemblyInfo.SRAssemblyTreeListDesign, typeof(System.Drawing.Design.UITypeEditor))]
		public TreeList TreeList { 
			get { return treeList; }
			set {
				if(TreeList == value) return;
				if(TreeList != null)
					TreeList.Dispose();
				treeList = value;
				if(treeList == null) 
					treeList = CreateTreeList();
				OnTreeListChanged();
			}
		}
		protected virtual void OnTreeListChanged() {
			InitTreeList(TreeList);
			AddToContainer(TreeList);
			OnTreeListPropertyChanged();
			OnDataSourceChanged();
		}
		protected override bool CancelPopupInputOnButtonClose { get { return true; } }
		[DXCategory(CategoryName.Data), DefaultValue(true)]
		public bool AutoComplete {
			get { return autoComplete; }
			set { autoComplete = value; }
		}
		[DXCategory(CategoryName.Data), DefaultValue(PopupFilterMode.Default)]
		public PopupFilterMode PopupFilterMode {
			get { return popupFilterMode; }
			set {
				if(PopupFilterMode == value) return;
				popupFilterMode = value;
				OnPopupFilterModeChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), DefaultValue(true)]
		public bool AutoExpandAllNodes {
			get { return autoExpandAllNodes; }
			set {
				if(AutoExpandAllNodes == value) return;
				autoExpandAllNodes = value;
				OnAutoExpandAllNodesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), DefaultValue(true)]
		public bool AllowSelectOnHover {
			get { return allowSelectOnHover; }
			set { allowSelectOnHover = value; }
		}
		protected virtual void OnAutoExpandAllNodesChanged() {
			if(IsLoading || !AutoExpandAllNodes) return;
			ScheduleExpandAll();
			ExpandAllNodes();
		}
		protected internal void ExpandAllNodes() {
			if(shouldExpandAllNodes && TreeList.Nodes.Count > 0) {
				TreeList.ExpandAll();
				shouldExpandAllNodes = false;
			}
		}
		internal IDictionary PropertyStoreCore { get { return PropertyStore; } }
		protected virtual TreeList CreateTreeList() {
			return new TreeList();
		}
		protected virtual void OnPopupFilterModeChanged() {
			if(OwnerEdit != null && OwnerEdit.IsPopupOpen)
				UpdateDisplayFilter();
		}
		protected internal virtual PopupFilterMode GetActualFilterMode() {
			if(TextEditStyle != TextEditStyles.Standard) return PopupFilterMode.StartsWith;
			if(PopupFilterMode == PopupFilterMode.Default) 
				return AutoComplete ? PopupFilterMode.StartsWith : PopupFilterMode.Contains;
			return PopupFilterMode;
		}
		protected virtual void InitTreeList(TreeList treeList) {
			treeList.LookUpOwner = this;
		}
		protected override void OnPopupFormSizeChanged() {
			ResetSavedPopupFormSize();
			base.OnPopupFormSizeChanged();
		}
		protected override void OnBestFitModeChanged() {
			ResetSavedPopupFormSize();
			base.OnBestFitModeChanged();
		}
		void ResetSavedPopupFormSize() {
			if(PropertyStore.Contains(LookUpPropertyNames.PopupBestWidth))
				PropertyStore.Remove(LookUpPropertyNames.PopupBestWidth);
			if(PropertyStore.Contains(LookUpPropertyNames.BlobSize))
				PropertyStore.Remove(LookUpPropertyNames.BlobSize);
		}
		object currentDataSource = null;
		int lockPropertiesChanged = 0;
		public override void Assign(RepositoryItem item) {
			RepositoryItemTreeListLookUpEdit source = item as RepositoryItemTreeListLookUpEdit;
			BeginUpdate();
			try {
				this.lockPropertiesChanged++;
				if(source == null || !object.ReferenceEquals(currentDataSource, source.DataSource)) {
					TreeList.DataSource = null;
					this.currentDataSource = source == null ? null : source.DataSource;
				}
				base.Assign(item);
				if(source == null) return;
				this.TreeList.AssignLookUpPropertiesFrom(source.TreeList);
				this.autoComplete = source.AutoComplete;
				this.popupFilterMode = source.PopupFilterMode;
				this.autoExpandAllNodes = source.AutoExpandAllNodes;
				UpdateTreeListDataSource();
			}
			finally {
				this.lockPropertiesChanged--;
				EndUpdate();
			}
		}
		public virtual string GetDisplayTextByKeyValue(object keyValue) {
			string displayText = string.Empty;
			if(keyValue == null || IsNullValue(keyValue))
				displayText = NullText;
			else {
				string result = string.Empty;
				if(displayTextCache.TryGetValue(keyValue, out result)) {
					displayText = result;
				}
				else {
					int index = FindRowByKeyValue(keyValue);
					displayText = GetDisplayTextCore(index, keyValue);
				}
			}
			CustomDisplayTextEventArgs e = new CustomDisplayTextEventArgs(keyValue, displayText);
			RaiseCustomDisplayText(e);
			return e.DisplayText;
		}
		public int GetIndexByKeyValue(object keyValue) {
			return FindRowByKeyValue(keyValue);
		}
		public object GetRowByKeyValue(object keyValue) {
			return GetRowValue(GetIndexByKeyValue(keyValue), null);
		}
		public virtual object GetKeyValueByIndex(int index) {
			return GetRowValue(index, ValueMember);
		}
		public virtual string GetDisplayTextByIndex(int index) {
			return GetDisplayTextCore(index, GetKeyValueByIndex(index));
		}
		protected object GetKeyValueByDisplayValue(object displayValue) {
			return GetKeyValueByIndex(FindRowByValue(displayValue, DisplayMember));
		}
		protected override void OnLoaded() {
			base.OnLoaded();
			UpdateTreeListDataSource();
		}
		protected override void OnDataSourceChanged() {
			if(!IsLoading) UpdateTreeListDataSource();
			base.OnDataSourceChanged();
		}
		protected override void OnValueMemberChanged(string oldValue, string newValue) {
			base.OnValueMemberChanged(oldValue, newValue);
			UpdateTreeListDataSource();
		}
		protected internal virtual void UpdateTreeListDataSource() {
			if(TreeList.BindingContext == null)
				TreeList.BindingContext = new BindingContext();
			object oldSource = TreeList.DataSource;
			TreeList.DataSource = DataSource;
			if(oldSource != TreeList.DataSource) 
				ScheduleExpandAll();
			ClearDisplayTextCache();
		}
		bool shouldExpandAllNodes;
		internal void ScheduleExpandAll() {
			shouldExpandAllNodes = AutoExpandAllNodes;
		}
		protected internal void UpdateTreeListFocusedNode(object value) {
			if(string.IsNullOrEmpty(ValueMember))
				TreeList.SetLookUpFocusedNode(TreeList.FindNode((node) => { return TreeList.GetDataRecordByNode(node) == value; }));
			else
				TreeList.SetLookUpFocusedNode(TreeList.FindNodeByFieldValue(ValueMember, value));
		}
		protected internal void UpdateTreeListFocusedNode(int index) {
			TreeList.SetLookUpFocusedNode(TreeList.FindNodeByID(index));
		}
		protected internal object GetRowValue(int index, string fieldName) {
			if(fieldName == null || TreeList.Data.Columns[fieldName] == null)
				return TreeList.Data.GetDataRow(index);
			return TreeList.Data.GetValue(index, fieldName);
		}
		protected virtual int FindRowByKeyValue(object value) {
			return FindRowByValue(value, ValueMember);
		}
		int FindRowByValue(object value, string fieldName) {
			for(int i = 0; i < TreeList.Data.DataList.Count; i++)
				if(Object.Equals(GetRowValue(i, fieldName), value)) return i;
			return -1;
		}
		protected internal virtual int FindRowByDisplayText(string text, bool partial) {
			text = text.ToLower();
			TreeListLookUpEditFindIndexOperation op = new TreeListLookUpEditFindIndexOperation((index) => {
				object val = GetRowValue(index, ValueMember);
				string displayText = GetDisplayTextCore(index, val);
				displayText = (displayText == null) ? string.Empty : displayText.ToLower();
				if(text.Length == 0) {
					if(displayText.Length == 0)
						return true;
					return false;
				}
				if(partial)
					displayText = displayText.Substring(0, Math.Min(text.Length, displayText.Length));
				if(displayText == text)
					return true;
				return false;
			});
			TreeList.NodesIterator.DoOperation(op);
			return op.Index;
		}
		public override string GetDisplayText(FormatInfo format, object editValue) {
			if(IsNullValue(editValue)) 
				return base.GetDisplayText(format, editValue);
			if(OwnerEdit != null && !OwnerEdit.IsDisplayTextValid) {
				if(TextEditStyle == TextEditStyles.Standard) 
					return OwnerEdit.AutoSearchText;
				return base.GetDisplayText(format, editValue);
			}
			string res = GetDisplayTextByKeyValue(editValue);
			if(res == null) return string.Empty;
			return res;
		}
		protected virtual string GetDisplayTextCore(int index, object keyValue) {
			string displayText = string.Empty;
			if(index < 0) return displayText;
			object displayValue = GetRowValue(index, DisplayMember);
			if(displayValue == null || displayValue == DBNull.Value)
				return displayText;
			displayText = displayValue.ToString();
			if(keyValue == null)
				return displayText;
			displayTextCache[keyValue] = displayText;
			return displayText;
		}
		protected internal virtual void UpdateDisplayFilter() { 
			if(OwnerEdit == null) return;
			TreeList.SetExtraFilter(OwnerEdit.GetAutoSearchTextFilterCore());
		}
		protected internal void ClosePopup() {
			if(OwnerEdit != null) OwnerEdit.ClosePopup();
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(TreeList != null)
					TreeList.Dispose();
				this.treeList = null;
			}
			base.Dispose(disposing);
		}
		protected internal void ClearDisplayTextCache() {
			displayTextCache.Clear();
		}
		protected internal virtual bool OnProcessInputNewValue(string text, out object keyValue) {
			keyValue = null;
			ProcessNewValueEventArgs e = new ProcessNewValueEventArgs(text);
			RaiseProcessNewValue(e);
			if(e.Handled) {
				ClearDisplayTextCache();
				keyValue = GetKeyValueByDisplayValue(e.DisplayValue);
			}
			return e.Handled;
		}
		internal void FireChangedCore() {
			FireChanged();
		}
		internal void AddToContainer(TreeList treeList) {
			if(IsLoading) return;
			IContainer container = Container;
			if(container == null && OwnerEdit != null) container = OwnerEdit.Container;
			if(container != null) {
				try {
					container.Add(treeList, OwnerEdit == null ? Name : OwnerEdit.Name + "TreeList");
				}
				catch {
					try {
						container.Add(treeList);
					}
					catch {
					}
				}
			}
		}
		protected internal void OnTreeListPropertyChanged() {
			if(this.lockPropertiesChanged != 0 || TreeList.IsLoading) return;
			try {
				IDisposable stream = PropertyStore[TreeListLayoutKeyName] as IDisposable;
				if(stream != null) {
					PropertyStore.Remove(TreeListLayoutKeyName);
					stream.Dispose();
				}
			}
			catch { }
		}
	}
	class TreeListLookUpEditFindIndexOperation : TreeListOperation {
		Function<bool, int> matchFunction;
		public int Index { get; private set; }
		public TreeListLookUpEditFindIndexOperation(Function<bool, int> matchFunction) {
			this.Index = -1;
			this.matchFunction = matchFunction;
		}
		public override void Execute(TreeListNode node) {
			if(!node.Visible) return;
			if(matchFunction(node.Id))
				Index = node.Id;
		}
		public override bool CanContinueIteration(TreeListNode node) {
			return Index == -1;
		}
	}
}
namespace DevExpress.XtraEditors.ViewInfo {
	public class TreeListLookUpEditBaseViewInfo : PopupBaseAutoSearchEditViewInfo {
		public TreeListLookUpEditBaseViewInfo(RepositoryItem item) : base(item) { }
		public new TreeListLookUpEdit OwnerEdit { get { return base.OwnerEdit as TreeListLookUpEdit; } }
		public new RepositoryItemTreeListLookUpEdit Item { get { return base.Item as RepositoryItemTreeListLookUpEdit; } }
	}
}
