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
using System.Collections;
using System.Runtime.InteropServices;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Popup;
namespace DevExpress.Accessibility {
	public class MessageBoxAccessible : ContainerBaseAccessible {
		public MessageBoxAccessible(XtraMessageBoxForm form) : base(form) {	}
		public new XtraMessageBoxForm Control { get { return base.Control as XtraMessageBoxForm; } }
		protected override ChildrenInfo GetChildrenInfo() {
			ChildrenInfo res = base.GetChildrenInfo();
			if(((XtraMessageBoxArgs)(Control.Message)).Icon != null) res["Icon"] = 1;
			res[ChildType.Text] = 1;
			return res;
		}
		public override AccessibleObject Parent { get { return Control.GetParentAccessible(); } }
		protected override AccessibleRole GetRole() { return AccessibleRole.Dialog; }
		protected override string GetName() { return Control.Message.Caption; }
		protected override void OnChildrenCountChanged() {
			if(((XtraMessageBoxArgs)Control.Message).Icon != null) AddChild(new MessageIconAccessible(Control));
			AddChild(new MessageLabelAccessible(Control));
			base.OnChildrenCountChanged();
		}
		protected class MessageLabelAccessible : BaseAccessible {
			XtraMessageBoxForm form;
			public MessageLabelAccessible(XtraMessageBoxForm form) {
				this.form = form;
			}
			public override Rectangle ClientBounds { get { return form.MessageRectangle; } }
			protected override AccessibleRole GetRole() { return AccessibleRole.StaticText; } 
			protected override string GetName() { return ((XtraMessageBoxArgs)form.Message).Text; }
		}
		protected class MessageIconAccessible : BaseAccessible {
			XtraMessageBoxForm form;
			public MessageIconAccessible(XtraMessageBoxForm form) {
				this.form = form;
			}
			public override Rectangle ClientBounds { get { return form.IconRectangle; } }
			protected override AccessibleRole GetRole() { return AccessibleRole.Graphic; } 
		}
	}
	public class MessageDialogAccessible : ContainerBaseAccessible {
		public MessageDialogAccessible(XtraDialogForm form) : base(form) { }
		public new XtraDialogForm Control { get { return base.Control as XtraDialogForm; } }
		public override AccessibleObject Parent { get { return Control.GetParentAccessible(); } }
		protected override AccessibleRole GetRole() { return AccessibleRole.Dialog; }
		protected override string GetName() { return Control.Message.Caption; }
		protected override void OnChildrenCountChanged() {
			if(((XtraDialogArgs)Control.Message).Content != null)
				AddChild(new MessageContentAccessible(Control));
			base.OnChildrenCountChanged();
		}
		protected class MessageContentAccessible : BaseAccessible {
			XtraDialogForm form;
			public MessageContentAccessible(XtraDialogForm form) {
				this.form = form;
			}
			public override Rectangle ClientBounds { get { return form.ContentRectangle; } }
			protected override AccessibleRole GetRole() { return AccessibleRole.WhiteSpace; }
			protected override string GetName() { return ((XtraMessageBoxArgs)form.Message).Text; }
		}
	}
	public class BaseEditAccessible : BaseAccessible {
		RepositoryItem item;
		public BaseEditAccessible(RepositoryItem item) {
			this.item = item;
			CustomInfo = new AccessibleEditUserInfoHelper(item);
		}
		protected RepositoryItem Item { get { return item; } }
		protected BaseEdit Edit { get { return Item.OwnerEdit; } }
		public override Control GetOwnerControl() { 
			if(Item.OwnerEdit != null) return Item.OwnerEdit;
			return null;
		} 
		protected override AccessibleRole GetRole() { return AccessibleRole.Client; }
		public override string Value { get { return Edit != null ? Edit.Text : null; } }
		protected override AccessibleStates GetState() {
			AccessibleStates state = base.GetState();
			if(Edit == null) return state;
			if(Item.ReadOnly) state |= AccessibleStates.ReadOnly;
			if(Edit.EditorContainsFocus) state |= AccessibleStates.Focused;
			if(Item.AllowFocused) state |= AccessibleStates.Focusable;
			return state; 
		}
	}
	public class MemoEditAccessible : TextEditAccessible {
		public MemoEditAccessible(RepositoryItem item) : base(item) { }
		protected override ChildrenInfo GetChildrenInfo() {
			ChildrenInfo info = new ChildrenInfo();
			info[ChildType.Text] = 1;
			MemoEdit me = Item.OwnerEdit as MemoEdit;
			if(me != null) {
				info[ChildType.HScroll] = 1;
				info[ChildType.VScroll] = 1;
			}
			return info;
		}
		protected override void OnChildrenCountChanged() {
			base.OnChildrenCountChanged();
			MemoEdit me = Item.OwnerEdit as MemoEdit;
			Children.Add(new ScrollBarAccessible(me.scrollHelper.HScroll));
			Children.Add(new ScrollBarAccessible(me.scrollHelper.VScroll));
		}
	}
	public class TextEditAccessible : BaseEditAccessible {
		TextAccessible text;
		public TextEditAccessible(RepositoryItem item) : base(item) { 
			CreateCollection();
		}
		protected override ChildrenInfo GetChildrenInfo() {
			ChildrenInfo info = new ChildrenInfo();
			info[ChildType.Text] = 1;
			return info;
		}
		protected override void OnChildrenCountChanged() {
			Children.Clear();
			AddChild(Text);
		}
		public TextAccessible Text { 
			get { 
				if(text == null) text = CreateTextAccessible();
				return text;
			} 
		}
		protected virtual TextAccessible CreateTextAccessible() {
			return new TextAccessible(Item);
		}
		protected new RepositoryItemTextEdit Item { get { return base.Item as RepositoryItemTextEdit; } }
		protected override AccessibleRole GetRole() { return AccessibleRole.Text; }
		protected override AccessibleStates GetState() {
			if(Item.IsPasswordBox) return AccessibleStates.Protected;
			if(IsSubstAsEmpty) return AccessibleStates.None;
			return base.GetState();
		}
		bool IsSubstAsEmpty {
			get {
				if(Edit != null && Edit.InplaceType != InplaceType.Standalone && Item.OwnerEdit.MaskBox != null && Item.OwnerEdit.MaskBox.Visible)
					return true;
				return false;
			}
		}
		protected override string GetName() {
			if(!Item.IsPasswordBox) return base.GetName();
			return "PasswordTextEdit";
		}
		public override string Value {
			get {
				if(!Item.IsPasswordBox) return base.Value;
				return null;
			}
			set {
				if (Edit != null)
					Edit.Text = value;
			}
		}
	}
	public class TextAccessible : BaseAccessible {
		RepositoryItemTextEdit item;
		public TextAccessible(RepositoryItemTextEdit item) {
			this.item = item;
		}
		protected RepositoryItemTextEdit Item { get { return item; } }
		protected TextEdit Edit { get { return Item.OwnerEdit; } }
		public override Control GetOwnerControl() { return Edit; } 
		public override Rectangle ClientBounds {
			get {
				return Edit == null || !Edit.Visible ? Rectangle.Empty : Edit.ViewInfo.MaskBoxRect;
			}
		}
		protected override AccessibleRole GetRole() { 
			if(Edit == null) return AccessibleRole.None;
			return AccessibleRole.Text;
		}
		protected override AccessibleStates GetState() {
			AccessibleStates res = base.GetState();
			if(Item.IsPasswordBox) res = AccessibleStates.Protected;
			res = UpdateReadonlyState(res);
			if(Edit.ContainsFocus) res |= AccessibleStates.Focused;
			return res;
		}
		protected virtual AccessibleStates UpdateReadonlyState(AccessibleStates state) {
			if(Edit.MaskBox == null || Edit.MaskBox.ReadOnly || !Edit.MaskBox.Visible) return state | AccessibleStates.ReadOnly;
			return state;
		}
		public override string Value {
			get {
				if (Item.IsPasswordBox) return null;
				return Edit == null ? null : Edit.Text;
			}
			set {
				if (Edit != null)
					Edit.Text = value;
			}
		}
		protected override string GetName() {
			if(Item.PasswordChar == 0) return base.GetName();
			return "PasswordTextEdit";
		}
	}
	public class ButtonEditAccessible : TextEditAccessible {
		public ButtonEditAccessible(RepositoryItem item) : base(item) { }
		protected new RepositoryItemButtonEdit Item { get { return base.Item as RepositoryItemButtonEdit; } }
		protected override ChildrenInfo GetChildrenInfo() {
			ChildrenInfo info = base.GetChildrenInfo();
			if(info == null) info = new ChildrenInfo();
			info[ChildType.Button] = Item.Buttons.Count;
			return info;
		}
		protected override void OnChildrenCountChanged() {
			base.OnChildrenCountChanged();
			foreach(EditorButton button in Item.Buttons) {
				AddButton(button);
			}
		}
		protected virtual void AddButton(EditorButton button) {
			AddChild(new ButtonAccessible(Item, button));
		}
		protected class ButtonAccessible : BaseAccessible {
			RepositoryItemButtonEdit item;
			EditorButton button;
			public ButtonAccessible(RepositoryItemButtonEdit item, EditorButton button) {
				this.item = item;
				this.button = button;
			}
			public virtual EditorButton Button { get { return button; } }
			public RepositoryItemButtonEdit Item { get { return item; } }
			public ButtonEdit Edit { get { return Item.OwnerEdit; } }
			public override Control GetOwnerControl() { return Edit; }
			public override Rectangle ClientBounds {
				get {
					if(Edit == null || !Edit.Visible || !Button.Visible) return Rectangle.Empty;
					if(GetInfo() != null) return GetInfo().Bounds;
					return Rectangle.Empty;
				}
			}
			protected override AccessibleRole GetRole() { 
				return AccessibleRole.PushButton;
			}
			protected virtual bool AllowClick {
				get {
					return Button.Enabled && Item.Enabled;
				}
			}
			protected override void DoDefaultAction() {
				if(!AllowClick) return;
				Item.RaiseButtonClick(new ButtonPressedEventArgs(Button));
			}
			public virtual EditorButtonObjectInfoArgs GetInfo() {
				return Edit == null ? null : Edit.ViewInfo.ButtonInfoByButton(Button);
			}
			protected override string GetDefaultAction() {
				return AllowClick ? GetString(AccStringId.ButtonPush) : null;
			}
			protected override AccessibleStates GetState() { 
				EditorButtonObjectInfoArgs info = GetInfo();
				AccessibleStates state = AccessibleStates.None;
				if(!Button.Visible) state |= AccessibleStates.Invisible;
				if(!Edit.Properties.IsButtonEnabled(Button)) return state | AccessibleStates.Unavailable;
				EditorButtonObjectInfoArgs press = Edit.ViewInfo.PressedInfo.HitObject as EditorButtonObjectInfoArgs;
				if(press != null && press.Button == info.Button) state |= AccessibleStates.Pressed;
				return state;
			}
		}
	}
	public class BaseSpinEditAccessible : ButtonEditAccessible {
		public BaseSpinEditAccessible(RepositoryItem item) : base(item) { }
		protected new RepositoryItemBaseSpinEdit Item { get { return base.Item as RepositoryItemBaseSpinEdit; } }
		protected override void AddButton(EditorButton button) {
			if(button.Index == Item.SpinButtonIndex)
				AddChild(new SpinBoxAccessible(Item, button));
			else
				base.AddButton(button);
		}
		protected class SpinBoxAccessible : ButtonAccessible {
			public SpinBoxAccessible(RepositoryItemBaseSpinEdit item, EditorButton button) : base(item, button) { }
			public new BaseSpinEdit Edit { get { return base.Edit as BaseSpinEdit; } }
			public new RepositoryItemBaseSpinEdit Item { get { return base.Item as RepositoryItemBaseSpinEdit; } }
			protected override AccessibleRole GetRole() { return AccessibleRole.SpinButton; }
			protected override void DoDefaultAction() { }
			protected override string GetDefaultAction() { return null; }
			protected override ChildrenInfo GetChildrenInfo() { return new ChildrenInfo("spin", 2); }
			protected override void OnChildrenCountChanged() {
				AddChild(new SpinButtonAccessible(this, Item, Button, true));
				AddChild(new SpinButtonAccessible(this, Item, Button, false));
			}
			public override Rectangle ClientBounds {
				get {
					return base.ClientBounds;
				}
			}
			protected override string GetName() { return GetString(AccStringId.SpinBox); }
		}
		protected class SpinButtonAccessible : ButtonAccessible {
			SpinBoxAccessible spinBox;
			bool upButton;
			public SpinButtonAccessible(SpinBoxAccessible spinBox, RepositoryItemBaseSpinEdit item, EditorButton button, bool upButton) : base(item, button) { 
				this.spinBox = spinBox;
				this.upButton = upButton;
			}
			public override EditorButton Button { 
				get { 
					if(GetSpinInfo() != null) return this.upButton ? GetSpinInfo().UpButton : GetSpinInfo().DownButton;
					return base.Button; 
				} 
			}
			public override EditorButtonObjectInfoArgs GetInfo() {
				if(GetSpinInfo() != null) return this.upButton ? GetSpinInfo().UpButtonInfo : GetSpinInfo().DownButtonInfo;
				return null;
			}
			protected SpinButtonObjectInfoArgs GetSpinInfo() { return spinBox.GetInfo() as SpinButtonObjectInfoArgs; } 
			protected override string GetName() { 
				if(GetInfo() == null) return null;
				switch(GetInfo().Button.Kind) {
					case ButtonPredefines.SpinLeft : return GetString(AccStringId.SpinLeftButton);
					case ButtonPredefines.SpinRight : return GetString(AccStringId.SpinRightButton);
					case ButtonPredefines.SpinUp : return GetString(AccStringId.SpinUpButton);
					case ButtonPredefines.SpinDown : return GetString(AccStringId.SpinDownButton);
				}
				return null;
			}
			protected override void DoDefaultAction() {
				if(spinBox.Edit == null) return;
				if(upButton) 
					spinBox.Edit.DoSpin(true);
				else
					spinBox.Edit.DoSpin(false);
			}
		}
	}
	public class TimeSpanEditAccessible : BaseSpinEditAccessible {
		public TimeSpanEditAccessible(RepositoryItem item) : base(item) { }
	}
	public class ComboBoxEditTextAccessible : TextAccessible {
		public ComboBoxEditTextAccessible(RepositoryItemTextEdit te) : base(te) { }
		protected override AccessibleRole GetRole() {
			if(Edit == null) return AccessibleRole.None;
			if(Edit.MaskBox == null || Edit.MaskBox.ReadOnly || !Edit.MaskBox.Visible) return AccessibleRole.StaticText;
			return AccessibleRole.Text;
		}
		protected override AccessibleStates UpdateReadonlyState(AccessibleStates state) {
			return state;
		}
	}
	public class ComboBoxItemAccessibleObject : BaseAccessibleObject {
		public ComboBoxItemAccessibleObject(IDXAccessible owner) : base(owner) { }
		ComboBoxEditAccessible ComboBoxAccessible {
			get { return ((ComboBoxItemAccessible)DXOwner).ComboBox; }
		}
		object Item { get { return ((ComboBoxItemAccessible)DXOwner).Item; } }
		public override void Select(AccessibleSelection flags) {
			ComboBoxAccessible.ComboBox.SelectedItem = Item;
		}
	}
	public class ComboBoxItemAccessible : BaseAccessible {
		object item;
		ComboBoxEditAccessible comboBox;
		public ComboBoxItemAccessible(ComboBoxEditAccessible comboBox, object item) {
			this.item = item;
			this.comboBox = comboBox;
		}
		ComboBoxItemAccessibleObject itemAccessibleObject;
		public override AccessibleObject Accessible {
			get {
				if(itemAccessibleObject == null)
					itemAccessibleObject = new ComboBoxItemAccessibleObject(this);
				return itemAccessibleObject;
			}
		}
		public object Item { get { return item; } }
		public ComboBoxEditAccessible ComboBox { get { return comboBox; } }
		protected override AccessibleRole GetRole() {
			return AccessibleRole.ListItem;
		}
		protected override AccessibleStates GetState() {
			AccessibleStates res = AccessibleStates.Selectable | AccessibleStates.Focusable;
			if(ComboBox.ComboBox.SelectedItem == Item)
				res |= AccessibleStates.Selected;
			return res;
		}
		public override string Value {
			get {
				return Item.ToString();
			}
		}
		protected override string GetName() {
			return Value;
		}
	}
	public class ComboBoxEditItemsListAccessible : BaseAccessible {
		ComboBoxEditAccessible comboBox;
		public ComboBoxEditItemsListAccessible(ComboBoxEditAccessible comboBox) {
			this.comboBox = comboBox;
		}
		public ComboBoxEditAccessible ComboBox { get { return comboBox; } }
		protected override AccessibleRole GetRole() {
			return AccessibleRole.List;
		}
		public override int GetChildCount() {
			return ComboBox.ComboBoxItem.Items.Count;
		}
		protected override ChildrenInfo GetChildrenInfo() {
			ChildrenInfo res =  base.GetChildrenInfo();
			if(res == null)
				res = new ChildrenInfo();
			res[ChildType.Item] = ComboBox.ComboBoxItem.Items.Count;
			return res;
		}
		public override AccessibleObject GetChild(int index) {
			return base.GetChild(index);
		}
		protected override void OnChildrenCountChanged() {
			base.OnChildrenCountChanged();
			foreach(object item in ComboBox.ComboBoxItem.Items) {
				AddChild(new ComboBoxItemAccessible(ComboBox, item));
			}
		}
	}
	public class ComboBoxEditAccessible : PopupEditAccessible {
		public ComboBoxEditAccessible(RepositoryItem item) : base(item) { }
		protected override TextAccessible CreateTextAccessible() {
			return new ComboBoxEditTextAccessible(Edit.Properties);
		}
		protected override AccessibleRole GetRole() {
			return AccessibleRole.ComboBox;
		}
		public ComboBoxEdit ComboBox { get { return (ComboBoxEdit)Edit; } }
		public RepositoryItemComboBox ComboBoxItem { get { return ComboBox.Properties; } }
		protected override ChildrenInfo GetChildrenInfo() {
			ChildrenInfo info = base.GetChildrenInfo();
			info["List"] = 1;
			return info;
		}
		protected override void OnChildrenCountChanged() {
			base.OnChildrenCountChanged();
			AddChild(new ComboBoxEditItemsListAccessible(this));
		}
	}
	public class PopupEditAccessible : ButtonEditAccessible {
		public PopupEditAccessible(RepositoryItem item) : base(item) { }
		protected new RepositoryItemPopupBase Item { get { return base.Item as RepositoryItemPopupBase; } }
		protected new PopupBaseEdit Edit { get { return base.Edit as PopupBaseEdit; } }
		protected override ChildrenInfo GetChildrenInfo() {
			ChildrenInfo res = base.GetChildrenInfo();
			if(res != null && Edit.IsPopupOpen) res["Popup"] = 1;
			return res;
		}
		protected override void OnChildrenCountChanged() {
			base.OnChildrenCountChanged();
			if(Edit == null || !Edit.IsPopupOpen) return;
			AddChild(Edit.PopupForm.DXAccessible);
		}
		protected override void AddButton(EditorButton button) {
			if(button.Index == Item.ActionButtonIndex)
				AddChild(new PopupButtonAccessible(Item, button));
			else
				base.AddButton(button);
		}
		protected class PopupButtonAccessible : ButtonAccessible {
			public PopupButtonAccessible(RepositoryItemButtonEdit item, EditorButton button) : base(item, button) { }
			protected new PopupBaseEdit Edit { get { return base.Edit as PopupBaseEdit; } }
			protected override void DoDefaultAction() {
				if(Edit == null || !AllowClick) return;
				if(Edit.IsPopupOpen) 
					Edit.ClosePopup();
				else
					Edit.ShowPopup();
			}
			protected override string GetName() {
				return GetDefaultAction();
			}
			protected override string GetDefaultAction() {
				if(Edit == null || !AllowClick) return null;
				return Edit.IsPopupOpen ? GetString(AccStringId.ButtonClose) : GetString(AccStringId.ButtonOpen);
			}
			protected override string GetKeyboardShortcut() {
				return GetString(AccStringId.OpenKeyboardShortcut);
			}
		}
	}
	public class AccessibleEditUserInfoHelper : IDXAccessibleUserInfo {
		RepositoryItem item;
		public AccessibleEditUserInfoHelper(RepositoryItem item) {
			this.item = item;
		}
		string IDXAccessibleUserInfo.DefaultAction { get { return item.AccessibleDefaultActionDescription; } }
		string IDXAccessibleUserInfo.AccessibleName { get { return item.AccessibleName; } }
		string IDXAccessibleUserInfo.AccessibleDescription { get { return item.AccessibleDescription; } }
		AccessibleRole IDXAccessibleUserInfo.AccessibleRole { get { return item.AccessibleRole; } }
	}
	public abstract class ListControlAccessible : BaseControlAccessible {
		protected ListControlAccessible(Control listControl) : base(listControl) {
			CreateCollection();
		}
		protected override BaseAccessible GetSelected() {
			if(SelectedItem == null) return null;
			int count = ChildCount;
			if(SelectedIndex >= count || count == 0) return null;
			return Children[SelectedIndex];
		}
		protected override BaseAccessible GetFocused() {
			if(IsVisible && Control.ContainsFocus) return GetSelected();
			return null;
		}
		protected override ChildrenInfo GetChildrenInfo() {
			ChildrenInfo info = base.GetChildrenInfo();
			if(info == null) info = new ChildrenInfo();
			info[ChildType.Item] = ItemCount;
			return info;
		}
		protected override void OnChildrenCountChanged() {
			base.OnChildrenCountChanged();
			for(int n = 0; n < ItemCount; n++) {
				AddItem(n);
			}
		}
		protected abstract void AddItem(int index);
		protected abstract object SelectedItem { get; }
		protected internal abstract int SelectedIndex { get; set; }
		protected abstract int ItemCount { get; }
		protected override AccessibleRole GetRole() { return AccessibleRole.List; }
		protected internal Control ListControl { get { return Control; } }
		protected internal abstract Rectangle GetItemRectangle(int index);
		protected internal abstract string GetItemText(int index); 
		protected class ListControlItemAccessible : BaseAccessible {
			int index;
			ListControlAccessible accessible;
			public ListControlItemAccessible(ListControlAccessible accessible, int index) {
				this.index = index;
				this.accessible = accessible;
			}
			protected override AccessibleRole GetRole() { return AccessibleRole.ListItem; }
			protected override string GetDefaultAction() { return GetString(AccStringId.MouseDoubleClick); }
			protected override string GetName() { return Owner.GetItemText(index); }
			public override Control GetOwnerControl() { return Owner.ListControl; }
			public override Rectangle ClientBounds { get { return Owner.GetItemRectangle(Index); } }
			protected override AccessibleStates GetState() { 
				AccessibleStates state = base.GetState() | AccessibleStates.Selectable;
				if(Owner.ListControl.Enabled) state |= AccessibleStates.Focusable;
				if(Owner.SelectedIndex == Index) {
					state |= AccessibleStates.Selected;
					if(Owner.ListControl.ContainsFocus && Owner.ListControl.Enabled) state |= AccessibleStates.Focused;
				}
				if(ClientBounds.IsEmpty) state |= AccessibleStates.Invisible;
				return state;
			}
			protected override void DoDefaultAction() { 
				Owner.SelectedIndex = Index; 
				PopupBaseForm popupForm = GetOwnerControl().FindForm() as PopupBaseForm;
				if(popupForm != null) popupForm.ClosePopup();
			}
			protected ListControlAccessible Owner { get { return accessible; } }
			protected int Index { get { return index; } }
		}
	}
	public class ListBoxAccessible : ListControlAccessible {
		public ListBoxAccessible(BaseListBoxControl listBox) : base(listBox) { 
		}
		protected new BaseListBoxControl Control { get { return base.Control as BaseListBoxControl; } }
		protected override ChildrenInfo GetChildrenInfo() {
			ChildrenInfo result = base.GetChildrenInfo();
			if(Control.ViewInfo != null) {
				result[ChildType.HScroll] = Control.ScrollInfo.HScrollVisible ? 1 : 0;
				result[ChildType.VScroll] = Control.ScrollInfo.VScrollVisible ? 1 : 0;
			}
			return result;
		}
		protected override void OnChildrenCountChanged() {
			base.OnChildrenCountChanged();
			if(Control.ScrollInfo.HScrollVisible) {
				AddChild(Control.ScrollInfo.HScroll.GetAccessible());
			}
			if(Control.ScrollInfo.VScrollVisible) {
				AddChild(Control.ScrollInfo.VScroll.GetAccessible());
			}
		}
		protected override void AddItem(int index) {
			AddChild(new ListControlItemAccessible(this, index));
		}
		protected override object SelectedItem { get { return Control.SelectedItem; } }
		protected internal override int SelectedIndex { get { return Control.SelectedIndex; } set { Control.SelectedIndex = value; } }
		protected override int ItemCount { get { return Control.ItemCount; } }
		protected internal override Rectangle GetItemRectangle(int index) { return Control.ViewInfo.GetItemRectangle(index); }
		protected internal override string GetItemText(int index) { return Control.GetItemText(index); }
	}
	public class ComboBoxListBoxAccessible : ListBoxAccessible {
		public ComboBoxListBoxAccessible(BaseListBoxControl listBox) : base(listBox) { }
		protected override AccessibleStates GetState() {
			return AccessibleStates.None;
		}
		protected override string GetName() {
			return string.Empty;
		}
		protected override void AddItem(int index) {
			AddChild(new ComboBoxListControlItemAccessible(this, index));
		}
		protected override BaseAccessible GetFocused() { return null; }
		protected override BaseAccessible GetSelected() { return null; }
		protected internal int HotItemIndex { get { return Control.HotItemIndex; } }
		protected class ComboBoxListControlItemAccessible : ListControlItemAccessible {
			ComboBoxListBoxAccessible comboBoxListBoxAccessible;
			public ComboBoxListControlItemAccessible(ComboBoxListBoxAccessible accessible, int index) : base(accessible, index) {
				comboBoxListBoxAccessible = accessible;
			}
			protected new ComboBoxListBoxAccessible Owner { get { return comboBoxListBoxAccessible; } }
			protected override AccessibleStates GetState() {
				AccessibleStates state = base.GetState();
				if(Owner.HotItemIndex == Index) state |= AccessibleStates.Focused;
				return state;
			}
		}
	}
	public class CheckedListBoxAccessible : ListBoxAccessible {
		public CheckedListBoxAccessible(BaseCheckedListBoxControl listBox) : base(listBox) {}
		protected override void AddItem(int index) {
			AddChild(new CheckedListBoxItemAccessible(this, index));
		}
		protected class CheckedListBoxItemAccessible : ListControlItemAccessible {
			public CheckedListBoxItemAccessible(CheckedListBoxAccessible accessible, int index) : base(accessible, index) {
			}
			protected override AccessibleRole GetRole() {
				return AccessibleRole.CheckButton;
			}
			protected override string GetDefaultAction() {
				if(Enabled) return (Checked ? GetString(AccStringId.CheckEditUncheck) : GetString(AccStringId.CheckEditCheck));
				return base.GetDefaultAction();
			}
			protected override void DoDefaultAction() {
				if(!Enabled) {
					ListBox.SelectedIndex = Index; 
					return;
				}
				ListBox.SetItemChecked(Index, !Checked);
			}
			protected override AccessibleStates GetState() { 
				AccessibleStates result = base.GetState();
				if(Checked) result |= AccessibleStates.Checked;
				if(!Enabled) result |= AccessibleStates.Unavailable;
				return result;
			}
			protected bool Enabled { get { return ListBox.GetItemEnabledCore(Index); } }
			protected bool Checked { get { return ListBox.GetItemChecked(Index); } }
			protected BaseCheckedListBoxControl ListBox { get { return GetOwnerControl() as BaseCheckedListBoxControl; } }
		}
		protected new BaseCheckedListBoxControl Control { get { return base.Control as BaseCheckedListBoxControl; } }
	}
	public class RadioGroupAccessible : ListControlAccessible {
		protected class RadioGroupItemAccessible : ListControlItemAccessible {
			public RadioGroupItemAccessible(RadioGroupAccessible group, int index) : base(group, index) { }
			protected override AccessibleRole GetRole() {
				return AccessibleRole.RadioButton;
			}
		}
		public RadioGroupAccessible(RadioGroup radioGroup) : base(radioGroup) {}
		protected new RadioGroup Control { get { return base.Control as RadioGroup; } }
		protected override int ItemCount { get { return Control.Properties.Items.Count; } }
		protected internal override int SelectedIndex { get { return Control.SelectedIndex; } set { Control.SelectedIndex = value; }}
		protected override object SelectedItem { 
			get {
				if(SelectedIndex == -1) return null;
				return Control.Properties.Items[SelectedIndex];
			}
		}
		protected RadioGroup RadioGroup { get { return GetOwnerControl() as RadioGroup; } }
		protected override void AddItem(int index) {
			AddChild(new RadioGroupItemAccessible(this,index));
		}
		protected internal override Rectangle GetItemRectangle(int index) { return RadioGroup.GetItemRectangle(index); }
		protected internal override string GetItemText(int index) {
			return RadioGroup.Properties.Items[index].ToString(); 
		}
	}
	public class CheckEditAccessible : BaseEditAccessible {
		public CheckEditAccessible(RepositoryItem check) : base(check) { }
		protected new RepositoryItemCheckEdit Item { get { return base.Item as RepositoryItemCheckEdit; } }
		protected new CheckEdit Edit { get { return base.Edit as CheckEdit; } }
		protected override AccessibleRole GetRole() { 
			return Item.IsRadioButton ? AccessibleRole.RadioButton : AccessibleRole.CheckButton; 
		}
		protected override void DoDefaultAction() { 
			if(Edit == null || Item.ReadOnly || !Item.Enabled) return;
			Edit.Toggle();
		}
		protected override string GetDefaultAction() {
			if(Edit == null || Item.ReadOnly || !Item.Enabled) return null;
			return !Edit.Checked || Item.IsRadioButton ? GetString(AccStringId.CheckEditCheck) : GetString(AccStringId.CheckEditUncheck);
		}
		protected override string GetName() { return Item.Caption; }
		protected override AccessibleStates GetState() {
			AccessibleStates state = base.GetState();
			if(Edit == null) return state;
			if(Item.IsRadioButton) {
				if(Edit.Checked) state |= AccessibleStates.Checked; 
			} else {
				switch(Edit.CheckState) {
					case CheckState.Checked : state |= AccessibleStates.Checked; break;
					case CheckState.Indeterminate : state |= AccessibleStates.Indeterminate; break;
				}
			}
			return state;
		}
		public override string Value { get { return null; } }
	}
	public class ToggleSwitchAccessible : BaseEditAccessible {
		public ToggleSwitchAccessible(RepositoryItem check) : base(check) { }
		protected new RepositoryItemToggleSwitch Item { get { return base.Item as RepositoryItemToggleSwitch; } }
		protected new ToggleSwitch Edit { get { return base.Edit as ToggleSwitch; } }
		protected override AccessibleRole GetRole() {
			return AccessibleRole.CheckButton;
		}
		protected override void DoDefaultAction() {
			if(Edit == null || Item.ReadOnly || !Item.Enabled) return;
			Edit.Toggle();
		}
		protected override string GetDefaultAction() {
			if(Edit == null || Item.ReadOnly || !Item.Enabled) return null;
			return Edit.IsOn ? GetString(AccStringId.CheckEditCheck) : GetString(AccStringId.CheckEditUncheck);
		}
		protected override string GetName() { return Item.Caption; }
		protected override AccessibleStates GetState() {
			AccessibleStates state = base.GetState();
			if(Edit == null) return state;
			if(Edit.IsOn) state |= AccessibleStates.Checked;	 
			return state;
		}
		public override string Value { get { return null; } }
	}
	public class RatingControlAccessible : BaseEditAccessible {
		public RatingControlAccessible(RepositoryItem item) : base(item) { }
		protected new RepositoryItemRatingControl Item { get { return base.Item as RepositoryItemRatingControl; } }
		protected new RatingControl Edit { get { return base.Edit as RatingControl; } }
		protected override AccessibleRole GetRole() { return AccessibleRole.Chart; }
		public override string Value { 
			get {
				if(Edit != null)
					return Edit.Rating.ToString();
				return String.Empty;
			}
		} 
	}
	public class ButtonControlAccessible : BaseControlAccessible {
		public ButtonControlAccessible(BaseButton button) : base(button) { }
		protected new BaseButton Control { get { return base.Control as BaseButton; } }
		protected override void DoDefaultAction() {
			if(AllowClick) {
				if(Control.CanSelect) 
					Control.PerformClick();
				else
					Control.FireClick();
			}
		}
		protected override AccessibleRole GetRole() { return AccessibleRole.PushButton; }
		protected bool AllowClick { get { return Control.Enabled && Control.Visible; } }
		protected override string GetDefaultAction() { return AllowClick ? GetString(AccStringId.ButtonPush) : null; }
		protected override string GetName() { return Control.Text; }
	}
	public class ProgressBarAccessible : BaseEditAccessible {
		public ProgressBarAccessible(RepositoryItem item) : base(item) { }
		protected new RepositoryItemProgressBar Item { get { return base.Item as RepositoryItemProgressBar; } }
		protected new ProgressBarControl Edit { get { return base.Edit as ProgressBarControl; } }
		protected ProgressBarViewInfo ViewInfo { get { return (Edit != null ? Edit.ViewInfo : null) as ProgressBarViewInfo; } }
		public override string Value { 
			get { 
				if(ViewInfo == null) return null;
				return string.Format("{0}%", ViewInfo.GetPercents());
			}
		}
		protected override AccessibleRole GetRole() { return AccessibleRole.ProgressBar; }
	}
	public class TokenEditAccessible : BaseEditAccessible {
		public TokenEditAccessible(RepositoryItem item)
			: base(item) {
		}
		protected override AccessibleRole GetRole() {
			return AccessibleRole.List;
		}
		public override string Value {
			get { return base.Value; }
		}
		protected new TokenEdit Edit { get { return base.Edit as TokenEdit; } }
		protected new RepositoryItemTokenEdit Item { get { return base.Item as RepositoryItemTokenEdit; } }
		protected TokenEditViewInfo ViewInfo { get { return (Edit != null ? Edit.ViewInfo : null) as TokenEditViewInfo; } }
	}
	public class NavigatorAccessible : BaseControlAccessible {
		public NavigatorAccessible(NavigatorBase owner) : base(owner) {
		}
		public new NavigatorBase Control {
			get { return base.Control as NavigatorBase; }
		}
		public override string Value {
			get { return (Control.ViewInfo as NavigatorControlViewInfo).GetText(); }
		}
		protected override ChildrenInfo GetChildrenInfo() {
			return new ChildrenInfo(ChildType.Button, Control.ButtonsCore.ButtonCollection.Count + Control.ButtonsCore.CustomButtons.Count);
		}
		protected override void OnChildrenCountChanged() {
			foreach(NavigatorButtonBase button in Control.ButtonsCore.ButtonCollection) 
				AddChild(new NavigatorButtonAccessible(Control, button));
			foreach(NavigatorButtonBase button in Control.ButtonsCore.CustomButtons) 
				AddChild(new NavigatorButtonAccessible(Control, button));
		}
		protected override AccessibleRole GetRole() { return AccessibleRole.Client; }
		protected class NavigatorButtonAccessible : ButtonAccessible {
			NavigatorBase owner;
			NavigatorButtonBase button;
			public NavigatorButtonAccessible(NavigatorBase owner, NavigatorButtonBase button) {
				this.owner = owner;
				this.button = button;
			}
			NavigatorBase Owner { get { return owner; } }
			NavigatorButtonBase Button { get { return button; } }
			protected override void DoDefaultAction() { 
				Owner.ButtonsCore.DoClick(button);
			}
			protected override string GetName() { return button.ViewInfoHint; } 
			protected override string GetDefaultAction() { return button.ViewInfoEnabled ? base.GetDefaultAction() : null;	}
			protected override AccessibleStates GetState() {
				AccessibleStates ret = AccessibleStates.Default;
				if(!button.Visible) ret |= AccessibleStates.Invisible;
				if(!button.ViewInfoEnabled) ret |= AccessibleStates.Unavailable;
				return ret;
			}
			public override Rectangle ClientBounds {
				get { 
					NavigatorButtonViewInfo viewInfo = Owner.ButtonsCore.ViewInfo.GetButtonViewInfo(button);
					if(viewInfo == null) return Rectangle.Empty;
					return viewInfo.Bounds;
				}
			}
		}
	}
	public abstract class BaseGridAccessibleObject : BaseAccessible {
		BaseGridDataPanelAccessibleObject dataPanel;
		BaseGridHeaderPanelAccessibleObject headerPanel;
		public BaseGridAccessibleObject() { }
		protected abstract IAccessibleGrid Grid { get;  }
		protected override ChildrenInfo GetChildrenInfo() {
			ChildrenInfo info = new ChildrenInfo();
			info["Header"] = Grid.HeaderCount > 0 ? 1 : 0;
			info["Rows"] = 1;
			info[ChildType.HScroll] = Grid.HScroll != null ? 1 : 0;
			info[ChildType.VScroll] = Grid.VScroll != null ? 1 : 0;
			return info;
		}
		protected BaseGridDataPanelAccessibleObject DataPanel {
			get {
				RequestChildren();
				return dataPanel;
			}
		}
		protected BaseGridHeaderPanelAccessibleObject HeaderPanel {
			get {
				RequestChildren();
				return headerPanel;
			}
		}
		protected override AccessibleRole GetRole() { return AccessibleRole.Table; }
		protected override void OnChildrenCountChanged() {
			if(Grid.HeaderCount > 0) {
				AddChild(headerPanel = CreateHeader());
			}
			AddChild(dataPanel = CreateRows());
			if(Grid.HScroll != null) AddChild(Grid.HScroll.GetAccessible());
			if(Grid.VScroll != null) AddChild(Grid.VScroll.GetAccessible());
		}
		protected abstract BaseGridHeaderPanelAccessibleObject CreateHeader();
		protected abstract BaseGridDataPanelAccessibleObject CreateRows();
		public virtual AccessibleObject GetAccessibleObjectById(int objectId, int childId) { return null; }
		protected class BaseGridHeaderPanelAccessibleObject : BaseAccessible {
			IAccessibleGrid grid;
			public BaseGridHeaderPanelAccessibleObject(IAccessibleGrid grid) {
				this.grid = grid;
			}
			protected IAccessibleGrid Grid { get { return grid; } }
			protected override AccessibleRole GetRole() { return AccessibleRole.Grouping; }
			protected override string GetName() { return GetString(AccStringId.GridHeaderPanel); }
			protected override ChildrenInfo GetChildrenInfo() { return new ChildrenInfo(ChildType.Item, Grid.HeaderCount); }
			public void ForceUpdateColumns() { ForceUpdateChildren(); }
		}
		protected abstract class BaseGridDataPanelAccessibleObject : BaseAccessible {
			IAccessibleGrid grid;
			public BaseGridDataPanelAccessibleObject(IAccessibleGrid grid) {
				this.grid = grid;
				CreateCollection();
			}
			protected override BaseAccessible GetSelected() {
				int index = Grid.SelectedRow;
				if(index < 0) return null;
				return Children[index];
			}
			protected IAccessibleGrid Grid { get { return grid; } }
			protected override ChildrenInfo GetChildrenInfo() { return new ChildrenInfo(ChildType.Item, Grid.RowCount); }
			protected override AccessibleRole GetRole() { return AccessibleRole.Grouping; }
			protected override string GetName() { return GetString(AccStringId.GridDataPanel); }
			protected override BaseAccessibleCollection CreateCollectionInstance() { return new BaseGridRowVirtualCollection(this); }
			protected abstract BaseAccessible CreateChild(int index);
			protected class BaseGridRowVirtualCollection : BaseAccessibleVirtualCollection {
				public BaseGridRowVirtualCollection(BaseGridDataPanelAccessibleObject owner) : base(owner) { }
				public new BaseGridDataPanelAccessibleObject Owner { get { return base.Owner as BaseGridDataPanelAccessibleObject; } }
				protected override BaseAccessible CreateChild(int index) {
					return Owner.CreateChild(index);
				}
				public override int IndexOf(BaseAccessible accessible) {
					BaseGridRowAccessibleObject row = accessible as BaseGridRowAccessibleObject;
					if(row != null) return row.Row.Index;
					return List.IndexOf(accessible);
				}
				protected override bool CompareChild(BaseAccessible acc1, BaseAccessible acc2) {
					if(!base.CompareChild(acc1, acc2)) return false;
					BaseGridRowAccessibleObject row1 = acc1 as BaseGridRowAccessibleObject;
					BaseGridRowAccessibleObject row2 = acc2 as BaseGridRowAccessibleObject;
					if(row1 != null && row2 != null) return row1.Row.Index == row2.Row.Index;
					return false;
				}
			}
			public override BaseAccessible GetBaseAccessible(int x, int y, bool recursive) {
				Point client = PointToClient(new Point(x, y));
				int index = Grid.FindRow(client.X, client.Y);
				if(index > -1) return Children[index];
				if(IsVisible && ScreenBounds.Contains(x, y)) return this;
				return null;
			}
		}
		protected class BaseGridHeaderAccessibleObject : BaseAccessible {
			IAccessibleGridHeaderCell header;
			public BaseGridHeaderAccessibleObject(IAccessibleGridHeaderCell header) {
				this.header = header;
			}
			protected override AccessibleRole GetRole() { return AccessibleRole.RowHeader; }
			protected IAccessibleGridHeaderCell Header { get { return header; } }
			public override Rectangle ClientBounds { get { return Header.Bounds; } }
			protected override AccessibleStates GetState() { return Header.GetState(); }
			protected override string GetDefaultAction() { return Header.GetDefaultAction(); }
			protected override void DoDefaultAction() { Header.DoDefaultAction(); }
			protected override string GetName() { return Header.GetName(); }
		}
		protected class BaseGridRowAccessibleObject : BaseAccessible {
			IAccessibleGridRow row;
			public BaseGridRowAccessibleObject(IAccessibleGridRow row) {
				this.row = row;
			}
			protected override ChildrenInfo GetChildrenInfo() { return new ChildrenInfo(ChildType.Item, Row.CellCount); }
			protected override void OnChildrenCountChanged() {
				int count = Row.CellCount;
				for(int n = 0; n < count; n++) {
					AddCell(n);
				}
			}
			protected virtual void AddCell(int cellIndex) {
				BaseAccessible acc = CreateCellInstance(cellIndex, Row.GetCell(cellIndex));
				if(acc != null) AddChild(acc);
			}
			protected virtual BaseAccessible CreateCellInstance(int cellIndex, IAccessibleGridRowCell cell) {
				if(cell == null) return null;
				return new BaseGridCellAccessibleObject(cell);
			}
			public IAccessibleGridRow Row { get { return row; } }
			public override Rectangle ClientBounds { get { return row.Bounds; } }
			protected override AccessibleRole GetRole() { return AccessibleRole.Row; }
			protected override string GetName() { return Row.GetName(); }
			protected override string GetDefaultAction() { return Row.GetDefaultAction(); }
			protected override void DoDefaultAction() { Row.DoDefaultAction(); }
			protected override AccessibleStates GetState() { return Row.GetState(); }
			public override string Value { get { return Row.GetValue(); } }
		}
		protected class BaseGridCellAccessibleObject : BaseAccessible {
			IAccessibleGridRowCell cell;
			public BaseGridCellAccessibleObject(IAccessibleGridRowCell cell) {
				this.cell = cell;
			}
			protected IAccessibleGridRowCell Cell { get { return cell; } }
			protected override AccessibleRole GetRole() { return AccessibleRole.Cell; }
			public override Rectangle ClientBounds { get { return Cell.Bounds; } }
			protected override string GetDefaultAction() { return Cell.GetDefaultAction(); }
			protected override void DoDefaultAction() { Cell.DoDefaultAction(); }
			protected override string GetName() { return Cell.GetName(); }
			public override string Value { get { return Cell.GetValue(); } }
			protected override AccessibleStates GetState() { return Cell.GetState(); }
			bool inRequestChildren = false;
			protected override void RequestChildren() {
				if(inRequestChildren)
					return;
				inRequestChildren = true;
				try {
					base.RequestChildren();
					BaseAccessible ce = Cell == null ? null : Cell.GetEdit();
					if(Children != null && Children.Count > 0 && ce != null && Children[0].GetHashCode() != ce.GetHashCode()) {
						ResetCollection();
						OnChildrenCountChanged();
					}
				}
				finally { inRequestChildren = false; }
			}
			protected override ChildrenInfo GetChildrenInfo() { 
				if(Cell.GetEdit() == null) return null;
				return new ChildrenInfo(ChildType.Text, 1);
			}
			protected override void OnChildrenCountChanged() {
				if(Cell.GetEdit() != null) AddChild(Cell.GetEdit());
			}
		}
	}
	public class LookupPopupAccessibleObject : BaseGridAccessibleObject {
		PopupLookUpEditForm lookUpForm;
		public LookupPopupAccessibleObject(PopupLookUpEditForm lookUpForm) {
			this.lookUpForm = lookUpForm;
			ParentCore = lookUpForm.OwnerEdit.DXAccessible;
		}
		public override Control GetOwnerControl() { return lookUpForm; }
		protected override AccessibleRole GetRole() { return AccessibleRole.Window; }
		protected override IAccessibleGrid Grid { get { return lookUpForm; } }
		protected override BaseGridHeaderPanelAccessibleObject CreateHeader() { return new LookUpHeaderPanel(lookUpForm); }
		protected override BaseGridDataPanelAccessibleObject CreateRows() { 
			return new LookUpDataPanel(lookUpForm);; 
		}
		protected class LookUpHeaderPanel : BaseGridHeaderPanelAccessibleObject {
			public LookUpHeaderPanel(PopupLookUpEditForm lookUpForm) : base(lookUpForm) { }
			protected PopupLookUpEditForm LookUpForm { get { return (PopupLookUpEditForm)Grid; } }
			protected override void OnChildrenCountChanged() {
				int n = 0;
				foreach(LookUpColumnInfo info in LookUpForm.Properties.Columns) {
					if(!info.Visible) continue;
					AddChild(CreateHeader(n++));
				}
			}
			public override Rectangle ClientBounds { get { return LookUpForm.ViewInfo.HeaderRect; } }
			protected virtual BaseGridHeaderAccessibleObject CreateHeader(int index) {
				return new BaseGridHeaderAccessibleObject(LookUpForm.ViewInfo.CreateHeader(index));
			}
		}
		protected class LookUpDataPanel : BaseGridDataPanelAccessibleObject {
			public LookUpDataPanel(PopupLookUpEditForm lookUpForm) : base(lookUpForm) { }
			protected PopupLookUpEditForm LookUpForm { get { return (PopupLookUpEditForm)Grid; } }
			protected override BaseAccessible CreateChild(int index) {
				IAccessibleGridRow row = Grid.GetRow(index);
				if(row == null) return null;
				return new BaseGridRowAccessibleObject(row);
			}
			public override Rectangle ClientBounds { get { return LookUpForm.ViewInfo.GridRect; } }
		}
		public class LookUpCell : IAccessibleGridRowCell {
			LookUpRowInfo row;
			int cellIndex;
			public LookUpCell(LookUpRowInfo row, int cellIndex) {
				this.cellIndex = cellIndex;
				this.row = row;
			}
			public virtual Rectangle Bounds { get { return row.GetCellBounds(cellIndex); } }
			IAccessibleGridRow IRow { get { return row as IAccessibleGridRow; } }
			string IAccessibleGridRowCell.GetDefaultAction() { return IRow.GetDefaultAction(); }
			BaseAccessible IAccessibleGridRowCell.GetEdit() { return null; }
			void IAccessibleGridRowCell.DoDefaultAction() { IRow.DoDefaultAction(); }
			string IAccessibleGridRowCell.GetName() {
				return row.GetCellName(cellIndex);
			}
			string IAccessibleGridRowCell.GetValue() {
				return row.GetCellValue(cellIndex);
			}
			AccessibleStates IAccessibleGridRowCell.GetState() { 
				AccessibleStates state = AccessibleStates.ReadOnly;
				if(Bounds.IsEmpty) state |= AccessibleStates.Invisible;
				return state;
			}
		}
	}
	public interface IAccessibleGrid {
		int HeaderCount { get; }
		int RowCount { get; }
		int SelectedRow { get; }
		ScrollBarBase HScroll { get; }
		ScrollBarBase VScroll { get; }
		IAccessibleGridRow GetRow(int index);
		int FindRow(int x, int y);
	}
	public interface IAccessibleGridRow {
		Rectangle Bounds { get; }
		int CellCount { get; }
		int Index { get; }
		IAccessibleGridRowCell GetCell(int index);
		string GetDefaultAction();
		void DoDefaultAction();
		string GetName();
		AccessibleStates GetState();
		string GetValue();
	}
	public interface IAccessibleGridRowCell {
		Rectangle Bounds { get; }
		string GetDefaultAction();
		void DoDefaultAction();
		string GetName();
		string GetValue();
		AccessibleStates GetState();
		BaseAccessible GetEdit();
	}
	public interface IAccessibleGridHeader {
		Rectangle Bounds { get; }
		int HeaderCellCount { get; }
		IAccessibleGridHeaderCell GetHeaderCell(int index);
	}
}
