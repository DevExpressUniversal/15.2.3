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
	public abstract class SchedulerHeaderComplexContentLayoutCalculator : SchedulerHeaderContentLayoutCalculator {
		Rectangle fImageBounds;
		Rectangle fTextBounds;
		protected SchedulerHeaderComplexContentLayoutCalculator(SchedulerHeader header, SchedulerHeaderPreliminaryLayoutResult preliminaryResult)
			: base(header, preliminaryResult) {
		}
		public Rectangle ImageBounds { get { return fImageBounds; } set { fImageBounds = value; } }
		public Rectangle TextBounds { get { return fTextBounds; } set { fTextBounds = value; } }
		public int ImageWidth { get { return fImageBounds.Width; } }
		public int ImageHeight { get { return fImageBounds.Height; } }
		public int TextWidth { get { return fTextBounds.Width; } }
		public int TextHeight { get { return fTextBounds.Height; } }
		public int ImageX { get { return fImageBounds.X; } set { fImageBounds.X = value; } }
		public int ImageY { get { return fImageBounds.Y; } set { fImageBounds.Y = value; } }
		public int TextX { get { return fTextBounds.X; } set { fTextBounds.X = value; } }
		public int TextY { get { return fTextBounds.Y; } set { fTextBounds.Y = value; } }
		public override void CalcPreliminaryLayout(GraphicsCache cache) {
			int contentWidth = CalculatePreliminaryContentWidth(PreliminaryResult);
			Size outputImageSize = CalculateOutputImageSize();
			PreliminaryResult.ImageSize = outputImageSize;
			if (PreliminaryResult.FixedHeaderHeight == 0) {
				Size maxAvailableTextSize = CalculateMaxAvailableTextSize(outputImageSize, new Size(contentWidth, Int32.MaxValue));
				Size outputTextSize = CalculateUnclippedOutputTextSize(cache, maxAvailableTextSize);
				PreliminaryResult.TextSize = outputTextSize;
				PreliminaryResult.Size = CalcTotalContentSize();
			} else {
				PreliminaryResult.Size = new Size(Header.Bounds.Width, PreliminaryResult.FixedHeaderHeight);
				Size contentSize = new Size(contentWidth, CalculateAvailableContentHeight(PreliminaryResult.FixedHeaderHeight, PreliminaryResult.Painter, PreliminaryResult));
				Size maxAvailableTextSize = CalculateMaxAvailableTextSize(outputImageSize, contentSize);
				PreliminaryResult.TextSize = ClipToSize(CalculateUnclippedOutputTextSize(cache, maxAvailableTextSize), contentSize);
			}
		}
		public override void CalcLayout(GraphicsCache cache) {
			if (cache == null)
				Exceptions.ThrowArgumentException("cache", cache);
			Size maxAvailableTextSize = CalculateMaxAvailableTextSize(PreliminaryResult.ImageSize, Header.ContentBounds.Size);
			Size outputTextSize = CalculateOutputTextSize(cache, maxAvailableTextSize);
			CreateInitialImageAndTextBounds(PreliminaryResult.ImageSize, outputTextSize);
			LayoutImageAndText();
			Header.ImageBounds = this.fImageBounds;
			Header.TextBounds = this.fTextBounds;
			UpdateToolTipVisibility(cache);
		}
		protected internal virtual void CreateInitialImageAndTextBounds(Size outputImageSize, Size outputTextSize) {
			this.fImageBounds = new Rectangle(Header.ContentBounds.Location, outputImageSize);
			this.fTextBounds = new Rectangle(Header.ContentBounds.Location, outputTextSize);
		}
		protected internal virtual Size CalculateOutputImageSize() {
			return Header.CalculateOutputImageSize(CalculateOutputImageSizeCore());
		}
		protected internal virtual Size CalculateOutputImageSizeCore() {
			switch (PreliminaryResult.ImageSizeMode) {
				case HeaderImageSizeMode.Normal:
					return Header.Image.Size;
				case HeaderImageSizeMode.CenterImage:
					return Header.Image.Size;
				case HeaderImageSizeMode.StretchImage:
					return PreliminaryResult.FixedImageSize;
				case HeaderImageSizeMode.ZoomImage:
					return CalcZoomedImageSize(Header.Image.Size, PreliminaryResult.FixedImageSize);
				default:
					Exceptions.ThrowInternalException();
					return Size.Empty;
			}
		}
		protected internal virtual Size CalculateOutputTextSize(GraphicsCache cache, Size maxAvailableTextSize) {
			Size size = CalculateUnclippedOutputTextSize(cache, maxAvailableTextSize);
			return ClipToSize(size, Header.ContentBounds.Size);
		}
		protected internal abstract Size CalculateMaxAvailableTextSize(Size outputImageSize, Size contentBoundsSize);
		protected internal abstract void LayoutImageAndText();
		protected internal abstract Size CalcTotalContentSize();
		internal Size CalcZoomedImageSize(Size imageSize, Size targetImageSize) {
			if (imageSize.Width == 0 || imageSize.Height == 0)
				return Size.Empty;
			float zoomX = targetImageSize.Width / (float)imageSize.Width;
			float zoomY = targetImageSize.Height / (float)imageSize.Height;
			float zoom = Math.Min(zoomX, zoomY);
			return new Size((int)(imageSize.Width * zoom), (int)(imageSize.Height * zoom));
		}
		internal Size ClipToSize(Size size, Size targetSize) {
			return new Size(Math.Min(targetSize.Width, size.Width), Math.Min(targetSize.Height, size.Height));
		}
	}
}
