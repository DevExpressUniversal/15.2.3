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
#if DXWhidbey
using System.Collections.Generic;
#endif
using System.Text;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Design;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Controls;
using DevExpress.XtraEditors.Controls;
using DevExpress.Accessibility;
using System.Windows.Forms.Design;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Design;
using System.Drawing.Drawing2D;
using DevExpress.Utils.WXPaint;
using DevExpress.XtraPrinting;
using System.Windows.Forms.Layout;
using System.Drawing;
using DevExpress.XtraEditors.Repository;
using DevExpress.Utils.Text;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;
using DevExpress.XtraEditors.ToolboxIcons;
namespace DevExpress.XtraEditors.Repository {
	public class TrackBarRangeConverter : TypeConverter {
		public TrackBarRangeConverter() : base() { }
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if(sourceType == typeof(string)) return true;
			return base.CanConvertFrom(context, sourceType);
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if(destinationType == typeof(InstanceDescriptor)) return true;
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			string str1 = value as string;
			if(str1 != null) {
				string str2 = str1.Trim();
				if(str2.Length != 0) {
					if(culture == null) culture = CultureInfo.CurrentCulture;
					char sep = culture.TextInfo.ListSeparator[0];
					string[] strValues = str2.Split(new char[] { sep });
					int[] values = new int[strValues.Length];
					TypeConverter intConv = TypeDescriptor.GetConverter(typeof(int));
					for(int i=0; i<values.Length; i++) { 
						values[i] = (int)intConv.ConvertFromString(strValues[i]);
					}
					if(strValues.Length != 2) { 
						throw new ArgumentException("TextParseFailedFormat min, max");
					}
					return new TrackBarRange(values[0], values[1]);
				}
				return null;
			}
			return base.ConvertFrom(context, culture, value);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType == null) {
				throw new ArgumentNullException("destinationType");
			}
			if(value is TrackBarRange) {
				if(destinationType == typeof(string)) {
					TrackBarRange range = (TrackBarRange)value;
					if(culture == null) culture = CultureInfo.CurrentCulture;
					string sep = culture.TextInfo.ListSeparator + " ";
					TypeConverter intConv = TypeDescriptor.GetConverter(typeof(int));
					string[] strValues = new string[2];
					strValues[0] = intConv.ConvertToString(range.Minimum);
					strValues[1] = intConv.ConvertToString(range.Maximum);
					return string.Join(sep, strValues);
				}
				if(destinationType == typeof(InstanceDescriptor)) {
					TrackBarRange range = (TrackBarRange)value;
					ConstructorInfo constructorInfo = typeof(TrackBarRange).GetConstructor(new Type[] { typeof(int), typeof(int) });
					if(constructorInfo != null) return new InstanceDescriptor(constructorInfo, new object[] { range.Minimum, range.Maximum });
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override object CreateInstance(ITypeDescriptorContext context, System.Collections.IDictionary propertyValues) {
			if(propertyValues == null) {
				throw new ArgumentNullException("propertyValues");
			}
			object propMin = propertyValues["Minimum"];
			object propMax = propertyValues["Maximum"];
			if(((propMin == null) || (propMax == null)) || (!(propMin is int) || !(propMax is int))) {
				throw new ArgumentException("PropertyValueInvalidEntry");
			}
			return new TrackBarRange((int)propMin, (int)propMax);
		}
		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context) { return true; }
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			return TypeDescriptor.GetProperties(typeof(TrackBarRange), attributes).Sort(new string[] { "Minimum", "Maximum" });
		}
		public override bool GetPropertiesSupported(ITypeDescriptorContext context) { return true; }
	}
	[Serializable, StructLayout(LayoutKind.Sequential), TypeConverter(typeof(TrackBarRangeConverter))]
	public struct TrackBarRange { 
		int min, max;
		public static readonly TrackBarRange Empty;
		static TrackBarRange() {
			Empty = new TrackBarRange(0, 0);
		}
		public TrackBarRange(int minimum, int maximum) { 
			this.min = minimum;
			this.max = maximum;
			if(this.min > this.max) {
				int tmp = this.max; this.max = min; this.min = tmp;
			}
		}
		public int Minimum { 
			get { return min; } 
			set {
				min = value;
				if(max < min) max = min;
			} 
		}
		public int Maximum { 
			get { return max; } 
			set {
				max = value;
				if(min > max) min = max;
			} 
		}
		[Browsable(false)]
		public bool IsEmpty { get { return min == 0 && max == 0; } }
		public override bool Equals(object obj) {
			if(obj is TrackBarRange) {
				TrackBarRange range = (TrackBarRange)obj;
				return range.min == min && range.max == max;
			}
			return false;
		}
		public override int GetHashCode() { return (min ^ max); }
		public static bool operator ==(TrackBarRange left, TrackBarRange right) {
			return left.Minimum == right.Minimum && left.Maximum == right.Maximum;
		}
		public static bool operator !=(TrackBarRange left, TrackBarRange right) { return !(left == right); }
		public override string ToString() {
			return ("{Min=" + Minimum.ToString(CultureInfo.CurrentCulture) + ",Max=" + Maximum.ToString(CultureInfo.CurrentCulture) + "}");
		}
	}
	public class RepositoryItemRangeTrackBar : RepositoryItemTrackBar {
		protected new RangeTrackBarControl OwnerEdit { get { return base.OwnerEdit as RangeTrackBarControl; } }
		[Browsable(false)]
		public override string EditorTypeName { get { return "RangeTrackBarControl"; } }
		protected internal virtual TrackBarRange ConvertRangeValue(object val) {
			if(val is TrackBarRange) return CheckRangeValue((TrackBarRange)val);
			return new TrackBarRange(Minimum, Minimum);
		}
		protected internal override int CheckValue(int val) { return val; }
		protected virtual TrackBarRange CheckValue(TrackBarRange range) {
			return new TrackBarRange(range.Minimum < Minimum? Minimum: range.Minimum, range.Maximum > Maximum? Maximum: range.Maximum);
		}
		protected internal new RangeTrackBarViewInfo CreateViewInfo() { return new RangeTrackBarViewInfo(this); }
		protected internal new RangeTrackBarPainter CreatePainter() { return new RangeTrackBarPainter(); }
		protected internal TrackBarRange CheckRangeValue(TrackBarRange val) {
			TrackBarRange range = TrackBarRange.Empty;
			range = val;
			range.Minimum = Math.Min(Math.Max(Minimum, range.Minimum), Maximum);
			range.Maximum = Math.Max(Math.Min(Maximum, range.Maximum), Minimum);
			return range;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new int SmallChange {
			get { return base.SmallChange; }
			set { base.SmallChange = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new int LargeChange {
			get { return base.LargeChange; }
			set { base.LargeChange = value; }
		}
		protected internal override int ConvertValue(object val) {
			return ((TrackBarRange)val).Maximum - ((TrackBarRange)val).Minimum;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool AllowMouseWheel {
			get { return base.AllowMouseWheel; }
			set { base.AllowMouseWheel = value; }
		}
	}
}
namespace DevExpress.XtraEditors.ViewInfo {
	public class RangeTrackBarInfoCalculator : TrackBarInfoCalculator {
		public RangeTrackBarInfoCalculator(RangeTrackBarViewInfo viewInfo, RangeTrackBarObjectPainter painter) : base(viewInfo, painter) { }
		public new RangeTrackBarViewInfo ViewInfo { get { return base.ViewInfo as RangeTrackBarViewInfo; } }
		public new RangeTrackBarObjectPainter TrackPainter { get { return base.TrackPainter as RangeTrackBarObjectPainter; } }
		protected override int[] CreateMinHeights() { return new int[] { 0, 2, 1, 1 }; }
		protected override int GetTrackLineRectInflateWidth() { return ViewInfo.TrackBarHelper.GetRectWidth(ViewInfo.MinThumbContentBounds) + ViewInfo.TrackBarHelper.GetRectWidth(ViewInfo.MaxThumbContentBounds) - 1; }
		protected override int PointsRectOffsetX { get { return ViewInfo.TrackBarHelper.GetRectWidth(ViewInfo.MinThumbContentBounds) - 1; } }
		public virtual Rectangle GetMinThumbBounds() { return ViewInfo.GetMinThumbBounds(); }
		public virtual Rectangle GetMaxThumbBounds() { return ViewInfo.GetMaxThumbBounds(); }
		public virtual Rectangle GetMaxThumbClientBounds(Rectangle rect) {
			return GetClientBounds(rect, true);
		}
		protected Rectangle GetClientBounds(Rectangle rect, bool isMaxThumb) {
			Size addedSize = CalcTouchAddedSize(rect.Size);
			if(ViewInfo.Orientation == Orientation.Horizontal) {
				if(ViewInfo.IsRightToLeft ^ !isMaxThumb) rect.X -= addedSize.Width * 3 / 4;
				rect.Y -= addedSize.Height / 2;
				rect.Height += addedSize.Height;
				rect.Width += addedSize.Width * 3 / 4;
			}
			else {
				rect.X -= addedSize.Width / 2;
				if(!ViewInfo.IsRightToLeft ^ !isMaxThumb) rect.Y -= addedSize.Height * 3 / 4;
				rect.Height += addedSize.Height * 3 / 4;
				rect.Width += addedSize.Width;
			}
			return rect;
		}
		public virtual Rectangle GetMinThumbClientBounds(Rectangle rect) {
			return GetClientBounds(rect, false);
		}
		protected override int CalcTouchTrackLineDeflate() {
			return base.CalcTouchTrackLineDeflate() * 2;
		}
	}
	public class SkinRangeTrackBarInfoCalculator : RangeTrackBarInfoCalculator { 
		public SkinRangeTrackBarInfoCalculator(RangeTrackBarViewInfo viewInfo, RangeTrackBarObjectPainter painter) : base(viewInfo, painter) { }
		public override int ThumbHeight { get { return GetThumbElementOriginSize().Height; } }
		public override int RealTrackLineHeight { get { return GetLineSize().Height; } }
		protected override int[] CreateMinHeights() { return new int[] { 0, 1, 1, 1 }; }
		public override SkinElementInfo GetThumbElementInfo() { 
			if(ViewInfo.TickStyle == TickStyle.Both)
				return new SkinElementInfo(EditorsSkins.GetSkin(ViewInfo.LookAndFeel.ActiveLookAndFeel)[EditorsSkins.SkinRangeTrackBarThumbBoth], Rectangle.Empty);
			return new SkinElementInfo(EditorsSkins.GetSkin(ViewInfo.LookAndFeel.ActiveLookAndFeel)[EditorsSkins.SkinRangeTrackBarLeftThumb], Rectangle.Empty);
		}
		protected virtual Size GetThumbElementSize() { return ScaleSkinThumbSize(GetThumbElementOriginSize()); }
		protected override Size ScaleSkinThumbSize(Size sz) { return new Size(sz.Width, RealThumbHeight); }
		protected virtual Size GetThumbElementOriginSize() { 
			SkinElementInfo thumbInfo = GetThumbElementInfo();
			if(thumbInfo == null || thumbInfo.Element.Image == null) return base.GetThumbBounds().Size;
			return new Size(Math.Min(thumbInfo.Element.Image.GetImageBounds(0).Size.Width, thumbInfo.Element.Size.MinSize.Width), thumbInfo.Element.Image.GetImageBounds(0).Size.Height);
		}
		protected internal override SkinElementInfo GetLineInfo() { return new SkinElementInfo(EditorsSkins.GetSkin(ViewInfo.LookAndFeel.ActiveLookAndFeel)[EditorsSkins.SkinTrackBarTrack]); }
		protected override int ThumbUpperPartHeight { get { return GetThumbElementOffset().Y; } }
		protected override int ThumbLowerPartHeight {
			get {
				Size sz = GetThumbElementSize();
				if(sz.IsEmpty) return base.ThumbLowerPartHeight;
				return sz.Height - GetThumbElementOffset().Y;
			}
		}
		protected virtual Point GetThumbElementOffset() { return ScaleThumbElementOffset(GetThumbElementOriginOffset()); }
		protected virtual Point GetThumbElementOriginOffset() {
			SkinElementInfo thumbInfo = GetThumbElementInfo();
			if(thumbInfo == null) {
				Size sz = base.GetThumbBounds().Size;
				return new Point(sz.Width / 2, sz.Height / 2);
			}
			return new Point(0, thumbInfo.Element.Offset.Offset.Y);
		}
		protected virtual Point ScaleThumbElementOffset(Point pt) {
			Size sz = GetThumbElementSize(), originSize = GetThumbElementOriginSize();
			if(ThumbHeight == RealThumbHeight) return pt;
			pt.Y = (int)(pt.Y * (float)RealThumbHeight / (float)ThumbHeight);
			return pt;
		}
		public override Rectangle GetMinThumbBounds() {
			int x = ViewInfo.IsRightToLeft ? ViewInfo.MinThumbPos.X : ViewInfo.MinThumbPos.X - GetThumbElementSize().Width;
			Rectangle rect = new Rectangle(new Point(x, GetThumbY()), GetThumbElementSize());
			return ViewInfo.TrackBarHelper.Rotate(rect);
		}
		public override Rectangle GetMaxThumbBounds() {
			int x = ViewInfo.IsRightToLeft ? ViewInfo.MaxThumbPos.X - GetThumbElementSize().Width : ViewInfo.MaxThumbPos.X;
			Rectangle rect = new Rectangle(new Point(x, GetThumbY()), GetThumbElementSize());
			return ViewInfo.TrackBarHelper.Rotate(rect);
		}
	}
	public class RangeTrackBarViewInfo : TrackBarViewInfo {
		Point minThumbPos, maxThumbPos;
		TrackBarRange range;
		public RangeTrackBarViewInfo(RepositoryItem item) : base(item) {
			this.minThumbPos = this.maxThumbPos = Point.Empty;
		}
		public override void Offset(int x, int y) {
			base.Offset(x, y);
			minThumbPos.Offset(x, y);
			maxThumbPos.Offset(x, y);
		}
		[EditorPainterActivator(typeof(SkinRangeTrackBarObjectPainter), typeof(TrackBarObjectPainter))]
		public override TrackBarObjectPainter GetTrackPainter() {
			if(IsPrinting)
				return new RangeTrackBarObjectPainter();
			if(this.LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.WindowsXP)
				return new RangeTrackBarObjectPainter();
			if(this.LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin)
				return new SkinRangeTrackBarObjectPainter(LookAndFeel.ActiveLookAndFeel);
			if(this.LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Office2003)
				return new Office2003RangeTrackBarObjectPainter();
			return new RangeTrackBarObjectPainter();
		}
		public Point MinThumbPos { get { return minThumbPos; } }
		public Point MaxThumbPos { get { return maxThumbPos; } }
		Rectangle MinArrowThumbRect {
			get { return new Rectangle(-TrackPainter.GetThumbBestWidth(this), -TrackPainter.GetThumbBestHeight(this) / 2, TrackPainter.GetThumbBestWidth(this), TrackPainter.GetThumbBestHeight(this)); }
		}
		Rectangle MaxArrowThumbRect {
			get { return new Rectangle(0, -TrackPainter.GetThumbBestHeight(this) / 2, TrackPainter.GetThumbBestWidth(this), TrackPainter.GetThumbBestHeight(this)); }
		}
		protected override bool UpdateObjectState()
		{
			ObjectState prevState = State;
			EditHitTest prevTest = HotInfo.HitTest;
			bool res = base.UpdateObjectState();
			if(Enabled) {
				if(PressedInfo.HitTest == EditHitTest.Button || PressedInfo.HitTest == EditHitTest.Button2) {
					State |= ObjectState.Pressed;
				}
			}
			return res || prevState != State || prevTest != HotInfo.HitTest;
		}
		protected internal int[,] MinArrowThumbOffset {
			get {
				if(IsRightToLeft) return RightArrowThumbOffset();
				return LeftArrowThumbOffset();
			}
		}
		protected internal int[,] MaxArrowThumbOffset {
			get {
				if(IsRightToLeft) return LeftArrowThumbOffset();
				return RightArrowThumbOffset();
			}
		}
		protected int[,] LeftArrowThumbOffset() {
			Rectangle tr = MinArrowThumbRect;
			int[,] offsetP1 = { { tr.Left, tr.Top }, { tr.Right, tr.Top }, { tr.Right, tr.Bottom }, { tr.Left, tr.Bottom - tr.Height / 3 }, { tr.Left, tr.Top } };
			return offsetP1;
		}
		protected int[,] RightArrowThumbOffset() {
			Rectangle tr = MaxArrowThumbRect;
			int[,] offsetP1 = { { tr.Left, tr.Top }, { tr.Right, tr.Top }, { tr.Right, tr.Bottom - tr.Height / 3 }, { tr.Left, tr.Bottom }, { tr.Left, tr.Top } };
			return offsetP1;
		}
		protected internal int[,] MinRectThumbOffset {
			get {
				if(IsRightToLeft) return RightRectThumbOffset();
				return LeftRectThumbOffset();
			}
		}
		protected internal int[,] MaxRectThumbOffset {
			get {
				if(IsRightToLeft) return LeftRectThumbOffset();
				return RightRectThumbOffset();
			}
		}
		protected int[,] LeftRectThumbOffset() {
			Rectangle tr = new Rectangle(-TrackPainter.GetThumbBestWidth(this), -TrackPainter.GetThumbBestHeight(this) / 2, TrackPainter.GetThumbBestWidth(this), TrackPainter.GetThumbBestHeight(this));
			int[,] offsetP1 = { { tr.Left, tr.Top }, { tr.Right, tr.Top }, { tr.Right, tr.Bottom }, { tr.Left, tr.Bottom }, { tr.Left, tr.Top } };
			return offsetP1;
		}
		protected int[,] RightRectThumbOffset() {
			Rectangle tr = new Rectangle(0, -TrackPainter.GetThumbBestHeight(this) / 2, TrackPainter.GetThumbBestWidth(this), TrackPainter.GetThumbBestHeight(this));
			int[,] offsetP1 = { { tr.Left, tr.Top }, { tr.Right, tr.Top }, { tr.Right, tr.Bottom }, { tr.Left, tr.Bottom }, { tr.Left, tr.Top } };
			return offsetP1;
		}
		public virtual Point[] MinArrowThumbRegion {
			get {
				int[,] offsetP1 = MinArrowThumbOffset;
				Point[] polygon = new Point[5];
				TransformPoints(offsetP1, polygon, MinThumbPos);
				return polygon;
			}
		}
		public virtual Point[] MaxArrowThumbRegion {
			get {
				int[,] offsetP1 = MaxArrowThumbOffset;
				Point[] polygon = new Point[5];
				TransformPoints(offsetP1, polygon, MaxThumbPos);
				return polygon;
			}
		}
		public virtual Point[] MinRectThumbRegion {
			get {
				Point[] polygon = new Point[5];
				TransformPoints(MinRectThumbOffset, polygon, MinThumbPos);
				return polygon;   
			}
		}
		public virtual Point[] MaxRectThumbRegion {
			get {
				Point[] polygon = new Point[5];
				TransformPoints(MaxRectThumbOffset, polygon, MaxThumbPos);
				return polygon;   
			}
		}
		public virtual Point[] MinThumbRegion {
			get {
				if(TickStyle == TickStyle.Both) return MinRectThumbRegion;
				return MinArrowThumbRegion;
			}
		}
		public virtual Point[] MaxThumbRegion {
			get {
				if(TickStyle == TickStyle.Both) return MaxRectThumbRegion;
				return MaxArrowThumbRegion;
			}
		}
		protected new RangeTrackBarInfoCalculator TrackCalculator { get { return base.TrackCalculator as RangeTrackBarInfoCalculator; } }
		protected internal virtual Rectangle GetMinThumbBounds() { return GetThumbBounds(MinRectThumbRegion); }
		protected internal virtual Rectangle GetMaxThumbBounds() { return GetThumbBounds(MaxRectThumbRegion); }
		public virtual Rectangle MinThumbContentBounds { get { return TrackCalculator.GetMinThumbBounds(); } }
		public virtual Rectangle MaxThumbContentBounds { get { return TrackCalculator.GetMaxThumbBounds(); } }
		public virtual Rectangle MinThumbBounds {
			get {
				if(IsTouchMode)
					return TrackCalculator.GetMinThumbClientBounds(MinThumbContentBounds);
				return MinThumbContentBounds;
			}
		}
		public virtual Rectangle MaxThumbBounds { 
			get {
				if(IsTouchMode) 
					return TrackCalculator.GetMaxThumbClientBounds(MaxThumbContentBounds);
				return MaxThumbContentBounds;
			} 
		}
		protected override void CalcThumbPos() {
			this.minThumbPos = CalcThumbPosCore(Value.Minimum);
			this.maxThumbPos = CalcThumbPosCore(Value.Maximum);
		}
		public new RepositoryItemRangeTrackBar Item { get { return base.Item as RepositoryItemRangeTrackBar; } }		
		protected override void OnEditValueChanged() {
			this.range = Item.ConvertRangeValue(EditValue);
		}
		public new TrackBarRange Value { get { return range; } }
		public override EditHitInfo CalcHitInfo(Point p) {
			EditHitInfo hi = base.CalcHitInfo(p);
			if(!ShouldUpdateState) {
				hi.SetHitObject(HotInfo.HitObject);
				hi.SetHitTest(HotInfo.HitTest);
				return hi;
			}
			if(Bounds.Contains(p)) {
				if (MinThumbBounds.Contains(p))
				{
					hi.SetHitTest(EditHitTest.Button);
					hi.SetHitObject(EditHitTest.Button);
				}
				else if (MaxThumbBounds.Contains(p))
				{
					hi.SetHitTest(EditHitTest.Button2);
					hi.SetHitObject(EditHitTest.Button2);
				}
			}
			return hi;
		}
		protected override bool IsHotEdit(EditHitInfo hitInfo) {
			return hitInfo.HitTest == EditHitTest.Button || hitInfo.HitTest == EditHitTest.Button2;
		}
		public override Point[] RectThumbRegion { get { return MinRectThumbRegion; } }
	}
}
namespace DevExpress.XtraEditors.Drawing {
	public class SkinRangeTrackBarObjectPainter : RangeTrackBarObjectPainter {
		ISkinProvider provider;
		public SkinRangeTrackBarObjectPainter(ISkinProvider provider) : base() {
			this.provider = provider;
		}
		protected override ISkinProvider Provider { get { return provider; } }
		protected internal override TrackBarInfoCalculator GetCalculator(TrackBarViewInfo viewInfo) {
			return new SkinRangeTrackBarInfoCalculator(viewInfo as RangeTrackBarViewInfo, this);
		}
		protected virtual ObjectState GetThumbState(RangeTrackBarViewInfo viewInfo, ObjectState state, EditHitTest hitTest) {
			if(viewInfo.HotInfo.HitTest != hitTest) {
				if(state == ObjectState.Disabled) return ObjectState.Disabled;
				if(viewInfo.Focused) return ObjectState.Selected;
				return ObjectState.Normal;
			}
			return state;
		}
		public override void FillThumb(TrackBarObjectInfoArgs e, bool bMirror) { }
		public override SkinElementInfo GetMinThumbInfo(RangeTrackBarViewInfo viewInfo, ObjectState state) {
			SkinElementInfo info = null;
			if (viewInfo.TickStyle == TickStyle.Both) return GetBothThumbInfo(viewInfo, EditHitTest.Button, state);
			info = new SkinElementInfo(EditorsSkins.GetSkin(Provider)[EditorsSkins.SkinRangeTrackBarLeftThumb]);
			if (info != null)
			{
				info.State = GetThumbState(viewInfo, state, EditHitTest.Button);
				info.ImageIndex = -1;
			}
			if(viewInfo.IsRightToLeft) info.RightToLeft = true;
			return info;
		}
		public override SkinElementInfo GetMaxThumbInfo(RangeTrackBarViewInfo viewInfo, ObjectState state) {
			SkinElementInfo info = null;
			if (viewInfo.TickStyle == TickStyle.Both) return GetBothThumbInfo(viewInfo, EditHitTest.Button2, state);
			info = new SkinElementInfo(EditorsSkins.GetSkin(Provider)[EditorsSkins.SkinRangeTrackBarRightThumb]);
			if(info != null)
			{
				info.State = GetThumbState(viewInfo, state, EditHitTest.Button2);
				info.ImageIndex = -1;
			}
			if(viewInfo.IsRightToLeft) info.RightToLeft = true;
			return info;
		}
		public override SkinElementInfo GetBothThumbInfo(RangeTrackBarViewInfo viewInfo, EditHitTest hitButton, ObjectState state) {
			SkinElementInfo info = new SkinElementInfo(EditorsSkins.GetSkin(Provider)[EditorsSkins.SkinRangeTrackBarThumbBoth]);
			if (info != null && viewInfo.HotInfo.HitTest == hitButton)
			{
				info.State = state;
				info.ImageIndex = -1;
			}
			return info;
		}
		public override SkinElementInfo GetLineInfo(RangeTrackBarViewInfo viewInfo) {
			return new SkinElementInfo(EditorsSkins.GetSkin(Provider)[EditorsSkins.SkinTrackBarTrack]);
		}
		protected override void DrawLine(TrackBarObjectInfoArgs e, Point p1, Point p2) {
			e.Paint.DrawLine(e.Graphics, e.Cache.GetPen(EditorsSkins.GetSkin(Provider)[EditorsSkins.SkinTrackBarTickLine].Color.GetForeColor()), p1, p2);
		}
		protected override Brush GetLabelsBrush(TrackBarObjectInfoArgs e) {
			RepositoryItemTrackBar ri = e.ViewInfo.Item;
			if(!ri.LabelAppearance.ForeColor.IsEmpty)
				return new SolidBrush(ri.LabelAppearance.ForeColor);
			return new SolidBrush(EditorsSkins.GetSkin(provider)[EditorsSkins.SkinTrackBarTickLine].Color.GetForeColor());
		}
		protected virtual Rectangle GetMinVerticalThumbRectangle(RangeTrackBarViewInfo ri, Rectangle minThumb) {
			Rectangle rect = GetVerticalThumbRectangle(ri, minThumb);
			if(ri.Orientation == Orientation.Horizontal) rect.X++;
			else rect.Y--;
			return rect;
		}
		public virtual void DrawMinThumb(TrackBarObjectInfoArgs e, bool bMirror) {
			RangeTrackBarViewInfo ri = e.ViewInfo as RangeTrackBarViewInfo;
			SkinElementInfo minInfo = GetMinThumbInfo(ri, e.State);
			if(minInfo == null) return;
			if(!ri.IsRightToLeft) minInfo.Bounds = GetMinVerticalThumbRectangle(ri, ri.MinThumbContentBounds);
			else minInfo.Bounds = GetVerticalThumbRectangle(ri, ri.MinThumbContentBounds);
			new RotateObjectPaintHelper().DrawRotated(e.Cache, minInfo, SkinElementPainter.Default, GetRotateAngle(e.ViewInfo), true);
		}
		public virtual void DrawMaxThumb(TrackBarObjectInfoArgs e, bool bMirror) {
			RangeTrackBarViewInfo ri = e.ViewInfo as RangeTrackBarViewInfo;
			SkinElementInfo maxInfo = GetMaxThumbInfo(ri, e.State);
			if(maxInfo == null) return;
			if(!ri.IsRightToLeft) maxInfo.Bounds = GetVerticalThumbRectangle(e.ViewInfo, ri.MaxThumbContentBounds);
			else maxInfo.Bounds = GetMinVerticalThumbRectangle(ri, ri.MaxThumbContentBounds);
			new RotateObjectPaintHelper().DrawRotated(e.Cache, maxInfo, SkinElementPainter.Default, GetRotateAngle(e.ViewInfo), true);
		}
		protected override RotateFlipType GetRotateAngle(TrackBarViewInfo vi) {
			if(vi.Orientation == Orientation.Horizontal) {
				if(vi.TickStyle == TickStyle.TopLeft) return RotateFlipType.RotateNoneFlipY;
				return RotateFlipType.RotateNoneFlipNone;
			}
			if(vi.TickStyle == TickStyle.TopLeft) {
				if(vi.IsRightToLeft) return RotateFlipType.Rotate90FlipX;
				return RotateFlipType.Rotate270FlipX;
			}
			if(vi.IsRightToLeft) return RotateFlipType.Rotate90FlipNone;
			if(vi.TickStyle == TickStyle.BottomRight) return RotateFlipType.Rotate270FlipNone;
			return RotateFlipType.Rotate270FlipNone;
		}
		public override void DrawMinArrowThumb(TrackBarObjectInfoArgs e, bool bMirror) {
			DrawMinThumb(e, bMirror);
		}
		public override void DrawMaxArrowThumb(TrackBarObjectInfoArgs e, bool bMirror) {
			DrawMaxThumb(e, bMirror);
		}
		public override void DrawMinRectThumb(TrackBarObjectInfoArgs e, bool bMirror) {
			DrawMinThumb(e, bMirror);
		}
		public override void DrawMaxRectThumb(TrackBarObjectInfoArgs e, bool bMirror) {
			DrawMaxThumb(e, bMirror);
		}
		protected override SkinElement GetTrack(TrackBarViewInfo vi) { return EditorsSkins.GetSkin(Provider)[EditorsSkins.SkinTrackBarTrack]; }
		protected override void DrawTrackLineCore(TrackBarObjectInfoArgs e, Rectangle bounds) {
			DrawSkinTrackLineCore(e, bounds);
		}
	}
	public class Office2003RangeTrackBarObjectPainter : RangeTrackBarObjectPainter {
		protected Brush GetFillBrush(TrackBarObjectInfoArgs e) {
			return new Office2003TrackBarThumbPainter().GetThumbFillBrush(e);
		}
		public override void FillThumb(TrackBarObjectInfoArgs e, bool bMirror, Point[] polygon, bool hot) {
			e.Graphics.FillPolygon(GetFillBrush(e), polygon);
		}
		protected virtual Color GetForeColor(TrackBarObjectInfoArgs e) {
			return new Office2003TrackBarThumbPainter().GetThumbBorderColor(e);
		}
		public override void DrawMinArrowThumb(TrackBarObjectInfoArgs e, bool bMirror) {
			RangeTrackBarViewInfo vi = e.ViewInfo as RangeTrackBarViewInfo;
			int[,] op = vi.TrackCalculator.ScalePoints(vi.MinArrowThumbOffset, vi.ThumbCriticalHeight);
			int[,] op1 = { { op[0, 0], op[0, 1] }, { op[1, 0], op[1, 1] }, { op[2, 0] - 1, op[2, 1] - 1 }, { op[3, 0], op[3, 1] } };
			int[,] op2 = { { op[1, 0], op[1, 1] }, { op[2, 0], op[2, 1] }, { op[3, 0], op[3, 1] }, { op[4, 0], op[4, 1] } };
			Color c = GetForeColor(e);
			Color[] color = { c, c, c, c};
			DrawThumb(e, bMirror, op1, op2, vi.MinThumbPos, color, 4);
		}
		public override void DrawMaxArrowThumb(TrackBarObjectInfoArgs e, bool bMirror) {
			RangeTrackBarViewInfo vi = e.ViewInfo as RangeTrackBarViewInfo;
			int[,] op = vi.TrackCalculator.ScalePoints(vi.MaxArrowThumbOffset, vi.ThumbCriticalHeight);
			int[,] op1 = { { op[0, 0], op[0, 1] }, { op[1, 0], op[1, 1] }, { op[2, 0], op[2, 1] }, { op[3, 0], op[3, 1] - 1 }, { op[3, 0] + 1, op[3, 1] - 1 } };
			int[,] op2 = { { op[1, 0], op[1, 1] }, { op[2, 0], op[2, 1] }, { op[3, 0], op[3, 1] }, { op[4, 0], op[4, 1] }, { op[4, 0] + 1, op[4, 1] + 1 } };
			Color c = GetForeColor(e);
			Color[] color = { c, c, c, c };
			DrawThumb(e, bMirror, op1, op2, vi.MaxThumbPos, color, 4);
		}
		public override void DrawRectThumb(TrackBarObjectInfoArgs e, bool bMirror, object offset, Point pt) {
			RangeTrackBarViewInfo vi = e.ViewInfo as RangeTrackBarViewInfo;
			int[,] op = vi.TrackCalculator.ScalePoints(offset, vi.ThumbCriticalHeight);
			int[,] op1 = { { op[0, 0], op[0, 1] }, { op[1, 0], op[1, 1] }, { op[2, 0], op[2, 1] }, { op[3, 0], op[3, 1] - 1 }, { op[2, 0] - 1, op[2, 1] - 1 }, { op[3, 0] + 1, op[3, 1] } };
			int[,] op2 = { { op[1, 0], op[1, 1] }, { op[2, 0], op[2, 1] }, { op[3, 0], op[3, 1] }, { op[4, 0], op[4, 1] }, { op[3, 0] + 1, op[3, 1] }, { op[4, 0] + 1, op[4, 1] + 1 } };
			Color c = GetForeColor(e);
			Color[] color = { c, c, c, c };
			DrawThumb(e, bMirror, op1, op2, pt, color, 4);
		}
	}
	public class RangeTrackBarObjectPainter : TrackBarObjectPainter {
		public static int RangeThumbBestWidth = 7;
		public virtual SkinElementInfo GetMinThumbInfo(RangeTrackBarViewInfo viewInfo, ObjectState state) {
			return null;
		}
		public virtual SkinElementInfo GetMaxThumbInfo(RangeTrackBarViewInfo viewInfo, ObjectState state) {
			return null;
		}
		public virtual SkinElementInfo GetBothThumbInfo(RangeTrackBarViewInfo viewInfo, EditHitTest hitButton, ObjectState state) {
			return null;
		}
		public virtual SkinElementInfo GetLineInfo(RangeTrackBarViewInfo viewInfo) {
			return null;
		}
		protected internal override TrackBarInfoCalculator GetCalculator(TrackBarViewInfo viewInfo) {
			return new RangeTrackBarInfoCalculator(viewInfo as RangeTrackBarViewInfo, this);
		}
		public override Rectangle GetFilledRect(TrackBarObjectInfoArgs e) {
			RangeTrackBarViewInfo ri = e.ViewInfo as RangeTrackBarViewInfo;
			Rectangle rect = ri.TrackLineRect;
			rect.X = ri.MinThumbPos.X;
			rect.Width = ri.MaxThumbPos.X - ri.MinThumbPos.X;
			return e.ViewInfo.TrackBarHelper.Rotate(rect);
		}
		public virtual void DrawMinArrowThumb(TrackBarObjectInfoArgs e, bool bMirror) {
			RangeTrackBarViewInfo vi = e.ViewInfo as RangeTrackBarViewInfo;
			int[,] op = vi.TrackCalculator.ScalePoints(vi.MinArrowThumbOffset, vi.ThumbCriticalHeight);
			int[,] op1 = { { op[0, 0] + 1, op[0, 1] + 1 }, { op[1, 0] - 1, op[1, 1] + 1 }, { op[0, 0], op[0, 1] }, { op[1, 0], op[1, 1] }, { op[2, 0] - 1, op[2, 1] - 1 }, { op[3, 0], op[3, 1] }, { op[2, 0] - 1, op[2, 1] - 1 }, { op[3, 0] + 1, op[3, 1] } };
			int[,] op2 = { { op[1, 0] - 1, op[1, 1] + 1 }, { op[2, 0] - 1, op[2, 1] - 1 }, { op[1, 0], op[1, 1] }, { op[2, 0], op[2, 1] }, { op[3, 0], op[3, 1] }, { op[4, 0], op[4, 1] }, { op[3, 0] + 1, op[3, 1] }, { op[4, 0] + 1, op[4, 1] + 1 } };
			Color[] color = { SystemColors.Control, SystemColors.ControlDark, SystemColors.ControlLightLight, SystemColors.ControlDarkDark, SystemColors.ControlLightLight, SystemColors.ControlLightLight, SystemColors.Control, SystemColors.Control };
			DrawThumb(e, bMirror, op1, op2, vi.MinThumbPos, color, 6);
		}
		public virtual void DrawMaxArrowThumb(TrackBarObjectInfoArgs e, bool bMirror) {
			RangeTrackBarViewInfo vi = e.ViewInfo as RangeTrackBarViewInfo;
			int[,] op = vi.TrackCalculator.ScalePoints(vi.MaxArrowThumbOffset, vi.ThumbCriticalHeight);
			int[,] op1 = { { op[2, 0] - 1, op[2, 1] }, { op[0, 0] + 1, op[0, 1] + 1 }, { op[1, 0] - 1, op[1, 1] + 1 }, { op[0, 0], op[0, 1] }, { op[1, 0], op[1, 1] }, { op[2, 0], op[2, 1] }, { op[3, 0], op[3, 1] - 1 }, { op[3, 0] + 1, op[3, 1] - 1 } };
			int[,] op2 = { { op[3, 0] + 1, op[3, 1] - 2 }, { op[1, 0] - 1, op[1, 1] + 1 }, { op[2, 0] - 1, op[2, 1] }, { op[1, 0], op[1, 1] }, { op[2, 0], op[2, 1] }, { op[3, 0], op[3, 1] }, { op[4, 0], op[4, 1] }, { op[4, 0] + 1, op[4, 1] + 1 } };
			Color[] color = { SystemColors.ControlDark, SystemColors.Control, SystemColors.ControlDark, SystemColors.ControlLightLight, SystemColors.ControlDarkDark, SystemColors.ControlDarkDark, SystemColors.ControlLightLight, SystemColors.Control };
			DrawThumb(e, bMirror, op1, op2, vi.MaxThumbPos, color, 7);
		}
		public virtual void DrawRectThumb(TrackBarObjectInfoArgs e, bool bMirror, object offset, Point pt) {
			RangeTrackBarViewInfo vi = e.ViewInfo as RangeTrackBarViewInfo;
			int[,] op = vi.TrackCalculator.ScalePoints(offset, vi.ThumbCriticalHeight);
			int[,] op1 = { { op[0, 0] + 1, op[0, 1] + 1 }, { op[1, 0] - 1, op[1, 1] + 1 }, { op[0, 0], op[0, 1] }, { op[1, 0], op[1, 1] }, { op[2, 0], op[2, 1] }, { op[3, 0], op[3, 1] - 1 }, { op[2, 0] - 1, op[2, 1] - 1 }, { op[3, 0] + 1, op[3, 1] } };
			int[,] op2 = { { op[1, 0] - 1, op[1, 1] + 1 }, { op[2, 0] - 1, op[2, 1] - 1 }, { op[1, 0], op[1, 1] }, { op[2, 0], op[2, 1] }, { op[3, 0], op[3, 1] }, { op[4, 0], op[4, 1] }, { op[3, 0] + 1, op[3, 1] }, { op[4, 0] + 1, op[4, 1] + 1 } };
			Color[] color = { SystemColors.Control, SystemColors.ControlDark, SystemColors.ControlLightLight, SystemColors.ControlDarkDark, SystemColors.ControlDarkDark, SystemColors.ControlLightLight, SystemColors.Control, SystemColors.Control };
			DrawThumb(e, bMirror, op1, op2, pt, color, 6);
		}
		public virtual void DrawMinRectThumb(TrackBarObjectInfoArgs e, bool bMirror) {
			RangeTrackBarViewInfo vi = e.ViewInfo as RangeTrackBarViewInfo;
			DrawRectThumb(e, bMirror, vi.MinRectThumbOffset, vi.MinThumbPos);
		}
		public virtual void DrawMaxRectThumb(TrackBarObjectInfoArgs e, bool bMirror) {
			RangeTrackBarViewInfo vi = e.ViewInfo as RangeTrackBarViewInfo;
			DrawRectThumb(e, bMirror, vi.MaxRectThumbOffset, vi.MaxThumbPos);
		}
		public override void DrawRectThumb(TrackBarObjectInfoArgs e) {
			FillThumb(e, false);
			DrawMaxRectThumb(e, false);
			DrawMinRectThumb(e, false);
		}
		public override void DrawArrowThumb(TrackBarObjectInfoArgs e, bool bMirror) {
			FillThumb(e, bMirror);
			DrawMaxArrowThumb(e, bMirror);
			DrawMinArrowThumb(e, bMirror);
		}
		public override void FillThumb(TrackBarObjectInfoArgs e, bool bMirror) {
			RangeTrackBarViewInfo viewInfo = e.ViewInfo as RangeTrackBarViewInfo;
			FillThumb(e, bMirror, viewInfo.MinThumbRegion, e.ViewInfo.PressedInfo.HitTest == EditHitTest.Button);
			FillThumb(e, bMirror, viewInfo.MaxThumbRegion, e.ViewInfo.PressedInfo.HitTest == EditHitTest.Button2);
		}
		public override int GetThumbBestWidth(TrackBarViewInfo vi) { return RangeTrackBarObjectPainter.RangeThumbBestWidth; }
		public virtual void DrawThumb(TrackBarObjectInfoArgs e, bool bMirror, int[,] offsetP1, int[,] offsetP2, Point center, Color[] colors, int lineCount) {
			Point p1 = Point.Empty, p2 = Point.Empty;
			int lineIndex;
			for(lineIndex = 0; lineIndex < lineCount; lineIndex++) {
				p1 = center; p2 = center;
				p1.X += offsetP1[lineIndex, 0];
				p1.Y += offsetP1[lineIndex, 1];
				p2.X += offsetP2[lineIndex, 0];
				p2.Y += offsetP2[lineIndex, 1];
				e.Paint.DrawLine(e.Graphics, e.Cache.GetPen(colors[lineIndex]), e.ViewInfo.TrackBarHelper.RotateAndMirror(p1, e.ViewInfo.MirrorPoint.Y, bMirror), e.ViewInfo.TrackBarHelper.RotateAndMirror(p2, e.ViewInfo.MirrorPoint.Y, bMirror));
			}
		}
	}
	public class RangeTrackBarPainter : TrackBarPainter { 
	}
}
namespace DevExpress.Accessibility {
	public class RangeTrackBarAccessible : TrackBarAccessible {
		public RangeTrackBarAccessible(RepositoryItem item) : base(item) { } 
	}	
}
namespace DevExpress.XtraEditors {
	[DXToolboxItem(DXToolboxItemKind.Free), Designer("DevExpress.XtraEditors.Design.TrackBarDesigner, " + AssemblyInfo.SRAssemblyEditorsDesign),
	 Description("Allows an end-user to select a range of values."),
	 ToolboxTabName(AssemblyInfo.DXTabCommon),
	ToolboxBitmap(typeof(ToolboxIconsRootNS), "RangeTrackBarControl")
	]
	public class RangeTrackBarControl : TrackBarControl {
		public RangeTrackBarControl() : base() {
			EditValue = TrackBarRange.Empty;
			AllowRangeDragging = false;
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("TrackBarControlProperties"),
#endif
 DXCategory(CategoryName.Properties), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), SmartTagSearchNestedProperties]
		public new RepositoryItemRangeTrackBar Properties { get { return base.Properties as RepositoryItemRangeTrackBar; } }
		public override string EditorTypeName { get { return "RangeTrackBarControl"; } }
		protected override object ConvertCheckValue(object val) {
			if((val == null) || (val is DBNull)) {
				return val;
			}
			return this.Properties.ConvertRangeValue(val);
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("TrackBarControlValue"),
#endif
 DXCategory(CategoryName.Behavior), Bindable(true)]
		public new TrackBarRange Value {
			get { return Properties.ConvertRangeValue(EditValue); }
			set {
				EditValue = Properties.CheckRangeValue(value);
				if(IsHandleCreated) Update();
			}
		}
		bool ShouldSerializeValue() { return Value != TrackBarRange.Empty; }
		protected override bool ShouldProcessHitTest(EditHitInfo hi) { return hi.HitTest == EditHitTest.Button2 || hi.HitTest == EditHitTest.Button; }
		protected override bool ShouldProcessTrackBarMouseMove { get { return false; } }
		protected override void OnMouseWheel(MouseEventArgs e) { }
		protected virtual void ShowMinValue() {
			ShowValue(Value.ToString(), (ViewInfo as RangeTrackBarViewInfo).MinThumbBounds);
		}
		protected virtual void ShowMaxValue() {
			ShowValue(Value.ToString(), (ViewInfo as RangeTrackBarViewInfo).MaxThumbBounds);
		}
		protected int CalcValueMinimum(Point p) {
			int rangeCenter = (Value.Minimum + Value.Maximum) / 2;
			int currentValue = ViewInfo.ValueFromPoint(ViewInfo.ControlToClient(p));
			return (Value.Maximum + currentValue - rangeCenter) < Properties.Maximum ?
					Value.Minimum + currentValue - rangeCenter : Properties.Maximum - (Value.Maximum - Value.Minimum);
		}
		protected int CalcValueMaximum(Point p) {
			int rangeCenter = (Value.Minimum + Value.Maximum) / 2;
			int currentValue = ViewInfo.ValueFromPoint(ViewInfo.ControlToClient(p));
			return (Value.Minimum + currentValue - rangeCenter) > Properties.Minimum ?
					Value.Maximum + currentValue - rangeCenter : Properties.Minimum + (Value.Maximum - Value.Minimum);   
		}
		bool shouldShowMaximumValue = false;
		protected override void OnMouseMove(MouseEventArgs e) {
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(e);
			base.OnMouseMove(ee);
			if(ee.Handled || ProcessGesture) return;
			ProcessRangeTrackBarMouseMove(e.Location);
		}
		void ProcessRangeTrackBarMouseMove(Point p) {
			if(Properties.ReadOnly) return;
			if(ViewInfo.PressedInfo.HitTest == EditHitTest.Button) {
				Value = new TrackBarRange(Math.Min(ViewInfo.ValueFromPoint(ViewInfo.ControlToClient(p)), Value.Maximum), Value.Maximum);
				ShowMinValue();
			}
			else if(ViewInfo.PressedInfo.HitTest == EditHitTest.Button2) {
				Value = new TrackBarRange(Value.Minimum, Math.Max(Value.Minimum, ViewInfo.ValueFromPoint(ViewInfo.ControlToClient(p))));
				ShowMaxValue();
			}
			else if(DragRangeState) {
				TrackBarRange newValue = new TrackBarRange(CalcValueMinimum(p), CalcValueMaximum(p));
				if(newValue.Maximum != Value.Maximum)
					shouldShowMaximumValue = newValue.Maximum > Value.Maximum;
				Value = newValue;
				if(shouldShowMaximumValue)
					ShowMaxValue();
				else
					ShowMinValue();
			}
		}
		[DefaultValue(false)]
		public bool AllowRangeDragging {
			get;
			set;
		}
		bool DragRangeState = false;
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			if(ProcessGesture) return;
			ProcessRangeTrackBarMouseUp(); 
		}
		void ProcessRangeTrackBarMouseUp() {
			if(AllowRangeDragging) DragRangeState = false;
		}
		protected override void UpdateValueFromPoint(Point pt) {
			if(!ShouldUpdateValueFromPoint(pt)) return;
			int value = ViewInfo.ValueFromPoint(ViewInfo.ControlToClient(pt));
			if(AllowRangeDragging)
				if((value > Value.Minimum) && (value < Value.Maximum)) {
					DragRangeState = true;
					return;
				}
			if(Math.Abs(value - Value.Minimum) < Math.Abs(value - Value.Maximum)) Value = new TrackBarRange(value, Value.Maximum);
			else Value = new TrackBarRange(Value.Minimum, value);
			if(AllowRangeDragging) DragRangeState = true; 
		}
		protected override bool ShouldProcessTrackKeyDown { get { return false; } }
		protected override void ProcessGidEnd(Point p) {
			base.ProcessGidEnd(p);
			ProcessRangeTrackBarMouseUp();
		}
		protected override void ProcessGidPan(Point p) {
			base.ProcessGidPan(p);
			ProcessRangeTrackBarMouseMove(p);
		}
	}	
}
