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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Drawing {
	public class DayViewFixedFormatHeaderCaptionCalculator : DayViewHeaderCaptionCalculator {
		#region Fields
		IHeaderCaptionService captionProvider;
		IHeaderToolTipService toolTipProvider;
		DayViewHeaderCaptionCalculator fallbackCalculator;
		#endregion
		public DayViewFixedFormatHeaderCaptionCalculator(IHeaderCaptionService captionFormatProvider, IHeaderToolTipService toolTipFormatProvider, DayViewHeaderCaptionCalculator fallbackCalculator) {
			if (captionFormatProvider == null)
				Exceptions.ThrowArgumentNullException("captionFormatProvider");
			if (toolTipFormatProvider == null)
				Exceptions.ThrowArgumentNullException("toolTipFormatProvider");
			if (fallbackCalculator == null)
				Exceptions.ThrowArgumentNullException("fallbackCalculator");
			this.captionProvider = captionFormatProvider;
			this.toolTipProvider = toolTipFormatProvider;
			this.fallbackCalculator = fallbackCalculator;
		}
		#region Properties
		public IHeaderCaptionService CaptionProvider { get { return captionProvider; } }
		public IHeaderToolTipService ToolTipProvider { get { return toolTipProvider; } }
		public DayViewHeaderCaptionCalculator FallbackCalculator { get { return fallbackCalculator; } }
		#endregion
		public override void CalculateFixedHeaderCaption(DayHeader header) {
			string format = CaptionProvider.GetDayColumnHeaderCaption(header);
			if (!String.IsNullOrEmpty(format))
				header.Caption = String.Format(CultureInfo.CurrentCulture, format, header.Interval.Start);
		}
		public override void CalculateOptimalHeaderCaption(DayHeader header, Graphics gr, Size textSize) {
			if (String.IsNullOrEmpty(header.Caption))
				fallbackCalculator.CalculateOptimalHeaderCaption(header, gr, textSize);
			else
				header.ShouldShowToolTip = (gr.MeasureString(header.Caption, header.CaptionAppearance.Font).Width > textSize.Width);
		}
		public override void CalculateHeaderToolTip(DayHeader header) {
			string format = ToolTipProvider.GetDayColumnHeaderToolTip(header);
			if (String.IsNullOrEmpty(format))
				fallbackCalculator.CalculateHeaderToolTip(header);
			else
				header.ToolTipText = String.Format(CultureInfo.CurrentCulture, format, header.Interval.Start);
		}
	}
}
