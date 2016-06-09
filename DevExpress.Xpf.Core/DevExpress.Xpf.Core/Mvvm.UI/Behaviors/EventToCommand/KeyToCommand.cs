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

using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
#if FREE
using DevExpress.Mvvm.UI.Native;
#else
using DevExpress.UI.Xaml.Editors.Native;
#endif
#else
using DevExpress.Mvvm.UI.Native;
#endif
namespace DevExpress.Mvvm.UI {
	public class KeyToCommand : EventToCommandBase {
#if !NETFX_CORE
		public static readonly DependencyProperty KeyGestureProperty =
			DependencyProperty.Register("KeyGesture", typeof(KeyGesture), typeof(KeyToCommand),
			new PropertyMetadata(null));
		public KeyGesture KeyGesture {
			get { return (KeyGesture)GetValue(KeyGestureProperty); }
			set { SetValue(KeyGestureProperty, value); }
		}
#else
		public static readonly DependencyProperty KeyGestureProperty =
			DependencyProperty.Register("KeyGesture", typeof(string), typeof(KeyToCommand),
			new PropertyMetadata(null, (d, e) => ((KeyToCommand)d).OnKeyGestureChanged(e)));
		void OnKeyGestureChanged(DependencyPropertyChangedEventArgs e) {
			keyGesture = (string)e.NewValue;
		}
		public string KeyGesture {
			get { return (string)GetValue(KeyGestureProperty); }
			set { SetValue(KeyGestureProperty, value); }
		}
		KeyGesture keyGesture = null;
#endif
#if !NETFX_CORE && !SILVERLIGHT
		static KeyToCommand() {
			EventNameProperty.OverrideMetadata(typeof(KeyToCommand), new PropertyMetadata("KeyUp"));
		}
#else
		public KeyToCommand() {
			EventName = "KeyUp";
		}
#endif
		protected override void Invoke(object sender, object eventArgs) {
			if(Command.CanExecute(CommandParameter))
				Command.Execute(CommandParameter);
		}
		protected override bool CanInvoke(object sender, object eventArgs) {
			bool res = base.CanInvoke(sender, eventArgs);
#if SILVERLIGHT
			if(KeyGesture == null || !(eventArgs is KeyEventArgs)) return res;
			KeyEventArgs keyEventArgs = (KeyEventArgs)eventArgs;
			return res && keyEventArgs.Key == KeyGesture.Key && Keyboard.Modifiers == KeyGesture.ModifierKeys;
#elif NETFX_CORE
			if(keyGesture == null || !(eventArgs is KeyRoutedEventArgs)) return res;
			KeyRoutedEventArgs keyEventArgs = (KeyRoutedEventArgs)eventArgs;
			return res && keyEventArgs.Key == keyGesture.Key && ModifierKeysHelper.GetKeyboardModifiers() == keyGesture.ModifierKeys;
#else
			if(KeyGesture == null || !(eventArgs is InputEventArgs)) return res;
			InputEventArgs inputEventArgs = (InputEventArgs)eventArgs;
			return res && KeyGesture.Matches(Source, inputEventArgs);
#endif
		}
	}
}
