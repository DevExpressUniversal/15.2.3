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
using System.Web.UI;
using DevExpress.Web;
using DevExpress.Web.Internal;
using System.Web.UI.WebControls;
namespace DevExpress.Web {
	public abstract class CalendarDayEventArgs : EventArgs {
		internal DayData DayData;
		internal CalendarDayEventArgs(DayData dayData) {
			DayData = dayData;
		}
		public DateTime Date { get { return DayData.Date; } }
		public bool IsSelected { get { return DayData.IsSelected; } }
		public bool IsOtherMonthDay { get { return DayData.IsOtherMonthDay; } }
	}
	public class CalendarCustomDisabledDateEventArgs : EventArgs {
		internal DayData DayData;
		internal CalendarCustomDisabledDateEventArgs(DayData dayData) {
			DayData = dayData;
		}
		public DateTime Date { get { return DayData.Date; } }
		public bool IsDisabled {
			get { return DayData.IsDisabled; }
			set { DayData.IsDisabled = value; }
		}
	}
	public class CalendarDayCellInitializeEventArgs : CalendarDayEventArgs {
		internal CalendarDayCellInitializeEventArgs(DayData dayData)
			: base(dayData) {
			EncodeHtml = true;
		}
		public string DisplayText {
			get { return DayData.DisplayText; }
			set { DayData.DisplayText = value; }
		}
		public string NavigateUrl {
			get { return DayData.NavigateUrl; }
			set { DayData.NavigateUrl = value; }
		}
		public string NavigateUrlTarget {
			get { return DayData.NavigateUrlTarget; }
			set { DayData.NavigateUrlTarget = value; }
		}
		public bool IsWeekend {
			get { return DayData.Weekend; }
			set { DayData.Weekend = value; }
		}
		public bool EncodeHtml { get; set; }
	}
	public class CalendarDayCellCreatedEventArgs : CalendarDayEventArgs {
		TableCell cell;
		internal CalendarDayCellCreatedEventArgs(DayData dayData, TableCell cell)
			: base(dayData) {
				this.cell = cell;
		}
		public ControlCollection Controls { get { return cell.Controls; } }
		public bool IsWeekend { get { return DayData.Weekend; } }
	}
	public class CalendarDayCellPreparedEventArgs : CalendarDayEventArgs {
		TableCell cell;
		WebControl textControl;
		internal CalendarDayCellPreparedEventArgs(DayData dayData, TableCell cell, WebControl textControl)
			: base(dayData) {
			this.cell = cell;
			this.textControl = textControl;
		}
		public TableCell Cell { get { return cell; } }
		public WebControl TextControl { get { return textControl; } }
		public bool IsWeekend { get { return DayData.Weekend; } }
	}
	#region Legacy
	public class DayRenderEventArgs : EventArgs {
		private ControlCollection controls;
		private AppearanceStyleBase style = new AppearanceStyleBase();
		private string text = "";
		private string url = "";
		private CalendarDayCell day = null;
		private string target = "";
		public ControlCollection Controls {
			get { return controls; }
		}
		public AppearanceStyleBase Style {
			get { return style; }
		}
		public string Text {
			get { return text; }
			set { text = value; }
		}
		public string NavigateUrl {
			get { return url; }
			set { url = value; }
		}
		public CalendarDayCell Day {
			get { return day; }
		}
		public string Target {
			get { return target; }
			set { target = value; }
		}
		public DayRenderEventArgs(ControlCollection controls, CalendarDayCell day, string text) {
			this.controls = controls;
			this.day = day;
			this.text = text;
		}
	}
	public delegate void DayRenderEventHandler(object sender, DayRenderEventArgs e);
	#endregion
}
