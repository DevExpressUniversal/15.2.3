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
using System.Collections.Generic;
using System.Text;
using DevExpress.Utils.Localization;
using System.Resources;
using DevExpress.Utils.Localization.Internal;
namespace DevExpress.XtraScheduler.Localization {
	#region SchedulerExtensionsStringId
	public enum SchedulerExtensionsStringId {
		Caption_RecurringAppointment,
		Caption_RecurringEvent,
		Caption_Appointment,
		Caption_Event,
		Caption_ViewSelector,
		Msg_AppointmentOccurs,
		Msg_AppointmentOccursInThePast,
		Msg_AllOccurrencesInThePast,
		Msg_AppointmentConflictsWithAnother,
		Msg_PatternOccurrencesConflictsWithOtherAppointments,
		Msg_Some,
		Caption_ViewNavigator,
		CaptionViewNavigator_Today,
		Caption_PageHome,
		Caption_PageFile,
		Caption_PageView,
		Caption_PageAppointment,
		Caption_GroupArrange,
		Caption_GroupActiveView,
		Caption_GroupTimeScale,
		Caption_GroupNavigator,
		Caption_GroupGroupBy,
		Caption_GroupAppointment,
		Caption_GroupCommon,
		Caption_GroupActions,
		Caption_GroupOptions,
		Caption_GroupPrint,
		Caption_GroupLayout,
		Caption_PageCategoryCalendarTools,
		Caption_Reminder
	}
	#endregion
	#region SchedulerExtensionsResLocalizer
	public class SchedulerExtensionsResLocalizer : XtraResXLocalizer<SchedulerExtensionsStringId> {
		public SchedulerExtensionsResLocalizer()
			: base(new SchedulerExtensionsLocalizer()) {
		}
		protected override ResourceManager CreateResourceManagerCore() {
			return new ResourceManager("DevExpress.XtraScheduler.LocalizationRes", typeof(SchedulerExtensionsResLocalizer).Assembly);
		}
	}
	#endregion
	#region SchedulerExtensionsLocalizer
	public class SchedulerExtensionsLocalizer : XtraLocalizer<SchedulerExtensionsStringId> {
		static SchedulerExtensionsLocalizer() {
			SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<SchedulerExtensionsStringId>(CreateDefaultLocalizer()));
		}
		public static new XtraLocalizer<SchedulerExtensionsStringId> Active {
			get { return XtraLocalizer<SchedulerExtensionsStringId>.Active; }
			set { XtraLocalizer<SchedulerExtensionsStringId>.Active = value; }
		}
		#region PopulateStringTable
		protected override void PopulateStringTable() {
			AddString(SchedulerExtensionsStringId.Caption_RecurringAppointment, "Recurring Appointment");
			AddString(SchedulerExtensionsStringId.Caption_RecurringEvent, "Recurring Event");
			AddString(SchedulerExtensionsStringId.Caption_Appointment, "Appointment");
			AddString(SchedulerExtensionsStringId.Caption_Event, "Event");
			AddString(SchedulerExtensionsStringId.Caption_ViewSelector, "View Selector");
			AddString(SchedulerExtensionsStringId.Msg_AppointmentOccurs, "Occurs {0}.\n");
			AddString(SchedulerExtensionsStringId.Msg_AppointmentOccursInThePast, "This appointment occurs in the past.\n");
			AddString(SchedulerExtensionsStringId.Msg_AllOccurrencesInThePast, "All instances of this recurring appointment occur in the past.\n");
			AddString(SchedulerExtensionsStringId.Msg_AppointmentConflictsWithAnother, "Conflicts with another appointment on your schedule.\n");
			AddString(SchedulerExtensionsStringId.Msg_PatternOccurrencesConflictsWithOtherAppointments, "{0} instance(s) of this recurring appointment conflict with other appointments on your schedule.\n");
			AddString(SchedulerExtensionsStringId.Msg_Some, "Some");
			AddString(SchedulerExtensionsStringId.Caption_ViewNavigator, "View Navigator");
			AddString(SchedulerExtensionsStringId.CaptionViewNavigator_Today, "Today");
			AddString(SchedulerExtensionsStringId.Caption_PageHome, "Home");
			AddString(SchedulerExtensionsStringId.Caption_PageFile, "File");
			AddString(SchedulerExtensionsStringId.Caption_PageView, "View");
			AddString(SchedulerExtensionsStringId.Caption_PageAppointment, "Appointment");
			AddString(SchedulerExtensionsStringId.Caption_GroupArrange, "Arrange");
			AddString(SchedulerExtensionsStringId.Caption_GroupActiveView, "Active View");
			AddString(SchedulerExtensionsStringId.Caption_GroupTimeScale, "Time Scale");
			AddString(SchedulerExtensionsStringId.Caption_GroupNavigator, "Navigate");
			AddString(SchedulerExtensionsStringId.Caption_GroupGroupBy, "Group By");
			AddString(SchedulerExtensionsStringId.Caption_GroupAppointment, "Appointment");
			AddString(SchedulerExtensionsStringId.Caption_GroupCommon, "Common");
			AddString(SchedulerExtensionsStringId.Caption_GroupPrint, "Print");
			AddString(SchedulerExtensionsStringId.Caption_GroupActions, "Actions");
			AddString(SchedulerExtensionsStringId.Caption_GroupOptions, "Options");
			AddString(SchedulerExtensionsStringId.Caption_GroupLayout, "Layout");
			AddString(SchedulerExtensionsStringId.Caption_PageCategoryCalendarTools, "Calendar Tools");
			AddString(SchedulerExtensionsStringId.Caption_Reminder, "Reminder:");
		}
		#endregion
		public static XtraLocalizer<SchedulerExtensionsStringId> CreateDefaultLocalizer() {
			return new SchedulerExtensionsResLocalizer();
		}
		public static string GetString(SchedulerExtensionsStringId id) {
			return Active.GetLocalizedString(id);
		}
		public override XtraLocalizer<SchedulerExtensionsStringId> CreateResXLocalizer() {
			return new SchedulerExtensionsResLocalizer();
		}
	}
	#endregion
}
namespace DevExpress.XtraScheduler.Native {
	#region ObsoleteText
	internal static class ObsoleteText {
		internal const string SRViewNavigator = "Delete the ViewNavigator component and generate bars automatically by selecting required items from the popup menu, which is invoked when clicking the scheduler's smart tag at design time.";
		internal const string SRViewSelector = "Delete the ViewSelector component and generate bars automatically by selecting required items from the popup menu, which is invoked when clicking the scheduler's smart tag at design time.";
		internal const string SRRibbonViewNavigator = "Delete the RibbonViewNavigator component and generate bars automatically by selecting required items from the popup menu, which is invoked when clicking the scheduler's smart tag at design time.";
		internal const string SRRibbonViewSelector = "Delete the RibbonViewSelector component and generate bars automatically by selecting required items from the popup menu, which is invoked when clicking the scheduler's smart tag at design time.";
	}
	#endregion
}
