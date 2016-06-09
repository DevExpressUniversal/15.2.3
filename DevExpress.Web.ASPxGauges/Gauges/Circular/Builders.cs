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

using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Web.ASPxGauges.Base;
using DevExpress.XtraGauges.Core.Base;
namespace DevExpress.Web.ASPxGauges.Gauges.Circular {
	public abstract class CircularGaugeComponentBuilder<T> : BaseComponentBuilder<T>
		where T : class, IComponent, INamed, ISupportAcceptOrder, ISupportLockUpdate {
		protected override void BuildAliases(IDictionary<string, string> aliases) {
			aliases.Add("ArcScale", "ScaleID");
		}
	}
	public class ArcScaleComponentBuilder : CircularGaugeComponentBuilder<ArcScaleComponent> {
		protected override string[] GetExpectedProperties() {
			return new string[] { 
				"Name","ZOrder","Shader",
				"Center","RadiusX","RadiusY",
				"StartAngle","EndAngle",
				"MinValue","MaxValue","Value",
				"AutoRescaling", "RescalingThresholdMin", "RescalingThresholdMax", "RescalingBestValues", "Logarithmic","LogarithmicBase", "CustomLogarithmicBase",
				"MinorTickmark-ShapeType","MajorTickmark-ShapeType",
				"MinorTickmark-ShapeScale","MajorTickmark-ShapeScale",
				"MinorTickmark-ShapeOffset","MajorTickmark-ShapeOffset",
				"MinorTickmark-ShowFirst","MajorTickmark-ShowFirst",
				"MinorTickmark-ShowLast","MajorTickmark-ShowLast",
				"MinorTickmark-ShowTick","MajorTickmark-ShowTick",
				"MajorTickmark-FormatString",
				"MajorTickmark-Multiplier", "MajorTickmark-Addend",
				"MajorTickmark-TextOrientation",
				"MajorTickmark-TextOffset",
				"MajorTickmark-AllowTickOverlap",
				"MajorTickmark-ShowText",
				"MinorTickCount","MajorTickCount",
				"AppearanceScale-Brush","AppearanceScale-Width",
				"AppearanceMajorTickmark-BorderBrush","AppearanceMajorTickmark-BorderWidth","AppearanceMajorTickmark-ContentBrush",
				"AppearanceMinorTickmark-BorderBrush","AppearanceMinorTickmark-BorderWidth","AppearanceMinorTickmark-ContentBrush",
				"AppearanceTickmarkTextBackground-BorderBrush","AppearanceTickmarkTextBackground-BorderWidth","AppearanceTickmarkTextBackground-ContentBrush",
				"AppearanceTickmarkText-Font","AppearanceTickmarkText-Format","AppearanceTickmarkText-Spacing","AppearanceTickmarkText-TextBrush",
				"OnDataBinding", "OnValueChanged", "OnCustomTickmarkText", "OnCustomRescaling"
			};
		}
		protected override void BuildAliases(IDictionary<string, string> aliases) { }
	}
	public class ArcScaleBackgroundLayerComponentBuilder : CircularGaugeComponentBuilder<ArcScaleBackgroundLayerComponent> {
		protected override string[] GetExpectedProperties() {
			return new string[] { 
				"Name","ZOrder","Shader",
				"ScaleID",
				"Size","ScaleCenterPos",
				"ShapeType"
			};
		}
	}
	public class ArcScaleEffectLayerComponentBuilder : CircularGaugeComponentBuilder<ArcScaleEffectLayerComponent> {
		protected override string[] GetExpectedProperties() {
			return new string[] { 
				"Name","ZOrder","Shader",
				"ScaleID",
				"Size","ScaleCenterPos",
				"ShapeType"
			};
		}
	}
	public class ArcScaleNeedleComponentBuilder : CircularGaugeComponentBuilder<ArcScaleNeedleComponent> {
		protected override string[] GetExpectedProperties() {
			return new string[] { 
				"Name","ZOrder","Shader","Value",
				"ScaleID",
				"StartOffset","EndOffset",
				"ShapeType"
			};
		}
	}
	public class ArcScaleMarkerComponentBuilder : CircularGaugeComponentBuilder<ArcScaleMarkerComponent> {
		protected override string[] GetExpectedProperties() {
			return new string[] { 
				"Name","ZOrder","Shader","Value",
				"ScaleID",
				"ShapeScale","ShapeOffset",
				"ShapeType"
			};
		}
	}
	public class ArcScaleRangeBarComponentBuilder : CircularGaugeComponentBuilder<ArcScaleRangeBarComponent> {
		protected override string[] GetExpectedProperties() {
			return new string[] { 
				"Name","ZOrder","Shader","Value",
				"ScaleID",
				"AppearanceRangeBar-BorderBrush","AppearanceRangeBar-BorderWidth","AppearanceRangeBar-BackgroundBrush","AppearanceRangeBar-ContentBrush",
				"StartOffset","EndOffset","RoundedCaps","ShowBackground",
				"AnchorValue"
			};
		}
	}
	public class ArcScaleSpindleCapComponentBuilder : CircularGaugeComponentBuilder<ArcScaleSpindleCapComponent> {
		protected override string[] GetExpectedProperties() {
			return new string[] { 
				"Name","ZOrder","Shader",
				"ScaleID",
				"Size",
				"ShapeType"
			};
		}
	}
	public class ArcScaleStateIndicatorComponentBuilder : CircularGaugeComponentBuilder<ArcScaleStateIndicatorComponent> {
		protected override string[] GetExpectedProperties() {
			return new string[] { 
				"Name","ZOrder","Shader",
				"ScaleID",
				"Size","Center",
			};
		}
		protected override void BuildAliases(IDictionary<string, string> aliases) {
			aliases.Add("IndicatorScale", "ScaleID");
		}
	}
}
