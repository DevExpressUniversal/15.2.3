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
using DevExpress.XtraScheduler.Native;
namespace DevExpress.Xpf.Scheduler.Native {
	public class WorkWeekViewPropertySyncManager : DayViewPropertySyncManager {
		public WorkWeekViewPropertySyncManager(WorkWeekView view)
			: base((WorkWeekView)view) {
		}
		public new WorkWeekView View { get { return (WorkWeekView)base.View; } }
		public override void Register() {
			base.Register();
			PropertyMapperTable.RegisterPropertyMapper(WorkWeekView.ShowFullWeekProperty, new WorkWeekShowFullWeekPropertyMapper(WorkWeekView.ShowFullWeekProperty, View));
		}
	}
	#region WorkWeekView mappers
	public abstract class WorkWeekViewPropertyMapperBase : SchedulerViewPropertyMapperBase<WorkWeekView, InnerWorkWeekView> {
		protected WorkWeekViewPropertyMapperBase(DependencyProperty property, WorkWeekView view)
			: base(property, view) {
		}
	}
	public class WorkWeekShowFullWeekPropertyMapper : WorkWeekViewPropertyMapperBase {
		public WorkWeekShowFullWeekPropertyMapper(DependencyProperty property, WorkWeekView view)
			: base(property, view) {
		}
		protected override bool CanUpdateOwnerProperty(SchedulerControlChangeType changeType) {
			return changeType == SchedulerControlChangeType.ShowFullWeekChanged;
		}
		public override object GetInnerPropertyValue() {
			return InnerView.ShowFullWeek;
		}
		protected override void SetInnerPropertyValue(object oldValue, object newValue) {
			InnerView.ShowFullWeek = (bool)newValue;
		}
	}
	#endregion
}
