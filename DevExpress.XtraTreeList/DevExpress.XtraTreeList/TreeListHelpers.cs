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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Paint;
using DevExpress.Utils.Win;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.ViewInfo;
using DevExpress.XtraTreeList.Nodes;
using System.Collections;
using System.Collections.Generic;
namespace DevExpress.XtraTreeList.Helpers {
	public interface IDesignNotified {
		void OnSelectionChanged(ISelectionService selService);
		void OnMouseDown(MouseEventArgs e);
		void OnMouseMove(MouseEventArgs e);
		void OnMouseUp(MouseEventArgs e);
		void OnMouseEnter();
		void OnMouseLeave();
	}
	public class DesignHelper { 
		public static void FireChanged(Component component, bool loading, bool designMode, IComponentChangeService serv) {
			if(loading || !designMode) return;
			if(serv != null) {
				serv.OnComponentChanged(component, null, null, null);
			}
		}
	}
	public class TreeListBandDragHelper {
		TreeList treeList;
		readonly Size DefaultSize = new Size(200, 20);
		public TreeListBandDragHelper(TreeList treeList) {
			this.treeList = treeList;
		}
		public void DragBand(TreeListBand band, MouseEventArgs e) {
			this.treeList.Capture = true;
			this.treeList.PressedBand = band;
			this.treeList.Handler.StateData.DownHitTest = new TreeListHitTest() { BandInfo = GetBandInfo(band, new Rectangle(Point.Empty, DefaultSize)), HitInfoType = HitInfoType.Band };
			treeList.Handler.SetControlState(TreeListState.BandDragging);
		}
		protected BandInfo GetBandInfo(TreeListBand band, Rectangle bounds) {
			BandInfo bi = new BandInfo(band) { HeaderPosition = HeaderPositionKind.Special, Bounds = bounds };
			bi.SetAppearance(treeList.ViewInfo.PaintAppearance.BandPanel);
			treeList.ViewInfo.ElementPainters.BandPainter.CalcObjectBounds(bi);
			return bi;
		}
	}
}
namespace DevExpress.XtraTreeList.Painter {
	public interface ITreeListPaintHelper {
		void DrawOpenCloseButton(CustomDrawNodeButtonEventArgs e);
		void DrawColumn(CustomDrawColumnHeaderEventArgs e);
		void DrawNodePreview(CustomDrawNodePreviewEventArgs e);
		void DrawNodeSelectImage(CustomDrawNodeImagesEventArgs e);
		void DrawNodeStateImage(CustomDrawNodeImagesEventArgs e);
		void DrawIndicator(CustomDrawNodeIndicatorEventArgs e);
		void DrawFooterBackGround(CustomDrawEventArgs e);
		void DrawRowFooterCell(CustomDrawRowFooterCellEventArgs e);
		void DrawFooterCell(CustomDrawFooterCellEventArgs e);
		void DrawNodeCell(CustomDrawNodeCellEventArgs e);
		void DrawNodeIndent(CustomDrawNodeIndentEventArgs e);
		void DrawEmptyArea(CustomDrawEmptyAreaEventArgs e);
		void DrawNodeCheckBox(CustomDrawNodeCheckBoxEventArgs e);
		void DrawBand(CustomDrawBandHeaderEventArgs e);
	}
	public class TreeListPaintHelper : ITreeListPaintHelper {
		public TreeListPaintHelper() {
		}
		public virtual void DrawFooterBackGround(CustomDrawEventArgs e) {
			e.Painter.DrawObject(e.ObjectArgs);
		}
		public virtual void DrawOpenCloseButton(CustomDrawNodeButtonEventArgs e) {
			e.Painter.DrawObject(e.ObjectArgs);
		}
		public virtual void DrawIndicator(CustomDrawNodeIndicatorEventArgs e) {
			e.Painter.DrawObject(e.ObjectArgs);
		}
		public virtual void DrawColumn(CustomDrawColumnHeaderEventArgs e) {
			e.Painter.DrawObject(e.ObjectArgs);
		}
		public virtual void DrawBand(CustomDrawBandHeaderEventArgs e) {
			e.Painter.DrawObject(e.ObjectArgs);
		}
		public virtual void DrawNodePreview(CustomDrawNodePreviewEventArgs e) {
			e.Appearance.FillRectangle(e.Cache, e.Bounds);
			Rectangle pr = Rectangle.Inflate(e.Bounds, -2, 0);
			e.Appearance.DrawString(e.Cache, e.PreviewText, pr, e.Appearance.GetForeBrush(e.Cache), e.Appearance.GetStringFormat(TreeListViewInfo.GetPreviewTextOptions()));
		}
		public virtual void DrawNodeSelectImage(CustomDrawNodeImagesEventArgs e) {
			DrawNodeImageCore(e, e.SelectRect, e.SelectImageLocation, e.Node.TreeList.SelectImageList, e.SelectImageIndex);
		}
		public virtual void DrawNodeStateImage(CustomDrawNodeImagesEventArgs e) {
			DrawNodeImageCore(e, e.StateRect, e.StateImageLocation, e.Node.TreeList.StateImageList, e.StateImageIndex);
		}
		protected virtual void DrawNodeImageCore(CustomDrawNodeImagesEventArgs e, Rectangle bounds, Point location, object imageList, int imageIndex) {
			if(e.FillBackground)
				e.Appearance.FillRectangle(e.Cache, bounds);
			if(ImageCollection.IsImageListImageExists(imageList, imageIndex)) {
				System.Drawing.Imaging.ImageAttributes attr = null;
				if(e.Node != null && e.Node.TreeList != null && e.Node.TreeList.OptionsView.AllowGlyphSkinning) {
					Color color = e.Node.TreeList.ViewInfo.PaintAppearance.Row.ForeColor;
					if(color == e.Appearance.BackColor) 
						color = DevExpress.LookAndFeel.LookAndFeelHelper.GetSystemColor(e.Node.TreeList.ElementsLookAndFeel, SystemColors.ControlText);
					attr = ImageColorizer.GetColoredAttributes(color);
				}
				ImageCollection.DrawImageListImage(e.Cache, imageList, imageIndex, new Rectangle(location, ImageCollection.GetImageListSize(imageList)), attr);
			}
		}
		public virtual void DrawRowFooterCell(CustomDrawRowFooterCellEventArgs e) {
			DrawFooterCell(e);
		}
		public virtual void DrawFooterCell(CustomDrawFooterCellEventArgs e) {
			if(e.ItemType != SummaryItemType.None) {
				e.Painter.DrawObject(e.ObjectArgs);
			}
		}
		public virtual void DrawNodeCell(CustomDrawNodeCellEventArgs e) {
			e.EditViewInfo.PaintAppearance = e.Appearance;
			e.EditViewInfo.SetDisplayText(e.CellText);
			e.EditViewInfo.MatchedString = string.Empty;
			if(e.Node.TreeList.State == TreeListState.IncrementalSearch && e.Column == e.Node.TreeList.FocusedColumn && e.Node == e.Node.TreeList.FocusedNode && e.Node.TreeList.IncrementalText != "") {
				e.EditViewInfo.MatchedString = e.Node.TreeList.IncrementalText;
			}
			if(!TreeListAutoFilterNode.IsAutoFilterNode(e.Node)) {
				if(e.Node.TreeList.GetAllowHighlightFindResults(e.Column)) {
					e.EditViewInfo.UseHighlightSearchAppearance = true;
					e.EditViewInfo.MatchedStringUseContains = true;
					e.EditViewInfo.MatchedString = e.Node.TreeList.GetFindMatchedText(e.Column, e.EditViewInfo.DisplayText);
				}
				else if(e.Node.TreeList.IsLookUpMode && e.Node.TreeList.LookUpDisplayColumn == e.Column) {
					e.EditViewInfo.UseHighlightSearchAppearance = true;
					e.EditViewInfo.MatchedStringUseContains = true;
					e.EditViewInfo.MatchedString = e.Node.TreeList.ExtraFilterHightlightText;
				}
			}
			e.EditPainter.Draw(new ControlGraphicsInfoArgs(e.EditViewInfo, e.Cache, e.EditViewInfo.Bounds));
			if(e.Focused && e.Node.TreeList.OptionsView.FocusRectStyle == DrawFocusRectStyle.CellFocus)
				XPaint.Graphics.DrawFocusRectangle(e.Graphics, e.Bounds, e.Appearance.GetForeColor(), e.Appearance.GetBackColor());
		}
		public virtual void DrawNodeIndent(CustomDrawNodeIndentEventArgs e) {
			e.Appearance.FillRectangle(e.Cache, e.Bounds);
		}
		public virtual void DrawEmptyArea(CustomDrawEmptyAreaEventArgs e) {
			if(e.EmptyAreaRegion == null || e.Graphics == null || e.EmptyAreaRegion.IsEmpty(e.Graphics)) return;
			e.Graphics.FillRegion(e.Appearance.GetBackBrush(e.Cache, e.Bounds), e.EmptyAreaRegion);
		}
		public virtual void DrawNodeCheckBox(CustomDrawNodeCheckBoxEventArgs e) {
			e.Painter.CalcObjectBounds(e.ObjectArgs);
			e.Painter.DrawObject(e.ObjectArgs);
		}
	}
	public class PaintLink {
		private TreeList treeList;
		public PaintLink(TreeList treeList) {
			this.treeList = treeList;
		}
		public void DrawIndicator(CustomDrawNodeIndicatorEventArgs e) {
			treeList.RaiseCustomDrawNodeIndicator(e);
		}
		public void DrawColumnHeader(CustomDrawColumnHeaderEventArgs e) {
			treeList.RaiseCustomDrawColumnHeader(e);
		}
		public void DrawNodePreview(CustomDrawNodePreviewEventArgs e) {
			treeList.RaiseCustomDrawNodePreview(e);
		}
		public void DrawNodeImages(CustomDrawNodeImagesEventArgs e) {
			treeList.RaiseCustomDrawNodeImages(e);
		}
		public void DrawNodeButton(CustomDrawNodeButtonEventArgs e) {
			treeList.RaiseCustomDrawNodeButton(e);
		}
		public void DrawNodeCell(CustomDrawNodeCellEventArgs e) {
			treeList.RaiseCustomDrawNodeCell(e);
		}
		public void DrawFooter(CustomDrawEventArgs e) {
			treeList.RaiseCustomDrawFooter(e);
		}
		public void DrawRowFooter(CustomDrawRowFooterEventArgs e) {
			treeList.RaiseCustomDrawRowFooter(e);
		}
		public void DrawFooterCell(CustomDrawFooterCellEventArgs e) {
			treeList.RaiseCustomDrawFooterCell(e);
		}
		public void DrawRowFooterCell(CustomDrawRowFooterCellEventArgs e) {
			treeList.RaiseCustomDrawRowFooterCell(e);
		}
		public void DrawEmptyArea(CustomDrawEmptyAreaEventArgs e) {
			treeList.RaiseCustomDrawEmptyArea(e);
		}
		public void DrawNodeIndent(CustomDrawNodeIndentEventArgs e) {
			treeList.RaiseCustomDrawNodeIndent(e);
		}
		public void DrawNodeCheckBox(CustomDrawNodeCheckBoxEventArgs e) {
			treeList.RaiseCustomDrawNodeCheckBox(e);
		}
		public void DrawBand(CustomDrawBandHeaderEventArgs e) {
			treeList.RaiseCustomDrawBandHeader(e);
		}
		public void DefaultPaintHelperChanged() {
			treeList.InternalDefaultPaintHelperChanged();
		}
		public BaseEditPainter GetPainter(RepositoryItem item) {
			if(item == null) return null;
			return treeList.ContainerHelper.GetPainter(item);
		}
		public bool IsColumnSelected(ColumnInfo colInfo) {
			if(colInfo == null || colInfo.Type != ColumnInfo.ColumnInfoType.Column) return false;
			return treeList.IsComponentSelected(colInfo.Column);
		}
		public bool IsBandSelected(BandInfo bi) {
			if(bi == null || bi.Type != ColumnInfo.ColumnInfoType.Column) return false;
			return treeList.IsComponentSelected(bi.Band);
		}
		public TreeList TreeList { get { return treeList; } }
		public DevExpress.LookAndFeel.UserLookAndFeel LookAndFeel{ get { return treeList.ElementsLookAndFeel; } }
		public DevExpress.XtraEditors.Controls.BorderStyles BorderStyle { get { return treeList.IsLookUpMode ? DevExpress.XtraEditors.Controls.BorderStyles.NoBorder : treeList.BorderStyle; } }
		public bool CanShowFocusRect { get { return treeList.HasFocus && treeList.OptionsView.FocusRectStyle == DrawFocusRectStyle.CellFocus && !treeList.IsLookUpMode; } }
		public ImageCollection IndicatorImages { get { return treeList.Painter.IndicatorImages; } }
	}
}
namespace DevExpress.XtraTreeList.Data {
	using System.Windows.Forms;
	using DevExpress.Data.Filtering.Helpers;
	using System.Collections.Generic;
	using DevExpress.XtraTreeList.StyleFormatConditions;
	using DevExpress.XtraGrid;
	using DevExpress.Data.Filtering;
using DevExpress.Data;
	using System.Collections;
	public class TreeListDataHelper {
		TreeList treeList;
		bool posting;
		public TreeListDataHelper(TreeList treeList) {
			this.treeList = treeList;
			this.posting = false;
		}
		public TreeListNode CreateNode(int nodeID, TreeListNode parentNode) {
			return treeList.InternalCreateNode(nodeID, parentNode, null);
		}
		public TreeListNode CreateNode(int nodeID, TreeListNode parentNode, object tag) {
			return treeList.InternalCreateNode(nodeID, parentNode, tag);
		}
		public void MoveNode(TreeListNode sourceNode, TreeListNode destinationNode) {
			treeList.MoveNodeCore(sourceNode, destinationNode, false);
		}
		public void DeleteNode(TreeListNode node) {
			treeList.InternalDeleteNode(node, false);
		}
		public void DeleteNode(TreeListNode node, bool modifySource) {
			treeList.InternalDeleteNode(node, modifySource);
		}
		public void ClearNodes() {
			treeList.ClearNodes();
		}
		public TreeListColumn GetColumn(int index) {
			if(index > -1 && index < treeList.Columns.Count)
				return treeList.Columns[index];
			return null;
		}
		public int GetIndexByColumnID(object columnID) {
			TreeListColumn col = GetTreeListColumnByID(columnID);
			if(col != null) return col.AbsoluteIndex;
			return -1;
		}
		public string GetColumnNameByColumnID(object columnID) {
			if(columnID is string) return (string)columnID; 
			TreeListColumn col = GetTreeListColumnByID(columnID);
			if(col != null) return col.FieldName;
			return string.Empty;
		}
		public string GetDisplayText(object columnID, TreeListNode node) {
			TreeListColumn col = GetTreeListColumnByID(columnID);
			if(col == null) return null;
			DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo tempEditorViewInfo = CellInfo.GetEditorViewInfo(col, node);
			return tempEditorViewInfo.DisplayText;
		}
		public TreeListColumn GetTreeListColumnByID(object columnID) {
			if(columnID == null) return null;
			if(columnID is TreeListColumn) return (TreeListColumn)columnID;
			if(columnID is int) return treeList.Columns[(int)columnID];
			if(columnID is string) return treeList.Columns[(string)columnID];
			if(columnID is DataColumn) return treeList.Columns[((DataColumn)columnID).ColumnName];
			return null;
		}
		public void InvalidValueException(Exception e, object value) {
			if(posting) throw e;
			treeList.ContainerHelper.OnInvalidValueException(treeList, e, value);
		}
		public void DataSourceDisposed() {
			treeList.DataSource = null;
			treeList.DataMember = string.Empty;
		}
		public void CheckCurrencyManagerPosition(int nodeID) {
			treeList.CheckCurrencyManagerPosition(nodeID == FocusedNodeId, nodeID);
		}
		int FocusedNodeId {
			get {
				if(treeList.FocusedNode == null) return -1;
				return treeList.FocusedNode.Id;
			}
		}
		public void ListChanged(object sender, ListChangedEventArgs e) {
			if(treeList.InvokeRequired)
				treeList.BeginInvoke(new ListChangedEventHandler(treeList.OnDataListChanged), new object[] { sender, e });
			else
				treeList.OnDataListChanged(sender, e);
		}
		public object GetUnboundData(int nodeId, string column, object value) {
			TreeListCustomColumnDataEventArgs e = new TreeListCustomColumnDataEventArgs(GetTreeListColumnByID(column), nodeId, value, true);
			treeList.RaiseCustomUnboundColumnData(e);
			return e.Value;
		}
		public void SetUnboundData(int nodeId, string column, object value) {
			TreeListCustomColumnDataEventArgs e  = new TreeListCustomColumnDataEventArgs(GetTreeListColumnByID(column), nodeId, value, false);
			treeList.RaiseCustomUnboundColumnData(e);
		}
		public TreeListColumnCollection Columns { get { return treeList.Columns; } }
		public bool Disposing { get { return treeList.TreeListDisposing; } }
		public bool Posting { get { return posting; } set { posting = value; } }
		public string KeyFieldName { get { return treeList.KeyFieldName; } }
		public string ParentFieldName { get { return treeList.ParentFieldName; } }
		public object RootValue { get { return treeList.RootValue; } }
		public bool IsDesignMode { get { return !treeList.IsDesignMode; } }
		public void SetNodeIndex(TreeListNode treeListNode, int i) {
			treeList.SetNodeIndexCore(treeListNode, i);
		}
		public ExpressionEvaluator CreateExpressionEvaluator(CriteriaOperator criteriaOperator, out Exception e) {
			e = null;
			try {
				ExpressionEvaluator evaluator = new ExpressionEvaluator(GetDescriptors(), criteriaOperator, false);
				evaluator.DataAccess = treeList;
				return evaluator;
			}
			catch(Exception ex) {
				e = ex;
				return null;
			}
		}
		protected virtual PropertyDescriptorCollection GetDescriptors() {
			List<DataColumnInfo> columns = new List<DataColumnInfo>();
			for(int i = 0; i < treeList.Data.Columns.Count; i++) {
				DataColumnInfo col = treeList.Data.Columns[i];
				if(!string.IsNullOrEmpty(col.ColumnName))
					columns.Add(col);
			}
			List<PropertyDescriptor> properties = new List<PropertyDescriptor>();
			for(int n = 0; n < columns.Count; n++)
				properties.Add(CreatePropertyDesciptor(columns[n]));
			if(treeList.IsFindFilterActive) {
				for(int n = 0; n < columns.Count; n++)
					properties.Add(CreateFindPropertyDescriptor(columns[n]));
			}
			return new PropertyDescriptorCollection(properties.ToArray());
		}
		PropertyDescriptor CreateFindPropertyDescriptor(DataColumnInfo column) {
			return new TreeFindFilterDisplayPropertyDescriptor(column);
		}
		PropertyDescriptor CreatePropertyDesciptor(DataColumnInfo column) {
			TreeListColumn treeListColumn = treeList.Columns[column.ColumnName];
			if(treeListColumn != null && treeList.GetColumnFilterMode(treeListColumn) == ColumnFilterMode.DisplayText)
				return new TreeDisplayPropertyDescriptor(column);
			return new TreePropertyDescriptor(column);
		}
	}
	public class TreeListUnboundColumnWrapper : IDataColumnInfo {
		TreeListColumn column;
		public TreeListUnboundColumnWrapper(TreeListColumn column) {
			this.column = column;
		}
		protected TreeList TreeList { get { return column.TreeList; } }
		#region IDataColumnInfo Members
		string IDataColumnInfo.Caption { get { return column.GetTextCaption(); } }
		List<IDataColumnInfo> IDataColumnInfo.Columns {
			get {
				List<IDataColumnInfo> res = new List<IDataColumnInfo>();
				if(TreeList == null) return res;
				foreach(TreeListColumn col in TreeList.Columns) {
					if(col == this.column) continue;
					if(!col.OptionsColumn.ShowInExpressionEditor) continue;
					res.Add(new TreeListUnboundColumnWrapper(col));
				}
				return res;
			}
		}
		DataControllerBase IDataColumnInfo.Controller { get { return null; } }
		string IDataColumnInfo.FieldName { get { return column.FieldName; } }
		Type IDataColumnInfo.FieldType { get { return column.ColumnType; } }
		string IDataColumnInfo.Name { get { return column.Name; } }
		string IDataColumnInfo.UnboundExpression { get { return column.UnboundExpression; } }
		#endregion
	}
}
namespace DevExpress.XtraTreeList.Helpers {
	public class HeaderWidthCalculator {
		public HeaderWidthInfoCollection HeaderWidths { get; private set; }
		public HeaderWidthCalculator() {
			this.HeaderWidths = new HeaderWidthInfoCollection(null);
		}
		public void Calculate(bool autoWidth, int totalWidth, int startIndex, IList headerObjects, Func<IHeaderObject, ICollection> getBandColumnRows = null) {
			HeaderWidths.Clear();
			CreateHeaderWidths(null, headerObjects, startIndex, false, getBandColumnRows);
			HeaderWidthInfoCollection lastChildren = GetLastChildren(HeaderWidths);
			UpdateWidths(lastChildren);
			CalculateCore(HeaderWidths, totalWidth, startIndex, autoWidth);
			UpdateWidths(lastChildren);
			UpdateHeaderActualWidths(HeaderWidths);
		}
		public void CalculateBestFit(bool autoWidth, int totalWidth, int startIndex, IList headerObjects, Func<IHeaderObject, int> calculateBestFit) {
			HeaderWidths.Clear();
			for(int i = startIndex; i < headerObjects.Count; i++) {
				IHeaderObject headerObject = headerObjects[i] as IHeaderObject;
				if(headerObject == null || !headerObject.Visible) continue;
				HeaderWidthInfo info = new HeaderWidthInfo(headerObject, false);
				info.Width = Math.Max(info.MinWidth, calculateBestFit(headerObject));
				HeaderWidths.Add(info);
			}
			CalculateCore(HeaderWidths, totalWidth, startIndex, autoWidth);
			UpdateHeaderActualWidths(HeaderWidths, false);
		}
		public void ResizeHeader(IHeaderObject headerObject, IList headerObjects, int delta, int maxPossibleWidth, bool autoWidth, Func<IHeaderObject, TreeListBandRowCollection> getBandColumnRows = null) {
			HeaderWidths.Clear();
			CreateHeaderWidths(null, headerObjects, 0, true, getBandColumnRows);
			HeaderWidthInfo hi = HeaderWidths.FindObject(headerObject);
			if(hi == null) return;
			HeaderWidthInfoCollection info = null;
			if(hi.Parent is MultiRowHeaderWidthInfo) {
				MultiRowHeaderWidthInfo mRowInfo = (MultiRowHeaderWidthInfo)hi.Parent;
				foreach(MultiRowHeaderWidthInfoRow row in mRowInfo.Rows) {
					if(row.Headers.FindObject(headerObject) != null) {
						info = row.Headers;
						break;
					}
				}
			}
			else {
				info = hi.Parent == null ? HeaderWidths : hi.Parent.Children;
			}
			while(info.IndexOf(hi) == info.Count - 1 && hi.Parent != null) {
				hi = hi.Parent;
				info = hi.Parent == null ? HeaderWidths : hi.Parent.Children;
			}
			int fromIndex = info.IndexOf(hi);
			if(hi.Width + delta < hi.MinWidth)
				delta = hi.MinWidth - hi.Width;
			fromIndex = fromIndex + 1;
			if(autoWidth) {
				bool fixedRight = (GetFixedColumnsCountRight(fromIndex, info) == info.Count - fromIndex) && hi.FixedWidth;
				if(delta > 0) {
					int sumMinWidthes = GetMinColumnsWidthRight(fromIndex, info, fixedRight);
					delta = Math.Min(delta, maxPossibleWidth - sumMinWidthes);
				}
				else if(delta < 0) {
					if(!hi.FixedWidth && GetFixedColumnsCountRight(fromIndex, info) == info.Count - fromIndex)
						delta = 0;
				}
				if(fixedRight)
					fromIndex = 0;
			}
			hi.Width += delta;
			info.UpdateTotalWidths();
			if(autoWidth)
				ResizeHeaderObjectsAutoWidth(fromIndex, -delta, info, 0, GetFixedColumnsCountRight(fromIndex, info), info.Count);
			else
				ResizeHeaderObjectsNonAutoWidth(fromIndex, hi, delta);
			if(autoWidth) {
				foreach(HeaderWidthInfo item in info) {
					if(item.HasChildren)
						CalculateCore(item.Children, item.Width, 0, autoWidth);
				}
				UpdateHeaderActualWidths(info, false);
				UpdateHeaderActualWidths(HeaderWidths, false);
			}
		}
		protected int GetMinColumnsWidthRight(int fromIndex, HeaderWidthInfoCollection info, bool ignoreFixedWidth) {
			int sum = 0;
			for(int i = fromIndex; i < info.Count; i++) {
				HeaderWidthInfo wi = info[i];
				if(wi.FixedWidth && !ignoreFixedWidth)
					sum += wi.Width;
				else
					sum += wi.MinWidth;
			}
			return sum;
		}
		protected virtual void CalculateCore(HeaderWidthInfoCollection infoCollection, int totalWidth, int fromIndex, bool autoWidth) {
			CalculateWidth(infoCollection, totalWidth, fromIndex, autoWidth);
			foreach(HeaderWidthInfo info in infoCollection) {
				if(info.HasChildren) {
					bool shouldUseAutoWidthForChildren = autoWidth;
					if(!shouldUseAutoWidthForChildren) {
						info.Children.UpdateTotalWidths();
						if(info.Children.TotalWidth < info.MinWidth) shouldUseAutoWidthForChildren = true;
					}
					CalculateCore(info.Children, info.Width, 0, shouldUseAutoWidthForChildren);
				}
				MultiRowHeaderWidthInfo mRowInfo = info as MultiRowHeaderWidthInfo;
				if(mRowInfo != null) {
					foreach(MultiRowHeaderWidthInfoRow row in mRowInfo.Rows) {
						CalculateCore(row.Headers, info.Width, 0, true);
					}
				}
			}
		}
		protected virtual void CalculateWidth(HeaderWidthInfoCollection info, int totalWidth, int fromIndex, bool autoWidth) {
			info.UpdateTotalWidths();
			if(autoWidth) {
				int cx = totalWidth - info.TotalWidth;
				ResizeHeaderObjectsAutoWidth(fromIndex, cx, info, 0, GetFixedColumnsCountRight(fromIndex, info), info.Count);
			}
		}
		protected virtual void ResizeHeaderObjectsNonAutoWidth(int fromIndex, HeaderWidthInfo hi, int delta) {
			if(hi.HasChildren) {
				UpdateHeaderActualWidths(HeaderWidths, false);
				CalculateCore(hi.Children, hi.Width, 0, true);
				UpdateHeaderActualWidths(hi.Children, false);
				UpdateWidths(hi.Children);
			}
			else {
				UpdateHeaderActualWidths(HeaderWidths, false);
				MultiRowHeaderWidthInfo mInfo = hi as MultiRowHeaderWidthInfo;
				if(mInfo == null) mInfo = hi.Parent as MultiRowHeaderWidthInfo;
				if(mInfo != null) {
					MultiRowHeaderWidthInfoRow row = mInfo.FindRow(hi.HeaderObject);
					if(row != null) {
						row.Headers.UpdateTotalWidths();
						ResizeHeaderObjectsAutoWidth(fromIndex, -delta, row.Headers, 0, GetFixedColumnsCountRight(0, row.Headers), row.Headers.Count);
					}
				}
				else {
					HeaderWidthInfoCollection info = GetLastChildren(HeaderWidths);
					UpdateWidths(info);
				}
				UpdateHeaderActualWidths(HeaderWidths, false);
			}
			UpdateHeaderActualWidths(HeaderWidths);
		}
		protected virtual void ResizeHeaderObjectsAutoWidth(int fromIndex, int delta, HeaderWidthInfoCollection widthInfo, int oldDx, int fixedCount, int visibleCount) {
			if(delta == 0) return;
			if(fromIndex < 0 || fromIndex > visibleCount - 1) return;
			int resizedCount = Math.Max(0, visibleCount - fromIndex - fixedCount);
			foreach(HeaderWidthInfo wi in widthInfo) {
				if(wi.Minimized)
					resizedCount--;
			}
			bool useFixedColumns = (resizedCount == 0);
			if(useFixedColumns) resizedCount = fixedCount;
			if(resizedCount == 0) return;
			int dx = delta / resizedCount;
			int nextPlus = delta - dx * resizedCount;
			if(dx == 0) { SlightResize(nextPlus, widthInfo); }
			else {
				for(int i = fromIndex; i < widthInfo.Count; i++) {
					HeaderWidthInfo wi = widthInfo[i];
					if(wi.FixedWidth && !useFixedColumns) continue;
					if(wi.Minimized) continue;
					if(!wi.Minimized && wi.Width + dx <= wi.MinWidth) {
						nextPlus += dx + (wi.Width - wi.MinWidth);
						wi.Width = wi.MinWidth;
						wi.Minimized = true;
					}
					else
						wi.Width += dx;
				}
				if(nextPlus != 0) {
					if(Math.Abs(nextPlus) > visibleCount - fromIndex)
						ResizeHeaderObjectsAutoWidth(fromIndex, nextPlus, widthInfo, dx, fixedCount, visibleCount); 
					else
						SlightResize(nextPlus, widthInfo);
				}
			}
		}
		protected int GetFixedColumnsCountRight(int fromIndex, HeaderWidthInfoCollection info) {
			int count = 0;
			for(int i = fromIndex; i < info.Count; i++)
				if(info[i].FixedWidth)
					count++;
			return count;
		}
		protected void SlightResize(int delta, HeaderWidthInfoCollection widthInfo) {
			if(delta == 0) return; 
			int n = delta;
			int one = delta / Math.Abs(delta);
			foreach(HeaderWidthInfo wi in widthInfo) {
				if(n == 0) break;
				if(wi.Width + one < wi.MinWidth) continue;
				if(wi.FixedWidth) continue;
				wi.Width += one;
				n -= one;
			}
			if(n != 0 && n != delta)
				SlightResize(n, widthInfo);
		}
		protected virtual void CreateHeaderWidths(HeaderWidthInfo parent, IList headerObjects, int startIndex, bool useVisibleWidth, Func<IHeaderObject, ICollection> getBandColumnRows = null) {
			List<IHeaderObject> parentObjects = new List<IHeaderObject>();
			for(int i = startIndex; i < headerObjects.Count; i++) {
				IHeaderObject headerObject = headerObjects[i] as IHeaderObject;
				if(headerObject == null || !headerObject.Visible) continue;
				if(headerObject.Parent != null && getBandColumnRows != null) {
					if(parentObjects.Contains(headerObject.Parent)) continue;
					parentObjects.Add(headerObject.Parent);
					HeaderWidths.Add(CreateMultiRowHeaderWidthInfo(headerObject.Parent, getBandColumnRows, useVisibleWidth));
				}
				else {
					HeaderWidthInfo info = new HeaderWidthInfo(headerObject, useVisibleWidth);
					if(parent == null)
						HeaderWidths.Add(info);
					else
						parent.Children.Add(info);
					if(headerObject.Children != null && headerObject.Children.Count > 0) {
						CreateHeaderWidths(info, headerObject.Children, 0, useVisibleWidth, getBandColumnRows);
					}
					else {
						if(headerObject.Columns != null && headerObject.Columns.Count > 0) {
							if(getBandColumnRows != null) {
								MultiRowHeaderWidthInfo mrInfo = CreateMultiRowHeaderWidthInfo(headerObject, getBandColumnRows, useVisibleWidth);
								if(mrInfo.Rows.Count > 0)
									info.Children.Add(mrInfo);
							}
							else {
								for(int n = 0; n < headerObject.Columns.Count; n++) {
									IHeaderObject column = headerObject.Columns[n] as IHeaderObject;
									if(column == null || !column.Visible) continue;
									info.Children.Add(new HeaderWidthInfo(column, useVisibleWidth));
								}
							}
						}
					}
				}
			}
		}
		protected MultiRowHeaderWidthInfo CreateMultiRowHeaderWidthInfo(IHeaderObject headerObject, Func<IHeaderObject, ICollection> getBandColumnRows, bool useVisibleWidth) {
			MultiRowHeaderWidthInfo mInfo = new MultiRowHeaderWidthInfo();
			ICollection rows = getBandColumnRows(headerObject);
			foreach(IBandRow row in rows) {
				if(row.Columns.Count == 0) continue;
				MultiRowHeaderWidthInfoRow mInfoRow = new MultiRowHeaderWidthInfoRow(mInfo);
				foreach(IHeaderObject column in row.Columns) {
					if(!column.Visible) continue;
					HeaderWidthInfo wi = new HeaderWidthInfo(column, useVisibleWidth);
					mInfoRow.Headers.Add(wi);
				}
				mInfo.Rows.Add(mInfoRow);
			}
			mInfo.UpdateTotalWidths();
			return mInfo;
		}
		protected void UpdateWidths(HeaderWidthInfoCollection lastChildren) {
			for(int n = 0; n < lastChildren.Count; n++) {
				HeaderWidthInfo info = lastChildren[n];
				if(info.Width < info.MinWidth)
					info.Width = info.MinWidth;
				if(info.Parent != null)
					UpdateWidths(info.Parent);
			}
		}
		protected void UpdateWidths(HeaderWidthInfo info) {
			if(!info.HasChildren) return;
			int minWidth = 0, width = 0;
			for(int n = 0; n < info.Children.Count; n++) {
				HeaderWidthInfo i = info.Children[n];
				width += i.Width;
				minWidth += i.MinWidth;
			}
			if(info.MinWidth < minWidth) info.MinWidth = minWidth;
			width = Math.Max(width, info.MinWidth);
			if(info.Width != width)
				info.Width = width;
			if(info.Parent != null)
				UpdateWidths(info.Parent.Children);
		}
		protected virtual HeaderWidthInfoCollection GetLastChildren(HeaderWidthInfoCollection root) {
			HeaderWidthInfoCollection result = new HeaderWidthInfoCollection(null) { LockUpdateParent = true };
			GetLastChildrenCore(root, result);
			return result;
		}
		protected void GetLastChildrenCore(HeaderWidthInfoCollection root, HeaderWidthInfoCollection result) {
			for(int n = 0; n < root.Count; n++) {
				HeaderWidthInfo info = root[n];
				if(!info.HasChildren)
					result.Add(info);
				else
					GetLastChildrenCore(info.Children, result);
			}
		}
		protected virtual void UpdateHeaderActualWidths(HeaderWidthInfoCollection headerObjects, bool onlyVisibleWidth = true) {
			for(int n = 0; n < headerObjects.Count; n++) {
				HeaderWidthInfo info = headerObjects[n];
				UpdateHeaderObjectActualWidth(info, onlyVisibleWidth);
				if(info.HasChildren) UpdateHeaderActualWidths(info.Children, onlyVisibleWidth);
				MultiRowHeaderWidthInfo mRowInfo = info as MultiRowHeaderWidthInfo;
				if(mRowInfo != null) {
					foreach(MultiRowHeaderWidthInfoRow row in mRowInfo.Rows) {
						UpdateHeaderActualWidths(row.Headers, onlyVisibleWidth);
					}
				}
			}
		}
		protected virtual void UpdateHeaderObjectActualWidth(HeaderWidthInfo info, bool onlyVisibleWidth) {
			if(info.HeaderObject == null) return;
			info.HeaderObject.SetWidth(info.Width, onlyVisibleWidth);
		}
	}
	public class HeaderWidthInfo {
		public IHeaderObject HeaderObject { get; private set; }
		public int Width { get; set; }
		public int MinWidth { get; set; }
		public bool FixedWidth { get; set; }
		public bool Minimized { get; set; }
		public HeaderWidthInfo Parent { get; protected internal set; }
		public HeaderWidthInfoCollection Children { get; private set; }
		public bool HasChildren { get { return Children.Count > 0; } }
		public HeaderWidthInfo(IHeaderObject headerObject, bool useVisibleWidth) {
			this.HeaderObject = headerObject;
			if(HeaderObject != null) {
				this.MinWidth = HeaderObject.MinWidth;
				this.Width = Math.Max(this.MinWidth, useVisibleWidth ? HeaderObject.VisibleWidth : HeaderObject.Width);
				this.FixedWidth = HeaderObject.FixedWidth;
			}
			this.Children = new HeaderWidthInfoCollection(this);
		}
	}
	public class MultiRowHeaderWidthInfo : HeaderWidthInfo {
		public MultiRowHeaderWidthInfo()
			: base(null, false) {
			Rows = new List<MultiRowHeaderWidthInfoRow>();
		}
		public List<MultiRowHeaderWidthInfoRow> Rows { get; private set; }
		public void UpdateTotalWidths() {
			foreach(MultiRowHeaderWidthInfoRow row in Rows) {
				row.Headers.UpdateTotalWidths();
				Width = Math.Max(Width, row.Headers.TotalWidth);
				MinWidth = Math.Max(MinWidth, row.Headers.TotalMinWidth);
			}
		}
		public MultiRowHeaderWidthInfoRow FindRow(IHeaderObject headerObject) {
			foreach(MultiRowHeaderWidthInfoRow row in Rows) {
				if(row.Headers.FindObject(headerObject) != null) return row;
			}
			return null;
		}
	}
	public class MultiRowHeaderWidthInfoRow {
		public MultiRowHeaderWidthInfoRow(HeaderWidthInfo owner) {
			Owner = owner;
			Headers = new HeaderWidthInfoCollection(Owner);
		}
		public HeaderWidthInfo Owner { get; private set; }
		public HeaderWidthInfoCollection Headers { get; private set; }
	}
	public class HeaderWidthInfoCollection : CollectionBase {
		public HeaderWidthInfoCollection(HeaderWidthInfo owner) {
			Owner = owner;
		}
		public int TotalWidth { get; set; }
		public int TotalMinWidth { get; set; }
		public HeaderWidthInfo this[int index] { get { return List[index] as HeaderWidthInfo; } }
		public bool LockUpdateParent { get; set; }
		public int IndexOf(HeaderWidthInfo info) { return List.IndexOf(info); }
		public void Add(HeaderWidthInfo headerObject) {
			List.Add(headerObject);
		}
		public void UpdateTotalWidths() {
			TotalWidth = 0; 
			TotalMinWidth = 0;
			for(int n = 0; n < Count; n++) {
				HeaderWidthInfo info = this[n];
				TotalWidth += info.Width;
				TotalMinWidth += info.MinWidth;
			}
		}
		protected override void OnInsertComplete(int index, object value) {
			base.OnInsertComplete(index, value);
			if(LockUpdateParent) return;
			HeaderWidthInfo headerObject = (HeaderWidthInfo)value;
			headerObject.Parent = Owner;
		}
		protected override void OnRemoveComplete(int index, object value) {
			base.OnRemoveComplete(index, value);
			if(LockUpdateParent) return;
			HeaderWidthInfo headerObject = (HeaderWidthInfo)value;
			headerObject.Parent = null;
		}
		public HeaderWidthInfo Owner { get; private set; }
		public virtual HeaderWidthInfo FindObject(IHeaderObject headerObject) {
			foreach(HeaderWidthInfo info in this) {
				if(info.HeaderObject == headerObject) return info;
				if(info.HasChildren) {
					HeaderWidthInfo res = info.Children.FindObject(headerObject);
					if(res != null) return res;
				}
				MultiRowHeaderWidthInfo mRowInfo = info as MultiRowHeaderWidthInfo;
				if(mRowInfo != null) {
					foreach(MultiRowHeaderWidthInfoRow row in mRowInfo.Rows) {
						HeaderWidthInfo res = row.Headers.FindObject(headerObject);
						if(res != null) return res;
					}
				}
			}
			return null;
		}
	}
}
