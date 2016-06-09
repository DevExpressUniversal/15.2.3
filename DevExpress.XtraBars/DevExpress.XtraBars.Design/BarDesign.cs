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
using System.Reflection;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Windows.Forms;
using System.Windows.Forms.Design;
#if DXWhidbey
using System.Windows.Forms.Design.Behavior;
#endif
using System.Drawing;
using System.Drawing.Design;
using DevExpress.XtraEditors.Design;
using DevExpress.XtraEditors.Repository;
using DevExpress.Utils.Design;
using DevExpress.XtraBars.Navigation;
namespace DevExpress.XtraBars.Design {
	public class BarShortcutEditor : UITypeEditor {
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if(context == null || context.Instance == null || provider == null)
				return value;
			IWindowsFormsEditorService edSvc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
			if(edSvc == null) return value;
			object savedValue = value;
			BarShortcutEditorForm form = new BarShortcutEditorForm(this, edSvc, value);
			edSvc.DropDownControl(form);
			value = form.EditValue;
			if(value is string) {
				value = DevExpress.XtraBars.Editors.ShortcutEditor.ShowShortcutEditor(edSvc, savedValue as BarShortcut); 
			}
			form.Dispose();
			return value;
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			if(context != null && context.Instance != null)
				return UITypeEditorEditStyle.DropDown;
			return base.GetEditStyle(context);
		}
	}
	[ToolboxItem(false)]
	public class BarShortcutEditorForm : Panel {
		BarShortcutEditor editor;
		ListBox listBox;
		object editValue, originalValue;
		IWindowsFormsEditorService edSvc;
		public BarShortcutEditorForm(BarShortcutEditor editor, IWindowsFormsEditorService edSvc, object editValue) {
			this.editValue = this.originalValue = editValue;
			this.editor = editor;
			this.edSvc = edSvc;
			this.BorderStyle = BorderStyle.None;
			listBox = new ListBox();
			listBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			listBox.Dock = DockStyle.Fill;
			foreach(object obj in DevExpress.XtraBars.TypeConverters.BarShortcutTypeConverter.KeysList) {
				listBox.Items.Add(obj);
			}
			listBox.Items.Insert(1, "(custom)");
			if(listBox.Items.Contains(EditValue))
				listBox.SelectedItem = EditValue; 
			else 
				listBox.SelectedIndex = 1;
			this.listBox.SelectedValueChanged += new EventHandler(OnSelectedValueChanged);
			this.listBox.Click += new EventHandler(OnClick);
			Controls.Add(listBox);
			this.listBox.CreateControl();
			this.Size = new Size(0, listBox.ItemHeight * Math.Min(listBox.Items.Count, 10));
		}
		protected override void OnGotFocus(EventArgs e) {
			base.OnGotFocus(e);
			this.listBox.Focus();
		}
		protected virtual void OnClick(object sender, EventArgs e) {
			edSvc.CloseDropDown();
		}
		protected virtual void OnSelectedValueChanged(object sender, EventArgs e) {
			EditValue = listBox.SelectedItem;
		}
		public BarShortcutEditor Editor { get { return editor; } }
		protected override bool ProcessDialogKey(Keys keyData) {
			if(keyData == Keys.Enter) {
				edSvc.CloseDropDown();
				return true;
			}
			if(keyData == Keys.Escape) {
				editValue = originalValue;
				edSvc.CloseDropDown();
				return true;
			}
			return base.ProcessDialogKey(keyData);
		}
		public object EditValue {
			get { return editValue; }
			set {
				if(editValue == value) return;
				editValue = value;
			}
		}
	}
	public class StringsEditor : CollectionEditor {
		public StringsEditor(Type t) : base(t) {
		}
		protected override Type CreateCollectionItemType() {
			return typeof(string);
		}
		protected override object CreateInstance(Type itemType) {
			return "";
		}
	}
	public class StandaloneBarDockControlDesigner : BarDockControlDesigner {
		protected virtual BarManager GetManager(IComponent component) {
			foreach(Object cmp in component.Site.Container.Components) {
				if(cmp is BarManager) return cmp as BarManager;
			}
			return null;
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			BarManager manager = GetManager(component);
			if(manager == null || manager.DockControls.Contains(DockControl) || manager.DockControls.Count < 4) return;
			manager.DockControls.Add(DockControl);
			DockControl.InitializeDesignTime(manager, BarDockStyle.Standalone);
		}
		public new StandaloneBarDockControl DockControl { get { return base.DockControl as StandaloneBarDockControl; } }
		public override void DoDefaultAction() { DoBaseDefaultAction(); }
		protected override bool AllowHookDebugMode { get { return false; } }
		protected override void OnDebuggingStateChanged() {
			OnBaseDebuggingStateChanged();
		}
		protected override void OnComponentRemoving(object sender, ComponentEventArgs e) {
			if(e.Component == DockControl) {
				if(DockControl.HasBars && !AllowDispose) {
					throw new ApplicationException("You can't remove StandaloneBarDockControl control now");
				}
				if(Manager == null) return;
				Manager.DockControls.Remove(DockControl);
				DockControl.InitializeDesignTime(null, BarDockStyle.Standalone);
			}
		}
		public override SelectionRules SelectionRules {
			get {
				return SelectionRules.AllSizeable | SelectionRules.Moveable | SelectionRules.Visible;
			}
		}
		protected override bool GetHitTest(Point point) { return GetBaseHitTest(point); }
		protected override bool EnableDragRect { get { return GetBaseEnableDragRect(); } }
		public override DesignerVerbCollection Verbs { get { return GetBaseVerbs(); } }
		protected override void OnContextMenu(int x, int y) {
			Point pt = DockControl.PointToClient(new Point(x, y));
			if(DockControl.GetChildAtPoint(pt) == null)
				OnBaseContextMenu(x, y); 
		}
		protected override void WndProc(ref Message m) {
			DoBaseWndProc(ref m);
		}
	}
	public class BarDockControlDesigner : BaseControlDesigner {
		IDesignerHost host = null;
		IComponentChangeService changeService = null;
		public BarManager Manager {
			get {
				if(DockControl == null) return null;
				return DockControl.Manager;
			}
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(this.host != null) {
					this.host = null;
				}
			}
			base.Dispose(disposing);
		}
		protected override bool AllowEditInherited {
			get {
				return false;
			}
		}
