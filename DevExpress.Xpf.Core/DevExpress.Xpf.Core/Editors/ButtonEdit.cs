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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Collections;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Markup;
using System.Windows.Automation.Peers;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Automation;
using DevExpress.Xpf.Core;
using System.ComponentModel;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Editors.EditStrategy;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Utils;
using System.Windows.Data;
using DevExpress.Xpf.Editors.Helpers;
using System.Windows.Media;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Bars.Internal;
namespace DevExpress.Xpf.Editors {
	[DXToolboxBrowsable(DXToolboxItemKind.Free)]
	[ContentProperty("Buttons")]
	public partial class ButtonEdit : TextEdit {
		#region static
		public static readonly DependencyProperty ShowEditorButtonsProperty;
		public static readonly DependencyProperty IsTextEditableProperty;
		public static readonly DependencyProperty AllowDefaultButtonProperty;
		public static readonly DependencyProperty ButtonsProperty;
		protected static readonly DependencyPropertyKey LeftButtonsPropertyKey;
		public static readonly DependencyProperty LeftButtonsProperty;
		protected static readonly DependencyPropertyKey RightButtonsPropertyKey;
		public static readonly DependencyProperty RightButtonsProperty;
		protected static readonly DependencyPropertyKey SortedButtonsPropertyKey;
		public static readonly DependencyProperty SortedButtonsProperty;
		public static readonly DependencyProperty ShowTextProperty;
		public static readonly DependencyProperty NullValueButtonPlacementProperty;
		public static readonly DependencyProperty ButtonsSourceProperty;
		public static readonly DependencyProperty ButtonTemplateProperty;
		public static readonly DependencyProperty ButtonTemplateSelectorProperty;
		public static readonly RoutedEvent DefaultButtonClickEvent;
		static ButtonEdit() {
			Type ownerType = typeof(ButtonEdit);
			ShowEditorButtonsProperty = DependencyPropertyManager.Register("ShowEditorButtons", typeof(bool), ownerType, new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.None, OnShowEditButtonsChanged));
			DefaultButtonClickEvent = EventManager.RegisterRoutedEvent("DefaultButtonClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), ownerType);
			AllowDefaultButtonProperty = DependencyPropertyManager.Register("AllowDefaultButton", typeof(bool?), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, OnAllowDefaultButtonChanged));
			ShowTextProperty = DependencyPropertyManager.Register("ShowText", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			IsTextEditableProperty = DependencyPropertyManager.Register("IsTextEditable", typeof(bool?), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, OnIsTextEditableChanged));
			ButtonsProperty = DependencyPropertyManager.Register("Buttons", typeof(ButtonInfoCollection), ownerType, new FrameworkPropertyMetadata(null, OnButtonsChanged));
			LeftButtonsPropertyKey = DependencyPropertyManager.RegisterReadOnly("LeftButtons", typeof(IEnumerable<ButtonInfoBase>), ownerType, new FrameworkPropertyMetadata(null));
			LeftButtonsProperty = LeftButtonsPropertyKey.DependencyProperty;
			RightButtonsPropertyKey = DependencyPropertyManager.RegisterReadOnly("RightButtons", typeof(IEnumerable<ButtonInfoBase>), ownerType, new FrameworkPropertyMetadata(null));
			RightButtonsProperty = RightButtonsPropertyKey.DependencyProperty;
			SortedButtonsPropertyKey = DependencyPropertyManager.RegisterReadOnly("SortedButtons", typeof(IEnumerable<ButtonInfoBase>), ownerType, new FrameworkPropertyMetadata(null));
			SortedButtonsProperty = SortedButtonsPropertyKey.DependencyProperty;
			NullValueButtonPlacementProperty = DependencyPropertyManager.Register("NullValueButtonPlacement", typeof(EditorPlacement?), ownerType,
				new FrameworkPropertyMetadata(null, (d, e) => ((ButtonEdit)d).NullValueButtonPlacementChanged((EditorPlacement?)e.NewValue)));
#if !SL
			DefaultStyleKeyProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(ownerType));
			IsEnabledProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(true, OnIsEnabledChanged));
			ButtonsSourceProperty = DependencyPropertyManager.Register("ButtonsSource", typeof(IEnumerable), ownerType, 
				new FrameworkPropertyMetadata(null, (d,e) => ((ButtonEdit)d).OnButtonsSourceChanged()));
			ButtonTemplateProperty = DependencyPropertyManager.Register("ButtonTemplate", typeof(DataTemplate), ownerType,
				new FrameworkPropertyMetadata(null, (d, e) => ((ButtonEdit)d).OnButtonTemplateChanged()));
			ButtonTemplateSelectorProperty = DependencyPropertyManager.Register("ButtonTemplateSelector", typeof(DataTemplateSelector), ownerType,
				new FrameworkPropertyMetadata(null, (d, e) => ((ButtonEdit)d).OnButtonTemplateSelectorChanged()));
#else
			ButtonStyleProperty = DependencyPropertyManager.Register("ButtonStyle", typeof(Style), typeof(ButtonEdit), new PropertyMetadata(null,
				new PropertyChangedCallback((d, e) => ((ButtonEdit)d).PropertyChangedButtonStyle((Style)e.OldValue))));
#endif
		}
		static void OnIsTextEditableChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((ButtonEdit)obj).OnIsTextEditableChanged();
		}
		static void OnButtonsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((ButtonEdit)obj).OnButtonsChanged((ButtonInfoCollection)e.OldValue, (ButtonInfoCollection)e.NewValue);
		}
		static void OnAllowDefaultButtonChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((ButtonEdit)obj).OnAllowDefaultButtonChanged();
		}
		protected static void OnShowEditButtonsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((ButtonEdit)obj).OnShowEditButtonsChanged((bool)e.OldValue);
		}
#if !SL
		static void OnIsEnabledChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((ButtonEdit)obj).OnIsEnabledChanged();
		}
#endif
		#endregion
#if !SL
		ButtonsControl LeftButtonsControl { get; set; }
		ButtonsControl RightButtonsControl { get; set; }
