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
using DevExpress.Data;
using DevExpress.DataAccess.UI.Localization;
using DevExpress.XtraEditors;
namespace DevExpress.DataAccess.UI.Native.ParametersGrid {
	public class PropertyGridView : PropertyGridForm, IPropertyGridView {
		readonly IWin32Window owner;
		public PropertyGridView(IWin32Window owner, IServiceProvider propertyGridServices)
			: base(propertyGridServices) {
			this.owner = owner;
			ServiceProvider = propertyGridServices;
			this.FormClosing += (sender, args) => RaiseCancel();
		}
		#region Overrides of OkCancelForm
		protected override void OnButtonOKClick() {
			base.OnButtonOKClick();
			RaiseOk();
		}
		#endregion
		#region Implementation of IView<in IPropertyGridPresenter>
		public void BeginUpdate() {
			SuspendLayout();
		}
		public void EndUpdate() {
			ResumeLayout();
		}
		public void RegisterPresenter(IPropertyGridPresenter presenter) { }
		public void Start() {
			this.ShowDialog(owner);
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
		#region Implementation of IPropertyGridView
		public IParameter Parameter {
			get { return SelectedObject as IParameter; }
			set { SelectedObject = value; }
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
