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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Collections;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Data;
using System.ComponentModel;
using System.Windows.Automation.Peers;
using DevExpress.Xpf.Editors.EditStrategy;
#if !SL
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Automation;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Utils;
#else
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Editors.Helpers;
#endif
#if SL
using ContextMenu = System.Windows.Controls.SLContextMenu;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using Keyboard = DevExpress.Xpf.Editors.WPFCompatibility.SLKeyboard;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using TextBox = DevExpress.Xpf.Editors.Controls.SLTextBox;
#endif
namespace DevExpress.Xpf.Editors {
	[DXToolboxBrowsable(DXToolboxItemKind.Free)]
	public partial class MemoEdit : PopupBaseEdit {
		#region static
		public static readonly DependencyProperty MemoTextWrappingProperty;
		public static readonly DependencyProperty MemoHorizontalScrollBarVisibilityProperty;
		public static readonly DependencyProperty MemoVerticalScrollBarVisibilityProperty;
		public static readonly DependencyProperty MemoAcceptsReturnProperty;
		public static readonly DependencyProperty MemoAcceptsTabProperty;
		public static readonly DependencyProperty ShowIconProperty;
		protected internal const double popupMinHeightConstant = 100d;
		static MemoEdit() {
			Type ownerType = typeof(MemoEdit);
			MemoTextWrappingProperty = DependencyPropertyManager.Register("MemoTextWrapping", typeof(TextWrapping), ownerType, new FrameworkPropertyMetadata(TextWrapping.NoWrap));
			MemoHorizontalScrollBarVisibilityProperty = DependencyPropertyManager.Register("MemoHorizontalScrollBarVisibility", typeof(ScrollBarVisibility), ownerType, new FrameworkPropertyMetadata(ScrollBarVisibility.Hidden));
			MemoVerticalScrollBarVisibilityProperty = DependencyPropertyManager.Register("MemoVerticalScrollBarVisibility", typeof(ScrollBarVisibility), ownerType, new FrameworkPropertyMetadata(ScrollBarVisibility.Hidden));
			MemoAcceptsReturnProperty = DependencyPropertyManager.Register("MemoAcceptsReturn", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			ShowIconProperty = DependencyPropertyManager.Register("ShowIcon", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			MemoAcceptsTabProperty = DependencyPropertyManager.Register("MemoAcceptsTab", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			PopupFooterButtonsProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(DevExpress.Xpf.Editors.PopupFooterButtons.OkCancel));
			ShowSizeGripProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(true));
			PopupMinHeightProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(popupMinHeightConstant));
		}
		#endregion
		#region constuctors
		public MemoEdit() {
			this.SetDefaultStyleKey(typeof(MemoEdit));
		}
		#endregion
		#region overrides
		protected override bool IsClosePopupWithAcceptGesture(Key key, ModifierKeys modifiers) {
			return key == Key.Enter && ((ModifierKeysHelper.IsCtrlPressed(modifiers) && MemoAcceptsReturn) || (ModifierKeysHelper.NoModifiers(modifiers) && !MemoAcceptsReturn));
		}
		protected override EditStrategyBase CreateEditStrategy() {
			return new MemoEditStrategy(this);
		}
		protected override void AcceptPopupValue() {
			base.AcceptPopupValue();
			if(!IsReadOnly)
				SyncWithMemo();
		}
		private void SyncWithMemo() {
			EditStrategy.SyncWithMemo();
		}
		protected override void OnPopupOpened() {
			Memo = GetMemo();
			base.OnPopupOpened();
			EditStrategy.OnPopupOpened();
		}
		protected override void OnPopupClosed() {
			base.OnPopupClosed();
			Memo = null;
		}
		protected internal override FrameworkElement PopupElement {
			get { return Memo != null ? Memo.EditCore : null; }
		}
		protected override ActualPropertyProvider CreateActualPropertyProvider() {
			return new MemoEditPropertyProvider(this);
		}
		protected internal override Type StyleSettingsType { get { return typeof(MemoEditStyleSettings); } }
		#endregion
		#region methods
		void SyncMemoWithEditor(TextEdit memo) {
			EditStrategy.SyncMemoWithEditor(memo);
		}
		protected TextEdit GetMemo() {
			return (TextEdit)LayoutHelper.FindElement(Popup.Child as FrameworkElement, (FrameworkElement element) => { return element is TextEdit && element.Name == "PART_PopupContent"; });
		}
		protected internal void UpdateOkButtonIsEnabled(bool isTextModified) {
			PropertyProvider.PopupViewModel.OkButtonIsEnabled = isTextModified;
		}
		#endregion
		#region properties
		TextEdit memo;
		protected internal TextEdit Memo {
			get { return memo; }
			set {
				if(value == memo) return;
#if SL
				TextEdit oldValue = memo;
#endif
				memo = value;
				if(memo != null)
					SyncMemoWithEditor(memo);
#if SL
				PropertyChangedMemo(oldValue);
#endif
			}
		}
		protected new MemoEditStrategy EditStrategy {
			get { return base.EditStrategy as MemoEditStrategy; }
			set { base.EditStrategy = value; }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("MemoEditMemoTextWrapping"),
#endif
 Category("Behavior")]
		public TextWrapping MemoTextWrapping {
			get { return (TextWrapping)GetValue(MemoTextWrappingProperty); }
			set { SetValue(MemoTextWrappingProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("MemoEditMemoAcceptsReturn"),
#endif
 Category("Behavior")]
		public bool MemoAcceptsReturn {
			get { return (bool)GetValue(MemoAcceptsReturnProperty); }
			set { SetValue(MemoAcceptsReturnProperty, value); }
		}
#if !SL
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("MemoEditMemoAcceptsTab"),
#endif
 Category("Behavior")]
		public bool MemoAcceptsTab {
			get { return (bool)GetValue(MemoAcceptsTabProperty); }
			set { SetValue(MemoAcceptsTabProperty, value); }
		}
#endif
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("MemoEditMemoHorizontalScrollBarVisibility"),
#endif
 Category("Behavior")]
		public ScrollBarVisibility MemoHorizontalScrollBarVisibility {
			get { return (ScrollBarVisibility)GetValue(MemoHorizontalScrollBarVisibilityProperty); }
			set { SetValue(MemoHorizontalScrollBarVisibilityProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("MemoEditMemoVerticalScrollBarVisibility"),
#endif
 Category("Behavior")]
		public ScrollBarVisibility MemoVerticalScrollBarVisibility {
			get { return (ScrollBarVisibility)GetValue(MemoVerticalScrollBarVisibilityProperty); }
			set { SetValue(MemoVerticalScrollBarVisibilityProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("MemoEditShowIcon"),
#endif
 Category("Behavior")]
		public bool ShowIcon {
			get { return (bool)GetValue(ShowIconProperty); }
			set { SetValue(ShowIconProperty, value); }
		}
		#endregion
#if !SL
		protected override AutomationPeer OnCreateAutomationPeer() {
			return new MemoEditAutomationPeer(this);
		}
#endif
	}
	public class MemoEditIconIndexConverter : IValueConverter {
		#region IValueConverter Members
		object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return String.IsNullOrEmpty((string)value) ? 0 : 1;
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
	}
	public class MemoEditPropertyProvider : PopupBaseEditPropertyProvider {
		static MemoEditPropertyProvider() {
			IsTextEditableProperty.OverrideMetadata(typeof(MemoEditPropertyProvider), new PropertyMetadata(false));
		}
		public MemoEditPropertyProvider(TextEdit editor)
			: base(editor) {
		}
		protected override bool GetFocusPopupOnOpenInternal() {
			return true;
		}
		public override bool CalcSuppressFeatures() {
			return false;
		}
	}
	public class MemoEditStyleSettings : PopupBaseEditStyleSettings {
		protected internal override PopupCloseMode GetClosePopupOnClickMode(PopupBaseEdit editor) {
			return PopupCloseMode.Normal;
		}
		public override bool GetIsTextEditable(ButtonEdit editor) {
			return false;
		}
	}
}
