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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraWaitForm;
namespace DevExpress.XtraWaitForm {
	public partial class ManualLayoutDemoWaitForm : WaitForm {
		public ManualLayoutDemoWaitForm() {
			InitializeComponent();
			ProgressPanel.AutoHeight = true;
		}
		#region Caption & Description
		public override void SetCaption(string caption) {
			base.SetCaption(caption);
			ProgressPanel.Caption = caption;
		}
		public override void SetDescription(string description) {
			base.SetDescription(description);
			ProgressPanel.Description = description;
		}
		public override void ProcessCommand(Enum cmd, object arg) {
			base.ProcessCommand(cmd, arg);
			WaitFormCommand command = (WaitFormCommand)cmd;
			if(command == WaitFormCommand.SetWidth) {
				OnSetWidthCommand((int)arg);
			}
			if(command == WaitFormCommand.SetHeight) {
				OnSetHeightCommand((int)arg);
			}
		}
		protected virtual void OnSetWidthCommand(int arg) {
			Width = arg;
		}
		protected virtual void OnSetHeightCommand(int arg) {
			Height = arg;
		}
		#endregion
		#region Command Info
		public enum WaitFormCommand {
			SetWidth,
			SetHeight
		}
		#endregion
		public ProgressPanel ProgressPanel { get { return progressPanel1; } }
	}
}
