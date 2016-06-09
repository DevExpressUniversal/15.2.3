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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.XtraScheduler;
using DependencyPropertyHelper = DevExpress.Xpf.Core.Native.DependencyPropertyHelper;
#if SILVERLIGHT
using DevExpress.Xpf.Editors.Controls;
using DevExpress.Xpf.Core;
using Decorator =  DevExpress.Xpf.Core.WPFCompatibility.Decorator;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Scheduler.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
#else
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Scheduler.Internal;
#endif
namespace DevExpress.Xpf.Scheduler.Drawing {
	#region ISchedulerTimeCellControl
	public interface ISchedulerTimeCellControl : ISupportSchedulerPanel {
		Rect GetCellRect(int cellIndex);
		TimeInterval GetCellInterval(int cellIndex);
		void InvalidateArrange();
		double ActualWidth { get; }
		double ActualHeight { get; }
	}
	#endregion
	#region SchedulerTimeCellControlBase<TItemContainer>
	public class SchedulerTimeCellControlBase<TItemContainer> : GenericContainerItemsControl<TItemContainer>, ISchedulerTimeCellControl where TItemContainer : XPFContentControl, new() {
		ISchedulerObservablePanel schedulerPanel;
		#region ISchedulerTimeCellControl Members
		public ISchedulerObservablePanel SchedulerPanel {
			get {
				return schedulerPanel;
			}
		}
		public Rect GetCellRect(int cellIndex) {
			return GetContentElementByName(cellIndex, "PART_CONTENT").GetBounds(this);
		}
		public TimeInterval GetCellInterval(int cellIndex) {
			VisualTimeCellBaseContent item = (VisualTimeCellBaseContent)Items[cellIndex];
			return new TimeInterval(item.IntervalStart, item.IntervalEnd);
		}
		#endregion
		#region SchedulerPanelChanged
		public event EventHandler SchedulerPanelChanged;
		public virtual void RaiseSchedulerPanelChanged() {
			if(SchedulerPanelChanged != null)
				SchedulerPanelChanged(this, EventArgs.Empty);
		}
		#endregion
		protected virtual ISchedulerObservablePanel GetSchedulerPanel() {
			FrameworkElement result = LayoutHelper.FindElement(this, (fe) => { return fe is Panel; });
			return result as ISchedulerObservablePanel;
		}
		protected override Size MeasureOverride(Size constraint) {
			Size measureResult = base.MeasureOverride(constraint);
			if(SchedulerPanel == null)
				ObtainSchedulerPanel();
			return measureResult;
		}
		void ObtainSchedulerPanel() {
			ISchedulerObservablePanel oldValue = SchedulerPanel;
			this.schedulerPanel = GetSchedulerPanel();
			if(SchedulerPanel != oldValue)
				RaiseSchedulerPanelChanged();
		}
		protected virtual FrameworkElement GetContentElementByName(int cellIndex, string name) {
			VisualResourceCellBase element = (VisualResourceCellBase)this.ItemContainerGenerator.ContainerFromIndex(cellIndex);
			FrameworkElement result = (FrameworkElement)element.ContentElement;
			if(result == null)
				return element;
#if !SL
			if(!result.IsMeasureValid || !result.IsArrangeValid)
#endif
				UpdateLayout(element);
			return result;
		}
		void UpdateLayout(VisualResourceCellBase element) {
			SchedulerPanel parent = VisualTreeHelper.GetParent(element) as SchedulerPanel;
			if(parent != null)
				parent.RecalculateLayout();
		}
	}
	#endregion
	#region SchedulerTimeCellControl
	public class SchedulerTimeCellControl : SchedulerTimeCellControlBase<VisualTimeCell> {
		#region TopBorderVisibility
		public static readonly DependencyProperty BottomBorderVisibilityProperty =
			DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterAttachedProperty<SchedulerTimeCellControl, Visibility>("BottomBorderVisibility", Visibility.Visible, FrameworkPropertyMetadataOptions.None, null);
		public static void SetBottomBorderVisibility(DependencyObject element, Visibility visibility) {
			element.SetValue(BottomBorderVisibilityProperty, visibility);
		}
		public static Visibility GetBottomBorderVisibility(DependencyObject element) {
			return (Visibility)element.GetValue(BottomBorderVisibilityProperty);
		}
		#endregion
	}
	#endregion
	#region SchedulerHorizontalTimeCellsControlBase<TItemContainer>
	public class SchedulerHorizontalTimeCellsControlBase<TItemContainer> : SchedulerTimeCellControlBase<TItemContainer> where TItemContainer : XPFContentControl, new() {
	}
	#endregion
	public class SchedulerHorizontalTimeCellsControl : SchedulerHorizontalTimeCellsControlBase<VisualDateCell> {
	}
	public class SchedulerAllDayAreaTimeCellsControl : SchedulerHorizontalTimeCellsControlBase<VisualAllDayAreaCell> {
	}
	public class SchedulerTimelineHorizontalTimeCellsControl : SchedulerHorizontalTimeCellsControlBase<VisualSingleTimelineCell> {
	}
}
