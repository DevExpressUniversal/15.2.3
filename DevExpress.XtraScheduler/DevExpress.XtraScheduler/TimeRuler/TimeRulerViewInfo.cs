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
using DevExpress.XtraScheduler.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Drawing {
	public class TimeRulerViewInfo : ViewInfoItemContainer {
		#region Fields
		TimeFormatInfo formatInfo;
		TimeRuler ruler;
		AppearanceObject backgroundAppearance;
		AppearanceObject lineAppearance;
		AppearanceObject hourLineAppearance;
		AppearanceObject nowLineAppearance;
		AppearanceObject nowAreaAppearance;
		AppearanceObject largeHourAppearance;
		ViewInfoItemCollection currentTimeItems = new ViewInfoItemCollection();
		Rectangle contentBounds;
		Rectangle clientBounds;
		Rectangle headerBounds;
		bool isFirst;
		ViewInfoTextItem headerCaptionItem;
		#endregion
		public TimeRulerViewInfo(TimeRuler ruler, DayViewAppearance appearance, TimeFormatInfo formatInfo) {
			if (ruler == null)
				Exceptions.ThrowArgumentException("ruler", ruler);
			if (appearance == null)
				Exceptions.ThrowArgumentException("appearance", appearance);
			if (formatInfo == null)
				Exceptions.ThrowArgumentException("formatInfo", formatInfo);
			this.formatInfo = formatInfo;
			this.ruler = ruler;
			this.backgroundAppearance = (AppearanceObject)appearance.TimeRuler.Clone();
			this.lineAppearance = (AppearanceObject)appearance.TimeRulerLine.Clone();
			this.hourLineAppearance = (AppearanceObject)appearance.TimeRulerHourLine.Clone();
			this.nowLineAppearance = (AppearanceObject)appearance.TimeRulerNowLine.Clone();
			this.nowAreaAppearance = (AppearanceObject)appearance.TimeRulerNowArea.Clone();
			this.largeHourAppearance = (AppearanceObject)this.backgroundAppearance.Clone();
			this.largeHourAppearance.Font = CreateLargeFont(backgroundAppearance.Font);
			this.largeHourAppearance.TextOptions.VAlignment = VertAlignment.Top;
			this.largeHourAppearance.TextOptions.HAlignment = HorzAlignment.Far;
		}
		#region IDisposable implementation
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (backgroundAppearance != null) {
						backgroundAppearance.Dispose();
						backgroundAppearance = null;
					}
					if (lineAppearance != null) {
						lineAppearance.Dispose();
						lineAppearance = null;
					}
					if (hourLineAppearance != null) {
						hourLineAppearance.Dispose();
						hourLineAppearance = null;
					}
					if (nowLineAppearance != null) {
						nowLineAppearance.Dispose();
						nowLineAppearance = null;
					}
					if (nowAreaAppearance != null) {
						nowAreaAppearance.Dispose();
						nowAreaAppearance = null;
					}
					if (largeHourAppearance != null) {
						largeHourAppearance.Dispose();
						largeHourAppearance = null;
					}
					if (currentTimeItems != null) {
						DisposeCurrentTimeItems();
						currentTimeItems = null;
					}
					if (headerCaptionItem != null) {
						DisposeItemCore(headerCaptionItem);
						headerCaptionItem = null;
					}
				}
			} finally {
				base.Dispose(disposing);
			}
		}
		#endregion
		#region Properties
		public override SchedulerHitTest HitTestType { get { return SchedulerHitTest.Ruler; } }
		public TimeFormatInfo FormatInfo { get { return formatInfo; } }
		public TimeRuler Ruler { get { return ruler; } }
		public AppearanceObject BackgroundAppearance { get { return backgroundAppearance; } }
		public AppearanceObject LineAppearance { get { return lineAppearance; } }
		public AppearanceObject HourLineAppearance { get { return hourLineAppearance; } }
		public AppearanceObject NowLineAppearance { get { return nowLineAppearance; } }
		public AppearanceObject NowAreaAppearance { get { return nowAreaAppearance; } }
		public AppearanceObject LargeHourAppearance { get { return largeHourAppearance; } }
		public ViewInfoItemCollection CurrentTimeItems { get { return currentTimeItems; } }
		public Rectangle ContentBounds { get { return contentBounds; } set { contentBounds = value; } }
		public Rectangle ClientBounds { get { return clientBounds; } set { clientBounds = value; } }
		public Rectangle HeaderBounds { get { return headerBounds; } set { headerBounds = value; } }
		public bool IsFirst { get { return isFirst; } set { isFirst = value; } }
		public ViewInfoTextItem HeaderCaptionItem { get { return headerCaptionItem; } set { headerCaptionItem = value; } }
		public object ShouldShowCurrentTime { get; set; }
		#endregion
		protected internal virtual Font CreateLargeFont(Font font) {
			return new Font(font.FontFamily, 2 * font.Size, font.Style, font.Unit, font.GdiCharSet, font.GdiVerticalFont);
		}
		protected internal override void CalcBorderBounds(BorderObjectPainter painter) {
			Rectangle oldBounds = this.Bounds;
			this.Bounds = this.ContentBounds;
			try {
				base.CalcBorderBounds(painter);
			} finally {
				this.Bounds = oldBounds;
			}
		}
		protected internal virtual void DisposeCurrentTimeItems() {
			DisposeItemsCore(CurrentTimeItems);
		}
	}
}
