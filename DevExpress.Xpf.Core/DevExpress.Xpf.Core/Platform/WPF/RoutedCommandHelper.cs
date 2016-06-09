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

using System.Windows;
using System;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
namespace DevExpress.Xpf.Core.Native {
	public interface ICommandProvider {
		ICommand GetCommand();
	}
	public class RoutedCommandHelper {
		[IgnoreDependencyPropertiesConsistencyChecker()]
		public static readonly DependencyProperty CommandProperty;
		[IgnoreDependencyPropertiesConsistencyChecker()]
		public static readonly DependencyProperty CommandParameterProperty;
		[IgnoreDependencyPropertiesConsistencyChecker()]
		public static readonly DependencyProperty CommandTargetProperty;
		static RoutedCommandHelper() {
			Type ownerType = typeof(RoutedCommandHelper);
			CommandProperty = DependencyProperty.RegisterAttached("Command", typeof(ICommandProvider), ownerType, new PropertyMetadata(PropertyChangedCommand));
			CommandParameterProperty = DependencyProperty.RegisterAttached("CommandParameter", typeof(object), ownerType, new PropertyMetadata(PropertyChangedCommandParameter));
			CommandTargetProperty = DependencyProperty.RegisterAttached("CommandTarget", typeof(object), ownerType, new PropertyMetadata(PropertyChangedCommandTarget));
		}
		public static object GetCommand(DependencyObject d) { return d.GetValue(CommandProperty); }
		public static void SetCommand(DependencyObject d, ICommandProvider value) { d.SetValue(CommandProperty, value); }
		public static object GetCommandParameter(DependencyObject d) { return d.GetValue(CommandParameterProperty); }
		public static void SetCommandParameter(DependencyObject d, object value) { d.SetValue(CommandParameterProperty, value); }
		public static object GetCommandTarget(DependencyObject d) { return d.GetValue(CommandTargetProperty); }
		public static void SetCommandTarget(DependencyObject d, object value) { d.SetValue(CommandTargetProperty, value); }
		static void PropertyChangedCommand(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ICommandProvider commandProvider = (ICommandProvider)e.NewValue;
			d.SetValue(ButtonBase.CommandProperty, (commandProvider != null) ? commandProvider.GetCommand() : null);
		}
		static void PropertyChangedCommandParameter(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			d.SetValue(ButtonBase.CommandParameterProperty, e.NewValue);
		}
		static void PropertyChangedCommandTarget(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			d.SetValue(ButtonBase.CommandTargetProperty, e.NewValue);
		}
	}
}
