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
using System.Collections.Generic;
namespace DevExpress.Pdf.Drawing {
	public class PdfPercentHatchPatternConstructor : PdfHatchPatternConstructor {
		readonly int[] elementsCount;
		readonly int[] offsets;
		protected double RectangleSize { get { return LineStep / 8; } }
		public PdfPercentHatchPatternConstructor(Color foreColor, Color backColor, int[] elementsCount, int[] offsets)
			: base(foreColor, backColor) {
			this.elementsCount = elementsCount;
			this.offsets = offsets;
		}
		protected override void GetCommands() {
			base.GetCommands();
			double rectangleSize = RectangleSize;
			for (int i = 0; i < elementsCount.Length; i++) {
				int count = elementsCount[i];
				double xPosition = i * LineStep / elementsCount.Length;
				for (int j = 0; j < count; j++) {
					double yPosition = j * LineStep / count + offsets[i] * rectangleSize;
					Constructor.FillRectangle(new PdfRectangle(xPosition, yPosition, xPosition + rectangleSize, yPosition + rectangleSize));
				}
			}
		}
	}
	public class PdfPercent05HatchPatternConstructor : PdfPercentHatchPatternConstructor {
		static int[] elementsCount = new[] { 1, 1 };
		static int[] offsets = new[] { 0, 4 };
		public PdfPercent05HatchPatternConstructor(Color foreColor, Color backColor)
			: base(foreColor, backColor, elementsCount, offsets) {
		}
	}
	public class PdfPercent10HatchPatternConstructor : PdfPercentHatchPatternConstructor {
		static int[] elementsCount = new[] { 2, 2 };
		static int[] offsets = new[] { 0, 2 };
		public PdfPercent10HatchPatternConstructor(Color foreColor, Color backColor)
			: base(foreColor, backColor, elementsCount, offsets) {
		}
	}
	public class PdfPercent20HatchPatternConstructor : PdfPercentHatchPatternConstructor {
		static int[] elementsCount = new[] { 2, 2, 2, 2 };
		static int[] offsets = new[] { 0, 2, 0, 2 };
		public PdfPercent20HatchPatternConstructor(Color foreColor, Color backColor)
			: base(foreColor, backColor, elementsCount, offsets) {
		}
	}
	public class PdfPercent25HatchPatternConstructor : PdfPercentHatchPatternConstructor {
		static int[] elementsCount = new[] { 4, 4, 4, 4 };
		static int[] offsets = new[] { 0, 1, 0, 1 };
		public PdfPercent25HatchPatternConstructor(Color foreColor, Color backColor)
			: base(foreColor, backColor, elementsCount, offsets) {
		}
	}
	public class PdfPercent30HatchPatternConstructor : PdfPercentHatchPatternConstructor {
		static int[] elementsCount = new[] { 4, 2, 4, 2, 4, 2, 4, 2 };
		static int[] offsets = new[] { 0, 1, 0, 3, 0, 1, 0, 3 };
		public PdfPercent30HatchPatternConstructor(Color foreColor, Color backColor)
			: base(foreColor, backColor, elementsCount, offsets) {
		}
	}
	public class PdfPercent40HatchPatternConstructor : PdfPercentHatchPatternConstructor {
		static int[] elementsCount = new[] { 4, 0, 4, 4, 4, 0, 4, 4 };
		static int[] offsets = new[] { 0, 0, 0, 1, 0, 4, 0, 1 };
		public PdfPercent40HatchPatternConstructor(Color foreColor, Color backColor)
			: base(foreColor, backColor, elementsCount, offsets) {
		}
		protected override void GetCommands() {
			base.GetCommands();
			double rectangleSize = RectangleSize;
			for (int i = 1; i < 6; i += 2) {
				Constructor.FillRectangle(new PdfRectangle(rectangleSize, i * rectangleSize, rectangleSize * 2, (i + 1) * rectangleSize));
				double xPosition = 5 * LineStep / 8;
				double y = ((i + 4) % 8)* rectangleSize;
				Constructor.FillRectangle(new PdfRectangle(xPosition, y, xPosition + rectangleSize, y + rectangleSize));
			}
		}
	}
	public class PdfPercent50HatchPatternConstructor : PdfPercentHatchPatternConstructor {
		static int[] elementsCount = new[] { 4, 4, 4, 4, 4, 4, 4, 4 };
		static int[] offsets = new[] { 0, 1, 0, 1, 0, 1, 0, 1 };
		public PdfPercent50HatchPatternConstructor(Color foreColor, Color backColor)
			: base(foreColor, backColor, elementsCount, offsets) {
		}
	}
	public class PdfPercent60HatchPatternConstructor : PdfPercent30HatchPatternConstructor {
		public PdfPercent60HatchPatternConstructor(Color foreColor, Color backColor)
			: base(backColor, foreColor) {
			MultipleTransform(new PdfTransformationMatrix(1, 0, 0, 1, 0, -RectangleSize));
		}
	}
	public class PdfPercent70HatchPatternConstructor : PdfPercent25HatchPatternConstructor {
		public PdfPercent70HatchPatternConstructor(Color foreColor, Color backColor)
			: base(backColor, foreColor) {
		}
	}
	public class PdfPercent75HatchPatternConstructor : PdfPercent20HatchPatternConstructor {
		public PdfPercent75HatchPatternConstructor(Color foreColor, Color backColor)
			: base(backColor, foreColor) {
		}
	}
	public class PdfPercent80HatchPatternConstructor : PdfPercent10HatchPatternConstructor {
		public PdfPercent80HatchPatternConstructor(Color foreColor, Color backColor)
			: base(backColor, foreColor) {
			MultipleTransform(new PdfTransformationMatrix(-1, 0, 0, -1, 0, RectangleSize));
		}
	}
	public class PdfPercent90HatchPatternConstructor : PdfPercent05HatchPatternConstructor {
		public PdfPercent90HatchPatternConstructor(Color foreColor, Color backColor)
			: base(backColor, foreColor) {
			RotateTransform(180);
			MultipleTransform(new PdfTransformationMatrix(-1, 0, 0, -1, 0, 0));
		}
	}
}
