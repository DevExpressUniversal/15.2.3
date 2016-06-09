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
using System.Text;
namespace DevExpress.Xpf.Map.Native {
	public class IndexRect {
		Rectangle rect;
		int index;
		public int Index { get { return index; } }
		public Rectangle Rect { get { return rect; } }
		public IndexRect(Rectangle rect, int indx) {
			this.rect = rect;
			this.index = indx;
		}
	}
	public class ViewPortRectangleCalculator {
		#region Comparers
		class IndexRectLeftComparer : Comparer<IndexRect> {
			public override int Compare(IndexRect rect1, IndexRect rect2) {
				return rect1.Rect.X.CompareTo(rect2.Rect.X);
			}
		}
		class IndexRectRightComparer : Comparer<IndexRect> {
			public override int Compare(IndexRect rect1, IndexRect rect2) {
				return (rect1.Rect.Right).CompareTo(rect2.Rect.Right);
			}
		}
		class IndexRectTopComparer : Comparer<IndexRect> {
			public override int Compare(IndexRect rect1, IndexRect rect2) {
				return rect1.Rect.Y.CompareTo(rect2.Rect.Y);
			}
		}
		class IndexRectBottomComparer : Comparer<IndexRect> {
			public override int Compare(IndexRect rect1, IndexRect rect2) {
				return (rect1.Rect.Bottom).CompareTo(rect2.Rect.Bottom);
			}
		}
		#endregion
		RectangleLayout[] leftIndeces;
		RectangleLayout[] rightIndeces;
		RectangleLayout[] topIndeces;
		RectangleLayout[] bottomIndeces;
		public RectangleLayout[] LeftIndeces { get { return leftIndeces; } }
		public RectangleLayout[] RightIndeces { get { return rightIndeces; } }
		public RectangleLayout[] TopIndeces { get { return topIndeces; } }
		public RectangleLayout[] BottomIndeces { get { return bottomIndeces; } }
		public ViewPortRectangleCalculator() {
			leftIndeces = new RectangleLayout[0];
			rightIndeces = new RectangleLayout[0];
			topIndeces = new RectangleLayout[0];
			bottomIndeces = new RectangleLayout[0];
		}
		void FillList1(List<IndexRect> list, RectangleLayout[] inpRects) {
			list.Sort(new IndexRectLeftComparer());
			for (int i = 0; i < leftIndeces.Length; i++) {
				inpRects[list[i].Index].LayoutIndeces[0] = i;
				leftIndeces[i] = new RectangleLayout(list[i].Rect.X, list[i].Rect.Y, list[i].Rect.Right, list[i].Rect.Bottom, new int[5] { i, 0, 0, 0, list[i].Index });
			}
		}
		void FillList2(List<IndexRect> list, RectangleLayout[] inpRects) {
			list.Sort(new IndexRectRightComparer());
			for (int i = 0; i < rightIndeces.Length; i++) {
				inpRects[list[i].Index].LayoutIndeces[1] = i;
				rightIndeces[i] = new RectangleLayout(list[i].Rect.X, list[i].Rect.Y, list[i].Rect.Right, list[i].Rect.Bottom, new int[5] { inpRects[list[i].Index].LayoutIndeces[0], i, 0, 0, list[i].Index });
			}
		}
		void FillList3(List<IndexRect> list, RectangleLayout[] inpRects) {
			list.Sort(new IndexRectTopComparer());
			for (int i = 0; i < topIndeces.Length; i++) {
				inpRects[list[i].Index].LayoutIndeces[2] = i;
				topIndeces[i] = new RectangleLayout(list[i].Rect.X, list[i].Rect.Y, list[i].Rect.Right, list[i].Rect.Bottom, new int[5] { inpRects[list[i].Index].LayoutIndeces[0], inpRects[list[i].Index].LayoutIndeces[1], i, 0, list[i].Index });
			}
		}
		void FillList4(List<IndexRect> list, RectangleLayout[] inpRects) {
			list.Sort(new IndexRectBottomComparer());
			for (int i = 0; i < bottomIndeces.Length; i++) {
				inpRects[list[i].Index].LayoutIndeces[3] = i;
				bottomIndeces[i] = new RectangleLayout(list[i].Rect.X, list[i].Rect.Y, list[i].Rect.Right, list[i].Rect.Bottom, new int[5] { inpRects[list[i].Index].LayoutIndeces[0], inpRects[list[i].Index].LayoutIndeces[1], inpRects[list[i].Index].LayoutIndeces[2], i, list[i].Index });
			}
		}
		void FinalizeFillList1(List<IndexRect> list, RectangleLayout[] inpRects) {
			for (int i = 0; i < leftIndeces.Length; i++) {
				leftIndeces[i].LayoutIndeces[1] = inpRects[list[i].Index].LayoutIndeces[1];
				leftIndeces[i].LayoutIndeces[2] = inpRects[list[i].Index].LayoutIndeces[2];
				leftIndeces[i].LayoutIndeces[3] = inpRects[list[i].Index].LayoutIndeces[3];
			}
		}
		void FinalizeFillList2(List<IndexRect> list, RectangleLayout[] inpRects) {
			for (int i = 0; i < rightIndeces.Length; i++) {
				rightIndeces[i].LayoutIndeces[2] = inpRects[list[i].Index].LayoutIndeces[2];
				rightIndeces[i].LayoutIndeces[3] = inpRects[list[i].Index].LayoutIndeces[3];
			}
		}
		void FinalizeFillList3(List<IndexRect> list, RectangleLayout[] inpRects) {
			for (int i = 0; i < topIndeces.Length; i++)
				topIndeces[i].LayoutIndeces[3] = inpRects[list[i].Index].LayoutIndeces[3];
		}
		public void ArrangeRectangles(Rectangle[] inputData) {
			leftIndeces = new RectangleLayout[inputData.Length];
			rightIndeces = new RectangleLayout[inputData.Length];
			topIndeces = new RectangleLayout[inputData.Length];
			bottomIndeces = new RectangleLayout[inputData.Length];
			RectangleLayout[] inpRects = new RectangleLayout[inputData.Length];
			IndexRect[] rectsWithIdx = new IndexRect[inputData.Length];
			for (int i = 0; i < inpRects.Length; i++) {
				inpRects[i] = new RectangleLayout(inputData[i]);
				rectsWithIdx[i] = new IndexRect(inputData[i], i);
			}
			List<IndexRect> l = new List<IndexRect>(rectsWithIdx);
			List<IndexRect> lRes1 = new List<IndexRect>(l);
			FillList1(lRes1, inpRects);
			List<IndexRect> lRes2 = new List<IndexRect>(l);
			FillList2(lRes2, inpRects);
			List<IndexRect> lRes3 = new List<IndexRect>(l);
			FillList3(lRes3, inpRects);
			List<IndexRect> lRes4 = new List<IndexRect>(l);
			FillList4(lRes4, inpRects);
			FinalizeFillList1(lRes1, inpRects);
			FinalizeFillList2(lRes2, inpRects);
			FinalizeFillList3(lRes3, inpRects);
		}
		int FindIndex(int[] array, int coordinate) {
			int result = Array.BinarySearch(array, coordinate);
			if (result < 0) result = ~result;
			return result;
		}
		int CalcPosition(RectangleLayout[] list, int coordinate, Func<RectangleLayout, int> func) {
			int[] range = new int[list.Length];
			for (int i = 0; i < list.Length; i++)
				range[i] = func(list[i]);
			return FindIndex(range, coordinate);
		}
		int MinimalWithIdx(out int index, params int[] values) {
			if (values.Length <= 0)
				throw new ArgumentException("Error in number of arguments");
			int res = values[0];
			index = 0;
			for (int i = 0; i < values.Length; i++)
				if (values[i] < res) {
					index = i;
					res = values[i];
				}
			return res;
		}
		List<Rectangle> Verify(int kIndex, int kValue, Rectangle viewPort) {
			List<Rectangle> list = new List<Rectangle>();
			switch (kIndex) {
				case 0:
					for (int i = 0; i < kValue; i++)
						if ((rightIndeces[leftIndeces[i].LayoutIndeces[1]].Right > viewPort.X) && (topIndeces[leftIndeces[i].LayoutIndeces[2]].Top < viewPort.Bottom) && (bottomIndeces[leftIndeces[i].LayoutIndeces[3]].Bottom > viewPort.Y))
							list.Add(new Rectangle(leftIndeces[i].Left, leftIndeces[i].Top, leftIndeces[i].Right - leftIndeces[i].Left, leftIndeces[i].Bottom - leftIndeces[i].Top));
					break;
				case 1:
					for (int i = rightIndeces.Length - kValue; i < rightIndeces.Length; i++)
						if ((leftIndeces[rightIndeces[i].LayoutIndeces[0]].Left < viewPort.Right) && (topIndeces[rightIndeces[i].LayoutIndeces[2]].Top < viewPort.Bottom) && (bottomIndeces[rightIndeces[i].LayoutIndeces[3]].Bottom > viewPort.Y))
							list.Add(new Rectangle(rightIndeces[i].Left, rightIndeces[i].Top, rightIndeces[i].Right - rightIndeces[i].Left, rightIndeces[i].Bottom - rightIndeces[i].Top));
					break;
				case 2:
					for (int i = 0; i < kValue; i++)
						if ((leftIndeces[topIndeces[i].LayoutIndeces[0]].Left < viewPort.Right) && (rightIndeces[topIndeces[i].LayoutIndeces[1]].Right > viewPort.X) && (bottomIndeces[topIndeces[i].LayoutIndeces[3]].Bottom > viewPort.Y))
							list.Add(new Rectangle(topIndeces[i].Left, topIndeces[i].Top, topIndeces[i].Right - topIndeces[i].Left, topIndeces[i].Bottom - topIndeces[i].Top));
					break;
				case 3:
					for (int i = bottomIndeces.Length - kValue; i < bottomIndeces.Length; i++)
						if ((leftIndeces[bottomIndeces[i].LayoutIndeces[0]].Left < viewPort.Right) && (rightIndeces[bottomIndeces[i].LayoutIndeces[1]].Right > viewPort.X) && (topIndeces[bottomIndeces[i].LayoutIndeces[2]].Top < viewPort.Bottom))
							list.Add(new Rectangle(bottomIndeces[i].Left, bottomIndeces[i].Top, bottomIndeces[i].Right - bottomIndeces[i].Left, bottomIndeces[i].Bottom - bottomIndeces[i].Top));
					break;
				default:
					throw new IndexOutOfRangeException("Wrong min index");
			}
			return list;
		}
		public Rectangle[] CalculateRectanglesInViewPort(Rectangle viewPort) {
			DateTime start = DateTime.Now;
			System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
			sw.Reset();
			sw.Start();
			int leftPos = CalcPosition(this.leftIndeces, viewPort.Right, (d) => d.Left);
			int rightPos = this.rightIndeces.Length - CalcPosition(this.rightIndeces, viewPort.Left, (d) => d.Right);
			int topPos = CalcPosition(this.topIndeces, viewPort.Bottom, (d) => d.Top);
			int bottomPos = this.bottomIndeces.Length - CalcPosition(this.bottomIndeces, viewPort.Top, (d) => d.Bottom);
			int minK;
			int min = MinimalWithIdx(out minK, leftPos, rightPos, topPos, bottomPos);
			List<Rectangle> list = Verify(minK, min, viewPort);
			Rectangle[] res = new Rectangle[list.Count];
			list.CopyTo(res);
			DateTime finish = DateTime.Now;
			sw.Stop();
			Console.WriteLine(finish.Millisecond - start.Millisecond + " ms " + sw.ElapsedMilliseconds.ToString());
			return res;
		}
	}
}
