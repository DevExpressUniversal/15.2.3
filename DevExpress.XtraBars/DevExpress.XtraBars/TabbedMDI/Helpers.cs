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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils.Win.Hook;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraTab;
using DevExpress.XtraTab.Registrator;
using DevExpress.XtraTab.ViewInfo;
namespace DevExpress.XtraTabbedMdi {
	public enum CloseTabOnMiddleClick {
		Default,
		OnMouseDown,
		OnMouseUp,
		Never
	}
	static class PageAssociation {
		static Dictionary<Form, XtraMdiTabPage> hash = new Dictionary<Form, XtraMdiTabPage>();
		public static void Add(Form form, XtraMdiTabPage page) {
			if(form == null || page == null) return;
			if(hash.ContainsKey(form))
				hash[form] = page;
			else hash.Add(form, page);
		}
		public static void Remove(Form form) {
			if(form != null) hash.Remove(form);
		}
		public static bool TryGetValue(Form form, out XtraMdiTabPage value) {
			return hash.TryGetValue(form, out value);
		}
	}
	public class FloatFormCollection : ICollection<Form>, IHookController, IDisposable {
		class Locker : IDisposable {
			FloatFormCollection Target;
			public Locker(FloatFormCollection collection) {
				Target = collection;
				Target.lockCounter++;
			}
			public void Dispose() {
				if(Target != null) {
					--Target.lockCounter;
					Target = null;
				}
			}
		}
		XtraTabbedMdiManager Manager;
		List<Form> List;
		public FloatFormCollection(XtraTabbedMdiManager manager) {
			List = new List<Form>();
			Manager = manager;
		}
		public void Dispose() {
			Form[] forms = new Form[Count];
			List.CopyTo(forms, 0);
			for(int i = 0; i < forms.Length; i++) {
				DisposeFloatPage(forms[i]);
			}
			GC.SuppressFinalize(this);
		}
		public Form this[int i] {
			get { return List[i]; }
		}
		int lockCounter = 0;
		public IDisposable Lock() {
			return new Locker(this);
		}
		public bool IsLocked {
			get { return lockCounter > 0; }
		}
		public void Add(Form form) {
			if(!List.Contains(form)) {
				List.Add(form);
				SubscribeFloatFormEvents(form);
				if(!Manager.IsDragFullWindows)
					HookMouse();
			}
		}
		public bool Remove(Form form) {
			if(List.Remove(form)) {
				UnSubscribeFloatFormEvents(form);
				if(Count == 0)
					UnHookMouse();
				return true;
			}
			return false;
		}
		public void Clear() {
			Form[] forms = List.ToArray();
			List.Clear();
			for(int i = 0; i < forms.Length; i++) {
				UnSubscribeFloatFormEvents(forms[i]);
			}
			UnHookMouse();
		}
		void SubscribeFloatFormEvents(Form floatForm) {
			if(floatForm != null) {
				floatForm.Activated += OnFloatMDIChildActivated;
				floatForm.Deactivate += OnFloatMDIChildDeactivated;
				floatForm.LocationChanged += OnFloatMdiChildLocationChanged;
				floatForm.FormClosed += OnFloatMdiChildClosed;
				floatForm.Disposed += OnFloatMdiChildDisposed;
			}
		}
		void UnSubscribeFloatFormEvents(Form floatForm) {
			if(floatForm != null) {
				floatForm.Activated -= OnFloatMDIChildActivated;
				floatForm.Deactivate -= OnFloatMDIChildDeactivated;
				floatForm.LocationChanged -= OnFloatMdiChildLocationChanged;
				floatForm.FormClosed -= OnFloatMdiChildClosed;
				floatForm.Disposed -= OnFloatMdiChildDisposed;
			}
		}
		void UnHookMouse() {
			HookManager.DefaultManager.RemoveController(this);
		}
		void HookMouse() {
			HookManager.DefaultManager.AddController(this);
		}
		void OnFloatMDIChildActivated(object sender, EventArgs e) {
			Manager.OnFloatFormActivated(sender as Form);
		}
		void OnFloatMDIChildDeactivated(object sender, EventArgs e) {
			Manager.OnFloatFormDeactivated(sender as Form);
		}
		void OnFloatMdiChildDisposed(object sender, EventArgs e) {
			DisposeFloatPage(sender as Form);
		}
		void OnFloatMdiChildClosed(object sender, FormClosedEventArgs e) {
			DisposeFloatPage(sender as Form);
		}
		void DisposeFloatPage(Form floatForm) {
			UnSubscribeFloatFormEvents(floatForm);
			XtraMdiTabPage page;
			if(PageAssociation.TryGetValue(floatForm, out page))
				((IDisposable)page).Dispose();
			List.Remove(floatForm);
			if(List.Count == 0)
				UnHookMouse();
		}
		void OnFloatMdiChildLocationChanged(object sender, EventArgs e) {
			if(IsLocked) return;
			Form form = sender as Form;
			Manager.OnFloatFormLocationChanged(form);
			if(!Manager.IsFloatingDragging && !form.Capture) {
				Manager.CancelFloating(false);
			}
		}
		public IEnumerator<Form> GetEnumerator() {
			return List.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		public bool Contains(Form item) {
			return List.Contains(item);
		}
		public void CopyTo(Form[] array, int arrayIndex) {
			List.CopyTo(array, arrayIndex);
		}
		public int Count { get { return List.Count; } }
		public bool IsReadOnly { get { return false; } }
		#region IHookController Members
		bool IHookController.InternalPostFilterMessage(int Msg, Control wnd, IntPtr HWnd, IntPtr WParam, IntPtr LParam) {
			return false;
		}
		bool IHookController.InternalPreFilterMessage(int Msg, Control wnd, IntPtr HWnd, IntPtr WParam, IntPtr LParam) {
			Form floatForm = GetForm(HWnd);
			if(floatForm != null)
				ProcessWindowDragging(Msg, floatForm);
			return false;
		}
		IntPtr IHookController.OwnerHandle {
			get { return (dragForm != null) ? dragForm.Handle : IntPtr.Zero; }
		}
		#endregion
		Form dragForm;
		void ProcessWindowDragging(int Msg, Form floatForm) {
			if(dragForm == null) {
				if(Msg == MSG.WM_NCLBUTTONDOWN)
					BeginDragging(floatForm);
			}
			else {
				if(dragForm == floatForm) {
					switch(Msg) {
						case MSG.WM_MOUSEMOVE:
							Dragging(dragForm);
							break;
						case MSG.WM_LBUTTONUP:
							EndDragging(dragForm);
							break;
					}
				}
			}
		}
		void BeginDragging(Form floatForm) {
			dragForm = floatForm;
		}
		void Dragging(Form floatForm) {
			Manager.OnFloatFormLocationChanged(floatForm);
		}
		void EndDragging(Form floatForm) {
			dragForm = null;
		}
		int GetInt(IntPtr ptr) {
			return IntPtr.Size == 8 ? (int)ptr.ToInt64() : ptr.ToInt32();
		}
		Form GetForm(IntPtr hWnd) {
			foreach(Form floatForm in List) {
				if(floatForm.Handle == hWnd)
					return floatForm;
			}
			return null;
		}
	}
	public delegate void FloatingEventHandler(object sender, FloatingEventArgs e);
	public delegate void FloatingCancelEventHandler(object sender, FloatingCancelEventArgs e);
	public delegate void FloatMDIChildDraggingEventHandler(object sender, FloatMDIChildDraggingEventArgs e);
	public class FloatingEventArgs : EventArgs {
		Form childFormCore;
		Form parentFormCore;
		public FloatingEventArgs(Form child, Form parent) {
			childFormCore = child;
			parentFormCore = parent;
		}
		public Form ParentForm {
			get { return parentFormCore; }
		}
		public Form ChildForm {
			get { return childFormCore; }
		}
	}
	public class EndFloatingEventArgs : FloatingEventArgs {
		bool isDockingCore;
		public EndFloatingEventArgs(Form child, Form parent, bool isDocking)
			: base(child, parent) {
			isDockingCore = isDocking;
		}
		public bool IsDocking {
			get { return isDockingCore; }
		}
	}
	public class FloatingCancelEventArgs : CancelEventArgs {
		Form childFormCore;
		Form parentFormCore;
		public FloatingCancelEventArgs(Form child, Form parent)
			: this(child, parent, false) {
		}
		public FloatingCancelEventArgs(Form child, Form parent, bool cancel)
			: base(cancel) {
			childFormCore = child;
			parentFormCore = parent;
		}
		public Form ParentForm {
			get { return parentFormCore; }
		}
		public Form ChildForm {
			get { return childFormCore; }
		}
	}
	public class FloatMDIChildDraggingEventArgs : FloatingEventArgs {
		Point screenPointCore;
		IList<XtraTabbedMdiManager> targetsCore;
		public Point ScreenPoint {
			get { return screenPointCore; }
		}
		public IList<XtraTabbedMdiManager> Targets {
			get { return targetsCore; }
		}
		public FloatMDIChildDraggingEventArgs(Form child, Form parent, Point screenPoint)
			: base(child, parent) {
			targetsCore = new List<XtraTabbedMdiManager>();
			screenPointCore = screenPoint;
		}
	}
	internal class XtraTabbedMdiCategoryName {
		public const string Appearance = "Appearance";
		public const string Behavior = "Behavior";
		public const string Events = "Events";
	}
	public class MdiTabPageEventArgs : EventArgs {
		XtraMdiTabPage page;
		public MdiTabPageEventArgs(XtraMdiTabPage page) {
			this.page = page;
		}
		public XtraMdiTabPage Page { get { return this.page; } }
	}
	public class MdiTabPageCancelEventArgs : CancelEventArgs {
		XtraMdiTabPage page;
		public MdiTabPageCancelEventArgs(XtraMdiTabPage page) {
			this.page = page;
		}
		public XtraMdiTabPage Page { get { return this.page; } }
	}
	public delegate void MdiTabPageEventHandler(object sender, MdiTabPageEventArgs e);
	public delegate void MdiTabPageCancelEventHandler(object sender, MdiTabPageCancelEventArgs e);
	public class SetNextMdiChildEventArgs : EventArgs {
		bool handled;
		Message m;
		public SetNextMdiChildEventArgs(Message m) {
			this.m = m;
			this.handled = false;
		}
		public bool Handled {
			get { return this.handled; }
			set { this.handled = value; }
		}
		protected Message Message { get { return m; } }
		public bool ForwardNavigation { get { return Message.LParam == IntPtr.Zero; } }
	}
	public delegate void SetNextMdiChildEventHandler(object sender, SetNextMdiChildEventArgs e);
	public enum SetNextMdiChildMode { Default, Windows, TabControl }
	public class XtraTabbedMdiDataObject : IDataObject {
		WeakReference wRefPage;
		public XtraTabbedMdiDataObject(IXtraTabPage page) {
			wRefPage = new WeakReference(page);
		}
		public virtual object GetData(string format) {
			return GetData(format, true);
		}
		public virtual object GetData(Type format) {
			if(format == null)
				return null;
			return GetData(format.FullName);
		}
		public virtual object GetData(string format, bool autoConvert) {
			Type type = Type.GetType(format);
			if(type == null) {
				if(format == typeof(IXtraTabPage).FullName)
					type = typeof(IXtraTabPage);
				else return null;
			}
			IXtraTabPage page = wRefPage.Target as IXtraTabPage;
			if((page != null) && type.IsAssignableFrom(page.GetType()))
				return page;
			return null;
		}
		public virtual bool GetDataPresent(string format) {
			return GetData(format) != null;
		}
		public virtual bool GetDataPresent(Type format) {
			return GetData(format) != null;
		}
		public virtual bool GetDataPresent(string format, bool autoConvert) {
			return GetData(format, autoConvert) != null;
		}
		public virtual string[] GetFormats() {
			return this.GetFormats(true);
		}
		public virtual string[] GetFormats(bool autoConvert) {
			return new string[] { typeof(IXtraTabPage).FullName };
		}
		public virtual void SetData(string format, bool autoConvert, object data) {
			throw new NotImplementedException();
		}
		public virtual void SetData(string format, object data) {
			this.SetData(format, true, data);
		}
		public virtual void SetData(object data) {
			this.SetData(data.GetType(), data);
		}
		public virtual void SetData(Type format, object data) {
			this.SetData(format.FullName, data);
		}
	}
	public class DragDropDrawTabPage : IXtraTabPage, IXtraTabPageExt {
		readonly IXtraTab owner;
		readonly IXtraTabPage prototype;
		readonly IXtraTabPageExt prototypeExt;
		public DragDropDrawTabPage(IXtraTab owner, IXtraTabPage prototype) {
			this.owner = owner;
			this.prototype = prototype;
			this.prototypeExt = prototype as IXtraTabPageExt;
		}
		int IXtraTabPage.TabPageWidth { get { return 0; } }
		IXtraTab IXtraTabPage.TabControl { get { return owner; } }
		void IXtraTabPage.Invalidate() {  }
		Image IXtraTabPage.Image { get { return prototype.Image; } }
		int IXtraTabPage.ImageIndex { get { return prototype.ImageIndex; } }
		string IXtraTabPage.Text { get { return prototype.Text; } }
		string IXtraTabPage.Tooltip { get { return prototype.Tooltip; } }
		string IXtraTabPage.TooltipTitle { get { return prototype.TooltipTitle; } }
		ToolTipIconType IXtraTabPage.TooltipIconType { get { return prototype.TooltipIconType; } }
		bool IXtraTabPage.PageEnabled { get { return prototype.PageEnabled; } }
		bool IXtraTabPage.PageVisible { get { return prototype.PageVisible; } }
		SuperToolTip IXtraTabPage.SuperTip { get { return prototype.SuperTip; } }
		PageAppearance IXtraTabPage.Appearance { get { return prototype.Appearance; } }
		DefaultBoolean IXtraTabPage.ShowCloseButton { get { return prototype.ShowCloseButton; } }
		DefaultBoolean IXtraTabPage.AllowGlyphSkinning { get { return prototype.AllowGlyphSkinning; } }
		Padding IXtraTabPage.ImagePadding { get { return prototype.ImagePadding; } }
		System.Drawing.Text.HotkeyPrefix IXtraTabPageExt.HotkeyPrefixOverride { get { return prototypeExt != null ? prototypeExt.HotkeyPrefixOverride : System.Drawing.Text.HotkeyPrefix.None; } }
		int IXtraTabPageExt.MaxTabPageWidth { get { return prototypeExt != null ? prototypeExt.MaxTabPageWidth : 0; } }
		bool IXtraTabPageExt.Pinned {
			get { return prototypeExt != null ? prototypeExt.Pinned : false; }
			set { prototypeExt.Pinned = value; }
		}
		DefaultBoolean IXtraTabPageExt.ShowPinButton { get { return prototypeExt != null ? prototypeExt.ShowPinButton : DefaultBoolean.Default; } }
		bool IXtraTabPageExt.UsePinnedTab { get { return prototypeExt != null ? prototypeExt.UsePinnedTab : false; } }
	}
	public class DragDropDrawTabControl : IXtraTab, IXtraTabProperties {
		readonly int oldPosition;
		int currentPosition;
		readonly IXtraTab originalControl;
		readonly BaseViewInfoRegistrator view;
		readonly BaseTabControlViewInfo viewInfo;
		readonly IList pages;
		public DragDropDrawTabControl(IXtraTab originalControl, int oldPosition, int currentPosition) {
			this.oldPosition = oldPosition;
			this.currentPosition = currentPosition;
			this.originalControl = originalControl;
			this.view = originalControl.View;
			this.viewInfo = this.View.CreateViewInfo(this);
			this.pages = new ArrayList();
			for(int i = 0; i < originalControl.PageCount; ++i) {
				if(i != oldPosition) {
					this.pages.Add(new DragDropDrawTabPage(this, originalControl.GetTabPage(i)));
				}
			}
			IXtraTabPage currentPage = new DragDropDrawTabPage(this, originalControl.GetTabPage(oldPosition));
			this.pages.Insert(currentPosition, currentPage);
			ViewInfo.Resize();
			ViewInfo.FirstVisiblePageIndex = originalControl.ViewInfo.FirstVisiblePageIndex;
			ViewInfo.SetSelectedTabPageCore(currentPage);
			ViewInfo.FillPageClient = originalControl.ViewInfo.FillPageClient;
			ViewInfo.LayoutChanged();
		}
		public bool UpdatePositions(int oldPosition, int newPosition) {
			if(this.oldPosition != oldPosition)
				throw new InvalidOperationException();
			if(this.currentPosition == newPosition)
				return false;
			IXtraTabPage currentPage = (IXtraTabPage)this.pages[this.currentPosition];
			this.pages.RemoveAt(this.currentPosition);
			this.currentPosition = newPosition;
			this.pages.Insert(this.currentPosition, currentPage);
			ViewInfo.SetSelectedTabPageCore(currentPage);
			ViewInfo.LayoutChanged();
			return true;
		}
		bool IXtraTab.RightToLeftLayout { get { return false; } }
		public IXtraTab Owner { get { return originalControl; } }
		public IXtraTabProperties OwnerProperties {
			get {
				if(Owner is IXtraTabProperties)
					return (IXtraTabProperties)Owner;
				return DefaultTabProperties.Default;
			}
		}
		public virtual int PageCount { get { return this.pages.Count; } }
		public virtual IXtraTabPage GetTabPage(int index) {
			return (IXtraTabPage)this.pages[index];
		}
		public virtual BaseViewInfoRegistrator View { get { return this.view; } }
		Rectangle IXtraTab.Bounds { get { return Owner.Bounds; } }
		TabHeaderLocation IXtraTab.HeaderLocation { get { return Owner.HeaderLocation; } }
		TabOrientation IXtraTab.HeaderOrientation { get { return Owner.HeaderOrientation; } }
		object IXtraTab.Images { get { return Owner.Images; } }
		BaseTabHitInfo IXtraTab.CreateHitInfo() {
			throw new NotImplementedException();
		}
		public virtual BaseTabControlViewInfo ViewInfo { get { return this.viewInfo; } }
		Control IXtraTab.OwnerControl { get { return Owner.OwnerControl; } }
		UserLookAndFeel IXtraTab.LookAndFeel { get { return Owner.LookAndFeel; } }
		void IXtraTab.OnPageChanged(IXtraTabPage page) { throw new NotImplementedException(); }
		void IXtraTab.LayoutChanged() { throw new NotImplementedException(); }
		void IXtraTab.Invalidate(Rectangle rect) { Owner.Invalidate(rect); }
		Point IXtraTab.ScreenPointToControl(Point point) { throw new NotImplementedException(); }
		int IXtraTabProperties.TabPageWidth { get { return OwnerProperties.TabPageWidth; } }
		DefaultBoolean IXtraTabProperties.AllowHotTrack { get { return OwnerProperties.AllowHotTrack; } }
		DefaultBoolean IXtraTabProperties.ShowTabHeader { get { return OwnerProperties.ShowTabHeader; } }
		DefaultBoolean IXtraTabProperties.ShowToolTips { get { return OwnerProperties.ShowToolTips; } }
		DefaultBoolean IXtraTabProperties.MultiLine { get { return OwnerProperties.MultiLine; } }
		DefaultBoolean IXtraTabProperties.HeaderAutoFill { get { return OwnerProperties.HeaderAutoFill; } }
		DefaultBoolean IXtraTabProperties.ShowHeaderFocus { get { return OwnerProperties.ShowHeaderFocus; } }
		TabPageImagePosition IXtraTabProperties.PageImagePosition { get { return OwnerProperties.PageImagePosition; } }
		AppearanceObject IXtraTabProperties.Appearance { get { return OwnerProperties.Appearance; } }
		PageAppearance IXtraTabProperties.AppearancePage { get { return OwnerProperties.AppearancePage; } }
		BorderStyles IXtraTabProperties.BorderStyle { get { return OwnerProperties.BorderStyle; } }
		BorderStyles IXtraTabProperties.BorderStylePage { get { return OwnerProperties.BorderStylePage; } }
		TabButtonShowMode IXtraTabProperties.HeaderButtonsShowMode { get { return OwnerProperties.HeaderButtonsShowMode; } }
		TabButtons IXtraTabProperties.HeaderButtons { get { return OwnerProperties.HeaderButtons; } }
		ClosePageButtonShowMode IXtraTabProperties.ClosePageButtonShowMode { get { return OwnerProperties.ClosePageButtonShowMode; } }
		PinPageButtonShowMode IXtraTabProperties.PinPageButtonShowMode { get { return OwnerProperties.PinPageButtonShowMode; } }
		DefaultBoolean IXtraTabProperties.AllowGlyphSkinning { get { return OwnerProperties.AllowGlyphSkinning; } }
	}
}
