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
using System.Drawing.Design;
using DevExpress.Map;
using DevExpress.Map.Native;
using DevExpress.XtraMap.Native;
namespace DevExpress.XtraMap {
	public abstract class SourceCoordinateSystem : IOwnedElement {
		CoordSystemCore coordSystemCore;
		object owner;
		protected CoordinateSystemDataAdapterBase Owner { get { return owner as CoordinateSystemDataAdapterBase; } }
		[DefaultValue(null),
		TypeConverter("DevExpress.XtraMap.Design.ExpandableObjectConverterShowsValueTypeNameInParentheses," + AssemblyInfo.SRAssemblyMapDesign),
		Editor("DevExpress.XtraMap.Design.CoordinateConverterPickerEditor," + AssemblyInfo.SRAssemblyMapDesign, typeof(UITypeEditor)),
#if !SL
	DevExpressXtraMapLocalizedDescription("SourceCoordinateSystemCoordinateConverter")
#else
	Description("")
#endif
]
		public ICoordPointConverter CoordinateConverter {
			get { return CoordSystemCore.CoordinateConverter; }
			set {
				if(object.Equals(CoordSystemCore.CoordinateConverter, value))
					return;
				CoordSystemCore.CoordinateConverter = value;
				OnPropertyChanged();
			}
		}
		protected internal CoordSystemCore CoordSystemCore {
			get {
				if (coordSystemCore == null)
					coordSystemCore = CreateCoreCoordinateSystem();
				return coordSystemCore;
			}
		}
		protected internal bool IsDefault { get { return CoordSystemCore.IsDefault; } }
		protected abstract CoordSystemCore CreateCoreCoordinateSystem();
		protected abstract CoordPointType SupportedPointType { get; }
		protected SourceCoordinateSystem() {
		}
		protected SourceCoordinateSystem(ICoordPointConverter converter) {
			CoordinateConverter = GetCoordinateConverter(converter);
		}
		ICoordPointConverter GetCoordinateConverter(ICoordPointConverter converter) {
			CoordinateConverterCore coreConverter = converter as CoordinateConverterCore;
			if (coreConverter == null)
				return converter;
			return CoordinateConverterCoreWrapper.CreateConverterByInnerConverter(coreConverter);
		}
		#region IOwnedElement members
		object IOwnedElement.Owner {
			get { return owner; }
			set { owner = value; }
		}
		#endregion
		internal CoordPointType GetSourcePointType() {
			CoordinateConverterBase conv = CoordinateConverter as CoordinateConverterBase;
			if (conv != null)
				return conv.DesinationPointType;
			return SupportedPointType;
		}
		protected internal abstract bool IsCompatibleTo(CoordPointType destinationPointType);
		protected void OnPropertyChanged() {
			if (Owner != null)
				Owner.OnPropertyChanged();
		}
		public CoordPoint CreatePoint(double x, double y) {
			return CoordSystemCore.CreatePoint(x, y);
		}
	}
	public class GeoSourceCoordinateSystem : SourceCoordinateSystem {
		protected override CoordSystemCore CreateCoreCoordinateSystem() {
			return new GeoCoordSystemCore(GeoPointFactory.Instance);
		}
		protected override CoordPointType SupportedPointType { get { return CoordPointType.Geo; } }
		protected internal GeoSourceCoordinateSystem(GeoCoordSystemCore coordSystemCore)
			: base(coordSystemCore.CoordinateConverter) {
		}
		public GeoSourceCoordinateSystem() {
		}
		protected internal override bool IsCompatibleTo(CoordPointType destinationPointType) {
			return destinationPointType == CoordPointType.Geo;
		}
		public override string ToString() {
			return "(GeoSourceCoordinateSystem)";
		}
	}
	public class CartesianSourceCoordinateSystem : SourceCoordinateSystem {
		static readonly MeasureUnit DefaultMeasureUnit = CartesianCoordSystemCore.DefaultMeasureUnit;
		protected CartesianCoordSystemCore CartesianCoordSystemCore { get { return (CartesianCoordSystemCore)base.CoordSystemCore; } }
#if !SL
	[DevExpressXtraMapLocalizedDescription("CartesianSourceCoordinateSystemMeasureUnit")]
#endif
		public MeasureUnit MeasureUnit { 
			get { return CartesianCoordSystemCore.MeasureUnit; } 
			set {
				if (CartesianCoordSystemCore.MeasureUnit == value)
					return;
				CartesianCoordSystemCore.MeasureUnit = value;
				OnPropertyChanged();
			} 
		}
		bool ShouldSerializeMeasureUnit() { return MeasureUnit != DefaultMeasureUnit; }
		void ResetMeasureUnit() { MeasureUnit = DefaultMeasureUnit; }
		protected internal CartesianSourceCoordinateSystem(CartesianCoordSystemCore coordSystemCore) : base(coordSystemCore.CoordinateConverter) {
			MeasureUnit = coordSystemCore.MeasureUnit;
		}
		public CartesianSourceCoordinateSystem() {
			MeasureUnit = DefaultMeasureUnit;
		}
		protected override CoordSystemCore CreateCoreCoordinateSystem() {
			return new CartesianCoordSystemCore(CartesianPointFactory.Instance);
		}
		protected override CoordPointType SupportedPointType { get { return CoordPointType.Cartesian; } }
		protected internal override bool IsCompatibleTo(CoordPointType destinationPointType) {
			return destinationPointType == CoordPointType.Cartesian;
		}
		public override string ToString() {
			return "(CartesianSourceCoordinateSystem)";
		}
	}
}
