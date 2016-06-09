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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
namespace DevExpress.Xpf.Map {
	public abstract class SourceCoordinateSystem : MapDependencyObject {
		CoordSystemCore coordSystemCore;
		public static readonly DependencyProperty CoordinateConverterProperty = DependencyPropertyManager.Register("CoordinateConverter",
			typeof(CoordinateConverterBase), typeof(SourceCoordinateSystem), new PropertyMetadata(null, CoordConverterChanged));
		[Category(Categories.Data)]
		public CoordinateConverterBase CoordinateConverter {
			get { return (CoordinateConverterBase)GetValue(CoordinateConverterProperty); }
			set { SetValue(CoordinateConverterProperty, value); }
		}
		protected internal CoordSystemCore CoordSystemCore {
			get {
				if (coordSystemCore == null)
					coordSystemCore = CreateCoreCoordinateSystem();
				return coordSystemCore;
			}
		}
		protected internal bool IsDefault { get { return CoordSystemCore.IsDefault; } }
		protected abstract CoordPointType SupportedPointType { get; }
		static void CoordConverterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SourceCoordinateSystem cs = (SourceCoordinateSystem)d;
			cs.CoordConverterChanged((CoordinateConverterBase)e.NewValue);
		}
		protected void CoordConverterChanged(CoordinateConverterBase conv) {
			CoordSystemCore.CoordinateConverter = conv;
		}
		protected abstract CoordSystemCore CreateCoreCoordinateSystem();		
		protected SourceCoordinateSystem(CoordSystemCore coordSystemCore) {
			this.coordSystemCore = coordSystemCore;
		}
		internal CoordPointType GetSourcePointType() {
			CoordinateConverterBase conv = CoordinateConverter as CoordinateConverterBase;
			if (conv != null)
				return conv.DesinationPointType;
			return SupportedPointType;
		}
		public CoordPoint CreatePoint(double x, double y) {
			return CoordSystemCore.CreatePoint(x, y);
		}
	}
	public class CartesianSourceCoordinateSystem : SourceCoordinateSystem {
		public static readonly DependencyProperty MeasureUnitProperty = DependencyPropertyManager.Register("MeasureUnit",
			typeof(MeasureUnit), typeof(CartesianSourceCoordinateSystem), new PropertyMetadata(MeasureUnit.Meter, MeasureUnitChanged));
		[Category(Categories.Data)]
		public MeasureUnit MeasureUnit {
			get { return (MeasureUnit)GetValue(MeasureUnitProperty); }
			set { SetValue(MeasureUnitProperty, value); }
		}
		static void MeasureUnitChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			CartesianSourceCoordinateSystem cs = (CartesianSourceCoordinateSystem)d;
			cs.MeasureUnitChanged((MeasureUnit)e.NewValue);
		}
		protected void MeasureUnitChanged(MeasureUnit measureUnit) {
			CartesianCoordSystemCore.MeasureUnit = measureUnit;
		}
		protected CartesianCoordSystemCore CartesianCoordSystemCore { get { return (CartesianCoordSystemCore)base.CoordSystemCore; } }
		protected override CoordPointType SupportedPointType { get { return CoordPointType.Cartesian; } }
		protected internal CartesianSourceCoordinateSystem(CartesianCoordSystemCore coordSystemCore)
			: base(coordSystemCore) {
		}
		public CartesianSourceCoordinateSystem()
			: base(null) {
		}
		protected override CoordSystemCore CreateCoreCoordinateSystem() {
			return new CartesianCoordSystemCore(CartesianPointFactory.Instance);
		}
		protected override MapDependencyObject CreateObject() {
			return new CartesianSourceCoordinateSystem();
		}
	}
	public class GeoSourceCoordinateSystem : SourceCoordinateSystem {
		protected override CoordPointType SupportedPointType { get { return CoordPointType.Geo; } }
		protected internal GeoSourceCoordinateSystem(GeoCoordSystemCore coordSystemCore)
			: base(coordSystemCore) {
		}
		public GeoSourceCoordinateSystem()
			: base(null) {
		}
		protected override CoordSystemCore CreateCoreCoordinateSystem() {
			return new GeoCoordSystemCore(GeoPointFactory.Instance);
		}
		protected override MapDependencyObject CreateObject() {
			return new GeoSourceCoordinateSystem();
		}
	}
}
