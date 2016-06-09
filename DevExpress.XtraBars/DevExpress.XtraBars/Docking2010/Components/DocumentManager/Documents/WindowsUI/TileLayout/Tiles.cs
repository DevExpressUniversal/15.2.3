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
using System.Drawing;
using DevExpress.XtraBars.Docking2010.Base;
using DevExpress.XtraEditors;
using DevExpress.Utils.Design;
namespace DevExpress.XtraBars.Docking2010.Views.WindowsUI {
	public class Gadget : BaseTile {
		public Gadget() {
			Appearances.Normal.BackColor = Color.Lime;
		}
		protected internal sealed override object GetID() {
			return GetHashCode();
		}
	}
	[Designer("DevExpress.XtraBars.Design.TileDesigner, " + AssemblyInfo.SRAssemblyBarsDesign, 
		typeof(System.ComponentModel.Design.IDesigner)), 
	SmartTagSupport(typeof(TileBoundsProvider), SmartTagSupportAttribute.SmartTagCreationMode.Auto),
	SmartTagAction(typeof(TileActionsProvider), "Elements", "Edit Elements", SmartTagActionType.CloseAfterExecute), 
	SmartTagAction(typeof(TileActionsProvider), "Frames", "Edit Frames", SmartTagActionType.CloseAfterExecute), 
	SmartTagAction(typeof(TileActionsProvider), "BackgroundImage", "Edit BackgroundImage", SmartTagActionType.CloseAfterExecute)
	]
	public class Tile : BaseTile {
		Document documentCore;
		public Tile() { }
		public Tile(IContainer container)
			: base(container) {
		}
		public Tile(IBaseTileProperties parentProperties)
			: base(parentProperties) {
		}
		public Tile(IBaseTileProperties parentProperties, Document document)
			: base(parentProperties) {
			documentCore = document;
		}
		protected override void OnDispose() {
			if(ActivationTarget != null)
				UnsubscribeActivationTarget(ActivationTarget);
			base.OnDispose();
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("TileDocument"),
#endif
 DefaultValue(null), Category(CategoryName.Behavior), SmartTagProperty("Document", "")]
		public Document Document {
			get { return documentCore; }
			set {
				if(documentCore == value) return;
				var view = Manager != null ? Manager.View as WindowsUIView : null;
				var tile = view != null ? view.Tiles[value] : null;
				CheckValue(value, view, tile);
				SetValue(ref documentCore, value);
				Register(view);
			}
		}
		void Register(WindowsUIView view) {
			if(Document != null && view != null) {
				view.Tiles.Register(this);
			}
		}
		void CheckValue(Document value, WindowsUIView view, BaseTile tile) {
			BaseTile currentTile = null;
			if(view != null) {
				if(value != null && tile != null)
					throw new InvalidOperationException(DocumentManagerLocalizer.GetString(DocumentManagerStringId.DuplicateDocumentInTileExceptionMessage));
				if(documentCore != null && (value == null || (view.Tiles.TryGetValue(Document, out currentTile) && currentTile == this)))
					view.Tiles.Unregister(this);
			}
		}
		protected internal sealed override bool IsEnabled {
			get { return (Document != null) && Document.IsEnabled; }
		}
		protected internal sealed override bool CanActivate {
			get { return (Document != null) && Document.Properties.CanActivate; }
		}
		protected internal sealed override Document[] AssociatedDocuments {
			get { return new Document[] { Document }; }
		}
		protected internal sealed override object GetID() {
			return documentCore;
		}
		protected internal sealed override IContentContainer AssociatedContentContainer {
			get { return activationTargetCore; }
		}
		IContentContainer activationTargetCore;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("TileActivationTarget"),
#endif
 DefaultValue(null), Category(CategoryName.Behavior), SmartTagProperty("Activation Target", "")]
		public IContentContainer ActivationTarget {
			get { return activationTargetCore; }
			set {
				if(activationTargetCore == value) return;
				if(ActivationTargetHelper.CanAddActivationTarget(this, value)) {
					if(ActivationTarget != null)
						UnsubscribeActivationTarget(ActivationTarget);
					activationTargetCore = value;
					if(ActivationTarget != null)
						SubscribeActivationTarget(ActivationTarget);
					RaiseHierarchyChanged();
				}
			}
		}
		protected virtual void SubscribeActivationTarget(IContentContainer container) {
			((DevExpress.Utils.Base.IBaseObject)container).Disposed += OnActivationTargetDisposed;
		}
		protected virtual void UnsubscribeActivationTarget(IContentContainer container) {
			((DevExpress.Utils.Base.IBaseObject)container).Disposed -= OnActivationTargetDisposed;
		}
		void OnActivationTargetDisposed(object sender, EventArgs e) {
			if(ActivationTarget != null)
				UnsubscribeActivationTarget(ActivationTarget);
			activationTargetCore = null;
		}
		protected void RaiseHierarchyChanged() {
			if(Manager != null && Manager.View != null && Manager.View is WindowsUIView) {
				((WindowsUIView)Manager.View).RaiseHierarchyChanged();
			}
		}
	}
	public class TileGroup : BaseTile {
		BaseTile[] tilesCore;
		public TileGroup(IBaseTileProperties parentProperties, BaseTile[] tiles)
			: base(parentProperties) {
			tilesCore = tiles;
			Appearances.Normal.BackColor = Color.Violet;
		}
		protected internal sealed override bool CanActivate {
			get { return tilesCore.Length > 0; }
		}
		protected internal sealed override bool IsContainer {
			get { return true; }
		}
		protected internal sealed override object GetID() {
			return GetHashCode();
		}
		protected internal override Document[] AssociatedDocuments {
			get { return Array.ConvertAll(tilesCore, (Converter<BaseTile, Document>)ToDocument); }
		}
		static Document ToDocument(BaseTile tile) {
			return ((Tile)tile).Document;
		}
		[Browsable(false)]
		public bool IsAutoCreated { get; private set; }
		protected internal void MarkAsAutoCreated() {
			IsAutoCreated = true;
		}
	}
	public class BaseTileCollection : BaseMutableListEx<BaseTile, WindowsUIView> {
		IDictionary<object, BaseTile> ids;
		public BaseTileCollection(WindowsUIView owner)
			: base(owner) {
			ids = new Dictionary<object, BaseTile>();
		}
		protected override void OnBeforeElementAdded(BaseTile element) {
			Owner.BeginUpdate();
			element.SetManager(Owner.Manager);
			base.OnBeforeElementAdded(element);
		}
		protected override void OnElementAdded(BaseTile element) {
			Owner.AddToContainer(element);
			base.OnElementAdded(element);
			Register(element);
			Owner.EndUpdate();
		}
		protected override void OnBeforeElementRemoved(BaseTile element) {
			Owner.BeginUpdate();
			base.OnBeforeElementRemoved(element);
		}
		protected override void OnElementRemoved(BaseTile element) {
			Unregister(element);
			base.OnElementRemoved(element);
			Owner.RemoveFromContainer(element);
			element.SetManager(null);
			Owner.EndUpdate();
		}
		protected override void OnElementRemoveCanceled(BaseTile element) {
			base.OnElementRemoveCanceled(element);
			Owner.CancelUpdate();
		}
		protected internal void EnsureRegistered(BaseTile element) {
			object id = element.GetID() ?? FindID(element);
			if(id != null) {
				ids.Remove(id);
				ids.Add(id, element);
			}
		}
		protected override bool CanAdd(BaseTile element) {
			var tile = element as Tile;
			if(tile != null && tile.Document != null && ids.ContainsKey(tile.Document)) {
				throw new InvalidOperationException(DocumentManagerLocalizer.GetString(DocumentManagerStringId.DuplicateDocumentInTileExceptionMessage));
			}
			return base.CanAdd(element);
		}
		protected internal void Register(BaseTile element) {
			object id = element.GetID();
			if(id != null)
				ids.Add(id, element);
		}
		protected internal void Unregister(BaseTile element) {
			object id = element.GetID() ?? FindID(element);
			if(id != null)
				ids.Remove(id);
		}
		public bool TryGetValue(object id, out BaseTile tile) {
			return ids.TryGetValue(id, out tile);
		}
		protected object FindID(BaseTile tile) {
			foreach(KeyValuePair<object, BaseTile> pair in ids)
				if(pair.Value == tile) return pair.Key;
			return null;
		}
		protected override void NotifyOwnerOnInsert(int index) {
		}
		public BaseTile this[string name] {
			get { return FindFirst((tile) => (!string.IsNullOrEmpty(tile.Name) && tile.Name.Equals(name))); }
		}
		public BaseTile this[BaseDocument document] {
			get { return FindFirst((tile) => ((tile as Tile) != null && (tile as Tile).Document != null && (tile as Tile).Document.Equals(document))); }
		}
	}
	public static class ActivationTargetHelper {
		public static bool CanAddActivationTarget(Tile targetTile, IContentContainer activationTarget) {
			bool result = true;
			IContentContainerInternal container = activationTarget as IContentContainerInternal;
			if(activationTarget == null || container == null) return true;
			TileContainer tileContainer = container as TileContainer;
			if(tileContainer != null)
				result &= !tileContainer.Items.Contains(targetTile);
			foreach(IContentContainerInternal child in container.Children) {
				if(child is IContentContainerInternal)
					result &= CanAddActivationTarget(targetTile, child);
			}
			return result;
		}
		public static bool CanRemoveChildItem(IContentContainer childItem, IContentContainer parentItem) {
			bool result = true;
			IContentContainerInternal container = parentItem as IContentContainerInternal;
			if(parentItem == null || container == null) return true;
			TileContainer tileContainer = container as TileContainer;
			if(tileContainer != null) {
				foreach(Tile tileItem in tileContainer.Items) {
					result &= (tileItem.ActivationTarget != childItem);
				}
			}
			foreach(IContentContainerInternal child in container.Children) {
				if(child is IContentContainerInternal)
					result &= CanRemoveChildItem(childItem, child);
			}
			return result;
		}
	}
}
