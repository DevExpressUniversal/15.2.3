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

using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Data;
using DevExpress.XtraEditors.Filtering;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
namespace DevExpress.DashboardWin {
	public class DashboardParameters : ReadOnlyCollection<DashboardParameterDescriptor> {
		internal DashboardParameters(IList<DashboardParameterDescriptor> list) : base(list) {
			foreach(DashboardParameterDescriptor parameter in this) {
				parameter.ParameterChanged += (s, e) => { RaiseCollectionChanged(); };
			}
		}
		public DashboardParameterDescriptor this[string name] {
			get {
				return this.Where(p => p.Name == name).First();
			}
		}
		internal void RaiseCollectionChanged() {
			if(CollectionChanged != null) {
				CollectionChanged(null, null);
			}
		}
		internal event EventHandler CollectionChanged;
	}
	public class DashboardParameterDescriptor : IParameter {
		static Type ParameterTypeToType(ParameterType parameterType) {
			switch(parameterType) {
			case ParameterType.Int:
				return typeof(int);
			case ParameterType.String:
				return typeof(string);
			case ParameterType.Float:
				return typeof(double);
			case ParameterType.DateTime:
				return typeof(DateTime);
			case ParameterType.Bool:
				return typeof(Boolean);
			default:
				return typeof(object);
			}
		}
			readonly DashboardParameterViewModel parameterViewModel;
		object value;
		public string Name {
			get { return parameterViewModel.Name; }
		}
		public object Value {
			get { return value; }
			set { 
				this.value = value;
				ParameterChanged(null, null);
			}
		}
		Type IFilterParameter.Type {
			get { return ParameterTypeToType(parameterViewModel.Type); }
		}
		public object DefaultValue {
			get { return parameterViewModel.DefaultValue; }
		}
		public string Description {
			get { return parameterViewModel.Description; }
		}
		public ParameterValueType Type {
			get { return PrepareType(parameterViewModel.Type); }
		}
		public IList<ParameterValue> Values {
			get { return PrepareParameterValues(parameterViewModel.Values); }
		}
		internal event EventHandler ParameterChanged;
		internal DashboardParameterDescriptor(DashboardParameterViewModel parameterViewModel) {
			this.parameterViewModel = parameterViewModel;
			this.value = parameterViewModel.DefaultValue;
		}
		internal DashboardParameter ToDashboardParameter() {
			return new DashboardParameter(Name, ((IFilterParameter)this).Type, value);
		}
		ParameterValueType PrepareType(ParameterType type) {
			switch(type) {
				case ParameterType.Bool:
					return ParameterValueType.Boolean;
				case ParameterType.DateTime:
					return ParameterValueType.DateTime;
				case ParameterType.Float:
					return ParameterValueType.Float;
				case ParameterType.Int:
					return ParameterValueType.Integer;
				default:
					return ParameterValueType.String;
			}
		}
		IList<ParameterValue> PrepareParameterValues(IList<ParameterValueViewModel> values) {
			List<ParameterValue> parameterValues = new List<ParameterValue>();
			if(values != null) {
				foreach(ParameterValueViewModel value in values) {
					parameterValues.Add(new ParameterValue { DisplayText = value.DisplayText, Value = value.Value });
				}
			}
			return parameterValues;
		}
	}
	public enum ParameterValueType {
		String = 0,
		Integer = 1,
		Float = 2,
		Boolean = 3,
		DateTime = 4,
	}
	public class ParameterValue {
		public string DisplayText { get; internal set; }
		public object Value { get; internal set; }
	}
}
