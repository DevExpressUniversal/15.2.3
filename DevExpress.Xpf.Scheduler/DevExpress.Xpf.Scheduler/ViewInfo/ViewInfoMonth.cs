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
using System.Windows.Markup;
using System.Windows.Data;
using System.Windows.Controls;
using System.Globalization;
using System;
using System.Windows;
using DevExpress.Xpf.Core.Native;
using System.Collections.Specialized;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using DevExpress.Xpf.Scheduler.Drawing;
#endif
namespace DevExpress.Xpf.Scheduler.Native {
	#region MonthViewFactoryHelper
	public class MonthViewFactoryHelper : ViewFactoryHelper
	{
		public override ViewGroupTypeStrategy CreateGroupByNoneStrategy()
		{
			return new MonthViewGroupByNoneStrategy();
		}
		public override ViewGroupTypeStrategy CreateGroupByDateStrategy()
		{
			return new MonthViewGroupByDateStrategy();
		}
		public override ViewGroupTypeStrategy CreateGroupByResourceStrategy()
		{
			return new MonthViewGroupByResourceStrategy();
		}
		public override AppointmentBaseLayoutCalculator CreateAppointmentLayoutCalculator(ISchedulerViewInfoBase viewInfo, bool alternate)
		{
			return new MonthViewAppointmentLayoutCalculator(viewInfo);
		}
	}
	#endregion
	#region MonthViewGroupByNoneStrategy
	public class MonthViewGroupByNoneStrategy : ViewGroupTypeStrategy {
		public override IContinuousCellsInfosCalculator CreateVisuallyContinuousCellsInfosCalculator(bool alternate) {
			return new MonthViewGroupByNoneVisuallyContinuousCellsInfosCalculator();
		}
		public override ISchedulerViewInfoBase CreateViewInfo(SchedulerViewBase view) {
			return new MonthViewGroupByNone((MonthView)view);
		}
	}
	#endregion
	#region MonthViewGroupByDateStrategy
	public class MonthViewGroupByDateStrategy : ViewGroupTypeStrategy {
		public override IContinuousCellsInfosCalculator CreateVisuallyContinuousCellsInfosCalculator(bool alternate) {
			return new WeekViewGroupByDateVisuallyContinuousCellsInfosCalculator();
		}
		public override ISchedulerViewInfoBase CreateViewInfo(SchedulerViewBase view) {
			return new MonthViewGroupByDate((MonthView)view);
		}
	}
	#endregion
	#region MonthViewGroupByResourceStrategy
	public class MonthViewGroupByResourceStrategy : ViewGroupTypeStrategy {
		public override IContinuousCellsInfosCalculator CreateVisuallyContinuousCellsInfosCalculator(bool alternate) {
			return new MonthViewGroupByResourceVisuallyContinuousCellsInfosCalculator();
		}
		public override ISchedulerViewInfoBase CreateViewInfo(SchedulerViewBase view) {
			return new MonthViewGroupByResource((MonthView)view);
		}
	}
	#endregion
}
#if WPF
namespace DevExpress.Xpf.Scheduler.UI {
	public class ViewInfoConverter : MarkupExtension, IValueConverter {
		DataTemplateSelector templateSelector;
		public DataTemplateSelector TemplateSelector {
			get {
				return templateSelector;
			}
			set {
				templateSelector = value;
			}
		}
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			VisualViewInfoBase view = value as VisualViewInfoBase;
			if (view != null) {
				ContentPresenter presenter = new ContentPresenter();
				presenter.Content = view;
				presenter.ContentTemplateSelector = templateSelector;
				return presenter;
			}
			else
				return null;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}
	}
}
#endif
