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

namespace DevExpress.Design.Filtering.UI {
	using System;
	using System.Collections.Generic;
	using DevExpress.Design.Filtering.Services;
	using DevExpress.Design.UI;
	using DevExpress.Utils.Filtering.Internal;
	public abstract class FilteringModelConfiguratorPageViewModel : WpfViewModelBase, IFilteringModelConfiguratorPageViewModel {
		readonly IFilteringModelConfiguratorContext contextCore;
		public FilteringModelConfiguratorPageViewModel(IFilteringModelConfiguratorViewModel parentViewModel, IFilteringModelConfiguratorContext context)
			: base(parentViewModel) {
			AssertionException.IsNotNull(context);
			this.contextCore = context;
		}
		IFilteringModelConfiguratorContext IStepByStepConfiguratorPageViewModel<IFilteringModelConfiguratorContext>.Context {
			get { return contextCore; }
		}
		void IStepByStepConfiguratorPageViewModel<IFilteringModelConfiguratorContext>.Enter() {
			OnEnter(contextCore);
		}
		void IStepByStepConfiguratorPageViewModel<IFilteringModelConfiguratorContext>.Leave() {
			OnLeave(contextCore);
		}
		bool IStepByStepConfiguratorPageViewModel<IFilteringModelConfiguratorContext>.IsCompleted {
			get { return CalcIsCompleted(contextCore); }
		}
		protected abstract void OnEnter(IFilteringModelConfiguratorContext context);
		protected abstract void OnLeave(IFilteringModelConfiguratorContext context);
		protected abstract bool CalcIsCompleted(IFilteringModelConfiguratorContext context);
		protected void UpdateIsCompleted() {
			RaisePropertyChanged("IsCompleted");
		}
		#region API
		protected Type GetModelType() {
			return GetModelType(contextCore);
		}
		protected IEndUserFilteringSettingsFactory GetSettingsFactory() {
			return contextCore.SettingsFactory;
		}
		protected IEndUserFilteringMetricAttributesFactory GetMetricAttributesFactory() {
			return contextCore.MetricAttributesFactory;
		}
		protected IEnumerable<IEndUserFilteringMetricAttributes> GetCustomAttributes() {
			return GetCustomAttributes(contextCore, GetMetricAttributesCollectionFactory());
		}
		protected IEndUserFilteringSettings CreateSettings(Type modelType, IEnumerable<IEndUserFilteringMetricAttributes> customAttributes) {
			return GetSettingsFactory().Create(modelType, customAttributes);
		}
		protected IEndUserFilteringMetricAttributes CreateMetricAttributes(string path, Type type, Attribute[] attributes) {
			return GetMetricAttributesFactory().Create(path, type, attributes);
		}
		protected IFilteringModelMetricAttributesCollectionFactory GetMetricAttributesCollectionFactory() {
			return Platform.ServiceContainer.Resolve<IFilteringModelMetricAttributesCollectionFactory>(contextCore.Metadata.Platform);
		}
		protected static Type GetModelType(IFilteringModelConfiguratorContext context) {
			return context.ModelType ?? GetModelType(context.Component, context.Metadata.ModelTypeProperty);
		}
		protected static IEnumerable<IEndUserFilteringMetricAttributes> GetCustomAttributes(IFilteringModelConfiguratorContext context, IFilteringModelMetricAttributesCollectionFactory factory) {
			return context.CustomAttributes ?? GetCustomAttributes(factory, context);
		}
		static Type GetModelType(object component, string modelTypeProperty) {
			return GetValue(component, modelTypeProperty) as Type;
		}
		static object GetValue(object component, string property) {
			if(component == null || string.IsNullOrEmpty(property)) return null;
			var pd = System.ComponentModel.TypeDescriptor.GetProperties(component)[property];
			return (pd != null) ? pd.GetValue(component) : null;
		}
		static IEnumerable<IEndUserFilteringMetricAttributes> GetCustomAttributes(IFilteringModelMetricAttributesCollectionFactory factory, IFilteringModelConfiguratorContext context) {
			return factory.Create(context);
		}
		#endregion API
	}
}
