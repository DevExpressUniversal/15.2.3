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
using System.Collections;
using System.ComponentModel;
namespace DevExpress.Xpf.Grid {
	public class TreeListNodeIterator : IEnumerator<TreeListNode>, IEnumerable<TreeListNode> {
		TreeListNode startNode;
		TreeListNodeCollection startNodes;
		Stack<TreeListNode> stack = new Stack<TreeListNode>();
		bool visibleOnly;
		TreeListNode current;
		public TreeListNodeIterator(TreeListNode startNode, bool visibleOnly) {
			this.startNode = startNode;
			this.visibleOnly = visibleOnly;
		}
		public TreeListNodeIterator(TreeListNode startNode) : this(startNode, false) { }
		public TreeListNodeIterator(TreeListNodeCollection startNodes, bool visibleOnly) {
			this.startNodes = startNodes;
			this.visibleOnly = visibleOnly;
		}
		public TreeListNodeIterator(TreeListNodeCollection startNodes) : this(startNodes, false) { }
		#region IEnumerator Members
#if !SL
	[DevExpressXpfGridLocalizedDescription("TreeListNodeIteratorCurrent")]
#endif
		public TreeListNode Current {
			get { return current; }
		}
		public bool MoveNext() {
			if(current == null) {
				current = startNode;
				if(current == null && startNodes != null) {
					TraverseChildNodes(startNodes);
					UpdateCurrent();
				}
			} else {
				if((!visibleOnly || current.IsExpanded))
					TraverseChildNodes(current.Nodes);
				UpdateCurrent();
			}
			return current != null;
		}
		public void Reset() {
			stack.Clear();
			current = null;
		}
		void TraverseChildNodes(TreeListNodeCollection nodes) {
			int count = nodes.Count;
			for(int i = 0; i < count; i++) {
				TreeListNode child = nodes[(count - 1) - i];
				stack.Push(child);
			}
		}
		void UpdateCurrent() {
			current = stack.Count > 0 ? stack.Pop() : null;
		}
		object System.Collections.IEnumerator.Current {
			get { return Current; }
		}
		#endregion
		#region IDisposable Members
		void IDisposable.Dispose() {
			Reset();
		}
		#endregion
		#region IEnumerable<TreeListNode> Members
		IEnumerator<TreeListNode> IEnumerable<TreeListNode>.GetEnumerator() {
			return this;
		}
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			return this;
		}
		#endregion
	}
}
