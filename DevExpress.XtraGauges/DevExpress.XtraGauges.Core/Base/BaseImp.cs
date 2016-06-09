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

using DevExpress.Compatibility.System;
using DevExpress.Compatibility.System.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace DevExpress.XtraGauges.Core.Base {
	public class BaseObject : IBaseObject {
		int lockUpdateCounter = 0;
		bool isDisposingCore = false;
		public virtual event EventHandler Changed;
		public event EventHandler Disposed;
		protected BaseObject() {
			OnCreate();
		}
		public void Dispose() {
			if (!IsDisposing) {
				isDisposingCore = true;
				OnDispose();
				RaiseDisposed(EventArgs.Empty);
				Changed = null;
				Disposed = null;
				GC.SuppressFinalize(this);
			}
		}
		protected virtual void OnCreate() { }
		protected virtual void OnDispose() { }
		[Browsable(false)]
		public bool IsDisposing {
			get { return isDisposingCore; }
		}
		protected IDisposable LockUpdate() {
			return new UpdateLocker(this, false);
		}
		protected IDisposable LockUpdate(bool cancelUpdate) {
			return new UpdateLocker(this, cancelUpdate);
		}
		public virtual void BeginUpdate() {
			lockUpdateCounter++;
		}
		public virtual void EndUpdate() {
			if (--lockUpdateCounter == 0) OnUnlockUpdate();
		}
		public virtual void CancelUpdate() {
			lockUpdateCounter--;
		}
		[Browsable(false)]
		public bool IsUpdateLocked {
			get { return lockUpdateCounter > 0 || IsDisposing; }
		}
		protected void OnUnlockUpdate() {
			OnUnlockUpdateCore();
		}
		protected virtual void OnUnlockUpdateCore() {
			OnObjectChanged("EndUpdate");
		}
		protected void OnObjectChanged(string propertyName) {
			OnObjectChanged(new PropertyChangedEventArgs(propertyName));
		}
		protected void OnObjectChanged(EventArgs e) {
			if (IsUpdateLocked || IsDisposing) return;
			using (LockUpdate(true)) {
				OnUpdateObjectCore();
				RaiseChanged(e);
			}
		}
		protected virtual void OnUpdateObjectCore() { }
		protected virtual void RaiseChanged(EventArgs e) {
			if (Changed != null) Changed(this, e);
		}
		protected void RaiseDisposed(EventArgs e) {
			if (Disposed != null) Disposed(this, e);
		}
		class UpdateLocker : IDisposable {
			BaseObject Source;
			bool cancelUpdate;
			public UpdateLocker(BaseObject source, bool cancelUpdate) {
				Source = source;
				this.cancelUpdate = cancelUpdate;
				Source.BeginUpdate();
			}
			public void Dispose() {
				if (cancelUpdate)
					Source.CancelUpdate();
				else Source.EndUpdate();
				Source = null;
			}
		}
	}
	public abstract class BaseObjectEx : BaseObject, ICloneable {
		public object Clone() {
			BaseObject clone = CloneCore();
			if (clone != null) {
				clone.BeginUpdate();
				CopyToCore(clone);
				clone.CancelUpdate();
			}
			return clone;
		}
		protected abstract BaseObject CloneCore();
		protected abstract void CopyToCore(BaseObject clone);
	}
	public abstract class BaseElement<T> : BaseObject, IElement<T>, ISupportCaching, ISupportVisitor<IElement<T>>, ISupportAcceptOrder
		where T : class, IElement<T> {
		string nameCore;
		internal IComposite<T> parentCore;
		internal bool nameLockedCore = false;
		int acceptOrderCore = 0;
		ISupportCaching cachingProviderCore;
		ISite siteCore;
		protected BaseElement()
			: base() {
		}
		protected BaseElement(string name)
			: this() {
			if (!String.IsNullOrEmpty(name)) Name = name;
		}
		protected override void OnCreate() {
			this.cachingProviderCore = CreateISupportCachingProvider();
		}
		protected override void OnDispose() {
			this.parentCore = null;
			Ref.Dispose(ref cachingProviderCore);
			base.OnDispose();
		}
		[Browsable(false)]
		public int AcceptOrder {
			get { return acceptOrderCore; }
			set {
				if (AcceptOrder == value) return;
				acceptOrderCore = value;
				OnPropertyChanged("AcceptOrder");
			}
		}
		protected void OnPropertyChanged(string propName) {
			RaiseChanged(new ElementUpdatedEventArgs<T>(this, propName));
		}
		protected virtual ISupportCaching CreateISupportCachingProvider() {
			return new SimpleCache();
		}
		[Browsable(false)]
		public ISite Site {
			get { return this.siteCore; }
			set { this.siteCore = value; }
		}
		public bool IsNameLocked {
			get { return nameLockedCore; }
		}
		[Bindable(false)]
		[Category("Scale")]
		[DevExpress.Utils.Serializing.XtraSerializableProperty]
		public virtual string Name {
			get { return nameCore; }
			set {
				if (IsNameLocked || Name == value) return;
				nameCore = value;
			}
		}
		public virtual IComposite<T> Parent {
			get { return parentCore; }
		}
		public virtual bool IsLeaf {
			get { return false; }
		}
		public virtual bool IsComposite {
			get { return false; }
		}
		public abstract ILeaf<T> Leaf { get; }
		public abstract IComposite<T> Composite { get; }
		public T Self {
			get { return this as T; }
		}
		bool ISupportCaching.IsCachingLocked {
			get { return cachingProviderCore.IsCachingLocked; }
		}
		void ISupportCaching.LockCaching() {
			cachingProviderCore.LockCaching();
		}
		void ISupportCaching.UnlockCaching() {
			cachingProviderCore.UnlockCaching();
		}
		void ISupportCaching.CacheValue(object key, object value) {
			cachingProviderCore.CacheValue(key, value);
		}
		bool ISupportCaching.HasValue(object key) {
			return cachingProviderCore.HasValue(key);
		}
		object ISupportCaching.TryGetValue(object key) {
			return cachingProviderCore.TryGetValue(key);
		}
		void ISupportCaching.ResetCache(object key) {
			cachingProviderCore.ResetCache(key);
		}
		void ISupportCaching.ResetCache() {
			cachingProviderCore.ResetCache();
		}
		public void Accept(IVisitor<IElement<T>> visitor) {
			AcceptCore(visitor);
		}
		public void Accept(VisitDelegate<IElement<T>> visit) {
			AcceptCore(visit);
		}
		void AcceptCore(IVisitor<IElement<T>> visitor) {
			if (visitor != null) {
				visitor.Visit(this);
				OnAcceptChildren(visitor);
			}
		}
		void AcceptCore(VisitDelegate<IElement<T>> visit) {
			if (visit != null) {
				visit(this);
				OnAcceptChildren(visit);
			}
		}
		protected abstract void OnAcceptChildren(IVisitor<IElement<T>> visitor);
		protected abstract void OnAcceptChildren(VisitDelegate<IElement<T>> visit);
		#region Tag
		object tagCore;
		bool ShouldSerializeTag() { return tagCore != null; }
		void ResetTag() { tagCore = null; }
		[DefaultValue(null), Category("Data")]
#if !DXPORTABLE
		[TypeConverter(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter))]
		[Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor))]
