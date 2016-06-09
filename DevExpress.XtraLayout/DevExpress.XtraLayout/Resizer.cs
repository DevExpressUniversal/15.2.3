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
using System.IO;
using System.Xml;
using System.Text;
using System.Collections;
using System.Linq;
#if DXWhidbey
using System.Collections.Generic;
#endif
using System.Drawing;
using System.ComponentModel;
using System.Globalization;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using DevExpress.XtraLayout.Utils;
using System.Diagnostics;
using DevExpress.XtraLayout.Registrator;
using DevExpress.XtraLayout.Customization;
namespace DevExpress.XtraLayout.Resizing {
#if DXWhidbey
	public interface ICoverageFixerElement {
		int GetPos(bool vertical, bool max);
		void SetPos(bool vertical, bool max, int value);
		void Shift(bool vertical, int range);
	}
	public class CoverageFixerHelper :BaseVisitor {
		int offsetX, offsetY;
		public CoverageFixerHelper(int dx, int dy) {
			offsetX = dx;
			offsetY = dy;
		}
		public override void Visit(BaseLayoutItem item) {
			base.Visit(item);
			item.X = item.X + offsetX;
			item.Y = item.Y + offsetY;
		}
	}
	public static class CoverageFixer {
		public static int GetPos(ICoverageFixerElement element, bool vertical, bool max) {
			return element.GetPos(vertical, max);
		}
		public static void SetPos(ICoverageFixerElement element, bool vertical, bool max, int value) {
			element.SetPos(vertical, max, value);
		}
		public static void Shift(ICoverageFixerElement element, bool vertical, int range) {
			element.Shift(vertical, range);
		}
		public static bool Crosses(ICoverageFixerElement e1, ICoverageFixerElement e2, bool vertical) {
			if(ReferenceEquals(e1, e2))
				return false;
			return Math.Max(GetPos(e1, vertical, false), GetPos(e2, vertical, false)) <= Math.Min(GetPos(e1, vertical, true), GetPos(e2, vertical, true));
		}
		public static bool Crosses(ICoverageFixerElement e1, ICoverageFixerElement e2) {
			return Crosses(e1, e2, true) && Crosses(e1, e2, false);
		}
		public static int Compare(int i1, int i2, bool inverse) {
			int result;
			if(i1 < i2) {
				result = -1;
			} else if(i1 > i2) {
				result = 1;
			} else {
				result = 0;
			}
			if(inverse) {
				result = -result;
			}
			return result;
		}
		[System.Diagnostics.Conditional("DEBUG")]
		static void AssertNotCrosses(IEnumerable list) {
			foreach(ICoverageFixerElement a in list) {
				foreach(ICoverageFixerElement b in list) {
					if(a == b)
						continue;
					if(Crosses(a, b))
						throw new InvalidOperationException();
				}
			}
		}
		[System.Diagnostics.Conditional("DEBUG")]
		static void AssertSolid(IEnumerable list) {
			if(Grow(list))
				throw new InvalidOperationException();
		}
		static bool Grow(IEnumerable list, bool vertical, bool max) {
			bool patched = false;
			AssertNotCrosses(list);
			int externalBound = 0;
			bool boundNotFound = true;
			foreach(ICoverageFixerElement e in list) {
				int pos = GetPos(e, vertical, max);
				if(boundNotFound || Compare(externalBound, pos, max) > 0) {
					externalBound = pos;
					boundNotFound = false;
				}
			}
			foreach(ICoverageFixerElement e in list) {
				int targetBound = externalBound;
				int ownBound = GetPos(e, vertical, max);
				foreach(ICoverageFixerElement another in list) {
					if(e == another)
						continue;
					if(!Crosses(e, another, !vertical))
						continue;
					int anotherBound = GetPos(another, vertical, !max);
					if(Compare(anotherBound, ownBound, max) > 0)
						continue;
					int candidateBound;
					if(max) {
						candidateBound = anotherBound - 1;
					} else {
						candidateBound = anotherBound + 1;
					}
					if(Compare(targetBound, candidateBound, max) < 0)
						targetBound = candidateBound;
				}
				if(ownBound != targetBound) {
					SetPos(e, vertical, max, targetBound);
					patched = true;
				}
			}
			AssertNotCrosses(list);
			return patched;
		}
		static bool Grow(IEnumerable list) {
			bool changes = false;
			if(Grow(list, true, false))
				changes = true;
			if(Grow(list, false, false))
				changes = true;
			if(Grow(list, false, true))
				changes = true;
			if(Grow(list, true, true))
				changes = true;
			return changes;
		}
		static void DeCross(IEnumerable list) {
			foreach(ICoverageFixerElement a in list) {
				foreach(ICoverageFixerElement b in list) {
					if(a == b)
						continue;
					if(!Crosses(a, b))
						continue;
					bool decrossingDirection;
					ICoverageFixerElement fix, mov;
					int moveRange;
					ResolveDecrossing(a, b, out decrossingDirection, out fix, out mov, out moveRange);
					int moveAllTreshhold = GetPos(mov, decrossingDirection, false);
					if(moveAllTreshhold == GetPos(fix, decrossingDirection, false))
						moveAllTreshhold += 1;
					foreach(ICoverageFixerElement e in list) {
						if(e == mov || GetPos(e, decrossingDirection, false) >= moveAllTreshhold)
							Shift(e, decrossingDirection, moveRange);
					}
					System.Diagnostics.Debug.Assert(!Crosses(a, b));
				}
			}
			AssertNotCrosses(list);
		}
		static void ResolveDecrossing(ICoverageFixerElement e1, ICoverageFixerElement e2, out bool vertical, out ICoverageFixerElement fix, out ICoverageFixerElement mov, out int range) {
			ICoverageFixerElement fixV, fixH, movV, movH;
			int rangeV;
			int rangeH;
			ResolveDecrossing(e1, e2, true, out fixV, out movV, out rangeV);
			ResolveDecrossing(e1, e2, false, out fixH, out movH, out rangeH);
			if(rangeV <= rangeH) {
				vertical = true;
				fix = fixV;
				mov = movV;
				range = rangeV;
			} else {
				vertical = false;
				fix = fixH;
				mov = movH;
				range = rangeH;
			}
		}
		static void ResolveDecrossing(ICoverageFixerElement e1, ICoverageFixerElement e2, bool vertical, out ICoverageFixerElement fix, out ICoverageFixerElement mov, out int range) {
			if(GetPos(e1, vertical, false) < GetPos(e2, vertical, false)) {
				fix = e1;
				mov = e2;
			} else if(GetPos(e1, vertical, false) > GetPos(e2, vertical, false)) {
				fix = e2;
				mov = e1;
			} else {
				if(GetPos(e1, vertical, true) <= GetPos(e2, vertical, true)) {
					fix = e1;
					mov = e2;
				} else {
					fix = e2;
					mov = e1;
				}
			}
			range = GetPos(fix, vertical, true) - GetPos(mov, vertical, false) + 1;
		}
		public static void Fix(ICollection list) {
			DeCross(list);
			Grow(list);
			Slice(list, list);
			AssertNotCrosses(list);
			AssertSolid(list);
		}
		static void Slice(ICollection allSubItems, ICollection layout) {
			AssertNotCrosses(layout);
			AssertSolid(layout);
			AssertNotCrosses(allSubItems);
			AssertSolid(allSubItems);
			switch(allSubItems.Count) {
				case 0:
				case 1:
					return;
			}
			IList n1items, n2items;
			if(Divide(allSubItems, out n1items, out n2items)) {
				Slice(n1items, layout);
				Slice(n2items, layout);
				return;
			}
			Patch(allSubItems, layout);
			Slice(allSubItems, layout);
		}
		static void Patch(ICollection allSubItems, ICollection layout) {
			ICoverageFixerElement bottomLeftElem = null;
			int top = 0;
			foreach(ICoverageFixerElement e in allSubItems) {
				if(bottomLeftElem == null) {
					bottomLeftElem = e;
					top = GetPos(e, true, false);
				} else if(GetPos(e, true, true) >= GetPos(bottomLeftElem, true, true) && GetPos(e, false, false) <= GetPos(bottomLeftElem, false, false)) {
					bottomLeftElem = e;
				}
				top = Math.Min(top, GetPos(e, true, false));
			}
			int terminalPos = GetPos(bottomLeftElem, false, false);
			int expandWidth = GetPos(bottomLeftElem, false, true) - terminalPos + 1;
			foreach(ICoverageFixerElement e in layout) {
				if(e == bottomLeftElem) {
					SetPos(e, true, false, top);
				} else if(IsInList(allSubItems, e)) {
					Shift(e, false, expandWidth);
				} else if(GetPos(e, false, false) > terminalPos) {
					Shift(e, false, expandWidth);
				} else if(GetPos(e, false, true) >= terminalPos) {
					SetPos(e, false, true, expandWidth + GetPos(e, false, true));
				}
			}
			Grow(allSubItems);
		}
		static bool IsInList(IEnumerable list, ICoverageFixerElement e) {
			foreach(ICoverageFixerElement q in list) {
				if(e == q)
					return true;
			}
			return false;
		}
		static bool Divide(IEnumerable list, out IList n1Items, out IList n2Items) {
			int divider;
			bool vertical;
			if(FindDivider(list, true, out divider)) {
				vertical = true;
			} else if(FindDivider(list, false, out divider)) {
				vertical = false;
			} else {
				n1Items = null;
				n2Items = null;
				return false;
			}
			n1Items = new List<ICoverageFixerElement>();
			n2Items = new List<ICoverageFixerElement>();
			foreach(ICoverageFixerElement item in list) {
				if(GetPos(item, vertical, false) < divider)
					n1Items.Add(item);
				else
					n2Items.Add(item);
			}
			return true;
		}
		static bool FindDivider(IEnumerable list, bool vertical, out int divider) {
			foreach(ICoverageFixerElement e in list) {
				int ePosMin = GetPos(e, vertical, false);
				bool rejected = false;
				bool lesserFound = false;
				foreach(ICoverageFixerElement q in list) {
					int qPosMin = GetPos(q, vertical, false);
					if(qPosMin >= ePosMin)
						continue;
					lesserFound = true;
					int qPosMax = GetPos(q, vertical, true);
					if(qPosMax >= ePosMin) {
						rejected = true;
						break;
					}
				}
				if(lesserFound && !rejected) {
					divider = ePosMin;
					return true;
				}
			}
			divider = -1;
			return false;
		}
	}
#endif
	public enum NeighbourType { Left, Right, Top, Bottom, None };
	public class ResizerUpdateHelper :BaseVisitor {
		protected BaseLayoutItem groupCore = null;
		protected BaseLayoutItem resultGroupCore = null;
		public ResizerUpdateHelper(BaseLayoutItem group) {
			groupCore = group;
		}
		public BaseLayoutItem ResultGroup {
			get { return resultGroupCore; }
		}
		public override bool StartVisit(BaseLayoutItem item) {
			return resultGroupCore == null;
		}
		public override void Visit(BaseLayoutItem item) {
			if(resultGroupCore != null) return;
			TableGroupResizeGroup tableGroupResizeGroup = item as TableGroupResizeGroup;
			if(tableGroupResizeGroup != null && tableGroupResizeGroup.groupsToResize.ContainsKey(groupCore)) {
				resultGroupCore = tableGroupResizeGroup.groupsToResize[groupCore];
			}
			GroupResizeGroup tempGroup = item as GroupResizeGroup;
			if(item == groupCore)
				resultGroupCore = item;
			if(tempGroup != null && tempGroup.Group == groupCore)
				resultGroupCore = tempGroup;
		}
	}
	public class ResizerParentSearcherToCompleteTree :ResizerUpdateHelper {
		protected LayoutClassificationArgs targetItemClassification, visitingItemClassification;
		protected bool searchInHiddenItemsCore = false;
		public ResizerParentSearcherToCompleteTree(BaseLayoutItem group, bool searchInHiddenItems)
			: base(group) {
			searchInHiddenItemsCore = searchInHiddenItems;
		}
		protected bool ProcessResizeGroup(bool condition1, bool condition2) {
			if(condition1 || condition2) { resultGroupCore = visitingItemClassification.BaseLayoutItem; return true; }
			return false;
		}
		protected virtual void VistItem(BaseLayoutItem item) {
			if(visitingItemClassification.IsTableGroupResizeGroup) {
				if(visitingItemClassification.TableGroupResizeGroup.groupsToResize.ContainsValue(groupCore))
					resultGroupCore = visitingItemClassification.TableGroupResizeGroup;
			}
			if(visitingItemClassification.IsGroupResizeGroup) {
				if(visitingItemClassification.GroupResizeGroup.Item == groupCore || visitingItemClassification.GroupResizeGroup.Group == groupCore)
					resultGroupCore = visitingItemClassification.GroupResizeGroup;
			}
			if(visitingItemClassification.IsResizeGroup) {
				BaseLayoutItem realItem1, realItem2, item1, item2;
				realItem1 = visitingItemClassification.ResizeGroup.RealItem1;
				realItem2 = visitingItemClassification.ResizeGroup.RealItem2;
				item1 = visitingItemClassification.ResizeGroup.Item1;
				item2 = visitingItemClassification.ResizeGroup.Item2;
				GroupResizeGroup realGrg1, realGrg2, grg1, grg2;
				realGrg1 = realItem1 as GroupResizeGroup;
				realGrg2 = realItem2 as GroupResizeGroup;
				grg1 = item1 as GroupResizeGroup;
				grg2 = item2 as GroupResizeGroup;
				if(searchInHiddenItemsCore) {
					if(!ProcessResizeGroup(realItem1 == groupCore, realItem2 == groupCore)) {
						ProcessResizeGroup(realGrg1 != null && realGrg1.Group == groupCore, realGrg2 != null && realGrg2.Group == groupCore);
					}
				} else {
					if(!ProcessResizeGroup(item1 == groupCore, item2 == groupCore)) {
						ProcessResizeGroup(grg1 != null && grg1.Group == groupCore, grg2 != null && grg2.Group == groupCore);
					}
				}
			}
		}
		public override void Visit(BaseLayoutItem item) {
			visitingItemClassification = LayoutClassifier.Default.ClassifyFull(item);
			VistItem(item);
		}
	}
	public class ResizerParentSearcherForVisibility :ResizerParentSearcherToCompleteTree {
		public ResizerParentSearcherForVisibility(BaseLayoutItem group, bool searchInHiddenItems)
			: base(group, searchInHiddenItems) {
			targetItemClassification = LayoutClassifier.Default.Classify(group);
		}
		public override void Visit(BaseLayoutItem item) {
			if(resultGroupCore != null) return;
			visitingItemClassification = LayoutClassifier.Default.ClassifyFull(item);
			if(targetItemClassification.IsGroup && targetItemClassification.Group.Items.Count > 0 && targetItemClassification.Group.Expanded) {
				VisitGroup(targetItemClassification.Group); return;
			}
			if(targetItemClassification.IsTabPage) {
				VisitTabPage(targetItemClassification.Group); return;
			}
			if(targetItemClassification.IsTabbedGroup) {
				VisitTabbedGroup(targetItemClassification.TabbedGroup); return;
			}
			VistItem(item); return;
		}
		public static bool IsGroupForGroupResizeGroup(BaseLayoutItem layoutGroup, GroupResizeGroup grg) {
			if(layoutGroup == null) return false;
			if(grg == null) return false;
			if(grg.Group == layoutGroup) return true;
			return false;
		}
		public static bool IsTabbedGroupForGroupResizeGroup(BaseLayoutItem tabbedGroup, GroupResizeGroup grg) {
			if(tabbedGroup == null) return false;
			if(grg == null) return false;
			if(grg.Group == tabbedGroup) return true;
			return false;
		}
		protected virtual void VisitGroup(BaseLayoutItem layoutGroup) {
			if(visitingItemClassification.IsGroupResizeGroup) {
				GroupResizeGroup grg = visitingItemClassification.GroupResizeGroup.Item as GroupResizeGroup;
				if(IsGroupForGroupResizeGroup(layoutGroup, grg)) resultGroupCore = visitingItemClassification.BaseLayoutItem;
			}
			if(visitingItemClassification.IsResizeGroup) {
				if(searchInHiddenItemsCore)
					ProcessResizeGroup(
						IsGroupForGroupResizeGroup(layoutGroup, visitingItemClassification.ResizeGroup.RealItem1 as GroupResizeGroup),
						IsGroupForGroupResizeGroup(layoutGroup, visitingItemClassification.ResizeGroup.RealItem2 as GroupResizeGroup)
						);
				else
					ProcessResizeGroup(
						IsGroupForGroupResizeGroup(layoutGroup, visitingItemClassification.ResizeGroup.Item1 as GroupResizeGroup),
						IsGroupForGroupResizeGroup(layoutGroup, visitingItemClassification.ResizeGroup.Item2 as GroupResizeGroup)
						);
			}
		}
		protected virtual void VisitTabPage(BaseLayoutItem item) { }
		protected virtual void VisitTabbedGroup(TabbedGroup tg) {
			if(visitingItemClassification.IsGroupResizeGroup) {
				GroupResizeGroup grg = visitingItemClassification.GroupResizeGroup.Item as GroupResizeGroup;
				if(IsGroupForGroupResizeGroup(tg, grg)) resultGroupCore = visitingItemClassification.BaseLayoutItem;
			}
			if(visitingItemClassification.IsResizeGroup) {
				if(searchInHiddenItemsCore)
					ProcessResizeGroup(
						IsTabbedGroupForGroupResizeGroup(tg, visitingItemClassification.ResizeGroup.RealItem1 as GroupResizeGroup),
						IsTabbedGroupForGroupResizeGroup(tg, visitingItemClassification.ResizeGroup.RealItem2 as GroupResizeGroup)
						);
				else
					ProcessResizeGroup(
						IsTabbedGroupForGroupResizeGroup(tg, visitingItemClassification.ResizeGroup.Item1 as GroupResizeGroup),
						IsTabbedGroupForGroupResizeGroup(tg, visitingItemClassification.ResizeGroup.Item2 as GroupResizeGroup)
						);
			}
		}
	}
	public class LayoutSimplifier {
		public static NeighbourType GetNeighbourType(BaseLayoutItem item1, BaseLayoutItem item2) {
			return GetNeighbourType(item1.Bounds, item2.Bounds);
		}
		public static NeighbourType GetNeighbourType(Rectangle rect1, Rectangle rect2) {
			if(rect1.X == rect2.Right && rect1.Height == rect2.Height) {
				return NeighbourType.Left;
			}
			if(rect1.Bottom == rect2.Y && rect1.Width == rect2.Width) {
				return NeighbourType.Top;
			}
			if(rect1.Right == rect2.X && rect1.Height == rect2.Height) {
				return NeighbourType.Right;
			}
			if(rect1.Y == rect2.Bottom && rect1.Width == rect2.Width) {
				return NeighbourType.Bottom;
			}
			return NeighbourType.None;
		}
	}
	public class SplitterHelper :IComparer<Rectangle> {
		public readonly BaseLayoutItem[] Items;
		public readonly Rectangle[] Rects;
		public readonly bool Horizontal;
		readonly List<BaseLayoutItem> Column1;
		readonly List<BaseLayoutItem> Column2;
		public SplitterHelper(List<BaseLayoutItem> items, LayoutType type)
			: this(items.ToArray(), type) {
		}
		public SplitterHelper(BaseLayoutItem[] items, LayoutType type) {
			Items = items;
			Horizontal = (type == LayoutType.Horizontal);
			Rects = GetBounds(Items);
			Array.Sort(Rects, Items, this);
			Column1 = new List<BaseLayoutItem>();
			Column2 = new List<BaseLayoutItem>();
		}
		int IComparer<Rectangle>.Compare(Rectangle r1, Rectangle r2) {
			if(r1 == r2) return 0;
			if(Horizontal && r1.X == r2.X) return r1.Y - r2.Y;
			if(!Horizontal && r1.Y == r2.Y) return r1.X - r2.X;
			return Horizontal ? r1.X - r2.X : r1.Y - r2.Y;
		}
		public void Split(out BaseLayoutItem[] col1, out BaseLayoutItem[] col2) {
			col1 = null; col2 = null;
			if(TryPopulateColumns(true)) {
				col1 = Column1.ToArray();
				col2 = Column2.ToArray();
				if(AreColumnsOptimal) return;
			}
			if(TryPopulateColumns(false)) {
				if(AreColumnsOptimal || col1 == null && col2 == null) {
					col1 = Column2.ToArray();
					col2 = Column1.ToArray();
				}
			}
			if(col1 == null && col2 == null) {
				col1 = new BaseLayoutItem[0]; col2 = Items;
			}
		}
		bool AreColumnsOptimal {
			get { return Column1.Count == 1 || Column2.Count == 1; }
		}
		bool TryPopulateColumns(bool firstToLast) {
			for(int i = 0; i < Rects.Length; i++) {
				Rectangle splitRect = GetSplitRect(Rects, i, Horizontal, firstToLast);
				if(HasSplits(splitRect, Rects)) continue;
				GetColumns(splitRect);
				return true;
			}
			return false;
		}
		void GetColumns(Rectangle splitRect) {
			Column1.Clear(); Column2.Clear();
			for(int i = 0; i < Rects.Length; i++) {
				if(splitRect.Contains(Rects[i]))
					Column1.Add(Items[i]);
				else
					Column2.Add(Items[i]);
			}
		}
		internal static Rectangle[] GetBounds(BaseLayoutItem[] items) {
			Rectangle[] result = new Rectangle[items.Length];
			for(int i = 0; i < items.Length; i++) {
				BaseLayoutItem item = items[i];
				BaseLayoutItem prev = (i > 0) ? items[i - 1] : null;
				BaseLayoutItem next = (i < items.Length - 1) ? items[i + 1] : null;
				result[i] = GetBounds(item, prev, next);
			}
			return result;
		}
		static Rectangle GetBounds(BaseLayoutItem item, BaseLayoutItem prev, BaseLayoutItem next) {
			Rectangle result = item.Bounds;
			if(item.Visibility == LayoutVisibility.Never) {
				Rectangle p = (prev != null) ? prev.Bounds : Rectangle.Empty;
				Rectangle n = (next != null) ? next.Bounds : Rectangle.Empty;
				if(!p.IsEmpty && p.IntersectsWith(result)) {
					result = Trim(result, p);
				}
				if(!n.IsEmpty && n.IntersectsWith(result)) {
					result = Trim(result, n);
				}
			}
			return result;
		}
		static Rectangle Trim(Rectangle a, Rectangle b) {
			int left = a.Left, top = a.Top, w = a.Width, h = a.Height;
			if((a.Top == b.Top) && (a.Height == b.Height)) {
				left = (a.Left > b.Left) ? Math.Max(a.Left, b.Right) : a.Left;
				w = (a.Left > b.Left) ? a.Right - b.Right : b.Left - a.Left;
			}
			if((a.Left == b.Left) && (a.Width == b.Width)) {
				top = (a.Top > b.Top) ? Math.Max(a.Top, b.Bottom) : a.Top;
				h = (a.Top > b.Top) ? a.Bottom - b.Bottom : b.Top - a.Top;
			}
			return new Rectangle(left, top, w, h);
		}
		static Rectangle GetBounds(Rectangle[] rects) {
			if(rects.Length == 0) return Rectangle.Empty;
			int left = int.MaxValue; int top = int.MaxValue;
			int right = 0; int bottom = 0;
			for(int i = 0; i < rects.Length; i++) {
				Rectangle itemRect = rects[i];
				left = Math.Min(itemRect.Left, left);
				top = Math.Min(itemRect.Top, top);
				right = Math.Max(itemRect.Right, right);
				bottom = Math.Max(itemRect.Bottom, bottom);
			}
			return new Rectangle(left, top, right - left, bottom - top);
		}
		internal static Rectangle GetSplitRect(Rectangle[] rects, int splitPosition, bool fHorz, bool firstToLast) {
			Rectangle[] source = new Rectangle[rects.Length];
			Array.Copy(rects, source, source.Length);
			if(!firstToLast)
				Array.Reverse(source);
			int length = Math.Min(splitPosition + 1, rects.Length);
			Rectangle[] splitRects = new Rectangle[length];
			Array.Copy(source, splitRects, length);
			Rectangle bounds = GetBounds(source);
			Rectangle split = GetBounds(splitRects);
			int w = fHorz ? split.Width : bounds.Width;
			int h = fHorz ? bounds.Height : split.Height;
			int left = firstToLast ? bounds.Left : bounds.Right - w;
			int top = firstToLast ? bounds.Top : bounds.Bottom - h;
			return new Rectangle(left, top, w, h);
		}
		internal static bool HasSplits(Rectangle splitRect, Rectangle[] rects) {
			for(int i = 0; i < rects.Length; i++) {
				Rectangle intersection = Rectangle.Intersect(rects[i], splitRect);
				if(intersection.Width == 0 || intersection.Height == 0) continue;
				if(intersection != rects[i]) return true;
			}
			return false;
		}
		public bool HasBorder(BaseLayoutItem[] col1, BaseLayoutItem[] col2) {
			Rectangle col1Rect = GetBounds(GetRects(col1));
			Rectangle col2Rect = GetBounds(GetRects(col2));
			return (LayoutSimplifier.GetNeighbourType(col1Rect, col2Rect) != NeighbourType.None);
		}
		Rectangle[] GetRects(BaseLayoutItem[] items) {
			Rectangle[] rects = new Rectangle[items.Length];
			for(int i = 0; i < items.Length; i++) {
				rects[i] = Rects[Array.IndexOf(Items, items[i])];
			}
			return rects;
		}
	}
	public class BaseSplitterHelper {
		public List<BaseLayoutItem> col1;
		public List<BaseLayoutItem> col2;
		protected List<BaseLayoutItem> items;
		protected LayoutType layoutType;
		public BaseSplitterHelper(List<BaseLayoutItem> items, LayoutType layoutType) {
			col1 = new List<BaseLayoutItem>();
			col2 = new List<BaseLayoutItem>();
			this.layoutType = layoutType;
			this.items = items;
		}
		protected void PopulateCollections(Rectangle rect) {
			foreach(BaseLayoutItem item in items) {
				Rectangle r = item.Bounds;
				r.Intersect(rect);
				if(r == item.Bounds && (new LayoutRectangle(item.Bounds, layoutType).Left != new LayoutRectangle(rect, layoutType).Right ||
					new LayoutRectangle(item.Bounds, layoutType).Right == new LayoutRectangle(rect, layoutType).Left))
					col1.Add(item);
				else
					col2.Add(item);
			}
		}
		protected virtual Rectangle GetNextRect(Point p, List<BaseLayoutItem> items, LayoutType layoutType) {
			BaseLayoutItem corner = null;
			LayoutRectangle bounds = new LayoutRectangle(BaseItemCollection.CalcItemsBounds(items), layoutType);
			foreach(BaseLayoutItem item in items) {
				if(item.Location == p) {
					LayoutSize size = new LayoutSize(item.Bounds.Size, layoutType);
					if(size.Width == 0) {
						if(size.Height == bounds.Height) {
							corner = item;
							break;
						}
					} else
						corner = item;
				}
			}
			if(corner == null)
				return Rectangle.Empty;
			bounds.Right = corner.GetLayoutBounds(layoutType).Right;
			return bounds.Rectangle;
		}
		protected virtual bool HasSplits(Rectangle rect, List<BaseLayoutItem> items, LayoutType layoutType) {
			foreach(BaseLayoutItem item in items) {
				Rectangle r = item.Bounds;
				r.Intersect(rect);
				if(r == Rectangle.Empty)
					continue;
				if(r != item.Bounds && new LayoutSize(r.Size, layoutType).Width != 0)
					return true;
			}
			return false;
		}
		public virtual Rectangle GetSplit() {
			return GetSplit(BaseItemCollection.CalcItemsBounds(items).Location);
		}
		protected virtual Rectangle GetSplit(Point location) {
			LayoutRectangle splitRect = new LayoutRectangle(new Rectangle(location, Size.Empty), layoutType);
			LayoutPoint workpoint;
			int watchDog = 0;
			do {
				watchDog++;
				if(watchDog > 1000) throw new LayoutControlInternalException("while failed in baseSplitterHelper");
				workpoint = splitRect.LayoutLocation;
				workpoint.X = splitRect.Right;
				splitRect = new LayoutRectangle(GetNextRect(workpoint.Point, items, layoutType), layoutType);
			} while(HasSplits(splitRect.Rectangle, items, layoutType));
			PopulateCollections(splitRect.Rectangle);
			return splitRect.Rectangle;
		}
	}
	public class ImprovedSplitterHelper :BaseSplitterHelper {
		ArrayList SplitColumnsCollectionsContains;
		ArrayList SplitColumnsCollectionsOthers;
		ArrayList SplitColumnsRects;
		public ImprovedSplitterHelper(List<BaseLayoutItem> items, LayoutType layoutType)
			: base(items, layoutType) {
			SplitColumnsCollectionsContains = new ArrayList();
			SplitColumnsCollectionsOthers = new ArrayList();
			SplitColumnsRects = new ArrayList();
		}
		protected void restoreCol1Col2(int index) {
			col1 = (List<BaseLayoutItem>)SplitColumnsCollectionsContains[index];
			col2 = (List<BaseLayoutItem>)SplitColumnsCollectionsOthers[index];
		}
		protected Rectangle FindBestSplitRectangle() {
			for(int i = 0; i < SplitColumnsRects.Count; i++) {
				if(((List<BaseLayoutItem>)SplitColumnsCollectionsContains[i]).Count == 1 ||
					((List<BaseLayoutItem>)SplitColumnsCollectionsOthers[i]).Count == 1) {
					restoreCol1Col2(i);
					return (Rectangle)SplitColumnsRects[i];
				}
			}
			restoreCol1Col2(0);
			return (Rectangle)SplitColumnsRects[0];
		}
		public override Rectangle GetSplit() {
			LayoutRectangle itemsRect = new LayoutRectangle(BaseItemCollection.CalcItemsBounds(items), layoutType);
			LayoutRectangle lastResult;
			LayoutPoint lastPoint = new LayoutPoint(itemsRect.Location, layoutType);
			int watchDog = 0;
			do {
				col1.Clear();
				col2.Clear();
				lastResult = new LayoutRectangle(GetSplit(lastPoint.Point), layoutType);
				SplitColumnsRects.Add(lastResult.Rectangle);
				SplitColumnsCollectionsContains.Add(new List<BaseLayoutItem>(col1));
				SplitColumnsCollectionsOthers.Add(new List<BaseLayoutItem>(col2));
				lastPoint = lastResult.LayoutLocation;
				lastPoint.X = lastResult.Right;
				watchDog++;
				if(watchDog > 1000) throw new LayoutControlInternalException("inconsistent layout");
			}
			while(itemsRect.Width != lastPoint.X && lastPoint.X != 0);
			return FindBestSplitRectangle();
		}
	}
	public enum ResizeItemStatus { Normal, Hidden }
	internal enum LayoutSizingType { Horizontal, Vertical, None }
	public abstract class ResizeGroup :CashedConstraintsItem {
		BaseLayoutItem item1, item2;
		protected ResizeItemStatus item1StatusCore, item2StatusCore;
		protected internal LayoutType layoutType;
		protected FakeEmptySpaceItem emptyItem1Core, emptyItem2Core;
		protected ResizeGroup(BaseLayoutItem item_a, BaseLayoutItem item_b, LayoutType layoutType)
			: base(null) {
			GC.SuppressFinalize(this);
			if(item_a != null && item_b != null) {
				SetItems(item_a, item_b);
				item1StatusCore = ResizeItemStatus.Normal;
				item2StatusCore = ResizeItemStatus.Normal;
				LayoutSize size = new LayoutSize(Item1.Size, layoutType);
				size.Width = size.Width + new LayoutSize(Item2.Size, layoutType).Width;
				base.SetInternalSize(size.Size);
				base.SetLocation(item_a.Location);
			} else throw new NullReferenceException("null parameters");
		}
		public override void Accept(BaseVisitor visitor) {
			base.Accept(visitor);
			Item1.Accept(visitor);
			Item2.Accept(visitor);
			if(RealItem1 != Item1) RealItem1.Accept(visitor);
			if(RealItem2 != Item2) RealItem2.Accept(visitor);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(item1 != null && !item1.DisposingFlag) item1.Dispose();
				if(item2 != null && !item2.DisposingFlag) item2.Dispose();
			}
			base.Dispose(disposing);
		}
		public void SetResizeItemStatus(bool isItem1, ResizeItemStatus newStatus, FakeEmptySpaceItem fakeItem) {
			if(isItem1) {
				emptyItem1Core = fakeItem;
				Item1Status = newStatus;
			} else {
				emptyItem2Core = fakeItem;
				Item2Status = newStatus;
			}
		}
		public void SetItemStatus(bool isItem1, ResizeItemStatus resizeStatus) {
			if(isItem1) Item1Status = resizeStatus;
			else Item2Status = resizeStatus;
		}
		public ResizeItemStatus GetItemStatus(bool isItem1) {
			return isItem1 ? Item1Status : Item2Status;
		}
		public BaseLayoutItem GetItem(bool isItem1) {
			return isItem1 ? Item1 : Item2;
		}
		internal BaseLayoutItem GetRealItem(bool isItem1) {
			return isItem1 ? RealItem1 : RealItem2;
		}
		public ResizeItemStatus Status {
			get {
				if(Item1Status == ResizeItemStatus.Hidden && Item2Status == ResizeItemStatus.Hidden) return ResizeItemStatus.Hidden;
				return ResizeItemStatus.Normal;
			}
		}
		public ResizeItemStatus Item1Status {
			get { return item1StatusCore; }
			set {
				ResizeItemStatus olditem1StatusCore = item1StatusCore;
				SynchronizeBefore(true, olditem1StatusCore, value);
				item1StatusCore = value;
				SynchronizeAfter(true, olditem1StatusCore, value);
			}
		}
		public ResizeItemStatus Item2Status {
			get { return item2StatusCore; }
			set {
				ResizeItemStatus olditem2StatusCore = item2StatusCore;
				SynchronizeBefore(false, olditem2StatusCore, value);
				item2StatusCore = value;
				SynchronizeAfter(false, olditem2StatusCore, value);
			}
		}
		protected Rectangle GetRealItemSize(BaseLayoutItem item) {
			GroupResizeGroup grg = item as GroupResizeGroup;
			if(grg != null && grg.Group != null) return grg.Group.Bounds;
			return item.Bounds;
		}
		protected void SetRealItemBounds(BaseLayoutItem bli, Rectangle bounds) {
			SplitterItem si = bli as SplitterItem;
			if(si != null) si.LockLayoutTypeChange++;
			bli.SetBounds(bounds);
			if(si != null) si.LockLayoutTypeChange--;
		}
		protected virtual void SynchronizeBefore(bool workWithItem1, ResizeItemStatus oldStatus, ResizeItemStatus newStatus) {
			if(workWithItem1) {
				if(oldStatus == ResizeItemStatus.Normal && newStatus == ResizeItemStatus.Hidden) EmptyItem1.SetBounds(GetRealItemSize(RealItem1));
				if(oldStatus == ResizeItemStatus.Hidden && newStatus == ResizeItemStatus.Normal && !(RealItem1 is FakeGroup)) SetRealItemBounds(RealItem1, EmptyItem1.Bounds);
			} else {
				if(oldStatus == ResizeItemStatus.Normal && newStatus == ResizeItemStatus.Hidden) EmptyItem2.SetBounds(GetRealItemSize(RealItem2));
				if(oldStatus == ResizeItemStatus.Hidden && newStatus == ResizeItemStatus.Normal && !(RealItem2 is FakeGroup)) SetRealItemBounds(RealItem2, EmptyItem2.Bounds);
			}
		}
		protected virtual void SynchronizeAfter(bool workWithItem1, ResizeItemStatus oldStatus, ResizeItemStatus newStatus) {
		}
		public BaseLayoutItem Item1 { get { return Item1Status == ResizeItemStatus.Normal ? RealItem1 : EmptyItem1; } }
		public BaseLayoutItem Item2 { get { return Item2Status == ResizeItemStatus.Normal ? RealItem2 : EmptyItem2; } }
		internal BaseLayoutItem RealItem1 { get { return item1; } }
		internal BaseLayoutItem RealItem2 { get { return item2; } }
		protected internal FakeEmptySpaceItem EmptyItem1 {
			get {
				if(emptyItem1Core == null) emptyItem1Core = CreateEmptyItem(item1);
				return emptyItem1Core;
			}
		}
		protected internal FakeEmptySpaceItem EmptyItem2 {
			get {
				if(emptyItem2Core == null) emptyItem2Core = CreateEmptyItem(item2);
				return emptyItem2Core;
			}
		}
		protected virtual FakeEmptySpaceItem CreateEmptyItem(BaseLayoutItem item) {
			FakeEmptySpaceItem esItem = new FakeEmptySpaceItem(item.CustomizationFormText);
			esItem.SizeConstraintsType = SizeConstraintsType.Custom;
			esItem.MinSize = new Size(1, 1);
			esItem.Visibility = LayoutVisibility.Never;
			esItem.MaxSize = Size.Empty;
			return esItem;
		}
		public override Size MinSize { get { return groupMinSize; } set { groupMinSize = value; } }
		public override Size MaxSize { get { return groupMaxSize; } set { groupMaxSize = value; } }
		public void SetItems(BaseLayoutItem item1, BaseLayoutItem item2) {
			this.item1 = item1;
			this.item2 = item2;
		}
		protected override void SetLocation(Point location) {
			int dif = new LayoutPoint(location, layoutType).X - new LayoutPoint(Item1.Location, layoutType).X;
			Item1.ChangeLocation(dif, layoutType);
			Item2.ChangeLocation(dif, layoutType);
			base.SetLocation(location);
		}
		public override Point Location {
			get { return Item1.Location; }
			set { }
		}
		public static bool Contains(BaseLayoutItem owner, BaseLayoutItem item) {
			ContainsItemVisitor visitor = new ContainsItemVisitor(item);
			owner.Accept(visitor);
			return visitor.Contains;
		}
		static protected Constraints GetConstraint(BaseLayoutItem item1Local, LayoutType layoutType) {
			if(item1Local != null) {
				Constraints constraint = new Constraints();
				constraint.min = new LayoutSize(item1Local.MinSize, layoutType).Width;
				constraint.max = new LayoutSize(item1Local.MaxSize, layoutType).Width;
				return constraint;
			} else
				throw new NullReferenceException("item is null");
		}
	}
