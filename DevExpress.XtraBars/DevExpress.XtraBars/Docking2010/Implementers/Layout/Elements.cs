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

using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraBars.Docking2010.DragEngine;
using DevExpress.XtraBars.Docking2010.Views;
namespace DevExpress.XtraBars.Docking2010.Dragging {
	abstract class DocumentLayoutElement : BaseLayoutElement, IDocumentLayoutElement {
		public virtual ILayoutElementBehavior GetBehavior() {
			return new DockLayoutElementBehavior();
		}
		IBaseElementInfo IDocumentLayoutElement.GetElementInfo() {
			return GetElementInfoCore();
		}
		protected virtual IBaseElementInfo GetElementInfoCore() { return null; }
	}
	abstract class DocumentLayoutContainer : BaseLayoutContainer, IDocumentLayoutContainer {
		public virtual ILayoutElementBehavior GetBehavior() {
			return new DockLayoutElementBehavior();
		}
		IBaseElementInfo IDocumentLayoutElement.GetElementInfo() {
			return GetElementInfoCore();
		}
		protected virtual IBaseElementInfo GetElementInfoCore() { return null; }
	}
	class DocumentManagerElement : DocumentLayoutContainer {
		DocumentManager managerCore;
		public DocumentManagerElement(DocumentManager manager) {
			managerCore = manager;
		}
		public DocumentManager Manager {
			get { return managerCore; }
		}
		protected override void EnsureBoundsCore() {
			Bounds = Manager.Bounds;
		}
		public override ILayoutElementBehavior GetBehavior() {
			return new DocumentManagerElementBehavior(this);
		}
		protected override LayoutElementHitInfo CreateHitInfo(Point pt) {
			return new DocumentManagerElementHitInfo(pt, this);
		}
		public override bool HitTestingEnabled {
			get { return Manager.HitTestEnabled; }
		}
	}
	class BaseViewElement : DocumentLayoutContainer {
		BaseView viewCore;
		Point offsetNC;
		public BaseViewElement(BaseView view) {
			viewCore = view;
			offsetNC = View.Manager.GetOffsetNC();
		}
		public BaseView View {
			get { return viewCore; }
		}
		protected override void EnsureBoundsCore() {
			Bounds = Offset(View.Bounds, offsetNC);
		}
		public override ILayoutElementBehavior GetBehavior() {
			return new BaseViewElementBehavior(this);
		}
		protected override LayoutElementHitInfo CreateHitInfo(Point pt) {
			return new BaseViewElementHitInfo(pt, this);
		}
		protected override IBaseElementInfo GetElementInfoCore() {
			return View.ViewInfo.EmptyViewAdornerInfo;
		}
	}
	class BaseFloatInfoElement : DocumentLayoutElement {
		IBaseDocumentInfo infoCore;
		Form formCore;
		public Form Form {
			get { return formCore; }
		}
		public BaseFloatInfoElement(IBaseDocumentInfo info) {
			infoCore = info;
			formCore = info.BaseDocument.Form;
		}
		protected override void EnsureBoundsCore() {
			Bounds = new Rectangle(GetClientRectOffset(Form), Form.Size);
		}
		protected override IBaseElementInfo GetElementInfoCore() {
			return infoCore;
		}
		static Point GetClientRectOffset(Form form) {
			Point clientOrigin = form.PointToScreen(Point.Empty);
			return new Point(form.Left - clientOrigin.X, form.Top - clientOrigin.Y);
		}
	}
	class FloatDocumentInfoElement : BaseFloatInfoElement {
		public FloatDocumentInfoElement(IFloatDocumentInfo info)
			: base(info) {
		}
		public override ILayoutElementBehavior GetBehavior() {
			return new FloatDocumentElementBehavior(this);
		}
	}
	class FloatPanelInfoElement : BaseFloatInfoElement {
		public FloatPanelInfoElement(IFloatPanelInfo info)
			: base(info) {
		}
		public override ILayoutElementBehavior GetBehavior() {
			return new FloatPanelElementBehavior(this);
		}
	}
	class DocumentsHostWindowElement : DocumentLayoutElement {
		Form Form;
		IBaseDocumentInfo infoCore;
		public DocumentsHostWindowElement(IDocumentsHostWindowRoot root) {
			infoCore = root;
			Form = root.Window as Form;
		}
		protected override IBaseElementInfo GetElementInfoCore() {
			return infoCore;
		}
		public override ILayoutElementBehavior GetBehavior() {
			return new DocumentsHostWindowElementBehavior(this);
		}
		protected override void EnsureBoundsCore() {
			Bounds = new Rectangle(GetClientRectOffset(Form), Form.Size);
		}
		static Point GetClientRectOffset(Form form) {
			Point clientOrigin = form.PointToScreen(Point.Empty);
			return new Point(form.Left - clientOrigin.X, form.Top - clientOrigin.Y);
		}
	}
}
