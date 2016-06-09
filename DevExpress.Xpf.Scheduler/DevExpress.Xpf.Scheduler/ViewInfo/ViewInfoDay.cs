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

#if WPF||SL
using DevExpress.Xpf.Scheduler;
using DevExpress.Xpf.Scheduler.Drawing;
#endif
namespace DevExpress.Xpf.Scheduler.Native {
	#region DayViewFactoryHelper
	public class DayViewFactoryHelper : ViewFactoryHelper
	{
		public override ViewGroupTypeStrategy CreateGroupByNoneStrategy()
		{
			return new DayViewGroupByNoneStrategy();
		}
		public override ViewGroupTypeStrategy CreateGroupByDateStrategy()
		{
			return new DayViewGroupByDateStrategy();
		}
		public override ViewGroupTypeStrategy CreateGroupByResourceStrategy()
		{
			return new DayViewGroupByResourceStrategy();
		}
		public override AppointmentBaseLayoutCalculator CreateAppointmentLayoutCalculator(ISchedulerViewInfoBase viewInfo, bool alternate)
		{
			if (alternate)
				return new DayViewLongAppointmentLayoutCalculator(viewInfo);
			else
				return new DayViewShortAppointmentLayoutCalculator(viewInfo);
		}
	}
	#endregion
	#region DayViewGroupByNoneStrategy
	public class DayViewGroupByNoneStrategy : ViewGroupTypeStrategy {
		public override IContinuousCellsInfosCalculator CreateVisuallyContinuousCellsInfosCalculator(bool alternate) {
			if (alternate)
				return new DayViewGroupByNoneAllDayAreaVisuallyContinuousCellsInfosCalculator();
			else
				return new DayViewGroupByNoneVisuallyContinuousCellsInfosCalculator();
		}
		public override ISchedulerViewInfoBase CreateViewInfo(SchedulerViewBase view) {
			return new DayViewGroupByNone((DayView)view);
		}
	}
	#endregion
	#region DayViewGroupByDateStrategy
	public class DayViewGroupByDateStrategy : ViewGroupTypeStrategy {
		public override IContinuousCellsInfosCalculator CreateVisuallyContinuousCellsInfosCalculator(bool alternate) {
			if (alternate)
				return new DayViewGroupByDateAllDayAreaVisuallyContinuousCellsInfosCalculator();
			else
			   return new DayViewGroupByDateVisuallyContinuousCellsInfosCalculator();
		}
		public override ISchedulerViewInfoBase CreateViewInfo(SchedulerViewBase view) {
			return new DayViewGroupByDate((DayView)view);
		}
	}
	#endregion
	#region DayViewGroupByResourceStrategy
	public class DayViewGroupByResourceStrategy : ViewGroupTypeStrategy {
		public override IContinuousCellsInfosCalculator CreateVisuallyContinuousCellsInfosCalculator(bool alternate) {
			if (alternate)
				return new DayViewGroupByResourceAllDayAreaVisuallyContinuousCellsInfosCalculator();
			else
				return new DayViewGroupByResourceVisuallyContinuousCellsInfosCalculator();
		}
		public override ISchedulerViewInfoBase CreateViewInfo(SchedulerViewBase view) {
			return new DayViewGroupByResource((DayView)view);
		}
	}
	#endregion
}
