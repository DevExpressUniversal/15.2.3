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
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Services;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using System.Collections.Generic;
namespace DevExpress.XtraScheduler {
	#region WinAppointmentSelectionController
	public class WinAppointmentSelectionController : AppointmentSelectionController {
		#region Fields
		SchedulerControl control;
		#endregion
		public WinAppointmentSelectionController(SchedulerControl control, bool allowAppointmentMultiSelect)
			: base(allowAppointmentMultiSelect) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
		}
		protected internal SchedulerControl Control { get { return control; } }
		public override void OnVisibleIntervalChanged() {
			SchedulerViewInfoBase viewInfo = Control.ActiveView.ViewInfo;
			if (viewInfo == null) {
				base.OnVisibleIntervalChanged();
				return;
			}
			ISchedulerStateService stateService = Control.GetService<ISchedulerStateService>();
			if (stateService != null && stateService.AreAppointmentsDragged)
				return;
			TimelineView timelineView = Control.ActiveView as TimelineView;
			if (timelineView != null && timelineView.OptionsSelectionBehavior.KeepSelectedAppointments) 
				return;
			SelectedAppointments.BeginUpdate();
			try {
				int count = SelectedAppointments.Count;
				IEnumerable<AppointmentViewInfo> allViewInfos = viewInfo.CopyAllAppointmentViewInfos();
				for (int i = count - 1; i >= 0; i--) {
					if (!this.CanKeepAppointmentSelected(i, allViewInfos))
						SelectedAppointments.RemoveAt(i);
				}
			}
			finally {
				SelectedAppointments.EndUpdate();
			}
		}
		protected internal virtual bool CanKeepAppointmentSelected(int selectedAppointmentIndex, IEnumerable<AppointmentViewInfo> aptViewInfos) {
			Appointment apt = SelectedAppointments[selectedAppointmentIndex];
			foreach (AppointmentViewInfo vi in aptViewInfos) {
				Appointment actualAppointment = vi.Appointment;
				if (AreSameAppointments(apt, actualAppointment)) {
					if (!Object.ReferenceEquals(apt, actualAppointment))
						ReplaceOccurrenceSilently(selectedAppointmentIndex, actualAppointment);
					return true;
				}
			}
			return false;
		}
		protected internal virtual bool AreSameAppointments(Appointment firstAppointment, Appointment secondAppointment) {
			if (firstAppointment.Type == AppointmentType.Occurrence && secondAppointment.Type == AppointmentType.Occurrence) {
				bool arePatternsEqual = (firstAppointment.RecurrencePattern == secondAppointment.RecurrencePattern);
				bool areIndexesEqual = (firstAppointment.RecurrenceIndex == secondAppointment.RecurrenceIndex);
				return arePatternsEqual && areIndexesEqual;
			}
			else
				return firstAppointment == secondAppointment;
		}
		protected internal virtual void ReplaceOccurrenceSilently(int selectedAppointmentIndex, Appointment newOccurrence) {
			XtraSchedulerDebug.Assert(SelectedAppointments[selectedAppointmentIndex].Type == AppointmentType.Occurrence);
			XtraSchedulerDebug.Assert(newOccurrence.Type == AppointmentType.Occurrence);
			DevExpress.Internal.DXCollectionAccessor.GetInnerList(SelectedAppointments)[selectedAppointmentIndex] = newOccurrence;
		}
	}
	#endregion
}