#endif
		public virtual object Tag {
			get { return tagCore; }
			set { tagCore = value; }
		}
		#endregion Tag
	}
	public abstract class BaseLeaf<T> : BaseElement<T>, ILeaf<T>
		where T : class, IElement<T> {
		protected BaseLeaf() : base() { }
		protected BaseLeaf(string name) : base(name) { }
		public sealed override bool IsLeaf {
			get { return true; }
		}
		public sealed override ILeaf<T> Leaf {
			get { return this; }
		}
		public sealed override IComposite<T> Composite {
			get { return null; }
		}
		protected sealed override void OnAcceptChildren(IVisitor<IElement<T>> visitor) { }
		protected sealed override void OnAcceptChildren(VisitDelegate<IElement<T>> visit) { }
	}
	public abstract class BaseComposite<T> : BaseElement<T>, IComposite<T>, ILeaf<T>
		where T : class, IElement<T> {
		BaseElementDictionary<T> collectionCore;
		protected BaseComposite() : base() { }
		protected BaseComposite(string name) : base(name) { }
		BaseElementDictionary<T> Collection {
			get { return collectionCore; }
		}
		protected override void OnCreate() {
			base.OnCreate();
			this.collectionCore = CreateColectionCore();
		}
		protected override void OnDispose() {
			if (Collection != null) {
				Collection.Dispose();
				collectionCore = null;
			}
			base.OnDispose();
		}
		public sealed override bool IsComposite {
			get { return true; }
		}
		public sealed override IComposite<T> Composite {
			get { return this; }
		}
		public sealed override ILeaf<T> Leaf {
			get { return this; }
		}
		protected BaseElementDictionary<T> Elements {
			get { return Collection; }
		}
		protected BaseElementDictionary<T> CreateColectionCore() {
			return new BaseElementDictionary<T>(this);
		}
		void IComposite<T>.Add(IElement<T> element) {
			AddCore(element);
		}
		void IComposite<T>.AddRange(IElement<T>[] elements) {
			foreach (IElement<T> e in elements) AddCore(e);
		}
		void AddCore(IElement<T> element) {
			CompositeException.Assert<T>(element);
			Elements.Add(element);
		}
		void IComposite<T>.Remove(IElement<T> element) {
			Elements.Remove(element);
		}
		IElementsReadOnlyCollection<T> IComposite<T>.Elements {
			get { return Elements; }
		}
		bool ISupportAccessByName<IElement<T>>.Contains(string name) {
			return Elements.Contains(name);
		}
		IElement<T> ISupportAccessByName<IElement<T>>.this[string name] {
			get { return Elements[name]; }
		}
		protected sealed override void OnAcceptChildren(IVisitor<IElement<T>> visitor) {
			Elements.Accept(visitor);
		}
		protected sealed override void OnAcceptChildren(VisitDelegate<IElement<T>> visit) {
			List<IElement<T>> list = new List<IElement<T>>(Elements.Collection.Values);
			list.Sort(new AcceptComparer<IElement<T>>());
			foreach (IElement<T> e in list) e.Accept(visit);
		}
	}
}
