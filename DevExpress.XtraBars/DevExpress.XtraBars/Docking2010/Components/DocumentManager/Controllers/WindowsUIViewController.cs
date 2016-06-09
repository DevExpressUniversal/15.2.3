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
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
namespace DevExpress.XtraBars.Docking2010.Views.WindowsUI {
	class WindowsUIViewController : BaseViewController, IWindowsUIViewController, IWindowsUIViewControllerInternal {
		public WindowsUIViewController(WindowsUIView view)
			: base(view) {
		}
		public new WindowsUIView View {
			get { return base.View as WindowsUIView; }
		}
		public bool Activate(BaseTile tile) {
			if(View.IsDisposing || !View.IsLoaded) return false;
			if(tile == null || !tile.IsEnabled || !tile.CanActivate) return false;
			using(View.LockPainting()) {
				IContentContainer container = tile.AssociatedContentContainer;
				if(container == null) {
					var contentContainer = View.ActiveContentContainer as IContentContainerInternal;
					if(contentContainer != null) {
						IContentContainer target = null;
						Document[] documents = tile.AssociatedDocuments;
						if(documents.Length == 1) {
							View.SetNavigationTag(tile.Tag);
							target = contentContainer.GetDetailContainer(documents[0]);
						}
						else target = null;
						return ActivateContentContainerCore(target);
					}
					return false;
				}
				else {
					((IWindowsUIViewControllerInternal)this).PrepareAssociatedContentContainer(tile, container);
					return ActivateContentContainerCore(container);
				}
			}
		}
		bool IWindowsUIViewControllerInternal.PrepareAssociatedContentContainer(BaseTile baseTile, IContentContainer associatedContainer) {
			Tile tile = baseTile as Tile;
			if(tile == null) return false;
			IDocumentSelector selector = associatedContainer as IDocumentSelector;
			if(selector != null && selector.Contains(tile.Document))
				selector.SetSelected(tile.Document);
			Page page = associatedContainer as Page;
			if(page != null && page.Document == null)
				page.Document = tile.Document;
			View.SetNavigationTag(tile.Tag);
			return true;
		}
		public bool Activate(BaseTile[] tiles) {
			if(View.IsDisposing || !View.IsLoaded) return false;
			TileGroup tileGroup = View.CreateTileGroup(tiles);
			tileGroup.MarkAsAutoCreated();
			return Activate(tileGroup);
		}
		public bool Activate(IContentContainer container) {
			if(container == null || View.IsDisposing || !View.IsLoaded) return false;
			using(View.LockPainting()) {
				return ActivateContentContainerCore(container);
			}
		}
		public bool Overview(IContentContainer container) {
			if(container == null || View.IsDisposing || !View.IsLoaded) return false;
			using(View.LockPainting()) {
				var contentContainer = container as IContentContainerInternal;
				if(contentContainer != null && contentContainer.ZoomLevel == ContextualZoomLevel.Normal) {
					if(contentContainer.Count > 1) {
						IContentContainer overviewContainer = contentContainer.GetOverviewContainer();
						return ActivateContentContainerCore(overviewContainer);
					}
				}
				return false;
			}
		}
		public bool Back() {
			if(View.IsDisposing || !View.IsLoaded) return false;
			IContentContainer container = View.ActiveContentContainer;
			return (container != null) && (container.Parent != null) &&
				Activate(container.Parent);
		}
		public bool Home() {
			if(View.IsDisposing || !View.IsLoaded) return false;
			IContentContainer container = View.ActiveContentContainer;
			IContentContainer root = BaseContentContainer.GetRoot(container);
			return (container != null) && (container != root) && (container.Parent != root) &&
				Activate(root);
		}
		public bool EnableFullScreenMode(bool enable) {
			if(View.IsDisposing || !View.IsLoaded) return false;
			if(enable == IsFullScreenMode(View.Manager)) return false;
			Form form = DocumentsHostContext.GetForm(View.Manager);
			if(form != null) {
				form.FormBorderStyle = enable ? FormBorderStyle.None : FormBorderStyle.Sizable;
				form.WindowState = enable ? FormWindowState.Maximized : FormWindowState.Normal;
			}
			return (form != null) && enable == IsFullScreenMode(View.Manager);
		}
		public static bool IsFullScreenMode(DocumentManager manager) {
			Form form = DocumentsHostContext.GetForm(manager);
			return (form != null) && (form.WindowState == FormWindowState.Maximized) && (form.FormBorderStyle == FormBorderStyle.None);
		}
		public bool Exit() {
			if(View.IsDisposing || !View.IsLoaded) return false;
			Form form = DocumentsHostContext.GetForm(View.Manager);
			if(form != null)
				form.Close();
			return (form != null) && form.IsDisposed;
		}
		public bool Rotate(SplitGroup group) {
			if(View.IsDisposing || !View.IsLoaded || group == null) return false;
			using(View.LockPainting()) {
				bool horz = group.Properties.ActualOrientation == Orientation.Horizontal;
				group.Properties.Orientation = horz ? Orientation.Vertical : Orientation.Horizontal;
				View.RequestInvokePatchActiveChild();
				return true;
			}
		}
		public bool Flip(SplitGroup group) {
			if(View.IsDisposing || !View.IsLoaded || group == null) return false;
			using(View.LockPainting()) {
				int[] lengths = group.GetLengths();
				if(lengths.Length > 1) {
					for(int i = 0; i < lengths.Length; i++)
						group[i] = lengths[(lengths.Length - 1) - i];
				}
				View.RequestInvokePatchActiveChild();
				return true;
			}
		}
		bool IWindowsUIViewControllerInternal.ProcessCheckedTiles(TileContainer tileContainer, Func<BaseTile, bool> action) {
			if(tileContainer == null || action == null)
				return false;
			bool hasUpdates = false;
			using(var update = BatchUpdate.Enter(tileContainer.Manager)) {
				foreach(BaseTile tile in tileContainer.CheckedTiles) 
					hasUpdates |= action(tile);
				if(hasUpdates) {
					tileContainer.RaiseActualActionsChanged();
				}
				else update.Cancel();
			}
			return hasUpdates;
		}
		protected bool ActivateContentContainerCore(IContentContainer container) {
			if(View.SetActiveContentContainerCore(container)) {
				View.RequestInvokePatchActiveChild();
				return true;
			}
			return false;
		}
		protected override void ActivateCore(BaseDocument document) {
			if(document != null && View.ActiveContentContainer != null) {
				IContentContainer container = View.ActiveContentContainer;
				if(container is IDocumentContainer)
					container = ((IDocumentContainer)container).Parent;
				if(container != null && container.Contains((Document)document)) {
					IDocumentSelector selector = container as IDocumentSelector;
					if(selector != null) {
						if(selector.SelectedDocument != document) {
							View.StartItemsNavigation(selector);
							selector.SetSelected((Document)document);
							Manager.InvokePatchActiveChildren();
						}
						return;
					}
					if(View.IsFlyoutDeactivation) return;
					var contentContainer = container as IContentContainerInternal;
					if(contentContainer != null) {
						IContentContainer detailContainer = contentContainer.GetDetailContainer(document);
						ActivateContentContainerCore(detailContainer);
					}
				}
			}
		}
		protected override bool AddDocumentCore(BaseDocument document) {
			return true;
		}
		protected override bool RemoveDocumentCore(BaseDocument document) {
			return true;
		}
		protected override bool DockCore(BaseDocument baseDocument) {
			Document document = baseDocument as Document;
			if(document == null) return false;
			if(AddDocumentCore(document)) {
				Manager.InvokePatchActiveChildren();
				return true;
			}
			return false;
		}
		protected override void PatchControlBeforeAdd(Control control) {
			if(Manager != null) Manager.PatchControlBeforeAdd(control);
		}
		protected override void PatchControlAfterRemove(Control control) {
			if(Manager != null) Manager.PatchControlAfterRemove(control);
		}
		public bool AddTile(Document document) {
			if(View.IsDisposing || document == null) return false;
			BaseTile tile = View.CreateTile(document);		   
			if(View.Tiles.Add(tile)) {
				if(document.Site != null && tile.Site != null) {
					try {
						if(!string.IsNullOrEmpty(document.ControlName))
							tile.Name = tile.Site.Name = document.ControlName + "Tile";
					}
					catch { }
				}
				return AddTileInContainers(View.ContentContainers, tile) != null;
			}
			return false;
		}
		public bool RemoveTile(Document document) {
			if(View.IsDisposing || document == null) return false;
			BaseTile tile;
			if(View.Tiles.TryGetValue(document, out tile)) {
				return View.Tiles.Remove(tile) &&
					 RemoveTileFromContainers(View.ContentContainers, tile);
			}
			return false;
		}
		public bool AddTile(BaseTile tile) {
			return (tile != null) && AddTileInContainers(View.ContentContainers, tile) != null;
		}
		protected bool RemoveTileFromContainers(ContentContainerCollection containers, BaseTile tile) {
			bool removed = false;
			IContentContainer[] tmp = containers.ToArray();
			foreach(IContentContainer container in tmp) {
				TileContainer tileContainer = container as TileContainer;
				if(tileContainer == null) continue;
				removed |= tileContainer.Items.Remove(tile);
			}
			return removed;
		}
		protected TileContainer AddTileInContainers(ContentContainerCollection containers, BaseTile tile) {
			if(!containers.Exists((e) => e is TileContainer))
				AddTileContainer(containers);
			foreach(IContentContainer container in containers) {
				TileContainer tileContainer = container as TileContainer;
				if(tileContainer != null && AddTileToContainer(tile, tileContainer))
					return tileContainer;
			}
			return AddTileInNewTileContainer(containers, tile);
		}
		protected TileContainer AddTileInNewTileContainer(ContentContainerCollection containers, BaseTile tile) {
			return AddTileInNewTileContainer(containers, tile, -1);
		}
		protected TileContainer AddTileInNewTileContainer(ContentContainerCollection containers, BaseTile tile, int insertIndex) {
			TileContainer result = AddTileContainer(containers, insertIndex);
			AddTileToContainer(tile, result);
			return result;
		}
		protected bool AddTileToContainer(BaseTile tile, TileContainer container) {
			return container.Items.Add(tile);
		}
		protected TileContainer AddTileContainer(ContentContainerCollection containers) {
			return AddTileContainer(containers, -1);
		}
		protected TileContainer AddTileContainer(ContentContainerCollection containers, int insertIndex) {
			TileContainer container = View.CreateTileContainer();
			if(insertIndex < 0 || insertIndex > containers.Count)
				containers.Add(container);
			else containers.Insert(insertIndex, container);
			return container;
		}
	}
}
