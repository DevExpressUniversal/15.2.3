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
using System.Text;
using System.Collections;
using System.Drawing;
#if SL
using DevExpress.Xpf.Collections;
#endif
namespace DevExpress.XtraPrinting.Native {
	public class BrickPagePairComparer : IComparer {
		PageList pages;
		public BrickPagePairComparer(PageList pages) {
			this.pages = pages;
		}
		public int Compare(object x, object y) {
			BrickPagePair xPair = x as BrickPagePair;
			BrickPagePair yPair = y as BrickPagePair;
			int res = xPair.PageIndex.CompareTo(yPair.PageIndex);
			RectangleF xBrickBounds = RectangleF.Empty;
			RectangleF yBrickBounds = RectangleF.Empty;
			if(res == 0) {
				xBrickBounds = xPair.GetBrickBounds(pages);
				yBrickBounds = yPair.GetBrickBounds(pages);
				int xTop = Convert.ToInt32(xBrickBounds.Y);
				int yTop = Convert.ToInt32(yBrickBounds.Y);
				res = xTop.CompareTo(yTop);
			}
			if(res == 0) {
				int xLeft = Convert.ToInt32(xBrickBounds.X);
				int yLeft = Convert.ToInt32(yBrickBounds.X);
				res = xLeft.CompareTo(yLeft);
			}
			return res;
		}
	}
	public class BrickPagePairCollection : CollectionBase {
		#region inner classes
		class BPPIdx : IComparable<BPPIdx> {
			readonly BrickPagePair bpp;
			readonly int idx;
			public int Idx { get { return idx; } }
			public BPPIdx(BrickPagePair bpp, int idx) {
				this.bpp = bpp;
				this.idx = idx;
			}
			public int CompareTo(BPPIdx other) {
				if(bpp.PageIndex != other.bpp.PageIndex)
					return bpp.PageIndex - other.bpp.PageIndex;
				if(bpp.BrickIndices.Length != other.bpp.BrickIndices.Length)
					return bpp.BrickIndices.Length - other.bpp.BrickIndices.Length;
				for(int i = 0; i < bpp.BrickIndices.Length; i++) {
					int diff = bpp.BrickIndices[i] - other.bpp.BrickIndices[i];
					if(diff != 0)
						return diff;
				}
				return 0;
			}
		}
		#endregion
		List<BPPIdx> indices = null;
		List<BPPIdx> Indices {
			get {
				if(indices == null) {
					indices = new List<BPPIdx>();
					for(int i = 0; i < Count; i++)
						indices.Add(new BPPIdx(this[i], i));
					indices.Sort();
				}
				return indices;
			}
		}
		void IndicesAdd(BrickPagePair bpp, int idx) {
			BPPIdx bppidx = new BPPIdx(bpp, idx);
			int idxFound = Indices.BinarySearch(bppidx);
			if(idxFound < 0)
				indices.Insert(~idxFound, bppidx);
		}
		public BrickPagePair this[int index] {
			get { return (BrickPagePair)InnerList[index]; }
		}
		public BrickPagePairCollection() {
		}
		public bool Contains(BrickPagePair pair) {
			return IndexOf(pair) >= 0;
		}
		public int Add(BrickPagePair pair) {
		   int index = IndexOf(pair);
		   if(index < 0) {
			   index = InnerList.Add(pair);
			   IndicesAdd(pair, index);
		   }
		   return index;
		}
		public void Remove(BrickPagePair pair) {
			if(Contains(pair)) {
				InnerList.Remove(pair);
				indices = null;
			}
		}
		public int IndexOf(BrickPagePair pair) {
			int idx = Indices.BinarySearch(new BPPIdx(pair, -1));
			return idx >= 0 ? indices[idx].Idx : -1;
		}
		public void Sort(IComparer sortComparer) {
			if(sortComparer != null) {
				InnerList.Sort(sortComparer);
				indices = null;
			}
		}
		protected override void OnClear() {
			base.OnClear();
			indices = null;
		}
		public Brick[] GetBricks(PageList pages, int pageIndex) {
			List<Brick> bricks = new List<Brick>();
			for(int i = 0; i < Count; i++) {
				if(this[i].PageIndex == pageIndex) {
					Brick brick = this[i].GetBrick(pages);
					bricks.Add(brick);
				}
			}
			return bricks.ToArray();
		}
	}
}
