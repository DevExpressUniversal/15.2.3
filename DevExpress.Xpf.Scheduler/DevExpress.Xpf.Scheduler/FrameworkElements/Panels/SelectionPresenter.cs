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
using DevExpress.Xpf.Scheduler.Internal;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.Scheduler.Drawing {
	public class SelectionPresenterPanelController : PanelController {
		public SelectionPresenterPanelController(SchedulerCellsBasedPanelBase ownerPanel)
			: base(ownerPanel) {
		}
		protected override void SubscirbeCellPanelEvents(ISchedulerObservablePanel cellPanel) {
			base.SubscirbeCellPanelEvents(cellPanel);
			if (cellPanel == null)
				return;
			cellPanel.Measured += new EventHandler(cellPanel_Measured);
		}
		protected override void UnsubscirbeCellPanelEvents(ISchedulerObservablePanel cellPanel) {
			if (cellPanel == null)
				return;
			cellPanel.Measured -= new EventHandler(cellPanel_Measured);
		}
		void cellPanel_Measured(object sender, EventArgs e) {
			RaiseLayoutChanged(AppointmentsPanelChangeActions.RecalculateCellsRect);
		}
	}
	public class SelectionPresenter : SchedulerCellsBasedPanelBase {
		SchedulerSelection selectionControl;
		LoadedUnloadedSubscriber loadedUnloadedSubscriber;
		XpfScrollViewerController scrollController;
		public SelectionPresenter() {
			this.loadedUnloadedSubscriber = new LoadedUnloadedSubscriber(this, SubscribeEvents, UnsubscribeEvents);
			this.selectionControl = new SchedulerSelection();
			this.scrollController = new XpfScrollViewerController();
			Children.Add(this.selectionControl);
		}
		public SchedulerSelection SelectionControl { get { return selectionControl; } }
		#region SelectedCells
		public SelectedCellIndexesInterval SelectedCells {
			get { return (SelectedCellIndexesInterval)GetValue(SelectedCellsProperty); }
			set { SetValue(SelectedCellsProperty, value); }
		}
		public static readonly DependencyProperty SelectedCellsProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SelectionPresenter, SelectedCellIndexesInterval>("SelectedCells", null, (d, e) => d.OnSelectedCellsChanged(e.OldValue, e.NewValue), null);
		void OnSelectedCellsChanged(SelectedCellIndexesInterval oldValue, SelectedCellIndexesInterval newValue) {
			if (oldValue == newValue)
				return;
			InvalidatePanelDependency(AppointmentsPanelChangeActions.RecalculateCellsRect);
			Dispatcher.BeginInvoke(new Action(() => {
				InvalidateMeasure();
				InvalidateArrange();
			}));
		}
		#endregion
		#region SelectionTemplate
		public ControlTemplate SelectionTemplate {
			get { return (ControlTemplate)GetValue(SelectionTemplateProperty); }
			set { SetValue(SelectionTemplateProperty, value); }
		}
		public static readonly DependencyProperty SelectionTemplateProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SelectionPresenter, ControlTemplate>("SelectionTemplate", null, (d, e) => d.OnSelectionTemplateChanged(e.OldValue, e.NewValue), null);
		void OnSelectionTemplateChanged(ControlTemplate oldValue, ControlTemplate newValue) {
			if (oldValue != null)
				selectionControl.ClearValue(SchedulerSelection.TemplateProperty);
			if (newValue != null)
				selectionControl.Template = newValue;
		}
		#endregion
		#region SelectionLayoutChanged
		LastSelectedCellLayoutChangedEventHandler lastSelectedCellLayoutChangedEvent;
		public event LastSelectedCellLayoutChangedEventHandler SelectionLayoutChanged {
			add { lastSelectedCellLayoutChangedEvent += value; }
			remove { lastSelectedCellLayoutChangedEvent -= value; }
		}
		protected internal virtual void RaiseSelectionLayoutChangedEvent(LastSelectedCellLayoutChangedEventArgs args) {
		}
		#endregion
		protected override PanelController CreatePanelController() {
			return new SelectionPresenterPanelController(this);
		}
		void SubscribeEvents(FrameworkElement fe) {
			this.scrollController.Attach(this);
		}
		void UnsubscribeEvents(FrameworkElement fe) {
			this.scrollController.Detach(this);
		}
		protected override Size DoMeasureOverride(Size availableSize) {
			if (EnsureCellsRectValid()) {
				if (this.selectionControl != null) {
					ReFillSelectionControl();
					selectionControl.Recalculate();
					selectionControl.Measure(availableSize);
					return new Size(1, 1);
				}
			}
			return base.DoMeasureOverride(availableSize);
		}
		protected override Size ArrangeOverride(Size finalSize) {
			if (this.selectionControl == null)
				return finalSize;
			Size actualFinalSize = finalSize;
			Panel cellPanel = this.PanelController.CellPanel as Panel;
			if (cellPanel != null)
				actualFinalSize = cellPanel.RenderSize;
			selectionControl.Arrange(new Rect(new Point(0, 0), actualFinalSize));
			if (SelectedCells != null) {
				if (SelectedCells.VisibleFirstSelectedCell)
					RaiseSelectionLayoutChangedEvent(new LastSelectedCellLayoutChangedEventArgs(SelectedCells.StartCellIndex, this));
				else if (SelectedCells.VisibleLastSelectedCell)
					RaiseSelectionLayoutChangedEvent(new LastSelectedCellLayoutChangedEventArgs(SelectedCells.EndCellIndex, this));
			}
			return finalSize;
		}
		void ReFillSelectionControl() {
			this.selectionControl.ClearBounds();
			if (SelectedCells == null)
				return;
			this.selectionControl.Interval = SelectedCells.Interval;
			this.selectionControl.Resource = SelectedCells.Resource;
			int firstCellIndex = SelectedCells.StartCellIndex;
			int lastCellIndex = SelectedCells.EndCellIndex;
			for (int i = firstCellIndex; i <= lastCellIndex; i++) {
				Rect rect = CellsInfo.GetCellRectByIndex(i);
				selectionControl.AddRect(rect);
			}
		}
		#region Move logic to ScrollViewerController
		internal Rect GetCellBounds(UIElement relativeTo, int cellIndex) {
			Rect rect = selectionControl.GetCellBounds(cellIndex - SelectedCells.StartCellIndex);
			if (rect == Rect.Empty)
				return rect;
			GeneralTransform transform = this.TransformToVisual(relativeTo);
			return transform.TransformBounds(rect);
		}
		#endregion
	}
#if !SL
	public class XpfScrollViewerController {
		public void Attach(SelectionPresenter selectionPresenter) {
			DayViewScrollViewer scrollViewer = FindDayViewScrollViewer(selectionPresenter);
			if(scrollViewer != null)
				scrollViewer.Subscribe(selectionPresenter);
		}
		public void Detach(SelectionPresenter selectionPresenter) {
			DayViewScrollViewer scrollViewer = FindDayViewScrollViewer(selectionPresenter);
			if(scrollViewer != null)
				scrollViewer.Unsubscribe(selectionPresenter);
		}
	protected DayViewScrollViewer FindDayViewScrollViewer(SelectionPresenter selectionPresenter) {
			DependencyObject parent = selectionPresenter;
			while(parent != null) {
				parent = VisualTreeHelper.GetParent(parent);
				DayViewScrollViewer scrollViewer = parent as DayViewScrollViewer;
				if(scrollViewer != null) {
					return scrollViewer;
				}
			}
			return null;
		}
	}
#else
	public class XpfScrollViewerController {
		ScrollViewer scrollViewer;
		public void Attach(SelectionPresenter selectionPresenter) {
			scrollViewer = FindDayViewScrollViewer(selectionPresenter);
			if (scrollViewer != null)
				Subscribe(selectionPresenter);
		}
		public void Detach(SelectionPresenter selectionPresenter) {
			scrollViewer = FindDayViewScrollViewer(selectionPresenter);
			if (scrollViewer != null)
				Unsubscribe(selectionPresenter);
		}
		protected virtual void OnSelectionLayoutChanged(object sender, LastSelectedCellLayoutChangedEventArgs e) {
			Rect rect = e.GetCellBounds(scrollViewer);
			if (rect == Rect.Empty)
				return;
			if (rect.Top < 0)
				scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + rect.Top);
			else if (rect.Bottom > scrollViewer.ViewportHeight)
				scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + (rect.Bottom - scrollViewer.ViewportHeight));
		}
		public void Subscribe(SelectionPresenter selectionPresenter) {
			selectionPresenter.SelectionLayoutChanged += new LastSelectedCellLayoutChangedEventHandler(OnSelectionLayoutChanged);
		}
		public void Unsubscribe(SelectionPresenter selectionPresenter) {
			selectionPresenter.SelectionLayoutChanged -= new LastSelectedCellLayoutChangedEventHandler(OnSelectionLayoutChanged);
		}
		protected ScrollViewer FindDayViewScrollViewer(SelectionPresenter selectionPresenter) {
			DependencyObject parent = selectionPresenter;
			while (parent != null) {
				parent = VisualTreeHelper.GetParent(parent);
				ScrollViewer scrollViewer = parent as ScrollViewer;
				if (scrollViewer != null) {
					return scrollViewer;
				}
			}
			return null;
		}
	}
#endif
}
