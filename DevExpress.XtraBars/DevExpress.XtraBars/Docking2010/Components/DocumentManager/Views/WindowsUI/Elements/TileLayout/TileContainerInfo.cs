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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.XtraEditors;
namespace DevExpress.XtraBars.Docking2010.Views.WindowsUI {
	public interface ITileContainerInfo : IContentContainerInfo, IInteractiveElementInfo {
		ITileControl TileControl { get; }
		void Register(BaseTile tile);
		bool Unregister(BaseTile tile);
		ScrollBarInfo ScrollBarInfo { get; }
		Rectangle ScrollRect { get; }
		void AllowAnimation(BaseTile tile, bool allow);
		void StartAnimation(BaseTile tile);
		void StopAnimation(BaseTile tile);
		void NextFrame(BaseTile tile);
	}
	public class BaseTileDragEventArgs : TileItemDragEventArgs {
		public BaseTileDragEventArgs(BaseTile tile) : this(tile, null) { }
		public BaseTileDragEventArgs(BaseTile tile, DevExpress.XtraEditors.TileGroup targetGroup) {
			this.TargetGroup = targetGroup;
			this.Tile = tile;
		}
		[System.ComponentModel.Browsable(false), System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		public new BaseTile Item { get { return Tile; } }
		public BaseTile Tile { get; internal set; }
	}
	class TileContainerInfo : BaseContentContainerInfo, ITileContainerInfo, ITileControl, ISupportXtraAnimation, IScrollBarInfoOwner, ITileControlUpdateSmartTag, ITileItemProvider {
		TileControlHandler handlerCore;
		TileControlNavigator navigatorCore;
		TileControlViewInfo viewInfoCore;
		ScrollBarInfo scrollBarInfoCore;
		public TileContainerInfo(WindowsUIView view, TileContainer container)
			: base(view, container) {
			InitTileControl();
			this.scrollBarInfoCore = CreateScrollBarInfo();
			ScrollBarInfo.ValueChanged += OnScrollBarInfoValueChanged;
			LayoutHelper.Register(this, (DragEngine.IUIElement)ScrollBarInfo);
			isInitialized = true;
		}
		public bool Capture { get; set; }
		protected override void OnDispose() {
			handlerCore.StopScrollTimer();
			viewInfoCore.StopItemContentAnimations();
			XtraAnimator.RemoveObject(this);
			LayoutHelper.Unregister(this, (DragEngine.IUIElement)ScrollBarInfo);
			ScrollBarInfo.ValueChanged -= OnScrollBarInfoValueChanged;
			Ref.Dispose(ref scrollBarInfoCore);
			base.OnDispose();
		}
		int scrollValueChanged = 0;
		void OnScrollBarInfoValueChanged(object sender, System.EventArgs e) {
			if(scrollValueChanged > 0) return;
			scrollValueChanged++;
			SetPositionCore(ScrollBarInfo.Value);
			scrollValueChanged--;
		}
		protected virtual ScrollBarInfo CreateScrollBarInfo() {
			return new TileContainerScrollBarInfo(this);
		}
		public ScrollBarInfo ScrollBarInfo {
			get { return scrollBarInfoCore; }
		}
		public Rectangle ScrollRect { get; private set; }
		#region IScrollBarInfoOwner Members
		public ScrollBarVisibility ScrollBarVisibility {
			get { return ScrollBarVisibility.Auto; }
		}
		public Point PointToClient(Point point) {
			return Owner.Manager.ScreenToClient(point);
		}
		public Point PointToScreen(Point point) {
			return Owner.Manager.ClientToScreen(point);
		}
		public void Invalidate() {
			Owner.Invalidate();
		}
		public bool IsHorizontal {
			get { return TileContainer != null ? TileContainer.IsHorizontal : false; }
		}
		public DevExpress.LookAndFeel.UserLookAndFeel GetLookAndFeel() {
			return Owner.Manager.LookAndFeel;
		}
		#endregion
		public sealed override Type GetUIElementKey() {
			return typeof(ITileContainerInfo);
		}
		public TileControlHitInfo CalcHitInfo(Point pt) {
			return viewInfoCore.CalcHitInfo(pt);
		}
		public System.ComponentModel.Component GetComponent(Point pt) {
			return (viewInfoCore as TileContainerViewInfo).GetComponent(pt);
		}
		public TileContainer TileContainer {
			get { return Container as TileContainer; }
		}
		bool isInitialized = false;
		public ITileControl TileControl {
			get { return this; }
		}
		int AnimationSuspendCount;
		public bool IsAnimationSuspended {
			get { return AnimationSuspendCount > 0; }
		}
		public void SuspendAnimation() {
			if(AnimationSuspendCount == 0)
				SuspendAnimationCore();
			AnimationSuspendCount++;
		}
		protected virtual void SuspendAnimationCore() { }
		protected virtual void ResumeAnimationCore() { }
		public void ResumeAnimation() {
			if(AnimationSuspendCount > 0)
				AnimationSuspendCount--;
			if(AnimationSuspendCount == 0)
				ResumeAnimationCore();
		}
		public void StartAnimation(BaseTile tile) {
			if(tile == null) return;
			TileItem item = GetTileItem(tile);
			if(item != null)
				item.StartAnimation();
		}
		public void StopAnimation(BaseTile tile) {
			if(tile == null) return;
			TileItem item = GetTileItem(tile);
			if(item != null)
				item.StopAnimation();
		}
		public void NextFrame(BaseTile tile) {
			if(tile == null) return;
			TileItem item = GetTileItem(tile);
			if(item != null) {
				item.AllowAnimation = false;
				item.NextFrame();
			}
		}
		public void AllowAnimation(BaseTile tile, bool allow) {
			if(tile == null) return;
			TileItem item = GetTileItem(tile);
			if(item != null) 
				item.AllowAnimation = allow;
		}
		protected void InitTileControl() {
			groupsCore = new TileGroupCollection(this);
			navigatorCore = new TileControlNavigator(this);
			viewInfoCore = new TileContainerViewInfo(this);
			handlerCore = new TileContainerHandler(this);
			TileControl.AnimateArrival = true;
		}
		IDictionary<BaseTile, TileItem> tileItems = new Dictionary<BaseTile, TileItem>();
		IDictionary<TileItem, BaseTile> tiles = new Dictionary<TileItem, BaseTile>();
		internal BaseTile GetTile(TileItem item) {
			BaseTile tile;
			tiles.TryGetValue(item, out tile);
			return tile;
		}
		internal TileItem GetTileItem(BaseTile item) {
			TileItem tileItem;
			tileItems.TryGetValue(item, out tileItem);
			return tileItem;
		}
		public void Register(BaseTile tile) {
			TileItem item;
			if(!tileItems.TryGetValue(tile, out item)) {
				lockPropertiesChanged++;
				try {
					item = CreateTileItem(tile);
					tiles.Add(item, tile);
					tileItems.Add(tile, item);
					tile.SetOwnerControl(this);
					if(tile.AllowAnimation.HasValue)
						item.AllowAnimation = tile.AllowAnimation.Value;
					AddTileToCheckedTiles(tile);
					AddTileInGroups(tile, item);
				}
				finally { lockPropertiesChanged--; }
			}
		}
		public new TileContainer Container {
			get { return base.Container as TileContainer; }
		}
		public bool Unregister(BaseTile tile) {
			TileItem item;
			if(tileItems.TryGetValue(tile, out item)) {
				lockPropertiesChanged++;
				try {
					if(Owner != null && !Owner.IsDeserializing && !Container.IsDeserializing) {
						XtraEditors.TileGroup targetGroup = item.Group;
						if(targetGroup != null)
							tile.groupCore = targetGroup.Name;
					}
					tiles.Remove(item);
					tileItems.Remove(tile);
					tile.SetOwnerControl(null);
					checkedTiles.Remove(tile);
					RemoveTileFromGroups(tile, item);
					Ref.Dispose(ref item);
					return true;
				}
				finally { lockPropertiesChanged--; }
			}
			return false;
		}
		void AddTileInGroups(BaseTile tile, TileItem item) {
			string groupName = tile.Group;
			XtraEditors.TileGroup targetGroup = groupsCore[groupName];
			if(targetGroup == null) {
				targetGroup = CreateTileGroup();
				targetGroup.Text = tile.Group;
				targetGroup.Name = groupName;
				groupsCore.Add(targetGroup);
			}
			targetGroup.Items.Add(item);
		}
		void RemoveTileFromGroups(BaseTile tile, TileItem item) {
			XtraEditors.TileGroup targetGroup = item.Group;
			if(targetGroup != null) {
				targetGroup.Items.Remove(item);
				if(targetGroup.Items.Count == 0) {
					groupsCore.Remove(targetGroup);
					Ref.Dispose(ref targetGroup);
				}
			}
		}
		void EnsureItemsOrder() {
			BaseTile[] tiles = TileContainer.Items.ToArray();
			List<string> groupNames = new List<string>();
			var affectedGroups = new HashSet<XtraEditors.TileGroup>();
			foreach(BaseTile tile in tiles) {
				if(!groupNames.Contains(tile.Group))
					groupNames.Add(tile.Group);
				if(tile.IsItemsOrderInvalid) {
					affectedGroups.Add(tileItems[tile].Group);
					tile.IsItemsOrderInvalid = false;
				}
			}
			foreach(var group in affectedGroups) {
				int index = groupNames.IndexOf(group.Name);
				if(index != -1) {
					groupsCore.ShouldRemoveItemsFromCache = false;
					groupsCore.Remove(group);
					groupsCore.Insert(index, group);
				}
			}
		}
		void EnsureGroupLayout() {
			var affectedGroups = new HashSet<XtraEditors.TileGroup>();
			foreach(BaseTile tile in TileContainer.Items) {
				if(!tile.IsGroupLayoutInvalid) continue;
				TileItem item = tileItems[tile];
				string groupName = tile.Group;
				if(item.Group.Name != tile.Group) {
					XtraEditors.TileGroup targetGroup = item.Group;
					targetGroup.Items.Remove(item);
					affectedGroups.Add(targetGroup);
					AddTileInGroups(tile, item);
				}
				tile.IsGroupLayoutInvalid = false;
			}
			foreach(var group in affectedGroups) {
				if(group.Items.Count == 0) {
					groupsCore.Remove(group);
					group.Dispose();
				}
			}
		}
		protected bool RaiseClick(BaseTile baseTile) {
			return baseTile.RaiseClick() && (TileContainer != null && Owner != null) &&
				TileContainer.RaiseClick(baseTile) && Owner.RaiseTileClick(baseTile);
		}
		protected void RaisePress(BaseTile baseTile) {
			baseTile.RaisePress();
			if(TileContainer != null)
				TileContainer.RaisePress(baseTile);
			if(Owner != null)
				Owner.RaiseTilePress(baseTile);
		}
		protected internal bool CanCheckTileItem(TileItem item) {
			BaseTile tile;
			return tiles.TryGetValue(item, out tile) && tile.Properties.CanCheck;
		}
		int nameCounter = 1;
		string GenerateName() {
			string name = "TileGroup" + nameCounter;
			foreach(XtraEditors.TileGroup group in groupsCore) {
				if(name == group.Name) {
					nameCounter++;
					return GenerateName();
				}
			}
			return name;
		}
		protected internal void OnGroupNameChanged(XtraEditors.TileGroup group) {
			XtraEditors.TileGroup existingGroup = groupsCore[group.Name];
			if(existingGroup != null && existingGroup != group)
				((TileContainerTileGroup)group).SetNameCore(GenerateName());
		}
		protected internal void OnCreateGroup(XtraEditors.TileGroup group) {
			if(string.IsNullOrEmpty(group.Name))
				group.Name = GenerateName();
			Owner.SetLayoutModified();
		}
		IList<BaseTile> checkedTiles = new List<BaseTile>();
		protected void ProcessTileCheckedChanged(BaseTile tile) {
			AddTileToCheckedTiles(tile);
			TileContainer.RaiseCheckedChanged(tile);
			Owner.RaiseTileCheckedChanged(tile);
			if(checkedTiles.Count > 0) {
				if(Owner.IsInNavigationAdorner)
					Owner.ForceShowNavigationAdorner = true;
				else
					Owner.ShowNavigationAdorner();
			}
			else Owner.HideNavigationAdorner();
			TileContainer.RaiseActualActionsChanged();
		}
		protected void AddTileToCheckedTiles(BaseTile tile) {
			if(tile.Checked.HasValue && tile.Checked.Value)
				checkedTiles.Add(tile);
			else
				checkedTiles.Remove(tile);
		}
		protected new TileContainerInfoPainter Painter {
			get { return base.Painter as TileContainerInfoPainter; }
		}
		protected override void CalcContent(Graphics g, Rectangle content) {
			bool horz = TileContainer.IsHorizontal;
			int scrollSize = horz ?
				SystemInformation.HorizontalScrollBarHeight :
				SystemInformation.VerticalScrollBarWidth;
			Rectangle clientRect = new Rectangle(content.Left, content.Top,
					horz ? content.Width : content.Width - scrollSize,
					horz ? content.Height - scrollSize : content.Height
				);
			ScrollRect = new Rectangle(
					horz ? content.Left : clientRect.Right,
					horz ? clientRect.Bottom : content.Top,
					horz ? content.Width : scrollSize,
					horz ? scrollSize : content.Height
				);
			Rectangle tileBounds = CalcContentWithMargins(content);
			foreach(KeyValuePair<BaseTile, TileItem> p in tileItems)
				UpdateTileItemProperties(p.Value, p.Key);
			lockPropertiesChanged++;
			try {
				EnsureItemsOrder();
				EnsureGroupLayout();
			}
			finally { lockPropertiesChanged--; }
			viewInfoCore.CalcViewInfo(tileBounds);
			ScrollBarInfo.Value = TileContainer.positionCore;
			ScrollBarInfo.ViewInfo.Reset();
			ScrollBarInfo.Bounds = ScrollRect;
		}
		void UpdateTileItemProperties(ITileItem item, BaseTile tile) {
			lockPropertiesChanged++;
			item.Appearances.Assign(tile.Appearances);
			item.Properties.Assign(tile.Properties.ActualProperties);
			item.BackgroundImage = tile.BackgroundImage;
			item.Padding = tile.Padding;
			item.Checked = tile.Checked.GetValueOrDefault(false);
			item.Enabled = tile.Enabled.GetValueOrDefault(true);
			item.Visible = tile.Visible.GetValueOrDefault(true);
			item.Tag = tile.Tag;
			item.Elements.Assign(tile.Elements);
			item.Frames.Assign(tile.Frames);
			lockPropertiesChanged--;
		}
		protected TileItem CreateTileItem(BaseTile tile) {
			return new TileItem();
		}
		void IInteractiveElementInfo.ProcessMouseDown(MouseEventArgs e) {
			if(!IsDisposing)
				handlerCore.OnMouseDown(e);
		}
		void IInteractiveElementInfo.ProcessMouseMove(MouseEventArgs e) {
			if(!IsDisposing)
				handlerCore.OnMouseMove(e);
		}
		void IInteractiveElementInfo.ProcessMouseUp(MouseEventArgs e) {
			if(!IsDisposing)
				handlerCore.OnMouseUp(e);
		}
		void IInteractiveElementInfo.ProcessMouseLeave() {
			if(!IsDisposing)
				handlerCore.OnMouseLeave(EventArgs.Empty);
		}
		void IInteractiveElementInfo.ProcessMouseWheel(MouseEventArgs e) {
			if(ScrollBarInfo != null && Control.ModifierKeys == Keys.None)
				ScrollBarInfo.Handler.OnMouseWheel(e);
		}
		protected override bool ProcessFlickCore(Point point, DevExpress.Utils.Gesture.FlickGestureArgs args) {
			switch(args.Direction) {
				case DevExpress.Utils.Gesture.FlickDirection.Down:
					handlerCore.OnMouseDown(new MouseEventArgs(MouseButtons.Right, 1, point.X, point.Y, 0));
					return true;
			}
			return base.ProcessFlickCore(point, args);
		}
		protected override bool ProcessGestureCore(GestureID gid, DevExpress.Utils.Gesture.GestureArgs args, object[] parameters) {
			switch(gid) {
				case GestureID.QueryAllowGesture:
					base.ProcessGestureCore(gid, args, parameters);
					Point point = (Point)parameters[0];
					var baseGestureAllowArgs = parameters[1] as DevExpress.Utils.Gesture.GestureAllowArgs[];
					var tilesGestureAllowArgs = ((DevExpress.Utils.Gesture.IGestureClient)handlerCore).CheckAllowGestures(point);
					DevExpress.Utils.Gesture.GestureAllowArgs[] result = new DevExpress.Utils.Gesture.GestureAllowArgs[baseGestureAllowArgs.Length + tilesGestureAllowArgs.Length];
					Array.Copy(baseGestureAllowArgs, 0, result, 0, baseGestureAllowArgs.Length);
					Array.Copy(tilesGestureAllowArgs, 0, result, baseGestureAllowArgs.Length, tilesGestureAllowArgs.Length);
					parameters[1] = result;
					return false;
				case GestureID.Begin:
					((DevExpress.Utils.Gesture.IGestureClient)handlerCore).OnBegin(args);
					break;
				case GestureID.Pan:
					Point delta = (Point)parameters[0];
					Point overPan = (Point)parameters[1];
					((DevExpress.Utils.Gesture.IGestureClient)handlerCore).OnPan(args, delta, ref overPan);
					if(handlerCore.TouchState == TileControlHandler.TileControlTouchState.Pan)
						parameters[1] = overPan;
					break;
			}
			return base.ProcessGestureCore(gid, args, parameters);
		}
		#region ITileControl Members
		bool ITileControl.AllowGlyphSkinning { get; set; }
		SizeF ITileControl.ScaleFactor { get { return Owner.Manager.ScaleFactor; } }
		Size ITileControl.DragSize { get { return Size.Empty; } set { } }
		bool ITileControl.EnableItemDoubleClickEvent { get; set; }
		ITileControlProperties ITileControl.Properties {
			get { return TileContainer.Properties.ActualProperties; }
		}
		bool ITileControl.AllowItemContentAnimation {
			get { return Owner.Manager.IsOwnerControlHandleCreated; }
		}
		bool ITileControl.DebuggingState { get { return false; } }
		object ITileControl.Images { get { return null; } set { } }
		string ITileControl.Text { get { return string.Empty; } set { } }
		bool ITileControl.Enabled { get { return true; } set { } }
		ContextItemCollection ITileControl.ContextButtons { get { return null; } }
		ContextItemCollectionOptions ITileControl.ContextButtonOptions { get { return null; } }
		void ITileControl.RaiseContextItemClick(ContextItemClickEventArgs e) { }
		void ITileControl.RaiseContextButtonCustomize(ITileItem tileItem, ContextItem item) { }
		void ITileControl.RaiseCustomContextButtonToolTip(ITileItem tileItem, ContextButtonToolTipEventArgs e) { }
		TileItemAppearances ITileControl.AppearanceItem {
			get { return TileContainer.AppearanceItem; }
		}
		AppearanceObject ITileControl.AppearanceText {
			get { return TileContainer.AppearanceText; }
		}
		AppearanceObject ITileControl.AppearanceGroupText {
			get { return TileContainer.AppearanceGroupText; }
		}
		bool ITileControl.AnimateArrival { get; set; }
		ISupportXtraAnimation ITileControl.AnimationHost { get { return this; } }
		Color ITileControl.BackColor { get { return Color.Empty; } }
		Image ITileControl.BackgroundImage { get { return TileContainer.BackgroundImage; } set { } }
		int ITileControl.ScrollButtonFadeAnimationTime { get; set; }
		System.ComponentModel.IContainer ITileControl.Container { get { return Owner.Container; } }
		System.ComponentModel.ISite ITileControl.Site { get { return Owner.Site; } }
		bool ITileControl.IsDesignMode { get { return Owner.Site != null && Owner.Site.DesignMode; } }
		Control ITileControl.Control { get { return Owner.Manager.GetOwnerControl(); } }
		Rectangle ITileControl.ClientRectangle { get { return Bounds; } }
		bool ITileControl.IsHandleCreated { get { return Owner.Manager.IsOwnerControlHandleCreated; } }
		IntPtr ITileControl.Handle { get { return Owner.Manager.GetOwnerControlHandle(); } }
		void ITileControl.AddControl(System.Windows.Forms.Control control) { }
		bool ITileControl.ContainsControl(System.Windows.Forms.Control control) { return false; }
		bool ITileControl.Focus() { return false; }
		DevExpress.LookAndFeel.UserLookAndFeel ITileControl.LookAndFeel { get { return Owner.ElementsLookAndFeel; } }
		DevExpress.XtraEditors.Controls.BorderStyles ITileControl.BorderStyle { get; set; }
		TileItem ITileControl.SelectedItem { get; set; }
		TileGroupCollection groupsCore;
		TileGroupCollection ITileControl.Groups {
			get { return groupsCore; }
		}
		void ITileControl.Invalidate(Rectangle rect) {
			if(!IsDisposing)
				Owner.Invalidate(rect);
		}
		bool ITileControl.IsLockUpdate {
			get { return Owner.IsUpdateLocked; }
		}
		void ITileControl.BeginUpdate() {
			Owner.BeginUpdate();
		}
		void ITileControl.EndUpdate() {
			if(lockPropertiesChanged > 0)
				Owner.CancelUpdate();
			else Owner.EndUpdate();
			this.viewInfoCore.SetDirty();
		}
		event TileItemClickEventHandler ITileControl.ItemCheckedChanged { add { } remove { } }
		event TileItemClickEventHandler ITileControl.ItemClick { add { } remove { } }
		event TileItemClickEventHandler ITileControl.ItemDoubleClick { add { } remove { } }
		event TileItemClickEventHandler ITileControl.RightItemClick { add { } remove { } }
		event TileItemClickEventHandler ITileControl.ItemPress { add { } remove { } }
		event TileItemClickEventHandler ITileControl.SelectedItemChanged { add { } remove { } }
		void ITileControl.UpdateSmartTag() {
			RaiseSmartTagUpdate();
		}
		event TileItemDragEventHandler startItemDraggingEvent;
		public event TileItemDragEventHandler StartItemDragging {
			add { startItemDraggingEvent += value; }
			remove { startItemDraggingEvent -= value; }
		}
		event TileItemDragEventHandler endItemDraggingEvent;
		public event TileItemDragEventHandler EndItemDragging {
			add { endItemDraggingEvent += value; }
			remove { endItemDraggingEvent -= value; }
		}
		TileControlNavigator ITileControl.Navigator {
			get { return navigatorCore; }
		}
		TileControlViewInfo ITileControl.ViewInfo {
			get { return viewInfoCore; }
		}
		TileControlPainter ITileControl.SourcePainter {
			get { return Painter.PainterCore; }
		}
		TileControlScrollMode ITileControl.ScrollMode {
			get { return TileControlScrollMode.None; }
			set { }
		}
		TileControlHandler ITileControl.Handler {
			get { return handlerCore; }
		}
		bool ITileControl.AllowDrag {
			get { return true; }
		}
		void ITileControl.OnItemClick(TileItem tileItem) {
			BaseTile baseTile = tiles[tileItem];
			baseTile.CurrentFrame = tileItem.CurrentFrame;
			if(!RaiseClick(baseTile) || (TileContainer == null) || (Owner == null) || (Owner.Site != null && Owner.Site.DesignMode)) return;
			IContentContainer activationTarget = baseTile.AssociatedContentContainer ?? TileContainer.ActivationTarget;
			if(activationTarget != null) {
				if(((IWindowsUIViewControllerInternal)Owner.Controller).PrepareAssociatedContentContainer(baseTile, activationTarget))
					Owner.Controller.Activate(activationTarget);
			}
			else Owner.Controller.Activate(baseTile);
		}
		void ITileControl.OnItemDoubleClick(TileItem tileItem) { }
		void ITileControl.OnRightItemClick(TileItem tileItem) { }
		void ITileControl.OnItemPress(TileItem tileItem) {
			BaseTile tile;
			if(tiles.TryGetValue(tileItem, out tile)) {
				tile.CurrentFrame = tileItem.CurrentFrame;
				RaisePress(tile);
			}
		}
		void ITileControl.OnItemCheckedChanged(TileItem tileItem) {
			if(lockPropertiesChanged > 0) return;
			BaseTile tile;
			if(tiles.TryGetValue(tileItem, out tile)) {
				tile.CurrentFrame = tileItem.CurrentFrame;
				tile.Checked = tileItem.Checked;
				ProcessTileCheckedChanged(tile);
			}
		}
		TileItemDragEventArgs ITileControl.RaiseStartItemDragging(TileItem item) {
			TileItemDragEventArgs e = CreateTileItemDragEventArgs(item);
			if(startItemDraggingEvent != null)
				startItemDraggingEvent(this, e);
			Container.RaiseStartItemDragging((BaseTileDragEventArgs)e);
			return e;
		}
		TileItemDragEventArgs ITileControl.RaiseEndItemDragging(TileItem item, XtraEditors.TileGroup targetGroup) {
			TileItemDragEventArgs e = CreateTileItemDragEventArgs(item, targetGroup);
			if(endItemDraggingEvent != null)
				endItemDraggingEvent(this, e);
			Container.RaiseEndItemDragging((BaseTileDragEventArgs)e);
			return e;
		}
		TileItemDragEventArgs CreateTileItemDragEventArgs(TileItem item) {
			TileItemDragEventArgs e = new BaseTileDragEventArgs(GetTile(item));
			e.Item = item;
			return e;
		}
		TileItemDragEventArgs CreateTileItemDragEventArgs(TileItem item, XtraEditors.TileGroup targetGroup) {
			TileItemDragEventArgs e = new BaseTileDragEventArgs(GetTile(item), targetGroup);
			e.Item = item;
			return e;
		}
		int lockPropertiesChanged = 0;
		void ITileControl.OnPropertiesChanged() {
			if(lockPropertiesChanged > 0) return;
			if(isInitialized && !IsDisposing)
				Owner.LayoutChanged();
		}
		int positionCore;
		int ITileControl.Position {
			get { return positionCore; }
			set { SetPositionCore(value); }
		}
		bool ITileControl.AllowDisabledStateIndication {
			get;
			set;
		}
		bool ITileControl.AllowSelectedItem {
			get { return false; }
		}
		bool ITileControl.AutoSelectFocusedItem {
			get;
			set;
		}
		bool ITileControl.AllowDragTilesBetweenGroups {
			get { return Container.Properties.CanDragTilesBetweenGroups; }
			set { }
		}
		void SetPositionCore(int value) {
			value = viewInfoCore.ConstraintOffset(value);
			if(positionCore == value)
				return;
			if(TileContainer != null)
				TileContainer.positionCore = value;
			positionCore = value;
			viewInfoCore.Offset = value;
			scrollValueChanged++;
			ScrollBarInfo.Value = value;
			scrollValueChanged--;
			RaiseSmartTagUpdate();
			Owner.Invalidate();
		}
		DevExpress.XtraEditors.ScrollBarBase ITileControl.ScrollBar { get { return null; } set { } }
		Color ITileControl.SelectionColor { get { return Color.White; } }
		#endregion
		#region ISupportXtraAnimation Members
		bool ISupportXtraAnimation.CanAnimate {
			get { return !IsDisposing; }
		}
		Control ISupportXtraAnimation.OwnerControl {
			get { return Owner.Manager.GetOwnerControl(); }
		}
		#endregion
		#region ITileControlUpdateSmartTag Members
		event SmartTagUpdateEventHandler smartTagUpdateCore;
		event SmartTagUpdateEventHandler ITileControlUpdateSmartTag.SmartTagUpdate {
			add { smartTagUpdateCore += value; }
			remove { smartTagUpdateCore -= value; }
		}
		void RaiseSmartTagUpdate() {
			var args = new TileControlSmartTagEventArgs() { Info = viewInfoCore, TileControl = this };
			RaiseSmartTagUpdate(this, args);
			foreach(TileItem item in tileItems.Values) {
				TileItemViewInfo viewInfo = (viewInfoCore as TileContainerViewInfo).GetItemInfo(item);
				var tile = GetTile(item);
				if(tile != null && !tile.IsDisposing) {
					args.Info = viewInfo;
					RaiseSmartTagUpdate(tile, args);
				}
			}
		}
		void RaiseSmartTagUpdate(object sender, TileControlSmartTagEventArgs args) {
			if(smartTagUpdateCore != null)
				smartTagUpdateCore(sender, args);
		}
		#endregion
		TileItem ITileItemProvider.GetTileItem(object obj) {
			if(obj is BaseTile)
				return GetTileItem(obj as BaseTile);
			return null;
		}
		internal XtraEditors.TileGroup CreateTileGroup() {
			return new TileContainerTileGroup(this);
		}
	}
	class TileContainerTileGroup : XtraEditors.TileGroup {
		TileContainerInfo infoCore;
		public TileContainerTileGroup(TileContainerInfo info) {
			this.infoCore = info;
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			infoCore = null;
		}
		protected TileContainerInfo Info {
			get { return infoCore; }
		}
		public override string Name {
			get { return base.Name; }
			set {
				string oldName = base.Name;
				SetNameCore(value);
				if(base.Name != oldName)
					OnNameChanged();
			}
		}
		internal void SetNameCore(string name) {
			base.Name = name;
		}
		void OnNameChanged() {
			if(Info != null)
				Info.OnGroupNameChanged(this);
		}
	}
	class TileContainerViewInfo : TileControlViewInfo {
		public TileContainerViewInfo(TileContainerInfo info)
			: base(info) {
		}
		public void AnimateArrival() {
			AnimateAppearance();
		}
		protected sealed override TileControlScrollMode ScrollMode {
			get { return TileControlScrollMode.None; }
		}
		protected sealed override void CreateScrollBar() { }
		protected sealed override void UpdateScrollParams() {
			ScrollBarInfo sbInfo = ((ITileContainerInfo)Owner).ScrollBarInfo;
			if(IsHorizontal) {
				sbInfo.Maximum = ContentBestWidth;
				sbInfo.LargeChange = GroupsContentBounds.Width;
			}
			else {
				sbInfo.Maximum = ContentBestHeight;
				sbInfo.LargeChange = GroupsContentBounds.Height;
			}
			sbInfo.SmallChange = Owner.Properties.ItemSize + Owner.Properties.IndentBetweenItems;
		}
		public TileItemViewInfo GetItemInfo(TileItem item) {
			return GetItemFromCache(item);
		}
		public int GetScrolMaximum() {
			return IsHorizontal ? ContentBestWidth : ContentBestHeight;
		}
		public virtual System.ComponentModel.Component GetComponent(Point point) {
			TileControlHitInfo hitInfo = base.CalcHitInfo(point);
			TileContainerInfo tileContainer = Owner as TileContainerInfo;
			if(tileContainer == null) return null;
			if(hitInfo.ItemInfo == null) {
				IBaseElementInfo info = tileContainer.TileContainer.Manager.CalcHitInfo(point).HitElement;
				if(info is TileContainerInfo) return tileContainer.TileContainer;
				if(info is WindowsUIViewInfo) return tileContainer.Owner;
				return null;
			}
			return tileContainer.GetTile(hitInfo.ItemInfo.Item);
		}
		protected override System.ComponentModel.Component GetTileComponent(TileItem tile) {
			return (Owner as TileContainerInfo).GetTile(tile);
		}
		public override bool IsLargeItem(TileItem item) {
			return (item.ItemSize == TileItemSize.Default) || (item.ItemSize == TileItemSize.Large) || (item.ItemSize == TileItemSize.Wide);
		}
		public override bool IsMediumItem(TileItem item) {
			return (item.ItemSize == TileItemSize.Medium);
		}
	}
	class TileContainerHandler : TileControlHandler {
		public TileContainerHandler(TileContainerInfo info)
			: base(info) {
		}
		protected TileContainerInfo Owner {
			get { return Control as TileContainerInfo; }
		}
		protected override bool CheckItemCore(TileItem item) {
			if(!Owner.CanCheckTileItem(item)) return false;
			return base.CheckItemCore(item);
		}
		protected override XtraEditors.TileGroup CreateNewGroupByDragging() {
			var group = Owner.CreateTileGroup();
			Owner.OnCreateGroup(group);
			return group;
		}
		protected override void AddInGroupByDropItem(DevExpress.XtraEditors.TileGroup group, TileItem item) {
			base.AddInGroupByDropItem(group, item);
			EnsureItemOrderInGroup(item);
		}
		protected override void InsertInGroupByDropItem(DevExpress.XtraEditors.TileGroup group, TileItem item, int index) {
			base.InsertInGroupByDropItem(group, item, index);
			EnsureItemOrderInGroup(item);
		}
		protected override void OnDragDrop(bool cancelDrop) {
			var manager = (Owner.Owner != null) ? Owner.Owner.Manager : null;
			var changeTracker = (manager != null) ? manager.TrackLayoutChanged() : null;
			using(changeTracker)
				base.OnDragDrop(cancelDrop);
		}
		void EnsureItemOrderInGroup(TileItem item) {
			var manager = (Owner.Owner != null) ? Owner.Owner.Manager : null;
			var changeTracker = (manager != null) ? manager.TrackLayoutChanged() : null;
			using(changeTracker) {
				SetGroup(Owner.GetTile(item), item);
				SetItemsOrder(Control.Groups);
			}
		}
		void SetGroup(BaseTile tile, TileItem item) {
			if(tile != null && tile.Group != item.Group.Name) {
				tile.Group = item.Group.Name;
				Owner.Owner.SetLayoutModified();
			}
		}
		void SetItemsOrder(XtraEditors.TileGroupCollection groups) {
			var containerItems = Owner.TileContainer.Items;
			foreach(XtraEditors.TileGroup group in groups) {
				foreach(TileItem item in group.Items) {
					BaseTile tile = Owner.GetTile(item);
					containerItems.Move(containerItems.Count, tile);
				}
			}
		}
		protected override void OnMouseDownDesignMode(MouseEventArgs e) {
			System.ComponentModel.Component component = Owner.GetComponent(e.Location);
			if(component != null) Control.ViewInfo.DesignTimeManager.SelectComponent(component);
		}
	}
	class TileContainerInfoPainter : ContentContainerInfoPainter {
		TileControlPainter painter;
		public TileContainerInfoPainter() {
			painter = new TileControlPainter();
		}
		protected override void DrawContent(GraphicsCache cache, IContentContainerInfo info) {
			ITileContainerInfo tileContainerInfo = info as ITileContainerInfo;
			if(tileContainerInfo != null) {
				if(tileContainerInfo.ScrollBarInfo.Visible)
					tileContainerInfo.ScrollBarInfo.DoDraw(cache, tileContainerInfo.ScrollRect);
				ITileControl tileControl = tileContainerInfo.TileControl;
				if(tileControl.AnimateArrival) {
					tileControl.AnimateArrival = false;
					((TileContainerViewInfo)tileControl.ViewInfo).AnimateArrival();
					return;
				}
				painter.Draw(new TileControlInfoArgs(cache, tileControl.ViewInfo));
			}
		}
		public override Padding GetContentMargins() {
			return new Padding(0, 0, 0, 40);
		}
		protected internal TileControlPainter PainterCore { get { return painter; } }
	}
	class TileContainerInfoSkinPainter : TileContainerInfoPainter {
		ISkinProvider providerCore;
		public TileContainerInfoSkinPainter(ISkinProvider provider) {
			providerCore = provider;
		}
		public override Padding GetContentMargins() {
			SkinElement tileContainer = GetTileContainerElement();	   
			if(tileContainer != null) {
				var edges = tileContainer.ContentMargins;
				return new Padding(edges.Left, edges.Top, edges.Right, edges.Bottom);
			}
			return base.GetContentMargins();
		}
		protected virtual SkinElement GetTileContainerElement() {
			return GetSkin()[MetroUISkins.SkinTileContainer];
		}
		protected virtual Skin GetSkin() {
			return MetroUISkins.GetSkin(providerCore);
		}
	}
}
