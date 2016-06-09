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

using DevExpress.XtraPrinting.Native;
using System;
using System.Drawing;
using DevExpress.XtraPrinting.Shape.Native;
using System.ComponentModel;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPrinting.Localization;
namespace DevExpress.XtraPrinting.Shape {
	public class ShapePolygon : ShapePolygonBase {
		internal override PreviewStringId ShapeStringId {
			get { return PreviewStringId.Shapes_Polygon; }
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("ShapePolygonNumberOfSides"),
#endif
	   DXDisplayNameAttribute(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.Shape.ShapePolygon.NumberOfSides"),
		DefaultValue(3),
		NotifyParentProperty(true),
		XtraSerializableProperty,
		]
		public new int NumberOfSides {
			get { return base.NumberOfSides; }
			set { base.NumberOfSides = value; }
		}
		public ShapePolygon() {
		}
		ShapePolygon(ShapePolygon source)
			: base(source) {
		}
		protected override ShapeBase CloneShape() {
			return new ShapePolygon(this);
		}
		protected internal override PointF[] CreatePoints(RectangleF bounds, int angle) {
			return CreatePointsCore(bounds, 0, 1);
		}
	}
}
