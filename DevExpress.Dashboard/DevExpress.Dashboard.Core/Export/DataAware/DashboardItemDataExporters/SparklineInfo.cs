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
using System.Collections.Generic;
using System.Drawing;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Data;
using DevExpress.Export.Xl;
using DevExpress.Skins;
using DevExpress.XtraExport.Helpers;
namespace DevExpress.DashboardExport {
	public class SparklineInfo : ISparklineInfo {
		readonly SparklineOptionsViewModel viewModel;
		public Color ColorFirst { get; private set; }
		public Color ColorLast { get; private set; }
		public Color ColorHigh { get; private set; }
		public Color ColorLow { get; private set; }
		public Color ColorNegative { get; private set; }
		public Color ColorMarker { get; private set; }
		public Color ColorSeries { get; private set; }
		public SparklineInfo(SparklineOptionsViewModel viewModel) {
			this.viewModel = viewModel;
			Skin skin = SparklineSkins.GetSkin(null);
			ColorFirst = skin.Colors.GetColor(SparklineSkins.ColorStartPoint);
			ColorLast = skin.Colors.GetColor(SparklineSkins.ColorEndPoint);
			ColorHigh = skin.Colors.GetColor(SparklineSkins.ColorMaxPoint);
			ColorLow = skin.Colors.GetColor(SparklineSkins.ColorMinPoint);
			ColorNegative = skin.Colors.GetColor(SparklineSkins.ColorNegativePoint);
			ColorMarker = skin.Colors.GetColor(SparklineSkins.ColorMarker);
			ColorSeries = skin.Colors.GetColor(SparklineSkins.Color);
		}
		double ISparklineInfo.LineWeight { get { return 1; } }
		bool ISparklineInfo.DisplayMarkers { get { return false; } }
		bool ISparklineInfo.HighlightNegative { get { return false; } }
		ColumnSortOrder ISparklineInfo.PointSortOrder { get { return ColumnSortOrder.None; } }
		bool ISparklineInfo.HighlightHighest { get { return viewModel.HighlightMinMaxPoints; } }
		bool ISparklineInfo.HighlightLowest { get { return viewModel.HighlightMinMaxPoints; } }
		bool ISparklineInfo.HighlightFirst { get { return viewModel.HighlightStartEndPoints; } }
		bool ISparklineInfo.HighlightLast { get { return viewModel.HighlightStartEndPoints; } }
		XlSparklineType ISparklineInfo.SparklineType {
			get {
				switch(viewModel.ViewType) {
					case SparklineViewType.Bar:
						return XlSparklineType.Column;
					case SparklineViewType.WinLoss:
						return XlSparklineType.WinLoss;
					default:
						return XlSparklineType.Line;
				}
			}
		}
	}
}
