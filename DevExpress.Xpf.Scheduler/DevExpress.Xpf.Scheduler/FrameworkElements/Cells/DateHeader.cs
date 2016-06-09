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
using System.Windows.Data;
using System.ComponentModel;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler;
namespace DevExpress.Xpf.Scheduler.Drawing {
	public class VisualDateHeader : VisualDateCell {
		public VisualDateHeader() {
			DefaultStyleKey = typeof(VisualDateHeader);
		}
		protected override XtraScheduler.Drawing.SchedulerHitTest HitTest { get { return SchedulerHitTest.DayHeader; } }
		#region DisableResourceColorProperty
		public static readonly DependencyProperty DisableResourceColorProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualDateHeader, bool>("DisableResourceColor", false);
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("VisualDateHeaderDisableResourceColor")]
#endif
		public bool DisableResourceColor { get { return (bool)GetValue(DisableResourceColorProperty); } set { SetValue(DisableResourceColorProperty, value); } }
		#endregion
		protected override XtraScheduler.Native.ISelectableIntervalViewInfo GetSelectableIntervalViewInfoCore(SchedulerControl control) {
			if (HitTest == SchedulerHitTest.None)
				return null;
			VisualDayViewColumn dayViewColumnContent = Content as VisualDayViewColumn;
			if (dayViewColumnContent != null) {
				TimeInterval contentInterval = new TimeInterval(dayViewColumnContent.IntervalStart, dayViewColumnContent.IntervalEnd);
				Resource resource = control.Storage.ResourceStorage.GetResourceById(dayViewColumnContent.ResourceId);
				return new VisualSelectableIntervalViewInfo(HitTest, contentInterval, resource, false);
			}
			VisualTimeCellBaseContent timeCellBaseContent = Content as VisualTimeCellBaseContent;
			if (timeCellBaseContent != null) {
				TimeInterval contentInterval = new TimeInterval(timeCellBaseContent.IntervalStart, timeCellBaseContent.IntervalEnd);
				Resource resource = control.Storage.ResourceStorage.GetResourceById(timeCellBaseContent.ResourceId);
				return new VisualSelectableIntervalViewInfo(HitTest, contentInterval, resource, false);
			}
			return null;
		}
	}
}
