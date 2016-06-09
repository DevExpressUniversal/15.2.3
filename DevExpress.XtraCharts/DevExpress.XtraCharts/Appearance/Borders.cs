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
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts {
	public abstract class BorderBase : ChartElement {
		const int DefaultThickness = 1;
		const DefaultBoolean DefaultVisibility = DefaultBoolean.Default;
		#region fields and properties
		bool defaultVisible;
		Color defaultColor = Color.Empty;
		Color color;
		int thickness = DefaultThickness;
		DefaultBoolean visibility = DefaultBoolean.Default;
		internal int ActualThickness { get { return GetActualThickness(ActualVisibility); } }
		internal bool ActualVisibility { get { return GetActualVisibility(Owner as ISupportBorderVisibility); } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the Visibility property instead."),
		XtraSerializableProperty(XtraSerializationVisibility.Visible)
		]
		public bool Visible {
			get { return ActualVisibility; }
			set { Visibility = DefaultBooleanUtils.ToDefaultBoolean(value); }
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("BorderBaseColor"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.BorderBase.Color"),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public Color Color {
			get { return color; }
			set {
				if(color == value)
					return;
				SendNotification(new ElementWillChangeNotification(this));
				color = value;
				RaiseControlChanged();
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("BorderBaseThickness"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.BorderBase.Thickness"),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public int Thickness {
			get { return thickness; }
			set {
				if(value <= 0)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectBorderThickness));
				if(thickness == value)
					return;
				SendNotification(new ElementWillChangeNotification(this));
				thickness = value;
				RaiseControlChanged();
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("BorderBaseVisibility"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.BorderBase.Visibility"),
		TypeConverter(typeof(DefaultBooleanConverter)),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public DefaultBoolean Visibility {
			get { return visibility; }
			set {
				if (visibility != value) {
					SendNotification(new ElementWillChangeNotification(this));
					visibility = value;
					RaiseControlChanged();
				}
			}
		}
		#endregion
		protected BorderBase(ChartElement owner, bool visible, Color color) : base(owner) {
			InitializeVisible(visible);
			this.color = color;
		}
		#region XtraSeralizing
		protected override bool XtraShouldSerialize(string propertyName) {
			if(propertyName == "Color")
				return ShouldSerializeColor();
			if(propertyName == "Thickness")
				return ShouldSerializeThickness();
			if(propertyName == "Visible")
				return ShouldSerializeVisible();
			if (propertyName == "Visibility")
				return ShouldSerializeVisibility();
			return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeVisible() {
			return false;
		}
		bool ShouldSerializeVisibility() {
			return Visibility != DefaultVisibility;
		}
		void ResetVisibility() {
			Visibility = DefaultVisibility;
		}
		bool ShouldSerializeColor() {
			return !color.IsEmpty;
		}
		bool ShouldSerializeThickness() {
			return this.thickness != DefaultThickness;
		}
		void ResetThickness() {
			Thickness = DefaultThickness;
		}
		protected internal override bool ShouldSerialize() {
			return 
				base.ShouldSerialize() ||
				ShouldSerializeColor() ||
				ShouldSerializeThickness() ||
				ShouldSerializeVisible() ||
				ShouldSerializeVisibility();
		}
		#endregion
		void InitializeVisible(bool visible) {
			this.defaultVisible = visible;
		}
		void ResetColor() {
			Color = defaultColor;
		}
		bool GetActualVisibility(ISupportBorderVisibility appearanceHolder) {
			if (appearanceHolder != null)
				return DefaultBooleanUtils.ToBoolean(visibility, appearanceHolder.BorderVisible);
			else
				return DefaultBooleanUtils.ToBoolean(visibility, defaultVisible);
		}
		int GetActualThickness(bool visible) {
			return visible ? Thickness : 0;
		}
		internal int GetActualThicknessBasedOnAppearance(ISupportBorderVisibility appearanceHolder) {
			return GetActualThickness(GetActualVisibility(appearanceHolder));
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			BorderBase border = obj as BorderBase;
			return border != null && border.GetType().Equals(GetType()) && 
				color == border.color && thickness == border.thickness && 
				visibility == border.visibility; 
		}
		public override string ToString() {
			return "(Border)";
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			BorderBase border = obj as BorderBase;
			if(border == null)
				return;
			color = border.color;
			thickness = border.thickness;
			visibility = border.visibility;
		}
	}
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public sealed class CustomBorder : BorderBase {
		internal CustomBorder(ChartElement owner, bool visible, Color color) : base(owner, visible, color) {
		}
		protected override ChartElement CreateObjectForClone() {
			return new CustomBorder(null, true, Color);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			CustomBorder border = obj as CustomBorder;
			return border != null && base.Equals(border);
		}
	}
	public abstract class RectangularBorder : BorderBase {
		internal static void Render(IRenderer renderer, RectangularBorderLayout layout, Color color) {
			renderer.FillRectangle(layout.TopRect, color);
			renderer.FillRectangle(layout.BottomRect, color);
			renderer.FillRectangle(layout.LeftRect, color);
			renderer.FillRectangle(layout.RightRect, color);
		}
		protected internal enum RectangularBorderItem {
			TopItem,
			BottomItem,
			LeftItem,
			RightItem
		}
		protected RectangularBorder(ChartElement owner, bool visible, Color color) : base(owner, visible, color) {
		}
		protected internal abstract RectangleF ConstructBorderedRect(RectangularBorderLayout borderLayout);
		protected internal abstract RectangleF Enlarge(RectangleF rect, RectangularBorderItem item);
		protected internal abstract RectangleF CalcBorderItem(RectangleF rect, RectangularBorderItem item, int actualThickness);
		internal void Render(IRenderer renderer, RectangleF rect, int actualThickness, Color actualColor) {
			if (!ActualVisibility)
				return;
			RectangularBorderLayout borderLayout = new RectangularBorderLayout(this, rect, actualThickness);
			Render(renderer, borderLayout, actualColor);
		}
		internal RectangleF CalcBorderedRect(RectangleF rect) {
			RectangularBorderLayout borderLayout = new RectangularBorderLayout(this, rect, ActualThickness);
			return ConstructBorderedRect(borderLayout);
		}
		internal Size CalcBorderedSize(Size size) {
			RectangleF rect = new RectangleF(0, 0, size.Width, size.Height);
			return MathUtils.StrongRound(CalcBorderedRect(rect).Size);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			RectangularBorder border = obj as RectangularBorder;
			return border != null && base.Equals(obj);
		}
	}
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]	
	public sealed class OutsideRectangularBorder : RectangularBorder {
		internal OutsideRectangularBorder(ChartElement owner, bool visible, Color color) : base(owner, visible, color) {
		}
		protected internal override RectangleF ConstructBorderedRect(RectangularBorderLayout borderLayout) {
			RectangleF top = borderLayout.TopRect;
			RectangleF bottom = borderLayout.BottomRect;
			RectangleF left = borderLayout.LeftRect;
			RectangleF right = borderLayout.RightRect;
			return new RectangleF(left.Location.X, top.Location.Y, right.Location.X + right.Width - left.Location.X, bottom.Location.Y + bottom.Height - top.Location.Y);
		}
		protected internal override RectangleF Enlarge(RectangleF rect, RectangularBorderItem item) {
			switch (item) {
				case RectangularBorderItem.TopItem:
					rect.Offset(-1, -1);
					rect.Width += 2;
					rect.Height++;
					break;
				case RectangularBorderItem.BottomItem:
					rect.Offset(-1, 0);
					rect.Width += 2;
					rect.Height++;
					break;
				case RectangularBorderItem.LeftItem:
					rect.Offset(-1, 0);
					rect.Width++;
					break;
				case RectangularBorderItem.RightItem:
					rect.Width++;
					break;
			}
			return rect;
		}
		protected internal override RectangleF CalcBorderItem(RectangleF rect, RectangularBorderItem item, int actualThickness) {
			switch (item) {
				case RectangularBorderItem.TopItem:
					return new RectangleF(rect.Location.X - actualThickness, rect.Location.Y - actualThickness, rect.Width + actualThickness * 2, actualThickness);
				case RectangularBorderItem.BottomItem:
					return new RectangleF(rect.Location.X - actualThickness, rect.Location.Y + rect.Height, rect.Width + actualThickness * 2, actualThickness);
				case RectangularBorderItem.LeftItem:
					return new RectangleF(rect.Location.X - actualThickness, rect.Location.Y , actualThickness, rect.Height);
				case RectangularBorderItem.RightItem:
					return new RectangleF(rect.Location.X + rect.Width, rect.Location.Y, actualThickness, rect.Height);
			}
			return RectangleF.Empty;
		}
		protected override ChartElement CreateObjectForClone() {
			return new OutsideRectangularBorder(null, true, Color);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			OutsideRectangularBorder border = obj as OutsideRectangularBorder;
			return border != null && base.Equals(obj);
		}
	}
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]	
	public sealed class InsideRectangularBorder : RectangularBorder {
		internal InsideRectangularBorder(ChartElement owner, bool visible, Color color) : base(owner, visible, color) {
		}
		protected internal override RectangleF ConstructBorderedRect(RectangularBorderLayout borderLayout) {
			RectangleF top = borderLayout.TopRect;
			RectangleF bottom = borderLayout.BottomRect;
			RectangleF left = borderLayout.LeftRect;
			RectangleF right = borderLayout.RightRect;
			return new RectangleF(left.Location.X + left.Width, top.Location.Y + top.Height, right.Location.X - left.Location.X - left.Width, bottom.Location.Y - top.Location.Y - top.Height);
		}
		protected internal override RectangleF Enlarge(RectangleF rect, RectangularBorderItem item) {
			switch (item) {
				case RectangularBorderItem.TopItem:
					rect.Height++;
					break;
				case RectangularBorderItem.BottomItem:
					rect.Offset(0, -1);
					rect.Height++;
					break;
				case RectangularBorderItem.LeftItem:
					rect.Width++;
					break;
				case RectangularBorderItem.RightItem:
					rect.Offset(-1, 0);
					rect.Width++;
					break;
			}
			return rect;
		}
		protected internal override RectangleF CalcBorderItem(RectangleF rect, RectangularBorderItem item, int actualThickness) {
			switch (item) {
				case RectangularBorderItem.TopItem:
					return new RectangleF(rect.Location.X, rect.Location.Y, rect.Width, actualThickness);
				case RectangularBorderItem.BottomItem:
					return new RectangleF(rect.Location.X, rect.Location.Y + rect.Height - actualThickness, rect.Width, actualThickness);
				case RectangularBorderItem.LeftItem:
					return new RectangleF(rect.Location.X, rect.Location.Y + actualThickness, actualThickness, rect.Height - 2 * actualThickness);
				case RectangularBorderItem.RightItem:
					return new RectangleF(rect.Location.X + rect.Width - actualThickness, rect.Location.Y + actualThickness, actualThickness, rect.Height - 2 * actualThickness);
			}
			return RectangleF.Empty;
		}
		protected override ChartElement CreateObjectForClone() {
			return new InsideRectangularBorder(null, true, Color);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			InsideRectangularBorder border = obj as InsideRectangularBorder;
			return border != null && base.Equals(obj);
		}
	}
}
namespace DevExpress.XtraCharts.Native {
	public class RectangularBorderLayout {
		readonly RectangularBorder border;
		RectangleF topRect;
		RectangleF bottomRect;
		RectangleF leftRect;
		RectangleF rightRect;
		public RectangleF TopRect { get { return topRect; } }
		public RectangleF BottomRect { get { return bottomRect; } }
		public RectangleF LeftRect { get { return leftRect; } }
		public RectangleF RightRect { get { return rightRect; } }
		public RectangularBorderLayout(RectangularBorder border, RectangleF rect, int actualThickness) {
			this.border = border;
			CalculateLayout(rect, actualThickness);
		}
		void CalculateLayout(RectangleF rect, int actualThickness) {
			topRect = border.CalcBorderItem(rect, RectangularBorder.RectangularBorderItem.TopItem, actualThickness);
			bottomRect = border.CalcBorderItem(rect, RectangularBorder.RectangularBorderItem.BottomItem, actualThickness);
			leftRect = border.CalcBorderItem(rect, RectangularBorder.RectangularBorderItem.LeftItem, actualThickness);
			rightRect = border.CalcBorderItem(rect, RectangularBorder.RectangularBorderItem.RightItem, actualThickness);
		}
		public void Enlarge() {
			topRect = border.Enlarge(topRect, RectangularBorder.RectangularBorderItem.TopItem);
			bottomRect = border.Enlarge(bottomRect, RectangularBorder.RectangularBorderItem.BottomItem);
			leftRect = border.Enlarge(leftRect, RectangularBorder.RectangularBorderItem.LeftItem);
			rightRect = border.Enlarge(rightRect, RectangularBorder.RectangularBorderItem.RightItem);
		}
	}
	public sealed class BorderHelper {
		public static void RenderBorder(IRenderer renderer, RectangularBorder border, RectangleF rect, HitTestState hitTestState, int actualThickness, Color actualColor) {
			if (border.ActualVisibility) {
				RectangularBorderLayout borderLayout = new RectangularBorderLayout(border, rect, actualThickness);
				if (!hitTestState.Normal)
					borderLayout.Enlarge();
				DevExpress.XtraCharts.RectangularBorder.Render(renderer, borderLayout, actualColor); 
			}
			else {
				if (hitTestState.Normal)
					return;
				renderer.DrawRectangle(rect, actualColor, 1);
			}
		}
		public static Color CalculateBorderColor(Color borderColor, Color automaticBorderColor) {
			return borderColor.IsEmpty ? automaticBorderColor : borderColor;
		}
		public static Color CalculateBorderColor(BorderBase border, Color automaticBorderColor) {
			return CalculateBorderColor(border.Color, automaticBorderColor);
		}
		public static Color CalculateBorderColor(BorderBase border, Color automaticBorderColor, HitTestState hitState) {
			if(hitState.Normal)
				return CalculateBorderColor(border, automaticBorderColor);
			else
				return hitState.ActualColor;
		}
		public static int CalculateBorderThickness(BorderBase border, int thickness) {
			return border.ActualVisibility ? thickness : 0;
		}
		BorderHelper() {
		}
	}
}
