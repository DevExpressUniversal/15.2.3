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

using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.Utils.Serializing;
namespace DevExpress.XtraCharts.Native {
	public class SelectedItemInfo {
		int seriesIndex;
		int seriesPointIndex;
		public bool IsSeriesSelection { get { return seriesPointIndex < 0; } }
		[XtraSerializableProperty]
		public int SeriesIndex {
			get { return seriesIndex; }
			set { seriesIndex = value; }
		}
		[XtraSerializableProperty]
		public int SeriesPointIndex {
			get { return seriesPointIndex; }
			set { seriesPointIndex = value; }
		}
		public SelectedItemInfo() { }
		public SelectedItemInfo(int seriesIndex) : this(seriesIndex, -1) { }
		public SelectedItemInfo(int seriesIndex, int refinedPointIndex) {
			this.seriesIndex = seriesIndex;
			this.seriesPointIndex = refinedPointIndex;
		}
	}
	public interface ISelectionController {
		SeriesSelectionMode SeriesSelectionMode { get; }
		void OnSelectedItemsChanged(ChartCollectionOperation operation, int index, object value);
	}
	public class SelectionController : ISelectionController {
		readonly SelectedItemCollection selectedItems;
		readonly List<object> newItems = new List<object>();
		readonly List<object> oldItems = new List<object>();
		Chart chart;
		ElementSelectionMode elementSelectionMode;
		SeriesSelectionMode seriesSelectionMode;
		SelectedItemsInfos selectedItemsInfos;
		internal SeriesSelectionMode ActualSeriesSelectionMode {
			get { return chart.ChartContainer.DesignMode ? SeriesSelectionMode.Series : seriesSelectionMode; }
		}
		public ElementSelectionMode ElementSelectionMode {
			get { return elementSelectionMode; }
			set {
				if (elementSelectionMode != value) {
					elementSelectionMode = value;
				}
			}
		}
		public SeriesSelectionMode SeriesSelectionMode {
			get { return seriesSelectionMode; }
			set {
				if (seriesSelectionMode != value) {
					seriesSelectionMode = value;
				}
			}
		}
		public IList SelectedItems { get { return selectedItems; } }
		public SelectedItemsInfos SelectedItemsInfos { get { return selectedItemsInfos; } }
		public SelectionController(Chart chart) {
			this.chart = chart;
			this.selectedItems = new SelectedItemCollection(this);
			this.selectedItemsInfos = new SelectedItemsInfos();
		}
		#region ISelectionController
		void ISelectionController.OnSelectedItemsChanged(ChartCollectionOperation operation, int index, object value) {
			object itemToSelect = FindItemForChangeHitTestState(value);
			newItems.Clear();
			oldItems.Clear();
			if (operation == ChartCollectionOperation.InsertItem) {
				newItems.Add(itemToSelect);
				UpdateSelectionHitStates(SelectionAction.Add, newItems);
				chart.ContainerAdapter.OnSelectedItemsChanged(new SelectedItemsChangedEventArgs(SelectedItemsChangedAction.Add, newItems, oldItems));
			}
			else if (operation == ChartCollectionOperation.RemoveItem) {
				oldItems.Add(itemToSelect);
				UpdateSelectionHitStates(SelectionAction.Remove, oldItems);
				chart.ContainerAdapter.OnSelectedItemsChanged(new SelectedItemsChangedEventArgs(SelectedItemsChangedAction.Remove, newItems, oldItems));
			}
			else if (operation == ChartCollectionOperation.Clear) {
				UpdateSelectionHitStates(SelectionAction.Reset, null);
				chart.ContainerAdapter.OnSelectedItemsChanged(new SelectedItemsChangedEventArgs(SelectedItemsChangedAction.Reset, newItems, oldItems));
			}
			chart.ContainerAdapter.Invalidate();
		}
		#endregion
		bool CaluclateItemsSelection(IList<object> itemsToSelect) {
			foreach (object item in itemsToSelect)
				if (ContainsInSelectedItems(item))
					return true;
			return false;
		}
		SelectionAction CalculateSelectionAction(ElementSelectionMode selectionMode, Keys keyModifiers, bool isItemSelected, bool forceSelect) {
			if (chart.ChartContainer.DesignMode)
				return SelectionAction.Replace;
			switch (selectionMode) {
				case ElementSelectionMode.Single:
					return isItemSelected ? SelectionAction.None : SelectionAction.Replace;
				case ElementSelectionMode.Multiple:
					return isItemSelected ? SelectionAction.Remove : SelectionAction.Add;
				case ElementSelectionMode.Extended:
					if (keyModifiers == Keys.Shift)
						return SelectionAction.Add;
					if (keyModifiers == Keys.Control)
						return isItemSelected ? SelectionAction.Remove : SelectionAction.Add;
					return SelectionAction.Replace;
			}
			return forceSelect ? SelectionAction.Add : SelectionAction.None;
		}
		RefinedPoint FindRefinedPoint(ISeriesPoint seriesPoint) {
			ISeries series = seriesPoint is SeriesPoint ? ((SeriesPoint)seriesPoint).Series : ((AggregatedSeriesPoint)seriesPoint).Series;
			IRefinedSeries refinedSeries = chart.ViewController.FindRefinedSeries((Series)series);
			if (refinedSeries != null)
				foreach (RefinedPoint refinedPoint in refinedSeries.Points) {
					if (Object.ReferenceEquals(refinedPoint.SeriesPoint, seriesPoint))
						return refinedPoint;
					foreach (RefinedPoint childPoint in refinedPoint.Children)
						if (Object.ReferenceEquals(childPoint.SeriesPoint, seriesPoint))
							return childPoint;
				}
			return null;
		}
		List<object> FindFinalRefinedPoints(object point) {
			ViewController viewController = chart.ViewController;
			List<object> finalRefinedPoints = new List<object>();
			RefinedPoint refinedPoint = null;
			if (point is ISeriesPoint)
				refinedPoint = FindRefinedPoint((ISeriesPoint)point);
			else if (point is RefinedPoint)
				refinedPoint = point as RefinedPoint;
			if (refinedPoint != null) {
				RefinedPoint fakePoint = new RefinedPoint(null, refinedPoint.Argument, 0);
				if (refinedPoint.SeriesPoint is AggregatedSeriesPoint) {
					foreach (RefinedSeries refinedSeries in viewController.ActiveRefinedSeries) {
						SortedRefinedPointCollection sortedCollection = (SortedRefinedPointCollection)refinedSeries.FinalPointsSortedByArgument;
						int index = sortedCollection.BinarySearch(fakePoint);
						if (index >= 0)
							finalRefinedPoints.Add(sortedCollection[index]);
					}
				}
				else {
					foreach (IRefinedSeries refinedSeries in viewController.ActiveRefinedSeries) {
						List<RefinedPoint> points = refinedSeries.FindAllPointsWithSameArgument(fakePoint);
						if (points != null && points.Count > 0)
							finalRefinedPoints.AddRange(points);
					}
				}
			}
			return finalRefinedPoints;
		}
		List<object> GetSeriesPoints(object point) {
			List<object> seriesPoints = new List<object>();
			ISeriesPoint seriesPoint = null;
			if (point is ISeriesPoint)
				seriesPoint = (ISeriesPoint)point;
			else if (point is RefinedPoint)
				seriesPoint = ((RefinedPoint)point).SeriesPoint;
			if (seriesPoint is SeriesPoint)
				seriesPoints.Add(GetSelectedItem((SeriesPoint)seriesPoint));
			else if (seriesPoint is AggregatedSeriesPoint) {
				AggregatedSeriesPoint aggregatedPoint = seriesPoint as AggregatedSeriesPoint;
				foreach (RefinedPoint item in aggregatedPoint.SourcePoints)
					seriesPoints.Add(GetSelectedItem((SeriesPoint)item.SeriesPoint));
			}
			return seriesPoints;
		}
		object GetSelectedItem(SeriesPoint seriesPoint) {
			return seriesPoint.Tag != null ? seriesPoint.Tag : seriesPoint;
		}
		void UpdateSelectedItems(SelectionAction action, IList<object> itemsToSelect) {
			selectedItems.LockUpdates();
			List<object> newItems = new List<object>();
			List<object> oldItems = new List<object>();
			switch (action) {
				case SelectionAction.Replace:
					foreach (var item in selectedItems)
						oldItems.Add(item);
					selectedItems.Clear();
					foreach (var item in itemsToSelect) {
						if (!ContainsInSelectedItems(item)) {
							SelectedItems.Add(item);
							newItems.Add(item);
						}
					}
					break;
				case SelectionAction.Add:
					foreach (var item in itemsToSelect) {
						if (!ContainsInSelectedItems(item)) {
							SelectedItems.Add(item);
							newItems.Add(item);
						}
					}
					break;
				case SelectionAction.Remove:
					foreach (var item in itemsToSelect) {
						if (selectedItems.Remove(item))
							oldItems.Add(item);
					}
					break;
				case SelectionAction.None:
					break;
			}
			selectedItems.UnlockUpdates();
			chart.ContainerAdapter.OnSelectedItemsChanged(new SelectedItemsChangedEventArgs((SelectedItemsChangedAction)action, newItems, oldItems));
		}
		void UpdateSelectionHitStates(SelectionAction action, IEnumerable itemsToSelect) {
			switch (action) {
				case SelectionAction.Replace:
					UndoSelectAll(false, false);
					foreach (var item in itemsToSelect)
						ChangeSelectionState(item, true);
					break;
				case SelectionAction.Add:
					foreach (var item in itemsToSelect)
						ChangeSelectionState(item, true);
					break;
				case SelectionAction.Remove:
					foreach (var item in itemsToSelect)
						ChangeSelectionState(item, false);
					break;
				case SelectionAction.Reset:
					UndoSelectAll(false, false);
					break;
				case SelectionAction.None:
					break;
			}
		}
		void UpdateHighlightingHitStates(IEnumerable<object> itemsToSelect) {
			foreach (var item in itemsToSelect)
				ChangeHighlightingState(item);
		}
		void ChangeHighlightingState(object item) {
			if (item is ISeriesBase) {
				((IHitTest)item).State.DoEnter(null);
			}
			else {
				ISeriesPoint seriesPoint;
				if (item is RefinedPoint)
					seriesPoint = ((RefinedPoint)item).SeriesPoint;
				else
					seriesPoint = item as ISeriesPoint;
				ISeries series = seriesPoint is SeriesPoint ? ((SeriesPoint)seriesPoint).Series : ((AggregatedSeriesPoint)seriesPoint).Series;
				((IHitTest)series).State.DoEnter(seriesPoint);
			}
		}
		void ChangeSelectionState(object item, bool needSelect) {
			if (item is ISeriesBase) {
				if (needSelect)
					((IHitTest)item).State.DoSelect(item, true, ElementSelectionMode);
				else
					((IHitTest)item).State.UndoSelect(item, false);
			}
			else {
				ISeriesPoint seriesPoint;
				if (item is RefinedPoint)
					seriesPoint = ((RefinedPoint)item).SeriesPoint;
				else
					seriesPoint = item as ISeriesPoint;
				HitTestState state = null;
				if (seriesPoint is SeriesPoint) {
					var point = (SeriesPoint)seriesPoint;
					if (point.Series != null)
						state = ((IHitTest)point.Series).State;
				}
				else if (seriesPoint is AggregatedSeriesPoint) {
					var point = (AggregatedSeriesPoint)seriesPoint;
					if (point.Series != null)
						state = ((IHitTest)point.Series).State;
				}
				if (state != null) {
					if (needSelect)
						state.DoSelect(seriesPoint, true, ElementSelectionMode);
					else
						state.UndoSelect(seriesPoint, false);
				}
			}
		}
		void UndoSelectAll(bool leaveSeriesSelection, bool clearHot) {
			foreach (IHitTest element in new ChartHitTestEnumerator(chart)) {
				if (leaveSeriesSelection && (element is ISeriesBase || element is ISeriesPoint || element is RefinedPoint))
					continue;
				element.State.UndoSelect(clearHot);
			}
		}
		IList<object> GetItemsForChangeHitState(IHitTest hitElement, object addtionalHitObject) {
			List<object> itemsToSelect = new List<object>();
			switch (ActualSeriesSelectionMode) {
				case SeriesSelectionMode.Series:
					itemsToSelect.Add(hitElement);
					break;
				case SeriesSelectionMode.Point:
					if (addtionalHitObject != null)
						itemsToSelect.Add(addtionalHitObject);
					break;
				case SeriesSelectionMode.Argument:
					if (addtionalHitObject != null)
						return FindFinalRefinedPoints(addtionalHitObject);
					break;
			}
			return itemsToSelect;
		}
		IList<object> GetItemsForChangeSelection(IList<object> itemsToChangeHitState) {
			List<object> itemsToSelect = new List<object>();
			if (itemsToChangeHitState != null && itemsToChangeHitState.Count > 0) {
				switch (SeriesSelectionMode) {
					case SeriesSelectionMode.Series:
						itemsToSelect.Add(itemsToChangeHitState[0]);
						break;
					case SeriesSelectionMode.Point:
						ISeriesPoint seriesPoint = itemsToChangeHitState[0] as ISeriesPoint;
						if (seriesPoint == null && itemsToChangeHitState[0] is RefinedPoint)
							seriesPoint = ((RefinedPoint)itemsToChangeHitState[0]).SeriesPoint;
						itemsToSelect.AddRange(GetSeriesPoints(seriesPoint));
						break;
					case SeriesSelectionMode.Argument:
						foreach (RefinedPoint refinedPoint in itemsToChangeHitState)
							itemsToSelect.AddRange(GetSeriesPoints(refinedPoint));
						break;
				}
			}
			return itemsToSelect;
		}
		void SyncSelectedItemsWithHitTestState() {
			List<object> itemsToChangeHitState = new List<object>();
			foreach (var item in selectedItems) {
				object findedItem = FindItemForChangeHitTestState(item);
				if (findedItem != null)
					itemsToChangeHitState.Add(findedItem);
			}
			UpdateSelectionHitStates(SelectionAction.Replace, itemsToChangeHitState);
		}
		object FindItemForChangeHitTestState(object item) {
			if (item is Series)
				return item;
			else if (item is SeriesPoint)
				return FindPointForSeriesPoint(item as SeriesPoint);
			else if (!(item is ChartElement))
				return FindPointForUserObject(item);
			return null;
		}
		bool IsAggregationEnabled(Series series) {
			IXYSeriesView view = series.View as IXYSeriesView;
			if (view == null)
				return false;
			if (view.AxisXData != null) {
				switch (view.AxisXData.AxisScaleTypeMap.ScaleType) {
					case ActualScaleType.Numerical:
						return view.AxisXData.NumericScaleOptions.ScaleMode != ScaleModeNative.Continuous && !double.IsNaN(view.AxisXData.NumericScaleOptions.MeasureUnit);
					case ActualScaleType.DateTime:
						return view.AxisXData.DateTimeScaleOptions.ScaleMode != ScaleModeNative.Continuous;
				}
			}
			return false;
		}
		ISeriesPoint FindPointForSeriesPoint(SeriesPoint seriesPoint) {
			Series series = seriesPoint.Series;
			if (!IsAggregationEnabled(series))
				return seriesPoint;
			else {
				IRefinedSeries refinedSeries = chart.ViewController.FindRefinedSeries(series);
				foreach (RefinedPoint finalPoint in refinedSeries.Points) {
					AggregatedSeriesPoint aggregatedPoint = finalPoint.SeriesPoint as AggregatedSeriesPoint;
					if (aggregatedPoint != null) {
						foreach (RefinedPoint refinedPoint in aggregatedPoint.SourcePoints) {
							if (Object.ReferenceEquals((SeriesPoint)refinedPoint.SeriesPoint, seriesPoint))
								return aggregatedPoint;
						}
					}
				}
			}
			return null;
		}
		ISeriesPoint FindPointForUserObject(object userObject) {
			foreach (IRefinedSeries series in chart.ViewController.ActiveRefinedSeries) {
				foreach (RefinedPoint point in series.Points) {
					if (point.SeriesPoint is SeriesPoint) {
						SeriesPoint seriesPoint = point.SeriesPoint as SeriesPoint;
						if (Object.ReferenceEquals(seriesPoint.Tag, userObject))
							return seriesPoint;
					}
					else {
						AggregatedSeriesPoint aggregatedPoint = point.SeriesPoint as AggregatedSeriesPoint;
						if (aggregatedPoint != null) {
							foreach (RefinedPoint refinedPoint in aggregatedPoint.SourcePoints) {
								if (Object.ReferenceEquals(((SeriesPoint)refinedPoint.SeriesPoint).Tag, userObject))
									return aggregatedPoint;
							}
						}
					}
				}
			}
			return null;
		}
		bool ContainsInSelectedItems(object itemToSelect) {
			foreach (var item in selectedItems) {
				if (Object.ReferenceEquals(item, itemToSelect) ||
					(item is SeriesPoint && ((SeriesPoint)item).Tag == itemToSelect) ||
					(itemToSelect is SeriesPoint && ((SeriesPoint)itemToSelect).Tag == item))
					return true;
			}
			return false;
		}
		public void FillSelectedItemInfos() {
			selectedItemsInfos.Clear();
			for (int seriesIndex = 0; seriesIndex < chart.Series.Count; seriesIndex++) {
				Series series = chart.Series[seriesIndex];
				foreach (object selectedObject in series.HitState.SelectedObjects) {
					if (selectedObject is ISeries)
						((IList)selectedItemsInfos).Add(new SelectedItemInfo(seriesIndex));
					else if (selectedObject is SeriesPoint) {
						int seriesPointIndex = series.Points.IndexOf((SeriesPoint)selectedObject);
						if (seriesPointIndex >= 0)
							((IList)selectedItemsInfos).Add(new SelectedItemInfo(seriesIndex, seriesPointIndex));
					}
					else if (selectedObject is AggregatedSeriesPoint) {
						AggregatedSeriesPoint aggregatedPoint = selectedObject as AggregatedSeriesPoint;
						foreach (RefinedPoint refinedPoint in aggregatedPoint.SourcePoints) {
							int seriesPointIndex = series.Points.IndexOf((SeriesPoint)refinedPoint.SeriesPoint);
							if (seriesPointIndex >= 0)
								((IList)selectedItemsInfos).Add(new SelectedItemInfo(seriesIndex, seriesPointIndex));
						}
					}
				}
			}
		}
		public void RestoreSelectedItems() {
			foreach (SelectedItemInfo info in selectedItemsInfos) {
				if (info.SeriesIndex >= chart.Series.Count)
					continue;
				Series series = chart.Series[info.SeriesIndex];
				if (info.IsSeriesSelection) {
					if (!ContainsInSelectedItems(series))
						SelectedItems.Add(series);
				}
				else {
					if (info.SeriesPointIndex >= series.Points.Count)
						continue;
					SeriesPoint seriesPoint = series.Points[info.SeriesPointIndex];
					if (!ContainsInSelectedItems(seriesPoint))
						SelectedItems.Add(seriesPoint.Tag == null ? seriesPoint : seriesPoint.Tag);
				}
			}
		}
		public void UpdateSelectedItemsForLegend() {
			foreach (Series series in chart.Series) {
				if (!series.ShouldBeDrawnOnDiagram) {
					selectedItems.Remove(series);
					foreach (SeriesPoint point in series.Points)
						selectedItems.Remove(point);
				}
			}
		}
		public void SelectHitElementInternal(IHitTest hitElement, object additionalHitObject, bool forceSelect, Keys modifierKeys) {
			SelectHitElementInternal(hitElement, additionalHitObject, forceSelect, modifierKeys, null);
		}
		public void SelectHitElementInternal(IHitTest hitElement, object additionalHitObject, bool forceSelect, Keys modifierKeys, ChartFocusedArea focusedArea) {
			if (!forceSelect && !chart.ContainerAdapter.SelectionEnabled)
				return;
			if (hitElement is ISeriesBase) {
				RestoreSelectedItems();
				UndoSelectAll(true, true);
				IList<object> itemsToChangeHitState = GetItemsForChangeHitState(hitElement, additionalHitObject);
				IList<object> itemsToSelect = GetItemsForChangeSelection(itemsToChangeHitState);
				SelectionAction action = CalculateSelectionAction(chart.SelectionMode, modifierKeys, CaluclateItemsSelection(itemsToSelect), forceSelect);
				UpdateSelectionHitStates(action, itemsToChangeHitState);
				UpdateSelectedItems(action, itemsToSelect);
				chart.ContainerAdapter.Invalidate();
			}
			else {
				if (hitElement is Legend && chart.Legend.UseCheckBoxes && focusedArea != null && focusedArea.Element is LegendItemViewData)
					UpdateSelectedItemsForLegend();
				else {
					selectedItems.Clear();
					new ChartHitTestEnumerator(chart).UndoSelect(hitElement, true);
					if (hitElement != null)
						hitElement.State.DoSelect(additionalHitObject, true, ElementSelectionMode);
					chart.ContainerAdapter.Invalidate();
				}
			}
		}
		public void HotHitElementInternal(IHitTest hitElement, object addtionalHitObject) {
			if (!chart.ContainerAdapter.SelectionEnabled)
				return;
			new ChartHitTestEnumerator(chart).DoLeave(hitElement);
			if (hitElement is ISeriesBase) {
				IList<object> itemsToChangeHitState = GetItemsForChangeHitState(hitElement, addtionalHitObject);
				UpdateHighlightingHitStates(itemsToChangeHitState);
			}
			else if (hitElement != null)
				hitElement.State.DoEnter(addtionalHitObject);
			chart.ContainerAdapter.Invalidate();
		}
		public void SyncSelectionStates() {
			SyncSelectedItemsWithHitTestState();
			chart.ContainerAdapter.Invalidate();
		}
		public void ClearSelection(bool clearHot) {
			UndoSelectAll(false, clearHot);
			selectedItems.LockUpdates();
			selectedItems.Clear();
			selectedItems.UnlockUpdates();
		}
		public void Assign(SelectionController selectionController) {
			selectedItemsInfos = new SelectedItemsInfos();
			foreach (var item in selectionController.selectedItemsInfos)
				((IList)selectedItemsInfos).Add(item);
			RestoreSelectedItems();
		}
	}
}
namespace DevExpress.XtraCharts {
	public class SelectedItemsInfos : CollectionBase, IList {
		protected override void OnClearComplete() {
			base.OnClearComplete();
		}
		protected override void OnRemoveComplete(int index, object value) {
			base.OnRemoveComplete(index, value);
		}
		protected override void OnInsertComplete(int index, object value) {
			base.OnInsertComplete(index, value);
		}
	}
	public class SelectedItemCollection : CollectionBase {
		readonly ISelectionController controller;
		bool lockUpdates = false;
		public SelectedItemCollection(ISelectionController controller) {
			this.controller = controller;
		}
		protected override void OnInsertComplete(int index, object value) {
			base.OnInsertComplete(index, value);
			if (!lockUpdates)
				controller.OnSelectedItemsChanged(ChartCollectionOperation.InsertItem, index, value);
		}
		protected override void OnRemoveComplete(int index, object value) {
			base.OnRemoveComplete(index, value);
			if (!lockUpdates)
				controller.OnSelectedItemsChanged(ChartCollectionOperation.RemoveItem, index, value);
		}
		protected override void OnClearComplete() {
			base.OnClearComplete();
			if (!lockUpdates)
				controller.OnSelectedItemsChanged(ChartCollectionOperation.Clear, -1, null);
		}
		internal bool Remove(object item) {
			if (List.Contains(item)) {
				List.Remove(item);
				return true;
			}
			return false;
		}
		internal void LockUpdates() {
			lockUpdates = true;
		}
		internal void UnlockUpdates() {
			lockUpdates = false;
		}
	}
}
