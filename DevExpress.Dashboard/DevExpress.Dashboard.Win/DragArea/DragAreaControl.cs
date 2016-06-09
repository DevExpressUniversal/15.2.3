#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.LookAndFeel.Helpers;
using DevExpress.XtraBars;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.DragDrop;
using System.Collections.Generic;
using DevExpress.XtraEditors;
using DevExpress.DataAccess.Native;
using DevExpress.XtraEditors.Drawing;
using DevExpress.DashboardWin.ServiceModel;
using DevExpress.Data.Utils;
namespace DevExpress.DashboardWin.Native {
	[DXToolboxItem(false)]
	public class DragAreaControl : XtraUserControl, IDropDestination, IToolTipControlClient, IMouseWheelSupport {
		readonly DashboardDesignerDragManager dragManager;
		readonly DragArea area;
		readonly DragAreaDrawingContext drawingContext;
		readonly BarManager barManager;
		readonly BarAndDockingController barController;
		readonly DragItemPopupMenu popupMenu;
		readonly ToolTipController toolTipController;
		readonly DragAreaScrollableControl scrollableControl;
		DataDashboardItem dashboardItem;
		readonly Locker arrangeLocker = new Locker();
		readonly Locker selectionLocker = new Locker();
		DashboardDesigner designer;
		DragAreaSelection currentSelection;
		Point mouseDownLocation;
		internal bool EnableDataFieldDrop { get { return DashboardItem != null && IsSelectedDataSourceEnabled(); } }
		public DashboardDesigner Designer { get { return designer; } }
		public DashboardDesignerDragManager DragManager { get { return dragManager; } }
		public DragArea Area { get { return area; } }
		public DragAreaDrawingContext DrawingContext { get { return drawingContext; } }
		public DataDashboardItem DashboardItem {
			get { return dashboardItem; }
			private set {
				UnsubscribeDashboardItemEvents();
				area.UnsubscribeDashboardItemEvents();
				dashboardItem = value;
				SubscribeDashboardItemEvents();
				area.SubscribeDashboardItemEvents();
				area.UpdateSections();
			}
		}
		public bool ArrangeLocked { get { return arrangeLocker.IsLocked; } }
		bool SelectionLocked { get { return selectionLocker.IsLocked; } }
		internal DragItemPopupMenu PopupMenu { get { return popupMenu; } }
		DragItem CurrentItem { get { return currentSelection.Item; } }
		IServiceProvider ServiceProvider { get { return designer; } }
		public DragAreaControl(DashboardDesigner designer, DragAreaScrollableControl scrollableControl) {
			Guard.ArgumentNotNull(designer, "designer");
			Guard.ArgumentNotNull(scrollableControl, "scrollableControl");
			this.designer = designer;
			this.scrollableControl = scrollableControl;
			SubscribeServiceEvents();
			dragManager = designer.DragManager;
			dragManager.RegisterDropTarget(this);
			area = new DragArea(designer, scrollableControl, this);
			LookAndFeel.StyleChanged += LookAndFeelStyleChanged;
			drawingContext = new DragAreaDrawingContext(LookAndFeel);
			barManager = new BarManager();
			barManager.Form = this;
			barController = new BarAndDockingController();
			barController.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			barManager.Controller = barController;
			barManager.AllowCustomization = false;
			popupMenu = new DragItemPopupMenu(this, barManager);
			toolTipController = new ToolTipController();
			toolTipController.AddClientControl(this);
			DoubleBuffered = true;
			Size = new Size(10, 10);
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
			SubscibeDragAreaScrollableControlEvents();
			SubscibePopupMenuControlEvents();
		}
		void OnDataFieldRenamed(object sernder, EventArgs e) {
			Area.Invalidate();
		}
		void SubscribeServiceEvents() {
			IDashboardDesignerSelectionService selectionService = ServiceProvider.RequestServiceStrictly<IDashboardDesignerSelectionService>();
			selectionService.DashboardItemSelected += OnDashboardItemSelected;
			IDataFieldChangeService changeService = ServiceProvider.RequestServiceStrictly<IDataFieldChangeService>();
			changeService.DataFieldRenamed += OnDataFieldRenamed;
		}
		void UnsubscribeServiceEvents() {
			if (ServiceProvider != null) {
				IDashboardDesignerSelectionService selectionService = ServiceProvider.RequestService<IDashboardDesignerSelectionService>();
				if (selectionService != null)
					selectionService.DashboardItemSelected -= OnDashboardItemSelected;
				IDataFieldChangeService changeService = ServiceProvider.RequestService<IDataFieldChangeService>();
				if (changeService != null )
					changeService.DataFieldRenamed -= OnDataFieldRenamed;
			}
		}
		void SubscibePopupMenuControlEvents() {
			popupMenu.CloseUp += OnPopupMenuCloseUp;
			popupMenu.BeforeCommandExecute += OnPopupMenuBeforeCommandExecute;
			popupMenu.AfterCommandExecute += OnPopupMenuAfterCommandExecute;
		}
		void UnubscibePopupMenuControlEvents() {
			if(popupMenu != null) {
				popupMenu.CloseUp -= OnPopupMenuCloseUp;
				popupMenu.BeforeCommandExecute -= OnPopupMenuBeforeCommandExecute;
				popupMenu.AfterCommandExecute -= OnPopupMenuAfterCommandExecute;
			}
		}
		void SubscibeDragAreaScrollableControlEvents() {
			scrollableControl.ClientSizeChanged += OnScrollableControlClientSizeChanged;
		}
		void UnsubscibeDragAreaScrollableControlEvents() {
			if (scrollableControl != null)
				scrollableControl.ClientSizeChanged -= OnScrollableControlClientSizeChanged;
		}
		void OnPopupMenuCloseUp(object sender, EventArgs e) {
			UnlockSelection();
		}
		void OnScrollableControlClientSizeChanged(object sender, EventArgs e) {
			drawingContext.UpdateSectionWidth(scrollableControl.ClientSize.Width);
			Area.Arrange();
			scrollableControl.PerformLayout();
		}
		void OnPopupMenuBeforeCommandExecute(object sender, EventArgs e) {
			DragGroup group = CurrentItem.Group;
			LockSelection();
			group.Highlight();
			CurrentItem.Select();
			group.Section.Area.Invalidate();
		}
		void OnPopupMenuAfterCommandExecute(object sender, EventArgs e) {
			DragGroup group = CurrentItem.Group;
			CurrentItem.ResetState();
			group.Cleanup();
			UnlockSelection();
			group.Section.Area.Invalidate();
		}
		public void BeginUpdate() {
			arrangeLocker.Lock();
		}
		public void EndUpdate() {
			arrangeLocker.Unlock();
			area.Arrange();
			if(SelectionLocked)
				SetSelection();
			Invalidate();
		}
		void SetSelection() {
			if(currentSelection != null && currentSelection.Type == DragAreaSelectionType.OptionsButton) {
				DragGroup oldGroup = currentSelection.Group;
				DragSection section = Area.Sections[oldGroup.SectionIndex];
				DragGroup newGroup = section.GetActualGroup(currentSelection.SelectedGroupIndex);
				newGroup.Select();
			}
		}
		public void LockSelection() {
			selectionLocker.Lock();
		}
		public void UnlockSelection() {
			selectionLocker.Unlock();
		}
		bool IsSelectedDataSourceEnabled() {
			IDataSourceSelectionService service = ServiceProvider.RequestServiceStrictly<IDataSourceSelectionService>();
			if (DashboardItem is RangeFilterDashboardItem && service.SelectedDataSourceInfo.GetDataSourceSafe() != null && !service.SelectedDataSourceInfo.GetDataSourceSafe().GetIsRangeFilterSupported())
				return false;
			return DashboardItem.DataSource == null || DataSourceInfoComparer.Comparer.Equals(service.SelectedDataSourceInfo,new DataSourceInfo(DashboardItem.DataSource, DashboardItem.DataMember));
		}
		void OnDashboardItemSelected(object sender, DashboardItemSelectedEventArgs e) {
			DashboardItem = e.SelectedDashboardItem as DataDashboardItem;
		}
		public bool ShowPopupMenu(DragItem dragItem, Point location) {
			LockSelection();
			return popupMenu.Show(dragItem, PointToScreen(location));
		}
		public void ForceUpdateSelection() {
			UpdateSelection(PointToClient(Cursor.Position), false);
		}
		public void UpdateDataSource() {
			if (dashboardItem != null)
				area.SetDataSource(dashboardItem.DataSourceSchema);
		}
		IDragObject CreateDragObject() {
			if(currentSelection.Type == DragAreaSelectionType.Group)
				return new GroupDragObject(currentSelection.Group);
			if(CurrentItem.DataItemsGroupIndex != Dimension.DefaultGroupIndex)
				return new OlapHierarchyDragObject(CurrentItem);
			IDataSourceSchema dataSource = Area.DashboardItem.DataSourceSchema;				
			OlapDataField olapDataField = dataSource != null ? dataSource.GetField(CurrentItem.DataItem.DataMember) as OlapDataField : null;
			if(olapDataField != null) {
				switch(olapDataField.NodeType) {
					case DataNodeType.OlapDimension:
						return new OlapDimensionDragObject(CurrentItem);
					case DataNodeType.OlapKpi:
					case DataNodeType.OlapMeasure:
						return new OlapMeasureDragObject(CurrentItem);
				}
			}
			return new DataItemDragObject(CurrentItem);
		}
		bool AcceptableDragObject(IDragObject dragObject) {
			DataFieldDragObject dataFieldDragObject = dragObject as DataFieldDragObject;
			if(dataFieldDragObject != null) {
				OlapDataField olapDataField = dataFieldDragObject.DataField as OlapDataField;
				if(olapDataField != null && olapDataField.NodeType != DataNodeType.OlapMeasure && olapDataField.NodeType != DataNodeType.OlapKpi) {
					RangeFilterDashboardItem rangeFilter = area.DashboardItem as RangeFilterDashboardItem;
					if(rangeFilter != null)
						return false;
					return !area.DashboardItem.GetDataMembers().Contains(olapDataField.DataMember);
				}
				CalculatedDataField calculatedDataField = dataFieldDragObject.DataField as CalculatedDataField;
				IDashboardParameterService service = ServiceProvider.GetService<IDashboardParameterService>();
				if (calculatedDataField != null && calculatedDataField.GetIsCorrupted(service))
					return false;
			}
			return true;
		}
		protected override void Dispose(bool disposing) {			
			if (disposing) {
				UnsubscribeDashboardItemEvents();
				UnsubscibeDragAreaScrollableControlEvents();
				UnubscibePopupMenuControlEvents();
				area.Dispose();
				drawingContext.Dispose();
				barManager.Dispose();
				popupMenu.Dispose();
				barController.Dispose();
				toolTipController.Dispose();
				UnsubscribeServiceEvents();
				if(designer != null)
					designer = null;
			}
			base.Dispose(disposing);
		}
		protected override void OnPaint(PaintEventArgs e) {
			if (!ArrangeLocked) {
				base.OnPaint(e);
				area.Paint(e);
			}
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			bool isLeftButtonDown = e.Button == MouseButtons.Left;
			if(!dragManager.DragWindow.IsDragging && e.Location != mouseDownLocation)
				if(isLeftButtonDown && currentSelection != null && (currentSelection.Type == DragAreaSelectionType.NonEmptyDragItem || currentSelection.Type == DragAreaSelectionType.Group)) {
					IDragObject dragObject = CreateDragObject();
					Bitmap bitmap = GetDragBitmap(dragObject);
					Rectangle selectedObjectBounds = currentSelection.Type == DragAreaSelectionType.NonEmptyDragItem ? currentSelection.Item.Bounds : currentSelection.Group.Bounds;
					Size offset = GetOffset(selectedObjectBounds, e.Location);
					dragManager.StartDrag(dragObject, bitmap, PointToScreen(e.Location), offset);
				}
				else
					UpdateSelection(e.Location, isLeftButtonDown);
		}
		Size GetOffset(Rectangle selectedObjectBounds, Point location) {
			int x = location.X - selectedObjectBounds.X;
			int y = location.Y - selectedObjectBounds.Y;
			return new Size(x, y);
		}
		Bitmap GetDragBitmap(IDragObject dragObject) {
			if(currentSelection.Type == DragAreaSelectionType.Group)
				return drawingContext.GetDragGroupBitmap(currentSelection.Group);
			return drawingContext.GetDataItemBitmap(DashboardItem.DataSourceSchema, dragObject, currentSelection.Item.Bounds.Width, currentSelection.Item.Bounds.Height);
		}
		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			if(!dragManager.DragWindow.IsDragging && !SelectionLocked) {
				ResetCurrentSelection();
				Invalidate();
			}
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			mouseDownLocation = e.Location;
			if(currentSelection == null)
				return;
			switch (e.Button) {
				case MouseButtons.Left:
					if(currentSelection.Type == DragAreaSelectionType.NonEmptyDragItem) {
						CurrentItem.Select();
						if(currentSelection.ElementWithButton != null)
							SetButtonStateToCurrentElementWithButton(DragAreaButtonState.Invisible);
						Invalidate();
					}
					else if(currentSelection.Type == DragAreaSelectionType.Group) {
						currentSelection.Group.Select();
						Invalidate();
					}
					else if(currentSelection.ElementWithButton != null) {
						SetButtonStateToCurrentElementWithButton(DragAreaButtonState.Selected);
						Invalidate();
					}
					break;
				case MouseButtons.Right:
					if (CurrentItem != null)
						ShowPopupMenu(CurrentItem, mouseDownLocation);
					break;
			}
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			mouseDownLocation = Point.Empty;
			DragAreaSelection selection = area.GetSelection(e.Location);
			if(currentSelection == null || !currentSelection.SameSelection(selection))
				return;
			if(e.Button == MouseButtons.Left)
				if(currentSelection.Type == DragAreaSelectionType.NonEmptyDragItem) {
					Dimension dimension = CurrentItem.DataItems.Count == 1 ? CurrentItem.DataItem as Dimension : null;
					if(dimension != null && DashboardItem.IsSortingEnabled(dimension)) {
						SortOrderHistoryItem historyItem = new SortOrderHistoryItem(dashboardItem, dimension, dimension.GetNextActualSortOrder());
						designer.History.RedoAndAdd(historyItem);
					}
					CurrentItem.ResetState();
					Invalidate(CurrentItem.Bounds);
				}
				else if(currentSelection.ElementWithButton != null) {
					currentSelection.ElementWithButton.ExecuteButtonClick(this);
				}
		}
		protected override void OnLeave(EventArgs e) {
			base.OnLeave(e);
			if(!dragManager.DragWindow.IsDragging && !SelectionLocked) {
				ResetCurrentSelection();
				Invalidate();
			}
		}
		void LookAndFeelStyleChanged(object sender, EventArgs e) {
			drawingContext.Update(LookAndFeel);
			area.Arrange();
		}
		void OnDashboardItemChanged(object sender, ChangedEventArgs e) {			
			switch (e.Reason) {
				case ChangeReason.View:				
					Invalidate();
					break;
				case ChangeReason.Coloring:
				case ChangeReason.ClientData:
					Area.Arrange();
					break;
			}
		}
		void UnsubscribeDashboardItemEvents() {
			if (dashboardItem != null)
				((IChangeService)dashboardItem).Changed -= OnDashboardItemChanged;
		}
		void SubscribeDashboardItemEvents() {
			if (dashboardItem != null)
				((IChangeService)dashboardItem).Changed += OnDashboardItemChanged;
		}
		void SetButtonStateToCurrentElementWithButton(DragAreaButtonState state) {
			if(currentSelection.ElementWithButton != null)
				currentSelection.ElementWithButton.SetButtonState(state);
		}
		internal void UpdateSelection(Point location, bool isLeftButtonDown) {
			if(isLeftButtonDown || SelectionLocked)
				return;
			DragAreaSelection newSelection = area.GetSelection(location);
			if (newSelection.SameSelection(currentSelection))
				return;
			ResetCurrentSelection();
			currentSelection = newSelection;
			UpdateCurrentSelection(isLeftButtonDown);
			UpdateCursor();
			Invalidate();
		}
		void UpdateCursor() {
			Cursor = currentSelection.Type == DragAreaSelectionType.Group ? Cursors.SizeAll : Cursors.Default;
		}
		void UpdateCurrentSelection(bool isLeftButtonDown) {
			switch(currentSelection.Type) {
				case DragAreaSelectionType.Group:
					currentSelection.Group.Highlight();
					break;
				case DragAreaSelectionType.NonEmptyDragItem: {
						if(currentSelection.Group.Section.AllowDragGroups)
							currentSelection.Group.Highlight();
						currentSelection.Item.Highlight();
						SetButtonStateToCurrentElementWithButton(DragAreaButtonState.Normal);
					}
					break;
				case DragAreaSelectionType.DragItemPopupButton: {
						if(currentSelection.Group.Section.AllowDragGroups && currentSelection.Group.DataItemsCount != 0)
							currentSelection.Group.Highlight();
						SetButtonStateToCurrentElementWithButton(isLeftButtonDown ? DragAreaButtonState.Selected : DragAreaButtonState.Hot);
						currentSelection.Item.Highlight();
					}
					break;
				case DragAreaSelectionType.OptionsButton: {
						if(currentSelection.Group.Section.AllowDragGroups && currentSelection.Group.DataItemsCount != 0)
							currentSelection.Group.Highlight();
						SetButtonStateToCurrentElementWithButton(isLeftButtonDown ? DragAreaButtonState.Selected : DragAreaButtonState.Hot);
					}
					break;
				case DragAreaSelectionType.ImageButton:
					currentSelection.ImageButton.SetOptionsButtonState(isLeftButtonDown ? DragAreaButtonState.Selected : DragAreaButtonState.Hot);
					break;
			}
		}
		void ResetCurrentSelection() {
			if(currentSelection == null)
				return;
			if(currentSelection.Group != null)
				currentSelection.Group.ResetState();
			if(currentSelection.Item != null)
				currentSelection.Item.ResetState();
			DragAreaButtonState elementWithButtonState = currentSelection.Type == DragAreaSelectionType.NonEmptyDragItem ||
														 currentSelection.Type == DragAreaSelectionType.DragItemPopupButton ? DragAreaButtonState.Invisible : DragAreaButtonState.Normal;
			SetButtonStateToCurrentElementWithButton(elementWithButtonState);
		}
		#region IToolTipControlClient
		bool IToolTipControlClient.ShowToolTips { get { return true; } }
		ToolTipControlInfo IToolTipControlClient.GetObjectInfo(Point point) {
			DragAreaSelection selection = area.GetSelection(point);
			if (selection.ImageButton != null) 
				return new ToolTipControlInfo(selection.ImageButton, selection.ImageButton.Tooltip,String.Empty, false, ToolTipIconType.None, DefaultBoolean.True);			
			DragItem item = selection.Item;
			if (item == null)
				return null;
			IList<DataItem> dataItems = item.DataItems;
			if (dataItems.Count == 0)
				return null;
			ToolTipControlInfo res = new ToolTipControlInfo(item, String.Empty, String.Empty, false, ToolTipIconType.None, DefaultBoolean.True);
			string titleText = String.Format("<b>{0}:</b>", item.Caption);
			if(dataItems.Count == 1) {
				res.Text = String.Format("{0} {1}", titleText, dataItems[0].DisplayName);
			}
			else {
				SuperToolTip tip = new SuperToolTip();
				ToolTipTitleItem titleItem = new ToolTipTitleItem();
				titleItem.AllowHtmlText = DefaultBoolean.True;
				titleItem.Text = titleText;
				tip.Items.Add(titleItem);
				foreach(DataItem dataItem in dataItems) {
					ToolTipItem tooltipItem = new ToolTipItem();
					tooltipItem.Text = dataItem.DisplayName;
					tip.Items.Add(tooltipItem);
				}
				res.SuperTip = tip;
			}
			return res;
		}
		#endregion
		Rectangle IDropDestination.ScreenBounds { get { return new Rectangle(PointToScreen(ClientRectangle.Location), ClientRectangle.Size); } }
		IDropAction IDropDestination.GetDropAction(IDragObject dragObject, Point screenPt) {
			if(dragObject.IsDataField && !EnableDataFieldDrop)
				return null;
			DragAreaScrollableControl scrollableControl = Parent as DragAreaScrollableControl;
			if(scrollableControl != null)
				scrollableControl.StartAutoScroll(screenPt);
			return AcceptableDragObject(dragObject) ? area.GetDropAction(PointToClient(screenPt), dragObject) : null;
		}
		#region IMouseWheelSupport Members
		protected sealed override void OnMouseWheel(MouseEventArgs e) {
			if (XtraForm.ProcessSmartMouseWheel(this, e))
				return;
			OnMouseWheelCore(e);
		}
		void IMouseWheelSupport.OnMouseWheel(MouseEventArgs e) {
			OnMouseWheelCore(e);
		}
		void OnMouseWheelCore(MouseEventArgs e) {
			if (!ControlHelper.IsHMouseWheel(e)) {
				scrollableControl.DoMouseWheel(e);
			}
		}
		#endregion
	}
}
