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

using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Drawing {
	public abstract class TimeRulerCalculatorHelperBase {
		TimeRulerPainter painter;
		GraphicsCache cache;
		protected TimeRulerCalculatorHelperBase(GraphicsCache cache, TimeRulerPainter painter) {
			this.painter = painter;
			this.cache = cache;
		}
		public GraphicsCache Cache { get { return cache; } }
		protected internal TimeRulerPainter Painter { get { return painter; } }
		protected internal virtual int CalcMaxDateStringWidth(AppearanceObject appearance, DateTime date, TimeFormatInfo formatInfo) {
			int width = CalcTimeStringWidth(appearance, date, ChooseFormat(date, true, formatInfo));
			return Math.Max(width, CalcTimeStringWidth(appearance, date, ChooseFormat(date, false, formatInfo)));
		}
		protected internal virtual int CalcTimeStringWidth(AppearanceObject appearance, DateTime date, string format) {
			string str = date.ToString(format);
			return (int)Math.Ceiling(appearance.CalcTextSize(Cache, str, Int32.MaxValue).Width);
		}
		protected internal virtual int CalculateTimeRulerWidth(TimeRuler ruler, TimeRulerViewInfo viewInfo) {
			int width = CalculateTimeRulerWidthCore(ruler, viewInfo);
			width += 2 * Painter.ContentSpan;
			return Painter.GetFullWidthByContentWidth(Cache, width);
		}
		protected internal abstract string ChooseFormat(DateTime time, bool useTimeDesignator, TimeFormatInfo formatInfo);
		protected internal abstract int CalculateTimeRulerWidthCore(TimeRuler ruler, TimeRulerViewInfo viewInfo);
		protected internal abstract ViewInfoItemCollection CreateTimeSeparatorLines(TimeRulerViewInfo ruler, DateTime[] actualTimes, Rectangle[] rowsBounds);
		protected internal abstract ViewInfoItemCollection CreateTimeCaptions(TimeRulerViewInfo ruler, DateTime[] actualTimes, Rectangle[] rowsBounds);
		protected internal abstract void PrepareLayout(TimeRulerViewInfo ruler);
	}
}
