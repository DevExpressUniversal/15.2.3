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
namespace DevExpress.Web.ASPxGauges.Gauges.Linear {
	public abstract class LinearGaugeComponentBuilder<T> : BaseComponentBuilder<T>
		where T : class, IComponent, INamed, ISupportAcceptOrder, ISupportLockUpdate {
		protected override void BuildAliases(IDictionary<string, string> aliases) {
			aliases.Add("LinearScale", "ScaleID");
		}
	}
	public class LinearScaleComponentBuilder : LinearGaugeComponentBuilder<LinearScaleComponent> {
		protected override string[] GetExpectedProperties() {
			return new string[] { 
				"Name","ZOrder","Shader",
				"StartPoint","EndPoint",
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
				"OnDataBinding", "OnValueChanged","OnCustomTickmarkText", "OnCustomRescaling"
			};
		}
		protected override void BuildAliases(IDictionary<string, string> aliases) { }
	}
	public class LinearScaleBackgroundLayerComponentBuilder : LinearGaugeComponentBuilder<LinearScaleBackgroundLayerComponent> {
		protected override string[] GetExpectedProperties() {
			return new string[] { 
				"Name","ZOrder","Shader",
				"ScaleID",
				"ScaleStartPos","ScaleEndPos",
				"ShapeType"
			};
		}
	}
	public class LinearScaleLevelComponentBuilder : LinearGaugeComponentBuilder<LinearScaleLevelComponent> {
		protected override string[] GetExpectedProperties() {
			return new string[] { 
				"Name","ZOrder","Shader","Value",
				"ScaleID",
				"ShapeType"
			};
		}
	}
	public class LinearScaleEffectLayerComponentBuilder : LinearGaugeComponentBuilder<LinearScaleEffectLayerComponent> {
		protected override string[] GetExpectedProperties() {
			return new string[] { 
				"Name","ZOrder","Shader",
				"ScaleID",
				"ScaleStartPos","ScaleEndPos",
				"ShapeType"
			};
		}
	}
	public class LinearScaleMarkerComponentBuilder : LinearGaugeComponentBuilder<LinearScaleMarkerComponent> {
		protected override string[] GetExpectedProperties() {
			return new string[] { 
				"Name","ZOrder","Shader","Value",
				"ScaleID",
				"ShapeScale","ShapeOffset",
				"ShapeType"
			};
		}
	}
	public class LinearScaleRangeBarComponentBuilder : LinearGaugeComponentBuilder<LinearScaleRangeBarComponent> {
		protected override string[] GetExpectedProperties() {
			return new string[] { 
				"Name","ZOrder","Shader","Value",
				"ScaleID",
				"AppearanceRangeBar-BorderBrush","AppearanceRangeBar-BorderWidth","AppearanceRangeBar-ContentBrush",
				"StartOffset","EndOffset",
				"AnchorValue"
			};
		}
	}
	public class LinearScaleStateIndicatorComponentBuilder : LinearGaugeComponentBuilder<LinearScaleStateIndicatorComponent> {
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
