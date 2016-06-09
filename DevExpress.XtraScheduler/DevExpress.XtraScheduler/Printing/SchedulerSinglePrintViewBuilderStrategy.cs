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

using DevExpress.Utils.Drawing;
using DevExpress.XtraScheduler.Internal;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using DevExpress.XtraScheduler.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Printing.Native {
	internal abstract class SchedulerSinglePrintViewBuilderStrategy {
		internal const int afterHeaderIndent = 10;
		public static SchedulerSinglePrintViewBuilderStrategy CreateStrategy(GraphicsInfo gInfo, SchedulerControl control, PrintStyleWithResourceOptions printStyle) {
			switch (printStyle.Kind) {
				case SchedulerPrintStyleKind.Daily:
					return new DailyPrintViewBuilderStrategy(gInfo, control, printStyle);
				case SchedulerPrintStyleKind.Weekly:
					if (((WeeklyPrintStyle)printStyle).ArrangeDays == ArrangeDaysKind.LeftToRight)
						return new WeeklyLeftToRightPrintViewBuilderStrategy(gInfo, control, printStyle);
					else
						return new WeeklyTopToBottomPrintViewBuilderStrategy(gInfo, control, printStyle);
				case SchedulerPrintStyleKind.Monthly:
					return new MonthlyPrintViewBuilderStrategy(gInfo, control, printStyle);
				default:
					XtraSchedulerDebug.Assert(false);
					return null;
			}
		}
		PrintStyleWithResourceOptions printStyle;
		GraphicsInfo gInfo;
		SchedulerControl control;
		public SchedulerSinglePrintViewBuilderStrategy(GraphicsInfo gInfo, SchedulerControl control, PrintStyleWithResourceOptions printStyle) {
			this.printStyle = printStyle;
			this.gInfo = gInfo;
			this.control = control;
		}
		public abstract PageLayout Layout { get; }
		protected internal virtual float SecondLineTextSizeMultiplier { get { return 1.0f; } }
		protected internal PrintStyleWithResourceOptions PrintStyle { get { return printStyle; } }
		internal GraphicsInfo GInfo { get { return gInfo; } }
		internal SchedulerControl Control { get { return control; } }
		public abstract ViewPart CalculateFirstViewPart();
		public virtual TimeInterval CalculateFirstInterval(DateTime startRangeDate, DateTime endRangeDate, DayOfWeek firstDayOfWeek) {
			TimeInterval interval = new TimeInterval(startRangeDate, endRangeDate);
			return AlignInterval(interval, firstDayOfWeek);
		}
		public virtual TimeInterval CalculateNextInterval(TimeInterval currentInterval) {
			DateTime newStart = currentInterval.Start + currentInterval.Duration;
			return new TimeInterval(newStart, currentInterval.Duration);
		}
		public virtual IPrintableObjectViewInfo CreateViewInfo(Rectangle bounds, TimeInterval interval, ViewPart part, ResourceBaseCollection resources) {
			TimeInterval alignedInterval = AlignInterval(interval, control.FirstDayOfWeek);
			CompositeViewInfo result = new CompositeViewInfo();
			IPrintableObjectViewInfo headerViewInfo = CreateHeaderViewInfo(bounds, alignedInterval, part);
			int headerHeight = 0;
			if (headerViewInfo != null) {
				result.AddChild(headerViewInfo);
				headerHeight = headerViewInfo.Bounds.Height;
			}
			if (headerHeight != 0)
				headerHeight += afterHeaderIndent;
			Rectangle contentBounds = RectUtils.CutFromTop(bounds, headerHeight);
			IPrintableObjectViewInfo viewInfo = CreateContentViewInfo(contentBounds, alignedInterval, part, resources);
			if (viewInfo != null)
				result.AddChild(viewInfo);
			return result;
		}
		protected internal virtual IPrintableObjectViewInfo CreateHeaderViewInfo(Rectangle pageBounds, TimeInterval interval, ViewPart part) {
			if (!PrintStyle.CalendarHeaderVisible)
				return null;
			HeaderPrintInfoCalculator calculator = CreateHeaderPrintInfoCalcualtor();
			HeaderPrintInfoCalculatorParameters calculatorParameters = new HeaderPrintInfoCalculatorParameters();
			calculatorParameters.FirstLineText = CalculateFirstLineText(interval);
			calculatorParameters.SecondLineText = CalculateSecondLineText(interval);
			calculatorParameters.Control = control;
			calculatorParameters.PageBounds = pageBounds;
			calculatorParameters.ViewPart = part;
			calculatorParameters.Interval = interval;
			calculatorParameters.Layout = Layout;
			calculatorParameters.SecondLineTextSizeMultiplier = SecondLineTextSizeMultiplier;
			calculatorParameters.AutoScaleHeadingsFont = PrintStyle.AutoScaleHeadingsFont;
			PreliminaryHeaderPrintInfo preliminaryInfo = calculator.CalculatePreliminaryHeaderInfo(calculatorParameters);
			return calculator.Calculate(preliminaryInfo, pageBounds, part, SecondLineTextSizeMultiplier);
		}
		protected internal virtual HeaderPrintInfoCalculator CreateHeaderPrintInfoCalcualtor() {
			return new HeaderPrintInfoCalculator(gInfo.Graphics, PrintStyle.HeadingsFont);
		}
		protected internal virtual IPrintableObjectViewInfo CreateContentViewInfo(Rectangle pageBounds, TimeInterval interval, ViewPart viewPart, ResourceBaseCollection resources) {
			SchedulerViewBase view = CreatePrintView(control, gInfo, viewPart);
			IViewAsyncSupport asyncSupport = view as IViewAsyncSupport;
			if (asyncSupport != null)
				asyncSupport.ForceSyncMode();
			view.Initialize();
			TimeIntervalCollection visibleIntervals = CalculateVisibleIntervals(interval, viewPart);
			view.SetVisibleIntervals(visibleIntervals);
			view.CreateViewInfo();
			SetViewParameters(view, interval);
			SetViewResources(view, resources);
			int appointmentHeight = PrintStyle.AppointmentHeight;
			if (appointmentHeight > 0) {
				view.AppointmentDisplayOptions.AppointmentAutoHeight = false;
				view.AppointmentDisplayOptions.AppointmentHeight = appointmentHeight;
			}
			view.InnerView.QueryAppointments();
			CalculateViewInfo(pageBounds, view);
			return view.ViewInfo;
		}
		protected internal virtual TimeIntervalCollection CalculateVisibleIntervals(TimeInterval currentInterval, ViewPart part) {
			TimeIntervalCollection visibleIntervals = new TimeIntervalCollection();
			visibleIntervals.Add(currentInterval);
			return visibleIntervals;
		}
		protected internal virtual void SetViewParameters(SchedulerViewBase view, TimeInterval currentInterval) {
			if (printStyle.ResourceOptions.UseActiveViewGroupType)
				view.GroupType = view.Control.GroupType;
			else
				view.GroupType = printStyle.ResourceOptions.GroupType;
		}
		protected internal virtual void SetViewResources(SchedulerViewBase view, ResourceBaseCollection resources) {
			view.VisibleResources.Clear();
			view.FilteredResources.Clear();
			view.VisibleResources.AddRange(resources);
			view.FilteredResources.AddRange(resources);
		}
		protected internal virtual void CalculateViewInfo(Rectangle pageBounds, SchedulerViewBase view) {
			view.RecreateViewInfo();
			view.ClearPreliminaryAppointmentsAndCellContainers();
			view.RecalcPreliminaryLayout(pageBounds);
			view.RecalcFinalLayout(pageBounds);
		}
		protected internal abstract TimeInterval AlignInterval(TimeInterval interval, DayOfWeek firstDayOfWeek);
		protected internal abstract string CalculateFirstLineText(TimeInterval currentInterval);
		protected internal abstract string CalculateSecondLineText(TimeInterval currentInterval);
		protected internal abstract SchedulerViewBase CreatePrintView(SchedulerControl control, GraphicsInfo gInfo, ViewPart viewPart);
	}
}
