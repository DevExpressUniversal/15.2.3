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
	public abstract class CommonExporterBase {
		BinaryWriter writer;
		protected void WriteInt(int value, bool useBigEndian) {
			CoreUtils.WriteInt(writer, value, useBigEndian);
		}
		protected void WriteDouble(double value) {
			CoreUtils.WriteDouble(writer, value);
		}
		protected void WriteByte(byte value) {
			CoreUtils.WriteByte(writer, value);
		}
		[CLSCompliant(false)]
		protected void WriteUInt16(UInt16 value) {
			CoreUtils.WriteUInt16(writer, value);
		}
		protected void WriteString(string value) {
			CoreUtils.WriteString(writer, value);
		}
		protected void WriteBool(bool value) {
			CoreUtils.WriteBool(writer, value);
		}
		protected abstract void WriteHeader();
		protected abstract void WriteRecords();
		public void Export(Stream stream) {
			using(Stream tempStream = new MemoryStream()) {
				using(this.writer = new BinaryWriter(tempStream)) {
					WriteHeader();
					WriteRecords();
					tempStream.Seek(0, SeekOrigin.Begin);
					tempStream.CopyTo(stream);
				}
			}
		}
	}
	public abstract class ShapeExporterCoreBase : CommonExporterBase {
		protected internal abstract ShpRecordTypes RecordType { get; }
		public abstract void UpdateExportingItems(IEnumerable<IMapItemCore> items);
	}
	public abstract class ShapeExporterCoreBase<T> : ShapeExporterCoreBase where T : ShpRecord {
		const int HeaderSize = 100;
		IList<T> records;
		int CalculateFileLength() {
			int recordsLength = HeaderSize / 2;
			foreach(T record in records) {
				recordsLength += record.GetWordSize() + 4;
			}
			return recordsLength;
		}
		double[] GetFileBoundingBox() {
			CoordBounds boundingBox = CoordBounds.Empty;
			foreach(T record in records)
				boundingBox = CoordBounds.Union(boundingBox, record.GetBoundingBox());
			return new double[] { boundingBox.X1, boundingBox.Y2, boundingBox.X2, boundingBox.Y1, 0, 0, 0, 0 };
		}
		protected override void WriteHeader() {
			WriteInt(9994, true);
			for(int i = 0; i < 5; i++)
				WriteInt(0, true);
			WriteInt(CalculateFileLength(), true);
			WriteInt(1000, false);
			WriteInt((int)RecordType, false);
			double[] boundingBox = GetFileBoundingBox();
			foreach(double val in boundingBox)
				WriteDouble(val);
		}
		protected override void WriteRecords() {
			for(int i = 1; i <= records.Count; i++) {
				WriteInt(i, true);
				WriteInt(records[i - 1].GetWordSize(), true);
				WriteInt((int)RecordType, false); 
				WriteRecord(records[i - 1]);
			}
		}
		protected abstract void WriteRecord(T record);
		protected abstract IList<T> CreateRecords(IEnumerable<IMapItemCore> items);
		public override void UpdateExportingItems(IEnumerable<IMapItemCore> items) {
			this.records = CreateRecords(items);
		}
	}
	public abstract class ShpBoundingBoxExporter<T> : ShapeExporterCoreBase<T> where T : BoundedShpRecord {
		protected override void WriteRecord(T record) {
			WriteDouble(record.XMin);
			WriteDouble(record.YMin);
			WriteDouble(record.XMax);
			WriteDouble(record.YMax);
		}
	}
	public class ShpPointExporter : ShapeExporterCoreBase<PointShpRecord> {
		protected internal override ShpRecordTypes RecordType { get { return ShpRecordTypes.Point; } }
		protected override IList<PointShpRecord> CreateRecords(IEnumerable<IMapItemCore> items) {
			return ShapeExporterFactory.PopulatePoints(items);
		}
		protected override void WriteRecord(PointShpRecord record) {
			WriteDouble(record.Point.X);
			WriteDouble(record.Point.Y);
		}
	}
	public class ShpPolylineExporter : ShpBoundingBoxExporter<PolyLineShpRecord> {
		protected internal override ShpRecordTypes RecordType { get { return ShpRecordTypes.PolyLine; } }
		protected override IList<PolyLineShpRecord> CreateRecords(IEnumerable<IMapItemCore> items) {
			return ShapeExporterFactory.PopulatePolylines(items);
		}
		protected override void WriteRecord(PolyLineShpRecord record) {
			base.WriteRecord(record);
			WriteInt(record.PartCount, false);
			WriteInt(record.PointCount, false);
			for(int i = 0; i < record.PartCount; i++)
				WriteInt(record.Parts[i], false);
			for(int i = 0; i < record.PointCount; i++) {
				WriteDouble(record.Points[i].X);
				WriteDouble(record.Points[i].Y);
			}
		}
	}
	public class ShpPolygonExporter : ShpBoundingBoxExporter<PolygonShpRecord> {
		protected internal override ShpRecordTypes RecordType { get { return ShpRecordTypes.Polygon; } }
		protected override IList<PolygonShpRecord> CreateRecords(IEnumerable<IMapItemCore> items) {
			return ShapeExporterFactory.PopulatePolygons(items);
		}
		protected override void WriteRecord(PolygonShpRecord record) {
			base.WriteRecord(record);
			WriteInt(record.PartCount, false);
			WriteInt(record.PointCount, false);
			for(int i = 0; i < record.PartCount; i++)
				WriteInt(record.Parts[i], false);
			for(int i = 0; i < record.PointCount; i++) {
				WriteDouble(record.Points[i].X);
				WriteDouble(record.Points[i].Y);
			}
		}
	}
	public class ShpMultipointExporter : ShpBoundingBoxExporter<MultiPointShpRecord> {
		protected internal override ShpRecordTypes RecordType { get { return ShpRecordTypes.MultiPoint; } }
		protected override IList<MultiPointShpRecord> CreateRecords(IEnumerable<IMapItemCore> items) {
			return ShapeExporterFactory.PopulateMultipoints(items);
		}
		protected override void WriteRecord(MultiPointShpRecord record) {
			base.WriteRecord(record);
			WriteInt(record.PointCount, false);
			for(int i = 0; i < record.PointCount; i++) {
				WriteDouble(record.Points[i].X);
				WriteDouble(record.Points[i].Y);
			}
		}
	}
	public class ShpPointZExporter : ShapeExporterCoreBase<PointZShpRecord> {
		protected internal override ShpRecordTypes RecordType { get { return ShpRecordTypes.PointZ; } }
		protected override IList<PointZShpRecord> CreateRecords(IEnumerable<IMapItemCore> items) {
			return ShapeExporterFactory.PopulatePointsZ(items);
		}
		protected override void WriteRecord(PointZShpRecord record) {
			WriteDouble(record.PointZ.X);
			WriteDouble(record.PointZ.Y);
			WriteDouble(record.PointZ.Z);
			WriteDouble(record.PointZ.M);
		}
	}
	public class ShpPolylineZExporter : ShpBoundingBoxExporter<PolyLineZShpRecord> {
		protected internal override ShpRecordTypes RecordType { get { return ShpRecordTypes.PolyLineZ; } }
		protected override IList<PolyLineZShpRecord> CreateRecords(IEnumerable<IMapItemCore> items) {
			return ShapeExporterFactory.PopulatePolylinesZ(items);
		}
		protected override void WriteRecord(PolyLineZShpRecord record) {
			base.WriteRecord(record);
			WriteInt(record.PartCount, false);
			WriteInt(record.PointCount, false);
			for(int i = 0; i < record.PartCount; i++)
				WriteInt(record.Parts[i], false);
			for(int i = 0; i < record.PointCount; i++) {
				WriteDouble(record.Points[i].X);
				WriteDouble(record.Points[i].Y);
			}
			WriteDouble(record.ZMin);
			WriteDouble(record.ZMax);
			for(int i = 0; i < record.PointCount; i++)
				WriteDouble(record.Points[i].Z);
			WriteDouble(record.MMin);
			WriteDouble(record.MMax);
			for(int i = 0; i < record.PointCount; i++)
				WriteDouble(record.Points[i].M);
		}
	}
	public class ShpPolygonZExporter : ShpBoundingBoxExporter<PolygonZShpRecord> {
		protected internal override ShpRecordTypes RecordType { get { return ShpRecordTypes.PolygonZ; } }
		protected override IList<PolygonZShpRecord> CreateRecords(IEnumerable<IMapItemCore> items) {
			return ShapeExporterFactory.PopulatePolygonsZ(items);
		}
		protected override void WriteRecord(PolygonZShpRecord record) {
			base.WriteRecord(record);
			WriteInt(record.PartCount, false);
			WriteInt(record.PointCount, false);
			for(int i = 0; i < record.PartCount; i++)
				WriteInt(record.Parts[i], false);
			for(int i = 0; i < record.PointCount; i++) {
				WriteDouble(record.Points[i].X);
				WriteDouble(record.Points[i].Y);
			}
			WriteDouble(record.ZMin);
			WriteDouble(record.ZMax);
			for(int i = 0; i < record.PointCount; i++)
				WriteDouble(record.Points[i].Z);
			WriteDouble(record.MMin);
			WriteDouble(record.MMax);
			for(int i = 0; i < record.PointCount; i++)
				WriteDouble(record.Points[i].M);
		}
	}
	public class ShpMultipointZExporter : ShpBoundingBoxExporter<MultiPointZShpRecord> {
		protected internal override ShpRecordTypes RecordType { get { return ShpRecordTypes.MultiPointZ; } }
		protected override IList<MultiPointZShpRecord> CreateRecords(IEnumerable<IMapItemCore> items) {
			return ShapeExporterFactory.PopulateMultipointsZ(items);
		}
		protected override void WriteRecord(MultiPointZShpRecord record) {
			base.WriteRecord(record);
			WriteInt(record.PointCount, false);
			for(int i = 0; i < record.PointCount; i++) {
				WriteDouble(record.PointZs[i].X);
				WriteDouble(record.PointZs[i].Y);
			}
			WriteDouble(record.ZMin);
			WriteDouble(record.ZMax);
			for(int i = 0; i < record.PointCount; i++)
				WriteDouble(record.PointZs[i].Z);
			WriteDouble(record.MMin);
			WriteDouble(record.MMax);
			for(int i = 0; i < record.PointCount; i++)
				WriteDouble(record.PointZs[i].M);
		}
	}
	public class ShpPointMExporter : ShapeExporterCoreBase<PointMShpRecord> {
		protected internal override ShpRecordTypes RecordType { get { return ShpRecordTypes.PointM; } }
		protected override IList<PointMShpRecord> CreateRecords(IEnumerable<IMapItemCore> items) {
			return ShapeExporterFactory.PopulatePointsM(items);
		}
		protected override void WriteRecord(PointMShpRecord record) {
			WriteDouble(record.PointM.X);
			WriteDouble(record.PointM.Y);
			WriteDouble(record.PointM.M);
		}
	}
	public class ShpPolylineMExporter : ShpBoundingBoxExporter<PolyLineMShpRecord> {
		protected internal override ShpRecordTypes RecordType { get { return ShpRecordTypes.PolyLineM; } }
		protected override IList<PolyLineMShpRecord> CreateRecords(IEnumerable<IMapItemCore> items) {
			return ShapeExporterFactory.PopulatePolylinesM(items);
		}
		protected override void WriteRecord(PolyLineMShpRecord record) {
			base.WriteRecord(record);
			WriteInt(record.PartCount, false);
			WriteInt(record.PointCount, false);
			for(int i = 0; i < record.PartCount; i++)
				WriteInt(record.Parts[i], false);
			for(int i = 0; i < record.PointCount; i++) {
				WriteDouble(record.Points[i].X);
				WriteDouble(record.Points[i].Y);
			}
			WriteDouble(record.MMin);
			WriteDouble(record.MMax);
			for(int i = 0; i < record.PointCount; i++)
				WriteDouble(record.Points[i].M);
		}
	}
	public class ShpPolygonMExporter : ShpBoundingBoxExporter<PolygonMShpRecord> {
		protected internal override ShpRecordTypes RecordType { get { return ShpRecordTypes.PolygonM; } }
		protected override IList<PolygonMShpRecord> CreateRecords(IEnumerable<IMapItemCore> items) {
			return ShapeExporterFactory.PopulatePolygonsM(items);
		}
		protected override void WriteRecord(PolygonMShpRecord record) {
			base.WriteRecord(record);
			WriteInt(record.PartCount, false);
			WriteInt(record.PointCount, false);
			for(int i = 0; i < record.PartCount; i++)
				WriteInt(record.Parts[i], false);
			for(int i = 0; i < record.PointCount; i++) {
				WriteDouble(record.Points[i].X);
				WriteDouble(record.Points[i].Y);
			}
			WriteDouble(record.MMin);
			WriteDouble(record.MMax);
			for(int i = 0; i < record.PointCount; i++)
				WriteDouble(record.Points[i].M);
		}
	}
	public class ShpMultipointMExporter : ShpBoundingBoxExporter<MultiPointMShpRecord> {
		protected internal override ShpRecordTypes RecordType { get { return ShpRecordTypes.MultiPointM; } }
		protected override IList<MultiPointMShpRecord> CreateRecords(IEnumerable<IMapItemCore> items) {
			return ShapeExporterFactory.PopulateMultipointsM(items);
		}
		protected override void WriteRecord(MultiPointMShpRecord record) {
			base.WriteRecord(record);
			WriteInt(record.PointCount, false);
			for(int i = 0; i < record.PointCount; i++) {
				WriteDouble(record.PointMs[i].X);
				WriteDouble(record.PointMs[i].Y);
			}
			WriteDouble(record.MMin);
			WriteDouble(record.MMax);
			for(int i = 0; i < record.PointCount; i++)
				WriteDouble(record.PointMs[i].M);
		}
	}
}
