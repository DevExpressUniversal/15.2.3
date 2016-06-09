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
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Win.Base;
using DevExpress.XtraGauges.Core.Model;
using System.ComponentModel;
namespace DevExpress.XtraGauges.Win.Gauges.Circular {
	public class ArcScaleComponentCollection :
		ComponentCollection<ArcScaleComponent> {
	}
	public class CircularGaugeComponentCollectionBase<T> : ComponentCollection<T>
			where T : class, INamed, IComponent, ISupportAcceptOrder, IArcScaleComponent {
		protected override void OnElementRemoved(T element) {
			base.OnElementRemoved(element);
			BaseObject bObj = element as BaseObject;
			if((bObj != null) && bObj.IsDisposing) return;
			element.ArcScale = ScaleFactory.EmptyArcScale;
		}
	}
	public class ArcScaleBackgroundLayerComponentCollection :
		CircularGaugeComponentCollectionBase<ArcScaleBackgroundLayerComponent> {
	}
	public class ArcScaleNeedleComponentCollection :
		CircularGaugeComponentCollectionBase<ArcScaleNeedleComponent> {
	}
	public class ArcScaleSpindleCapComponentCollection :
		CircularGaugeComponentCollectionBase<ArcScaleSpindleCapComponent> {
	}
	public class ArcScaleMarkerComponentCollection :
		CircularGaugeComponentCollectionBase<ArcScaleMarkerComponent> {
	}
	public class ArcScaleRangeBarComponentCollection :
		CircularGaugeComponentCollectionBase<ArcScaleRangeBarComponent> {
	}
	public class ArcScaleEffectLayerComponentCollection :
		CircularGaugeComponentCollectionBase<ArcScaleEffectLayerComponent> {
	}
	public class ArcScaleStateIndicatorComponentCollection :
		ComponentCollection<ArcScaleStateIndicatorComponent> {
		protected override void OnElementRemoved(ArcScaleStateIndicatorComponent element) {
			base.OnElementRemoved(element);
			if(element.IsDisposing) return;
			element.IndicatorScale = ScaleFactory.EmptyArcScale;
		}
	}
}
