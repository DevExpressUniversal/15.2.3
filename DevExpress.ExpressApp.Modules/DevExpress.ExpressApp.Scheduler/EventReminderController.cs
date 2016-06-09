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

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.Persistent.Base.General;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.ExpressApp.Scheduler {
	public class NotificationsEventController : ObjectViewController<DetailView, IEvent> {
		protected DateTime cachedStartOnForAllDay;
		protected DateTime cachedEndOnForAllDay;
		protected bool cachedAllDayValue;
		private void ObjectSpace_ObjectChanged(object sender, ObjectChangedEventArgs e) {
			if(ViewCurrentObject != null) {
				if(e.PropertyName == "AllDay" && cachedAllDayValue != ViewCurrentObject.AllDay) {
					ChangeEventStartAndEndTime();
					cachedAllDayValue = ViewCurrentObject.AllDay;
				}
				else {
					IReminderEvent reminderEvent = ViewCurrentObject as IReminderEvent;
					if(reminderEvent != null) {
						if(e.PropertyName == "RemindIn") {
							if(reminderEvent.RemindIn.HasValue) {
								reminderEvent.AlarmTime = ViewCurrentObject.StartOn - reminderEvent.RemindIn.Value;
							}
							else {
								reminderEvent.AlarmTime = null;
							}
						}
						else if(e.PropertyName == "StartOn") {
							if(reminderEvent.RemindIn.HasValue) {
								reminderEvent.AlarmTime = ViewCurrentObject.StartOn - reminderEvent.RemindIn.Value;
							}
						}
						else if(e.PropertyName == "ReminderTime") {
							ObjectSpace.SetModified(ViewCurrentObject);
						}
					}
				}
			}
		}
		private void ChangeEventStartAndEndTime() {
			if(ViewCurrentObject.AllDay == true) {
				cachedStartOnForAllDay = ViewCurrentObject.StartOn;
				cachedEndOnForAllDay = ViewCurrentObject.EndOn;
				ViewCurrentObject.StartOn = ViewCurrentObject.StartOn.Date;
				ViewCurrentObject.EndOn = ViewCurrentObject.StartOn.Date.Add(TimeSpan.FromDays(1));
			}
			else {
				ViewCurrentObject.StartOn = (cachedStartOnForAllDay == DateTime.MinValue) ? ViewCurrentObject.StartOn : cachedStartOnForAllDay;
				ViewCurrentObject.EndOn = (cachedEndOnForAllDay == DateTime.MinValue) ? ViewCurrentObject.EndOn : cachedEndOnForAllDay;
			}
		}
		private void View_CurrentObjectChanged(object sender, EventArgs e) {
			if(View.CurrentObject != null) {
				cachedAllDayValue = ViewCurrentObject.AllDay;
				cachedStartOnForAllDay = DateTime.MinValue;
				cachedEndOnForAllDay = DateTime.MinValue;
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			View.CurrentObjectChanged += View_CurrentObjectChanged;
			View.ObjectSpace.ObjectChanged += ObjectSpace_ObjectChanged;
			if(View.CurrentObject != null) {
				cachedAllDayValue = ViewCurrentObject.AllDay;
			}
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			View.CurrentObjectChanged -= View_CurrentObjectChanged;
			View.ObjectSpace.ObjectChanged -= ObjectSpace_ObjectChanged;
		}
	}
}
