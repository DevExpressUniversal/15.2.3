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
using System.Drawing.Design;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.XtraBars.Navigation;
using System.Collections.Generic;
using DevExpress.XtraBars.Navigation.Frames;
using DevExpress.XtraBars.Design;
using DevExpress.Utils.Design.Internal;
namespace DevExpress.XtraBars.Navigation.Design {
	public class AccordionControlComponentDesigner : BaseComponentDesigner {
		protected override bool AllowEditInherited { get { return false; } }
	}
	public class AccordionControlElementDesigner : AccordionControlComponentDesigner {
		protected override bool AllowInheritanceWrapper { get { return true; } }
		protected AccordionControlElement Element { get { return (AccordionControlElement)Component; } }
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(new AccordionControlElementActionList(this));
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			Element.DragDrop += OnDragDrop;
		}
		void OnDragDrop(object sender, EventArgs e) {
			BaseDesignerActionListGlyphHelper.RefreshSmartPanelBounds(Component);
		}
		public override ICollection AssociatedComponents {
			get {
				ArrayList controls = new ArrayList();
				AccordionControlHelper.ForEachElement(c => { controls.Add(c); }, Element.Elements, false);
				AddBase(controls);
				return controls;
			}
		}
		void AddBase(ArrayList controls) {
			foreach(object obj in base.AssociatedComponents) {
				if(controls.Contains(obj)) continue;
				controls.Add(obj);
			}
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(Element != null) {
					Element.DragDrop -= OnDragDrop;
				}
			}
			base.Dispose(disposing);
		}
	}
	public class AccordionDesignerActionMethodItem : DesignerActionMethodItem {
		public AccordionDesignerActionMethodItem(DesignerActionList list, string memberName, string displayName, string category)
			: base(list, memberName, displayName, category) {
			this.list = list;
		}
		DesignerActionList list;
		public override void Invoke() {
			base.Invoke();
			BaseDesignerActionListGlyphHelper.RefreshSmartPanelBounds(list.Component);
		}
	}
	public class AccordionControlElementActionList : DesignerActionList {
		AccordionControlElementDesigner designer;
		public AccordionControlElementActionList(AccordionControlElementDesigner designer)
			: base(designer.Component) {
			this.designer = designer;
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection res = new DesignerActionItemCollection();
			res.Add(new DesignerActionHeaderItem("Appearance", "Appearance"));
			res.Add(new DesignerActionPropertyItem("Text", "Text", "Appearance"));
			res.Add(new DesignerActionPropertyItem("Expanded", "Expanded", "Behavior"));
			res.Add(new DesignerActionHeaderItem("Actions"));
			res.Add(new DesignerActionMethodItem(this, "AddItem", "Add Item", "Actions"));
			if(Element.Style == ElementStyle.Group)
				res.Add(new DesignerActionMethodItem(this, "AddGroup", "Add Group", "Actions"));
			else if(Element.ContentContainer == null)
				res.Add(new DesignerActionMethodItem(this, "AddContentContainer", "Add ContentContainer", "Actions"));
			res.Add(new AccordionDesignerActionMethodItem(this, "MoveTowardBeginning", "Move Toward Beginning", "Actions"));
			res.Add(new AccordionDesignerActionMethodItem(this, "MoveTowardEnd", "Move Toward End", "Actions"));
			return res;
		}
		protected void SetPropertyValue(string property, object value) {
			EditorContextHelper.SetPropertyValue(Designer, Component, property, value);
		}
		public AccordionControlElementDesigner Designer { get { return designer; } }
		IDesignerHost DesignerHost { get { return GetService(typeof(IDesignerHost)) as IDesignerHost; } }
		public AccordionControlElement Element { get { return (AccordionControlElement)Component; } }
		[DefaultValue("")]
		public string Text {
			get { return Element.Text; }
			set { SetPropertyValue("Text", value); }
		}
		public bool Expanded {
			get { return Element.Expanded; }
			set { SetPropertyValue("Expanded", value); }
		}
		public void AddItem() {
			if(DesignerHost == null) return;
			AccordionControlElement item = new AccordionControlElement(ElementStyle.Item);
			DesignerHost.Container.Add(item);
			item.Text = item.Site.Name.Replace("accordionControl", "");
			AddItemCore(item);
		}
		protected void AddItemCore(AccordionControlElement item) {
			if(Element.Style == ElementStyle.Group) {
				Element.Elements.Add(item);
				if(!Element.Expanded) {
					EditorContextHelper.SetPropertyValue(Component.Site, Element, "Expanded", true);
					BaseDesignerActionListGlyphHelper.RefreshSmartPanelContent(Component);
				}
				return;
			}
			if(Element.OwnerElement != null)
				Element.OwnerElement.Elements.Add(item);
			else if(Element.AccordionControl != null)
				Element.AccordionControl.Elements.Add(item);
		}
		public void AddGroup() {
			if(DesignerHost == null) return;
			AccordionControlElement group = new AccordionControlElement(ElementStyle.Group);
			DesignerHost.Container.Add(group);
			group.Text = group.Site.Name.Replace("accordionControl", "");
			Element.Elements.Add(group);
			if(!Element.Expanded) {
				EditorContextHelper.SetPropertyValue(Component.Site, Element, "Expanded", true);
				BaseDesignerActionListGlyphHelper.RefreshSmartPanelContent(Component);
			}
		}
		public void AddContentContainer() {
			if(DesignerHost == null) return;
			AccordionContentContainer contentContainer = new AccordionContentContainer();
			DesignerHost.Container.Add(contentContainer);
			Element.ContentContainer = contentContainer;
			if(!Element.Expanded) {
				Element.AccordionControl.LockUpdate();
				try {
					EditorContextHelper.SetPropertyValue(Component.Site, Element, "Expanded", true);
				}
				finally { Element.AccordionControl.UnlockUpdate(); }
			}
			if(Element.AccordionControl != null)
				Element.AccordionControl.Refresh();
			BaseDesignerActionListGlyphHelper.RefreshSmartPanelContent(Component);
		}
		public void MoveTowardBeginning() {
			AccordionContronDesignerHelper.MoveTowardBeginning(Element);
		}
		public void MoveTowardEnd() {
			AccordionContronDesignerHelper.MoveTowardEnd(Element);
		}
	}
	public static class AccordionContronDesignerHelper {
		public static void MoveTowardBeginning(AccordionControlElement element) {
			int index = -1;
			if(element.OwnerElement == null) {
				AccordionControlElementCollection elements = element.AccordionControl.Elements;
				index = elements.IndexOf(element);
				if(index > 0) {
					elements.Remove(element);
					elements.Insert(index - 1, element);
				}
			}
			else {
				AccordionControlElementCollection elements = element.OwnerElement.Elements;
				index = elements.IndexOf(element);
				if(index > 0) {
					elements.Remove(element);
					elements.Insert(index - 1, element);
				}
			}
			element.AccordionControl.Refresh();
		}
		public static void MoveTowardEnd(AccordionControlElement element) {
			int index = -1;
			if(element.OwnerElement == null) {
				AccordionControlElementCollection elements = element.AccordionControl.Elements;
				index = elements.IndexOf(element);
				if(index < elements.Count - 1 && index != -1) {
					elements.Remove(element);
					elements.Insert(index + 1, element);
				}
			}
			else {
				AccordionControlElementCollection elements = element.OwnerElement.Elements;
				index = elements.IndexOf(element);
				if(index < elements.Count - 1 && index != -1) {
					elements.Remove(element);
					elements.Insert(index + 1, element);
				}
			}
			element.AccordionControl.Refresh();
		}
	}
	public class AccordionControlDesigner : BaseParentControlDesigner, IAccordionSmartTagCommandsImp, IKeyCommandProcessInfo {
		AccordionControlEditorForm editor;
#if DXWhidbey
		protected override bool AllowHookDebugMode { get { return true; } }
#endif
		public AccordionControlDesigner() {
			editor = null;
		}
		protected override bool AllowInheritanceWrapper { get { return true; } }
		protected override bool UseVerbsAsActionList { get { return true; } }
		protected override bool AllowEditInherited { get { return false; } }
		IDesignerHost host;
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			this.host = GetService(typeof(IDesignerHost)) as IDesignerHost;
			LoaderPatcherService.InstallService(host);
			bool allowBonusSkins = SkinHelper.InitSkins(component.Site);
			this.keyCommandProcessHelper = new AccordionControlKeyCommandProcessHelper(this);
		}
		protected override void Dispose(bool disposing) {
			LoaderPatcherService.UnInstallService(host);
			this.host = null;
			if(disposing) {
				Editor = null;
				if(KeyCommandProcessHelper != null)
					KeyCommandProcessHelper.Dispose();
			}
			base.Dispose(disposing);
		}
		protected override void PostFilterProperties(IDictionary properties) {
			base.PostFilterProperties(properties);
			DXPropertyDescriptor.ConvertDescriptors(properties, null);
		}
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			if(AccordionControl == null || DesignerHost == null) return;
			AccordionControlElement group = new AccordionControlElement(ElementStyle.Group);
			DesignerHost.Container.Add(group);
			group.Text = group.Site.Name.Replace("accordionControl", "");
			AccordionControl.Elements.Add(group);
		}
		protected override bool DrawGrid { get { return false; } }
		protected override bool EnableDragRect { get { return false; } }
		public override bool CanParent(Control control) {
			return control is AccordionContentContainer;
		}
		protected override void OnDragEnter(DragEventArgs de) {
			de.Effect = DragDropEffects.None;
		}
		protected override void OnDragOver(DragEventArgs de) {
			de.Effect = DragDropEffects.None;
		}
		protected override IComponent[] CreateToolCore(ToolboxItem tool, int x, int y, int width, int height, bool hasLocation, bool hasSize) {
			Type ownerType = GetOwnerType(tool);
			if(ownerType != null && typeof(Component).IsAssignableFrom(ownerType) && !typeof(Control).IsAssignableFrom(ownerType)) {
				return base.CreateToolCore(tool, x, y, width, height, hasLocation, hasSize);
			}
			return null;
		}
		protected Type GetOwnerType(ToolboxItem tool) {
			Type res = null;
			try {
				res = tool.GetType(DesignerHost);
			}
			catch { }
			return res;
		}
		protected virtual bool GetHitTestCore(Point client) {
			AccordionControlHitInfo hInfo = AccordionControl.CalcHitInfo(client);
			if(hInfo.HitTest == AccordionControlHitTest.Group || hInfo.HitTest == AccordionControlHitTest.Item || hInfo.HitTest == AccordionControlHitTest.ScrollBar)
				return true;
			return false;
		}
		protected override bool GetHitTest(Point point) {
			bool res = base.GetHitTest(point);
			if(!AllowDesigner || DebuggingState) return res;
			if(AccordionControl == null || res) return res;
			Point client = AccordionControl.PointToClient(point);
			return GetHitTestCore(client);
		}
		protected AccordionControlEditorForm Editor {
			get { return editor; }
			set {
				if(Editor == value) return;
				if(Editor != null) Editor.Dispose();
				editor = value;
			}
		}
		public void RunDesigner() {
			if(AccordionControl == null) return;
			Editor = new AccordionControlEditorForm();
			editor.InitEditingObject(AccordionControl);
			Editor.ShowDialog();
			Editor = null;
		}
		protected AccordionControl AccordionControl { get { return Control as AccordionControl; } }
		IDesignerHost DesignerHost { get { return GetService(typeof(IDesignerHost)) as IDesignerHost; } }
		DesignerActionListCollection smartTagCore = null;
		public override DesignerActionListCollection ActionLists {
			get {
				if(smartTagCore == null) {
					smartTagCore = new DesignerActionListCollection();
					smartTagCore.Add(new AccordionControlDesignerActionList(Component, this));
					DXSmartTagsHelper.CreateDefaultLinks(this, smartTagCore);
				}
				return smartTagCore;
			}
		}
		public override ICollection AssociatedComponents {
			get {
				if(AccordionControl == null) return base.AssociatedComponents;
				ArrayList controls = new ArrayList();
				AccordionControlHelper.ForEachElement(c => { controls.Add(c); }, AccordionControl.Elements, false);
				AddBase(controls);
				return controls;
			}
		}
		void AddBase(ArrayList controls) {
			foreach(object obj in base.AssociatedComponents) {
				if(controls.Contains(obj)) continue;
				controls.Add(obj);
			}
		}
		#region IAccordionSmartTagCommandsImp
		public void AddGroup() {
			AddElement(ElementStyle.Group);
		}
		public void AddItem() {
			AddElement(ElementStyle.Item);
		}
		protected void AddElement(ElementStyle style) {
			if(AccordionControl == null || DesignerHost == null) return;
			AccordionControlElement elem = new AccordionControlElement();
			elem.Style = style;
			AccordionControl.Elements.Add(elem);
			DesignerHost.Container.Add(elem);
			elem.Text = elem.Site.Name.Replace("accordionControl", "");
		}
		#endregion
		protected override bool CanUseComponentSmartTags {
			get { return true; }
		}
		DesignTimeKeyCommandProcessHelperBase keyCommandProcessHelper;
		public IServiceProvider ServiceProvider {
			get { return Component.Site; }
		}
		public BaseDesignTimeManager DesignTimeManager {
			get { return null; }
		}
		protected DesignTimeKeyCommandProcessHelperBase KeyCommandProcessHelper {
			get { return keyCommandProcessHelper; }
		}
	}
	public interface IAccordionSmartTagCommandsImp {
		void AddGroup();
		void AddItem();
		void RunDesigner();
	}
	public class AccordionControlDesignerActionList : DesignerActionList {
		IAccordionSmartTagCommandsImp imp;
		public AccordionControlDesignerActionList(IComponent component, IAccordionSmartTagCommandsImp imp)
			: base(component) {
			this.imp = imp;
		}
		public void AddGroup() {
			imp.AddGroup();
		}
		public void AddItem() {
			imp.AddItem();
		}
		public void RunDesigner() {
			imp.RunDesigner();
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection res = new DesignerActionItemCollection();
			res.Add(new DesignerActionMethodItem(this, "AddGroup", "Add Group", true));
			res.Add(new DesignerActionMethodItem(this, "AddItem", "Add Item", true));
			res.Add(new DesignerActionPropertyItem("Dock", "Choose Dock Style"));
			res.Add(new DesignerActionMethodItem(this, "RunDesigner", "Run Designer", true));
			return res;
		}
		public DockStyle Dock {
			get {
				if(Control == null) return DockStyle.None;
				return Control.Dock;
			}
			set {
				EditorContextHelper.SetPropertyValue(Component.Site, Control, "Dock", value);
			}
		}
		AccordionControl Control { get { return Component as AccordionControl; } }
	}
	public class AccordionContentContainerDesigner : ParentControlDesigner {
		public override SelectionRules SelectionRules {
			get {
				return SelectionRules.Visible | SelectionRules.BottomSizeable;
			}
		}
		protected override bool DrawGrid {
			get {
				if(base.DrawGrid) return true; 
				return true;
			}
		}
	}
	public class AccordionControlKeyCommandProcessHelper : DesignTimeKeyCommandProcessHelperBase {
		AccordionControlDesigner designer;
		public AccordionControlKeyCommandProcessHelper(AccordionControlDesigner designer)
			: base(designer) {
			this.designer = designer;
		}
		protected AccordionControl AccordionControl { get { return (AccordionControl)designer.Control; } }
		public override void OnKeyCancel(object sender, EventArgs e) {
			if(AccordionControl == null || AccordionControl.DesignManager == null) return;
			if(SelectedComponents.Count == 1) {
				AccordionControlElement elem = SelectedComponents[0] as AccordionControlElement;
				if(elem != null && elem.AccordionControl == AccordionControl) {
					if(elem.OwnerElement != null) {
						AccordionControl.DesignManager.SelectComponent(elem.OwnerElement);
						return;
					}
					AccordionControl.DesignManager.SelectComponent(AccordionControl);
					return;
				}
				AccordionContentContainer container = SelectedComponents[0] as AccordionContentContainer;
				if(container != null && container.Parent == AccordionControl && container.OwnerElement != null) {
					AccordionControl.DesignManager.SelectComponent(container.OwnerElement);
					return;
				}
			}
			PassControlToOldKeyCancelHandler();
		}
	}
	public class AccordionControlElementCollectionEditor : DXCollectionEditorBase {
		public AccordionControlElementCollectionEditor(Type type) : base(type) { }
		DevExpress.XtraBars.Navigation.AccordionControlElementCollection collection;
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			elements = new List<AccordionControlElement>();
			collection = value as DevExpress.XtraBars.Navigation.AccordionControlElementCollection;
			return base.EditValue(context, provider, value);
		}
		List<AccordionControlElement> elements;
		protected override bool AllowLiveUpdates {
			get { return true; }
		}
		protected override bool StandardCollectionEditorRemoveBehavior {
			get { return true; }
		}
		protected override void StandardCollectionEditorLiveUpdateRemoving(CollectionChangedEventArgs e) {
			AccordionControlElement element = e.Item as AccordionControlElement;
			if(element != null) {
				List<AccordionControlElement> elementsWithContentContainer = new List<AccordionControlElement>();
				GetElementsWithContentContainer(element, elementsWithContentContainer);
				if(elementsWithContentContainer.Count != 0 && element.AccordionControl != null) {
					foreach(var item in elementsWithContentContainer) {
						element.AccordionControl.Controls.Remove(item.ContentContainer);
						elements.Add(item);
					}
				}
			}
			base.StandardCollectionEditorLiveUpdateRemoving(e);
			if(collection == null || collection.Container == null) return;
			collection.Container.OnElementCollectionChanged();
		}
		void GetElementsWithContentContainer(AccordionControlElement element, List<AccordionControlElement> elementContainer) {
			if(element.ContentContainer != null)
				elementContainer.Add(element);
			foreach(var child in element.Elements)
				GetElementsWithContentContainer(child, elementContainer);
		}
		protected override DXCollectionEditorBase.DXCollectionEditorBaseForm CreateCollectionForm() {
			return new CollectionEditorForm(this);
		}
		void RestoreContentContainers() {
			foreach(var element in elements) {
				element.AccordionControl.Controls.Add(element.ContentContainer);
			}
		}
		class CollectionEditorForm : DXCollectionEditorBase.DXCollectionEditorBaseForm {
			AccordionControlElementCollectionEditor editor;
			public CollectionEditorForm(AccordionControlElementCollectionEditor editor)
				: base(editor) {
				this.editor = editor;
			}
			protected override void OnClosing(CancelEventArgs e) {
				base.OnClosing(e);
				if(this.DialogResult == System.Windows.Forms.DialogResult.Cancel)
					this.editor.RestoreContentContainers();
			}
		}
	}
}
