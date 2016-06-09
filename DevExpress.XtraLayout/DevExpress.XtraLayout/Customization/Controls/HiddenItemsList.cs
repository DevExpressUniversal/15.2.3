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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DevExpress.Data.Filtering;
using DevExpress.Data.Helpers;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.DragDrop;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ListControls;
using DevExpress.XtraLayout.Customization.Templates;
using DevExpress.XtraLayout.Handlers;
using DevExpress.XtraLayout.Utils;
namespace DevExpress.XtraLayout.Customization.Controls {
	public class HiddenItemsListHandler :ListBoxControlHandler {
		public HiddenItemsListHandler(HiddenItemsList listBox) : base(listBox) { }
		protected override ListBoxControlState CreateState(HandlerState state) {
			return new HiddenItemsListSingleSelectState(this);
		}
		public class HiddenItemsListSingleSelectState :ListBoxControlHandler.SingleSelectState {
			public HiddenItemsListSingleSelectState(HiddenItemsListHandler handler) : base(handler) { }
			protected override void OnTimer(object sender, EventArgs e) { }
		}
	}
	[DesignTimeVisible(true), ToolboxItem(false)]
	[Designer(LayoutControlConstants.HiddenItemsListDesignerName, typeof(System.ComponentModel.Design.IDesigner)), ToolboxBitmap(typeof(LayoutControl), "Images.hidden-items.bmp")]
	public class HiddenItemsList :ListBoxControl, IDragDropDispatcherClient, IToolFrameOwner, ICustomizationFormControl {
		protected DragDropClientDescriptor clientDescriptorCore = null;
		bool fAllowProcessDragging = true;
		bool fAllowNonItemDrop = true;
		bool fAllowResetFocusOnMouseLeave = true;
		int hotTrackedItemIndex = -1;
		int pressedItemIndex = -1;
		internal DragHeaderPainter hPainter;
		public HiddenItemsList() {
			this.SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.DoubleBuffer, true);
			this.SelectionMode = SelectionMode.MultiSimple;
			clientDescriptorCore = DragDropDispatcherFactory.Default.RegisterClient(this as IDragDropDispatcherClient);
			DragDropDispatcherFactory.Default.SetClientToolFrame(clientDescriptorCore, DragDropItemCursor.Default);
			this.ItemHeight = GetItemHeight();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ToolTipController ToolTipController {
			get { return base.ToolTipController; }
			set { base.ToolTipController = value; }
		}
		bool IDragDropDispatcherClient.ExcludeChildren { get { return false; } }
		protected void Init() {
			if(OwnerControl != null) hPainter = new DragHeaderPainter(OwnerControl.LookAndFeel.ActiveLookAndFeel, OwnerControl.LookAndFeel.ActiveStyle);
		}
		public void BeginInit() { }
		public void EndInit() { }
		#region ICustomizationFormControl
		public void Register() {
			Init();
			OwnerControl.HiddenItems.Changed += new CollectionChangeEventHandler(HiddenItems_Changed);
		}
		public void UnRegister() {
			if(OwnerControl != null && OwnerControl.HiddenItems != null) OwnerControl.HiddenItems.Changed -= new CollectionChangeEventHandler(HiddenItems_Changed);
		}
		public ILayoutControl OwnerControl { get { return GetOwnerControl(); } }
		protected override void OnPaint(PaintEventArgs e) {
			if(OwnerControl == null) { WrongParentTypeMessagePainter.Default.Draw(new Rectangle(0, 0, Width, Height), e); return; }
			base.OnPaint(e);
		}
		protected virtual ILayoutControl GetOwnerControl() {
			return OwnerControlHelper.GetOwnerControl(Parent);
		}
		public UserLookAndFeel ControlOwnerLookAndFeel { get { return OwnerControl != null ? OwnerControl.LookAndFeel : null; } }
		#endregion
		protected override ListBoxControlHandler CreateHandler() {
			return new HiddenItemsListHandler(this);
		}
		void HiddenItems_Changed(object sender, CollectionChangeEventArgs e) {
			UpdateContent();
		}
		#region IDragDropDispatcherClient
		IntPtr IDragDropDispatcherClient.ClientHandle {
			get { return this.Handle; }
		}
		DragDropClientDescriptor IDragDropDispatcherClient.ClientDescriptor {
			get { return clientDescriptorCore; }
		}
		Rectangle IDragDropDispatcherClient.ScreenBounds {
			get { return this.RectangleToScreen(this.ClientRectangle); }
		}
		bool IDragDropDispatcherClient.IsActiveAndCanProcessEvent {
			get { return this.Visible; }
		}
		bool IDragDropDispatcherClient.AllowProcessDragging {
			get { return fAllowProcessDragging; }
			set { fAllowProcessDragging = value; }
		}
		bool IDragDropDispatcherClient.AllowNonItemDrop {
			get { return fAllowNonItemDrop; }
			set { fAllowNonItemDrop = value; }
		}
		public bool AllowResetFocusOnMouseLeave {
			get { return fAllowResetFocusOnMouseLeave; }
			set { fAllowResetFocusOnMouseLeave = value; }
		}
		bool IDragDropDispatcherClient.IsPointOnItem(Point clientPoint) {
			return ItemByPoint(clientPoint) != null;
		}
		BaseLayoutItem IDragDropDispatcherClient.GetItemAtPoint(Point clientPoint) {
			return ItemByPoint(clientPoint) as BaseLayoutItem;
		}
		BaseLayoutItem IDragDropDispatcherClient.ProcessDragItemRequest(Point clientPoint) {
			return ItemByPoint(clientPoint) as BaseLayoutItem;
		}
		DragDropClientGroupDescriptor IDragDropDispatcherClient.ClientGroup {
			get {
				IDragDropDispatcherClient client = OwnerControl as IDragDropDispatcherClient;
				if(client != null) {
					return client.ClientGroup;
				}
				return null;
			}
		}
		void IDragDropDispatcherClient.OnDragModeKeyDown(KeyEventArgs kea) {
			switch(kea.KeyCode) {
				case Keys.Tab:
					if(kea.Control) {
						RestoreCursor();
						DragCursorOff(DragDropItemCursor.Default);
					}
					break;
			}
		}
		void IDragDropDispatcherClient.OnDragEnter() {
			DragCursorOn(DragDropItemCursor.Default);
			ShowCursorCross(DragDropItemCursor.Default);
		}
		void IDragDropDispatcherClient.OnDragLeave() {
			RestoreCursor();
			DragCursorOff(DragDropItemCursor.Default);
		}
		void IDragDropDispatcherClient.DoDragging(Point clientPoint) {
			ShowCursorCross(DragDropItemCursor.Default);
			DragCursorOn(DragDropItemCursor.Default);
			DragCursorSetPos(DragDropItemCursor.Default, clientPoint);
		}
		void IDragDropDispatcherClient.DoDrop(Point clientPoint) {
			BaseLayoutItem dragItem = DragDropDispatcherFactory.Default.DragItem;
			if(!dragItem.IsHidden && dragItem.Owner != null) {
				dragItem.HideToCustomization();
			}
			RestoreCursor();
			DragCursorOff(DragDropItemCursor.Default);
		}
		void IDragDropDispatcherClient.DoBeginDrag() {
			ShowCursorCross(DragDropItemCursor.Default);
			DragCursorOn(DragDropItemCursor.Default);
		}
		void IDragDropDispatcherClient.DoDragCancel() {
			RestoreCursor();
			DragCursorOff(DragDropItemCursor.Default);
			ResetPressedIndex();
		}
		#endregion
		private void DragCursorOn(DragDropItemCursor cursor) {
			if(!cursor.Visible && !RDPHelper.IsRemoteSession) {
				cursor.FrameOwner = this;
				cursor.DragItem = DragDropDispatcherFactory.Default.DragItem;
				cursor.Visible = true;
				cursor.Update();
				cursor.BringToFront();
			}
		}
		private void DragCursorOff(DragDropItemCursor cursor) {
			if(cursor.Visible) cursor.Visible = false;
		}
		private void DragCursorSetPos(DragDropItemCursor cursor, Point clientPoint) {
			if(cursor.Visible) {
				Rectangle cursorRect = new Rectangle(clientPoint.X - (DragDropItemCursor.cursorWidth / 2), clientPoint.Y - (DragDropItemCursor.cursorHeight / 2), DragDropItemCursor.cursorWidth, DragDropItemCursor.cursorHeight);
				cursor.Location = PointToScreen(cursorRect.Location);
				cursor.Size = cursorRect.Size;
			}
		}
		void ShowCursorCross(DragDropItemCursor cursor) {
			bool canHide = (cursor.DragItem != null) && cursor.DragItem.AllowHide;
			cursor.Cursor = canHide ? DragManager.DragRemoveCursor : Cursors.No;
		}
		void RestoreCursor() {
			DragDropItemCursor.Default.Cursor = Cursors.Arrow;
		}
		public virtual int PressedItemIndex {
			get { return pressedItemIndex; }
			set {
				if(value != pressedItemIndex) {
					int oldIndex = pressedItemIndex;
					pressedItemIndex = value;
					InvalidateItem(oldIndex);
					InvalidateItem(pressedItemIndex);
					if(pressedItemIndex >= 0) {
						((ILayoutControl)OwnerControl).SelectionChanged(ViewInfo.GetItemByIndex(pressedItemIndex).Item as BaseLayoutItem);
					}
					Update();
				}
			}
		}
		public int HotTrackedItemIndex {
			get { return hotTrackedItemIndex; }
			set {
				if(value != hotTrackedItemIndex) {
					int oldIndex = hotTrackedItemIndex;
					hotTrackedItemIndex = value;
					InvalidateItem(oldIndex);
					InvalidateItem(hotTrackedItemIndex);
					Update();
				}
			}
		}
		void InvalidateItem(int index) {
			DevExpress.XtraEditors.ViewInfo.BaseListBoxViewInfo.ItemInfo itemInfo = ViewInfo.GetItemByIndex(index);
			if(itemInfo != null) Invalidate(itemInfo.Bounds);
		}
		public int IndexOf(object obj) {
			if(obj != null) {
				for(int i = 0; i < ViewInfo.VisibleItems.Count; i++) {
					if(obj.Equals(ViewInfo.GetItemByIndex(i).Item)) return i;
				}
			}
			return -1;
		}
		protected override void RaiseDrawItem(ListBoxDrawItemEventArgs e) {
			if(e.Index < 0 || e.Index >= Items.Count) return;
			DrawItemObject(e.Cache, e.Bounds, e.Item as BaseLayoutItem, e.Index);
			base.RaiseDrawItem(e);
			e.Handled = true;
		}
		internal ListBoxScrollInfo ItemsListScrollInfo {
			get { return ScrollInfo; }
		}
		public virtual void UpdateContent() {
			if(!Visible || IsDisposing) return;
			int scrollPosition = 0;
			if(ScrollInfo != null && ScrollInfo.VScroll != null) scrollPosition = ScrollInfo.VScroll.Value;
			Items.Clear();
			if(OwnerControl == null || OwnerControl.HiddenItems == null) return;
			ArrayList nonFixedItems = new ArrayList(OwnerControl.HiddenItems);
			if(OwnerControl.HiddenItemsSortComparer != null && nonFixedItems.Count > 0) {
				nonFixedItems.Sort(OwnerControl.HiddenItemsSortComparer);
			}
			foreach(BaseLayoutItem fixedItem in OwnerControl.HiddenItems.FixedItems) Items.Add(fixedItem);
			foreach(BaseLayoutItem nonFixedItem in nonFixedItems) {
				if(nonFixedItem.ShowInCustomizationForm) Items.Add(nonFixedItem);
			}
			if(ScrollInfo != null && ScrollInfo.VScroll != null) ScrollInfo.VScroll.Value = scrollPosition;
		}
		protected override void OnVisibleChanged(EventArgs e) {
			UpdateContent();
			UpdateCustomizationPropertyGridOrPanel();
		}
		protected internal static AppearanceDefault UpdateSystemColors(ISkinProvider provider, AppearanceDefault info) {
			info.ForeColor = CommonSkins.GetSkin(provider).TranslateColor(info.ForeColor);
			info.BackColor = CommonSkins.GetSkin(provider).TranslateColor(info.BackColor);
			return info;
		}
		protected internal static AppearanceDefault UpdateAppearance(ISkinProvider provider, string elementName, AppearanceDefault info) {
			SkinElement element = GridSkins.GetSkin(provider)[elementName];
			if(element == null) return info;
			element.Apply(info);
			return info;
		}
		protected internal static AppearanceDefault UpdateAppearanceEx(ISkinProvider provider, string elementName, AppearanceDefault info) {
			info = UpdateSystemColors(provider, info);
			return UpdateAppearance(provider, elementName, info);
		}
		protected virtual void DrawItemObject(GraphicsCache cache, Rectangle bounds, BaseLayoutItem item, int itemIndex) {
			if(hPainter != null) {
				bounds = new Rectangle(bounds.X + 2, bounds.Y + 1, bounds.Width - 3, bounds.Height - 1);
				using(AppearanceObject appearance = GetItemPaintAppearance()) {
					if(item is IFixedLayoutControlItem)
						appearance.Font = new Font(appearance.Font, FontStyle.Bold);
					else
						appearance.Font = new Font(appearance.Font, FontStyle.Regular);
					hPainter.Paint(cache.Graphics, item, CalcItemState(itemIndex), bounds, appearance);
				}
			}
		}
		protected AppearanceObject GetItemPaintAppearance() {
			FrozenAppearance appearance = new FrozenAppearance(AppearanceDefault.Control);
			if(OwnerControl != null) {
				if(OwnerControl.LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin) {
					appearance = new FrozenAppearance(
						UpdateAppearanceEx(OwnerControl.LookAndFeel, GridSkins.SkinHeader,
						new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Near, VertAlignment.Center)));
				}
				if(appearance.ForeColor == Color.Transparent || appearance.ForeColor == Color.Empty)
					appearance.ForeColor = LookAndFeelHelper.GetTransparentForeColor(OwnerControl.LookAndFeel, this);
			}
			return appearance;
		}
		protected ObjectState CalcItemState(int itemIndex) {
			ObjectState state = ObjectState.Normal;
			if(itemIndex == hotTrackedItemIndex) state = ObjectState.Hot;
			if(itemIndex == pressedItemIndex) state = ObjectState.Pressed;
			return state;
		}
		Size oldSize;
		protected override void OnSizeChanged(EventArgs e) {
			if(oldSize != Size) { Invalidate(); oldSize = Size; }
			base.OnSizeChanged(e);
		}
		public virtual int GetItemHeight() {
			using(AppearanceObject appearance = GetItemPaintAppearance()) {
				using(Graphics g = Graphics.FromHwnd(Handle)) {
					return Math.Max(24, appearance.CalcDefaultTextSize(g).Height + 8);
				}
			}
		}
		protected object ItemByPoint(Point pt) {
			XtraEditors.ViewInfo.BaseListBoxViewInfo.ItemInfo info = ViewInfo.GetItemInfoByPoint(pt);
			if(info == null) return null;
			return info.Item;
		}
		void UpdatePressedIndex(Point point) {
			PressedItemIndex = IndexFromPoint(point);
		}
		void UpdateHotTrackedIndex(Point point) {
			HotTrackedItemIndex = IndexFromPoint(point);
		}
		void ResetHotTrackedIndex() {
			HotTrackedItemIndex = -1;
		}
		protected virtual void ResetPressedIndex() {
			PressedItemIndex = -1;
		}
		protected internal void ProcessMessage(EventType eventType, MouseEventArgs e) {
			switch(eventType) {
				case EventType.MouseDown: OnMouseDown(e); break;
				case EventType.MouseUp: OnMouseUp(e); break;
				case EventType.MouseMove: OnMouseMove(e); break;
				case EventType.MouseLeave: OnMouseLeave(EventArgs.Empty); break;
			}
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			UpdatePressedIndex(e.Location);
			DragDropDispatcherFactory.Default.ProcessMouseEvent(clientDescriptorCore, new ProcessEventEventArgs(EventType.MouseDown, e));
			UpdateCustomizationPropertyGridOrPanel();
			base.OnMouseDown(e);
		}
		private void UpdateCustomizationPropertyGridOrPanel() {
			BaseLayoutItem bli = null;
			if (PressedItemIndex >= Items.Count) PressedItemIndex = -1;
			if(PressedItemIndex != -1) bli = Items[PressedItemIndex] as BaseLayoutItem;
			CustomizationForm form = GetCustomizationForm();
			if(form == null) return;
			if(OwnerControl == null) return;
			if(!Visible && form.panelTemplateItem != null) {
				form.panelTemplateItem.Visibility = LayoutVisibility.Never;
				if(!OwnerControl.OptionsCustomizationForm.ShowPropertyGrid) {
					form.propertyGridItem.Visibility = LayoutVisibility.Never;
					form.splitterItem1.Visibility = LayoutVisibility.Never;
				}
				if(OwnerControl.OptionsCustomizationForm.ShowPropertyGrid) {
					form.propertyGridItem.Visibility = LayoutVisibility.Always;
					form.splitterItem1.Visibility = LayoutVisibility.Always;
				}
				return;
			}
			if(bli == null) return;
			if(bli.Tag is TemplateManager) {
				form.panelTemplateItem.Visibility = LayoutVisibility.Always;
				form.propertyGridItem.Visibility = LayoutVisibility.Never;
				form.splitterItem1.Visibility = LayoutVisibility.Always;
				UpdatePanelTemplate();
			} else {
				form.panelTemplateItem.Visibility = LayoutVisibility.Never;
				if(!OwnerControl.OptionsCustomizationForm.ShowPropertyGrid) {
					form.propertyGridItem.Visibility = LayoutVisibility.Never;
					form.splitterItem1.Visibility = LayoutVisibility.Never;
				}
				if(OwnerControl.OptionsCustomizationForm.ShowPropertyGrid) {
					form.propertyGridItem.Visibility = LayoutVisibility.Always;
					form.splitterItem1.Visibility = LayoutVisibility.Always;
				}
			}
		}
		int oldPressedItemIndex = -1;
		protected void UpdatePanelTemplate() {
			if(oldPressedItemIndex == PressedItemIndex) return;
			BaseLayoutItem bli = ViewInfo.GetItemByIndex(PressedItemIndex).Item as BaseLayoutItem;
			oldPressedItemIndex = PressedItemIndex;
			Panel panelPreviewControl = GetCustomizationForm().panelTemplate;
			TemplateManager tManager = bli.Tag as TemplateManager;
			if(tManager != null && panelPreviewControl != null) ShowTemplatePreview(panelPreviewControl, tManager);
		}
		static void ShowTemplatePreview(Panel panelPreviewControl, TemplateManager tManager) {
			panelPreviewControl.Controls.Clear();
			LayoutControl childItemLayoutControl = new LayoutControl();
			childItemLayoutControl.Root.Padding = childItemLayoutControl.Root.Spacing = new Utils.Padding(0);
			childItemLayoutControl.AllowCustomization = false;
			Rectangle rectangle = GetUnionItemsSize(tManager);
			childItemLayoutControl.Size = rectangle.Size;
			try {
				tManager.RestoreTemplatePreview(null, childItemLayoutControl, new DevExpress.XtraLayout.Customization.LayoutItemDragController(null, childItemLayoutControl.Root, Point.Empty));
			} catch(TemplateException e) {
				ShowError(panelPreviewControl, e);
				return;
			}
			childItemLayoutControl.Name = tManager.Name;
			PanelPreviewUC parentItemLayoutControl = new PanelPreviewUC();
			panelPreviewControl.Controls.Add(parentItemLayoutControl);
			parentItemLayoutControl.Dock = DockStyle.Fill;
			LayoutControlItem lciChild = parentItemLayoutControl.restoreLayoutControlItem;
			try {
				childItemLayoutControl.Size = childItemLayoutControl.MinimumSize = childItemLayoutControl.MaximumSize = lciChild.MinSize = lciChild.MaxSize = rectangle.Size;
				lciChild.Control = childItemLayoutControl;
				lciChild.TextVisible = false;
			} catch(Exception e) {
				parentItemLayoutControl.Dispose();
				panelPreviewControl.Controls.Clear();
				ShowError(panelPreviewControl, e);
				return;
			}
			lciChild.FillControlToClientArea = false;
			lciChild.ControlAlignment = ContentAlignment.TopCenter;
			lciChild.Parent.Owner.Size = panelPreviewControl.Size;
			childItemLayoutControl.BackColor = Color.Transparent;
		}
		static Rectangle GetUnionItemsSize(TemplateManager tManager) {
			List<BaseLayoutItem> listBLI = new List<BaseLayoutItem>();
			foreach(BaseLayoutItem item in tManager.Items) {
				if(item.ParentName == tManager.Items[0].ParentName)
					listBLI.Add(item);
			}
			return BaseItemCollection.CalcItemsBounds(listBLI);
		}
		private static void ShowError(Panel panelPreviewControl, Exception e) {
			PanelPreviewErrorUC panelUC = new PanelPreviewErrorUC(e);
			panelPreviewControl.Controls.Add(panelUC);
			panelUC.Dock = DockStyle.Fill;
		}
		CustomizationForm GetCustomizationForm() {
			Control control = this;
			int watchDog = 0;
			while(control.Parent != null && watchDog++ < 20) {
				if(control.Parent is CustomizationForm) return control.Parent as CustomizationForm;
				control = control.Parent;
			}
			return null;
		}
		void WmMouseMove(ref Message m) {
			if(!this.GetStyle(ControlStyles.UserMouse)) {
				this.DefWndProc(ref m);
			}
			MouseButtons mb = System.Windows.Forms.MouseButtons.None;
			switch(m.WParam.ToInt32()) {
				case 1: mb = System.Windows.Forms.MouseButtons.Left; break;
				case 2: mb = System.Windows.Forms.MouseButtons.Right; break;
			}
			this.OnMouseMove(new MouseEventArgs(mb, 0, RenamerTextBox.SignedLOWORD(m.LParam.ToInt32()), RenamerTextBox.SignedHIWORD(m.LParam.ToInt32()), 0));
		}
		protected override void WndProc(ref Message m) {
			if(m.Msg == MSG.WM_MOUSEMOVE) {
				WmMouseMove(ref m);
			} else base.WndProc(ref m);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			DragDropDispatcherFactory.Default.ProcessMouseEvent(clientDescriptorCore, new ProcessEventEventArgs(EventType.MouseUp, e));
		}
		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			ResetHotTrackedIndex();
			if(AllowResetFocusOnMouseLeave) {
				ResetPressedIndex();
			}
		}
		Point prevPoint = Point.Empty;
		protected override void OnMouseMove(MouseEventArgs e) {
			UpdateHotTrackedIndex(e.Location);
			DragDropDispatcherFactory.Default.ProcessMouseEvent(clientDescriptorCore, new ProcessEventEventArgs(EventType.MouseMove, e));
		}
		public void AddLayoutItem(BaseLayoutItem item) {
			this.Items.Add(item);
		}
		public void RemoveItem(BaseLayoutItem item) {
			this.Items.Remove(item);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(Items != null) Items.Clear();
				UnRegister();
				if(clientDescriptorCore != null) {
					DragDropDispatcherFactory.Default.UnRegisterClient(clientDescriptorCore);
					clientDescriptorCore = null;
				}
			}
			base.Dispose(disposing);
		}
		protected override SearchControlProviderBase CreateSearchControlProviderBase() {
			return new HiddenItemsCriteriaProvider(this);
		}
		protected override ListBoxItemCollection CreateItemsCollection() {
			return new HiddenListBoxItemCollection();
		}
