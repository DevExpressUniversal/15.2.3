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
using System.Windows.Controls;
using System.Windows.Markup;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Map {
	[
	TemplatePart(Name = "PART_MapImage", Type = typeof(MultiScaleImage)),
	TemplatePart(Name = "PART_ErrorPanel", Type = typeof(FrameworkElement)),
	ContentProperty("DataProvider")
	]
	public class ImageTilesLayer : LayerBase {
		public static readonly DependencyProperty DataProviderProperty = DependencyPropertyManager.Register("DataProvider",
			typeof(MapDataProviderBase), typeof(ImageTilesLayer), new PropertyMetadata(null, DataProviderPropertyChanged));
		[Category(Categories.Data)]
		public MapDataProviderBase DataProvider {
			get { return (MapDataProviderBase)GetValue(DataProviderProperty); }
			set { SetValue(DataProviderProperty, value); }
		}
		static void DataProviderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ImageTilesLayer layer = d as ImageTilesLayer;
			if (layer != null) {
				layer.SubscribeToTileSourceChanged(e.OldValue as MapDataProviderBase, e.NewValue as MapDataProviderBase);
				layer.UpdateDataProvider(e.NewValue as MapDataProviderBase);
			}
		}
		MultiScaleImage mapImage = null;
		FrameworkElement errorElement = null;
		Grid InvalidKeyPanel {
			get {
				IInvalidKeyPanelHolder holder = Owner as IInvalidKeyPanelHolder;
				return holder != null ? holder.InvalidKeyPanel : null;
			}
		}
		protected internal override ProjectionBase ActualProjection {
			get { return DataProvider != null && DataProvider.Projection != null ? DataProvider.Projection : MapControl.DefaultMapProjection; }
		}
		protected internal override bool IsDataReady { get { return mapImage != null ? mapImage.IsDataReady : true; } }
		public ImageTilesLayer() {
			DefaultStyleKey = typeof(ImageTilesLayer);
			Loaded += new RoutedEventHandler(OnLayerLoaded);
		}
		void OnLayerLoaded(object sender, RoutedEventArgs e) {
			UpdateDataProvider(DataProvider);
		}
		void SubscribeToTileSourceChanged(MapDataProviderBase oldValue, MapDataProviderBase newValue) {
			if (oldValue != null)
				oldValue.TileSourceChanged -= new TileSourceChangedEventHandler(OnTileSourceChanged);
			if (newValue != null)
				newValue.TileSourceChanged += new TileSourceChangedEventHandler(OnTileSourceChanged);
		}
		void OnTileSourceChanged(object sender, TileSourceChangedEventArgs args) {
			CheckDataProvider(sender as MapDataProviderBase);
		}
		void CheckDataProvider(MapDataProviderBase provider) {
			if (provider != null && IsLoaded && provider.ShouldShowInvalidKeyMessage) {
				if (InvalidKeyPanel != null) {
					InvalidKeyPanel.DataContext = provider.GetInvalidKeyMessage();
					InvalidKeyPanel.Visibility = Visibility.Visible;
				}
			}
			else if (InvalidKeyPanel != null) {
				InvalidKeyPanel.Visibility = Visibility.Collapsed;
			}
		}
		void ApplyViewport() {
			if (mapImage != null) {
				Rect viewport = ActualViewport;
				mapImage.ViewportOrigin = new Point(viewport.X, viewport.Y);
				mapImage.ViewportWidth = viewport.Width;
			};
		}
		void UpdateDataProvider(MapDataProviderBase DataProvider) {
			CheckDataProvider(DataProvider);
			CheckCompatibility();
		}
		protected override void ViewportUpdated(bool zoomLevelChanged) {
			base.ViewportUpdated(zoomLevelChanged);
			ApplyViewport();
		}
		protected override void OwnerChanged() {
			base.OwnerChanged();
			UpdateDataProvider(DataProvider);
		}
		protected override Size GetMapSizeInPixels(double zoomLevel) {
			return DataProvider != null ? DataProvider.GetMapSizeInPixels(zoomLevel) : Size.Empty;
		}
		protected internal override Size GetMapBaseSizeInPixels() {
			return new Size(512, 512); 
		}
		protected internal override void CheckCompatibility() {
			ISupportProjection supportProjection = Owner as ISupportProjection;
			ProjectionBase mapProjection = supportProjection != null ? supportProjection.Projection : null;
			if (mapImage == null)
				return;
			if (ActualProjection != mapProjection) {
				mapImage.Visibility = Visibility.Collapsed;
				errorElement.DataContext = DXMapStrings.MsgIncorrectMapProjection;
				errorElement.Visibility = Visibility.Visible;
			}
			else {
				errorElement.Visibility = Visibility.Collapsed;
				errorElement.DataContext = null;
				mapImage.Visibility = Visibility.Visible;
			}
		}
		public override void OnApplyTemplate() {
			mapImage = GetTemplateChild("PART_MapImage") as MultiScaleImage;
			if (mapImage != null)
				mapImage.Layer = this;
			errorElement = (FrameworkElement)GetTemplateChild("PART_ErrorPanel");
			base.OnApplyTemplate();
			ApplyViewport();
			UpdateDataProvider(DataProvider);
		}
		public void ClearCache() {
			if(this.mapImage != null)
				this.mapImage.Reset();
		}
	}
}
