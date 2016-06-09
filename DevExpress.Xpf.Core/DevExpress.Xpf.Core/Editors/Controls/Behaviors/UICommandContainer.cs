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
using System.Windows.Input;
using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Editors {
	public class UICommandContainer : DXFrameworkElement {
		public static readonly DependencyProperty IdProperty;
		public static readonly DependencyProperty CaptionProperty;
		public static readonly DependencyProperty CommandProperty;
		public static readonly DependencyProperty IsDefaultProperty;
		public static readonly DependencyProperty IsCancelProperty;
		static UICommandContainer() {
			IdProperty = DependencyPropertyRegistrator.Register<UICommandContainer, object>(owner => owner.Id, null);
			CaptionProperty = DependencyPropertyRegistrator.Register<UICommandContainer, object>(owner => owner.Caption, null);
			CommandProperty = DependencyPropertyRegistrator.Register<UICommandContainer, ICommand>(owner => owner.Command, null);
			IsDefaultProperty = DependencyPropertyRegistrator.Register<UICommandContainer, bool>(owner => owner.IsDefault, false);
			IsCancelProperty = DependencyPropertyRegistrator.Register<UICommandContainer, bool>(owner => owner.IsCancel, false);
		}
		public UICommandContainer() {
		}
		public UICommandContainer(UICommand command) {
			Id = command.Id;
			Caption = command.Caption;
			Command = command.Command;
			IsDefault = command.IsDefault;
			IsCancel = command.IsCancel;
			Tag = command.Tag;
		}
		public object Id {
			get { return GetValue(IdProperty); }
			set { SetValue(IdProperty, value); }
		}
		public object Caption {
			get { return GetValue(CaptionProperty); }
			set { SetValue(CaptionProperty, value); }
		}
		public ICommand Command {
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}
		public bool IsDefault {
			get { return (bool)GetValue(IsDefaultProperty); }
			set { SetValue(IsDefaultProperty, value); }
		}
		public bool IsCancel {
			get { return (bool)GetValue(IsCancelProperty); }
			set { SetValue(IsCancelProperty, value); }
		}
	}
}
