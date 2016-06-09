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
using DevExpress.Diagram.Core;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Diagram;
namespace DevExpress.Xpf.Reports.UserDesigner.XRDiagram {
	partial class XRDiagramItem : ILogicalOwner {
		protected class XRDiagramItemControllerBase : DiagramItemController {
			public XRDiagramItemControllerBase(IDiagramItem item) : base(item) { }
			protected new XRDiagramControl Diagram { get { return (XRDiagramControl)base.Diagram; } }
			protected new DiagramItem Item { get { return (DiagramItem)base.Item; } }
			protected override IAdorner CreateDragPreviewAdorner(IDiagramControl diagram) {
				return XRDiagramItemBase.CreateDragPreviewAdorner(diagram);
			}
			protected override void BeforeDelete(Transaction transaction) {
				XRDiagramItemBase.GetBeforeDeleteCallback(Item).Do(x => x(transaction));
			}
		}
		protected override IEnumerable<PropertyDescriptor> GetEditablePropertiesCore() {
			return XRDiagramItemBase.GetEditableProperties(this) ?? new PropertyDescriptor[] { };
		}
		protected sealed override DiagramItemController CreateItemController() {
			return CreateXRItemController();
		}
		protected virtual XRDiagramItemControllerBase CreateXRItemController() {
			return new XRDiagramItemController(this);
		}
		void ILogicalOwner.AddChild(object child) {
			XRDiagramItemBase.AddLogicalChild(this, child);
		}
		void ILogicalOwner.RemoveChild(object child) {
			XRDiagramItemBase.RemoveLogicalChild(this, child);
		}
		protected override IEnumerator LogicalChildren { get { return XRDiagramItemBase.GetLogicalChildrenEnumerator(this, base.LogicalChildren); } }
	}
	partial class XRDiagramList : ILogicalOwner {
		protected class XRDiagramListControllerBase : DiagramListController {
			public XRDiagramListControllerBase(IDiagramList item) : base(item) { }
			protected new XRDiagramControl Diagram { get { return (XRDiagramControl)base.Diagram; } }
			protected new DiagramItem Item { get { return (DiagramItem)base.Item; } }
			protected override IAdorner CreateDragPreviewAdorner(IDiagramControl diagram) {
				return XRDiagramItemBase.CreateDragPreviewAdorner(diagram);
			}
			protected override void BeforeDelete(Transaction transaction) {
				XRDiagramItemBase.GetBeforeDeleteCallback(Item).Do(x => x(transaction));
			}
		}
		protected override IEnumerable<PropertyDescriptor> GetEditablePropertiesCore() {
			return XRDiagramItemBase.GetEditableProperties(this) ?? new PropertyDescriptor[] { };
		}
		protected sealed override DiagramListController CreateListController() {
			return CreateXRListController();
		}
		protected virtual XRDiagramListControllerBase CreateXRListController() {
			return new XRDiagramListController(this);
		}
		void ILogicalOwner.AddChild(object child) {
			XRDiagramItemBase.AddLogicalChild(this, child);
		}
		void ILogicalOwner.RemoveChild(object child) {
			XRDiagramItemBase.RemoveLogicalChild(this, child);
		}
		protected override IEnumerator LogicalChildren { get { return XRDiagramItemBase.GetLogicalChildrenEnumerator(this, base.LogicalChildren); } }
	}
	partial class XRDiagramContainer : ILogicalOwner {
		protected class XRDiagramContainerControllerBase : DiagramContainerController {
			public XRDiagramContainerControllerBase(IDiagramContainer item) : base(item) { }
			protected new XRDiagramControl Diagram { get { return (XRDiagramControl)base.Diagram; } }
			protected new DiagramItem Item { get { return (DiagramItem)base.Item; } }
			protected override IAdorner CreateDragPreviewAdorner(IDiagramControl diagram) {
				return XRDiagramItemBase.CreateDragPreviewAdorner(diagram);
			}
			protected override void BeforeDelete(Transaction transaction) {
				XRDiagramItemBase.GetBeforeDeleteCallback(Item).Do(x => x(transaction));
			}
		}
		protected override IEnumerable<PropertyDescriptor> GetEditablePropertiesCore() {
			return XRDiagramItemBase.GetEditableProperties(this) ?? new PropertyDescriptor[] { };
		}
		protected sealed override DiagramContainerController CreateContainerController() {
			return CreateXRContainerController();
		}
		protected virtual XRDiagramContainerControllerBase CreateXRContainerController() {
			return new XRDiagramContainerController(this);
		}
		void ILogicalOwner.AddChild(object child) {
			XRDiagramItemBase.AddLogicalChild(this, child);
		}
		void ILogicalOwner.RemoveChild(object child) {
			XRDiagramItemBase.RemoveLogicalChild(this, child);
		}
		protected override IEnumerator LogicalChildren { get { return XRDiagramItemBase.GetLogicalChildrenEnumerator(this, base.LogicalChildren); } }
	}
	partial class XRDiagramRoot : ILogicalOwner {
		protected class XRDiagramRootControllerBase : DiagramRootController {
			public XRDiagramRootControllerBase(IDiagramRoot item) : base(item) { }
			protected new XRDiagramControl Diagram { get { return (XRDiagramControl)base.Diagram; } }
			protected new DiagramItem Item { get { return (DiagramItem)base.Item; } }
			protected override IAdorner CreateDragPreviewAdorner(IDiagramControl diagram) {
				return XRDiagramItemBase.CreateDragPreviewAdorner(diagram);
			}
			protected override void BeforeDelete(Transaction transaction) {
				XRDiagramItemBase.GetBeforeDeleteCallback(Item).Do(x => x(transaction));
			}
		}
		protected override IEnumerable<PropertyDescriptor> GetEditablePropertiesCore() {
			return XRDiagramItemBase.GetEditableProperties(this) ?? new PropertyDescriptor[] { };
		}
		protected sealed override DiagramRootController CreateRootController() {
			return CreateXRRootController();
		}
		protected virtual XRDiagramRootControllerBase CreateXRRootController() {
			return new XRDiagramRootController(this);
		}
		void ILogicalOwner.AddChild(object child) {
			XRDiagramItemBase.AddLogicalChild(this, child);
		}
		void ILogicalOwner.RemoveChild(object child) {
			XRDiagramItemBase.RemoveLogicalChild(this, child);
		}
		protected override IEnumerator LogicalChildren { get { return XRDiagramItemBase.GetLogicalChildrenEnumerator(this, base.LogicalChildren); } }
	}
}
