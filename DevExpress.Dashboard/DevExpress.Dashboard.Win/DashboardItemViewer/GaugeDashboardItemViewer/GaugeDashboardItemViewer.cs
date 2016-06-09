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

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Service;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Skins;
namespace DevExpress.DashboardWin.Native {
	[DXToolboxItem(false)]
	public class GaugeDashboardItemViewer : DataDashboardItemViewer, IWinContentProvider {
		ContentScrollableControl contentScrollableControl;
		DashboardGaugeControl gaugeControl;
		DashboardGaugeControlViewer controlViewer;
		IContentProvider IWinContentProvider.ContentProvider { get { return controlViewer; } }
		Control IWinContentProvider.ContentControl { get { return gaugeControl; } }
		protected override string ElementName { get { return DashboardLocalizer.GetString(DashboardStringId.ElementNameGauges); } }
#if DEBUGTEST
		public IList<GaugeModel> GaugeModels { get { return controlViewer.GaugeModels; } }
		public ContentScrollableControl ContentScrollableControl { get { return contentScrollableControl; } }
#endif
		public double GetGaugeMin(string gaugeId) {
			return controlViewer.GetGaugeMin(gaugeId);
		}
		public double GetGaugeMax(string gaugeId) {
			return controlViewer.GetGaugeMax(gaugeId);
		}
		GaugeDashboardItemViewModel GaugeViewModel { get { return (GaugeDashboardItemViewModel)ViewModel; } }
		event EventHandler<PaintEventArgs> Painted;
		event EventHandler<PaintEventArgs> IWinContentProvider.Painted {
			add { Painted = (EventHandler<PaintEventArgs>)Delegate.Combine(Painted, value); }
			remove { Painted = (EventHandler<PaintEventArgs>)Delegate.Remove(Painted, value); }
		}
		protected override void UpdateViewer() {
			base.UpdateViewer();
			if(ViewModel.ShouldIgnoreUpdate)
				return;
			contentScrollableControl.Model.InitializeContent(GaugeViewModel.ContentDescription);
			DataDashboardItemDesigner designer = DesignerProvider.GetDesigner<DataDashboardItemDesigner>();			
			if(designer != null) {
				controlViewer.Update((GaugeDashboardItemViewModel)ViewModel, MultiDimensionalData, DrillDownUniqueValues != null, true);
				designer.SetDataReducedImage(controlViewer.IsDataReduced);
			}
			else {
				controlViewer.Update((GaugeDashboardItemViewModel)ViewModel, MultiDimensionalData, DrillDownUniqueValues != null, false);
			}
		}
		protected override DataPointInfo GetDataPointInfo(Point location, bool onlyTargetAxes) {
			IValuesProvider valueProvider = contentScrollableControl.GetHitItem(location) as IValuesProvider;
			if(valueProvider != null) {
				return GetDataPoint(valueProvider);
			}
			return null;
		}
		DataPointInfo GetDataPoint(IValuesProvider hitItem) {
			DataPointInfo dataPointInfo = new DataPointInfo();
			IList dimensionValues = hitItem.SelectionValues;
			string id = hitItem.MeasureID;
			if(dimensionValues != null) {
				dataPointInfo.DimensionValues.Add(DashboardDataAxisNames.DefaultAxis, dimensionValues);
			}
			if(GaugeViewModel != null && GaugeViewModel.Gauges != null && GaugeViewModel.Gauges.Count > 0) {
				foreach(GaugeViewModel gauge in GaugeViewModel.Gauges) {
					string gaugeId = gauge.ID;
					if(gaugeId == id) {
						if(gauge.DataItemType == KpiElementDataItemType.Delta)
							dataPointInfo.Deltas.Add(gaugeId);
						else
							dataPointInfo.Measures.Add(gaugeId);
					}
				}
			}
			return dataPointInfo;
		}
		void OnStyleChanged(object sender, EventArgs e) {
			controlViewer.OnStyleChanged();
		}
		void OnPainted(object sender, PaintEventArgs e) {
			if(Painted != null)
				Painted(this, e);
		}
		public override void OnLookAndFeelChanged() {
			Skin skin = CommonSkins.GetSkin(LookAndFeel);			
			BackColor = skin.Colors.GetColor(CommonColors.Window);
		}
		protected override Control GetViewControl() {
			gaugeControl = new DashboardGaugeControl();
			gaugeControl.LookAndFeel.StyleChanged += OnStyleChanged;
			gaugeControl.Painted += OnPainted;
			controlViewer = new DashboardGaugeControlViewer(gaugeControl);
			controlViewer.GaugeCountChanged += OnGaugeCountChanged;
			contentScrollableControl = new ContentScrollableControl(this);
			return contentScrollableControl;
		}
		void OnGaugeCountChanged(object sender, EventArgs e) {
			RaiseUnderlyingControlUpdated();
		}
		protected override Control GetUnderlyingControl() {
			return gaugeControl;
		}
		protected override void PrepareViewControl() {
			base.PrepareViewControl();
			contentScrollableControl.Model.ContentArrangementOptions = ContentArrangementOptions.AlignCenter;
			contentScrollableControl.MouseClick += OnContentScrollableControlMouseClick;
			contentScrollableControl.MouseDoubleClick += OnContentScrollableControlMouseDoubleClick;
			contentScrollableControl.MouseMove += OnContentScrollableControlMouseMove;
			contentScrollableControl.KeyDown += OnContentScrollableControlKeyDown;
			contentScrollableControl.KeyUp += OnContentScrollableControlKeyUp;
			contentScrollableControl.LostFocus += OnContentScrollableControlLostFocus;
			contentScrollableControl.MouseEnter += OnContentScrollableControlMouseEnter;
			contentScrollableControl.MouseLeave += OnContentScrollableControlMouseLeave;
			contentScrollableControl.MouseDown += OnContentScrollableControlMouseDown;
			contentScrollableControl.MouseUp += OnContentScrollableControlMouseUp;
			contentScrollableControl.MouseHover += OnContentScrollableControlMouseHover;
			contentScrollableControl.MouseWheel += OnContentScrollableControlMouseWheel;
		}
		void OnContentScrollableControlMouseMove(object sender, MouseEventArgs e) {
			OnDashboardItemViewerMouseMove(e);
			RaiseMouseMove(e.Location);
		}
		void OnContentScrollableControlMouseClick(object sender, MouseEventArgs e) {
			OnDashboardItemViewerMouseClick(e);
			RaiseClick(e.Location);
		}
		void OnContentScrollableControlMouseDoubleClick(object sender, MouseEventArgs e) {
			OnDashboardItemViewerMouseDoubleClick(e);
			RaiseDoubleClick(e.Location);
		}
		void OnContentScrollableControlMouseEnter(object sender, EventArgs e) {
			RaiseMouseEnter();
		}
		void OnContentScrollableControlMouseLeave(object sender, EventArgs e) {
			OnDashboardItemViewerMouseLeave();
			RaiseMouseLeave();
		}
		void OnContentScrollableControlLostFocus(object sender, EventArgs e) {
			OnDashboardItemViewerLostFocus();
		}
		void OnContentScrollableControlKeyDown(object sender, KeyEventArgs e) {
			OnDashboardItemViewerKeyDown(e);
		}
		void OnContentScrollableControlKeyUp(object sender, KeyEventArgs e) {
			OnDashboardItemViewerKeyUp(e);
		}
		void OnContentScrollableControlMouseDown(object sender, MouseEventArgs e) {
			RaiseMouseDown(e.Location);
		}
		void OnContentScrollableControlMouseUp(object sender, MouseEventArgs e) {
			RaiseMouseUp(e.Location);
		}
		void OnContentScrollableControlMouseWheel(object sender, MouseEventArgs e) {
			RaiseMouseWheel();
		}
		void OnContentScrollableControlMouseHover(object sender, EventArgs e) {
			RaiseMouseHover();
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				RaiseBeforeUnderlyingControlDisposed();
				contentScrollableControl.MouseClick -= OnContentScrollableControlMouseClick;
				contentScrollableControl.MouseDoubleClick -= OnContentScrollableControlMouseDoubleClick;
				contentScrollableControl.KeyDown -= OnContentScrollableControlKeyDown;
				contentScrollableControl.KeyUp -= OnContentScrollableControlKeyUp;
				contentScrollableControl.LostFocus -= OnContentScrollableControlLostFocus;
				contentScrollableControl.MouseMove -= OnContentScrollableControlMouseMove;
				contentScrollableControl.MouseEnter -= OnContentScrollableControlMouseEnter;
				contentScrollableControl.MouseLeave -= OnContentScrollableControlMouseLeave;
				contentScrollableControl.MouseDown -= OnContentScrollableControlMouseDown;
				contentScrollableControl.MouseUp -= OnContentScrollableControlMouseUp;
				contentScrollableControl.MouseHover -= OnContentScrollableControlMouseHover;
				contentScrollableControl.MouseWheel -= OnContentScrollableControlMouseWheel;
				contentScrollableControl.Dispose();
				gaugeControl.LookAndFeel.StyleChanged -= OnStyleChanged;
				gaugeControl.Painted -= OnPainted;
				gaugeControl.Dispose();
				controlViewer.Dispose();
			}
			base.Dispose(disposing);
		}
		protected override void PrepareClientState(ItemViewerClientState state) {
			state.ViewerArea = GetControlClientArea(contentScrollableControl);
			contentScrollableControl.PrepareScrollingState(state);
		}
		protected override void SetHighlight(List<AxisPointTuple> higtlight) {
			contentScrollableControl.HighlightValues(GetDimestionValueByAxis(higtlight, TargetAxes));
		}
		protected override void SetSelection(List<AxisPointTuple> selection) {
			contentScrollableControl.SelectValues(GetDimestionValueByAxis(selection, TargetAxes)); 
		}
	}
}
