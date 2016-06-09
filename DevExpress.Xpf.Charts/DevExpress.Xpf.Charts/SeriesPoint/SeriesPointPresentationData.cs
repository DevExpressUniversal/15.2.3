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
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using DevExpress.Utils;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	[NonCategorized]
	public class SeriesPointPresentationData : NotifyPropertyChangedObject {
		Color pointColor;
		string legendText;
		string labelText;
		double opacity;
		SeriesPointLayout layout;
		bool isHighlighted = false;
		bool isSelected = false;
		Brush opacityMask = null;
		SeriesPoint seriesPoint;
		internal VisualSelectionType SeriesSelectionType { get; set; }
		internal Color CrosshairPointColor { get { return pointColor; } }
		[Browsable(false), DevExpress.Utils.DXBrowsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Brush OpacityMask { get { return opacityMask; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("SeriesPointPresentationDataPointColor")]
#endif
		public Color PointColor {
			get { return GetPointColor(); }
			set {
				pointColor = value;
				UpdatePointColor();
			}
		}
#if !SL
	[DevExpressXpfChartsLocalizedDescription("SeriesPointPresentationDataLegendText")]
#endif
		public string LegendText {
			get { return legendText; }
			set {
				legendText = value;
				OnPropertyChanged("LegendText");
			}
		}
#if !SL
	[DevExpressXpfChartsLocalizedDescription("SeriesPointPresentationDataLabelText")]
#endif
		public string LabelText {
			get { return labelText; }
			set {
				labelText = value;
				OnPropertyChanged("LabelText");
			}
		}
#if !SL
	[DevExpressXpfChartsLocalizedDescription("SeriesPointPresentationDataOpacity")]
#endif
		public double Opacity {
			get { return opacity; }
			set {
				opacity = value;
				OnPropertyChanged("Opacity");
			}
		}
#if !SL
	[DevExpressXpfChartsLocalizedDescription("SeriesPointPresentationDataLayout")]
#endif
		public SeriesPointLayout Layout {
			get { return layout; }
			set {
				layout = value;
				UpdateOpacityMask();
				OnPropertyChanged("Layout");
			}
		}
#if !SL
	[DevExpressXpfChartsLocalizedDescription("SeriesPointPresentationDataSeriesPoint")]
#endif
		public SeriesPoint SeriesPoint {
			get { return seriesPoint; }
			internal set {
				seriesPoint = value;
				OnPropertyChanged("SeriesPoint");
			}
		}
#if !SL
	[DevExpressXpfChartsLocalizedDescription("SeriesPointPresentationDataIsHighlighted")]
#endif
		public bool IsHighlighted {
			get { return isHighlighted; }
			internal set {
				isHighlighted = value;
				UpdatePointColor();
				OnPropertyChanged("IsHighlighted");
			}
		}
#if !SL
	[DevExpressXpfChartsLocalizedDescription("SeriesPointPresentationDataIsSelected")]
#endif
		public bool IsSelected {
			get { return isSelected; }
			internal set {
				isSelected = value;
				OnSelectedStateChanged();
				OnPropertyChanged("IsSelected");
			}
		}
		void UpdateOpacityMask() {
			if (isSelected && VisualSelectionHelper.SupportsHatchSelection(SeriesSelectionType) && layout != null)
				opacityMask = VisualSelectionHelper.GetOpacityMask(layout.Transform);
			else
				opacityMask = null;
			OnPropertyChanged("OpacityMask");
		}
		void UpdatePointColor() {
			OnPropertyChanged("PointColor");
		}
		Color GetPointColor() {
			if (VisualSelectionHelper.SupportsBrightnessSelection(SeriesSelectionType) && isSelected)
				return VisualSelectionHelper.GetSelectedPointColor(pointColor);
			else if (isHighlighted)
				return VisualSelectionHelper.GetHighlightedPointColor(pointColor);
			else
				return pointColor;
		}
		void OnSelectedStateChanged() {
			if (VisualSelectionHelper.SupportsHatchSelection(SeriesSelectionType))
				UpdateOpacityMask();
			if (VisualSelectionHelper.SupportsBrightnessSelection(SeriesSelectionType))
				UpdatePointColor();
		}
	}
}
