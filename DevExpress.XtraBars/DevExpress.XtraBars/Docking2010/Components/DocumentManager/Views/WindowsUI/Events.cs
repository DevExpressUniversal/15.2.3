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
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
namespace DevExpress.XtraBars.Docking2010.Views.WindowsUI {
	public class ContentContainerEventArgs : EventArgs {
		IContentContainer contentContainerCore;
		public ContentContainerEventArgs(IContentContainer contentContainer) {
			contentContainerCore = contentContainer;
		}
		public IContentContainer ContentContainer {
			get { return contentContainerCore; }
		}
	}
	public class ContentContainerCancelEventArgs : CancelEventArgs {
		IContentContainer contentContainerCore;
		public ContentContainerCancelEventArgs(IContentContainer contentContainer)
			: this(contentContainer, false) {
		}
		public ContentContainerCancelEventArgs(IContentContainer contentContainer, bool cancel)
			: base(cancel) {
			contentContainerCore = contentContainer;
		}
		public IContentContainer ContentContainer {
			get { return contentContainerCore; }
		}
	}
	public delegate void ContentContainerEventHandler(
		object sender, ContentContainerEventArgs e);
	public delegate void ContentContainerCancelEventHandler(
		object sender, ContentContainerCancelEventArgs e);
	public delegate void QueryContentContainerEventHandler(
		object sender, QueryContentContainerEventArgs e);
	public class QueryContentContainerEventArgs : EventArgs {
		IContentContainer contentContainerCore;
		public IContentContainer ContentContainer {
			get { return contentContainerCore; }
			set { contentContainerCore = value; }
		}
	}
	public delegate void NavigationBarsButtonClickEventHandler(
	   object sender, NavigationBarsButtonClickEventArgs e);
	public class NavigationBarsButtonClickEventArgs : EventArgs {
		DevExpress.XtraEditors.ButtonPanel.BaseButton buttonCore;
		public NavigationBarsButtonClickEventArgs(DevExpress.XtraEditors.ButtonPanel.BaseButton button) {
			buttonCore = button;
		}
		public DevExpress.XtraEditors.ButtonPanel.BaseButton Button {
			get { return buttonCore; }
		}
		public bool Handled { get; set; }
	}
	public delegate void BackButtonClickEventHandler(
	  object sender, BackButtonClickEventArgs e);
	public class BackButtonClickEventArgs : EventArgs {
		public BackButtonClickEventArgs() { }
		public bool Handled { get; set; }
	}
	public delegate void NavigationBarsCancelEventHandler(
		object sender, NavigationBarsCancelEventArgs e);
	public class NavigationBarsCancelEventArgs : ContentContainerCancelEventArgs {
		public NavigationBarsCancelEventArgs(IContentContainer container,
			System.Windows.Forms.Control originalSource, EventArgs eaSource, bool cancel)
			: base(container, cancel) {
			OriginalSource = originalSource;
			OriginalSourceEventArgs = eaSource;
		}
		public EventArgs OriginalSourceEventArgs { get; private set; }
		public System.Windows.Forms.Control OriginalSource { get; private set; }
	}
	public delegate void FlyoutCancelEventHandler(object sender, FlyoutCancelEventArgs e);
	public class FlyoutCancelEventArgs : CancelEventArgs { }
	public class FlyoutResultCancelEventArgs : FlyoutCancelEventArgs {
		public FlyoutResultCancelEventArgs(DialogResult result) {
			this.Result = result;
		}
		public DialogResult Result { get; set; }
	}
	public delegate void FlyoutResultEventHandler(object sender, FlyoutResultEventArgs e);
	public class FlyoutResultEventArgs : EventArgs {
		public FlyoutResultEventArgs(DialogResult result) {
			this.Result = result;
		}
		public DialogResult Result { get; set; }
	}
	public delegate void QueryFlyoutActionsControlEventHandler(object sender, QueryFlyoutActionsControlArgs e);
	public class QueryFlyoutActionsControlArgs : EventArgs {
		public QueryFlyoutActionsControlArgs(IContentContainerAction action) {
			Action = action;
		}
		public IContentContainerAction Action { get; private set; }
		public Control Control { get; set; }
	}
	public delegate void SearchPanelCancelEventHandler(object sender, SearchPanelCancelEventArgs e);
	public class SearchPanelCancelEventArgs : CancelEventArgs {
		Control originalSourceCore;
		public SearchPanelCancelEventArgs() { }
		public SearchPanelCancelEventArgs(Control originalSource, bool cancel)
			: base(cancel) {
				originalSourceCore = originalSource;
		}
		public Control OriginalSource { get { return originalSourceCore; } }
	}
	public delegate void SearchPanelEventHandler(object sender, SearchPanelEventArgs e);
	public class SearchPanelEventArgs : EventArgs { }
	public delegate void ContentContainerActionCustomizationEventHandler(
		object sender, ContentContainerActionCustomizationEventArgs e);
	public class ContentContainerActionCustomizationEventArgs : ContentContainerEventArgs {
		IList<IContentContainerAction> originalActions;
		public ContentContainerActionCustomizationEventArgs(IContentContainer container,
			IList<IContentContainerAction> originalActions)
			: base(container) {
			this.originalActions = originalActions;
		}
		public IEnumerable<IContentContainerAction> Actions {
			get {
				foreach(var action in originalActions)
					yield return action;
			}
		}
		public bool Contains(IContentContainerAction action) {
			return originalActions.Contains(action);
		}
		public bool Remove(IContentContainerAction action) {
			return originalActions.Remove(action);
		}
	}
	public delegate void CustomizeSearchItemsEventHandler(object sender, CustomizeSearchItemsEventArgs e);
	public class CustomizeSearchItemsEventArgs {
		public CustomizeSearchItemsEventArgs(IContentContainer sourceContainer, Base.BaseComponent source) {
			SourceContainer = sourceContainer;
			Source = source;
			Visible = true;
		}
		public IContentContainer SourceContainer { get; private set; }
		public Base.BaseComponent Source { get; private set; }
		public IEnumerable<string> Content { get; set; }
		public System.Drawing.Image Image { get; set; }
		public string SubTitle { get; set; }
		public string Title { get; set; }
		public DevExpress.Utils.DefaultBoolean AllowGlyphSkinning { get; set; }
		public bool Visible { get; set; }
	}
	public class SearchItemClickEventArgs {
		public SearchItemClickEventArgs(DevExpress.XtraBars.Docking2010.Customization.ISearchObjectContext context) {
			Title = context.Title;
			SourceContainer = context.SourceContainer;
			Source = context.Source;
		}
		public string Title { get; private set; }
		public Views.WindowsUI.IContentContainer SourceContainer { get; private set; }
		public Base.BaseComponent Source { get; private set; }
		public bool Cancel { get; set; }
	}
	public delegate void SearchItemClickEventHandler(object sender, SearchItemClickEventArgs e);
	public class TileEventArgs : EventArgs {
		BaseTile tileCore;
		public TileEventArgs(BaseTile tile) {
			tileCore = tile;
		}
		public BaseTile Tile {
			get { return tileCore; }
		}
	}
	public class TileCancelEventArgs : CancelEventArgs {
		BaseTile tileCore;
		public TileCancelEventArgs(BaseTile tile)
			: this(tile, false) {
		}
		public TileCancelEventArgs(BaseTile tile, bool cancel)
			: base(cancel) {
			tileCore = tile;
		}
		public BaseTile Tile {
			get { return tileCore; }
		}
	}
	public class TileClickEventArgs : TileEventArgs {
		public TileClickEventArgs(BaseTile tile)
			: base(tile) {
		}
		public bool Handled { get; set; }
	}
	public delegate void TileEventHandler(
		object sender, TileEventArgs e);
	public delegate void TileCancelEventHandler(
		object sender, TileCancelEventArgs e);
	public delegate void TileClickEventHandler(
		object sender, TileClickEventArgs e);
	public class SplitGroupEventArgs : EventArgs {
		SplitGroup splitGroupCore;
		public SplitGroupEventArgs(SplitGroup group) {
			splitGroupCore = group;
		}
		public SplitGroup Group {
			get { return splitGroupCore; }
		}
	}
	public delegate void SplitGroupEventHandler(
		object sender, SplitGroupEventArgs e);
	public class TileContainerEventArgs : EventArgs {
		TileContainer tileContainerCore;
		public TileContainerEventArgs(TileContainer group) {
			tileContainerCore = group;
		}
		public TileContainer Container {
			get { return tileContainerCore; }
		}
	}
	public delegate void TileContainerEventHandler(
		object sender, TileContainerEventArgs e);
	public delegate void NavigationEventHandler(
		object sender, NavigationEventArgs e);
	public class BaseNavigationEventArgs : System.EventArgs {
		public BaseNavigationEventArgs(WindowsUIView view, IContentContainer source) {
			this.View = view;
			this.SourceContextualZoomLevel = GetZoomLevel(source);
			this.Source = CheckContainer(source);
		}
		protected ContextualZoomLevel GetZoomLevel(IContentContainer container) {
			DetailContainer detailContainer = container as DetailContainer;
			if(detailContainer != null)
				return ContextualZoomLevel.Detail;
			OverviewContainer overviewContainer = container as OverviewContainer;
			if(overviewContainer != null)
				return ContextualZoomLevel.Overview;
			return ContextualZoomLevel.Normal;
		}
		protected IContentContainer CheckContainer(IContentContainer container) {
			DetailContainer detailContainer = container as DetailContainer;
			if(detailContainer != null)
				return detailContainer.Parent;
			OverviewContainer overviewContainer = container as OverviewContainer;
			if(overviewContainer != null)
				return overviewContainer.Parent;
			return container;
		}
		public WindowsUIView View { get; private set; }
		public IContentContainer Source { get; private set; }
		public ContextualZoomLevel SourceContextualZoomLevel { get; private set; }
	}
	public class NavigationEventArgs : BaseNavigationEventArgs, INavigationArgs {
		public NavigationEventArgs(WindowsUIView view, Document document, IContentContainer target, IContentContainer source, object tag) :
			base(view, source) {
			Base.AssertionException.IsNotNull(target);
			this.Document = document;
			this.Tag = tag;
			this.TargetContextualZoomLevel = GetZoomLevel(target);
			this.Target = CheckContainer(target);
		}
		public Document Document { get; private set; }
		public IContentContainer Target { get; private set; }
		public ContextualZoomLevel TargetContextualZoomLevel { get; private set; }
		public NavigationMode NavigationMode {
			get {
				if(Source == null)
					return NavigationMode.New;
				if(Source == Target)
					return NavigationMode.Refresh;
				if(Source.Parent == Target)
					return NavigationMode.Back;
				if(Target.Parent != Source)
					return NavigationMode.New;
				else
					return NavigationMode.Forward;
			}
		}
		public object Tag { get; private set; }
		public object Parameter { get; set; }
	}
	class NavigationArgs : INavigationArgs {
		NavigationEventArgs e;
		public NavigationArgs(NavigationEventArgs e) {
			this.e = e;
		}
		public WindowsUIView View {
			get { return e.View; }
		}
		public Document Document {
			get { return e.Document; }
		}
		public IContentContainer Source {
			get { return e.Source; }
		}
		public IContentContainer Target {
			get { return e.Target; }
		}
		public ContextualZoomLevel SourceContextualZoomLevel {
			get { return e.SourceContextualZoomLevel; }
		}
		public ContextualZoomLevel TargetContextualZoomLevel {
			get { return e.TargetContextualZoomLevel; }
		}
		public NavigationMode NavigationMode {
			get { return e.NavigationMode; }
		}
		public object Tag {
			get { return e.Tag; }
		}
		public object Parameter {
			get { return e.Parameter; }
			set { e.Parameter = value; }
		}
	}
	public delegate void QueryDocumentActionsEventHandler(
		object sender, QueryDocumentActionsEventArgs e);
	public class QueryDocumentActionsEventArgs : BaseNavigationEventArgs, IDocumentActionsArgs {
		IList<IDocumentAction> actionsCore;
		public QueryDocumentActionsEventArgs(WindowsUIView view, IContentContainer container, Document document)
			: base(view, container) {
			Base.AssertionException.IsNotNull(container);
			Base.AssertionException.IsNotNull(document);
			this.actionsCore = new List<IDocumentAction>();
			this.Document = document;
			this.FlyoutAction = null;
		}
		public Document Document { get; private set; }
		public FlyoutAction FlyoutAction { get; set; }
		public IList<IDocumentAction> DocumentActions {
			get { return actionsCore; }
		}
		IEnumerable<IUIAction<Document>> IUIActionsArgs<Document>.UIActions {
			get { return actionsCore; }
		}
	}
	public class CustomDrawBackButtonEventArgs : EventArgs {
		DevExpress.Utils.Drawing.GraphicsCache cacheCore;
		XtraEditors.ButtonPanel.BaseButtonInfo buttonInfoCore;
		XtraEditors.ButtonPanel.BaseButtonPainter buttonPainterCore;
		DevExpress.Utils.AppearanceObject appearanceCore;
		internal CustomDrawBackButtonEventArgs(
			DevExpress.Utils.Drawing.GraphicsCache cache, XtraEditors.ButtonPanel.BaseButtonInfo buttonInfo, XtraEditors.ButtonPanel.BaseButtonPainter buttonPainter, DevExpress.Utils.AppearanceObject appearance) {
			this.cacheCore = cache;
			this.buttonInfoCore = buttonInfo;
			this.buttonPainterCore = buttonPainter;
			this.appearanceCore = appearance;
		}
		public DevExpress.Utils.Drawing.GraphicsCache GraphicsCache { get { return cacheCore; } }
		public System.Drawing.Graphics Graphics {
			get { return cacheCore.Graphics; }
		}
		public System.Drawing.Rectangle Bounds {
			get { return buttonInfoCore.Bounds; }
		}
		public IHeaderButton Button {
			get { return buttonInfoCore.Button as IHeaderButton; }
		}
		public XtraEditors.ButtonPanel.BaseButtonPainter ButtonPainter {
			get { return buttonPainterCore; }
		}
		public XtraEditors.ButtonPanel.BaseButtonInfo ButtonInfo {
			get { return buttonInfoCore; }
		}
		public DevExpress.Utils.AppearanceObject Appearance {
			get { return appearanceCore; }
		}
		public void Draw() {
			DevExpress.Utils.Drawing.ObjectPainter.DrawObject(GraphicsCache, ButtonPainter, ButtonInfo);
		}
		public void DrawGlyph(System.Drawing.Image glyph) {
			if(glyph != null) {
				var glyphRect = DevExpress.Utils.PlacementHelper.Arrange(glyph.Size, ButtonInfo.ImageBounds, System.Drawing.ContentAlignment.MiddleCenter);
				Graphics.DrawImage(glyph, glyphRect);
			}
		}
		public bool Handled { get; set; }
	}
	public delegate void CustomDrawBackButtonEventHandler(
		object sender, CustomDrawBackButtonEventArgs e);
}
