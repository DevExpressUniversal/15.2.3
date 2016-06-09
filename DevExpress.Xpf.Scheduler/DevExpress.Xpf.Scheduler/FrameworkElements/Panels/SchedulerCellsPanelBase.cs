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
using Panel = DevExpress.Xpf.Core.PanelBase;
namespace DevExpress.Xpf.Scheduler.Drawing {
	public abstract class SchedulerCellsPanelBase : Panel {
		Rect[] cellsRect;
		bool isContentArrangeValid;
		bool isCellsRectValid;
		ISchedulerObservablePanel schedulerPanel;
		protected SchedulerCellsPanelBase() {
			this.Loaded += new RoutedEventHandler(OnLoaded);
			this.Unloaded += new RoutedEventHandler(OnUnloaded);
		}
		void OnUnloaded(object sender, RoutedEventArgs e) {
			LayoutUpdated -= new EventHandler(OnLayoutUpdated);
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			LayoutUpdated -= new EventHandler(OnLayoutUpdated);
			LayoutUpdated += new EventHandler(OnLayoutUpdated);
			OnLayoutUpdated(this, EventArgs.Empty);
		}
		#region SchedulerTimeCellControl
		public ISchedulerTimeCellControl SchedulerTimeCellControl {
			get { return (ISchedulerTimeCellControl)GetValue(SchedulerTimeCellControlProperty); }
			set { SetValue(SchedulerTimeCellControlProperty, value); }
		}
		public static readonly DependencyProperty SchedulerTimeCellControlProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerCellsPanelBase, ISchedulerTimeCellControl>("SchedulerTimeCellControl", null, (d, e) => d.OnSchedulerTimeCellControlChanged(e.OldValue, e.NewValue));
		protected virtual void OnSchedulerTimeCellControlChanged(ISchedulerTimeCellControl oldValue, ISchedulerTimeCellControl newValue) {
			if (oldValue != null)
				UnsubscirbeSchedulerTimeCellControlEvents(oldValue);
			if (newValue != null)
				SubscirbeSchedulerTimeCellControlEvents(newValue);
		}
		#endregion
		#region CellContainer
		public static readonly DependencyProperty CellContainerProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerCellsPanelBase, VisualCellContainer>("CellContainer", null, (d, e) => d.CellContainerChanged(e.OldValue, e.NewValue));
		public VisualCellContainer CellContainer {
			get {
				return (VisualCellContainer)GetValue(CellContainerProperty);
			}
			set {
				SetValue(CellContainerProperty, value);
			}
		}
		#endregion        
		protected virtual Rect[] CellsRect { get { return cellsRect; } set { cellsRect = value; } }
		protected bool IsContentArrangeValid { get { return isContentArrangeValid; } }
		protected bool IsCellsRectValid { get { return isCellsRectValid; } }
		protected virtual void CellContainerChanged(VisualCellContainer oldVaue, VisualCellContainer newValue) {
			if (Object.ReferenceEquals(oldVaue, newValue))
				return;
			if (oldVaue != null)
				UnsubscirbeCellContainerEvent(oldVaue);
			if (newValue != null)
				SubscribeCellContainerEvent(newValue);
		}
		protected virtual void SubscribeCellContainerEvent(VisualCellContainer newValue) {
		}
		protected virtual void UnsubscirbeCellContainerEvent(VisualCellContainer oldVaue) {			
		}
		protected virtual void SubscirbeSchedulerTimeCellControlEvents(ISchedulerTimeCellControl control) {
		}
		private void UnsubscirbeSchedulerTimeCellControlEvents(ISchedulerTimeCellControl control) {
		}
		protected virtual void InvalidateContentArrange() {
			this.isContentArrangeValid = false;
		}
		protected virtual void OnLayoutUpdated(object sender, System.EventArgs e) {
			if (CellContainer == null || SchedulerTimeCellControl == null)
				return;			
			Rect[] newCellsRect = CalculateCellsRect();
			isCellsRectValid = true;
			ApplyNewCellsRect(newCellsRect);
		}
		protected internal bool EnsureCellsRectValid() {
			if(!isCellsRectValid || cellsRect == null || GetCellCount() != this.cellsRect.Length) {
				isCellsRectValid = false;
				return false;
			}
			return true;
		}
		protected void EnsureCellsRect() {
			if(isCellsRectValid)
				return;
			Rect[] newCellsRect = CalculateCellsRect();
			isCellsRectValid = true;
			ApplyNewCellsRect(newCellsRect);			
		}
		protected virtual Rect[] CalculateCellsRect() {
			VisualTimeCellCollection cells = GetCells();
			int count = cells.Count;			
			Rect[] result = new Rect[count];
			if (count == 0)
				return result;
			EnsureSchedulerPanel(cells[0]);
			if (schedulerPanel != null && isCellsRectValid)
				return CellsRect;
			for (int i = 0; i < count; i++) {
				result[i] = GetCellRect(cells[i]);
			}
			return result;
		}
		protected virtual void EnsureSchedulerPanel(VisualTimeCellBase xpfTimeCellBase) {
			ISchedulerObservablePanel newSchedulerPanel = VisualTreeHelper.GetParent(xpfTimeCellBase) as ISchedulerObservablePanel;
			if (newSchedulerPanel == schedulerPanel)
				return;
			UnsubscirbeSchedulerPanelEvents(schedulerPanel);
			if (newSchedulerPanel == null)
				return;
			SubscirbeSchedulerPanelEvents(newSchedulerPanel);
			schedulerPanel = newSchedulerPanel;
			isCellsRectValid = false;
		}
		protected virtual void SubscirbeSchedulerPanelEvents(ISchedulerObservablePanel panel) {
			if (panel == null)
				return;
			panel.Measured += new EventHandler(OnMeasureSchedulerPanel);
			panel.Arranged += new EventHandler(OnArrangSchedulerPanel);
		}
		protected virtual void UnsubscirbeSchedulerPanelEvents(ISchedulerObservablePanel panel) {
			if (panel == null)
				return;
			panel.Measured -= new EventHandler(OnMeasureSchedulerPanel);
			panel.Arranged -= new EventHandler(OnArrangSchedulerPanel);			
		}
		void OnMeasureSchedulerPanel(object sender, EventArgs e) {
			isCellsRectValid = false;
		}
		void OnArrangSchedulerPanel(object sender, EventArgs e) {
			isCellsRectValid = false;
		}
		protected int GetCellCount() {
			SchedulerItemsControl itemsControl = SchedulerTimeCellControl as SchedulerItemsControl;
			return itemsControl.Items.Count;
		}
		protected virtual VisualTimeCellCollection GetCells() {
			VisualTimeCellCollection result = new VisualTimeCellCollection();
			SchedulerItemsControl itemsControl = SchedulerTimeCellControl as SchedulerItemsControl;
			if (itemsControl == null)
				return result;
			ItemCollection items = itemsControl.Items;
			ItemContainerGenerator generator = itemsControl.ItemContainerGenerator;
			int count = items.Count;
			for (int i = 0; i < count; i++) {
				VisualTimeCellBase cell = generator.ContainerFromIndex(i) as VisualTimeCellBase;
				if (cell != null)
					result.Add(cell);
			}
			return result;
		}
		protected virtual Rect GetCellRect(VisualTimeCellBase cell) {
			return GetCellRectCore(cell);
		}
		protected Rect GetCellRectCore(VisualTimeCellBase cell) {
			FrameworkElement content = GetCellContent(cell);
			GeneralTransform transform = content.TransformToVisual(this);
			Rect cellContentRect = new Rect(0, 0, content.ActualWidth, content.ActualHeight);
			return transform.TransformBounds(cellContentRect);
		}
		protected virtual FrameworkElement GetCellContent(VisualTimeCellBase cell) {
			FrameworkElement result = cell.GetCellContent();
			return result ?? cell;
		}
		protected virtual void ApplyNewCellsRect(Rect[] newCellsRect) {
			Guard.ArgumentNotNull(newCellsRect, "newCellsRect");
			if (IsDifferenceCellsRect(CellsRect, newCellsRect) || !IsContentArrangeValid) {
				CellsRect = newCellsRect;
				if(CellsRect != null && CellsRect.Length > 0)
					ArrangeContent();				
			}
		}
		protected virtual bool IsDifferenceCellsRect(Rect[] CellsRect, Rect[] newCellsRect) {
			Guard.ArgumentNotNull(newCellsRect, "newCellsRect");
			if (CellsRect == null)
				return true;
			if (CellsRect.Length != newCellsRect.Length)
				return true;
			int count = newCellsRect.Length;
			for (int i = 0; i < count; i++) {
				if(!GetCellRectByIndex(i).Equals(newCellsRect[i]))
					return true;
			}
			return false;
		}
		protected virtual void ArrangeContent() {
			if (SchedulerTimeCellControl != null && CellContainer != null) {
				ArrangeContentCore();
			}
			isContentArrangeValid = true;
		}
		protected internal Rect GetCellRectByIndex(int index) {
			return CellsRect[index];
		}
		protected abstract void ArrangeContentCore();
	}
}
