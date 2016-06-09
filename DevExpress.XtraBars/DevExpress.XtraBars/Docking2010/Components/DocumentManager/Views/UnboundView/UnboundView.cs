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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.Utils.Drawing;
using DevExpress.XtraBars.Docking2010.Customization;
namespace DevExpress.XtraBars.Docking2010.Views.NoDocuments {
	public class Document : BaseDocument {
		public Document() { }
		public Document(IContainer container)
			: base(container) {
		}
		public Document(IBaseDocumentProperties defaultProperties)
			: base(defaultProperties) {
		}
		protected internal override bool Borderless {
			get { return false; }
		}
	}
	public class NoDocumentsView : BaseView {
		public NoDocumentsView() { }
		public NoDocumentsView(IContainer container)
			: base(container) {
		}
		public sealed override ViewType Type {
			get { return ViewType.NoDocuments; }
		}
		protected sealed internal override Type GetUIElementKey() {
			return typeof(NoDocumentsView);
		}
		protected internal sealed override bool CanProcessHooks() {
			return false;
		}
		protected sealed internal override bool AllowMdiLayout {
			get { return false; }
		}
		protected sealed internal override bool AllowMdiSystemMenu {
			get { return false; }
		}
		protected override IBaseViewController CreateController() {
			return new NoDocumentsViewController(this);
		}
		protected internal override void PatchActiveChildren(Point offset) { }
		protected internal override void PatchBeforeActivateChild(Control activatedChild, Point offset) { }
		#region XtraSerializable
		protected override BaseSerializableDocumentInfo CreateSerializableDocumentInfo(BaseDocument document) {
			throw new NotSupportedException();
		}
		#endregion XtraSerializable
		protected override void OnShowingDockGuidesCore(DockGuidesConfiguration configuration, BaseDocument document, BaseViewHitInfo hitInfo) {
			base.OnShowingDockGuidesCore(configuration, document, hitInfo);
			configuration.Disable(DockHint.Center);
		}
		protected internal override void RegisterListeners(DragEngine.BaseUIView uiView) {
			uiView.RegisterUIServiceListener(new Dragging.DocumentManagerUIViewRegularDragListener());
			uiView.RegisterUIServiceListener(new Dragging.DocumentManagerUIViewDockingListener());
			uiView.RegisterUIServiceListener(new Dragging.DocumentManagerUIViewFloatingDragListener());
			uiView.RegisterUIServiceListener(new Dragging.DocumentManagerUIViewUIInteractionListener());
		}
	}
	class NoDocumentsViewInfo : BaseViewInfo {
		public NoDocumentsViewInfo(NoDocumentsView view)
			: base(view) {
		}
		protected internal override Point GetFloatLocation(BaseDocument document) {
			throw new NotSupportedException();
		}
		protected override Rectangle[] CalculateCore(Graphics g, Rectangle bounds) {
			return new Rectangle[] {  };
		}
		protected override IDockingAdornerInfo CreateEmptyViewAdornerInfo() {
			return new EmptyViewDockingAdornerInfo(View as NoDocumentsView);
		}
		protected override bool UseEmptyViewAdorner() {
			return true;
		}
	}
	class NoDocumentsViewHitInfo : BaseViewHitInfo {
		public NoDocumentsViewHitInfo(NoDocumentsView view)
			: base(view) {
		}
	}
	class NoDocumentsViewPainter : BaseViewPainter {
		public NoDocumentsViewPainter(NoDocumentsView view)
			: base(view) {
		}
		protected override void DrawCore(DevExpress.Utils.Drawing.GraphicsCache bufferedCache, Rectangle clip) {
		}
	}
	class NoDocumentsViewSkinPainter : NoDocumentsViewPainter {
		Skin skin;
		public NoDocumentsViewSkinPainter(NoDocumentsView view)
			: base(view) {
			skin = DockingSkins.GetSkin(View.ElementsLookAndFeel);
		}
		protected internal override ObjectPainter GetDocumentSelectorHeaderPainter() {
			return new Customization.DocumentSelectorHeaderSkinPainter(View.ElementsLookAndFeel);
		}
		protected internal override ObjectPainter GetDocumentSelectorFooterPainter() {
			return new Customization.DocumentSelectorFooterSkinPainter(View.ElementsLookAndFeel);
		}
		protected internal override ObjectPainter GetDocumentSelectorItemsListPainter() {
			return new Customization.DocumentSelectorItemsListSkinPainter(View.ElementsLookAndFeel);
		}
		protected internal override ObjectPainter GetDocumentSelectorPreviewPainter() {
			return new Customization.DocumentSelectorPreviewSkinPainter(View.ElementsLookAndFeel);
		}
		protected internal override ObjectPainter GetDocumentSelectorBackgroundPainter() {
			return new Customization.DocumentSelectorBackgroundSkinPainter(View.ElementsLookAndFeel);
		}
	}
	class NoDocumentsViewController : BaseViewController {
		public NoDocumentsViewController(NoDocumentsView view)
			: base(view) {
		}
		protected override bool DockCore(BaseDocument baseDocument) {
			throw new NotSupportedException();
		}
		protected override bool RemoveDocumentCore(BaseDocument document) {
			throw new NotSupportedException();
		}
		protected override bool AddDocumentCore(BaseDocument document) {
			throw new NotSupportedException();
		}
		protected override void PatchControlAfterRemove(Control control) {
			throw new NotSupportedException();
		}
		protected override void PatchControlBeforeAdd(Control control) {
			throw new NotSupportedException();
		}
	}
}
