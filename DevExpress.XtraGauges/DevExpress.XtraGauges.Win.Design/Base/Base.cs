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
using System.Windows.Forms;
using DevExpress.Utils.Design;
using DevExpress.XtraGauges.Win.Base;
using System.ComponentModel.Design;
using System.IO;
using DevExpress.Utils.Serializing;
using System.ComponentModel;
using DevExpress.XtraGauges.Win.Gauges.Digital;
using DevExpress.XtraGauges.Win.Gauges.Linear;
namespace DevExpress.XtraGauges.Win.Design.Base {
	public class WinActionList : DesignerActionList {
		GaugeContainerDesigner designerCore;
		public WinActionList(GaugeContainerDesigner designer)
			: base(designer.Component) {
			this.designerCore = designer;
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection res = new DesignerActionItemCollection();
			if(this.GaugeControl != null) {
				res.Add(new DesignerActionHeaderItem("Presets"));
				res.Add(new DesignerActionMethodItem(this, "RunPresetManager", "Run Preset Manager", "Presets", true));
				res.Add(new DesignerActionMethodItem(this, "CustomizeCurrent", "Customize Gauge Control", "Presets", true));
				res.Add(new DesignerActionHeaderItem("Control Style"));
				res.Add(new DesignerActionMethodItem(this, "RunStyleManager", "Run Style Manager", "Control Style", true));
				res.Add(new DesignerActionHeaderItem("Properties"));
				res.Add(new DesignerActionPropertyItem("Dock", "Choose DockStyle", "Properties"));
				res.Add(new DesignerActionPropertyItem("AutoLayout", "Enable Auto Layout", "Properties"));
				res.Add(new DesignerActionMethodItem(this, "About", "About", null, true));
			}
			return res;
		}
		public void RunStyleManager() {
			designerCore.RunStyleManager();
		}
		public void RunPresetManager() {
			designerCore.RunPresetManager();
		}
		public void CustomizeCurrent() {
			designerCore.CustomizeCurrent();
		}
		public void AddDigitalGauge() {
			this.GaugeControl.AddDigitalGauge();
		}
		public void AddLinearGauge() {
			this.GaugeControl.AddLinearGauge();
		}
		public void AddCircularGauge() {
			this.GaugeControl.AddCircularGauge();
		}
		public void AddStateIndicatorGauge() {
			this.GaugeControl.AddStateIndicatorGauge();
		}
		public GaugeControl GaugeControl {
			get { return this.designerCore.Component as GaugeControl; }
		}
		public bool AutoLayout {
			get { return (this.GaugeControl == null) ? false : this.GaugeControl.AutoLayout; }
			set { EditorContextHelper.SetPropertyValue(this.designerCore, this.GaugeControl, "AutoLayout", value); }
		}
		public DockStyle Dock {
			get { return (this.GaugeControl == null) ? DockStyle.None : this.GaugeControl.Dock; }
			set { EditorContextHelper.SetPropertyValue(this.designerCore, this.GaugeControl, "Dock", value); }
		}
		public void About() {
			DevExpress.XtraGauges.Win.GaugeControl.About();
		}
	}
}
