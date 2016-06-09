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
using System.Windows;
using System.Collections.Generic;
using System.Linq;
#if !NETFX_CORE
using System.Windows.Media;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#endif
#if DXWINDOW
namespace DevExpress.Internal.DXWindow {
#elif DESIGN || SLDESIGN
namespace DevExpress.Design.UI {
#elif MVVM
namespace DevExpress.Mvvm.UI.Native {
#else
namespace DevExpress.Xpf.Core.Native {
#endif
	public enum EnumeratorDirection { Forward, Backward }
	public abstract class NestedObjectEnumeratorBase : IEnumerator {
		#region inner class
		protected class EnumStack : Stack<IEnumerator> {
			public IEnumerator TopEnumerator {
				get { return (IsEmpty == false) ? Peek() : null; }
			}
			public bool IsEmpty {
				get { return Count == 0; }
			}
			public EnumStack() {
			}
		}
		#endregion
		protected static readonly IEnumerator EmptyEnumerator = (new object[0]).GetEnumerator();
		IEnumerator objects;
		protected EnumStack stack;
		protected IEnumerator Enumerator { get { return (IEnumerator)this; } }
		public object CurrentParent {
			get { return GetParents().FirstOrDefault(); }
		}
		public IEnumerable<object> GetParents() {
			IEnumerator<IEnumerator> en = stack.GetEnumerator();
			if(en.MoveNext()) {
				while(en.MoveNext()) {
					yield return en.Current.Current;
				}
			}
		}
		public int Level { get { return stack.Count; } }
		protected NestedObjectEnumeratorBase(object obj) {
			this.objects = new object[] { obj }.GetEnumerator();
			stack = new EnumStack();
			Reset();
		}
		object IEnumerator.Current {
			get { return stack.TopEnumerator.Current; }
		}
		public virtual bool MoveNext() {
			if(stack.IsEmpty) {
				stack.Push(objects);
				return stack.TopEnumerator.MoveNext();
			}
			IEnumerator nestedObjects = GetNestedObjects(Enumerator.Current);
			if(nestedObjects.MoveNext()) {
				stack.Push(nestedObjects);
				return true;
			}
			while(stack.TopEnumerator.MoveNext() == false) {
				stack.Pop();
				if(stack.IsEmpty)
					return false;
			}
			return true;
		}
		protected abstract IEnumerator GetNestedObjects(object obj);
		public virtual void Reset() {
			stack.Clear();
			objects.Reset();
		}
	}
	public class VisualTreeEnumerator : NestedObjectEnumeratorBase, IDisposable{
		static IEnumerator<object> GetDependencyObjectEnumerator(DependencyObject dObject, int startIndex, int endIndex, int step) {
			for(int i = startIndex; i != endIndex; i += step) {
				yield return VisualTreeHelper.GetChild(dObject, i);
			}
		}
		EnumeratorDirection direction = EnumeratorDirection.Forward;
		public DependencyObject Current {
			get { return (DependencyObject)Enumerator.Current; }
		}
		public VisualTreeEnumerator(DependencyObject dObject)
			: this(dObject, EnumeratorDirection.Forward) {
		}
		protected VisualTreeEnumerator(DependencyObject dObject, EnumeratorDirection direction)
			: base(dObject) {
			this.direction = direction;
		}
		public void Dispose() {
			Reset();
		}
		protected virtual bool IsObjectVisual(DependencyObject d) {
#if !SILVERLIGHT && !NETFX_CORE || SLDESIGN
			return d is Visual;
#else
			return d is UIElement;
#endif
		}
		protected override IEnumerator GetNestedObjects(object obj) {
			DependencyObject dObject = (DependencyObject)obj;
			int count = IsObjectVisual(dObject) ? VisualTreeHelper.GetChildrenCount(dObject) : 0;
			return direction == EnumeratorDirection.Forward ?
				GetDependencyObjectEnumerator(dObject, 0, count, 1) :
				GetDependencyObjectEnumerator(dObject, count - 1, -1, -1);
		}
		public IEnumerable<DependencyObject> GetVisualParents() {
			return GetParents().Cast<DependencyObject>();
		}
	}
#if !SILVERLIGHT && !NETFX_CORE
	public class LogicalTreeEnumerator : VisualTreeEnumerator {
		Hashtable acceptedVisuals = new Hashtable();
		static IEnumerator GetVisualAndLogicalChildren(object obj, IEnumerator visualChildren, bool dependencyObjectsOnly, Hashtable acceptedVisuals) {
			while(visualChildren.MoveNext()) {
				var visual = visualChildren.Current;
				if(visual != null) {
					if(acceptedVisuals.ContainsKey(visual))
						continue;
					acceptedVisuals.Add(visual, visual);
				}
				yield return visual;
			}
			foreach(object logicalChild in LogicalTreeHelper.GetChildren((DependencyObject)obj)) {
				if(dependencyObjectsOnly && !(logicalChild is DependencyObject)) continue;
				if(acceptedVisuals.ContainsKey(logicalChild))
					continue;
				yield return logicalChild;
			}
		}
		public LogicalTreeEnumerator(DependencyObject dObject)
			: base(dObject) {
		}
		public override void Reset() {
			acceptedVisuals.Clear();
			base.Reset();
		}
		protected virtual bool DependencyObjectsOnly { get { return false; } }
		protected override IEnumerator GetNestedObjects(object obj) {
			return GetVisualAndLogicalChildren(obj, base.GetNestedObjects(obj), DependencyObjectsOnly, acceptedVisuals);
		}
	}
	public class SerializationEnumerator : LogicalTreeEnumerator {
		Func<DependencyObject, bool> nestedChildrenPredicate;
		protected override bool DependencyObjectsOnly {
			get { return true; }
		}
		protected override IEnumerator GetNestedObjects(object obj) {
			if(nestedChildrenPredicate((DependencyObject)obj))
				return base.GetNestedObjects(obj);
			return Enumerable.Empty<object>().GetEnumerator();
		}
		public SerializationEnumerator(DependencyObject dObject, Func<DependencyObject, bool> nestedChildrenPredicate)
			: base(dObject) {
			this.nestedChildrenPredicate = nestedChildrenPredicate;
		}
	}
	public class VisualTreeEnumeratorWithConditionalStop : IEnumerator<DependencyObject> {
		DependencyObject Root;
		Stack<DependencyObject> Stack;
		Predicate<DependencyObject> TreeStop;
		public VisualTreeEnumeratorWithConditionalStop(DependencyObject root, Predicate<DependencyObject> treeStop) {
			Stack = new Stack<DependencyObject>();
			Root = root;
			TreeStop = treeStop;
		}
		public void Dispose() {
			Reset();
			Stack = null;
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
						if(TreeStop != null && TreeStop(child)) continue;
						Stack.Push(child);
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
#endif
	public class SingleObjectEnumerator : VisualTreeEnumerator {
		public SingleObjectEnumerator(DependencyObject dObject)
			: base(dObject) {
		}
		protected override IEnumerator GetNestedObjects(object obj) {
			return EmptyEnumerator;
		}
	}
}
