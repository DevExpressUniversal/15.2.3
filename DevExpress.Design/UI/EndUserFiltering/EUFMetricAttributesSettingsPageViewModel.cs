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
	using Utils.Filtering.Internal;
	public class MetricAttributesSettingsPageViewModel : FilteringModelConfiguratorPageViewModel, IMetricAttributesSettingsPageViewModel {
		public MetricAttributesSettingsPageViewModel(IFilteringModelConfiguratorViewModel parentViewModel, IFilteringModelConfiguratorContext context)
			: base(parentViewModel, context) {
		}
		#region Properties
		IEndUserFilteringMetric metricCore;
		public IEndUserFilteringMetric Metric {
			get { return metricCore; }
		}
		IEnumerable<IFilteringModelMetricProperty> metricsPropertiesCore;
		public IEnumerable<IFilteringModelMetricProperty> MetricProperties {
			get { return metricsPropertiesCore; }
		}
		#endregion Properties
		IList<Attribute> metricAttributes;
		IEnumerable<IFilteringModelMetricProperty> CalcMetricProperties() {
			var metricProperties = new List<IFilteringModelMetricProperty>();
			if(Metric != null) {
				metricProperties.Add(new EditableEndUserFilteringMetricCaptionProperty(metricAttributes, Metric));
				metricProperties.Add(new EditableEndUserFilteringMetricEditorTypeProperty(metricAttributes, Metric));
				metricProperties.Add(new EditableEndUserFilteringMetricDescriptionProperty(metricAttributes, Metric));
			}
			return metricProperties;
		}
		protected override bool CalcIsCompleted(IFilteringModelConfiguratorContext context) {
			return (Metric != null);
		}
		protected override void OnEnter(IFilteringModelConfiguratorContext context) {
			metricCore = context.SelectedMetric;
			metricAttributes = new List<Attribute>(GetMetricAttributes());
			metricsPropertiesCore = CalcMetricProperties();
		}
		protected override void OnLeave(IFilteringModelConfiguratorContext context) {
			var settings = GetService<IEndUserFilteringSettings>();
			var attributesList = new List<IEndUserFilteringMetricAttributes>(settings.CustomAttributes);
			var index = attributesList.FindIndex(a => a.Path == Metric.Path);
			var ma = CreateMetricAttributes(Metric.Path, Metric.Type, metricAttributes.ToArray());
			if(index != -1)
				attributesList[index] = ma;
			else
				attributesList.Add(ma);
			context.CustomAttributes = attributesList.ToArray();
		}
		protected TService GetService<TService>() where TService : class {
			return (Metric is IServiceProvider) ? 
				((IServiceProvider)Metric).GetService(typeof(TService)) as TService : null;
		}
		protected IEnumerable<Attribute> GetMetricAttributes() {
			var settings = GetService<IEndUserFilteringSettings>();
			var attributes = settings.CustomAttributes.FirstOrDefault(a => a.Path == Metric.Path);
			return attributes ?? Enumerable.Empty<Attribute>();
		}
	}
}
