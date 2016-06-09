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
using DevExpress.XtraScheduler;
using DevExpress.Xpf.Scheduler.Native;
using DevExpress.Xpf.Scheduler.Internal.Implementations;
namespace DevExpress.Xpf.Scheduler.Drawing {
	public class VisualDayViewColumn : VisualSimpleResourceInterval {
		#region VerticalCellContainer
		public static readonly DependencyProperty VerticalCellContainerProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualDayViewColumn, VisualDayViewVerticalCellContainer>("VerticalCellContainer", null);
		public VisualDayViewVerticalCellContainer VerticalCellContainer { get { return (VisualDayViewVerticalCellContainer)GetValue(VerticalCellContainerProperty); } set { SetValue(VerticalCellContainerProperty, value); } }
		#endregion
		#region DateFormats
		public static readonly DependencyProperty DateFormatsProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualDayViewColumn, ObservableStringCollection>("DateFormats", null);
		public ObservableStringCollection DateFormats { get { return (ObservableStringCollection)GetValue(DateFormatsProperty); } set { SetValue(DateFormatsProperty, value); } }
		#endregion
		#region MoreButtonsVisibility
		public static readonly DependencyProperty MoreButtonsVisibilityProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualDayViewColumn, Visibility>("MoreButtonsVisibility", Visibility.Collapsed);
		public Visibility MoreButtonsVisibility { get { return (Visibility)GetValue(MoreButtonsVisibilityProperty); } set { SetValue(MoreButtonsVisibilityProperty, value); } }
		#endregion
		#region IsAlternate
		public static readonly DependencyProperty IsAlternateProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualDayViewColumn, bool>("IsAlternate", false, (d, e) => d.OnIsAlternateChanged(e.OldValue, e.NewValue));
		public bool IsAlternate { get { return (bool)GetValue(IsAlternateProperty); } set { SetValue(IsAlternateProperty, value); } }
		bool lastSettedIsAlternate;
		protected virtual void OnIsAlternateChanged(bool oldValue, bool newValue) {
			lastSettedIsAlternate = newValue;
		}
		#endregion
		#region ResourceId
		object lastResourceId = ResourceInstance.Empty.Id;
		public object ResourceId {
			get { return (object)GetValue(ResourceIdProperty); }
			set { if (this.lastResourceId != value) SetValue(ResourceIdProperty, value); }
		}
		public static readonly DependencyProperty ResourceIdProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualDayViewColumn, object>("ResourceId", ResourceInstance.Empty.Id, (d, e) => d.OnResourceIdChanged(e.OldValue, e.NewValue), null);
		void OnResourceIdChanged(object oldValue, object newValue) {
			this.lastResourceId = newValue;
		}
		#endregion
		protected internal override void OnIntervalStartChanged(DateTime oldValue, DateTime newValue) {
			base.OnIntervalStartChanged(oldValue, newValue);
			UpdateIsAlternateProperty(newValue);
		}
		protected override void CopyFromCore(ICellContainer source) {
			DayViewColumn dayVewColumnSource = (DayViewColumn)source;
			if (VerticalCellContainer == null)
				VerticalCellContainer = new VisualDayViewVerticalCellContainer();
			((ISupportCopyFrom<ICellContainer>)VerticalCellContainer).CopyFrom(source);
			this.View = dayVewColumnSource.View;
			ResourceId = dayVewColumnSource.Resource.Id;
			this.IntervalStart = dayVewColumnSource.Interval.Start;
			this.IntervalEnd = dayVewColumnSource.Interval.End;
			DateTime intervalCellsStart = IntervalStart;
			DateTime intervalCellsEnd = IntervalEnd;
			if (dayVewColumnSource.CellCount > 0) {
				intervalCellsStart = dayVewColumnSource.Cells[0].Interval.Start;
				intervalCellsEnd = dayVewColumnSource.Cells[dayVewColumnSource.CellCount - 1].Interval.End;
			}
			if (IntervalCells.Start != intervalCellsStart && IntervalCells.End != intervalCellsEnd)
				IntervalCells = new TimeInterval(intervalCellsStart, intervalCellsEnd);
			if (DateFormats == null)
				DateFormats = new ObservableStringCollection();
			CollectionCopyHelper.CopyDateFormats(DateFormats, dayVewColumnSource.DateFormats);
			if (Brushes == null)
				Brushes = new VisualResourceBrushes();
			Brushes.CopyFrom(dayVewColumnSource.Brushes, (object)source.Resource.Id);
			UpdateIsAlternateProperty(IntervalStart);
			this.MoreButtonsVisibility = CalculateShouldShowMoreButtons(dayVewColumnSource);
		}
		private void UpdateIsAlternateProperty(DateTime start) {
			if (View == null)
				return;
			bool newIsAlternate = XpfSchedulerUtils.IsTodayDate(View.Control.InnerControl.TimeZoneHelper, start);
			if (lastSettedIsAlternate != newIsAlternate)
				this.IsAlternate = newIsAlternate;
		}
		public override void CopyAppointmentsFrom(ICellContainer cellContainer) {
			if (VerticalCellContainer == null)
				return;
			VerticalCellContainer.CopyAppointmentsFrom(cellContainer);
		}
		public Visibility CalculateShouldShowMoreButtons(DayViewColumn dayVewColumnSource) {
			DevExpress.XtraScheduler.Native.InnerDayView innerView = dayVewColumnSource.View.InnerView;
			return innerView.ShowMoreButtons && innerView.ShowMoreButtonsOnEachColumn ? Visibility.Visible : Visibility.Collapsed;
		}
	}
}
