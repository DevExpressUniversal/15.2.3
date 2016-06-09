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
	public class PdfShingleHatchPatternConstructor : PdfRectangleBasedHatchPatternConstructor {
		static PdfHatchPatternRect[] rectangles;
		static PdfShingleHatchPatternConstructor() {
			rectangles = new PdfHatchPatternRect[10];
			int i = 0;
			for (int j = 0; j < 3; j++) {
				rectangles[i++] = new PdfHatchPatternRect(5 - j, j + 1);
				rectangles[i++] = new PdfHatchPatternRect(j, j + 1);
			}
			rectangles[i++] = new PdfHatchPatternRect(6, 0, 2, 1);
			rectangles[i++] = new PdfHatchPatternRect(4, 4, 2, 1);
			rectangles[i++] = new PdfHatchPatternRect(6, 5);
			rectangles[i] = new PdfHatchPatternRect(7, 6, 1, 2);
		}
		public PdfShingleHatchPatternConstructor(Color foreColor, Color backColor)
			: base(foreColor, backColor, rectangles) {
		}
	}
}
