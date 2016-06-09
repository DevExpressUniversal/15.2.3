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
using DevExpress.Design.SmartTags;
using DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using DevExpress.Xpf.Core.Design;
namespace DevExpress.Xpf.Map.Design {
	public sealed class VectorLayerPropertyLinesProvider : PropertyLinesProviderBase {
		public VectorLayerPropertyLinesProvider() : base(typeof(VectorLayer)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			IPropertyLineCommandProvider listSourceProvider = new RunListSourceWizardCommandProvider(viewModel);
			IPropertyLineCommandProvider bubbleSourceProvider = new RunBubbleSourceWizardCommandProvider(viewModel);
			IPropertyLineCommandProvider pieSourceProvider = new RunPieSourceWizardCommandProvider(viewModel);
			IPropertyLineCommandProvider shapeSourceProvider = new CreateShapefileDataAdapterCommandProvider(viewModel);
			IPropertyLineCommandProvider kmlSourceProvider = new CreateKMLFileDataAdapterCommandProvider(viewModel);
			lines.Add(() => ActionPropertyLineViewModel.CreateLine(listSourceProvider));
			lines.Add(() => ActionPropertyLineViewModel.CreateLine(bubbleSourceProvider));
			lines.Add(() => ActionPropertyLineViewModel.CreateLine(pieSourceProvider));
			lines.Add(() => new SeparatorLineViewModel(viewModel));
			lines.Add(() => ActionPropertyLineViewModel.CreateLine(shapeSourceProvider));
			lines.Add(() => ActionPropertyLineViewModel.CreateLine(kmlSourceProvider));
			return lines;
		}
	}
	public sealed class ShapefileDataAdapterPropertyLinesProvider : PropertyLinesProviderBase {
		public ShapefileDataAdapterPropertyLinesProvider() : base(typeof(ShapefileDataAdapter)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => ShapefileDataAdapter.FileUriProperty)));
			return lines;
		}
	}
	public sealed class KmlFileDataAdapterPropertyLinesProvider : PropertyLinesProviderBase {
		public KmlFileDataAdapterPropertyLinesProvider() : base(typeof(KmlFileDataAdapter)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => KmlFileDataAdapter.FileUriProperty)));
			return lines;
		}
	}
	public sealed class DataSourceDataAdapterPropertyLinesProvider : PropertyLinesProviderBase {
		public DataSourceDataAdapterPropertyLinesProvider() : base(typeof(DataSourceAdapterBase)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DataSourceAdapterBase.DataSourceProperty)));
			return lines;
		}
	}
	public abstract class RunDataSourceWizardCommandProvider : CommandActionLineProvider {
		IModelProperty DataProperty { get { return Context.ModelItem.Properties["Data"]; } }
		IModelProperty MappingsProperty { get { return DataProperty.Value.Properties["Mappings"]; } }
		protected abstract Type SupportedDataType { get; }
		public RunDataSourceWizardCommandProvider(FrameworkElementSmartTagPropertiesViewModel ownerViewModel)
			: base(ownerViewModel) {
		}
		void CreateMappings() {
			IModelProperty mappingsProperty = MappingsProperty;
			if (mappingsProperty.ComputedValue == null)
				mappingsProperty.SetValue(CreateMappingsCore());
		}
		bool ShouldCreateDataAdapter(MapDataAdapterBase initialAdapter) {
			return initialAdapter == null || !initialAdapter.GetType().IsAssignableFrom(SupportedDataType);
		}
		protected abstract MapItemMappingInfoBase CreateMappingsCore();
		protected abstract MapDataAdapterBase CreateDataAdapter();
		protected override void OnCommandExecute(object param) {
			if (Context != null && Context.ModelItem != null && Context.ModelItem.View != null && Context.ModelItem.View.PlatformObject is VectorLayer) {
				IModelProperty dataProperty = DataProperty;
				MapDataAdapterBase initialDataAdapter = dataProperty.ComputedValue as MapDataAdapterBase;
				if (ShouldCreateDataAdapter(initialDataAdapter))
					dataProperty.SetValue(CreateDataAdapter());
				CreateMappings();
				bool? wizardResult = DevExpress.Xpf.Core.Design.DataAccess.UI.ItemsSourceWizard.Run(XpfModelItem.ToModelItem(dataProperty.Value));
				if (!wizardResult.HasValue || wizardResult == false)
					dataProperty.SetValue(initialDataAdapter);
			}
		}
	}
	public class RunListSourceWizardCommandProvider : RunDataSourceWizardCommandProvider {
		protected override Type SupportedDataType { get { return typeof(ListSourceDataAdapter); } }
		public RunListSourceWizardCommandProvider(FrameworkElementSmartTagPropertiesViewModel ownerViewModel)
			: base(ownerViewModel) {
		}
		protected override string GetCommandText() {
			return "Add Data Source";
		}
		protected override MapItemMappingInfoBase CreateMappingsCore() {
			return new MapItemMappingInfo();
		}
		protected override MapDataAdapterBase CreateDataAdapter() {
			return new ListSourceDataAdapter();
		}
	}
	public class RunBubbleSourceWizardCommandProvider : RunDataSourceWizardCommandProvider {
		protected override Type SupportedDataType { get { return typeof(BubbleChartDataAdapter); } }
		public RunBubbleSourceWizardCommandProvider(FrameworkElementSmartTagPropertiesViewModel ownerViewModel)
			: base(ownerViewModel) {
		}
		protected override string GetCommandText() {
			return "Add Bubble Chart Source";
		}
		protected override MapItemMappingInfoBase CreateMappingsCore() {
			return new MapBubbleMappingInfo();
		}
		protected override MapDataAdapterBase CreateDataAdapter() {
			return new BubbleChartDataAdapter();
		}
	}
	public class RunPieSourceWizardCommandProvider : RunDataSourceWizardCommandProvider {
		protected override Type SupportedDataType { get { return typeof(PieChartDataAdapter); } }
		public RunPieSourceWizardCommandProvider(FrameworkElementSmartTagPropertiesViewModel ownerViewModel)
			: base(ownerViewModel) {
		}
		protected override string GetCommandText() {
			return "Add Pie Chart Source";
		}
		protected override MapItemMappingInfoBase CreateMappingsCore() {
			return new MapPieMappingInfo();
		}
		protected override MapDataAdapterBase CreateDataAdapter() {
			return new PieChartDataAdapter();
		}
	}
	public abstract class CreateFileDataAdapterCommandProvider : CommandActionLineProvider {
		IModelProperty DataProperty { get { return Context.ModelItem.Properties["Data"]; } }
		protected abstract Type SupportedDataType { get; }
		public CreateFileDataAdapterCommandProvider(FrameworkElementSmartTagPropertiesViewModel ownerViewModel)
			: base(ownerViewModel) {
		}
		bool ShouldCreateDataAdapter(MapDataAdapterBase initialAdapter) {
			return initialAdapter == null || !initialAdapter.GetType().IsAssignableFrom(SupportedDataType);
		}
		void SetFileUri() {
			IModelProperty uriProperty = DataProperty.Value.Properties["FileUri"];
			if (uriProperty.ComputedValue == null)
				uriProperty.SetValue("INSERT_FILE_URI");
		}
		protected abstract MapDataAdapterBase CreateDataAdapter();
		protected override void OnCommandExecute(object param) {
			if (Context != null && Context.ModelItem != null && Context.ModelItem.View != null && Context.ModelItem.View.PlatformObject is VectorLayer) {
				IModelProperty dataProperty = DataProperty;
				MapDataAdapterBase initialDataAdapter = dataProperty.ComputedValue as MapDataAdapterBase;
				if (ShouldCreateDataAdapter(initialDataAdapter))
					dataProperty.SetValue(CreateDataAdapter());
				SetFileUri();
			}
		}
	}
	public class CreateShapefileDataAdapterCommandProvider : CreateFileDataAdapterCommandProvider {
		protected override Type SupportedDataType { get { return typeof(ShapefileDataAdapter); } }
		public CreateShapefileDataAdapterCommandProvider(FrameworkElementSmartTagPropertiesViewModel ownerViewModel)
			: base(ownerViewModel) {
		}
		protected override string GetCommandText() {
			return "Add Shapefile Source";
		}
		protected override MapDataAdapterBase CreateDataAdapter() {
			return new ShapefileDataAdapter();
		}
	}
	public class CreateKMLFileDataAdapterCommandProvider : CreateFileDataAdapterCommandProvider {
		protected override Type SupportedDataType { get { return typeof(KmlFileDataAdapter); } }
		public CreateKMLFileDataAdapterCommandProvider(FrameworkElementSmartTagPropertiesViewModel ownerViewModel)
			: base(ownerViewModel) {
		}
		protected override string GetCommandText() {
			return "Add KML File Source";
		}
		protected override MapDataAdapterBase CreateDataAdapter() {
			return new KmlFileDataAdapter();
		}
	}
}
