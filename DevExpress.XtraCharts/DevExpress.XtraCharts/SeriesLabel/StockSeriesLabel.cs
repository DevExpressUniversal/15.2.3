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
using System.Drawing;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using DevExpress.Charts.Native;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(StockSeriesLabelTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class StockSeriesLabel : SeriesLabelBase {
		protected internal override bool ShadowSupported { get { return true; } }
		protected internal override bool ConnectorSupported { get { return false; } }
		protected internal override bool ConnectorEnabled { get { return false; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public new int LineLength { get { return 0; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public new bool LineVisible { get { return false; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public new Color LineColor { get { return Color.Empty; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public new LineStyle LineStyle { get { return null; } }
		public StockSeriesLabel() : base() {
		}
		protected override ChartElement CreateObjectForClone() {
			return new StockSeriesLabel();
		}
		protected internal override void CalculateLayout(SeriesLabelLayoutList labelLayoutList, RefinedPointData pointData, TextMeasurer textMeasurer) {
			XYDiagramSeriesLabelLayoutList xyLabelLayoutList = labelLayoutList as XYDiagramSeriesLabelLayoutList;
			IFinancialPoint financialPoint = pointData.RefinedPoint;
			if (xyLabelLayoutList == null || financialPoint == null)
				return;			
			XYDiagramMappingBase mapping = xyLabelLayoutList.GetMapping(financialPoint.Argument, financialPoint.High);
			if (mapping == null)
				return;
			DiagramPoint anchorPoint = mapping.GetScreenPoint(financialPoint.Argument, financialPoint.High);
			SeriesLabelViewData labelViewData = pointData.LabelViewData[0];
			TextPainter painter = labelViewData.CreateTextPainterForFlankDrawing(this, textMeasurer, anchorPoint, 
				XYDiagramMappingHelper.CorrectAngle(mapping, Math.PI / 2));
			XYDiagramSeriesLabelLayout layout = XYDiagramSeriesLabelLayout.Create(pointData, 
				pointData.DrawOptions.Color, painter, null, ResolveOverlappingMode, anchorPoint);
			xyLabelLayoutList.Add(layout);
		}
	}
}
