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
using System.Text;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraPrinting.Native;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Utils.StoredObjects;
#if SL
using DevExpress.Xpf.Collections;
#endif
namespace DevExpress.XtraPrinting {
#if DEBUGTEST
	[System.Diagnostics.DebuggerDisplay(@"\{{GetType().FullName,nq}, PageIndex = {PageIndex}, Indices = {Indices}}")]
#endif
	public class BrickPagePair {
		#region // inner classes
		class BrickPagePairStoreAgent : StoreAgent<BrickPagePair> {
			public override void StoreObject(IRepositoryProvider provider, object obj, System.IO.BinaryWriter writer) {
				BrickPagePair pair = (BrickPagePair)obj;
				writer.Write(pair.brickIndices.Length);
				for(int i = 0; i < pair.brickIndices.Length; i++)
					writer.Write(pair.brickIndices[i]);
				writer.Write(pair.pageIndex);
				writer.Write(pair.PageID);
				writer.WriteRectangleF(pair.brickBounds);
			}
			public override object RestoreObject(IRepositoryProvider provider, System.IO.BinaryReader reader) {
				BrickPagePair pair = new BrickPagePair();
				int length = reader.ReadInt32();
				pair.brickIndices = new int[length];
				for(int i = 0; i < length; i++)
					pair.brickIndices[i] = reader.ReadInt32();
				pair.pageIndex = reader.ReadInt32();
				pair.PageID = reader.ReadInt32();
				pair.brickBounds = reader.ReadRectangleF();
				return pair;
			}
		}
		#endregion
		#region //static
		static BrickPagePair() {
			StoreAgentRepository.Register<BrickPagePair>(new BrickPagePairStoreAgent());
		}
		#endregion
		internal const int UndefinedPageIndex = -1;
		static BrickPagePair empty = new BrickPagePair(BrickPagePairHelper.EmptyIndices, UndefinedPageIndex, UndefinedPageIndex);
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("BrickPagePairEmpty")]
#endif
		public static BrickPagePair Empty { get { return empty; } }
		public static BrickPagePair Create(Brick brick, Page page) {
			return Create(page.GetIndicesByBrick(brick), page);
		}
		public static BrickPagePair Create(int[] indices, Page page, RectangleF brickBounds) {
			if(page == null)
				return BrickPagePair.Empty;
			return new BrickPagePair(indices, page.Index, page.ID, brickBounds);
		}
		public static BrickPagePair Create(int[] indices, int pageIndex, int pageID, RectangleF brickBounds) {
			if(pageIndex == UndefinedPageIndex)
				return BrickPagePair.Empty;
			return new BrickPagePair(indices, pageIndex, pageID, brickBounds);
		}
		public static BrickPagePair Create(int[] indices, Page page) {
			return Create(indices, page, RectangleF.Empty);
		}
		int[] brickIndices;
		int pageIndex;
		RectangleF brickBounds;
		[DXHelpExclude(true)]
		public int[] BrickIndices { get { return brickIndices; } }
		internal string Indices {
			get {
				return BrickPagePairHelper.IndicesFromArray(brickIndices);
			}
		}
		[Obsolete("Use the Page.GetBrickByIndices method instead."),
		EditorBrowsable(EditorBrowsableState.Never)]
		public Brick Brick { get { throw new NotSupportedException(); } }
		[Obsolete("Use the PageIndex property instead."),
		EditorBrowsable(EditorBrowsableState.Never)]
		public Page Page { get { throw new NotSupportedException(); } }
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("BrickPagePairPageIndex")]
#endif
		public int PageIndex { get { return pageIndex; } }
		internal Int32 PageID { get; private set; }
		protected BrickPagePair(int[] brickIndices, int pageIndex, Int32 pageID)
			: this(brickIndices, pageIndex, pageID, RectangleF.Empty) {
		}
		protected BrickPagePair(int[] brickIndices, int pageIndex, Int32 pageID, RectangleF brickBounds) {
			this.brickIndices = brickIndices;
			this.pageIndex = pageIndex;
			this.PageID = pageID;
			this.brickBounds = brickBounds;
		}
		protected BrickPagePair() {
		}
		public virtual RectangleF GetBrickBounds(PageList pages) {
			if(brickBounds == RectangleF.Empty) {
				Page page = GetPage(pages);
				if(page != null)
					brickBounds = page.GetBrickBounds(BrickIndices);
			}
			return brickBounds;
		}
		public virtual Page GetPage(PageList pages) {
			Page page;
			if(pages.TryGetPageByIndex(PageIndex, out page)) {
				if(PageID == page.ID)
					return page;
			}
			if(pages.TryGetPageByID(PageID, out page)) {
				pageIndex = page.Index;
				return page;
			}
			return null;
		}
		public override bool Equals(object obj) {
			BrickPagePair pair = obj as BrickPagePair;
			return pair != null && ArrayAreEqual(this.brickIndices, pair.brickIndices) && Comparer.Equals(this.pageIndex, pair.pageIndex);
		}
		static bool ArrayAreEqual(int[] arr1, int[] arr2) {
			if(arr1.Length != arr2.Length)
				return false;
			for(int i = 0; i < arr1.Length; i++)
				if(arr1[i] != arr2[i])
					return false;
			return true;
		}
		public override int GetHashCode() {
			int[] values = new int[brickIndices.Length + 1];
			Array.Copy(brickIndices, values, values.Length - 1);
			values[values.Length - 1] = PageIndex;
			return HashCodeHelper.CalcHashCode2(values); 
		}
	}
}
namespace DevExpress.XtraPrinting.Native {
	public static class BrickPagePairHelper {
		public static readonly int[] EmptyIndices = new int[0];
		public const char IndexSeparator = ',';
		public static string IndicesFromArray(int[] indexArray) {
			StringBuilder builder = new StringBuilder();
			for(int i = 0; i < indexArray.Length; i++) {
				if(i > 0)
					builder.Append(IndexSeparator);
				builder.Append(indexArray[i].ToString());
			}
			return builder.ToString();
		}
		public static int[] ParseIndices(string indices) {
			string[] items = indices.Split(IndexSeparator);
			if(items.Length == 1 && string.IsNullOrEmpty(items[0]))
				return EmptyIndices;
			int[] brickIndices = new int[items.Length];
			for(int i = 0; i < items.Length; i++) {
				brickIndices[i] = int.Parse(items[i]);
			}
			return brickIndices;
		}
		public static Brick GetBrick(this BrickPagePair pair, PageList pages) {
			return pair.GetPage(pages).GetBrickByIndices(pair.BrickIndices) as Brick;
		}
		public static int[] GetIndicesByBrick(this Page page, Brick brick) {
			if(page == null)
				return EmptyIndices;
			NestedBrickIterator iterator = new NestedBrickIterator(page.InnerBrickList);
			while(iterator.MoveNext()) {
				if(iterator.CurrentBrick.Equals(brick))
					return iterator.GetCurrentBrickIndices();
			}
			return EmptyIndices;
		}
		public static BrickBase GetBrickByIndices(this Page page, int[] indices) {
			if(page == null)
				return null;
			BrickBase brickBase = page;
			for(int i = 0; i < indices.Length; i++) {
				int index = indices[i];
				if(index >= brickBase.InnerBrickList.Count)
					break;
				brickBase = (BrickBase)brickBase.InnerBrickList[index];
			}
			return brickBase;
		}
		public static RectangleF GetBrickBounds(this Page page, int[] indices) {
			if(indices.Length == 0) throw new ArgumentException();
			PointF pos = PointF.Empty;
			BrickBase brick = page;
			for(int i = 0; i < indices.Length - 1; i++) {
				int index = indices[i];
				BrickBase nextBrick = (BrickBase)brick.InnerBrickList[index];
				PointF location = nextBrick.Location;
				pos.X += location.X + brick.InnerBrickListOffset.X;
				pos.Y += location.Y + brick.InnerBrickListOffset.Y;
				brick = nextBrick;
			}
			pos.X += brick.InnerBrickListOffset.X;
			pos.Y += brick.InnerBrickListOffset.Y;
			RectangleF rect = ((BrickBase)brick.InnerBrickList[indices[indices.Length - 1]]).Rect;
			rect.Offset(pos);
			return rect;
		}
	}
}
