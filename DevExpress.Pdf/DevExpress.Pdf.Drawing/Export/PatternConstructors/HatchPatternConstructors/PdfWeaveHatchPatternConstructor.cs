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

using System.Drawing;
namespace DevExpress.Pdf.Drawing {
	public class PdfWeaveHatchPatternConstructor : PdfRectangleBasedHatchPatternConstructor {
		static PdfHatchPatternRect[] rectangles;
		static PdfWeaveHatchPatternConstructor() {
			rectangles = new PdfHatchPatternRect[19];
			int i = 0;
			for (int j = 0; j < 5; j++) {
				rectangles[i++] = new PdfHatchPatternRect(0 + j, 4 - j);
				rectangles[i++] = new PdfHatchPatternRect(1 + j, 7 - j);
			}
			for (int j = 0; j < 3; j++) {
				rectangles[i++] = new PdfHatchPatternRect(5 + j, 1 + j);
				rectangles[i++] = new PdfHatchPatternRect(7 - j, 7 - j);
			}
			for (int j = 0; j < 2; j++)
				rectangles[i++] = new PdfHatchPatternRect(j, j);
			rectangles[i] = new PdfHatchPatternRect(3, 7);
		}
		public PdfWeaveHatchPatternConstructor(Color foreColor, Color backColor)
			: base(foreColor, backColor, rectangles) {
		}
	}
}