#if DXWhidbey
		protected override bool AllowHookDebugMode { get { return true; } }
#endif
		public BarDockControl DockControl { get { return Control as BarDockControl; } }
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			this.host = GetService(typeof(IDesignerHost)) as IDesignerHost;
			this.changeService = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(this.changeService != null) {
				this.changeService.ComponentRemoving += OnComponentRemoving;
			}
			if(SelectionService != null) {
				SelectionService.SelectionChanged += new EventHandler(OnComponentSelectionChanged);
			}
		}
		protected bool AllowDispose {
			get {
				return DockControl.AllowDispose || DockControl.Parent == null;
			}
		}
		protected virtual void OnComponentRemoving(object sender, ComponentEventArgs e) {
			if(e.Component == DockControl && !AllowDispose)
				throw new ApplicationException("You can't remove BarDockControl control");
		}
		void OnComponentSelectionChanged(object sender, EventArgs e) {
			CheckDockControls();
		}
#if DXWhidbey
		protected virtual void OnBaseDebuggingStateChanged() {
			base.OnDebuggingStateChanged();	
		}
		protected override void OnDebuggingStateChanged() {
		}
#endif
		protected override void PostFilterProperties(IDictionary properties) {
			base.PostFilterProperties(properties);
			DXPropertyDescriptor.ConvertDescriptors(properties, null);
		}
		protected virtual void ConvertDescriptors(IDictionary properties) {
			DXPropertyDescriptor.ConvertDescriptors(properties, null);
		}
		protected virtual SelectionRules GetBaseSelectionRules() { return base.SelectionRules; }
		protected virtual DesignerVerbCollection GetBaseVerbs() { return base.Verbs; }
		protected virtual bool GetBaseEnableDragRect() { return base.EnableDragRect; }
		protected virtual void OnBaseContextMenu(int x, int y) { base.OnContextMenu(x, y); }
		public override SelectionRules SelectionRules { get { return SelectionRules.Locked; } }
		public override DesignerVerbCollection Verbs { get { return null; } }
		protected override bool EnableDragRect { get { return false; } }
		protected override void OnContextMenu(int x,int y) {  }
		protected virtual void DoBaseDefaultAction() { base.DoDefaultAction(); }
		public override void DoDefaultAction() { }
		protected virtual bool GetBaseHitTest(Point pt) { return base.GetHitTest(pt); }
		protected override bool GetHitTest(Point point) {
			if(DebuggingState) return false;
			if(Manager == null || Manager.IsDesignMode) return true;
			return false;
		}
		ISelectionService selectionServiceCore = null;
		protected ISelectionService SelectionService {
			get {
				if(selectionServiceCore == null) {
					selectionServiceCore = GetService(typeof(ISelectionService)) as ISelectionService;
				}
				return selectionServiceCore;
			}
		}
		protected override bool CanUseComponentSmartTags {
			get { return true; }
		}
		protected virtual void CheckDockControls() {
			IComponent component = SelectionService.PrimarySelection as IComponent;
			if(component == null)
				return;
			BarItem item = component as BarItem;
			if(item == null) {
				ResetBarDockRegions();
			}
		}
		protected override void OnComponentSmartTagChangedCore(Control owner, Rectangle glyphBounds) {
			base.OnComponentSmartTagChangedCore(owner, glyphBounds);
			if(ShouldUseGlyphRegion(owner)) ApplyGlyphRegion(glyphBounds);
		}
		protected virtual bool ShouldUseGlyphRegion(Control owner) {
			if(owner == null) return false;
			BarDockControl dc = owner.Parent as BarDockControl;
			if(dc == null) return false;
			return object.ReferenceEquals(DockControl, dc);
		}
		protected virtual void ApplyGlyphRegion(Rectangle glyphBounds) {
			ResetBarDockRegions();
			DockControl.Region = GetGlyphRegion(glyphBounds);
		}
		protected void ResetBarDockRegions() {
			if(DockControl == null || DockControl.Manager == null) return;
			foreach(BarDockControl dc in DockControl.Manager.DockControls) {
				dc.Region = null;
			}
		}
		protected virtual Region GetGlyphRegion(Rectangle glyphBounds) {
			Region rgn = new Region(new Rectangle(Point.Empty, DockControl.Size));
			rgn.Exclude(glyphBounds);
			return rgn;
		}
		protected virtual void DoBaseWndProc(ref Message m) {
			base.WndProc(ref m);
		}
		protected override void WndProc(ref Message m) {
			if(m.Msg == 0x007B ) {
				return;
			}
			DoBaseWndProc(ref m);
		}
