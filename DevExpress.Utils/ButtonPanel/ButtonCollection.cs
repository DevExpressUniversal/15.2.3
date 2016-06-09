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
using System.ComponentModel;
using System.Collections;
namespace DevExpress.XtraEditors.ButtonPanel {
	public interface IButtonCollection<T> : IEnumerable<T> where T : IBaseButton {
		bool Contains(T element);
	}
	[ListBindable(false), TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter)), RefreshProperties(RefreshProperties.All)]
	public class BaseButtonCollection : ButtonCollectionBase<IBaseButton> {
		public BaseButtonCollection(IButtonsPanel panel)
			: base(panel) {
		}
		public IBaseButton this[string caption] {
			get {
				foreach(IBaseButton button in List) {
					if(button.Properties.Caption == caption)
						return button;
				}
				return null;
			}
		}
		protected override void OnElementAdded(IBaseButton button) {
			button.SetOwner(Owner);
			if(Owner != null)
				button.CheckedChanged += Owner.CheckedChanged;
			button.Changed += ButtonChanged;
			button.Disposed += OnElementDisposed;
		}
		protected override void OnElementRemoved(IBaseButton button) {
			if(Owner != null)
				button.CheckedChanged -= Owner.CheckedChanged;
			button.Changed -= ButtonChanged;
			button.Disposed -= OnElementDisposed;
			button.SetOwner(null);
		}
		protected virtual void OnElementDisposed(object sender, EventArgs ea) {
			IBaseButton element = sender as IBaseButton;
			if(List != null && List.Contains(element))
				Remove(element);
		}
		protected override string ToStringCore() {
			if(Count == 0) return "None";
			if(Count == 1)
				return string.Concat("{", ((IBaseButton)List[0]).Properties.Caption, "}");
			return string.Format("Count {0}", Count);
		}
		protected void ButtonChanged(object sender, EventArgs e) {
			if(Owner != null)
				Owner.LayoutChanged();
		}
	}
	public class ButtonCollectionBase<T> : CollectionBase, IButtonCollection<T>
		where T : IBaseButton {
		public IButtonsPanel Owner { get; set; }
		int lockUpdateCore;
		public event CollectionChangeEventHandler CollectionChanged;
		public ButtonCollectionBase(IButtonsPanel panel) {
			lockUpdateCore = 0;
			Owner = panel;
		}
		IEnumerator<T> IEnumerable<T>.GetEnumerator() {
			foreach(T item in List)
				yield return item;
		}
		protected override void OnInsertComplete(int index, object value) {
			base.OnInsertComplete(index, value);
			OnElementAdded((T)value);
			RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, value));
		}
		protected override void OnRemoveComplete(int index, object value) {
			base.OnRemoveComplete(index, value);
			OnElementRemoved((T)value);
			RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, value));
		}
		public T this[int index] { get { return (T)List[index]; } }
		public virtual void AddRange(T[] buttons) {
			BeginUpdate();
			try {
				foreach(T button in buttons) {
					AddButton(button);
				}
			}
			finally {
				EndUpdate();
			}
		}
		public virtual bool Add(T button) {
			return AddButton(button);
		}
		public virtual int IndexOf(T button) { return List.IndexOf(button); }
		public virtual bool Insert(int index, T button) {
			if(CanAdd(button)) {
				List.Insert(index, button);
				return true;
			}
			return false;
		}
		public virtual bool Remove(T element) {
			if(List.Contains(element)) {
				List.Remove(element);
				return !List.Contains(element);
			}
			return false;
		}
		public virtual bool Contains(object element) { return List.Contains(element); }
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public void AddButtonInternal(T button) {
			List.Add(button);
		}
		bool AddButton(T button) {
			if(CanAdd(button)) {
				List.Add(button);
				return true;
			}
			return false;
		}
		protected virtual void OnElementAdded(T button) { }
		protected virtual void OnElementRemoved(T button) { }
		protected override void OnClear() {
			base.OnClear();
			if(Count == 0) return;
			BeginUpdate();
			try {
				for(int n = Count - 1; n >= 0; n--)
					RemoveAt(n);
			}
			finally {
				EndUpdate();
			}
		}
		protected internal void RaiseCollectionChanged(CollectionChangeEventArgs e) {
			if(lockUpdateCore != 0) return;
			if(CollectionChanged != null)
				CollectionChanged(this, e);
		}
		public override string ToString() {
			return ToStringCore();
		}
		protected virtual string ToStringCore() {
			return string.Empty;
		}
		public void CopyTo(Array target, int index) {
			List.CopyTo(target, index);
		}
		public virtual void Merge(IEnumerable<T> collection) {
			Merge(collection, false);
		}
		public virtual void Merge(IEnumerable<T> collection, bool raiseCollectionChanged) {
			BeginUpdate();
			for(int i = List.Count - 1; i >= 0; i--) {
				T button = (T)List[i];
				if(button is IDefaultButton) continue;
				List.Remove(button);
				button.SetMerged(null);
			}
			foreach(T button in collection) {
				if(CanAdd(button)) {
					button.SetMerged(Owner);
					List.Add(button);
				}
			}
			if(raiseCollectionChanged)
				EndUpdate();
			else
				CancelUpdate();
		}
		public virtual void ClearMergedButtons() {
			BeginUpdate();
			for(int i = List.Count - 1; i >= 0; i--) {
				T button = (T)List[i];
				if(button is IDefaultButton) continue;
				List.Remove(button);
				button.SetMerged(null);
			}
			CancelUpdate();
		}
		protected bool CanAdd(T element) {
			return List.IndexOf(element) < 0;
		}
		public virtual void BeginUpdate() {
			lockUpdateCore++;
		}
		public virtual void CancelUpdate() {
			lockUpdateCore--;
		}
		public virtual void EndUpdate() {
			if(--lockUpdateCore == 0)
				RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
		}
		public T[] ToArray() {
			return InnerList.ToArray(typeof(T)) as T[];
		}
		public Array ToArray<TTargetType>() {
			return InnerList.ToArray(typeof(TTargetType)) as TTargetType[];
		}
		public void ForEach(Action<T> action) {
			foreach(T item in InnerList) {
				action(item);
			}
		}
		public T[] CleanUp() {
			T[] elements = ToArray();
			RemoveRange(elements);
			return elements;
		}
		public Array CleanUp<TTargetType>() {
			T[] elements = ToArray();
			TTargetType[] result = ToArray<TTargetType>() as TTargetType[];
			RemoveRange(elements);
			return result;
		}
		public void RemoveRange(T[] elements) {
			for(int i = 0; i < elements.Length; i++)
				Remove(elements[i]);
		}
		public T FindFirst(Predicate<T> match) {
			foreach(T element in List) {
				if(!match(element)) continue;
				return element;
			}
			return default(T);
		}
		public bool Contains(T element) {
			return List.Contains(element);
		}
	}
}
