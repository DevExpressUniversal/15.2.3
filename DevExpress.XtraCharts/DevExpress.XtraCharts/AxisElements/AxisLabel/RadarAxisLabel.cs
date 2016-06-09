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
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
using System.ComponentModel.Design.Serialization;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile),
	]
	public enum RadarAxisXLabelTextDirection {
		LeftToRight,
		TopToBottom,
		BottomToTop,
		Radial,
		Tangent
	}
	public class RadarAxisXLabel : AxisLabel {
		const RadarAxisXLabelTextDirection DefaultTextDirection = RadarAxisXLabelTextDirection.LeftToRight;
		RadarAxisXLabelTextDirection textDirection = DefaultTextDirection;
		protected override bool DefaultAntialiasing {
			get { return textDirection.Equals(RadarAxisXLabelTextDirection.Radial) || textDirection.Equals(RadarAxisXLabelTextDirection.Tangent); }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public new bool Staggered { get { return false; } set { } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public new int Angle { get { return 0; } set { } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RadarAxisXLabelTextDirection"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.RadarAxisXLabel.TextDirection"),
		Category("Appearance"),
		XtraSerializableProperty
		]
		public RadarAxisXLabelTextDirection TextDirection {
			get { return textDirection; }
			set {
				if (value != textDirection) {
					SendNotification(new ElementWillChangeNotification(this));
					textDirection = value;
					RaiseControlChanged();
				}
			}
		}
		public RadarAxisXLabel(RadarAxisX axis) : base(axis) {
		}
		#region ShouldSerialize & Reset
		bool ShouldSerializeTextDirection() {
			return textDirection != DefaultTextDirection;
		}
		void ResetTextDirection() {
			TextDirection = DefaultTextDirection;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializeTextDirection();
		}
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			return propertyName == "TextDirection" ? ShouldSerializeTextDirection() : base.XtraShouldSerialize(propertyName);
		}
		#endregion
		protected override ChartElement CreateObjectForClone() {
			return new RadarAxisXLabel(null);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			RadarAxisXLabel label = obj as RadarAxisXLabel;
			if (label != null)
				textDirection = label.textDirection;
		}
		public override string ToString() {
			return "(RadarAxisXLabel)";
		}
	}
	public class RadarAxisYLabel : AxisLabel {
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public new bool Staggered { get { return false; } set { } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public new int Angle { get { return 0; } set { } }
		public RadarAxisYLabel(RadarAxisY axis)
			: base(axis) {
		}
		protected override ChartElement CreateObjectForClone() {
			return new RadarAxisYLabel(null);
		}
		public override string ToString() {
			return "(RadarAxisYLabel)";
		}
	}
}
