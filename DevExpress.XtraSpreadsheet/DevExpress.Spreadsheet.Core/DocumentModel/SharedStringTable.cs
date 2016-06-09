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

using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Utils;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using DevExpress.Compatibility.System;
namespace DevExpress.XtraSpreadsheet.Model {
	#region ISharedStringItem
	public interface ISharedStringItem {
		string Content { get; set; }
		int RunsCount { get; }
		void CheckIntegrity();
	}
	#endregion
	#region PlaintTextStringItem
	public class PlainTextStringItem : ISharedStringItem {
		string content = String.Empty;
		public string Content { get { return content; } set { content = value; } }
		public int RunsCount { get { return 1; } }
		public void CheckIntegrity() {
		}
	}
	#endregion
	#region FormattedStringItem
	public class FormattedStringItem : ISharedStringItem {
		readonly DocumentModel documentModel;
		public FormattedStringItem(DocumentModel documentModel) {
			this.Items = new List<FormattedStringItemPart>();
			this.documentModel = documentModel;
		}
		public string Content {
			set {
				Exceptions.ThrowInvalidOperationException("setting Content to FormattedStringItem instead FormattedStringItemPart");
			}
			get {
				return String.Empty;
			}
		}
		public List<FormattedStringItemPart> Items { get; set; }
		public int RunsCount { get { return Items.Count; } }
		public DocumentModel DocumentModel { get { return documentModel; } }
		protected internal virtual FormattedStringItemPart AddNewFormattedStringItemPart() {
			FormattedStringItemPart itemPart = new FormattedStringItemPart(documentModel);
			Items.Add(itemPart);
			return itemPart;
		}
		public string GetPlainText() {
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < RunsCount; i++) {
				sb.Append(Items[i].Content);
			}
			return sb.ToString();
		}
		public void CheckIntegrity() {
			if (RunsCount <= 0)
				IntegrityChecks.Fail("FormattedStringItem: RunsCount should be >0");
			for (int i = 0; i < RunsCount; i++)
				Items[i].CheckIntegrity();
		}
	}
	#endregion
	#region FormattedStringItemPart
	public class FormattedStringItemPart : SpreadsheetUndoableIndexBasedObject<RunFontInfo>, ISharedStringItem {  
		string content;
		public FormattedStringItemPart(DocumentModel documentModel)
			: base(documentModel) {
			this.Content = String.Empty;
		}
		#region Properties
		public string Content { get { return content; } set { content = value; } }
		public int RunsCount { get { return 1; } }
		public RunFontInfo Font { get { return Info; } }
		#endregion
		public void CheckIntegrity() {
		}
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None;
		}
		protected override UniqueItemsCache<RunFontInfo> GetCache(IDocumentModel documentModel) {
			return DocumentModel.Cache.FontInfoCache;
		}
		protected override void ApplyChanges(DocumentModelChangeActions changeActions) {
			DocumentModel.ApplyChanges(changeActions);
		}
	}
	#endregion
	#region SharedStringInnerIndex
	class SharedStringInnerIndex {
		#region NodeColor
		enum NodeColor {
			Black,
			Red
		}
		#endregion
		#region TreeNode
		class TreeNode {
			const uint maskColor = 0x80000000;
			const uint maskValue = 0x7fffffff;
			uint packedValues;
			string key = string.Empty;
			public string Key {
				get { return key; }
				set {
					if (value == null)
						key = string.Empty;
					else
						key = value;
				}
			}
			public int Value {
				get { return (int)(packedValues & maskValue); }
				set { packedValues = (packedValues & maskColor) | ((uint)value & maskValue); }
			}
			public NodeColor Color {
				get { return (packedValues & maskColor) != 0 ? NodeColor.Red : NodeColor.Black; }
				set {
					if (value == NodeColor.Black)
						packedValues &= maskValue;
					else
						packedValues |= maskColor;
				}
			}
			public TreeNode Parent { get; set; }
			public TreeNode Left { get; set; }
			public TreeNode Right { get; set; }
		}
		#endregion
		readonly TreeNode sentinel = new TreeNode();
		TreeNode root;
		public SharedStringInnerIndex() {
			root = sentinel;
		}
		public void Clear() {
			root = sentinel;
		}
		public int GetIndex(string text) {
			TreeNode node = FindNode(text);
			if (node == null)
				return -1;
			return node.Value;
		}
		public void AddIndex(string text, int index) {
			AddIndexCore(text, index, true);
		}
		public void SetIndex(string text, int index) {
			AddIndexCore(text, index, false);
		}
		void AddIndexCore(string text, int index, bool add) {
			TreeNode parent = null;
			TreeNode current = root;
			while (current != sentinel) {
				int result = string.Compare(text, current.Key);
				if (result == 0) {
					if (add)
						throw new Exception("Duplicated text");
					current.Value = index;
					return;
				}
				parent = current;
				if (result < 0)
					current = current.Left;
				else
					current = current.Right;
			}
			TreeNode node = new TreeNode() {
				Key = text,
				Value = index,
				Parent = parent,
				Left = sentinel,
				Right = sentinel,
				Color = NodeColor.Red
			};
			if (parent == null) {
				root = node;
				root.Color = NodeColor.Black;
			}
			else {
				if (string.Compare(text, current.Key) < 0)
					parent.Left = node;
				else
					parent.Right = node;
				BalanceTree(node);
			}
		}
		TreeNode FindNode(string key) {
			TreeNode node = root;
			while (node != sentinel) {
				int result = string.Compare(key, node.Key);
				if (result == 0)
					return node;
				node = (result < 0) ? node.Left : node.Right;
			}
			return null;
		}
		void BalanceTree(TreeNode node) {
			while (node != root && node.Parent.Color == NodeColor.Red) {
				TreeNode uncle;
				if (node.Parent == node.Parent.Parent.Left) {
					uncle = node.Parent.Parent.Right;
					if (uncle.Color == NodeColor.Black) {
						if (node == node.Parent.Right) {
							node = node.Parent;
							RotateLeft(node);
						}
						node.Parent.Color = NodeColor.Black;
						node.Parent.Parent.Color = NodeColor.Red;
						RotateRight(node.Parent.Parent);
					}
					else {
						node.Parent.Color = NodeColor.Black;
						uncle.Color = NodeColor.Black;
						node.Parent.Parent.Color = NodeColor.Red;
						node = node.Parent.Parent;
					}
				}
				else {
					uncle = node.Parent.Parent.Left;
					if (uncle.Color == NodeColor.Black) {
						if (node == node.Parent.Left) {
							node = node.Parent;
							RotateRight(node);
						}
						node.Parent.Color = NodeColor.Black;
						node.Parent.Parent.Color = NodeColor.Red;
						RotateLeft(node.Parent.Parent);
					}
					else {
						node.Parent.Color = NodeColor.Black;
						uncle.Color = NodeColor.Black;
						node.Parent.Parent.Color = NodeColor.Red;
						node = node.Parent.Parent;
					}
				}
			}
			root.Color = NodeColor.Black;
		}
		void RotateLeft(TreeNode node) {
			TreeNode neighbor = node.Right;
			node.Right = neighbor.Left;
			if (neighbor.Left != sentinel)
				neighbor.Left.Parent = node;
			if (neighbor != sentinel)
				neighbor.Parent = node.Parent;
			if (node.Parent != null) {
				if (node == node.Parent.Left)
					node.Parent.Left = neighbor;
				else
					node.Parent.Right = neighbor;
			}
			else
				root = neighbor;
			neighbor.Left = node;
			if (node != sentinel)
				node.Parent = neighbor;
		}
		void RotateRight(TreeNode node) {
			TreeNode neighbor = node.Left;
			node.Left = neighbor.Right;
			if (neighbor.Right != sentinel)
				neighbor.Right.Parent = node;
			if (neighbor != sentinel)
				neighbor.Parent = node.Parent;
			if (node.Parent != null) {
				if (node == node.Parent.Right)
					node.Parent.Right = neighbor;
				else
					node.Parent.Left = neighbor;
			}
			else
				root = neighbor;
			neighbor.Right = node;
			if (node != sentinel)
				node.Parent = neighbor;
		}
	}
	#endregion
	#region SharedStringTable
	public class SharedStringTable {
		readonly ChunkedArray<ISharedStringItem> items;
		readonly ChunkedDictionary<string, int> plainItems;
		public SharedStringTable() {
			this.items = new ChunkedArray<ISharedStringItem>(8192, 8192 * 1024);
			this.plainItems = new ChunkedDictionary<string, int>(30293);
		}
		public ISharedStringItem this[int index] { get { return items[index]; } }
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1043")]
		public ISharedStringItem this[SharedStringIndex index] { get { return items[index.ToInt()]; } }
		public int Count { get { return items.Count; } }
		public void CheckIntegrity() {
			int count = items.Count;
			for (int i = 0; i < count; i++)
				items[i].CheckIntegrity();
		}
		public SharedStringIndex RegisterString(string text) {
			if (text == null)
				text = String.Empty;
			int index = Count;
			if (!plainItems.TryGetOrAdd(text, Count, out index)) {
				PlainTextStringItem newItem = new PlainTextStringItem();
				newItem.Content = text;
				items.Add(newItem);
			}
			return new SharedStringIndex(index);
		}
		public void Clear() {
			items.Clear();
			plainItems.Clear();
		}
		public void Add(FormattedStringItem item) {
			items.Add(item);
		}
		public int Add(PlainTextStringItem item) {
			int result = Count;
			plainItems[item.Content] = result;
			items.Add(item);
			return result;
		}
	}
	#endregion
	#region SharedStringIndex
	[Serializable, StructLayout(LayoutKind.Sequential), ComVisible(false)]
	public struct SharedStringIndex : IConvertToInt<SharedStringIndex>, IComparable<SharedStringIndex>, IEquatable<SharedStringIndex> {
		readonly int m_value;
		public static SharedStringIndex DontCare = new SharedStringIndex(-1);
		public static SharedStringIndex MinValue = new SharedStringIndex(0);
		public static SharedStringIndex Zero = new SharedStringIndex(0);
		public static SharedStringIndex MaxValue = new SharedStringIndex(int.MaxValue);
		[System.Diagnostics.DebuggerStepThrough]
		public SharedStringIndex(int value) {
			m_value = value;
		}
		public bool Equals(SharedStringIndex obj) {
			return this.m_value == obj.m_value;
		}
		public override bool Equals(object obj) {
			return ((obj is SharedStringIndex) && (this.m_value == ((SharedStringIndex)obj).m_value));
		}
		public override int GetHashCode() {
			return m_value;
		}
		public override string ToString() {
			return m_value.ToString();
		}
		public static SharedStringIndex operator +(SharedStringIndex index, int value) {
			return new SharedStringIndex(index.m_value + value);
		}
		public static int operator -(SharedStringIndex index1, SharedStringIndex index2) {
			return index1.m_value - index2.m_value;
		}
		public static SharedStringIndex operator -(SharedStringIndex index, int value) {
			return new SharedStringIndex(index.m_value - value);
		}
		public static SharedStringIndex operator ++(SharedStringIndex index) {
			return new SharedStringIndex(index.m_value + 1);
		}
		public static SharedStringIndex operator --(SharedStringIndex index) {
			return new SharedStringIndex(index.m_value - 1);
		}
		public static bool operator <(SharedStringIndex index1, SharedStringIndex index2) {
			return index1.m_value < index2.m_value;
		}
		public static bool operator <=(SharedStringIndex index1, SharedStringIndex index2) {
			return index1.m_value <= index2.m_value;
		}
		public static bool operator >(SharedStringIndex index1, SharedStringIndex index2) {
			return index1.m_value > index2.m_value;
		}
		public static bool operator >=(SharedStringIndex index1, SharedStringIndex index2) {
			return index1.m_value >= index2.m_value;
		}
		public static bool operator ==(SharedStringIndex index1, SharedStringIndex index2) {
			return index1.m_value == index2.m_value;
		}
		public static bool operator !=(SharedStringIndex index1, SharedStringIndex index2) {
			return index1.m_value != index2.m_value;
		}
		public int ToInt() {
			return m_value;
		}
		#region IConvertToInt<SharedStringIndex> Members
		[System.Diagnostics.DebuggerStepThrough]
		int IConvertToInt<SharedStringIndex>.ToInt() {
			return m_value;
		}
		[System.Diagnostics.DebuggerStepThrough]
		SharedStringIndex IConvertToInt<SharedStringIndex>.FromInt(int value) {
			return new SharedStringIndex(value);
		}
		#endregion
		#region IComparable<SharedStringIndex> Members
		public int CompareTo(SharedStringIndex other) {
			if (m_value < other.m_value)
				return -1;
			if (m_value > other.m_value)
				return 1;
			else
				return 0;
		}
		#endregion
	}
	#endregion
}
