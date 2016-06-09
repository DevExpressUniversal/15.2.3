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
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using DevExpress.Data.WizardFramework;
namespace DevExpress.Xpf.Core.WizardFramework {
	public partial class WizardView : UserControl, IWizardView {
		public WizardView() {
			InitializeComponent();
		}
		private void previous_Click(object sender, RoutedEventArgs e) {
			if(Previous != null) {
				Previous(this, EventArgs.Empty);
			}
		}
		private void next_Click(object sender, RoutedEventArgs e) {
			if(Next != null) {
				Next(this, EventArgs.Empty);
			}
		}
		private void cancel_Click(object sender, RoutedEventArgs e) {
			if(Cancel != null) {
				Cancel(this, EventArgs.Empty);
			}
		}
		private void finish_Click(object sender, RoutedEventArgs e) {
			if(Finish != null) {
				Finish(this, EventArgs.Empty);
			}
		}
		#region IWizardView Members
		public event EventHandler Cancel;
		public event EventHandler Next;
		public event EventHandler Previous;
		public event EventHandler Finish;
		public void EnableNext(bool enable) {
			nextButton.IsEnabled = enable;
		}
		public void EnablePrevious(bool enable) {
			previousButton.IsEnabled = enable;
		}
		public void EnableFinish(bool enable) {
			finishButton.IsEnabled = enable;
		}
		public void SetPageContent(object content) {
			pageContent.Content = content;
		}
		public void ShowError(string error) {
			MessageBox.Show(error);
		}
		#endregion
	}
}
