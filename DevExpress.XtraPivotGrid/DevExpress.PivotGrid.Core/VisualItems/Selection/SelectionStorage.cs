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

using DevExpress.Compatibility.System.Drawing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
namespace DevExpress.XtraPivotGrid.Selection {
	public class SelectionStorage {
		List<RegionSelection> store = new List<RegionSelection>();
		int count = 0;
		abstract internal class RegionSelection : IEnumerable<Point> {
			public static RegionSelection Create(Rectangle rect) {
				if(rect.Height == 0 || rect.Width == 0 || rect.IsEmpty)
					return null;
				if(rect.Height == 1 && rect.Width == 1)
					return new PointSelection(rect.Location);
				return new RectangleSelection(rect);
			}
			public static RegionSelection Create(int x, int y, int width, int height) {
				if(width <= 0 || height <= 0)
					return null;
				if(width == 1 && height == 1)
					return new PointSelection(x, y);
				return new RectangleSelection(x, y, width, height);
			}
			public abstract int Count { get; }
			public abstract int MinX { get; }
			public abstract int MinY { get; }
			public abstract int MaxX { get; }
			public abstract int MaxY { get; }
			protected abstract IEnumerator<Point> GetEnumerator();
			public abstract bool Contains(Point point);
			public abstract IEnumerable<RegionSelection> Remove(RegionSelection substraction);
			public abstract IEnumerable<RegionSelection> RemoveCore(Point point);
			public abstract IEnumerable<RegionSelection> Correct(Rectangle rect);
			public abstract Point GetAt(int index);
			public abstract void FillRows(Dictionary<int, int> dic);
			public abstract void FillColumns(Dictionary<int, int> dic);
			public IEnumerable<RegionSelection> Remove(Point point) {
				if(!Contains(point))
					return null;
				return RemoveCore(point);
			}
			IEnumerator<Point> IEnumerable<Point>.GetEnumerator() {
				return GetEnumerator();
			}
			IEnumerator IEnumerable.GetEnumerator() {
				return GetEnumerator();
			}
		}
		class PointSelection : RegionSelection {
			readonly Point p;
			public override int Count { get { return 1; } }
			public override int MinX { get { return p.X; } }
			public override int MinY { get { return p.Y; } }
			public override int MaxX { get { return p.X; } }
			public override int MaxY { get { return p.Y; } }
			public PointSelection(Point point) {
				p = point;
			}
			public PointSelection(int x, int y) : this(new Point(x, y)) { }
			public override bool Contains(Point point) {
				return p == point;
			}
			public override IEnumerable<RegionSelection> RemoveCore(Point point) {
				return Enumerable.Empty<RegionSelection>();
			}
			public override IEnumerable<RegionSelection> Remove(RegionSelection substraction) {
				if(!substraction.Contains(p))
					return null;
				return Enumerable.Empty<RegionSelection>();
			}
			public override IEnumerable<RegionSelection> Correct(Rectangle rect) {
				if(rect.Contains(p))
					return null;
				return Enumerable.Empty<RegionSelection>();
			}
			public override Point GetAt(int index) {
				return p;
			}
			protected override IEnumerator<Point> GetEnumerator() {
				yield return p;
			}
			public override void FillRows(Dictionary<int, int> dic) {
				int c;
				if(!dic.TryGetValue(p.Y, out c))
					dic.Add(p.Y, 1);
				else
					dic[p.Y] = c++;
			}
			public override void FillColumns(Dictionary<int, int> dic) {
				int c;
				if(!dic.TryGetValue(p.X, out c))
					dic.Add(p.X, 1);
				else
					dic[p.X] = c + 1;
			}
		}
		class RectangleSelection : RegionSelection {
			readonly Rectangle rect;
			public override int Count { get { return rect.Width * rect.Height; } }
			public override int MinX { get { return rect.Left; } }
			public override int MinY { get { return rect.Top; } }
			public override int MaxX { get { return rect.Right - 1; } }
			public override int MaxY { get { return rect.Bottom - 1; } }
			public RectangleSelection(Rectangle rect) {
				this.rect = rect;
			}
			public RectangleSelection(int x, int y, int width, int height) : this(new Rectangle(x, y, width, height)) { }
			public override bool Contains(Point point) {
				return rect.Contains(point);
			}
			public override IEnumerable<RegionSelection> RemoveCore(Point point) {
				RegionSelection newRect = Create(rect.Left, rect.Top, point.X - rect.Left, rect.Height);
				if(newRect != null)
					yield return newRect;
				newRect = Create(point.X + 1, rect.Top, rect.Right - point.X - 1, rect.Height);
				if(newRect != null)
					yield return newRect;
				newRect = Create(point.X, rect.Top, 1, point.Y - rect.Top);
				if(newRect != null)
					yield return newRect;
				newRect = Create(point.X, point.Y + 1, 1, rect.Bottom - point.Y - 1);
				if(newRect != null)
					yield return newRect;
			}
			public override IEnumerable<RegionSelection> Remove(RegionSelection substraction) {
				Rectangle intersection = Rectangle.Intersect(rect, Rectangle.FromLTRB(substraction.MinX, substraction.MinY, substraction.MaxX + 1, substraction.MaxY + 1));
				if(intersection.IsEmpty || intersection.Width == 0 || intersection.Height == 0)
					return null;
				return RemoveCore(intersection);
			}
			IEnumerable<RegionSelection> RemoveCore(Rectangle intersection) {
				RegionSelection newRect = Create(rect.Left, rect.Top, intersection.X - rect.Left, rect.Height);
				if(newRect != null)
					yield return newRect;
				newRect = Create(intersection.Right, rect.Top, rect.Right - intersection.Right, rect.Height);
				if(newRect != null)
					yield return newRect;
				newRect = Create(intersection.Left, rect.Top, intersection.Right - intersection.X, intersection.Y - rect.Top);
				if(newRect != null)
					yield return newRect;
				newRect = Create(intersection.Left, intersection.Bottom, intersection.Right - intersection.X, rect.Bottom - intersection.Bottom);
				if(newRect != null)
					yield return newRect;
			}
			public override IEnumerable<RegionSelection> Correct(Rectangle correction) {
				if(correction.Contains(rect))
					return null;
				RegionSelection newRect = Create(Rectangle.Intersect(rect, correction));
				if(newRect != null)
					return Return(newRect);
				else
					return Enumerable.Empty<RegionSelection>();
			}
			IEnumerable<RegionSelection> Return(RegionSelection baseRect) {
				yield return baseRect;
			}
			public override Point GetAt(int index) {
				return new Point(rect.Left + (int)Math.Floor((double)index / rect.Height), rect.Top + index % rect.Height);
			}
			protected override IEnumerator<Point> GetEnumerator() {
				for(int i = rect.Left; i < rect.Right; i++)
					for(int j = rect.Top; j < rect.Bottom; j++)
						yield return new Point(i, j);
			}
			public override void FillRows(Dictionary<int, int> dic) {
				int c;
				for(int i = rect.Top; i < rect.Bottom; i++) {
					if(!dic.TryGetValue(i, out c))
						dic.Add(i, rect.Width);
					else
						dic[i] = c + rect.Width;
				}
			}
			public override void FillColumns(Dictionary<int, int> dic) {
				int c;
				for(int i = rect.Left; i < rect.Right; i++) {
					if(!dic.TryGetValue(i, out c))
						dic.Add(i, rect.Height);
					else
						dic[i] = c + rect.Height;
				}
			}
		}
		public int Count { get { return count; } }
		public SelectionStorage() {
		}
		public void Add(Point point) {
			if(Contains(point))
				return;
			store.Add(new PointSelection(point));
			count++;
		}
		public void Add(Rectangle rect) {
			RegionSelection newRect = RegionSelection.Create(rect);
			if(newRect == null)
				return;
			Remove(RegionSelection.Create(rect));
			store.Add(newRect);
			count += rect.Width * rect.Height;
			EnsureCount();
		}
		public bool Contains(Point point) {
			foreach(RegionSelection rect in store)
				if(rect.Contains(point))
					return true;
			return false;
		}
		public Point this[int index] {
			get {
				int count = 0;
				foreach(RegionSelection rect in store) {
					int add = rect.Count;
					if(count + add > index)
						return rect.GetAt(index - count);
					count += add;
				}
				return Point.Empty;
			}
		}
		public void Clear() {
			store.Clear();
			EnsureCount();
		}
		public Rectangle GetRect() {
			int minX = store[0].MinX,
				minY = store[0].MinY,
				maxX = store[0].MaxX,
				maxY = store[0].MaxY;
			for(int i = 1; i < store.Count; i++) {
				if(store[i].MinX < minX)
					minX = store[i].MinX;
				if(store[i].MinY < minY)
					minY = store[i].MinY;
				if(store[i].MaxX > maxX)
					maxX = store[i].MaxX;
				if(store[i].MaxY > maxY)
					maxY = store[i].MaxY;
			}
			return new Rectangle(minX, minY, maxX - minX + 1, maxY - minY + 1);
		}
		void EnsureCount() {
			count = 0;
			foreach(RegionSelection rect in store)
				count += rect.Count;
		}
		public void Substract(SelectionStorage substraction) {
			List<RegionSelection> res = new List<RegionSelection>();
			foreach(RegionSelection elem in substraction.store)
				Remove(elem);
			EnsureCount();
		}
		internal bool Remove(RegionSelection elem) {
			if(elem == null)
				return false;
			bool changed = false;
			for(int i = store.Count - 1; i >= 0; i--) {
				IEnumerable<RegionSelection> result = store[i].Remove(elem);
				if(result != null) {
					store.AddRange(result);
					store.RemoveAt(i);
					changed = true;
				}
			}
			EnsureCount();
			return changed;
		}
		public IEnumerator<Point> GetPointEnumerator() {
			foreach(RegionSelection rect in store)
				foreach(Point point in rect)
					yield return point;
		}
		internal IEnumerator<SelectionStorage.RegionSelection> GetRectEnumerator() {
			return store.GetEnumerator();
		}
		internal IEnumerable<SelectionStorage.RegionSelection> GetRectEnumerable() {
			return store;
		}
		public SelectionStorage Clone() {
			SelectionStorage clone = new SelectionStorage();
			clone.store.AddRange(store);
			clone.EnsureCount();
			return clone;
		}
		internal void AddRange(IEnumerable<RegionSelection> points) {
			store.AddRange(points);
			EnsureCount();
		}
		public bool Correct(Rectangle rect) {
			bool changed = false;
			List<RegionSelection> res = new List<RegionSelection>();
			for(int i = store.Count - 1; i >= 0; i--) {
				IEnumerable<RegionSelection> result = store[i].Correct(rect);
				if(result != null) {
					store.AddRange(result);
					store.RemoveAt(i);
					changed = true;
				}
			}
			EnsureCount();
			return changed;
		}
		internal Dictionary<int, int> GetRows() {
			Dictionary<int, int> dic = new Dictionary<int, int>();
			foreach(RegionSelection rect in store)
				rect.FillRows(dic);
			return dic;
		}
		internal Dictionary<int, int> GetColumns() {
			Dictionary<int, int> dic = new Dictionary<int, int>();
			foreach(RegionSelection rect in store)
				rect.FillColumns(dic);
			return dic;
		}
	}
}