#endif
		protected void EnsureButtons() {
			this.SetCurrentValue(ButtonsProperty, CreateButtonCollection());
			this.actualButtons = new List<ButtonInfoBase>();
		}
		ButtonInfoCollection CreateButtonCollection() {
			return new ButtonInfoCollection();
		}
		public ButtonEdit() {
			ButtonsUpdateLocker = new Locker();
			ButtonsUpdateLocker.DoLockedAction(EnsureButtons);
		}
		#region overrrides
		protected override AutomationPeer OnCreateAutomationPeer() {
			return new ButtonEditAutomationPeer(this);
		}
		protected override ControlTemplate GetActualEditorControlTemplate() {
			if (PropertyProvider.IsTextEditable)
				return base.GetActualEditorControlTemplate();
			else
				return EditNonEditableTemplate;
		}
		protected override IEnumerator LogicalChildren {
			get {
				List<object> children = new List<object>();
				if (base.LogicalChildren != null) {
					IEnumerator enumerator = base.LogicalChildren;
					while (enumerator.MoveNext())
						children.Add(enumerator.Current);
				}
				foreach (ButtonInfoBase info in ActualButtons)
					children.Add(info);
				return children.GetEnumerator();
			}
		}
		#endregion
		Locker ButtonsUpdateLocker { get; set; }
		[Category(EditSettingsCategories.Behavior)]
		public EditorPlacement? NullValueButtonPlacement {
			get { return (EditorPlacement?)GetValue(NullValueButtonPlacementProperty); }
			set { SetValue(NullValueButtonPlacementProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("ButtonEditShowEditorButtons"),
#endif
 Category("Behavior")]
		public bool ShowEditorButtons {
			get { return (bool)GetValue(ShowEditorButtonsProperty); }
			set { SetValue(ShowEditorButtonsProperty, value); }
		}
		[Category("Action")]
		public event RoutedEventHandler DefaultButtonClick {
			add { this.AddHandler(DefaultButtonClickEvent, value); }
			remove { this.RemoveHandler(DefaultButtonClickEvent, value); }
		}
		void RaiseDefaultButtonClick() {
			this.RaiseEvent(new RoutedEventArgs(DefaultButtonClickEvent));
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("ButtonEditShowText"),
#endif
 Category("Behavior")]
		public bool ShowText {
			get { return (bool)GetValue(ShowTextProperty); }
			set { SetValue(ShowTextProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("ButtonEditAllowDefaultButton"),
#endif
 Category("Behavior")]
		public bool? AllowDefaultButton {
			get { return (bool?)GetValue(AllowDefaultButtonProperty); }
			set { SetValue(AllowDefaultButtonProperty, value); }
		}
		[
		DefaultValue(true),
		Category("Behavior")]
		public bool? IsTextEditable {
			get { return (bool?)GetValue(IsTextEditableProperty); }
			set { SetValue(IsTextEditableProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("ButtonEditButtons"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Category("Common Properties")]
		public ButtonInfoCollection Buttons {
			get { return (ButtonInfoCollection)GetValue(ButtonsProperty); }
			set { SetValue(ButtonsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("ButtonEditLeftButtons"),
#endif
 Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IEnumerable<ButtonInfoBase> LeftButtons {
			get { return (IEnumerable<ButtonInfoBase>)GetValue(LeftButtonsProperty); }
			protected set { this.SetValue(LeftButtonsPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("ButtonEditRightButtons"),
#endif
 Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IEnumerable<ButtonInfoBase> RightButtons {
			get { return (IEnumerable<ButtonInfoBase>)GetValue(RightButtonsProperty); }
			protected set { this.SetValue(RightButtonsPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("ButtonEditSortedButtons"),
#endif
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IEnumerable<ButtonInfoBase> SortedButtons {
			get { return (IEnumerable<ButtonInfoBase>)GetValue(SortedButtonsProperty); }
			protected set { this.SetValue(SortedButtonsPropertyKey, value); }
		}
		public IEnumerable ButtonsSource {
			get { return (IEnumerable)GetValue(ButtonsSourceProperty); }
			set { SetValue(ButtonsSourceProperty, value); }
		}
		public DataTemplate ButtonTemplate {
			get { return (DataTemplate)GetValue(ButtonTemplateProperty); }
			set { SetValue(ButtonTemplateProperty, value); }
		}
		public DataTemplateSelector ButtonTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ButtonTemplateSelectorProperty); }
			set { SetValue(ButtonTemplateSelectorProperty, value); }
		}
		List<ButtonInfoBase> ButtonsSourceButtons { get; set; }
		protected internal override Type StyleSettingsType { get { return typeof(ButtonEditStyleSettings); } }
		new ButtonEditPropertyProvider PropertyProvider { get { return base.PropertyProvider as ButtonEditPropertyProvider; } }
		protected internal new ButtonEditSettings Settings { get { return (ButtonEditSettings)base.Settings; } }
		ButtonInfoBase GetDefaultButtonInfo(IEnumerable<ButtonInfoBase> buttons) {
			return buttons.FirstOrDefault(info => info.IsDefaultButton);
		}
		protected virtual ButtonInfoBase CreateNullValueButtonInfo() {
			return Settings.CreateNullValueButtonInfo();
		}
		protected ButtonInfoBase CreateDefaultButtonInfo() {
			return Settings.CreateDefaultButtonInfo();
		}
		protected internal virtual void OnDefaultButtonClick(object sender, System.Windows.RoutedEventArgs e) {
			RaiseDefaultButtonClick();
			Settings.RaiseDefaultButtonClick(this, e);
		}
#if !SL
		protected internal virtual void OnDefaultRenderButtonClick(IFrameworkRenderElementContext sender, RenderEventArgsBase args) {
			RaiseDefaultButtonClick();
			Settings.RaiseDefaultButtonClick(this, args.OriginalEventArgs as RoutedEventArgs);
		}
#endif
		void SubscribeButtonCollection(ButtonInfoCollection newButtons) {
			if (newButtons != null)
				newButtons.CollectionChanged += ButtonsCollectionChanged;
		}
		void UnsubscribeButtonCollection(ButtonInfoCollection oldButtons) {
			if (oldButtons != null)
				oldButtons.CollectionChanged -= ButtonsCollectionChanged;
		}
		IList<ButtonInfoBase> actualButtons;
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("ButtonEditRightButtons"),
#endif
 Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IList<ButtonInfoBase> ActualButtons {
			get { return actualButtons; }
		}
		IList<ButtonInfoBase> GetActualButtons() {
			IList<ButtonInfoBase> result = new List<ButtonInfoBase>();
			InsertDefaultButtonInfo(result);
			InsertCommandButtonInfo(result);
			if (Buttons != null)
				foreach (ButtonInfoBase info in Buttons)
					result.Add(info);
			if(ButtonsSource != null){
				foreach (var button in ButtonsSourceButtons) {
					result.Add(button);
				}
			}
			result =  result.OrderBy(x => x.Index).ToList();
			return new ReadOnlyCollection<ButtonInfoBase>(result);
		}
		void UpdateButtonsSourceButtons() {
			if (ButtonsSource == null) return;
			ButtonsSourceButtons = Settings.CreateButtonsSourceButtons(ButtonsSource, ButtonTemplate, ButtonTemplateSelector);
		}
		void UpdateActualButtons() {
			ProcessOldButtons((IList)ActualButtons);
			actualButtons = GetActualButtons();
			ProcessNewButtons((IList)ActualButtons);
			ActualButtonsChanged();
		}
		protected virtual void ActualButtonsChanged() {
		}
		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);
			UpdateButtonInfoCollections();
			PropertyProvider.SetIsTextEditable(this);
		}
		protected virtual void OnAllowDefaultButtonChanged() {
			UpdateButtonInfoCollections();
		}
		protected virtual void OnButtonsSourceChanged() {
			UpdateButtonsSourceButtons();
			UpdateButtonInfoCollections();
		}
		protected virtual void OnButtonTemplateChanged() {
			UpdateButtonsSourceButtons();
			UpdateButtonInfoCollections();
		}
		protected virtual void OnButtonTemplateSelectorChanged() {
			UpdateButtonsSourceButtons();
			UpdateButtonInfoCollections();
		}
		internal void InsertDefaultButtonInfo(IList<ButtonInfoBase> collection) {
			if (GetDefaultButtonInfo(collection) == null && PropertyProvider.GetActualAllowDefaultButton(this)) {
				var bi = CreateDefaultButtonInfo();
				collection.Insert(0, bi);
			}
		}
		protected virtual void InsertCommandButtonInfo(IList<ButtonInfoBase> collection) {
			if (PropertyProvider.GetNullValueButtonPlacement() == EditorPlacement.EditBox) {
				var bi = CreateNullValueButtonInfo();
				UpdateNullValueButtonInfo(bi);
				collection.Insert(0, bi);
			}
		}
		void UpdateNullValueButtonInfo(ButtonInfoBase bi) {
			(bi as ButtonInfo).Do(x => x.Command = SetNullValueCommand);
		}
#if !SL
		protected virtual void OnIsEnabledChanged() {
			foreach (ButtonInfoBase info in ActualButtons)
				info.CoerceValue(ContentElement.IsEnabledProperty);
		}
#endif
		protected virtual List<DependencyProperty> SpinCommandsAffectingProperties {
			get {
				List<DependencyProperty> affectingProperties = new List<DependencyProperty>();
				affectingProperties.Add(BaseEdit.EditModeProperty);
				affectingProperties.Add(BaseEdit.IsReadOnlyProperty);
				affectingProperties.Add(BaseEdit.IsEnabledProperty);
				return affectingProperties;
			}
		}
		protected override void UpdateCommands(DependencyProperty property) {
			base.UpdateCommands(property);
			if (SpinCommandsAffectingProperties.Contains(property))
				UpdateSpinCommands();
			if (property == IsReadOnlyProperty)
				UpdateSetNullValueCommand();
		}
		protected virtual void UpdateSetNullValueCommand() {
			UpdateCommand(SetNullValueCommand);
		}
		protected virtual void UpdateSpinCommands() {
			UpdateCommand(SpinUpCommand);
			UpdateCommand(SpinDownCommand);
		}
		protected virtual void NullValueButtonPlacementChanged(EditorPlacement? newValue) {
			UpdateButtonInfoCollections();
		}
		protected virtual void OnIsTextEditableChanged() {
			Settings.IsTextEditable = IsTextEditable;
			PropertyProvider.SetIsTextEditable(this);
			UpdateActualEditorControlTemplate();
		}
		protected override ControlTemplate GetEditTemplate() {
			return PropertyProvider.IsTextEditable ? base.GetEditTemplate() : EditNonEditableTemplate;
		}
		protected override bool IsTextBlockModeCore() {
			return !PropertyProvider.IsTextEditable || base.IsTextBlockModeCore();
		}
		protected override void OnEditModeChanged(EditMode oldValue, EditMode newValue) {
			base.OnEditModeChanged(oldValue, newValue);
#if !SL
			foreach (var button in ActualButtons) {
				button.UpdateOnEditModeChanged();
			}
#endif
		}
#if !SL
		internal override void UpdateButtonPanelsInplaceMode() {
			if (ShowEditorButtons) {
				if (LeftButtonsControl == null) {
					LeftButtonsControl = new ButtonsControl();
					AddVisualChild(LeftButtonsControl);
					additionalInplaceModeElements.Add(LeftButtonsControl);
					DockPanel.SetDock(LeftButtonsControl, Dock.Left);
					LeftButtonsControl.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(LeftButtonsProperty.Name) { Source = this });
					InvalidateMeasure();
				}
				if (RightButtonsControl == null) {
					RightButtonsControl = new ButtonsControl();
					AddVisualChild(RightButtonsControl);
					additionalInplaceModeElements.Add(RightButtonsControl);
					DockPanel.SetDock(RightButtonsControl, Dock.Right);
					RightButtonsControl.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(RightButtonsProperty.Name) { Source = this });
					InvalidateMeasure();
				}
			}
			else {
				if (LeftButtonsControl != null) {
					additionalInplaceModeElements.Remove(LeftButtonsControl);
					RemoveVisualChild(LeftButtonsControl);
					LeftButtonsControl = null;
					InvalidateMeasure();
				}
				if (RightButtonsControl != null) {
					additionalInplaceModeElements.Remove(RightButtonsControl);
					RemoveVisualChild(RightButtonsControl);
					RightButtonsControl = null;
					InvalidateMeasure();
				}
			}
		}
#endif
		protected override EditStrategyBase CreateEditStrategy() {
			return new TextEditStrategy(this);
		}
		protected override ActualPropertyProvider CreateActualPropertyProvider() {
			return new ButtonEditPropertyProvider(this);
		}
		protected virtual void OnShowEditButtonsChanged(bool oldValue) {
			UpdateShowEditorButtons();
			ContentManagementStrategy.UpdateButtonPanels();
		}
		private void UpdateShowEditorButtons() {
			PropertyProvider.ShowLeftButtons = ShowEditorButtons && LeftButtons != null && ((IList)LeftButtons).Count > 0;
			PropertyProvider.ShowRightButtons = ShowEditorButtons && RightButtons != null && ((IList)RightButtons).Count > 0;
		}
		protected virtual void OnButtonsChanged(ButtonInfoCollection oldButtons, ButtonInfoCollection newButtons) {
			UnsubscribeButtonCollection(oldButtons);
			SubscribeButtonCollection(newButtons);
			UpdateButtonInfoCollections();
		}
		void ButtonsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			UpdateButtonInfoCollections();
		}
		void ProcessOldButtons(IList items) {
			if (items != null)
				foreach (ButtonInfoBase info in items)
					ProcessOldButton(info);
		}
		void ProcessNewButtons(IList items) {
			if (items != null)
				foreach (ButtonInfoBase info in items)
					ProcessNewButton(info);
		}
		void ProcessOldButton(ButtonInfoBase info) {
#if !SL
			if (IsInitialized)
				RemoveLogicalChild(info);
#else
			SetButtonInfoOwner(info, null);
			info.ClearValue(FrameworkElement.DataContextProperty);
#endif
		}
		void ProcessNewButton(ButtonInfoBase info) {
#if !SL
			if (IsInitialized) {
				AddLogicalChild(info);
				info.UpdateActualMargin();
			}
#else
			SetButtonInfoOwner(info, this);
			if (info.Style == null && info.GetBindingExpression(ButtonInfoBase.StyleProperty) == null)
				info.Style = ButtonStyle;
			SetBindingToOwnerDataContext(info);
			ProcessNewButtonInternal(info);
#endif
		}
		protected virtual void ProcessNewButtonInternal(ButtonInfoBase info) {
		}
		void SetBindingToOwnerDataContext(ButtonInfoBase info) {
			System.Windows.Data.BindingOperations.SetBinding(info, FrameworkElement.DataContextProperty, new System.Windows.Data.Binding("DataContext") { Source = this });
		}
		internal void UpdateButtonInfoCollections() {
			ButtonsUpdateLocker.DoLockedActionIfNotLocked(UpdateButtonInfoCollectionsInternal);
		}
		void UpdateButtonInfoCollectionsInternal() {
			UpdateActualButtons();
			UpdateLeftAndRightButtons();
			UpdateSortedButtons();
			UpdateShowEditorButtons();
		}
		void UpdateSortedButtons() {
			var sortedButtons = new ReadOnlyItemsSource<ButtonInfoBase>();
			foreach (ButtonInfoBase info in LeftButtons)
				sortedButtons.Add(info);
			foreach (ButtonInfoBase info in RightButtons)
				sortedButtons.Add(info);
			SortedButtons = sortedButtons;
		}
		void UpdateLeftAndRightButtons() {
			var leftButtons = new ReadOnlyItemsSource<ButtonInfoBase>();
			var rightButtons = new ReadOnlyItemsSource<ButtonInfoBase>();
			foreach (ButtonInfoBase info in ActualButtons) {
				if (info.IsLeft)
					leftButtons.Add(info);
				else
					rightButtons.Add(info);
			}
			LeftButtons = leftButtons;
			RightButtons = rightButtons;
		}
		protected internal override bool GetShowEditorButtons() {
			return ShowEditorButtons;
		}
		protected internal override void SetShowEditorButtons(bool show) {
			ShowEditorButtons = show;
		}
	}
	public enum EditorPlacement {
		None,
		EditBox,
		Popup
	}
	public enum GlyphKind {
		None,
		User,
		Custom,
		DropDown,
		Regular,
		Right,
		Left,
		Up,
		Down,
		Cancel,
		Apply,
		Plus,
		Minus,
		Redo,
		Undo,
		Refresh,
		Search,
		NextPage,
		PrevPage,
		Last,
		First,
		Edit
	}
	public enum ButtonKind {
		Simple,
		Repeat,
		Toggle
	}
	internal class ReadOnlyItemsSource<T> : List<T>,
		INotifyPropertyChanged, INotifyCollectionChanged { 
		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged { add { } remove { } }
		event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged { add { } remove { } }
	}
	public abstract partial class ButtonInfoBase : FrameworkContentElement, ICloneable, ILogicalOwner, ILogicalChildrenContainerProvider {
		#region static
		public static readonly DependencyProperty IsDefaultButtonProperty;
		public static readonly DependencyProperty IsLeftProperty;
		public static readonly DependencyProperty TemplateProperty;
		public static readonly DependencyProperty ClickModeProperty;
		public static readonly DependencyProperty ForegroundProperty;
		public static readonly DependencyProperty MarginProperty;
		static readonly DependencyPropertyKey ActualMarginPropertyKey;
		public static readonly DependencyProperty ActualMarginProperty;
		public static readonly DependencyProperty MarginCorrectionProperty;
		public static readonly DependencyProperty VisibilityProperty;
		public static new readonly DependencyProperty IsMouseOverProperty;
		public static readonly DependencyProperty IsPressedProperty;
		public static readonly DependencyProperty IndexProperty;
		public static readonly DependencyProperty RenderTemplateProperty;
		public static readonly DependencyProperty RaiseClickEventInInplaceInactiveModeProperty;
		static ButtonInfoBase() {
			Type ownerType = typeof(ButtonInfoBase);
			TemplateProperty = DependencyPropertyManager.Register("Template", typeof(DataTemplate), ownerType, new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnTemplateChanged)));
			IsDefaultButtonProperty = DependencyPropertyManager.Register("IsDefaultButton", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
			IsLeftProperty = DependencyPropertyManager.Register("IsLeft", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, OnIsLeftChanged));
			ClickModeProperty = DependencyPropertyManager.Register("ClickMode", typeof(ClickMode), ownerType, new FrameworkPropertyMetadata(ClickMode.Press));
			ForegroundProperty = DependencyPropertyManager.Register("Foreground", typeof(Brush), ownerType, new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Black)));
			MarginCorrectionProperty = DependencyPropertyManager.Register("MarginCorrection", typeof(Thickness), ownerType,
				new FrameworkPropertyMetadata(new Thickness(-1), (o, args) => ((ButtonInfoBase)o).MarginCorrectionChanged((Thickness)args.NewValue)));
			MarginProperty = DependencyPropertyManager.Register("Margin", typeof(Thickness), ownerType,
				new FrameworkPropertyMetadata(new Thickness(0), (o, args) => ((ButtonInfoBase)o).MarginChanged((Thickness)args.NewValue)));
			ActualMarginPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualMargin", typeof(Thickness), ownerType, new FrameworkPropertyMetadata(new Thickness(0)));
			ActualMarginProperty = ActualMarginPropertyKey.DependencyProperty;
			RaiseClickEventInInplaceInactiveModeProperty = DependencyPropertyManager.Register("RaiseClickEventInInplaceInactiveMode", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			RenderTemplateProperty = DependencyPropertyManager.Register("RenderTemplate", typeof(RenderTemplate), ownerType, new FrameworkPropertyMetadata(null));
			VisibilityProperty = DependencyPropertyManager.Register("Visibility", typeof(Visibility), ownerType, new FrameworkPropertyMetadata(Visibility.Visible));
			IsMouseOverProperty = DependencyPropertyManager.Register("IsMouseOver", typeof(bool), ownerType, new PropertyMetadata(false));
			IsPressedProperty = DependencyPropertyManager.Register("IsPressed", typeof(bool), ownerType, new PropertyMetadata(false));
			IndexProperty = DependencyPropertyManager.Register("Index", typeof(int), ownerType, 
				new FrameworkPropertyMetadata(0, (d,e)=>((ButtonInfoBase)d).OnIndexChanged()));
		}
		static void OnTemplateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((ButtonInfoBase)obj).OnTemplateChanged();
		}
		static void OnIsLeftChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((ButtonInfoBase)obj).OnIsLeftChanged();
		}
		#endregion
		internal EventHandlerList events;
		protected internal LogicalChildrenContainer LCContainer { get; private set; }
		protected override IEnumerator LogicalChildren { get { return new MergedEnumerator(base.LogicalChildren, this.LCContainer.GetEnumerator()); } }
		protected ButtonInfoBase() {
			this.events = new EventHandlerList();
			this.LCContainer = new LogicalChildrenContainer(this);
		}
		public int Index {
			get { return (int)GetValue(IndexProperty); }
			set { SetValue(IndexProperty, value); }
		}
		public Thickness ActualMargin {
			get { return (Thickness)GetValue(ActualMarginProperty); }
			private set { this.SetValue(ActualMarginPropertyKey, value); }
		}
		public Thickness Margin {
			get { return (Thickness)GetValue(MarginProperty); }
			set { SetValue(MarginProperty, value); }
		}
		public Thickness MarginCorrection {
			get { return (Thickness)GetValue(MarginCorrectionProperty); }
			set { SetValue(MarginCorrectionProperty, value); }
		}
		protected ButtonEdit Owner { get { return LogicalTreeHelper.GetParent(this) as ButtonEdit; } }
		public new bool IsMouseOver {
			get { return (bool)GetValue(IsMouseOverProperty); }
			set { SetValue(IsMouseOverProperty, value); }
		}
		public bool IsPressed {
			get { return (bool)GetValue(IsPressedProperty); }
			set { SetValue(IsPressedProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("ButtonInfoBaseVisibility"),
#endif
 Category("Behavior")]
		public Visibility Visibility {
			get { return (Visibility)GetValue(VisibilityProperty); }
			set { SetValue(VisibilityProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("ButtonInfoBaseIsDefaultButton"),
#endif
 Category("Behavior")]
		public bool IsDefaultButton {
			get { return (bool)GetValue(IsDefaultButtonProperty); }
			set { this.SetValue(IsDefaultButtonProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("ButtonInfoBaseIsLeft"),
#endif
 Category("Layout")]
		public bool IsLeft {
			get { return (bool)GetValue(IsLeftProperty); }
			set { SetValue(IsLeftProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("ButtonInfoBaseTemplate"),
#endif
 Browsable(false)]
		public DataTemplate Template {
			get { return (DataTemplate)GetValue(TemplateProperty); }
			set { SetValue(TemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("ButtonInfoBaseRenderTemplate"),
#endif
 Browsable(false)]
		public RenderTemplate RenderTemplate {
			get { return (RenderTemplate)GetValue(RenderTemplateProperty); }
			set { SetValue(RenderTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("ButtonInfoBaseRaiseClickEventInInplaceInactiveMode"),
#endif
 Browsable(false)]
		public bool RaiseClickEventInInplaceInactiveMode {
			get { return (bool)GetValue(RaiseClickEventInInplaceInactiveModeProperty); }
			set { SetValue(RaiseClickEventInInplaceInactiveModeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("ButtonInfoBaseClickMode"),
#endif
 Category("Behavior")]
		public ClickMode ClickMode {
			get { return (ClickMode)GetValue(ClickModeProperty); }
			set { SetValue(ClickModeProperty, value); }
		}
		public Brush Foreground {
			get { return (Brush)GetValue(ForegroundProperty); }
			set { SetValue(ForegroundProperty, value); }
		}
		internal protected virtual void Subscribe(FrameworkElement element) {
		}
		internal protected virtual void SubscribeAsDefault(FrameworkElement element) {
		}
		internal protected virtual void FindContent(FrameworkElement templatedParent) {
		}
		internal protected virtual void FindContent(RenderContentControlContext templatedParent) {
		}
		internal void UpdateActualMargin() {
			ActualMargin = CalcActualMargin(Margin, MarginCorrection, IsLeft);
		}
		protected virtual void OnIndexChanged() {
			if (Owner != null)
				Owner.UpdateButtonInfoCollections();
		}
		public void UpdateOnEditModeChanged() {
			UpdateActualMargin();
		}
		protected virtual void MarginCorrectionChanged(Thickness newValue) {
			UpdateActualMargin();
		}
		protected virtual void MarginChanged(Thickness newValue) {
			UpdateActualMargin();
		}
		Thickness CalcActualMargin(Thickness thickness, Thickness marginCorrection, bool isLeft) {
			Thickness newValue = thickness;
			if (Owner.Return(x => x.EditMode != EditMode.Standalone, () => false)) {
				return CalcActualMarginInternal(thickness, marginCorrection, isLeft);
			}
			return newValue;
		}
		Thickness CalcActualMarginInternal(Thickness thickness, Thickness marginCorrection, bool isLeft) {
			Thickness newValue = thickness;
			Thickness delta = marginCorrection;
			ThicknessHelper.Inc(ref newValue, delta);
			return newValue;
		}
		internal Thickness CalcRenderActualMargin() {
			return CalcActualMarginInternal(Margin, MarginCorrection, IsLeft);
		}
		protected virtual void OnTemplateChanged() {
		}
		protected virtual void OnIsLeftChanged() {
			if (Owner != null)
				Owner.UpdateButtonInfoCollections();
			UpdateActualMargin();
		}
		protected abstract ButtonInfoBase CreateClone();
		protected void AssignToClone(ButtonInfoBase clone) {
			AssignToCloneInternal(clone);
			foreach (DependencyProperty property in CloneProperties)
				AssignValueToClone(clone, property);
		}
		protected virtual void AssignToCloneInternal(ButtonInfoBase clone) {
			clone.events.AddHandlers(events);
		}
		protected bool ShouldAssignToClone(DependencyProperty property) {
			return this.IsPropertySet(property);
		}
		protected void AssignValueToClone(ButtonInfoBase clone, DependencyProperty property) {
			if (ShouldAssignToClone(property))
				clone.SetValue(property, this.GetValue(property));
		}
		#region ICloneable Members
		IEnumerable<DependencyProperty> cloneProperties;
		internal IEnumerable<DependencyProperty> CloneProperties {
			get {
				if (cloneProperties == null)
					cloneProperties = CreateCloneProperties();
				return cloneProperties;
			}
		}
		protected virtual List<DependencyProperty> CreateCloneProperties() {
			List<DependencyProperty> list = new List<DependencyProperty>();
			list.Add(IsDefaultButtonProperty);
			list.Add(ClickModeProperty);
			list.Add(IsLeftProperty);
			list.Add(RenderTemplateProperty);
			list.Add(RaiseClickEventInInplaceInactiveModeProperty);
			list.Add(IndexProperty);
			return list;
		}
		object ICloneable.Clone() {
			ButtonInfoBase clone = CreateClone();
			AssignToClone(clone);
			return clone;
		}
		#endregion
		protected internal bool IsClone(ButtonInfoBase clone) {
			Type xType = GetType();
			Type yType = clone.GetType();
			if (xType != yType)
				return false;
			if (!IsCloneInternal(clone))
				return false;
			return CloneProperties.TrueForEach(p => !this.IsPropertyAssigned(p) && !clone.IsPropertyAssigned(p) || Equals(this.GetValue(p), clone.GetValue(p)));
		}
		protected virtual bool IsCloneInternal(ButtonInfoBase clone) {
			return true;
		}
		double ILogicalOwner.ActualWidth {
			get { return 0; }
		}
		double ILogicalOwner.ActualHeight {
			get { return 0; }
		}
		void ILogicalOwner.AddChild(object child) {
			AddLogicalChild(child);
		}
		void ILogicalOwner.RemoveChild(object child) {
			RemoveLogicalChild(child);
		}
		ILogicalChildrenContainer2 ILogicalChildrenContainerProvider.LogicalChildrenContainer { get { return LCContainer; } }
	}
	[ContentProperty("Content")]
	public partial class ButtonInfo : ButtonInfoBase {
		internal const string TemplateChildName = "PART_Item";
		#region static
		internal static readonly object click = new object();
		public static readonly DependencyProperty GlyphKindProperty;
		public static readonly DependencyProperty ContentProperty;
		public static readonly DependencyProperty ContentTemplateProperty;
		public static readonly DependencyProperty IsCheckedProperty;
		public static readonly DependencyProperty CommandParameterProperty;
		public static readonly DependencyProperty CommandProperty;
		public static readonly DependencyProperty CommandTargetProperty;
		public static readonly DependencyProperty ContentRenderTemplateProperty;
		public static readonly DependencyProperty ButtonKindProperty;
		static ButtonInfo() {
			Type ownerType = typeof(ButtonInfo);
			GlyphKindProperty = DependencyPropertyManager.Register("GlyphKind", typeof(GlyphKind), ownerType, new FrameworkPropertyMetadata(GlyphKind.None, FrameworkPropertyMetadataOptions.AffectsMeasure));
			ContentProperty = DependencyPropertyManager.Register("Content", typeof(object), ownerType, new FrameworkPropertyMetadata(null));
			ContentTemplateProperty = DependencyPropertyManager.Register("ContentTemplate", typeof(DataTemplate), ownerType, new FrameworkPropertyMetadata(null));
			IsCheckedProperty = DependencyPropertyManager.Register("IsChecked", typeof(bool?), ownerType, new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
			ButtonKindProperty = DependencyPropertyManager.Register("ButtonKind", typeof(ButtonKind), ownerType, new FrameworkPropertyMetadata(ButtonKind.Simple));
			CommandProperty = DependencyPropertyManager.Register("Command", typeof(ICommand), ownerType, new FrameworkPropertyMetadata(null));
			CommandParameterProperty = DependencyPropertyManager.Register("CommandParameter", typeof(object), ownerType, new FrameworkPropertyMetadata(null));
			ContentRenderTemplateProperty = DependencyPropertyManager.Register("ContentRenderTemplate", typeof(RenderTemplate), ownerType, new FrameworkPropertyMetadata(null));
			DefaultStyleKeyProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(ownerType));
			CommandTargetProperty = DependencyPropertyManager.Register("CommandTarget", typeof(IInputElement), ownerType, new FrameworkPropertyMetadata(null));
		}
		#endregion
		[Category("Behavior")]
		public event RoutedEventHandler Click {
			add { events.AddHandler(click, value); }
			remove { events.RemoveHandler(click, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("ButtonInfoGlyphKind"),
#endif
 Category("Appearance")]
		public GlyphKind GlyphKind {
			get { return (GlyphKind)GetValue(GlyphKindProperty); }
			set { SetValue(GlyphKindProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("ButtonInfoContent"),
#endif
 Category("Content"), TypeConverter(typeof(ObjectConverter))]
		public object Content {
			get { return GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("ButtonInfoContentTemplate"),
#endif
 Browsable(false)]
		public DataTemplate ContentTemplate {
			get { return (DataTemplate)GetValue(ContentTemplateProperty); }
			set { SetValue(ContentTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("ButtonInfoIsChecked"),
#endif
 TypeConverter(typeof(NullableBoolConverter)), Category("Common Properties")]
		public bool? IsChecked {
			get { return (bool?)GetValue(IsCheckedProperty); }
			set { SetValue(IsCheckedProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("ButtonInfoCommand"),
#endif
 Category("Action"), Localizability(LocalizationCategory.NeverLocalize)]
		public ICommand Command {
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("ButtonInfoCommandParameter"),
#endif
 Category("Action"), Localizability(LocalizationCategory.NeverLocalize)]
		public object CommandParameter {
			get { return GetValue(CommandParameterProperty); }
			set { SetValue(CommandParameterProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("ButtonInfoCommandTarget"),
#endif
 Category("Action")]
		public IInputElement CommandTarget {
			get { return (IInputElement)GetValue(CommandTargetProperty); }
			set { SetValue(CommandTargetProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("ButtonInfoContentRenderTemplate"),
#endif
 Browsable(false)]
		public RenderTemplate ContentRenderTemplate {
			get { return (RenderTemplate)GetValue(ContentRenderTemplateProperty); }
			set { SetValue(ContentRenderTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("ButtonInfoButtonKind"),
#endif
 Category("Behavior")]
		public ButtonKind ButtonKind {
			get { return (ButtonKind)GetValue(ButtonKindProperty); }
			set { SetValue(ButtonKindProperty, value); }
		}
		internal protected virtual void RaiseClickEvent(object sender, System.Windows.RoutedEventArgs e) {
			RoutedEventHandler handler = events[click] as RoutedEventHandler;
			if (handler != null)
				handler(sender, e);
		}
		protected virtual void OnButtonClick(object sender, System.Windows.RoutedEventArgs e) {
			if (ShouldRaiseClickEvent())
				RaiseClickEvent(sender, e);
		}
		bool ShouldRaiseClickEvent() {
			return Owner == null || !Owner.If(x => x.EditMode == EditMode.InplaceInactive && !RaiseClickEventInInplaceInactiveMode).ReturnSuccess();
		}
		protected virtual void OnRenderButtonClick(IFrameworkRenderElementContext sender, RenderEventArgsBase args) {
			if (!RaiseClickEventInInplaceInactiveMode)
				return;
			if (IsDefaultButton) {
				if (Owner != null)
					Owner.OnDefaultRenderButtonClick(sender, args);
				else {
					var ibe = ((FrameworkRenderElementContext)sender).ElementHost.Parent as InplaceBaseEdit;
					var settings = ibe.With(x => x.Settings) as ButtonEditSettings;
					if (settings == null)
						return;
					settings.RaiseDefaultButtonClick(settings, args.OriginalEventArgs as RoutedEventArgs);
				}
			}
			else
				RaiseClickEvent(sender, args.OriginalEventArgs as RoutedEventArgs);
		}
		protected internal override void FindContent(FrameworkElement templatedParent) {
			if (Template != null) {
				ButtonBase button = Template.FindName(TemplateChildName, templatedParent) as ButtonBase;
				if (button != null && Owner != null)
					if (IsDefaultButton)
						button.Click += Owner.OnDefaultButtonClick;
					else
						button.Click += OnButtonClick;
			}
		}
		protected internal override void FindContent(RenderContentControlContext templatedParent) {
			base.FindContent(templatedParent);
			var bContext = templatedParent as RenderButtonContext;
			if (bContext != null) {
				if (bContext != null)
					bContext.Click += OnRenderButtonClick;
			}
		}
		protected override ButtonInfoBase CreateClone() {
			return new ButtonInfo();
		}
		protected override List<DependencyProperty> CreateCloneProperties() {
			List<DependencyProperty> list = base.CreateCloneProperties();
			list.Add(GlyphKindProperty);
			list.Add(ButtonKindProperty);
			list.Add(TemplateProperty);
			list.Add(ContentProperty);
			list.Add(ContentTemplateProperty);
			list.Add(CommandProperty);
			list.Add(CommandParameterProperty);
			list.Add(ContentRenderTemplateProperty);
			list.Add(ToolTipService.ShowDurationProperty);
			list.Add(ToolTipService.InitialShowDelayProperty);
			return list;
		}
		protected override void AssignToCloneInternal(ButtonInfoBase clone) {
			base.AssignToCloneInternal(clone);
			ButtonInfo info = clone as ButtonInfo;
			info.CommandTarget = CommandTarget;
		}
		protected override bool IsCloneInternal(ButtonInfoBase clone) {
			return base.IsCloneInternal(clone) && events[click] == null && clone.events[click] == null;
		}
	}
	public enum SpinStyle {
		Vertical,
		Horizontal
	}
	public partial class SpinButtonInfo : ButtonInfoBase {
		const string spinUpButtonName = "PART_SpinUpButton";
		const string spinDownButtonName = "PART_SpinDownButton";
		protected static readonly object spinUpClickLocker = new object();
		protected static readonly object spinDownClickLocker = new object();
		public static readonly DependencyProperty SpinStyleProperty;
		public static readonly DependencyProperty SpinUpCommandProperty;
		public static readonly DependencyProperty SpinUpCommandParameterProperty;
		public static readonly DependencyProperty SpinDownCommandProperty;
		public static readonly DependencyProperty SpinDownCommandParameterProperty;
		public static readonly DependencyProperty SpinUpCommandTargetProperty;
		public static readonly DependencyProperty SpinDownCommandTargetProperty;
		static SpinButtonInfo() {
			Type ownerType = typeof(SpinButtonInfo);
			SpinStyleProperty = DependencyPropertyManager.Register("SpinStyle", typeof(SpinStyle), ownerType, new FrameworkPropertyMetadata(SpinStyle.Vertical));
			SpinUpCommandProperty = DependencyPropertyManager.Register("SpinUpCommand", typeof(ICommand), ownerType, new FrameworkPropertyMetadata(null));
			SpinUpCommandParameterProperty = DependencyPropertyManager.Register("SpinUpCommandParameter", typeof(object), ownerType, new FrameworkPropertyMetadata(null));
			SpinDownCommandProperty = DependencyPropertyManager.Register("SpinDownCommand", typeof(ICommand), ownerType, new FrameworkPropertyMetadata(null));
			SpinDownCommandParameterProperty = DependencyPropertyManager.Register("SpinDownCommandParameter", typeof(object), ownerType, new FrameworkPropertyMetadata(null));
			DefaultStyleKeyProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(ownerType));
			SpinUpCommandTargetProperty = DependencyPropertyManager.Register("SpinUpCommandTarget", typeof(IInputElement), ownerType, new FrameworkPropertyMetadata(null));
			SpinDownCommandTargetProperty = DependencyPropertyManager.Register("SpinDownCommandTarget", typeof(IInputElement), ownerType, new FrameworkPropertyMetadata(null));
		}
		[Category("Behavior")]
		public event RoutedEventHandler SpinUpClick {
			add { events.AddHandler(spinUpClickLocker, value); }
			remove { events.RemoveHandler(spinUpClickLocker, value); }
		}
		[Category("Behavior")]
		public event RoutedEventHandler SpinDownClick {
			add { events.AddHandler(spinDownClickLocker, value); }
			remove { events.RemoveHandler(spinDownClickLocker, value); }
		}
		[Category("Appearance")]
		public SpinStyle SpinStyle {
			get { return (SpinStyle)GetValue(SpinStyleProperty); }
			set { SetValue(SpinStyleProperty, value); }
		}
		[Category("Action"), Localizability(LocalizationCategory.NeverLocalize)]
		public ICommand SpinUpCommand {
			get { return (ICommand)GetValue(SpinUpCommandProperty); }
			set { SetValue(SpinUpCommandProperty, value); }
		}
		[Category("Action"), Localizability(LocalizationCategory.NeverLocalize)]
		public object SpinUpCommandParameter {
			get { return GetValue(SpinUpCommandParameterProperty); }
			set { SetValue(SpinUpCommandParameterProperty, value); }
		}
		[Category("Action"), Localizability(LocalizationCategory.NeverLocalize)]
		public ICommand SpinDownCommand {
			get { return (ICommand)GetValue(SpinDownCommandProperty); }
			set { SetValue(SpinDownCommandProperty, value); }
		}
		[Category("Action"), Localizability(LocalizationCategory.NeverLocalize)]
		public object SpinDownCommandParameter {
			get { return GetValue(SpinDownCommandParameterProperty); }
			set { SetValue(SpinDownCommandParameterProperty, value); }
		}
		[Category("Action")]
		public IInputElement SpinUpCommandTarget {
			get { return (IInputElement)GetValue(SpinUpCommandTargetProperty); }
			set { SetValue(SpinUpCommandTargetProperty, value); }
		}
		[Category("Action")]
		public IInputElement SpinDownCommandTarget {
			get { return (IInputElement)GetValue(SpinDownCommandTargetProperty); }
			set { SetValue(SpinDownCommandTargetProperty, value); }
		}
		protected internal virtual void OnSpinUpButtonClick(object sender, System.Windows.RoutedEventArgs e) {
			RaiseSpinUpClickEvent(sender, e);
		}
		protected internal virtual void OnSpinDownButtonClick(object sender, System.Windows.RoutedEventArgs e) {
			RaiseSpinDownClickEvent(sender, e);
		}
		protected internal virtual void OnRenderSpinUpButtonClick(IFrameworkRenderElementContext sender, RenderEventArgsBase args) {
			if (!RaiseClickEventInInplaceInactiveMode)
				return;
			RaiseSpinUpClickEvent(sender, args.OriginalEventArgs as RoutedEventArgs);
		}
		protected internal virtual void OnRenderSpinDownButtonClick(IFrameworkRenderElementContext sender, RenderEventArgsBase args) {
			if (!RaiseClickEventInInplaceInactiveMode)
				return;
			RaiseSpinDownClickEvent(sender, args.OriginalEventArgs as RoutedEventArgs);
		}
		protected virtual void RaiseSpinUpClickEvent(object sender, System.Windows.RoutedEventArgs e) {
			RoutedEventHandler handler = events[spinUpClickLocker] as RoutedEventHandler;
			if (handler != null)
				handler(sender, e);
		}
		protected virtual void RaiseSpinDownClickEvent(object sender, System.Windows.RoutedEventArgs e) {
			RoutedEventHandler handler = events[spinDownClickLocker] as RoutedEventHandler;
			if (handler != null)
				handler(sender, e);
		}
		protected internal override void FindContent(FrameworkElement templatedParent) {
			if (Template != null) {
				ButtonBase spinUpButton = Template.FindName(spinUpButtonName, templatedParent) as ButtonBase;
				ButtonBase spinDownButton = Template.FindName(spinDownButtonName, templatedParent) as ButtonBase;
				if (IsDefaultButton) {
					if (spinUpButton != null)
						spinUpButton.Command = Owner.SpinUpCommand;
					if (spinDownButton != null)
						spinDownButton.Command = Owner.SpinDownCommand;
				}
				else {
					if (spinUpButton != null)
						spinUpButton.Click += OnSpinUpButtonClick;
					if (spinDownButton != null)
						spinDownButton.Click += OnSpinDownButtonClick;
				}
			}
		}
		protected internal override void FindContent(RenderContentControlContext templatedParent) {
			base.FindContent(templatedParent);
			if (templatedParent != null) {
				RenderButtonContext spinUpButton = templatedParent.InnerNamescope.GetElement(spinUpButtonName) as RenderButtonContext;
				RenderButtonContext spinDownButton = templatedParent.InnerNamescope.GetElement(spinDownButtonName) as RenderButtonContext;
				if (spinUpButton != null)
					spinUpButton.Click += OnRenderSpinUpButtonClick;
				if (spinDownButton != null)
					spinDownButton.Click += OnRenderSpinDownButtonClick;
			}
		}
		protected override ButtonInfoBase CreateClone() {
			return new SpinButtonInfo();
		}
		protected override List<DependencyProperty> CreateCloneProperties() {
			List<DependencyProperty> list = base.CreateCloneProperties();
			list.Add(SpinUpCommandProperty);
			list.Add(SpinUpCommandParameterProperty);
			list.Add(SpinDownCommandProperty);
			list.Add(SpinDownCommandParameterProperty);
			list.Add(SpinUpCommandTargetProperty);
			list.Add(SpinDownCommandTargetProperty);
			return list;
		}
	}
	public class SpinButton : RepeatButton {
		public static readonly DependencyProperty StartIntervalProperty;
		static SpinButton() {
			Type ownerType = typeof(SpinButton);
			StartIntervalProperty = DependencyPropertyManager.Register("StartInterval", typeof(int), ownerType, new FrameworkPropertyMetadata(500));
			FocusableProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(false));
		}
		public SpinButton() {
			Reset();
		}
		public int StartInterval {
			get { return (int)GetValue(StartIntervalProperty); }
			set { SetValue(StartIntervalProperty, value); }
		}
		protected Counter Counter = new Counter();
		int intervalDecrementValue = 50;
		void RestoreInterval() {
			Interval = StartInterval;
		}
		protected override void OnClick() {
			if (Counter.IsClear)
				Reset();
			base.OnClick();
			ChangeInterval();
			Counter.Increment();
		}
		protected virtual void ChangeInterval() {
			Interval = Math.Max(10, Interval - intervalDecrementValue);
		}
		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
			base.OnMouseLeftButtonUp(e);
			Reset();
		}
		protected override void OnKeyUp(KeyEventArgs e) {
			base.OnKeyUp(e);
			if (e.Key == Key.Space)
				Reset();
		}
		protected override void OnLostMouseCapture(MouseEventArgs e) {
			base.OnLostMouseCapture(e);
			Reset();
		}
		void Reset() {
			Counter.Reset();
			RestoreInterval();
		}
	}
#if !SL
	public class ButtonContainer : ContentPresenter {
		protected internal ButtonInfoBase Info { get { return DataContext as ButtonInfoBase; } }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			if (Info == null)
				return;
			Info.FindContent(this);
		}
	}
#else
	public partial class ButtonContainer : ContentControl { }
#endif
	public partial class ButtonsControl : ItemsControl {
		public ButtonsControl() {
			this.SetDefaultStyleKey(typeof(ButtonsControl));
#if SL
			ConstructorSLPart();
#endif
		}
		protected override DependencyObject GetContainerForItemOverride() {
			return new ButtonContainer();
		}
		protected override bool IsItemItsOwnContainerOverride(object item) {
			return item is ButtonContainer;
		}
	}
}
namespace DevExpress.Xpf.Editors {
#if SL
	using DevExpress.Data.Browsing;
#else
#endif
	public class ButtonInfoCollection : ObservableCollection<ButtonInfoBase> {
		public void Add(ButtonInfo item) {
			base.Add(item);
		}
		public void Add(SpinButtonInfo item) {
			base.Add(item);
		}
		public override bool Equals(object obj) {
			ButtonInfoCollection buttons = obj as ButtonInfoCollection;
			if (buttons == null || buttons.Count != Count)
				return false;
			for (int i = 0; i < Count; i++) {
				if (!ButtonInfoEquals(buttons[i], this[i]))
					return false;
			}
			return true;
		}
		public override int GetHashCode() {
			return Count;
		}
		protected bool ButtonInfoEquals(ButtonInfoBase x, ButtonInfoBase y) {
			return x.IsClone(y);
		}
	}
}
namespace DevExpress.Xpf.Editors {
	public class ButtonsPanel : Panel {
		protected override Size MeasureOverride(Size availableSize) {
			double width = 0d;
			double height = 0d;
			foreach (UIElement child in InternalChildren) {
				child.Measure(availableSize);
				width += child.DesiredSize.Width;
				height = Math.Max(child.DesiredSize.Height, height);
			}
			return new Size(width, height);
		}
		protected override Size ArrangeOverride(Size finalSize) {
			double left = 0.0;
			Size innerDesiredSize = new Size(InternalChildren.Cast<UIElement>().Sum(x => x.DesiredSize.Width), InternalChildren.Cast<UIElement>().Max(x => x.DesiredSize.Width));
			if (Math.Abs(innerDesiredSize.Width) <= double.Epsilon)
				return innerDesiredSize;
			for (int i = 0; i < InternalChildren.Count; i++) {
				if (InternalChildren[i].DesiredSize.IsEmpty)
					continue;
				double coeff = InternalChildren[i].DesiredSize.Width / innerDesiredSize.Width;
				double width = coeff * finalSize.Width;
				InternalChildren[i].Arrange(new Rect(
					new Point(left, 0),
					new Size(width, finalSize.Height)
					));
				left += width;
			}
			return finalSize;
		}
	}
}
