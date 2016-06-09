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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using DevExpress.DataAccess.Native.Sql.QueryBuilder;
using DevExpress.DataAccess.Sql;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Xpo.DB;
using ConditionType = DevExpress.DataAccess.Sql.RelationColumnInfo.ConditionType;
using RelationColumnInfoList = DevExpress.DataAccess.Sql.RelationInfo.RelationColumnInfoList;
namespace DevExpress.Xpf.DataAccess.DataSourceWizard {
	public class JoinEditorView : IJoinEditorView {
		public JoinEditorView(Action<Action<IDialogService>> doWithJoinEditorDialogService) {
			this.doWithJoinEditorDialogService = doWithJoinEditorDialogService;
			this.items = new ObservableCollection<JoinEditorItem>();
		}
		bool isCancel;
		readonly Action<Action<IDialogService>> doWithJoinEditorDialogService;
		public Action<Action<IMessageBoxService>> DoWithMessageBoxService;
		public event EventHandler Cancel;
		public event EventHandler Ok;
		public JoinType JoinType { get; set; }
		readonly ObservableCollection<JoinEditorItem> items;
		public ObservableCollection<JoinEditorItem> Items { get { return items; } }
		string leftTable;
		public string LeftTable { get { return leftTable; } }
		IEnumerable<string> leftColumns;
		public IEnumerable<string> LeftColumns { get { return leftColumns; } }
		IEnumerable<KeyValuePair<string, IEnumerable<string>>> right;
		public IEnumerable<KeyValuePair<string, IEnumerable<string>>> Right { get { return right; } }
		public void InitLookups(string leftTable, IEnumerable<string> leftColumns, IEnumerable<KeyValuePair<string, IEnumerable<string>>> right) {
			this.leftTable = leftTable;
			this.leftColumns = leftColumns;
			this.right = right;
		}
		public void SetColumns(IDictionary<string, RelationColumnInfoList> value) {
			if(!value.Any())
				items.Add(new JoinEditorItem(this) { LeftTable = leftTable });
			else {
				foreach(KeyValuePair<string, RelationInfo.RelationColumnInfoList> pair in value) {
					string rightTable = pair.Key;
					RelationInfo.RelationColumnInfoList columns = pair.Value;
					foreach(RelationColumnInfo column in columns)
						items.Add(new JoinEditorItem(this) { LeftTable = leftTable, LeftColumn = column.NestedKeyColumn, OperatorType = column.ConditionOperator, RightTable = rightTable, RightColumn = column.ParentKeyColumn });
				}
			}
			items.Add(new JoinEditorItem(this) { IsDeletable = true, LeftTable = leftTable });
		}
		public IDictionary<string, RelationColumnInfoList> GetColumns() {
			var result = new Dictionary<string, RelationColumnInfoList>();
			var rows = items.ToList();
			rows.Remove(rows.Last());
			foreach(var row in rows) {
				string rightTable = row.RightTable;
				if(rightTable == null) return null;
				RelationColumnInfoList columns;
				if(!result.TryGetValue(rightTable, out columns)) {
					columns = new RelationColumnInfoList();
					result.Add(rightTable, columns);
				}
				columns.Add(new RelationColumnInfo(row.RightColumn, row.LeftColumn, (ConditionType)row.OperatorType));
			}
			return result;
		}
		public void Start() {
			doWithJoinEditorDialogService(dialog => {
				var commands = UICommand.GenerateFromMessageButton(MessageButton.OKCancel, new DefaultMessageButtonLocalizer());
				var okCommand = commands.First(x => Equals(x.Id, MessageResult.OK));
				okCommand.Command = new DelegateCommand<CancelEventArgs>(e => {
					Ok.Do(x => Ok(this, EventArgs.Empty));
					e.Cancel = isCancel;
					isCancel = false;
				}, false);
				if(dialog.ShowDialog(commands, "Join Editor", this) != okCommand)
					Cancel.Do(x => Cancel(this, EventArgs.Empty));
			});
		}
		public void Warning(string message) {
			isCancel = true;
			DoWithMessageBoxService(dialog => {
				dialog.ShowMessage(message, "Warning", MessageButton.OK, MessageIcon.Warning);
			});
		}
		public void BeginUpdate() { }
		public void EndUpdate() { }
		public void RegisterPresenter(IJoinEditorPresenter presenter) { }
		public void Stop() { }
	}
}
