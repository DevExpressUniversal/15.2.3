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
using System.Text;
using DevExpress.Data;
using DevExpress.DataAccess;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using DevExpress.Xpf.DataAccess.Native;
namespace DevExpress.Xpf.DataAccess.DataSourceWizard {
	[POCOViewModel]
	public class ParameterViewModel : IParameter, IDataErrorInfo {
		public static ParameterViewModel Create(IParameter parameter) {
			return ViewModelSource.Create(() => new ParameterViewModel(parameter));
		}
		public virtual string Name { get; set; }
		public virtual Type Type { get; set; }
		public virtual string SelectedReportParameterName { get; set; }
		public virtual object Value { get; set; }
		protected void OnValueChanged() {
			if(Value is DBNull)
				Value = ParametersHelper.GetDefaultValue(Type);
			else if(IsExpression)
				Value = new Expression(this.Value == null ? string.Empty : this.Value.ToString(), Type);
		}
		protected ParameterViewModel(IParameter parameter) {
			this.Name = parameter.Name;
			this.Type = parameter.Type;
			this.Value = parameter.Value;
		}
		public virtual bool IsExpression { get; set; }
		protected void OnIsExpressionChanged() {
			this.Value = IsExpression ? new Expression(this.Value == null ? string.Empty : this.Value.ToString(), Type) : GetValue();
		}
		protected object GetValue() {
			try {
				return Value == null ? null : Convert.ChangeType(Value, Type);
			} catch(InvalidCastException) {
				return ParametersHelper.GetDefaultValue(Type);
			}
		}
		#region IDataErrorInfo implementation
		string IDataErrorInfo.Error { get { return null; } }
		string IDataErrorInfo.this[string columnName] {
			get {
				if(Type == null)
					return string.Empty;
				if(Value != null && columnName == BindableBase.GetPropertyName(() => Value) && !(Value is Expression)) {
					try {
						var oldValue = Value;
						Value = Convert.ChangeType(oldValue, Type);
					} catch(InvalidCastException) {
						Value = ParametersHelper.GetDefaultValue(Type);
						return string.Format("Can't convert '{0}' to {1}", Value, Type.Name);
					} catch(Exception) {
						Value = ParametersHelper.GetDefaultValue(Type);
						return string.Format("Can't convert '{0}' to {1}", Value, Type.Name);
					}
				}
				return string.Empty;
			}
		}
		#endregion
	}
}
