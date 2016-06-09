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

using System.Windows;
using System.Windows.Media;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Charts {
	[NonCategorized]
	public class IndicatorItem : NotifyPropertyChangedObject, IWeakEventListener {
		Brush brush;
		LineStyle lineStyle;
		Indicator indicator;
		IndicatorLabel label;
		IndicatorGeometry geometry;
		int actualLineThickness;
		public IndicatorGeometry IndicatorGeometry { 
			get { return  geometry; } 
			set { 
				geometry = value;
				OnPropertyChanged("IndicatorGeometry");
			}
		}
		public Brush Brush {
			get { return brush; }
			set {
				brush = value;
				ISupportIndicatorLabel labledIndicator = indicator as ISupportIndicatorLabel;
				if (labledIndicator != null && labledIndicator.Label == null)
					Label.Foreground = value;
				OnPropertyChanged("Brush");
			}
		}
		public LineStyle LineStyle {
			get { return lineStyle; }
			set {
				CommonUtils.SubscribePropertyChangedWeakEvent(lineStyle, value, this);
				lineStyle = value;
				OnPropertyChanged("LineStyle");
				UpdateSelection();
			}
		}
		public bool ShowInLegend {
			get { return indicator.ShowInLegend; }
		}
		public string LegendText { get { return indicator.LegendText; } }
		public IndicatorLabel Label { 
			get { return label;  }
			set { label = value; }
		}
		public int ActualLineThickness {
			get { return actualLineThickness; }
			set {
				if (actualLineThickness != value) {
					actualLineThickness = value;
					OnPropertyChanged("ActualLineThickness");
				}
			}
		}
		public bool Visible { get { return indicator.GetActualVisible(); } }
		internal LegendItem LegendItem { get; set; }
		internal XYSeries2D XYSeries2D { get { return indicator.XYSeries; } }
		internal Indicator Indicator {get { return indicator; } }
		internal IndicatorItem(Indicator indicator) {
			this.indicator = indicator;
			brush = indicator.Brush != null ? indicator.Brush : new SolidColorBrush(Colors.Black);
			lineStyle = indicator.LineStyle != null ? indicator.LineStyle : new LineStyle();
			Label = new IndicatorLabel();
		}
		internal void UpdateSelection() {
			int lineThickness = (LineStyle == null) ? (int)DevExpress.Xpf.Charts.LineStyle.ThicknessProperty.GetMetadata(typeof(LineStyle)).DefaultValue : LineStyle.Thickness;
			ActualLineThickness = VisualSelectionHelper.GetLineThickness(lineThickness, ((IInteractiveElement)indicator).IsSelected);
		}
		bool IWeakEventListener.ReceiveWeakEvent(System.Type managerType, object sender, System.EventArgs e) {
			if (managerType == typeof(PropertyChangedWeakEventManager) && (sender is LineStyle)) {
				UpdateSelection();
				return true;
			}
			return false;
		}
	}
}
