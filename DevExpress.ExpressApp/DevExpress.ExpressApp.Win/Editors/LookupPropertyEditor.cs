#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Windows.Forms;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.ExpressApp.Win.Editors.Grid;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.ExpressApp.Win.Templates;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraLayout;
namespace DevExpress.ExpressApp.Win.Editors {
	public class LookupPropertyEditor : DXPropertyEditor, IDependentPropertyEditor, IComplexViewItem, ISupportViewShowing, ILookupEditProvider, IFrameContainer {
		private LookupEditorHelper helper;
		private void DestroyPopupForm() {
			if(Control != null) {
				Control.DestroyPopupForm();
			}
		}
		private void Control_QueryPopUp(object sender, CancelEventArgs e) {
			OnQueryPopUp(e);
			OnViewShowingNotification();
		}
		private void Control_QueryCloseUp(object sender, CancelEventArgs e) {
			OnQueryCloseUp(e);
		}
		protected virtual void OnQueryPopUp(CancelEventArgs e) {
			if(QueryPopUp != null) {
				QueryPopUp(this, e);
			}
		}
		protected virtual void OnQueryCloseUp(CancelEventArgs e) {
			if(QueryCloseUp != null) {
				QueryCloseUp(this, e);
			}
		}
		protected virtual void OnViewShowingNotification() {
			if(viewShowingNotification != null) {
				viewShowingNotification(this, EventArgs.Empty);
			}
		}
		protected override void SetupRepositoryItem(RepositoryItem item) {
			base.SetupRepositoryItem(item);
			((RepositoryItemLookupEdit)item).Init(this, DisplayFormat, helper);
		}
		protected override RepositoryItem CreateRepositoryItem() {
			return new RepositoryItemLookupEdit();
		}
		protected override object CreateControlCore() {
			return new LookupEdit();
		}
		protected override void OnControlCreated() {
			Control.QueryPopUp += new CancelEventHandler(Control_QueryPopUp);
			Control.QueryCloseUp += new CancelEventHandler(Control_QueryCloseUp);
			base.OnControlCreated();
			OnLookupEditCreated(Control);
		}
		protected override void ReadValueCore() {
			DestroyPopupForm();
			base.ReadValueCore();
		}
		protected override void Dispose(bool disposing) {
			try {
				if(Control != null) {
					OnLookupEditHide(Control);
					Control.QueryPopUp -= new CancelEventHandler(Control_QueryPopUp);
					Control.QueryCloseUp -= new CancelEventHandler(Control_QueryCloseUp);
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		protected override void OnCurrentObjectChanged() {
			base.OnCurrentObjectChanged();
			if(helper != null) {
				helper.SetDataType(CurrentObject);
			}
		}
		public LookupPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) { }
		public override void Refresh() {
			base.Refresh();
			if(Control != null) {
				Control.UpdateDisplayText();
			}
		}
		public void Setup(IObjectSpace objectSpace, XafApplication application) {
			if(helper == null) {
				helper = new LookupEditorHelper(application, objectSpace, MemberInfo.MemberTypeInfo, Model);
			}
			if(objectSpace == null) {
				DestroyPopupForm();
			}
			helper.SetObjectSpace(objectSpace);
		}
		public new LookupEdit Control {
			get { return (LookupEdit)base.Control; }
		}
		public LookupEditorHelper Helper {
			get { return helper; }
		}
		public event CancelEventHandler QueryPopUp;
		public event CancelEventHandler QueryCloseUp;
		IList<string> IDependentPropertyEditor.MasterProperties {
			get { return helper.MasterProperties; }
		}
		public override bool CanFormatPropertyValue {
			get { return true; }
		}
		#region ISupportViewShowing Members
		private event EventHandler<EventArgs> viewShowingNotification;
		event EventHandler<EventArgs> ISupportViewShowing.ViewShowingNotification {
			add { viewShowingNotification += value; }
			remove { viewShowingNotification -= value; }
		}
		#endregion
		#region ILookupEditProvider Members
		private event EventHandler<LookupEditProviderEventArgs> lookupEditCreated;
		private event EventHandler<LookupEditProviderEventArgs> lookupEditHide;
		protected void OnLookupEditCreated(LookupEdit editor) {
			if(lookupEditCreated != null) {
				lookupEditCreated(this, new LookupEditProviderEventArgs(editor));
			}
		}
		protected void OnLookupEditHide(LookupEdit editor) {
			if(lookupEditHide != null) {
				lookupEditHide(this, new LookupEditProviderEventArgs(editor));
			}
		}
		event EventHandler<LookupEditProviderEventArgs> ILookupEditProvider.ControlCreated {
			add { lookupEditCreated += value; }
			remove { lookupEditCreated -= value; }
		}
		event EventHandler<LookupEditProviderEventArgs> ILookupEditProvider.HideControl {
			add { lookupEditHide += value; }
			remove { lookupEditHide -= value; }
		}
		#endregion
		#region IFrameContainer Members
		public Frame Frame {
			get {
				if(Control != null) {
					return Control.Frame;
				}
				return null;
			}
		}
		public void InitializeFrame() {
		}
		#endregion
	}
	public class LookupEditProviderEventArgs : EventArgs {
		private LookupEdit lookupEdit;
		public LookupEdit LookupEdit { get { return lookupEdit; } }
		public LookupEditProviderEventArgs(LookupEdit lookupEdit) {
			this.lookupEdit = lookupEdit;
		}
	}
	public interface ILookupEditProvider {
		event EventHandler<LookupEditProviderEventArgs> ControlCreated;
		event EventHandler<LookupEditProviderEventArgs> HideControl;
	}
	[ToolboxItem(false)]
	public class LookupEdit : PopupContainerEdit, IGridInplaceEdit, ILookupEditTest {
		static LookupEdit() {
			RepositoryItemLookupEdit.Register();
		}
		private List<IDisposable> disposableObjects = new List<IDisposable>();
		private object gridEditingObject;
		private bool suppressCloseUp;
		private bool lockDisposePopupForm;
		private bool isValidating = false;
		private NestedFrame frame;
		private PopupContainerControl popupControl;
		object IGridInplaceEdit.GridEditingObject {
			get { return gridEditingObject; }
			set {
				if(gridEditingObject != value) {
					gridEditingObject = value;
					OnEditingObjectChanged();
				}
			}
		}
		string ILookupEditTest.DisplayMember {
			get { return Properties.Helper.DisplayMember.Name; }
		}
		bool ILookupEditTest.CanShowPopup {
			get { return CanShowPopupForm; }
		}
		ILookupEditPopupForm ILookupEditTest.PopupForm {
			get { return (ILookupEditPopupForm)PopupForm; }
		}
		String ILookupEditTest.GetDisplayText(object obj) {
			if(obj is XafDataViewRecord) {
				return Properties.Helper.GetDisplayText(obj, "", "", Properties.Helper.ObjectSpace);
			}
			else {
				return Properties.GetDisplayText(Properties.DisplayFormat, obj);
			}
		}
		private Boolean CanShowPopupForm {
			get { return !Properties.ReadOnly && Enabled; }
		}
		private void OnEditingObjectChanged() {
			DestroyPopupForm();
			if(EditValue != null && BindingHelper.FindEditingObject(this) == null) {
				EditValue = null;
			}
		}
		private void DataBindings_CollectionChanged(object sender, CollectionChangeEventArgs e) {
			OnEditingObjectChanged();
		}
		private void DisposeFrame() {
			if(frame != null) {
				frame.SaveModel();
				frame.Dispose();
				frame = null;
			}
		}
		protected override PopupBaseForm CreatePopupForm() {
			frame = Properties.Helper.Application.CreateNestedFrame(Properties.ViewItem, TemplateContext.LookupControl);
			LookupEditPopupForm resultForm = null;
			LockDisposePopupForm = true;
			try {
				frame.CreateTemplate();
				ILookupPopupFrameTemplate template = (ILookupPopupFrameTemplate)frame.Template;
				Object editingObject = Properties.OwnerEdit.FindEditingObject();
				ListView listView = Properties.Helper.CreateListView(editingObject);
				IButtonsContainersOwner lookupControlTemplate = template as IButtonsContainersOwner;
				if(lookupControlTemplate != null) {
					lookupControlTemplate.ShowButtonsContainersPanel = false;
				}
				frame.SetView(listView, null);
				FilterController filterController = frame.GetController<FilterController>();
				template.IsSearchEnabled = filterController != null && filterController.FullTextFilterAction.Active && Properties.Helper.CanFilterDataSource(listView.CollectionSource, editingObject);
				popupControl = new PopupContainerControl();
				((Control)frame.Template).Dock = DockStyle.Fill;
				popupControl.Controls.Add((Control)frame.Template);
				Properties.PopupControl = popupControl;
				if(lookupControlTemplate != null && !lookupControlTemplate.IsButtonsContainersPanelEmpty()) {
					resultForm = new LookupEditPopupForm(this, lookupControlTemplate.ButtonsContainersPanel);
				}
				else {
					resultForm = new LookupEditPopupForm(this);
				}
				template.FocusFindEditor();
				ILookupListEditor listEditor = listView.Editor as ILookupListEditor;
				if(listEditor != null) {
					listEditor.ProcessSelectedItemBySingleClick = true;
					listEditor.TrackMousePosition = true;
					listEditor.BeginCustomization += new EventHandler(listEditor_BeginCustomization);
					listEditor.EndCustomization += new EventHandler(listEditor_EndCustomization);
				}
				IConfigurableLookupListEditor configurableLookupListEditor = listView.Editor as IConfigurableLookupListEditor;
				if(configurableLookupListEditor != null) {
					configurableLookupListEditor.Setup();
				}
			}
			finally {
				LockDisposePopupForm = false;
			}
			return resultForm;
		}
		private void listEditor_BeginCustomization(object sender, EventArgs e) {
			SuppressCloseUp = true;
		}
		private void listEditor_EndCustomization(object sender, EventArgs e) {
			SuppressCloseUp = false;
		}
		protected override void OnEditorKeyPress(KeyPressEventArgs e) {
			base.OnEditorKeyPress(e);
			if(!Properties.ReadOnly && !e.Handled && e.KeyChar != '\t') {
				char charCode = e.KeyChar;
				if(Properties.CharacterCasing != CharacterCasing.Normal) {
					charCode = Properties.CharacterCasing == CharacterCasing.Lower ? Char.ToLower(e.KeyChar) : Char.ToUpper(e.KeyChar);
				}
				ShowPopup();
				if(!Char.IsControl(charCode) || charCode == '\b') {
					string searchString = charCode == '\b' ? "" : charCode.ToString();
					PopupForm.Template.SetStartSearchString(searchString);
				}
				e.Handled = true; 
			}
		}
		protected override void OnEditorKeyDown(KeyEventArgs e) {
			base.OnEditorKeyDown(e);
			if(e.KeyData == (Keys.Down | Keys.Alt) || e.KeyCode == Keys.Delete) {
				if(!Properties.ReadOnly && !e.Handled) {
					ShowPopup();
					PopupForm.Template.SetStartSearchString("");
					e.Handled = true;
				}
			}
		}
		protected override void ClosePopup(PopupCloseMode closeMode) {
			if(!suppressCloseUp) {
				if(Frame != null && Frame.View is ListView) {
					Frame.SaveModel();
				}
				base.ClosePopup(closeMode);
				if(Frame != null && Frame.View is ListView && !this.IsDisposing && this.IsHandleCreated) {
					BeginInvoke(new MethodInvoker(HideMemoEditors));
				}
			}
		}
		private void HideMemoEditors() {
			if(Frame != null && Frame.View != null) {
				GridListEditor gridListEditor = ((ListView)Frame.View).Editor as GridListEditor;
				if(gridListEditor != null) {
					DevExpress.XtraEditors.MemoExEdit edit = gridListEditor.GridView.ActiveEditor as DevExpress.XtraEditors.MemoExEdit;
					if(edit != null)
						edit.ClosePopup();
				}
			}
		}
		protected override void DoShowPopup() {
			if(popupControl != null) {
				Properties.PopupControl = popupControl;
			}
			base.DoShowPopup();
			PopupForm.SelectObject(EditValue);
		}
		protected override bool IsAcceptCloseMode(PopupCloseMode closeMode) {
			return closeMode != PopupCloseMode.Cancel && PopupForm != null && PopupForm.IsUserTakesChoice;
		}
		protected override void OnValidating(CancelEventArgs e) {
			isValidating = true;
			try {
				base.OnValidating(e);
			}
			finally {
				isValidating = false;
			}
		}
		protected override bool CanShowPopup {
			get { return CanShowPopupForm; }
		}
		protected override void Dispose(bool disposing) {
			DataBindings.CollectionChanged -= new CollectionChangeEventHandler(DataBindings_CollectionChanged);
			base.Dispose(disposing);
			if(disposing) {
				DisposeFrame();
				if(popupControl != null) {
					popupControl.Dispose();
					popupControl = null;
				}
				foreach(IDisposable disposableObject in disposableObjects) {
					disposableObject.Dispose();
				}
				disposableObjects.Clear();
			}
		}
		protected override void OnPressButton(EditorButtonObjectInfoArgs buttonInfo) {
			if(RepositoryItemLookupEdit.ClearButtonIdentifier.Equals(buttonInfo.Button.Tag)) {
				ClearValue();
			}
			base.OnPressButton(buttonInfo);
		}
		protected virtual void ClearValue() {
			if(IsPopupOpen) {
				CancelPopup();
			}
			EditValue = null;
		}
		public LookupEdit() {
			DataBindings.CollectionChanged += new CollectionChangeEventHandler(DataBindings_CollectionChanged);
		}
		public new void UpdateDisplayText() {
			base.UpdateDisplayText();
			base.Refresh();
		}
		public override void SendKey(object message, KeyPressEventArgs e) {
			OnKeyPress(e);
		}
		public new void DestroyPopupForm() {
			base.DestroyPopupForm();
		}
		protected override void DestroyPopupFormCore(bool dispose) {
			if(!LockDisposePopupForm) {
				DisposeFrame();
				base.DestroyPopupFormCore(dispose);
			}
		}
		public CollectionSourceBase GetPopupFormCollectionSource() {
			LookupEditPopupForm popupForm = (LookupEditPopupForm)CreatePopupForm();
			disposableObjects.Add(popupForm);
			return popupForm.ListView.CollectionSource;
		}
		public object FindEditingObject() {
			return BindingHelper.FindEditingObject(this);
		}
		public override string EditorTypeName {
			get { return RepositoryItemLookupEdit.EditorName; }
		}
		public override object EditValue {
			get { return base.EditValue; }
			set {
				object newValue = value;
				if(newValue != DBNull.Value && newValue != null) {
					if(newValue is XafDataViewRecord && Properties.Helper.ObjectSpace != null) {
						newValue = Properties.Helper.ObjectSpace.GetObject(newValue);
					}
					if(!Properties.Helper.LookupObjectType.IsInstanceOfType(newValue)) {
						if(Properties.ThrowInvalidCastException) {
							throw new InvalidCastException(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.UnableToCast,
								newValue.GetType(),
								Properties.Helper.LookupObjectType));
						}
						else {
							base.EditValue = null;
							return;
						}
					}
				}
				base.EditValue = newValue;
			}
		}
		public bool LockDisposePopupForm {
			get { return lockDisposePopupForm; }
			set { lockDisposePopupForm = value; }
		}
		public new RepositoryItemLookupEdit Properties {
			get { return (RepositoryItemLookupEdit)base.Properties; }
		}
		public new LookupEditPopupForm PopupForm {
			get { return (LookupEditPopupForm)base.PopupForm; }
		}
		public IList PopupFormDataSource {
			get {
				if(PopupForm != null) {
					return PopupForm.ListView.CollectionSource.List;
				}
				return null;
			}
		}
		public bool SuppressCloseUp {
			get { return suppressCloseUp; }
			set { suppressCloseUp = value; }
		}
		public override bool IsModified {
			get { return base.IsModified; }
			set {
				if(!isValidating) {
					base.IsModified = value;
				}
			}
		}
		public NestedFrame Frame {
			get { return frame; }
		}
	}
	public class LookupEditPopupFormViewInfo : CustomBlobPopupFormViewInfo {
		private Rectangle buttonsPanelRect = Rectangle.Empty;
		public LookupEditPopupFormViewInfo(LookupEditPopupForm form) : base(form) { }
		protected override int CalcSizeBarHeight(Size gripSize) {
			if(ButtonsPanel != null) {
				return ButtonsPanel.Root.MinSize.Height; 
			}
			return base.CalcSizeBarHeight(gripSize);
		}
		protected virtual void CalcButtonsPanelRect() {
			if(ButtonsPanel == null) return;
			int x = 0;
			int width = 0;
			if(!SizeBarRect.IsEmpty) {
				x = SizeBarRect.X;
				width = SizeBarRect.Width;
				if(!SizeGripRect.IsEmpty) {
					if(IsLeftSizeGrip) {
						x = SizeGripRect.Right;
						width = SizeBarRect.Right - SizeGripRect.Right;
					}
					else {
						width = SizeGripRect.X - x;
					}
				}
				if(!ButtonSize.IsEmpty) {
					if(Form.CloseButtonStyle != BlobCloseButtonStyle.Glyph) {
						if(Form.ShowOkButton) {
							width = OkButtonRect.X - x;
						}
						else if(Form.ShowCloseButton && !CloseButtonRect.IsEmpty) {
							width = CloseButtonRect.X - x;
						}
					}
					else {
						if(Form.ShowCloseButton) {
							if(!SizeGripRect.IsEmpty) {
								if(!IsLeftSizeGrip) {
									x = CloseButtonRect.Right;
									width = (Form.ShowOkButton) ? OkButtonRect.X - x : SizeGripRect.X - x;
								}
								else {
									if(Form.ShowOkButton) {
										x = OkButtonRect.Right;
									}
									width = CloseButtonRect.X - x;
								}
							}
							else {
								x = CloseButtonRect.Right;
								width = (Form.ShowOkButton) ? OkButtonRect.X - x : SizeBarRect.Right - x;
							}
						}
						else {
							if(!SizeGripRect.IsEmpty && !IsLeftSizeGrip) {
								width = OkButtonRect.X - SizeGripRect.Right;
							}
							else {
								width = OkButtonRect.X - SizeBarRect.Right;
							}
						}
					}
				}
			}
			buttonsPanelRect = new Rectangle(x, SizeBarRect.Y, width, SizeBarRect.Height);
		}
		protected override void CalcContentRect(Rectangle bounds) {
			base.CalcContentRect(bounds);
			CalcButtonsPanelRect();
		}
		public LayoutControl ButtonsPanel {
			get { return ((LookupEditPopupForm)Form).ButtonsPanel; }
		}
		public Rectangle ButtonsPanelRect {
			get { return buttonsPanelRect; }
		}
	}
	public class LookupEditPopupForm : PopupContainerForm, ILookupEditPopupForm {
		private ListView listView;
		private Object resultValue;
		private Boolean isUserTakesChoice;
		private Object createdObject;
		private Boolean isInitialization;
		private List<IObjectSpace> newObjectSpaces = new List<IObjectSpace>();
		private ListViewProcessCurrentObjectController processCurrentObjectController;
		private LayoutControl buttonsPanel;
		private XafApplication application;
		private Frame ownerEditFrame;
		private void newObjectViewController_ObjectCreated(Object sender, ObjectCreatedEventArgs e) {
			ClosePopup();
			createdObject = e.CreatedObject;
			e.ObjectSpace.ModifiedChanged += new EventHandler(objectSpace_ModifiedChanged);
			newObjectSpaces.Add(e.ObjectSpace);
		}
		private void application_ViewShowing(Object sender, ViewShowingEventArgs e) {
			if((e.View is DetailView) && (((DetailView)e.View).CurrentObject == createdObject)) {
				DetailViewLinkController detailViewLinkController = e.TargetFrame.GetController<DetailViewLinkController>();
				detailViewLinkController.SkipCurrentObjectReloading = true;
			}
		}
		private void objectSpace_ModifiedChanged(Object sender, EventArgs e) {
			((LookupEdit)OwnerEdit).LockDisposePopupForm = true;
			try {
				Object createdObject_ = listView.ObjectSpace.GetObject(createdObject);
				OwnerEdit.IsModified = true;
				OwnerEdit.EditValue = createdObject_;
				OwnerEdit.DoValidate();
			}
			finally {
				((LookupEdit)OwnerEdit).LockDisposePopupForm = false;
			}
		}
		private void processCurrentObjectController_CustomProcessSelectedItem(object sender, CustomProcessListViewSelectedItemEventArgs e) {
			e.Handled = true;
			if(!isInitialization) {
				CommitSelectedValue();
			}
		}
		private void UpdateButtonsPanelPosition() {
			if(buttonsPanel != null) {
				UpdateSeparatorPosition();
				buttonsPanel.Bounds = ViewInfo.ButtonsPanelRect;
			}
		}
		private void UpdateSeparatorPosition() {
			UpdatePaddings();
			IButtonsContainersOwner lookupControlTemplate = Template as IButtonsContainersOwner;
			if(lookupControlTemplate != null) {
				lookupControlTemplate.BottomSeparator.Dock = ViewInfo.IsTopSizeBar ? DockStyle.Top : DockStyle.Bottom;
			}
		}
		private void UpdatePaddings() {
			if(this.buttonsPanel != null) {
				if(!ViewInfo.IsTopSizeBar) {
					this.buttonsPanel.Dock = DockStyle.Bottom;
					this.buttonsPanel.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 23, 1);
				}
				else {
					this.buttonsPanel.Dock = DockStyle.Top;
					this.buttonsPanel.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 1, 23);
				}
			}
		}
		protected override Size MinFormSize {
			get { return CalcFormSize(((Control)OwnerEdit.Frame.Template).MinimumSize); }
		}
		protected new LookupEditPopupFormViewInfo ViewInfo {
			get {
				return (base.ViewInfo as LookupEditPopupFormViewInfo);
			}
		}
		protected override void UpdateControlPositionsCore() {
			base.UpdateControlPositionsCore();
			UpdateButtonsPanelPosition();
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(processCurrentObjectController != null) {
					processCurrentObjectController.CustomProcessSelectedItem -= new EventHandler<CustomProcessListViewSelectedItemEventArgs>(processCurrentObjectController_CustomProcessSelectedItem);
					processCurrentObjectController = null;
				}
				if(application != null) {
					application.ViewShowing -= new EventHandler<ViewShowingEventArgs>(application_ViewShowing);
					application = null;
				}
				if(ownerEditFrame != null) {
					NewObjectViewController newObjectViewController = ownerEditFrame.GetController<NewObjectViewController>();
					if(newObjectViewController != null) {
						newObjectViewController.ObjectCreated -= new EventHandler<ObjectCreatedEventArgs>(newObjectViewController_ObjectCreated);
					}
					ownerEditFrame = null;
				}
				foreach(IObjectSpace objectSpace in newObjectSpaces) {
					objectSpace.ModifiedChanged -= new EventHandler(objectSpace_ModifiedChanged);
				}
				newObjectSpaces.Clear();
			}
			base.Dispose(disposing);
		}
		protected override Size CalcFormSizeCore() {
			Size result;
			Control template = (Control)OwnerEdit.Frame.Template;
			if(OwnerEdit.Properties.PopupFormPreferredSize.IsEmpty) {
				result = CalcFormSize(template.PreferredSize);
			}
			else {
				result = new Size(
					Math.Max(template.MinimumSize.Width, OwnerEdit.Properties.PopupFormPreferredSize.Width),
					Math.Max(template.MinimumSize.Height, OwnerEdit.Properties.PopupFormPreferredSize.Height));
			}
			result.Width = Math.Max(OwnerEdit.Size.Width, Math.Max(MinFormSize.Width, result.Width));
			result.Height = Math.Max(MinFormSize.Height, result.Height);
			return result;
		}
		protected override void OnVisibleChanged(EventArgs e) {
			base.OnVisibleChanged(e);
			if(Visible) {
				isUserTakesChoice = false;
			}
		}
		protected internal void SelectObject(Object obj) {
			isInitialization = true;
			try {
				SelectedObject = obj;
			}
			finally {
				isInitialization = false;
			}
		}
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			if(Visible) {
				OwnerEdit.Properties.PopupFormPreferredSize = Size;
			}
		}
		protected override PopupBaseFormViewInfo CreateViewInfo() {
			return new LookupEditPopupFormViewInfo(this);
		}
		public LookupEditPopupForm(LookupEdit ownerEdit) : this(ownerEdit, null) { }
		public LookupEditPopupForm(LookupEdit ownerEdit, LayoutControl buttonsPanel)
			: base(ownerEdit) {
			ownerEditFrame = OwnerEdit.Frame;
			if(ownerEdit.Properties.ShowActionContainersPanel) {
				this.buttonsPanel = buttonsPanel;
				if(this.buttonsPanel != null) {
					this.buttonsPanel.Dock = DockStyle.Bottom;
					Controls.Add(this.buttonsPanel);
				}
			}
			UpdatePaddings();
			listView = (ListView)ownerEditFrame.View;
			NewObjectViewController newObjectViewController = ownerEditFrame.GetController<NewObjectViewController>();
			if(newObjectViewController != null) {
				newObjectViewController.ObjectCreated += new EventHandler<DevExpress.ExpressApp.SystemModule.ObjectCreatedEventArgs>(newObjectViewController_ObjectCreated);
			}
			application = ownerEdit.Properties.Helper.Application;
			application.ViewShowing += new EventHandler<ViewShowingEventArgs>(application_ViewShowing);
			processCurrentObjectController = ownerEditFrame.GetController<ListViewProcessCurrentObjectController>();
			if(processCurrentObjectController != null) {
				processCurrentObjectController.CustomProcessSelectedItem += new EventHandler<CustomProcessListViewSelectedItemEventArgs>(processCurrentObjectController_CustomProcessSelectedItem);
			}
			this.Padding = new Padding(12);
		}
		public override void ShowPopupForm() {
			base.ShowPopupForm();
			FocusFormControl(EmbeddedControl);
			SelectNextControl(EmbeddedControl, true, true, true, true);
			if(buttonsPanel != null) {
				this.buttonsPanel.SendToBack();
			}
		}
		public override void ProcessKeyDown(KeyEventArgs e) {
			if(e.KeyCode == Keys.Tab) {
				return;
			}
			base.ProcessKeyDown(e);
		}
		public void CommitSelectedValue() {
			isUserTakesChoice = true;
			resultValue = listView.GetCurrentTypedObject();
			ClosePopup();
		}
		public override object ResultValue {
			get { return resultValue; }
		}
		public new RepositoryItemLookupEdit Properties {
			get { return (RepositoryItemLookupEdit)base.Properties; }
		}
		public new LookupEdit OwnerEdit {
			get { return (LookupEdit)base.OwnerEdit; }
		}
		public object SelectedObject {
			get {
				return listView.GetCurrentTypedObject();
			}
			set {
				listView.CurrentObject = value;
				if(!isInitialization) {
					isUserTakesChoice = true;
				}
			}
		}
		public ListView ListView {
			get { return listView; }
		}
		public ILookupPopupFrameTemplate Template {
			get { return (ILookupPopupFrameTemplate)ownerEditFrame.Template; }
		}
		public bool IsUserTakesChoice {
			get { return isUserTakesChoice; }
		}
		public LayoutControl ButtonsPanel {
			get { return buttonsPanel; }
		}
	}
	public class RepositoryItemLookupEdit : RepositoryItemPopupContainerEdit, ILookupEditRepositoryItem {
		internal const string EditorName = "LookupEdit";
		public const string ClearButtonIdentifier = "Clear";
		private LookupEditorHelper helper;
		private ViewItem viewItem;
		private bool showActionContainersPanel = true;
		private bool throwInvalidCastException = true;
		protected internal bool ThrowInvalidCastException {
			get { return throwInvalidCastException; }
			set { throwInvalidCastException = value; }
		}
		protected internal ViewItem ViewItem {
			get { return viewItem; }
		}
		static RepositoryItemLookupEdit() {
			Register();
		}
		private bool GetAllowClear(LookupEditorHelper helper) {
			bool result = true;
			if((helper != null) && (helper.Model != null)) {
				result = helper.Model.AllowClear;
			}
			return result;
		}
		protected override bool IsButtonEnabled(EditorButton button) {
			if(ReadOnly && ClearButtonIdentifier.Equals(button.Tag)) {
				return false;
			}
			return base.IsButtonEnabled(button);
		}
		protected override void RaiseCustomDisplayText(DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e) {
			if(helper != null) {
				e.DisplayText = helper.GetDisplayText(e.Value, String.Empty, DisplayFormat.FormatString);
			}
			base.RaiseCustomDisplayText(e);
		}
		public RepositoryItemLookupEdit() {
			TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			ExportMode = ExportMode.DisplayText; 
			ShowPopupCloseButton = false;
			ResetTextEditStyleToStandardInFilterControl = false;
			ActionButtonIndex = 1;
		}
		public override void Assign(RepositoryItem item) {
			base.Assign(item);
			RepositoryItemLookupEdit source = (RepositoryItemLookupEdit)item;
			this.helper = source.helper;
			this.viewItem = source.viewItem;
			this.throwInvalidCastException = source.throwInvalidCastException;
			this.showActionContainersPanel = source.ShowActionContainersPanel;
		}
		public override void CreateDefaultButton() {
			EditorButton clearButton = new EditorButton(ButtonPredefines.Delete);
			clearButton.IsDefaultButton = true;
			clearButton.Tag = ClearButtonIdentifier;
			Buttons.Add(clearButton);
			base.CreateDefaultButton();
		}
		public void Init(ViewItem viewItem, string displayFormat, LookupEditorHelper helper) {
			this.helper = helper;
			this.viewItem = viewItem;
			BeginUpdate();
			this.DisplayFormat.FormatString = displayFormat;
			this.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
			this.EditFormat.FormatString = displayFormat;
			this.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
			bool clearingEnabled = GetAllowClear(helper);
			foreach(EditorButton button in Buttons) {
				if(ClearButtonIdentifier.Equals(button.Tag)) {
					button.Visible = clearingEnabled;
					break;
				}
			}
			AllowNullInput = clearingEnabled ? DevExpress.Utils.DefaultBoolean.True : DevExpress.Utils.DefaultBoolean.False;
			EndUpdate();
		}
		public IModelFormState GetFormStateNode() {
			string viewId = "Default";
			IModelFormState result = null;
			if(helper == null) return result;
			if(helper.LookupListViewModel != null && !string.IsNullOrEmpty(helper.LookupListViewModel.Id)) {
				viewId = helper.LookupListViewModel.Id;
			}
			if(OwnerEdit.Frame != null && OwnerEdit.Frame.Template != null) {
				IModelTemplateWin lookupTemplateModel = helper.GetLookupTemplateModel(OwnerEdit.Frame.Template) as IModelTemplateWin;
				if(lookupTemplateModel != null) {
					result = lookupTemplateModel.FormStates[viewId];
					if(result == null) {
						result = lookupTemplateModel.FormStates.AddNode<IModelFormState>(viewId);
					}
				}
			}
			return result;
		}
		public override bool IsFilterLookUp { get { return true; } }
		public override string EditorTypeName { get { return EditorName; } }
		public new LookupEdit OwnerEdit {
			get { return (LookupEdit)base.OwnerEdit; }
		}
		Type ILookupEditRepositoryItem.LookupObjectType {
			get { return helper.LookupObjectType; }
		}
		string ILookupEditRepositoryItem.DisplayMember {
			get { return helper.DisplayMember != null ? helper.DisplayMember.Name : string.Empty; }
		}
		public LookupEditorHelper Helper {
			get { return helper; }
		}
		public static void Register() {
			if(!EditorRegistrationInfo.Default.Editors.Contains(EditorName)) {
				EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(EditorName, typeof(LookupEdit),
					typeof(RepositoryItemLookupEdit), typeof(PopupBaseEditViewInfo),
					new ButtonEditPainter(), true, EditImageIndexes.LookUpEdit, typeof(DevExpress.Accessibility.PopupEditAccessible)));
			}
		}
		public Size PopupFormPreferredSize {
			get {
				Size size = new Size(0, 0);
				IModelFormState formState = GetFormStateNode();
				if(formState != null) {
					FormStateAndBoundsManager manager = new FormStateAndBoundsManager();
					SettingsStorage storage = new SettingsStorageOnModel(formState);
					size = manager.LoadFormSize(storage, false);
				}
				return size;
			}
			set {
				IModelFormState formState = GetFormStateNode();
				if(formState != null) {
					Size formSize = new Size(value.Width, value.Height);
					SettingsStorage storage = new SettingsStorageOnModel(formState);
					new FormStateAndBoundsManager().SaveFormSize(storage, formSize);
				}
				if(PopupFormPreferredSizeChanged != null) {
					PopupFormPreferredSizeChanged(this, EventArgs.Empty);
				}
			}
		}
		public bool ShowActionContainersPanel {
			get { return showActionContainersPanel; }
			set { showActionContainersPanel = value; }
		}
		public event EventHandler<EventArgs> PopupFormPreferredSizeChanged;
	}
}
