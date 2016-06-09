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

using DevExpress.Map.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
namespace DevExpress.Xpf.Map {
	public enum MarkerType {
		Square,
		Diamond,
		Triangle,
		InvertedTriangle,
		Circle,
		Plus,
		Cross,
		Star5,
		Star6,
		Star8,
		Pentagon,
		Hexagon,
		Custom
	}
	public class MapBubble : MapChartItemBase, IMapChartItem, IKeyColorizerElement, IPointCore, IClusterItem ,IClusterable {
		const string templateSource = "<local:MapBubbleControl/>";
		IList<IClusterable> clusteredItems = new List<IClusterable>();
		public static readonly DependencyProperty MarkerTypeProperty = DependencyPropertyManager.Register("MarkerType",
			typeof(MarkerType), typeof(MapBubble), new PropertyMetadata(MarkerType.Circle, LayoutPropertyChanged));
		public static readonly DependencyProperty ValueProperty = DependencyPropertyManager.Register("Value",
			typeof(double), typeof(MapBubble), new PropertyMetadata(0.0, LayoutPropertyChanged));
		public static readonly DependencyProperty CustomMarkerTemplateProperty = DependencyPropertyManager.Register("CustomMarkerTemplate",
			typeof(DataTemplate), typeof(MapBubble), new PropertyMetadata(null));
		[Category(Categories.Layout)]
		public MarkerType MarkerType {
			get { return (MarkerType)GetValue(MarkerTypeProperty); }
			set { SetValue(MarkerTypeProperty, value); }
		}
		[Category(Categories.Layout)]
		public DataTemplate CustomMarkerTemplate {
			get { return (DataTemplate)GetValue(CustomMarkerTemplateProperty); }
			set { SetValue(CustomMarkerTemplateProperty, value); }
		}
		[Category(Categories.Data)]
		public double Value {
			get { return (double)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}
		static ControlTemplate template = GetDefaultTemplate();
		static ControlTemplate GetDefaultTemplate() {
			XamlHelper.SetLocalNamespace(CommonUtils.localNamespace);
			return XamlHelper.GetControlTemplate(templateSource);
		}
		protected override double ItemValue {
			get { return Value; }
			set { Value = value; }
		}
		protected internal override ControlTemplate ItemTemplate { get { return template; } }
		public MapBubble() {
		}
		#region IKeyColorizerElement implementation
		object IKeyColorizerElement.ColorItemKey {
			get { return ItemId; }
		}
		#endregion
		protected override MapDependencyObject CreateObject() {
			return new MapBubble();
		}
		protected internal override ToolTipPatternParser CreateToolTipPatternParser() {
			return new BubbleToolTipPatternParser(ActualToolTipPattern, ItemId, Value);
		}
		protected internal override IList<DevExpress.Map.CoordPoint> GetItemPoints() {
			return new DevExpress.Map.CoordPoint[] { Location };
		}
		#region IClusterItem, IClusterable implementation
		string IClusterItem.Text {
			get {
				return Title.Text;
			}
			set {
				Title.Text = value;
			}
		}
		IList<IClusterable> IClusterItem.ClusteredItems { get { return clusteredItems; } set { clusteredItems = value; } }
		object IClusterItemCore.Owner {
			get {
				return ((IOwnedElement)this).Owner;
			}
			set {
				SetOwnerInternal(value);
				ApplyTitleOptions();
			}
		}
		void IClusterItem.ApplySize(double size) {
			Size = size;
		}
		IMapUnit IClusterItemCore.GetUnitLocation() {
			return Layout.LocationInMapUnits;
		}
		IClusterItem IClusterable.CreateInstance() {
			return new MapBubble();
		}
		#endregion
	}
	[NonCategorized]
	public class MapChartItemInfo : MapItemInfo, IMapItemStyleProvider {
		Brush fill;
		Brush stroke;
		Visibility contentVisibility = Visibility.Visible;
		double strokeThickness;
		double strokeMiterLimit;
		double strokeDshOffset;
		DoubleCollection strokeDashArray;
		PenLineCap strokeDashCap;
		PenLineCap strokeEndLineCap;
		PenLineCap strokeStartLineCap;
		PenLineJoin strokeLineJoin;
		Effect effect;
		public Brush Fill {
			get { return fill; }
			set {
				if(fill != value) {
					fill = value;
					NotifyPropertyChanged("Fill");
				}
			}
		}
		public Brush Stroke {
			get { return stroke; }
			set {
				if(stroke != value) {
					stroke = value;
					NotifyPropertyChanged("Stroke");
				}
			}
		}
		public double StrokeThickness {
			get { return strokeThickness; }
			set {
				if(strokeThickness != value) {
					strokeThickness = value;
					NotifyPropertyChanged("StrokeThickness");
				}
			}
		}
		public double StrokeDashOffset {
			get { return strokeDshOffset; }
			set {
				if(strokeDshOffset != value) {
					strokeDshOffset = value;
					NotifyPropertyChanged("StrokeDashOffset");
				}
			}
		}
		public double StrokeMiterLimit {
			get { return strokeMiterLimit; }
			set {
				if(strokeMiterLimit != value) {
					strokeMiterLimit = value;
					NotifyPropertyChanged("StrokeMiterLimit");
				}
			}
		}
		public DoubleCollection StrokeDashArray {
			get { return strokeDashArray; }
			set {
				if(strokeDashArray != value) {
					strokeDashArray = value;
					NotifyPropertyChanged("StrokeDashArray");
				}
			}
		}
		public PenLineCap StrokeDashCap {
			get { return strokeDashCap; }
			set {
				if(strokeDashCap != value) {
					strokeDashCap = value;
					NotifyPropertyChanged("StrokeDashCap");
				}
			}
		}
		public PenLineCap StrokeStartLineCap {
			get { return strokeStartLineCap; }
			set {
				if(strokeStartLineCap != value) {
					strokeStartLineCap = value;
					NotifyPropertyChanged("StrokeStartLineCap");
				}
			}
		}
		public PenLineCap StrokeEndLineCap {
			get { return strokeEndLineCap; }
			set {
				if(strokeEndLineCap != value) {
					strokeEndLineCap = value;
					NotifyPropertyChanged("StrokeEndLineCap");
				}
			}
		}
		public PenLineJoin StrokeLineJoin {
			get { return strokeLineJoin; }
			set {
				if(strokeLineJoin != value) {
					strokeLineJoin = value;
					NotifyPropertyChanged("StrokeLineJoin");
				}
			}
		}
		public Effect Effect {
			get { return effect; }
			set {
				if(effect != value) {
					effect = value;
					NotifyPropertyChanged("Effect");
				}
			}
		}
		public Visibility ContentVisibility {
			get { return contentVisibility; }
			set {
				if(contentVisibility != value) {
					contentVisibility = value;
					NotifyPropertyChanged("ContentVisibility");
				}
			}
		}
		public MapChartItemInfo(MapItem item)
			: base(item) {
		}
		#region IMapItemStyleProvider Members
		Visibility IMapItemStyleProvider.Visibility {
			get { return ContentVisibility; }
			set { ContentVisibility = value; }
		}
		#endregion
	}
}
