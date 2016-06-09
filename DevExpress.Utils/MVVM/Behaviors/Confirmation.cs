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

namespace DevExpress.Utils.MVVM {
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;
	using DevExpress.Utils.MVVM.Services;
	public enum ConfirmationButtons {
		OKCancel = 1,
		YesNoCancel = 3,
		YesNo = 4,
	}
	public class ConfirmationBehavior<TEventArgs> : EventTriggerBase<TEventArgs>
		where TEventArgs : CancelEventArgs {
		public ConfirmationBehavior(string eventName)
			: base(eventName) {
			Text = "Please confirm your action.";
			Caption = "Need confirmation";
			Buttons = ConfirmationButtons.YesNo;
			ShowQuestionIcon = true;
		}
		public string Text { get; set; }
		public string Caption { get; set; }
		public ConfirmationButtons Buttons { get; set; }
		public bool ShowQuestionIcon { get; set; }
		protected virtual string GetConfirmationText() {
			return Text;
		}
		protected virtual string GetConfirmationCaption() {
			return Caption;
		}
		static IMessageBoxProxy proxy;
		protected virtual bool Confirm() {
			if(proxy == null) {
				Type messageBoxType = typeof(MessageBox);
				var editorsAssembly = AssemblyHelper.GetLoadedAssembly(AssemblyInfo.SRAssemblyEditors)
					?? AssemblyHelper.LoadDXAssembly(AssemblyInfo.SRAssemblyEditors);
				if(editorsAssembly != null)
					messageBoxType = editorsAssembly.GetType("DevExpress.XtraEditors.XtraMessageBox");
				proxy = new MessageBoxProxy(messageBoxType);
			}
			var buttons = Buttons.ToMessageButton();
			var icon = (ShowQuestionIcon ? MessageBoxIcon.Question : MessageBoxIcon.None).ToMessageIcon();
			var positiveResult = (Buttons == ConfirmationButtons.OKCancel ? DialogResult.OK : DialogResult.Yes).ToMessageResult();
			return proxy.Show(GetConfirmationText(), GetConfirmationCaption(), buttons, icon, 0) == positiveResult;
		}
		protected sealed override void OnEvent() {
			Args.Cancel = !Confirm();
		}
	}
}
