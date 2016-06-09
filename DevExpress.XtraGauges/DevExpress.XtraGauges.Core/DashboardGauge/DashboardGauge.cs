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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using DevExpress.Utils;
using DevExpress.XtraGauges.Base;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Drawing;
using DevExpress.XtraGauges.Core.Model;
using DevExpress.XtraGauges.Core.Primitive;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing.Imaging;
namespace DevExpress.XtraGauges.Core.Customization {
	public interface IDashboardGauge {
		BaseGaugeModel Model { get; }
		string LabelFormatString { get; set; }
		bool ShowMarker { get; }
		bool ShowDelta { get; set; }
		float MaxValue { get; set; }
		float MinValue { get; set; }
		float Value { get; set; }
		float MarkerValue { get; set; }
		DashboardGaugeType Type { get; set; }
		DashboardGaugeTheme Theme { get; set; }
		DashboardGaugeStyle Style { get; set; }
		Rectangle Bounds { get; set; }
		void Render(Graphics g);
		List<ISerizalizeableElement> Elements { get; }
		BaseDiscreteScaleProvider Scale { get; set; }
		ValueIndicatorComponent<BaseMarkerProvider> Marker { get; set; }
		int MajorTickCount { get; set; }
		Image GetImage();
		DevExpress.Utils.DefaultBoolean AutoSize { get; set; }
		GraphicsProperties GraphicsProperties { get; }
	}
	public class DashboardGauge : BaseGauge, IDashboardGauge {
		public DashboardGauge() {
			elementsCore = new List<ISerizalizeableElement>();
		}
		public void CalcModel(Graphics g) {
			Model.BeginTransform();
			CalculateModel();
			RaiseChanged();
			Model.EndTransform();
			Model.CalculateBackgroundShape(Bounds);
		}
		protected override void OnModelChanged(bool reset) {
			base.OnModelChanged(reset);
			if(IsUpdateLocked || IsDisposing) return;
			CalculateModelTransform();
		}
		public DevExpress.Utils.DefaultBoolean AutoSize {
			get { return base.AutoSizeByActualBounds; }
			set { base.AutoSizeByActualBounds = value; }
		}
		public GraphicsProperties GraphicsProperties { get; private set; }
		RectangleF cachedContentRect = RectangleF.Empty;
		protected override void SetModelProportions() {
			SizeF contentSize = Model.ContentSize;
			bool isSmartLayout = Model.SmartLayout && AutoSizeByActualBounds != DefaultBoolean.False;
			if(!ProportionalStretch && !isSmartLayout) {
				float sx = (float)Bounds.Width / contentSize.Width;
				float sy = (float)Bounds.Height / contentSize.Height;
				Model.ScaleFactor = new FactorF2D(sx, sy);
				Model.Location = new PointF(
					(float)Bounds.Left + (Bounds.Width - contentSize.Width * sx) * 0.5f,
					(float)Bounds.Top + (Bounds.Height - contentSize.Height * sy) * 0.5f);
			}
			else {
				RectangleF contentRect = RectangleF.Empty;
				if(isSmartLayout) {
					if(cachedContentRect.IsEmpty)
						cachedContentRect = CalcContentRect(Model.Composite);
					if(cachedContentRect.IsEmpty)
						return;
					contentRect = cachedContentRect;
				}
				else contentRect = new RectangleF(Point.Empty, Model.ContentSize);
				float sx = (float)Bounds.Width / contentRect.Width;
				float sy = (float)Bounds.Height / contentRect.Height;
				float scaleFactor = Math.Min(sx, sy);
				Model.ScaleFactor = new FactorF2D(scaleFactor, scaleFactor);
				Model.Location = new PointF(
					(float)Bounds.Left - contentRect.Left * scaleFactor + (Bounds.Width - contentRect.Width * scaleFactor) * 0.5f,
					(float)Bounds.Top - contentRect.Top * scaleFactor + (Bounds.Height - contentRect.Height * scaleFactor) * 0.5f);
			}
		}
		class GraphicsCustomizer : IDisposable {
			GraphicsState savedState;
			Graphics graphicsCore;
			public GraphicsCustomizer(Graphics g, GraphicsProperties graphicsProperties) {
				savedState = g.Save();
				graphicsCore = g;
				g.SmoothingMode = graphicsProperties.SmoothingMode;
				g.InterpolationMode = graphicsProperties.InterpolationMode;
				g.CompositingQuality = graphicsProperties.CompositingQuality;
				g.TextRenderingHint = graphicsProperties.TextRenderingHint;
			}
			public void Dispose() {
				graphicsCore.Restore(savedState);
			}
		}
		public void Render(Graphics g) {
			using(new GraphicsCustomizer(g, GraphicsProperties)) {
				using(IRenderingContext rContext = RenderingContext.FromGraphics(g)) {
					CalcModel(g);
					Model.Self.Render(rContext);
				}
			}
		}
		public Image GetImage() {
			Image image = new Bitmap(Bounds.Width, Bounds.Height, PixelFormat.Format32bppArgb);
			using(Graphics g = Graphics.FromImage(image)) 
				Render(g);
			return image;
		}
		List<ISerizalizeableElement> elementsCore;
		public List<ISerizalizeableElement> Elements {
			get { return elementsCore; }
		}
		protected override List<ISerizalizeableElement> GetChildernCore() {
			List<ISerizalizeableElement> list = new List<ISerizalizeableElement>(base.GetChildernCore());
			CollectChildren(list, Elements, string.Empty);
			return list;
		}
		protected override void InitializeDefaultCore() { }
		protected override void OnCreate() {
			GraphicsProperties = new GraphicsProperties();
		}
		protected override void OnDispose() {
			elementsCore.Clear();
			DestroyModel();
		}
		protected override BaseGaugeModel CreateModel() {
			return new DashboardGaugeModel(this);
		}
		protected override void ClearCore() { }
		protected override System.Collections.Generic.List<string> GetNamesCore() { return new List<string>(); }
		protected override void SetEnabledCore(bool enabled) { }
		protected override void AddGaugeElementToComponentCollection(IComponent component) { }
		protected override BaseElement<IRenderableElement> DuplicateGaugeElementCore(BaseElement<IRenderableElement> element) { return element; }
		public BaseDiscreteScaleProvider Scale { get; set; }
		#region IDashboardGauge Members
		public string LabelFormatString {
			get { return Scale.MajorTickmark.FormatString; }
			set { Scale.MajorTickmark.FormatString = value; }
		}
		bool showMarkerCore;
		public bool ShowMarker {
			get { return showMarkerCore; }
			set { showMarkerCore = value; }
		}
		bool showDeltaCore;
		public bool ShowDelta {
			get { return showDeltaCore; }
			set { showDeltaCore = value; }
		}
		public float MaxValue {
			get { return Scale.MaxValue; }
			set { Scale.MaxValue = value; }
		}
		public float MinValue {
			get { return Scale.MinValue; }
			set { Scale.MinValue = value; }
		}
		public float Value {
			get { return Scale.Value; }
			set { Scale.Value = value; }
		}
		DashboardGaugeType typeCore;
		public DashboardGaugeType Type {
			get { return typeCore; }
			set { typeCore = value; }
		}
		DashboardGaugeTheme themeNameCore;
		public DashboardGaugeTheme Theme {
			get { return themeNameCore; }
			set { themeNameCore = value; }
		}
		DashboardGaugeStyle styleNameCore;
		public DashboardGaugeStyle Style {
			get { return styleNameCore; }
			set { styleNameCore = value; }
		}
		public float MarkerValue {
			get {
				if(Marker != null && Marker.Value.HasValue)
					return Marker.Value.Value;
				else
					return Scale.Value;
			}
			set {
				if(Marker != null)
					Marker.Value = value;
			}
		}
		public int MajorTickCount {
			get { return Scale.MajorTickCount; }
			set { Scale.MajorTickCount = value; }
		}
		public ValueIndicatorComponent<BaseMarkerProvider> Marker { get; set; }
		#endregion
	}
	public class DashboardGaugeModel : BaseGaugeModel {
		BaseGauge gaugeCore;
		public DashboardGaugeModel(BaseGauge gauge) : base(gauge) { gaugeCore = gauge; }
		public override void Calc(IGauge owner, RectangleF bounds) {
			OnShapesChanged();
			base.Calc(owner, bounds);
		}
		public override bool SmartLayout {
			get { return true; }
		}
		public override SizeF ContentSize {
			get {
				var gauge = Owner as IDashboardGauge;
				if(gauge.Type == DashboardGaugeType.Circular)
					return new SizeF(250, 250);
				else
					return (gauge.Style == DashboardGaugeStyle.Horizontal) ? new SizeF(250, 125) : new SizeF(125, 250);
			}
		}
	}
}
