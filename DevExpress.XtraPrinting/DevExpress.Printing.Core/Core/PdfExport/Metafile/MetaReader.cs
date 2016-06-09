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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
namespace DevExpress.Printing.Core.PdfExport.Metafile {
	public class MetaReader : BinaryReader {
		public MetaReader(Stream stream)
			: base(stream) {
		}
		public Color ReadColorRGB() {
			Color color = Color.FromArgb(255, ReadByte(), ReadByte(), ReadByte());
			ReadByte();
			return color;
		}
		public Color ReadColorBGR(bool alpha = false) {
			int b = ReadByte();
			int g = ReadByte();
			int r = ReadByte();
			int a = alpha ? ReadByte() : 255;
			return Color.FromArgb(a, r, g, b);
		}
		public Color ReadEmfPlusARGB() {
			return ReadColorBGR(true);
		}
		public string ReadANSIString(int maxLength) {
			StringBuilder sb = new StringBuilder();
			for(int i = 0; i < maxLength; i++) {
				byte b = ReadByte();
				if(b == 0)
					break;
				sb.Append((char)b);
			}
			return sb.ToString();
		}
		public Point ReadPointYX() {
			int y = ReadInt16();
			int x = ReadInt16();
			return new Point(x, y);
		}
		public Point ReadPointXY() {
			int x = ReadInt16();
			int y = ReadInt16();
			return new Point(x, y);
		}
		public PointF ReadPointF() {
			return new PointF(ReadSingle(), ReadSingle());
		}
		public PointF[] ReadPoints(long numberOfPoints, bool compressed) {
			PointF[] points = new PointF[numberOfPoints];
			for(int i = 0; i < numberOfPoints; i++) {
				if(compressed) {
					points[i] = ReadPointXY();
				}
				else {
					points[i] = new PointF(ReadSingle(), ReadSingle());
				}
			}
			return points;
		}
		public byte[] ReadToEnd() {
			long deviceIndependentBitmapLength = BaseStream.Length - BaseStream.Position;
			return ReadBytes((int)deviceIndependentBitmapLength);
		}
		public RectangleF ReadRect() {
			return new RectangleF(ReadInt16(), ReadInt16(), ReadInt16(), ReadInt16());
		}
		public RectangleF ReadRectF() {
			return new RectangleF(ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle());
		}
		public RectangleF[] ReadRectangles(bool compressed, long count) {
			RectangleF[] rectangles = new RectangleF[count];
			for(int i = 0; i < count; i++) {
				rectangles[i] = compressed ? ReadRect() : ReadRectF();
			}
			return rectangles;
		}
		public Matrix ReadMatrix() {
			return new Matrix(ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle());
		}
		public float[] ReadSingleArray(int count) {
			float[] result = new float[count];
			for(int i = 0; i < count; i++)
				result[i] = ReadSingle();
			return result;
		}
	}
}
