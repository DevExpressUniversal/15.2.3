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
using DevExpress.XtraReports.UI;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraReports.Native;
using System.ComponentModel.Design;
using DevExpress.XtraReports.Design.Tools;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.Native.Data;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraReports.Design.GroupSort {
	public class GroupSortController : TreeListController {
		XtraReportBase report;
		Control fakeControl;
		int activeID = -1;
		string activeFieldName = string.Empty;
		RealModifierStrategy realModifierStrategy;
		public GroupSortController(IServiceProvider serviceProvider)
			: base(serviceProvider) {
			this.fakeControl = new Control();
			IntPtr ignore = this.fakeControl.Handle;
			realModifierStrategy = new RealModifierStrategy(serviceProvider);
		}
		int ActiveID {
			get { return activeID; }
			set {
				activeID = value;
				activeFieldName = activeID >= 0 && treeList.FocusedColumn != null ?
					treeList.FocusedColumn.FieldName :
					string.Empty;
			}
		}
		GroupSortReflectCollection Data {
			get { return (GroupSortReflectCollection)treeList.DataSource; }
		}
		public XtraReportBase Report {
			get { return report; }
		}
		TreeListNode SelectedNode {
			get { return treeList.Selection.Count > 0 ? treeList.Selection[0] : null; }
		}
		public override void Dispose() {
			base.Dispose();
			if(this.fakeControl != null) {
				this.fakeControl.Dispose();
				this.fakeControl = null;
			}
		}
		public string GetItemDisplayName(string itemName) {
			return !ReportIsDisposed ? report.GetShortFieldDisplayName(itemName) : string.Empty;
		}
		public string GetFieldName(int id) {
			GroupSortReflectItem item = Data.GetItemByID(id);
			return item != null ? item.GroupField.FieldName : string.Empty;
		}
		protected override void OnSelectionChanged(object sender, EventArgs e) {
			if(DesignMethods.IsDesignerInTransaction(designerHost))
				return;
			if(report != GetXtraReport(selectionService.PrimarySelection))
				UpdateView();
		}
		public override void UpdateTreeList() {
			UpdateView();
		}
		void UpdateView() {
			if(!IsControlAlive(treeList) || designerHost.Loading)
				return;
			Dictionary<string, int> columnInfos = GetColumnInfos();
			treeList.BeginUpdate();
			try {
				report = GetXtraReport(selectionService.PrimarySelection);
				if(ReportIsDisposed)
					return;
				treeList.DataSource = CreateData(report);
				InitializeColumns(columnInfos);
				SetFocucedNode();
			} finally {
				treeList.EndUpdate();
			}
		}
		bool ReportIsDisposed {
			get { return report == null || report.IsDisposed; }
		}
		internal bool DesignerHostIsLoading {
			get { return designerHost.Loading; }
		}
		void SetFocucedNode() {
			if(this.ActiveID < 0)
				return;
			TreeListNode node = GetNodeById(treeList.Nodes, Math.Max(0, Math.Min(Data.Count - 1, this.ActiveID)));
			if(node != null)
				treeList.FocusedNode = node;
			if(!string.IsNullOrEmpty(activeFieldName)) {
				TreeListColumn column = treeList.Columns[activeFieldName];
				if(column != null)
					treeList.FocusedColumn = column;
			}
			this.ActiveID = -1;
		}
		TreeListNode GetNodeById(TreeListNodes nodes, int id) {
			foreach(TreeListNode node in nodes) {
				GroupSortReflectItem item = treeList.GetDataRecordByNode(node) as GroupSortReflectItem;
				if(item != null && item.ID == id)
					return node;
				return GetNodeById(node.Nodes, id);
			}
			return null;
		}
		Dictionary<string, int> GetColumnInfos() {
			Dictionary<string, int> columnInfos = new Dictionary<string, int>();
			columnInfos.Add(ColumnNames.FieldName, 227);
			columnInfos.Add(ColumnNames.SortOrder, 92);
			columnInfos.Add(ColumnNames.ShowHeader, 92);
			columnInfos.Add(ColumnNames.ShowFooter, 92);
			foreach(TreeListColumn column in treeList.Columns) {
				if(columnInfos.ContainsKey(column.FieldName))
					columnInfos[column.FieldName] = column.Width;
			}
			return columnInfos;
		}
		void InitializeColumns(Dictionary<string, int> columnInfos) {
			foreach(string name in columnInfos.Keys) {
				TreeListColumn column = treeList.Columns[name];
				if(column == null)
					continue;
				column.MinWidth = columnInfos[name];
				column.OptionsColumn.AllowSort = false;
				column.OptionsColumn.AllowMove = false;
				SetColumnEditor(column);
				column.BestFit();
			}
		}
		void SetColumnEditor(TreeListColumn column) {
			if(column != null && column.ColumnEdit == null)
				column.ColumnEdit = treeList.RepositoryItems[column.FieldName];
		}
		XtraReportBase GetXtraReport(object component) {
			if(component is XtraReportBase)
				return (XtraReportBase)component;
			if(component is XRControl)
				return ((XRControl)component).Report != null ? ((XRControl)component).Report : this.Report;
			return this.Report;
		}
		GroupSortReflectCollection CreateData(XtraReportBase report) {
			if(report.Bands == null)
				return new GroupSortReflectCollection();
			DetailBand detailBand = report.Bands[BandKind.Detail] as DetailBand;
			GroupField[] groupFields = XtraReportBase.CollectGroupFields(report.Groups, detailBand);
			return CreateData(groupFields);
		}
		GroupSortReflectCollection CreateData(GroupField[] groupFields) {
			GroupSortReflectCollection result = new GroupSortReflectCollection();
			for(int i = 0; i < groupFields.Length; i++) {
				result.Add(new GroupSortReflectItem(this, i, groupFields[i]));
			}
			for(int i = 0; i < result.Count; i++) {
				GroupSortReflectItem item = result[i];
				if(!(item.Band is GroupHeaderBand))
					continue;
				GroupSortReflectItem nextItem = i + 1 < result.Count ? result[i + 1] : null;
				if(nextItem == null || item.Band != nextItem.Band)
					Initialize(item);
			}
			return result;
		}
		void Initialize(GroupSortReflectItem item) {
			XRGroup group = GetXRGroup(item.Band);
			if(group != null)
				item.Initialize(group.Header != null, group.Footer != null);
		}
		protected override bool UpdateNeeded(object component) {
			return component is XtraReportBase || component is GroupBand || component is DetailBand || component is GroupField;
		}		
		public bool CanAddGroupSort() {
			if(ReportIsDisposed)
				return false;
			using(XRDataContext dataContext = new XRDataContext(null, true)) {
				DevExpress.Data.Browsing.DataBrowser dataBrowser = dataContext.GetDataBrowser(report.GetEffectiveDataSource(), report.DataMember, true);
				return dataBrowser != null ? dataBrowser.GetItemProperties().Count > 0 : false;
			}
		}
		#region add group
		public void AddGroup(string fieldName) {
			fakeControl.BeginInvoke(new Action<string>(AddGroupProc), fieldName);
		}
		void AddGroupProc(string fieldName) {
			DetailBand detail = GetDetail();
			if(detail == null)
				return;
			this.ActiveID = Data.Count;
			AddGroupProcCore(realModifierStrategy, fieldName, detail);
		}
		void AddGroupProcCore(ModifierStrategyBase strategy, string fieldName, DetailBand detail) {
			if(ReportIsDisposed)
				return;
			new ReportGroupHeaderModifier(strategy, fieldName).AddGroupHeader(report, detail.SortFields, 0);
		}
		DetailBand GetDetail() {
			return !ReportIsDisposed ? (DetailBand)report.Bands[BandKind.Detail] : null;
		}
		#endregion
		#region add sort
		public void AddSort(string fieldName) {
			fakeControl.BeginInvoke(new Action<string>(AddSortProc), fieldName);
		}
		void AddSortProc(string fieldName) {
			DetailBand detail = GetDetail();
			if(detail == null)
				return;
			this.ActiveID = Data.Count;
			AddSortProcCore(realModifierStrategy, fieldName, detail);
		}
		void AddSortProcCore(ModifierStrategyBase strategy, string fieldName, DetailBand detail) {
			if(ReportIsDisposed)
				return;
			new BandAddSortModifier(strategy, fieldName).Modify(detail);
		}
		#endregion
		#region move sort up
		public void MoveSortUp() {
			GroupSortReflectItem item = GetSelectedReflectItem();
			if(item == null)
				return;
			this.ActiveID = item.ID - 1;
			fakeControl.BeginInvoke(new Action<GroupSortReflectItem>(MoveSortUpProc), item);
		}
		void MoveSortUpProc(GroupSortReflectItem item) {
			MoveSortUpProcCore(realModifierStrategy, item);
		}
		void MoveSortUpProcCore(ModifierStrategyBase strategy, GroupSortReflectItem item) {
			MoveSortDownProcCore(strategy, this.Data.GetPreviousItem(item));
		}
		GroupSortReflectItem GetSelectedReflectItem() {
			return SelectedNode != null ? Data.GetItemByID(SelectedNode.Id) : null;
		}
		#endregion
		#region move sort down
		public void MoveSortDown() {
			GroupSortReflectItem item = GetSelectedReflectItem();
			if(item == null)
				return;
			this.ActiveID = item.ID + 1;
			fakeControl.BeginInvoke(new Action<GroupSortReflectItem>(MoveSortDownProc), item);
		}
		void MoveSortDownProc(GroupSortReflectItem item) {			
			MoveSortDownProcCore(realModifierStrategy, item);
		}
		void MoveSortDownProcCore(ModifierStrategyBase strategy, GroupSortReflectItem item) {
			if(item == null || ReportIsDisposed)
				return;
			GroupSortReflectItem nextItem = this.Data.GetNextItem(item);
			if(nextItem == null)
				return;
			if(item.ShowHeader && !nextItem.ShowHeader) {
				new BandMoveSortDownModifier(strategy, nextItem.GroupField).Modify(item.Band, nextItem.Band);
			} else if(item.ShowHeader && nextItem.ShowHeader) {
				new BandMoveGroupDownModifier(strategy).Modify(item.Band, nextItem.Band);
			} else if(!item.ShowHeader && !nextItem.ShowHeader) {
				new BandMoveSortDownModifier2(strategy, item.GroupField).Modify(item.Band);
			} else if(!item.ShowHeader && nextItem.ShowHeader) {
				new BandMoveSortDownModifier3(strategy, item.GroupField).Modify(item.Band, GetNextBand(nextItem));
			}
		}
		Band GetNextBand(GroupSortReflectItem reflectItem) {
			Band nextBand = Data.GetNextBand(reflectItem);
			return nextBand != null ? nextBand : GetDetail();
		}
		#endregion
		#region delete sort
		public void DeleteSort() {
			GroupSortReflectItem item = GetSelectedReflectItem();
			if(item == null)
				return;
			this.ActiveID = item.ID;
			fakeControl.BeginInvoke(new Action<GroupSortReflectItem>(DeleteSortProc), item);
		}
		void DeleteSortProc(GroupSortReflectItem reflectItem) {
			DeleteSortProcCore(realModifierStrategy, reflectItem);
		}
		void DeleteSortProcCore(ModifierStrategyBase strategy, GroupSortReflectItem reflectItem) {
			if(ReportIsDisposed)
				return;
			if(reflectItem.ShowHeader) {
				XRGroup group = GetXRGroup(reflectItem.Band);
				if(group != null)
					new ReportDeleteHeaderModifier(strategy).DeleteHeader(group, GetNextSortFields(reflectItem), group.GroupFields.Count - 1);
			} else
				new BandRemoveSortModifier(strategy, reflectItem.GroupField).Modify(reflectItem.Band);
		}
		XRGroup GetXRGroup(Band band) {
			return band is GroupBand ? report.Groups.FindGroupByBand((GroupBand)band) : null;
		}
		GroupFieldCollection GetNextSortFields(GroupSortReflectItem reflectItem) {
			Band nextBand = GetNextBand(reflectItem);
			return nextBand != null ? nextBand.SortFieldsInternal : null;
		}
		public bool CanDeleteSort() {
			GroupSortReflectItem item = GetSelectedReflectItem();
			if(item != null) {
				XRGroup group = GetXRGroup(item.Band);
				if(group != null && (BandContainsControls(group.Header) || BandContainsControls(group.Footer)))
					return DialogResult.Yes == ShowWarning();
			}
			return true;
		}
		static bool BandContainsControls(Band band) {
			return band != null && band.Controls != null && band.Controls.Count > 0;
		}
		DialogResult ShowWarning() {
			return XtraMessageBox.Show(treeList.LookAndFeel, treeList, ReportLocalizer.GetString(ReportStringId.Msg_GroupSortWarning), ReportLocalizer.GetString(ReportStringId.UD_Title_GroupAndSort), MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
		}
		#endregion
		#region show footer
		public void ShowFooter(GroupSortReflectItem reflectItem) {
			this.ActiveID = reflectItem.ID;
			fakeControl.BeginInvoke(new Action<GroupSortReflectItem>(ShowFooterProc), reflectItem);
		}
		void ShowFooterProc(GroupSortReflectItem reflectItem) {
			ShowFooterProcCore(realModifierStrategy, reflectItem);
		}
		void ShowFooterProcCore(ModifierStrategyBase strategy, GroupSortReflectItem reflectItem) {
			if(ReportIsDisposed)
				return;
			Band band = reflectItem.GroupField.Band;
			new ReportAddGroupFooterModifier(strategy).AddGroupFooter(band.Report, band.LevelInternal);
		}
		#endregion
		#region hide footer
		public void HideFooter(GroupSortReflectItem reflectItem) {
			this.ActiveID = reflectItem.ID;
			fakeControl.BeginInvoke(new Action<GroupSortReflectItem>(HideFooterProc), reflectItem);
		}
		void HideFooterProc(GroupSortReflectItem reflectItem) {
			HideFooterProcCore(realModifierStrategy, reflectItem);
		}
		void HideFooterProcCore(ModifierStrategyBase strategy, GroupSortReflectItem reflectItem) {
			if(ReportIsDisposed)
				return;
			XRGroup group = GetXRGroup(reflectItem.Band);
			if(group != null)
				new ReportDeleteModifier(strategy).DeleteComponents(group.Footer);
		}
		public bool CanHideFooter() {
			GroupSortReflectItem item = GetSelectedReflectItem();
			if(item != null) {
				XRGroup group = GetXRGroup(item.Band);
				if(group != null && BandContainsControls(group.Footer))
					return DialogResult.Yes == ShowWarning();
			}
			return true;
		}
		#endregion
		#region show header
		public void ShowHeader(GroupSortReflectItem reflectItem) {
			this.ActiveID = reflectItem.ID;
			fakeControl.BeginInvoke(new Action<GroupSortReflectItem>(ShowHeaderProc), reflectItem);
		}
		void ShowHeaderProc(GroupSortReflectItem reflectItem) {
			ShowHeaderProcCore(realModifierStrategy, reflectItem);
		}
		void ShowHeaderProcCore(ModifierStrategyBase strategy, GroupSortReflectItem reflectItem) {
			if(ReportIsDisposed)
				return;
			Band band = reflectItem.GroupField.Band;
			new ReportGroupHeaderModifier2(strategy, reflectItem.GroupField.Index + 1).AddGroupHeader(band.Report, band.SortFieldsInternal, band.LevelInternal + 1);
		}
		#endregion
		#region hide header
		public void HideHeader(GroupSortReflectItem reflectItem) {
			this.ActiveID = reflectItem.ID;
			fakeControl.BeginInvoke(new Action<GroupSortReflectItem>(HideHeaderProc), reflectItem);
		}
		void HideHeaderProc(GroupSortReflectItem reflectItem) {
			HideHeaderProcCore(realModifierStrategy, reflectItem);
		}
		void HideHeaderProcCore(ModifierStrategyBase strategy, GroupSortReflectItem reflectItem) {
			if(ReportIsDisposed)
				return;
			XRGroup group = GetXRGroup(reflectItem.Band);
			if(group != null)
				new ReportDeleteHeaderModifier(strategy).DeleteHeader(group, GetNextSortFields(reflectItem), int.MaxValue);
		}
		public bool CanHideHeader() {
			GroupSortReflectItem item = GetSelectedReflectItem();
			if(item != null) {
				XRGroup group = GetXRGroup(item.Band);
				if(group != null && (BandContainsControls(group.Header) || BandContainsControls(group.Footer)))
					return DialogResult.Yes == ShowWarning();
			}
			return true;
		}
		#endregion
		#region set property value
		public void SetPropertyValue(GroupSortReflectItem reflectItem, PropertyDescriptor propertyDescriptor, object value) {
			this.ActiveID = reflectItem.ID;
			fakeControl.BeginInvoke(new Action3<GroupSortReflectItem, PropertyDescriptor, object>(SetPropertyValueProc), reflectItem, propertyDescriptor, value);
		}
		public override void CaptureTreeList(System.Windows.Forms.Control control) {
			base.CaptureTreeList(((GroupSortUserControl)control).GroupSortTreeList);
		}
		internal protected virtual IPopupFieldNamePicker CreatePopupFieldNamePicker() {
			return new PopupFieldNamePicker();
		}
		void SetPropertyValueProc(GroupSortReflectItem reflectItem, PropertyDescriptor propertyDescriptor, object value) {
			SetPropertyValueProcCore(realModifierStrategy, reflectItem, propertyDescriptor, value);
		}
		void SetPropertyValueProcCore(ModifierStrategyBase strategy, GroupSortReflectItem reflectItem, PropertyDescriptor propertyDescriptor, object value) {
			if(ReportIsDisposed)
				return;
			new BandPropertyModifier(strategy, reflectItem.GroupField, propertyDescriptor, value).Modify(reflectItem.GroupField.Band);
		}
		#endregion
		public bool IsFieldNameEnabled() {
			GroupSortReflectItem item = GetSelectedReflectItem();
			if(item == null)
				return true;
			CheckModifierStrategy strategy = new CheckModifierStrategy(serviceProvider);
			PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(typeof(GroupField))[ColumnNames.FieldName];
			SetPropertyValueProcCore(strategy, item, propertyDescriptor, string.Empty);
			return strategy.Valid;
		}		
		public bool IsSortOrderEnabled() {
			GroupSortReflectItem item = GetSelectedReflectItem();
			if(item == null)
				return true;
			CheckModifierStrategy strategy = new CheckModifierStrategy(serviceProvider);
			PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(typeof(GroupField))[ColumnNames.SortOrder];
			SetPropertyValueProcCore(strategy, item, propertyDescriptor, XRColumnSortOrder.Ascending);
			return strategy.Valid;
		}
		public bool IsShowHeaderEnabled() {
			GroupSortReflectItem item = GetSelectedReflectItem();
			if(item == null)
				return true;
			CheckModifierStrategy strategy = new CheckModifierStrategy(serviceProvider);
			if(item.ShowHeader)
				HideHeaderProcCore(strategy, item);
			else
				ShowHeaderProcCore(strategy, item);
			return strategy.Valid;
		}
		public bool IsShowFooterEnabled() {
			GroupSortReflectItem item = GetSelectedReflectItem();
			if(item == null)
				return true;
			CheckModifierStrategy strategy = new CheckModifierStrategy(serviceProvider);
			if(item.ShowFooter)
				HideFooterProcCore(strategy, item);
			else
				ShowFooterProcCore(strategy, item);
			return strategy.Valid;
		}
		public bool IsAddSortEnabled() {
			DetailBand detail = GetDetail();
			if (detail == null)
				return true;
			CheckModifierStrategy strategy = new CheckModifierStrategy(serviceProvider);
			AddSortProcCore(strategy, string.Empty, detail);
			return strategy.Valid;			
		}
		public bool IsAddGroupEnabled() {
			DetailBand detail = GetDetail();
			if(detail == null)
				return true;
			CheckModifierStrategy strategy = new CheckModifierStrategy(serviceProvider);
			AddGroupProcCore(strategy, string.Empty, detail);
			return strategy.Valid;
		}
		public bool IsMoveSortDownEnabled() {
			GroupSortReflectItem item = GetSelectedReflectItem();
			if(item == null)
				return true;
			CheckModifierStrategy strategy = new CheckModifierStrategy(serviceProvider);
			MoveSortDownProcCore(strategy, item);
			return strategy.Valid;
		}
		public bool IsMoveSortUpEnabled() {
			GroupSortReflectItem item = GetSelectedReflectItem();
			if(item == null)
				return true;
			CheckModifierStrategy strategy = new CheckModifierStrategy(serviceProvider);
			MoveSortUpProcCore(strategy, item);
			return strategy.Valid;
		}
		public bool IsDeleteSortEnabled() {
			GroupSortReflectItem item = GetSelectedReflectItem();
			if(item == null)
				return true;
			CheckModifierStrategy strategy = new CheckModifierStrategy(serviceProvider);
			DeleteSortProcCore(strategy, item);
			return strategy.Valid;
		}
	}
	static class ColumnNames {
		public const string FieldName = "FieldName";
		public const string SortOrder = "SortOrder";
		public const string ShowHeader = "ShowHeader";
		public const string ShowFooter = "ShowFooter";
	}
	public class GroupSortReflectCollection : List<GroupSortReflectItem> {
		public GroupSortReflectItem GetItemByGroupField(GroupField groupField) {
			foreach(GroupSortReflectItem item in this) {
				if(item.GroupField == groupField)
					return item;
			}
			return null;
		}
		public GroupSortReflectItem GetItemByID(int id) {
			foreach(GroupSortReflectItem item in this) {
				if(item.ID == id)
					return item;
			}
			return null;
		}
		public Band GetNextBand(GroupSortReflectItem item) {
			return GetNextBandCore(item, IndexOf(item));
		}
		public GroupSortReflectItem GetNextItem(GroupSortReflectItem item) {
			int nextIndex = IndexOf(item) + 1;
			return nextIndex < this.Count ? this[nextIndex] : null;
		}
		public GroupSortReflectItem GetPreviousItem(GroupSortReflectItem item) {
			int prevIndex = IndexOf(item) - 1;
			return prevIndex >= 0 ? this[prevIndex] : null;
		}
		Band GetNextBandCore(GroupSortReflectItem item, int index) {
			int nextIndex = index + 1;
			if(nextIndex < this.Count) {
				GroupSortReflectItem nextItem = this[nextIndex];
				if(item.Band != nextItem.Band)
					return nextItem.Band;
				return GetNextBandCore(nextItem, nextIndex);
			}
			return null;
		}
	}
	public class GroupSortReflectItem {
		int id;
		GroupField groupField;
		bool showHeader;
		bool showFooter;
		GroupSortController controller;
		public GroupSortReflectItem(GroupSortController controller, int id, GroupField groupField) {
			this.id = id;
			this.groupField = groupField;
			this.controller = controller;
		}
		internal Band Band {
			get {
				return groupField.Band;
			}
		}
		internal GroupField GroupField {
			get { return groupField; }
		}
		public int ID {
			get { return id; }
		}
		public int ParentID {
			get { return id - 1; }
		}
		[DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraReports.UI.GroupField.FieldName")]
		public string FieldName {
			get { return groupField.FieldName; }
			set {
				PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(typeof(GroupField))[ColumnNames.FieldName];
				controller.SetPropertyValue(this, propertyDescriptor, value);
			}
		}
		[DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraReports.UI.GroupField.SortOrder")]
		public XRColumnSortOrder SortOrder {
			get { return groupField.SortOrder; }
			set {
				PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(typeof(GroupField))[ColumnNames.SortOrder];
				controller.SetPropertyValue(this, propertyDescriptor, value);
			}
		}
		[DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraReports.Design.GroupSort.GroupSortReflectItem.ShowHeader")]
		public bool ShowHeader {
			get { return showHeader; }
			set {
				if(value)
					this.controller.ShowHeader(this);
				else
					this.controller.HideHeader(this);
			}
		}
		[DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraReports.Design.GroupSort.GroupSortReflectItem.ShowFooter")]
		public bool ShowFooter {
			get { return showFooter; }
			set {
				if(value)
					this.controller.ShowFooter(this);
				else
					this.controller.HideFooter(this);
			}
		}
		public void Initialize(bool showHeader, bool showFooter) {
			this.showHeader = showHeader;
			this.showFooter = showFooter;
		}
	}
}
