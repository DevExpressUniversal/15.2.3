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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Xpo.DB;
using ConditionType = DevExpress.DataAccess.Sql.RelationColumnInfo.ConditionType;
namespace DevExpress.Xpf.DataAccess.DataSourceWizard {
	#region Inner classes
	public class JoinEditorMenuItem {
		public JoinEditorMenuItem(string name, JoinEditorItem joinEditorItem, object parameter) {
			this.name = name;
			this.joinEditorItem = joinEditorItem;
			this.parameter = parameter;
		}
		readonly string name;
		public string Name { get { return name; } }
		readonly JoinEditorItem joinEditorItem;
		public JoinEditorItem JoinEditorItem { get { return joinEditorItem; } }
		readonly object parameter;
		public object Parameter { get { return parameter; } }
	}
	public class JoinEditorOperatorMenuItem {
		public JoinEditorOperatorMenuItem(string name, JoinEditorMenuItem menuItem) {
			this.name = name;
			this.menuItem = menuItem;
		}
		readonly string name;
		public string Name { get { return name; } }
		readonly JoinEditorMenuItem menuItem;
		public JoinEditorMenuItem MenuItem { get { return menuItem; } }
	}
	#endregion
	public class JoinEditorItem : INotifyPropertyChanged {
		public JoinEditorItem(JoinEditorView joinEditorView) {
			this.joinEditorView = joinEditorView;
			this.leftColumnMenuItems = new ObservableCollection<JoinEditorMenuItem>();
			foreach(var column in joinEditorView.LeftColumns)
				leftColumnMenuItems.Add(new JoinEditorMenuItem(column, this, column));
			this.rightTableMenuItems = new ObservableCollection<JoinEditorMenuItem>();
			foreach(var column in joinEditorView.Right)
				rightTableMenuItems.Add(new JoinEditorMenuItem(column.Key, this, column.Key));
			this.rightColumnMenuItems = new ObservableCollection<JoinEditorMenuItem>();
			this.signOperatorMenuItems = new List<JoinEditorOperatorMenuItem>() {
				new JoinEditorOperatorMenuItem("Equal", new JoinEditorMenuItem("Equals to", this, ConditionType.Equal)),
				new JoinEditorOperatorMenuItem("NotEqual", new JoinEditorMenuItem("Does not equal to", this, ConditionType.NotEqual)),
				new JoinEditorOperatorMenuItem("Greater", new JoinEditorMenuItem("Is greater than", this, ConditionType.Greater)),
				new JoinEditorOperatorMenuItem("GreaterOrEqual", new JoinEditorMenuItem("Is greater than or equal to", this, ConditionType.GreaterOrEqual)),
				new JoinEditorOperatorMenuItem("Less", new JoinEditorMenuItem("Is less than", this, ConditionType.Less)),
				new JoinEditorOperatorMenuItem("LessOrEqual", new JoinEditorMenuItem("Is less than or equal to", this, ConditionType.LessOrEqual))
			}.AsReadOnly();
			InitializeCommands();
		}
		void InitializeCommands() {
			AddCommand = DelegateCommandFactory.Create(Add);
			RemoveCommand = DelegateCommandFactory.Create(Remove);
			SelectLeftColumnCommand = DelegateCommandFactory.Create<string>(SelectLeftColumn);
			SelectSignOperatorCommand = DelegateCommandFactory.Create<ConditionType>(SelectSignOperator);
			SelectRightTableCommand = DelegateCommandFactory.Create<string>(SelectRightTable);
			SelectRightColumnCommand = DelegateCommandFactory.Create<string>(SelectRightColumn);
		}
		public event PropertyChangedEventHandler PropertyChanged;
		readonly JoinEditorView joinEditorView;
		#region Properties
		public ICommand AddCommand { get; private set; }
		public ICommand RemoveCommand { get; private set; }
		public ICommand SelectLeftColumnCommand { get; private set; }
		public ICommand SelectSignOperatorCommand { get; private set; }
		public ICommand SelectRightTableCommand { get; private set; }
		public ICommand SelectRightColumnCommand { get; private set; }
		string leftTable;
		public string LeftTable {
			get { return leftTable; }
			set {
				leftTable = value;
				RaisePropertyChanged("LeftTable");
			}
		}
		string leftColumn;
		public string LeftColumn {
			get { return leftColumn; }
			set {
				leftColumn = value;
				RaisePropertyChanged("LeftColumn");
			}
		}
		ConditionType operatorType;
		public ConditionType OperatorType {
			get { return operatorType; }
			set {
				operatorType = value;
				RaisePropertyChanged("OperatorType");
			}
		}
		string rightTable;
		public string RightTable {
			get { return rightTable; }
			set {
				rightTable = value;
				rightColumnMenuItems.Clear();
				foreach(var column in joinEditorView.Right.First(x => x.Key == rightTable).Value)
					rightColumnMenuItems.Add(new JoinEditorMenuItem(column, this, column));
				RaisePropertyChanged("RightTable");
			}
		}
		string rightColumn;
		public string RightColumn {
			get { return rightColumn; }
			set {
				rightColumn = value;
				RaisePropertyChanged("RightColumn");
			}
		}
		bool isDeletable;
		public bool IsDeletable {
			get { return isDeletable; }
			set {
				isDeletable = value;
				RaisePropertyChanged("IsDeletable");
			}
		}
		readonly ObservableCollection<JoinEditorMenuItem> leftColumnMenuItems;
		public IEnumerable<JoinEditorMenuItem> LeftColumnMenuItems { get { return leftColumnMenuItems; } }
		readonly IEnumerable<JoinEditorOperatorMenuItem> signOperatorMenuItems;
		public IEnumerable<JoinEditorOperatorMenuItem> SignOperatorMenuItems { get { return signOperatorMenuItems; } }
		readonly ObservableCollection<JoinEditorMenuItem> rightTableMenuItems;
		public IEnumerable<JoinEditorMenuItem> RightTableMenuItems { get { return rightTableMenuItems; } }
		readonly ObservableCollection<JoinEditorMenuItem> rightColumnMenuItems;
		public IEnumerable<JoinEditorMenuItem> RightColumnMenuItems { get { return rightColumnMenuItems; } }
		#endregion
		#region Private Methods
		void SelectLeftColumn(string name) {
			LeftColumn = name;
		}
		void SelectSignOperator(ConditionType operatorType) {
			OperatorType = operatorType;
		}
		void SelectRightTable(string name) {
			RightTable = name;
		}
		void SelectRightColumn(string name) {
			RightColumn = name;
		}
		void Add() {
			IsDeletable = false;
			joinEditorView.Items.Add(new JoinEditorItem(joinEditorView) { IsDeletable = true, LeftTable = LeftTable });
		}
		void Remove() {
			joinEditorView.Items.Remove(this);
		}
		#endregion
		protected void RaisePropertyChanged(string propertyName = null) {
			if(PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
