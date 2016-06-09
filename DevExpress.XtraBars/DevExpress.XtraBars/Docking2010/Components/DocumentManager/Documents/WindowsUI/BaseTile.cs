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
using DevExpress.XtraBars.Docking2010.Base;
using DevExpress.XtraEditors;
using DevExpress.Utils.Design;
using DevExpress.Utils.VisualEffects;
namespace DevExpress.XtraBars.Docking2010.Views.WindowsUI {
	[ToolboxItem(false), DesignTimeVisible(false)]
	public abstract class BaseTile : BaseComponent,
		ITileItem,
		INotifyElementPropertiesChanged, INamed, Customization.ISearchObject, ISupportAdornerElement {
		IBaseTileDefaultProperties propertiesCore;
		TileItemElementCollection elementsCore;
		TileItemFrameCollection framesCore;
		ITileControl controlCore;
		protected BaseTile()
			: base(null) {
			InitProperties(null);
		}
		protected BaseTile(IContainer container)
			: base(container) {
			InitProperties(null);
		}
		protected BaseTile(IBaseTileProperties parentProperties)
			: base(null) {
			InitProperties(parentProperties);
		}
		protected override void OnCreate() {
			base.OnCreate();
			paddingCore = DefaultPadding;
			appearancesCore = new TileItemAppearances();
			elementsCore = new TileItemElementCollection(this);
			framesCore = new TileItemFrameCollection(this);
			Appearances.Changed += OnAppearancesChanged;
		}
		void InitProperties(IBaseTileProperties parentProperties) {
			propertiesCore = CreateDefaultProperties(parentProperties);
			Properties.Changed += OnPropertiesChanged;
		}
		protected override void LockComponentBeforeDisposing() {
			base.LockComponentBeforeDisposing();
			Properties.Changed -= OnPropertiesChanged;
			Appearances.Changed -= OnAppearancesChanged;
		}
		protected override void OnDispose() {
			Ref.Dispose(ref appearancesCore);
			Ref.Dispose(ref propertiesCore);
			base.OnDispose();
		}
		[Browsable(false)]
		public string Name { get; set; }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseTileElements"),
#endif
 Category("Layout"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TileItemElementCollection Elements {
			get { return elementsCore; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseTileFrames"),
#endif
 Category(CategoryName.Appearance),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TileItemFrameCollection Frames {
			get { return framesCore; }
		}
		Padding paddingCore;
		internal static Padding DefaultPadding = new Padding(-1);
		bool ShouldSerializePadding() { return Padding != DefaultPadding; }
		void ResetPadding() { Padding = DefaultPadding; }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseTilePadding"),
#endif
 Category("Layout")]
		public Padding Padding {
			get { return paddingCore; }
			set { SetValue(ref paddingCore, value); }
		}
		object tagCore;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseTileTag"),
#endif
 DefaultValue(null), Category("Data"),
		Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor)),
		TypeConverter(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter))]
		public object Tag {
			get { return tagCore; }
			set { tagCore = value; }
		}
		protected internal bool CanCheck {
			get { return IsInitializing || Properties.CanCheck; }
		}
		protected internal virtual bool CanActivate {
			get { return false; }
		}
		protected internal virtual bool IsEnabled {
			get { return Enabled.GetValueOrDefault(true); }
		}
		protected internal virtual bool IsContainer {
			get { return false; }
		}
		protected internal virtual BaseTile[] GetChildren() {
			return new BaseTile[] { };
		}
		protected internal virtual IContentContainer AssociatedContentContainer {
			get { return null; }
		}
		protected internal virtual Document[] AssociatedDocuments {
			get { return new Document[0]; }
		}
		protected internal bool? AllowAnimation { get; set; }
		void OnAppearancesChanged(object sender, System.EventArgs e) {
			LayoutChanged();
		}
		protected override void OnLayoutChanged() {
			base.OnLayoutChanged();
			if(Manager != null)
				Manager.LayoutChanged();
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseTileProperties"),
#endif
 Category(CategoryName.Properties),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), SmartTagSearchNestedProperties]
		public IBaseTileDefaultProperties Properties {
			get { return propertiesCore; }
		}
		bool ShouldSerializeProperties() { return (Properties != null) && Properties.ShouldSerialize(); }
		void ResetProperties() { Properties.Reset(); }
		protected virtual IBaseTileDefaultProperties CreateDefaultProperties(IBaseTileProperties parentProperties) {
			return new BaseTileDefaultProperties(parentProperties);
		}
		void OnPropertiesChanged(object sender, EventArgs e) {
			LayoutChanged();
		}
		internal string groupCore = "";
		internal bool IsItemsOrderInvalid;
		internal bool IsGroupLayoutInvalid;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseTileGroup"),
