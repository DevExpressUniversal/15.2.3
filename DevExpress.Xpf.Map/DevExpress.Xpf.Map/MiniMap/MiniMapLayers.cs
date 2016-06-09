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
using System.ComponentModel;
using DevExpress.Map;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
using System.Windows.Media;
using System.Windows.Markup;
namespace DevExpress.Xpf.Map {
	public class MiniMapLayerCollection : MapElementCollection<MiniMapLayerBase> {
		public MiniMapLayerCollection(MiniMap miniMap) {
			((IOwnedElement)this).Owner = miniMap;
		}
	}
	[TemplatePart(Name = "PART_Layer", Type = typeof(LayerBase))]
	public abstract class MiniMapLayerBase : MapElement {
		LayerBase innerLayer;
		protected internal LayerBase InnerLayer { get { return innerLayer; } }
		MiniMap MiniMap { get { return Owner as MiniMap; } }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			innerLayer = GetTemplateChild("PART_Layer") as LayerBase;
			if (MiniMap != null) {
				((IOwnedElement)innerLayer).Owner = MiniMap;
				MiniMap.UpdateMapViewport();
			}
		}
		protected override void OwnerChanged() {
			base.OwnerChanged();
			if (InnerLayer != null)
				((IOwnedElement)InnerLayer).Owner = Owner;
		}
		internal void UpdateViewport() {
			if (InnerLayer != null)
				InnerLayer.Viewport = MiniMap.Viewport;
		}
	}
	[ContentProperty("DataProvider")]
	public class MiniMapImageTilesLayer : MiniMapLayerBase {
		public static readonly DependencyProperty DataProviderProperty = DependencyPropertyManager.Register("DataProvider",
			typeof(MapDataProviderBase), typeof(MiniMapImageTilesLayer));
		[Category(Categories.Data)]
		public MapDataProviderBase DataProvider {
			get { return (MapDataProviderBase)GetValue(DataProviderProperty); }
			set { SetValue(DataProviderProperty, value); }
		}
		public MiniMapImageTilesLayer() {
			DefaultStyleKey = typeof(MiniMapImageTilesLayer);
		}
	}
	[ContentProperty("Data")]
	public class MiniMapVectorLayer : MiniMapLayerBase {
		public static readonly DependencyProperty DataProperty = DependencyPropertyManager.Register("Data",
			typeof(MapDataAdapterBase), typeof(MiniMapVectorLayer));
		public static readonly DependencyProperty ShapeFillProperty = DependencyPropertyManager.Register("ShapeFill",
			typeof(Brush), typeof(MiniMapVectorLayer));
		public static readonly DependencyProperty ShapeStrokeProperty = DependencyPropertyManager.Register("ShapeStroke",
			typeof(Brush), typeof(MiniMapVectorLayer));
		public static readonly DependencyProperty ShapeStrokeStyleProperty = DependencyPropertyManager.Register("ShapeStrokeStyle",
			typeof(StrokeStyle), typeof(MiniMapVectorLayer));
		[Category(Categories.Data)]
		public MapDataAdapterBase Data {
			get { return (MapDataAdapterBase)GetValue(DataProperty); }
			set { SetValue(DataProperty, value); }
		}
		[Category(Categories.Appearance)]
		public Brush ShapeFill {
			get { return (Brush)GetValue(ShapeFillProperty); }
			set { SetValue(ShapeFillProperty, value); }
		}
		[Category(Categories.Appearance)]
		public Brush ShapeStroke {
			get { return (Brush)GetValue(ShapeStrokeProperty); }
			set { SetValue(ShapeStrokeProperty, value); }
		}
		[Category(Categories.Appearance)]
		public StrokeStyle ShapeStrokeStyle {
			get { return (StrokeStyle)GetValue(ShapeStrokeStyleProperty); }
			set { SetValue(ShapeStrokeStyleProperty, value); }
		}
		public MiniMapVectorLayer() {
			DefaultStyleKey = typeof(MiniMapVectorLayer);
		}
	}
}
