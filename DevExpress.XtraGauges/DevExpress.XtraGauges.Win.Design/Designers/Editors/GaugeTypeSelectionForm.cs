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
using DevExpress.XtraGauges.Presets.PresetManager;
using DevExpress.XtraGauges.Win.Design;
using DevExpress.XtraGauges.Win.Design.Base;
namespace DevExpress.XtraGauges.Design {
	public class GaugeTypeSelectionForm : ItemKindChoosingForm {
		GaugeKind gaugeKindCore;
		public GaugeKind GaugeKind {
			get { return gaugeKindCore; }
		}
		protected override void OnLoad(EventArgs e) {
			gallery.SelectedIndex = 0;
			base.OnLoad(e);
		}
		IDictionary<string, GaugeKind> gaugeKinds;
		protected override void Initialize() {
			gaugeKinds = new Dictionary<string, GaugeKind>();
			Text = "Select Gauge Type";
			ClientSize = new System.Drawing.Size(400, 436);
			AddItem(new CircularGaugeKind());
			AddItem(new LinearGaugeKind());
			AddItem(new DigitalGaugeKind());
			AddItem(new StateIndicatorGaugeKind());
			gallery.LayoutChanged();
		}
		protected override void OnSelectedIndexChanged() {
			if(gallery.SelectedItem != null)
				this.gaugeKindCore = gaugeKinds[gallery.SelectedItem.Name];
			else 
				this.gaugeKindCore = null;
		}
		void AddItem(GaugeKind gaugeKind) {
			string name = gaugeKind.GetName();
			gaugeKinds.Add(name, gaugeKind);
			gallery.Items.Add(new GalleryItem(name, gaugeKind.GetImage()));
		}
	}
}
