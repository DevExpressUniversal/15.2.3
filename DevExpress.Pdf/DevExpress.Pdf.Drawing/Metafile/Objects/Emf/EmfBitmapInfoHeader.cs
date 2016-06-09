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

using System;
using System.IO;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf.Drawing {
	public class EmfBitmapInfoHeader : EmfBitmapHeader {
		readonly EmfCompression compression;
		readonly int imageSize;
		readonly int xPelsPerMeter;
		readonly int yPelsPerMeter;
		readonly int colorUsed;
		readonly int colorImportant;
		readonly EmfRgbQuad[] quadColors;
		public EmfCompression Compression { get { return compression; } }
		public int ImageSize { get { return imageSize; } }
		public int XPelsPerMeter { get { return xPelsPerMeter; } }
		public int YPelsPerMeter { get { return yPelsPerMeter; } }
		public int ColorUsed { get { return colorUsed; } }
		public int ColorImportant { get { return colorImportant; } }
		public EmfRgbQuad[] QuadColors { get { return quadColors; } }
		public EmfBitmapInfoHeader(BinaryReader reader)
			: base(reader.ReadInt32(), reader.ReadInt32(), reader) {
			compression = EmfEnumToValueConverter.ParseEmfEnum<EmfCompression>(reader.ReadInt32());
			imageSize = reader.ReadInt32();
			xPelsPerMeter = reader.ReadInt32();
			yPelsPerMeter = reader.ReadInt32();
			colorUsed = reader.ReadInt32();
			colorImportant = reader.ReadInt32();
			if (BitCount > 0) {
				int colorsCount = ((int)BitCount < 16) ? Math.Min((int)Math.Pow(2, (int)BitCount), colorUsed) : colorUsed;
				quadColors = new EmfRgbQuad[colorsCount];
				for (int i = 0; i < colorsCount; i++)
					quadColors[i] = new EmfRgbQuad(reader);
			}
		}
	}
}
