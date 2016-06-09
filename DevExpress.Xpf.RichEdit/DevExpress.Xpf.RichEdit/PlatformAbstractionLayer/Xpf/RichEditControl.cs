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
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Security;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using DevExpress.Office.Drawing;
using DevExpress.Office.Forms;
using DevExpress.Office.Internal;
using DevExpress.Office.Model;
using DevExpress.Office.Services;
using DevExpress.Office.Services.Implementation;
using DevExpress.Office.Utils;
using DevExpress.Services;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Menu;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.XtraRichEdit.Drawing;
using DevExpress.XtraRichEdit.Export;
using DevExpress.XtraRichEdit.Export.Rtf;
using DevExpress.XtraRichEdit.Forms;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Internal.PrintLayout;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.XtraRichEdit.Menu;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Mouse;
using DevExpress.XtraRichEdit.Services;
using DevExpress.XtraRichEdit.Services.Implementation;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Printing;
using DevExpress.XtraRichEdit.Tables.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Bars.Native;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Utils.Themes;
using DevExpress.Xpf.Ribbon;
using DevExpress.Xpf.RichEdit;
using DevExpress.Xpf.RichEdit.Menu;
using DevExpress.Xpf.RichEdit.UI;
using DevExpress.Xpf.RichEdit.Controls.Internal;
using DevExpress.Xpf.RichEdit.Views;
using DevExpress.Xpf.RichEdit.Localization;
using KeyboardHandlerClass = DevExpress.Utils.KeyboardHandler.KeyboardHandler;
using ImageHelper = DevExpress.Xpf.Core.Native.ImageHelper;
using DevExpress.Xpf.Office.UI;
using DevExpress.Xpf.RichEdit.Themes;
using DevExpress.Office.Layout;
using DevExpress.XtraRichEdit.Keyboard;
#if SL
using PlatformIndependentMouseEventArgs = DevExpress.Data.MouseEventArgs;
using PlatformIndependentKeyEventArgs = DevExpress.Data.KeyEventArgs;
using PlatformIndependentKeyPressArgs = DevExpress.Data.KeyPressEventArgs;
using PlatformIndependentDragEventArgs = DevExpress.Utils.DragEventArgs;
using PlatformIndependentDragDropEffects = DevExpress.Utils.DragDropEffects;
using PlatformIndependentMouseButtons = DevExpress.Data.MouseButtons;
using PlatformIndependentCursor = System.Windows.Input.Cursor;
using PlatformIndependentColor = System.Windows.Media.Color;
using PlatformIndependentFontStyle = DevExpress.Xpf.Drawing.FontStyle;
using PlatformIWin32Window = DevExpress.Xpf.Core.Native.IWin32Window;
using PlatformColor = System.Windows.Media.Color;
using PlatformBrush = System.Windows.Media.Brush;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using DevExpress.Xpf.Core.WPFCompatibility;
using DXKeys = DevExpress.Data.Keys;
using EmptyDelegate = System.Action;
using PlatformDialogResult = DevExpress.Xpf.Core.DialogResult;
using DevExpress.Xpf.Drawing;
#else
using PlatformIndependentMouseEventArgs = System.Windows.Forms.MouseEventArgs;
using PlatformIndependentKeyEventArgs = System.Windows.Forms.KeyEventArgs;
using PlatformIndependentKeyPressArgs = System.Windows.Forms.KeyPressEventArgs;
using PlatformIndependentDragEventArgs = System.Windows.Forms.DragEventArgs;
using PlatformIndependentDragDropEffects = System.Windows.Forms.DragDropEffects;
using PlatformIndependentMouseButtons = System.Windows.Forms.MouseButtons;
using PlatformIndependentCursor = System.Windows.Forms.Cursor;
using PlatformIndependentColor = System.Drawing.Color;
using PlatformIndependentFontStyle = System.Drawing.FontStyle;
using PlatformIWin32Window = System.Windows.Forms.IWin32Window;
using PlatformColor = System.Windows.Media.Color;
using PlatformBrush = System.Windows.Media.Brush;
using DXKeys = System.Windows.Forms.Keys;
using PlatformDialogResult = System.Windows.Forms.DialogResult;
using DialogResult = System.Windows.Forms.DialogResult;
using DevExpress.XtraRichEdit.Export.PlainText;
#endif
namespace DevExpress.Xpf.RichEdit {
	[TemplatePart(Name = SurfaceName, Type = typeof(Panel))]
	[TemplatePart(Name = SurfaceBorderName, Type = typeof(Border))]
	[TemplatePart(Name = VerticalScrollBarName, Type = typeof(ScrollBar))]
	[TemplatePart(Name = HorizontalScrollBarName, Type = typeof(ScrollBar))]
	[TemplatePart(Name = VerticalRulerName, Type = typeof(VerticalRulerControl))]
	[TemplatePart(Name = HorizontalRulerName, Type = typeof(HorizontalRulerControl))]
	[TemplatePart(Name = KeyCodeConverterName, Type = typeof(KeyCodeConverter))]
	[DXToolboxBrowsableAttribute]
	[ToolboxItem(true)]
	[ToolboxTabName(AssemblyInfo.DXTabWpfRichEdit)]
	public partial class RichEditControl : Control, PlatformIWin32Window, IDisposable, IToolTipControlClient, ILogicalOwner, IOfficeFontSizeProvider {
		#region Consts
		const string SurfaceName = "Surface";
		const string SurfaceBorderName = "SurfaceBorder";
		const string FocusElementName = "FocusElement";
		const string VerticalScrollBarName = "VerticalScrollBar";
		const string HorizontalScrollBarName = "HorizontalScrollBar";
		const string VerticalRulerName = "VerticalRuler";
		const string HorizontalRulerName = "HorizontalRuler";
		const string KeyCodeConverterName = "KeyCodeConverter";
		#endregion
		#region Dependency Properties
		#region ShowHoverMenu
		public static readonly DependencyProperty ShowHoverMenuProperty = DependencyPropertyManager.Register("ShowHoverMenu", typeof(bool), typeof(RichEditControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnShowHoverMenuChanged)));
#if !SL
	[DevExpressXpfRichEditLocalizedDescription("RichEditControlShowHoverMenu")]
#endif
		public bool ShowHoverMenu {
			get { return (bool)GetValue(ShowHoverMenuProperty); }
			set { SetValue(ShowHoverMenuProperty, value); }
		}
		static void OnShowHoverMenuChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			RichEditControl instance = (RichEditControl)d;
			instance.OnShowHoverMenuChanged((bool)e.OldValue, (bool)e.NewValue);
		}
		#endregion
		#region VerticalScrollBarVisibility
		public static readonly DependencyProperty VerticalScrollBarVisibilityProperty = DependencyPropertyManager.Register("VerticalScrollBarVisibility", typeof(Visibility), typeof(RichEditControl), new PropertyMetadata(Visibility.Visible, OnVerticalScrollBarVisibilityChanged));
#if !SL
	[DevExpressXpfRichEditLocalizedDescription("RichEditControlVerticalScrollBarVisibility")]
#endif
		public Visibility VerticalScrollBarVisibility {
			get { return (Visibility)GetValue(VerticalScrollBarVisibilityProperty); }
			set { SetValue(VerticalScrollBarVisibilityProperty, value); }
		}
		protected static void OnVerticalScrollBarVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			RichEditControl instance = d as RichEditControl;
			if (instance != null)
				instance.OnVerticalScrollBarVisibilityChanged((Visibility)e.OldValue, (Visibility)e.NewValue);
		}
		#endregion
		#region HorizontalScrollBarVisibility
		public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty = DependencyPropertyManager.Register("HorizontalScrollBarVisibility", typeof(Visibility), typeof(RichEditControl), new PropertyMetadata(Visibility.Visible, OnHorizontalScrollBarVisibilityChanged));
#if !SL
	[DevExpressXpfRichEditLocalizedDescription("RichEditControlHorizontalScrollBarVisibility")]
#endif
		public Visibility HorizontalScrollBarVisibility {
			get { return (Visibility)GetValue(HorizontalScrollBarVisibilityProperty); }
			set { SetValue(HorizontalScrollBarVisibilityProperty, value); }
		}
		protected static void OnHorizontalScrollBarVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			RichEditControl instance = d as RichEditControl;
			if (instance != null)
				instance.OnHorizontalScrollBarVisibilityChanged((Visibility)e.OldValue, (Visibility)e.NewValue);
		}
		#endregion
		#region CornerBoxVisibility
		public static readonly DependencyProperty CornerBoxVisibilityProperty = DependencyPropertyManager.Register("CornerBoxVisibility", typeof(Visibility), typeof(RichEditControl), new PropertyMetadata(Visibility.Visible));
		[
#if !SL
	DevExpressXpfRichEditLocalizedDescription("RichEditControlCornerBoxVisibility"),
#endif
 Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Visibility CornerBoxVisibility {
			get { return (Visibility)GetValue(CornerBoxVisibilityProperty); }
			set { SetValue(CornerBoxVisibilityProperty, value); }
		}
		#endregion
		#region HorizontalRulerVisibilityProperty
		public static readonly DependencyProperty HorizontalRulerVisibilityProperty = DependencyPropertyManager.Register("HorizontalRulerVisibility", typeof(Visibility), typeof(RichEditControl), new PropertyMetadata());
		[
#if !SL
	DevExpressXpfRichEditLocalizedDescription("RichEditControlHorizontalRulerVisibility"),
#endif
 Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Visibility HorizontalRulerVisibility {
			get { return (Visibility)GetValue(HorizontalRulerVisibilityProperty); }
			set {
				SetValue(HorizontalRulerVisibilityProperty, value);
				ChangeRulerMargin(value);
			}
		}
		#endregion
		#region VerticalRulerVisibility
		public static readonly DependencyProperty VerticalRulerVisibilityProperty = DependencyPropertyManager.Register("VerticalRulerVisibility", typeof(Visibility), typeof(RichEditControl), new PropertyMetadata());
		[
#if !SL
	DevExpressXpfRichEditLocalizedDescription("RichEditControlVerticalRulerVisibility"),
#endif
 Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Visibility VerticalRulerVisibility {
			get { return (Visibility)GetValue(VerticalRulerVisibilityProperty); }
			set { SetValue(VerticalRulerVisibilityProperty, value); }
		}
		#endregion
		#region BarManager
		public static readonly DependencyProperty BarManagerProperty = DependencyPropertyManager.Register(
				"BarManager",
				typeof(BarManager),
				typeof(RichEditControl),
				new UIPropertyMetadata(null, new PropertyChangedCallback(OnBarManagerChanged)));
#if !SL
	[DevExpressXpfRichEditLocalizedDescription("RichEditControlBarManager")]
#endif
		public BarManager BarManager {
			get { return (BarManager)GetValue(BarManagerProperty); }
			set { SetValue(BarManagerProperty, value); }
		}
		static void OnBarManagerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			RichEditControl instance = (RichEditControl)d;
			instance.OnBarManagerChanged((BarManager)e.OldValue, (BarManager)e.NewValue);
		}
		#endregion
		#region Ribbon
		public static readonly DependencyProperty RibbonProperty = DependencyPropertyManager.Register(
				"Ribbon",
				typeof(RibbonControl),
				typeof(RichEditControl),
				new UIPropertyMetadata(null, new PropertyChangedCallback(OnRibbonChanged)));
#if !SL
	[DevExpressXpfRichEditLocalizedDescription("RichEditControlRibbon")]
#endif
		public RibbonControl Ribbon {
			get { return (RibbonControl)GetValue(RibbonProperty); }
			set { SetValue(RibbonProperty, value); }
		}
		static void OnRibbonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			RichEditControl instance = (RichEditControl)d;
			instance.OnRibbonChanged((RibbonControl)e.OldValue, (RibbonControl)e.NewValue);
		}
		#endregion
		#region ShowBorder
		public static readonly DependencyProperty ShowBorderProperty = DependencyPropertyManager.Register("ShowBorder", typeof(bool), typeof(RichEditControl), new PropertyMetadata(true));
#if !SL
	[DevExpressXpfRichEditLocalizedDescription("RichEditControlShowBorder")]
#endif
		public bool ShowBorder {
			get { return (bool)GetValue(ShowBorderProperty); }
			set { SetValue(ShowBorderProperty, value); }
		}
		#endregion
		#region Content
		public static readonly DependencyProperty ContentProperty = DependencyPropertyManager.Register("Content", typeof(RichEditDocumentContent), typeof(RichEditControl), new PropertyMetadata(new RichEditDocumentContent(DocumentFormat.PlainText, String.Empty), OnContentChanged));
#if !SL
	[DevExpressXpfRichEditLocalizedDescription("RichEditControlContent")]
#endif
		public RichEditDocumentContent Content {
			get { return (RichEditDocumentContent)GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}
		static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			RichEditControl instance = (RichEditControl)d;
			instance.OnContentChanged((RichEditDocumentContent)e.OldValue, (RichEditDocumentContent)e.NewValue);
		}
		#endregion
		#region HandleWindowClose
		public static readonly DependencyProperty HandleWindowCloseProperty = DependencyPropertyManager.Register("HandleWindowClose", typeof(bool), typeof(RichEditControl), new PropertyMetadata(true));
#if !SL
	[DevExpressXpfRichEditLocalizedDescription("RichEditControlHandleWindowClose")]
#endif
		public bool HandleWindowClose {
			get { return (bool)GetValue(HandleWindowCloseProperty); }
			set { SetValue(HandleWindowCloseProperty, value); }
		}
		#endregion
		#region UseDeferredDataBindingNotifications
		public static readonly DependencyProperty UseDeferredDataBindingNotificationsProperty = DependencyPropertyManager.Register("UseDeferredDataBindingNotifications", typeof(bool), typeof(RichEditControl), new PropertyMetadata(true));
#if !SL
	[DevExpressXpfRichEditLocalizedDescription("RichEditControlUseDeferredDataBindingNotifications")]
#endif
		public bool UseDeferredDataBindingNotifications {
			get { return (bool)GetValue(UseDeferredDataBindingNotificationsProperty); }
			set { SetValue(UseDeferredDataBindingNotificationsProperty, value); }
		}
		#endregion
		#region AutoBackground
		public static readonly DependencyProperty AutoBackgroundProperty = DependencyPropertyManager.Register("AutoBackground", typeof(SolidColorBrush), typeof(RichEditControl), new PropertyMetadata(null, OnAutoColorChanged));
		public SolidColorBrush AutoBackground {
			get { return (SolidColorBrush)GetValue(AutoBackgroundProperty); }
			set { SetValue(AutoBackgroundProperty, value); }
		}
		static void OnAutoColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RichEditControl)d).InitializeThemeColors();
		}
		#endregion
		#region AutoForeground
		public static readonly DependencyProperty AutoForegroundProperty = DependencyPropertyManager.Register("AutoForeground", typeof(SolidColorBrush), typeof(RichEditControl), new PropertyMetadata(null, OnAutoColorChanged));
		public SolidColorBrush AutoForeground {
			get { return (SolidColorBrush)GetValue(AutoForegroundProperty); }
			set { SetValue(AutoForegroundProperty, value); }
		}
		#endregion
		#endregion
		#region Fields
		ScrollBar verticalScrollBar;
		ScrollBar horizontalScrollBar;
		Panel surface;
		Border surfaceBorder;
		Control focusElement;
		internal KeyCodeConverter keyCodeConverter;
		VerticalRulerControl verticalRuler;
		HorizontalRulerControl horizontalRuler;
		DispatcherTimer flushPendingTextInputTimer;
		bool presenterLoaded;
		bool showCaretInReadOnly = true;
		ToolTipController toolTipController;
		FontAndForeColorPropertyListener fontAndForeColorListener;
		RichEditPopupMenu menu;
		BarManagerMenuController menuController;
		ImeController imeController;
		RichEditControlAccessor accessor;
		readonly Dictionary<object, ImageSource> imageSourceCache;
		readonly RichEditControlBarCommandManager commandManager;
#if !SL
		int threadIdleSubscribeCount;
		ThreadIdleWeakEventHandler<RichEditControl> threadIdleHandler;
#endif
		#endregion
		static RichEditControl() {
#if !SL
			if (!System.Windows.Interop.BrowserInteropHelper.IsBrowserHosted)
				SetResourceDictionarySourceSwitchLevel();
#endif
		}
#if !SL
		[SecuritySafeCritical]
		static void SetResourceDictionarySourceSwitchLevel() {
			try {
				System.Diagnostics.PresentationTraceSources.ResourceDictionarySource.Switch.Level = System.Diagnostics.SourceLevels.Error;
			}
			catch {
			}
		}
#endif
		CommentPadding commentPadding;
		System.Windows.Point? lineOffset;
		public RichEditControl() {
			DefaultStyleKey = typeof(RichEditControl);
			this.accessor = new RichEditControlAccessor(this);
			this.commandManager = new RichEditControlBarCommandManager(this);
#if !SL
			this.threadIdleHandler = new ThreadIdleWeakEventHandler<RichEditControl>(this, OnApplicationIdleHandler);
#endif
			this.imageSourceCache = new Dictionary<object, ImageSource>();
			this.innerControl = CreateInnerControl();
			this.imeController = new Controls.Internal.ImeController(this);
			SizeChanged += RichEditorControl_SizeChanged;
			IsTabStop = false;
			LayoutUpdated += OnLayoutUpdated;
			IsEnabledChanged += OnIsEnabledChanged;
			this.toolTipController = new ToolTipController(this);
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
#if !SL
			RequestBringIntoView += OnRequestBringIntoView;
#endif
			this.menuController = new BarManagerMenuController(Menu);
			this.fontAndForeColorListener = new FontAndForeColorPropertyListener(this);
			this.fontAndForeColorListener.BeginListening();
			BeginInitialize();
#if SL
			RecalculateMaxClippingValue();			
#endif
		}
		#region FixSLCLipping
#if SL        
		int maxClippingValue = 31000;
		protected internal virtual int MaxClippingValue { get { return maxClippingValue; } set { maxClippingValue = value; } }
		protected virtual void SubscribeZoomEvent() {
			if(Application.Current != null && Application.Current.Host != null && Application.Current.Host.Content != null) {
				Application.Current.Host.Content.Zoomed += OnBrowserZoomed;
			}
		}
		protected virtual void UnsubscribeZoomEvent() {
			if (Application.Current != null && Application.Current.Host != null && Application.Current.Host.Content != null) {
				Application.Current.Host.Content.Zoomed -= OnBrowserZoomed;
			}
		}
		protected virtual void OnBrowserZoomed(object sender, EventArgs e) {
			RecalculateMaxClippingValue();
			Redraw();
		}
		protected virtual void RecalculateMaxClippingValue() {
			if (Application.Current.Host.Content.ZoomFactor > 1)
				MaxClippingValue = (int)(31000 / Application.Current.Host.Content.ZoomFactor);
			else
				MaxClippingValue = 31000;
		}
#endif
		#endregion
		#region Properties
#if !SL
	[DevExpressXpfRichEditLocalizedDescription("RichEditControlHoverMenuCalculator")]
#endif
		public HoverMenuCalculator HoverMenuCalculator { get; protected set; }
#if !SL
	[DevExpressXpfRichEditLocalizedDescription("RichEditControlDocumentLoader")]
#endif
		public IRichEditDocumentLoader DocumentLoader { get; set; }
#if !SL
	[DevExpressXpfRichEditLocalizedDescription("RichEditControlSurface")]
#endif
		public virtual Panel Surface { get { return surface; } }
		protected internal virtual Border SurfaceBorder { get { return surfaceBorder; } }
#if !SL
	[DevExpressXpfRichEditLocalizedDescription("RichEditControlFocusElement")]
#endif
		public virtual Control FocusElement { get { return focusElement; } }
#if !SL
	[DevExpressXpfRichEditLocalizedDescription("RichEditControlKeyCodeConverter")]
#endif
		public KeyCodeConverter KeyCodeConverter { get { return keyCodeConverter; } }
#if !SL
	[DevExpressXpfRichEditLocalizedDescription("RichEditControlSearchPanel")]
