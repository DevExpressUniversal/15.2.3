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
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
namespace DevExpress.Web.Design {
	class FormLayoutItemsEditorFrame : ItemsEditorFrame {
		const string RegenerateItemsMessage = "Would you like to regenerate the layout items?\r\nWarning: this will delete all existing items.\r\n\r\nRefresh items for '{0}'";
		const string GenerateDefaultLayoutMessage = "Are you sure you want to create the default layout items?\r\nAll current edit form layout items will be removed.";
		Size LayoutViewPanelMinSize = new Size(220, 200);
		int LayoutViewPanelNormalWidth = 400;
		int RightPanelNormalWidth = 320;
		Size FrameMinimumSize = new Size(700, 580);
		PanelControl layoutViewPanel;
		int layoutViewPanelWidth = -1;
		HashSet<IUpdatableViewControl> updatableViewControls;
		public FormLayoutItemsEditorFrame()
			: base() {
			MinimumSize = FrameMinimumSize;
		}
		SplitContainerControl RightSplitContainer { get; set; }
		PanelControl LayoutViewPanel {
			get {
				if(layoutViewPanel == null)
					layoutViewPanel = RightSplitContainer.Panel1;
				return layoutViewPanel;
			}
		}
		internal FormLayoutView LayoutView { get; set; }
		FormLayoutItemsOwner FormLayoutItemsOwner { get { return (FormLayoutItemsOwner)ItemsOwner; } }
		HashSet<IUpdatableViewControl> UpdatableViewControls {
			get {
				if(updatableViewControls == null)
					updatableViewControls = new HashSet<IUpdatableViewControl>();
				return updatableViewControls;
			}
		}
		public override void DoInitFrame() {
			base.DoInitFrame();
			FormLayoutItemsOwner.OnUpdateView = OnUpdateView;
		}
		protected void OnUpdateView() {
			foreach(var control in UpdatableViewControls)
				control.UpdateView();
		}
		protected override void SaveChanges() {
			FormLayoutItemsOwner.SaveChanges();
		}
		public override void StoreLocalProperties(DevExpress.Utils.Design.PropertyStore localStore) {
			base.StoreLocalProperties(localStore);
			localStore.AddProperty(GetPropertyPath("RightSplitContainerSplitterPosition"), RightSplitContainer.SplitterPosition);
			localStore.AddProperty(GetPropertyPath("LayoutViewPanelWidth"), LayoutViewPanel.Width);
		}
		public override void RestoreLocalProperties(DevExpress.Utils.Design.PropertyStore localStore) {
			base.RestoreLocalProperties(localStore);
			RightSplitContainer.SplitterPosition = localStore.RestoreIntProperty(GetPropertyPath("RightSplitContainerSplitterPosition"), RightPanelNormalWidth);
			layoutViewPanelWidth = LayoutViewPanel.Width = localStore.RestoreIntProperty(GetPropertyPath("LayoutViewPanelWidth"), LayoutViewPanelNormalWidth);
		}
		protected override void CreateRightPanelPart() {
			RightSplitContainer = new SplitContainerControl() { Name = "RightSplitContainer" };
			RightSplitContainer.Parent = MainSplitContainer.Panel2;
			RightSplitContainer.Dock = DockStyle.Fill;
			RightSplitContainer.FixedPanel = SplitFixedPanel.Panel2;
			RightSplitContainer.Panel1.MinimumSize = LayoutViewPanelMinSize;
			RightSplitContainer.Resize += RightSplitContainer_Resize_UpdateLayoutViewWidth;
			RightSplitContainer.SplitterMoved += RightSplitContainer_SplitterMoved_UpdateLayoutViewWidth;
			CreateLayoutView();
			RightPanel = RightSplitContainer.Panel2;
		}
		void RightSplitContainer_SplitterMoved_UpdateLayoutViewWidth(object sender, EventArgs e) { UpdateLayoutViewWidth(); }
		void RightSplitContainer_Resize_UpdateLayoutViewWidth(object sender, EventArgs e) { UpdateLayoutViewWidth(); }
		void UpdateLayoutViewWidth() {
			if(!PostponeControlsCreated)
				return;
			if(layoutViewPanelWidth != -1) {
				layoutViewPanel.Width = layoutViewPanelWidth;
				if(layoutViewPanelWidth != layoutViewPanel.Width)
					return;
				layoutViewPanelWidth = -1;
			}
			RightSplitContainer.Resize -= RightSplitContainer_Resize_UpdateLayoutViewWidth;
			RightSplitContainer.SplitterMoved -= RightSplitContainer_SplitterMoved_UpdateLayoutViewWidth;
			RightSplitContainer.Resize += (s, e) => { UpdateSplitterPosition(); };
			RightSplitContainer.SplitterMoved += (s, e) => { UpdateSplitterPosition(); };
		}
		void UpdateSplitterPosition() {
			var leftSplitter = RightSplitContainer.Panel2.Left - 12;
			if(LayoutViewPanel.Width > leftSplitter)
				RightSplitContainer.SplitterPosition -= LayoutViewPanel.Width - leftSplitter;
		}
		void CreateLayoutView() {
			LayoutViewPanel.MinimumSize = LayoutViewPanelMinSize;
			LayoutViewPanel.AlwaysScrollActiveControlIntoView = false;
			LayoutViewPanel.AutoScroll = true;
			LayoutViewPanel.Margin = new System.Windows.Forms.Padding(14);
			Load += (s, e) => { PostponeCreateLayoutView(); };
		}
		void PostponeCreateLayoutView() {
			PostponeCreateControls += (s, e) => {
				SuspendLayout();
				LayoutView = new FormLayoutView(FormLayoutItemsOwner);
				LayoutView.ItemsContextMenu.BeforePopup += TreeListContextMenu_BeforePopup;
				LayoutViewPanel.Controls.Add(LayoutView);
				LayoutView.ShowRootLayout();
				LayoutView.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
				LayoutView.Width = LayoutViewPanel.Width;
				ResumeLayout();
				FillUpdateViewControls(this);
			};
		}
		bool expandItems;
		protected override void UpdateTreeListItems() {
			base.UpdateTreeListItems();
			if(!expandItems) {
				TreeListItems.ExpandAll();
				expandItems = false;
			}
		}
		void FillUpdateViewControls(Control control) {
			if(control is IUpdatableViewControl)
				UpdatableViewControls.Add((IUpdatableViewControl)control);
			foreach(Control childControl in control.Controls)
				FillUpdateViewControls(childControl);
		}
		protected override void EndDataOwnerUpdate(TreeListNodesState validatedState) {
			base.EndDataOwnerUpdate(validatedState);
			if(LayoutView != null)
				LayoutView.ShowLayout(FormLayoutItemsOwner.RootLayoutGroup);
		}
		protected override void ProcessMenuItemAction(DesignEditorDescriptorItem rootItem, DesignEditorDescriptorItem descriptorItem) {
			switch(descriptorItem.ActionType) {
				case DesignEditorMenuRootItemActionType.IncreaseColumn:
					FormLayoutItemsOwner.IncreaseDecreaseFocusedGroupColCount(true);
					break;
				case DesignEditorMenuRootItemActionType.DecreaseColumn:
					FormLayoutItemsOwner.IncreaseDecreaseFocusedGroupColCount(false);
					break;
				case DesignEditorMenuRootItemActionType.IncreaseColSpan:
					FormLayoutItemsOwner.IncreaseDecreaseFocusedItemColSpan(true);
					break;
				case DesignEditorMenuRootItemActionType.DecreaseColSpan:
					FormLayoutItemsOwner.IncreaseDecreaseFocusedItemColSpan(false);
					break;
				case DesignEditorMenuRootItemActionType.IncreaseRowSpan:
					FormLayoutItemsOwner.IncreaseDecreaseFocusedItemRowSpan(true);
					break;
				case DesignEditorMenuRootItemActionType.DecreaseRowSpan:
					FormLayoutItemsOwner.IncreaseDecreaseFocusedItemRowSpan(false);
					break;
				case DesignEditorMenuRootItemActionType.RetriveFields:
					OnRetrieveFields(FormLayoutItemsOwner);
					break;
				case DesignEditorMenuRootItemActionType.CreateDefaultItems:
					OnCreateDefaultItems();
					break;
				default:
					base.ProcessMenuItemAction(rootItem, descriptorItem);
					break;
			}
		}
		public static void OnRetrieveFields(FormLayoutItemsOwner owner) {
			var formLayout = owner.FormLayout;
			if(formLayout.Items.Count > 0 && ConfirmMessageBox.Show(string.Format(RegenerateItemsMessage, formLayout.ID), false, owner.ServiceProvider).Dialogresult == DialogResult.None)
				return;
			var serviceProvider = owner.ServiceProvider;
			var context = owner.Context;
			var typeHolder = new DataTypeContainer();
			if(DesignUtils.ShowDialog(serviceProvider, new FormLayoutChooseDataTypeForm(formLayout, context, serviceProvider, typeHolder)) == DialogResult.OK)
				owner.GenerateItemsByDataType(typeHolder.DataType);
		}
		void OnCreateDefaultItems() {
			var formLayout = FormLayoutItemsOwner.FormLayout;
			if(!FormLayoutItemsOwner.ShowConfirmCreateDefaultLayout() ||
				ConfirmMessageBox.Show(string.Format(GenerateDefaultLayoutMessage, formLayout.ID), false, FormLayoutItemsOwner.ServiceProvider).Dialogresult == DialogResult.OK)
				FormLayoutItemsOwner.CreateDefaultLayout();
			TreeListItems.ExpandToLevel(0);
		}
	}
	public class FormLayoutView : UserControl, IUpdatableViewControl {
		const int CanvasRightPadding = 1;
		const int DraggingHoverWeight = 4;
		const int DraggingOffset = 8;
		const int dragCursorTextBoxMargin = 10;
		const int dragCursorTextBoxBorderSize = 1;
		const int dragCursorHotSpotIconSize = 0;
		bool isCanvasHeightCalculating = false;
		Rectangle MovingDirectionRect = Rectangle.Empty;
		Rectangle oldTargetRect;
		DXPopupMenu itemsContextMenu;
		Graphics movingGraphics;
		Graphics imageGraphics;
		Font dragCursorFont;
		Pen dragCursorBorderPen;
		SolidBrush dragCursorFontBrush;
		SolidBrush dragCursorTextBoxBackgroundBrush;
		public FormLayoutView(FormLayoutItemsOwner itemsOwner) {
			ItemsOwner = itemsOwner;
			RootElement = ItemsOwner.RootLayoutGroup;
			DoubleBuffered = true;
			CursorCache = new Dictionary<string, System.Windows.Forms.Cursor>();
		}
		LayoutItemBase CachedItem { get; set; }
		LayoutGroupMap CachedMap { get; set; }
		FormLayoutItemsOwner ItemsOwner { get; set; }
		LayoutItemBase RootElement { get; set; }
		public Graphics Graphics { get; set; }
		Bitmap LayoutViewBitmap { get; set; }
		InsertDirection MovingDirection { get { return TargetMovingItem != null ? TargetMovingItem.Direction : InsertDirection.None; } }
		Rectangle OldTargetRect {
			get {
				if(oldTargetRect == null)
					oldTargetRect = new Rectangle();
				return oldTargetRect;
			}
			set {
				oldTargetRect.Location = value.Location;
				oldTargetRect.Size = value.Size;
			}
		}
		MovingItem TargetMovingItem { get; set; }
		public DXPopupMenu ItemsContextMenu {
			get {
				if(itemsContextMenu == null)
					itemsContextMenu = new DXPopupMenu();
				return itemsContextMenu;
			}
		}
		bool ItemDragging { get; set; }
		Point MouseDownLocation { get; set; }
		Graphics MovingGraphics {
			get {
				if(movingGraphics == null)
					movingGraphics = CreateGraphics();
				return movingGraphics;
			}
		}
		Dictionary<string, Cursor> CursorCache { get; set; }
		Point ScreenLocation { get; set; }
		Bitmap StoreBitmap { get; set; }
		Graphics ImageGraphics {
			get {
				bool needCreate = false;
				if(StoreBitmap == null) {
					needCreate = true;
				} else if(OldTargetRect.Width != StoreBitmap.Width) {
					needCreate = true;
					StoreBitmap.Dispose();
				}
				if(needCreate) {
					if(OldTargetRect.Height == DraggingHoverWeight)
						StoreBitmap = new Bitmap(OldTargetRect.Width, DraggingHoverWeight);
					else
						StoreBitmap = new Bitmap(DraggingHoverWeight, OldTargetRect.Height);
					imageGraphics = Graphics.FromImage(StoreBitmap);
				}
				return imageGraphics;
			}
		}
		Font DragCursorFont {
			get {
				if(dragCursorFont == null)
					dragCursorFont = new Font("Tahoma", 12);
				return dragCursorFont;
			}
		}
		Pen DragCursorBorderPen {
			get {
				if(dragCursorBorderPen == null)
					dragCursorBorderPen = new Pen(Color.FromArgb(60, 212, 212, 212), dragCursorTextBoxBorderSize);
				return dragCursorBorderPen;
			}
		}
		SolidBrush DragCursorFontBrush {
			get {
				if(dragCursorFontBrush == null)
					dragCursorFontBrush = new SolidBrush(Color.FromArgb(150, 150, 150));
				return dragCursorFontBrush;
			}
		}
		SolidBrush DragCursorTextBoxBackgroundBrush {
			get {
				if(dragCursorTextBoxBackgroundBrush == null)
					dragCursorTextBoxBackgroundBrush = new SolidBrush(Color.White);
				return dragCursorTextBoxBackgroundBrush;
			}
		}
		Bitmap DragCursorBitmap { get; set; }
		Cursor DragCursor { get; set; }
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			foreach(var cursor in CursorCache.Values)
				cursor.Dispose();
		}
		public LayoutItemBase ShowLayout(LayoutItemBase itemToSelect) {
			return ShowLayout(itemToSelect, Point.Empty);
		}
		public LayoutItemBase ShowLayout(Point clickPoint) {
			return ShowLayout(null, clickPoint);
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			ShowFocusedLayout(e.X, e.Y);
			if(e.Button == System.Windows.Forms.MouseButtons.Left)
				MouseDownLocation = e.Location;
		}
		internal void ShowFocusedLayout(int x, int y) {
			ItemsOwner.ChangeFocusedItem(ShowLayout(new Point(x, y)));
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			if(ItemDragging)
				DragItem(e.Location);
			else if(e.Button == System.Windows.Forms.MouseButtons.Left && ItemMovingDetected(e.Location))
				StartItemDragging();
		}
		bool ItemMovingDetected(Point mouseLocation) {
			if(MouseDownLocation.IsEmpty)
				return false;
			var point = new Point(mouseLocation.X - MouseDownLocation.X, mouseLocation.Y - MouseDownLocation.Y);
			return Math.Abs(point.X) > DraggingOffset || Math.Abs(point.Y) > DraggingOffset;
		}
		void StartItemDragging() {
			ItemDragging = true;
			ScreenLocation = PointToScreen(Point.Empty);
			DetermineNeighbours();
			CreateDraggingCursor();
			CommonDesignerServiceRegisterHelper.SetEscapeBtnUpService(ItemsOwner.ServiceProvider, () => {
				var result = ItemDragging;
				RedrawOldTargetSelection(Rectangle.Empty);
				StopItemDragging();
				return result;
			});
		}
		void DetermineNeighbours() {
			if(CachedMap != null) {
				var layoutItem = CachedMap.GetItemFromLocation(MouseDownLocation.X, MouseDownLocation.Y);
				if(layoutItem != null)
					ItemsOwner.FillNeighbourItems(layoutItem);
			}
		}
		void DragItem(Point location) {
			TargetMovingItem = CachedMap.GetMovingItemFromLocation(ItemsOwner, location.X, location.Y);
			if(TargetMovingItem.Path == RootElement.Path || (CachedItem != null && TargetMovingItem.Path == CachedItem.Path)) {
				RedrawOldTargetSelection(Rectangle.Empty);
				return;
			}
			UpdateCursor();
			ViewTargetSelection();
		}
		void UpdateCursor() {
			if(!ItemDragging)
				Cursor = Cursors.Default;
			else
				Cursor = MovingDirection != InsertDirection.None ? DragCursor : Cursors.No;
		}
		void CreateDraggingCursor() {
			var focusedItem = ItemsOwner.FocusedItem;
			if(focusedItem == null)
				return;
			if(DragCursorBitmap != null)
				DragCursorBitmap.Dispose();
			var caption = focusedItem.Caption;
			var textSize = DesignTimeFormHelper.GetTextSize(this, caption, DragCursorFont);
			var summaryMarginBorder = dragCursorTextBoxMargin * 2 + dragCursorTextBoxBorderSize * 2;
			var textBoxSize = new Size(textSize.Width + summaryMarginBorder, textSize.Height + summaryMarginBorder + dragCursorHotSpotIconSize);
			int cursorWidth = textBoxSize.Width + 10;
			int offsetX = cursorWidth - 20;
			int cursorHeight = textBoxSize.Height + 10;
			int offsetY = cursorHeight - 20;
			DragCursorBitmap = new Bitmap(cursorWidth + offsetX, cursorHeight + offsetY);
			using(var g = Graphics.FromImage(DragCursorBitmap)) {
				var textBoxRect = new Rectangle(offsetX, dragCursorHotSpotIconSize + offsetY, textBoxSize.Width, textBoxSize.Height - dragCursorHotSpotIconSize);
				g.DrawRectangle(DragCursorBorderPen, textBoxRect);
				var inflateValue = -dragCursorTextBoxBorderSize;
				textBoxRect.Inflate(inflateValue, inflateValue);
				g.FillRectangle(DragCursorTextBoxBackgroundBrush, textBoxRect);
				g.DrawString(caption, DragCursorFont, DragCursorFontBrush, dragCursorTextBoxMargin + offsetX, dragCursorTextBoxMargin + dragCursorHotSpotIconSize + offsetY);
			}
			DragCursor = new Cursor(DragCursorBitmap.GetHicon());
		}
		void ViewTargetSelection() {
			if(MovingDirection == InsertDirection.None) {
				RedrawOldTargetSelection(Rectangle.Empty);
				return;
			}
			var targetRect = TargetMovingItem.TargetRect;
			if(targetRect.Width > 0 && targetRect.Height > 0) {
				switch(MovingDirection) {
					case InsertDirection.After:
						UpdateDirectionRect(targetRect.Left, targetRect.Bottom, targetRect.Width, DraggingHoverWeight);
						break;
					case InsertDirection.Before:
					case InsertDirection.Inside:
						UpdateDirectionRect(targetRect.Left, targetRect.Top, targetRect.Width, DraggingHoverWeight);
						break;
					case InsertDirection.Left:
						UpdateDirectionRect(targetRect.Left, targetRect.Top, DraggingHoverWeight, targetRect.Height);
						break;
					case InsertDirection.Right:
						UpdateDirectionRect(targetRect.Right - DraggingHoverWeight, targetRect.Top, DraggingHoverWeight, targetRect.Height);
						break;
				}
				RedrawOldTargetSelection(MovingDirectionRect);
				using(var brush = new SolidBrush(Color.Cyan))
					MovingGraphics.FillRectangle(brush, MovingDirectionRect);
			}
		}
		void UpdateDirectionRect(int x, int y, int width, int height) {
			var rect = new Rectangle(x, y, width, height);
			if(MovingDirectionRect.IsEmpty || !MovingDirectionRect.Equals(rect))
				MovingDirectionRect = rect;
		}
		void RedrawOldTargetSelection(Rectangle rect) {
			if(!OldTargetRect.IsEmpty && OldTargetRect != rect)
				MovingGraphics.DrawImage(StoreBitmap, OldTargetRect);
			if(!rect.IsEmpty && OldTargetRect != rect) {
				OldTargetRect = rect;
				var destinationPoint = new Point(ScreenLocation.X + OldTargetRect.Location.X, ScreenLocation.Y + OldTargetRect.Location.Y);
				ImageGraphics.CopyFromScreen(destinationPoint, Point.Empty, OldTargetRect.Size);
			}
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			if(e.Button == MouseButtons.Right) {
				if(ItemsContextMenu == null)
					return;
				var position = PointToClient(MousePosition);
				var lookAndFeel = ((XtraForm)this.ParentForm).LookAndFeel;
				MenuManagerHelper.GetMenuManager(lookAndFeel, this).ShowPopupMenu(ItemsContextMenu, this, position);
			} else if(ItemDragging && MovingDirection != InsertDirection.None && TargetMovingItem.LayoutItem != null) {
				ItemsOwner.MoveFocusedItemTo(TargetMovingItem.LayoutItem, MovingDirection);
			}
			StopItemDragging();
		}
		void StopItemDragging() {
			MouseDownLocation = Point.Empty;
			ItemDragging = false;
			TargetMovingItem = null;
			OldTargetRect = Rectangle.Empty;
			ItemsOwner.ClearNeighbourItems();
			UpdateCursor();
			CommonDesignerServiceRegisterHelper.RemoveEscapeBtnUpService(ItemsOwner.ServiceProvider);
		}
		internal LayoutGroupMap TargetMap;
		public LayoutItemBase ShowRootLayout() {
			return ShowLayout(ItemsOwner.FormLayout.Root, Point.Empty, out TargetMap);
		}
		LayoutItemBase ShowLayout(LayoutItemBase itemToSelect, Point clickPoint) {
			LayoutGroupMap targetMap = null;
			return ShowLayout(itemToSelect, clickPoint, out targetMap);
		}
		LayoutItemBase ShowLayout(LayoutItemBase itemToSelect, Point clickPoint, out LayoutGroupMap targetMap) {
			targetMap = TargetMap != null ? TargetMap : LayoutGroupMap.CreateLayoutGroupMap(ItemsOwner.FormLayout, itemToSelect);
			ItemsOwner.FormLayout.CalculateRequiredAndOptionalFieldCounts();
			CalculateCanvasHeight(targetMap);
			var result = ShowLayoutCore(targetMap, itemToSelect, clickPoint);
			UpdateCache(result, targetMap);
			return result;
		}
		void ShowLayoutFromCache() {
			ShowLayoutCore(CachedMap, CachedItem, Point.Empty);
		}
		LayoutItemBase ShowLayoutCore(LayoutGroupMap targetMap, LayoutItemBase itemToSelect, Point clickPoint) {
			LayoutViewBitmap = new Bitmap(Width, Height);
			Graphics = Graphics.FromImage(LayoutViewBitmap);
			Graphics.Clear(BackColor);
			var selectedItem = targetMap.Draw(Graphics, BackColor, new Rectangle(0, 0, Width - CanvasRightPadding, 0), clickPoint, itemToSelect);
			Invalidate();
			return selectedItem;
		}
		void CalculateCanvasHeight(LayoutGroupMap targetMap) {
			try {
				isCanvasHeightCalculating = true;
				Height = targetMap.ProcessLayoutElement(RootElement);
			} finally {
				isCanvasHeightCalculating = false;
			}
		}
		void UpdateCache(LayoutItemBase item, LayoutGroupMap map) {
			CachedItem = item;
			CachedMap = map;
		}
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			if(LayoutViewBitmap != null)
				e.Graphics.DrawImage(LayoutViewBitmap, 0, 0);
		}
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			if(!isCanvasHeightCalculating)
				ShowLayoutFromCache();
		}
		void IUpdatableViewControl.UpdateView() {
			if(CachedItem != ItemsOwner.FocusedItem)
				ShowLayout(ItemsOwner.FocusedItem as LayoutItemBase);
		}
	}
	public enum FormLayoutViewEditMode { View, Edit }
	public class DataTypeContainer {
		private Type dataType;
		public Type DataType {
			get { return dataType; }
			set { dataType = value; }
		}
	}
	public class FormLayoutChooseDataTypeForm : ChooseDataTypeForm {
		const string ChooseDataTypeDescription = "The ASPxFormLayout control can automatically generate an edit form for objects of a particular type.\nSpecify the type of objects whose fields should be edited.";
		public FormLayoutChooseDataTypeForm(object component, ITypeDescriptorContext context, IServiceProvider provider, object propertyValue)
			: base(component, context, provider, propertyValue, ChooseDataTypeDescription) { }
	}
	public class ChooseDataTypeForm : EditorFormBase {
		private const string DXTypeNameStarting = "DevExpress.";
		private const int MaxGetTypesAttemptCount = 3;
		private string[] filteredPublicKeys = new string[] {
			"b77a5c561934e089", "b03f5f7f11d50a3a", "31bf3856ad364e35", "89845dcd8080cc91",
			"b88d1754d700e49a", "79868b8147b5eae4", "30ad4fe6b2a6aeed"
		};
		private System.Windows.Forms.ComboBox possibleDataTypesComboBox = new System.Windows.Forms.ComboBox();
		public ChooseDataTypeForm(object component, ITypeDescriptorContext context, IServiceProvider provider, object propertyValue, string description)
			: base(component, context, provider, propertyValue) {
			SuspendLayout();
			Label chooseDataTypeLabel = new Label();
			chooseDataTypeLabel.Text = description;
			chooseDataTypeLabel.Location = new Point(12, 20);
			chooseDataTypeLabel.AutoSize = true;
			possibleDataTypesComboBox.Location = new Point(12, 65);
			possibleDataTypesComboBox.Width = 540;
			possibleDataTypesComboBox.DropDownWidth = 540;
			possibleDataTypesComboBox.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
			possibleDataTypesComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			possibleDataTypesComboBox.DropDown += new EventHandler(possibleDataTypesComboBox_DropDown);
			FillPossibleDataTypesComboBox();
			Controls.Add(chooseDataTypeLabel);
			Controls.Add(possibleDataTypesComboBox);
			Size = FormMinimumSize;
			Text = "Choose Data Type...";
			ResumeLayout(false);
		}
		protected override void InitializeForm() {
			base.InitializeForm();
			MaximizeBox = MinimizeBox = CloseBox = false;
		}
		protected override string GetPropertyStorePathPrefix() {
			return "ChooseDataTypeForm";
		}
		protected override void SaveChanges() {
			((DataTypeContainer)PropertyValue).DataType = possibleDataTypesComboBox.SelectedItem as Type;
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			FillPossibleDataTypesComboBox();
		}
		protected override Size FormMinimumSize {
			get { return new Size(580, 200); }
		}
		private void possibleDataTypesComboBox_DropDown(object sender, EventArgs e) {
			FillPossibleDataTypesComboBox();
		}
		private void FillPossibleDataTypesComboBox() {
			possibleDataTypesComboBox.Items.Clear();
			possibleDataTypesComboBox.Items.AddRange(GetTypes());
		}
		private Type[] GetTypes() {
			ITypeDiscoveryService service = ServiceProvider.GetService(typeof(ITypeDiscoveryService)) as ITypeDiscoveryService;
			List<Type> types = new List<Type>();
			int attemptCounter = 0;
			do {
				types.Clear();
				foreach (Type type in service.GetTypes(typeof(object), true))
					if (IsTypeValid(type))
						types.Add(type);
			}
			while (types.Count == 0 && ++attemptCounter < MaxGetTypesAttemptCount);
			return types.ToArray();
		}
		private bool IsTypeValid(Type type) {
			return !type.IsAbstract && !type.IsInterface && !type.IsGenericTypeDefinition && !IsSignedWithFilteredPublicKey(type);
		}
		private bool IsSignedWithFilteredPublicKey(Type type) {
			var pkt = BitConverter.ToString(type.Assembly.GetName().GetPublicKeyToken()).Replace("-", string.Empty);
			return filteredPublicKeys.Contains(pkt, StringComparer.InvariantCultureIgnoreCase);
		}
	}
}
