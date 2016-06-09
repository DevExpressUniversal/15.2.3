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
using System.IO;
using DevExpress.DemoData.Utils;
using DevExpress.DemoData.Helpers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using DevExpress.DemoData.Model;
namespace DevExpress.DemoData.DemoParts {
	class DemoCenterErrorMessage : Control, IMessageBoxHelperSupport {
		#region Dependency Properties
		public static readonly DependencyProperty MessageProperty;
		static DemoCenterErrorMessage() {
			Type ownerType = typeof(DemoCenterErrorMessage);
			DefaultStyleKeyProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(ownerType));
			MessageProperty = DependencyProperty.Register("Message", typeof(string), ownerType, new PropertyMetadata(null));
		}
		public bool ShowIgnoreButton {
			get { return (bool)GetValue(ShowIgnoreButtonProperty); }
			set { SetValue(ShowIgnoreButtonProperty, value); }
		}
		public static readonly DependencyProperty ShowIgnoreButtonProperty =
			DependencyProperty.Register("ShowIgnoreButton", typeof(bool), typeof(DemoCenterErrorMessage), new PropertyMetadata(false));
		public bool IsError {
			get { return (bool)GetValue(IsErrorProperty); }
			set { SetValue(IsErrorProperty, value); }
		}
		public static readonly DependencyProperty IsErrorProperty =
			DependencyProperty.Register("IsError", typeof(bool), typeof(DemoCenterErrorMessage), new PropertyMetadata(true));
		#endregion
		public string Message { get { return (string)GetValue(MessageProperty); } set { SetValue(MessageProperty, value); } }
		public MessageBoxHelperResult Result { get; private set; }
		public event EventHandler Close;
		void OnOKButtonClick(object sender, RoutedEventArgs e) {
			SetResultAndClose(MessageBoxHelperResult.OK);
		}
		void OnIgnoreButtonClick(object sender, RoutedEventArgs e) {
			SetResultAndClose(MessageBoxHelperResult.Ignore);
		}
		void SetResultAndClose(MessageBoxHelperResult result) {
			Result = result;
			if(Close != null)
				Close(this, EventArgs.Empty);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			Button okButton = (Button)GetTemplateChild("OKButton");
			Button ignoreButton = (Button)GetTemplateChild("IgnoreButton");
			if(okButton != null)
				okButton.Click += OnOKButtonClick;
			if(ignoreButton != null)
				ignoreButton.Click += OnIgnoreButtonClick;
		}
	}
	public class DefaultDemoRunnerMessageBox : IDemoRunnerMessageBox {
		public MessageBoxHelperResult Show(string message, bool isError, bool showIgnoreButton) {
			return MessageBoxHelper.Show(
			new DevExpress.DemoData.DemoParts.DemoCenterErrorMessage() {
				Message = message,
				IsError = isError,
				ShowIgnoreButton = showIgnoreButton
			});
		}
	}
}
