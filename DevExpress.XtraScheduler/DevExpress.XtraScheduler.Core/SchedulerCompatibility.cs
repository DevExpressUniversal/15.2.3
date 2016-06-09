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

using DevExpress.XtraScheduler.Internal.Implementations;
using System;
using System.Linq;
namespace DevExpress.XtraScheduler {
	public static class SchedulerCompatibility {
		static SchedulerCompatibility() {
			Base64XmlObjectSerialization = true;
		}
		public static bool Base64XmlObjectSerialization { get; set; }
	}
}
namespace DevExpress.XtraScheduler.Compatibility {
	public abstract class CompatibleAppointment : Internal.Implementations.AppointmentInstance {
	}
	public static class StaticAppointmentFactory {
		public static Appointment CreateAppointment(AppointmentType type) {
			return new AppointmentInstance(type);
		}
		public static Appointment CreateAppointment(AppointmentType type, DateTime start, DateTime end) {
			return new AppointmentInstance(type, start, end);
		}
		public static Appointment CreateAppointment(AppointmentType type, DateTime start, TimeSpan duration) {
			return new AppointmentInstance(type, start, duration);
		}
		public static Appointment CreateAppointment(AppointmentType type, DateTime start, DateTime end, string subject) {
			return new AppointmentInstance(type, start, end, subject);
		}
		public static Appointment CreateAppointment(AppointmentType type, DateTime start, TimeSpan duration, string subject) {
			return new AppointmentInstance(type, start, duration, subject);
		}
	}
}
