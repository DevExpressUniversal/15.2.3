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

namespace DevExpress.Snap.Core.API {
	using System;
	using System.Drawing;
	using DevExpress.Snap.Core.Fields;
	using DevExpress.Sparkline;
	public interface SnapSparkline : SnapSingleListItemEntity {
		string DataSourceName { get; set; }				 
		SparklineViewType ViewType { get; set; }			
		int Width { get; set; }							 
		int Height { get; set; }							
		Size Size { get; set; }							 
		bool HighlightMaxPoint { get; set; }				
		bool HighlightMinPoint { get; set; }				
		bool HighlightStartPoint { get; set; }			  
		bool HighlightEndPoint { get; set; }				
		Color Color { get; set; }						   
		Color MaxPointColor { get; set; }				   
		Color MinPointColor { get; set; }				   
		Color StartPointColor { get; set; }				 
		Color EndPointColor { get; set; }				   
		Color NegativePointColor { get; set; }			  
		int LineWidth { get; set; }						 
		bool HighlightNegativePoints { get; set; }		  
		bool ShowMarkers { get; set; }					  
		int MarkerSize { get; set; }						
		int MaxPointMarkerSize { get; set; }				
		int MinPointMarkerSize { get; set; }				
		int StartPointMarkerSize { get; set; }			  
		int EndPointMarkerSize { get; set; }				
		int NegativePointMarkerSize { get; set; }		   
		Color MarkerColor { get; set; }					 
		byte AreaOpacity { get; set; }					  
		int BarDistance { get; set; }					   
	}
}
namespace DevExpress.Snap.API.Native {
	using System;
	using System.Drawing;
	using DevExpress.Snap.Core.Fields;
	using DevExpress.Snap.Core.API;
	using ApiField = DevExpress.XtraRichEdit.API.Native.Field;
	using DevExpress.Sparkline;
	public class NativeSnapSparkline : NativeSnapSingleListItemEntity, SnapSparkline {
		int width;
		int height;
		SparklineViewType viewType;
		string dataSourceName;
		bool highlightMaxPoint;
		bool highlightMinPoint;
		bool highlightStartPoint;
		bool highlightEndPoint;
		Color color;
		Color maxPointColor;
		Color minPointColor;
		Color startPointColor;
		Color endPointColor;
		Color negativePointColor;
		int lineWidth;
		bool highlightNegativePoints;
		bool showMarkers;
		int markerSize;
		int maxPointMarkerSize;
		int minPointMarkerSize;
		int startPointMarkerSize;
		int endPointMarkerSize;
		int negativePointMarkerSize;
		Color markerColor;
		byte areaOpacity;
		int barDistance;
		public NativeSnapSparkline(SnapNativeDocument document, ApiField field) : base(document, field) { }
		public NativeSnapSparkline(SnapSubDocument subDocument, SnapNativeDocument document, ApiField field) : base(subDocument, document, field) { }
		protected override void Init() {
			base.Init();
			SNSparklineField parsedField = GetParsedField<SNSparklineField>();
			this.dataSourceName = parsedField.DataSourceName;
			this.viewType = parsedField.ViewType;
			this.width = parsedField.Width;
			this.height = parsedField.Height;
			this.highlightMaxPoint = parsedField.HighlightMaxPoint;
			this.highlightMinPoint = parsedField.HighlightMinPoint;
			this.highlightStartPoint = parsedField.HighlightStartPoint;
			this.highlightEndPoint = parsedField.HighlightEndPoint;
			this.color = parsedField.Color;
			this.maxPointColor = parsedField.MaxPointColor;
			this.minPointColor = parsedField.MinPointColor;
			this.startPointColor = parsedField.StartPointColor;
			this.endPointColor = parsedField.EndPointColor;
			this.negativePointColor = parsedField.NegativePointColor;
			this.lineWidth = parsedField.LineWidth;
			this.highlightNegativePoints = parsedField.HighlightNegativePoints;
			this.showMarkers = parsedField.ShowMarkers;
			this.markerSize = parsedField.MarkerSize;
			this.maxPointMarkerSize = parsedField.MaxPointMarkerSize;
			this.minPointMarkerSize = parsedField.MinPointMarkerSize;
			this.startPointMarkerSize = parsedField.StartPointMarkerSize;
			this.endPointMarkerSize = parsedField.EndPointMarkerSize;
			this.negativePointMarkerSize = parsedField.NegativePointMarkerSize;
			this.markerColor = parsedField.MarkerColor;
			this.areaOpacity = parsedField.AreaOpacity;
			this.barDistance = parsedField.BarDistance;
		}
		int GetScale(SNSparklineField parsedField, string key) { return parsedField.Switches.Switches.ContainsKey('\\' + key) ? parsedField.Switches.GetInt(key) : 100; }
		public string DataSourceName {
			get { return dataSourceName; }
			set {
				EnsureUpdateBegan();
				if(dataSourceName == value) return;
				Controller.SetSwitch(SNSparklineField.SnSparklineDataSourceNameSwitch, value);
				dataSourceName = value;
			}
		}
		public SparklineViewType ViewType {
			get { return viewType; }
			set {
				EnsureUpdateBegan();
				if(viewType == value) return;
				Controller.SetSwitch(SNSparklineField.SnSparklineViewTypeSwitch, Convert.ToString(value));
				viewType = value;
			}
		}
		#region SnapImage Members
		public int Width {
			get { return width; }
			set {
				EnsureUpdateBegan();
				width = value;
				if(width == 0 && height == 0) {
					Controller.RemoveSwitch(SNSparklineField.SnSparklineWidthSwitch);
					Controller.RemoveSwitch(SNSparklineField.SnSparklineHeightSwitch);
				}
				else
					Controller.SetSwitch(SNSparklineField.SnSparklineWidthSwitch, Convert.ToString(value));
			}
		}
		public int Height {
			get { return height; }
			set {
				EnsureUpdateBegan();
				height = value;
				if(width == 0 && height == 0) {
					Controller.RemoveSwitch(SNSparklineField.SnSparklineWidthSwitch);
					Controller.RemoveSwitch(SNSparklineField.SnSparklineHeightSwitch);
				}
				else
					Controller.SetSwitch(SNSparklineField.SnSparklineHeightSwitch, Convert.ToString(value));
			}
		}
		public Size Size {
			get { return new Size(Width, Height); }
			set {
				Width = value.Width;
				Height = value.Height;
			}
		}
		#endregion
		#region SparklineViewBase
		public bool HighlightMaxPoint {
			get { return highlightMaxPoint; }
			set {
				EnsureUpdateBegan();
				if(highlightMaxPoint == value) return;
				Controller.SetSwitch(SNSparklineField.SnSparklineHighlightMaxPointSwitch, Convert.ToString(value));
				highlightMaxPoint = value;
			}
		}
		public bool HighlightMinPoint {
			get { return highlightMinPoint; }
			set {
				EnsureUpdateBegan();
				if(highlightMinPoint == value) return;
				Controller.SetSwitch(SNSparklineField.SnSparklineHighlightMinPointSwitch, Convert.ToString(value));
				highlightMinPoint = value;
			}
		}
		public bool HighlightStartPoint {
			get { return highlightStartPoint; }
			set {
				EnsureUpdateBegan();
				if(highlightStartPoint == value) return;
				Controller.SetSwitch(SNSparklineField.SnSparklineHighlightStartPointSwitch, Convert.ToString(value));
				highlightStartPoint = value;
			}
		}
		public bool HighlightEndPoint {
			get { return highlightEndPoint; }
			set {
				EnsureUpdateBegan();
				if(highlightEndPoint == value) return;
				Controller.SetSwitch(SNSparklineField.SnSparklineHighlightEndPointSwitch, Convert.ToString(value));
				highlightEndPoint = value;
			}
		}
		public Color Color {
			get { return color; }
			set {
				EnsureUpdateBegan();
				if(color == value) return;
				Controller.SetSwitch(SNSparklineField.SnSparklineColorSwitch, Convert.ToString(value.ToArgb()));
				color = value;
			}
		}
		public Color MaxPointColor {
			get { return maxPointColor; }
			set {
				EnsureUpdateBegan();
				if(maxPointColor == value) return;
				Controller.SetSwitch(SNSparklineField.SnSparklineMaxPointColorSwitch, Convert.ToString(value.ToArgb()));
				maxPointColor = value;
			}
		}
		public Color MinPointColor {
			get { return minPointColor; }
			set {
				EnsureUpdateBegan();
				if(minPointColor == value) return;
				Controller.SetSwitch(SNSparklineField.SnSparklineMinPointColorSwitch, Convert.ToString(value.ToArgb()));
				minPointColor = value;
			}
		}
		public Color StartPointColor {
			get { return startPointColor; }
			set {
				EnsureUpdateBegan();
				if(startPointColor == value) return;
				Controller.SetSwitch(SNSparklineField.SnSparklineStartPointColorSwitch, Convert.ToString(value.ToArgb()));
				startPointColor = value;
			}
		}
		public Color EndPointColor {
			get { return endPointColor; }
			set {
				EnsureUpdateBegan();
				if(endPointColor == value) return;
				Controller.SetSwitch(SNSparklineField.SnSparklineEndPointColorSwitch, Convert.ToString(value.ToArgb()));
				endPointColor = value;
			}
		}
		public Color NegativePointColor {
			get { return negativePointColor; }
			set {
				EnsureUpdateBegan();
				if(negativePointColor == value) return;
				Controller.SetSwitch(SNSparklineField.SnSparklineNegativePointColorSwitch, Convert.ToString(value.ToArgb()));
				negativePointColor = value;
			}
		}
		#endregion SparklineViewBase
		#region Line fields
		public int LineWidth {
			get { return lineWidth; }
			set {
				EnsureUpdateBegan();
				if(lineWidth == value) return;
				Controller.SetSwitch(SNSparklineField.SnSparklineLineWidthSwitch, Convert.ToString(value));
				lineWidth = value;
			}
		}
		public bool HighlightNegativePoints {
			get { return highlightNegativePoints; }
			set {
				EnsureUpdateBegan();
				if(highlightNegativePoints == value) return;
				Controller.SetSwitch(SNSparklineField.SnSparklineHighlightNegativePointsSwitch, Convert.ToString(value));
				highlightNegativePoints = value;
			}
		}
		public bool ShowMarkers {
			get { return showMarkers; }
			set {
				EnsureUpdateBegan();
				if(showMarkers == value) return;
				Controller.SetSwitch(SNSparklineField.SnSparklineShowMarkersSwitch, Convert.ToString(value));
				showMarkers = value;
			}
		}
		public int MarkerSize {
			get { return markerSize; }
			set {
				EnsureUpdateBegan();
				if(markerSize == value) return;
				Controller.SetSwitch(SNSparklineField.SnSparklineMarkerSizeSwitch, Convert.ToString(value));
				markerSize = value;
			}
		}
		public int MaxPointMarkerSize {
			get { return maxPointMarkerSize; }
			set {
				EnsureUpdateBegan();
				if(maxPointMarkerSize == value) return;
				Controller.SetSwitch(SNSparklineField.SnSparklineMaxPointMarkerSizeSwitch, Convert.ToString(value));
				maxPointMarkerSize = value;
			}
		}
		public int MinPointMarkerSize {
			get { return minPointMarkerSize; }
			set {
				EnsureUpdateBegan();
				if(minPointMarkerSize == value) return;
				Controller.SetSwitch(SNSparklineField.SnSparklineMinPointMarkerSizeSwitch, Convert.ToString(value));
				minPointMarkerSize = value;
			}
		}
		public int StartPointMarkerSize {
			get { return startPointMarkerSize; }
			set {
				EnsureUpdateBegan();
				if(startPointMarkerSize == value) return;
				Controller.SetSwitch(SNSparklineField.SnSparklineStartPointMarkerSizeSwitch, Convert.ToString(value));
				startPointMarkerSize = value;
			}
		}
		public int EndPointMarkerSize {
			get { return endPointMarkerSize; }
			set {
				EnsureUpdateBegan();
				if(endPointMarkerSize == value) return;
				Controller.SetSwitch(SNSparklineField.SnSparklineEndPointMarkerSizeSwitch, Convert.ToString(value));
				endPointMarkerSize = value;
			}
		}
		public int NegativePointMarkerSize {
			get { return negativePointMarkerSize; }
			set {
				EnsureUpdateBegan();
				if(negativePointMarkerSize == value) return;
				Controller.SetSwitch(SNSparklineField.SnSparklineNegativePointMarkerSizeSwitch, Convert.ToString(value));
				negativePointMarkerSize = value;
			}
		}
		public Color MarkerColor {
			get { return markerColor; }
			set {
				EnsureUpdateBegan();
				if(markerColor == value) return;
				Controller.SetSwitch(SNSparklineField.SnSparklineMarkerColorSwitch, Convert.ToString(value.ToArgb()));
				markerColor = value;
			}
		}
		#endregion Line fields
		#region Area fields
		public byte AreaOpacity {
			get { return areaOpacity; }
			set {
				EnsureUpdateBegan();
				if(areaOpacity == value) return;
				Controller.SetSwitch(SNSparklineField.SnSparklineAreaOpacitySwitch, Convert.ToString(value));
				areaOpacity = value;
			}
		}
		#endregion Area fields
		#region BarSparklineViewBase fields
		public int BarDistance {
			get { return barDistance; }
			set {
				EnsureUpdateBegan();
				if(barDistance == value) return;
				Controller.SetSwitch(SNSparklineField.SnSparklineBarDistanceSwitch, Convert.ToString(value));
				barDistance = value;
			}
		}
		#endregion BarSparklineViewBase fields
	}
}
