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
using DevExpress.XtraScheduler.Services.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Drawing {
	#region WeekViewBasedFixedFormatHeaderCaptionCalculator
	public abstract class WeekViewBasedFixedFormatHeaderCaptionCalculatorBase : WeekBasedViewHeaderCaptionCalculator {
		#region Fields
		HeaderCaptionFormatProviderBase captionProvider;
		WeekBasedViewHeaderCaptionCalculator fallbackCalculator;
		#endregion
		protected WeekViewBasedFixedFormatHeaderCaptionCalculatorBase(HeaderCaptionFormatProviderBase captionProvider, WeekBasedViewHeaderCaptionCalculator fallbackCalculator) {
			if (captionProvider == null)
				Exceptions.ThrowArgumentNullException("headerCaptionProvider");
			if (fallbackCalculator == null)
				Exceptions.ThrowArgumentNullException("fallbackCalculator");
			this.captionProvider = captionProvider;
			this.fallbackCalculator = fallbackCalculator;
		}
		#region Properties
		public HeaderCaptionFormatProviderBase CaptionProvider { get { return captionProvider; } }
		public WeekBasedViewHeaderCaptionCalculator FallbackCalculator { get { return fallbackCalculator; } }
		#endregion
		public override void CalculateFullHeaderCaption(DayOfWeekHeader header) {
			string format = CaptionProvider.GetDayOfWeekHeaderCaption(header);
			if (String.IsNullOrEmpty(format)) {
				fallbackCalculator.CalculateFullHeaderCaption(header);
			} else {
				DateTimeFormatInfo dtfi = DateTimeFormatHelper.CurrentDateTimeFormat;
				header.Caption = String.Format(CultureInfo.CurrentCulture, format, dtfi.GetDayName(header.DayOfWeek), header.Interval.Start);
			}
		}
		public override void CalculateAbbreviatedHeaderCaption(DayOfWeekHeader header) {
			string format = CaptionProvider.GetDayOfWeekAbbreviatedHeaderCaption(header);
			if (String.IsNullOrEmpty(format))
				fallbackCalculator.CalculateAbbreviatedHeaderCaption(header);
			else {
				DateTimeFormatInfo dtfi = DateTimeFormatHelper.CurrentDateTimeFormat;
				header.Caption = String.Format(CultureInfo.CurrentCulture, format, dtfi.GetDayName(header.DayOfWeek), header.Interval.Start);
			}
		}
	}
	#endregion
}