#endif
 Category("Layout"), DefaultValue(""), SmartTagProperty("Group", "")]
		public virtual string Group {
			get { return groupCore == null ? string.Empty : groupCore; }
			set {
				if(groupCore == value) return;
				groupCore = value;
				if(!IsInitializing)
					IsGroupLayoutInvalid = true;
				LayoutChanged();
			}
		}
		Image backgroundImageCore;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseTileBackgroundImage"),
#endif
 DefaultValue(null), Category(CategoryName.Appearance), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(System.Drawing.Design.UITypeEditor))]
		public virtual Image BackgroundImage {
			get { return backgroundImageCore; }
			set {
				if(backgroundImageCore == value) return;
				backgroundImageCore = value;
				LayoutChanged();
			}
		}
		bool? checkedCore;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseTileChecked"),
#endif
 DefaultValue(null), Category(CategoryName.Behavior), SmartTagProperty("Checked", "")]
		public virtual bool? Checked {
			get { return checkedCore; }
			set {
				if(checkedCore == value || !CanCheck) return;
				checkedCore = value;
				OnCheckedChanged();
			}
		}
		protected virtual void OnCheckedChanged() {
			RaiseCheckedChanged();
			LayoutChanged();
		}
		bool? visibleCore;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseTileVisible"),
#endif
 DefaultValue(null), Category(CategoryName.Behavior)]
		public virtual bool? Visible {
			get { return visibleCore; }
			set {
				if(visibleCore == value) return;
				visibleCore = value;
				OnVisibleChanged();
			}
		}
		protected virtual void OnVisibleChanged() {
			LayoutChanged();
		}
		bool? enabledCore;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseTileEnabled"),
#endif
 DefaultValue(null), Category(CategoryName.Behavior), SmartTagProperty("Enabled", "")]
		public virtual bool? Enabled {
			get { return enabledCore; }
			set {
				if(enabledCore == value) return;
				enabledCore = value;
				OnEnabledChanged();
			}
		}
		protected virtual void OnEnabledChanged() {
			LayoutChanged();
		}
		TileItemAppearances appearancesCore;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseTileAppearances"),
#endif
 Category(CategoryName.Appearance),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TileItemAppearances Appearances {
			get { return appearancesCore; }
		}
		void ResetAppearances() { Appearances.Reset(); }
		bool ShouldSerializeAppearances() { return Appearances.ShouldSerialize(); }
		protected abstract internal object GetID();
		bool isActiveCore;
		[Browsable(false)]
		public bool IsActive {
			get { return isActiveCore; }
		}
		internal void SetIsActive(bool value) {
			if(isActiveCore == value) return;
			isActiveCore = value;
			OnIsActiveChanged();
		}
		TileItemFrame currentFrameCore;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TileItemFrame CurrentFrame {
			get {
				if(currentFrameCore != null)
					return currentFrameCore;
				if(Properties.ActualCurrentFrameIndex >= Frames.Count)
					return null;
				return Frames[Properties.ActualCurrentFrameIndex];
			}
			internal set { currentFrameCore = value; }
		}
		DocumentManager managerCore;
		[Browsable(false)]
		public DocumentManager Manager {
			get { return managerCore; }
		}
		internal void SetManager(DocumentManager manager) {
			if(managerCore == manager) return;
			managerCore = manager;
			OnManagerChanged();
		}
		protected virtual void OnIsActiveChanged() {
			LayoutChanged();
		}
		protected virtual void OnManagerChanged() { }
		protected internal void SetOwnerControl(ITileControl control) {
			controlCore = control;
			UpdateVisualEffects(UpdateAction.OwnerChanged);
		}
		#region ITileItem
		ITileItemProperties ITileItem.Properties {
			get { return Properties.ActualProperties; }
		}
		event TileItemClickEventHandler ITileItem.CheckedChanged {
			add { }
			remove { }
		}
		bool ITileItem.Checked {
			get { return Checked.GetValueOrDefault(false); }
			set { Checked = value; }
		}
		bool ITileItem.Visible {
			get { return Visible.GetValueOrDefault(true); }
			set { Visible = value; }
		}
		bool ITileItem.Enabled {
			get { return Enabled.GetValueOrDefault(true); }
			set { Enabled = value; }
		}
		public virtual void SetContent(TileItemFrame frame, bool animated) { }
		ITileControl ITileItem.Control { get { return controlCore; } }
		void ITileItem.OnInfoChanged() { }
		int ITileItem.CurrentFrameIndex { get { return Properties.ActualCurrentFrameIndex; } }
		#endregion
		#region events
		static readonly object click = new object();
		static readonly object press = new object();
		static readonly object checkedChanged = new object();
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
		protected internal bool RaiseClick() {
			TileClickEventHandler handler = (TileClickEventHandler)Events[click];
			TileClickEventArgs e = new TileClickEventArgs(this);
			if(handler != null)
				handler(this, e);
			return !e.Handled;
		}
		protected internal void RaisePress() {
			TileEventHandler handler = (TileEventHandler)Events[press];
			TileEventArgs e = new TileEventArgs(this);
			if(handler != null)
				handler(this, e);
		}
		protected internal void RaiseCheckedChanged() {
			TileEventHandler handler = (TileEventHandler)Events[checkedChanged];
			if(handler != null)
				handler(this, new TileEventArgs(this));
		}
		#endregion events
		#region INotifyElementPropertiesChanged Members
		void INotifyElementPropertiesChanged.OnElementPropertiesChanged(TileItemElement element) {
			OnPropertiesChanged(this, EventArgs.Empty);
		}
		ITileControl INotifyElementPropertiesChanged.TileControl {
			get { return null; }
		}
		#endregion
		#region ISearchObject Members
		string Customization.ISearchObject.SearchText { get { return GetSearchText(); } }
		string Customization.ISearchObject.SearchTag { get { return GetSearchTag(); } }
		string[] searchTagsCore;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseTileSearchTags"),
