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
using System.Drawing;
using DevExpress.Data.Browsing;
using DevExpress.Data.Browsing.Design;
using DevExpress.Data.Design;
using DevExpress.Snap.Core.Native;
using DevExpress.Snap.Core.Native.Data;
using DevExpress.Snap.Core.Native.Data.Implementations;
using DevExpress.Snap.Core.Native.Services;
using DevExpress.Office.Utils;
using DevExpress.Services.Internal;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.Snap.Core.Fields {
	public class SNChartField : SNMergeFieldBase {
		#region static
		public static new readonly string FieldType = "SNCHART";
		public static readonly string ChartLayoutSwitch = "l";
		public static readonly string ChartSeriesDataBindingsSwitch = "d";
		public static readonly string ChartDataBindingsSwitch = "ds";
		public static readonly string ChartWidthSwitch = "w";
		public static readonly string ChartHeightSwitch = "h";
		public static new CalculatedFieldBase Create() {
			return new SNChartField();
		}
		static readonly Dictionary<string, bool> chartSwitchesWithArgument;
		static SNChartField() {
			chartSwitchesWithArgument = CreateSwitchesWithArgument(ChartLayoutSwitch, ChartSeriesDataBindingsSwitch, ChartDataBindingsSwitch, ChartWidthSwitch, ChartHeightSwitch);
			foreach(KeyValuePair<string, bool> sw in MergefieldField.SwitchesWithArgument)
				chartSwitchesWithArgument.Add(sw.Key, sw.Value);
		}
		#endregion
		readonly Size DefaultSize = new Size(400, 300);
		Base64StringDataContainer currentLayout;
		protected override Dictionary<string, bool> SwitchesWithArguments { get { return chartSwitchesWithArgument; } }
		public Base64StringDataContainer Layout { get; private set; }
		public Base64StringDataContainer SeriesDataBindings { get; private set; }
		public String ChartDataBinding { get; private set; }
		public override void Initialize(PieceTable pieceTable, InstructionCollection instructions) {
			base.Initialize(pieceTable, instructions);
			SetLayout(pieceTable, instructions);
			SetChartDataBindings(instructions);
			SetSeriesDataBindings(pieceTable, instructions);
		}
		public override bool HasProperties { get { return false; } }
		protected override bool SholdApplyFormating {
			get { return false; }
		}
		void SetLayout(PieceTable pieceTable, InstructionCollection instructions) {
			string str = instructions.GetBase64String(ChartLayoutSwitch, pieceTable);
			if (String.IsNullOrEmpty(str))
				str = instructions.GetString(ChartLayoutSwitch);
			if (!String.IsNullOrEmpty(str))
				Layout = Base64StringDataContainer.FromBase64String(str);
		}
		void SetSeriesDataBindings(PieceTable pieceTable, InstructionCollection instructions) {
			string str = instructions.GetBase64String(ChartSeriesDataBindingsSwitch, pieceTable);
			if (String.IsNullOrEmpty(str))
				str = instructions.GetString(ChartSeriesDataBindingsSwitch);
			if (!String.IsNullOrEmpty(str))
				SeriesDataBindings = Base64StringDataContainer.FromBase64String(str);
		}
		void SetChartDataBindings(InstructionCollection instructions) {
			string str = instructions.GetString(ChartDataBindingsSwitch);
			if(!String.IsNullOrEmpty(str))
				ChartDataBinding = str;
		}
		public ISNChartContainer GetChartContainer(SnapDocumentModel documentModel, SnapFieldInfo fieldInfo) {
			ServiceManager serviceManager = GetServiceManager(documentModel);
			IDataContextService dataContextService = (IDataContextService)serviceManager.GetService(typeof(IDataContextService));
			IFieldDataAccessService fieldDataAccessService = documentModel.GetService<IFieldDataAccessService>();
			ICalculationContext calculationContext = fieldDataAccessService.FieldContextService.BeginCalculation(documentModel.DataSourceDispatcher);
			try {
				IFieldContext fieldContext = fieldDataAccessService.GetFieldContext(fieldInfo);
				DataContext dataContext = dataContextService.CreateDataContext(new Data.Browsing.DataContextOptions(true, true), true);
				calculationContext.PrepareDataContext(dataContext, fieldContext);
				ISNChartContainer chartContainer = CreateChartContainer(documentModel, dataContext);
				AssignDataSources(documentModel.DataSourceDispatcher, chartContainer);
				return chartContainer;
			}
			finally {
				fieldDataAccessService.FieldContextService.EndCalculation(calculationContext);
			}
		}
		void AssignDataSources(IDataSourceDispatcher dispatcher, ISNChartContainer chartContainer) {
			try {
				chartContainer.Chart.AssignSeriesDatasources(dispatcher);
				chartContainer.Chart.AssignChartDataSource(dispatcher, ChartDataBinding);
			}
			catch {
			}
		}
		ServiceManager GetServiceManager(SnapDocumentModel documentModel) {
			ServiceManager serviceManager = new ServiceManager();
			serviceManager.AddService(typeof(IDataContextService), new DataContextService(documentModel));
			serviceManager.AddService(typeof(IDataSourceCollectorService), new DataSourceCollectorService(documentModel));
			serviceManager.AddService(typeof(IDataSourceDisplayNameProvider), new DataSourceDisplayNameProvider(documentModel));
			serviceManager.AddService(typeof(IMessageBoxService), documentModel.GetService(typeof(IMessageBoxService)));
			return serviceManager;
		}
		ISNChartContainer CreateChartContainer(SnapDocumentModel documentModel, DataContext dataContext) {
			IChartService chartService = documentModel.GetService<IChartService>();
			ISNChartContainer chartContainer = chartService.GetChartContainer(GetServiceManager(documentModel), dataContext);
			if(Layout == null)
				chartService.LoadDefaultChart(chartContainer.Chart);
			else
				LoadChart(chartContainer.Chart);
			return chartContainer;
		}
		void LoadChart(SNChart chart) {
			if(currentLayout == Layout) return;
			SNChartHelper.LoadChartLayout(chart, Layout);
			if(SeriesDataBindings != null)
				SNChartHelper.LoadChartSeriesDataBindings(chart, SeriesDataBindings);
			currentLayout = Layout;
		}
		OfficeImage GetChartImage(SnapDocumentModel documentModel, Size sizeInPixels, SnapFieldInfo fieldInfo) {
			using (ISNChartContainer container = GetChartContainer(documentModel, fieldInfo)) 
				return documentModel.CreateImage(CreateChartRawImage(container.Chart, sizeInPixels));
		}
		Image CreateChartRawImage(SNChart chart, Size sizeInPixels) {
			if (chart.OptionsPrint.ImageFormat == XtraCharts.Printing.PrintImageFormat.Metafile)
				return chart.CreateMetafile(sizeInPixels, System.Drawing.Imaging.MetafileFrameUnit.Pixel);
			return chart.CreateBitmap(sizeInPixels);
		}
		public override CalculatedFieldValue GetCalculatedValueCore(PieceTable sourcePieceTable, MailMergeDataMode mailMergeDataMode, Field documentField) {
			SnapDocumentModel documentModel = ((SnapPieceTable)sourcePieceTable).DocumentModel;
			ChartRun oldRun = sourcePieceTable.Runs[documentField.Result.Start] as ChartRun;
			Size? oldSize = null;
			Size size;
			if (oldRun != null) {
				oldSize = oldRun.ActualSize;
				size = SnapSizeConverter.ModelUnitsToPixels(documentModel, oldRun.ActualSize, oldRun.Image.HorizontalResolution);
			}
			else
				size = GetSavedSize() ?? DefaultSize;
			DocumentModel targetModel = documentModel.GetFieldResultModel();
			ChartRun run = ((SnapPieceTable)targetModel.MainPieceTable).InsertChart(DocumentLogPosition.Zero, GetChartImage(documentModel, size, new SnapFieldInfo((SnapPieceTable)sourcePieceTable, documentField)));
			if (oldSize.HasValue)
				run.ActualSize = oldSize.Value;
			run.ResizingShadowDisplayMode = ResizingShadowDisplayMode.WireFrame;
			run.LockAspectRatio = false;
			return new CalculatedFieldValue(targetModel);
		}
		Size? GetSavedSize() {
			int? width = Switches.GetNullableInt(ChartWidthSwitch);
			int? height = Switches.GetNullableInt(ChartHeightSwitch);
			if(!width.HasValue || !height.HasValue)
				return null;
			return new Size(width.Value, height.Value);
		}
	}
	public class SNChartFieldController : SizeAndScaleFieldController<SNChartField> {
		public SNChartFieldController(InstructionController controller)
			: base(controller, GetRectangularObject(controller)) {
		}
		static IRectangularObject GetRectangularObject(InstructionController controller) {
			OfficeImage image = ((ChartRun)controller.PieceTable.Runs[controller.Field.Result.Start]).Image;
			return new RichEditImageWrapper(image);
		}
		protected override void SetImageSizeInfoCore() {
			SetSize();
		}
	}
}
