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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.ExpressApp.Controls;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraPrinting;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Localization;
using DevExpress.XtraTreeList.Nodes;
namespace DevExpress.ExpressApp.TreeListEditors.Win {
	public class TreeListColumnWrapper : ColumnWrapper {
		private const int defaultColumnWidth = 75;
		static ColumnSortOrder Convert(System.Windows.Forms.SortOrder systemSortOrder) {
			if(systemSortOrder == System.Windows.Forms.SortOrder.None)
				return ColumnSortOrder.None;
			if(systemSortOrder == System.Windows.Forms.SortOrder.Ascending)
				return ColumnSortOrder.Ascending;
			if(systemSortOrder == System.Windows.Forms.SortOrder.Descending)
				return ColumnSortOrder.Descending;
			throw new ArgumentException(systemSortOrder.ToString());
		}
		static SortOrder Convert(ColumnSortOrder columnSortOrder) {
			if(columnSortOrder == ColumnSortOrder.None)
				return System.Windows.Forms.SortOrder.None;
			if(columnSortOrder == ColumnSortOrder.Ascending)
				return System.Windows.Forms.SortOrder.Ascending;
			if(columnSortOrder == ColumnSortOrder.Descending)
				return System.Windows.Forms.SortOrder.Descending;
			throw new ArgumentException(columnSortOrder.ToString());
		}
		static DevExpress.XtraTreeList.SummaryItemType Convert(SummaryType value) {
			return (DevExpress.XtraTreeList.SummaryItemType)Enum.Parse(typeof(DevExpress.XtraTreeList.SummaryItemType), value.ToString());
		}
		static SummaryType Convert(DevExpress.XtraTreeList.SummaryItemType value) {
			return (SummaryType)Enum.Parse(typeof(SummaryType), value.ToString());
		}
		static string GetSummaryFooterFormat(DevExpress.XtraTreeList.SummaryItemType itemType) {
			string result = "";
			switch(itemType) {
				case DevExpress.XtraTreeList.SummaryItemType.Sum: result = TreeListLocalizer.Active.GetLocalizedString(TreeListStringId.MenuFooterSumFormat); break;
				case DevExpress.XtraTreeList.SummaryItemType.Min: result = TreeListLocalizer.Active.GetLocalizedString(TreeListStringId.MenuFooterMinFormat); break;
				case DevExpress.XtraTreeList.SummaryItemType.Max: result = TreeListLocalizer.Active.GetLocalizedString(TreeListStringId.MenuFooterMaxFormat); break;
				case DevExpress.XtraTreeList.SummaryItemType.Count: result = TreeListLocalizer.Active.GetLocalizedString(TreeListStringId.MenuFooterCountFormat); break;
				case DevExpress.XtraTreeList.SummaryItemType.Average: result = TreeListLocalizer.Active.GetLocalizedString(TreeListStringId.MenuFooterAverageFormat); break;
			}
			return result;
		}
		private TreeListColumn column;
		public TreeListColumnWrapper(TreeListColumn column) {
			this.column = column;
		}
		public TreeListColumn Column {
			get { return column; }
		}
		public override string Id {
			get { return ((TreeListColumnTag)column.Tag).Model.Id; }
		}
		public override string PropertyName {
			get { return column.Name; }
		}
		public override int SortIndex {
			get { return column.SortIndex; }
			set { column.SortIndex = value; }
		}
		public override ColumnSortOrder SortOrder {
			get { return Convert(column.SortOrder); }
			set { column.SortOrder = Convert(value); }
		}
		public override bool AllowSortingChange {
			get { return column.OptionsColumn.AllowSort; }
			set { column.OptionsColumn.AllowSort = value; }
		}
		public override bool AllowSummaryChange {
			get { return ((TreeListColumnTag)column.Tag).AllowSummaryChange; }
			set { ((TreeListColumnTag)column.Tag).AllowSummaryChange = value; }
		}
		public override bool Visible {
			get { return column.Visible; }
		}
		public override int VisibleIndex {
			get { return column.VisibleIndex; }
			set { column.VisibleIndex = value; }
		}
		public override IList<SummaryType> Summary {
			get {
				IList<SummaryType> summary = new List<SummaryType>();
				if(column.SummaryFooter != DevExpress.XtraTreeList.SummaryItemType.None) {
					summary.Add(Convert(column.SummaryFooter));
				}
				return summary;
			}
			set {
				if(value.Count > 0) {
					column.SummaryFooter = Convert(value[0]);
				}
				else {
					column.SummaryFooter = DevExpress.XtraTreeList.SummaryItemType.None;
				}
				column.SummaryFooterStrFormat = GetSummaryFooterFormat(column.SummaryFooter);
			}
		}
		public override string Caption {
			get { return column.Caption; }
			set { column.Caption = value; }
		}
		public override string ToolTip {
			get { return column.ToolTip; }
			set { column.ToolTip = value; }
		}
		public override int Width {
			get {
				if(column.Width == defaultColumnWidth) {
					return 0;
				}
				return column.Width;
			}
			set {
				if(value != 0) {
					column.Width = value;
				}
			}
		}
		public override string DisplayFormat {
			get { return column.Format.FormatString; }
			set {
				column.Format.FormatString = value;
				column.Format.FormatType = FormatType.Custom;
			}
		}
		public override bool ShowInCustomizationForm {
			get { return column.OptionsColumn.ShowInCustomizationForm; }
			set { column.OptionsColumn.ShowInCustomizationForm = value; }
		}
		public override void DisableFeaturesForProtectedContentColumn() {
			base.DisableFeaturesForProtectedContentColumn();
			column.AllowIncrementalSearch = false;
		}
		public override void ApplyModel(IModelColumn columnInfo) {
			base.ApplyModel(columnInfo);
			((TreeListColumnTag)column.Tag).ApplyModel(columnInfo);
			column.AllNodesSummary = ((IModelColumnTreeListWin)columnInfo).AllNodesSummary;
		}
		public override void SynchronizeModel() {
			base.SynchronizeModel();
			((TreeListColumnTag)column.Tag).SynchronizeModel();
			IModelColumn columnInfo = ((TreeListColumnTag)column.Tag).Model;
			((IModelColumnTreeListWin)columnInfo).AllNodesSummary = column.AllNodesSummary;
		}
	}
	public class TreeListColumnTag {
		private IModelColumn columnInfo;
		private TreeListColumn column;
		private TreeListEditor editor;
		private bool allowSummaryChange;
		private ModelSynchronizer CreateModelSynchronizer(IModelColumn columnInfo) {
			return new ColumnWrapperModelSynchronizer(new TreeListColumnWrapper(column), columnInfo, editor);
		}
		public TreeListColumnTag(TreeListColumn column, TreeListEditor editor) {
			this.column = column;
			this.editor = editor;
		}
		public IModelColumn Model {
			get { return columnInfo; }
		}
		public void ApplyModel(IModelColumn columnInfo) {
			this.columnInfo = columnInfo;
			CreateModelSynchronizer(columnInfo).ApplyModel();
		}
		public void SynchronizeModel() {
			CreateModelSynchronizer(columnInfo).SynchronizeModel();
		}
		public bool AllowSummaryChange {
			get { return allowSummaryChange; }
			set { allowSummaryChange = value; }
		}
	}
	public class TreeListEditor : ColumnsListEditor, IDXPopupMenuHolder, IComplexListEditor, IControlOrderProvider, ILookupListEditor, IFocusedElementCaptionProvider, INodeObjectAdapterProvider, ISupportFooter, IExportable, ISupportUpdate, IContextMenuTarget {
		private ObjectTreeList treeList;
		private RepositoryEditorsFactory repositoryFactory;
		private TreeNodeInterfaceAdapter adapter;
		private ActionsDXPopupMenu popupMenu;
		private IObjectSpace objectSpace;
		private CollectionSourceBase collectionSource;
		private ITreeNode rootValue;
		private bool processSelectedItemBySingleClick;
		private Boolean trackMousePosition;
		private bool deferredNodeLoading = true;
		private bool holdRootValue = false;
		private bool clearFocusedObjectOnMouseClick = true;
		private void treeList_HandleCreated(object sender, EventArgs e) {
			UpdateControlDataSource();
			treeList.ForceInitialize(); 
			treeList.MoveFirst(); 
			treeList.Selection.Set(treeList.FocusedNode); 
		}
		private void treeList_VisibleChanged(object sender, EventArgs e) {
			UpdateControlDataSource();
		}
		private void treeList_SelectionChanged(object sender, EventArgs e) {
			OnSelectionChanged();
		}
		private void treeList_BeforeFocusNode(object sender, BeforeFocusNodeEventArgs e) {
			e.CanFocus = OnFocusedObjectChanging();
		}
		private void treeList_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e) {
			OnFocusedObjectChanged();
			if(!treeList.OptionsSelection.MultiSelect) {
				OnSelectionChanged();
			}
		}
		private void treeList_DoubleClick(object sender, EventArgs e) {
			MouseEventArgs mouseArgs = e as MouseEventArgs;
			if(mouseArgs != null && treeList.CalcHitInfo(mouseArgs.Location).HitInfoType == HitInfoType.Cell) {
				OnProcessSelectedItem();
			}
		}
		private void treeList_KeyDown(object sender, KeyEventArgs e) {
			if((treeList.State == TreeListState.Regular) && (e.KeyCode == Keys.Enter) && (FocusedObject != null)) {
				OnProcessSelectedItem();
			}
		}
		private void treeList_MouseClick(object sender, MouseEventArgs e) {
			TreeListHitTest hit = treeList.ViewInfo.GetHitTest(e.Location);
			if(clearFocusedObjectOnMouseClick && (hit.HitInfoType == HitInfoType.Empty || hit.HitInfoType == HitInfoType.None)) {
				treeList.Selection.Clear();
				treeList.BeginUpdate();
				int topVisibleNodeIndex = treeList.TopVisibleNodeIndex;
				FocusedObject = null;
				treeList.TopVisibleNodeIndex = topVisibleNodeIndex;
				treeList.EndUpdate();
			}
			if(processSelectedItemBySingleClick && hit.HitInfoType == HitInfoType.Cell) {
				DXMouseEventArgs.GetMouseArgs(treeList, e).Handled = true;
				OnProcessSelectedItem();
			}
		}
		private void treeList_MouseMove(object sender, MouseEventArgs e) {
			if(trackMousePosition) {
				TreeListHitTest hit = treeList.ViewInfo.GetHitTest(e.Location);
				if(hit.HitInfoType == HitInfoType.Cell && hit.Node != null && treeList.FocusedNode != hit.Node) {
					if(hit.Node is ObjectTreeListNode) {
						treeList.FocusedNode = (ObjectTreeListNode)hit.Node;
					}
				}
			}
		}
		private void treeList_HideCustomizationForm(object sender, EventArgs e) {
			if(EndCustomization != null) {
				EndCustomization(this, EventArgs.Empty);
			}
		}
		private void treeList_ShowCustomizationForm(object sender, EventArgs e) {
			if(BeginCustomization != null) {
				BeginCustomization(this, EventArgs.Empty);
			}
		}
		private void objectSpace_ObjectSaved(object sender, ObjectManipulatingEventArgs e) {
			if(treeList != null && this.FocusedObject == e.Object) {
				treeList.RefreshObject(e.Object);
			}
		}
		private void objectSpace_ObjectChanged(object sender, ObjectChangedEventArgs e) {
			if(treeList != null) {
				treeList.RefreshObject(e.Object);
			}
		}
		private void ResetDataSource() {
			if(treeList != null && treeList.IsHandleCreated) {
				treeList.DataSource = null;
			}
			UpdateControlDataSource();
		}
		private void UpdateDeferredNodeLoading() {
			if(treeList != null) {
				treeList.BuildChildNodesRecursive = !DeferredNodeLoading;
			}
		}
		private void RemoveColumnFromModel(IModelColumn columnInfo) {
			if(columnInfo != null) {
				columnInfo.Remove();
			}
		}
		private void treeList_MouseDown(object sender, MouseEventArgs e) {
			if(e.Button == MouseButtons.Right && treeList.State == TreeListState.Regular) {
				TreeListHitInfo info = treeList.CalcHitInfo(e.Location);
				if(info.HitInfoType == HitInfoType.Cell || info.HitInfoType == HitInfoType.RowIndicator) {
					if(info.Node is ObjectTreeListNode) {
						treeList.FocusedNode = (ObjectTreeListNode)info.Node;
					}
				}
			}
		}
		private void adapter_Changed(object sender, EventArgs e) {
			rootValue = (ITreeNode)adapter.RootValue;
			holdRootValue = adapter.HoldRootValue;
			ResetDataSource();
		}
		protected virtual DevExpress.XtraTreeList.Data.UnboundColumnType GetUnboundColumnType(Type memberType) {
			DevExpress.XtraTreeList.Data.UnboundColumnType result = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
			if(memberType == typeof(int)) {
				result = DevExpress.XtraTreeList.Data.UnboundColumnType.Integer;
			}
			if(memberType == typeof(Decimal)) {
				result = DevExpress.XtraTreeList.Data.UnboundColumnType.Decimal;
			}
			if(memberType == typeof(bool)) {
				result = DevExpress.XtraTreeList.Data.UnboundColumnType.Boolean;
			}
			if(memberType == typeof(DateTime)) {
				result = DevExpress.XtraTreeList.Data.UnboundColumnType.DateTime;
			}
			if(memberType == typeof(string)) {
				result = DevExpress.XtraTreeList.Data.UnboundColumnType.String;
			}
			return result;
		}
		protected virtual TreeNodeInterfaceAdapter CreateAdapter() {
			return new TreeNodeInterfaceAdapter();
		}
		protected virtual ObjectTreeList CreateTreeListControl() {
			return new ObjectTreeList(Adapter);
		}
		protected override object CreateControlsCore() {
			if(treeList == null) {
				adapter = CreateAdapter();
				adapter.ObjectSpace = objectSpace;
				adapter.RootValue = rootValue;
				adapter.HoldRootValue = holdRootValue;
				adapter.Changed += new EventHandler(adapter_Changed);
				treeList = CreateTreeListControl();
				treeList.Tag = EasyTestTagHelper.FormatTestTable(Name);
				treeList.Dock = DockStyle.Fill;
				treeList.OptionsView.ShowIndicator = false;
				treeList.OptionsView.ShowHorzLines = false;
				treeList.OptionsView.ShowVertLines = false;
				treeList.OptionsView.ShowButtons = true;
				treeList.OptionsView.FocusRectStyle = DrawFocusRectStyle.CellFocus;
				treeList.OptionsView.ShowColumns = true;
				treeList.OptionsView.ShowRoot = true;
				treeList.OptionsBehavior.AutoPopulateColumns = false;
				treeList.OptionsBehavior.Editable = false;
				treeList.ShowButtonMode = ShowButtonModeEnum.ShowOnlyInEditor;
				treeList.OptionsBehavior.KeepSelectedOnClick = true;
				treeList.OptionsNavigation.AutoMoveRowFocus = true;
				treeList.OptionsSelection.EnableAppearanceFocusedCell = false;
				treeList.OptionsSelection.MultiSelect = true;
				treeList.HandleCreated += new EventHandler(treeList_HandleCreated);
				treeList.FocusedNodeChanged += new FocusedNodeChangedEventHandler(treeList_FocusedNodeChanged);
				treeList.BeforeFocusNode += new BeforeFocusNodeEventHandler(treeList_BeforeFocusNode);
				treeList.SelectionChanged += new EventHandler(treeList_SelectionChanged);
				treeList.DoubleClick += new EventHandler(treeList_DoubleClick);
				treeList.KeyDown += new KeyEventHandler(treeList_KeyDown);
				treeList.MouseDown += new MouseEventHandler(treeList_MouseDown);
				treeList.MouseClick += new MouseEventHandler(treeList_MouseClick);
				treeList.MouseMove += new MouseEventHandler(treeList_MouseMove);
				treeList.ShowCustomizationForm += new EventHandler(treeList_ShowCustomizationForm);
				treeList.HideCustomizationForm += new EventHandler(treeList_HideCustomizationForm);
				treeList.VisibleChanged += new EventHandler(treeList_VisibleChanged);
				ApplyModel();
				OnPrintableChanged();
			}
			return treeList;
		}
		public override void BreakLinksToControls() {
			base.BreakLinksToControls();
			if(popupMenu != null) {
				popupMenu.Dispose();
				popupMenu = null;
			}
			if(adapter != null) {
				adapter.Changed -= new EventHandler(adapter_Changed);
				adapter.ObjectSpace = null;
				adapter.RootValue = null;
				adapter.Collection = null;
				adapter = null;
			}
			if(treeList != null) {
				treeList.MouseClick -= new MouseEventHandler(treeList_MouseClick);
				treeList.DoubleClick -= new EventHandler(treeList_DoubleClick);
				treeList.HandleCreated -= new EventHandler(treeList_HandleCreated);
				treeList.BeforeFocusNode -= new BeforeFocusNodeEventHandler(treeList_BeforeFocusNode);
				treeList.FocusedNodeChanged -= new FocusedNodeChangedEventHandler(treeList_FocusedNodeChanged);
				treeList.SelectionChanged -= new EventHandler(treeList_SelectionChanged);
				treeList.MouseMove -= new MouseEventHandler(treeList_MouseMove);
				treeList.MouseDown -= new MouseEventHandler(treeList_MouseDown);
				treeList.ShowCustomizationForm -= new EventHandler(treeList_ShowCustomizationForm);
				treeList.HideCustomizationForm -= new EventHandler(treeList_HideCustomizationForm);
				treeList.KeyDown -= new KeyEventHandler(treeList_KeyDown);
				treeList.VisibleChanged -= new EventHandler(treeList_VisibleChanged);
				treeList.Dispose();
				treeList = null;
				OnPrintableChanged();
			}
		}
		public override void ApplyModel() {
			if(Model != null) {
				CancelEventArgs args = new CancelEventArgs();
				OnModelApplying(args);
				if(!args.Cancel) {
					if(TreeList != null) {
						TreeList.OptionsView.ShowSummaryFooter = Model.IsFooterVisible;
						new ColumnsListEditorModelSynchronizer(this, Model).ApplyModel();
					}
					base.ApplyModel();
					OnModelApplied();
				}
			}
		}
		public override void SaveModel() {
			if(Model != null) {
				CancelEventArgs args = new CancelEventArgs();
				OnModelSaving(args);
				if(!args.Cancel) {
					if(TreeList != null) {
						Model.IsFooterVisible = TreeList.OptionsView.ShowSummaryFooter;
						new ColumnsListEditorModelSynchronizer(this, Model).SynchronizeModel();
					}
					base.SaveModel();
					OnModelSaved();
				}
			}
		}
		protected override void OnControlsCreated() {
			UpdateDeferredNodeLoading();
			base.OnControlsCreated();
			UpdateControlDataSource();
		}
		protected override void OnDataSourceChanged() {
			base.OnDataSourceChanged();
			UpdateControlDataSource();
		}
		protected override void OnProtectedContentTextChanged() {
			base.OnProtectedContentTextChanged();
			repositoryFactory.ProtectedContentText = ProtectedContentText;
		}
		protected void OnPrintableChanged() {
			if(PrintableChanged != null) {
				PrintableChanged(this, new PrintableChangedEventArgs(Printable));
			}
		}
		protected void UpdateControlDataSource() {
			if(treeList != null) {
				if(treeList.IsHandleCreated && treeList.Visible) {
					adapter.Collection = List;
					treeList.DataSource = List;
				}
				else {
					adapter.Collection = null;
					treeList.DataSource = null;
				}
			}
		}
		protected override void AssignDataSourceToControl(Object dataSource) {
		}
		public override void Refresh() {
			if(treeList != null) {
				treeList.RefreshDataSource();
			}
		}
		public TreeListEditor(IModelListView model)
			: base(model) {
			popupMenu = new ActionsDXPopupMenu();
		}
		public override void Dispose() {
			try {
				BreakLinksToControls();
				if(objectSpace != null) {
					objectSpace.ObjectSaved -= new EventHandler<ObjectManipulatingEventArgs>(objectSpace_ObjectSaved);
					objectSpace.ObjectChanged -= new EventHandler<ObjectChangedEventArgs>(objectSpace_ObjectChanged);
				}
				objectSpace = null;
				rootValue = null;
			}
			finally {
				base.Dispose();
			}
		}
		public override IContextMenuTemplate ContextMenuTemplate {
			get { return popupMenu; }
		}
		public override IList GetSelectedObjects() {
			return (treeList != null) ? treeList.GetSelectedObjects() : new Object[] { };
		}
		private bool IsMediaData(IModelColumn columnInfo) {
			if(columnInfo.ModelMember != null) {
				MediaDataObjectAttribute mediaDataAttribute = columnInfo.ModelMember.MemberInfo.MemberTypeInfo.FindAttribute<MediaDataObjectAttribute>(false);
				return mediaDataAttribute != null;
			}
			return false;
		}
		protected override ColumnWrapper AddColumnCore(IModelColumn columnInfo) {
			string fieldName = IsMediaData(columnInfo) ? columnInfo.FieldName : columnInfo.PropertyName;
			TreeListColumn column = treeList.Columns.AddField(fieldName);
			column.Name = columnInfo.PropertyName;
			column.MinWidth = 5;
			column.Tag = new TreeListColumnTag(column, this);
			TreeListColumnWrapper columnWrapper = new TreeListColumnWrapper(column);
			columnWrapper.ApplyModel(columnInfo);
			if(repositoryFactory != null) {
				IMemberInfo memberDescriptor = ObjectTypeInfo.FindMember(columnInfo.PropertyName);
				if(SimpleTypes.IsClass(memberDescriptor.MemberType) || memberDescriptor.MemberType.IsInterface) {
					column.SortMode = DevExpress.XtraGrid.ColumnSortMode.DisplayText;
				}
				else {
					column.SortMode = DevExpress.XtraGrid.ColumnSortMode.Value;
				}
				column.UnboundType = GetUnboundColumnType(memberDescriptor.MemberType);
				bool isGranted = DataManipulationRight.CanRead(collectionSource.ObjectTypeInfo.Type, columnInfo.PropertyName, null, collectionSource, collectionSource.ObjectSpace);
				RepositoryItem repositoryItem = repositoryFactory.CreateRepositoryItem(!isGranted, columnInfo, ObjectType);
				if(repositoryItem != null) {
					repositoryItem.ReadOnly = true;
					treeList.RepositoryItems.Add(repositoryItem);
					column.ColumnEdit = repositoryItem;
					if(!column.Format.IsEquals(repositoryItem.DisplayFormat)) {
						column.Format.FormatType = repositoryItem.DisplayFormat.FormatType;
						column.Format.Format = repositoryItem.DisplayFormat.Format;
						column.Format.FormatString = repositoryItem.DisplayFormat.FormatString;
					}
				}
			}
			return columnWrapper;
		}
		public override void RemoveColumn(ColumnWrapper column) {
			TreeListColumn treeListColumn = ((TreeListColumnWrapper)column).Column;
			bool found = false;
			if(treeList != null) {
				foreach(TreeListColumn col in treeList.Columns) {
					if((col.FieldName == column.PropertyName) || (col.FieldName == column.PropertyName + "!")) {
						TreeListColumnTag columnProperties = (TreeListColumnTag)col.Tag;
						RemoveColumnFromModel(columnProperties.Model);
						treeList.Columns.Remove(treeListColumn);
						found = true;
						break;
					}
				}
			}
			if(!found) {
				throw new ArgumentException(string.Format(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.GridColumnDoesNotExist), column.PropertyName), "PropertyName");
			}
		}
		public override Boolean SupportsDataAccessMode(CollectionSourceDataAccessMode dataAccessMode) {
			return (dataAccessMode == CollectionSourceDataAccessMode.Client);
		}
		public override object FocusedObject {
			get { return treeList != null ? treeList.FocusedObject : null; }
			set {
				if(treeList != null) {
					treeList.FocusedObject = value;
				}
			}
		}
		public override SelectionType SelectionType {
			get { return SelectionType.Full; }
		}
		public override string Name {
			get { return base.Name; }
			set {
				base.Name = value;
				if(treeList != null) {
					treeList.Tag = EasyTestTagHelper.FormatTestTable(value);
				}
			}
		}
		public TreeList TreeList {
			get { return treeList; }
		}
		public ITreeNode RootValue {
			get { return (adapter != null) ? adapter.RootValue as ITreeNode : rootValue; }
			set {
				if(value != rootValue) {
					rootValue = value;
					if(adapter != null) {
						adapter.RootValue = rootValue;
						ResetDataSource();
					}
				}
			}
		}
		public bool HoldRootValue {
			get { return (adapter != null) ? adapter.HoldRootValue : holdRootValue; }
			set {
				if(value != holdRootValue) {
					holdRootValue = value;
					if(adapter != null) {
						adapter.HoldRootValue = holdRootValue;
						ResetDataSource();
					}
				}
			}
		}
		public bool DeferredNodeLoading {
			get { return deferredNodeLoading; }
			set {
				deferredNodeLoading = value;
				UpdateDeferredNodeLoading();
			}
		}
		public bool ClearFocusedObjectOnMouseClick {
			get { return clearFocusedObjectOnMouseClick; }
			set { clearFocusedObjectOnMouseClick = value; }
		}
		#region IDXPopupMenuHolder Members
		public Control PopupSite {
			get { return treeList; }
		}
		public virtual bool CanShowPopupMenu(Point position) {
			HitInfoType hitType = treeList.CalcHitInfo(treeList.PointToClient(position)).HitInfoType;
			return hitType == HitInfoType.Cell || hitType == HitInfoType.Empty || hitType == HitInfoType.None;
		}
		public void SetMenuManager(IDXMenuManager manager) {
			if(treeList != null) {
				treeList.MenuManager = manager;
			}
		}
		#endregion
		#region IComplexListEditor Members
		public void Setup(CollectionSourceBase collectionSource, XafApplication application) {
			this.collectionSource = collectionSource;
			repositoryFactory = new RepositoryEditorsFactory(application, collectionSource.ObjectSpace);
			objectSpace = collectionSource.ObjectSpace;
			objectSpace.ObjectSaved += new EventHandler<ObjectManipulatingEventArgs>(objectSpace_ObjectSaved);
			objectSpace.ObjectChanged += new EventHandler<ObjectChangedEventArgs>(objectSpace_ObjectChanged);
		}
		#endregion
		#region IControlOrderProvider Members
		public int GetIndexByObject(Object obj) {
			IList objects = GetOrderedObjects();
			return objects.IndexOf(obj);
		}
		public Object GetObjectByIndex(int index) {
			IList objects = GetOrderedObjects();
			if((index >= 0) && (index < objects.Count)) {
				return objects[index];
			}
			return null;
		}
		public IList GetOrderedObjects() {
			List<Object> list = new List<Object>();
			if(treeList != null && treeList.Nodes.Count > 0) {
				TreeListNode current = treeList.Nodes[0];
				while(current != null) {
					object obj = ((ObjectTreeListNode)current).Object;
					if(obj != null) {
						list.Add(obj);
					}
					if(current.Nodes.Count > 0) {
						current = current.Nodes[0];
					}
					else {
						while(current.NextNode == null && current.ParentNode != null) {
							current = current.ParentNode;
						}
						current = current.NextNode;
					}
				}
			}
			return list;
		}
		#endregion
		#region ILookupListEditor Members
		public bool ProcessSelectedItemBySingleClick {
			get { return processSelectedItemBySingleClick; }
			set { processSelectedItemBySingleClick = value; }
		}
		public Boolean TrackMousePosition {
			get { return trackMousePosition; }
			set { trackMousePosition = value; }
		}
		public event EventHandler BeginCustomization;
		public event EventHandler EndCustomization;
		#endregion
		#region IGetCurrentValue Members
		object IFocusedElementCaptionProvider.FocusedElementCaption {
			get {
				string result = "";
				if(TreeList.FocusedNode != null) {
					result = TreeList.FocusedNode.GetDisplayText(TreeList.FocusedColumn.AbsoluteIndex);
				}
				return result;
			}
		}
		#endregion
		#region INodeObjectAdapterProvider Members
		public NodeObjectAdapter Adapter {
			get { return adapter; }
		}
		#endregion
		#region ISupportFooter Members
		public bool IsFooterVisible {
			get { return TreeList.OptionsView.ShowSummaryFooter; }
			set { TreeList.OptionsView.ShowSummaryFooter = value; }
		}
		#endregion
		#region IContextMenuTarget Members
		void IContextMenuTarget.SetMenuManager(IDXMenuManager menuManager) {
			if(TreeList == null) {
				throw new InvalidOperationException("Cannot set the 'MenuManager' property because the 'TreeListEditor.TreeList' property is null");
			}
			TreeList.MenuManager = menuManager;
		}
		bool IContextMenuTarget.CanShowContextMenu(Point position) {
			return CanShowPopupMenu(position);
		}
		Control IContextMenuTarget.ContextMenuSite {
			get { return TreeList; }
		}
		bool IContextMenuTarget.ContextMenuEnabled {
			get { return true; }
		}
		event EventHandler IContextMenuTarget.ContextMenuEnabledChanged {
			add { }
			remove { }
		}
		#endregion
		public void BeginUpdate() {
			LockSelectionEvents();
		}
		public void EndUpdate() {
			UnlockSelectionEvents();
		}
		public override IList<ColumnWrapper> Columns {
			get {
				List<ColumnWrapper> result = new List<ColumnWrapper>();
				if(TreeList != null) {
					foreach(TreeListColumn column in TreeList.Columns) {
						if(column.Tag is TreeListColumnTag) { 
							result.Add(new TreeListColumnWrapper(column));
						}
					}
				}
				return result;
			}
		}
		protected override bool HasProtectedContent(string propertyName) {
			return !(ObjectTypeInfo.FindMember(propertyName) == null || DataManipulationRight.CanRead(ObjectType, propertyName, null, collectionSource, objectSpace));
		}
		public List<ExportTarget> SupportedExportFormats {
			get {
				if(Printable != null) {
					return new List<ExportTarget>() {
						ExportTarget.Csv,
						ExportTarget.Html,
						ExportTarget.Image,
						ExportTarget.Mht,
						ExportTarget.Pdf,
						ExportTarget.Rtf,
						ExportTarget.Text,
						ExportTarget.Xls,
						ExportTarget.Xlsx
					};
				}
				else {
					return new List<ExportTarget>();
				}
			}
		}
		public IPrintable Printable {
			get { return TreeList; }
		}
		public void OnExporting() { }
		public event EventHandler<PrintableChangedEventArgs> PrintableChanged;
	}
	#region Obsolete 14.2
	[Obsolete(ObsoleteMessages.TypeIsNotUsedAnymore), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class TreeListEditorModelSynchronizerList {
	}
	#endregion
}
