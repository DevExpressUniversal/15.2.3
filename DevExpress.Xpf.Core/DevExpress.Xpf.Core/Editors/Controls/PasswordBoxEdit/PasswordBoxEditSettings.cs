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
using System.ComponentModel;
using System.Collections;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
#if !SL
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Editors.Popups;
using System.Windows.Markup;
using DevExpress.Xpf.Editors.Settings.Extension;
#else
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.WPFToSLUtils;
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
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
namespace DevExpress.Xpf.Editors.Settings {
#if !SL
	public class PasswordBoxEditSettingsExtension : BaseSettingsExtension {
		public int MaxLength { get; set; }
		public char PasswordChar { get; set; }
		public PasswordBoxEditSettingsExtension() {
			MaxLength = (int)PasswordBoxEdit.MaxLengthProperty.DefaultMetadata.DefaultValue;
			PasswordChar = (char)PasswordBoxEdit.PasswordCharProperty.DefaultMetadata.DefaultValue;
		}
		protected override BaseEditSettings CreateEditSettings() {
			return new PasswordBoxEditSettings() {
				MaxLength = this.MaxLength,
				PasswordChar = this.PasswordChar
			};
		}
	}
#endif
	public class PasswordBoxEditSettings : BaseEditSettings {
		public static readonly DependencyProperty PasswordCharProperty;
		public static readonly DependencyProperty MaxLengthProperty;
		static PasswordBoxEditSettings() {
			Type ownerType = typeof(PasswordBoxEditSettings);
			PasswordCharProperty = DependencyPropertyManager.Register("PasswordChar", typeof(char), ownerType, new FrameworkPropertyMetadata('●'));
			MaxLengthProperty = DependencyPropertyManager.Register("MaxLength", typeof(int), ownerType, new FrameworkPropertyMetadata(0));	 
		}
		public char PasswordChar {
			get { return (char)GetValue(PasswordCharProperty); }
			set { SetValue(PasswordCharProperty, value); }
		}
		public int MaxLength {
			get { return (int)GetValue(MaxLengthProperty); }
			set { SetValue(MaxLengthProperty, value); }
		}
		protected override void AssignToEditCore(IBaseEdit edit) {
			base.AssignToEditCore(edit);
			PasswordBoxEdit editor = edit as PasswordBoxEdit;
			if(editor == null)
				return;
			SetValueFromSettings(PasswordCharProperty, () => editor.PasswordChar = this.PasswordChar);
			SetValueFromSettings(MaxLengthProperty, () => editor.MaxLength = this.MaxLength);
		}
		public override string GetDisplayTextFromEditor(object editValue) {
			string displayText = base.GetDisplayTextFromEditor(editValue);
			return string.IsNullOrEmpty(displayText) ? displayText : new string(PasswordChar, displayText.Length);
		}
		protected internal override bool IsActivatingKey(Key key, ModifierKeys modifiers) {
			if (IsPasteGesture(key, modifiers))
				return true;
			return base.IsActivatingKey(key, modifiers);
		}
	}
}
