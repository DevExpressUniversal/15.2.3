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
using DependencyPropertyHelper = DevExpress.Xpf.Core.Native.DependencyPropertyHelper;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.Xpf.Scheduler.Native;
using System.Collections.Generic;
using DevExpress.Utils;
using System.Diagnostics;
using DevExpress.XtraScheduler.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Scheduler.Internal;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Scheduler.Drawing {
	public class DynamicVerticalAppointmentsPanel : DynamicAppointmentsPanelBase {
		AppointmentsInfoChangedNotifier<DayViewAppointmentInfo> notifier;
		LoadedUnloadedSubscriber subscriber;
		List<DayViewAppointmentInfo> oldAppointmentInfos;
		List<DayViewAppointmentInfo> newAppointmentInfos;
		public DynamicVerticalAppointmentsPanel() {
			this.notifier = new AppointmentsInfoChangedNotifier<DayViewAppointmentInfo>();
			this.subscriber = new LoadedUnloadedSubscriber(this, OnLoad, OnUnload);
		}
		protected virtual void OnLoad(FrameworkElement fe) {
			this.notifier.RegisterListeners(this);
			this.notifier.NotifyAppointmentsChanged(null, oldAppointmentInfos);
		}
		protected virtual void OnUnload(FrameworkElement fe) {
			this.notifier.NotifyAppointmentsChanged(oldAppointmentInfos, null);
			this.notifier.UnregisterListeners();
		}
		protected override void BeginArrangeAppointments() {
			base.BeginArrangeAppointments();
			this.newAppointmentInfos = new List<DayViewAppointmentInfo>();
		}
		protected override void EndArrangeAppointments() {
			base.EndArrangeAppointments();
			this.notifier.NotifyAppointmentsChanged(oldAppointmentInfos, newAppointmentInfos);
			this.oldAppointmentInfos = newAppointmentInfos;
		}
		public override VisualAppointmentControl CreateVisualAppointment() {
			return new VisualVerticalAppointmentControl();
		}
		protected override void CalculateAppointmentsLayout(IAppointmentGenerator appointmentGenerator, Size availableSize) {
			foreach(AppointmentControl appointmentControl in AppointmentControls)
				if(CanArrangeAppointment(appointmentControl.IntermediateViewInfo))
					appointmentGenerator.GenerateNext(appointmentControl);
		}
		protected override Rect CalculateAppointmentRect(VisualAppointmentControl appointmentControl) {
			Rect firstCellRect = CellsInfo.GetCellRectByIndex(appointmentControl.LayoutViewInfo.FirstCellIndex);
			double top = GetAppointmentTop(appointmentControl);
			if(ShouldExtendAppointmentBounds(appointmentControl))
				CalculateExtendedAppointmentProperties(appointmentControl);
			bool isFirstAppointment = appointmentControl.LayoutViewInfo.FirstIndexPosition == 0;
			bool isLastAppointment = appointmentControl.LayoutViewInfo.LastIndexPosition == appointmentControl.LayoutViewInfo.MaxIndexInGroup;
			double paddingBeforeAppointment = isFirstAppointment ? CellPadding.Left : GapBetweenAppointments / 2.0;
			double paddingAfterAppointment = isLastAppointment ? CellPadding.Right : GapBetweenAppointments / 2.0;
			double factor = firstCellRect.Width / (appointmentControl.LayoutViewInfo.MaxIndexInGroup + 1);
			double left = Math.Round(appointmentControl.LayoutViewInfo.FirstIndexPosition * factor + paddingBeforeAppointment);
			double right = Math.Round((appointmentControl.LayoutViewInfo.LastIndexPosition + 1) * factor - paddingAfterAppointment);
			double height = GetAppointmentActualHeight(appointmentControl);
			double width = Math.Max(0, right - left);
			Rect bounds = new Rect(firstCellRect.Left + left, top, width, height);
			appointmentControl.Measure(RectHelper.Size(bounds));
			appointmentControl.CachedRectangle = bounds;
			this.newAppointmentInfos.Add(CreateAppointmentInfo(appointmentControl, bounds));
			return bounds;
		}
		protected virtual DayViewAppointmentInfo CreateAppointmentInfo(VisualAppointmentControl appointmentControl, Rect bounds) {
			return new DayViewAppointmentInfo(bounds, appointmentControl.GetAppointment());
		}
		protected internal virtual double GetAppointmentActualHeight(VisualAppointmentControl appointmentControl) {
			double height = GetAppointmentHeight(appointmentControl);
			if(ShouldExtendAppointmentBounds(appointmentControl))
				return GetAppointmentMinWidth();
			return height;
		}
		protected internal double GetAppointmentHeight(VisualAppointmentControl appointmentControl) {
			return Math.Max(0, GetAppointmentBottom(appointmentControl) - GetAppointmentTop(appointmentControl));
		}
		protected internal double GetAppointmentBottom(VisualAppointmentControl appointmentControl) {
			Rect lastCellRect = GetCellRectByIndex(appointmentControl.LayoutViewInfo.LastCellIndex);
			return lastCellRect.Bottom - appointmentControl.LayoutViewInfo.EndRelativeOffset / 100.0 * lastCellRect.Height - CellPadding.Bottom;
		}
		protected internal virtual bool ShouldExtendAppointmentBounds(VisualAppointmentControl appointmentControl) {
			if(SnapToCells == AppointmentSnapToCellsMode.Never)
				return GetAppointmentHeight(appointmentControl) < GetAppointmentMinWidth();
			else
				return false;
		}
		protected internal virtual int CalculateExtendedAppointmentLastIndex(VisualAppointmentControl appointmentControl) {
			Rect lastCellBounds = CellsInfo.GetCellRectByIndex(appointmentControl.LayoutViewInfo.LastCellIndex);
			double aptBottomBound = GetAppointmentTop(appointmentControl) + GetAppointmentMinWidth();
			if(aptBottomBound <= lastCellBounds.Bottom)
				return appointmentControl.LayoutViewInfo.LastCellIndex;
			return Math.Min(CellsInfo.GetCellCount() - 1, appointmentControl.LayoutViewInfo.LastCellIndex + 1);
		}
		protected internal double GetAppointmentTop(VisualAppointmentControl appointmentControl) {
			Rect firstCellRect = GetCellRectByIndex(appointmentControl.LayoutViewInfo.FirstCellIndex);
			double result = firstCellRect.Top + appointmentControl.LayoutViewInfo.StartRelativeOffset / 100.0 * firstCellRect.Height + CellPadding.Top;
			if(ShouldCorrectTop(appointmentControl.LayoutViewInfo))
				result--;
			return result;
		}
		protected virtual bool CanArrangeAppointment(AppointmentIntermediateViewInfoCore layoutInfo) {
			int cellCount = CellsInfo.Rects.Length;
			return (layoutInfo.FirstCellIndex >= 0 && layoutInfo.FirstCellIndex < cellCount) && (layoutInfo.LastCellIndex >= 0 && layoutInfo.LastCellIndex < cellCount);
		}
		protected override Size CalculateDesiredSize(Size availableSize, UIElementCollection Children) {
			return Size.Empty;
		}
		protected internal bool ShouldCorrectTop(VisualLayoutViewInfo xpfLayoutViewInfo) {
			if(xpfLayoutViewInfo.FirstCellIndex <= 0)
				return false;
			return xpfLayoutViewInfo.StartRelativeOffset == 0;
		}
		protected internal virtual void CalculateExtendedAppointmentProperties(VisualAppointmentControl appointmentControl) {
			appointmentControl.LayoutViewInfo.LastCellIndex = CalculateExtendedAppointmentLastIndex(appointmentControl);
			appointmentControl.LayoutViewInfo.EndRelativeOffset = RecalcEndOffset(appointmentControl);
			TimeInterval interval = CalculateExtendedAppointmentInterval(appointmentControl);
			appointmentControl.ViewInfo.IntervalStart = interval.Start;
			appointmentControl.ViewInfo.IntervalEnd = interval.End;
		}
		protected internal int RecalcEndOffset(VisualAppointmentControl appointmentControl) {
			Rect cellBounds = CellsInfo.GetCellRectByIndex(appointmentControl.LayoutViewInfo.LastCellIndex);
			if(cellBounds.Height == 0)
				return 0;
			else {
				int endOffset = (int)((cellBounds.Bottom - GetAppointmentTop(appointmentControl) - GetAppointmentMinWidth()) * 100 / cellBounds.Height);
				return Math.Max(0, endOffset);
			}
		}
		protected internal virtual TimeInterval CalculateExtendedAppointmentInterval(VisualAppointmentControl appointmentControl) {
			DateTime start = appointmentControl.ViewInfo.IntervalStart;
			TimeInterval lastCellInterval = GetCellIntervalByIndex(appointmentControl.LayoutViewInfo.LastCellIndex);
			DateTime adjustedEnd = AppointmentTimeScaleHelper.CalculateEndTimeByOffset(lastCellInterval, appointmentControl.LayoutViewInfo.EndRelativeOffset);
			DateTime actualEnd = DateTimeHelper.Max(start, adjustedEnd);
			return new TimeInterval(start, actualEnd);
		}
		protected internal virtual TimeInterval GetCellIntervalByIndex(int index) {
			SchedulerItemsControl itemsControl = SchedulerTimeCellControl as SchedulerItemsControl;
			if(itemsControl == null)
				return TimeInterval.Empty;
			ItemContainerGenerator generator = itemsControl.ItemContainerGenerator;
			VisualTimeCellBase cell = generator.ContainerFromIndex(index) as VisualTimeCellBase;
			return cell == null ? TimeInterval.Empty : ((VisualTimeCellBaseContent)cell.Content).GetInterval();
		}
		protected internal Rect GetCellRectByIndex(int index) {
			return CellsInfo.Rects[index];
		}
		protected override PanelController CreatePanelController() {
			return new AppointmentPanelController(this);
		}
		protected override IAppointmentGenerator CreateAppointmentGenerator(Style appointmentStyle) {
			if(IsDraggedMode)
				return new DraggedAppointmentGenerator(this, appointmentStyle);
			return new VerticalAppointmentGenerator(this, appointmentStyle);
		}
	}
	public class VerticalAppointmentGenerator : AppointmentGeneratorBase<VisualVerticalAppointmentControl> {
		public VerticalAppointmentGenerator(DynamicVerticalAppointmentsPanel panel, Style appointmentStyle)
			: base(panel, appointmentStyle) {
		}
	}
	public class DraggedAppointmentGenerator : AppointmentGeneratorBase<VisualDraggedAppointmentControl> {
		public DraggedAppointmentGenerator(DynamicAppointmentsPanelBase panel, Style appointmentStyle)
			: base(panel, appointmentStyle) {
		}		
	}
}
