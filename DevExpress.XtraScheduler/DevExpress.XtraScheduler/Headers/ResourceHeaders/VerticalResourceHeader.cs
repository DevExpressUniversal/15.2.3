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

using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraScheduler.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
namespace DevExpress.XtraScheduler.Drawing {
	public class VerticalResourceHeader : ResourceHeader {
		bool rotateCaption = false;
		public VerticalResourceHeader(BaseHeaderAppearance appearance, SchedulerResourceHeaderOptions options)
			: base(appearance, options) {
		}
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("VerticalResourceHeaderRotateCaption")]
#endif
		public override bool RotateCaption { get { return rotateCaption; } }
		protected internal override string CalcActualSkinElementName(bool hideSelection) {
			return SchedulerSkins.SkinHeaderResourceVertical;
		}
		protected internal override SchedulerHeaderComplexContentLayoutCalculator CreateComplexContentLayoutCalculator(SchedulerHeaderPreliminaryLayoutResult preliminaryResult) {
			if (preliminaryResult.RotateCaption)
				return CreateComplexContentLayoutCalculatorForRotatedCaption(preliminaryResult);
			else
				return CreateComplexContentLayoutCalculatorForNonRotatedCaption(preliminaryResult);
		}
		protected internal virtual SchedulerHeaderComplexContentLayoutCalculator CreateComplexContentLayoutCalculatorForNonRotatedCaption(SchedulerHeaderPreliminaryLayoutResult preliminaryResult) {
			switch (preliminaryResult.ImageAlign) {
				case HeaderImageAlign.Left:
					return new SchedulerHeaderComplexContentLayoutCalculatorBottomImageAlign(this, preliminaryResult);
				case HeaderImageAlign.Right:
					return new SchedulerHeaderComplexContentLayoutCalculatorTopImageAlign(this, preliminaryResult);
				case HeaderImageAlign.Top:
					return new SchedulerHeaderComplexContentLayoutCalculatorLeftImageAlign(this, preliminaryResult);
				case HeaderImageAlign.Bottom:
					return new SchedulerHeaderComplexContentLayoutCalculatorRightImageAlign(this, preliminaryResult);
				default:
					Exceptions.ThrowInternalException();
					return null;
			}
		}
		protected internal override void CustomizeIntermediateParametersCore(SchedulerHeaderPreliminaryLayoutResult preliminaryResult) {
			base.CustomizeIntermediateParametersCore(preliminaryResult);
			preliminaryResult.ImageAlign = CalculateActualImageAlign(preliminaryResult.ImageAlign);
		}
		protected internal override void CalcPreliminaryLayoutCore(GraphicsCache cache, SchedulerHeaderPreliminaryLayoutResult preliminaryResult) {
			this.rotateCaption = preliminaryResult.RotateCaption;
			base.CalcPreliminaryLayoutCore(cache, preliminaryResult);
		}
		protected internal virtual HeaderImageAlign CalculateActualImageAlign(HeaderImageAlign align) {
			switch (align) {
				case HeaderImageAlign.Left:
					return HeaderImageAlign.Right;
				case HeaderImageAlign.Right:
					return HeaderImageAlign.Left;
				default:
					return align;
			}
		}
		protected internal override Size CalculateOutputImageSize(Size size) {
			return new Size(size.Height, size.Width);
		}
		protected internal override Size CalculateTextSize(GraphicsCache cache, Size maxAvailableTextSize) {
			Size result = base.CalculateTextSize(cache, maxAvailableTextSize);
			return AdjustTextSize(cache, result);
		}
		protected internal virtual Size AdjustTextSize(GraphicsCache cache, Size preliminarySize) {
			return SchedulerWinUtils.FitSizeForVerticalText(preliminarySize);
		}
		protected internal override Size CalculateWrappedTextSizeForRotatedCaption(GraphicsCache cache, Size maxAvailableTextSize) {
			Size targetSize = new Size(maxAvailableTextSize.Height, maxAvailableTextSize.Width);
			Size result = EstimateOptimalSize(cache, targetSize);
			return new Size(result.Height, result.Width);
		}
		protected internal override Size CalculateTextSizeForRotatedCaption(GraphicsCache cache, Size maxAvailableTextSize) {
			Size targetSize = new Size(maxAvailableTextSize.Height, maxAvailableTextSize.Width);
			Size result = base.CalculateTextSize(cache, targetSize);
			return new Size(result.Height, result.Width);
		}
		protected internal virtual SizeF CalcTextSizeInternal(GraphicsCache cache, AppearanceObject appearance, string text) {
			return appearance.CalcTextSize(cache, text, Int32.MaxValue);
		}
		protected internal virtual int CalculateMinimalWidth(GraphicsCache cache) {
			string[] words = Regex.Split(Caption, " ");
			int count = words.Length;
			if (count <= 0)
				return 0;
			AppearanceObject captionAppearance = this.CaptionAppearance;
			float result = CalcTextSizeInternal(cache, captionAppearance, words[0]).Width;
			for (int i = 1; i < count; i++)
				result = Math.Max(result, CalcTextSizeInternal(cache, captionAppearance, words[i]).Width);
			return (int)Math.Ceiling(result);
		}
		protected internal virtual Size EstimateOptimalSize(GraphicsCache cache, Size targetSize) {
			SizeF textSize = CalcTextSizeInternal(cache, CaptionAppearance, Caption);
			Size result = new Size((int)Math.Ceiling(textSize.Width), (int)Math.Ceiling(textSize.Height));
			int widthRangeEnd = result.Width;
			if (widthRangeEnd <= 0)
				return targetSize;
			int minHeight = result.Height;
			if (minHeight > targetSize.Height)
				return new Size(result.Width, targetSize.Height);
			int minimalAcceptableWidth = CalculateMinimalWidth(cache);
			int widthRangeStart = minimalAcceptableWidth;
			if (widthRangeStart == widthRangeEnd)
				return result;
			while (widthRangeStart <= widthRangeEnd) {
				int width = (widthRangeStart + widthRangeEnd) >> 1;
				Size size = CalcCaptionSizeForFixedWidth(cache, width);
				if (size.Height <= targetSize.Height && size.Width <= result.Width && size.Width >= minimalAcceptableWidth) {
					result = size;
				}
				int deltaHeight = targetSize.Height - size.Height;
				if (deltaHeight == 0)
					return result;
				else {
					if (deltaHeight < 0)
						widthRangeStart = width + 1;
					else
						widthRangeEnd = width - 1;
				}
			}
			return result;
		}
		protected internal virtual Size CalcCaptionSizeForFixedWidth(GraphicsCache cache, int width) {
			SizeF textSize = CaptionAppearance.CalcTextSize(cache, this.Caption, width);
			return new Size((int)Math.Ceiling(textSize.Width), (int)Math.Ceiling(textSize.Height));
		}
		protected override void CalculateHeaderAppearances(SchedulerHeaderPainter painter) {
			SetAppearanceParameters(painter, Appearance.HeaderCaption, SchedulerSkins.SkinHeaderResourceVertical);
		}
		protected override Size CalculateResourceHeaderAutoSize(SchedulerHeaderPreliminaryLayoutResult preliminaryResult, int xPadding, int yPadding) {
			return CalculateResourceHeaderImageSize(Bounds.Width - xPadding, preliminaryResult.ViewSize.Width / 6 - yPadding);
		}
		protected override SchedulerHeader CreateCloneInstance() {
			return new VerticalResourceHeader(Appearance, Options);
		}
	}
}
