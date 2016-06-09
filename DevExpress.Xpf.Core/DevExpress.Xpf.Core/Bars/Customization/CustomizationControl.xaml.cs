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
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Utils;
using System;
using System.Windows.Controls;
using DevExpress.Xpf.Bars.Native;
namespace DevExpress.Xpf.Bars.Customization {
	public partial class CustomizationControl : UserControl {
		public static readonly DependencyProperty ToolbarsCustomizationControlProperty =
			DependencyPropertyManager.Register("ToolbarsCustomizationControl", typeof(ToolbarsCustomizationControl), typeof(CustomizationControl), new PropertyMetadata());
		public static readonly DependencyProperty CommandsCustomizationControlProperty =
			DependencyPropertyManager.Register("CommandsCustomizationControl", typeof(CommandsCustomizationControl), typeof(CustomizationControl), new PropertyMetadata());
		public static readonly DependencyProperty OptionsControlProperty =
			DependencyPropertyManager.Register("OptionsControl", typeof(OptionsControl), typeof(CustomizationControl), new PropertyMetadata());
		public ToolbarsCustomizationControl ToolbarsCustomizationControl {
			get { return (ToolbarsCustomizationControl)GetValue(ToolbarsCustomizationControlProperty); }
			set { SetValue(ToolbarsCustomizationControlProperty, value); }
		}
		public CommandsCustomizationControl CommandsCustomizationControl {
			get { return (CommandsCustomizationControl)GetValue(CommandsCustomizationControlProperty); }
			set { SetValue(CommandsCustomizationControlProperty, value); }
		}
		public OptionsControl OptionsControl {
			get { return (OptionsControl)GetValue(OptionsControlProperty); }
			set { SetValue(OptionsControlProperty, value); }
		}
		protected readonly BarManagerCustomizationHelper helper;
		public DXTabItem ToolbarsCustomizationTab { get { return this.tabItemToolbars; } }
		public DXTabItem CommandsCustomizationTab { get { return this.tabItemCommands; } }
		public DXTabItem OptionsTab { get { return this.tabItemOptions; } }
		public CustomizationControl(BarManagerCustomizationHelper helper) {
			this.helper = helper;
			DataContext = this;
			InitializeComponent();
			InitializeCustomizationControls();
		}
		protected virtual ToolbarsCustomizationControl CreateToolbarsCustomizationControl() {
			return new ToolbarsCustomizationControl(helper);
		}
		protected virtual CommandsCustomizationControl CreateCommandsCustomizationControl() {
			return new CommandsCustomizationControl(helper);
		}
		protected virtual OptionsControl CreateOptionsControl() {
			return new OptionsControl(helper);
		}
		protected virtual void OnCloseButtonClick(object sender, RoutedEventArgs e) {
			helper.CloseCustomizationForm();
		}
		protected virtual void OnLoaded(object sender, RoutedEventArgs e) {
			Keyboard.Focus(tabControl);
		}
		void InitializeCustomizationControls() {
			ToolbarsCustomizationControl = CreateToolbarsCustomizationControl();
			CommandsCustomizationControl = CreateCommandsCustomizationControl();
			OptionsControl = CreateOptionsControl();			
		}
	}
}
