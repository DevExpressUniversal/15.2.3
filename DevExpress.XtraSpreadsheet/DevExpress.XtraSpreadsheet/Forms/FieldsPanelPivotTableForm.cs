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

using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.DragDrop;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Customization;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
namespace DevExpress.XtraSpreadsheet.Forms {
	public partial class FieldsPanelPivotTableForm : XtraForm, IPivotFieldDragDropProvider, IPivotFieldItemPainter, IPivotTableFieldsPanel {
		#region Fields
		const int draggingFeedbackWidth = 120;
		static ImageCollection areaIcons = null;
		readonly SpreadsheetControl control;
		FieldListPanelPivotTableViewModel viewModel;
		PivotFieldListPanelBoxType initialBoxType;
		int initialPosition;
		PivotFieldViewInfo draggingItem;
		bool isDefferedShowing;
		SpreadsheetPivotTableFieldListStartPosition startPosition;
		Point startLocation;
		#endregion
		FieldsPanelPivotTableForm() {
			InitializeComponent();
		}
		public FieldsPanelPivotTableForm(SpreadsheetControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
			InitializeComponent();
			InitializeListBosex(control.MenuManager);
			SetAreaIcons();
		}
		#region Properties
		protected override bool ShowWithoutActivation { get { return true; } }
		#endregion
		public static ImageCollection AreaIcons {
			get {
				if (areaIcons == null)
					areaIcons = ImageHelper.CreateImageCollectionFromResources(
						"DevExpress.XtraSpreadsheet.Images.PivotFieldsPanelAreas.png",
						Assembly.GetExecutingAssembly(), new Size(16, 16));
				return areaIcons;
			}
		}
		void InitializeListBosex(IDXMenuManager menuManager) {
			lbFields.Type = PivotFieldListPanelBoxType.Fields;
			lbFilters.Type = PivotFieldListPanelBoxType.Filters;
			lbColumns.Type = PivotFieldListPanelBoxType.Columns;
			lbRows.Type = PivotFieldListPanelBoxType.Rows;
			lbValues.Type = PivotFieldListPanelBoxType.Values;
			lbFields.DragDropProvider = this;
			lbFilters.DragDropProvider = this;
			lbColumns.DragDropProvider = this;
			lbRows.DragDropProvider = this;
			lbValues.DragDropProvider = this;
			lbFields.ItemPainter = this;
			lbFilters.ItemPainter = this;
			lbColumns.ItemPainter = this;
			lbRows.ItemPainter = this;
			lbValues.ItemPainter = this;
			lbFilters.MenuManager = menuManager;
			lbColumns.MenuManager = menuManager;
			lbRows.MenuManager = menuManager;
			lbValues.MenuManager = menuManager;
		}
		void SetAreaIcons() {
			SetAreaIcon(pbFilter, 3);
			SetAreaIcon(pbColumns, 1);
			SetAreaIcon(pbRows, 2);
			SetAreaIcon(pbValues, 4);
		}
		void SetAreaIcon(PictureBox box, int pictureIndex) {
			Image image = AreaIcons.Images[pictureIndex];
			box.Image = image;
			box.Size = image.Size;
		}
		void SetBindings() {
			this.chkDeferLayoutUpdate.DataBindings.Clear();
			this.chkDeferLayoutUpdate.DataBindings.Add("EditValue", viewModel, "DeferLayoutUpdate", false, DataSourceUpdateMode.OnPropertyChanged);
			this.btnUpdate.DataBindings.Clear();
			this.btnUpdate.DataBindings.Add("Enabled", viewModel, "UpdateEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
		}
		void PopulateBoxes() {
			viewModel.BeginCalculateFieldCaptions();
			PopulateBox(lbFilters, viewModel.GetFilterInfos());
			PopulateBox(lbColumns, viewModel.GetColumnInfos());
			PopulateBox(lbRows, viewModel.GetRowInfos());
			PopulateBox(lbValues, viewModel.GetValueInfos());
			PopulateBox(lbFields, viewModel.EndCalculateFieldCaptions());
		}
		void PopulateBox(SpreadsheetCustomizationListBox box, List<PivotFieldViewInfo> infos) {
			box.Items.Clear();
			foreach (PivotFieldViewInfo info in infos)
				box.Items.Add(info);
		}
		void SubscribeEvents() {
			viewModel.DataChanged += OnDataChanged;
		}
		void UnsubscribeEvents() {
			if (viewModel != null)
				viewModel.DataChanged -= OnDataChanged;
		}
		void OnDataChanged(object sender, EventArgs e) {
			PopulateBoxes();
		}
		string IPivotFieldDragDropProvider.DraggingItemCaption { get { return draggingItem.Caption; } }
		void IPivotFieldDragDropProvider.DoDragDrop(PivotFieldListPanelBoxType initialBoxType, int initialPosition, PivotFieldViewInfo selectedItem, Point screenPoint) {
			using (PivotFieldsPanelDragManager dragManager = new PivotFieldsPanelDragManager(this, this)) {
				this.initialBoxType = initialBoxType;
				this.initialPosition = initialPosition;
				this.draggingItem = selectedItem;
				dragManager.DoDragDrop(new Size(draggingFeedbackWidth, SpreadsheetCustomizationListBox.ItemDrawingHeight), PointToClient(screenPoint));
			}
		}
		void IPivotFieldDragDropProvider.EndDrag(Point screenPoint) {
			DragState state = GetDragState(screenPoint);
			if (state != DragState.None) {
				SpreadsheetCustomizationListBox box = GetBoxByPoint(screenPoint);
				int position = box != null ? box.GetItemIndexByPoint(screenPoint) : -1;
				PivotFieldListPanelBoxType boxType = box != null ? box.Type : PivotFieldListPanelBoxType.None;
				MoveItem(boxType, position);
			}
			this.initialBoxType = PivotFieldListPanelBoxType.None;
			this.initialPosition = -1;
		}
		SpreadsheetCustomizationListBox GetBoxByPoint(Point screenPoint) {
			if (lbFields.ContainsPoint(screenPoint))
				return lbFields;
			if (lbFilters.ContainsPoint(screenPoint))
				return lbFilters;
			if (lbColumns.ContainsPoint(screenPoint))
				return lbColumns;
			if (lbRows.ContainsPoint(screenPoint))
				return lbRows;
			if (lbValues.ContainsPoint(screenPoint))
				return lbValues;
			return null;
		}
		PivotFieldListPanelBoxType GetBoxTypeByPoint(Point screenPoint) {
			SpreadsheetCustomizationListBox box = GetBoxByPoint(screenPoint);
			return box != null ? box.Type : PivotFieldListPanelBoxType.None;
		}
		void MoveItem(PivotFieldListPanelBoxType lastBoxType, int position) {
			PivotTableAxis from = viewModel.ConvertBoxTypeToAxis(initialBoxType);
			PivotTableAxis to = viewModel.ConvertBoxTypeToAxis(lastBoxType);
			viewModel.MovePivotField(draggingItem.FieldIndex, from, initialPosition, to, position);
		}
		SpreadsheetCustomizationListBox GetBoxByType(PivotFieldListPanelBoxType type) {
			switch (type) {
				case PivotFieldListPanelBoxType.Fields:
					return lbFields;
				case PivotFieldListPanelBoxType.Filters:
					return lbFilters;
				case PivotFieldListPanelBoxType.Columns:
					return lbColumns;
				case PivotFieldListPanelBoxType.Rows:
					return lbRows;
				case PivotFieldListPanelBoxType.Values:
					return lbValues;
			}
			return null;
		}
		public DragState GetDragState(Point screenPoint) {
			Point clientPoint = PointToClient(screenPoint);
			bool outsideClient = !ClientRectangle.Contains(clientPoint);
			PivotFieldListPanelOperationType operation = viewModel.GetOperation(draggingItem.FieldIndex, initialBoxType, GetBoxTypeByPoint(screenPoint), outsideClient);
			switch (operation) {
				case PivotFieldListPanelOperationType.NotAllowed:
					return DragState.None;
				case PivotFieldListPanelOperationType.Move:
					return DragState.Move;
				case PivotFieldListPanelOperationType.Remove:
					return DragState.Remove;
			}
			throw new InvalidOperationException();
		}
		Cursor IPivotFieldDragDropProvider.GetDragCursor(DragState state) {
			switch (state) {
				case DragState.Copy:
				case DragState.Move:
					return SpreadsheetCursors.DragItemMove.Cursor;
				case DragState.Remove:
					return SpreadsheetCursors.DragItemRemove.Cursor;
				case DragState.None:
				default:
					return SpreadsheetCursors.DragItemNone.Cursor;
			}
		}
		#region implement IPivotFieldItemPainter
		void IPivotFieldItemPainter.DrawItem(GraphicsCache cache, Rectangle bounds, string text, bool isSelected) {
			DrawItemCore(cache, bounds, text, isSelected);
		}
		void IPivotFieldItemPainter.DrawDraggingItem(GraphicsCache cache, Rectangle bounds, string text) {
			DrawItemCore(cache, bounds, text, false);
		}
		void DrawItemCore(GraphicsCache cache, Rectangle bounds, string text, bool isSelected) {
			HeaderObjectInfoArgs infoArgs = CreateInfoArgs(cache, bounds, text, isSelected);
			infoArgs.SetAppearance(lbFields.GetPaintAppearance()); 
			GetHeaderPainter().CalcObjectBounds(infoArgs);
			GetHeaderPainter().DrawObject(infoArgs);
		}
		HeaderObjectInfoArgs CreateInfoArgs(GraphicsCache cache, Rectangle bounds, string text, bool isSelected) {
			HeaderObjectInfoArgs result = new HeaderObjectInfoArgs();
			result.Caption = text;
			result.Bounds = bounds;
			result.CaptionRect = bounds;
			result.Cache = cache;
			result.HeaderPosition = HeaderPositionKind.Special;
			if (!Enabled)
				result.State = ObjectState.Disabled;
			else
				result.State = isSelected ? ObjectState.Pressed : ObjectState.Normal;
			return result;
		}
		HeaderObjectPainter GetHeaderPainter() {
			return LookAndFeel.ActiveLookAndFeel.Painter.Header;
		}
		#endregion
		void OnUpdateClick(object sender, EventArgs e) {
			viewModel.Update();
		}
		#region IPivotTableFieldsPanel implementation
		Point IPivotTableFieldsPanel.Location {
			get { return this.Location; }
			set {
				this.StartPosition = FormStartPosition.Manual;
				this.Location = value;
			}
		}
		void IPivotTableFieldsPanel.SetStartPosition(SpreadsheetPivotTableFieldListStartPosition position, Point location) {
			if (control.IsPainted)
				SetStartPosition(position, location);
			else {
				this.startPosition = position;
				this.startLocation = location;
			}
		}
		void SetStartPosition(SpreadsheetPivotTableFieldListStartPosition position, Point location) {
			if (position == SpreadsheetPivotTableFieldListStartPosition.CenterScreen)
				this.StartPosition = FormStartPosition.CenterScreen;
			else if (position == SpreadsheetPivotTableFieldListStartPosition.CenterSpreadsheetControl)
				this.StartPosition = FormStartPosition.CenterParent;
			else if (position == SpreadsheetPivotTableFieldListStartPosition.ManualScreen) {
				this.StartPosition = FormStartPosition.Manual;
				this.Location = location;
			}
			else if (position == SpreadsheetPivotTableFieldListStartPosition.ManualSpreadsheetControl) {
				this.StartPosition = FormStartPosition.Manual;
				Point newLocation = control.PointToScreen(Point.Empty);
				newLocation.Offset(location);
				this.Location = newLocation;
			}
		}
		void IPivotTableFieldsPanel.Show(FieldListPanelPivotTableViewModel viewModel) {
			ChangeViewModel(viewModel);
			if (control.IsPainted)
				ShowForm();
			else if (!isDefferedShowing) {
				isDefferedShowing = true;
				control.Paint += OnControlPaint;
			}
		}
		void OnControlPaint(object sender, PaintEventArgs e) {
			isDefferedShowing = false;
			control.Paint -= OnControlPaint;
			SetStartPosition(startPosition, startLocation);
			ShowForm();
		}
		void ShowForm() {
			control.ShowModelessForm(this, null);
		}
		void IPivotTableFieldsPanel.Hide() {
			if (isDefferedShowing)
				control.Paint -= OnControlPaint;
			DisposeViewModel();
			this.Close();
		}
		void DisposeViewModel() {
			UnsubscribeEvents();
			if (viewModel != null) {
				viewModel.Dispose();
				viewModel = null;
			}
		}
		void IPivotTableFieldsPanel.ChangeViewModel(FieldListPanelPivotTableViewModel viewModel) {
			ChangeViewModel(viewModel);
		}
		void ChangeViewModel(FieldListPanelPivotTableViewModel viewModel) {
			if (viewModel.Equals(this.viewModel))
				return;
			DisposeViewModel();
			this.viewModel = viewModel;
			SetBindings();
			PopulateBoxes();
			SubscribeEvents();
		}
		#region Closing
		EventHandler onClosing;
		public new event EventHandler Closing { add { onClosing += value; } remove { onClosing -= value; } }
		void RaiseClosing() {
			if (onClosing != null)
				onClosing(this, EventArgs.Empty);
		}
		#endregion
		protected override void OnClosing(CancelEventArgs e) {
			base.OnClosing(e);
			RaiseClosing();
		}
		#endregion
	}
	#region SpreadsheetCustomizationListBox
	[ToolboxItem(false)]
	public class SpreadsheetCustomizationListBox : CustomizationListBoxBase {
		#region Fields
		const int itemHeight = 19; 
		internal static int ItemDrawingHeight = itemHeight - 1;
		#endregion
		public SpreadsheetCustomizationListBox()
			: base() {
		}
		public override int GetItemHeight() {
			return itemHeight;
		}
		#region Properties
		public PivotFieldListPanelBoxType Type { get; set; }
		public IPivotFieldDragDropProvider DragDropProvider { get; set; }
		public IPivotFieldItemPainter ItemPainter { get; set; }
		public IDXMenuManager MenuManager { get; set; }
		#endregion
		public AppearanceObject GetPaintAppearance() {
			return ViewInfo.PaintAppearance;
		}
		protected override void ShowItemMenu(object item) {
		}
		protected override void DoDragDrop(object dragItem, Point p) {
			int index = Items.IndexOf(dragItem);
			DragDropProvider.DoDragDrop(Type, index, (PivotFieldViewInfo)dragItem, PointToScreen(p));
		}
		protected internal bool ContainsPoint(Point screenPoint) {
			Rectangle rect = new Rectangle(0, 0, Bounds.Width, Bounds.Height);
			return rect.Contains(PointToClient(screenPoint));
		}
		protected internal int GetItemIndexByPoint(Point screenPoint) {
			int index = IndexFromPoint(PointToClient(screenPoint));
			return index < 0 ? Items.Count : index;
		}
		protected override void DrawItemObject(GraphicsCache cache, int index, Rectangle bounds, DrawItemState itemState) {
			DrawItemObjectCore(cache, bounds, GetItemText(index), false);
		}
		protected internal void DrawItemObjectCore(GraphicsCache cache, Rectangle bounds, string text, bool isSelected) {
			bounds = PrepareDrawingBounds(bounds);
			ItemPainter.DrawItem(cache, bounds, text, isSelected);
		}
		Rectangle PrepareDrawingBounds(Rectangle bounds) {
			bounds.X++;
			bounds.Width -= 2;
			bounds.Y++;
			bounds.Height = ItemDrawingHeight;
			return bounds;
		}
	}
	#endregion
	#region SpreadsheetFieldsCustomizationListBox
	[ToolboxItem(false)]
	public class SpreadsheetFieldsCustomizationListBox : SpreadsheetCustomizationListBox {
		public SpreadsheetFieldsCustomizationListBox()
			: base() {
		}
		protected override void DrawItemObject(GraphicsCache cache, int index, Rectangle bounds, DrawItemState itemState) {
			PivotFieldViewInfo info = (PivotFieldViewInfo)Items[index];
			DrawItemObjectCore(cache, bounds, info.Caption, info.IsSelected);
		}
	}
	#endregion
	#region IPivotFieldDragDropProvider
	public interface IPivotFieldDragDropProvider {
		void DoDragDrop(PivotFieldListPanelBoxType initialBoxType, int initialPosition, PivotFieldViewInfo selectedItem, Point screenPoint);
		void EndDrag(Point screenPoint);
		string DraggingItemCaption { get; }
		DragState GetDragState(Point screenPoint);
		Cursor GetDragCursor(DragState state);
	}
	#endregion
	#region IPivotFieldItemPainter
	public interface IPivotFieldItemPainter {
		void DrawItem(GraphicsCache cache, Rectangle bounds, string text, bool isSelected);
		void DrawDraggingItem(GraphicsCache cache, Rectangle bounds, string text);
	}
	#endregion
	#region PivotFieldsPanelDragManager
	public class PivotFieldsPanelDragManager : DragManager {
		#region Fields
		readonly IPivotFieldDragDropProvider provider;
		readonly IPivotFieldItemPainter painter;
		#endregion
		public PivotFieldsPanelDragManager(IPivotFieldDragDropProvider provider, IPivotFieldItemPainter painter) {
			Guard.ArgumentNotNull(provider, "provider");
			Guard.ArgumentNotNull(painter, "painter");
			this.provider = provider;
			this.painter = painter;
		}
		protected override DragState GetDragState(Point p) {
			return provider.GetDragState(p);
		}
		public override void SetDragCursor(DragState e) {
			Cursor.Current = provider.GetDragCursor(e);
		}
		protected override void RaisePaint(PaintEventArgs e) {
			GraphicsCache cache = new GraphicsCache(e.Graphics);
			painter.DrawDraggingItem(cache, e.ClipRectangle, provider.DraggingItemCaption);
		}
		protected override void OnDragDrop(Point p) {
			provider.EndDrag(p);
		}
	}
	#endregion
}
