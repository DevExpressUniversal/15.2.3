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

namespace DevExpress.XtraTreeList {
	using System;
	using System.ComponentModel;
	using System.Drawing;
	using System.Windows.Forms;
	using DevExpress.XtraTreeList;
	using DevExpress.XtraTreeList.Columns;
	using DevExpress.XtraTreeList.Nodes;
	using DevExpress.XtraTreeList.ViewInfo;
	using DevExpress.XtraEditors.Drawing;
	using DevExpress.XtraEditors.ViewInfo;
	using DevExpress.Utils;
	using DevExpress.Utils.Drawing;
	using DevExpress.XtraTreeList.Data;
	using System.Collections;
	using DevExpress.XtraTreeList.Dragging;
	using DevExpress.XtraEditors;
	using DevExpress.XtraTreeList.FilterEditor;
	using System.Collections.Generic;
	using DevExpress.XtraEditors.Repository;
	using DevExpress.XtraEditors.Design;
	public delegate void AfterDragNodeEventHandler(object sender, AfterDragNodeEventArgs e);
	public delegate void AfterDropNodeEventHandler(object sender, AfterDropNodeEventArgs e);
	public delegate void BeforeExpandEventHandler(object sender, BeforeExpandEventArgs e);
	public delegate void BeforeCollapseEventHandler(object sender, BeforeCollapseEventArgs e);
	public delegate void BeforeDragNodeEventHandler(object sender, BeforeDragNodeEventArgs e);
	public delegate void BeforeDropNodeEventHandler(object sender, BeforeDropNodeEventArgs e);
	public delegate void BeforeFocusNodeEventHandler(object sender, BeforeFocusNodeEventArgs e);
	public delegate void CalcNodeHeightEventHandler(object sender, CalcNodeHeightEventArgs e);
	public delegate void CalcNodeDragImageIndexEventHandler(object sender, CalcNodeDragImageIndexEventArgs e);
	public delegate void CellValueChangedEventHandler(object sender, CellValueChangedEventArgs e);
	public delegate void ColumnChangedEventHandler(object sender, ColumnChangedEventArgs e);
	public delegate void ColumnWidthChangedEventHandler(object sender, ColumnChangedEventArgs e);
	public delegate void CompareNodeValuesEventHandler(object sender, CompareNodeValuesEventArgs e);
	public delegate void CustomDrawNodeIndicatorEventHandler(object sender, CustomDrawNodeIndicatorEventArgs e);
	public delegate void CustomDrawColumnHeaderEventHandler(object sender, CustomDrawColumnHeaderEventArgs e);
	public delegate void CustomDrawNodeButtonEventHandler(object sender, CustomDrawNodeButtonEventArgs e);
	public delegate void CustomDrawNodeCellEventHandler(object sender, CustomDrawNodeCellEventArgs e);
	public delegate void CustomDrawNodePreviewEventHandler(object sender, CustomDrawNodePreviewEventArgs e);
	public delegate void CustomDrawNodeImagesEventHandler(object sender, CustomDrawNodeImagesEventArgs e);
	public delegate void CustomDrawFooterEventHandler(object sender, CustomDrawEventArgs e);
	public delegate void CustomDrawRowFooterEventHandler(object sender, CustomDrawRowFooterEventArgs e);
	public delegate void CustomDrawFooterCellEventHandler(object sender, CustomDrawFooterCellEventArgs e);
	public delegate void CustomDrawRowFooterCellEventHandler(object sender, CustomDrawRowFooterCellEventArgs e);
	public delegate void CustomDrawEmptyAreaEventHandler(object sender, CustomDrawEmptyAreaEventArgs e);
	public delegate void CustomDrawNodeIndentEventHandler(object sender, CustomDrawNodeIndentEventArgs e);
	public delegate void CustomDrawNodeCheckBoxEventHandler(object sender, CustomDrawNodeCheckBoxEventArgs e);
	public delegate void CustomizeNewNodeFromOuterDataEventHandler(object sender, CustomizeNewNodeFromOuterDataEventArgs e);
	public delegate void GetStateImageEventHandler(object sender, GetStateImageEventArgs e);
	public delegate void GetSelectImageEventHandler(object sender, GetSelectImageEventArgs e);
	public delegate void GetPreviewTextEventHandler(object sender, GetPreviewTextEventArgs e);
	public delegate void GetCustomSummaryValueEventHandler(object sender, GetCustomSummaryValueEventArgs e);
	public delegate void GetCustomNodeCellEditEventHandler(object sender, GetCustomNodeCellEditEventArgs e);
	public delegate void GetCustomNodeCellStyleEventHandler(object sender, GetCustomNodeCellStyleEventArgs e);
	public delegate void GetNodeDisplayValueEventHandler(object sender, GetNodeDisplayValueEventArgs e);
	public delegate void CustomColumnDataEventHandler(object sender, TreeListCustomColumnDataEventArgs e);
	public delegate void FocusedNodeChangedEventHandler(object sender, FocusedNodeChangedEventArgs e);
	public delegate void FocusedColumnChangedEventHandler(object sender, FocusedColumnChangedEventArgs e);
	public delegate void InvalidNodeExceptionEventHandler(object sender, InvalidNodeExceptionEventArgs e);
	public delegate void ValidateNodeEventHandler(object sender, ValidateNodeEventArgs e);
	public delegate void NodeEventHandler(object sender, NodeEventArgs e);
	public delegate void NodeChangedEventHandler(object sender, NodeChangedEventArgs e);
	public delegate void NodeClickEventHandler(object sender, NodeClickEventArgs e);
	public delegate void TreeListMenuItemClickEventHandler(object sender, TreeListMenuItemClickEventArgs e);
	[Obsolete("You should use the 'PopupMenuShowingEventHandler' instead", false), EditorBrowsable(EditorBrowsableState.Never)]
	public delegate void TreeListMenuEventHandler(object sender, TreeListMenuEventArgs e);
	public delegate void PopupMenuShowingEventHandler(object sender, PopupMenuShowingEventArgs e);
	public delegate void CreateCustomNodeEventHandler(object sender, CreateCustomNodeEventArgs e);
	public delegate void VirtualTreeGetChildNodesEventHandler(object sender, VirtualTreeGetChildNodesInfo e);
	public delegate void VirtualTreeSetCellValueEventHandler(object sender, VirtualTreeSetCellValueInfo e);
	public delegate void VirtualTreeGetCellValueEventHandler(object sender, VirtualTreeGetCellValueInfo e);
	public delegate void FilterNodeEventHandler(object sender, FilterNodeEventArgs e);
	public delegate void DragObjectDropEventHandler(object sender, DragObjectDropEventArgs e);
	public delegate void DragObjectOverEventHandler(object sender, DragObjectOverEventArgs e);
	public delegate void DragObjectStartEventHandler(object sender, DragObjectStartEventArgs e);
	public delegate void CheckNodeEventHandler(object sender, CheckNodeEventArgs e);
	public delegate void NodePreviewHeightEventHandler(object sender, NodePreviewHeightEventArgs e);
	public delegate void CustomDrawObjectEventHandler(object sender, CustomDrawObjectEventArgs e);
	public delegate void FilterControlEventHandler(object sender, FilterControlEventArgs e);
	public delegate void FilterPopupDateEventHandler(object sender, FilterPopupDateEventArgs e);
	public delegate void FilterPopupListBoxEventHandler(object sender, FilterPopupListBoxEventArgs e);
	public delegate void FilterPopupCheckedListBoxEventHandler(object sender, FilterPopupCheckedListBoxEventArgs e);
	public delegate void UnboundExpressionEditorEventHandler(object sender, TreeListUnboundExpressionEditorEventArgs e);
	public delegate void CustomDrawBandHeaderEventHandler(object sender, CustomDrawBandHeaderEventArgs e);
	public delegate void BandEventHandler(object sender, BandEventArgs e);
	public class NodeEventArgs : EventArgs {
		TreeListNode node;
		public NodeEventArgs(TreeListNode node) {
			this.node = node;
		}
		public TreeListNode Node { get { return node; } }
	}
	public class CellEventArgs : NodeEventArgs {
		TreeListColumn column;
		public CellEventArgs(TreeListColumn column, TreeListNode node)
			: base(node) {
			this.column = column;
		}
		public TreeListColumn Column { get { return column; } }
	}
	public class GetStateImageEventArgs : NodeEventArgs {
		int nodeImageIndex;
		public GetStateImageEventArgs(TreeListNode node, int nodeImageIndex)
			: base(node) {
			this.nodeImageIndex = nodeImageIndex;
		}
		public int NodeImageIndex {
			get { return nodeImageIndex; }
			set { nodeImageIndex = value; }
		}
	}
	public class GetSelectImageEventArgs : GetStateImageEventArgs {
		bool focusedNode;
		public GetSelectImageEventArgs(TreeListNode node, int nodeImageIndex, bool focusedNode)
			: base(node, nodeImageIndex) {
			this.focusedNode = focusedNode;
		}
		public bool FocusedNode {
			get { return focusedNode; }
			set { focusedNode = value; }
		}
	}
	public class CellValueChangedEventArgs : CellEventArgs {
		protected object fValue;
		public CellValueChangedEventArgs(TreeListColumn column, TreeListNode node, object val)
			: base(column, node) {
			this.fValue = val;
		}
		public virtual object Value {
			get { return fValue; }
			set { }
		}
	}
	public class GetNodeDisplayValueEventArgs : CellValueChangedEventArgs {
		public GetNodeDisplayValueEventArgs(TreeListColumn column, TreeListNode node)
			: base(column, node, (node == null || node.Id == -1) ? null : node[column]) {
		}
		public override object Value {
			get { return fValue; }
			set { fValue = value; }
		}
	}
	public class TreeListCustomColumnDataEventArgs : EventArgs {
		int nodeId;
		public TreeListCustomColumnDataEventArgs(TreeListColumn column, int nodeId, object value, bool isGet) {
			this.nodeId = nodeId;
			Column = column;
			IsGetData = isGet;
			IsSetData = !IsGetData;
			Value = value;
		}
		protected TreeList TreeList { get { return Column.TreeList; } }
		public bool IsGetData { get; private set; }
		public bool IsSetData { get; private set; }
		public TreeListColumn Column { get; private set; }
		public TreeListNode Node { get { return TreeList != null ? TreeList.FindNodeByID(nodeId) : null; } }
		public object Row { get { return TreeList != null ? TreeList.Data.GetDataRow(nodeId) : null; } }
		public object Value { get; set; }
	}
	public class ColumnChangedEventArgs : EventArgs {
		TreeListColumn column;
		public ColumnChangedEventArgs(TreeListColumn column) {
			this.column = column;
		}
		public TreeListColumn Column {
			get { return column; }
		}
	}
	public class GetPreviewTextEventArgs : NodeEventArgs {
		string previewText;
		public GetPreviewTextEventArgs(TreeListNode node, string previewText)
			: base(node) {
			this.previewText = previewText;
		}
		public string PreviewText {
			get { return previewText; }
			set {
				if(value == null)
					value = "";
				previewText = value;
			}
		}
	}
	public class GetCustomSummaryValueEventArgs : EventArgs {
		TreeListNodes nodes;
		TreeListColumn column;
		bool isSummaryFooter;
		object customValue;
		public GetCustomSummaryValueEventArgs(TreeListNodes nodes, TreeListColumn column,
			bool isSummaryFooter) {
			this.nodes = nodes;
			this.column = column;
			this.isSummaryFooter = isSummaryFooter;
			customValue = null;
		}
		public TreeListNodes Nodes { get { return nodes; } }
		public TreeListColumn Column { get { return column; } }
		public bool IsSummaryFooter { get { return isSummaryFooter; } }
		public object CustomValue {
			get { return customValue; }
			set { customValue = value; }
		}
	}
	public class BeforeExpandEventArgs : NodeEventArgs {
		bool canExpand;
		public BeforeExpandEventArgs(TreeListNode node)
			: base(node) {
			this.canExpand = true;
		}
		public bool CanExpand {
			get { return canExpand; }
			set { canExpand = value; }
		}
	}
	public class BeforeCollapseEventArgs : NodeEventArgs {
		bool canCollapse;
		public BeforeCollapseEventArgs(TreeListNode node)
			: base(node) {
			this.canCollapse = true;
		}
		public bool CanCollapse {
			get { return canCollapse; }
			set { canCollapse = value; }
		}
	}
	public class AfterDragNodeEventArgs : NodeEventArgs {
		public AfterDragNodeEventArgs(IEnumerable<TreeListNode> nodes, TreeListNode node)
			: base(node) {
			Nodes = nodes;
		}
		public IEnumerable<TreeListNode> Nodes { get; private set; }
	}
	public class BeforeDragNodeEventArgs : NodeEventArgs {
		bool canDrag;
		public BeforeDragNodeEventArgs(TreeListNode node)
			: base(node) {
			this.canDrag = true;
		}
		public BeforeDragNodeEventArgs(IList<TreeListNode> nodes, TreeListNode node)
			: this(node) {
			Nodes = nodes;
		}
		public bool CanDrag {
			get { return canDrag; }
			set { canDrag = value; }
		}
		public IList<TreeListNode> Nodes { get; private set; }
	}
	public class BeforeFocusNodeEventArgs : FocusedNodeChangedEventArgs {
		bool canFocus;
		public BeforeFocusNodeEventArgs(TreeListNode old, TreeListNode node)
			: base(old, node) {
			this.canFocus = true;
		}
		public bool CanFocus {
			get { return canFocus; }
			set { canFocus = value; }
		}
	}
	public class NodeChangedEventArgs : NodeEventArgs {
		NodeChangeTypeEnum changeType;
		public NodeChangedEventArgs(TreeListNode node, NodeChangeTypeEnum changeType)
			: base(node) {
			this.changeType = changeType;
		}
		public NodeChangeTypeEnum ChangeType { get { return changeType; } }
	}
	public class FocusedNodeChangedEventArgs : NodeEventArgs {
		TreeListNode oldNode;
		public FocusedNodeChangedEventArgs(TreeListNode old, TreeListNode node)
			: base(node) {
			this.oldNode = old;
		}
		public TreeListNode OldNode { get { return oldNode; } }
	}
	public class FocusedColumnChangedEventArgs : ColumnChangedEventArgs {
		TreeListColumn oldColumn;
		public FocusedColumnChangedEventArgs(TreeListColumn column, TreeListColumn oldColumn)
			: base(column) {
			this.oldColumn = oldColumn;
		}
		public TreeListColumn OldColumn { get { return oldColumn; } }
	}
	public class CompareNodeValuesEventArgs : EventArgs {
		TreeListNode node1;
		TreeListNode node2;
		object nodeValue1;
		object nodeValue2;
		TreeListColumn column;
		SortOrder sortOrder;
		int result;
		public CompareNodeValuesEventArgs(TreeListNode node1, TreeListNode node2,
			object nodeValue1, object nodeValue2, TreeListColumn column,
			SortOrder sortOrder, int result) {
			this.node1 = node1;
			this.node2 = node2;
			this.nodeValue1 = nodeValue1;
			this.nodeValue2 = nodeValue2;
			this.column = column;
			this.sortOrder = sortOrder;
			this.result = result;
		}
		public TreeListNode Node1 { get { return node1; } }
		public TreeListNode Node2 { get { return node2; } }
		public object NodeValue1 { get { return nodeValue1; } }
		public object NodeValue2 { get { return nodeValue2; } }
		public TreeListColumn Column { get { return column; } }
		public SortOrder SortOrder { get { return sortOrder; } }
		public int Result {
			get { return result; }
			set { result = value; }
		}
	}
	public class ValidateNodeEventArgs : NodeEventArgs {
		bool valid;
		string errorText;
		public ValidateNodeEventArgs(TreeListNode node)
			: base(node) {
			this.valid = true;
			this.errorText = string.Empty;
		}
		public bool Valid {
			get { return valid; }
			set { valid = value; }
		}
		public string ErrorText {
			get { return errorText; }
			set { errorText = value; }
		}
	}
	public class InvalidNodeExceptionEventArgs : DevExpress.XtraEditors.Controls.ExceptionEventArgs {
		TreeListNode node;
		public InvalidNodeExceptionEventArgs(Exception except, string windowText, TreeListNode node)
			: base(windowText, except) {
			this.node = node;
		}
		public TreeListNode Node { get { return node; } }
	}
	public class GetCustomNodeCellEditEventArgs : CellEventArgs {
		DevExpress.XtraEditors.Repository.RepositoryItem repositoryItem;
		public GetCustomNodeCellEditEventArgs(TreeListColumn column, TreeListNode node, DevExpress.XtraEditors.Repository.RepositoryItem repositoryItem) :
			base(column, node) {
			this.repositoryItem = repositoryItem;
		}
		public DevExpress.XtraEditors.Repository.RepositoryItem RepositoryItem {
			get { return repositoryItem; }
			set {
				if(value == null) return;
				repositoryItem = value;
			}
		}
	}
	public class GetCustomNodeCellStyleEventArgs : CellEventArgs {
		AppearanceObject appearance;
		public GetCustomNodeCellStyleEventArgs(TreeListColumn column, TreeListNode node, AppearanceObject appearance) :
			base(column, node) {
			this.appearance = appearance;
		}
		public AppearanceObject Appearance { get { return appearance; } }
	}
	public class CustomDrawEventArgs : EventArgs {
		bool handled;
		protected ObjectPainter fPainter;
		protected StyleObjectInfoArgs fObjectArgs;
		MethodInvoker defaultDraw;
		public CustomDrawEventArgs(GraphicsCache cache, Rectangle r, AppearanceObject appearance) : this(cache, r, appearance, null) { }
		public CustomDrawEventArgs(GraphicsCache cache, Rectangle r, AppearanceObject appearance, ObjectPainter painter) : this(cache, r, appearance, painter, false) { }
		public CustomDrawEventArgs(GraphicsCache cache, Rectangle r, AppearanceObject appearance, ObjectPainter painter, bool isRightToLeft)
			: this(painter) {
			this.fObjectArgs = CreateObjectArgs(cache, r, appearance);
			this.fObjectArgs.RightToLeft = isRightToLeft;
		}
		protected CustomDrawEventArgs(ObjectPainter painter) {
			this.handled = false;
			this.fPainter = painter;
		}
		protected virtual StyleObjectInfoArgs CreateObjectArgs(GraphicsCache cache, Rectangle r, AppearanceObject appearance) {
			return new StyleObjectInfoArgs(cache, r, appearance, ObjectState.Normal);
		}
		public bool IsRightToLeft { get { return fObjectArgs.RightToLeft; } }
		public Graphics Graphics { get { return Cache.Graphics; } }
		public GraphicsCache Cache { get { return ObjectArgs.Cache; } }
		public ObjectPainter Painter { get { return fPainter; } }
		public ObjectInfoArgs ObjectArgs { get { return fObjectArgs; } }
		public Rectangle Bounds { get { return ObjectArgs.Bounds; } }
		public AppearanceObject Appearance {
			get { return fObjectArgs.Appearance; }
		}
		public bool Handled { get { return handled; } set { handled = value; } }
		public void DefaultDraw() {
			if(defaultDraw != null && !Handled) {
				Handled = true;
				defaultDraw();
			}
		}
		internal void SetDefaultDraw(MethodInvoker defaultDraw) {
			this.defaultDraw = defaultDraw;
		}
	}
	public class CustomDrawNodeEventArgs : CustomDrawEventArgs {
		TreeListNode node;
		public CustomDrawNodeEventArgs(GraphicsCache cache, Rectangle r, AppearanceObject appearance) : this(cache, r, appearance, null) { }
		public CustomDrawNodeEventArgs(GraphicsCache cache, Rectangle r, AppearanceObject appearance, ObjectPainter painter) : this(cache, r, appearance, null, painter, false) { }
		public CustomDrawNodeEventArgs(GraphicsCache cache, Rectangle r, AppearanceObject appearance, TreeListNode node, ObjectPainter painter, bool isRightToLeft)
			: base(painter) {
			this.node = node;
			this.fObjectArgs = CreateObjectArgs(cache, r, appearance, node);
			this.fObjectArgs.RightToLeft = isRightToLeft;
		}
		protected virtual StyleObjectInfoArgs CreateObjectArgs(GraphicsCache cache, Rectangle r, AppearanceObject appearance, TreeListNode node) {
			return CreateObjectArgs(cache, r, appearance);
		}
		sealed protected override StyleObjectInfoArgs CreateObjectArgs(GraphicsCache cache, Rectangle r, AppearanceObject appearance) { return base.CreateObjectArgs(cache, r, appearance); }
		public TreeListNode Node { get { return node; } }
	}
	public class CustomDrawNodeIndicatorEventArgs : CustomDrawNodeEventArgs {
		bool isNodeIndicator;
		public CustomDrawNodeIndicatorEventArgs(GraphicsCache cache, Rectangle r, AppearanceObject appearance,
			bool isNodeIndicator, int imageIndex, bool topMost, TreeListNode node, ObjectPainter painter)
			: this(cache, r, appearance, isNodeIndicator, imageIndex, topMost, node, painter, false) { }
		public CustomDrawNodeIndicatorEventArgs(GraphicsCache cache, Rectangle r, AppearanceObject appearance,
			bool isNodeIndicator, int imageIndex, bool topMost, TreeListNode node, ObjectPainter painter, bool isRightToLeft)
			: base(cache, r, appearance, node, painter, isRightToLeft) {
			this.isNodeIndicator = isNodeIndicator;
			IndicatorArgs.Kind = isNodeIndicator ? IndicatorKind.Row : IndicatorKind.RowFooter;
			IndicatorArgs.ImageIndex = imageIndex;
			IndicatorArgs.IsTopMost = topMost;
			IndicatorArgs.ImageIndex = imageIndex;
		}
		protected override StyleObjectInfoArgs CreateObjectArgs(GraphicsCache cache, Rectangle r, AppearanceObject appearance, TreeListNode node) {
			object imageCollection = (node != null && node.TreeList != null) ? node.TreeList.Painter.ElementPainters.IndicatorImageCollection : null;
			IndicatorObjectInfoArgs args = new IndicatorObjectInfoArgs(r, appearance, imageCollection, 0, IndicatorKind.Row);
			args.Cache = cache;
			return args;
		}
		public bool IsNodeIndicator { get { return isNodeIndicator; } }
		public int ImageIndex {
			get { return IndicatorArgs.ImageIndex; }
			set { IndicatorArgs.ImageIndex = value; }
		}
		IndicatorObjectInfoArgs IndicatorArgs { get { return (IndicatorObjectInfoArgs)ObjectArgs; } }
	}
	public class CustomDrawColumnHeaderEventArgs : CustomDrawEventArgs {
		ColumnInfo columnInfo;
		HitInfoType columnType;
		public CustomDrawColumnHeaderEventArgs(GraphicsCache cache, Rectangle r, AppearanceObject appearance) : this(cache, r, appearance, false) { }
		public CustomDrawColumnHeaderEventArgs(GraphicsCache cache, Rectangle r, AppearanceObject appearance, bool isRightToLeft) :
			base(cache, r, appearance, null, isRightToLeft) {
			this.columnType = HitInfoType.None;
			this.columnInfo = new ColumnInfo(null);
		}
		internal void Init(DevExpress.XtraTreeList.ViewInfo.ColumnInfo ci, HeaderObjectPainter painter, bool dtSelected) {
			columnInfo = ci;
			columnInfo.Cache = fObjectArgs.Cache;
			columnInfo.DesignTimeSelected = dtSelected;
			fPainter = painter;
			fObjectArgs = columnInfo;
			switch(columnInfo.Type) {
				case DevExpress.XtraTreeList.ViewInfo.ColumnInfo.ColumnInfoType.BehindColumn:
					columnType = HitInfoType.BehindColumn;
					break;
				case DevExpress.XtraTreeList.ViewInfo.ColumnInfo.ColumnInfoType.ColumnButton:
					columnType = HitInfoType.ColumnButton;
					break;
				case DevExpress.XtraTreeList.ViewInfo.ColumnInfo.ColumnInfoType.Column:
					columnType = HitInfoType.Column;
					break;
			}
		}
		public Rectangle CaptionRect { get { return columnInfo.CaptionRect; } }
		public Rectangle SortShapeRect { get { return columnInfo.SortShapeRect; } }
		public TreeListColumn Column { get { return columnInfo.Column; } }
		public bool Pressed { get { return columnInfo.Pressed; } }
		public bool HotTrack { get { return columnInfo.MouseOver; } }
		public HitInfoType ColumnType { get { return columnType; } }
		public string Caption {
			get { return columnInfo.Caption; }
			set {
				if(value == null) value = string.Empty;
				columnInfo.Caption = value;
			}
		}
	}
	public class CustomDrawNodePreviewEventArgs : CustomDrawNodeEventArgs {
		string previewText;
		public CustomDrawNodePreviewEventArgs(GraphicsCache cache, Rectangle r, AppearanceObject appearance, TreeListNode node, string previewText) : this(cache, r, appearance, node, previewText, false) { }
		public CustomDrawNodePreviewEventArgs(GraphicsCache cache, Rectangle r, AppearanceObject appearance, TreeListNode node, string previewText, bool isRightToLeft)
			: base(cache, r, appearance, node, null, isRightToLeft) {
			this.previewText = previewText;
		}
		public string PreviewText {
			get { return previewText; }
			set {
				if(value == null) value = string.Empty;
				previewText = value;
			}
		}
	}
	public class CustomDrawNodeCellEventArgs : CustomDrawNodeEventArgs {
		TreeListColumn column;
		BaseEditViewInfo viewInfo;
		BaseEditPainter painter;
		string cellText;
		bool focused, fillBackground;
		public CustomDrawNodeCellEventArgs(GraphicsCache cache, Rectangle r, AppearanceObject appearance, BaseEditPainter painter,
			TreeListColumn column, TreeListNode node, BaseEditViewInfo viewInfo, bool focused)
			: this(cache, r, appearance, painter, column, node, viewInfo, focused, false) {
		}
		public CustomDrawNodeCellEventArgs(GraphicsCache cache, Rectangle r, AppearanceObject appearance, BaseEditPainter painter,
			TreeListColumn column, TreeListNode node, BaseEditViewInfo viewInfo, bool focused, bool isRightToLeft)
			: base(cache, r, appearance, node, null, isRightToLeft) {
			this.column = column;
			this.painter = painter;
			this.viewInfo = viewInfo;
			this.cellText = viewInfo != null ? viewInfo.DisplayText : "";
			this.focused = focused;
			this.fillBackground = false;
		}
		public TreeListColumn Column { get { return column; } }
		public object CellValue { get { return EditViewInfo.EditValue; } }
		public string CellText { get { return cellText; } set { cellText = (value == null ? string.Empty : value); } }
		public bool Focused { get { return focused; } }
		public BaseEditViewInfo EditViewInfo { get { return viewInfo; } }
		public BaseEditPainter EditPainter { get { return painter; } }
		internal bool FillBackground {
			get {
				if(EditViewInfo.FillBackground) return false;
				return fillBackground;
			}
			set { fillBackground = value; }
		}
	}
	public class CustomDrawFooterEventArgs : CustomDrawEventArgs {
		public CustomDrawFooterEventArgs(GraphicsCache cache, Rectangle r, AppearanceObject appearance, ObjectPainter painter) : this(cache, r, appearance, painter, false) { }
		public CustomDrawFooterEventArgs(GraphicsCache cache, Rectangle r, AppearanceObject appearance, ObjectPainter painter, bool isRightToLeft) : base(cache, r, appearance, painter, isRightToLeft) { }
		protected override StyleObjectInfoArgs CreateObjectArgs(GraphicsCache cache, Rectangle r, AppearanceObject appearance) {
			return new FooterPanelInfoArgs(cache, appearance, r, 1, 0);
		}
	}
	public class CustomDrawRowFooterEventArgs : CustomDrawFooterEventArgs {
		TreeListNode node;
		public CustomDrawRowFooterEventArgs(GraphicsCache cache, Rectangle r, AppearanceObject appearance, ObjectPainter painter, TreeListNode node) : this(cache, r, appearance, painter, node, false) { }
		public CustomDrawRowFooterEventArgs(GraphicsCache cache, Rectangle r, AppearanceObject appearance, ObjectPainter painter, TreeListNode node, bool isRightToLeft)
			: base(cache, r, appearance, painter, isRightToLeft) {
			this.node = node;
		}
		public TreeListNode Node { get { return node; } }
	}
	public class CustomDrawFooterCellEventArgs : CustomDrawEventArgs {
		TreeListColumn column;
		SummaryItemType itemType;
		public CustomDrawFooterCellEventArgs(GraphicsCache cache, Rectangle r, AppearanceObject appearance,
			TreeListColumn column, SummaryItemType itemType, string text, ObjectPainter painter)
			: this(cache, r, appearance, column, itemType, text, painter, false) { }
		public CustomDrawFooterCellEventArgs(GraphicsCache cache, Rectangle r, AppearanceObject appearance,
			TreeListColumn column, SummaryItemType itemType, string text, ObjectPainter painter, bool isRightToLeft)
			: base(cache, r, appearance, painter, isRightToLeft) {
			this.column = column;
			this.itemType = itemType;
			this.Text = text;
		}
		protected override StyleObjectInfoArgs CreateObjectArgs(GraphicsCache cache, Rectangle r, AppearanceObject appearance) {
			FooterCellInfoArgs args = new FooterCellInfoArgs(cache);
			args.SetAppearance(appearance);
			args.Bounds = r;
			return args;
		}
		public TreeListColumn Column { get { return column; } }
		public SummaryItemType ItemType { get { return itemType; } }
		public string Text {
			get { return ((FooterCellInfoArgs)ObjectArgs).DisplayText; }
			set {
				if(value == null) value = string.Empty;
				((FooterCellInfoArgs)ObjectArgs).DisplayText = value;
			}
		}
	}
	public class CustomDrawRowFooterCellEventArgs : CustomDrawFooterCellEventArgs {
		TreeListNode node;
		public CustomDrawRowFooterCellEventArgs(GraphicsCache cache, Rectangle r, AppearanceObject appearance,
			TreeListColumn column, TreeListNode node, SummaryItemType itemType, string text, ObjectPainter painter)
			: this(cache, r, appearance, column, node, itemType, text, painter, false) {
			this.node = node;
		}
		public CustomDrawRowFooterCellEventArgs(GraphicsCache cache, Rectangle r, AppearanceObject appearance,
			TreeListColumn column, TreeListNode node, SummaryItemType itemType, string text, ObjectPainter painter, bool isRightToLeft)
			: base(cache, r, appearance, column, itemType, text, painter, isRightToLeft) {
			this.node = node;
		}
		public TreeListNode Node { get { return node; } }
	}
	public class CustomDrawNodeImagesEventArgs : CustomDrawNodeEventArgs {
		int selectImageIndex, stateImageIndex;
		Rectangle selectRect, stateRect;
		Point selectImageLocation, stateImageLocation;
		bool fillBackground;
		public CustomDrawNodeImagesEventArgs(GraphicsCache cache, Rectangle r, AppearanceObject appearance, TreeListNode node, int selectImageIndex, int stateImageIndex, Rectangle selectRect, Rectangle stateRect,
			Point selectImageLocation, Point stateImageLocation)
			: this(cache, r, appearance, node, selectImageIndex, stateImageIndex, selectRect, stateRect, selectImageLocation, stateImageLocation, false) { }
		public CustomDrawNodeImagesEventArgs(GraphicsCache cache, Rectangle r, AppearanceObject appearance, TreeListNode node, int selectImageIndex, int stateImageIndex, Rectangle selectRect, Rectangle stateRect,
			Point selectImageLocation, Point stateImageLocation, bool isRightToLeft)
			: base(cache, r, appearance, node, null, isRightToLeft) {
			this.selectImageIndex = selectImageIndex;
			this.stateImageIndex = stateImageIndex;
			this.selectRect = selectRect;
			this.stateRect = stateRect;
			this.selectImageLocation = selectImageLocation;
			this.stateImageLocation = stateImageLocation;
			this.fillBackground = false;
		}
		TreeList TreeList { get { return Node.TreeList; } }
		internal bool FillBackground { get { return fillBackground; } set { fillBackground = value; } }
		public Rectangle SelectRect { get { return selectRect; } }
		public Rectangle StateRect { get { return stateRect; } }
		public Point SelectImageLocation { get { return selectImageLocation; } }
		public Point StateImageLocation { get { return stateImageLocation; } }
		public int SelectImageIndex {
			get { return selectImageIndex; }
			set {
				if(TreeList.SelectImageList == null) return;
				if(!ImageCollection.IsImageListImageExists(TreeList.SelectImageList, value)) value = -1;
				selectImageIndex = value;
			}
		}
		public int StateImageIndex {
			get { return stateImageIndex; }
			set {
				if(TreeList.StateImageList == null) return;
				if(!ImageCollection.IsImageListImageExists(TreeList.StateImageList, value)) value = -1;
				stateImageIndex = value;
			}
		}
	}
	public class CustomDrawNodeButtonEventArgs : CustomDrawNodeEventArgs {
		public CustomDrawNodeButtonEventArgs(GraphicsCache cache, Rectangle r, AppearanceObject appearance, TreeListNode node, ObjectPainter painter) : this(cache, r, appearance, node, painter, false) { }
		public CustomDrawNodeButtonEventArgs(GraphicsCache cache, Rectangle r, AppearanceObject appearance, TreeListNode node, ObjectPainter painter, bool isRightToLeft)
			: base(cache, r, appearance, node, painter, isRightToLeft) {
			((OpenCloseButtonInfoArgs)fObjectArgs).Opened = Expanded;
		}
		protected override StyleObjectInfoArgs CreateObjectArgs(GraphicsCache cache, Rectangle r, AppearanceObject appearance, TreeListNode node) {
			return new OpenCloseButtonInfoArgs(cache, r, false, appearance, ObjectState.Normal);
		}
		public bool Expanded { get { return Node.Expanded; } }
	}
	public class CustomDrawNodeCheckBoxEventArgs : CustomDrawNodeEventArgs {
		public CustomDrawNodeCheckBoxEventArgs(GraphicsCache cache, Rectangle r, AppearanceObject appearance, TreeListNode node, ObjectPainter painter) : this(cache, r, appearance, node, painter, false) { }
		public CustomDrawNodeCheckBoxEventArgs(GraphicsCache cache, Rectangle r, AppearanceObject appearance, TreeListNode node, ObjectPainter painter, bool isRightToLeft) : base(cache, r, appearance, node, painter, isRightToLeft) { }
		protected override StyleObjectInfoArgs CreateObjectArgs(GraphicsCache cache, Rectangle r, AppearanceObject appearance, TreeListNode node) {
			CheckObjectInfoArgs args = new CheckObjectInfoArgs(cache, appearance);
			args.GlyphAlignment = HorzAlignment.Center;
			args.Bounds = r;
			return args;
		}
	}
	public class CustomDrawEmptyAreaEventArgs : CustomDrawEventArgs {
		Rectangle behindColumn, emptyRows;
		Region emptyAreaRegion;
		public CustomDrawEmptyAreaEventArgs(GraphicsCache cache, Rectangle r, AppearanceObject appearance, Rectangle emptyRows, Rectangle bc, Region emptyAreaRegion) : this(cache, r, appearance, emptyRows, bc, emptyAreaRegion, false) { }
		public CustomDrawEmptyAreaEventArgs(GraphicsCache cache, Rectangle r, AppearanceObject appearance, Rectangle emptyRows, Rectangle bc, Region emptyAreaRegion, bool isRightToLeft)
			: base(cache, r, appearance, null, isRightToLeft) {
			this.behindColumn = bc;
			this.emptyRows = emptyRows;
			this.emptyAreaRegion = emptyAreaRegion;
		}
		public Rectangle BehindColumn { get { return behindColumn; } }
		public Rectangle EmptyRows { get { return emptyRows; } }
		public Region EmptyAreaRegion { get { return emptyAreaRegion; } }
	}
	public class CustomDrawNodeIndentEventArgs : CustomDrawNodeEventArgs {
		bool isNodeIndent;
		public CustomDrawNodeIndentEventArgs(GraphicsCache cache, Rectangle r, AppearanceObject appearance, ObjectPainter painter, TreeListNode node, bool isNodeIndent) : this(cache, r, appearance, painter, node, isNodeIndent, false) { }
		public CustomDrawNodeIndentEventArgs(GraphicsCache cache, Rectangle r, AppearanceObject appearance, ObjectPainter painter, TreeListNode node, bool isNodeIndent, bool isRightToLeft)
			: base(cache, r, appearance, node, painter, isRightToLeft) {
			this.isNodeIndent = isNodeIndent;
		}
		public bool IsNodeIndent { get { return isNodeIndent; } }
	}
	public class CustomDrawObjectEventArgs : CustomDrawEventArgs {
		ObjectPainter painter;
		ObjectInfoArgs info;
		public CustomDrawObjectEventArgs(GraphicsCache cache, ObjectPainter painter, ObjectInfoArgs info, AppearanceObject appearance) : this(cache, painter, info, appearance, false) { }
		public CustomDrawObjectEventArgs(GraphicsCache cache, ObjectPainter painter, ObjectInfoArgs info, AppearanceObject appearance, bool isRightToLeft)
			: base(cache, info.Bounds, appearance, null, isRightToLeft) {
			this.info = info;
			this.painter = painter;
		}
		public ObjectInfoArgs Info { get { return info; } }
	}
	public class TreeListMenuItemClickEventArgs : EventArgs {
		TreeListColumn column;
		bool isFooter;
		SummaryItemType type;
		DevExpress.XtraTreeList.Menu.TreeListMenuType mtype;
		DevExpress.Utils.Menu.DXMenuItem menuItem;
		string format;
		bool handled;
		public TreeListMenuItemClickEventArgs(TreeListColumn column, bool isFooter, SummaryItemType type, string format, DevExpress.XtraTreeList.Menu.TreeListMenuType mtype, DevExpress.Utils.Menu.DXMenuItem menuItem) {
			this.column = column;
			this.isFooter = isFooter;
			this.SummaryType = type;
			this.SummaryFormat = format;
			this.mtype = mtype;
			this.menuItem = menuItem;
			handled = false;
		}
		public bool IsFooter { get { return isFooter; } }
		public TreeListColumn Column { get { return column; } }
		public DevExpress.XtraTreeList.Menu.TreeListMenuType MenuType { get { return mtype; } }
		public DevExpress.Utils.Menu.DXMenuItem MenuItem { get { return menuItem; } }
		public SummaryItemType SummaryType {
			get { return type; }
			set { type = value; }
		}
		public string SummaryFormat {
			get { return format; }
			set { format = value; }
		}
		public bool Handled {
			get { return handled; }
			set { handled = value; }
		}
	}
	public class PopupMenuShowingEventArgs : EventArgs {
		DevExpress.XtraTreeList.Menu.TreeListMenu menu;
		Point point;
		bool allow;
		public PopupMenuShowingEventArgs(DevExpress.XtraTreeList.Menu.TreeListMenu menu, Point point, bool allow) {
			this.menu = menu;
			this.point = point;
			this.allow = allow;
		}
		public bool Allow {
			get { return allow; }
			set { allow = value; }
		}
		public DevExpress.XtraTreeList.Menu.TreeListMenu Menu {
			get { return menu; }
			set { menu = value; }
		}
		public Point Point {
			get { return point; }
			set { point = value; }
		}
	}
	[Obsolete("You should use the 'PopupMenuShowingEventArgs' instead", false), EditorBrowsable(EditorBrowsableState.Never)]
	public class TreeListMenuEventArgs : PopupMenuShowingEventArgs {
		public TreeListMenuEventArgs(DevExpress.XtraTreeList.Menu.TreeListMenu menu, Point point, bool allow)
			: base(menu, point, allow) {
		}
	}
	public class NodeClickEventArgs : NodeEventArgs {
		Point point;
		public NodeClickEventArgs(TreeListNode node, Point point)
			: base(node) {
			this.point = point;
		}
		public Point Point { get { return point; } }
	}
	public class CalcNodeHeightEventArgs : NodeEventArgs {
		int nodeHeight;
		public CalcNodeHeightEventArgs(TreeListNode node, int nodeHeight)
			: base(node) {
			this.nodeHeight = nodeHeight;
		}
		public int NodeHeight {
			get { return nodeHeight; }
			set {
				if(Node.TreeList != null) value = Node.TreeList.CheckRowHeight(value);
				nodeHeight = value;
			}
		}
	}
	public class NodePreviewHeightEventArgs : NodeEventArgs {
		int height;
		public NodePreviewHeightEventArgs(TreeListNode node)
			: base(node) {
			this.height = -1;
		}
		public int PreviewHeight {
			get { return height; }
			set {
				if(value < -1) value = -1;
				height = value;
			}
		}
	}
	public class CalcNodeDragImageIndexEventArgs : NodeEventArgs {
		int imageIndex;
		Point ptClient;
		DragEventArgs dragArgs;
		public CalcNodeDragImageIndexEventArgs(TreeListNode overNode, int imageIndex, Point ptClient, DragEventArgs dragArgs)
			: base(overNode) {
			this.imageIndex = imageIndex;
			this.ptClient = ptClient;
			this.dragArgs = dragArgs;
		}
		public Point PtClient { get { return ptClient; } }
		public DragEventArgs DragArgs { get { return dragArgs; } }
		public int ImageIndex {
			get { return imageIndex; }
			set { imageIndex = value; }
		}
	}
	public class CreateCustomNodeEventArgs : EventArgs {
		TreeListNode node;
		int nodeID;
		TreeListNodes owner;
		object tag;
		public CreateCustomNodeEventArgs(int nodeID, TreeListNodes owner, object tag) {
			this.nodeID = nodeID;
			this.owner = owner;
			this.tag = tag;
		}
		public int NodeID { get { return nodeID; } }
		public TreeListNodes Owner { get { return owner; } }
		public TreeListNode Node { get { return node; } set { node = value; } }
		public object Tag { get { return tag; } }
	}
	public class VirtualTreeGetChildNodesInfo : EventArgs {
		IList children;
		object node;
		public VirtualTreeGetChildNodesInfo(object node) {
			this.node = node;
		}
		public IList Children {
			get {
				return children;
			}
			set {
				children = value;
			}
		}
		public object Node {
			get {
				return node;
			}
		}
	}
	public class VirtualTreeSetCellValueInfo : EventArgs {
		object node;
		object newCellData;
		object oldCellData;
		TreeListColumn column;
		bool cancel;
		public VirtualTreeSetCellValueInfo(object oldCellData, object newCellData, object node, TreeListColumn column) {
			this.oldCellData = oldCellData;
			this.newCellData = newCellData;
			this.node = node;
			this.column = column;
			this.cancel = false;
		}
		public object Node { get { return node; } }
		public TreeListColumn Column { get { return column; } }
		public object OldCellData { get { return oldCellData; } }
		public object NewCellData { get { return newCellData; } }
		public bool Cancel {
			get { return cancel; }
			set { cancel = value; }
		}
	}
	public class VirtualTreeGetCellValueInfo : EventArgs {
		object cellData;
		object node;
		TreeListColumn column;
		public VirtualTreeGetCellValueInfo(object node, TreeListColumn column) {
			this.node = node;
			this.column = column;
		}
		public object Node {
			get {
				return node;
			}
		}
		public object CellData {
			get {
				return cellData;
			}
			set {
				if(cellData != value)
					cellData = value;
			}
		}
		public TreeListColumn Column { get { return column; } }
	}
	public class FilterNodeEventArgs : EventArgs {
		TreeListNode node;
		bool handled = false;
		bool isFit;
		public FilterNodeEventArgs(TreeListNode node, bool isFit) {
			this.node = node;
			this.isFit = isFit;
		}
		public TreeListNode Node { get { return node; } }
		public bool IsFitDefaultFilter { get { return isFit; } }
		public bool Handled {
			get { return handled; }
			set { handled = value; }
		}
	}
	public class DragObjectStartEventArgs : EventArgs {
		object dragObject;
		bool allow;
		public DragObjectStartEventArgs(object dragObject) {
			this.dragObject = dragObject;
			this.allow = true;
		}
		public object DragObject { get { return dragObject; } }
		public bool Allow {
			get { return allow; }
			set { allow = value; }
		}
	}
	public class DragObjectOverEventArgs : EventArgs {
		object dragObject;
		PositionInfo dropInfo;
		public DragObjectOverEventArgs(object dragObject, PositionInfo dropInfo) {
			this.dragObject = dragObject;
			this.dropInfo = dropInfo;
		}
		public object DragObject { get { return dragObject; } }
		public PositionInfo DropInfo { get { return dropInfo; } }
	}
	public class DragObjectDropEventArgs : EventArgs {
		object dragObject;
		PositionInfo dropInfo;
		public DragObjectDropEventArgs(object dragObject, PositionInfo dropInfo) {
			this.dragObject = dragObject;
			this.dropInfo = dropInfo;
		}
		public object DragObject { get { return dragObject; } }
		public PositionInfo DropInfo { get { return dropInfo; } }
		public bool Canceled { get { return !DropInfo.Valid; } }
	}
	public class CheckNodeEventArgs : NodeEventArgs {
		bool canCheck;
		CheckState prevState, state;
		public CheckNodeEventArgs(TreeListNode node, CheckState prevState, CheckState state)
			: base(node) {
			this.canCheck = true;
			this.prevState = prevState;
			this.state = state;
		}
		public bool CanCheck { get { return canCheck; } set { canCheck = value; } }
		public CheckState PrevState { get { return prevState; } }
		public CheckState State { get { return state; } set { state = value; } }
	}
	public class PositionInfo {
		protected bool fValid;
		protected int fIndex, fRowIndex, fColumnIndex;
		protected Rectangle fBounds;
		protected TreeListBandCollection fTargetCollection;
		protected bool isHorizontalCore;
		protected TreeListBand fBand;
		public PositionInfo() {
			Assign(-1, Rectangle.Empty, false);
		}
		public static PositionInfo Empty { get { return new PositionInfo(); } }
		public virtual bool IsEquals(PositionInfo pi) {
			return this.Valid == pi.Valid && this.Bounds == pi.Bounds && this.Index == pi.Index && this.TargetCollection == pi.TargetCollection && this.RowIndex == pi.fRowIndex && this.ColumnIndex == pi.ColumnIndex;
		}
		public virtual bool Valid { get { return fValid; } set { fValid = value; } }
		public Rectangle Bounds { get { return fBounds; } set { fBounds = value; } }
		public int Index { get { return fIndex; } }
		public int RowIndex { get { return fRowIndex; } }
		public int ColumnIndex { get { return fColumnIndex; } }
		public TreeListBandCollection TargetCollection { get { return fTargetCollection; } }
		public TreeListBand Band { get { return fBand; } }
		protected internal void SetTargetCollection(TreeListBandCollection targetCollection) { this.fTargetCollection = targetCollection; }
		protected internal void SetBand(TreeListBand band) { this.fBand = band; }
		protected internal void SetIndex(int newIndex) { this.fIndex = newIndex; }
		protected internal void SetRowAndColumnIndex(int rowIndex, int columnIndex) {
			this.fRowIndex = rowIndex;
			this.fColumnIndex = columnIndex;
		}
		public bool IsHorizontal {
			get { return isHorizontalCore; }
			internal set { isHorizontalCore = value; }
		}
		protected internal void Assign(int newIndex, Rectangle newBounds, bool valid, TreeListBandCollection targetCollectin = null, TreeListBand band = null) {
			this.fValid = valid;
			this.fBounds = newBounds;
			this.fIndex = newIndex;
			this.fTargetCollection = targetCollectin;
			this.fBand = band;
		}
		protected internal void Assign(PositionInfo pos) {
			this.fValid = pos.fValid;
			this.fBounds = pos.fBounds;
			this.fIndex = pos.fIndex;
			this.fTargetCollection = pos.fTargetCollection;
			this.isHorizontalCore = pos.isHorizontalCore;
			this.fBand = pos.Band;
			this.fRowIndex = pos.fRowIndex;
			this.fColumnIndex = pos.fColumnIndex;
		}
	}
	public class FilterControlEventArgs : EventArgs {
		IFilterEditorForm owner;
		bool show = true;
		public FilterControlEventArgs(IFilterEditorForm owner) {
			this.owner = owner;
		}
		public IFilterControl FilterControl { get { return owner.FilterControl; } }
		public bool ShowFilterEditor { get { return show; } set { show = value; } }
	}
	public class TreeListFilterButtonInfoArgs : GridFilterButtonInfoArgs {
	}
	public class FilterPopupListBoxEventArgs : EventArgs {
		TreeListColumn column;
		RepositoryItemComboBox comboBox;
		public FilterPopupListBoxEventArgs(TreeListColumn column, RepositoryItemComboBox comboBox) {
			this.column = column;
			this.comboBox = comboBox;
		}
		public TreeListColumn Column { get { return column; } }
		public RepositoryItemComboBox ComboBox { get { return comboBox; } }
	}
	public class FilterPopupCheckedListBoxEventArgs : EventArgs {
		TreeListColumn column;
		RepositoryItemCheckedComboBoxEdit comboBox;
		public FilterPopupCheckedListBoxEventArgs(TreeListColumn column, RepositoryItemCheckedComboBoxEdit comboBox) {
			this.column = column;
			this.comboBox = comboBox;
		}
		public TreeListColumn Column { get { return column; } }
		public RepositoryItemCheckedComboBoxEdit CheckedComboBox { get { return comboBox; } }
	}
	public class FilterPopupDateEventArgs : EventArgs {
		TreeListColumn column;
		List<FilterDateElement> list;
		public FilterPopupDateEventArgs(TreeListColumn column, List<FilterDateElement> list) {
			this.column = column;
			this.list = list;
		}
		public TreeListColumn Column { get { return column; } }
		public List<FilterDateElement> List { get { return list; } }
	}
	public class TreeListUnboundExpressionEditorEventArgs : EventArgs {
		public TreeListUnboundExpressionEditorEventArgs(ExpressionEditorForm form, TreeListColumn column) {
			ExpressionEditorForm = form;
			Column = column;
			ShowExpressionEditor = true;
		}
		public ExpressionEditorForm ExpressionEditorForm { get; private set; }
		public TreeListColumn Column { get; private set; }
		public bool ShowExpressionEditor { get; set; }
	}
	public static class DXDragEventArgsExtension {
		public static DXDragEventArgs GetDXDragEventArgs(this DragEventArgs e, TreeList treeList) {
			return DXDragEventArgs.GetEventArgs(treeList, e);
		}
		public static DXDragEventArgs GetDXDragEventArgs(this TreeList treeList, DragEventArgs e) {
			return DXDragEventArgs.GetEventArgs(treeList, e);
		}
	}
	public class DXDragEventArgs : DragEventArgs {
		#region static
		public static DXDragEventArgs GetEventArgs(TreeList treeList, DragEventArgs e) {
			return (e as DXDragEventArgs) ?? new DXDragEventArgs(treeList, e);
		}
		#endregion static
		DXDragEventArgs(TreeList treeList, DragEventArgs e) :
			base(e.Data, e.KeyState, e.X, e.Y, e.AllowedEffect, e.Effect) {
			OriginalEventArgs = e;
			TreeList = treeList;
			HitPoint = treeList.PointToClient(new Point(X, Y));
		}
		protected TreeList TreeList { get; private set; }
		protected DragEventArgs OriginalEventArgs { get; private set; }
		public Point HitPoint {
			get;
			private set;
		}
		TreeListHitInfo hitInfoCore;
		public TreeListHitInfo HitInfo {
			get {
				if(hitInfoCore == null)
					hitInfoCore = TreeList.CalcHitInfo(HitPoint);
				return hitInfoCore;
			}
		}
		public TreeListNode TargetNode {
			get { return HitInfo.HitTest.Node; }
		}
		public TreeListNode TargetRootNode {
			get { return (TargetNode != null) ? TargetNode.RootNode : null; }
		}
		TreeListNode nodeCore;
		public TreeListNode Node {
			get {
				if(nodeCore == null)
					nodeCore = Data.GetData(typeof(TreeListNode)) as TreeListNode;
				return nodeCore;
			}
		}
		public TreeListNode RootNode {
			get { return (Node != null) ? Node.RootNode : null; }
		}
		public DragInsertPosition DragInsertPosition {
			get { return TreeList.Handler.GetCurrentDragInsertPosition(); }
		}
		public new DragDropEffects Effect {
			get { return base.Effect; }
			set {
				base.Effect = value;
				OriginalEventArgs.Effect = value;
			}
		}
	}
	public class CustomDrawBandHeaderEventArgs : CustomDrawEventArgs {
		public CustomDrawBandHeaderEventArgs(GraphicsCache cache, HeaderObjectPainter painter, BandInfo info) : this(cache, painter, info, false) { }
		public CustomDrawBandHeaderEventArgs(GraphicsCache cache, HeaderObjectPainter painter, BandInfo info, bool isRightToLeft)
			: base(cache, info.Bounds, info.Appearance, painter, isRightToLeft) {
			fObjectArgs = info;
		}
		public TreeListBand Band { get { return Info.Band; } }
		protected BandInfo Info { get { return ObjectArgs as BandInfo; } }
	}
	public class BandEventArgs : EventArgs {
		public BandEventArgs(TreeListBand band) {
			Band = band;
		}
		public TreeListBand Band { get; private set; }
	}
	public class CustomizeNewNodeFromOuterDataEventArgs {
		public CustomizeNewNodeFromOuterDataEventArgs(TreeListNode sourceNode, TreeListNode destinationNode, Dictionary<string, object> newValue) {
			SourceNode = sourceNode;
			DestinationNode = destinationNode;
			NewData = newValue;
		}
		public TreeListNode SourceNode { get; private set; }
		public TreeListNode DestinationNode { get; private set; }
		public Dictionary<string, object> NewData { get; private set; }
		public bool Handled { get; set; }
	}
	public class BeforeDropNodeEventArgs {
		public BeforeDropNodeEventArgs(TreeListNode sourceNode, TreeListNode destinationNode, bool isCopy) {
			SourceNode = sourceNode;
			DestinationNode = destinationNode;
			IsCopy = isCopy;
		}
		public TreeListNode SourceNode { get; private set; }
		public TreeListNode DestinationNode { get; private set; }
		public bool IsCopy { get; private set; }
		public bool Cancel { get; set; }
	}
	public class AfterDropNodeEventArgs {
		public AfterDropNodeEventArgs(TreeListNode node, TreeListNode destinationNode, bool isSuccess) {
			Node = node;
			IsSuccess = isSuccess;
			DestinationNode = destinationNode;
		}
		public TreeListNode Node { get; private set; }
		public bool IsSuccess { get; private set; }
		public TreeListNode DestinationNode { get; private set; }
	}
}
