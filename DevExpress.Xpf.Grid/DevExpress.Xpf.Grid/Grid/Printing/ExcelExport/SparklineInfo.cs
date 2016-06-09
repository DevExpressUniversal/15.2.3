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

using System.Windows.Media;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.XtraExport.Helpers;
namespace DevExpress.Xpf.Grid.Printing {
	public class SparklineInfo : ISparklineInfo {
		readonly SparklineEditSettings EditSettings;
		public SparklineInfo(SparklineEditSettings editSettings) {
			this.EditSettings = editSettings;
		}
		#region StyleSettings
		SparklineStyleSettings _styleSettings;
		SparklineStyleSettings StyleSettings {
			get {
				if(_styleSettings == null) {
					_styleSettings = (SparklineStyleSettings)EditSettings.StyleSettings;
					if(_styleSettings == null)
						_styleSettings = new EmptySparklineStyleSettings();
				}
				return _styleSettings;
			}
		}
		EmptySparklineStyleSettings EmptySparkSettings { get { return StyleSettings as EmptySparklineStyleSettings; } }
		LineSparklineStyleSettings LineSparkSettings { get { return StyleSettings as LineSparklineStyleSettings; } }	 
		BarSparklineStyleSettings BarSparkSettings { get { return StyleSettings as BarSparklineStyleSettings; } }
		WinLossSparklineStyleSettings WinLossSparkSettings { get { return StyleSettings as WinLossSparklineStyleSettings; } }
		#endregion
		SparklineViewType? _viewType;
		SparklineViewType? ViewType {
			get {
				if(_viewType == null)
					_viewType = GetViewType();
				return _viewType;
			}
		}	
		#region ISparklineInfo Members
		public System.Drawing.Color ColorFirst {
			get { return AppearanceHelper.BrushToColor(StyleSettings.StartPointBrush); }
		}
		public System.Drawing.Color ColorHigh {
			get { return AppearanceHelper.BrushToColor(StyleSettings.MaxPointBrush); }
		}
		public System.Drawing.Color ColorLast {
			get { return AppearanceHelper.BrushToColor(StyleSettings.EndPointBrush); }
		}
		public System.Drawing.Color ColorLow {
			get { return AppearanceHelper.BrushToColor(StyleSettings.MinPointBrush); }
		}
		public System.Drawing.Color ColorMarker {
			get { return GetMarkerColor(); }
		}
		public System.Drawing.Color ColorNegative {
			get { return AppearanceHelper.BrushToColor(StyleSettings.NegativePointBrush); }
		}
		public System.Drawing.Color ColorSeries {
			get { return AppearanceHelper.BrushToColor(StyleSettings.Brush); }
		}
		public bool DisplayMarkers {
			get { return GetDisplayMarker(); }
		}
		public bool HighlightFirst {
			get { return StyleSettings.HighlightStartPoint; }
		}
		public bool HighlightHighest {
			get { return StyleSettings.HighlightMaxPoint; }
		}
		public bool HighlightLast {
			get { return StyleSettings.HighlightEndPoint; }
		}
		public bool HighlightLowest {
			get { return StyleSettings.HighlightMinPoint; }
		}
		public bool HighlightNegative {
			get { return GetHighlightNegative(); }
		}
		public double LineWeight {
			get { return GetLineWeight(); }
		}
		public DevExpress.Data.ColumnSortOrder PointSortOrder {
			get {
				switch(EditSettings.PointArgumentSortOrder) {
					case SparklineSortOrder.Ascending:
						return DevExpress.Data.ColumnSortOrder.Ascending;
					case SparklineSortOrder.Descending:
						return DevExpress.Data.ColumnSortOrder.Descending;
					default:
						return DevExpress.Data.ColumnSortOrder.None;
				}
			}
		}
		public Export.Xl.XlSparklineType SparklineType {
			get {
				if(!ViewType.HasValue)
					return Export.Xl.XlSparklineType.Line;
				switch(ViewType.Value) {
					case SparklineViewType.Area:
					case SparklineViewType.Line:
						return Export.Xl.XlSparklineType.Line;
					case SparklineViewType.Bar:
						return Export.Xl.XlSparklineType.Column;
					case SparklineViewType.WinLoss:
						return Export.Xl.XlSparklineType.WinLoss;
					default:
						return Export.Xl.XlSparklineType.Line;
				}
			}
		} 
		#endregion
		SparklineViewType? GetViewType() {
			if(StyleSettings is AreaSparklineStyleSettings)
				return SparklineViewType.Area;
			if(StyleSettings is LineSparklineStyleSettings || StyleSettings is EmptySparklineStyleSettings)
				return SparklineViewType.Line;		 
			if(StyleSettings is BarSparklineStyleSettings)
				return SparklineViewType.Bar;
			if(StyleSettings is WinLossSparklineStyleSettings)
				return SparklineViewType.WinLoss;
			return null;
		}
		System.Drawing.Color GetMarkerColor() {
			return AppearanceHelper.BrushToColor(GetMarkerBrush());
		}
		Brush GetMarkerBrush() {
			if(!ViewType.HasValue)
				return null;
			switch(ViewType.Value) {
				case SparklineViewType.Area:		   
				case SparklineViewType.Line:
					return LineSparkSettings != null ? LineSparkSettings.MarkerBrush : null;
				case SparklineViewType.Bar:
				case SparklineViewType.WinLoss:
				default:
					return null;
			}
		}
		bool GetDisplayMarker() {
			if(!ViewType.HasValue)
				return false;
			switch(ViewType.Value) {
				case SparklineViewType.Area:				 
				case SparklineViewType.Line:
					return LineSparkSettings != null ? LineSparkSettings.ShowMarkers : false;
				case SparklineViewType.Bar:					
				case SparklineViewType.WinLoss:
				default:
					return false;
			}
		}
		bool GetHighlightNegative() {
			if(!ViewType.HasValue)
				return false;
			switch(ViewType.Value) {
				case SparklineViewType.Area:									  
				case SparklineViewType.Line:
					return  LineSparkSettings != null ? LineSparkSettings.HighlightNegativePoints : false;
				case SparklineViewType.Bar:
					return BarSparkSettings.HighlightNegativePoints;
				case SparklineViewType.WinLoss:					
				default:
					return false;
			}
		}
		double GetLineWeight() {
			if(!ViewType.HasValue)
				return 1;
			switch(ViewType.Value) {
				case SparklineViewType.Area:			   
				case SparklineViewType.Line:
					return LineSparkSettings != null ? LineSparkSettings.LineWidth : 1;
				case SparklineViewType.Bar:					
				case SparklineViewType.WinLoss:					
				default:
					return 1;
			}
		}
	}
	class EmptySparklineStyleSettings : SparklineStyleSettings {
		protected override SparklineViewType ViewType {
			get { return SparklineViewType.Line; }
		}
	}
}
