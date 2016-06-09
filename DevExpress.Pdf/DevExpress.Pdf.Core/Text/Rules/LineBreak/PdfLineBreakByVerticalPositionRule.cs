#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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

using System;
namespace DevExpress.Pdf.Native {
	public class PdfLineBreakByVerticalPositionRule : IPdfTextRule {
		const double lineOverlapFactor = 0.7;
		public PdfLineBreakByVerticalPositionRule() {
		}
		bool IsIndex(PdfTextParserState parserState) {
			const double overlapFactor = 0.35;
			const double indexHeightRatio = 1.3;
			PdfTextBlock block1 = parserState.PreviousCharacterBlock;
			PdfTextBlock block2 = parserState.CurrentCharacterBlock;
			double block1FontHeight = block1.FontHeight;
			double block2FontHeight = block2.FontHeight;
			PdfPoint block1TopLeft = parserState.PreviousCharacter.Rectangle.TopLeft;
			PdfPoint block2TopLeft = parserState.CurrentCharacter.Rectangle.TopLeft;
			double block1Angle = block1.Angle;
			double block2Angle = block2.Angle;
			if (block1FontHeight / indexHeightRatio > block2FontHeight || block2FontHeight / indexHeightRatio > block1FontHeight) {
				PdfPoint rowBlockTopLeft = block1TopLeft;
				PdfPoint indexBlockTopLeft = block2TopLeft;
				double rowBlockHeight = block1FontHeight;
				double indexBlockHeight = block2FontHeight;
				double rowBlockAngle = block1Angle;
				double indexBlockAngle = block2Angle;
				if (block1FontHeight < block2FontHeight) {
					rowBlockTopLeft = block2TopLeft;
					indexBlockTopLeft = block1TopLeft;
					rowBlockHeight = block2FontHeight;
					indexBlockHeight = block1FontHeight;
					rowBlockAngle = block2Angle;
					indexBlockAngle = block1Angle;
				}
				double overlapValue = indexBlockHeight * overlapFactor;
				PdfPoint rotatedRowBlockTopLeft = PdfTextUtils.RotatePoint(rowBlockTopLeft, -rowBlockAngle);
				PdfPoint rotatedIndexBlockTopLeft = PdfTextUtils.RotatePoint(indexBlockTopLeft, -indexBlockAngle);
				double actualOverlap = PdfMathUtils.Min(rotatedRowBlockTopLeft.Y, rotatedIndexBlockTopLeft.Y) - PdfMathUtils.Max(rotatedRowBlockTopLeft.Y - rowBlockHeight, 
					rotatedIndexBlockTopLeft.Y - indexBlockHeight);
				return actualOverlap >= overlapValue;
			}
			return false;
		}
		public bool IsApplicable(PdfTextParserState parserState) {
			PdfTextBlock previousBlock = parserState.PreviousCharacterBlock;
			PdfPoint previousBlockLocation = previousBlock.Location;
			PdfTextBlock currentBlock = parserState.CurrentCharacterBlock;
			PdfPoint currentBlockLocation = currentBlock.Location;
			double previousBlockAngle = previousBlock.Angle;
			double currentBlockAngle = currentBlock.Angle;
			double verticalDifference = Math.Abs(PdfTextUtils.RotatePoint(previousBlockLocation, -previousBlockAngle).Y - PdfTextUtils.RotatePoint(currentBlockLocation, -currentBlockAngle).Y);
			double blockFontHeight = Math.Min(currentBlock.FontHeight, previousBlock.FontHeight);
			return verticalDifference > blockFontHeight * (1 - lineOverlapFactor) && !IsIndex(parserState);
		}
	}
}
