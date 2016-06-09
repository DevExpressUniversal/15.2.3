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
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
namespace DevExpress.Xpf.Map {
	public class SvgFileDataAdapter : MapDataAdapterBase {
		public static readonly DependencyProperty FileUriProperty = DependencyPropertyManager.Register("FileUri",
			typeof(Uri), typeof(SvgFileDataAdapter), new PropertyMetadata(null, OnPropertyChanged));
		public static readonly DependencyProperty BoundaryPoint1Property = DependencyPropertyManager.Register("BoundaryPoint1",
			typeof(CoordPoint), typeof(SvgFileDataAdapter), new PropertyMetadata(null, OnPropertyChanged));
		public static readonly DependencyProperty BoundaryPoint2Property = DependencyPropertyManager.Register("BoundaryPoint2",
			typeof(CoordPoint), typeof(SvgFileDataAdapter), new PropertyMetadata(null, OnPropertyChanged));
		[Category(Categories.Data)]
		public Uri FileUri {
			get { return (Uri)GetValue(FileUriProperty); }
			set { SetValue(FileUriProperty, value); }
		}
		[Category(Categories.Behavior), TypeConverter(typeof(GeoPointConverter))]
		public CoordPoint BoundaryPoint1 {
			get { return (CoordPoint)GetValue(BoundaryPoint1Property); }
			set { SetValue(BoundaryPoint1Property, value); }
		}
		[Category(Categories.Behavior), TypeConverter(typeof(GeoPointConverter))]
		public CoordPoint BoundaryPoint2 {
			get { return (CoordPoint)GetValue(BoundaryPoint2Property); }
			set { SetValue(BoundaryPoint2Property, value); }
		}
		static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SvgFileDataAdapter dataAdapter = d as SvgFileDataAdapter;
			if (dataAdapter != null)
				dataAdapter.LoadDataInternal();
		}
		public event ShapesLoadedEventHandler ShapesLoaded;
		readonly IFileDataAdapter fileAdapter;
		protected XpfSvgFileLoader ItemsLoader { get { return (XpfSvgFileLoader)fileAdapter.ItemsLoader; } }
		protected internal override MapVectorItemCollection ItemsCollection { get { return fileAdapter.ItemsCollection; } }
		protected override bool CanLoadData { get { return base.CanLoadData && FileUri != null; } }
		public SvgFileDataAdapter() {
			fileAdapter = new FileDataAdapter(new XpfSvgFileLoader());
			fileAdapter.ItemsLoaded += ItemsLoaded;
			fileAdapter.BoundsCalculated += BoundsCalculated;
		}
		void ItemsLoaded(object sender, ItemsLoadedEventArgs<MapItem> e) {
			if (ShapesLoaded != null)
				ShapesLoaded(this, new ShapesLoadedEventArgs(ItemsLoader.Items));
		}
		void BoundsCalculated(object sender, BoundsCalculatedEventArgs e) {
			if (ItemsLoader != null) {
				XpfSvgPointConverterBase pointConverter = CreateSvgPointConverter(e.Bounds);
				pointConverter.SetPrintingBounds(BoundaryPoint1, BoundaryPoint2);
				ItemsLoader.PointConverter = pointConverter;
			}
		}
#if DEBUGTEST
		internal void LoadItems(Stream stream) {
			ItemsLoader.LoadFormStream(stream);
		}
#endif
		protected override MapDependencyObject CreateObject() {
			return new SvgFileDataAdapter();
		}
		protected override void LoadDataCore() {
			if (FileUri != null)
				ItemsLoader.Load(FileUri, DesignerProperties.GetIsInDesignMode(this));
		}
		protected internal XpfSvgPointConverterBase CreateSvgPointConverter(CoordBounds bounds) {
			MapControl map = Layer != null ? Layer.Map : null;
			if(map != null && map.CoordinateSystem.PointType == CoordPointType.Geo)
				return new XpfSvgGeoPointConverter(map.CoordinateSystem, Layer, bounds);
			return new XpfSvgCartesianPointConverter(map != null ? map.CoordinateSystem : CommonUtils.SvgDefaultCoordinateSystem, Layer, bounds);
		}
		protected internal override bool IsCSCompatibleTo(MapCoordinateSystem coordinateSystem) {
			return true;
		}
		public override object GetItemSourceObject(MapItem item) {
			return item;
		}
	}
}
