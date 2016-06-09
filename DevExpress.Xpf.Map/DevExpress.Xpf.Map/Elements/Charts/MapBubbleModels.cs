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
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Map {
	[NonCategorized]
	public class MarkerTypeToModelConverter : IValueConverter {
		MarkerBubbleControlBase CreateModelControl(MarkerType type) {
			switch(type) {
				case MarkerType.Square:
					return new SquareMarkerModelControl();
				case MarkerType.Diamond:
					return new DiamondMarkerModelControl();
				case MarkerType.Triangle:
					return new TriangleMarkerModelControl();
				case MarkerType.InvertedTriangle:
					return new InvertedTriangleMarkerModelControl();
				case MarkerType.Circle:
					return new CircleMarkerModelControl();
				case MarkerType.Plus:
					return new PlusMarkerModelControl();
				case MarkerType.Cross:
					return new CrossMarkerModelControl();
				case MarkerType.Star5:
					return new Star5MarkerModelControl();
				case MarkerType.Star6:
					return new Star6MarkerModelControl();
				case MarkerType.Star8:
					return new Star8MarkerModelControl();
				case MarkerType.Pentagon:
					return new PentagonMarkerModelControl();
				case MarkerType.Hexagon:
					return new HexagonMarkerModelControl();
				case MarkerType.Custom:
					return new CustomMarkerModelControl();
			}
			return null;
		}
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			Style shapeStyle = parameter as Style;
			if(value is MarkerType && shapeStyle != null) {
				MarkerBubbleControlBase bubbleMarkerControl = CreateModelControl((MarkerType)value);
				if (bubbleMarkerControl != null)
					bubbleMarkerControl.ShapeStyle = shapeStyle;
				return bubbleMarkerControl;
			}
			return null;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotSupportedException();
		}
		#endregion
	}
	public class MapBubbleControl : Control {
		public MapBubbleControl() {
			DefaultStyleKey = typeof(MapBubbleControl);
		}
	}
	[NonCategorized]
	public class MarkerBubbleControlBase : Control {
		public static readonly DependencyProperty ShapeStyleProperty = DependencyProperty.Register("ShapeStyle",
			typeof(Style), typeof(MarkerBubbleControlBase), new PropertyMetadata());
		public Style ShapeStyle {
			get { return (Style)GetValue(ShapeStyleProperty); }
			set { SetValue(ShapeStyleProperty, value); }
		}
	}
	public class SquareMarkerModelControl : MarkerBubbleControlBase {
		public SquareMarkerModelControl() {
			DefaultStyleKey = typeof(SquareMarkerModelControl);
		}
	}
	public class DiamondMarkerModelControl : MarkerBubbleControlBase {
		public DiamondMarkerModelControl() {
			DefaultStyleKey = typeof(DiamondMarkerModelControl);
		}
	}
	public class TriangleMarkerModelControl : MarkerBubbleControlBase {
		public TriangleMarkerModelControl() {
			DefaultStyleKey = typeof(TriangleMarkerModelControl);
		}
	}
	public class InvertedTriangleMarkerModelControl : MarkerBubbleControlBase {
		public InvertedTriangleMarkerModelControl() {
			DefaultStyleKey = typeof(InvertedTriangleMarkerModelControl);
		}
	}
	public class CircleMarkerModelControl : MarkerBubbleControlBase {
		public CircleMarkerModelControl() {
			DefaultStyleKey = typeof(CircleMarkerModelControl);
		}
	}
	public class PlusMarkerModelControl : MarkerBubbleControlBase {
		public PlusMarkerModelControl() {
			DefaultStyleKey = typeof(PlusMarkerModelControl);
		}
	}
	public class CrossMarkerModelControl : MarkerBubbleControlBase {
		public CrossMarkerModelControl() {
			DefaultStyleKey = typeof(CrossMarkerModelControl);
		}
	}
	public class Star5MarkerModelControl : MarkerBubbleControlBase {
		public Star5MarkerModelControl() {
			DefaultStyleKey = typeof(Star5MarkerModelControl);
		}
	}
	public class Star6MarkerModelControl : MarkerBubbleControlBase {
		public Star6MarkerModelControl() {
			DefaultStyleKey = typeof(Star6MarkerModelControl);
		}
	}
	public class Star8MarkerModelControl : MarkerBubbleControlBase {
		public Star8MarkerModelControl() {
			DefaultStyleKey = typeof(Star8MarkerModelControl);
		}
	}
	public class PentagonMarkerModelControl : MarkerBubbleControlBase {
		public PentagonMarkerModelControl() {
			DefaultStyleKey = typeof(PentagonMarkerModelControl);
		}
	}
	public class HexagonMarkerModelControl : MarkerBubbleControlBase {
		public HexagonMarkerModelControl() {
			DefaultStyleKey = typeof(HexagonMarkerModelControl);
		}
	}
	public class CustomMarkerModelControl : MarkerBubbleControlBase {
		public CustomMarkerModelControl() {
			DefaultStyleKey = typeof(CustomMarkerModelControl);
		}
	}
}
