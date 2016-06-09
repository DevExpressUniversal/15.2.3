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

using System.Text;
using System.Collections.Generic;
namespace DevExpress.Pdf.Native {
	public class PdfNotEmbeddedLineFormatterState : PdfLineFormatterState {
		public PdfNotEmbeddedLineFormatterState(PdfRectangle layout)
			: base(layout) {
		}
		protected override PdfStringGlyphRun CreateGlyphRun() {
			return new PdfNotEmbeddedStringGlyphRun();
		}
	}
	public class PdfLineFormatterState {
		readonly PdfRectangle layout;
		readonly IList<PdfStringGlyphRun> lines = new List<PdfStringGlyphRun>();
		PdfStringGlyphRun currentLine;
		public IList<PdfStringGlyphRun> Lines {
			get {
				int last = lines.Count - 1;
				if (lines.Count > 0 && lines[last].IsEmpty) {
					List<PdfStringGlyphRun> nonEmpty = new List<PdfStringGlyphRun>(lines);
					nonEmpty.RemoveAt(last);
					return nonEmpty;
				}
				return lines;
			}
		}
		public bool IsCurrentLineEmpty { get { return currentLine.IsEmpty; } }
		public int CurrentLineGlyphCount { get { return currentLine.Glyphs.Count; } }
		public double LayoutWidth { get { return layout.Width; } }
		public double CurrentLineWidth { get { return currentLine.Width; } }
		public PdfLineFormatterState(PdfRectangle layout) {
			this.layout = layout;
			currentLine = CreateGlyphRun();
		}
		public void Append(PdfStringGlyph glyph) {
			currentLine.Append(glyph);
		}
		public void Append(PdfStringGlyphRun text) {
			currentLine.Append(text);
		}
		public void RemoveLastGlyph() {
			currentLine.RemoveLast();
		}
		public void FinishLine() {
			lines.Add(currentLine);
			currentLine = CreateGlyphRun();
		}
		protected virtual PdfStringGlyphRun CreateGlyphRun() {
			return new PdfStringGlyphRun();
		}
	}
}
