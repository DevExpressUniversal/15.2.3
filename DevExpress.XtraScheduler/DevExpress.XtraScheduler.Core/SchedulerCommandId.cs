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
using DevExpress.Utils;
namespace DevExpress.XtraScheduler.Commands {
	public struct SchedulerCommandId : IConvertToInt<SchedulerCommandId>, IEquatable<SchedulerCommandId> {
		public static readonly SchedulerCommandId None = new SchedulerCommandId(0);		
		public static readonly SchedulerCommandId IncrementResourcePerPageCount = new SchedulerCommandId(1);
		public static readonly SchedulerCommandId DecrementResourcePerPageCount = new SchedulerCommandId(2);
		public static readonly SchedulerCommandId NavigateNextResource = new SchedulerCommandId(3);
		public static readonly SchedulerCommandId NavigateResourcePageForward = new SchedulerCommandId(4);
		public static readonly SchedulerCommandId NavigateLastResource = new SchedulerCommandId(5);
		public static readonly SchedulerCommandId NavigateFirstResource = new SchedulerCommandId(6);
		public static readonly SchedulerCommandId NavigateResourcePageBackward = new SchedulerCommandId(7);
		public static readonly SchedulerCommandId NavigatePrevResource = new SchedulerCommandId(8);
		public static readonly SchedulerCommandId SwitchToDayView = new SchedulerCommandId(9);
		public static readonly SchedulerCommandId SwitchToWorkWeekView = new SchedulerCommandId(10);
		public static readonly SchedulerCommandId SwitchToWeekView = new SchedulerCommandId(11);
		public static readonly SchedulerCommandId SwitchToMonthView = new SchedulerCommandId(12);
		public static readonly SchedulerCommandId SwitchToTimelineView = new SchedulerCommandId(13);
		public static readonly SchedulerCommandId SwitchToGanttView = new SchedulerCommandId(14);
		public static readonly SchedulerCommandId SwitchToFullWeekView = new SchedulerCommandId(15);
		public static readonly SchedulerCommandId GotoToday = new SchedulerCommandId(16);
		public static readonly SchedulerCommandId NavigateViewBackward = new SchedulerCommandId(17);
		public static readonly SchedulerCommandId NavigateViewForward = new SchedulerCommandId(18);
		public static readonly SchedulerCommandId ViewZoomIn = new SchedulerCommandId(19);
		public static readonly SchedulerCommandId ViewZoomOut = new SchedulerCommandId(20);
		public static readonly SchedulerCommandId SwitchToGroupByNone = new SchedulerCommandId(21);
		public static readonly SchedulerCommandId SwitchToGroupByDate = new SchedulerCommandId(22);
		public static readonly SchedulerCommandId SwitchToGroupByResource = new SchedulerCommandId(23);
		public static readonly SchedulerCommandId CollapseResource = new SchedulerCommandId(24);
		public static readonly SchedulerCommandId ExpandResource = new SchedulerCommandId(25);
		public static readonly SchedulerCommandId NewAppointment = new SchedulerCommandId(26);
		public static readonly SchedulerCommandId NewRecurringAppointment = new SchedulerCommandId(27);
		public static readonly SchedulerCommandId DeleteAppointmentsUI = new SchedulerCommandId(28);
		public static readonly SchedulerCommandId PrintPreview = new SchedulerCommandId(29);
		public static readonly SchedulerCommandId Print = new SchedulerCommandId(30);
		public static readonly SchedulerCommandId PrintPageSetup = new SchedulerCommandId(31);
		public static readonly SchedulerCommandId SwitchShowWorkTimeOnly = new SchedulerCommandId(32);
		public static readonly SchedulerCommandId SwitchCompressWeekend = new SchedulerCommandId(33);
		public static readonly SchedulerCommandId SwitchCellsAutoHeight = new SchedulerCommandId(34);
		public static readonly SchedulerCommandId EditAppointmentUI = new SchedulerCommandId(35);
		public static readonly SchedulerCommandId SplitAppointment = new SchedulerCommandId(36);
		public static readonly SchedulerCommandId ToggleRecurrence = new SchedulerCommandId(37);
		public static readonly SchedulerCommandId ChangeAppointmentStatusUI = new SchedulerCommandId(38);
		public static readonly SchedulerCommandId ChangeAppointmentLabelUI = new SchedulerCommandId(39);
		public static readonly SchedulerCommandId ChangeAppointmentReminderUI = new SchedulerCommandId(40);
		public static readonly SchedulerCommandId ChangeTimelineScaleWidth = new SchedulerCommandId(41);
		public static readonly SchedulerCommandId SaveSchedule = new SchedulerCommandId(42);
		public static readonly SchedulerCommandId OpenSchedule = new SchedulerCommandId(43);
		public static readonly SchedulerCommandId ChangeSnapToCellsUI = new SchedulerCommandId(44);
		public static readonly SchedulerCommandId EditOccurrenceUI = new SchedulerCommandId(45);
		public static readonly SchedulerCommandId EditSeriesUI = new SchedulerCommandId(46);
		public static readonly SchedulerCommandId DeleteOccurrenceUI = new SchedulerCommandId(47);
		public static readonly SchedulerCommandId DeleteSeriesUI = new SchedulerCommandId(48);
		public static readonly SchedulerCommandId SwitchTimeScalesUICommand = new SchedulerCommandId(49);
		public static readonly SchedulerCommandId SwitchTimeScalesCaptionUICommand= new SchedulerCommandId(50);
		public static readonly SchedulerCommandId SetTimeIntervalCount = new SchedulerCommandId(51);
		public static readonly SchedulerCommandId EditAppointmentSeriesGroup = new SchedulerCommandId(52);
		public static readonly SchedulerCommandId DeleteAppointmentSeriesGroupCommand = new SchedulerCommandId(53);
		public static readonly SchedulerCommandId ToolsAppointmentCommandGroup = new SchedulerCommandId(54);
		public static readonly SchedulerCommandId SwitchToMoreDetailedView = new SchedulerCommandId(55);
		public static readonly SchedulerCommandId SwitchToLessDetailedView = new SchedulerCommandId(56);
		public static readonly SchedulerCommandId GotoDay = new SchedulerCommandId(57);
		public static readonly SchedulerCommandId DayViewMovePrevDay = new SchedulerCommandId(58);
		public static readonly SchedulerCommandId DayViewMoveNextDay = new SchedulerCommandId(59);
		public static readonly SchedulerCommandId DayViewGroupByResourceMovePrevDay = new SchedulerCommandId(60);
		public static readonly SchedulerCommandId DayViewGroupByResourceMoveNextDay = new SchedulerCommandId(61);
		public static readonly SchedulerCommandId DayViewGroupByDateMovePrevDay = new SchedulerCommandId(62);
		public static readonly SchedulerCommandId DayViewGroupByDateMoveNextDay = new SchedulerCommandId(63);
		public static readonly SchedulerCommandId WorkWeekViewMovePrevDay = new SchedulerCommandId(64);
		public static readonly SchedulerCommandId WorkWeekViewMoveNextDay = new SchedulerCommandId(65);
		public static readonly SchedulerCommandId WorkWeekViewGroupByResourceMovePrevDay = new SchedulerCommandId(66);
		public static readonly SchedulerCommandId WorkWeekViewGroupByResourceMoveNextDay = new SchedulerCommandId(67);
		public static readonly SchedulerCommandId WorkWeekViewGroupByDateMovePrevDay = new SchedulerCommandId(68);
		public static readonly SchedulerCommandId WorkWeekViewGroupByDateMoveNextDay = new SchedulerCommandId(69);
		public static readonly SchedulerCommandId WeekViewMoveLeft = new SchedulerCommandId(70);
		public static readonly SchedulerCommandId WeekViewMoveRight = new SchedulerCommandId(71);
		public static readonly SchedulerCommandId WeekViewMovePrevDay = new SchedulerCommandId(72);
		public static readonly SchedulerCommandId WeekViewMoveNextDay = new SchedulerCommandId(73);
		public static readonly SchedulerCommandId WeekViewMoveFirstDay = new SchedulerCommandId(74);
		public static readonly SchedulerCommandId WeekViewMoveLastDay = new SchedulerCommandId(75);
		public static readonly SchedulerCommandId WeekViewMovePageUp = new SchedulerCommandId(76);
		public static readonly SchedulerCommandId WeekViewMovePageDown = new SchedulerCommandId(77);
		public static readonly SchedulerCommandId WeekViewMoveStartOfMonth = new SchedulerCommandId(78);
		public static readonly SchedulerCommandId WeekViewMoveEndOfMonth = new SchedulerCommandId(79);
		public static readonly SchedulerCommandId WeekViewExtendLeft = new SchedulerCommandId(80);
		public static readonly SchedulerCommandId WeekViewExtendRight = new SchedulerCommandId(81);
		public static readonly SchedulerCommandId WeekViewExtendPrevDay = new SchedulerCommandId(82);
		public static readonly SchedulerCommandId WeekViewExtendNextDay = new SchedulerCommandId(83);
		public static readonly SchedulerCommandId WeekViewExtendFirstDay = new SchedulerCommandId(84);
		public static readonly SchedulerCommandId WeekViewExtendLastDay = new SchedulerCommandId(85);
		public static readonly SchedulerCommandId WeekViewExtendPageUp = new SchedulerCommandId(86);
		public static readonly SchedulerCommandId WeekViewExtendPageDown = new SchedulerCommandId(87);
		public static readonly SchedulerCommandId WeekViewExtendStartOfMonth = new SchedulerCommandId(88);
		public static readonly SchedulerCommandId WeekViewExtendEndOfMonth = new SchedulerCommandId(89);
		public static readonly SchedulerCommandId WeekViewGroupByResourceMoveLeft = new SchedulerCommandId(90);
		public static readonly SchedulerCommandId WeekViewGroupByResourceMoveRight = new SchedulerCommandId(91);
		public static readonly SchedulerCommandId WeekViewGroupByDateMoveUp = new SchedulerCommandId(92);
		public static readonly SchedulerCommandId WeekViewGroupByDateMoveDown = new SchedulerCommandId(93);
		public static readonly SchedulerCommandId WeekViewGroupByDateExtendUp = new SchedulerCommandId(94);
		public static readonly SchedulerCommandId WeekViewGroupByDateExtendDown = new SchedulerCommandId(95);
		public static readonly SchedulerCommandId MonthViewMovePrevDay = new SchedulerCommandId(96);
		public static readonly SchedulerCommandId MonthViewMoveNextDay = new SchedulerCommandId(97);
		public static readonly SchedulerCommandId MonthViewMoveUp = new SchedulerCommandId(98);
		public static readonly SchedulerCommandId MonthViewMoveDown = new SchedulerCommandId(99);
		public static readonly SchedulerCommandId MonthViewMoveFirstDay = new SchedulerCommandId(100);
		public static readonly SchedulerCommandId MonthViewMoveLastDay = new SchedulerCommandId(101);
		public static readonly SchedulerCommandId MonthViewMovePageUp = new SchedulerCommandId(102);
		public static readonly SchedulerCommandId MonthViewMovePageDown = new SchedulerCommandId(103);
		public static readonly SchedulerCommandId MonthViewMoveStartOfMonth = new SchedulerCommandId(104);
		public static readonly SchedulerCommandId MonthViewMoveEndOfMonth = new SchedulerCommandId(105);
		public static readonly SchedulerCommandId MonthViewExtendPrevDay = new SchedulerCommandId(106);
		public static readonly SchedulerCommandId MonthViewExtendNextDay = new SchedulerCommandId(107);
		public static readonly SchedulerCommandId MonthViewExtendUp = new SchedulerCommandId(108);
		public static readonly SchedulerCommandId MonthViewExtendDown = new SchedulerCommandId(109);
		public static readonly SchedulerCommandId MonthViewExtendFirstDay = new SchedulerCommandId(110);
		public static readonly SchedulerCommandId MonthViewExtendLastDay = new SchedulerCommandId(111);
		public static readonly SchedulerCommandId MonthViewExtendPageUp = new SchedulerCommandId(112);
		public static readonly SchedulerCommandId MonthViewExtendPageDown = new SchedulerCommandId(113);
		public static readonly SchedulerCommandId MonthViewExtendStartOfMonth = new SchedulerCommandId(114);
		public static readonly SchedulerCommandId MonthViewExtendEndOfMonth = new SchedulerCommandId(115);
		public static readonly SchedulerCommandId MonthViewGroupByResourceMoveLeft = new SchedulerCommandId(116);
		public static readonly SchedulerCommandId MonthViewGroupByResourceMoveRight = new SchedulerCommandId(117);
		public static readonly SchedulerCommandId MonthViewGroupByDateMoveUp = new SchedulerCommandId(118);
		public static readonly SchedulerCommandId MonthViewGroupByDateMoveDown = new SchedulerCommandId(119);
		public static readonly SchedulerCommandId TimelineViewMovePrevDate = new SchedulerCommandId(200);
		public static readonly SchedulerCommandId TimelineViewMoveNextDate = new SchedulerCommandId(201);
		public static readonly SchedulerCommandId TimelineViewMoveToStartOfVisibleTime = new SchedulerCommandId(202);
		public static readonly SchedulerCommandId TimelineViewMoveToEndOfVisibleTime = new SchedulerCommandId(203);
		public static readonly SchedulerCommandId TimelineViewMoveToMajorStart = new SchedulerCommandId(204);
		public static readonly SchedulerCommandId TimelineViewMoveToMajorEnd = new SchedulerCommandId(205);
		public static readonly SchedulerCommandId TimelineViewExtendPrevDate = new SchedulerCommandId(206);
		public static readonly SchedulerCommandId TimelineViewExtendNextDate = new SchedulerCommandId(207);
		public static readonly SchedulerCommandId EditAppointmentQuery = new SchedulerCommandId(208);
		public static readonly SchedulerCommandId GotoDate = new SchedulerCommandId(209);
		public static readonly SchedulerCommandId DeleteAppointmentsQueryOrDependencies = new SchedulerCommandId(210);
		public static readonly SchedulerCommandId EditAppointmentViaInplaceEditor = new SchedulerCommandId(211);
		public static readonly SchedulerCommandId EditAppointmentOrNewAppointmentViaInplaceEditor = new SchedulerCommandId(212);
		public static readonly SchedulerCommandId DayViewMoveFirstDayOfWeek = new SchedulerCommandId(213);
		public static readonly SchedulerCommandId DayViewMoveLastDayOfWeek = new SchedulerCommandId(214);
		public static readonly SchedulerCommandId DayViewMoveStartOfMonth = new SchedulerCommandId(215);
		public static readonly SchedulerCommandId DayViewMoveEndOfMonth = new SchedulerCommandId(216);
		public static readonly SchedulerCommandId DayViewMovePrevWeek = new SchedulerCommandId(217);
		public static readonly SchedulerCommandId DayViewMoveNextWeek = new SchedulerCommandId(218);
		public static readonly SchedulerCommandId DayViewExtendFirstDayOfWeek = new SchedulerCommandId(219);
		public static readonly SchedulerCommandId DayViewExtendLastDayOfWeek = new SchedulerCommandId(220);
		public static readonly SchedulerCommandId DayViewExtendStartOfMonth = new SchedulerCommandId(221);
		public static readonly SchedulerCommandId DayViewExtendEndOfMonth = new SchedulerCommandId(222);
		public static readonly SchedulerCommandId DayViewExtendPrevDay = new SchedulerCommandId(223);
		public static readonly SchedulerCommandId DayViewExtendNextDay = new SchedulerCommandId(224);
		public static readonly SchedulerCommandId DayViewMovePrevCell = new SchedulerCommandId(225);
		public static readonly SchedulerCommandId DayViewMoveNextCell = new SchedulerCommandId(226);
		public static readonly SchedulerCommandId DayViewMoveToStartOfWorkTime = new SchedulerCommandId(227);
		public static readonly SchedulerCommandId DayViewMoveToEndOfWorkTime = new SchedulerCommandId(228);
		public static readonly SchedulerCommandId DayViewMoveToStartOfVisibleTime = new SchedulerCommandId(229);
		public static readonly SchedulerCommandId DayViewMoveToEndOfVisibleTime = new SchedulerCommandId(230);
		public static readonly SchedulerCommandId DayViewMovePageUp = new SchedulerCommandId(231);
		public static readonly SchedulerCommandId DayViewMovePageDown = new SchedulerCommandId(232);
		public static readonly SchedulerCommandId DayViewExtendPrevCell = new SchedulerCommandId(234);
		public static readonly SchedulerCommandId DayViewExtendNextCell = new SchedulerCommandId(235);
		public static readonly SchedulerCommandId DayViewExtendToStartOfWorkTime = new SchedulerCommandId(236);
		public static readonly SchedulerCommandId DayViewExtendToEndOfWorkTime = new SchedulerCommandId(237);
		public static readonly SchedulerCommandId DayViewExtendToStartOfVisibleTime = new SchedulerCommandId(238);
		public static readonly SchedulerCommandId DayViewExtendToEndOfVisibleTime = new SchedulerCommandId(239);
		public static readonly SchedulerCommandId DayViewExtendPageUp = new SchedulerCommandId(240);
		public static readonly SchedulerCommandId DayViewExtendPageDown = new SchedulerCommandId(241);
		public static readonly SchedulerCommandId WorkWeekViewMoveFirstDayOfWeek = new SchedulerCommandId(242);
		public static readonly SchedulerCommandId WorkWeekViewMoveLastDayOfWeek = new SchedulerCommandId(243);
		public static readonly SchedulerCommandId WorkWeekViewMovePrevWeek = new SchedulerCommandId(244);
		public static readonly SchedulerCommandId WorkWeekViewMoveNextWeek = new SchedulerCommandId(245);
		public static readonly SchedulerCommandId WorkWeekViewExtendFirstDayOfWeek = new SchedulerCommandId(246);
		public static readonly SchedulerCommandId WorkWeekViewExtendLastDayOfWeek = new SchedulerCommandId(247);
		public static readonly SchedulerCommandId WorkWeekViewExtendPrevDay = new SchedulerCommandId(248);
		public static readonly SchedulerCommandId WorkWeekViewExtendNextDay = new SchedulerCommandId(249);
		public static readonly SchedulerCommandId WorkWeekViewMovePrevCell = new SchedulerCommandId(250);
		public static readonly SchedulerCommandId WorkWeekViewMoveNextCell = new SchedulerCommandId(251);
		public static readonly SchedulerCommandId WorkWeekViewMoveToStartOfWorkTime = new SchedulerCommandId(252);
		public static readonly SchedulerCommandId WorkWeekViewMoveToEndOfWorkTime = new SchedulerCommandId(253);
		public static readonly SchedulerCommandId WorkWeekViewMoveToStartOfVisibleTime = new SchedulerCommandId(254);
		public static readonly SchedulerCommandId WorkWeekViewMoveToEndOfVisibleTime = new SchedulerCommandId(255);
		public static readonly SchedulerCommandId WorkWeekViewMovePageUp = new SchedulerCommandId(256);
		public static readonly SchedulerCommandId WorkWeekViewMovePageDown = new SchedulerCommandId(257);
		public static readonly SchedulerCommandId WorkWeekViewExtendPrevCell = new SchedulerCommandId(258);
		public static readonly SchedulerCommandId WorkWeekViewExtendNextCell = new SchedulerCommandId(259);
		public static readonly SchedulerCommandId WorkWeekViewExtendToStartOfWorkTime = new SchedulerCommandId(260);
		public static readonly SchedulerCommandId WorkWeekViewExtendToEndOfWorkTime = new SchedulerCommandId(261);
		public static readonly SchedulerCommandId WorkWeekViewExtendToStartOfVisibleTime = new SchedulerCommandId(262);
		public static readonly SchedulerCommandId WorkWeekViewExtendToEndOfVisibleTime = new SchedulerCommandId(263);
		public static readonly SchedulerCommandId WorkWeekViewExtendPageUp = new SchedulerCommandId(264);
		public static readonly SchedulerCommandId WorkWeekViewExtendPageDown = new SchedulerCommandId(265);
		public static readonly SchedulerCommandId TimelineViewGroupByDateMoveUp = new SchedulerCommandId(266);
		public static readonly SchedulerCommandId TimelineViewGroupByDateMoveDown = new SchedulerCommandId(267);
		public static readonly SchedulerCommandId SelectNextAppointment = new SchedulerCommandId(268);
		public static readonly SchedulerCommandId MoveFocusNext = new SchedulerCommandId(269);
		public static readonly SchedulerCommandId MoveFocusPrev = new SchedulerCommandId(270);
		public static readonly SchedulerCommandId SelectPrevAppointment = new SchedulerCommandId(271);
		readonly int m_value;
		public SchedulerCommandId(int value) {
			m_value = value;
		}
		public override bool Equals(object obj) {
			return ((obj is SchedulerCommandId) && (this.m_value == ((SchedulerCommandId)obj).m_value));
		}
		public override int GetHashCode() {
			return m_value.GetHashCode();
		}
		public override string ToString() {
			return m_value.ToString();
		}
		public static bool operator ==(SchedulerCommandId id1, SchedulerCommandId id2) {
			return id1.m_value == id2.m_value;
		}
		public static bool operator !=(SchedulerCommandId id1, SchedulerCommandId id2) {
			return id1.m_value != id2.m_value;
		}
		#region IConvertToInt<SchedulerCommandId> Members
		int IConvertToInt<SchedulerCommandId>.ToInt() {
			return m_value;
		}
		SchedulerCommandId IConvertToInt<SchedulerCommandId>.FromInt(int value) {
			return new SchedulerCommandId(value);
		}
		#endregion
		#region IEquatable<SchedulerCommandId> Members
		public bool Equals(SchedulerCommandId other) {
			return this.m_value == other.m_value;
		}
		#endregion
	}
}
