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

using System.ComponentModel;
using System.Windows;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Map {
	public class MapRectangleSettings : MapItemSettings {
		public static readonly DependencyProperty HeightProperty = DependencyPropertyManager.Register("Height",
		  typeof(double), typeof(MapRectangleSettings), new PropertyMetadata(0.0, UpdateItems));
		public static readonly DependencyProperty WidthProperty = DependencyPropertyManager.Register("Width",
		  typeof(double), typeof(MapRectangleSettings), new PropertyMetadata(0.0, UpdateItems));
		public static readonly DependencyProperty RadiusXProperty = DependencyPropertyManager.Register("RadiusX",
		  typeof(double), typeof(MapRectangleSettings), new PropertyMetadata(0.0, UpdateItems));
		public static readonly DependencyProperty RadiusYProperty = DependencyPropertyManager.Register("RadiusY",
		  typeof(double), typeof(MapRectangleSettings), new PropertyMetadata(0.0, UpdateItems));
		[Category(Categories.Layout)]
		public double Height {
			get { return (double)GetValue(HeightProperty); }
			set { SetValue(HeightProperty, value); }
		}
		[Category(Categories.Layout)]
		public double Width {
			get { return (double)GetValue(WidthProperty); }
			set { SetValue(WidthProperty, value); }
		}
		[Category(Categories.Appearance)]
		public double RadiusX {
			get { return (double)GetValue(RadiusXProperty); }
			set { SetValue(RadiusXProperty, value); }
		}
		[Category(Categories.Appearance)]
		public double RadiusY {
			get { return (double)GetValue(RadiusYProperty); }
			set { SetValue(RadiusYProperty, value); }
		}
		protected override MapDependencyObject CreateObject() {
			return new MapRectangleSettings();
		}
		protected override MapItem CreateItemInstance() {
			return new MapRectangle();
		}
		protected override void FillPropertiesMappings(DependencyPropertyMappingDictionary mappings) {
			base.FillPropertiesMappings(mappings);
			mappings.Add(MapRectangleSettings.HeightProperty, MapRectangle.HeightProperty);
			mappings.Add(MapRectangleSettings.WidthProperty, MapRectangle.WidthProperty);
			mappings.Add(MapRectangleSettings.RadiusXProperty, MapRectangle.RadiusXProperty);
			mappings.Add(MapRectangleSettings.RadiusYProperty, MapRectangle.RadiusYProperty);
		}
	}
}
