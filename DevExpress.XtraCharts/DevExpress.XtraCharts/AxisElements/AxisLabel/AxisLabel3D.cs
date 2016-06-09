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

using System.Drawing;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile),
	]
	public enum AxisLabel3DPosition {
		Bottom = NearTextPosition.Bottom,
		Left = NearTextPosition.Left,
		Right = NearTextPosition.Right,
		Top = NearTextPosition.Top,
		Auto
	}
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class AxisLabel3D : AxisLabel {
		const AxisLabel3DPosition DefaultPosition = AxisLabel3DPosition.Auto;
		AxisLabel3DPosition position = DefaultPosition;
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AxisLabel3DPosition"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.AxisLabel3D.Position"),
		XtraSerializableProperty
		]
		public AxisLabel3DPosition Position { 
			get { return position; }
			set {
				if (value != position) {
					SendNotification(new ElementWillChangeNotification(this));
					position = value;
					RaiseControlChanged();
				}
			}
		}
		public AxisLabel3D(Axis3D axis) : base(axis) {
		}
		#region ShouldSerialize & Reset
		bool ShouldSerializePosition() {
			return position != DefaultPosition;
		}
		void ResetPosition() {
			Position = DefaultPosition;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializePosition();
		}
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			return propertyName == "Position" ? ShouldSerializePosition() : base.XtraShouldSerialize(propertyName);
		}
		#endregion
		protected override Color GetTextColor(IChartAppearance actualAppearance) {
			return actualAppearance.XYDiagram3DAppearance.LabelsColor;
		}
		public override string ToString() {
			return "(AxisLabel3D)";
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			AxisLabel3D label = obj as AxisLabel3D;
			if (label != null)
				position = label.position;
		}
	}
}
