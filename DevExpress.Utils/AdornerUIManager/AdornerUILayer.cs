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
using System.Drawing;
using DevExpress.Utils.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Utils.Drawing.Helpers;
using System.Collections;
using System.Security;
namespace DevExpress.Utils.VisualEffects {
	public abstract class AdornerUILayerBase : NativeWindow, IDisposable {
		const int
			WS_POPUP = unchecked((int)0x80000000),
			WS_EX_LAYERED = 0x80000,
			ULW_ALPHA = 0x02;
		const byte alphaCore = 255;
		byte AC_SRC_OVER = 0x00;
		byte AC_SRC_ALPHA = 0x01;
		Rectangle bounds = Rectangle.Empty;
		bool isVisibleCore = false;
		Control ownerCore;
		bool isDisposing;
		Size lastSize = Size.Empty;
		public AdornerUILayerBase(Control owner) {
			ownerCore = owner;
			Create(ownerCore.Handle);
			Initialize();
		}
		public bool IsCreated { get { return Handle != IntPtr.Zero; } }
		public bool IsVisible { get { return isVisibleCore; } }
		public Rectangle Bounds { get { return bounds; } }
		public virtual Point Location {
			get { return bounds.Location; }
			set {
				if(Location == value) return;
				bounds.Location = value;
				OnWindowChanged();
			}
		}
		public virtual Size Size {
			get { return bounds.Size; }
			set {
				if(Size == value) return;
				bounds.Size = value;
				OnWindowChanged();
			}
		}
		public void Show() {
			if(IsCreated && IsVisible) return;
			ShowCore();
		}
		public Control Owner { get { return ownerCore; } }
		[SecuritySafeCritical]
		public void Hide() {
			if(!IsCreated || !IsVisible) return;
			NativeMethods.SetWindowPos(Handle, hWndInsertAfter, 0, 0, 0, 0,
				NativeMethods.SWP.SWP_NOACTIVATE | NativeMethods.SWP.SWP_HIDEWINDOW | NativeMethods.SWP.SWP_NOSIZE | NativeMethods.SWP.SWP_NOMOVE
				| NativeMethods.SWP.SWP_NOZORDER | NativeMethods.SWP.SWP_NOOWNERZORDER);
			this.isVisibleCore = false;
		}
		public void Update() {
			if(CheckBounds()) return;
			UpdateLayeredWindowCore(DrawToBackBuffer);
		}
		public void Clear() {
			if(CheckBounds()) return;
			UpdateLayeredWindowCore(null);
		}
		protected void Create(IntPtr handle) {
			if(IsCreated) return;
			CreateParams cp = new CreateParams();
			cp.Parent = handle;
			cp.Style = WS_POPUP;
			cp.ExStyle = WS_EX_LAYERED;
			cp.Caption = null;
			CreateHandle(cp);
		}
		protected virtual void Initialize() {
			if(ownerCore == null) return;
			bounds = ownerCore.ClientRectangle;
		}
		protected virtual IntPtr hWndInsertAfter { get { return new IntPtr(0); } }
		protected virtual void OnWindowChanged() {
			if(!IsCreated || !IsVisible) return;
			ShowCore();
		}
		[SecuritySafeCritical]
		protected void ShowCore() {
			if(!IsCreated) return;
			this.isVisibleCore = true;
			int flags = NativeMethods.SWP.SWP_NOACTIVATE | NativeMethods.SWP.SWP_SHOWWINDOW | NativeMethods.SWP.SWP_DRAWFRAME |
				NativeMethods.SWP.SWP_NOOWNERZORDER;
			if(this.lastSize == Size) {
				flags |= NativeMethods.SWP.SWP_NOSIZE | NativeMethods.SWP.SWP_NOZORDER;
			}
			this.lastSize = Size;
			NativeMethods.SetWindowPos(Handle, hWndInsertAfter, Bounds.X, Bounds.Y, Size.Width, Size.Height, flags);
		}
		protected override void WndProc(ref Message m) {
			switch(m.Msg) {
				case MSG.WM_NCHITTEST:
					NCHitTest(ref m);
					return;
			}
			base.WndProc(ref m);
		}
		protected virtual void NCHitTest(ref Message m) { m.Result = new IntPtr(NativeMethods.HT.HTTRANSPARENT); }
		protected void UpdateLayeredWindowCore(Action<IntPtr> updateBackBufferCallback) {
			using(Bitmap bmp = new Bitmap(Bounds.Width, Bounds.Height)) {
				using(Graphics g = Graphics.FromImage(bmp)) {
					IntPtr dc = g.GetHdc();
					IntPtr backBufferDC = NativeMethods.CreateCompatibleDC(dc);
					IntPtr hBufferBitmap = DevExpress.Utils.Internal.DXLayeredWindowEx.Create32bppDIBSection(dc, Bounds.Width, Bounds.Height);
					IntPtr tmp = IntPtr.Zero;
					try {
						tmp = NativeMethods.SelectObject(backBufferDC, hBufferBitmap);
						if(updateBackBufferCallback != null)
							updateBackBufferCallback(backBufferDC);
						NativeMethods.POINT newLocation = new NativeMethods.POINT(Bounds.Location);
						NativeMethods.SIZE newSize = new NativeMethods.SIZE(Bounds.Size);
						NativeMethods.POINT sourceLocation = new NativeMethods.POINT(0, 0);
						NativeMethods.BLENDFUNCTION blend = new NativeMethods.BLENDFUNCTION();
						blend.BlendOp = AC_SRC_OVER;
						blend.BlendFlags = 0;
						blend.SourceConstantAlpha = alphaCore;
						blend.AlphaFormat = AC_SRC_ALPHA;
						NativeMethods.UpdateLayeredWindow(Handle, dc, ref newLocation, ref newSize, backBufferDC, ref sourceLocation, 0, ref blend, ULW_ALPHA);
					}
					finally {
						NativeMethods.SelectObject(backBufferDC, tmp);
						NativeMethods.DeleteObject(hBufferBitmap);
						NativeMethods.DeleteDC(backBufferDC);
						g.ReleaseHdc(dc);
					}
				}
			}
		}
		[SecuritySafeCritical]
		protected void DrawToBackBuffer(IntPtr backBufferDC) {
			Rectangle rect = new Rectangle(0, 0, Bounds.Width, Bounds.Height);
			using(XtraBufferedGraphics bg = XtraBufferedGraphicsManager.Current.Allocate(backBufferDC, rect)) {
				using(GraphicsCache cache = new GraphicsCache(bg.Graphics))
					DrawCore(cache);
				bg.Render();
			}
		}
		bool CheckBounds() {
			if(Bounds.Width <= 0 || Bounds.Height <= 0) return true;
			return false;
		}
		protected abstract void DrawCore(GraphicsCache cache);
		public void Dispose() {
			if(!isDisposing) {
				isDisposing = true;
				OnDisposing();
			}
		}
		protected virtual void OnDisposing() {
			if(IsCreated)
				DestroyHandle();
			ownerCore = null;
		}
	}
	public class AdornerUILayer : AdornerUILayerBase {
		IList<IAdornerElement> elementCollectionCore;
		bool isDirtyRegion = false;
		public AdornerUILayer(Control owner)
			: base(owner) {
			elementCollectionCore = new List<IAdornerElement>();
		}
		public void RegisterElement(IAdornerElement element) {
			if(elementCollectionCore.Contains(element)) return;
			if(element.TargetElement is ISupportAdornerElementBarItem) return;
			elementCollectionCore.Add(element);
		}
		internal void RegisterElementWrapper(BarItemAdornerWrapper wrapper) {
			foreach(IAdornerElement element in wrapper.Children)
				RegisterElement(element);
		}
		public void UnregisterElement(IAdornerElement element) {
			if(!elementCollectionCore.Contains(element)) return;
			elementCollectionCore.Remove(element);
		}
		public void Reset() { elementCollectionCore.Clear(); }
		Rectangle GetTargetElementBounds(AdornerElementContext context) {
			if(context.TargetElement == null) return Rectangle.Empty;
			if(context.TargetElement == Owner) return ClientBounds;
			Rectangle bounds = context.TargetBounds;
			if(context.Parent == Owner) return bounds;
			Point screenLocation = GetScreenLocation(context.Handle, IntPtr.Zero, bounds.Location);
			return new Rectangle(screenLocation.X - Bounds.X, screenLocation.Y - Bounds.Y, bounds.Width, bounds.Height);
		}
		public override Point Location {
			get { return base.Location; }
			set {
				Point newLocation = GetScreenLocation(Owner.Handle, IntPtr.Zero, value);
				base.Location = newLocation;
			}
		}
		protected Point GetScreenLocation(IntPtr source, IntPtr target, Point p) {
			NativeMethods.POINT pt = new NativeMethods.POINT(p.X, p.Y);
			NativeMethods.MapWindowPoints(source, target, ref pt, 1);
			return new Point(pt.X, pt.Y);
		}
		void CalcAdornerElement(GraphicsCache cache, AdornerElementContext context) {
			if(context.Element.ViewInfo.IsReady) return;
			Rectangle targetBounds = GetTargetElementBounds(context);
			context.Element.ViewInfo.Calc(cache.Graphics, targetBounds);
			isDirtyRegion = false;
		}
		Rectangle ClientBounds { get { return new Rectangle(Point.Empty, Bounds.Size); } }
		[System.Security.SecuritySafeCritical]
		protected override void DrawCore(GraphicsCache cache) {
			List<Rectangle> rects = new List<Rectangle>();
			foreach(IAdornerElement element in elementCollectionCore) {
				if(!element.IsVisible) continue;
				AppearanceHelper.Combine(element.ViewInfo.PaintAppearance, new AppearanceObject[] { element.Appearance, element.ParentAppearance }, element.Painter.DefaultAppearance);
				using(AdornerElementContext context = new AdornerElementContext(element)) {
					IEnumerable<Rectangle> regions = DrawAdornerElement(cache, context);
					if(regions != null)
						rects.AddRange(regions);
				}
			}
			CheckWindowRegion(rects);
		}
		protected virtual IEnumerable<Rectangle> DrawAdornerElement(GraphicsCache cache, AdornerElementContext context) {
			Rectangle clipBounds = GetClipBounds(context);
			if(clipBounds.IsEmpty) return null;
			CalcAdornerElement(cache, context);
			IAdornerElement element = context.Element;
			Rectangle elementBounds = element.ViewInfo.Bounds;
			if(elementBounds.IsEmpty) return null;
			if(Rectangle.Intersect(ClientBounds, elementBounds).IsEmpty) return null;
			GraphicsClipState oldState = cache.ClipInfo.SaveAndSetClip(clipBounds);
			ObjectPainter.DrawObject(cache, element.Painter, element.ViewInfo as ObjectInfoArgs);
			cache.ClipInfo.RestoreClipRelease(oldState);
			return element.ViewInfo.CalcRegions();
		}
		Rectangle GetClipBounds(AdornerElementContext context) {
			if(context.TargetElement == null) return Rectangle.Empty;
			if(context.TargetElement == Owner) return ClientBounds;
			if(context.Parent == Owner) return ClientBounds;
			Rectangle clipRectangle = context.ClipRectangle;
			Point screenLocation = GetScreenLocation(context.Handle, IntPtr.Zero, clipRectangle.Location);
			screenLocation.Offset(-Bounds.X, -Bounds.Y);
			clipRectangle.Location = screenLocation;
			return clipRectangle;
		}
		void CheckWindowRegion(IEnumerable<Rectangle> regions) {
			if(isDirtyRegion) return;
			using(Region region = new Region()) {
				region.MakeEmpty();
				foreach(Rectangle r in regions) {
					if(r.IsEmpty) continue;
					region.Union(r);
				}
				using(Graphics g = Graphics.FromHwndInternal(Handle)) {
					DevExpress.Utils.Drawing.Helpers.NativeMethods.SetWindowRgn(Handle, region.GetHrgn(g), false);
				}
				isDirtyRegion = true;
			}
		}
		public void Update(bool updateRegions) {
			if(updateRegions)
				isDirtyRegion = false;
			Update();
		}
		protected override void OnDisposing() {
			Reset();
			base.OnDisposing();
		}
		protected class AdornerElementContext : IDisposable {
			IAdornerElement elementCore;
			IntPtr handleCore;
			Rectangle clipRectangleCore;
			Rectangle targetBoundsCore;
			object parentCore;
			object targetElementCore;
			public AdornerElementContext(IAdornerElement element) {
				elementCore = element;
				if(element.TargetElement is ISupportAdornerElement)
					Init(element.TargetElement as ISupportAdornerElement);
				else
					Init(element.TargetElement as Control);
			}
			void Init(ISupportAdornerElement targetElement) {
				if(targetElement == null) return;
				ISupportAdornerUIManager owner = targetElement.Owner;
				if(owner != null && owner.IsHandleCreated) {
					handleCore = owner.Handle;
					targetBoundsCore = targetElement.Bounds;
					clipRectangleCore = GetClipRectangle(handleCore, owner.ClientRectangle);
					parentCore = owner;
				}
				targetElementCore = targetElement;
			}
			void Init(Control targetElement) {
				if(targetElement == null) return;
				Control parent = targetElement.Parent;
				targetBoundsCore = targetElement.Bounds;
				if(parent != null && parent.IsHandleCreated) {
					handleCore = parent.Handle;
					clipRectangleCore = GetClipRectangle(parent, parent.ClientRectangle);
					parentCore = parent;
				}
				targetElementCore = targetElement;
			}
			Rectangle GetClipRectangle(IntPtr handle, Rectangle client) {
				if(handle == IntPtr.Zero) return client;
				return GetClipRectangle(Control.FromHandle(handle), client);
			}
			Rectangle GetClipRectangle(Control control, Rectangle client) {
				if(control == null) return client;
				NativeMethods.RECT rect;
				NativeMethods.GetClientRect(control.Handle, out rect);
				return GetClipRectangleCore(control.Parent, rect.ToRectangle());
			}
			System.Drawing.Rectangle GetClipRectangleCore(System.Windows.Forms.Control parent, System.Drawing.Rectangle clientRect) {
				if(parent == null) return clientRect;
				NativeMethods.RECT rect;
				NativeMethods.GetClientRect(parent.Handle, out rect);
				Rectangle parentRect = rect.ToRectangle();
				System.Drawing.Rectangle newBounds = System.Drawing.Rectangle.Intersect(parentRect, clientRect);
				return GetClipRectangleCore(parent.Parent, newBounds);
			}
			public Rectangle TargetBounds { get { return targetBoundsCore; } }
			public object Parent { get { return parentCore; } }
			public Rectangle ClipRectangle { get { return clipRectangleCore; } }
			public IntPtr Handle { get { return handleCore; } }
			public object TargetElement { get { return targetElementCore; } }
			public IAdornerElement Element { get { return elementCore; } }
			#region IDisposable Members
			void IDisposable.Dispose() {
				targetElementCore = null;
				parentCore = null;
				handleCore = IntPtr.Zero;
				elementCore = null;
			}
			#endregion
		}
	}
	[Editor("DevExpress.Utils.Design.AdornerElementCollectionEditor, " + AssemblyInfo.SRAssemblyDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
	public class AdornerElementCollection : CollectionBase, IList<AdornerElement>, IEnumerable<AdornerElement>, ICollection<AdornerElement> {
		event CollectionChangeEventHandler collectionChangedCore;
		public AdornerElementCollection() { }
		public AdornerElement[] GetElementsBySource(object source) {
			List<AdornerElement> elements = new List<AdornerElement>();
			foreach(var element in this) {
				if(element.TargetElement == source) {
					elements.Add(element);
				}
			}
			return elements.ToArray();
		}
		public TType[] GetElementsBySource<TType>(object source) where TType : AdornerElement {
			List<TType> elements = new List<TType>();
			foreach(var element in this) {
				TType el = element as TType;
				if(el != null && el.TargetElement == source) {
					elements.Add(el);
				}
			}
			return elements.ToArray();
		}
		#region ICollection Members
		public void Add(AdornerElement item) {
			if(item == null || item.IsDisposing || Contains(item)) return;
			List.Add(item);
		}
		public void AddRange(IEnumerable<AdornerElement> items) {
			if(items == null) return;
			foreach(AdornerElement item in items)
				Add(item);
		}
		protected override void OnClear() {
			try {
				for(int n = Count - 1; n >= 0; n--)
					RemoveAt(n);
			}
			finally { }
		}
		protected override void OnRemoveComplete(int index, object value) {
			base.OnRemoveComplete(index, value);
			RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, value));
		}
		protected override void OnInsertComplete(int index, object value) {
			base.OnInsertComplete(index, value);
			RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, value));
		}
		public bool Contains(AdornerElement item) { return List.Contains(item); }
		public void CopyTo(AdornerElement[] array, int arrayIndex) { List.CopyTo(array, arrayIndex); }
		public bool Remove(AdornerElement item) {
			if(item == null || !Contains(item)) return false;
			List.Remove(item);
			return true;
		}
		#endregion
		#region IList Members
		public bool IsReadOnly { get { return List.IsReadOnly; } }
		public int IndexOf(AdornerElement item) { return List.IndexOf(item); }
		void IList<AdornerElement>.Insert(int index, AdornerElement item) { }
		public AdornerElement this[int index] {
			get {
				if(index < 0 || index >= Count) return null;
				return (AdornerElement)List[index];
			}
			set { }
		}
		#endregion
		#region IEnumerable Members
		public new IEnumerator<AdornerElement> GetEnumerator() {
			foreach(AdornerElement item in List)
				yield return item;
		}
		#endregion
		protected virtual void RaiseCollectionChanged(CollectionChangeEventArgs e) {
			if(collectionChangedCore == null) return;
			collectionChangedCore(this, e);
		}
		public event CollectionChangeEventHandler CollectionChanged {
			add { collectionChangedCore += value; }
			remove { collectionChangedCore -= value; }
		}
	}
	class AdornerWrapperCollection : CollectionBase, IList<BarItemAdornerWrapper>, IEnumerable<BarItemAdornerWrapper>, ICollection<BarItemAdornerWrapper> {
		public AdornerWrapperCollection() { }
		#region ICollection Members
		public void Add(BarItemAdornerWrapper item) {
			if(item == null || Contains(item)) return;
			List.Add(item);
		}
		public void AddRange(IEnumerable<BarItemAdornerWrapper> items) {
			if(items == null) return;
			foreach(BarItemAdornerWrapper item in items)
				Add(item);
		}
		protected override void OnClear() {
			try {
				for(int n = Count - 1; n >= 0; n--)
					RemoveAt(n);
			}
			finally { }
		}
		public bool Contains(BarItemAdornerWrapper item) { return List.Contains(item); }
		public void CopyTo(BarItemAdornerWrapper[] array, int arrayIndex) { List.CopyTo(array, arrayIndex); }
		public bool Remove(BarItemAdornerWrapper item) {
			if(item == null || !Contains(item)) return false;
			List.Remove(item);
			return true;
		}
		#endregion
		#region IList Members
		public bool IsReadOnly { get { return List.IsReadOnly; } }
		public int IndexOf(BarItemAdornerWrapper item) { return List.IndexOf(item); }
		void IList<BarItemAdornerWrapper>.Insert(int index, BarItemAdornerWrapper item) { }
		public BarItemAdornerWrapper this[int index] {
			get {
				if(index < 0 || index >= Count) return null;
				return (BarItemAdornerWrapper)List[index];
			}
			set { }
		}
		public BarItemAdornerWrapper this[IAdornerElement element] {
			get {
				if(List != null) {
					foreach(var wrapper in this) {
						if(wrapper.Root == element) return wrapper;
					}
				}
				return null;
			}
		}
		#endregion
		#region IEnumerable Members
		public new IEnumerator<BarItemAdornerWrapper> GetEnumerator() {
			foreach(BarItemAdornerWrapper item in List)
				yield return item;
		}
		#endregion
	}
	class BarItemAdornerWrapper : IDisposable {
		IAdornerUIManagerInternal managerCore;
		IAdornerElement rootElementCore;
		IList<IAdornerElement> childrenCore;
		public IEnumerable<IAdornerElement> Children { get { return childrenCore; } }
		public IAdornerElement Root { get { return rootElementCore; } }
		ISupportAdornerElementBarItem targetCore;
		ISupportAdornerElementBarItem Target { get { return targetCore; } }
		public BarItemAdornerWrapper(IAdornerElement rootElement, IAdornerUIManagerInternal manager) {
			this.rootElementCore = rootElement;
			this.managerCore = manager;
			this.childrenCore = new List<IAdornerElement>();
			this.targetCore = rootElementCore.TargetElement as ISupportAdornerElementBarItem;
			Init();
		}
		protected virtual void DestroyChildren() {
			if(childrenCore == null) return;
			foreach(var child in Children) {
				managerCore.UnregisterElement(child);
				child.Dispose();
			}
			childrenCore.Clear();
		}
		protected virtual void RefreshChildren() {
			DestroyChildren();
			GenerateChildren();
			managerCore.UpdateLayer(true);
		}
		protected virtual void GenerateChildren() {
			if(Target == null || Target.Elements == null) return;
			foreach(var element in Target.Elements) {
				IAdornerElement clone = rootElementCore.Clone() as IAdornerElement;
				clone.TargetElement = element;
				childrenCore.Add(clone);
				managerCore.RegisterElement(clone);
			}
		}
		protected virtual void UpdateChildren() {
			if(childrenCore == null) return;
			foreach(var child in Children)
				child.Assign(rootElementCore);
			managerCore.UpdateLayer(true);
		}
		protected virtual void Init() {
			rootElementCore.TargetChanged += OnRootElementTargetChanged;
			rootElementCore.Updated += OnRootElementUpdated;
			SubscribeTarget();
			GenerateChildren();
		}
		void SubscribeTarget() {
			if(Target == null) return;
			Target.Changed += OnChanged;
			Target.CollectionChanged += OnCollectionChanged;
		}
		void UnsubscribeTarget() {
			if(Target == null) return;
			Target.Changed -= OnChanged;
			Target.CollectionChanged -= OnCollectionChanged;
		}
		void OnCollectionChanged(object sender, System.ComponentModel.CollectionChangeEventArgs e) {
			RefreshChildren();
		}
		void OnChanged(object sender, UpdateActionEvendArgs e) {
			switch(e.Action) {
				case UpdateAction.OwnerChanged:
					RefreshChildren();
					break;
				case UpdateAction.Dispose:
					rootElementCore.TargetElement = null;
					break;
				case UpdateAction.BeginUpdate:
					managerCore.BeginUpdate();
					break;
				case UpdateAction.EndUpdate:
					UpdateChildren();
					managerCore.EndUpdate();
					break;
				default:
					UpdateChildren();
					break;
			}
		}
		void OnRootElementUpdated(object sender, EventArgs e) { UpdateChildren(); }
		void OnRootElementTargetChanged(object sender, EventArgs e) {
			UnsubscribeTarget();
			targetCore = rootElementCore.TargetElement as ISupportAdornerElementBarItem;
			SubscribeTarget();
			RefreshChildren();
		}
		public void RegisterChildren() {
			if(childrenCore == null) return;
			foreach(var child in Children)
				managerCore.RegisterElement(child);
		}
		#region IDisposable Members
		bool isDisposing;
		public void Dispose() {
			if(isDisposing) return;
			isDisposing = true;
			OnDispose();
		}
		protected virtual void OnDispose() {
			rootElementCore.TargetChanged -= OnRootElementTargetChanged;
			rootElementCore.Updated -= OnRootElementUpdated;
			UnsubscribeTarget();
			DestroyChildren();
			rootElementCore = null;
			targetCore = null;
			managerCore = null;
		}
		#endregion
	}
}
