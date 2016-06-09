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
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.XtraBars.Design;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraBars.Docking.Helpers;
using DevExpress.Utils.Design;
namespace DevExpress.XtraBars.Docking.Design {
	public class MouseHelper {
		Control control;
		const int WM_MOUSEMOVE = 0x200, WM_LBUTTONDOWN = 0x201, WM_LBUTTONUP = 0x202, MK_LBUTTON = 0x0001;
		static string[] methodNames = new string[] { "OnMouseMove", "OnMouseDown", "OnMouseUp" };
		public MouseHelper(Control control) {
			this.control = control;
		}
		public void ProcessMouseEvent(ref Message m) {
			if(m.Msg != WM_LBUTTONUP && m.Msg != WM_LBUTTONDOWN) return;
			Point p = new Point(m.LParam.ToInt32());
			int btns = m.WParam.ToInt32();
			MouseButtons buttons = MouseButtons.None;
			if((btns & MK_LBUTTON) != 0) buttons |= MouseButtons.Left;
			MethodInfo mi = GetMethodInfo(methodNames[m.Msg - WM_MOUSEMOVE]);
			if(mi != null)
				mi.Invoke(Control, new object[] { new MouseEventArgs(buttons, 0, p.X, p.Y, 0) });
		}
		public void MouseEnter() {
			MethodInfo mi = GetMethodInfo("OnMouseEnter");
			if(mi != null) mi.Invoke(Control, new object[] { EventArgs.Empty });
		}
		public void MouseLeave() {
			MethodInfo mi = GetMethodInfo("OnMouseLeave");
			if(mi != null) mi.Invoke(Control, new object[] { EventArgs.Empty });
		}
		protected MethodInfo GetMethodInfo(string name) {
			return typeof(Control).GetMethod(name, BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.Instance);
		}
		public Point PointToClient(int x, int y) {
			return Control.PointToClient(new Point(x, y));
		}
		public void SelectControl(Control controlToSelect) {
			ISelectionService selServ = GetSelectionService();
			if(selServ != null) {
				if(controlToSelect == null)
					selServ.SetSelectedComponents(new object[] { }); 
				else
					selServ.SetSelectedComponents(new object[] { controlToSelect });
			}
		}
		internal ISelectionService GetSelectionService() {
			if(Control.Site != null)
				return (Control.Site.GetService(typeof(ISelectionService)) as ISelectionService);
			return null;
		}
		protected Control Control { get { return control; } }
	}
	public class DockPanelSmartTagProvider : ControlSmartTagProviderBase {
		public DockPanelSmartTagProvider(IServiceProvider provider) : base(provider) { }
		protected override bool CheckComponent(IComponent component) {
			DockPanel panel = component as DockPanel;
			if(panel != null)
				return (base.CheckComponent(component) && (panel.Dock != DockingStyle.Float || panel.DockedAsTabbedDocument == true));
			return false;
		}
		protected override IComponentSmartTagInfoParser CreateSmartTagInfoParser() {
			return new DockPanelSmartTagParser();
		}
	}
	public class DockPanelSmartTagParser : ComponentSmartTagInfoParserBase {
		protected override DesignerActionList CreateDesignerActionListCore(ComponentDesigner designer, IComponent component) {
			return new DesignerActionListDockPanel(designer, component);
		}
	}
	public class DesignerActionListDockPanel : SmartDesignerActionList {
		System.Collections.Generic.List<string> filter;
		public DesignerActionListDockPanel(ComponentDesigner designer, IComponent component) : base(designer, component) {
			filter = new System.Collections.Generic.List<string>();
		}
		DockPanel Panel { get { return Component as DockPanel; } }
		protected override bool PreFilterProperties(MemberDescriptor descriptor) {
			return Contains(FilterProperties, descriptor.Name);
		}
		string[] FilterProperties {
			get {
				if(Panel != null && Panel.DockedAsTabbedDocument)
					return new string[] { "Visibility" };
				return null;
			}
		}
		bool Contains(string[] filter, string value) {
			if(filter != null) {
				foreach(string str in filter)
					if(value == str) return false;
			}
			return true;
		}
		protected override bool PreFilterMethod(DesignerActionMethodItem actionMethodItem) {
			return Contains(FilterAttributes, actionMethodItem.MemberName);
		}
		string[] FilterAttributes {
			get {
				if(Panel != null) {
					GetFilterDockedAsTabbedDocumentAttributes(Panel);								  
					GetFilterDockingStyleAttributes(Panel, Panel.DockedAsTabbedDocument);
				}
				return filter.ToArray();
			}
		}
		void GetFilterDockingStyleAttributes(DockPanel panel, bool dockTabbed) { 
			if((panel.Dock == DockingStyle.Bottom && !dockTabbed) || !panel.Options.AllowDockBottom) filter.Add("DockToBottom");
			if((panel.Dock == DockingStyle.Left && !dockTabbed) || !panel.Options.AllowDockLeft) filter.Add("DockToLeft");
			if((panel.Dock == DockingStyle.Top && !dockTabbed) || !panel.Options.AllowDockTop) filter.Add("DockToTop");
			if((panel.Dock == DockingStyle.Right && !dockTabbed) || !panel.Options.AllowDockRight) filter.Add("DockToRight");
			if(!panel.Options.AllowFloating) filter.Add("Float");			
		}
		void GetFilterDockedAsTabbedDocumentAttributes(DockPanel panel) {
			if(panel == null || panel.DockManager == null) return;
			Docking2010.DocumentManager manager = Docking2010.DocumentManager.FromContainer(panel.DockManager.Container, panel.DockManager.Form) ?? Docking2010.DocumentManager.FromControl(panel.DockManager.Form);
			if(manager == null) {
				filter.Add("DockedAsTabbedDocument");
				return;
			}
			if(panel.DockedAsTabbedDocument) {
				filter.AddRange(new string[] { "DockedAsTabbedDocument", "AddCustomHeaderButtons" });
				return;
			}
			if(!panel.Options.AllowDockAsTabbedDocument)
				filter.Add("DockedAsTabbedDocument");
		}
	}
	public class DockPanelDesigner : BaseControlDesigner {
		public override bool CanBeParentedTo(IDesigner parentDesigner) {
			if(Control.Parent is DockPanel) return false;
			return base.CanBeParentedTo(parentDesigner);
		}
		MouseHelper mouseHelper;
		public DockPanelDesigner() {
		}
		protected override bool CanUseComponentSmartTags {
			get { return true; }
		}
		protected override ISmartTagProvider CreateComponentSmartTagProviderCore(IServiceProvider serviceProvider) {
			return new DockPanelSmartTagProvider(serviceProvider);
		}
		protected override void PreFilterProperties(IDictionary properties) {
			if(Panel != null && Panel.DockedAsTabbedDocument) {
				if(Panel.DockManager != null && Panel.DockManager.SerializationInProgress) return;
				foreach(string prop in FilterProperties)
					properties.Remove(prop);
			}
			base.PreFilterProperties(properties);
		}
		string[] FilterProperties {
			get {
				return new string[] { "Visibility", "Dock", "TabText", "TabsPosition", "TabsScroll", "Size", "Padding", "Margin", "Location", "AutoScroll", "AutoScrollMargin", "AutoScrollMinSize", "Anchor", "CustomHeaderButtons" };
			}
		}
		protected override bool UseVerbsAsActionList { get { return true; } }
#if DXWhidbey
		protected override bool AllowHookDebugMode { get { return true; } }
#endif
		public override void Initialize(IComponent component) {
#if DXWhidbey
			FieldInfo fi = typeof(ControlDesigner).GetField("forceVisible", BindingFlags.Instance | BindingFlags.NonPublic);
			fi.SetValue(this, false);
#endif
			base.Initialize(component);
			this.mouseHelper = new MouseHelper(Control);
			this.Control.SizeChanged += new EventHandler(Control_SizeChanged); 
		}
		void Control_SizeChanged(object sender, EventArgs e) {
			ISelectionService service = MouseHelper.GetSelectionService();
			if(service != null && service.GetComponentSelected(Control)) {
				MouseHelper.SelectControl(null);
				MouseHelper.SelectControl(Control);
			}
		}
		protected override void Dispose(bool disposing) {
			this.Control.SizeChanged -= new EventHandler(Control_SizeChanged);
			base.Dispose(disposing);
		}
		protected override void WndProc(ref Message m) {
			base.WndProc(ref m);
			if(DebuggingState) return;
			if(MouseHelper != null) MouseHelper.ProcessMouseEvent(ref m);
			const int WM_CANCELMODE = 0x001F;
			if(m.Msg == WM_CANCELMODE)
				CancelCurrentMode();
		}
		protected override void OnMouseEnter() {
			base.OnMouseEnter();
			MouseHelper.MouseEnter();
		}
		protected override void OnMouseLeave() {
			base.OnMouseLeave();
			MouseHelper.MouseLeave();
		}
		protected override bool GetHitTest(Point pt) {
			if(DebuggingState) return false;
			if(Panel == null) return false;
			return (Panel.GetHitInfo(pt).ResizeZone != null);
		}
		protected virtual void CancelCurrentMode() {
			if(Panel == null) return;
			Type panelType = Panel.GetType();
			MethodInfo info = panelType.GetMethod("ResetDragging",
				BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			if(info != null) {
				info.Invoke(Panel, new object[] { true });
			}
		}
		public override ICollection AssociatedComponents {
			get {
				if(Panel == null) return base.AssociatedComponents;
				if(Panel.ControlContainer == null) return base.AssociatedComponents;
				ArrayList al = new ArrayList(base.AssociatedComponents);
				al.Add(Panel.ControlContainer);
				return al;
			}
		}
		protected override void OnMouseDragEnd(bool cancel) {
			base.OnMouseDragEnd(true);
		}
		protected override void OnMouseDragMove(int x, int y) { }
		protected override void OnMouseDragBegin(int x, int y) {
			if(Panel == null) return;
			base.OnMouseDragBegin(x, y);
			Point ptClient = MouseHelper.PointToClient(x, y);
			HitInfo hitInfo = Panel.GetHitInfo(ptClient);
			MouseHelper.SelectControl(hitInfo.HitTest == HitTest.Tab ? hitInfo.Tab : Panel);
		}
		public override SelectionRules SelectionRules {
			get { return SelectionRules.None; }
		}
		protected DockPanel Panel { get { return Control as DockPanel; } }
		protected MouseHelper MouseHelper { get { return mouseHelper; } }
	}
	public class BaseParentDockControlDesigner : BaseParentControlDesigner {
		MouseHelper mouseHelper;
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			this.mouseHelper = new MouseHelper(Control);
		}
		protected override bool UseVerbsAsActionList { get { return true; } }
#if DXWhidbey
		protected override bool AllowHookDebugMode { get { return true; } }
#endif
		protected MouseHelper MouseHelper { get { return mouseHelper; } }
	}
	public class ContainerControlDesigner : BaseParentDockControlDesigner {
		protected override void OnMouseDragBegin(int x, int y) {
			base.OnMouseDragBegin(x, y);
			MouseHelper.SelectControl(Control.Parent);
		}
		public override bool CanBeParentedTo(IDesigner parentDesigner) {
			return false;
		}
		public override bool CanParent(Control control) {
			if(control is DockPanel) return false;
			return base.CanParent(control);
		}
#if DXWhidbey
		public override System.Windows.Forms.Design.Behavior.GlyphCollection GetGlyphs(System.Windows.Forms.Design.Behavior.GlyphSelectionType selectionType) {
			System.Windows.Forms.Design.Behavior.GlyphCollection col = base.GetGlyphs(selectionType);
			col.Clear();
			return col;
		}
#endif
	}
}
