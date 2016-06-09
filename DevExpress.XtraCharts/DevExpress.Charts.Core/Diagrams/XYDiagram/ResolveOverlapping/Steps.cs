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

using System.Collections.Generic;
namespace DevExpress.Charts.Native {
	public struct TestPosition {
		public static int CompareByDistance(TestPosition x, TestPosition y) {
			double diff = x.Distance - y.Distance;
			if (diff < 0)
				return -1;
			if (diff > 0)
				return 1;
			int xDiff = x.Position.X - y.Position.X;
			return xDiff == 0 ? x.Position.Y - y.Position.Y : xDiff;
		}
		readonly double distance;
		readonly GPoint2D position;
		bool check;
		public double Distance { get { return distance; } }
		public GPoint2D Position { get { return position; } }
		public bool Check { get { return check; } set { check = value; } }
		public TestPosition(GPoint2D position, double distance) {
			this.position = position;
			this.distance = distance;
			this.check = false;
		}
	}
	public abstract class Step {
		readonly GRect2D allocationRect;
		readonly RectanglesLayout layout;
		readonly double centerX;
		readonly double centerY;
		bool isEnd = false;
		int position;
		List<TestPosition> primaryTestPositions = new List<TestPosition>();
		List<TestPosition> testPositions = new List<TestPosition>();
		double maxDistance;
		protected GRect2D AllocationRect { get { return allocationRect; } }
		protected abstract int Increment { get; }
		protected RectanglesLayout Layout { get { return layout; } }
		public bool IsEnd { get { return isEnd; } }
		public int Position { get { return position; } }
		public List<TestPosition> TestPositions { get { return testPositions; } }
		public Step(int position, GRect2D rect, RectanglesLayout layout) {
			this.position = position;
			this.layout = layout;
			this.allocationRect = rect;
			this.centerX = rect.Left + rect.Width * 0.5;
			this.centerY = rect.Top + rect.Height * 0.5;
		}
		protected void AddPrimaryTestPosition(GPoint2D position) {
			double distance = CalcDistanceToAllocationRect(position);
			if (distance > maxDistance)
				maxDistance = distance;
			TestPosition test = new TestPosition(position, distance);
			primaryTestPositions.Add(test);
			testPositions.Add(test);
		}
		protected void AddSecondaryTestPosition(GPoint2D position) {
			double distance = CalcDistanceToAllocationRect(position);
			if (distance <= maxDistance)
				testPositions.Add(new TestPosition(position, distance));
		}
		protected double CalcDistanceToAllocationRect(GPoint2D position) {
			double x = position.X + allocationRect.Width * 0.5;
			double y = position.Y + allocationRect.Height * 0.5;
			return (x - centerX) * (x - centerX) + (y - centerY) * (y - centerY);
		}
		protected bool IsUpdateEnd(double distance) {
			return distance > maxDistance;
		}
		protected abstract void UpdatePrimaryTestPositions();
		protected abstract void UpdateSecondaryTestPositions();
		protected abstract bool IsEndPosition(int position);
		public bool IsTestingEnd() {
			foreach (TestPosition test in primaryTestPositions) {
				if (!test.Check)
					return false;
			}
			return true;
		}
		public void Update() {
			maxDistance = 0;
			testPositions.Clear();
			primaryTestPositions.Clear();
			if (IsEnd)
				return;
			UpdatePrimaryTestPositions();
			UpdateSecondaryTestPositions();
		}
		public void Next() {
			if (isEnd)
				return;
			if (!IsEndPosition(position + Increment))
				position += Increment;
			else
				isEnd = true;
		}
	}
	public abstract class HorizontalStep : Step {
		readonly int startRowIndex;
		protected int StartRowIndex { get { return startRowIndex; } }
		public HorizontalStep(int position, int startRowIndex, GRect2D rect, RectanglesLayout layout)
			: base(position, rect, layout) {
			this.startRowIndex = startRowIndex;
		}
		protected abstract GPoint2D CalcNearPosition(Cell cell);
		protected abstract void CalcFarPositions(Cell cell, out GPoint2D point1, out GPoint2D point2);
		protected override void UpdatePrimaryTestPositions() {
			Cell cell = Layout.GetCell(startRowIndex, Position);
			if (cell.IsEmpty)
				AddPrimaryTestPosition(CalcNearPosition(cell));
			for (int i = Layout.StepToBottom.Position; i <= Layout.StepToTop.Position; i++) {
				cell = Layout.GetCell(i, Position);
				if (cell.IsEmpty) {
					GPoint2D point1, point2;
					CalcFarPositions(cell, out point1, out point2);
					AddPrimaryTestPosition(point1);
					AddPrimaryTestPosition(point2);
				}
			}
		}
		protected override void UpdateSecondaryTestPositions() {
			for (int i = Position + Increment; !IsEndPosition(i); i += Increment) {
				Cell cell = Layout.GetCell(startRowIndex, i);
				GPoint2D point = CalcNearPosition(cell);
				double distance = CalcDistanceToAllocationRect(point);
				if (IsUpdateEnd(distance))
					break;
				if (cell.IsEmpty)
					AddSecondaryTestPosition(point);
				for (int j = Layout.StepToBottom.Position; j <= Layout.StepToTop.Position; j++) {
					cell = Layout.GetCell(j, i);
					if (cell.IsEmpty) {
						GPoint2D point1, point2;
						CalcFarPositions(cell, out point1, out point2);
						AddSecondaryTestPosition(point1);
						AddSecondaryTestPosition(point2);
					}
				}
			}
		}
	}
	public class StepToleft : HorizontalStep {
		protected override int Increment { get { return -1; } }
		public StepToleft(int position, int startRowIndex, GRect2D rect, RectanglesLayout layout)
			: base(position, startRowIndex, rect, layout) {
		}
		protected override GPoint2D CalcNearPosition(Cell cell) {
			return new GPoint2D(cell.Bounds.Right - AllocationRect.Width, AllocationRect.Top);
		}
		protected override void CalcFarPositions(Cell cell, out GPoint2D point1, out GPoint2D point2) {
			point1 = new GPoint2D(cell.Bounds.Right - AllocationRect.Width, cell.Bounds.Top);
			point2 = new GPoint2D(point1.X, cell.Bounds.Bottom - AllocationRect.Height);
		}
		protected override bool IsEndPosition(int position) {
			return position < 0 || !GRect2D.IsIntersected(Layout.ValidRect, Layout.GetCell(StartRowIndex, position).Bounds);
		}
	}
	public class StepToRight : HorizontalStep {
		protected override int Increment { get { return 1; } }
		public StepToRight(int position, int startRowIndex, GRect2D rect, RectanglesLayout layout)
			: base(position, startRowIndex, rect, layout) {
		}
		protected override GPoint2D CalcNearPosition(Cell cell) {
			return new GPoint2D(cell.Bounds.Left, AllocationRect.Top);
		}
		protected override void CalcFarPositions(Cell cell, out GPoint2D point1, out GPoint2D point2) {
			point1 = new GPoint2D(cell.Bounds.Left, cell.Bounds.Top);
			point2 = new GPoint2D(cell.Bounds.Left, cell.Bounds.Bottom - AllocationRect.Height);
		}
		protected override bool IsEndPosition(int position) {
			return position >= Layout.ColumnsCount || !GRect2D.IsIntersected(Layout.ValidRect, Layout.GetCell(StartRowIndex, position).Bounds);
		}
	}
	public abstract class VerticalStep : Step {
		readonly int startColumnIndex;
		protected int StartColumnIndex { get { return startColumnIndex; } }
		public VerticalStep(int position, int startColumnIndex, GRect2D rect, RectanglesLayout layout)
			: base(position, rect, layout) {
			this.startColumnIndex = startColumnIndex;
		}
		protected abstract GPoint2D CalcNearPosition(Cell cell);
		protected abstract void CalcFarPositions(Cell cell, out GPoint2D point1, out GPoint2D point2);
		protected override void UpdatePrimaryTestPositions() {
			Cell cell = Layout.GetCell(Position, startColumnIndex);
			if (cell.IsEmpty)
				AddPrimaryTestPosition(CalcNearPosition(cell));
			for (int i = Layout.StepToLeft.Position; i <= Layout.StepToRight.Position; i++) {
				cell = Layout.GetCell(Position, i);
				if (cell.IsEmpty) {
					GPoint2D point1, point2;
					CalcFarPositions(cell, out point1, out point2);
					AddPrimaryTestPosition(point1);
					AddPrimaryTestPosition(point2);
				}
			}
		}
		protected override void UpdateSecondaryTestPositions() {
			for (int i = Position + Increment; !IsEndPosition(i); i += Increment) {
				Cell cell = Layout.GetCell(i, startColumnIndex);
				GPoint2D point = CalcNearPosition(cell);
				double distance = CalcDistanceToAllocationRect(point);
				if (IsUpdateEnd(distance))
					break;
				if (cell.IsEmpty)
					AddSecondaryTestPosition(point);
				for (int j = Layout.StepToLeft.Position; j <= Layout.StepToRight.Position; j++) {
					cell = Layout.GetCell(i, j);
					if (cell.IsEmpty) {
						GPoint2D point1, point2;
						CalcFarPositions(cell, out point1, out point2);
						AddSecondaryTestPosition(point1);
						AddSecondaryTestPosition(point2);
					}
				}
			}
		}
	}
	public class StepToBottom : VerticalStep {
		protected override int Increment { get { return -1; } }
		public StepToBottom(int position, int startColumnIndex, GRect2D rect, RectanglesLayout layout)
			: base(position, startColumnIndex, rect, layout) {
		}
		protected override GPoint2D CalcNearPosition(Cell cell) {
			return new GPoint2D(AllocationRect.Left, cell.Bounds.Bottom - AllocationRect.Height);
		}
		protected override void CalcFarPositions(Cell cell, out GPoint2D point1, out GPoint2D point2) {
			point1 = new GPoint2D(cell.Bounds.Left, cell.Bounds.Bottom - AllocationRect.Height);
			point2 = new GPoint2D(cell.Bounds.Right - AllocationRect.Width, point1.Y);
		}
		protected override bool IsEndPosition(int position) {
			return position < 0 || !GRect2D.IsIntersected(Layout.ValidRect, Layout.GetCell(position, StartColumnIndex).Bounds);				
		}
	}
	public class StepToTop : VerticalStep {
		protected override int Increment { get { return 1; } }
		public StepToTop(int position, int startColumnIndex, GRect2D rect, RectanglesLayout layout)
			: base(position, startColumnIndex, rect, layout) {
		}
		protected override GPoint2D CalcNearPosition(Cell cell) {
			return new GPoint2D(AllocationRect.Left, cell.Bounds.Top);
		}
		protected override void CalcFarPositions(Cell cell, out GPoint2D point1, out GPoint2D point2) {
			point1 = new GPoint2D(cell.Bounds.Left, cell.Bounds.Top);
			point2 = new GPoint2D(cell.Bounds.Right - AllocationRect.Width, cell.Bounds.Top);
		}
		protected override bool IsEndPosition(int position) {
			return position >= Layout.RowsCount || !GRect2D.IsIntersected(Layout.ValidRect, Layout.GetCell(position, StartColumnIndex).Bounds);			
		}
	}
}
