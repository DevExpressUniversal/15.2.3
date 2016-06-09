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
namespace DevExpress.Map.Native {
	public enum ShpRecordTypes {
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
	internal interface IPointRecord {
		void RecreatePoint(double x, double y);
	}
	internal interface IPointsArrayRecord {
		int PartCount { get; set; }
		int[] Parts { get; set; }
		int PointCount { get; set; }
		void RecreatePointsArray(int length);
		void RecreatePointValue(int pointIndex, double x, double y);
	}
	internal interface IPolylineRecord : IPointsArrayRecord {
	}
	internal interface IPolygonRecord : IPointsArrayRecord {
	}
	internal interface IMultipointRecord : IPointsArrayRecord {
	}
	public abstract class ShpRecord {
		readonly int recordNumber;
		readonly ShpRecordTypes recordType;
		public int RecordNumber { get { return recordNumber; } }
		public ShpRecordTypes RecordType { get { return recordType; } }
		protected ShpRecord(ShpRecordTypes recordType, int recordNumber) {
			this.recordType = recordType;
			this.recordNumber = recordNumber;
		}
		public abstract int GetWordSize();
		public abstract CoordBounds GetBoundingBox();
	}
	public class NullShpRecord : ShpRecord {
		public NullShpRecord(int recordNumber)
			: base(ShpRecordTypes.NullShape, recordNumber) {
		}
		public override int GetWordSize() {
			return 2;
		}
		public override CoordBounds GetBoundingBox() {
			return CoordBounds.Empty;
		}
	}
	public class PointShpRecord : ShpRecord, IPointRecord {
		ShpPoint point;
		public ShpPoint Point { get { return point; } set { point = value; } }
		public PointShpRecord(int recordNumber)
			: base(ShpRecordTypes.Point, recordNumber) {
		}
		public override int GetWordSize() {
			return 10;
		}
		#region IPointRecord implementation
		void IPointRecord.RecreatePoint(double x, double y) {
			this.point = new ShpPoint() { X = x, Y = y };
		}
		#endregion
		public override CoordBounds GetBoundingBox() {
			return new CoordBounds(point.X, point.Y, point.X, point.Y);
		}
	}
	public class PointMShpRecord : ShpRecord, IPointRecord {
		ShpPointM shpPointM;
		public ShpPointM PointM { get { return shpPointM; } set { shpPointM = value; } }
		public PointMShpRecord(int recordNumber)
			: base(ShpRecordTypes.PointM, recordNumber) {
		}
		#region IPointRecord implementation
		void IPointRecord.RecreatePoint(double x, double y) {
			this.shpPointM = new ShpPointM() { X = x, Y = y, M = 0.0 };
		}
		#endregion
		public override int GetWordSize() {
			return 14;
		}
		public override CoordBounds GetBoundingBox() {
			return new CoordBounds(shpPointM.X, shpPointM.Y, shpPointM.X, shpPointM.Y);
		}
	}
	public class PointZShpRecord : ShpRecord, IPointRecord {
		ShpPointZ shpPointZ;
		public ShpPointZ PointZ { get { return shpPointZ; } set { shpPointZ = value; } }
		public PointZShpRecord(int recordNumber)
			: base(ShpRecordTypes.PointZ, recordNumber) {
		}
		#region IPointRecord implementation
		void IPointRecord.RecreatePoint(double x, double y) {
			this.shpPointZ = new ShpPointZ() { X = x, Y = y, M = 0.0, Z = 0.0 };
		}
		#endregion
		public override int GetWordSize() {
			return 18;
		}
		public override CoordBounds GetBoundingBox() {
			return new CoordBounds(shpPointZ.X, shpPointZ.Y, shpPointZ.X, shpPointZ.Y);
		}
	}
	public abstract class BoundedMShpRecord : BoundedShpRecord {
		double mmin;
		double mmax;
		public double MMin { get { return mmin; } set { mmin = value; } }
		public double MMax { get { return mmax; } set { mmax = value; } }
		protected BoundedMShpRecord(ShpRecordTypes recordType, int recordNumber) : base(recordType, recordNumber) { }
	}
	public abstract class BoundedZShpRecord : BoundedMShpRecord {
		public double ZMin { get; set; }
		public double ZMax { get; set; }
		protected BoundedZShpRecord(ShpRecordTypes recordType, int recordNumber) : base(recordType, recordNumber) { }
	}
	public class MultiPointMShpRecord : BoundedMShpRecord, IMultipointRecord {
		int pointCount;
		ShpPointM[] pointMs;
		public int PointCount { get { return pointCount; } set { pointCount = value; } }
		public ShpPointM[] PointMs { get { return pointMs; } set { pointMs = value; } }
		public MultiPointMShpRecord(int recordNumber) : base(ShpRecordTypes.MultiPointM, recordNumber) { }
		#region IMultipointRecord implementation
		int IPointsArrayRecord.PartCount {
			get { return 1; }
			set { ; }
		}
		int[] IPointsArrayRecord.Parts {
			get { return new int[1] { 0 }; }
			set { ; }
		}
		void IPointsArrayRecord.RecreatePointsArray(int length) {
			this.pointMs = new ShpPointM[length];
		}
		void IPointsArrayRecord.RecreatePointValue(int pointIndex, double x, double y) {
			this.pointMs[pointIndex] = new ShpPointM() { X = x, Y = y, M = 0.0 };
			AppendPointToBox(x, y);
		}
		#endregion
		public override int GetWordSize() {
			return (56 + PointCount * 24) / 2;
		}
	}
	public class MultiPointZShpRecord : BoundedZShpRecord, IMultipointRecord {
		public int PointCount { get; set; } 
		public ShpPointZ[] PointZs { get; set; } 
		public MultiPointZShpRecord(int recordNumber) : base(ShpRecordTypes.MultiPointZ, recordNumber) { }
		#region IMultipointRecord implementation
		int IPointsArrayRecord.PartCount {
			get { return 1; }
			set { ; }
		}
		int[] IPointsArrayRecord.Parts {
			get { return new int[1] { 0 }; }
			set { ; }
		}
		void IPointsArrayRecord.RecreatePointsArray(int length) {
			this.PointZs = new ShpPointZ[length];
		}
		void IPointsArrayRecord.RecreatePointValue(int pointIndex, double x, double y) {
			this.PointZs[pointIndex] = new ShpPointZ() { X = x, Y = y, M = 0.0, Z = 0.0 };
			AppendPointToBox(x, y);
		}
		#endregion
		public override int GetWordSize() {
			return (72 + 32 * PointCount) / 2;
		}
	}
	public class PolyLineMShpRecord : BoundedMShpRecord, IPolylineRecord {
		int pointCount, partCount;
		int[] parts;
		ShpPointM[] pointMs;
		public int PointCount { get { return pointCount; } set { pointCount = value; } }
		public int PartCount { get { return partCount; } set { partCount = value; } }
		public int[] Parts { get { return parts; } set { parts = value; } }
		public ShpPointM[] Points { get { return pointMs; } set { pointMs = value; } }
		public PolyLineMShpRecord(int recordNumber) : base(ShpRecordTypes.PolyLineM, recordNumber) { }
		#region IPolylineRecord implementation
		void IPointsArrayRecord.RecreatePointsArray(int length) {
			this.pointMs = new ShpPointM[length];
		}
		void IPointsArrayRecord.RecreatePointValue(int pointIndex, double x, double y) {
			this.pointMs[pointIndex]  = new ShpPointM() { X = x, Y = y, M = 0.0 };
			AppendPointToBox(x, y);
		}
		#endregion
		public override int GetWordSize() {
			return (60 + 24 * PointCount + 4 * PartCount) / 2;
		}
	}
	public class PolyLineZShpRecord : BoundedZShpRecord, IPolylineRecord {
		public int PointCount { get; set; }
		public int PartCount { get; set; }
		public int[] Parts { get; set; }
		public ShpPointZ[] Points { get; set; }
		public PolyLineZShpRecord(int recordNumber) : base(ShpRecordTypes.PolyLineZ, recordNumber) { }
		#region IPolylineRecord implementation
		void IPointsArrayRecord.RecreatePointsArray(int length) {
			this.Points = new ShpPointZ[length];
		}
		void IPointsArrayRecord.RecreatePointValue(int pointIndex, double x, double y) {
			this.Points[pointIndex] = new ShpPointZ() { X = x, Y = y, M = 0.0, Z = 0.0 };
			AppendPointToBox(x, y);
		}
		#endregion
		public override int GetWordSize() {
			return (76 + 32 * PointCount + 4 * PartCount) / 2;
		}
	}
	public class PolygonMShpRecord : BoundedMShpRecord, IPolygonRecord {
		int pointCount, partCount;
		int[] parts;
		ShpPointM[] points;
		public int PointCount { get { return pointCount; } set { pointCount = value; } }
		public int PartCount { get { return partCount; } set { partCount = value; } }
		public int[] Parts { get { return parts; } set { parts = value; } }
		public ShpPointM[] Points { get { return points; } set { points = value; } }
		public PolygonMShpRecord(int recordNumber)
			: base(ShpRecordTypes.PolygonM, recordNumber) {
		}
		#region IPolygonRecord implementation
		void IPointsArrayRecord.RecreatePointsArray(int length) {
			this.points = new ShpPointM[length];
		}
		void IPointsArrayRecord.RecreatePointValue(int pointIndex, double x, double y) {
			this.points[pointIndex] = new ShpPointM() { X = x, Y = y, M = 0.0 };
			this.AppendPointToBox(x, y);
		}
		#endregion
		public override int GetWordSize() {
			return (60 + 24 * PointCount + 4 * PartCount) / 2;
		}
	}
	public class PolygonZShpRecord : BoundedZShpRecord, IPolygonRecord {
		public int PointCount { get; set; }
		public int PartCount { get; set; }
		public int[] Parts { get; set; }
		public ShpPointZ[] Points { get; set; }
		public PolygonZShpRecord(int recordNumber)
			: base(ShpRecordTypes.PolygonZ, recordNumber) {
		}
		#region IPolygonRecord implementation
		void IPointsArrayRecord.RecreatePointsArray(int length) {
			this.Points = new ShpPointZ[length];
		}
		void IPointsArrayRecord.RecreatePointValue(int pointIndex, double x, double y) {
			this.Points[pointIndex] = new ShpPointZ() { X = x, Y = y, M = 0.0, Z = 0.0 };
			this.AppendPointToBox(x, y);
		}
		#endregion
		public override int GetWordSize() {
			return (76 + 32 * PointCount + 4 * PartCount) / 2;
		}
	}
	public abstract class BoundedShpRecord : ShpRecord {
		double xmin = double.PositiveInfinity, ymin = double.PositiveInfinity, xmax = double.NegativeInfinity, ymax = double.NegativeInfinity;
		public double XMin { get { return xmin; } set { xmin = value; } }
		public double YMin { get { return ymin; } set { ymin = value; } }
		public double XMax { get { return xmax; } set { xmax = value; } }
		public double YMax { get { return ymax; } set { ymax = value; } }
		protected BoundedShpRecord(ShpRecordTypes recordType, int recordNumber)
			: base(recordType, recordNumber) {
		}
		public override CoordBounds GetBoundingBox() {
			return new CoordBounds(xmin, ymin, xmax, ymax);
		}
		protected void AppendPointToBox(double x, double y) {
			this.xmin = Math.Min(xmin, x);
			this.xmax = Math.Max(xmax, x);
			this.ymin = Math.Min(ymin, y);
			this.ymax = Math.Max(ymax, y);
		}
	}
	public class MultiPointShpRecord : BoundedShpRecord, IMultipointRecord {
		int pointCount;
		ShpPoint[] points;
		public int PointCount { get { return pointCount; } set { pointCount = value; } }
		public ShpPoint[] Points { get { return points; } set { points = value; } }
		public MultiPointShpRecord(int recordNumber)
			: base(ShpRecordTypes.MultiPoint, recordNumber) {
		}
		#region IMultipointRecord implementation
		int IPointsArrayRecord.PartCount {
			get { return 1; }
			set { ; }
		}
		int[] IPointsArrayRecord.Parts {
			get { return new int[1] { 0 }; }
			set { ; }
		}
		void IPointsArrayRecord.RecreatePointsArray(int length) {
			this.points = new ShpPoint[length];
		}
		void IPointsArrayRecord.RecreatePointValue(int pointIndex, double x, double y) {
			this.points[pointIndex] = new ShpPoint() { X = x, Y = y };
			AppendPointToBox(x, y);
		}
		#endregion
		public override int GetWordSize() {
			return (40 + 16 * PointCount) / 2;
		}
	}
	public class PolyLineShpRecord : BoundedShpRecord, IPolylineRecord {
		int pointCount, partCount;
		int[] parts;
		ShpPoint[] points;
		public int PointCount { get { return pointCount; } set { pointCount = value; } }
		public int PartCount { get { return partCount; } set { partCount = value; } }
		public int[] Parts { get { return parts; } set { parts = value; } }
		public ShpPoint[] Points { get { return points; } set { points = value; } }
		public PolyLineShpRecord(int recordNumber)
			: this(ShpRecordTypes.PolyLine, recordNumber) {
		}
		protected PolyLineShpRecord(ShpRecordTypes recotdType, int recordNumber)
			: base(recotdType, recordNumber) {
		}
		#region IPolylineRecord implementation
		void IPointsArrayRecord.RecreatePointsArray(int length) {
			this.points = new ShpPoint[length];
		}
		void IPointsArrayRecord.RecreatePointValue(int pointIndex, double x, double y) {
			this.points[pointIndex] = new ShpPoint() { X = x, Y = y };
			AppendPointToBox(x, y);
		}
		#endregion
		public override int GetWordSize() {
			return (44 + 16 * PointCount + 4 * PartCount) / 2;
		}
	}
	public class PolygonShpRecord : BoundedShpRecord, IPolygonRecord {
		int pointCount, partCount;
		int[] parts;
		ShpPoint[] points;
		public int PointCount { get { return pointCount; } set { pointCount = value; } }
		public int PartCount { get { return partCount; } set { partCount = value; } }
		public int[] Parts { get { return parts; } set { parts = value; } }
		public ShpPoint[] Points { get { return points; } set { points = value; } }
		public PolygonShpRecord(int recordNumber)
			: base(ShpRecordTypes.Polygon, recordNumber) {
		}
		#region IPolygonRecord implementation
		void IPointsArrayRecord.RecreatePointsArray(int length) {
			this.points = new ShpPoint[length];
		}
		void IPointsArrayRecord.RecreatePointValue(int pointIndex, double x, double y) {
			this.points[pointIndex] = new ShpPoint() { X = x, Y = y };
			this.AppendPointToBox(x, y);
		}
		#endregion
		public override int GetWordSize() {
			return (44 + 16 * PointCount + 4 * PartCount) / 2;
		}
	}
}
