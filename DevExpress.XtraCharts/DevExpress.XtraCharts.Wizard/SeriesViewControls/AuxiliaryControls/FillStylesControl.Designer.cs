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

namespace DevExpress.XtraCharts.Wizard.SeriesViewControls {
	partial class FillStylesControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FillStylesControl));
			this.rectangleFillStyleControl = new DevExpress.XtraCharts.Wizard.SeriesViewControls.RectangleFillStyleSeriesViewControl();
			this.rectangleFillStyle3DControl = new DevExpress.XtraCharts.Wizard.SeriesViewControls.RectangleFillStyle3DSeriesViewControl();
			this.polygonFillStyleControl = new DevExpress.XtraCharts.Wizard.SeriesViewControls.PolygonFillStyleSeriesViewControl();
			this.polygonFillStyle3DControl = new DevExpress.XtraCharts.Wizard.SeriesViewControls.PolygonFillStyle3DSeriesViewControl();
			this.SuspendLayout();
			resources.ApplyResources(this.rectangleFillStyleControl, "rectangleFillStyleControl");
			this.rectangleFillStyleControl.Name = "rectangleFillStyleControl";
			resources.ApplyResources(this.rectangleFillStyle3DControl, "rectangleFillStyle3DControl");
			this.rectangleFillStyle3DControl.Name = "rectangleFillStyle3DControl";
			resources.ApplyResources(this.polygonFillStyleControl, "polygonFillStyleControl");
			this.polygonFillStyleControl.Name = "polygonFillStyleControl";
			resources.ApplyResources(this.polygonFillStyle3DControl, "polygonFillStyle3DControl");
			this.polygonFillStyle3DControl.Name = "polygonFillStyle3DControl";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.polygonFillStyle3DControl);
			this.Controls.Add(this.polygonFillStyleControl);
			this.Controls.Add(this.rectangleFillStyle3DControl);
			this.Controls.Add(this.rectangleFillStyleControl);
			this.Name = "FillStylesControl";
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraCharts.Wizard.SeriesViewControls.RectangleFillStyleSeriesViewControl rectangleFillStyleControl;
		private DevExpress.XtraCharts.Wizard.SeriesViewControls.RectangleFillStyle3DSeriesViewControl rectangleFillStyle3DControl;
		private DevExpress.XtraCharts.Wizard.SeriesViewControls.PolygonFillStyleSeriesViewControl polygonFillStyleControl;
		private DevExpress.XtraCharts.Wizard.SeriesViewControls.PolygonFillStyle3DSeriesViewControl polygonFillStyle3DControl;
	}
}
