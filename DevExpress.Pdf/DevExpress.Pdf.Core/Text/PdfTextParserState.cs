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

using System.Collections.Generic;
namespace DevExpress.Pdf.Native {
	public class PdfTextParserState {
		readonly IPdfTextRule lineBreakRules = new PdfRuleSet(new IPdfTextRule[] {
			new PdfLineBreakByAngleRule(),
			new PdfLineBreakByVerticalPositionRule()
		});
		readonly IPdfTextRule spaceBreakRules = new PdfRuleSet(new IPdfTextRule[] {
			new PdfWhitespaceBreakByPositionRule(),
			new PdfWhitespaceBreakBySymbolRule(),
		});
		readonly IPdfTextRule textWrapRule = new PdfTextWrapRule();
		readonly IPdfTextRule separatorRule = new PdfWhitespaceBreakBySeparatorRule();
		readonly IPdfTextRule rtlRule = new PdfTextRTLRule();
		readonly IList<PdfTextBlock> blocks;
		readonly PdfRectangle pageCropBox;
		PdfTextBlock previousCharacterBlock;
		PdfTextBlock currentCharacterBlock;
		PdfCharacter previousCharacter;
		PdfCharacter currentCharacter;
		PdfPoint currentCharLocation;
		PdfPoint previousCharLocation;
		int characterIndex;
		int blockIndex;
		bool isFinished;
		bool isRtl;
		bool isLineStart;
		bool isSpace;
		bool isSeparator;
		bool isWrap;
		bool actualRtl = false;
		public PdfTextBlock CurrentCharacterBlock { get { return currentCharacterBlock; } }
		public PdfTextBlock PreviousCharacterBlock { get { return previousCharacterBlock; } }
		public PdfCharacter CurrentCharacter { get { return currentCharacter; } }
		public PdfCharacter PreviousCharacter { get { return previousCharacter; } }
		public PdfPoint CurrentCharLocation { get { return currentCharLocation; } }
		public PdfPoint PreviousCharLocation { get { return previousCharLocation; } }
		public bool IsSpace { get { return isSpace; } }
		public bool IsLineStart { get { return isLineStart; } }
		public bool IsSeparator { get { return isSeparator; } }
		public bool IsFinished { get { return isFinished; } }
		public bool IsWrap { get { return isWrap; } }
		public bool IsRtl {
			get {
				if (!actualRtl) {
					isRtl = rtlRule.IsApplicable(this);
					actualRtl = true;
				}
				return isRtl;
			}
		}
		public PdfRectangle PageCropBox { get { return pageCropBox; } }
		public PdfTextParserState(IList<PdfTextBlock> blocks, PdfRectangle pageCropBox) {
			this.blocks = blocks;
			this.pageCropBox = pageCropBox;
			currentCharacterBlock = blocks[0];
			previousCharacterBlock = currentCharacterBlock;
			currentCharacter = currentCharacterBlock.Characters[0];
			previousCharacter = currentCharacter;
			currentCharLocation = currentCharacterBlock.CharLocations[0];
			previousCharLocation = currentCharLocation;
		}
		bool MoveToNextCharacter() {
			previousCharacter = currentCharacter;
			previousCharLocation = currentCharLocation;
			if (++characterIndex >= currentCharacterBlock.Characters.Count)
				return false;
			previousCharacterBlock = currentCharacterBlock;
			currentCharacter = currentCharacterBlock.Characters[characterIndex];
			currentCharLocation = currentCharacterBlock.CharLocations[characterIndex];
			return true;
		}
		bool MoveToNextBlock() {
			previousCharacterBlock = currentCharacterBlock;
			if (++blockIndex >= blocks.Count)
				return false;
			currentCharacterBlock = blocks[blockIndex];
			characterIndex = 0;
			previousCharacter = currentCharacter;
			previousCharLocation = currentCharLocation;
			currentCharLocation = currentCharacterBlock.CharLocations[characterIndex];
			currentCharacter = currentCharacterBlock.Characters[characterIndex];
			return true;
		}
		public void MoveNext() {
			if (!MoveToNextCharacter())
				if (!MoveToNextBlock()) {
					isFinished = true;
					return;
				}
			actualRtl = false;
			isLineStart = lineBreakRules.IsApplicable(this);
			isSpace = isLineStart || spaceBreakRules.IsApplicable(this);
			isSeparator = separatorRule.IsApplicable(this);
			isWrap = isLineStart && textWrapRule.IsApplicable(this);
		}
	}
}
