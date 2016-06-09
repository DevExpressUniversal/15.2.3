﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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

using DevExpress.XtraScheduler.Commands;
using DevExpress.XtraScheduler.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Commands {
	public class SelectNextAppointmentCommand : SelectPrevNextAppointmentCommand {
		public SelectNextAppointmentCommand(ISchedulerCommandTarget control)
			: base(control) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.SelectNextAppointment; } }
		protected internal override int CalcCurrentSelectedAppointmentViewInfoIndex(AppointmentViewInfoCollection selectedAppointmentViewInfos) {
			return selectedAppointmentViewInfos.Count - 1;
		}
		protected internal override AppointmentViewInfo CalculateNewDefaultSelectedAppointmentViewInfo(AppointmentViewInfoCollection visibleAppointmentViewInfos) {
			return visibleAppointmentViewInfos[0];
		}
		protected internal override int CalcNewSelectedAppointmentViewInfoIndex(AppointmentViewInfoCollection visibleAppointmentViewInfos, int currentSelectedAppointmentViewInfoIndex) {
			int result = currentSelectedAppointmentViewInfoIndex;
			Appointment apt = visibleAppointmentViewInfos[currentSelectedAppointmentViewInfoIndex].Appointment;
			for (; ; ) {
				result++;
				int count = visibleAppointmentViewInfos.Count;
				if (result >= count)
					return -1;
				else {
					if (visibleAppointmentViewInfos[result].Appointment != apt)
						return result;
				}
			}
		}
		protected internal override AppointmentViewInfo FindNearestAppointmentViewInfo(AppointmentViewInfoCollection appointmentViewInfos, TimeInterval interval) {
			int count = appointmentViewInfos.Count;
			if (count <= 0)
				return null;
			for (int i = 0; i < count; i++) {
				if (appointmentViewInfos[i].Interval.Start >= interval.Start)
					return appointmentViewInfos[i];
			}
			return appointmentViewInfos[0];
		}
	}
}
