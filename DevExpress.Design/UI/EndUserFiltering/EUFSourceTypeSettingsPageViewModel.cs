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
	using System.Linq;
	using DevExpress.Design.UI;
	using Utils.Filtering.Internal;
	public class ModelTypeSettingsPageViewModel : FilteringModelConfiguratorPageViewModel, IModelTypeSettingsPageViewModel {
		public ModelTypeSettingsPageViewModel(IFilteringModelConfiguratorViewModel parentViewModel, IFilteringModelConfiguratorContext context)
			: base(parentViewModel, context) {
			AssertionException.IsNotNull(context);
			EditCommand = new WpfDelegateCommand(OnEdit, CanEdit);
			NewCommand = new WpfDelegateCommand(OnNew);
			this.typesCore = ServiceContainer.Resolve<DevExpress.Design.DataAccess.IDataAccessTechnologyTypesProvider>()
				.Select(t => CreateTypeInfo(t));
			StartInitializationAsync(context);
		}
		public ICommand<object> EditCommand {
			get;
			private set;
		}
		public ICommand<object> NewCommand {
			get;
			private set;
		}
		bool CanEdit() {
			return (SelectedMetric != null);
		}
		void OnEdit() {
			var parentViewModel = GetParentViewModel<IFilteringModelConfiguratorViewModel>();
			if(parentViewModel != null) {
				if(parentViewModel.PrevPageCommand.CanExecute(this))
					parentViewModel.PrevPageCommand.Execute(this);
			}
		}
		void OnNew() {
		}
		#region Properties
		ITypeInfo modelTypeCore;
		public ITypeInfo ModelType {
			get { return modelTypeCore; }
			set { SetProperty(ref modelTypeCore, value, "ModelType", OnModelTypeChanged); }
		}
		bool hasModelTypeCore;
		public bool HasModelType {
			get { return hasModelTypeCore; }
			private set { SetProperty(ref hasModelTypeCore, value, "HasModelType"); }
		}
		IEnumerable<IEndUserFilteringMetric> metricsCore;
		public IEnumerable<IEndUserFilteringMetric> Metrics {
			get { return metricsCore; }
			private set { SetProperty(ref metricsCore, value, "Metrics"); }
		}
		bool hasMetricsCore;
		public bool HasMetrics {
			get { return hasMetricsCore; }
			private set { SetProperty(ref hasMetricsCore, value, "HasMetrics"); }
		}
		IEndUserFilteringMetric selectedMetricCore;
		public IEndUserFilteringMetric SelectedMetric {
			get { return selectedMetricCore; }
			set { SetProperty(ref selectedMetricCore, value, "SelectedMetric", OnSelectedMetricChanged); }
		}
		IEnumerable<ITypeInfo> typesCore;
		public IEnumerable<ITypeInfo> Types {
			get { return typesCore; }
		}
		IEnumerable<IFilteringModelMetricProperty> metricsPropertiesCore;
		public IEnumerable<IFilteringModelMetricProperty> MetricProperties {
			get { return metricsPropertiesCore; }
		}
		#endregion Properties
		void OnModelTypeChanged() {
			HasModelType = (ModelType != null);
			Type modelType = GetType(ModelType);
			((IFilteringModelConfiguratorPageViewModel)this).Context.ModelType = modelType;
			Metrics = CreateSettings(modelType, GetCustomAttributes());
			HasMetrics = Metrics.Any();
			SelectedMetric = Metrics.FirstOrDefault();
			UpdateIsCompleted();
		}
		void OnSelectedMetricChanged() {
			metricsPropertiesCore = CalcMetricProperties();
			RaisePropertyChanged(() => MetricProperties);
			((WpfDelegateCommand)EditCommand).RaiseCanExecuteChanged();
			UpdateIsCompleted();
		}
		IEnumerable<IFilteringModelMetricProperty> CalcMetricProperties() {
			var metricProperties = new List<IFilteringModelMetricProperty>();
			if(SelectedMetric != null) {
				if(SelectedMetric.Caption != null)
					metricProperties.Add(new EndUserFilteringMetricProperty(MetricPropertyCodeName.Caption, SelectedMetric));
				metricProperties.Add(new EndUserFilteringMetricProperty(MetricPropertyCodeName.EditorType, SelectedMetric));
				if(SelectedMetric.Description != null)
					metricProperties.Add(new EndUserFilteringMetricProperty(MetricPropertyCodeName.Description, SelectedMetric));
			}
			return metricProperties;
		}
		protected override bool CalcIsCompleted(IFilteringModelConfiguratorContext context) {
			return HasMetrics || (SelectedMetric != null);
		}
		protected override void OnEnter(IFilteringModelConfiguratorContext context) {
			StartInitializationAsync(context);
		}
		protected override void OnLeave(IFilteringModelConfiguratorContext context) {
			context.ModelType = GetType(ModelType);
			context.CustomAttributes = CalcCustomMetrics();
			context.SelectedMetric = SelectedMetric;
		}
		IEnumerable<IEndUserFilteringMetricAttributes> CalcCustomMetrics() {
			return ((IEndUserFilteringSettings)Metrics).CustomAttributes;
		}
#if DEBUGTEST
		internal static bool avoidAsync = false;
#endif
		static InitializationInfo inititalizationInfo;
		void StartInitializationAsync(IFilteringModelConfiguratorContext context) {
#if DEBUGTEST
			if(avoidAsync) {
				InitMetrics(context);
				NotifyInitialized();
				return;
			}
#endif
			if(inititalizationInfo == null) {
				inititalizationInfo = new InitializationInfo(context);
				System.Threading.ThreadPool.RegisterWaitForSingleObject(inititalizationInfo.Completed, InitializationComplete, inititalizationInfo, -1, true);
				System.Threading.ThreadPool.QueueUserWorkItem(InitializationRoutine, inititalizationInfo);
			}
		}
		class InitializationInfo {
			public InitializationInfo(IFilteringModelConfiguratorContext context) {
				this.Context = context;
				this.Completed = new System.Threading.ManualResetEvent(false);
			}
			public readonly System.Threading.ManualResetEvent Completed;
			public IFilteringModelConfiguratorContext Context { get; private set; }
		}
		void InitializationRoutine(object state) {
			InitializationInfo info = state as InitializationInfo;
			try {
				InitMetrics(info.Context);
			}
			finally { info.Completed.Set(); }
		}
		void InitializationComplete(object state, bool timeout) {
			InitializationInfo info = state as InitializationInfo;
			try {
				NotifyInitialized();
				inititalizationInfo = null;
			}
			finally { info.Completed.Close(); }
		}
		void NotifyInitialized() {
			RaisePropertiesChanged(
				"ModelType", "HasModelType",
				"Metrics", "HasMetrics", "SelectedMetric",
				"MetricProperties");
			((WpfDelegateCommand)EditCommand).RaiseCanExecuteChanged();
			UpdateIsCompleted();
		}
		void InitMetrics(IFilteringModelConfiguratorContext context) {
			Type type = GetModelType(context);
			this.modelTypeCore = GetModelTypeInfoFromType(type);
			this.hasModelTypeCore = (ModelType != null);
			this.metricsCore = CreateSettings(GetType(ModelType), GetCustomAttributes(context, GetMetricAttributesCollectionFactory()));
			this.hasMetricsCore = Metrics.Any();
			this.selectedMetricCore = (context.SelectedMetric != null) ? Metrics.FirstOrDefault((m => m.Path == context.SelectedMetric.Path)) : Metrics.FirstOrDefault();
			this.metricsPropertiesCore = CalcMetricProperties();
		}
		protected virtual ITypeInfo GetModelTypeInfoFromType(Type type) {
			if(typesCore == null || type == null) return null;
			foreach(var typeInfo in typesCore)
				if(typeInfo.Type == type)
					return typeInfo;
			return CreateTypeInfo(type);
		}
		protected virtual Type GetType(ITypeInfo typeInfo) {
			return (typeInfo != null) ? typeInfo.Type : null;
		}
		protected virtual ITypeInfo CreateTypeInfo(Type type) {
			return new TypeInfo(type);
		}
		#region TypeInfo
		class TypeInfo : ITypeInfo {
			public TypeInfo(Type type) {
				this.Type = type;
				this.TypeName = type.Name;
			}
			public Type Type { get; private set; }
			public string TypeName { get; private set; }
			public override string ToString() {
				return TypeName;
			}
			public override int GetHashCode() {
				return TypeName.GetHashCode();
			}
		}
		#endregion
	}
}
