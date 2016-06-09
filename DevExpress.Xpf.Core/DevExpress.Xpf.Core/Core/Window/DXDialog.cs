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
using DevExpress.Xpf.Core.Native;
using System.Windows.Controls;
using System.Windows.Media;
namespace DevExpress.Xpf.Core {
	public enum DialogButtons { Ok, OkCancel, YesNoCancel, YesNo }
	[TemplatePart(Name = FooterName, Type = typeof(Panel))]
	[TemplatePart(Name = OkButtonName, Type = typeof(Button))]
	[TemplatePart(Name = CancelButtonName, Type = typeof(Button))]
	[TemplatePart(Name = YesButtonName, Type = typeof(Button))]
	[TemplatePart(Name = NoButtonName, Type = typeof(Button))]
	public class DXDialog : DXWindow {
		#region Constants
		const string FooterName = "Footer";
		const string OkButtonName = "OkButton";
		internal const string CancelButtonName = "CancelButton";
		const string YesButtonName = "YesButton";
		const string NoButtonName = "NoButton";
		#endregion
		static DialogButtons GetDialogButtons(MessageBoxButton buttons) {
			switch(buttons) {
				case MessageBoxButton.OK:
					return DialogButtons.Ok;
				case MessageBoxButton.OKCancel:
					return DialogButtons.OkCancel;
				case MessageBoxButton.YesNo:
					return DialogButtons.YesNo;
				case MessageBoxButton.YesNoCancel:
					return DialogButtons.YesNoCancel;
				default:
					return default(DialogButtons);
			}
		}
		bool setButtonHandlers;
		public DialogButtons Buttons { get; set; }
		public Button OkButton { get; private set; }
		public Button CancelButton { get; private set; }
		public Button YesButton { get; private set; }
		public Button NoButton { get; private set; }
		protected Panel Footer { get; private set; }
		MessageBoxResult messageBoxResult;
		static DXDialog() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(DXDialog), new FrameworkPropertyMetadata(typeof(DXDialog)));	
		}
		public DXDialog() : this(string.Empty) { }
		public DXDialog(string title) : this(title, DialogButtons.OkCancel) { }
		public DXDialog(string title, DialogButtons dialogButtons) : this(title, dialogButtons, true) { }
		public DXDialog(string title, DialogButtons dialogButtons, bool setButtonHandlers) {
			this.Title = title;
			this.Buttons = dialogButtons;
			this.setButtonHandlers = setButtonHandlers;
		}
		[
		Browsable(false), 
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public new void Show() {
			throw new NotSupportedException();
		}
		public MessageBoxResult ShowDialog(MessageBoxButton button) {
			Buttons = GetDialogButtons(button);
			messageBoxResult = MessageBoxResult.None;
			ShowDialog();
			return messageBoxResult;
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			GetTemplateChildren();
			if(setButtonHandlers)
				SetButtonHandlers();
			ApplyDialogButtonProperty();
		}
		protected virtual void GetTemplateChildren() {			
			Footer = GetTemplateChild(OkButtonName) as Panel;
			OkButton = GetTemplateChild(OkButtonName) as Button;
			CancelButton = GetTemplateChild(CancelButtonName) as Button;
			YesButton = GetTemplateChild(YesButtonName) as Button;
			NoButton = GetTemplateChild(NoButtonName) as Button; 
		}
		protected virtual void SetButtonHandlers() {
			SetButtonHandler(OkButton, true, MessageBoxResult.OK);
			SetButtonHandler(CancelButton, false, MessageBoxResult.Cancel);
			SetButtonHandler(YesButton, true, MessageBoxResult.Yes);
			SetButtonHandler(NoButton, false, MessageBoxResult.No);
		}
		protected void SetButtonHandler(Button button, bool? result, MessageBoxResult messageBoxResult) {
			if(button != null)
				button.Click += (d, e) => { OnButtonClick(result, messageBoxResult); };
		}
		protected virtual void OnButtonClick(bool? result, MessageBoxResult messageBoxResult) {
			DialogResult = result;
			this.messageBoxResult = messageBoxResult;
			OnClosing(new CancelEventArgs());
		}
		protected virtual void ApplyDialogButtonProperty() {
			switch(Buttons) {
				case DialogButtons.Ok:
					SetButtonVisibilities(true, false, false, false, false);
					break;
				case DialogButtons.OkCancel:
					SetButtonVisibilities(true, true, false, false, false);
					break;
				case DialogButtons.YesNo:
					SetButtonVisibilities(false, false, true, true, false);
					break;
				case DialogButtons.YesNoCancel:
					SetButtonVisibilities(false, true, true, true, false);
					break;
			};
		}
		protected virtual void SetButtonVisibilities(bool ok, bool cancel, bool yes, bool no, bool apply) {
			SetButtonVisibility(OkButton, ok);
			SetButtonVisibility(CancelButton, cancel);
			SetButtonVisibility(YesButton, yes);
			SetButtonVisibility(NoButton, no);
		}
		protected void SetButtonVisibility(Button button, bool visible) {
			if(button != null)
				button.SetVisible(visible);
		}
	}
}
