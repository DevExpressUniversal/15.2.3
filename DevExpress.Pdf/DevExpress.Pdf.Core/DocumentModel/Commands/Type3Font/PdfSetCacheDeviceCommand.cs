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

using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	public class PdfSetCacheDeviceCommand : PdfCommand {
		internal const string Name = "d1";
		readonly double charWidth;
		readonly PdfRectangle boundingBox;
		public double CharWidth { get { return charWidth; } }
		public PdfRectangle BoundingBox { get { return boundingBox; } }
		public PdfSetCacheDeviceCommand(double charWidth, PdfRectangle boundingBox) {
			this.charWidth = charWidth;
			this.boundingBox = boundingBox;
		}
		internal PdfSetCacheDeviceCommand(PdfOperands operands) {
			charWidth = operands.PopDouble();
			if (operands.PopDouble() != 0)
				PdfDocumentReader.ThrowIncorrectDataException();
			double left = operands.PopDouble();
			double bottom = operands.PopDouble();
			double right = operands.PopDouble();
			double top = operands.PopDouble();
			boundingBox = new PdfRectangle(left, bottom, right, top);
		}
		protected internal override void Write(PdfResources resources, PdfDocumentWritableStream writer) {
			writer.WriteSpace();
			writer.WriteDouble(charWidth);
			writer.WriteSpace();
			writer.WriteInt(0);
			writer.WriteSpace();
			writer.WriteDouble(boundingBox.Left);
			writer.WriteSpace();
			writer.WriteDouble(boundingBox.Bottom);
			writer.WriteSpace();
			writer.WriteDouble(boundingBox.Right);
			writer.WriteSpace();
			writer.WriteDouble(boundingBox.Top);
			writer.WriteSpace();
			writer.WriteString(Name);
		}
	}
}
