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
using System.Drawing;
using DevExpress.Utils.Drawing;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Services;
using DevExpress.XtraScheduler.Services.Internal;
namespace DevExpress.XtraScheduler.Drawing {
	public interface ISupportWeekCells {
		bool CompressWeekend { get; }
		DayOfWeek FirstDayOfWeek { get; }
		WeekViewAppearance PaintAppearance { get; }
		HeaderCaptionFormatProviderBase GetCaptionFormatProvider();
		bool ShouldHideCellContent(SchedulerViewCellBase cell);
	}
	public class WeekViewInfo : SchedulerViewInfoBase, ISupportWeekCells {
		HeaderCaptionFormatProvider captionFormatProvider;
		bool compressWeekend = true;
		SchedulerHeaderCollection corners = new SchedulerHeaderCollection();
		DayOfWeek firstDayOfWeek;
		bool showWeekend = true;
		SchedulerHeaderCollection weekDaysHeaders = new SchedulerHeaderCollection();
		public WeekViewInfo(WeekView view)
			: base(view) {
			IHeaderCaptionService service = (IHeaderCaptionService)View.Control.GetService(typeof(IHeaderCaptionService));
			this.captionFormatProvider = new HeaderCaptionFormatProvider(service);
		}
		#region Properties
		public new WeekView View { get { return (WeekView)base.View; } }
		public new WeekViewAppearance PaintAppearance { get { return (WeekViewAppearance)base.PaintAppearance; } }
		public SchedulerHeaderCollection WeekDaysHeaders { get { return weekDaysHeaders; } }
		public SchedulerHeaderCollection Corners { get { return corners; } }
		public bool CompressWeekend {
			get { return compressWeekend; }
			set {
				if (compressWeekend == value)
					return;
				compressWeekend = value;
				OnCompressWeekendChanged();
			}
		}
		public bool ShowWeekend { get { return showWeekend; } set { showWeekend = value; } }
		public DayOfWeek FirstDayOfWeek { get { return firstDayOfWeek; } set { firstDayOfWeek = value; } }
		public SchedulerViewCellContainerCollection Weeks { get { return CellContainers; } }
		internal new WeekViewPreliminaryLayoutResult PreliminaryLayoutResult { get { return (WeekViewPreliminaryLayoutResult)base.PreliminaryLayoutResult; } }	 
		#endregion
		#region IDisposable implementation
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (weekDaysHeaders != null) {
						DisposeWeekDaysHeaders();
						weekDaysHeaders = null;
					}
					if (corners != null) {
						DisposeCorners();
						corners = null;
					}
				}
			} finally {
				base.Dispose(disposing);
			}
		}
		#endregion
		protected virtual void OnCompressWeekendChanged() {
		}
		protected internal override SchedulerHitInfo CalculateLayoutHitInfo(Point pt) {
			SchedulerHitInfo baseHitInfo = base.CalculateLayoutHitInfo(pt);
			if (baseHitInfo.HitTest != SchedulerHitTest.None)
				return baseHitInfo;
			return CalculateHeaderCollectionHitInfo(pt, WeekDaysHeaders);
		}
		protected internal virtual VerticalSingleWeekViewInfo CreateResourceWeek(Resource resource, DateTime start, SchedulerColorSchema colorSchema) {
			return new VerticalSingleWeekViewInfo(this, resource, start, colorSchema);
		}
		protected internal virtual void DisposeCorners() {
			for (int i = 0; i < this.corners.Count; i++) {
				this.corners[i].Dispose();
			}
			this.corners.Clear();
			this.corners = null;
		}
		protected internal virtual void DisposeWeekDaysHeaders() {
			for (int i = 0; i < this.weekDaysHeaders.Count; i++) {
				this.weekDaysHeaders[i].Dispose();
			}
			this.weekDaysHeaders.Clear();
			this.weekDaysHeaders = null;
		}
		protected internal override void ExecuteAppointmentLayoutCalculatorCore(GraphicsCache cache) {
			AppointmentsLayoutStrategyFixedHeightCells layoutStrategy = (AppointmentsLayoutStrategyFixedHeightCells)View.FactoryHelper.CreateAppointmentsLayoutStrategy(View);
			layoutStrategy.CalculateLayout(cache);
		}
		protected internal virtual DayOfWeek[] GetActualWeekDays(DayOfWeek[] dayOfWeek) {
			return dayOfWeek;
		}
		protected internal SchedulerHeaderCollection GetResourceWeekDaysHeaders(Resource resource) {
			SchedulerHeaderCollection result = new SchedulerHeaderCollection();
			int count = WeekDaysHeaders.Count;
			for (int i = 0; i < count; i++) {
				SchedulerHeader header = WeekDaysHeaders[i];
				if (Object.Equals(header.Resource, resource))
					result.Add(header);
			}
			return result;
		}
		protected internal virtual DateTime[] GetWeekDates(DateTime start) {
			return DateTimeHelper.GetWeekDates(start, FirstDayOfWeek, CompressWeekend, ShowWeekend);
		}
		protected internal virtual bool ShouldHideCellContent(SchedulerViewCellBase cell) {
			return false;
		}
		#region IWeekCellsSupport implementation
		bool ISupportWeekCells.ShouldHideCellContent(SchedulerViewCellBase cell) {
			return ShouldHideCellContent(cell);
		}
		HeaderCaptionFormatProviderBase ISupportWeekCells.GetCaptionFormatProvider() { return captionFormatProvider; }
		#endregion
		protected internal override ViewInfoBasePreliminaryLayoutResult CreatePreliminaryLayoutResult() {
			return new WeekViewPreliminaryLayoutResult();
		}
	}
}
