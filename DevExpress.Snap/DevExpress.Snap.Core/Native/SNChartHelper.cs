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

using DevExpress.Snap.Core.Fields;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
namespace DevExpress.Snap.Core.Native {
	public static class SNChartHelper {
		public static string SaveChartLayoutAsString(SNChart chart) { 
			return StreamSerializeHelper.ToBase64String(stream => chart.SaveLayout(stream));
		}
		public static string SaveChartSeriesDataBindingsAsString(SNChart chart) { 
			return StreamSerializeHelper.ToBase64String(chart.GetSeriesDataBindings());
		}
		public static byte[] SaveChartLayoutAsByteArray(SNChart chart) {
			return StreamSerializeHelper.ToByteArray(stream => chart.SaveLayout(stream));
		}
		public static byte[] SaveChartSeriesDataBindingsAsByteArray(SNChart chart) {
			return StreamSerializeHelper.ToByteArray(chart.GetSeriesDataBindings());
		}
		public static void LoadChartLayout(SNChart chart, Base64StringDataContainer layout) {
			StreamSerializeHelper.FromBase64StringDataContainer(layout, stream => chart.LoadLayout(stream));
		}
		public static void LoadChartSeriesDataBindings(SNChart chart, Base64StringDataContainer bindings) {
			chart.SetSeriesDataBindings(StreamSerializeHelper.FromBase64StringDataContainer<SeriesDataBindingList>(bindings));
		}
		public static void SaveChartField(SNChartField chartField, PieceTable pieceTable, Field field, SNChart chart) {
			SnapDocumentModel model = (SnapDocumentModel)pieceTable.DocumentModel;
			model.BeginUpdate();
			try {
				using (InstructionController controller = new InstructionController(pieceTable, chartField, field)) {
					DocumentModel layout = CreateDocumentModel(SNChartHelper.SaveChartLayoutAsByteArray(chart), model);
					controller.SetSwitch(SNChartField.ChartLayoutSwitch, layout, false);
					DocumentModel bindings = CreateDocumentModel(SNChartHelper.SaveChartSeriesDataBindingsAsByteArray(chart), model);
					controller.SetSwitch(SNChartField.ChartSeriesDataBindingsSwitch, bindings, false);
					string chartDataSourceName = chart.GetChartDataBindingName();
					if(String.IsNullOrEmpty(chartDataSourceName))
						controller.RemoveSwitch(SNChartField.ChartDataBindingsSwitch);
					else
						controller.SetSwitch(SNChartField.ChartDataBindingsSwitch, chartDataSourceName);
				}
			}
			finally {
				model.EndUpdate();
			}
		}
		public static DocumentModel CreateChartLayout(SNChart chart, DocumentModel source) {
			return CreateDocumentModel(SaveChartLayoutAsByteArray(chart), (SnapDocumentModel)source);
		}
		public static DocumentModel CreateChartSeriesDataBindings(SNChart chart, DocumentModel source) {
			return CreateDocumentModel(SaveChartSeriesDataBindingsAsByteArray(chart), (SnapDocumentModel)source);
		}
		public static DocumentModel CreateDocumentModel(byte[] content, SnapDocumentModel source) {
			DocumentModel result = source.CreateNew();
			result.IntermediateModel = true;
			result.BeginSetContent();
			try {
				result.InheritDataServices(source);
				Base64StringDataContainer container = new Base64StringDataContainer();
				container.SetData(content);
				result.MainPieceTable.InsertDataContainerRun(DocumentLogPosition.Zero, container, false);
			}
			finally {
				result.EndSetContent(DocumentModelChangeType.LoadNewDocument, false, null);
			}
			return result;
		}
	}
}
