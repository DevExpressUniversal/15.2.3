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
using DevExpress.Utils.Gesture;
using DevExpress.XtraBars.Docking2010.Customization;
using DevExpress.XtraTab.Buttons;
using DevExpress.XtraBars.Docking2010.Views.Widget;
namespace DevExpress.XtraBars.Docking2010.Views {
	public class DocumentHeaderClickEventArgs : DocumentEventArgsHandled {
		public DocumentHeaderClickEventArgs(BaseDocument document)
			: base(document) {
		}
	}
	public delegate void DocumentHeaderClickEventHandler(
	   object sender, DocumentHeaderClickEventArgs e);
	public class DocumentEventArgs : EventArgs {
		BaseDocument documentCore;
		public DocumentEventArgs(BaseDocument document) {
			documentCore = document;
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("DocumentEventArgsDocument")]
#endif
		public BaseDocument Document {
			get { return documentCore; }
		}
	}
	public class DocumentEventArgsHandled : DocumentEventArgs {
		public DocumentEventArgsHandled(BaseDocument document)
			: base(document) {
		}
		public bool Handled { get; set; }
	}
	public class DocumentCancelEventArgs : CancelEventArgs {
		BaseDocument documentCore;
		public DocumentCancelEventArgs(BaseDocument document)
			: this(document, false) {
		}
		public DocumentCancelEventArgs(BaseDocument document, bool cancel)
			: base(cancel) {
			documentCore = document;
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("DocumentCancelEventArgsDocument")]
#endif
		public BaseDocument Document {
			get { return documentCore; }
		}
	}
	public delegate void DeferredControlLoadEventHandler(
		object sender, DeferredControlLoadEventArgs e);
	public class DeferredControlLoadEventArgs : DocumentEventArgs {
		public DeferredControlLoadEventArgs(BaseDocument document, System.Windows.Forms.Control control)
			: base(document) {
			this.controlCore = control;
		}
		System.Windows.Forms.Control controlCore;
		public System.Windows.Forms.Control Control {
			get { return controlCore; }
		}
	}
	public delegate void QueryControlEventHandler(
			object sender, QueryControlEventArgs e
		);
	public class QueryControlEventArgs : DocumentEventArgs {
		public QueryControlEventArgs(BaseDocument document)
			: base(document) {
		}
		System.Windows.Forms.Control controlCore;
		public System.Windows.Forms.Control Control {
			get { return controlCore; }
			set { controlCore = value; }
		}
	}
	public delegate void ControlReleasingEventHandler(
			object sender, ControlReleasingEventArgs e
		);
	public class ControlReleasingEventArgs : DocumentCancelEventArgs {
		public ControlReleasingEventArgs(BaseDocument document, bool keepControl, bool disposeControl)
			: base(document, keepControl) {
			DisposeControl = disposeControl;
		}
		public bool KeepControl {
			get { return Cancel; }
			set { Cancel = value; }
		}
		public bool DisposeControl { get; set; }
	}
	public class BeginFloatingEventArgs : DocumentCancelEventArgs {
		public BeginFloatingEventArgs(BaseDocument document, FloatingReason reason)
			: this(document, false, reason) {
		}
		public BeginFloatingEventArgs(BaseDocument document, bool cancel, FloatingReason reason)
			: base(document, cancel) {
			reasonCore = reason;
		}
		FloatingReason reasonCore;
		public FloatingReason FloatingReason {
			get { return reasonCore; }
		}
	}
	public class EndFloatingEventArgs : DocumentEventArgs {
		public EndFloatingEventArgs(BaseDocument document, EndFloatingReason reason)
			: base(document) {
			reasonCore = reason;
		}
		EndFloatingReason reasonCore;
		public EndFloatingReason EndFloatingReason {
			get { return reasonCore; }
		}
	}
	public delegate void DocumentEventHandler(
		object sender, DocumentEventArgs e);
	public delegate void DocumentCancelEventHandler(
		object sender, DocumentCancelEventArgs e);
	public delegate void NextDocumentEventHandler(
		object sender, NextDocumentEventArgs e);
	public class NextDocumentEventArgs : DocumentEventArgsHandled {
		bool forwardCore;
		BaseDocument nextDocumentCore;
		public NextDocumentEventArgs(BaseDocument document, BaseDocument next, bool forward)
			: base(document) {
			forwardCore = forward;
			nextDocumentCore = next;
		}
		public bool ForwardNavigation {
			get { return forwardCore; }
		}
		public BaseDocument NextDocument {
			get { return nextDocumentCore; }
			set { nextDocumentCore = value; }
		}
	}
	public class ShowingDockGuidesEventArgs : DocumentEventArgs {
		BaseViewHitInfo hitInfoCore;
		DockGuidesConfiguration configurationCore;
		public ShowingDockGuidesEventArgs(BaseDocument document, DockGuidesConfiguration configuration, BaseViewHitInfo hitInfo)
			: base(document) {
			configurationCore = configuration;
			hitInfoCore = hitInfo;
		}
		public DockGuidesConfiguration Configuration {
			get { return configurationCore; }
		}
		public BaseViewHitInfo HitInfo {
			get { return hitInfoCore; }
		}
	}
	public delegate void ShowingDockGuidesEventHandler(
		object sender, ShowingDockGuidesEventArgs e);
	public delegate void PopupMenuShowingEventHandler(
		object sender, PopupMenuShowingEventArgs e);
	public class PopupMenuShowingEventArgs : CancelEventArgs {
		BaseViewHitInfo hitInfoCore;
		BaseViewControllerMenu menuCore;
		public PopupMenuShowingEventArgs(BaseViewControllerMenu menu, BaseViewHitInfo hitInfo) {
			this.hitInfoCore = hitInfo;
			this.menuCore = menu;
		}
		public BaseViewControllerMenu Menu {
			get { return menuCore; }
		}
		public BaseViewHitInfo HitInfo {
			get { return hitInfoCore; }
		}
		public BaseDocument GetDocument() {
			FloatDocumentForm floatForm = (Menu != null) ? Menu.PlacementTarget as FloatDocumentForm : null;
			return (floatForm != null) ? floatForm.Document : BaseDocument.GetDocument(HitInfo.HitElement);
		}
	}
	public delegate void CustomHeaderButtonEventHandler(
		object sender, CustomHeaderButtonEventArgs e);
	public class CustomHeaderButtonEventArgs : EventArgs {
		Tabbed.Document documentCore;
		CustomHeaderButton buttonCore;
		public CustomHeaderButtonEventArgs(CustomHeaderButton button, Tabbed.Document document) {
			buttonCore = button;
			documentCore = document;
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("CustomHeaderButtonEventArgsDocument")]
#endif
		public Tabbed.Document Document {
			get { return documentCore; }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("CustomHeaderButtonEventArgsButton")]
#endif
		public CustomHeaderButton Button {
			get { return buttonCore; }
		}
	}
	public delegate void LayoutResettingEventHandler(
		   object sender, LayoutResettingEventArgs e
	   );
	public class LayoutResettingEventArgs : CancelEventArgs { }
	public delegate void LayoutBeginSizingEventHandler(
		   object sender, LayoutBeginSizingEventArgs e
	   );
	public delegate void LayoutEndSizingEventHandler(
	   object sender, LayoutEndSizingEventArgs e
	   );
	public class LayoutSizingEventArgs : CancelEventArgs {
		public LayoutSizingEventArgs(IBaseSplitterInfo splitInfo) {
			IsHorizontal = splitInfo.IsHorizontal;
			BaseDocument[] documents = splitInfo.GetDocuments();
		}
		public bool IsHorizontal { get; private set; }
		public BaseDocument Document1 { get; private set; }
		public BaseDocument Document2 { get; private set; }
	}
	public class LayoutBeginSizingEventArgs : LayoutSizingEventArgs {
		public LayoutBeginSizingEventArgs(IBaseSplitterInfo splitInfo) : base(splitInfo) { }
	}
	public class LayoutEndSizingEventArgs : LayoutSizingEventArgs {
		public LayoutEndSizingEventArgs(int change, IBaseSplitterInfo splitInfo)
			: base(splitInfo) {
			Change = change;
		}
		public int Change { get; private set; }
	}
	public delegate void DocumentSelectorCustomSortItemsEventHandler(object sender, DocumentSelectorCustomSortItemsEventArgs e);
	public class DocumentSelectorCustomSortItemsEventArgs : EventArgs {
		public DocumentSelectorCustomSortItemsEventArgs()
			: base() {
			DocumentComparer = new DefaultDocumentComparer();
			DockPanelComparer = new DefaultDockPanelComparer();
		}
		public System.Collections.Generic.IComparer<BaseDocument> DocumentComparer { get; set; }
		public System.Collections.Generic.IComparer<DevExpress.XtraBars.Docking.DockPanel> DockPanelComparer { get; set; }
	}
	public delegate void StackGroupDraggingEventHandler(
	  object sender, StackGroupDraggingEventArgs e
	);
	public class StackGroupDraggingEventArgs : CancelEventArgs {
		StackGroup groupCore;
		StackGroup targetGroupCore;
		public StackGroupDraggingEventArgs() : base() { }
		public StackGroupDraggingEventArgs(StackGroup group, StackGroup targetGroup)
			: base() {
			groupCore = group;
			targetGroupCore = targetGroup;
		}
		public StackGroupDraggingEventArgs(StackGroup group, StackGroup targetGroup, bool cancel)
			: base(cancel) {
			groupCore = group;
			targetGroupCore = targetGroup;
		}
		public StackGroup Group {
			get { return groupCore; }
		}
		public StackGroup TargetGroup {
			get { return targetGroupCore; }
		}
	}
	public delegate void LoadingIndicatorShowingEventHandler(
		object sender, LoadingIndicatorShowingEventArgs e
	);
	public class LoadingIndicatorShowingEventArgs : DocumentCancelEventArgs {
		public LoadingIndicatorShowingEventArgs(BaseDocument document, bool show)
			: base(document, !show) {
		}
		public bool Show {
			get { return !Cancel; }
			set { Cancel = !value; }
		}
	}
}
namespace DevExpress.XtraBars.Docking2010 {
	using DevExpress.XtraBars.Docking2010.Views;
	public class DocumentsHostWindowEventArgs : EventArgs {
		IDocumentsHostWindow hostWindowCore;
		public DocumentsHostWindowEventArgs(IDocumentsHostWindow hostWindow) {
			this.hostWindowCore = hostWindow;
		}
		public IDocumentsHostWindow HostWindow { get { return hostWindowCore; } }
	}
	public delegate void DocumentsHostWindowEventHandler(
		object sender, DocumentsHostWindowEventArgs e);
	public enum EmptyDocumentsHostWindowReason {
		DocumentClosed,
		DocumentDisposed
	}
	public class EmptyDocumentsHostWindowEventArgs : DocumentsHostWindowEventArgs {
		public EmptyDocumentsHostWindowEventArgs(IDocumentsHostWindow hostWindow, bool keepOpen, EmptyDocumentsHostWindowReason reason)
			: base(hostWindow) {
			KeepOpen = keepOpen;
			Reason = reason;
		}
		public EmptyDocumentsHostWindowReason Reason { get; private set; }
		public bool KeepOpen { get; set; }
	}
	public delegate void EmptyDocumentsHostWindowEventHandler(
		object sender, EmptyDocumentsHostWindowEventArgs e);
	public class CustomDocumentsHostWindowEventArgs : DocumentCancelEventArgs {
		public CustomDocumentsHostWindowEventArgs(BaseDocument document, DocumentsHostWindowConstructor defaultConstructor)
			: base(document) {
			Constructor = defaultConstructor;
		}
		public DocumentsHostWindowConstructor Constructor { get; set; }
	}
	public delegate void CustomDocumentsHostWindowEventHandler(
		object sender, CustomDocumentsHostWindowEventArgs e);
	public delegate IDocumentsHostWindow DocumentsHostWindowConstructor();
	public class CustomDrawBackgroundEventArgs : EventArgs {
		DevExpress.Utils.Drawing.GraphicsCache cacheCore;
		System.Drawing.Rectangle boundsCore;
		internal CustomDrawBackgroundEventArgs(
			DevExpress.Utils.Drawing.GraphicsCache cache, System.Drawing.Rectangle bounds) {
			this.cacheCore = cache;
			this.boundsCore = bounds;
		}
		public virtual DevExpress.Utils.Drawing.GraphicsCache GraphicsCache { get { return cacheCore; } }
		public virtual System.Drawing.Graphics Graphics { get { return cacheCore.Graphics; } }
		public virtual System.Drawing.Rectangle Bounds { get { return boundsCore; } }
	}
	public delegate void CustomDrawBackgroundEventHandler(
		object sender, CustomDrawBackgroundEventArgs e);
	public enum GestureID {
		QueryAllowGesture,
		Begin,
		Pan,
		Rotate,
		Zoom,
		PressAndTap,
		TwoFingerTap
	}
	public delegate void GestureHandler(
		GestureID gid, GestureArgs args, object[] parameters);
	public delegate bool FlickGestureHandler(
		Point point, FlickGestureArgs args);
	public class ViewEventArgs : EventArgs {
		BaseView viewCore;
		public ViewEventArgs(BaseView view) {
			this.viewCore = view;
		}
		public BaseView View { get { return viewCore; } }
	}
	public delegate void ViewEventHandler(
		object sender, ViewEventArgs args
	);
}
