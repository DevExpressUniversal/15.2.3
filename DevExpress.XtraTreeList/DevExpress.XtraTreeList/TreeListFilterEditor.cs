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
using DevExpress.XtraEditors.Filtering;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraTreeList.Columns;
using System.Drawing;
using DevExpress.XtraGrid;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Utils;
using DevExpress.Data.Filtering;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors;
using System.Windows.Forms;
using System.ComponentModel;
using DevExpress.Utils.Menu;
using DevExpress.LookAndFeel;
using DevExpress.XtraTreeList.Localization;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.Utils.Drawing;
using DevExpress.XtraTreeList.ViewInfo;
using DevExpress.XtraTreeList.Internal;
using System.Collections;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.Helpers;
using System.Reflection;
namespace DevExpress.XtraTreeList.FilterEditor {
	public interface IFilterEditorForm : IDisposable {
		IFilterControl FilterControl { get; }
		DialogResult ShowDialog(IWin32Window owner);
	}
	[ToolboxItem(false)]
	public class FilterEditorForm : XtraForm, IFilterEditorForm {
		TreeList treeList;
		protected PanelControl controlsPanel;
		protected IFilterControl filterControlCore;
		protected SimpleButton sbOK;
		protected SimpleButton sbCancel;
		protected SimpleButton sbApply;
		PanelControl separatorPanel1;
		PanelControl separatorPanel2;
		bool allowClose = true;
		FilterEditorForm() {
			InitializeComponent();
			InitializeFormControls();
		}
		public FilterEditorForm(FilterColumnCollection filterColumns, FilterColumn defaultColumn, TreeList treeList)
			: this() {
			this.treeList = treeList;
			this.filterControlCore = CreateFilterControl();
			Control filterControl = FilterControl as Control;
			if(filterControl != null) {
				filterControl.Dock = DockStyle.Fill;
				filterControl.Parent = this;
			}
			RightToLeft = TreeList.IsRightToLeft ? RightToLeft.Yes : RightToLeft.No;
			LookAndFeel.Assign(TreeList.ElementsLookAndFeel);
			InitializeFilterControl(filterColumns, defaultColumn);
			UpdateControlsPanel();
		}
		protected virtual void UpdateControlsPanel() {
			Size bestSize = sbOK.CalcBestSize();
			controlsPanel.Height = bestSize.Height + controlsPanel.Padding.Vertical;
			sbOK.Width = Math.Max(bestSize.Width, sbOK.Width);
			sbApply.Width = Math.Max(sbApply.CalcBestSize().Width, sbApply.Width);
			sbCancel.Width = Math.Max(sbCancel.CalcBestSize().Width, sbCancel.Width);
		}
		protected virtual IFilterControl CreateFilterControl() {
			return TreeList.OptionsFilter.DefaultFilterEditorView == FilterEditorViewMode.Visual ? CreateDefaultFilterControl() : (CreateAdvancedFilterControl() ?? CreateDefaultFilterControl());
		}
		protected IFilterControl CreateDefaultFilterControl() {
			return new FilterControl();
		}
		protected IFilterControl CreateAdvancedFilterControl() {
			string assemblyName = AssemblyInfo.SRAssemblyRichEdit + ", Version=" + AssemblyInfo.Version,
				   typeName = "DevExpress.XtraFilterEditor.FilterEditorControl";
			IFilterControl filterControl = TryCreateObject(assemblyName, typeName) as IFilterControl;
			if(filterControl != null)
				filterControl.SetViewMode(TreeList.OptionsFilter.DefaultFilterEditorView);
			return filterControl;
		}
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FilterEditorForm));
			this.controlsPanel = new DevExpress.XtraEditors.PanelControl();
			this.sbOK = new DevExpress.XtraEditors.SimpleButton();
			this.separatorPanel1 = new DevExpress.XtraEditors.PanelControl();
			this.sbCancel = new DevExpress.XtraEditors.SimpleButton();
			this.separatorPanel2 = new DevExpress.XtraEditors.PanelControl();
			this.sbApply = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.controlsPanel)).BeginInit();
			this.controlsPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.separatorPanel1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.separatorPanel2)).BeginInit();
			this.SuspendLayout();
			this.controlsPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.controlsPanel.Controls.Add(this.sbOK);
			this.controlsPanel.Controls.Add(this.separatorPanel1);
			this.controlsPanel.Controls.Add(this.sbCancel);
			this.controlsPanel.Controls.Add(this.separatorPanel2);
			this.controlsPanel.Controls.Add(this.sbApply);
			resources.ApplyResources(this.controlsPanel, "controlsPanel");
			this.controlsPanel.Name = "controlsPanel";
			this.sbOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			resources.ApplyResources(this.sbOK, "sbOK");
			this.sbOK.Name = "sbOK";
			this.sbOK.Click += new System.EventHandler(this.sbOK_Click);
			this.separatorPanel1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.separatorPanel1, "separatorPanel1");
			this.separatorPanel1.Name = "separatorPanel1";
			this.sbCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.sbCancel, "sbCancel");
			this.sbCancel.Name = "sbCancel";
			this.sbCancel.Click += new System.EventHandler(this.sbCancel_Click);
			this.separatorPanel2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.separatorPanel2, "separatorPanel2");
			this.separatorPanel2.Name = "separatorPanel2";
			resources.ApplyResources(this.sbApply, "sbApply");
			this.sbApply.Name = "sbApply";
			this.sbApply.Click += new System.EventHandler(this.sbApply_Click);
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.sbCancel;
			this.Controls.Add(this.controlsPanel);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FilterEditorForm";
			this.ShowInTaskbar = false;
			((System.ComponentModel.ISupportInitialize)(this.controlsPanel)).EndInit();
			this.controlsPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.separatorPanel1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.separatorPanel2)).EndInit();
			this.ResumeLayout(false);
		}
		public IFilterControl FilterControl {
			get { return filterControlCore; }
		}
		protected TreeList TreeList {
			get { return treeList; }
		}
		protected virtual void InitializeFormControls() {
			sbOK.Text = TreeListLocalizer.Active.GetLocalizedString(TreeListStringId.FilterEditorOkButton);
			sbCancel.Text = TreeListLocalizer.Active.GetLocalizedString(TreeListStringId.FilterEditorCancelButton);
			sbApply.Text = TreeListLocalizer.Active.GetLocalizedString(TreeListStringId.FilterEditorApplyButton);
			Text = TreeListLocalizer.Active.GetLocalizedString(TreeListStringId.FilterEditorCaption);
			Icon = new Icon((typeof(FilterEditorForm).Assembly.GetManifestResourceStream("DevExpress.XtraTreeList.Images.FilterEditor.ico")));
		}
		protected virtual void InitializeFilterControl(FilterColumnCollection filterColumns, FilterColumn defaultColumn) {
			FilterControl.SetFilterColumnsCollection(filterColumns, TreeList.MenuManager);
			FilterControl.SetDefaultColumn(defaultColumn);
			FilterControl.FilterCriteria = TreeList.ActiveFilterCriteria;
			FilterControl.LookAndFeel.Assign(TreeList.ElementsLookAndFeel);
			BringToFrontCore();
		}
		void BringToFrontCore() {
			Control filterControl = FilterControl as Control;
			if(filterControl != null)
				filterControl.BringToFront();
		}
		protected virtual void ApplyFilter() {
			try {
				allowClose = true;
				TreeList.ActiveFilterCriteria = FilterControl.FilterCriteria; 
			}
			catch(Exception e) {
				XtraMessageBox.Show(LookAndFeel, e.Message, TreeListLocalizer.Active.GetLocalizedString(TreeListStringId.WindowErrorCaption), MessageBoxButtons.OK, MessageBoxIcon.Error);
				allowClose = false;
			}
		}
		protected override void OnFormClosing(FormClosingEventArgs e) {
			base.OnFormClosing(e);
			e.Cancel = !allowClose;
		}
		void sbOK_Click(object sender, System.EventArgs e) {
			ApplyFilter();
		}
		void sbApply_Click(object sender, System.EventArgs e) {
			ApplyFilter();
		}
		void sbCancel_Click(object sender, EventArgs e) {
			allowClose = true;
		}
		#region helpers
		object TryCreateObject(string assemblyName, string typeName) {
			try {
				Type constructorType = DevExpress.Xpo.Helpers.XPTypeActivator.GetType(assemblyName, typeName);
				if(constructorType != null) {
					ConstructorInfo constructorInfoObj = constructorType.GetConstructor(Type.EmptyTypes);
					if(constructorInfoObj != null)
						return constructorInfoObj.Invoke(null);
				}
			}
			catch { }
			return null;
		}
		#endregion
	}
	public class TreeListFilterColumn : FilterColumn {
		TreeListColumn column;
		TreeList treeList;
		RepositoryItem editor;
		Type type;
		string caption;
		Image image;
		public TreeListFilterColumn(TreeListColumn column)
			: base() {
			this.column = column;
			this.treeList = column.TreeList;
		}
		public TreeListColumn Column { get { return column; } }
		public override string FieldName { get { return Column.FieldName; } }
		public override string ColumnCaption {
			get {
				if(caption == null) 
					caption = GetCaption();
				return caption;
			}
		}
		public override Type ColumnType {
			get {
				if(type == null)
					type = GetColumnType();
				return type;
			}
		}
		public override Image Image {
			get {
				if(image == null)
					image = GetImage();
				return image;
			}
		}
		public override RepositoryItem ColumnEditor {
			get {
				if(editor == null)
					editor = CreateRepository();
				return editor;
			}
		}
		public override FilterColumnClauseClass ClauseClass {
			get {
				if(GetColumnFilterMode() == ColumnFilterMode.DisplayText)
					return FilterColumnClauseClass.String;
				if(ColumnEditor is RepositoryItemLookUpEditBase || ColumnEditor is RepositoryItemImageComboBox || ColumnEditor is RepositoryItemRadioGroup)
					return FilterColumnClauseClass.Lookup;
				if(ColumnEditor is RepositoryItemPictureEdit || ColumnEditor is RepositoryItemImageEdit)
					return FilterColumnClauseClass.Blob;
				if(Column.ColumnType == typeof(string))
					return FilterColumnClauseClass.String;
				if(Column.ColumnType == typeof(DateTime) || Column.ColumnType == typeof(DateTime?))
					return FilterColumnClauseClass.DateTime;
				return FilterColumnClauseClass.Generic;
			}
		}
		protected virtual RepositoryItem CreateRepository() {
			if(GetColumnFilterMode() == ColumnFilterMode.DisplayText)
				return new RepositoryItemTextEdit();
			RepositoryItem columnEdit = treeList.GetColumnEdit(Column);
			if(columnEdit == null)
				return new RepositoryItemTextEdit();
			if(columnEdit is RepositoryItemBaseProgressBar)
				return new RepositoryItemSpinEdit();
			if(columnEdit is RepositoryItemMemoEdit || IsRichTextEdit(columnEdit)) 
				return new RepositoryItemMemoExEdit() { ShowIcon = false };
			if(columnEdit is RepositoryItemRadioGroup) {
				RepositoryItemImageComboBox edit = new RepositoryItemImageComboBox();
				foreach(RadioGroupItem item in ((RepositoryItemRadioGroup)columnEdit).Items)
					edit.Items.Add(new ImageComboBoxItem(item.Description, item.Value, -1));
				return edit;
			}
			RepositoryItem result = (RepositoryItem)columnEdit.Clone();
			result.Assign(columnEdit);
			result.ResetEvents();
			return result;
		}
		protected virtual Image GetImage() { 
			return ImageCollection.GetImageListImage(Column.Images, Column.ImageIndex);
		}
		protected ColumnFilterMode GetColumnFilterMode() {
			return treeList.GetColumnFilterMode(Column);
		}
		protected virtual string GetCaption() {
			string columnCaption = Column.GetTextCaption();
			if(columnCaption == null)
				columnCaption = string.Empty;
			columnCaption = columnCaption.Replace('\n', ' ');
			columnCaption = columnCaption.Replace('\r', ' ');
			columnCaption = columnCaption.Replace('\t', ' ');
			if(columnCaption != Column.GetTextCaption())
				columnCaption.Replace("  ", " ").Replace("  ", " ");
			return columnCaption;
		}
		protected virtual Type GetColumnType() {
			if(GetColumnFilterMode() == ColumnFilterMode.DisplayText) return typeof(string);
			Type result = Column.ColumnType;
			Type underlyingType = Nullable.GetUnderlyingType(result);
			if(underlyingType != null)
				result = underlyingType;
			return result;
		}
		public override void Dispose() {
			base.Dispose();
			if(editor != null) {
				editor.Dispose();
				editor = null;
			}
		}
		public virtual void SetRepositoryItem(RepositoryItem item) {
			editor = item;
		}
		public override void SetColumnEditor(RepositoryItem item) {
			SetRepositoryItem(item);
		}
		public override void SetColumnCaption(string caption) {
			this.caption = caption;
		}
		public override void SetImage(Image image) {
			this.image = image;
		}
		static bool IsRichTextEdit(RepositoryItem item) {
			if(item == null) return false;
			return item.GetType().Name.Contains("RichTextEdit");
		}
	}
	public class TreeListFilterColumnCollection : FilterColumnCollection {
		TreeList treeList;
		public TreeListFilterColumnCollection(TreeList treeList) {
			this.treeList = treeList;
			CreateColumns();
		}
		protected void CreateColumns() {
			foreach(TreeListColumn column in treeList.Columns) {
				if(!IsValidForFilter(column)) continue;
				Add(CreateFilterColumn(column));
			}
		}
		protected virtual FilterColumn CreateFilterColumn(TreeListColumn column) {
			return new TreeListFilterColumn(column);
		}
		protected virtual bool IsValidForFilter(TreeListColumn column) {
			if(!column.Visible && !column.OptionsColumn.ShowInCustomizationForm) return false;
			return column.OptionsFilter.AllowFilter;
		}
		public override string GetValueScreenText(OperandProperty property, object value) {
			FilterColumn col = this[property];
			if(col == null)
				return base.GetValueScreenText(property, value);
			TreeListColumn column = treeList.Columns[col.FieldName];
			if(column == null)
				return base.GetValueScreenText(property, value);
			return treeList.GetFilterDisplayTextByColumn(column, value);
		}
	}
	public abstract class TreeListFilterPopupBase : IDisposable {
		TreeList treeList;
		RepositoryItemPopupBase item;
		PopupEditActivator popupActivator;
		bool isInitialized = false;
		public TreeListFilterPopupBase(TreeList treeList) {
			this.treeList = treeList;
		}
		public TreeList TreeList { get { return treeList; } }
		public RepositoryItemPopupBase Item { get { return item; } }
		public bool IsFocused { get { return PopupActivator.ContainsFocus; } }
		protected PopupEditActivator PopupActivator { get { return popupActivator; } }
		protected bool IsInitialized { get { return isInitialized; } }
		public void Init() {
			if(IsInitialized) return;
			this.isInitialized = true;
			this.item = CreateRepositoryItem();
			if(Item == null) return;
			this.InitializeItem(Item);
			this.popupActivator = new PopupEditActivator(false, true) { Owner = TreeList, PopupItem = Item };
			this.popupActivator.CloseUp += OnPopupActivatorCloseUp;
		}
		public virtual bool CanShow { get { return Item != null; } }
		public virtual void Show(Rectangle ownerBounds) {
			if(!CanShow) return;
			PopupActivator.ShowPopup(ownerBounds);
		}
		protected internal virtual void OnBeforeShow() { }
		protected abstract RepositoryItemPopupBase CreateRepositoryItem();
		protected virtual void InitializeItem(RepositoryItemPopupBase item) {   }
		protected virtual void OnPopupActivatorCloseUp(object sender, CloseUpEventArgs e) {  }
		public virtual void Dispose() {
			if(Item != null) Item.Dispose();
			if(PopupActivator != null) {
				PopupActivator.CloseUp -= OnPopupActivatorCloseUp;
				PopupActivator.DestroyPopup();
				PopupActivator.Dispose();
			}
		}
	}
	public class TreeListMRUFilterPopup : TreeListFilterPopupBase {
		TreeList treeList;
		public TreeListMRUFilterPopup(TreeList treeList) : base(treeList) {
			this.treeList = treeList;
		}
		protected override RepositoryItemPopupBase CreateRepositoryItem() {
			return new RepositoryItemMruFilterCombo(this);
		}
		protected override void InitializeItem(RepositoryItemPopupBase item) {
			RepositoryItemMruFilterCombo mruComboBox = (RepositoryItemMruFilterCombo)item;
			foreach(TreeListFilterInfo filter in treeList.MRUFilters) {
				if(Equals(filter.FilterCriteria, treeList.ActiveFilterCriteria)) continue;
				if(filter.DisplayText == null)
					filter.SetDisplayText(TreeList.GetFilterDisplayText(filter.FilterCriteria));
				mruComboBox.Items.Add(filter);
			}
			mruComboBox.DropDownRows = treeList.OptionsFilter.MRUFilterListPopupCount;
		}
		protected override void OnPopupActivatorCloseUp(object sender, CloseUpEventArgs e) {
			if(e.AcceptValue){
				TreeListFilterInfo info = e.Value as TreeListFilterInfo;
				if(info != null)
					treeList.ActiveFilterCriteria = info.FilterCriteria;
			}
			treeList.MRUFilterPopup = null;
			ResetFilterPanelState();
		}
		void ResetFilterPanelState() {
			treeList.ViewInfo.FilterPanel.TextState = ObjectState.Normal;
			treeList.ViewInfo.FilterPanel.MRUButtonInfo.State = ObjectState.Normal;
			treeList.LayoutChanged();
		}
		protected internal void RemoveMruItem(TreeListFilterInfo item) {
			treeList.MRUFilters.Remove(item);
		}
		#region inner classes
		class RepositoryItemMruFilterCombo : RepositoryItemComboBox {
			TreeListMRUFilterPopup filterPopup;
			public RepositoryItemMruFilterCombo(TreeListMRUFilterPopup filterPopup) {
				this.filterPopup = filterPopup;
			}
			public override BaseEdit CreateEditor() {
				return new MruFilterCombo(filterPopup);
			}
		}
		class MruFilterCombo : ComboBoxEdit {
			TreeListMRUFilterPopup filterPopup;
			public MruFilterCombo(TreeListMRUFilterPopup filterPopup) {
				this.filterPopup = filterPopup;
			}
			protected override void OnActionItemClick(ListItemActionInfo action) {
				TreeListFilterInfo item = action.Item as TreeListFilterInfo;
				if(!SilentRemove(item)) return;
				this.filterPopup.RemoveMruItem(item);
				if(IsPopupOpen) RefreshPopup();
				if(!CanShowPopup) ClosePopup(PopupCloseMode.Cancel);
			}
			protected override bool HasItemActions { get { return true; } }
			protected override void CreateItemActions(BaseListBoxViewInfo.ItemInfo itemInfo) {
				itemInfo.ActionInfo = new ListItemActionCollection();
				itemInfo.ActionInfo.Add(new ListItemDeleteActionInfo(itemInfo));
			}
		}
		#endregion
	}
	public abstract class TreeListColumnFilterPopupBase : TreeListFilterPopupBase {
		TreeListColumn column;
		ColumnFilterPopupDefaultItems defaultItems;
		public TreeListColumnFilterPopupBase(TreeList treeList, TreeListColumn column)
			: base(treeList) {
			this.column = column;
			this.defaultItems = new ColumnFilterPopupDefaultItems(this);
		}
		protected ColumnFilterPopupDefaultItems DefaultFilterPopupItems { get { return defaultItems; } }
		protected bool UseDisplayText { get { return TreeList.GetColumnFilterMode(Column) == ColumnFilterMode.DisplayText; } }
		public TreeListColumn Column { get { return column; } }
		public override void Show(Rectangle ownerBounds) {
			Size columnFilterSize = Column.FilterPopupSize;
			if(Item != null) {
				if(!columnFilterSize.IsEmpty)
					Item.PopupFormSize = columnFilterSize;
			}
			base.Show(ownerBounds);
		}
		protected override void InitializeItem(RepositoryItemPopupBase item) {
			base.InitializeItem(item);
			item.LookAndFeel.Assign(TreeList.ElementsLookAndFeel);
		}
		protected override void OnPopupActivatorCloseUp(object sender, CloseUpEventArgs e) {
			Column.FilterPopupSize = Item.PopupFormSize;
			OnPopupActivatorCloseUpCore(e);
			TreeList.FilterPopup = null;
			ResetFilterButtonState(Column);
		}
		protected virtual void OnPopupActivatorCloseUpCore(CloseUpEventArgs e) {  }
		void ResetFilterButtonState(TreeListColumn column) {
			if(column == null) return;
			ColumnInfo ci = TreeList.ViewInfo.ColumnsInfo[column];
			if(ci == null) return;
			ci.SetFilterButtonState(ObjectState.Normal);
			ci.SetFilterButtonVisible(!column.FilterInfo.IsEmpty);
			TreeList.ViewInfo.PaintAnimatedItems = false;
			TreeList.Invalidate(ci.Bounds);
		}
		protected object[] GetFilterValues(bool showAll, bool displayText) {
			return TreeList.FilterHelper.GetFilterPopupValues(Column, showAll, displayText);
		}
		protected virtual bool AllowBlankItems { get { return Column.OptionsFilter.ShowBlanksFilterItems != DefaultBoolean.False; } }
		protected virtual List<FilterItem> GetFilterItems(object[] values, bool displayText, out bool hasNullValues) {
			hasNullValues = false;
			if(values == null) return null;
			List<FilterItem> result = new List<FilterItem>();
			for(int n = 0; n < values.Length; n++) {
				object value = values[n];
				FilterItem item;
				if(IsNullValue(value)) {
					hasNullValues = true;
					continue;
				}
				if(!displayText)
					item = new FilterItem(TreeList.GetFilterDisplayTextByColumn(Column, value), value);
				else
					item = new FilterItem(value.ToString(), value);
				result.Add(item);
			}
			if(displayText)
				result.Sort(new FilterItemComparer());
			return result;
		}
		protected bool IsNullValue(object value) {
			return value == null || value == DBNull.Value || string.Empty.Equals(value);
		}
		#region inner classes
		public class ColumnFilterPopupDefaultItems {
			public readonly TreeListFilterInfo PopupFilterAll;
			public readonly TreeListFilterInfo PopupFilterBlanks;
			public readonly TreeListFilterInfo PopupFilterNonBlanks;
			TreeListColumnFilterPopupBase filterPopup;
			public ColumnFilterPopupDefaultItems(TreeListColumnFilterPopupBase filterPopup) {
				this.filterPopup = filterPopup;
				PopupFilterAll = new TreeListFilterInfo(null, TreeListLocalizer.Active.GetLocalizedString(TreeListStringId.PopupFilterAll));
				PopupFilterBlanks = new TreeListFilterInfo(CreateDefaultCriteriaOperator(true), TreeListLocalizer.Active.GetLocalizedString(TreeListStringId.PopupFilterBlanks));
				PopupFilterNonBlanks = new TreeListFilterInfo(CreateDefaultCriteriaOperator(false), TreeListLocalizer.Active.GetLocalizedString(TreeListStringId.PopupFilterNonBlanks));
			}
			protected TreeListColumn Column { get { return filterPopup.Column; } }
			protected TreeList TreeList { get { return filterPopup.TreeList; } }
			public bool IsDefaultItem(TreeListFilterInfo info) {
				if(info == null) return false;
				return info.Equals(PopupFilterAll) || info.Equals(PopupFilterBlanks) || info.Equals(PopupFilterNonBlanks);
			}
			CriteriaOperator CreateDefaultCriteriaOperator(bool isBlanks) {
				if(TreeList.GetColumnFilterMode(Column) == ColumnFilterMode.Value) {
					if(isBlanks)
						return new OperandProperty(Column.FieldName).IsNull();
					else
						return new OperandProperty(Column.FieldName).IsNotNull();
				}
				else {
					if(isBlanks)
						return new OperandProperty(Column.FieldName) == string.Empty;
					else
						return new OperandProperty(Column.FieldName) != string.Empty;
				}
			}
		}
		#endregion
	}
	public class TreeListColumnFilterPopup : TreeListColumnFilterPopupBase {
		public TreeListColumnFilterPopup(TreeList treeList, TreeListColumn column)
			: base(treeList, column) {
		}
		public new RepositoryItemFilterComboBox Item { get { return base.Item as RepositoryItemFilterComboBox; } }
		public override bool CanShow { get { return base.CanShow && Item.Items.Count > 0; } }
		protected override RepositoryItemPopupBase CreateRepositoryItem() {
			return new RepositoryItemFilterComboBox(this) { PopupSizeable = true };
		}
		public override void Show(Rectangle ownerBounds) {
			int columnFilterWidth = Column.FilterPopupSize.Width;
			if(Item != null) {
				if(columnFilterWidth > 0)
					Item.PopupFormSize = new Size(columnFilterWidth, 0);
			}
			base.Show(ownerBounds);
		}
		protected internal override void OnBeforeShow() {
			TreeList.RaiseColumnFilterPopupEvent(this);
		}
		protected override void InitializeItem(RepositoryItemPopupBase item) {
			base.InitializeItem(item);
			RepositoryItemFilterComboBox columnComboBox = (RepositoryItemFilterComboBox)item;
			columnComboBox.DropDownRows = Math.Max(2, TreeList.OptionsFilter.ColumnFilterPopupRowCount);
			InitMRUItems(columnComboBox);
			InitValueItems(columnComboBox);
		}
		void InitDefaultItems(RepositoryItemFilterComboBox columnComboBox, bool hasNullValues) {
			if(!Column.FilterInfo.IsEmpty)
				columnComboBox.Items.Add(DefaultFilterPopupItems.PopupFilterAll);
			if(AllowBlankItems) {
				if((hasNullValues && Column.OptionsFilter.ShowBlanksFilterItems == DefaultBoolean.Default) || Column.OptionsFilter.ShowBlanksFilterItems == DefaultBoolean.True) {
					columnComboBox.Items.Add(DefaultFilterPopupItems.PopupFilterBlanks);
					columnComboBox.Items.Add(DefaultFilterPopupItems.PopupFilterNonBlanks);
				}
			}
		}
		void InitMRUItems(RepositoryItemFilterComboBox columnComboBox) {
			if(!TreeList.OptionsFilter.AllowColumnMRUFilterList) return;
			TreeListColumnFilterInfo active = Column.FilterInfo;
			int count = 0;
			foreach(TreeListFilterInfo mru in Column.MRUFilters) {
				if(ReferenceEquals(mru.FilterCriteria, active.FilterCriteria)) continue;
				if(mru.DisplayText == null)
					mru.SetDisplayText(TreeList.GetFilterDisplayText(mru.FilterCriteria));
				columnComboBox.Items.Add(mru);
				if(++count >= TreeList.OptionsFilter.MRUColumnFilterListCount) break;
			}
			if(count > 0) columnComboBox.Items.Add(FilterComboBox.MRUDivider);
		}
		void InitValueItems(RepositoryItemFilterComboBox columnComboBox) {
			bool hasNullValues;
			object[] values = GetFilterValues(!Column.FilterInfo.IsEmpty || TreeList.OptionsFilter.ShowAllValuesInFilterPopup || Control.ModifierKeys == Keys.Shift, UseDisplayText);
			List<FilterItem> items = GetFilterItems(values, UseDisplayText, out hasNullValues);
			InitDefaultItems(columnComboBox, hasNullValues);
			if(items == null) return;
			columnComboBox.Items.AddRange(items);
		}
		protected override void OnPopupActivatorCloseUpCore(CloseUpEventArgs e) {
			if(e.AcceptValue && e.CloseMode != PopupCloseMode.Immediate) {
				TreeListFilterInfo info = e.Value as TreeListFilterInfo;
				object value = null;
				if(info == null) {
					FilterItem itemValue = e.Value as FilterItem;
					if(itemValue != null) {
						TreeListFilterInfo fi = itemValue.Value as TreeListFilterInfo;
						if(fi != null) {
							info = fi;
						}
						else {
							CriteriaOperator filterCriteria = TreeList.FilterHelper.CreateColumnFilterCriteriaByValue(TreeList.Data.GetDataColumnInfo(Column.FieldName), itemValue.Value, UseDisplayText, TreeList.IsRoundDateTime(Column), null);
							if(!ReferenceEquals(null, filterCriteria))
								info = new TreeListFilterInfo(filterCriteria, itemValue.DisplayText);
							value = itemValue.Value;
						}
					}
				}
				if(info != null) {
					if(!DefaultFilterPopupItems.IsDefaultItem(info))
						Column.MRUFilters.AddMRUFilter(info, TreeList.OptionsFilter.MRUColumnFilterListCount);
					Column.FilterInfo.Set(info.FilterCriteria, value);
					TreeList.ApplyColumnFilterInternal(Column);
				}
			}
		}
		#region inner classes
		[ToolboxItem(false)]
		public class RepositoryItemFilterComboBox : RepositoryItemComboBox {
			TreeListColumnFilterPopup filterPopup;
			const string PopupSizeKey = "ComboPopupSize";
			readonly Size defaultMinPopupSize = new Size(100, 25);
			public RepositoryItemFilterComboBox(TreeListColumnFilterPopup filterPopup) {
				this.filterPopup = filterPopup;
				this.PopupFormMinSize = defaultMinPopupSize;
			}
			public override Size PopupFormSize {
				get {
					object size = PropertyStore[PopupSizeKey];
					if(size == null) return Size.Empty;
					return (Size)size;
				}
				set {
					PropertyStore[PopupSizeKey] = value;
				}
			}
			public override BaseEdit CreateEditor() {
				return new FilterComboBox(filterPopup);
			}
		}
		class FilterComboBox : ComboBoxEdit {
			public const string MRUDivider = "-";
			TreeListColumnFilterPopup filterPopup;
			public FilterComboBox(TreeListColumnFilterPopup filterPopup) {
				this.filterPopup = filterPopup;
			}
			protected override void OnPopupShown() {
				base.OnPopupShown();
				if(PopupForm != null) PopupForm.ListBox.HotTrackSelectMode = HotTrackSelectMode.SelectItemOnHotTrackEx;
			}
			protected override void OnPopupSelectedIndexChanged() {
				AutoSearchText = string.Empty;
				base.OnPopupSelectedIndexChanged();
			}
			protected override void RaiseDropDownCustomDrawItem(ListBoxDrawItemEventArgs e) {
				base.RaiseDropDownCustomDrawItem(e);
				if(object.Equals(e.Item, MRUDivider)) {
					DrawMruDivider(e);
					e.Handled = true;
					return;
				}
				CheckHighlightPopupAutoSearchText(e);
			}
			void DrawMruDivider(ListBoxDrawItemEventArgs e) {
				Rectangle r = e.Bounds;
				r.Inflate(-1, 0);
				r.Height = 1;
				r.Y += e.Bounds.Height / 2 - 2;
				Color clr = SystemColors.GrayText;
				e.Cache.Paint.FillRectangle(e.Graphics, e.Cache.GetSolidBrush(clr), r);
				r.Y += 2;
				e.Cache.Paint.FillRectangle(e.Graphics, e.Cache.GetSolidBrush(clr), r);
			}
			protected override bool HasItemActions { get { return true; } }
			protected override void CreateItemActions(BaseListBoxViewInfo.ItemInfo itemInfo) {
				TreeListFilterInfo info = itemInfo.Item as TreeListFilterInfo;
				if(info == null) return;
				if(filterPopup.Column.MRUFilters.Contains(info)) {
					itemInfo.ActionInfo = new ListItemActionCollection();
					itemInfo.ActionInfo.Add(new ListItemDeleteActionInfo(itemInfo));
				}
			}
			protected override void OnActionItemClick(ListItemActionInfo action) {
				if(!SilentRemove(action.Item)) return;
				if(Properties.Items.Count > 0 && Object.Equals(Properties.Items[0], MRUDivider))
					SilentRemove(MRUDivider);
				TreeListFilterInfo item = action.Item as TreeListFilterInfo;
				if(item == null) return;
				filterPopup.Column.MRUFilters.Remove(item);
				if(IsPopupOpen) RefreshPopup();
				if(!CanShowPopup) ClosePopup(PopupCloseMode.Cancel);
			}
		}
		#endregion
	}
	public class TreeListCheckedColumnFilterPopup : TreeListColumnFilterPopupBase {
		public TreeListCheckedColumnFilterPopup(TreeList treeList, TreeListColumn column) : base(treeList, column) {  }
		public new RepositoryItemFilterComboBox Item { get { return base.Item as RepositoryItemFilterComboBox; } }
		protected override RepositoryItemPopupBase CreateRepositoryItem() {
			return new RepositoryItemFilterComboBox(this) { IncrementalSearch = true, AllowMultiSelect = true }; 
		}
		protected override void InitializeItem(RepositoryItemPopupBase item) {
			base.InitializeItem(item);
			RepositoryItemFilterComboBox columnComboBox = (RepositoryItemFilterComboBox)item;
			columnComboBox.DropDownRows = Math.Max(2, TreeList.OptionsFilter.ColumnFilterPopupRowCount);
			InitValueItems(columnComboBox);
		}
		public override bool CanShow { get { return base.CanShow && Item.Items.Count > 0; } }
		void InitDefaultItems(RepositoryItemFilterComboBox columnComboBox, bool hasNullValues) {
			if(AllowBlankItems) {
				if((hasNullValues && Column.OptionsFilter.ShowBlanksFilterItems == DefaultBoolean.Default) || Column.OptionsFilter.ShowBlanksFilterItems == DefaultBoolean.True) 
					columnComboBox.Items.Add(DefaultFilterPopupItems.PopupFilterBlanks);
			}
		}
		void InitValueItems(RepositoryItemFilterComboBox columnComboBox) {
			bool hasNullValues = false;
			object[] values = GetFilterValues(!Column.FilterInfo.IsEmpty || TreeList.OptionsFilter.ShowAllValuesInCheckedFilterPopup || Control.ModifierKeys == Keys.Shift, UseDisplayText);
			List<FilterItem> items = GetFilterItems(values, UseDisplayText, out hasNullValues);
			InitDefaultItems(columnComboBox, hasNullValues);
			if(items == null) return;
			columnComboBox.Items.AddRange(items.ToArray());
		}
		protected internal override void OnBeforeShow() {
			TreeList.RaiseCheckedColumnFilterPopupEvent(this);
		}
		protected override void OnPopupActivatorCloseUpCore(CloseUpEventArgs e) {
			base.OnPopupActivatorCloseUpCore(e);
			if(e.AcceptValue && e.CloseMode != PopupCloseMode.Immediate) {
				List<object> checkedValues = Item.Items.GetCheckedValues();
				if(checkedValues != null)
					ApplyColumnFilter(Item.Items.GetCheckedValues());
			}
		}
		protected virtual void ApplyColumnFilter(List<object> checkedValues) {
			bool hasBlanks = false;
			InOperator op = new InOperator(new OperandProperty(Column.FieldName));
			foreach(object item in checkedValues) {
				TreeListFilterInfo info = item as TreeListFilterInfo;
				if(info != null && info == DefaultFilterPopupItems.PopupFilterBlanks)
					hasBlanks = true;
				FilterItem fi = item as FilterItem;
				if(fi != null)
					op.Operands.Add(new OperandValue(fi.Value));
			}
			CriteriaOperator result = op;
			switch(op.Operands.Count) {
				case 0:
					result = null;
					break;
				case 1:
					result = op.LeftOperand == ((CriteriaOperator)op.Operands[0]);
					break;
				case 2:
					result = op.LeftOperand == ((CriteriaOperator)op.Operands[0]) | op.LeftOperand == ((CriteriaOperator)op.Operands[1]);
					break;
			}
			if(hasBlanks) result |= new FunctionOperator(FunctionOperatorType.IsNullOrEmpty, new OperandProperty(Column.FieldName));
			Column.FilterInfo.Set(result, null);
			TreeList.ApplyColumnFilterInternal(Column);
		}
		#region inner classes
		[ToolboxItem(false)]
		public class RepositoryItemFilterComboBox : RepositoryItemCheckedComboBoxEdit {
			const string PopupSizeKey = "BlobSize";
			const string EmptyFilter = "<XtraTreeList Empty Filter>";
			TreeListCheckedColumnFilterPopup filterPopup;
			public RepositoryItemFilterComboBox(TreeListCheckedColumnFilterPopup filterPopup) {
				this.filterPopup = filterPopup;
			}
			protected TreeListColumn Column { get { return filterPopup.Column; } }
			public override Size PopupFormSize {
				get {
					object size = PropertyStore[PopupSizeKey];
					if(size == null) return Size.Empty;
					return (Size)size;
				}
				set {
					PropertyStore[PopupSizeKey] = value;
				}
			}
			public override BaseEdit CreateEditor() {
				return new CheckedComboBoxEdit();
			}
			protected override void OnOwnerEditChanged() {
				base.OnOwnerEditChanged();
				if(OwnerEdit != null) {
					CriteriaOperator op = null;
					bool hasNull = IsNullOrEmptyEliminator.Eliminate(Column.FilterInfo.FilterCriteria, Column.FieldName, out op);
					string[] strings = GetStringsByCriteria(Items, hasNull ? op : Column.FilterInfo.FilterCriteria);
					if(strings != null) {
						string result;
						if(strings.Length == 0)
							result = EmptyFilter;
						else
							result = string.Join(SeparatorChar.ToString(), strings);
						if(hasNull)
							result += SeparatorChar + TreeListLocalizer.Active.GetLocalizedString(TreeListStringId.PopupFilterBlanks);
						OwnerEdit.EditValue = result;
					}
				}
			}
			string GetStringByValue(IEnumerable items, CriteriaOperator op) {
				OperandValue value = op as OperandValue;
				if(ReferenceEquals(value, null))
					return null;
				foreach(object obj in items) {
					ListBoxItem item = (ListBoxItem)obj;
					FilterItem filterValue = item.Value as FilterItem;
					if(filterValue == null)
						continue;
					if(Equals(filterValue.Value, value.Value)) 
						return filterValue.DisplayText;
				}
				return null;
			}
			string[] GetStringsByCriteria(IEnumerable items, CriteriaOperator op) {
				ArrayList rv = new ArrayList();
				if(ReferenceEquals(op, null))
					return new string[0];
				GroupOperator grop = op as GroupOperator;
				if(!ReferenceEquals(grop, null)) {
					if(grop.OperatorType != GroupOperatorType.Or && grop.Operands.Count > 1)
						return null;
					foreach(CriteriaOperator nop in grop.Operands) {
						IList nres = GetStringsByCriteria(items, nop);
						if(nres == null)
							return null;
						rv.AddRange(nres);
					}
				}
				else {
					BinaryOperator bop = op as BinaryOperator;
					if(!ReferenceEquals(bop, null)) {
						if(bop.OperatorType != BinaryOperatorType.Equal)
							return null;
						string toAdd = GetStringByValue(items, bop.RightOperand);
						if(toAdd != null)
							rv.Add(toAdd);
					}
					else {
						InOperator iop = op as InOperator;
						if(!ReferenceEquals(iop, null)) {
							foreach(CriteriaOperator rop in iop.Operands) {
								string toAdd = GetStringByValue(items, rop);
								if(toAdd != null)
									rv.Add(toAdd);
							}
						}
						else
							return null;
					}
				}
				return (string[])rv.ToArray(typeof(string));
			}
		}
		#endregion
	}
	public class TreeListDateFilterPopup : TreeListColumnFilterPopupBase, IPopupOutlookDateFilterOwner {
		public TreeListDateFilterPopup(TreeList treeList, TreeListColumn column) : base(treeList, column) { }
		public CriteriaOperator ActiveFilterCriteria { get { return Column.FilterInfo.FilterCriteria; } }
		new RepositoryItemDateFilterPopupEdit Item { get { return base.Item as RepositoryItemDateFilterPopupEdit; } }
		protected override RepositoryItemPopupBase CreateRepositoryItem() {
			return new RepositoryItemDateFilterPopupEdit(this);
		}
		protected override bool AllowBlankItems { get { return false; } }		
		protected override void OnPopupActivatorCloseUpCore(CloseUpEventArgs e) {
			base.OnPopupActivatorCloseUpCore(e);
			if(e.AcceptValue && e.CloseMode != PopupCloseMode.Immediate) {
				DateFilterResult result = Item.GetResult();
				if(result != null)
					ApplyFilterResult(result);
			}
		}
		protected object[] GetDateFilterValues() {
			return GetFilterValues(true, UseDisplayText);
		}
		#region IPopupOutlookDateFilterOwner
		void IPopupOutlookDateFilterOwner.RaiseFilterListUpdate(List<FilterDateElement> list) {
			TreeList.RaiseFilterPopupDate(this, list);
		}
		void IPopupOutlookDateFilterOwner.OnDateModified() {
			ApplyImmediateFilter();
		}
		void IPopupOutlookDateFilterOwner.OnCheckedChanged() {
			ApplyImmediateFilter();
		}
		protected void ApplyImmediateFilter() {
			if(Column.OptionsFilter.ImmediateUpdatePopupDateFilter == DefaultBoolean.False) return;
			DateFilterResult result = Item.GetImmediateResult();
			if(result != null) 
				ApplyFilterResult(result);
		}
		void ApplyFilterResult(DateFilterResult result) {
			Column.FilterInfo.Set(result.FilterCriteria, null);
			TreeList.SaveDateFilterInfo(Column, new TreeListDateFilterInfoCache() { LastFilterResult = result, Cache = Item.FilterInfoCache });
			TreeList.ApplyColumnFilterInternal(Column);
		}
		#endregion
		#region inner classes
		class RepositoryItemDateFilterPopupEdit : RepositoryItemPopupContainerEdit {
			TreeListDateFilterPopup filterPopup;
			PopupOutlookDateFilterControl dateFilterControl;
			DateFilterInfoCache filterInfoCache;
			public RepositoryItemDateFilterPopupEdit(TreeListDateFilterPopup filterPopup) {
				this.filterPopup = filterPopup;
				this.PopupControl = new PopupContainerControl();
				this.dateFilterControl = new PopupOutlookDateFilterControl(filterPopup);
				InitializeDateFilterControl(dateFilterControl);
				CloseActAsOkButton = true;
				PopupSizeable = false;
			}
			internal DateFilterInfoCache FilterInfoCache { get { return filterInfoCache; } }
			protected TreeListColumn Column { get { return filterPopup.Column; } }
			protected PopupOutlookDateFilterControl DateFilterControl { get { return dateFilterControl; } }
			protected virtual void InitializeDateFilterControl(PopupOutlookDateFilterControl dateFilterControl) {
				TreeListDateFilterInfoCache info = filterPopup.TreeList.GetCachedDateFilterInfo(Column);
				if(info != null) {
					filterInfoCache = info.Cache;
				}
				else {
					filterInfoCache = new DateFilterInfoCache();
					filterInfoCache.Init(filterPopup.GetDateFilterValues());
				}
				dateFilterControl.Cache = filterInfoCache;
				dateFilterControl.Field = new TreeListFilterColumn(Column);
				dateFilterControl.ShowEmptyFilter = Column.OptionsFilter.ShowEmptyDateFilter;
				dateFilterControl.ElementsLookAndFeel = LookAndFeel;
				dateFilterControl.Init(info != null ? info.LastFilterResult : null, filterPopup.ActiveFilterCriteria);
				dateFilterControl.CreateControls();
				dateFilterControl.Dock = DockStyle.Fill;
				PopupControl.ClientSize = dateFilterControl.Size;
				PopupControl.Controls.Add(dateFilterControl);
			}
			public DateFilterResult GetResult() {
				if(DateFilterControl.Result != null) 
					return DateFilterControl.Result;
				DateFilterControl.ApplyFilter();
				return DateFilterControl.Result;
			}
			public DateFilterResult GetImmediateResult() {
				return DateFilterControl.CalcFilterResult();
			}
			public override BaseEdit CreateEditor() {
				PopupContainerEdit edit = base.CreateEditor() as PopupContainerEdit;
				DateFilterControl.OwnerEdit = edit;
				return edit;
			}
		}
		#endregion
	}
}
