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

using System.Windows.Controls;
using System.Windows;
using System;
using System.Windows.Media;
using DevExpress.Utils;
using DevExpress.Xpf.Scheduler.Internal;
using DevExpress.XtraScheduler.Internal.Diagnostics;
namespace DevExpress.Xpf.Scheduler.Drawing {
	public abstract class SchedulerCellsBasedPanelBase : SchedulerPanelCoreBase {
		readonly PanelController panelController;
		protected SchedulerCellsBasedPanelBase() {
			this.panelController = CreatePanelController();
			CellsInfo = new CellsRectInfo(0);
			SubscribePanelDependecnyEvents();
			SubscribeCellsRectInfoEvents();
		}
		#region SchedulerTimeCellControl
		public ISchedulerTimeCellControl SchedulerTimeCellControl {
			get { return (ISchedulerTimeCellControl)GetValue(SchedulerTimeCellControlProperty); }
			set { SetValue(SchedulerTimeCellControlProperty, value); }
		}
		public static readonly DependencyProperty SchedulerTimeCellControlProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerCellsBasedPanelBase, ISchedulerTimeCellControl>("SchedulerTimeCellControl", null, (d, e) => d.OnSchedulerTimeCellControlChanged(e.OldValue, e.NewValue), null);
		void OnSchedulerTimeCellControlChanged(ISchedulerTimeCellControl oldValue, ISchedulerTimeCellControl newValue) {
			PanelController.CellItemsControl = newValue;
		}
		#endregion
		#region CellContainer
		public VisualCellContainer CellContainer {
			get { return (VisualCellContainer)GetValue(CellContainerProperty); }
			set { SetValue(CellContainerProperty, value); }
		}
		public static readonly DependencyProperty CellContainerProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerCellsBasedPanelBase, VisualCellContainer>("CellContainer", null, (d, e) => d.OnCellContainerChanged(e.OldValue, e.NewValue), null);
		void OnCellContainerChanged(VisualCellContainer oldValue, VisualCellContainer newValue) {
			PanelController.CellContainer = newValue;
		}
		#endregion
		protected PanelController PanelController { get { return panelController; } }
		protected internal CellsRectInfo CellsInfo { get; set; }
		public Thickness Padding { get; set; }
		void SubscribePanelDependecnyEvents() {
			PanelController.LayoutChanged += OnPanelDependencyLayoutChanged;
		}
		private void SubscribeCellsRectInfoEvents() {
			CellsInfo.Changed += OnCellsRectInfoChanged;
		}
		protected virtual void OnCellsRectInfoChanged(object sender, EventArgs e) {
			DoInvalidateMeasure();
		}
		void OnPanelDependencyLayoutChanged(object sender, LayoutChangedEventArgs e) {
			InvalidatePanelDependency(e.ChangeActions);
		}
		protected void InvalidatePanelDependency(AppointmentsPanelChangeActions changeActions) {
			if (changeActions == AppointmentsPanelChangeActions.RecalculateCellsRect) {
				CellsRectCalculator calculator = CreateCellsRectCalculator();
				CellsRectInfo newCellsRectInfo = calculator.CalculateCellsRect(Padding);
				CellsInfo.CopyFrom(newCellsRectInfo);
				PanelController.UpdateCellsVisualStatistics(newCellsRectInfo);
			}
			if (changeActions == AppointmentsPanelChangeActions.RecalculateAppointmentsLayout) {
				DoInvalidateMeasure();
			}
		}
		protected virtual CellsRectCalculator CreateCellsRectCalculator() {
			return new CellsRectCalculator(PanelController);
		}
		protected internal bool EnsureCellsRectValid() {
			return PanelController.EnsureCellsInfoValid(CellsInfo);
		}		
		protected virtual void DoInvalidateMeasure() {
			InvalidateMeasure();
		}
		protected virtual PanelController CreatePanelController() {
			return new PanelController(this);
		}
	}
	public class PanelController {
		VisualCellContainer cellsContainer;
		ISchedulerTimeCellControl schedulerTimeCellControl;
		ISchedulerObservablePanel cellsPanel;
		bool isLayoutLocked = false;
		SchedulerCellsBasedPanelBase ownerPanel;
		public PanelController(SchedulerCellsBasedPanelBase ownerPanel) {
			this.ownerPanel = ownerPanel;
		}
		public VisualCellContainer CellContainer {
			get { return cellsContainer; }
			set {
				if(Object.ReferenceEquals(cellsContainer, value))
					return;
				VisualCellContainer oldValue = cellsContainer;
				cellsContainer = value;
				OnCellContainerChanged(oldValue, cellsContainer);
			}
		}
		public ISchedulerTimeCellControl CellItemsControl {
			get { return schedulerTimeCellControl; }
			set {
				if(schedulerTimeCellControl == value)
					return;
				UnsubscribeSchedulerTimeCellControlEvents(schedulerTimeCellControl);
				schedulerTimeCellControl = value;
				SubscribeSchedulerTimeCellControlEvents(schedulerTimeCellControl);
				OnCellsPanelChanged();
			}
		}
		public ISchedulerObservablePanel CellPanel {
			get { return cellsPanel; }
			set {
				if(cellsPanel == value)
					return;
				UnsubscirbeCellPanelEvents(CellPanel);
				if(value == null)
					return;
				SubscirbeCellPanelEvents(value);
				cellsPanel = value;
				RaiseLayoutChanged(AppointmentsPanelChangeActions.RecalculateCellsRect);
			}
		}
		public SchedulerCellsBasedPanelBase OwnerPanel { get { return ownerPanel; } }
		public bool IsLayoutLocked { get { return isLayoutLocked; } }
		#region LayoutChanged
		public event EventHandler<LayoutChangedEventArgs> LayoutChanged;
		public virtual void RaiseLayoutChanged(AppointmentsPanelChangeActions changeActions) {
			if(IsLayoutLocked)
				return;
			if(LayoutChanged != null)
				LayoutChanged(this, new LayoutChangedEventArgs(changeActions));
		}
		#endregion
		protected virtual void OnCellsPanelChanged() {
			if(CellItemsControl == null)
				return;
			CellPanel = CellItemsControl.SchedulerPanel;
		}
		protected virtual void OnCellContainerChanged(VisualCellContainer oldValue, VisualCellContainer newValue) {
		}
		public VisualTimeCellCollection GetCells() {
			VisualTimeCellCollection result = new VisualTimeCellCollection();
			SchedulerItemsControl itemsControl = CellItemsControl as SchedulerItemsControl;
			if(itemsControl == null)
				return result;
			ItemCollection items = itemsControl.Items;
			ItemContainerGenerator generator = itemsControl.ItemContainerGenerator;
			int count = items.Count;
			for(int i = 0; i < count; i++) {
				VisualTimeCellBase cell = generator.ContainerFromIndex(i) as VisualTimeCellBase;
				if(cell != null)
					result.Add(cell);
			}
			return result;
		}
		public bool EnsureCellsInfoValid(CellsRectInfo cellsInfo) {
			return cellsInfo.GetCellCount() != 0 && cellsInfo.GetCellCount() == GetCellCount();
		}
		protected int GetCellCount() {
			SchedulerItemsControl itemsControl = CellItemsControl as SchedulerItemsControl;
			return itemsControl.Items.Count;
		}
		protected virtual void SubscirbeCellPanelEvents(ISchedulerObservablePanel cellPanel) {
			if(cellPanel == null)
				return;
			cellPanel.Arranged += OnCellPanelArranged;
			cellPanel.VisualChildrenChanged += OnCellPanelVisualChildrenChanged;
		}
		protected virtual void UnsubscirbeCellPanelEvents(ISchedulerObservablePanel cellPanel) {
			if(cellPanel == null)
				return;
			cellPanel.Arranged -= OnCellPanelArranged;
			cellPanel.VisualChildrenChanged -= OnCellPanelVisualChildrenChanged;
		}
		void SubscribeSchedulerTimeCellControlEvents(ISchedulerTimeCellControl schedulerTimeCellControl) {
			if(schedulerTimeCellControl != null)
				schedulerTimeCellControl.SchedulerPanelChanged += OnPanelChanged;
		}
		void UnsubscribeSchedulerTimeCellControlEvents(ISchedulerTimeCellControl schedulerTimeCellControl) {
			if(schedulerTimeCellControl != null)
				schedulerTimeCellControl.SchedulerPanelChanged -= OnPanelChanged;
		}
		void OnCellPanelArranged(object sender, EventArgs e) {
			this.isLayoutLocked = false;
			RaiseLayoutChanged(AppointmentsPanelChangeActions.RecalculateCellsRect);
		}
		void OnCellPanelVisualChildrenChanged(object sender, EventArgs e) {
			this.isLayoutLocked = true;
		}
		void OnPanelChanged(object sender, EventArgs e) {
			OnCellsPanelChanged();
		}
		public virtual void UpdateCellsVisualStatistics(CellsRectInfo newCellsRectInfo) {
		}
	}
	public class CellsRectInfo : ICellInfoProvider {
		Rect[] rects;
		public CellsRectInfo(int count) {
			this.rects = new Rect[count];
		}
		public Rect[] Rects {
			get { return rects; }
			set {
				if(rects == value)
					return;
				rects = value;
			}
		}
		#region OnChanged
		public event EventHandler Changed;
		public virtual void RaiseChanged() {
			if(Changed != null)
				Changed(this, EventArgs.Empty);
		}
		#endregion
		public void CopyFrom(CellsRectInfo other) {
			if(IsDifferenceCellsRect(Rects, other.Rects) ) {
				Rects = other.Rects;
				RaiseChanged();
			}
		}
		bool IsDifferenceCellsRect(Rect[] CellsRect, Rect[] newCellsRect) {
			Guard.ArgumentNotNull(newCellsRect, "newCellsRect");
			if(CellsRect == null)
				return true;
			if(CellsRect.Length != newCellsRect.Length)
				return true;
			int count = newCellsRect.Length;
			for(int i = 0; i < count; i++) {
				if(!CellsRect[i].Equals(newCellsRect[i]))
					return true;
			}
			return false;
		}
		public Rect GetCellRectByIndex(int index) {
			return Rects[index];
		}
		public void Invalidate() {
			Rects = new Rect[0];
			RaiseChanged();
		}
		public int GetCellCount() {
			if(Rects == null)
				return 0;
			return Rects.Length;
		}
	}
	public class CellsRectCalculator {
		PanelController panelController;
		public CellsRectCalculator(PanelController panelController) {
			this.panelController = panelController;
		}
		public PanelController PanelController { get { return panelController; } }
		public CellsRectInfo CalculateCellsRect(Thickness padding) {
			VisualTimeCellCollection cells = PanelController.GetCells();
			int count = cells.Count;
			CellsRectInfo result = new CellsRectInfo(count);
			if(count == 0)
				return result;
			SchedulerPanelCoreBase cellPanel = PanelController.CellPanel as SchedulerPanelCoreBase;
			bool isLayoutReady = cellPanel.IsReady && PanelController.OwnerPanel.IsReady;
			if(!isLayoutReady)
				return new CellsRectInfo(0);
			Rect cellRect = GetCellRect(cells[0]);
			double minX = cellRect.X;
			double minY = cellRect.Y;
			for(int i = 0; i < count; i++) {
				cellRect = GetCellRect(cells[i]);
				if (cellRect.Width <= 0 || cellRect.Height <= 0) 
					continue;
				if (cellRect.X == minX) {
					cellRect.Width -= padding.Left;
					cellRect.X += padding.Left;
				}
				if (cellRect.Y == minY) {
					cellRect.Height += padding.Top;
					cellRect.Y -= padding.Top;
				}
				result.Rects[i] = cellRect;
			}
			return result;
		}
		Rect GetCellRect(VisualTimeCellBase cell) {
			return GetCellRectCore(cell);
		}
		protected virtual Rect GetCellRectCore(VisualTimeCellBase cell) {
			FrameworkElement content = GetCellContent(cell);
			XtraSchedulerDebug.WriteLine("CellContent: {0}, {1}, ({2}, {3})", content.DesiredSize, content.RenderSize, content.ActualWidth, content.ActualHeight);
			GeneralTransform transform = content.TransformToVisual(PanelController.OwnerPanel);
			Rect cellContentRect = new Rect(0, 0, content.ActualWidth, content.ActualHeight);
			return transform.TransformBounds(cellContentRect);
		}
		protected virtual FrameworkElement GetCellContent(VisualTimeCellBase cell) {
			FrameworkElement result = cell.GetCellContent();
			return result ?? cell;
		}
	}
	[Flags]
	public enum AppointmentsPanelChangeActions {
		RecalculateCellsRect, RecalculateAppointmentsLayout
	}
}
