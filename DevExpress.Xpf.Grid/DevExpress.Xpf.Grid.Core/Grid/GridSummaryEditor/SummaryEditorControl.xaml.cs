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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DevExpress.Xpf.Core;
using DevExpress.Data.Summary;
using DevExpress.Data;
using DevExpress.Xpf.Editors;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Themes;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Utils.Themes;
using DevExpress.Data.Helpers;
namespace DevExpress.Xpf.Grid {
	public partial class SummaryEditorControl : UserControl, IDialogContent {
		DataViewBase view;
		public SummaryEditorType SummaryEditorType { get; set; }
		GridSummaryItemsEditorController summaryController;
		SummaryEditorOrderListHelper orderListHelper;
		public GridSummaryItemsEditorController Controller { get { return summaryController; } }
		protected SummaryEditorUIItem ActiveUIItem { get { return itemList.SelectedItem as SummaryEditorUIItem; } }
		protected SummaryEditorOrderUIItem ActiveOrderUIItem { get { return GetActiveOrderUIItem(); } }
		public SummaryEditorControl(GridSummaryItemsEditorController controller, SummaryEditorType summaryEditorType) {
			this.summaryController = controller;
			this.SummaryEditorType = summaryEditorType;
			orderListHelper = new SummaryEditorOrderListHelper(Controller);
			SetView();
			InitializeComponent();
			itemList.ItemsSource = Controller.UIItems;
			InitializeUIElements((Controller.Owner as GridSummaryHelper).view);
			UpdateOrderItems();
			UpdateButtonsState();
			itemList.SelectedIndex = 0;
			DataContext = this;
#if DEBUGTEST
			Loaded += new RoutedEventHandler(SummaryEditorControl_Loaded);
#endif
		}
#if DEBUGTEST
		public static event EventHandler SummaryEditorControlLoaded;
		void SummaryEditorControl_Loaded(object sender, RoutedEventArgs e) {
			Loaded -= new RoutedEventHandler(SummaryEditorControl_Loaded);
			if(SummaryEditorControlLoaded != null)
				SummaryEditorControlLoaded(this, EventArgs.Empty);
		}
#endif
		void InitializeUIElements(DataViewBase ownerView) {
			tabItems.Header = ownerView.GetLocalizedString(GridControlStringId.SummaryEditorFormItemsTabCaption);
			tabOrder.Header = SummaryEditorType != SummaryEditorType.TotalSummaryPanel ?
				ownerView.GetLocalizedString(GridControlStringId.SummaryEditorFormOrderTabCaption) :
				ownerView.GetLocalizedString(GridControlStringId.SummaryEditorFormOrderAndAlignmentTabCaption);
			tbLeftSideCaption.Text = ownerView.GetLocalizedString(GridControlStringId.SummaryEditorFormOrderLeftSide);
			tbRightSideCaption.Text = ownerView.GetLocalizedString(GridControlStringId.SummaryEditorFormOrderRightSide);
			InitializeSummaryCheckEdit(maxCheck, SummaryItemType.Max);
			InitializeSummaryCheckEdit(minCheck, SummaryItemType.Min);
			InitializeSummaryCheckEdit(averageCheck, SummaryItemType.Average);
			InitializeSummaryCheckEdit(sumCheck, SummaryItemType.Sum);
			InitializeSummaryCheckEdit(countCheck, SummaryItemType.Count);
			InitializeCountCheckEdit();
			InitializeOrderLists();
		}
		void InitializeOrderLists() {
			if(SummaryEditorType != SummaryEditorType.TotalSummaryPanel) {
				gridTabOrder.ColumnDefinitions.Remove(gridTabOrder.ColumnDefinitions[2]);
				return;
			}
			rightButton.Visibility = Visibility.Visible;
			leftButton.Visibility = Visibility.Visible;
			tbLeftSideCaption.Visibility = Visibility.Visible;
			gridRightSide.Visibility = Visibility.Visible;
		}
		void InitializeCountCheckEdit() {
			if(!IsGlobalCountCheckEnabled())
				return;
			InitializeGlobalCountSummaryCheckEdit();
			globalCountCheck.IsChecked = Controller.HasFixedCountSummary();
			countCheck.Visibility = Visibility.Collapsed;
			globalCountCheck.Visibility = Visibility.Visible;
		}
		bool IsGlobalCountCheckEnabled() {
			return IsOneCountCase();
		}
		bool IsOneCountCase() {
			switch(SummaryEditorType) {
				case SummaryEditorType.TotalSummaryPanel: return true;
				case SummaryEditorType.GroupSummary:
					IGroupSummaryDisplayMode viewWithDisplayMode = view as IGroupSummaryDisplayMode;
					if(viewWithDisplayMode == null)
						return false;
					if(viewWithDisplayMode.GroupSummaryDisplayMode == GroupSummaryDisplayMode.AlignByColumns)
						return false;
					return true;
				default: return false;
			}
		}
		void SetView() {
			GridSummaryHelper helper = Controller.Owner as GridSummaryHelper;
			view = helper.view;
		}
		public DataViewBase View {
			get { return view; }
		}
		void InitializeSummaryCheckEdit(CheckEdit edit, SummaryItemType type) {
			edit.Content = GridSummaryItemsEditorController.GetNameBySummaryType(type);
			edit.Tag = type;
		}
		void InitializeGlobalCountSummaryCheckEdit() {
			globalCountCheck.Content = GridSummaryItemsEditorController.GetGlobalCountSummaryName();
			globalCountCheck.Tag = SummaryItemType.Count;
		}
		bool GetSummaryTypeCheckBoxState(SummaryItemType summaryType) {
			return ActiveUIItem[summaryType];
		}
		void OnSummaryTypeChanged(object sender, RoutedEventArgs e) {
			if(ActiveUIItem == null) return;
			CheckEdit edit = sender as CheckEdit;
			SummaryItemType summaryType = (SummaryItemType)edit.Tag;
			Controller[ActiveUIItem.FieldName][summaryType] = edit.IsChecked.HasValue ? edit.IsChecked.Value : false;
			UpdateOrderItems();
		}
		void OnCountSummaryTypeChanged(object sender, RoutedEventArgs e) {
			Controller.SetSummary("", SummaryItemType.Count, globalCountCheck.IsChecked.Value);
			UpdateOrderItems();
		}
		void OnItemListSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
			if(ActiveUIItem == null) return;
			foreach(UIElement control in summaryTypePanel.Children) {
				CheckEdit edit = control as CheckEdit;
				if(control == null) continue;
				SummaryItemType summaryType = (SummaryItemType)edit.Tag;
				if(summaryType == SummaryItemType.Count && SummaryEditorType == SummaryEditorType.TotalSummaryPanel)
					continue;
				edit.IsChecked = GetSummaryTypeCheckBoxState(summaryType);
				edit.IsEnabled = ActiveUIItem.CanDoSummary(summaryType);
			}
		}
		void UpdateOrderItems() {
			displayFormatControl.CurrentDisplayFormat = null;
			List<SummaryEditorOrderUIItem> orderItems = Controller.CreateOrderItems();
			if(SummaryEditorType != SummaryEditorType.TotalSummaryPanel)
				orderList.ItemsSource = orderListHelper.GetOrderListSource(orderItems);
			else {
				orderList.ItemsSource = orderListHelper.GetOrderListSource(orderItems, GridSummaryItemAlignment.Left);
				orderListRight.ItemsSource = orderListHelper.GetOrderListSource(orderItems, GridSummaryItemAlignment.Right);
			}
		}
		void OnMoveButtonClick(object sender, RoutedEventArgs e) {
			SummaryEditorOrderUIItem item = ActiveOrderUIItem;
			ListBox list = orderList.Items.Contains(item) ? orderList : orderListRight;
			int index = list.Items.IndexOf(item);
			if(item == null) return;
			Button button = sender as Button;
			if(button == upButton) {
				item.MoveUp();
				index--;
			}
			else {
				item.MoveDown();
				index++;
			}
			UpdateOrderItems();
			UpdateButtonsState();
			list.SelectedIndex = index;
			list.ScrollIntoView(list.Items[index]);
		}
		internal void OnAlignmentButtonClick(object sender, RoutedEventArgs e) {
			if(ActiveOrderUIItem == null)
				return;
			GridSummaryItemAlignment needAlignment = sender == leftButton ? GridSummaryItemAlignment.Left : GridSummaryItemAlignment.Right;
			if(orderListHelper.GetSummaryItemAlignment(ActiveOrderUIItem.Item) == needAlignment)
				return;
			orderListHelper.SetSummaryItemAlignment(ActiveOrderUIItem.Item, needAlignment);
			ListBox list = sender == leftButton ? orderListRight : orderList;
			int selectedOrderItemIndex = list.SelectedIndex;
			UpdateOrderItems();
			SetOrderlistSelectedIndex(list, selectedOrderItemIndex);
		}
		void SetOrderlistSelectedIndex(ListBox list, int index) {
			list.SelectedIndex = index < list.Items.Count ? index : list.Items.Count - 1;
		}
		void OnOrderListSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
			displayFormatControl.CurrentDisplayFormat = null;
			UpdateButtonsState();
			if(e.AddedItems.Count != 0) {
				if(sender == orderList)
					orderListRight.SelectedItem = null;
				else
					orderList.SelectedItem = null;
				ISummaryItem item = ((SummaryEditorOrderUIItem)e.AddedItems[0]).Item;
				SetCurrentDisplayFormat(item);
			}
		}
		SummaryEditorOrderUIItem GetActiveOrderUIItem() {
			if(orderList.SelectedItem != null)
				return (SummaryEditorOrderUIItem)orderList.SelectedItem;
			else
				return (SummaryEditorOrderUIItem)orderListRight.SelectedItem;
		}
		void UpdateButtonsState() {
			leftButton.IsEnabled = false;
			rightButton.IsEnabled = false;
			if(ActiveOrderUIItem == null) {
				upButton.IsEnabled = false;
				downButton.IsEnabled = false;
			}
			else {
				upButton.IsEnabled = ActiveOrderUIItem.CanUp;
				downButton.IsEnabled = ActiveOrderUIItem.CanDown;
				if(orderList.Items.Contains(ActiveOrderUIItem))
					rightButton.IsEnabled = true;
				if(orderListRight.Items.Contains(ActiveOrderUIItem))
					leftButton.IsEnabled = true;
			}
		}
		#region IDialogContent Members
		bool IDialogContent.CanCloseWithOKResult() {
			return true;
		}
		void IDialogContent.OnApply() {
			throw new NotImplementedException();
		}
		void IDialogContent.OnOk() {
			orderListHelper.ApplyAlignments();
			ApplyDisplayFormats();
			Controller.Apply();
		}
		#endregion
		#region TotalSummaryPanel
		Visibility GetAlignmentListBoxVisibility(IList addedItems) {
			if(SummaryEditorType != SummaryEditorType.TotalSummaryPanel)
				return Visibility.Collapsed;
			if(addedItems.Count == 0)
				return Visibility.Collapsed;
			IAlignmentItem alignmentItem = ((SummaryEditorOrderUIItem)addedItems[0]).Item as IAlignmentItem;
			if(alignmentItem == null)
				return Visibility.Collapsed;
			DataViewBase ownerView = (Controller.Owner as GridSummaryHelper).view;
			if(ownerView.ShowFixedTotalSummary)
				return Visibility.Visible;
			GridSummaryItemAlignment actualItemAlignment;
			if(!orderListHelper.TryGetAlignment(alignmentItem, out actualItemAlignment))
				actualItemAlignment = alignmentItem.Alignment;
			return actualItemAlignment != GridSummaryItemAlignment.Default ? Visibility.Visible : Visibility.Collapsed;
		}
		#endregion
		#region DisplayFormat
		bool IsForseShowColumnName() {
			return IsOneCountCase();
		}
		Dictionary<ISummaryItem, string> displayFormatListSource = new Dictionary<ISummaryItem, string>();
		void ApplyDisplayFormats() {
			foreach(ISummaryItem summaryItem in Controller.Items) {
				if(!displayFormatListSource.ContainsKey(summaryItem))
					continue;
				summaryItem.DisplayFormat = displayFormatListSource[summaryItem];
			}
		}
		Type GetColumnValueType(SummaryItemBase item) {
			if(item.SummaryType == SummaryItemType.Count || String.IsNullOrEmpty(item.FieldName))
				return typeof(Int32);
			return View.GetColumnType(item.FieldName, null);
		}
		void displayFormatControl_CurrentDisplayFormatChanged(object sender, EditValueChangedEventArgs e) {
			if(ActiveOrderUIItem == null)
				return;
			ISummaryItem item = ((SummaryEditorOrderUIItem)ActiveOrderUIItem).Item;
			string displayedText = (string)e.NewValue;
			if(item.DisplayFormat == displayedText) {
				if(displayFormatListSource.ContainsKey(item))
					displayFormatListSource.Remove(item);
				return;
			}
			if(displayFormatListSource.ContainsKey(item)) {
				displayFormatListSource[item] = displayedText;
				return;
			}
			displayFormatListSource.Add(item, displayedText);
		}
		void SetCurrentDisplayFormat(ISummaryItem item) {
			displayFormatControl.NullValueDisplayFormat = GetNullValueDisplayFormat(item);
			displayFormatControl.SecondParameterName = item.FieldName;
			displayFormatControl.SourceValueType = GetColumnValueType((SummaryItemBase)item);
			displayFormatControl.CurrentDisplayFormat =
				displayFormatListSource.ContainsKey(item) ? displayFormatListSource[item] : item.DisplayFormat;
		}
		internal string GetNullValueDisplayFormat(ISummaryItem item) {
			string displayFormatFromColumn = GetNullValueDisplayFormatFromColumn(item);
			string displayFormatOverride = GetDsiplayFormatEditSettigsModificator(item);
			if(String.IsNullOrEmpty(displayFormatOverride))
				return displayFormatFromColumn;
			return String.Format(String.Format(displayFormatFromColumn, "{0}", "{1}"), displayFormatOverride, "{1}");
		}
		string GetNullValueDisplayFormatFromColumn(ISummaryItem item) {
			SummaryItemBase sumItem = item as SummaryItemBase;
			if(sumItem == null)
				return String.Empty;
			switch(SummaryEditorType) {
				case SummaryEditorType.TotalSummaryPanel:
					return sumItem.GetFooterDisplayFormat(SummaryItemBase.ColumnSummaryType.Total);
				case SummaryEditorType.GroupSummary:
					return sumItem.GetGroupDisplayFormat();
				default:
					if(String.IsNullOrEmpty(sumItem.FieldName) || sumItem.ShowInColumn == sumItem.FieldName)
						return sumItem.GetFooterDisplayFormatSameColumn(SummaryItemBase.ColumnSummaryType.Group);
					else
						return sumItem.GetFooterDisplayFormat(SummaryItemBase.ColumnSummaryType.Group);
			}
		}
		string GetDsiplayFormatEditSettigsModificator(ISummaryItem item) {
			if(View == null)
			   return String.Empty;
			ColumnBase column = View.ColumnsCore[item.FieldName];
			if(column == null || column.ActualEditSettings == null)
				return String.Empty;
			return column.DisplayFormat;
		}
		#endregion
	}
	public class AlignmentSummaryEditorOrderUIItem : SummaryEditorOrderUIItem {
		SummaryEditorOrderListHelper orderHelper;
		public AlignmentSummaryEditorOrderUIItem(SummaryItemsEditorController controller, SummaryEditorOrderListHelper orderHelper, IAlignmentItem item, string caption)
			: base(controller, item, caption) {
			this.orderHelper = orderHelper;
		}
		protected override bool IsCanUp() {
			return GetPreviousIndex() >= 0;
		}
		protected override bool IsCanDown() {
			return GetNextIndex() < Controller.Items.Count;
		}
		int GetPreviousIndex() {
			GridSummaryItemAlignment actualAlignment = orderHelper.GetSummaryItemAlignment(Item).Value;
			for(int i = Index - 1; i >= 0; i--) {
				if(orderHelper.GetSummaryItemAlignment(Controller.Items[i]) == actualAlignment)
					return i;
			}
			return -1;
		}
		int GetNextIndex() {
			GridSummaryItemAlignment actualAlignment = orderHelper.GetSummaryItemAlignment(Item).Value;
			for(int i = Index + 1; i < Controller.Items.Count; i++) {
				if(orderHelper.GetSummaryItemAlignment(Controller.Items[i]) == actualAlignment)
					return i;
			}
			return Controller.Items.Count;
		}
		public override void MoveUp() {
			if(!CanUp) return;
			InterchangeItems(GetPreviousIndex());
		}
		public override void MoveDown() {
			if(!CanDown) return;
			InterchangeItems(GetNextIndex());
		}
		void InterchangeItems(int prevIndex) {
			ISummaryItem firstSummaryItem = Controller.Items[Index];
			Controller.Items[Index] = Controller.Items[prevIndex];
			Controller.Items[prevIndex] = firstSummaryItem;
		}
	}
	public enum SummaryEditorType {
		Default,
		TotalSummaryPanel,
		GroupSummary
	}
	public class SummaryEditorOrderListHelper {
		GridSummaryItemsEditorController Controller { get; set; }
		public SummaryEditorOrderListHelper(GridSummaryItemsEditorController controller) {
			Controller = controller;
		}
		Dictionary<IAlignmentItem, GridSummaryItemAlignment> alignmentListSource = new Dictionary<IAlignmentItem, GridSummaryItemAlignment>();
		public List<AlignmentSummaryEditorOrderUIItem> GetOrderListSource(List<SummaryEditorOrderUIItem> orderItems) {
			Dictionary<IAlignmentItem, GridSummaryItemAlignment> actualAlignmentListSource =
				new Dictionary<IAlignmentItem, GridSummaryItemAlignment>();
			List<AlignmentSummaryEditorOrderUIItem> orderListSource = new List<AlignmentSummaryEditorOrderUIItem>();
			foreach(SummaryEditorOrderUIItem orderItem in orderItems) {
				GridSummaryItemAlignment actualItemAlignment;
				if(!alignmentListSource.TryGetValue((IAlignmentItem)orderItem.Item, out actualItemAlignment))
					actualItemAlignment = ((IAlignmentItem)orderItem.Item).Alignment;
				orderListSource.Add(new AlignmentSummaryEditorOrderUIItem(Controller, this, (IAlignmentItem)orderItem.Item, orderItem.Caption));
				actualAlignmentListSource.Add((IAlignmentItem)orderItem.Item, actualItemAlignment);
			}
			alignmentListSource = actualAlignmentListSource;
			return orderListSource;
		}
		public List<AlignmentSummaryEditorOrderUIItem> GetOrderListSource(List<SummaryEditorOrderUIItem> orderItems, GridSummaryItemAlignment alignment) {
			List<AlignmentSummaryEditorOrderUIItem> orderList = GetOrderListSource(orderItems);
			return orderList.Where(item => GetSummaryItemAlignment(item.Item) == alignment).ToList();
		}
		public void ApplyAlignments() {
			foreach(ISummaryItem summaryItem in Controller.Items) {
				IAlignmentItem alignmentItem = summaryItem as IAlignmentItem;
				if(alignmentItem == null)
					continue;
				if(!alignmentListSource.ContainsKey(alignmentItem))
					continue;
				alignmentItem.Alignment = alignmentListSource[alignmentItem];
			}
		}
		public void SetSummaryItemAlignment(ISummaryItem item, GridSummaryItemAlignment alignment) {
			IAlignmentItem alignedItem = item as IAlignmentItem;
			if(alignedItem == null)
				return;
			foreach(IAlignmentItem alignmentItem in alignmentListSource.Keys) {
				if(alignmentItem == alignedItem) {
					alignmentListSource[alignmentItem] = alignment;
					return;
				}
			}
		}
		public GridSummaryItemAlignment? GetSummaryItemAlignment(ISummaryItem item) {
			IAlignmentItem alignedItem = item as IAlignmentItem;
			if(alignedItem == null)
				return null;
			foreach(IAlignmentItem alignmentItem in alignmentListSource.Keys) {
				if(alignmentItem == alignedItem)
					return alignmentListSource[alignmentItem];
			}
			return null;
		}
		public bool TryGetAlignment(IAlignmentItem alignmentItem, out GridSummaryItemAlignment actualItemAlignment) {
			if(!alignmentListSource.TryGetValue(alignmentItem, out actualItemAlignment)) {
				actualItemAlignment = alignmentItem.Alignment;
				return true;
			}
			else {
				actualItemAlignment = GridSummaryItemAlignment.Default;
				return false;
			}
		}
	}
}
