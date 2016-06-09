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
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Forms;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Export.Xl;
namespace DevExpress.Xpf.Spreadsheet.Forms {
	public partial class FormatCellsBorderControl : UserControl {
		public FormatCellsBorderControl() {
			InitializeComponent();
		}
		private void NoneBordersButtonClick(object sender, RoutedEventArgs e) {
			ResetBorders();
		}
		private void ResetBorders() {
			FormatCellsViewModel viewModel = DataContext as FormatCellsViewModel;
			viewModel.IsNoBorder = true;
			viewModel.RightColor = DXColor.Empty;
			viewModel.TopColor = DXColor.Empty;
			viewModel.BottomColor = DXColor.Empty;
			viewModel.LeftColor = DXColor.Empty;
			viewModel.DiagonalColor = DXColor.Empty;
			viewModel.VerticalColor = DXColor.Empty;
			viewModel.HorizontalColor = DXColor.Empty;
			viewModel.LeftLineStyle = XlBorderLineStyle.None;
			viewModel.RightLineStyle = XlBorderLineStyle.None;
			viewModel.BottomLineStyle = XlBorderLineStyle.None;
			viewModel.TopLineStyle = XlBorderLineStyle.None;
			viewModel.DiagonalDownLineStyle = XlBorderLineStyle.None;
			viewModel.DiagonalUpLineStyle = XlBorderLineStyle.None;
			viewModel.VerticalLineStyle = XlBorderLineStyle.None;
			viewModel.HorizontalLineStyle = XlBorderLineStyle.None;
			viewModel.ApplyTopBorder = false;
			viewModel.ApplyBottomBorder = false;
			viewModel.ApplyLeftBorder = false;
			viewModel.ApplyRightBorder = false;
			viewModel.ApplyDiagonalDownBorder = false;
			viewModel.ApplyDiagonalUpBorder = false;
			viewModel.ApplyVerticalBorder = false;
			viewModel.ApplyHorizontalBorder = false;
			viewModel.IsNoBorder = false;
		}
		private void OutlineBordersButtonClick(object sender, RoutedEventArgs e) {
			SetOutlineBorders();
		}
		private void SetOutlineBorders() {
			FormatCellsViewModel viewModel = DataContext as FormatCellsViewModel;
			viewModel.IsOutline = true;
			viewModel.RightColor = viewModel.Color;
			viewModel.TopColor = viewModel.Color;
			viewModel.BottomColor = viewModel.Color;
			viewModel.LeftColor = viewModel.Color;
			viewModel.LeftLineStyle = viewModel.LineStyle;
			viewModel.RightLineStyle = viewModel.LineStyle;
			viewModel.BottomLineStyle = viewModel.LineStyle;
			viewModel.TopLineStyle = viewModel.LineStyle;
			viewModel.ApplyTopBorder = true;
			viewModel.ApplyBottomBorder = true;
			viewModel.ApplyLeftBorder = true;
			viewModel.ApplyRightBorder = true;
			viewModel.IsOutline = false;
		}
		private void InsideBorderButtonClick(object sender, RoutedEventArgs e) {
			FormatCellsViewModel viewModel = DataContext as FormatCellsViewModel;
			viewModel.IsInside = true;
			viewModel.VerticalColor = viewModel.Color;
			viewModel.HorizontalColor = viewModel.Color;
			viewModel.VerticalLineStyle = viewModel.LineStyle;
			viewModel.HorizontalLineStyle = viewModel.LineStyle;
			viewModel.ApplyVerticalBorder = true;
			viewModel.ApplyHorizontalBorder = true;
			viewModel.IsInside = false;
		}
	}
}
