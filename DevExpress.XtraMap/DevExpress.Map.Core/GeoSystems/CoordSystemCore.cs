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

using DevExpress.Utils;
using System;
namespace DevExpress.Map.Native {
	public abstract class CoordSystemCore {
		CoordObjectFactory pointFactory;
		ICoordPointConverter coordinateConverter;
		protected CoordObjectFactory PointFactory { get { return pointFactory; } }
		public ICoordPointConverter CoordinateConverter { get { return coordinateConverter; } set { coordinateConverter = value; } }
		public virtual bool IsDefault { get { return false; } }
		protected CoordSystemCore(CoordObjectFactory factory, ICoordPointConverter converter) {
			Guard.ArgumentNotNull(factory, "factory");
			this.pointFactory = factory;
			this.coordinateConverter = converter;
		}
		protected CoordSystemCore(CoordObjectFactory factory) 
			:this(factory, null){ }
		public virtual CoordPoint CreatePoint(double x, double y) {
			CoordPoint point = PointFactory.CreatePoint(x, y);
			return CoordinateConverter == null ? point : CoordinateConverter.Convert(point);
		}
	}
	public class GeoCoordSystemCore : CoordSystemCore {
		public override bool IsDefault { get { return CoordinateConverter == null; } }
		public GeoCoordSystemCore(CoordObjectFactory factory, ICoordPointConverter converter)
			: base(factory, converter) { 
		}
		public GeoCoordSystemCore(CoordObjectFactory factory) : this(factory, null) { 
		}
		public override CoordPoint CreatePoint(double x, double y) {
			y = Math.Max(Math.Min(90, y), -90);
			return base.CreatePoint(x, y);
		}
	}
	public class CartesianCoordSystemCore : CoordSystemCore {
		static readonly MeasureUnitCore defaultMeasureUnit = MeasureUnitCore.Meter;
		public static MeasureUnitCore DefaultMeasureUnit { get { return defaultMeasureUnit; } }
		public MeasureUnitCore MeasureUnit {  get; set; }
		public CartesianCoordSystemCore(CoordObjectFactory factory, ICoordPointConverter converter, MeasureUnitCore unit) : base(factory, converter) {
			MeasureUnit = unit;
		}
		public CartesianCoordSystemCore(CoordObjectFactory factory, MeasureUnitCore unit) : this(factory, null, unit) { 
		}
		public CartesianCoordSystemCore(CoordObjectFactory factory, ICoordPointConverter converter) : this(factory, converter, DefaultMeasureUnit) { 
		}
		public CartesianCoordSystemCore(CoordObjectFactory factory) : this(factory, null, DefaultMeasureUnit) {
		}
		public override CoordPoint CreatePoint(double x, double y) {
			double xMeters = MeasureUnit.ToMeters(x);
			double yMeters = MeasureUnit.ToMeters(y);
			return base.CreatePoint(xMeters, yMeters);
		}
	}
}
