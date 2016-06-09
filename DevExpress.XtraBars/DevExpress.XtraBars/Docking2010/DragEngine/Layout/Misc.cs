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
namespace DevExpress.XtraBars.Docking2010.DragEngine {
	public class LayoutElementEnumerator : IEnumerator<ILayoutElement> {
		Stack<ILayoutElement> Stack;
		ILayoutElement Root;
		ILayoutElement current;
		public LayoutElementEnumerator(ILayoutElement root) {
			Stack = new Stack<ILayoutElement>(16);
			Root = root;
		}
		void IDisposable.Dispose() {
			ResetCore();
			Stack = null;
			Root = null;
			GC.SuppressFinalize(this);
		}
		#region IEnumerator Members
		ILayoutElement IEnumerator<ILayoutElement>.Current {
			get { return current; }
		}
		object IEnumerator.Current {
			get { return current; }
		}
		bool IEnumerator.MoveNext() {
			if(current == null) {
				current = Root;
			}
			else {
				int nodesCount = current.Nodes.Length;
				if(nodesCount > 0) {
					for(int i = 0; i < nodesCount; i++) {
						ILayoutElement child = current.Nodes[(nodesCount - 1) - i];
						Stack.Push(child);
					}
				}
				current = Stack.Count > 0 ? Stack.Pop() : null;
			}
			return current != null;
		}
		void IEnumerator.Reset() {
			ResetCore();
		}
		void ResetCore() {
			if(Stack != null)
				Stack.Clear();
			current = null;
		}
		#endregion
	}
	public class IUIElementEnumerator : IEnumerator<IUIElement> {
		IUIElement Root;
		Stack<IUIElement> Stack;
		Predicate<IUIElement> Filter;
		public IUIElementEnumerator(IUIElement element)
			: this(element, null) {
		}
		public IUIElementEnumerator(IUIElement element, Predicate<IUIElement> filter) {
			Stack = new Stack<IUIElement>(8);
			Root = element;
			Filter = filter;
		}
		public void Dispose() {
			Reset();
			Root = null;
			Stack = null;
			Filter = null;
			GC.SuppressFinalize(this);
		}
		#region IEnumerator Members
		IUIElement current;
		object IEnumerator.Current {
			get { return current; }
		}
		public IUIElement Current {
			get { return current; }
		}
		public bool MoveNext() {
			if(current == null) {
				current = Root;
			}
			else {
				if(current.Children != null) {
					IUIElement[] children = current.Children.GetElements();
					if(children.Length > 0) {
						for(int i = 0; i < children.Length; i++) {
							IUIElement child = children[(children.Length - 1) - i];
							if(Filter == null || Filter(child)) {
								Stack.Push(child);
							}
						}
					}
				}
				current = Stack.Count > 0 ? Stack.Pop() : null;
			}
			return current != null;
		}
		public void Reset() {
			if(Stack != null)
				Stack.Clear();
			current = null;
		}
		#endregion
	}
}
