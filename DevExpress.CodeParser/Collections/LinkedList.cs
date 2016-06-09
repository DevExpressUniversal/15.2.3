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

#define USE_NODE_CACHE
using System;
using System.Collections;
using System.Threading;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	class LinkedList : IEnumerable, ICollection, IList
	{
		class Node
		{
			public Node Next;
			public Node Prior;
			public object Data;
		}
#if USE_NODE_CACHE
		class NodeCache
		{
			Node[] m_Stack;
			int m_StackIndex = -1;
			public NodeCache()
			{
				m_Stack = new Node[16];
			}
			private void GrowStack()
			{
				int lOldStackSize = m_Stack.Length;
				Node[] lNewStack = new Node[lOldStackSize * 2];
				Array.Copy(m_Stack, 0, lNewStack, 0, lOldStackSize);
				m_Stack = lNewStack;
			}
			public Node Alloc()
			{
				Node lResult;
				if (m_StackIndex >= 0)
				{
					lResult = m_Stack[m_StackIndex];
					m_StackIndex--;
				}
				else
					lResult = new Node();
				return lResult;
			}
			public void Free(Node node)
			{
				node.Data = null;
				node.Next = null;
				node.Prior = null;
				m_StackIndex++;
				if (m_StackIndex == m_Stack.Length)
					GrowStack();
				m_Stack[m_StackIndex] = node;
			}
		}
#endif
		class LinkedListEnumerator : IEnumerator
		{
			private LinkedList m_LinkedList;
			public LinkedListEnumerator(LinkedList linkedList)
			{
				if (linkedList == null)
					throw new ArgumentNullException("linkedList");
				m_LinkedList = linkedList;
				Reset();
			}
			public bool MoveNext()
			{
				m_LinkedList.MoveNext();
				return !m_LinkedList.IsAfterLast;
			}
			public void Reset()
			{
				m_LinkedList.MoveBeforeFirst();
			}
			public object Current
			{
				get
				{
					return m_LinkedList.Current;
				}
			}
		}
		int m_Count;
		Node m_Cursor;
		int m_CursorIndex;
		Node m_Head;
		Node m_Tail;
#if USE_NODE_CACHE
		NodeCache m_NodeCache;
#endif
		object m_SyncRoot;
		public LinkedList()
		{
			m_Head = AllocNode();
			m_Tail = AllocNode();
			m_Head.Next = m_Tail;
			m_Tail.Prior = m_Head;
			MoveBeforeFirst();
		}
		private Node AllocNode()
		{
#if USE_NODE_CACHE
			if (m_NodeCache == null)
				m_NodeCache = new NodeCache();
			return m_NodeCache.Alloc();
#else
	  return new Node();
#endif
		}
		private Node AllocNode(object value)
		{
			Node lResult = AllocNode();
			lResult.Data = value;
			return lResult;
		}
		private void FreeNode(Node node)
		{
#if USE_NODE_CACHE
			if (m_NodeCache == null)
				m_NodeCache = new NodeCache();
			m_NodeCache.Free(node);
#else
	  node.Data = null;
	  node.Next = null;
	  node.Prior = null;
#endif
		}
		private void PositionAtIndex(int index)
		{
			if (index < 0 || index > m_Count) {
#if !SL
				throw new ArgumentOutOfRangeException("index", index, "Index is out of range.");
#else
				throw new ArgumentOutOfRangeException("index", "Index is out of range.");
#endif
			}
			Node lWorkCursor = m_Cursor;
			int lWorkCursorIndex = m_CursorIndex;
			if (index == lWorkCursorIndex)
				return;
			if (index < lWorkCursorIndex)
			{
				if ((index - 0) < (lWorkCursorIndex - index))
				{
					lWorkCursor = m_Head;
					lWorkCursorIndex = -1;
				}
			}
			else
			{
				if ((index - lWorkCursorIndex) > (m_Count - index))
				{
					lWorkCursor = m_Tail;
					lWorkCursorIndex = m_Count;
				}
			}
			while (lWorkCursorIndex < index)
			{
				lWorkCursor = lWorkCursor.Next;
				lWorkCursorIndex++;
			}
			while (lWorkCursorIndex > index)
			{
				lWorkCursor = lWorkCursor.Prior;
				lWorkCursorIndex--;
			}
			m_Cursor = lWorkCursor;
			m_CursorIndex = lWorkCursorIndex;
		}
		public int Add(object value)
		{
			MoveAfterLast();
			int lResult = m_Count;
			InsertAtCursor(value);
			return lResult;
		}
		public void AddRange(ICollection range)
		{
			if (range == null)
				throw new ArgumentNullException("range");
			foreach (object obj in range)
				Add(obj);
		}
		public void Clear()
		{
			Node lTemp = m_Head.Next;
			while (lTemp != m_Tail)
			{
				m_Head.Next = lTemp.Next;
				FreeNode(lTemp);
				lTemp = m_Head.Next;
			}
			m_Tail.Prior = m_Head;
			m_Count = 0;
			m_Cursor = m_Head;
			m_CursorIndex = -1;
		}
		public bool Contains(object value)
		{
			return (IndexOf(value) != -1);
		}
		public void DeleteAtCursor()
		{
			Node lTemp = m_Cursor;
			if (lTemp == m_Head)
				throw new InvalidOperationException("Cannot delete at cursor because it is positioned before the first item.");
			else if (lTemp == m_Tail)
				throw new InvalidOperationException("Cannot delete at cursor because it is positioned after the last item.");
			lTemp.Prior.Next = lTemp.Next;
			lTemp.Next.Prior = lTemp.Prior;
			m_Cursor = lTemp.Next;
			m_Count--;
			FreeNode(lTemp);
		}
		public object First()
		{
			PositionAtIndex(0);
			return m_Cursor.Data;
		}
		public int IndexOf(object value)
		{
			Node lWorkCursor = m_Head.Next;
			int lWorkCursorIndex = 0;
			while (lWorkCursor != m_Tail)
			{
				if (lWorkCursor.Data == value)
				{
					m_Cursor = lWorkCursor;
					m_CursorIndex = lWorkCursorIndex;
					return lWorkCursorIndex;
				}
				lWorkCursor = lWorkCursor.Next;
				lWorkCursorIndex++;
			}
			return -1;
		}
		public void Insert(int index, object value)
		{
			PositionAtIndex(index);
			InsertAtCursor(value);
		}
		public void InsertAtCursor(object value)
		{
			if (m_Cursor == m_Head)
				MoveNext();
			if (m_Cursor == null)
				return;
			Node lNewNode = AllocNode(value);
			if (lNewNode == null)
				return;
			lNewNode.Next = m_Cursor;
			lNewNode.Prior = m_Cursor.Prior;
			if (lNewNode.Prior != null)
				lNewNode.Prior.Next = lNewNode;
			m_Cursor.Prior = lNewNode;
			m_Cursor = lNewNode;
			m_Count++;
		}
		public object Last()
		{
			PositionAtIndex(m_Count - 1);
			return m_Cursor.Data;
		}
		public void MoveAfterLast()
		{
			m_Cursor = m_Tail;
			m_CursorIndex = m_Count;
		}
		public void MoveBeforeFirst()
		{
			m_Cursor = m_Head;
			m_CursorIndex = -1;
		}
		public void MoveNext()
		{
			if (m_Cursor != m_Tail)
			{
				m_Cursor = m_Cursor.Next;
				m_CursorIndex++;
			}
		}
		public void MovePrior()
		{
			if (m_Cursor != m_Head)
			{
				m_Cursor = m_Cursor.Prior;
				m_CursorIndex--;
			}
		}
		public void Remove(object value)
		{
			if (IndexOf(value) != -1)
				DeleteAtCursor();
		}
		public void RemoveAt(int index)
		{
			PositionAtIndex(index);
			DeleteAtCursor();
		}
		public int Count
		{
			get
			{
				return m_Count;
			}
		}
		public object Current
		{
			get
			{
				if (m_Cursor == m_Head)
					throw new InvalidOperationException("Cannot get current item because the cursor is positioned before the first item.");
				if (m_Cursor == m_Tail)
					throw new InvalidOperationException("Cannot get current item because the cursor is positioned after the last item.");
				return m_Cursor.Data;
			}
		}
		public bool IsAfterLast
		{
			get
			{
				return (m_Cursor == m_Tail);
			}
		}
		public bool IsBeforeFirst
		{
			get
			{
				return (m_Cursor == m_Head);
			}
		}
		public object this[int index]
		{
			get
			{
				PositionAtIndex(index);
				return m_Cursor.Data;
			}
			set
			{
				PositionAtIndex(index);
				m_Cursor.Data = value;
			}
		}
		public IEnumerator GetEnumerator()
		{
			return new LinkedListEnumerator(this);
		}
		public void CopyTo(Array array, int index)
		{
			if (array == null)
				throw new ArgumentNullException("array");
			if (array.Rank != 1)
				throw new ArgumentException("Array is multi-dimensional.", "array");
			if (index < 0 || index >= array.Length) {
#if !SL
				throw new ArgumentOutOfRangeException("index", index, "Index is out of range.");
#else
				throw new ArgumentOutOfRangeException("index", "Index is out of range.");
#endif
			}
			if (m_Count > array.Length - index)
				throw new ArgumentException("The number of items in this list is greater than the available space in the destination array.");
			MoveBeforeFirst();
			for (int i = 0; i < m_Count; i++)
			{
				MoveNext();
				array.SetValue(m_Cursor.Data, index + i);
			}
		}
		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}
		public object SyncRoot
		{
			get
			{
				if (m_SyncRoot == null)
					Interlocked.CompareExchange(ref m_SyncRoot, new object(), null);
				return m_SyncRoot;
			}
		}
		public bool IsFixedSize
		{
			get
			{
				return false;
			}
		}
		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}
	}
}
