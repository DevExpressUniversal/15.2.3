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
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Utils.Design;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ButtonsPanelControl;
using DevExpress.Utils.Drawing;
using System.Windows.Forms.Design.Behavior;
namespace DevExpress.XtraEditors.Design {
	public class GroupDesigner : BaseParentControlDesigner { 
		protected override void OnPaintAdornments(PaintEventArgs pe) {
			if(this.DrawGrid) {
				Rectangle r = Control.DisplayRectangle;
				r.Width++;
				r.Height++;
				ControlPaint.DrawGrid(pe.Graphics, r, base.GridSize, Control.BackColor);
			}
			DrawBorder(pe.Graphics);
		}
		protected virtual void DrawBorder(Graphics graphics) {
			if((typeof(PanelControl).Assembly.GetHashCode() != Component.GetType().Assembly.GetHashCode() &&
				Component.GetType().Name == "PanelControl") || (Panel as PanelControl) == null) return; 
			if(Panel.BorderStyle != BorderStyles.NoBorder) return;
			DrawBorder(graphics, Panel, Panel.BackColor);
		}
		public PanelControl Panel { get { return Control as PanelControl; } }
		public static void DrawBorder(Graphics graphics, Control control, Color backColor) {
			DXControlPaint.DrawDashedBorder(graphics, control, backColor);
		}
		protected override bool CanUseComponentSmartTags { get { return true; } }
	}
	public class GroupBoxButtonCollectionEditor : DXCollectionEditorBase {
		public GroupBoxButtonCollectionEditor(Type type) : base(type) { }
		protected override Type[] CreateNewItemTypes() {
			return new Type[] { typeof(GroupBoxButton) };
		}
		protected override object CreateCustomInstance(Type itemType) {
			return new GroupBoxButton();
		}
		protected override bool AllowLiveUpdates {
			get { return true; }
		}
	}
	public class SplitGroupPanelDesigner : GroupDesigner {
		ISelectionService selectionService;
		IDesignerHost host;
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			this.host = GetService(typeof(IDesignerHost)) as IDesignerHost;
			if(host == null) return;
			this.selectionService = (ISelectionService)host.GetService(typeof(ISelectionService));
			this.selectionService.SelectionChanged += new EventHandler(OnSelectionChanged);
			MenuHandlerService.InstallService(host, new AllowMenuCommandEventHandler(OnAllowMenuCommand));
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(this.host != null)
					MenuHandlerService.UnInstallService(host, new AllowMenuCommandEventHandler(OnAllowMenuCommand));
				if(this.selectionService != null) {
					this.selectionService.SelectionChanged -= new EventHandler(OnSelectionChanged);
					this.selectionService = null;
				}
			}
			base.Dispose(disposing);
		}
		public override DesignerActionListCollection ActionLists { get { return null; } }
		protected override bool CanUseComponentSmartTags { get { return false; } }
		protected override void DrawBorder(Graphics graphics) { }
#if DXWhidbey
		protected override InheritanceAttribute InheritanceAttribute {
			get {
				if(host != null && this.Panel.Owner != null) {
					SplitContainerControlDesigner designer = host.GetDesigner(Panel.Owner) as SplitContainerControlDesigner;
					if(designer != null) return designer.GetInheritanceAttribute();
				}
				if(!InheritanceHelper.DisableVisualInheritance)
					return base.InheritanceAttribute;
				if(AllowEditInherited) return base.InheritanceAttribute;
				if(base.InheritanceAttribute != InheritanceAttribute.Inherited && base.InheritanceAttribute != InheritanceAttribute.InheritedReadOnly) {
					return base.InheritanceAttribute;
				}
				return InheritanceAttribute.InheritedReadOnly;
			}
		}
