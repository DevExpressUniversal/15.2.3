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
	public abstract class SchedulerHeaderComplexContentLayoutCalculatorHorizontalImageAlign : SchedulerHeaderComplexContentLayoutCalculator {
		protected SchedulerHeaderComplexContentLayoutCalculatorHorizontalImageAlign(SchedulerHeader header, SchedulerHeaderPreliminaryLayoutResult preliminaryResult)
			: base(header, preliminaryResult) {
		}
		protected internal virtual void AlignImageVertically() {
			int heightDifference = Header.ContentBounds.Height - ImageHeight;
			if (heightDifference < 0) {
				if (PreliminaryResult.ImageSizeMode == HeaderImageSizeMode.CenterImage)
					ImageY += heightDifference / 2;
			} else {
				ImageY += heightDifference / 2;
			}
		}
		protected internal virtual void AlignTextVertically() {
			int heightDifference = Header.ContentBounds.Height - TextHeight;
			XtraSchedulerDebug.Assert(heightDifference >= 0);
			TextY += heightDifference / 2;
		}
		protected internal override void LayoutImageAndText() {
			AlignImageVertically();
			AlignTextVertically();
			AlignImageAndTextHorizontally();
		}
		protected internal override Size CalculateMaxAvailableTextSize(Size outputImageSize, Size contentBoundsSize) {
			return new Size(Math.Max(0, contentBoundsSize.Width - outputImageSize.Width - PreliminaryResult.ImageTextGapSize), contentBoundsSize.Height);
		}
		protected internal override Size CalcTotalContentSize() {
			SchedulerHeaderPainter headerPainter = (SchedulerHeaderPainter)PreliminaryResult.Painter;
			int headerHeight = CalculateHeaderHeight(Math.Max(PreliminaryResult.TextSize.Height, PreliminaryResult.ImageSize.Height), headerPainter, PreliminaryResult);
			return new Size(Header.Bounds.Width, headerHeight);
		}
		protected internal abstract void AlignImageAndTextHorizontally();
	}
}
