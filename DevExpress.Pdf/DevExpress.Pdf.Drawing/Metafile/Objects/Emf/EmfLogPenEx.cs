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

using System.IO;
using System.Drawing;
namespace DevExpress.Pdf.Drawing {
	public struct EmfLogPenEx : IEmfStateObject {
		readonly EmfPenStyle penStyle;
		readonly int width;
		readonly EmfBrushStyle brushStyle;
		readonly EmfColorRef colorRef;
		readonly EmfHatchStyle brushHatch;
		readonly int[] dashesLength;
		readonly int[] gapsLength;
		PdfPen pen;
		public EmfPenStyle PenStyle { get { return penStyle; } }
		public int Width { get { return width; } }
		public EmfBrushStyle BrushStyle { get { return brushStyle; } }
		public EmfColorRef ColorRef { get { return colorRef; } }
		public EmfHatchStyle BrushHatch { get { return brushHatch; } }
		public int[] DashesLength { get { return dashesLength; } }
		public int[] GapsLength { get { return gapsLength; } }
		public EmfLogPenEx(BinaryReader reader) {
			this.pen = null;
			this.penStyle = (EmfPenStyle)(reader.ReadInt32());
			this.width =  reader.ReadInt32();
			this.brushStyle = EmfEnumToValueConverter.ParseEmfEnum<EmfBrushStyle>(reader.ReadInt32());
			this.colorRef = new EmfColorRef(reader);
			int hatchStyleValue = reader.ReadInt32();
			if (brushStyle == EmfBrushStyle.BS_HATCHED)
				this.brushHatch = EmfEnumToValueConverter.ParseEmfEnum<EmfHatchStyle>(hatchStyleValue);
			else
				this.brushHatch = EmfHatchStyle.HS_SOLIDCLR;
			int styleEntriesCount = reader.ReadInt32();
			switch (styleEntriesCount) {
				case 0:
					dashesLength = null;
					gapsLength = null;
					break;
				case 1:
					dashesLength = new int[1];
					dashesLength[0] = reader.ReadInt32();
					gapsLength = null;
					break;
				default:
					int halfStyleEntriesCount = styleEntriesCount / 2;
					dashesLength = new int[halfStyleEntriesCount + styleEntriesCount % 2];
					gapsLength = new int[halfStyleEntriesCount];
					for (int i = 0; i < halfStyleEntriesCount; i++) {
						dashesLength[i] = reader.ReadInt32();
						gapsLength[i] = reader.ReadInt32();
					}
					break;
			}
		}
		void IEmfStateObject.SetObject(EmfMetafileGraphics context) {
			if (pen == null) {
				switch (brushStyle) {
					case EmfBrushStyle.BS_SOLID:
						pen = new PdfPen(Color.FromArgb(colorRef.Red, colorRef.Green, colorRef.Blue), width);
						break;
					default :
						break;
				}
			}
			context.SetPen(pen);
		}
		void IEmfStateObject.DeleteObject() {
				pen = null;
		}
	}
}
