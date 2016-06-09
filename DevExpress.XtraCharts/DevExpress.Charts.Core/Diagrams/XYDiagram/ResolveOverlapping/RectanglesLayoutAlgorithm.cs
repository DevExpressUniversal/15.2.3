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
namespace DevExpress.Charts.Native {
	public class RectanglesLayout {
		const int stepToLeftIndex = 0;
		const int stepToRightIndex = 1;
		const int stepToBottomIndex = 2;
		const int stepToTopIndex = 3;
		readonly GRect2D bounds;
		readonly Rows rows;
		GRect2D validRect;
		Step[] steps = new Step[4];
		List<TestPosition> testContainer = new List<TestPosition>();
		List<GRect2D> occupiedRectList = new List<GRect2D>();
		public int RowsCount { get { return rows.Count; } }
		public int ColumnsCount { get { return rows.ColumnsCount; } }
		public IEnumerable<Step> Steps { get { return steps; } }
		public Step StepToLeft { get { return steps[stepToLeftIndex]; } }
		public Step StepToRight { get { return steps[stepToRightIndex]; } }
		public Step StepToBottom { get { return steps[stepToBottomIndex]; } }
		public Step StepToTop { get { return steps[stepToTopIndex]; } }
		public GRect2D ValidRect { get { return validRect; } }		
		public RectanglesLayout(GRect2D bounds) {
			this.bounds = bounds;
			rows = new Rows(bounds);
		}
		void Arrange(ref GRect2D rect) {
			UpdateSteps(rect);
			UpdateTestContainer();
			rect = RunAlgorithm(rect);
		}
		bool IsAlgorithmEnd() {
			foreach (Step step in steps) {
				if (!step.IsEnd)
					return false;
			}
			return true;
		}
		bool IsEmptyRegion(GRect2D rect) {
			if (!validRect.Contains(rect))
				return false;
			return rows.IsEmptyRegion(rect);
		}
		GRect2D RunAlgorithm(GRect2D allocationRect) {
			int index = 0;
			while (true) {
				if (IsAlgorithmEnd() && testContainer.Count == 0)
					return allocationRect;
				if (index >= testContainer.Count) {
					foreach (Step step in steps)
						step.Next();
					foreach (Step step in steps)
						step.Update();
					UpdateTestContainer();
					index = 0;
					continue;
				}
				TestPosition test = testContainer[index];
				GRect2D rect = new GRect2D(test.Position.X, test.Position.Y, allocationRect.Width, allocationRect.Height);
				if (IsEmptyRegion(rect))
					return rect;
				else {
					test.Check = true;
					bool updateFlag = false;
					foreach (Step step in steps) {
						if (step.IsTestingEnd() && !step.IsEnd) {
							step.Next();
							step.Update();
							updateFlag = true;
						}
					}
					if (updateFlag) {
						UpdateTestContainer();
						index = 0;
						continue;
					}
				}
				++index;
			}
		}
		void UpdateTestContainer() {
			testContainer = new List<TestPosition>(steps[0].TestPositions.Count + steps[1].TestPositions.Count +
				steps[2].TestPositions.Count + steps[3].TestPositions.Count);
			foreach (Step step in steps)
				testContainer.AddRange(step.TestPositions);
			testContainer.Sort(TestPosition.CompareByDistance);
		}
		public void UpdateSteps(GRect2D rect) {
			int rowIndex, columnIndex;
			rows.FindNearCell(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2, out rowIndex, out columnIndex);
			if (!GRect2D.IsIntersected(validRect, GetCell(rowIndex, columnIndex).Bounds))
				rows.FindNearCell(validRect.Left + validRect.Width / 2, validRect.Top + validRect.Height / 2, out rowIndex, out columnIndex);
			steps[stepToLeftIndex] = new StepToleft(columnIndex, rowIndex, rect, this);
			steps[stepToRightIndex] = new StepToRight(columnIndex, rowIndex, rect, this);
			steps[stepToBottomIndex] = new StepToBottom(rowIndex, columnIndex, rect, this);
			steps[stepToTopIndex] = new StepToTop(rowIndex, columnIndex, rect, this);
			foreach (Step step in steps)
				step.Update();
		}
		public void UpdateValidRect(GRect2D rect) {
			if (rect.IsEmpty)
				validRect = bounds;
			else {
				validRect = GRect2D.Intersect(bounds, rect);
				rows.SeparateByHorizontal(validRect.Top);
				rows.SeparateByHorizontal(validRect.Bottom);
				rows.SeparateByVertical(validRect.Left);
				rows.SeparateByVertical(validRect.Right);				  
			}
		}
		public GRect2D ArrangeRectangle(GRect2D rect, int overlappingIndent, GRect2D validRect, GRect2D tempExcludedRect) {
			if (rect.IsEmpty)
				return rect;
			rect.Inflate(overlappingIndent, overlappingIndent);
			UpdateValidRect(validRect);
			if (!tempExcludedRect.IsEmpty)
				rows.AddRectangle(tempExcludedRect);
			if (!IsEmptyRegion(rect))
				Arrange(ref rect);
			if (!tempExcludedRect.IsEmpty)
				rows.DeleteRectangle(tempExcludedRect);
			rect.Inflate(-overlappingIndent, -overlappingIndent);
			AddOccupiedRectangle(rect);			
			return rect;
		}
		public void AddOccupiedRectangle(GRect2D rect) {
			occupiedRectList.Add(rect);
			rows.AddRectangle(rect);
		}
		public bool IsEmptyRegion(GRect2D rect, GRect2D validRect) {
			UpdateValidRect(validRect);
			return IsEmptyRegion(rect);
		}
		public bool IsEmptyRegionByList(GRect2D rect) {
			foreach (GRect2D item in occupiedRectList) 
				if (GRect2D.IsIntersected(rect, item))
					return false;			
			return true;
		}
		public Cell GetCell(int rowIndex, int columnIndex) {
			return rows.GetCell(rowIndex, columnIndex);
		}
	}
	public class RectanglesLayoutAlgorithm {
		readonly RectanglesLayout layout;
		readonly RectanglesLayout layoutWithExcludedRegions;
		readonly int overlappingIndent;
		public RectanglesLayoutAlgorithm(GRect2D bounds, int overlappingIndet) {
			if (bounds.IsEmpty)
				bounds = new GRect2D(Int16.MinValue / 2, Int16.MinValue / 2, Int16.MaxValue, Int16.MaxValue);
			else
				bounds.Inflate(overlappingIndet, overlappingIndet);
			layout = new RectanglesLayout(bounds);
			layoutWithExcludedRegions = new RectanglesLayout(bounds);
			this.overlappingIndent = overlappingIndet;
		}
		public RectanglesLayoutAlgorithm(int overlappingIndet) : this(GRect2D.Empty, overlappingIndet) {
		}
		public GRect2D ArrangeRectangle(GRect2D rect, GRect2D validRect, bool useExcludedRegions) {
			return ArrangeRectangle(rect, validRect, useExcludedRegions, GRect2D.Empty);
		}
		public GRect2D ArrangeRectangle(GRect2D rect, GRect2D validRect, bool useExcludedRegions, GRect2D tempExcludedRect) {
			if (!validRect.IsEmpty)
				validRect.Inflate(rect.Width / 2 + overlappingIndent, rect.Height / 2 + overlappingIndent);			
			if (!tempExcludedRect.IsEmpty)
				tempExcludedRect.Inflate(-overlappingIndent, -overlappingIndent);
			if (useExcludedRegions) {
				rect = layoutWithExcludedRegions.ArrangeRectangle(rect, overlappingIndent, validRect, tempExcludedRect);
				layout.AddOccupiedRectangle(rect);
			}
			else {
				rect = layout.ArrangeRectangle(rect, overlappingIndent, validRect, tempExcludedRect);
				layoutWithExcludedRegions.AddOccupiedRectangle(rect);
			}
			return rect;
		}
		public void AddExcludedRectangle(GRect2D rect) {
			rect.Inflate(-overlappingIndent, -overlappingIndent);
			layoutWithExcludedRegions.AddOccupiedRectangle(rect);
		}
		public void AddOccupiedRectangle(GRect2D rect) {
			layoutWithExcludedRegions.AddOccupiedRectangle(rect);
			layout.AddOccupiedRectangle(rect);
		}
		public bool IsEmptyRegion(GRect2D rect, bool useExcludedRegions) {
			rect.Inflate(overlappingIndent, overlappingIndent);
			if (useExcludedRegions)
				return layoutWithExcludedRegions.IsEmptyRegion(rect, GRect2D.Empty);
			else
				return layout.IsEmptyRegion(rect, GRect2D.Empty);
		}
		public bool IsEmptyRegionByList(GRect2D rect, bool useExcludedRegions) {
			rect.Inflate(overlappingIndent, overlappingIndent);
			if (useExcludedRegions)
				return layoutWithExcludedRegions.IsEmptyRegionByList(rect);
			else
				return layout.IsEmptyRegionByList(rect);
		}		
	}
}
