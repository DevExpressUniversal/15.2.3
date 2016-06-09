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
	using DevExpress.Design.UI;
	using Utils.Filtering.Internal;
	public interface IFilteringModelConfiguratorContext {
		IFilteringModelMetadata Metadata { get; }
		object Component { get; }
		IEndUserFilteringSettingsFactory SettingsFactory { get; }
		IEndUserFilteringMetricAttributesFactory MetricAttributesFactory { get; }
		Type ModelType { get; set; }
		IEnumerable<IEndUserFilteringMetricAttributes> CustomAttributes { get; set; }
		IEndUserFilteringMetric SelectedMetric { get; set; }
	}
	public interface IFilteringModelConfiguratorViewModel :
		IStepByStepConfiguratorViewModel<IFilteringModelConfiguratorPageViewModel, IFilteringModelConfiguratorContext>, IDXDesignWindowContentViewModel {
	}
	public interface IFilteringModelConfiguratorPageViewModel
		: IStepByStepConfiguratorPageViewModel<IFilteringModelConfiguratorContext> {
	}
	public interface IModelTypeSettingsPageViewModel : IFilteringModelConfiguratorPageViewModel {
		IEnumerable<ITypeInfo> Types { get; }
		ITypeInfo ModelType { get; set; }
		bool HasModelType { get; }
		IEnumerable<IEndUserFilteringMetric> Metrics { get; }
		bool HasMetrics { get; }
		IEndUserFilteringMetric SelectedMetric { get; set; }
		IEnumerable<IFilteringModelMetricProperty> MetricProperties { get; }
		ICommand<object> EditCommand { get; }
		ICommand<object> NewCommand { get; }
	}
	public interface IMetricAttributesSettingsPageViewModel : IFilteringModelConfiguratorPageViewModel {
		IEndUserFilteringMetric Metric { get; }
		IEnumerable<IFilteringModelMetricProperty> MetricProperties { get; }
	}
	public interface ITypeInfo {
		Type Type { get; }
		string TypeName { get; }
	}
}
