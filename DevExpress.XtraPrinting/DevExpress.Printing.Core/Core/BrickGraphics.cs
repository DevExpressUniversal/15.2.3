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
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.UI;
using DevExpress.XtraPrinting.NativeBricks;
#if SL
using System.Windows.Media;
using System.Windows;
#endif
namespace DevExpress.XtraPrinting {
	public class BrickGraphics : IDisposable, IBrickGraphics {
		[ThreadStatic]
		static BrickStyle internalBrickStyle;
		[ThreadStatic]
		static BrickStyle internalBrickStyleDefault;
		[ThreadStatic]
		static Stack<BrickStyle> brickStyleStack;
#if SL
		internal static Color DefaultPageBackColor = SystemColors.WindowColor;
#else
		internal static Color DefaultPageBackColor = SystemColors.Window;
#endif
		private int stackLevel;	
		private float dpi = GraphicsDpi.Pixel;
		private Color pageBackColor;
		private BrickModifier modifier;
		private PrintingSystemBase ps;
		private GraphicsUnit pageUnit = GraphicsUnit.Pixel;
		private bool deviceIndependentPixel = false;
		internal static BrickStyle InternalBrickStyle {
			get { return internalBrickStyle != null ? internalBrickStyle : InternalBrickStyleDefault; }
		}
		static BrickStyle InternalBrickStyleDefault {
			get {
				if(internalBrickStyleDefault == null)
					internalBrickStyleDefault = BrickStyle.CreateDefault();
				return internalBrickStyleDefault;
			}
		}
		static Stack<BrickStyle> BrickStyleStack {
			get {
				if(brickStyleStack == null)
					brickStyleStack = new Stack<BrickStyle>();
				return brickStyleStack;
			}
		}
#if DEBUGTEST
		public static BrickStyle Test_InternalBrickStyleDefault {
			get { return internalBrickStyleDefault; }
			set { internalBrickStyleDefault = value; }
		}
		public static BrickStyle Test_InternalBrickStyle {
			get { return internalBrickStyle; }
			set { internalBrickStyle = value; }
		}
		public static Stack<BrickStyle> Test_BrickStyleStack {
			get { return BrickStyleStack; }
		}
		public int Test_StackLevel {
			get { return stackLevel; }
			set { stackLevel = value; }
		}
#endif
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("BrickGraphicsDefaultBrickStyle")]
#endif
		public BrickStyle DefaultBrickStyle {
			get { return InternalBrickStyle; }
			set {
				internalBrickStyle = value;
			}
		}
		internal float Dpi { get { return dpi; } }
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("BrickGraphicsPageBackColor")]
#endif
		public Color PageBackColor { 
			get { return pageBackColor; } 
			set {
				pageBackColor = value;
				ps.RaisePageBackgrChanged(EventArgs.Empty);
			} 
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("BrickGraphicsModifier")]
#endif
		public BrickModifier Modifier {
			get { return modifier; }
			set {
				if(modifier != value) {
					modifier = value;
					RaiseModifierChangedInternal();
				}
			}
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("BrickGraphicsPageUnit")]
#endif
		public GraphicsUnit PageUnit {
			get { return pageUnit; }
			set {
				pageUnit = value;
				UpdateDpi();
			}
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("BrickGraphicsDeviceIndependentPixel")]
#endif
		public bool DeviceIndependentPixel {
			get { return deviceIndependentPixel; }
			set {
				deviceIndependentPixel = value;
				UpdateDpi();
			}
		}
		void UpdateDpi() {
			dpi = IsUsingDip ? GraphicsDpi.DeviceIndependentPixel : GraphicsDpi.UnitToDpi(pageUnit);
		}
		bool IsUsingDip {
			get { return pageUnit == GraphicsUnit.Pixel && deviceIndependentPixel; }
		}
		internal event EventHandler ModifierChanged;
		#region inner classes
		class UnionCollection {
			static void AdjustBoundsByBorder(ref RectangleF bounds, BorderSide borderSides, float borderWidth) {
				if((borderSides & BorderSide.Top) != 0) {
					bounds.Y -= borderWidth;
					bounds.Height += borderWidth;
				}
				if((borderSides & BorderSide.Left) != 0) {
					bounds.X -= borderWidth;
					bounds.Width += borderWidth;
				}
				if((borderSides & BorderSide.Bottom) != 0)
					bounds.Height += borderWidth;
				if((borderSides & BorderSide.Right) != 0)
					bounds.Width += borderWidth;
			}
			static RectangleF GetRealBrickBounds(Brick brick) {
				VisualBrick visualBrick = brick as VisualBrick;
				RectangleF result = brick.Rect;
				float borderWidth = XRConvert.Convert(visualBrick.BorderWidth, GraphicsDpi.Pixel, GraphicsDpi.Document);
				if(visualBrick != null) {
					switch(visualBrick.BorderStyle) {
						case BrickBorderStyle.Outset:
							AdjustBoundsByBorder(ref result, visualBrick.Sides, borderWidth);
							break;
						case BrickBorderStyle.Center:
							AdjustBoundsByBorder(ref result, visualBrick.Sides, borderWidth / 2f);
							break;
						case BrickBorderStyle.Inset:
							break;
					}
				}
				return result;
			}
			List<Brick> bricks = new List<Brick>();
			internal void Add(Brick brick) {
				bricks.Add(brick);
			}
			internal void Unite(PanelBrick unionBrick) {
				RectangleF bounds = RectangleF.Empty;
				foreach(Brick brick in bricks) {
					RectangleF brickRealBounds = brick.Rect;
					if(bounds.IsEmpty)
						bounds = brickRealBounds;
					else
						bounds = RectangleF.Union(bounds, brickRealBounds);
				}
				foreach(Brick brick in bricks) {
					brick.Location = new PointF(Math.Max(0f, brick.Location.X - bounds.Location.X), Math.Max(0f, brick.Location.Y - bounds.Location.Y));
					unionBrick.Bricks.Add(brick);
				}
				bricks.Clear();
				unionBrick.Location = bounds.Location;
				unionBrick.Size = bounds.Size;
			}
		}
		#endregion
