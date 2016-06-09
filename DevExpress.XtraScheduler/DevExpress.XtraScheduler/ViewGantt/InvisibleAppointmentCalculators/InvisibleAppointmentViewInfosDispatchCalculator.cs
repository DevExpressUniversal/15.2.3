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

using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraScheduler.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Drawing {
	public class InvisibleAppointmentViewInfosDispatchCalculator {
		GanttViewInfo viewInfo;
		public InvisibleAppointmentViewInfosDispatchCalculator(GanttViewInfo view) {
			Guard.ArgumentNotNull(view, "view");
			this.viewInfo = view;
		}
		internal GanttViewInfo ViewInfo { get { return viewInfo; } }
		internal InnerSchedulerControl InnerControl { get { return ViewInfo.InnerControl; } }
		public virtual AppointmentViewInfoCollection Calculate(HashSet<object> invisibleAppointmentIdCollection, GraphicsCache cache) {
			if (invisibleAppointmentIdCollection.Count == 0)
				return new AppointmentViewInfoCollection();
			AppointmentBaseCollection nonRecurringAppointments = InnerControl.GetNonRecurringAppointments();
			InvisibleAppointmentsCalculator invisibleAptCalculator = new InvisibleAppointmentsCalculator(ViewInfo);
			AppointmentBaseCollection invisibleAppointments = invisibleAptCalculator.Calculate(invisibleAppointmentIdCollection, nonRecurringAppointments);
			invisibleAppointments = this.ViewInfo.View.Control.AppointmentChangeHelper.GetActualAppointments(invisibleAppointments);
			return CalculateInvisibleAppointmentViewInfosCore(invisibleAppointments, cache);
		}
		protected internal virtual AppointmentViewInfoCollection CalculateInvisibleAppointmentViewInfosCore(AppointmentBaseCollection invisibleAppointments, GraphicsCache cache) {
			AppointmentViewInfoCollection result = new AppointmentViewInfoCollection();
			TimeInterval visibleInterval = ViewInfo.VisibleIntervals.Interval;
			VisuallyContinuousCellsInfoCollection cellsInfos = CalculateCellsInfos();
			if (cellsInfos.Count == 0)
				return result;
			VisibleIntervalAppointmentsSeparator separator = new VisibleIntervalAppointmentsSeparator(visibleInterval, ViewInfo.TimeZoneHelper);
			separator.Process(invisibleAppointments);
			AppointmentViewInfoCollection viewInfos = CalculateInvisibleViewInfosInVisibleInterval(separator.DestinationCollection, cellsInfos, cache);
			result.AddRange(viewInfos);
			viewInfos = CalculateInvisibleViewInfosOutsideVisibleInterval(separator.InvisibleAppointmentsBefore, cellsInfos, cache);
			result.AddRange(viewInfos);
			viewInfos = CalculateInvisibleViewInfosOutsideVisibleInterval(separator.InvisibleAppointmentsAfter, cellsInfos, cache);
			result.AddRange(viewInfos);
			return result;
		}
		protected internal virtual AppointmentViewInfoCollection CalculateInvisibleViewInfosInVisibleInterval(AppointmentBaseCollection appointments, VisuallyContinuousCellsInfoCollection cellsInfos, GraphicsCache cache) {
			InvisibleAppointmentViewInfosCalculatorBase calculator = new InvisibleAppointmentViewInfosAtVisibleIntervalCalculator(ViewInfo);
			return calculator.Calculate(appointments, cellsInfos, cache);
		}
		protected internal virtual AppointmentViewInfoCollection CalculateInvisibleViewInfosOutsideVisibleInterval(AppointmentBaseCollection appointments, VisuallyContinuousCellsInfoCollection cellsInfos, GraphicsCache cache) {
			InvisibleAppointmentViewInfosOutsideVisibleIntervalCalculator calculator = new InvisibleAppointmentViewInfosOutsideVisibleIntervalCalculator(ViewInfo);
			return calculator.Calculate(appointments, cellsInfos, cache);
		}
		protected internal virtual VisuallyContinuousCellsInfoCollection CalculateCellsInfos() {
			TimelineViewAppointmentsLayoutFixedHeightStrategy layoutStrategy = new TimelineViewAppointmentsLayoutFixedHeightStrategy(ViewInfo.View);
			ResourceVisuallyContinuousCellsInfosCollection resourceCellsInfo = layoutStrategy.CalculateFinalCellsInfos();
			return resourceCellsInfo.MergeCellInfos();
		}
	}
}
