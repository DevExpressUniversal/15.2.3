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
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.Office.Drawing;
using DevExpress.Office.DrawingML;
using DevExpress.Office.Model;
namespace DevExpress.XtraSpreadsheet.Model {
	#region DocumentCache
	public class DocumentCache :  IDisposable {
		#region Fields
		bool isDisposed;
		CellAlignmentInfoCache cellAlignmentInfoCache;
		ColorModelInfoCache colorModelInfoCache;
		BorderInfoCache borderInfoCache;
		RunFontInfoCache fontInfoCache;
		FillInfoCache fillInfoCache;
		NumberFormatCollection numberFormatCache;
		CellFormatCache cellFormatCache;
		MarginsInfoCache marginInfoCache;
		PrintSetupInfoCache printSetupInfoCache;
		GroupAndOutlinePropertiesInfoCache groupAndOutlinePropertiesInfoCache;
		AutoFilterColumnInfoCache autoFilterColumnInfoCache;
		SortStateInfoCache sortStateInfoCache;
		SheetFormatInfoCache sheetFormatInfoCache;
		SortConditionInfoCache sortConditionInfoCache;
		TableInfoCache tableInfoCache;
		VmlShapeInfoCache vmlShapeInfoCache;
		DateGroupingInfoCache dateGroupingInfoCache;
		WorkbookWindowInfoCache windowInfoCache;
		WorksheetProtectionInfoCache worksheetProtectionInfoCache;
		DataValidationInfoCache dataValidationInfoCache;
		CalculationOptionsInfoCache calculationOptionsInfoCache;
		TableStyleElementFormatCache tableStyleElementFormatCache;
		ConditionalFormattingValueInfoCache conditionalFormattingValueInfoCache;
		ConditionalFormattingInfoCache conditionalFormattingInfoCache;
		DrawingObjectInfoCache drawingObjectInfoCache;
		PictureInfoCache pictureInfoCache;
		ModelShapeInfoCache modelShapeInfoCache;
		Transform2DInfoCache transform2DInfoCache;
		ShapeStyleInfoCache shapeStyleInfoCache;
		ShapePropertiesInfoCache shapePropertiesInfoCache;
		HeaderFooterInfoCache headeFooterInfoCache;
		ChartInfoCache chartInfoCache;
		View3DInfoCache view3DInfoCache;
		PictureOptionsInfoCache pictureOptionsInfoCache;
		DataTableInfoCache dataTableInfoCache;
		AxisInfoCache axisInfoCache;
		ScalingInfoCache scalingInfoCache;
		DisplayUnitInfoCache displayUnitInfoCache;
		ChartViewInfoCache chartViewInfoCache;
		DataLabelInfoCache dataLabelInfoCache;
		ErrorBarsInfoCache errorBarsInfoCache;
		TrendlineInfoCache trendlineInfoCache;
		GradientFillInfoCache gradientFillInfoCache;
		GradientStopInfoCache gradientStopInfoCache;
		SparklineGroupInfoCache sparklineGroupInfoCache;
		PivotTableCache pivotTableCache;
		PivotFieldCache pivotFieldCache;
		IgnoredErrorInfoCache ignoredErrorInfoCache;
		CommonDrawingLocksInfoCache commonDrawingLocksInfoCache;
		GroupShapeInfoCache groupShapeInfoCache;
		FontKeyCache fontKeyCache;
		#endregion
		public DocumentCache(DocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			Initialize(documentModel);
		}
		#region Properties
		protected internal bool IsDisposed { get { return isDisposed; } }
		public CellAlignmentInfoCache CellAlignmentInfoCache { get { return cellAlignmentInfoCache; } }
		public ColorModelInfoCache ColorModelInfoCache { get { return colorModelInfoCache; } }
		public BorderInfoCache BorderInfoCache { get { return borderInfoCache; } }
		public RunFontInfoCache FontInfoCache { get { return fontInfoCache; } }
		public FillInfoCache FillInfoCache { get { return fillInfoCache; } }
		public NumberFormatCollection NumberFormatCache { get { return numberFormatCache; } }
		public CellFormatCache CellFormatCache { get { return cellFormatCache; } }
		public MarginsInfoCache MarginInfoCache { get { return marginInfoCache; } }
		public PrintSetupInfoCache PrintSetupInfoCache { get { return printSetupInfoCache; } }
		public GroupAndOutlinePropertiesInfoCache GroupAndOutlinePropertiesInfoCache { get { return groupAndOutlinePropertiesInfoCache; } }
		public AutoFilterColumnInfoCache AutoFilterColumnInfoCache { get { return autoFilterColumnInfoCache; } }
		public SortStateInfoCache SortStateInfoCache { get { return sortStateInfoCache; } }
		public SheetFormatInfoCache SheetFormatInfoCache { get { return sheetFormatInfoCache; } }
		public SortConditionInfoCache SortConditionInfoCache { get { return sortConditionInfoCache; } }
		public TableInfoCache TableInfoCache { get { return tableInfoCache; } }
		public VmlShapeInfoCache VmlShapeInfoCache { get { return vmlShapeInfoCache; } }
		public DateGroupingInfoCache DateGroupingInfoCache { get { return dateGroupingInfoCache; } }
		public WorkbookWindowInfoCache WindowInfoCache { get { return windowInfoCache; } }
		public WorksheetProtectionInfoCache WorksheetProtectionInfoCache { get { return worksheetProtectionInfoCache; } }
		public DataValidationInfoCache DataValidationInfoCache { get { return dataValidationInfoCache; } }
		public CalculationOptionsInfoCache CalculationOptionsInfoCache { get { return calculationOptionsInfoCache; } }
		public TableStyleElementFormatCache TableStyleElementFormatCache { get { return tableStyleElementFormatCache; } }
		public ConditionalFormattingValueInfoCache ConditionalFormattingValueCache { get { return conditionalFormattingValueInfoCache; } }
		public ConditionalFormattingInfoCache ConditionalFormattingInfoCache { get { return conditionalFormattingInfoCache; } }
		public DrawingObjectInfoCache DrawingObjectInfoCache { get { return drawingObjectInfoCache; } }
		public PictureInfoCache PictureInfoCache { get { return pictureInfoCache; } }
		public ModelShapeInfoCache ModelShapeInfoCache { get { return modelShapeInfoCache; } }
		public Transform2DInfoCache Transform2DInfoCache { get { return transform2DInfoCache; } }
		public ShapeStyleInfoCache ShapeStyleInfoCache { get { return shapeStyleInfoCache; } }
		public ShapePropertiesInfoCache ShapePropertiesInfoCache { get { return shapePropertiesInfoCache; } }
		public HeaderFooterInfoCache HeaderFooterInfoCache { get { return headeFooterInfoCache; } }
		public ChartInfoCache ChartInfoCache { get { return chartInfoCache; } }
		public View3DInfoCache View3DInfoCache { get { return view3DInfoCache; } }
		public PictureOptionsInfoCache PictureOptionsInfoCache { get { return pictureOptionsInfoCache; } }
		public DataTableInfoCache DataTableInfoCache { get { return dataTableInfoCache; } }
		public AxisInfoCache AxisInfoCache { get { return axisInfoCache; } }
		public ScalingInfoCache ScalingInfoCache { get { return scalingInfoCache; } }
		public DisplayUnitInfoCache DisplayUnitInfoCache { get { return displayUnitInfoCache; } }
		public ChartViewInfoCache ChartViewInfoCache { get { return chartViewInfoCache; } }
		public DataLabelInfoCache DataLabelInfoCache { get { return dataLabelInfoCache; } }
		public ErrorBarsInfoCache ErrorBarsInfoCache { get { return errorBarsInfoCache; } }
		public TrendlineInfoCache TrendlineInfoCache { get { return trendlineInfoCache; } }
		public GradientFillInfoCache GradientFillInfoCache { get { return gradientFillInfoCache; } }
		public GradientStopInfoCache GradientStopInfoCache { get { return gradientStopInfoCache; } }
		public SparklineGroupInfoCache SparklineGroupInfoCache { get { return sparklineGroupInfoCache; } }
		public PivotTableCache PivotTableCache { get { return pivotTableCache; } }
		public PivotFieldCache PivotFieldCache { get { return pivotFieldCache; } }
		public IgnoredErrorInfoCache IgnoredErrorInfoCache { get { return ignoredErrorInfoCache; } }
		public CommonDrawingLocksInfoCache CommonDrawingLocksInfoCache { get { return commonDrawingLocksInfoCache; } }
		public GroupShapeInfoCache GroupShapeInfoCache { get { return groupShapeInfoCache; } }
		public FontKeyCache FontKeyCache { get { return fontKeyCache; } }
		#endregion
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
			}
			isDisposed = true;
		}
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		#region Initialize
		protected virtual void Initialize(DocumentModel documentModel) {
			DocumentModelUnitConverter unitConverter = documentModel.UnitConverter;
			cellAlignmentInfoCache = new CellAlignmentInfoCache(unitConverter);
			colorModelInfoCache = new ColorModelInfoCache(unitConverter);
			borderInfoCache = new BorderInfoCache(unitConverter);
			fontInfoCache = new RunFontInfoCache(unitConverter);
			fillInfoCache = new FillInfoCache(unitConverter);
			numberFormatCache = new NumberFormatCollection(unitConverter);
			cellFormatCache = new CellFormatCache(unitConverter, documentModel);
			marginInfoCache = new MarginsInfoCache(unitConverter);
			printSetupInfoCache = new PrintSetupInfoCache(unitConverter);
			groupAndOutlinePropertiesInfoCache = new GroupAndOutlinePropertiesInfoCache(unitConverter);
			autoFilterColumnInfoCache = new AutoFilterColumnInfoCache(unitConverter);
			sortStateInfoCache = new SortStateInfoCache(unitConverter);
			sheetFormatInfoCache = new SheetFormatInfoCache(unitConverter);
			sortConditionInfoCache = new SortConditionInfoCache(unitConverter);
			tableInfoCache = new TableInfoCache(unitConverter);
			vmlShapeInfoCache = new VmlShapeInfoCache(unitConverter);
			dateGroupingInfoCache = new DateGroupingInfoCache(unitConverter);
			windowInfoCache = new WorkbookWindowInfoCache(unitConverter);
			worksheetProtectionInfoCache = new WorksheetProtectionInfoCache(unitConverter);
			dataValidationInfoCache = new DataValidationInfoCache(unitConverter);
			calculationOptionsInfoCache = new CalculationOptionsInfoCache(unitConverter);
			tableStyleElementFormatCache = new TableStyleElementFormatCache(unitConverter, documentModel);
			conditionalFormattingValueInfoCache = new ConditionalFormattingValueInfoCache(unitConverter);
			conditionalFormattingInfoCache = new ConditionalFormattingInfoCache(unitConverter);
			drawingObjectInfoCache = new DrawingObjectInfoCache(unitConverter);
			pictureInfoCache = new PictureInfoCache(unitConverter);
			modelShapeInfoCache = new ModelShapeInfoCache(unitConverter);
			transform2DInfoCache = new Transform2DInfoCache(unitConverter);
			shapeStyleInfoCache = new ShapeStyleInfoCache(unitConverter);
			shapePropertiesInfoCache = new ShapePropertiesInfoCache(unitConverter);
			headeFooterInfoCache = new HeaderFooterInfoCache(unitConverter);
			chartInfoCache = new ChartInfoCache(unitConverter);
			view3DInfoCache = new View3DInfoCache(unitConverter);
			pictureOptionsInfoCache = new PictureOptionsInfoCache(unitConverter);
			dataTableInfoCache = new DataTableInfoCache(unitConverter);
			axisInfoCache = new AxisInfoCache(unitConverter);
			scalingInfoCache = new ScalingInfoCache(unitConverter);
			displayUnitInfoCache = new DisplayUnitInfoCache(unitConverter);
			chartViewInfoCache = new ChartViewInfoCache(unitConverter);
			dataLabelInfoCache = new DataLabelInfoCache(unitConverter);
			errorBarsInfoCache = new ErrorBarsInfoCache(unitConverter);
			trendlineInfoCache = new TrendlineInfoCache(unitConverter);
			gradientFillInfoCache = new GradientFillInfoCache(unitConverter);
			gradientStopInfoCache = new GradientStopInfoCache(unitConverter);
			sparklineGroupInfoCache = new SparklineGroupInfoCache(unitConverter);
			pivotTableCache = new PivotTableCache(unitConverter);
			pivotFieldCache = new PivotFieldCache(unitConverter);
			ignoredErrorInfoCache = new IgnoredErrorInfoCache(unitConverter);
			commonDrawingLocksInfoCache = new CommonDrawingLocksInfoCache(unitConverter);
			groupShapeInfoCache = new GroupShapeInfoCache(unitConverter);
			fontKeyCache = new FontKeyCache(unitConverter);
		}
		#endregion
		public List<SizeOfInfo> GetSizeOfInfo() {
			List<SizeOfInfo> result = ObjectSizeHelper.CalculateSizeOfInfo(this);
			result.Insert(0, ObjectSizeHelper.CalculateTotalSizeOfInfo(result, "DocumentModel.Cache Total"));
			return result;
		}
	}
	#endregion
}
