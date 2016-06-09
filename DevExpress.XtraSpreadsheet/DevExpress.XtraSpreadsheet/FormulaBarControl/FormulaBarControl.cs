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
using System.Security.Permissions;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Gesture;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.Office.Utils;
using DevExpress.Office.Services.Implementation;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Mouse;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Services;
using DevExpress.XtraSpreadsheet.Services.Implementation;
using DevExpress.XtraSpreadsheet.Forms;
using DevExpress.Utils.Controls;
using DevExpress.Spreadsheet;
using System.Collections.Generic;
using System.Drawing.Imaging;
using DevExpress.Utils.Drawing;
using DevExpress.Skins;
using DevExpress.Utils.Internal;
namespace DevExpress.XtraSpreadsheet {
	[
		System.Runtime.InteropServices.ComVisible(false),
		ToolboxBitmap(typeof(SpreadsheetFormulaBarControl), DevExpress.Utils.ControlConstants.BitmapPath + "FormulaBarControl.bmp"),
		DXToolboxItem(true),
		ToolboxTabName(AssemblyInfo.DXTabSpreadsheet),
		Designer("DevExpress.XtraSpreadsheet.Design.SpreadsheetFormulaBarDesigner," + AssemblyInfo.SRAssemblySpreadsheetDesign),
		Docking(DockingBehavior.Ask),
		Description("A control that displays the content of the active cell and allows editing."),
]
	public partial class SpreadsheetFormulaBarControl : XtraUserControl, IToolTipControlClient, IFormulaBarControllerOwner, IFormulaBarControl {
		#region Fields
		bool isDisposed;
		FormulaBarController controller;
		FormulaBarCellInplaceEditor formulaBar;
		FormulaBarButtons buttons;
		ResourceNavigator resourceNavigator;
		IToolTipService toolTipService;
		ToolTipController toolTipController;
		#endregion
		public SpreadsheetFormulaBarControl()
			: base() {
			InitializeComponent();
			Initialize();
			this.resourceNavigator = new ResourceNavigator(this);
			this.buttons.NavigatableControl = this.ResourceNavigator;
			this.buttons.Buttons.CancelEdit.Enabled = false;
			this.buttons.Buttons.EndEdit.Enabled = false;
			this.buttons.Buttons.Function.Enabled = true;
			this.toolTipService = new FormulaBarToolTipService(this);
			RegisterToolTipClientControl(ToolTipController.DefaultController);
			PrepareForTouchUI();
		}
		#region Properties
#if !SL
	[DevExpressXtraSpreadsheetLocalizedDescription("SpreadsheetFormulaBarControlController")]
#endif
		public FormulaBarController Controller { get { return controller; } }
		[
#if !SL
	DevExpressXtraSpreadsheetLocalizedDescription("SpreadsheetFormulaBarControlSpreadsheetControl"),
#endif
 DefaultValue(null)]
		public ISpreadsheetControl SpreadsheetControl {
			get { return Controller != null ? Controller.SpreadsheetControl : null; }
			set {
				if (Object.ReferenceEquals(SpreadsheetControl, value))
					return;
				DetachFromSpreadsheetControl();
				Controller.SpreadsheetControl = value;
				formulaBar.SpreadsheetControl = value;
				AttachToSpreadsheetControl();
			}
		}
		ICellInplaceEditor IFormulaBarControl.InplaceEditor { get { return formulaBar; } }
		internal bool InnerIsDisposed { get { return isDisposed; } }
		protected internal ResourceNavigator ResourceNavigator { get { return resourceNavigator; } }
		[
#if !SL
	DevExpressXtraSpreadsheetLocalizedDescription("SpreadsheetFormulaBarControlExpanded"),
#endif
 Category(CategoryName.Appearance), DefaultValue(false)]
		public bool Expanded {
			get { return formulaBar != null ? formulaBar.IsExpand : false; }
			set { formulaBar.IsExpand = value; }
		}
		bool IFormulaBarControllerOwner.EditMode { get { return formulaBar.Registered; } }
		internal ToolTipController ActualToolTipController { get { return toolTipController != null ? toolTipController : ToolTipController.DefaultController; } }
		#region ToolTipController
		internal ToolTipController ToolTipController {
			get { return toolTipController; }
			set {
				if (value == ToolTipController.DefaultController)
					toolTipController = null;
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
		#endregion
		void PrepareForTouchUI() {
			if (LookAndFeel.GetTouchUI())
				Scale(TouchUIAdapter.GetSize(LookAndFeel));
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			int minHeight = formulaBar.MinHeight;
			ChangeParentHeight(minHeight);
			ChangeParentMinHeight(minHeight);
		}
		protected override void OnEnter(EventArgs e) {
			RaiseBeforeEnter(e);
			base.OnEnter(e);
		}
		void DetachFromSpreadsheetControl() {
			if (SpreadsheetControl == null)
				return;
			SpreadsheetControl.RemoveService(typeof(IFormulaBarControl));
			SpreadsheetControl control = SpreadsheetControl as SpreadsheetControl;
			if (control != null) {
				control.EnabledChanged -= OnSpreadsheetControlEnabledChanged;
			}
		}
		void AttachToSpreadsheetControl() {
			if (SpreadsheetControl == null)
				return;
			SpreadsheetControl.AddService(typeof(IFormulaBarControl), this);
			SpreadsheetControl control = SpreadsheetControl as SpreadsheetControl;
			if (control != null) {
				control.EnabledChanged += OnSpreadsheetControlEnabledChanged;
			}
		}
		void OnSpreadsheetControlEnabledChanged(object sender, EventArgs e) {
			SpreadsheetControl control = SpreadsheetControl as SpreadsheetControl;
			if (control != null)
				this.Enabled = control.Enabled;
		}
		protected internal virtual FormulaBarController CreateController() {
			return new FormulaBarController(this);
		}
		void Initialize() {
			controller = CreateController();
			SubscribeControlEvents();
			SubscribeControllerEvents();
		}
		void SubscribeControllerEvents() {
			Controller.SelectionChanged += OnControllerSelectionChanged;
		}
		void UnsubscribeControllerEvents() {
			Controller.SelectionChanged -= OnControllerSelectionChanged;
		}
		protected internal virtual void RegisterToolTipClientControl(ToolTipController controller) {
			controller.AddClientControl(this);
		}
		protected internal virtual void UnregisterToolTipClientControl(ToolTipController controller) {
			controller.RemoveClientControl(this);
		}
		protected internal virtual void SubscribeToolTipControllerEvents(ToolTipController controller) {
			controller.Disposed += OnToolTipControllerDisposed;
		}
		protected internal virtual void UnsubscribeToolTipControllerEvents(ToolTipController controller) {
			controller.Disposed -= OnToolTipControllerDisposed;
		}
		protected internal virtual void OnToolTipControllerDisposed(object sender, EventArgs e) {
			ToolTipController = null;
		}
		void OnControllerSelectionChanged(object sender, EventArgs e) {
			UnsubscribeControlEvents();
			try {
				ChangeFormulaBarText();
			}
			finally {
				SubscribeControlEvents();
			}
		}
		void SubscribeControlEvents() {
			Resize += OnResize;
			formulaBar.GotFocus += OnFormulaBarGotFocus;
			formulaBar.KeyDown += OnFormulaBarKeyDown;
			formulaBar.Rollback += OnFormulaBarRollback;
			formulaBar.ActivateEditor += OnActivateEditor;
			formulaBar.DeactivateEditor += OnDeactivateEditor;
			formulaBar.Collapse += OnFormulaBarCollapse;
			formulaBar.Expand += OnFormulaBarExpand;
			if (Controller != null)
				formulaBar.SelectionChanged += Controller.OnOwnerSelectionChanged;
		}
		void UnsubscribeControlEvents() {
			Resize -= OnResize;
			formulaBar.GotFocus -= OnFormulaBarGotFocus;
			formulaBar.KeyDown -= OnFormulaBarKeyDown;
			formulaBar.Rollback -= OnFormulaBarRollback;
			formulaBar.ActivateEditor -= OnActivateEditor;
			formulaBar.DeactivateEditor -= OnDeactivateEditor;
			formulaBar.Collapse -= OnFormulaBarCollapse;
			formulaBar.Expand -= OnFormulaBarExpand;
			if (Controller != null)
				formulaBar.SelectionChanged -= Controller.OnOwnerSelectionChanged;
		}
		void OnResize(object sender, EventArgs e) {
			formulaBar.Height = Height;
		}
		void OnFormulaBarCollapse(object sender, EventArgs e) {
			ChangeHeight(formulaBar.MinHeight);
		}
		void OnFormulaBarExpand(object sender, EventArgs e) {
			ChangeHeight(formulaBar.MaxHeight);
		}
		void ChangeHeight(int height) {
			UnsubscribeControlEvents();
			try {
				this.formulaBar.Height = height;
				this.Height = height;
				ChangeParentHeight(height);
			}
			finally {
				SubscribeControlEvents();
			}
		}
		void ChangeParentHeight(int height) {
			SplitContainerControl spliterContainer = GetParent();
			if (spliterContainer != null)
				spliterContainer.Height = height;
		}
		void ChangeParentMinHeight(int minHeight) {
			SplitContainerControl spliterContainer = GetParent();
			if (spliterContainer != null)
				spliterContainer.MinimumSize = new Size(spliterContainer.MinimumSize.Width, minHeight);
		}
		SplitContainerControl GetParent() {
			SplitGroupPanel parent = this.Parent as SplitGroupPanel;
			if (parent == null)
				return null;
			return parent.Parent as SplitContainerControl;
		}
		void ChangeFormulaBarText() {
			formulaBar.Text = Controller.OwnersText;
		}
		void OnFormulaBarCloseInplaceEditor(object sender, EventArgs e) {
			buttons.Buttons.CancelEdit.Enabled = false;
			buttons.Buttons.EndEdit.Enabled = false;
		}
		void OnFormulaBarOpenInplaceEditor(object sender, EventArgs e) {
			buttons.Buttons.CancelEdit.Enabled = true;
			buttons.Buttons.EndEdit.Enabled = true;
		}
		void OnActivateEditor(object sender, EventArgs e) {
			SetButtonsEnabled(true);
		}
		void OnDeactivateEditor(object sender, EventArgs e) {
			SetButtonsEnabled(false);
		}
		void SetButtonsEnabled(bool enabled) {
			FormulaBarControlButtons fbButtons = buttons.Buttons;
			fbButtons.CancelEdit.Enabled = enabled;
			fbButtons.EndEdit.Enabled = enabled;
		}
		void OnFormulaBarRollback(object sender, EventArgs e) {
			RaiseRollback();
		}
		void OnFormulaBarKeyDown(object sender, KeyEventArgs e) {
			RaiseKeyDown(e.KeyData);
		}
		void OnFormulaBarGotFocus(object sender, EventArgs e) {
			OnGotFocus(e);
			if (SpreadsheetControl != null && SpreadsheetControl.InnerControl != null) {
				if (!SpreadsheetControl.InnerControl.IsInplaceEditorActive) {
					SpreadsheetControl.Focus();
				}
			}
		}
		ToolTipControlInfo IToolTipControlClient.GetObjectInfo(Point point) {
			return toolTipService.CalculateToolTipInfo(point);
		}
		bool IToolTipControlClient.ShowToolTips {
			get { return true; }
		}
		#region IDisposable implementation
		protected override void Dispose(bool disposing) {
			try {
				base.Dispose(disposing);
				if (disposing) {
					DisposeCore();
				}
			}
			finally {
				this.isDisposed = true;
			}
		}
		protected internal virtual void DisposeCore() {
			lock (this) {
				if (!IsDisposed)
					RaiseBeforeDispose();
				DisposeFormulaBarControl();
				DisposeController();
				DisposeResourceNavigator();
				DisposeToolTipService();
				DisposeFormulaBarInplaceEditor();
			}
		}
		void DisposeFormulaBarInplaceEditor() {
			if (formulaBar != null) {
				UnsubscribeControlEvents();
				formulaBar.Dispose();
				formulaBar = null;
			}
		}
		void DisposeToolTipService() {
			if (toolTipController != null) {
				ToolTipController = null;
				UnsubscribeToolTipControllerEvents(ToolTipController.DefaultController);
				UnregisterToolTipClientControl(ToolTipController.DefaultController);
			}
		}
		void DisposeResourceNavigator() {
			if (ResourceNavigator != null) {
				resourceNavigator.Dispose();
				resourceNavigator = null;
			}
		}
		void DisposeFormulaBarControl() {
			if (SpreadsheetControl != null) {
				SpreadsheetControl.RemoveService(typeof(IFormulaBarControl));
			}
		}
		void DisposeController() {
			if (Controller != null) {
				UnsubscribeControllerEvents();
				controller.Dispose();
				controller = null;
			}
		}
		#endregion
		#region Events
		#region BeforeDispose
#if !SL && !WPF
		static readonly object onBeforeDispose = new object();
#if !SL
	[DevExpressXtraSpreadsheetLocalizedDescription("SpreadsheetFormulaBarControlBeforeDispose")]
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
#else
		public event EventHandler BeforeDispose { add { } remove { }
		}
#endif
		#endregion
		#endregion
		#region designer
		private void InitializeComponent() {
			this.buttons = new DevExpress.XtraSpreadsheet.FormulaBarButtons();
			this.formulaBar = new DevExpress.XtraSpreadsheet.FormulaBarCellInplaceEditor();
			this.SuspendLayout();
			this.buttons.Buttons.CancelEdit.Hint = "Cancel";
			this.buttons.Buttons.EndEdit.Hint = "Enter";
			this.buttons.Buttons.Function.Hint = "Insert Function";
			this.buttons.Location = new System.Drawing.Point(0, 0);
			this.buttons.MinimumSize = new System.Drawing.Size(0, 20);
			this.buttons.Name = "buttons";
			this.buttons.ShowToolTips = true;
			this.buttons.Size = new System.Drawing.Size(60, 20);
			this.buttons.TabIndex = 0;
			this.buttons.Text = "controlNavigator1";
			this.formulaBar.ActiveViewType = DevExpress.XtraRichEdit.RichEditViewType.Simple;
			this.formulaBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
			| System.Windows.Forms.AnchorStyles.Right)));
			this.formulaBar.EnableToolTips = true;
			this.formulaBar.IsExpand = false;
			this.formulaBar.Location = new System.Drawing.Point(66, 0);
			this.formulaBar.MinimumSize = new System.Drawing.Size(0, 20);
			this.formulaBar.Name = "formulaBar";
			this.formulaBar.Options.CopyPaste.MaintainDocumentSectionSettings = false;
			this.formulaBar.Options.Fields.UpdateDocVariablesBeforeCopy = true;
			this.formulaBar.Options.Fields.UpdateDocVariablesBeforePrint = true;
			this.formulaBar.Options.Fields.UseCurrentCultureDateTimeFormat = false;
			this.formulaBar.Options.MailMerge.KeepLastParagraph = false;
			this.formulaBar.Registered = false;
			this.formulaBar.Size = new System.Drawing.Size(680, 20);
			this.formulaBar.SpreadsheetControl = null;
			this.formulaBar.TabIndex = 1;
			this.Controls.Add(this.buttons);
			this.Controls.Add(this.formulaBar);
			this.MinimumSize = new System.Drawing.Size(0, 20);
			this.Name = "SpreadsheetFormulaBarControl";
			this.Size = new System.Drawing.Size(746, 20);
			this.ResumeLayout(false);
		}
		#endregion
	}
}
