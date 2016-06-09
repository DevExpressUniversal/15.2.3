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

using System.Windows;
using System;
using System.Collections.ObjectModel;
using DevExpress.XtraScheduler.Drawing;
using System.Windows.Media;
using System.Windows.Controls;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler;
using System.ComponentModel;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Scheduler.Drawing {
	public class VisualMonthViewGroupByNone : VisualResourcesBasedViewInfo {
		#region DayOfWeekHeaders
		static readonly DependencyPropertyKey DayOfWeekHeadersPropertyKey = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterReadOnlyProperty<VisualMonthViewGroupByNone, VisualDayOfWeekHeaderContentCollection>("DayOfWeekHeaders", null);
		public static readonly DependencyProperty DayOfWeekHeadersProperty = DayOfWeekHeadersPropertyKey.DependencyProperty;
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("VisualMonthViewGroupByNoneDayOfWeekHeaders")]
#endif
		public VisualDayOfWeekHeaderContentCollection DayOfWeekHeaders { get { return (VisualDayOfWeekHeaderContentCollection)GetValue(DayOfWeekHeadersProperty); } protected set { this.SetValue(DayOfWeekHeadersPropertyKey, value); } }
		#endregion
		protected override VisualResource CreateVisualResource() {
			return new VisualHorizontalWeekViewResource();					   
		}
		protected override void CopyFromCore(ISchedulerViewInfoBase source) {
			base.CopyFromCore(source);
			MonthViewGroupByNone groupByNone = source as MonthViewGroupByNone;
			if (ResourceContainers == null)
				ResourceContainers = new VisualResourcesCollection();
			CollectionCopyHelper.Copy(ResourceContainers, groupByNone.ResourcesContainers, CreateVisualResource);
			if (DayOfWeekHeaders == null)
				DayOfWeekHeaders = new VisualDayOfWeekHeaderContentCollection();
			CollectionCopyHelper.Copy(DayOfWeekHeaders, groupByNone.DayOfWeekHeaders, CreateVisualDayOfWeekHeader);			
		}
		protected virtual VisualDayOfWeekHeaderContent CreateVisualDayOfWeekHeader() {			
			VisualDayOfWeekHeaderContent result = new VisualDayOfWeekHeaderContent();			
			return result;
		}
		protected override void CopyAppointmentsViewInfoCore(ISchedulerViewInfoBase source) {
			base.CopyAppointmentsViewInfoCore(source);
			if (ResourceContainers == null)
				return;
			MonthViewGroupByNone groupByNone = source as MonthViewGroupByNone;
			int count = groupByNone.ResourcesContainers.Count;
			for (int i = 0; i < count; i++)
				ResourceContainers[i].CopyAppointmentsFrom(groupByNone.ResourcesContainers[i]);
		}   
	}
}
