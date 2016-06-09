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
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.Helpers;
using DevExpress.Utils.Drawing;
using System.ComponentModel.Design;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Drawing;
using DevExpress.Skins;
using DevExpress.XtraEditors.Controls;
using DevExpress.Utils.Controls;
using System.Runtime.InteropServices;
using DevExpress.XtraWizard.Internal;
using DevExpress.XtraWizard.Localization;
using DevExpress.Utils.About;
namespace DevExpress.XtraWizard {
	[DXToolboxItem(true),
	Designer("DevExpress.XtraWizard.Design.WizardControlDesigner, " + AssemblyInfo.SRAssemblyWizardDesign, typeof(IDesigner)),
	 ToolboxTabName(AssemblyInfo.DXTabNavigation),
	 Description("Allows you to create a step-by-step Wizard.")
]
	public class WizardControl : Control, ISupportLookAndFeel, ISupportInitialize, IAppearanceOwner {
		#region Events
		static readonly object selectedPageChanged = new object();
		static readonly object selectedPageChanging = new object();
		static readonly object cancelClick = new object();
		static readonly object finishClick = new object();
		static readonly object nextClick = new object();
		static readonly object prevClick = new object();
		static readonly object helpClick = new object();
		static readonly object customizeButtons = new object(); 
		[Category("Pages")]
		public event WizardPageChangedEventHandler SelectedPageChanged {
			add { Events.AddHandler(selectedPageChanged, value); }
			remove { Events.RemoveHandler(selectedPageChanged, value); }
		}
		[Category("Pages")]
		public event WizardPageChangingEventHandler SelectedPageChanging {
			add { Events.AddHandler(selectedPageChanging, value); }
			remove { Events.RemoveHandler(selectedPageChanging, value); }
		}
		[Category("Command Buttons")]
		public event CancelEventHandler CancelClick {
			add { Events.AddHandler(cancelClick, value); }
			remove { Events.RemoveHandler(cancelClick, value); }
		}
		[Category("Command Buttons")]
		public event CancelEventHandler FinishClick {
			add { Events.AddHandler(finishClick, value); }
			remove { Events.RemoveHandler(finishClick, value); }
		}
		[Category("Command Buttons")]
		public event WizardCommandButtonClickEventHandler NextClick {
			add { Events.AddHandler(nextClick, value); }
			remove { Events.RemoveHandler(nextClick, value); }
		}
		[Category("Command Buttons")]
		public event WizardCommandButtonClickEventHandler PrevClick {
			add { Events.AddHandler(prevClick, value); }
			remove { Events.RemoveHandler(prevClick, value); }
		}
		[Category("Command Buttons")]
		public event WizardButtonClickEventHandler HelpClick {
			add { Events.AddHandler(helpClick, value); }
			remove { Events.RemoveHandler(helpClick, value); }
		}
		[Category("CommandButtons")]
		public event WizardCustomizeCommandButtonsEventHandler CustomizeCommandButtons {
			add { Events.AddHandler(customizeButtons, value); }
			remove { Events.RemoveHandler(customizeButtons, value); }
		}
#endregion
		WizardAppearanceCollection appearance;
		ControlUserLookAndFeel lookAndFeel;
		WizardPageCollection pages;
		string cancelText, finishText, helpText, nextText, previousText;
		bool helpVisible, useAcceptButton, useCancelButton;
		internal WizardButton btnCancel, btnHelp, btnNext, btnPrevious, btnFinish;
		internal BackButton btnBack;
		WizardPainter painter;
		WizardViewInfo viewInfo;
		BaseWizardPage selectedPage;
		internal int lockUpdate;
		Image image, headerImage, titleImage;
		int loading, imageWidth;
		bool showHeaderImage;
		ImageLayout imageLayout;
		WizardStyle wizardStyle;
		NavigationMode navMode;
		int animationInterval;
		bool allowAnimation, allowPagePadding;
		Stack<BaseWizardPage> pagesStack;
		bool parentMoving = false;
		DefaultBoolean allowAutoScaling = DefaultBoolean.Default;
		Point mouseDownPoint;
		bool allowHtmlText;
		public static void About() {
		}
		public WizardControl() {
			SetStyle(ControlStyles.SupportsTransparentBackColor | ControlConstants.DoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserMouse, true);
			this.appearance = new WizardAppearanceCollection(this);
			this.appearance.Changed += new EventHandler(OnAppearanceChanged);
			this.lookAndFeel = new ControlUserLookAndFeel(this);
			this.pages = CreatePageCollection();
			this.pagesStack = new Stack<BaseWizardPage>();
			this.wizardStyle = WizardStyle.Wizard97;
			this.navMode = NavigationMode.Sequential; 
			this.showHeaderImage = false;
			InitDefaultValues();
			this.btnPrevious = CreateButton(this.PreviousText);
			this.btnNext = CreateButton(this.NextText);
			this.btnFinish = CreateButton(this.FinishText);
			this.btnCancel = CreateButton(this.CancelText);
			this.btnHelp = CreateButton(this.HelpText);
			this.btnBack = CreateBackButton();
			SetCausesValidation();
			this.btnHelp.Visible = HelpVisible;
			this.btnPrevious.Click += new EventHandler(OnPrevButtonClick);
			this.btnNext.Click += new EventHandler(OnNextButtonClick);
			this.btnFinish.Click += new EventHandler(OnFinishButtonClick);
			this.btnCancel.Click += new EventHandler(OnCancelButtonClick);
			this.btnBack.Click += new EventHandler(OnPrevButtonClick);
			this.btnHelp.Click += new EventHandler(OnHelpButtonClick); 
			this.Pages.CollectionChanged += new CollectionChangeEventHandler(OnPagesCollectionChanged);
			this.LookAndFeel.StyleChanged += new EventHandler(OnLookAndFeelChanged);
			this.btnNext.Enabled = btnPrevious.Enabled = false;
			this.btnFinish.Visible = false;
			this.lockUpdate = this.loading = 0;
			this.animationInterval = 200;
			this.allowAnimation = this.allowPagePadding = this.useAcceptButton = this.useCancelButton = true;
			this.allowHtmlText = false;
			this.Dock = DockStyle.Fill;
			this.TabStop = false;
			this.imageWidth = Wizard97Consts.ImageWidth;
			this.imageLayout = ImageLayout.Stretch;
			this.Text = GetLocalizedDefaultString(WizardStringId.WizardTitle);
		}
		internal static string GetLocalizedDefaultString(WizardStringId id) {
			return WizardLocalizer.Active.GetLocalizedString(id);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				this.pagesStack.Clear();
				this.image = this.headerImage = null;
				if(this.parentInternal != null) this.parentInternal.HandleCreated -= new EventHandler(OnParentHandleCreated);
				if(this.selectedPage != null) this.selectedPage.MouseMove -= new MouseEventHandler(OnChildControlMouseMove); 
				this.btnPrevious.Click -= new EventHandler(OnPrevButtonClick);
				this.btnNext.Click -= new EventHandler(OnNextButtonClick);
				this.btnFinish.Click -= new EventHandler(OnFinishButtonClick);
				this.btnCancel.Click -= new EventHandler(OnCancelButtonClick);
				this.btnBack.Click -= new EventHandler(OnPrevButtonClick);
				this.btnHelp.Click -= new EventHandler(OnHelpButtonClick);
				this.appearance.Changed -= new EventHandler(OnAppearanceChanged);
				UnSubscribeButtonMouseMoveEvent();
				Pages.CollectionChanged -= new CollectionChangeEventHandler(OnPagesCollectionChanged);
				LookAndFeel.StyleChanged -= new EventHandler(OnLookAndFeelChanged);
				LookAndFeel.Dispose();
				Appearance.Dispose();
				ViewInfo.Dispose();
				this.parentInternal = null;
				this.selectedPage = null;
			}
			base.Dispose(disposing);
		}
		void UnSubscribeButtonMouseMoveEvent() {
			foreach(Control control in Controls) {
				if(control is BaseButton)
					control.MouseMove -= new MouseEventHandler(OnChildControlMouseMove);
			}
		}
		void InitDefaultValues() {
			this.cancelText = GetLocalizedDefaultString(WizardStringId.CancelText);
			this.finishText = GetLocalizedDefaultString(WizardStringId.FinishText);
			this.helpText = GetLocalizedDefaultString(WizardStringId.HelpText);
			this.nextText = GetLocalizedDefaultString(WizardStringId.NextText);
			this.previousText = GetLocalizedDefaultString(WizardStringId.PreviousText);
			this.helpVisible = false;
			this.selectedPage = null;
		}
		void SetCausesValidation() {
			this.btnCancel.CausesValidation = this.btnBack.CausesValidation = btnPrevious.CausesValidation = btnHelp.CausesValidation = false;
		}
		protected internal WizardViewInfo ViewInfo {
			get {
				if(viewInfo == null)
					viewInfo = CreateViewInfo();
				return viewInfo;
			}
		}
		protected internal WizardPainter Painter {
			get {
				if(painter == null)
					painter = CreatePainter();
				return painter;
			}
		}
		protected virtual WizardViewInfo CreateViewInfo() {
			return new WizardViewInfo(this);
		}
		protected virtual WizardPainter CreatePainter() {
			return new WizardPainter();
		}
		internal bool IsDesignMode { get { return DesignMode; } }
		protected virtual void OnPagesCollectionChanged(object sender, CollectionChangeEventArgs e) {
			switch(e.Action) {
				case CollectionChangeAction.Add:
					BaseWizardPage page = e.Element as BaseWizardPage;
					if(!Controls.Contains(page))
						Controls.Add(page);
					OnPageAdded(e.Element as BaseWizardPage);
					break;
				case CollectionChangeAction.Remove:
					OnPageRemoved(e.Element as BaseWizardPage);
					break;
			}
		}
		protected virtual void OnPageAdded(BaseWizardPage page) {
			if(DesignMode && !IsLoading)
				SelectedPage = page;
			FireChanged();
		}
		protected virtual void OnPageRemoved(BaseWizardPage page) {
			Controls.Remove(page);
			if(Pages.Count > 0)
				SelectedPage = Pages.lastRemovedPageIndex > Pages.Count - 1 ? Pages[0] : Pages[Pages.lastRemovedPageIndex];
			else SelectedPage = null;
			FireChanged();
		}
		void FireChanged() {
			if(!DesignMode || Site == null) return;
			DevExpress.Utils.Design.EditorContextHelper.FireChanged(Site, this);
		}
		BackButton CreateBackButton() {
			BackButton button = new BackButton(this);
			button.Parent = this;
			button.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			button.MouseMove += new MouseEventHandler(OnChildControlMouseMove);
			return button;
		}
		WizardButton CreateButton(string text) {
			WizardButton ret = new WizardButton(this);
			ret.SetText(text);
			ret.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			ret.MouseMove += new MouseEventHandler(OnChildControlMouseMove);
			return ret;
		}
		void OnChildControlMouseMove(object sender, MouseEventArgs e) {
			Point pt = e.Location;
			Control child = sender as Control;
			pt.Offset(child.Left, child.Top);
			OnMouseMove(new MouseEventArgs(e.Button, e.Clicks, pt.X, pt.Y, e.Delta));
		}
		#region Common Properties
		bool ShouldSerializeLookAndFeel() { return LookAndFeel.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Appearance")]
		public virtual UserLookAndFeel LookAndFeel { get { return lookAndFeel; } }
		bool ISupportLookAndFeel.IgnoreChildren { get { return false; } }
		[DefaultValue(null), Category("Appearance")]
		public Image Image {
			get { return image; }
			set {
				if(Image == value) return;
				image = value;
				Refresh();
			}
		}
		[DefaultValue(Wizard97Consts.ImageWidth), Category("Appearance")]
		public int ImageWidth {
			get { return imageWidth; }
			set {
				if(value <= 0) value = Wizard97Consts.ImageWidth;
				if(ImageWidth == value) return;
				imageWidth = value;
				LayoutChanged();
				Update();
			}
		}
		[DefaultValue(ImageLayout.Stretch), Category("Appearance")]
		public ImageLayout ImageLayout {
			get { return imageLayout; }
			set {
				if(ImageLayout == value) return;
				imageLayout = value;
				Refresh();
			}
		}
		[DefaultValue(null), Category("Appearance")]
		public Image HeaderImage {
			get { return headerImage; }
			set {
				if(HeaderImage == value) return;
				headerImage = value;
				Refresh();
			}
		}
		[DefaultValue(null), Category("Appearance")]
		public Image TitleImage {
			get { return titleImage; }
			set {
				if(TitleImage == value) return;
				titleImage = value;
				Refresh();
			}
		}
		[DefaultValue(false), Category("Appearance")]
		public bool ShowHeaderImage {
			get { return showHeaderImage; }
			set {
				if(ShowHeaderImage == value) return;
				showHeaderImage = value;
				Refresh();
			}
		}
		[DefaultValue(DefaultBoolean.Default), Category("Layout")]
		public DefaultBoolean AllowAutoScaling {
			get { return allowAutoScaling; }
			set {
				if(AllowAutoScaling == value) return;
				allowAutoScaling = value;
				LayoutChanged();
			}
		}
		protected internal virtual bool AllowAutoScalingBool {
			get {
				if(AllowAutoScaling == DefaultBoolean.Default)
					return this.FindForm() == null ? false : this.FindForm().AutoScaleMode != AutoScaleMode.None;
				return AllowAutoScaling == DefaultBoolean.True;
			}
		}
		[DefaultValue(WizardStyle.Wizard97), Category("Appearance")]
		public WizardStyle WizardStyle {
			get { return wizardStyle; }
			set {
				if(WizardStyle == value) return;
				wizardStyle = value;
				ViewInfo.CreateWizardModel();
				LayoutChanged();
			}
		}
		[DefaultValue(NavigationMode.Sequential), Category("Behavior")]
		public NavigationMode NavigationMode {
			get { return navMode; }
			set { navMode = value; }
		}
		[Localizable(true)]
		public override string Text {
			get { return base.Text; }
			set { 
				base.Text = value;
				Refresh();
			}
		}
		bool ShouldSerializeText() { return Text != GetLocalizedDefaultString(WizardStringId.WizardTitle); }
		new void ResetText() { Text = GetLocalizedDefaultString(WizardStringId.WizardTitle); }
		[DefaultValue(200)]
		public int AnimationInterval {
			get { return animationInterval; }
			set {
				if(value < 1 || value > 1000)
					throw new ArgumentException("value");
				animationInterval = value;
			}
		}
		[DefaultValue(true)]
		public bool AllowTransitionAnimation {
			get { return allowAnimation; }
			set { allowAnimation = value; }
		}
		[DefaultValue(true), Category("Appearance")]
		public bool AllowPagePadding {
			get { return allowPagePadding; }
			set {
				if(AllowPagePadding == value) return;
				allowPagePadding = value;
				if(!IsLoading) {
					LayoutChanged();
				}
			}
		}
		[DefaultValue(true), Category("Behavior")]
		public bool UseAcceptButton {
			get { return useAcceptButton; }
			set {
				if(UseAcceptButton == value) return;
				useAcceptButton = value;
				UpdateAcceptCancelButtons();
			}
		}
		[DefaultValue(true), Category("Behavior")]
		public bool UseCancelButton {
			get { return useCancelButton; }
			set {
				if(UseCancelButton == value) return;
				useCancelButton = value;
				UpdateAcceptCancelButtons();
			}
		}
		[DefaultValue(false), Category("Appearance")]
		public bool AllowHtmlText {
			get { return allowHtmlText; }
			set {
				if(allowHtmlText == value) return;
				allowHtmlText = value;
				LayoutChanged();
			}
		}
		void ResetAppearance() { Appearance.Reset(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Appearance")]
		public WizardAppearanceCollection Appearance { get { return appearance; } }
		#endregion
		#region Buttons Properties and Methods
		bool ShouldSerializeCancelText() { return CancelText != GetLocalizedDefaultString(WizardStringId.CancelText); }
		void ResetCancelText() { CancelText = GetLocalizedDefaultString(WizardStringId.CancelText); }
		[Localizable(true), Category("Appearance")]
		public string CancelText {
			get { return cancelText; }
			set {
				if(CancelText == value) return;
				cancelText = value;
				btnCancel.SetText(cancelText);
				LayoutChanged();
			}
		}
		bool ShouldSerializeHelpText() { return HelpText != GetLocalizedDefaultString(WizardStringId.HelpText); }
		void ResetHelpText() { HelpText = GetLocalizedDefaultString(WizardStringId.HelpText); }
		[Localizable(true), Category("Appearance")]
		public string HelpText {
			get { return helpText; }
			set {
				if(HelpText == value) return;
				helpText = value;
				btnHelp.SetText(helpText);
				LayoutChanged();
			}
		}
		bool ShouldSerializeNextText() { return NextText != GetLocalizedDefaultString(WizardStringId.NextText); }
		void ResetNextText() { NextText = GetLocalizedDefaultString(WizardStringId.NextText); }
		[Localizable(true), Category("Appearance")]
		public string NextText {
			get { return nextText; }
			set {
				if(NextText == value) return;
				nextText = value;
				btnNext.SetText(nextText);
				LayoutChanged();
			}
		}
		bool ShouldSerializePreviousText() { return PreviousText != GetLocalizedDefaultString(WizardStringId.PreviousText); }
		void ResetPreviousText() { PreviousText = GetLocalizedDefaultString(WizardStringId.PreviousText); }
		[Localizable(true), Category("Appearance")]
		public string PreviousText {
			get { return previousText; }
			set {
				if(PreviousText == value) return;
				previousText = value;
				btnPrevious.SetText(previousText);
				LayoutChanged();
			}
		}
		bool ShouldSerializeFinishText() { return FinishText != GetLocalizedDefaultString(WizardStringId.FinishText); }
		void ResetFinishText() { FinishText = GetLocalizedDefaultString(WizardStringId.FinishText); }
		[Localizable(true), Category("Appearance")]
		public string FinishText {
			get { return finishText; }
			set {
				if(FinishText == value) return;
				finishText = value;
				btnFinish.SetText(finishText);
				LayoutChanged();
			}
		}
		[DefaultValue(false), Category("Appearance")]
		public bool HelpVisible {
			get { return helpVisible; }
			set {
				if(HelpVisible == value) return;
				helpVisible = value;
				btnHelp.Visible = helpVisible;
				LayoutChanged();
			}
		}
		protected internal virtual void OnFinishButtonClick(object sender, EventArgs e) {
			if(DesignMode) return;
			if(!ValidatePageControls(SelectedPage)) return;
			if(SelectedPage != null) {
				if(!ProcessPageValidating(SelectedPage, Direction.Forward)) return;
				SelectedPage.RaisePageCommit(); 
			}
			if(!RaiseFinishClick()) return;
			PagesStack.Clear();
			ProcessAutoCloseModalForm(DialogResult.OK);
		}
		protected internal virtual void OnCancelButtonClick(object sender, EventArgs e) {
			if(DesignMode) return;
			if(CancelButtonCausesValidation && !ValidatePageControls(SelectedPage)) return;
			if(!RaiseCancelClick()) return;
			ProcessRollbackAll();
			ProcessAutoCloseModalForm(DialogResult.Cancel);
		}
		void ProcessAutoCloseModalForm(DialogResult result) {
			Form form = FindForm();
			if(form != null && form.Modal)
				form.DialogResult = result;
		}
		protected internal virtual void OnPrevButtonClick(object sender, EventArgs e) {
			if(PreviousButtonCausesValidation && !ValidatePageControls(SelectedPage)) return;  
			if(RaisePrevClick(SelectedPage)) return;
			SetPreviousPage();
		}
		protected internal virtual void OnNextButtonClick(object sender, EventArgs e) {
			if(!ValidatePageControls(SelectedPage)) return;
			if(RaiseNextClick(SelectedPage)) return;
			SetNextPage();
		}
		protected internal virtual void OnHelpButtonClick(object sender, EventArgs e) {
			if(HelpButtonCausesValidation && !ValidatePageControls(SelectedPage)) return;
			RaiseHelpClick(SelectedPage);
		}
		protected bool ValidatePageControls(BaseWizardPage page) {
			if(page == null || page.CausesValidation || IsDesignMode) return true;
			return page.ValidateChildren();
		}
		public void RefreshDesignButtons() {
			btnNext.Refresh();
			btnPrevious.Refresh();
			btnBack.Refresh();
		}
		internal void UpdateButtonsStates() {
			ViewInfo.UpdateButtonsState();
			ViewInfo.CustomizeButtons();
		}
		public void SetDesignButtonsCursor() {
			btnNext.Cursor = btnPrevious.Cursor = btnBack.Cursor = Cursors.Hand;
		}
		#endregion
		#region Page Properties and Methods
		[RefreshProperties(RefreshProperties.All), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int SelectedPageIndex {
			get {
				if(SelectedPage == null) return -1;
				return Pages.IndexOf(SelectedPage);
			}
			set {
				if(value < 0 && value >= Pages.Count) return;
				SelectedPage = Pages[value];
			}
		}
		[RefreshProperties(RefreshProperties.All), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BaseWizardPage SelectedPage {
			get {
				if(selectedPage == null && Pages.Count == 0) return null;
				if(selectedPage == null)
					selectedPage = Pages[0];
				return selectedPage;
			}
			set {
				if(SelectedPage == value) return;
				if(value != null && !Pages.Contains(value)) return;
				SetSelectedPageCore(SelectedPage, value, GetDirection(SelectedPage, value));
			}
		}
		protected Stack<BaseWizardPage> PagesStack { get { return pagesStack; } }
		protected bool IsStackedNavigation { get { return NavigationMode == NavigationMode.Stacked; } }
		protected virtual bool SetSelectedPageCore(BaseWizardPage prevPage, BaseWizardPage value, Direction direction) {
			if(!ProcessPageValidating(prevPage, direction)) return false;
			WizardPageChangingEventArgs args = RaiseSelectedPageChanging(prevPage, value, direction);
			if(args.Cancel) return false;
			value = args.Page;
			if(prevPage != null)
				prevPage.MouseMove -= new MouseEventHandler(OnChildControlMouseMove);
			this.selectedPage = value;
			if(SelectedPage != null)
				SelectedPage.MouseMove += new MouseEventHandler(OnChildControlMouseMove);
			UpdateSelectedPage(false);
			if(direction == Direction.Forward)
				ProcessPageCommit(prevPage);
			else
				ProcessPageRollback(value, prevPage);
			if(SelectedPage != null)
				SelectedPage.RaisePageInit();
			RaiseSelectedPageChanged(prevPage, SelectedPage, direction);
			return true;
		}
		protected virtual Color GetTransitionColor() {
			return ViewInfo.PaintAppearance.Page.BackColor;
		}
		void UpdateSelectedPage(bool lockFocus) {
			if(IsLockUpdate) return;
			TransitionHelper helper = null;
			if(CanAnimatePageTransition() && !lockFocus) {
				Rectangle contentBounds = ViewInfo.GetContentBounds();
				helper = new TransitionHelper(this, GetTransitionColor(), new Rectangle(PointToScreen(contentBounds.Location), contentBounds.Size), AnimationInterval);
				helper.BeginTransition();
			}
			ViewInfo.lockFocus = lockFocus;
			try {
				ViewInfo.UpdateSelectionPage();
			}
			finally {
				ViewInfo.lockFocus = false;
			}
			Invalidate();
			if(helper != null) {
				Update();
				helper.EndTransition();
			}
		}
		protected bool ProcessPageValidating(BaseWizardPage page, Direction direction) {
			WizardPageValidatingEventArgs e = page.RaisePageValidating(direction);
			if(!e.Valid) {
				if(!string.IsNullOrEmpty(e.ErrorText))
					ShowErrorMessage(e.ErrorText, e.ErrorIconType, GetLocalizedDefaultString(WizardStringId.CaptionError));
			}
			return e.Valid;
		}
		protected virtual void ShowErrorMessage(string errorText, MessageBoxIcon errorIconType, string caption) {
			if(LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin)
				XtraMessageBox.Show(LookAndFeel, this, errorText, caption, MessageBoxButtons.OK, errorIconType);
			else
				MessageBox.Show(this.Parent, errorText, caption, MessageBoxButtons.OK, errorIconType);
		}
		protected virtual void ProcessPageCommit(BaseWizardPage prevPage) {
			prevPage.RaisePageCommit();
			if(PagesStack.Count > 0) {
				BaseWizardPage topPage = PagesStack.Peek();
				if(topPage == prevPage) return;
			}
			PagesStack.Push(prevPage);
		}
		protected virtual void ProcessPageRollback(BaseWizardPage page, BaseWizardPage prevPage) {
			if(!IsStackedNavigation) {
				while(pagesStack.Count > 0) {
					BaseWizardPage topPage = PagesStack.Peek();
					if(topPage == page) break;
					PagesStack.Pop();
				}
			}
			prevPage.RaisePageRollback();
		}
		protected virtual void ProcessRollbackAll() {
			while(pagesStack.Count > 0) {
				BaseWizardPage page = pagesStack.Pop();
				if(page.Owner != null)
					page.RaisePageRollback();
			}
		}
		protected Direction GetDirection(BaseWizardPage oldPage, BaseWizardPage newPage) {
			int result = Comparer.Default.Compare(Pages.IndexOf(oldPage), Pages.IndexOf(newPage));
			return result < 0 ? Direction.Forward : Direction.Backward;
		}
		protected virtual void RaiseSelectedPageChanged(BaseWizardPage prevPage, BaseWizardPage page, Direction direction) {
			WizardPageChangedEventHandler handler = (WizardPageChangedEventHandler)this.Events[selectedPageChanged];
			if(handler != null) handler(this, new WizardPageChangedEventArgs(prevPage, page, direction));
		}
		protected virtual WizardPageChangingEventArgs RaiseSelectedPageChanging(BaseWizardPage oldPage, BaseWizardPage newPage, Direction direction) {
			WizardPageChangingEventHandler handler = (WizardPageChangingEventHandler)this.Events[selectedPageChanging];
			WizardPageChangingEventArgs e = new WizardPageChangingEventArgs(oldPage, newPage, direction);
			if(handler != null) handler(this, e);
			return e;
		}
		protected virtual bool RaiseCancelClick() {
			CancelEventHandler handler = (CancelEventHandler)this.Events[cancelClick];
			CancelEventArgs e = new CancelEventArgs();
			if(handler != null) handler(this, e);
			return !e.Cancel;
		}
		protected virtual bool RaiseFinishClick() {
			CancelEventHandler handler = (CancelEventHandler)this.Events[finishClick];
			CancelEventArgs e = new CancelEventArgs();
			if(handler != null) handler(this, e);
			return !e.Cancel;
		}
		protected virtual void RaiseHelpClick(BaseWizardPage page) {
			WizardButtonClickEventHandler handler = (WizardButtonClickEventHandler)this.Events[helpClick];
			if(handler != null) handler(this, new WizardButtonClickEventArgs(page));
		}
		protected virtual bool RaiseNextClick(BaseWizardPage page) {
			WizardCommandButtonClickEventHandler handler = (WizardCommandButtonClickEventHandler)this.Events[nextClick];
			WizardCommandButtonClickEventArgs e = new WizardCommandButtonClickEventArgs(page);
			if(handler != null)
				handler(this, e);
			return e.Handled;
		}
		protected virtual bool RaisePrevClick(BaseWizardPage page) {
			WizardCommandButtonClickEventHandler handler = (WizardCommandButtonClickEventHandler)this.Events[prevClick];
			WizardCommandButtonClickEventArgs e = new WizardCommandButtonClickEventArgs(page);
			if(handler != null)
				handler(this, e);
			return e.Handled;
		}
		protected internal virtual void RaiseCustomizeCommandButtons(CustomizeCommandButtonsEventArgs e) {
			WizardCustomizeCommandButtonsEventHandler handler = (WizardCustomizeCommandButtonsEventHandler)this.Events[customizeButtons];
			if(handler != null)
				handler(this, e);
		}
		[Browsable(false)]
		public bool IsWelcomePageCreated {
			get {
				foreach(BaseWizardPage page in Pages)
					if(page is WelcomeWizardPage) return true;
				return false;
			}
		}
		[Browsable(false)]
		public bool IsCompletionPageCreated {
			get {
				foreach(BaseWizardPage page in Pages)
					if(page is CompletionWizardPage) return true;
				return false;
			}
		}
		protected internal virtual BaseWizardPage GetNextPage(int currentIndex) {
			if(currentIndex == Pages.Count - 1 || currentIndex == -1) return null;
			BaseWizardPage nextPage = Pages[currentIndex + 1];
			if(nextPage.Visible) return nextPage;
			return GetNextPage(currentIndex + 1);
		}
		protected internal virtual BaseWizardPage GetPreviousPage(int currentIndex) {
			if(currentIndex < 1) return null;
			BaseWizardPage prevPage = Pages[currentIndex - 1];
			if(prevPage.Visible) return prevPage;
			return GetPreviousPage(currentIndex - 1);
		}
		public bool SetPreviousPage() {
			if(SelectedPage == null) return false;
			BaseWizardPage prevPage = null;
			if(IsStackedNavigation && PagesStack.Count > 0) 
				prevPage = (BaseWizardPage)PagesStack.Pop(); 
			else  
				prevPage = GetPreviousPage(SelectedPageIndex);
			if(prevPage == null) return false;
			if(!SetSelectedPageCore(SelectedPage, prevPage, Direction.Backward)) {
				if(IsStackedNavigation)
					PagesStack.Push(prevPage);
			}
			return true;
		}
		public bool SetNextPage() {
			if(SelectedPage == null) return false;
			BaseWizardPage nextPage = GetNextPage(SelectedPageIndex);
			if(nextPage == null) return false;
			SetSelectedPageCore(SelectedPage, nextPage, Direction.Forward);
			return true;
		}
		internal void SelectVisiblePage() {
			if(SetNextPage() || SetPreviousPage()) return;
			if(SelectedPage != null)
				SelectedPage.Visible = true;
		} 
		protected virtual bool CanAnimatePageTransition() {
			if(!AllowTransitionAnimation) return false;
			return !IsDesignMode && IsHandleCreated && NativeMethods.IsWindowVisible(this.Handle);
		}
		#endregion
		#region Hidden Properties
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Image BackgroundImage {
			get { return base.BackgroundImage; }
			set { base.BackgroundImage = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override ImageLayout BackgroundImageLayout {
			get { return base.BackgroundImageLayout; }
			set { base.BackgroundImageLayout = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override DockStyle Dock {
			get { return base.Dock; }
			set { base.Dock = DockStyle.Fill; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AnchorStyles Anchor {
			get { return base.Anchor; }
			set { }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override Color BackColor {
			get { return Color.Transparent; }
			set { }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override Color ForeColor {
			get { return base.ForeColor; }
			set { }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override Font Font {
			get { return base.Font; }
			set { }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool TabStop {
			get { return base.TabStop; }
			set { base.TabStop = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new Point Location {
			get { return base.Location; }
			set { base.Location = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new Size Size {
			get { return base.Size; }
			set { base.Size = value; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new int TabIndex {
			get { return base.TabIndex; }
			set { base.TabIndex = 0; }
		}
		#endregion
		[Category("Validation"), DefaultValue(false)]
		public bool CancelButtonCausesValidation {
			get { return btnCancel.CausesValidation; }
			set { btnCancel.CausesValidation = value; }
		}
		[Category("Validation"), DefaultValue(false)]
		public bool PreviousButtonCausesValidation {
			get { return btnPrevious.CausesValidation; }
			set { btnBack.CausesValidation = btnPrevious.CausesValidation = value; }
		}
		[Category("Validation"), DefaultValue(false)]
		public bool HelpButtonCausesValidation {
			get { return btnHelp.CausesValidation; }
			set { btnHelp.CausesValidation = value; }
		}
		[Browsable(false)]
		public bool IsLockUpdate { get { return lockUpdate != 0; } }
		public void BeginUpdate() { this.lockUpdate++; }
		public void EndUpdate() {
			if(--this.lockUpdate == 0)
				LayoutChanged();
		}
		public void LayoutChanged() {
			if(IsLockUpdate || IsLoading || !IsHandleCreated) return;
			UpdateSelectedPage(true);
			Invalidate(true);
		}
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			if(!firstPaint)
				LayoutChanged();
		}
		bool needUpdateTheme = false; 
		protected virtual void OnLookAndFeelChanged(object sender, EventArgs e) {
			this.needUpdateTheme = true;
			ViewInfo.SetAppearanceDirty();
			ViewInfo.UpdatePainters();
			ViewInfo.UpdateButtonsState();
			Invalidate(true);
		}
		void OnAppearanceChanged(object sender, EventArgs e) {
			ViewInfo.SetAppearanceDirty();
			Invalidate(true);
		}
		void UpdateTheme() {
			UpdateParentFormMargins();
			this.needUpdateTheme = false;
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			if(!firstPaint)
				LayoutChanged();
		}
		protected virtual WizardPageCollection CreatePageCollection() { return new WizardPageCollection(this); }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual WizardPageCollection Pages { get { return pages; } }
		bool firstPaint = true;
		protected override void OnPaint(PaintEventArgs e) {
			OnBeforePaint();
			Painter.DrawWizardClient(ViewInfo, e);
		}
		void OnBeforePaint() {
			if(IsLoading) return;
			if(needUpdateTheme)
				UpdateTheme();
			if(firstPaint) {
				firstPaint = false;
				OnLoaded();
			}
		}
		protected override void OnPaintBackground(PaintEventArgs pevent) {
			if(ViewInfo.IsWizardAeroStyle) {
				if(ViewInfo.IsAeroEnabled() && !IsDesignMode) 
					pevent.Graphics.SetClip(new Rectangle(0, 0, Width, (ViewInfo.Model as WizardViewInfo.WizardAeroModel).TitleBarHeight), System.Drawing.Drawing2D.CombineMode.Exclude);
			}
			base.OnPaintBackground(pevent);
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			if(e.Button != MouseButtons.Left) return;
			if(ViewInfo.IsWizardAeroStyle && ViewInfo.IsAeroEnabled()) {
				Point p = new Point(e.X, e.Y);
				Rectangle rect = (ViewInfo.Model as WizardViewInfo.WizardAeroModel).GetTitleBarBounds();
				if(!rect.Contains(p)) return;
				mouseDownPoint = p;
				parentMoving = true;
			}
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			if(parentMoving && parentInternal != null)
				parentInternal.Location = new Point(
					parentInternal.Location.X + (e.X - mouseDownPoint.X),
					parentInternal.Location.Y + (e.Y - mouseDownPoint.Y));
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			if(e.Button == MouseButtons.Left)
				parentMoving = false;
		}
		protected virtual void OnLoaded() {
			if(IsLoading) return;
			LayoutChanged();
			if(SelectedPage != null) {
				SelectedPage.MouseMove += new MouseEventHandler(OnChildControlMouseMove);
				SelectedPage.RaisePageInit();
				RaiseSelectedPageChanged(null, SelectedPage, Direction.Forward);
			}
			Invalidate(true);
		}
		internal void UpdateAcceptCancelButtons() {
			Form form = FindForm();
			if(form == null) return;
			form.AcceptButton = UseAcceptButton ? (btnFinish.Visible ? btnFinish : btnNext) : null;
			form.CancelButton = UseCancelButton ? btnCancel : null;
		}
		Form parentInternal = null;
		protected override void OnParentChanged(EventArgs e) {
			base.OnParentChanged(e);
			if(parentInternal != null) 
				parentInternal.HandleCreated -= new EventHandler(OnParentHandleCreated);
			parentInternal = Parent as Form;
			if(parentInternal != null)
				parentInternal.HandleCreated += new EventHandler(OnParentHandleCreated);
			ViewInfo.SetAppearanceDirty();
		}
		protected override void OnParentForeColorChanged(EventArgs e) {
			base.OnParentForeColorChanged(e);
			Invalidate(true);
		}
		void OnParentHandleCreated(object sender, EventArgs e) {
			this.needUpdateTheme = true;
		}
		protected void UpdateParentFormMargins() {
			if(!ViewInfo.IsWizardAeroStyle || !ViewInfo.IsAeroEnabled() || IsDesignMode) return;
			NativeMethods.Margins mrg = new NativeMethods.Margins();
			mrg.Top = (ViewInfo.Model as WizardViewInfo.WizardAeroModel).TitleBarHeight;
			NativeVista.DwmExtendFrameIntoClientArea(Parent.Handle, ref mrg);
		}
		protected override Size DefaultSize { get { return new Size(400, 300); } }
		protected override Size DefaultMinimumSize { get { return new Size(100, 100); } }
		public virtual WizardHitInfo CalcHitInfo(Point pt) {
			return ViewInfo.CalcHitInfo(pt);
		}
		#region ISupportInitialize Members
		public virtual void BeginInit() {
			loading++;
		}
		public virtual void EndInit() {
			if(loading == 0) return;
			loading--;
		}
		[Browsable(false)]
		public virtual bool IsLoading { get { return loading != 0; } }
		#endregion
		#region IAppearanceOwner Members
		bool IAppearanceOwner.IsLoading {
			get { return IsLoading; }
		}
		#endregion
		internal Size GetScaleSize(Size size) {
			if(AllowAutoScalingBool)
				return DevExpress.Utils.ScaleUtils.GetScaleSize(size);
			return size;
		}
		internal int GetScaleHeight(int height) {
			if(AllowAutoScalingBool)
				return DevExpress.Utils.ScaleUtils.GetScaleSize(new Size(0, height)).Height;
			return height;
		}
		internal int GetScaleWidth(int width) {
			if(AllowAutoScalingBool)
				return DevExpress.Utils.ScaleUtils.GetScaleSize(new Size(width, 0)).Width;
			return width;
		}
	}
	public enum WizardStyle { Wizard97, WizardAero }
	public enum Direction { Forward, Backward }
	public enum NavigationMode { Sequential, Stacked } 
}