#endif
		Category("Search Properties"), DefaultValue(null), Localizable(true)]
		public string[] SearchTags {
			get { return searchTagsCore; }
			set {
				if(searchTagsCore == value) return;
				searchTagsCore = value;
			}
		}
		protected virtual string GetSearchTag() {
			string searchTags = string.Empty;
			if(SearchTags == null || SearchTags.Length <= 0) {
				if(this.Elements == null) return searchTags;
				foreach(TileItemElement element in this.Elements)
					if(!string.IsNullOrEmpty(element.Text))
						searchTags += element.Text;
				return searchTags;
			}
			foreach(string tag in SearchTags) {
				if(string.IsNullOrEmpty(tag)) continue;
				searchTags += tag;
			}
			return searchTags;
		}
		protected virtual string GetSearchText() {
			if(this.Elements != null) {
				foreach(TileItemElement element in this.Elements)
					if(!string.IsNullOrEmpty(element.Text))
						return element.Text;
			}
			return Name;
		}
		bool excludeFromSearchCore;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseTileExcludeFromSearch"),
#endif
 Category("Search Properties"), DefaultValue(false)]
		public bool ExcludeFromSearch {
			get { return excludeFromSearchCore; }
			set {
				if(ExcludeFromSearch == value) return;
				excludeFromSearchCore = value;
			}
		}
		bool Customization.ISearchObject.EnabledInSearch { get { return EnabledInSearch; } }
		protected virtual bool EnabledInSearch { get { return ((ITileItem)this).Visible && !this.IsDisposing && !ExcludeFromSearch; } }
		#endregion
		#region ISupportAdornerElement Members
		Rectangle ISupportAdornerElement.Bounds {
			get {
				if(controlCore == null || controlCore.ViewInfo == null) return Rectangle.Empty;
				TileItem item = (controlCore as TileContainerInfo).GetTileItem(this);
				if(item == null) return Rectangle.Empty;
				return item.ItemInfo.Bounds;
			}
		}
		bool ISupportAdornerElement.IsVisible {
			get {
				TileItem item = (controlCore as TileContainerInfo).GetTileItem(this);
				return
					this.Visible.GetValueOrDefault(true) &&
					controlCore != null &&
					controlCore.Handler.State != TileControlHandlerState.DragMode &&
					item != null &&
					item.ItemInfo != null &&
					!item.ItemInfo.IsInTransition;
			}
		}
		ISupportAdornerUIManager ISupportAdornerElement.Owner {
			get { return controlCore != null ? controlCore.ViewInfo as ISupportAdornerUIManager : null; }
		}
		readonly static object targetChanged = new object();
		event UpdateActionEventHandler ISupportAdornerElement.Changed {
			add { Events.AddHandler(targetChanged, value); }
			remove { Events.RemoveHandler(targetChanged, value); }
		}
		protected internal void UpdateVisualEffects(UpdateAction action) {
			UpdateActionEventHandler handler = Events[targetChanged] as UpdateActionEventHandler;
			if(handler == null) return;
			UpdateActionEvendArgs e = new UpdateActionEvendArgs(action);
			handler(this, e);
		}
		#endregion
	}
	public class TileCollection : BaseMutableListEx<BaseTile, TileContainer> {
		public TileCollection(TileContainer owner)
			: base(owner) {
		}
		protected override bool CanAdd(BaseTile element) {
			return !Owner.IsFilledUp && base.CanAdd(element);
		}
		protected override void NotifyOwnerOnInsert(int index) {
			Owner.OnInsert(index);
		}
		protected override void NotifyOwnerOnMove(int index, BaseTile element) {
			Owner.OnMove(index, element);
		}
		public BaseTile this[string name] {
			get { return FindFirst((tile) => (!string.IsNullOrEmpty(tile.Name) && tile.Name.Equals(name))); }
		}
	}
}
