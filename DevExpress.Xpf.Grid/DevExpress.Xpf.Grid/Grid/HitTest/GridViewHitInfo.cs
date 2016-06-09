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

using System.Windows;
using DevExpress.Xpf.Core.Native;
using System;
using System.Linq;
using DevExpress.Xpf.Core;
using System.Windows.Controls;
using DevExpress.Xpf.Grid.HitTest;
using DevExpress.Xpf.Grid.Native;
using DevExpress.Xpf.Grid.TreeList;
using System.Windows.Media;
namespace DevExpress.Xpf.Grid {
	public abstract class GridViewHitInfoBase : HitInfoBase<DataViewHitTestVisitorBase>, IDataViewHitInfo {
		public static DataViewHitTestAcceptorBase GetHitTestAcceptor(DependencyObject obj) {
			return (DataViewHitTestAcceptorBase)obj.GetValue(HitTestAcceptorProperty);
		}
		public static void SetHitTestAcceptor(DependencyObject obj, DataViewHitTestAcceptorBase value) {
			obj.SetValue(HitTestAcceptorProperty, value);
		}
		public static readonly DependencyProperty HitTestAcceptorProperty =
			DependencyProperty.RegisterAttached("HitTestAcceptor", typeof(DataViewHitTestAcceptorBase), typeof(GridViewHitInfoBase), new PropertyMetadata(null));
		internal static void SetHitTestAcceptorSafe(DependencyObject obj, DataViewHitTestAcceptorBase value) {
			if(obj != null)
				obj.SetValue(HitTestAcceptorProperty, value);
		}
		protected static bool HitInfoInArea<T>(T value, params T[] areaTypes) {
			return areaTypes.Contains<T>(value);
		}
		internal static TreeListViewHitTest ConvertToTreeListViewHitTest(TableViewHitTest hitTest) {
			return
				Enum.IsDefined(typeof(TreeListViewHitTest), hitTest.ToString())
				? (TreeListViewHitTest)Enum.Parse(typeof(TreeListViewHitTest), hitTest.ToString(), true)
				: TreeListViewHitTest.None;
		}
		internal static TableViewHitTest ConvertToTableViewHitTest(TreeListViewHitTest hitTest) {
			switch(hitTest) {
				case TreeListViewHitTest.ExpandButton:
				case TreeListViewHitTest.NodeImage:
				case TreeListViewHitTest.NodeIndent:
					return TableViewHitTest.Row;
				default:
					return
						Enum.IsDefined(typeof(TableViewHitTest), hitTest.ToString())
						? (TableViewHitTest)Enum.Parse(typeof(TableViewHitTest), hitTest.ToString(), true)
						: TableViewHitTest.None;
			}
		}
#if !SL
		internal static CardViewHitTest ConvertToCardViewHitTest(TableViewHitTest hitTest) {
			switch(hitTest) {
				case TableViewHitTest.RowCell:
					return CardViewHitTest.FieldValue;
				case TableViewHitTest.Row:
					return CardViewHitTest.Card;
				default:
					return (CardViewHitTest)Enum.Parse(typeof(CardViewHitTest), hitTest.ToString(), true);
			}
		}
		internal static TableViewHitTest ConvertToTableViewHitTest(CardViewHitTest hitTest) {
			switch(hitTest) {
				case CardViewHitTest.FieldValue:
					return TableViewHitTest.RowCell;
				case CardViewHitTest.Card:
					return TableViewHitTest.Row;
				default:
					return Enum.IsDefined(typeof(TableViewHitTest), hitTest.ToString())
						? (TableViewHitTest)Enum.Parse(typeof(TableViewHitTest), hitTest.ToString(), true)
						: TableViewHitTest.None;
			}
		}
#endif
		protected GridViewHitInfoBase(DependencyObject d, DataViewBase view)
			: base(d, view) {
			RowHandle = GridControl.InvalidRowHandle;
			Accept(CreateDefaultVisitor());
			isEmpty = view == null;
			this.view = view;
		}
		internal readonly DataViewBase view;
		readonly bool isEmpty;
		protected ColumnBase columnCore;
		protected BandBase bandCore;
		internal virtual void SetHitTest(TableViewHitTest hitTest) { }
		public int RowHandle { get; internal set; }
		ColumnBase IDataViewHitInfo.Column { get { return columnCore; } }
		public GridColumn Column {
			get { return (GridColumn)columnCore; }
		}
		internal void SetColumn(ColumnBase value) {
			if(!isEmpty)
				columnCore = value;
		}
		public BandBase Band {
			get {
				if(columnCore != null)
					return columnCore.ParentBand;
				return bandCore;
			}
		}
		internal void SetBand(BandBase value) {
			if(!isEmpty)
				bandCore = value;
		}
		protected sealed override HitTestAcceptorBase<DataViewHitTestVisitorBase> GetAcceptor(DependencyObject treeElement) {
			return TableViewHitInfo.GetHitTestAcceptor(treeElement);
		}
		protected abstract GridViewHitTestVisitorBase CreateDefaultVisitor();
		bool HitInfoInArea(params TableViewHitTest[] areaTypes) {
			return HitInfoInArea(GetTableViewHitTest(), areaTypes);
		}
		protected abstract TableViewHitTest GetTableViewHitTest();
		public bool InFilterPanel {
			get {
				return HitInfoInArea(
					TableViewHitTest.FilterPanel,
					TableViewHitTest.FilterPanelText,
					TableViewHitTest.FilterPanelCloseButton,
					TableViewHitTest.FilterPanelCustomizeButton,
					TableViewHitTest.FilterPanelActiveButton,
					TableViewHitTest.MRUFilterListComboBox);
			}
		}
		public virtual bool InGroupPanel {
			get {
				return HitInfoInArea(
					TableViewHitTest.GroupPanel,
					TableViewHitTest.GroupPanelColumnHeader,
					TableViewHitTest.GroupPanelColumnHeaderFilterButton);
			}
		}
		public bool InColumnPanel {
			get {
				return HitInfoInArea(
					TableViewHitTest.ColumnHeaderPanel,
					TableViewHitTest.ColumnButton,
					TableViewHitTest.ColumnHeader,
					TableViewHitTest.ColumnHeaderFilterButton,
					TableViewHitTest.ColumnEdge);
			}
		}
		public bool InBandPanel {
			get {
				return HitInfoInArea(
					TableViewHitTest.BandHeaderPanel,
					TableViewHitTest.BandButton,
					TableViewHitTest.BandHeader,
					TableViewHitTest.BandEdge);
			}
		}
		public bool InGroupColumn {
			get {
				return HitInfoInArea(
					TableViewHitTest.GroupPanelColumnHeader,
					TableViewHitTest.GroupPanelColumnHeaderFilterButton);
			}
		}
		public bool InColumnHeader {
			get {
				return HitInfoInArea(
					TableViewHitTest.ColumnHeader,
					TableViewHitTest.ColumnHeaderFilterButton,
					TableViewHitTest.ColumnEdge);
			}
		}
		public virtual bool InRow {
			get {
				return HitInfoInArea(
					TableViewHitTest.RowCell,
					TableViewHitTest.Row,
					TableViewHitTest.GroupRow,
					TableViewHitTest.GroupRowButton,
					TableViewHitTest.GroupValue,
					TableViewHitTest.GroupSummary,
					TableViewHitTest.RowIndicator);
			}
		}
		#region IDataViewHitInfo Members
		void IDataViewHitInfo.SetHitTest(TableViewHitTest hitTest) {
			SetHitTest(hitTest);
		}
		void IDataViewHitInfo.SetColumn(ColumnBase column) {
			SetColumn(column);
		}
		void IDataViewHitInfo.SetBand(BandBase band) {
			SetBand(band);
		}
		void IDataViewHitInfo.SetRowHandle(int rowHandle) {
			RowHandle = rowHandle;
		}
		bool IDataViewHitInfo.IsRowCell { get { return IsRowCellCore(); } }
		internal abstract bool IsRowCellCore();
		#endregion
	}
	public class TableViewHitInfo : GridViewHitInfoBase, ITableViewHitInfo {
		internal static TableViewHitInfo CalcHitInfo(DependencyObject d, ITableView view) {
			return new TableViewHitInfo(DataViewBase.GetStartHitTestObject(d, view.ViewBase), null, view);
		}
		internal static TableViewHitInfo CalcHitInfo(Point point, ITableView view) {
			UIElement result = null;
			HitTestFilterCallback filterCallback = e => {
				UIElement element = e as UIElement;
				return element == null || element.IsVisible ? HitTestFilterBehavior.Continue : HitTestFilterBehavior.ContinueSkipSelfAndChildren;
			};
			HitTestResultCallback resultCallback = e => {
				result = e.VisualHit as UIElement;
				return result != null ? HitTestResultBehavior.Stop : HitTestResultBehavior.Continue;
			};
			VisualTreeHelper.HitTest(view.ViewBase, filterCallback, resultCallback, new PointHitTestParameters(point));
			return new TableViewHitInfo(DataViewBase.GetStartHitTestObject(result, view.ViewBase), point, view);
		}
		internal static readonly TableViewHitInfo Instance = new TableViewHitInfo(null, null, null);
		TableViewHitTest? hitTest;
		internal TableViewHitInfo(DependencyObject d, Point? relativePoint, ITableView view)
			: base(d, view != null ? view.ViewBase : null) {
			CorrectRowHandleIfNeeded(d, relativePoint);
		}
		void CorrectRowHandleIfNeeded(DependencyObject d, Point? relativePoint) {
			if(view == null || relativePoint == null || RowHandle == DataControlBase.AutoFilterRowHandle || RowHandle == DataControlBase.NewItemRowHandle) return;
			LightweightCellEditor cell = view.GetCellElementByTreeElement(d) as LightweightCellEditor;
			if(cell == null) return;
			relativePoint = view.TranslatePoint(relativePoint.Value, cell);
			DataViewBase dataView = cell.Column.View;
			if(!dataView.ActualAllowCellMerge) return;
			double height = cell.ActualHeight;
			int visibleIndex = dataView.DataControl.GetRowVisibleIndexByHandleCore(RowHandle);
			do {
				if(!dataView.IsPrevRowCellMerged(visibleIndex, Column, true))
					break;
				FrameworkElement row = dataView.GetRowElementByRowHandle(dataView.DataControl.GetRowHandleByVisibleIndexCore(visibleIndex));
				if(relativePoint.Value.Y > height - row.ActualHeight)
					break;
				height -= row.ActualHeight;
				visibleIndex--;
			} while(true);
			RowHandle = dataView.DataControl.GetRowHandleByVisibleIndexCore(visibleIndex);
		}
		protected override GridViewHitTestVisitorBase CreateDefaultVisitor() {
			return new HitInfoTableViewHitTestVisitor(this);
		}
		internal override void SetHitTest(TableViewHitTest hitTest) {
			if(this.hitTest == null)
				this.hitTest = hitTest;
		}
		protected override TableViewHitTest GetTableViewHitTest() {
			return HitTest;
		}
		public TableViewHitTest HitTest {
			get { return hitTest ?? TableViewHitTest.None; }
		}
		public bool InRowCell {
			get {
				return HitInfoInArea(HitTest,
					TableViewHitTest.RowCell);
			}
		}
		#region ITableViewHitInfo implementation
		bool ITableViewHitInfo.IsRowIndicator { get { return HitTest == TableViewHitTest.RowIndicator; } }
		internal override bool IsRowCellCore() {
			return HitTest == TableViewHitTest.RowCell;
		}
		#endregion
	}
#if !SL
	public class CardViewHitInfo : GridViewHitInfoBase {
		internal static readonly CardViewHitInfo Instance = new CardViewHitInfo(null, null);
		CardViewHitTest? hitTest;
		internal CardViewHitInfo(DependencyObject d, CardView view)
			: base(d, view) {
		}
		protected override GridViewHitTestVisitorBase CreateDefaultVisitor() {
			return new HitInfoCardViewHitTestVisitor(this);
		}
		internal override void SetHitTest(TableViewHitTest hitTest) {
			SetCardHitTest(ConvertToCardViewHitTest(hitTest));
		}
		internal void SetCardHitTest(CardViewHitTest hitTest) {
			if(this.hitTest == null)
				this.hitTest = hitTest;
		}
		public CardViewHitTest HitTest {
			get { return hitTest ?? CardViewHitTest.None; }
		}
		protected override TableViewHitTest GetTableViewHitTest() {
			return ConvertToTableViewHitTest(HitTest);
		}
		public override bool InRow {
			get {
				return base.InRow || HitInfoInArea(HitTest,
					CardViewHitTest.FieldCaption,
					CardViewHitTest.CardHeader,
					CardViewHitTest.CardHeaderButton);
			}
		}
		public override bool InGroupPanel {
			get {
				return base.InGroupPanel || HitInfoInArea(HitTest,
					CardViewHitTest.ColumnPanelShowButton);
			}
		}
		public bool InCardRow {
			get {
				return HitInfoInArea(HitTest,
					CardViewHitTest.FieldValue,
					CardViewHitTest.FieldCaption);
			}
		}
		public bool InCardHeader {
			get {
				return HitInfoInArea(HitTest,
					CardViewHitTest.CardHeader,
					CardViewHitTest.CardHeaderButton);
			}
		}
		internal override bool IsRowCellCore() {
			return InCardRow;
		}
	}
#endif
}
