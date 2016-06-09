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
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using DevExpress.XtraPrinting.Native;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraPrinting.Export
{
	public class DocToPixelConverter : GraphicsUnitConverter {
		public static int Convert(float val) {
			return System.Convert.ToInt32(GraphicsUnitConverter.Convert(val, GraphicsDpi.Document, GraphicsDpi.Pixel));
		}
		public static Point Convert(PointF val) {
			return Point.Round(Convert(val, GraphicsDpi.Document, GraphicsDpi.Pixel));
		}
		public static Size Convert(SizeF val) {
			return Size.Round(Convert(val, GraphicsDpi.Document, GraphicsDpi.Pixel));
		}
		public static Rectangle Convert(RectangleF val) {
			return Round(GraphicsUnitConverter.Convert(val, GraphicsDpi.Document, GraphicsDpi.Pixel));
		}
	}
	public class ObjectInfo 
	{
		int rowIndex = 0;
		int colIndex = 0;
		int rowSpan = 0;
		int colSpan = 0;
		object obj = null;
		public ObjectInfo(int colIndex, int rowIndex, int colSpan, int rowSpan, object obj) {
			this.rowIndex = rowIndex;
			this.colIndex = colIndex;
			this.rowSpan = rowSpan;
			this.colSpan = colSpan;
			this.obj = obj;
		}
		public int RowIndex { get { return rowIndex; }
		}
		public int ColIndex { get { return colIndex; }
		}
		public int RowSpan { get { return rowSpan; }
		}
		public int ColSpan { get { return colSpan; }
		}
		public object Object { get { return obj; }
		}
	}
	public class RectangleDivider {
		static void MakeArrayListUnique(List<int> arrayList) {
			if (arrayList.Count == 0)
				return;
			int pos = 0;
			int arrayListCount = arrayList.Count;
			for (int i = 1; i < arrayListCount; i++)
				if (arrayList[i] != arrayList[pos])
					arrayList[++pos] = arrayList[i];
			pos++;
			int removeCount = arrayListCount - pos;
			if (removeCount == 0)
				return;
			arrayList.RemoveRange(pos, removeCount);
		}
		static List<int> GetIntervals(List<int> bounds) {
			List<int> intervals = new List<int>();
			int maxIndex = bounds.Count - 1;
			for(int i = 0; i < maxIndex; i++)
				intervals.Add(bounds[i + 1] - bounds[i]);
			return intervals;
		}
		Rectangle bounds;
		List<int> xCoords, yCoords;
		List<Rectangle> innerAreas;
		bool finalized;
		public List<int> XCoords { 
			get {
				ProcessData(); 
				return xCoords; 
			} 
		}
		public List<int> YCoords { 
			get {
				ProcessData(); 
				return yCoords; 
			} 
		}
		public List<int> ColWidths { get { return GetIntervals(XCoords); } }
		public List<int> RowHeights { get { return GetIntervals(YCoords); } }
		public RectangleDivider(Rectangle bounds) {
			this.bounds = bounds;
			xCoords = new List<int>();
			yCoords = new List<int>();
			innerAreas = new List<Rectangle>();
			AddPoint(bounds.Left, bounds.Top);
			AddPoint(bounds.Right, bounds.Bottom);
		}
		void AddPoint(int xCoord, int yCoord) {
			xCoords.Add(xCoord);
			yCoords.Add(yCoord);
			finalized = false;
		}
		void ProcessData() {
			if (!finalized) {
				xCoords.Sort();
				yCoords.Sort();
				MakeArrayListUnique(xCoords);
				MakeArrayListUnique(yCoords);
				finalized = true;
			}
		}
		public void AddInnerArea(Rectangle area, bool onlyTopLeft) {
			innerAreas.Add(area);
			AddPoint(area.Left, area.Top);
			if (!onlyTopLeft)
				AddPoint(area.Right, area.Bottom);
		}
		public void AddInnerAreas(Rectangle[] areas, bool onlyTopLeft) {
			foreach(Rectangle item in areas)
				AddInnerArea(item, onlyTopLeft);
		}
		public List<Rectangle> GenerateEmptyAreas() {
			DateTime time = DateTime.Now;
			List<Rectangle> result = new List<Rectangle>();
			ProcessData();
			int yCoordsCount = yCoords.Count;
			for (int j = 1; j < yCoordsCount; j++) {
				int height = yCoords[j] - yCoords[j - 1];
				int xCoordsCount = xCoords.Count;
				for (int i = 1; i < xCoordsCount; i++) {
					int width = xCoords[i] - xCoords[i - 1];
					Rectangle rect = new Rectangle(xCoords[i - 1], yCoords[j - 1], width, height);
					bool found = false;
					int innerAreasCount = innerAreas.Count;
					for (int k = 0; k < innerAreasCount; k++)
						if (innerAreas[k].Contains(rect)) {
							found = true;
							break;
						}
					if (!found)
						result.Add(rect);
				}
			}
			return result;
		}
	}
#if DEBUGTEST
	[System.Diagnostics.DebuggerDisplay(@"\{{GetType().FullName,nq}, coord = {coord}, index = {index}, isStart= {isStart}}")]
#endif
	public class CoordInfo : IComparable<CoordInfo> {
		public float coord;
		public int index;
		public bool isStart;
		public int CompareTo(CoordInfo other) {
			return coord.CompareTo(other.coord);
		}
	}
	public class RectangleDividerF {
		class RangeCalculator {
			protected Range[] ranges;
			List<CoordInfo> coords;
			int farBound;
			List<int> lengths;
			public List<int> Lengths { get { return lengths; } }
			public int FarBound { get { return farBound; } }
			public RangeCalculator(int size, Range[] ranges) {
				this.ranges = ranges;
				coords = new List<CoordInfo>(size);
				lengths = new List<int>();
			}
			public virtual void AddArea(float start, float end, int i) {
				AddValue(start, i, true);
				AddValue(end, i, false);
			}
			public virtual bool CalcRanges() {
				coords.Sort();
				int current = 0;
				int i = 0;
				farBound = 0;
				float prevCoord = 0.0f;
				int prevPosition = 0;
				bool zeroBound = false;
				if(!ShouldAddPosition(coords[0], prevCoord, prevPosition)) {
					ranges[coords[0].index].start = current;
					prevCoord = coords[0].coord;
					prevPosition = Round(coords[0].coord);
					zeroBound = true;
					i++;
				}
				for(; i < coords.Count; i++) {
					CoordInfo coordInfo = coords[i];
					if(ShouldAddPosition(coordInfo, prevCoord, prevPosition)) {
						prevCoord = coordInfo.coord;
						prevPosition = AddPosition(coordInfo.coord, prevPosition);
						current++;
					}
					AssignRange(coordInfo, current);
				}
				for(i = 0; i < lengths.Count; i++)
					farBound += lengths[i];
				return zeroBound;
			}
			static bool ShouldAddPosition(CoordInfo coordInfo, float prevCoord, int prevPosition) {
				return coordInfo.coord - prevCoord > 0.5 && Round(coordInfo.coord) != prevPosition;
			}
			protected virtual int AddPosition(float position, int prevPosition) {
				int intPosition = Round(position);
				lengths.Add(intPosition - prevPosition);
				return intPosition;
			}
			protected virtual void AssignRange(CoordInfo coordInfo, int current) {
				if(coordInfo.isStart)
					ranges[coordInfo.index].start = current;
				else 
					ranges[coordInfo.index].end = current;
			}
			protected void AddValue(float value, int index, bool isStart) {
				coords.Add(new CoordInfo { coord = value, index = index, isStart = isStart });
			}
		}
		class RangeCalculatorTopLeftOnly : RangeCalculator { 
			float maxBound;
			List<float> bounds;
			public List<float> Bounds { get { return bounds; } }
			public RangeCalculatorTopLeftOnly(int size, Range[] ranges)
				: base(size, ranges) {
				maxBound = float.MinValue;
				bounds = new List<float>();
			}
			public override void AddArea(float start, float end, int i) {
				AddValue(start, i, true);
				maxBound = Math.Max(maxBound, end);
			}
			public override bool CalcRanges() {
				AddValue(maxBound, -1, false);
				return base.CalcRanges();
			}
			protected override int AddPosition(float position, int prevPosition) {
				bounds.Add(position);
				return base.AddPosition(position, prevPosition);
			}
			protected override void AssignRange(CoordInfo coordInfo, int current) {
				if(coordInfo.isStart)
					ranges[coordInfo.index].start = current;
			}
		}
#if DEBUGTEST
		[System.Diagnostics.DebuggerDisplay(@"\{{GetType().FullName,nq}, S = {start}, E = {end}, Span= {end - start}}")]
#endif
		public struct Range {
			public int start;
			public int end;
		}
		List<int> columnWidths;
		List<int> rowHeights;
		bool zeroColumnNeeded;
		bool zeroRowNeeded;
		Range[] xRanges;
		Range[] yRanges;
		int right;
		int bottom;
		public int Right {
			get { return right; }
		}
		public int Bottom {
			get { return bottom; }
		}
		public List<int> ColWidths { 
			get { return columnWidths; }
		}
		public List<int> RowHeights { 
			get { return rowHeights; }
		}
		public Range[] XRanges {
			get { return xRanges; }
		}
		public Range[] YRanges {
			get { return yRanges; }
		}
		public bool ZeroColumnNeeded { 
			get { return zeroColumnNeeded; } 
		}
		public bool ZeroRowNeeded { 
			get { return zeroRowNeeded; } 
		}
		public RectangleDividerF(RectangleF[] areas, bool onlyTopLeft) {
			int size = areas.Length;
			xRanges = new Range[size];
			yRanges = new Range[size];
			RangeCalculator xCalc = CreateRangeCalculator(onlyTopLeft, size, xRanges);
			RangeCalculator yCalc = CreateRangeCalculator(onlyTopLeft, size, yRanges);
			for(int i = 0; i < areas.Length; i++) {
				RectangleF area = areas[i];
				xCalc.AddArea(area.Left, area.Right, i);
				yCalc.AddArea(area.Top, area.Bottom, i);
			}
			zeroColumnNeeded = xCalc.CalcRanges();
			columnWidths = xCalc.Lengths;
			right = xCalc.FarBound;
			zeroRowNeeded = yCalc.CalcRanges();
			rowHeights = yCalc.Lengths;
			bottom = yCalc.FarBound;
			if(onlyTopLeft) {
				List<float> columnBounds = ((RangeCalculatorTopLeftOnly)xCalc).Bounds;
				List<float> rowBounds = ((RangeCalculatorTopLeftOnly)yCalc).Bounds;
				for(int i = 0; i < areas.Length; i++) {
					xRanges[i].end = FindBound(columnBounds, areas[i].Right, xRanges[i].start) + 1;
					yRanges[i].end = FindBound(rowBounds, areas[i].Bottom, yRanges[i].start) + 1;
				}
			}
		}
		static RangeCalculator CreateRangeCalculator(bool onlyTopLeft, int size, Range[] ranges) {
			return onlyTopLeft ? new RangeCalculatorTopLeftOnly(size, ranges) : new RangeCalculator(size, ranges);
		}
		static int FindBound(List<float> bounds, float areaBound, int start) {
			int index = bounds.BinarySearch(start, bounds.Count - start, Round(areaBound), null);
			index = index < 0 ? ~index : index;
			return index > 0 && areaBound - bounds[index - 1] < 1 ? index - 1 : index;
		}
		static int Round(float value) {
			return (int)Math.Round(value, MidpointRounding.AwayFromZero);
		}
	}
	public class MegaTable
	{
		#region internal classes
		enum MoveAction { MoveColumns, MoveRows };
		class DescLayoutControlComparer : IComparer<ILayoutControl> {
			int minLeft = int.MaxValue;
			int minTop = int.MaxValue;
			public int MinLeft { get { return minLeft; } }
			public int MinTop { get { return minTop; } }
			public DescLayoutControlComparer() { }
			public DescLayoutControlComparer(ILayoutControl startControl) {
				this.minLeft = startControl.Left;
				this.minTop = startControl.Top;
			}
			int IComparer<ILayoutControl>.Compare(ILayoutControl xControl, ILayoutControl yControl) {
				return Compare(xControl.Left, xControl.Top, yControl.Left, yControl.Top);
			}
			int Compare(int x1, int y1, int x2, int y2) {
				if(x1 > x2) {
					minLeft = Math.Min(minLeft, x2);
					minTop = Math.Min(minTop, Math.Min(y1, y2));
					return 1;
				} else if(x1 < x2) {
					minLeft = Math.Min(minLeft, x1);
					minTop = Math.Min(minTop, Math.Min(y1, y2));
					return -1;
				} else if(y1 > y2) {
					minTop = Math.Min(minTop, y2);
					return 1;
				} else if(y1 < y2) {
					minTop = Math.Min(minTop, y1);
					return -1;
				} else
					return 0;
			}
		}		
		#endregion
		#region fields and properties
		int tableRight;
		int tableBottom;
		List<int> colWidths = new List<int>();
		List<int> rowHeights = new List<int>();
		ObjectInfo[] fObjects = new ObjectInfo[0];
		LayoutControlCollection layoutControls;
		bool isEmpty = false;
		bool highGridDensity = false;
		public ObjectInfo[] Objects { get { return fObjects; }
		}
		public int Width {
			get { return tableRight; }
		}
		public int Height {
			get { return tableBottom; }
		}
		public List<int> ColWidths {
			get { return colWidths; }
		}
		public List<int> RowHeights {
			get { return rowHeights; }
		}
		public bool ZeroColumnNeeded { get; private set; }
		public bool ZeroRowNeeded { get; private set; }
		public virtual int RowCount { get { return rowHeights.Count; } 
		}
		public virtual int ColumnCount { get { return colWidths.Count; }
		}
		public bool IsEmpty { get { return isEmpty; }
		}
		#endregion
		public MegaTable(LayoutControlCollection layoutControls, bool highGridDensity, bool correctControlBounds) {
			if(layoutControls == null || layoutControls.Count == 0)  {
				isEmpty = true;
				return;
			}
			this.highGridDensity = highGridDensity;
			this.layoutControls = layoutControls;
			DescLayoutControlComparer comparer = new DescLayoutControlComparer(layoutControls[0]);
			layoutControls.Sort(comparer);
			RectangleF[] rects = new RectangleF[layoutControls.Count];
			double criticalDelta =  Math.Ceiling(GraphicsUnitConverter.Convert(1f, GraphicsDpi.Millimeter, GraphicsDpi.DeviceIndependentPixel));
			for(int i = 0; i < layoutControls.Count; i++) {
				rects[i] = layoutControls[i].BoundsF;
			}
			if(correctControlBounds)
				CorrectControlBounds(rects);
			if(!highGridDensity) {
				for(int i = 0; i < layoutControls.Count; i++) {
					RectangleF rect = RectHelper.OffsetRectF(rects[i], -comparer.MinLeft, -comparer.MinTop);
					if(i > 0) {
						float delta = rect.X - rects[i - 1].X;
						if(delta < 0 && Math.Ceiling(Math.Abs(delta)) < criticalDelta) {
							rect.X -= delta;
							rect.Width += delta;
						}
					}
					rects[i] = rect;
				}
			}
			DivideAreaByRectanglesAndCalculateSpans(layoutControls, rects);
		}
		void DivideAreaByRectanglesAndCalculateSpans(LayoutControlCollection layoutControls, RectangleF[] rects) {
			RectangleDividerF divider = new RectangleDividerF(rects, !highGridDensity);
			tableRight = divider.Right;
			tableBottom = divider.Bottom;
			colWidths = divider.ColWidths;
			rowHeights = divider.RowHeights;
			ZeroColumnNeeded = divider.ZeroColumnNeeded;
			ZeroRowNeeded = divider.ZeroRowNeeded;
			List<ObjectInfo> objects = new List<ObjectInfo>(rects.Length);
			for(int i = 0; i < rects.Length; i++) {
				RectangleDividerF.Range xRange = divider.XRanges[i];
				RectangleDividerF.Range yRange = divider.YRanges[i];
				int right = xRange.end - xRange.start;
				int bottom = yRange.end - yRange.start;
				objects.Add(new ObjectInfo(xRange.start, yRange.start, right, bottom, layoutControls[i]));
			}
			fObjects = objects.ToArray();
		}
		public static void Dump(RectangleF[] rects) {
#if !DXPORTABLE
			System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
			System.IO.MemoryStream ms = new System.IO.MemoryStream();
			bf.Serialize(ms, rects);
#endif
		}
		static void CorrectControlBounds(RectangleF[] rects) {
			float xDelta, yDelta;
			RectangleF firstControlBounds, secondControlBounds;
			for(int i = 0; i < rects.Length; i++) {
				firstControlBounds = rects[i];
				for(int j = i + 1; j < rects.Length; j++) {
					secondControlBounds = rects[j];
					if(firstControlBounds.Right <= secondControlBounds.X)
						break;
					if(Contains(firstControlBounds, secondControlBounds.Location)) {
						xDelta = firstControlBounds.Right - secondControlBounds.Left;
						yDelta = firstControlBounds.Bottom - secondControlBounds.Top;
						MoveControlBounds(rects, j, xDelta, yDelta);
					}
				}
			}
		}
		static void MoveControlBounds(RectangleF[] rects, int index, float xDelta, float yDelta) {
			if(GetMoveAction(xDelta, yDelta) == MoveAction.MoveColumns)
				MoveControlColumnBounds(rects, index, xDelta);
			else
				MoveControlRowBounds(rects, index, yDelta);
		}
		static bool Contains(RectangleF rectangle, PointF point) {
			return (rectangle.Left < point.X && point.X < rectangle.Right
				&& (rectangle.Top <= point.Y && point.Y < rectangle.Bottom)) 
				||
				(rectangle.Top < point.Y && point.Y < rectangle.Bottom
				&& (rectangle.Left <= point.X && point.X < rectangle.Right));
		}
		static MoveAction GetMoveAction(float xDelta, float yDelta) {
			System.Diagnostics.Debug.Assert(xDelta >=0 && yDelta >= 0);
			return xDelta > 0 && (yDelta == 0 || xDelta <= yDelta) ? MoveAction.MoveColumns : 
				MoveAction.MoveRows;
		}
		static void MoveControlColumnBounds(RectangleF[] rects, int index, float xDelta) {
			System.Diagnostics.Debug.Assert(xDelta > 0);
			for(int i = index; i < rects.Length; i++) {
				RectangleF rect = rects[i];
				rects[i] = new RectangleF(rect.Left + xDelta, rect.Top, rect.Width, rect.Height);
			}
		}
		static void MoveControlRowBounds(RectangleF[] rects, int index, float yDelta) {
			System.Diagnostics.Debug.Assert(yDelta > 0);
			float top = rects[index].Top;
			for(int i = 0; i < rects.Length; i++) {
				RectangleF rect = rects[i];
				if(rect.Top >= top)
					rects[i] = new RectangleF(rect.Left, rect.Top + yDelta, rect.Width, rect.Height);
			}
		}
#if DEBUGTEST
		public static void Test_SortLayoutControls(LayoutControlCollection layoutControls) {
			layoutControls.Sort(new DescLayoutControlComparer());
		}
		public static void Test_CorrectControlBounds(LayoutControlCollection layoutControls) {
			RectangleF[] rects = new RectangleF[layoutControls.Count];
			for(int i = 0; i < layoutControls.Count; i++)
				rects[i] = layoutControls[i].BoundsF;
			CorrectControlBounds(rects);
			for(int i = 0; i < layoutControls.Count; i++)
				layoutControls[i].Bounds = Rectangle.Round(rects[i]);
		}
#endif
	}
	public class LayoutControlDivider {
		class CoordComparer : IComparer<CoordInfo> {
			int IComparer<CoordInfo>.Compare(CoordInfo first, CoordInfo second) {
				int result = first.CompareTo(second);
				return result == 0 ? first.isStart.CompareTo(second.isStart) : result;
			}
		}
		class CoordCalculator {
			List<CoordInfo> coords;
			public List<CoordInfo> Coords { get { return coords; } }
			public CoordCalculator(int size) {
				coords = new List<CoordInfo>(size);
			}
			public virtual void AddArea(float start, float end, int i) {
				AddValue(start, i, true);
				AddValue(end, i, false);
			}
			public void SortMergeCoords() {
				coords.Sort();
				int current = 0;
				int i = 0;
				float prevCoord = 0.0f;
				int prevPosition = 0;
				if(!ShouldAdvancePosition(coords[0], prevCoord, prevPosition)) {
					prevCoord = coords[0].coord;
					prevPosition = Round(coords[0].coord);
					i++;
				}
				for(; i < coords.Count; i++) {
					CoordInfo coordInfo = coords[i];
					if(ShouldAdvancePosition(coordInfo, prevCoord, prevPosition)) {
						prevCoord = coordInfo.coord;
						prevPosition = Round(coordInfo.coord);
						current++;
					} else {
						coordInfo.coord = prevCoord;
					}
				}
				coords.Sort(new CoordComparer());
			}
			static bool ShouldAdvancePosition(CoordInfo coordInfo, float prevCoord, int prevPosition) {
				return coordInfo.coord - prevCoord > 0.5 && Round(coordInfo.coord) != prevPosition;
			}
			void AddValue(float value, int index, bool isStart) {
				coords.Add(new CoordInfo { coord = value, index = index, isStart = isStart });
			}
		}
		List<LayoutControlCollection> parts;
		public List<LayoutControlCollection> Parts {
			get { return parts; }
		}
		public LayoutControlDivider(LayoutControlCollection layoutControls) {
			if(layoutControls == null || layoutControls.Count == 0) {
				return;
			}
			CoordCalculator yCalc = new CoordCalculator(layoutControls.Count);
			for(int i = 0; i < layoutControls.Count; i++) {
				RectangleF area = layoutControls[i].BoundsF;
				yCalc.AddArea(area.Top, area.Bottom, i);
			}
			yCalc.SortMergeCoords();
			parts = FillParts(layoutControls, yCalc);
		}
		static List<LayoutControlCollection> FillParts(LayoutControlCollection layoutControls, CoordCalculator yCalc) {
			List<LayoutControlCollection> parts = new List<LayoutControlCollection>();
			LayoutControlCollection partControls = new LayoutControlCollection();
			List<int> currentAreas = new List<int>();
			List<CoordInfo> coords = yCalc.Coords;
			for(int i = 0; i < coords.Count; i++) {
				var coord = coords[i];
				if(coord.isStart) {
					partControls.Add(layoutControls[coord.index]);
					currentAreas.Add(coord.index);
				} else {
					currentAreas.Remove(coord.index);
					if(currentAreas.Count == 0) {
						parts.Add(partControls);
						partControls = new LayoutControlCollection();
					}
				}
			}
			return parts;
		}
		static int Round(float value) {
			return (int)Math.Round(value, MidpointRounding.AwayFromZero);
		}
	}
	public class ColorCollection : CollectionBase 
	{
		public ColorCollection() : base() {
		}
		public int Add(Color color) {
			return List.Add(color);
		}
		public Color this[int index] {
			get { return (Color)List[index]; }
			set { List[index] = value; }
		}
		public int IndexOf(Color color) {
			for(int i = 0; i < Count; i++) {
				if(IsColorEqual(this[i], color))
					return i;
			} 
			return -1;
		}
		internal static bool IsColorEqual(Color firstColor, Color secondColor) {
			return firstColor.R == secondColor.R &&
				firstColor.G == secondColor.G &&
				firstColor.B == secondColor.B &&
				firstColor.A == secondColor.A;			
		}
	}
}