#endif
		public FrameworkElement SearchPanel { get { return RichEditSearchPanel; } }
		protected internal RichEditSearchPanel RichEditSearchPanel { get { ApplyTemplate(); return GetTemplateChild("SearchPanel") as RichEditSearchPanel; } }
		[
#if !SL
	DevExpressXpfRichEditLocalizedDescription("RichEditControlOptions"),
#endif
NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RichEditControlOptions Options { get { return (RichEditControlOptions)InnerControl.Options; } }
		bool IInnerRichEditControlOwner.Focused { get { return this.IsFocused; } }
		bool IInnerRichEditControlOwner.Enabled { get { return this.IsEnabled; } }
		bool IInnerRichEditControlOwner.IsHandleCreated { get { return PresenterLoaded; } }
		protected internal bool PresenterLoaded { get { return presenterLoaded; } set { presenterLoaded = value; } }
		internal BarManagerMenuController MenuController { get { return menuController; } }
		internal BarManagerActionCollection MenuCustomizations { get { return menuController.ActionContainer.Actions; } }
		bool IRichEditControl.UseGdiPlus { get { return false; } }
		protected internal RichEditControlAccessor Accessor { get { return this.accessor; } }
		protected internal RichEditControlBarCommandManager CommandManager { get { return commandManager; } }
		#region ForeColor
		PlatformIndependentColor IInnerRichEditControlOwner.ForeColor { get { return XpfTypeConverter.ToPlatformIndependentColor(this.ForeColor); } }
		protected internal PlatformColor ForeColor {
			get {
				PlatformBrush foregroundBrush = this.Foreground;
				SolidColorBrush solidColorBrush = foregroundBrush as SolidColorBrush;
				if (solidColorBrush != null)
					return solidColorBrush.Color;
				LinearGradientBrush linearGradientBrush = foregroundBrush as LinearGradientBrush;
				if (linearGradientBrush != null && linearGradientBrush.GradientStops.Count > 0)
					return linearGradientBrush.GradientStops[0].Color;
				return XpfTypeConverter.ToPlatformColor(DXColor.Empty);
			}
		}
		#endregion
		#region Font
		Font IInnerRichEditControlOwner.Font { get { return this.Font; } }
		protected internal Font Font {
			get {
				PlatformIndependentFontStyle fontStyle = PlatformIndependentFontStyle.Regular;
				if (FontStyle == FontStyles.Italic)
					fontStyle = PlatformIndependentFontStyle.Italic;
				if (IsBold(FontWeight))
					fontStyle = PlatformIndependentFontStyle.Bold;
				return new Font(FontFamily.Source, (float)FontSize, fontStyle);
			}
		}
		#endregion
		#region Selection
#if !SL
	[DevExpressXpfRichEditLocalizedDescription("RichEditControlSelection")]
#endif
		public string Selection {
			get {
				return ((DevExpress.XtraRichEdit.API.Native.Implementation.NativeSubDocument)Document).GetText(Document.Selections, null, null);
			}
		}
		#endregion
		#region SelectionRTF
#if !SL
	[DevExpressXpfRichEditLocalizedDescription("RichEditControlSelectionRTF")]
#endif
		public string SelectionRTF {
			get {
				RtfDocumentExporterOptions options = new RtfDocumentExporterOptions();
				options.ExportFinalParagraphMark = DevExpress.XtraRichEdit.Export.Rtf.ExportFinalParagraphMark.Never;
				return ((DevExpress.XtraRichEdit.API.Native.Implementation.NativeSubDocument)Document).GetRtfText(Document.Selections, options);
			}
		}
		#endregion
		#region PlainText
		[Obsolete("Please use the Text property instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string PlainText { get { return Text; } }
		#endregion
		#region Statistics
#if !SL
	[DevExpressXpfRichEditLocalizedDescription("RichEditControlSectionCount")]
#endif
		public int SectionCount { get { return DocumentModel.Sections.Count; } }
#if !SL
	[DevExpressXpfRichEditLocalizedDescription("RichEditControlParagraphCount")]
#endif
		public int ParagraphCount { get { return DocumentModel.MainPieceTable.Paragraphs.Count; } }
#if !SL
	[DevExpressXpfRichEditLocalizedDescription("RichEditControlCharCount")]
#endif
		public int CharCount { get { return ((IConvertToInt<DocumentLogPosition>)DocumentModel.MainPieceTable.DocumentEndLogPosition).ToInt(); } }
#if !SL
	[DevExpressXpfRichEditLocalizedDescription("RichEditControlWordCount")]
#endif
		public int WordCount {
			get {
				PieceTable pieceTable = DocumentModel.MainPieceTable;
				WordsDocumentModelIterator iterator = new WordsDocumentModelIterator(pieceTable);
				DocumentModelPosition pos = DocumentModelPosition.FromRunStart(DocumentModel.MainPieceTable, new RunIndex(0));
				int cnt = iterator.IsAtWord(pos) ? 1 : 0;
				for (; ; ) {
					pos = iterator.MoveForward(pos);
					if (iterator.IsAtWord(pos))
						cnt++;
					if (pos.LogPosition >= pieceTable.DocumentEndLogPosition)
						break;
				}
				return cnt;
			}
		}
#if !SL
	[DevExpressXpfRichEditLocalizedDescription("RichEditControlSectionsCount")]
#endif
		[Obsolete("Please use the 'SectionCount' property instead", false)]
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public string SectionsCount { get { return SectionCount.ToString(); } }
#if !SL
	[DevExpressXpfRichEditLocalizedDescription("RichEditControlParagraphsCount")]
#endif
		[Obsolete("Please use the 'ParagraphCount' property instead", false)]
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public string ParagraphsCount { get { return ParagraphCount.ToString(); } }
#if !SL
	[DevExpressXpfRichEditLocalizedDescription("RichEditControlCharsCount")]
#endif
		[Obsolete("Please use the 'CharCount' property instead", false)]
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public string CharsCount { get { return CharCount.ToString(); } }
#if !SL
	[DevExpressXpfRichEditLocalizedDescription("RichEditControlWordsCount")]
#endif
		[Obsolete("Please use the 'WordCount' property instead", false)]
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public string WordsCount { get { return WordCount.ToString(); } }
		#endregion
		bool caretVisibility = true;
#if !SL
	[DevExpressXpfRichEditLocalizedDescription("RichEditControlIsCaretVisible")]
#endif
		public bool IsCaretVisible {
			get { return caretVisibility; }
			set {
				if (caretVisibility == value)
					return;
				caretVisibility = value;
				RefreshView(RefreshAction.Selection);
			}
		}
#if SL
public bool IsFocused { get { return InnerIsFocused; } }
#endif
		protected internal bool InnerIsFocused { get { return KeyCodeConverter != null && KeyCodeConverter.InnerIsFocused; } }
#if !SL
	[DevExpressXpfRichEditLocalizedDescription("RichEditControlShowCaretInReadOnly")]
#endif
		public bool ShowCaretInReadOnly {
			get { return showCaretInReadOnly; }
			set {
				if (value == showCaretInReadOnly) return;
				showCaretInReadOnly = value;
				if (ActiveView != null) {
					RefreshView(RefreshAction.Selection);
				}
			}
		}
		protected virtual System.Drawing.Size SurfaceSize {
			get {
				if (Surface == null || Surface.Parent == null) return new System.Drawing.Size(5, 5);
				Panel p = Surface.Parent as Panel;
				if (p == null) return new System.Drawing.Size(5, 5);
				return new System.Drawing.Size((int)p.ActualWidth, (int)p.ActualHeight);
			}
		}
#if !SL
	[DevExpressXpfRichEditLocalizedDescription("RichEditControlVerticalRuler")]
#endif
		public VerticalRulerControl VerticalRuler { get { return verticalRuler; } }
#if !SL
	[DevExpressXpfRichEditLocalizedDescription("RichEditControlHorizontalRuler")]
#endif
		public HorizontalRulerControl HorizontalRuler { get { return horizontalRuler; } }
#if !SL
	[DevExpressXpfRichEditLocalizedDescription("RichEditControlVerticalScrollBar")]
#endif
		public ScrollBar VerticalScrollBar { get { return verticalScrollBar; } }
#if !SL
	[DevExpressXpfRichEditLocalizedDescription("RichEditControlHorizontalScrollBar")]
#endif
		public ScrollBar HorizontalScrollBar { get { return horizontalScrollBar; } }
#if !SL
	[DevExpressXpfRichEditLocalizedDescription("RichEditControlViewBounds")]
#endif
		public Rectangle ViewBounds {
			get {
				System.Windows.Point p = this.GetPositionSafe();
				Rectangle result = new Rectangle((int)p.X, (int)p.Y, (int)ActualWidth, (int)ActualHeight);
				if (ActiveView != null)
					return ActiveView.DocumentModel.LayoutUnitConverter.PixelsToLayoutUnits(result, DpiX, DpiY);
				else
					return Units.PixelsToDocuments(result, DpiX, DpiY);
			}
		}
		PlatformIndependentCursor IRichEditControl.Cursor {
			get { return XpfTypeConverter.ToPlatformIndependentCursor(Surface != null ? Surface.Cursor : this.Cursor); }
			set {
				if (Surface != null)
					Surface.Cursor = XpfTypeConverter.ToPlatformCursor(value);
				else
					this.Cursor = XpfTypeConverter.ToPlatformCursor(value);
			}
		}
		protected internal RichEditPopupMenu Menu {
			get {
				if (menu == null)
					menu = new RichEditPopupMenu(this);
				return menu;
			}
		}
		internal ImeController ImeController { get { return imeController; } }
		internal Dictionary<object, ImageSource> ImageSourceCache { get { return imageSourceCache; } }
		int IRichEditControl.SkinLeftMargin { get { return 0; } }
		int IRichEditControl.SkinRightMargin { get { return 0; } }
		int IRichEditControl.SkinTopMargin { get { return 0; } }
		int IRichEditControl.SkinBottomMargin { get { return 0; } }
		bool IRichEditControl.UseSkinMargins { get { return false; } }
		#endregion
		CommentPadding IInnerRichEditControlOwner.CommentPadding {
			get {
				if (commentPadding == null)
					commentPadding = ObtainCommentPadding();
				return commentPadding;
			}
		}
		protected internal System.Windows.Point LineOffset {
			get {
				if (!lineOffset.HasValue) {
					object objLineOffset = ResourceHelper.FindResource(this, new RichEditControlThemeKeyExtension() { ResourceKey = RichEditControlThemeKeys.CommentLineOffset });
					if (objLineOffset is System.Windows.Point)
						lineOffset = (System.Windows.Point)objLineOffset;
				}
				return lineOffset.GetValueOrDefault();
			}
		}
		public AutoSizeMode AutoSizeMode { get; set; }
		public void UpdateControlAutoSize() { }
		protected internal void ChangeRulerMargin(Visibility visibility) {
		}
		bool IInnerRichEditControlOwner.ShouldApplyForeColor() {
			return this.ShouldApplyForeColor();
		}
		bool IInnerRichEditControlOwner.ShouldApplyFont() {
			return this.ShouldApplyFont();
		}
		void IInnerRichEditControlOwner.ApplyFont(CharacterProperties characterProperties, Font font) {
			this.ApplyFont(characterProperties, font);
		}
		CommentPadding ObtainCommentPadding() {
			CommentPadding defaultPaddings = CommentPadding.GetDefaultCommentPadding(DocumentModel);
			DocumentLayoutUnitConverter converter = DocumentModel.LayoutUnitConverter;
			object objCommentsAreaLeftPadding = ResourceHelper.FindResource(this, new RichEditControlThemeKeyExtension() { ResourceKey = RichEditControlThemeKeys.CommentsAreaLeftPadding });
			object objCommentsAreaRightPadding = ResourceHelper.FindResource(this, new RichEditControlThemeKeyExtension() { ResourceKey = RichEditControlThemeKeys.CommentsAreaRightPadding });
			object objCommentContentPadding = ResourceHelper.FindResource(this, new RichEditControlThemeKeyExtension() { ResourceKey = RichEditControlThemeKeys.CommentContentPadding });
			int commentsAreaLeftPadding = objCommentsAreaLeftPadding is System.Double ? converter.PixelsToLayoutUnits(Convert.ToInt32(objCommentsAreaLeftPadding), DpiX) : defaultPaddings.CommentLeft;
			int commentsAreaRightPadding = objCommentsAreaRightPadding is System.Double ? converter.PixelsToLayoutUnits(Convert.ToInt32(objCommentsAreaRightPadding), DpiX) : defaultPaddings.CommentRight;
			if (objCommentContentPadding is Thickness) {
				Thickness commentContentPadding = (Thickness)objCommentContentPadding;
				return new CommentPadding(commentsAreaLeftPadding, commentsAreaRightPadding,
					converter.PixelsToLayoutUnits((int)commentContentPadding.Left, DpiX), converter.PixelsToLayoutUnits((int)commentContentPadding.Top, DpiY),
					converter.PixelsToLayoutUnits((int)commentContentPadding.Right, DpiX), converter.PixelsToLayoutUnits((int)commentContentPadding.Bottom, DpiY),
					defaultPaddings.DistanceBetweenComments, new System.Drawing.Size(0, 0), 0, 0, MoreButtonHorizontalAlignment.Right);
			}
			else
				return new CommentPadding(commentsAreaLeftPadding, commentsAreaRightPadding, defaultPaddings.ContentLeft, defaultPaddings.ContentTop, defaultPaddings.ContentRight, defaultPaddings.ContentBottom, defaultPaddings.DistanceBetweenComments, new System.Drawing.Size(0, 0), 0, 0, MoreButtonHorizontalAlignment.Right);
		}
		protected internal virtual bool ShouldApplyForeColor() {
			switch (Options.Behavior.ForeColorSource) {
				default:
				case RichEditBaseValueSource.Auto:
					if (HasLocalValue(ForegroundProperty))
						return DXColor.IsTransparentOrEmpty(XpfTypeConverter.ToPlatformIndependentColor(ForeColor));
					else
						return false;
				case RichEditBaseValueSource.Control:
					return true;
				case RichEditBaseValueSource.Document:
					return false;
			}
		}
		protected internal virtual bool ShouldApplyFont() {
			switch (Options.Behavior.FontSource) {
				default:
				case RichEditBaseValueSource.Auto:
					if (HasLocalValue(FontFamilyProperty) || HasLocalValue(FontSizeProperty) || HasLocalValue(FontStyleProperty) || HasLocalValue(FontWeightProperty))
						return true;
					else
						return false;
				case RichEditBaseValueSource.Control:
					return true;
				case RichEditBaseValueSource.Document:
					return false;
			}
		}
#if !SL
		Window window;
		internal Window Window {
			get { return window; }
			set {
				if (this.window != null)
					this.window.Closed -= OnWindowClosed;
				this.window = value;
				if (this.window != null)
					this.window.Closed += OnWindowClosed;
			}
		}
#endif
		void OnLoaded(object sender, RoutedEventArgs e) {
#if !SL
			this.Window = Window.GetWindow(this);
#endif
			Menu.Init();
			DestroyFlushPendingTextInputTimer();
			InitializeFlushPendingTextInputTimer();
			CreateKeyCodeTextBox();
#if !SL
			InternalUpdateBarManager();
#endif
			UnsubscribeToSpellCheckerEvents();
			SubscribeToSpellCheckerEvents();
#if!SL
			ThemeManager.ThemeChanged -= new ThemeChangedRoutedEventHandler(OnThemeChanged);
			ThemeManager.ThemeChanged += new ThemeChangedRoutedEventHandler(OnThemeChanged);
#else
			ThemeManager.ApplicationThemeChanged += new ThemeChangedRoutedEventHandler(OnThemeChanged);
#endif
			DependencyPropertyChangeHandler.AddHandler(this, "Language", OnLanguageChanged);
#if !SL
			if (threadIdleSubscribeCount <= 0) {
				System.Windows.Interop.ComponentDispatcher.ThreadIdle += threadIdleHandler.Handler; 
				threadIdleSubscribeCount++;
			}
#endif
#if SL && !DEBUG
#endif
#if SL
			SubscribeZoomEvent();
#endif
#if !SL
			AddBehaviors();
#endif
		}
		void OnUnloaded(object sender, RoutedEventArgs e) {
#if !SL
			System.Windows.Interop.ComponentDispatcher.ThreadIdle -= threadIdleHandler.Handler;
			threadIdleSubscribeCount = Math.Max(0, threadIdleSubscribeCount - 1);
#endif
			Menu.Reset();
			DestroyFlushPendingTextInputTimer();
			DestroyKeyCodeTextBox();
#if!SL
			ThemeManager.ThemeChanged -= OnThemeChanged;
#else
			ThemeManager.ApplicationThemeChanged -= OnThemeChanged;
#endif
#if !SL
			this.Window = null;
#endif
			UnsubscribeToSpellCheckerEvents();
#if SL
			UnsubscribeZoomEvent();
#endif
			DependencyPropertyChangeHandler.RemoveHandler(this, "Language");
#if !SL
			RemoveBehaviors();
#endif
		}
		void OnThemeChanged(DependencyObject sender, ThemeChangedRoutedEventArgs e) {
			commentPadding = null;
			lineOffset = null;
		}
		protected internal virtual void OnLayoutUnitChanged(object sender, EventArgs e) {
			commentPadding = null;
			lineOffset = null;
		}
		void OnLanguageChanged() {
			CommandManager.UpdateBarItemsDefaultValues();
			CommandManager.UpdateRibbonItemsDefaultValues();
		}
		void SubscribeToSpellCheckerEvents() {
			if (InnerControl != null)
				InnerControl.SubscribeToSpellCheckerEvents();
		}
		void UnsubscribeToSpellCheckerEvents() {
			if (InnerControl != null)
				InnerControl.UnsubscribeToSpellCheckerEvents();
		}
#if !SL
		void OnApplicationIdleHandler(RichEditControl control, object sender, EventArgs e) {
			if (control != null)
				control.OnApplicationIdle(sender, e);
		}
#endif
		void OnApplicationIdle(object sender, EventArgs e) {
			if (InnerControl != null)
				InnerControl.OnApplicationIdle();
		}
		void OnWindowClosed(object sender, EventArgs e) {
#if !SL
			Window = null;
#endif
			if (!HandleWindowClose)
				return;
			this.Dispose();
		}
		bool HasLocalValue(DependencyProperty property) {
			object val = ReadLocalValue(property);
			return val != null && val != DependencyProperty.UnsetValue;
		}
		bool IsBold(FontWeight weight) {
			return weight == FontWeights.Black ||
				weight == FontWeights.Bold ||
				weight == FontWeights.ExtraBlack ||
				weight == FontWeights.ExtraBold;
		}
		void OnLayoutUpdated(object sender, EventArgs e) {
			UpdateWindowlessMouseWheelSupport();
		}
		void OnIsEnabledChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e) {
			OnUpdateUI(this, EventArgs.Empty);
		}
		void UpdateWindowlessMouseWheelSupport() {
#if SL
			try {
				if (this.IsInDesignTool())
					return;
				if (!Application.Current.Host.Settings.Windowless)
					return;
				if (Parent != null || VisualTreeHelper.GetParent(this) != null)
					MouseWheelHelper.SetMouseWheelHandler(OnMouseWheel);
				else
					MouseWheelHelper.UnsetMouseWheelHandler(OnMouseWheel);
			}
			catch {
			}
#endif
		}
		protected virtual void InitializeHoverMenu() {
			HoverMenuCalculator = new HoverMenuCalculator(this);
			SelectionChanged += (d, e) => HoverMenuCalculator.OnHoverChanged();
		}
		protected internal virtual bool CalculateVerticalRulerVisibility() {
			RichEditRulerVisibility visibility = Options.VerticalRuler.Visibility;
			switch (visibility) {
				default:
				case RichEditRulerVisibility.Auto:
					if (ActiveView != null)
						return ActiveView.ShowVerticalRulerByDefault;
					else
						return false;
				case RichEditRulerVisibility.Visible:
					return true;
				case RichEditRulerVisibility.Hidden:
					return false;
			}
		}
		protected internal virtual bool CalculateHorizontalRulerVisibility() {
			RichEditRulerVisibility visibility = Options.HorizontalRuler.Visibility;
			switch (visibility) {
				default:
				case RichEditRulerVisibility.Auto:
					if (ActiveView != null)
						return ActiveView.ShowHorizontalRulerByDefault;
					else
						return false;
				case RichEditRulerVisibility.Visible:
					return true;
				case RichEditRulerVisibility.Hidden:
					return false;
			}
		}
		#region Events
		#region TextChanged
		EventHandler onTextChanged;
		public event EventHandler TextChanged { add { onTextChanged += value; } remove { onTextChanged -= value; } }
		protected internal virtual void RaiseTextChanged() {
			if (onTextChanged != null)
				onTextChanged(this, EventArgs.Empty);
		}
		#endregion
		#region CustomMarkDraw
		CustomMarkDrawEventHandler onCustomMarkDraw;
		public event CustomMarkDrawEventHandler CustomMarkDraw { add { onCustomMarkDraw += value; } remove { onCustomMarkDraw -= value; } }
		protected internal virtual void RaiseCustomMarkDraw(DevExpress.XtraRichEdit.Layout.Export.CustomMarkVisualInfoCollection customMarkVisualInfoCollection) {
			if (onCustomMarkDraw != null)
				onCustomMarkDraw(this, new CustomMarkDrawEventArgs(customMarkVisualInfoCollection));
		}
		#endregion
		#region SearchBoxClosed
		public event EventHandler SearchBoxClosed;
		void RaiseSearchClosed() {
			if (SearchBoxClosed != null)
				SearchBoxClosed(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				DestroyFlushPendingTextInputTimer();
				DisposeCommon();
				ClearImageSourceCache();
				if (!toolTipController.IsDisposed)
					toolTipController.Dispose();
			}
			isDisposed = true;
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~RichEditControl() {
			Dispose(false);
		}
		bool isDisposed;
		bool IRichEditDocumentServer.IsDisposed { get { return isDisposed; } }
		#endregion
		protected virtual Panel GetSurface() {
			return GetTemplateChild(SurfaceName) as Panel;
		}
		protected virtual Border GetSurfaceBorder() {
			return GetTemplateChild(SurfaceBorderName) as Border;
		}
		protected virtual Control GetFocusElement() {
			return GetTemplateChild(FocusElementName) as Control;
		}
		protected virtual ScrollBar GetVerticalScrollBar() {
			return GetTemplateChild(VerticalScrollBarName) as ScrollBar;
		}
		protected virtual ScrollBar GetHorizontalScrollBar() {
			return GetTemplateChild(HorizontalScrollBarName) as ScrollBar;
		}
		protected virtual HorizontalRulerControl GetHorizontalRuler() {
			return GetTemplateChild(HorizontalRulerName) as HorizontalRulerControl;
		}
		protected virtual VerticalRulerControl GetVerticalRuler() {
			return GetTemplateChild(VerticalRulerName) as VerticalRulerControl;
		}
		protected virtual KeyCodeConverter GetKeyCodeTextBox() {
			return GetTemplateChild(KeyCodeConverterName) as KeyCodeConverter;
		}
		protected internal void CreateFocusElement() {
			focusElement = GetFocusElement();
		}
		protected void CreateSurface() {
			surface = GetSurface();
			if (surface != null) {
				surface.SizeChanged += new SizeChangedEventHandler(surface_SizeChanged);
				surface.MouseLeftButtonDown += surface_MouseLeftButtonDown;
				surface.MouseLeftButtonUp += surface_MouseLeftButtonUp;
				surface.MouseRightButtonDown += surface_MouseRightButtonDown;
				surface.MouseRightButtonUp += surface_MouseRightButtonUp;
				surface.MouseMove += surface_MouseMove;
				surface.Drop += surface_Drop;
				surface.DragEnter += surface_DragEnter;
				surface.DragLeave += surface_DragLeave;
				surface.DragOver += surface_DragOver;
#if !SL
				if (this.gestureHelper == null)
					this.gestureHelper = new DevExpress.Xpf.Office.Internal.GestureHelper(this);
				gestureHelper.Stop();
				gestureHelper.Start(surface);
#endif
			}
			surfaceBorder = GetSurfaceBorder();
		}
		void surface_SizeChanged(object sender, SizeChangedEventArgs e) {
			SetViewPortSize();
		}
		IOfficeScrollbar IInnerRichEditControlOwner.CreateVerticalScrollBar() {
			return this.CreateVerticalScrollBar();
		}
		IOfficeScrollbar IInnerRichEditControlOwner.CreateHorizontalScrollBar() {
			return this.CreateHorizontalScrollBar();
		}
		protected internal virtual IOfficeScrollbar CreateVerticalScrollBar() {
			this.verticalScrollBar = GetVerticalScrollBar();
			if (VerticalScrollBar != null) {
				VerticalScrollBar.ValueChanged += VerticalScrollBar_ValueChanged;
				return new XpfOfficeScrollbar(VerticalScrollBar);
			}
			else
				return null;
		}
		protected internal virtual IOfficeScrollbar CreateHorizontalScrollBar() {
			this.horizontalScrollBar = GetHorizontalScrollBar();
			if (HorizontalScrollBar != null) {
				HorizontalScrollBar.ValueChanged += HorizontalScrollBar_ValueChanged;
				return new XpfOfficeScrollbar(HorizontalScrollBar);
			}
			else
				return null;
		}
		protected void CreateVerticalRuler() {
			verticalRuler = GetVerticalRuler();
			if (verticalRuler != null)
				VerticalRuler.Initialize(this);
		}
		protected void CreateHorizontalRuler() {
			horizontalRuler = GetHorizontalRuler();
			if (horizontalRuler != null)
				HorizontalRuler.Initialize(this);
		}
		void IInnerRichEditControlOwner.OnResizeCore() {
			this.OnResizeCore();
		}
		protected internal virtual void OnResizeCore() {
			UpdateRulersCore();
			UpdateVerticalScrollBar(false);
		}
		void IInnerRichEditControlOwner.ActivateViewPlatformSpecific(RichEditView view) {
			this.ActivateViewPlatformSpecific(view);
		}
		void IInnerRichEditControlOwner.DeactivateViewPlatformSpecific(RichEditView view) {
			this.DeactivateViewPlatformSpecific(view);
		}
		void IInnerRichEditControlOwner.OnZoomFactorChangingPlatformSpecific() {
			this.OnZoomFactorChangingPlatformSpecific();
		}
		protected virtual void DeactivateViewPlatformSpecific(RichEditView view) {
		}
		protected virtual void ActivateViewPlatformSpecific(RichEditView view) {
		}
		protected internal virtual void OnZoomFactorChangingPlatformSpecific() {
		}
		protected void CreateKeyCodeTextBox() {
			DestroyKeyCodeTextBox();
			keyCodeConverter = GetKeyCodeTextBox();
			if (KeyCodeConverter == null)
				return;
			keyCodeConverter.ImeController = ImeController;
			KeyCodeConverter.Owner = FocusElement;
			KeyCodeConverter.KeyPress += KeyCodeConverter_KeyPress;
			KeyCodeConverter.KeyDown += FocusElement_KeyDown;
			KeyCodeConverter.KeyUp += FocusElement_KeyUp;
			KeyCodeConverter.GotKeyboardFocus += OnKeyCodeConverterGotFocus;
			KeyCodeConverter.LostKeyboardFocus += OnKeyCodeConverterLostFocus;
		}
		protected void DestroyKeyCodeTextBox() {
			if (KeyCodeConverter != null) {
				KeyCodeConverter.Owner = null;
				KeyCodeConverter.KeyPress -= KeyCodeConverter_KeyPress;
				KeyCodeConverter.KeyDown -= FocusElement_KeyDown;
				KeyCodeConverter.KeyUp -= FocusElement_KeyUp;
				KeyCodeConverter.GotKeyboardFocus -= OnKeyCodeConverterGotFocus;
				KeyCodeConverter.LostKeyboardFocus -= OnKeyCodeConverterLostFocus;
				keyCodeConverter = null;
			}
		}
		protected internal virtual void AddServicesPlatformSpecific() {
			AddService(typeof(IRichEditCommandFactoryService), new XpfRichEditCommandFactoryService(this));
			AddService(typeof(IImeService), new ImeService(this));
			AddService(typeof(IRichEditPrintingService), new RichEditPrintingService());
			AddService(typeof(IFontCharacterSetService), new FontCharsetService());
		}
		protected internal virtual void RefreshView(RefreshAction refreshAction) {
			IXpfRichEditView view = (IXpfRichEditView)ActiveView;
			if (view == null)
				return;
			DevExpress.XtraRichEdit.Layout.Export.CustomMarkExporter customMarkExporter = new DevExpress.XtraRichEdit.Layout.Export.CustomMarkExporter();
			view.Adapter.Refresh(refreshAction, customMarkExporter);
			RaiseCustomMarkDraw(customMarkExporter.CustomMarkVisualInfoCollection);
		}
		protected virtual void OnKeyCodeConverterGotFocus(object sender, RoutedEventArgs e) {
			RefreshView(RefreshAction.Selection);
		}
		protected virtual void OnKeyCodeConverterLostFocus(object sender, RoutedEventArgs e) {
			RefreshView(RefreshAction.Selection);
			if (HoverMenuCalculator != null)
				HoverMenuCalculator.CloseCurrentHover();
		}
		protected internal bool IsAltGrPressed() {
#if !SL
			if (System.Windows.Interop.BrowserInteropHelper.IsBrowserHosted)
				return false;
			else
				return IsAltGrPressedCore();
#else
			return false;
#endif
		}
#if !SL
		[System.Security.SecuritySafeCritical]
		bool IsAltGrPressedCore() {
			const int VK_LCONTROL = 0xA2;
			const int VK_RMENU = 0xA5;
			bool leftCtrlPressed = DevExpress.Office.PInvoke.Win32.GetAsyncKeyState((System.Windows.Forms.Keys)VK_LCONTROL) != 0;
			bool rightMenuPressed = DevExpress.Office.PInvoke.Win32.GetAsyncKeyState((System.Windows.Forms.Keys)VK_RMENU) != 0;
			return leftCtrlPressed && rightMenuPressed;
		}
#endif
		void FocusElement_KeyUp(object sender, KeyEventArgs e) {
			try {
				FocusKeyCodeTextBox();
				if (ShouldShowPopupMenu(e)) {
					OnPopupMenu(CalculateDefaultPopupMenuPosition());
					e.Handled = true;
					return;
				}
				if (InnerControl != null && !IsAltGrPressed()) {
					PlatformIndependentKeyEventArgs args = e.ToPlatformIndependent();
					InnerControl.OnKeyUp(args);
					e.Handled = args.Handled;
				}
			}
			catch (Exception ex) {
				if (!HandleException(ex))
					throw;
			}
		}
		void FocusElement_KeyDown(object sender, KeyEventArgs e) {
			try {
				ClosePopups();
				if (ShouldShowPopupMenu(e)) {
					e.Handled = true;
					return;
				}
				if (InnerControl != null && !IsAltGrPressed()) {
					PlatformIndependentKeyEventArgs args = e.ToPlatformIndependent();
					InnerControl.OnKeyDown(args);
					e.Handled = args.Handled;
				}
			}
			catch (Exception ex) {
				if (!HandleException(ex))
					throw;
			}
		}
		bool ShouldShowPopupMenu(KeyEventArgs e) {
			try {
#if SL
				PlatformID platform = Environment.OSVersion.Platform;
				if (platform == PlatformID.Win32Windows || platform == PlatformID.Win32S || platform == PlatformID.Win32NT || platform == PlatformID.WinCE)
					return e.Key == Key.Unknown && e.PlatformKeyCode == 93;
				else
					return false;
#else
				return e.Key == Key.Apps;
#endif
			}
			catch {
				return false;
			}
		}
		System.Windows.Point CalculateDefaultPopupMenuPosition() {
			try {
				System.Windows.Point result = CalculateDefaultPopupMenuPositionCore();
				if (Double.IsInfinity(result.X) || Double.IsNaN(result.X))
					result.X = 0;
				if (Double.IsInfinity(result.Y) || Double.IsNaN(result.Y))
					result.Y = 0;
				return result;
			}
			catch {
				return new System.Windows.Point(0, 0);
			}
		}
		System.Windows.Point CalculateDefaultPopupMenuPositionCore() {
			CaretPosition caretPosition = ActiveView.CaretPosition;
			if (!caretPosition.Update(DocumentLayoutDetailsLevel.Character))
				return new System.Windows.Point(ActualWidth / 2, ActualWidth / 2);
			Rectangle caretBounds = caretPosition.CalculateCaretBounds();
			System.Drawing.Point pointInLayoutUnits = ActiveView.CreatePhysicalPoint(caretPosition.PageViewInfo, new System.Drawing.Point(caretBounds.X, caretBounds.Bottom));
			System.Drawing.Point pt = DocumentModel.LayoutUnitConverter.LayoutUnitsToPixels(pointInLayoutUnits, DpiX, DpiY);
			return new System.Windows.Point(pt.X, pt.Y);
		}
		bool FindParent(DependencyObject child, DependencyObject parent) {
			if (child == null)
				return false;
			if (parent == null)
				return false;
			if (VisualTreeHelper.GetParent(child) == null)
				return false;
			if (child == parent)
				return true;
			return FindParent(VisualTreeHelper.GetParent(child), parent);
		}
		protected internal virtual void OnMouseWheel(DevExpress.XtraRichEdit.Mouse.MouseWheelEventArgsEx e) {
			if (KeyCodeConverter.InnerIsFocused)
				OnMouseWheelCore(CreateMouseEventArgs(e));
		}
		protected override void OnMouseWheel(MouseWheelEventArgs e) {
			e.Handled = OnMouseWheelCore(CreateMouseEventArgs(e));
			base.OnMouseWheel(e);
		}
		protected internal virtual bool OnMouseWheelCore(PlatformIndependentMouseEventArgs e) {
			ClosePopups();
			if (InnerControl != null)
				return InnerControl.OnMouseWheel(e);
			else
				return false;
		}
		protected virtual void ClosePopups() {
			PopupMenuManager.CloseAllPopups();
		}
		internal PlatformIndependentMouseButtons ObtainPressedMouseButtons() {
			PlatformIndependentMouseButtons buttons = PlatformIndependentMouseButtons.None;
			if (LeftButtonPressed)
				buttons |= PlatformIndependentMouseButtons.Left;
			if (RightButtonPressed)
				buttons |= PlatformIndependentMouseButtons.Right;
			return buttons;
		}
		PlatformIndependentMouseEventArgs CreateMouseWheelEventArgsCore(float originalDelta) {
			int delta = (int)originalDelta;
#if SL
			return new PlatformIndependentMouseEventArgs(ObtainPressedMouseButtons(), 0, 0, 0, delta, KeyboardHandlerClass.IsShiftPressed, KeyboardHandlerClass.IsControlPressed);
#else
			return new PlatformIndependentMouseEventArgs(ObtainPressedMouseButtons(), 0, 0, 0, delta);
#endif
		}
		PlatformIndependentMouseEventArgs CreateMouseEventArgs(MouseWheelEventArgs e) {
			return CreateMouseWheelEventArgsCore(e.Delta);
		}
		PlatformIndependentMouseEventArgs CreateMouseEventArgs(DevExpress.XtraRichEdit.Mouse.MouseWheelEventArgsEx e) {
			return CreateMouseWheelEventArgsCore((float)e.Delta);
		}
		PlatformIndependentMouseEventArgs ConvertMouseEventArgs(MouseEventArgs e, PlatformIndependentMouseButtons buttons, int clicks) {
			System.Windows.Point p = e.GetPosition(Surface);
#if SL
			return new PlatformIndependentMouseEventArgs(buttons, clicks, (int)p.X, (int)p.Y, 0, KeyboardHandlerClass.IsShiftPressed, KeyboardHandlerClass.IsControlPressed);
#else
			return new PlatformIndependentMouseEventArgs(buttons, clicks, (int)p.X, (int)p.Y, 0);
#endif
		}
		PlatformIndependentDragEventArgs ConvertDragEventArgs(System.Windows.DragEventArgs e) {
			System.Windows.Point p = e.GetPosition(Surface);
			return new PlatformIndependentDragEventArgs(new XpfDataObject(e.Data), 0, (int)p.X, (int)p.Y, PlatformIndependentDragDropEffects.All, PlatformIndependentDragDropEffects.Copy);
		}
		internal bool LeftButtonPressed;
		bool RightButtonPressed;
		System.Windows.Point mousePosition;
		internal System.Windows.Point MousePosition { get { return mousePosition; } }
		void surface_MouseMove(object sender, MouseEventArgs e) {
			try {
				mousePosition = e.GetPosition(this);
				if (InnerControl != null)
					InnerControl.OnMouseMove(ConvertMouseEventArgs(e, ObtainPressedMouseButtons(), 0));
				base.OnMouseMove(e);
			}
			catch (Exception ex) {
				if (!HandleException(ex))
					throw;
			}
		}
		void surface_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
			try {
				Surface.ReleaseMouseCapture();
				LeftButtonPressed = false;
				if (InnerControl != null)
					InnerControl.OnMouseUp(ConvertMouseEventArgs(e, PlatformIndependentMouseButtons.Left, 1));
				base.OnMouseLeftButtonUp(e);
			}
			catch (Exception ex) {
				if (!HandleException(ex))
					throw;
			}
		}
		void surface_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
			try {
				if (!Surface.CaptureMouse())
					System.Diagnostics.Debug.Assert(true); 
				SetFocus();
				LeftButtonPressed = true;
				if (InnerControl != null)
					InnerControl.OnMouseDown(ConvertMouseEventArgs(e, PlatformIndependentMouseButtons.Left, 1));
				base.OnMouseLeftButtonDown(e);
			}
			catch (Exception ex) {
				if (!HandleException(ex))
					throw;
			}
		}
		void surface_MouseRightButtonUp(object sender, MouseButtonEventArgs e) {
			try {
				Surface.ReleaseMouseCapture();
				RightButtonPressed = false;
				if (InnerControl != null)
					InnerControl.OnMouseUp(ConvertMouseEventArgs(e, PlatformIndependentMouseButtons.Right, 1));
				e.Handled = true;
				base.OnMouseRightButtonUp(e);
			}
			catch (Exception ex) {
				if (!HandleException(ex))
					throw;
			}
		}
		void surface_MouseRightButtonDown(object sender, MouseButtonEventArgs e) {
			try {
				if (!Surface.CaptureMouse())
					System.Diagnostics.Debug.Assert(true); 
				SetFocus();
				RightButtonPressed = true;
				if (InnerControl != null)
					InnerControl.OnMouseDown(ConvertMouseEventArgs(e, PlatformIndependentMouseButtons.Right, 1));
				e.Handled = true;
				base.OnMouseRightButtonDown(e);
			}
			catch (Exception ex) {
				if (!HandleException(ex))
					throw;
			}
		}
		void surface_DragOver(object sender, System.Windows.DragEventArgs e) {
			try {
				if (InnerControl != null)
					InnerControl.OnDragOver(ConvertDragEventArgs(e));
			}
			catch (Exception ex) {
				if (!HandleException(ex))
					throw;
			}
		}
		void surface_DragLeave(object sender, System.Windows.DragEventArgs e) {
			try {
				if (InnerControl != null)
					InnerControl.OnDragLeave(ConvertDragEventArgs(e));
			}
			catch (Exception ex) {
				if (!HandleException(ex))
					throw;
			}
		}
		void surface_DragEnter(object sender, System.Windows.DragEventArgs e) {
			try {
				if (InnerControl != null)
					InnerControl.OnDragEnter(ConvertDragEventArgs(e));
			}
			catch (Exception ex) {
				if (!HandleException(ex))
					throw;
			}
		}
		void surface_Drop(object sender, System.Windows.DragEventArgs e) {
			try {
				if (InnerControl != null)
					InnerControl.OnDragDrop(ConvertDragEventArgs(e));
			}
			catch (Exception ex) {
				if (!HandleException(ex))
					throw;
			}
		}
		void KeyCodeConverter_KeyPress(object sender, PlatformIndependentKeyPressArgs e) {
			try {
				InnerRichEditControl innerControl = InnerControl;
				if (innerControl != null)
					innerControl.OnKeyPress(e);
			}
			catch (Exception ex) {
				if (!HandleException(ex))
					throw;
			}
		}
		void IRichEditControl.ForceFlushPendingTextInput() {
			this.ForceFlushPendingTextInput();
		}
		void OnFlushPendingTextInputTimerTick(object sender, EventArgs e) {
			if (IsUpdateLocked)
				return;
			ForceFlushPendingTextInput();
		}
		protected virtual void ForceFlushPendingTextInput() {
			if (KeyCodeConverter != null)
				KeyCodeConverter.FlushPendingTextInput();
			if (InnerControl != null)
				InnerControl.FlushPendingTextInput();
		}
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
			base.OnMouseLeftButtonDown(e);
			e.Handled = true;
		}
		void RichEditorControl_SizeChanged(object sender, SizeChangedEventArgs e) {
			SetViewPortSize();
		}
		void VerticalScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
		}
		void HorizontalScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
		}
		protected virtual double SurfaceHeight {
			get {
				Grid parent = Surface.Parent as Grid;
				return parent.RowDefinitions[0].ActualHeight;
			}
		}
		private void PerformLayout() {
			if (!isTemplateLoaded)
				return;
			SetViewPortSize();
		}
		bool isTemplateLoaded;
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			CreateFocusElement();
			CreateSurface();
			CreateKeyCodeTextBox();
			isTemplateLoaded = true;
			PerformLayout();
			InitializeSearchPanel();
			InitializeHoverMenu();
			horizontalRuler = null;
			verticalRuler = null;
			DoInitializationOnLoaded();
			CreateHorizontalRuler();
			CreateVerticalRuler();
			DestroyFlushPendingTextInputTimer();
			InitializeFlushPendingTextInputTimer();
			InitializeThemeColors();
		}
		void InitializeThemeColors() {
			SolidColorBrush backgroundBrush = AutoBackground;
			PlatformIndependentColor backgroundColor = backgroundBrush != null ? XpfTypeConverter.ToPlatformIndependentColor(backgroundBrush.Color) : DXColor.White;
			SolidColorBrush foregroundBrush = AutoForeground;
			PlatformIndependentColor textColor = foregroundBrush != null ? XpfTypeConverter.ToPlatformIndependentColor(foregroundBrush.Color) : DXColor.Black;
			skinTextColors = TextColors.FromSkinColors(backgroundColor, textColor);
		}
		protected internal virtual void InitializeFlushPendingTextInputTimer() {
			this.flushPendingTextInputTimer = new DispatcherTimer();
			flushPendingTextInputTimer.Tick += OnFlushPendingTextInputTimerTick;
			flushPendingTextInputTimer.Interval = TimeSpan.FromMilliseconds(DevExpress.XtraRichEdit.Keyboard.NormalKeyboardHandler.PendingTextFlushMilliseconds);
			flushPendingTextInputTimer.Start();
		}
		protected internal virtual void DestroyFlushPendingTextInputTimer() {
			if (flushPendingTextInputTimer != null) {
				flushPendingTextInputTimer.Stop();
				flushPendingTextInputTimer.Tick -= OnFlushPendingTextInputTimerTick;
				flushPendingTextInputTimer = null;
			}
		}
		private void InitializeSearchPanel() {
			if (RichEditSearchPanel != null) {
				RichEditSearchPanel.RichEditControl = this;
				RichEditSearchPanel.Closed += OnSearchPanelClosed;
			}
		}
		protected internal virtual void OnSearchPanelClosed(object sender, RoutedEventArgs e) {
			CollapseSearch();
		}
		public void CollapseSearch() {
			VisualStateManager.GoToState(this, "SearchCollapsed", true);
			IDisposable disposable = RichEditSearchPanel.ViewModel as IDisposable;
			if (disposable != null)
				disposable.Dispose();
			RichEditSearchPanel.ViewModel = null;
			SetFocus();
			RaiseSearchClosed();
		}
		protected void DoInitializationOnLoaded() {
			PerformLayout();
			OnPresenterLoaded();
			if (DocumentLoader != null && DocumentLoader.Stream != null)
				LoadDocument(DocumentLoader.Stream, DocumentLoader.Format);
		}
		MeasurementAndDrawingStrategy IInnerRichEditDocumentServerOwner.CreateMeasurementAndDrawingStrategy(DocumentModel documentModel) {
#if SL
			return new SilverlightMeasurementAndDrawingStrategy(documentModel);
#else
			return new WpfMeasurementAndDrawingStrategy(documentModel);
#endif
		}
		protected internal virtual void ReplaceDefaultStyles(DocumentModel targetModel, DocumentModel sourceModel) {
			ReplaceStylesCore(targetModel.CharacterStyles, sourceModel.CharacterStyles);
			ReplaceStylesCore(targetModel.ParagraphStyles, sourceModel.ParagraphStyles);
			ReplaceStylesCore(targetModel.TableStyles, sourceModel.TableStyles);
			ReplaceStylesCore(targetModel.NumberingListStyles, sourceModel.NumberingListStyles);
		}
		protected internal virtual void ReplaceStylesCore<T>(StyleCollectionBase<T> targetStyles, StyleCollectionBase<T> sourceStyles) where T : StyleBase<T> {
			int count = targetStyles.Count;
			for (int i = 0; i < count; i++) {
				T targetStyle = targetStyles[i];
				T sourceStyle = sourceStyles.GetStyleByName(targetStyle.StyleName);
				if (sourceStyle != null)
					targetStyle.CopyProperties(sourceStyle);
			}
		}
		public void RedrawCore(RefreshAction action) {
			RedrawCore(action, true);
		}
		public void RedrawCore(RefreshAction action, bool doRedraw) {
			if (IsUpdateLocked) {
				ControlDeferredChanges.Redraw = true;
				ControlDeferredChanges.RedrawAction |= action;
				if (!doRedraw) return;
				InnerControl.BeginDocumentRendering();
				try {
				}
				finally {
					InnerControl.EndDocumentRendering();
				}
			}
			else {
				if (ActiveView == null)
					return;
				DocumentRendering(action);
			}
		}
		void DocumentRendering(RefreshAction action) {
			if (InnerControl == null)
				return;
			InnerControl.BeginDocumentRendering();
			try {
				RenderDocumentCore(action);
			}
			finally {
				InnerControl.EndDocumentRendering();
			}
		}
		private void RenderDocumentCore(RefreshAction action) {
			if (!Dispatcher.CheckAccess()) {
				XpfBackgroundWorker.InvokeInUIThread(delegate { RefreshView(action); }, InvokerType.Refresh);
			}
			else {
				RefreshView(action);
			}
		}
		public void ShowCaret() { } 
		public void HideCaret() { } 
		public void FocusKeyCodeTextBox() {
			if ((System.Windows.Input.Keyboard.Modifiers & ModifierKeys.Control) == 0)
				SetFocus();
		}
		internal void SetFocus() {
			if (KeyCodeConverter != null) {
#if !SL
				FocusManager.SetFocusedElement(this, KeyCodeConverter);
#endif
				KeyCodeConverter.Focus();
			}
		}
		class ControlThreadSyncService : IThreadSyncService {
			readonly RichEditControl control;
			public ControlThreadSyncService(RichEditControl control) {
				Guard.ArgumentNotNull(control, "control");
				this.control = control;
			}
			public void EnqueueInvokeInUIThread(Action action) {
				control.BeginInvoke(action);
			}
		}
		public void BeginInvoke(Action method) {
			if (Dispatcher.CheckAccess()) {
				method();
			}
			else
				Dispatcher.BeginInvoke(method, new object[0]);
		}
		protected internal void OnPresenterLoaded() {
			PerformDefferedUpdaterOnPresenterLoaded();
			DoInitializationOnPresenterLoaded();
			PresenterLoaded = true;
			ApplyFontAndForeColor();
		}
		protected internal void DoInitializationOnPresenterLoaded() {
			BeginUpdate();
			try {
				EndInitialize();
				commandManager.UpdateBarItemsDefaultValues();
				commandManager.UpdateRibbonItemsDefaultValues();
				SetViewPortSize();
			}
			finally {
				EndUpdate();
			}
			UpdateUI += OnUpdateUI;
			OnUpdateUI(this, EventArgs.Empty);
		}
		protected internal virtual void EndInitialize() {
			EndInitializeCommon();
			ApplyFontAndForeColor();
			InitializeThemeColors();
		}
		void IInnerRichEditControlOwner.OnResize() {
		}
		void IInnerRichEditControlOwner.ApplyChangesCorePlatformSpecific(DocumentModelChangeActions changeActions) {
			this.ApplyChangesCorePlatformSpecific(changeActions);
		}
		void ApplyChangesCorePlatformSpecific(DocumentModelChangeActions changeActions) {
			RefreshAction action = RefreshAction.Selection;
			if ((changeActions & DocumentModelChangeActions.ResetAllPrimaryLayout) != 0 ||
				(changeActions & DocumentModelChangeActions.ResetPrimaryLayout) != 0 ||
				(changeActions & DocumentModelChangeActions.RaiseContentChanged) != 0 ||
				(changeActions & DocumentModelChangeActions.ResetSecondaryLayout) != 0)
				action = RefreshAction.AllDocument;
			if ((changeActions & DocumentModelChangeActions.Redraw) != 0 ||
				(changeActions & DocumentModelChangeActions.ResetSelectionLayout) != 0)
				RedrawCore(action, (changeActions & DocumentModelChangeActions.Redraw) != 0);
		}
		void IInnerRichEditDocumentServerOwner.RaiseDeferredEvents(DocumentModelChangeActions changeActions) {
			this.RaiseDeferredEvents(changeActions);
		}
		protected internal virtual void RaiseDeferredEvents(DocumentModelChangeActions changeActions) {
			InnerRichEditControl innerControl = InnerControl;
			IThreadSyncService service = GetService<IThreadSyncService>();
			if (!UseDeferredDataBindingNotifications) {
				DocumentModel.InternalAPI.OnEndDocumentUpdate(DocumentModel, new DocumentUpdateCompleteEventArgs(DocumentModel.DeferredChanges));
				if ((changeActions & DocumentModelChangeActions.RaiseContentChanged) != 0)
					RaiseBindingNotifications(); 
				if (innerControl != null) {
					service.EnqueueInvokeInUIThread(new Action(delegate() { innerControl.RaiseDeferredEventsCore(changeActions | DocumentModelChangeActions.SuppressBindingsNotifications); }));
				}
			}
			else {
				if (innerControl != null)
					service.EnqueueInvokeInUIThread(new Action(delegate() { innerControl.RaiseDeferredEventsCore(changeActions); }));
			}
		}
		protected virtual void PerformDefferedUpdaterOnPresenterLoaded() {
			DeferredBackgroundThreadUIUpdater deferredUpdater = BackgroundThreadUIUpdater as DeferredBackgroundThreadUIUpdater;
			InnerControl.BackgroundThreadUIUpdater = new BeginInvokeBackgroundThreadUIUpdater(new ControlThreadSyncService(this));
			if (deferredUpdater != null)
				PerformDeferredUIUpdates(deferredUpdater);
		}
		public virtual void SetViewPortSize() {
			if (ActiveView == null)
				return;
			var surfaceSize = SurfaceSize;
			if (surfaceSize == System.Drawing.Size.Empty) 
				return;
			ActiveView.OnResize(new Rectangle(new System.Drawing.Point(), ActiveView.DocumentModel.LayoutUnitConverter.PixelsToLayoutUnits(surfaceSize)), true);
			OnResizeCore();
		}
		public System.Drawing.Point PointToScreen(System.Drawing.Point point) {
			return point;
		}
		void IInnerRichEditControlOwner.Redraw() {
			this.Redraw();
		}
		void IInnerRichEditControlOwner.RedrawAfterEndUpdate() {
			this.Redraw();
		}
		void IInnerRichEditControlOwner.Redraw(RefreshAction action) {
			this.Redraw(action);
		}
		protected internal virtual void Redraw() {
			Redraw(RefreshAction.AllDocument);
		}
		public void RedrawEnsureSecondaryFormattingComplete() {
			Redraw();
		}
		public void Redraw(RefreshAction action) {
			if (Dispatcher.CheckAccess())
				RedrawCore(action);
			else
				UpdateUIFromBackgroundThread(delegate { RedrawCore(action); });
		}
		public void RedrawEnsureSecondaryFormattingComplete(RefreshAction action) {
			Redraw(action);
		}
		public void ShowSearchForm(string searchString) {
			VisualStateManager.GoToState(this, "SearchVisible", true);
			if (RichEditSearchPanel != null) {
				SearchPanelViewModel viewModel = new SearchPanelViewModel(this, false);
				viewModel.SearchString = searchString;
				RichEditSearchPanel.ViewModel = viewModel;
				RichEditSearchPanel.SetFocus();
				Storyboard sb = GetTemplateChild("SearchVisibleStoryboard") as Storyboard;
				if (sb != null)
					sb.Completed += OnSearchVisibleStoryboardCompleted;
			}
		}
		void OnSearchVisibleStoryboardCompleted(object sender, EventArgs e) {
			Storyboard sb = sender as Storyboard;
			if (sb != null)
				sb.Completed -= OnSearchVisibleStoryboardCompleted;
			if (RichEditSearchPanel != null)
				RichEditSearchPanel.SetFocus();
		}
		public void ShowReplaceForm() {
			VisualStateManager.GoToState(this, "SearchVisible", true);
			if (RichEditSearchPanel != null) {
				RichEditSearchPanel.ViewModel = new SearchPanelViewModel(this, true);
				System.Windows.Media.Animation.Storyboard sb = GetTemplateChild("SearchVisibleStoryboard") as System.Windows.Media.Animation.Storyboard;
				if (sb != null)
					sb.Completed += OnSearchVisibleStoryboardCompleted;
			}
		}
		public PredefinedFontSizeCollection GetPredefinedFontSizeCollection() {
			return InnerControl.PredefinedFontSizeCollection;
		}
		protected virtual void ShowDialog(UserControl form,  DialogClosedDelegate onDialogClosed, string title) {
			ShowDialog(form, this, onDialogClosed, title);
		}
		protected virtual void ShowDialog(UserControl form, FrameworkElement rootElement, DialogClosedDelegate onDialogClosed, string title) {
			ShowDialog(form, this, onDialogClosed, title, true);
		}
		protected virtual void ShowDialog(UserControl form, FrameworkElement rootElement, DialogClosedDelegate onDialogClosed, string title, bool allowSizing) {
			ShowDialog(form, this, onDialogClosed, title, allowSizing, true);
		}
