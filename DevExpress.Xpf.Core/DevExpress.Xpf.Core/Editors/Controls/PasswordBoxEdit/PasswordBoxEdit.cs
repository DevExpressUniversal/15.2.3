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
using System.Collections;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Windows.Markup;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Windows.Automation.Peers;
#if !SL
using DevExpress.Data.Access;
using DevExpress.Xpf.Editors.EditStrategy;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.Themes;
using DevExpress.Xpf.Utils;
using DevExpress.Data.Mask;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Automation;
using System.Security;
#else
using System.Windows.Media;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.EditStrategy;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.WPFCompatibility;
using DevExpress.Xpf.Editors.WPFCompatibility.Extensions;
using DevExpress.Data.Mask;
using DevExpress.WPFToSLUtils;
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Editors.Automation;
#endif
#if SL
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
#endif
namespace DevExpress.Xpf.Editors {
	[DXToolboxBrowsable(DXToolboxItemKind.Free)]
	public class PasswordBoxEdit : TextEditBase {
		internal static readonly DependencyPropertyKey PasswordStrengthPropertyKey;
		public static readonly DependencyProperty PasswordStrengthProperty;
		public static readonly DependencyProperty PasswordCharProperty;
		public static readonly DependencyProperty PasswordProperty;
		public static readonly RoutedEvent CustomPasswordStrengthEvent;
#if !SL
		public static readonly DependencyProperty ShowCapsLockWarningToolTipProperty;
		public static readonly DependencyProperty CapsLockWarningToolTipTemplateProperty;
#endif
		static PasswordBoxEdit() {
			Type ownerType = typeof(PasswordBoxEdit);
			PasswordCharProperty = DependencyPropertyManager.Register("PasswordChar", typeof(char), ownerType, new FrameworkPropertyMetadata('●', FrameworkPropertyMetadataOptions.None,
				PasswordCharPropertyChanged));
			PasswordProperty = DependencyPropertyManager.Register("Password", typeof(string), ownerType, new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.None,
				PasswordPropertyChanged, CoercePasswordProperty));
			PasswordStrengthPropertyKey = DependencyPropertyManager.RegisterReadOnly("PasswordStrength", typeof(PasswordStrength), ownerType, new FrameworkPropertyMetadata(PasswordStrength.Weak));
			PasswordStrengthProperty = PasswordStrengthPropertyKey.DependencyProperty;
			CustomPasswordStrengthEvent = EventManager.RegisterRoutedEvent("CustomPasswordStrength", RoutingStrategy.Direct, typeof(CustomPasswordStrengthEventHandler), ownerType);
#if !SL
			ShowCapsLockWarningToolTipProperty = DependencyPropertyManager.Register("ShowCapsLockWarningToolTip", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
			CapsLockWarningToolTipTemplateProperty = DependencyPropertyManager.Register("CapsLockWarningToolTipTemplate", typeof(DataTemplate), ownerType, new FrameworkPropertyMetadata(null));
#endif
		}
		static object CoercePasswordProperty(DependencyObject d, object value) {
			return ((PasswordBoxEdit)d).EditStrategy.CoercePassword(value);
		}
		static void PasswordCharPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PasswordBoxEdit)d).PasswordCharChanged((char)e.NewValue);
		}
		static void PasswordPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PasswordBoxEdit)d).PasswordChanged((string)e.OldValue, (string)e.NewValue);
		}
		public event CustomPasswordStrengthEventHandler CustomPasswordStrength {
			add { this.AddHandler(CustomPasswordStrengthEvent, value); }
			remove { this.RemoveHandler(CustomPasswordStrengthEvent, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PasswordBoxEditPasswordStrength")]
#endif
		public PasswordStrength PasswordStrength {
			get { return (PasswordStrength)GetValue(PasswordStrengthProperty); }
			internal set { this.SetValue(PasswordStrengthPropertyKey, value); }
		}
#if !SL
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PasswordBoxEditShowCapsLockWarningToolTip")]
#endif
		public bool ShowCapsLockWarningToolTip {
			get { return (bool)GetValue(ShowCapsLockWarningToolTipProperty); }
			set { SetValue(ShowCapsLockWarningToolTipProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PasswordBoxEditCapsLockWarningToolTipTemplate")]
#endif
		public DataTemplate CapsLockWarningToolTipTemplate {
			get { return (DataTemplate)GetValue(CapsLockWarningToolTipTemplateProperty); }
			set { SetValue(CapsLockWarningToolTipTemplateProperty, value); }
		}
#endif
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PasswordBoxEditPasswordChar")]
#endif
#if SL
		[TypeConverter(typeof(CharConverter))]
#endif
		public char PasswordChar {
			get { return (char)GetValue(PasswordCharProperty); }
			set { SetValue(PasswordCharProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("PasswordBoxEditPassword")]
#endif
		public string Password {
			get { return (string)GetValue(PasswordProperty); }
			set { SetValue(PasswordProperty, value); }
		}
		internal PasswordBox PasswordBox { get { return EditCore as PasswordBox; } }
		protected new PasswordBoxEditStrategy EditStrategy { get { return (PasswordBoxEditStrategy)base.EditStrategy; } }
		protected override EditStrategyBase CreateEditStrategy() {
			return new PasswordBoxEditStrategy(this);
		}
		public PasswordBoxEdit() {
			this.SetDefaultStyleKey(typeof(PasswordBoxEdit));
		}
		protected override EditBoxWrapper CreateEditBoxWrapper() {
			return new PasswordBoxWrapper(this);
		}
		protected virtual void PasswordCharChanged(char value) {
			EditStrategy.PasswordCharChanged(value);
		}
		protected virtual void PasswordChanged(string oldValue, string value) {
			EditStrategy.PasswordChanged(oldValue, value);
		}
		protected override void SubscribeEditEventsCore() {
			base.SubscribeEditEventsCore();
			if(PasswordBox != null)
				PasswordBox.PasswordChanged += PasswordBoxPasswordChanged;
		}
		protected override void UnsubscribeEditEventsCore() {
			base.UnsubscribeEditEventsCore();
			if(PasswordBox != null)
				PasswordBox.PasswordChanged -= PasswordBoxPasswordChanged;
		}
		void PasswordBoxPasswordChanged(object sender, System.Windows.RoutedEventArgs e) {
			EditStrategy.SyncWithEditor();
		}
		protected override string GetExportText() {
			return DisplayText == null ? string.Empty : new string(PasswordChar, DisplayText.Length);
		}
#if !SL
		bool shouldHandle;
		protected override void ProcessFocusEditCore(MouseButtonEventArgs e) {
			shouldHandle = FocusEditCore();
		}
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
			base.OnMouseLeftButtonDown(e);
			e.Handled = shouldHandle;
		}
#else
		protected override void SLOnEditCoreAssigned(FrameworkElement oldValue) {
			base.SLOnEditCoreAssigned(oldValue);
			UpdateIsTabStop();
		}
		protected override bool GetIsTabStopCore() {
			return EditCore == null;
		}
#endif
		protected override ActualPropertyProvider CreateActualPropertyProvider() {
			return new TextEditPropertyProvider(this);
		}
	}
}
