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

using DevExpress.LookAndFeel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Drawing {
	public class SchedulerFlatPaintStyle : SchedulerPaintStyle {
		public override string Name { get { return "Flat"; } }
		public override ViewPainterBase CreateDayViewPainter() {
			ViewPainterBase painter = new DayViewPainterFlat();
			painter.Initialize();
			return painter;
		}
		public override ViewPainterBase CreateWorkWeekViewPainter() {
			ViewPainterBase painter = new WorkWeekViewPainterFlat();
			painter.Initialize();
			return painter;
		}
		public override ViewPainterBase CreateWeekViewPainter() {
			ViewPainterBase painter = new WeekViewPainterFlat();
			painter.Initialize();
			return painter;
		}
		public override ViewPainterBase CreateMonthViewPainter() {
			ViewPainterBase painter = new MonthViewPainterFlat();
			painter.Initialize();
			return painter;
		}
		public override ViewPainterBase CreateTimelineViewPainter() {
			ViewPainterBase painter = new TimelineViewPainterFlat();
			painter.Initialize();
			return painter;
		}
		public override ViewPainterBase CreateGanttViewPainter() {
			ViewPainterBase painter = new GanttViewPainterFlat();
			painter.Initialize();
			return painter;
		}
		protected internal override UserLookAndFeel CreateUserLookAndFeel() {
			UserLookAndFeel lf = new DevExpress.LookAndFeel.Helpers.EmbeddedLookAndFeel();
			lf.SetFlatStyle();
			return lf;
		}
	}
}