#endif
		public new SplitGroupPanel Panel { get { return Control as SplitGroupPanel; } }
		protected void OnAllowMenuCommand(object sender, AllowMenuCommandEventArgs e) {
			if(Panel == null || Panel.Owner == null) return;
			CheckMenu(Panel.Owner, e);
		}
		bool prevSelected = false;
		protected void OnSelectionChanged(object sender, EventArgs e) {
			bool oldSelected = this.prevSelected;
			this.prevSelected = this.selectionService.GetComponentSelected(Panel);
			if(oldSelected != prevSelected) Panel.Invalidate();
		}
		protected override void OnPaintAdornments(PaintEventArgs e) {
			base.OnPaintAdornments(e);
			if(this.selectionService.GetComponentSelected(Panel)) {
				using(Pen pen = new Pen(ControlPaint.Light(Panel.ForeColor))) {
					pen.DashStyle = DashStyle.Dash;
					Rectangle r = Rectangle.Inflate(Panel.ClientRectangle, -1, -1);
					DevExpress.Utils.Paint.XPaint.Graphics.DrawRectangle(e.Graphics, pen, r);
				}
			}
		}
		internal static void CheckMenu(SplitContainerControl container, AllowMenuCommandEventArgs e) {
			if(e.SelectionService == null) {
				e.Allow = false;
				return;
			}
			if(e.Command == MenuCommands.Delete || e.Command == MenuCommands.Copy || e.Command == MenuCommands.Cut) {
				if(!IsAnySelected(container, e)) return;
				if(e.SelectionService.GetComponentSelected(container)) return;
				e.Allow = e.SelectionService.GetComponentSelected(container) &&
					e.SelectionService.GetComponentSelected(container.Panel1) && e.SelectionService.GetComponentSelected(container.Panel2);
			}
		}
		static bool IsAnySelected(SplitContainerControl container, AllowMenuCommandEventArgs e) {
			return e.SelectionService.GetComponentSelected(container) ||
				e.SelectionService.GetComponentSelected(container.Panel1) || e.SelectionService.GetComponentSelected(container.Panel2);
		}
		PropertyDescriptor nameDescriptor = null;
		string Name {
			get {
				if(nameDescriptor != null) nameDescriptor.GetValue(Component).ToString();
				return "";
			}
			set {
				if(Panel != null && Panel.Owner != null) value = Panel.Owner.Name + "_Panel" + (Panel.Owner.Panel1 == Panel ? "1" : "2");
				if(nameDescriptor != null) nameDescriptor.SetValue(Component, value);
			}
		}
		public override void DoDefaultAction() {
		}
		protected override void PreFilterProperties(IDictionary properties) {
			base.PreFilterProperties(properties);
			ArrayList list = new ArrayList(properties.Keys);
			for(int n = list.Count - 1; n >= 0; n--) {
				string name = list[n].ToString();
				if(Array.IndexOf(SplitContainerControlDesigner.panelProperties, name) != -1) continue;
#if !DXWhidbey
				PropertyDescriptor pd = properties[name] as PropertyDescriptor;
				if(pd.DisplayName == "Name") {
					this.nameDescriptor = properties[name] as PropertyDescriptor;
					properties[name] = TypeDescriptor.CreateProperty(typeof(SplitGroupPanelDesigner), this.nameDescriptor, new Attribute[0]);
					continue;
				}
#endif
				properties.Remove(name);
			}
		}
	}
	public class SplitContainerControlDesigner : GroupDesigner {
		FilterObject panel1 = null, panel2 = null;
		IDesignerHost host;
		IComponentChangeService changeService;
		ISelectionService selectionService;
		bool justDropped = false;
		protected VisualizersBehavior visualizersBehaviourCore;
		protected override bool AllowControlLasso {
			get { return false; }
		}
		public override ICollection AssociatedComponents {
			get {
				if(Split == null) return base.AssociatedComponents;
				ArrayList controls = new ArrayList(base.AssociatedComponents);
				if(!controls.Contains(Split.Panel1)) controls.Add(Split.Panel1);
				if(!controls.Contains(Split.Panel2)) controls.Add(Split.Panel2);
				return controls;
			}
		}
		internal InheritanceAttribute GetInheritanceAttribute() { return InheritanceAttribute; }
		protected override void OnPaintAdornments(PaintEventArgs pe) { }
		public override GlyphCollection GetGlyphs(GlyphSelectionType selectionType) {
			GlyphCollection glyphs = base.GetGlyphs(selectionType);
			if(selectionType == GlyphSelectionType.NotSelected) 
				glyphs.Insert(0, new SelectorGlyph(CreateSelectorGlyphBounds(), visualizersBehaviourCore, Split.BackColor));
			return glyphs;
		}
		Rectangle CreateSelectorGlyphBounds() {
			Point location = BehaviorService.ControlToAdornerWindow((Control)base.Component);
			Rectangle containerBounds = new Rectangle(location, Split.Size);
			return Rectangle.Inflate(containerBounds, 5, 5);
		}
		public BehaviorService BehaviorServiceCore {
			get {
				return BehaviorService;
			}
		}
		protected override void OnCreateHandle() {
			base.OnCreateHandle();
			if(this.justDropped && Split.Parent is SplitGroupPanel) Split.SetPanelBorderStyle(BorderStyles.NoBorder);
			this.justDropped = false;
		}
#if DXWhidbey
		public override void InitializeNewComponent(IDictionary defaultValues) {
			this.justDropped = true;
			base.InitializeNewComponent(defaultValues);
			Split.Panel1.Text = Split.Panel1.Name;
			Split.Panel2.Text = Split.Panel2.Name;
		}
		protected override Control GetParentForComponent(IComponent component) {
			return Split.Panel1;
		}
		protected override IComponent[] CreateToolCore(System.Drawing.Design.ToolboxItem tool, int x, int y, int width, int height, bool hasLocation, bool hasSize) {
			SplitGroupPanel selectedPanel = GetSelectedPanel();
			SplitGroupPanelDesigner toInvoke = (SplitGroupPanelDesigner)host.GetDesigner(selectedPanel);
			ParentControlDesigner.InvokeCreateTool(toInvoke, tool);
			return null;
		}
		protected SplitGroupPanel GetSelectedPanel() {
			var selection = selectionService.GetSelectedComponents();
			if(selection != null) {
				foreach(var item in selection) {
					SplitGroupPanel panel = item as SplitGroupPanel;
					if(panel != null && panel.Parent == Split)
						return panel;
				}
			}
			return Split.Panel1;
		}
#else
		public override void OnSetComponentDefaults() {
			this.justDropped = true;
			base.OnSetComponentDefaults();
			Split.Panel1.Text = Split.Panel1.Name;
			Split.Panel2.Text = Split.Panel2.Name;
		}
#endif
		public override bool CanParent(Control control) {
			if((control is SplitGroupPanel)) return true;
			return false;
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			this.host = (IDesignerHost)component.Site.GetService(typeof(IDesignerHost));
#if DXWhidbey
			EnableDesignMode(Split.Panel1, "Panel1");
			EnableDesignMode(Split.Panel2, "Panel2");
#else
			this.host.Container.Add(Split.Panel1);
			this.host.Container.Add(Split.Panel2);
			UpdateNames();
#endif
			if(host != null) {
				selectionService = (ISelectionService)this.host.GetService(typeof(ISelectionService));
				if(selectionService != null)
					selectionService.SelectionChanged += OnSelectionChanged;
				changeService = (IComponentChangeService)this.host.GetService(typeof(IComponentChangeService));
				if(changeService != null)
					changeService.ComponentRename += OnComponentRename;
			}
			visualizersBehaviourCore = new VisualizersBehavior(this);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(this.changeService != null) {
					this.changeService.ComponentRename -= OnComponentRename;
					this.changeService = null;
				}
				if(this.selectionService != null) {
					this.selectionService.SelectionChanged -= OnSelectionChanged;
					this.selectionService = null;
				}
			}
			base.Dispose(disposing);
		}
		protected void InvalidateParent() {
			if(Split.Parent != null) {
				Split.Parent.Invalidate();
				Split.Invalidate();
			}
		}
		bool prevSelected = false;
		protected void OnSelectionChanged(object sender, EventArgs e) {
			bool oldSelected = this.prevSelected;
			this.prevSelected = this.selectionService.GetComponentSelected(Split);
			if(oldSelected != prevSelected) InvalidateParent();
		}
		protected void OnComponentRename(object sender, ComponentRenameEventArgs e) {
			if(e.Component == Split) {
				UpdateNames();
			}
		}
		protected void OnAllowMenuCommand(object sender, AllowMenuCommandEventArgs e) {
			if(Split == null) return;
			SplitGroupPanelDesigner.CheckMenu(Split, e);
		}
		void UpdateNames() {
			try {
				if(Split.Panel1.Site != null)
					Split.Panel1.Site.Name = Split.Name + "_Panel1";
				if(Split.Panel2.Site != null)
					Split.Panel2.Site.Name = Split.Name + "_Panel2";
			}
			catch {
			}
		}
		public object Panel1 {
			get {
				if(panel1 == null && Split != null) {
					panel1 = new ComponentFilterObject(Split.Panel1, GetPanelObjects(Split.Panel1), panelProperties);
				}
				return panel1;
			}
		}
		public object Panel2 {
			get {
				if(panel2 == null && Split != null) {
					panel2 = new ComponentFilterObject(Split.Panel2, GetPanelObjects(Split.Panel2), panelProperties);
				}
				return panel2;
			}
		}
		internal static object[] GetPanelObjects(SplitGroupPanel panel) {
			object[] res = new object[panelProperties.Length];
			for(int n = 0; n < res.Length; n++) res[n] = null;
			res[0] = panel;
			return res;
		}
		internal static string[] panelProperties = new string[] { 
				"Controls", "MinSize", "ShowCaption", "BorderStyle", "Tag",
				"Appearance", "AppearanceCaption", "Text", "CaptionLocation", "AutoScroll",
#if DXWhidbey
				"Padding"
#else
				"DockPadding"
#endif
		};
		protected override bool GetHitTest(Point pt) {
			pt = Control.PointToClient(pt);
			bool ret = Split.SplitterBounds.Contains(pt);
			if(ret)
				Cursor.Current = Split.Horizontal ? Cursors.VSplit : Cursors.HSplit;
			return ret;
		}
		bool IsMouseMessage(Message m) {
			return m.Msg == WM_MOUSEMOVE || m.Msg == WM_LBUTTONDOWN || m.Msg == WM_SETCURSOR;
		}
		const int WM_MOUSEMOVE = 0x200, MK_LBUTTON = 0x0001, MK_RBUTTON = 0x0002, WM_SETCURSOR = 0x0020, WM_LBUTTONDOWN = 0x0201;
		protected MethodInfo GetMethodInfo(Control control, string name) {
			return control.GetType().GetMethod(name, BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.Instance);
		}
		protected override void WndProc(ref Message m) {
			Point p = Point.Empty;
			if(IsMouseMessage(m)) p = new Point(m.LParam.ToInt32());
			Control ctrl = Control.FromHandle(m.HWnd);
			bool forceDefProcOnly = false;
			if(m.HWnd == ctrl.Handle) {
				if(m.Msg == WM_LBUTTONDOWN) { 
					if(GetHitTest(p)) forceDefProcOnly = true;
				}
				if(m.Msg == WM_MOUSEMOVE) {
					int btns = m.WParam.ToInt32();
					MouseButtons buttons = MouseButtons.None;
					if((btns & MK_LBUTTON) != 0) buttons |= MouseButtons.Left;
					if((btns & MK_RBUTTON) != 0) buttons |= MouseButtons.Right;
					MethodInfo mi = GetMethodInfo(ctrl, "OnMouseMove");
					if(mi != null) mi.Invoke(ctrl, new object[] { new MouseEventArgs(buttons, 0, p.X, p.Y, 0) });
					if(GetHitTest(p)) forceDefProcOnly = true;
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
		protected override void OnMouseEnter() {
			base.OnMouseEnter();
			MethodInfo mi = GetMethodInfo(Control, "OnMouseEnter");
			if(mi != null) mi.Invoke(Control, new object[] { EventArgs.Empty });
		}
		protected override void OnMouseLeave() {
			base.OnMouseLeave();
			MethodInfo mi = GetMethodInfo(Control, "OnMouseLeave");
			if(mi != null) mi.Invoke(Control, new object[] { EventArgs.Empty });
		}
		public SplitContainerControl Split { get { return Component as SplitContainerControl; } }
#if DXWhidbey
#else
		protected override void PreFilterProperties(IDictionary properties) {
			base.PreFilterProperties(properties);
			Attribute[] attr = new Attribute[] { 
												   new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content),
												   new TypeConverterAttribute(typeof(ExpandableObjectConverter))
											   };
			string[] shadow = new string[] {"Panel1", "Panel2", "Splitter"};
			for(int i = 0; i < shadow.Length; i++) {
				PropertyDescriptor prop = (PropertyDescriptor)properties[shadow[i]];
				if(prop != null) 
					properties[shadow[i]] = TypeDescriptor.CreateProperty(typeof(SplitContainerControlDesigner), prop.Name, typeof(object), attr);
			}
		}
#endif
	}
	public class SplitGroupPanelSerializer : System.ComponentModel.Design.Serialization.CodeDomSerializer {
#if !DXWhidbey
		public override object Deserialize(IDesignerSerializationManager manager, object codeObject) {
			return null;
		}
		public override object Serialize(IDesignerSerializationManager manager, object value) { 
			return null;
		}
#endif
	}
	public class ComponentFilterObject : FilterObject {
		bool allowAllEvents = true;
		public ComponentFilterObject(object sourceObject, string[] supportedProperties) : base(sourceObject, supportedProperties) { }
		public ComponentFilterObject(object sourceObject, object[] sampleObjects, string[] supportedProperties) : base(sourceObject, sampleObjects, supportedProperties) { }
		protected override bool IsSupportPropertyDescriptor(PropertyDescriptor propDescriptor) {
			if(propDescriptor.SerializationVisibility == DesignerSerializationVisibility.Hidden) return false;
			return true;
		}
		public bool AllowAllEvents { get { return allowAllEvents; } set { allowAllEvents = value; } }
		protected override bool IsNestedPropertyDescriptor(PropertyDescriptor propDescriptor) {
			if(propDescriptor.Name == "Controls") return false;
			return base.IsNestedPropertyDescriptor(propDescriptor);
		}
		protected override bool IsEventSupported(EventDescriptor eventDescriptor) {
			if(AllowAllEvents) return true;
			return base.IsEventSupported(eventDescriptor);
		}
	}
	public class VisualizersBehavior : Behavior {
		protected SplitContainerControlDesigner owner;
		public VisualizersBehavior(SplitContainerControlDesigner owner) {
			this.owner = owner;
		}
		public BehaviorService BehaviorService { get { return owner.BehaviorServiceCore; } }
		public Rectangle OwnerRectInAdronerWindow() {
			if(owner == null || owner.Component == null || BehaviorService == null) return Rectangle.Empty;
			return this.BehaviorService.ControlRectInAdornerWindow(owner.Split);
		}
	}
	public class SelectorGlyph : Glyph {
		Rectangle glyphBounds;
		Color backColor;
		public override Rectangle Bounds { get { return glyphBounds; } }
		public SelectorGlyph(Rectangle glyphBounds, VisualizersBehavior behavior, Color backColor) : base(behavior) {
			this.glyphBounds = glyphBounds;
			this.backColor = backColor;
		}
		public override Cursor GetHitTest(Point p) {
			return null;
		}
		public override void Paint(PaintEventArgs pe) {
			ControlPaint.DrawSelectionFrame(pe.Graphics, true, glyphBounds, Rectangle.Inflate(glyphBounds, -2, -2), backColor);
		}
	}
}
