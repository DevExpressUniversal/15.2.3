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
using System.Windows.Forms;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraBars.Docking2010.Base;
using DevExpress.XtraEditors;
using DevExpress.Utils.Base;
namespace DevExpress.XtraBars.Docking2010.Views.WindowsUI {
	public interface ITileContainerProperties : IContentContainerProperties,
		ITileControlProperties {
		new Orientation Orientation { get; set; }
		bool AllowDragTilesBetweenGroups { get; set; }
	}
	public interface ITileContainerDefaultProperties : IContentContainerDefaultProperties {
		int? RowCount { get; set; }
		int? ColumnCount { get; set; }
		int? ItemSize { get; set; }
		int? LargeItemWidth { get; set; }
		int? IndentBetweenItems { get; set; }
		int? IndentBetweenGroups { get; set; }
		Padding? Padding { get; set; }
		Padding? ItemPadding { get; set; }
		ImageLayout? BackgroundImageLayout { get; set; }
		TileItemContentAlignment? ItemImageAlignment { get; set; }
		TileItemContentAlignment? ItemBackgroundImageAlignment { get; set; }
		DevExpress.Utils.HorzAlignment? HorizontalContentAlignment { get; set; }
		DevExpress.Utils.VertAlignment? VerticalContentAlignment { get; set; }
		TileItemContentShowMode? ItemTextShowMode { get; set; }
		TileItemImageScaleMode? ItemImageScaleMode { get; set; }
		TileItemImageScaleMode? ItemBackgroundImageScaleMode { get; set; }
		TileItemCheckMode? ItemCheckMode { get; set; }
		TileItemContentAnimationType? ItemContentAnimation { get; set; }
		TileItemBorderVisibility? ItemBorderVisibility { get; set; }
		DevExpress.Utils.DefaultBoolean AllowHtmlDraw { get; set; }
		DevExpress.Utils.DefaultBoolean AllowItemHover { get; set; }
		DevExpress.Utils.DefaultBoolean AllowSelectedItem { get; set; }
		DevExpress.Utils.DefaultBoolean AllowDrag { get; set; }
		DevExpress.Utils.DefaultBoolean AllowDragTilesBetweenGroups { get; set; }
		DevExpress.Utils.DefaultBoolean ShowText { get; set; }
		DevExpress.Utils.DefaultBoolean ShowGroupText { get; set; }
		DevExpress.Utils.DefaultBoolean AllowGroupHighlighting { get; set; }
		GroupHighlightingProperties AppearanceGroupHighlighting { get; set; }
		int ActualRowCount { get; }
		int ActualColumnCount { get; }
		int ActualItemSize { get; }
		int ActualLargeItemWidth { get; }
		int ActualIndentBetweenItems { get; }
		int ActualIndentBetweenGroups { get; }
		Padding ActualPadding { get; }
		Padding ActualItemPadding { get; }
		ImageLayout ActualBackgroundImageLayout { get; }
		TileItemContentAlignment ActualItemImageAlignment { get; }
		TileItemContentAlignment ActualItemBackgroundImageAlignment { get; }
		DevExpress.Utils.HorzAlignment ActualHorizontalContentAlignment { get; }
		DevExpress.Utils.VertAlignment ActualVerticalContentAlignment { get; }
		TileItemImageScaleMode ActualItemImageScaleMode { get; }
		TileItemImageScaleMode ActualItemBackgroundImageScaleMode { get; }
		TileItemContentAnimationType ActualItemContentAnimation { get; }
		TileItemBorderVisibility ActualItemBorderVisibility { get; }
		TileItemCheckMode ActualItemCheckMode { get; }
		TileItemContentShowMode ActualItemTextShowMode { get; }
		GroupHighlightingProperties ActualAppearanceGroupHighlighting { get; }
		bool CanHtmlDraw { get; }
		bool CanItemHover { get; }
		bool CanDrag { get; }
		bool CanDragTilesBetweenGroups { get; }
		bool ActualAllowSelectedItem { get; }
		bool CanShowText { get; }
		bool CanShowGroupText { get; }
		bool CanGroupHighlighting { get; }
		ITileControlProperties ActualProperties { get; }
	}
	public interface ISupportActivation {
		event EventHandler Activated;
		event EventHandler Deactivated;
	}
	[ToolboxItem(false), DesignTimeVisible(false)]
	[ProvideProperty("ID", "DevExpress.XtraBars.Docking2010.Views.WindowsUI.BaseTile")]
	[Designer("DevExpress.XtraBars.Design.TileContainerDesigner, " + AssemblyInfo.SRAssemblyBarsDesign, typeof(System.ComponentModel.Design.IDesigner)),
	SmartTagSupport(typeof(TileContainerBoundsProvider), DevExpress.Utils.Design.SmartTagSupportAttribute.SmartTagCreationMode.Auto),
	SmartTagAction(typeof(TileContainerActionsProvider), "BackgroundImage", "Edit BackgroundImage"),
	SmartTagAction(typeof(TileContainerActionsProvider), "Image", "Edit Image"),
	SmartTagAction(typeof(TileContainerActionsProvider), "Buttons", "Edit Buttons")
	]
	public class TileContainer : BaseContentContainer, DevExpress.Utils.IAppearanceOwner, IXtraSerializable, IExtenderProvider, ISupportActivation {
		TileCollection itemsCore;
		IDictionary<BaseTile, int> idCollection;
		public TileContainer()
			: base((IContainer)null) {
		}
		public TileContainer(IContainer container)
			: base(container) {
		}
		public TileContainer(ITileContainerProperties defaultProperties)
			: base(defaultProperties) {
		}
		protected override int Count {
			get { return Items.Count; }
		}
		protected override ContextualZoomLevel GetZoomLevel() {
			return ContextualZoomLevel.Overview;
		}
		protected sealed override Document[] GetDocumentsCore() {
			HashSet<Document> documentsHash = new HashSet<Document>();
			foreach(BaseTile tile in Items) {
				Document[] documents = tile.AssociatedDocuments;
				for(int i = 0; i < documents.Length; i++) {
					if(documents[i] == null) continue;
					documentsHash.Add(documents[i]);
				}
			}
			Document[] result = new Document[documentsHash.Count];
			documentsHash.CopyTo(result);
			return result;
		}
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TileCollection Items {
			get { return itemsCore; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("TileContainerProperties"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category(CategoryName.Properties), SmartTagSearchNestedProperties]
		public new ITileContainerDefaultProperties Properties {
			get { return base.Properties as ITileContainerDefaultProperties; }
		}
		void ResetAppearanceItem() { AppearanceItem.Reset(); }
		bool ShouldSerializeAppearanceItem() { return AppearanceItem.ShouldSerialize(); }
		TileItemAppearances appearances;
#if !SL
	[DevExpressXtraBarsLocalizedDescription("TileContainerAppearanceItem")]
#endif
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category(CategoryName.Appearance)]
		public TileItemAppearances AppearanceItem {
			get {
				if(appearances == null) {
					appearances = new TileItemAppearances(this);
					appearances.Changed += OnAppearanceChanged;
				}
				return appearances;
			}
		}
		void OnAppearanceChanged(object sender, EventArgs e) {
			LayoutChanged();
		}
		bool DevExpress.Utils.IAppearanceOwner.IsLoading {
			get { return IsActive; }
		}
		void ResetAppearanceText() { AppearanceText.Reset(); }
		bool ShouldSerializeAppearanceText() { return AppearanceText.ShouldSerialize(); }
		DevExpress.Utils.AppearanceObject appearanceText;
#if !SL
	[DevExpressXtraBarsLocalizedDescription("TileContainerAppearanceText")]
#endif
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category(CategoryName.Appearance)]
		public DevExpress.Utils.AppearanceObject AppearanceText {
			get {
				if(appearanceText == null)
					appearanceText = new DevExpress.Utils.AppearanceObject();
				return appearanceText;
			}
		}
		void ResetAppearanceGroupText() { AppearanceGroupText.Reset(); }
		bool ShouldSerializeAppearanceGroupText() { return AppearanceGroupText.ShouldSerialize(); }
		DevExpress.Utils.AppearanceObject appearanceGroupText;
#if !SL
	[DevExpressXtraBarsLocalizedDescription("TileContainerAppearanceGroupText")]
#endif
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category(CategoryName.Appearance)]
		public DevExpress.Utils.AppearanceObject AppearanceGroupText {
			get {
				if(appearanceGroupText == null)
					appearanceGroupText = new DevExpress.Utils.AppearanceObject();
				return appearanceGroupText;
			}
		}
		IContentContainer activationTargetCore;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("TileContainerActivationTarget"),
#endif
 DefaultValue(null), Category(CategoryName.Behavior)]
		public IContentContainer ActivationTarget {
			get { return activationTargetCore; }
			set {
				if(activationTargetCore == value || IsChild(this, value) || value == this) return;
				if(ActivationTarget != null)
					UnsubscribeActivationTarget(ActivationTarget);
				activationTargetCore = value;
				if(ActivationTarget != null)
					SubscribeActivationTarget(ActivationTarget);
				RaiseHierarchyChanged();
				if(ActivationTarget != null)
					ActivationTarget.Parent = this;
			}
		}
		internal int positionCore;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("TileContainerPosition"),
