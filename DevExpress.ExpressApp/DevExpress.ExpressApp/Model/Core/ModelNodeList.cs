#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.ComponentModel;
using System.Diagnostics;
namespace DevExpress.ExpressApp.Model.Core {
	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
#if DEBUG
	[DebuggerDisplay("{DebuggerString,nq}, Count = {Count}, Layer Index = {LayerIndex}")]
#else
	[DebuggerDisplay("{DebuggerString,nq}, Count = {Count}")]
#endif
	public class ModelNodeList<NodeType> : ModelNode, IModelList<NodeType> {
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public ModelNodeList(ModelNodeInfo nodeInfo, string nodeId) : base(nodeInfo, nodeId) { }
		public int Count { get { return NodeCount; } }
		public new NodeType this[string id] { get { return (NodeType)(object)base[id]; } }
		public new NodeType this[int index] { get { return (NodeType)(object)base[index]; } }
		bool ICollection<NodeType>.IsReadOnly { get { return false; } }
		IEnumerable<SpecificNodeType> IModelList<NodeType>.GetNodes<SpecificNodeType>() {
			List<SpecificNodeType> result = new List<SpecificNodeType>();
			foreach(ModelNode node in GetSortedNodes()) {
				if(node is SpecificNodeType) {
					result.Add((SpecificNodeType)(object)node);
				}
			}
			return result.AsReadOnly();
		}
		int IList<NodeType>.IndexOf(NodeType item) {
			return Array.IndexOf<ModelNode>(GetSortedNodes(), (ModelNode)(object)item);
		}
		bool ICollection<NodeType>.Contains(NodeType item) {
			return Array.IndexOf<ModelNode>(GetNodes(), (ModelNode)(object)item) > -1;
		}
		void ICollection<NodeType>.CopyTo(NodeType[] array, int arrayIndex) {
			ModelNode[] nodesToCopy = GetSortedNodes();
			Array.Copy(nodesToCopy, 0, array, arrayIndex, nodesToCopy.Length);
		}
		void IModelList<NodeType>.ClearNodes() {
			foreach(ModelNode node in GetNodes()) {
				node.Delete();
			}
		}
	  public IEnumerator<NodeType> GetEnumerator() {
			return new ModelNodeListEnumerator(this);
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return new ModelNodeListEnumerator(this);
		}
		#region ModelNodeListEnumerator
		private struct ModelNodeListEnumerator : IEnumerator<NodeType> {
			private readonly IEnumerator enumerator;
			internal ModelNodeListEnumerator(ModelNodeList<NodeType> list) {
				enumerator = list.GetSortedNodes().GetEnumerator();
			}
			public NodeType Current { get { return (NodeType)enumerator.Current; } }
			object IEnumerator.Current { get { return enumerator.Current; } }
			public bool MoveNext() {
				return enumerator.MoveNext();
			}
			public void Reset() {
				enumerator.Reset();
			}
			public void Dispose() { }
		}
		#endregion
		#region Obsolete 11.2
		NodeType IList<NodeType>.this[int index] {
			get { return (NodeType)(object)base[index]; }
			set { throw new NotSupportedException(ModelNodeListObsoleteMessages.SetIndexMethodObsolete); }  
		}
		[Obsolete]
		void IList<NodeType>.Insert(int index, NodeType item) {
			((IModelList<NodeType>)this).Insert(index, item);
		}
		[Obsolete]
		void ICollection<NodeType>.Add(NodeType item) {
			((IModelList<NodeType>)this).Add(item);
		}
		[Obsolete]
		void IList<NodeType>.RemoveAt(int index) {
			((IModelList<NodeType>)this).RemoveAt(index);
		}
		[Obsolete]
		bool ICollection<NodeType>.Remove(NodeType item) {
			return ((IModelList<NodeType>)this).Remove(item);
		}
		[Obsolete]
		void ICollection<NodeType>.Clear() {
			((IModelList<NodeType>)this).Clear();
		}
		void IModelList<NodeType>.Insert(int index, NodeType item) {
			throw new NotSupportedException(ModelNodeListObsoleteMessages.InsertMethodObsolete);
		}
		void IModelList<NodeType>.Add(NodeType item) {
			throw new NotSupportedException(ModelNodeListObsoleteMessages.AddMethodObsolete);
		}
		void IModelList<NodeType>.RemoveAt(int index) {
			throw new NotSupportedException(ModelNodeListObsoleteMessages.RemoveAtMethodObsolete);
		}
		bool IModelList<NodeType>.Remove(NodeType item) {
			throw new NotSupportedException(ModelNodeListObsoleteMessages.RemoveMethodObsolete);
		}
		void IModelList<NodeType>.Clear() {
			throw new NotSupportedException(ModelNodeListObsoleteMessages.ClearMethodObsolete);
		}
		#endregion
	}
	static class ModelNodeListObsoleteMessages {
		private const string MethodNotSupported = "This method is not supported. ";
		private const string ReferToBC = "Refer to the BC1336 breaking change for more information on how to update your code.";
		internal const string InsertMethodObsolete = MethodNotSupported + "Use 'IModelNode.AddNode' instead. " + ReferToBC;
		internal const string AddMethodObsolete = MethodNotSupported + "Use 'IModelNode.AddNode' instead. " + ReferToBC;
		internal const string RemoveAtMethodObsolete = MethodNotSupported + "Use 'IModelNode.Remove' instead. " + ReferToBC;
		internal const string RemoveMethodObsolete = MethodNotSupported + "Use 'IModelNode.Remove' instead. " + ReferToBC;
		internal const string ClearMethodObsolete = MethodNotSupported + "Use 'ClearNodes' instead. " + ReferToBC;
		internal const string SetIndexMethodObsolete = MethodNotSupported + "Use 'IModelNode.AddNode' to add a node to the collection or 'IModelNode.Remove' to remove a node from the collection. " + ReferToBC;
	}
}
