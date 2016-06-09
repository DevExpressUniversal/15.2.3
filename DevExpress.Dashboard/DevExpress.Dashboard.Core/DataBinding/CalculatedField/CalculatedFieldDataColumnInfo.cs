#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Linq;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.PivotGrid.CriteriaVisitors;
namespace DevExpress.DashboardCommon.Native {
	public class CalculatedFieldDataColumnInfo : IDataColumnInfo {
		readonly CalculatedField field;
		readonly Func<IEnumerable<DashboardIDataColumnInfoWrapper>> columns;
		public CalculatedFieldDataColumnInfo(CalculatedField field, DataNode root, CalculatedFieldCollection calculatedFields, IEnumerable<IParameter> parameters) :
			this(field, root, calculatedFields, parameters, false) {
		}
		public CalculatedFieldDataColumnInfo(CalculatedField field, DataNode root, CalculatedFieldCollection calculatedFields, IEnumerable<IParameter> parameters, bool expandCaption) {
			this.field = field;
			columns = () => GetColumns(root, calculatedFields, parameters, expandCaption);
		}
		IEnumerable<DashboardIDataColumnInfoWrapper> GetColumns(DataNode root, CalculatedFieldCollection calculatedFields, IEnumerable<IParameter> parameters, bool showComplexCaption) {
			if(root != null)
				foreach(DataField dataField in root.GetAllDataFields())
					yield return new DashboardIDataColumnInfoWrapper(dataField.DataMember, GetFullCaption(dataField, showComplexCaption), dataField.SourceType, this);
			foreach(CalculatedField calculatedField in calculatedFields)
				yield return new DashboardIDataColumnInfoWrapper(calculatedField.Name, calculatedField.Name, calculatedField.DataType.ToType(), this);
			foreach(DashboardParameter parameter in parameters)
				yield return new DashboardIDataColumnInfoWrapper(string.Format(CalculatedFieldsController.ParameterFormatString, parameter.Name), parameter.Name, parameter.Type, this);
		}
		static string GetFullCaption(DataNode dataField, bool expandCaption) {
			if(!expandCaption)
				return dataField.DisplayName;
			int index = dataField.DataMember.IndexOf('.');
			if(index < 1)
				return dataField.DisplayName;
			string prev = dataField.DataMember.Substring(0, index);
			DataNode a = dataField.FindNodeDeep(prev);
			return a == null ? dataField.DisplayName : GetFullCaption(a, expandCaption) + '.' + dataField.DisplayName;
		}
		string IDataColumnInfo.Caption {
			get { return field.Name; }
		}
		List<IDataColumnInfo> IDataColumnInfo.Columns {
			get { return columns().Select<DashboardIDataColumnInfoWrapper, IDataColumnInfo>((c) => c).ToList(); }
		}
		DataControllerBase IDataColumnInfo.Controller {
			get { return null; }
		}
		string IDataColumnInfo.FieldName {
			get { return field.Name; }
		}
		System.Type IDataColumnInfo.FieldType {
			get { return field.DataType.ToType(); }
		}
		string IDataColumnInfo.Name {
			get { return field.Name; }
		}
		string IDataColumnInfo.UnboundExpression {
			get { 
				if(string.IsNullOrEmpty(field.Expression))
					return null;
				try {
					CriteriaOperator criteria = CriteriaOperator.Parse(field.Expression);
					if(ReferenceEquals(null, criteria))
						return null;
					return CriteriaOperator.ToString(criteria.Accept(new CustomFunctionReplacer(DashboardDistinctCountFunction.FunctionName, DashboardDistinctCountFunction.ExpressionEditorName)));
				} catch {
					return null;
				} 
			}
		}
	}
}
