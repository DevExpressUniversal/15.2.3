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
	public class MapDotSettings : MapItemSettings {
		public static readonly DependencyProperty SizeProperty = DependencyPropertyManager.Register("Size",
		  typeof(double), typeof(MapDotSettings), new PropertyMetadata(3.0, UpdateItems));
		public static readonly DependencyProperty ShapeKindProperty = DependencyPropertyManager.Register("ShapeKind",
		  typeof(MapDotShapeKind), typeof(MapDotSettings), new PropertyMetadata(MapDotShapeKind.Circle, UpdateItems));
		[Category(Categories.Layout)]
		public double Size {
			get { return (double)GetValue(SizeProperty); }
			set { SetValue(SizeProperty, value); }
		}
		[Category(Categories.Appearance)]
		public MapDotShapeKind ShapeKind {
			get { return (MapDotShapeKind)GetValue(ShapeKindProperty); }
			set { SetValue(ShapeKindProperty, value); }
		}
		protected override MapDependencyObject CreateObject() {
			return new MapDotSettings();
		}
		protected override MapItem CreateItemInstance() {
			return new MapDot();
		}
		protected override void FillPropertiesMappings(DependencyPropertyMappingDictionary mappings) {
			base.FillPropertiesMappings(mappings);
			mappings.Add(MapDotSettings.ShapeKindProperty, MapDot.ShapeKindProperty);
			mappings.Add(MapDotSettings.SizeProperty, MapDot.SizeProperty);
		}
	}
}
