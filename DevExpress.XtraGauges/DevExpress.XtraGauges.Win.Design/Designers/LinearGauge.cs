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
using DevExpress.XtraGauges.Win.Gauges.Linear;
namespace DevExpress.XtraGauges.Win.Design {
	public class LinearGaugeDesigner : GaugeDesigner {
		protected override bool AllowEditInherited { get { return false; } }
		protected LinearGauge LinearGauge {
			get { return base.Component as LinearGauge; }
		}
		public override ICollection AssociatedComponents {
			get {
				ArrayList components = new ArrayList(base.AssociatedComponents);
				if(!Gauge.IsDisposing) {
					components.AddRange(LinearGauge.Scales);
					components.AddRange(LinearGauge.BackgroundLayers);
					components.AddRange(LinearGauge.Levels);
					components.AddRange(LinearGauge.Markers);
					components.AddRange(LinearGauge.RangeBars);
					components.AddRange(LinearGauge.EffectLayers);
					components.AddRange(LinearGauge.Indicators);
					components.AddRange(LinearGauge.Labels);
				}
				return components;
			}
		}
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			LinearGauge.AddDefaultElements();
		}
	}
	public class LinearGaugeKind : GaugeKind {
		public override string GetName() {
			return "Linear Gauge";
		}
		public override Image GetImage() {
			return DesignHelper.GetImageFromDesignResources("LinearGauge.png");
		}
		public override IGauge CreateInstance() {
			return new LinearGauge();
		}
	}
}
