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

using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Calendar;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraScheduler.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Printing.Native {
	public class CalendarHeaderPrintInfo : IPrintableObjectViewInfo, ISupportClear {
		DateNavigator dateNavigator;
		Rectangle bounds;
		public CalendarHeaderPrintInfo(DateNavigator dateNavigator, Rectangle bounds) {
			this.dateNavigator = dateNavigator;
			this.bounds = bounds;
		}
		public Rectangle Bounds { get { return bounds; } }
		public virtual void Print(GraphicsInfoArgs graphicsInfoArgs) {
			int count = dateNavigator.Calendars.Count;
			DateNavigatorPrinter datePainter = new DateNavigatorPrinter(dateNavigator);
			Point oldOffset = ((XBrickPaint)graphicsInfoArgs.Cache.Paint).SetOffset(new Point(Bounds.Right - 1 - dateNavigator.Size.Width, Bounds.Top + 1));
			for (int i = 0; i < count; i++) {
				CalendarObjectViewInfo calendar = dateNavigator.Calendars[i];
				CalendarControlInfoArgs calendarInfo = new CalendarControlInfoArgs(dateNavigator.InternalViewInfo, graphicsInfoArgs.Cache, calendar.CalendarInfo.ClientRect);
				datePainter.DrawObject(new CalendarControlObjectInfoArgs(calendarInfo, calendar, graphicsInfoArgs.Cache));
			}
			((XBrickPaint)graphicsInfoArgs.Cache.Paint).SetOffset(oldOffset);
		}
		public virtual void Clear() {
			if (dateNavigator != null) {
				this.dateNavigator.SchedulerControl = null;
				this.dateNavigator.Dispose();
				this.dateNavigator = null;
			}
		}
		void ISupportClear.Clear() {
			this.Clear();
		}
	}
}
