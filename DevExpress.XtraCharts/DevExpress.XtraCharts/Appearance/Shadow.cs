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
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts {
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public sealed class Shadow : ChartElement {
		const int DefaultSize = 2;
		static readonly Color DefaultColor = Color.FromArgb(80, Color.Black);
		bool defaultVisible = false;
		int size = DefaultSize;
		Color color = DefaultColor;
		bool visible;
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ShadowSize"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Shadow.Size"),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public int Size {
			get { return size; }
			set {
				if(value != size) {
					if(value <= 0)
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectShadowSize));
					SendNotification(new ElementWillChangeNotification(this));
					size = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ShadowColor"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Shadow.Color"),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public Color Color {
			get { return color; }
			set {
				if(value != color) {
					SendNotification(new ElementWillChangeNotification(this));
					color = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ShadowVisible"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Shadow.Visible"),
		TypeConverter(typeof(BooleanTypeConverter)),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public bool Visible {
			get { return visible; }
			set {
				if(value != visible) {
					SendNotification(new ElementWillChangeNotification(this));
					visible = value;
					RaiseControlChanged();
				}
			}
		}
		internal Shadow(ChartElement owner) : this(owner, false) {
		}
		internal Shadow(ChartElement owner, bool defaultVisible) : base(owner) {
			this.visible = defaultVisible;
			this.defaultVisible = defaultVisible;
		}
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			if(propertyName == "Color")
				return ShouldSerializeColor();
			if(propertyName == "Visible")
				return ShouldSerializeVisible();
			if(propertyName == "Size")
				return ShouldSerializeSize();
			return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeColor() {
			return color != DefaultColor;
		}
		void ResetColor() {
			Color = DefaultColor;
		}
		bool ShouldSerializeVisible() {
			return visible != this.defaultVisible;
		}
		void ResetVisible() {
			Visible = this.defaultVisible;
		}
		bool ShouldSerializeSize() {
			return this.size != DefaultSize;
		}
		void ResetSize() {
			Size = DefaultSize;
		}
		protected internal override bool ShouldSerialize() {
			return
				base.ShouldSerialize() ||
				ShouldSerializeColor() ||
				ShouldSerializeVisible() ||
				ShouldSerializeSize();
		}
		#endregion
		Size IncreaseSize(Size size, int shadowSize) {
			size.Width += GetActualSize(shadowSize);
			size.Height += GetActualSize(shadowSize);
			return size;
		}
		protected override ChartElement CreateObjectForClone() {
			return new Shadow(null);
		}		
		internal Size DecreaseSize(Size size, int shadowSize) {
			size.Width -= GetActualSize(shadowSize);
			size.Height -= GetActualSize(shadowSize);
			return size;
		}
		internal Size IncreaseSize(Size size) {
			return IncreaseSize(size, -1);
		}
		internal Size DecreaseSize(Size size) {
			return DecreaseSize(size, -1);
		}
		internal GraphicsCommand CreateGraphicsCommand(GraphicsCommand shadowCommand) {
			return CreateGraphicsCommand(shadowCommand, size);
		}
		internal void BeforeShadowRender(IRenderer renderer) {
			BeforeShadowRender(renderer, size);
		}
		internal void AfterShadowRender(IRenderer renderer) {
			AfterShadowRender(renderer, size);	
		}
		internal GraphicsCommand CreateGraphicsCommand(GraphicsCommand shadowCommand, int shadowSize) {
			if (!visible)
				return null;
			GraphicsCommand command = new SaveStateGraphicsCommand();
			GraphicsCommand translateCommand = new TranslateGraphicsCommand(shadowSize, shadowSize);
			command.AddChildCommand(translateCommand);
			GraphicsCommand smoothCommand = new PolygonAntialiasingGraphicsCommand();
			translateCommand.AddChildCommand(smoothCommand);
			smoothCommand.AddChildCommand(shadowCommand);
			return command;
		}
		internal void BeforeShadowRender(IRenderer renderer, int shadowSize) {
			if (!visible)
				return;
			renderer.SaveState();
			renderer.TranslateModel(new Point(shadowSize, shadowSize));
			renderer.EnablePolygonAntialiasing(true);
		}
		internal void AfterShadowRender(IRenderer renderer, int shadowSize) {
			if (!visible)
				return;
			renderer.RestorePolygonAntialiasing();
			renderer.RestoreState();
		}
		internal void Render(IRenderer renderer, LineStrip strip, int thickness) {
			Render(renderer, strip, thickness, -1);
		}
		internal void Render(IRenderer renderer, LineStrip strip, int thickness, int shadowSize) {
			if (!visible)
				return;
			if (shadowSize < 0)
				shadowSize = size;
			renderer.SaveState();
			renderer.TranslateModel(new Point(shadowSize, shadowSize));
			renderer.EnableAntialiasing(true);
			StripsUtils.Render(renderer, strip, color, thickness);
			renderer.RestoreAntialiasing();
			renderer.RestoreState();
		}
		internal GraphicsCommand CreateGraphicsCommand(ZPlaneRectangle rect) {
			return CreateGraphicsCommand(rect, -1);
		}
		internal void Render(IRenderer renderer, RectangleF rect) {
			Render(renderer, rect, -1);
		}
		internal GraphicsCommand CreateGraphicsCommand(ZPlaneRectangle rect, int shadowSize) {
			if (!visible)
				return null;
			if (shadowSize < 0)
				shadowSize = size;
			GraphicsCommand command = new SaveStateGraphicsCommand();
			GraphicsCommand translateCommand = new TranslateGraphicsCommand(shadowSize, shadowSize);
			command.AddChildCommand(translateCommand);
			GraphicsCommand smoothCommand = new PolygonAntialiasingGraphicsCommand();
			translateCommand.AddChildCommand(smoothCommand);
			smoothCommand.AddChildCommand(new SolidRectangleGraphicsCommand(rect, color));
			return command;
		}
		internal void Render(IRenderer renderer, RectangleF rect, int shadowSize) {
			if (!visible)
				return;
			if (shadowSize < 0)
				shadowSize = size;
			try {
				renderer.SaveState();
				renderer.EnableAntialiasing(true);
				rect.X += shadowSize;
				rect.Y += shadowSize;
				renderer.FillRectangle(rect, color);
			}
			finally {
				renderer.RestoreAntialiasing();
				renderer.RestoreState();
			}
		}
		internal void Render(IRenderer renderer, GRealPoint2D center, float radius) {
			Render(renderer, center, radius, -1);
		}
		internal void Render(IRenderer renderer, GRealPoint2D center, float radius, int shadowSize) {
			if (!visible)
				return;
			if (shadowSize < 0)
				shadowSize = size;
			renderer.SaveState();
			renderer.TranslateModel(new Point(shadowSize, shadowSize));
			renderer.EnablePolygonAntialiasing(true);
			renderer.FillCircle(new Point((int)center.X, (int)center.Y), (int)radius, color);
			renderer.RestorePolygonAntialiasing();
			renderer.RestoreState();
		}
		internal void Render(IRenderer renderer, VariousPolygon polygon) {
			Render(renderer, polygon, -1);
		}
		internal void Render(IRenderer renderer, VariousPolygon polygon, int shadowSize) {
			if (!visible)
				return;
			if (shadowSize < 0)
				shadowSize = size;
			renderer.SaveState();
			renderer.TranslateModel(new Point(shadowSize, shadowSize));
			renderer.EnablePolygonAntialiasing(true);
			polygon.Render(renderer, color);
			renderer.RestorePolygonAntialiasing();
			renderer.RestoreState();
		}
		internal void Render(IRenderer renderer, RangeStrip strip) {
			Render(renderer, strip, -1);
		}
		internal void Render(IRenderer renderer, RangeStrip strip, int shadowSize) {
			if (!visible)
				return;
			if (shadowSize < 0)
				shadowSize = size;
			renderer.SaveState();
			renderer.TranslateModel(new Point(shadowSize, shadowSize));
			renderer.EnablePolygonAntialiasing(true);
			StripsUtils.Render(renderer, strip, color);
			renderer.RestorePolygonAntialiasing();
			renderer.RestoreState();
		}
		internal int GetActualSize(int shadowSize) {
			if (!visible)
				return 0;
			return shadowSize < 0 ? size : shadowSize;
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			Shadow shadow = obj as Shadow;
			if (shadow == null)
				return;
			size = shadow.size;
			color = shadow.color;
			visible = shadow.visible;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			Shadow shadow = obj as Shadow;
			return shadow != null && size == shadow.size && color == shadow.color && visible == shadow.visible;
		}
	}
}
