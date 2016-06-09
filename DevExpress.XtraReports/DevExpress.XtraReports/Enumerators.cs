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
using System.ComponentModel;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports.Native 
{
	public class ObjectEnumeratorBase : IEnumerator 
	{
		private IEnumerator en;
		public ObjectEnumeratorBase(IEnumerable objs) {
			en = objs.GetEnumerator();
		}
		public object Current { get { return en.Current; }
		}
		public virtual bool MoveNext() {
			return en.MoveNext();
		}
		public virtual void Reset() {
			en.Reset();
		}
	}
	public class TypedComponentEnumerator : ObjectEnumeratorBase 
	{
		private Type type;
		public TypedComponentEnumerator(IEnumerable comps, Type type) : base(comps) {
			this.type = type;
		}
		public override bool MoveNext() {
			while( base.MoveNext() ) {
				if( type.IsAssignableFrom(Current.GetType()) )
					return true;
			}
			return false;
		}
	}
	public class ComponentEnumerator : ObjectEnumeratorBase 
	{
		public new XRControl Current { get { return ((IEnumerator)this).Current as XRControl; }
		}
		public ComponentEnumerator(IList comps) : base(comps) {
		}
	}
	public abstract class NestedObjectEnumeratorBase : IEnumerator, IEnumerable 
	{
		#region inner class
		protected class EnumStack : Stack 
		{
			public IEnumerator Enumerator { get { return  (IsEmpty == false) ? (IEnumerator)Peek() : null; }
			}
			public bool IsEmpty { get { return  Count == 0; }
			}
			public EnumStack() {
			}
		}
		#endregion
		protected EnumStack stack;
		private IList objects;
		protected NestedObjectEnumeratorBase(IList objects) {
			this.objects = objects;
			stack = new EnumStack();
		}
		object IEnumerator.Current { get { return stack.Enumerator.Current; }
		}
		public virtual bool MoveNext() {
			if(stack.IsEmpty) {
				stack.Push( GetEnumerator(objects) );
				return stack.Enumerator.MoveNext();
			} 
			IList nestedObjects = GetNestedObjects();
			if(nestedObjects != null && nestedObjects.Count > 0) {
				stack.Push( GetEnumerator(nestedObjects) );
				return stack.Enumerator.MoveNext();
			}
			while(stack.Enumerator.MoveNext() == false) {
				stack.Pop();
				if(stack.IsEmpty) return false; 
			}
			return true;
		}
		protected abstract IList GetNestedObjects();
		protected virtual IEnumerator GetEnumerator(IList objects) {
			return objects.GetEnumerator();
		}
		public virtual void Reset() {
			stack.Clear();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return this;
		}
	}
	public class NestedVisualBricksEnumerator : NestedBricksEnumerator {
		public new VisualBrick Current { get { return (VisualBrick)((IEnumerator)this).Current; } }
		public NestedVisualBricksEnumerator(VisualBrick brick) : base(brick) {
		}
	}
	public class NestedComponentEnumerator : NestedObjectEnumeratorBase {
		Func<XRControl, IList> getNestedObjects;
		public XRControl Current { get { return ((ComponentEnumerator)stack.Enumerator).Current; }
		}
		public NestedComponentEnumerator(IList comps) : this(comps, null) {
		}
		public NestedComponentEnumerator(IList comps, Func<XRControl, IList> getNestedObjects) : base(comps) {
			this.getNestedObjects = getNestedObjects;
		}
		protected override IList GetNestedObjects() {
			return getNestedObjects != null ? getNestedObjects(Current) : Current.Controls;
		}
		protected override IEnumerator GetEnumerator(IList objects) {
			return new ComponentEnumerator(objects);
		}
	}
	public class PageVisualBricksEnumerator : NestedObjectEnumeratorBase {
		public VisualBrick Current { get { return (VisualBrick)stack.Enumerator.Current; } }
		public PageVisualBricksEnumerator(VisualBrick[] bricks)
			: base(bricks) {
		}
		protected override IList GetNestedObjects() {
			return Current.Bricks;
		}
	}
}
