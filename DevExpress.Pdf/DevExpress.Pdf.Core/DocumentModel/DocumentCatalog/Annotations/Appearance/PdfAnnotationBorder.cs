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
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	public class PdfAnnotationBorder {
		const double defaultHorizontalCornerRadius = 0;
		const double defaultVerticalCornerRadius = 0;
		const double defaultLineWidth = 1;
		readonly double horizontalCornerRaduis;
		readonly double verticalCornerRadius;
		readonly double lineWidth;
		readonly PdfLineStyle lineStyle;
		public double HorizontalCornerRadius { get { return horizontalCornerRaduis; } }
		public double VerticalCornerRadius { get { return verticalCornerRadius; } }
		public double LineWidth { get { return lineWidth; } }
		public PdfLineStyle LineStyle { get { return lineStyle; } }
		public bool IsDefault { 
			get { return horizontalCornerRaduis == defaultHorizontalCornerRadius && lineWidth == defaultLineWidth && verticalCornerRadius == defaultVerticalCornerRadius && !lineStyle.IsDashed; } 
		}
		public PdfAnnotationBorder() {
			horizontalCornerRaduis = defaultHorizontalCornerRadius;
			verticalCornerRadius = defaultVerticalCornerRadius;
			lineWidth = defaultLineWidth;
			lineStyle = PdfLineStyle.CreateSolid();
		}
		internal PdfAnnotationBorder(IList<object> values) {
			switch (values.Count) {
				case 4:
					object value = values[3];
					IList<object> lineStyleArray = value as IList<object>;
					if (lineStyleArray == null) {
						if (!(value is int) && !(value is double))
							PdfDocumentReader.ThrowIncorrectDataException();
						lineStyleArray = new object[] { value };
					}
					lineStyle = PdfAnnotationBorderStyle.ParseLineStyle(lineStyleArray);
					goto case 3;
				case 3:
					horizontalCornerRaduis = PdfDocumentReader.ConvertToDouble(values[0]);
					verticalCornerRadius = PdfDocumentReader.ConvertToDouble(values[1]);
					lineWidth = PdfDocumentReader.ConvertToDouble(values[2]);
					if (horizontalCornerRaduis < 0 || verticalCornerRadius < 0 || lineWidth < 0)
						PdfDocumentReader.ThrowIncorrectDataException();
					if (lineStyle == null)
						lineStyle = PdfLineStyle.CreateSolid();
					break;
				default:
					PdfDocumentReader.ThrowIncorrectDataException();
					break;
			}
		}
		protected internal object ToWritableObject() {
			if (lineStyle.IsDashed)
				return new object[] { horizontalCornerRaduis, verticalCornerRadius, lineWidth, lineStyle.Data[0] };
			return new PdfWritableDoubleArray(horizontalCornerRaduis, verticalCornerRadius, lineWidth);
		}
	}
}
