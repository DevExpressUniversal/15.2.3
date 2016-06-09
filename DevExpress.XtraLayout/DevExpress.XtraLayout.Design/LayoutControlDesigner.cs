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
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.Design.Behavior;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Design;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.XtraDataLayout.DesignTime;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout.Customization;
using DevExpress.XtraLayout.Customization.Templates;
using DevExpress.XtraLayout.Dragging;
using DevExpress.XtraLayout.Handlers;
using DevExpress.XtraLayout.Helpers;
using Microsoft.CSharp;
namespace DevExpress.XtraLayout.DesignTime {
	public class BindingInfo {
		Control label, bControl;
		public BindingInfo(Control label, Control control) { this.label = label; this.bControl = control; }
		public Control Label {
			get { return label; }
			set { label = value; }
		}
		public Control Control {
			get { return bControl; }
			set { bControl = value; }
		}
	}
	public class Vs2005DataSourceDragHelper :IDisposable {
		List<BindingInfo> bInfo;
		Size sizeBeforePatch = Size.Empty;
		Timer timer;
		bool killing = false;
		LayoutControlDesigner designerCore;
		public Vs2005DataSourceDragHelper(LayoutControlDesigner designer) {
			designerCore = designer;
			timer = new Timer();
			timer.Interval = 500;
			timer.Tick += new EventHandler(timer_Tick);
			bInfo = new List<BindingInfo>();
		}
		void timer_Tick(object sender, EventArgs e) {
			Reset();
		}
		protected void Reset() {
			sizeBeforePatch = Size.Empty;
			bInfo.Clear();
			timer.Stop();
		}
		public void SelectionChanged(ICollection collection) {
			if(killing) return;
			if(bInfo.Count != 0 && collection.Count >= 2) {
				ArrayList list = new ArrayList(collection);
				killing = true;
				bool needResize = false;
				try {
					foreach(BindingInfo info in bInfo) {
						if(list.Contains(info.Label) && list.Contains(info.Control)) {
							this.designerCore.KillLabel(info.Label, info.Control);
							needResize = true;
						}
					}
				} finally {
					if(needResize) this.designerCore.Component.Size = sizeBeforePatch;
					Reset();
					killing = false;
				}
			}
		}
		public void AddControl(Control control) {
			if(control is Label) {
				bInfo.Add(new BindingInfo(control, null));
				timer.Start();
			} else {
				if(bInfo.Count != 0) {
					bInfo[bInfo.Count - 1].Control = control;
					if(sizeBeforePatch == Size.Empty) sizeBeforePatch = designerCore.Component.Size;
				} else
					Reset();
			}
		}
		public void Dispose() {
			timer.Tick -= new EventHandler(timer_Tick);
		}
	}
	public class LayoutControlDesigner :BaseLayoutControlDesigner {
		Point lastGetHittestPoint = Point.Empty;
		Vs2005DataSourceDragHelper dsHelper;
		bool selectionChanging = false;
		ArrayList associatedComponentsCollection;
		protected VisualizersBehavior visualizersBehaviourCore;
		protected DragBehavior dragBehaviourCore;
		protected RightButtonBehavior rightButtonBehaviourCore;
		protected ResizeEmptyPaddingBehaviour resizeEmptyPaddingBehaviour;
		protected override void Dispose(bool disposing) {
			if(disposing) {
				MenuCommandManager.UnregisterClient(this);
				UnSubscribeEvents();
			}
			base.Dispose(disposing);
		}
		public override ICollection AssociatedComponents {
			get {
				if(associatedComponentsCollection == null) {
					associatedComponentsCollection = new ArrayList();
					associatedComponentsCollection.AddRange(Component.Items);
					associatedComponentsCollection.AddRange(Component.Controls);
					foreach(Control tControl in Component.Controls)
						if(((ILayoutDesignerMethods)Component).IsInternalControl(tControl))
							associatedComponentsCollection.Remove(tControl);
				}
				return associatedComponentsCollection;
			}
		}
#if DXWhidbey
		ResizeBehavior behaviour;
		LayoutGrabHandleGlyph[] glyphs = new LayoutGrabHandleGlyph[8];
		public BehaviorService BehaviorServiceCore {
			get {
				return BehaviorService;
			}
		}
		public override void InitializeNewComponent(IDictionary defaultValues) {
			(Component as ILayoutControl).FireChanging(Component);
			Component.BeginUpdate();
			base.InitializeNewComponent(defaultValues);
			ArrayList list = new ArrayList(Component.Controls);
			foreach(Control control in list) {
				if(((ILayoutDesignerMethods)Component).IsInternalControl(control)) continue;
				control.Parent = Component.Parent;
				Point p = control.Location;
				p.Offset(Component.Location);
				control.Location = p;
			}
			Component.EndUpdate();
			(Component as ILayoutControl).SetControlDefaultsLast();
			(Component as ILayoutControl).FireChanged(Component);
		}
		public override bool ParticipatesWithSnapLines {
			get {
				return false;
			}
		}
		public override IList SnapLines {
			get {
				ArrayList list1 = (ArrayList)base.SnapLines;
				ArrayList list2 = new ArrayList(4);
				foreach(SnapLine line1 in list1) {
					if((line1.Filter != null) && line1.Filter.Contains("Padding")) {
						list2.Add(line1);
					}
				}
				foreach(SnapLine line2 in list2) {
					list1.Remove(line2);
				}
				return list1;
			}
		}
		private void CreateLayoutGrabHandleGlyphs(GlyphSelectionType selectionType, GlyphCollection glyphCollection) {
			if(BehaviorService != null && (selectionType == GlyphSelectionType.Selected || selectionType == GlyphSelectionType.SelectedPrimary)) {
				int i = 0;
				glyphs = new LayoutGrabHandleGlyph[8];
				foreach(LayoutGrabHandleGlyphType gtype in Enum.GetValues(typeof(LayoutGrabHandleGlyphType))) {
					LayoutGrabHandleGlyph glyph = new LayoutGrabHandleGlyph(BehaviorService, Component, gtype, behaviour, true);
					glyphs[i] = glyph;
					i++;
				}
				glyphCollection.AddRange(glyphs);
			}
		}
		public override GlyphCollection GetGlyphs(GlyphSelectionType selectionType) {
			InitializeMenuCommands();
			GlyphCollection glyphCollection = base.GetGlyphs(selectionType);
			if((((this.SelectionRules & SelectionRules.Moveable) != SelectionRules.None) && (this.InheritanceAttribute != InheritanceAttribute.InheritedReadOnly))) {
				if(selectionType != GlyphSelectionType.NotSelected) {
					ArrayList listToDelete = new ArrayList();
					foreach(object obj in glyphCollection) {
						String strType = obj.GetType().ToString();
						if(strType.Contains(".GrabHandleGlyph") || strType.Contains(".SelectionBorderGlyph")) {
							listToDelete.Add(obj);
						}
					}
					foreach(object obj in listToDelete) {
						glyphCollection.Remove((Glyph)obj);
					}
					CreateLayoutGrabHandleGlyphs(selectionType, glyphCollection);
				}
			}
			glyphCollection.AddRange(CreateSizeConstraintsAndDragWidgetGlyphs());
			return glyphCollection;
		}
		protected bool AllowShowGlyph(LayoutControlItem lci) {
			if(SelectedComponents.Contains(lci) || SelectedComponents.Contains(lci.Owner)) return true;
			return false;
		}
		protected Glyph[] CreateSizeConstraintsAndDragWidgetGlyphs() {
			ArrayList glyphs = new ArrayList();
			ConstraintsVisualizer cv = new ConstraintsVisualizer();
			Rectangle componentRect = new Rectangle(Point.Empty, Component.Size);
			if(Component.Items != null)
				foreach(BaseLayoutItem item in Component.Items) {
					AddResizeEmptuPaddingGlyph(glyphs, componentRect, item);
					LayoutControlItem lci = item as LayoutControlItem;
					if(lci != null) {
						Rectangle itemRectangle = lci.ViewInfo.ClientAreaRelativeToControl;
						if(lci.Control != null && SelectedComponents.Contains(lci.Control)) {
							DragWidgetGlyph dwg = new DragWidgetGlyph(dragBehaviourCore, itemRectangle, componentRect);
							RightButtonGlyph rbg = new RightButtonGlyph(rightButtonBehaviourCore, itemRectangle, componentRect);
							glyphs.Add(dwg);
							glyphs.Add(rbg);
						}
						if(!SelectedComponents.Contains(lci.Owner)) continue;
						Image image = cv.GetItemStateImage(lci);
						if(image != null) {
							CenterConstraintsGlyphWithImage glyph = new CenterConstraintsGlyphWithImage(
								visualizersBehaviourCore,
								itemRectangle, componentRect, image);
							glyphs.Add(glyph);
						}
					}
				}
			return (Glyph[])glyphs.ToArray(typeof(Glyph));
		}
		private void AddResizeEmptuPaddingGlyph(ArrayList glyphs, Rectangle componentRect, BaseLayoutItem item) {
			if(item.IsHidden) return;
			if(item.Parent == null) return;
			if(item.Padding.Left <= 0) {
				Rectangle rect = new Rectangle(item.ViewInfo.BoundsRelativeToControl.Location, new Size(ResizeEmptyPaddingGlyph.ResizeAreaTickness, item.Height));
				ResizeEmptyPaddingGlyph repg = new ResizeEmptyPaddingGlyph(resizeEmptyPaddingBehaviour, rect, componentRect);
				glyphs.Add(repg);
			}
			if(item.Padding.Right <= 0) {
				Rectangle rect = new Rectangle(new Point(item.ViewInfo.BoundsRelativeToControl.Right - ResizeEmptyPaddingGlyph.ResizeAreaTickness, item.ViewInfo.BoundsRelativeToControl.Location.Y), new Size(ResizeEmptyPaddingGlyph.ResizeAreaTickness, item.Height));
				ResizeEmptyPaddingGlyph repg = new ResizeEmptyPaddingGlyph(resizeEmptyPaddingBehaviour, rect, componentRect);
				glyphs.Add(repg);
			}
			if(item.Padding.Top <= 0) {
				Rectangle rect = new Rectangle(item.ViewInfo.BoundsRelativeToControl.Location, new Size(item.Width, ResizeEmptyPaddingGlyph.ResizeAreaTickness));
				ResizeEmptyPaddingGlyph repg = new ResizeEmptyPaddingGlyph(resizeEmptyPaddingBehaviour, rect, componentRect);
				glyphs.Add(repg);
			}
			if(item.Padding.Bottom <= 0) {
				Rectangle rect = new Rectangle(new Point(item.ViewInfo.BoundsRelativeToControl.Location.X, item.ViewInfo.BoundsRelativeToControl.Bottom - ResizeEmptyPaddingGlyph.ResizeAreaTickness), new Size(item.Width, ResizeEmptyPaddingGlyph.ResizeAreaTickness));
				ResizeEmptyPaddingGlyph repg = new ResizeEmptyPaddingGlyph(resizeEmptyPaddingBehaviour, rect, componentRect);
				glyphs.Add(repg);
			}
		}
#else
		public override void OnSetComponentDefaults() {
			(Component as ILayoutControl).FireChanging(Component);
			base.OnSetComponentDefaults();
			(Component as ILayoutControl).SetControlDefaultsLast();
			(Component as ILayoutControl).FireChanged(Component);
		}
#endif
		protected bool AllowEdit { get { return !(Inherited && !AllowEditInherited); } }
		protected override bool AllowEditInherited { get { return true; } }
		bool allowSelectItems = true;
		protected override void SelectedComponentsChanged(object sender, EventArgs ea) {
			if(Component == null || Component.Root == null) return;
			if(Component.Capture) Component.Capture = false;
			MenuCommandManager.SetLastClient(this);
			ICollection col = SelectedComponents;
			if(col.Count > 0) {
				if(((ArrayList)col).Contains(Control)) {
					allowSelectItems = false;
					Component.Root.ClearSelection();
					allowSelectItems = true;
					return;
				}
				allowSelectItems = false;
				Component.Root.ClearSelection();
				foreach(object obj in SelectedComponents) {
					BaseLayoutItem bli = obj as BaseLayoutItem;
					if(bli != null && !bli.Selected) {
						bli.Selected = true;
					}
				}
				allowSelectItems = true;
				dsHelper.SelectionChanged(col);
			}
		}
		protected override void SelectedComponentsChanging(object sender, EventArgs ea) { }
		ICollection GetSelectedObjects(LayoutGroup group) {
			ArrayList selectionCore = new ArrayList(RightButtonMenuManager.GetSelectedObjects(group));
			if(group.Parent == null && selectionCore.Count == 0) ((ArrayList)selectionCore).Add(Component);
			return selectionCore;
		}
		protected ICollection GetHiddenSelectedObjects() {
			ArrayList list = new ArrayList();
			return list;
		}
		protected virtual bool CheckInheritanceRestrictions(IComponent component) {
			IInheritanceService service = (IInheritanceService)this.GetService(typeof(IInheritanceService));
			InheritanceAttribute ia = InheritanceAttribute.Default;
			if(service != null) {
				ia = service.GetInheritanceAttribute(component);
			}
			if(ia == InheritanceAttribute.InheritedReadOnly) return false;
			else return true;
		}
		protected ArrayList FilterSelectedObjects(ArrayList list) {
			ArrayList result = new ArrayList();
			foreach(IComponent o in list) {
				if(CheckInheritanceRestrictions(o)) {
					result.Add(o);
				}
			}
			return result;
		}
		protected ICollection GetAllSelectedObjects(LayoutGroup group) {
			ICollection col = GetSelectedObjects(group);
			ICollection hcol = GetHiddenSelectedObjects();
			ArrayList list = new ArrayList();
			if(col != null) list.AddRange(col);
			if(hcol != null) list.AddRange(hcol);
			return FilterSelectedObjects(list);
		}
		protected void UpdateVerbsAndSelection(LayoutGroup group, object sender) {
			if(selectionChanging) return;
			if(group == null) return;
			ICollection cols = SelectedComponents;
			if(Control.ModifierKeys != Keys.Control && Control.ModifierKeys != Keys.Shift && Control.ModifierKeys != (Keys.Control | Keys.Control)) {
				foreach(object obj in cols) {
					BaseLayoutItem bli = obj as BaseLayoutItem; 
					if(bli != null && bli.Owner != Control && bli.Owner != null && bli.Owner.RootGroup != null) {
						selectionChanging = true;
						bli.Owner.RootGroup.ClearSelection();
						bli.Owner.RootGroup.Selected = false;
						selectionChanging = false;
					}
				}
			}
			ICollection col = GetAllSelectedObjects(group);
			col = FilterSelectedObjectsByParent(col, sender);
#if DXWhidbey
			SelectionService.SetSelectedComponents(col, ShouldClearSelection ? SelectionTypes.Replace : SelectionTypes.Add);
#else
			SelectionService.SetSelectedComponents(col);
#endif
		}
		private ICollection FilterSelectedObjectsByParent(ICollection col, object sender) {
			BaseLayoutItem lastSelecetedObject = sender as BaseLayoutItem;
			if(lastSelecetedObject == null) return col;
			ArrayList returnCollection = new ArrayList(col);
			foreach(var item in col) {
				BaseLayoutItem itemToAdd = item as BaseLayoutItem;
				if(itemToAdd != null && lastSelecetedObject.Parent != itemToAdd.Parent) {
					returnCollection.Remove(itemToAdd);
				}
			}
			return returnCollection;
		}
		public virtual void OnSelectionChanged(object sender, EventArgs e) {
			if(!allowSelectItems) return;
			BaseLayoutItem lastselItem = sender as BaseLayoutItem;
			if(sender is LayoutControl) {
				lastselItem = ((LayoutControl)sender).Root;
			}
			if(lastselItem != null) {
				BaseLayoutItem locSel = RightButtonMenuManager.GetLocalSelectedParent(Component);
				lastselItem = locSel == null ? lastselItem : locSel;
				if(lastselItem is LayoutControlGroup) {
					UpdateVerbsAndSelection((LayoutGroup)lastselItem, sender);
				}
				if(lastselItem is LayoutControlItem && (lastselItem.Parent != null || lastselItem.IsHidden))
					UpdateVerbsAndSelection(lastselItem.Parent, sender);
				if(lastselItem is TabbedControlGroup && (lastselItem.Parent != null || lastselItem.IsHidden))
					UpdateVerbsAndSelection(lastselItem.Parent, sender);
				if(Verbs == null || SelectedComponents.Count == 0) return;
				if(Control != null) Control.Invalidate();
			}
		}
		protected MenuCommand InitializeMenuCommand(EventHandler handler, CommandID id) {
			MenuCommand mc_old = MenuCommandService.FindCommand(id);
			if(mc_old != null) {
				MenuCommandService.RemoveCommand(mc_old);
				if(MenuCommandService.FindCommand(id) != null) return MenuCommandService.FindCommand(id);
				MenuCommand mc_new = new MenuCommand(handler, id);
				mc_new.Enabled = true;
				mc_new.Visible = true;
				mc_new.Supported = true;
				MenuCommandService.AddCommand(mc_new);
			}
			return mc_old;
		}
		protected internal void RestoreMenuCommand(MenuCommand command, CommandID id) {
			MenuCommand mc_old = MenuCommandService.FindCommand(id);
			if(mc_old != null)
				MenuCommandService.RemoveCommand(mc_old);
			if(MenuCommandService.FindCommand(id) != null) return;
			if(command != null)
				MenuCommandService.AddCommand(command);
		}
		protected void OnComponentRename(object sender, ComponentRenameEventArgs e) {
			if(Component == null) return;
			if(e.Component != null) {
				BaseLayoutItem item = e.Component as BaseLayoutItem;
				if(item != null && item.Owner == Component) {
					item.Name = e.NewName;
				}
				Control control = e.Component as Control;
				if(control != null && control.Parent == Component) {
					LayoutControlItem citem = Component.GetItemByControl(control);
					if(citem != null) {
						citem.ControlName = e.NewName;
					}
				}
			}
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			IMenuCommandService menuCommandService;
			if(DesignerHost != null) menuCommandService = MenuCommandServiceHelper.RegisterMenuCommandService(DesignerHost);
			else Debug.WriteLine("Menu commands were not initialized : DesignerHost == null");
			if(AllowEdit) InitializeMenuCommands();
			else Debug.WriteLine("Menu commands were not initialized: AllowEdit == false");
			Component.Changed += OnChanged;
			Component.Changing += OnChanging;
			Component.ItemSelectionChanged += new EventHandler(OnSelectionChanged);
			Component.ControlAdded += OnControlAdded;
			Component.ControlRemoved += OnControlRemoved;
			Component.PopupMenuShowing += OnShowContextMenu;
			ITemplateManagerImplementor itmi = Component as ITemplateManagerImplementor;
			if(itmi != null) {
				itmi.SerializeControl += OnSerializeControl;
			}
			if(Component is ILayoutDesignerMethods) {
				(Component as ILayoutDesignerMethods).DeleteSelectedItems += LayoutControlDesigner_DeleteSelectedItems;
			}
			dsHelper = new Vs2005DataSourceDragHelper(this);
			if(DesignerHost != null) {
				DesignerHost.Activated += new EventHandler(ShowDesignerElements);
				DesignerHost.Deactivated += new EventHandler(HideDesignerElements);
				DesignerHost.LoadComplete += new EventHandler(OnLoadComplete);
			}
			if(ComponentChangeService != null)
				ComponentChangeService.ComponentRename += new ComponentRenameEventHandler(OnComponentRename);
			else
				throw new Exception("Could not initialize ComponentChangeService");
#if DXWhidbey
			if(BehaviorService != null) {
				behaviour = new ResizeBehavior(this);
			}
#endif
			visualizersBehaviourCore = new VisualizersBehavior(this);
			dragBehaviourCore = new DragBehavior(this);
			rightButtonBehaviourCore = new RightButtonBehavior(this);
			resizeEmptyPaddingBehaviour = new ResizeEmptyPaddingBehaviour(this);
			((ILayoutDesignerMethods)Component).DragDropDispatcherClientHelper.Initialize();
		}
		void LayoutControlDesigner_DeleteSelectedItems(object sender, DeleteSelectedItemsEventArgs e) {
			if(e == null || e.Collection == null || e.Collection.Count == 0) return;
			object[] selectedObjects = e.Collection.ToArray();
			if(selectedObjects.Length > 0 && selectedObjects[0] != null) {
				try {
					foreach(IDisposable disposable in selectedObjects) {
						SelectionService.SetSelectedComponents(
								new object[] { disposable }, SelectionTypes.Replace
							);
						OnDeleteCore(disposable);
					}
				} catch { }
			}
		}
		protected bool IsInEditorChangeTypeTransaction {
			get {
				return DesignerHost != null && DesignerHost.InTransaction && DesignerHost.TransactionDescription != null && DesignerHost.TransactionDescription == "ChangeEditorType";
			}
		}
		protected bool IsInPropertyChangeTransaction {
			get {
				return DesignerHost != null && DesignerHost.InTransaction && DesignerHost.TransactionDescription != null && DesignerHost.TransactionDescription.StartsWith("Set ");
			}
		}
		bool disbleDesignerChanges = false;
		public void ResizeStarted() {
			OnChanging(this, new CancelEventArgs());
			disbleDesignerChanges = true;
		}
		public void ResizeFinished() {
			disbleDesignerChanges = false;
			OnChanged(this, EventArgs.Empty);
		}
		bool customizationFormVisibilityState = false;
		protected virtual void ShowDesignerElements(object sender, EventArgs e) {
			if(Component != null) {
				if(Component.CustomizationForm != null) {
					Component.CustomizationForm.Visible = customizationFormVisibilityState;
				}
			}
			MenuCommandManager.SetLastClient(this);
		}
		protected virtual void OnLoadComplete(object sender, EventArgs e) {
			if(!DebuggingState && Component.Root != null) {
				try {
					disbleDesignerChanges = true;
					CheckDTComponents();
					((ILayoutDesignerMethods)Component).ResetResizer();
					((ILayoutDesignerMethods)Component).RecreateHandle();
					Component.Refresh();
				} finally {
					disbleDesignerChanges = false;
				}
			}
		}
		protected virtual void CheckDTComponents() {
			Component.Root.Accept(new ComponentsUpdateHelper(ComponentsUpdateHelperRoles.Add, Component));
		}
		protected virtual void HideDesignerElements(object sender, EventArgs e) {
			if(Component != null) {
				if(Component.CustomizationForm != null) {
					customizationFormVisibilityState = Component.CustomizationForm.Visible;
					Component.CustomizationForm.Hide();
				}
			}
		}
		protected override void OnSetCursor() {
			if(((this.ToolboxService != null) && this.ToolboxService.SetCursor()) && !this.InheritanceAttribute.Equals(InheritanceAttribute.InheritedReadOnly)) {
				return;
			}
			Cursor.Current = Component.Cursor;
		}
		internal IComponent RootComponent{
			get{
				return DesignerHost.RootComponent;
			} 
		}
		bool initalizeMenuCommands = false;
		protected void InitializeMenuCommands() {
			if(initalizeMenuCommands || MenuCommandService.FindCommand(StandardCommands.Delete) == null) return;
			MenuCommandManager.AddOldHandler(InitializeMenuCommand(new EventHandler(MenuCommandManager.OnDelete), StandardCommands.Delete), ComandType.Delete, RootComponent);
			MenuCommandManager.AddOldHandler(InitializeMenuCommand(new EventHandler(MenuCommandManager.OnKeyCancel), MenuCommands.KeyCancel), ComandType.KeyCancel, RootComponent);
			MenuCommandManager.AddOldHandler(InitializeMenuCommand(new EventHandler(MenuCommandManager.OnCut), StandardCommands.Cut), ComandType.KeyCut, RootComponent);
			MenuCommandManager.AddOldHandler(InitializeMenuCommand(new EventHandler(MenuCommandManager.OnKeyCopy), StandardCommands.Copy), ComandType.KeyCopy, RootComponent);
			MenuCommandManager.AddOldHandler(InitializeMenuCommand(new EventHandler(MenuCommandManager.OnKeyPaste), StandardCommands.Paste), ComandType.KeyPaste, RootComponent);
			Debug.WriteLine("MenuCommands were initialized");
			MenuCommandManager.RegisterClient(this);
			initalizeMenuCommands = true;
		}
		Point lastDropPoint = Point.Empty;
		DraggingVisualizer dragVisualizer;
		protected virtual DraggingVisualizer DragBoundsVisualizer {
			get {
				if(dragVisualizer == null) {
					dragVisualizer = new DraggingVisualizer(Component);
				}
				return dragVisualizer;
			}
		}
		MouseEventArgs CreateMouseEventArgs(DragEventArgs de) {
			Point p = Point.Empty;
			if(de != null)
				p = new Point(de.X, de.Y);
			p = Component.PointToClient(p);
			return new MouseEventArgs(MouseButtons.Left, 0, p.X, p.Y, 0);
		}
		protected override void OnDragOver(DragEventArgs de) {
			FieldInfo fi = typeof(ParentControlDesigner).GetField("mouseDragTool", BindingFlags.Instance | BindingFlags.NonPublic);
			object tool = fi.GetValue(this);
			if(tool != null) {
				DragBoundsVisualizer.ProcessMessageFromDesigner(EventType.MouseMove, CreateMouseEventArgs(de));
			} else {
				if(SelectedComponents.Count > 0 && !Component.Controls.Contains((Control)SelectedComponents[0])) {
					DragBoundsVisualizer.ProcessMessageFromDesigner(EventType.MouseMove, CreateMouseEventArgs(de));
				}
			}
			base.OnDragOver(de);
		}
		protected override void OnDragEnter(DragEventArgs de) {
			DragBoundsVisualizer.ProcessMessageFromDesigner(EventType.MouseEnter, CreateMouseEventArgs(de));
			base.OnDragEnter(de);
		}
		protected override void OnDragLeave(EventArgs e) {
			DragBoundsVisualizer.ProcessMessageFromDesigner(EventType.MouseLeave, CreateMouseEventArgs(null));
			base.OnDragLeave(e);
		}
		protected override void OnDragDrop(DragEventArgs de) {
			lastDropPoint = new Point(de.X, de.Y);
			DragBoundsVisualizer.ProcessMessageFromDesigner(EventType.MouseLeave, CreateMouseEventArgs(null));
			base.OnDragDrop(de);
		}
		protected bool OnDeleteCore(IDisposable disposable) {
			if(disposable == null) return false;
			bool result = false;
			try {
				LayoutControlGroup root = disposable as LayoutControlGroup;
				if(root != null && root.Parent == null) {
					disposable = (IDisposable)root.Owner;
				}
				Control control = disposable as Control;
				Component component = disposable as Component;
				if(Component != null && disposable != Component &&
					((component != null && ((ILayoutControl)Component).Components.Contains(component)) ||
					(control != null && Component.Controls.Contains(control)))) {
					SelectionService.SetSelectedComponents(new object[] { });
					((ILayoutControl)Component).FireChanging(null);
					result = true;
					LayoutControlItem controlItem = disposable as LayoutControlItem;
					if(controlItem != null && controlItem.Control != null) {
						DesignerHost.DestroyComponent(controlItem.Control);
						DesignerHost.DestroyComponent(controlItem);
						return result;
					}
					LayoutItemContainer LIContainer = disposable as LayoutItemContainer;
					if(LIContainer != null) {
						List<BaseLayoutItem> nestedItems = new FlatItemsList().GetItemsList(LIContainer);
						for(int i = nestedItems.Count - 1; i >= 0; i--) {
							LayoutControlItem cItem = nestedItems[i] as LayoutControlItem;
							if(cItem != null && cItem.Control != null)
								DesignerHost.DestroyComponent(cItem.Control);
							DesignerHost.DestroyComponent(nestedItems[i]);
						}
						return result;
					}
					IComponent disposedComponent = disposable as IComponent;
					if(disposedComponent != null)
						DesignerHost.DestroyComponent(disposedComponent);
					else disposable.Dispose();
				} else {
					if(root != null || disposable == Component) {
						SelectionService.SetSelectedComponents(new object[] { });
						if(Component != null && Component.CustomizationForm != null)
							if(Component.CustomizationForm != null)
								Component.CustomizationForm.Close();
						SelectionService.SetSelectedComponents(new object[] { component }, SelectionTypes.Replace);
					}
					MenuCommandManager.InvokeBaseHandler(ComandType.Delete, RootComponent);
				}
			} finally {
				if(result && (ILayoutControl)Component != null) ((ILayoutControl)Component).FireChanged(null);
			}
			return result;
		}
		protected bool CanCutePaste() {
			foreach(object obj in SelectedComponents) { if(!(obj is BaseLayoutItem)) return false; }
			return true;
		}
		public void OnKeyCopy(object sender, EventArgs e) {
			OnCopyCut(sender, e, false);
		}
		private static void CheckTemplate(string fullPathToXML) {
			TemplateManager templateRestoreManager = new TemplateManager();
			LayoutControl restoreCheckLC = new LayoutControl();
			templateRestoreManager.RestoreTemplatePreview(fullPathToXML, restoreCheckLC, new LayoutItemDragController(null, restoreCheckLC.Root, Point.Empty), true);
		}
		private static void DeleteOldCopyTemplate() {
			if(Directory.Exists(TemplateString.PathToCopyTemplate)) {
				foreach(string file in Directory.GetFiles(TemplateString.PathToCopyTemplate, "*.xml", SearchOption.TopDirectoryOnly))
					File.Delete(file);
			}
		}
		static string Code(string fullPathToXML) {
			var bt = Encoding.UTF8.GetBytes(fullPathToXML);
			return Convert.ToBase64String(bt);
		}
		static string Decode(string fullPathToXML) {
			var bt = Convert.FromBase64String(fullPathToXML);
			return Encoding.UTF8.GetString(bt);
		}
		static bool IsCodeString(string fullPathToXML) {
			fullPathToXML = fullPathToXML.Trim();
			if(fullPathToXML.Length % 4 != 0) return false;
			if(!Regex.IsMatch(fullPathToXML, @"^[a-zA-Z0-9\+/]*={0,3}$")) return false;
			return true;
		}
		void InvokeBasePasteHandler() {
			SelectionService.SetSelectedComponents(new object[] { Component }, SelectionTypes.Replace);
			MenuCommandManager.InvokeBaseHandler(ComandType.KeyPaste, RootComponent);
		}
		public void OnKeyPaste(object sender, EventArgs e) {
			if(!IsCodeString(Clipboard.GetText())) {
				InvokeBasePasteHandler();
				return;
			}
			string fullPathToXML = Decode(Clipboard.GetText());
			if(!fullPathToXML.Contains(TemplateString.CopyTemplateName)) {
				InvokeBasePasteHandler();
				return;
			}
			TemplateManager tm = new TemplateManager();
			LayoutItemDragController dragController = new LayoutItemDragController(new EmptySpaceItem(), Component.Root, Utils.MoveType.Inside, Utils.InsertLocation.Before, Utils.LayoutType.Vertical);
			Cursor.Current = Cursors.WaitCursor;
			try {
				OnChanging(Component, new CancelEventArgs());
				Component.BeginUpdate();
				DisableUndoEngine();
				tm.RestoreTemplate(fullPathToXML, Component, dragController);
				SelectItemsAfterPasteTemplate();
			} finally {
				Component.EndUpdate();
				EnableUndoEngine();
				OnChanged(Component, new CancelEventArgs());
				Cursor.Current = Cursors.Default;
				tm = null;
			}
		}
		private void SelectItemsAfterPasteTemplate() {
			List<BaseLayoutItem> listSelection = new List<BaseLayoutItem>();
			Component.Root.ClearSelection();
			foreach(BaseLayoutItem item in Component.Items) {
				if(item.Name == TemplateString.LayoutGroupForRestoreName && item is LayoutGroup && item.Tag as string == TemplateString.LayoutGroupForRestoreName) {
					item.Selected = true;
					FlatItemsList FIL = new FlatItemsList();
					listSelection = FIL.GetItemsList(item);
					(item as LayoutGroup).Parent.UngroupSelected();
					break;
				}
			}
			Component.Root.UngroupSelected();
			foreach(BaseLayoutItem bli in listSelection) bli.Selected = true;
		}
		public void OnCut(object sender, EventArgs e) {
			if(CanCutePaste()) OnCopyCut(sender, e, true);
			else MessageBox.Show("Cutting individual layout items and groups is not supported. Select and cut the LayoutControl instead.");
		}
		void OnCopyCut(object sender, EventArgs e, bool isCut) {
			DeleteOldCopyTemplate();
			object[] selectedObjects = SelectedComponents.ToArray();
			if(selectedObjects == null || selectedObjects.Length == 0) return;
			List<BaseLayoutItem> listItems = new List<BaseLayoutItem>();
			foreach(object obj in selectedObjects) {
				if(obj is BaseLayoutItem) listItems.Add(obj as BaseLayoutItem);
			}
			if(listItems.Count == 0) return;
			LayoutControl lControl = listItems[0].Owner as LayoutControl;
			if(lControl == null) return;
			Cursor.Current = Cursors.WaitCursor;
			TemplateManager tm = new TemplateManager();
			TemplateManagerImplementorEventArgs evArgs = new TemplateManagerImplementorEventArgs() { TemplateManager = tm };
			OnSerializeControl(null, evArgs);
			string pathToUserXml = TemplateString.PathToCopyTemplate;
			if(!Directory.Exists(pathToUserXml)) Directory.CreateDirectory(pathToUserXml);
			string fullPathToXML = TemplateMangerAskNameForm.GetUniqueFileName(pathToUserXml, TemplateString.CopyTemplateName);
			try {
				tm.CreateTemplate(TemplateString.CopyTemplateName, tm.Items, tm.ControlsInfo, fullPathToXML, false);
				CheckTemplate(fullPathToXML);
				Clipboard.SetText(Code(fullPathToXML));
				if(isCut && File.Exists(fullPathToXML)) {
					OnDelete(sender, e);
				}
			} catch {
				if(File.Exists(fullPathToXML)) File.Delete(fullPathToXML);
			} finally {
				Cursor.Current = Cursors.Default;
				tm = null;
			}
		}
		public void OnDelete(object sender, EventArgs e) {
			object[] selectedObjects = SelectedComponents.ToArray();
			if(selectedObjects.Length > 0 && selectedObjects[0] != null) {
				try {
					foreach(IDisposable disposable in selectedObjects) {
						SelectionService.SetSelectedComponents(
								new object[] { disposable }, SelectionTypes.Replace
							);
						OnDeleteCore(disposable);
					}
				} catch { }
			}
		}
		void PassControlToOldKeyCancelHandler() {
			MenuCommandManager.InvokeBaseHandler(ComandType.KeyCancel,RootComponent);
		}
		public void OnKeyCancel(object sender, EventArgs e) {
			if(Component == null) return;
			ILayoutControl icontrol = Component as ILayoutControl;
			if(icontrol != null && icontrol.ActiveHandler != null && DragDropDispatcherFactory.Default.State == DragDropDispatcherState.ClientDragging) {
				DragDropDispatcherFactory.Default.CancelAllDragOperations();
			}
			if(SelectedComponents.Count > 0 && SelectedComponents[0] != null) {
				BaseLayoutItem item = SelectedComponents[0] as BaseLayoutItem;
				if(item != null) {
					if(!Component.Items.Contains(item)) {
						PassControlToOldKeyCancelHandler();
						return;
					}
					if(item.Parent != null) {
						LayoutGroup group = item as LayoutGroup;
						Component.Root.ClearSelection();
						if(group != null && group.ParentTabbedGroup != null) {
							group.ParentTabbedGroup.Selected = true;
							SelectionService.SetSelectedComponents(new object[] { group.ParentTabbedGroup });
						} else {
							item.Parent.Selected = true;
							SelectionService.SetSelectedComponents(new object[] { item.Parent });
						}
					} else {
						Component.Root.ClearSelection();
						SelectionService.SetSelectedComponents(new object[] { Component });
					}
				} else {
					Control control = SelectedComponents[0] as Control;
					if(control != null) {
						if(Component.Controls.Contains(control)) {
							BaseLayoutItem bitem = Component.GetItemByControl(control, Component.Root);
							if(Component.Contains(control) && bitem != null) {
								Component.Root.ClearSelection();
								bitem.Selected = true;
								SelectionService.SetSelectedComponents(new object[] { bitem });
							}
						} else {
							PassControlToOldKeyCancelHandler();
						}
					} else {
						PassControlToOldKeyCancelHandler();
					}
				}
			} else {
				PassControlToOldKeyCancelHandler();
			}
		}
		protected override void UnSubscribeEvents() {
			base.UnSubscribeEvents();
			if(ComponentChangeService != null) ComponentChangeService.ComponentRename -= new ComponentRenameEventHandler(OnComponentRename);
			if(Component != null) {
				Component.Changed -= OnChanged;
				Component.Changing -= OnChanging;
				Component.ItemSelectionChanged -= OnSelectionChanged;
				Component.ControlAdded -= OnControlAdded;
				Component.ControlRemoved -= OnControlRemoved;
				Component.PopupMenuShowing -= OnShowContextMenu;
				ITemplateManagerImplementor itmi = Component as ITemplateManagerImplementor;
				if(itmi != null) {
					itmi.SerializeControl -= OnSerializeControl;
				}
				if(Component is ILayoutDesignerMethods) {
					(Component as ILayoutDesignerMethods).DeleteSelectedItems -= LayoutControlDesigner_DeleteSelectedItems;
				}
			}
			if(DesignerHost != null) {
				DesignerHost.Activated -= new EventHandler(ShowDesignerElements);
				DesignerHost.Deactivated -= new EventHandler(HideDesignerElements);
				DesignerHost.LoadComplete -= new EventHandler(OnLoadComplete);
				if(DesignerHost.Loading) {
					MenuCommandManager.UnregisterClient(this);
				}
			}
		}
		protected virtual bool CanManageUndoEngine {
			get {
				if(DesignerHost == null) return false;
				if(DesignerHost.Container == null) return false;
				return DesignerHost.Container.Components.Count > 60 || needEnableUndoEngine;
			}
		}
		DesignerTransaction temporaryTransaction;
		ICollection selection;
		internal bool isChangingInProgess = false;
		public virtual void OnChanging(object sender, CancelEventArgs e) {
			if(disbleDesignerChanges || DebuggingState) return;
			isChangingInProgess = true;
			selection = SelectedComponents;
			bool shouldCreateTransaction = !IsInPropertyChangeTransaction;
			if(CanManageUndoEngine) DisableUndoEngine();
			if(shouldCreateTransaction)
				temporaryTransaction = DesignerHost.CreateTransaction("layout control internal change");
			else {
				temporaryTransaction = null;
			}
			try {
				ComponentChangeService.OnComponentChanging(Component, null);
				PassChangeEventToChildren(true);
			} catch {
				e.Cancel = true;
				if(temporaryTransaction != null) {
					temporaryTransaction.Cancel();
					temporaryTransaction = null;
				}
				if(CanManageUndoEngine) EnableUndoEngine();
			} finally {
			}
		}
		void PassChangeEventToChildren(bool changing) {
			foreach(IComponent component in Component.Container.Components) {
				BaseLayoutItem item = component as BaseLayoutItem;
				if(item == null || item.IsDisposing) continue;
				LayoutControlItem lci = component as LayoutControlItem;
				if(lci != null && lci.Control != null && lci.Control.Disposing) continue;
				if(item.Owner == Component) {
					LayoutControlItem controlItem = component as LayoutControlItem;
					if(controlItem != null && controlItem.Control != null) {
						if(controlItem.Control.IsDisposed) continue;
						if(controlItem.Control is LayoutControl && ((LayoutControl)controlItem.Control).Items == null) continue;
					}
					if(changing)
						ComponentChangeService.OnComponentChanging(component, null);
					else
						ComponentChangeService.OnComponentChanged(component, null, null, null);
				}
			}
		}
		protected bool NeedUpdateSelection() {
			ArrayList newSelectedComponents = new ArrayList(SelectedComponents);
			if(selection.Count != newSelectedComponents.Count) return true;
			foreach(object obj in selection) {
				if(!newSelectedComponents.Contains(obj)) return true;
				BaseLayoutItem bli = obj as BaseLayoutItem;
				if(bli != null) {
					if(bli.Owner == null) return true;
					if(bli.IsHidden) return true;
				}
			}
			return false;
		}
		public virtual void OnChanged(Object sender, EventArgs e) {
			if(disbleDesignerChanges || DebuggingState) return;
			try {
				PassChangeEventToChildren(false);
				ComponentChangeService.OnComponentChanged(Component, null, null, null);
			} finally {
			}
			if(temporaryTransaction != null) {
				temporaryTransaction.Commit();
				if(selection != null && ((ArrayList)selection).Count > 0 && NeedUpdateSelection()) {
					SelectionService.SetSelectedComponents(selection);
				}
			}
			temporaryTransaction = null;
			if(CanManageUndoEngine) EnableUndoEngine();
			isChangingInProgess = false;
		}
		protected bool DriveScrolls(ref Message m) {
			if(Component != null && ((m.Msg == MouseEvents.WM_MOUSEMOVE && Control.MouseButtons != MouseButtons.None) || m.Msg == MouseEvents.WM_LBUTTONUP)) RefreshSelectionManager();
			if(Component == null ||
			   (Component as ILayoutControl) == null ||
			   (Component as ILayoutControl).Scroller == null ||
			   !(Component as ILayoutControl).Scroller.HScroll.IsHandleCreated ||
			   !(Component as ILayoutControl).Scroller.VScroll.IsHandleCreated
		   )
				return false;
			if(((Component as ILayoutControl).Scroller.HScroll.Handle == m.HWnd || (Component as ILayoutControl).Scroller.VScroll.Handle == m.HWnd)) {
				Type childWindowType = (Component as ILayoutControl).Scroller.HScroll.WindowTarget.GetType();
				PropertyInfo info = childWindowType.GetProperty("OldWindowTarget");
				IWindowTarget window;
				if((Component as ILayoutControl).Scroller.HScroll.Handle == m.HWnd)
					window = (IWindowTarget)info.GetValue((Component as ILayoutControl).Scroller.HScroll.WindowTarget, info.GetIndexParameters());
				else
					window = (IWindowTarget)info.GetValue((Component as ILayoutControl).Scroller.VScroll.WindowTarget, info.GetIndexParameters());
				window.OnMessage(ref m);
				return true;
			}
			return false;
		}
		bool needEnableUndoEngine = false;
		protected void DisableUndoEngine() {
			if(UndoEngine == null) return;
			needEnableUndoEngine = true;
			UndoEngine.Enabled = false;
		}
		protected void EnableUndoEngine() {
			if(UndoEngine == null) return;
			needEnableUndoEngine = false;
			UndoEngine.Enabled = true;
		}
		protected UndoEngine UndoEngine {
			get { return (UndoEngine)DevExpress.XtraDataLayout.LayoutCreator.GetIDesignerHost(this.Component.Site.Container).GetService(typeof(UndoEngine)); }
		}
		public bool IsUndoInProgress {
			get {
				if(UndoEngine == null) return false;
				return UndoEngine.UndoInProgress;
			}
		}
		protected bool AllowAddRemoveControlProcessing() {
			StackTrace st = new StackTrace(System.Threading.Thread.CurrentThread, false);
			if(st != null && st.FrameCount > 13) {
				StackFrame sf = st.GetFrame(13);
				if(sf != null && sf.GetMethod().Name.Contains("Deserialize")) return false;
			}
			return true;
		}
		static string GetControlName(Control control) {
			if(control == null) return string.Empty;
			if(control.Site != null && control.Site.Name != null) {
				return control.Site.Name;
			}
			return control.Name;
		}
		void OnSerializeControl(object sender, TemplateManagerImplementorEventArgs e) {
			SelectionHelper sh = new SelectionHelper();
			List<BaseLayoutItem> listBLI = sh.GetItemsList(Component.Root);
			listBLI = TemplateManager.GetSelectedItemsWithChildren(listBLI);
			(Component as ISupportImplementor).Implementor.CheckNames();
			TemplateManager templateManager = e.TemplateManager as TemplateManager;
			Hashtable list = new Hashtable();
			foreach(BaseLayoutItem item in listBLI) {
				UpdateControlName(item);
				templateManager.Items.Add(item);
				if(item is LayoutControlItem && (item as LayoutControlItem).Control != null) {
					list.Add(Guid.NewGuid(), (item as LayoutControlItem).Control);
				}
			}
			foreach(IComponent component in SelectedComponents) {
				if(CheckBaseType(component))
					list.Add(Guid.NewGuid(), component);
			}
			GetControlDataItem(templateManager, new Control(), "Fake", list.Values);
			e.TemplateManager = templateManager;
		}
		static void UpdateControlName(BaseLayoutItem item) {
			if(item is LayoutControlItem && (item as LayoutControlItem).Control != null) {
				(item as LayoutControlItem).ControlName = GetControlName((item as LayoutControlItem).Control);
			}
		}
		private bool CheckBaseType(IComponent component) {
			var rootType = component.GetType();
			if(rootType == typeof(BaseLayoutItem)) return false;
			while(rootType.BaseType != null) {
				rootType = rootType.BaseType;
				if(rootType == typeof(BaseLayoutItem)) return false;
			}
			return true;
		}
		void GetControlDataItem(TemplateManager templateManager, IComponent control, string controlName, ICollection members) {
			Dictionary<object, object> restoreList = new Dictionary<object, object>();
			SaveStyleController(members, restoreList);
			try {
				MenuCommandManager.ClearOldHandler(this,RootComponent);
				DTEHelper.GetCurrentDTE().Documents.SaveAll();
				IDesignerHost idh = GetService(typeof(IDesignerHost)) as IDesignerHost;
				DesignerSerializationManager dsm = new DesignerSerializationManager(idh);
				dsm.PreserveNames = true;
				dsm.RecycleInstances = false;
				dsm.PropertyProvider = control;
				dsm.CreateSession();
				CodeTypeDeclaration codeTypeDeclaration = new CodeTypeDeclaration();
				TypeCodeDomSerializer typeSerializer = new TypeCodeDomSerializer();
				codeTypeDeclaration = typeSerializer.Serialize(dsm, control, members);
				StringWriter strWriter = new StringWriter();
				CSharpCodeProvider cscp = new CSharpCodeProvider();
				codeTypeDeclaration = UpdateCodeTypeDeclaration(codeTypeDeclaration);
				List<string> listControlProperty = GetControlPropertyList(codeTypeDeclaration);
				cscp.GenerateCodeFromMember(codeTypeDeclaration, strWriter, new CodeGeneratorOptions() { });
				if(members.Count != 0) {
					List<string> assembly = GetAssemblies(control, codeTypeDeclaration);
					string dataString = GetDataString(strWriter.ToString(), listControlProperty, codeTypeDeclaration, controlName);
					string resource = GetResourceFile(dataString);
					templateManager.ControlsInfo = new ControlData() { Name = controlName, Data = dataString, Assembly = assembly, Resources = resource };
				}
			} finally {
				RestoreStyleController(restoreList);
			}
		}
		private string GetResourceFile(string dataString) {
			IResourceService resourceService = GetService(typeof(IResourceService)) as IResourceService;
			IResourceReader resourceReader = resourceService.GetResourceReader(System.Globalization.CultureInfo.InvariantCulture);
			ResXResourceReader resXResourceReader = resourceReader as ResXResourceReader;
			StringWriter stream = new StringWriter();
			ResXResourceWriter writer = new ResXResourceWriter(stream);
			foreach(DictionaryEntry d in resXResourceReader) {
				if(dataString.Contains(d.Key.ToString())) {
					writer.AddResource(d.Key.ToString(), d.Value);
				}
			}
			writer.Generate();
			return stream.ToString();
		}
		private static void RestoreStyleController(Dictionary<object, object> restoreList) {
			foreach(IComponent component in restoreList.Keys) {
				ISupportStyleController sc = component as ISupportStyleController;
				sc.StyleController = restoreList[component] as IStyleController;
			}
		}
		private static void SaveStyleController(ICollection members, Dictionary<object, object> restoreList) {
			foreach(IComponent component in members) {
				ISupportStyleController sc = component as ISupportStyleController;
				if(sc != null) {
					restoreList.Add(component, sc.StyleController);
					sc.StyleController = null;
				}
			}
		}
		static List<string> GetAssemblies(IComponent control, CodeTypeDeclaration ctd) {
			List<string> assemblyList = new List<string>();
			Hashtable hTable = new Hashtable();
			string typePatchedAssembly = PatchAssemblyName(control.GetType().Assembly.GetName());
			assemblyList.Add(typePatchedAssembly);
			hTable.Add(typePatchedAssembly, null);
			foreach(AssemblyName assemblyName in control.GetType().Assembly.GetReferencedAssemblies()) {
				string tempAssembly = PatchAssemblyName(assemblyName);
				assemblyList.Add(tempAssembly);
				hTable.Add(tempAssembly, null);
			}
			for(int i = 0; i < ctd.Members.Count; i++) {
				try {
					CodeMemberField cmf = ctd.Members[i] as CodeMemberField;
					if(cmf != null) AddAssembliesFromType(Type.GetType(cmf.Type.BaseType), assemblyList, hTable);
				} catch { }
			}
			assemblyList = RemoveUnnecessaryAssembly(assemblyList);
			return assemblyList;
		}
		static List<string> RemoveUnnecessaryAssembly(List<string> assemblyList) {
			for(int i = 0; i < assemblyList.Count; i++) {
				if(assemblyList[i].StartsWith(TemplateString.Nunit) || assemblyList[i].Contains(TemplateString.Designer)) {
					assemblyList.Remove(assemblyList[i]);
					i--;
				}
			}
			string layoutAssembly = PatchAssemblyName(typeof(LayoutControl).Assembly.GetName());
			if(!assemblyList.Contains(layoutAssembly))
				assemblyList.Add(layoutAssembly);
			return assemblyList;
		}
		static string PatchAssemblyName(AssemblyName assemblyName) {
			string tempString = string.Empty;
			if(assemblyName.FullName.StartsWith(TemplateString.DevExpress)) {
				tempString = assemblyName.FullName.Replace(assemblyName.Version.Major + "." + assemblyName.Version.Minor, TemplateString.ReplaceVersion);
			} else tempString = assemblyName.FullName;
			return tempString;
		}
		static void AddAssembliesFromType(Type currentType, List<string> assemblyList, Hashtable hTable) {
			if(!hTable.Contains(PatchAssemblyName(currentType.Assembly.GetName()))) {
				assemblyList.Add(PatchAssemblyName(currentType.Assembly.GetName()));
				hTable.Add(assemblyList[assemblyList.Count - 1], null);
			}
			foreach(AssemblyName assembly in currentType.Assembly.GetReferencedAssemblies()) {
				if(!hTable.Contains(PatchAssemblyName(assembly))) {
					assemblyList.Add(PatchAssemblyName(assembly));
					hTable.Add(assemblyList[assemblyList.Count - 1], null);
				}
				foreach(AssemblyName childAssembly in assembly.GetType().Assembly.GetReferencedAssemblies()) {
					if(!hTable.Contains(PatchAssemblyName(childAssembly))) {
						assemblyList.Add(PatchAssemblyName(childAssembly));
						hTable.Add(assemblyList[assemblyList.Count - 1], null);
					}
				}
			}
		}
		string GetDataString(string controlData, List<string> listControlProperty, CodeTypeDeclaration codeTypeDeclaration, string replaceControlName) {
			string returnString = controlData.Replace("this.", "target.");
			returnString = returnString.Replace(String.Format("{0}({1}", replaceControlName, codeTypeDeclaration.BaseTypes[0].BaseType), String.Format("void InitializeInstance({0}", codeTypeDeclaration.BaseTypes[0].BaseType));
			returnString = returnString.Replace("this;", "target;");
			returnString = returnString.Replace("(this", "(target");
			if(listControlProperty != null) {
				foreach(string property in listControlProperty)
					returnString = returnString.Replace(String.Format("target.{0}", property), String.Format("{0}", property));
			}
			returnString = returnString.Replace("public class  : System.Windows.Forms.Control {", "public class NewDXFakeClass : System.Windows.Forms.Control {");
			returnString = returnString.Replace("public (System.Windows.Forms.Control target) {", "public void InitializeInstance(System.Windows.Forms.Control target,string Resource) {");
			returnString = returnString.Replace("new System.ComponentModel.ComponentResourceManager(typeof(void))", "new System.ComponentModel.ComponentResourceManager(Resource)");
			returnString = returnString.Replace("System.ComponentModel.ComponentResourceManager", "DevExpress.XtraLayout.Customization.Templates.DXTemplateResourceManager");
			return returnString;
		}
		List<string> GetControlPropertyList(CodeTypeDeclaration codeTypeDeclaration) {
			List<string> list = new List<string>();
			foreach(CodeTypeMember member in codeTypeDeclaration.Members) {
				if(!(member is CodeMemberMethod))
					list.Add(member.Name);
			}
			return list;
		}
		CodeTypeDeclaration UpdateCodeTypeDeclaration(CodeTypeDeclaration codeTypeDeclaration) {
			int positionOfMember = 0;
			CodeStatementCollection codeStatementCollection = new CodeStatementCollection();
			for(int i = 0; i < codeTypeDeclaration.Members.Count; i++) {
				codeTypeDeclaration.Members[i].Attributes = MemberAttributes.Public;
				try {
					CodeMemberMethod codeMM = codeTypeDeclaration.Members[i] as CodeMemberMethod;
					if(codeMM == null) continue;
					(codeMM as CodeMemberMethod).Parameters.Add(new CodeParameterDeclarationExpression(codeTypeDeclaration.BaseTypes[0].BaseType, "target"));
					for(int j = 0; j < (codeMM as CodeMemberMethod).Statements.Count; j++) {
						if(!(codeMM.Statements[j] is CodeCommentStatement) && !(codeMM.Statements[j] is CodeAttachEventStatement) && !(codeMM.Statements[j] is CodeRemoveEventStatement))
							codeStatementCollection.Add(codeMM.Statements[j]);
					}
					positionOfMember = i;
				} catch { }
			}
			foreach(CodeStatement codeStatement in codeStatementCollection) {
				CodeAssignStatement cas = codeStatement as CodeAssignStatement;
				if(cas == null)
					continue;
				CodePropertyReferenceExpression cpre = cas.Left as CodePropertyReferenceExpression;
				if(cpre == null)
					continue;
				CodeThisReferenceExpression thisRef = cpre.TargetObject as CodeThisReferenceExpression;
				if(thisRef == null)
					continue;
				cpre.TargetObject = new CodePropertyReferenceExpression(null, "target");
				cas = codeStatement as CodeAssignStatement;
				if(cas == null)
					continue;
				cpre = cas.Right as CodePropertyReferenceExpression;
				if(cpre == null)
					continue;
				thisRef = cpre.TargetObject as CodeThisReferenceExpression;
				if(thisRef == null)
					continue;
				cpre.TargetObject = new CodePropertyReferenceExpression(null, "target");
			}
			(codeTypeDeclaration.Members[positionOfMember] as CodeMemberMethod).Statements.Clear();
			(codeTypeDeclaration.Members[positionOfMember] as CodeMemberMethod).Statements.AddRange(codeStatementCollection);
			return codeTypeDeclaration;
		}
		public void OnShowContextMenu(object sender, PopupMenuShowingEventArgs e) {
			if(DebuggingState) e.Allow = false;
		}
		LayoutControlItem transControlItem = null;
		public void OnControlRemoved(object sender, ControlEventArgs ea) {
			if(!((ILayoutDesignerMethods)Component).AllowHandleControlRemovedEvent) return;
			if(!AllowEdit) return;
			LayoutControlItem item = (LayoutControlItem)Component.GetItemByControl(ea.Control);
			if(item != null) {
				if(item.IsDisposing) return;
				if(IsUndoInProgress) return;
				if(!AllowAddRemoveControlProcessing()) return;
				if(IsInEditorChangeTypeTransaction) {
					((ILayoutDesignerMethods)Component).BeginChangeUpdate();
					item.Control = null;
					transControlItem = item;
					return;
				}
				((ILayoutDesignerMethods)Component).BeginChangeUpdate();
				item.Control = null;
				if(item.Parent != null) item.Parent.Remove(item);
				((ILayoutDesignerMethods)Component).EndChangeUpdate();
			}
		}
		public void KillLabel(Control label, Control control) {
			if(label == null || control == null) return;
			BaseLayoutItem itemControl = Component.GetItemByControl(control);
			if(itemControl != null) {
				itemControl.Text = label.Text;
				((ILayoutControl)Component).ConstraintsManager.UpdateIResizableConstraints(control);
			}
			BaseLayoutItem itemLabel = Component.GetItemByControl(label);
			if(itemLabel != null && itemLabel.Parent != null) { label.Dispose(); }
		}
		protected bool AllowCreataLayoutItemForControl(Control control) {
			if(control is Splitter) return false;
			if(control is PopupContainerControl) return false;
			if(control is DevExpress.Utils.Menu.IDXDropDownControl) return false;
			return true;
		}
		public void OnControlAdded(object sender, ControlEventArgs ea) {
			if(!AllowEdit) return;
			if(IsUndoInProgress) return;
			if(!AllowAddRemoveControlProcessing()) return;
			if(!AllowCreataLayoutItemForControl(ea.Control)) return;
			if(transControlItem != null) {
				transControlItem.Control = ea.Control;
				transControlItem = null;
				((ILayoutDesignerMethods)Component).EndChangeUpdate();
				return;
			}
			if(Component.IsUpdateLocked) return;
			((ILayoutDesignerMethods)Component).BeginChangeUpdate();
			LayoutControlItem item = null;
			if(lastDropPoint == Point.Empty) {
				item = (LayoutControlItem)Component.Root.AddItem();
			} else {
				item = (LayoutControlItem)Component.CreateLayoutItem(null);
				Point controlPoint = Component.PointToClient(lastDropPoint);
				LayoutItemDragController controller = new LayoutItemDragController(item, Component.Root, controlPoint);
				controller.DragWildItem();
				lastDropPoint = Point.Empty;
			}
			item.Control = ea.Control;
			dsHelper.AddControl(ea.Control);
			if((Control.ModifierKeys & Keys.Control) != 0) item.TextVisible = false;
			if((Control.ModifierKeys & Keys.Shift) != 0) item.TextVisible = true;
			((ILayoutDesignerMethods)Component).EndChangeUpdate();
		}
#if DXWhidbey
		protected override void OnDebuggingStateChanged() {
			base.OnDebuggingStateChanged();
			((ILayoutDesignerMethods)Component).AllowHandleEvents = !DebuggingState;
		}
#endif
		protected override bool GetHitTest(Point point) {
			ToolboxItem item = null;
			try { item = ToolBoxService.GetSelectedToolboxItem(); } catch { } finally { }
			lastGetHittestPoint = point;
			if(item != null && item.TypeName != null) return false; else return true;
		}
		protected override void WndProc(ref Message m) {
			if(!AllowEdit) {
				if(m.Msg == MouseEvents.WM_LBUTTONDOWN ||
				   m.Msg == MouseEvents.WM_LBUTTONUP ||
				   m.Msg == MouseEvents.WM_RBUTTONDOWN ||
				   m.Msg == MouseEvents.WM_RBUTTONUP ||
				   m.Msg == MouseEvents.WM_MOUSEMOVE ||
				   m.Msg == MouseEvents.WM_MOUSELEAVE)
					return;
				else
					base.WndProc(ref m);
			}
			if(!AllowDesigner || Control == null || DebuggingState) {
				base.WndProc(ref m);
				return;
			}
			int num1 = m.Msg;
			if((Component as ILayoutControl).DisposingFlag) { base.WndProc(ref m); return; }
			if(DriveScrolls(ref m)) return;
			if(num1 == MouseEvents.WM_MOUSELEAVE) {
				MethodInfo mi = Component.GetType().GetMethod("OnMouseLeaveCore", BindingFlags.Instance | BindingFlags.NonPublic);
				if(mi != null) mi.Invoke(Component, new object[] { EventArgs.Empty });
			}
			if(Component.Capture && m.Msg == MSG.WM_LBUTTONUP) {
				Message fakeMessageNCHitTest = m;
				fakeMessageNCHitTest.Msg = 132;
				base.WndProc(ref fakeMessageNCHitTest);
			}
			base.WndProc(ref m);
		}
		protected override bool EnableDragRect {
			get { return !GetHitTest(lastGetHittestPoint); }
		}
		internal bool ShouldClearSelection = true;
	}
	public class DragWidgetGlyph :BindingGlyph {
		public DragWidgetGlyph(VisualizersBehavior behavior, Rectangle rect, Rectangle componentRect)
			: base(behavior, rect, componentRect, "*") {
		}
		public override void Paint(PaintEventArgs pe) {
			pe.Graphics.FillRectangle(Brushes.Black, itemBounds);
			pe.Graphics.DrawImage(this.MoveGlyph, itemBounds);
		}
		private Bitmap glyph;
		private Bitmap MoveGlyph {
			get {
				if(this.glyph == null) {
					this.glyph = ResourceImageHelper.CreateBitmapFromResources(@"DevExpress.XtraLayout.Design.Images.Drag-Indicator.png", Assembly.GetExecutingAssembly());
					this.glyph.MakeTransparent();
				}
				return this.glyph;
			}
		}
		protected override void CalculateBounds() {
			inflateH = 2;
			inflateW = 3;
			itemBounds = ConvertToScreen(itemBounds);
			Size intImageSize = MoveGlyph.Size;
			if(itemBounds.Width > (intImageSize.Width + inflateW * 2))
				itemBounds.X += inflateW;
			itemBounds.Y += itemBounds.Height > (intImageSize.Height * 2) ? intImageSize.Height + inflateH : inflateH;
			itemBounds.Width = intImageSize.Width;
			itemBounds.Height = intImageSize.Height;
			componentBounds = ConvertToScreen(componentBounds);
			if(!componentBounds.Contains(itemBounds)) itemBounds = Rectangle.Empty;
		}
		public override Cursor GetHitTest(Point p) {
			if(itemBounds.Contains(p))
				return Cursors.SizeAll;
			return null;
		}
	}
	public class ResizeEmptyPaddingGlyph :BindingGlyph {
		public ResizeEmptyPaddingGlyph(VisualizersBehavior behavior, Rectangle rect, Rectangle componentRect)
			: base(behavior, rect, componentRect, "*") {
		}
		public override void Paint(PaintEventArgs pe) {
		}
		protected override void CalculateBounds() {
			itemBounds = ConvertToScreen(itemBounds);
		}
		public override Cursor GetHitTest(Point p) {
			if(itemBounds.Contains(p)) {
				if(itemBounds.Width == ResizeAreaTickness) {
					return Cursors.SizeWE;
				}
				if(itemBounds.Height == ResizeAreaTickness) {
					return Cursors.SizeNS;
				}
			}
			return null;
		}
		public static int ResizeAreaTickness { get { return 3; } }
	}
	public class RightButtonGlyph :BindingGlyph {
		public RightButtonGlyph(VisualizersBehavior behavior, Rectangle rect, Rectangle componentRect)
			: base(behavior, rect, componentRect, "*") {
		}
		public override void Paint(PaintEventArgs pe) {
			pe.Graphics.FillRectangle(Brushes.Black, itemBounds);
			pe.Graphics.DrawImage(this.MoveGlyph, itemBounds);
		}
		private Bitmap glyph;
		private Bitmap MoveGlyph {
			get {
				if(this.glyph == null) {
					this.glyph = ResourceImageHelper.CreateBitmapFromResources(@"DevExpress.XtraLayout.Design.Images.Drop-Down.png", Assembly.GetExecutingAssembly());
					this.glyph.MakeTransparent();
				}
				return this.glyph;
			}
		}
		protected override void CalculateBounds() {
			inflateH = 2;
			inflateW = 20;
			itemBounds = ConvertToScreen(itemBounds);
			Size intImageSize = MoveGlyph.Size;
			if(itemBounds.Width > (intImageSize.Width + inflateW * 2))
				itemBounds.X += inflateW;
			itemBounds.Y += itemBounds.Height > (intImageSize.Height * 2) ? intImageSize.Height + inflateH : inflateH;
			itemBounds.Width = intImageSize.Width;
			itemBounds.Height = intImageSize.Height;
			componentBounds = ConvertToScreen(componentBounds);
			if(!componentBounds.Contains(itemBounds)) itemBounds = Rectangle.Empty;
		}
		public override Cursor GetHitTest(Point p) {
			if(itemBounds.Contains(p))
				return Cursors.Arrow;
			return null;
		}
	}
	public class BindingGlyph :Glyph {
		protected Rectangle itemBounds, textBounds, componentBounds;
		protected String caption;
		protected AppearanceObject appearance;
		protected int inflateW, inflateH;
		public BindingGlyph(VisualizersBehavior behavior, Rectangle rect, Rectangle componentRect, String caption)
			: base(behavior) {
			this.itemBounds = rect;
			this.componentBounds = componentRect;
			this.caption = caption;
			InitAppearance();
			inflateH = 0;
			inflateW = 1;
			CalculateBounds();
		}
		protected void InitAppearance() {
			appearance = new AppearanceObject();
			appearance.Font = BaseButton.DefaultFont;
			appearance.Font = new Font(appearance.Font.FontFamily, 7);
			appearance.TextOptions.HAlignment = HorzAlignment.Center;
			appearance.TextOptions.VAlignment = VertAlignment.Center;
			appearance.TextOptions.Trimming = Trimming.EllipsisCharacter;
			TuneAppearanceColors();
		}
		protected virtual void TuneAppearanceColors() {
			appearance.BackColor = Color.FromArgb(202, 228, 174);
			appearance.BorderColor = Color.FromArgb(122, 156, 85);
			appearance.ForeColor = Color.FromArgb(68, 81, 53);
		}
		protected virtual void CalculateBounds() {
			GraphicsInfo.Default.AddGraphics(null);
			itemBounds = ConvertToScreen(itemBounds);
			SizeF textSize = appearance.CalcTextSize(GraphicsInfo.Default.Graphics, caption, itemBounds.Width);
			Size intTextSize = textSize.ToSize();
			if(itemBounds.Width > (intTextSize.Width + inflateW * 2))
				itemBounds.X += itemBounds.Width - (intTextSize.Width + inflateW * 2);
			else {
				intTextSize.Width -= (inflateW * 2 + 1);
				itemBounds.X += inflateW;
			}
			itemBounds.Y += inflateH;
			itemBounds.Width = intTextSize.Width;
			itemBounds.Height = intTextSize.Height;
			textBounds = itemBounds;
			itemBounds.Inflate(inflateW, inflateH);
			componentBounds = ConvertToScreen(componentBounds);
			if(!componentBounds.Contains(itemBounds)) itemBounds = Rectangle.Empty;
			GraphicsInfo.Default.ReleaseGraphics();
		}
		protected Rectangle ConvertToScreen(Rectangle rectangle) {
			Rectangle crectangle = ((VisualizersBehavior)Behavior).OwnerRectInAdronerWindow();
			rectangle.Offset(crectangle.Location);
			return rectangle;
		}
		public override void Paint(PaintEventArgs pe) {
			if(itemBounds == Rectangle.Empty) return;
			if(!pe.ClipRectangle.IntersectsWith(itemBounds)) return;
			GraphicsInfo.Default.AddGraphics(pe.Graphics);
			appearance.DrawBackground(GraphicsInfo.Default.Cache, itemBounds);
			pe.Graphics.DrawRectangle(appearance.GetBorderPen(GraphicsInfo.Default.Cache), itemBounds);
			appearance.DrawString(GraphicsInfo.Default.Cache, caption, textBounds);
			GraphicsInfo.Default.ReleaseGraphics();
		}
		public override Cursor GetHitTest(Point p) {
			if(itemBounds.Contains(p))
				return Cursors.Arrow;
			return null;
		}
	}
	public class HConstraintsGlyph :BindingGlyph {
		public HConstraintsGlyph(VisualizersBehavior behavior, Rectangle rect, Rectangle componentRect, String caption)
			: base(behavior, rect, componentRect, caption) {
		}
		protected virtual void UpdateOffset() {
			itemBounds.Y += (itemBounds.Height - textBounds.Height);
			itemBounds.X += (itemBounds.Width / 2 - textBounds.Width / 2);
		}
		protected override void TuneAppearanceColors() {
			appearance.BackColor = Color.FromArgb(209, 0, 25);
			appearance.BorderColor = Color.FromArgb(164, 0, 20);
			appearance.ForeColor = Color.White;
		}
		protected override void CalculateBounds() {
			GraphicsInfo.Default.AddGraphics(null);
			itemBounds = ConvertToScreen(itemBounds);
			SizeF textSize = appearance.CalcTextSize(GraphicsInfo.Default.Graphics, caption, itemBounds.Width);
			Size intTextSize = textSize.ToSize();
			textBounds.Width = intTextSize.Width;
			textBounds.Height = intTextSize.Height;
			UpdateOffset();
			itemBounds.Width = intTextSize.Width;
			itemBounds.Height = intTextSize.Height;
			textBounds = itemBounds;
			itemBounds.Inflate(inflateW, inflateH);
			componentBounds = ConvertToScreen(componentBounds);
			if(!componentBounds.Contains(itemBounds)) itemBounds = Rectangle.Empty;
			GraphicsInfo.Default.ReleaseGraphics();
		}
	}
	public class CenterConstraintsGlyphWithImage :Glyph {
		Image imageCore;
		Rectangle rectCore, componentRectCore;
		public CenterConstraintsGlyphWithImage(VisualizersBehavior behavior, Rectangle rect, Rectangle componentRect, Image image)
			: base(behavior) {
			this.imageCore = image;
			this.rectCore = rect;
			this.componentRectCore = componentRect;
			CalculateBounds();
		}
		protected Rectangle ConvertToScreen(Rectangle rectangle) {
			Rectangle crectangle = ((VisualizersBehavior)Behavior).OwnerRectInAdronerWindow();
			rectangle.Offset(crectangle.Location);
			return rectangle;
		}
		public override void Paint(PaintEventArgs pe) {
			if(rectCore == Rectangle.Empty) return;
			if(imageCore == null) return;
			Rectangle rect = ((VisualizersBehavior)Behavior).OwnerRectInAdronerWindow();
			GraphicsState gs = pe.Graphics.Save();
			pe.Graphics.SetClip(rect);
			if(!pe.ClipRectangle.IntersectsWith(rectCore)) return;
			pe.Graphics.DrawImageUnscaled(imageCore, rectCore);
			pe.Graphics.Restore(gs);
		}
		public override Cursor GetHitTest(Point p) {
			if(rectCore.Contains(p))
				return Cursors.Arrow;
			return null;
		}
		protected virtual void CalculateBounds() {
			rectCore = ConvertToScreen(rectCore);
			rectCore = new Rectangle(rectCore.X + rectCore.Width / 2 - imageCore.Width / 2, rectCore.Y + rectCore.Height / 2 - imageCore.Height / 2, imageCore.Width, imageCore.Height);
		}
	}
	public class VConstraintsGlyph :HConstraintsGlyph {
		public VConstraintsGlyph(VisualizersBehavior behavior, Rectangle rect, Rectangle componentRect, String caption)
			: base(behavior, rect, componentRect, caption) {
		}
		protected override void UpdateOffset() {
			itemBounds.Y += (itemBounds.Height / 2 - textBounds.Height / 2);
		}
	}
	static class MenuCommandManager {
		static List<LayoutControlDesigner> clients = new List<LayoutControlDesigner>();
		static LayoutControlDesigner lastClient;
		#region MenuCommand
		readonly static List<OldMenuCommandContainer> oldDeleteHandlers = new List<OldMenuCommandContainer>();
		readonly static List<OldMenuCommandContainer> oldKeyCancelHandlers = new List<OldMenuCommandContainer>();
		readonly static List<OldMenuCommandContainer> oldKeyCopyHandlers = new List<OldMenuCommandContainer>();
		readonly static List<OldMenuCommandContainer> oldKeyPasteHandlers = new List<OldMenuCommandContainer>();
		readonly static List<OldMenuCommandContainer> oldKeyCutHandlers = new List<OldMenuCommandContainer>();
		static MenuCommand oldDelete, oldKeyCancel, oldKeyCopy, oldKeyPaste, oldKeyCut;
		#endregion
		public static void RegisterClient(LayoutControlDesigner lcd) {
			clients.Add(lcd);
			lastClient = lcd;
		}
		public static void UnregisterClient(LayoutControlDesigner lcd) {
			clients.Remove(lcd);
			ClearOldHandler(lcd,lcd.RootComponent);
			if(lastClient == lcd) lastClient = null;
		}
		public static LayoutControlDesigner IdentifyLayoutControlBySelectedComponents(bool deleteOrCancel = false) {
			if(clients.Count == 0) return null;
			if(lastClient == null) lastClient = clients[0];
			object[] selectedObjects = lastClient.SelectedComponents.ToArray();
			if(selectedObjects == null || selectedObjects.Length == 0) return null;
			if(deleteOrCancel && lastClient.ToolboxService.GetSelectedToolboxItem() != null) return null;
			if(selectedObjects.Length == 1 && selectedObjects[0] is LayoutControl && !deleteOrCancel) return null;
			BaseLayoutItem bli = null;
			foreach(object obj in selectedObjects) {
				if(obj is BaseLayoutItem) {
					bli = obj as BaseLayoutItem;
					if(bli.Owner == null) return null;
					return GetClient(bli.Owner as LayoutControl);
				}
				if(deleteOrCancel && GetClientByControl(obj) != null) return GetClientByControl(obj);
				if(obj is LayoutControl) return GetClient(obj as LayoutControl);
				if(obj is Control && (obj as Control).Parent is LayoutControl) continue;
				if(obj is Control) continue;
			}
			return null;
		}
		static LayoutControlDesigner GetClient(LayoutControl lControl) {
			foreach(LayoutControlDesigner client in clients) {
				if(client.Component == lControl) return client;
			}
			return null;
		}
		static LayoutControlDesigner GetClientByControl(object obj) {
			if(!(obj is Control)) return null;
			Control baseControl = obj as Control;
			LayoutControl lControl = baseControl.Parent as LayoutControl;
			if(lControl == null) return null;
			foreach(LayoutControlDesigner client in clients) {
				if(client.Component == lControl) return client;
			}
			return null;
		}
		#region EventHandlers
		internal static void OnCut(object sender, EventArgs e) {
			LayoutControlDesigner lcd = IdentifyLayoutControlBySelectedComponents();
			if(lcd != null) { lcd.OnCut(sender, e); return; }
			if(lastClient != null) { InvokeBaseHandler(ComandType.KeyCut, lastClient.RootComponent); return; }
			if(oldKeyCut != null) { oldKeyCut.Invoke(); return; }
		}
		internal static void OnKeyCopy(object sender, EventArgs e) {
			LayoutControlDesigner lcd = IdentifyLayoutControlBySelectedComponents();
			if(lcd != null) { lcd.OnKeyCopy(sender, e); return; }
			if(lastClient != null) { InvokeBaseHandler(ComandType.KeyCopy, lastClient.RootComponent); return; }
			if(oldKeyCopy != null) { oldKeyCopy.Invoke(); return; }
		}
		internal static void OnKeyPaste(object sender, EventArgs e) {
			LayoutControlDesigner lcd = IdentifyLayoutControlBySelectedComponents();
			if(lcd != null) { lcd.OnKeyPaste(sender, e); return; }
			if(lastClient != null) { InvokeBaseHandler(ComandType.KeyPaste, lastClient.RootComponent); return; }
			if(oldKeyPaste != null) { oldKeyPaste.Invoke(); return; }
		}
		internal static void OnDelete(object sender, EventArgs e) {
			LayoutControlDesigner lcd = IdentifyLayoutControlBySelectedComponents(true);
			if(lcd != null) {lcd.OnDelete(sender, e); return; }
			if(lastClient != null) { InvokeBaseHandler(ComandType.Delete, lastClient.RootComponent); return;}
			if(oldDelete != null) { oldDelete.Invoke(); return; }
		}
		internal static void OnKeyCancel(object sender, EventArgs e) {
			LayoutControlDesigner lcd = IdentifyLayoutControlBySelectedComponents(true);
			if(lcd != null) { lcd.OnKeyCancel(sender, e); return; }
			if(lastClient != null) { InvokeBaseHandler(ComandType.KeyCancel, lastClient.RootComponent); return; }
			if(oldKeyCancel != null) { oldKeyCancel.Invoke(); return; }
		}
		#endregion
		internal static void ClearOldHandler(LayoutControlDesigner lcd, IComponent rootComponent) {
			if(GetHandler(oldDeleteHandlers, rootComponent) != null) {
				lcd.RestoreMenuCommand(GetHandler(oldDeleteHandlers, rootComponent).oldHandler, StandardCommands.Delete);
				if(oldDeleteHandlers.Count == 1) oldDelete = oldDeleteHandlers[0].oldHandler;
				oldDeleteHandlers.Remove(GetHandler(oldDeleteHandlers, rootComponent));
			}
			if(GetHandler(oldKeyCancelHandlers, rootComponent) != null) {
				lcd.RestoreMenuCommand(GetHandler(oldKeyCancelHandlers, rootComponent).oldHandler, MenuCommands.KeyCancel);
				if(oldKeyCancelHandlers.Count == 1) oldKeyCancel = oldKeyCancelHandlers[0].oldHandler;
				oldKeyCancelHandlers.Remove(GetHandler(oldKeyCancelHandlers, rootComponent));
			}
			if(GetHandler(oldKeyCopyHandlers, rootComponent) != null) {
				lcd.RestoreMenuCommand(GetHandler(oldKeyCopyHandlers, rootComponent).oldHandler, StandardCommands.Copy);
				if(oldKeyCopyHandlers.Count == 1) oldKeyCopy = oldKeyCopyHandlers[0].oldHandler;
				oldKeyCopyHandlers.Remove(GetHandler(oldKeyCopyHandlers, rootComponent));
			}
			if(GetHandler(oldKeyCutHandlers, rootComponent) != null) {
				lcd.RestoreMenuCommand(GetHandler(oldKeyCutHandlers, rootComponent).oldHandler, StandardCommands.Cut);
				if(oldKeyCutHandlers.Count == 1) oldKeyCut = oldKeyCutHandlers[0].oldHandler;
				oldKeyCutHandlers.Remove(GetHandler(oldKeyCutHandlers, rootComponent));
			}
			if(GetHandler(oldKeyPasteHandlers, rootComponent) != null) {
				lcd.RestoreMenuCommand(GetHandler(oldKeyPasteHandlers, rootComponent).oldHandler, StandardCommands.Paste);
				if(oldKeyPasteHandlers.Count == 1) oldKeyPaste = oldKeyPasteHandlers[0].oldHandler;
				oldKeyPasteHandlers.Remove(GetHandler(oldKeyPasteHandlers, rootComponent));
			}
		}
		private static OldMenuCommandContainer GetHandler(List<OldMenuCommandContainer> listOldMenuCommandCotainer, IComponent rootComponent) {
			foreach(LayoutControlDesigner client in clients) {
				if(rootComponent == client.RootComponent) return null;
			}
			foreach(OldMenuCommandContainer oldMenu in listOldMenuCommandCotainer) {
				if(oldMenu.rootComponent == rootComponent) return oldMenu;
			}
			return null;
		}
		internal static void SetLastClient(LayoutControlDesigner layoutControlDesigner) {
			lastClient = layoutControlDesigner;
		}
		internal static void InvokeBaseHandler(ComandType comandType, IComponent rootComponent) {
			switch(comandType) {
				case ComandType.Delete:
					InvokeHandleCore(rootComponent, oldDeleteHandlers);
					break;
				case ComandType.KeyCancel:
					InvokeHandleCore(rootComponent, oldKeyCancelHandlers);
					break;
				case ComandType.KeyCopy:
					InvokeHandleCore(rootComponent, oldKeyCopyHandlers);
					break;
				case ComandType.KeyCut:
					InvokeHandleCore(rootComponent, oldKeyCutHandlers);
					break;
				case ComandType.KeyPaste:
					InvokeHandleCore(rootComponent, oldKeyPasteHandlers);
					break;
			}
		}
		static void InvokeHandleCore(IComponent rootComponent, List<OldMenuCommandContainer> list) {
			foreach(OldMenuCommandContainer menuComandOld in list) {
				if(menuComandOld.rootComponent == rootComponent) {
					menuComandOld.oldHandler.Invoke();
					break;
				}
			}
		}
		static void AddOldHandlerCore(MenuCommand oldHandler, IComponent rootComponent, List<OldMenuCommandContainer> list) {
			bool shouldAdd = true;
			foreach(OldMenuCommandContainer menuComandOld in list) {
				if(menuComandOld.rootComponent == rootComponent) shouldAdd = false;
			}
			if(shouldAdd) list.Add(new OldMenuCommandContainer(oldHandler, rootComponent));
		}
		internal static void AddOldHandler(MenuCommand oldHandler, ComandType comandType, IComponent rootComponent) {
			switch(comandType) {
				case ComandType.Delete:
					AddOldHandlerCore(oldHandler, rootComponent, oldDeleteHandlers);
					break;
				case ComandType.KeyCancel:
					AddOldHandlerCore(oldHandler, rootComponent, oldKeyCancelHandlers);
					break;
				case ComandType.KeyCopy:
					AddOldHandlerCore(oldHandler, rootComponent, oldKeyCopyHandlers);
					break;
				case ComandType.KeyCut:
					AddOldHandlerCore(oldHandler, rootComponent, oldKeyCutHandlers);
					break;
				case ComandType.KeyPaste:
					AddOldHandlerCore(oldHandler, rootComponent, oldKeyPasteHandlers);
					break;
			}
		}
	}
	internal class OldMenuCommandContainer {
		public OldMenuCommandContainer(MenuCommand oldHandler, IComponent rootComponent) {
			this.oldHandler = oldHandler;
			this.rootComponent = rootComponent;
		}
		internal MenuCommand oldHandler;
		internal IComponent rootComponent;
	}
	enum ComandType {
		Delete,
		KeyCancel,
		KeyCopy,
		KeyCut,
		KeyPaste
	}
}
