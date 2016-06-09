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

using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Services;
using DevExpress.XtraScheduler.Services.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Drawing {
	public class WeekViewBasedFixedFormatHeaderCaptionCalculator : WeekViewBasedFixedFormatHeaderCaptionCalculatorBase {
		IHeaderToolTipService toolTipProvider;
		public WeekViewBasedFixedFormatHeaderCaptionCalculator(HeaderCaptionFormatProviderBase captionProvider, IHeaderToolTipService toolTipProvider, WeekBasedViewHeaderCaptionCalculator fallbackCalculator)
			: base(captionProvider, fallbackCalculator) {
			if (toolTipProvider == null)
				Exceptions.ThrowArgumentNullException("headerToolTipProvider");
			this.toolTipProvider = toolTipProvider;
		}
		public IHeaderToolTipService ToolTipProvider { get { return toolTipProvider; } }
		public override void CalculateHeaderToolTip(DayOfWeekHeader header) {
			string format = ToolTipProvider.GetDayOfWeekHeaderToolTip(header);
			if (String.IsNullOrEmpty(format))
				FallbackCalculator.CalculateHeaderToolTip(header);
			else {
				DateTimeFormatInfo dtfi = DateTimeFormatHelper.CurrentDateTimeFormat;
				header.ToolTipText = String.Format(CultureInfo.CurrentCulture, format, dtfi.GetDayName(header.DayOfWeek), header.Interval.Start);
			}
		}
	}
}
