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
using System.Linq;
using System.Text;
#if MVVM
namespace DevExpress.Mvvm.Native {
#else
namespace DevExpress.Internal {
#endif
#if MVVM
	public
#endif
	interface IImmutableStack<out T> : IEnumerable<T> {
		T Peek();
		IImmutableStack<T> Pop();
		bool IsEmpty { get; }
	}
#if MVVM
	public
#endif
	static class ImmutableStack {
		class EmptyStack<T> : IImmutableStack<T> {
			public static readonly IImmutableStack<T> Instance = new EmptyStack<T>();
			EmptyStack() { }
			T IImmutableStack<T>.Peek() { throw new InvalidOperationException(); }
			IImmutableStack<T> IImmutableStack<T>.Pop() { throw new InvalidOperationException();  }
			IEnumerator<T> IEnumerable<T>.GetEnumerator() {
				yield break;
			}
			IEnumerator IEnumerable.GetEnumerator() {
				yield break;
			}
			bool IImmutableStack<T>.IsEmpty { get { return true; } }
		}
		class SimpleStack<T> : IImmutableStack<T> {
			readonly T head;
			readonly IImmutableStack<T> tail;
			T IImmutableStack<T>.Peek() { return head; }
			IImmutableStack<T> IImmutableStack<T>.Pop() { return tail; }
			bool IImmutableStack<T>.IsEmpty { get { return false; } }
			public SimpleStack(T head, IImmutableStack<T> tail) {
				this.head = head;
				this.tail = tail;
			}
			IEnumerator<T> IEnumerable<T>.GetEnumerator() {
				return GetEnumeratorCore();
			}
			IEnumerator IEnumerable.GetEnumerator() {
				return GetEnumeratorCore();
			}
			IEnumerator<T> GetEnumeratorCore() {
				return LinqExtensions.Unfold<IImmutableStack<T>>(this, x => x.Pop(), x => x.IsEmpty)
					.Select(x => x.Peek()).GetEnumerator();
			}
		}
		public static IImmutableStack<T> Empty<T>() {
			return EmptyStack<T>.Instance;
		}
		public static IImmutableStack<T> Push<T>(this IImmutableStack<T> stack, T item) {
			return new SimpleStack<T>(item, stack);
		}
		public static IImmutableStack<T> PushMultiple<T>(this IImmutableStack<T> source, IEnumerable<T> items) {
			return items.Aggregate(source, (stack, x) => stack.Push(x));
		}
		public static IImmutableStack<T> Reverse<T>(this IImmutableStack<T> stack) {
			var reverse = Empty<T>();
			while(!stack.IsEmpty) {
				reverse = reverse.Push(stack.Peek());
				stack = stack.Pop();
			}
			return reverse;
		}
		public static IImmutableStack<T> ToImmutableStack<T>(this IEnumerable<T> source) {
			return Empty<T>().PushMultiple(source.Reverse());
		}
	}
}
