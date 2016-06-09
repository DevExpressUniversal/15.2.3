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
using System.ComponentModel.Design;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.Design.Behavior;
using DevExpress.LookAndFeel.DesignService;
using DevExpress.Utils.About;
using DevExpress.Utils.Drawing;
namespace DevExpress.Utils.Design {
#if DXWhidbey
	public class DebugInfoDesigner {
		internal static string noModifyDebug = "The file {0} cannot be modified in the designer while building or debugging.";
		internal static void ShowErrorMessage(ComponentDesigner designer) {
			IServiceProvider provider = GetServiceProvider(designer);
			if (provider == null) return;
			object loader = provider.GetService(typeof(IResourceService));
			if (loader == null) return;
			if (loader.GetType().Name == "VSCodeDomDesignerLoader") {
				System.Reflection.FieldInfo fi = loader.GetType().GetField("_docData", BindingFlags.Instance | BindingFlags.NonPublic);
				if (fi != null) {
					System.Reflection.PropertyInfo pi = fi.GetValue(loader).GetType().GetProperty("Name", typeof(string));
					if (pi != null) {
						object[] objArray1 = new object[] { pi.GetValue(fi.GetValue(loader), null) };
						MessageBox.Show(string.Format(noModifyDebug, objArray1), designer.ToString());
					}
				}
			}
		}
		internal static Component GetComponent(object obj) {
			Component component = obj as Component;
			if (component != null) return component;
			ComponentDesigner designer = obj as ComponentDesigner;
			if (designer != null) return designer.Component as Component;
			return null;
		}
		internal static IServiceProvider GetServiceProvider(object obj) {
			IServiceProvider provider = obj as IServiceProvider;
			if (provider != null) return provider;
			Component component = obj as Component;
			if(component != null) return component.Site;
			ComponentDesigner designer = obj as ComponentDesigner;
			if (designer != null && designer.Component != null) return designer.Component.Site;
			return null;
		}
		static System.Reflection.PropertyInfo pInfoIsDebugging;
		internal static bool IsDebugging(object obj) {
			IServiceProvider provider = GetServiceProvider(obj);
			if(provider == null) return false;
			object loader = provider.GetService(typeof(IResourceService));
			if (loader == null) return false;
			if (loader.GetType().Name == "VSCodeDomDesignerLoader") {
				if(pInfoIsDebugging == null)
					pInfoIsDebugging = loader.GetType().GetProperty("IsDebugging", BindingFlags.Instance | BindingFlags.NonPublic);
				if(pInfoIsDebugging != null) 
					return (bool)pInfoIsDebugging.GetValue(loader, null);
			}
			return false;
		}
		internal static void OnDebuggingStateChanged(bool state, object obj) {
			IServiceProvider provider = GetServiceProvider(obj);
			Component component = GetComponent(obj);
			if (provider == null || component == null) return;
			ISelectionService srv = provider.GetService(typeof(ISelectionService)) as ISelectionService;
			if (srv != null) {
				if (srv.GetComponentSelected(component)) {
					srv.SetSelectedComponents(null);
					srv.SetSelectedComponents(new object[] { component }, SelectionTypes.Replace);
				}
			}
		}
	}
#endif
	public class BaseComponentDesigner : BaseComponentDesignerSimple, IUndoEngine {
		bool debuggingState = false;
		int stateChanging = 0;
		static BaseComponentDesigner() {
			DXAssemblyResolverEx.Init(); 
		}
		#region IUndoEngine Members
		UndoEngine GetEngine() { return GetService(typeof(UndoEngine)) as UndoEngine; }
		bool IUndoEngine.Enabled {
			get { return GetEngine() != null ? GetEngine().Enabled : true; }
			set { if(GetEngine() != null) GetEngine().Enabled = value; }
		}
		bool IUndoEngine.UndoInProgress { get { return GetEngine() != null ? GetEngine().UndoInProgress : false; } }
		#endregion
		protected virtual bool DebuggingState {
			get {
				if(debuggingState && AllowHookDebugMode) return debuggingState;
				debuggingState = DebugInfoDesigner.IsDebugging(this);
				return debuggingState;
			}
			set {
				if(debuggingState == value) return;
				debuggingState = value;
#if DXWhidbey
				this.stateChanging++;
				try {
					OnDebuggingStateChanged();
				} finally {
					this.stateChanging--;
				}
#endif
			}
		}
		protected bool IsDebuggingStateChanging { get { return stateChanging != 0; } }
		protected virtual bool AllowHookDebugMode { get { return false; } }
#if DXWhidbey
		protected virtual void OnDebuggingStateChanged() {
			DebugInfoDesigner.OnDebuggingStateChanged(DebuggingState, this);
		}
		void Application_Idle(object sender, EventArgs e) {
			DebuggingState = DebugInfoDesigner.IsDebugging(this);
		}
#endif
		public override DesignerVerbCollection Verbs {
			get {
#if DXWhidbey
				if (DebuggingState) return null;
#endif
				return DXVerbs;
			}
		}
#if DXWhidbey
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			OnInitializeNew(defaultValues);
		}
#else
		public override void OnSetComponentDefaults() {
			base.OnSetComponentDefaults();
			OnInitializeNew(null);
		}
#endif
		protected virtual void OnInitializeNew(IDictionary defaultValues) { }
		DesignerVerbCollection fVerbs = new DesignerVerbCollection();
		public virtual DesignerVerbCollection DXVerbs { get { return fVerbs; } }
		protected virtual bool AllowInheritanceWrapper { get { return false; } }
		Hashtable dxInheritedProps = null;
		protected override void PostFilterProperties(IDictionary properties) {
			if(AllowInheritanceWrapper) {
				if(this.dxInheritedProps == null) this.dxInheritedProps = InitializeNestedInheritedProperties(Component, InheritanceAttribute, properties);
				DesignerHelper.FilterOutBaseIherited(this, this.dxInheritedProps);
			}
			base.PostFilterProperties(properties);
			if(AllowInheritanceWrapper) DesignerHelper.PostFilterProperties(dxInheritedProps, InheritanceAttribute, properties);
		}
		public override void Initialize(IComponent component) {
			((DevExpress.LookAndFeel.Design.UserLookAndFeelDefault)DevExpress.LookAndFeel.UserLookAndFeel.Default).UpdateDesignTimeLookAndFeelEx(component);
			base.Initialize(component);
			if(component != null) {
				LookAndFeelServiceHelper.AddVsLookAndFeelService(component.Site);
			}
#if DXWhidbey
			if(AllowInheritanceWrapper) this.dxInheritedProps = InitializeNestedInheritedProperties(component, InheritanceAttribute, null);
			if(AllowHookDebugMode) Application.Idle += new EventHandler(Application_Idle);
#endif
		}
		protected virtual Hashtable InitializeNestedInheritedProperties(object component, InheritanceAttribute attribute, IDictionary properties) {
			return DesignerHelper.InitializeNestedInheritedProperties(component, attribute, properties);
		}
		protected override void Dispose(bool disposing) {
#if DXWhidbey
			if(AllowHookDebugMode) Application.Idle -= new EventHandler(Application_Idle);
#endif
			base.Dispose(disposing);
		}
		protected virtual bool AllowEditInherited { get { return true; } }
		protected virtual bool AllowDesigner {
			get {
#if DXWhidbey
				return AllowEditInherited || InheritanceAttribute != InheritanceAttribute.InheritedReadOnly;
#else
				return true;
#endif
			}
		}
#if DXWhidbey
		protected override InheritanceAttribute InheritanceAttribute {
			get {
				if(!InheritanceHelper.DisableVisualInheritance)
					return base.InheritanceAttribute;
				if(AllowEditInherited) return base.InheritanceAttribute;
				if(base.InheritanceAttribute != InheritanceAttribute.Inherited && base.InheritanceAttribute != InheritanceAttribute.InheritedReadOnly) {
					return base.InheritanceAttribute;
				}
				return InheritanceAttribute.InheritedReadOnly;
			}
		}
		public override DesignerActionListCollection ActionLists {
#else
		public virtual DesignerActionListCollection ActionLists {
#endif
			get {
#if DXWhidbey
				if (DebuggingState) return null;			  
#endif
				return base.ActionLists;
			}
		}
	}
	static class LookAndFeelServiceHelper {
		public static void AddVsLookAndFeelService(IServiceProvider serviceProvider) {
			IDesignerHost designerHost = serviceProvider != null ? (IDesignerHost)serviceProvider.GetService(typeof(IDesignerHost)) : null;
			if(designerHost != null && designerHost.GetService(typeof(ILookAndFeelService)) == null) {
				var lookAndFeelService = new VSLookAndFeelService(designerHost);
				designerHost.AddService(typeof(ILookAndFeelService), lookAndFeelService);
			}
		}
	}
	public class BaseParentControlDesigner : BaseParentControlDesignerSimple, IUndoEngine {
		bool debuggingState = false;
		#region IUndoEngine Members
		UndoEngine GetEngine() { return GetService(typeof(UndoEngine)) as UndoEngine; }
		bool IUndoEngine.Enabled {
			get { return GetEngine() != null ? GetEngine().Enabled : true; }
			set { if(GetEngine() != null) GetEngine().Enabled = value; }
		}
		bool IUndoEngine.UndoInProgress { get { return GetEngine() != null ? GetEngine().UndoInProgress : false; } }
		#endregion
		protected virtual bool DebuggingState {
		  get {
				if(debuggingState && AllowHookDebugMode) return debuggingState;
				debuggingState = DebugInfoDesigner.IsDebugging(this);
				return debuggingState;
			}	set {
				if(debuggingState == value) return;
				debuggingState = value;
#if DXWhidbey
				OnDebuggingStateChanged();
#endif
			}
		}
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
		}
		protected override void OnPaintAdornments(PaintEventArgs pe) {
			base.OnPaintAdornments(pe);
			BaseControlDesigner.CheckDrawExpired(pe, this);
		}
		protected virtual bool AllowHookDebugMode { get { return false; } }
#if DXWhidbey
		protected virtual void OnDebuggingStateChanged() {
			DebugInfoDesigner.OnDebuggingStateChanged(DebuggingState, this);
		}
		void Application_Idle(object sender, EventArgs e) {
			DebuggingState = DebugInfoDesigner.IsDebugging(this);
		}
#endif
		DesignerActionListCollection actionLists;
		protected virtual bool AllowEditInherited { get { return true; } }
		protected virtual bool UseVerbsAsActionList { get { return false; } }
		protected virtual bool AllowDesigner {
			get {
#if DXWhidbey
				return AllowEditInherited || InheritanceAttribute != InheritanceAttribute.InheritedReadOnly;
#else
				return true;
#endif
			}
		}
#if DXWhidbey
		protected override InheritanceAttribute InheritanceAttribute {
			get {
				if(!InheritanceHelper.DisableVisualInheritance)
					return base.InheritanceAttribute;
				if(AllowEditInherited) return base.InheritanceAttribute;
				if(base.InheritanceAttribute != InheritanceAttribute.Inherited && base.InheritanceAttribute != InheritanceAttribute.InheritedReadOnly) {
					return base.InheritanceAttribute;
				}
				return InheritanceAttribute.InheritedReadOnly;
			}
		}
		protected virtual void ResetActionLists() {
			this.actionLists = null;
		}
		protected virtual bool AlwaysCreateActionLists { get { return false; } }
		public override DesignerActionListCollection ActionLists {
#else
		public virtual DesignerActionListCollection ActionLists {
#endif
			get {
#if DXWhidbey
				if (DebuggingState) return null;
				if (UseVerbsAsActionList) return base.ActionLists;
#endif
				if(actionLists == null || AlwaysCreateActionLists) actionLists = CreateActionLists();
				return actionLists;
			}
		}
		public override DesignerVerbCollection Verbs {
			get {
#if DXWhidbey
				if (DebuggingState) return null;
#endif
				return DXVerbs;
			}
		}
		DesignerVerbCollection fVerbs = new DesignerVerbCollection();
		public virtual DesignerVerbCollection DXVerbs { get { return fVerbs; } }
		protected virtual bool AllowInheritanceWrapper { get { return false; } }
		Hashtable dxInheritedProps = null;
		protected override void PostFilterProperties(IDictionary properties) {
			if(AllowInheritanceWrapper) {
				if(this.dxInheritedProps == null) this.dxInheritedProps = DesignerHelper.InitializeNestedInheritedProperties(Component, InheritanceAttribute, properties);
				DesignerHelper.FilterOutBaseIherited(this, this.dxInheritedProps);
			}
			base.PostFilterProperties(properties);
			if(AllowInheritanceWrapper) DesignerHelper.PostFilterProperties(dxInheritedProps, InheritanceAttribute, properties);
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			if(DXStackHelper.IsCalledFromDataWindow) OnInitilizeDataWindowDrop(component);
#if DXWhidbey
			if(AllowInheritanceWrapper) this.dxInheritedProps = DesignerHelper.InitializeNestedInheritedProperties(component, InheritanceAttribute, null);
			if(AllowHookDebugMode) Application.Idle += new EventHandler(Application_Idle);
#endif
		}
		protected override void Dispose(bool disposing) {
#if DXWhidbey
			if(AllowHookDebugMode) Application.Idle -= new EventHandler(Application_Idle);
#endif
			base.Dispose(disposing);
		}
		protected virtual void OnInitilizeDataWindowDrop(IComponent component) { }
		protected virtual DesignerActionListCollection CreateActionLists() {
			DesignerActionListCollection list = new DesignerActionListCollection();
			RegisterActionLists(list);
			return list;
		}
		protected virtual void RegisterActionLists(DesignerActionListCollection list) {
			RegisterAboutAction(list);
		}
		protected virtual void RegisterAboutAction(DesignerActionListCollection list) {
			DXAboutActionList about = GetAboutAction();
			if(about != null) {
				list.Insert(list.Count, about);
			}
			DXSmartTagsHelper.CreateDefaultLinks(this, list);
		}
		protected virtual DXAboutActionList GetAboutAction() { return null; }
		bool IsMouseMessage(Message m) {
			return m.Msg == WM_MOUSEMOVE || m.Msg == WM_LBUTTONDOWN || m.Msg == WM_SETCURSOR;
		}
		const int WM_MOUSEMOVE = 0x200, MK_LBUTTON = 0x0001, MK_RBUTTON = 0x0002, WM_SETCURSOR = 0x0020, WM_LBUTTONDOWN = 0x0201;
		protected override void OnMouseEnter() {
			base.OnMouseEnter();
			MethodInfo mi = GetMethodInfo("OnMouseEnter");
			if (mi != null) mi.Invoke(Control, new object[] { EventArgs.Empty });
		}
		protected override void OnMouseLeave() {
			base.OnMouseLeave();
			MethodInfo mi = GetMethodInfo("OnMouseLeave");
			if (mi != null) mi.Invoke(Control, new object[] { EventArgs.Empty });
		}
		protected MethodInfo GetMethodInfo(string name) {
			return typeof(Control).GetMethod(name, BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.Instance);
		}	  
		protected override void WndProc(ref Message m) {
			Point p = Point.Empty;
			if(!AllowDesigner || Control == null || DebuggingState) {
				base.WndProc(ref m);
				return;
			}
			if (IsMouseMessage(m)) p = new Point(m.LParam.ToInt32());
			Control ctrl = Control.FromHandle(m.HWnd);
			bool forceDefProcOnly = false;
			if (m.HWnd == Control.Handle) {
				if (m.Msg == WM_LBUTTONDOWN) { 
					if(GetHitTest(Control.PointToScreen(p))) forceDefProcOnly = true;
				}
				if (m.Msg == WM_MOUSEMOVE) {
					int btns = m.WParam.ToInt32();
					MouseButtons buttons = MouseButtons.None;
					if ((btns & MK_LBUTTON) != 0) buttons |= MouseButtons.Left;
					if ((btns & MK_RBUTTON) != 0) buttons |= MouseButtons.Right;
					MethodInfo mi = GetMethodInfo("OnMouseMove");
					if (mi != null) mi.Invoke(Control, new object[] { new MouseEventArgs(buttons, 0, p.X, p.Y, 0) });
					if(GetHitTest(Control.PointToScreen(p))) forceDefProcOnly = true;
				}
			}
			if (forceDefProcOnly) {
#if DXWhidbey
				DefWndProc(ref m);
				return;
#endif
			}
			base.WndProc(ref m);
		}
	}
	public interface IDebuggingStateProvider {
		bool IsDebuggingState { get; }
	}
	public static class ControlMouseHelper {
		public static void InvokeMouseEnter(Control control) {
			if(control == null) return;
			MethodInfo mi = GetMethodInfo("OnMouseEnter");
			if(mi != null) mi.Invoke(control, new object[] { EventArgs.Empty });
		}
		public static void InvokeMouseLeave(Control control) {
			if(control == null) return;
			MethodInfo mi = GetMethodInfo("OnMouseLeave");
			if(mi != null) mi.Invoke(control, new object[] { EventArgs.Empty });
		}
		public static void InvokeMouseMove(Control control, MouseEventArgs e) {
			if(control == null) return;
			MethodInfo mi = GetMethodInfo("OnMouseMove");
			if(mi != null) mi.Invoke(control, new object[] { e });
		}
		public static void InvokeMouseDown(Control control, MouseEventArgs e) {
			if(control == null) return;
			MethodInfo mi = GetMethodInfo("OnMouseDown");
			if(mi != null) mi.Invoke(control, new object[] { e });
		}
		public static void InvokeMouseUp(Control control, MouseEventArgs e) {
			if(control == null) return;
			MethodInfo mi = GetMethodInfo("OnMouseUp");
			if(mi != null) mi.Invoke(control, new object[] { e });
		}
		static MethodInfo GetMethodInfo(string name) {
			return typeof(Control).GetMethod(name, BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.Instance);
		}
	}
	public class BaseControlDesigner : BaseControlDesignerSimple, IDebuggingStateProvider, IUndoEngine {
		static BaseControlDesigner() {
			DXAssemblyResolverEx.Init();			 
		}
		protected override void OnPaintAdornments(PaintEventArgs pe) {
			base.OnPaintAdornments(pe);
			CheckDrawExpired(pe, this);
		}
		public static bool IsExpired(ControlDesigner designer) {
			return false;
		}
		public static void CheckDrawExpired(PaintEventArgs e, ControlDesigner designer) {
		}
		static bool CheckAttributes(IComponent component) {
			return false;
		}
		#region IUndoEngine Members
		UndoEngine GetEngine() { return GetService(typeof(UndoEngine)) as UndoEngine; }
		bool IUndoEngine.Enabled {
			get { return GetEngine() != null ? GetEngine().Enabled : true; }
			set { if(GetEngine() != null) GetEngine().Enabled = value; }
		}
		bool IUndoEngine.UndoInProgress { get { return GetEngine() != null ? GetEngine().UndoInProgress : false; } }
		#endregion
		bool debuggingState = false;
		protected virtual bool DebuggingState {
 get {
				if(debuggingState && AllowHookDebugMode) return debuggingState;
				debuggingState = DebugInfoDesigner.IsDebugging(this);
				return debuggingState;
			}
			set {
				if(debuggingState == value) return;
				debuggingState = value;
#if DXWhidbey
				OnDebuggingStateChanged();
#endif
			}
		}
		bool IDebuggingStateProvider.IsDebuggingState { get { return DebuggingState; } } 
		protected virtual bool AllowHookDebugMode { get { return false; } }
#if DXWhidbey
		protected virtual void OnDebuggingStateChanged() {
			DebugInfoDesigner.OnDebuggingStateChanged(DebuggingState, this);
		}
		void Application_Idle(object sender, EventArgs e) {
			DebuggingState = DebugInfoDesigner.IsDebugging(this);
		}
#endif
		protected virtual bool AllowEditInherited { get { return true; } }
		protected virtual bool AllowDesigner {
			get {
#if DXWhidbey
				return AllowEditInherited || InheritanceAttribute != InheritanceAttribute.InheritedReadOnly;
#else
				return true;
#endif
			}
		}
#if DXWhidbey
		protected override InheritanceAttribute InheritanceAttribute {
			get {
				if(!InheritanceHelper.DisableVisualInheritance)
					return base.InheritanceAttribute;
				if(AllowEditInherited) return base.InheritanceAttribute;
				if(base.InheritanceAttribute != InheritanceAttribute.Inherited && base.InheritanceAttribute != InheritanceAttribute.InheritedReadOnly) {
					return base.InheritanceAttribute;
				}
				return InheritanceAttribute.InheritedReadOnly;
			}
		}
		public override DesignerActionListCollection ActionLists {
#else
		public virtual DesignerActionListCollection ActionLists {
#endif
			get {
#if DXWhidbey
				if(DebuggingState) return null;
#endif          
				return base.ActionLists;
			}
		}
		public override DesignerVerbCollection Verbs {
			get {
#if DXWhidbey
				if (DebuggingState) return null;
#endif
				return DXVerbs;
			}
		}
		DesignerVerbCollection fVerbs = new DesignerVerbCollection();
		public virtual DesignerVerbCollection DXVerbs { get { return fVerbs; } }
		protected virtual bool AllowInheritanceWrapper { get { return false; } }
		Hashtable dxInheritedProps = null;
		protected override void PostFilterProperties(IDictionary properties) {
			if(AllowInheritanceWrapper) {
				if(this.dxInheritedProps == null) this.dxInheritedProps = InitializeNestedInheritedProperties(Component, InheritanceAttribute, properties);
				DesignerHelper.FilterOutBaseIherited(this, this.dxInheritedProps);
			}
			base.PostFilterProperties(properties);
			if(AllowInheritanceWrapper) DXPostFilterProperties(dxInheritedProps, InheritanceAttribute, properties);
		}
		public override void Initialize(IComponent component) {
			((DevExpress.LookAndFeel.Design.UserLookAndFeelDefault)DevExpress.LookAndFeel.UserLookAndFeel.Default).UpdateDesignTimeLookAndFeelEx(component);
			base.Initialize(component);
			if(component != null) {
				LookAndFeelServiceHelper.AddVsLookAndFeelService(component.Site);
			}
			if(DXStackHelper.IsCalledFromDataWindow) OnInitilizeDataWindowDrop(component);
#if DXWhidbey
			if(AllowInheritanceWrapper) this.dxInheritedProps = InitializeNestedInheritedProperties(component, InheritanceAttribute, null);
			if(AllowHookDebugMode) Application.Idle += new EventHandler(Application_Idle);
#endif   
		}
		protected virtual void DXPostFilterProperties(Hashtable inheritedProps, InheritanceAttribute attribute, IDictionary properties) {
			DesignerHelper.PostFilterProperties(inheritedProps, attribute, properties);
		}
		protected virtual Hashtable InitializeNestedInheritedProperties(object component, InheritanceAttribute attribute, IDictionary properties) {
			return DesignerHelper.InitializeNestedInheritedProperties(component, attribute, properties);
		}
		protected override void Dispose(bool disposing) {
#if DXWhidbey
			if(AllowHookDebugMode) Application.Idle -= new EventHandler(Application_Idle);
#endif
			base.Dispose(disposing);
		}
		protected virtual void OnInitilizeDataWindowDrop(IComponent component) { }
		bool IsMouseMessage(Message m) {
			return m.Msg == WM_MOUSEMOVE || m.Msg == WM_LBUTTONDOWN || m.Msg == WM_SETCURSOR;
		}
		protected virtual bool AllowMouseActions { get { return true; } }
		const int WM_MOUSEMOVE = 0x200, MK_LBUTTON = 0x0001, MK_RBUTTON = 0x0002, WM_SETCURSOR = 0x0020, WM_LBUTTONDOWN = 0x0201;
		protected override void OnMouseEnter() {
			base.OnMouseEnter();
			if(AllowMouseActions) ControlMouseHelper.InvokeMouseEnter(Control);
		}
		protected override void OnMouseLeave() {
			base.OnMouseLeave();
			if(AllowMouseActions) ControlMouseHelper.InvokeMouseLeave(Control);
		}
		protected MethodInfo GetMethodInfo(string name) {
			return typeof(Control).GetMethod(name, BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.Instance);
		}	  
		protected override void WndProc(ref Message m) {
			Point p = Point.Empty;
			if(!AllowDesigner || Control == null || DebuggingState) {
				base.WndProc(ref m);
				return;
			}
			if (IsMouseMessage(m)) p = new Point(m.LParam.ToInt32());
			Control ctrl = Control.FromHandle(m.HWnd);
			bool forceDefProcOnly = false;
			if(m.HWnd == Control.Handle && AllowMouseActions) {
				if(m.Msg == WM_LBUTTONDOWN) { 
					if(GetHitTest(Control.PointToScreen(p))) forceDefProcOnly = true;
				}
				if(m.Msg == WM_MOUSEMOVE) {
					int btns = m.WParam.ToInt32();
					MouseButtons buttons = MouseButtons.None;
					if((btns & MK_LBUTTON) != 0) buttons |= MouseButtons.Left;
					if((btns & MK_RBUTTON) != 0) buttons |= MouseButtons.Right;
					ControlMouseHelper.InvokeMouseMove(Control, new MouseEventArgs(buttons, 0, p.X, p.Y, 0));
					if(Control == null) {base.WndProc(ref m); return;}
					if(GetHitTest(Control.PointToScreen(p))) forceDefProcOnly = true;
				}
			}
			if(forceDefProcOnly) {
#if DXWhidbey
				DefWndProc(ref m);
				return;
#endif
			}
			base.WndProc(ref m);
		}
		#region DataAccess
		protected internal virtual void CreateDataSource() {
			var designerHost = GetService(typeof(IDesignerHost)) as IDesignerHost;
			if(designerHost != null)
				DevExpress.Design.DataAccess.UI.DataSourceWizard.Run(designerHost, Component);
		}
		#endregion DataAccess       
	}
	public class BaseScrollableControlDesigner : ScrollableControlDesigner, DevExpress.XtraEditors.XtraScrollableControl.IXtraScrollableControlDesigner, ISmartTagGlyphObserver {		
		public override GlyphCollection GetGlyphs(GlyphSelectionType selectionType) {
			GlyphCollection res = base.GetGlyphs(selectionType);
			if(CanUseComponentSmartTags) {
				SmartTagInfo ti = ComponentSmartTagProvider.UpdateGlyphs(res);
				if(ti != null) {
					OnComponentSmartTagChangedCore(ti.OwnerControl, ti.GlyphBounds);
				}
			}
			return res;
		}
		DesignerActionListCollection actionList;
		public override DesignerActionListCollection ActionLists {
			get {
				if(actionList == null) actionList = CreateActionLists();
				return actionList;
			}
		}
		protected virtual DesignerActionListCollection CreateActionLists() {
			DesignerActionListCollection list = new DesignerActionListCollection();
			RegisterActionLists(list);
			return list;
		}
		protected virtual void RegisterActionLists(DesignerActionListCollection list) {
			RegisterAboutAction(list);
		}
		protected virtual void RegisterAboutAction(DesignerActionListCollection list) {
			DXAboutActionList about = GetAboutAction();
			if(about != null) {
				list.Insert(list.Count, about);
			}
			DXSmartTagsHelper.CreateDefaultLinks(this, list);
		}
		protected virtual DXAboutActionList GetAboutAction() { return null; }
		#region Component Smart Tags
		protected virtual bool CanUseComponentSmartTags {
			get { return false; }
		}
		ISmartTagProvider componentSmartTagProviderCore = null;
		protected ISmartTagProvider ComponentSmartTagProvider {
			get {
				if(componentSmartTagProviderCore == null) {
					componentSmartTagProviderCore = CreateComponentSmartTagProviderCore(Component.Site);
				}
				return componentSmartTagProviderCore;
			}
		}
		protected virtual ISmartTagProvider CreateComponentSmartTagProviderCore(IServiceProvider serviceProvider) {
			return new ControlSmartTagProviderBase(serviceProvider);
		}
		#endregion
		#region ISmartTagGlyphObserver
		void ISmartTagGlyphObserver.OnComponentSmartTagChanged(Control owner, System.Drawing.Rectangle glyphBounds) {
			OnComponentSmartTagChangedCore(owner, glyphBounds);
		}
		#endregion
		protected virtual void OnComponentSmartTagChangedCore(Control owner, System.Drawing.Rectangle glyphBounds) {
		}
		#region IXtraScrollableControlDesigner Members
		void DevExpress.XtraEditors.XtraScrollableControl.IXtraScrollableControlDesigner.Update() {
			Update();
		}
		protected virtual void Update() { }
		#endregion
	}
	public delegate string GetDisplayNameDelegate();
	public class DesignerActionHeaderItemEx : DesignerActionTextItem {
		GetDisplayNameDelegate getDisplayName;
		public DesignerActionHeaderItemEx(GetDisplayNameDelegate getDisplayName) : base("", "") {
			this.getDisplayName = getDisplayName;
		}
		public override string DisplayName {
			get {
				return getDisplayName();
			}
		}
		public override string Category {
			get {
				return DisplayName; ;
			}
		}
	}
	public class EditorContextHelperEx {
		public static void RefreshSmartPanel(IComponent component) {
			object actionUI = GetActionUI(component);
			if(actionUI != null && RefreshSmartPanelCore(actionUI, component)) return;
			DesignerActionUIService uiService = GetDesignerUIService(component);
			if(uiService != null) uiService.Refresh(component);
		}
		static bool RefreshSmartPanelCore(object actionUI, IComponent component) {
			var mi = actionUI.GetType().GetMethod("RecreateInternal", BindingFlags.Instance | BindingFlags.NonPublic);
			if(mi != null) {
				mi.Invoke(actionUI, new object[] { component });
				return true;
			}
			return false;
		}
		public static void HideSmartPanel(IComponent component) {
			DesignerActionUIService uiService = GetDesignerUIService(component);
			if(uiService != null) uiService.HideUI(component);
		}
		public static void ShowSmartPanel(IComponent component) {
			DesignerActionUIService uiService = GetDesignerUIService(component);
			if(uiService != null) uiService.ShowUI(component);
		}
		public static DesignerActionUIService GetDesignerUIService(IComponent component) {
			if(component == null || component.Site == null) return null;
			return component.Site.GetService(typeof(DesignerActionUIService)) as DesignerActionUIService;
		}
		public static object GetActionUI(IComponent component) {
			if(component == null || component.Site == null) return null;
			Type type = typeof(DocumentDesigner).Assembly.GetType("System.Windows.Forms.Design.Behavior.SelectionManager", false);
			if(type == null) return null;
			object selectionManager = component.Site.GetService(type);
			if(selectionManager == null) return null;
			var fi = selectionManager.GetType().GetField("designerActionUI", BindingFlags.NonPublic | BindingFlags.Instance);
			if(fi != null) return fi.GetValue(selectionManager);
			return null;
		}
		public static IComponent GetLastActionUIComponent(IComponent component) {
			object actionUI = GetActionUI(component);
			if(actionUI == null) return null;
			var pi = actionUI.GetType().GetProperty("LastPanelComponent", BindingFlags.NonPublic | BindingFlags.Instance);
			if(pi != null) return pi.GetValue(actionUI, null) as IComponent;
			return null;
		}
	}
	public class SingleMethodActionList : DesignerActionList {
		MethodInvoker method;
		string displayName;
		bool useAsVerb;
		public SingleMethodActionList(ComponentDesigner designer, MethodInvoker method, string displayName, bool useAsVerb)
			: base(designer.Component) {
			this.useAsVerb = useAsVerb;
			this.displayName = displayName;
			this.method = method;
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection res = new DesignerActionItemCollection();
			res.Add(new DesignerActionMethodItem(this, "Invoke", this.displayName, this.useAsVerb));
			return res;
		}
		public void Invoke() {
			if(this.method != null) this.method();
		}
	}
	public class DXStackHelper {
		public static bool IsCalledFromDataWindow {
			get {
#if DXWhidbey
				return ContainsMethod("InvokeTableCreator", 3, 16);
#else
				return false;
#endif
			}
		}
		public static bool ContainsMethod(string method, int minFrameCount, int maxFramePosition) {
			System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
			if(st.FrameCount > minFrameCount) {
				for(int n = 2; n < st.FrameCount; n++) {
					if(n > maxFramePosition) break;
					System.Diagnostics.StackFrame sf = st.GetFrame(n);
					System.Reflection.MemberInfo mi = sf.GetMethod();
					if(mi != null && mi.Name == method) return true;
				}
			}
			return false;
		}
	}
	public class DataSourceWizardDesignerActionMethodItem : DesignerActionMethodItem {
		public DataSourceWizardDesignerActionMethodItem(DesignerActionList actionList) :
			base(actionList, "CreateDataSource", GetActionName()) {
		}
		public DataSourceWizardDesignerActionMethodItem(DesignerActionList actionList, string category) :
			base(actionList, "CreateDataSource", GetActionName(), category) {
		}
		static string GetActionName() {
			return DevExpress.Design.DataAccess.DataAccessLocalizer.GetString(DevExpress.Design.DataAccess.DataAccessLocalizerStringId.DataSourceSmartTagActionName);
		}
	}
}
