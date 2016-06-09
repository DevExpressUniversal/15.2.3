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
using System.Drawing.Design;
using DevExpress.XtraMap.Native;
using DevExpress.Map.Native;
using DevExpress.Map;
namespace DevExpress.XtraMap {
	public class SvgFileDataAdapter : FileDataAdapterBase {
		CoordPoint boundaryPoint1;
		CoordPoint boundaryPoint2;
		protected new WinSvgFileLoader InnerLoader { get { return (WinSvgFileLoader)base.InnerLoader; } }
		[
		Category(SRCategoryNames.Data), DefaultValue(null),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		TypeConverter("DevExpress.XtraMap.Design.ExpandableNoneStringSupportedTypeConverter," + AssemblyInfo.SRAssemblyMapDesign),
		Editor("DevExpress.XtraMap.Design.SvgFileUrlEditor," + AssemblyInfo.SRAssemblyMapDesign, typeof(UITypeEditor)),
		RefreshProperties(RefreshProperties.All)
		]
		public new Uri FileUri {
			get { return base.FileUri; }
			set { base.FileUri = value; }
		}
		[
		Category(SRCategoryNames.Data),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public new CartesianSourceCoordinateSystem SourceCoordinateSystem {
			get { return (CartesianSourceCoordinateSystem)base.SourceCoordinateSystem; }
			set { base.SourceCoordinateSystem = value; }
		}
		[
		DefaultValue(null),
		Category(SRCategoryNames.Behavior),
		TypeConverter("DevExpress.XtraMap.Design.CoordPointTypeConverter," + "DevExpress.XtraMap" + AssemblyInfo.VSuffixDesign),
		]
		public CoordPoint BoundaryPoint1 {
			get { return boundaryPoint1; }
			set {
				if (boundaryPoint1 == value) return;
				boundaryPoint1 = value; 
				OnPropertyChanged(); 
			}
		}
		[
		DefaultValue(null),
		Category(SRCategoryNames.Behavior),
		TypeConverter("DevExpress.XtraMap.Design.CoordPointTypeConverter," + "DevExpress.XtraMap" + AssemblyInfo.VSuffixDesign),
		]
		public CoordPoint BoundaryPoint2 { 
			get {return boundaryPoint2; }
			set {
				if (boundaryPoint2 == value) return;
				boundaryPoint2 = value;
				OnPropertyChanged();
			}
		}
		void ResetSourceCoordinateSystem() {
		}
		bool ShouldSerializeSourceCoordinateSystem() {
			return false;
		}
		protected override SourceCoordinateSystem CreateDefaultCoordinateSystem() {
			return new CartesianSourceCoordinateSystem();
		}
		protected override MapLoaderCore<MapItem> CreateInnerLoader() {
			return new WinSvgFileLoader();
		}
		protected override bool IsCSCompatibleTo(MapCoordinateSystem mapCS) {
			return true;
		}
		protected override void OnBoundsCalculated(object sender, BoundsCalculatedEventArgs e) {
			base.OnBoundsCalculated(sender, e);
			if (InnerLoader != null) {
				WinSvgPointConverterBase pointConverter = CreateSvgPointConverter(e.Bounds);
				pointConverter.SetPrintingBounds(BoundaryPoint1, BoundaryPoint2);
				InnerLoader.PointConverter = pointConverter;
			}
		}
		protected internal WinSvgPointConverterBase CreateSvgPointConverter(CoordBounds bounds) {
			InnerMap map = Layer != null ? Layer.Map : null;
			if(map != null && map.CoordinateSystem.PointType == CoordPointType.Geo)
				return new WinSvgGeoPointConverter(map.CoordinateSystem, Layer, bounds);
			return new WinSvgCartesianPointConverter(map != null ? map.CoordinateSystem : MapUtils.SvgDefaultCoordinateSystem, Layer, bounds);
		}
		public override string ToString() {
			return "(SvgFileDataAdapter)";
		}
	}
}
