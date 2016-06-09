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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using DevExpress.Data.Utils;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Core {
	public class DXDialogWindow : DXWindow, IWindowSurrogate {
		#region Static
		internal const string SetSourceException = "Cannot use CommandsSource if CommandButtons collection is not empty.";
		const string SetFooterSourceException = "Cannot use CommandsSource if FooterButtons collection is not empty.";
		internal const string ButtonsCollectionChangedException = "Cannot change CommandButtons collection if CommandSource is set.";
		const string FooterButtonsCollectionChangedException = "Cannot change FooterButtons collection if CommandSource is set.";
		internal const string ShowDXDialogWindowException = @"Cannot use ShowDialogWindow(MessageBoxButton dialogButtons) if CommandButtons collection is not empty. 
            Use ShowDialogWindow() instead.";
		internal const string CloseOnEscapeException = @"CloseOnEscape is not supported by DXDialogWindow. Instead, you can define default and cancel commands. 
If you populate Dialog buttons from UICommands source, you can set the UICommand.IsDefault and UICommand.IsCancel properties. Default buttons are executed on Enter. Cancel buttons - on Escape.";
		internal const string ShowDialogWindowCoreException = @"Dialog is already shown.";
		public static readonly DependencyProperty CommandButtonStyleProperty =
			DependencyProperty.Register("CommandButtonStyle", typeof(Style), typeof(DXDialogWindow), new PropertyMetadata(null));
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static readonly DependencyProperty CommandButtonsPanelProperty =
			DependencyProperty.Register("CommandButtonsPanel", typeof(ItemsPanelTemplate), typeof(DXDialogWindow), new PropertyMetadata(null));
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty ActualDialogWindowProperty;
		public static readonly DependencyProperty CommandsSourceProperty;
		static readonly DependencyPropertyKey ActualFooterPropertyKey;
		public static readonly DependencyProperty ActualFooterProperty;
		static readonly DependencyPropertyKey HasFooterButtonsPropertyKey;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static readonly DependencyProperty HasFooterButtonsProperty;
		static DXDialogWindow() {
			DependencyPropertyRegistrator<DXDialogWindow>.New()
				.RegisterReadOnly(d => d.HasFooterButtons, out HasFooterButtonsPropertyKey, out HasFooterButtonsProperty, false)
				.RegisterReadOnly(d => d.ActualFooter, out ActualFooterPropertyKey, out ActualFooterProperty, null, (d, e) => d.OnActualFooterChanged(e))
				.Register(d => d.CommandsSource, out CommandsSourceProperty, null, (d, e) => d.OnCommandsSourceChanged(e))
				.RegisterAttached((DependencyObject d) => GetActualDialogWindow(d), out ActualDialogWindowProperty, null, FrameworkPropertyMetadataOptions.Inherits)
				.OverrideDefaultStyleKey()
			;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static DXDialogWindow GetActualDialogWindow(DependencyObject d) { return (DXDialogWindow)d.GetValue(ActualDialogWindowProperty); }
		static void SetActualDialogWindow(DependencyObject d, DXDialogWindow window) { d.SetValue(ActualDialogWindowProperty, window); }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool HasFooterButtons {
			get { return (bool)GetValue(HasFooterButtonsProperty); }
			private set { SetValue(HasFooterButtonsPropertyKey, value); }
		}
		public UIElement ActualFooter {
			get { return (UIElement)GetValue(ActualFooterProperty); }
			private set { SetValue(ActualFooterPropertyKey, value); }
		}
		readonly Dictionary<UIElement, int> customFootersDictionary = new Dictionary<UIElement, int>();
		readonly SortedDictionary<int, List<UIElement>> customFooters = new SortedDictionary<int, List<UIElement>>();
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void AddCustomFooter(UIElement footer) {
			int priority = LinqExtensions.Unfold<DependencyObject>(footer, x => LayoutHelper.GetParent(x, true), x => x == this).Count();
			customFootersDictionary.Add(footer, priority);
			customFooters.GetOrAdd(priority, () => new List<UIElement>()).Add(footer);
			UpdateActualFooter();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void RemoveCustomFooter(UIElement footer) {
			int priority = customFootersDictionary[footer];
			customFootersDictionary.Remove(footer);
			var list = customFooters[priority];
			list.Remove(footer);
			if(list.Count == 0)
				customFooters.Remove(priority);
			UpdateActualFooter();
		}
		void UpdateActualFooter() {
			ActualFooter = customFooters.LastOrDefault().Value.First();
		}
		bool footerIsWindowLogicalChild;
		void OnActualFooterChanged(DependencyPropertyChangedEventArgs e) {
			var oldValue = (UIElement)e.OldValue;
			var newValue = (UIElement)e.NewValue;
			if(oldValue != null) {
				BindingOperations.ClearBinding(oldValue, DialogButtonsControl.CommandButtonStyleProperty);
				if(footerIsWindowLogicalChild)
					RemoveLogicalChild(oldValue);
				footerIsWindowLogicalChild = false;
			}
			if(newValue != null) {
				footerIsWindowLogicalChild = LogicalTreeHelper.GetParent(newValue) == null;
				if(footerIsWindowLogicalChild)
					AddLogicalChild(newValue);
				BindingOperations.SetBinding(newValue, DialogButtonsControl.CommandButtonStyleProperty, new Binding() { Path = new PropertyPath(CommandButtonStyleProperty), Source = this, Mode = BindingMode.OneWay });
			}
			UpdateFooterPresenter();
		}
		NonLogicalDecorator footerPresenter;
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			if(footerPresenter != null) {
				footerPresenter.Child = null;
				footerPresenter = null;
			}
			footerPresenter = (NonLogicalDecorator)GetTemplateChild("FooterPresenter");
			UpdateFooterPresenter();
		}
		void UpdateFooterPresenter() {
			footerPresenter.Do(x => x.Child = ActualFooter);
			Dispatcher.BeginInvoke((Action)(() => footerPresenter.With(x => LayoutTreeHelper.GetVisualParents(x).OfType<ContentPresenter>().FirstOrDefault()).Do(x => x.InvalidateMeasure())));
		}
		protected override IEnumerator LogicalChildren {
			get {
				return new MergedEnumerator(base.LogicalChildren, footerIsWindowLogicalChild ? new SingleLogicalChildEnumerator(ActualFooter) : null);
			}
		}
		protected virtual DialogButtonsControl CreateDefaultFooter() {
			var footer = new DialogButtonsControl();
			footer.SetBinding(DialogButtonsControl.ItemsSourceProperty, new Binding() { Path = new PropertyPath(DialogButtonsControl.CommandsSourceProperty), Source = footer, Mode = BindingMode.OneWay });
			return footer;
		}
		readonly DialogButtonCollection footerButtons = new DialogButtonCollection();
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DialogButtonCollection FooterButtons { get { return footerButtons; } }
		readonly HierarchyCollection<object, DXDialogWindow> footerCommandsSourceCollection;
		IDisposable footerCommandsSourceCollectionBinding;
		void SetFooterCommandsSource(IEnumerable footerCommandsSource) {
			if(footerCommandsSourceCollectionBinding != null)
				footerCommandsSourceCollectionBinding.Dispose();
			footerCommandsSourceCollectionBinding = null;
			footerCommandsSourceCollection.Clear();
			if(footerCommandsSource != null)
				footerCommandsSourceCollectionBinding = CollectionBindingHelper.BindOneWay(footerCommandsSourceCollection, (object x) => x, footerCommandsSource);
			DialogButtonsControl.SetCommandsSource(this, footerCommandsSource);
		}
		void AttachUICommand(object item) {
			DoWithFooterCommand(item, x => x.Executed += OnUICommandExecuted, x => x.Executed += OnDialogButtonExecuted);
		}
		void DetachUICommand(object item) {
			DoWithFooterCommand(item, x => x.Executed -= OnUICommandExecuted, x => x.Executed -= OnDialogButtonExecuted);
		}
		void DoWithFooterCommand(object commandsSourceItem, Action<IUICommand> uiCommandAction, Action<DialogButton> uiCommandButtonAction) {
			var uiCommand = commandsSourceItem as UICommandContainer;
			if(uiCommand != null) {
				IUICommand nativeCommand = uiCommand.UICommand;
				if(nativeCommand != null)
					uiCommandAction(nativeCommand);
				return;
			}
			var uiCommandButton = commandsSourceItem as DialogButton;
			if(uiCommandButton != null) {
				uiCommandButtonAction(uiCommandButton);
				return;
			}
		}
		void OnUICommandExecuted(object sender, EventArgs e) {
			CloseCore((UICommand)sender);
		}
		void OnDialogButtonExecuted(object sender, EventArgs e) {
			CloseCore(null);
		}
		public static List<UICommand> GenerateUICommands(MessageBoxButton dialogButtons, MessageBoxResult? defaultButton = null, MessageBoxResult? cancelButton = null) {
			return UICommand.GenerateFromMessageBoxButton(dialogButtons, new DXDialogWindowMessageBoxButtonLocalizer(), defaultButton, cancelButton);
		}
		static List<UICommand> GetDefaultAndCancelCommands(IEnumerable<UICommand> commandsSource, bool includeDefaultButtons = true, bool includeCancelButtons = true) {
			if(commandsSource == null) return null;
			List<UICommand> res = new List<UICommand>();
			foreach(UICommand command in commandsSource) {
				if(command.IsDefault && includeDefaultButtons)
					res.Add(command);
				if(command.IsCancel && includeCancelButtons)
					res.Add(command);
			}
			return res;
		}
		#endregion Static
		public IEnumerable<UICommand> CommandsSource {
			get { return (IEnumerable<UICommand>)GetValue(CommandsSourceProperty); }
			set { SetValue(CommandsSourceProperty, value); }
		}
		public Style CommandButtonStyle {
			get { return (Style)GetValue(CommandButtonStyleProperty); }
			set { SetValue(CommandButtonStyleProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Use the FooterTemplate property instead")]
		public ItemsPanelTemplate CommandButtonsPanel {
			get { return (ItemsPanelTemplate)GetValue(CommandButtonsPanelProperty); }
			set { SetValue(CommandButtonsPanelProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Use the FooterButtons property instead")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DXDialogWindowCommandButtonsCollection CommandButtons { get; private set; }
		MessageBoxResult dialogWindowResult = MessageBoxResult.None;
		UICommand dialogWindowResultCommand = null;
		protected bool IsDialogMode = false;
		public DXDialogWindow() {
			footerCommandsSourceCollection = new HierarchyCollection<object, DXDialogWindow>(this, (item, owner) => owner.AttachUICommand(item), (item, owner) => owner.DetachUICommand(item));
			footerCommandsSourceCollection.CollectionChanged += (s, e) => HasFooterButtons = footerCommandsSourceCollection.Any();
			FooterButtons.CollectionChanged += (s, e) => {
				if(CommandsSource != null)
					throw new InvalidOperationException(FooterButtonsCollectionChangedException);
				SetFooterCommandsSource(FooterButtons);
			};
			ActualFooter = CreateDefaultFooter();
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
#pragma warning disable 612, 618
			CommandButtons = new DXDialogWindowCommandButtonsCollection();
#pragma warning restore 612, 618
			SetActualDialogWindow(this, this);
		}
		public DXDialogWindow(string title) :
			this() {
			Title = title;
		}
		public DXDialogWindow(string title, MessageBoxButton dialogButtons, MessageBoxResult? defaultButton = null, MessageBoxResult? cancelButton = null) :
			this(title, GenerateUICommands(dialogButtons, defaultButton, cancelButton)) {
		}
		public DXDialogWindow(string title, IEnumerable<UICommand> commands)
			: this() {
			Title = title;
			CommandsSource = commands;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new void Show() {
			BeforeShow();
			base.Show();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool? ShowDialog() {
			ShowDialogWindowCore();
			return DialogResult;
		}
		public MessageBoxResult ShowDialogWindow(MessageBoxButton dialogButtons, MessageBoxResult? defaultButton = null, MessageBoxResult? cancelButton = null) {
			ShowDialogWindowMessageBoxResult(dialogButtons, defaultButton, cancelButton);
			return dialogWindowResult;
		}
		public UICommand ShowDialogWindow() {
			ShowDialogWindowUICommand();
			return dialogWindowResultCommand;
		}
		protected override void OnClosed(EventArgs e) {
			base.OnClosed(e);
			OnClosedCore();
		}
#if DEBUGTEST
		public event CancelEventHandler Closing_Test;
		protected override void OnClosing(CancelEventArgs e) {
			base.OnClosing(e);
			if(Closing_Test != null)
				Closing_Test(this, e);
		}
#endif
		void ShowDialogWindowMessageBoxResult(MessageBoxButton dialogButtons, MessageBoxResult? defaultButton = null, MessageBoxResult? cancelButton = null) {
#pragma warning disable 612, 618
			if(CommandsSource == null && CommandButtons.Count > 0)
				throw new InvalidOperationException(ShowDXDialogWindowException);
			CommandsSource = GenerateUICommands(dialogButtons, defaultButton, cancelButton);
			ShowDialogWindowCore();
#pragma warning restore 612, 618
		}
		void ShowDialogWindowUICommand() {
			ShowDialogWindowCore();
		}
		protected virtual void BeforeShow() { }
		protected virtual void ShowDialogWindowCore() {
			BeforeShow();
			IsDialogMode = true;
			dialogWindowResult = MessageBoxResult.None;
			dialogWindowResultCommand = null;
			base.ShowDialog();
		}
		void OnClosedCore() {
			IsDialogMode = false;
			if(CommandsSource != null) {
#pragma warning disable 612, 618
				CommandButtons.ClearSource();
#pragma warning restore 612, 618
				SetFooterCommandsSource(null);
			}
		}
		protected virtual void CloseCore(UICommand command) {
			if(IsDialogMode) {
				dialogWindowResultCommand = command;
				bool? dialogResult = true;
				if(command.Tag is MessageBoxResult) {
					MessageBoxResult result = (MessageBoxResult)command.Tag;
					dialogWindowResult = result;
					if(result == MessageBoxResult.OK || result == MessageBoxResult.Yes)
						dialogResult = true;
					else if(result == MessageBoxResult.No || result == MessageBoxResult.Cancel)
						dialogResult = false;
					else dialogResult = null;
				}
				DialogResult = dialogResult;
			} else Close();
		}
		protected virtual Button CreateButton(UICommand command) {
			DXDialogWindowUICommandWrapper commandWrapper = new DXDialogWindowUICommandWrapper(command);
			Button button = new Button() { DataContext = commandWrapper };
			button.SetBinding(Button.CommandProperty, new Binding("RealCommand"));
			button.SetBinding(Button.ContentProperty, new Binding("UICommand.Caption"));
			button.SetBinding(Button.StyleProperty, new Binding() { Path = new PropertyPath(CommandButtonStyleProperty), Source = this });
			button.SetBinding(Button.IsDefaultProperty, new Binding("UICommand.IsDefault"));
			button.SetBinding(Button.IsCancelProperty, new Binding("UICommand.IsCancel"));
			SubscribeButton(button);
			return button;
		}
		protected virtual void ClearButton(UIElement element) {
			Button button = (Button)element;
			UnsubscribeButton(button);
			button.DataContext = null;
		}
		void OnCommandsSourceChanged(DependencyPropertyChangedEventArgs e) {
			var oldValue = (IEnumerable<UICommand>)e.OldValue;
			var newValue = (IEnumerable<UICommand>)e.NewValue;
			if(FooterButtons.Count != 0)
				throw new InvalidOperationException(DXDialogWindow.SetFooterSourceException);
			if(newValue == null) {
				SetFooterCommandsSource(null);
			} else {
				UICommandContainerCollection weakCollection = new UICommandContainerCollection();
				var newList = newValue as IList;
				if(newList == null)
					newList = newValue.ToList();
				CollectionBindingHelper.Bind(weakCollection, (UICommand command) => command.With(x => new UICommandContainer(x)), x => x.UICommand, newList);
				SetFooterCommandsSource(weakCollection);
			}
#pragma warning disable 612, 618
			CommandButtons.SetSource(newValue, CreateButton, ClearButton, true);
#pragma warning restore 612, 618
		}
		void OnLoaded(object sender, EventArgs e) {
#pragma warning disable 612, 618
			foreach(UIElement button in CommandButtons) {
				if(button is Button) {
					SubscribeButton((Button)button);
				}
			}
#pragma warning restore 612, 618
		}
		void OnUnloaded(object sender, EventArgs e) {
#pragma warning disable 612, 618
			foreach(UIElement button in CommandButtons) {
				if(button is Button) {
					UnsubscribeButton((Button)button);
				}
			}
#pragma warning restore 612, 618
		}
		void UICommandExecuted(object sender, CancelEventArgs e) {
			UICommand command = ((DXDialogWindowUICommandWrapper)sender).UICommand;
			if(e.Cancel) return;
			CloseCore(command);
		}
		void SubscribeButton(Button button) {
			UnsubscribeButton(button);
			DXDialogWindowUICommandWrapper commandWrapper = button.DataContext as DXDialogWindowUICommandWrapper;
			if(commandWrapper == null) return;
			commandWrapper.Subscribe();
			commandWrapper.CommandExecuted += UICommandExecuted;
		}
		void UnsubscribeButton(Button button) {
			DXDialogWindowUICommandWrapper commandWrapper = button.DataContext as DXDialogWindowUICommandWrapper;
			if(commandWrapper == null) return;
			commandWrapper.Unsubscribe();
			commandWrapper.CommandExecuted -= UICommandExecuted;
		}
		#region IWindowSurrogate
		Window IWindowSurrogate.RealWindow { get { return this; } }
		void IWindowSurrogate.Show() {
			Show();
		}
		bool? IWindowSurrogate.ShowDialog() {
			return ShowDialog();
		}
		#endregion
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class DXDialogWindowMessageBoxButtonLocalizer : IMessageBoxButtonLocalizer {
		public string Localize(MessageBoxResult button) {
			switch(button) {
				case MessageBoxResult.OK:
					return DXMessageBoxLocalizer.GetString(DXMessageBoxStringId.Ok);
				case MessageBoxResult.Cancel:
					return DXMessageBoxLocalizer.GetString(DXMessageBoxStringId.Cancel);
				case MessageBoxResult.Yes:
					return DXMessageBoxLocalizer.GetString(DXMessageBoxStringId.Yes);
				case MessageBoxResult.No:
					return DXMessageBoxLocalizer.GetString(DXMessageBoxStringId.No);
				default:
					return string.Empty;
			}
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class DXDialogWindowUICommandWrapper {
		public UICommand UICommand { get; private set; }
		public DelegateCommand RealCommand { get; private set; }
		public event CancelEventHandler CommandExecuted;
		public DXDialogWindowUICommandWrapper(UICommand uiCommand) {
			UICommand = uiCommand;
			RealCommand = new DelegateCommand(CommandWrapperExecute, CommandWrapperCanExecute);
		}
		public void Subscribe() {
			Unsubscribe();
			UICommand.PropertyChanged += OnUICommandPropertyChanged;
			if(UICommand.Command != null)
				UICommand.Command.CanExecuteChanged += OnCommandCanExecuteChanged;
		}
		public void Unsubscribe() {
			if(UICommand.Command != null)
				UICommand.Command.CanExecuteChanged -= OnCommandCanExecuteChanged;
			UICommand.PropertyChanged -= OnUICommandPropertyChanged;
		}
		void OnUICommandPropertyChanged(object sender, PropertyChangedEventArgs e) {
			if(e.PropertyName == BindableBase.GetPropertyName(() => UICommand.Command)) {
				Subscribe();
				RealCommand.RaiseCanExecuteChanged();
			}
		}
		void OnCommandCanExecuteChanged(object sender, EventArgs e) {
			if(lockOnCommandCanExecuteChanged > 0) {
				lockOnCommandCanExecuteChanged = 0;
				return;
			}
			RealCommand.RaiseCanExecuteChanged();
		}
		void CommandWrapperExecute() {
			CancelEventArgs e = new CancelEventArgs();
			if(UICommand.Command != null)
				UICommand.Command.Execute(e);
			if(CommandExecuted != null)
				CommandExecuted(this, e);
		}
		int lockOnCommandCanExecuteChanged = 0;
		bool CommandWrapperCanExecute() {
			lockOnCommandCanExecuteChanged++;
			return UICommand.Command.Return(x => x.CanExecute(null), () => true);
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class DXDialogWindowCommandButtonsCollection : ObservableCollection<UIElement> {
		Func<UICommand, UIElement> CreateElement;
		Action<UIElement> ClearElement;
		IEnumerable<UICommand> Commands = null;
		bool allowCollectionChanged = true;
		public Button this[UICommand command] {
			get { return FindButton(this, command); }
		}
		static Button FindButton(DXDialogWindowCommandButtonsCollection buttons, UICommand command) {
			if(command == null) return null;
			foreach(UIElement element in buttons) {
				Button bt = element as Button;
				if(bt == null) continue;
				DXDialogWindowUICommandWrapper c = bt.DataContext as DXDialogWindowUICommandWrapper;
				if(c != null && c.UICommand == command)
					return bt;
			}
			return null;
		}
		internal void SetSource(IEnumerable<UICommand> commands, Func<UICommand, UIElement> createElement, Action<UIElement> clearElement, bool doNotGenerateButtons) {
			if(Commands == null && Count > 0)
				throw new InvalidOperationException(DXDialogWindow.SetSourceException);
			ClearSource();
			Commands = commands;
			CreateElement = createElement;
			ClearElement = clearElement;
			if(Commands == null) return;
			Func<object, object> convertItemAction = new Func<object, object>((p) => CreateElement((UICommand)p));
			SyncCollectionHelper.PopulateCore(this, doNotGenerateButtons ? Enumerable.Empty<UICommand>() : Commands, convertItemAction);
			allowCollectionChanged = false;
			if(!doNotGenerateButtons && Commands is INotifyCollectionChanged)
				((INotifyCollectionChanged)Commands).CollectionChanged += OnCommandsCollectionChanged;
		}
		internal void ClearSource() {
			if(Commands == null) return;
			if(Commands is INotifyCollectionChanged)
				((INotifyCollectionChanged)Commands).CollectionChanged -= OnCommandsCollectionChanged;
			allowCollectionChanged = true;
			foreach(UIElement element in this)
				ClearElement(element);
			Clear();
			Commands = null;
			CreateElement = null;
			ClearElement = null;
		}
		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e) {
			if(!allowCollectionChanged)
				throw new InvalidOperationException(DXDialogWindow.ButtonsCollectionChangedException);
			base.OnCollectionChanged(e);
		}
		void OnCommandsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			if(!(Commands is IList)) return;
			allowCollectionChanged = true;
			Func<object, object> convertItemAction = new Func<object, object>((p) => CreateElement((UICommand)p));
			SyncCollectionHelper.SyncCollection(e, this, (IList)Commands, convertItemAction);
			allowCollectionChanged = false;
		}
	}
	public class DialogButtonsControl : ItemsControl {
		public static readonly DependencyProperty CommandsSourceProperty;
		public static readonly DependencyProperty CommandButtonStyleProperty;
		static DialogButtonsControl() {
			DependencyPropertyRegistrator<DialogButtonsControl>.New()
				.RegisterAttached((DependencyObject d) => GetCommandsSource(d), out CommandsSourceProperty, null, FrameworkPropertyMetadataOptions.Inherits)
				.RegisterAttached((DependencyObject d) => GetCommandButtonStyle(d), out CommandButtonStyleProperty, null, FrameworkPropertyMetadataOptions.Inherits)
				.OverrideDefaultStyleKey()
			;
		}
		public static IEnumerable GetCommandsSource(DependencyObject d) { return (IEnumerable)d.GetValue(CommandsSourceProperty); }
		public static void SetCommandsSource(DependencyObject d, IEnumerable commandsSource) { d.SetValue(CommandsSourceProperty, commandsSource); }
		public static Style GetCommandButtonStyle(DependencyObject d) { return (Style)d.GetValue(CommandButtonStyleProperty); }
		public static void SetCommandButtonStyle(DependencyObject d, Style style) { d.SetValue(CommandButtonStyleProperty, style); }
		readonly List<UIElement> logicalChildren = new List<UIElement>();
		protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e) {
			base.OnItemsChanged(e);
			foreach(var item in logicalChildren)
				RemoveLogicalChild(item);
			logicalChildren.Clear();
			logicalChildren.AddRange(Items.OfType<UIElement>());
			foreach(var item in logicalChildren)
				AddLogicalChild(item);
		}
		protected override IEnumerator LogicalChildren {
			get {
				return new MergedEnumerator(base.LogicalChildren, logicalChildren.GetEnumerator());
			}
		}
		protected override DependencyObject GetContainerForItemOverride() {
			return new DialogButton();
		}
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			base.PrepareContainerForItemOverride(element, item);
			var button = element as DialogButton;
			var command = UICommandContainer.GetUICommand(item);
			if(button != null && command != null) {
				button.DialogUICommandTag = command;
				button.Content = DialogButton.NotSetContent;
			}
		}
		protected override void ClearContainerForItemOverride(DependencyObject element, object item) {
			var button = element as DialogButton;
			var command = UICommandContainer.GetUICommand(item);
			if(button != null && command != null)
				button.DialogUICommandTag = command;
			base.ClearContainerForItemOverride(element, item);
		}
	}
	public class DialogButtonCollection : ObservableCollection<DialogButton> { }
	public class DialogButton : ContentControl {
		class NotSetCommandImpl : ICommand {
			event EventHandler ICommand.CanExecuteChanged { add { } remove { } }
			bool ICommand.CanExecute(object parameter) { return true; }
			void ICommand.Execute(object parameter) { }
		}
		public static readonly object NotSetContent = new object();
		public static readonly ICommand NotSetCommand = new NotSetCommandImpl();
		public static readonly DependencyProperty CommandButtonStyleProperty;
		public static readonly DependencyProperty CommandProperty;
		public static readonly DependencyProperty IsCancelProperty;
		public static readonly DependencyProperty IsDefaultProperty;
		static readonly DependencyPropertyKey ActualCommandPropertyKey;
		public static readonly DependencyProperty ActualCommandProperty;
		static readonly DependencyPropertyKey ActualContentPropertyKey;
		public static readonly DependencyProperty ActualContentProperty;
		static readonly DependencyPropertyKey ActualIsCancelPropertyKey;
		public static readonly DependencyProperty ActualIsCancelProperty;
		static readonly DependencyPropertyKey ActualIsDefaultPropertyKey;
		public static readonly DependencyProperty ActualIsDefaultProperty;
		public static readonly DependencyProperty DialogUICommandTagProperty;
		public static readonly DependencyProperty DialogResultProperty;
		public static readonly DependencyProperty CommandsSourceProperty;
		static readonly DependencyPropertyKey UICommandPropertyKey;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static readonly DependencyProperty UICommandProperty;
		static readonly DependencyPropertyKey CommandButtonVisibilityPropertyKey;
		public static readonly DependencyProperty CommandButtonVisibilityProperty;
		static DialogButton() {
			DependencyPropertyRegistrator<DialogButton>.New()
				.OverrideMetadata(ContentProperty, NotSetContent, d => d.UpdateActualContent())
				.AddOwner(d => d.CommandButtonStyle, out CommandButtonStyleProperty, DialogButtonsControl.CommandButtonStyleProperty)
				.Register(d => d.Command, out CommandProperty, NotSetCommand, d => d.UpdateActualCommand())
				.Register(d => d.IsDefault, out IsDefaultProperty, null, d => d.UpdateActualIsDefault())
				.Register(d => d.IsCancel, out IsCancelProperty, null, d => d.UpdateActualIsCancel())
				.RegisterReadOnly(d => d.ActualCommand, out ActualCommandPropertyKey, out ActualCommandProperty, null)
				.RegisterReadOnly(d => d.ActualContent, out ActualContentPropertyKey, out ActualContentProperty, null)
				.RegisterReadOnly(d => d.ActualIsDefault, out ActualIsDefaultPropertyKey, out ActualIsDefaultProperty, false)
				.RegisterReadOnly(d => d.ActualIsCancel, out ActualIsCancelPropertyKey, out ActualIsCancelProperty, false)
				.Register(d => d.DialogUICommandTag, out DialogUICommandTagProperty, null, d => d.OnDialogUICommandTagChanged())
				.Register(d => d.DialogResult, out DialogResultProperty, null, d => d.OnDialogUICommandTagChanged())
				.AddOwner(d => d.CommandsSource, out CommandsSourceProperty, DialogButtonsControl.CommandsSourceProperty, null, (d, e) => d.OnCommandsSourceChanged(e))
				.RegisterReadOnly(d => d.UICommand, out UICommandPropertyKey, out UICommandProperty, null, (d, e) => d.OnUICommandChanged(e))
				.RegisterReadOnly(d => d.CommandButtonVisibility, out CommandButtonVisibilityPropertyKey, out CommandButtonVisibilityProperty, Visibility.Visible)
				.OverrideDefaultStyleKey()
			;
		}
		readonly PropertyChangedWeakEventHandler<DialogButton> onUICommandPropertyChanged;
		public DialogButton() {
			onUICommandPropertyChanged = new PropertyChangedWeakEventHandler<DialogButton>(this, (owner, sender, e) => owner.OnUICommandPropertyChanged(sender, e));
			commandsSourceCollection = new HierarchyCollection<UICommand, DialogButton>(this, (item, owner) => owner.StartTrackCommandSourceItem(item), (item, onwer) => onwer.StopTrackCommandSourceItem(item));
		}
		public Style CommandButtonStyle {
			get { return (Style)GetValue(CommandButtonStyleProperty); }
			set { SetValue(CommandButtonStyleProperty, value); }
		}
		public ICommand Command {
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}
		public bool? IsDefault {
			get { return (bool?)GetValue(IsDefaultProperty); }
			set { SetValue(IsDefaultProperty, value); }
		}
		public bool? IsCancel {
			get { return (bool?)GetValue(IsCancelProperty); }
			set { SetValue(IsCancelProperty, value); }
		}
		void OnUICommandChanged(DependencyPropertyChangedEventArgs e) {
			var oldValue = (UICommand)e.OldValue;
			var newValue = (UICommand)e.NewValue;
			if(oldValue != null)
				oldValue.PropertyChanged -= this.onUICommandPropertyChanged.Handler;
			if(newValue != null)
				newValue.PropertyChanged += this.onUICommandPropertyChanged.Handler;
			UpdateActualProperties();
		}
		void OnUICommandPropertyChanged(object sender, PropertyChangedEventArgs e) {
			UpdateActualProperties();
		}
		void UpdateActualProperties() {
			UpdateActualCommand();
			UpdateActualContent();
			UpdateActualIsDefault();
			UpdateActualIsCancel();
		}
		void UpdateActualCommand() {
			var commandFromUICommand = GetCommand(UICommand);
			var commandFromCommandProperty = Equals(Command, NotSetCommand) ? null : GetCommand(Command);
			var commands = new ICommand[] { commandFromCommandProperty, commandFromUICommand }.Where(x => x != null);
			var oldCommand = (CombinedCommand)ActualCommand;
			ActualCommand = null;
			if(oldCommand != null)
				oldCommand.Dispose();
			ActualCommand = commands.Any() ? new CombinedCommand(commands, _ => new CancelEventArgs()) : null;
		}
		void UpdateActualContent() {
			ActualContent = Equals(Content, NotSetContent) ? UICommand.With(x => x.Caption) : Content;
		}
		void UpdateActualIsDefault() {
			ActualIsDefault = IsDefault != null ? IsDefault.Value : UICommand != null ? UICommand.IsDefault : false;
		}
		void UpdateActualIsCancel() {
			ActualIsCancel = IsCancel != null ? IsCancel.Value : UICommand != null ? UICommand.IsCancel : false;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ICommand ActualCommand {
			get { return (ICommand)GetValue(ActualCommandProperty); }
			private set { SetValue(ActualCommandPropertyKey, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public object ActualContent {
			get { return GetValue(ActualContentProperty); }
			private set { SetValue(ActualContentPropertyKey, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool ActualIsDefault {
			get { return (bool)GetValue(ActualIsDefaultProperty); }
			private set { SetValue(ActualIsDefaultPropertyKey, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool ActualIsCancel {
			get { return (bool)GetValue(ActualIsCancelProperty); }
			private set { SetValue(ActualIsCancelPropertyKey, value); }
		}
		public IEnumerable CommandsSource {
			get { return (IEnumerable)GetValue(CommandsSourceProperty); }
			set { SetValue(CommandsSourceProperty, value); }
		}
		public object DialogUICommandTag {
			get { return GetValue(DialogUICommandTagProperty); }
			set { SetValue(DialogUICommandTagProperty, value); }
		}
		public MessageResult? DialogResult {
			get { return (MessageResult?)GetValue(DialogResultProperty); }
			set { SetValue(DialogResultProperty, value); }
		}
		object actualDialogUICommandTag;
		void OnDialogUICommandTagChanged() {
			actualDialogUICommandTag = DialogResult == null ? DialogUICommandTag : DialogResult.Value.ToString();
			UpdateCommandsSourceCollection();
		}
		readonly HierarchyCollection<UICommand, DialogButton> commandsSourceCollection;
		IDisposable commandsSourceCollectionBinding;
		void OnCommandsSourceChanged(DependencyPropertyChangedEventArgs e) {
			UpdateCommandsSourceCollection();
		}
		void UpdateCommandsSourceCollection() {
			if(commandsSourceCollectionBinding != null)
				commandsSourceCollectionBinding.Dispose();
			commandsSourceCollectionBinding = null;
			if(actualDialogUICommandTag == null || CommandsSource == null)
				commandsSourceCollection.Clear();
			else
				commandsSourceCollectionBinding = CollectionBindingHelper.BindOneWay(commandsSourceCollection, (object x) => UICommandContainer.GetUICommand(x), CommandsSource);
			UpdateUICommand();
		}
		void StartTrackCommandSourceItem(UICommand uiCommand) {
			if(uiCommand == null) return;
			uiCommand.PropertyChanged += OnCommandsSourceItemPropertyChanged;
		}
		void StopTrackCommandSourceItem(UICommand uiCommand) {
			if(uiCommand == null) return;
			uiCommand.PropertyChanged -= OnCommandsSourceItemPropertyChanged;
		}
		void OnCommandsSourceItemPropertyChanged(object sender, PropertyChangedEventArgs e) {
			if(string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == ExpressionHelper.GetPropertyName((UICommand x) => x.Tag))
				UpdateUICommand();
		}
		void UpdateUICommand() {
			if(actualDialogUICommandTag == null || CommandsSource == null) {
				UICommand = null;
				CommandButtonVisibility = Visibility.Visible;
			} else {
				UICommand = GetUICommandByTag(commandsSourceCollection, actualDialogUICommandTag);
				CommandButtonVisibility = UICommand == null ? Visibility.Collapsed : Visibility.Visible;
			}
		}
		public Visibility CommandButtonVisibility {
			get { return (Visibility)GetValue(CommandButtonVisibilityProperty); }
			private set { SetValue(CommandButtonVisibilityPropertyKey, value); }
		}
		public static UICommand GetUICommandByTag(IEnumerable<UICommand> uiCommands, object tag) {
			var tagCommand = tag as UICommand;
			if(tagCommand != null) return tagCommand;
			var tagString = tag as string;
			if(tagString != null)
				return uiCommands.Where(uiCommand => string.Equals(uiCommand.With(x => x.Tag).With(x => x.ToString()), tagString, StringComparison.Ordinal)).FirstOrDefault();
			else
				return uiCommands.Where(uiCommand => Equals(uiCommand.With(x => x.Tag), tag)).FirstOrDefault();
		}
		public static UICommand GetUICommandByDialogResult(IEnumerable<UICommand> uiCommands, MessageResult dialogResult) {
			return GetUICommandByTag(uiCommands, dialogResult.ToString());
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public UICommand UICommand {
			get { return (UICommand)GetValue(UICommandProperty); }
			private set { SetValue(UICommandPropertyKey, value); }
		}
		internal event EventHandler Executed;
		ICommand GetCommand(ICommand command) {
			return new CommandWrapper(command, () => {
				if(Executed != null)
					Executed(this, EventArgs.Empty);
			});
		}
		internal static ICommand GetCommand(UICommand uiCommand) {
			if(uiCommand == null) return null;
			IUICommand nativeUICommand = uiCommand;
			return new CommandWrapper(uiCommand.Command, nativeUICommand.RaiseExecuted);
		}
		sealed class CommandWrapper : ICommand, ICommandWrapper {
			readonly ICommand innerCommand;
			readonly Action onExecute;
			public CommandWrapper(ICommand innerCommand, Action onExecute) {
				this.onExecute = onExecute;
				this.innerCommand = innerCommand;
				if(this.innerCommand != null)
					this.innerCommand.CanExecuteChanged += OnInnerCommandCanExecuteChanged;
			}
			void OnInnerCommandCanExecuteChanged(object sender, EventArgs e) {
				if(canExecuteChanged != null)
					canExecuteChanged(this, e);
			}
			EventHandler canExecuteChanged;
			event EventHandler ICommand.CanExecuteChanged {
				add { canExecuteChanged += value; }
				remove { canExecuteChanged -= value; }
			}
			bool ICommand.CanExecute(object parameter) {
				return parameter is CancelEventArgs && (innerCommand == null || innerCommand.CanExecute(parameter));
			}
			void ICommand.Execute(object parameter) {
				var args = (CancelEventArgs)parameter;
				if(args.Cancel) return;
				if(innerCommand != null)
					innerCommand.Execute(args);
				if(args.Cancel) return;
				onExecute();
			}
#if DEBUGTEST
			ICommand ICommandWrapper.InnerCommand { get { return innerCommand; } }
#endif
		}
		sealed class CombinedCommand : ICommand, ICombinedCommand, IDisposable {
			readonly IEnumerable<ICommand> innerCommands;
			readonly Func<object, object> coerceParameter;
			public CombinedCommand(IEnumerable<ICommand> innerCommands, Func<object, object> coerceParameter) {
				this.coerceParameter = coerceParameter;
				this.innerCommands = innerCommands.ToArray();
				foreach(var innerCommand in innerCommands)
					innerCommand.CanExecuteChanged += OnInnerCommandCanExecuteChanged;
			}
			public void Dispose() {
				foreach(var innerCommand in innerCommands)
					innerCommand.CanExecuteChanged -= OnInnerCommandCanExecuteChanged;
			}
			void OnInnerCommandCanExecuteChanged(object sender, EventArgs e) {
				if(canExecuteChanged != null)
					canExecuteChanged(this, EventArgs.Empty);
			}
			EventHandler canExecuteChanged;
			event EventHandler ICommand.CanExecuteChanged {
				add { canExecuteChanged += value; }
				remove { canExecuteChanged -= value; }
			}
			bool ICommand.CanExecute(object parameter) {
				parameter = coerceParameter(parameter);
				return !innerCommands.Where(x => !x.CanExecute(parameter)).Any();
			}
			void ICommand.Execute(object parameter) {
				parameter = coerceParameter(parameter);
				foreach(var innerCommand in innerCommands) {
					innerCommand.Execute(parameter);
				}
			}
#if DEBUGTEST
			IEnumerable<ICommand> ICombinedCommand.InnerCommands { get { return innerCommands; } }
#endif
		}
#if DEBUGTEST
		internal interface ICombinedCommand {
			IEnumerable<ICommand> InnerCommands { get; }
		}
		internal interface ICommandWrapper {
			ICommand InnerCommand { get; }
		}
#else
		interface ICombinedCommand { }
		interface ICommandWrapper { }
#endif
	}
	public class UICommandContainer : INotifyPropertyChanged {
		public UICommandContainer(UICommand uiCommand) {
			uiCommandRef = new WeakReference(uiCommand);
		}
		readonly WeakReference uiCommandRef;
		public UICommand UICommand { get { return (UICommand)uiCommandRef.Target; } }
		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged { add { } remove { } }
		internal static UICommand GetUICommand(object commandSourceItem) {
			var uiCommandContainer = commandSourceItem as UICommandContainer;
			if(uiCommandContainer != null) return uiCommandContainer.UICommand;
			var uiCommandButton = commandSourceItem as DialogButton;
			if(uiCommandButton != null) return uiCommandButton.UICommand;
			return commandSourceItem as UICommand;
		}
	}
	public class UICommandContainerCollection : ObservableCollection<UICommandContainer> { }
	public class CurrentDialogService : CurrentWindowService, ICurrentDialogService {
		IEnumerable<UICommand> ICurrentDialogService.UICommands { get { return GetUICommands(); } }
		void ICurrentDialogService.Close(MessageResult dialogResult) {
			Close(GetUICommands().With(x => DialogButton.GetUICommandByDialogResult(x, dialogResult)));
		}
		void ICurrentDialogService.Close(UICommand dialogResult) {
			Close(dialogResult);
		}
		void Close(UICommand dialogResult) {
			if(dialogResult == null) {
				ICurrentWindowService windowService = this;
				windowService.Close();
				return;
			}
			ICommand command = DialogButton.GetCommand(dialogResult);
			var args = new CancelEventArgs();
			if(command.CanExecute(args))
				command.Execute(args);
		}
		IEnumerable<UICommand> GetUICommands() {
			return DialogButtonsControl.GetCommandsSource(WindowSource ?? AssociatedObject).With(x => x.Cast<object>().Select(UICommandContainer.GetUICommand));
		}
	}
	[ContentProperty("Child")]
	public class NonVisualDecorator : FrameworkElement, IAddChild {
		readonly static DependencyPropertyKey ActualChildPropertyKey;
		public static readonly DependencyProperty ActualChildProperty;
		static NonVisualDecorator() {
			DependencyPropertyRegistrator<NonVisualDecorator>.New()
				.RegisterReadOnly(d => d.ActualChild, out ActualChildPropertyKey, out ActualChildProperty, null, (d, e) => d.OnActualChildChanged(e))
			;
		}
		public UIElement ActualChild {
			get { return (UIElement)GetValue(ActualChildProperty); }
			private set { SetValue(ActualChildPropertyKey, value); }
		}
		protected virtual void OnActualChildChanged(DependencyPropertyChangedEventArgs e) { }
		void IAddChild.AddChild(object value) {
			var uiElement = GuardHelper.ArgumentMatchType<UIElement>(value, "value");
			if(Child != null)
				throw new ArgumentException("", "value");
			Child = uiElement;
		}
		void IAddChild.AddText(string text) { }
		[DefaultValue(null)]
		public virtual UIElement Child {
			get { return ActualChild; }
			set {
				var actualChild = ActualChild;
				if(actualChild == value) return;
				RemoveLogicalChild(actualChild);
				AddLogicalChild(value);
				ActualChild = value;
			}
		}
		protected override IEnumerator LogicalChildren {
			get {
				return new SingleLogicalChildEnumerator(ActualChild);
			}
		}
		protected override int VisualChildrenCount { get { return 0; } }
		protected override Visual GetVisualChild(int index) { throw new ArgumentOutOfRangeException("index"); }
		protected override Size MeasureOverride(Size constraint) { return new Size(); }
		protected override Size ArrangeOverride(Size arrangeSize) { return arrangeSize; }
	}
	public class DialogFooter : NonVisualDecorator {
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty ActualDialogWindowProperty;
		static DialogFooter() {
			DependencyPropertyRegistrator<DialogFooter>.New()
				.AddOwner(d => d.ActualDialogWindow, out ActualDialogWindowProperty, DXDialogWindow.ActualDialogWindowProperty, null, (d, e) => d.OnActualDialogWindowChanged(e))
			;
		}
		public DXDialogWindow ActualDialogWindow { get { return (DXDialogWindow)GetValue(ActualDialogWindowProperty); } }
		void OnActualDialogWindowChanged(DependencyPropertyChangedEventArgs e) {
			var oldValue = (DXDialogWindow)e.OldValue;
			if(oldValue != null && ActualChild != null)
				oldValue.RemoveCustomFooter(ActualChild);
			var newValue = (DXDialogWindow)e.NewValue;
			if(newValue != null && ActualChild != null)
				newValue.AddCustomFooter(ActualChild);
		}
		protected override void OnActualChildChanged(DependencyPropertyChangedEventArgs e) {
			base.OnActualChildChanged(e);
			var oldChild = (UIElement)e.OldValue;
			var newChild = (UIElement)e.NewValue;
			if(oldChild != null && ActualDialogWindow != null)
				ActualDialogWindow.RemoveCustomFooter(oldChild);
			if(newChild != null && ActualDialogWindow != null)
				ActualDialogWindow.AddCustomFooter(newChild);
		}
	}
}
