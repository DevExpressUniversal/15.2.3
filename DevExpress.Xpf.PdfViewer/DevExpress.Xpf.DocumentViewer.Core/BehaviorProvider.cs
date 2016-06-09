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
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.DocumentViewer {
	public class ZoomHelper {
		public Size Viewport { get; set; }
		public Size PageSize { get; set; }
		public Size PageVisibleSize { get; set; }
		public double CalcZoomFactor(ZoomMode zoomMode, double currentZoomFactor) {
			switch (zoomMode) {
				case ZoomMode.ActualSize:
					return 1d;
				case ZoomMode.PageLevel:
					return CalcViewWholePage();
				case ZoomMode.FitToWidth:
					return CalcViewPageWidth();
				case ZoomMode.FitToVisible:
					return CalcViewPageVisibleWidth();
				case ZoomMode.Custom:
					return currentZoomFactor;
				default:
					throw new ArgumentException("zoomMode");
			}
		}
		public double CalcZoomFactor(ZoomMode zoomMode) {
			return CalcZoomFactor(zoomMode, 1d);
		}
		double CalcViewPageVisibleWidth() {
			if (Viewport.IsEmpty || PageVisibleSize.IsEmpty)
				return 1d;
			return Viewport.Width / PageVisibleSize.Width;
		}
		double CalcViewPageWidth() {
			if (Viewport.IsEmpty || PageSize.IsEmpty)
				return 1d;
			return Viewport.Width / PageSize.Width;
		}
		double CalcViewWholePage() {
			if (Viewport.IsEmpty || PageSize.IsEmpty)
				return 1d;
			double fitWidthProp = Viewport.Width / PageSize.Width;
			double fitHeightProp = Viewport.Height / PageSize.Height;
			return Math.Min(fitWidthProp, fitHeightProp);
		}
	}
	public class ZoomChangedEventArgs : EventArgs {
		public double OldZoomFactor { get; private set; }
		public double ZoomFactor { get; private set; }
		public ZoomMode OldZoomMode { get; private set; }
		public ZoomMode ZoomMode { get; private set; }
		public ZoomChangedEventArgs(double oldZoomFactor, double zoomFactor, ZoomMode oldZoomMode, ZoomMode zoomMode) {
			OldZoomFactor = oldZoomFactor;
			ZoomFactor = zoomFactor;
			OldZoomMode = oldZoomMode;
			ZoomMode = zoomMode;
		}
	}
	public class RotateAngleChangedEventArgs : EventArgs {
		public double OldValue { get; private set; }
		public double NewValue { get; private set; }
		public RotateAngleChangedEventArgs(double oldValue, double newValue) {
			OldValue = oldValue;
			NewValue = newValue;
		}
	}
	public class PageIndexChangedEventArgs : EventArgs {
		public int PageIndex { get; private set; }
		public PageIndexChangedEventArgs(int pageIndex) {
			PageIndex = pageIndex;
		}
	}
	public class BehaviorProvider : BindableBase {
		Locker updateZoomLocker = new Locker();
		double zoomFactor = 1d;
		ZoomMode zoomMode = ZoomMode.ActualSize;
		double rotateAngle;
		int pageIndex;
		protected virtual List<double> DefaultZoomLevels { get { return new List<double> { 0.10, 0.25, 0.5, 0.75, 1d, 1.25, 1.5, 2d, 4d, 5d }; } }
		Size viewport;
		Size pageSize;
		Size pageVisibleSize;
		ZoomHelper ZoomHelper { get; set; }
		public ZoomMode ZoomMode {
			get { return zoomMode; }
			set { SetProperty(ref zoomMode, value, () => ZoomMode, OnZoomModeChanged); }
		}
		public double ZoomFactor {
			get { return zoomFactor; }
			set {
				double oldValue = zoomFactor;
				SetProperty(ref zoomFactor, value, () => ZoomFactor, () => OnZoomFactorChanged(oldValue, value)); 
			}
		}
		public Size Viewport {
			get { return viewport; }
			set { SetProperty(ref viewport, value, () => Viewport, OnViewportChanged); }
		}
		public Size PageSize {
			get { return pageSize; }
			set { SetProperty(ref pageSize, value, () => PageSize, OnPageSizeChanged); }
		}
		public Size PageVisibleSize {
			get { return pageVisibleSize; }
			set { SetProperty(ref pageVisibleSize, value, () => PageVisibleSize, OnPageVisibleSizeChanged); }
		}
		public double RotateAngle {
			get { return rotateAngle; }
			set {
				double oldValue = rotateAngle;
				SetProperty(ref rotateAngle, value, () => RotateAngle, () => OnRotateAngleChanged(oldValue, value)); 
			}
		}
		public int PageIndex {
			get { return pageIndex; }
			set { SetProperty(ref pageIndex, value, () => PageIndex, OnPageIndexChanged); }
		}
		public event EventHandler<ZoomChangedEventArgs> ZoomChanged;
		public event EventHandler<RotateAngleChangedEventArgs> RotateAngleChanged;
		public event EventHandler<PageIndexChangedEventArgs> PageIndexChanged;
		public BehaviorProvider() {
			ZoomHelper = new ZoomHelper();
		}
		public void ZoomIn() {
			Zoom(true);
		}
		public void ZoomOut() {
			Zoom(false);
		}
		public bool CanZoomIn() {
			List<double> zoomFactors = GetZoomFactors();
			int index = FindCurrentZoomIndex(zoomFactors);
			return index < zoomFactors.Count - 1;
		}
		public bool CanZoomOut() {
			List<double> zoomFactors = GetZoomFactors();
			int index = FindCurrentZoomIndex(zoomFactors);
			return index <= zoomFactors.Count - 1 && index > 0;
		}
		public double GetNextZoomFactor(bool isZoomIn) {
			List<double> zoomFactors = GetZoomFactors();
			int index = Math.Max(FindCurrentZoomIndex(zoomFactors) + (isZoomIn ? 1 : -1), 0);
			index = Math.Min(index, zoomFactors.Count - 1);
			return zoomFactors[index];
		}
		ZoomMode GetZoomModeByZoomFactor(double zoomFactor) {
			if (ZoomHelper.CalcZoomFactor(ZoomMode.FitToWidth).AreClose(zoomFactor))
				return ZoomMode.FitToWidth;
			if (ZoomHelper.CalcZoomFactor(ZoomMode.FitToVisible).AreClose(zoomFactor))
				return ZoomMode.FitToVisible;
			if (ZoomHelper.CalcZoomFactor(ZoomMode.PageLevel).AreClose(zoomFactor))
				return ZoomMode.PageLevel;
			if (zoomFactor.AreClose(1d))
				return ZoomMode.ActualSize;
			return ZoomMode.Custom;
		}
		void Zoom(bool isZoomIn) {
			double nextZoomFactor = GetNextZoomFactor(isZoomIn);
			ZoomFactor = nextZoomFactor;
		}
		int FindCurrentZoomIndex(List<double> zoomFactors) {
			int index = zoomFactors.BinarySearch(ZoomFactor);
			return index >= 0 ? index : -index - 1;
		}
		protected void UpdateZoomFactor() {
			updateZoomLocker.DoLockedActionIfNotLocked(() => ZoomFactor = ZoomHelper.CalcZoomFactor(ZoomMode, ZoomFactor));
		}
		protected void UpdateZoomMode() {
			updateZoomLocker.DoLockedActionIfNotLocked(() => ZoomMode = GetZoomModeByZoomFactor(ZoomFactor));
		}
		void RaiseZoomChanged(double oldZoomFactor, double newZoomFactor, ZoomMode oldZoomMode, ZoomMode newZoomMode) {
			if (ZoomChanged != null)
				ZoomChanged(this, new ZoomChangedEventArgs(oldZoomFactor, newZoomFactor, oldZoomMode, newZoomMode));
		}
		void RaiseRotateAngleChanged(double oldValue, double newValue) {
			if (RotateAngleChanged != null)
				RotateAngleChanged(this, new RotateAngleChangedEventArgs(oldValue, newValue));
		}
		void RaisePageIndexChanged() {
			if (PageIndexChanged != null)
				PageIndexChanged(this, new PageIndexChangedEventArgs(PageIndex));
		}
		protected virtual List<double> GetZoomFactors() {
			if (ZoomHelper == null)
				return new List<double>(DefaultZoomLevels);
			List<double> result = new List<double>(DefaultZoomLevels) {
				ZoomHelper.CalcZoomFactor(ZoomMode.FitToWidth),
				ZoomHelper.CalcZoomFactor(ZoomMode.PageLevel)
			};
			result = result.Distinct().ToList();
			result.Sort();
			return result;
		}
		protected virtual void OnPageVisibleSizeChanged() {
			ZoomHelper.PageVisibleSize = PageVisibleSize;
			UpdateZoomFactor();
		}
		protected virtual void OnPageSizeChanged() {
			ZoomHelper.PageSize = PageSize;
			UpdateZoomFactor();
		}
		protected virtual void OnViewportChanged() {
			ZoomHelper.Viewport = Viewport;
			UpdateZoomFactor();
		}
		protected virtual void OnZoomFactorChanged(double oldValue, double newValue) {
			ZoomMode oldZoomMode = ZoomMode;
			UpdateZoomMode();
			RaiseZoomChanged(oldValue, newValue, oldZoomMode, ZoomMode);
		}
		protected virtual void OnZoomModeChanged() {
			UpdateZoomFactor();
		}
		protected virtual void OnRotateAngleChanged(double oldValue, double newValue) {
			ZoomHelper.PageSize = PageSize;
			UpdateZoomFactor();
			RaiseRotateAngleChanged(oldValue, newValue);
		}
		protected virtual void OnPageIndexChanged() {
			RaisePageIndexChanged();
		}
	}
}
