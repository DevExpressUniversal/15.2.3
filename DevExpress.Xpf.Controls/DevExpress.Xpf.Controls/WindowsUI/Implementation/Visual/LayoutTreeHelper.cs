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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace DevExpress.Xpf.WindowsUI.Base {
	class LayoutTreeHelper {
		public static TTemplatedParent GetTemplateParent<TTemplatedParent>(DependencyObject dObj)
			where TTemplatedParent : DependencyObject {
			return GetTemplateParent<TTemplatedParent, Control>(dObj);
		}
		public static T GetTemplateParent<T, TTemplatedParent>(DependencyObject dObj)
			where T : DependencyObject
			where TTemplatedParent : DependencyObject {
			if(dObj == null)
				return null;
			DependencyObject parent = GetVisualParent(dObj);
			while(parent != null) {
				if(parent is T || parent is TTemplatedParent) break;
				parent = GetVisualParent(parent);
			}
			return parent as T;
		}
		public static bool IsTemplateChild<T>(DependencyObject dObj, T root)
			where T : DependencyObject {
			return FindParentObject<T>(dObj) == root;
		}
		public static T GetTemplateChild<T, TTemplatedParent>(DependencyObject element)
			where T : DependencyObject
			where TTemplatedParent : DependencyObject {
			T result = null;
			using(var e = GetEnumerator<TTemplatedParent>(element, null)) {
				while(e.MoveNext()) {
					if(e.Current is T) {
						result = e.Current as T;
						break;
					}
				}
			}
			return result;
		}
		public static T GetTemplateChildByName<T, TTemplatedParent>(DependencyObject element, string name)
			where T : DependencyObject
			where TTemplatedParent : DependencyObject {
			T result = null;
			using(var e = GetEnumerator<TTemplatedParent>(element, null)) {
				while(e.MoveNext()) {
					if(e.Current is T && ((FrameworkElement)e.Current).Name == name) {
						result = e.Current as T;
						break;
					}
				}
			}
			return result;
		}
		public static T GetTemplateChild<T, TTemplatedParent>(DependencyObject element, bool acceptRoot)
			where T : DependencyObject
			where TTemplatedParent : DependencyObject {
			T result = null;
			using(var e = GetEnumerator<TTemplatedParent>(element, null)) {
				while(e.MoveNext()) {
					if(e.Current is T) {
						if(acceptRoot || e.Current != element) {
							result = e.Current as T;
							break;
						}
					}
				}
			}
			return result;
		}
		public static T GetChild<T>(DependencyObject element)
			where T : DependencyObject {
			if(element != null) {
				if(VisualTreeHelper.GetChildrenCount(element) == 1) {
					return VisualTreeHelper.GetChild(element, 0) as T;
				}
			}
			return null;
		}
		public static IEnumerator<DependencyObject> GetEnumerator<TTemplatedParent>(DependencyObject element, Predicate<DependencyObject> filter) {
			return new TemplateTreeEnumerator<TTemplatedParent>(element, filter);
		}
		static DependencyObject GetVisualParent(DependencyObject dObj) {
			return VisualTreeHelper.GetParent(dObj);
		}
		static T FindParentObject<T>(DependencyObject child) where T : class {
			while(child != null) {
				if(child is T)
					return child as T;
				child = GetVisualParent(child);
			}
			return null;
		}
		class TemplateTreeEnumerator<TTemplatedParent> : IEnumerator<DependencyObject> {
			DependencyObject Root;
			Stack<DependencyObject> Stack;
			Predicate<DependencyObject> Filter;
			public TemplateTreeEnumerator(DependencyObject root, Predicate<DependencyObject> filter) {
				Stack = new Stack<DependencyObject>(16);
				Filter = filter;
				Root = root;
			}
			public void Dispose() {
				Reset();
				Stack = null;
				Filter = null;
			}
			#region IEnumerator Members
			DependencyObject current;
			object System.Collections.IEnumerator.Current {
				get { return current; }
			}
			public DependencyObject Current {
				get { return current; }
			}
			public bool MoveNext() {
				if(current == null) {
					current = Root;
				}
				else {
					int count = VisualTreeHelper.GetChildrenCount(current);
					DependencyObject[] children = new DependencyObject[count];
					for(int i = 0; i < count; i++) {
						children[i] = VisualTreeHelper.GetChild(current, i);
					}
					if(children.Length > 0) {
						for(int i = 0; i < children.Length; i++) {
							DependencyObject child = children[(children.Length - 1) - i];
							if(child is TTemplatedParent) continue;
							if(Filter == null || Filter(child))
								Stack.Push(child);
						}
					}
					current = Stack.Count > 0 ? Stack.Pop() : null;
				}
				return current != null;
			}
			public void Reset() {
				Stack.Clear();
				current = null;
			}
			#endregion
		}
	}
}
