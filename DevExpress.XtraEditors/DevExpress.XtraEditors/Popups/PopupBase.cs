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
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Win;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.Utils.Design;
namespace DevExpress.XtraEditors.Repository {
	[ToolboxItem(false)]
	public class RepositoryItemPopupBase : RepositoryItemButtonEdit, IContextItemCollectionOptionsOwner, IContextItemCollectionOwner, IContextItemProvider {
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete(ObsoleteText.SRObsoletePropertiesText)]
		public new RepositoryItemPopupBase Properties { get { return this; } }
		internal static readonly object queryCloseUp = new object();
		internal static readonly object queryPopUp = new object();
		internal static readonly object closeUp = new object();
		internal static readonly object closed = new object();
		internal static readonly object popup = new object();
		private static readonly object popupAllowClick = new object();
		private static readonly object contextButtonClick = new object();
		private static readonly object contextButtonValueChanged = new object();
		private static readonly object customizeContextItem = new object();
		bool _showPopupShadow;
		KeyShortcut _closeUpKey = new KeyShortcut(Keys.F4);
		ShowDropDown _showDropDown;
		ResizeMode _popupResizeMode;
		int _actionButtonIndex;
		Size popupFormMinSize = Size.Empty, popupFormSize = Size.Empty;
		PopupBorderStyles _popupBorderStyle;
		AppearanceObject appearanceDropDown;
		DefaultBoolean _readOnlyAllowsDropDown; 
		public RepositoryItemPopupBase() {
			this._popupResizeMode = ResizeMode.Default;
			this._popupBorderStyle = PopupBorderStyles.Default;
			this._showPopupShadow = true;
			this._showDropDown = ShowDropDown.SingleClick;
			this._actionButtonIndex = 0;
			this.appearanceDropDown = CreateAppearance("DropDown");
			this._readOnlyAllowsDropDown = DefaultBoolean.Default;
		}
		protected override void DestroyAppearances() {
			DestroyAppearance(AppearanceDropDown);
		}
		public override void Assign(RepositoryItem item) {
			RepositoryItemPopupBase source = item as RepositoryItemPopupBase;
			BeginUpdate(); 
			try {
				base.Assign(item);
				if(source == null) return;
				this.popupFormMinSize = source.PopupFormMinSize;
				this.popupFormSize = source.PopupFormSize;
				this._popupBorderStyle = source.PopupBorderStyle;
				this._actionButtonIndex = source.ActionButtonIndex;
				this._closeUpKey = source.CloseUpKey;
				this._showDropDown = source.ShowDropDown;
				this._popupResizeMode = source.PopupResizeMode;
				this._showPopupShadow = source.ShowPopupShadow;
				this._readOnlyAllowsDropDown = source.AllowDropDownWhenReadOnly;
				this.appearanceDropDown.Assign(source.AppearanceDropDown);
				this.SuppressMouseEventOnOuterMouseClick = source.SuppressMouseEventOnOuterMouseClick;
				this.contextButtons = source.contextButtons;
				this.contextButtonOptions = source.contextButtonOptions;
			} finally {
				EndUpdate();
			}
			Events.AddHandler(closeUp, source.Events[closeUp]);
			Events.AddHandler(closed, source.Events[closed]);
			Events.AddHandler(popup, source.Events[popup]);
			Events.AddHandler(queryCloseUp, source.Events[queryCloseUp]);
			Events.AddHandler(queryPopUp, source.Events[queryPopUp]);
			Events.AddHandler(popupAllowClick, source.Events[popupAllowClick]);
		}
		void ResetAppearanceDropDown() { AppearanceDropDown.Reset(); }
		bool ShouldSerializeAppearanceDropDown() { return AppearanceDropDown.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemPopupBaseAppearanceDropDown"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DXCategory(CategoryName.Appearance)]
		public virtual AppearanceObject AppearanceDropDown { get { return appearanceDropDown; } }
		[Browsable(false)]
		public new PopupBaseEdit OwnerEdit { get { return base.OwnerEdit as PopupBaseEdit; } }
		[Browsable(false)]
		public override string EditorTypeName { get { return "PopupBaseEdit"; } }
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemPopupBaseShowPopupShadow"),
#endif
 DefaultValue(true)]
		public virtual bool ShowPopupShadow {
			get { return _showPopupShadow; }
			set {
				if(ShowPopupShadow == value) return;
				_showPopupShadow = value;
				OnPropertiesChanged();
			}
		}
		[DefaultValue(false)]
		public virtual bool SuppressMouseEventOnOuterMouseClick { get; set; }
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemPopupBaseAllowDropDownWhenReadOnly"),
#endif
 DefaultValue(DefaultBoolean.Default)]
		public virtual DefaultBoolean AllowDropDownWhenReadOnly {
			get { return _readOnlyAllowsDropDown; }
			set {
				if(AllowDropDownWhenReadOnly == value) return;
				_readOnlyAllowsDropDown = value;
				OnPropertiesChanged();
			}
		}
		protected internal virtual bool AllowClosePopup { get { return true; } }
		protected internal virtual bool IsReadOnlyAllowsDropDown {
			get {
				return AllowDropDownWhenReadOnly == DefaultBoolean.True;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int PopupFormWidth {
			get { return popupFormSize.Width; }
			set { PopupFormSize = new Size(value, PopupFormSize.Height); }
		}
		protected virtual bool ShouldSerializePopupFormSize() { return PopupFormSize != Size.Empty; }
		protected virtual void ResetPopupFormSize() { PopupFormSize = Size.Empty; }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemPopupBasePopupFormSize"),
#endif
 DXCategory(CategoryName.Behavior)]
		public virtual Size PopupFormSize {
			get { return popupFormSize; }
			set {
				if(value.Width < 10) value.Width = 0;
				if(value.Height < 10) value.Height = 0;
				if(PopupFormSize == value) return;
				popupFormSize = value;
				OnPopupFormSizeChanged();
			}
		}
		protected virtual void OnPopupFormSizeChanged() {
			OnPropertiesChanged();
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemPopupBasePopupFormMinSize"),
#endif
 DXCategory(CategoryName.Behavior)]
		public virtual Size PopupFormMinSize {
			get { return popupFormMinSize; }
			set {
				if(value.Width < 10) value.Width = 0;
				if(value.Height < 10) value.Height = 0;
				if(popupFormMinSize == value) return;
				popupFormMinSize = value;
				OnPopupFormSizeChanged();
			}
		}
		protected internal Size GetDesiredPopupFormSize(bool forceMinSize) {
			Size res = PopupFormSize;
			if(forceMinSize) {
				res.Width = Math.Max(res.Width, PopupFormMinSize.Width);
				res.Height = Math.Max(res.Height, PopupFormMinSize.Height);
			}
			return ScaleSize(res);
		}
		protected virtual bool ShouldSerializePopupFormMinSize() { return PopupFormMinSize != Size.Empty; }
		protected virtual void ResetPopupFormMinSize() { PopupFormMinSize = Size.Empty; }
		bool ShouldSerializePopupBorderStyle() { return StyleController == null && PopupBorderStyles.Default != PopupBorderStyle; }
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemPopupBasePopupBorderStyle")
#else
	Description("")
#endif
]
		public virtual PopupBorderStyles PopupBorderStyle { 
			get { 
				if(StyleController != null) return StyleController.PopupBorderStyle;
				return _popupBorderStyle; 
			}
			set {
				if(PopupBorderStyle == value) return;
				_popupBorderStyle = value;
				OnPropertiesChanged();
			}
		}
		bool ShouldSerializeCloseUpKey() { return CloseUpKey.Key != Keys.F4; }
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemPopupBaseCloseUpKey"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Editor("DevExpress.XtraEditors.Design.EditorButtonShortcutEditor, " + AssemblyInfo.SRAssemblyEditorsDesign, typeof(System.Drawing.Design.UITypeEditor))]
		public virtual KeyShortcut CloseUpKey { 
			get { return _closeUpKey; }
			set {
				if(CloseUpKey == value) return;
				_closeUpKey = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemPopupBasePopupResizeMode"),
#endif
 DefaultValue(ResizeMode.Default)]
		public virtual ResizeMode PopupResizeMode {
			get { return _popupResizeMode; }
			set {
				if (PopupResizeMode == value) return;
				_popupResizeMode = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemPopupBaseShowDropDown"),
#endif
 DefaultValue(ShowDropDown.SingleClick)]
		public virtual ShowDropDown ShowDropDown { 
			get { return _showDropDown; }
			set {
				if(ShowDropDown == value) return;
				_showDropDown = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemPopupBaseActionButtonIndex"),
#endif
 DefaultValue(0)]
		public virtual int ActionButtonIndex { 
			get { return _actionButtonIndex; }
			set {
				if(!IsLoading && value > Buttons.Count - 1) value = Buttons.Count - 1; 
				if(value < 1) value = 0;
				if(ActionButtonIndex == value) return;
				_actionButtonIndex = value;
				OnPropertiesChanged();
			}
		}
		public override void CreateDefaultButton() {
			EditorButton btn = new EditorButton(ButtonPredefines.Combo);
			btn.IsDefaultButton = true;
			Buttons.Add(btn);
		}
		protected internal virtual bool ForceDisableButtonOnReadOnly { get { return !IsReadOnlyAllowsDropDown; } }
		protected internal override bool IsButtonEnabled(EditorButton button) {
			if(!base.IsButtonEnabled(button)) return false;
			if(ReadOnly && ForceDisableButtonOnReadOnly) {
				if(button.Index == ActionButtonIndex) return false;
			}
			return true;
		}
		protected override bool AllowCheckExtraKeys { get { return base.AllowCheckExtraKeys && (OwnerEdit != null && !OwnerEdit.IsPopupOpen); } }
		protected internal override bool NeededKeysContains(Keys key) {
			if(CloseUpKey.IsExist && key == CloseUpKey.Key)
				return true;
			if(key == (Keys.Down | Keys.Alt))
				return true;
			if(OwnerEdit != null && OwnerEdit.IsPopupOpen) {
				if(NeededKeysPopupContains(key))
					return true;
			}
			return base.NeededKeysContains(key);
		}
		protected internal override bool ActivationKeysContains(Keys key) {
			if(CloseUpKey.IsExist && key == CloseUpKey.Key)
				return true;
			if(key == (Keys.Down | Keys.Alt))
				return true;
			return base.ActivationKeysContains(key);
		}
		protected virtual bool NeededKeysPopupContains(Keys key) {
			if(AllowCloseByEscape && key == Keys.Escape)
				return true;
			return false;
		}
		protected internal override bool AllowValidateOnEnterKey { get { return base.AllowValidateOnEnterKey && (OwnerEdit != null && !OwnerEdit.IsPopupOpen); } }
		protected internal override bool IsNowReadOnly { get { return base.IsNowReadOnly || (!AllowInputOnOpenPopup && OwnerEdit != null && OwnerEdit.IsPopupOpen); } }
		protected internal virtual bool AllowInputOnOpenPopup { get { return true; } }
		protected internal virtual bool AllowCloseByEscape { get { return true; } }
		protected internal virtual bool CancelPopupInputOnButtonClose { get { return false; } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemPopupBaseQueryCloseUp"),
#endif
 DXCategory(CategoryName.Events)]
		public virtual event CancelEventHandler QueryCloseUp {
			add { this.Events.AddHandler(queryCloseUp, value); }
			remove { this.Events.RemoveHandler(queryCloseUp, value); }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemPopupBaseQueryPopUp"),
#endif
 DXCategory(CategoryName.Events)]
		public virtual event CancelEventHandler QueryPopUp {
			add { this.Events.AddHandler(queryPopUp, value); }
			remove { this.Events.RemoveHandler(queryPopUp, value); }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemPopupBaseCloseUp"),
#endif
 DXCategory(CategoryName.Events)]
		public virtual event CloseUpEventHandler CloseUp {
			add { this.Events.AddHandler(closeUp, value); }
			remove { this.Events.RemoveHandler(closeUp, value); }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemPopupBaseClosed"),
#endif
 DXCategory(CategoryName.Events)]
		public virtual event ClosedEventHandler Closed {
			add { this.Events.AddHandler(closed, value); }
			remove { this.Events.RemoveHandler(closed, value); }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemPopupBasePopup"),
#endif
 DXCategory(CategoryName.Events)]
		public virtual event EventHandler Popup {
			add { this.Events.AddHandler(popup, value); }
			remove { this.Events.RemoveHandler(popup, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public event PopupAllowClickEventHandler PopupAllowClick {
			add { this.Events.AddHandler(popupAllowClick, value); }
			remove { this.Events.RemoveHandler(popupAllowClick, value); }
		}
		protected internal virtual void RaiseQueryCloseUp(CancelEventArgs e) {
			CancelEventHandler handler = (CancelEventHandler)this.Events[queryCloseUp];
			if(handler != null) handler(GetEventSender(), e);
		}
		protected internal virtual void RaiseQueryPopUp(CancelEventArgs e) {
			CancelEventHandler handler = (CancelEventHandler)this.Events[queryPopUp];
			if(handler != null) handler(GetEventSender(), e);
		}
		protected internal virtual void RaiseCloseUp(CloseUpEventArgs e) {
			CloseUpEventHandler handler = (CloseUpEventHandler)this.Events[closeUp];
			if(handler != null) handler(GetEventSender(), e);
		}
		protected internal virtual void RaiseClosed(ClosedEventArgs e) {
			ClosedEventHandler handler = (ClosedEventHandler)this.Events[closed];
			if(handler != null) handler(GetEventSender(), e);
		}
		protected internal virtual void RaisePopup(EventArgs e) {
			EventHandler handler = (EventHandler)this.Events[popup];
			if(handler != null) handler(GetEventSender(), e);
		}
		protected internal virtual void RaisePopupAllowClick(PopupBaseEdit popupBaseEdit, PopupAllowClickEventArgs e) {
			PopupAllowClickEventHandler handler = (PopupAllowClickEventHandler)this.Events[popupAllowClick];
			if(handler != null)
				handler(popupBaseEdit, e);
		}
		ContextItemCollection contextButtons;
		[Editor("DevExpress.XtraEditors.Design.SimpleContextItemCollectionUITypeEditor, " + AssemblyInfo.SRAssemblyEditorsDesign, typeof(System.Drawing.Design.UITypeEditor)), 
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Browsable(false)]
		public virtual ContextItemCollection ContextButtons {
			get {
				if(contextButtons == null) {
					contextButtons = CreateContextButtonsCollection();
					contextButtons.Options = ContextButtonOptions;
				}
				return contextButtons;
			}
		}
		SimpleContextItemCollectionOptions contextButtonOptions;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter(typeof(ExpandableObjectConverter)), Browsable(false)]
		public virtual SimpleContextItemCollectionOptions ContextButtonOptions {
			get {
				if(contextButtonOptions == null) {
					contextButtonOptions = CreateContextButtonOptions();
				}
				return contextButtonOptions;
			}
		}
		protected virtual SimpleContextItemCollectionOptions CreateContextButtonOptions() {
			return new SimpleContextItemCollectionOptions(this);
		}
		protected virtual ContextItemCollection CreateContextButtonsCollection() {
			return new ContextItemCollection(this);
		}
		ContextAnimationType IContextItemCollectionOptionsOwner.AnimationType {
			get { return ContextAnimationType.OpacityAnimation; }
		}
		void IContextItemCollectionOptionsOwner.OnOptionsChanged(string propertyName, object oldValue, object newValue) {
			OnPropertiesChanged();
		}
		bool IContextItemCollectionOwner.IsDesignMode {
			get { return IsDesignMode; }
		}
		bool IContextItemCollectionOwner.IsRightToLeft {
			get {
				if(OwnerEdit != null) return OwnerEdit.IsRightToLeft;
				return false;
			}
		}
		void IContextItemCollectionOwner.OnCollectionChanged() {
			OnPropertiesChanged();
		}
		void IContextItemCollectionOwner.OnItemChanged(ContextItem item, string propertyName, object oldValue, object newValue) {
			if(propertyName == "Visibility" || propertyName == "Value") {
				if(OwnerEdit != null) {
					OwnerEdit.Invalidate();
					OwnerEdit.Update();
				}
				return;
			}
			OnPropertiesChanged();
		}
		ContextItem IContextItemProvider.CreateContextItem(Type type) {
			ContextItem item = CreateContextItemCore(type);
			if(item == null) return null;
			item.Name = item.GetType().Name;
			return item;
		}
		ContextItem CreateContextItemCore(Type type) {
			if(type == typeof(ContextButton)) return new SimpleContextButton();
			if(type == typeof(RatingContextButton)) return new SimpleRatingContextButton();
			if(type == typeof(CheckContextButton)) return new SimpleCheckContextButton();
			if(type == typeof(TrackBarContextButton)) return new SimpleTrackBarContextButton();
			return null;
		}
		public event ContextItemClickEventHandler ContextButtonClick {
			add { Events.AddHandler(contextButtonClick, value); }
			remove { Events.RemoveHandler(contextButtonClick, value); }
		}
		protected internal void RaiseContextButtonClick(ContextItemClickEventArgs e) {
			ContextItemClickEventHandler handler = Events[contextButtonClick] as ContextItemClickEventHandler;
			if(handler != null)
				handler(this, e);
		}
		public event ContextButtonValueChangedEventHandler ContextButtonValueChanged {
			add { Events.AddHandler(contextButtonValueChanged, value); }
			remove { Events.RemoveHandler(contextButtonValueChanged, value); }
		}
		protected internal void RaiseContextButtonValueChanged(ContextButtonValueEventArgs e) {
			ContextButtonValueChangedEventHandler handler = Events[contextButtonValueChanged] as ContextButtonValueChangedEventHandler;
			if(handler != null)
				handler(this, e);
		}
		public event ListBoxControlContextButtonCustomizeEventHandler CustomizeContextItem {
			add { Events.AddHandler(customizeContextItem, value); }
			remove { Events.RemoveHandler(customizeContextItem, value); }
		}
		protected internal void RaiseCustomizeContextItem(ListBoxControlContextButtonCustomizeEventArgs e) {
			ListBoxControlContextButtonCustomizeEventHandler handler = Events[customizeContextItem] as ListBoxControlContextButtonCustomizeEventHandler;
			if(handler != null)
				handler(this, e);
		}
	}
}
namespace DevExpress.XtraEditors {
	public interface IPopupHelperController {
		IPopupHelper PopupHelper { get; set; }
	}
	public interface IPopupHelper {
		void HidePopupForm(PopupBaseForm popupForm);
	}
	public enum PopupCloseMode { Normal, Cancel, Immediate, ButtonClick, CloseUpKey }
	[SmartTagFilter(typeof(PopupBaseEditFilter))]
	public abstract class PopupBaseEdit : ButtonEdit, IPopupControlEx, IPopupHelperController {
		PopupBaseForm _popupForm;
		IPopupHelper popupHelper;
		Rectangle altBounds;
		Size storePopupFormSize = Size.Empty;
		bool isPopupOpened = false;
		PopupCloseMode popupCloseModeCore;
		public PopupBaseEdit() {
			this.popupHelper = null;
			this._popupForm = null;
			this.altBounds = Rectangle.Empty;
			this.popupCloseModeCore = PopupCloseMode.Immediate;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual PopupCloseMode PopupCloseMode {
			get { return popupCloseModeCore; }
			set {
				if(PopupCloseMode == value) return;
				popupCloseModeCore = value;
			}
		}
		IPopupHelper IPopupHelperController.PopupHelper { get { return popupHelper; } set { popupHelper = value; } }
		protected internal Rectangle AltBounds { get { return altBounds; } set { altBounds = value; } }
		protected override void Dispose(bool disposing) {
			fDisposing = true;
			if(disposing) {
				ClosePopup(PopupCloseMode.Immediate);
				DestroyPopupForm();
			}
			base.Dispose(disposing);
		}
		void IPopupControl.ClosePopup() {
			if(IsHandleCreated) {
				BeginInvoke(new MethodInvoker(delegate() {
			ClosePopup(PopupCloseMode);
				}));
			}
			else
				ClosePopup(PopupCloseMode);
		}
		bool IPopupControl.SuppressOutsideMouseClick {
			get { return Properties.SuppressMouseEventOnOuterMouseClick; }
		} 
		bool IPopupControl.AllowMouseClick(Control control, Point mousePosition) {
			if(PopupForm == null) return false;
			return PopupForm.AllowMouseClick(control, mousePosition);
		}
		void IPopupControlEx.WindowHidden(Control activeControl) {
			if(PopupForm == null) return;
			Form form = FindForm();
			if(form != null && form.IsHandleCreated && form == activeControl) ClosePopup(PopupCloseMode.Immediate);
		}
		void IPopupControlEx.CheckClosePopup(Control activeControl) {
			if(PopupForm == null) return;
			PopupForm.CheckClosePopup(activeControl);
		}
		internal ResizeMode CurrentPopupResizeMode {
			get {
				if (Properties.PopupResizeMode == ResizeMode.Default) {
					if (DevExpress.Utils.Drawing.Helpers.NativeVista.IsVista) 
						return ResizeMode.LiveResize;
					else
						return ResizeMode.FrameResize;
				}
				return Properties.PopupResizeMode;
			}
		}
		protected internal override bool AllowSmartMouseWheel { get { return base.AllowSmartMouseWheel && !IsPopupOpen; } }
		Control IPopupControl.PopupWindow { get { return PopupForm; } }
		protected internal virtual PopupBaseForm PopupForm { get { return _popupForm; } }
		[Browsable(false)]
		public override string EditorTypeName { get { return "PopupBaseEdit"; } }
		[DXCategory(CategoryName.Properties), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("PopupBaseEditProperties"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new RepositoryItemPopupBase Properties { get { return base.Properties as RepositoryItemPopupBase; } }
		protected virtual bool CanShowPopup { 
			get {
				if(Properties.ReadOnly && Properties.ForceDisableButtonOnReadOnly) return false;
				return true; 
			} 
		} 
		protected internal override void SetEmptyEditValue(object emptyEditValue) {
			if(PopupForm != null) PopupForm.ResetResultValue();
			base.SetEmptyEditValue(emptyEditValue);
		}
		protected internal override void OnPropertiesChanged() {
			OnPropertiesChanged_SyncPopup();
			base.OnPropertiesChanged();
		}
		protected virtual bool IsNeedHideCursorOnPopup { get { return false; } }
		protected internal virtual bool AllowPopupTabOut { get { return true; } }
		protected internal virtual void ProcessPopupTabKey(KeyEventArgs e) {
			ClosePopup(PopupCloseMode.Normal);
			if(IsPopupOpen) return;
			if(InplaceType == InplaceType.Grid) return;
			base.ProcessDialogKey(e.KeyData);
		}
		protected virtual void OnPropertiesChanged_SyncPopup() {
			ClosePopup(PopupCloseMode.Immediate);
			if(InplaceType == InplaceType.Standalone) DestroyPopupForm();
		}
		public override bool IsNeededKey(KeyEventArgs e) {
			if(base.IsNeededKey(e)) return true;
			if(!IsPopupOpen) return false;
			return true;
		}
		[Browsable(false)]
		public virtual bool IsPopupOpen { get { return PopupForm != null && this.isPopupOpened; } }
		public virtual void ShowPopup() {
			if(IsPopupOpen) return;
			CancelEventArgs e = new CancelEventArgs();
			Properties.RaiseQueryPopUp(e);
			if(e.Cancel || !CanShowPopup) return;
			DoShowPopup();
		}
		public virtual void ClosePopup() {
			ClosePopup(PopupCloseMode.Normal);
		}
		public virtual void CancelPopup() {
			ClosePopup(PopupCloseMode.Cancel);
		}
		protected virtual void SwitchPopupState() {
			if(IsPopupOpen) ClosePopup();
			else ShowPopup();
		}
		protected override void OnEditorKeyPress(KeyPressEventArgs e) {
			base.OnEditorKeyPress(e);
			if(!e.Handled && IsPopupOpen) {
				PopupForm.ProcessKeyPress(e);
			}
		}
		protected override void OnEditorKeyUp(KeyEventArgs e) {
			base.OnEditorKeyUp(e);
			if(!e.Handled) {
				if(IsPopupOpen)
					PopupForm.ProcessKeyUp(e);
			}
		}
		protected override void OnEditorKeyDown(KeyEventArgs e) {
			base.OnEditorKeyDown(e);
			if(e.Handled) return;
			if(e.KeyData == (Keys.Down | Keys.Alt) || (Properties.CloseUpKey.IsExist && e.KeyData == Properties.CloseUpKey.Key)) {
				if(IsPopupOpen) {
					if(Properties.CancelPopupInputOnButtonClose) 
						CancelPopup();
					else
						ClosePopup(PopupCloseMode.CloseUpKey);
				} else
					ShowPopup();
				e.Handled = true;
			}
			if(!e.Handled) {
				if(IsPopupOpen) {
					PopupForm.ProcessKeyDown(e);
				}
			}
		}
		protected override bool ProcessDialogKey(Keys keyData) {
			if(IsPopupOpen) {
				if(keyData == (Keys.Control | Keys.Tab)) return true;
				return false;
			}
			return base.ProcessDialogKey(keyData);
		}
		protected internal override bool SuppressMouseWheel(MouseEventArgs e) {
			if(IsPopupOpen)
				return true;
			return false;
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(e);
			if(ee.Handled) {
				base.OnMouseDown(ee);
				return;
			}
			EditHitInfo hitInfo = ViewInfo.CalcHitInfo(new Point(e.X, e.Y));
			if(e.Button == MouseButtons.Left) {
				if(hitInfo.HitTest == EditHitTest.MaskBox || hitInfo.HitTest == EditHitTest.Bounds) {
					if(!IsPopupOpen) {
						bool showPopup = false;
						if(Properties.ShowDropDown == ShowDropDown.DoubleClick && e.Clicks == 2) showPopup = true;
						if(Properties.ShowDropDown == ShowDropDown.SingleClick && e.Clicks == 1 && !IsMaskBoxAvailable) showPopup = true;
						if(showPopup) {
							if(!GetValidationCanceled()) {
								ee.Handled = true;
								BeginInvoke(new MethodInvoker(() => {
									ShowPopup();
								}));
							}
						}
					}
					else {
						OnMouseDownClosePopup();
					}
				}
			}
			base.OnMouseDown(ee);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public event PopupAllowClickEventHandler PopupAllowClick {
			add { Properties.PopupAllowClick += value; }
			remove { Properties.PopupAllowClick -= value; }
		}
		protected virtual void OnMouseDownClosePopup() {
			ClosePopup();
		}
		protected override void OnRightToLeftChanged() {
			base.OnRightToLeftChanged();
			DestroyPopupForm();
		}
		protected virtual bool AllowCloseByEscape { get { return true; } }
		protected abstract PopupBaseForm CreatePopupForm();
		protected virtual void DestroyPopupFormCore(bool dispose) {
			if(PopupForm != null) {
				SubscribePopupFormEvents(false);
				ServiceObject.PopupClosed(this);
				Form popupForm = PopupForm;
				this._popupForm = null;
				popupForm.Disposed -= new EventHandler(OnPopupForm_Disposed);
				if(dispose) popupForm.Dispose();
			}
		}
		protected override void OnMove(EventArgs e) {
			ClosePopup(PopupCloseMode.Immediate);
			base.OnMove(e);
		}
		protected internal void DestroyPopupForm() {
			DestroyPopupFormCore(true);
		}
		protected virtual void SubscribePopupFormEvents(bool subscribe) {
			if(PopupForm == null) return;
			if(subscribe) {
				PopupForm.ResultValueChanged += new EventHandler(OnPopupForm_ResultValueChanged);
			} else {
				PopupForm.ResultValueChanged -= new EventHandler(OnPopupForm_ResultValueChanged);
			}
		}
		void OnPopupForm_Disposed(object sender, EventArgs e) {
			DestroyPopupFormCore(false);
		}
		void OnPopupForm_ResultValueChanged(object sender, EventArgs e) {
			OnPopupFormValueChanged();
		}
		protected virtual void OnPopupFormValueChanged() {
		}
		protected internal virtual Size CalcPopupFormSize() { 
			if(PopupForm == null) return Size.Empty;
			return PopupForm.CalcFormSize();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void ForceClosePopup() { ClosePopup(PopupCloseMode.Immediate); }
		protected internal virtual void ClosePopup(PopupCloseMode closeMode) {
			if(!IsPopupOpen) return;
			if(closeMode != PopupCloseMode.Immediate) {
				CancelEventArgs e = new CancelEventArgs();
				Properties.RaiseQueryCloseUp(e);
				if(e.Cancel) return;
			}
			DoClosePopup(closeMode);
		}
		protected internal override void OnEndIme() {
			base.OnEndIme();
			if(IsPopupOpen && PopupForm != null) PopupForm.FocusFormControl(null);
		}
		[Browsable(false)]
		public override bool EditorContainsFocus { 
			get { 
				return base.EditorContainsFocus || ( PopupForm != null && PopupForm.FormContainsFocus); 
			} 
		}
		protected override void OnLostFocus(EventArgs e) {
			if(IsPopupOpen && !EditorContainsFocus && Properties.AllowClosePopup) {
				ClosePopup(PopupCloseMode);
			}
			base.OnLostFocus(e);
		}
		protected internal virtual PopupBaseForm GetPopupForm() {
			if(PopupForm == null) {
				this._popupForm = GetPopupFormCore();
				if(this._popupForm == null) return null;
				this._popupForm.RightToLeft = WindowsFormsSettings.GetRightToLeft(this);
				this._popupForm.Disposed += new EventHandler(OnPopupForm_Disposed);
			}
			return PopupForm;
		}
		protected virtual PopupBaseForm GetPopupFormCore() {
			return CreatePopupForm();
		}
		protected internal virtual Rectangle CalcPopupFormBounds(Size size) {
			Point editorLocation = this.PointToScreen(Point.Empty);
			editorLocation.Offset(Properties.PopupOffset.X, Properties.PopupOffset.Y);
			Point bottomPoint = new Point(editorLocation.X, editorLocation.Y + (AltBounds.IsEmpty ? this.Height : AltBounds.Height));
			Point newLoc = Point.Empty;
			Form parentForm = FindForm();
			if(BlobBasePopupForm.ShowPopupWithinParentForm) {
				BoundsEventArgs e = new BoundsEventArgs();
				if(parentForm != null)
					e.Bounds = parentForm.Bounds;
				BlobBasePopupForm.RaiseGetPopupParentBounds(this, e);
				if(!e.Bounds.IsEmpty)
					newLoc = DevExpress.Utils.ControlUtils.CalcLocation(bottomPoint, editorLocation, size, this.Size, IsRightToLeft, e.Bounds);
				else
					newLoc = DevExpress.Utils.ControlUtils.CalcLocation(bottomPoint, editorLocation, size, this.Size, IsRightToLeft);
			}
			else
				newLoc = DevExpress.Utils.ControlUtils.CalcLocation(bottomPoint, editorLocation, size, this.Size, IsRightToLeft);
			return ConstrainFormBounds(new Rectangle(newLoc, size));
		}
		protected virtual Rectangle ConstrainFormBounds(Rectangle r) {
			return ControlUtils.ConstrainFormBounds(this, r);
		}
		protected void KeepPopupFormSize(Size size) {
			storePopupFormSize = size;
		}
		protected internal virtual void RefreshPopup() {
			if(!IsPopupOpen) return;
			PopupBaseForm form = PopupForm;
			if(form == null) return;
			Size size = CalcPopupFormSize();
			if(size.IsEmpty) size = form.ClientSize;
			Rectangle bounds = CalcPopupFormBounds(size);
			form.ClientSize = bounds.Size;
			form.Location = bounds.Location;
			form.UpdateViewInfo(null);
		}
		protected virtual void DoShowPopup() {
			if(Parent == null) return; 
			PopupBaseForm form = GetPopupForm();
			if(form == null) return;
			form.OnBeforeShowPopup();
			Size size = CalcPopupFormSize();
			if(size.IsEmpty) return;
			this.isPopupOpened = true;
			if(storePopupFormSize != Size.Empty) 
				size = storePopupFormSize; 
			SubscribePopupFormEvents(true);
			Rectangle bounds = CalcPopupFormBounds(size);
			form.ClientSize = bounds.Size;
			form.Location = bounds.Location;
			form.ForceCreateHandle();
			form.UpdateViewInfo(null);
			if(Parent != null) Parent.VisibleChanged += new EventHandler(OnParent_VisibleChanged);
			bool prevLockFormat = Properties.LockFormatParse;
			try {
				Properties.LockFormatParse = false;
				form.ShowPopupForm();
			}
			finally {
				if(prevLockFormat)
					Properties.LockFormatParse = true;
			}
			ServiceObject.PopupShowing(this);
			if(PopupForm != null) {
				Form parentForm = FindForm();
				if(parentForm != null) parentForm.AddOwnedForm(PopupForm);
			}
			RefreshVisualLayout();
			if(PopupForm != null && IsNeedHideCursorOnPopup) HideCaret();
			OnPopupShown();
		}
		protected override void OnVisibleChanged(EventArgs e) {
			base.OnVisibleChanged(e);
			if(!Visible) ClosePopup(PopupCloseMode.Cancel);
		}
		protected virtual bool IsAcceptCloseMode(PopupCloseMode closeMode) {
			return closeMode != PopupCloseMode.Cancel;
		}
		public virtual void RefreshEditValue() {
			object val = GetPopupForm().QueryResultValue();
			BeginInternalTextChange();
			try {
				EditValue = val;
			} finally {
				EndInternalTextChange();
			}
		}
		protected virtual void DoClosePopup(PopupCloseMode closeMode) {
			if(!IsPopupOpen) return;
			if(Parent != null) Parent.VisibleChanged -= new EventHandler(OnParent_VisibleChanged);
			SubscribePopupFormEvents(false);
			object val = !IsAcceptCloseMode(closeMode) ? PopupForm.QueryOldEditValue() : PopupForm.QueryResultValue();
			bool isPopupFormActive = PopupForm == Form.ActiveForm;
			PopupBaseForm form = PopupForm;
			if(this.popupHelper != null) 
				this.popupHelper.HidePopupForm(form);
			else
				form.HidePopupForm();
			this.isPopupOpened = false;
			ServiceObject.PopupClosed(this);
			CloseUpEventArgs e = new CloseUpEventArgs(val, IsAcceptCloseMode(closeMode), closeMode);
			OnPopupClosing(e);
			UpdateEditValueOnClose(closeMode, e.AcceptValue, e.Value, val);
			if(IsNeedHideCursorOnPopup) ShowCaret();
			RefreshVisualLayout();
			Form parentForm = FindForm();
			if(PopupForm != null) {
				if(parentForm != null) parentForm.RemoveOwnedForm(PopupForm);
			}
			if(parentForm != null && isPopupFormActive) parentForm.Activate();
			OnPopupClosed(closeMode);
		}
		protected virtual void UpdateEditValueOnClose(PopupCloseMode closeMode, bool acceptValue, object newValue, object oldValue) {
			if(!Properties.ReadOnly) {
				if(acceptValue) {
					AcceptPopupValue(newValue);
				} else {
					if(!IsAcceptCloseMode(closeMode)) {
						BeginInternalTextChange();
						try {
							EditValue = oldValue;
						} finally {
							EndInternalTextChange();
						}
					}
				}
			}
		}
		protected virtual void OnPopupShown() { 
			Properties.RaisePopup(EventArgs.Empty);
		}
		protected virtual void OnPopupClosing(CloseUpEventArgs e) {
			Properties.RaiseCloseUp(e);
		}
		protected virtual void OnPopupClosed(PopupCloseMode closeMode) {
			Properties.RaiseClosed(new ClosedEventArgs(closeMode));
		}
		void OnParent_VisibleChanged(object sender, EventArgs e) {
			ClosePopup(PopupCloseMode.Cancel);
		}
		protected virtual void AcceptPopupValue(object val) {
			if(!CompareEditValue(EditValue, val, true)) Properties.LockFormatParse = true;
			BeginInternalTextChange();
			try {
				BeginAcceptEditValue();
				try {
					EditValue = val;
				}
				finally {
					EndAcceptEditValue();
				}
			}
			finally {
				EndInternalTextChange();
			}
		}
		protected override void DoNullInputKeysCore(KeyEventArgs e) {
			CancelPopup();
			base.DoNullInputKeysCore(e);
		}
		protected override void OnParentVisibleChanged(EventArgs e) {
			base.OnParentVisibleChanged(e);
			ClosePopup(PopupCloseMode.Cancel);
		}
		protected virtual void ActionShowPopup(EditorButtonObjectInfoArgs buttonInfo) {
			if(IsPopupOpen) {
				DevExpress.XtraEditors.ViewInfo.EditHitInfo hi = ViewInfo.CalcHitInfo(new System.Drawing.Point(buttonInfo.Bounds.X + 1, buttonInfo.Bounds.Y + 1));
				ClearHotPressed(hi);
				if(Properties.CancelPopupInputOnButtonClose)
					CancelPopup();
				else
					ClosePopup(PopupCloseMode.ButtonClick);
			}
			else {
				ShowPopup();
			}
		}
		protected virtual void OnDefaultPressButton(EditorButtonObjectInfoArgs buttonInfo) {
			if(IsActionButton(buttonInfo))
				ActionShowPopup(buttonInfo);
		}
		protected override void OnPressButton(EditorButtonObjectInfoArgs buttonInfo) {
			OnDefaultPressButton(buttonInfo);
			base.OnPressButton(buttonInfo);
		}
		protected virtual bool IsActionButton(EditorButtonObjectInfoArgs buttonInfo) {
			return Properties.ActionButtonIndex >= 0 && Properties.Buttons.IndexOf(buttonInfo.Button) == Properties.ActionButtonIndex;
		}
		public override bool AllowMouseClick(Control control, Point p) {
			bool res = base.AllowMouseClick(control, p);
			if(res) return true;
			if(!IsPopupOpen) return false;
			return PopupForm.AllowMouseClick(control, p);
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("PopupBaseEditQueryCloseUp"),
#endif
 DXCategory(CategoryName.Events)]
		public virtual event CancelEventHandler QueryCloseUp {
			add { Properties.QueryCloseUp += value; }
			remove { Properties.QueryCloseUp -= value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("PopupBaseEditPopup"),
#endif
 DXCategory(CategoryName.Events)]
		public virtual event EventHandler Popup {
			add { Properties.Popup += value; }
			remove { Properties.Popup -= value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("PopupBaseEditQueryPopUp"),
#endif
 DXCategory(CategoryName.Events)]
		public virtual event CancelEventHandler QueryPopUp {
			add { Properties.QueryPopUp += value; }
			remove { Properties.QueryPopUp -= value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("PopupBaseEditCloseUp"),
#endif
 DXCategory(CategoryName.Events)]
		public virtual event CloseUpEventHandler CloseUp {
			add { Properties.CloseUp += value; }
			remove { Properties.CloseUp -= value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("PopupBaseEditClosed"),
#endif
 DXCategory(CategoryName.Events)]
		public virtual event ClosedEventHandler Closed {
			add { Properties.Closed += value; }
			remove { Properties.Closed -= value; }
		}
		protected internal virtual void RaisePopupAllowClick(PopupAllowClickEventArgs e) {
			Properties.RaisePopupAllowClick(this, e);
		}
	}
}
namespace DevExpress.XtraEditors.ViewInfo {
	public class PopupBaseEditViewInfo : ButtonEditViewInfo {
		public PopupBaseEditViewInfo(RepositoryItem item) : base(item) {
		}
		[Browsable(false)]
		public new PopupBaseEdit OwnerEdit { get { return base.OwnerEdit as PopupBaseEdit; } }
		public new RepositoryItemPopupBase Item { get { return base.Item as RepositoryItemPopupBase; } }
		public override bool DrawFocusRect {
			get {
				if(OwnerEdit != null && Item.TextEditStyle == TextEditStyles.DisableTextEditor) {
					return (Item.AllowFocused && OwnerEdit.EditorContainsFocus && !OwnerEdit.IsPopupOpen);
				}
				return false;
			}
		}
		protected bool IsPopupOpen {
			get { return OwnerEdit != null && OwnerEdit.IsPopupOpen; }
		}
		protected override bool IsAnyButtonPressed(EditHitInfo hitInfo) {
			return base.IsAnyButtonPressed(hitInfo) || IsPopupOpen;
		}
		protected override bool IsButtonPressed(EditorButtonObjectInfoArgs info) {
			return base.IsButtonPressed(info) || ( IsPopupOpen && Item.ActionButtonIndex == info.Button.Index);
		}
		protected override bool IsHotEdit(EditHitInfo hitInfo) {
			return base.IsHotEdit(hitInfo) || IsPopupOpen;
		}
	}
}
