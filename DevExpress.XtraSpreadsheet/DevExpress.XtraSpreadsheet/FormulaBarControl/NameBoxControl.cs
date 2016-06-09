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
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Services.Implementation;
using DevExpress.XtraEditors.Repository;
using DevExpress.Utils.Design;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Popup;
using DevExpress.Utils.Internal;
namespace DevExpress.XtraSpreadsheet {
	#region SpreadsheetNameBoxControl
	[
	System.Runtime.InteropServices.ComVisible(false),
		ToolboxBitmap(typeof(SpreadsheetNameBoxControl), DevExpress.Utils.ControlConstants.BitmapPath + "NameBoxControl.bmp"),
	DXToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabSpreadsheet),
	Designer("DevExpress.XtraSpreadsheet.Design.SpreadsheetNameBoxDesigner," + AssemblyInfo.SRAssemblySpreadsheetDesign),
	Docking(DockingBehavior.Ask),
	Description("A control that displays a name of the selected object, allows creating defined names and can be used for navigation by references and names."),
]
	public partial class SpreadsheetNameBoxControl : ComboBoxEdit, INameBoxControllerOwner, INameBoxControl {
		#region Fields
		NameBoxController controller;
		DevExpress.XtraSpreadsheet.SpreadsheetControl winSpreadsheetControl;
		bool isDisposed;
		NameBoxToolTipService toolTipService;
		DocumentModel workbook;
		bool selectionMode;
		DefaultBoolean readOnly;
		#endregion
		static SpreadsheetNameBoxControl() { RepositoryItemNameBox.RegisterSpreadsheetNameBox(); }
		public SpreadsheetNameBoxControl() {
			InitializeComponent();
			Initialize();
			this.Properties.AutoComplete = false;
			this.toolTipService = new NameBoxToolTipService(this);
			this.readOnly = DefaultBoolean.Default;
		}
		#region Properties
#if !SL
	[DevExpressXtraSpreadsheetLocalizedDescription("SpreadsheetNameBoxControlEditorTypeName")]
#endif
		public override string EditorTypeName { get { return RepositoryItemNameBox.SpreadsheetNameBoxControlName; } }
		[
#if !SL
	DevExpressXtraSpreadsheetLocalizedDescription("SpreadsheetNameBoxControlProperties"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new RepositoryItemNameBox Properties { get { return base.Properties as RepositoryItemNameBox; } }
		protected internal NameBoxController Controller { get { return controller; } }
		[
#if !SL
	DevExpressXtraSpreadsheetLocalizedDescription("SpreadsheetNameBoxControlSpreadsheetControl"),
#endif
 DefaultValue(null)]
		public ISpreadsheetControl SpreadsheetControl {
			get { return Controller.SpreadsheetControl; }
			set {
				if (Object.ReferenceEquals(SpreadsheetControl, value))
					return;
				if (SpreadsheetControl != null)
					SpreadsheetControl.RemoveService(typeof(INameBoxControl));
				if (value != null)
					value.AddService(typeof(INameBoxControl), this);
				Controller.SpreadsheetControl = value;
				SubscribeSpreadsheetControlEvents();
			}
		}
		DevExpress.XtraSpreadsheet.SpreadsheetControl WinSpreadsheetControl {
			get {
				if (winSpreadsheetControl == null)
					winSpreadsheetControl = SpreadsheetControl as DevExpress.XtraSpreadsheet.SpreadsheetControl;
				return winSpreadsheetControl;
			}
		}
		bool INameBoxControllerOwner.IsEnabled { get { return Enabled; } set { Enabled = value; } }
		internal bool InnerIsDisposed { get { return isDisposed; } }
		protected override bool CanShowPopup { get { return true; } }
		protected internal DocumentModel Workbook {
			get {
				if (workbook == null && SpreadsheetControl != null)
					workbook = SpreadsheetControl.InnerControl.DocumentModel;
				return workbook;
			}
		}
		bool INameBoxControllerOwner.SelectionMode { get { return selectionMode; } set { selectionMode = value; } }
		bool INameBoxControl.SelectionMode {
			get { return selectionMode; }
			set {
				if (selectionMode != value)
					selectionMode = value;
				if (!selectionMode)
					Controller.RefreshSelection();
			}
		}
		protected override Size DefaultSize { get { return new Size(145, 20); } }
		public new DefaultBoolean ReadOnly {
			get { return readOnly; }
			set {
				if (readOnly == value)
					return;
				readOnly = value;
				Properties.ReadOnly = value.ToBoolean(false);
				RaiseReadOnlyChanged();
			}
		}
		#endregion
		#region Events
		#region ReadOnlyChanged
		EventHandler onReadOnlyChanged;
#if !SL
	[DevExpressXtraSpreadsheetLocalizedDescription("SpreadsheetNameBoxControlReadOnlyChanged")]
#endif
		public event EventHandler ReadOnlyChanged { add { onReadOnlyChanged += value; } remove { onReadOnlyChanged -= value; } }
		protected internal virtual void RaiseReadOnlyChanged() {
			if (onReadOnlyChanged != null)
				onReadOnlyChanged(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			PrepareForTouchUI();
		}
		void PrepareForTouchUI() {
			if (LookAndFeel.GetTouchUI()) {
				this.Properties.AutoHeight = false;
				Scale(TouchUIAdapter.GetSize(LookAndFeel));
				this.Dock = DockStyle.Top;
			}
		}
		protected internal virtual NameBoxController CreateController() {
			return new NameBoxController(this);
		}
		protected override PopupBaseForm CreatePopupForm() {
			return new NameBoxPopupListBoxForm(this);
		}
		protected override ToolTipControlInfo GetToolTipInfo(Point point) {
			return toolTipService.CalculateToolTipInfo(point);
		}
		void Initialize() {
			controller = CreateController();
			SubscribeControllerEvents();
		}
		void OnControllerNameBoxShowPopup(object sender, EventArgs e) {
			ShowPopup();
		}
		void SubscribeSpreadsheetControlEvents() {
			if (WinSpreadsheetControl != null) {
				WinSpreadsheetControl.GotFocus += OnWinSpreadsheetControlGotFocus;
				WinSpreadsheetControl.EnabledChanged += OnWinSpreadsheetControlEnabledChanged;
			}
		}
		void UnsubscribeSpreadsheetControlEvents() {
			if (WinSpreadsheetControl != null) {
				WinSpreadsheetControl.GotFocus -= OnWinSpreadsheetControlGotFocus;
				WinSpreadsheetControl.EnabledChanged -= OnWinSpreadsheetControlEnabledChanged;
			}
		}
		void SubscribeControllerEvents() {
			Controller.VisibleDefinedNamesChanged += OnControllerVisibleDefinedNamesChanged;
			Controller.SelectionChanged += OnControllerSelectionChanged;
		}
		void ChangeNameBoxText() {
			EditValue = Controller.OwnersText;
			SelectionLength = 0;
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			Controller.OnNameBoxKeyDown(this, e);
			base.OnKeyDown(e);
		}
		void OnFormulaBarOpenInplaceEditor(object sender, EventArgs e) {
			TrySetReadOnly(true);
		}
		internal void TrySetReadOnly(bool value) {
			if (ReadOnly == DefaultBoolean.Default)
				Properties.ReadOnly = value;
		}
		void OnControllerVisibleDefinedNamesChanged(object sender, VisibleDefinedNamesChangedEventArgs e) {
			Properties.Items.Clear();
			if (e.VisibleDefinedNames.Count != 0)
				Properties.Items.AddRange(e.VisibleDefinedNames);
		}
		void UnsubscribeControllerEvents() {
			if (Controller == null)
				return;
			Controller.VisibleDefinedNamesChanged -= OnControllerVisibleDefinedNamesChanged;
			Controller.SelectionChanged -= OnControllerSelectionChanged;
		}
		void OnWinSpreadsheetControlGotFocus(object sender, EventArgs e) {
			TrySetReadOnly(false);
		}
		void OnWinSpreadsheetControlEnabledChanged(object sender, EventArgs e) {
			this.Enabled = WinSpreadsheetControl.Enabled;
		}
		void OnControllerSelectionChanged(object sender, EventArgs e) {
			ChangeNameBoxText();
		}
		protected override void Dispose(bool disposing) {
			try {
				if (disposing)
					DisposeCore();
			}
			finally {
				base.Dispose(disposing);
				this.isDisposed = true;
			}
		}
		protected internal virtual void DisposeCore() {
			lock (this) {
				if (!IsDisposed)
					RaiseBeforeDispose();
				UnsubscribeControllerEvents();
				UnsubscribeSpreadsheetControlEvents();
				if (SpreadsheetControl != null)
					SpreadsheetControl.RemoveService(typeof(INameBoxControl));
			}
		}
		#region Events
		#region BeforeDispose
#if !SL && !WPF
		static readonly object onBeforeDispose = new object();
#if !SL
	[DevExpressXtraSpreadsheetLocalizedDescription("SpreadsheetNameBoxControlBeforeDispose")]
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
	}
	#endregion
	#region RepositoryItemNameBox
	[UserRepositoryItem("RegisterSpreadsheetNameBox")]
	public class RepositoryItemNameBox : RepositoryItemComboBox {
		static RepositoryItemNameBox() { RegisterSpreadsheetNameBox(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public new RepositoryItemNameBox Properties { get { return this; } }
		[DXCategory(CategoryName.Behavior),  DefaultValue(false), SmartTagProperty("Auto Complete", ""), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool AutoComplete { get { return base.AutoComplete; } set { base.AutoComplete = value; } }
		protected internal const string SpreadsheetNameBoxControlName = "SpreadsheetNameBoxControl";
		public override string EditorTypeName { get { return SpreadsheetNameBoxControlName; } }
		protected internal static void RegisterSpreadsheetNameBox() {
			EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(SpreadsheetNameBoxControlName,
			  typeof(SpreadsheetNameBoxControl), typeof(RepositoryItemNameBox), typeof(ComboBoxViewInfo), new ButtonEditPainter(),
			  true, EditImageIndexes.ComboBoxEdit, typeof(DevExpress.Accessibility.ComboBoxEditAccessible)));
		}
		[Localizable(true), DXCategory(CategoryName.Data),  DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design", typeof(System.Drawing.Design.UITypeEditor))]
		public override ComboBoxItemCollection Items { get { return base.Items; } }
	}
	#endregion
	#region NameBoxPopupListBoxForm
	public class NameBoxPopupListBoxForm : ComboBoxPopupListBoxForm {
		public NameBoxPopupListBoxForm(SpreadsheetNameBoxControl control)
			: base(control) {
		}
		protected override int GetItemCountForEmptyList() {
			return 8;
		}
	}
	#endregion
}