#if !SL
		public static SizeF MeasureString(string text, Font font, int width, StringFormat stringFormat, GraphicsUnit pageUnit) {
			return Measurement.MeasureString(text, font, width, stringFormat, pageUnit);
		}
#endif
		#region Fields & Properties
		private PanelBrick unionBrick;
		private UnionCollection unionBricks = new UnionCollection();
		Action<Brick> brickCollector;
		internal Action<Brick> BrickCollector {
			get { return brickCollector != null ? brickCollector : ps.Document.AddBrick; }
			set { brickCollector = value; }
		}
#if !SL
		internal Measurer Measurer { get { return ((IPrintingSystemContext)this.PrintingSystem).Measurer; } }
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("BrickGraphicsBorderWidth")]
#endif
		public float BorderWidth { get { return InternalBrickStyle.BorderWidth; } set { InternalBrickStyle.BorderWidth = value; } }
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("BrickGraphicsBorderColor")]
#endif
		public Color BorderColor { get { return InternalBrickStyle.BorderColor; } set { InternalBrickStyle.BorderColor = value; } }
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("BrickGraphicsBackColor")]
#endif
		public Color BackColor { get { return InternalBrickStyle.BackColor; } set { InternalBrickStyle.BackColor = value; } }
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("BrickGraphicsForeColor")]
#endif
		public Color ForeColor { get { return InternalBrickStyle.ForeColor; } set { InternalBrickStyle.ForeColor = value; } }
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("BrickGraphicsFont")]
#endif
		public Font Font { get { return InternalBrickStyle.Font; } set { InternalBrickStyle.Font = (Font)value.Clone(); } }
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("BrickGraphicsDefaultFont")]
#endif
		public Font DefaultFont { get { return BrickStyle.DefaultFont; } set { ;} }
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("BrickGraphicsStringFormat")]
#endif
		public BrickStringFormat StringFormat { get { return InternalBrickStyle.StringFormat; } set { InternalBrickStyle.StringFormat = (BrickStringFormat)value.Clone(); } }
#endif
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("BrickGraphicsPrintingSystem")]
#endif
		public PrintingSystemBase PrintingSystem { get { return ps; } }
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("BrickGraphicsClientPageSize")]
#endif
		public SizeF ClientPageSize {
			get {
				SizeF size = ps.PageSettings.UsefulPageRectF.Size;
				return GraphicsUnitConverter.Convert(size, GraphicsDpi.Document, dpi);
			}
		}
		#endregion
		void IBrickGraphics.RaiseModifierChanged() {
			RaiseModifierChangedInternal();
		}
		public void BeginUnionRect() {
			unionBrick = CreatePanelUnionBrick();
			unionBrick.Modifier = Modifier;
			unionBrick.NoClip = true;
		}
		public void EndUnionRect() {
			if(unionBrick != null) {
				unionBricks.Unite(unionBrick);
				PanelBrick resultPanelUnionBrick = unionBrick;
				unionBrick = null;
				AddBrick(resultPanelUnionBrick);
			}
		}
