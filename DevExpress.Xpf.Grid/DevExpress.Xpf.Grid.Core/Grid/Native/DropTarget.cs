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
using DevExpress.Xpf.Core;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils.Native;
using System.Windows.Markup;
using DevExpress.Xpf.Grid.Native;
using System.Collections;
namespace DevExpress.Xpf.Grid {
	public class ColumnHeaderDropTargetFactory : GridDropTargetFactoryBase {
		protected sealed override IDropTarget CreateDropTarget(UIElement dropTargetElement) {
			return CreateDropTargetCore((Panel)dropTargetElement);
		}
		protected virtual IDropTarget CreateDropTargetCore(Panel panel) {
			return new DropTarget(panel);
		}
	}
	public class FixedNoneColumnHeaderDropTargetFactory : ColumnHeaderDropTargetFactory {
		protected override IDropTarget CreateDropTargetCore(Panel panel) {
			return new FixedNoneDropTarget(panel);
		}
	}
	public class BandedViewColumnHeaderDropTargetFactory : ColumnHeaderDropTargetFactory {
		public FixedStyle FixedStyle { get; set; }
		protected override IDropTarget CreateDropTargetCore(Panel panel) {
			return new BandedViewColumnHeadersDropTarget(panel, FixedStyle);
		}
	}
	public class BandedViewBandHeaderDropTargetFactory : ColumnHeaderDropTargetFactory {
		public FixedStyle FixedStyle { get; set; }
		protected override IDropTarget CreateDropTargetCore(Panel panel) {
			return new BandedViewBandHeadersDropTarget(panel, FixedStyle);
		}
	}
	public class FixedRightColumnHeaderDropTargetFactory : ColumnHeaderDropTargetFactory {
		protected override IDropTarget CreateDropTargetCore(Panel panel) {
			return new FixedRightDropTarget(panel);
		}
	}
	public class FixedLeftColumnHeaderDropTargetFactory : ColumnHeaderDropTargetFactory {
		protected override IDropTarget CreateDropTargetCore(Panel panel) {
			return new FixedLeftDropTarget(panel);
		}
	}
	public class IndicatorColumnHeaderDropTargetFactory : GridDropTargetFactoryBase {
		protected sealed override IDropTarget CreateDropTarget(UIElement dropTargetElement) {
			return new IndicatorColumnHeaderDropTarget(dropTargetElement as GridColumnHeaderBase);
		}
	}
	public class FitColumnHeaderDropTargetFactory : GridDropTargetFactoryBase {
		protected sealed override IDropTarget CreateDropTarget(UIElement dropTargetElement) {
			return new FitColumnHeaderDropTarget(dropTargetElement as GridColumnHeaderBase);
		}
	}
#region obsolete
	[Obsolete("Instead use the ColumnHeaderDropTargetFactory class.")]
	public class ColumnHeaderDropTargetFactoryExtension : ColumnHeaderDropTargetFactory { }
	[Obsolete("Instead use the FixedNoneColumnHeaderDropTargetFactory class.")]
	public class FixedNoneColumnHeaderDropTargetFactoryExtension : FixedNoneColumnHeaderDropTargetFactory { }
	[Obsolete("Instead use the FixedRightColumnHeaderDropTargetFactory class.")]
	public class FixedRightColumnHeaderDropTargetFactoryExtension : FixedRightColumnHeaderDropTargetFactory { }
	[Obsolete("Instead use the FixedLeftColumnHeaderDropTargetFactory class.")]
	public class FixedLeftColumnHeaderDropTargetFactoryExtension : FixedLeftColumnHeaderDropTargetFactory { }
#endregion
}
 namespace DevExpress.Xpf.Grid.Native {
	 public abstract class ColumnHeaderDropTargetBase : HeaderDropTargetBase {
		 protected virtual DataViewBase GridView { get { return (DataViewBase)DataControlBase.FindCurrentView(Panel); } }
		 protected DataControlBase Grid { get { return GridView == null ? null : GridView.DataControl; } }
		 protected override FrameworkElement GridElement { get { return Grid; } }
		 protected override FrameworkElement DragIndicatorTemplateSource { get { return GridView; } }
		 public ColumnHeaderDropTargetBase(Panel panel)
			 : base(panel) { }
		 HeaderPresenterType HeaderPresenterType { get { return BaseGridColumnHeader.GetHeaderPresenterTypeFromLocalValue(AdornableElement); } }
		 protected abstract bool CanDropCore(int dropIndex, ColumnBase sourceColumn, HeaderPresenterType headerPresenterType);
		 protected abstract bool DenyDropIfGroupingIsNotAllowed(HeaderPresenterType sourceType);
		 ColumnBase GetColumnByDragElement(UIElement element) {
			 return BaseGridColumnHeader.GetColumnByDragElement(element);
		 }
		 HeaderPresenterType GetColumnPresenterTypeByDragElement(UIElement element) {
			 return BaseGridColumnHeader.GetHeaderPresenterTypeFromGridColumnHeader(element);
		 }
		 protected virtual bool ContainsColumn(FrameworkElement element, ColumnBase column) {
			 return element.DataContext == column;
		 }
		 protected sealed override bool CanDrop(UIElement source, int dropIndex) {
			 if(dropIndex < 0 || Grid.ColumnsCore.Count == 0)
				 return false;
			 ColumnBase sourceColumn = DropTargetHelper.GetColumnFromDragSource(source) as ColumnBase;
			 if(!Grid.GetOriginationDataControl().DataControlOwner.CanGroupColumn(sourceColumn) && DenyDropIfGroupingIsNotAllowed(BaseGridColumnHeader.GetHeaderPresenterTypeFromGridColumnHeader(source)))
				 return false;
			 return CanDropCore(dropIndex, sourceColumn, BaseGridColumnHeader.GetHeaderPresenterTypeFromGridColumnHeader(source));
		 }
		 protected override string DragIndicatorTemplatePropertyName {
			 get { return DataViewBase.ColumnHeaderDragIndicatorTemplatePropertyName; }
		 }
		 protected override HeaderDropTargetBase.HeaderHitTestResult CreateHitTestResult() {
			 return new ColumnHeaderHitTestResult();
		 }
		 protected override Orientation GetDropPlaceOrientation(DependencyObject element) {
			 return BaseGridColumnHeader.GetDropPlaceOrientation(element);
		 }
		 protected override int GetRelativeVisibleIndex(UIElement element) {
			 if(Grid.DataView.ColumnsCore.Count == 0)
				 return -1;
			 ColumnBase column = GetDependentColumnFromDragSource(element);
			 return GetVisibleIndex(column);
		 }
		 protected override int GetVisibleIndex(DependencyObject obj) {
			 return ColumnBase.GetVisibleIndex(obj) - DropIndexCorrection;
		 }
		 protected virtual int GetVisibleIndex(ColumnBase column) {
			 return IndexOfColumnInChildrenCollection(column);
		 }
		 protected override string HeaderButtonTemplateName {
			 get { return BaseGridColumnHeader.ColumnHeaderContentTemplateName; }
		 }
		 protected override bool GetIsFarCorner(HeaderDropTargetBase.HeaderHitTestResult result, Point point) {
			 FrameworkElement element = result.HeaderElement as FrameworkElement;
			 if(element == null)
				 return false;
			 Rect rect = LayoutHelper.GetRelativeElementRect(element, Panel);
			 if(GetDropPlaceOrientation(element) == Orientation.Horizontal)
				 return (((FrameworkElement)result.HeaderElement).ActualWidth / 2) < point.X - rect.X;
			 else
				 return (((FrameworkElement)result.HeaderElement).ActualHeight / 2) < point.Y - rect.Y;
		 }
		 int IndexOfColumnInChildrenCollection(ColumnBase column) {
			 for(int i = 0; i < Children.Count; i++) {
				 if(ContainsColumn((FrameworkElement)Children[i], column))
					 return i;
			 }
			 return -1;
		 }
		 protected override bool IsSameSource(UIElement element) {
			 return HeaderPresenterType == BaseGridColumnHeader.GetHeaderPresenterTypeFromGridColumnHeader(element);
		 }
		 ColumnBase GetDependentColumnFromDragSource(UIElement source) {
			 ColumnBase sourceColumn = DropTargetHelper.GetColumnFromDragSource(source) as ColumnBase;
			 return CloneDetailHelper.SafeGetDependentCollectionItem<ColumnBase>(sourceColumn, sourceColumn.View.ColumnsCore, Grid.ColumnsCore);
		 }
		 protected override void MoveColumnTo(UIElement source, int dropIndex) {
			 GridView.MoveColumnTo(GetDependentColumnFromDragSource(source), dropIndex + DropIndexCorrection, GetColumnPresenterTypeByDragElement(source), HeaderPresenterType);
		 }
		 protected override void SetColumnHeaderDragIndicatorSize(DependencyObject element, double value) {
			 DataViewBase.SetColumnHeaderDragIndicatorSize(element, value);
		 }
		 protected override Point CorrectDragIndicatorLocation(Point point) {
			 if(!BaseGridColumnHeader.GetCorrectDragIndicatorLocation(Panel))
				 return base.CorrectDragIndicatorLocation(point);
			 return GridView.CorrectDragIndicatorLocation(Panel, point);
		 }
		 protected override void SetDropPlaceOrientation(DependencyObject element, Orientation value) {
			 BaseGridColumnHeader.SetDropPlaceOrientation(element, value);
		 }
		 protected class ColumnHeaderHitTestResult : HeaderDropTargetBase.HeaderHitTestResult {
			 protected override DropPlace GetDropPlace(DependencyObject visualHit) {
				 return DropPlace.None;
			 }
			 protected override DependencyObject GetGridColumn(DependencyObject visualHit) {
				 FrameworkElement element = LayoutHelper.FindParentObject<BaseGridHeader>(visualHit);
				 if(element != null)
					 return BaseGridHeader.GetGridColumn(element);
				 return null;
			 }
		 }
	 }
	 public class DropTarget : ColumnHeaderDropTargetBase {
		 public DropTarget(Panel panel)
			 : base(panel) {
		 }
		 protected override bool DenyDropIfGroupingIsNotAllowed(HeaderPresenterType sourceType) {
			 return sourceType == HeaderPresenterType.GroupPanel;
		 }
		 protected override bool CanDropCore(int dropIndex, ColumnBase sourceColumn, HeaderPresenterType headerPresenterType) {
			 return true;
		 }
		 protected override int DropIndexCorrection { get { return 0; } }
		 protected override bool ContainsColumn(FrameworkElement element, ColumnBase column) {
			 return element.DataContext == column;
		 }
	 }
	 public abstract class FixedDropTarget : DropTarget {
		 protected FixedDropTarget(Panel panel)
			 : base(panel) {
		 }
		 protected ITableView TableView { get { return (ITableView)GridView; } }
		 protected override IList Children {
			 get {
				 return (IList)((OrderPanelBase)Panel).GetSortedChildren(); ;
			 }
		 }
		 protected bool WouldBeFirstFixedNoneColumn(ColumnBase sourceColumn) {
			 return sourceColumn.Fixed == FixedStyle.None && FixedNoneVisibleColumnsCount == 0; 
		 }
		 protected int FixedNoneVisibleColumnsCount { get { return TableView.TableViewBehavior.FixedNoneVisibleColumns.Count; } }
		 protected int FixedLeftVisibleColumnsCount { get { return TableView.TableViewBehavior.FixedLeftVisibleColumns.Count; } }
		 protected int FixedRightVisibleColumnsCount { get { return TableView.TableViewBehavior.FixedRightVisibleColumns.Count; } }
	 }
	 public class FixedLeftDropTarget : FixedDropTarget {
		 public FixedLeftDropTarget(Panel panel)
			 : base(panel) {
		 }
		 protected override int DropIndexCorrection { get { return 0; } }
		 protected override bool CanDropCore(int dropIndex, ColumnBase sourceColumn, HeaderPresenterType headerPresenterType) {
			 return WouldBeFirstFixedNoneColumn(sourceColumn)
				 || sourceColumn.Fixed == FixedStyle.Right && FixedNoneVisibleColumnsCount == 0 && FixedRightVisibleColumnsCount == 0
				 || sourceColumn.Fixed == FixedStyle.Left;
		 }
		 protected override int GetDropIndexFromDragSource(UIElement element, Point pt) {
			 int dropIndex = base.GetDropIndexFromDragSource(element, pt);
			 if(TableView.IsCheckBoxSelectorColumnVisible && dropIndex == 0)
				 dropIndex = -1;
			 return dropIndex;
		 }
	 }
	 public class BandedViewBandHeadersDropTarget : BandedViewColumnHeadersDropTarget {
		 public BandedViewBandHeadersDropTarget(Panel panel, FixedStyle fixedStyle) : base(panel, fixedStyle) {
		 }
		 protected override bool DenyDropToAnotherParent(BaseColumn column, BaseColumn target, HeaderPresenterType moveFrom) {
			 return !IsSameRow(column, target, moveFrom) && !DesignerHelper.GetValue(column, column.AllowChangeParent, true);
		 }
		 protected override bool IsSameRow(BaseColumn column, BaseColumn target, HeaderPresenterType moveFrom) {
			 if(column is BandBase)
				 return AreSameBandsOwners(((BandBase)column).Owner, ((BandBase)target).Owner);
			 if(!Grid.DataView.IsColumnVisibleInHeaders(column) || !column.Visible || moveFrom == HeaderPresenterType.GroupPanel)
				 return AreSameColumns(column.ParentBandInternal, target.ParentBandInternal);
			 return false;
		 }
		 bool AreSameBandsOwners(IBandsOwner source, IBandsOwner target) {
			 IBandsOwner actualSource = source;
			 if(source.DataControl.IsOriginationDataControl())
				 actualSource = source.FindClone(Grid);
			 return actualSource == target;
		 }
		 protected override bool DropToTopBottomEdges(BaseColumn sourceColumn, BaseGridHeader target, Point point, HeaderPresenterType moveFrom, out BandedViewDropPlace dropPlace) {
			 if(sourceColumn.ParentBandInternal != sourceColumn) {
				 if(((BandBase)target.BaseColumn).VisibleBands.Count == 0)
					 dropPlace = ((BandBase)target.BaseColumn).ActualRows.Count == 0 ? BandedViewDropPlace.Left : BandedViewDropPlace.Bottom;
				 else
					 dropPlace = BandedViewDropPlace.None;
				 return true;
			 }
			 dropPlace = BandedViewDropPlace.None;
			 if(point.Y > target.ActualHeight - 4) {
				 BandsLayoutBase bandsLayout = GridView.DataControl.BandsLayoutCore;
				 if(!DesignerHelper.GetValue(bandsLayout, bandsLayout.AllowChangeBandParent, true) || sourceColumn == target.BaseColumn) return true;
				 dropPlace = BandedViewDropPlace.Bottom;
				 return true;
			 }
			 return false;
		 }
		 protected override bool ShouldCalcOffsetForOtherColumns { get { return false; } }
		 protected override void MoveBandColumn(BaseColumn sourceColumn, BaseGridHeader targetHeader, Point point, HeaderPresenterType moveFrom) {
			 BandedViewDropPlace dropPlace = GetDropPlace(sourceColumn, targetHeader, GetLocation(targetHeader, point), moveFrom);
			 BandBase sourceBand = sourceColumn as BandBase;
			 if(sourceBand != null)
				 GridView.DataControl.BandsLayoutCore.MoveBandTo(sourceBand, (BandBase)targetHeader.BaseColumn, dropPlace);
			 else {
				 GridView.DataControl.BandsLayoutCore.MoveColumnToBand(sourceColumn, (BandBase)targetHeader.BaseColumn, dropPlace, moveFrom);
			 }
		 }
		 protected override Point GetAdornerLocationOffset(BaseColumn sourceColumn, BaseGridHeader header, BandedViewDropPlace dropPlace) {
			 if(dropPlace == BandedViewDropPlace.Left && !sourceColumn.IsBand)
				 return new Point(0, header.ActualHeight);
			 return base.GetAdornerLocationOffset(sourceColumn, header, dropPlace);
		 }
	 }
	 public enum BandedViewDropPlace { None, Left, Top, Right, Bottom }
	 public class BandedViewColumnHeadersDropTarget : DropTarget {
		 FixedStyle fixedStyle;
		 public BandedViewColumnHeadersDropTarget(Panel panel, FixedStyle fixedStyle) : base(panel) {
			 this.fixedStyle = fixedStyle;
		 }
		 FixedStyle GetFixedDropPlace(BaseColumn column) {
			 if(fixedStyle != FixedStyle.None) return FixedStyle.None;
			 if(column.Fixed == FixedStyle.Left && GridView.DataControl.BandsLayoutCore.FixedLeftVisibleBands.Count == 0)
				 return FixedStyle.Left;
			 if(column.Fixed == FixedStyle.Right && GridView.DataControl.BandsLayoutCore.FixedRightVisibleBands.Count == 0)
				 return FixedStyle.Right;
			 return FixedStyle.None;
		 }
		 protected virtual bool IsCompatibleSource(BaseColumn column) {
			 if(!column.IsBand || GridView.DataControl.BandsLayoutCore.GetRootBand(column.ParentBandInternal).Fixed == fixedStyle)
				 return true;
			 return GetFixedDropPlace(column) != FixedStyle.None;
		 }
		 BaseColumn GetColumnFromDragSource(UIElement source) {
			 BaseColumn column = DropTargetHelper.GetColumnFromDragSource(source) as BaseColumn;
#if DEBUGTEST
			 if(column == null) return ((BaseGridHeader)source).BaseColumn;
#endif
			 return column;
		 }
		 protected override bool CanDropCore(UIElement source, Point pt, out object dragAnchor, bool isDrag) {
			 dragAnchor = pt;
			 BaseColumn column = GetColumnFromDragSource(source);
			 if(column == null || !IsCompatibleSource(column)) return false;
			 BaseGridHeader targetHeader = GetColumnHeaderHitTestResult(pt).HeaderElement as BaseGridHeader;
			 HeaderPresenterType moveFrom = ColumnBase.GetHeaderPresenterType(source);
			 if(targetHeader == null){
				 BandBase band = GetBandByOffset(pt.X);
				 return band != null && column.CanDropTo(band) && !DenyDropToAnotherParent(column, band, moveFrom);
			 }
			 return GetDropPlace(column, targetHeader, GetLocation(targetHeader, pt), moveFrom) != BandedViewDropPlace.None;
		 }
		 protected virtual bool DenyDropToAnotherParent(BaseColumn column, BaseColumn target, HeaderPresenterType moveFrom) {
			 if(column.IsBand && ((BandBase)column).Owner == target)
				 return false;
			 return !(AreSameColumns(column.ParentBandInternal, target.ParentBandInternal) || DesignerHelper.GetValue(column, column.AllowChangeParent, true));
		 }
		 protected bool AreSameColumns(BaseColumn source, BaseColumn target) {
			 BaseColumn actualSource = source;
			 if(source.View.DataControl.IsOriginationDataControlCore()) {
				 Func<DataControlBase, BaseColumn> getClone = source.CreateCloneAccessor();
				 actualSource = getClone(Grid);
			 }
			 return actualSource == target;
		 }
		 protected virtual bool DropToTopBottomEdges(BaseColumn sourceColumn, BaseGridHeader target, Point point, HeaderPresenterType moveFrom, out BandedViewDropPlace dropPlace) {
			 dropPlace = BandedViewDropPlace.None;
			 if(sourceColumn.IsBand) {
				 dropPlace = BandedViewDropPlace.Top;
				 return true;
			 }
			 if(sourceColumn == target.BaseColumn && sourceColumn.BandRow.Columns.Count == 1) 
				 return moveFrom != HeaderPresenterType.GroupPanel;
			 int targetRowIndex = target.BaseColumn.ParentBandInternal.ActualRows.IndexOf(target.BaseColumn.BandRow);
			 int sourceRowIndex = sourceColumn.ParentBandInternal.ActualRows.IndexOf(sourceColumn.BandRow);
			 bool hasSameParent = target.BaseColumn.ParentBandInternal == sourceColumn.ParentBandInternal;
			 if(point.Y < 4)
				 if(!(sourceRowIndex == targetRowIndex - 1 && hasSameParent) || sourceColumn.BandRow == null || sourceColumn.BandRow.Columns.Count != 1) {
					 dropPlace = BandedViewDropPlace.Top;
					 return true;
				 }
			 if(point.Y > target.ActualHeight - 4)
				 if(!(sourceRowIndex == targetRowIndex + 1 && hasSameParent) || sourceColumn.BandRow == null || sourceColumn.BandRow.Columns.Count != 1) {
					 dropPlace = BandedViewDropPlace.Bottom;
					 return true;
				 }
			 return false;
		 }
		 protected virtual BandedViewDropPlace GetDropPlace(BaseColumn sourceColumn, BaseGridHeader target, Point point, HeaderPresenterType moveFrom) {
			 if(!CanDrop(sourceColumn, target, moveFrom))
				return BandedViewDropPlace.None;
			 if(IsCheckBoxSelectorBand(target.BaseColumn))
				 return GetCheckBoxSelectorBandDropPlace(target, point);
			 BandedViewDropPlace dropPlace = BandedViewDropPlace.None;
			 if(DropToTopBottomEdges(sourceColumn, target, point, moveFrom, out dropPlace))
				 return dropPlace;
			 bool isFarSide = IsFarSide(target, point);
			 if(sourceColumn != null && IsSameRow(sourceColumn, target.BaseColumn, moveFrom) && sourceColumn.Visible) {
				 if(sourceColumn.VisibleIndex == target.BaseColumn.VisibleIndex - 1 && !isFarSide)
					 return BandedViewDropPlace.None;
				 if(sourceColumn.VisibleIndex == target.BaseColumn.VisibleIndex)
					 return moveFrom == HeaderPresenterType.GroupPanel ? BandedViewDropPlace.Left : BandedViewDropPlace.None;
				 if(sourceColumn.VisibleIndex == target.BaseColumn.VisibleIndex + 1 && isFarSide)
					 return BandedViewDropPlace.None;
			 }
			 return isFarSide ? BandedViewDropPlace.Right : BandedViewDropPlace.Left;
		 }
		 BandedViewDropPlace GetCheckBoxSelectorBandDropPlace(BaseGridHeader target, Point point) {
			 return IsFarSide(target, point) ? BandedViewDropPlace.Right : BandedViewDropPlace.None;
		 }
		 bool IsFarSide(FrameworkElement element, Point point) {
			 return point.X > element.ActualWidth / 2;
		 }
		 protected bool IsCheckBoxSelectorColumn(BaseColumn column) {
			 return column.IsServiceColumn() && !column.IsBand;
		 }
		 protected bool IsCheckBoxSelectorBand(BaseColumn column) {
			 return column.IsServiceColumn() && column.IsBand;
		 }
		 bool CanDrop(BaseColumn sourceColumn, BaseGridHeader target, HeaderPresenterType moveFrom) {
			 if(!sourceColumn.CanDropTo(target.BaseColumn) || DenyDropToAnotherParent(sourceColumn, target.BaseColumn, moveFrom))
				 return false;
			 return !IsCheckBoxSelectorColumn(target.BaseColumn);
		 }
		 protected virtual bool IsSameRow(BaseColumn column, BaseColumn target, HeaderPresenterType moveFrom) {
			 return column.BandRow == target.BandRow;
		 }
		 protected Point GetLocation(UIElement relativeTo, Point point) {
			 return GetLocation(LayoutHelper.GetRelativeElementRect(relativeTo, AdornableElement), point);
		 }
		 Point GetLocation(Rect rect, Point point) {
			 PointHelper.Offset(ref point, -rect.Left, -rect.Top);
			 return point;
		 }
		 Rect GetTargetHeaderRelativeRect(BaseGridHeader header, Point point, BaseColumn sourceColumn) {
			 if(header != null) {
				 Rect rect = LayoutHelper.GetRelativeElementRect(header, AdornableElement);
				 if(header.BaseColumn.IsBand && !sourceColumn.IsBand)
					 rect.Height = GridView.HeadersPanel.ActualHeight;
				 return rect;
			 }
			 double offset = 0;
			 BandBase band = GetBandByOffset(GetBandsCollection(fixedStyle), point.X, ref offset);
			 return new Rect(offset, 0, band.ActualHeaderWidth, Panel.ActualHeight);
		 }
		 BandBase GetBandByOffset(double offset) {
			 double actualOffset = 0;
			 return GetBandByOffset(GetBandsCollection(fixedStyle), offset, ref actualOffset);
		 }
		 IEnumerable GetBandsCollection(FixedStyle fixedStyle) {
			 if(fixedStyle == FixedStyle.Left)
				 return GridView.DataControl.BandsLayoutCore.FixedLeftVisibleBands;
			 else if(fixedStyle == FixedStyle.Right)
				 return GridView.DataControl.BandsLayoutCore.FixedRightVisibleBands;
			 else
				 return GridView.DataControl.BandsLayoutCore.FixedNoneVisibleBands;
		 }
		 BandBase GetBandByOffset(IEnumerable bands, double offset, ref double actualOffset) {
			 BandBase current = null;
			 foreach(BandBase band in bands) {
				 if(actualOffset + band.ActualHeaderWidth < offset) {
					 actualOffset += band.ActualHeaderWidth;
					 continue;
				 }
				 if(band.VisibleBands.Count != 0) {
					 current = GetBandByOffset(band.VisibleBands, offset, ref actualOffset);
					 if(current != null) return current;
				 } else {
					 return band;
				 }
			 }
			 return null;
		 }
		 protected virtual bool ShouldCalcOffsetForOtherColumns { get { return true; } }
		 protected override void UpdateDragAdornerLocationCore(UIElement sourceElement, object headerAnchor) {
			 if(DragIndicator == null) return;
			 BaseColumn sourceColumn = GetColumnFromDragSource(sourceElement);
			 FixedStyle fixedDropPlace = GetFixedDropPlace(sourceColumn);
			 if(fixedDropPlace != FixedStyle.None) {
				 UpdateDragAdornerLocation(new Point(fixedDropPlace == FixedStyle.Left ? 0 : Panel.ActualWidth, 0));
				 SetDropPlaceOrientation(DragIndicator, Orientation.Horizontal);
				 SetColumnHeaderDragIndicatorSize(DragIndicator, Panel.ActualHeight);
				 return;
			 }
			 Point pt = (Point)headerAnchor;
			 BaseGridHeader header = GetColumnHeaderHitTestResult(pt).HeaderElement as BaseGridHeader;
			 Rect rect = GetTargetHeaderRelativeRect(header, pt, sourceColumn);
			 BandedViewDropPlace dropPlace = BandedViewDropPlace.Left;
			 if(header != null)
				 dropPlace = GetDropPlace(sourceColumn, header, GetLocation(rect, pt), ColumnBase.GetHeaderPresenterType(sourceElement));
			 else if(sourceColumn.IsBand) {
				 dropPlace = BandedViewDropPlace.Top;
			 }
			 Point location = rect.Location();
			 Point offset = GetAdornerLocationOffset(sourceColumn, header, dropPlace);
			 PointHelper.Offset(ref location, offset.X, offset.Y);
			 if(sourceColumn.IsBand && (header == null || !header.BaseColumn.IsBand)) {
				 location.Y = 0;
			 }
			 if(dropPlace == BandedViewDropPlace.Top || dropPlace == BandedViewDropPlace.Bottom) {
				 SetDropPlaceOrientation(DragIndicator, Orientation.Vertical);
				 SetColumnHeaderDragIndicatorSize(DragIndicator, CalcVerticalDragIndicatorSize(location, header != null ? header.BaseColumn.ParentBandInternal.ActualHeaderWidth : rect.Width));
			 } else {
				 SetDropPlaceOrientation(DragIndicator, Orientation.Horizontal);
				 SetColumnHeaderDragIndicatorSize(DragIndicator, rect.Height);
			 }
			 UpdateDragAdornerLocation(location);
		 }
		 void UpdateDragAdornerLocation(Point location) {
#if !SL
			 DragAdorner.UpdateLocation(CorrectDragIndicatorLocation(location));
#else
			 DragAdorner.FloatLocation = CorrectDragIndicatorLocation(location);
#endif
		 }
		 double CalcVerticalDragIndicatorSize(Point point, double width) {
			 return GridView.CalcVerticalDragIndicatorSize(Panel, point, width);
		 }
		 protected virtual Point GetAdornerLocationOffset(BaseColumn sourceColumn, BaseGridHeader header, BandedViewDropPlace dropPlace) {
			 double left = 0;
			 double top = 0;
			 if(dropPlace == BandedViewDropPlace.Top || dropPlace == BandedViewDropPlace.Bottom) {
				 if(ShouldCalcOffsetForOtherColumns && header != null) {
					 for(int i = header.BaseColumn.BandRow.Columns.IndexOf((ColumnBase)header.BaseColumn) - 1; i >= 0; i--)
						 left -= header.BaseColumn.BandRow.Columns[i].ActualHeaderWidth;
				 }
			 }
			 if(dropPlace == BandedViewDropPlace.Bottom)
				 top += header.ActualHeight;
			 if(dropPlace == BandedViewDropPlace.Right)
				 left += header.ActualWidth;
			 return new Point(left, top);
		 }
		 protected override void MoveColumnToCore(UIElement source, object dropAnchor) {
			 Point point = (Point)dropAnchor;
			 BaseColumn sourceColumn = GetColumnFromDragSource(source);
			 if(GetFixedDropPlace(sourceColumn) != FixedStyle.None) {
				 sourceColumn.Visible = true;
				 return;
			 }
			 BaseGridHeader targetHeader = GetColumnHeaderHitTestResult(point).HeaderElement as BaseGridHeader;
			 MoveBandColumn(sourceColumn, targetHeader, point, ColumnBase.GetHeaderPresenterType(source));
		 }
		 protected virtual void MoveBandColumn(BaseColumn sourceColumn, BaseGridHeader targetHeader, Point point, HeaderPresenterType moveFrom) {
			 BandedViewDropPlace dropPlace = BandedViewDropPlace.Left;
			 if(targetHeader != null)
				 dropPlace = GetDropPlace(sourceColumn, targetHeader, GetLocation(targetHeader, point), moveFrom);
			 if(sourceColumn.IsBand)
				 dropPlace = BandedViewDropPlace.Bottom;
			 BaseColumn targetColumn = targetHeader != null ? targetHeader.BaseColumn : GetBandByOffset(point.X);
			 BandBase sourceBand = sourceColumn as BandBase;
			 if(sourceBand != null)
				 GridView.DataControl.BandsLayoutCore.MoveBandTo(sourceBand, targetColumn.ParentBandInternal, dropPlace);
			 else
				 GridView.DataControl.BandsLayoutCore.MoveColumnTo(sourceColumn, targetColumn, dropPlace, moveFrom);
		 }
	 }
	 public class FixedNoneDropTarget : FixedDropTarget {
		 public FixedNoneDropTarget(Panel panel)
			 : base(panel) {
		 }
		 protected override int DropIndexCorrection { get { return TableView.TableViewBehavior.FixedLeftVisibleColumns.Count; } }
		 protected override bool CanDropCore(int dropIndex, ColumnBase sourceColumn, HeaderPresenterType headerPresenterType) {
			 if(sourceColumn.Fixed == FixedStyle.Left || sourceColumn.Fixed == FixedStyle.Right) {
				 int fixedVisibleColumnsCount = sourceColumn.Fixed == FixedStyle.Left
					 ? FixedLeftVisibleColumnsCount
					 : FixedRightVisibleColumnsCount;
				 return (headerPresenterType == HeaderPresenterType.GroupPanel || headerPresenterType == HeaderPresenterType.ColumnChooser)
					 && fixedVisibleColumnsCount == 0;
			 }
			 return true;
		 }
		 protected override int GetDragIndex(int dropIndex) {
			 return dropIndex - GetFirstVisibleIndex() + DropIndexCorrection;
		 }
		 protected override int GetRelativeVisibleIndex(UIElement element) {
			 return base.GetRelativeVisibleIndex(element) + GetFirstVisibleIndex() - DropIndexCorrection;
		 }
		 int GetFirstVisibleIndex() {
			 return Children.Count > 0 ? ColumnBase.GetVisibleIndex(GetColumn((FrameworkElement)Children[0])) : 0;
		 }
		 ColumnBase GetColumn(FrameworkElement element) {
			 return element.DataContext as ColumnBase;
		 }
		 protected override int GetDropIndexFromDragSource(UIElement element, Point pt) {
			 int dropIndex = base.GetDropIndexFromDragSource(element, pt);
			 if(TableView.IsCheckBoxSelectorColumnVisible && dropIndex == 0 && FixedLeftVisibleColumnsCount == 0)
				 dropIndex = -1;
			 return dropIndex;
		 }
	 }
	 public class FixedRightDropTarget : FixedDropTarget {
		 public FixedRightDropTarget(Panel panel)
			 : base(panel) {
		 }
		 protected override int DropIndexCorrection { get { return TableView.TableViewBehavior.FixedLeftVisibleColumns.Count + TableView.TableViewBehavior.FixedNoneVisibleColumns.Count; } }
		 protected override bool CanDropCore(int dropIndex, ColumnBase sourceColumn, HeaderPresenterType headerPresenterType) {
			 return WouldBeFirstFixedNoneColumn(sourceColumn)
				 || sourceColumn.Fixed == FixedStyle.Left && FixedNoneVisibleColumnsCount == 0 && FixedLeftVisibleColumnsCount == 0
				 || sourceColumn.Fixed == FixedStyle.Right;
		 }
	 }
	 internal static class DropTargetHelper {
		 internal static BaseColumn GetColumnFromDragSource(UIElement source) {
			BaseGridHeader header = (BaseGridHeader)source;
			return header.draggedColumn ?? header.Column; 
		 }
	 }
	 public abstract class ServiceColumnHeaderDropTarget : FixedDropTarget {
		 public ServiceColumnHeaderDropTarget(GridColumnHeaderBase columnHeader) 
			 : base(null) {
			this.ColumnHeader = columnHeader;
		 }
		 protected override IList Children {
			 get { return null; }
		 }
		 protected override int ChildrenCount {
			 get { return 1; }
		 }
		 public GridColumnHeaderBase ColumnHeader { get; private set; }
		 protected override DataViewBase GridView {
			 get { return (DataViewBase)DataControlBase.FindCurrentView(ColumnHeader); }
		 }
		 protected override Orientation GetDropPlaceOrientation(DependencyObject element) {
			 return Orientation.Horizontal;
		 }
		 protected override UIElement AdornableElement {
			 get { return ColumnHeader; }
		 }
		 protected override object GetHeaderButtonOwner(int index) {
			 return ColumnHeader;
		 }
		 protected override Point CorrectDragIndicatorLocation(Point point) {
			 return GridView.CorrectDragIndicatorLocation(ColumnHeader, point);
		 }
		 protected bool IsLastColumnOfItsType(ColumnBase column) {
			 switch(column.Fixed) {
				 case FixedStyle.None:
					 return FixedNoneVisibleColumnsCount != 0 &&
						 TableView.TableViewBehavior.FixedNoneVisibleColumns[FixedNoneVisibleColumnsCount - 1] == column;
				 case FixedStyle.Left:
					 return FixedLeftVisibleColumnsCount != 0 &&
						 TableView.TableViewBehavior.FixedLeftVisibleColumns[FixedLeftVisibleColumnsCount - 1] == column;
				 case FixedStyle.Right:
					 return FixedRightVisibleColumnsCount != 0 &&
						 TableView.TableViewBehavior.FixedRightVisibleColumns[FixedRightVisibleColumnsCount - 1] == column;
			 }
			 return false;
		 }
		 protected bool IsFirstColumnOfItsType(ColumnBase column) {
			 switch(column.Fixed) {
				 case FixedStyle.None:
					 return FixedNoneVisibleColumnsCount != 0 &&
						 TableView.TableViewBehavior.FixedNoneVisibleColumns[0] == column;
				 case FixedStyle.Left:
					 return FixedLeftVisibleColumnsCount != 0 &&
						 TableView.TableViewBehavior.FixedLeftVisibleColumns[0] == column;
				 case FixedStyle.Right:
					 return FixedRightVisibleColumnsCount != 0 &&
						 TableView.TableViewBehavior.FixedRightVisibleColumns[0] == column;
			 }
			 return false;
		 }
		 protected bool ThereIsNoFixedNoneColumns { get { return FixedNoneVisibleColumnsCount == 0; } }
		 protected bool ThereIsNoFixedRightColumns { get { return FixedRightVisibleColumnsCount == 0; } }
		 protected bool ThereIsNoFixedLeftColumns { get { return FixedLeftVisibleColumnsCount == 0; } }
		 protected bool AllowDragToServiceColumn { get { return Grid.BandsCore.Count == 0; } }
		 protected ScrollInfoBase ScrollInfo { 
			 get {
				 if(this.GridView.DataPresenter == null)
					 return null;
				 return this.GridView.DataPresenter.ScrollInfoCore.HorizontalScrollInfo; 
			 } 
		 }
	 }
	 public class IndicatorColumnHeaderDropTarget : ServiceColumnHeaderDropTarget {
		 public IndicatorColumnHeaderDropTarget(GridColumnHeaderBase columnHeader) : base(columnHeader) { }
		 protected override int GetDropIndexFromDragSource(UIElement element, Point pt){
			 if(TableView.IsCheckBoxSelectorColumnVisible)
				 return -1;
			 return 0;
		 }
		 protected override bool CanDropCore(int dropIndex, ColumnBase sourceColumn, HeaderPresenterType headerPresenterType) {
			 if(!AllowDragToServiceColumn)
				 return false;
			 if(IsFirstColumnOfItsType(sourceColumn))
				 return false;
			 switch(sourceColumn.Fixed) {
				 case FixedStyle.None:
					 return ThereIsNoFixedLeftColumns && (ScrollInfo == null || ScrollInfo.Offset == 0.0d);
				 case FixedStyle.Left:
					 return true;
				 case FixedStyle.Right:
					 return ThereIsNoFixedLeftColumns && ThereIsNoFixedNoneColumns;
			 }
			 return false;
		 }
	 }
	 public class FitColumnHeaderDropTarget : ServiceColumnHeaderDropTarget {
		 public FitColumnHeaderDropTarget(GridColumnHeaderBase columnHeader) : base(columnHeader) { }
		 protected override int GetDropIndexFromDragSource(UIElement element, Point pt) {
			 return 0;
		 }
		 protected override int DropIndexCorrection {
			 get {
				 return FixedLeftVisibleColumnsCount + FixedNoneVisibleColumnsCount + FixedRightVisibleColumnsCount;
			 }
		 }
		 protected override bool CanDropCore(int dropIndex, ColumnBase sourceColumn, HeaderPresenterType headerPresenterType) {
			 if(!AllowDragToServiceColumn)
				 return false;
			 if(IsLastColumnOfItsType(sourceColumn))
				 return false;
			 switch(sourceColumn.Fixed) {
				 case FixedStyle.None:
					 ScrollInfoBase scrollInfo = ScrollInfo;
					 return ThereIsNoFixedRightColumns && (scrollInfo == null || sourceColumn.Owner.AutoWidth || scrollInfo.Extent <= scrollInfo.Viewport || IsScrolledToEnd(scrollInfo));
				 case FixedStyle.Left:
					 return ThereIsNoFixedRightColumns && ThereIsNoFixedNoneColumns;
				 case FixedStyle.Right:
					 return true;
			 }
			 return false;
		 }
		 static bool IsScrolledToEnd(ScrollInfoBase scrollInfo) {
			 return scrollInfo.Extent - scrollInfo.Viewport == scrollInfo.Offset;
		 }
	 }
}
