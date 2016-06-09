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

using DevExpress.Xpf.Layout.Core;
using DevExpress.Xpf.Layout.Core.Base;
namespace DevExpress.Xpf.Docking.Platform {
	class EmptyBehavior : ILayoutElementBehavior {
		public virtual bool CanDrag(OperationType operation) { return false; }
		public virtual bool CanSelect() { return false; }
		public virtual bool AllowDragging { get { return false; } }
	}
	public class DockLayoutElementBehavior : ILayoutElementBehavior {
		public IDockLayoutElement Element { get; private set; }
		public DockLayoutElementBehavior(IDockLayoutElement element) {
			AssertElement(element);
			Element = element;
		}
		protected virtual void AssertElement(IDockLayoutElement element) {
			AssertionException.IsNotNull(element);
			AssertionException.IsNotNull(element.Item);
		}
		public virtual bool CanDrag(OperationType operation) { return false; }
		public virtual bool CanSelect() { return false; }
		public virtual bool AllowDragging { 
			get { return Element.Item.AllowDrag; } 
		}
		public virtual bool AllowResizing {
			get { return Element.Item.AllowSizing; }
		}
	}
	public class DockPaneElementBehavior : DockLayoutElementBehavior {
		public DockPaneElementBehavior(DockPaneElement element)
			: base(element) {
		}
		public override bool CanDrag(OperationType operation) {
			switch(operation) {
				case OperationType.FloatingMoving:
					return Element.Item.IsFloatingRootItem;
				case OperationType.Floating:
					return Element.Item.AllowFloat && !Element.Item.IsFloatingRootItem;
				case OperationType.ClientDragging:
					return !Element.Item.AllowFloat && Element.Item.AllowMove;
			}
			return false;
		}
		public override bool CanSelect() { return Element.Item.AllowSelection; }
	}
	public class GroupPaneElementBehavior : DockLayoutElementBehavior {
		public GroupPaneElementBehavior(GroupPaneElement element)
			: base(element) {
		}
		public override bool CanDrag(OperationType operation) {
			DockLayoutManager manager = DockLayoutManager.GetDockLayoutManager(Element.View);
			switch(operation) {
				case OperationType.ClientDragging:
					return ((manager != null) && manager.IsCustomization) && Element.Item.AllowMove;
			}
			return false;
		}
		public override bool CanSelect() {
			DockLayoutManager manager = DockLayoutManager.GetDockLayoutManager(Element.View);
			if(manager == null) return false;
			return manager.AllowSelection && Element.Item.AllowSelection;
		}
	}
	public class TabbedLayoutGroupElementBehaviour : DockLayoutElementBehavior {
		public TabbedLayoutGroupElementBehaviour(TabbedLayoutGroupElement element)
			: base(element) {
		}
		public override bool CanDrag(OperationType operation) {
			DockLayoutManager manager = DockLayoutManager.GetDockLayoutManager(Element.View);
			switch(operation) {
				case OperationType.ClientDragging:
					return ((manager != null) && manager.IsCustomization) && Element.Item.AllowMove;
			}
			return false;
		}
		public override bool CanSelect() {
			DockLayoutManager manager = DockLayoutManager.GetDockLayoutManager(Element.View);
			if(manager == null) return false;
			return manager.AllowSelection && Element.Item.AllowSelection;
		}
	}
	public class FloatDocumentElementBehavior : DockLayoutElementBehavior {
		public FloatDocumentElementBehavior(FloatDocumentElement element)
			: base(element) {
		}
		public override bool CanDrag(OperationType operation) {
			switch(operation) {
				case OperationType.FloatingMoving:
					return AllowDragging;
				case OperationType.FloatingResizing: {
						FloatGroup fGroup = Element.Item.Parent as FloatGroup;
						return AllowResizing && (fGroup != null && !fGroup.IsMaximized);
					}
			}
			return false;
		}
		public override bool AllowDragging {
			get { return base.AllowDragging || AllowResizing; }
		}
		public override bool CanSelect() { return Element.Item.AllowSelection; }
	}
	public class FloatPanePresenterElementBehavior : DockLayoutElementBehavior {
		public FloatPanePresenterElementBehavior(FloatPanePresenterElement element)
			: base(element) {
		}
		public override bool CanDrag(OperationType operation) {
			switch(operation) {
				case OperationType.FloatingMoving:
					return AllowDragging;
				case OperationType.FloatingResizing:
					return AllowResizing && !((FloatGroup)Element.Item).IsMaximized;
			}
			return false;
		}
		public override bool AllowDragging {
			get { return base.AllowDragging || AllowResizing; }
		}
	}
	public class AutoHideTrayElementBehavior : DockLayoutElementBehavior {
		public AutoHideTrayElementBehavior(AutoHideTrayElement element)
			: base(element) {
		}
		protected override void AssertElement(IDockLayoutElement element) {
			AssertionException.IsNotNull(element);
		}
	}
	public class AutoHidePaneElementBehavior : DockLayoutElementBehavior {
		public AutoHidePaneElementBehavior(AutoHidePaneElement element)
			: base(element) {
		}
		protected override void AssertElement(IDockLayoutElement element) {
			AssertionException.IsNotNull(element);
		}
		public override bool CanDrag(OperationType operation) {
			switch(operation) {
				case OperationType.Resizing: return AllowResizing;
			}
			return false;
		}
		public override bool AllowDragging {
			get { return base.AllowDragging || AllowResizing; }
		}
	}
	public class AutoHidePaneHeaderItemElementBehavior : DockLayoutElementBehavior {
		public AutoHidePaneHeaderItemElementBehavior(AutoHidePaneHeaderItemElement element)
			: base(element) {
		}
		public override bool CanDrag(OperationType operation) {
			switch(operation) {
				case OperationType.Reordering: return true;
				case OperationType.Floating: return Element.Item.AllowFloat;
				case OperationType.ClientDragging:
					return !Element.Item.AllowFloat && Element.Item.AllowMove;
			}
			return false;
		}
	}
	public class TabbedPaneItemElementBehavior : DockLayoutElementBehavior {
		public TabbedPaneItemElementBehavior(TabbedPaneItemElement element)
			: base(element) {
		}
		public override bool CanDrag(OperationType operation) {
			switch(operation) {
				case OperationType.Reordering:
					return true;
				case OperationType.Floating:
					return Element.Item.AllowFloat;
				case OperationType.ClientDragging:
					return !Element.Item.AllowFloat && Element.Item.AllowMove;
			}
			return false;
		}
	}
	public class TabbedPaneElementBehavior : DockLayoutElementBehavior {
		public TabbedPaneElementBehavior(TabbedPaneElement element)
			: base(element) {
		}
		public override bool CanDrag(OperationType operation) {
			switch(operation) {
				case OperationType.FloatingMoving:
					return Element.Item.IsFloatingRootItem;
				case OperationType.Floating:
					return Element.Item.AllowFloat && !Element.Item.IsFloatingRootItem;
				case OperationType.ClientDragging:
					return !Element.Item.AllowFloat && Element.Item.AllowMove;
			}
			return false;
		}
	}
	public class TabbedLayoutGroupHeaderElementBehavior : DockLayoutElementBehavior {
		public TabbedLayoutGroupHeaderElementBehavior(TabbedLayoutGroupHeaderElement element)
			: base(element) {
		}
		public override bool CanDrag(OperationType operation) {
			DockLayoutManager manager = DockLayoutManager.GetDockLayoutManager(Element.View);
			switch(operation) {
				case OperationType.Reordering:
					return ((manager != null) && manager.IsCustomization) && Element.Item.AllowMove;
				case OperationType.ClientDragging:
					return ((manager != null) && manager.IsCustomization) && Element.Item.AllowMove;
			}
			return false;
		}
		public override bool CanSelect() {
			DockLayoutManager manager = DockLayoutManager.GetDockLayoutManager(Element.View);
			if(manager == null) return false;
			return manager.AllowSelection && Element.Item.AllowSelection;
		}
	}
	public class DocumentPaneItemElementBehavior : DockLayoutElementBehavior {
		protected LayoutPanel LayoutPanel {
			get { return Element.Item as LayoutPanel; }
		}
		public DocumentPaneItemElementBehavior(DocumentPaneItemElement element)
			: base(element) {
		}
		public override bool CanDrag(OperationType operation) {
			switch(operation) {
				case OperationType.Reordering:
					return true;
				case OperationType.Floating:
					return LayoutPanel.AllowFloat;
				case OperationType.ClientDragging:
					return !LayoutPanel.AllowFloat && LayoutPanel.AllowMove;
			}
			return false;
		}
	}
	public class MDIDocumentElementBehavior : DockLayoutElementBehavior {
		protected DocumentPanel Document {
			get { return Element.Item as DocumentPanel; }
		}
		public MDIDocumentElementBehavior(MDIDocumentElement element)
			: base(element) {
		}
		public override bool CanDrag(OperationType operation) {
			if(Document == null) return false;
			switch(operation) {
				case OperationType.Reordering: return true;
				case OperationType.Resizing:
					return !(Document.IsMaximized || Document.IsMinimized) && AllowResizing;
			}
			return false;
		}
		public override bool AllowDragging {
			get { return base.AllowDragging || AllowResizing; }
		}
	}
	public class ControlItemElementBehavior : DockLayoutElementBehavior {
		public ControlItemElementBehavior(ControlItemElement element)
			: base(element) {
		}
		public override bool CanDrag(OperationType operation) {
			DockLayoutManager manager = DockLayoutManager.GetDockLayoutManager(Element.View);
			switch(operation) {
				case OperationType.ClientDragging:
					return ((manager != null) && manager.IsCustomization) && Element.Item.AllowMove;
			}
			return false;
		}
		public override bool CanSelect() {
			DockLayoutManager manager = DockLayoutManager.GetDockLayoutManager(Element.View);
			if(manager == null) return false;
			return manager.AllowSelection && Element.Item.AllowSelection; 
		}
	}
	public class HiddenItemElementBehavior : DockLayoutElementBehavior {
		public HiddenItemElementBehavior(HiddenItemElement element)
			: base(element) {
		}
		public override bool CanDrag(OperationType operation) {
			switch(operation) {
				case OperationType.ClientDragging: return Element.Item.AllowRestore;
			}
			return false;
		}
	}
	public class TreeItemElementBehavior : DockLayoutElementBehavior {
		public TreeItemElementBehavior(TreeItemElement element)
			: base(element) {
		}
		public override bool CanDrag(OperationType operation) {
			switch(operation) {
				case OperationType.ClientDragging: return Element.Item.AllowMove;
			}
			return false;
		}
		public override bool CanSelect() { return Element.Item.AllowSelection; }
	}
	public class FixedItemElementBehavior : DockLayoutElementBehavior {
		public FixedItemElementBehavior(FixedItemElement element)
			: base(element) {
		}
		public override bool CanDrag(OperationType operation) {
			DockLayoutManager manager = DockLayoutManager.GetDockLayoutManager(Element.View);
			switch(operation) {
				case OperationType.ClientDragging:
					return ((manager != null) && manager.IsCustomization) && Element.Item.AllowMove;
			}
			return false;
		}
		public override bool CanSelect() {
			DockLayoutManager manager = DockLayoutManager.GetDockLayoutManager(Element.View);
			if(manager == null) return false;
			return manager.AllowSelection && Element.Item.AllowSelection;
		}
	}
	public class LayoutSplitterElementBehavior : DockLayoutElementBehavior {
		public LayoutSplitterElementBehavior(LayoutSplitterElement element)
			: base(element) {
		}
		public override bool CanDrag(OperationType operation) {
			DockLayoutManager manager = DockLayoutManager.GetDockLayoutManager(Element.View);
			switch(operation) {
				case OperationType.ClientDragging:
					return ((manager != null) && manager.IsCustomization) && Element.Item.AllowMove;
			}
			return false;
		}
		public override bool CanSelect() {
			DockLayoutManager manager = DockLayoutManager.GetDockLayoutManager(Element.View);
			if(manager == null) return false;
			return manager.AllowSelection && Element.Item.AllowSelection;
		}
	}
}