#if !SL
		[Obsolete]
		public void BeginCalculateRectangle() {
		}
		[Obsolete]
		public void UnionCalculateRectangle(RectangleF rect) {
		}
		[Obsolete]
		public RectangleF EndCalculateRectangle() {
			return RectangleF.Empty;
		}
		public SizeF MeasureString(string text, Font font, int width, StringFormat stringFormat) {
			SizeF result = Measurer.MeasureString(text, font, width, stringFormat, GetMeasuringPageUnit());
			return ConverFromMeasuringUnit(result);
		}
		public SizeF MeasureString(string text, Font font) {
			SizeF result = Measurer.MeasureString(text, font, GetMeasuringPageUnit());
			return ConverFromMeasuringUnit(result);
		}
		public SizeF MeasureString(string text, int width, StringFormat stringFormat) {
			SizeF result = Measurer.MeasureString(text, Font, width, stringFormat, GetMeasuringPageUnit());
			return ConverFromMeasuringUnit(result);
		}
		public SizeF MeasureString(string text, int width) {
			SizeF result = Measurer.MeasureString(text, Font, width, null, GetMeasuringPageUnit());
			return ConverFromMeasuringUnit(result);
		}
		public SizeF MeasureString(string text) {
			SizeF result = Measurer.MeasureString(text, Font, GetMeasuringPageUnit());
			return ConverFromMeasuringUnit(result);
		}
		GraphicsUnit GetMeasuringPageUnit() {
			return IsUsingDip ? GraphicsUnit.Document : pageUnit;
		}
		private SizeF ConverFromMeasuringUnit(SizeF result) {
			return IsUsingDip ? XRConvert.Convert(result, GraphicsDpi.Document, dpi) : result;
		}
		public float DocumValueOf(float val) {
			return GraphicsUnitConverter.Convert(val, dpi, GraphicsDpi.Document);
		}
		public float UnitValueOf(float val) {
			return GraphicsUnitConverter.Convert(val, GraphicsDpi.Document, dpi);
		}
#endif
		#region Draw Functions
		IBrick IBrickGraphics.DrawBrick(IBrick userBrick, RectangleF rect) {
			if(userBrick is Brick)
				return DrawBrick((Brick)userBrick, rect);
			Brick brick = CreateUserBrick(userBrick);
			return DrawBrick(brick, rect);
		}
		private Brick CreateUserBrick(IBrick userBrick) {
			if((modifier & (BrickModifier.MarginalHeader | BrickModifier.MarginalFooter)) > 0)
				return new UserPageBrick(userBrick);
			return new UserVisualBrick(userBrick);
		}
		public Brick DrawBrick(Brick brick, RectangleF rect) {
			InitializeBrick(brick, rect);
			return AddBrick(brick);
		}
		public Brick DrawBrick(Brick brick) {
			InitializeBrick(brick, brick.Rect);
			return AddBrick(brick);
		}
		PanelBrick CreatePanelUnionBrick() {
			PanelBrick brick = new PanelBrick();
#if SL
			brick.BackColor = Colors.Transparent;
#else
			brick.BackColor = Color.Transparent;
#endif
			brick.BorderWidth = 0f;
			InitializeBrick(brick, RectangleF.Empty);
			return brick;
		}
