#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using DevExpress.Persistent.Base.General;
using DevExpress.XtraScheduler;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.Scheduler.Win {
	public class SourceObjectHelperWin : SourceObjectHelperBase {
		private ISchedulerStorageBase schedulerStorage;
		public SourceObjectHelperWin(ISchedulerStorageBase schedulerStorage, IObjectSpace objectSpace) {
			Guard.ArgumentNotNull(schedulerStorage, "schedulerStorage");
			Guard.ArgumentNotNull(objectSpace, "objectSpace");
			this.schedulerStorage = schedulerStorage;
			this.objectSpace = objectSpace;
		}
		public override object GetObjectInCollectionSource(object targetObject) {
			if(targetObject is XafDataViewRecord) {
				return targetObject;
			}
			else {
				return GetSourceObject(targetObject);
			}
		}
		public override IEvent GetSourceObject(object targetObject) {
			IEvent result = null;
			if(targetObject is IEvent) {
				result = (IEvent)targetObject;
			}
			else if(targetObject is XafDataViewRecord) {
				result = objectSpace.GetObject(targetObject) as IEvent;
			}
			else if(targetObject is Appointment) {
				Appointment appointment = (Appointment)targetObject;
				Object sourceObject = appointment.GetSourceObject(schedulerStorage);
				result = objectSpace.GetObject(sourceObject) as IEvent;
			}
			return result;
		}
	}
}
