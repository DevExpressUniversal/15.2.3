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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Utils;
namespace DevExpress.Xpf.Grid.Native {
	public abstract class MouseMoveSelectionBase {
		public abstract bool CanScrollHorizontally { get; }
		public abstract bool CanScrollVertically { get; }
		public abstract bool AllowNavigation { get; }
		public abstract void OnMouseDown(DataViewBase tableView, IDataViewHitInfo hitInfo);
		public abstract void UpdateSelection(ITableView tableView);
		public abstract void OnMouseUp(DataViewBase tableView);
		public abstract void CaptureMouse(DataViewBase tableView);
		public virtual void ReleaseMouseCapture(DataViewBase tableView) {
			DataViewBase rootView = tableView.RootView;
			if(MouseHelper.Captured == rootView)
				MouseHelper.ReleaseCapture(rootView);
		}
	}
	public class MouseMoveSelectionNone : MouseMoveSelectionBase {
		public static readonly MouseMoveSelectionNone Instance = new MouseMoveSelectionNone();
		public override bool CanScrollHorizontally { get { return false; } }
		public override bool CanScrollVertically { get { return false; } }
		public override bool AllowNavigation { get { return true; } }
		public override void OnMouseDown(DataViewBase tableView, IDataViewHitInfo hitInfo) {
		}
		public override void UpdateSelection(ITableView tableView) {
		}
		public override void OnMouseUp(DataViewBase tableView) {
		}
		public override void CaptureMouse(DataViewBase tableView){
		}
	}
	public class MouseMoveSelectionRowIndicator : MouseMoveSelectionBase {
		public static readonly MouseMoveSelectionRowIndicator Instance = new MouseMoveSelectionRowIndicator();
		public override bool CanScrollHorizontally { get { return false; } }
		public override bool CanScrollVertically { get { return true; } }
		public override bool AllowNavigation { get { return false; } }
		public override void OnMouseDown(DataViewBase tableView, IDataViewHitInfo hitInfo) {
			tableView.SelectionAnchor = new SelectionAnchorCell(tableView, hitInfo.RowHandle, null);
			tableView.RootView.ScrollTimer.Start();
		}
		public override void CaptureMouse(DataViewBase tableView) {
			DataViewBase rootView = tableView.RootView;
			if(MouseHelper.Captured != rootView) {
				if(rootView.IsKeyboardFocusWithin)
					MouseHelper.Capture(rootView);
				else
					rootView.StopSelection();
			}
		}
		public override void UpdateSelection(ITableView tableView) {
			DataViewBase rootView = tableView.ViewBase.RootView;
			KeyValuePair<DataViewBase, int> viewVisibleIndex = rootView.GetViewAndVisibleIndex(((ITableView)rootView).TableViewBehavior.LastMousePosition.Y); ;
			DataViewBase view = viewVisibleIndex.Key;
			int visibleIndex = viewVisibleIndex.Value;
			if(visibleIndex < 0) return;
			int rowHandle = view.DataControl.GetRowHandleByVisibleIndexCore(visibleIndex);
			if(view.SelectionOldCell != null && view == view.SelectionOldCell.View && rowHandle == view.SelectionOldCell.RowHandleCore)
				return;
			CaptureMouse(view);
			DataViewBase selectionAnchorView = view.SelectionAnchor.View;
			int startCommonVisibleIndex = selectionAnchorView.DataControl.GetCommonVisibleIndex(view.SelectionAnchor.RowHandleCore);
			int endCommonVisibleIndex = view.DataControl.GetCommonVisibleIndex(rowHandle);
			view.SelectionStrategy.SelectOnlyThisMasterDetailRange(startCommonVisibleIndex, endCommonVisibleIndex);
			view.SelectionOldCell = new SelectionAnchorCell(view, rowHandle, null);
		}
		public override void OnMouseUp(DataViewBase tableView) {
			tableView.RootView.ScrollTimer.Stop();
		}
	}
	public class MouseMoveStrategyGridCell : MouseMoveSelectionBase {
		public static readonly MouseMoveStrategyGridCell Instance = new MouseMoveStrategyGridCell();
		public override bool CanScrollHorizontally { get { return true; } }
		public override bool CanScrollVertically { get { return true; } }
		public override bool AllowNavigation { get { return false; } }
		public override void OnMouseDown(DataViewBase tableView, IDataViewHitInfo hitInfo) {
			tableView.SelectionAnchor = new SelectionAnchorCell(tableView, hitInfo.RowHandle, hitInfo.Column);
			tableView.SelectionOldCell = tableView.SelectionAnchor;
			tableView.ScrollTimer.Start();
		}
		public override void UpdateSelection(ITableView tableView) {
			DataViewBase baseView = tableView.ViewBase;
			if(baseView.IsInvalidFocusedRowHandle)
				return;
			int visibleIndex = (int)baseView.GetViewAndVisibleIndex(tableView.TableViewBehavior.LastMousePosition.Y).Value;
			if(visibleIndex < 0) return;
			int rowHandle = baseView.DataControl.GetRowHandleByVisibleIndexCore(visibleIndex);
			ColumnBase column = tableView.TableViewBehavior.GetColumn(tableView.TableViewBehavior.LastMousePosition.X);
			if(rowHandle == baseView.SelectionOldCell.RowHandleCore && column == baseView.SelectionOldCell.ColumnCore)
				return;
			CaptureMouse(baseView);
			baseView.BeginSelectionCore();
			tableView.TableViewBehavior.UpdateSelectionRectCore(rowHandle, column);
			baseView.EndSelectionCore();
			baseView.SelectionOldCell = new SelectionAnchorCell(baseView, rowHandle, column);
		}
		public override void CaptureMouse(DataViewBase tableView) {
			if (MouseHelper.Captured != tableView) {
				if(tableView.IsKeyboardFocusWithin)
					MouseHelper.Capture(tableView);
				else
					tableView.StopSelection();
			}
		}
		public override void OnMouseUp(DataViewBase tableView) {
			tableView.ScrollTimer.Stop();
		}
	}
	public class MouseMoveSelectionRectangleRowIndicator : MouseMoveSelectionRowIndicator {
		public new static readonly MouseMoveSelectionRectangleRowIndicator Instance = new MouseMoveSelectionRectangleRowIndicator();
		MouseMoveSelectionRectangleHelper helper = new MouseMoveSelectionRectangleHelper();
		public override void OnMouseDown(DataViewBase tableView, IDataViewHitInfo hitInfo) {
			base.OnMouseDown(tableView, hitInfo);
			helper.OnMouseDown(tableView);
		}
		public override void UpdateSelection(ITableView tableView) {
			base.UpdateSelection(tableView);
			helper.UpdateSelection(tableView.ViewBase, tableView.TableViewBehavior, tableView.ShowIndicator ? tableView.IndicatorWidth : 0);
		}
		public override void OnMouseUp(DataViewBase tableView) {
			base.OnMouseUp(tableView);
			helper.OnMouseUp(tableView);
		}
	}
	public class MouseMoveSelectionRectangleGridCell : MouseMoveStrategyGridCell {
		public new static readonly MouseMoveSelectionRectangleGridCell Instance = new MouseMoveSelectionRectangleGridCell();
		MouseMoveSelectionRectangleHelper helper = new MouseMoveSelectionRectangleHelper();
		public override void OnMouseDown(DataViewBase tableView, IDataViewHitInfo hitInfo) {
			base.OnMouseDown(tableView, hitInfo);
			helper.OnMouseDown(tableView);
		}
		public override void UpdateSelection(ITableView tableView) {
			base.UpdateSelection(tableView);
			helper.UpdateSelection(tableView.ViewBase, tableView.TableViewBehavior, tableView.ShowIndicator ? tableView.IndicatorWidth : 0);
		}
		public override void OnMouseUp(DataViewBase tableView) {
			base.OnMouseUp(tableView);
			helper.OnMouseUp(tableView);		  
		}	 
	}
	public class MouseMoveSelectionRectangleHelper {
		Point startClickPoint;
		Point startPoint;
		Point endPoint;
		Rect actualRect;
		double scrollOffsetVertical;
		double scrollOffsetHorizontal;
		public Point StartPoint { get { return startPoint; } }
		public Point EndPoint { get { return endPoint; } }
		double IndicatorWidth { get; set; }
		public void OnMouseDown(DataViewBase view) {
			startPoint = startClickPoint = endPoint = GetTransformPoint(view, view.ViewBehavior);
			UpdateSelectionRect(view);
			IScrollInfo scrollInfo = (IScrollInfo)view.DataPresenter;
			scrollOffsetVertical = scrollInfo.VerticalOffset;
			scrollOffsetHorizontal = scrollInfo.HorizontalOffset;
		}
		public void UpdateSelection(DataViewBase view, DataViewBehavior behavior, double indicatorWidth = 0) {
			IndicatorWidth = indicatorWidth;
			endPoint = GetTransformPoint(view, behavior);
			Point min = GetMinTransformPoint(view, IndicatorWidth);
			Point max = GetMaxTransformPoint(view);
			IScrollInfo scrollInfo = (IScrollInfo)view.DataPresenter;
			if(Math.Abs(scrollInfo.VerticalOffset - scrollOffsetVertical) > 0.1) {
				double offset = scrollInfo.VerticalOffset - scrollOffsetVertical;			  
				bool moveDown = scrollInfo.VerticalOffset > scrollOffsetVertical;
				if(Math.Abs(offset) <= scrollInfo.ViewportHeight)
					startPoint.Y = startClickPoint.Y - (max.Y - min.Y) * (offset / scrollInfo.ViewportHeight);
				else 
					startPoint.Y = moveDown ? min.Y : max.Y;	
			} else
				startPoint.Y = startClickPoint.Y;
			if(Math.Abs(scrollInfo.HorizontalOffset - scrollOffsetHorizontal) > 0.1) {
				double offset = scrollInfo.HorizontalOffset - scrollOffsetHorizontal;				
				bool moveRight = scrollInfo.HorizontalOffset > scrollOffsetHorizontal;
				if(Math.Abs(offset) <= scrollInfo.ViewportWidth) 
					startPoint.X = startClickPoint.X - (max.X - min.X) * (offset / scrollInfo.ViewportWidth);
				 else 
					startPoint.X = moveRight ? min.X : max.X;		
			} else 
				startPoint.X = startClickPoint.X;	 
			UpdateActualRect();
			if(min.Y > actualRect.Y || min.X > actualRect.X) {
				if(min.X - actualRect.X > 0)
					startPoint.X = min.X;
				if(min.Y - actualRect.Y > 0)
					startPoint.Y = min.Y;
			} else if(max.Y < actualRect.Y + actualRect.Height || max.X < actualRect.X + actualRect.Width) {
				if(max.X - (actualRect.X + actualRect.Width) < 0)
					startPoint.X = max.X;
				if(max.Y - (actualRect.Y + actualRect.Height) < 0)
					startPoint.Y = max.Y;
			}
			UpdateSelectionRect(view);
		}
		public void OnMouseUp(DataViewBase view) {
			view.SelectionFrame.Visibility = Visibility.Collapsed;
		}
		Point GetTransformPoint(DataViewBase view, DataViewBehavior behavior) {
			return GetTransformPoint(view, behavior.LastMousePosition);		  
		}
		public Point GetTransformPoint(DataViewBase view, Point point) {
			Point min = GetMinTransformPoint(view, IndicatorWidth);
			Point max = GetMaxTransformPoint(view);
			Point result = view.ScrollContentPresenter.TransformToAncestor(view.DataControl).Transform(point);
			if(result.Y > max.Y)
				result.Y = max.Y;
			if(result.Y < min.Y)
				result.Y = min.Y;
			if(result.X > max.X)
				result.X = max.X;
			if(result.X < min.X)
				result.X = min.X;
			return result;
		}
		public static Point GetMinTransformPoint(DataViewBase view, double indicatorWidth) {
			return view.ScrollContentPresenter.TransformToAncestor(view.DataControl).Transform(new Point(indicatorWidth, 0));
		}
		public static Point GetMaxTransformPoint(DataViewBase view) {
			return view.ScrollContentPresenter.TransformToAncestor(view.DataControl).Transform(new Point(view.ScrollContentPresenter.ActualWidth, view.ScrollContentPresenter.ActualHeight));
		}
		void UpdateActualRect() {
			double x, y, width, height;
			if(endPoint.X < startPoint.X) {
				x = endPoint.X;
				width = startPoint.X - endPoint.X;
			} else {
				x = startPoint.X;
				width = endPoint.X - startPoint.X;
			}
			if(endPoint.Y < startPoint.Y) {
				y = endPoint.Y;
				height = startPoint.Y - endPoint.Y;
			} else {
				y = startPoint.Y;
				height = endPoint.Y - startPoint.Y;
			}
			actualRect = new Rect(x, y, width, height);
		}
		void UpdateSelectionRect(DataViewBase view) {
			UpdateActualRect();
			System.Windows.Controls.Canvas.SetLeft(view.SelectionFrame, actualRect.X);
			System.Windows.Controls.Canvas.SetTop(view.SelectionFrame, actualRect.Y);
			view.SelectionFrame.Width = actualRect.Width;
			view.SelectionFrame.Height = actualRect.Height;
			if(actualRect.Width > 0 || actualRect.Height > 0)
				view.SelectionFrame.Visibility = Visibility.Visible;
		}
	}
}
