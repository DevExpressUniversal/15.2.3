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
namespace DevExpress.XtraPivotGrid.Customization {
	public interface IEnumerableTreeNode<T>
	where T : IEnumerableTreeNode<T> {
		T Parent { get; }
		T FirstChild { get; }
		T NextSibling { get; }
	}
	public sealed class TreeDepthFirstEnumerator<T> : TreeEnumerator<T>
	where T : class, IEnumerableTreeNode<T> {
		public TreeDepthFirstEnumerator(T root) : base(root) { }
		public override bool MoveNext() {
			if(Current == null) {
				MoveToRoot();
				return true;
			}
			return TryMoveToFirstChild() || TryMoveToNextSibling() || TryMoveToNextAncestorsSibling();
		}
	}
	public abstract class TreeEnumerator<T> : IEnumerator<T>
	where T : class, IEnumerableTreeNode<T> {
		T root;
		T current;
		protected TreeEnumerator(T root) {
			this.root = root;
		}
		public T Root { get { return root; } }
		public T Current { get { return current; } private set { current = value; } }
		object IEnumerator.Current { get { return Current; } }
		public abstract bool MoveNext();
		protected void MoveToRoot() {
			Current = Root;
		}
		protected bool TryMoveToFirstChild() {
			return TryMoveCore(Current.FirstChild);
		}
		protected bool TryMoveToNextSibling() {
			return TryMoveCore(Current.NextSibling);
		}
		bool TryMoveCore(T node) {
			if(node == null) return false;
			Current = node;
			return true;
		}
		protected bool TryMoveToNextAncestorsSibling() {
			T nextNode;
			do {
				Current = Current.Parent;
				if(Current == Root.Parent) {
					Current = null;
					return false;
				}
				nextNode = Current.NextSibling;
			} while(nextNode == null);
			Current = nextNode;
			return true;
		}
		public void Reset() {
			Current = null;
		}
		public void Dispose() { }
	}
}
