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
using System.Drawing;
using System.Linq;
using DevExpress.DashboardCommon.Data;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.DataAccess.Native;
using DevExpress.XtraGauges.Base;
namespace DevExpress.DashboardCommon.Viewer {
	public class DashboardGaugeControlViewer : IDisposable, IContentProvider {
		public const decimal DefaultBorderProportion = 0.109m;
		readonly Locker locker = new Locker();
		readonly IDashboardGaugeControl gaugeControl;
		readonly List<IGauge> gaugesCache = new List<IGauge>();
		readonly List<GaugeModel> gaugeModels = new List<GaugeModel>();
		readonly List<IGauge> lastVisibleGauges = new List<IGauge>();
		Point clientOffset;
		GaugeGenerator gaugeGenerator;
		GaugeDashboardItemViewModel viewModel;
		GaugeViewerDataController dataController;
		bool isDisposed;
		decimal IContentProvider.BorderProportion { get { return DefaultBorderProportion / 3; } }
		Size IContentProvider.ItemMargin { get { return Size.Empty; } }
		int IContentProvider.ItemMinWidth { get { return gaugeGenerator.GaugeMinWidth; } }
		Size IContentProvider.ItemProportions { get { return gaugeGenerator.GetGaugeModelSize(true); } }
		ICollection IContentProvider.Items { get { return gaugeModels; } }
		public IList<GaugeModel> GaugeModels { get { return gaugeModels; } }
		public Point ClientOffset {
			get { return clientOffset; }
			set {
				clientOffset = value;
				RaiseChanged(ContentProviderChangeReason.Data);
			}
		}
		public bool IsDataReduced { get { return dataController.IsDataReduced; } }
		event EventHandler<ContentProviderEventArgs> Changed;
		public event EventHandler GaugeCountChanged;
		event EventHandler<ContentProviderEventArgs> IContentProvider.Changed {
			add { Changed = (EventHandler<ContentProviderEventArgs>)Delegate.Combine(Changed, value); }
			remove { Changed = (EventHandler<ContentProviderEventArgs>)Delegate.Remove(Changed, value); }
		}
		public DashboardGaugeControlViewer(IDashboardGaugeControl gaugeControl) {
			this.gaugeControl = gaugeControl;
			gaugeControl.ClearBorder();
			gaugeControl.ClearLayout();
			UpdateGaugeGenerator();
		}
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if(disposing)
				DestroyGaugeGenerator();
			isDisposed = true;
		}
		public double GetGaugeMin(string gaugeId) {
			return dataController.GetGaugeMin(gaugeId);
		}
		public double GetGaugeMax(string gaugeId) {
			return dataController.GetGaugeMax(gaugeId);
		}
		void RaiseChanged(ContentProviderChangeReason reason) {
			if(Changed != null)
				Changed(this, new ContentProviderEventArgs(reason));
		}
		void AddGaugesToCache(IEnumerable<IGauge> gauges) {
			gaugesCache.InsertRange(0, gauges);
		}
		void ClearGauges() {
			gaugesCache.Clear();
			gaugeControl.ClearGauges();
			if(gaugeGenerator!=null)
				gaugeGenerator.Clear();
		}
		IGauge GetGauge() {
			IGauge gauge;
			if(gaugesCache.Count > 0) {
				gauge = gaugesCache[0];
				gaugesCache.RemoveAt(0);
			}
			else
				gauge = gaugeGenerator.CreateGauge();
			return gauge;
		}
		void RemoveCachedGauges() {
			foreach(IGauge gauge in gaugesCache)
				gaugeControl.RemoveGauge(gauge);
		}
		bool BeginUpdate() {
			if(isDisposed)
				return false;
			bool shouldUpdate = !locker.IsLocked;
			locker.Lock();
			return shouldUpdate && UpdateGaugeGenerator();
		}
		void EndUpdate(bool raiseChanged) {
			if(isDisposed)
				return;
			locker.Unlock();
			if(!locker.IsLocked) {
				locker.Lock();
				if(raiseChanged)
					RaiseChanged(ContentProviderChangeReason.ItemProperties);
				Rectangle clientBounds = new Rectangle(clientOffset, gaugeControl.Size);
				foreach(GaugeModel model in gaugeModels) {
					bool isModelVisible = model.IsVisible(clientBounds);
					if(isModelVisible) {
						IGauge gauge = GetGauge();
						gauge.BeginUpdate();
						gaugeControl.AddGauge(gauge, model);
						gaugeGenerator.ApplyGaugeSettings(gauge, model);
						Rectangle modelBounds = model.Bounds;
						Size modelSize = gaugeGenerator.GetGaugeModelSize(false);
						decimal modelWidth = modelSize.Width;
						decimal proportion = modelBounds.Width / modelWidth;
						Rectangle gaugeBounds = new Rectangle(modelBounds.X, modelBounds.Y + (int)(gaugeGenerator.GetTopPadding() * proportion),
							Convert.ToInt32(modelWidth * proportion), Convert.ToInt32(modelSize.Height * proportion));
						gaugeControl.SetGaugeBounds(gauge, gaugeBounds, new Rectangle(model.Left, model.Top, model.Width, model.Height));
						gauge.EndUpdate();
					}
					model.Visible = isModelVisible;
				}
				RemoveCachedGauges();
				if(gaugeControl.GaugesCount == 0)
					gaugeControl.Invalidate();
				locker.Unlock();
			}
		}
		void RaiseGaugeCountChanged() {
			if(GaugeCountChanged != null)
				GaugeCountChanged(this, new EventArgs());
		}
		Color GetVariationBrush(IndicatorType indicatorType, bool isGood) {
			switch(indicatorType) {
				case IndicatorType.Warning:
					return gaugeGenerator.DeltaColorsGetter.Warning;
				case IndicatorType.DownArrow:
				case IndicatorType.UpArrow:
					return isGood ? gaugeGenerator.DeltaColorsGetter.Good : gaugeGenerator.DeltaColorsGetter.Bad;
				default:
					return gaugeGenerator.DeltaColorsGetter.Neutral;
			}
		}
		void GenerateGauges() {
			if(viewModel != null) {
				foreach(GaugeViewModel gaugeViewModel in viewModel.Gauges) {
					GaugeRangeModel rangeModel = dataController.CalculateGaugeRange(gaugeViewModel);
					foreach(KpiViewerElementData kpiData in dataController.GetElementData(gaugeViewModel)) {
						PrepareGauge(gaugeViewModel, CreateGaugeData(kpiData), rangeModel);
					}
				}
			}
		}
		void PrepareGauge(GaugeViewModel gaugeViewModel, GaugeData gaugeData, GaugeRangeModel rangeModel) {
			float? targetValue = null;
			IndicatorType deltaIndicatorType = IndicatorType.None;
			bool deltaIsGood = true;
			float actualValue;
			string deltaValueText = gaugeData.ValueText;
			actualValue = Convert.ToSingle(gaugeData.Value);
			Color valueColor = gaugeGenerator.DeltaColorsGetter.ActualValueColor;
			if(!gaugeData.SingleValue) {
				actualValue = Convert.ToSingle(gaugeData.ActualValue);
				targetValue = Convert.ToSingle(gaugeData.TargetValue);
				deltaIndicatorType = gaugeData.IndicatorType;
				deltaIsGood = gaugeData.IsGood;
				if(!gaugeViewModel.IgnoreDeltaColor)
					valueColor = GetVariationBrush(deltaIndicatorType, deltaIsGood);
			}
			IList selectionValues = gaugeData.SelectionValues;
			string[] titleText = new string[0];
			if(viewModel.ShowGaugeCaptions) {
				titleText = new string[] { gaugeData.Title };
				if(selectionValues != null) {
					ElementTitleFormatter formatter = new ElementTitleFormatter(gaugeData.SelectionCaptions);
					string mainTitle = formatter.MainTitle;
					string subTitle = formatter.SubTitle;
					titleText = String.IsNullOrEmpty(subTitle) ? new string[] { mainTitle } : new string[] { mainTitle, subTitle };
				}
			}
			string[] valueText = new string[] { deltaValueText };
			GaugeModel model = new GaugeModel(valueText, titleText, actualValue, targetValue, rangeModel, selectionValues,
				deltaIndicatorType, deltaIsGood, valueColor, gaugeViewModel.ID);
			gaugeModels.Add(model);
		}
		GaugeData CreateGaugeData(KpiViewerElementData kpiData) {
			return new GaugeData(kpiData.Title, kpiData.SelectionData, kpiData.DeltaValues);
		}
		bool UpdateGaugeGenerator() {
			GaugeViewType viewType = (viewModel != null) ? viewModel.ViewType : GaugeViewType.CircularFull;
			GaugeTheme theme = gaugeControl.IsDarkBackground ? GaugeGenerator.GetDarkTheme(viewType) : GaugeGenerator.GetLightTheme(viewType);
			if(gaugeGenerator != null && gaugeGenerator.GetIsCurrent(viewType, theme)) {
				AddGaugesToCache(gaugeControl.Gauges);
				return false;
			}
			ClearGauges();
			DestroyGaugeGenerator();
			gaugeGenerator = GaugeGenerator.Create(viewType, theme, new GaugeDeltaColorGetter(gaugeControl.GetSkin(), viewType == GaugeViewType.CircularFull));
			return true;
		}
		void Arrange() {
			bool arrangeDirty = BeginUpdate();
			try {
				RaiseChanged(ContentProviderChangeReason.Data);
			}
			finally {
				EndUpdate(arrangeDirty);
			}
		}
		void DestroyGaugeGenerator() {
			if(gaugeGenerator != null) {
				gaugeGenerator.Dispose();
				gaugeGenerator = null;
			}
		}
		void UpdateCore() {
			UpdateGaugeGenerator();
			RemoveCachedGauges();
			gaugeModels.Clear();
			bool arrangeDirty = BeginUpdate();
			try {
				GenerateGauges();
				Arrange();
				arrangeDirty |= gaugeGenerator.Measure(gaugeModels, gaugeControl.IsDarkBackground, gaugeControl.DashboardGaugeForeColor, gaugeControl.DashboardGaugeBackColor);
			}
			finally {
				EndUpdate(arrangeDirty);
			}
		}
		void IContentProvider.BeginUpdate() {
			BeginUpdate();
		}
		void IContentProvider.EndUpdate() {
			EndUpdate(false);
			if(!locker.IsLocked && !lastVisibleGauges.SequenceEqual(gaugeControl.Gauges)) {
				RaiseGaugeCountChanged();
				lastVisibleGauges.Clear();
				lastVisibleGauges.AddRange(gaugeControl.Gauges);
			}
		}
		void IContentProvider.SetSize(Size size) {
			gaugeControl.Size = size;
		}
		public void Update(GaugeDashboardItemViewModel viewModel, MultiDimensionalData data, bool drilledDown) {
			Update(viewModel, data, drilledDown, false);
		}
		public void Update(GaugeDashboardItemViewModel viewModel, MultiDimensionalData data, bool drilledDown, bool isDesignMode) {
			this.viewModel = viewModel;
			this.dataController = new GaugeViewerDataController(data, viewModel, drilledDown, isDesignMode);
			UpdateCore();
		}
		public void OnStyleChanged() {
			DestroyGaugeGenerator();
			UpdateCore();
		}
	}
}
