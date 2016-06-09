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
using DevExpress.XtraGauges.Core.Model;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.Web.ASPxGauges.Gauges;
using DevExpress.Web.ASPxGauges.Gauges.Circular;
using DevExpress.Web.ASPxGauges.Gauges.Linear;
using DevExpress.Web.ASPxGauges.Gauges.State;
using DevExpress.Web.ASPxGauges.Gauges.Digital;
using DevExpress.XtraGauges.Base;
using DevExpress.XtraGauges.Core.Drawing;
namespace DevExpress.Web.ASPxGauges.Base {
	public interface IWebGauge {
		LabelComponentCollection Labels { get;}
	}
	public interface ICircularGauge : IWebGauge {
		ArcScaleBackgroundLayerComponentCollection BackgroundLayers { get;}
		ArcScaleEffectLayerComponentCollection EffectLayers { get;}
		ArcScaleComponentCollection Scales { get;}
		ArcScaleNeedleComponentCollection Needles { get;}
		ArcScaleRangeBarComponentCollection RangeBars { get;}
		ArcScaleMarkerComponentCollection Markers { get;}
		ArcScaleSpindleCapComponentCollection SpindleCaps { get;}
		ArcScaleStateIndicatorComponentCollection Indicators { get;}
	}
	public interface ILinearGauge : IWebGauge {
		ScaleOrientation Orientation { get;set;}
		LinearScaleComponentCollection Scales { get;}
		LinearScaleBackgroundLayerComponentCollection BackgroundLayers { get;}
		LinearScaleEffectLayerComponentCollection EffectLayers { get;}
		LinearScaleLevelComponentCollection Levels { get;}
		LinearScaleMarkerComponentCollection Markers { get;}
		LinearScaleRangeBarComponentCollection RangeBars { get;}
		LinearScaleStateIndicatorComponentCollection Indicators { get;}
	}
	public interface IStateIndicatorGauge : IWebGauge {
		StateIndicatorComponentCollection Indicators { get;}
	}
	public interface IDigitalGauge : IWebGauge {
		DigitalGaugeDisplayMode DisplayMode { get;set;}
		string Text { get;set;}
		int DigitCount { get;set;}
		TextSpacing Padding { get;set;}
		float LetterSpacing { get;set;}
		BaseShapeAppearance AppearanceOn { get;}
		BaseShapeAppearance AppearanceOff { get;}
		DigitalBackgroundLayerComponentCollection BackgroundLayers { get;}
		DigitalEffectLayerComponentCollection EffectLayers { get;}
	}
	public interface IScaleDependentElement {
		string ScaleID { get;set;}
	}
}
