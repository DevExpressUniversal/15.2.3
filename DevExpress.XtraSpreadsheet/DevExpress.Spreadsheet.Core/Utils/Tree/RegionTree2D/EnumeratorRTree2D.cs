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
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Utils.Trees {
	partial class RTree2D<T> {
		class RTreeEnumerator : IEnumerator<T> {
			readonly Stack<IEnumerator<Node>> nodesEnumerators = new Stack<IEnumerator<RTree2D<T>.Node>>();
			readonly Node root;
			T current;
			public RTreeEnumerator(RTree2D<T>.Node root) {
				Guard.ArgumentNotNull(root, "root");
				this.root = root;
				Reset();
			}
			T IEnumerator<T>.Current { get { return GetCurrent(); } }
			object IEnumerator.Current { get { return GetCurrent(); } }
			void IDisposable.Dispose() {
			}
			public bool MoveNext() {
				if (nodesEnumerators.Count <= 0)
					return false;
				IEnumerator<RTree2D<T>.Node> enumerator = nodesEnumerators.Peek();
				for (; ; ) {
					if (!enumerator.MoveNext()) {
						nodesEnumerators.Pop();
						return false;
					}
					RTree2D<T>.Node currentNode = enumerator.Current;
					RTree2D<T>.Entry entry = currentNode as RTree2D<T>.Entry;
					if (entry != null) {
						this.current = entry.Value;
						return true;
					}
					else {
						nodesEnumerators.Push(currentNode.Children.GetEnumerator());
						if (MoveNext())
							return true;
					}
				}
			}
			public void Reset() {
				this.nodesEnumerators.Clear();
				this.nodesEnumerators.Push(root.Children.GetEnumerator());
			}
			T GetCurrent() {
				return current;
			}
		}
	}
}
