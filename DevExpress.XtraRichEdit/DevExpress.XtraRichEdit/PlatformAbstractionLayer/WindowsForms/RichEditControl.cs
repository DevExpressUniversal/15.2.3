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
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Security.Permissions;
using System.Text;
using System.Windows.Forms;
using DevExpress.Services;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Text;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
using DevExpress.XtraRichEdit.Drawing;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.XtraRichEdit.Forms;
using DevExpress.XtraEditors.Controls;
using DevExpress.Utils.Serializing;
using DevExpress.XtraSpellChecker;
using DevExpress.Utils.Controls;
using DevExpress.XtraPrinting;
using DevExpress.XtraRichEdit.Painters;
using DevExpress.XtraRichEdit.Layout.Export;
using DevExpress.Utils.Menu;
using DevExpress.XtraRichEdit.Menu;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Internal.PrintLayout;
using DevExpress.Data;
using DevExpress.XtraRichEdit.Native;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Services;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Mouse;
using DevExpress.XtraRichEdit.Ruler;
using DevExpress.XtraRichEdit.Services.Implementation;
using DevExpress.Services.Implementation;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Tables.Native;
using DevExpress.XtraRichEdit.Printing;
using DevExpress.Utils.Gesture;
using DevExpress.Office;
using DevExpress.Office.Drawing;
using DevExpress.Office.Services.Implementation;
using DevExpress.Office.Utils;
using DevExpress.Utils.Commands;
using System.Drawing.Drawing2D;
using DevExpress.Office.PInvoke;
using DevExpress.Office.Internal;
using DevExpress.XtraEditors.Drawing;
using DevExpress.Utils.About;
using DevExpress.Office.Forms;
using DevExpress.Skins;
using DevExpress.XtraPrinting.Links;
using DevExpress.XtraPrinting.Preview;
using DevExpress.Utils.Internal;
using Debug = System.Diagnostics.Debug;
#if !DXPORTABLE
using DevExpress.Pdf;
using System.Diagnostics;
#endif
namespace DevExpress.XtraRichEdit {
	[
	System.Runtime.InteropServices.ComVisible(false),
	ToolboxBitmap(typeof(RichEditControl), DevExpress.Utils.ControlConstants.BitmapPath + "RichTextControl.bmp"),
	DXToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabRichEdit),
	Designer("DevExpress.XtraRichEdit.Design.XtraRichEditDesigner," + AssemblyInfo.SRAssemblyRichEditDesign),
	Docking(DockingBehavior.Ask),
	Description("A control to create, load, modify, print, save and convert rich text documents in different formats."),
]
	public partial class RichEditControl : Control, IPrintable, ISupportLookAndFeel, IToolTipControlClient, IGestureClient, IRectangularObjectRotationControllerOwner, IMouseWheelSupport {
		#region Fields
		bool isDisposing;
		bool isDisposed;
		bool useGdiPlus;
		AutoSizeMode autoSizeMode;
		Size borderSize;
		DevExpress.XtraEditors.VScrollBar verticalScrollbar;
		DevExpress.XtraEditors.HScrollBar horizontalScrollbar;
		Caret caret;
		DragCaret dragCaret;
		Timer caretTimer;
		Timer verticalScrollBarUpdateTimer;
		Timer flushPendingTextInputTimer;
		Timer flushRotationControllerTimer;
		readonly LeakSafeEventRouter leakSafeEventRouter;
		RichEditControlPainter painter;
		BorderStyles borderStyle = BorderStyles.Default;
		UserLookAndFeel lookAndFeel;
		Rectangle lastValidClientRectangle; 
		Rectangle clientBounds; 
		Rectangle cornerBounds; 
		Rectangle backgroundBounds; 
		Rectangle viewBounds; 
		SearchTextForm searchForm;
		InsertMergeFieldForm insertMergeFieldForm;
		RichEditViewBackgroundPainter backgroundPainter;
		RichEditViewPainter viewPainter;
		IDXMenuManager menuManager;
		RichEditAppearance appearance;
		HorizontalRulerControl horizontalRuler;
		VerticalRulerControl verticalRuler;
		ToolTipController toolTipController;
		DragDropMode dragDropMode;
		bool acceptsTab = true;
		bool acceptsReturn = true;
		bool acceptsEscape = true;
		bool showCaretInReadOnly = true;
		bool useDeferredDataBindingNotifications = true;
		GestureHelper gestureHelper;
		internal static readonly IntPtr systemCaretBitmapHandle;
		RectangularObjectRotationController rotationController;
		CommentPadding commentPadding;
		bool isCaretDestroyed = true;
		#endregion
		static RichEditControl() {
			Bitmap systemCaretBitmap = CreateSystemCaretBitmap();
			systemCaretBitmapHandle = CreateSystemCaretBitmapHandle(systemCaretBitmap);
		}
		[System.Security.SecuritySafeCritical]
		static IntPtr CreateSystemCaretBitmapHandle(Bitmap systemCaretBitmap) {
			return systemCaretBitmap.GetHbitmap(Color.White);
		}
		bool insideHandleDestroyed;
		public RichEditControl()
			: this(false) {
			Validate();
		}
		internal RichEditControl(bool useGdiPlus) {
			this.useGdiPlus = useGdiPlus;
			this.leakSafeEventRouter = new LeakSafeEventRouter(this);
			this.AllowDrop = true;
			this.innerControl = CreateInnerControl();
			this.gestureHelper = new GestureHelper(this);
			BeginInitialize();
			LayoutUnit = DefaultLayoutUnit;
			EndInitialize();
			RegisterToolTipClient();
		}
		protected virtual void Validate() {
		}
		#region Properties
		internal bool InnerIsDisposed { get { return isDisposed; } }
		internal bool InnerIsDisposing { get { return isDisposing; } }
		internal ScrollBarBase VerticalScrollBar { get { return verticalScrollbar; } }
		internal ScrollBarBase HorizontalScrollBar { get { return horizontalScrollbar; } }
		protected override Size DefaultSize { get { return new Size(400, 200); } }
		protected internal Caret Caret { get { return caret; } }
		protected internal DragCaret DragCaret { get { return dragCaret; } }
		protected internal Timer CaretTimer { get { return caretTimer; } }
		protected internal Timer VerticalScrollBarUpdateTimer { get { return verticalScrollBarUpdateTimer; } }
		protected internal Timer FlushPendingTextInputTimer { get { return flushPendingTextInputTimer; } }
		protected internal HorizontalRulerControl HorizontalRuler { get { return horizontalRuler; } }
		protected internal VerticalRulerControl VerticalRuler { get { return verticalRuler; } }
		protected internal RichEditControlPainter Painter { get { return painter; } }
		protected internal Size BorderSize { get { return borderSize; } }
		[
#if !SL
	DevExpressXtraRichEditLocalizedDescription("RichEditControlAutoSizeMode"),
#endif
		DefaultValue(AutoSizeMode.None)]
		public AutoSizeMode AutoSizeMode {
			get { return autoSizeMode; }
			set {
				if (autoSizeMode == value)
					return;
				autoSizeMode = value;
				AutoSizeModeChanged();
			}
		}
		#region BorderStyle
		[
#if !SL
	DevExpressXtraRichEditLocalizedDescription("RichEditControlBorderStyle"),
#endif
Category(CategoryName.Appearance), DefaultValue(BorderStyles.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public BorderStyles BorderStyle {
			get { return borderStyle; }
			set {
				if (borderStyle == value)
					return;
				borderStyle = value;
				OnBorderStyleChanged();
			}
		}
		[
#if !SL
	DevExpressXtraRichEditLocalizedDescription("RichEditControlOptions"),
#endif
 NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RichEditControlOptions Options { get { return InnerControl != null ? (RichEditControlOptions)InnerControl.Options : null; } }
		#endregion
		protected internal Rectangle ClientBounds { get { return clientBounds; } }
		protected internal Rectangle BackgroundBounds { get { return backgroundBounds; } }
		protected internal Rectangle CornerBounds { get { return cornerBounds; } }
		Rectangle IRichEditControl.ViewBounds { get { return this.ViewBounds; } }
		bool IRichEditControl.ShowCaretInReadOnly { get { return this.ShowCaretInReadOnly; } }
		protected internal Rectangle ViewBounds { get { return viewBounds; } }
		protected internal SearchTextForm SearchForm { get { return searchForm; } }
		protected internal RichEditViewBackgroundPainter BackgroundPainter { get { return backgroundPainter; } }
		protected internal RichEditViewPainter ViewPainter { get { return viewPainter; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override Image BackgroundImage { get { return base.BackgroundImage; } set { base.BackgroundImage = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override ImageLayout BackgroundImageLayout { get { return base.BackgroundImageLayout; } set { base.BackgroundImageLayout = value; } }
		#region ForeColor
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override Color ForeColor {
			get {
				if (appearance == null || appearance.Text == null)
					return DXColor.Empty;
				return appearance.Text.ForeColor;
			}
			set {
				if (appearance == null || appearance.Text == null)
					return;
				appearance.Text.ForeColor = value;
			}
		}
		#endregion
		Color backColor = Color.Empty;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new Color BackColor { get { return backColor; } set { backColor = value; } }
		#region Font
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override Font Font {
			get {
				if (appearance == null || appearance.Text == null)
					return null;
				return appearance.Text.Font;
			}
			set {
				if (appearance == null || appearance.Text == null)
					return;
				appearance.Text.Font = value;
			}
		}
		[
#if !SL
	DevExpressXtraRichEditLocalizedDescription("RichEditControlAppearance"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RichEditAppearance Appearance { get { return appearance; } }
		#endregion
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override RightToLeft RightToLeft { get { return base.RightToLeft; } set { base.RightToLeft = value; } }
		protected override bool ScaleChildren { get { return false; } }
		[
#if !SL
	DevExpressXtraRichEditLocalizedDescription("RichEditControlMenuManager"),
#endif
			DefaultValue(null)]
		public IDXMenuManager MenuManager { get { return menuManager; } set { menuManager = value; } }
		internal ToolTipController ActualToolTipController { get { return toolTipController != null ? toolTipController : ToolTipController.DefaultController; } }
		#region ToolTipController
		[
#if !SL
	DevExpressXtraRichEditLocalizedDescription("RichEditControlToolTipController"),
#endif
 Category(CategoryName.Appearance), DefaultValue(null)]
		public ToolTipController ToolTipController {
			get { return toolTipController; }
			set {
				if (value == ToolTipController.DefaultController)
					value = null;
				if (ToolTipController == value)
					return;
				UnsubscribeToolTipControllerEvents(ActualToolTipController);
				UnregisterToolTipClientControl(ActualToolTipController);
				this.toolTipController = value;
				RegisterToolTipClientControl(ActualToolTipController);
				SubscribeToolTipControllerEvents(ActualToolTipController);
			}
		}
		#endregion
		[DefaultValue(true), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool AllowDrop { get { return base.AllowDrop; } set { base.AllowDrop = value; } }
		[
#if !SL
	DevExpressXtraRichEditLocalizedDescription("RichEditControlDragDropMode"),
#endif
 DefaultValue(DragDropMode.Standard)]
		public DragDropMode DragDropMode { get { return dragDropMode; } set { dragDropMode = value; } }
		[DefaultValue(false)]
		internal bool UseGdiPlus {
			get { return useGdiPlus; }
			set {
				if (useGdiPlus == value)
					return;
				useGdiPlus = value;
				OnUseGdiPlusChanged();
			}
		}
		bool IRichEditControl.UseGdiPlus { get { return this.UseGdiPlus; } }
		#region AcceptsTab
		[
#if !SL
	DevExpressXtraRichEditLocalizedDescription("RichEditControlAcceptsTab"),
#endif
		DefaultValue(true)]
		public bool AcceptsTab {
			get { return acceptsTab; }
			set {
				if (acceptsTab == value)
					return;
				acceptsTab = value;
			}
		}
		#endregion
		#region AcceptsReturn
		[
#if !SL
	DevExpressXtraRichEditLocalizedDescription("RichEditControlAcceptsReturn"),
#endif
		DefaultValue(true)]
		public bool AcceptsReturn {
			get { return acceptsReturn; }
			set {
				if (acceptsReturn == value)
					return;
				acceptsReturn = value;
			}
		}
		#endregion
		#region AcceptsEscape
		[
#if !SL
	DevExpressXtraRichEditLocalizedDescription("RichEditControlAcceptsEscape"),
#endif
		DefaultValue(true)]
		public bool AcceptsEscape {
			get { return acceptsEscape; }
			set {
				if (acceptsEscape == value)
					return;
				acceptsEscape = value;
			}
		}
		#endregion
		[
#if !SL
	DevExpressXtraRichEditLocalizedDescription("RichEditControlShowCaretInReadOnly"),
#endif
		DefaultValue(true)]
		public bool ShowCaretInReadOnly {
			get { return showCaretInReadOnly; }
			set {
				if (value == showCaretInReadOnly)
					return;
				showCaretInReadOnly = value;
				OnReadOnlyChangedPlatformSpecific();
			}
		}
		protected internal virtual bool ShouldShowCaret {
			get {
				if (!Enabled || !Focused)
					return false;
				if (ShowCaretInReadOnly)
					return true;
				else
					return !ReadOnly;
			}
		}
		#region UseDeferredDataBindingNotifications
		[
#if !SL
	DevExpressXtraRichEditLocalizedDescription("RichEditControlUseDeferredDataBindingNotifications"),
#endif
		DefaultValue(true)]
		public bool UseDeferredDataBindingNotifications { get { return useDeferredDataBindingNotifications; } set { useDeferredDataBindingNotifications = value; } }
		#endregion
		#endregion
		#region ISupportLookAndFeel Members
		bool ISupportLookAndFeel.IgnoreChildren { get { return false; } }
		[
#if !SL
	DevExpressXtraRichEditLocalizedDescription("RichEditControlLookAndFeel"),
#endif
Category(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public UserLookAndFeel LookAndFeel { get { return lookAndFeel; } }
		#endregion
		#region Events
		#region BeforeDispose
		static readonly object onBeforeDispose = new object();
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlBeforeDispose")]
#endif
		public event EventHandler BeforeDispose {
			add { Events.AddHandler(onBeforeDispose, value); }
			remove { Events.RemoveHandler(onBeforeDispose, value); }
		}
		protected internal virtual void RaiseBeforeDispose() {
			EventHandler handler = (EventHandler)Events[onBeforeDispose];
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
		#endregion
		#region FontFormShowing
		static readonly object onFontFormShowing = new object();
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlFontFormShowing")]
#endif
		public event FontFormShowingEventHandler FontFormShowing {
			add { Events.AddHandler(onFontFormShowing, value); }
			remove { Events.RemoveHandler(onFontFormShowing, value); }
		}
		protected internal virtual void RaiseFontFormShowing(FontFormShowingEventArgs e) {
			FontFormShowingEventHandler handler = (FontFormShowingEventHandler)Events[onFontFormShowing];
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region FloatingObjectLayoutOptionsFormShowing
		static readonly object onFloatingInlineObjectLayoutOptionsFormShowing = new object();
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlFloatingInlineObjectLayoutOptionsFormShowing")]
#endif
		public event FloatingInlineObjectLayoutOptionsFormShowingEventHandler FloatingInlineObjectLayoutOptionsFormShowing {
			add { Events.AddHandler(onFloatingInlineObjectLayoutOptionsFormShowing, value); }
			remove { Events.RemoveHandler(onFloatingInlineObjectLayoutOptionsFormShowing, value); }
		}
		protected internal virtual void RaiseFloatingInlineObjectLayoutOptionsFormShowing(FloatingInlineObjectLayoutOptionsFormShowingEventArgs e) {
			FloatingInlineObjectLayoutOptionsFormShowingEventHandler handler = (FloatingInlineObjectLayoutOptionsFormShowingEventHandler)Events[onFloatingInlineObjectLayoutOptionsFormShowing];
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region ParagraphFormShowing
		static readonly object onParagraphFormShowing = new object();
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlParagraphFormShowing")]
#endif
		public event ParagraphFormShowingEventHandler ParagraphFormShowing {
			add { Events.AddHandler(onParagraphFormShowing, value); }
			remove { Events.RemoveHandler(onParagraphFormShowing, value); }
		}
		protected internal virtual void RaiseParagraphFormShowing(ParagraphFormShowingEventArgs e) {
			ParagraphFormShowingEventHandler handler = (ParagraphFormShowingEventHandler)Events[onParagraphFormShowing];
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region TabsFormShowing
		static readonly object onTabsFormShowing = new object();
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlTabsFormShowing")]
#endif
		public event TabsFormShowingEventHandler TabsFormShowing {
			add { Events.AddHandler(onTabsFormShowing, value); }
			remove { Events.RemoveHandler(onTabsFormShowing, value); }
		}
		protected internal virtual void RaiseTabsFormShowing(TabsFormShowingEventArgs e) {
			TabsFormShowingEventHandler handler = (TabsFormShowingEventHandler)Events[onTabsFormShowing];
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region BookmarkFormShowing
		static readonly object onBookmarkFormShowing = new object();
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlBookmarkFormShowing")]
#endif
		public event BookmarkFormShowingEventHandler BookmarkFormShowing {
			add { Events.AddHandler(onBookmarkFormShowing, value); }
			remove { Events.RemoveHandler(onBookmarkFormShowing, value); }
		}
		protected internal virtual void RaiseBookmarkFormShowing(BookmarkFormShowingEventArgs e) {
			BookmarkFormShowingEventHandler handler = (BookmarkFormShowingEventHandler)Events[onBookmarkFormShowing];
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region EditStyleFormShowing
		static readonly object onEditStyleFormShowing = new object();
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlEditStyleFormShowing")]
#endif
		public event EditStyleFormShowingEventHandler EditStyleFormShowing {
			add { Events.AddHandler(onEditStyleFormShowing, value); }
			remove { Events.RemoveHandler(onEditStyleFormShowing, value); }
		}
		protected internal virtual void RaiseEditStyleFormShowing(EditStyleFormShowingEventArgs e) {
			EditStyleFormShowingEventHandler handler = (EditStyleFormShowingEventHandler)Events[onEditStyleFormShowing];
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region TableStyleFormShowing
		static readonly object onTableStyleFormShowing = new object();
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlTableStyleFormShowing")]
#endif
		public event TableStyleFormShowingEventHandler TableStyleFormShowing {
			add { Events.AddHandler(onTableStyleFormShowing, value); }
			remove { Events.RemoveHandler(onTableStyleFormShowing, value); }
		}
		protected internal virtual void RaiseTableStyleFormShowing(TableStyleFormShowingEventArgs e) {
			TableStyleFormShowingEventHandler handler = (TableStyleFormShowingEventHandler)Events[onTableStyleFormShowing];
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region HyperlinkFormShowing
		static readonly object onHyperlinkFormShowing = new object();
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlHyperlinkFormShowing")]
#endif
		public event HyperlinkFormShowingEventHandler HyperlinkFormShowing {
			add { Events.AddHandler(onHyperlinkFormShowing, value); }
			remove { Events.RemoveHandler(onHyperlinkFormShowing, value); }
		}
		protected internal virtual void RaiseHyperlinkFormShowing(HyperlinkFormShowingEventArgs e) {
			HyperlinkFormShowingEventHandler handler = (HyperlinkFormShowingEventHandler)Events[onHyperlinkFormShowing];
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region RangeEditingPermissionsFormShowing
		static readonly object onRangeEditingPermissionsFormShowing = new object();
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlRangeEditingPermissionsFormShowing")]
#endif
		public event RangeEditingPermissionsFormShowingEventHandler RangeEditingPermissionsFormShowing {
			add { Events.AddHandler(onRangeEditingPermissionsFormShowing, value); }
			remove { Events.RemoveHandler(onRangeEditingPermissionsFormShowing, value); }
		}
		protected internal virtual void RaiseRangeEditingPermissionsFormShowing(RangeEditingPermissionsFormShowingEventArgs e) {
			RangeEditingPermissionsFormShowingEventHandler handler = (RangeEditingPermissionsFormShowingEventHandler)Events[onRangeEditingPermissionsFormShowing];
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region DocumentProtectionQueryNewPasswordFormShowing
		static readonly object onDocumentProtectionQueryNewPasswordFormShowing = new object();
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlDocumentProtectionQueryNewPasswordFormShowing")]
#endif
		public event DocumentProtectionQueryNewPasswordFormShowingEventHandler DocumentProtectionQueryNewPasswordFormShowing {
			add { Events.AddHandler(onDocumentProtectionQueryNewPasswordFormShowing, value); }
			remove { Events.RemoveHandler(onDocumentProtectionQueryNewPasswordFormShowing, value); }
		}
		protected internal virtual void RaiseDocumentProtectionQueryNewPasswordFormShowing(DocumentProtectionQueryNewPasswordFormShowingEventArgs e) {
			DocumentProtectionQueryNewPasswordFormShowingEventHandler handler = (DocumentProtectionQueryNewPasswordFormShowingEventHandler)Events[onDocumentProtectionQueryNewPasswordFormShowing];
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region DocumentProtectionQueryPasswordFormShowing
		static readonly object onDocumentProtectionQueryPasswordFormShowing = new object();
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlDocumentProtectionQueryPasswordFormShowing")]
#endif
		public event DocumentProtectionQueryPasswordFormShowingEventHandler DocumentProtectionQueryPasswordFormShowing {
			add { Events.AddHandler(onDocumentProtectionQueryPasswordFormShowing, value); }
			remove { Events.RemoveHandler(onDocumentProtectionQueryPasswordFormShowing, value); }
		}
		protected internal virtual void RaiseDocumentProtectionQueryPasswordFormShowing(DocumentProtectionQueryPasswordFormShowingEventArgs e) {
			DocumentProtectionQueryPasswordFormShowingEventHandler handler = (DocumentProtectionQueryPasswordFormShowingEventHandler)Events[onDocumentProtectionQueryPasswordFormShowing];
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region LineNumberingFormShowing
		static readonly object onLineNumberingFormShowing = new object();
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlLineNumberingFormShowing")]
#endif
		public event LineNumberingFormShowingEventHandler LineNumberingFormShowing {
			add { Events.AddHandler(onLineNumberingFormShowing, value); }
			remove { Events.RemoveHandler(onLineNumberingFormShowing, value); }
		}
		protected internal virtual void RaiseLineNumberingFormShowing(LineNumberingFormShowingEventArgs e) {
			LineNumberingFormShowingEventHandler handler = (LineNumberingFormShowingEventHandler)Events[onLineNumberingFormShowing];
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region PageSetupFormShowing
		static readonly object onPageSetupFormShowing = new object();
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlPageSetupFormShowing")]
#endif
		public event PageSetupFormShowingEventHandler PageSetupFormShowing {
			add { Events.AddHandler(onPageSetupFormShowing, value); }
			remove { Events.RemoveHandler(onPageSetupFormShowing, value); }
		}
		protected internal virtual void RaisePageSetupFormShowing(PageSetupFormShowingEventArgs e) {
			PageSetupFormShowingEventHandler handler = (PageSetupFormShowingEventHandler)Events[onPageSetupFormShowing];
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region ColumnsSetupFormShowing
		static readonly object onColumnsSetupFormShowing = new object();
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlColumnsSetupFormShowing")]
#endif
		public event ColumnsSetupFormShowingEventHandler ColumnsSetupFormShowing {
			add { Events.AddHandler(onColumnsSetupFormShowing, value); }
			remove { Events.RemoveHandler(onColumnsSetupFormShowing, value); }
		}
		protected internal virtual void RaiseColumnsSetupFormShowing(ColumnsSetupFormShowingEventArgs e) {
			ColumnsSetupFormShowingEventHandler handler = (ColumnsSetupFormShowingEventHandler)Events[onColumnsSetupFormShowing];
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region PasteSpecialFormShowing
		static readonly object onPasteSpecialFormShowing = new object();
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlPasteSpecialFormShowing")]
#endif
		public event PasteSpecialFormShowingEventHandler PasteSpecialFormShowing {
			add { Events.AddHandler(onPasteSpecialFormShowing, value); }
			remove { Events.RemoveHandler(onPasteSpecialFormShowing, value); }
		}
		protected internal virtual void RaisePasteSpecialFormShowing(PasteSpecialFormShowingEventArgs e) {
			PasteSpecialFormShowingEventHandler handler = (PasteSpecialFormShowingEventHandler)Events[onPasteSpecialFormShowing];
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region SymbolFormShowing
		static readonly object onSymbolFormShowing = new object();
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlSymbolFormShowing")]
#endif
		public event SymbolFormShowingEventHandler SymbolFormShowing {
			add { Events.AddHandler(onSymbolFormShowing, value); }
			remove { Events.RemoveHandler(onSymbolFormShowing, value); }
		}
		protected internal virtual void RaiseSymbolFormShowing(SymbolFormShowingEventArgs e) {
			SymbolFormShowingEventHandler handler = (SymbolFormShowingEventHandler)Events[onSymbolFormShowing];
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region NumberingListFormShowing
		static readonly object onNumberingListFormShowing = new object();
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlNumberingListFormShowing")]
#endif
		public event NumberingListFormShowingEventHandler NumberingListFormShowing {
			add { Events.AddHandler(onNumberingListFormShowing, value); }
			remove { Events.RemoveHandler(onNumberingListFormShowing, value); }
		}
		protected internal virtual void RaiseNumberingListFormShowing(NumberingListFormShowingEventArgs e) {
			NumberingListFormShowingEventHandler handler = (NumberingListFormShowingEventHandler)Events[onNumberingListFormShowing];
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region SearchFormShowing
		static readonly object onSearchFormShowing = new object();
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlSearchFormShowing")]
#endif
		public event SearchFormShowingEventHandler SearchFormShowing {
			add { Events.AddHandler(onSearchFormShowing, value); }
			remove { Events.RemoveHandler(onSearchFormShowing, value); }
		}
		protected internal virtual void RaiseSearchFormShowing(SearchFormShowingEventArgs e) {
			SearchFormShowingEventHandler handler = (SearchFormShowingEventHandler)Events[onSearchFormShowing];
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region InsertMergeFieldFormShowing
		static readonly object onInsertMergeFieldFormShowing = new object();
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlInsertMergeFieldFormShowing")]
#endif
		public event InsertMergeFieldFormShowingEventHandler InsertMergeFieldFormShowing {
			add { Events.AddHandler(onInsertMergeFieldFormShowing, value); }
			remove { Events.RemoveHandler(onInsertMergeFieldFormShowing, value); }
		}
		protected internal virtual void RaiseInsertMergeFieldFormShowing(InsertMergeFieldFormShowingEventArgs e) {
			InsertMergeFieldFormShowingEventHandler handler = (InsertMergeFieldFormShowingEventHandler)Events[onInsertMergeFieldFormShowing];
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region InsertTableFormShowing
		static readonly object onInsertTableFormShowing = new object();
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlInsertTableFormShowing")]
#endif
		public event InsertTableFormShowingEventHandler InsertTableFormShowing {
			add { Events.AddHandler(onInsertTableFormShowing, value); }
			remove { Events.RemoveHandler(onInsertTableFormShowing, value); }
		}
		protected internal virtual void RaiseInsertTableFormShowing(InsertTableFormShowingEventArgs e) {
			InsertTableFormShowingEventHandler handler = (InsertTableFormShowingEventHandler)Events[onInsertTableFormShowing];
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region InsertTableCellsFormShowing
		static readonly object onInsertTableCellsFormShowing = new object();
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlInsertTableCellsFormShowing")]
#endif
		public event InsertTableCellsFormShowingEventHandler InsertTableCellsFormShowing {
			add { Events.AddHandler(onInsertTableCellsFormShowing, value); }
			remove { Events.RemoveHandler(onInsertTableCellsFormShowing, value); }
		}
		protected internal virtual void RaiseInsertTableCellsFormShowing(InsertTableCellsFormShowingEventArgs e) {
			InsertTableCellsFormShowingEventHandler handler = (InsertTableCellsFormShowingEventHandler)Events[onInsertTableCellsFormShowing];
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region DeleteTableCellsFormShowing
		static readonly object onDeleteTableCellsFormShowing = new object();
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlDeleteTableCellsFormShowing")]
#endif
		public event DeleteTableCellsFormShowingEventHandler DeleteTableCellsFormShowing {
			add { Events.AddHandler(onDeleteTableCellsFormShowing, value); }
			remove { Events.RemoveHandler(onDeleteTableCellsFormShowing, value); }
		}
		protected internal virtual void RaiseDeleteTableCellsFormShowing(DeleteTableCellsFormShowingEventArgs e) {
			DeleteTableCellsFormShowingEventHandler handler = (DeleteTableCellsFormShowingEventHandler)Events[onDeleteTableCellsFormShowing];
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region SplitTableCellsFormShowing
		static readonly object onSplitTableCellsFormShowing = new object();
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlSplitTableCellsFormShowing")]
#endif
		public event SplitTableCellsFormShowingEventHandler SplitTableCellsFormShowing {
			add { Events.AddHandler(onSplitTableCellsFormShowing, value); }
			remove { Events.RemoveHandler(onSplitTableCellsFormShowing, value); }
		}
		protected internal virtual void RaiseSplitTableCellsFormShowing(SplitTableCellsFormShowingEventArgs e) {
			SplitTableCellsFormShowingEventHandler handler = (SplitTableCellsFormShowingEventHandler)Events[onSplitTableCellsFormShowing];
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region TablePropertiesFormShowing
		static readonly object onTablePropertiesFormShowing = new object();
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlTablePropertiesFormShowing")]
#endif
		public event TablePropertiesFormShowingEventHandler TablePropertiesFormShowing {
			add { Events.AddHandler(onTablePropertiesFormShowing, value); }
			remove { Events.RemoveHandler(onTablePropertiesFormShowing, value); }
		}
		protected internal virtual void RaiseTablePropertiesFormShowing(TablePropertiesFormShowingEventArgs e) {
			TablePropertiesFormShowingEventHandler handler = (TablePropertiesFormShowingEventHandler)Events[onTablePropertiesFormShowing];
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region TableOptionsFormShowing
		static readonly object onTableOptionsFormShowing = new object();
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlTableOptionsFormShowing")]
#endif
		public event TableOptionsFormShowingEventHandler TableOptionsFormShowing {
			add { Events.AddHandler(onTableOptionsFormShowing, value); }
			remove { Events.RemoveHandler(onTableOptionsFormShowing, value); }
		}
		protected internal virtual void RaiseTableOptionsFormShowing(TableOptionsFormShowingEventArgs e) {
			TableOptionsFormShowingEventHandler handler = (TableOptionsFormShowingEventHandler)Events[onTableOptionsFormShowing];
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region PreparePopupMenu
		internal static readonly object onPreparePopupMenu = new object();
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete("You should use the 'PopupMenuShowing' instead", false)]
		public event PreparePopupMenuEventHandler PreparePopupMenu {
			add { Events.AddHandler(onPreparePopupMenu, value); }
			remove { Events.RemoveHandler(onPreparePopupMenu, value); }
		}
		[Obsolete("You should use the 'RaisePopupMenuShowing' instead")]
		protected internal virtual RichEditPopupMenu RaisePreparePopupMenu(RichEditPopupMenu menu) {
			PreparePopupMenuEventHandler handler = (PreparePopupMenuEventHandler)this.Events[onPreparePopupMenu];
			if (handler != null) {
				PreparePopupMenuEventArgs args = new PreparePopupMenuEventArgs(menu);
				handler(this, args);
				return args.Menu;
			}
			else
				return menu;
		}
		#endregion
		#region PopupMenuShowing
		internal static readonly object onPopupMenuShowing = new object();
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlPopupMenuShowing")]
#endif
		public event PopupMenuShowingEventHandler PopupMenuShowing {
			add { Events.AddHandler(onPopupMenuShowing, value); }
			remove { Events.RemoveHandler(onPopupMenuShowing, value); }
		}
		protected internal virtual RichEditPopupMenu RaisePopupMenuShowing(RichEditPopupMenu menu) {
			PopupMenuShowingEventHandler handler = (PopupMenuShowingEventHandler)this.Events[onPopupMenuShowing];
			if (handler != null) {
				PopupMenuShowingEventArgs args = new PopupMenuShowingEventArgs(menu);
				handler(this, args);
				return args.Menu;
			}
			else
				return menu;
		}
		#endregion
		#region CustomDrawActiveView
		RichEditViewCustomDrawEventHandler onCustomDrawActiveView;
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlCustomDrawActiveView")]
#endif
		public event RichEditViewCustomDrawEventHandler CustomDrawActiveView { add { onCustomDrawActiveView += value; } remove { onCustomDrawActiveView -= value; } }
		protected internal virtual void RaiseCustomDrawActiveView(GraphicsCache cache) {
			if (onCustomDrawActiveView != null)
				onCustomDrawActiveView(this, new RichEditViewCustomDrawEventArgs(cache));
		}
		#endregion
		#region CustomMarkDraw
		RichEditCustomMarkDrawEventHandler onCustomMarkDraw;
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlCustomMarkDraw")]
#endif
		public event RichEditCustomMarkDrawEventHandler CustomMarkDraw { add { onCustomMarkDraw += value; } remove { onCustomMarkDraw -= value; } }
		protected internal virtual void RaiseCustomMarkDraw(Graphics graphics, ICustomMarkExporter customMarkExporter) {
			if (onCustomMarkDraw != null) {
				bool oldCaretHiddenState = Caret.IsHidden;
				GraphicsCache cache = null;
				if (!oldCaretHiddenState) {
					Caret.IsHidden = true;
					cache = new GraphicsCache(graphics);
					Painter.DrawCaretCore(cache, customMarkExporter);
				}
				onCustomMarkDraw(this, new RichEditCustomMarkDrawEventArgs(graphics, customMarkExporter.CustomMarkVisualInfoCollection));
				if (!oldCaretHiddenState) {
					Caret.IsHidden = false;
					Painter.DrawCaretCore(cache, customMarkExporter);
					cache.Dispose();
				}
			}
		}
		#endregion
		#endregion
		CommentPadding IInnerRichEditControlOwner.CommentPadding { get { return commentPadding; } }
		RichEditViewRepository IInnerRichEditControlOwner.CreateViewRepository() {
			return this.CreateViewRepository();
		}
		protected internal virtual RichEditViewRepository CreateViewRepository() {
			return new WinFormsRichEditViewRepository(this);
		}
		RichEditControlOptionsBase IInnerRichEditDocumentServerOwner.CreateOptions(InnerRichEditDocumentServer documentServer) {
			return new RichEditControlOptions(documentServer);
		}
		RichEditMouseHandler IInnerRichEditControlOwner.CreateMouseHandler() {
			return new RichEditMouseHandler(this);
		}
		protected internal virtual void EndInitialize() {
			this.ReplaceDataController();
			this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlConstants.DoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserMouse, true);
			this.appearance = new RichEditAppearance();
			SubscribeAppearanceEvents();
			this.lookAndFeel = new DevExpress.LookAndFeel.Helpers.ControlUserLookAndFeel(this);
			SubscribeLookAndFeelEvents();
			skinTextColors = GetSkinTextColors();
			commentPadding = GetCommentsSkinPadding();
			this.painter = CreatePainter();
			EndInitializeCommon();
			this.caret = new Caret();
		}
		protected internal virtual void ReplaceDataController() {
			RichEditDataControllerAdapter adapter = (RichEditDataControllerAdapter)DocumentModel.MailMergeDataController;
			bool isCurrencyController = adapter.DataController is CurrencyDataController;
			if (isCurrencyController)
				return;
			adapter.ReplaceDataController(new CurrencyDataController());
		}
		protected virtual void RegisterToolTipClient() {
			RegisterToolTipClientControl(ToolTipController.DefaultController);
		}
		IRulerControl IInnerRichEditControlOwner.CreateVerticalRuler() {
			return this.CreateVerticalRuler();
		}
		IRulerControl IInnerRichEditControlOwner.CreateHorizontalRuler() {
			return this.CreateHorizontalRuler();
		}
		protected internal virtual IRulerControl CreateVerticalRuler() {
			this.verticalRuler = new VerticalRulerControl(this);
			return CreateRulerCore(verticalRuler);
		}
		protected internal virtual IRulerControl CreateHorizontalRuler() {
			this.horizontalRuler = new HorizontalRulerControl(this);
			return CreateRulerCore(horizontalRuler);
		}
		protected internal virtual IRulerControl CreateRulerCore(RulerControlBase ruler) {
			this.Controls.Add(ruler);
			return ruler as IRulerControl;
		}
		bool IRichEditControl.UseStandardDragDropMode { get { return DragDropMode == DragDropMode.Standard; } }
		RichEditMouseHandlerStrategyFactory IRichEditControl.CreateRichEditMouseHandlerStrategyFactory() {
			return new WinFormsRichEditMouseHandlerStrategyFactory();
		}
		IOfficeScrollbar IInnerRichEditControlOwner.CreateVerticalScrollBar() {
			return this.CreateVerticalScrollBar();
		}
		IOfficeScrollbar IInnerRichEditControlOwner.CreateHorizontalScrollBar() {
			return this.CreateHorizontalScrollBar();
		}
		protected internal virtual IOfficeScrollbar CreateVerticalScrollBar() {
			this.verticalScrollbar = new DevExpress.XtraEditors.VScrollBar();
			return CreateScrollBarCore(verticalScrollbar);
		}
		protected internal virtual IOfficeScrollbar CreateHorizontalScrollBar() {
			this.horizontalScrollbar = new DevExpress.XtraEditors.HScrollBar();
			return CreateScrollBarCore(horizontalScrollbar);
		}
		protected internal virtual IOfficeScrollbar CreateScrollBarCore(ScrollBarBase scrollBar) {
			scrollBar.Cursor = Cursors.Default;
			ScrollBarBase.ApplyUIMode(scrollBar);
			this.Controls.Add(scrollBar);
			scrollBar.LookAndFeel.ParentLookAndFeel = this.lookAndFeel;
			return new WinFormsOfficeScrollbar(scrollBar);
		}
		protected internal virtual void SubscribeToolTipControllerEvents(ToolTipController controller) {
			controller.Disposed += new EventHandler(OnToolTipControllerDisposed);
		}
		protected internal virtual void UnsubscribeToolTipControllerEvents(ToolTipController controller) {
			controller.Disposed -= new EventHandler(OnToolTipControllerDisposed);
		}
		protected internal virtual void OnToolTipControllerDisposed(object sender, EventArgs e) {
			ToolTipController = null;
		}
		protected internal virtual void RegisterToolTipClientControl(ToolTipController controller) {
			controller.AddClientControl(this);
		}
		protected internal virtual void UnregisterToolTipClientControl(ToolTipController controller) {
			controller.RemoveClientControl(this);
		}
		protected virtual TextColors GetSkinTextColors() {
			if (LookAndFeel.Style == LookAndFeelStyle.Skin) {
				Color window = LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.Window);
				Color windowText = LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.WindowText);
				return TextColors.FromSkinColors(window, windowText);
			}
			else
				return TextColors.Defaults;
		}
		protected virtual CommentPadding GetCommentsSkinPadding() {
			if (LookAndFeel.Style == LookAndFeelStyle.Skin) {
				SkinElement commentElement = SkinPaintHelper.GetRichEditSkinElement(LookAndFeel, DevExpress.Skins.RichEditSkins.SkinCommentBorder);
				SkinElement commentsAreaElement = SkinPaintHelper.GetRichEditSkinElement(LookAndFeel, DevExpress.Skins.RichEditSkins.SkinCommentsArea);
				SkinElement commentMoreButton = SkinPaintHelper.GetRichEditSkinElement(LookAndFeel, DevExpress.Skins.RichEditSkins.SkinCommentMoreButton);
				if (commentElement == null || commentsAreaElement == null || commentMoreButton == null || commentElement.Image == null)
					return CommentPadding.GetDefaultCommentPadding(DocumentModel);
				DevExpress.Skins.SkinPaddingEdges edges = commentElement.ContentMargins;
				DevExpress.Office.Layout.DocumentLayoutUnitConverter converter = DocumentModel.LayoutUnitConverter;
				SkinPaddingEdges edge = commentsAreaElement.ContentMargins;
				int commentsLeft = edge.Left;
				int commentsRight = edge.Right;
				int verticalCommentsDistance = SkinPaintHelper.GetRichEditSkinIntProperty(LookAndFeel, SkinPaintHelper.MinVerticalCommentsDistanceProperty);
				Size moreButtonSize = commentMoreButton.Image != null ? commentMoreButton.Image.GetImageBounds(0).Size : new Size(0, 0);
				return new CommentPadding(
					converter.PixelsToLayoutUnits(commentsLeft, DpiX),
					converter.PixelsToLayoutUnits(commentsRight, DpiX),
					converter.PixelsToLayoutUnits(edges.Left, DpiX),
					converter.PixelsToLayoutUnits(edges.Top, DpiY),
					converter.PixelsToLayoutUnits(edges.Right, DpiX),
					converter.PixelsToLayoutUnits(edges.Bottom, DpiY),
					converter.PixelsToLayoutUnits(verticalCommentsDistance, DpiY),
					new Size(converter.PixelsToLayoutUnits(moreButtonSize.Width, DpiX), converter.PixelsToLayoutUnits(moreButtonSize.Height, DpiY)),
					commentMoreButton.Offset.Offset.X, commentMoreButton.Offset.Offset.Y,
					GetMoreButtonAlignment(commentMoreButton.Offset.Kind)
					);
			}
			else
				return CommentPadding.GetDefaultCommentPadding(DocumentModel);
		}
		protected virtual MoreButtonHorizontalAlignment GetMoreButtonAlignment(SkinOffsetKind skinOffsetKind) {
			switch (skinOffsetKind) {
				case SkinOffsetKind.Center:
					return MoreButtonHorizontalAlignment.Center;
				case SkinOffsetKind.Near:
					return MoreButtonHorizontalAlignment.Left;
				case SkinOffsetKind.Far:
					return MoreButtonHorizontalAlignment.Right;
				default:
					return MoreButtonHorizontalAlignment.Right;
			}
		}
		protected internal virtual RichEditControlPainter CreatePainter() {
			return new RichEditControlPainter(this);
		}
		MeasurementAndDrawingStrategy IInnerRichEditDocumentServerOwner.CreateMeasurementAndDrawingStrategy(DocumentModel documentModel) {
			return this.CreateMeasurementAndDrawingStrategy(documentModel);
		}
		protected internal virtual MeasurementAndDrawingStrategy CreateMeasurementAndDrawingStrategy(DocumentModel documentModel) {
			if (useGdiPlus)
				return new GdiPlusMeasurementAndDrawingStrategy(documentModel);
			else
				return new GdiMeasurementAndDrawingStrategy(documentModel);
		}
		protected internal virtual void OnUseGdiPlusChanged() {
			if (InnerControl != null)
				InnerControl.RecreateMeasurementAndDrawingStrategy();
		}
		protected internal virtual void InitializeCaretTimer() {
			int caretBlinkTime = Win32.GetCaretBlinkTime();
			this.caretTimer = new Timer();
			this.caretTimer.Interval = Math.Max(1, caretBlinkTime);
			this.caretTimer.Tick += new EventHandler(leakSafeEventRouter.OnCaretTimerTick);
			this.caretTimer.Enabled = (caretBlinkTime > 0);
		}
		protected internal virtual void DestroyCaretTimer() {
			if (caretTimer != null) {
				this.caretTimer.Enabled = false;
				this.caretTimer.Tick -= new EventHandler(leakSafeEventRouter.OnCaretTimerTick);
				this.caretTimer.Dispose();
				this.caretTimer = null;
			}
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Mobility", "CA1601")]
		protected internal virtual void InitializeFlushPendingTextInputTimer() {
			this.flushPendingTextInputTimer = new Timer();
			this.flushPendingTextInputTimer.Interval = DevExpress.XtraRichEdit.Keyboard.NormalKeyboardHandler.PendingTextFlushMilliseconds;
			this.flushPendingTextInputTimer.Tick += leakSafeEventRouter.OnFlushPendingTextInputTimerTick;
			this.flushPendingTextInputTimer.Enabled = false;
		}
		protected internal virtual void DestroyFlushPendingTextInputTimer() {
			if (flushPendingTextInputTimer != null) {
				this.flushPendingTextInputTimer.Enabled = false;
				this.flushPendingTextInputTimer.Tick -= leakSafeEventRouter.OnFlushPendingTextInputTimerTick;
				this.flushPendingTextInputTimer.Dispose();
				this.flushPendingTextInputTimer = null;
			}
		}
		void IRichEditControl.ForceFlushPendingTextInput() {
			this.ForceFlushPendingTextInput();
		}
		protected internal virtual void OnFlushPendingTextInputTimerTick(object sender, EventArgs e) {
			if (IsUpdateLocked)
				return;
			ForceFlushPendingTextInput();
		}
		protected internal virtual void ForceFlushPendingTextInput() {
			if (InnerControl != null)
				InnerControl.FlushPendingTextInput();
		}
		protected internal virtual void InitializeVerticalScrollBarUpdateTimer() {
			this.verticalScrollBarUpdateTimer = new Timer();
			this.verticalScrollBarUpdateTimer.Interval = 3000;
			this.verticalScrollBarUpdateTimer.Tick += leakSafeEventRouter.OnVerticalScrollBarUpdateTimerTick;
			this.verticalScrollBarUpdateTimer.Enabled = true;
		}
		protected internal virtual void DestroyVerticalScrollBarUpdateTimer() {
			if (verticalScrollBarUpdateTimer != null) {
				this.verticalScrollBarUpdateTimer.Enabled = false;
				this.verticalScrollBarUpdateTimer.Tick -= leakSafeEventRouter.OnVerticalScrollBarUpdateTimerTick;
				this.verticalScrollBarUpdateTimer.Dispose();
				this.verticalScrollBarUpdateTimer = null;
			}
		}
		protected internal virtual void OnVerticalScrollBarUpdateTimerTick(object sender, EventArgs e) {
			if (IsUpdateLocked)
				return;
			UpdateVerticalScrollBar(true);
		}
		protected internal virtual void InitializeFlushRotationControllerTimer() {
			this.flushRotationControllerTimer = new Timer();
			this.flushRotationControllerTimer.Interval = 1000;
			this.flushRotationControllerTimer.Tick += leakSafeEventRouter.OnFlushRotationControllerTimerTick;
			this.flushRotationControllerTimer.Enabled = true;
		}
		protected internal virtual void DestroyFlushRotationControllerTimer() {
			if (flushRotationControllerTimer != null) {
				this.flushRotationControllerTimer.Enabled = false;
				this.flushRotationControllerTimer.Tick -= leakSafeEventRouter.OnFlushRotationControllerTimerTick;
				this.flushRotationControllerTimer.Dispose();
				this.flushRotationControllerTimer = null;
			}
		}
		protected internal virtual void OnFlushRotationControllerTimerTick(object sender, EventArgs e) {
			if (IsUpdateLocked)
				return;
			if (rotationController == null)
				return;
			rotationController.OnTimerTick();
		}
		protected internal virtual void OnCaretTimerTick(object sender, EventArgs e) {
			if (IsUpdateLocked)
				return;
			caret.IsHidden = !caret.IsHidden;
			Painter.DrawCaret();
		}
		IPlatformSpecificScrollBarAdapter IRichEditControl.CreatePlatformSpecificScrollBarAdapter() {
			return new WinFormsScrollBarAdapter();
		}
		RichEditViewVerticalScrollController IRichEditControl.CreateRichEditViewVerticalScrollController(RichEditView view) {
			return new WinFormsRichEditViewVerticalScrollController(view);
		}
		RichEditViewHorizontalScrollController IRichEditControl.CreateRichEditViewHorizontalScrollController(RichEditView view) {
			return new WinFormsRichEditViewHorizontalScrollController(view);
		}
		void IRichEditControl.ShowFontForm(MergedCharacterProperties characterProperties, ShowFontFormCallback callback, object callbackData) {
			this.ShowFontForm(characterProperties, callback, callbackData);
		}
		internal virtual void ShowFontForm(MergedCharacterProperties characterProperties, ShowFontFormCallback callback, object callbackData) {
			FontFormControllerParameters controllerParameters = new FontFormControllerParameters(this, characterProperties);
			FontFormShowingEventArgs args = new FontFormShowingEventArgs(controllerParameters);
			RaiseFontFormShowing(args);
			if (!args.Handled) {
				using (FontForm form = new FontForm(controllerParameters)) {
					form.LookAndFeel.ParentLookAndFeel = LookAndFeel;
					DialogResult result = FormTouchUIAdapter.ShowDialog(form, this);
					if (result == DialogResult.OK && callback != null)
						callback(form.Controller.SourceCharacterProperties, callbackData);
				}
			}
			else {
				DialogResult result = args.DialogResult;
				if (result == DialogResult.OK && callback != null)
					callback(controllerParameters.SourceCharacterProperties, callbackData);
			}
		}
		#region floatingObjectLayoutOptions
		void IRichEditControl.ShowFloatingInlineObjectLayoutOptionsForm(FloatingInlineObjectParameters parameters, ShowFloatingInlineObjectLayoutOptionsFormCallback callback, object callbackData) {
			this.ShowFloatingInlineObjectLayoutOptionsForm(parameters, callback, callbackData);
		}
		internal virtual void ShowFloatingInlineObjectLayoutOptionsForm(FloatingInlineObjectParameters parameters, ShowFloatingInlineObjectLayoutOptionsFormCallback callback, object callbackData) {
			FloatingInlineObjectLayoutOptionsFormControllerParameters controllerParameters = new FloatingInlineObjectLayoutOptionsFormControllerParameters(this, parameters);
			FloatingInlineObjectLayoutOptionsFormShowingEventArgs args = new FloatingInlineObjectLayoutOptionsFormShowingEventArgs(controllerParameters);
			RaiseFloatingInlineObjectLayoutOptionsFormShowing(args);
			DialogResult result;
			if (args.Handled)
				result = args.DialogResult;
			else
				using (FloatingObjectLayoutForm form = new FloatingObjectLayoutForm(controllerParameters)) {
					form.LookAndFeel.ParentLookAndFeel = LookAndFeel;
					result = FormTouchUIAdapter.ShowDialog(form, this);
				}
			if ((result == DialogResult.OK) && (callback != null))
				callback(controllerParameters.FloatingInlineObjectParameters, callbackData);
		}
		#endregion floatingObjectLayoutOptions
		void IRichEditControl.ShowEditStyleForm(ParagraphStyle paragraphSourceStyle, ParagraphIndex index, ShowEditStyleFormCallback callback) {
			this.ShowEditStyleForm(paragraphSourceStyle, null, index, callback);
		}
		void IRichEditControl.ShowEditStyleForm(CharacterStyle characterSourceStyle, ParagraphIndex index, ShowEditStyleFormCallback callback) {
			this.ShowEditStyleForm(null, characterSourceStyle, index, callback);
		}
		internal virtual void ShowEditStyleForm(ParagraphStyle paragraphSourceStyle, CharacterStyle characterSourceStyle, ParagraphIndex index, ShowEditStyleFormCallback callback) {
			EditStyleFormControllerParameters controllerParameters;
			if (characterSourceStyle == null)
				controllerParameters = new EditStyleFormControllerParameters(this, paragraphSourceStyle, index);
			else
				controllerParameters = new EditStyleFormControllerParameters(this, characterSourceStyle, index);
			EditStyleFormShowingEventArgs args = new EditStyleFormShowingEventArgs(controllerParameters);
			RaiseEditStyleFormShowing(args);
			if (!args.Handled) {
				using (EditStyleForm form = new EditStyleForm(controllerParameters)) {
					FormTouchUIAdapter.ShowDialog(form, this);
				}
			}
		}
		void IRichEditControl.ShowTableStyleForm(TableStyle style) {
			this.ShowTableStyleForm(style);
		}
		internal virtual void ShowTableStyleForm(TableStyle style) {
			TableStyleFormControllerParameters controllerParameters;
			controllerParameters = new TableStyleFormControllerParameters(this, style);
			TableStyleFormShowingEventArgs args = new TableStyleFormShowingEventArgs(controllerParameters);
			RaiseTableStyleFormShowing(args);
			if (!args.Handled) {
				using (TableStyleForm form = new TableStyleForm(controllerParameters)) {
					DialogResult result = FormTouchUIAdapter.ShowDialog(form, this);
					if (result == DialogResult.OK)
						ApplyShowTableStyleFormResults(style);
				}
			}
			else if (args.DialogResult == DialogResult.OK)
				ApplyShowTableStyleFormResults(style);
		}
		void ApplyShowTableStyleFormResults(TableStyle style) {
			TableStyle modelStyle = DocumentModel.TableStyles.GetStyleByName(style.StyleName);
			if (modelStyle == null)
				DocumentModel.TableStyles.AddNewStyle(style);
			else
				modelStyle.CopyProperties(style);
		}
		void IRichEditControl.ShowParagraphForm(MergedParagraphProperties paragraphProperties, ShowParagraphFormCallback callback, object callbackData) {
			this.ShowParagraphForm(paragraphProperties, callback, callbackData);
		}
		internal virtual void ShowParagraphForm(MergedParagraphProperties paragraphProperties, ShowParagraphFormCallback callback, object callbackData) {
			ParagraphFormControllerParameters controllerParameters = new ParagraphFormControllerParameters(this, paragraphProperties, DocumentModel.UnitConverter);
			ParagraphFormShowingEventArgs args = new ParagraphFormShowingEventArgs(controllerParameters);
			RaiseParagraphFormShowing(args);
			if (!args.Handled) {
				using (ParagraphForm form = new ParagraphForm(controllerParameters)) {
					form.LookAndFeel.ParentLookAndFeel = LookAndFeel;
					DialogResult result = FormTouchUIAdapter.ShowDialog(form, this);
					if (result == DialogResult.OK)
						callback(form.Controller.SourceProperties, callbackData);
				}
			}
			else {
				DialogResult result = args.DialogResult;
				if (result == DialogResult.OK)
					callback(controllerParameters.ParagraphProperties, callbackData);
			}
		}
		void IRichEditControl.ShowTabsForm(TabFormattingInfo tabInfo, int defaultTabWidth, ShowTabsFormCallback callback, object callbackData) {
			this.ShowTabsForm(tabInfo, defaultTabWidth, callback, callbackData);
		}
		void IRichEditControl.ShowTOCForm(Field field) {
			using (TableOfContentsForm form = new TableOfContentsForm()) {
				form.LookAndFeel.ParentLookAndFeel = LookAndFeel;
				TableOfContentsFormController controller = new TableOfContentsFormController(new TOCFormControllerParameters(this, field));
				form.Initialize(controller);
				form.ShowDialog();
			}
		}
		void IRichEditControl.ShowLanguageForm(DocumentModel documentModel) {
			using (LanguageForm form = new LanguageForm()) {
				form.LookAndFeel.ParentLookAndFeel = LookAndFeel;
				LanguageFormController controller = new LanguageFormController(this, documentModel);
				form.Initialize(controller);
				form.ShowDialog();
			}
		}
		internal virtual void ShowTabsForm(TabFormattingInfo tabInfo, int defaultTabWidth, ShowTabsFormCallback callback, object callbackData) {
			TabsFormCommandUIState commandState = callbackData as TabsFormCommandUIState;
			IFormOwner tabFormOwner = (commandState != null) ? commandState.TabsFormOwner : null;
			TabsFormControllerParameters controllerParameters = new TabsFormControllerParameters(this, tabInfo, defaultTabWidth, DocumentModel.UnitConverter, tabFormOwner);
			TabsFormShowingEventArgs args = new TabsFormShowingEventArgs(controllerParameters);
			RaiseTabsFormShowing(args);
			if (!args.Handled) {
				using (TabsForm form = new TabsForm(controllerParameters)) {
					form.LookAndFeel.ParentLookAndFeel = LookAndFeel;
					DialogResult result = FormTouchUIAdapter.ShowDialog(form, this);
					if (result == DialogResult.OK)
						callback(form.Controller.SourceTabInfo, form.Controller.SourceDefaultTabWidth, callbackData);
				}
			}
			else {
				DialogResult result = args.DialogResult;
				if (result == DialogResult.OK)
					callback(controllerParameters.TabInfo, controllerParameters.DefaultTabWidth, callbackData);
			}
		}
		void IRichEditControl.ShowBookmarkForm() {
			this.ShowBookmarkForm();
		}
		internal virtual void ShowBookmarkForm() {
			BookmarkFormControllerParameters controllerParameters = new BookmarkFormControllerParameters(this);
			BookmarkFormShowingEventArgs args = new BookmarkFormShowingEventArgs(controllerParameters);
			RaiseBookmarkFormShowing(args);
			if (!args.Handled) {
				using (BookmarkForm form = new BookmarkForm(controllerParameters)) {
					form.LookAndFeel.ParentLookAndFeel = LookAndFeel;
					FormTouchUIAdapter.ShowDialog(form, this);
				}
			}
		}
		void IRichEditControl.ShowHyperlinkForm(HyperlinkInfo hyperlinkInfo, RunInfo runInfo, string title, ShowHyperlinkFormCallback callback) {
			this.ShowHyperlinkForm(hyperlinkInfo, runInfo, title, callback);
		}
		internal virtual void ShowHyperlinkForm(HyperlinkInfo hyperlinkInfo, RunInfo runInfo, string title, ShowHyperlinkFormCallback callback) {
			HyperlinkFormControllerParameters controllerParameters = new HyperlinkFormControllerParameters(this, hyperlinkInfo, runInfo);
			HyperlinkFormShowingEventArgs args = new HyperlinkFormShowingEventArgs(controllerParameters);
			RaiseHyperlinkFormShowing(args);
			if (!args.Handled) {
				using (HyperlinkForm form = new HyperlinkForm(controllerParameters)) {
					form.LookAndFeel.ParentLookAndFeel = LookAndFeel;
					form.Text = title;
					DialogResult result = FormTouchUIAdapter.ShowDialog(form, this);
					if (result == DialogResult.OK)
						callback(form.Controller.HyperlinkInfo, form.Controller.TextSource, runInfo, form.Controller.TextToDisplay);
				}
			}
			else {
				DialogResult result = args.DialogResult;
				if (result == DialogResult.OK)
					callback(controllerParameters.HyperlinkInfo, controllerParameters.TextSource, runInfo, controllerParameters.TextToDisplay);
			}
		}
		void IRichEditControl.ShowRangeEditingPermissionsForm() {
			this.ShowRangeEditingPermissionsForm();
		}
		protected internal virtual void ShowRangeEditingPermissionsForm() {
			RangeEditingPermissionsFormControllerParameters controllerParameters = new RangeEditingPermissionsFormControllerParameters(this);
			RangeEditingPermissionsFormShowingEventArgs args = new RangeEditingPermissionsFormShowingEventArgs(controllerParameters);
			RaiseRangeEditingPermissionsFormShowing(args);
			if (!args.Handled) {
				using (RangeEditingPermissionsForm form = new RangeEditingPermissionsForm(controllerParameters)) {
					form.LookAndFeel.ParentLookAndFeel = LookAndFeel;
					FormTouchUIAdapter.ShowDialog(form, this);
				}
			}
		}
		void IRichEditControl.ShowDocumentProtectionQueryNewPasswordForm(PasswordInfo passwordInfo, PasswordFormCallback callback) {
			this.ShowDocumentProtectionQueryNewPasswordForm(passwordInfo, callback);
		}
		protected internal virtual void ShowDocumentProtectionQueryNewPasswordForm(PasswordInfo passwordInfo, PasswordFormCallback callback) {
			DocumentProtectionQueryNewPasswordFormControllerParameters controllerParameters = new DocumentProtectionQueryNewPasswordFormControllerParameters(this, passwordInfo);
			DocumentProtectionQueryNewPasswordFormShowingEventArgs args = new DocumentProtectionQueryNewPasswordFormShowingEventArgs(controllerParameters);
			RaiseDocumentProtectionQueryNewPasswordFormShowing(args);
			if (!args.Handled) {
				using (DocumentProtectionQueryNewPasswordForm form = new DocumentProtectionQueryNewPasswordForm(controllerParameters)) {
					form.LookAndFeel.ParentLookAndFeel = LookAndFeel;
					DialogResult result = FormTouchUIAdapter.ShowDialog(form, this);
					if (result == DialogResult.OK)
						callback(passwordInfo);
				}
			}
			else {
				DialogResult result = args.DialogResult;
				if (result == DialogResult.OK)
					callback(controllerParameters.PasswordInfo);
			}
		}
		void IRichEditControl.ShowDocumentProtectionQueryPasswordForm(PasswordInfo passwordInfo, PasswordFormCallback callback) {
			this.ShowDocumentProtectionQueryPasswordForm(passwordInfo, callback);
		}
		protected internal virtual void ShowDocumentProtectionQueryPasswordForm(PasswordInfo passwordInfo, PasswordFormCallback callback) {
			DocumentProtectionQueryPasswordFormControllerParameters controllerParameters = new DocumentProtectionQueryPasswordFormControllerParameters(this, passwordInfo);
			DocumentProtectionQueryPasswordFormShowingEventArgs args = new DocumentProtectionQueryPasswordFormShowingEventArgs(controllerParameters);
			RaiseDocumentProtectionQueryPasswordFormShowing(args);
			if (!args.Handled) {
				using (DocumentProtectionQueryPasswordForm form = new DocumentProtectionQueryPasswordForm(controllerParameters)) {
					form.LookAndFeel.ParentLookAndFeel = LookAndFeel;
					DialogResult result = FormTouchUIAdapter.ShowDialog(form, this);
					if (result == DialogResult.OK)
						callback(passwordInfo);
				}
			}
			else {
				DialogResult result = args.DialogResult;
				if (result == DialogResult.OK)
					callback(controllerParameters.PasswordInfo);
			}
		}
		void IRichEditControl.ShowLineNumberingForm(LineNumberingInfo properties, ShowLineNumberingFormCallback callback, object callbackData) {
			this.ShowLineNumberingForm(properties, callback, callbackData);
		}
		internal virtual void ShowLineNumberingForm(LineNumberingInfo properties, ShowLineNumberingFormCallback callback, object callbackData) {
			LineNumberingFormControllerParameters controllerParameters = new LineNumberingFormControllerParameters(this, properties);
			LineNumberingFormShowingEventArgs args = new LineNumberingFormShowingEventArgs(controllerParameters);
			RaiseLineNumberingFormShowing(args);
			if (!args.Handled) {
				using (LineNumberingForm form = new LineNumberingForm(controllerParameters)) {
					form.LookAndFeel.ParentLookAndFeel = LookAndFeel;
					DialogResult result = FormTouchUIAdapter.ShowDialog(form, this);
					if (result == DialogResult.OK)
						callback(form.Controller.SourceLineNumberingInfo, callbackData);
				}
			}
			else {
				DialogResult result = args.DialogResult;
				if (result == DialogResult.OK)
					callback(controllerParameters.LineNumberingInfo, callbackData);
			}
		}
		void IRichEditControl.ShowPageSetupForm(PageSetupInfo properties, ShowPageSetupFormCallback callback, object callbackData, PageSetupFormInitialTabPage initialTabPage) {
			this.ShowPageSetupForm(properties, callback, callbackData, initialTabPage);
		}
		internal virtual void ShowPageSetupForm(PageSetupInfo properties, ShowPageSetupFormCallback callback, object callbackData, PageSetupFormInitialTabPage initialTabPage) {
			PageSetupFormControllerParameters controllerParameters = new PageSetupFormControllerParameters(this, properties);
			controllerParameters.InitialTabPage = initialTabPage;
			PageSetupFormShowingEventArgs args = new PageSetupFormShowingEventArgs(controllerParameters);
			RaisePageSetupFormShowing(args);
			if (!args.Handled) {
				using (PageSetupForm form = new PageSetupForm(controllerParameters)) {
					form.LookAndFeel.ParentLookAndFeel = LookAndFeel;
					DialogResult result = FormTouchUIAdapter.ShowDialog(form, this);
					if (result == DialogResult.OK)
						callback(form.Controller.SourcePageSetupInfo, callbackData);
				}
			}
			else {
				DialogResult result = args.DialogResult;
				if (result == DialogResult.OK)
					callback(controllerParameters.PageSetupInfo, callbackData);
			}
		}
		void IRichEditControl.ShowColumnsSetupForm(ColumnsInfoUI properties, ShowColumnsSetupFormCallback callback, object callbackData) {
			this.ShowColumnsSetupForm(properties, callback, callbackData);
		}
		internal virtual void ShowColumnsSetupForm(ColumnsInfoUI properties, ShowColumnsSetupFormCallback callback, object callbackData) {
			ColumnsSetupFormControllerParameters controllerParameters = new ColumnsSetupFormControllerParameters(this, properties);
			ColumnsSetupFormShowingEventArgs args = new ColumnsSetupFormShowingEventArgs(controllerParameters);
			RaiseColumnsSetupFormShowing(args);
			if (!args.Handled) {
				using (ColumnsSetupForm form = new ColumnsSetupForm(controllerParameters)) {
					form.LookAndFeel.ParentLookAndFeel = LookAndFeel;
					DialogResult result = FormTouchUIAdapter.ShowDialog(form, this);
					if (result == DialogResult.OK)
						callback(form.Controller.SourceColumnsInfo, callbackData);
				}
			}
			else {
				DialogResult result = args.DialogResult;
				if (result == DialogResult.OK)
					callback(controllerParameters.ColumnsInfo, callbackData);
			}
		}
		void IRichEditControl.ShowPasteSpecialForm(PasteSpecialInfo properties, ShowPasteSpecialFormCallback callback, object callbackData) {
			this.ShowPasteSpecialForm(properties, callback, callbackData);
		}
		internal virtual void ShowPasteSpecialForm(PasteSpecialInfo properties, ShowPasteSpecialFormCallback callback, object callbackData) {
			PasteSpecialFormControllerParameters controllerParameters = new PasteSpecialFormControllerParameters(this, properties);
			PasteSpecialFormShowingEventArgs args = new PasteSpecialFormShowingEventArgs(controllerParameters);
			RaisePasteSpecialFormShowing(args);
			if (!args.Handled) {
				using (PasteSpecialForm form = new PasteSpecialForm(controllerParameters)) {
					form.LookAndFeel.ParentLookAndFeel = LookAndFeel;
					DialogResult result = FormTouchUIAdapter.ShowDialog(form, this);
					if (result == DialogResult.OK)
						callback(form.Controller.SourcePasteSpecialInfo, callbackData);
				}
			}
			else {
				DialogResult result = args.DialogResult;
				if (result == DialogResult.OK)
					callback(controllerParameters.PasteSpecialInfo, callbackData);
			}
		}
		void IRichEditControl.ShowSymbolForm(InsertSymbolViewModel viewModel) {
			this.ShowSymbolForm(viewModel);
		}
		internal virtual void ShowSymbolForm(InsertSymbolViewModel viewModel) {
			RichEditInsertSymbolViewModel richEditViewModel = viewModel as RichEditInsertSymbolViewModel;
			SymbolFormShowingEventArgs args;
			if (richEditViewModel != null) {
				args = new SymbolFormShowingEventArgs(richEditViewModel);
				RaiseSymbolFormShowing(args);
			}
			else
				args = null;
			if (args == null || !args.Handled) {
				using (SymbolForm form = new SymbolForm(viewModel)) {
					form.LookAndFeel.ParentLookAndFeel = LookAndFeel;
					FormTouchUIAdapter.ShowDialog(form, this);
				}
			}
		}
		bool IRichEditControl.CanShowNumberingListForm { get { return true; } }
		void IRichEditControl.ShowNumberingListForm(ParagraphList paragraphs, ShowNumberingListFormCallback callback, object callbackData) {
			this.ShowNumberingListForm(paragraphs, callback, callbackData);
		}
		internal virtual void ShowNumberingListForm(ParagraphList paragraphs, ShowNumberingListFormCallback callback, object callbackData) {
			NumberingListFormControllerParameters controllerParameters = new NumberingListFormControllerParameters(this, paragraphs);
			NumberingListFormShowingEventArgs args = new NumberingListFormShowingEventArgs(controllerParameters);
			RaiseNumberingListFormShowing(args);
			if (!args.Handled) {
				using (NumberingListForm form = new NumberingListForm(controllerParameters)) {
					form.LookAndFeel.ParentLookAndFeel = LookAndFeel;
					DialogResult result = FormTouchUIAdapter.ShowDialog(form, this);
					if (result == DialogResult.OK)
						callback(paragraphs, callbackData);
				}
			}
			else {
				DialogResult result = args.DialogResult;
				if (result == DialogResult.OK)
					callback(paragraphs, callbackData);
			}
		}
		public virtual void ShowSearchForm() {
			SearchFormControllerParameters controllerParameters = new SearchFormControllerParameters(this, SearchFormActivePage.Find);
			ShowSearchForm(controllerParameters);
		}
		public virtual void ShowReplaceForm() {
			SearchFormControllerParameters controllerParameters = new SearchFormControllerParameters(this, SearchFormActivePage.Replace);
			ShowSearchForm(controllerParameters);
		}
		protected internal virtual void ShowSearchForm(SearchFormControllerParameters controllerParameters) {
			SearchFormShowingEventArgs e = new SearchFormShowingEventArgs(controllerParameters);
			RaiseSearchFormShowing(e);
			if (e.Handled)
				return;
			ShowSearchFormCore(controllerParameters);
		}
		void ShowSearchFormCore(SearchFormControllerParameters controllerParameters) {
			if (searchForm == null) {
				searchForm = CreateSearchForm(controllerParameters);
				searchForm.LookAndFeel.ParentLookAndFeel = LookAndFeel;
				SubscribeSearchFormEvents();
				FormTouchUIAdapter.Show(searchForm, this);
			}
			else {
				searchForm.ActivePage = controllerParameters.ActivePage;
				searchForm.Activate();
			}
		}
		protected internal virtual void SubscribeSearchFormEvents() {
			searchForm.FormClosed += OnSearchFormClosed;
		}
		protected internal virtual void UnsubscribeSearchFormEvents() {
			searchForm.FormClosed -= OnSearchFormClosed;
		}
		protected internal virtual void OnSearchFormClosed(object sender, FormClosedEventArgs e) {
			if (searchForm == null)
				return;
			UnsubscribeSearchFormEvents();
			searchForm.Dispose();
			searchForm = null;
		}
		protected internal virtual SearchTextForm CreateSearchForm(SearchFormControllerParameters controllerParameters) {
			return new SearchTextForm(controllerParameters);
		}
		void IRichEditControl.ShowReviewingPaneForm(DocumentModel documentModel, CommentViewInfo commentViewInfo, bool selectParagraph) {
			this.ShowReviewingPaneForm(documentModel, commentViewInfo, selectParagraph, DocumentLogPosition.Zero, DocumentLogPosition.Zero, false, true);
		}
		void IRichEditControl.ShowReviewingPaneForm(DocumentModel documentModel, CommentViewInfo commentViewInfo, DocumentLogPosition start, DocumentLogPosition end, bool setFocus) {
			this.ShowReviewingPaneForm(documentModel, commentViewInfo, false, start, end, setFocus, false);
		}
		protected internal virtual void ShowReviewingPaneForm(DocumentModel documentModel, CommentViewInfo commentViewInfo, bool selectParagraph, DocumentLogPosition start, DocumentLogPosition end, bool setFocus, bool synchronizeSelection) {
			ShowReviewingPaneEventHandler showReviewingPaneHandler = ShowReviewingPane;
			Action action = delegate() {
				if (showReviewingPaneHandler == null)
					return;
				showReviewingPaneHandler(this, new ShowReviewingPaneEventArg(commentViewInfo, selectParagraph, start, end, setFocus, synchronizeSelection));
			};
			if (IsHandleCreated)
				BeginInvoke(action);
			else
				action();
		}
		public void ShowReviewingPaneForm() {
			this.ShowReviewingPaneForm(null, null, false, DocumentLogPosition.Zero, DocumentLogPosition.Zero, false, false);
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
		void IRichEditControl.ShowInsertMergeFieldForm() {
			this.ShowInsertMergeFieldForm();
		}
		protected internal virtual void ShowInsertMergeFieldForm() {
			InsertMergeFieldFormControllerParameters controllerParameters = new InsertMergeFieldFormControllerParameters(this);
			InsertMergeFieldFormShowingEventArgs args = new InsertMergeFieldFormShowingEventArgs(controllerParameters);
			RaiseInsertMergeFieldFormShowing(args);
			if (args.Handled)
				return;
			if (this.insertMergeFieldForm != null)
				this.insertMergeFieldForm.Activate();
			else {
				this.insertMergeFieldForm = new InsertMergeFieldForm(controllerParameters);
				this.insertMergeFieldForm.LookAndFeel.ParentLookAndFeel = LookAndFeel;
				SubscribeInsertMergeFieldFormEvents();
				FormTouchUIAdapter.Show(this.insertMergeFieldForm, this);
			}
		}
		void SubscribeInsertMergeFieldFormEvents() {
			this.insertMergeFieldForm.FormClosed += OnInsertMergeFieldFormClosed;
		}
		void UnsubscribeInsertMergeFieldFormEvents() {
			this.insertMergeFieldForm.FormClosed -= OnInsertMergeFieldFormClosed;
		}
		void OnInsertMergeFieldFormClosed(object sender, EventArgs e) {
			UnsubscribeInsertMergeFieldFormEvents();
			this.insertMergeFieldForm.Dispose();
			this.insertMergeFieldForm = null;
		}
		void IRichEditControl.ShowInsertTableForm(CreateTableParameters parameters, ShowInsertTableFormCallback callback, object callbackData) {
			this.ShowInsertTableForm(parameters, callback, callbackData);
		}
		internal virtual void ShowInsertTableForm(CreateTableParameters parameters, ShowInsertTableFormCallback callback, object callbackData) {
			InsertTableFormControllerParameters controllerParameters = new InsertTableFormControllerParameters(this, parameters);
			InsertTableFormShowingEventArgs args = new InsertTableFormShowingEventArgs(controllerParameters);
			RaiseInsertTableFormShowing(args);
			if (!args.Handled) {
				using (InsertTableForm form = new InsertTableForm(controllerParameters)) {
					form.LookAndFeel.ParentLookAndFeel = LookAndFeel;
					DialogResult result = FormTouchUIAdapter.ShowDialog(form, this);
					if (result == DialogResult.OK)
						callback(form.Controller.SourceParameters, callbackData);
				}
			}
			else {
				DialogResult result = args.DialogResult;
				if (result == DialogResult.OK)
					callback(controllerParameters.Parameters, callbackData);
			}
		}
		void IRichEditControl.ShowInsertTableCellsForm(TableCellsParameters parameters, ShowInsertDeleteTableCellsFormCallback callback, object callbackData) {
			this.ShowInsertTableCellsForm(parameters, callback, callbackData);
		}
		internal virtual void ShowInsertTableCellsForm(TableCellsParameters parameters, ShowInsertDeleteTableCellsFormCallback callback, object callbackData) {
			InsertTableCellsFormControllerParameters controllerParameters = new InsertTableCellsFormControllerParameters(this, parameters);
			InsertTableCellsFormShowingEventArgs args = new InsertTableCellsFormShowingEventArgs(controllerParameters);
			RaiseInsertTableCellsFormShowing(args);
			if (!args.Handled) {
				using (InsertTableCellsForm form = new InsertTableCellsForm(controllerParameters)) {
					form.LookAndFeel.ParentLookAndFeel = LookAndFeel;
					DialogResult result = FormTouchUIAdapter.ShowDialog(form, this);
					if (result == DialogResult.OK)
						callback(form.Controller.SourceParameters, callbackData);
				}
			}
			else {
				DialogResult result = args.DialogResult;
				if (result == DialogResult.OK)
					callback(controllerParameters.CellsParameters, callbackData);
			}
		}
		void IRichEditControl.ShowDeleteTableCellsForm(TableCellsParameters parameters, ShowInsertDeleteTableCellsFormCallback callback, object callbackData) {
			this.ShowDeleteTableCellsForm(parameters, callback, callbackData);
		}
		internal virtual void ShowDeleteTableCellsForm(TableCellsParameters parameters, ShowInsertDeleteTableCellsFormCallback callback, object callbackData) {
			DeleteTableCellsFormControllerParameters controllerParameters = new DeleteTableCellsFormControllerParameters(this, parameters);
			DeleteTableCellsFormShowingEventArgs args = new DeleteTableCellsFormShowingEventArgs(controllerParameters);
			RaiseDeleteTableCellsFormShowing(args);
			if (!args.Handled) {
				using (DeleteTableCellsForm form = CreateDeleteTableCellsFrom(controllerParameters)) {
					form.LookAndFeel.ParentLookAndFeel = LookAndFeel;
					DialogResult result = FormTouchUIAdapter.ShowDialog(form, this);
					if (result == DialogResult.OK)
						callback(form.Controller.SourceParameters, callbackData);
				}
			}
			else {
				DialogResult result = args.DialogResult;
				if (result == DialogResult.OK)
					callback(controllerParameters.CellsParameters, callbackData);
			}
		}
		protected internal virtual DeleteTableCellsForm CreateDeleteTableCellsFrom(DeleteTableCellsFormControllerParameters controllerParameters) {
			return new DeleteTableCellsForm(controllerParameters);
		}
		void IRichEditControl.ShowSplitTableCellsForm(SplitTableCellsParameters parameters, ShowSplitTableCellsFormCallback callback, object callbackData) {
			this.ShowSplitTableCellsForm(parameters, callback, callbackData);
		}
		internal virtual void ShowSplitTableCellsForm(SplitTableCellsParameters parameters, ShowSplitTableCellsFormCallback callback, object callbackData) {
			SplitTableCellsFormControllerParameters controllerParameters = new SplitTableCellsFormControllerParameters(this, parameters);
			SplitTableCellsFormShowingEventArgs args = new SplitTableCellsFormShowingEventArgs(controllerParameters);
			RaiseSplitTableCellsFormShowing(args);
			if (!args.Handled) {
				using (SplitTableCellsForm form = new SplitTableCellsForm(controllerParameters)) {
					form.LookAndFeel.ParentLookAndFeel = LookAndFeel;
					DialogResult result = FormTouchUIAdapter.ShowDialog(form, this);
					if (result == DialogResult.OK)
						callback(form.Controller.SourceParameters, callbackData);
				}
			}
			else {
				DialogResult result = args.DialogResult;
				if (result == DialogResult.OK)
					callback(controllerParameters.Parameters, callbackData);
			}
		}
		void IRichEditControl.ShowTablePropertiesForm(SelectedCellsCollection selectedCells) {
			ShowTablePropertiesForm(selectedCells);
		}
		void IRichEditControl.ShowBorderShadingForm(SelectedCellsCollection selectedCells) {
			ShowBorderShadingForm(selectedCells);
		}
		internal virtual void ShowBorderShadingForm(SelectedCellsCollection selectedCells) {
		}
		internal virtual void ShowTablePropertiesForm(SelectedCellsCollection selectedCells) {
			TablePropertiesFormControllerParameters controllerParameters = new TablePropertiesFormControllerParameters(this, selectedCells);
			TablePropertiesFormShowingEventArgs args = new TablePropertiesFormShowingEventArgs(controllerParameters);
			RaiseTablePropertiesFormShowing(args);
			if (!args.Handled) {
				using (TablePropertiesForm form = new TablePropertiesForm(controllerParameters)) {
					form.LookAndFeel.ParentLookAndFeel = LookAndFeel;
					FormTouchUIAdapter.ShowDialog(form, this);
				}
			}
		}
		void IRichEditControl.ShowTableOptionsForm(Table table) {
			ShowTableOptionsForm(table, this);
		}
		internal void ShowTableOptionsForm(Table table, IWin32Window owner) {
			TableOptionsFormControllerParameters controllerParameters = new TableOptionsFormControllerParameters(this, table);
			TableOptionsFormShowingEventArgs args = new TableOptionsFormShowingEventArgs(controllerParameters);
			RaiseTableOptionsFormShowing(args);
			if (!args.Handled) {
				using (TableOptionsForm form = new TableOptionsForm(args.ControllerParameters)) {
					form.LookAndFeel.ParentLookAndFeel = LookAndFeel;
					FormTouchUIAdapter.ShowDialog(form, owner);
				}
			}
		}
		void IInnerRichEditControlOwner.OnOptionsChangedPlatformSpecific(BaseOptionChangedEventArgs e) {
			this.OnOptionsChangedPlatformSpecific(e);
		}
		protected internal virtual void OnOptionsChangedPlatformSpecific(BaseOptionChangedEventArgs e) {
			if (e.Name != "VisibleAuthors")
				OnDeferredResizeCore();
			if (e.Name == "DataSource" || e.Name == "DataMember" || e.Name == "ViewMergedData" || e.Name == "ActiveRecord")
				SetMailMergeDataControllerBindingContext();
		}
		protected internal virtual void OnReadOnlyChangedPlatformSpecific() {
			if (ShouldShowCaret)
				StartCaretBlinking();
			else
				StopCaretBlinking();
		}
		protected internal virtual void OnActiveViewChangedPlatformSpecific() {
		}
		void IRichEditControl.ShowCaret() {
			this.ShowCaret();
		}
		protected internal virtual void ShowCaret() {
			if (ShouldShowCaret)
				ShowCaretCore();
		}
		protected internal virtual void ShowCaretCore() {
			if (IsUpdateLocked)
				return;
			EnsureSystemCaretCreated();
			if (caret.IsHidden) {
				ShowSystemCaret();
				Painter.DrawCaret();
				caret.IsHidden = false;
			}
		}
		void IRichEditControl.HideCaret() {
			this.HideCaret();
		}
		protected internal virtual void HideCaret() {
			if (Focused)
				HideCaretCore();
		}
		protected internal virtual void HideCaretCore() {
			if (IsUpdateLocked)
				return;
			EnsureSystemCaretCreated();
			if (!caret.IsHidden) {
				HideSystemCaret();
				Painter.DrawCaret();
				caret.IsHidden = true;
			}
		}
		protected internal virtual void EnsureSystemCaretCreated() {
			if (!IsHandleCreated || !Focused)
				return;
			DestroyCaret();
			if (!isPopupMenuShown)
				isCaretDestroyed = !Win32.CreateCaret(Handle, systemCaretBitmapHandle, 0, 0);
		}
		private void DestroyCaret() {
			if (!isCaretDestroyed)
				isCaretDestroyed = Win32.DestroyCaret();
		}
		protected internal virtual void ShowSystemCaret() {
			if (!IsHandleCreated)
				return;
			if (isPopupMenuShown)
				return;
			Rectangle caretBounds = ActiveView.GetCursorBounds();
			if (Bounds.Contains(caretBounds.Location)) {
				Win32.SetCaretPos(caretBounds.X, caretBounds.Y);
				Win32.ShowCaret(Handle);
			}
			else {
				Win32.HideCaret(Handle);
				DestroyCaret();
			}
		}
		protected internal virtual void HideSystemCaret() {
			if (!IsHandleCreated)
				return;
			Rectangle caretBounds = ActiveView.GetCursorBounds();
			if (Bounds.Contains(caretBounds.Location)) {
				Win32.SetCaretPos(caretBounds.X, caretBounds.Y);
				Win32.HideCaret(Handle);
			}
			else {
				Win32.HideCaret(Handle);
				DestroyCaret();
			}
		}
		protected internal virtual void CreateDragCaret() {
			this.dragCaret = new DragCaret(ActiveView);
		}
		protected internal virtual void DestroyDragCaret() {
			if (DragCaret != null && !DragCaret.IsHidden)
				Painter.DrawDragCaret();
			this.dragCaret = null;
		}
		void IInnerRichEditControlOwner.DeactivateViewPlatformSpecific(RichEditView view) {
			this.DeactivateViewPlatformSpecific(view);
		}
		protected internal virtual void DeactivateViewPlatformSpecific(RichEditView view) {
			DisposeBackgroundPainter();
			DisposeViewPainter();
		}
		void IInnerRichEditControlOwner.ActivateViewPlatformSpecific(RichEditView view) {
			this.ActivateViewPlatformSpecific(view);
		}
		protected internal virtual void ActivateViewPlatformSpecific(RichEditView view) {
			CreateBackgroundPainter(view);
			CreateViewPainter(view);
			if (horizontalRuler != null)
				horizontalRuler.RecreatePainter();
			if (verticalRuler != null)
				verticalRuler.RecreatePainter();
		}
		protected internal virtual void DisposeBackgroundPainter() {
			if (backgroundPainter != null) {
				this.backgroundPainter.Dispose();
				this.backgroundPainter = null;
			}
		}
		protected internal virtual RichEditViewBackgroundPainterFactory CreateViewBackgroundPainterFactory() {
			switch (LookAndFeel.ActiveStyle) {
				case ActiveLookAndFeelStyle.Flat:
					return new RichEditViewBackgroundPainterFlatFactory();
				case ActiveLookAndFeelStyle.UltraFlat:
					return new RichEditViewBackgroundPainterUltraFlatFactory();
				case ActiveLookAndFeelStyle.Style3D:
					return new RichEditViewBackgroundPainterStyle3DFactory();
				case ActiveLookAndFeelStyle.Office2003:
					return new RichEditViewBackgroundPainterOffice2003Factory();
				case ActiveLookAndFeelStyle.WindowsXP:
					return new RichEditViewBackgroundPainterWindowsXPFactory();
				case ActiveLookAndFeelStyle.Skin:
				default:
					return new RichEditViewBackgroundPainterSkinFactory();
			}
		}
		protected internal virtual void CreateBackgroundPainter(RichEditView view) {
			RichEditViewBackgroundPainterFactory factory = CreateViewBackgroundPainterFactory();
			this.backgroundPainter = factory.CreatePainter(view);
		}
		protected internal virtual void RecreateBackgroundPainter(RichEditView view) {
			DisposeBackgroundPainter();
			CreateBackgroundPainter(view);
		}
		protected internal virtual void DisposeViewPainter() {
			if (viewPainter != null) {
				this.viewPainter.Dispose();
				this.viewPainter = null;
			}
		}
		protected internal virtual RichEditViewPainterFactory CreateViewPainterFactory() {
			switch (LookAndFeel.ActiveStyle) {
				case ActiveLookAndFeelStyle.Flat:
					return new RichEditViewPainterFlatFactory();
				case ActiveLookAndFeelStyle.UltraFlat:
					return new RichEditViewPainterUltraFlatFactory();
				case ActiveLookAndFeelStyle.Style3D:
					return new RichEditViewPainterStyle3DFactory();
				case ActiveLookAndFeelStyle.Office2003:
					return new RichEditViewPainterOffice2003Factory();
				case ActiveLookAndFeelStyle.WindowsXP:
					return new RichEditViewPainterWindowsXPFactory();
				case ActiveLookAndFeelStyle.Skin:
				default:
					return new RichEditViewPainterSkinFactory();
			}
		}
		protected internal virtual void CreateViewPainter(RichEditView view) {
			RichEditViewPainterFactory factory = CreateViewPainterFactory();
			this.viewPainter = factory.CreatePainter(view);
		}
		protected internal virtual void RecreateViewPainter(RichEditView view) {
			DisposeViewPainter();
			CreateViewPainter(view);
		}
		public static void About() {
			DevExpress.Utils.About.AboutHelper.Show(ProductKind.DXperienceWin, new ProductStringInfoWin(DevExpress.Utils.About.ProductInfoHelper.WinScheduler));
		}
		#region IDisposable implementation
		protected override void Dispose(bool disposing) {
			this.isDisposing = true;
			try {
				if (disposing)
					DisposeCore();
			}
			finally {
				base.Dispose(disposing);
				this.isDisposed = true;
				this.isDisposing = false;
			}
		}
		#endregion
		protected internal virtual void DisposeCore() {
			lock (this) {
				if (!IsDisposed)
					RaiseBeforeDispose();
				DestroyCaretTimer();
				DestroyVerticalScrollBarUpdateTimer();
				DestroyFlushPendingTextInputTimer();
				DestroyFlushRotationControllerTimer();
				DisposeCommon();
				if (printer != null) {
					printer.Dispose();
					printer = null;
				}
				if (verticalScrollbar != null) {
					verticalScrollbar.Dispose();
					verticalScrollbar = null;
				}
				if (horizontalScrollbar != null) {
					horizontalScrollbar.Dispose();
					horizontalScrollbar = null;
				}
				if (verticalRuler != null) {
					verticalRuler.Dispose();
					verticalRuler = null;
				}
				if (horizontalRuler != null) {
					horizontalRuler.Dispose();
					horizontalRuler = null;
				}
				if (appearance != null) {
					UnsubscribeAppearanceEvents();
					appearance.Dispose();
					appearance = null;
				}
				if (lookAndFeel != null) {
					UnsubscribeLookAndFeelEvents();
					lookAndFeel.Dispose();
					lookAndFeel = null;
				}
				DisposeBackgroundPainter();
				DisposeViewPainter();
				if (painter != null) {
					painter.Dispose();
					painter = null;
				}
				this.menuManager = null;
				this.caret = null;
				if (searchForm != null)
					searchForm.Close();
			}
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			BeginUpdate();
			ApplyFontAndForeColor();
			ForceHandleLookAndFeelChanged();
			InitializeFlushPendingTextInputTimer();
			InitializeVerticalScrollBarUpdateTimer();
			InitializeFlushRotationControllerTimer();
			DeferredBackgroundThreadUIUpdater deferredUpdater = (DeferredBackgroundThreadUIUpdater)BackgroundThreadUIUpdater;
			InnerControl.BackgroundThreadUIUpdater = new BeginInvokeBackgroundThreadUIUpdater(GetService<IThreadSyncService>());
			PerformDeferredUIUpdates(deferredUpdater);
			SubscribeIdleEvent(leakSafeEventRouter.OnApplicationIdle);
			if (InnerControl != null)
				InnerControl.UpdateUIOnIdle = true;
			OnUpdateUI();
			((IRichEditControl)this).UpdateControlAutoSize();
			EndUpdate();
		}
		protected virtual void SubscribeIdleEvent(EventHandler handler) {
			Application.Idle += handler;
		}
		protected virtual void UnsubscribeIdleEvent(EventHandler handler) {
			Application.Idle -= handler;
		}
		protected override void OnBindingContextChanged(EventArgs e) {
			base.OnBindingContextChanged(e);
			SetMailMergeDataControllerBindingContext();
		}
		void SetMailMergeDataControllerBindingContext() {
			RichEditDataControllerAdapter dataController = (RichEditDataControllerAdapter)DocumentModel.MailMergeDataController;
			dataController.SetBindingContext(BindingContext);
		}
		protected internal virtual void OnUpdateUI() {
			if (InnerControl != null)
				InnerControl.OnUpdateUI();
		}
		protected override void OnHandleDestroyed(EventArgs e) {
			UnsubscribeIdleEvent(leakSafeEventRouter.OnApplicationIdle);
			if (InnerControl != null)
				InnerControl.UpdateUIOnIdle = false;
			OnUpdateUI();
			DestroyCaretTimer();
			DestroyCaret();
			DestroyVerticalScrollBarUpdateTimer();
			DestroyFlushPendingTextInputTimer();
			if (InnerControl != null) {
				InnerControl.BackgroundThreadUIUpdater = new DeferredBackgroundThreadUIUpdater();
			}
			this.insideHandleDestroyed = true;
			base.OnHandleDestroyed(e);
			this.insideHandleDestroyed = false;
		}
		protected internal virtual void OnApplicationIdle(object sender, EventArgs e) {
			if (InnerControl != null)
				InnerControl.OnApplicationIdle();
		}
		protected override void OnPaint(PaintEventArgs e) {
			if (IsUpdateLocked) {
				ControlDeferredChanges.Redraw = true;   
				return;
			}
			CustomMarkExporter customMarkExporter = new CustomMarkExporter();
			Painter.Draw(e.Graphics, customMarkExporter);
			RaiseCustomMarkDraw(e.Graphics, customMarkExporter);
			base.OnPaint(e);
		}
		void IInnerRichEditControlOwner.Redraw() {
			this.Redraw(false);
		}
		void IInnerRichEditControlOwner.RedrawAfterEndUpdate() {
			this.Redraw(true);
		}
		protected internal virtual void Redraw(bool afterEndUpdate) {
			if (IsUpdateLocked) {
				ControlDeferredChanges.Redraw = true;
			}
			else
				if (!afterEndUpdate)
					CustomRefresh();
				else
					if (IsHandleCreated && !isInsideRefresh)
						BeginInvoke(new Action(CustomRefresh));
		}
		bool isInsideRefresh;
		void CustomRefresh() {
			CustomRefresh(false);
		}
		void CustomRefresh(bool invalidateChildren) {
			isInsideRefresh = true;
			((IRichEditControl)this).UpdateControlAutoSize();
			Invalidate(invalidateChildren);
			Update();
			isInsideRefresh = false;
		}
		void IRichEditControl.RedrawEnsureSecondaryFormattingComplete(RefreshAction action) {
			this.RedrawEnsureSecondaryFormattingComplete(action);
		}
		void IInnerRichEditControlOwner.RedrawEnsureSecondaryFormattingComplete() {
			this.RedrawEnsureSecondaryFormattingComplete();
		}
		internal void RedrawEnsureSecondaryFormattingComplete(RefreshAction action) {
			RedrawEnsureSecondaryFormattingComplete(action == RefreshAction.Transforms);
		}
		internal void RedrawEnsureSecondaryFormattingComplete() {
			RedrawEnsureSecondaryFormattingComplete(false);
		}
		protected internal virtual void RedrawEnsureSecondaryFormattingComplete(bool invalidateChildren) {
			if (IsUpdateLocked) {
				InnerControl.BeginDocumentRendering();
				try {
				}
				finally {
					InnerControl.EndDocumentRendering();
				}
				ControlDeferredChanges.Redraw = true;
			}
			else
				CustomRefresh(invalidateChildren);
		}
		protected override void OnPaintBackground(PaintEventArgs pevent) {
			using (GraphicsCache cache = new GraphicsCache(pevent.Graphics)) {
				BackgroundPainter.Draw(cache, BackgroundBounds);
			}
		}
		bool insideResize;
		protected override void OnResize(EventArgs e) {
			if (!IsHandleCreated)
				return;
			bool nestedResize = insideResize;
			insideResize = true;
			BeginUpdate();
			try {
				RichEditControlPainter oldPainter = this.painter;
				this.painter = new EmptyRichEditControlPainter(this);
				try {
					base.OnResize(e);
					OnResizeCore(false);
					if (!nestedResize && (AutoSizeMode == XtraRichEdit.AutoSizeMode.Both || AutoSizeMode == XtraRichEdit.AutoSizeMode.Vertical)) {
						Size bestSize = CalcBestSize(Size.Width, Size.Height, true);
						Size = bestSize;
					}
				}
				finally {
					this.painter = oldPainter;
				}
			}
			finally {
				EndUpdate();
				insideResize = false;
			}
		}
		protected internal virtual Rectangle CalculateInitialClientBounds() {
			Form form = FindForm();
			if (form != null) {
				if (form.WindowState != FormWindowState.Minimized)
					this.lastValidClientRectangle = this.ClientRectangle;
			}
			else
				this.lastValidClientRectangle = this.ClientRectangle;
			using (GraphicsCache cache = new GraphicsCache(Graphics.FromHwnd(IntPtr.Zero))) {
				BorderPainter borderPainter = BorderHelper.GetPainter(this.BorderStyle, LookAndFeel);
				BorderObjectInfoArgs args = new BorderObjectInfoArgs(cache, lastValidClientRectangle, null, ObjectState.Normal);
				Rectangle objectClientRectangle = borderPainter.GetObjectClientRectangle(args);
				borderSize = lastValidClientRectangle.Size - objectClientRectangle.Size;
				return objectClientRectangle;
			}
		}
		protected internal virtual int CalculateVerticalRulerWidth() {
			RichEditRulerVisibility visibility = Options.VerticalRuler.Visibility;
			switch (visibility) {
				default:
				case RichEditRulerVisibility.Auto:
					if (ActiveView.ShowVerticalRulerByDefault)
						return verticalRuler.GetRulerWidthInPixels();
					else
						return 0;
				case RichEditRulerVisibility.Visible:
					return verticalRuler.GetRulerWidthInPixels();
				case RichEditRulerVisibility.Hidden:
					return 0;
			}
		}
		protected internal virtual int CalculateHorizontalRulerHeight() {
			RichEditRulerVisibility visibility = Options.HorizontalRuler.Visibility;
			switch (visibility) {
				default:
				case RichEditRulerVisibility.Auto:
					if (ActiveView.ShowHorizontalRulerByDefault)
						return horizontalRuler.GetRulerHeightInPixels();
					else
						return 0;
				case RichEditRulerVisibility.Visible:
					return horizontalRuler.GetRulerHeightInPixels();
				case RichEditRulerVisibility.Hidden:
					return 0;
			}
		}
		protected internal virtual int CalculateVerticalScrollbarWidth() {
			RichEditScrollbarVisibility visibility = Options.VerticalScrollbar.Visibility;
			switch (visibility) {
				default:
				case RichEditScrollbarVisibility.Auto:
					return verticalScrollbar.GetDefaultVerticalScrollBarWidth();
				case RichEditScrollbarVisibility.Visible:
					return verticalScrollbar.GetDefaultVerticalScrollBarWidth(); 
				case RichEditScrollbarVisibility.Hidden:
					return 0;
			}
		}
		protected internal virtual int CalculateHorizontalScrollbarHeight() {
			bool scrollBarVisible = CalculateHorizontalScrollbarVisibility();
			if (scrollBarVisible)
				return horizontalScrollbar.GetDefaultHorizontalScrollBarHeight();
			else
				return 0;
		}
		protected virtual bool ShouldUpdateRulers() {
			return IsHandleCreated && Parent != null;
		}
		void IInnerRichEditControlOwner.UpdateRulers() {
			this.UpdateRulers();
		}
		void IInnerRichEditControlOwner.UpdateVerticalRuler() {
			UpdateVerticalRulerCore();
		}
		void IInnerRichEditControlOwner.UpdateHorizontalRuler() {
			UpdateHorizontalRulerCore();
		}
		protected internal virtual void UpdateRulersCore() {
			UpdateHorizontalRulerCore();
			UpdateVerticalRulerCore();
		}
		protected internal virtual void UpdateRulers() {
			if (horizontalRuler != null && horizontalRuler.CanUpdate())
				UpdateHorizontalRulerCore();
			if (verticalRuler != null && verticalRuler.CanUpdate())
				UpdateVerticalRulerCore();
		}
		protected internal virtual void UpdateHorizontalRulerCore() {
			bool visible = CalculateHorizontalRulerVisibility();
			horizontalRuler.Visible = visible;
			if (horizontalRuler != null && visible)
				horizontalRuler.Reset();
		}
		protected internal virtual void UpdateVerticalRulerCore() {
			bool visible = CalculateVerticalRulerVisibility();
			VerticalRuler.Visible = visible;
			if (verticalRuler != null && visible)
				verticalRuler.Reset();
		}
		protected internal virtual bool CalculateVerticalRulerVisibility() {
			RichEditRulerVisibility visibility = Options.VerticalRuler.Visibility;
			switch (visibility) {
				default:
				case RichEditRulerVisibility.Auto:
					return ActiveView.ShowVerticalRulerByDefault;
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
					return ActiveView.ShowHorizontalRulerByDefault;
				case RichEditRulerVisibility.Visible:
					return true;
				case RichEditRulerVisibility.Hidden:
					return false;
			}
		}
		protected internal virtual void UpdateScrollbarsVisibility() {
			verticalScrollbar.SetVisibility(CalculateVerticalScrollbarVisibility());
			horizontalScrollbar.SetVisibility(CalculateHorizontalScrollbarVisibility());
			if (horizontalScrollbar.ActualVisible)
				horizontalScrollbar.Enabled = ActiveView.HorizontalScrollController.IsScrollPossible();
			if (verticalScrollbar.ActualVisible)
				verticalScrollbar.Enabled = ActiveView.VerticalScrollController.IsScrollPossible();
		}
		protected internal virtual bool CalculateVerticalScrollbarVisibility() {
			RichEditScrollbarVisibility visibility = Options.VerticalScrollbar.Visibility;
			switch (visibility) {
				default:
				case RichEditScrollbarVisibility.Auto:
					return true;
				case RichEditScrollbarVisibility.Visible:
					return true;
				case RichEditScrollbarVisibility.Hidden:
					return false;
			}
		}
		protected internal virtual bool CalculateHorizontalScrollbarVisibility() {
			RichEditScrollbarVisibility visibility = Options.HorizontalScrollbar.Visibility;
			switch (visibility) {
				default:
				case RichEditScrollbarVisibility.Auto:
					bool isScrollPossible = ActiveView.HorizontalScrollController.IsScrollPossible();
					if (isScrollPossible)
						return true;
					ActiveView.PageViewInfoGenerator.CalculateMaxPageWidth();
					if (ActiveView.PageViewInfoGenerator.MaxPageWidth > ActiveView.PageViewInfoGenerator.ViewPortBounds.Width + 1)
						return true;
					else
						return false;
				case RichEditScrollbarVisibility.Visible:
					return true;
				case RichEditScrollbarVisibility.Hidden:
					return false;
			}
		}
		protected internal virtual void OnDeferredResizeCore() {
			if (IsUpdateLocked)
				ControlDeferredChanges.Resize = true;
			else {
				OnResizeCore(true);
				RedrawEnsureSecondaryFormattingComplete();
			}
		}
		void IInnerRichEditControlOwner.OnResizeCore() {
			this.OnResizeCore(true);
		}
		protected internal virtual void OnResizeCore(bool ensureCaretVisibleonResize) {
			Debug.Assert(!DocumentModel.IsUpdateLocked);
			Size initialSize = Size;
			Rectangle initialClientBounds = CalculateInitialClientBounds();
			bool oldInsideResize = insideResize;
			insideResize = true;
			for (; ; ) {
				UpdateScrollbarsVisibility();
				if (!PerformResize(initialClientBounds, ensureCaretVisibleonResize))
					break;
				if (Size != initialSize) {
					initialSize = Size;
					initialClientBounds = CalculateInitialClientBounds();
				}
			}
			UpdateRulersCore();
			UpdateVerticalScrollBar(false);
			insideResize = oldInsideResize;
		}
		protected internal virtual bool PerformResize(Rectangle initialClientBounds, bool ensureCaretVisibleonResize) {
			Debug.Assert(!DocumentModel.IsUpdateLocked);
			this.clientBounds = initialClientBounds;
			int verticalScrollbarWidth = CalculateVerticalScrollbarWidth();
			int horizontalScrollbarHeight = CalculateHorizontalScrollbarHeight();
			this.SuspendLayout();
			verticalScrollbar.Bounds = GetVerticalScrollbarBounds(verticalScrollbarWidth, 0, horizontalScrollbar.IsOverlapScrollBar ? 0 : horizontalScrollbarHeight);
			horizontalScrollbar.Bounds = new Rectangle(clientBounds.Left, clientBounds.Bottom - horizontalScrollbarHeight, clientBounds.Width - verticalScrollbarWidth, horizontalScrollbarHeight);
			this.ResumeLayout(false);
			if (!horizontalScrollbar.IsOverlapScrollBar) {
				this.clientBounds.Width -= verticalScrollbarWidth;
				this.clientBounds.Height -= horizontalScrollbarHeight;
				this.cornerBounds = Rectangle.FromLTRB(horizontalScrollbar.Right, verticalScrollbar.Bottom, clientBounds.Right + verticalScrollbarWidth, clientBounds.Bottom + horizontalScrollbarHeight);
			}
			else
				this.cornerBounds = Rectangle.Empty;
			this.backgroundBounds = Rectangle.FromLTRB(0, 0, clientBounds.Right, clientBounds.Bottom);
			int horizontalRulerHeight = CalculateHorizontalRulerHeight();
			int verticalRulerWidth = CalculateVerticalRulerWidth();
			this.SuspendLayout();
			horizontalRuler.Bounds = new Rectangle(clientBounds.Left, clientBounds.Top, clientBounds.Width, horizontalRulerHeight);
			verticalRuler.Bounds = new Rectangle(clientBounds.Left, clientBounds.Top, verticalRulerWidth, clientBounds.Height);
			this.ResumeLayout(false);
			this.clientBounds.Y += horizontalRulerHeight;
			this.clientBounds.Height -= horizontalRulerHeight;
			this.clientBounds.X += verticalRulerWidth;
			this.clientBounds.Width -= verticalRulerWidth;
			ResizeView(ensureCaretVisibleonResize);
			return horizontalScrollbarHeight != CalculateHorizontalScrollbarHeight();
		}
		protected internal virtual Rectangle GetVerticalScrollbarBounds(int width, int offset, int horizontalScrollbarHeight) {
			return new Rectangle(clientBounds.Right - width, clientBounds.Top + offset, width, clientBounds.Height - offset - horizontalScrollbarHeight);
		}
		void IInnerRichEditControlOwner.ResizeView(bool ensureCaretVisibleonResize) {
			this.ResizeView(ensureCaretVisibleonResize);
		}
		protected internal virtual void ResizeView(bool ensureCaretVisibleonResize) {
			this.viewBounds = CalculateViewBounds(clientBounds);
			Rectangle normalizedViewBounds = viewBounds;
			normalizedViewBounds.X = 0;
			normalizedViewBounds.Y = 0;
			ActiveView.OnResize(normalizedViewBounds, ensureCaretVisibleonResize);
		}
		void IRichEditControl.OnViewPaddingChanged() {
			this.OnViewPaddingChanged();
		}
		protected internal virtual void OnViewPaddingChanged() {
			BeginUpdate();
			try {
				OnResizeCore(true);
				RedrawEnsureSecondaryFormattingComplete();
			}
			finally {
				EndUpdate();
			}
		}
		#region Keyboard handling
		bool suppressKeyPressHandling;
		[System.Security.SecuritySafeCritical]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
		protected override bool ProcessDialogChar(char charCode) {
			if (Control.ModifierKeys != Keys.Alt)
				return false;
			else
				return base.ProcessDialogChar(charCode);
		}
		protected override bool IsInputKey(Keys keyData) {
			if ((keyData == Keys.Tab || (keyData == (Keys.Tab | Keys.Shift))) && !AcceptsTab)
				return false;
			if (keyData == Keys.Return && !AcceptsReturn)
				return false;
			if (keyData == Keys.Escape && !AcceptsEscape)
				return false;
			if (InnerControl == null)
				return false;
#if !SL
			return InnerControl.Enabled;
#else
			return InnerControl.IsEnabled;
#endif
		}
		protected internal bool IsAltGrPressed() {
			const int VK_LCONTROL = 0xA2;
			const int VK_RMENU = 0xA5;
			bool leftCtrlPressed = Win32.GetAsyncKeyState((Keys)VK_LCONTROL) != 0;
			bool rightMenuPressed = Win32.GetAsyncKeyState((Keys)VK_RMENU) != 0;
			return leftCtrlPressed && rightMenuPressed;
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			if (IsDisposed)
				return;
			base.OnKeyDown(e);
			if (e.Handled)
				return;
			if (e.KeyCode == Keys.Tab && !AcceptsTab)
				return;
			if (e.KeyCode == Keys.Return && !AcceptsReturn)
				return;
			if (e.KeyCode == Keys.Escape && !AcceptsEscape)
				return;
			if (InnerControl != null && !IsAltGrPressed()) {
				InnerControl.OnKeyDown(e);
				suppressKeyPressHandling = e.Handled;
			}
		}
		protected override void OnKeyUp(KeyEventArgs e) {
			if (IsDisposed)
				return;
			suppressKeyPressHandling = false;
			if (InnerControl != null && !IsAltGrPressed())
				InnerControl.OnKeyUp(e);
			base.OnKeyUp(e);
		}
		protected override void OnKeyPress(KeyPressEventArgs e) {
			if (IsDisposed)
				return;
			if (!suppressKeyPressHandling) {
				base.OnKeyPress(e);
				if (e.Handled)
					return;
				if (InnerControl != null)
					InnerControl.OnKeyPress(e);
				e.Handled = true; 
			}
			else
				base.OnKeyPress(e);
		}
		#endregion
		#region Mouse handling
		protected override void OnMouseMove(MouseEventArgs e) {
			if (IsDisposed)
				return;
			if (DesignMode)
				return;
			if (horizontalScrollbar != null) horizontalScrollbar.OnAction(ScrollNotifyAction.MouseMove);
			if (verticalScrollbar != null) verticalScrollbar.OnAction(ScrollNotifyAction.MouseMove);
			if (InnerControl != null)
				InnerControl.OnMouseMove(e);
			base.OnMouseMove(e);
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			if (IsDisposed)
				return;
			if (DesignMode)
				return;
			if (InnerControl != null)
				InnerControl.OnMouseDown(e);
			base.OnMouseDown(e);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			if (IsDisposed)
				return;
			if (DesignMode)
				return;
			if (InnerControl != null)
				InnerControl.OnMouseUp(e);
			(InnerControl as IGestureStateIndicator).OnGestureEnd();
			base.OnMouseUp(e);
		}
		protected override void OnMouseWheel(MouseEventArgs e) {
			if (IsDisposed)
				return;
			if (DesignMode)
				return;
			if (XtraForm.ProcessSmartMouseWheel(this, e))
				return;
			OnMouseWheelCore(e);
		}
		protected virtual void OnMouseWheelCore(MouseEventArgs e) {
			if (InnerControl != null)
				InnerControl.OnMouseWheel(e);
			base.OnMouseWheel(e);
		}
		protected override void OnDragEnter(DragEventArgs e) {
			if (IsDisposed)
				return;
			if (DesignMode) {
				base.OnDragEnter(e);
				return;
			}
			if (InnerControl != null)
				InnerControl.OnDragEnter(e);
			base.OnDragEnter(e);
		}
		protected override void OnDragOver(DragEventArgs e) {
			if (IsDisposed)
				return;
			if (DesignMode) {
				base.OnDragOver(e);
				return;
			}
			if (InnerControl != null)
				InnerControl.OnDragOver(e);
			base.OnDragOver(e);
		}
		protected override void OnDragDrop(DragEventArgs e) {
			if (IsDisposed)
				return;
			if (DesignMode) {
				base.OnDragDrop(e);
				return;
			}
			if (InnerControl != null)
				InnerControl.OnDragDrop(e);
			base.OnDragDrop(e);
		}
		protected override void OnDragLeave(EventArgs e) {
			if (IsDisposed)
				return;
			if (DesignMode) {
				base.OnDragLeave(e);
				return;
			}
			if (InnerControl != null)
				InnerControl.OnDragLeave(e);
			base.OnDragLeave(e);
		}
		protected override void OnGiveFeedback(GiveFeedbackEventArgs e) {
			if (IsDisposed)
				return;
			if (InnerControl != null)
				InnerControl.OnGiveFeedback(e);
			base.OnGiveFeedback(e);
		}
		protected override void OnQueryContinueDrag(QueryContinueDragEventArgs e) {
			if (IsDisposed)
				return;
			if (InnerControl != null)
				InnerControl.OnQueryContinueDrag(e);
			base.OnQueryContinueDrag(e);
		}
		#endregion
		void IInnerRichEditControlOwner.OnResize() {
			this.OnResize(EventArgs.Empty);
		}
		void IInnerRichEditControlOwner.ApplyChangesCorePlatformSpecific(DocumentModelChangeActions changeActions) {
			this.ApplyChangesCorePlatformSpecific(changeActions);
		}
		void IInnerRichEditDocumentServerOwner.RaiseDeferredEvents(DocumentModelChangeActions changeActions) {
			this.RaiseDeferredEvents(changeActions);
		}
		protected internal virtual void ApplyChangesCorePlatformSpecific(DocumentModelChangeActions changeActions) {
			if ((changeActions & DocumentModelChangeActions.Redraw) != 0)
				RedrawEnsureSecondaryFormattingComplete();
		}
		protected internal virtual void RaiseDeferredEvents(DocumentModelChangeActions changeActions) {
			if ((changeActions & DocumentModelChangeActions.RaiseEmptyDocumentCreated) != 0 || (changeActions & DocumentModelChangeActions.RaiseDocumentLoaded) != 0)
				OnPageBackgroundChangedPlatformSpecific();
			InnerRichEditControl innerControl = InnerControl;
#if!SL
			if ((innerControl != null) && ((changeActions & DocumentModelChangeActions.RaiseContentChanged) != 0)) {
				RaiseContentChangedCore();
			}
			IThreadSyncService service = GetService<IThreadSyncService>();
			if (!UseDeferredDataBindingNotifications || IsBoundUpdatingProperty("RtfText") || IsBoundUpdatingProperty("WordMLText") || IsBoundUpdatingProperty("Text") || IsBoundUpdatingProperty("HtmlText")) {
				DocumentModel.InternalAPI.OnEndDocumentUpdate(DocumentModel, new DocumentUpdateCompleteEventArgs(DocumentModel.DeferredChanges));
				if (innerControl != null) {
					if ((changeActions & DocumentModelChangeActions.RaiseContentChanged) != 0)
						innerControl.RaiseBindingNotifications();
					service.EnqueueInvokeInUIThread(new Action(delegate() { innerControl.RaiseDeferredEventsCore(changeActions | DocumentModelChangeActions.SuppressBindingsNotifications); }));
				}
			}
			else
#endif
				if (innerControl != null)
					service.EnqueueInvokeInUIThread(new Action(delegate() { innerControl.RaiseDeferredEventsCore(changeActions); }));
		}
#if !SL && !WPF
		internal bool IsBoundUpdatingProperty(string name) { return IsInPropertySetting(DataBindings[name]); }
		static System.Reflection.FieldInfo inSetPropValueFieldInfo = null;
		internal bool IsInPropertySetting(Binding binding) {
			if (binding == null)
				return false;
			try {
				if (inSetPropValueFieldInfo == null)
					inSetPropValueFieldInfo = typeof(Binding).GetField("inSetPropValue", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
				if (inSetPropValueFieldInfo != null)
					return (bool)inSetPropValueFieldInfo.GetValue(binding);
			}
			catch {
			}
			return false;
		}
#endif
		protected internal virtual Rectangle CalculateViewPixelBounds(Rectangle clientBounds) {
			Padding viewPadding = ActiveView.ActualPadding;
			Rectangle result = clientBounds;
			result.X += viewPadding.Left;
			result.Width -= viewPadding.Left + viewPadding.Right;
			result.Y += viewPadding.Top;
			result.Height -= viewPadding.Top + viewPadding.Bottom;
			return result;
		}
		protected internal virtual Size CalcViewBestSize(bool fixedWidth) {
			Size result = ActiveView.CalcBestSize(fixedWidth);
			Padding viewPadding = ActiveView.ActualPadding;
			result.Width += viewPadding.Left + viewPadding.Right;
			result.Height += viewPadding.Top + viewPadding.Bottom;
			return result;
		}
		protected internal virtual Rectangle CalculateViewBounds(Rectangle clientBounds) {
			Rectangle viewPixelBounds = CalculateViewPixelBounds(clientBounds);
			return DocumentModel.LayoutUnitConverter.PixelsToLayoutUnits(viewPixelBounds, DpiX, DpiY);
		}
		Rectangle IInnerRichEditControlOwner.CalculateActualViewBounds(Rectangle previousViewBounds) {
			return this.CalculateActualViewBounds(previousViewBounds);
		}
		protected internal virtual Rectangle CalculateActualViewBounds(Rectangle previousViewBounds) {
			return CalculateViewBounds(ClientBounds);
		}
		void IInnerRichEditControlOwner.Redraw(RefreshAction action) {
			this.Redraw(action);
		}
		internal void Redraw(RefreshAction action) {
			Redraw(false);
		}
		internal void SetFocus() {
			this.Focus();
		}
		protected override void OnGotFocus(EventArgs e) {
			base.OnGotFocus(e);
			if (ShouldShowCaret)
				StartCaretBlinking();
			StartPendingInput();
			OnUpdateUI();
		}
		protected override void OnLostFocus(EventArgs e) {
			base.OnLostFocus(e);
			if (InnerIsDisposing || IsDisposed) 
				return;
			if (!ShouldShowCaret) {
				StopCaretBlinking();
				DestroyCaret();
			}
			StopPendingInput();
			CloseImeWindow(ImeCloseStatus.ImeCompositionComplete);
			OnUpdateUI();
		}
		protected override void OnEnabledChanged(EventArgs e) {
			base.OnEnabledChanged(e);
			if (this.Enabled && this.Focused) {
				if (ShouldShowCaret)
					StartCaretBlinking();
				StartPendingInput();
			}
			else {
				if (!ShouldShowCaret)
					StopCaretBlinking();
				StopPendingInput();
			}
			VerticalScrollBar.Enabled = Enabled;
			HorizontalScrollBar.Enabled = Enabled;
			OnUpdateUI();
		}
		protected internal virtual void StartCaretBlinking() {
			DestroyCaretTimer();
			InitializeCaretTimer();
			ShowCaretCore();
		}
		protected internal virtual void StopCaretBlinking() {
			DestroyCaretTimer();
			HideCaretCore();
		}
		protected internal virtual void StartPendingInput() {
			if (flushPendingTextInputTimer != null)
				flushPendingTextInputTimer.Enabled = true;
		}
		protected internal virtual void StopPendingInput() {
			if (flushPendingTextInputTimer != null)
				flushPendingTextInputTimer.Enabled = false;
		}
		protected override void OnVisibleChanged(EventArgs e) {
			base.OnVisibleChanged(e);
			OnUpdateUI();
		}
		public virtual void LoadDocument(string fileName) {
			if (InnerControl != null)
				InnerControl.LoadDocument(fileName, DocumentFormat.Undefined);
		}
		public virtual void LoadDocument(string fileName, DocumentFormat documentFormat) {
			if (InnerControl != null)
				InnerControl.LoadDocument(fileName, documentFormat);
		}
		public virtual void LoadDocumentTemplate(string fileName) {
			if (InnerControl != null)
				InnerControl.LoadDocumentTemplate(fileName, DocumentFormat.Undefined);
		}
		public virtual void LoadDocumentTemplate(string fileName, DocumentFormat documentFormat) {
			if (InnerControl != null)
				InnerControl.LoadDocumentTemplate(fileName, documentFormat);
		}
		public virtual void SaveDocument(string fileName, DocumentFormat documentFormat) {
			if (InnerControl != null)
				InnerControl.SaveDocument(fileName, documentFormat);
		}
		public virtual void LoadDocument(IWin32Window parent) {
			if (InnerControl != null)
				InnerControl.LoadDocument(parent);
		}
		public virtual bool SaveDocument() {
			if (InnerControl != null)
				return InnerControl.SaveDocument();
			else
				return false;
		}
		public virtual bool SaveDocument(IWin32Window parent) {
			if (InnerControl != null)
				return InnerControl.SaveDocument(parent);
			else
				return false;
		}
		protected internal IPrintable PrintableImplementation { get { return InnerControl; } }
		protected internal virtual void CheckPrintableImplmenentation() {
			if (PrintableImplementation == null) {
				string message = String.Format(XtraRichEditLocalizer.GetString(XtraRichEditStringId.Msg_PrintingUnavailable),
					AssemblyInfo.SRAssemblyPrinting + ", Version=" + AssemblyInfo.Version,
					String.Empty);
				throw new NotSupportedException(message);
			}
		}
		#region IPrintable Members
		bool IPrintable.CreatesIntersectedBricks {
			get {
				CheckPrintableImplmenentation();
				return PrintableImplementation.CreatesIntersectedBricks;
			}
		}
		UserControl IPrintable.PropertyEditorControl {
			get {
				CheckPrintableImplmenentation();
				return PrintableImplementation.PropertyEditorControl;
			}
		}
		void IPrintable.AcceptChanges() {
			CheckPrintableImplmenentation();
			PrintableImplementation.AcceptChanges();
		}
		void IPrintable.RejectChanges() {
			CheckPrintableImplmenentation();
			PrintableImplementation.RejectChanges();
		}
		bool IPrintable.HasPropertyEditor() {
			CheckPrintableImplmenentation();
			return PrintableImplementation.HasPropertyEditor();
		}
		void IPrintable.ShowHelp() {
			CheckPrintableImplmenentation();
			PrintableImplementation.ShowHelp();
		}
		bool IPrintable.SupportsHelp() {
			CheckPrintableImplmenentation();
			return PrintableImplementation.SupportsHelp();
		}
		#endregion
		#region IBasePrintable Members
		void IBasePrintable.Initialize(IPrintingSystem ps, ILink link) {
			CheckPrintableImplmenentation();
			PrintableImplementation.Initialize(ps, link);
		}
		void IBasePrintable.CreateArea(string areaName, IBrickGraphics graphics) {
			CheckPrintableImplmenentation();
			PrintableImplementation.CreateArea(areaName, graphics);
		}
		void IBasePrintable.Finalize(IPrintingSystem ps, ILink link) {
			if (PrintableImplementation == null)
				return;
			PrintableImplementation.Finalize(ps, link);
		}
		#endregion
		#region Printing support methods and properties
		IDisposable printer;
		ComponentPrinterBase Printer {
			get {
				if (printer == null) {
					ExportToPdfPrintingSystem printingSystem = new ExportToPdfPrintingSystem();
					printer = new RichEditComponentPrinter(this, printingSystem);
				}
				return (ComponentPrinterBase)printer;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsPrintingAvailable { get { return PrintableImplementation != null && ComponentPrinterBase.IsPrintingAvailable(false); } }
		bool IRichEditControl.IsPrintPreviewAvailable { get { return IsPrintingAvailable; } }
		int IRichEditControl.SkinLeftMargin { get { return SkinPaintHelper.GetSkinEdges(LookAndFeel, RichEditPrintingSkins.SkinPageBorder).Left; } }
		int IRichEditControl.SkinRightMargin { get { return SkinPaintHelper.GetSkinEdges(LookAndFeel, RichEditPrintingSkins.SkinPageBorder).Right; } }
		int IRichEditControl.SkinTopMargin { get { return SkinPaintHelper.GetSkinEdges(LookAndFeel, RichEditPrintingSkins.SkinPageBorder).Top; } }
		int IRichEditControl.SkinBottomMargin { get { return SkinPaintHelper.GetSkinEdges(LookAndFeel, RichEditPrintingSkins.SkinPageBorder).Bottom; } }
		bool IRichEditControl.UseSkinMargins { get { return true; } }
		public void ShowPrintPreview() {
			ExecutePrinterAction(delegate() {
				if (Options.Printing.PrintPreviewFormKind == PrintPreviewFormKind.Bars)
					Printer.ShowPreview(FindForm(), this.LookAndFeel);
				else
					Printer.ShowRibbonPreview(FindForm(), this.LookAndFeel);
			});
		}
		public void ShowPrintDialog() {
			ExecutePrinterAction(delegate() {
				Printer.PrintDialog();
				Printer.ClearDocument();
			});
		}
		public void Print() {
			ExecutePrinterAction(delegate() {
				Printer.Print();
				Printer.ClearDocument();
			});
		}
		#endregion
		#region SubscribeLookAndFeelEvents
		protected internal virtual void SubscribeLookAndFeelEvents() {
			lookAndFeel.StyleChanged += new EventHandler(OnLookAndFeelChanged);
		}
		#endregion
		#region UnsubscribeLookAndFeelEvents
		protected internal virtual void UnsubscribeLookAndFeelEvents() {
			lookAndFeel.StyleChanged -= new EventHandler(OnLookAndFeelChanged);
		}
		#endregion
		protected internal virtual void ForceHandleLookAndFeelChanged() {
			this.horizontalRuler.OnLookAndFeelChanged();
			this.verticalRuler.OnLookAndFeelChanged();
			RecreateBackgroundPainter(ActiveView);
			RecreateViewPainter(ActiveView);
			OnResizeCore(true);
		}
		protected internal virtual void OnLookAndFeelChanged(object sender, EventArgs e) {
			if (painter != null)
				this.painter.Dispose();
			skinTextColors = GetSkinTextColors();
			commentPadding = GetCommentsSkinPadding();
			this.painter = CreatePainter();
			InnerControl.ApplyNewCommentPadding();
			if (!IsHandleCreated)
				return;
			ForceHandleLookAndFeelChanged();
			RedrawEnsureSecondaryFormattingComplete();
		}
		#region SubscribeAppearanceEvents
		protected internal virtual void SubscribeAppearanceEvents() {
			appearance.Changed += OnAppearanceChanged;
		}
		#endregion
		#region UnsubscribeAppearanceEvents
		protected internal virtual void UnsubscribeAppearanceEvents() {
			appearance.Changed -= OnAppearanceChanged;
		}
		#endregion
		protected internal virtual void OnAppearanceChanged(object sender, EventArgs e) {
			ApplyFontAndForeColor();
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
		protected internal virtual bool ShouldApplyForeColor() {
			switch (Options.Behavior.ForeColorSource) {
				default:
				case RichEditBaseValueSource.Auto:
					if (appearance == null)
						return false;
					AppearanceObject text = appearance.Text;
					return text.Options.UseForeColor && text.ForeColor != DXColor.Empty && text.ForeColor != DXColor.Transparent;
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
					if (appearance == null)
						return false;
					AppearanceObject text = appearance.Text;
					return text.Options.UseFont;
				case RichEditBaseValueSource.Control:
					return true;
				case RichEditBaseValueSource.Document:
					return false;
			}
		}
		protected internal virtual void ApplyFont(CharacterProperties characterProperties, Font font) {
			switch (Options.Behavior.FontSource) {
				default:
				case RichEditBaseValueSource.Auto:
					CharacterPropertiesFontAssignmentHelper.AssignFont(characterProperties, font);
					break;
				case RichEditBaseValueSource.Control:
					CharacterPropertiesFontAssignmentHelper.AssignFont(characterProperties, this.Font);
					break;
				case RichEditBaseValueSource.Document:
					break;
			}
		}
		protected internal virtual void OnBorderStyleChanged() {
			if (!IsHandleCreated)
				return;
			OnResizeCore(true);
			RedrawEnsureSecondaryFormattingComplete();
		}
		void IInnerRichEditControlOwner.OnZoomFactorChangingPlatformSpecific() {
			this.OnZoomFactorChangingPlatformSpecific();
		}
		protected internal virtual void OnZoomFactorChangingPlatformSpecific() {
			ViewPainter.ResetCache();
		}
		protected internal virtual void AddServicesPlatformSpecific() {
			AddService(typeof(IRichEditCommandFactoryService), CreateRichEditCommandFactoryService());
			AddService(typeof(IImeService), new ImeService(this));
			AddService(typeof(IToolTipService), new ToolTipService(this));
		}
		protected virtual IRichEditCommandFactoryService CreateRichEditCommandFactoryService() {
			return new WinFormsRichEditCommandFactoryService(this);
		}
		[System.Security.SecuritySafeCritical]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
		protected override void WndProc(ref Message m) {
			try {
				if (gestureHelper.WndProc(ref m))
					return;
				const int WM_CONTEXTMENU = 0x7B;
				const int WM_GETDLGCODE = 0x87;
				const int DLGC_WANTTAB = 0x02;
				const int DLGC_WANTALLKEYS = 0x04;
				const int WM_HMOUSEWHEEL = 0x20E;
				switch (m.Msg) {
					case WM_CONTEXTMENU:
						if (OnWmContextMenu(ref m))
							return;
						else
							break;
					case WM_GETDLGCODE:
						if (AcceptsTab)
							m.Result = (IntPtr)(DLGC_WANTTAB | DLGC_WANTALLKEYS);
						else
							m.Result = (IntPtr)(DLGC_WANTALLKEYS);
						return;
					case WM_HMOUSEWHEEL:
						DXMouseEventArgs args = ControlHelper.GenerateMouseHWheel(ref m, this);
						OnMouseWheel(args);
						return;
					default:
						break;
				}
				if (ImeHelper.IsImeMessage(ref m)) {
					IImeService imeService = GetService<IImeService>();
					if (imeService != null && imeService.TrackMessage(ref m))
						return;
				}
				base.WndProc(ref m);
				CodedUISupport.CodedUIMessagesHandler.ProcessCodedUIMessage(ref m, this);
			}
			catch (Exception e) {
				if (!HandleException(e))
					throw;
			}
		}
		[System.Security.SecuritySafeCritical]
		protected internal virtual bool OnWmContextMenu(ref Message m) {
			this.Focus();
			Point screenMousePosition = new Point((short)((int)m.LParam), ((int)m.LParam) >> 0x10);
			Point pt = CalculatePopupMenuPosition(screenMousePosition);
			if (screenMousePosition.X != -1)
				InnerControl.MouseHandler.OnPopupMenu(new MouseEventArgs(MouseButtons.Right, 1, pt.X, pt.Y, 0));
			if (ClientBounds.Contains(pt))
				return OnPopupMenu(pt);
			return false;
		}
		protected internal virtual Point CalculatePopupMenuPosition(Point screenMousePosition) {
			if (screenMousePosition.X == -1) {
				CaretPosition caretPosition = ActiveView.CaretPosition;
				if (!caretPosition.Update(DocumentLayoutDetailsLevel.Character))
					return new Point(Width / 2, Height / 2);
				Rectangle caretBounds = caretPosition.CalculateCaretBounds();
				Point pointInLayoutUnits = ActiveView.CreatePhysicalPoint(caretPosition.PageViewInfo, new Point(caretBounds.X, caretBounds.Bottom));
				Point pt = DocumentModel.LayoutUnitConverter.LayoutUnitsToPixels(pointInLayoutUnits, DpiX, DpiY);
				pt.X = Math.Max(pt.X, ClientBounds.X);
				pt.X = Math.Min(pt.X, ClientBounds.Right - 1);
				pt.Y = Math.Max(pt.Y, ClientBounds.Y);
				pt.Y = Math.Min(pt.Y, ClientBounds.Bottom - 1);
				return pt;
			}
			else
				return PointToClient(screenMousePosition);
		}
		protected internal virtual bool OnPopupMenu(Point point) {
			if (!Options.Behavior.ShowPopupMenuAllowed)
				return false;
			RichEditPopupMenu menu = CreatePopupMenu();
			menu = RaisePopupMenuShowing(menu);
#pragma warning disable 612 // Obsolete
#pragma warning disable 618 // Obsolete
			menu = RaisePreparePopupMenu(menu);
#pragma warning restore 618 // Obsolete
#pragma warning restore 612 // Obsolete
			if (menu == null || menu.Items.Count <= 0)
				return false;
			isPopupMenuShown = true;
			Win32.HideCaret(Handle);
			DestroyCaret();
			menu.CloseUp += OnPopupMenuCloseUp;
			MenuManagerHelper.ShowMenu(menu, LookAndFeel, MenuManager, this, point);
			return true;
		}
		protected virtual RichEditPopupMenu CreatePopupMenu() {
			RichEditContentMenuBuilder builder = CreateContentMenuBuilder();
			return (RichEditPopupMenu)builder.CreatePopupMenu();
		}
		protected virtual RichEditContentMenuBuilder CreateContentMenuBuilder() {
			if (WindowsFormsSettings.PopupMenuStyle == PopupMenuStyle.RadialMenu) {
				return new WinFormsRichEditContentRadialMenuBuilder(this, new WinFormsRichEditMenuBuilderUIFactory());
			}
			return new WinFormsRichEditContentMenuBuilder(this, new WinFormsRichEditMenuBuilderUIFactory());
		}
		void OnPopupMenuCloseUp(object sender, EventArgs e) {
			RichEditPopupMenu menu = sender as RichEditPopupMenu;
			if (menu != null)
				menu.CloseUp -= OnPopupMenuCloseUp;
			isPopupMenuShown = false;
		}
		bool isPopupMenuShown;
		public bool IsImeWindowOpen() {
			IImeService imeService = GetService(typeof(IImeService)) as IImeService;
			if (imeService == null)
				return false;
			return imeService.IsActive;
		}
		public void CloseImeWindow(ImeCloseStatus closeSatus) {
			IImeService imeService = GetService(typeof(IImeService)) as IImeService;
			if (imeService == null)
				return;
			if (!imeService.IsActive)
				return;
			if (closeSatus == ImeCloseStatus.ImeCompositionCancel)
				imeService.Cancel();
			else if (closeSatus == ImeCloseStatus.ImeCompositionComplete)
				imeService.Close();
		}
		protected internal virtual Point GetPhysicalPoint(Point point) {
			int x = DocumentModel.LayoutUnitConverter.PixelsToLayoutUnits(point.X, DpiX) - ViewBounds.Left;
			int y = DocumentModel.LayoutUnitConverter.PixelsToLayoutUnits(point.Y, DpiY) - ViewBounds.Top;
			return new Point(x, y);
		}
		bool IRichEditControl.IsHyperlinkActive() {
			return this.IsHyperlinkActive();
		}
		protected internal virtual bool IsHyperlinkActive() {
			return InnerControl != null && InnerControl.IsHyperlinkModifierKeysPress() && ActualToolTipController.ActiveObject is HyperlinkInfo;
		}
		#region IToolTipControlClient Members
		bool IToolTipControlClient.ShowToolTips { get { return InnerControl.MouseHandler.State.CanShowToolTip && enableToolTips; } }
		ToolTipControlInfo IToolTipControlClient.GetObjectInfo(Point point) {
			IToolTipService service = GetService<IToolTipService>();
			return (service != null) ? service.CalculateToolTipInfo(point) : null;
		}
		bool enableToolTips = true;
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlEnableToolTips")]
#endif
		public bool EnableToolTips { get { return enableToolTips; } set { enableToolTips = value; } }
		#endregion
		protected internal virtual void OnInnerControlContentChangedPlatformSpecific(bool suppressBindingNotifications) {
		}
		protected internal virtual void OnPageBackgroundChangedPlatformSpecific() {
			OnBackColorChanged();
		}
		void IInnerRichEditControlOwner.OnActiveViewBackColorChanged() {
			OnBackColorChanged();
		}
		protected internal virtual void OnBackColorChanged() {
			ViewPainter.ResetCache();
			Redraw(false);
		}
		DialogResult IRichEditControl.ShowWarningMessage(string message) {
			return XtraMessageBox.Show(LookAndFeel, this, message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
		}
		DialogResult IRichEditControl.ShowErrorMessage(string message) {
			return XtraMessageBox.Show(LookAndFeel, this, message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
		public void ExportToPdf(string fileName) {
#if !DXPORTABLE
			ExportToPdf(fileName, new PdfCreationOptions() { Compatibility = PdfCompatibility.Pdf }, new PdfSaveOptions());
#endif
		}
		public void ExportToPdf(Stream stream) {
#if !DXPORTABLE
			ExportToPdf(stream, new PdfCreationOptions() { Compatibility = PdfCompatibility.Pdf }, new PdfSaveOptions());
#endif
		}
#if !DXPORTABLE
		public void ExportToPdf(string fileName, PdfCreationOptions creationOptions, PdfSaveOptions saveOptions) {
			using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read)) {
				ExportToPdf(stream, creationOptions, saveOptions);
			}
		}
		public void ExportToPdf(Stream stream, PdfCreationOptions creationOptions, PdfSaveOptions saveOptions) {
			if (InnerControl != null)
				InnerControl.ExportToPdf(stream, creationOptions, saveOptions);
		}
#endif
#if !DXPORTABLE
		[Obsolete("Use the 'RichEditControl.ExportToPdf(string fileName, PdfCreationOptions creationOptions, PdfSaveOptions saveOptions)' method instead.", false)]
#endif
		public void ExportToPdf(string fileName, PdfExportOptions pdfExportOptions) {
			ExecutePrinterAction(delegate() {
				Printer.Export(ExportTarget.Pdf, fileName, pdfExportOptions);
				Printer.ClearDocument();
			});
		}
#if !DXPORTABLE
		[Obsolete("Use the 'RichEditControl.ExportToPdf(Stream stream, PdfCreationOptions creationOptions, PdfSaveOptions saveOptions)' method instead.", false)]
#endif
		public void ExportToPdf(Stream stream, PdfExportOptions pdfExportOptions) {
			ExecutePrinterAction(delegate() {
				Printer.Export(ExportTarget.Pdf, stream, pdfExportOptions);
				Printer.ClearDocument();
			});
		}
		void ExecutePrinterAction(Action0 action) {
			Printer.ClearDocument();
			action();
		}
		#region IGestureClient Members
		GestureAllowArgs[] IGestureClient.CheckAllowGestures(Point point) {
			gestureHelper.PanWithGutter = true;
			List<GestureAllowArgs> result = new List<GestureAllowArgs>();
			RichEditBehaviorOptions behaviorOptions = Options.Behavior;
			if (!ClientBounds.Contains(point) || !behaviorOptions.TouchAllowed)
				return result.ToArray();
			if (behaviorOptions.ShowPopupMenuAllowed)
				result.Add(GestureAllowArgs.PressAndTap);
			if (behaviorOptions.ZoomingAllowed)
				result.Add(GestureAllowArgs.Zoom);
			Point physicalPoint = GetPhysicalPoint(point);
			RichEditHitTestResult hitTestResult = ActiveView.CalculateNearestCharacterHitTest(physicalPoint, InnerControl.DocumentModel.ActivePieceTable);
			bool isPanAllowed = true;
			if (hitTestResult != null) {
				HotZone hotZone = ActiveView.SelectionLayout.CalculateHotZone(hitTestResult, ActiveView);
				isPanAllowed = hitTestResult.FloatingObjectBox == null && hotZone == null;
			}
			if (isPanAllowed) {
				if (ActiveView.HorizontalScrollController.IsScrollPossible())
					result.Add(GestureAllowArgs.Pan);
				else
					result.Add(GestureAllowArgs.PanVertical);
			}
			else {
				gestureHelper.PanWithGutter = false;
				GestureAllowArgs args = new GestureAllowArgs();
				args.GID = GestureAllowArgs.Pan.GID;
				args.AllowID = 0; 
				args.BlockID = GestureHelper.GC_PAN_ALL;
				result.Add(args);
				result.Add(GestureAllowArgs.Rotate);
			}
			(InnerControl as IGestureStateIndicator).OnGestureBegin();
			return result.ToArray();
		}
		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		IntPtr IGestureClient.Handle { get { return this.Handle; } }
		void IGestureClient.OnEnd(GestureArgs info) { }
		void IGestureClient.OnBegin(GestureArgs info) {
			if (!Options.Behavior.TouchAllowed)
				return;
		}
		int overPanX;
		int overPanY;
		void IGestureClient.OnPan(GestureArgs info, Point delta, ref Point overPan) {
			if (!Options.Behavior.TouchAllowed)
				return;
			if (info.IsBegin) {
				overPanX = 0;
				overPanY = 0;
			}
			if (delta.IsEmpty)
				return;
			BeginUpdate();
			try {
				if (ActiveView.HorizontalScrollController.IsScrollPossible()) {
					ScrollHorizontallyByPhysicalOffsetCommand horizontalScrollingCommand = new ScrollHorizontallyByPhysicalOffsetCommand(this);
					horizontalScrollingCommand.PhysicalOffset = -ActiveView.DocumentLayout.UnitConverter.PixelsToLayoutUnits(delta.X, DpiX);
					horizontalScrollingCommand.Execute();
					if (!horizontalScrollingCommand.ExecutedSuccessfully) {
						overPanX += delta.X;
						overPan.X = overPanX;
					}
				}
				ScrollVerticallyByPhysicalOffsetCommand verticalScrollingCommand = new ScrollVerticallyByPhysicalOffsetCommand(this);
				verticalScrollingCommand.PhysicalOffset = -ActiveView.DocumentLayout.UnitConverter.PixelsToLayoutUnits(delta.Y, DpiY);
				verticalScrollingCommand.Execute();
				if (!verticalScrollingCommand.ExecutedSuccessfully) {
					overPanY += delta.Y;
					overPan.Y = overPanY;
				}
			}
			finally {
				EndUpdate();
			}
		}
		void IGestureClient.OnRotate(GestureArgs info, Point center, double degreeDelta) {
			if (!Options.Behavior.TouchAllowed)
				return;
			if (rotationController == null) {
				RichEditRectangularObjectRotateByGestureMouseHandlerState state = RectangularObjectRotationController.CreateMouseHandlerState(center, this);
				if (state == null)
					return;
				rotationController = new RectangularObjectRotationController(this, state);
				state.Start();
				state.Rotate((float)degreeDelta);
			}
			else
				rotationController.Rotate(center, degreeDelta);
		}
		void IGestureClient.OnTwoFingerTap(GestureArgs info) {
			if (!Options.Behavior.TouchAllowed)
				return;
		}
		[System.Security.SecuritySafeCritical]
		void IGestureClient.OnPressAndTap(GestureArgs info) {
			if (!Options.Behavior.TouchAllowed)
				return;
			Point screenPoint = this.PointToScreen(new Point(info.Start.X, info.Start.Y));
			Message msg = new Message();
			msg.LParam = new IntPtr((long)(screenPoint.X | (screenPoint.Y << 0x10)));
			OnWmContextMenu(ref msg);
		}
		void IGestureClient.OnZoom(GestureArgs info, Point center, double zoomDelta) {
			if (!Options.Behavior.TouchAllowed)
				return;
			if (this.Options.Behavior.ZoomingAllowed)
				ActiveView.ZoomFactor *= (float)zoomDelta;
		}
		IntPtr IGestureClient.OverPanWindowHandle { get { return GestureHelper.FindOverpanWindow(this); } }
		Point IGestureClient.PointToClient(Point p) {
			return this.PointToClient(p);
		}
		#endregion
		void ICommandAwareControl<RichEditCommandId>.Focus() {
			this.Focus();
		}
		void ICommandAwareControl<RichEditCommandId>.CommitImeContent() {
			this.CloseImeWindow(ImeCloseStatus.ImeCompositionComplete);
		}
		static Bitmap CreateSystemCaretBitmap() {
			int caretSize = 16;
			Bitmap bmp = new Bitmap(caretSize, caretSize);
			using (Graphics gr = Graphics.FromImage(bmp)) {
				using (SolidBrush brush = new SolidBrush(Color.Black)) {
					gr.FillRectangle(brush, new Rectangle(0, 0, caretSize, caretSize));
				}
			}
			return bmp;
		}
		void IRectangularObjectRotationControllerOwner.CompleteRectangularObjectRotation() {
			this.rotationController = null;
		}
		void OnEmptyDocumentCreatedPlatformSpecific() {
		}
		void OnDocumentLoadedPlatformSpecific() {
		}
		void OnLayoutUnitChanged(object sender, EventArgs e) {
			commentPadding = GetCommentsSkinPadding();
			InnerControl.ApplyNewCommentPadding();
		}
		void IMouseWheelSupport.OnMouseWheel(MouseEventArgs e) {
			OnMouseWheelCore(e);
		}
		void AutoSizeModeChanged() {
			ActiveView.OnAutoSizeModeChanged();
			((IRichEditControl)this).UpdateControlAutoSize();
		}
		bool isUpdateAutoSizeMode;
		Size lastSize;
		Size contentSize;
		void IRichEditControl.UpdateControlAutoSize() {
			if (AutoSizeMode == AutoSizeMode.None)
				return;
			if (!isUpdateAutoSizeMode && !insideResize) {
				isUpdateAutoSizeMode = true;
				Size newSize = CalcBestSize(Size.Width, Size.Height, ((AutoSizeMode & AutoSizeMode.Horizontal) == 0) || Size.Width == MaximumSize.Width && MaximumSize.Width != 0);
				contentSize = newSize;
				if (newSize != lastSize) {
					Size = newSize;
					lastSize = newSize;
				}
				isUpdateAutoSizeMode = false;
			}
		}
		Size CalcBestSize(int width, int height, bool fixedWidth) {
			Size bestSize = CalcViewBestSize(fixedWidth);
			bestSize.Width += CalculateVerticalScrollbarWidth() + borderSize.Width;
			bestSize.Height += CalculateHorizontalScrollbarHeight() + borderSize.Height;
			Size newSize = new Size();
			switch (AutoSizeMode) {
				case AutoSizeMode.Vertical:
					newSize = new Size(width, bestSize.Height);
					break;
				case AutoSizeMode.Horizontal:
					newSize = new Size(bestSize.Width, height);
					break;
				case AutoSizeMode.Both:
					newSize = new Size(bestSize.Width, bestSize.Height);
					break;
			}
			return newSize;
		}
		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {
			if ((AutoSizeMode != XtraRichEdit.AutoSizeMode.None && !isUpdateAutoSizeMode) && !insideResize) {
				Size bestSize = CalcBestSize(width, height, false);
				width = bestSize.Width;
				height = bestSize.Height;
			}
			base.SetBoundsCore(x, y, width, height, specified);
		}
	}
}
namespace DevExpress.Office.Internal {
	public class WinFormsOfficeScrollbar : IOfficeScrollbar {
		readonly ScrollBarBase scrollbar;
		public WinFormsOfficeScrollbar(ScrollBarBase scrollbar) {
			Guard.ArgumentNotNull(scrollbar, "scrollbar");
			this.scrollbar = scrollbar;
		}
		public ScrollBarBase ScrollBar { get { return scrollbar; } }
		#region IOfficeScrollbar Members
		public int Value { get { return ScrollBar.Value; } set { ScrollBar.Value = value; } }
		public int Minimum { get { return ScrollBar.Minimum; } set { ScrollBar.Minimum = value; } }
		public int Maximum { get { return ScrollBar.Maximum; } set { ScrollBar.Maximum = value; } }
		public int LargeChange { get { return ScrollBar.LargeChange; } set { ScrollBar.LargeChange = value; } }
		public int SmallChange { get { return ScrollBar.SmallChange; } set { ScrollBar.SmallChange = value; } }
		public bool Enabled { get { return ScrollBar.Enabled; } set { ScrollBar.Enabled = value; } }
		public event ScrollEventHandler Scroll { add { ScrollBar.Scroll += value; } remove { ScrollBar.Scroll -= value; } }
		public void BeginUpdate() {
			ScrollBar.BeginUpdate();
		}
		public void EndUpdate() {
			ScrollBar.EndUpdate();
		}
		#endregion
	}
}
namespace DevExpress.XtraRichEdit.Drawing {
	#region GdiPlusMeasurementAndDrawingStrategy
	public class GdiPlusMeasurementAndDrawingStrategy : GdiPlusMeasurementAndDrawingStrategyBase {
		public GdiPlusMeasurementAndDrawingStrategy(DocumentModel documentModel)
			: base(documentModel) {
		}
		public override Painter CreateDocumentPainter(IDrawingSurface surface) {
			GraphicsCacheDrawingSurface graphicsSurface = (GraphicsCacheDrawingSurface)surface;
			return new GdiPlusPainter(graphicsSurface.Cache);
		}
	}
	#endregion
	#region GdiMeasurementAndDrawingStrategy
	public class GdiMeasurementAndDrawingStrategy : GdiMeasurementAndDrawingStrategyBase {
		readonly Dictionary<OfficeImage, WeakReference> bitmapCache;
		public GdiMeasurementAndDrawingStrategy(DocumentModel documentModel)
			: base(documentModel) {
			this.bitmapCache = new Dictionary<OfficeImage, WeakReference>();
		}
		public override Painter CreateDocumentPainter(IDrawingSurface surface) {
			GraphicsCacheDrawingSurface graphicsSurface = (GraphicsCacheDrawingSurface)surface;
			RichEditGdiPainter res = new RichEditGdiPainter(graphicsSurface.Cache, (GdiBoxMeasurer)Measurer);
			res.BitmapCache = bitmapCache;
			return res;
		}
	}
	#endregion
	#region GraphicsCacheDrawingSurface
	public class GraphicsCacheDrawingSurface : IDrawingSurface {
		readonly GraphicsCache cache;
		public GraphicsCacheDrawingSurface(GraphicsCache cache) {
			Guard.ArgumentNotNull(cache, "cache");
			this.cache = cache;
		}
		public GraphicsCache Cache { get { return cache; } }
	}
	#endregion
	#region LeakSafeEventRouter
	public class LeakSafeEventRouter {
		readonly WeakReference weakControlRef;
		public LeakSafeEventRouter(RichEditControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.weakControlRef = new WeakReference(control);
		}
		public void OnCaretTimerTick(object sender, EventArgs e) {
			RichEditControl control = (RichEditControl)weakControlRef.Target;
			if (control != null)
				control.OnCaretTimerTick(sender, e);
		}
		public void OnVerticalScrollBarUpdateTimerTick(object sender, EventArgs e) {
			RichEditControl control = (RichEditControl)weakControlRef.Target;
			if (control != null)
				control.OnVerticalScrollBarUpdateTimerTick(sender, e);
		}
		public void OnFlushPendingTextInputTimerTick(object sender, EventArgs e) {
			RichEditControl control = (RichEditControl)weakControlRef.Target;
			if (control != null)
				control.OnFlushPendingTextInputTimerTick(sender, e);
		}
		public void OnFlushRotationControllerTimerTick(object sender, EventArgs e) {
			RichEditControl control = (RichEditControl)weakControlRef.Target;
			if (control != null)
				control.OnFlushRotationControllerTimerTick(sender, e);
		}
		public void OnApplicationIdle(object sender, EventArgs e) {
			RichEditControl control = (RichEditControl)weakControlRef.Target;
			if (control != null)
				control.OnApplicationIdle(sender, e);
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Painters {
	public delegate void DrawDelegate(GraphicsCache cache, ICustomMarkExporter customMarkExporter);
	#region RichEditControlPainter
	public class RichEditControlPainter : IDisposable {
		readonly RichEditControl control;
		readonly List<DrawDelegate> deferredDraws = new List<DrawDelegate>();
		public RichEditControlPainter(RichEditControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
		}
		#region Properties
		public RichEditControl Control { get { return control; } }
		public UserLookAndFeel LookAndFeel { get { return Control.LookAndFeel; } }
		protected internal virtual Caret Caret { get { return Control.Caret; } }
		protected internal virtual DragCaret DragCaret { get { return Control.DragCaret; } }
		protected internal virtual RichEditView ActiveView { get { return Control.ActiveView; } }
		#endregion
		public virtual void Draw(Graphics gr, ICustomMarkExporter customMarkExporter) {
			if (Control.InnerControl == null)
				return;
			using (GraphicsCache cache = new GraphicsCache(gr)) {
				BorderPainter borderPainter = BorderHelper.GetPainter(Control.BorderStyle, Control.LookAndFeel);
				borderPainter.DrawObject(new BorderObjectInfoArgs(cache, Control.ClientRectangle, null, ObjectState.Normal));
				if (control.CornerBounds != Rectangle.Empty)
					cache.FillRectangle(LookAndFeelHelper.GetSystemColor(Control.LookAndFeel, ((Control)Control).BackColor), control.CornerBounds);
				Control.InnerControl.BeginDocumentRendering();
				try {
					GraphicsClipState clipState = cache.ClipInfo.SaveAndSetClip(Control.ClientBounds);
					try {
						PerformRenderingInUnits(cache, customMarkExporter, RenderDocument);
						PerformRenderingInPixels(cache, customMarkExporter, RenderDocumentDecorators);
						int count = deferredDraws.Count;
						if (count > 0) {
							for (int i = 0; i < count; i++) {
								PerformRenderingInUnits(cache, null, deferredDraws[i]);
							}
							deferredDraws.Clear();
						}
					}
					finally {
						cache.ClipInfo.RestoreClipRelease(clipState);
					}
				}
				finally {
					Control.InnerControl.EndDocumentRendering();
				}
			}
		}
		public virtual void DrawCaret() {
			DrawCaret(DrawCaretCore);
		}
		public virtual void DrawDragCaret() {
			DrawCaret(DrawDragCaretCore);
		}
		void DrawCaret(DrawDelegate drawCaret) {
			IntPtr hdc = Win32.GetWindowDC(Control.Handle);
			try {
				using (Graphics gr = Graphics.FromHdc(hdc)) {
					using (GraphicsCache cache = new GraphicsCache(gr)) {
						GraphicsClipState clipState = cache.ClipInfo.SaveAndSetClip(Control.ClientBounds);
						try {
							PerformRenderingInPixels(cache, null, drawCaret);
						}
						finally {
							cache.ClipInfo.RestoreClipRelease(clipState);
						}
					}
				}
			}
			finally {
				Win32.ReleaseDC(Control.Handle, hdc);
			}
		}
		public void RegisterDeferredDraw(DrawDelegate draw) {
			deferredDraws.Add(draw);
		}
		public void DeferredDraw(PageViewInfo page, DrawAtPageDelegate drawAtPage) {
			DrawDelegate draw = delegate(GraphicsCache cache, ICustomMarkExporter customMarkExporter) {
				Control.ViewPainter.DrawAtPageCore(cache, page, drawAtPage);
			};
			RegisterDeferredDraw(draw);
		}
		public void DeferredDrawReversibleFrame(Rectangle bounds, PageViewInfo page) {
			DrawDelegate draw = delegate(GraphicsCache cache, ICustomMarkExporter customMarkExporter) {
				Control.ViewPainter.DrawReversibleFrameAtPage(cache, bounds, page);
			};
			RegisterDeferredDraw(draw);
		}
		public virtual void DrawReversibleHorizontalLine(int y, PageViewInfo page) {
			DrawDelegate draw = delegate(GraphicsCache graphicsCache, ICustomMarkExporter customMarkExporter) {
				DrawReversibleHorizontalLineCore(graphicsCache, y, page);
			};
			DrawReversibleCore(page, draw);
		}
		public virtual void DrawReversibleVerticalLine(int x, PageViewInfo page) {
			DrawDelegate draw = delegate(GraphicsCache graphicsCache, ICustomMarkExporter customMarkExporter) {
				DrawReversibleVerticalLineCore(graphicsCache, x, page);
			};
			DrawReversibleCore(page, draw);
		}
		internal virtual void DrawReversibleCore(PageViewInfo page, DrawDelegate draw) {
			IntPtr hdc = Win32.GetWindowDC(Control.Handle);
			try {
				using (Graphics gr = Graphics.FromHdc(hdc)) {
					using (GraphicsCache cache = new GraphicsCache(gr)) {
						GraphicsClipState clipState = cache.ClipInfo.SaveAndSetClip(Control.ClientBounds);
						try {
							PerformRenderingInUnits(cache, null, draw);
						}
						finally {
							cache.ClipInfo.RestoreClipRelease(clipState);
						}
					}
				}
			}
			finally {
				Win32.ReleaseDC(Control.Handle, hdc);
			}
		}
		internal virtual void PerformRenderingInUnits(GraphicsCache cache, ICustomMarkExporter customMarkExporter, DrawDelegate draw) {
			Graphics gr = cache.Graphics;
			using (GraphicsToLayoutUnitsModifier modifier = new GraphicsToLayoutUnitsModifier(gr, Control.DocumentModel.LayoutUnitConverter)) {
				Rectangle viewBounds = Control.ViewBounds;
				gr.TranslateTransform(viewBounds.Left, viewBounds.Top);
				using (HdcOriginModifier hdcOriginModifier = new HdcOriginModifier(gr, viewBounds.Location, 1.0f)) {
					draw(cache, customMarkExporter);
				}
			}
		}
		internal virtual void PerformRenderingInPixels(GraphicsCache cache, ICustomMarkExporter customMarkExporter, DrawDelegate draw) {
			cache.ClipInfo.SetClip(Control.ClientBounds);
			draw(cache, customMarkExporter);
		}
		protected internal virtual void RenderDocument(GraphicsCache cache, ICustomMarkExporter customMarkExporter) {
			ActiveView.SelectionLayout.Update();
			Control.ViewPainter.Draw(cache, customMarkExporter);
		}
		protected internal virtual void RenderDocumentDecorators(GraphicsCache cache, ICustomMarkExporter customMarkExporter) {
			Control.ViewPainter.DrawDecorators(cache);
			if (!Caret.IsHidden)
				DrawCaretCore(cache, customMarkExporter);
			if (DragCaret != null && !DragCaret.IsHidden)
				DrawDragCaretCore(cache, customMarkExporter);
		}
		protected internal virtual void DrawCaretCore(GraphicsCache cache, ICustomMarkExporter customMarkExporter) {
			DrawCaretCore(Caret, cache, customMarkExporter);
		}
		protected internal virtual void DrawDragCaretCore(GraphicsCache cache, ICustomMarkExporter customMarkExporter) {
			DrawCaretCore(DragCaret, cache, customMarkExporter);
		}
		protected internal virtual void DrawCaretCore(Caret caret, GraphicsCache cache, ICustomMarkExporter customMarkExporter) {
			if (!caret.ShouldDrawCaret(ActiveView.DocumentModel))
				return;
			CaretPosition position = caret.GetCaretPosition(ActiveView);
			if (!position.Update(DocumentLayoutDetailsLevel.Character))
				return;
			CommentCaretPosition commentCaretPosition = position as CommentCaretPosition;
			if (commentCaretPosition != null && Control.InnerControl.CommentViewInfoNoContainsCursor(commentCaretPosition))
				return;
			Rectangle viewBounds = ActiveView.CreateLogicalRectangle(position.PageViewInfo, ActiveView.Bounds);
			Padding padding = ActiveView.ActualPadding;
			viewBounds.Offset(-padding.Left, -padding.Top);
			viewBounds.Width += padding.Horizontal;
			viewBounds.Height += padding.Vertical;
			Rectangle logicalBounds = Rectangle.Intersect(viewBounds, position.CalculateCaretBounds());
			caret.Bounds = Control.GetPixelPhysicalBounds(position.PageViewInfo, logicalBounds);
			if (caret.Bounds.IsEmpty || caret.Bounds.Width < 0 || caret.Bounds.Height < 0)
				return;
			Rectangle bounds = caret.Bounds;
			bounds.Width = Math.Max(1, bounds.Width);
			bounds.Height = Math.Max(1, bounds.Height);
			caret.Bounds = bounds;
			caret.Draw(cache.Graphics);
		}
		protected internal virtual Rectangle GetActualBounds(Graphics gr, PageViewInfo page, Rectangle bounds) {
			Rectangle result = ActiveView.CreatePhysicalRectangleFast(page, bounds);
			return ActiveView.DocumentLayout.UnitConverter.LayoutUnitsToPixels(result, gr.DpiX, gr.DpiY);
		}
		protected internal virtual void DrawReversibleFrameCore(GraphicsCache cache, Rectangle bounds, PageViewInfo page) {
			Control.ViewPainter.DrawReversibleFrameAtPage(cache, bounds, page);
			if (!ActiveView.CaretPosition.Update(DocumentLayoutDetailsLevel.Character))
				return;
		}
		protected internal virtual void DrawReversibleHorizontalLineCore(GraphicsCache cache, int y, PageViewInfo page) {
			Control.ViewPainter.DrawReversibleHorizontalLineAtPage(cache, y, page);
		}
		protected internal virtual void DrawReversibleVerticalLineCore(GraphicsCache cache, int x, PageViewInfo page) {
			Control.ViewPainter.DrawReversibleVerticalLineAtPage(cache, x, page);
		}
		#region IDisposable Members
		protected virtual void Dispose(bool disposing) {
		}
		public void Dispose() {
			Dispose(true);
		}
		#endregion
	}
	#endregion
	#region EmptyRichEditControlPainter
	public class EmptyRichEditControlPainter : RichEditControlPainter {
		public EmptyRichEditControlPainter(RichEditControl control)
			: base(control) {
		}
		public override void Draw(Graphics gr, ICustomMarkExporter cutomMarkExporter) {
		}
		public override void DrawCaret() {
		}
	}
	#endregion
	#region DragCaretPainter
	public class DragCaretPainter : RichEditControlPainter {
		readonly DragCaret caret;
		public DragCaretPainter(RichEditControl control)
			: base(control) {
			this.caret = new DragCaret(control.ActiveView);
		}
		protected internal override Caret Caret { get { return caret; } }
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Native {
	class RichEditComponentPrinter : ComponentPrinter {
		PreviewFormState previewFormState;
		PreviewFormState previewRibbonFormState;
		public RichEditComponentPrinter(IPrintable component, PrintingSystemBase printingSystem)
			: base(component, printingSystem) {
			this.previewFormState = new PreviewFormState();
			this.previewRibbonFormState = new PreviewFormState();
		}
		public PreviewFormState PreviewFormState { get { return previewFormState; } }
		public PreviewFormState PreviewRibbonFormState { get { return previewRibbonFormState; } }
		public override Form ShowRibbonPreview(IWin32Window owner, object lookAndFeel) {
			var tool = new LinkPrintTool(Link);
			if (PreviewRibbonFormState.IsEmpty)
				InitForm(tool.PreviewRibbonForm, owner);
			else
				LoadViewState(tool.PreviewRibbonForm, PreviewRibbonFormState);
			tool.PreviewRibbonForm.FormClosed += PrintPreviewRibbonFormClosed;
			tool.ShowRibbonPreviewDialog(owner, (UserLookAndFeel)lookAndFeel);
			return tool.PreviewRibbonForm;
		}
		public override Form ShowPreview(IWin32Window owner, object lookAndFeel) {
			var tool = new LinkPrintTool(Link);
			if (PreviewFormState.IsEmpty)
				InitForm(tool.PreviewForm, owner);
			else
				LoadViewState(tool.PreviewForm, PreviewFormState);
			tool.PreviewForm.FormClosed += PrintPreviewFormClosed;
			tool.ShowPreviewDialog(owner, (UserLookAndFeel)lookAndFeel);
			return tool.PreviewForm;
		}
		void InitForm(Form frm, IWin32Window owner) {
			if (owner == null)
				return;
			Form parent = (Form)owner;
			frm.StartPosition = FormStartPosition.Manual;
			frm.Location = parent.Location;
			frm.Size = parent.Size;
		}
		void SaveViewState(Form form, PreviewFormState state) {
			Form parentForm = form.Owner as Form;
			if (parentForm != null && form.Location == parentForm.Location && form.Size == parentForm.Size)
				return;
			state.WindowState = form.WindowState;
			state.Location = form.Location;
			state.Size = form.Size;
		}
		void LoadViewState(Form form, PreviewFormState state) {
			form.WindowState = state.WindowState;
			form.StartPosition = FormStartPosition.Manual;
			form.Location = state.Location;
			form.Size = state.Size;
		}
		void PrintPreviewFormClosed(object sender, FormClosedEventArgs e) {
			SaveViewState((Form)sender, PreviewFormState);
			ClearDocument();
		}
		void PrintPreviewRibbonFormClosed(object sender, FormClosedEventArgs e) {
			SaveViewState((Form)sender, PreviewRibbonFormState);
			ClearDocument();
		}
	}
	public class PreviewFormState {
		public FormWindowState WindowState { get; set; }
		public Point Location { get; set; }
		public Size Size { get; set; }
		public bool IsEmpty { get { return WindowState == FormWindowState.Normal && Location == Point.Empty && Size == Size.Empty; } }
	}
	public class RightToLeftHelper {
		public static CreateParams PatchCreateParams(CreateParams cp, Control control) {
			const int WS_EX_LAYOUTRTL = 0x00400000;
			const int WS_EX_NOINHERITLAYOUT = 0x00100000;
			const int WS_EX_RTLREADING = 0x00002000;
			const int WS_EX_RIGHT = 0x00001000;
			const int WS_EX_LEFTSCROLLBAR = 0x00004000;
			if (control.RightToLeft == RightToLeft.Yes) {
				Form form = control.FindForm();
				if (form != null && form.RightToLeftLayout) {
					cp.ExStyle |= WS_EX_LAYOUTRTL | WS_EX_NOINHERITLAYOUT;
					cp.ExStyle &= ~(WS_EX_RTLREADING | WS_EX_RIGHT | WS_EX_LEFTSCROLLBAR);
				}
			}
			return cp;
		}
	}
}
