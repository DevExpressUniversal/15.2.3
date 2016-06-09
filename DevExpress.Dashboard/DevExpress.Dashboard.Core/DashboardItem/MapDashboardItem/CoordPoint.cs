#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System;
namespace DevExpress.DashboardCommon.Native {
	public class GeoPointEx : CoordPoint {
		public double Latitude { get { return YCoord; } }
		public double Longitude { get { return XCoord; } }
		public GeoPointEx(double y, double x)
			: base(x, y) {
			if (y > 90 || y < -90)
				throw new ArgumentException("Latitude must be inside [-90, 90]");
		}
		protected override CoordPoint CreateNormalized() {
			return new GeoPointEx(YCoord, XCoord);
		}
		public override CoordPoint Offset(double offsetX, double offsetY) {
			return new GeoPointEx(YCoord + offsetY, XCoord + offsetX);
		}
	}
	public class GeoPointFactory : CoordObjectFactory {
		static readonly GeoPointFactory instance = new GeoPointFactory();
		public static GeoPointFactory Instance { get { return instance; } }
		public override CoordPoint CreatePoint(double x, double y) {
			return new GeoPointEx(y, x);
		}
	}
}
