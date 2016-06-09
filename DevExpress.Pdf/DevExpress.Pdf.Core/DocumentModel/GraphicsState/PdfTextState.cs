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
	public class PdfTextState {
		double characterSpacing;
		double wordSpacing;
		double horizontalScaling = 100;
		double leading;
		PdfFont font;
		double fontSize;
		PdfTextRenderingMode renderingMode;
		double rise;
		double knockout;
		PdfTransformationMatrix textLineMatrix = new PdfTransformationMatrix();
		PdfTransformationMatrix textMatrix = new PdfTransformationMatrix();
		public double CharacterSpacing {
			get { return characterSpacing; }
			set { characterSpacing = value; }
		}
		public double WordSpacing {
			get { return wordSpacing; }
			set { wordSpacing = value; }
		}
		public double HorizontalScaling {
			get { return horizontalScaling; }
			set { horizontalScaling = value; }
		}
		public double Leading {
			get { return leading; }
			set { leading = value; }
		}
		public PdfFont Font {
			get { return font; }
			set { font = value; }
		}
		public double FontSize {
			get { return fontSize; }
			set { fontSize = value; }
		}
		public PdfTextRenderingMode RenderingMode {
			get { return renderingMode; }
			set { renderingMode = value; }
		}
		public double Rise {
			get { return rise; }
			set { rise = value; }
		}
		public double Knockout {
			get { return knockout; }
			set { knockout = value; }
		}
		public PdfTransformationMatrix TextLineMatrix {
			get { return textLineMatrix; }
			set { textLineMatrix = value; }
		}
		public PdfTransformationMatrix TextMatrix {
			get { return textMatrix; }
			set { textMatrix = value; }
		}
		public PdfTransformationMatrix TextRenderingMatrix { get { return PdfTransformationMatrix.Multiply(new PdfTransformationMatrix(fontSize * horizontalScaling, 0, 0, fontSize, 0, rise), textMatrix); } }
		public PdfTextState Clone() {
			PdfTextState pdfTextState = new PdfTextState();
			pdfTextState.characterSpacing = characterSpacing;
			pdfTextState.wordSpacing = wordSpacing;
			pdfTextState.horizontalScaling = horizontalScaling;
			pdfTextState.leading = leading;
			pdfTextState.font = font;
			pdfTextState.fontSize = fontSize;
			pdfTextState.renderingMode = renderingMode;
			pdfTextState.rise = rise;
			pdfTextState.knockout = knockout;
			return pdfTextState;
		}
	}
}
