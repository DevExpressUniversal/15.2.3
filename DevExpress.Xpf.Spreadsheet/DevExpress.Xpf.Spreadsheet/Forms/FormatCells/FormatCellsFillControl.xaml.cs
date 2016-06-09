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

using DevExpress.Utils;
using DevExpress.Xpf.Spreadsheet.Internal;
using DevExpress.Export.Xl;
using DevExpress.XtraSpreadsheet.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
namespace DevExpress.Xpf.Spreadsheet.Forms {
	public partial class FormatCellsFillControl : UserControl {
		#region Fields
		#endregion
		public FormatCellsFillControl() {
			InitializeComponent();
		}
		void OnBackColorChanged(object sender, RoutedEventArgs e) {
			FormatCellsViewModel ViewModel = DataContext as FormatCellsViewModel;
			if (ViewModel.PatternType.HasValue && ViewModel.PatternType == XlPatternType.None)
				ViewModel.PatternType = XlPatternType.Solid;
		}
		protected override void OnRender(DrawingContext dc) {
			base.OnRender(dc);
			FormatCellsViewModel viewModel = DataContext as FormatCellsViewModel;
			preview.Clip = new RectangleGeometry(new Rect(GetPosition(preview), new Size(preview.ActualWidth, preview.ActualHeight)));
			Rect fillBounds = preview.Clip.Bounds;
			preview.ClipToBounds = true;
			if (viewModel.FillForeColor.HasValue && viewModel.FillBackColor.HasValue && !DXColor.IsTransparentOrEmpty(viewModel.FillBackColor.Value) && viewModel.PatternType.HasValue && viewModel.PatternType != XlPatternType.None)
				dc.DrawRectangle(new SolidColorBrush(ToMediaColor(viewModel.FillBackColor)), null, fillBounds);
			if (viewModel.FillForeColor.HasValue && viewModel.FillBackColor.HasValue && viewModel.PatternType.HasValue && viewModel.PatternType != XlPatternType.Solid && viewModel.PatternType != XlPatternType.None) {
				Brush brush = PatternFillProvider.GetPatternFillBrush(viewModel.PatternType.Value, ToMediaColor(viewModel.FillForeColor));
				dc.DrawRectangle(brush, null, fillBounds);
			}
		}
		Point GetPosition(Visual element) {
			var positionTransform = element.TransformToAncestor(this);
			var areaPosition = positionTransform.Transform(new Point(0, 0));
			return areaPosition;
		}
		Color ToMediaColor(System.Drawing.Color? color) {
			if (color.GetValueOrDefault().IsEmpty)
				return Color.FromArgb(System.Drawing.Color.Black.A, System.Drawing.Color.Black.R, System.Drawing.Color.Black.G, System.Drawing.Color.Black.B);
			return Color.FromArgb(color.Value.A, color.Value.R, color.Value.G, color.Value.B);
		}
		void backColor_TargetUpdated(object sender, DataTransferEventArgs e) {
			this.InvalidateVisual();
		}
		void backColor_SourceUpdated(object sender, DataTransferEventArgs e) {
			this.InvalidateVisual();
		}
		void patternColor_TargetUpdated(object sender, DataTransferEventArgs e) {
			this.InvalidateVisual();
		}
		void patternColor_SourceUpdated(object sender, DataTransferEventArgs e) {
			this.InvalidateVisual();
		}
		void patternStyle_TargetUpdated(object sender, DataTransferEventArgs e) {
			this.InvalidateVisual();
		}
		void patternStyle_SourceUpdated(object sender, DataTransferEventArgs e) {
			this.InvalidateVisual();
		}
	}
}
