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
using System.ComponentModel;
using System.Linq;
using DevExpress.DataAccess;
using DevExpress.DataAccess.EntityFramework;
using DevExpress.DataAccess.ObjectBinding;
using DevExpress.DataAccess.Sql;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Reports.UserDesigner.ReportModel;
using DevExpress.Xpf.Reports.UserDesigner.XRDiagram;
using DevExpress.XtraCharts;
using DevExpress.XtraReports.UI;
namespace DevExpress.Xpf.Reports.UserDesigner.Native {
	public class StandardModelsRegistry : IModelRegistry<XRControlModelFactory>, IModelRegistry<XRChartElementModelFactory> {
		void IModelRegistry<XRControlModelFactory>.Register(XRControlModelFactory factory) {
			factory.Registry<XRControl>().Register(x => XRControlModel.New(x, null));
			factory.Registry<Band>().Register(x => new BandModel(x));
			factory.Registry<XRChart>().Register(x => new XRChartModel(x, DXImageHelper.GetImageSource(@"Images/Toolbox Items/Chart2_16x16.png")));
			factory.Registry<XRTable>().Register(x => new XRTableModel(x, DXImageHelper.GetImageSource(@"Images/Toolbox Items/Table_16x16.png")));
			factory.Registry<XRTableRow>().Register(x => new XRTableRowModel(x));
			factory.Registry<XRTableCell>().Register(x => new XRTableCellModel(x));
			factory.Registry<XRTableOfContents>().Register(x => new XRTableOfContentsModel(x, DXImageHelper.GetImageSource(@"Images/Toolbox Items/TableOfContent_16x16.png")));
			factory.Registry<XRPictureBox>().Register(x => new XRPictureBoxModel(x, DXImageHelper.GetImageSource(@"Images/Toolbox Items/PictureBox_16x16.png")));
			factory.Registry<XRBarCode>().Register(x => new XRBarCodeModel(x, DXImageHelper.GetImageSource(@"Images/Toolbox Items/Barcode2_16x16.png")));
			factory.Registry<XRSparkline>().Register(x => new XRSparklineModel(x, DXImageHelper.GetImageSource(@"Images/Toolbox Items/Sparkline_16x16.png")));
			factory.Registry<XRShape>().Register(x => new XRShapeModel(x, DXImageHelper.GetImageSource(@"Images/Toolbox Items/Shape_16x16.png")));
			factory.Registry<XRRichText>().Register(x => new XRRichTextModel(x, DXImageHelper.GetImageSource(@"Images/Toolbox Items/RichText_16x16.png")));
			factory.Registry<XRSubreport>().Register(x => XRControlModel<XRSubreportDiagramItem>.New(x, DXImageHelper.GetImageSource(@"Images/Toolbox Items/Subreport_16x16.png")));
			factory.Registry<XRPageBreak>().Register(x => new XRPageBreakModel(x, DXImageHelper.GetImageSource(@"Images/Toolbox Items/PageBreak_16x16.png")));
			factory.Registry<XRPanel>().Register(x => XRContainerModel<XRControlModelBase, XRPanelDiagramItem>.New(x, DXImageHelper.GetImageSource(@"Images/Toolbox Items/Panel_16x16.png")));
			factory.Registry<XRLabel>().Register(x => XRTextModel<XRLabelDiagramItem>.New(x, DXImageHelper.GetImageSource(@"Images/Toolbox Items/Label_16x16.png")));
			factory.Registry<XRCheckBox>().Register(x => XRTextModel<XRCheckBoxDiagramItem>.New(x, DXImageHelper.GetImageSource(@"Images/Toolbox Items/CheckBox2_16x16.png")));
			factory.Registry<XRLine>().Register(x => XRControlModel<XRLineDiagramItem>.New(x, DXImageHelper.GetImageSource(@"Images/Toolbox Items/LineItem_16x16.png")));
			factory.Registry<XRZipCode>().Register(x => XRTextModel<XRZipCodeDiagramItem>.New(x, DXImageHelper.GetImageSource(@"Images/Toolbox Items/ZipCode_16x16.png")));
			factory.Registry<XRGauge>().Register(x => XRControlModel<XRGaugeDiagramItem>.New(x, XRControlModelBase.GetDefaultIcon("XRGauge")));
			factory.Registry<XRPivotGrid>().Register(x => XRControlModel<XRPivotGridDiagramItem>.New(x, DXImageHelper.GetImageSource(@"Images/Toolbox Items/PivotGrid_16x16.png")));
			factory.Registry<XRPageInfo>().Register(x => XRControlModel<XRPageInfoDiagramItem>.New(x, DXImageHelper.GetImageSource(@"Images/Toolbox Items/PageInfo_16x16.png")));
			factory.Registry<XRCrossBandLine>().Register(x => XRCrossBandControlModel<XRCrossBandLineDiagramItem>.New(x, DXImageHelper.GetImageSource(@"Images/Toolbox Items/CrossbandLine_16x16.png")));
			factory.Registry<XRCrossBandBox>().Register(x => XRCrossBandControlModel<XRCrossBandBoxDiagramItem>.New(x, DXImageHelper.GetImageSource(@"Images/Toolbox Items/CrossbandBox_16x16.png")));
			factory.Registry<IComponent>().Register(x => XRComponentModel.New(x, XRComponentModelBase.GetDefaultIcon("Component")));
			factory.Registry<FormattingRule>().Register(x => XRComponentModel<XRFormattingRuleDiagramItem>.New(x, XRComponentModelBase.GetDefaultIcon("FormattingRule")));
			factory.Registry<XRControlStyle>().Register(x => XRComponentModel<XRControlStyleDiagramItem>.New(x, XRComponentModelBase.GetDefaultIcon("ControlStyle")));
			factory.Registry<CalculatedField>().Register(x => XRComponentModel<CalculatedFieldDiagramItem>.New(x, XRComponentModelBase.GetDefaultIcon("Component")));
			factory.Registry<DevExpress.XtraReports.Parameters.Parameter>().Register(x => XRComponentModel<ParameterDiagramItem>.New(x, XRComponentModelBase.GetDefaultIcon("Component")));
			factory.Registry<SqlDataSource>().Register(x => XRComponentModel.New(x, XRComponentModelBase.GetDefaultIcon("SqlDataSource")));
			factory.Registry<EFDataSource>().Register(x => XRComponentModel.New(x, XRComponentModelBase.GetDefaultIcon("EFDataSource")));
			factory.Registry<ObjectDataSource>().Register(x => XRComponentModel.New(x, XRComponentModelBase.GetDefaultIcon("ObjectDataSource")));
		}
		void IModelRegistry<XRChartElementModelFactory>.Register(XRChartElementModelFactory factory) {
			factory.Registry<AxisX>().Register(x => XRChartElementModel.New(x));
			factory.Registry<AxisY>().Register(x => XRChartElementModel.New(x));
			factory.Registry<SeriesLabelBase>().Register(x => XRChartElementModel.New(x));
			factory.Registry<Series>().Register(x => new XRChartSeries(x));
			factory.Registry<XtraCharts.Diagram>().Register(x => new XRChartDiagramModel(x));
			factory.Registry<XYDiagram>().Register(x => new XRChartXYDiagramModel(x));
			factory.Registry<ChartTitle>().Register(x => XRChartElementModel<XRChartTitleDiagramItem>.New(x));
		}
	}
}
