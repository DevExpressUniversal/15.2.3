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
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using DevExpress.XtraBars.Docking2010.Views.Tabbed;
using DevExpress.XtraBars.Docking2010.Views.NativeMdi;
using DevExpress.XtraBars.Docking2010.Views.WindowsUI;
using DevExpress.XtraBars.Docking2010.Views.NoDocuments;
using DevExpress.XtraBars.Docking2010.Views.Widget;
namespace DevExpress.XtraBars.Docking2010.Views {
	public interface IViewRegistrator {
		BaseViewPainter CreatePainter(BaseView view);
		BaseViewInfo CreateViewInfo(BaseView view);
		BaseViewHitInfo CreateHitInfo(BaseView view);
		BaseDocument CreateDocument(BaseView view, Control control);
		DocumentContainer CreateDocumentContainer(BaseDocument document);
		Control CreateDocumentsHost(IDocumentsHostOwner owner);
	}
	public interface IBaseRegistrator {
		BaseViewPainter CreatePainter();
		BaseViewInfo CreateViewInfo();
		BaseViewHitInfo CreateHitInfo();
		BaseDocument CreateDocument(Control control);
		DocumentContainer CreateDocumentContainer(BaseDocument document);
		Control CreateDocumentsHost(IDocumentsHostOwner owner);
	}
	public abstract class BaseRegistrator : IBaseRegistrator {
		public delegate IBaseRegistrator Create(BaseView view);
		protected readonly BaseView View;
		protected BaseRegistrator(BaseView view) {
			View = view;
		}
		public abstract BaseViewPainter CreatePainter();
		public abstract BaseViewInfo CreateViewInfo();
		public abstract BaseViewHitInfo CreateHitInfo();
		public abstract BaseDocument CreateDocument(Control control);
		public abstract DocumentContainer CreateDocumentContainer(BaseDocument document);
		public abstract Control CreateDocumentsHost(IDocumentsHostOwner owner);
		protected abstract Create Register();
		public static Create Register<T>()
			where T : BaseRegistrator, new() {
			return new T().Register();
		}
	}
	class TabbedViewRegistrator : BaseRegistrator {
		public TabbedViewRegistrator()
			: base(null) {
		}
		public TabbedViewRegistrator(TabbedView view)
			: base(view) {
		}
		public override BaseViewPainter CreatePainter() {
			if(View.IsSkinPaintStyle)
				return new TabbedViewSkinPainter(View as TabbedView);
			return new TabbedViewPainter(View as TabbedView);
		}
		public override BaseViewInfo CreateViewInfo() {
			return new TabbedViewInfo(View as TabbedView);
		}
		public override BaseViewHitInfo CreateHitInfo() {
			return new TabbedViewHitInfo(View as TabbedView);
		}
		public override BaseDocument CreateDocument(Control control) {
			return new Tabbed.Document(View.DocumentProperties as Tabbed.IDocumentProperties);
		}
		protected override Create Register() {
			return delegate(BaseView view) {
				return new TabbedViewRegistrator(view as TabbedView);
			};
		}
		public override DocumentContainer CreateDocumentContainer(BaseDocument document) {
			return new DocumentContainer(document);
		}
		public override Control CreateDocumentsHost(IDocumentsHostOwner owner) {
			return new DocumentsHost(owner);
		}
	}
	class NativeMdiViewRegistrator : BaseRegistrator {
		public NativeMdiViewRegistrator()
			: base(null) {
		}
		public NativeMdiViewRegistrator(NativeMdiView view)
			: base(view) {
		}
		public override BaseViewPainter CreatePainter() {
			if(View.IsSkinPaintStyle)
				return new NativeMdiViewSkinPainter(View as NativeMdiView);
			return new NativeMdiViewPainter(View as NativeMdiView);
		}
		public override BaseViewInfo CreateViewInfo() {
			return new NativeMdiViewInfo(View as NativeMdiView);
		}
		public override BaseViewHitInfo CreateHitInfo() {
			return new NativeMdiViewHitInfo(View as NativeMdiView);
		}
		public override BaseDocument CreateDocument(Control control) {
			return new NativeMdi.Document(View.DocumentProperties);
		}
		protected override Create Register() {
			return delegate(BaseView view) {
				return new NativeMdiViewRegistrator(view as NativeMdiView);
			};
		}
		public override DocumentContainer CreateDocumentContainer(BaseDocument document) {
			return new DocumentContainer(document);
		}
		public override Control CreateDocumentsHost(IDocumentsHostOwner owner) {
			return new DocumentsHost(owner);
		}
	}
	class NoDocumentsViewRegistrator : BaseRegistrator {
		public NoDocumentsViewRegistrator()
			: base(null) {
		}
		public NoDocumentsViewRegistrator(NoDocumentsView view)
			: base(view) {
		}
		public override BaseViewPainter CreatePainter() {
			if(View.IsSkinPaintStyle)
				return new NoDocumentsViewSkinPainter(View as NoDocumentsView);
			return new NoDocumentsViewPainter(View as NoDocumentsView);
		}
		public override BaseViewInfo CreateViewInfo() {
			return new NoDocumentsViewInfo(View as NoDocumentsView);
		}
		public override BaseViewHitInfo CreateHitInfo() {
			return new NoDocumentsViewHitInfo(View as NoDocumentsView);
		}
		public override BaseDocument CreateDocument(Control control) {
			return new NoDocuments.Document(View.DocumentProperties);
		}
		protected override Create Register() {
			return delegate(BaseView view) {
				return new NoDocumentsViewRegistrator(view as NoDocumentsView);
			};
		}
		public override DocumentContainer CreateDocumentContainer(BaseDocument document) {
			return new DocumentContainer(document);
		}
		public override Control CreateDocumentsHost(IDocumentsHostOwner owner) {
			return new DocumentsHost(owner);
		}
	}
	class WindowsUIViewRegistrator : BaseRegistrator {
		public WindowsUIViewRegistrator()
			: base(null) {
		}
		public WindowsUIViewRegistrator(WindowsUIView view)
			: base(view) {
		}
		public override BaseViewPainter CreatePainter() {
			if(View.IsSkinPaintStyle)
				return new WindowsUIViewSkinPainter(View as WindowsUIView);
			return new WindowsUIViewPainter(View as WindowsUIView);
		}
		public override BaseViewInfo CreateViewInfo() {
			return new WindowsUIViewInfo(View as WindowsUIView);
		}
		public override BaseViewHitInfo CreateHitInfo() {
			return new WindowsUIViewHitInfo(View as WindowsUIView);
		}
		public override BaseDocument CreateDocument(Control control) {
			return new WindowsUI.Document(View.DocumentProperties as WindowsUI.IDocumentProperties);
		}
		protected override Create Register() {
			return delegate(BaseView view) {
				return new WindowsUIViewRegistrator(view as WindowsUIView);
			};
		}
		public override DocumentContainer CreateDocumentContainer(BaseDocument document) {
			return new DocumentContainer(document);
		}
		public override Control CreateDocumentsHost(IDocumentsHostOwner owner) {
			return new DocumentsHost(owner);
		}
	}
	class WidgetViewRegistrator : BaseRegistrator {
		public WidgetViewRegistrator()
			: base(null) {
		}
		public WidgetViewRegistrator(WidgetView view)
			: base(view) {
		}
		public override BaseViewPainter CreatePainter() {
			if(View.IsSkinPaintStyle)
				return new WidgetViewSkinPainter(View as WidgetView);
			return new WidgetViewPainter(View as WidgetView);
		}
		public override BaseViewInfo CreateViewInfo() {
			return new WidgetViewInfo(View as WidgetView);
		}
		public override BaseViewHitInfo CreateHitInfo() {
			return new WidgetViewHitInfo(View as WidgetView);
		}
		public override BaseDocument CreateDocument(Control control) {
			return new Widget.Document(View.DocumentProperties as Widget.IDocumentProperties);
		}
		protected override Create Register() {
			return delegate(BaseView view) {
				return new WidgetViewRegistrator(view as WidgetView);
			};
		}
		public override DocumentContainer CreateDocumentContainer(BaseDocument document) {
			return new WidgetContainer(document);
		}
		public override Control CreateDocumentsHost(IDocumentsHostOwner owner) {
			return new WidgetsHost(owner);
		}
	}
}
