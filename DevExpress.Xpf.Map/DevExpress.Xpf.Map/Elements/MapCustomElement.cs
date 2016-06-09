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

using DevExpress.Map;
using DevExpress.Map.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
namespace DevExpress.Xpf.Map {
	[ContentProperty("Content")]
	public class MapCustomElement : MapItem, ISupportCoordLocation, IPointCore, IClusterable, IClusterItem, IColorizerElement {
		const string templateSource = "<ContentControl ContentTemplate=\"{Binding MapItem.ContentTemplate}\" Content=\"{Binding MapItem.Content}\" Visibility=\"{Binding Path=MapItem.Visible, Converter={local:BoolToVisibilityConverter}}\"/>";
		IList<IClusterable> clusteredItems = new List<IClusterable>();
		public static readonly DependencyProperty TemplateProperty = DependencyPropertyManager.Register("Template",
			typeof(ControlTemplate), typeof(MapCustomElement), new PropertyMetadata(GetDefaultTemplate()));
		public static readonly DependencyProperty LocationProperty = DependencyPropertyManager.Register("Location",
		   typeof(CoordPoint), typeof(MapCustomElement), new PropertyMetadata(new GeoPoint(0, 0), LayoutPropertyChanged, CoerceLocation));
		public static readonly DependencyProperty ContentProperty = DependencyPropertyManager.Register("Content",
		   typeof(object), typeof(MapCustomElement), new PropertyMetadata(null));
		public static readonly DependencyProperty ContentTemplateProperty = DependencyPropertyManager.Register("ContentTemplate",
		   typeof(DataTemplate), typeof(MapCustomElement), new PropertyMetadata(null));
		static object CoerceLocation(DependencyObject d, object baseValue) {
			if (baseValue == null)
				return new GeoPoint(0, 0);
			return baseValue;
		}
		protected internal override object Source {
			get { return Content != null ? Content : base.Source; }
		}
		protected internal override ControlTemplate ItemTemplate {
			get { return Template; }
		}
		protected internal override IList<CoordPoint> GetItemPoints() {
			return new CoordPoint[] { Location };
		}
		[Category(Categories.Layout), TypeConverter(typeof(GeoPointConverter))]
		public CoordPoint Location {
			get { return (CoordPoint)GetValue(LocationProperty); }
			set { SetValue(LocationProperty, value); }
		}
		[Category(Categories.Data)]
		public object Content {
			get { return (object)GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}
		[Category(Categories.Presentation)]
		public DataTemplate ContentTemplate {
			get { return (DataTemplate)GetValue(ContentTemplateProperty); }
			set { SetValue(ContentTemplateProperty, value); }
		}
		[Category(Categories.Presentation)]
		public ControlTemplate Template {
			get { return (ControlTemplate)GetValue(TemplateProperty); }
			set { SetValue(TemplateProperty, value); }
		}
		internal static ControlTemplate GetDefaultTemplate() {
			XamlHelper.SetLocalNamespace(CommonUtils.localNamespace);
			return XamlHelper.GetControlTemplate(templateSource);
		}
		protected override bool IsVisualElement { get { return true; } }
		protected override MapDependencyObject CreateObject() {
			return new MapCustomElement();
		}
		protected internal override CoordBounds CalculateBounds() {
			return new CoordBounds(Location, Location);
		}
		protected override void CalculateLayoutInMapUnits() {
			Layout.Location = Location;
			Layout.LocationInMapUnits = CoordinateSystem.CoordPointToMapUnit(Location);
		}
		protected override void CalculateLayout() {
			Layout.LocationInPixels = Layer.MapUnitToScreenZeroOffset(Layout.LocationInMapUnits);
			Layout.SizeInPixels = new Size(double.PositiveInfinity, double.PositiveInfinity);
		}
		#region IClusterItem, IClusterable implementation
		IClusterItem IClusterable.CreateInstance() {
			return new MapPushpin();
		}
		IMapUnit IClusterItemCore.GetUnitLocation() {
			return Layout.LocationInMapUnits;
		}
		object IClusterItemCore.Owner {
			get {
				return ((IOwnedElement)this).Owner;
			}
			set {
				SetOwnerInternal(value);
			}
		}
		void IClusterItem.ApplySize(double size) {
		}
		IList<IClusterable> IClusterItem.ClusteredItems { get { return clusteredItems; } set { clusteredItems = value; } }
		string IClusterItem.Text { get; set; }
		#endregion
		System.Windows.Media.Color IColorizerElement.ColorizerColor {
			get { return ColorizerColor; }
			set { ColorizerColor = value; }
		}
	}
}