#if DEBUGTEST
	[System.Diagnostics.DebuggerDisplay("Constraints: Min={min}, Max={max}")]
#endif
	public struct Constraints {
		public Constraints(int min, int max) {
			this.min = min;
			this.max = max;
		}
		public Constraints(BaseLayoutItem item, LayoutType layoutType)
			:
			this(new LayoutSize((item != null ? item.Size : Size.Empty), layoutType).Width) { }
		public Constraints(int minMax) {
			this.min = minMax;
			this.max = minMax;
		}
		public Constraints(Size size, LayoutType layoutType) :
			this(new LayoutSize(size, layoutType).Width) { }
		public int min;
		public int max;
		public static Constraints Empty {
			get {
				return new Constraints(0);
			}
		}
	}
	public class TabbedGroupResizeGroup :GroupResizeGroup {
		public TabbedGroupResizeGroup(LayoutType layoutType, BaseLayoutItem item, BaseLayoutItem group) : base(layoutType, item, group) { }
		public override string Text {
			get {
				return "Tabbed group resize group";
			}
		}
	}
	public abstract class CashedConstraintsItem :BaseLayoutItem {
		public CashedConstraintsItem(LayoutGroup parent) : base(parent) { }
		protected Size groupMaxSize, groupMinSize;
		public void UpdateConstraints() {
			UpdateMinSize();
			UpdateMaxSize();
		}
		protected abstract void UpdateMinSize();
		protected abstract void UpdateMaxSize();
		public override Size MinSize { get { return groupMinSize; } set { groupMinSize = value; } }
		public override Size MaxSize { get { return groupMaxSize; } set { groupMaxSize = value; } }
	}
	public class FlowGroupResizeGroup :GroupResizeGroup {
		public FlowGroupResizeGroup(LayoutType layoutType, LayoutGroup group) : base(layoutType, null, group) { }
		LayoutGroup flowGroup { get { return Group as LayoutGroup; } }
		public LayoutGroupItemCollection Items { get { return flowGroup.Items; } }
		public override BaseLayoutItem Item {
			get {
				return Group;
			}
		}
		protected internal override int ChangeItemSize(BaseLayoutItem item, int dif, LayoutType layoutType, Constraints constraint) {
			int result = 0;
			if(Items != null && Items.Contains(item))
				ChangeFlowItemSize(item, dif, layoutType);
			else {
				result = base.ChangeItemSize(item, dif, layoutType, constraint);
				flowGroup.UpdateFlowLayoutItems(false);
			}
			return result;
		}
		protected override LayoutSize ChangeExpandedGroupItemSize(LayoutType layoutType, LayoutSize newGroupSize) {
			return new LayoutSize();
		}
		protected internal override int BestFitCore() {
			flowGroup.UpdateFlowLayoutItems(true);
			return 0;
		}
		void ChangeFlowItemSize(BaseLayoutItem item, int dif, LayoutType layoutType) {
			if(item == null) return;
			switch(layoutType) {
				case LayoutType.Horizontal:
					if(Math.Abs(dif) >= flowGroup.CellSize.Width) {
						Rectangle rect = new Rectangle(item.X, item.Y, item.Width + dif, item.Height);
						if(Width < rect.Width || rect.Width < flowGroup.CellSize.Width) return;
						if(item.MinSize.Width != 0 && rect.Width < item.MinSize.Width) return;
						if(item.MaxSize.Width != 0 && rect.Width > item.MaxSize.Width) return;
						item.SetBounds(rect);
						flowGroup.UpdateFlowLayoutItems(true);
					}
					break;
				case LayoutType.Vertical:
					if(Math.Abs(dif) >= flowGroup.CellSize.Height) {
						Rectangle rect = new Rectangle(item.X, item.Y, item.Width, item.Height + dif);
						if(Height < rect.Height || rect.Height < flowGroup.CellSize.Height) return;
						if(item.MinSize.Height != 0 && rect.Height < item.MinSize.Height) return;
						if(item.MaxSize.Height != 0 && rect.Height > item.MaxSize.Height) return;
						item.SetBounds(rect);
						flowGroup.UpdateFlowLayoutItems(true);
					}
					break;
			}
		}
		protected override void UpdateMaxSize() {
			if(Items == null || Items.Count == 0)
				groupMaxSize = Size.Empty;
			else
				if(Items.Where(e => e.Visible).Count() == 0) return;
			groupMaxSize = AddLabelIndentions(new Size(Items.Where(e => e.Visible).Sum(e => e.Bounds.Width), Items.ItemsBounds.Height));
		}
		protected override void UpdateMinSize() {
			if(Items == null || Items.Count == 0)
				groupMinSize = Size.Empty;
			else
				if(Items.Where(e => e.Visible).Count() == 0) return;
			groupMinSize = AddLabelIndentions(new Size(Items.Where(e => e.Visible).Max(e => e.Bounds.Width), Items.ItemsBounds.Height));
		}
		protected override void SetLocation(Point location) {
			if(location == Location) return;
			base.SetLocation(location);
			flowGroup.UpdateFlowLayoutItems(false);
		}
		protected internal override void ChangeSize(int dif, LayoutType layoutType) {
			if(dif == 0) return;
			base.ChangeSize(dif, layoutType);
			flowGroup.UpdateFlowLayoutItems(false);
		}
		protected override void SetSize(Size value) {
			base.SetSize(value);
			flowGroup.UpdateFlowLayoutItems(false);
		}
		protected override Size SubLabelSizeIndentions(Size groupSize) {
			if(flowGroup.ParentTabbedGroup == null) return base.SubLabelSizeIndentions(groupSize);
			else return groupSize;
		}
		protected override Size AddLabelIndentions(Size groupSize) {
			if(flowGroup.ParentTabbedGroup == null) return base.AddLabelIndentions(groupSize);
			else return groupSize;
		}
	}
	class FlowLayoutMap {
		public FlowLayoutMap(int mapWidth, int parameterW, int parameterH) {
			mapWidthCore = mapWidth > 0 ? mapWidth : 1;
			Map = new int[mapWidthCore];
			itemsWCore = parameterW;
			itemsHCore = parameterH;
		}
		public int[] Map { get; set; }
		public int MapWidth { get { return mapWidthCore; } }
		public int ItemsW { get { return itemsWCore; } }
		public int ItemsH { get { return itemsHCore; } }
		int mapWidthCore, itemsWCore, itemsHCore;
		internal bool CheckLayoutMap(ref Rectangle checkRectangle, bool shouldStartNewLine) {
			Rectangle mapRectangle = new Rectangle(checkRectangle.X / ItemsW, checkRectangle.Y / ItemsH, checkRectangle.Width / ItemsW, checkRectangle.Height / ItemsH);
			if(MapWidth < mapRectangle.X + mapRectangle.Width) return false;
			bool returnValue = false;
			int startPosition = 0;
			if(shouldStartNewLine) SetNewLine(0);
			for(int i = 0; i < MapWidth; i++) {
				bool FillRectangle = true;
				if(Map[i] <= mapRectangle.Y) {
					startPosition = i;
					for(int j = startPosition; j < startPosition + mapRectangle.Width; j++) {
						if(j >= MapWidth) {
							FillRectangle = false;
							break;
						}
						if(Map[j] - 1 >= mapRectangle.Y) FillRectangle = false;
					}
					if(FillRectangle) {
						mapRectangle.X = startPosition;
						checkRectangle.X = startPosition * ItemsW;
						returnValue = true;
						break;
					}
				}
			}
			if(returnValue) {
				for(int i = mapRectangle.X; i < mapRectangle.X + mapRectangle.Width; i++) {
					Map[i] = mapRectangle.Y + mapRectangle.Height;
				}
			}
			return returnValue;
		}
		internal void SetNewLine(int delay) {
			int max = Map.Max() + delay;
			for(int k = 0; k < MapWidth; k++)
				Map[k] = max;
		}
		internal void ClearMap() {
			for(int i = 0; i < Map.Count(); i++) {
				Map[i] = 0;
			}
		}
	}
	public class TableGroupResizeGroup :GroupResizeGroup {
		public TableGroupResizeGroup(LayoutType layoutType, LayoutGroup group, LayoutGroupItemCollection layoutGroupItemCollection, Resizer resizer) : base(layoutType, null, group) {
			foreach(BaseLayoutItem item in layoutGroupItemCollection) {
				if(item is LayoutItemContainer) {
					groupsToResize.Add(item, resizer.GroupItems(layoutType, new List<BaseLayoutItem>(new BaseLayoutItem[] { item })));
				}
			}
		}
		LayoutGroup tableGroup { get { return Group as LayoutGroup; } }
		internal Dictionary<BaseLayoutItem, BaseLayoutItem> groupsToResize = new Dictionary<BaseLayoutItem, BaseLayoutItem>();
		public LayoutGroupItemCollection Items { get { return tableGroup.Items; } }
		public override void Accept(BaseVisitor visitor) {
			foreach(BaseLayoutItem bli in groupsToResize.Values) {
				bli.Accept(visitor);
			}
			base.Accept(visitor);
		}
		public override BaseLayoutItem Item {
			get {
				return Group;
			}
		}
		protected internal override int ChangeItemSize(BaseLayoutItem item, int dif, LayoutType layoutType, Constraints constraint) {
			int result = 0;
			if(item != null) {
				foreach(BaseLayoutItem bli in groupsToResize.Values) {
					if(ResizeGroup.Contains(bli, item)) {
						bli.ChangeItemSize(item, dif, layoutType, bli is GroupResizeGroup ? new Constraints((bli as GroupResizeGroup).Group, layoutType) : new Constraints(bli, layoutType));
					}
				}
			}
			if(Items != null && Items.Contains(item))
				ChangeTableItemSize(item, dif, layoutType, constraint);
			else {
				result = base.ChangeItemSize(item, dif, layoutType, constraint);
			}
			tableGroup.UpdateTableLayoutCore();
			return result;
		}
		private void ChangeTableItemSize(BaseLayoutItem item, int dif, LayoutType layoutType, Constraints constraint) {
			Resizer manager;
			if(layoutType == LayoutType.Horizontal) manager = tableGroup.tableLayoutManagerHorizontal;
			else manager = tableGroup.tableLayoutManagerVertical;
			item = manager.GroupForTable.GetItem(layoutType == LayoutType.Horizontal ? item.OptionsTableLayoutItem.ColumnIndex : item.OptionsTableLayoutItem.RowIndex);
			if(item == null) return;
			item.Size = new Size(item.Width + (layoutType == LayoutType.Horizontal ? dif : 0), item.Height + (layoutType == LayoutType.Horizontal ? 0 : dif));
			manager.UpdateResizer(manager.GroupForTable);
			item.Parent.Size = tableGroup.sizeToSetTableManagerGroup;
		}
		protected override LayoutSize ChangeExpandedGroupItemSize(LayoutType layoutType, LayoutSize newGroupSize) {
			return new LayoutSize();
		}
		protected internal override int BestFitCore() {
			tableGroup.UpdateTableLayoutCore();
			return 0;
		}
		protected override void SetLocation(Point location) {
			if(location == Location) return;
			base.SetLocation(location);
		}
		protected override Size SubLabelSizeIndentions(Size groupSize) {
			if(tableGroup.ParentTabbedGroup == null) return base.SubLabelSizeIndentions(groupSize);
			else return groupSize;
		}
		protected override Size AddLabelIndentions(Size groupSize) {
			if(tableGroup.ParentTabbedGroup == null) return base.AddLabelIndentions(groupSize);
			else return groupSize;
		}
		protected override void UpdateMaxSize() {
			if(tableGroup.IsDisposing) return;
			if(tableGroup.Expanded) {
				Size result = tableGroup.GetTableMaxSize();
				groupMaxSize = AddLabelIndentions(result);
				if(result.Width == 0) groupMaxSize.Width = 0;
				if(result.Height == 0) groupMaxSize.Height = 0;
			} else groupMaxSize = tableGroup.MaxSize;
		}
		protected override void UpdateMinSize() {
			if(tableGroup.IsDisposing) return;
			if(tableGroup.Expanded) {
				groupMinSize = AddLabelIndentions(tableGroup.GetTableMinSize());
			} else
				groupMinSize = tableGroup.MinSize;
		}
		protected override bool SetInternalSize(Size size) {
			Group.SetSizeWithoutCorrection(size);
			tableGroup.UpdateTableLayoutCore();
			UpdateInnerGroupResizeGroup();
			return true;
		}
		public override Size Size {
			get {
				return Item.Size;
			}
			set {
				Item.Size = value;
			}
		}
		protected override void SetSize(Size value) {
			base.SetSize(value);
		}
		private void UpdateInnerGroupResizeGroup() {
			foreach(BaseLayoutItem bli in groupsToResize.Values) {
				if(bli is GroupResizeGroup) {
					GroupResizeGroup grg = bli as GroupResizeGroup;
					int itemDif;
					LayoutSize size1 = new LayoutSize(grg.Size, layoutType);
					LayoutSize size2 = new LayoutSize(grg.Group.Size, layoutType);
					itemDif = size2.Width - size1.Width;
					grg.ChangeSize(itemDif, layoutType);
				}
			}
		}
		internal int GetMinConstraints(BaseLayoutItem item, LayoutType layoutType, bool MinMax, Resizer resizer) {
			int result = -1;
			foreach(BaseLayoutItem bli in groupsToResize.Values) {
				if(bli == null) continue;
				if(ResizeGroup.Contains(bli, item)) {
					result = resizer.GetMinConstraints(bli, item, layoutType, MinMax);
					break;
				}
			}
			if(result != -1) return result;
			if(Items.Contains(item)) return MinMax ? new LayoutSize(item.MaxSize, layoutType).Width : new LayoutSize(item.MinSize, layoutType).Width;
			return -1;
		}
		internal void UpdateTableConstraints() {
			UpdateConstraints();
			foreach(BaseLayoutItem bli in groupsToResize.Values) {
				if(bli == null) continue;
				Resizer.UpdateConstraintsInternal(bli);
			}
		}
	}
	public class GroupResizeGroup :CashedConstraintsItem {
		BaseLayoutItem group;
		BaseLayoutItem item;
		ResizeItemStatus groupStatusCore;
		protected LayoutType layoutType;
		public GroupResizeGroup(LayoutType layoutType, BaseLayoutItem item, BaseLayoutItem group)
			: base(null) {
			GC.SuppressFinalize(this);
			this.layoutType = layoutType;
			this.item = item;
			groupStatusCore = ResizeItemStatus.Normal;
			if(group != null) {
				this.group = group;
				base.SetInternalSize(group.Size);
				base.SetLocation(group.Location);
			} else
				throw new LayoutControlInternalException("group parameter is null");
		}
		public override void Accept(BaseVisitor visitor) {
			base.Accept(visitor);
			if(Item != null) Item.Accept(visitor);
			if(group != null) group.Accept(visitor);
		}
		public virtual BaseLayoutItem Item {
			get { return item; }
			set { item = value; }
		}
		public virtual ResizeItemStatus GroupStatus {
			get { return groupStatusCore; }
			set { groupStatusCore = value; }
		}
		public BaseLayoutItem Group {
			get { return group; }
		}
		protected internal override int BestFitCore() {
			return Item.BestFitCore();
		}
		protected virtual Size SubLabelSizeIndentions(Size groupSize) {
			return group.ViewInfo.SubLabelSizeIndentions(groupSize);
		}
		protected virtual Size AddLabelIndentions(Size groupSize) {
			return group.ViewInfo.AddLabelIndentions(groupSize);
		}
		protected override bool SetInternalSize(Size size) {
			int itemDif = 0;
			int groupDif = 0;
			bool result = true;
			if(group != null && group.Expanded) {
				LayoutSize size1 = new LayoutSize(Item.Size, layoutType);
				LayoutSize size2 = new LayoutSize(SubLabelSizeIndentions(size), layoutType);
				LayoutSize lms = new LayoutSize(Item.MinSize, layoutType);
				itemDif = size2.Width - size1.Width;
				if((size1.Width + itemDif) < lms.Width) {
					int absDif = Math.Abs(size1.Width - lms.Width);
					if(itemDif < 0) itemDif = -absDif;
					else itemDif = absDif;
					Item.ChangeSize(itemDif, layoutType);
					CalcDiffSimple(ref size, ref groupDif);
				} else {
					Item.ChangeSize(itemDif, layoutType);
					groupDif = new LayoutSize(AddLabelIndentions(Item.Size), layoutType).Width - new LayoutSize(group.Size, layoutType).Width;
					result = itemDif == groupDif;
				}
			} else {
				CalcDiffSimple(ref size, ref groupDif);
			}
			group.ChangeSize(groupDif, layoutType);
			base.SetInternalSize(group.Size);
			return result;
		}
		internal void CalcDiffSimple(ref Size size, ref int groupDif) {
			LayoutSize actualSize = new LayoutSize(group.Size, layoutType);
			LayoutSize requaredSize = new LayoutSize(size, layoutType);
			groupDif = requaredSize.Width - actualSize.Width;
		}
		protected override void SetLocation(Point location) {
			int dif = new LayoutPoint(location, layoutType).X - new LayoutPoint(Location, layoutType).X;
			group.ChangeLocation(dif, layoutType);
			base.SetLocation(location);
		}
		protected override void UpdateMinSize() { groupMinSize = CheckGroupMinMaxSize(CalculatedMinSize); }
		protected override void UpdateMaxSize() { groupMaxSize = CheckGroupMinMaxSize(CalculatedMaxSize); }
		private Size CheckGroupMinMaxSize(Size size) {
			if(size.Width < 0) size.Width = 0;
			if(size.Height < 0) size.Height = 0;
			return size;
		}
		protected Size CalculatedMinSize {
			get {
				if(GroupStatus == ResizeItemStatus.Hidden) return AddLabelIndentions(new Size(1, 2));
				if(group.Expanded) return AddLabelIndentions(Item.MinSize);
				else
					return group.MinSize;
			}
		}
		protected Size CalculatedMaxSize {
			get {
				if(GroupStatus == ResizeItemStatus.Hidden) return AddLabelIndentions(new Size(1, 2));
				if(group.Expanded) {
					Size size = AddLabelIndentions(Size.Empty);
					if(Item.MaxSize.Width != 0)
						size.Width += Item.MaxSize.Width;
					else
						size.Width = 0;
					if(Item.MaxSize.Height != 0)
						size.Height += Item.MaxSize.Height;
					else
						size.Height = 0;
					return size;
				} else
					return group.MaxSize;
			}
			set {
				Size size = SubLabelSizeIndentions(value);
				if(value.Width == 0)
					size.Width = 0;
				if(value.Height == 0)
					size.Height = 0;
				Item.MaxSize = size;
			}
		}
		protected internal override int ChangeItemSize(BaseLayoutItem item, int dif, LayoutType layoutType, Constraints constraint) {
			LayoutGroup igroup = group as LayoutGroup;
			TabbedGroup tgroup = group as TabbedGroup;
			int realdif = 0;
			if(item != Group && ((igroup != null && igroup.Contains(item))
				|| (tgroup != null && tgroup.Contains(item)))) {
				LayoutSize newSize;
				newSize = new LayoutSize(SubLabelSizeIndentions(Size.Empty), layoutType);
				if(constraint.max != 0) constraint.max += newSize.Width;
				if(constraint.min != 0) constraint.min += newSize.Width;
				realdif = Item.ChangeItemSize(item, dif, layoutType, constraint);
				LayoutSize size1 = new LayoutSize(Item.Size, layoutType);
				LayoutSize size2 = new LayoutSize(group.Size, layoutType);
				int groupDif = (size1.Width - newSize.Width) - size2.Width;
				group.ChangeSize(groupDif, layoutType);
				realdif = groupDif;
			} else {
				realdif = dif;
				LayoutSize ls = new LayoutSize(this.Size, layoutType);
				if(constraint.max == 0)
					constraint.max = new LayoutSize(this.MaxSize, layoutType).Width;
				else {
					if(new LayoutSize(this.MaxSize, layoutType).Width != 0 && constraint.max > new LayoutSize(this.MaxSize, layoutType).Width)
						constraint.max = new LayoutSize(this.MaxSize, layoutType).Width;
				}
				if(constraint.min == 0)
					constraint.min = new LayoutSize(this.MinSize, layoutType).Width;
				else {
					if(constraint.min < new LayoutSize(this.MinSize, layoutType).Width)
						constraint.min = new LayoutSize(this.MinSize, layoutType).Width;
				}
				if(constraint.max != 0 && constraint.min > constraint.max)
					constraint.max = constraint.min;
				if(constraint.max != 0 && ls.Width + realdif > constraint.max)
					realdif = constraint.max - ls.Width;
				if(constraint.min != 0 && ls.Width + realdif < constraint.min)
					realdif = constraint.min - ls.Width;
				group.ChangeSize(realdif, layoutType);
				LayoutSize newGroupSize = new LayoutSize(group.Size, layoutType);
				newGroupSize.Width = newGroupSize.Width;
				if(Group.Expanded) {
					newGroupSize = ChangeExpandedGroupItemSize(layoutType, newGroupSize);
				}
			}
			base.SetInternalSize(group.Size);
			return realdif;
		}
		protected virtual LayoutSize ChangeExpandedGroupItemSize(LayoutType layoutType, LayoutSize newGroupSize) {
			LayoutSize newItemSize = new LayoutSize(SubLabelSizeIndentions(newGroupSize.Size), layoutType);
			LayoutSize itemSize = new LayoutSize(Item.Size, layoutType);
			int iDif = newItemSize.Width - itemSize.Width;
			Item.ChangeItemSize(null, iDif, layoutType,
				new Constraints(newItemSize.Width));
			return newGroupSize;
		}
	}
	public class VerticalResizeGroup :ResizeGroup {
		public VerticalResizeGroup(BaseLayoutItem item_a, BaseLayoutItem item_b, LayoutType layoutType)
			: base(item_a, item_b, LayoutGeometry.InvertLayout(layoutType)) {
			this.layoutType = layoutType;
		}
		protected Size CalcualteMinSize() {
			LayoutSize lsMinSize, ls2;
			lsMinSize = new LayoutSize(this.Item1.MinSize, this.layoutType);
			ls2 = new LayoutSize(this.Item2.MinSize, this.layoutType);
			if(lsMinSize.Width < ls2.Width)
				lsMinSize = ls2;
			return lsMinSize.Size;
		}
		public override string Text {
			get {
				return "Vertical resize group";
			}
		}
		protected override void UpdateMinSize() {
			groupMinSize = CalcualteMinSize();
		}
		protected internal override int BestFitCore() {
			int rating1 = Item1.BestFitCore();
			int rating2 = Item2.BestFitCore();
			return Math.Max(rating1, rating2);
		}
		protected override void UpdateMaxSize() {
			LayoutSize ls2;
			LayoutSize lsMinSize = new LayoutSize(CalcualteMinSize(), layoutType);
			LayoutSize lsMaxSize = new LayoutSize(this.Item1.MaxSize, this.layoutType);
			ls2 = new LayoutSize(this.Item2.MaxSize, this.layoutType);
			if(lsMaxSize.Width == 0 || lsMaxSize.Width > ls2.Width && ls2.Width != 0)
				lsMaxSize = ls2;
			if(lsMaxSize.Width != 0 && lsMaxSize.Width < lsMinSize.Width)
				groupMaxSize = groupMinSize;
			else
				groupMaxSize = lsMaxSize.Size;
		}
		protected override bool SetInternalSize(Size size) {
			LayoutSize sizeI1 = new LayoutSize(Item1.Size, layoutType);
			LayoutSize sizeI2 = new LayoutSize(Item2.Size, layoutType);
			LayoutSize size2 = new LayoutSize(size, layoutType);
			int dif1 = size2.Width - sizeI1.Width;
			int dif2 = size2.Width - sizeI2.Width;
			Item1.ChangeSize(dif1, layoutType);
			Item2.ChangeSize(dif2, layoutType);
			base.SetInternalSize(size);
			return new LayoutSize(Item1.Size, layoutType).Width == size2.Width && new LayoutSize(Item2.Size, layoutType).Width == size2.Width;
		}
		internal Constraints GetConstraint(Constraints prev, Constraints newConstraint) {
			Constraints constraint = new Constraints();
			if((prev.max != 0 && prev.max < newConstraint.max) || newConstraint.max == 0)
				constraint.max = prev.max;
			else
				constraint.max = newConstraint.max;
			if(prev.min != 0 && prev.min > newConstraint.min)
				constraint.min = prev.min;
			else
				constraint.min = newConstraint.min;
			if(constraint.min > constraint.max && constraint.max != 0)
				constraint.max = constraint.min;
			return constraint;
		}
		internal void UpdateSize(LayoutType layoutType) {
			LayoutSize size = new LayoutSize(Item1.Size, layoutType);
			size.Height += new LayoutSize(Item2.Size, layoutType).Height;
			base.SetInternalSize(size.Size);
		}
		protected internal override int ChangeItemSize(BaseLayoutItem item, int dif, LayoutType layoutType, Constraints constraint) {
			BaseLayoutItem item1, item2;
			if(Contains(Item1, item)) {
				item1 = Item1;
				item2 = Item2;
			} else {
				if(!Contains(Item2, item))
					item = null;
				item2 = Item1;
				item1 = Item2;
			}
			Constraints item1Constraints = GetConstraint(item1, layoutType);
			Constraints item2Constraints = GetConstraint(item2, layoutType);
			Constraints resultConstraints = item2Constraints;
			if(resultConstraints.min < item1Constraints.min) { resultConstraints.min = item1Constraints.min; }
			if(item1Constraints.max != 0)
				if(resultConstraints.max > item1Constraints.max) resultConstraints.max = item1Constraints.max;
			Constraints tmpConstraints = GetConstraint(constraint, resultConstraints);
			int realdif = item1.ChangeItemSize(item, dif, layoutType, tmpConstraints);
			item2.ChangeItemSize(null, realdif, layoutType,
				new Constraints(item1, layoutType));
			UpdateSize(layoutType);
			return realdif;
		}
	}
	public class SplitterHorizontalResizeGroup :HorizontalResizeGroup {
		public SplitterHorizontalResizeGroup(BaseLayoutItem item_a, BaseLayoutItem item_b, LayoutType layoutType) : base(item_a, item_b, layoutType) { }
		public SplitterItem SplitterItem {
			get {
				SplitterItem sp1 = GetSplitterItem(Item1, RealItem1);
				SplitterItem sp2 = GetSplitterItem(Item2, RealItem2);
				return (sp1 != null) ? sp1 : sp2;
			}
		}
		public BaseLayoutItem NonSplitterItem {
			get {
				SplitterItem sp1 = GetSplitterItem(Item1, RealItem1);
				return (sp1 == null) ? Item1 : Item2;
			}
		}
		static SplitterItem GetSplitterItem(BaseLayoutItem item, BaseLayoutItem realItem) {
			SplitterItem splitter = item as SplitterItem;
			if(splitter == null) splitter = realItem as SplitterItem;
			return splitter;
		}
	}
	public class HorizontalResizeGroup :ResizeGroup {
		internal double relationCore;
		static double minRelation = 0.01;
		public HorizontalResizeGroup(BaseLayoutItem item_a, BaseLayoutItem item_b, LayoutType layoutType)
			: base(item_a, item_b, layoutType) {
			this.layoutType = layoutType;
			UpdateProportion();
		}
		public override string Text {
			get {
				return "Horizontal resize group " + Relation.ToString("##.##");
			}
		}
		protected internal double Relation {
			get {
				return relationCore;
			}
			set {
				relationCore = value;
			}
		}
		public virtual void UpdateProportion() {
			LayoutSize size1 = new LayoutSize(Item1.Size, layoutType);
			LayoutSize size2 = new LayoutSize(Item2.Size, layoutType);
			Relation = (double)size1.Width / (size2.Width + size1.Width);
		}
		protected override void SynchronizeAfter(bool workWithItem1, ResizeItemStatus oldStatus, ResizeItemStatus newStatus) {
			base.SynchronizeAfter(workWithItem1, oldStatus, newStatus);
		}
		protected bool IsEmptyItem(BaseLayoutItem item) {
			return (item.MaxSize.Width == 0 && item.MaxSize.Height == 1) || (item.MaxSize.Width == 1 && item.MaxSize.Height == 0);
		}
		protected bool IsHiddenPart(BaseLayoutItem item, bool isReallyHidden) {
			ResizeGroup resizeGroup = item as ResizeGroup;
			GroupResizeGroup grg = item as GroupResizeGroup;
			if(resizeGroup != null) {
				return IsHiddenPart(resizeGroup.Item1, isReallyHidden) &&
					IsHiddenPart(resizeGroup.Item2, isReallyHidden);
			}
			if(grg != null) {
				return grg.GroupStatus == ResizeItemStatus.Hidden || IsHiddenPart(grg.Item, isReallyHidden);
			}
			if(item.Owner != null && item.Owner.DesignMode) return false;
			if(!isReallyHidden) return item.Visibility == LayoutVisibility.Never;
			else return item is FakeEmptySpaceItem;
		}
		protected virtual bool CanPatchMaxSize(BaseLayoutItem item) {
			return !(item is FakeEmptySpaceItem) && IsHiddenPart(item, false) && !(item is LayoutControlItem);
		}
		protected virtual void PatchMaxSize(BaseLayoutItem item) {
			if(layoutType == LayoutType.Horizontal)
				item.MaxSize = new Size(item.MinSize.Width, item.MaxSize.Height);
			else
				item.MaxSize = new Size(item.MaxSize.Height, item.MinSize.Height);
		}
		protected override void UpdateMaxSize() {
			Check();
			LayoutSize minSize1, minSize2;
			minSize1 = new LayoutSize(this.Item1.MinSize, layoutType);
			minSize2 = new LayoutSize(this.Item2.MinSize, layoutType);
			if(CanPatchMaxSize(Item1)) PatchMaxSize(Item1);
			if(CanPatchMaxSize(Item2)) PatchMaxSize(Item2);
			LayoutSize ls1, ls2;
			ls1 = new LayoutSize(this.Item1.MaxSize, layoutType);
			ls2 = new LayoutSize(this.Item2.MaxSize, layoutType);
			if(ls2.Width != 0 && ls1.Width != 0)
				ls1.Width = ls1.Width + ls2.Width;
			else
				ls1.Width = 0;
			groupMaxSize = ls1.Size;
		}
		protected internal override int BestFitCore() {
			int rating1 = Item1.BestFitCore();
			int rating2 = Item2.BestFitCore();
			if((rating2 + rating1) != 0) {
				Relation = (float)rating1 / (float)(rating2 + rating1);
				if(Relation == 0) Relation = minRelation;
			} else
				throw new LayoutControlInternalException("resizer internal error");
			return rating1 + rating2;
		}
		protected override void UpdateMinSize() {
			LayoutSize ls1, ls2;
			ls1 = new LayoutSize(this.Item1.MinSize, layoutType);
			ls2 = new LayoutSize(this.Item2.MinSize, layoutType);
			ls1.Width = ls1.Width + ls2.Width;
			var currentSize = new LayoutSize(Size, layoutType);
#if DEBUGTEST
			if(currentSize.Width > 0 && ls1.Width > currentSize.Width) {
			}
#endif
			groupMinSize = ls1.Size;
		}
		protected override bool SetInternalSize(Size size) {
			Check();
			LayoutSize newSize = new LayoutSize(size, layoutType);
			int width = (int)Math.Round(newSize.Width * Relation);
			SplitterHorizontalResizeGroup shrg1 = Item1 as SplitterHorizontalResizeGroup, shrg2 = Item2 as SplitterHorizontalResizeGroup;
			if(shrg1 != null || shrg2 != null) {
				SplitterItem splitter = shrg1 != null ? shrg1.SplitterItem : shrg2.SplitterItem;
				if(splitter != null && splitter.FixedStyle != SplitterItemFixedStyles.None) {
					BaseLayoutItem leftItem, rightItem;
					if(shrg1 != null) {
						leftItem = shrg1.NonSplitterItem;
						rightItem = Item2;
					} else {
						leftItem = shrg2.NonSplitterItem;
						rightItem = Item1;
					}
					LayoutRectangle leftItemRect = new LayoutRectangle(leftItem.Bounds, layoutType);
					LayoutRectangle rightItemRect = new LayoutRectangle(rightItem.Bounds, layoutType);
					if(leftItemRect.X > rightItemRect.X) {
						BaseLayoutItem temp = leftItem;
						leftItem = rightItem;
						rightItem = temp;
					}
					int originalItem1Width = new LayoutSize(Item1.Size, layoutType).Width;
					int originalItem2Width = new LayoutSize(Item2.Size, layoutType).Width;
					if(splitter.FixedStyle == SplitterItemFixedStyles.LeftTop) {
						if(leftItem == Item1) width = originalItem1Width;
						else width = newSize.Width - originalItem2Width;
					} else {
						if(rightItem == Item1) width = originalItem1Width;
						else width = newSize.Width - originalItem2Width;
					}
				}
			}
			LayoutSize ls1, ls2;
			ls1 = new LayoutSize(this.Item1.MinSize, layoutType);
			ls2 = new LayoutSize(this.Item2.MinSize, layoutType);
			if(ls1.Width > width)
				width = ls1.Width;
			if(ls2.Width > (newSize.Width - width)) {
				width = newSize.Width - ls2.Width;
			}
			ls1 = new LayoutSize(this.Item1.MaxSize, layoutType);
			ls2 = new LayoutSize(this.Item2.MaxSize, layoutType);
			if(ls2.Width != 0 && ls2.Width < (newSize.Width - width)) {
				width = newSize.Width - ls2.Width;
			}
			if(ls1.Width != 0 && ls1.Width < width)
				width = ls1.Width;
			if(width < 1) width = 1;
			width = PatchWidth(width, newSize);
			int dif = (width - new LayoutSize(Item1.Size, layoutType).Width);
			Item1.ChangeSize(dif, layoutType);
			Item2.ChangeLocation(dif, layoutType);
			dif = ((newSize.Width - width) - new LayoutSize(Item2.Size, layoutType).Width);
			Item2.ChangeSize(dif, layoutType);
			base.SetInternalSize(size);
			return true;
		}
		protected virtual int PatchWidth(int width, LayoutSize newSize) { return width; }
		protected Constraints GetConstraint(Constraints prev, Constraints newConstraint) {
			Constraints constraint = GetMaxConstraint(prev, newConstraint);
			if(prev.min != 0 && newConstraint.max != 0)
				constraint.min = prev.min - newConstraint.max;
			else
				constraint.min = 0;
			if(constraint.min < 0)
				constraint.min = 0;
			return constraint;
		}
		Constraints GetMaxConstraint(Constraints prev, Constraints newConstraint) {
			Constraints constraint = new Constraints();
			if(prev.max != 0)
				constraint.max = prev.max - newConstraint.min;
			else
				constraint.max = 0;
			return constraint;
		}
		protected void UpdateSize(LayoutType layoutType) {
			LayoutSize size = new LayoutSize(Item1.Size, layoutType);
			size.Width += new LayoutSize(Item2.Size, layoutType).Width;
			base.SetInternalSize(size.Size);
		}
		protected int CorrectDif(int dif, int correctValue) {
			if(dif < 0)
				return dif + correctValue;
			else
				return dif - correctValue;
		}
		[Conditional("DEBUGTEST")]
		protected void Check() {
			if(this.Relation == 0) {
				System.Diagnostics.Debug.Assert(this.Relation != 0);
			}
		}
		protected internal override int ChangeItemSize(BaseLayoutItem item, int dif, LayoutType layoutType, Constraints constraint) {
			Check();
			int rest;
			LayoutType layouTypeForNewResize;
			if(layoutType == LayoutType.Vertical) layouTypeForNewResize = LayoutType.Horizontal;
			else layouTypeForNewResize = LayoutType.Vertical;
			LayoutRectangle lRectItem = new LayoutRectangle();
			LayoutRectangle lRectItemParent = new LayoutRectangle();
			if(item != null && item.Parent != null && item.Parent.Items != null) {
				lRectItemParent = new LayoutRectangle(item.Parent.Items.ItemsBounds, layouTypeForNewResize); 
				lRectItem = new LayoutRectangle(item.Bounds, layouTypeForNewResize);
			}
			if(ContainsSplitter && (Contains(Item1, item) || Contains(Item2, item))) {
				int originalDif = dif;
				LayoutSize item1Size = new LayoutSize(Item1.Size, layoutType);
				LayoutSize item2Size = new LayoutSize(Item2.Size, layoutType);
				Constraints item1Constaints = GetConstraint(Item1, layoutType);
				Constraints item2Constaints = GetConstraint(Item2, layoutType);
				int newItem1Width = item1Size.Width + dif;
				int newItem2Width = item2Size.Width - dif;
				if(item1Constaints.min > newItem1Width)
					dif = 0;
				if(item1Constaints.max != 0 && item1Constaints.max < newItem1Width)
					dif = CorrectDif(dif, newItem1Width - item1Constaints.max);
				if(item2Constaints.min > newItem2Width)
					dif = CorrectDif(dif, item2Constaints.min - newItem2Width);
				if(item2Constaints.max != 0 && item2Constaints.max < newItem2Width)
					dif = CorrectDif(dif, newItem2Width - item2Constaints.max);
				if(originalDif != 0 && dif == 0) {
					BaseLayoutItem targetItem = null;
					if(Contains(Item1, item)) targetItem = Item1; else targetItem = Item2;
					LayoutSize cSize = new LayoutSize(targetItem.Size, layoutType);
					targetItem.ChangeItemSize(item, originalDif, layoutType, new Constraints(cSize.Width, cSize.Width));
				}
			}
			if(Contains(Item1, item)) {
				Constraints item1Constaints = GetConstraint(Item1, layoutType);
				Constraints item2Constaints = GetConstraint(Item2, layoutType);
				Constraints constaintsToSet = GetConstraint(constraint, item2Constaints);
				Constraints ownConstaints = item1Constaints;
				if(ownConstaints.max != 0 && constaintsToSet.min > ownConstaints.max)
					constaintsToSet.min = ownConstaints.max;
				int restdif = Item1.ChangeItemSize(item, dif, layoutType, constaintsToSet);
				Item2.ChangeLocation(restdif, layoutType);
				if(item.Parent != null) lRectItemParent = new LayoutRectangle(item.Parent.Items.ItemsBounds, layouTypeForNewResize); 
				if(item != null && item.Owner != null && !item.Owner.OptionsView.FitControlsToDisplayAreaHeight && item.Parent != null && !ContainsSplitter) {
					if(lRectItem.Width == lRectItemParent.Width) rest = restdif;
					else rest = restdif + Item2.ChangeItemSize(item, -restdif, layoutType, GetConstraint(constraint, item1Constaints));
				} else {
					rest = restdif + Item2.ChangeItemSize(item, -restdif, layoutType, GetConstraint(constraint, item1Constaints));
				}
			} else {
				if(Contains(Item2, item)) {
					LayoutSize ls1 = new LayoutSize(Item1.Size, layoutType);
					LayoutSize ls2 = new LayoutSize(Item2.Size, layoutType);
					LayoutSize lsm1 = new LayoutSize(Item1.MinSize, layoutType);
					if(dif > 0 && ls1.Width + ls2.Width + dif > constraint.max && (constraint.max != 0 || ContainsSplitter)) {
						rest = Item2.ChangeItemSize(item, dif, layoutType, new Constraints(0, constraint.max - lsm1.Width));
						if(rest != 0) {
							LayoutSize lsItem2AfterResize = new LayoutSize(Item2.Size, layoutType);
							int minConstraintForItem1 = 0;
							if(constraint.min - lsItem2AfterResize.Width > 0) minConstraintForItem1 = constraint.min - lsItem2AfterResize.Width;
							int restdif = Item1.ChangeItemSize(item, -rest, layoutType, new Constraints(minConstraintForItem1, 0));
							Item2.ChangeLocation(restdif, layoutType);
							rest += restdif;
						}
					} else {
						rest = Item2.ChangeItemSize(item, dif, layoutType,
							GetConstraint(constraint, new Constraints(Item1, layoutType)));
					}
				} else {
					if(item != null) {
						int restdif = Item1.ChangeItemSize(item, dif, layoutType, new Constraints());
						Item2.ChangeLocation(restdif, layoutType);
						rest = restdif + Item2.ChangeItemSize(item, dif - restdif, layoutType,
							GetConstraint(constraint, new Constraints(Item1, layoutType)));
					} else {
						int restdif = Item2.ChangeItemSize(item, dif, layoutType, GetConstraint(constraint, GetConstraint(Item1, layoutType)));
						rest = restdif + Item1.ChangeItemSize(item, dif - restdif, layoutType,
							GetConstraint(constraint, new Constraints(Item2, layoutType)));
						Item2.ChangeLocation(rest - restdif, layoutType);
					}
				}
			}
			UpdateSize(layoutType);
			return rest;
		}
		protected bool ContainsSplitter {
			get {
				return
					AllowSplitterResizing(Item1 as SplitterHorizontalResizeGroup) ||
					AllowSplitterResizing(Item2 as SplitterHorizontalResizeGroup);
			}
		}
		protected bool AllowSplitterResizing(SplitterHorizontalResizeGroup shrg) {
			if(shrg == null) return false;
			SplitterItem sitem = shrg.Item1 as SplitterItem;
			if(sitem == null) sitem = shrg.Item2 as SplitterItem;
			if(sitem == null) return true;
			return sitem.ResizeMode == SplitterItemResizeMode.OnlyAdjacentControls;
		}
	}
	public class FakeEmptySpaceItem :EmptySpaceItem {
		protected internal override int BestFitCore() { return 1; }
		public FakeEmptySpaceItem(string realName)
			: base() {
			realItemName = realName;
		}
		string realItemName = "";
		protected override string GetDisplayName() {
			return "Fake " + realItemName;
		}
	}
	public class FakeGroup :GroupResizeGroup {
		public FakeGroup(LayoutType layoutType, BaseLayoutItem item, BaseLayoutItem group)
			: base(layoutType, item, group) {
		}
		public override string Text {
			get {
				return "Fake group for tab";
			}
		}
		protected internal override int ChangeItemSize(BaseLayoutItem item, int dif, LayoutType layoutType, Constraints constraint) {
			int diff = Item.ChangeItemSize(item, dif, layoutType, constraint);
			Group.SetSizeWithoutCorrection(Item.Size);
			SetSizeWithoutCorrection(Item.Size);
			return diff;
		}
		public override Size MinSize {
			get { return Item.MinSize; }
		}
		public override Size MaxSize {
			get {
				return Item.MaxSize;
			}
		}
		protected internal override void ChangeSize(int dif, LayoutType layoutType) {
			if(Group.Size != Size || Group.Location != Location) {
				SetSizeWithoutCorrection(Group.Size);
				SetLocationWithoutCorrection(Group.Location);
			}
			base.ChangeSize(dif, layoutType);
		}
		protected override bool SetInternalSize(Size size) {
			LayoutSize size1 = new LayoutSize(Item.Size, layoutType);
			LayoutSize size2 = new LayoutSize(size, layoutType);
			int dif = size2.Width - size1.Width;
			Item.ChangeSize(dif, layoutType);
			Group.SetSizeWithoutCorrection(size);
			SetSizeWithoutCorrection(size);
			return true;
		}
	}
