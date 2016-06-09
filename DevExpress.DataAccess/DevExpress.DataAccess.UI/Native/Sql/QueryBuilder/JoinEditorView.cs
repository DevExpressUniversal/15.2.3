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
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.Native.Sql.QueryBuilder;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.UI.Localization;
using DevExpress.LookAndFeel;
using DevExpress.Xpo.DB;
using DevExpress.XtraEditors;
namespace DevExpress.DataAccess.UI.Native.Sql.QueryBuilder {
	public partial class JoinEditorView : XtraForm, IJoinEditorView {
		readonly IWin32Window owner;
		readonly IDisplayNameProvider displayNameProvider;
		public JoinEditorView(IWin32Window owner, UserLookAndFeel lookAndFeel): this(owner, lookAndFeel, null) { }
		public JoinEditorView(IWin32Window owner, UserLookAndFeel lookAndFeel, IDisplayNameProvider displayNameProvider): this() {
			this.owner = owner;
			LookAndFeel.ParentLookAndFeel = lookAndFeel;
			this.displayNameProvider = displayNameProvider;
		}
		JoinEditorView() {
			InitializeComponent();
			LocalizeComponent();
			this.cmbJoinType.Properties.Items.AddRange(
				Enum.GetValues(typeof(JoinType))
					.Cast<ActionType>()
					.Select(t => new ActionTypeData(t))
					.Cast<object>()
					.ToArray());
		}
		#region Implementation of IView<in IJoinEditorPresenter>
		public void BeginUpdate() { SuspendLayout(); }
		public void EndUpdate() { ResumeLayout(); }
		public void RegisterPresenter(IJoinEditorPresenter presenter) { }
		public void Start() { ShowDialog(this.owner); }
		public void Stop() { Close(); }
		public void Warning(string message) {
			XtraMessageBox.Show(LookAndFeel, this, message,
				DataAccessUILocalizer.GetString(DataAccessUIStringId.MessageBoxWarningTitle), MessageBoxButtons.OK,
				MessageBoxIcon.Warning);
		}
		public event EventHandler Ok;
		public event EventHandler Cancel;
		#endregion
		#region Implementation of IJoinEditorView
		public JoinType JoinType {
			get { return (JoinType)((ActionTypeData)this.cmbJoinType.EditValue).ActionType; }
			set {
				this.cmbJoinType.SelectedItem =
					this.cmbJoinType.Properties.Items.Cast<ActionTypeData>()
						.First(item => item.ActionType == (ActionType)value);
			}
		}
		public void InitLookups(string leftTable, IEnumerable<string> leftColumns, IEnumerable<KeyValuePair<string, IEnumerable<string>>> right) {
			this.joinEditorControl.InitLookups(leftTable, leftColumns, right, displayNameProvider);
		}
		public IDictionary<string, RelationInfo.RelationColumnInfoList> GetColumns() {
			var result = new Dictionary<string, RelationInfo.RelationColumnInfoList>();
			foreach(ConditionControl control in this.joinEditorControl.Items) {
				string rightTable = control.RightTableName;
				if(rightTable == null)
					return null;
				RelationInfo.RelationColumnInfoList columns;
				if(!result.TryGetValue(rightTable, out columns)) {
					columns = new RelationInfo.RelationColumnInfoList();
					result.Add(rightTable, columns);
				}
				columns.Add(new RelationColumnInfo(control.RightColumnName, control.LeftColumnName, (RelationColumnInfo.ConditionType)control.OperatorType));
			}
			return result;
		}
		public void SetColumns(IDictionary<string, RelationInfo.RelationColumnInfoList> value) {
			this.joinEditorControl.ClearItems();
			bool any = false;
			foreach(KeyValuePair<string, RelationInfo.RelationColumnInfoList> pair in value) {
				any = true;
				string rightTable = pair.Key;
				RelationInfo.RelationColumnInfoList columns = pair.Value;
				foreach(RelationColumnInfo column in columns)
					this.joinEditorControl.AddCondition(column.NestedKeyColumn, (BinaryOperatorType)column.ConditionOperator, rightTable, column.ParentKeyColumn);
			}
			if(!any)
				this.joinEditorControl.AddCondition(null, BinaryOperatorType.Equal, null, null);
			this.joinEditorControl.AlignItems();
		}
		#endregion
		void LocalizeComponent() {
			btnCancel.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.Button_Cancel);
			btnOk.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.Button_OK);
			lciJoinType.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.JoinEditor_JoinType);
			Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.JoinEditor);
		}
		void RaiseOk() {
			if(Ok != null)
				Ok(this, EventArgs.Empty);
		}
		void RaiseCancel() {
			if(Cancel != null)
				Cancel(this, EventArgs.Empty);
		}
		private void btnOk_Click(object sender, EventArgs e) {
			RaiseOk();
		}
		private void btnCancel_Click(object sender, EventArgs e) {
			RaiseCancel();
		}
		private void JoinEditorView_FormClosing(object sender, FormClosingEventArgs e) {
			RaiseCancel();
		}
	}
}
