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
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using DevExpress.Xpf.Core.Native;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections;
using DevExpress.Xpf.Editors.Helpers;
using System.Linq;
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.Editors.Automation {
	public abstract class BaseEditAutomationPeer : FrameworkElementAutomationPeer, IValueProvider {
		public BaseEditAutomationPeer(BaseEdit element)
			: base(element) {
		}
		protected BaseEdit Editor { get { return base.Owner as BaseEdit; } }
		protected override string GetClassNameCore() {
			return Editor.GetType().Name;
		}
		object GetValueInterface() {
			return this;
		}
		public override object GetPattern(PatternInterface patternInterface) {
			if(patternInterface == PatternInterface.Value)
				return GetValueInterface();
			return null;
		}
		protected virtual string GetValue() {
			if (Editor.EditValue != null)
				return Editor.EditValue.ToString();
			return string.Empty;
		}
		protected virtual void SetValue(string value) {
			if (!base.IsEnabled() || Editor.IsReadOnly)
				throw new ElementNotEnabledException();
			BaseEditHelper.SetCurrentValue(Editor, BaseEdit.EditValueProperty, value);
		}
		protected override Rect GetBoundingRectangleCore() {
#if !SL
			return base.GetBoundingRectangleCore();
#else
			return this.GetBoundingRectangleCore(Editor);
#endif
		}
		#region IValueProvider Members
		bool IValueProvider.IsReadOnly {
			get { return Editor.IsReadOnly; }
		}
		void IValueProvider.SetValue(string value) {
			SetValue(value);
		}
		string IValueProvider.Value {
			get { return GetValue(); }
		}
		#endregion
	}
	public class CheckEditAutomationPeer : BaseEditAutomationPeer, IToggleProvider {
		new protected CheckEdit Editor { get { return base.Editor as CheckEdit; } }
		public CheckEditAutomationPeer(CheckEdit element)
			: base(element) {
		}
		static ToggleState ConvertToToggleState(bool? isChecked) {
			if(isChecked.HasValue) {
				if(isChecked.Value)
					return ToggleState.On;
				return ToggleState.Off;
			}
			return ToggleState.Indeterminate;
		}
		public override object GetPattern(PatternInterface patternInterface) {
			if(patternInterface == PatternInterface.Toggle)
				return this;
			return base.GetPattern(patternInterface);
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.CheckBox;
		}
		protected override string GetNameCore() {
			object content = Editor.Content;
			if (content == null || (content is FrameworkElement))
				return string.Empty;
			return content.ToString();
		}
		internal void RaiseToggleStatePropertyChangedEvent(bool? oldValue, bool? newValue) {
			if (oldValue != newValue) {
				base.RaisePropertyChangedEvent(TogglePatternIdentifiers.ToggleStateProperty, ConvertToToggleState(oldValue), ConvertToToggleState(newValue));
			}
		}
		#region IToggleProvider Members
		void IToggleProvider.Toggle() {
			if(!base.IsEnabled())
				throw new ElementNotEnabledException();
			Editor.OnToggle();
		}
		ToggleState IToggleProvider.ToggleState {
			get { return ConvertToToggleState(Editor.IsChecked); }
		}
		#endregion
	}
	public class TextEditAutomationPeer : BaseEditAutomationPeer {
		public TextEditAutomationPeer(TextEditBase element)
			: base(element) {
		}
		new protected TextEdit Editor { get { return base.Editor as TextEdit; } }
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.Edit;
		}
		public override object GetPattern(PatternInterface patternInterface) {
			object result = null;
			switch(patternInterface) {
#if !SL
				case PatternInterface.Text:
					result = GetTextInterface();
					break;
#endif
				case PatternInterface.Scroll:
					result = GetScrollInterface();
					break;
				default:
					result = base.GetPattern(patternInterface);
					break;
			}
			return result;
		}
		object GetPatternFromEditBox(PatternInterface pi) {
			if(Editor.EditCore != null) {
				AutomationPeer peer = FrameworkElementAutomationPeer.CreatePeerForElement(Editor.EditCore);
				return peer.GetPattern(pi);
			}
			return null;
		}
#if !SL
		object GetTextInterface() {
			return GetPatternFromEditBox(PatternInterface.Text);
		}
#endif
		object GetScrollInterface() {
			return GetPatternFromEditBox(PatternInterface.Scroll);
		}
	}
	public class ButtonEditAutomationPeer : TextEditAutomationPeer {
		new protected ButtonEdit Editor { get { return base.Editor as ButtonEdit; } }
		public ButtonEditAutomationPeer(ButtonEdit element)
			: base(element) {
		}
		protected override List<AutomationPeer> GetChildrenCore() {
			return base.GetChildrenCore();
		}
	}
	public class SpinEditAutomationPeer : ButtonEditAutomationPeer {
		new protected SpinEdit Editor { get { return base.Editor as SpinEdit; } }
		public SpinEditAutomationPeer(SpinEdit element)
			: base(element) {
		}
	}
	public class PopupBaseEditAutomationPeer : ButtonEditAutomationPeer, IExpandCollapseProvider {
		new protected PopupBaseEdit Editor { get { return base.Editor as PopupBaseEdit; } }
		public PopupBaseEditAutomationPeer(PopupBaseEdit element)
			: base(element) {
		}
		public override object GetPattern(PatternInterface pattern) {
			if(pattern == PatternInterface.ExpandCollapse)
				return this;
			return base.GetPattern(pattern);
		}
		#region IExpandCollapseProvider Members
		void IExpandCollapseProvider.Collapse() {
			Editor.IsPopupOpen = false;
		}
		void IExpandCollapseProvider.Expand() {
			Editor.IsPopupOpen = true;
		}
		ExpandCollapseState IExpandCollapseProvider.ExpandCollapseState {
			get {
				if(Editor.IsPopupOpen)
					return ExpandCollapseState.Expanded;
				return ExpandCollapseState.Collapsed;
			}
		}
		#endregion
	}
	public class ComboBoxEditAutomationPeer : PopupBaseEditAutomationPeer, ISelectionProvider {
		new protected ComboBoxEdit Editor { get { return base.Editor as ComboBoxEdit; } }
		public ComboBoxEditAutomationPeer(ComboBoxEdit element)
			: base(element) {
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.ComboBox;
		}
		public override object GetPattern(PatternInterface pattern) {
			if(pattern == PatternInterface.Selection)
				return this;
			return base.GetPattern(pattern);
		}
		protected ComboBoxEditItemAutomationPeer CreateItemAutomationPeer(object item) {
			return new ComboBoxEditItemAutomationPeer(item, Editor);
		}
		protected override List<AutomationPeer> GetChildrenCore() {
			return GetItemPeers(base.GetChildrenCore());
		}
		List<AutomationPeer> GetItemPeers() {
			return GetItemPeers(null);
		}
		List<AutomationPeer> GetItemPeers(List<AutomationPeer> baseChildren) {
#if !SL
			ClearAutomationEventsHelper.ClearAutomationEvents();
#endif
			List<AutomationPeer> result = new List<AutomationPeer>();
			if(baseChildren != null)
				result = new List<AutomationPeer>(baseChildren);
			if (!Editor.IsPopupOpen)
				return result;
			foreach(object item in Editor.ItemsProvider.VisibleListSource)
				result.Add(CreateItemAutomationPeer(item));
			return result;
		}
		#region ISelectionProvider Members
		IRawElementProviderSimple[] ISelectionProvider.GetSelection() {
			int count = Editor.SelectedItems.Count;
			List<AutomationPeer> itemPeers = GetItemPeers();
			if((count <= 0) || (itemPeers.Count <= 0))
				return null;
			List<IRawElementProviderSimple> list = new List<IRawElementProviderSimple>();
			foreach(ComboBoxEditItemAutomationPeer peer in itemPeers)
				if(peer != null)
					list.Add(base.ProviderFromPeer(peer));
			return list.ToArray();
		}
		bool ISelectionProvider.CanSelectMultiple {
			get { return Editor.EditStrategy.StyleSettings.GetSelectionMode(Editor) != SelectionMode.Single; }
		}
		bool ISelectionProvider.IsSelectionRequired {
			get { return false; }
		}
		#endregion
	}
	public class DateEditAutomationPeer : PopupBaseEditAutomationPeer {
		new protected DateEdit Editor { get { return base.Editor as DateEdit; } }
		public DateEditAutomationPeer(DateEdit element)
			: base(element) {
		}
	}
	public class MemoEditAutomationPeer : PopupBaseEditAutomationPeer {
		new protected MemoEdit Editor { get { return base.Editor as MemoEdit; } }
		public MemoEditAutomationPeer(MemoEdit element)
			: base(element) {
		}
	}
	public abstract class RangeBaseEditAutomationPeer : BaseEditAutomationPeer, IRangeValueProvider {
		new protected RangeBaseEdit Editor { get { return base.Editor as RangeBaseEdit; } }
		public RangeBaseEditAutomationPeer(RangeBaseEdit element)
			: base(element) {
		}
		public override object GetPattern(PatternInterface patternInterface) {
			if(patternInterface == PatternInterface.RangeValue)
				return this;
			return base.GetPattern(patternInterface);
		}
		internal void RaiseValuePropertyChangedEvent(double oldValue, double newValue) {
			base.RaisePropertyChangedEvent(RangeValuePatternIdentifiers.ValueProperty, oldValue, newValue);
		}
		#region IRangeValueProvider Members
		bool IRangeValueProvider.IsReadOnly {
			get { return Editor.IsReadOnly; }
		}
		double IRangeValueProvider.LargeChange {
			get { return Editor.LargeStep; }
		}
		double IRangeValueProvider.Maximum {
			get { return Editor.Maximum; }
		}
		double IRangeValueProvider.Minimum {
			get { return Editor.Minimum; }
		}
		void IRangeValueProvider.SetValue(double value) {
			Editor.Value = value;
		}
		double IRangeValueProvider.SmallChange {
			get { return Editor.SmallStep; }
		}
		double IRangeValueProvider.Value {
			get { return Editor.Value; }
		}
		#endregion
	}
	public class TrackBarEditAutomationPeer : RangeBaseEditAutomationPeer {
		new protected TrackBarEdit Editor { get { return base.Editor as TrackBarEdit; } }
		public TrackBarEditAutomationPeer(TrackBarEdit element)
			: base(element) {
		}
		public override object GetPattern(PatternInterface patternInterface) {
			if (Editor.IsRange)
				return (patternInterface == PatternInterface.Value) ? this : null;
			return base.GetPattern(patternInterface);
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.Slider;
		}
		protected override string GetValue() {
			if (Editor.IsRange)
				return GetValueCore(Editor.SelectionStart, Editor.SelectionEnd);
			return base.GetValue();
		}
		protected virtual string GetValueCore(double selectionStart, double selectionEnd) {
			return String.Format("selection start equals {0}, selection end equals {1}", selectionStart, selectionEnd);
		}
		protected override void SetValue(string value) {
			if (!Editor.IsRange)
				base.SetValue(value);
		}
		internal void RaiseSelectionEndPropertyChangedEvent(double oldValue, double newValue) {
			base.RaisePropertyChangedEvent(ValuePatternIdentifiers.ValueProperty, GetValueCore(Editor.SelectionStart, oldValue), GetValueCore(Editor.SelectionStart, newValue));
		}
		internal void RaiseSelectionStartPropertyChangedEvent(double oldValue, double newValue) {
			base.RaisePropertyChangedEvent(ValuePatternIdentifiers.ValueProperty, GetValueCore(oldValue, Editor.SelectionEnd), GetValueCore(newValue, Editor.SelectionEnd));
		}
	}
