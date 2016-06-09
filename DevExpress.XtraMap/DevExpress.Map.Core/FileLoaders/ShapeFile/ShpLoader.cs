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
using System.IO;
namespace DevExpress.Map.Native {
	public class ShpLoader : IDisposable {
		readonly byte[] buf4Bytes = new byte[4];
		readonly Stream stream;
		readonly ShpHeader header;
		readonly BinaryReader reader;
		readonly List<ShpRecord> records = new List<ShpRecord>();
		public ShpHeader Header { get { return header; } }
		public List<ShpRecord> Records { get { return records; } }
		public ShpLoader(Stream stream) {
			this.stream = stream;
			this.reader = new BinaryReader(stream);
			this.header = LoadHeader();
			LoadRecords();
		}
		public int GetIntBigEndian(byte[] block, int index) {
			return (int)(((
				(block[index] << 0x18) |
				(block[index + 0x1] << 0x10)) |
				(block[index + 0x2] << 0x8)) |
				block[index + 0x3]);
		}
		public int GetIntLittleEndian(byte[] block, int index) {
			return (int)((
				(block[index] |
				(block[index + 0x1] << 0x8)) |
				(block[index + 0x2] << 0x10)) |
				(block[index + 0x3] << 0x18));
		}
		int ReadInt(Stream stream, bool useBidEndian) {
			if (stream.Read(buf4Bytes, 0, 4) == 4) {
				if (useBidEndian)
					return GetIntBigEndian(buf4Bytes, 0);
				else
					return GetIntLittleEndian(buf4Bytes, 0);
			}
			else
				throw new Exception("Incorrect Shp File Format");
		}
		double ReadDouble() {
			return reader.ReadDouble();
		}
		ShpHeader LoadHeader() {
			try {
				ShpHeader result = new ShpHeader();
				result.FileCode = ReadInt(stream, true);
				result.Empty1 = ReadInt(stream, true);
				result.Empty2 = ReadInt(stream, true);
				result.Empty3 = ReadInt(stream, true);
				result.Empty4 = ReadInt(stream, true);
				result.Empty5 = ReadInt(stream, true);
				result.FileLength = ReadInt(stream, true);
				result.Version = ReadInt(stream, false);
				result.ShapeType = ReadInt(stream, false);
				result.Xmin = ReadDouble();
				result.Ymin = ReadDouble();
				result.Xmax = ReadDouble();
				result.Ymax = ReadDouble();
				result.Zmin = ReadDouble();
				result.Zmax = ReadDouble();
				result.Mmin = ReadDouble();
				result.Mmax = ReadDouble();
				return result;
			}
			catch {
				throw new Exception("Incorrect Shp File Format");
			}
		}
		void LoadRecords() {
			try {
				while (stream.Position < stream.Length)
					records.Add(LoadRecord(stream));
			}
			catch {
				throw new Exception("Incorrect Shp File Format");
			}
		}
		void SkipRecord(Stream stream, long contentLength) {
			stream.Seek(contentLength * 2 - 4, SeekOrigin.Current);
		}
		PointShpRecord LoadPoint(int recordNumber) {
			PointShpRecord result = new PointShpRecord(recordNumber);
			ShpPoint point = new ShpPoint();
			point.X = ReadDouble();
			point.Y = ReadDouble();
			result.Point = point;
			return result;
		}
		ShpRecord LoadPointM(int recordNumber) {
			PointMShpRecord result = new PointMShpRecord(recordNumber);
			result.PointM = new ShpPointM() {
				X = ReadDouble(),
				Y = ReadDouble(),
				M = ReadDouble()
			};
			return result;
		}
		ShpRecord LoadPointZ(int recordNumber) {
			PointZShpRecord result = new PointZShpRecord(recordNumber);
			result.PointZ = new ShpPointZ() {
				X = ReadDouble(),
				Y = ReadDouble(),
				Z = ReadDouble(),
				M = ReadDouble()
			};
			return result;
		}
		MultiPointShpRecord LoadMultiPoint(Stream stream, int recordNumber) {
			MultiPointShpRecord result = new MultiPointShpRecord(recordNumber);
			result.XMin = ReadDouble();
			result.YMin = ReadDouble();
			result.XMax = ReadDouble();
			result.YMax = ReadDouble();
			result.PointCount = ReadInt(stream, false);
			result.Points = new ShpPoint[result.PointCount];
			for (int i = 0; i < result.PointCount; i++) {
				result.Points[i].X = ReadDouble();
				result.Points[i].Y = ReadDouble();
			}
			return result;
		}
		PolyLineShpRecord LoadPolyLine(Stream stream, int recordNumber) {
			PolyLineShpRecord result = new PolyLineShpRecord(recordNumber);
			result.XMin = ReadDouble();
			result.YMin = ReadDouble();
			result.XMax = ReadDouble();
			result.YMax = ReadDouble( );
			result.PartCount = ReadInt(stream, false);
			result.PointCount = ReadInt(stream, false);
			result.Parts = new int[result.PartCount];
			result.Points = new ShpPoint[result.PointCount];
			for (int i = 0; i < result.PartCount; i++)
				result.Parts[i] = ReadInt(stream, false);
			for (int i = 0; i < result.PointCount; i++) {
				result.Points[i].X = ReadDouble();
				result.Points[i].Y = ReadDouble();
			}
			return result;
		}
		PolygonShpRecord LoadPolygon(Stream stream, int recordNumber) {
			PolygonShpRecord result = new PolygonShpRecord(recordNumber);
			result.XMin = ReadDouble();
			result.YMin = ReadDouble();
			result.XMax = ReadDouble();
			result.YMax = ReadDouble();
			result.PartCount = ReadInt(stream, false);
			result.PointCount = ReadInt(stream, false);
			result.Parts = new int[result.PartCount];
			result.Points = new ShpPoint[result.PointCount];
			for (int i = 0; i < result.PartCount; i++)
				result.Parts[i] = ReadInt(stream, false);
			for (int i = 0; i < result.PointCount; i++) {
				result.Points[i].X = ReadDouble();
				result.Points[i].Y = ReadDouble();
			}
			return result;
		}
		ShpRecord LoadRecord(Stream stream) {
			int recordNumber = ReadInt(stream, true);
			long contentLength = ReadInt(stream, true);
			int recordType = ReadInt(stream, false);
			switch (((ShpRecordTypes)recordType)) {
				case ShpRecordTypes.NullShape:
					break;
				case ShpRecordTypes.Point: return LoadPoint(recordNumber);
				case ShpRecordTypes.MultiPoint: return LoadMultiPoint(stream, recordNumber);
				case ShpRecordTypes.PolyLine: return LoadPolyLine(stream, recordNumber);
				case ShpRecordTypes.Polygon: return LoadPolygon(stream, recordNumber);
				case ShpRecordTypes.MultiPointM: return LoadMultiPointM(stream, recordNumber, contentLength);
				case ShpRecordTypes.PointM: return LoadPointM(recordNumber);
				case ShpRecordTypes.PolyLineM: return LoadPolyLineM(stream, recordNumber, contentLength);
				case ShpRecordTypes.PolygonM: return LoadPolygonM(stream, recordNumber, contentLength);
				case ShpRecordTypes.PointZ: return LoadPointZ(recordNumber);
				case ShpRecordTypes.MultiPatch:
					System.Diagnostics.Debug.WriteLine("Warning: loading of MultiPatch is not implemented.");
					break;
				case ShpRecordTypes.MultiPointZ: return LoadMultiPointZ(stream, recordNumber, contentLength);
				case ShpRecordTypes.PolygonZ: return LoadPolygonZ(stream, recordNumber, contentLength);
				case ShpRecordTypes.PolyLineZ: return LoadPolyLineZ(stream, recordNumber, contentLength);
			}
			SkipRecord(stream, contentLength);
			return new NullShpRecord(recordNumber);
		}
		ShpRecord LoadPolyLineM(Stream stream, int recordNumber, long contentLength) {
			PolyLineMShpRecord result = new PolyLineMShpRecord(recordNumber);
			result.XMin = ReadDouble();
			result.YMin = ReadDouble();
			result.XMax = ReadDouble();
			result.YMax = ReadDouble();
			result.PartCount = ReadInt(stream, false);
			result.PointCount = ReadInt(stream, false);
			result.Parts = new int[result.PartCount];
			result.Points = new ShpPointM[result.PointCount];
			for (int i = 0; i < result.PartCount; i++)
				result.Parts[i] = ReadInt(stream, false);
			for (int i = 0; i < result.PointCount; i++) {
				result.Points[i].X = ReadDouble();
				result.Points[i].Y = ReadDouble();
			}
			int readLenght = 44 + result.PartCount * 4 + result.PointCount * 16;
			bool canReadMeasures = 2 * contentLength > readLenght;
			if (canReadMeasures) {
				result.MMin = ReadDouble();
				result.MMax = ReadDouble();
				for (int i = 0; i < result.PointCount; i++)
					result.Points[i].M = ReadDouble();
			}
			return result;
		}
		ShpRecord LoadPolyLineZ(Stream stream, int recordNumber, long contentLength) {
			var result = new PolyLineZShpRecord(recordNumber);
			result.XMin = ReadDouble();
			result.YMin = ReadDouble();
			result.XMax = ReadDouble();
			result.YMax = ReadDouble();
			result.PartCount = ReadInt(stream, false);
			result.PointCount = ReadInt(stream, false);
			result.Parts = new int[result.PartCount];
			result.Points = new ShpPointZ[result.PointCount];
			for (int i = 0; i < result.PartCount; i++)
				result.Parts[i] = ReadInt(stream, false);
			for (int i = 0; i < result.PointCount; i++) {
				result.Points[i].X = ReadDouble();
				result.Points[i].Y = ReadDouble();
			}
			result.ZMin = ReadDouble();
			result.ZMax = ReadDouble();
			for (int i = 0; i < result.PointCount; i++)
				result.Points[i].Z = ReadDouble();
			int readLenght = 44 + result.PartCount * 4 + result.PointCount * 16 + 16 + result.PointCount * 8;
			bool canReadMeasures = 2 * contentLength > readLenght;
			if (canReadMeasures) {
				result.MMin = ReadDouble();
				result.MMax = ReadDouble();
				for (int i = 0; i < result.PointCount; i++)
					result.Points[i].M = ReadDouble();
			}
			return result;
		}
		ShpRecord LoadPolygonM(Stream stream, int recordNumber, long contentLength) {
			PolygonMShpRecord result = new PolygonMShpRecord(recordNumber);
			result.XMin = ReadDouble();
			result.YMin = ReadDouble();
			result.XMax = ReadDouble();
			result.YMax = ReadDouble();
			result.PartCount = ReadInt(stream, false);
			result.PointCount = ReadInt(stream, false);
			result.Parts = new int[result.PartCount];
			result.Points = new ShpPointM[result.PointCount];
			for (int i = 0; i < result.PartCount; i++)
				result.Parts[i] = ReadInt(stream, false);
			for (int i = 0; i < result.PointCount; i++) {
				result.Points[i].X = ReadDouble();
				result.Points[i].Y = ReadDouble();
			}
			int readLenght = 44 + result.PartCount * 4 + result.PointCount * 16;
			bool canReadMeasures = 2 * contentLength > readLenght;
			if (canReadMeasures) {
				result.MMin = ReadDouble();
				result.MMax = ReadDouble();
				for (int i = 0; i < result.PointCount; i++)
					result.Points[i].M = ReadDouble();
			}
			return result;
		}
		ShpRecord LoadPolygonZ(Stream stream, int recordNumber, long contentLength) {
			var result = new PolygonZShpRecord(recordNumber);
			result.XMin = ReadDouble();
			result.YMin = ReadDouble();
			result.XMax = ReadDouble();
			result.YMax = ReadDouble();
			result.PartCount = ReadInt(stream, false);
			result.PointCount = ReadInt(stream, false);
			result.Parts = new int[result.PartCount];
			result.Points = new ShpPointZ[result.PointCount];
			for (int i = 0; i < result.PartCount; i++)
				result.Parts[i] = ReadInt(stream, false);
			for (int i = 0; i < result.PointCount; i++) {
				result.Points[i].X = ReadDouble();
				result.Points[i].Y = ReadDouble();
			}
			result.ZMin = ReadDouble();
			result.ZMax = ReadDouble();
			for (int i = 0; i < result.PointCount; i++)
				result.Points[i].Z = ReadDouble();
			int readLenght = 44 + result.PartCount * 4 + result.PointCount * 16 + 16 + result.PointCount * 8;
			bool canReadMeasures = 2 * contentLength > readLenght;
			if (canReadMeasures) {
				result.MMin = ReadDouble();
				result.MMax = ReadDouble();
				for (int i = 0; i < result.PointCount; i++)
					result.Points[i].M = ReadDouble();
			}
			return result;
		}
		ShpRecord LoadMultiPointM(Stream stream, int recordNumber, long contentLength) {
			MultiPointMShpRecord result = new MultiPointMShpRecord(recordNumber);
			result.XMin = ReadDouble();
			result.YMin = ReadDouble();
			result.XMax = ReadDouble();
			result.YMax = ReadDouble();
			result.PointCount = ReadInt(stream, false);
			result.PointMs = new ShpPointM[result.PointCount];
			for (int i = 0; i < result.PointCount; i++) {
				result.PointMs[i].X = ReadDouble();
				result.PointMs[i].Y = ReadDouble();
			}
			int readLenght = 44 + result.PointCount * 16;
			bool canReadMeasures = 2 * contentLength > readLenght;
			if (canReadMeasures) {
				result.MMin = ReadDouble();
				result.MMax = ReadDouble();
				for (int i = 0; i < result.PointCount; i++)
					result.PointMs[i].M = ReadDouble();
			}
			return result;
		}
		ShpRecord LoadMultiPointZ(Stream stream, int recordNumber, long contentLength) {
			var result = new MultiPointZShpRecord(recordNumber);
			result.XMin = ReadDouble();
			result.YMin = ReadDouble();
			result.XMax = ReadDouble();
			result.YMax = ReadDouble();
			result.PointCount = ReadInt(stream, false);
			result.PointZs = new ShpPointZ[result.PointCount];
			for (int i = 0; i < result.PointCount; i++) {
				result.PointZs[i].X = ReadDouble();
				result.PointZs[i].Y = ReadDouble();
			}
			result.ZMin = ReadDouble();
			result.ZMax = ReadDouble();
			for (int i = 0; i < result.PointCount; i++)
				result.PointZs[i].Z = ReadDouble();
			int readLenght = 44 + result.PointCount * 16 + 16 + result.PointCount * 8;
			bool canReadMeasures = 2 * contentLength > readLenght;
			if (canReadMeasures) {
				result.MMin = ReadDouble();
				result.MMax = ReadDouble();
				for (int i = 0; i < result.PointCount; i++)
					result.PointZs[i].M = ReadDouble();
			}
			return result;
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (reader != null)
					reader.Dispose();
			}
		}
		~ShpLoader() {
			Dispose(false);
		}
	}
}
