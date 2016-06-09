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
using DevExpress.Xpf.Core;
using DevExpress.XtraSpreadsheet.Forms;
using DevExpress.XtraSpreadsheet.Internal;
using System;
using System.Windows.Controls;
using System.Windows;
using DrawingPoint = System.Drawing.Point;
using DrawingSize = System.Drawing.Size;
using DevExpress.Xpf.Spreadsheet.Localization;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Xpf.Spreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Xpf.Editors;
using System.Windows.Threading;
namespace DevExpress.Xpf.Spreadsheet.Forms {
	public partial class FieldsPanelPivotTableControl : DXWindow, IPivotTableFieldsPanel {
		#region Fields
		SpreadsheetControl control;
		FieldListPanelPivotTableViewModel viewModel;
		PivotFieldListPanelBoxType initialBoxType;
		ListBoxEditItem draggingItem;
		int initialPosition;
		int selectedItemsCount;
		#endregion
		public FieldsPanelPivotTableControl(SpreadsheetControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
			this.ShowActivated = false;
			InitializeComponent();
			ResetDragProperties();
			SubscribeDragEvents();
		}
		#region Closing
		EventHandler onClosing;
		event EventHandler IPivotTableFieldsPanel.Closing { add { onClosing += value; } remove { onClosing -= value; } }
		void RaiseClosing() {
			if (onClosing != null)
				onClosing(this, EventArgs.Empty);
		}
		#endregion
		#region Properties
		DrawingPoint IPivotTableFieldsPanel.Location {
			get { return new DrawingPoint((int)Left, (int)Top); }
			set { SetLocation(value); }
		}
		DrawingSize IPivotTableFieldsPanel.Size {
			get { return new DrawingSize((int)Width, (int)Height); }
			set {
				Width = value.Width;
				Height = value.Height;
				SizeToContent = System.Windows.SizeToContent.Manual;
			}
		}
		PivotFieldViewInfo DraggingInfo { get { return (PivotFieldViewInfo)draggingItem.Content; } }
		#endregion
		void IPivotTableFieldsPanel.SetStartPosition(XtraSpreadsheet.SpreadsheetPivotTableFieldListStartPosition position, DrawingPoint location) {
			if (position == XtraSpreadsheet.SpreadsheetPivotTableFieldListStartPosition.CenterScreen)
				WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
			else if (position == XtraSpreadsheet.SpreadsheetPivotTableFieldListStartPosition.CenterSpreadsheetControl)
				WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
			else if (position == XtraSpreadsheet.SpreadsheetPivotTableFieldListStartPosition.ManualScreen)
				SetLocation(location);
			else {
				Point newLocation = control.PointToScreen(new Point());
				newLocation.Offset(location.X, location.Y);
				SetLocation(new DrawingPoint((int)newLocation.X, (int)newLocation.Y));
			}
		}
		void SetLocation(DrawingPoint location) {
			Left = location.X;
			Top = location.Y;
			WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
		}
		#region Subscribe / Unsubscribe
		void SubscribeEvents() {
			viewModel.DataChanged += OnDataChanged;
		}
		void UnsubscribeEvents() {
			if (viewModel != null)
				viewModel.DataChanged -= OnDataChanged;
		}
		void SubscribeDragEvents() {
			lstFields.PreviewMouseDown += BeginDrag;
			lstFields.MouseUp += EndDrag;
			lstFilters.PreviewMouseDown += BeginDrag;
			lstFilters.MouseUp += EndDrag;
			lstColumns.PreviewMouseDown += BeginDrag;
			lstColumns.MouseUp += EndDrag;
			lstRows.PreviewMouseDown += BeginDrag;
			lstRows.MouseUp += EndDrag;
			lstValues.PreviewMouseDown += BeginDrag;
			lstValues.MouseUp += EndDrag;
		}
		void UnsubscribeDragEvents() {
			lstFields.PreviewMouseDown -= BeginDrag;
			lstFields.MouseUp -= EndDrag;
			lstFilters.PreviewMouseDown -= BeginDrag;
			lstFilters.MouseUp -= EndDrag;
			lstColumns.PreviewMouseDown -= BeginDrag;
			lstColumns.MouseUp -= EndDrag;
			lstRows.PreviewMouseDown -= BeginDrag;
			lstRows.MouseUp -= EndDrag;
			lstValues.PreviewMouseDown -= BeginDrag;
			lstValues.MouseUp -= EndDrag;
		}
		#endregion
		#region PopulateBoxes
		void OnDataChanged(object sender, EventArgs e) {
			Dispatcher.BeginInvoke(new Action(() => PopulateBoxes()), DispatcherPriority.Render);
		}
		void PopulateBoxes() {
			viewModel.BeginCalculateFieldCaptions();
			PopulateBox(lstFilters, viewModel.GetFilterInfos());
			PopulateBox(lstColumns, viewModel.GetColumnInfos());
			PopulateBox(lstRows, viewModel.GetRowInfos());
			PopulateBox(lstValues, viewModel.GetValueInfos());
			PopulateBox(lstFields, viewModel.EndCalculateFieldCaptions());
		}
		void PopulateBox(ListBoxEdit box, List<PivotFieldViewInfo> infos) {
			box.SelectionMode = SelectionMode.Multiple;
			box.Items.Clear();
			foreach (PivotFieldViewInfo info in infos) {
				ListBoxEditItem item = new ListBoxEditItem();
				item.Content = info;
				box.Items.Add(item);
				if (info.IsSelected)
					box.SelectedItems.Add(item);
			}
		}
		#endregion
		void IPivotTableFieldsPanel.Show(FieldListPanelPivotTableViewModel viewModel) {
				ChangeViewModel(viewModel);
				string title = XpfSpreadsheetLocalizer.GetString(SpreadsheetControlStringId.Caption_PivotTableFieldsFormTitle);
				control.ShowDXWindow(this, title, this.SizeToContent, this.WindowStartupLocation, ResizeMode.CanResize);
		}
		void IPivotTableFieldsPanel.ChangeViewModel(FieldListPanelPivotTableViewModel viewModel) {
			ChangeViewModel(viewModel);
		}
		void ChangeViewModel(FieldListPanelPivotTableViewModel viewModel) {
			if (viewModel.Equals(this.viewModel))
				return;
			DisposeViewModel();
			this.viewModel = viewModel;
			DataContext = viewModel;
			PopulateBoxes();
			SubscribeEvents();
		}
		void IPivotTableFieldsPanel.Hide() {
			this.Close();
		}
		void DisposeViewModel() {
			UnsubscribeEvents();
			if (viewModel != null) {
				viewModel.Dispose();
				viewModel = null;
			}
		}
		protected override void OnClosed(EventArgs e) {
			DisposeViewModel();
			UnsubscribeDragEvents();
			this.control = null;
			RaiseClosing();
			base.OnClosed(e);
		}
		#region Drag & Drop
		void BeginDrag(object sender, MouseButtonEventArgs e) {
			ListBoxEdit dragSource = (ListBoxEdit)sender;
			ListBoxEditItem item = GetItem(dragSource, e.GetPosition(dragSource));
			if (item != null) {
				draggingItem = item;
				initialPosition = dragSource.Items.IndexOf(item);
				initialBoxType = GetBoxType(dragSource);
				selectedItemsCount = dragSource.SelectedItems.Count;
				dragSource.PreviewMouseMove += OnPreviewMouseMove;
			}
		}
		void EndDrag(object sender, MouseButtonEventArgs e) {
			if (draggingItem == null)
				return;
			ListBoxEdit dragSource = (ListBoxEdit)sender;
			CorrectSelection(dragSource);
			Mouse.OverrideCursor = null;
			ListBoxEdit dragTarget = GetBoxByPoint(e.GetPosition(this));
			PivotFieldListPanelBoxType targetBoxType = GetBoxType(dragTarget);
			Point point = e.GetPosition(dragTarget);
			PivotFieldListPanelOperationType operation = GetOperation(targetBoxType, point);
			if (operation != PivotFieldListPanelOperationType.NotAllowed)
				MovePivotField(dragTarget, targetBoxType, point);
			ResetDragProperties();
			dragSource.PreviewMouseMove -= OnPreviewMouseMove;
		}
		void OnPreviewMouseMove(object sender, MouseEventArgs e) {
			if (draggingItem == null)
				return;
			ListBoxEdit dragTarget = GetBoxByPoint(e.GetPosition(this));
			PivotFieldListPanelOperationType operation = GetOperation(GetBoxType(dragTarget), e.GetPosition(dragTarget));
			Mouse.OverrideCursor = GetCursor(operation);
		}
		void MovePivotField(ListBoxEdit dragTarget, PivotFieldListPanelBoxType targetBoxType, Point point) {
			PivotTableAxis from = viewModel.ConvertBoxTypeToAxis(initialBoxType);
			PivotTableAxis to = viewModel.ConvertBoxTypeToAxis(targetBoxType);
			int targetPosition = GetTargetPosition(dragTarget, point);
			viewModel.MovePivotField(DraggingInfo.FieldIndex, from, initialPosition, to, targetPosition);
		}
		void CorrectSelection(ListBoxEdit box) {
			int count = box.SelectedItems.Count;
			if (count < selectedItemsCount) {
				box.SelectedItems.Add(draggingItem);
				return;
			}
			if (count == 1)
				box.SelectedItem = null;
			else
				box.SelectedItems.RemoveAt(count - 1);
		}
		void ResetDragProperties() {
			initialBoxType = PivotFieldListPanelBoxType.None;
			initialPosition = -1;
			draggingItem = null;
		}
		#endregion
		#region Get methods
		Cursor GetCursor(PivotFieldListPanelOperationType operation) {
			SpreadsheetCursor spreadsheetCursor;
			if (operation == PivotFieldListPanelOperationType.Move)
				spreadsheetCursor = SpreadsheetCursors.DragItemMove;
			else if (operation == PivotFieldListPanelOperationType.Remove)
				spreadsheetCursor = SpreadsheetCursors.DragItemRemove;
			else
				spreadsheetCursor = SpreadsheetCursors.DragItemNone;
			return CursorsProvider.GetCursor(spreadsheetCursor.Cursor);
		}
		PivotFieldListPanelBoxType GetBoxType(ListBoxEdit box) {
			if (Object.ReferenceEquals(box, lstFields))
				return PivotFieldListPanelBoxType.Fields;
			else if (Object.ReferenceEquals(box, lstFilters))
				return PivotFieldListPanelBoxType.Filters;
			else if (Object.ReferenceEquals(box, lstColumns))
				return PivotFieldListPanelBoxType.Columns;
			else if (Object.ReferenceEquals(box, lstRows))
				return PivotFieldListPanelBoxType.Rows;
			else if (Object.ReferenceEquals(box, lstValues))
				return PivotFieldListPanelBoxType.Values;
			return PivotFieldListPanelBoxType.None;
		}
		ListBoxEdit GetBoxByPoint(Point point) {
			if (ContainsPoint(lstFields, point))
				return lstFields;
			else if (ContainsPoint(lstFilters, point))
				return lstFilters;
			else if (ContainsPoint(lstColumns, point))
				return lstColumns;
			else if (ContainsPoint(lstRows, point))
				return lstRows;
			else if (ContainsPoint(lstValues, point))
				return lstValues;
			return null;
		}
		bool ContainsPoint(FrameworkElement element, Point point) {
			return element.GetBounds(this).Contains(point);
		}
		ListBoxEditItem GetItem(ListBoxEdit box, Point point) {
			IInputElement hitTestResult = box.InputHitTest(point);
			Border border = hitTestResult as Border;
			if (border != null)
				return (ListBoxEditItem)border.TemplatedParent;
			TextBlock textBlock = hitTestResult as TextBlock;
			if (textBlock != null) {
				foreach (ListBoxEditItem item in box.Items) {
					PivotFieldViewInfo data = (PivotFieldViewInfo)item.Content;
					if (data.Caption == textBlock.Text)
						return item;
				}
			}
			return null;
		}
		PivotFieldListPanelOperationType GetOperation(PivotFieldListPanelBoxType targetBoxType, Point point) {
			return viewModel.GetOperation(DraggingInfo.FieldIndex, initialBoxType, targetBoxType, !ContainsPoint(this, point));
		}
		int GetTargetPosition(ListBoxEdit dragTarget, Point point) {
			if (dragTarget != null) {
				ListItemCollection items = dragTarget.Items;
				object targetData = GetItem(dragTarget, point);
				return targetData != null ? items.IndexOf(targetData) : items.Count;
			}
			return 0;
		}
		#endregion
		void btnUpdate_Click(object sender, RoutedEventArgs e) {
			viewModel.Update();
		}
	}
}
