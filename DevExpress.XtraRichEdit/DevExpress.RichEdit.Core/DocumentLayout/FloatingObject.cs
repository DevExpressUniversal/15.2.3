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
using System.Drawing;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Layout.Export;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Drawing.Drawing2D;
using System.Drawing.Drawing2D;
#if SL
using DevExpress.Xpf.Core.Native;
using DevExpress.Office.Drawing;
#else
using DevExpress.Office.Drawing;
#endif
namespace DevExpress.XtraRichEdit.Layout {
	#region ILayoutRectangularFloatingObject
	public interface ILayoutRectangularFloatingObject {
		bool CanPutTextAtLeft { get; }
		bool CanPutTextAtRight { get; }
		Rectangle Bounds { get; }
		int X { get; }
		int Y { get; }
	}
	#endregion
	#region ParagraphFramesLayout
	public class ParagraphFramesLayout {
		static readonly IComparer<ParagraphFrameBox> horizontalObjectComparer = new LayoutRectangularParagraphFrameXComparer();
		static readonly IComparer<ParagraphFrameBox> verticalObjectComparer = new LayoutRectangularParagraphFrameYComparer();
		readonly List<ParagraphFrameBox> items = new List<ParagraphFrameBox>();
		protected internal List<ParagraphFrameBox> Items { get { return items; } }
		public bool ContainsParagraph(Paragraph paragraph) {
			return ContainsParagraph(paragraph.Index);
		}
		public bool ContainsParagraph(ParagraphIndex index) {
			if (items.Count == 0)
				return false;
			ParagraphFrameBox lastItem = items[items.Count - 1];
			return lastItem.ParagraphIndex == index;
		}
		public ParagraphFrameBox GetFloatingObject(Paragraph paragraph) {
			if (items.Count == 0)
				return null;
			ParagraphIndex index = paragraph.Index;
			for (int i = 0; i < Items.Count; i++) {
				if (Items[i].ParagraphIndex == index)
					return Items[i];
			}
			return null;
		}
		public void AddParagraphFrameBox(ParagraphFrameBox paragraphFrameBox) {
			if (!items.Contains(paragraphFrameBox))
				Items.Add(paragraphFrameBox);
		}
		public IList<ParagraphFrameBox> GetObjectsInRectangle(Rectangle bounds) {
			bounds.Height = Math.Max(1, bounds.Height);
			List<ParagraphFrameBox> result = new List<ParagraphFrameBox>();
			GetObjectsInRectangle(Items, result, bounds);
			result.Sort(horizontalObjectComparer);
			return result;
		}
		void GetObjectsInRectangle(List<ParagraphFrameBox> where, List<ParagraphFrameBox> to, Rectangle bounds) {
			int count = where.Count;
			for (int i = 0; i < count; i++) {
				if (where[i].ExtendedBounds.IntersectsWith(bounds))
					to.Add(where[i]);
			}
		}
		public List<TextArea> CalculateTextAreas(IList<ParagraphFrameBox> items, Rectangle bounds) {
			List<ParagraphFrameBox> processedObjects = new List<ParagraphFrameBox>();
			TextAreaCollectionEx result = new TextAreaCollectionEx();
			int left = Math.Min(bounds.Left, bounds.Right);
			result.Add(new TextArea(left, Math.Max(left + 1, bounds.Right)));
			int count = items.Count;
			for (int i = 0; i < count; i++)
				ProcessParagraphFrame(items[i], processedObjects, result, bounds);
			result.Sort();
			return result.InnerList;
		}
		public void ProcessParagraphFrame(ParagraphFrameBox paragraphFrame, List<ParagraphFrameBox> processedObjects, TextAreaCollectionEx result, Rectangle initialBounds) {
			Rectangle bounds = paragraphFrame.ExtendedBounds;
			int leftMostX = FindLeftMostX(processedObjects, bounds.Left, initialBounds);
			int rightMostX = FindRightMostX(processedObjects, bounds.Right, initialBounds);
			processedObjects.Add(paragraphFrame);
			if (paragraphFrame.FrameProperties.Info.TextWrapType != ParagraphFrameTextWrapType.None)
				result.Remove(new TextArea(bounds.Left, bounds.Right));
			if (paragraphFrame.FrameProperties.Info.TextWrapType == ParagraphFrameTextWrapType.NotBeside)
				result.Remove(new TextArea(leftMostX, rightMostX));
		}
		int FindLeftMostX(List<ParagraphFrameBox> processedObjects, int initialX, Rectangle bounds) {
			int result = bounds.Left;
			int count = processedObjects.Count;
			for (int i = 0; i < count; i++) {
				int objectBoundsRight = processedObjects[i].ExtendedBounds.Right;
				if (objectBoundsRight < initialX && objectBoundsRight > result)
					result = objectBoundsRight;
			}
			return result;
		}
		int FindRightMostX(List<ParagraphFrameBox> processedObjects, int initialX, Rectangle bounds) {
			int result = bounds.Right;
			int count = processedObjects.Count;
			for (int i = 0; i < count; i++) {
				int objectBoundsLeft = processedObjects[i].ExtendedBounds.Left;
				if (objectBoundsLeft > initialX && objectBoundsLeft < result)
					result = objectBoundsLeft;
			}
			return result;
		}
		public void Clear() {
			Items.Clear();
		}
	}
	#endregion
	#region FloatingObjectsLayout
	public class FloatingObjectsLayout {
		static readonly IComparer<FloatingObjectBox> horizontalObjectComparer = new LayoutRectangularFloatingObjectXComparer();
		static readonly IComparer<FloatingObjectBox> verticalObjectComparer = new LayoutRectangularFloatingObjectYComparer();
		readonly List<FloatingObjectBox> items = new List<FloatingObjectBox>();
		readonly List<FloatingObjectBox> foregroundItems = new List<FloatingObjectBox>();
		readonly List<FloatingObjectBox> backgroundItems = new List<FloatingObjectBox>();
		readonly List<FloatingObjectAnchorRun> runs = new List<FloatingObjectAnchorRun>();
		readonly Dictionary<FloatingObjectAnchorRun, FloatingObjectBox> objectsTable = new Dictionary<FloatingObjectAnchorRun, FloatingObjectBox>();
		readonly Dictionary<FloatingObjectBox, FloatingObjectAnchorRun> objectToRunMapTable = new Dictionary<FloatingObjectBox, FloatingObjectAnchorRun>();
		protected internal List<FloatingObjectAnchorRun> Runs { get { return runs; } }
		protected internal List<FloatingObjectBox> Items { get { return items; } }
		protected internal List<FloatingObjectBox> ForegroundItems { get { return foregroundItems; } }
		protected internal List<FloatingObjectBox> BackgroundItems { get { return backgroundItems; } }
		public bool ContainsRun(FloatingObjectAnchorRun objectAnchorRun) {
			return runs.Contains(objectAnchorRun);
		}
		public FloatingObjectBox GetFloatingObject(FloatingObjectAnchorRun objectAnchorRun) {
			FloatingObjectBox result;
			if (objectsTable.TryGetValue(objectAnchorRun, out result))
				return result;
			else
				return null;
		}
		public void Add(FloatingObjectAnchorRun objectAnchorRun, FloatingObjectBox floatingObject) {
			if (runs.Contains(objectAnchorRun))
				return;
			runs.Add(objectAnchorRun);
			objectsTable.Add(objectAnchorRun, floatingObject);
			objectToRunMapTable.Add(floatingObject, objectAnchorRun);
			if (objectAnchorRun.FloatingObjectProperties.TextWrapType == FloatingObjectTextWrapType.None) {
				if (objectAnchorRun.FloatingObjectProperties.IsBehindDoc)
					backgroundItems.Add(floatingObject);
				else
					foregroundItems.Add(floatingObject);
			}
			else
				Add(floatingObject);
		}
		internal void Add(FloatingObjectBox floatingObject) {
			int index = items.BinarySearch(floatingObject, verticalObjectComparer);
			if (index >= 0) {
				items.Insert(index, floatingObject);
				return;
			}
			index = ~index;
			if (index >= items.Count)
				items.Add(floatingObject);
			else
				items.Insert(index, floatingObject);
		}
		public IList<FloatingObjectBox> GetObjectsInRectangle(Rectangle bounds) {
			bounds.Height = Math.Max(1, bounds.Height);
			List<FloatingObjectBox> result = new List<FloatingObjectBox>();
			GetObjectsInRectangle(Items, result, bounds);
			result.Sort(horizontalObjectComparer);
			return result;
		}
		public IList<FloatingObjectBox> GetAllObjectsInRectangle(Rectangle bounds) {
			bounds.Height = Math.Max(1, bounds.Height);
			List<FloatingObjectBox> result = new List<FloatingObjectBox>();
			GetObjectsInRectangle(Items, result, bounds);
			GetObjectsInRectangle(ForegroundItems, result, bounds);
			GetObjectsInRectangle(BackgroundItems, result, bounds);
			return result;
		}
		void GetObjectsInRectangle(List<FloatingObjectBox> where, List<FloatingObjectBox> to, Rectangle bounds) {
			int count = where.Count;
			for (int i = 0; i < count; i++) {
				if (where[i].ExtendedBounds.IntersectsWith(bounds))
					to.Add(where[i]);
			}
		}
		public List<TextArea> CalculateTextAreas(IList<FloatingObjectBox> items, Rectangle bounds) {
			List<FloatingObjectBox> processedObjects = new List<FloatingObjectBox>();
			TextAreaCollectionEx result = new TextAreaCollectionEx();
			int left = Math.Min(bounds.Left, bounds.Right);
			result.Add(new TextArea(left, Math.Max(left + 1, bounds.Right)));
			int count = items.Count;
			for (int i = 0; i < count; i++)
				ProcessFloatingObject(items[i], processedObjects, result, bounds);
			result.Sort();
			return result.InnerList;
		}
		public void ProcessFloatingObject(FloatingObjectBox floatingObject, List<FloatingObjectBox> processedObjects, TextAreaCollectionEx result, Rectangle initialBounds) {
			Rectangle bounds = floatingObject.ExtendedBounds;
			int leftMostX = FindLeftMostX(processedObjects, bounds.Left, initialBounds);
			int rightMostX = FindRightMostX(processedObjects, bounds.Right, initialBounds);
			processedObjects.Add(floatingObject);
			result.Remove(new TextArea(bounds.Left, bounds.Right));
			int leftSideWidth = bounds.Left - leftMostX;
			int rightSideWidth = rightMostX - bounds.Right;
			if (!floatingObject.CanPutTextAtLeft || (floatingObject.PutTextAtLargestSide && leftSideWidth <= rightSideWidth))
				result.Remove(new TextArea(leftMostX, bounds.Left));
			if (!floatingObject.CanPutTextAtRight || (floatingObject.PutTextAtLargestSide && rightSideWidth < leftSideWidth))
				result.Remove(new TextArea(bounds.Right, rightMostX));
		}
		int FindLeftMostX(List<FloatingObjectBox> processedObjects, int initialX, Rectangle bounds) {
			int result = bounds.Left;
			int count = processedObjects.Count;
			for (int i = 0; i < count; i++) {
				int objectBoundsRight = processedObjects[i].ExtendedBounds.Right;
				if (objectBoundsRight < initialX && objectBoundsRight > result)
					result = objectBoundsRight;
			}
			return result;
		}
		int FindRightMostX(List<FloatingObjectBox> processedObjects, int initialX, Rectangle bounds) {
			int result = bounds.Right;
			int count = processedObjects.Count;
			for (int i = 0; i < count; i++) {
				int objectBoundsLeft = processedObjects[i].ExtendedBounds.Left;
				if (objectBoundsLeft > initialX && objectBoundsLeft < result)
					result = objectBoundsLeft;
			}
			return result;
		}
		public void ClearFloatingObjects(RunIndex runIndex) {
			ClearFloatingObjects(runIndex, Items);
			ClearFloatingObjects(runIndex, BackgroundItems);
			ClearFloatingObjects(runIndex, ForegroundItems);
		}
		void ClearFloatingObjects(RunIndex runIndex, List<FloatingObjectBox> objects) {
			int count = objects.Count;
			for (int i = count - 1; i >= 0; i--) {
				FloatingObjectBox floatingObject = objects[i];
				if (runIndex <= floatingObject.StartPos.RunIndex) {
					objects.RemoveAt(i);
					floatingObject.LockPosition = false;
					FloatingObjectAnchorRun run = objectToRunMapTable[floatingObject];
					objectToRunMapTable.Remove(floatingObject);
					Runs.Remove(run);
					objectsTable.Remove(run);
				}
			}
		}
		public void Clear() {
			Items.Clear();
			BackgroundItems.Clear();
			ForegroundItems.Clear();
			Runs.Clear();
			objectsTable.Clear();
			objectToRunMapTable.Clear();
		}
		public IList<FloatingObjectBox> GetFloatingObjects(PieceTable pieceTable) {
			IList<FloatingObjectBox> result = new List<FloatingObjectBox>();
			GetFloatingObjects(result, pieceTable, Items);
			GetFloatingObjects(result, pieceTable, ForegroundItems);
			GetFloatingObjects(result, pieceTable, BackgroundItems);
			return result;
		}
		public void GetFloatingObjects(IList<FloatingObjectBox> where, PieceTable pieceTable, List<FloatingObjectBox> objects) {
			if (objects == null)
				return;
			int count = objects.Count;
			for (int i = 0; i < count; i++)
				if (Object.ReferenceEquals(objects[i].PieceTable, pieceTable))
					where.Add(objects[i]);
		}
		public void MoveFloatingObjectsVertically(int deltaY, PieceTable pieceTable) {
			IList<FloatingObjectBox> objects = GetFloatingObjects(pieceTable);
			int count = objects.Count;
			for (int i = 0; i < count; i++)
				objects[i].MoveVertically(deltaY);
		}
	}
	#endregion
	#region TextArea
	public struct TextArea {
		static readonly TextAreaStartComparer startComparer = new TextAreaStartComparer();
		public static TextAreaStartComparer StartComparer { get { return startComparer; } }
		public static TextArea Empty = new TextArea(0, 0);
		int start;
		int end;
		public TextArea(int start, int end) {
			this.start = start;
			this.end = end;
		}
		public int Start { get { return start; } set { start = value; } }
		public int End { get { return end; } set { end = value; } }
		public int Width { get { return End - Start; } }
		public bool IntersectsWith(TextArea interval) {
			return interval.End >= Start && interval.Start <= End;
		}
		public bool IntersectsWithExcludingBounds(TextArea interval) {
			return interval.End > Start && interval.Start < End;
		}
		public static TextArea Union(TextArea interval1, TextArea interval2) {
			int start = Math.Min(interval1.Start, interval2.Start);
			int end = Math.Max(interval1.End, interval2.End);
			return new TextArea(start, end);
		}
		public List<TextArea> Subtract(TextArea interval) {
			TextAreaCollectionEx result = new TextAreaCollectionEx();
			if (!IntersectsWithExcludingBounds(interval)) {
				result.Add(this);
				return result.InnerList;
			}
			if (interval.Contains(this))
				return result.InnerList;
			if (this.Contains(interval)) {
				if (Start != interval.Start)
					result.Add(new TextArea(Start, interval.Start));
				if (End != interval.End)
					result.Add(new TextArea(interval.End, End));
				return result.InnerList;
			}
			if (Start >= interval.Start) {
				if (End != interval.End)
					result.Add(new TextArea(interval.End, End));
			}
			else {
				if (Start != interval.Start)
					result.Add(new TextArea(Start, interval.Start));
			}
			return result.InnerList;
		}
		public bool Contains(TextArea interval) {
			return interval.Start >= Start && interval.End <= End;
		}
		public override string ToString() {
			return String.Format("[{0} - {1}]", Start, End);
		}
	}
	#endregion
	#region TextAreaStartComparer
	public class TextAreaStartComparer : IComparer<TextArea> {
		#region IComparer<TextArea> Members
		public int Compare(TextArea x, TextArea y) {
			return Comparer<int>.Default.Compare(x.Start, y.Start);
		}
		#endregion
	}
	#endregion
	#region TextAreaCollectionEx
	public class TextAreaCollectionEx {
		readonly List<TextArea> innerList = new List<TextArea>();
		protected internal List<TextArea> InnerList { get { return innerList; } }
		public int Count { get { return InnerList.Count; } }
		public TextArea this[int index] { get { return InnerList[index]; } }
		public void Add(TextArea interval) {
			AddCore(interval);
		}
		protected int AddCore(TextArea interval) {
			if (Contains(interval))
				return 0;
			List<TextArea> toRemove = new List<TextArea>();
			int count = Count;
			for (int i = 0; i < count; i++) {
				if (interval.IntersectsWith(this[i])) {
					interval = TextArea.Union(this[i], interval);
					toRemove.Add(InnerList[i]);
				}
			}
			RemoveCore(toRemove);
			if (interval.Width > 0)
				InnerList.Add(interval);
			return Count - 1;
		}
		void RemoveCore(List<TextArea> toRemove) {
			int count = toRemove.Count;
			for (int i = 0; i < count; i++)
				InnerList.Remove(toRemove[i]);
		}
		void AddCore(List<TextArea> toAdd) {
			int count = toAdd.Count;
			for (int i = 0; i < count; i++)
				InnerList.Add(toAdd[i]);
		}
		public bool Remove(TextArea interval) {
			int index = InnerList.IndexOf(interval);
			if (index >= 0) {
				InnerList.RemoveAt(index);
				return true;
			}
			List<TextArea> toRemove = new List<TextArea>();
			List<TextArea> toAdd = new List<TextArea>();
			int count = Count;
			for (int i = 0; i < count; i++) {
				if (interval.IntersectsWithExcludingBounds(this[i])) {
					toRemove.Add(this[i]);
					List<TextArea> subtractResult = this[i].Subtract(interval);
					int subtractResultCount = subtractResult.Count;
					for (int subtractResultIndex = 0; subtractResultIndex < subtractResultCount; subtractResultIndex++)
						toAdd.Add(subtractResult[subtractResultIndex]);
				}
			}
			RemoveCore(toRemove);
			AddCore(toAdd);
			return true;
		}
		public bool Contains(TextArea interval) {
			if (InnerList.Contains(interval))
				return true;
			int count = Count;
			for (int i = 0; i < count; i++)
				if (this[i].Contains(interval))
					return true;
			return false;
		}
		public void Sort() {
			InnerList.Sort(TextArea.StartComparer);
		}
	}
	#endregion
	#region LayoutRectangularParagraphFrameYComparer
	public class LayoutRectangularParagraphFrameYComparer : IComparer<ParagraphFrameBox> {
		#region IComparer<ParagraphFrameBox> Members
		public int Compare(ParagraphFrameBox x, ParagraphFrameBox y) {
			return Comparer<int>.Default.Compare(x.Y, y.Y);
		}
		#endregion
	}
	#endregion
	#region LayoutRectangularParagraphFrameXComparer
	public class LayoutRectangularParagraphFrameXComparer : IComparer<ParagraphFrameBox> {
		#region IComparer<ParagraphFrameBox> Members
		public int Compare(ParagraphFrameBox x, ParagraphFrameBox y) {
			return Comparer<int>.Default.Compare(x.X, y.X);
		}
		#endregion
	}
	#endregion
	#region LayoutRectangularFloatingObjectYComparer
	public class LayoutRectangularFloatingObjectYComparer : IComparer<FloatingObjectBox> {
		#region IComparer<FloatingObjectBox> Members
		public int Compare(FloatingObjectBox x, FloatingObjectBox y) {
			return Comparer<int>.Default.Compare(x.Y, y.Y);
		}
		#endregion
	}
	#endregion
	#region LayoutRectangularFloatingObjectXComparer
	public class LayoutRectangularFloatingObjectXComparer : IComparer<FloatingObjectBox> {
		#region IComparer<FloatingObjectBox> Members
		public int Compare(FloatingObjectBox x, FloatingObjectBox y) {
			return Comparer<int>.Default.Compare(x.X, y.X);
		}
		#endregion
	}
	#endregion
	#region LayoutRectangularFloatingObjectYComparable
	public class LayoutRectangularFloatingObjectYComparable : IComparable<FloatingObjectBox> {
		readonly int y;
		public LayoutRectangularFloatingObjectYComparable(int y) {
			this.y = y;
		}
		#region IComparable<FloatingObjectBox> Members
		public int CompareTo(FloatingObjectBox other) {
			return Comparer<int>.Default.Compare(other.Y, y);
		}
		#endregion
	}
	#endregion
	public class FloatingObjectLocation : IFloatingObjectLocation {
		int offsetX;
		int offsetY;
		FloatingObjectHorizontalPositionAlignment horizontalPositionAlignment;
		FloatingObjectVerticalPositionAlignment verticalPositionAlignment;
		FloatingObjectHorizontalPositionType horizontalPositionType;
		FloatingObjectVerticalPositionType verticalPositionType;
		FloatingObjectTextWrapType textWrapType;
		int actualWidth;
		int actualHeight;
		public int OffsetX { get { return offsetX; } set { offsetX = value; } }
		public int OffsetY { get { return offsetY; } set { offsetY = value; } }
		public FloatingObjectHorizontalPositionAlignment HorizontalPositionAlignment { get { return horizontalPositionAlignment; } set { horizontalPositionAlignment = value; } }
		public FloatingObjectVerticalPositionAlignment VerticalPositionAlignment { get { return verticalPositionAlignment; } set { verticalPositionAlignment = value; } }
		public FloatingObjectHorizontalPositionType HorizontalPositionType { get { return horizontalPositionType; } set { horizontalPositionType = value; } }
		public FloatingObjectVerticalPositionType VerticalPositionType { get { return verticalPositionType; } set { verticalPositionType = value; } }
		public FloatingObjectTextWrapType TextWrapType { get { return textWrapType; } set { textWrapType = value; } }
		public int ActualWidth { get { return actualWidth; } set { actualWidth = value; } }
		public int ActualHeight { get { return actualHeight; } set { actualHeight = value; } }
		public bool UseRelativeWidth { get; set; }
		public bool UseRelativeHeight { get; set; }
		public FloatingObjectRelativeWidth RelativeWidth { get; set; }
		public FloatingObjectRelativeHeight RelativeHeight { get; set; }
		public int PercentOffsetX { get; set; }
		public int PercentOffsetY { get; set; }
	}
	public class FloatingObjectTargetPlacementInfo {
		Rectangle pageBounds;
		Rectangle pageClientBounds;
		Rectangle columnBounds;
		Rectangle originalColumnBounds;
		int originX;
		int originY;
		public Rectangle PageBounds { get { return pageBounds; } set { pageBounds = value; } }
		public Rectangle PageClientBounds { get { return pageClientBounds; } set { pageClientBounds = value; } }
		public Rectangle ColumnBounds { get { return columnBounds; } set { columnBounds = value; originalColumnBounds = value; } }
		public Rectangle OriginalColumnBounds { get { return originalColumnBounds; } set { originalColumnBounds = value; } }
		public int OriginX { get { return originX; } set { originX = value; } }
		public int OriginY { get { return originY; } set { originY = value; } }
	}
	public class FloatingObjectHorizontalPositionCalculator {
		readonly DocumentModelUnitToLayoutUnitConverter unitConverter;
		public FloatingObjectHorizontalPositionCalculator(DocumentModelUnitToLayoutUnitConverter unitConverter) {
			this.unitConverter = unitConverter;
		}
		public virtual int CalculateAbsoluteFloatingObjectWidth(IFloatingObjectLocation location, FloatingObjectTargetPlacementInfo placementInfo) {
			if (!location.UseRelativeWidth || location.RelativeWidth.Width == 0)
				return location.ActualWidth;
			else
				return CalculateAbsoluteFloatingObjectWidthCore(location, placementInfo);
		}
		int CalculateAbsoluteFloatingObjectWidthCore(IFloatingObjectLocation location, FloatingObjectTargetPlacementInfo placementInfo) {
			FloatingObjectRelativeWidth relativeWidth = location.RelativeWidth;
			return (int)(relativeWidth.Width / 100000.0 * GetPercentBaseWidth(relativeWidth.From, placementInfo));
		}
		int GetPercentBaseWidth(FloatingObjectRelativeFromHorizontal from, FloatingObjectTargetPlacementInfo placementInfo) {
			switch (from) {
				case FloatingObjectRelativeFromHorizontal.Page:
					return unitConverter.ToModelUnits(placementInfo.PageBounds.Width);
				case FloatingObjectRelativeFromHorizontal.Margin:
					return unitConverter.ToModelUnits(placementInfo.PageClientBounds.Width);
				case FloatingObjectRelativeFromHorizontal.OutsideMargin:
				case FloatingObjectRelativeFromHorizontal.LeftMargin:
					return unitConverter.ToModelUnits(placementInfo.PageClientBounds.Left - placementInfo.PageBounds.Left);
				case FloatingObjectRelativeFromHorizontal.InsideMargin:
				case FloatingObjectRelativeFromHorizontal.RightMargin:
					return unitConverter.ToModelUnits(placementInfo.PageBounds.Right - placementInfo.PageBounds.Right);
				default:
					Exceptions.ThrowInternalException();
					return 0;
			}
		}
		int CalculateFloatingObjectOffsetPercentBase(FloatingObjectHorizontalPositionType type, FloatingObjectTargetPlacementInfo placementInfo) {
			switch (type) {
				case FloatingObjectHorizontalPositionType.Page:
					return unitConverter.ToModelUnits(placementInfo.PageBounds.Width);
				case FloatingObjectHorizontalPositionType.Margin:
					return unitConverter.ToModelUnits(placementInfo.PageClientBounds.Width);
				case FloatingObjectHorizontalPositionType.OutsideMargin:
				case FloatingObjectHorizontalPositionType.LeftMargin:
					return unitConverter.ToModelUnits(placementInfo.PageClientBounds.Left - placementInfo.PageBounds.Left);
				case FloatingObjectHorizontalPositionType.InsideMargin:
				case FloatingObjectHorizontalPositionType.RightMargin:
					return unitConverter.ToModelUnits(placementInfo.PageBounds.Right - placementInfo.PageBounds.Right);
				default:
					return unitConverter.ToModelUnits(placementInfo.PageClientBounds.Width);
			}
		}
		public virtual int CalculateAbsoluteFloatingObjectX(IFloatingObjectLocation location, FloatingObjectTargetPlacementInfo placementInfo, int actualWidth) {
			if (location.HorizontalPositionAlignment == FloatingObjectHorizontalPositionAlignment.None) {
				int offsetX = location.OffsetX;
				if (location.PercentOffsetX != 0) {
					int percentBaseWidth = CalculateFloatingObjectOffsetPercentBase(location.HorizontalPositionType, placementInfo);
					offsetX = (int)(location.PercentOffsetX / 100000.0 * percentBaseWidth);
				}
				int value = CalculateAbsoluteFloatingObjectXCore(location.HorizontalPositionType, offsetX, placementInfo);
				return value;
			}
			else {
				Rectangle alignBounds = CalculateAlignBounds(location, placementInfo);
				return CalculateAbsoluteFloatingObjectHorizontalAlignmentPosition(location.HorizontalPositionAlignment, alignBounds, actualWidth);
			}
		}
		public virtual int CalculateFloatingObjectOffsetX(FloatingObjectHorizontalPositionType horizontalPositionType, int x, FloatingObjectTargetPlacementInfo placementInfo) {
			switch (horizontalPositionType) {
				case FloatingObjectHorizontalPositionType.LeftMargin:
				case FloatingObjectHorizontalPositionType.InsideMargin:
				case FloatingObjectHorizontalPositionType.Page:
					return unitConverter.ToModelUnits(x - placementInfo.PageBounds.Left);
				case FloatingObjectHorizontalPositionType.Column:
					return unitConverter.ToModelUnits(x - placementInfo.ColumnBounds.Left);
				case FloatingObjectHorizontalPositionType.Margin:
					return unitConverter.ToModelUnits(x - placementInfo.PageClientBounds.Left);
				case FloatingObjectHorizontalPositionType.OutsideMargin:
				case FloatingObjectHorizontalPositionType.RightMargin:
					return unitConverter.ToModelUnits(x - placementInfo.PageClientBounds.Right);
				case FloatingObjectHorizontalPositionType.Character:
					return unitConverter.ToModelUnits(x - placementInfo.OriginX);
				default:
					Exceptions.ThrowInternalException();
					return 0;
			}
		}
		protected internal Rectangle CalculateAlignBounds(IFloatingObjectLocation location, FloatingObjectTargetPlacementInfo placementInfo) {
			switch (location.HorizontalPositionType) {
				case FloatingObjectHorizontalPositionType.LeftMargin:
				case FloatingObjectHorizontalPositionType.InsideMargin: {
						Rectangle pageBounds = placementInfo.PageBounds;
						return new Rectangle(pageBounds.Left, pageBounds.Top, placementInfo.PageClientBounds.Left - pageBounds.Left, pageBounds.Height);
					}
				case FloatingObjectHorizontalPositionType.Page:
					return placementInfo.PageBounds;
				case FloatingObjectHorizontalPositionType.Column:
					return placementInfo.OriginalColumnBounds;
				case FloatingObjectHorizontalPositionType.OutsideMargin:
				case FloatingObjectHorizontalPositionType.RightMargin: {
						Rectangle pageBounds = placementInfo.PageBounds;
						Rectangle pageClientBounds = placementInfo.PageClientBounds;
						return new Rectangle(pageClientBounds.Right, pageBounds.Top, pageBounds.Right - pageClientBounds.Right, pageBounds.Height);
					}
				case FloatingObjectHorizontalPositionType.Margin:
					return placementInfo.PageClientBounds;
				case FloatingObjectHorizontalPositionType.Character:
					return new Rectangle(placementInfo.OriginX, 0, 0, 0);
				default:
					Exceptions.ThrowInternalException();
					return Rectangle.Empty;
			}
		}
		internal int ValidateFloatingObjectX(int actualWidth, Rectangle pageBounds, int value) {
			int overflow = value + unitConverter.ToLayoutUnits(actualWidth) - pageBounds.Right;
			if (overflow > 0)
				value -= overflow;
			return Math.Max(pageBounds.Left, value);
		}
		int CalculateAbsoluteFloatingObjectXCore(FloatingObjectHorizontalPositionType horizontalPositionType, int offsetX, FloatingObjectTargetPlacementInfo placementInfo) {
			switch (horizontalPositionType) {
				case FloatingObjectHorizontalPositionType.LeftMargin:
				case FloatingObjectHorizontalPositionType.InsideMargin:
				case FloatingObjectHorizontalPositionType.Page:
					return unitConverter.ToLayoutUnits(offsetX) + placementInfo.PageBounds.Left;
				case FloatingObjectHorizontalPositionType.Column:
					return unitConverter.ToLayoutUnits(offsetX) + placementInfo.ColumnBounds.Left;
				case FloatingObjectHorizontalPositionType.Margin:
					return unitConverter.ToLayoutUnits(offsetX) + placementInfo.PageClientBounds.Left;
				case FloatingObjectHorizontalPositionType.OutsideMargin:
				case FloatingObjectHorizontalPositionType.RightMargin:
					return unitConverter.ToLayoutUnits(offsetX) + placementInfo.PageClientBounds.Right;
				case FloatingObjectHorizontalPositionType.Character:
					return unitConverter.ToLayoutUnits(offsetX) + placementInfo.OriginX;
				default:
					Exceptions.ThrowInternalException();
					return 0;
			}
		}
		protected internal virtual int CalculateAbsoluteFloatingObjectHorizontalAlignmentPosition(FloatingObjectHorizontalPositionAlignment alignment, Rectangle alignBounds, int actualWidth) {
			switch (alignment) {
				case FloatingObjectHorizontalPositionAlignment.Inside:
				case FloatingObjectHorizontalPositionAlignment.Left:
					return alignBounds.Left;
				case FloatingObjectHorizontalPositionAlignment.Outside:
				case FloatingObjectHorizontalPositionAlignment.Right:
					return alignBounds.Right - unitConverter.ToLayoutUnits(actualWidth);
				case FloatingObjectHorizontalPositionAlignment.Center:
					return (alignBounds.Right + alignBounds.Left) / 2 - unitConverter.ToLayoutUnits(actualWidth) / 2;
				default:
					Exceptions.ThrowInternalException();
					return 0;
			}
		}
	}
	public class FloatingObjectVerticalPositionCalculator {
		readonly DocumentModelUnitToLayoutUnitConverter unitConverter;
		public FloatingObjectVerticalPositionCalculator(DocumentModelUnitToLayoutUnitConverter unitConverter) {
			this.unitConverter = unitConverter;
		}
		public virtual int CalculateAbsoluteFloatingObjectHeight(IFloatingObjectLocation location, FloatingObjectTargetPlacementInfo placementInfo) {
			if (!location.UseRelativeHeight || location.RelativeHeight.Height == 0)
				return location.ActualHeight;
			else
				return CalculateAbsoluteFloatingObjectHeightCore(location, placementInfo);
		}
		int CalculateAbsoluteFloatingObjectHeightCore(IFloatingObjectLocation location, FloatingObjectTargetPlacementInfo placementInfo) {
			FloatingObjectRelativeHeight relativeHeight = location.RelativeHeight;
			return (int)(relativeHeight.Height / 100000.0 * GetPercentBaseHeight(relativeHeight.From, placementInfo));
		}
		int GetPercentBaseHeight(FloatingObjectRelativeFromVertical from, FloatingObjectTargetPlacementInfo placementInfo) {
			switch (from) {
				case FloatingObjectRelativeFromVertical.Page:
					return unitConverter.ToModelUnits(placementInfo.PageBounds.Height);
				case FloatingObjectRelativeFromVertical.Margin:
					return unitConverter.ToModelUnits(placementInfo.PageClientBounds.Height);
				case FloatingObjectRelativeFromVertical.OutsideMargin:
				case FloatingObjectRelativeFromVertical.InsideMargin:
				case FloatingObjectRelativeFromVertical.TopMargin:
					return unitConverter.ToModelUnits(placementInfo.PageClientBounds.Top - placementInfo.PageBounds.Top);
				case FloatingObjectRelativeFromVertical.BottomMargin:
					return unitConverter.ToModelUnits(placementInfo.PageBounds.Bottom - placementInfo.PageBounds.Bottom);
				default:
					Exceptions.ThrowInternalException();
					return 0;
			}
		}
		int CalculateFloatingObjectOffsetPercentBase(FloatingObjectVerticalPositionType type, FloatingObjectTargetPlacementInfo placementInfo) {
			switch (type) {
				case FloatingObjectVerticalPositionType.Page:
					return unitConverter.ToModelUnits(placementInfo.PageBounds.Height);
				case FloatingObjectVerticalPositionType.Margin:
					return unitConverter.ToModelUnits(placementInfo.PageClientBounds.Height);
				case FloatingObjectVerticalPositionType.OutsideMargin:
				case FloatingObjectVerticalPositionType.InsideMargin:
				case FloatingObjectVerticalPositionType.TopMargin:
					return unitConverter.ToModelUnits(placementInfo.PageClientBounds.Top - placementInfo.PageBounds.Top);
				case FloatingObjectVerticalPositionType.BottomMargin:
					return unitConverter.ToModelUnits(placementInfo.PageBounds.Bottom - placementInfo.PageBounds.Bottom);
				default:
					return unitConverter.ToModelUnits(placementInfo.PageClientBounds.Height);
			}
		}
		public virtual int CalculateAbsoluteFloatingObjectY(IFloatingObjectLocation location, FloatingObjectTargetPlacementInfo placementInfo, int actualHeight) {
			if (location.VerticalPositionAlignment == FloatingObjectVerticalPositionAlignment.None) {
				int offsetY = location.OffsetY;
				if (location.PercentOffsetY != 0) {
					int percentBaseWidth = CalculateFloatingObjectOffsetPercentBase(location.VerticalPositionType, placementInfo);
					offsetY = (int)(location.PercentOffsetY / 100000.0 * percentBaseWidth);
				}
				int value = CalculateAbsoluteFloatingObjectYCore(location.VerticalPositionType, offsetY, placementInfo);
				return value;
			}
			else {
				Rectangle alignBounds = CalculateAlignBounds(location, placementInfo);
				return CalculateAbsoluteFloatingObjectVerticalAlignmentPosition(location.VerticalPositionAlignment, alignBounds, actualHeight);
			}
		}
		protected virtual int ValidateY(int y, int actualHeight, Rectangle targetBounds) {
			int height = unitConverter.ToLayoutUnits(actualHeight);
			int bottom = y + height;
			if (bottom >= targetBounds.Bottom)
				y -= bottom - targetBounds.Bottom;
			if (y < targetBounds.Top)
				y = targetBounds.Top;
			return y;
		}
		public virtual int CalculateFloatingObjectOffsetY(FloatingObjectVerticalPositionType verticalPositionType, int y, FloatingObjectTargetPlacementInfo placementInfo) {
			switch (verticalPositionType) {
				case FloatingObjectVerticalPositionType.Paragraph:
				case FloatingObjectVerticalPositionType.Line:
					return unitConverter.ToModelUnits(y - placementInfo.OriginY);
				case FloatingObjectVerticalPositionType.Page:
				case FloatingObjectVerticalPositionType.OutsideMargin:
				case FloatingObjectVerticalPositionType.InsideMargin:
				case FloatingObjectVerticalPositionType.TopMargin:
					return unitConverter.ToModelUnits(y - placementInfo.PageBounds.Y);
				case FloatingObjectVerticalPositionType.BottomMargin:
					return unitConverter.ToModelUnits(y - placementInfo.PageBounds.Bottom);
				case FloatingObjectVerticalPositionType.Margin:
					return unitConverter.ToModelUnits(y - placementInfo.ColumnBounds.Y);
				default:
					Exceptions.ThrowInternalException();
					return 0;
			}
		}
		int CalculateAbsoluteFloatingObjectYCore(FloatingObjectVerticalPositionType type, int offsetY, FloatingObjectTargetPlacementInfo placementInfo) {
			switch (type) {
				case FloatingObjectVerticalPositionType.Paragraph:
				case FloatingObjectVerticalPositionType.Line:
					return placementInfo.OriginY + unitConverter.ToLayoutUnits(offsetY);
				case FloatingObjectVerticalPositionType.Page:
				case FloatingObjectVerticalPositionType.OutsideMargin:
				case FloatingObjectVerticalPositionType.InsideMargin:
				case FloatingObjectVerticalPositionType.TopMargin:
					return placementInfo.PageBounds.Y + unitConverter.ToLayoutUnits(offsetY);
				case FloatingObjectVerticalPositionType.BottomMargin:
					return placementInfo.PageBounds.Bottom + unitConverter.ToLayoutUnits(offsetY);
				case FloatingObjectVerticalPositionType.Margin:
					return placementInfo.ColumnBounds.Y + unitConverter.ToLayoutUnits(offsetY);
				default:
					Exceptions.ThrowInternalException();
					return 0;
			}
		}
		protected internal Rectangle CalculateAlignBounds(IFloatingObjectLocation location, FloatingObjectTargetPlacementInfo placementInfo) {
			switch (location.VerticalPositionType) {
				case FloatingObjectVerticalPositionType.TopMargin:
				case FloatingObjectVerticalPositionType.OutsideMargin:
				case FloatingObjectVerticalPositionType.InsideMargin: {
						Rectangle pageBounds = placementInfo.PageBounds;
						Rectangle pageClientBounds = placementInfo.PageClientBounds;
						return new Rectangle(pageBounds.Left, pageBounds.Top, pageBounds.Width, pageClientBounds.Top - pageBounds.Top);
					}
				case FloatingObjectVerticalPositionType.Page:
					return placementInfo.PageBounds;
				case FloatingObjectVerticalPositionType.BottomMargin: {
						Rectangle pageBounds = placementInfo.PageBounds;
						Rectangle pageClientBounds = placementInfo.PageClientBounds;
						return new Rectangle(pageBounds.Left, pageClientBounds.Bottom, pageBounds.Width, pageBounds.Bottom - pageClientBounds.Bottom);
					}
				case FloatingObjectVerticalPositionType.Margin:
					return placementInfo.PageClientBounds;
				case FloatingObjectVerticalPositionType.Line:
				case FloatingObjectVerticalPositionType.Paragraph:
					return new Rectangle(0, placementInfo.OriginY, 0, 0);
				default:
					Exceptions.ThrowInternalException();
					return Rectangle.Empty;
			}
		}
		protected internal virtual int CalculateAbsoluteFloatingObjectVerticalAlignmentPosition(FloatingObjectVerticalPositionAlignment alignment, Rectangle alignBounds, int actualHeight) {
			switch (alignment) {
				case FloatingObjectVerticalPositionAlignment.Inside:
				case FloatingObjectVerticalPositionAlignment.Top:
					return alignBounds.Top;
				case FloatingObjectVerticalPositionAlignment.Outside:
				case FloatingObjectVerticalPositionAlignment.Bottom:
					return alignBounds.Bottom - unitConverter.ToLayoutUnits(actualHeight);
				case FloatingObjectVerticalPositionAlignment.Center:
					return (alignBounds.Bottom + alignBounds.Top) / 2 - unitConverter.ToLayoutUnits(actualHeight) / 2;
				default:
					Exceptions.ThrowInternalException();
					return 0;
			}
		}
	}
	public class ParagraphFrameHorizontalPositionCalculator {
		readonly DocumentModelUnitToLayoutUnitConverter unitConverter;
		public ParagraphFrameHorizontalPositionCalculator(DocumentModelUnitToLayoutUnitConverter unitConverter) {
			this.unitConverter = unitConverter;
		}
		public virtual int CalculateAbsoluteFloatingObjectX(IParagraphFrameLocation location, FloatingObjectTargetPlacementInfo placementInfo, int actualWidth) {
			if (location.HorizontalPositionAlignment == ParagraphFrameHorizontalPositionAlignment.None) {
				return CalculateAbsoluteFloatingObjectXCore(location.HorizontalPositionType, location.X, placementInfo);
			}
			else {
				Rectangle alignBounds = CalculateAlignBounds(location, placementInfo);
				return CalculateAbsoluteFloatingObjectHorizontalAlignmentPosition(location.HorizontalPositionAlignment, alignBounds, actualWidth);
			}
		}
		public virtual int CalculateFloatingObjectOffsetX(ParagraphFrameHorizontalPositionType horizontalPositionType, int x, FloatingObjectTargetPlacementInfo placementInfo) {
			switch (horizontalPositionType) {
				case ParagraphFrameHorizontalPositionType.Page:
					return unitConverter.ToModelUnits(x - placementInfo.PageBounds.Left);
				case ParagraphFrameHorizontalPositionType.Column:
					return unitConverter.ToModelUnits(x - placementInfo.ColumnBounds.Left);
				case ParagraphFrameHorizontalPositionType.Margin:
					return unitConverter.ToModelUnits(x - placementInfo.PageClientBounds.Left);
				default:
					Exceptions.ThrowInternalException();
					return 0;
			}
		}
		protected internal Rectangle CalculateAlignBounds(IParagraphFrameLocation location, FloatingObjectTargetPlacementInfo placementInfo) {
			switch (location.HorizontalPositionType) {
				case ParagraphFrameHorizontalPositionType.Page:
					return placementInfo.PageBounds;
				case ParagraphFrameHorizontalPositionType.Column:
					return placementInfo.OriginalColumnBounds;
				case ParagraphFrameHorizontalPositionType.Margin:
					return placementInfo.PageClientBounds;
				default:
					Exceptions.ThrowInternalException();
					return Rectangle.Empty;
			}
		}
		internal int ValidateFloatingObjectX(int actualWidth, Rectangle pageBounds, int value) {
			int overflow = value + unitConverter.ToLayoutUnits(actualWidth) - pageBounds.Right;
			if (overflow > 0)
				value -= overflow;
			return Math.Max(pageBounds.Left, value);
		}
		int CalculateAbsoluteFloatingObjectXCore(ParagraphFrameHorizontalPositionType horizontalPositionType, int offsetX, FloatingObjectTargetPlacementInfo placementInfo) {
			switch (horizontalPositionType) {
				case ParagraphFrameHorizontalPositionType.Page:
					return unitConverter.ToLayoutUnits(offsetX) + placementInfo.PageBounds.Left;
				case ParagraphFrameHorizontalPositionType.Column:
					return unitConverter.ToLayoutUnits(offsetX) + placementInfo.ColumnBounds.Left;
				case ParagraphFrameHorizontalPositionType.Margin:
					return unitConverter.ToLayoutUnits(offsetX) + placementInfo.PageClientBounds.Left;
				default:
					Exceptions.ThrowInternalException();
					return 0;
			}
		}
		protected internal virtual int CalculateAbsoluteFloatingObjectHorizontalAlignmentPosition(ParagraphFrameHorizontalPositionAlignment alignment, Rectangle alignBounds, int actualWidth) {
			switch (alignment) {
				case ParagraphFrameHorizontalPositionAlignment.Outside:
				case ParagraphFrameHorizontalPositionAlignment.Left:
					return alignBounds.Left;
				case ParagraphFrameHorizontalPositionAlignment.Inside:
				case ParagraphFrameHorizontalPositionAlignment.Right:
					return alignBounds.Right - unitConverter.ToLayoutUnits(actualWidth);
				case ParagraphFrameHorizontalPositionAlignment.Center:
					return (alignBounds.Right + alignBounds.Left) / 2 - unitConverter.ToLayoutUnits(actualWidth) / 2;
				default:
					Exceptions.ThrowInternalException();
					return 0;
			}
		}
	}
	public class ParagraphFrameVerticalPositionCalculator {
		readonly DocumentModelUnitToLayoutUnitConverter unitConverter;
		public ParagraphFrameVerticalPositionCalculator(DocumentModelUnitToLayoutUnitConverter unitConverter) {
			this.unitConverter = unitConverter;
		}
		public virtual int CalculateAbsoluteFloatingObjectY(IParagraphFrameLocation location, FloatingObjectTargetPlacementInfo placementInfo, int actualHeight) {
			if (location.VerticalPositionAlignment == ParagraphFrameVerticalPositionAlignment.None) {
				return CalculateAbsoluteFloatingObjectYCore(location.VerticalPositionType, location.Y, placementInfo);
			}
			else {
				Rectangle alignBounds = CalculateAlignBounds(location, placementInfo);
				return CalculateAbsoluteParagraphFrameVerticalAlignmentPosition(location.VerticalPositionAlignment, alignBounds, actualHeight);
			}
		}
		protected virtual int ValidateY(int y, int actualHeight, Rectangle targetBounds) {
			int height = unitConverter.ToLayoutUnits(actualHeight);
			int bottom = y + height;
			if (bottom >= targetBounds.Bottom)
				y -= bottom - targetBounds.Bottom;
			if (y < targetBounds.Top)
				y = targetBounds.Top;
			return y;
		}
		public virtual int CalculateFloatingObjectOffsetY(ParagraphFrameVerticalPositionType verticalPositionType, int y, FloatingObjectTargetPlacementInfo placementInfo) {
			switch (verticalPositionType) {
				case ParagraphFrameVerticalPositionType.Paragraph:
					return unitConverter.ToModelUnits(y - placementInfo.OriginY);
				case ParagraphFrameVerticalPositionType.Page:
					return unitConverter.ToModelUnits(y - placementInfo.PageBounds.Y);
				case ParagraphFrameVerticalPositionType.Margin:
					return unitConverter.ToModelUnits(y - placementInfo.ColumnBounds.Y);
				default:
					Exceptions.ThrowInternalException();
					return 0;
			}
		}
		int CalculateAbsoluteFloatingObjectYCore(ParagraphFrameVerticalPositionType type, int offsetY, FloatingObjectTargetPlacementInfo placementInfo) {
			switch (type) {
				case ParagraphFrameVerticalPositionType.Paragraph:
					return placementInfo.OriginY + unitConverter.ToLayoutUnits(offsetY);
				case ParagraphFrameVerticalPositionType.Page:
					return placementInfo.PageBounds.Y + unitConverter.ToLayoutUnits(offsetY);
				case ParagraphFrameVerticalPositionType.Margin:
					return placementInfo.ColumnBounds.Y + unitConverter.ToLayoutUnits(offsetY);
				default:
					Exceptions.ThrowInternalException();
					return 0;
			}
		}
		protected internal Rectangle CalculateAlignBounds(IParagraphFrameLocation location, FloatingObjectTargetPlacementInfo placementInfo) {
			switch (location.VerticalPositionType) {
				case ParagraphFrameVerticalPositionType.Page:
					return placementInfo.PageBounds;
				case ParagraphFrameVerticalPositionType.Margin:
					return placementInfo.PageClientBounds;
				case ParagraphFrameVerticalPositionType.Paragraph:
					return new Rectangle(0, placementInfo.OriginY, 0, 0);
				default:
					Exceptions.ThrowInternalException();
					return Rectangle.Empty;
			}
		}
		protected internal virtual int CalculateAbsoluteParagraphFrameVerticalAlignmentPosition(ParagraphFrameVerticalPositionAlignment alignment, Rectangle alignBounds, int actualHeight) {
			switch (alignment) {
				case ParagraphFrameVerticalPositionAlignment.Inline:
				case ParagraphFrameVerticalPositionAlignment.Inside:
				case ParagraphFrameVerticalPositionAlignment.Top:
					return alignBounds.Top;
				case ParagraphFrameVerticalPositionAlignment.Outside:
				case ParagraphFrameVerticalPositionAlignment.Bottom:
					return alignBounds.Bottom - unitConverter.ToLayoutUnits(actualHeight);
				case ParagraphFrameVerticalPositionAlignment.Center:
					return (alignBounds.Bottom + alignBounds.Top) / 2 - unitConverter.ToLayoutUnits(actualHeight) / 2;
				default:
					Exceptions.ThrowInternalException();
					return 0;
			}
		}
	}
	#region FloatingObjectAnchorBox
	public class FloatingObjectAnchorBox : SinglePositionBox {
		Rectangle actualBounds;
		Rectangle shapeBounds;
		Rectangle contentBounds;
		Rectangle actualSizeBounds;
		FloatingObjectAnchorRun anchorRun;
		public override Box CreateBox() {
			return new FloatingObjectAnchorBox();
		}
		public override bool IsVisible { get { return true; } }
		public override bool IsNotWhiteSpaceBox { get { return true; } }
		public override bool IsLineBreak { get { return false; } }
		public Rectangle ActualBounds { get { return actualBounds; } set { actualBounds = value; } }
		public Rectangle ContentBounds { get { return contentBounds; } set { contentBounds = value; } }
		public Rectangle ShapeBounds { get { return shapeBounds; } set { shapeBounds = value; } }
		public override Rectangle ActualSizeBounds { get { return actualSizeBounds; } }
		internal void SetActualSizeBounds(Rectangle bounds) {
			this.actualSizeBounds = bounds;
		}
		public void IncreaseHeight(int delta) {
			this.Bounds = new Rectangle(this.Bounds.X, this.Bounds.Y, this.Bounds.Width, this.Bounds.Height + delta);
			this.actualBounds.Height += delta;
			this.actualSizeBounds.Height += delta;
			this.contentBounds.Height += delta;
			this.shapeBounds.Height += delta;
		}
		public override int CalcDescent(PieceTable pieceTable) {
			return 0;
		}
		public FloatingObjectAnchorRun GetFloatingObjectRun(PieceTable pieceTable) {
			if (this.anchorRun == null)
				this.anchorRun = (FloatingObjectAnchorRun)GetRun(pieceTable);
			return this.anchorRun;
		}
		internal void SetFloatingObjectRun(FloatingObjectAnchorRun anchorRun) {
			this.anchorRun = anchorRun;
		}
		public override int CalcAscentAndFree(PieceTable pieceTable) {
			return Bounds.Height;
		}
		public override int CalcBaseAscentAndFree(PieceTable pieceTable) {
			return CalcAscentAndFree(pieceTable);
		}
		public override int CalcBaseDescent(PieceTable pieceTable) {
			return CalcDescent(pieceTable);
		}
		public override void ExportTo(IDocumentLayoutExporter exporter) {
		}
		public override BoxHitTestManager CreateHitTestManager(IBoxHitTestCalculator calculator) {
			return null;
		}
	}
	#endregion
	#region FloatingObjectBox
	public class FloatingObjectBox : SinglePositionBox, ILayoutRectangularFloatingObject {
		#region Fields
		bool canPutTextAtLeft;
		bool canPutTextAtRight;
		bool putTextAtLargestSide;
		bool lockPosition;
		Rectangle extendedBounds;
		Rectangle contentBounds;
		Rectangle actualSizeBounds;
		PieceTable pieceTable;
		DocumentLayout documentLayout;
		#endregion
		public override Box CreateBox() {
			return new FloatingObjectBox();
		}
		#region Properties
		public override bool IsVisible { get { return true; } }
		public override bool IsNotWhiteSpaceBox { get { return true; } }
		public override bool IsLineBreak { get { return false; } }
		public bool CanPutTextAtLeft { get { return canPutTextAtLeft; } set { canPutTextAtLeft = value; } }
		public bool CanPutTextAtRight { get { return canPutTextAtRight; } set { canPutTextAtRight = value; } }
		public bool PutTextAtLargestSide { get { return putTextAtLargestSide; } set { putTextAtLargestSide = value; } }
		public Rectangle ExtendedBounds { get { return extendedBounds; } set { extendedBounds = value; } }
		public Rectangle ContentBounds { get { return contentBounds; } set { contentBounds = value; } }
		public override Rectangle ActualSizeBounds { get { return actualSizeBounds; } }
		public PieceTable PieceTable { get { return pieceTable; } set { pieceTable = value; } }
		public DocumentLayout DocumentLayout { get { return documentLayout; } set { documentLayout = value; } }
		Rectangle ILayoutRectangularFloatingObject.Bounds { get { return extendedBounds; } }
		public int X { get { return extendedBounds.X; } set { extendedBounds.X = value; } }
		public int Y { get { return extendedBounds.Y; } set { extendedBounds.Y = value; } }
		public bool LockPosition { get { return lockPosition; } set { lockPosition = value; } }
		public bool WasRestart { get; set; }
		#endregion
		internal void SetActualSizeBounds(Rectangle bounds) {
			this.actualSizeBounds = bounds;
		}
		public void IncreaseHeight(int delta) {
			this.Bounds = new Rectangle(this.Bounds.X, this.Bounds.Y, this.Bounds.Width, this.Bounds.Height + delta);
			this.actualSizeBounds.Height += delta;
			this.extendedBounds.Height += delta;
			this.contentBounds.Height += delta;
		}
		public override int CalcDescent(PieceTable pieceTable) {
			return 0;
		}
		public FloatingObjectAnchorRun GetFloatingObjectRun() {
			TextRunBase currentRun = GetRun(pieceTable);
			return (FloatingObjectAnchorRun)currentRun;
		}
		public override TextRunBase GetRun(PieceTable pieceTable) {
			return base.GetRun(this.PieceTable);
		}
		public override int CalcAscentAndFree(PieceTable pieceTable) {
			return Bounds.Height;
		}
		public override int CalcBaseAscentAndFree(PieceTable pieceTable) {
			return CalcAscentAndFree(this.PieceTable);
		}
		public override int CalcBaseDescent(PieceTable pieceTable) {
			return CalcDescent(this.PieceTable);
		}
		public override void ExportTo(IDocumentLayoutExporter exporter) {
			exporter.ExportFloatingObjectBox(this);
		}
		public override BoxHitTestManager CreateHitTestManager(IBoxHitTestCalculator calculator) {
			return null;
		}
		public Rectangle GetTextBoxContentBounds() {
			TextRunBase currentRun = GetRun(PieceTable);
			FloatingObjectAnchorRun run = GetFloatingObjectRun();
			TextBoxFloatingObjectContent content = run.Content as TextBoxFloatingObjectContent;
			if (content == null)
				return ContentBounds;
			DocumentModelUnitToLayoutUnitConverter unitConverter = this.pieceTable.DocumentModel.ToDocumentLayoutUnitConverter;
			Point location = ContentBounds.Location;
			TextBoxProperties textBoxProperties = content.TextBoxProperties;
			int leftMargin = unitConverter.ToLayoutUnits(textBoxProperties.LeftMargin);
			int topMargin = unitConverter.ToLayoutUnits(textBoxProperties.TopMargin);
			location.X += leftMargin;
			location.Y += topMargin;
			Size size = ContentBounds.Size;
			size.Width -= leftMargin + unitConverter.ToLayoutUnits(textBoxProperties.RightMargin);
			size.Height -= topMargin + unitConverter.ToLayoutUnits(textBoxProperties.BottomMargin);
			size.Width = Math.Max(size.Width, 50);
			size.Height = Math.Max(size.Height, 10);
			return new Rectangle(location, size);
		}
		public static Matrix CreateTransformUnsafe(float angle, Rectangle bounds) {
			if ((angle % 360f) == 0)
				return null;
			Matrix transform = new Matrix();
			transform.RotateAt(angle, RectangleUtils.CenterPoint(bounds));
			return transform;
		}
		public Matrix CreateBackwardTransformUnsafe() {
			return CreateTransformUnsafe(-PieceTable.DocumentModel.GetBoxEffectiveRotationAngleInDegrees(this), ActualSizeBounds);
		}
		public Matrix CreateTransformUnsafe() {
			return CreateTransformUnsafe(PieceTable.DocumentModel.GetBoxEffectiveRotationAngleInDegrees(this), ActualSizeBounds);
		}
		public Point TransformPointBackward(Point point) {
			Matrix transform = CreateBackwardTransformUnsafe();
			if (transform == null)
				return point;
			else
				return transform.TransformPoint(point);
		}
		public override void MoveVertically(int deltaY) {
			if (GetFloatingObjectRun().FloatingObjectProperties.VerticalPositionAlignment != FloatingObjectVerticalPositionAlignment.None)
				return;
			base.MoveVertically(deltaY);
			extendedBounds.Y += deltaY;
			contentBounds.Y += deltaY;
			actualSizeBounds.Y += deltaY;
			if (documentLayout != null && documentLayout.Pages.Count > 0)
				documentLayout.Pages[0].MoveVertically(deltaY);
		}
	}
	#endregion
	#region FloatingObjectBoxZOrderComparer
	public class FloatingObjectBoxZOrderComparer : IComparer<FloatingObjectBox> {
		public int Compare(FloatingObjectBox first, FloatingObjectBox second) {
			return Comparer<int>.Default.Compare(first.GetFloatingObjectRun().FloatingObjectProperties.ZOrder, second.GetFloatingObjectRun().FloatingObjectProperties.ZOrder);
		}
	}
	#endregion
	#region ParagraphFrameBoxIndexComparer
	public class ParagraphFrameBoxIndexComparer : IComparer<ParagraphFrameBox> {
		public int Compare(ParagraphFrameBox first, ParagraphFrameBox second) {
			return Comparer<ParagraphIndex>.Default.Compare(first.ParagraphIndex, second.ParagraphIndex);
		}
	}
	#endregion
	public class TextBoxFloatingObjectContentPrinter : SimpleDocumentPrinter {
		readonly ContentTypeBase textBoxContentType;
		readonly Page pageNumberSource;
		readonly BoxMeasurer measurer;
		public TextBoxFloatingObjectContentPrinter(ContentTypeBase textBoxContentType, Page pageNumberSource, BoxMeasurer measurer)
			: base(textBoxContentType.DocumentModel) {
			this.textBoxContentType = textBoxContentType;
			this.pageNumberSource = pageNumberSource;
			this.measurer = measurer;
		}
		public override PieceTable PieceTable { get { return textBoxContentType.PieceTable; } }
		protected override Page PageNumberSource { get { return pageNumberSource; } }
		public int Format(int maxHeight) {
			PieceTable oldPieceTable = this.measurer.PieceTable;
			this.measurer.PieceTable = PieceTable;
			try {
				return Format(null, maxHeight);
			}
			finally {
				this.measurer.PieceTable = oldPieceTable;
			}
		}
		protected internal override DocumentPrinterController CreateDocumentPrinterController() {
			return null;
		}
		protected internal override DocumentPrinterFormattingController CreateDocumentPrinterFormattingController(DocumentLayout documentLayout, PieceTable pieceTable) {
			return new TextBoxDocumentPrinterFormattingController(documentLayout, pieceTable);
		}
		protected internal override BoxMeasurer CreateMeasurer(Graphics gr) {
			return this.measurer;
		}
	}
	public class CommentContentPrinter : TextBoxFloatingObjectContentPrinter {
		public CommentContentPrinter(ContentTypeBase content, Page pageNumberSource, BoxMeasurer measurer)
			: base(content, pageNumberSource, measurer) {
		}
		public string CommentHeading { get; set; }
		public Rectangle CommentHeadingBounds { get; set; }
		public FontInfo CommentHeadingFontInfo { get; set; }
		public int CommentHeadingOffset { get; set; }
		protected internal override DocumentPrinterFormattingController CreateDocumentPrinterFormattingController(DocumentLayout documentLayout, PieceTable pieceTable) {
			return new CommentPrinterFormattingController(documentLayout, pieceTable);
		}
		protected internal override int PerformPrimaryFormat(int maxHeight) {
			int index = DocumentModel.FontCache.CalcFontIndex("Calibri", 20, true, false, CharacterFormattingScript.Normal, false, false);
			CommentHeadingFontInfo = DocumentModel.FontCache[index];
			Size sizeCommentHeading = DocumentLayout.Measurer.MeasureText(CommentHeading, CommentHeadingFontInfo);
			Paragraph paragraph = PieceTable.Paragraphs[ParagraphIndex.Zero];
			DocumentModelUnitToLayoutUnitConverter unitConverter = DocumentModel.ToDocumentLayoutUnitConverter;
			CommentHeadingOffset = unitConverter.ToLayoutUnits(paragraph.LeftIndent);
			CommentHeadingBounds = new Rectangle(new Point(0, 0), sizeCommentHeading);
			CommentRowsController controller = (CommentRowsController)Controller.RowsController;
			controller.FirstParagraphIndent = sizeCommentHeading.Width;
			return base.PerformPrimaryFormat(maxHeight);
		}
	}
}
