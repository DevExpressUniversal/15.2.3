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

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Media;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Map.Native;
namespace DevExpress.Xpf.Map {
	public abstract class MapLegendItemBase : INotifyPropertyChanged {
		Brush brush;
		string text;
		double value = double.NaN;
		string format;
		string actualText;
		[Category(Categories.Appearance)]
		public double Value {
			get { return value; }
			set {
				if(this.value != value) {
					this.value = value;
					UpdateText();
				}
			}
		}
		[Category(Categories.Appearance)]
		public string Format {
			get { return format; }
			set {
				if(format != value) {
					format = value;
					UpdateText();
				}
			}
		}
		[Category(Categories.Appearance)]
		public string Text {
			get { return text; }
			set {
				if(text != value) {
					text = value;
					UpdateText();
				}
			}
		}
		[Category(Categories.Appearance)]
		public Brush Fill {
			get { return brush; }
			set {
				if(brush != value) {
					brush = value;
					RaisePropertyChanged("Fill");
				}
			}
		}
		public string ActualText { get { return actualText; } }
		void UpdateText() {
			if(value.IsNumber() && string.IsNullOrEmpty(Text))
				this.actualText = Value.ToString(Format, CultureInfo.CurrentCulture);
			else
				this.actualText = Text;
			RaisePropertyChanged("ActualText");
		}
		#region INotifyPropertyChanged members
		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged(string propertyName) {
			PropertyChangedEventHandler propertyChanged = PropertyChanged;
			if(propertyChanged != null)
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
	}
	public class ColorLegendItem : MapLegendItemBase {
		Color color;
		[Category(Categories.Appearance)]
		public Color Color {
			get { return color; }
			set {
				if(!Color.Equals(color, value)) {
					color = value;
					Fill = new SolidColorBrush(color);
					RaisePropertyChanged("Color");
				}
			}
		}
	}
	public class SizeLegendItem : MapLegendItemBase {
		double markerSize;
		double maxItemize = 0.0;
		MarkerType markerType = MarkerType.Circle;
		Brush stroke;
		bool showTickMark = true;
		bool showLabel = true;
		[Category(Categories.Layout), Browsable(false)]
		public MarkerType MarkerType {
			get { return markerType; }
			set {
				if(markerType == value)
					return;
				markerType = value;
				RaisePropertyChanged("MarkerType");
			}
		}
		[Category(Categories.Layout), Browsable(false)]
		public double MaxItemSize {
			get { return maxItemize; }
			set {
				if(maxItemize == value)
					return;
				maxItemize = value;
				RaisePropertyChanged("MaxItemSize");
			}
		}
		[Category(Categories.Layout), Browsable(false)]
		public bool ShowTickMark {
			get { return showTickMark; }
			set {
				if(showTickMark == value)
					return;
				showTickMark = value;
				RaisePropertyChanged("ShowTickMark");
				RaisePropertyChanged("ActualTickMarkVisibility");
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public bool ActualTickMarkVisibility {
			get { return showTickMark && showLabel; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public bool ShowLabel {
			get { return showLabel; }
			set {
				if (showLabel == value)
					return;
				showLabel = value;
				RaisePropertyChanged("ShowLabel");
				RaisePropertyChanged("ActualTickMarkVisibility");
			}
		}
		[Category(Categories.Appearance)]
		public double MarkerSize {
			get { return markerSize; }
			set {
				if(markerSize == value)
					return;
				markerSize = value;
				RaisePropertyChanged("MarkerSize");
			}
		}
		[Category(Categories.Appearance)]
		public Brush Stroke {
			get { return stroke; }
			set {
				if(stroke == value)
					return;
				stroke = value;
				RaisePropertyChanged("Stroke");
			}
		}
	}
}
