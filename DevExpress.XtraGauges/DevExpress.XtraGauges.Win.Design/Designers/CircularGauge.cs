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
using System.Collections;
using DevExpress.XtraGauges.Base;
using DevExpress.XtraGauges.Win.Design.Base;
using DevExpress.XtraGauges.Win.Gauges.Circular;
namespace DevExpress.XtraGauges.Win.Design {
	public class CircularGaugeDesigner : GaugeDesigner {
		protected override bool AllowEditInherited { get { return false; } }
		protected CircularGauge CircularGauge {
			get { return base.Component as CircularGauge; }
		}
		public override ICollection AssociatedComponents {
			get {
				ArrayList components = new ArrayList(base.AssociatedComponents);
				if(!CircularGauge.IsDisposing) {
					components.AddRange(CircularGauge.Scales);
					components.AddRange(CircularGauge.BackgroundLayers);
					components.AddRange(CircularGauge.Needles);
					components.AddRange(CircularGauge.Markers);
					components.AddRange(CircularGauge.RangeBars);
					components.AddRange(CircularGauge.SpindleCaps);
					components.AddRange(CircularGauge.EffectLayers);
					components.AddRange(CircularGauge.Indicators);
					components.AddRange(CircularGauge.Labels);
				}
				return components;
			}
		}
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			CircularGauge.AddDefaultElements();
		}
	}
	public class CircularGaugeKind : GaugeKind {
		public override string GetName() {
			return "Circular Gauge";
		}
		public override Image GetImage() {
			return DesignHelper.GetImageFromDesignResources("CircularGauge.png");
		}
		public override IGauge CreateInstance() {
			return new CircularGauge();
		}
	}
}
