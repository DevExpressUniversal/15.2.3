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
using DevExpress.Data.Browsing;
using DevExpress.DocumentServices.ServiceModel.Client;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.Xpf.Printing.Parameters.Models.Native;
using DevExpress.XtraReports.Parameters;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.DocumentServices.ServiceModel;
using DevExpress.Mvvm.POCO;
using System.ComponentModel;
using System.Linq;
using DevExpress.Utils;
using DevExpress.XtraReports.Parameters.Native;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Printing.Parameters.Models {
	[POCOViewModel]
	public class ParametersModel : INotifyPropertyChanged {
		#region fields and properties
		ILookUpValuesProvider lookUpValuesProvider;
		event EventHandler<ValidateParameterEventArgs> validate;
		public virtual ReadOnlyCollection<ParameterModel> Parameters { get; protected set; }
		protected internal virtual bool IsChanged { get; protected set; }
		internal bool IsSubmitted { get; set; }
		public bool HasVisibleParameters {
			get {
				return Parameters.Any(x => x.Visible);
			}
		}
		public virtual bool CanEdit { get; set; }
		internal ILookUpValuesProvider LookUpValuesProvider {
			get { return lookUpValuesProvider; }
			set { lookUpValuesProvider = value; }
		}
		public event EventHandler<ValidateParameterEventArgs> Validate {
			add { 
				validate += value;
				ValidateParameters();
			}
			remove { validate -= value; }
		}
		public event EventHandler Submit;
		#endregion
		#region ctor
		public static ParametersModel CreateParametersModel() {
			return ViewModelSource.Create(() => new ParametersModel());
		}
		protected ParametersModel() {
			Parameters = new ReadOnlyCollection<ParameterModel>(new List<ParameterModel>());
			CanEdit = true;
		}
		#endregion
		#region methods
		public void AssignParameters(IList<ParameterModel> parameters) {
			IsSubmitted = false;
			if(this.IsInDesignMode())
				return;
			if(Parameters != null)
				UnsubscribeParameters();
			this.Parameters = new ReadOnlyCollection<ParameterModel>(parameters ?? Enumerable.Empty<ParameterModel>().ToList());
			RaisePropertyChanged("HasVisibleParameters");
			SubscribeParameters();
			ValidateParameters();
		}
		public void ResetParameters() {
			Parameters.ForEach(x => {
				x.UpdateParameter(UpdateAction.Reset);
			});
			IsChanged = CalculateIsChanged();
		}
		public bool CanResetParameters() {
			return IsChanged;
		}
		public void SubmitParameters() {
			Parameters.ForEach(x => x.UpdateParameter(UpdateAction.Submit));
			IsSubmitted = true;
			if(Submit != null)
				Submit(this, EventArgs.Empty);
			IsChanged = CalculateIsChanged();
		}
		public bool CanSubmitParameters() {
			return !Parameters
				.Cast<IDataErrorInfo>()
				.Any(x => !string.IsNullOrEmpty(x.Error) || !string.IsNullOrEmpty(x["Value"]));
		}
		public ParameterModel FindParameterByName(string name) {
			return Parameters.FirstOrDefault(x => x.Name == name);
		}
		void UnsubscribeParameters() {
			Parameters.ForEach(x => {
				x.PropertyChanged -= OnParameterPropertyChanged;
				x.ValueChanged -= OnParameterValueChanged;
			});
		}
		void SubscribeParameters() {
			Parameters.ForEach(x => {
				x.PropertyChanged += OnParameterPropertyChanged;
				x.ValueChanged += OnParameterValueChanged;
			});
		}
		void OnParameterPropertyChanged(object sender, PropertyChangedEventArgs e) {
			if(e.PropertyName == "Visible")
				RaisePropertyChanged("HasVisibleParameters");
		}
		void OnParameterValueChanged(object sender, EventArgs e) {
			var changedModel = sender as ParameterModel;
			IsChanged = CalculateIsChanged();
			ValidateParameters();
			if(Parameters.Any(x=> x.IsFilteredLookUpSettings))
				UpdateLookUpValues(changedModel);
		}
		void ValidateParameters() {
			Parameters.ForEach(x => RaiseValidate(x));
		}
		void RaiseValidate(ParameterModel model) {
			if(validate != null) {
				var args = new ValidateParameterEventArgs(model);
				validate(this, args);
				model.SetError(args.Error);
			}
		}
		bool CalculateIsChanged() {
			var isChanged = false;
			Parameters.ForEach(x => isChanged = isChanged | x.IsChanged);
			return isChanged;
		}
		void UpdateLookUpValues(ParameterModel changedParameterModel) {
			LookUpValuesProvider.Do(x => {
				CanEdit = false;
				var parameterValueProvider = new ParameterValueProvider(this.Parameters);
				var task = LookUpValuesProvider.GetLookUpValues(changedParameterModel.Parameter, parameterValueProvider);
				task.ContinueWith(t => {
					if(t.Exception != null) {
					}
					UpdateLookUpEditors(t.Result);
					CanEdit = true;
					IsChanged = CalculateIsChanged();
				});
			});
		}
		void UpdateLookUpEditors(IEnumerable<ParameterLookUpValuesContainer> lookUpValues) {
			foreach(var item in lookUpValues) {
				var parameterModel = Parameters.SingleOrDefault(model => model.Parameter == item.Parameter);
				parameterModel.Do(m => m.UpdateLookUpValues(item.LookUpValues));
			}
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
}
