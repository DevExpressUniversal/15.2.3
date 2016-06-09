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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using DevExpress.Printing.Core.Native;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraPrinting.Native;
#if SL
using DevExpress.Xpf.Collections;
#endif
namespace DevExpress.XtraPrinting {
	public class BookmarkNode : IBookmarkNode, IXtraPartlyDeserializable, IXtraSupportShouldSerializeCollectionItem, IXtraSupportDeserializeCollectionItem {
		string text = string.Empty;
		BrickPagePair pair;
		readonly IBookmarkNodeCollection nodes;
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("BookmarkNodeNodes")]
#endif
		[XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, false, 0, XtraSerializationFlags.DeserializeCollectionItemBeforeCallSetIndex)]
		public IBookmarkNodeCollection Nodes {
			get { return nodes; }
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("BookmarkNodeText")]
#endif
		[XtraSerializableProperty]
		public string Text {
			get { return text; }
			set { text = value; }
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("BookmarkNodePair")]
#endif
		public BrickPagePair Pair {
			get { return pair; }
			internal set { pair = value; }
		}
		[Obsolete("Use the Page.GetBrickByIndices method instead."),
		EditorBrowsable(EditorBrowsableState.Never)]
		public Brick Brick {
			get { throw new NotSupportedException(); }
		}
		[DefaultValue("")]
		[XtraSerializableProperty]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string Indices {
			get { return pair.Indices; }
		}
		[DefaultValue(BrickPagePair.UndefinedPageIndex)]
		[XtraSerializableProperty]
		public int PageIndex {
			get { return pair.PageIndex; }
		}
		[Obsolete("Use the PageIndex property instead."),
		EditorBrowsable(EditorBrowsableState.Never)]
		public Page Page {
			get { throw new NotSupportedException(); }
		}
		public BookmarkNode(string text, Brick brick, Page page)
			: this(text, BrickPagePair.Create(brick, page)) {
		}
		public BookmarkNode(string text)
			: this(text, BrickPagePair.Empty) {
		}
		public BookmarkNode(string text, BrickPagePair bpPair) {
			Text = text;
			Pair = bpPair;
			nodes = CreateBookmarkNodeCollection();
		}
		[Obsolete("Use CreateBookmarkNodeCollection instead")]
		protected virtual IBookmarkNodeCollection CreateBookmarkodeCollection() {
			return CreateBookmarkNodeCollection();
		}
		protected virtual IBookmarkNodeCollection CreateBookmarkNodeCollection() {
			return new BookmarkNodeCollection();
		}
		protected internal virtual int GetPageRangeIndex(int[] indices) {
			return Array.BinarySearch<int>(indices, PageIndex);
		}
		internal bool IsValid(Document document) {
			return PageIndex == BrickPagePair.UndefinedPageIndex || Pair.GetPage(document.Pages) != null;
		}
		#region Serialization
		bool IXtraSupportShouldSerializeCollectionItem.ShouldSerializeCollectionItem(XtraItemEventArgs e) {
			return ((BookmarkNode)e.Item.Value).IsValid((Document)e.RootObject);
		}
		void IXtraPartlyDeserializable.Deserialize(object rootObject, IXtraPropertyCollection properties) {
			pair = ((Document)rootObject).CreateBrickPagePair(properties["PageIndex"], properties["Indices"]);
		}
		void IXtraSupportDeserializeCollectionItem.SetIndexCollectionItem(string propertyName, XtraSetItemIndexEventArgs e) {
			if(propertyName == PrintingSystemSerializationNames.Nodes)
				Nodes.Add((BookmarkNode)e.Item.Value);
		}
		object IXtraSupportDeserializeCollectionItem.CreateCollectionItem(string propertyName, XtraItemEventArgs e) {
			if(propertyName == PrintingSystemSerializationNames.Nodes)
				return new BookmarkNode(string.Empty, null, null);
			return null;
		}
		#endregion
		#region IBookmarkNode Members
		int IBookmarkNode.PageIndex {
			get { return PageIndex; }
		}
		string IBookmarkNode.Text {
			get { return Text; }
		}
		IEnumerable<IBookmarkNode> IBookmarkNode.Nodes {
			get { return Nodes.Cast<IBookmarkNode>(); }
		}
		#endregion
	}
	public class RootBookmarkNode : BookmarkNode {
		public RootBookmarkNode(string text)
			: base(text, null, null) {
		}
		protected internal override int GetPageRangeIndex(int[] indices) {
			return indices.Length > 0 ? 0 : -1;
		}
	}
	public interface IBookmarkNodeCollection : IList {
		new BookmarkNode this[int index] { get; }
		void Add(BookmarkNode item);
	}
	static class BookmarkNodeCollectionExtensions {
		public static IList<BookmarkNode> GetPageNodes(this IBookmarkNodeCollection collection, Page page) {
			List<BookmarkNode> nodes = new List<BookmarkNode>();
			foreach(BookmarkNode node in collection) {
				if(node.Pair.PageID == page.ID)
					nodes.Add(node);
			}
			return nodes;
		}
		public static void InsertNodes(this IBookmarkNodeCollection collection, int[] prevPages, IList<BookmarkNode> nodes) {
			int index = prevPages.Length > 0 ? GetNodeIndex(collection, prevPages) : 0;
			for(int i = 0; i < nodes.Count; i++)
				collection.Insert(index + i, nodes[i]);
		}
		static int GetNodeIndex(IBookmarkNodeCollection collection, int[] prevPages) {
			int currentID = -1;
			for(int i = 0; i < collection.Count; i++) {
				if(collection[i].Pair.PageID == currentID)
					continue;
				currentID = collection[i].Pair.PageID;
				if(Array.IndexOf<int>(prevPages, currentID) < 0)
					return i;
			}
			return collection.Count;
		}
	}
	public class BookmarkNodeCollection : Collection<BookmarkNode>, IBookmarkNodeCollection {
		protected override void ClearItems() {
			base.ClearItems();
			foreach(BookmarkNode item in this)
				item.Nodes.Clear();
		}
	}
}