#if DXWhidbey
	public class RestoreLayoutHelper :BaseVisitor {
		public override void Visit(BaseLayoutItem item) {
			LayoutGroup group = item as LayoutGroup;
			if(group != null) {
				group.BeginInit();
				foreach(BaseLayoutItem bitem in new ArrayList(group.Items)) {
					bitem.BeginInit();
					Size sizeToSet = bitem.Size;
					if(sizeToSet.Width > 1) sizeToSet.Width--;
					if(sizeToSet.Height > 1) sizeToSet.Height--;
					if(sizeToSet.Width <= 0) sizeToSet.Width = 1;
					if(sizeToSet.Height <= 0) sizeToSet.Height = 1;
					bitem.Size = sizeToSet;
				}
				CoverageFixer.Fix(group.Items);
				foreach(BaseLayoutItem bitem in new ArrayList(group.Items)) {
					bitem.Width++;
					bitem.Height++;
					bitem.EndInit();
				}
				group.EndInit();
				if(group.Items.ItemsBounds.Location != Point.Empty) {
					Point offset = group.Items.ItemsBounds.Location;
					group.BeginInit();
					foreach(BaseLayoutItem bitem in new ArrayList(group.Items)) bitem.SetBounds(new Rectangle(new Point(bitem.X - offset.X, bitem.Y - offset.Y), bitem.Size));
					group.EndInit();
				}
			}
		}
	}
