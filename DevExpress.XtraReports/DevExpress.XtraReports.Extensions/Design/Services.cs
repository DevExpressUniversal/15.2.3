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
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Design;
using System.ComponentModel;
using System.ComponentModel.Design;
using DevExpress.XtraReports.UI;
using System.Collections;
using DevExpress.XtraReports;
using System.Windows.Forms.Design;
using System.Drawing.Printing;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Design.Commands;
using DevExpress.XtraReports.Design.Ruler;
using DevExpress.XtraPrinting;
using DevExpress.Utils.Design;
namespace DevExpress.XtraReports.Design 
{
	public class RulerService {
		#region static
		public static IList CreateSections(BandViewInfo[] bandViewInfos) {
			ArrayList sections = new ArrayList();
			bool canResize = false;
			for(int i = 0; i < bandViewInfos.Length; i++) {
				BandViewInfo viewInfo = bandViewInfos[i];
				RulerSection s = new VRulerSection(viewInfo.BandBounds.Height,
					viewInfo.Expanded, 
					canResize, 
					viewInfo.Level,
					viewInfo.BandBounds.Top - viewInfo.CaptionBounds.Top,
					IsTopNeighbor(bandViewInfos, i), 
					viewInfo.Selected);
				sections.Add(s);
				canResize = !(viewInfo.Band is DetailReportBand) && !viewInfo.Locked;
			}
			return sections;
		}
		static bool IsTopNeighbor(BandViewInfo[] viewInfos, int index) {
			if(index >= viewInfos.Length - 1)
				return false;
			return viewInfos[index + 1].Level > viewInfos[index].Level || !viewInfos[index].Expanded;
		}
		static void DrawShadows(RulerBase ruler, Rectangle[] shadowRects, int level) {
			if(ruler != null) ruler.DrawShadows(shadowRects, level);
		}
		static void HideShadows(RulerBase ruler) {
			if(ruler != null) ruler.HideShadows();
		}
		#endregion
		HRuler hRuler;
		VRuler vRuler;
		#region tests
		#if DEBUG
		public HRuler HRuler { get { return hRuler; }
		}
		public VRuler VRuler { get { return vRuler; }
		}
		#endif
		#endregion
		public RulerService(ReportDesigner designer, HRuler hRuler, VRuler vRuler) {
			this.hRuler = hRuler;
			this.vRuler = vRuler;
		}
		public void HideShadows() {
			HideShadows(hRuler);
			HideShadows(vRuler);
		}
		public void DrawShadows(RectangleF[] shadowRects, int level) {
			Rectangle[] rects = Array.ConvertAll<RectangleF, Rectangle>(shadowRects, delegate(RectangleF value) { return Rectangle.Round(value); });
			DrawShadows(hRuler, rects, level);
			DrawShadows(vRuler, rects, level);
		}
	}
	public class ZoomService {
		#region static
		const float Lag = 0.00001f;
		const float ZoomStep = 0.1f;
		public const float MinZoom = 0.25f;
		public const float MaxZoom = 8.0f;
		public const int MinZoomInPercents = (int)(MinZoom * 100);
		public const int MaxZoomInPercents = (int)(MaxZoom * 100);
		public const string ZoomStringFormat = "{0:###}%";
		public const int DefaultZoomFactorInPercents = 100;
		public static readonly ZoomService NullZoomService = new ZoomService(null);
		public static readonly int[] PredefinedZoomFactorsInPercents = new int[] { 800, 400, 300, 200, 150, 100, 50, 25 };
		public static ZoomService GetInstance(IServiceProvider serviceProvider) {
			return (ZoomService)ServiceHelper.GetServiceInstance(serviceProvider, typeof(ZoomService), NullZoomService);
		}
		static bool ValuesEquals(float f1, float f2) {
			return Math.Abs(f1 - f2) < Lag;
		}
		static bool ZoomIsCorrect(float value) {
			return MinZoom - Lag <= value && value <= MaxZoom + Lag;
		}
		#endregion
		#region inner clases
		abstract class InplaceEditorElementsBase {
			protected ZoomService zoomService;
			public InplaceEditorElementsBase(ZoomService zoomService) {
				this.zoomService = zoomService;
			}
			public abstract Size AdjustInplaceEditorSize(Size scaledSize);
			public abstract Font AdjustInplaceEditorFont(Font normalFont);
			public abstract BorderStyle GetInplaceEditorBorderStyle();
			public abstract float GetInplaceEditorZoomFactor();
		}
		class ScaledInplaceEditorElements : InplaceEditorElementsBase {
			public ScaledInplaceEditorElements(ZoomService zoomService) : base(zoomService) {
			}
			public override Size AdjustInplaceEditorSize(Size scaledSize) {
				return scaledSize;
			}
			public override Font AdjustInplaceEditorFont(Font normalFont) {
				return new Font(normalFont.FontFamily, zoomService.ScaleValueF(normalFont.Size), normalFont.Style, normalFont.Unit, normalFont.GdiCharSet, normalFont.GdiVerticalFont);
			}
			public override BorderStyle GetInplaceEditorBorderStyle() {
				return BorderStyle.None;
			}
			public override float GetInplaceEditorZoomFactor() {
				return zoomService.ZoomFactor;
			}
		}
		class NormalInplaceEditorElements : InplaceEditorElementsBase {
			public NormalInplaceEditorElements(ZoomService zoomService) : base(zoomService) {
			}
			public override Size AdjustInplaceEditorSize(Size scaledSize) {
				return zoomService.UnscaleSize(scaledSize);
			}
			public override Font AdjustInplaceEditorFont(Font normalFont) {
				return normalFont;
			}
			public override BorderStyle GetInplaceEditorBorderStyle() {
				return BorderStyle.Fixed3D;
			}
			public override float GetInplaceEditorZoomFactor() {
				return 1;
			}
		}
		#endregion
		public event EventHandler ZoomChanged;
		float scale = 1.0f;
		IDesignerHost host;
		InplaceEditorElementsBase InplaceEditorElements { 
			get { 
				if(scale < 1f)
					return new NormalInplaceEditorElements(this);
				return new ScaledInplaceEditorElements(this);
			}
		}
		IBandViewInfoService BandViewInfoService {
			get { return (IBandViewInfoService)host.GetService(typeof(IBandViewInfoService)); }
		}
		MenuCommandHandler MenuCommandHandler {
			get { return (MenuCommandHandler)host.GetService(typeof(MenuCommandHandler)); }
		}
		float ScaledPixelDpi { get { return ScaleValueF(DevExpress.XtraPrinting.GraphicsDpi.Pixel); }
		}
		public bool CanZoomIn { get { return ZoomIsCorrect(scale + ZoomStep); }	}
		public bool CanZoomOut { get { return ZoomIsCorrect(scale - ZoomStep); } }
		public bool CanScrollZoomIn { get { return ZoomIsCorrect(scale + ScrollZoomStep); } }
		public bool CanScrollZoomOut { get { return ZoomIsCorrect(scale - ScrollZoomStep); } }
		public bool CanChangeZoomFactor { 
			get { 
				if(host == null) return false;
				if(BandViewInfoService == null) return false;
				return MenuCommandHandler != null ? !MenuCommandHandler.IsInplaceEditorActive() : false;
			}
		}
		public int ZoomFactorInPercents { get { return (int)Math.Round((double)(scale * 100f)); }
		}
		public float ZoomFactor { 
			get { return scale; }
			set { SetScale(value); }
		}
		float ScrollZoomStep {
			get { return scale < 1 ? 0.5f * ZoomStep : scale < 2 ? ZoomStep : scale < 4 ? 2 * ZoomStep : 4 * ZoomStep; }
		}
		public ZoomService(IDesignerHost host) {
			this.host = host;
		}
		public void ZoomIn() {
			if(CanZoomIn) {
				SetScale(scale + ZoomStep);
			}
		}
		public void ZoomOut() {
			if(CanZoomOut) {
				SetScale(scale - ZoomStep);
			}
		}
		public void ScrollZoomIn() {
			if(CanScrollZoomIn) {
				SetScale(scale + ScrollZoomStep);
			}
		}
		public void ScrollZoomOut() {
			if(CanScrollZoomOut) {
				SetScale(scale - ScrollZoomStep);
			}
		}
		#region ToScaledPixels
		public Size ToScaledPixels(Size value, float fromDpi) {
			return XRConvert.Convert(value, fromDpi, ScaledPixelDpi);
		}
		public SizeF ToScaledPixels(SizeF value, float fromDpi) {
			return XRConvert.Convert(value, fromDpi, ScaledPixelDpi);
		}
		public Rectangle ToScaledPixels(Rectangle value, float fromDpi) {
			return XRConvert.Convert(value, fromDpi, ScaledPixelDpi);
		}
		public RectangleF ToScaledPixels(RectangleF value, float fromDpi) {
			return XRConvert.Convert(value, fromDpi, ScaledPixelDpi);
		}
		public Point ToScaledPixels(Point value, float fromDpi) {
			return XRConvert.Convert(value, fromDpi, ScaledPixelDpi);
		}
		public PointF ToScaledPixels(PointF value, float fromDpi) {
			return XRConvert.Convert(value, fromDpi, ScaledPixelDpi);
		}
		public int ToScaledPixels(int value, float fromDpi) {
			return XRConvert.Convert(value, fromDpi, ScaledPixelDpi);
		}
		public float ToScaledPixels(float value, float fromDpi) {
			return XRConvert.Convert(value, fromDpi, ScaledPixelDpi);
		}
		public RectangleF UnscaleRectangleF(RectangleF rect) {
			return XRConvert.Convert(rect, ScaledPixelDpi, GraphicsDpi.Pixel);
		}
		#endregion
		#region FromScaledPixels
		public Size FromScaledPixels(Size value, float toDpi) {
			return XRConvert.Convert(value, ScaledPixelDpi, toDpi);
		}
		public SizeF FromScaledPixels(SizeF value, float toDpi) {
			return XRConvert.Convert(value, ScaledPixelDpi, toDpi);
		}
		public Rectangle FromScaledPixels(Rectangle value, float toDpi) {
			return XRConvert.Convert(value, ScaledPixelDpi, toDpi);
		}
		public RectangleF FromScaledPixels(RectangleF value, float toDpi) {
			return XRConvert.Convert(value, ScaledPixelDpi, toDpi);
		}
		public Point FromScaledPixels(Point value, float toDpi) {
			return XRConvert.Convert(value, ScaledPixelDpi, toDpi); 
		}
		public PointF FromScaledPixels(PointF value, float toDpi) {
			return XRConvert.Convert(value, ScaledPixelDpi, toDpi);
		}
		public int FromScaledPixels(int value, float toDpi) {
			return XRConvert.Convert(value, ScaledPixelDpi, toDpi);
		}
		public float FromScaledPixels(float value, float toDpi) {
			return XRConvert.Convert(value, ScaledPixelDpi, toDpi);
		}
		public float FromScaledPixels(float value, Graphics gr) {
			return XRConvert.Convert(value, ScaledPixelDpi, GraphicsDpi.GetGraphicsDpi(gr));
		}
		#endregion
		#region scale methods
		public int UnscaleValue(int value) {
			return (int)(value / scale);
		}
		public PointF UnscalePointF(PointF value) {
			return new PointF(UnscaleValueF(value.X), UnscaleValueF(value.Y));
		}
		public Size UnscaleSize(Size value) {
			return new Size(UnscaleValue(value.Width), UnscaleValue(value.Height));
		}
		public Size ScaleSize(Size value) {
			return new Size(ScaleValue(value.Width), ScaleValue(value.Height));
		}
		public SizeF ScaleSizeF(SizeF value) {
			return new SizeF(ScaleValueF(value.Width), ScaleValueF(value.Height));
		}
		public Point ScalePoint(Point value) {
			return new Point(ScaleValue(value.X), ScaleValue(value.Y));
		}
		public PointF ScalePointF(PointF value) {
			return new PointF(ScaleValueF(value.X), ScaleValueF(value.Y));
		}
		public int ScaleValue(int value) {
			return (int)(value * scale);
		}
		public float ScaleValueF(float value) {
			return value * scale;
		}
		public float UnscaleValueF(float value) {
			return value / scale;
		}
		public double ScaleValueD(double value) {
			return value * (double)scale;
		}
		#endregion
		#region inplace editors
		public Size AdjustInplaceEditorSize(Size scaledSize) {
			return InplaceEditorElements.AdjustInplaceEditorSize(scaledSize);
		}
		public Font AdjustInplaceEditorFont(Font normalFont) {
			return InplaceEditorElements.AdjustInplaceEditorFont(normalFont);
		}
		public BorderStyle GetInplaceEditorBorderStyle() {
			return InplaceEditorElements.GetInplaceEditorBorderStyle();
		}
		public float GetInplaceEditorZoomFactor() {
			return InplaceEditorElements.GetInplaceEditorZoomFactor();
		}
		#endregion
		public void ScaleGraphics(Graphics gr) {
			gr.ScaleTransform(scale, scale);
		}
		void SetScale(float scale) {
			if(!ZoomIsCorrect(scale) || ValuesEquals(this.scale, scale) || !CanChangeZoomFactor) 
				return;
			System.Diagnostics.Debug.Assert(MenuCommandHandler != null && BandViewInfoService != null, "");
			SetScaleValue(scale);
			BandViewInfoService.UpdateView();
			MenuCommandHandler.UpdateCommandStatus();
			OnZoomChanged();
		}
		void SetScaleValue(float scale) {
			this.scale = scale; 
		}
		void OnZoomChanged() {
			if(ZoomChanged != null)
				ZoomChanged(this, EventArgs.Empty);
		}
	}
	public static class ServiceHelper{
		public static object GetServiceInstance(IServiceProvider serviceProvider, Type serviceType, object nullService) {
			if(serviceProvider == null) {
				System.Diagnostics.Debug.Assert(false);
				return nullService;
			}
			object result = serviceProvider.GetService(serviceType);
			if(result == null || !result.GetType().IsInstanceOfType(result)) {
				return nullService;
			}
			return result;
		}
	}
	public static class ProjectEnvironmentHelper {
		static IProjectEnvironmentService fProjectEnvironmentService;
		public static bool IsLightSwitchProject() {
			return fProjectEnvironmentService != null && fProjectEnvironmentService.IsLightSwitchProject();
		}
		public static void RegisterProjectEnvironmentService(IProjectEnvironmentService projectEnvironmentService) {
			fProjectEnvironmentService = projectEnvironmentService;
		}
	}
	public interface IProjectEnvironmentService {
		bool IsLightSwitchProject();
	}
}
