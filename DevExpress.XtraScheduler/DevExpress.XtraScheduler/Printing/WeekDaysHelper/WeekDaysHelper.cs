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

using DevExpress.XtraScheduler.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Printing.Native {
	public class WeekDaysHelper : PrintWeekDaysHelperBase {
		ViewPart viewPart;
		int totalColumnCount;
		public WeekDaysHelper(WeekViewInfo viewInfo, ViewPart viewPart)
			:
			base(viewInfo.FirstDayOfWeek, viewInfo.ShowWeekend, viewInfo.CompressWeekend) {
			this.viewPart = viewPart;
			if (ShowWeekend)
				totalColumnCount = CompressWeekend ? 6 : 7;
			else
				totalColumnCount = 5;
		}
		internal ViewPart ViewPart { get { return viewPart; } set { viewPart = value; } }
		internal int TotalColumnCount { get { return totalColumnCount; } set { totalColumnCount = value; } }
		public virtual DateTime[] GetWeekDates(DateTime[] weekDates) {
			List<DateTime> result = new List<DateTime>();
			int count = weekDates.Length;
			for (int i = 0; i < count; i++) {
				DayOfWeek day = weekDates[i].DayOfWeek;
				if (IsWeekDayVisible(day))
					result.Add(weekDates[i]);
			}
			return result.ToArray();
		}
		public virtual DayOfWeek[] GetActualWeekDays(DayOfWeek[] daysOfWeek) {
			return SelectVisibleWeekDays(daysOfWeek);
		}
		protected internal override bool IsWeekDayVisibleCore(DayOfWeek dayOfWeek) {
			if (viewPart == ViewPart.Both)
				return true;
			int column = ((int)dayOfWeek - (int)FirstDayOfWeek + TotalColumnCount) % TotalColumnCount;
			if (viewPart == ViewPart.Left)
				return column < (TotalColumnCount + 1) / 2;
			else
				return column >= (TotalColumnCount + 1) / 2;
		}
	}
}
