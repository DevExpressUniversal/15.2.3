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
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Office;
using System.Globalization;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		#region DataLabelShared
		void GenerateDataLabelShared(DataLabelBase label) {
			GenerateNumberFormatContent(label.NumberFormat);
			GenerateChartShapeProperties(label.ShapeProperties);
			GenerateTextPropertiesContent(label.TextProperties);
			if (label.LabelPosition != DataLabelPosition.Default)
				GenerateChartSimpleStringAttributeTag("dLblPos", DataLabelPositionTable[label.LabelPosition]);
			GenerateChartSimpleBoolAttributeTag("showLegendKey", label.ShowLegendKey);
			GenerateChartSimpleBoolAttributeTag("showVal", label.ShowValue);
			GenerateChartSimpleBoolAttributeTag("showCatName", label.ShowCategoryName);
			GenerateChartSimpleBoolAttributeTag("showSerName", label.ShowSeriesName);
			GenerateChartSimpleBoolAttributeTag("showPercent", label.ShowPercent);
			GenerateChartSimpleBoolAttributeTag("showBubbleSize", label.ShowBubbleSize);
			if (label.Separator != DataLabelBase.DefaultSeparator)
				GenerateChartSimpleStringTag("separator", label.Separator);
		}
		#endregion
		#region DataLabels
		protected internal void GenerateDataLabels(DataLabels dataLabels) {
			if (!dataLabels.Apply)
				return;
			WriteStartElement("dLbls", DrawingMLChartNamespace);
			try {
				foreach (DataLabel label in dataLabels.Labels)
					GenerateDataLabel(label);
				if (dataLabels.Delete)
					GenerateChartSimpleBoolAttributeTag("delete", true);
				else {
					GenerateDataLabelShared(dataLabels);
					if (!dataLabels.ShowLeaderLines)
						GenerateChartSimpleBoolAttributeTag("showLeaderLines", dataLabels.ShowLeaderLines);
					else
						GenerateLeaderLinesProperties(dataLabels.LeaderLinesProperties);
				}
			}
			finally {
				WriteEndElement();
			}
		}
		void GenerateLeaderLinesProperties(ShapeProperties properties) {
			if (!ShouldExportChartShapeProperties(properties))
				return;
			WriteStartElement("leaderLines", DrawingMLChartNamespace);
			try {
				GenerateChartShapeProperties(properties);
			}
			finally {
				WriteEndElement();
			}
		}
		#endregion
		#region DataLabel
		void GenerateDataLabel(DataLabel dataLabel) {
			WriteStartElement("dLbl", DrawingMLChartNamespace);
			try {
				GenerateChartSimpleIntAttributeTag("idx", dataLabel.ItemIndex);
				if (dataLabel.Delete)
					GenerateChartSimpleBoolAttributeTag("delete", true);
				else {
					GenerateChartLayoutContent(dataLabel.Layout);
					GenerateChartTextContent(dataLabel.Text);
					GenerateDataLabelShared(dataLabel);
				}
			}
			finally {
				WriteEndElement();
			}
		}
		#endregion
	}
}
