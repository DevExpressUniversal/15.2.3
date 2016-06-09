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

namespace DevExpress.XtraGauges.Win.Design {
	public abstract class CircularGaugeComponentDesigner : BaseGaugeComponentDesigner { }
	public class ArcScaleComponentDesigner : CircularGaugeComponentDesigner {
		protected override string[] GetExpectedProperties() {
			return new string[] { 
				"DataBindings", "Tag",
				"Name","ZOrder",
				"Shader",
				"AppearanceScale" , "AppearanceMinorTickmark", "AppearanceMajorTickmark", "AppearanceTickmarkTextBackground", "AppearanceTickmarkText", "UseColorScheme",
				"Center","RadiusX","RadiusY",
				"StartAngle","EndAngle",
				"MinValue","MaxValue","Value","Logarithmic","LogarithmicBase", "CustomLogarithmicBase",
				"AutoRescaling", "RescalingThresholdMin", "RescalingThresholdMax", "RescalingBestValues",
				"MinorTickmark", "MinorTickCount",
				"MajorTickmark", "MajorTickCount",
				"Labels","Ranges"
			};
		}
	}
	public class ArcScaleBackgroundLayerComponentDesigner : CircularGaugeComponentDesigner {
		protected override string[] GetExpectedProperties() {
			return new string[] { 
				"DataBindings", "Tag",
				"Name","ZOrder",
				"Shader",
				"ArcScale",
				"Size","ScaleCenterPos",
				"ShapeType"
			};
		}
	}
	public class ArcScaleNeedleComponentDesigner : CircularGaugeComponentDesigner {
		protected override string[] GetExpectedProperties() {
			return new string[] { 
				"DataBindings", "Tag", "Value",
				"Name","ZOrder",
				"Shader",
				"ArcScale",
				"StartOffset","EndOffset",
				"ShapeType"
			};
		}
	}
	public class ArcScaleSpindleCapComponentDesigner : CircularGaugeComponentDesigner {
		protected override string[] GetExpectedProperties() {
			return new string[] { 
				"DataBindings", "Tag",
				"Name","ZOrder",
				"Shader",
				"ArcScale",
				"Size",
				"ShapeType"
			};
		}
	}
	public class ArcScaleMarkerComponentDesigner : CircularGaugeComponentDesigner {
		protected override string[] GetExpectedProperties() {
			return new string[] { 
				"DataBindings", "Tag", "Value",
				"Name","ZOrder",
				"Shader",
				"ArcScale",
				"ShapeScale",
				"ShapeOffset",
				"ShapeType"
			};
		}
	}
	public class ArcScaleRangeBarComponentDesigner : CircularGaugeComponentDesigner {
		protected override string[] GetExpectedProperties() {
			return new string[] { 
				"DataBindings", "Tag", "Value",
				"Name","ZOrder",
				"Shader",
				"ArcScale",
				"AppearanceRangeBar",
				"StartOffset","EndOffset",
				"AnchorValue",
				"RoundedCaps",
				"ShowBackground",
				"StartAngle",
				"EndAngle"
			};
		}
	}
	public class ArcScaleEffectLayerComponentDesigner : CircularGaugeComponentDesigner {
		protected override string[] GetExpectedProperties() {
			return new string[] { 
				"DataBindings", "Tag",
				"Name","ZOrder",
				"Shader",
				"ArcScale",
				"Size","ScaleCenterPos",
				"ShapeType"
			};
		}
	}
	public class ArcScaleStateIndicatorComponentDesigner : CircularGaugeComponentDesigner {
		protected override string[] GetExpectedProperties() {
			return new string[] { 
				"DataBindings", "Tag",
				"Name","ZOrder",
				"Shader",
				"IndicatorScale",
				"Size","Center",
				"States"
			};
		}
	}
	public class StateImageIndicatorComponentDesigner : CircularGaugeComponentDesigner {
		protected override string[] GetExpectedProperties() {
			return new string[] { 
				"DataBindings", "Tag",
				"Name","ZOrder",
				"Shader",
				"AppearanceBackground" ,
				"Position","Size", "Image", "ImageLayoutMode", "StateImages", "StateIndex", "AllowImageSkinning", "ImageStateCollection", "IndicatorScale", "Color"
			};
		}
	}
}
