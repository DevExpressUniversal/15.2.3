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

using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Printing;
using DevExpress.Xpf.Utils;
using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
#if SL
using DevExpress.WPFToSLUtils;
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Editors.WPFCompatibility;
using DevExpress.Xpf.Editors.WPFCompatibility.Extensions;
using Popup = DevExpress.Xpf.Core.SLPopup;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
#endif
namespace DevExpress.Xpf.Editors {
	public enum ShowLoadDialogOnClickMode { Never, Always, Empty }
	public enum ShowMenuMode { Always, Hover }
	[DXToolboxBrowsable(DXToolboxItemKind.Free)]
	public class ImageEdit : BaseEdit, IImageEdit, IImageExportSettings {
		#region static
		public static readonly DependencyProperty ShowMenuProperty;
		public static readonly DependencyProperty ShowMenuModeProperty;
		public static readonly DependencyProperty SourceProperty;
		public static readonly DependencyProperty StretchProperty;
		public static readonly DependencyProperty HasImageProperty;
		static readonly DependencyPropertyKey HasImagePropertyKey;
		public static readonly DependencyProperty EmptyContentTemplateProperty;
		public static readonly DependencyProperty ShowLoadDialogOnClickModeProperty;
		public static readonly DependencyProperty MenuTemplateProperty;
		public static readonly DependencyProperty MenuContainerTemplateProperty;
		public static readonly DependencyProperty ImageEffectProperty;
		public static readonly RoutedEvent ConvertEditValueEvent;
#if SL
		public static readonly RoutedCommand OpenCommand;
		public static readonly RoutedCommand ClearCommand;
#endif
		static ImageEdit() {
			Type ownerType = typeof(ImageEdit);
			SourceProperty = DependencyPropertyManager.Register("Source", typeof(ImageSource), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, OnSourceChanged, CoerceSource));
			ShowMenuProperty = DependencyPropertyManager.Register("ShowMenu", typeof(bool), ownerType, new FrameworkPropertyMetadata(true, (obj, baseValue) => { ((ImageEdit)obj).OnShowMenuChanged(); }, CoerceShowMenu));
			ShowMenuModeProperty = DependencyPropertyManager.Register("ShowMenuMode", typeof(ShowMenuMode), ownerType, new FrameworkPropertyMetadata(ShowMenuMode.Hover, (obj, baseValue) => { ((ImageEdit)obj).OnShowMenuModeChanged(); }));
			StretchProperty = DependencyPropertyManager.Register("Stretch", typeof(Stretch), ownerType, new FrameworkPropertyMetadata(Stretch.Uniform, FrameworkPropertyMetadataOptions.AffectsMeasure));
			HasImagePropertyKey = DependencyPropertyManager.RegisterReadOnly("HasImage", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
			HasImageProperty = HasImagePropertyKey.DependencyProperty;
			EmptyContentTemplateProperty = DependencyPropertyManager.Register("EmptyContentTemplate", typeof(ControlTemplate), ownerType, new FrameworkPropertyMetadata(null));
			ShowLoadDialogOnClickModeProperty = DependencyPropertyManager.Register("ShowLoadDialogOnClickMode", typeof(ShowLoadDialogOnClickMode), ownerType, new FrameworkPropertyMetadata(ShowLoadDialogOnClickMode.Empty));
			MenuTemplateProperty = DependencyPropertyManager.Register("MenuTemplate", typeof(ControlTemplate), ownerType, new FrameworkPropertyMetadata(null));
			MenuContainerTemplateProperty = DependencyPropertyManager.Register("MenuContainerTemplate", typeof(ControlTemplate), ownerType, new FrameworkPropertyMetadata(null));
			ImageEffectProperty = DependencyPropertyManager.Register("ImageEffect", typeof(Effect), ownerType, new FrameworkPropertyMetadata(null));
			ConvertEditValueEvent = EventManager.RegisterRoutedEvent("ConvertEditValue", RoutingStrategy.Direct, typeof(ConvertEditValueEventHandler), ownerType);
#if !SL
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(ApplicationCommands.Cut, (d, e) => ((ImageEdit)d).Cut(), (d, e) => ((ImageEdit)d).CanCut(d, e)));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(ApplicationCommands.Delete, (d, e) => ((ImageEdit)d).Clear(), (d, e) => ((ImageEdit)d).CanDelete(d, e)));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(ApplicationCommands.Copy, (d, e) => ((ImageEdit)d).Copy(), (d, e) => ((ImageEdit)d).CanCopy(d, e)));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(ApplicationCommands.Paste, (d, e) => ((ImageEdit)d).Paste(), (d, e) => ((ImageEdit)d).CanPaste(d, e)));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(ApplicationCommands.Open, (d, e) => ((ImageEdit)d).Load(), (d, e) => ((ImageEdit)d).CanLoad(d, e)));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(ApplicationCommands.Save, (d, e) => ((ImageEdit)d).Save(), (d, e) => ((ImageEdit)d).CanSave(d, e)));
#else 
			OpenCommand = new RoutedCommand("Open", ownerType);
			ClearCommand = new RoutedCommand("Clear", ownerType);
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(OpenCommand, (d, e) => ((ImageEdit)d).Load(), null));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(ClearCommand, (d, e) => ((ImageEdit)d).Clear(), null));
#endif
		}
		static object CoerceSource(DependencyObject d, object value) {
			return ((ImageEdit)d).CoerceSource((ImageSource)value);
		}
		static object CoerceShowMenu(DependencyObject d, object value) {
			return ((ImageEdit)d).CoerceShowMenu((bool)value);
		}
		static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ImageEdit)d).OnSourceChanged((ImageSource)e.OldValue, (ImageSource)e.NewValue);
		}
		#endregion
		DispatcherTimer mouseLeaveTimer;
		MenuContentControl menuPopupContainerControl;
		public ImageEdit() {
			this.SetDefaultStyleKey(typeof(ImageEdit));
			mouseLeaveTimer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(100) };
			mouseLeaveTimer.Tick += new EventHandler(OnLeaveTimerTick);
		}
		#region public properties
