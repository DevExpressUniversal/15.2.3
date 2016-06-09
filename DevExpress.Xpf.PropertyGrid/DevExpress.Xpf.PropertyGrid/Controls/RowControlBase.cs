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

using System.Windows.Input;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using DevExpress.Xpf.PropertyGrid.Internal;
using System.Windows.Controls.Primitives;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.PropertyGrid {
	public enum RowControlVisualStates {
		Normal = 0x0,
		Inactive = 0x1,
		Selected = 0x2,
		ReadOnly = 0x4,
		SelectedInactive = Selected | Inactive,
		SelectedReadOnly = Selected | ReadOnly,
		SelectedInactiveReadOnly = SelectedInactive | ReadOnly,
	}
	public class RowControlBase : Control, INavigationSupport {		
		public static readonly DependencyProperty RowDataProperty;
		public static readonly DependencyProperty OwnerViewProperty;
		public static readonly DependencyProperty ActualIndentProperty;
		public static readonly DependencyProperty ExpandButtonWidthProperty;
		public static readonly DependencyProperty DesiredHeaderWidthProperty;
		public static readonly DependencyProperty ActualHeaderWidthProperty;
		public static readonly DependencyProperty StateProperty;
		public static readonly DependencyProperty RenderReadOnlyProperty;
		public static readonly DependencyProperty IsReadOnlyProperty;
		protected static readonly DependencyPropertyKey DescriptionPropertyKey;
		public static readonly DependencyProperty DescriptionProperty;
		protected static readonly DependencyPropertyKey IsCategoryPropertyKey;
		public static readonly DependencyProperty IsCategoryProperty;
		public static readonly DependencyProperty CellTemplateSelectorProperty;
		public static readonly DependencyProperty CellTemplateProperty;
		protected static readonly DependencyPropertyKey ActualCellTemplateSelectorPropertyKey;
		public static readonly DependencyProperty ActualCellTemplateSelectorProperty;
		public static readonly DependencyProperty CustomShowCommandButtonProperty;
		public static readonly DependencyProperty ShowCommandButtonProperty;
		public static readonly DependencyProperty ActualShowCommandButtonProperty;
		public DataTemplateSelector ActualCellTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ActualCellTemplateSelectorProperty); }
			protected internal set { this.SetValue(ActualCellTemplateSelectorPropertyKey, value); }
		}
		public DataTemplate CellTemplate {
			get { return (DataTemplate)GetValue(CellTemplateProperty); }
			set { SetValue(CellTemplateProperty, value); }
		}
		public DataTemplateSelector CellTemplateSelector {
			get { return (DataTemplateSelector)GetValue(CellTemplateSelectorProperty); }
			set { SetValue(CellTemplateSelectorProperty, value); }
		}
		public bool IsCategory {
			get { return (bool)GetValue(IsCategoryProperty); }
			protected internal set { this.SetValue(IsCategoryPropertyKey, value); }
		}		
		public double ActualHeaderWidth {
			get { return (double)GetValue(ActualHeaderWidthProperty); }
			set { SetValue(ActualHeaderWidthProperty, value); }
		}
		public object Description {
			get { return (object)GetValue(DescriptionProperty); }
			protected internal set { this.SetValue(DescriptionPropertyKey, value); }
		}
		public bool ShowCommandButton {
			get { return (bool)GetValue(ShowCommandButtonProperty); }
			set { SetValue(ShowCommandButtonProperty, value); }
		}
		public bool? CustomShowCommandButton {
			get { return (bool?)GetValue(CustomShowCommandButtonProperty); }
			set { SetValue(CustomShowCommandButtonProperty, value); }
		}
		public bool ActualShowCommandButton {
			get { return (bool)GetValue(ActualShowCommandButtonProperty); }
			set { SetValue(ActualShowCommandButtonProperty, value); }
		}
		protected internal bool IsLastElement { get; set; }
		internal event RequestPresenterEventHandler RequestPresenter;
		public bool IsSelected {
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}
		public bool IsExpanded {
			get { return (bool)GetValue(IsExpandedProperty); }
			set { SetValue(IsExpandedProperty, value); }
		}
		public static readonly DependencyProperty IsExpandedProperty;		
		public static readonly DependencyProperty IsSelectedProperty;		
		static RowControlBase() {
			Type ownerType = typeof(RowControlBase);
			DefaultStyleKeyProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(ownerType));
			NavigationManager.NavigationModeProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(NavigationMode.Auto));
			RowDataProperty = DependencyPropertyManager.Register("RowData", typeof(RowDataBase), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((RowControlBase)d).OnRowDataChanged((RowDataBase)e.OldValue)));
			ActualIndentProperty = DependencyPropertyManager.Register("ActualIndent", typeof(double), ownerType, new FrameworkPropertyMetadata(null));
			ExpandButtonWidthProperty = DependencyPropertyManager.Register("ExpandButtonWidth", typeof(double), ownerType, new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure, (d, e) => ((RowControlBase)d).OnExpandButtonWidthChanged((double)e.OldValue)));
			OwnerViewProperty = DependencyPropertyManager.Register("OwnerView", typeof(PropertyGridView), ownerType, new FrameworkPropertyMetadata(null, (d,e)=>((RowControlBase)d).OnOwnerViewChanged((PropertyGridView)e.OldValue, (PropertyGridView)e.NewValue)));
			DesiredHeaderWidthProperty = DependencyPropertyManager.Register("DesiredHeaderWidth", typeof(double), ownerType, new FrameworkPropertyMetadata(0d, (d, e) => ((RowControlBase)d).OnDesiredHeaderWidthChanged((double)e.OldValue)));
			StateProperty = DependencyPropertyManager.Register("State", typeof(RowControlVisualStates), ownerType, new FrameworkPropertyMetadata(RowControlVisualStates.Normal));
			RenderReadOnlyProperty = DependencyPropertyManager.Register("RenderReadOnly", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, (d, e) => ((RowControlBase)d).OnRenderReadOnlyChanged((bool)e.OldValue)));
			IsReadOnlyProperty = DependencyPropertyManager.Register("IsReadOnly", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, (d, e) => ((RowControlBase)d).OnIsReadOnlyChanged((bool)e.OldValue)));			
			IsEnabledProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(true, (d, e) => ((RowControlBase)d).OnIsEnabledChanged((bool)e.OldValue)));
			IsSelectedProperty = Selector.IsSelectedProperty.AddOwner(typeof(RowControlBase), new FrameworkPropertyMetadata(false, (d, e) => ((RowControlBase)d).OnIsSelectedChanged((bool)e.OldValue)));
			IsExpandedProperty = DependencyPropertyManager.Register("IsExpanded", typeof(bool), typeof(RowControlBase), new FrameworkPropertyMetadata(false, (d,e)=>((RowControlBase)d).OnIsExpandedChanged((bool)e.OldValue)));
			ActualHeaderWidthProperty = DependencyPropertyManager.Register("ActualHeaderWidth", typeof(double), typeof(RowControlBase), new FrameworkPropertyMetadata(double.NaN, (d, e) => ((RowControlBase)d).OnActualHeaderWidthChanged((double)e.OldValue, (double)e.NewValue)));
			DescriptionPropertyKey = DependencyPropertyManager.RegisterReadOnly("Description", typeof(object), typeof(RowControlBase), new FrameworkPropertyMetadata(null));
			DescriptionProperty = DescriptionPropertyKey.DependencyProperty;
			IsCategoryPropertyKey = DependencyPropertyManager.RegisterReadOnly("IsCategory", typeof(bool), typeof(RowControlBase), new FrameworkPropertyMetadata(false, (d, e) => ((RowControlBase)d).OnIsCategoryChanged((bool)e.OldValue)));
			IsCategoryProperty = IsCategoryPropertyKey.DependencyProperty;
			CellTemplateSelectorProperty = DependencyPropertyManager.Register("CellTemplateSelector", typeof(DataTemplateSelector), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((RowControlBase)d).OnCellTemplateSelectorChanged((DataTemplateSelector)e.OldValue, (DataTemplateSelector)e.NewValue)));
			CellTemplateProperty = DependencyPropertyManager.Register("CellTemplate", typeof(DataTemplate), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((RowControlBase)d).OnCellTemplateChanged((DataTemplate)e.OldValue, (DataTemplate)e.NewValue)));
			ActualCellTemplateSelectorPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualCellTemplateSelector", typeof(DataTemplateSelector), ownerType, new FrameworkPropertyMetadata(null));
			ActualCellTemplateSelectorProperty = ActualCellTemplateSelectorPropertyKey.DependencyProperty;
			CustomShowCommandButtonProperty = DependencyPropertyManager.Register("CustomShowCommandButton", typeof(bool?), typeof(RowControlBase), new FrameworkPropertyMetadata(null, (d, e) => ((RowControlBase)d).UpdateActualShowCommandButton()));
			ShowCommandButtonProperty = DependencyPropertyManager.Register("ShowCommandButton", typeof(bool), typeof(RowControlBase), new FrameworkPropertyMetadata(true, (d, e) => ((RowControlBase)d).UpdateActualShowCommandButton()));
			ActualShowCommandButtonProperty = DependencyPropertyManager.Register("ActualShowCommandButton", typeof(bool), typeof(RowControlBase), new FrameworkPropertyMetadata(true));
		}
		public bool CanShowCollectionButton {
			get { return (bool)GetValue(CanShowCollectionButtonProperty); }
			set { SetValue(CanShowCollectionButtonProperty, value); }
		}
		public static readonly DependencyProperty CanShowCollectionButtonProperty =
			DependencyPropertyManager.Register("CanShowCollectionButton", typeof(bool), typeof(RowControlBase), new FrameworkPropertyMetadata(false, OnCanShowCollectionButtonPropertyChanged));
		protected static void OnCanShowCollectionButtonPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RowControlBase)d).OnCanShowCollectionButtonChanged((bool)e.OldValue);
		}
		protected virtual void OnCanShowCollectionButtonChanged(bool oldValue) {
			UpdateShowCollectionButton();
		}
		public bool IsCollectionRow {
			get { return (bool)GetValue(IsCollectionRowProperty); }
			set { SetValue(IsCollectionRowProperty, value); }
		}
		public bool ShowCollectionButton {
			get { return (bool)GetValue(ShowCollectionButtonProperty); }
			set { SetValue(ShowCollectionButtonProperty, value); }
		}
		public static readonly DependencyProperty ShowCollectionButtonProperty =
			DependencyPropertyManager.Register("ShowCollectionButton", typeof(bool), typeof(RowControlBase), new FrameworkPropertyMetadata(false, OnShowCollectionButtonPropertyChanged));
		protected static void OnShowCollectionButtonPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RowControlBase)d).OnShowCollectionButtonChanged((bool)e.OldValue);
		}
		protected virtual void OnShowCollectionButtonChanged(bool oldValue) {
		}
		public static readonly DependencyProperty IsCollectionRowProperty =
			DependencyPropertyManager.Register("IsCollectionRow", typeof(bool), typeof(RowControlBase), new FrameworkPropertyMetadata(false, OnIsCollectionRowPropertyChanged));
		protected static void OnIsCollectionRowPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RowControlBase)d).OnIsCollectionRowChanged((bool)e.OldValue);
		}
		protected virtual void OnIsCollectionRowChanged(bool oldValue) {
		}
		public RowDataBase RowData {
			get { return (RowDataBase)GetValue(RowDataProperty); }
			set { SetValue(RowDataProperty, value); }
		}		
		public double ActualIndent {
			get { return (double)GetValue(ActualIndentProperty); }
			set { SetValue(ActualIndentProperty, value); }
		}
		public double ExpandButtonWidth {
			get { return (double)GetValue(ExpandButtonWidthProperty); }
			set { SetValue(ExpandButtonWidthProperty, value); }
		}
		public PropertyGridView OwnerView {
			get { return (PropertyGridView)GetValue(OwnerViewProperty); }
			set { SetValue(OwnerViewProperty, value); }
		}
		public double DesiredHeaderWidth {
			get { return (double)GetValue(DesiredHeaderWidthProperty); }
			set { SetValue(DesiredHeaderWidthProperty, value); }
		}
		public RowControlVisualStates State {
			get { return (RowControlVisualStates)GetValue(StateProperty); }
			set { SetValue(StateProperty, value); }
		}
		public bool RenderReadOnly {
			get { return (bool)GetValue(RenderReadOnlyProperty); }
			set { SetValue(RenderReadOnlyProperty, value); }
		}
		public bool IsReadOnly {
			get { return (bool)GetValue(IsReadOnlyProperty); }
			set { SetValue(IsReadOnlyProperty, value); }
		}				
		protected internal virtual FrameworkElement MenuPlacementTarget {
			get { return this; }
		}
		public RowControlBase() {
			SetBinding(ShowCommandButtonProperty, new Binding("OwnerView.PropertyGrid.ShowMenuButtonInRows") { RelativeSource = RelativeSource.Self });
			PropertyGridHelper.SetRowControl(this, this);
		}
		protected virtual void OnIsCategoryChanged(bool oldValue) { }		
		protected virtual void OnExpandButtonWidthChanged(double oldValue) {
			UpdateActualIndent();
		}
		readonly Locker setIsExpandedLocker = new Locker();
		void OnRowDataIsExpandedChanged(object sender, EventArgs e) {
			if (setIsExpandedLocker.IsLocked)
				return;
			using (setIsExpandedLocker.Lock()) {
				IsExpanded = RowData.IsExpanded;
			}			
		}
		protected virtual void OnIsExpandedChanged(bool oldValue) {
			if (setIsExpandedLocker.IsLocked)
				return;
			using (setIsExpandedLocker.Lock()) {
				RowData.IsExpanded = IsExpanded;
			}
		}
		protected virtual void OnDesiredHeaderWidthChanged(double oldValue) {
			UpdateActualDescription();
		}
		protected virtual void OnActualHeaderWidthChanged(double oldValue, double newValue) {
			UpdateActualDescription();
		}
		void UpdateActualDescription() {
			if (RowData == null)
				return;
			if (RowData.Description == null || !OwnerView.With(x => x.PropertyGrid).If(x => (x.ShowDescriptionIn & DescriptionLocation.ToolTip) != 0).ReturnSuccess()) {
				if (ActualHeaderWidth < DesiredHeaderWidth) {
					bool shouldSet = !(Description is ContentControl) || Description is PropertyDescriptionPresenterControl;
					var desc = !shouldSet ? Description as ContentControl : new ContentControl();
					desc.Content = RowData.Header;
					desc.Opacity = !(RenderReadOnly || IsReadOnly) ? 1.0 : 0.75;
					if (shouldSet)
						Description = desc;
					return;
				}
			}
			else {
				if (Description is PropertyDescriptionPresenterControl)
					return;
				var desc = new PropertyDescriptionPresenterControl();
				desc.SelectedRow = RowData;
				desc.SetBinding(ContentControl.ContentTemplateSelectorProperty, new Binding("ActualDescriptionTemplateSelector") { Source = OwnerView.PropertyGrid });
				desc.SetBinding(ContentControl.StyleProperty, new Binding("ActualDescriptionContainerStyle") { Source = OwnerView.PropertyGrid });
				desc.IsInTooltip = true;
				desc.IsEnabled = !(RenderReadOnly || IsReadOnly);
				Description = desc;
				return;
			}
			Description = null;
			return;
		}
		bool initPending = false;
		public override void BeginInit() {
			if (initPending)
				return;
			initPending = true;
			base.BeginInit();
		}
		public override void EndInit() {
			if (!initPending)
				return;
			base.EndInit();
			initPending = false;
			UpdateActualCellTemplateSelector();
		}
		protected virtual void OnOwnerViewChanged(PropertyGridView oldValue, PropertyGridView newValue) { }
		protected internal event EventHandler RowDataChanged;
		protected virtual void OnRowDataChanged(RowDataBase oldValue) {
			BeginInit();			
			if (oldValue != null) {
				oldValue.IsReadOnlyChanged -= OnRowDataIsReadOnlyChanged;
				oldValue.IsExpandedChanged -= OnRowDataIsExpandedChanged;
				oldValue.IsSelectedChanged -= OnRowDataIsSelectedChanged;
				oldValue.IsCollectionRowChanged -= OnRowDataIsCollectionRowChanged;
				oldValue.IsModifiableCollectionItemChanged -= OnRowDataIsModifiableCollectionItemChanged;
				oldValue.ShowMenuButtonChanged -= OnRowShowMenuButtonChanged;
				oldValue.RenderReadOnlyChanged -= OnRowDataRenderReadOnlyChanged;
				oldValue.HeaderChanged -= OnRowDataHeaderChanged;
			}
			if (RowData != null) {
				RowData.IsReadOnlyChanged += OnRowDataIsReadOnlyChanged;
				RowData.IsExpandedChanged += OnRowDataIsExpandedChanged;
				RowData.IsSelectedChanged += OnRowDataIsSelectedChanged;
				RowData.IsCollectionRowChanged += OnRowDataIsCollectionRowChanged;
				RowData.IsModifiableCollectionItemChanged += OnRowDataIsModifiableCollectionItemChanged;
				RowData.ShowMenuButtonChanged += OnRowShowMenuButtonChanged;
				RowData.RenderReadOnlyChanged += OnRowDataRenderReadOnlyChanged;
				RowData.HeaderChanged += OnRowDataHeaderChanged;
				UpdateActualIndent();
				OnRowDataIsReadOnlyChanged(null, null);
				OnRowDataIsExpandedChanged(null, null);
				OnRowDataIsSelectedChanged(null, null);
				OnRowDataIsCollectionRowChanged(null, null);
				OnRowDataIsModifiableCollectionItemChanged(null, null);
				OnRowShowMenuButtonChanged(null, null);
				OnRowDataRenderReadOnlyChanged(null, null);
				OnRowDataHeaderChanged(null, null);				
			}						
			UpdateCellTemplateAndSelector();
			PropertyGridHelper.SetRowData(this, RowData);
			EndInit();
			if (RowDataChanged != null)
				RowDataChanged(this, EventArgs.Empty);
		}		
		protected virtual void UpdateCellTemplateAndSelector() {
			if (RowData != null && !IsCategory) {
				BindingOperations.SetBinding(this, CellTemplateProperty, new Binding("Definition.CellTemplate") { Source = RowData, Mode = BindingMode.TwoWay });
				BindingOperations.SetBinding(this, CellTemplateSelectorProperty, new Binding("Definition.CellTemplateSelector") { Source = RowData, Mode = BindingMode.TwoWay });
			}
			else {
				ClearValue(CellTemplateProperty);
				ClearValue(CellTemplateSelectorProperty);
			}
		}
		void OnRowDataIsCollectionRowChanged(object sender, EventArgs e) { IsCollectionRow = RowData.IsCollectionRow; }		
		void OnRowDataIsReadOnlyChanged(object sender, EventArgs e) { IsReadOnly = RowData.IsReadOnly; }
		void OnRowDataIsModifiableCollectionItemChanged(object sender, EventArgs e) { CanShowCollectionButton = RowData.IsModifiableCollectionItem; }
		void OnRowShowMenuButtonChanged(object sender, EventArgs e) { CustomShowCommandButton = RowData.CanShowMenu; }
		void OnRowDataRenderReadOnlyChanged(object sender, EventArgs e) { RenderReadOnly = RowData.RenderReadOnly; }
		void OnRowDataHeaderChanged(object sender, EventArgs e) {
			string stringHeader = RowData == null ? null : RowData.Header;
			if (stringHeader != null) {
				Typeface typeface = new Typeface(TextElement.GetFontFamily(this), TextElement.GetFontStyle(this), TextElement.GetFontWeight(this), TextElement.GetFontStretch(this));
				FormattedText formattedText = new FormattedText(stringHeader, System.Globalization.CultureInfo.CurrentCulture, FlowDirection, typeface, TextElement.GetFontSize(this),
				Brushes.Black, new NumberSubstitution() { Substitution = NumberSubstitution.GetSubstitution(this) }, TextOptions.GetTextFormattingMode(this));
				DesiredHeaderWidth = formattedText.Width;
			}
			else {
				DesiredHeaderWidth = 0d;
			}
		}
		protected virtual void OnRenderReadOnlyChanged(bool oldValue) {
			UpdateState();
		}
		protected virtual void OnIsReadOnlyChanged(bool oldValue) {
			UpdateState();
		}
		protected virtual void OnIsEnabledChanged(bool oldValue) {
			UpdateState();
		}
		protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e) {
			base.OnIsKeyboardFocusWithinChanged(e);
			UpdateState();
			if (IsKeyboardFocusWithin) {
				if (OwnerView == null)
					return;
				var rowControl = (Keyboard.FocusedElement as DependencyObject).With(PropertyGridHelper.GetRowControl);
				var pGrid = rowControl.With(x => x.OwnerView).With(x => x.PropertyGrid);
				if (rowControl == this || pGrid != OwnerView.PropertyGrid && PropertyGridHelper.GetRowControl(VisualTreeHelper.GetParent(rowControl)) == this)
					OwnerView.SelectionStrategy.SelectViaHandle(RowData.Handle);
			}
		}
		readonly Locker changeIsSelectedLocker = new Locker();
		void OnRowDataIsSelectedChanged(object sender, EventArgs e) {
			if (changeIsSelectedLocker.IsLocked)
				return;
			using (changeIsSelectedLocker.Lock()) {
				IsSelected = RowData.IsSelected;
			}			
		}
		protected virtual void OnIsSelectedChanged(bool oldValue) {
			if (!changeIsSelectedLocker.IsLocked) {
				using (changeIsSelectedLocker.Lock())
					RowData.IsSelected = IsSelected;
			}
			if (!IsSelected) {
				Navigating = false;
				GetPresenters().ForEach(x => x.IsSelected = false);
			} else {
				GetPresenters().OrderBy(x => NavigationManager.GetNavigationIndex(x)).If(x => x.All(p => !p.IsSelected)).With(x => x.FirstOrDefault()).Do(x => x.IsSelected = true);
			}
			UpdateState();
		}
		protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e) {
			base.OnMouseEnter(e);
			UpdateState();
		}
		IEnumerable<CellEditorPresenter> GetPresenters() {
			if (RequestPresenter == null)
				return new CellEditorPresenter[] { };
			RequestPresenterEventArgs args = new RequestPresenterEventArgs();
			RequestPresenter(this, args);
			return args.GetPresenters();
		}
		protected internal void EnsureSelection(CellEditorPresenter cellEditorPresenter) {
			foreach (var presenter in GetPresenters()) {
				presenter.IsSelected = (presenter == cellEditorPresenter);
			}
		}
		protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e) {
			base.OnMouseLeave(e);
			UpdateState();
		}
		protected virtual void UpdateShowCollectionButton() {
			ShowCollectionButton = CanShowCollectionButton;
		}
		public override void OnApplyTemplate() { base.OnApplyTemplate(); }				
		protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e) {
		}
		protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e) {
			IsSelected = true;
		}		
		protected virtual void UpdateActualIndent() {
			if (RowData != null)
				ActualIndent = RowData.Level * ExpandButtonWidth;
		}
		protected virtual void UpdateState() {
			RowControlVisualStates currentState = RowControlVisualStates.Normal;
			if (!IsEnabled || RenderReadOnly && !IsCategory) {
				currentState |= RowControlVisualStates.ReadOnly;
			}
			if (IsSelected) {
				currentState |= RowControlVisualStates.Selected;
				if (!OwnerView.Return(x=>x.IsKeyboardFocusWithin, ()=>false))
					currentState |= RowControlVisualStates.Inactive;
			}
			State = currentState;
		}		
		protected virtual void OnCellTemplateSelectorChanged(DataTemplateSelector oldValue, DataTemplateSelector newValue) {
			UpdateActualCellTemplateSelector();
		}
		protected virtual void OnCellTemplateChanged(DataTemplate oldValue, DataTemplate newValue) {
			UpdateActualCellTemplateSelector();
		}		
		protected internal virtual void UpdateActualShowCommandButton() {
			ActualShowCommandButton = CustomShowCommandButton.HasValue ? CustomShowCommandButton.Value : ShowCommandButton;
		}
		protected internal void UpdateActualCellTemplateSelector() {
			if (initPending || RowData==null)
				return;
			ActualCellTemplateSelector = new PrioritizedDataTemplateSelector() { CustomTemplate = CellTemplate, CustomTemplateSelector = CellTemplateSelector };
		}		
		#region INavigationSupport Members
		bool HasActiveEditor { get { return OwnerView.CellEditorOwner.ActiveEditor != null; } }
		protected internal bool NavigatingInsideHeader {
			get {
				return false;
			}
		}
		protected internal bool Navigating { get; set; }
		bool INavigationSupport.ProcessNavigation(NavigationDirection direction) {
			if (direction == NavigationDirection.Right && RowData.CanExpand && !RowData.IsExpanded) {
				RowData.IsExpanded = true;
				return true;
			}
			if (direction == NavigationDirection.Left && RowData.IsExpanded) {
				RowData.IsExpanded = false;
				return true;
			}
			if (direction == NavigationDirection.Enter) {
				return NavigationManager.GetChildren(this).FirstOrDefault().If(x => !x.IsKeyboardFocusWithin).Return(x => x.Focus(), () => false);
			}
			return false;
		}
		protected bool HasCustomTemplate { get { return Template != null; } }
		IEnumerable<FrameworkElement> INavigationSupport.GetChildren() {
			if (!HasCustomTemplate) {
				return GetPresenters();
			} else {
				List<FrameworkElement> children = new List<FrameworkElement>();
				var enumerator = new ConditionalVisualTreeEnumerator(this, x => NavigationManager.GetNavigationMode(x) == NavigationMode.None);
				while (enumerator.MoveNext())
					(enumerator.Current as FrameworkElement).If(x => NavigationManager.GetNavigationMode(x) != NavigationMode.None).Do(x => children.Add(x));
				return children;
			}			
		}
		FrameworkElement INavigationSupport.GetParent() { return OwnerView; }
		bool INavigationSupport.GetSkipNavigation() {
			return 
				!IsCategory 
				&& (HasActiveEditor || !OwnerView.CellEditorOwner.EditorWasClosed
					|| OwnerView.PropertyGrid.NavigationManager.OpenEditorOnSelection
					|| (OwnerView.SelectedItem as RowDataBase).With(x=>OwnerView.GetRowControl(x)).If(x=>x.NavigatingInsideHeader).ReturnSuccess())
				|| HasCustomTemplate;
		}
		bool INavigationSupport.GetUseLinearNavigation() { return !((INavigationSupport)this).GetSkipNavigation() || !HasCustomTemplate; }
		#endregion
	}   
}
