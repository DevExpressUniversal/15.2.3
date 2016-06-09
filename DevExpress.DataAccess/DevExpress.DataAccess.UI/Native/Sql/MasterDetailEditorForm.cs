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
using System.Windows.Forms;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.UI.Localization;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
namespace DevExpress.DataAccess.UI.Native.Sql {
	public partial class MasterDetailEditorForm : OkCancelForm {
		public MasterDetailEditorForm(UserLookAndFeel lookAndFeel) : this() {
			LookAndFeel.ParentLookAndFeel = lookAndFeel;
		}
		MasterDetailEditorForm() {
			InitializeComponent();
			LocalizeComponent();
			this.StartPosition = FormStartPosition.CenterParent;
		}
		public SqlDataSource DataSource {
			get {
				return this.masterDetailEditorControl.DataSource;
			}
			set {
				this.masterDetailEditorControl.DataSource = value;
			}
		}
		public IEnumerable<MasterDetailInfo> Relations {
			get { return this.masterDetailEditorControl.Relations; }
		}
		protected override void OnClosing(CancelEventArgs e) {
			if(this.masterDetailEditorControl.RelationNameInplaceEditorIsShown) {
				this.masterDetailEditorControl.CloseInplaceEditor();
				e.Cancel = true;
				FormClosingEventArgs args = e as FormClosingEventArgs;
				if(args != null && args.CloseReason == CloseReason.None)
					return;
			}
			base.OnClosing(e);
		}
		void LocalizeComponent() {
			Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.MasterDetailEditorForm_Title);
		}
		void btnOK_Click(object sender, EventArgs e) {
			try {
				this.masterDetailEditorControl.CreateRelations();
			} catch(IncompleteConditionException ex) {
				ShowException(ex);
			} catch(InvalidConditionException ex) {
				ShowException(ex);
			} catch(RelationException) {
				DialogResult = DialogResult.None;
			}
		}
		void ShowException(Exception ex) {
			DialogResult = DialogResult.None;
			XtraMessageBox.Show(LookAndFeel, this, ex.Message, DataAccessUILocalizer.GetString(DataAccessUIStringId.MessageBoxErrorTitle), MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}
}
