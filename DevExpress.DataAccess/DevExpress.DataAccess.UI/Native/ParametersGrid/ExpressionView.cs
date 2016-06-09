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
using System.Windows.Forms;
using DevExpress.DataAccess.UI.Localization;
using DevExpress.DataAccess.UI.Native.Sql;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
namespace DevExpress.DataAccess.UI.Native.ParametersGrid {
	public class ExpressionView : QueryParameterExpressionEditorForm, IExpressionView {
		readonly IWin32Window owner;
		public ExpressionView(object contextInstance, IWin32Window owner, IServiceProvider serviceProvider, UserLookAndFeel lookAndFeel)
			: base(contextInstance, serviceProvider, lookAndFeel) {
			this.owner = owner;
			this.FormClosing += (sender, args) => {
				if(DialogResult == DialogResult.Cancel)
					RaiseCancel();
				else if(DialogResult == DialogResult.OK)
					RaiseOk();
			};
		}
		#region Implementation of IView<in IExpressionPresenter>
		public void BeginUpdate() {
			SuspendLayout();
		}
		public void EndUpdate() {
			ResumeLayout();
		}
		public void RegisterPresenter(IExpressionPresenter presenter) { }
		public void Start() {
			ShowDialog(owner);
		}
		public void Stop() {
			Close();
		}
		public void Warning(string message) {
			XtraMessageBox.Show(LookAndFeel, this, message,
				DataAccessUILocalizer.GetString(DataAccessUIStringId.MessageBoxWarningTitle), MessageBoxButtons.OK,
				MessageBoxIcon.Warning);
		}
		public event EventHandler Ok;
		public event EventHandler Cancel;
		#endregion
		#region Implementation of IExpressionView
		public string ExpressionString {
			get { return this.Expression; }
		}
		#endregion
		void RaiseOk() {
			if(Ok != null)
				Ok(this, EventArgs.Empty);
		}
		void RaiseCancel() {
			if(Cancel != null)
				Cancel(this, EventArgs.Empty);
		}
	}
}
