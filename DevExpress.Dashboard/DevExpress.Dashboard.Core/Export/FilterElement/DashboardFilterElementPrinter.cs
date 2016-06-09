#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Printing;
using DevExpress.DashboardCommon.Service;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Utils;
using DevExpress.XtraPrinting;
namespace DevExpress.DashboardExport {
	public class DashboardFilterElementPrinter : DashboardExportPrinter  {
		readonly RectangleF rectangle;
		readonly FilterElementDashboardItemViewControl viewControl;
		readonly ExportFilterElementControl exportControl = new ExportFilterElementControl();
		readonly DashboardFontInfo fontInfo;
		public DashboardFilterElementPrinter(DashboardItemServerData serverData, ItemViewerClientState clientState, DashboardFontInfo fontInfo) {
			this.viewControl = new FilterElementDashboardItemViewControl(exportControl);
			FilterElementDashboardItemViewModel viewModel = (FilterElementDashboardItemViewModel)serverData.ViewModel;
			MultiDimensionalData mddata = CreateMultiDimensionalData(serverData);
			viewControl.Update(viewModel, mddata);
			SetSelection(serverData.SelectedValues, mddata);
			this.rectangle = new RectangleF(new Point(0, 0), new Size(clientState.ViewerArea.Width, clientState.ViewerArea.Height));
			this.fontInfo = fontInfo;
		}
		protected override void CreateDetail(IBrickGraphics graph) {
			ITextBrick textBrick = PS.CreateTextBrick();
			textBrick.BorderWidth = 0;
			textBrick.Style.StringFormat = new BrickStringFormat(textBrick.StringFormat, StringTrimming.EllipsisCharacter);
			textBrick.Style.StringFormat = new BrickStringFormat(textBrick.StringFormat, textBrick.Style.StringFormat.FormatFlags | StringFormatFlags.NoWrap);
			Font font = DevExpress.DashboardCommon.Printing.FontHelper.GetFont(AppearanceObject.DefaultFont, fontInfo);
			textBrick.Font = font;
			FillBrickText(textBrick);
			graph.DrawBrick(textBrick, rectangle);
		}
		void FillBrickText(ITextBrick textBrick) {
			IList<string> lines = new List<string>();
			IEnumerable<object> selection = viewControl.GetExportTexts();
			foreach(IEnumerable<string> line in selection)
				lines.Add(String.Join(", ", line));
			textBrick.Text = exportControl.BuildText(lines);
		}
		MultiDimensionalData CreateMultiDimensionalData(DashboardItemServerData serverData) {
			HierarchicalMetadata metaData = serverData.Metadata;
			ClientHierarchicalMetadata metadata = new ClientHierarchicalMetadata(serverData.Metadata);
			return new MultiDimensionalData(serverData.MultiDimensionalData.HierarchicalDataParams, metadata);
		}
		void SetSelection(IList selection, MultiDimensionalData data) {
			if(selection != null && selection.Count > 0) {
				IList<AxisPointTuple> selectedTuples = new List<AxisPointTuple>();
				foreach(IList<object> selectedValue in selection) {
					IList<AxisPoint> axisPoints = new List<AxisPoint>();
					axisPoints.Add(data.GetAxisPointByUniqueValues(DashboardDataAxisNames.DefaultAxis, DataUtils.CheckOlapNullValues(selectedValue).ToArray()));
					selectedTuples.Add(data.CreateTuple(axisPoints));
				}
				viewControl.SetSelection(selectedTuples);
			}
		}
	}
}
