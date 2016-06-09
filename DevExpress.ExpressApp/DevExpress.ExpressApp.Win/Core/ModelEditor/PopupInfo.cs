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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.ExpressApp.Win.Templates;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.Win.Core.ModelEditor {
	public static class PopupInfo {
		public static Form CreateForm(string content, string caption, Image iconImage) {
			LargeStringEdit editor = new LargeStringEdit(0, 0);
			editor.Properties.ReadOnly = true;
			editor.Text = content;
			return CreateForm(editor, caption, iconImage);
		}
		public static Form CreateForm(string message, string content, string caption, Image iconImage) {
			Panel panelContainer = new Panel();
			panelContainer.Width = 500;
			int paddingSize = 10;
			LabelControl label = new LabelControl();
			label.Top = paddingSize;
			label.Left = paddingSize;
			label.Width = panelContainer.Width - 2 * paddingSize;
			label.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
			label.AutoSizeMode = LabelAutoSizeMode.Vertical;
			label.AutoSize = true;
			label.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Default;
			label.Text = message;
			panelContainer.Controls.Add(label);
			LargeStringEdit editor = new LargeStringEdit(0, 0);
			editor.Properties.ReadOnly = true;
			editor.Text = content;
			panelContainer.Controls.Add(editor);
			editor.Top = label.Bottom + paddingSize;
			editor.Width = panelContainer.Width;
			editor.Height = panelContainer.Height - editor.Top;
			editor.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
			label.Resize += delegate(object sender, EventArgs e) {
				editor.Top = label.Bottom + paddingSize;
				editor.Height = panelContainer.Height - editor.Top;
			};
			return CreateForm(panelContainer, caption, iconImage);
		}
		public static Form CreateForm(Control control, string caption, Image iconImage) {
			Icon icon = iconImage == null ? null : Icon.FromHandle(new Bitmap(iconImage).GetHicon());
			PopupForm form = new PopupForm();
			form.AutoShrink = false;
			control.Dock = DockStyle.Fill;
			form.AddControl(control, caption);
			SimpleAction cancelAction = new SimpleAction();
			cancelAction.ActionMeaning = ActionMeaning.Cancel;
			cancelAction.Caption = "Close";
			form.ButtonsContainer.Register(cancelAction);
			if(icon != null) {
				form.Icon = icon;
			}
			LookAndFeelUtils.ApplyStyle(form);
			return form;
		}
		public static void Show(Control control, string caption, Image iconImage, bool isDialog) {
			using(Form form = CreateForm(control, caption, iconImage)) {
				if(isDialog) {
					form.ShowDialog();
				}
				else {
					form.Show();
				}
			}
		}
		public static void Show(string content, string caption, Image iconImage, bool isDialog) {
			using(Form form = CreateForm(content, caption, iconImage)) {
				if(isDialog) {
					form.ShowDialog();
				}
				else {
					form.Show();
				}
			}
		}
		public static void Show(string message, string content, string caption, Image iconImage, bool isDialog) {
			using(Form form = CreateForm(message, content, caption, iconImage)) {
				if(isDialog) {
					form.ShowDialog();
				}
				else {
					form.Show();
				}
			}
		}
	}
}
