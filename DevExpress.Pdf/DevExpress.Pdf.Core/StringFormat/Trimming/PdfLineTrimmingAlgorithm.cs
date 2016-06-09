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

namespace DevExpress.Pdf.Native {
	public abstract class PdfLineTrimmingAlgorithm {
		public static PdfLineTrimmingAlgorithm Create(PdfLineFormatterState state, PdfStringGlyphRun ellipsis, PdfStringTrimming trimming) {
			switch (trimming) {
				case PdfStringTrimming.Character:
					return new PdfLineTrimmingCharAlgorithm(state, ellipsis);
				case PdfStringTrimming.Word:
					return new PdfLineTrimmingWordAlgorithm(state, ellipsis);
				case PdfStringTrimming.EllipsisCharacter:
					return new PdfLineTrimmingEllipsisCharAlgorithm(state, ellipsis);
				case PdfStringTrimming.EllipsisWord:
					return new PdfLineTrimmingEllipsisWordAlgorithm(state, ellipsis);
				default:
					return null;
			}
		}
		readonly PdfLineFormatterState state;
		readonly PdfStringGlyphRun ellipsis;
		int ellipsisPosition = 0;
		protected virtual bool UseEllipsis { get { return false; } }
		protected PdfLineFormatterState State { get { return state; } }
		protected PdfStringGlyphRun Ellipsis { get { return ellipsis; } }
		protected PdfLineTrimmingAlgorithm(PdfLineFormatterState state, PdfStringGlyphRun ellipsis) {
			this.state = state;
			this.ellipsis = ellipsis;
		}
		protected void TryInsertEllipsis() {
			if (ellipsisPosition != 0) {
				double layoutWidth = state.LayoutWidth;
				double ellipsisWidth = ellipsis.Width;
				int glyphsToRemove = state.CurrentLineGlyphCount - ellipsisPosition;
				for (int i = 0; i < glyphsToRemove; i++)
					state.RemoveLastGlyph();
				if (state.CurrentLineWidth + ellipsisWidth <= layoutWidth)
					state.Append(ellipsis);
			}
		}
		protected void SaveEllipsisPosition() {
			if (state.CurrentLineWidth + ellipsis.Width <= state.LayoutWidth)
				ellipsisPosition = state.CurrentLineGlyphCount;
		}
		public abstract bool ProcessWord(PdfStringGlyphRun word);
	}
}
