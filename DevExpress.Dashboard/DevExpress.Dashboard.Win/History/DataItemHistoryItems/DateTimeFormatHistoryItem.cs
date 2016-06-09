#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.Localization;
namespace DevExpress.DashboardWin.Native {
	public class DateTimeFormatHistoryItem : DataItemHistoryItem {
		static void AssignDateTimeFormat(DataItemDateTimeFormat target, DataItemDateTimeFormat source) {
			target.BeginUpdate();
			try {
				target.DateFormat = source.DateFormat;
				target.DateHourFormat = source.DateHourFormat;
				target.DateHourMinuteFormat = source.DateHourMinuteFormat;
				target.DateTimeFormat = source.DateTimeFormat;
				target.DayOfWeekFormat = source.DayOfWeekFormat;
				target.MonthFormat = source.MonthFormat;
				target.QuarterFormat = source.QuarterFormat;
				target.YearFormat = source.YearFormat;
				target.HourFormat = source.HourFormat;
				target.ExactDateFormat = source.ExactDateFormat;
			}
			finally {
				target.EndUpdate();
			}
		}
		readonly DataItemDateTimeFormat format;
		readonly DataItemDateTimeFormat previousFormat;
		public DateTimeFormatHistoryItem(DataDashboardItem dashboardItem, DataItem dataItem, DataItemDateTimeFormat format)
			: base(dashboardItem, dataItem) {
			this.format = format;
			previousFormat = new DataItemDateTimeFormat(null);
			AssignDateTimeFormat(previousFormat, dataItem.DateTimeFormat);
		}
		public override string Caption {
			get { return String.Format(DashboardWinLocalizer.GetString(DashboardWinStringId.HistoryItemDataItemDateTimeFormat), DataItemCaption); }
		}
		void PerformOperation(DataItemDateTimeFormat format) {
			AssignDateTimeFormat(DataItem.DateTimeFormat, format);
		}
		protected override void PerformUndo() {
			PerformOperation(previousFormat);
		}
		protected override void PerformRedo() {
			PerformOperation(format);
		}
	}
}