#if !SL
		protected virtual FloatingContainer ShowDialog(UserControl form, FrameworkElement rootElement, DialogClosedDelegate onDialogClosed, string title, bool allowSizing, bool showModal) {
#else
		DXDialog ShowDialog(UserControl form, FrameworkElement rootElement, DialogClosedDelegate onDialogClosed, string title, bool allowSizing, bool showModal) {
#endif
#if !SL
			HoverMenuCalculator.CloseCurrentHover();
#endif
			FloatingContainerParameters parameters = new FloatingContainerParameters();
			parameters.ClosedDelegate = onDialogClosed;
			parameters.Title = title;
			parameters.CloseOnEscape = true;
			parameters.AllowSizing = allowSizing;
#if !SL
			parameters.ShowModal = showModal;
#endif
#if !SL
			FloatingContainer container =
 FloatingContainer.ShowDialogContent(form, rootElement, System.Windows.Size.Empty, parameters, true, (FrameworkElement)LayoutHelper.GetTopContainerWithAdornerLayer(rootElement));
			container.SizeToContent = SizeToContent.WidthAndHeight;
			if (!showModal) {
				FloatingWindowContainer windowContainer = container as FloatingWindowContainer;
				if (windowContainer != null)
					EnableModelessKeyboardInterop(windowContainer.Window);
			}
#else
			DXDialog dialog = FloatingContainer.ShowDialogContent(form, rootElement, System.Windows.Size.Empty, parameters, true);
			Popup parentWindow = dialog.Parent as Popup;
			string automationId = System.Windows.Automation.AutomationProperties.GetAutomationId(form);
			string automationName = System.Windows.Automation.AutomationProperties.GetName(form);
			if (!String.IsNullOrEmpty(automationId) && parentWindow != null)
				System.Windows.Automation.AutomationProperties.SetAutomationId(parentWindow, automationId);
			if (!String.IsNullOrEmpty(automationName) && parentWindow != null)
				System.Windows.Automation.AutomationProperties.SetName(parentWindow, automationName);
			dialog.MinWidth = dialog.ActualWidth;
			dialog.MinHeight = dialog.ActualHeight;
			return dialog;
#endif
#if !SL
			container.Hidden += OnDialogClosed;
			return container;
#endif
		}
#if !SL
		void EnableModelessKeyboardInterop(Window window) {
			System.Windows.Interop.HwndSource source = PresentationSource.FromVisual(this) as System.Windows.Interop.HwndSource;
			if (source != null) {
				System.Windows.Forms.Control control = System.Windows.Forms.Control.FromChildHandle(source.Handle);
				if (control != null && control.GetType().Name == "ElementHost") {
					MethodInfo methodInfo = control.GetType().GetMethod("EnableModelessKeyboardInterop", BindingFlags.Static | BindingFlags.Public);
					if (methodInfo != null)
						methodInfo.Invoke(null, new object[] { window });
				}
			}
		}
		protected virtual void OnDialogClosed(object sender, RoutedEventArgs e) {
			FloatingContainer container = sender as FloatingContainer;
			if (container == null)
				return;
			try {
				container.Hidden -= OnDialogClosed;
			}
			catch {
			}
			HoverMenuCalculator.OnHoverChanged();
		}
#endif
		void IRichEditControl.ShowFontForm(MergedCharacterProperties characterProperties, ShowFontFormCallback callback, object callbackData) {
			this.ShowFontForm(characterProperties, callback, callbackData);
		}
		void IRichEditControl.ShowFloatingInlineObjectLayoutOptionsForm(FloatingInlineObjectParameters parameters, ShowFloatingInlineObjectLayoutOptionsFormCallback callback, object callbackData) {
			FloatingInlineObjectLayoutOptionsFormControllerParameters controllerParameters = new FloatingInlineObjectLayoutOptionsFormControllerParameters(this, parameters);
			FloatingObjectLayoutFormControl form = new FloatingObjectLayoutFormControl(controllerParameters);
			DocumentModel.BeginUpdate();
			try {
				DialogClosedDelegate onFormClosed = (dialogResult) => {
					if (dialogResult.HasValue && dialogResult.Value)
						callback(form.Controller.FloatingInlineObjectParameters, callbackData);
				};
				ShowDialog(form, onFormClosed, XpfRichEditLocalizer.GetString(RichEditControlStringId.Caption_FloatingObjectLayoutForm));
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		internal virtual void ShowFontForm(MergedCharacterProperties characterProperties, ShowFontFormCallback callback, object callbackData) {
			FontFormControllerParameters controllerParameters = new FontFormControllerParameters(this, characterProperties);
			RichEditFontControl form = new RichEditFontControl(controllerParameters);
			DocumentModel.BeginUpdate();
			try {
				DialogClosedDelegate onFormClosed = (dialogResult) => {
					if (dialogResult.HasValue && dialogResult.Value)
						callback(form.Controller.SourceCharacterProperties, callbackData);
				};
				ShowDialog(form, onFormClosed, XpfRichEditLocalizer.GetString(RichEditControlStringId.Caption_FontForm));
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		void IRichEditControl.ShowParagraphForm(MergedParagraphProperties paragraphProperties, ShowParagraphFormCallback callback, object callbackData) {
			this.ShowParagraphForm(paragraphProperties, callback, callbackData);
		}
		internal virtual void ShowParagraphForm(MergedParagraphProperties paragraphProperties, ShowParagraphFormCallback callback, object callbackData) {
			ParagraphFormControllerParameters controllerParameters = new ParagraphFormControllerParameters(this, paragraphProperties, DocumentModel.UnitConverter);
			ParagraphFormControl form = new ParagraphFormControl(controllerParameters);
			DocumentModel.BeginUpdate();
			try {
				DialogClosedDelegate onFormClosed = (dialogResult) => {
					if (dialogResult.HasValue && dialogResult.Value)
						callback(form.Controller.SourceProperties, callbackData);
				};
				ShowDialog(form, onFormClosed, XpfRichEditLocalizer.GetString(RichEditControlStringId.Caption_ParagraphForm));
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		void IRichEditControl.ShowEditStyleForm(ParagraphStyle paragraphSourceStyle, ParagraphIndex index, ShowEditStyleFormCallback callback) {
			this.ShowEditStyleForm(paragraphSourceStyle, null, index, callback);
		}
		void IRichEditControl.ShowEditStyleForm(CharacterStyle characterSourceStyle, ParagraphIndex index, ShowEditStyleFormCallback callback) {
			this.ShowEditStyleForm(null, characterSourceStyle, index, callback);
		}
		internal virtual void ShowEditStyleForm(ParagraphStyle paragraphSourceStyle, CharacterStyle characterSourceStyle, ParagraphIndex index, ShowEditStyleFormCallback callback) {
			EditStyleFormControllerParameters controllerParameters;
			if (paragraphSourceStyle != null)
				controllerParameters = new EditStyleFormControllerParameters(this, paragraphSourceStyle, index);
			else
				controllerParameters = new EditStyleFormControllerParameters(this, characterSourceStyle, index);
			EditStyleFormControl form = new EditStyleFormControl(controllerParameters);
			DocumentModel.BeginUpdate();
			try {
				ShowDialog(form, null, XpfRichEditLocalizer.GetString(RichEditControlStringId.Caption_EditStyleForm));
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		void IRichEditControl.ShowTableStyleForm(TableStyle style) { }
		void IRichEditControl.ShowTOCForm(DevExpress.XtraRichEdit.Model.Field field) { }
		void IRichEditControl.ShowLanguageForm(DocumentModel documentModel) {
		}
		bool IRichEditControl.CanShowNumberingListForm { get { return false; } }
		void IRichEditControl.ShowNumberingListForm(ParagraphList paragraphs, ShowNumberingListFormCallback callback, object callbackData) {
			this.ShowNumberingListForm(paragraphs, callback, callbackData);
		}
		internal virtual void ShowNumberingListForm(ParagraphList paragraphs, ShowNumberingListFormCallback callback, object callbackData) {
		}
		void IRichEditControl.ShowTabsForm(TabFormattingInfo tabInfo, int defaultTabWidth, ShowTabsFormCallback callback, object callbackData) {
			this.ShowTabsForm(tabInfo, defaultTabWidth, callback, callbackData);
		}
		internal virtual void ShowTabsForm(TabFormattingInfo tabInfo, int defaultTabWidth, ShowTabsFormCallback callback, object callbackData) {
			TabsFormControllerParameters controllerParameters = new TabsFormControllerParameters(this, tabInfo, defaultTabWidth, DocumentModel.UnitConverter);
			TabsFormControl form = new TabsFormControl(controllerParameters);
			DocumentModel.BeginUpdate();
			try {
				DialogClosedDelegate onFormClosed = (dialogResult) => {
					if (dialogResult.HasValue && dialogResult.Value)
						callback(form.Controller.SourceTabInfo, form.Controller.SourceDefaultTabWidth, callbackData);
				};
				ShowDialog(form, onFormClosed, XpfRichEditLocalizer.GetString(RichEditControlStringId.Caption_TabsForm));
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		void IRichEditControl.ShowHyperlinkForm(HyperlinkInfo hyperlinkInfo, RunInfo runInfo, string title, ShowHyperlinkFormCallback callback) {
			this.ShowHyperlinkForm(hyperlinkInfo, runInfo, title, callback);
		}
		internal virtual void ShowHyperlinkForm(HyperlinkInfo hyperlinkInfo, RunInfo runInfo, string title, ShowHyperlinkFormCallback callback) {
			HyperlinkFormControllerParameters controllerParameters = new HyperlinkFormControllerParameters(this, hyperlinkInfo, runInfo);
			HyperlinkFormControl form = new HyperlinkFormControl(controllerParameters);
			DocumentModel.BeginUpdate();
			try {
				DialogClosedDelegate onFormClosed = (dialogResult) => {
					if (dialogResult.HasValue && dialogResult.Value)
						callback(form.Controller.HyperlinkInfo, form.Controller.TextSource, runInfo, form.Controller.TextToDisplay);
				};
				ShowDialog(form, onFormClosed, title);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		internal virtual void ShowBookmarkForm() {
			BookmarkFormControllerParameters controllerParameters = new BookmarkFormControllerParameters(this);
			BookmarkFormControl form = new BookmarkFormControl(controllerParameters);
			DialogClosedDelegate onFormClosed = null;
			ShowDialog(form, onFormClosed, XpfRichEditLocalizer.GetString(RichEditControlStringId.Caption_BookmarkForm));
		}
		protected internal virtual void ShowRangeEditingPermissionsForm() {
			RangeEditingPermissionsFormControllerParameters controllerParameters = new RangeEditingPermissionsFormControllerParameters(this);
			RangeEditingPermissionsControl form = new RangeEditingPermissionsControl(controllerParameters);
			ShowDialog(form, null, XpfRichEditLocalizer.GetString(RichEditControlStringId.Caption_RangeEditingPermissionsForm));
		}
		protected internal virtual void ShowDocumentProtectionQueryNewPasswordForm(PasswordInfo passwordInfo, PasswordFormCallback callback) {
			DocumentProtectionQueryNewPasswordFormControllerParameters controllerParameters = new DocumentProtectionQueryNewPasswordFormControllerParameters(this, passwordInfo);
			DocumentProtectionQueryNewPasswordControl form = new DocumentProtectionQueryNewPasswordControl(controllerParameters);
			DialogClosedDelegate onFormClosed = (dialogResult) => {
				if (dialogResult.HasValue && dialogResult.Value)
					callback(passwordInfo);
			};
			ShowDialog(form, onFormClosed, XpfRichEditLocalizer.GetString(RichEditControlStringId.Caption_DocumentProtectionQueryNewPasswordForm));
		}
		protected internal virtual void ShowDocumentProtectionQueryPasswordForm(PasswordInfo passwordInfo, PasswordFormCallback callback) {
			DocumentProtectionQueryPasswordFormControllerParameters controllerParameters = new DocumentProtectionQueryPasswordFormControllerParameters(this, passwordInfo);
			DocumentProtectionQueryPasswordControl form = new DocumentProtectionQueryPasswordControl(controllerParameters);
			DialogClosedDelegate onFormClosed = (dialogResult) => {
				if (dialogResult.HasValue && dialogResult.Value)
					callback(passwordInfo);
			};
			ShowDialog(form, onFormClosed, XpfRichEditLocalizer.GetString(RichEditControlStringId.Caption_DocumentProtectionQueryPasswordForm));
		}
		internal virtual void ShowLineNumberingForm(LineNumberingInfo properties, ShowLineNumberingFormCallback callback, object callbackData) {
			LineNumberingFormControllerParameters controllerParameters = new LineNumberingFormControllerParameters(this, properties);
			LineNumberingFormControl form = new LineNumberingFormControl(controllerParameters);
			DialogClosedDelegate onFormClosed = (dialogResult) => {
				if (dialogResult.HasValue && dialogResult.Value)
					callback(form.Controller.SourceLineNumberingInfo, callbackData);
			};
			ShowDialog(form, onFormClosed, XpfRichEditLocalizer.GetString(RichEditControlStringId.Caption_LineNumberingForm));
		}
		internal virtual void ShowPageSetupForm(PageSetupInfo properties, ShowPageSetupFormCallback callback, object callbackData, PageSetupFormInitialTabPage initialTabPage) {
			PageSetupFormControllerParameters controllerParameters = new PageSetupFormControllerParameters(this, properties);
			controllerParameters.InitialTabPage = initialTabPage;
			PageSetupFormControl form = new PageSetupFormControl(controllerParameters);
			DialogClosedDelegate onFormClosed = (dialogResult) => {
				if (dialogResult.HasValue && dialogResult.Value)
					callback(form.Controller.SourcePageSetupInfo, callbackData);
			};
			ShowDialog(form, onFormClosed, XpfRichEditLocalizer.GetString(RichEditControlStringId.Caption_PageSetupForm));
		}
		internal virtual void ShowColumnsSetupForm(ColumnsInfoUI properties, ShowColumnsSetupFormCallback callback, object callbackData) {
			ColumnsSetupFormControllerParameters controllerParameters = new ColumnsSetupFormControllerParameters(this, properties);
			ColumnsSetupFormControl form = new ColumnsSetupFormControl(controllerParameters);
			DialogClosedDelegate onFormClosed = (dialogResult) => {
				if (dialogResult.HasValue && dialogResult.Value)
					callback(form.Controller.SourceColumnsInfo, callbackData);
			};
			ShowDialog(form, onFormClosed, XpfRichEditLocalizer.GetString(RichEditControlStringId.Caption_ColumnsSetupForm));
		}
		internal virtual void ShowPasteSpecialForm(PasteSpecialInfo properties, ShowPasteSpecialFormCallback callback, object callbackData) {
			PasteSpecialFormControllerParameters controllerParameters = new PasteSpecialFormControllerParameters(this, properties);
			PasteSpecialFormControl form = new PasteSpecialFormControl(controllerParameters);
			DialogClosedDelegate onFormClosed = (dialogResult) => {
				if (dialogResult.HasValue && dialogResult.Value)
					callback(form.Controller.SourcePasteSpecialInfo, callbackData);
			};
			ShowDialog(form, onFormClosed, XpfRichEditLocalizer.GetString(RichEditControlStringId.Caption_PasteSpecialForm));
		}
		void IRichEditControl.ShowInsertTableForm(CreateTableParameters parameters, ShowInsertTableFormCallback callback, object callbackData) {
			this.ShowInsertTableForm(parameters, callback, callbackData);
		}
		internal virtual void ShowInsertTableForm(CreateTableParameters parameters, ShowInsertTableFormCallback callback, object callbackData) {
			InsertTableFormControllerParameters controllerParameters = new InsertTableFormControllerParameters(this, parameters);
			InsertTableFormControl form = new InsertTableFormControl(controllerParameters);
			DocumentModel.BeginUpdate();
			try {
				DialogClosedDelegate onFormClosed = (dialogResult) => {
					if (dialogResult.HasValue && dialogResult.Value)
						callback(form.Controller.SourceParameters, callbackData);
				};
				ShowDialog(form, onFormClosed, XpfRichEditLocalizer.GetString(RichEditControlStringId.Caption_InsertTableForm));
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		void IRichEditControl.ShowInsertTableCellsForm(TableCellsParameters parameters, ShowInsertDeleteTableCellsFormCallback callback, object callbackData) {
			this.ShowInsertTableCellsForm(parameters, callback, callbackData);
		}
		internal virtual void ShowInsertTableCellsForm(TableCellsParameters parameters, ShowInsertDeleteTableCellsFormCallback callback, object callbackData) {
			InsertTableCellsFormControllerParameters controllerParameters = new InsertTableCellsFormControllerParameters(this, parameters);
			InsertTableCellsFormControl form = new InsertTableCellsFormControl(controllerParameters);
			DocumentModel.BeginUpdate();
			try {
				DialogClosedDelegate onFormClosed = (dialogResult) => {
					if (dialogResult.HasValue && dialogResult.Value)
						callback(form.Controller.SourceParameters, callbackData);
				};
				ShowDialog(form, onFormClosed, XpfRichEditLocalizer.GetString(RichEditControlStringId.Caption_InsertTableCellsForm));
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		void IRichEditControl.ShowDeleteTableCellsForm(TableCellsParameters parameters, ShowInsertDeleteTableCellsFormCallback callback, object callbackData) {
			this.ShowDeleteTableCellsForm(parameters, callback, callbackData);
		}
		internal virtual void ShowDeleteTableCellsForm(TableCellsParameters parameters, ShowInsertDeleteTableCellsFormCallback callback, object callbackData) {
			DeleteTableCellsFormControllerParameters controllerParameters = new DeleteTableCellsFormControllerParameters(this, parameters);
			DeleteTableCellsFormControl form = new DeleteTableCellsFormControl(controllerParameters);
			DocumentModel.BeginUpdate();
			try {
				DialogClosedDelegate onFormClosed = (dialogResult) => {
					if (dialogResult.HasValue && dialogResult.Value)
						callback(form.Controller.SourceParameters, callbackData);
				};
				ShowDialog(form, onFormClosed, XpfRichEditLocalizer.GetString(RichEditControlStringId.Caption_DeleteTableCellsForm));
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		void IRichEditControl.ShowSplitTableCellsForm(SplitTableCellsParameters parameters, ShowSplitTableCellsFormCallback callback, object callbackData) {
			this.ShowSplitTableCellsForm(parameters, callback, callbackData);
		}
		internal virtual void ShowSplitTableCellsForm(SplitTableCellsParameters parameters, ShowSplitTableCellsFormCallback callback, object callbackData) {
			SplitTableCellsFormControllerParameters controllerParameters = new SplitTableCellsFormControllerParameters(this, parameters);
			SplitTableCellsFormControl form = new SplitTableCellsFormControl(controllerParameters);
			DocumentModel.BeginUpdate();
			try {
				DialogClosedDelegate onFormClosed = (dialogResult) => {
					if (dialogResult.HasValue && dialogResult.Value)
						callback(form.Controller.SourceParameters, callbackData);
				};
				ShowDialog(form, onFormClosed, XpfRichEditLocalizer.GetString(RichEditControlStringId.Caption_SplitTableCellsForm));
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		void IRichEditControl.ShowTablePropertiesForm(SelectedCellsCollection selectedCells) {
			ShowTablePropertiesForm(selectedCells);
		}
		internal virtual void ShowTablePropertiesForm(SelectedCellsCollection selectedCells) {
			TablePropertiesFormControllerParameters controllerParameters = new TablePropertiesFormControllerParameters(this, selectedCells);
			TablePropertiesFormControl form = new TablePropertiesFormControl(controllerParameters);
			ShowDialog(form, null, XpfRichEditLocalizer.GetString(RichEditControlStringId.Caption_TablePropertiesForm));
		}
		void IRichEditControl.ShowTableOptionsForm(Table table) {
			ShowTableOptionsForm(table, this);
		}
		internal virtual void ShowTableOptionsForm(Table table, FrameworkElement rootElement) {
			TableOptionsFormControllerParameters controllerParameters = new TableOptionsFormControllerParameters(this, table);
			TableOptionsFormControl form = new TableOptionsFormControl(controllerParameters);
			ShowDialog(form, rootElement, null, XpfRichEditLocalizer.GetString(RichEditControlStringId.Caption_TableOptionsForm));
		}
		void IRichEditControl.ShowBorderShadingForm(SelectedCellsCollection selectedCells) {
			ShowBorderShadingForm(selectedCells);
		}
		internal void ShowBorderShadingForm(SelectedCellsCollection selectedCells) {
			DocumentModel documentModel = selectedCells.FirstSelectedCell.DocumentModel;
			BorderShadingFormController controller = new BorderShadingFormController(this, documentModel, selectedCells);
			BorderShadingFormControl form = new BorderShadingFormControl(controller);
			ShowDialog(form, null, XpfRichEditLocalizer.GetString(RichEditControlStringId.Caption_BorderShadingForm));
		}
		internal virtual void ShowTableCellOptionsForm(List<TableCell> tableCells, FrameworkElement rootElement) {
			TableCellOptionsFormControl form = new TableCellOptionsFormControl(tableCells, this);
			ShowDialog(form, rootElement, null, XpfRichEditLocalizer.GetString(RichEditControlStringId.Caption_TableCellOptionsForm));
		}
#if!SL
		FloatingContainer symbolForm;
#else
		DXDialog symbolForm;
#endif
		internal void ShowSymbolForm(InsertSymbolViewModel viewModel) {
			if (symbolForm != null) {
				symbolForm.Activate();
				return;
			}
			DevExpress.Xpf.Office.UI.CharacterMapControl characterMap = new DevExpress.Xpf.Office.UI.CharacterMapControl() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = System.Windows.VerticalAlignment.Stretch };
			characterMap.CharDoubleClick += OnCharacterMapControlCharClick;
			characterMap.ServiceProvider = this;
			symbolForm = ShowDialog(characterMap, this, OnSymbolFormClosed, XpfRichEditLocalizer.GetString(RichEditControlStringId.Caption_SymbolForm), false, false);
			characterMap.Dispatcher.BeginInvoke(new Action(() => characterMap.FontName = viewModel.FontName));
			characterMap.UpdateLayout();
		}
		void OnSymbolFormClosed(bool? dialogResult) {
			symbolForm = null;
		}
		void OnCharacterMapControlCharClick(object sender, EventArgs e) {
			DevExpress.Xpf.Office.UI.CharacterMapControl map = (DevExpress.Xpf.Office.UI.CharacterMapControl)sender;
			InsertSymbolCommand command = new InsertSymbolCommand(this);
			if (command.CanExecute()) {
				DefaultValueBasedCommandUIState<SymbolProperties> state = new DefaultValueBasedCommandUIState<SymbolProperties>();
				state.Value = new SymbolProperties(map.Selection.ToString()[0], map.FontName);
				command.ForceExecute(state);
			}
		}
		protected internal virtual void OnPopupMenu(System.Windows.Point point) {
			if (!Options.Behavior.ShowPopupMenuAllowed)
				return;
			ClosePopups();
			RichEditContentMenuBuilder builder = new XpfRichEditContentMenuBuilder(this, new XpfRichEditMenuBuilderUIFactory());
			RichEditMenuBuilderInfo builderInfo = (RichEditMenuBuilderInfo)builder.CreatePopupMenu();
			Menu.MenuBuilderInfo = builderInfo;
			Menu.PlacementTarget = this;
#if SL
			Menu.Placement = PlacementMode.Top;
			Menu.Placement2 = PlacementMode2.Relative;
#else
			Menu.Placement = PlacementMode.Relative;
#endif
			point = Surface.TranslatePoint(point, this);
			Menu.HorizontalOffset = point.X;
			Menu.VerticalOffset = point.Y;
			Menu.ShowPopup(this);
		}
		void IInnerRichEditControlOwner.OnOptionsChangedPlatformSpecific(BaseOptionChangedEventArgs e) {
			this.OnOptionsChangedPlatformSpecific(e);
		}
		protected internal virtual void OnOptionsChangedPlatformSpecific(BaseOptionChangedEventArgs e) {
			UpdateRulersVisibility();
		}
		protected internal virtual void OnViewPaddingChanged() {
			if (SurfaceBorder != null) {
				SurfaceBorder.Padding = ActiveView.ActualPadding.ToThickness();
			}
		}
		Rectangle IInnerRichEditControlOwner.CalculateActualViewBounds(Rectangle previousViewBounds) {
			return this.CalculateActualViewBounds(previousViewBounds);
		}
		protected internal virtual Rectangle CalculateActualViewBounds(Rectangle previousViewBounds) {
			return previousViewBounds;
		}
		#region PreparePopupMenu
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete("You should use the 'PopupMenuShowing' instead", true)]
		public event PreparePopupMenuEventHandler PreparePopupMenu;
		[Obsolete("Please use the RaisePopupMenuShowing instead")]
		protected internal virtual void RaisePreparePopupMenu(PreparePopupMenuEventArgs args) {
			if (PreparePopupMenu != null)
				PreparePopupMenu(this, args);
		}
		#endregion
		#region PopupMenuShowing
		public event PopupMenuShowingEventHandler PopupMenuShowing;
		protected internal virtual void RaisePopupMenuShowing(PopupMenuShowingEventArgs args) {
			if (PopupMenuShowing != null)
				PopupMenuShowing(this, args);
		}
		#endregion
		#region PrepareHoverMenu
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete("You should use the 'HoverMenuShowing' instead", false)]
		public event PrepareHoverMenuEventHandler PrepareHoverMenu;
		[Obsolete("Please use the RaiseHoverMenuShowing instead")]
		protected internal virtual RichEditHoverMenu RaisePrepareHoverMenu(RichEditHoverMenu menu) {
			if (PrepareHoverMenu != null) {
				PrepareHoverMenuEventArgs args = new PrepareHoverMenuEventArgs(menu);
				PrepareHoverMenu(this, args);
				return (RichEditHoverMenu)args.Menu;
			}
			else
				return menu;
		}
		#endregion
		#region HoverMenuShowing
		HoverMenuShowingEventHandler onHoverMenuShowing;
		public event HoverMenuShowingEventHandler HoverMenuShowing {
			add {
				onHoverMenuShowing += value;
				if (HoverMenuCalculator != null)
					HoverMenuCalculator.ForceCloseCurrentHover();
			}
			remove {
				onHoverMenuShowing -= value;
				if (HoverMenuCalculator != null)
					HoverMenuCalculator.ForceCloseCurrentHover();
			}
		}
		protected internal virtual RichEditHoverMenu RaiseHoverMenuShowing(RichEditHoverMenu menu) {
			if (onHoverMenuShowing != null) {
				HoverMenuShowingEventArgs args = new HoverMenuShowingEventArgs(menu);
				onHoverMenuShowing(this, args);
				return (RichEditHoverMenu)args.Menu;
			}
			else
				return menu;
		}
		protected internal bool IsDefaultHoverMenu { get { return onHoverMenuShowing == null; } }
		#endregion
		protected internal virtual void OnTextChanged(EventArgs e) {
			RaiseTextChanged();
		}
		internal PlatformIndependentDragDropEffects DoDragDrop(object data, PlatformIndependentDragDropEffects allowedEffects) {
			return allowedEffects;
		}
		void IInnerRichEditControlOwner.UpdateRulers() {
			this.UpdateRulers();
		}
		void IInnerRichEditControlOwner.UpdateVerticalRuler() {
			this.UpdateVerticalRulerCore();
		}
		void IInnerRichEditControlOwner.UpdateHorizontalRuler() {
			this.UpdateHorizontalRulerCore();
		}
		protected internal virtual void UpdateRulersCore() {
			UpdateHorizontalRulerCore();
			UpdateVerticalRulerCore();
		}
		protected internal virtual void UpdateRulersVisibility() {
			SetHorizontalRulerVisibility(CalculateHorizontalRulerVisibility());
			SetVerticalRulerVisibility(CalculateVerticalRulerVisibility());
		}
		protected internal virtual void UpdateRulers() {
			if (horizontalRuler != null && horizontalRuler.CanUpdate())
				UpdateHorizontalRulerCore();
			if (verticalRuler != null && verticalRuler.CanUpdate())
				UpdateVerticalRulerCore();
		}
		protected internal virtual void UpdateHorizontalRulerCore() {
			bool visibility = CalculateHorizontalRulerVisibility();
			SetHorizontalRulerVisibility(visibility);
			if (horizontalRuler != null && visibility)
				horizontalRuler.Reset();
		}
		protected internal virtual void UpdateVerticalRulerCore() {
			bool visibility = CalculateVerticalRulerVisibility();
			SetVerticalRulerVisibility(visibility);
			if (verticalRuler != null && visibility)
				verticalRuler.Reset();
		}
		void SetHorizontalRulerVisibility(bool visibility) {
			if (visibility)
				HorizontalRulerVisibility = Visibility.Visible;
			else
				HorizontalRulerVisibility = Visibility.Collapsed;
		}
		void SetVerticalRulerVisibility(bool visibility) {
			if (visibility)
				VerticalRulerVisibility = Visibility.Visible;
			else
				VerticalRulerVisibility = Visibility.Collapsed;
		}
		void UpdateCornerBoxVisibility() {
			if (VerticalScrollBarVisibility == Visibility.Collapsed || HorizontalScrollBarVisibility == Visibility.Collapsed)
				CornerBoxVisibility = Visibility.Collapsed;
			else
				CornerBoxVisibility = Visibility.Visible;
		}
		protected internal virtual void OnVerticalScrollBarVisibilityChanged(Visibility oldValue, Visibility newValue) {
			UpdateCornerBoxVisibility();
		}
		protected internal virtual void OnHorizontalScrollBarVisibilityChanged(Visibility oldValue, Visibility newValue) {
			UpdateCornerBoxVisibility();
		}
		protected internal virtual void ApplyFont(CharacterProperties characterProperties, Font font) {
			switch (Options.Behavior.FontSource) {
				default:
				case RichEditBaseValueSource.Auto:
				case RichEditBaseValueSource.Control:
					CharacterPropertiesFontAssignmentHelper.AssignFont(characterProperties, font);
					break;
				case RichEditBaseValueSource.Document:
					break;
			}
		}
		void IInnerRichEditControlOwner.ResizeView(bool ensureCaretVisibleonResize) {
			this.ResizeView(ensureCaretVisibleonResize);
		}
		protected internal virtual void ResizeView(bool ensureCaretVisibleonResize) {
			Rectangle normalizedViewBounds = ViewBounds;
			normalizedViewBounds.X = 0;
			normalizedViewBounds.Y = 0;
			if (ActiveView != null)
				ActiveView.OnResize(normalizedViewBounds, ensureCaretVisibleonResize);
		}
		protected internal virtual void OnPageBackgroundChangedPlatformSpecific() {
		}
		void IInnerRichEditControlOwner.OnActiveViewBackColorChanged() {
			this.OnActiveViewBackColorChanged();
		}
		protected internal virtual void OnActiveViewBackColorChanged() {
			RefreshView(RefreshAction.AllDocument);
		}
		RichEditViewRepository IInnerRichEditControlOwner.CreateViewRepository() {
			return this.CreateViewRepository();
		}
		protected internal virtual RichEditViewRepository CreateViewRepository() {
			return new XpfRichEditViewRepository(this);
		}
		RichEditMouseHandler IInnerRichEditControlOwner.CreateMouseHandler() {
			return new RichEditMouseHandler(this);
		}
		RichEditControlOptionsBase IInnerRichEditDocumentServerOwner.CreateOptions(InnerRichEditDocumentServer documentServer) {
			return new RichEditControlOptions(documentServer);
		}
		void IRichEditControl.Print() {
			PrintCore(true);
		}
		void IRichEditControl.ShowPrintDialog() {
			PrintCore(false);
		}
		protected virtual void PrintCore(bool printDirect) {
			using (DevExpress.Xpf.Printing.LegacyPrintableComponentLink link = new DevExpress.Xpf.Printing.LegacyPrintableComponentLink(InnerControl)) {
				link.CreateDocument();
				if (link.PrintingSystem.Document.PageCount > 0)
					if (printDirect)
						link.PrintDirect();
					else
						link.Print();
			}
		}
		void IRichEditControl.ShowPrintPreview() {
			IRichEditPrintingService service = GetService<IRichEditPrintingService>();
			if (service != null)
				service.ShowPrintPreview(this);
			else {
#if SL
				this.BrowserPrintPreview();
#endif
			}
		}
		void IRichEditControl.ShowInsertMergeFieldForm() {
		}
		void IRichEditControl.ShowSearchForm() {
			this.ShowSearchForm(String.Empty);
		}
		void IRichEditControl.ShowReviewingPaneForm(DocumentModel documentModel, CommentViewInfo commentViewInfo, bool selectParagraph) {
			this.ShowReviewingPaneForm(documentModel, commentViewInfo, selectParagraph, DocumentLogPosition.Zero, DocumentLogPosition.Zero, false);
		}
		void IRichEditControl.ShowReviewingPaneForm(DocumentModel documentModel, CommentViewInfo commentViewInfo, DocumentLogPosition start, DocumentLogPosition end, bool setFocus) {
			this.ShowReviewingPaneForm(documentModel, commentViewInfo, false, start, end, setFocus);
		}
		protected internal void ShowReviewingPaneForm(DocumentModel documentModel, CommentViewInfo commentViewInfo, bool selectParagraph, DocumentLogPosition start, DocumentLogPosition end, bool setFocus) {
			this.RaiseShowReviewingPane(new ShowReviewingPaneEventArg(commentViewInfo, selectParagraph, start, end, setFocus, false));
		}
		void IRichEditControl.CloseReviewingPaneForm() {
			this.CloseReviewingPaneForm();
		}
		protected internal virtual void CloseReviewingPaneForm() {
			this.RaiseCloseReviewingPane();
		}
		bool IRichEditControl.IsVisibleReviewingPane() {
			return this.IsVisibleReviewingPane();
		}
		protected internal virtual bool IsVisibleReviewingPane() {
			ObtainReviewingPaneVisibleEventArg e = new ObtainReviewingPaneVisibleEventArg();
			this.RaiseObtainReviewingPaneVisible(e);
			return e.ReviewingPaneVisible;
		}
		void IRichEditControl.ShowBookmarkForm() {
			this.ShowBookmarkForm();
		}
		void IRichEditControl.ShowRangeEditingPermissionsForm() {
			this.ShowRangeEditingPermissionsForm();
		}
		void IRichEditControl.ShowDocumentProtectionQueryNewPasswordForm(PasswordInfo passwordInfo, PasswordFormCallback callback) {
			this.ShowDocumentProtectionQueryNewPasswordForm(passwordInfo, callback);
		}
		void IRichEditControl.ShowDocumentProtectionQueryPasswordForm(PasswordInfo passwordInfo, PasswordFormCallback callback) {
			this.ShowDocumentProtectionQueryPasswordForm(passwordInfo, callback);
		}
		void IRichEditControl.ShowLineNumberingForm(LineNumberingInfo properties, ShowLineNumberingFormCallback callback, object callbackData) {
			this.ShowLineNumberingForm(properties, callback, callbackData);
		}
		void IRichEditControl.ShowPageSetupForm(PageSetupInfo properties, ShowPageSetupFormCallback callback, object callbackData, PageSetupFormInitialTabPage initialTabPage) {
			this.ShowPageSetupForm(properties, callback, callbackData, initialTabPage);
		}
		void IRichEditControl.ShowColumnsSetupForm(XtraRichEdit.Forms.ColumnsInfoUI properties, ShowColumnsSetupFormCallback callback, object callbackData) {
			this.ShowColumnsSetupForm(properties, callback, callbackData);
		}
		void IRichEditControl.ShowPasteSpecialForm(PasteSpecialInfo properties, ShowPasteSpecialFormCallback callback, object callbackData) {
			this.ShowPasteSpecialForm(properties, callback, callbackData);
		}
		void IRichEditControl.ShowSymbolForm(InsertSymbolViewModel viewModel) {
			this.ShowSymbolForm(viewModel);
		}
		bool IRichEditControl.IsPrintingAvailable { get { return true; } }
		bool IRichEditControl.IsHyperlinkActive() {
			if (InnerControl != null && InnerControl.IsHyperlinkModifierKeysPress() && toolTipController.ActiveObject is HyperlinkInfo)
				return Options != null && Options.Hyperlinks.ShowToolTip;
			return false;
		}
		PlatformDialogResult IRichEditControl.ShowWarningMessage(string message) {
#if SL
			DXDialog dialog = new DXDialog();
			dialog.Title = XpfRichEditLocalizer.GetString(RichEditControlStringId.Msg_Warning);
			dialog.Buttons = DialogButtons.Ok;
			dialog.Content = message;
			dialog.IsSizable = false;
			dialog.Padding = new Thickness(20);
			dialog.ShowDialog();
#else
			DXMessageBox.Show(message, System.Windows.Forms.Application.ProductName, MessageBoxButton.OK, MessageBoxImage.Warning);
#endif
			return PlatformDialogResult.OK;
		}
		PlatformDialogResult IRichEditControl.ShowErrorMessage(string message) {
#if SL
			DXDialog dialog = new DXDialog();
			dialog.Title = XpfRichEditLocalizer.GetString(RichEditControlStringId.Msg_Warning);
			dialog.Buttons = DialogButtons.Ok;
			dialog.Content = message;
			dialog.IsSizable = false;
			dialog.Padding = new Thickness(20);
			dialog.ShowDialog();
#else
			DXMessageBox.Show(message, System.Windows.Forms.Application.ProductName, MessageBoxButton.OK, MessageBoxImage.Error);
#endif
			return PlatformDialogResult.OK;
		}
		void IRichEditControl.OnViewPaddingChanged() {
			this.OnViewPaddingChanged();
		}
		IPlatformSpecificScrollBarAdapter IRichEditControl.CreatePlatformSpecificScrollBarAdapter() {
			return new XpfScrollBarAdapter();
		}
		RichEditViewVerticalScrollController IRichEditControl.CreateRichEditViewVerticalScrollController(RichEditView view) {
			return new XpfRichEditViewVerticalScrollController(view);
		}
		RichEditViewHorizontalScrollController IRichEditControl.CreateRichEditViewHorizontalScrollController(RichEditView view) {
			return new XpfRichEditViewHorizontalScrollController(view);
		}
		IRulerControl IInnerRichEditControlOwner.CreateHorizontalRuler() {
			return null;
		}
		IRulerControl IInnerRichEditControlOwner.CreateVerticalRuler() {
			return null;
		}
		PlatformIndependentDragDropEffects IRichEditControl.DoDragDrop(object data, PlatformIndependentDragDropEffects allowedEffects) {
			return this.DoDragDrop(data, allowedEffects);
		}
#if !SL
		IntPtr PlatformIWin32Window.Handle { get { return IntPtr.Zero; } }
#endif
		#region IToolTipControlClient Members
#if !SL
	[DevExpressXpfRichEditLocalizedDescription("RichEditControlHasToolTip")]
#endif
		public bool HasToolTip { get { return InnerControl != null ? InnerControl.MouseHandler.State.CanShowToolTip : false; } }
		public ToolTipControlInfo GetObjectInfo(System.Windows.Point point) {
			if (!this.IsInVisualTree())
				return null;
			point = point.ToRootVisualSafe(this);
			point = point.ToLocalSafe(Surface);
			System.Drawing.Point pt = new System.Drawing.Point();
			pt.X = DocumentModel.LayoutUnitConverter.PixelsToLayoutUnits((int)point.X, DpiX);
			pt.Y = DocumentModel.LayoutUnitConverter.PixelsToLayoutUnits((int)point.Y, DpiY);
			RichEditHitTestResult hitTestResult = ActiveView.CalculateHitTest(pt, DocumentLayoutDetailsLevel.Character);
			if (hitTestResult == null)
				return null;
			ToolTipControlInfo info = CreateTooltipInfo(hitTestResult, pt);
			if (info != null)
				info.Position = point.ToRootVisualSafe(Surface);
			return info;
		}
		ToolTipControlInfo CreateTooltipInfo(RichEditHitTestResult hitTestResult, System.Drawing.Point pt) {
			if (hitTestResult != null && hitTestResult.CommentViewInfo != null)
				return CreateCommentViewInfoToolTipInfo(hitTestResult.CommentViewInfo);
			if (DocumentModel.CommentOptions.Visibility == RichEditCommentVisibility.Visible || DocumentModel.CommentOptions.HighlightCommentedRange)
			{
				ToolTipControlInfo result = CreateCommentToolTipInfo(hitTestResult);
				if (result != null)
					return result;
				return null;
			}
			return CreateHyperlinkToolTipInfo(hitTestResult);
		}
		ToolTipControlInfo CreateCommentToolTipInfo(RichEditHitTestResult hitTestResult) {
			Comment comment = FindCommentByHitTestResult(hitTestResult);
			XpfRichEditToolTipHelper helper = new XpfRichEditToolTipHelper(this);
			if (comment != null)
				return helper.CalculateCommentToolTipInfo(comment);
			return null;
		}
		Comment FindCommentByHitTestResult(RichEditHitTestResult hitTestResult) {
			if (hitTestResult.DetailsLevel < DocumentLayoutDetailsLevel.Character)
				return null;
			if (DocumentModel.ActivePieceTable.IsMain && hitTestResult.Character!=null)
			{
				FormatterPosition start = hitTestResult.Character.StartPos;
				DocumentLogPosition logPosition = DocumentModel.MainPieceTable.GetRunLogPosition(start.RunIndex) + start.Offset;
				return DocumentModel.MainPieceTable.FindCommentByDocumentLogPosition(logPosition);
			}
			return null;
		}
		ToolTipControlInfo CreateCommentViewInfoToolTipInfo(CommentViewInfo commentViewInfo) {
			XpfRichEditToolTipHelper helper = new XpfRichEditToolTipHelper(this);
			return helper.CalculateCommentViewInfoToolTipInfo(commentViewInfo);
		}
		ToolTipControlInfo CreateHyperlinkToolTipInfo(RichEditHitTestResult hitTestResult) {
			Field hyperlink = ActiveView.GetHyperlinkField(hitTestResult);
			if (hyperlink != null && !hyperlink.IsCodeView) {
				HyperlinkInfo hyperlinkInfo = DocumentModel.ActivePieceTable.GetHyperlinkInfo(hyperlink);
				XpfRichEditToolTipHelper helper = new XpfRichEditToolTipHelper(this);
				return helper.CalculateHyperlinkToolTipInfo(hyperlinkInfo);
			}
			return null;
		}
		#endregion
#if SL
		#region ILogicalOwner Members
		List<object> logicalChildren = new List<object>();
		void ILogicalOwner.AddChild(object child) {
			logicalChildren.Add(child);
		}
		void ILogicalOwner.RemoveChild(object child) {
			logicalChildren.Remove(child);
		}
		bool ILogicalOwner.IsLoaded { get { return true; } }
		event KeyboardFocusChangedEventHandler IInputElement.PreviewGotKeyboardFocus { add {} remove {} }
		event KeyboardFocusChangedEventHandler IInputElement.PreviewLostKeyboardFocus { add { } remove { } }
		#endregion
#else
		#region ILogicalOwner Members
		List<object> logicalChildren = new List<object>();
		void ILogicalOwner.AddChild(object child) {
			logicalChildren.Add(child);
			AddLogicalChild(child);
		}
		void ILogicalOwner.RemoveChild(object child) {
			logicalChildren.Remove(child);
			RemoveLogicalChild(child);
		}
		protected override IEnumerator LogicalChildren {
			get { return new MergedEnumerator(base.LogicalChildren, logicalChildren.GetEnumerator()); }
		}
		#endregion
#endif
		void OnUpdateUI(object sender, EventArgs e) {
			try {
				commandManager.UpdateBarItemsState();
				commandManager.UpdateRibbonItemsState();
			}
			catch (Exception ex) {
				if (!HandleException(ex))
					throw;
			}
		}
		#region BarManager interop
		protected internal virtual void SubscribeBarManagerEvents(BarManager barManager) {
			barManager.Loaded += OnBarManagerLoaded;
		}
		protected internal virtual void UnsubscribeBarManagerEvents(BarManager barManager) {
			barManager.Loaded -= OnBarManagerLoaded;
		}
		protected internal virtual void SubscribeRibbonEvents(RibbonControl ribbon) {
			ribbon.Loaded += OnRibbonLoaded;
		}
		protected internal virtual void UnsubscribeRibbonEvents(RibbonControl ribbon) {
			ribbon.Loaded -= OnRibbonLoaded;
		}
		protected internal virtual void OnShowHoverMenuChanged(bool oldValue, bool newValue) {
			if (newValue == false) {
				if (HoverMenuCalculator != null)
					HoverMenuCalculator.CloseCurrentHover();
			}
		}
		protected internal virtual void OnBarManagerChanged(BarManager oldValue, BarManager newValue) {
			if (HoverMenuCalculator != null)
				HoverMenuCalculator.ForceCloseCurrentHover();
			if (oldValue != null) {
				commandManager.UnsubscribeBarItemsEvents(oldValue);
				UnsubscribeBarManagerEvents(oldValue);
			}
			if (newValue != null)
				SubscribeBarManagerEvents(newValue);
			commandManager.UpdateBarItemsDefaultValues();
		}
		protected internal virtual void OnRibbonChanged(RibbonControl oldValue, RibbonControl newValue) {
			if (oldValue != null)
				UnsubscribeRibbonEvents(oldValue);
			if (newValue != null)
				SubscribeRibbonEvents(newValue);
			commandManager.UpdateRibbonItemsDefaultValues();
		}
		void OnBarManagerLoaded(object sender, RoutedEventArgs e) {
			commandManager.UpdateBarItemsDefaultValues();
			UnsubscribeBarManagerEvents(BarManager);
			OnUpdateUI(this, EventArgs.Empty);
		}
		void OnRibbonLoaded(object sender, RoutedEventArgs e) {
			commandManager.UpdateRibbonItemsDefaultValues();
			UnsubscribeRibbonEvents(Ribbon);
			OnUpdateUI(this, EventArgs.Empty);
		}
		#endregion
#if !SL
		internal void InternalUpdateBarManager() {
			ForceElementNameBinding(this, BarManagerProperty);
			if (BarManager == null) return;
			foreach (BarItem item in BarManager.Items)
				ForceBarItemElementNameBinding(item);
		}
		void ForceBarItemElementNameBinding(BarItem item) {
			ForceElementNameBinding(item, BarItem.CommandProperty);
			ForceElementNameBinding(item, BarItem.CommandParameterProperty);
			ForceElementNameBinding(item, BarItem.CommandTargetProperty);
			IRichEditControlDependencyPropertyOwner propertyOwner = item as IRichEditControlDependencyPropertyOwner;
			if (propertyOwner != null)
				ForceElementNameBinding(item, propertyOwner.DependencyProperty);
			BarEditItem editItem = item as BarEditItem;
			if (editItem != null && editItem.EditSettings != null) {
				IRichEditControlDependencyPropertyOwner settings = editItem.EditSettings as IRichEditControlDependencyPropertyOwner;
				if (settings != null)
					ForceElementNameBinding(editItem.EditSettings, settings.DependencyProperty);
			}
		}
		internal void ForceElementNameBinding(DependencyObject o, DependencyProperty p) {
			BindingExpression bindingExpression = BindingOperations.GetBindingExpression(o, p);
			if (bindingExpression == null || bindingExpression.Status != BindingStatus.PathError) return;
			Binding binding = bindingExpression.ParentBinding;
			BindingOperations.ClearBinding(o, p);
			BindingOperations.SetBinding(o, p, binding);
		}
#endif
		protected internal virtual void OnContentChanged(RichEditDocumentContent oldValue, RichEditDocumentContent newValue) {
			oldValue.Control = null;
			if (DocumentModel == null)
				return;
			newValue.Control = this;
			if (newValue.Format == DocumentFormat.Undefined) 
				return;
			if (newValue.Format == DocumentFormat.PlainText)
				Text = newValue.Content as String;
			else if (newValue.Format == DocumentFormat.Rtf)
				RtfText = newValue.Content as String;
			if (newValue.Format == DocumentFormat.Html)
				HtmlText = newValue.Content as String;
			if (newValue.Format == DocumentFormat.Mht)
				MhtText = newValue.Content as String;
			if (newValue.Format == DocumentFormat.WordML)
				WordMLText = newValue.Content as String;
			if (newValue.Format == DocumentFormat.Xaml)
				XamlText = newValue.Content as String;
			if (newValue.Format == DocumentFormat.OpenXml)
				OpenXmlBytes = newValue.Content as byte[];
			if (newValue.Format == DocumentFormat.OpenDocument)
				OpenDocumentBytes = newValue.Content as byte[];
		}
		protected internal virtual void OnInnerControlContentChangedPlatformSpecific(bool suppressBindingNotifications) {
			if (!suppressBindingNotifications)
				RaiseBindingNotifications();
		}
		protected internal virtual void RaiseBindingNotifications() {
			RichEditDocumentContent content = new RichEditDocumentContent(DocumentFormat.Undefined, null);
			content.Control = this;
			content.Version = Content.Version + 1;
			BindingExpression bindingExpression = GetBindingExpression(ContentProperty);
			if (bindingExpression == null || (bindingExpression.ParentBinding != null && bindingExpression.ParentBinding.Mode == BindingMode.TwoWay))
				this.Content = content; 
		}
		protected internal virtual void OnReadOnlyChangedPlatformSpecific() {
		}
		protected internal virtual void OnActiveViewChangedPlatformSpecific() {
			if (HoverMenuCalculator != null)
				HoverMenuCalculator.ForceCloseCurrentHover();
		}
		event EventHandler ICommandAwareControl<RichEditCommandId>.BeforeDispose { add { } remove { } }
		void ICommandAwareControl<RichEditCommandId>.Focus() {
			this.SetFocus();
		}
		void ICommandAwareControl<RichEditCommandId>.CommitImeContent() {
		}
		void OnEmptyDocumentCreatedPlatformSpecific() {
			ClearImageSourceCache();
		}
		void OnDocumentLoadedPlatformSpecific() {
			ClearImageSourceCache();
		}
		void ClearImageSourceCache() {
			imageSourceCache.Clear();
		}
		#region IOfficeFontSizeProvider Members
		List<int> IOfficeFontSizeProvider.GetPredefinedFontSizes() {
			return GetPredefinedFontSizeCollection();
		}
		#endregion
		protected internal void BeginProcessMultipleKeyPress() {
			NormalKeyboardHandler handler = InnerControl.KeyboardHandler as NormalKeyboardHandler;
			if (handler != null)
				handler.BeginProcessMultipleKeyPress();
		}
		protected internal void EndProcessMultipleKeyPress() {
			NormalKeyboardHandler handler = InnerControl.KeyboardHandler as NormalKeyboardHandler;
			if (handler != null)
				handler.EndProcessMultipleKeyPress();
		}
		public void InvalidateDocumentLayout() {
			Redraw(RefreshAction.AllDocument);
		}
	}
	#region RichEditControlOptions
	public class RichEditControlOptions : RichEditControlOptionsBase {
		public RichEditControlOptions(InnerRichEditDocumentServer documentServer)
			: base(documentServer) {
		}
	}
	#endregion
	#region SpecificFormatToContentConverter
	public abstract class SpecificFormatToContentConverter : IValueConverter {
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if (targetType != typeof(RichEditDocumentContent))
				throw new NotSupportedException(XpfRichEditLocalizer.GetString(RichEditControlStringId.Msg_TargetTypeNotSupported));
			return ConvertToContent(value);
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if (targetType != typeof(object) && targetType != TargetType)
				throw new NotSupportedException(XpfRichEditLocalizer.GetString(RichEditControlStringId.Msg_TargetTypeNotSupported));
			if (!(value is RichEditDocumentContent))
				return value;
			RichEditDocumentContent content = (RichEditDocumentContent)value;
			if (content.Control == null)
				return value;
			return ConvertToTargetType(content.Control);
		}
		#endregion
		protected internal virtual RichEditDocumentContent ConvertToContent(object value) {
			return new RichEditDocumentContent(Format, value);
		}
		protected internal abstract DocumentFormat Format { get; }
		protected internal abstract Type TargetType { get; }
		protected internal abstract object ConvertToTargetType(RichEditControl control);
	}
	#endregion
	#region RtfToContentConverter
#if !SL
	[ValueConversion(typeof(string), typeof(RichEditDocumentContent))]
#endif
	public class RtfToContentConverter : SpecificFormatToContentConverter {
		protected internal override DocumentFormat Format { get { return DocumentFormat.Rtf; } }
		protected internal override Type TargetType { get { return typeof(string); } }
		protected internal override object ConvertToTargetType(RichEditControl control) {
			return control.RtfText;
		}
	}
	#endregion
	#region PlainTextToContentConverter
#if !SL
	[ValueConversion(typeof(string), typeof(RichEditDocumentContent))]
#endif
	public class PlainTextToContentConverter : SpecificFormatToContentConverter {
		protected internal override DocumentFormat Format { get { return DocumentFormat.PlainText; } }
		protected internal override Type TargetType { get { return typeof(string); } }
		protected internal override object ConvertToTargetType(RichEditControl control) {
			return control.Text;
		}
	}
	#endregion
	#region HtmlToContentConverter
#if !SL
	[ValueConversion(typeof(string), typeof(RichEditDocumentContent))]
#endif
	public class HtmlToContentConverter : SpecificFormatToContentConverter {
		protected internal override DocumentFormat Format { get { return DocumentFormat.Html; } }
		protected internal override Type TargetType { get { return typeof(string); } }
		protected internal override object ConvertToTargetType(RichEditControl control) {
			return control.HtmlText;
		}
	}
	#endregion
	#region MhtToContentConverter
#if !SL
	[ValueConversion(typeof(string), typeof(RichEditDocumentContent))]
#endif
	public class MhtToContentConverter : SpecificFormatToContentConverter {
		protected internal override DocumentFormat Format { get { return DocumentFormat.Mht; } }
		protected internal override Type TargetType { get { return typeof(string); } }
		protected internal override RichEditDocumentContent ConvertToContent(object value) {
			return base.ConvertToContent(GetMhtString(value));
		}
		string GetMhtString(object value) {
			byte[] bytes = value as byte[];
			if (bytes != null)
				return EmptyEncoding.Instance.GetString(bytes, 0, bytes.Length);
			System.IO.MemoryStream stream = value as System.IO.MemoryStream;
			if (stream != null) {
				bytes = stream.ToArray();
				return EmptyEncoding.Instance.GetString(bytes, 0, bytes.Length);
			}
			return value as string;
		}
		protected internal override object ConvertToTargetType(RichEditControl control) {
			return control.MhtText;
		}
	}
	#endregion
	#region WordMLToContentConverter
#if !SL
	[ValueConversion(typeof(string), typeof(RichEditDocumentContent))]
#endif
	public class WordMLToContentConverter : SpecificFormatToContentConverter {
		protected internal override DocumentFormat Format { get { return DocumentFormat.WordML; } }
		protected internal override Type TargetType { get { return typeof(string); } }
		protected internal override object ConvertToTargetType(RichEditControl control) {
			return control.WordMLText;
		}
	}
	#endregion
	#region OpenXmlToContentConverter
#if !SL
	[ValueConversion(typeof(byte[]), typeof(RichEditDocumentContent))]
#endif
	public class OpenXmlToContentConverter : SpecificFormatToContentConverter {
		protected internal override DocumentFormat Format { get { return DocumentFormat.OpenXml; } }
		protected internal override Type TargetType { get { return typeof(byte[]); } }
		protected internal override object ConvertToTargetType(RichEditControl control) {
			return control.OpenXmlBytes;
		}
	}
	#endregion
	#region OpenDocumentToContentConverter
#if !SL
	[ValueConversion(typeof(byte[]), typeof(RichEditDocumentContent))]
#endif
	public class OpenDocumentToContentConverter : SpecificFormatToContentConverter {
		protected internal override DocumentFormat Format { get { return DocumentFormat.OpenDocument; } }
		protected internal override Type TargetType { get { return typeof(byte[]); } }
		protected internal override object ConvertToTargetType(RichEditControl control) {
			return control.OpenDocumentBytes;
		}
	}
	#endregion
	#region ContentToSpecificFormatConverter
	public abstract class ContentToSpecificFormatConverter : IValueConverter {
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return CreateConverter().ConvertBack(value, targetType, parameter, culture);
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return CreateConverter().Convert(value, targetType, parameter, culture);
		}
		#endregion
		protected internal abstract SpecificFormatToContentConverter CreateConverter();
	}
	#endregion
	#region ContentToRtfConverter
#if !SL
	[ValueConversion(typeof(RichEditDocumentContent), typeof(string))]
#endif
	public class ContentToRtfConverter : ContentToSpecificFormatConverter {
		protected internal override SpecificFormatToContentConverter CreateConverter() {
			return new RtfToContentConverter();
		}
	}
	#endregion
	#region ContentToPlainTextConverter
#if !SL
	[ValueConversion(typeof(RichEditDocumentContent), typeof(string))]
#endif
	public class ContentToPlainTextConverter : ContentToSpecificFormatConverter {
		protected internal override SpecificFormatToContentConverter CreateConverter() {
			return new PlainTextToContentConverter();
		}
	}
	#endregion
	#region ContentToHtmlConverter
#if !SL
	[ValueConversion(typeof(RichEditDocumentContent), typeof(string))]
#endif
	public class ContentToHtmlConverter : ContentToSpecificFormatConverter {
		protected internal override SpecificFormatToContentConverter CreateConverter() {
			return new HtmlToContentConverter();
		}
	}
	#endregion
	#region ContentToMhtConverter
#if !SL
	[ValueConversion(typeof(RichEditDocumentContent), typeof(string))]
#endif
	public class ContentToMhtConverter : ContentToSpecificFormatConverter {
		protected internal override SpecificFormatToContentConverter CreateConverter() {
			return new MhtToContentConverter();
		}
	}
	#endregion
	#region ContentToWordMLConverter
#if !SL
	[ValueConversion(typeof(RichEditDocumentContent), typeof(string))]
#endif
	public class ContentToWordMLConverter : ContentToSpecificFormatConverter {
		protected internal override SpecificFormatToContentConverter CreateConverter() {
			return new WordMLToContentConverter();
		}
	}
	#endregion
	#region ContentToOpenXmlConverter
#if !SL
	[ValueConversion(typeof(RichEditDocumentContent), typeof(byte[]))]
#endif
	public class ContentToOpenXmlConverter : ContentToSpecificFormatConverter {
		protected internal override SpecificFormatToContentConverter CreateConverter() {
			return new OpenXmlToContentConverter();
		}
	}
	#endregion
	#region ContentToOpenDocumentConverter
#if !SL
	[ValueConversion(typeof(RichEditDocumentContent), typeof(byte[]))]
#endif
	public class ContentToOpenDocumentConverter : ContentToSpecificFormatConverter {
		protected internal override SpecificFormatToContentConverter CreateConverter() {
			return new OpenDocumentToContentConverter();
		}
	}
	#endregion
	#region RichEditDocumentContent
	public struct RichEditDocumentContent {
		#region Fields
		readonly DocumentFormat format;
		readonly object content;
		RichEditControl control;
		int version;
		#endregion
		public RichEditDocumentContent(DocumentFormat format, object content) {
			this.format = format;
			this.content = content;
			this.control = null;
			this.version = 0;
		}
		#region Properties
		public DocumentFormat Format { get { return format; } }
		public object Content { get { return content; } }
		internal RichEditControl Control { get { return control; } set { control = value; } }
		internal int Version { get { return version; } set { version = value; } }
		#endregion
	}
	#endregion
#if !SL
	public class RichEditDocumentXpfPrinter {
		public static System.Windows.Documents.FixedDocument CreateFixedDocument(RichEditDocumentServer server) {
			Dictionary<FontInfo, WpfFontInfo> mapFontInfoToWpfFontInfo = new Dictionary<FontInfo, WpfFontInfo>(); ;
			try {
				XpfServerFixedDocumentPrinter printer = new XpfServerFixedDocumentPrinter(server, mapFontInfoToWpfFontInfo);
				return printer.CreateFixedDocument();
			}
			finally {
				foreach (var mapPair in mapFontInfoToWpfFontInfo) {
					mapPair.Value.Dispose();
				}
			}
		}
	}
#endif
}
namespace DevExpress.Xpf.RichEdit.Themes {
	public enum RichEditControlThemeKeys {
		BackgroundControlTemplate,
		BorderControlTemplate,
		PrintLayoutViewPageBorderControlTemplate,
		SimpleViewPageBorderControlTemplate,
		DraftViewPageBorderControlTemplate,
		RichEditCommentBorderControlTemplate,
		ResizePicturePlaceholderRectangleStyle,
		SelectionPathStyle,
		PageNumberWindowControlTemplate,
		RichEditBackgroundBrush,
		RichEditForegroundBrush,
		RichEditViewCommentsPresenterControlTemplate,
		CommentsAreaLeftPadding,
		CommentsAreaRightPadding,
		CommentContentPadding,
		CommentMoreButtonStyle,
		CommentLineOffset
	}
	public class RichEditControlThemeKeyExtension : ThemeKeyExtensionBase<RichEditControlThemeKeys> {
	}
	public enum RichEditRulerThemeKeys {
		HorizontalRulerControlTemplate,
		VerticalRulerControlTemplate,
		ActiveAreaControlTemplate,
		HorizontalSpaceAreaControlTemplate,
		VerticalSpaceAreaControlTemplate,
		TickMarkControlTemplate,
		NumberTickMarkControlTemplate,
		DefaultTabControlTemplate,
		DisabledHotZoneControlTemplate,
		LeftIndentHotZoneControlTemplate,
		FirstLineIndentHotZoneControlTemplate,
		LeftIndentBottomHotZoneControlTemplate,
		RightTabHotZoneControlTemplate,
		LeftTabHotZoneControlTemplate,
		CenterTabHotZoneControlTemplate,
		DecimalTabHotZoneControlTemplate,
		HorizontalTableHotZoneControlTemplate,
		TabTypeToggleBackgroundControlTemplate,
		TabTypeToggleHotZoneControlTemplate,
		HorizontalRulerBackgroundControlTemplate,
		VerticalRulerBackgroundControlTemplate,
		HorizontalRulerContentStyle,
		VerticalRulerContentStyle,
		HorizontalRulerShadowStyle,
		VerticalRulerShadowStyle,
	}
	public class RichEditRulerThemeKeyExtension : ThemeKeyExtensionBase<RichEditRulerThemeKeys> {
	}
	public enum RichEditOffice2007ColorThemeKeys {
		RichEditControlBorderBrush,
		RichEditControlBackgroundBrush,
		RichEditControlBackgroundPatternBrush,
		PageShadowBackgroundBrush,
		PageBorderBrush,
		RulerBackgroundBrush,
		RulerBorderBrush,
		RulerTabHotZoneBrush,
		RulerIndentHotZoneBackgroundBrush,
		RulerIndentHotZoneBorderBrush,
		RulerTableHotZoneBackgroundBrush,
		RulerTableHotZoneBorderBrush,
		RulerTickMarkBrush,
		RulerDefaultTabMarkBrush,
		RulerSpaceAreaBrush,
		TabTypeToggleHotZoneControlTemplateBackgroundBrush,
	}
	public class RichEditOffice2007ColorThemeKeyExtension : ThemeKeyExtensionBase<RichEditOffice2007ColorThemeKeys> {
	}
	public enum RichEditStyleEditThemeKeys {
		TextBlockStyle
	}
	public class RichEditStyleEditThemeKeyExtension : ThemeKeyExtensionBase<RichEditStyleEditThemeKeys> {
	}
}
namespace DevExpress.XtraRichEdit.Internal {
#if SL
	#region SilverlightMeasurementAndDrawingStrategy
	public class SilverlightMeasurementAndDrawingStrategy : PrecalculatedMetricsMeasurementAndDrawingStrategyBase {
		public SilverlightMeasurementAndDrawingStrategy(DocumentModel documentModel)
			: base(documentModel) {
		}
		public override Painter CreateDocumentPainter(IDrawingSurface surface) {
			XpfDrawingSurface xpfSurface = (XpfDrawingSurface)surface;
			return new XpfPainterOverwrite(DocumentModel.LayoutUnitConverter, xpfSurface);
		}
	}
	#endregion
#else
	#region WpfMeasurementAndDrawingStrategy
	public class WpfMeasurementAndDrawingStrategy : MeasurementAndDrawingStrategy {
		#region Fields
		GraphicsToLayoutUnitsModifier graphicsModifier;
		Graphics measureGraphics;
		#endregion
		public WpfMeasurementAndDrawingStrategy(DocumentModel documentModel)
			: base(documentModel) {
		}
		public override void Initialize() {
			this.measureGraphics = CreateMeasureGraphics();
			base.Initialize();
		}
		#region Properties
		public Graphics MeasureGraphics { get { return measureGraphics; } }
		#endregion
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (graphicsModifier != null) {
						graphicsModifier.Dispose();
						graphicsModifier = null;
					}
					if (measureGraphics != null) {
						measureGraphics.Dispose();
						measureGraphics = null;
					}
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		protected internal override void OnLayoutUnitChanged() {
			this.graphicsModifier.Dispose();
			this.graphicsModifier = new GraphicsToLayoutUnitsModifier(measureGraphics, DocumentModel.LayoutUnitConverter);
			base.OnLayoutUnitChanged();
		}
		protected internal virtual Graphics CreateMeasureGraphics() {
			Graphics result = Graphics.FromHwnd(IntPtr.Zero);
			this.graphicsModifier = new GraphicsToLayoutUnitsModifier(result, DocumentModel.LayoutUnitConverter);
			return result;
		}
		public override BoxMeasurer CreateBoxMeasurer() {
			return new WpfBoxMeasurer(DocumentModel, MeasureGraphics);
		}
		public override Painter CreateDocumentPainter(IDrawingSurface surface) {
			XpfDrawingSurface xpfSurface = (XpfDrawingSurface)surface;
			return new XpfPainterOverwrite(DocumentModel.LayoutUnitConverter, xpfSurface);
		}
		public override FontCacheManager CreateFontCacheManager() {
			return new WpfFontCacheManager(DocumentModel.LayoutUnitConverter);
		}
	}
	#endregion
#endif
	#region XpfRichEditToolTipHelper
	public class XpfRichEditToolTipHelper {
		readonly RichEditControl control;
		public XpfRichEditToolTipHelper(RichEditControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
		}
		public HyperlinkOptions Options { get { return control.Options.Hyperlinks; } }
		public ToolTipControlInfo CalculateHyperlinkToolTipInfo(HyperlinkInfo hyperlinkInfo) {
			ToolTipControlInfo result = new ToolTipControlInfo(hyperlinkInfo);
			result.Text = hyperlinkInfo.GetActualToolTip();
			result.Footer = GetHyperlinkToolTipFooter();
			return result;
		}
		public ToolTipControlInfo CalculateCommentViewInfoToolTipInfo(CommentViewInfo commentViewInfo) {
			ToolTipControlInfo result = new ToolTipControlInfo(commentViewInfo);
			RichEditToolTipHelper helper = new RichEditToolTipHelper();
			result.Header = helper.GetCommentToolTipHeader(commentViewInfo);
			result.Text = XtraRichEditLocalizer.GetString(XtraRichEditStringId.Msg_ClickToComment);
			return result;
		}
		public ToolTipControlInfo CalculateCommentToolTipInfo(Comment comment) {
			ToolTipControlInfo result = new ToolTipControlInfo(comment);
			RichEditToolTipHelper helper = new RichEditToolTipHelper();
			result.Text = helper.GetCommentToolTipText(comment);
			result.Header = helper.GetCommentToolTipHeader(comment);
			return result;
		}
		string GetHyperlinkToolTipFooter() {
			List<string> keyList = new List<string>();
			string keysStr = GetHyperlinkModifierKeysString(Options.ModifierKeys, keyList);
			string footer = XtraRichEditLocalizer.GetString(XtraRichEditStringId.Msg_ClickToFollowHyperlink);
			if (String.IsNullOrEmpty(keysStr))
				return footer;
			return String.Format("{0} + {1}", keysStr, footer);
		}
		string GetHyperlinkModifierKeysString(DXKeys keys, List<string> keyList) {
			if ((keys & DXKeys.Control) != 0)
				keyList.Add(XtraRichEditLocalizer.GetString(XtraRichEditStringId.KeyName_Control));
			if ((keys & DXKeys.Alt) != 0)
				keyList.Add(XtraRichEditLocalizer.GetString(XtraRichEditStringId.KeyName_Alt));
			if ((keys & DXKeys.Shift) != 0)
				keyList.Add(XtraRichEditLocalizer.GetString(XtraRichEditStringId.KeyName_Shift));
			return String.Join("+", keyList.ToArray());
		}
	}
	#endregion
	public class CustomBarDockInfo : BarDockInfo {
		readonly HoverMenuCalculator calculator;
		public CustomBarDockInfo(HoverMenuCalculator calculator, RichEditHoverMenu bar)
			: base(bar) {
			Guard.ArgumentNotNull(calculator, "calculator");
			this.calculator = calculator;
		}
		protected override void OnContainerChanged(DependencyPropertyChangedEventArgs e) {
			base.OnContainerChanged(e);
			calculator.OnBarDockContainerChanged();
		}
	}
	#region HoverMenuCalculator
	public class HoverMenuCalculator {
		RichEditHoverMenuAnimator hoverMenuAnimator = new RichEditHoverMenuAnimator();
		DateTime lastHoverChanged;
		public HoverMenuCalculator(RichEditControl control) {
			Control = control;
			CurrentHoverMenu = null;
			hoverMenuAnimator.Tick += HoverMenuAnimator_Tick;
		}
		public RichEditControl Control { get; private set; }
		public RichEditView ActiveView { get { return Control.ActiveView; } }
		public Panel Surface { get { return Control.Surface; } }
		public FrameworkElement Root {
			get {
				FrameworkElement root = Control.FocusElement;
				if (root == null)
					root = (FrameworkElement)LayoutHelper.GetTopLevelVisual(Control);
				return root;
			}
		}
		public RichEditHoverMenu CurrentHoverMenu { get; set; }
		bool IsDefaultHoverMenu { get { return Control != null && Control.IsDefaultHoverMenu; } }
		void HoverMenuAnimator_Tick(object sender, EventArgs e) {
			var timeIterval = DateTime.Now - lastHoverChanged;
			if (timeIterval.TotalMilliseconds < hoverMenuAnimator.Interval.TotalMilliseconds / 2)
				return;
			if (Control.LeftButtonPressed) {
				hoverMenuAnimator.Initiate();
				return;
			}
			CreateNewHover();
		}
		public void OnHoverChanged() {
			this.lastHoverChanged = DateTime.Now;
			CloseCurrentHover();
			if (CurrentHoverMenu != null) {
				BarDockInfo dockInfo = CurrentHoverMenu.DockInfo;
				if (dockInfo != null) {
					System.Windows.Point location = CalculatePosition();
					if (location != new System.Windows.Point())
						dockInfo.FloatBarOffset = location;
				}
			}
			hoverMenuAnimator.Initiate();
		}
		protected internal virtual void OnBarDockContainerChanged() {
			if (CurrentHoverMenu == null)
				return;
			FloatingBarPopup popup = GetBarPopup(CurrentHoverMenu);
			if (popup != null && popup.Child != null) {
				popup.Closed += OnPopupClosed;
				popup.Child.MouseMove += Root_MouseMove;
				popup.Child.MouseLeave += Root_MouseLeave;
				((ILinksHolder)CurrentHoverMenu).ImmediateActionsManager.EnqueueAction(UpdatePopupOffsetsByAlignments);
				ChangeHoverMenuOpacityAccordingLastMousePosition();
			}
		}
		protected virtual void CreateNewHover() {
			if (!Surface.IsInVisualTree())
				return;
			if (!Control.ShowHoverMenu || Control.DocumentModel.Selection.Length < 1 || Control.BarManager == null || Control.Menu.IsOpen || Control.IsFloatingObjectSelected)
				return;
			if (CurrentHoverMenu != null && CurrentHoverMenu.Visible)
				return;
			RichEditHoverMenu menu = ObtainHoverMenu();
			ShowHoverMenu(menu);
		}
		RichEditHoverMenu ObtainHoverMenu() {
			if (IsDefaultHoverMenu && this.CurrentHoverMenu != null)
				return this.CurrentHoverMenu;
			RichEditHoverMenuBuilder builder = new RichEditHoverMenuBuilder(Control, new XpfRichEditHoverMenuBuilderUIFactory(Control.BarManager));
			RichEditHoverMenu menu = (RichEditHoverMenu)builder.CreatePopupMenu();
			menu.Visible = false;
			menu.AllowCustomizationMenu = false;
			BarManagerHelper.SetBarIsPrivate(menu, true);
			BarDockInfo dockInfo = new CustomBarDockInfo(this, menu);
			dockInfo.ContainerType = BarContainerType.Floating;
			System.Windows.Point location = CalculatePosition();
			if (location == new System.Windows.Point())
				return null;
			dockInfo.FloatBarOffset = location;
			dockInfo.ShowHeaderInFloatBar = false;
			menu.DockInfo = dockInfo;
			Control.BarManager.Bars.Add(menu);
			menu = Control.RaiseHoverMenuShowing(menu);
#pragma warning disable 612 // Obsolete
#pragma warning disable 618 // Obsolete
			menu = Control.RaisePrepareHoverMenu(menu);
#pragma warning restore 618 // Obsolete
#pragma warning restore 612 // Obsolete
			return menu;
		}
		void ShowHoverMenu(RichEditHoverMenu menu) {
			if (menu == null || menu.ItemLinks.Count <= 0)
				return;
			CurrentHoverMenu = menu;
			Root.MouseMove -= Root_MouseMove;
			Root.MouseMove += Root_MouseMove;
			Root.MouseLeave -= Root_MouseLeave;
			Root.MouseLeave += Root_MouseLeave;
			menu.Visible = true;
			UpdatePopupOffsetsByAlignments();
			UpdateHoverItemsState(menu);
			hoverMenuAnimator.Stop();
			ChangeHoverMenuOpacityAccordingLastMousePosition();
		}
		void UpdatePopupOffsetsByAlignments() {
			FloatingBarPopup popup = GetBarPopup(CurrentHoverMenu);
			if (popup == null || popup.Child == null)
				return;
			popup.VerticalOffset -= popup.Child.RenderSize.Height;
		}
		void UpdateHoverItemsState(RichEditHoverMenu menu) {
			BarManager barManager = menu.Manager;
			if (barManager == null)
				return;
			BarItemLinkCollection itemLinks = menu.ItemLinks;
			if (itemLinks == null)
				return;
			for (int i = itemLinks.Count - 1; i >= 0; i--) {
				BarItemLink link = itemLinks[i] as BarItemLink;
				if (link != null) {
					BarItem item = link.Item;
					if (item != null)
						Control.CommandManager.UpdateBarItemState(item);
				}
			}
		}
		double Distance(System.Windows.Point p, Rect r) {
			if (p.X > r.Left && p.Y > r.Top && p.X < r.Right && p.Y < r.Bottom)
				return 0;
			if (p.Y > r.Bottom) {
				if (p.X < r.Left)
					return p.Distance(r.Corner(RectCorner.BottomLeft));
				if (p.X > r.Right)
					return p.Distance(r.Corner(RectCorner.BottomRight));
				return p.Y - r.Bottom;
			}
			if (p.Y < r.Top) {
				if (p.X < r.Left)
					return p.Distance(r.Corner(RectCorner.TopLeft));
				if (p.X > r.Right)
					return p.Distance(r.Corner(RectCorner.TopRight));
				return r.Bottom - p.Y;
			}
			if (p.X < r.Left)
				return r.Left - p.X;
			if (p.X > r.Right)
				return p.X - r.Right;
			return 0;
		}
		FloatingBarPopup GetBarPopup(RichEditHoverMenu menu) {
			if (menu == null)
				return null;
			FloatingBarContainerControl container = menu.DockInfo.Container as FloatingBarContainerControl;
			if (container == null)
				return null;
			return FloatingBarPopupHelper.GetPopup(container);
		}
		void Root_MouseMove(object sender, MouseEventArgs e) {
			if (CurrentHoverMenu == null) {
				Root.MouseMove -= Root_MouseMove;
				Root.MouseLeave -= Root_MouseLeave;
				return;
			}
			if (Control.BarManager == null || !Control.BarManager.IsInVisualTree())
				return;
			ChangeHoverMenuOpacity(e.GetPosition(Control.BarManager)); 
		}
		void Root_MouseLeave(object sender, MouseEventArgs e) {
			if (CurrentHoverMenu == null) {
				Root.MouseMove -= Root_MouseMove;
				Root.MouseLeave -= Root_MouseLeave;
				return;
			}
			if (Control.BarManager == null || !Control.BarManager.IsInVisualTree())
				return;
			if (sender is DevExpress.Xpf.Bars.FloatingBarPopupContentControl) {
				var posRelativeToRich = e.GetPosition(Root);
				var rootSize = Root.RenderSize;
				if (posRelativeToRich.X < 0 || posRelativeToRich.Y < 0 || posRelativeToRich.X > rootSize.Width || posRelativeToRich.Y > rootSize.Height)
					CloseCurrentHover();
			}
			else if (sender is System.Windows.Controls.ContentControl) {
				FloatingBarPopup popup = GetBarPopup(CurrentHoverMenu);
				if (popup == null || popup.Child == null) {
					CloseCurrentHover();
					return;
				}
				var posRelativeToPopup = e.GetPosition(popup.Child);
				var popupSize = popup.Child.RenderSize;
				if (posRelativeToPopup.X < 0 || posRelativeToPopup.Y < 0 || posRelativeToPopup.X > popupSize.Width || posRelativeToPopup.Y > popupSize.Height)
					CloseCurrentHover();
			}
		}
		void ChangeHoverMenuOpacityAccordingLastMousePosition() {
			if (Control.BarManager == null || !Control.BarManager.IsInVisualTree())
				return;
			ChangeHoverMenuOpacity(Control.TranslatePoint(Control.MousePosition, Control.BarManager));
		}
		void ChangeHoverMenuOpacity(System.Windows.Point barManagerPoint) {
			if (Control.BarManager == null || !Control.BarManager.IsInVisualTree())
				return;
			FloatingBarPopup popup = GetBarPopup(CurrentHoverMenu);
			if (popup == null || popup.Child == null)
				return;
			System.Windows.Size size = popup.Child.RenderSize;
#if !SL
			var screenRect = ScreenHelper.GetScreenRect(popup);
			double menuDropAlignmentCorrection = SystemParameters.MenuDropAlignment ? size.Width : 0;
			System.Windows.Point topLeft = Control.BarManager.PointFromScreen(screenRect.TopLeft);
			double popupRightSidePos = popup.HorizontalOffset + (SystemParameters.MenuDropAlignment ? 0 : size.Width);
			System.Windows.Point bottomRight = Control.BarManager.PointFromScreen(screenRect.BottomRight);
#else
			var actualWidth = (double)System.Windows.Browser.HtmlPage.Window.Eval("screen.width");
			var actualHeight = (double)System.Windows.Browser.HtmlPage.Window.Eval("screen.height");
			double menuDropAlignmentCorrection = 0;
			double popupRightSidePos = popup.HorizontalOffset + size.Width;
			var transform = Control.BarManager.TransformToVisual(Application.Current.RootVisual);
			var barManagerOffsetOnTheScreen = transform.Transform(new System.Windows.Point(0, 0));
			System.Windows.Point topLeft = new System.Windows.Point(-barManagerOffsetOnTheScreen.X, -barManagerOffsetOnTheScreen.Y);
			System.Windows.Point bottomRight = new System.Windows.Point(actualWidth - barManagerOffsetOnTheScreen.X, actualHeight - barManagerOffsetOnTheScreen.Y);
#endif
			double horizontalOffset = popup.HorizontalOffset - menuDropAlignmentCorrection;
			horizontalOffset = Math.Max(horizontalOffset, topLeft.X);
			if (popupRightSidePos > bottomRight.X)
				horizontalOffset -= popupRightSidePos - bottomRight.X;
			double verticalOffset = popup.VerticalOffset;
			double distance = Distance(barManagerPoint, new Rect(horizontalOffset, verticalOffset, size.Width, size.Height));
			const double hoverVisibilityDistance = 150;
			double distance2 = (hoverVisibilityDistance - distance) * (hoverVisibilityDistance - distance);
			double opacity = (distance > hoverVisibilityDistance ? 0 : distance2) / (hoverVisibilityDistance * hoverVisibilityDistance);
			popup.Child.Opacity = opacity;
			if (distance >= hoverVisibilityDistance * 2)
				CloseCurrentHover();
		}
		protected virtual System.Windows.Point CalculatePosition() {
			if (!Surface.IsInVisualTree() || Control.BarManager == null || !Control.BarManager.IsInVisualTree() || Control.IsUpdateLocked)
				return new System.Windows.Point();
			DevExpress.XtraRichEdit.Layout.Page page = ActiveView.SelectionLayout.StartLayoutPosition.Page;
			if (page == null)
				return new System.Windows.Point();
			PageViewInfo pageViewInfo = ActiveView.LookupPageViewInfoByPage(page);
			if (pageViewInfo == null)
				return new System.Windows.Point();
			if (!ActiveView.SelectionLayout.StartLayoutPosition.Update(ActiveView.DocumentLayout.Pages, DocumentLayoutDetailsLevel.Character))
				return new System.Windows.Point();
			System.Drawing.Point logicalPoint = ActiveView.SelectionLayout.StartLayoutPosition.Character.Bounds.Location;
			System.Drawing.Point physicalPoint = ActiveView.CreatePhysicalPoint(pageViewInfo, logicalPoint);
			System.Drawing.Point controlPoint = ActiveView.DocumentModel.LayoutUnitConverter.LayoutUnitsToPixels(physicalPoint, Control.DpiX, Control.DpiY);
			System.Windows.Point position = new System.Windows.Point(controlPoint.X, controlPoint.Y);
			position = Surface.TranslatePoint(position, Control.BarManager);
			return position;
		}
		protected internal virtual void OnPopupClosed(object sender, EventArgs e) {
			CloseCurrentHover();
		}
		protected internal void ForceCloseCurrentHover() {
			if (CurrentHoverMenu == null)
				return;
			FloatingBarPopup popup = GetBarPopup(CurrentHoverMenu);
			if (popup != null) {
				popup.Closed -= OnPopupClosed;
				if (popup.Child != null) {
					popup.Child.MouseMove -= Root_MouseMove;
					popup.Child.MouseLeave -= Root_MouseLeave;
				}
			}
			if (popup != null)
				popup.IsOpen = false;
			CurrentHoverMenu.Visible = false;
			CurrentHoverMenu.ItemLinks.Clear();
			CurrentHoverMenu.Manager.Bars.Remove(CurrentHoverMenu);
			CurrentHoverMenu = null;
		}
		public virtual void CloseCurrentHover() {
			if (IsDefaultHoverMenu) {
				if (CurrentHoverMenu != null)
					CurrentHoverMenu.Visible = false;
				return;
			}
			ForceCloseCurrentHover();
		}
	}
	#endregion
	#region RichEditHoverMenuAnimator
	public class RichEditHoverMenuAnimator {
		public RichEditHoverMenuAnimator() {
			Timer = new DispatcherTimer();
			Timer.Interval = Interval;
			Stop();
		}
		protected DispatcherTimer Timer { get; set; }
		public TimeSpan Interval { get { return TimeSpan.FromMilliseconds(500); } }
		public event EventHandler Tick;
		public void Initiate() {
			Stop();
			Timer.Start();
			Timer.Tick += OnTimerTick;
		}
		public void Stop() {
			Timer.Stop();
			Timer.Tick -= OnTimerTick;
		}
		protected virtual void OnTimerTick(object sender, EventArgs e) {
			OnTick();
		}
		protected virtual void OnTick() {
			Stop();
			if (Tick == null)
				return;
			Tick(this, EventArgs.Empty);
		}
	}
	#endregion
	#region XpfRichEditCommandFactoryService
	public class XpfRichEditCommandFactoryService : RichEditCommandFactoryService {
		static readonly Type[] constructorParametersInterface = new Type[] { typeof(RichEditControl) };
		public XpfRichEditCommandFactoryService(IRichEditControl control)
			: base(control) {
		}
		protected internal override ConstructorInfo GetConstructorInfo(Type commandType) {
			ConstructorInfo ci = base.GetConstructorInfo(commandType);
			if (ci == null)
				ci = commandType.GetConstructor(constructorParametersInterface);
			return ci;
		}
		protected internal override void PopulateConstructorTable(RichEditCommandConstructorTable table) {
			base.PopulateConstructorTable(table);
#if SL
			AddCommandConstructor(table, RichEditCommandId.BrowserPrint, typeof(BrowserPrintCommand));
			AddCommandConstructor(table, RichEditCommandId.BrowserPrintPreview, typeof(BrowserPrintPreviewCommand));
#else
			AddCommandConstructor(table, RichEditCommandId.FindNext, typeof(FindCommand));
			AddCommandConstructor(table, RichEditCommandId.FindPrev, typeof(FindCommand));
			AddCommandConstructor(table, RichEditCommandId.QuickPrint, typeof(QuickPrintCommand));
			AddCommandConstructor(table, RichEditCommandId.FileSave, typeof(SaveDocumentCommand));
#endif
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Menu {
	public class ButtonEditAdapter : IButtonEditAdapter {
		readonly ButtonEdit edit;
		public ButtonEditAdapter(ButtonEdit edit) {
			Guard.ArgumentNotNull(edit, "edit");
			this.edit = edit;
		}
		#region IButtonEditAdapter Members
		public string Text { get { return edit.Text; } set { edit.Text = value; } }
		public void Select(int start, int length) {
			edit.Select(start, length);
		}
		#endregion
	}
}
namespace DevExpress.XtraRichEdit.Internal {
	using DevExpress.Data.Utils;
#if !SL
	public class ThreadIdleWeakEventHandler<TOwner> : WeakEventHandler<TOwner, EventArgs, EventHandler> where TOwner : class {
		public ThreadIdleWeakEventHandler(TOwner owner, Action<TOwner, Object, EventArgs> onEventAction)
			: base(owner, onEventAction, OnDetach, CreateHandler) {
		}
		static EventHandler CreateHandler(WeakEventHandler<TOwner, EventArgs, EventHandler> handler) {
			return new EventHandler(handler.OnEvent);
		}
		static void OnDetach(WeakEventHandler<TOwner, EventArgs, EventHandler> handler, object obj) {
			System.Windows.Interop.ComponentDispatcher.ThreadIdle -= handler.Handler;
		}
	}
#endif
	public interface IRichEditControlDependencyPropertyOwner {
		DependencyProperty DependencyProperty { get; }
	}
	public class FontAndForeColorPropertyListener : FrameworkElement {
		readonly RichEditControl control;
		bool listening;
		public static readonly DependencyProperty FontFamilyProperty = DependencyPropertyManager.Register("FontFamily", typeof(System.Windows.Media.FontFamily), typeof(FontAndForeColorPropertyListener), new PropertyMetadata(null, OnObjectPropertyChanged));
		public static readonly DependencyProperty FontSizeProperty = DependencyPropertyManager.Register("FontSize", typeof(double), typeof(FontAndForeColorPropertyListener), new PropertyMetadata(0.0, OnObjectPropertyChanged));
		public static readonly DependencyProperty FontWeightProperty = DependencyPropertyManager.Register("FontWeight", typeof(FontWeight), typeof(FontAndForeColorPropertyListener), new PropertyMetadata(FontWeights.Normal, OnObjectPropertyChanged));
		public static readonly DependencyProperty FontStyleProperty = DependencyPropertyManager.Register("FontStyle", typeof(System.Windows.FontStyle), typeof(FontAndForeColorPropertyListener), new PropertyMetadata(FontStyles.Normal, OnObjectPropertyChanged));
		public static readonly DependencyProperty ForegroundProperty = DependencyPropertyManager.Register("Foreground", typeof(System.Windows.Media.Brush), typeof(FontAndForeColorPropertyListener), new PropertyMetadata(null, OnObjectPropertyChanged));
		public FontAndForeColorPropertyListener(RichEditControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
		}
		public void BeginListening() {
			EndListening();
			CreateBinding(FontFamilyProperty, "FontFamily");
			CreateBinding(FontSizeProperty, "FontSize");
			CreateBinding(FontWeightProperty, "FontWeight");
			CreateBinding(FontStyleProperty, "FontStyle");
			CreateBinding(ForegroundProperty, "Foreground");
			this.listening = true;
		}
		public void EndListening() {
			this.listening = false;
			ClearValue(FontFamilyProperty);
			ClearValue(FontSizeProperty);
			ClearValue(FontWeightProperty);
			ClearValue(FontStyleProperty);
			ClearValue(ForegroundProperty);
		}
		void CreateBinding(DependencyProperty property, string name) {
			Binding binding = new Binding();
			binding.Path = new PropertyPath(name);
			binding.Source = control;
			binding.Mode = BindingMode.OneWay;
			SetBinding(property, binding);
		}
		static void OnObjectPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			FontAndForeColorPropertyListener obj = d as FontAndForeColorPropertyListener;
			if (obj != null)
				obj.OnObjectPropertyChanged(e);
		}
		void OnObjectPropertyChanged(DependencyPropertyChangedEventArgs e) {
			if (listening)
				control.ApplyFontAndForeColor();
		}
	}
}
namespace DevExpress.Xpf.RichEdit.Controls.Internal {
	public class DXRichEditThemesLoader : DXThemesLoaderBase {
		protected override Type TargetType { get { return typeof(RichEditControl); } }
	}
	#region RichEditControlBorder
	public class RichEditControlBorder : DXRichEditThemesLoader {
		#region VisibleBorderTemplate
		public static readonly DependencyProperty VisibleBorderTemplateProperty = DependencyPropertyManager.Register(
				"VisibleBorderTemplate",
				typeof(ControlTemplate),
				typeof(RichEditControlBorder),
				new UIPropertyMetadata(null, new PropertyChangedCallback(OnVisibleBorderTemplateChanged)));
		public ControlTemplate VisibleBorderTemplate {
			get { return (ControlTemplate)GetValue(VisibleBorderTemplateProperty); }
			set { SetValue(VisibleBorderTemplateProperty, value); }
		}
		static void OnVisibleBorderTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			RichEditControlBorder instance = (RichEditControlBorder)d;
			instance.OnVisibleBorderTemplateChanged((ControlTemplate)e.OldValue, (ControlTemplate)e.NewValue);
		}
		#endregion
		#region InvisibleBorderTemplate
		public static readonly DependencyProperty InvisibleBorderTemplateProperty = DependencyPropertyManager.Register(
				"InvisibleBorderTemplate",
				typeof(ControlTemplate),
				typeof(RichEditControlBorder),
				new UIPropertyMetadata(null, new PropertyChangedCallback(OnInvisibleBorderTemplateChanged)));
		public ControlTemplate InvisibleBorderTemplate {
			get { return (ControlTemplate)GetValue(InvisibleBorderTemplateProperty); }
			set { SetValue(InvisibleBorderTemplateProperty, value); }
		}
		static void OnInvisibleBorderTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			RichEditControlBorder instance = (RichEditControlBorder)d;
			instance.OnInvisibleBorderTemplateChanged((ControlTemplate)e.OldValue, (ControlTemplate)e.NewValue);
		}
		#endregion
		#region ShowBorder
		public static readonly DependencyProperty ShowBorderProperty = DependencyPropertyManager.Register(
				"ShowBorder",
				typeof(bool),
				typeof(RichEditControlBorder),
				new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowBorderChanged)));
		public bool ShowBorder {
			get { return (bool)GetValue(ShowBorderProperty); }
			set { SetValue(ShowBorderProperty, value); }
		}
		static void OnShowBorderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			RichEditControlBorder instance = (RichEditControlBorder)d;
			instance.OnShowBorderChanged((bool)e.OldValue, (bool)e.NewValue);
		}
		#endregion
		protected internal virtual void OnVisibleBorderTemplateChanged(ControlTemplate oldValue, ControlTemplate newValue) {
			SelectTemplate();
		}
		protected internal virtual void OnInvisibleBorderTemplateChanged(ControlTemplate oldValue, ControlTemplate newValue) {
			SelectTemplate();
		}
		protected internal virtual void OnShowBorderChanged(bool oldValue, bool newValue) {
			SelectTemplate();
		}
		protected internal virtual void SelectTemplate() {
			ControlTemplate template = ShowBorder ? VisibleBorderTemplate : InvisibleBorderTemplate;
			if (!Object.ReferenceEquals(Template, template))
				Template = template;
		}
	}
	#endregion
}