#if !SL
	public class ProgressBarEditAutomationPeer : RangeBaseEditAutomationPeer {
		new protected ProgressBarEdit Editor { get { return base.Editor as ProgressBarEdit; } }
		public ProgressBarEditAutomationPeer(ProgressBarEdit element)
			: base(element) {
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.ProgressBar;
		}
	}
#endif
	public class ListBoxEditAutomationPeer : BaseEditAutomationPeer, ISelectionProvider {
		ListBoxAutomationPeer ListBoxPeer { get { return Editor.ListBoxCore != null ? (ListBoxAutomationPeer)CreatePeerForElement(Editor.ListBoxCore) : null; } }
		new protected ListBoxEdit Editor { get { return base.Editor as ListBoxEdit; } }
		public ListBoxEditAutomationPeer(ListBoxEdit element)
			: base(element) {
		}
		protected override string GetClassNameCore() {
			return "ListBoxEdit";
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.List;
		}
		protected override List<AutomationPeer> GetChildrenCore() {
			if (Editor.ListBoxCore != null)
				return new List<AutomationPeer>() { ListBoxPeer };
			return null;
		}
		public override object GetPattern(PatternInterface patternInterface) {
			if (patternInterface == PatternInterface.Selection)
				return this;
			return base.GetPattern(patternInterface);
		}
		#region ISelectionProvider Members
		public bool CanSelectMultiple { get { return Editor.SelectionMode != SelectionMode.Single; } }
		public IRawElementProviderSimple[] GetSelection() {
			ISelectionProvider selector = ListBoxPeer;
			return selector != null ? selector.GetSelection() : null;
		}
		public bool IsSelectionRequired {
			get {
				ISelectionProvider selector = ListBoxPeer;
				return selector != null && selector.IsSelectionRequired;
			}
		}
		#endregion
	}
	public class ComboBoxEditItemAutomationPeer : AutomationPeer, ISelectionItemProvider, IScrollItemProvider {
		ComboBoxEdit Owner;
		protected object Item;
		public ComboBoxEditItemAutomationPeer(object item, ComboBoxEdit owner) {
			Owner = owner;
			Item = item;
		}
		bool CanSelectMultiple { get { return Owner.EditStrategy.StyleSettings.GetSelectionMode(Owner) != SelectionMode.Single; } }
		FrameworkElement GetWrapper() {
			if(Item is ComboBoxEditItem)
				return Item as FrameworkElement;
			if(Owner.ListBox != null)
				return Owner.ListBox.ItemContainerGenerator.ContainerFromItem(Item) as FrameworkElement;
			return null;
		}
		AutomationPeer GetWrapperPeer() {
			UIElement wrapper = GetWrapper();
			if(wrapper == null)
				return null;
			return FrameworkElementAutomationPeer.CreatePeerForElement(wrapper);
		}
		protected override string GetAcceleratorKeyCore() {
			AutomationPeer wrapperPeer = GetWrapperPeer();
			if(wrapperPeer != null)
				return wrapperPeer.GetAcceleratorKey();
			return string.Empty;
		}
		protected override string GetAccessKeyCore() {
			AutomationPeer wrapperPeer = GetWrapperPeer();
			if(wrapperPeer != null)
				return wrapperPeer.GetAccessKey();
			return string.Empty;
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.ListItem;
		}
		protected override string GetAutomationIdCore() {
			AutomationPeer wrapperPeer = GetWrapperPeer();
			if(wrapperPeer != null)
				return wrapperPeer.GetAutomationId();
			return string.Empty;
		}
		protected override Rect GetBoundingRectangleCore() {
			AutomationPeer wrapperPeer = GetWrapperPeer();
			if(wrapperPeer != null)
				return wrapperPeer.GetBoundingRectangle();
			return new Rect();
		}
		protected override List<AutomationPeer> GetChildrenCore() {
			AutomationPeer wrapperPeer = GetWrapperPeer();
			if(wrapperPeer != null)
				return wrapperPeer.GetChildren();
			return null;
		}
		protected override string GetClassNameCore() {
			return "ComboBoxEditItem";
		}
		protected override Point GetClickablePointCore() {
			AutomationPeer wrapperPeer = this.GetWrapperPeer();
			if(wrapperPeer != null)
				return wrapperPeer.GetClickablePoint();
			return new Point(double.NaN, double.NaN);
		}
		protected override string GetHelpTextCore() {
			AutomationPeer wrapperPeer = this.GetWrapperPeer();
			if(wrapperPeer != null)
				return wrapperPeer.GetHelpText();
			return string.Empty;
		}
		protected override string GetItemStatusCore() {
			AutomationPeer wrapperPeer = this.GetWrapperPeer();
			if(wrapperPeer != null)
				return wrapperPeer.GetItemStatus();
			return string.Empty;
		}
		protected override string GetItemTypeCore() {
			AutomationPeer wrapperPeer = this.GetWrapperPeer();
			if(wrapperPeer != null)
				return wrapperPeer.GetItemType();
			return string.Empty;
		}
		protected override AutomationPeer GetLabeledByCore() {
			AutomationPeer wrapperPeer = this.GetWrapperPeer();
			if(wrapperPeer != null)
				return wrapperPeer.GetLabeledBy();
			return null;
		}
		protected override string GetNameCore() {
			AutomationPeer wrapperPeer = this.GetWrapperPeer();
			string name = null;
			if(wrapperPeer != null)
				name = wrapperPeer.GetName();
			if((name == null) && (Item is string))
				name = (string)Item;
			if(name == null)
				name = string.Empty;
			return name;
		}
		protected override AutomationOrientation GetOrientationCore() {
			AutomationPeer wrapperPeer = this.GetWrapperPeer();
			if(wrapperPeer != null)
				return wrapperPeer.GetOrientation();
			return AutomationOrientation.None;
		}
		public override object GetPattern(PatternInterface patternInterface) {
			if(patternInterface == PatternInterface.SelectionItem)
				return this;
			if(patternInterface == PatternInterface.ScrollItem)
				return this;
			return null;
		}
		protected override bool HasKeyboardFocusCore() {
			AutomationPeer wrapperPeer = this.GetWrapperPeer();
			return wrapperPeer != null && wrapperPeer.HasKeyboardFocus();
		}
		protected override bool IsContentElementCore() {
			AutomationPeer wrapperPeer = this.GetWrapperPeer();
			if(wrapperPeer != null)
				return wrapperPeer.IsContentElement();
			return true;
		}
		protected override bool IsControlElementCore() {
			AutomationPeer wrapperPeer = this.GetWrapperPeer();
			if(wrapperPeer != null)
				return wrapperPeer.IsControlElement();
			return true;
		}
		protected override bool IsEnabledCore() {
			AutomationPeer wrapperPeer = this.GetWrapperPeer();
			return wrapperPeer != null && wrapperPeer.IsEnabled();
		}
		protected override bool IsKeyboardFocusableCore() {
			AutomationPeer wrapperPeer = this.GetWrapperPeer();
			return wrapperPeer != null && wrapperPeer.IsKeyboardFocusable();
		}
		protected override bool IsOffscreenCore() {
			AutomationPeer wrapperPeer = this.GetWrapperPeer();
			if(wrapperPeer != null)
				return wrapperPeer.IsOffscreen();
			return true;
		}
		protected override bool IsPasswordCore() {
			AutomationPeer wrapperPeer = this.GetWrapperPeer();
			return wrapperPeer != null && wrapperPeer.IsPassword();
		}
		protected override bool IsRequiredForFormCore() {
			AutomationPeer wrapperPeer = this.GetWrapperPeer();
			return ((wrapperPeer != null) && wrapperPeer.IsRequiredForForm());
		}
		protected override void SetFocusCore() {
			AutomationPeer wrapperPeer = this.GetWrapperPeer();
			if(wrapperPeer != null)
				wrapperPeer.SetFocus();
		}
		#region ISelectionItemProvider Members
		bool ISelectionItemProvider.IsSelected {
			get {
				if(CanSelectMultiple)
					return Owner.SelectedItems.Contains(Item);
				return Owner.SelectedItem == Item;
			}
		}
		void ISelectionItemProvider.AddToSelection() {
			if(CanSelectMultiple && !Owner.SelectedItems.Contains(Item))
				Owner.SelectedItems.Add(Item);
			else
				Owner.SelectedItem = Item;
		}
		void ISelectionItemProvider.RemoveFromSelection() {
			if(CanSelectMultiple && Owner.SelectedItems.Contains(Item))
				Owner.SelectedItems.Remove(Item);
			else
				if(Item == Owner.SelectedItem)
					Owner.SelectedItem = null;
		}
		void ISelectionItemProvider.Select() {
			if(CanSelectMultiple) {
				Owner.SelectedItems.Clear();
				Owner.SelectedItems.Add(Item);
			}
			Owner.SelectedItem = Item;
		}
		IRawElementProviderSimple ISelectionItemProvider.SelectionContainer {
			get { return base.ProviderFromPeer(FrameworkElementAutomationPeer.CreatePeerForElement(Owner)); }
		}
		#endregion
		#region IScrollItemProvider Members
		void IScrollItemProvider.ScrollIntoView() {
			if(Owner.ListBox != null)
				Owner.ListBox.ScrollIntoView(Item);
		}
		#endregion
#if SL
		protected override string GetLocalizedControlTypeCore() {
			return "ListItem";
		}
#endif
	}
	public class CheckEditBoxAutomationPeer : ToggleButtonAutomationPeer {
		public CheckEditBoxAutomationPeer(CheckEditBox owner) : base(owner) { }
		protected override bool IsControlElementCore() {
			return false;
		}
	}
	public class RangeEditBasePanelAutomationPeer : FrameworkElementAutomationPeer {
		public RangeEditBasePanelAutomationPeer(RangeEditBasePanel owner) : base(owner) { }
		protected override bool IsControlElementCore() {
			return false;
		}
	}
}