#endif
	public class InconsistentLayoutException :LayoutControlInternalException {
		public InconsistentLayoutException() : base("null") { }
		public InconsistentLayoutException(string reason) : base(reason) { }
	}
	public class Resizer {
		protected internal LayoutGroup group;
		internal LayoutGroup GroupForTable { get { return group; } }
		Size originalSize;
		internal BaseLayoutItem resultH, resultV;
#if DXWhidbey
		protected virtual void RestoreBrokenLayoutCore2(BaseLayoutItem group) {
			try {
				RestoreLayoutHelper helper = new RestoreLayoutHelper();
				group.Accept(helper);
				group.UpdateAfterRestore();
			} catch(LayoutControlInternalException) {
				DiagnosticsSaveIncorrectLayout(group);
				if(group.Owner != null) group.Owner.ExceptionsThrown = true;
			}
		}
		private static void DiagnosticsSaveIncorrectLayout(BaseLayoutItem group) {
			LayoutGroup lGroup = group as LayoutGroup;
			if(lGroup != null && lGroup.Owner != null) {
				(lGroup.Owner as ISupportImplementor).Implementor.ResizerBroken = true;
				lGroup.Owner.SaveLayoutToXml("incorrectLayout.xml");
				(lGroup.Owner as ISupportImplementor).Implementor.ResizerBroken = false;
			}
		}
#endif
		protected virtual void RestoreBrokenLayoutCore(BaseLayoutItem item) {
			LayoutGroup lgroup = item as LayoutGroup;
			LayoutControlItem citem = item as LayoutControlItem;
			TabbedGroup tgroup = item as TabbedGroup;
			if(lgroup != null) {
				lgroup.BeginInit();
				int prevItemBottom = 0;
				foreach(BaseLayoutItem bitem in lgroup.Items) {
					bitem.SetBounds(new Rectangle(0, prevItemBottom, lgroup.ViewInfo.ClientArea.Width, lgroup.ViewInfo.ClientArea.Height / lgroup.Items.Count));
					prevItemBottom += lgroup.ViewInfo.ClientArea.Height / lgroup.Items.Count;
					RestoreBrokenLayoutCore(bitem);
				}
				lgroup.EndInit();
			}
			if(tgroup != null) {
				tgroup.BeginInit();
				foreach(LayoutGroup tempGroup in tgroup.TabPages) {
					RestoreBrokenLayoutCore(tempGroup);
				}
				tgroup.EndInit();
			}
		}
		internal bool isLayoutRestored = false;
		internal static bool wasLayoutBroken = false;
		bool Check5Items(Rectangle rect1, Rectangle rect2) { return rect2.Height > rect1.Height && rect1.Bottom == rect2.Bottom && (rect2.Right == rect1.Left || rect1.Right == rect2.Left); }
		protected Point RotatePoint90(Point point) {
			return new Point(-point.Y, point.X);
		}
		protected Rectangle RotateRectangle90(Rectangle rectangle) {
			Point p00 = new Point(rectangle.X, rectangle.Y), p10 = new Point(rectangle.Right, rectangle.Y), p01 = new Point(rectangle.X, rectangle.Bottom), p11 = new Point(rectangle.Right, rectangle.Bottom);
			p00 = RotatePoint90(p00);
			p01 = RotatePoint90(p01);
			p10 = RotatePoint90(p10);
			p11 = RotatePoint90(p11);
			return new Rectangle(
				Math.Min(p00.X, Math.Min(p01.X, Math.Min(p10.X, p11.X))),
				Math.Min(p00.Y, Math.Min(p01.Y, Math.Min(p10.Y, p11.Y))),
				rectangle.Height, rectangle.Width);
		}
		protected Rectangle RotateRectangle180(Rectangle rectangle) { return RotateRectangle90(RotateRectangle90(rectangle)); }
		protected Rectangle RotateRectangle270(Rectangle rectangle) { return RotateRectangle180(RotateRectangle90(rectangle)); }
		protected virtual void Fix5ItemsProblem(List<BaseLayoutItem> leftItems) {
			if(leftItems.Count >= 5) {
				int minX = int.MaxValue;
				int minY = int.MaxValue;
				foreach(BaseLayoutItem item1 in leftItems) {
					int counter = 0;
					foreach(BaseLayoutItem item2 in leftItems) {
						minX = Math.Min(minX, item2.X);
						minY = Math.Min(minY, item2.Y);
						bool left = Check5Items(item1.Bounds, item2.Bounds);
						bool top = Check5Items(RotateRectangle90(item1.Bounds), RotateRectangle90(item2.Bounds));
						bool right = Check5Items(RotateRectangle180(item1.Bounds), RotateRectangle180(item2.Bounds));
						bool bottom = Check5Items(RotateRectangle270(item1.Bounds), RotateRectangle270(item2.Bounds));
						if(left || top || bottom || right) counter++;
					}
					if(counter >= 4) {
						CoverageFixerHelper cfh = new CoverageFixerHelper(minX - item1.X, minY - item1.Y);
						item1.Accept(cfh);
						break;
					}
				}
			}
		}
		protected virtual void RestoreBrokenLayout() { RestoreBrokenLayout(null); }
		protected virtual void RestoreBrokenLayout(List<BaseLayoutItem> leftItems) {
			if(isLayoutRestored) return;
			if(resultH != null && resultV != null && (currentlyAffectedGroup == null || currentlyAffectedGroup == group)) return;
			try {
				wasLayoutBroken = true;
				throw new InconsistentLayoutException("Warning layout was broken. Trying to restore");
			} finally {
				if(leftItems != null) {
					Fix5ItemsProblem(leftItems);
				}
				RestoreBrokenLayoutCore2(currentlyAffectedGroup != null ? currentlyAffectedGroup : group);
				isLayoutRestored = true;
				group.SetShouldUpdateViewInfo();
			}
		}
		int deepCounter = 0;
		static bool HasBorder(List<BaseLayoutItem> col1, List<BaseLayoutItem> col2) {
			if(LayoutSimplifier.GetNeighbourType(BaseItemCollection.CalcItemsBounds(col1), BaseItemCollection.CalcItemsBounds(col2)) == NeighbourType.None)
				return false;
			return true;
		}
		protected internal virtual BaseLayoutItem GroupTwoItems(List<BaseLayoutItem> items, LayoutType layoutType, bool invert) {
			if(isLayoutRestored) return null;
			SplitterHelper helper = new SplitterHelper(items, layoutType);
			BaseLayoutItem[] prev, next;
			try {
				helper.Split(out prev, out next);
			} catch {
				RestoreBrokenLayout();
				return null;
			}
			if(next.Length == 0 && invert) {
				if(prev.Length > 0) {
					RestoreBrokenLayout();
					return null;
				}
			}
			if(next.Length == 0) return null;
			if(!helper.HasBorder(prev, next)) {
				RestoreBrokenLayout();
				return null;
			}
			if(invert) {
				layoutType = (layoutType == LayoutType.Vertical) ? LayoutType.Horizontal : LayoutType.Vertical;
			}
			deepCounter++;
			if(deepCounter > 200 && group.Owner is LayoutControl) {
				string warningString = "The layout is not efficient, as there is a group, containing more than 200 items per level. To resolve the issue, reorganize the layout by moving extra items to a new borderless group.";
				if(group.Owner != null && !group.Owner.EnableCustomizationMode) throw new LayoutControlException(warningString);
				else {
					if(!layoutNotEfficientWarningShown) System.Windows.Forms.MessageBox.Show(warningString, "Warning", System.Windows.Forms.MessageBoxButtons.OK);
					layoutNotEfficientWarningShown = true;
				}
			}
			BaseLayoutItem item1 = GroupItems(layoutType, new List<BaseLayoutItem>(prev));
			BaseLayoutItem item2 = GroupItems(layoutType, new List<BaseLayoutItem>(next));
			deepCounter = 0;
			if(item1 == null || item2 == null) return null;
			if(invert)
				return CreateVerticalResizeGruopInstance(item1, item2, layoutType);
			else
				return CreateHorizontalResizeGruop(item1, item2, layoutType);
		}
		static bool layoutNotEfficientWarningShown = false;
		protected virtual HorizontalResizeGroup CreateHorizontalResizeGruopInstance(BaseLayoutItem item1, BaseLayoutItem item2, LayoutType layoutType) {
			HorizontalResizeGroup result = new HorizontalResizeGroup(item1, item2, layoutType);
			AddToDictionary(item1, item2, layoutType, result);
			return result;
		}
		void AddToDictionary(BaseLayoutItem item1, BaseLayoutItem item2, LayoutType layoutType, BaseLayoutItem result) {
			if(layoutType == LayoutType.Horizontal) {
				AddToDictionaryCore(hash_Horizontal, item1, result);
				AddToDictionaryCore(hash_Horizontal, item2, result);
			} else {
				AddToDictionaryCore(hash_Vertical, item1, result);
				AddToDictionaryCore(hash_Vertical, item2, result);
			}
		}
		static void AddToDictionaryCore(Dictionary<BaseLayoutItem, BaseLayoutItem> hash, BaseLayoutItem realItem, BaseLayoutItem resultResizeNode) {
			if(!hash.ContainsKey(realItem)) hash.Add(realItem, resultResizeNode);
		}
		Dictionary<BaseLayoutItem, BaseLayoutItem> hash_Vertical = new Dictionary<BaseLayoutItem, BaseLayoutItem>();
		Dictionary<BaseLayoutItem, BaseLayoutItem> hash_Horizontal = new Dictionary<BaseLayoutItem, BaseLayoutItem>();
		protected virtual VerticalResizeGroup CreateVerticalResizeGruopInstance(BaseLayoutItem item1, BaseLayoutItem item2, LayoutType layoutType) {
			VerticalResizeGroup result = new VerticalResizeGroup(item1, item2, layoutType);
			AddToDictionary(item1, item2, layoutType, result);
			return result;
		}
		protected HorizontalResizeGroup CreateHorizontalResizeGruop(BaseLayoutItem item1, BaseLayoutItem item2, LayoutType layoutType) {
			if(item1 is SplitterItem || item2 is SplitterItem)
				return new SplitterHorizontalResizeGroup(item1, item2, layoutType);
			return CreateHorizontalResizeGruopInstance(item1, item2, layoutType);
		}
		protected void WriteCommonAttributes(BaseLayoutItem item, XmlTextWriter writer, int percent) {
			writer.WriteAttributeString("Width", percent.ToString());
			writer.WriteAttributeString("Name", item.Name);
			writer.WriteAttributeString("Text", item.Text);
		}
		protected void StartSerialization(XmlTextWriter writer, String name) {
			writer.WriteStartElement(name);
		}
		protected void EndSerialization(XmlTextWriter writer) {
			writer.WriteEndElement();
		}
		public LayoutType GetRealLayoutType(ResizeGroup resizeGroup) {
			LayoutType layoutType = resizeGroup.layoutType;
			if(resizeGroup is VerticalResizeGroup)
				layoutType = LayoutGeometry.InvertLayout(layoutType);
			return layoutType;
		}
		protected internal virtual void ExportLayoutCore(BaseLayoutItem item, XmlTextWriter writer, int percent) {
			if(item == null) return;
			LayoutControlItem citem = item as LayoutControlItem;
			LayoutControlGroup cgroup = item as LayoutControlGroup;
			TabbedControlGroup tgroup = item as TabbedControlGroup;
			HorizontalResizeGroup hrGroup = item as HorizontalResizeGroup;
			VerticalResizeGroup vrGroup = item as VerticalResizeGroup;
			GroupResizeGroup grGroup = item as GroupResizeGroup;
			if(citem != null) {
				StartSerialization(writer, "LayoutControlItem");
				WriteCommonAttributes(item, writer, percent);
				writer.WriteAttributeString("ControlName", citem.ControlName);
				EndSerialization(writer);
			}
			if(cgroup != null) {
				StartSerialization(writer, "LayoutControlGroup");
				WriteCommonAttributes(item, writer, percent);
				EndSerialization(writer);
			}
			if(tgroup != null) {
				StartSerialization(writer, "TabbedGroup");
				WriteCommonAttributes(item, writer, percent);
				EndSerialization(writer);
			}
			if(hrGroup != null) {
				StartSerialization(writer, "HorizontalResizeGroup");
				WriteCommonAttributes(item, writer, percent);
				int ip1 = (int)(hrGroup.Relation * 100.0);
				ExportLayoutCore(hrGroup.Item1, writer, ip1);
				ExportLayoutCore(hrGroup.Item2, writer, 100 - ip1);
				EndSerialization(writer);
			}
			if(vrGroup != null) {
				StartSerialization(writer, "VreticalResizeGroup");
				WriteCommonAttributes(item, writer, percent);
				StartSerialization(writer, "VerticalResizeGroupItem1");
				ExportLayoutCore(vrGroup.Item1, writer, 100);
				EndSerialization(writer);
				StartSerialization(writer, "VerticalResizeGroupItem2");
				ExportLayoutCore(vrGroup.Item2, writer, 100);
				EndSerialization(writer);
				EndSerialization(writer);
			}
			if(grGroup != null) {
				StartSerialization(writer, "GroupResizeGroup");
				WriteCommonAttributes(item, writer, percent);
				ExportLayoutCore(grGroup.Group, writer, 100);
				ExportLayoutCore(grGroup.Item, writer, 100);
				EndSerialization(writer);
			}
		}
		public void ExportLayout(Stream stream) {
			XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8);
			writer.Formatting = Formatting.Indented;
			writer.WriteStartDocument();
			String PItext = "type='text/xsl' href='layoutExport.xsl'";
			writer.WriteProcessingInstruction("xml-stylesheet", PItext);
			writer.WriteComment("XtraLayoutControl layout tree export");
			ExportLayoutCore(resultH, writer, 100);
			writer.Flush();
		}
		public void BestFit() {
			resultH.BestFitCore();
			UpdateConstraints();
			resultV.BestFitCore();
			UpdateConstraints();
		}
		protected BaseLayoutItem ProcessTab(LayoutType layoutType, LayoutGroup group) {
			if(group != null) {
				if(group.LayoutMode == LayoutMode.Flow) return new FlowGroupResizeGroup(layoutType, group);
				if(group.LayoutMode == LayoutMode.Table) return new TableGroupResizeGroup(layoutType, group, group.Items, this);
				if(group.Items.Count == 0) return group;
				BaseLayoutItem item = GroupItems(layoutType, group.Items.ConvertToTypedList());
				return new FakeGroup(layoutType, item, group);
			} else
				throw new NullReferenceException("group is null");
		}
		protected BaseLayoutItem ProcessTabs(LayoutType layoutType, TabbedGroupsCollection TabPages) {
			if(TabPages == null)
				return null;
			BaseLayoutItem item = ProcessTab(layoutType, TabPages[0]);
			for(int i = 1; i < TabPages.Count; i++) {
				item = CreateVerticalResizeGruopInstance(item, ProcessTab(layoutType, TabPages[i]), layoutType);
			}
			return item;
		}
		public BaseLayoutItem GroupItems(LayoutType layoutType, List<BaseLayoutItem> items) {
			if(isLayoutRestored) return null;
			if(items != null) {
				if(items.Count == 1) {
					LayoutGroup tempLayoutGroup = items[0] as LayoutGroup;
					if(tempLayoutGroup != null && tempLayoutGroup.Expanded) {
						if(tempLayoutGroup.LayoutMode == LayoutMode.Table) {
							return new TableGroupResizeGroup(layoutType, tempLayoutGroup, tempLayoutGroup.Items, this);
						}
						if(tempLayoutGroup.Items.Count != 0) {
							if(tempLayoutGroup.LayoutMode == LayoutMode.Flow) {
								return new FlowGroupResizeGroup(layoutType, tempLayoutGroup);
							}
							BaseLayoutItem contentResizeElement = GroupItems(layoutType, tempLayoutGroup.Items.ConvertToTypedList());
							if(contentResizeElement == null) return null;
							return new GroupResizeGroup(layoutType, contentResizeElement, tempLayoutGroup);
						}
					}
					TabbedGroup tgroup = items[0] as TabbedGroup;
					if(tgroup != null && tgroup.TabPages.Count > 0) {
						return new TabbedGroupResizeGroup(layoutType, ProcessTabs(layoutType, tgroup.TabPages), tgroup);
					}
					return (BaseLayoutItem)items[0];
				}
				BaseLayoutItem item = GroupTwoItems(items, layoutType, false);
				if(item == null)
					item = GroupTwoItems(items, layoutType == LayoutType.Vertical ? LayoutType.Horizontal : LayoutType.Vertical, true);
				if(item == null) {
					RestoreBrokenLayout(items);
					return null;
				}
				return item;
			} else
				throw new NullReferenceException("items collection is null");
		}
		protected internal Size GetItemMinSize(BaseLayoutItem item) {
			if(item != null) {
				if(!item.IsGroup) return item.MinSize;
				return new Size(Get_Width(item, true), Get_Height(item, true));
			} else
				return Size.Empty;
		}
		protected internal Size GetItemMaxSize(BaseLayoutItem item) {
			if(item != null) {
				if(!item.IsGroup) return item.MaxSize;
				return new Size(Get_Width(item, false), Get_Height(item, false));
			}
			return Size.Empty;
		}
		protected int Get_Height(BaseLayoutItem item, bool MinMax) {
			if(resultV == null) CreateNewResizing();
			int height = GetMinConstraints(resultV, item, LayoutType.Vertical, MinMax);
			if(height < 0) throw new LayoutControlInternalException("there is no such item in resizer");
			return height;
		}
		protected int Get_Width(BaseLayoutItem item, bool MinMax) {
			if(resultH == null) CreateNewResizing();
			int width = GetMinConstraints(resultH, item, LayoutType.Horizontal, MinMax);
			if(width < 0) throw new LayoutControlInternalException("there is no such item in resizer");
			return width;
		}
		protected internal int GetMinConstraints(BaseLayoutItem lookupElement, BaseLayoutItem item, LayoutType layoutType, bool MinMax) {
			if(lookupElement != null) {
				int tempval1, tempval2;
				TableGroupResizeGroup tableGroup = lookupElement as TableGroupResizeGroup;
				GroupResizeGroup tempGroupResizeGroup = lookupElement as GroupResizeGroup;
				ResizeGroup tempResizeGroup = lookupElement as ResizeGroup;
				LayoutGroup group = lookupElement as LayoutGroup;
				if(tableGroup != null) {
					if(tableGroup.Group.Expanded) {
						if(tableGroup.Group == item)
							return new LayoutSize(MinMax ? lookupElement.MinSize : lookupElement.MaxSize, layoutType).Width;
						tempval1 = tableGroup.GetMinConstraints(item, layoutType, MinMax, this);
						if(tempval1 >= 0) return tempval1;
					} else {
						return new LayoutSize(MinMax ? tableGroup.Group.MinSize : tableGroup.Group.MaxSize, layoutType).Width;
					}
				}
				if(tempGroupResizeGroup != null) {
					if(tempGroupResizeGroup.Group.Expanded) {
						if(tempGroupResizeGroup.Group == item)
							return new LayoutSize(MinMax ? lookupElement.MinSize : lookupElement.MaxSize, layoutType).Width;
						tempval1 = GetMinConstraints(tempGroupResizeGroup.Item, item, layoutType, MinMax);
						if(tempval1 >= 0) return tempval1;
					} else {
						return new LayoutSize(MinMax ? tempGroupResizeGroup.Group.MinSize : tempGroupResizeGroup.Group.MaxSize, layoutType).Width;
					}
				}
				if(tempResizeGroup != null) {
					tempval1 = GetMinConstraints(tempResizeGroup.Item1, item, layoutType, MinMax);
					if(tempval1 >= 0) return tempval1;
					tempval2 = GetMinConstraints(tempResizeGroup.Item2, item, layoutType, MinMax);
					if(tempval2 >= 0) return tempval2;
				}
				if(group != null) {
					if(group.Items != null && group.Contains(item) && group.LayoutMode != LayoutMode.Regular && item is LayoutGroup) {
						if(MinMax) {
							var groupInFlowLayoutGroup = item as LayoutGroup;
							return new LayoutSize(groupInFlowLayoutGroup.ViewInfo.AddLabelIndentions(groupInFlowLayoutGroup.Items.ItemsBounds.Size), layoutType).Width;
						} else {
							return 0;
						}
					}
				}
			}
			return -1;
		}
		public Resizer(LayoutGroup group) {
			if(group != null) {
				this.group = group;
				UpdateOriginalSize();
				CreateNewResizing();
			} else throw new NullReferenceException("group is null");
		}
		public void RestoreSize() {
			if(originalSize == Size.Empty) return;
			SizeIt(originalSize, false);
		}
		protected void UpdateOriginalSize() {
			if(group == null) return;
			originalSize = group.Size;
		}
		internal bool CompleteIfNeeded(LayoutGroup incompleteGroup) {
			bool result1 = CompleteIfNeeded(resultH, incompleteGroup, LayoutType.Horizontal);
			bool result2 = CompleteIfNeeded(resultV, incompleteGroup, LayoutType.Vertical);
			return result1 | result2;
		}
		internal bool CompleteIfNeeded(BaseLayoutItem resizerComponent, LayoutGroup incompleteGroup, LayoutType layoutType) {
			if(incompleteGroup.Parent == null) return false;
			BaseLayoutItem resizeGroup = FindGroupForModifiedGroup(incompleteGroup, resizerComponent);
			if(resizeGroup is GroupResizeGroup || incompleteGroup.Items.Count == 0) return false;
			BaseLayoutItem rParent = FindResizerParentToCompleteTree(resizerComponent, incompleteGroup, false);
			if(rParent != null) {
				switch(incompleteGroup.LayoutMode) {
					case LayoutMode.Regular:
						ReplaceResizeTreeNode(resizerComponent, incompleteGroup, new GroupResizeGroup(layoutType, GroupItems(layoutType, incompleteGroup.Items.ConvertToTypedList()), incompleteGroup));
						break;
					case LayoutMode.Flow:
						ReplaceResizeTreeNode(resizerComponent, incompleteGroup, new FlowGroupResizeGroup(layoutType, incompleteGroup));
						break;
					case LayoutMode.Table:
						ReplaceResizeTreeNode(resizerComponent, incompleteGroup, new TableGroupResizeGroup(layoutType, incompleteGroup, incompleteGroup.Items, this));
						break;
				}
			} else
				throw new LayoutControlInternalException("cant find resize tree node");
			return true;
		}
		protected LayoutGroup currentlyAffectedGroup = null;
		internal void UpdateResizer(LayoutGroup affectedGroup) {
			currentlyAffectedGroup = affectedGroup;
			cache = new FindResizerParentCache();
			hash_Horizontal.Clear();
			hash_Vertical.Clear();
			UpdateResizerCore(resultH, affectedGroup, LayoutType.Horizontal);
			UpdateResizerCore(resultV, affectedGroup, LayoutType.Vertical);
			foreach(var item in hash_Horizontal) cache.AddItem(resultH, item.Key, true, item.Value);
			foreach(var item in hash_Vertical) cache.AddItem(resultV, item.Key, true, item.Value);
			currentlyAffectedGroup = null;
		}
		protected internal BaseLayoutItem FindResizerParentToCompleteTree(BaseLayoutItem searchArea, BaseLayoutItem item, bool searchHiddenItems) {
			ResizerParentSearcherToCompleteTree helper = new ResizerParentSearcherToCompleteTree(item, searchHiddenItems);
			searchArea.Accept(helper);
			return helper.ResultGroup;
		}
		protected internal class FindResizerParentCache {
			protected internal class ComplexKey {
				BaseLayoutItem searchArea;
				BaseLayoutItem item;
				bool searchHiddenItems;
				public ComplexKey(BaseLayoutItem searchArea, BaseLayoutItem item, bool searchHiddenItems) {
					this.searchArea = searchArea;
					this.item = item;
					this.searchHiddenItems = searchHiddenItems;
				}
				public override int GetHashCode() {
					if(searchArea == null || item == null) return base.GetHashCode();
					return searchArea.GetHashCode() + item.GetHashCode();
				}
				public override bool Equals(object obj) {
					ComplexKey compare = obj as ComplexKey;
					if(compare == null) return base.Equals(obj);
					return searchArea == compare.searchArea && item == compare.item && searchHiddenItems == compare.searchHiddenItems;
				}
			}
			Hashtable table = new Hashtable();
			public BaseLayoutItem CheckItem(BaseLayoutItem searchArea, BaseLayoutItem item, bool searchHiddenItems) {
				ComplexKey key = new ComplexKey(searchArea, item, searchHiddenItems);
				if(table.ContainsKey(key)) return table[key] as BaseLayoutItem;
				return null;
			}
			public void AddItem(BaseLayoutItem searchArea, BaseLayoutItem item, bool searchHiddenItems, BaseLayoutItem result) {
				ComplexKey key = new ComplexKey(searchArea, item, searchHiddenItems);
				if(!table.ContainsKey(key)) table.Add(key, result);
			}
		}
		internal FindResizerParentCache cache = new FindResizerParentCache();
		protected internal BaseLayoutItem FindResizerParent(BaseLayoutItem searchArea, BaseLayoutItem item, bool searchHiddenItems) {
			BaseLayoutItem result = cache.CheckItem(searchArea, item, searchHiddenItems);
			if(result == null) {
				ResizerParentSearcherForVisibility helper = new ResizerParentSearcherForVisibility(item, searchHiddenItems);
				searchArea.Accept(helper);
				result = helper.ResultGroup;
				cache.AddItem(searchArea, item, searchHiddenItems, result);
			}
			return result;
		}
		protected internal BaseLayoutItem FindGroupForModifiedGroup(LayoutGroup affectedgroup, BaseLayoutItem resizerItem) {
			ResizerUpdateHelper helper = new ResizerUpdateHelper(affectedgroup);
			resizerItem.Accept(helper);
			return helper.ResultGroup;
		}
		protected void ReplaceResizeTreeNode(BaseLayoutItem resizerComponent, BaseLayoutItem oldNode, BaseLayoutItem newNode) {
			BaseLayoutItem resizerParent = FindResizerParentToCompleteTree(resizerComponent, oldNode, false);
			TableGroupResizeGroup tgrg = resizerParent as TableGroupResizeGroup;
			GroupResizeGroup grg = resizerParent as GroupResizeGroup;
			ResizeGroup resizeGroup = resizerParent as ResizeGroup;
			if(tgrg != null) {
				BaseLayoutItem key = null;
				foreach(var item in tgrg.groupsToResize) {
					if(item.Value.Equals(oldNode)) {
						key = item.Key;
					}
				}
				tgrg.groupsToResize.Remove(key);
				tgrg.groupsToResize.Add(key, newNode);
			}
			if(grg != null && tgrg == null) grg.Item = newNode;
			if(resizeGroup != null) {
				if(resizeGroup.Item1 == oldNode) resizeGroup.SetItems(newNode, resizeGroup.RealItem2);
				if(resizeGroup.Item2 == oldNode) resizeGroup.SetItems(resizeGroup.RealItem1, newNode);
			}
		}
		protected void UpdateResizerCore(BaseLayoutItem resizerComponent, LayoutGroup affectedGroup, LayoutType layoutType) {
			if(resizerComponent != null) {
				BaseLayoutItem groupForModifiedGroup = FindGroupForModifiedGroup(affectedGroup, resizerComponent);
				GroupResizeGroup groupResizeGroupForModifiedGroup = groupForModifiedGroup as GroupResizeGroup;
				BaseLayoutItem item;
				if(affectedGroup == null || (affectedGroup.Items.Count == 0 && affectedGroup.LayoutMode != LayoutMode.Table)) item = null;
				else {
					if(affectedGroup.LayoutMode != LayoutMode.Regular) {
						if(affectedGroup.LayoutMode == LayoutMode.Flow) ReplaceResizeTreeNode(resizerComponent, groupForModifiedGroup, new FlowGroupResizeGroup(layoutType, affectedGroup));
						if(affectedGroup.LayoutMode == LayoutMode.Table) ReplaceResizeTreeNode(resizerComponent, groupForModifiedGroup, new TableGroupResizeGroup(layoutType, affectedGroup, affectedGroup.Items, this));
						return;
					} else {
						item = GroupItems(layoutType, affectedGroup.Items.ConvertToTypedList());
						if((groupResizeGroupForModifiedGroup is FlowGroupResizeGroup || groupResizeGroupForModifiedGroup is TableGroupResizeGroup) && item != null) {
							if(affectedGroup.ParentTabbedGroup != null)
								ReplaceResizeTreeNode(resizerComponent, groupForModifiedGroup, new FakeGroup(layoutType, item, affectedGroup));
							else
								ReplaceResizeTreeNode(resizerComponent, groupForModifiedGroup, new GroupResizeGroup(layoutType, item, affectedGroup));
							return;
						}
					}
				}
				if(groupResizeGroupForModifiedGroup != null) {
					if(item == null) {
						if(affectedGroup.Items.Count > 0) {
							if(item == null) {
								RestoreBrokenLayoutCore2(affectedGroup);
								Fix5ItemsProblem(affectedGroup.Items.ConvertToTypedList());
							}
							item = GroupItems(layoutType, affectedGroup.Items.ConvertToTypedList());
							if(item == null) throw new LayoutControlInternalException("Internal error 2123");
						}
						ReplaceResizeTreeNode(resizerComponent, groupResizeGroupForModifiedGroup, affectedGroup);
					} else
						groupResizeGroupForModifiedGroup.Item = item;
				} else {
					if(item != null) {
						if(affectedGroup.ParentTabbedGroup == null) {
							switch(affectedGroup.LayoutMode) {
								case LayoutMode.Regular:
									ReplaceResizeTreeNode(resizerComponent, groupForModifiedGroup, new GroupResizeGroup(layoutType, item, affectedGroup));
									break;
								case LayoutMode.Flow:
									ReplaceResizeTreeNode(resizerComponent, groupForModifiedGroup, new FlowGroupResizeGroup(layoutType, affectedGroup));
									break;
								case LayoutMode.Table:
									ReplaceResizeTreeNode(resizerComponent, groupForModifiedGroup, new TableGroupResizeGroup(layoutType, affectedGroup, affectedGroup.Items, this));
									break;
							}
						} else
							ReplaceResizeTreeNode(resizerComponent, groupForModifiedGroup, new FakeGroup(layoutType, item, affectedGroup));
					} else
						ReplaceResizeTreeNode(resizerComponent, groupForModifiedGroup, affectedGroup);
				}
			}
		}
		public virtual void CreateNewResizing() {
			resultH = null;
			resultV = null;
			hash_Horizontal.Clear();
			hash_Vertical.Clear();
			BaseLayoutItem tresultH = null, tresultV = null;
			tresultH = GroupItems(LayoutType.Horizontal, new List<BaseLayoutItem>(new BaseLayoutItem[] { group }));
			tresultV = GroupItems(LayoutType.Vertical, new List<BaseLayoutItem>(new BaseLayoutItem[] { group }));
			if(tresultH != null) resultH = tresultH;
			if(tresultV != null) resultV = tresultV;
			foreach(var item in hash_Horizontal) cache.AddItem(resultH, item.Key, true, item.Value);
			foreach(var item in hash_Vertical) cache.AddItem(resultV, item.Key, true, item.Value);
			UpdateConstraints();
		}
		protected void CalcFakeItemMaxSize(bool isFirstItem, ResizeGroup resizeGroup, ResizeStatusInfo info) {
			HorizontalResizeGroup hrg = resizeGroup as HorizontalResizeGroup;
			if(hrg == null) return;
			FakeEmptySpaceItem fakeItem = info.FakeItem;
			if(fakeItem == null) return;
			bool isFirstStage = fakeItem.MaxSize == Size.Empty;
			if(!isFirstStage) return;
			if(info.Item is ResizeGroup) {
				if(hrg.layoutType == LayoutType.Horizontal) fakeItem.MaxSize = new Size(2, 0);
				if(hrg.layoutType == LayoutType.Vertical) fakeItem.MaxSize = new Size(0, 2);
			} else {
				if(hrg.layoutType == LayoutType.Horizontal) fakeItem.MaxSize = new Size(1, 0);
				if(hrg.layoutType == LayoutType.Vertical) fakeItem.MaxSize = new Size(0, 1);
			}
		}
		protected void ProcessResizeGroupItem(bool isFirstItem, ResizeGroup resizeGroup, ResizeStatusInfo info) {
			if(info.FakeItem == null) {
				resizeGroup.SetItemStatus(isFirstItem, info.ResizeStatus);
				if(info.ResizeStatus == ResizeItemStatus.Hidden) info.FakeItem = resizeGroup.GetItem(isFirstItem) as FakeEmptySpaceItem;
				CalcFakeItemMaxSize(isFirstItem, resizeGroup, info);
			} else {
				resizeGroup.SetResizeItemStatus(isFirstItem, info.ResizeStatus, info.FakeItem);
				CalcFakeItemMaxSize(isFirstItem, resizeGroup, info);
			}
		}
		protected bool CalcResizeItem(ResizeGroup resizeGruop, ResizeStatusInfo info) {
			if(resizeGruop == null) throw new LayoutControlInternalException("CalcResizeItem error 1");
			if(resizeGruop.RealItem1 == info.Item) return true;
			if(resizeGruop.RealItem2 == info.Item) return false;
			if(ResizerParentSearcherForVisibility.IsGroupForGroupResizeGroup(info.Item, resizeGruop.RealItem1 as GroupResizeGroup)) return true;
			if(ResizerParentSearcherForVisibility.IsGroupForGroupResizeGroup(info.Item, resizeGruop.RealItem2 as GroupResizeGroup)) return false;
			throw new LayoutControlInternalException("CalcResizeItem error 2");
		}
		protected void SetItemResizeStatus(ResizeStatusInfo info) {
			info.ResizeParent = FindResizerParent(info.TargetResizeItem, info.Item, true);
			if(info.ResizeParent == null) return;
			ResizeGroup resizeGroup = info.ResizeParent as ResizeGroup;
			GroupResizeGroup grg = info.ResizeParent as GroupResizeGroup;
			if(info.ResizeStatus == ResizeItemStatus.Normal && resizeGroup != null && resizeGroup.Status == ResizeItemStatus.Hidden) SetParentResizeStatus(info);
			if(resizeGroup != null) {
				ProcessResizeGroupItem(CalcResizeItem(resizeGroup, info), resizeGroup, info);
			}
			if(grg != null) {
				if(grg.Group == info.Item || ResizerParentSearcherForVisibility.IsGroupForGroupResizeGroup(info.Item as LayoutGroup, grg.Item as GroupResizeGroup)
					|| ResizerParentSearcherForVisibility.IsTabbedGroupForGroupResizeGroup(info.Item as TabbedGroup, grg.Item as GroupResizeGroup)
					) {
					grg.GroupStatus = info.ResizeStatus;
					return;
				}
			}
			if(info.ResizeStatus == ResizeItemStatus.Hidden && resizeGroup != null && resizeGroup.Status == ResizeItemStatus.Hidden) SetParentResizeStatus(info);
		}
		private void SetParentResizeStatus(ResizeStatusInfo info) {
			ResizeStatusInfo parentStatus = new ResizeStatusInfo();
			parentStatus.TargetResizeItem = info.TargetResizeItem;
			parentStatus.Item = info.ResizeParent;
			parentStatus.ResizeStatus = info.ResizeStatus;
			SetItemResizeStatus(parentStatus);
		}
		protected void SetResizeStatus(ResizeStatusInfo info) {
			info.TargetResizeItem = resultH;
			SetItemResizeStatus(info);
			info.TargetResizeItem = resultV;
			SetItemResizeStatus(info);
		}
		internal Rectangle GetHiddenItemRealBounds(BaseLayoutItem item) {
			ResizeStatusInfo info = new ResizeStatusInfo();
			info.Item = item;
			info.TargetResizeItem = resultH;
			info.ResizeParent = FindResizerParent(info.TargetResizeItem, info.Item, true);
			if(info.ResizeParent == null) return Rectangle.Empty;
			ResizeGroup resizeGruop = info.ResizeParent as ResizeGroup;
			GroupResizeGroup grg = info.ResizeParent as GroupResizeGroup;
			if(resizeGruop != null) {
				if(resizeGruop.RealItem1 == item) return resizeGruop.EmptyItem1.Bounds;
				if(resizeGruop.RealItem2 == item) return resizeGruop.EmptyItem2.Bounds;
			}
			if(grg != null) {
			}
			return Rectangle.Empty;
		}
		internal void MarkRemoved(BaseLayoutItem item) {
			ResizeStatusInfo info = new ResizeStatusInfo();
			info.Item = item;
			info.ResizeStatus = ResizeItemStatus.Hidden;
			SetResizeStatus(info);
		}
		internal void MarkRestored(BaseLayoutItem item) {
			ResizeStatusInfo info = new ResizeStatusInfo();
			info.Item = item;
			info.ResizeStatus = ResizeItemStatus.Normal;
			SetResizeStatus(info);
		}
		public void UpdateProportions(SplitterItem allowResetElementsInSplitterGroupsSplitterItem) {
			try {
				UpdateProportionsInternal(resultH, allowResetElementsInSplitterGroupsSplitterItem);
				UpdateProportionsInternal(resultV, allowResetElementsInSplitterGroupsSplitterItem);
				UpdateOriginalSize();
			} catch { }
		}
		public void UpdateConstraints() {
			try {
				UpdateConstraintsInternal(resultH);
				UpdateConstraintsInternal(resultV);
			} catch { }
		}
		protected void UpdateProportionsInternal(BaseLayoutItem item, SplitterItem allowResetElementsInSplitterGroupsSplitterItem) {
			TableGroupResizeGroup tableGroupResizeGroup = item as TableGroupResizeGroup;
			if(tableGroupResizeGroup != null && tableGroupResizeGroup.groupsToResize.Count != 0) {
				foreach(BaseLayoutItem bli in tableGroupResizeGroup.groupsToResize.Values) {
					UpdateProportionsInternal(bli, allowResetElementsInSplitterGroupsSplitterItem);
				}
			}
			GroupResizeGroup groupResizeGroup = item as GroupResizeGroup;
			ResizeGroup resizeGroup = item as ResizeGroup;
			if(resizeGroup != null) {
				if(allowResetElementsInSplitterGroupsSplitterItem != null) {
					HorizontalResizeGroup hrgr = resizeGroup as HorizontalResizeGroup;
					SplitterHorizontalResizeGroup shg = resizeGroup as SplitterHorizontalResizeGroup;
					SplitterHorizontalResizeGroup shgI1 = resizeGroup.Item1 as SplitterHorizontalResizeGroup;
					SplitterHorizontalResizeGroup shgI2 = resizeGroup.Item2 as SplitterHorizontalResizeGroup;
					if(shg != null && shg.SplitterItem == allowResetElementsInSplitterGroupsSplitterItem) { shg.UpdateProportion(); return; }
					if(shgI1 != null && hrgr != null && shgI1.SplitterItem == allowResetElementsInSplitterGroupsSplitterItem) { shgI1.UpdateProportion(); hrgr.UpdateProportion(); return; }
					if(shgI2 != null && hrgr != null && shgI2.SplitterItem == allowResetElementsInSplitterGroupsSplitterItem) { shgI2.UpdateProportion(); hrgr.UpdateProportion(); return; }
				}
				UpdateProportionsInternal(resizeGroup.Item1, allowResetElementsInSplitterGroupsSplitterItem);
				UpdateProportionsInternal(resizeGroup.Item2, allowResetElementsInSplitterGroupsSplitterItem);
				HorizontalResizeGroup hrg = resizeGroup as HorizontalResizeGroup;
				if(hrg != null) {
					hrg.UpdateProportion();
				}
			}
			if(groupResizeGroup != null) {
				UpdateProportionsInternal(groupResizeGroup.Item, allowResetElementsInSplitterGroupsSplitterItem);
			}
		}
		protected internal static void UpdateConstraintsInternal(BaseLayoutItem item) {
			TableGroupResizeGroup tgrg = item as TableGroupResizeGroup;
			GroupResizeGroup groupResizeGroup = item as GroupResizeGroup;
			ResizeGroup resizeGroup = item as ResizeGroup;
			if(resizeGroup != null) {
				UpdateConstraintsInternal(resizeGroup.Item1);
				UpdateConstraintsInternal(resizeGroup.Item2);
				resizeGroup.UpdateConstraints();
			}
			if(groupResizeGroup != null && tgrg == null) {
				UpdateConstraintsInternal(groupResizeGroup.Item);
				groupResizeGroup.UpdateConstraints();
			}
			if(tgrg != null) {
				tgrg.UpdateTableConstraints();
			}
		}
		public Size MaxSize {
			get {
				return new Size(resultH.MaxSize.Width, resultV.MaxSize.Height);
			}
		}
		public Size MinSize {
			get {
				return new Size(resultH.MinSize.Width, resultV.MinSize.Height);
			}
		}
		Size CorrectSize(Size newSize, BaseLayoutItem resultHp, BaseLayoutItem resultVp) {
			if(newSize.Width < resultHp.MinSize.Width) newSize.Width = resultHp.MinSize.Width;
			newSize.Width -= resultHp.Width;
			if(newSize.Height < resultVp.MinSize.Height) newSize.Height = resultVp.MinSize.Height;
			newSize.Height -= resultVp.Height;
			return newSize;
		}
		public void SizeIt(Size newSize) {
#if DEBUGTEST
			System.Diagnostics.Debug.Assert(group.Parent == null);
#endif
			SizeIt(newSize, true);
		}
		public void SizeIt(Size newSize, bool force) {
			SizeIt(newSize, group, force);
		}
		protected BaseLayoutItem CheckResizeParent(BaseLayoutItem res, BaseLayoutItem target) {
			GroupResizeGroup grg = res as GroupResizeGroup;
			if(grg != null && grg.Group == target) return res;
			else return null;
		}
		protected BaseLayoutItem CorrectResizeParent(BaseLayoutItem res, BaseLayoutItem target) {
			if(CheckResizeParent(res, target) != null) return res;
			ResizeGroup rg = res as ResizeGroup;
			if(rg != null && CheckResizeParent(rg.RealItem1, target) != null) return rg.RealItem1;
			if(rg != null && CheckResizeParent(rg.RealItem2, target) != null) return rg.RealItem2;
			GroupResizeGroup grg = res as GroupResizeGroup;
			if(grg != null) return CheckResizeParent(grg.Item, target);
			return null;
		}
		public void SizeIt(Size newSize, BaseLayoutItem target, bool force) {
			BaseLayoutItem resultHp, resultVp;
			if(target == group) {
				resultHp = resultH;
				resultVp = resultV;
			} else {
				resultHp = CorrectResizeParent(FindResizerParent(resultH, target, false), target);
				resultVp = CorrectResizeParent(FindResizerParent(resultV, target, false), target);
			}
			SizeItCore(newSize, force, resultHp, resultVp);
		}
		protected void SizeItCore(Size newSize, bool force, BaseLayoutItem resultHp, BaseLayoutItem resultVp) {
			Size size = CorrectSize(newSize, resultHp, resultVp);
			if(size.Width != 0 || force)
				resultHp.ChangeSize(size.Width, LayoutType.Horizontal);
			if(group != null && group.Owner != null && !group.Owner.OptionsView.FitControlsToDisplayAreaHeight) return;
			if(size.Height != 0 || force)
				resultVp.ChangeSize(size.Height, LayoutType.Vertical);
		}
		Size CalculateDif(BaseLayoutItem item, Size newSize) {
			Size size = item.Size;
			size.Width = newSize.Width - size.Width;
			size.Height = newSize.Height - size.Height;
			return size;
		}
		public void ExactResize(Size newSize) {
			Size size = new Size(newSize.Width - resultH.Width, newSize.Height - resultV.Height);
			resultH.ChangeItemSize(null, size.Width, LayoutType.Horizontal, new Constraints(newSize.Width));
			UpdateConstraints();
			resultV.ChangeItemSize(null, size.Height, LayoutType.Vertical, new Constraints(newSize.Height));
		}
		public void SafeSetSize(BaseLayoutItem item, Size newSize) {
			Size size = CalculateDif(item, newSize);
			resultH.ChangeItemSize(item, size.Width, LayoutType.Horizontal, new Constraints(group.PreferredSize.Width));
			UpdateConstraints();
			resultV.ChangeItemSize(item, size.Height, LayoutType.Vertical, new Constraints(group.Owner != null ? (group.Owner.OptionsView.FitControlsToDisplayAreaHeight ? group.PreferredSize.Height : 0) : group.PreferredSize.Height));
		}
	}
	public class ResizeStatusInfo {
		BaseLayoutItem item;
		FakeEmptySpaceItem fakeItem;
		BaseLayoutItem resizeParent;
		BaseLayoutItem targetResizeItem;
		ResizeItemStatus resizeStatus;
		public BaseLayoutItem Item { get { return item; } set { item = value; } }
		public BaseLayoutItem ResizeParent { get { return resizeParent; } set { resizeParent = value; } }
		public FakeEmptySpaceItem FakeItem { get { return fakeItem; } set { fakeItem = value; } }
		public ResizeItemStatus ResizeStatus { get { return resizeStatus; } set { resizeStatus = value; } }
		public BaseLayoutItem TargetResizeItem { get { return targetResizeItem; } set { targetResizeItem = value; } }
	}
}
