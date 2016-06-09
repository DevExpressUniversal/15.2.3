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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraScheduler.Design.PrintStyleControls;
using DevExpress.XtraScheduler.Forms;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Printing;
using DevExpress.XtraScheduler.UI;
using DevExpress.XtraEditors.Native;
using DevExpress.XtraScheduler.Internal.Diagnostics;
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupFormatTabControl.grpOptions")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupFormatTabControl.grpFont")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupFormatTabControl.grpShading")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupFormatTabControl.btnDefineNewStyles")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupFormatTabControl.btnAppointmentsFont")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupFormatTabControl.btnDateHeadingsFont")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupFormatTabControl.printStyleOptionsControl")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupFormatTabControl.grpPreview")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupFormatTabControl.lblStyleName")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupFormatTabControl.lblAppointmentsFont")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupFormatTabControl.lblDateHeadingsFont")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupFormatTabControl.pctPreview")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupFormatTabControl.cbPrintStyle")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PageSetupFormatTabControl.cbShading")]
#endregion
namespace DevExpress.XtraScheduler.Design {
	[DXToolboxItem(false), System.Runtime.InteropServices.ComVisible(false)]
	public class PageSetupFormatTabControl : DevExpress.XtraEditors.XtraUserControl, IBatchUpdateable, IBatchUpdateHandler {
		IContainer components = null;
		protected GroupControl grpOptions;
		protected GroupControl grpFont;
		protected GroupControl grpShading;
		protected SimpleButton btnDefineNewStyles;
		protected FontButtonEdit btnAppointmentsFont;
		protected FontButtonEdit btnDateHeadingsFont;
		protected PrintStyleOptionsControlBase printStyleOptionsControl;
		protected GroupControl grpPreview;
		protected DevExpress.XtraEditors.LabelControl lblStyleName;
		protected DevExpress.XtraEditors.LabelControl lblAppointmentsFont;
		protected DevExpress.XtraEditors.LabelControl lblDateHeadingsFont;
		protected PictureEdit pctPreview;
		protected ImageComboBoxEdit cbPrintStyle;
		SchedulerPrintStyleCollection editedPrintStyles;
		protected ImageComboBoxEdit cbShading;
		Hashtable htOptionsControls = new Hashtable();
		SchedulerPrintStyle currentPrintStyle;
		bool defferedCurrentPrintStyleChanged;
		ChangingEventArgs defferedCurrentPrintStyleChanging;
		bool defferedSelectionPrintStyleChanged;
		BatchUpdateHelper batchUpdateHelper;
		Utils.Menu.IDXMenuManager menuManager;
		public PageSetupFormatTabControl() {
			batchUpdateHelper = new BatchUpdateHelper(this);
			InitializeComponent();
			FillPrintColorConverterList();
			SchedulerPrintStyleCollection emptySchedulerPrintStyleCollection = new SchedulerPrintStyleCollection(false);
			AttachEditedPrintStyles(emptySchedulerPrintStyleCollection);
			SubscribeEvents();
			SubscribeEditedPrintStylesComboboxEvents();
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal PrintStyleOptionsControlBase PrintStyleOptionsControl {
			get { return printStyleOptionsControl; }
			set {
				if (value == null)
					Exceptions.ThrowArgumentNullException("value");
				if (printStyleOptionsControl == value)
					return;
				PrintStyleOptionsControlBase newOptionsControl = value;
				newOptionsControl.PrintStyle = CurrentPrintStyle;
				newOptionsControl.SetPosition(printStyleOptionsControl);
				printStyleOptionsControl = newOptionsControl;
				grpOptions.Controls.Clear();
				grpOptions.Controls.Add(printStyleOptionsControl);
			}
		}
		protected SchedulerPrintStyleCollection EditedPrintStyles { get { return editedPrintStyles; } }
		public SchedulerPrintStyle CurrentPrintStyle { get { return currentPrintStyle; } }
		public Utils.Menu.IDXMenuManager MenuManager { get { return this.menuManager; } }
		public virtual void SetMenuManager(Utils.Menu.IDXMenuManager menuManager) {
			MenuManagerUtils.SetMenuManager(Controls, menuManager);
			this.menuManager = menuManager;
		}
		public virtual void SetEditedPrintStyles(SchedulerPrintStyleCollection editedPrintStyles, SchedulerPrintStyleKind kindFutureActivePrintStyle) {
			XtraSchedulerDebug.Assert(this.editedPrintStyles != null);
			if (editedPrintStyles == null)
				Exceptions.ThrowArgumentNullException("value");
			AttachEditedPrintStyles(editedPrintStyles);
			UpdateEditedPrintStylesList(kindFutureActivePrintStyle);
			UpdateCurrentEditedPrintStyleCore();
		}
		protected internal virtual void UpdateEditedPrintStylesList(SchedulerPrintStyleKind kindFutureActivePrintStyle) {
			UnsubscribeEditedPrintStylesComboboxEvents();
			UpdateEditedPrintStylesListCore(kindFutureActivePrintStyle);
			SubscribeEditedPrintStylesComboboxEvents();
		}
		protected internal virtual void AttachEditedPrintStyles(SchedulerPrintStyleCollection editedPrintStyles) {
			XtraSchedulerDebug.Assert(editedPrintStyles != null);
			CollectionChangedEventHandler<SchedulerPrintStyle> collectionChangedEventHandler = new CollectionChangedEventHandler<SchedulerPrintStyle>(OnEditedPrintStylesCollectionChanged);
			if (this.editedPrintStyles != null)
				this.editedPrintStyles.CollectionChanged -= collectionChangedEventHandler;
			this.editedPrintStyles = editedPrintStyles;
			this.editedPrintStyles.CollectionChanged += collectionChangedEventHandler;
		}
		protected internal virtual void UpdateFromEditedPrintStyles() {
			UnsubscribeEditedPrintStylesComboboxEvents();
			SchedulerPrintStyleKind futureActivePrintStyleKind = SchedulerPrintStyleKind.Daily;
			if (EditedPrintStyles.Count > 0)
				futureActivePrintStyleKind = EditedPrintStyles[0].Kind;
			UpdateEditedPrintStylesListCore(futureActivePrintStyleKind);
			SubscribeEditedPrintStylesComboboxEvents();
		}
		protected internal virtual void UpdateEditedPrintStylesListCore(SchedulerPrintStyleKind kind) {
			ClearPrintStyles();
			FillPrintStyles();
			if (EditedPrintStyles.Count > 0)
				SetCurrentPrintStyleFromKind(kind);
		}
		protected internal virtual void OnEditedPrintStylesCollectionChanged(object sender, CollectionChangedEventArgs<SchedulerPrintStyle> e) {
			UpdateFromEditedPrintStyles();
		}
		protected internal virtual void SetCurrentPrintStyleFromKind(SchedulerPrintStyleKind kind) {
			XtraSchedulerDebug.Assert(editedPrintStyles.Count > 0);
			int index = FindIndexByKind(editedPrintStyles, kind);
			SetCurrentPrintStyleFromIndex(index);
		}
		protected internal virtual int FindIndexByKind(SchedulerPrintStyleCollection editedPrintStyles, SchedulerPrintStyleKind kind) {
			int index = 0;
			int count = editedPrintStyles.Count;
			for (index = 0; index < count; index++)
				if (editedPrintStyles[index].Kind == kind)
					break;
			if (index >= count)
				index = 0;
			return index;
		}
		protected internal virtual void SetCurrentPrintStyleFromIndex(int index) {
			int count = editedPrintStyles.Count;
			if (index >= count || index < 0)
				Exceptions.ThrowArgumentException("index", index);
			currentPrintStyle = editedPrintStyles[index];
			cbPrintStyle.EditValue = currentPrintStyle;
		}
		protected internal virtual void UnsubscribeEditedPrintStylesComboboxEvents() {
			cbPrintStyle.SelectedIndexChanged -= new EventHandler(OnCbPrintStyleSelectedIndexChanged);
			cbPrintStyle.EditValueChanging -= new ChangingEventHandler(OnCbPrintStyleEditValueChanging);
		}
		protected internal virtual void SubscribeEditedPrintStylesComboboxEvents() {
			cbPrintStyle.SelectedIndexChanged += new EventHandler(OnCbPrintStyleSelectedIndexChanged);
			cbPrintStyle.EditValueChanging += new ChangingEventHandler(OnCbPrintStyleEditValueChanging);
		}
		protected internal virtual void FillPrintStyles() {
			int count = editedPrintStyles.Count;
			cbPrintStyle.Properties.Items.BeginUpdate();
			try {
				for (int i = 0; i < count; i++) {
					SchedulerPrintStyle style = editedPrintStyles[i];
					AddPrintStyle(style);
				}
			} finally {
				cbPrintStyle.Properties.Items.EndUpdate();
			}
			cbPrintStyle.EditValue = currentPrintStyle = null;
		}
		protected internal virtual void ClearPrintStyles() {
			if (this.editedPrintStyles == null)
				return;
			int count = cbPrintStyle.Properties.Items.Count;
			for (int i = 0; i < count; i++) {
				PrintStyleOptionsControlBase styleOptionsControl = htOptionsControls[cbPrintStyle.Properties.Items[i].Value] as PrintStyleOptionsControlBase;
				XtraSchedulerDebug.Assert(styleOptionsControl != null);
				styleOptionsControl.Dispose();
			}
			htOptionsControls.Clear();
			cbPrintStyle.Properties.Items.Clear();
		}
		protected internal virtual void AddPrintStyle(SchedulerPrintStyle style) {
			PrintStyleOptionsControlBase styleOptionsControl;
			styleOptionsControl = PrintStyleOptionsControlBase.CreateOptionsControl(style.Kind);
			styleOptionsControl.PrintStyle = style;
			styleOptionsControl.SetPosition(PrintStyleOptionsControl);
			htOptionsControls[style] = styleOptionsControl;
			cbPrintStyle.Properties.Items.Add(new ImageComboBoxItem(style.DisplayName, (object)style));
		}
		protected internal virtual void SubscribeEvents() { 
			printStyleOptionsControl.PrintStyleChanged += new EventHandler(OnPrintStyleOptionsControlChanged);
			btnAppointmentsFont.SelectedFontChanged += new EventHandler(OnAppointmentsFontChanged);
			btnDateHeadingsFont.SelectedFontChanged += new EventHandler(OnDateHeadingsFontChanged);
			cbShading.EditValueChanged += new EventHandler(OnCbShadingEditValueChanged);
			cbShading.ButtonClick += new ButtonPressedEventHandler(OnCbShadingButtonClick);
		}
		protected internal virtual void UnsubscribeEvents() { 
			printStyleOptionsControl.PrintStyleChanged -= new EventHandler(OnPrintStyleOptionsControlChanged);
			btnAppointmentsFont.SelectedFontChanged -= new EventHandler(OnAppointmentsFontChanged);
			btnDateHeadingsFont.SelectedFontChanged -= new EventHandler(OnDateHeadingsFontChanged);
			cbShading.EditValueChanged -= new EventHandler(OnCbShadingEditValueChanged);
			cbShading.ButtonClick -= new ButtonPressedEventHandler(this.OnCbShadingButtonClick);
		}
		protected internal virtual void FillPrintColorConverterList() {
			string customizeShadingString = SchedulerLocalizer.GetString(SchedulerStringId.PrintPageSetupFormatTabControlCustomizeShading);
			cbShading.Properties.Items.AddRange(new ImageComboBoxItem[] {
																			new ImageComboBoxItem(PrintColorConverter.FullColor.DisplayName, PrintColorConverter.FullColor),
																			new ImageComboBoxItem(PrintColorConverter.GrayScaleColor.DisplayName, PrintColorConverter.GrayScaleColor),
																			new ImageComboBoxItem(PrintColorConverter.BlackAndWhiteColor.DisplayName, PrintColorConverter.BlackAndWhiteColor),
																			new ImageComboBoxItem(customizeShadingString, null)
																		});
		}
		#region CurrentPrintStyleChanged
		internal static readonly object onCurrentPrintStyleChanged = new object();
		public event EventHandler CurrentPrintStyleChanged {
			add { Events.AddHandler(onCurrentPrintStyleChanged, value); }
			remove { Events.RemoveHandler(onCurrentPrintStyleChanged, value); }
		}
		protected internal virtual void RaiseCurrentPrintStyleChanged() {
			EventHandler handler = (EventHandler)Events[onCurrentPrintStyleChanged];
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
		#endregion
		#region CurrentPrintStyleChanging
		internal static readonly object onCurrentPrintStyleChanging = new object();
		public event ChangingEventHandler CurrentPrintStyleChanging {
			add { Events.AddHandler(onCurrentPrintStyleChanging, value); }
			remove { Events.RemoveHandler(onCurrentPrintStyleChanging, value); }
		}
		protected internal virtual void RaiseCurrentPrintStyleChanging(ChangingEventArgs e) {
			ChangingEventHandler handler = (ChangingEventHandler)Events[onCurrentPrintStyleChanging];
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region SelectionPrintStyleChanged
		internal static readonly object onSelectionPrintStyleChanged = new object();
		public event EventHandler SelectionPrintStyleChanged {
			add { Events.AddHandler(onSelectionPrintStyleChanged, value); }
			remove { Events.RemoveHandler(onSelectionPrintStyleChanged, value); }
		}
		protected internal virtual void RaiseSelectionPrintStyleChanged() {
			EventHandler handler = (EventHandler)Events[onSelectionPrintStyleChanged];
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
		#endregion
		protected internal virtual void OnCurrentPrintStyleChanged() {
			if (IsUpdateLocked)
				defferedCurrentPrintStyleChanged = true;
			else
				RaiseCurrentPrintStyleChanged();
		}
		protected internal virtual void OnCurrentPrintStyleChanging(ChangingEventArgs e) {
			if (IsUpdateLocked)
				defferedCurrentPrintStyleChanging = e;
			else
				RaiseCurrentPrintStyleChanging(e);
		}
		protected internal virtual void OnSelectionPrintStyleChanged() {
			if (IsUpdateLocked)
				defferedSelectionPrintStyleChanged = true;
			else
				RaiseSelectionPrintStyleChanged();
		}
		#region Dispose
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (pctPreview != null && pctPreview.Image != null) {
					Image image = pctPreview.Image;
					pctPreview.Image = null;
					image.Dispose();
					pctPreview = null;
				}
				if (EditedPrintStyles != null) {
					ClearPrintStyles();
					editedPrintStyles = null;
				}
				if (components != null) {
					components.Dispose();
					components = null;
				}
			}
			base.Dispose(disposing);
		}
		#endregion
		#region Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PageSetupFormatTabControl));
			this.grpPreview = new DevExpress.XtraEditors.GroupControl();
			this.pctPreview = new DevExpress.XtraEditors.PictureEdit();
			this.grpOptions = new DevExpress.XtraEditors.GroupControl();
			this.grpFont = new DevExpress.XtraEditors.GroupControl();
			this.lblDateHeadingsFont = new DevExpress.XtraEditors.LabelControl();
			this.lblAppointmentsFont = new DevExpress.XtraEditors.LabelControl();
			this.lblStyleName = new DevExpress.XtraEditors.LabelControl();
			this.grpShading = new DevExpress.XtraEditors.GroupControl();
			this.cbShading = new DevExpress.XtraEditors.ImageComboBoxEdit();
			this.btnDefineNewStyles = new DevExpress.XtraEditors.SimpleButton();
			this.cbPrintStyle = new DevExpress.XtraEditors.ImageComboBoxEdit();
			this.printStyleOptionsControl = new DevExpress.XtraScheduler.Design.PrintStyleControls.PrintStyleOptionsControlBase();
			this.btnAppointmentsFont = new DevExpress.XtraScheduler.UI.FontButtonEdit();
			this.btnDateHeadingsFont = new DevExpress.XtraScheduler.UI.FontButtonEdit();
			((System.ComponentModel.ISupportInitialize)(this.grpPreview)).BeginInit();
			this.grpPreview.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pctPreview.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grpOptions)).BeginInit();
			this.grpOptions.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.grpFont)).BeginInit();
			this.grpFont.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.grpShading)).BeginInit();
			this.grpShading.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbShading.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbPrintStyle.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.btnAppointmentsFont.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.btnDateHeadingsFont.Properties)).BeginInit();
			this.SuspendLayout();
			this.grpPreview.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
			this.grpPreview.Controls.Add(this.pctPreview);
			resources.ApplyResources(this.grpPreview, "grpPreview");
			this.grpPreview.Name = "grpPreview";
			resources.ApplyResources(this.pctPreview, "pctPreview");
			this.pctPreview.Name = "pctPreview";
			this.pctPreview.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.Window;
			this.pctPreview.Properties.AllowFocused = false;
			this.pctPreview.Properties.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("pctPreview.Properties.Appearance.BackColor")));
			this.pctPreview.Properties.Appearance.ForeColor = ((System.Drawing.Color)(resources.GetObject("pctPreview.Properties.Appearance.ForeColor")));
			this.pctPreview.Properties.Appearance.Options.UseBackColor = true;
			this.pctPreview.Properties.Appearance.Options.UseForeColor = true;
			this.pctPreview.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pctPreview.Properties.ReadOnly = true;
			this.pctPreview.Properties.ShowMenu = false;
			this.grpOptions.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
			this.grpOptions.Controls.Add(this.printStyleOptionsControl);
			resources.ApplyResources(this.grpOptions, "grpOptions");
			this.grpOptions.Name = "grpOptions";
			this.grpFont.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
			this.grpFont.Controls.Add(this.btnAppointmentsFont);
			this.grpFont.Controls.Add(this.btnDateHeadingsFont);
			this.grpFont.Controls.Add(this.lblDateHeadingsFont);
			this.grpFont.Controls.Add(this.lblAppointmentsFont);
			resources.ApplyResources(this.grpFont, "grpFont");
			this.grpFont.Name = "grpFont";
			resources.ApplyResources(this.lblDateHeadingsFont, "lblDateHeadingsFont");
			this.lblDateHeadingsFont.Name = "lblDateHeadingsFont";
			resources.ApplyResources(this.lblAppointmentsFont, "lblAppointmentsFont");
			this.lblAppointmentsFont.Name = "lblAppointmentsFont";
			resources.ApplyResources(this.lblStyleName, "lblStyleName");
			this.lblStyleName.Name = "lblStyleName";
			this.grpShading.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
			this.grpShading.Controls.Add(this.cbShading);
			resources.ApplyResources(this.grpShading, "grpShading");
			this.grpShading.Name = "grpShading";
			resources.ApplyResources(this.cbShading, "cbShading");
			this.cbShading.Name = "cbShading";
			this.cbShading.Properties.AccessibleName = resources.GetString("cbShading.Properties.AccessibleName");
			this.cbShading.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.Text;
			this.cbShading.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbShading.Properties.Buttons")))),
			new DevExpress.XtraEditors.Controls.EditorButton()});
			resources.ApplyResources(this.btnDefineNewStyles, "btnDefineNewStyles");
			this.btnDefineNewStyles.Name = "btnDefineNewStyles";
			this.btnDefineNewStyles.Click += new System.EventHandler(this.OnBtnDefineNewStylesClick);
			resources.ApplyResources(this.cbPrintStyle, "cbPrintStyle");
			this.cbPrintStyle.Name = "cbPrintStyle";
			this.cbPrintStyle.Properties.AccessibleName = resources.GetString("cbPrintStyle.Properties.AccessibleName");
			this.cbPrintStyle.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.ComboBox;
			this.cbPrintStyle.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbPrintStyle.Properties.Buttons"))))});
			this.printStyleOptionsControl.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("printStyleOptionsControl.Appearance.BackColor")));
			this.printStyleOptionsControl.Appearance.Options.UseBackColor = true;
			resources.ApplyResources(this.printStyleOptionsControl, "printStyleOptionsControl");
			this.printStyleOptionsControl.Name = "printStyleOptionsControl";
			resources.ApplyResources(this.btnAppointmentsFont, "btnAppointmentsFont");
			this.btnAppointmentsFont.Name = "btnAppointmentsFont";
			this.btnAppointmentsFont.Properties.AccessibleName = resources.GetString("btnAppointmentsFont.Properties.AccessibleName");
			this.btnAppointmentsFont.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.Text;
			this.btnAppointmentsFont.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.btnAppointmentsFont.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.btnAppointmentsFont.SelectedFont = null;
			resources.ApplyResources(this.btnDateHeadingsFont, "btnDateHeadingsFont");
			this.btnDateHeadingsFont.Name = "btnDateHeadingsFont";
			this.btnDateHeadingsFont.Properties.AccessibleName = resources.GetString("btnDateHeadingsFont.Properties.AccessibleName");
			this.btnDateHeadingsFont.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.Text;
			this.btnDateHeadingsFont.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.btnDateHeadingsFont.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.btnDateHeadingsFont.SelectedFont = null;
			this.Controls.Add(this.cbPrintStyle);
			this.Controls.Add(this.btnDefineNewStyles);
			this.Controls.Add(this.grpShading);
			this.Controls.Add(this.lblStyleName);
			this.Controls.Add(this.grpFont);
			this.Controls.Add(this.grpOptions);
			this.Controls.Add(this.grpPreview);
			this.Name = "PageSetupFormatTabControl";
			resources.ApplyResources(this, "$this");
			((System.ComponentModel.ISupportInitialize)(this.grpPreview)).EndInit();
			this.grpPreview.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pctPreview.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grpOptions)).EndInit();
			this.grpOptions.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.grpFont)).EndInit();
			this.grpFont.ResumeLayout(false);
			this.grpFont.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.grpShading)).EndInit();
			this.grpShading.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cbShading.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbPrintStyle.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.btnAppointmentsFont.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.btnDateHeadingsFont.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		protected internal virtual void OnCbPrintStyleSelectedIndexChanged(object sender, System.EventArgs e) {
			currentPrintStyle = (SchedulerPrintStyle)cbPrintStyle.EditValue;
			UpdateCurrentEditedPrintStyleCore();
			OnSelectionPrintStyleChanged();
		}
		protected internal virtual void OnCbPrintStyleEditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e) {
			OnCurrentPrintStyleChanging(e);
		}
		protected internal virtual void UpdateCurrentEditedPrintStyle() {
			if (CurrentPrintStyle == null)
				return;
			UpdateCurrentEditedPrintStyleCore();
			OnCurrentPrintStyleChanged();
		}
		protected internal virtual void UpdateCurrentEditedPrintStyleCore() {
			XtraSchedulerDebug.Assert(CurrentPrintStyle != null);
			if (editedPrintStyles.Count < 1)
				return;
			UnsubscribeEvents();
			SchedulerPrintStyle printStyle = CurrentPrintStyle;
			XtraSchedulerDebug.Assert(htOptionsControls[printStyle] != null);
			PrintStyleOptionsControl = htOptionsControls[printStyle] as PrintStyleOptionsControlBase;
			UpdatePreviewImage();
			UpdateFonts();
			UpdateColorShading();
			SubscribeEvents();
		}
		protected internal virtual void UpdateFonts() {
			SchedulerPrintStyle printStyle = CurrentPrintStyle;
			btnAppointmentsFont.SelectedFont = printStyle.AppointmentFont;
			btnDateHeadingsFont.SelectedFont = printStyle.HeadingsFont;
		}
		protected internal virtual void UpdateColorShading() {
			SchedulerPrintStyle printStyle = CurrentPrintStyle;
			if (printStyle.ColorConverter.IsPredefinedConverter)
				cbShading.EditValue = printStyle.ColorConverter;
			else
				cbShading.SelectedIndex = cbShading.Properties.Items.Count - 1;
		}
		protected internal virtual void UpdatePreviewImage() {
			Image oldImage = pctPreview.Image;
			SchedulerPrintStyle printStyle = CurrentPrintStyle;
			pctPreview.Image = printStyle.CreateBitmap(pctPreview.Width, pctPreview.Height);
			if (oldImage != null)
				oldImage.Dispose();
		}
		protected internal virtual void OnBtnDefineNewStylesClick(object sender, System.EventArgs e) {
			CollectionChangedEventHandler<SchedulerPrintStyle> collectionChangedEventHandler = new CollectionChangedEventHandler<SchedulerPrintStyle>(OnEditedPrintStylesCollectionChanged);
			editedPrintStyles.CollectionChanged -= collectionChangedEventHandler;
			DefineNewStylesForm defineStylesForm = new DefineNewStylesForm((XtraEditors.XtraForm)this.ParentForm, EditedPrintStyles);
			defineStylesForm.SetMenuManager(MenuManager);
			defineStylesForm.ShowDialog(this.ParentForm);
			defineStylesForm.Dispose();
			editedPrintStyles.CollectionChanged += collectionChangedEventHandler;
			UpdateFromEditedPrintStyles();
			UpdateCurrentEditedPrintStyleCore();
		}
		protected internal virtual void OnPrintStyleOptionsControlChanged(object sender, System.EventArgs e) {
			UpdateCurrentEditedPrintStyle();
		}
		protected internal virtual void OnDateHeadingsFontChanged(object sender, System.EventArgs e) {
			CurrentPrintStyle.HeadingsFont = btnDateHeadingsFont.SelectedFont;
			UpdateCurrentEditedPrintStyle();
		}
		protected internal virtual void OnAppointmentsFontChanged(object sender, System.EventArgs e) {
			CurrentPrintStyle.AppointmentFont = btnAppointmentsFont.SelectedFont;
			UpdateCurrentEditedPrintStyle();
		}
		protected internal virtual void OnCbShadingEditValueChanged(object sender, System.EventArgs e) {
			if (cbShading.EditValue != null) {
				CurrentPrintStyle.ColorConverter = (PrintColorConverter)cbShading.EditValue;
				UpdateCurrentEditedPrintStyle();
			}
		}
		protected internal virtual void OnCbShadingButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e) {
			if (e.Button.Kind != ButtonPredefines.Ellipsis)
				return;
			if (cbShading.EditValue != null)
				cbShading.SelectedIndex = cbShading.Properties.Items.Count - 1;
			PrintColorConverter currentColorConverter = CurrentPrintStyle.ColorConverter;
			ShadingSetupForm form = new ShadingSetupForm((XtraEditors.XtraForm)ParentForm, currentColorConverter);
			form.ShowDialog(ParentForm);
			if (form.DialogResult == DialogResult.OK)
				CurrentPrintStyle.ColorConverter = form.ColorConverter;
			form.Dispose();
		}
		#region IBatchUpdateable implementation
		public void BeginUpdate() {
			batchUpdateHelper.BeginUpdate();
		}
		public void EndUpdate() {
			batchUpdateHelper.EndUpdate();
		}
		public void CancelUpdate() {
			batchUpdateHelper.CancelUpdate();
		}
		BatchUpdateHelper IBatchUpdateable.BatchUpdateHelper { get { return batchUpdateHelper; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsUpdateLocked { get { return batchUpdateHelper.IsUpdateLocked; } }
		#endregion
		#region IBatchUpdateHandler implementation
		void IBatchUpdateHandler.OnFirstBeginUpdate() {
			defferedCurrentPrintStyleChanged = false;
			defferedCurrentPrintStyleChanging = null;
			defferedSelectionPrintStyleChanged = false;
		}
		void IBatchUpdateHandler.OnBeginUpdate() {
		}
		void IBatchUpdateHandler.OnEndUpdate() {
		}
		void IBatchUpdateHandler.OnLastEndUpdate() {
			if (defferedCurrentPrintStyleChanged)
				RaiseCurrentPrintStyleChanged();
			if (defferedCurrentPrintStyleChanging != null)
				RaiseCurrentPrintStyleChanging(defferedCurrentPrintStyleChanging);
			if (defferedSelectionPrintStyleChanged)
				RaiseSelectionPrintStyleChanged();
		}
		void IBatchUpdateHandler.OnCancelUpdate() {
		}
		void IBatchUpdateHandler.OnLastCancelUpdate() {
		}
		#endregion
	}
}
namespace DevExpress.XtraScheduler.UI {
	[DXToolboxItem(false)]
	public class FontButtonEdit : DevExpress.XtraEditors.ButtonEdit {
		Font selectedFont;
		public FontButtonEdit() {
			SubscribeEvents();
		}
		public event EventHandler SelectedFontChanged;
		void RaiseSelectedFontChangedEvent() {
			if (SelectedFontChanged != null)
				SelectedFontChanged(this, new EventArgs());
		}
		public Font SelectedFont {
			get { return selectedFont; }
			set {
				if (Object.ReferenceEquals(selectedFont, value))
					return;
				selectedFont = value;
				UpdateSelectedFont();
				RaiseSelectedFontChangedEvent();
			}
		}
		void SubscribeEvents() {
			this.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(btnClick);
		}
		void UpdateSelectedFont() {
			int sz = (int)(SelectedFont.SizeInPoints + 0.5);
			this.Text = String.Format(SchedulerLocalizer.GetString(SchedulerStringId.PrintPageSetupFormatTabControlSizeAndFontName), sz, SelectedFont.Name);
		}
		private void btnClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e) {
			FontDialog fontDialog = new FontDialog();
			fontDialog.Font = SelectedFont;
			DialogResult result = fontDialog.ShowDialog();
			if (result == DialogResult.Cancel)
				return;
			SelectedFont = fontDialog.Font;
			fontDialog.Dispose();
		}
	}
}
