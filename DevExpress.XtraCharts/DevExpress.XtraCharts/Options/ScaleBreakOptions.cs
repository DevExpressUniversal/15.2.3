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
using System.Drawing;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Design;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile),
	]
	public enum ScaleBreakStyle {
		Straight,
		Ragged,
		Waved
	}
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design"),
	TypeConverter(typeof(ScaleBreakOptionsTypeConverter))
	]
	public class ScaleBreakOptions : ChartElement {
		const int DefaultSizeInPixels = 4;
		const ScaleBreakStyle DefaultStyle = ScaleBreakStyle.Ragged;
		static readonly Color DefaultColor = Color.Empty;
		int sizeInPixels = DefaultSizeInPixels;
		ScaleBreakStyle style = DefaultStyle;
		Color color = DefaultColor;
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ScaleBreakOptionsSizeInPixels"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ScaleBreakOptions.SizeInPixels"),
		XtraSerializableProperty
		]
		public int SizeInPixels {
			get { return sizeInPixels; }
			set {
				if (value != sizeInPixels) {
					if (value < -1 || value > 50)
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgInvalidSizeInPixels));
					SendNotification(new ElementWillChangeNotification(this));
					sizeInPixels = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ScaleBreakOptionsStyle"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ScaleBreakOptions.Style"),
		XtraSerializableProperty
		]
		public ScaleBreakStyle Style {
			get { return style; }
			set {
				if (value != style) {
					SendNotification(new ElementWillChangeNotification(this));
					style = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ScaleBreakOptionsColor"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ScaleBreakOptions.Color"),
		XtraSerializableProperty
		]
		public Color Color {
			get { return color; }
			set {
				if (value != color) {
					SendNotification(new ElementWillChangeNotification(this));
					color = value;
					RaiseControlChanged();
				}
			}
		}
		internal ScaleBreakOptions(Axis axis) : base(axis) { }
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			if(propertyName == "SizeInPixels")
				return ShouldSerializeSizeInPixels();
			if(propertyName == "Style")
				return ShouldSerializeStyle();
			if(propertyName == "Color")
				return ShouldSerializeColor();
			return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeSizeInPixels() {
			return sizeInPixels != DefaultSizeInPixels;
		}
		void ResetSizeInPixels() {
			SizeInPixels = DefaultSizeInPixels;
		}
		bool ShouldSerializeStyle() {
			return style != DefaultStyle;
		}
		void ResetStyle() {
			Style = DefaultStyle;
		}
		bool ShouldSerializeColor() {
			return color != DefaultColor;
		}
		void ResetColor() {
			Color = DefaultColor;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializeSizeInPixels() || ShouldSerializeStyle() || ShouldSerializeColor();
		}
		#endregion
		protected override ChartElement CreateObjectForClone() {
			return new ScaleBreakOptions(null);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			ScaleBreakOptions options = obj as ScaleBreakOptions;
			if(options == null)
				return;
			sizeInPixels = options.sizeInPixels;
			style = options.style;
			color = options.color;
		}
	}
}
