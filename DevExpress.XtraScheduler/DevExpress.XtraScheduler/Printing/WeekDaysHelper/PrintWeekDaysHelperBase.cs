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
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Printing.Native {
	public abstract class PrintWeekDaysHelperBase {
		DayOfWeek firstDayOfWeek;
		bool showWeekend;
		bool compressWeekend;
		protected PrintWeekDaysHelperBase(DayOfWeek firstDayOfWeek, bool showWeekend, bool compressWeekend) {
			this.showWeekend = showWeekend;
			this.compressWeekend = compressWeekend;
			this.firstDayOfWeek = CalculateVisibleFirstDayOfWeek(firstDayOfWeek);
		}
		internal DayOfWeek FirstDayOfWeek { get { return firstDayOfWeek; } set { firstDayOfWeek = value; } }
		internal bool ShowWeekend { get { return showWeekend; } set { showWeekend = value; } }
		internal bool CompressWeekend { get { return compressWeekend; } set { compressWeekend = value; } }
		internal DayOfWeek CalculateVisibleFirstDayOfWeek(DayOfWeek firstDayOfWeek) {
			if (showWeekend) {
				if (compressWeekend)
					return firstDayOfWeek != DayOfWeek.Sunday ? firstDayOfWeek : DayOfWeek.Monday;
				else
					return firstDayOfWeek;
			} else
				return (firstDayOfWeek != DayOfWeek.Sunday && firstDayOfWeek != DayOfWeek.Saturday) ? firstDayOfWeek : DayOfWeek.Monday;
		}
		protected DayOfWeek[] SelectVisibleWeekDays(DayOfWeek[] daysOfWeek) {
			List<DayOfWeek> result = new List<DayOfWeek>();
			int count = daysOfWeek.Length;
			for (int i = 0; i < count; i++) {
				DayOfWeek day = daysOfWeek[i];
				if (IsWeekDayVisible(day))
					result.Add(day);
			}
			return result.ToArray();
		}
		protected internal virtual bool IsWeekDayVisible(DayOfWeek dayOfWeek) {
			if (!ShowWeekend && (dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday))
				return false;
			if (CompressWeekend && dayOfWeek == DayOfWeek.Sunday)
				dayOfWeek = DayOfWeek.Saturday;
			return IsWeekDayVisibleCore(dayOfWeek);
		}
		protected internal abstract bool IsWeekDayVisibleCore(DayOfWeek dayOfWeek);
	}
}
