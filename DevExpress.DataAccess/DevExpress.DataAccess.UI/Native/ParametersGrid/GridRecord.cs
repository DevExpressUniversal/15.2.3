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
using System.Linq;
using DevExpress.Data;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.Wizard.Services;
namespace DevExpress.DataAccess.UI.Native.ParametersGrid {
	public class GridRecord : INotifyPropertyChanged {
		internal static GridRecord FromIParameter(IParameter parameter, IParameterService parameterService) {
			return parameter == null ? null : new GridRecord(parameter, parameterService);
		}
		readonly IParameterService parameterService;
		string name;
		Type type;
		object value;
		bool isExpression;
		public GridRecord(string name, IParameterService parameterService) {
			this.name = name;
			type = typeof(int);
			value = ValueHelper.GetDefaultValue(type);
			this.parameterService = parameterService;
		}
		public GridRecord(IParameter parameter, IParameterService parameterService) {
			ApplyParameterCore(parameter, true, true);
			this.parameterService = parameterService;
		}
		public string Name {
			get { return name; }
			set {
				if(value == name)
					return;
				name = value;
				OnPropertyChanged("Name");
			}
		}
		[TypeConverter("DevExpress.DataAccess.UI.Native.Sql.QueryParameterTypeConverter," + AssemblyInfo.SRAssemblyDataAccessUI)]
		public Type Type {
			get { return type; }
			set {
				if(value == type || value == typeof(Expression))
					return;
				if(IsExpression)
					this.Expression.ResultType = value;
				else {
					type = value;
					object newValue;
					ValueHelper.ConvertValue(Value, type, out newValue);
					Value = newValue;
				}
				OnPropertyChanged("Type");
			}
		}
		public object Value {
			get { return value; }
			set {
				if(Equals(value, this.value))
					return;
				if(IsExpression) {
					if(value is Expression) {
						this.value = value;
					} else {
						Expression.ExpressionString = value.ToString();
					}
				} else {
					object newValue;
					if(ValueHelper.ConvertValue(value, Type, out newValue)) {
						this.value = newValue;
					}
				}
				OnPropertyChanged("Value");
				OnPropertyChanged("Expression");
			}
		}
		public bool IsExpression {
			get { return isExpression; }
			set {
				if(value == isExpression)
					return;
				if(value) {
					if(Value != null) {
						this.value = new Expression(Value.ToString(), Type);
					} else {
						this.value = new Expression(string.Empty, type);
					}
					type = typeof(Expression);
				} else {
					type = Expression.ResultType;
					this.value = Expression.EvaluateExpression(SourceParameters) ?? ValueHelper.GetDefaultValue(Expression.ResultType);
				}
				isExpression = value;
				OnPropertyChanged("IsExpression");
				OnPropertyChanged("Expression");
			}
		}
		public Expression Expression {
			get { return IsExpression ? Value as Expression : null; }
		}
		public event PropertyChangedEventHandler PropertyChanged;
		public void ApplyParameter(IParameter parameter, bool applyType) {
			ApplyParameterCore(parameter, false, applyType);
		}
		protected virtual void OnPropertyChanged(string propertyName) {
			if(PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		void ApplyParameterCore(IParameter parameter, bool applyName, bool applyType) {
			if(applyName) {
				this.name = parameter.Name;
			}
			if(applyType) {
				this.type = parameter.Type ?? typeof(int);
			}
			this.value = parameter.Value == null || parameter.Value == DBNull.Value ? ValueHelper.GetDefaultValue(parameter.Type) : parameter.Value;
			this.isExpression = parameter.Value is Expression;
		}
		IEnumerable<IParameter> SourceParameters {
			get { return parameterService != null ? parameterService.Parameters : Enumerable.Empty<IParameter>(); }
		}
	}
}
