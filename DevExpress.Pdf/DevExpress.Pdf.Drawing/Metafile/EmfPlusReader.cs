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
using System.Drawing.Drawing2D;
namespace DevExpress.Pdf.Drawing {
	public class EmfPlusReader : BinaryReader {
		public EmfPlusReader(Stream stream)
			: base(stream) {
		}
		public int ReadEmfPlusInt() {
			byte first = ReadByte();
			byte firstByteValue = (byte)(first >> 1);
			if ((first & 1) == 0)
				return (first & 0x80) == 0 ? firstByteValue : firstByteValue | (-1 ^ 0x7F);
			else {
				int value = firstByteValue | ((int)ReadByte()) << 7;
				return (value & 0x4000) == 0 ? value : (value | (-1 ^ 0x3FFF));
			}
		}
		public PointF ReadPointF(bool compressed) {
			if (compressed)
				return new PointF(ReadInt16(), ReadInt16());
			return new PointF(ReadSingle(), ReadSingle());
		}
		public RectangleF ReadRectF(bool compressed) {
			if (compressed)
				return new RectangleF(ReadInt16(), ReadInt16(), ReadInt16(), ReadInt16());
			return new RectangleF(ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle());
		}
		public Color ReadColor() {
			return Color.FromArgb(ReadInt32());
		}
		public PdfTransformationMatrix ReadTransformMatrix() {
			return new PdfTransformationMatrix(ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle());
		}
		public PdfBlend ReadBlend() {
			int count = ReadInt32();
			PdfBlend blend = new PdfBlend(count);
			for (int i = 0; i < count; i++)
				blend.Positions[i] = ReadSingle();
			for (int i = 0; i < count; i++)
				blend.Factors[i] = ReadSingle();
			return blend;
		}
		public PdfColorBlend ReadColorBlend() {
			int count = ReadInt32();
			PdfColorBlend colorBlend = new PdfColorBlend(count);
			for (int i = 0; i < count; i++)
				colorBlend.Positions[i] = ReadSingle();
			for (int i = 0; i < count; i++) {
				Color color = ReadColor();
				colorBlend.Colors[i] = new PdfColor(color.R / 255, color.G / 255, color.B / 255);
			}
			return colorBlend;
		}
	}
}
