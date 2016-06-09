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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler;
using System.ComponentModel;
using System.Windows.Markup;
using DependencyPropertyHelper = DevExpress.Xpf.Core.Native.DependencyPropertyHelper;
#if SILVERLIGHT
using DevExpress.Xpf.Editors.Controls;
#else
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using System.Collections.Specialized;
using System.Windows.Input;
using DevExpress.XtraScheduler.Drawing;
using System.Windows.Controls.Primitives;
using DevExpress.Xpf.Scheduler.Native;
#endif
namespace DevExpress.Xpf.Scheduler.Drawing {
	public class ElementPositionConverter : TypeConverter {
		internal static ElementPosition CreateFromString(string s) {
			string[] tokens = s.Split(',');
			if(tokens.Length < 4)
				Exceptions.ThrowArgumentException("ElementPosition", s);
			ElementRelativePosition horizontalElementPosition = (ElementRelativePosition)Enum.Parse(typeof(ElementRelativePosition), tokens[0], true);
			ElementRelativePosition verticalElementPosition = (ElementRelativePosition)Enum.Parse(typeof(ElementRelativePosition), tokens[1], true);
			InnerSeparator horizontalSeparatorPosition = (InnerSeparator)Enum.Parse(typeof(InnerSeparator), tokens[2], true);
			InnerSeparator verticalSeparatorPosition = (InnerSeparator)Enum.Parse(typeof(InnerSeparator), tokens[3], true);
			if(tokens.Length == 4)
				return new ElementPosition(horizontalElementPosition, verticalElementPosition, horizontalSeparatorPosition, verticalSeparatorPosition);
			if(tokens.Length < 6)
				Exceptions.ThrowArgumentException("ElementPosition", s);
			OuterSeparator ownHorizontalSeparatorPosition = ParseOuterSeparator(tokens[4]);
			OuterSeparator ownVerticalSeparatorPosition = ParseOuterSeparator(tokens[5]);
			if(tokens.Length == 6)
				return new ElementPosition(horizontalElementPosition, verticalElementPosition, horizontalSeparatorPosition, verticalSeparatorPosition, ownHorizontalSeparatorPosition, ownVerticalSeparatorPosition, MarginPosition.NotDefined, MarginPosition.NotDefined);
			if(tokens.Length < 8)
				Exceptions.ThrowArgumentException("ElementPosition", s);
			MarginPosition ownHorizontalMargin = (MarginPosition)Enum.Parse(typeof(MarginPosition), tokens[6], true);
			MarginPosition ownVerticalMargin = (MarginPosition)Enum.Parse(typeof(MarginPosition), tokens[7], true);
			if(tokens.Length == 8)
				return new ElementPosition(horizontalElementPosition, verticalElementPosition, horizontalSeparatorPosition, verticalSeparatorPosition, ownHorizontalSeparatorPosition, ownVerticalSeparatorPosition, ownHorizontalMargin, ownVerticalMargin);
			if(tokens.Length != 9)
				Exceptions.ThrowArgumentException("ElementPosition", s);
			string[] thicknessParts = tokens[8].Split(',');
			double left = Double.Parse(thicknessParts[0], System.Globalization.CultureInfo.InvariantCulture);
			double top = Double.Parse(thicknessParts[1], System.Globalization.CultureInfo.InvariantCulture);
			double right = Double.Parse(thicknessParts[2], System.Globalization.CultureInfo.InvariantCulture);
			double bottom = Double.Parse(thicknessParts[3], System.Globalization.CultureInfo.InvariantCulture);
			Thickness thickness = new Thickness(left, top, right, bottom);
			return new ElementPosition(horizontalElementPosition, verticalElementPosition, horizontalSeparatorPosition, verticalSeparatorPosition, ownHorizontalSeparatorPosition, ownVerticalSeparatorPosition, ownHorizontalMargin, ownVerticalMargin, thickness);
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if(sourceType == typeof(String))
				return true;
			return base.CanConvertFrom(context, sourceType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
			String s = value as String;
			if(s != null)
				return FromString(s);
			return base.ConvertFrom(context, culture, value);
		}
		protected virtual ElementPosition FromString(string s) {
			return CreateFromString(s);
		}
		internal static OuterSeparator ParseOuterSeparator(string stringValue) {
			string[] tokens = stringValue.Split('|');
			int count = tokens.Length;
			OuterSeparator result = OuterSeparator.NotDefined;
			for(int i = 0; i < count; i++)
				result |= (OuterSeparator)Enum.Parse(typeof(OuterSeparator), tokens[i].Trim(), true);
			return result;
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if(destinationType == typeof(String))
				return true;
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
			ElementPosition position = value as ElementPosition;
			if(destinationType == typeof(String) && position != null)
				return position.ToString();
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
	public class ElementPositionCore {
		public ElementRelativePosition ElementPosition { get; set; }
		public InnerSeparator InnerSeparator { get; set; }
		public OuterSeparator OuterSeparator { get; set; }
		public bool IsStart {
			get { return (ElementPosition & ElementRelativePosition.Start) != 0; }
		}
		public bool IsEnd {
			get { return (ElementPosition & ElementRelativePosition.End) != 0; }
		}
		public ElementPositionCore Clone() {
			ElementPositionCore result = new ElementPositionCore();
			result.ElementPosition = ElementPosition;
			result.InnerSeparator = InnerSeparator;
			result.OuterSeparator = OuterSeparator;
			return result;
		}
		public override bool Equals(object obj) {
			ElementPositionCore other = obj as ElementPositionCore;
			if (other == null)
				return false;
			if (ElementPosition != other.ElementPosition)
				return false;
			if (InnerSeparator != other.InnerSeparator)
				return false;
			if (OuterSeparator != other.OuterSeparator)
				return false;
			return true;
		}
		public override int GetHashCode() {
			return 0;
		}
	}
	[TypeConverter(typeof(ElementPositionConverter))]
	[ContentProperty("Content")]
	public class ElementPosition {
		static readonly ElementPosition standalone = new ElementPosition(ElementRelativePosition.Standalone, ElementRelativePosition.Standalone, InnerSeparator.Both, InnerSeparator.Both);
		public static ElementPosition Standalone { get { return standalone; } }
		string content;
		public ElementPosition() { 
		}
		public ElementPosition(ElementPosition position, OuterSeparatorPosition outerSeparator)
			: this(position.HorizontalElementPosition.ElementPosition, position.VerticalElementPosition.ElementPosition, position.HorizontalElementPosition.InnerSeparator, position.VerticalElementPosition.InnerSeparator, outerSeparator.HorizontalSeparatorPosition, outerSeparator.VerticalSeparatorPosition, position.OwnHorizontalMarginPosition, position.OwnVerticalMarginPosition) {
		}
		public ElementPosition(ElementPositionCore horizontalElementPosition, ElementPositionCore verticalElementPosition) {
			this.HorizontalElementPosition = horizontalElementPosition;
			this.VerticalElementPosition = verticalElementPosition;
		}
		public ElementPosition(ElementRelativePosition horizontalPosition, ElementRelativePosition verticalPostion, InnerSeparator horizontalSeparatorPosition, InnerSeparator verticalSeparatorPosition)
			: this(horizontalPosition, verticalPostion, horizontalSeparatorPosition, verticalSeparatorPosition, OuterSeparator.NotDefined, OuterSeparator.NotDefined, MarginPosition.NotDefined, MarginPosition.NotDefined) {
		}
		public ElementPosition(ElementRelativePosition horizontalPosition, ElementRelativePosition verticalPostion, InnerSeparator horizontalSeparatorPosition, InnerSeparator verticalSeparatorPosition, OuterSeparator ownHorizontalSeparatorPosition, OuterSeparator ownVerticalSeparatorPosition, MarginPosition ownHorizontalMarginPosition, MarginPosition ownVerticalMarginPosition)
			: this(horizontalPosition, verticalPostion, horizontalSeparatorPosition, verticalSeparatorPosition, ownHorizontalSeparatorPosition, ownVerticalSeparatorPosition, ownHorizontalMarginPosition, ownVerticalMarginPosition, new Thickness(0)) {
		}
		public ElementPosition(ElementRelativePosition horizontalPosition, ElementRelativePosition verticalPosition, InnerSeparator horizontalSeparatorPosition, InnerSeparator verticalSeparatorPosition, OuterSeparator ownHorizontalSeparatorPosition, OuterSeparator ownVerticalSeparatorPosition, MarginPosition ownHorizontalMarginPosition, MarginPosition ownVerticalMarginPosition, Thickness innerContentPadding) {
			ElementPositionCore horizontalElementPosition = new ElementPositionCore();
			horizontalElementPosition.ElementPosition = horizontalPosition;
			horizontalElementPosition.InnerSeparator = horizontalSeparatorPosition;
			horizontalElementPosition.OuterSeparator = ownHorizontalSeparatorPosition;
			this.HorizontalElementPosition = horizontalElementPosition;
			ElementPositionCore verticalElementPosition = new ElementPositionCore();
			verticalElementPosition.ElementPosition = verticalPosition;
			verticalElementPosition.InnerSeparator = verticalSeparatorPosition;
			verticalElementPosition.OuterSeparator = ownVerticalSeparatorPosition;
			this.VerticalElementPosition = verticalElementPosition;
			this.OwnHorizontalMarginPosition = ownHorizontalMarginPosition;
			this.OwnVerticalMarginPosition = ownVerticalMarginPosition;
			this.InnerContentPadding = innerContentPadding;
		}
		public ElementPositionCore HorizontalElementPosition { get; set; }
		public ElementPositionCore VerticalElementPosition { get; set; }
		public ElementRelativePosition HorizontalPosition { get { return HorizontalElementPosition.ElementPosition; } }
		public ElementRelativePosition VerticalPosition { get { return VerticalElementPosition.ElementPosition; } }
		public InnerSeparator HorizontalSeparatorPosition { get { return HorizontalElementPosition.InnerSeparator; } }
		public InnerSeparator VerticalSeparatorPosition { get { return VerticalElementPosition.InnerSeparator; } }
		public OuterSeparator OwnHorizontalSeparatorPosition { get { return HorizontalElementPosition.OuterSeparator; } }
		public OuterSeparator OwnVerticalSeparatorPosition { get { return VerticalElementPosition.OuterSeparator; } }
		public MarginPosition OwnHorizontalMarginPosition { get; protected set; }
		public MarginPosition OwnVerticalMarginPosition { get; protected set; }
		public Thickness InnerContentPadding { get; protected set; }
		public ElementRelativePosition VerticalWeekHorizontalPosition { get; set; }
		public ElementRelativePosition VerticalWeekVerticalPosition { get; set; }
		#region Content
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string Content {
			get {
				return content;
			}
			set {
				content = value;
				if(content == null)
					return;
				ElementPosition ep = ElementPositionConverter.CreateFromString(value);
				Assign(ep);
			}
		}
		#endregion
		void Assign(ElementPosition ep) {
			HorizontalElementPosition = ep.HorizontalElementPosition;
			VerticalElementPosition = ep.VerticalElementPosition;
			OwnHorizontalMarginPosition = ep.OwnHorizontalMarginPosition;
			OwnVerticalMarginPosition = ep.OwnVerticalMarginPosition;
			InnerContentPadding = ep.InnerContentPadding;
			VerticalWeekHorizontalPosition = ep.VerticalWeekHorizontalPosition;
			VerticalWeekVerticalPosition = ep.VerticalWeekVerticalPosition;
		}
		public override bool Equals(object obj) {
			ElementPosition other = obj as ElementPosition;
			if (other == null)
				return base.Equals(obj);
			if (!Object.Equals(HorizontalElementPosition,other.HorizontalElementPosition))
				return false;
			if (!Object.Equals(VerticalElementPosition, other.VerticalElementPosition))
				return false;
			if (OwnHorizontalMarginPosition != other.OwnHorizontalMarginPosition)
				return false;
			if (OwnVerticalMarginPosition != other.OwnVerticalMarginPosition)
				return false;
			if (InnerContentPadding != other.InnerContentPadding)
				return false;
			if (VerticalWeekHorizontalPosition != other.VerticalWeekHorizontalPosition)
				return false;
			if (VerticalWeekVerticalPosition != other.VerticalWeekVerticalPosition)
				return false; 
			return true;
		}
		public override string ToString() {
			return String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}", HorizontalPosition, VerticalPosition, HorizontalSeparatorPosition, VerticalSeparatorPosition, OwnHorizontalSeparatorPosition, OwnVerticalSeparatorPosition, OwnHorizontalMarginPosition, OwnVerticalMarginPosition, InnerContentPadding);
		}
		public override int GetHashCode() {
			return (((int)HorizontalPosition) << 28) + (((int)VerticalPosition) << 24) + (((int)HorizontalSeparatorPosition) << 20) + (((int)VerticalSeparatorPosition) << 16)
				+ (((int)OwnVerticalSeparatorPosition) << 12) + (((int)OwnVerticalSeparatorPosition) << 8) + (((int)OwnHorizontalMarginPosition) << 4) + (((int)OwnVerticalMarginPosition));
		}
		public bool HasLeftBorder {
			get {
				if((OwnHorizontalSeparatorPosition & OuterSeparator.Start) != 0)
					return true;
				if((OwnHorizontalSeparatorPosition & OuterSeparator.NoStart) != 0)
					return false;
				return (HorizontalSeparatorPosition & InnerSeparator.Start) != 0;
			}
		}
		public bool HasRightBorder {
			get {
				if((OwnHorizontalSeparatorPosition & OuterSeparator.End) != 0)
					return true;
				if((OwnHorizontalSeparatorPosition & OuterSeparator.NoEnd) != 0)
					return false;
				return (HorizontalSeparatorPosition & InnerSeparator.End) != 0;
			}
		}
		public bool HasTopBorder {
			get {
				if((OwnVerticalSeparatorPosition & OuterSeparator.Start) != 0)
					return true;
				if((OwnVerticalSeparatorPosition & OuterSeparator.NoStart) != 0)
					return false;
				return (VerticalSeparatorPosition & InnerSeparator.Start) != 0;
			}
		}
		public bool HasBottomBorder {
			get {
				if((OwnVerticalSeparatorPosition & OuterSeparator.End) != 0)
					return true;
				if((OwnVerticalSeparatorPosition & OuterSeparator.NoEnd) != 0)
					return false;
				return (VerticalSeparatorPosition & InnerSeparator.End) != 0;
			}
		}
		public bool IsLeft { get { return (HorizontalElementPosition.ElementPosition & ElementRelativePosition.Start) != 0; } }
		public bool IsRight { get { return (HorizontalElementPosition.ElementPosition & ElementRelativePosition.End) != 0; } }
		public bool IsTop { get { return (VerticalElementPosition.ElementPosition & ElementRelativePosition.Start) != 0; } }
		public bool IsBottom { get { return (VerticalElementPosition.ElementPosition & ElementRelativePosition.End) != 0; } }
		public virtual ElementPosition Clone() {
			ElementPosition result = new ElementPosition(HorizontalElementPosition.Clone(), VerticalElementPosition.Clone());
			result.VerticalWeekVerticalPosition = VerticalWeekVerticalPosition;
			result.VerticalWeekHorizontalPosition = VerticalWeekHorizontalPosition;
			return result;
		}
	}
	[TypeConverter(typeof(OuterSeparatorPositionConverter))]
	[ContentProperty("Content")]
	public class OuterSeparatorPosition {
		OuterSeparator horizontal;
		OuterSeparator vertical;
		string content;
		public OuterSeparatorPosition() {
		}
		public OuterSeparatorPosition(OuterSeparator horizontal, OuterSeparator vertical) {
			this.horizontal = horizontal;
			this.vertical = vertical;
		}
		public OuterSeparator HorizontalSeparatorPosition { get { return horizontal; } }
		public OuterSeparator VerticalSeparatorPosition { get { return vertical; } }
		#region Content
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public String Content {
			get {
				return content;
			}
			set {
				content = value;
				if(content == null)
					return;
				OuterSeparatorPosition osp = OuterSeparatorPositionConverter.CreateFromString(content);
				Assign(osp);
			}
		}
		#endregion
		void Assign(OuterSeparatorPosition other) {
			this.horizontal = other.HorizontalSeparatorPosition;
			this.vertical = other.VerticalSeparatorPosition;
		}
	}
	public class OuterSeparatorPositionConverter : TypeConverter {
		internal static OuterSeparatorPosition CreateFromString(string s) {
			string[] tokens = s.Split(',');
			if(tokens.Length != 2)
				Exceptions.ThrowArgumentException("ElementPosition", s);
			OuterSeparator horizontal = ElementPositionConverter.ParseOuterSeparator(tokens[0]);
			OuterSeparator vertical = ElementPositionConverter.ParseOuterSeparator(tokens[1]);
			return new OuterSeparatorPosition(horizontal, vertical);
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if(sourceType == typeof(String))
				return true;
			return base.CanConvertFrom(context, sourceType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
			String s = value as String;
			if(s != null)
				return FromString(s);
			return base.ConvertFrom(context, culture, value);
		}
		protected virtual OuterSeparatorPosition FromString(string s) {
			return CreateFromString(s);
		}
	}
	[Flags]
	public enum ElementRelativePosition {
		NotDefined = 0,
		Start = 1,
		End = 2,
		Middle = 4,
		Standalone = Start | End
	}
	[Flags]
	public enum InnerSeparator {
		None = 0,
		Start = 1,
		End = 2,
		Both = Start | End
	}
	[Flags]
	public enum OuterSeparator {
		NotDefined = 0,
		Start = 1,
		End = 2,
		Both = Start | End,
		NoStart = 4,
		NoEnd = 8,
		None = NoStart | NoEnd,
		NoStartCorner = 16,
		NoEndCorner = 32,
		SharpEnd = End | NoEndCorner,
		SharpStart = Start | NoStartCorner
	}
	[Flags]
	public enum MarginPosition {
		NotDefined = 0,
		Start = 1,
		End = 2
	}
	public class ResourceNavigatorVisibilityMarginResolver : DependencyObject, IValueConverter {
		#region MarginWhenVisible
		public Thickness MarginWhenVisible {
			get { return (Thickness)GetValue(MarginWhenVisibleProperty); }
			set { SetValue(MarginWhenVisibleProperty, value); }
		}
		public static readonly DependencyProperty MarginWhenVisibleProperty = CreateMarginWhenVisibleProperty();
		static DependencyProperty CreateMarginWhenVisibleProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ResourceNavigatorVisibilityMarginResolver, Thickness>("MarginWhenVisible", new Thickness(0), (d, e) => d.OnMarginWhenVisibleChanged(e.OldValue, e.NewValue));
		}
		void OnMarginWhenVisibleChanged(Thickness oldValue, Thickness newValue) {
		}
		#endregion
		#region MarginWhenInvisible
		public Thickness MarginWhenInvisible {
			get { return (Thickness)GetValue(MarginWhenInvisibleProperty); }
			set { SetValue(MarginWhenInvisibleProperty, value); }
		}
		public static readonly DependencyProperty MarginWhenInvisibleProperty = CreateMarginWhenInvisibleProperty();
		static DependencyProperty CreateMarginWhenInvisibleProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ResourceNavigatorVisibilityMarginResolver, Thickness>("MarginWhenInvisible", new Thickness(0), (d, e) => d.OnMarginWhenInvisibleChanged(e.OldValue, e.NewValue));
		}
		void OnMarginWhenInvisibleChanged(Thickness oldValue, Thickness newValue) {
		}
		#endregion
		#region IValueConverter Members
		public virtual object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if(!(value is ResourceNavigatorVisibility))
				return new Thickness(0);
			ResourceNavigatorVisibility visibility = (ResourceNavigatorVisibility)value;
			return (visibility == ResourceNavigatorVisibility.Never) ? MarginWhenInvisible : MarginWhenVisible;
		}
		public virtual object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return value;
		}
		#endregion
	}
	public class ResourceNavigatorVisibilityEmptyMarginResolver : ResourceNavigatorVisibilityMarginResolver {
		public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return new Thickness(0);
		}
	}
	public class ConverterProxy : FrameworkElement {
		public ConverterProxy() {
			Loaded += new RoutedEventHandler(OnLoaded);
		}
		#region ConverterResourceName
		public object ConverterResourceName {
			get { return (object)GetValue(ConverterResourceNameProperty); }
			set { SetValue(ConverterResourceNameProperty, value); }
		}
		public static readonly DependencyProperty ConverterResourceNameProperty = CreateConverterResourceNameProperty();
		static DependencyProperty CreateConverterResourceNameProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ConverterProxy, object>("ConverterResourceName", null, (d, e) => d.OnConverterResourceNameChanged(e.OldValue, e.NewValue));
		}
		void OnConverterResourceNameChanged(object oldValue, object newValue) {
			UpdateResultValue();
		}
		#endregion
		#region ValueToConvert
		public object ValueToConvert {
			get { return (object)GetValue(ValueToConvertProperty); }
			set { SetValue(ValueToConvertProperty, value); }
		}
		public static readonly DependencyProperty ValueToConvertProperty = CreateValueToConvertProperty();
		static DependencyProperty CreateValueToConvertProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ConverterProxy, object>("ValueToConvert", null, (d, e) => d.OnValueToConvertChanged(e.OldValue, e.NewValue));
		}
		void OnValueToConvertChanged(object oldValue, object newValue) {
			UpdateResultValue();
		}
		#endregion
		#region ConvertedValue
		public object ConvertedValue {
			get { return (object)GetValue(ConvertedValueProperty); }
			set { SetValue(ConvertedValueProperty, value); }
		}
		public static readonly DependencyProperty ConvertedValueProperty = CreateConvertedValueProperty();
		static DependencyProperty CreateConvertedValueProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ConverterProxy, object>("ConvertedValue", null);
		}
		#endregion
		void OnLoaded(object sender, RoutedEventArgs e) {
			UpdateResultValue();
		}
		void UpdateResultValue() {
			if(ValueToConvert != null && ConverterResourceName != null) {
				IValueConverter converter = FindResource(this, ConverterResourceName) as IValueConverter;
				if(converter == null)
					return;
				ConvertedValue = converter.Convert(ValueToConvert, null, null, null);
			}
		}
		public object FindResource(FrameworkElement root, object key) {
			if(root == null || key == null)
				return null;
#if SL
			object result = root.Resources[key];
#else
			object result = root.TryFindResource(key);
#endif
			if(result == null) {
				FrameworkElement parent = VisualTreeHelper.GetParent(root) as FrameworkElement;
				if(parent != null)
					result = FindResource(parent, key);
			}
			return result;
		}
	}
}
