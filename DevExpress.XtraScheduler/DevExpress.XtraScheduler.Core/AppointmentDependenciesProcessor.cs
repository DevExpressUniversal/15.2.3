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
using DevExpress.Utils;
using DevExpress.XtraScheduler.Internal;
namespace DevExpress.XtraScheduler.Native {
	#region EmptyAppointmentDependencyFilterPredicate
	public class EmptyAppointmentDependencyFilterPredicate : SchedulerEmptyPredicate<AppointmentDependency> {
	}
	#endregion
	#region CompositeAppointmentDependencyPredicate
	public class CompositeAppointmentDependencyPredicate : SchedulerCompositePredicate<AppointmentDependency> {
	}
	#endregion
	#region FilterAppointmentDependencyViaStorageEventPredicate
	public class FilterAppointmentDependencyViaStorageEventPredicate : FilterObjectViaStorageEventPredicate<AppointmentDependency> {
		public FilterAppointmentDependencyViaStorageEventPredicate(SchedulerStorageBase storage)
			: base(storage) {
		}
		public override bool Calculate(AppointmentDependency obj) {
			return ((IInternalSchedulerStorageBase)Storage).RaiseFilterDependency(obj);
		}
	}
	#endregion
	#region FilterAppointmentDependencyPredicate
	public class FilterAppointmentDependencyPredicate : FilterObjectViaFilterCriteriaPredicate<AppointmentDependency> {
		public FilterAppointmentDependencyPredicate(IAppointmentDependencyStorage dependencyStorage)
			: base(dependencyStorage) {
		}
	}
	#endregion
	#region AppointmentDependencyFilter
	public class AppointmentDependencyFilter : SimpleProcessorBase<AppointmentDependency> {
		public AppointmentDependencyFilter(IPredicate<AppointmentDependency> predicate)
			: base(predicate) {
		}
		protected internal override NotificationCollection<AppointmentDependency> CreateDestinationCollection() {
			return new AppointmentDependencyCollection();
		}
	}
	#endregion
}