#if DEBUGTEST
		protected internal void CallOnMouseDown(MouseEventArgs e) { OnMouseDown(e); }
		protected internal void CallOnMouseUp(MouseEventArgs e) { OnMouseUp(e); }
		protected internal void CallOnMouseMove(MouseEventArgs e) { OnMouseMove(e); }
#endif
	}
	public class TemplateItemsList :HiddenItemsList {
		public override void UpdateContent() {
			if(!Visible || IsDisposing) return;
			Items.Clear();
			if(OwnerControl == null || OwnerControl.HiddenItems == null) return;
			if(OwnerControl.DesignMode) {
				if(TemplateManager.GetTemplateCollection().Count > 0) {
					foreach(BaseLayoutItem templateItem in TemplateManager.GetTemplateCollection()) {
						Items.Add(templateItem);
					}
					PressedItemIndex = 0;
				}
			}
		}
		protected override void DrawItemObject(GraphicsCache cache, Rectangle bounds, BaseLayoutItem item, int itemIndex) {
			if(hPainter != null) {
				bounds = new Rectangle(bounds.X + 2, bounds.Y + 1, bounds.Width - 3, bounds.Height - 1);
				using(AppearanceObject appearance = GetItemPaintAppearance()) {
					TemplateManager tm = item.Tag as TemplateManager;
					if(tm != null && tm.IsStandartTemplate)
						appearance.Font = new Font(appearance.Font, FontStyle.Bold);
					else
						appearance.Font = new Font(appearance.Font, FontStyle.Regular);
					hPainter.Paint(cache.Graphics, item, CalcItemState(itemIndex), bounds, appearance);
				}
			}
		}
		protected override void ResetPressedIndex() {
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			if(e.Button == System.Windows.Forms.MouseButtons.Right) {
				if(PressedItemIndex < 0 || PressedItemIndex > Items.Count) return;
				BaseLayoutItem item = ViewInfo.GetItemByIndex(PressedItemIndex).Item as BaseLayoutItem;
				if(item == null) return;
				if(!File.Exists(item.Name)) return;
				popUpItem = item;
				Point popupPoint = PointOperations.Subtract(PointToScreen(e.Location), OwnerControl.Control.PointToScreen(Point.Empty));
				OwnerControl.CustomizationMenuManager.ShowPopUpMenuForTempalte(this, popupPoint);
			}
		}
		BaseLayoutItem popUpItem;
		internal void DeleteTemplate(object sender, EventArgs e) {
			if(popUpItem != null) {
				try {
					File.Delete(popUpItem.Name);
					UpdateContent();
					UpdatePanelTemplate();
					popUpItem = null;
				} catch { }
			}
		}
	}
	public class HiddenItemsCriteriaProvider :ListBoxCriteriaProvider {
		const string columnToSearch = "CustomizationFormText";
		public HiddenItemsCriteriaProvider(HiddenItemsList listBox)
			: base(listBox) {
		}
		protected override Data.Filtering.CriteriaOperator CalcActiveCriteriaOperatorCore(SearchControlQueryParamsEventArgs args, Data.Helpers.FindSearchParserResults result) {
			return DxFtsContainsHelper.Create(new string[] { columnToSearch }, result, args.FilterCondition);
		}
	}
	public class HiddenListBoxItemCollection :ListBoxItemCollection {
		protected override SearchDataAdapter CreateDataAdapter() {
			return new SearchDataAdapter<BaseLayoutItem>();
		}
		protected override int GetSourceIndexCore(int filteredIndex) {
			if(AdapterEnabled) {
				object val = adapter.GetRow(filteredIndex);
				return List.IndexOf(val);
			}
			return filteredIndex;
		}
	}
}
