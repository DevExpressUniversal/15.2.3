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
using DevExpress.XtraScheduler.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Printing.Native {
	abstract class PrintViewInfoBuilder {
		#region CreateInstance (static)
		public static PrintViewInfoBuilder CreateInstance(SchedulerPrintStyle printStyle, SchedulerControl control, GraphicsInfo gInfo) {
			switch (printStyle.Kind) {
				case SchedulerPrintStyleKind.CalendarDetails:
					return new CalendarDetailsPrintViewInfoBuilder((CalendarDetailsPrintStyle)printStyle, control, gInfo);
				case SchedulerPrintStyleKind.Daily:
					return new SchedulerSingleViewPrintBuilder((DailyPrintStyle)printStyle, control, gInfo);
				case SchedulerPrintStyleKind.Weekly:
					return new SchedulerSingleViewPrintBuilder((WeeklyPrintStyle)printStyle, control, gInfo);
				case SchedulerPrintStyleKind.Monthly:
					return new SchedulerSingleViewPrintBuilder((MonthlyPrintStyle)printStyle, control, gInfo);
				case SchedulerPrintStyleKind.TriFold:
					return new TriFoldPrintViewInfoBuilder((TriFoldPrintStyle)printStyle, control, gInfo);
				case SchedulerPrintStyleKind.Memo:
					return new MemoPrintViewInfoBuilder((MemoPrintStyle)printStyle, control, gInfo);
				default:
					Exceptions.ThrowInternalException();
					return null;
			}
		}
		#endregion
		SchedulerPrintStyle printStyle;
		SchedulerControl control;
		GraphicsInfo gInfo;
		public PrintViewInfoBuilder(SchedulerPrintStyle printStyle, SchedulerControl control, GraphicsInfo gInfo) {
			this.printStyle = printStyle;
			this.control = control;
			this.gInfo = gInfo;
		}
		protected SchedulerPrintStyle PrintStyle { get { return printStyle; } }
		protected SchedulerControl Control { get { return control; } }
		protected GraphicsInfo GInfo { get { return gInfo; } }
		public abstract IPrintableObjectViewInfo CreateViewInfo(Rectangle pageBounds);
	}
}
