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
using DevExpress.XtraEditors.Container;
using System.Windows.Forms.Design;
using DevExpress.XtraVerticalGrid.ViewInfo;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraVerticalGrid.Rows;
using DevExpress.XtraVerticalGrid.Data;
using DevExpress.XtraEditors;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using DevExpress.Utils;
using System.ComponentModel;
using System.Collections;
using System.Drawing.Design;
using System.Drawing;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.Utils.Win;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraVerticalGrid.Editors;
using DevExpress.XtraVerticalGrid.Native;
namespace DevExpress.XtraVerticalGrid.Editors {
	[System.ComponentModel.ToolboxItem(false)]
	public class GridEditorContainerHelper : EditorContainerHelper {
		public GridEditorContainerHelper(VGridControlBase grid)
			: base(grid) {
		}
		public virtual void ActivateEditor(RowValueInfo cell, UpdateEditorInfoArgs args) {
			RepositoryItem item = Owner.GetCustomRowEditForEditing(cell.Properties, Owner.FocusedRecord, cell.Item);
			if(item == null)
				return;
			BaseEdit be = UpdateEditor(item, args);
			if(be == null)
				return;
			be.EditValueChanging += new ChangingEventHandler(Owner.OnActiveEditor_ValueChanging);
			be.LostFocus += new EventHandler(Owner.OnActiveEditor_LostFocus);
			ShowEditor(be, Owner);
			if(ActiveEditor != null) {
				if(Owner.OptionsBehavior.AutoSelectAllInEditor)
					ActiveEditor.SelectAll();
				else ActiveEditor.DeselectAll();
			}
		}
		protected virtual VGridControlBase Grid { get { return Owner as VGridControlBase; } }
		public override BaseEdit UpdateEditor(RepositoryItem ritem, UpdateEditorInfoArgs args) {
			if(Grid != null) args.RightToLeft = Grid.ViewInfo.IsRightToLeft;
			return base.UpdateEditor(ritem, args);
		}
		public virtual void DeactivateEditor() {
			if(ActiveEditor == null) return;
			ActiveEditor.EditValueChanging -= new ChangingEventHandler(Owner.OnActiveEditor_ValueChanging);
			ActiveEditor.LostFocus -= new EventHandler(Owner.OnActiveEditor_LostFocus);
			HideEditorCore(Owner, Owner.HasFocus);
		}
		protected internal virtual void ActiveEditor_LMouseDown() {
			if(ActiveEditor == null) return;
			try {
				ActiveEditor.SendMouse(ActiveEditor.PointToClient(Control.MousePosition), MouseButtons.Left);
			} finally {
			}
		}
		protected override void OnRepositoryItemChanged(RepositoryItem item) {
			base.OnRepositoryItemChanged(item);
			Owner.EditorsPropertiesChanged(item);
		}
		protected override void OnEditorsRepositoryChanged() { Owner.EditorsRepositoryChanged(); }
		protected override void RaiseValidatingEditor(BaseContainerValidateEditorEventArgs e) {
			BeginLockFocus();
			try {
				Owner.RaiseValidatingEditor(e);
			} finally {
				CancelLockFocus();
			}
		}
		protected override void RaiseInvalidValueException(InvalidValueExceptionEventArgs e) {
			BeginLockFocus();
			try {
				Owner.RaiseInvalidValueException(e);
			} finally {
				CancelLockFocus();
			}
		}
		public void BeginLockFocus() { fInternalFocusLock++; }
		public void CancelLockFocus() { fInternalFocusLock--; }
		protected new VGridControlBase Owner { get { return base.Owner as VGridControlBase; } }
		public virtual bool BreakPostOnEquals() {
			return false;
		}
	}
	[System.ComponentModel.ToolboxItem(false)]
	public class PGridEditorContainerHelper : GridEditorContainerHelper {
		EditProviderBase provider;
		object unmodifiedEditValue;
		public PGridEditorContainerHelper(VGridControlBase grid)
			: base(grid) {
		}
		public object UnmodifiedEditValue { get { return unmodifiedEditValue; } set { unmodifiedEditValue = value; } }
		protected internal new PropertyGridControl Owner { get { return base.Owner as PropertyGridControl; } }
		internal EditProviderBase EditorProvider { get { return provider; } }
		public override void DeactivateEditor() {
			Owner.WindowsFormsEditorService.BeforeDeactivateEditor();
			EditorProvider.UnsubscribeEvents();
			base.DeactivateEditor();
		}
		public override bool ValidateEditor(IWin32Window owner) {
			if(ActiveEditor != null && !ActiveEditor.IsModified)
				return true;
			return base.ValidateEditor(owner);
		}
		protected override void RaiseValidatingEditor(BaseContainerValidateEditorEventArgs e) {
			BeginLockFocus();
			try {
				if (EditorProvider != null)
					EditorProvider.ValidatingEditor(e);
				Owner.RaiseValidatingEditor(e);
			}
			finally {
				CancelLockFocus();
			}
		}
		public override object EditingValue {
			get {
				if (EditorProvider != null)
					return EditorProvider.GetValue(ActiveEditor);
				return base.EditingValue;
			}
			set {
				if (EditorProvider != null) {
					if (!EditorProvider.BlockParse) {
						EditorProvider.SetEditValue(value);
					}
					base.EditingValue = EditorProvider.InitializeDisplayValue(value);
					return;
				}
				base.EditingValue = value;
			}
		}
		public override void ActivateEditor(RowValueInfo cell, UpdateEditorInfoArgs args) {
			DescriptorContext context = Owner.DataModeHelper.GetDescriptorContext(cell.Properties.FieldName);
			if(context.PropertyDescriptor == null)
				return;
			provider = CreateEditorProvider(cell.Item, args.MakeReadOnly, context);
			provider.ActivateEditor(this, cell, args);
		}
		internal void ActivateEditorBase(RowValueInfo cell, UpdateEditorInfoArgs args) {
			base.ActivateEditor(cell, args);
		}
		EditProviderBase CreateEditorProvider(RepositoryItem item, bool readOnly, ITypeDescriptorContext context) {
			EditProviderBase provider = null;
			IAutoEditor edit = item as IAutoEditor;
			if(edit != null) {
				switch(edit.GetEditorType()) {
					case EditorType.ComboBox:
						provider = new StandardValuesEditProvider((PGRepositoryItemComboBox)edit);
						break;
					case EditorType.Button:
						provider = new UIModalEditProvider((RepositoryItemButtonEdit)edit);
						break;
					case EditorType.PopupContainer:
						provider = new UIPopupEditProvider((RepositoryItemPopupBase)edit);
						break;
					case EditorType.Text:
						provider = new EditProviderBase((RepositoryItem)edit);
						break;
				}
			} else {
				if(item is RepositoryItemColorEdit || item is RepositoryItemFontEdit || item is RepositoryItemCalcEdit)
					provider = new PopupEditProvider((RepositoryItemPopupBase)item);
				else
					provider = new SimpleEditProvider(item);
			}
			provider.ReadOnly = readOnly;
			provider.Context = context;
			return provider;
		}
		public override HorzAlignment GetDefaultValueAlignment(RepositoryItem editor, Type columnType) {
			return HorzAlignment.Near;
		}
		internal void ActiveEditorEditValueChanged(object sender, EventArgs e) {
			if(this.provider.PostChangedValue && Owner.PostEditor()) {
				Owner.UpdateEditor();
			}
		}
		internal bool OverrideEditorDisplayText(RepositoryItem item, string fieldName) {
			ITypeDescriptorContext context = Owner.DataModeHelper.GetDescriptorContext(fieldName);
			if(context.PropertyDescriptor == null)
				return true;
			EditProviderBase provider = CreateEditorProvider(item, true, context);
			return provider.AllowConversionToDisplay();
		}
		public override bool BreakPostOnEquals() {
			return (Owner.GridDisposing || object.Equals(UnmodifiedEditValue, EditingValue));
		}
	}
	class WindowsFormsEditorService : IWindowsFormsEditorService {
		static NullUITypeEditor NullEditor = new NullUITypeEditor();
		PropertyGridControl owner;
		bool postPopupValue;
		UITypeEditor editor;
		bool uiEditing;
		PGPopupContainerEdit popupContainerEdit;
		BaseRow focusedRow;
		ITypeDescriptorContext currentContext;
		int focusedRecord;
		int recordCellIndex;
		RowProperties properties;
		object selectedObject;
		public bool UIEditing { get { return uiEditing; } }
		BaseEdit ActiveEditor { get; set; }
		RowProperties Properties { get { return properties; } set { properties = value; } }
		BaseRow Row { get { return focusedRow; } set { focusedRow = value; } }
		int Record { get { return focusedRecord; } set { focusedRecord = value; } }
		int RecordCellIndex { get { return recordCellIndex; } set { recordCellIndex = value; } }
		ITypeDescriptorContext CurrentContext { get { return currentContext; } set { currentContext = value; } }
		PGPopupContainerEdit PopupContainerEdit {
			get {
				if(popupContainerEdit == null) {
					popupContainerEdit = ActiveEditor as PGPopupContainerEdit;
				}
				return popupContainerEdit;
			}
		}
		UITypeEditor Editor {
			get {
				if(editor == NullEditor) {
					editor = owner.GetUITypeEditor(CurrentContext.PropertyDescriptor);
				}
				if(editor == null) {
					return NullEditor;
				}
				return editor;
			}
		}
		public WindowsFormsEditorService(PropertyGridControl owner) {
			this.owner = owner;
			Clear();
		}
		public void ProcessUIEditing() {
			Initialize();
			if(CurrentContext.PropertyDescriptor == null || UIEditing)
				return;
			owner.ContainerHelper.BeginAllowHideException();
			try {
				owner.BeginUpdate();
				owner.PostEditor();
			} catch {
				Release();
				return;
			} finally {
				owner.CancelUpdate();
				owner.ContainerHelper.EndAllowHideException();
			}
			try {
				object value = null;
				if(owner.ActiveEditor != null) {
					value = owner.EditorHelper.EditorProvider.GetValue(owner.ActiveEditor);
				}
				this.uiEditing = true;
				object newValue = Editor.EditValue(CurrentContext, owner, value);
				if(owner.SelectedObject != null && owner.SelectedObject.Equals(selectedObject) && PostValue(value, newValue)) {
					owner.DataModeHelper.InvalidateCache();
					owner.UpdateEditor();
					((IRowChangeListener)Row).RowPropertiesChanged(Properties, RowChangeTypeEnum.Value);
				}
			} catch(Exception e) {
				this.owner.ProcessUIEditingException(e);
			} finally {
				Release();
			}
		}
		void Initialize() {
			if(owner == null)
				throw new ArgumentNullException("PropertyGridControl");
			Clear();
			ActiveEditor = owner.ActiveEditor;
			Row = owner.FocusedRow;
			Record = owner.FocusedRecord;
			RecordCellIndex = owner.FocusedRecordCellIndex;
			Properties = Row.GetRowProperties(RecordCellIndex);
			CurrentContext = owner.DataModeHelper.GetDescriptorContext(Properties.FieldName);
			selectedObject = owner.SelectedObject;
		}
		void Clear() {
			this.editor = NullEditor;
			this.postPopupValue = true;
			this.uiEditing = false;
			this.popupContainerEdit = null;
		}
		bool PostValue(object value, object newValue) {
			if (!postPopupValue)
				return false;
			if (ActiveEditor == null || ActiveEditor != owner.ActiveEditor) {
				if (!object.Equals(newValue, value)) {
					owner.SetCellValue(Row, Record, newValue);
					return true;
				}
				return false;
			}
			if (owner.ActiveEditor == null)
				return false;
			try {
				this.owner.EditorHelper.EditorProvider.BlockParse = true;
				owner.EditorHelper.EditorProvider.SetEditValue(newValue);
				owner.ActiveEditor.EditValue = owner.EditorHelper.EditorProvider.InitializeDisplayValue(newValue);
				ActiveEditor.IsModified = !object.Equals(newValue, value);
				return owner.PostEditor();
			}
			finally {
				this.owner.EditorHelper.EditorProvider.BlockParse = false;
			}
		}
		void Release() {
			Clear();
		}
		#region IWindowsFormsEditorService Members
		void IWindowsFormsEditorService.CloseDropDown() {
			PopupContainerEdit.ClosePopup();
			DisposePopupContainerControl();
		}
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern int MsgWaitForMultipleObjects(int nCount, IntPtr pHandles, bool fWaitAll, int dwMilliseconds, int dwWakeMask);
		[System.Security.SecuritySafeCritical]
		static void MsgWaitForMultipleObjects() {
			MsgWaitForMultipleObjects(1, IntPtr.Zero, true, 250, 0xff);
		}
		bool controlCreatedFirstTime = false;
		void IWindowsFormsEditorService.DropDownControl(Control control) {
			if(control != null)
				control.Dock = DockStyle.Fill;
#if DXWhidbey
			if (CurrentContext.PropertyDescriptor != null) {
				PopupContainerEdit.Properties.PopupSizeable = Editor.IsDropDownResizable;
			}
#else
					edit.Properties.PopupSizeable = false;
#endif
			if (PopupContainerEdit.Properties.PopupControl == null) {
				PopupContainerEdit.Properties.PopupControl = new PopupContainerControl();
			}
			PopupContainerControl popup = PopupContainerEdit.Properties.PopupControl;
			PopupContainerEdit.Properties.UsePopupControlMinSize = true;
			if(control != null) {
				if (!control.IsHandleCreated) {
					this.controlCreatedFirstTime = true;
					control.CreateControl();
				}
				int width = PopupContainerEdit.Width;
				if (Editor.IsDropDownResizable && !this.controlCreatedFirstTime) {
					width = control.Size.Width;
				}
				popup.Size = new Size(width, control.Height);
				if (!control.MinimumSize.IsEmpty)
					popup.MinimumSize = control.MinimumSize;
				if (!popup.Controls.Contains(control))
					popup.Controls.Add(control);
				control.Visible = true;
			}
			PopupContainerEdit.Properties.CloseUp += new CloseUpEventHandler(DropDown_CloseUp);
			PopupContainerEdit.Properties.CloseOnLostFocus = true;
			this.postPopupValue = false;
			this.controlCreatedFirstTime = false;
			if (!PopupContainerEdit.IsPopupOpen)
				PopupContainerEdit.ShowPopupInternal();
			PopupContainerEdit.Capture = false;
			while(PopupContainerEdit != null && PopupContainerEdit.IsPopupOpen) {
				Application.DoEvents();
				MsgWaitForMultipleObjects();
			}
		}
		DialogResult IWindowsFormsEditorService.ShowDialog(Form dialog) {
			XtraForm xtraDialog = dialog as XtraForm;
			if(xtraDialog != null) {
				xtraDialog.LookAndFeel.UseDefaultLookAndFeel = true;
				xtraDialog.LookAndFeel.ParentLookAndFeel = owner.LookAndFeel;
			}
			return dialog.ShowDialog(owner);
		}
		#endregion
		void DropDown_CloseUp(object sender, CloseUpEventArgs e) {
			PopupContainerEdit.Properties.CloseUp -= new CloseUpEventHandler(DropDown_CloseUp);
			this.postPopupValue = e.AcceptValue;
			PopupContainerControl popupControl = PopupContainerEdit.Properties.PopupControl;
			if (popupControl == null)
				return;
			popupControl.Controls.Clear();
		}
		class NullUITypeEditor : UITypeEditor { }
		internal void BeforeDeactivateEditor() {
			DisposePopupContainerControl();
		}
		void DisposePopupContainerControl() {
			if (PopupContainerEdit == null)
				return;
			PopupContainerControl popupControl = PopupContainerEdit.Properties.PopupControl;
			if (popupControl == null)
				return;
			popupControl.Controls.Clear();
			popupControl.Dispose();
			PopupContainerEdit.Properties.PopupControl = null;
		}
	}
	class EditProviderBase {
		RepositoryItem item;
		bool readOnly;
		ITypeDescriptorContext context;
		protected object editValue;
		internal bool BlockParse = false;
		public EditProviderBase(RepositoryItem item) {
			this.item = item;
		}
		protected PGridEditorContainerHelper ContainerHelper { get; set; }
		public virtual bool PostChangedValue { get { return item is RepositoryItemCheckEdit; } }
		protected virtual bool CanFormat { get { return !KeepValueAlong && !(item is RepositoryItemColorEdit); } }
		protected virtual bool KeepValueAlong { get { return false; } }
		public void ExtractValue(object editValue) {
			if (KeepValueAlong)
				this.editValue = ParseEditValueCore(editValue);
		}
		protected object ParseEditValueCore(object value) {
			Type valueType = value != null ? value.GetType() : typeof(object);
			if (PropertyHelper.CanConvertFrom(Context, valueType)) {
				return PropertyHelper.ConvertFrom(Context, value);
			}
			return value;
		}
		public void UnsubscribeEvents() {
			if (ContainerHelper == null)
				return;
			UnsubscribeEditValueChangedEvent();
			UnsubscribeOthersEvents();
			ContainerHelper = null;
		}
		protected void UnsubscribeEditValueChangedEvent() {
			ContainerHelper.ActiveEditor.EditValueChanged -= ContainerHelper.ActiveEditorEditValueChanged;
		}
		protected virtual void UnsubscribeOthersEvents() {
			if(CanFormat) {
				ContainerHelper.ActiveEditor.FormatEditValue -= ActiveEditorFormatEditValue;
			}
		}
		public virtual bool AllowConversionToDisplay() {
			return PerformEditValueAsString;
		}
		protected virtual void InitializeEdit() { }
		protected void SubscribeEvents(RowProperties properties) {
			SubscribeEditValueChangedEvent();
			SubscribeOthersEvents(properties);
		}
		protected void SubscribeEditValueChangedEvent() {
			ContainerHelper.ActiveEditor.EditValueChanged += ContainerHelper.ActiveEditorEditValueChanged;
		}
		protected virtual void SubscribeOthersEvents(RowProperties properties) {
		}
		void ActiveEditorFormatEditValue(object sender, ConvertEditValueEventArgs e) {
			e.Handled = true;
			if(PerformEditValueAsString) {
				e.Value = PropertyHelper.ConvertToString(Context, e.Value);
			}
		}
		protected virtual bool CanInputText { get { return !ReadOnly; } }
		public virtual bool PerformEditValueAsString {
			get {
				bool canConvertFrom = PropertyHelper.CanConvertFrom(Context, typeof(string)),
					canConvertTo = PropertyHelper.CanConvertTo(Context, typeof(string));
				return ((CanInputText && canConvertTo && canConvertFrom) || (!CanInputText && canConvertTo)) && IsAllowConversion;
			}
		}
		internal protected ITypeDescriptorContext Context { get { return context; } set { context = value; } }
		protected bool IsAllowConversion {
			get {
#if DEBUGTEST
				if(Item == null)
					throw new ArgumentNullException();
#endif
				return !(Item is RepositoryItemComboBox) && !(Item is RepositoryItemCheckEdit) && !(Item is RepositoryItemLookUpEdit);
			}
		}
		protected RepositoryItem Item { get { return item; } }
		internal bool ReadOnly { get { return readOnly; } set { readOnly = value; } }
		internal void ActivateEditor(PGridEditorContainerHelper containerHelper, RowValueInfo cell, UpdateEditorInfoArgs args) {
			ContainerHelper = containerHelper;
			InitializeEdit();
			InitializeEditValue(args.EditValue);
			object value = InitializeDisplayValue(args.EditValue);
			UpdateEditorInfoArgs newArgs = new UpdateEditorInfoArgs(args.MakeReadOnly, args.Bounds, args.Appearance, value, args.LookAndFeel, args.ErrorText, args.ErrorIcon);
			if(CanFormat)
				Item.FormatEditValue += ActiveEditorFormatEditValue;
			containerHelper.ActivateEditorBase(cell, newArgs);
			SubscribeEvents(cell.Properties);
		}
		public virtual object InitializeDisplayValue(object value) {
			if (KeepValueAlong && PerformEditValueAsString)
				return PropertyHelper.ConvertToString(Context, value);
			return value;
		}
		public virtual void InitializeEditValue(object editValue) {
			ContainerHelper.UnmodifiedEditValue = editValue;
		}
		public void SetEditValue(object editValue) {
			if (KeepValueAlong)
				this.editValue = editValue;
		}
		public virtual object GetValue(BaseEdit editor) {
			if (KeepValueAlong)
				return editValue;
			else
				return editor.EditValue;
		}
		internal void ValidatingEditor(BaseContainerValidateEditorEventArgs e) {
			if (!KeepValueAlong)
				return;
			if (!BlockParse) {
				ExtractValue(ContainerHelper.ActiveEditor.EditValue);
				e.Value = GetValue(ContainerHelper.ActiveEditor);
			}
		}
	}
	class StandardValuesEditProvider : EditProviderBase {
		bool exclusive;
		public StandardValuesEditProvider(PGRepositoryItemComboBox item) : base(item) { }
		protected override void InitializeEdit() {
			bool canConvertToString = PropertyHelper.GetConverter(Context).CanConvertTo(Context, typeof(string));
			ICollection values = PropertyHelper.GetStandardValues(Context, out this.exclusive);
			PopulateComboEditor(values, canConvertToString);
			ComboItem.TextEditStyle = exclusive ? TextEditStyles.DisableTextEditor : TextEditStyles.Standard;
		}
		public override bool PostChangedValue { get { return this.exclusive; } }
		protected override bool CanInputText { get { return base.CanInputText && ComboItem.TextEditStyle == TextEditStyles.Standard; } }
		protected virtual RepositoryItemComboBox ComboItem { get { return Item as RepositoryItemComboBox; } }
		void PopulateComboEditor(ICollection values, bool convertValuesToString) {
			ComboItem.Items.Clear();
			if(values == null)
				return;
			foreach(object value in values) {
				string caption = null;
				if(convertValuesToString) {
					caption = Convert.ToString(PropertyHelper.ConvertToString(Context, value));
				} else
					caption = value == null ? string.Empty : value.ToString();
				object comboValue = CreateComboBoxItem(caption, value);
				ComboItem.Items.Add(comboValue);
			}
		}
		ComboBoxItem CreateComboBoxItem(string caption, object value) {
			return new ImageComboBoxItem(caption, value);
		}
		public override bool PerformEditValueAsString { get { return false; } }
		public override bool AllowConversionToDisplay() {
			return true;
		}
	}
	class SimpleEditProvider : EditProviderBase {
		public SimpleEditProvider(RepositoryItem item) : base(item) { }
		public override bool PerformEditValueAsString { get { return false; } }
		public override bool AllowConversionToDisplay() {
			return false;
		}
	}
	class ButtonEditProvider : EditProviderBase {
		public ButtonEditProvider(RepositoryItemButtonEdit item) : base(item) { }
		protected virtual void ProcessUIEditing(object sender, ButtonPressedEventArgs args) { }
		protected override bool CanInputText { get { return !ReadOnly && ButtonItem.TextEditStyle == TextEditStyles.Standard; } }
		protected virtual RepositoryItemButtonEdit ButtonItem { get { return Item as RepositoryItemButtonEdit; } }
		protected override void SubscribeOthersEvents(RowProperties properties) {
			((ButtonEdit)ContainerHelper.ActiveEditor).ButtonClick += OnButtonClick;
		}
		protected override void UnsubscribeOthersEvents() {
			((ButtonEdit)ContainerHelper.ActiveEditor).ButtonClick -= OnButtonClick;
		}
		protected virtual void OnButtonClick(object sender, ButtonPressedEventArgs args) { }
		public override void InitializeEditValue(object editValue) {
			base.InitializeEditValue(editValue);
			SetEditValue(editValue);
		}
	}
	class UIModalEditProvider : ButtonEditProvider {
		public UIModalEditProvider(RepositoryItemButtonEdit item)
			: base(item) {
		}
		protected override void OnButtonClick(object sender, ButtonPressedEventArgs args) {
			ContainerHelper.Owner.ProcessUIEditing(args);
		}
		protected override bool KeepValueAlong { get { return true; } }
	}
	class PopupEditProvider : ButtonEditProvider {
		bool popup;
		public PopupEditProvider(RepositoryItemPopupBase item) : base(item) { }
		protected override void SubscribeOthersEvents(RowProperties properties) {
			base.SubscribeOthersEvents(properties);
			PopupBaseEdit popupEdit = ContainerHelper.ActiveEditor as PopupBaseEdit;
			popupEdit.Closed += StopPopup;
			popupEdit.Popup += StartPopup;
			ColorPickEditBase colorPickEdit = ContainerHelper.ActiveEditor as ColorPickEditBase;
			if(colorPickEdit != null) {
				colorPickEdit.ColorPickDialogShowing += StartModalDialog;
				colorPickEdit.ColorPickDialogClosed += StopModalDialog;
			}
		}
		public override bool PostChangedValue { get { return Popup || ModalDialog; } }
		protected override void UnsubscribeOthersEvents() {
			base.UnsubscribeOthersEvents();
			PopupBaseEdit popupEdit = (PopupBaseEdit)ContainerHelper.ActiveEditor as PopupBaseEdit;
			popupEdit.Closed -= StopPopup;
			popupEdit.Popup -= StartPopup;
			ColorPickEditBase colorPickEdit = ContainerHelper.ActiveEditor as ColorPickEditBase;
			if(colorPickEdit != null) {
				colorPickEdit.ColorPickDialogShowing -= StartModalDialog;
				colorPickEdit.ColorPickDialogClosed -= StopModalDialog;
			}
		}
		bool ModalDialog { get; set; }
		bool Popup { get { return popup; } }
		void StopPopup(object sender, ClosedEventArgs e) {
			this.popup = false;
		}
		void StartPopup(object sender, EventArgs e) {
			this.popup = true;
		}
		void StopModalDialog(object sender, ColorPickDialogClosedEventArgs e) {
			ModalDialog = false;
		}
		void StartModalDialog(object sender, ColorPickDialogShowingEventArgs e) {
			ModalDialog = true;
		}
	}
	class UIPopupEditProvider : PopupEditProvider {
		public UIPopupEditProvider(RepositoryItemPopupBase item) : base(item) { }
		protected override bool KeepValueAlong { get { return true; } }
		protected override void OnButtonClick(object sender, ButtonPressedEventArgs args) {
			ContainerHelper.Owner.ProcessUIEditing(args);
		}
	}
	enum EditorType {
		PopupContainer,
		ComboBox,
		Button,
		Text
	}
	interface IAutoEditor {
		EditorType GetEditorType();
	}
	class PGRepositoryItemPopupContainerEdit : RepositoryItemPopupContainerEdit, IAutoEditor {
		static PGRepositoryItemPopupContainerEdit() {
			if(!EditorRegistrationInfo.Default.Editors.Contains("PGPopupContainerEdit"))
				EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo("PGPopupContainerEdit", typeof(PGPopupContainerEdit), typeof(PGRepositoryItemPopupContainerEdit), typeof(PopupContainerEditViewInfo), new ButtonEditPainter(), false, EditImageIndexes.PopupContainerEdit, typeof(DevExpress.Accessibility.PopupEditAccessible)));
		}
		public PGRepositoryItemPopupContainerEdit()
			: base() {
			this.ShowPopupCloseButton = false;
			this.ShowDropDown = XtraEditors.Controls.ShowDropDown.Never;
		}
		public override string EditorTypeName { get { return "PGPopupContainerEdit"; } }
		EditorType IAutoEditor.GetEditorType() {
			return EditorType.PopupContainer;
		}
		public override object Clone() {
			PGRepositoryItemPopupContainerEdit clone = new PGRepositoryItemPopupContainerEdit();
			clone.Assign(this);
			return clone;
		}
	}
	class PGPopupContainerEdit : PopupContainerEdit {
		PropertyGridControl Grid { get { return (PropertyGridControl)EditorContainer; } }
		[Browsable(false)]
		public override string EditorTypeName { get { return "PGPopupContainerEdit"; } }
		new internal ButtonEditViewInfo ViewInfo { get { return base.ViewInfo; } }
		public new void BeginAcceptEditValue() {
			base.BeginAcceptEditValue();
		}
		public new void EndAcceptEditValue() {
			base.EndAcceptEditValue();
		}
		protected override Rectangle ConstrainFormBounds(Rectangle r) {
			Rectangle corrected = CorrectFormRectangle(r);
			return base.ConstrainFormBounds(corrected);
		}
		Rectangle CorrectFormRectangle(Rectangle r) {
			Rectangle actual = r,
				original = r;
			actual.Offset(Location);
			Rectangle gridRectangle = RectangleToScreen(Grid.ViewInfo.ViewRects.Client);
			Screen screen = Screen.FromControl(this);
			int rightConstraint = gridRectangle.Right - 2;
			if(rightConstraint < actual.Right)
				original.Offset(rightConstraint - actual.Right, 0);
			int leftConstraint = screen.WorkingArea.Left;
			if(original.Left < leftConstraint)
				original.Offset(leftConstraint - original.Left, 0);
			return original;
		}
		protected override void UpdateEditValueOnClose(PopupCloseMode closeMode, bool acceptValue, object newValue, object oldValue) {
		}
		public void ShowPopupInternal() {
			DoShowPopup();
		}
		public override void ShowPopup() {
		}
	}
}
namespace DevExpress.XtraVerticalGrid.Native {
	public class PGRepositoryItemComboBox : RepositoryItemImageComboBox, IAutoEditor {
		static PGRepositoryItemComboBox() {
			if (!EditorRegistrationInfo.Default.Editors.Contains("PGComboBoxEdit"))
				EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo("PGComboBoxEdit", typeof(PGComboBoxEdit), typeof(PGRepositoryItemComboBox), typeof(ImageComboBoxEditViewInfo), new ButtonEditPainter(), false));
		}
		public PGRepositoryItemComboBox()
			: base() {
		}
		public override string EditorTypeName { get { return "PGComboBoxEdit"; } }
		public override TextEditStyles TextEditStyle {
			get {
				return base.TextEditStyle;
			}
			set {
				if (TextEditStyle == value)
					return;
				fTextEditStyle = value;
				OnTextEditStyleChanged();
				OnPropertiesChanged();
			}
		}
		public override object Clone() {
			PGRepositoryItemComboBox clone = new PGRepositoryItemComboBox();
			clone.Assign(this);
			return clone;
		}
		EditorType IAutoEditor.GetEditorType() {
			return EditorType.ComboBox;
		}
	}
	[DXToolboxItem(false)]
	public class PGComboBoxEdit : ImageComboBoxEdit {
		public override string EditorTypeName {
			get { return "PGComboBoxEdit"; }
		}
#if DEBUGTEST
		new internal BaseEditViewInfo ViewInfo { get { return base.ViewInfo; } }
#endif
	}
	public class PGRepositoryItemButtonEdit : RepositoryItemButtonEdit, IAutoEditor {
		public override object Clone() {
			PGRepositoryItemButtonEdit clone = new PGRepositoryItemButtonEdit();
			clone.Assign(this);
			return clone;
		}
		EditorType IAutoEditor.GetEditorType() {
			return EditorType.Button;
		}
	}
	public class PGRepositoryItemTextEdit : RepositoryItemTextEdit, IAutoEditor {
		public override object Clone() {
			PGRepositoryItemTextEdit clone = new PGRepositoryItemTextEdit();
			clone.Assign(this);
			return clone;
		}
		EditorType IAutoEditor.GetEditorType() {
			return EditorType.Text;
		}
	}
}
