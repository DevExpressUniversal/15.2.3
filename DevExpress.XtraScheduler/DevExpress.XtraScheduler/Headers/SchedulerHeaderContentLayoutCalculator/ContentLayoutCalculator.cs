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
namespace DevExpress.XtraScheduler.Drawing {
	public abstract class SchedulerHeaderContentLayoutCalculator {
		#region Fields
		SchedulerHeader header;
		SchedulerHeaderPreliminaryLayoutResult preliminaryResult;
		#endregion
		protected SchedulerHeaderContentLayoutCalculator(SchedulerHeader header, SchedulerHeaderPreliminaryLayoutResult preliminaryResult) {
			if (header == null)
				Exceptions.ThrowArgumentException("header", header);
			if (preliminaryResult == null)
				Exceptions.ThrowArgumentException("preliminaryResult", preliminaryResult);
			this.header = header;
			this.preliminaryResult = preliminaryResult;
		}
		#region Properties
		protected internal SchedulerHeader Header { get { return header; } }
		protected internal SchedulerHeaderPreliminaryLayoutResult PreliminaryResult { get { return preliminaryResult; } }
		#endregion
		protected internal virtual int CalculatePreliminaryContentWidth(SchedulerHeaderPreliminaryLayoutResult preliminaryLayout) {
			SchedulerHeaderPainter painter = preliminaryLayout.Painter;
			int width = Header.Bounds.Width;
			if (Header.HasLeftBorder)
				width -= painter.GetLeftBorderWidth(Header);
			if (Header.HasRightBorder)
				width -= painter.GetRightBorderWidth(Header);
			if (Header.HasSeparator)
				width -= painter.HeaderSeparatorWidth;
			width -= preliminaryLayout.ContentLeftPadding;
			width -= preliminaryLayout.ContentRightPadding;
			return Math.Max(0, width);
		}
		protected internal virtual int CalculateHeaderHeight(int textHeight, SchedulerHeaderPainter painter, SchedulerHeaderPreliminaryLayoutResult preliminaryLayout) {
			int height = textHeight;
			if (Header.HasTopBorder)
				height += painter.GetTopBorderWidth(Header);
			if (Header.HasBottomBorder)
				height += painter.GetBottomBorderWidth(Header);
			height += painter.CaptionLineWidth;
			height += preliminaryLayout.ContentTopPadding;
			height += preliminaryLayout.ContentBottomPadding;
			return height;
		}
		protected internal virtual int CalculateAvailableContentHeight(int headerHeight, SchedulerHeaderPainter painter, SchedulerHeaderPreliminaryLayoutResult preliminaryLayout) {
			int height = headerHeight;
			if (Header.HasTopBorder)
				height -= painter.GetTopBorderWidth(Header);
			;
			if (Header.HasBottomBorder)
				height -= painter.GetBottomBorderWidth(Header);
			;
			height -= painter.CaptionLineWidth;
			height -= preliminaryLayout.ContentTopPadding;
			height -= preliminaryLayout.ContentBottomPadding;
			return height;
		}
		protected internal virtual Size CalculateUnclippedOutputTextSize(GraphicsCache cache, Size maxAvailableTextSize) {
			if (!PreliminaryResult.RotateCaption) {
				if (SchedulerWinUtils.IsWordWrap(Header.CaptionAppearance))
					return Header.CalculateWrappedTextSizeForRotatedCaption(cache, maxAvailableTextSize);
				else
					return Header.CalculateTextSizeForRotatedCaption(cache, maxAvailableTextSize);
			} else
				return Header.CalculateTextSize(cache, maxAvailableTextSize);
		}
		protected internal virtual void UpdateToolTipVisibility(GraphicsCache cache) {
			if (String.IsNullOrEmpty(Header.Caption))
				return;  
			if (Header.AllowAbbreviatedCaption)
				return;
			Size clippedTextSize = CalcClippedTextSize();
			if (IsTextSizeTooSmall(clippedTextSize))
				Header.ShouldShowToolTip = true;
			else {
				if (SchedulerWinUtils.IsWordWrap(Header.CaptionAppearance)) {
					if ((Header is VerticalResourceHeader) && !PreliminaryResult.RotateCaption)
						Header.ShouldShowToolTip = CalculateWrappedVerticalHeaderToolTipVisibility(cache, clippedTextSize);
					else
						Header.ShouldShowToolTip = CalculateWrappedHeaderToolTipVisibility(cache, clippedTextSize);
				} else
					Header.ShouldShowToolTip = CalculateUnwrappedHeaderToolTipVisibility(cache, clippedTextSize);
			}
		}
		protected internal virtual bool CalculateUnwrappedHeaderToolTipVisibility(GraphicsCache cache, Size clippedTextSize) {
			Size size = CalculateUnclippedOutputTextSize(cache, new Size(Int32.MaxValue, Int32.MaxValue));
			return (size.Width > clippedTextSize.Width) || (size.Height > clippedTextSize.Height);
		}
		protected internal virtual bool CalculateWrappedHeaderToolTipVisibility(GraphicsCache cache, Size clippedTextSize) {
			Size size = CalculateUnclippedOutputTextSize(cache, new Size(clippedTextSize.Width, Int32.MaxValue));
			return size.Height > clippedTextSize.Height;
		}
		protected internal virtual bool CalculateWrappedVerticalHeaderToolTipVisibility(GraphicsCache cache, Size clippedTextSize) {
			Size size = Header.CalculateTextSize(cache, new Size(clippedTextSize.Height, Int32.MaxValue));
			return size.Height > clippedTextSize.Width;
		}
		protected internal Size CalcClippedTextSize() {
			Rectangle clippedTextBounds = Rectangle.Intersect(Header.TextBounds, Header.ContentBounds);
			return clippedTextBounds.Size;
		}
		protected internal bool IsTextSizeTooSmall(Size clippedTextSize) {
			return clippedTextSize.Width <= 2 || clippedTextSize.Height <= 2;
		}
		public abstract void CalcLayout(GraphicsCache cache);
		public abstract void CalcPreliminaryLayout(GraphicsCache cache);
	}
}
