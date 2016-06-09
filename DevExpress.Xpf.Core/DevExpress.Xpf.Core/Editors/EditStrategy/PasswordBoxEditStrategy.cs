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
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Globalization;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Validation.Native;
#if SL
using ContextMenu = System.Windows.Controls.SLContextMenu;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using TextBox = DevExpress.Xpf.Editors.Controls.SLTextBox;
#endif
namespace DevExpress.Xpf.Editors.EditStrategy {
	public partial class PasswordBoxEditStrategy : TextEditStrategy {
		protected internal override bool ShouldApplyNullTextToDisplayText { get { return false; } }
		protected new PasswordBoxEdit Editor { get { return base.Editor as PasswordBoxEdit; } }
		PasswordBox PasswordBox { get { return Editor.PasswordBox; } }
		public PasswordBoxEditStrategy(PasswordBoxEdit editor) : base(editor) {
		}
		public virtual void PasswordCharChanged(char value) {
			UpdateDisplayText();
		}
		protected override void ProcessPreviewKeyDownInternal(KeyEventArgs e) {
			base.ProcessPreviewKeyDownInternal(e);
			if (!AllowEditing) {
				if (e.Key == Key.Space || e.Key == Key.Back || e.Key == Key.Delete)
					e.Handled = true;
			}
#if !SL
			CoerceToolTip();
#endif
		}
		protected override void RegisterUpdateCallbacks() {
			base.RegisterUpdateCallbacks();
			PropertyUpdater.Register(PasswordBoxEdit.EditValueProperty, baseValue => GetBaseValue(baseValue), baseValue => baseValue);
			PropertyUpdater.Register(PasswordBoxEdit.PasswordProperty, baseValue => baseValue, baseValue => baseValue != null ? baseValue.ToString() : string.Empty);
			PropertyUpdater.Register(PasswordBoxEdit.PasswordStrengthPropertyKey, baseValue => baseValue, baseValue => CalcPasswordStrength(baseValue != null ? baseValue.ToString() : string.Empty), baseValue => Editor.PasswordStrength = (PasswordStrength)baseValue);
		}
		PasswordStrength CalcPasswordStrength(string password) {
			PasswordStrengthEventArgs args = new PasswordStrengthEventArgs(PasswordBoxEdit.CustomPasswordStrengthEvent) { Password = password };
			Editor.RaiseEvent(args);
			if (args.Handled)
				return args.PasswordStrength;
			return PasswordStrengthCalculator.Calculate(password);
		}
		object GetBaseValue(object baseValue) {
			if(baseValue == null || baseValue is string)
				return baseValue;
			try {
				object temp = Convert.ChangeType(Convert.ChangeType(baseValue, typeof(string), CultureInfo.CurrentCulture), baseValue.GetType(), CultureInfo.CurrentCulture);
				return object.Equals(baseValue, temp) ? baseValue : string.Empty;
			}
			catch {
				return string.Empty;
			}
		}
		public virtual object CoercePassword(object value) {
			return CoerceValue(PasswordBoxEdit.PasswordProperty, value);
		}
		public virtual void PasswordChanged(string oldValue, string value) {
			if(ShouldLockUpdate)
				return;
			SyncWithValue(PasswordBoxEdit.PasswordProperty, oldValue, value);
		}
		protected override void UpdateEditCoreTextInternal(string displayText) {
			base.UpdateEditCoreTextInternal(displayText);
			EditBox.EditValue = displayText;
		}
		public override bool CanPaste() {
			return base.CanPaste() && AllowEditing;
		}
#if !SL
		public override void OnPreviewTextInput(TextCompositionEventArgs e) {
			if (!AllowEditing)
				e.Handled = true;
			base.OnPreviewTextInput(e);
		}
		public override object CoerceValidationToolTip(object tooltip) {
			if(CapsLockHelper.IsCapsLockToggled)
				return ShowCapsLockToolTip(tooltip);
			return base.CoerceValidationToolTip(tooltip);
		}
		protected virtual object ShowCapsLockToolTip(object tooltip) {
			return Editor.ShowCapsLockWarningToolTip ? new ContentControl { ContentTemplate = Editor.CapsLockWarningToolTipTemplate } : tooltip;
		}
#endif
	}
}
