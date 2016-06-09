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
using System.ComponentModel;
using System.Collections;
using System.Runtime.InteropServices;
#if SL
using DevExpress.Xpf.Collections;
#endif
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	[Serializable]
	[StructLayout(LayoutKind.Explicit)]
	public class NodeList : IList
	{
	[FieldOffset(0)]
	int _Size;
	[FieldOffset(8)]
	object _Object;
#if !SL
	[NonSerialized]
	[FieldOffset(8)]
	object[] _Items;
#else
	object[] _Items { get { return (object[])_Object; } set { _Object = value; } }
#endif
		public static readonly NodeList Empty = new EmptyNodeList();
		private class NodeListEnumerator : IEnumerator
		{
			NodeList _List;
			int _Index;
			public NodeListEnumerator(NodeList list)
			{
				_List = list;
				_Index = -1;
			}
			#region IEnumerator Members
			public void Reset()
			{
				_Index = -1;
			}
			public object Current
			{
				get
				{
					return _List[_Index];
				}
			}
			public bool MoveNext()
			{
				_Index ++;
				if (_Index >= _List._Size)
				{
					_Index = _List._Size;
					return false;
				}
				return true;
			}
			#endregion
		}
		public NodeList() : this (1)
		{
		}
		public NodeList(int capacity)
		{
			if (capacity > 1)
				_Items = new object[capacity];
		}
		private bool IsSingleValue
		{
			get
			{
				return _Size == 1;
			}
		}
		private void EnsureCapacity(int capacity)
		{
			object[] lNewArray = new object[capacity * 2];
			Array.Copy(_Items, 0, lNewArray, 0, _Size);
			_Items = lNewArray;
		}
		private void EnsureCapacity(int capacity, int count)
		{
			if (capacity * 2 <= capacity + count)
				capacity = capacity * 2 + count;
			else
				capacity = capacity * 2;
			EnsureCapacity(capacity);
		}
		protected virtual NodeList CreateInstance()
		{
			return new NodeList();
		}
		protected virtual void CloneElements(NodeList targetList, ElementCloneOptions options)
		{
			if (targetList == null)
				return;
			for (int i = 0; i < Count; i++)
			{
				ICloneable lElement = this[i] as ICloneable;
				if (lElement == null)
					throw new Exception("Object must implement ICloneable.");
				LanguageElement leElement = lElement as LanguageElement;
				if (leElement != null)
				{
					BaseElement clone = leElement.Clone(options);
					if (clone != null)
						targetList.Add(clone);
				}
				else
				{
					object clone = lElement.Clone();
					if (clone != null)
						targetList.Add(clone);
				}
			}
		}
		#region IList Members
		public virtual bool IsReadOnly
		{
			get
			{
				return false;
			}
		}
		public virtual int Add(object value)
		{
			Insert(_Size, value);
			return _Size;
		}
		public virtual void RemoveAt(int index)
		{
			if ((index < 0) || (index >= _Size))
				throw new ArgumentOutOfRangeException("index");
			if (index == 0 && IsSingleValue)
			{
				_Object = null;
				_Size --;
			}
			else
			{
				_Size --;
				if (_Size == 1)
					_Object = _Items[index == 0 ? 1 : 0];
				else
				{
					if (index < _Size)
						Array.Copy(_Items, (index + 1), _Items, index, _Size - index);
					_Items[_Size] = null;
				}
			}
		}
	public virtual void RemoveFrom(int index)
	{
	  if ((index < 0) || (index >= _Size))
		throw new ArgumentOutOfRangeException("index");
	  if (index == 0 && IsSingleValue)
	  {
		_Object = null;
		_Size--;
	  }
	  else
	  {
		if (index == 1)
		{
		  _Object = _Items[0];
		  _Size = 1;
		}
		else
		{
		  Array.Clear(_Items, index, _Size - index);
		  _Size = index;
		}
	  }
	}
		public virtual void Insert(int index, object value)
		{
			if (value == null)
				throw new ArgumentNullException("value");
			if ((index < 0) || (index > _Size))
				throw new ArgumentOutOfRangeException("index");
			if (_Size == 0)
			{
				_Size ++;
				_Object = value;
			}
			else
			{
				if (_Size == 1)
				{
					object lTemp = _Object;
					_Items = new object[2];
					_Items[index == 0 ? 1 : 0] = lTemp;
					_Items[index] = value;
				}
				else
				{
					if (_Size == _Items.Length)				
						EnsureCapacity((_Size + 1));
					if (index < _Size)
						Array.Copy(_Items, index, _Items, (index + 1), (_Size - index));
 					_Items[index] = value;
				}
				_Size++;
			}
		}
		public virtual void InsertRange(int index, IList value)
		{
			if (value == null)
				throw new ArgumentNullException("value");
			if ((index < 0) || (index > _Size))
				throw new ArgumentOutOfRangeException("index");
			int countList = value.Count;
			if (value.Count == 0)
				return;
			if (countList == 1)
			{
				Insert(index, value[0]);				
			}
			else
			{
				if (_Size == 0)
				{
					_Items = new object[countList + 1];
				}
				else
				{
					if(_Size == 1)
					{
						object lTemp = _Object;
						_Items = new object[1];
						_Items[0] = lTemp;
					}
					if (_Items != null || _Size < _Items.Length + countList)
						EnsureCapacity((_Size + 1), countList);				
					if (index < _Size)
						Array.Copy(_Items, index, _Items, (index + countList), (_Size - index));
				}
				for (int i = 0; i < countList; i++)
				{				
						_Items[index + i] = value[i];
				}
				_Size = _Size + countList;
			}
		}
		public virtual void Remove(object value)
		{
			int lIndex = IndexOf(value);
			if (lIndex >= 0)
					RemoveAt(lIndex);
 		}
		public bool Contains(object value)
		{
			return IndexOf(value) >= 0;
		}
		public virtual void Clear()
		{
			try
			{			
				if(_Size == 0)
					return;
				if (_Size == 1)
					_Object = null;
				else
					Array.Clear(_Items, 0, _Size);
				_Size = 0;						
			}
			finally
			{
				OnClear();
			}
		}
		public int IndexOf(object value)
		{
			if (value == null)
				return -1;
			if (_Size == 0)
				return -1;
			if (_Size == 1)
				return value.Equals(_Object) ? 0 : -1;
			return Array.IndexOf(_Items, value);
		}		
		public bool IsFixedSize
		{
			get
			{
				return false;
			}
		}
		#endregion
		#region ICollection Members
		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}
		public int Count
		{
			get
			{
				return _Size;
			}
		}
		public void CopyTo(Array array, int index)
		{
			if (array == null)
				throw new ArgumentNullException("array");
			if (array.Rank != 1)
				throw new ArgumentException("array rank is not equal to 1");
	  if (_Size == 0)
		return;
			if (_Size == 1)
				array.SetValue(_Object, index);
			else
				Array.Copy(_Items, 0, array, index, _Size);
		}
		public virtual object this[int index]
		{
			get
			{
				if (index < 0 || index >= _Size)
					throw new ArgumentOutOfRangeException("index");
				if (_Size == 1 && index == 0)
					return _Object;
				return _Items[index];
			}
			set
			{
				if (index < 0 || index >= _Size)
					throw new ArgumentOutOfRangeException("index");
				if (_Size == 1 && index == 0)
					_Object = value;
				else
					_Items[index] = value;
			}
		}
		public object SyncRoot
		{
			get
			{
				return this;
			}
		}
		#endregion
		#region IEnumerable Members
		public IEnumerator GetEnumerator()
		{
			return new NodeListEnumerator(this);
		}
		#endregion
	public static void VisitNodeList(NodeList list, IElementVisitor visitor)
	{
	  if (list == null)
		return;
	  list.VisitNodeList(visitor);
	}
	public void VisitNodeList(IElementVisitor visitor)
	{
	  if (visitor == null)
		return;
	  int count = Count;
	  for (int i = 0; i < count; i++)
		visitor.Visit(this[i] as IElement);
	}
		public virtual void AddRange(ICollection range) 
		{
	  if (range == null)
		return;
			foreach(object obj in range)
				Add(obj);
		}
		public void Sort(IComparer comparer) 
		{
			Sort(0, _Size, comparer);
		}
		public void Sort(int index, int length, IComparer comparer) 
		{
			if(_Size > 1)
				Array.Sort(_Items, index, length, comparer);
		}
		public object[] ToArray() 
		{
			object[] lObjects = new object[_Size];
			if(_Size == 1)
				lObjects[0] = _Object;
			else
				if(_Size > 1)
					Array.Copy(_Items, lObjects, _Size);
			return lObjects;
		}
		public Array ToArray(Type type)
		{
			if (type == null)			
				throw new ArgumentNullException("type");			
			Array lArray = Array.CreateInstance(type, _Size);
			if(_Size == 1)
				lArray.SetValue(_Object, 0);
			else
				if(_Size > 1)
				Array.Copy(_Items, lArray, _Size);
			return lArray;
		} 
		public void IncreaseNodesIndices(int startFrom)
		{
			if(_Size < 1)
				return;
			LanguageElement element = null;
			if(_Size == 1)
			{
				if (startFrom <= 0)
				{
					element = _Object as LanguageElement;
					if (element != null)
						element.SetIndex(element.Index + 1);
				}
			}
			else
			{
				object[] lList = _Items;
				int lListCount = _Size;
				for (int i = startFrom; i < lListCount; i++)
				{
					element = lList[i] as LanguageElement;
					element.SetIndex(element.Index + 1);
				}
			}
		}
		public int BinarySearch(int index, object value, IComparer comparer)
		{
			return BinarySearch(index, Count - index, value, comparer);
		}
		public int BinarySearch(int index, int length, object value, IComparer comparer)
		{
			if (_Size == 0)
				return ~index;
			if (_Size > 1)
				return Array.BinarySearch(_Items, index, length, value, comparer);
			int res = comparer.Compare(_Object, value);
			return res > 0 ? -1 : (res == 0 ? 0 : -2);
		}
		public static ArrayList ToArrayList(NodeList list)
		{
			ArrayList lResult = new ArrayList();
			lResult.AddRange(list);
			return lResult;
		}
		public ArrayList ToArrayList()
		{
			return ToArrayList(this);
		}
		public virtual void Replace(object oldObject, object newObject)
		{
			int lIdx = IndexOf(oldObject);
			if (lIdx < 0)
				return;
			RemoveAt(lIdx);
			if (newObject != null)
				Insert(lIdx, newObject);
		}
		public virtual NodeList DeepClone()
		{
			return DeepClone(ElementCloneOptions.Default);
		}
		public virtual NodeList DeepClone(ElementCloneOptions options)
		{			
			NodeList lClonedNodes = CreateInstance();
			CloneElements(lClonedNodes, options);
			return lClonedNodes;
		}
		protected virtual void OnClear()
		{
		}		
	}
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal class EmptyNodeList : NodeList
	{
		void ThrowException()
		{
			throw new InvalidOperationException("EmptyNodeList is readonly and cannot be modified directly. Use AddNode or AddDetailNode instead if you need to add new node to a LanguageElement.");
		}		
		public override int Add(object value)
		{
			ThrowException();
			return -1;
		}
		public override void AddRange(ICollection range) 
		{
			ThrowException();
		}
		public override void RemoveAt(int index)
		{
			ThrowException();
		}
		public override void Insert(int index, object value)
		{
			ThrowException();
		}
		public override void InsertRange(int index, IList value)
		{
			ThrowException();
		}
		public override void Remove(object value)
		{
			ThrowException();
		}
		public override void Replace(object oldObject, object newObject)
		{
			ThrowException();
		}
		public override bool IsReadOnly
		{
			get
			{
				return true;
			}
		}
		public override object this[int index]
		{
			get
			{
				return null;
			}
			set
			{
				ThrowException();
			}
		}
	}
}
