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

using System.IO;
using System.Collections.Generic;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf
{
	internal class PdfTriangleWriter : BinaryWriter
	{
		readonly double xMultipler;
		readonly double yMultipler;
		readonly double[] cMultipler;
		readonly PdfRange xRange;
		readonly PdfRange yRange;
		readonly IList<PdfRange> cRanges;
		readonly int bitsPerFlag;
		readonly int bitsPerComponent;
		readonly int bitsPerCoordinate;
		byte current = 0;
		int currentPosition = 0;
		public PdfTriangleWriter(Stream stream, PdfRange xRange, PdfRange yRange, IList<PdfRange> cRanges, int bitsPerCoordinate, int bitsPerFlag, int bitsPerComponent)
			: base(stream)
		{
			this.bitsPerFlag = bitsPerFlag;
			this.bitsPerComponent = bitsPerComponent;
			this.bitsPerCoordinate = bitsPerCoordinate;
			uint coordinateMaxValue = uint.MaxValue >> (32 - bitsPerCoordinate);
			xMultipler = coordinateMaxValue / (xRange.Max - xRange.Min);
			yMultipler = coordinateMaxValue / (yRange.Max - yRange.Min);
			cMultipler = new double[cRanges.Count];
			int componentMaxValue = (1 << bitsPerComponent) - 1;
			for (int i = 0; i < cRanges.Count; i++)
				cMultipler[i] = componentMaxValue / (cRanges[i].Max - cRanges[i].Min);
			this.xRange = xRange;
			this.yRange = yRange;
			this.cRanges = cRanges;
		}
		public virtual void Write(PdfTriangle triangle)
		{
			Write(triangle.Vertex1);
			Write(triangle.Vertex2);
			Write(triangle.Vertex3);
		}
		public virtual void Write(PdfVertex value)
		{
			WriteBits(0, bitsPerFlag);
			CheckValue(xRange, value.Point.X);
			WriteBits((uint)((value.Point.X - xRange.Min) * xMultipler), bitsPerCoordinate);
			CheckValue(yRange, value.Point.Y);
			WriteBits((uint)((value.Point.Y - yRange.Min) * yMultipler), bitsPerCoordinate);
			double[] componenets = value.Color.Components;
			for (int i = 0; i < componenets.Length; i++)
			{
				CheckValue(cRanges[i], componenets[i]);
				WriteBits((uint)((componenets[i] - cRanges[i].Min) * cMultipler[i]), bitsPerComponent);
			}
			if (currentPosition != 0)
				WriteByte();
		}
		public override void Flush()
		{
			if (currentPosition != 0)
				WriteByte();
			base.Flush();
		}
		protected override void Dispose(bool disposing)
		{
			if (disposing && currentPosition != 0)
				WriteByte();
			base.Dispose(disposing);
		}
		void CheckValue(PdfRange range, double value)
		{
			if (value > range.Max || value < range.Min)
				PdfDocumentReader.ThrowIncorrectDataException();
		}
		void WriteBits(uint value, int bitsCount)
		{
			int remainBits = bitsCount;
			int freeBitsCount = 8 - currentPosition;
			while (remainBits > freeBitsCount)
			{
				int currentOffset = remainBits - freeBitsCount;
				current |= (byte)((value >> currentOffset) & (uint.MaxValue >> currentOffset));
				currentPosition += freeBitsCount;
				WriteByte();
				remainBits -= freeBitsCount;
				freeBitsCount = 8 - currentPosition;
			}
			int remainBitsMask = (1 << remainBits) - 1;
			current |= (byte)((value & remainBitsMask) << (freeBitsCount - remainBits));
			currentPosition += remainBits;
		}
		void WriteByte()
		{
			Write(current);
			current = 0;
			currentPosition = 0;
		}
	}
}
