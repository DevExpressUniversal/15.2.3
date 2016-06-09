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
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Spreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.XtraSpreadsheet.Model;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Windows.Data;
using DevExpress.Utils;
using System.Windows.Media;
using DevExpress.Xpf.Spreadsheet.Extensions.Internal;
using DrawingColor = System.Drawing.Color;
namespace DevExpress.Xpf.Spreadsheet.Internal {
	public class AddCommand : ICommand {
		public bool CanExecute(object parameter) { return true; }
		public event EventHandler CanExecuteChanged { add { } remove { } }
		public void Execute(object parameter) {
			SpreadsheetTabSelector selector = LayoutHelper.FindParentObject<SpreadsheetTabSelector>(parameter as FrameworkElement);
			selector.OnAddNewItem();
		}
	}
	public class PrevTabCommand : ICommand {
		public bool CanExecute(object parameter) { return true; }
		public event EventHandler CanExecuteChanged { add { } remove { } }
		public void Execute(object parameter) {
			SpreadsheetTabSelector selector = parameter as SpreadsheetTabSelector;
			selector.ScrollToPrevTab();
		}
	}
	public class NextTabCommand : ICommand {
		public bool CanExecute(object parameter) { return true; }
		public event EventHandler CanExecuteChanged { add { } remove { } }
		public void Execute(object parameter) {
			SpreadsheetTabSelector selector = parameter as SpreadsheetTabSelector;
			selector.ScrollToNextTab();
		}
	}
	public class LastTabCommand : ICommand {
		public bool CanExecute(object parameter) { return true; }
		public event EventHandler CanExecuteChanged { add { } remove { } }
		public void Execute(object parameter) {
			SpreadsheetTabSelector selector = parameter as SpreadsheetTabSelector;
			selector.ScrollToLastTab();
		}
	}
	public class FirstTabCommand : ICommand {
		public bool CanExecute(object parameter) { return true; }
		public event EventHandler CanExecuteChanged { add { } remove { } }
		public void Execute(object parameter) {
			SpreadsheetTabSelector selector = parameter as SpreadsheetTabSelector;
			selector.ScrollToFirstTab();
		}
	}
	public class TabSelectorItem : Control {
		#region Fields
		public static readonly DependencyProperty IsSelectedProperty;
		public static readonly DependencyProperty TextProperty;
		public static readonly DependencyProperty ColorProperty;
		#endregion
		static TabSelectorItem() {
			Type ownerType = typeof(TabSelectorItem);
			IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, (d, e) => ((TabSelectorItem)d).OnIsSelectedChanged()));
			TextProperty = DependencyProperty.Register("Text", typeof(string), ownerType,
				new FrameworkPropertyMetadata("", (d, e) => ((TabSelectorItem)d).OnTextChanged()));
			ColorProperty = DependencyProperty.Register("Color", typeof(Color), ownerType);
		}
		public TabSelectorItem() {
			DefaultStyleKey = typeof(TabSelectorItem);
		}
		#region Properties
		public bool IsSelected {
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}
		public string Text {
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
		public Color Color {
			get { return (Color)GetValue(ColorProperty); }
			set { SetValue(ColorProperty, value); }
		}
		#endregion
		private void OnTextChanged() { }
		private void OnIsSelectedChanged() { }
		Grid root;
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			root = LayoutHelper.FindElementByName(this, "PART_Root") as Grid;
		}
		protected override Size MeasureOverride(Size constraint) {
			if (root == null) return new Size();
			root.Measure(constraint);
			return root.DesiredSize;
		}
	}
	public class TabSelectorPanel : Panel {
		#region Fields
		public static readonly DependencyProperty AddButtonTemplateProperty;
		public static readonly DependencyProperty TabSelectorItemDefaultTemplateProperty;
		public static readonly DependencyProperty TabSelectorItemColoredTemplateProperty;
		SpreadsheetTabSelector owner;
		List<UIElement> measuredItems;
		#endregion
		static TabSelectorPanel() {
			Type ownerType = typeof(TabSelectorPanel);
			AddButtonTemplateProperty = DependencyProperty.Register("AddButtonTemplate", typeof(DataTemplate), ownerType);
			TabSelectorItemDefaultTemplateProperty = DependencyProperty.Register("TabSelectorItemDefaultTemplate", typeof(ControlTemplate), ownerType);
			TabSelectorItemColoredTemplateProperty = DependencyProperty.Register("TabSelectorItemColoredTemplate", typeof(ControlTemplate), ownerType);
		}
		public TabSelectorPanel() {
			CreateAddButtonContainer();
		}
		private void CreateAddButtonContainer() {
			ContentPresenter presenter = new ContentPresenter();
			Binding bind = new Binding("AddButtonTemplate") { Source = this };
			presenter.SetBinding(ContentPresenter.ContentTemplateProperty, bind);
			Children.Add(presenter);
		}
		#region Properties
		public int Offset { get; set; }
		public bool FindOffset { get; set; }
		public double Viewport { get; set; }
		public List<string> Items { get; set; }
		public DataTemplate AddButtonTemplate {
			get { return (DataTemplate)GetValue(AddButtonTemplateProperty); }
			set { SetValue(AddButtonTemplateProperty, value); }
		}
		public ControlTemplate TabSelectorItemDefaultTemplate {
			get { return (ControlTemplate)GetValue(TabSelectorItemDefaultTemplateProperty); }
			set { SetValue(TabSelectorItemDefaultTemplateProperty, value); }
		}
		public ControlTemplate TabSelectorItemColoredTemplate {
			get { return (ControlTemplate)GetValue(TabSelectorItemColoredTemplateProperty); }
			set { SetValue(TabSelectorItemColoredTemplateProperty, value); }
		}
		#endregion
		ContentPresenter AddButtonContainer {
			get { return Children.Count > 0 ? Children[0] as ContentPresenter : null; }
			set { Children[0] = value; }
		}
		internal void SetOwner(SpreadsheetTabSelector owner) {
			this.owner = owner;
		}
		protected override Size MeasureOverride(Size availableSize) {
			if (owner == null) return new Size();
			Reset();
			Items = owner.GetTabItems();
			if (Items == null)
				return new Size();
			MeasureAddButton(availableSize);
			if (double.IsInfinity(availableSize.Height)) availableSize.Height = AddButtonContainer.DesiredSize.Height;
			if (double.IsInfinity(availableSize.Width)) availableSize.Width = AddButtonContainer.DesiredSize.Width;
			if (FindOffset) {
				double width = Viewport - AddButtonContainer.ActualWidth;
				FindOffset = false;
				for (int i = Items.Count - 1; i >= 0; i--) {
					TabSelectorItem selectorItem = MeasureTab(availableSize, i);
					width -= selectorItem.ActualWidth;
					if (width < 0) {
						Offset = i + 1;
						measuredItems.RemoveAt(measuredItems.Count - 1);
						freeItems--;
						break;
					}
				}
				measuredItems.Reverse();
			}
			else {
				double width = Viewport;
				for (int i = Offset; i < Items.Count; i++) {
					if (width < 0) break;
					TabSelectorItem selectorItem = MeasureTab(availableSize, i);
					width -= selectorItem.ActualWidth;
				}
			}
			measuredItems.Add(AddButtonContainer);
			return availableSize;
		}
		TabSelectorItem MeasureTab(Size availableSize, int i) {
			TabSelectorItem selectorItem = GetItem();
			string tabName = Items[i];
			selectorItem.Text = tabName;
			selectorItem.IsSelected = owner.IsTabSelected(tabName);
			DrawingColor tabColor = owner.GetTabColor(tabName);
			selectorItem.Color = tabColor.ToWpfColor();
			selectorItem.Template = GetTabSelectorItemTemplate(tabColor);
			selectorItem.Measure(availableSize);
			measuredItems.Add(selectorItem);
			return selectorItem;
		}
		ControlTemplate GetTabSelectorItemTemplate(DrawingColor tabColor) {
			if (DXColor.IsTransparentOrEmpty(tabColor))
				return TabSelectorItemDefaultTemplate;
			return TabSelectorItemColoredTemplate;
		}
		private void Reset() {
			measuredItems = new List<UIElement>();
			freeItems = 1;
		}
		private void MeasureAddButton(Size availableSize) {
			if (AddButtonContainer != null) {
				if (AddButtonContainer.Content == null) AddButtonContainer.Content = owner.AddCommand;
				AddButtonContainer.Measure(availableSize);
			}
		}
		protected override Size ArrangeOverride(Size finalSize) {
			if (measuredItems == null) return finalSize;
			double x = 0;
			foreach (UIElement item in measuredItems) {
				Rect rect = new Rect(new Point(x, 0), item.DesiredSize);
				item.Arrange(rect);
				x += rect.Width;
			}
			HideFreeItems();
			if (Math.Sign(Viewport) > 0)
				Clip = new RectangleGeometry(new Rect(0, 0, Viewport, ActualHeight));
			owner.OnSelectorPanelArrangeFinished();
			return finalSize;
		}
		private void HideFreeItems() {
			for (int i = freeItems; i < Children.Count; i++)
				Children[i].Arrange(new Rect(0, 0, 0, 0));
		}
		int freeItems = 1;
		private TabSelectorItem GetItem() {
			if (freeItems >= Children.Count) {
				TabSelectorItem item = new TabSelectorItem();
				Children.Add(item);
			}
			return Children[freeItems++] as TabSelectorItem;
		}
		internal string GetTextByHitTest(Point point) {
			foreach (UIElement child in Children) {
				if (child.TransformToVisual(this).TransformBounds(new Rect(0, 0, child.DesiredSize.Width, child.DesiredSize.Height)).Contains(point) && child.GetType() == typeof(TabSelectorItem))
					return ((TabSelectorItem)child).Text;
			}
			return "";
		}
		internal void ScrollToLastTab(bool isNewAdded = false) {
			Rect bounds = AddButtonContainer.TransformToVisual(this).TransformBounds(new Rect(0, 0, AddButtonContainer.ActualWidth, AddButtonContainer.ActualHeight));
			if (!IsLastTabVisible() || isNewAdded) {
				FindOffset = true;
				InvalidateMeasure();
			}
		}
		internal void ScrollPrevTab() {
			Offset = Math.Max(Offset - 1, 0);
			InvalidateMeasure();
		}
		internal void ScrollNextTab() {
			if (!IsLastTabVisible()) {
				Offset = Math.Min(Offset + 1, Items.Count - 1);
				InvalidateMeasure();
			}
		}
		internal void ScrollFirstTab() {
			Offset = 0;
			InvalidateMeasure();
		}
		internal bool IsLastTabVisible() {
			Rect bounds = AddButtonContainer.TransformToVisual(this).TransformBounds(new Rect(0, 0, AddButtonContainer.ActualWidth, AddButtonContainer.ActualHeight));
			return bounds.Right <= Viewport;
		}
	}
	public class SpreadsheetTabSelector : Control, ITabSelector {
		public static readonly DependencyProperty AddCommandProperty;
		public static readonly DependencyProperty PrevTabCommandProperty;
		public static readonly DependencyProperty NextTabCommandProperty;
		public static readonly DependencyProperty LastTabCommandProperty;
		public static readonly DependencyProperty FirstTabCommandProperty;
		public static readonly DependencyProperty VisibleWidthProperty;
		public static readonly DependencyProperty AllowStretchProperty;
		static SpreadsheetTabSelector() {
			Type ownerType = typeof(SpreadsheetTabSelector);
			AddCommandProperty = DependencyProperty.Register("AddCommand", typeof(AddCommand), ownerType);
			PrevTabCommandProperty = DependencyProperty.Register("PrevTabCommand", typeof(ICommand), ownerType);
			NextTabCommandProperty = DependencyProperty.Register("NextTabCommand", typeof(ICommand), ownerType);
			LastTabCommandProperty = DependencyProperty.Register("LastTabCommand", typeof(ICommand), ownerType);
			FirstTabCommandProperty = DependencyProperty.Register("FirstTabCommand", typeof(ICommand), ownerType);
			VisibleWidthProperty = DependencyProperty.Register("VisibleWidth", typeof(double), ownerType,
				new FrameworkPropertyMetadata(0d, (d, e) => ((SpreadsheetTabSelector)d).OnVisibleWidthChanged()));
			AllowStretchProperty = DependencyProperty.Register("AllowStretch", typeof(bool), ownerType,
			  new FrameworkPropertyMetadata(false, (d, e) => ((SpreadsheetTabSelector)d).OnAllowStretchChanged()));
		}
		private void OnAllowStretchChanged() {
			UpdateTabSelectorPanel();
		}
		private void OnVisibleWidthChanged() {
			UpdateTabSelectorPanel();
		}
		public SpreadsheetTabSelector() {
			this.DefaultStyleKey = typeof(SpreadsheetTabSelector);
			AddCommand = new AddCommand();
			PrevTabCommand = new PrevTabCommand();
			NextTabCommand = new NextTabCommand();
			FirstTabCommand = new FirstTabCommand();
			LastTabCommand = new LastTabCommand();
			this.Loaded += SpreadsheetTabSelectorLoaded;
			this.Unloaded += SpreadsheetTabSelectorUnloaded;
			this.ContextMenuOpening += SpreadsheetTabSelector_ContextMenuOpening;
			this.SizeChanged += SpreadsheetTabSelector_SizeChanged;
		}
		void SpreadsheetTabSelector_SizeChanged(object sender, SizeChangedEventArgs e) {
			UpdateTabSelectorPanel();
		}
		void SpreadsheetTabSelector_ContextMenuOpening(object sender, ContextMenuEventArgs e) {
			e.Handled = spreadsheetControl.ShowContextMenu();
		}
		public event EventHandler ActiveTabChanging;
		DocumentModel DocumentModel { get { return spreadsheetControl.DocumentModel; } }
		SpreadsheetControl spreadsheetControl;
		void SpreadsheetTabSelectorLoaded(object sender, RoutedEventArgs e) {
			spreadsheetControl = LayoutHelper.FindParentObject<SpreadsheetControl>(this);
			if (spreadsheetControl != null) {
				SyncSelectedTabIndex();
				PopulateTabs();
				SubscribeEvents();
			}
		}
		private void SyncSelectedTabIndex() {
			SelectedTabName = DocumentModel.ActiveSheet.Name;
		}
		void DocumentModel_SheetRemoved(object sender, DevExpress.Spreadsheet.SheetRemovedEventArgs e) {
		}
		void DocumentModel_SheetInserted(object sender, DevExpress.Spreadsheet.SheetInsertedEventArgs e) {
			SelectTabByName(e.SheetName);
		}
		void OnSheetRenamed(object sender, DevExpress.Spreadsheet.SheetRenamedEventArgs e) {
			PopulateTabs();
		}
		void OnDocumentCleared(object sender, EventArgs e) {
			if (selectorPanel != null) selectorPanel.Offset = 0;
			PopulateTabs();
		}
		private void OnAfterSheetInserted(object sender, WorksheetCollectionChangedEventArgs e) {
			PopulateTabs();
		}
		void OnSheetVisibleStateChanged(object sender, SheetVisibleStateChangedEventArgs e) {
			if (e.OldValue == SheetVisibleState.Visible || e.NewValue == SheetVisibleState.Visible) {
				PopulateTabs();
				SelectTabByName(e.SheetName);
			}
		}
		void OnActiveSheetChanged(object sender, DevExpress.Spreadsheet.ActiveSheetChangedEventArgs e) {
			SelectTabByName(e.NewActiveSheetName);
		}
		void OnTabColorChanged(object sender, SheetTabColorChangedEventArgs e) {
			PopulateTabs();
		}
		private void SelectTabByName(string name) {
			SelectedTabName = name;
		}
		void SpreadsheetTabSelectorUnloaded(object sender, RoutedEventArgs e) {
			UnSubscribeEvents();
		}
		private void SubscribeEvents() {
			if (spreadsheetControl == null)
				return;
			DocumentModel.InnerActiveSheetChanged += OnActiveSheetChanged;
			DocumentModel.SheetVisibleStateChanged += OnSheetVisibleStateChanged;
			DocumentModel.InternalAPI.AfterSheetInserted += OnAfterSheetInserted;
			DocumentModel.InnerSheetRenamed += OnSheetRenamed;
			DocumentModel.DocumentCleared += OnDocumentCleared;
			DocumentModel.SheetInserted += DocumentModel_SheetInserted;
			DocumentModel.SheetRemoved += DocumentModel_SheetRemoved;
			DocumentModel.SheetTabColorChanged += OnTabColorChanged;
		}
		private void UnSubscribeEvents() {
			if (spreadsheetControl == null)
				return;
			DocumentModel.InnerActiveSheetChanged -= OnActiveSheetChanged;
			DocumentModel.SheetVisibleStateChanged -= OnSheetVisibleStateChanged;
			DocumentModel.InternalAPI.AfterSheetInserted -= OnAfterSheetInserted;
			DocumentModel.InnerSheetRenamed -= OnSheetRenamed;
			DocumentModel.DocumentCleared -= OnDocumentCleared;
			DocumentModel.SheetInserted -= DocumentModel_SheetInserted;
			DocumentModel.SheetRemoved -= DocumentModel_SheetRemoved;
			DocumentModel.SheetTabColorChanged -= OnTabColorChanged;
		}
		public ICommand AddCommand {
			get { return (ICommand)GetValue(AddCommandProperty); }
			set { SetValue(AddCommandProperty, value); }
		}
		public ICommand PrevTabCommand {
			get { return (ICommand)GetValue(PrevTabCommandProperty); }
			set { SetValue(PrevTabCommandProperty, value); }
		}
		public ICommand NextTabCommand {
			get { return (ICommand)GetValue(NextTabCommandProperty); }
			set { SetValue(NextTabCommandProperty, value); }
		}
		public ICommand LastTabCommand {
			get { return (LastTabCommand)GetValue(LastTabCommandProperty); }
			set { SetValue(LastTabCommandProperty, value); }
		}
		public ICommand FirstTabCommand {
			get { return (FirstTabCommand)GetValue(FirstTabCommandProperty); }
			set { SetValue(FirstTabCommandProperty, value); }
		}
		public double VisibleWidth {
			get { return (double)GetValue(VisibleWidthProperty); }
			set { SetValue(VisibleWidthProperty, value); }
		}
		Grid root;
		Grid navButtonsContainer;
		Grid addButtonContainer;
		TabSelectorPanel selectorPanel;
		Button prevBtn, nextBtn, firstBtn, lastBtn;
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			UnSubcribeMouseEvents();
			root = LayoutHelper.FindElementByName(this, "PART_RootContainer") as Grid;
			navButtonsContainer = LayoutHelper.FindElementByName(this, "PART_NavButtonContainer") as Grid;
			FindButtons();
			addButtonContainer = LayoutHelper.FindElementByName(this, "PART_AddButtonContainer") as Grid;
			selectorPanel = LayoutHelper.FindElementByType(this, typeof(TabSelectorPanel)) as TabSelectorPanel;
			selectorPanel.SetOwner(this);
			UpdateTabSelectorPanel();
			SubcribeMouseEvents();
		}
		void UpdateTabSelectorPanel() {
			if (selectorPanel != null) {
				SetSelectorPanelViewport();
				MeasureSelectorPanel();
			}
		}
		private void SetSelectorPanelViewport() {
			if (selectorPanel != null)
				selectorPanel.Viewport = GetViewport();
		}
		public List<string> GetTabNames() {
			return DocumentModel != null ? DocumentModel.GetVisibleSheetNames() : null;
		}
		private void SubcribeMouseEvents() {
			selectorPanel.MouseDown += selectorPanel_MouseDown;
			selectorPanel.MouseUp += selectorPanel_MouseUp;
		}
		private void UnSubcribeMouseEvents() {
			if (selectorPanel != null) {
				selectorPanel.MouseDown -= selectorPanel_MouseDown;
				selectorPanel.MouseUp -= selectorPanel_MouseUp;
			}
		}
		void selectorPanel_MouseUp(object sender, MouseButtonEventArgs e) {
			string tabName = selectorPanel.GetTextByHitTest(e.GetPosition(selectorPanel));
			if (!string.IsNullOrEmpty(tabName) && isMouseDown) {
				SelectTabByName(tabName);
			}
			isMouseDown = false;
		}
		bool isMouseDown = false;
		void selectorPanel_MouseDown(object sender, MouseButtonEventArgs e) {
			e.Handled = true;
			isMouseDown = true;
		}
		public bool AllowStretch {
			get { return (bool)GetValue(AllowStretchProperty); }
			set { SetValue(AllowStretchProperty, value); }
		}
		private double GetViewport() {
			return !AllowStretch ? VisibleWidth - navButtonsContainer.ActualWidth : ActualWidth - navButtonsContainer.ActualWidth;
		}
		private void FindButtons() {
			if (navButtonsContainer != null) {
				prevBtn = LayoutHelper.FindElementByName(navButtonsContainer, "PART_PrevButton") as Button;
				nextBtn = LayoutHelper.FindElementByName(navButtonsContainer, "PART_NextButton") as Button;
				lastBtn = LayoutHelper.FindElementByName(navButtonsContainer, "PART_LastButton") as Button;
				firstBtn = LayoutHelper.FindElementByName(navButtonsContainer, "PART_FirstButton") as Button;
			}
		}
		void OnSelectedTabChanged() {
			RaiseSelectedTabChanging();
			ChangeActiveSheet();
			MeasureSelectorPanel();
		}
		void ChangeActiveSheet() {
			DocumentModel.BeginUpdateFromUI();
			try {
				DocumentModel.ActiveSheet = DocumentModel.Sheets[SelectedTabName];
			}
			finally {
				DocumentModel.EndUpdateFromUI();
			}
		}
		void MeasureSelectorPanel() {
			if (selectorPanel != null)
				selectorPanel.InvalidateMeasure();
		}
		void RaiseSelectedTabChanging() {
			if (this.ActiveTabChanging != null)
				this.ActiveTabChanging(this, EventArgs.Empty);
		}
		void SelectFirstTab() {
			SelectedTabName = GetFirstSheet().Name;
		}
		IWorksheet GetFirstSheet() {
			return DocumentModel.GetSheets()[0];
		}
		void SelectLastItem() {
			SelectedTabName = GetLastSheet().Name;
		}
		IWorksheet GetLastSheet() {
			var sheets = DocumentModel.GetSheets();
			return sheets[sheets.Count - 1];
		}
		internal void ScrollToLastTab() {
			selectorPanel.ScrollToLastTab();
		}
		internal void ScrollToPrevTab() {
			selectorPanel.ScrollPrevTab();
		}
		private void UpdateButtonsVisibility() {
			bool isEnabled = selectorPanel.Offset > 0;
			prevBtn.IsEnabled = isEnabled;
			firstBtn.IsEnabled = isEnabled;
			isEnabled = !selectorPanel.IsLastTabVisible();
			nextBtn.IsEnabled = isEnabled;
			lastBtn.IsEnabled = isEnabled;
		}
		internal void ScrollToNextTab() {
			selectorPanel.ScrollNextTab();
		}
		internal void ScrollToFirstTab() {
			selectorPanel.ScrollFirstTab();
		}
		internal void PopulateTabs() {
			MeasureSelectorPanel();
		}
		internal void OnAddNewItem() {
			SpreadsheetCommand command = CreateAddNewWorksheetCommand();
			command.Execute();
			selectorPanel.ScrollToLastTab(true);
		}
		private SpreadsheetCommand CreateAddNewWorksheetCommand() {
			return spreadsheetControl.CreateCommand(SpreadsheetCommandId.AddNewWorksheet);
		}
		internal string GetTabText(int i, out bool isSelected) {
			IWorksheet sheet = DocumentModel.GetSheetByIndex(i);
			isSelected = sheet == DocumentModel.ActiveSheet;
			return sheet.Name;
		}
		string selectedTabName = "";
		public string SelectedTabName {
			get { return selectedTabName; }
			internal set {
				if (ValidateTabName(value)) {
					selectedTabName = value;
					OnSelectedTabChanged();
				}
			}
		}
		private bool ValidateTabName(string value) {
			return DocumentModel.GetSheetByName(value) != null;
		}
		internal bool CanGetText(int index) {
			return DocumentModel.GetVisibleSheets().Count > index;
		}
		internal void OnSelectorPanelArrangeFinished() {
			UpdateButtonsVisibility();
		}
		#region ITabSelector Members
		public void SwitchNextWorksheet() {
			NextTabCommand.Execute(this);
		}
		public void SwitchPreviousWorksheet() {
			PrevTabCommand.Execute(this);
		}
		#endregion
		internal List<string> GetTabItems() {
			if (spreadsheetControl == null)
				return null;
			return DocumentModel.GetVisibleSheetNames();
		}
		internal bool IsTabSelected(string tabName) {
			return DocumentModel.ActiveSheet.Name == tabName;
		}
		internal DrawingColor GetTabColor(string tabName) {
			return DocumentModel.Sheets[tabName].GetTabColor();
		}
		internal string GetTabNameByPoint(Point point) {
			Point tabPoint = this.TransformToVisual(selectorPanel).Transform(point);
			return selectorPanel.GetTextByHitTest(tabPoint);
		}
	}
}
