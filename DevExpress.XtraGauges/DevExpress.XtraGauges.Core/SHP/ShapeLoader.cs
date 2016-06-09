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
using System.Drawing.Drawing2D;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using DevExpress.XtraGauges.Core.Drawing;
using System.Drawing;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Drawing.Drawing2D;
namespace DevExpress.XtraGauges.Core.SHP {
	public enum ShpShapeTypes {
		NullShape = 0,
		Point = 1,
		PolyLine = 3,
		Polygon = 5,
		MultiPoint = 8,
		PointZ = 11,
		PolyLineZ = 13,
		PolygonZ = 15,
		MultiPointZ = 18,
		PointM = 21,
		PolyLineM = 23,
		PolygonM = 25,
		MultiPointM = 28,
		MultiPatch = 31
	}
	public struct ShpHeader {
		public int fileCode;
		public int empty1;
		public int empty2;
		public int empty3;
		public int empty4;
		public int empty5;
		public int fileLength;
		public int version;
		public int shapeType;
		public double xmin;
		public double ymin;
		public double xmax;
		public double ymax;
		public double zmin;
		public double zmax;
		public double mmin;
		public double mmax;
	}
	public class NullShpRecord {
		public int recordNumber;
		public int contentLength;
		public int recordType;
	}
	public class PointShpRecord : NullShpRecord {
		public PointD point;
	}
	public struct PointD {
		public double x;
		public double y;
	}
	public class MultiPointShpRecord : NullShpRecord {
		public double xmin;
		public double ymin;
		public double xmax;
		public double ymax;
		public int numPoints;
		public PointD[] points;
	}
	public class PoliLineShpRecord : NullShpRecord {
		public double xmin;
		public double ymin;
		public double xmax;
		public double ymax;
		public int numPoints;
		public int numParts;
		public int[] parts;
		public PointD[] points;
	}
	public class ByteOrderManager {
		public static int GetIntBigEndian(byte[] block, int index) {
			return (int)(((
				(block[index] << 0x18) |
				(block[index + 0x1] << 0x10)) |
				(block[index + 0x2] << 0x8)) |
				block[index + 0x3]);
		}
		public static int GetIntLittleEndian(byte[] block, int index) {
			return (int)((
				(block[index] |
				(block[index + 0x1] << 0x8)) |
				(block[index + 0x2] << 0x10)) |
				(block[index + 0x3] << 0x18));
		}
	}
	public class ShpLoader : BaseShapeLoader {
		internal ShpHeader header;
		byte[] b4 = new byte[4];
		BinaryReader reader;
		public override ComplexShape LoadFromStream(Stream stream) {
			int offset = 0;
			reader = new BinaryReader(stream);
			LoadHeader(stream, ref offset);
			ComplexShape cs = new ComplexShape();
			cs.Bounds = new RectangleF((float)header.xmin, (float)header.ymin, (float)(header.xmax - header.xmin), (float)(header.ymin - header.ymax));
			cs.Name = "Root";
			LoadRecords(stream, ref offset, cs);
			return cs;
		}
		protected override void OnCreate() { }
		protected override void OnDispose() { }
		protected virtual int ReadInt(Stream stream, bool useBidEndian) {
			if (stream.Read(b4, 0, 4) == 4) {
				if (useBidEndian) return ByteOrderManager.GetIntBigEndian(b4, 0);
				else return ByteOrderManager.GetIntLittleEndian(b4, 0);
			} else throw new Exception("Error reading from stream");
		}
		protected virtual double ReadDouble(Stream stream, bool useBidEndian) {
			return reader.ReadDouble();
		}
		protected virtual void LoadHeader(Stream stream, ref int offset) {
			header.fileCode = ReadInt(stream, true);
			header.empty1 = ReadInt(stream, true);
			header.empty2 = ReadInt(stream, true);
			header.empty3 = ReadInt(stream, true);
			header.empty4 = ReadInt(stream, true);
			header.empty5 = ReadInt(stream, true);
			header.fileLength = ReadInt(stream, true);
			header.version = ReadInt(stream, false);
			header.shapeType = ReadInt(stream, false);
			header.xmin = ReadDouble(stream, false);
			header.ymin = ReadDouble(stream, false);
			header.xmax = ReadDouble(stream, false);
			header.ymax = ReadDouble(stream, false);
			header.zmin = ReadDouble(stream, false);
			header.zmax = ReadDouble(stream, false);
			header.mmin = ReadDouble(stream, false);
			header.mmax = ReadDouble(stream, false);
		}
		protected virtual void LoadRecords(Stream stream, ref int offset, ComplexShape cs) {
			cs.BeginUpdate();
			while (stream.Position < stream.Length)
				LoadRecord(stream, ref offset, cs);
			cs.EndUpdate();
		}
		protected virtual NullShpRecord LoadRecordBase(Stream stream) {
			NullShpRecord rec = new NullShpRecord();
			rec.recordNumber = ReadInt(stream, true);
			rec.contentLength = ReadInt(stream, true);
			rec.recordType = ReadInt(stream, false);
			return rec;
		}
		protected void SkipRecord(Stream stream, NullShpRecord recordInfo) {
			stream.Seek(recordInfo.contentLength * 2 - 4, SeekOrigin.Current);
		}
		protected virtual void LoadBoundingBox(Stream stream, out RectangleF bounds) {
			double xmin = ReadDouble(stream, false);
			double ymin = ReadDouble(stream, false);
			double xmax = ReadDouble(stream, false);
			double ymax = ReadDouble(stream, false);
			bounds = new RectangleF((float)xmin, (float)ymin, (float)(xmax - xmin), (float)(ymax - ymin));
		}
		protected virtual void LoadPolygon(Stream stream, ComplexShape cs) {
			RectangleF bounds;
			LoadBoundingBox(stream, out bounds);
			int numParts = ReadInt(stream, false);
			int numPoints = ReadInt(stream, false);
			int[] parts = new int[numParts];
			PointD[] points = new PointD[numPoints];
			for (int i = 0; i < numParts; i++)
				parts[i] = ReadInt(stream, false);
			for (int i = 0; i < numPoints; i++) {
				points[i].x = ReadDouble(stream, false);
				points[i].y = ReadDouble(stream, false);
			}
			for (int i = 0; i < numParts; i++) {
				int end = 0;
				if ((i + 1) == numParts) end = points.Length;
				else end = parts[i + 1];
				ShapePoint[] sPoints = new ShapePoint[end - parts[i]];
				for (int j = parts[i]; j < end; j++)
					sPoints[j - parts[i]] = new ShapePoint(new PointF((float)points[j].x, (float)points[j].y), PathPointType.Line);
				PathShape ps = new PathShape(sPoints);
				ps.Bounds = bounds;
				ps.Appearance.BorderWidth = 1;
				ps.Appearance.BorderBrush = new SolidBrushObject(Color.Black);
				ps.Appearance.ContentBrush = new SolidBrushObject(Color.Green);
				cs.Add(ps);
			}
		}
		protected virtual NullShpRecord LoadRecord(Stream stream, ref int offset, ComplexShape cs) {
			NullShpRecord result = LoadRecordBase(stream);
			switch (((ShpShapeTypes)result.recordType)) {
				case ShpShapeTypes.Polygon:
					LoadPolygon(stream, cs);
					break;
				case ShpShapeTypes.MultiPatch: 
				case ShpShapeTypes.MultiPoint: 
				case ShpShapeTypes.MultiPointM:
				case ShpShapeTypes.MultiPointZ:
				case ShpShapeTypes.NullShape: 
				case ShpShapeTypes.Point: 
				case ShpShapeTypes.PointM:
				case ShpShapeTypes.PointZ: 
				case ShpShapeTypes.PolygonM: 
				case ShpShapeTypes.PolygonZ: 
				case ShpShapeTypes.PolyLine: 
				case ShpShapeTypes.PolyLineM:
				case ShpShapeTypes.PolyLineZ:
				default: SkipRecord(stream, result); break;
			}
			return result;
		}
	}
}