#if !SL
		EmptyBrick CreateEmptyBrick(RectangleF rect) {
			EmptyBrick brick = new EmptyBrick();
			InitializeBrick(brick, rect);
			return brick;
		}
		public EmptyBrick DrawEmptyBrick(RectangleF rect) {
			return (EmptyBrick)AddBrick(CreateEmptyBrick(rect));
		}
		public VisualBrick DrawRect(RectangleF rect, BorderSide sides, Color backColor, Color borderColor) {
			SeparableBrick brick = new SeparableBrick();
			InitializeBrick(brick, rect);
			brick.Style = new BrickStyle(sides, BorderWidth,
				borderColor.IsEmpty ? BorderColor : borderColor,
				backColor.IsEmpty ? BackColor : backColor, ForeColor, Font,
				StringFormat);
			return (VisualBrick)AddBrick(brick);
		}
		public TextBrick DrawString(String text, RectangleF rect) {
			TextBrick brick = new TextBrick();
			InitializeBrick(brick, rect);
			brick.Style = new BrickStyle(DefaultBrickStyle);
			brick.Text = text;
			return (TextBrick)AddBrick(brick);
		}
		public TextBrick DrawString(String text, Color foreColor, RectangleF rect, BorderSide sides) {
			TextBrick brick = new TextBrick();
			InitializeBrick(brick, rect);
			brick.Style = new BrickStyle(sides, BorderWidth, BorderColor,
				BackColor, foreColor.IsEmpty ? ForeColor : foreColor, Font,
				StringFormat);
			brick.Text = text;
			return (TextBrick)AddBrick(brick);
		}
		public PageInfoBrick DrawPageInfo(PageInfo pageInfo, string format, Color foreColor, RectangleF rect, BorderSide sides) {
			PageInfoBrick brick = new PageInfoBrick();
			InitializeBrick(brick, rect);
			brick.Style = new BrickStyle(sides, BorderWidth, BorderColor,
				BackColor, foreColor.IsEmpty ? ForeColor : foreColor, Font,
				StringFormat);
			brick.Format = format;
			brick.PageInfo = pageInfo;
			return (PageInfoBrick)AddBrick(brick);
		}
		public ImageBrick DrawImage(Image image, RectangleF rect) {
			ImageBrick brick = new ImageBrick();
			InitializeBrick(brick, rect);
			brick.Style = new BrickStyle(DefaultBrickStyle);
			brick.Image = image;
			return (ImageBrick)AddBrick(brick);
		}
		public ImageBrick DrawImage(Image image, RectangleF rect, BorderSide sides, Color backColor) {
			ImageBrick brick = new ImageBrick();
			InitializeBrick(brick, rect);
			brick.Style = new BrickStyle(sides, BorderWidth, BorderColor,
				backColor.IsEmpty ? BackColor : backColor, ForeColor, Font,
				StringFormat);
			brick.Image = image;
			return (ImageBrick)AddBrick(brick);
		}
		public PageImageBrick DrawPageImage(Image image, RectangleF rect, BorderSide sides, Color backColor) {
			PageImageBrick brick = new PageImageBrick();
			InitializeBrick(brick, rect);
			brick.Style = new BrickStyle(sides, BorderWidth, BorderColor,
				backColor.IsEmpty ? BackColor : backColor, ForeColor, Font,
				StringFormat);
			brick.Image = image;
			return (PageImageBrick)AddBrick(brick);
		}
		public CheckBoxBrick DrawCheckBox(RectangleF rect, bool check) {
			CheckBoxBrick brick = new CheckBoxBrick();
			InitializeBrick(brick, rect);
			brick.Style = new BrickStyle(DefaultBrickStyle);
			brick.Checked = check;
			return (CheckBoxBrick)AddBrick(brick);
		}
		public CheckBoxBrick DrawCheckBox(RectangleF rect, BorderSide sides, Color backColor, bool check) {
			CheckBoxBrick brick = new CheckBoxBrick();
			InitializeBrick(brick, rect);
			brick.Style = new BrickStyle(sides, BorderWidth, BorderColor,
				backColor.IsEmpty ? BackColor : backColor, ForeColor, Font,
				StringFormat);
			brick.Checked = check;
			return (CheckBoxBrick)AddBrick(brick);
		}
		public LineBrick DrawLine(PointF pt1, PointF pt2, Color foreColor, float width) {
			LineBrick brick = new LineBrick(ps, GraphicsUnitConverter.Convert(pt1, dpi, GraphicsDpi.Document), GraphicsUnitConverter.Convert(pt2, dpi, GraphicsDpi.Document), GraphicsUnitConverter.PixelToDoc(width));
			brick.ForeColor = foreColor;
			return (LineBrick)AddBrick(brick);
		}
#endif
		#endregion
		protected virtual Brick AddBrick(Brick brick) {
			if(brick != null) {
				brick.Modifier = modifier;
				if(unionBrick != null)
					unionBricks.Add(brick);
				else
					BrickCollector.Invoke(brick);
			}
			return brick;
		}
		public BrickGraphics(PrintingSystemBase ps) {
			this.ps = ps;
			pageBackColor = DefaultPageBackColor;
			modifier = BrickModifier.None;
		}
		void IDisposable.Dispose() {
			this.ps = null;
			Clear();
		}
		internal void Init() {
			BrickStyleStack.Push(internalBrickStyle);
			this.stackLevel = BrickStyleStack.Count;
		}
		internal void Clear() {
			ClearInternalBrickStyle();
			if(internalBrickStyleDefault != null) {
				internalBrickStyleDefault.Dispose();
				internalBrickStyleDefault = null;
			}
		}
		void ClearInternalBrickStyle() {
			BrickStyle lastBrickStyle = null;
			while(BrickStyleStack.Count >= this.stackLevel) {
				lastBrickStyle = BrickStyleStack.Count > 0 ? BrickStyleStack.Pop() : null;
				if(!object.ReferenceEquals(lastBrickStyle, internalBrickStyle) && internalBrickStyle != null)
					internalBrickStyle.Dispose();
				internalBrickStyle = lastBrickStyle;
				if(BrickStyleStack.Count == 0)
					break;
			}
		}
#if DEBUGTEST
		public void Test_ClearInternalBrickStyle() {
			ClearInternalBrickStyle();
		}
#endif
		internal void InitializeBrick(Brick brick, RectangleF rect, bool cacheStyle) {
			brick.Initialize(ps, GraphicsUnitConverter.Convert(rect, Dpi, GraphicsDpi.Document), cacheStyle);
		}
		internal void InitializeBrick(Brick brick, RectangleF rect) {
			InitializeBrick(brick, rect, true);
		}
		void RaiseModifierChangedInternal() {
			if(ModifierChanged != null)
				ModifierChanged(this, EventArgs.Empty);
		}
	}
}