#if !DXWhidbey
	}
#else
		protected virtual ControlBodyGlyph GetBaseControlGlyph(GlyphSelectionType selectionType) {
			return base.GetControlGlyph(selectionType);
		}
		protected override ControlBodyGlyph GetControlGlyph(GlyphSelectionType selectionType) {
			return new BarDockControlGlyph(this, Manager, Control, BehaviorService.ControlRectInAdornerWindow(Control));
		}
		protected internal bool IsDebuggingState { get { return DebuggingState; } }
	}
	public class BarDockControlGlyph : ControlBodyGlyph {
		BarManager manager;
		BarDockControlDesigner designer;
		public BarDockControlGlyph(BarDockControlDesigner designer, BarManager manager, Control owner, Rectangle bounds)
			: base(bounds, Cursor.Current, owner, new BarDockControlBehavior(designer, owner)) {
			this.designer = designer;
			this.manager = manager;
		}
		public override Cursor GetHitTest(Point p) {
			if(designer.IsDebuggingState) return Cursors.Default;
			if(manager == null || manager.Form == null || designer.Control == null || !designer.Control.Visible || !manager.IsDragging) return null;
			if(this.Bounds.Contains(p)) return Cursors.Default;
			return null;
		}
	}
	internal class DragDropControlBehavior : System.Windows.Forms.Design.Behavior.Behavior {
		Control owner;
		public DragDropControlBehavior(Control owner) {
			this.owner = owner;
		}
		protected Control Owner { get { return owner; } }
		protected virtual Control GetControl(int x, int y) {
			Point pt = Owner.PointToClient(new Point(x, y));
			foreach(Control ctrl in owner.Controls) {
				if(ctrl.Bounds.Contains(pt)) return ctrl;
			}
			return null;
		}
		protected virtual bool ShouldProcessDragEvent(DragEventArgs e) { return true; }
		MethodInfo GetMethod(string name) {
			return typeof(Control).GetMethod(name, BindingFlags.NonPublic | BindingFlags.Instance);
		}
		void Invoke(string name, int x, int y, object args) {
			Control ctrl = GetControl(x, y);
			if(ctrl == null) return;
			MethodInfo mi = GetMethod(name);
			if(mi != null) mi.Invoke(ctrl, new object[] { args });
		}
		bool shouldProcessDragEvent = true;
		public override void OnDragEnter(Glyph g, DragEventArgs e) {
			this.shouldProcessDragEvent = ShouldProcessDragEvent(e);
			if(this.shouldProcessDragEvent) {
				e.Effect = DragDropEffects.All;
				Invoke("OnDragEnter", e.X, e.Y, e);
			}
			else
				DoBaseDragEnter(g, e);
		}
		protected virtual void DoBaseDragEnter(Glyph g, DragEventArgs e) {
			base.OnDragEnter(g, e);
		}
		public override void OnDragOver(Glyph g, DragEventArgs e) {
			if(this.shouldProcessDragEvent) {
				Invoke("OnDragOver", e.X, e.Y, e);
			}
			else
				DoBaseDragOver(g, e);
		}
		protected virtual void DoBaseDragOver(Glyph g, DragEventArgs e) {
			base.OnDragOver(g, e);
		}
		public override void OnDragLeave(Glyph g, EventArgs e) {
			if(this.shouldProcessDragEvent) {
				Invoke("OnDragLeave", Control.MousePosition.X, Control.MousePosition.Y, e);
			}
			else
				DoBaseDragLeave(g, e);
		}
		protected virtual void DoBaseDragLeave(Glyph g, EventArgs e) {
			base.OnDragLeave(g, e);
		}
		public override void OnDragDrop(Glyph g, DragEventArgs e) {
			if(this.shouldProcessDragEvent) {
				Invoke("OnDragDrop", e.X, e.Y, e);
			}
			else
				DoBaseDragDrop(g, e);
		}
		protected virtual void DoBaseDragDrop(Glyph g, DragEventArgs e) {
			base.OnDragDrop(g, e);
		}
		public override void OnGiveFeedback(Glyph g, GiveFeedbackEventArgs e) {
			if(this.shouldProcessDragEvent) {
				Invoke("OnGiveFeedback", Control.MousePosition.X, Control.MousePosition.Y, e);
			}
			else
				DoBaseGiveFeedback(g, e);
		}
		protected virtual void DoBaseGiveFeedback(Glyph g, GiveFeedbackEventArgs e) {
			base.OnGiveFeedback(g, e);
		}
	}
	internal class BarDockControlBehavior : DragDropControlBehavior {
		BarDockControlDesigner designer;
		public BarDockControlBehavior(BarDockControlDesigner designer, Control owner) : base(owner) {
			this.designer = designer;
		}
		protected override Control GetControl(int x, int y) {
			if(designer.IsDebuggingState) return null;
			return base.GetControl(x, y);
		}
	}
#endif
	[CLSCompliant(false)]
	public class RepositoryEditEditor : EditFieldEditor {
		protected override RepositoryItemCollection[] GetRepository(ITypeDescriptorContext context) {
			if(context == null) return null;
			BarEditItem item = context.Instance as BarEditItem;
			if(item == null || item.Manager == null) return null;
			RepositoryItemCollection[] res = null;
			if(item.Manager.ExternalRepository != null) {
				res = new RepositoryItemCollection[2];
				res[1] = item.Manager.ExternalRepository.Items;
			} else {
				res = new RepositoryItemCollection[1];
			}
			res[0] = item.Manager.RepositoryItems;
			return res;
		}
	}
}
