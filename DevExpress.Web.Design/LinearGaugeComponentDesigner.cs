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
using System.Collections.Generic;
using System.Text;
using DevExpress.Web.ASPxGauges.Design.Base;
namespace DevExpress.Web.ASPxGauges.Design {
	public abstract class LinearGaugeComponentDesigner :
		BaseGaugeComponentDesigner {
	}
	public class LinearScaleComponentDesigner : LinearGaugeComponentDesigner {
		protected override string[] GetExpectedProperties() {
			return new string[] { 
				"Name","ZOrder",
				"Shader",
				"AppearanceScale" , "AppearanceMinorTickmark", "AppearanceMajorTickmark", "AppearanceTickmarkTextBackground", "AppearanceTickmarkText",
				"StartPoint","EndPoint",
				"MinValue","MaxValue","Value","Logarithmic","LogarithmicBase", "CustomLogarithmicBase",
				"MinorTickmark", "MinorTickCount",
				"MajorTickmark", "MajorTickCount",
				"Ranges","Labels"
			};
		}
	}
	public class LinearScaleBackgroundLayerComponentDesigner : LinearGaugeComponentDesigner {
		protected override string[] GetExpectedProperties() {
			return new string[] { 
				"Name","ZOrder",
				"Shader",
				"ScaleID",
				"Size","ScaleStartPos","ScaleEndPos",
				"ShapeType"
			};
		}
	}
	public class LinearScaleEffectLayerComponentDesigner : LinearGaugeComponentDesigner {
		protected override string[] GetExpectedProperties() {
			return new string[] { 
				"Name","ZOrder",
				"Shader",
				"ScaleID",
				"Size","ScaleStartPos","ScaleEndPos",
				"ShapeType"
			};
		}
	}
	public class LinearScaleLevelComponentDesigner : LinearGaugeComponentDesigner {
		protected override string[] GetExpectedProperties() {
			return new string[] { 
				"Name","ZOrder","Value",
				"Shader",
				"ScaleID",
				"ShapeType"
			};
		}
	}
	public class LinearScaleMarkerComponentDesigner : LinearGaugeComponentDesigner {
		protected override string[] GetExpectedProperties() {
			return new string[] { 
				"Name","ZOrder","Value",
				"Shader",
				"ScaleID",
				"ShapeScale",
				"ShapeOffset",
				"ShapeType"
			};
		}
	}
	public class LinearScaleRangeBarComponentDesigner : LinearGaugeComponentDesigner {
		protected override string[] GetExpectedProperties() {
			return new string[] { 
				"Name","ZOrder","Value",
				"Shader",
				"ScaleID",
				"AppearanceRangeBar",
				"StartOffset","EndOffset",
				"AnchorValue"
			};
		}
	}
	public class LinearScaleStateIndicatorComponentDesigner : LinearGaugeComponentDesigner {
		protected override string[] GetExpectedProperties() {
			return new string[] { 
				"Name","ZOrder",
				"Shader",
				"ScaleID",
				"Center","Size",
				"States"
			};
		}
	}
}
