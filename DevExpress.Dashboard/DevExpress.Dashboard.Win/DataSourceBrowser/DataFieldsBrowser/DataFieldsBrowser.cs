#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.Localization;
using DevExpress.DataAccess.Native;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.DashboardWin.ServiceModel;
namespace DevExpress.DashboardWin.Native {
	public partial class DataFieldsBrowser : DashboardUserControl, IDataFieldsBrowserView {
		public static DataFieldsBrowserItem GetItem(TreeListNode node) {
			return node.GetValue(0) as DataFieldsBrowserItem;
		}
		public static DataField GetDataField(TreeListNode node) {
			if (node != null) {
				DataFieldsBrowserItem item = GetItem(node);
				if (item != null)
					return item.DataNode as DataField;
			}
			return null;
		}
		static bool CheckIsUnsupportedItem(DataFieldsBrowserItem item) {
			return item != null && item.DataField == null && item.DataNode.IsList && item.DataNode.ChildNodes.Count == 0;
		}
		readonly ExpandAndFocusCache expandAndFocusCache = new ExpandAndFocusCache();
		readonly Locker lockerUI = new Locker();
		Skin skin;
		DataFieldsBrowserDisplayMode displayMode = DataFieldsBrowserDisplayMode.All;
		bool allowGlyphSkinning;
		bool isOlap;
		IServiceProvider serviceProvider;
		protected override IEnumerable<object> Children {
			get { 
				yield return panelControl;
				foreach (Control control in panelControl.Controls)
					yield return control;
			}
		}
		public bool AllowGlyphSkinning {
			get { return this.allowGlyphSkinning; }
			set {
				if(value != allowGlyphSkinning) {
					this.allowGlyphSkinning = value;
					UpdateGlyphSkinning();
				}
			}
		}
		public bool ActualGroupByType { get { return ((IDataFieldsBrowserView)this).GroupByTypeEnabled && ((IDataFieldsBrowserView)this).GroupByType; } }
		protected ImageList ImageList { get { return imageList; } }
		protected TreeList TreeList { get { return treeList; } }
		protected override BarManager BarManager { get { return barManager; } }
		event EventHandler groupAndSortChanged;
		event EventHandler<RequestChildNodesEventArgs> requestChildNodes;
		event EventHandler<DataFieldEventArgs> dataFieldDoubleClick;
		event EventHandler<DataFieldEventArgs> focusedDataFieldChanged;
		event EventHandler RefreshButtonClick;
		public DataFieldsBrowser() {
			InitializeComponent();
			if(!IsVSDesignMode) {
				groupByTypeBarItem.Caption = DashboardWinLocalizer.GetString(DashboardWinStringId.TooltipGroupByType);
				sortAscendingBarItem.Caption = DashboardWinLocalizer.GetString(DashboardWinStringId.TooltipSortAscending);
				sortDescendingBarItem.Caption = DashboardWinLocalizer.GetString(DashboardWinStringId.TooltipSortDescending);
				refreshFieldListBarItem.Caption = DashboardWinLocalizer.GetString(DashboardWinStringId.TooltipRefreshFieldList);
				refreshFieldListBarItem.Visibility = BarItemVisibility.Never;
				LookAndFeel.StyleChanged += OnLookAndFeelStyleChanged;
				SetSkin();
				UpdateGlyphSkinning();
			}
			this.standaloneBarDockControl.AllowTransparency = true;
		}
		public void SetToolbarVisibility(bool visible) {
			toolsBar.Visible = visible;
			standaloneBarDockControl.Visible = visible;
			treeList.Dock = visible ? DockStyle.None : DockStyle.Fill;
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (components != null)
					components.Dispose();
			}
			base.Dispose(disposing);
		}
		protected DataField GetDataField(Point location) {
			return GetDataField(GetNode(location));
		}
		void UpdateGlyphSkinning() {
			string imageNamePostfix = AllowGlyphSkinning ? "_Monochrome" : string.Empty;
			ResourceImageHelper.FillImageListFromResources(imageList, "Images.DataPickerImages.png", typeof(ResFinder));
			ResourceImageHelper.FillImageListFromResources(imageList, "Images.DataPickerOlapImages.png", typeof(ResFinder));
			groupByTypeBarItem.Glyph = ImageHelper.GetImage("GroupBy" + imageNamePostfix);
			sortAscendingBarItem.Glyph = ImageHelper.GetImage("SortAscending" + imageNamePostfix);
			sortDescendingBarItem.Glyph = ImageHelper.GetImage("SortDescending" + imageNamePostfix);
			refreshFieldListBarItem.Glyph = ImageHelper.GetImage("Refresh" + imageNamePostfix);
			refreshFieldListBarItem.AllowGlyphSkinning = 
				groupByTypeBarItem.AllowGlyphSkinning =
				sortAscendingBarItem.AllowGlyphSkinning =
				sortDescendingBarItem.AllowGlyphSkinning = AllowGlyphSkinning ? DefaultBoolean.True : DefaultBoolean.False;
		}
		void toolTipController1_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e) {
			TreeListNode node = GetNode(e.ControlMousePosition);
			if (node != null) {
				DataFieldsBrowserItem item = GetItem(node);
				if (CheckIsUnsupportedItem(item))
					e.Info = new ToolTipControlInfo(item, DashboardWinLocalizer.GetString(DashboardWinStringId.MessageCollectionTypesNotSupported), false, ToolTipIconType.Warning);
			}
		}
		void SetSkin() {
			skin = DashboardSkins.GetSkin(LookAndFeel);
		}
		void OnLookAndFeelStyleChanged(object sender, EventArgs e) {
			SetSkin();
		}
		void treeList_CustomDrawNodeCell(object sender, CustomDrawNodeCellEventArgs e) {
			DataFieldsBrowserItem item = e.CellValue as DataFieldsBrowserItem;
			if (CheckIsUnsupportedItem(item))
				e.Appearance.ForeColor = skin != null ? skin.CommonSkin.Colors["DisabledText"] : SystemColors.GrayText;
		}
		void GroupAndSortBarItem_CheckedChanged(object sender, ItemClickEventArgs e) {
			if(groupAndSortChanged != null)
				groupAndSortChanged(this, EventArgs.Empty);
		}			
		void treeList_BeforeExpand(object sender, BeforeExpandEventArgs e) {
			if (e.CanExpand) {
				DataFieldsBrowserItem item = GetItem(e.Node);
				DataNode dataNode = item.DataNode;
				if (dataNode.IsDummyParent)
					dataNode.Expand(null);
				if (item.IsDummyParent) {
					UpdateNodes(e.Node, dataNode, false);
					item.IsDummyParent = false;
				}
			}
		}
		void treeList_AfterExpand(object sender, NodeEventArgs e) {
			if (!lockerUI.IsLocked)
				expandAndFocusCache.OnExpand(e.Node);
		}
		void treeList_AfterCollapse(object sender, NodeEventArgs e) {
			if (!lockerUI.IsLocked)
				expandAndFocusCache.OnCollapse(e.Node);
		}
		void treeList_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e) {
			if (!lockerUI.IsLocked)
				expandAndFocusCache.OnFocus(e.Node);
			if(focusedDataFieldChanged != null)
				focusedDataFieldChanged(this, new DataFieldEventArgs(GetDataField(treeList.FocusedNode)));
		}
		void treeList_MouseDoubleClick(object sender, MouseEventArgs e) {
			if(dataFieldDoubleClick != null)
				dataFieldDoubleClick(this, new DataFieldEventArgs(GetDataField(e.Location)));
		}
		void OnRefreshFieldListBarItemClick(object sender, ItemClickEventArgs e) {
			if(RefreshButtonClick != null) {
				RefreshButtonClick(this, new EventArgs());
			}
		}
		void ClearNodes(TreeListNode parentNode) {
			if (parentNode != null)
				parentNode.Nodes.Clear();
			else
				treeList.ClearNodes();
		}
		void UpdateNodes(TreeListNode parentNode, DataNode parentDataNode, bool checkNodeType) {
			treeList.BeginUnboundLoad();
			try {
				ClearNodes(parentNode);
				if (parentDataNode != null) {
					if (parentNode == null) {
						IDashboardParameterService service = (IDashboardParameterService)serviceProvider.GetService(typeof(IDashboardParameterService));
						parentNode = AddTreeListNode(null, new DataFieldsBrowserItem(parentDataNode, service));
					}
					if(parentNode != null)
						AddTreeListNodes(parentNode, parentDataNode, checkNodeType);
				}
			}
			finally {
				treeList.EndUnboundLoad();
			}
		}
		void AddTreeListNodes(TreeListNode parentNode, DataNode parentDataNode, bool checkNodeType) {
			IList childNodes;
			if (requestChildNodes != null) {
				RequestChildNodesEventArgs args = new RequestChildNodesEventArgs(parentDataNode);
				requestChildNodes(this, args);
				childNodes = args.ChildNodes;
			}
			else
				childNodes = null;
			if (childNodes != null) {
				List<DataFieldsBrowserItem> items = new List<DataFieldsBrowserItem>(childNodes.Count);
				foreach (DataNode dataNode in childNodes)
					if (!checkNodeType || !isOlap || CanAddOlapNode(dataNode.DataMember)) {
						IDashboardParameterService service = (IDashboardParameterService)serviceProvider.GetService(typeof(IDashboardParameterService));
						items.Add(new DataFieldsBrowserItem(dataNode, service));
					}
				if (ActualGroupByType) {
					ItemGroupContainer groupContainer = new ItemGroupContainer();
					items.ForEach(groupContainer.AddItem);
					foreach (List<DataFieldsBrowserItem> group in groupContainer.Groups)
						AddTreeListNodes(parentNode, parentDataNode, group);
					AddTreeListNodes(parentNode, parentDataNode, groupContainer.UntypedGroup);
				}
				else
					AddTreeListNodes(parentNode, parentDataNode, items);
			}
		}
		TreeListNode AddTreeListNode(TreeListNode parentNode, DataFieldsBrowserItem item) {
			TreeListNode node = treeList.AppendNode(new object[] { item, item.DisplayName }, parentNode);
			node.ImageIndex = node.SelectImageIndex = item.ImageIndex;
			return node;
		}
		bool CanAddOlapNode(string dataMember) {
			return displayMode == DataFieldsBrowserDisplayMode.All || 
				displayMode != DataFieldsBrowserDisplayMode.None && 
				(displayMode == DataFieldsBrowserDisplayMode.Measures) == OlapDataNodesCreator.IsMeasureFolder(dataMember);
		}
		void AddTreeListNodes(TreeListNode parentNode, DataNode parentDataNode, List<DataFieldsBrowserItem> items) {
			if(parentDataNode == null || !(parentDataNode is OlapHierarchyDataField))
				items.Sort(new ItemComparer(((IDataFieldsBrowserView)this).SortAscending)); 
			items.ForEach(item => {
				TreeListNode node = AddTreeListNode(parentNode, item);
				if(node != null)
					AddTreeListNodes(node, item.DataNode, false);
			});
		}
		TreeListNode GetRootNode() {
			return treeList.Nodes.Count > 0 ? treeList.Nodes[0] : null;
		}
		void ExpandRootNode() {
			TreeListNode rootNode = GetRootNode();
			if (rootNode != null)
				rootNode.Expanded = true;
		}
		void RefreshRootNode() {
			TreeListNode rootNode = GetRootNode();
			if (rootNode != null)
				treeList.RefreshNode(rootNode);
		}
		TreeListNode GetNode(Point location) {
			return treeList.CalcHitInfo(location).Node;
		}
		void ForEachNodeNonSetter(Action<TreeListNode> action, TreeListNodes nodes) {
			foreach (TreeListNode node in nodes) {
				action(node);
				ForEachNodeNonSetter(action, node.Nodes);
			}
		}
		#region IDataFieldsBrowserView members
		DataField IDataFieldsBrowserView.SelectedDataField {
			get {
				TreeListMultiSelection selection = treeList.Selection;
				if (selection.Count > 0) {
					TreeListNode node = treeList.Selection[0];
					DataFieldsBrowserItem item = GetItem(node);
					return item.DataField;
				}
				return null;
			}
		}
		DataField IDataFieldsBrowserView.FocusedDataField { get { return GetDataField(treeList.FocusedNode); } }
		DataFieldsBrowserDisplayMode IDataFieldsBrowserView.DisplayMode {
			get { return displayMode; }
			set { displayMode = value; }
		}
		bool IDataFieldsBrowserView.GroupByTypeEnabled {
			get { return groupByTypeBarItem.Visibility == BarItemVisibility.Always; }
			set { groupByTypeBarItem.Visibility = value ? BarItemVisibility.Always : BarItemVisibility.Never; }
		}
		bool IDataFieldsBrowserView.GroupByType {
			get { return groupByTypeBarItem.Checked; }
			set { groupByTypeBarItem.Checked = value; }
		}
		bool IDataFieldsBrowserView.IsOlap {
			get { return isOlap; }
			set { isOlap = value; }
		}
		bool IDataFieldsBrowserView.SortAscending {
			get { return sortAscendingBarItem.Checked; }
			set { sortAscendingBarItem.Checked = value; }
		}
		bool IDataFieldsBrowserView.SortDescending {
			get { return sortDescendingBarItem.Checked; }
			set { sortDescendingBarItem.Checked = value; }
		}
		event EventHandler IDataFieldsBrowserView.GroupAndSortChanged {
			add { groupAndSortChanged += value; }
			remove { groupAndSortChanged -= value; }
		}
		event EventHandler<RequestChildNodesEventArgs> IDataFieldsBrowserView.RequestChildNodes {
			add { requestChildNodes += value; }
			remove { requestChildNodes -= value; }
		}
		event EventHandler<DataFieldEventArgs> IDataFieldsBrowserView.DataFieldDoubleClick {
			add { dataFieldDoubleClick += value; }
			remove { dataFieldDoubleClick -= value; }
		}
		event EventHandler<DataFieldEventArgs> IDataFieldsBrowserView.FocusedDataFieldChanged {
			add { focusedDataFieldChanged += value; }
			remove { focusedDataFieldChanged -= value; }
		}
		event EventHandler IDataFieldsBrowserView.RefreshFieldListClick { 
			add { RefreshButtonClick += value; }
			remove { RefreshButtonClick -= value; }
		}
		void IDataFieldsBrowserView.ClearAndBuildNodes(DataSourceNodeBase dataSourceNode, bool expandAll, IServiceProvider serviceProvider) {
			this.serviceProvider = serviceProvider;
			expandAndFocusCache.Clear();
			((IDataFieldsBrowserView)this).BuildNodes(dataSourceNode, serviceProvider);
			if (expandAll)
				treeList.ExpandAll();
			treeList.ForceInitialize();
			treeList.BestFitColumns();
		}
		void IDataFieldsBrowserView.BuildNodes(DataSourceNodeBase dataSourceNode, IServiceProvider serviceProvider) {
			this.serviceProvider = serviceProvider;
			treeList.BeginUpdate();
			lockerUI.Lock();
			try {
				UpdateNodes(null, dataSourceNode, true);
				ExpandRootNode();
				treeList.NodesIterator.DoOperation(new ExpandAndFocusOperation(expandAndFocusCache));
			}
			finally {
				lockerUI.Unlock();
				treeList.EndUpdate();
			}
		}
		void IDataFieldsBrowserView.SelectNode(string dataMember) {
			expandAndFocusCache.OnFocus(dataMember);
			treeList.NodesIterator.DoOperation(new ExpandAndFocusOperation(expandAndFocusCache));
		}
		void IDataFieldsBrowserView.RestoreSelection(string dataMember) {
			TreeListNode newNode = null;
			ForEachNodeNonSetter((node) => {
				if (newNode != null)
					return;
				DataField field = GetDataField(node);
				if (field == null)
					return;
				if (field.DataMember == dataMember)
					newNode = node;
			}, TreeList.Nodes);
			if (newNode != null)
				TreeList.SetFocusedNode(newNode);
		}
		bool IDataFieldsBrowserView.RefreshButtonVisible {
			get {
				return refreshFieldListBarItem.Visibility == BarItemVisibility.Always;
			}
			set {
				refreshFieldListBarItem.Visibility = value ? BarItemVisibility.Always : BarItemVisibility.Never;
			}
		}
		#endregion
	}
}
