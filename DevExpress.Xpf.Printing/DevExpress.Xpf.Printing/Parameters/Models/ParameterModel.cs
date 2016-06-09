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
using System.Linq;
using System.ComponentModel;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using DevExpress.XtraReports.Parameters;
using System.Linq.Expressions;
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.Printing.Parameters.Models {
	[POCOViewModel]
	public class ParameterModel : IDataErrorInfo, INotifyPropertyChanged {
		readonly Parameter parameter;
		string error;
		public event EventHandler ValueChanged;
		internal Parameter Parameter {
			get {
				return parameter;
			}
		}
		public string Path { get; set; }
		public string Name {
			get {
				return parameter.Name;
			}
		}
		public string Description {
			get {
				return parameter.Description;
			}
		}
		public bool MultiValue {
			get { return parameter.MultiValue; }
		}
		public Type Type {
			get { return parameter.Type; }
		}
		public virtual LookUpValueCollection LookUpValues { get; set; }
		public virtual object Value { get; set; }
		public bool Visible {
			get { return parameter.Visible; }
		}
		internal bool IsChanged { get; set; }
		public bool IsFilteredLookUpSettings { get; internal set; }
		#region ctor
		public static ParameterModel CreateParameterModel(Parameter parameter, LookUpValueCollection lookUpValues) {
			return ViewModelSource.Create(() => new ParameterModel(parameter, lookUpValues));
		}
		protected ParameterModel(Parameter parameter, LookUpValueCollection lookUpValues) {
			this.parameter = parameter;
			Value = parameter.Value;
			LookUpValues = lookUpValues;
			IsChanged = false;
		}
		#endregion
		internal void UpdateLookUpValues(LookUpValueCollection values) {
			if(values == null)
				return;
			Value = values.Any(x => x.Value.Equals(Value))
				? Value
				: values.Count > 0
				? values[0].Value
				: null;
			LookUpValues = values;
		}
		internal void UpdateParameter(UpdateAction action) {
			if(action == UpdateAction.Submit)
				parameter.Value = Value;
			else {
				Value = LookUpValues == null || LookUpValues.Any(x => x.Value.Equals(parameter.Value))
					? parameter.Value
					: null;
			}
			IsChanged = false;
		}
		protected void OnValueChanged() {
			IsChanged = Value != parameter.Value;
			if(ValueChanged != null)
				ValueChanged(this, EventArgs.Empty);
		}
		#region IDataErrorInfo
		string IDataErrorInfo.Error {
			get { return string.Empty; }
		}
		string IDataErrorInfo.this[string columnName] {
			get { return error; }
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetError(string error) {
			this.error = error;
			RaisePropertyChanged("");
		}
		#endregion
		#region INotifyPropertyChanged
		[EditorBrowsable(EditorBrowsableState.Never)]
		public event PropertyChangedEventHandler PropertyChanged;
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected void RaisePropertyChanged(string property) {
			if(PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(property));
		}
		#endregion
	}
	public enum UpdateAction {
		Submit,
		Reset
	}
}
