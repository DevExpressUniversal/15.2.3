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

using DevExpress.XtraScheduler.Internal.Diagnostics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Drawing {
	public abstract class SchedulerHeaderComplexContentLayoutCalculatorVerticalImageAlign : SchedulerHeaderComplexContentLayoutCalculator {
		protected SchedulerHeaderComplexContentLayoutCalculatorVerticalImageAlign(SchedulerHeader header, SchedulerHeaderPreliminaryLayoutResult preliminaryResult)
			: base(header, preliminaryResult) {
		}
		protected internal virtual void AlignImageHorizontally() {
			int widthDifference = Header.ContentBounds.Width - ImageWidth;
			if (widthDifference < 0) {
				if (PreliminaryResult.ImageSizeMode == HeaderImageSizeMode.CenterImage)
					ImageX += widthDifference / 2;
			} else {
				ImageX += widthDifference / 2;
			}
		}
		protected internal virtual void AlignTextHorizontally() {
			int widthDifference = Header.ContentBounds.Width - TextWidth;
			XtraSchedulerDebug.Assert(widthDifference >= 0);
			TextX += widthDifference / 2;
		}
		protected internal override void LayoutImageAndText() {
			AlignImageHorizontally();
			AlignTextHorizontally();
			AlignImageAndTextVertically();
		}
		protected internal override Size CalculateMaxAvailableTextSize(Size outputImageSize, Size contentBoundsSize) {
			return new Size(contentBoundsSize.Width, Math.Max(0, contentBoundsSize.Height - outputImageSize.Height - PreliminaryResult.ImageTextGapSize));
		}
		protected internal override Size CalcTotalContentSize() {
			SchedulerHeaderPainter headerPainter = (SchedulerHeaderPainter)PreliminaryResult.Painter;
			int headerHeight = CalculateHeaderHeight(PreliminaryResult.TextSize.Height + PreliminaryResult.ImageSize.Height + PreliminaryResult.ImageTextGapSize, headerPainter, PreliminaryResult);
			return new Size(Header.Bounds.Width, headerHeight);
		}
		protected internal abstract void AlignImageAndTextVertically();
	}
}