#endif
 DefaultValue(0), Category(CategoryName.Layout)]
		public int Position {
			get { return (Info != null) ? Info.TileControl.Position : positionCore; }
			set {
				if(IsDisposing || positionCore == value) return;
				if(Info != null && Info.TileControl != null)
					Info.TileControl.Position = value;
				else positionCore = value;
			}
		}
		Image backgroundImageCore;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("TileContainerBackgroundImage"),
#endif
 Category(CategoryName.Appearance),
		DefaultValue(null), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(System.Drawing.Design.UITypeEditor))]
		public Image BackgroundImage {
			get { return backgroundImageCore; }
			set { SetValue(ref backgroundImageCore, value); }
		}
		protected virtual void SubscribeActivationTarget(IContentContainer container) {
			((IBaseObject)container).Disposed += OnActivationTargetDisposed;
		}
		protected virtual void UnsubscribeActivationTarget(IContentContainer container) {
			((IBaseObject)container).Disposed -= OnActivationTargetDisposed;
		}
		void OnActivationTargetDisposed(object sender, EventArgs e) {
			if(ActivationTarget != null)
				UnsubscribeActivationTarget(ActivationTarget);
			activationTargetCore = null;
		}
		protected override IBaseProperties GetParentProperties(WindowsUIView view) {
			return view.TileContainerProperties;
		}
		protected override void OnCreate() {
			itemsCore = CreateItems();
			Items.CollectionChanged += OnItemsCollectionChanged;
			idCollection = new Dictionary<BaseTile, int>();
			base.OnCreate();
		}
		protected override void LockComponentBeforeDisposing() {
			if(appearances != null)
				appearances.Changed -= OnAppearanceChanged;
			if(Items != null)
				Items.CollectionChanged -= OnItemsCollectionChanged;
			if(ActivationTarget != null)
				UnsubscribeActivationTarget(ActivationTarget);
			base.LockComponentBeforeDisposing();
		}
		protected override void OnDispose() {
			Ref.Dispose(ref itemsCore);
			Ref.Clear(ref idCollection);
			base.OnDispose();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected internal int this[int index] {
			get { return GetIDCore(GetBaseTile(index)); }
			set { SetIDCore(GetBaseTile(index), value); }
		}
		protected BaseTile GetBaseTile(int index) {
			return (index >= 0 && index < Items.Count) ? Items[index] : null;
		}
		protected int GetIDCore(BaseTile tile) {
			int id;
			return (tile != null) && idCollection.TryGetValue(tile, out id) ? id : -1;
		}
		protected void SetIDCore(BaseTile tile, int value) {
			if(GetIDCore(tile) == -1) {
				if(IsInitializing || IsDeserializing)
					idCollection.Add(tile, value);
				return;
			}
			idCollection[tile] = value;
		}
		int GenerateID(int startValue) {
			if(idCollection != null && idCollection.Values.Contains(startValue)) {
				return GenerateID(++startValue);
			}
			return startValue;
		}
		#region IExtenderProvider Members
		public bool CanExtend(object extendee) {
			return extendee is BaseTile && Items != null && Items.Contains((BaseTile)extendee);
		}
		[Category(CategoryName.Layout), DefaultValue(0)]
		public int GetID(BaseTile tile) {
			return GetIDCore(tile);
		}
		[Category(CategoryName.Layout)]
		public void SetID(BaseTile tile, int value) {
			SetIDCore(tile, value);
		}
		#endregion
		protected override IContentContainerInfo CreateContentContainerInfo(WindowsUIView view) {
			return new TileContainerInfo(view, this);
		}
		protected override IContentContainerDefaultProperties CreateDefaultProperties(IContentContainerProperties parentProperties) {
			return new TileContainerDefaultProperties(parentProperties as ITileContainerProperties);
		}
		protected virtual TileCollection CreateItems() {
			return new TileCollection(this);
		}
		void OnItemsCollectionChanged(CollectionChangedEventArgs<BaseTile> ea) {
			if(ea.ChangedType == CollectionChangedType.ElementAdded) {
				if(!idCollection.ContainsKey(ea.Element) && !IsDeserializing)
					idCollection.Add(ea.Element, GenerateID(0));
				RegisterTile(ea.Element);
			}
			if(ea.ChangedType == CollectionChangedType.ElementDisposed) {
				idCollection.Remove(ea.Element);
				UnregisterTile(ea.Element);
				CheckDestroyAutomatically();
			}
			if(ea.ChangedType == CollectionChangedType.ElementRemoved) {
				idCollection.Remove(ea.Element);
				UnregisterTile(ea.Element);
				if(!ea.Clear)
					CheckDestroyAutomatically();
			}
			LayoutChanged();
		}
		protected sealed override void ReleaseDeferredControlLoadDocuments() {
		}
		protected sealed override void EnsureDeferredControlLoadDocuments() {
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new ITileContainerInfo Info {
			get { return base.Info as ITileContainerInfo; }
		}
		protected override void NotifyNavigatedTo() {
			WindowsUIView view = GetView();
			if(view != null)
				view.NotifyNavigatedTo();
		}
		protected override void NotifyNavigatedFrom() {
			WindowsUIView view = GetView();
			if(view != null)
				view.NotifyNavigatedFrom();
		}
		protected override void OnActivated() {
			WindowsUIView view = GetView();
			if(view != null)
				view.NavigationBarsShown += OnNavigationBarsShown;
			foreach(BaseTile tile in Items)
				Info.Register(tile);
			RaiseActivated();
		}
		object activated = new object();
		object deactivated = new object();
		event EventHandler ISupportActivation.Activated {
			add { Events.AddHandler(activated, value); }
			remove { Events.AddHandler(activated, value); }
		}
		event EventHandler ISupportActivation.Deactivated {
			add { Events.AddHandler(deactivated, value); }
			remove { Events.AddHandler(deactivated, value); }
		}
		void RaiseActivated() {
			EventHandler handler = Events[activated] as EventHandler;
			if(handler != null) handler(this, EventArgs.Empty);
		}
		void RaiseDeactivated() {
			EventHandler handler = Events[deactivated] as EventHandler;
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected override void OnDeactivated() {
			WindowsUIView view = GetView();
			if(view != null)
				view.NavigationBarsShown -= OnNavigationBarsShown;
			Info.TileControl.ViewInfo.SetHoverInfoCore(TileControlHitInfo.Empty);
			Info.TileControl.ViewInfo.SetPressedInfoCore(TileControlHitInfo.Empty);
			foreach(BaseTile tile in Items)
				Info.Unregister(tile);
			RaiseDeactivated();
		}
		void OnNavigationBarsShown(object sender, EventArgs e) {
			if(Info == null || Info.TileControl == null) return;
			if(Info.TileControl.Handler != null)
				Info.TileControl.Handler.ResetMouseScroll();
		}
		protected override void PatchChildrenCore(Rectangle view, bool active) {
			BaseDocument[] documents = Manager.View.Documents.ToArray();
			foreach(BaseDocument document in documents) {
				if(document.IsControlLoaded) {
					Control child = Manager.GetChild(document);
					child.Location = new Point(view.X - child.Width, view.Y - child.Height);
				}
			}
		}
		protected void RegisterTile(BaseTile tile) {
			if(IsActive)
				Info.Register(tile);
		}
		protected void UnregisterTile(BaseTile tile) {
			if(IsActive)
				Info.Unregister(tile);
		}
		[Browsable(false)]
		public IEnumerable<BaseTile> CheckedTiles {
			get {
				foreach(BaseTile tile in Items) {
					if(tile.Checked.HasValue && tile.Checked.Value)
						yield return tile;
				}
			}
		}
		protected override bool ContainsCore(Document document) {
			foreach(BaseTile tile in Items) {
				Document[] documents = tile.AssociatedDocuments;
				if(Array.IndexOf(documents, document) != -1)
					return true;
			}
			return false;
		}
		protected internal void OnInsert(int index) { }
		protected internal void OnMove(int index, BaseTile element) {
			element.IsItemsOrderInvalid = true;
		}
		protected override IEnumerable<Customization.ISearchObject> GetSearchObjectList() {
			List<Customization.ISearchObject> searchObjectList = new List<Customization.ISearchObject>();
			foreach(Tile ob in this.Items) {
				searchObjectList.Add((Customization.ISearchObject)ob);
				if(ob.AssociatedDocuments != null)
					foreach(var doc in ob.AssociatedDocuments)
						searchObjectList.Add((Customization.ISearchObject)doc);
			}
			return searchObjectList;
		}
		#region events
		static readonly object click = new object();
		static readonly object press = new object();
		static readonly object checkedChanged = new object();
		static readonly object startItemDragging = new object();
		static readonly object endItemDragging = new object();
		[Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		public event TileClickEventHandler Click {
			add { Events.AddHandler(click, value); }
			remove { Events.RemoveHandler(click, value); }
		}
		[Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		public event TileEventHandler Press {
			add { Events.AddHandler(press, value); }
			remove { Events.RemoveHandler(press, value); }
		}
		[Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		public event TileEventHandler CheckedChanged {
			add { Events.AddHandler(checkedChanged, value); }
			remove { Events.RemoveHandler(checkedChanged, value); }
		}
		[Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		public event TileItemDragEventHandler StartItemDragging {
			add { Events.AddHandler(startItemDragging, value); }
			remove { Events.RemoveHandler(startItemDragging, value); }
		}
		[Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		public event TileItemDragEventHandler EndItemDragging {
			add { Events.AddHandler(endItemDragging, value); }
			remove { Events.RemoveHandler(endItemDragging, value); }
		}
		protected internal bool RaiseClick(BaseTile tile) {
			TileClickEventHandler handler = (TileClickEventHandler)Events[click];
			TileClickEventArgs e = new TileClickEventArgs(tile);
			if(handler != null)
				handler(this, e);
			return !e.Handled;
		}
		protected internal void RaisePress(BaseTile tile) {
			TileEventHandler handler = (TileEventHandler)Events[press];
			TileEventArgs e = new TileEventArgs(tile);
			if(handler != null)
				handler(this, e);
		}
		protected internal void RaiseCheckedChanged(BaseTile tile) {
			TileEventHandler handler = (TileEventHandler)Events[checkedChanged];
			if(handler != null)
				handler(this, new TileEventArgs(tile));
		}
		protected internal void RaiseStartItemDragging(BaseTileDragEventArgs e) {
			TileItemDragEventHandler handler = (TileItemDragEventHandler)Events[startItemDragging];
			if(handler != null)
				handler(this, e);
		}
		protected internal void RaiseEndItemDragging(BaseTileDragEventArgs e) {
			TileItemDragEventHandler handler = (TileItemDragEventHandler)Events[endItemDragging];
			if(handler != null)
				handler(this, e);
		}
		protected override void GetActualActionsCore(IList<IContentContainerAction> actions) {
			base.GetActualActionsCore(actions);
			actions.Add(TileContainerAction.TileSizeFlyoutPanel);
			actions.Add(TileContainerAction.ToggleTileSize);
			actions.Add(TileContainerAction.ClearSelection);
		}
		#endregion events
		#region Serialization
		int serializing, deserializing;
		internal int currentGroupIndex;
		IList<SerializableGroupInfo> groupsCore;
		IDictionary<BaseTile, int> cacheIdCollection;
		internal bool IsDeserializing { get { return deserializing > 0; } }
		internal bool IsSerializing { get { return serializing > 0; } }
		public void SaveLayoutToXml(string path) {
			SaveLayoutCore(new XmlXtraSerializer(), path);
		}
		public void RestoreLayoutFromXml(string path) {
			RestoreLayoutCore(new XmlXtraSerializer(), path);
		}
		public void SaveLayoutToStream(System.IO.Stream stream) {
			SaveLayoutCore(new BinaryXtraSerializer(), stream);
		}
		public void RestoreLayoutFromStream(System.IO.Stream stream) {
			RestoreLayoutCore(new BinaryXtraSerializer(), stream);
		}
		public void SaveLayoutToRegistry(string path) {
			SaveLayoutCore(new RegistryXtraSerializer(), path);
		}
		public void RestoreLayoutFromRegistry(string path) {
			RestoreLayoutCore(new RegistryXtraSerializer(), path);
		}
		protected virtual bool SaveLayoutCore(XtraSerializer serializer, object path) {
			using(BatchUpdate.Enter(Manager, true)) {
				using(BatchUpdate.Enter(this, true)) {
					System.IO.Stream stream = path as System.IO.Stream;
					if(stream != null)
						return serializer.SerializeObject(this, stream, "TileControl", null);
					else
						return serializer.SerializeObject(this, path.ToString(), "TileControl", null);
				}
			}
		}
		protected virtual void RestoreLayoutCore(XtraSerializer serializer, object path) {
			using(BatchUpdate.Enter(Manager)) {
				using(BatchUpdate.Enter(this)) {
					System.IO.Stream stream = path as System.IO.Stream;
					if(stream != null)
						serializer.DeserializeObject(this, stream, "TileControl", null);
					else
						serializer.DeserializeObject(this, path.ToString(), "TileControl", null);
				}
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		[XtraSerializableProperty(XtraSerializationVisibility.Collection, true, true, false)]
		public IList<SerializableGroupInfo> Groups { get { return groupsCore; } }
		public class SerializableGroupInfo {
			IList<SerializableItemInfo> itemsCore;
			public SerializableGroupInfo() {
				itemsCore = new List<SerializableItemInfo>();
			}
			public SerializableGroupInfo(string groupName)
				: this() {
				Name = groupName;
			}
			[XtraSerializableProperty]
			public string Text { get; set; }
			[XtraSerializableProperty]
			public string Name { get; set; }
			[XtraSerializableProperty(XtraSerializationVisibility.Collection, true, true, false)]
			public IList<SerializableItemInfo> Items { get { return itemsCore; } }
			protected object XtraFindItemsItem(XtraItemEventArgs e) {
				XtraPropertyInfo nameInfo = e.Item.ChildProperties["Id"];
				foreach(SerializableItemInfo objectInfo in Items) {
					if(Equals(objectInfo.Id, nameInfo.Value))
						return objectInfo;
				}
				return null;
			}
			protected object XtraCreateItemsItem(XtraItemEventArgs e) {
				XtraPropertyInfo nameInfo = e.Item.ChildProperties["Id"];
				SerializableItemInfo res = SerializableItemInfo.CreateSerializableItemInfo(Convert.ToInt32(nameInfo.Value));
				itemsCore.Add(res);
				return res;
			}
			public static SerializableGroupInfo CreateSerializableGroupInfo(string groupName) {
				return new SerializableGroupInfo(groupName);
			}
		}
		public class SerializableItemInfo {
			public SerializableItemInfo() { }
			public SerializableItemInfo(int id) { Id = id; }
			[XtraSerializableProperty]
			public int Id { get; set; }
			public static SerializableItemInfo CreateSerializableItemInfo(int id) {
				return new SerializableItemInfo(id);
			}
		}
		SerializableGroupInfo GetSerializableGroupInfo(string groupName) {
			foreach(var group in Groups)
				if(group.Name == groupName) return group;
			SerializableGroupInfo newGroup = SerializableGroupInfo.CreateSerializableGroupInfo(groupName);
			groupsCore.Add(newGroup);
			return newGroup;
		}
		void AddItemsGroup(BaseTile tile) {
			SerializableGroupInfo group = GetSerializableGroupInfo(tile.Group);
			group.Items.Add(new SerializableItemInfo(GetIDCore(tile)));
		}
		void IXtraSerializable.OnStartSerializing() {
			serializing++;
			BeginSaveLayout();
		}
		void IXtraSerializable.OnEndSerializing() {
			EndSaveLayout();
			serializing--;
		}
		void IXtraSerializable.OnStartDeserializing(DevExpress.Utils.LayoutAllowEventArgs e) {
			deserializing++;
			BeginRestoreLayout();
		}
		void IXtraSerializable.OnEndDeserializing(string restoredVersion) {
			EndRestoreLayout();
			deserializing--;
		}
		protected virtual void BeginSaveLayout() {
			PrepareSerializableObjectInfos();
		}
		protected virtual void EndSaveLayout() {
			ClearSerializableObjectInfos();
		}
		protected virtual void BeginRestoreLayout() {
			currentGroupIndex = 0;
			groupsCore = new List<SerializableGroupInfo>();
			cacheIdCollection = new Dictionary<BaseTile, int>(idCollection);
			BeginUpdate();
			Items.Clear();
		}
		void RestoreNewItems() {
			foreach(var id in cacheIdCollection) {
				Items.Add(id.Key);
				SetIDCore(id.Key, id.Value);
			}
			Ref.Clear(ref cacheIdCollection);
		}
		protected virtual void EndRestoreLayout() {
			foreach(SerializableGroupInfo group in Groups) {
				foreach(SerializableItemInfo item in group.Items) {
					BaseTile tile = RestoreTile(item);
					tile.Group = group.Name;
					SetIDCore(tile, item.Id);
					Items.Add(tile);
				}
			}
			ClearSerializableObjectInfos();
			RestoreNewItems();
			EndUpdate();
		}
		protected BaseTile RestoreTile(SerializableItemInfo item) {
			foreach(var tile in cacheIdCollection) {
				if(tile.Value == item.Id) {
					cacheIdCollection.Remove(tile.Key);
					return tile.Key;
				}
			}
			return ((WindowsUIView)Info.Owner).CreateTile(null);
		}
		protected void PrepareSerializableObjectInfos() {
			groupsCore = new List<SerializableGroupInfo>();
			foreach(BaseTile tile in Items)
				AddItemsGroup(tile);
		}
		protected void ClearSerializableObjectInfos() {
			Ref.Clear(ref groupsCore);
		}
		protected object XtraFindGroupsItem(XtraItemEventArgs e) {
			currentGroupIndex++;
			return null;
		}
		protected object XtraCreateGroupsItem(XtraItemEventArgs e) {
			SerializableGroupInfo res = new SerializableGroupInfo();
			object text = e.Item.ChildProperties["Text"].Value;
			res.Text = text == null ? "" : text.ToString();
			object name = e.Item.ChildProperties["Name"].Value;
			res.Name = name == null ? "" : name.ToString();
			groupsCore.Add(res);
			return res;
		}
		#endregion Serialization
		public void AllowAnimation(BaseTile tile, bool allow) {
			if(tile != null) tile.AllowAnimation = allow;
			if(IsActive) Info.AllowAnimation(tile, allow);
		}
		public void StartAnimation(BaseTile tile) {
			if(IsActive) Info.StartAnimation(tile);
		}
		public void StopAnimation(BaseTile tile) {
			if(IsActive) Info.StopAnimation(tile);
		}
		public void NextFrame(BaseTile tile) {
			if(IsActive) Info.NextFrame(tile);
		}
	}
	public class TileContainerCollection : BaseMutableListEx<TileContainer> {
		public bool Insert(int index, TileContainer container) {
			return InsertCore(index, container);
		}
	}
	public class TileContainerProperties : ContentContainerProperties, ITileContainerProperties {
		public TileContainerProperties() {
			InitContentPropertyCore("AppearanceGroupHighlighting", new GroupHighlightingProperties());
			SetDefaultValueCore("Orientation", Orientation.Horizontal);
			SetDefaultValueCore("RowCount", 5);
			SetDefaultValueCore("ItemSize", TileContainerDefaultProperties.DefaultItemSize);
			SetDefaultValueCore("LargeItemWidth", -1);
			SetDefaultValueCore("IndentBetweenItems", 12);
			SetDefaultValueCore("IndentBetweenGroups", 80);
			SetDefaultValueCore("Padding", Padding.Empty);
			SetDefaultValueCore("ItemPadding", DefaultItemPadding);
			SetDefaultValueCore("ItemCheckMode", TileItemCheckMode.Multiple);
			SetDefaultValueCore("AllowHtmlDraw", true);
			SetDefaultValueCore("AllowDrag", true);
			SetDefaultValueCore("AllowDragTilesBetweenGroups", true);
			SetDefaultValueCore("AllowGroupHighlighting", false);
		}
		[Category("Layout")]
		[DefaultValue(Orientation.Horizontal), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public override Orientation Orientation {
			get { return base.Orientation; }
			set { base.Orientation = value; }
		}
		protected override IBaseProperties CloneCore() {
			return new TileContainerProperties();
		}
		[Category("Layout")]
		[DefaultValue(5), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public int RowCount {
			get { return GetValueCore<int>("RowCount"); }
			set { SetValueCore("RowCount", value); }
		}
		[Category("Layout")]
		[DefaultValue(0), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public int ColumnCount {
			get { return GetValueCore<int>("ColumnCount"); }
			set { SetValueCore("ColumnCount", value); }
		}
		[Category("Layout")]
		[DefaultValue(180), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public int ItemSize {
			get { return GetValueCore<int>("ItemSize"); }
			set { SetValueCore("ItemSize", value); }
		}
		[Category("Layout")]
		[DefaultValue(-1), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public int LargeItemWidth {
			get { return GetValueCore<int>("LargeItemWidth"); }
			set { SetValueCore("LargeItemWidth", value); }
		}
		[Category("Layout")]
		[DefaultValue(12), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public int IndentBetweenItems {
			get { return GetValueCore<int>("IndentBetweenItems"); }
			set { SetValueCore("IndentBetweenItems", value); }
		}
		[Category("Layout")]
		[DefaultValue(80), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public int IndentBetweenGroups {
			get { return GetValueCore<int>("IndentBetweenGroups"); }
			set { SetValueCore("IndentBetweenGroups", value); }
		}
		[Category("Layout")]
		[DefaultValue(ImageLayout.None), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public ImageLayout BackgroundImageLayout {
			get { return GetValueCore<ImageLayout>("BackgroundImageLayout"); }
			set { SetValueCore("BackgroundImageLayout", value); }
		}
		void ResetPadding() { Padding = Padding.Empty; }
		bool ShouldSerializePadding() { return Padding != Padding.Empty; }
		[XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Layout")]
		public Padding Padding {
			get { return GetValueCore<Padding>("Padding"); }
			set { SetValueCore("Padding", value); }
		}
		internal static Padding DefaultItemPadding = new Padding(12, 8, 12, 8);
		bool ShouldSerializeItemPadding() { return !IsDefault("ItemPadding"); }
		void ResetItemPadding() { Reset("ItemPadding"); }
		[XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Layout")]
		public Padding ItemPadding {
			get { return GetValueCore<Padding>("ItemPadding"); }
			set { SetValueCore("ItemPadding", value); }
		}
		[Category(CategoryName.Appearance)]
		[DefaultValue(DevExpress.Utils.HorzAlignment.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public DevExpress.Utils.HorzAlignment HorizontalContentAlignment {
			get { return GetValueCore<DevExpress.Utils.HorzAlignment>("HorizontalContentAlignment"); }
			set { SetValueCore("HorizontalContentAlignment", value); }
		}
		[Category(CategoryName.Appearance)]
		[DefaultValue(DevExpress.Utils.VertAlignment.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public DevExpress.Utils.VertAlignment VerticalContentAlignment {
			get { return GetValueCore<DevExpress.Utils.VertAlignment>("VerticalContentAlignment"); }
			set { SetValueCore("VerticalContentAlignment", value); }
		}
		[Category(CategoryName.Appearance)]
		[DefaultValue(TileItemContentAlignment.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public TileItemContentAlignment ItemImageAlignment {
			get { return GetValueCore<TileItemContentAlignment>("ItemImageAlignment"); }
			set { SetValueCore("ItemImageAlignment", value); }
		}
		[Category(CategoryName.Appearance)]
		[DefaultValue(TileItemContentAlignment.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public TileItemContentAlignment ItemBackgroundImageAlignment {
			get { return GetValueCore<TileItemContentAlignment>("ItemBackgroundImageAlignment"); }
			set { SetValueCore("ItemBackgroundImageAlignment", value); }
		}
		[Category(CategoryName.Appearance)]
		[DefaultValue(TileItemContentAnimationType.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public TileItemContentAnimationType ItemContentAnimation {
			get { return GetValueCore<TileItemContentAnimationType>("ItemContentAnimation"); }
			set { SetValueCore("ItemContentAnimation", value); }
		}
		[Category(CategoryName.Appearance)]
		[DefaultValue(TileItemImageScaleMode.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public TileItemImageScaleMode ItemImageScaleMode {
			get { return GetValueCore<TileItemImageScaleMode>("ItemImageScaleMode"); }
			set { SetValueCore("ItemImageScaleMode", value); }
		}
		[Category(CategoryName.Appearance)]
		[DefaultValue(TileItemImageScaleMode.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public TileItemImageScaleMode ItemBackgroundImageScaleMode {
			get { return GetValueCore<TileItemImageScaleMode>("ItemBackgroundImageScaleMode"); }
			set { SetValueCore("ItemBackgroundImageScaleMode", value); }
		}
		[Category(CategoryName.Behavior)]
		[DefaultValue(TileItemCheckMode.Multiple), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public TileItemCheckMode ItemCheckMode {
			get { return GetValueCore<TileItemCheckMode>("ItemCheckMode"); }
			set { SetValueCore("ItemCheckMode", value); }
		}
		[Category(CategoryName.Behavior)]
		[DefaultValue(TileItemContentShowMode.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public TileItemContentShowMode ItemTextShowMode {
			get { return GetValueCore<TileItemContentShowMode>("ItemTextShowMode"); }
			set { SetValueCore("ItemTextShowMode", value); }
		}
		[Category(CategoryName.Appearance)]
		[DefaultValue(true), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public bool AllowHtmlDraw {
			get { return GetValueCore<bool>("AllowHtmlDraw"); }
			set { SetValueCore("AllowHtmlDraw", value); }
		}
		[Category(CategoryName.Behavior)]
		[DefaultValue(false), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public bool AllowItemHover {
			get { return GetValueCore<bool>("AllowItemHover"); }
			set { SetValueCore("AllowItemHover", value); }
		}
		[Category(CategoryName.Behavior)]
		[DefaultValue(false), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public bool AllowSelectedItem {
			get { return GetValueCore<bool>("AllowSelectedItem"); }
			set { SetValueCore("AllowSelectedItem", value); }
		}
		[Category(CategoryName.Behavior)]
		[DefaultValue(true), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public bool AllowDrag {
			get { return GetValueCore<bool>("AllowDrag"); }
			set { SetValueCore("AllowDrag", value); }
		}
		[Category(CategoryName.Behavior)]
		[DefaultValue(true), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public bool AllowDragTilesBetweenGroups {
			get { return GetValueCore<bool>("AllowDragTilesBetweenGroups"); }
			set { SetValueCore("AllowDragTilesBetweenGroups", value); }
		}
		[Category(CategoryName.Appearance)]
		[DefaultValue(false), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public bool ShowText {
			get { return GetValueCore<bool>("ShowText"); }
			set { SetValueCore("ShowText", value); }
		}
		[Category(CategoryName.Appearance)]
		[DefaultValue(false), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public bool ShowGroupText {
			get { return GetValueCore<bool>("ShowGroupText"); }
			set { SetValueCore("ShowGroupText", value); }
		}
		[Category(CategoryName.Appearance)]
		[DefaultValue(false), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public bool AllowGroupHighlighting {
			get { return GetValueCore<bool>("AllowGroupHighlighting"); }
			set { SetValueCore("AllowGroupHighlighting", value); }
		}
		[Category(CategoryName.Appearance)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public GroupHighlightingProperties AppearanceGroupHighlighting {
			get { return GetContentCore<GroupHighlightingProperties>("AppearanceGroupHighlighting"); }
			set { SetContentCore("AppearanceGroupHighlighting", value); }
		}
		void ResetAppearanceGroupHighlighting() {
			AppearanceGroupHighlighting.Reset();
			PropertiesChanged("AppearanceGroupHighlighting");
		}
		bool ShouldSerializeAppearanceGroupHighlighting() {
			return AppearanceGroupHighlighting.ShouldSerialize();
		}
		[Category(CategoryName.Appearance)]
		[DefaultValue(TileItemBorderVisibility.Auto), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public TileItemBorderVisibility ItemBorderVisibility {
			get { return GetValueCore<TileItemBorderVisibility>("ItemBorderVisibility"); }
			set { SetValueCore("ItemBorderVisibility", value); }
		}
		public void Assign(ITileControlProperties source) {
			BeginUpdate();
			AllowDrag = source.AllowDrag;
			AllowHtmlDraw = source.AllowHtmlDraw;
			AllowItemHover = source.AllowItemHover;
			AllowSelectedItem = source.AllowSelectedItem;
			BackgroundImageLayout = source.BackgroundImageLayout;
			HorizontalContentAlignment = source.HorizontalContentAlignment;
			IndentBetweenGroups = source.IndentBetweenGroups;
			IndentBetweenItems = source.IndentBetweenItems;
			ItemBackgroundImageAlignment = source.ItemBackgroundImageAlignment;
			ItemBackgroundImageScaleMode = source.ItemBackgroundImageScaleMode;
			ItemCheckMode = source.ItemCheckMode;
			ItemContentAnimation = source.ItemContentAnimation;
			ItemImageAlignment = source.ItemImageAlignment;
			ItemImageScaleMode = source.ItemImageScaleMode;
			ItemPadding = source.ItemPadding;
			ItemSize = source.ItemSize;
			ItemTextShowMode = source.ItemTextShowMode;
			ItemBorderVisibility = source.ItemBorderVisibility;
			LargeItemWidth = source.LargeItemWidth;
			Orientation = source.Orientation;
			Padding = source.Padding;
			RowCount = source.RowCount;
			ColumnCount = source.ColumnCount;
			ShowGroupText = source.ShowGroupText;
			AllowGroupHighlighting = source.AllowGroupHighlighting;
			AppearanceGroupHighlighting.Assign(source.AppearanceGroupHighlighting);
			ShowText = source.ShowText;
			VerticalContentAlignment = source.VerticalContentAlignment;
			CancelUpdate();
		}
	}
	public class TileContainerDefaultProperties : ContentContainerDefaultProperties, ITileContainerDefaultProperties {
		public static int DefaultItemSize = 180;
		public TileContainerDefaultProperties(ITileContainerProperties parentProperties)
			: base(parentProperties) {
			InitContentPropertyCore("AppearanceGroupHighlighting", new GroupHighlightingProperties());
			SetDefaultValueCore("AllowHtmlDraw", DevExpress.Utils.DefaultBoolean.Default);
			SetDefaultValueCore("AllowItemHover", DevExpress.Utils.DefaultBoolean.Default);
			SetDefaultValueCore("AllowSelectedItem", DevExpress.Utils.DefaultBoolean.Default);
			SetDefaultValueCore("AllowDrag", DevExpress.Utils.DefaultBoolean.Default);
			SetDefaultValueCore("AllowDragTilesBetweenGroups", DevExpress.Utils.DefaultBoolean.Default);
			SetDefaultValueCore("ShowText", DevExpress.Utils.DefaultBoolean.Default);
			SetDefaultValueCore("ShowGroupText", DevExpress.Utils.DefaultBoolean.Default);
			SetDefaultValueCore("AllowGroupHighlighting", DevExpress.Utils.DefaultBoolean.Default);
			SetConverter("RowCount", GetNullableValueConverter(5));
			SetConverter("ItemSize", GetNullableValueConverter(DefaultItemSize));
			SetConverter("LargeItemWidth", GetNullableValueConverter(-1));
			SetConverter("IndentBetweenItems", GetNullableValueConverter(12));
			SetConverter("IndentBetweenGroups", GetNullableValueConverter(80));
			SetConverter("ItemPadding", GetNullableValueConverter(TileContainerProperties.DefaultItemPadding));
			SetConverter("ItemCheckMode", GetNullableValueConverter(TileItemCheckMode.Multiple));
			SetConverter("AllowHtmlDraw", GetDefaultBooleanConverter(true));
			SetConverter("AllowItemHover", GetDefaultBooleanConverter(false));
			SetConverter("AllowSelectedItem", GetDefaultBooleanConverter(false));
			SetConverter("AllowDrag", GetDefaultBooleanConverter(true));
			SetConverter("AllowDragTilesBetweenGroups", GetDefaultBooleanConverter(true));
			SetConverter("ShowText", GetDefaultBooleanConverter(false));
			SetConverter("ShowGroupText", GetDefaultBooleanConverter(false));
			SetConverter("AllowGroupHighlighting", GetDefaultBooleanConverter(false));
		}
		protected override IBaseProperties CloneCore() {
			return new TileContainerDefaultProperties(ParentProperties as ITileContainerProperties);
		}
		[DefaultValue(null), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Layout")]
		[TypeConverter(typeof(DevExpress.Utils.Design.NullableTypeConverter))]
		public int? RowCount {
			get { return GetValueCore<int?>("RowCount"); }
			set { SetValueCore("RowCount", value); }
		}
		[DefaultValue(null), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Layout")]
		[TypeConverter(typeof(DevExpress.Utils.Design.NullableTypeConverter))]
		public int? ColumnCount {
			get { return GetValueCore<int?>("ColumnCount"); }
			set { SetValueCore("ColumnCount", value); }
		}
		[DefaultValue(null), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Layout")]
		[TypeConverter(typeof(DevExpress.Utils.Design.NullableTypeConverter))]
		public int? ItemSize {
			get { return GetValueCore<int?>("ItemSize"); }
			set { SetValueCore("ItemSize", value); }
		}
		[DefaultValue(null), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Layout")]
		[TypeConverter(typeof(DevExpress.Utils.Design.NullableTypeConverter))]
		public int? LargeItemWidth {
			get { return GetValueCore<int?>("LargeItemWidth"); }
			set { SetValueCore("LargeItemWidth", value); }
		}
		[DefaultValue(null), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Layout")]
		[TypeConverter(typeof(DevExpress.Utils.Design.NullableTypeConverter))]
		public int? IndentBetweenItems {
			get { return GetValueCore<int?>("IndentBetweenItems"); }
			set { SetValueCore("IndentBetweenItems", value); }
		}
		[DefaultValue(null), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Layout")]
		[TypeConverter(typeof(DevExpress.Utils.Design.NullableTypeConverter))]
		public int? IndentBetweenGroups {
			get { return GetValueCore<int?>("IndentBetweenGroups"); }
			set { SetValueCore("IndentBetweenGroups", value); }
		}
		[DefaultValue(null), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Layout")]
		[TypeConverter(typeof(DevExpress.Utils.Design.NullableTypeConverter))]
		public Padding? Padding {
			get { return GetValueCore<Padding?>("Padding"); }
			set { SetValueCore("Padding", value); }
		}
		[DefaultValue(null), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Layout")]
		[TypeConverter(typeof(DevExpress.Utils.Design.NullableTypeConverter))]
		public Padding? ItemPadding {
			get { return GetValueCore<Padding?>("ItemPadding"); }
			set { SetValueCore("ItemPadding", value); }
		}
		[DefaultValue(null), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category(CategoryName.Appearance)]
		[TypeConverter(typeof(DevExpress.Utils.Design.NullableTypeConverter))]
		public ImageLayout? BackgroundImageLayout {
			get { return GetValueCore<ImageLayout?>("BackgroundImageLayout"); }
			set { SetValueCore("BackgroundImageLayout", value); }
		}
		[DefaultValue(null), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category(CategoryName.Appearance)]
		[TypeConverter(typeof(DevExpress.Utils.Design.NullableTypeConverter))]
		public TileItemContentAlignment? ItemImageAlignment {
			get { return GetValueCore<TileItemContentAlignment?>("ItemImageAlignment"); }
			set { SetValueCore("ItemImageAlignment", value); }
		}
		[DefaultValue(null), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category(CategoryName.Appearance)]
		[TypeConverter(typeof(DevExpress.Utils.Design.NullableTypeConverter))]
		public TileItemContentAlignment? ItemBackgroundImageAlignment {
			get { return GetValueCore<TileItemContentAlignment?>("ItemBackgroundImageAlignment"); }
			set { SetValueCore("ItemBackgroundImageAlignment", value); }
		}
		[DefaultValue(null), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category(CategoryName.Appearance)]
		[TypeConverter(typeof(DevExpress.Utils.Design.NullableTypeConverter))]
		public DevExpress.Utils.HorzAlignment? HorizontalContentAlignment {
			get { return GetValueCore<DevExpress.Utils.HorzAlignment?>("HorizontalContentAlignment"); }
			set { SetValueCore("HorizontalContentAlignment", value); }
		}
		[DefaultValue(null), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category(CategoryName.Appearance)]
		[TypeConverter(typeof(DevExpress.Utils.Design.NullableTypeConverter))]
		public DevExpress.Utils.VertAlignment? VerticalContentAlignment {
			get { return GetValueCore<DevExpress.Utils.VertAlignment?>("VerticalContentAlignment"); }
			set { SetValueCore("VerticalContentAlignment", value); }
		}
		[DefaultValue(null), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category(CategoryName.Appearance)]
		[TypeConverter(typeof(DevExpress.Utils.Design.NullableTypeConverter))]
		public TileItemImageScaleMode? ItemImageScaleMode {
			get { return GetValueCore<TileItemImageScaleMode?>("ItemImageScaleMode"); }
			set { SetValueCore("ItemImageScaleMode", value); }
		}
		[DefaultValue(null), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category(CategoryName.Appearance)]
		[TypeConverter(typeof(DevExpress.Utils.Design.NullableTypeConverter))]
		public TileItemImageScaleMode? ItemBackgroundImageScaleMode {
			get { return GetValueCore<TileItemImageScaleMode?>("ItemBackgroundImageScaleMode"); }
			set { SetValueCore("ItemBackgroundImageScaleMode", value); }
		}
		[DefaultValue(null), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category(CategoryName.Appearance)]
		[TypeConverter(typeof(DevExpress.Utils.Design.NullableTypeConverter))]
		public TileItemContentAnimationType? ItemContentAnimation {
			get { return GetValueCore<TileItemContentAnimationType?>("ItemContentAnimation"); }
			set { SetValueCore("ItemContentAnimation", value); }
		}
		[DefaultValue(null), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category(CategoryName.Appearance)]
		[TypeConverter(typeof(DevExpress.Utils.Design.NullableTypeConverter))]
		public TileItemBorderVisibility? ItemBorderVisibility {
			get { return GetValueCore<TileItemBorderVisibility?>("ItemBorderVisibility"); }
			set { SetValueCore("ItemBorderVisibility", value); }
		}
		[DefaultValue(null), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category(CategoryName.Behavior), SmartTagProperty("Item Check Mode", "")]
		[TypeConverter(typeof(DevExpress.Utils.Design.NullableTypeConverter))]
		public TileItemCheckMode? ItemCheckMode {
			get { return GetValueCore<TileItemCheckMode?>("ItemCheckMode"); }
			set { SetValueCore("ItemCheckMode", value); }
		}
		[DefaultValue(null), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category(CategoryName.Behavior), SmartTagProperty("Item Text Show Mode", "")]
		[TypeConverter(typeof(DevExpress.Utils.Design.NullableTypeConverter))]
		public TileItemContentShowMode? ItemTextShowMode {
			get { return GetValueCore<TileItemContentShowMode?>("ItemTextShowMode"); }
			set { SetValueCore("ItemTextShowMode", value); }
		}
		[Category(CategoryName.Appearance)]
		[DefaultValue(DevExpress.Utils.DefaultBoolean.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public DevExpress.Utils.DefaultBoolean AllowHtmlDraw {
			get { return GetValueCore<DevExpress.Utils.DefaultBoolean>("AllowHtmlDraw"); }
			set { SetValueCore("AllowHtmlDraw", value); }
		}
		[Category(CategoryName.Behavior)]
		[DefaultValue(DevExpress.Utils.DefaultBoolean.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public DevExpress.Utils.DefaultBoolean AllowItemHover {
			get { return GetValueCore<DevExpress.Utils.DefaultBoolean>("AllowItemHover"); }
			set { SetValueCore("AllowItemHover", value); }
		}
		[Category(CategoryName.Behavior)]
		[DefaultValue(DevExpress.Utils.DefaultBoolean.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public DevExpress.Utils.DefaultBoolean AllowSelectedItem {
			get { return GetValueCore<DevExpress.Utils.DefaultBoolean>("AllowSelectedItem"); }
			set { SetValueCore("AllowSelectedItem", value); }
		}
		[Category(CategoryName.Behavior)]
		[DefaultValue(DevExpress.Utils.DefaultBoolean.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public DevExpress.Utils.DefaultBoolean AllowDrag {
			get { return GetValueCore<DevExpress.Utils.DefaultBoolean>("AllowDrag"); }
			set { SetValueCore("AllowDrag", value); }
		}
		[Category(CategoryName.Behavior)]
		[DefaultValue(DevExpress.Utils.DefaultBoolean.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public DevExpress.Utils.DefaultBoolean AllowDragTilesBetweenGroups {
			get { return GetValueCore<DevExpress.Utils.DefaultBoolean>("AllowDragTilesBetweenGroups"); }
			set { SetValueCore("AllowDragTilesBetweenGroups", value); }
		}
		[Category(CategoryName.Appearance)]
		[DefaultValue(DevExpress.Utils.DefaultBoolean.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), SmartTagProperty("Show Text", "")]
		public DevExpress.Utils.DefaultBoolean ShowText {
			get { return GetValueCore<DevExpress.Utils.DefaultBoolean>("ShowText"); }
			set { SetValueCore("ShowText", value); }
		}
		[Category(CategoryName.Appearance)]
		[DefaultValue(DevExpress.Utils.DefaultBoolean.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), SmartTagProperty("Show Group Text", "")]
		public DevExpress.Utils.DefaultBoolean ShowGroupText {
			get { return GetValueCore<DevExpress.Utils.DefaultBoolean>("ShowGroupText"); }
			set { SetValueCore("ShowGroupText", value); }
		}
		[Category(CategoryName.Appearance)]
		[DefaultValue(DevExpress.Utils.DefaultBoolean.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public DevExpress.Utils.DefaultBoolean AllowGroupHighlighting {
			get { return GetValueCore<DevExpress.Utils.DefaultBoolean>("AllowGroupHighlighting"); }
			set { SetValueCore("AllowGroupHighlighting", value); }
		}
		[Category(CategoryName.Appearance)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public GroupHighlightingProperties AppearanceGroupHighlighting {
			get { return GetContentCore<GroupHighlightingProperties>("AppearanceGroupHighlighting"); }
			set { SetContentCore("AppearanceGroupHighlighting", value); }
		}
		void ResetAppearanceGroupHighlighting() {
			AppearanceGroupHighlighting.Reset();
			PropertiesChanged("AppearanceGroupHighlighting");
		}
		bool ShouldSerializeAppearanceGroupHighlighting() {
			return AppearanceGroupHighlighting.ShouldSerialize();
		}
		[Browsable(false)]
		public int ActualRowCount {
			get { return GetActualValueFromNullable<int>("RowCount"); }
		}
		[Browsable(false)]
		public int ActualColumnCount {
			get { return GetActualValueFromNullable<int>("ColumnCount"); }
		}
		[Browsable(false)]
		public int ActualItemSize {
			get { return GetActualValueFromNullable<int>("ItemSize"); }
		}
		[Browsable(false)]
		public int ActualLargeItemWidth {
			get { return GetActualValueFromNullable<int>("LargeItemWidth"); }
		}
		[Browsable(false)]
		public int ActualIndentBetweenItems {
			get { return GetActualValueFromNullable<int>("IndentBetweenItems"); }
		}
		[Browsable(false)]
		public int ActualIndentBetweenGroups {
			get { return GetActualValueFromNullable<int>("IndentBetweenGroups"); }
		}
		[Browsable(false)]
		public Padding ActualPadding {
			get { return GetActualValueFromNullable<Padding>("Padding"); }
		}
		[Browsable(false)]
		public Padding ActualItemPadding {
			get { return GetActualValueFromNullable<Padding>("ItemPadding"); }
		}
		[Browsable(false)]
		public ImageLayout ActualBackgroundImageLayout {
			get { return GetActualValueFromNullable<ImageLayout>("BackgroundImageLayout"); }
		}
		[Browsable(false)]
		public TileItemContentAlignment ActualItemImageAlignment {
			get { return GetActualValueFromNullable<TileItemContentAlignment>("ItemImageAlignment"); }
		}
		[Browsable(false)]
		public TileItemContentAlignment ActualItemBackgroundImageAlignment {
			get { return GetActualValueFromNullable<TileItemContentAlignment>("ItemBackgroundImageAlignment"); }
		}
		[Browsable(false)]
		public DevExpress.Utils.HorzAlignment ActualHorizontalContentAlignment {
			get { return GetActualValueFromNullable<DevExpress.Utils.HorzAlignment>("HorizontalContentAlignment"); }
		}
		[Browsable(false)]
		public DevExpress.Utils.VertAlignment ActualVerticalContentAlignment {
			get { return GetActualValueFromNullable<DevExpress.Utils.VertAlignment>("VerticalContentAlignment"); }
		}
		[Browsable(false)]
		public TileItemImageScaleMode ActualItemImageScaleMode {
			get { return GetActualValueFromNullable<TileItemImageScaleMode>("ItemImageScaleMode"); }
		}
		[Browsable(false)]
		public TileItemImageScaleMode ActualItemBackgroundImageScaleMode {
			get { return GetActualValueFromNullable<TileItemImageScaleMode>("ItemBackgroundImageScaleMode"); }
		}
		[Browsable(false)]
		public TileItemContentAnimationType ActualItemContentAnimation {
			get { return GetActualValueFromNullable<TileItemContentAnimationType>("ItemContentAnimation"); }
		}
		[Browsable(false)]
		public TileItemBorderVisibility ActualItemBorderVisibility {
			get { return GetActualValueFromNullable<TileItemBorderVisibility>("ItemBorderVisibility"); }
		}
		[Browsable(false)]
		public TileItemCheckMode ActualItemCheckMode {
			get { return GetActualValueFromNullable<TileItemCheckMode>("ItemCheckMode"); }
		}
		[Browsable(false)]
		public TileItemContentShowMode ActualItemTextShowMode {
			get { return GetActualValueFromNullable<TileItemContentShowMode>("ItemTextShowMode"); }
		}
		[Browsable(false)]
		public bool CanHtmlDraw {
			get { return GetActualValue<DevExpress.Utils.DefaultBoolean, bool>("AllowHtmlDraw"); }
		}
		[Browsable(false)]
		public bool CanItemHover {
			get { return GetActualValue<DevExpress.Utils.DefaultBoolean, bool>("AllowItemHover"); }
		}
		[Browsable(false)]
		public bool CanDrag {
			get { return GetActualValue<DevExpress.Utils.DefaultBoolean, bool>("AllowDrag"); }
		}
		[Browsable(false)]
		public bool CanDragTilesBetweenGroups {
			get { return GetActualValue<DevExpress.Utils.DefaultBoolean, bool>("AllowDragTilesBetweenGroups"); }
		}
		[Browsable(false)]
		public bool ActualAllowSelectedItem {
			get { return GetActualValue<DevExpress.Utils.DefaultBoolean, bool>("AllowSelectedItem"); }
		}
		[Browsable(false)]
		public bool CanShowText {
			get { return GetActualValue<DevExpress.Utils.DefaultBoolean, bool>("ShowText"); }
		}
		[Browsable(false)]
		public bool CanShowGroupText {
			get { return GetActualValue<DevExpress.Utils.DefaultBoolean, bool>("ShowGroupText"); }
		}
		[Browsable(false)]
		public bool CanGroupHighlighting {
			get { return GetActualValue<DevExpress.Utils.DefaultBoolean, bool>("AllowGroupHighlighting"); }
		}
		[Browsable(false)]
		public GroupHighlightingProperties ActualAppearanceGroupHighlighting {
			get { return GetActualAppearanceGroupHighlighting(); }
		}
		protected GroupHighlightingProperties GetActualAppearanceGroupHighlighting() {
			GroupHighlightingProperties result = AppearanceGroupHighlighting.Clone() as GroupHighlightingProperties;
			if(ParentProperties != null)
				result.Combine(ParentProperties.GetContent<GroupHighlightingProperties>("AppearanceGroupHighlighting"));
			return result;
		}
		[Browsable(false)]
		public ITileControlProperties ActualProperties {
			get { return new ActualTileControlProperties(this); }
		}
		class ActualTileControlProperties : ITileControlProperties {
			ITileContainerDefaultProperties ownerProperties;
			internal ActualTileControlProperties(ITileContainerDefaultProperties properties) {
				ownerProperties = properties;
			}
			public int RowCount {
				get { return ownerProperties.ActualRowCount; }
				set { ownerProperties.RowCount = GetNullableValue(value, 5); }
			}
			public int ColumnCount {
				get { return ownerProperties.ActualColumnCount; }
				set { ownerProperties.ColumnCount = GetNullableValue(value, 0); }
			}
			public int ItemSize {
				get { return ownerProperties.ActualItemSize; }
				set { ownerProperties.ItemSize = GetNullableValue(value, DefaultItemSize); }
			}
			public int LargeItemWidth {
				get { return ownerProperties.ActualLargeItemWidth; }
				set { ownerProperties.LargeItemWidth = GetNullableValue(value, -1); }
			}
			public int IndentBetweenItems {
				get { return ownerProperties.ActualIndentBetweenItems; }
				set { ownerProperties.IndentBetweenItems = GetNullableValue(value, 12); }
			}
			public int IndentBetweenGroups {
				get { return ownerProperties.ActualIndentBetweenGroups; }
				set { ownerProperties.IndentBetweenGroups = GetNullableValue(value, 80); }
			}
			public Orientation Orientation {
				get { return ownerProperties.ActualOrientation; }
				set { ownerProperties.Orientation = GetNullableValue(value, Orientation.Horizontal); }
			}
			public DevExpress.Utils.HorzAlignment HorizontalContentAlignment {
				get { return ownerProperties.ActualHorizontalContentAlignment; }
				set { ownerProperties.HorizontalContentAlignment = GetNullableValue(value, DevExpress.Utils.HorzAlignment.Default); }
			}
			public DevExpress.Utils.VertAlignment VerticalContentAlignment {
				get { return ownerProperties.ActualVerticalContentAlignment; }
				set { ownerProperties.VerticalContentAlignment = GetNullableValue(value, DevExpress.Utils.VertAlignment.Default); }
			}
			public Padding Padding {
				get { return ownerProperties.ActualPadding; }
				set { ownerProperties.Padding = GetNullableValue(value, Padding.Empty); }
			}
			public Padding ItemPadding {
				get { return ownerProperties.ActualItemPadding; }
				set { ownerProperties.ItemPadding = GetNullableValue(value, TileContainerProperties.DefaultItemPadding); }
			}
			public ImageLayout BackgroundImageLayout {
				get { return ownerProperties.ActualBackgroundImageLayout; }
				set { ownerProperties.BackgroundImageLayout = GetNullableValue(value, ImageLayout.None); }
			}
			public TileItemContentAlignment ItemImageAlignment {
				get { return ownerProperties.ActualItemImageAlignment; }
				set { ownerProperties.ItemImageAlignment = GetNullableValue(value, TileItemContentAlignment.Default); }
			}
			public TileItemContentAlignment ItemBackgroundImageAlignment {
				get { return ownerProperties.ActualItemBackgroundImageAlignment; }
				set { ownerProperties.ItemBackgroundImageAlignment = GetNullableValue(value, TileItemContentAlignment.Default); }
			}
			public TileItemImageScaleMode ItemImageScaleMode {
				get { return ownerProperties.ActualItemImageScaleMode; }
				set { ownerProperties.ItemImageScaleMode = GetNullableValue(value, TileItemImageScaleMode.Default); }
			}
			public TileItemImageScaleMode ItemBackgroundImageScaleMode {
				get { return ownerProperties.ActualItemBackgroundImageScaleMode; }
				set { ownerProperties.ItemBackgroundImageScaleMode = GetNullableValue(value, TileItemImageScaleMode.Default); }
			}
			public TileItemContentAnimationType ItemContentAnimation {
				get { return ownerProperties.ActualItemContentAnimation; }
				set { ownerProperties.ItemContentAnimation = GetNullableValue(value, TileItemContentAnimationType.Default); }
			}
			public TileItemContentShowMode ItemTextShowMode {
				get { return ownerProperties.ActualItemTextShowMode; }
				set { ownerProperties.ItemTextShowMode = GetNullableValue(value, TileItemContentShowMode.Default); }
			}
			public TileItemCheckMode ItemCheckMode {
				get { return ownerProperties.ActualItemCheckMode; }
				set { ownerProperties.ItemCheckMode = GetNullableValue(value, TileItemCheckMode.Multiple); }
			}
			public bool AllowHtmlDraw {
				get { return ownerProperties.CanHtmlDraw; }
				set { ownerProperties.AllowHtmlDraw = value ? DevExpress.Utils.DefaultBoolean.Default : DevExpress.Utils.DefaultBoolean.False; }
			}
			public bool AllowItemHover {
				get { return ownerProperties.CanItemHover; }
				set { ownerProperties.AllowItemHover = !value ? DevExpress.Utils.DefaultBoolean.Default : DevExpress.Utils.DefaultBoolean.True; }
			}
			public bool AllowSelectedItem {
				get { return ownerProperties.ActualAllowSelectedItem; }
				set { ownerProperties.AllowSelectedItem = !value ? DevExpress.Utils.DefaultBoolean.Default : DevExpress.Utils.DefaultBoolean.True; }
			}
			public bool AllowDrag {
				get { return ownerProperties.CanDrag; }
				set { ownerProperties.AllowDrag = value ? DevExpress.Utils.DefaultBoolean.Default : DevExpress.Utils.DefaultBoolean.False; }
			}
			public bool ShowText {
				get { return ownerProperties.CanShowText; }
				set { ownerProperties.ShowText = !value ? DevExpress.Utils.DefaultBoolean.Default : DevExpress.Utils.DefaultBoolean.True; }
			}
			public bool ShowGroupText {
				get { return ownerProperties.CanShowGroupText; }
				set { ownerProperties.ShowGroupText = !value ? DevExpress.Utils.DefaultBoolean.Default : DevExpress.Utils.DefaultBoolean.True; }
			}
			public bool AllowGroupHighlighting {
				get { return ownerProperties.CanGroupHighlighting; }
				set { ownerProperties.AllowGroupHighlighting = !value ? DevExpress.Utils.DefaultBoolean.Default : DevExpress.Utils.DefaultBoolean.True; }
			}
			public GroupHighlightingProperties AppearanceGroupHighlighting {
				get { return ownerProperties.ActualAppearanceGroupHighlighting; }
				set { ownerProperties.AppearanceGroupHighlighting = value; }
			}
			public TileItemBorderVisibility ItemBorderVisibility {
				get { return ownerProperties.ActualItemBorderVisibility; }
				set { ownerProperties.ItemBorderVisibility = GetNullableValue(value, TileItemBorderVisibility.Auto); }
			}
			public void Assign(ITileControlProperties source) { }
			static Nullable<T> GetNullableValue<T>(T value, T defaultValue) where T : struct {
				return object.Equals(value, defaultValue) ? (T?)null : new Nullable<T>(value);
			}
		}
	}
	public abstract class TileContainerAction : ContentContainerAction {
		#region static
		public static readonly IContentContainerAction TileSizeFlyoutPanel = new TileSizeFlyoutPanelAction();
		public static readonly IContentContainerAction ToggleTileSize = new ToggleTileSizeAction();
		public static readonly IContentContainerAction ClearSelection = new ClearSelectionAction();
		#endregion static
		static bool All(IEnumerable<BaseTile> tiles, Predicate<BaseTile> match) {
			int matchesCount = 0, count = 0;
			foreach(BaseTile tile in tiles) {
				if(match(tile))
					matchesCount++;
				count++;
			}
			return count > 0 && matchesCount != 0;
		}
		[ActionGroup("TileContainer", ActionType.Context, Order = 1, Index = 0, Edge = ActionEdge.Left, Behavior = ActionBehavior.UpdateBarOnClick)]
		abstract class TileResizeAction : TileContainerAction {
			protected override void ExecuteCore(WindowsUIView view, IContentContainer container) {
				TileContainer tileContainer = container as TileContainer;
				((IWindowsUIViewControllerInternal)view.Controller).ProcessCheckedTiles(tileContainer, ChangeTileSize);
			}
			bool ChangeTileSize(BaseTile tile) {
				if(!ChangeTileSizeCore(tile)) return false;
				if(tile.Manager != null && tile.Manager.View != null)
					tile.Manager.View.SetLayoutModified();
				return true;
			}
			protected abstract bool ChangeTileSizeCore(BaseTile tile);
		}
		[ActionGroup("TileContainer", ActionType.Context, Order = 0, Index = 0, Edge = ActionEdge.Left)]
		class TileSizeFlyoutPanelAction : ContentContainerPopupMenuAction {
			public TileSizeFlyoutPanelAction()
				: base() { }
			protected override bool CanExecuteCore(IContentContainer container) {
				TileContainer tileContainer = container as TileContainer;
				foreach(BaseTile tile in tileContainer.CheckedTiles)
					return true;
				return false;
			}
			protected override DocumentManagerStringId ID {
				get { return DocumentManagerStringId.CommandTileSizeFlyoutPanel; }
			}
			protected override DocumentManagerStringId DescriptionID {
				get { return DocumentManagerStringId.CommandTileSizeFlyoutPanelDescription; }
			}
			protected override IList<IUIAction<IContentContainer>> GetActions() {
				return new IContentContainerAction[] { new TileLargeSizeAction(), new TileSmallSizeAction(), new TileMediumSizeAction(), new TileWideSizeAction() };
			}
			protected override Orientation GetOrientation() { return Orientation.Vertical; }
		}
		class TileLargeSizeAction : TileResizeAction {
			protected override DocumentManagerStringId ID {
				get { return DocumentManagerStringId.CommandTileLargeSize; }
			}
			protected override DocumentManagerStringId DescriptionID {
				get { return DocumentManagerStringId.CommandTileLargeSizeDescription; }
			}
			protected override bool CanExecuteCore(IContentContainer container) {
				TileContainer tileContainer = container as TileContainer;
				return All(tileContainer.CheckedTiles, (tile) => !tile.Properties.ActualIsLarge);
			}
			protected override bool ChangeTileSizeCore(BaseTile tile) {
				if(tile.Properties.ActualItemSize == TileItemSize.Large) return false;
				tile.Properties.ItemSize = TileItemSize.Large;
				return true;
			}
		}
		class TileSmallSizeAction : TileResizeAction {
			protected override DocumentManagerStringId ID {
				get { return DocumentManagerStringId.CommandTileSmallSize; }
			}
			protected override DocumentManagerStringId DescriptionID {
				get { return DocumentManagerStringId.CommandTileSmallSizeDescription; }
			}
			protected override bool CanExecuteCore(IContentContainer container) {
				TileContainer tileContainer = container as TileContainer;
				return All(tileContainer.CheckedTiles, (tile) => tile.Properties.ActualItemSize != TileItemSize.Small);
			}
			protected override bool ChangeTileSizeCore(BaseTile tile) {
				if(tile.Properties.ActualItemSize == TileItemSize.Small) return false;
				tile.Properties.ItemSize = TileItemSize.Small;
				return true;
			}
		}
		class TileWideSizeAction : TileResizeAction {
			protected override DocumentManagerStringId ID {
				get { return DocumentManagerStringId.CommandTileWideSize; }
			}
			protected override DocumentManagerStringId DescriptionID {
				get { return DocumentManagerStringId.CommandTileWideSizeDescription; }
			}
			protected override bool CanExecuteCore(IContentContainer container) {
				TileContainer tileContainer = container as TileContainer;
				return All(tileContainer.CheckedTiles, (tile) => !tile.Properties.ActualIsWide);
			}
			protected override bool ChangeTileSizeCore(BaseTile tile) {
				if(tile.Properties.ActualItemSize == TileItemSize.Wide) return false;
				tile.Properties.ItemSize = TileItemSize.Wide;
				return true;
			}
		}
		class TileMediumSizeAction : TileResizeAction {
			protected override DocumentManagerStringId ID {
				get { return DocumentManagerStringId.CommandTileMediumSize; }
			}
			protected override DocumentManagerStringId DescriptionID {
				get { return DocumentManagerStringId.CommandTileMediumSizeDescription; }
			}
			protected override bool CanExecuteCore(IContentContainer container) {
				TileContainer tileContainer = container as TileContainer;
				return All(tileContainer.CheckedTiles, (tile) => !tile.Properties.ActualIsMedium);
			}
			protected override bool ChangeTileSizeCore(BaseTile tile) {
				if(tile.Properties.ActualItemSize == TileItemSize.Medium) return false;
				tile.Properties.ItemSize = TileItemSize.Medium;
				return true;
			}
		}
		class ToggleTileSizeAction : TileResizeAction {
			protected override DocumentManagerStringId ID {
				get { return DocumentManagerStringId.CommandToggleTileSize; }
			}
			protected override DocumentManagerStringId DescriptionID {
				get { return DocumentManagerStringId.CommandToggleTileSizeDescription; }
			}
			protected override bool CanExecuteCore(IContentContainer container) {
				TileContainer tileContainer = container as TileContainer;
				bool checkedTilesIsNotEmpty = false;
				foreach(BaseTile tile in tileContainer.CheckedTiles) {
					checkedTilesIsNotEmpty = true;
					if(!(tile.Properties.ActualIsWide || tile.Properties.ActualIsMedium)) return false;
				}
				return checkedTilesIsNotEmpty;
			}
			protected override bool ChangeTileSizeCore(BaseTile tile) {
				switch(tile.Properties.ActualItemSize) {
					case TileItemSize.Medium:
						tile.Properties.ItemSize = TileItemSize.Default;
						return true;
					case TileItemSize.Default:
					case TileItemSize.Wide:
						tile.Properties.ItemSize = TileItemSize.Medium;
						return true;
				}
				return false;
			}
		}
		[ActionGroup("TileContainer", ActionType.Context, Order = 1, Index = 1, Edge = ActionEdge.Left, Behavior = ActionBehavior.HideBarOnClick)]
		class ClearSelectionAction : TileContainerAction {
			protected override DocumentManagerStringId ID {
				get { return DocumentManagerStringId.CommandClearSelection; }
			}
			protected override DocumentManagerStringId DescriptionID {
				get { return DocumentManagerStringId.CommandClearSelectionDescription; }
			}
			protected override bool CanExecuteCore(IContentContainer container) {
				TileContainer tileContainer = container as TileContainer;
				return System.Linq.Enumerable.Count(tileContainer.CheckedTiles) > 0;
			}
			protected override void ExecuteCore(WindowsUIView view, IContentContainer container) {
				((IWindowsUIViewControllerInternal)view.Controller).ProcessCheckedTiles((TileContainer)container, ClearChecked);
			}
			bool ClearChecked(BaseTile tile) {
				tile.Checked = null;
				if(tile.Manager != null && tile.Manager.View != null)
					tile.Manager.View.SetLayoutModified();
				return true;
			}
		}
	}
}
