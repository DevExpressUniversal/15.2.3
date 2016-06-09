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
using System.Xml.Linq;
using DevExpress.Data.Filtering;
using DevExpress.DataAccess.Localization;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.Utils;
using DevExpress.Xpo.DB;
namespace DevExpress.DataAccess.Native.Data {	
	public class DataReferences : List<DataReference> {
		internal const string XmlReferences = "References";
		internal const string XmlJoinType = "JoinType";
		internal const string XmlActionType = "ActionType";
		internal const ActionType DefaultActionType = ActionType.InnerJoin;
		static QueryOperand CreateQueryOperand(DBTable table, string columnName, string nodeAlias) {
			DBColumn column = table.Columns.Find(_column => _column.Name == columnName);
			if(column == null)
				throw new Exception(String.Format("'{0}' column doesn't exist", columnName));
			return new QueryOperand(columnName, nodeAlias, column.ColumnType);
		}
		readonly DataSelection dataSelection;
		ActionType actionType = DefaultActionType;
		string alias;
		string keyDBTableName;
		DBTable keyDBTable;
		public string Alias { get { return this.alias; } set { this.alias = value; } }
		public ActionType ActionType { get { return this.actionType; } set { SetActionType(value, true); } }
		JoinType JoinType { get { return (JoinType)ActionType; } }
		public DataSelection DataSelection { get { return this.dataSelection; } }
		public DBTable KeyDBTable
		{
			get
			{
				if(this.keyDBTable == null)
					this.keyDBTable = this.dataSelection.DataProvider.FindTable(this.keyDBTableName);
				return this.keyDBTable;
			}
			private set
			{
				this.keyDBTable = value;
				this.keyDBTableName = value.Name;
			}
		}
		public DataReferences(DataSelection dataSelection, DBTable keyTable, string alias) {
			Guard.ArgumentNotNull(dataSelection, "dataSelection");
			this.dataSelection = dataSelection;
			KeyDBTable = keyTable;
			Alias = alias;
		}
		public DataReferences(DataSelection dataSelection, string tableName, string alias) {
			Guard.ArgumentNotNull(dataSelection, "dataSelection");
			this.dataSelection = dataSelection;
			this.keyDBTableName = tableName;
			this.alias = alias;
		}
		public DataReferences(DataSelection dataSelection, string tableName, string alias, XElement element) :
			this(dataSelection, tableName, alias) {
			string joinTypeString = XmlHelperBase.GetAttributeValue(element, XmlJoinType);
			if(joinTypeString != null)
				this.actionType = (ActionType)XmlHelperBase.EnumFromString<JoinType>(joinTypeString);
			else {
				string actionTypeString = XmlHelperBase.GetAttributeValue(element, XmlActionType);
				if(actionTypeString != null)
					this.actionType = XmlHelperBase.EnumFromString<ActionType>(actionTypeString);
			}
			foreach(XElement referenceElement in element.Elements(DataReference.XmlReference)) {
				Add(new DataReference(dataSelection, tableName, referenceElement));
			}
		}
		public void PrepareSelect(SelectStatement selectStatement) {
			if(Count > 0 && ActionType != ActionType.MasterDetailRelation) {
				string itemAlias = this.alias;
				JoinNode joinNode = new JoinNode(KeyDBTable, itemAlias, JoinType);
				List<CriteriaOperator> binaryOperators = new List<CriteriaOperator>();
				foreach(DataReference reference in this) {
					QueryOperand operand = CreateQueryOperand(KeyDBTable, reference.KeyColumn, itemAlias);
					QueryOperand parentOperand = CreateQueryOperand(reference.ParentDataTable.Table, reference.ParentKeyColumn, reference.ParentDataTable.Alias);
					binaryOperators.Add(new BinaryOperator(operand, parentOperand, reference.OperatorType));
				}
				joinNode.Condition = GroupOperator.And(binaryOperators);
				selectStatement.SubNodes.Add(joinNode);
			}
		}
		public XElement SaveToXml() {
			XElement element = new XElement(XmlReferences);
			if(this.actionType != DefaultActionType)
				element.Add(new XAttribute(XmlActionType, this.actionType));
			foreach(DataReference reference in this)
				element.Add(reference.SaveToXml());
			return element;
		}
		public void LoadDataObjects() {
			if(this.keyDBTable == null)
				this.keyDBTable = this.dataSelection.DataProvider.FindTable(this.keyDBTableName);
			foreach(DataReference reference in this)
				reference.LoadDataObjects();
		}
		public DataReferences Clone() {
			DataReferences newReferences = new DataReferences(this.dataSelection, KeyDBTable, Alias);
			newReferences.Capacity = Count;
			newReferences.AddRange(this.Select(reference => reference.Clone()));
			newReferences.SetActionType(ActionType, false);
			return newReferences;
		}
		public void SetActionType(ActionType actionType, bool raiseChanged) {
			if(this.actionType != actionType) {
				this.actionType = actionType;
				if(raiseChanged)
					this.dataSelection.OnTableChanged();
			}
		}
		public override string ToString() {
			if(Count > 0) {
				StringBuilder builder = new StringBuilder();
				builder.Append(this[0].ToString());
				for(int i = 1; i < this.Count; i++) {
					builder.Append(DataAccessLocalizer.GetString(DataAccessStringId.QueryDesignerJoinExpressionElementSeparator));
					builder.Append(this[i].ToString());
				}
				return string.Format(DataAccessLocalizer.GetString(DataAccessStringId.QueryDesignerJoinExpressionPattern), new ActionTypeData(ActionType).ToString(), builder.ToString());
			}
			return string.Empty;
		}
	}
}
