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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Serializing;
using DevExpress.XtraEditors.ButtonPanel;
namespace DevExpress.XtraBars.Docking2010.Views.Widget {
	[ToolboxItem(false)]
	public class Document : BaseDocument, IButtonsPanelOwner {
		IDocumentInfo infoCore;
		StackGroup parentCore;
		ButtonsPanel buttonsPanelCore;
		ButtonCollection userButtonCollectionCore;
		bool isMaximizedCore;
		public Document() { }
		public Document(IContainer container)
			: base(container) {
		}
		public Document(IDocumentProperties defaultProperties)
			: base(defaultProperties) {
		}
		protected override void OnCreate() {
			base.OnCreate();
			Width = 200;
			Height = 200;
			isMaximizedCore = false;
			allowCaptionColorBlendingCore = true;
			appearanceCaptionCore = new AppearanceObject();
			appearanceActiveCaptionCore = new AppearanceObject();
			appearanceActiveCaptionCore.Changed += OnAppearanceChanged;
			appearanceCaptionCore.Changed += OnAppearanceChanged;
			buttonsPanelCore = CreateButtonsPanel();
			SubscribeButtonPanel();
			userButtonCollectionCore = new ButtonCollection(buttonsPanelCore);
			userButtonCollectionCore.CollectionChanged += this.buttonsPanelCore.OnButtonsCollectionChanged;
			RowIndex = 0;
			ColumnIndex = 0;
			ColumnSpan = 1;
			RowSpan = 1;
		}
		protected override void OnDispose() {
			base.OnDispose();
			UnsubscribeButtonPanel();
			userButtonCollectionCore.CollectionChanged -= this.buttonsPanelCore.OnButtonsCollectionChanged;
			userButtonCollectionCore = null;
			buttonsPanelCore = null;
			infoCore = null;
			SetParent(null);
			appearanceActiveCaptionCore.Changed -= OnAppearanceChanged;
			appearanceCaptionCore.Changed -= OnAppearanceChanged;
			Ref.Dispose(ref appearanceActiveCaptionCore);
			Ref.Dispose(ref appearanceCaptionCore);
		}
		void OnAppearanceChanged(object sender, EventArgs e) {
			LayoutChanged();
		}
		protected internal ButtonsPanel ButtonsPanel {
			get { return buttonsPanelCore; }
		}
		[Browsable(false)]
		public override bool IsMaximized {
			get { return isMaximizedCore; }
		}
		protected internal override bool HasMaximizeButton() {
			return Properties.CanMaximize && Properties.CanShowMaximizeButton;
		}
		protected internal override bool CanMaximize() {
			return Properties.CanMaximize;
		}
		protected override internal void SetBoundsCore(Rectangle bounds) {
			if(Info != null) {
				Width = bounds.Width;
				Height = bounds.Height;
				Info.Bounds = bounds;
			}
		}
		protected virtual void UpdateLayout() {
			if(Manager == null || Manager.IsDepopulating) return;
			var widgetHost = Manager.GetOwnerControl() as WidgetsHost;
			if(widgetHost != null)
				widgetHost.UpdateExistAnimation();
			SetLayoutModified();
			LayoutChanged();
		}
		protected void SetLayoutModified() {
			if(Manager == null) return;
			if(Manager.View != null)
				Manager.View.SetLayoutModified();
		}
		protected override void OnControlShown() {
			base.OnControlShown();
			UpdateLayout();
		}
		protected override void OnControlHidden() {
			base.OnControlHidden();
			UpdateLayout();
		}
		protected void SubscribeButtonPanel() {
			ButtonsPanel.Changed += EmbeddedButtonPanelChanged;
		}
		protected void UnsubscribeButtonPanel() {
			ButtonsPanel.Changed -= EmbeddedButtonPanelChanged;
		}
		protected internal override bool Borderless { get { return Properties != null ? !Properties.CanShowBorders : false; } }
		void EmbeddedButtonPanelChanged(object sender, EventArgs e) {
			OnLayoutChanged();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public IDocumentInfo Info {
			get { return infoCore; }
		}
		protected override bool CalcIsVisible() {
			return base.CalcIsVisible() && IsVisibleCore();
		}
		bool IsVisibleCore() {
			if(Manager == null) return false;
			WidgetView view = Manager.View as WidgetView;
			if(view == null) return false;
			if(view.LayoutMode == LayoutMode.TableLayout)
				return RowIndex > -1 && ColumnIndex > -1;
			return true;
		}
		AppearanceObject appearanceCaptionCore;
		bool allowCaptionColorBlendingCore;
		[Category("Appearance")]
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DocumentAllowCaptionColorBlending"),
#endif
 DefaultValue(true)]
		[XtraSerializableProperty]
		public bool AllowCaptionColorBlending {
			get { return allowCaptionColorBlendingCore; }
			set { SetValue(ref allowCaptionColorBlendingCore, value); }
		}
		protected override bool CanBlendCaptionColor() {
			return AllowCaptionColorBlending;
		}
		[Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
#if !SL
	[DevExpressXtraBarsLocalizedDescription("DocumentAppearanceCaption")]
#endif
		public AppearanceObject AppearanceCaption {
			get { return appearanceCaptionCore; }
		}
		bool ShouldSerializeAppearanceCaption() {
			return ((AppearanceCaption != null) && AppearanceCaption.ShouldSerialize());
		}
		void ResetAppearanceCaption() {
			AppearanceCaption.Reset();
		}
		AppearanceObject appearanceActiveCaptionCore;
		[Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
#if !SL
	[DevExpressXtraBarsLocalizedDescription("DocumentAppearanceActiveCaption")]
#endif
		public AppearanceObject AppearanceActiveCaption {
			get { return appearanceActiveCaptionCore; }
		}
		bool ShouldSerializeAppearanceActiveCaption() {
			return ((AppearanceActiveCaption != null) && AppearanceActiveCaption.ShouldSerialize());
		}
		void ResetAppearanceActiveCaption() {
			AppearanceActiveCaption.Reset();
		}
		int rowIndexCore;
		[Category("Layout"), XtraSerializableProperty, DefaultValue(0),
#if !SL
	DevExpressXtraBarsLocalizedDescription("DocumentRowIndex")
#else
	Description("")
#endif
]
		public int RowIndex {
			get { return rowIndexCore; }
			set { SetValue(ref rowIndexCore, value, UpdateLayout); }
		}
		int columnIndexCore;
		[Category("Layout"), XtraSerializableProperty, DefaultValue(0),
#if !SL
	DevExpressXtraBarsLocalizedDescription("DocumentColumnIndex")
#else
	Description("")
#endif
]
		public int ColumnIndex {
			get { return columnIndexCore; }
			set { SetValue(ref columnIndexCore, value, UpdateLayout); }
		}
		int rowSpanCore;
		[Category("Layout"), XtraSerializableProperty, DefaultValue(1),
#if !SL
	DevExpressXtraBarsLocalizedDescription("DocumentRowSpan")
#else
	Description("")
#endif
]
		public int RowSpan {
			get { return rowSpanCore; }
			set { SetValue(ref rowSpanCore, value, UpdateLayout); }
		}
		int columnSpanCore;
		[Category("Layout"), XtraSerializableProperty, DefaultValue(1),
#if !SL
	DevExpressXtraBarsLocalizedDescription("DocumentColumnSpan")
#else
	Description("")
#endif
]
		public int ColumnSpan {
			get { return columnSpanCore; }
			set { SetValue(ref columnSpanCore, value, UpdateLayout); }
		}
		int widthCore;
		[DefaultValue(200), XtraSerializableProperty,
#if !SL
	DevExpressXtraBarsLocalizedDescription("DocumentWidth")
#else
	Description("")
#endif
]
		public int Width {
			get { return widthCore; }
			set { SetValue(ref widthCore, value, UpdateLayout); }
		}
		int heightCore;
		[DefaultValue(200), XtraSerializableProperty,
#if !SL
	DevExpressXtraBarsLocalizedDescription("DocumentHeight")
#else
	Description("")
#endif
]
		public int Height {
			get { return heightCore; }
			set { SetValue(ref heightCore, value, UpdateLayout); }
		}
		Control maximizedControlCore;
		[DefaultValue(null), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("DocumentMaximizedControl")
#else
	Description("")
#endif
]
		public Control MaximizedControl {
			get { return maximizedControlCore; }
			set {
				if(maximizedControlCore == value) return;
				if(Manager != null && Manager.View != null) {
					Manager.View.Documents.UnregisterControl(maximizedControlCore);
					Manager.View.Documents.RegisterControl(value, this);
				}
				maximizedControlCore = value;
			}
		}
		[ListBindable(false), TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Editor("DevExpress.XtraBars.Docking.Design.CustomHeaderButtonCollectionEditor, " + AssemblyInfo.SRAssemblyBarsDesignFull,
		  typeof(System.Drawing.Design.UITypeEditor)), Category("Custom Header Buttons"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("DocumentCustomHeaderButtons")
#else
	Description("")
#endif
]
		public ButtonCollection CustomHeaderButtons {
			get { return userButtonCollectionCore; }
		}
		static readonly object customButtonClick = new object();
#if !SL
	[DevExpressXtraBarsLocalizedDescription("DocumentCustomButtonClick")]
#endif
		public event ButtonEventHandler CustomButtonClick {
			add { this.Events.AddHandler(customButtonClick, value); }
			remove { this.Events.RemoveHandler(customButtonClick, value); }
		}
		static readonly object customButtonUnchecked = new object();
#if !SL
	[DevExpressXtraBarsLocalizedDescription("DocumentCustomButtonUnchecked")]
#endif
		public event ButtonEventHandler CustomButtonUnchecked {
			add { this.Events.AddHandler(customButtonUnchecked, value); }
			remove { this.Events.RemoveHandler(customButtonUnchecked, value); }
		}
		static readonly object customButtonChecked = new object();
#if !SL
	[DevExpressXtraBarsLocalizedDescription("DocumentCustomButtonChecked")]
#endif
		public event ButtonEventHandler CustomButtonChecked {
			add { this.Events.AddHandler(customButtonChecked, value); }
			remove { this.Events.RemoveHandler(customButtonChecked, value); }
		}
		static readonly object maximized = new object();
#if !SL
	[DevExpressXtraBarsLocalizedDescription("DocumentMaximized")]
#endif
		public event EventHandler Maximized {
			add { this.Events.AddHandler(maximized, value); }
			remove { this.Events.RemoveHandler(maximized, value); }
		}
		static readonly object restored = new object();
#if !SL
	[DevExpressXtraBarsLocalizedDescription("DocumentRestored")]
#endif
		public event EventHandler Restored {
			add { this.Events.AddHandler(restored, value); }
			remove { this.Events.RemoveHandler(restored, value); }
		}
		string tooltipCore;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DocumentToolTip"),
#endif
 Category("Tooltip")]
		[DefaultValue(null), Localizable(true), Editor(DevExpress.Utils.ControlConstants.MultilineStringEditor, typeof(System.Drawing.Design.UITypeEditor))]
		public virtual string ToolTip {
			get { return tooltipCore; }
			set { tooltipCore = value; }
		}
		DevExpress.Utils.SuperToolTip superTipCore;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DocumentSuperTip"),
#endif
 Category("Tooltip")]
		[Localizable(true), Editor("DevExpress.XtraEditors.Design.ToolTipContainerUITypeEditor, " + AssemblyInfo.SRAssemblyEditorsDesign, typeof(System.Drawing.Design.UITypeEditor))]
		public virtual DevExpress.Utils.SuperToolTip SuperTip {
			get { return superTipCore; }
			set { superTipCore = value; }
		}
		bool ShouldSerializeSuperTip() { return SuperTip != null && !SuperTip.IsEmpty; }
		void ResetSuperTip() { SuperTip = null; }
		string tooltipTitleCore;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DocumentTooltipTitle"),
#endif
 Category("Tooltip")]
		[Localizable(true), DefaultValue(null)]
		public string TooltipTitle {
			get { return tooltipTitleCore; }
			set { tooltipTitleCore = value; }
		}
		DevExpress.Utils.ToolTipIconType toolTipIconTypeCore = DevExpress.Utils.ToolTipIconType.None;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DocumentTooltipIconType"),
#endif
 Category("Tooltip")]
		[Localizable(true), DefaultValue(DevExpress.Utils.ToolTipIconType.None)]
		public DevExpress.Utils.ToolTipIconType TooltipIconType {
			get { return toolTipIconTypeCore; }
			set { toolTipIconTypeCore = value; }
		}
		protected internal void RaiseCustomButtonChecked(ButtonEventArgs e) {
			if(e.Button is DefaultButton) return;
			ButtonEventHandler handler = (ButtonEventHandler)this.Events[customButtonChecked];
			if(handler != null) handler(this, e);
		}
		protected internal void RaiseCustomButtonUnchecked(ButtonEventArgs e) {
			if(e.Button is DefaultButton) return;
			ButtonEventHandler handler = (ButtonEventHandler)this.Events[customButtonUnchecked];
			if(handler != null) handler(this, e);
		}
		protected internal void RaiseCustomButtonClick(ButtonEventArgs e) {
			if(e.Button is DefaultButton) return;
			ButtonEventHandler handler = (ButtonEventHandler)this.Events[customButtonClick];
			if(handler != null) handler(this, e);
		}
		protected internal void SetMaximized(bool maximized) {
			isMaximizedCore = maximized;
			if(IsMaximized)
				OnMaximized();
			else
				OnRestored();
		}
		protected internal void OnMaximized() {
			RaiseMaximized();
			var maximizedControl = MaximizedControl;
			if(maximizedControl == null){
				maximizedControl = (Manager.View as WidgetView).RaiseQueryMaximizedControl(this);
				if(maximizedControl != null) {
					(Manager.View as WidgetView).RaiseMaximizedControlLoaded(this, maximizedControl);
					IsMaximizedControlLoadedByQueryControl = true;
					MaximizedControl = maximizedControl;
				}
			}
			if(MaximizedControl != null)
				SwapControls(MaximizedControl);
		}
		protected void RaiseMaximized() {
			EventHandler handler = (EventHandler)this.Events[maximized];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		int lockReleaseDeferredLoadControl = 0;
		protected internal void ReleaseDeferredLoadMaximizedControl(WidgetView view) {
			ReleaseDeferredLoadMaximizedControl(view, true, true);
		}
		protected internal void ReleaseDeferredLoadMaximizedControl(WidgetView view, bool keepControl, bool disposeControl) {
			if(view == null || !IsMaximizedControlLoadedByQueryControl) return;
			if(IsDesignMode(view)) return;
			ReleaseDeferredLoadMaximizedControlCore(view, keepControl, disposeControl);
		}
		protected void ReleaseDeferredLoadMaximizedControlCore(WidgetView view, bool keepControl, bool disposeControl) {
			if(lockReleaseDeferredLoadControl > 0) return;
			lockReleaseDeferredLoadControl++;
			try {
				bool disposeControlResult;
				if(!(view as WidgetView).RaiseMaximizedControlReleasing(this, keepControl, disposeControl, out disposeControlResult)) return;
				using(BatchUpdate.Enter(view.Manager, true)) {
					if(!disposeControlResult)
						LockFormDisposing();
					ReleaseDeferredLoadMaximizedControlCore(view, disposeControlResult);
					if(!disposeControlResult)
						UnLockFormDisposing();
				}
			}
			finally { lockReleaseDeferredLoadControl--; }
		}
		protected void ReleaseDeferredLoadMaximizedControlCore(WidgetView view, bool disposeControl) {
			using(BatchUpdate.Enter(this, true)) {
				Form.Visible = false;
				Control control = MaximizedControl;
				if(control != Form) {
					DocumentContainer container = control.Parent as DocumentContainer;
					if(container != null)
						container.LockDocumentDisposing();
					control.Parent = null;
				}
				ReleaseMaximizedControl();
				view.OnDeferredLoadDocumentMaximizedControlReleased(control, this);
				if(disposeControl) {
					if(!control.IsDisposed)
						Ref.Dispose(ref control);
				}
				IsMaximizedControlLoadedByQueryControl = false;
				MaximizedControl = null;
			}
		}
		protected void ReleaseMaximizedControl() {
			if(MaximizedControl != null && IsMaximizedControlLoadedByQueryControl) {
				if(Control == MaximizedControl && savedControl != null) {
					SwapControls(savedControl);
					savedControl = null;
				}
			}
		}
		protected internal bool IsMaximizedControlLoadedByQueryControl { get; private set; }
		protected internal void OnRestored() {
			if(savedControl != null)
				SwapControls(savedControl);
			if(IsMaximizedControlLoadedByQueryControl)
				ReleaseDeferredLoadMaximizedControl(Manager.View as WidgetView);
			isMaximizedCore = false;
			Manager.InvokePatchActiveChildren();
			RaiseRestored();
		}
		protected virtual void RaiseRestored() {
			EventHandler handler = (EventHandler)this.Events[restored];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		Control parentContainer;
		internal void SwapControls(Control control) {
			if(controlCore == control) return;
			UnsubscribeControl();
			if(controlCore != null) {
				parentContainer = controlCore.Parent;
				savedControl = controlCore;
				controlCore.Location = new Point(-32000, -32000);
			}
			controlCore = control;
			if(control != null) {
				if(parentContainer != null) {
					controlCore.Parent = parentContainer;
					controlCore.Dock = DockStyle.Fill;
					controlCore.BringToFront();
				}
				var settings = BaseDocumentSettings.GetProvider<IBaseDocumentSettings>(control);
				if(settings != null)
					ApplySettingsCore(settings);
			}
			SubscribeControl();
			LayoutChanged();
		}
		Control savedControl;
		protected virtual ButtonsPanel CreateButtonsPanel() {
			return new ButtonsPanel(this);
		}
		protected internal void SetInfo(IDocumentInfo info) {
			if(Manager != null && Manager.IsFloating)
				Form.Size = infoCore.Bounds.Size;
			infoCore = info;
		}
		internal void SetParent(StackGroup parent) {
			if(parentCore == parent) return;
			if(parentCore != null)
				parentCore.Items.Remove(this);
			parentCore = parent;
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Layout"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("DocumentProperties")
#else
	Description("")
#endif
]
		public new IDocumentDefaultProperties Properties {
			get { return base.Properties as IDocumentDefaultProperties; }
		}
		[Browsable(false)]
		public StackGroup Parent {
			get { return parentCore; }
		}
		[Browsable(false)]
		public object Images {
			get { return Manager != null ? Manager.Images : null; }
		}
		public ObjectPainter GetPainter() {
			if(Manager != null && Manager.View != null)
				return Manager.View.IsSkinPaintStyle ? new ButtonsPanelSkinPainter(Manager.GetBarAndDockingController().LookAndFeel) as ObjectPainter :
					new ButtonPanelOffice2000Painter();
			return new ButtonPanelOffice2000Painter();
		}
		[Browsable(false)]
		public bool IsSelected {
			get { return Manager.View.ActiveDocument == this && Manager.View.IsFocused; }
		}
		protected override void OnLayoutChanged() {
			if(Manager == null) return;
			if(Control != null) {
				IFloatDocumentFormInfoOwner control = Manager.GetChild(this) as IFloatDocumentFormInfoOwner;
				if(IsFloating && IsFloatDocument) {
					(Form as FloatDocumentForm).InvalidateNC();
					return;
				}
				if(control != null) {
					control.InvalidateNC();
				}
			}
			if(Manager.View!= null)
				Manager.View.LayoutChanged();
		}
		public void AssignProperties(Document document) {
			Width = document.Width;
			Height = document.Height;
			RowSpan = document.RowSpan;
			ColumnSpan = document.ColumnSpan;
			RowIndex = document.RowIndex;
			ColumnIndex = document.ColumnIndex;
		}
		public void Invalidate() {
			if(Manager != null)
				Manager.Invalidate();
		}
		public void Invalidate(System.Drawing.Rectangle bounds) {
			if(Manager != null)
				Manager.Invalidate(bounds);
		}
		protected override IBaseDocumentDefaultProperties CreateDefaultProperties(IBaseDocumentProperties parentProperties) {
			return new DocumentDefaultProperties(parentProperties as IDocumentProperties);
		}
		protected override AppearanceObject GetDocumentCaptionAppearance() {
			return AppearanceCaption;
		}
		protected override AppearanceObject GetActiveDocumentCaptionAppearance() {
			return AppearanceActiveCaption;
		}
		protected override void OnIsActiveChanged() { }
		#region IButtonsPanelOwner Members
		bool IButtonsPanelOwner.AllowHtmlDraw {
			get { return false; }
		}
		bool IButtonsPanelOwner.AllowGlyphSkinning {
			get { return Properties.CanUseGlyphSkinning; }
		}
		DevExpress.XtraEditors.ButtonsPanelControl.ButtonsPanelControlAppearance IButtonsPanelOwner.AppearanceButton {
			get { return null; }
		}
		object IButtonsPanelOwner.ButtonBackgroundImages {
			get { return null; }
		}
		bool IButtonsPanelOwner.Enabled {
			get { return true; }
		}
		#endregion
	}
	public class EmptyDocument : Document {
		public EmptyDocument() { }
		public EmptyDocument(IContainer container)
			: base(container) {
		}
		public int Index { get; set; }
		public EmptyDocument(IDocumentProperties defaultProperties)
			: base(defaultProperties) {
		}
		protected override void UpdateLayout() {
			if(Manager == null) return;
			var widgetHost = Manager.GetOwnerControl() as WidgetsHost;
			if(widgetHost != null)
				widgetHost.UpdateLayout();
		}
		protected override bool CalcIsVisible() {
			return true;
		}
		protected internal bool Sizing { get; set; }
	}
	public class DocumentCollection : BaseDocumentCollection<Document, StackGroup> {
		public DocumentCollection(StackGroup owner)
			: base(owner) {
		}
		protected override bool CanAdd(Document element) {
			return !Owner.IsFilledUp && base.CanAdd(element);
		}
		protected override void NotifyOwnerOnInsert(int index) {
			Owner.OnInsert(index);
		}
		protected override void OnElementRemoved(Document element) {
			element.SetParent(null);
			base.OnElementRemoved(element);
		}
	}
}
