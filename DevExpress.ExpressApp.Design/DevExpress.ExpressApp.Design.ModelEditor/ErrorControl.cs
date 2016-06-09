#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.ExpressApp.Design.ModelEditor {
	public partial class ErrorControl : UserControl {
		private Icon icon;
		private void SetHeight(TextBox control) {
			Graphics g = control.CreateGraphics();
			SizeF size = g.MeasureString(control.Text, control.Font, control.Width);
			control.Height = (int)size.Height + 1;
		}
		public ErrorControl() {
			InitializeComponent();
			icon = SystemIcons.Error;
			panel1.Width = icon.Width + 10;
			ErrorMessage.Text = @"The unrecoverable error!";
			CallStack.Text = @"The unrecoverable stack!";
		}
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			SetHeight(CallStack);
			SetHeight(ErrorMessage);
			Height = CallStack.Height + ErrorMessage.Height + textBox1.Height + textBox2.Height + 20;
		}
		private void panel1_Paint(object sender, PaintEventArgs e) {
			e.Graphics.DrawIcon(icon, 0, 0);
		}
		public void SetErrorMessage(Exception e) {
			ErrorMessage.Text = e.Message;
			CallStack.Text = e.StackTrace;
		}
	}
}
