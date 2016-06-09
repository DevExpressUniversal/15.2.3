#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Drawing;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.Skins;
using DevExpress.Utils.Controls;
using DevExpress.XtraGauges.Base;
using DevExpress.XtraPrinting;
using System.Collections;
using DevExpress.DashboardCommon.Native;
using System;
namespace DevExpress.DashboardExport {
	public class ExportGaugeControl : IDisposable, IDashboardGaugeControl {
		readonly List<ExportGauge> gauges = new List<ExportGauge>();
		Size size;
		bool IDashboardGaugeControl.IsDarkBackground { get { return false; } }
		Color IDashboardGaugeControl.DashboardGaugeForeColor { get { return Color.Empty; } }
		Color IDashboardGaugeControl.DashboardGaugeBackColor { get { return Color.Empty; } }
		IEnumerable<IGauge> IDashboardGaugeControl.Gauges {
			get {
				foreach(ExportGauge gauge in gauges)
					yield return gauge.IGauge;
			}
		}
		int IDashboardGaugeControl.GaugesCount { get { return gauges.Count; } }
		Size IDashboardGaugeControl.Size { get { return size; } set { size = value; } }
		public List<ExportGauge> Gauges { get { return gauges; } }
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if(disposing)
				DisposeAndClearGauges();
		}
		void DisposeAndClearGauges() {
			gauges.ForEach(gauge => gauge.Dispose());
			gauges.Clear();
		}
		ExportGauge FindGauge(IGauge iGauge) {
			return gauges.Find(gauge => gauge.IGauge == iGauge);
		}
		void IDashboardGaugeControl.ClearBorder() {
		}
		void IDashboardGaugeControl.ClearLayout() {
			gauges.ForEach(gauge => gauge.Container.AutoLayout = false);
		}
		void IDashboardGaugeControl.ClearGauges() {
			DisposeAndClearGauges();
		}
		Skin IDashboardGaugeControl.GetSkin() {
			return null;
		}
		void IDashboardGaugeControl.Invalidate() {
		}
		void IDashboardGaugeControl.AddGauge(IGauge iGauge, GaugeModel model) {
			ExportGauge gauge = FindGauge(iGauge);
			if(gauge != null)
				gauge.GaugeModel = model;
			else {
				GaugeContainer container = new GaugeContainer();
				container.Gauges.Add(iGauge);
				gauges.Add(new ExportGauge(container, model));
			}
		}
		void IDashboardGaugeControl.RemoveGauge(IGauge iGauge) {
			ExportGauge gauge = FindGauge(iGauge);
			gauge.Dispose();
			gauges.Remove(gauge);
		}
		void IDashboardGaugeControl.SetGaugeBounds(IGauge iGauge, Rectangle gaugeBounds, Rectangle containerBounds) {
			FindGauge(iGauge).SetBounds(gaugeBounds, containerBounds);
		}
		public void Print(IBrickGraphics graph, IList selectedValues, Size actualPrintSize) {
			ExportHelper.DrawEmptyBrick(graph, actualPrintSize);
			foreach(ExportGauge gauge in gauges) {
				Image image = gauge.GetImage();
				bool isSelected = DataUtils.ListContains(selectedValues, ((IValuesProvider)gauge.GaugeModel).SelectionValues);
				if(isSelected)
					image = ExportContentScrollableControl.BlackoutImage(image);
				ImageBrick brick = new ImageBrick();
				brick.Image = image;
				brick.Sides = BorderSide.None;
				if(isSelected)
					brick.BackColor = ExportContentScrollableControl.SelectedBackColor;
				graph.DrawBrick(brick, gauge.Bounds);
			}
		}
	}
}
