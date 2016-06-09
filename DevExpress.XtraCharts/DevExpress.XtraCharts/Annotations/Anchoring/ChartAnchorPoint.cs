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
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Reflection;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(ChartAnchorPointTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class ChartAnchorPoint : AnnotationAnchorPoint {
		const int DefaultX = 0;
		const int DefaultY = 0;
		int x;
		int y;
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ChartAnchorPointX"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ChartAnchorPoint.X"),
		XtraSerializableProperty,
		]
		public int X {
			get { return x; }
			set {
				if (value == x)
					return;
				SendNotification(new ElementWillChangeNotification(this));
				x = value;
				RaiseControlChanged();
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ChartAnchorPointY"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ChartAnchorPoint.Y"),
		XtraSerializableProperty,
		]
		public int Y {
			get { return y; }
			set {
				if (y == value)
					return;
				SendNotification(new ElementWillChangeNotification(this));
				y = value;
				RaiseControlChanged();
			}
		}
		public ChartAnchorPoint() : this(DefaultX, DefaultY) {
		}
		public ChartAnchorPoint(int x, int y) {
			this.x = x;
			this.y = y;
		}
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "X":
					return ShouldSerializeX();
				case "Y":
					return ShouldSerializeY();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeX() {
			return x != DefaultX;
		}
		void ResetX() {
			X = DefaultX;
		}
		bool ShouldSerializeY() {
			return y != DefaultY;
		}
		void ResetY() {
			Y = DefaultY;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() ||ShouldSerializeX() || ShouldSerializeY();
		}
		#endregion
		protected override ChartElement CreateObjectForClone() {
			return new ChartAnchorPoint();
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			ChartAnchorPoint anchorPoint = obj as ChartAnchorPoint;
			if (anchorPoint == null)
				return;
			x = anchorPoint.x;
			y = anchorPoint.y;
		}
		protected internal override DiagramPoint GetAnchorPoint(ZPlaneRectangle viewport) {
			return new DiagramPoint(viewport.Location.X + x, viewport.Location.Y + y);
		}
		internal void SetPosition(int x, int y) {
			this.x = x;
			this.y = y;
		}
	}
}
