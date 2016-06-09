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
using System.Windows.Controls;
namespace DevExpress.Xpf.Core {
	public partial class DialogControl : UserControl {
		#region static
		public static readonly DependencyProperty DialogContentProperty =
			DependencyProperty.Register("DialogContent", typeof(FrameworkElement), typeof(DialogControl), new UIPropertyMetadata(null));
		public static readonly DependencyProperty UseContentIndentsProperty =
			DependencyProperty.Register("UseContentIndents", typeof(bool), typeof(DialogControl), new PropertyMetadata(false));
		public static readonly DependencyProperty ShowApplyButtonProperty =
			DependencyProperty.Register("ShowApplyButton", typeof(bool), typeof(DialogControl), new PropertyMetadata(false));
		#endregion static
		public DialogControl() {
			InitializeComponent();
		}
		public FrameworkElement DialogContent {
			get { return (FrameworkElement)GetValue(DialogContentProperty); }
			set { SetValue(DialogContentProperty, value); }
		}
		public bool ShowApplyButton {
			get { return (bool)GetValue(ShowApplyButtonProperty); }
			set { SetValue(ShowApplyButtonProperty, value); }
		}
		public bool UseContentIndents {
			get { return (bool)GetValue(UseContentIndentsProperty); }
			set { SetValue(UseContentIndentsProperty, value); }
		}
		IDialogContent IDialogContent {
			get { return DialogContent as IDialogContent; }
		}
		#region Click Handlers
		void okButton_Click(object sender, RoutedEventArgs e) {
			if(IDialogContent != null) {
				if(!IDialogContent.CanCloseWithOKResult())
					return;
				IDialogContent.OnOk();
			}
			FloatingContainer.CloseDialog(this, true);
		}
		void cancelButton_Click(object sender, RoutedEventArgs e) {
			FloatingContainer.CloseDialog(this, false);
		}
		void applyButton_Click(object sender, RoutedEventArgs e) {
			IDialogContent.OnApply();
		}
		#endregion Click Handlers
	}
}
