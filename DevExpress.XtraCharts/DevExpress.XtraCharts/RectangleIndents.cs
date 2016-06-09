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
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Localization;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts {
	[Obsolete("This class is now obsolete. Use the RectangleIndents class instead.")]
	public class Padding : RectangleIndents {
		internal Padding(ChartElement owner, int defaultPadding) : base(owner, defaultPadding) {
		}
	}
	[Obsolete("This class is now obsolete. Use the RectangleIndents class instead.")]
	public class Margins : RectangleIndents {
		internal Margins(ChartElement owner, int defaultMargins) : base(owner, defaultMargins) {
		}
	}
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class RectangleIndents : ChartElement {
		public const int Undefined = -1;
		readonly int defaultIndent;
		int left;
		int top;
		int right;
		int bottom;
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RectangleIndentsLeft"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.RectangleIndents.Left"),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public int Left {
			get { return left; }
			set {
				if (value < 0)
					value = defaultIndent;
				if (value != left) {
					SendNotification(new ElementWillChangeNotification(this));
					left = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RectangleIndentsTop"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.RectangleIndents.Top"),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public int Top {
			get { return top; }
			set {
				if (value < 0)
					value = defaultIndent;
				if (value != top) {
					SendNotification(new ElementWillChangeNotification(this));
					top = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RectangleIndentsRight"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.RectangleIndents.Right"),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public int Right {
			get { return right; }
			set {
				if (value < 0)
					value = defaultIndent;
				if (value != right) {
					SendNotification(new ElementWillChangeNotification(this));
					right = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RectangleIndentsBottom"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.RectangleIndents.Bottom"),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public int Bottom {
			get { return bottom; }
			set {
				if (value < 0)
					value = defaultIndent;
				if (value != bottom) {
					SendNotification(new ElementWillChangeNotification(this));
					bottom = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RectangleIndentsAll"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.RectangleIndents.All"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		RefreshProperties(RefreshProperties.All),
		]
		public int All {
			get { return left == top && left == right && left == bottom ? left : Undefined; }
			set {
				if (value < 0) {
					SendNotification(new ElementWillChangeNotification(this));
					SetLeftTopRightBottom(defaultIndent);
					RaiseControlChanged();
				}
				else if (value != All) {
					SendNotification(new ElementWillChangeNotification(this));
					SetLeftTopRightBottom(value);
					RaiseControlChanged();
				}
			}
		}
		internal RectangleIndents(ChartElement owner, int defaultIndent) : base(owner) {
			this.defaultIndent = defaultIndent;
			SetLeftTopRightBottom(defaultIndent);
		}
		internal RectangleIndents(ChartElement owner) : this(owner, Undefined) {
		}
		void SetLeftTopRightBottom(int value) {
			SetIndents(value, value, value, value);			
		}
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			if (propertyName == "Left")
				return ShouldSerializeLeft();
			if (propertyName == "Top")
				return ShouldSerializeTop();
			if (propertyName == "Right")
				return ShouldSerializeRight();
			if (propertyName == "Bottom")
				return ShouldSerializeBottom();
			return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeLeft() {
			return left != defaultIndent;
		}
		void ResetLeft() {
			Left = defaultIndent;
		}
		bool ShouldSerializeTop() {
			return top != defaultIndent;
		}
		void ResetTop() {
		   Top = defaultIndent;
		}
		bool ShouldSerializeRight() {
			return right != defaultIndent;
		}
		void ResetRight() {
			Right = defaultIndent;
		}
		bool ShouldSerializeBottom() {
			return bottom != defaultIndent;
		}
		void ResetBottom() {
			Bottom = defaultIndent;
		}
		bool ShouldSerializeAll() { 
			return ShouldSerializeLeft() || ShouldSerializeTop() || ShouldSerializeRight() || ShouldSerializeBottom();
		}
		void ResetAll() {
			All = defaultIndent;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializeLeft() || ShouldSerializeTop() || ShouldSerializeRight() || ShouldSerializeBottom();
		}
		#endregion
		internal Rectangle DecreaseRectangle(Rectangle rect) {
			int width = Math.Max(rect.Width - left - right, 0);
			int height = Math.Max(rect.Height - top - bottom, 0);
			return new Rectangle(rect.X + left, rect.Y + top, width, height);
		}
		internal RectangleF DecreaseRectangle(RectangleF rect) {
			float width = Math.Max(rect.Width - left - right, 0);
			float height = Math.Max(rect.Height - top - bottom, 0);
			return new RectangleF(rect.X + left, rect.Y + top, width, height);
		}
		internal ZPlaneRectangle DecreaseRectangle(ZPlaneRectangle rect) {
			DiagramPoint location = new DiagramPoint(rect.Location.X + left, rect.Location.Y + top);
			double width = Math.Max(0, rect.Width - right - left);
			double height = Math.Max(0, rect.Height - bottom - top);
			return new ZPlaneRectangle(location, width, height);
		}
		internal Size IncreaseSize(Size size) {
			return new Size(size.Width + left + right, size.Height + top + bottom);
		}
		internal SizeF IncreaseSizeF(SizeF size) {
			return new SizeF(size.Width + left + right, size.Height + top + bottom);
		}
		internal GRealSize2D IncreaseSizeF(GRealSize2D size) {
			return new GRealSize2D(size.Width + left + right, size.Height + top + bottom);
		}
		internal Rectangle IncreaseRectangle(Rectangle rectangle) {
			return new Rectangle(rectangle.X - left, rectangle.Y - top, rectangle.Width + left + right, rectangle.Height + top + bottom);
		}
		internal RectangleF IncreaseRectangle(RectangleF rectangle) {
			return new RectangleF(rectangle.X - left, rectangle.Y - top, rectangle.Width + left + right, rectangle.Height + top + bottom);
		}
		internal GRealRect2D IncreaseRectangle(GRealRect2D rectangle) {
			return new GRealRect2D(rectangle.Left - left, rectangle.Top - top, rectangle.Width + left + right, rectangle.Height + top + bottom);
		}		
		internal void SetIndents(int left, int top, int right, int bottom) {
			this.left = left;
			this.top = top;
			this.right = right;
			this.bottom = bottom;   
		}
		protected override ChartElement CreateObjectForClone() {
			return new RectangleIndents(null, defaultIndent);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			RectangleIndents rectangleIndents = obj as RectangleIndents;
			if (rectangleIndents != null) {
				left = rectangleIndents.left;
				top = rectangleIndents.top;
				right = rectangleIndents.right;
				bottom = rectangleIndents.bottom;
			}
		}
	}
}
