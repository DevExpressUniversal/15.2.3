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

using DevExpress.XtraScheduler;
using System;
using System.Windows;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Scheduler.Drawing {
	public class VisualDayViewResource : VisualResource {
		#region ResourceAllDayArea
		static readonly DependencyPropertyKey ResourceAllDayAreaPropertyKey = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterReadOnlyProperty<VisualDayViewResource, VisualResourceAllDayArea>("ResourceAllDayArea", null);
		public static readonly DependencyProperty ResourceAllDayAreaProperty = ResourceAllDayAreaPropertyKey.DependencyProperty;
		public VisualResourceAllDayArea ResourceAllDayArea { get { return (VisualResourceAllDayArea)GetValue(ResourceAllDayAreaProperty); } protected set { this.SetValue(ResourceAllDayAreaPropertyKey, value); } }
		#endregion
		#region TimeIndicatorVisibility
		public static readonly DependencyProperty TimeIndicatorVisibilityProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualDayViewResource, TimeIndicatorVisibility>("TimeIndicatorVisibility", TimeIndicatorVisibility.TodayView);
		public TimeIndicatorVisibility TimeIndicatorVisibility {
			get { return (TimeIndicatorVisibility)GetValue(TimeIndicatorVisibilityProperty); }
			set { SetValue(TimeIndicatorVisibilityProperty, value); }
		}
		#endregion
		protected override VisualSimpleResourceInterval CreateVisualSimpleResourceInterval() {
			return new VisualDayViewColumn();
		}
		protected override void CopyFromCore(SingleResourceViewInfo source) {
			base.CopyFromCore(source);
			DayBasedSingleResourceViewInfo dayBasedSource = (DayBasedSingleResourceViewInfo)source;
			if (dayBasedSource.HorizontalCellContainer == null) {
				ResourceAllDayArea = null;
				return;
			}
			if (ResourceAllDayArea == null)
				ResourceAllDayArea = new VisualResourceAllDayArea();
			((ISupportCopyFrom<ICellContainer>)ResourceAllDayArea).CopyFrom(dayBasedSource.HorizontalCellContainer);
			TimeIndicatorVisibility = dayBasedSource.TimeIndicatorVisibility;
		}
		public override void CopyAppointmentsFrom(SingleResourceViewInfo source) {
			base.CopyAppointmentsFrom(source);
			if (ResourceAllDayArea == null)
				return;
			ResourceAllDayArea.CopyAppointmentsFrom(((DayBasedSingleResourceViewInfo)source).HorizontalCellContainer);
		}
		protected override CellContainerCollection GetCellContainers(SingleResourceViewInfo source) {
			DayBasedSingleResourceViewInfo dayBasedSource = (DayBasedSingleResourceViewInfo)source;
			return dayBasedSource.VerticalCellContainers;
		}
		protected override VisualResourceHeaderBaseContent CreateResourceHeaderCore() {
			return new VisualResourceHeaderContent();
		}
	}
}
