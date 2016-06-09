﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
using System.Linq;
using System.Text;
namespace DevExpress.Xpf.Core.Design {
	public class TypeTree<TValue> {
		struct Data {
			public Data(TValue value, int index)
				: this() {
				Value = value;
				Index = index;
			}
			public TValue Value { get; private set; }
			public int Index { get; private set; }
		}
		readonly PartiallySortedDictionary<Type, Data> dictionary = new PartiallySortedDictionary<Type, Data>(GreaterThen);
		readonly Dictionary<Type, Data> interfaces = new Dictionary<Type, Data>();
		int currentIndex = 0;
		public void Add(Type key, TValue value) {
			if(!TryAdd(key, value))
				throw new ArgumentException("key");
		}
		public TValue GetOrAdd(Type key, Func<TValue> value) {
			if(key == null)
				throw new ArgumentNullException("key");
			if(key.IsInterface) {
				Data data;
				if(!interfaces.TryGetValue(key, out data)) {
					data = WrapValue(value());
					interfaces.Add(key, data);
				}
				return data.Value;
			}
			return dictionary.GetOrAdd(key, () => WrapValue(value())).Value;
		}
		public bool TryAdd(Type key, TValue value) {
			if(key == null)
				throw new ArgumentNullException("key");
			if(key.IsInterface) {
				if(interfaces.ContainsKey(key)) return false;
				interfaces.Add(key, WrapValue(value));
				return true;
			}
			return dictionary.TryAdd(key, WrapValue(value));
		}
		public IEnumerable<TypeTreeSearchResultItem<TValue>> Find(Type key) {
			return FindInDictionary(key).Concat(FindInInterfaces(key))
				.Select((p, i) => new { p = p, first = i == 0 })
				.OrderBy(d => d.p.Value.Index)
				.Select(d => new TypeTreeSearchResultItem<TValue>(d.p.Key, d.p.Value.Value, d.first))
				.ToArray();
		}
		IEnumerable<KeyValuePair<Type, Data>> FindInInterfaces(Type key) {
			return interfaces.Where(i => i.Key.IsAssignableFrom(key));
		}
		IEnumerable<KeyValuePair<Type, Data>> FindInDictionary(Type key) {
			IEnumerable<KeyValuePair<Type, Data>> data;
			return dictionary.TryFindGreaterThenOrEqual(key, out data) ? data : new KeyValuePair<Type, Data>[] { };
		}
		Data WrapValue(TValue value) {
			return new Data(value, ++currentIndex);
		}
		static bool GreaterThen(Type t1, Type t2) { return t1.IsAssignableFrom(t2); }
	}
	public class TypeTreeSearchResultItem<TValue> {
		public TypeTreeSearchResultItem(Type type, TValue value, bool isNearest) {
			Type = type;
			Value = value;
			IsNearest = isNearest;
		}
		public Type Type { get; private set; }
		public TValue Value { get; private set; }
		public bool IsNearest { get; private set; }
	}
	public class PartiallySortedDictionary<TKey, TValue> {
		protected enum NodeFindResult { Found, FoundDuplicate, FoundChild }
		protected class PartiallySortedTreeNode {
			readonly TKey key;
			readonly LinkedList<PartiallySortedTreeNode> children = new LinkedList<PartiallySortedTreeNode>();
			public PartiallySortedTreeNode(TKey key) {
				this.key = key;
			}
			public TKey Key { get { return key; } }
			public TValue Value { get; set; }
			public PartiallySortedTreeNode Parent { get; private set; }
			public LinkedListNode<PartiallySortedTreeNode> FirstChild { get { return children.First; } }
			public LinkedListNode<PartiallySortedTreeNode> SecondChild {
				get {
					if(children.First == null)
						throw new InvalidOperationException();
					return children.First.Next;
				}
			}
			public void AddFirstChild(LinkedListNode<PartiallySortedTreeNode> child) {
				child.Value.Parent = this;
				children.AddFirst(child);
			}
			public void RemoveChild(LinkedListNode<PartiallySortedTreeNode> child) {
				children.Remove(child);
			}
		}
		readonly Func<TKey, TKey, bool> greaterThen;
		public PartiallySortedDictionary(Func<TKey, TKey, bool> greaterThen) {
			if(greaterThen == null)
				throw new ArgumentNullException("greaterThen");
			this.greaterThen = greaterThen;
			Root = new PartiallySortedTreeNode(default(TKey));
		}
		public TValue GetOrAdd(TKey key, Func<TValue> value) {
			NodeFindResult findResult;
			return GetOrAddCore(key, value, out findResult);
		}
		public void Add(TKey key, TValue value) {
			if(!TryAdd(key, value))
				throw new ArgumentException("key");
		}
		public bool TryAdd(TKey key, TValue value) {
			NodeFindResult findResult;
			GetOrAddCore(key, () => value, out findResult);
			return findResult != NodeFindResult.FoundDuplicate;
		}
		public bool TryFindNearestGreaterThenOrEqual(TKey key, out KeyValuePair<TKey, TValue> near) {
			PartiallySortedTreeNode node;
			FindNearestNodeGreaterThenOrEqual(key, out node);
			if(node == Root) {
				near = default(KeyValuePair<TKey, TValue>);
				return false;
			}
			near = new KeyValuePair<TKey, TValue>(node.Key, node.Value);
			return true;
		}
		public bool TryFindGreaterThenOrEqual(TKey key, out IEnumerable<KeyValuePair<TKey, TValue>> branch) {
			PartiallySortedTreeNode node;
			FindNearestNodeGreaterThenOrEqual(key, out node);
			if(node == Root) {
				branch = null;
				return false;
			}
			List<KeyValuePair<TKey, TValue>> result = new List<KeyValuePair<TKey, TValue>>();
			for(PartiallySortedTreeNode t = node; t != Root; t = t.Parent)
				result.Add(new KeyValuePair<TKey, TValue>(t.Key, t.Value));
			branch = result;
			return true;
		}
		protected PartiallySortedTreeNode Root { get; private set; }
		protected TValue GetOrAddCore(TKey key, Func<TValue> value, out NodeFindResult findResult) {
			if(key == null)
				throw new ArgumentNullException("key");
			PartiallySortedTreeNode treeNode;
			findResult = FindNearestNodeGreaterThenOrEqual(key, out treeNode);
			if(findResult == NodeFindResult.FoundDuplicate) return treeNode.Value;
			TValue valueInstance = value();
			PartiallySortedTreeNode newTreeNode = new PartiallySortedTreeNode(key) { Value = valueInstance };
			if(findResult == NodeFindResult.FoundChild) {
				treeNode.AddFirstChild(new LinkedListNode<PartiallySortedTreeNode>(newTreeNode));
				LinkedListNode<PartiallySortedTreeNode> nextNode;
				for(LinkedListNode<PartiallySortedTreeNode> listNode = treeNode.SecondChild; listNode != null; listNode = nextNode) {
					nextNode = listNode.Next;
					if(!greaterThen(key, listNode.Value.Key)) continue;
					treeNode.RemoveChild(listNode);
					newTreeNode.AddFirstChild(listNode);
				}
			} else {
				treeNode.AddFirstChild(new LinkedListNode<PartiallySortedTreeNode>(newTreeNode));
			}
			return valueInstance;
		}
		protected NodeFindResult FindNearestNodeGreaterThenOrEqual(TKey key, out PartiallySortedTreeNode nearNode) {
			PartiallySortedTreeNode treeNode = Root;
			LinkedListNode<PartiallySortedTreeNode> listNode = Root.FirstChild;
			while(true) {
				if(listNode == null) {
					nearNode = treeNode;
					return NodeFindResult.Found;
				}
				if(object.Equals(listNode.Value.Key, key)) {
					nearNode = listNode.Value;
					return NodeFindResult.FoundDuplicate;
				}
				if(greaterThen(listNode.Value.Key, key)) {
					treeNode = listNode.Value;
					listNode = treeNode.FirstChild;
				} else if(greaterThen(key, listNode.Value.Key)) {
					nearNode = treeNode;
					return NodeFindResult.FoundChild;
				} else {
					listNode = listNode.Next;
				}
			}
		}
	}
}
