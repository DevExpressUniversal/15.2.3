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

using DevExpress.Xpf.Printing.Native;
using DevExpress.XtraPrinting.Drawing;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.XamlExport;
using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace DevExpress.Xpf.Reports.UserDesigner.Native.ReportExtension {
	public static class ReportWatermarkPreviewHelper {
		public static FrameworkElement GetReportWatermarkPreview(this XtraReport report, Watermark watermark) {
			var page = new PSPage(new ReadonlyPageData(report.Margins, new Margins(), report.PaperKind, report.PageSize, report.Landscape));
			var copy = new Watermark();
			copy.CopyFrom(watermark);
			page.AssignWatermark(copy);
			Stream stream = new BrickPageVisualizer(TextMeasurementSystem.NativeXpf).SaveToStream(page, 0, 0);
			var canvas = (Canvas)XamlReaderHelper.Load(stream);
			System.Windows.Controls.Grid grid = new System.Windows.Controls.Grid();
			grid.Height = report.PageSize.Height;
			grid.Width = report.PageSize.Width;
			var gridBorder = new Border();
			gridBorder.BorderBrush = Brushes.Gray;
			gridBorder.Margin = new Thickness(-12, -12, -12, -12);
			gridBorder.BorderThickness = new Thickness(12);
			grid.Children.Add(gridBorder);
			if(canvas.Children.Count == 0) return grid;
			TextBlock textBlock = null;
			Image image = null;
			foreach(Border border in canvas.Children) {
				if(textBlock == null && (textBlock = border.Child as TextBlock) != null)
					border.Child = null;
				if(image == null && (image = border.Child as Image) != null)
					border.Child = null;
			}
			if(image != null) {
				if(watermark.ImageViewMode != ImageViewMode.Clip || watermark.ImageTiling) {
					image.Width = double.NaN;
					image.Height = double.NaN;
				}
				image.Margin = new Thickness(0, 0, 0, 0);
				grid.Children.Add(image);
			}
			if(textBlock != null)
				grid.Children.Add(textBlock);
			return grid;
		}
	}
}
