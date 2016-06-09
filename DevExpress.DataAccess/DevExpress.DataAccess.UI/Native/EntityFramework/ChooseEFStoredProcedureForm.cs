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
using System.Windows.Forms;
using DevExpress.DataAccess.UI.Localization;
using DevExpress.DataAccess.Wizard.Views;
namespace DevExpress.DataAccess.UI.Native.EntityFramework {
	public partial class ChooseEFStoredProcedureForm : OkCancelForm {
		public IEnumerable<StoredProcedureViewInfo> SelectedStoredProcedures {
			get {
				return this.listBoxControlMain.SelectedItems.OfType<StoredProcedureViewInfo>();
			}
		}
		public ChooseEFStoredProcedureForm(IServiceProvider serviceProvider)
			: base(serviceProvider) {
			InitializeComponent();
			LocalizeComponent();
		}
		ChooseEFStoredProcedureForm()
			: this(null) {
		}
		public void Initialize(IEnumerable<StoredProcedureViewInfo> procedures) {
			this.listBoxControlMain.DataSource = procedures.ToList();
		}
		void LocalizeComponent() {
			Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.ChooseEFStoredProceduresDialog);
		}
		void listBoxControl1_MouseDoubleClick(object sender, MouseEventArgs e) {
			if(this.listBoxControlMain.SelectedItems.Count > 0)
				DialogResult = System.Windows.Forms.DialogResult.OK;
		}
	}
}