#if !SL
	[DevExpressXpfCoreLocalizedDescription("ImageEditShowMenu")]
#endif
public bool ShowMenu {
			get { return (bool)GetValue(ShowMenuProperty); }
			set { SetValue(ShowMenuProperty, value); }
		}
		public ShowMenuMode ShowMenuMode {
			get { return (ShowMenuMode)GetValue(ShowMenuModeProperty); }
			set { SetValue(ShowMenuModeProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("ImageEditSource")]
#endif
public ImageSource Source {
			get { return (ImageSource)GetValue(SourceProperty); }
			set { base.SetValue(SourceProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("ImageEditStretch")]
#endif
public Stretch Stretch {
			get { return (Stretch)GetValue(StretchProperty); }
			set { SetValue(StretchProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("ImageEditHasImage")]
#endif
public bool HasImage {
			get { return (bool)GetValue(HasImageProperty); }
			private set { this.SetValue(HasImagePropertyKey, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("ImageEditEmptyContentTemplate")]
#endif
public ControlTemplate EmptyContentTemplate {
			get { return (ControlTemplate)GetValue(EmptyContentTemplateProperty); }
			set { SetValue(EmptyContentTemplateProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("ImageEditImageEffect")]
#endif
public Effect ImageEffect {
			get { return (Effect)GetValue(ImageEffectProperty); }
			set { SetValue(ImageEffectProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("ImageEditShowLoadDialogOnClickMode")]
#endif
public ShowLoadDialogOnClickMode ShowLoadDialogOnClickMode {
			get { return (ShowLoadDialogOnClickMode)GetValue(ShowLoadDialogOnClickModeProperty); }
			set { SetValue(ShowLoadDialogOnClickModeProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("ImageEditMenuTemplate")]
#endif
public ControlTemplate MenuTemplate {
			get { return (ControlTemplate)GetValue(MenuTemplateProperty); }
			set { SetValue(MenuTemplateProperty, value); }
		}
		[Browsable(false)]
		public ControlTemplate MenuContainerTemplate {
			get { return (ControlTemplate)GetValue(MenuContainerTemplateProperty); }
			set { SetValue(MenuContainerTemplateProperty, value); }
		}
		#endregion
		#region events 
		public event ConvertEditValueEventHandler ConvertEditValue {
			add { this.AddHandler(ConvertEditValueEvent, value); }
			remove { this.RemoveHandler(ConvertEditValueEvent, value); }
		}
		#endregion
		protected internal Image Image { get { return EditCore as Image; } }
		protected internal Popup MenuPopup { get; private set; }
		protected new ImageEditStrategy EditStrategy { get { return base.EditStrategy as ImageEditStrategy; } }
		protected internal IImageEdit PopupOwnerEdit { get { return PopupBaseEdit.GetPopupOwnerEdit(this) as IImageEdit; } }
		protected internal new ImageEditSettings Settings { get { return base.Settings as ImageEditSettings; } }
		protected MenuContentControl MenuPopupContentControl {
			get { return menuPopupContainerControl; }
			private set {
				if(MenuPopupContentControl != null)
					MenuPopupContentControl.MouseLeave -= new MouseEventHandler(OnPopupContaierControlMouseLeave);
				menuPopupContainerControl = value;
				if(MenuPopupContentControl != null)
					MenuPopupContentControl.MouseLeave += new MouseEventHandler(OnPopupContaierControlMouseLeave);
			}
		}
		new protected internal ImageEditPropertyProvider PropertyProvider { get { return base.PropertyProvider as ImageEditPropertyProvider; } }
		#region API
#if !SL
		public virtual void Copy() {
			if(HasImage && !BrowserInteropHelper.IsBrowserHosted)
				CopyCore();
		}
		protected virtual void CopyCore() {
			try {
				Clipboard.SetImage(ImageLoader.GetSafeBitmapSource((BitmapSource)Source, ImageEffect));
			}
			catch {			 
			}
		}
		public virtual void Cut() {
			Copy();
			Clear();
		}
		public virtual void Paste() {
			if(!BrowserInteropHelper.IsBrowserHosted)
				PasteCore();
		}
		protected virtual void PasteCore() {
			if (IsReadOnly)
				return;
			try {
				using(MemoryStream memoryStream = new MemoryStream(ImageLoader.ImageToByteArray(Clipboard.GetImage()))) {
					Source = ImageHelper.CreateImageFromStream(memoryStream);
				}
			}
			catch { 
			}
		}
		public void Save() {
			HideMenuPopup();
			if(IsInactiveMode) return;
			if(PopupOwnerEdit != null)
				PopupOwnerEdit.Save();
			else
				SaveCore();
		}
		protected virtual void SaveCore() {
			if(!HasImage)
				return;
			BitmapSource source = ImageLoader.GetSafeBitmapSource((BitmapSource)Source, ImageEffect);
			ImageLoader.SaveImage(source);
		}
		protected virtual void CanCut(object sender, CanExecuteRoutedEventArgs e) {
			e.CanExecute = IsEnabled && !IsReadOnly && HasImage && !BrowserInteropHelper.IsBrowserHosted;
		}
		protected virtual void CanCopy(object sender, CanExecuteRoutedEventArgs e) {
			e.CanExecute = HasImage && !BrowserInteropHelper.IsBrowserHosted;
		}
		protected virtual void CanDelete(object sender, CanExecuteRoutedEventArgs e) {
			e.CanExecute = IsEnabled && !IsReadOnly && HasImage && !BrowserInteropHelper.IsBrowserHosted;
		}
		protected virtual void CanPaste(object sender, CanExecuteRoutedEventArgs e) {
			if(BrowserInteropHelper.IsBrowserHosted)
				e.CanExecute = false;
			else
				e.CanExecute = !IsReadOnly && ClipboardContainsImage();
		}
		bool ClipboardContainsImage() {
			return Clipboard.ContainsImage();
		}
		protected virtual void CanLoad(object sender, CanExecuteRoutedEventArgs e) {
			e.CanExecute = CanLoadCore() && !BrowserInteropHelper.IsBrowserHosted;
		}
		protected virtual void CanSave(object sender, CanExecuteRoutedEventArgs e) {
			e.CanExecute = CanSaveCore() && !BrowserInteropHelper.IsBrowserHosted;
		}
		protected virtual bool CanSaveCore() {
			return HasImage;
		}
#endif
		public virtual void Clear() {
			SetCurrentValue(SourceProperty, null);
		}
		public void Load() {
			HideMenuPopup();
			if(IsInactiveMode) return;
			if(PopupOwnerEdit != null)
				PopupOwnerEdit.Load();
			else
				LoadCore();
		}
		protected virtual void LoadCore() {
			if(Image == null)
				return;
			ImageSource image = ImageLoader.LoadImage();
			if(image != null)
				EditStrategy.SetImage(image);
		}
		#endregion
#if SL
		protected Grid ContainerElement { get; private set; }
		protected override void OnEditCoreAssigned() {
			base.OnEditCoreAssigned();
			ContainerElement = LayoutHelper.FindElementByName(this, "PART_Container") as Grid;
		}
		protected void AddToContainer(FrameworkElement element) {
			if(ContainerElement != null)
				ContainerElement.Children.Add(element);
		}
		protected void RemoveFromContainer(FrameworkElement element) {
			if(ContainerElement != null)
				ContainerElement.Children.Remove(element);
		}
#endif
		protected virtual bool CanLoadCore() {
			return true;
		}
		protected override EditStrategyBase CreateEditStrategy() {
			return new ImageEditStrategy(this);
		}
		protected override ActualPropertyProvider CreateActualPropertyProvider() {
			return new ImageEditPropertyProvider(this);
		}
		protected virtual object CoerceSource(ImageSource value) {
			return EditStrategy.CoerceSource(value);
		}
		protected virtual object CoerceShowMenu(bool value) {
			return value;
		}
		protected virtual void OnSourceChanged(ImageSource oldValue, ImageSource newValue) {
			HasImage = newValue != null;
			EditStrategy.OnSourceChanged(oldValue, newValue);
		}
		protected virtual void OnShowMenuChanged() {
			PropertyProvider.UpdateInplaceMenuVisibility();
		}
		protected virtual void OnShowMenuModeChanged() {
			PropertyProvider.UpdateInplaceMenuVisibility();
		}
		protected internal override BaseEditSettings CreateEditorSettings() {
			return new ImageEditSettings();
		}
		protected virtual void UpdateMenuPosition() {
			if(MenuPopup == null || MenuPopupContentControl == null) return;
			const double defaultOffset = 12;
			Size size = GetActualSize();
			MenuPopup.HorizontalOffset = (size.Width - MenuPopupContentControl.ActualWidth) / 2;
			MenuPopup.VerticalOffset = size.Height - MenuPopupContentControl.ActualHeight - defaultOffset;
		}
		protected virtual Size GetActualSize() {
			UIElement root;
			System.Windows.Controls.Primitives.Popup popup = LayoutHelper.FindParentObject<System.Windows.Controls.Primitives.Popup>(this);
			if (popup != null)
				root = popup.Child;
			else
				root = LayoutHelper.FindRoot(this) as UIElement;
			GeneralTransform transformToRoot = this.TransformToVisual(root);
			Rect screenRect = new Rect(transformToRoot.Transform(new Point(0, 0)), transformToRoot.Transform(new Point(this.ActualWidth, this.ActualHeight)));
			DependencyObject parent = VisualTreeHelper.GetParent(this);
			while (parent != null) {
				FrameworkElement parentElement = parent as FrameworkElement;
				if (parent != null && parentElement != null) {
					transformToRoot = parentElement.TransformToVisual(root);
					Point pointAncestorTopLeft = transformToRoot.Transform(new Point(0, 0));
					Point pointAncestorBottomRight = transformToRoot.Transform(new Point(parentElement.ActualWidth, parentElement.ActualHeight));
					Rect ancestorRect = new Rect(pointAncestorTopLeft, pointAncestorBottomRight);
					screenRect.Intersect(ancestorRect);
				}
				parent = VisualTreeHelper.GetParent(parent);
			}
			return screenRect.Size();
		}
		protected void EnsureMenuPopup() {
			if(MenuPopup == null)
				MenuPopup = new Popup();
			MenuPopupContentControl = new MenuContentControl() { Template = MenuContainerTemplate };
			ContentControl menuContentControl = new ContentControl() { Template = MenuTemplate };
			ImageEdit.SetOwnerEdit(menuContentControl, this);
			if (PopupOwnerEdit != null)
				PopupBaseEdit.SetPopupOwnerEdit(menuContentControl, (PopupImageEdit)PopupOwnerEdit);
			MenuPopupContentControl.Content = menuContentControl;
			MenuPopup.Child = MenuPopupContentControl;
			InitMenuPopup();
		}
		protected virtual void InitMenuPopup() {
			MenuPopup.PlacementTarget = this;
#if !SL
			MenuPopup.Focusable = false;
			MenuPopup.Placement = PlacementMode.Relative;
			MenuPopup.AllowsTransparency = true;
			MenuPopup.PopupAnimation = PopupAnimation.Fade;
#else 
			MenuPopup.Placement2 = PlacementMode2.Relative;
#endif
		}
		internal protected void ShowMenuPopup() {
			if (CanShowMenuPopup)
				ShowMenuPopupInternal();
		}
		protected bool CanShowMenuPopup {
			get { return MenuPopup == null && ShowMenu && ShowMenuMode == ShowMenuMode.Hover; }
		}
		void ShowMenuPopupInternal() {
			if (IsInactiveMode || IsReadOnly)
				return;
			EnsureMenuPopup();
#if SL
			AddToContainer(MenuPopup);
#endif
			MenuPopup.IsOpen = true;
			MenuPopup.UpdateLayout();
			UpdateMenuPosition();
		}
		protected internal void HideMenuPopup() {
			if(MenuPopup == null) return;
			MenuPopup.IsOpen = false;
#if SL
			RemoveFromContainer(MenuPopup);
#endif
			MenuPopupContentControl = null;
			MenuPopup = null;
		}
		protected override void OnMouseEnter(MouseEventArgs e) {
			ShowMenuPopup();
			base.OnMouseEnter(e);
		}
		protected override void OnMouseLeave(MouseEventArgs e) {
			StartLeaveTimer();
			base.OnMouseLeave(e);
		}
		void OnPopupContaierControlMouseLeave(object sender, MouseEventArgs e) {
			StartLeaveTimer();
		}
		void StartLeaveTimer() {
			if(MenuPopup != null)
				mouseLeaveTimer.Start();
		}
		void OnLeaveTimerTick(object sender, EventArgs e) {
			if(MenuPopup != null && !MenuPopupContentControl.IsMouseOver && !IsMouseOver)
				HideMenuPopup();
			mouseLeaveTimer.Stop();
		}
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
			base.OnMouseLeftButtonDown(e);
			if(EditMode != Editors.EditMode.InplaceInactive)
				DoShowLoadDialogOnClick();
		}
		protected void DoShowLoadDialogOnClick() {
			if(IsReadOnly) return;
			if (ShowLoadDialogOnClickMode == ShowLoadDialogOnClickMode.Empty && !HasImage || ShowLoadDialogOnClickMode == ShowLoadDialogOnClickMode.Always) {
				if (Stylus.DirectlyOver == null || LayoutHelper.IsChildElement(this, Stylus.DirectlyOver as DependencyObject))
					Load();
			}
		}
		protected virtual object GetDataFromImageCore(ImageSource source) {
			ConvertEditValueEventArgs args = new ConvertEditValueEventArgs(ImageEdit.ConvertEditValueEvent) { ImageSource = source };
			RaiseEvent(args);
			Settings.RaiseConvertEditValue(this, args);
			if(args.Handled)
				return args.EditValue;
			return source;
		}
		object IImageEdit.GetDataFromImage(ImageSource source) {
			if(PopupOwnerEdit != null) 
				return PopupOwnerEdit.GetDataFromImage(source);
			return GetDataFromImageCore(source);
		}
		#region IImageExportSettings Members
		FrameworkElement IImageExportSettings.SourceElement { get { return Image; } }
		ImageRenderMode IImageExportSettings.ImageRenderMode { get { return ImageRenderMode.UseImageSource; } }
		bool IImageExportSettings.ForceCenterImageMode { get { return false; } }
		object IImageExportSettings.ImageKey { get { return null; } }
		#endregion
	}
	[DXToolboxBrowsable(DXToolboxItemKind.Free)]
	public partial class PopupImageEdit : PopupBaseEdit, IImageEdit {
		public const double DefaultPopupMinHeight = 200d;
		public const double DefaultPopupMinWidth = 200d;
		#region static
		public static readonly DependencyProperty ShowMenuProperty;
		public static readonly DependencyProperty ShowMenuModeProperty;
		public static readonly DependencyProperty SourceProperty;
		public static readonly DependencyProperty StretchProperty;
		public static readonly DependencyProperty HasImageProperty;
		static readonly DependencyPropertyKey HasImagePropertyKey;
		public static readonly DependencyProperty EmptyContentTemplateProperty;
		public static readonly DependencyProperty ShowLoadDialogOnClickModeProperty;
		public static readonly DependencyProperty MenuTemplateProperty;
		public static readonly DependencyProperty MenuContainerTemplateProperty;
		public static readonly DependencyProperty AutoSizePopupProperty;
		public static readonly DependencyProperty ImageEffectProperty;
		public static readonly RoutedEvent ConvertEditValueEvent;
#if !SL
		public static readonly RoutedEvent ImageFailedEvent;
#endif
		public static readonly RoutedCommand OKCommand;
		public static readonly RoutedCommand CancelCommand;
		static PopupImageEdit() {
			Type ownerType = typeof(PopupImageEdit);
			ShowMenuProperty = ImageEdit.ShowMenuProperty.AddOwner(ownerType);
			ShowMenuModeProperty = ImageEdit.ShowMenuModeProperty.AddOwner(ownerType);
			SourceProperty = ImageEdit.SourceProperty.AddOwner(ownerType, new FrameworkPropertyMetadata(null, OnSourcePropertyChanged, CoerceSource));
			StretchProperty = ImageEdit.StretchProperty.AddOwner(ownerType);
			HasImagePropertyKey = DependencyPropertyManager.RegisterReadOnly("HasImage", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
			HasImageProperty = HasImagePropertyKey.DependencyProperty;
			EmptyContentTemplateProperty = ImageEdit.EmptyContentTemplateProperty.AddOwner(ownerType);
			ShowLoadDialogOnClickModeProperty = ImageEdit.ShowLoadDialogOnClickModeProperty.AddOwner(ownerType);
			MenuTemplateProperty = ImageEdit.MenuTemplateProperty.AddOwner(ownerType);
			MenuContainerTemplateProperty = ImageEdit.MenuContainerTemplateProperty.AddOwner(ownerType);
			ImageEffectProperty = ImageEdit.ImageEffectProperty.AddOwner(ownerType);
			AutoSizePopupProperty = DependencyPropertyManager.Register("AutoSizePopup", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
			ConvertEditValueEvent = EventManager.RegisterRoutedEvent("ConvertEditValue", RoutingStrategy.Direct, typeof(ConvertEditValueEventHandler), ownerType);
			OKCommand = new RoutedCommand("OK", ownerType);
			CancelCommand = new RoutedCommand("Cancel", ownerType);
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(OKCommand, (d, e) => ((PopupImageEdit)d).ClosePopup(), null));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(CancelCommand, (d, e) => ((PopupImageEdit)d).CancelPopup(), null));
#if !SL
			ImageFailedEvent = EventManager.RegisterRoutedEvent("ImageFailed", RoutingStrategy.Direct, typeof(ImageFailedEventHandler), ownerType);
			IsTextEditableProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(false));
			PopupFooterButtonsProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(DevExpress.Xpf.Editors.PopupFooterButtons.None));
			ShowSizeGripProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(true));
			PopupMinHeightProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(DefaultPopupMinHeight));
			PopupMinWidthProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(DefaultPopupMinWidth));
#endif
		}
		static object CoerceSource(DependencyObject d, object value) {
			return ((PopupImageEdit)d).CoerceSource((ImageSource)value);
		}
		static void OnSourcePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((PopupImageEdit)obj).OnSourcePropertyChanged(e);
		}
		#endregion
		public PopupImageEdit() {
			this.SetDefaultStyleKey(typeof(PopupImageEdit));
#if SL
			PopupMinHeight = DefaultPopupMinHeight;
#endif
		}
		protected internal override FrameworkElement PopupElement {
			get { return ImageEditControl; }
		}
		protected internal ImageEdit ImageEditControl { get; private set; }
		protected internal new PopupImageEditSettings Settings { get { return base.Settings as PopupImageEditSettings; } }
		protected new PopupImageEditStrategy EditStrategy {
			get { return base.EditStrategy as PopupImageEditStrategy; }
			set { base.EditStrategy = value; }
		}
		#region public properties
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PopupImageEditSource")]
#endif
public ImageSource Source {
			get { return (ImageSource)GetValue(SourceProperty); }
			set { SetValue(SourceProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PopupImageEditStretch")]
#endif
public Stretch Stretch {
			get { return (Stretch)GetValue(StretchProperty); }
			set { SetValue(StretchProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PopupImageEditShowMenu")]
#endif
public bool ShowMenu {
			get { return (bool)GetValue(ShowMenuProperty); }
			set { SetValue(ShowMenuProperty, value); }
		}
		public ShowMenuMode ShowMenuMode {
			get { return (ShowMenuMode)GetValue(ShowMenuModeProperty); }
			set { SetValue(ShowMenuModeProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PopupImageEditHasImage")]
#endif
public bool HasImage {
			get { return (bool)GetValue(HasImageProperty); }
			protected internal set { this.SetValue(HasImagePropertyKey, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PopupImageEditEmptyContentTemplate")]
#endif
public ControlTemplate EmptyContentTemplate {
			get { return (ControlTemplate)GetValue(EmptyContentTemplateProperty); }
			set { SetValue(EmptyContentTemplateProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PopupImageEditImageEffect")]
#endif
public Effect ImageEffect {
			get { return (Effect)GetValue(ImageEffectProperty); }
			set { SetValue(ImageEffectProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PopupImageEditShowLoadDialogOnClickMode")]
#endif
public ShowLoadDialogOnClickMode ShowLoadDialogOnClickMode {
			get { return (ShowLoadDialogOnClickMode)GetValue(ShowLoadDialogOnClickModeProperty); }
			set { SetValue(ShowLoadDialogOnClickModeProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PopupImageEditMenuTemplate")]
#endif
public ControlTemplate MenuTemplate {
			get { return (ControlTemplate)GetValue(MenuTemplateProperty); }
			set { SetValue(MenuTemplateProperty, value); }
		}
		[Browsable(false)]
		public ControlTemplate MenuContainerTemplate {
			get { return (ControlTemplate)GetValue(MenuContainerTemplateProperty); }
			set { SetValue(MenuContainerTemplateProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PopupImageEditAutoSizePopup")]
#endif
public bool AutoSizePopup {
			get { return (bool)GetValue(AutoSizePopupProperty); }
			set { SetValue(AutoSizePopupProperty, value); }
		}
		#endregion
		#region events
		public event ConvertEditValueEventHandler ConvertEditValue {
			add { this.AddHandler(ConvertEditValueEvent, value); }
			remove { this.RemoveHandler(ConvertEditValueEvent, value); }
		}
#if !SL
		public event ImageFailedEventHandler ImageFailed;
#endif
		#endregion
		protected internal override BaseEditSettings CreateEditorSettings() {
			return new PopupImageEditSettings();
		}
		protected override EditStrategyBase CreateEditStrategy() {
			return new PopupImageEditStrategy(this);
		}
		protected override ActualPropertyProvider CreateActualPropertyProvider() {
			return new PopupImageEditPropertyProvider(this);
		}
		protected ImageEdit GetImageEditControl() {
			return LayoutHelper.FindElement(Popup.Child as FrameworkElement, (FrameworkElement element) => { return element is ImageEdit && element.Name == "PART_PopupContent"; }) as ImageEdit;
		}
		protected virtual void OnSourcePropertyChanged(DependencyPropertyChangedEventArgs e) {
			HasImage = e.NewValue != null;
			EditStrategy.SourceChanged((ImageSource)e.OldValue, (ImageSource)e.NewValue);
		}
		protected virtual object CoerceSource(ImageSource value) {
			return EditStrategy.CoerceSource(value);
		}
		protected override void OnPopupOpened() {
			ImageEditControl = GetImageEditControl();
			base.OnPopupOpened();
#if !SL
			SetInitialPopupSize();
#endif
			EditStrategy.SyncWithValue();
		}
		protected override void OnPopupClosed() {
			base.OnPopupClosed();
			if(ImageEditControl != null)
				ImageEditControl.HideMenuPopup();
			ImageEditControl = null;
		}
		protected override void AcceptPopupValue() {
			base.AcceptPopupValue();
			EditStrategy.AcceptPopupValue();
		}
		void SetInitialPopupSize() {
#if !SL
			try {
				if(double.IsNaN(ActualPopupWidth) || double.IsNaN(ActualPopupHeight)) {
					if(AutoSizePopup && Source != null && Source.Width > 0 && Source.Height > 0) {
						double ratio = 0, ratioW = 0, rationH = 0;
						ratioW = Math.Max(PopupMinWidth, ActualWidth) / Source.Width;
						rationH = PopupMinHeight / Source.Height;
						ratio = rationH < ratioW ? rationH : ratioW;
						ActualPopupWidth = (Math.Max(PopupMinWidth, (Source.Width * ratio)));
						ActualPopupHeight = (Math.Max(PopupMinHeight, (Source.Height * ratio)));
					}
				}
			}
			catch(Exception exception) {
				SetCurrentValue(SourceProperty, null);
				RaiseImageFailed(exception);
			}
#else
			if(double.IsNaN(PopupWidth))
				PopupWidth = ActualWidth;
			if(double.IsNaN(PopupHeight))
				PopupHeight = PopupMinHeight;
#endif
		}
		protected internal override bool ShouldApplyPopupSize {
			get { return !AutoSizePopup; }
		}
		protected virtual object GetDataFromImageCore(ImageSource source) {
			ConvertEditValueEventArgs args = new ConvertEditValueEventArgs(PopupImageEdit.ConvertEditValueEvent) { ImageSource = source };
			RaiseEvent(args);
			Settings.RaiseConvertEditValue(this, args);
			if(args.Handled)
				return args.EditValue;
			return source;
		}
#if !SL
		internal protected virtual void UpdateBaseUri() {
			Source.Do(x => ImageHelper.UpdateBaseUri(this, x));
		}
		void RaiseImageFailed(Exception exception) {
			var args = new ImageFailedEventArgs(ImageFailedEvent, this, exception);
			RaiseEvent(args);
			if (ImageFailed != null)
				ImageFailed(this, args);
		}
#endif
		#region IImageLoader Members
		void IImageEdit.Load() {
			CancelPopup();
			ImageSource image = ImageLoader.LoadImage();
			if(image != null) {
				Source = image;
				EditStrategy.UpdateDisplayText();
			}
		}
		object IImageEdit.GetDataFromImage(ImageSource source) {
			return GetDataFromImageCore(source);
		}
#if !SL
		void IImageEdit.Save() {
			CancelPopup();
			if(Source != null) {
				ImageLoader.SaveImage(ImageLoader.GetSafeBitmapSource((BitmapSource)Source, ImageEffect));
			}
		}
#endif
		#endregion
	}
	public class PopupImageEditPropertyProvider : PopupBaseEditPropertyProvider {
		public PopupImageEditPropertyProvider(PopupImageEdit editor) : base(editor) {
		}
		public override bool CalcSuppressFeatures() {
			return false;
		}
	}
	public class ConvertEditValueEventArgs : RoutedEventArgs {
		public ConvertEditValueEventArgs(RoutedEvent routedEvent) : base(routedEvent) { }
		public ImageSource ImageSource { get; internal set; }
		public object EditValue { get; set; }
	}
	public class ImageFailedEventArgs : RoutedEventArgs {
		public ImageFailedEventArgs(RoutedEvent routedEvent, object sender, Exception errorException) : base(routedEvent, sender) {
			ErrorException = errorException;
		}
		public Exception ErrorException { get; protected set; }
	}
	public delegate void ConvertEditValueEventHandler(DependencyObject sender, ConvertEditValueEventArgs args);
	public delegate void ImageFailedEventHandler(DependencyObject sender, ImageFailedEventArgs args);
	public class BytesToImageSourceConverter : IValueConverter {
		#region IValueConverter Members
		public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			byte[] bytes = value as byte[];
			if(bytes == null)
				return null;
			try {
				using(MemoryStream stream = new MemoryStream(bytes)) {
					return ImageHelper.CreateImageFromStream(stream);
				}
			}
			catch {
				return null;
			}
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
	}
	public class ImageEditPropertyProvider : ActualPropertyProvider {
		public ImageEditPropertyProvider(BaseEdit editor) : base(editor) { }
		public static readonly DependencyProperty InplaceMenuVisibilityProperty;
		static ImageEditPropertyProvider() {
			Type ownerType = typeof(ImageEditPropertyProvider);
			InplaceMenuVisibilityProperty = DependencyPropertyManager.Register("InplaceMenuVisibility", typeof(Visibility), ownerType, new FrameworkPropertyMetadata(Visibility.Collapsed));
		}
		new public ImageEdit Editor { get { return base.Editor as ImageEdit; } }
		public Visibility InplaceMenuVisibility {
			get { return (Visibility)GetValue(InplaceMenuVisibilityProperty); }
			set { SetValue(InplaceMenuVisibilityProperty, value); }
		}
		public void UpdateInplaceMenuVisibility() {
			InplaceMenuVisibility = Editor.ShowMenu && Editor.ShowMenuMode == ShowMenuMode.Always ? Visibility.Visible : Visibility.Collapsed;
		}
		public override bool CalcSuppressFeatures() {
			return false;
		}
	}
}
