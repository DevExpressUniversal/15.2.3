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

using DevExpress.Design.SmartTags;
using DevExpress.Xpf.Core.Design;
using System.Collections.Generic;
using System;
namespace DevExpress.Xpf.Map.Design {
	public sealed class InformationLayerPropertyLinesProvider : PropertyLinesProviderBase {
		public InformationLayerPropertyLinesProvider() : base(typeof(InformationLayer)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			lines.Add(() => new ItemListPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => InformationLayer.DataProviderProperty), typeof(InformationDataProviderBase), DXTypeInfoInstanceSource.FromTypeList(RegisterHelper.InformationDataProviderTypes)));
			lines.Add(() => new NestedPropertyLinesViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => InformationLayer.DataProviderProperty)));
			return lines;
		}
	}
	public sealed class BingGeocodeDataProviderPropertyLinesProvider : PropertyLinesProviderBase {
		public BingGeocodeDataProviderPropertyLinesProvider() : base(typeof(BingGeocodeDataProvider)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => BingGeocodeDataProvider.BingKeyProperty)));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => BingGeocodeDataProvider.MaxVisibleResultCountProperty)));
			return lines;
		}
	}
	public sealed class BingSearchDataProviderPropertyLinesProvider : PropertyLinesProviderBase {
		public BingSearchDataProviderPropertyLinesProvider() : base(typeof(BingSearchDataProvider)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => BingSearchDataProvider.BingKeyProperty)));
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => BingSearchDataProvider.ShowSearchPanelProperty)));
			return lines;
		}
	}
	public sealed class BingRouteDataProviderPropertyLinesProvider : PropertyLinesProviderBase {
		public BingRouteDataProviderPropertyLinesProvider() : base(typeof(BingRouteDataProvider)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => BingRouteDataProvider.BingKeyProperty)));
			return lines;
		}
	}
}
