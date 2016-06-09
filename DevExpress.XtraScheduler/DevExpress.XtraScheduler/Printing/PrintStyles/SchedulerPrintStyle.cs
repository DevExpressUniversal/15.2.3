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

using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using DevExpress.XtraScheduler.Native;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.XtraScheduler.Native {
	public delegate void SchedulerPrintStylePropertyValidationCallback(object property, object value);
}
namespace DevExpress.XtraScheduler.Printing {
	[TypeConverter(typeof(DevExpress.Utils.Design.UniversalTypeConverterEx))]
	public abstract class SchedulerPrintStyle : UserInterfaceObject, ICaptionSupport, ICloneable, IDisposable {
		#region static
		internal static readonly Font defaultHeadingsFont = (new Font(FontFamily.GenericSerif, 14)).Clone() as Font;
		internal static readonly Font defaultAppointmentFont = (new Font(FontFamily.GenericSerif, 12)).Clone() as Font;
		internal static readonly object NullObject = new object();
		static DefaultPageSettingsProvider defaultPageSettingsProvider;
		internal static SchedulerPrintStyle CreateInstanceCore(SchedulerPrintStyleKind kind) {
			switch (kind) {
				case SchedulerPrintStyleKind.Daily:
					return new DailyPrintStyle(false, false);
				case SchedulerPrintStyleKind.Weekly:
					return new WeeklyPrintStyle(false, false);
				case SchedulerPrintStyleKind.Monthly:
					return new MonthlyPrintStyle(false, false);
				case SchedulerPrintStyleKind.TriFold:
					return new TriFoldPrintStyle(false, false);
				case SchedulerPrintStyleKind.CalendarDetails:
					return new CalendarDetailsPrintStyle(false, false);
				case SchedulerPrintStyleKind.Memo:
					return new MemoPrintStyle(false, false);
				default:
					XtraSchedulerDebug.Assert(false);
					return null;
			}
		}
		internal static SchedulerPrintStyle CreateInstance(SchedulerPrintStyleKind kind, bool baseStyle) {
			SchedulerPrintStyle style = CreateInstanceCore(kind);
			style.RegisterProperties();
			style.SetPropertyValue(baseStyleProperty, baseStyle);
			return style;
		}
		#endregion
		#region static UserInterfaceObject
		static readonly Size largeImageSize = new Size(128, 76);
		static readonly Size smallImageSize = new Size(58, 32);
		static readonly Color transparentColor = Color.Fuchsia;
		static readonly ImageList LargeImageList = new ImageList();
		internal static readonly ImageList SmallImageList = new ImageList();
		static readonly Hashtable htImagesIndex = new Hashtable();
		const string bitmapNameFormatString = "DevExpress.XtraScheduler.Images.{0}_{1}.bmp";
		static SchedulerPrintStyle() {
			LargeImageList.ImageSize = largeImageSize;
			LargeImageList.ColorDepth = ColorDepth.Depth32Bit;
			SmallImageList.ImageSize = smallImageSize;
			SmallImageList.ColorDepth = ColorDepth.Depth32Bit;
			defaultPageSettingsProvider = new DefaultPageSettingsProvider();
		}
		#endregion
		bool isDisposedProperty = false;
		Hashtable htStyleProperties;
		Dictionary<object, SchedulerPrintStylePropertyValidationCallback> htStylePropertiesValidationCallbacks;
		protected internal SchedulerPrintStyle(bool registerProperties, bool baseStyle)
			: base(null, string.Empty) {
			this.htStyleProperties = new Hashtable();
			this.htStylePropertiesValidationCallbacks = new Dictionary<object, SchedulerPrintStylePropertyValidationCallback>();
			if (registerProperties) {
				RegisterProperties();
				SetPropertyValue(baseStyleProperty, baseStyle);
			}
			DisplayName = DefaultDisplayName;
		}
		string ICaptionSupport.Caption { get { return DisplayName; } }
		internal Hashtable HtStyleProperties { get { return htStyleProperties; } }
		public bool IsDisposed { get { return isDisposedProperty; } }
		#region properties and methods for UserInterfaceObject
		protected abstract string GetStyleBitmapName();
		int LoadStyleBitmapFromResource(string bitmapShortName) {
			string bigBitmapName = String.Format(bitmapNameFormatString, "big", bitmapShortName);
			string smallBitmapName = String.Format(bitmapNameFormatString, "small", bitmapShortName);
			Bitmap bigBitmap = ResourceImageHelper.CreateBitmapFromResources(bigBitmapName, Assembly.GetExecutingAssembly());
			Bitmap smallBitmap = ResourceImageHelper.CreateBitmapFromResources(smallBitmapName, Assembly.GetExecutingAssembly());
			bigBitmap.MakeTransparent(transparentColor);
			smallBitmap.MakeTransparent(transparentColor);
			int indexBig = LargeImageList.Images.Add(bigBitmap, transparentColor);
			int indexSmall = SmallImageList.Images.Add(smallBitmap, transparentColor);
			System.Diagnostics.Debug.Assert(indexBig == indexSmall);
			htImagesIndex[bitmapShortName] = indexBig;
			return indexBig;
		}
		internal int GetImageListIndex() {
			string bitmapShortName = GetStyleBitmapName();
			if (htImagesIndex[bitmapShortName] != null)
				return (int)htImagesIndex[bitmapShortName];
			return LoadStyleBitmapFromResource(bitmapShortName);
		}
		public virtual Bitmap CreateBitmap(int width, int height) {
			int index = GetImageListIndex();
			if (width >= largeImageSize.Width && height >= largeImageSize.Height)
				return (Bitmap)LargeImageList.Images[index];
			else
				return (Bitmap)SmallImageList.Images[index];
		}
		#endregion
		#region Kind property
		[Browsable(false)]
		public abstract SchedulerPrintStyleKind Kind { get; }
		#endregion
		#region PrintColorConverter
		static readonly object colorConverterProperty = new object();
		[Browsable(false)]
		[XtraSerializableProperty()]
		public PrintColorConverter ColorConverter {
			get {
				return (PrintColorConverter)GetPropertyValue(colorConverterProperty);
			}
			set {
				PrintColorConverter colorConverter = ColorConverter;
				if (Object.ReferenceEquals(colorConverter, value))
					return;
				SetPropertyValue(colorConverterProperty, value.Clone());
			}
		}
		bool ShouldSerializeColorConverter() {
			return !ColorConverter.Equals(PrintColorConverter.DefaultConverter);
		}
		protected bool XtraShouldSerializeColorConverter() {
			return !ColorConverter.Equals(PrintColorConverter.DefaultConverter);
		}
		void ResetColorConverter() {
			ColorConverter = PrintColorConverter.DefaultConverter;
		}
		#endregion
		#region BaseStyle property
		static readonly object baseStyleProperty = new object();
		[Browsable(false)]
		[DefaultValue(true)]
		[XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public bool BaseStyle {
			get { return (bool)GetPropertyValue(baseStyleProperty); }
			set {
				if (BaseStyle == value)
					return;
				SetPropertyValue(baseStyleProperty, value);
			}
		}
		#endregion
		#region AutoScaleHeadingsFont property
		static readonly object autoScaleHeadingsFontProperty = new object();
		[Browsable(false)]
		[DefaultValue(true)]
		[XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public bool AutoScaleHeadingsFont {
			get { return (bool)GetPropertyValue(autoScaleHeadingsFontProperty); }
			set { SetPropertyValue(autoScaleHeadingsFontProperty, value); }
		}
		#endregion
		#region HeadingsFont property
		static readonly object headingsFontProperty = new object();
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerPrintStyleHeadingsFont"),
#endif
XtraSerializableProperty()]
		public Font HeadingsFont {
			get { return (Font)GetPropertyValue(headingsFontProperty); }
			set {
				Font headingsFont = HeadingsFont;
				if (Object.ReferenceEquals(headingsFont, value))
					return;
				if (headingsFont != null)
					headingsFont.Dispose();
				SetPropertyValue(headingsFontProperty, value.Clone());
			}
		}
		bool ShouldSerializeHeadingsFont() {
			return !HeadingsFont.Equals(defaultHeadingsFont);
		}
		protected bool XtraShouldSerializeHeadingsFont() {
			return ShouldSerializeHeadingsFont();
		}
		void ResetHeadingsFont() {
			HeadingsFont = defaultHeadingsFont;
		}
		#endregion
		#region AppointmentFont property
		static readonly object appointmentFontProperty = new object();
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerPrintStyleAppointmentFont"),
#endif
XtraSerializableProperty()]
		public Font AppointmentFont {
			get { return (Font)GetPropertyValue(appointmentFontProperty); }
			set {
				Font appointmentFont = AppointmentFont;
				if (Object.ReferenceEquals(appointmentFont, value))
					return;
				if (appointmentFont != null)
					appointmentFont.Dispose();
				SetPropertyValue(appointmentFontProperty, value.Clone());
			}
		}
		bool ShouldSerializeAppointmentFont() {
			return !AppointmentFont.Equals(defaultAppointmentFont);
		}
		protected bool XtraShouldSerializeAppointmentFont() {
			return ShouldSerializeAppointmentFont();
		}
		void ResetAppointmentFont() {
			AppointmentFont = defaultAppointmentFont;
		}
		#endregion
		#region PageSettings property
		static readonly object pageSettingsProperty = new object();
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public PageSettings PageSettings {
			get {
				PageSettings pageSettings = (PageSettings)GetPropertyValue(pageSettingsProperty);
				if (pageSettings != null)
					return pageSettings;
				pageSettings = ClonePageSettings(defaultPageSettingsProvider.GetPageSettings());
				SetPropertyValue(pageSettingsProperty, pageSettings);
				return pageSettings;
			}
			set {
				SetPropertyValue(pageSettingsProperty, ClonePageSettings(value));
			}
		}
		#endregion
		internal static PageSettings ClonePageSettings(PageSettings sourcePage) {
			PageSettings result = (PageSettings)sourcePage.Clone();
			result.PrinterSettings = (PrinterSettings)result.PrinterSettings.Clone();
			return result;
		}
		protected internal override bool ShouldSerializeDisplayName() {
			return DisplayName != DefaultDisplayName;
		}
		protected internal override bool ShouldSerializeMenuCaption() {
			return MenuCaption != DefaultMenuCaption;
		}
		[Obsolete("Method is deprecated.", false)]
		protected internal virtual void ResetPageSettings() {
		}
		protected internal virtual void RegisterProperties() {
			RegisterProperty(colorConverterProperty, PrintColorConverter.DefaultConverter.Clone());
			RegisterProperty(baseStyleProperty, true);
			RegisterProperty(autoScaleHeadingsFontProperty, true);
			RegisterProperty(headingsFontProperty, defaultHeadingsFont.Clone());
			RegisterProperty(appointmentFontProperty, defaultAppointmentFont.Clone());
			RegisterProperty(pageSettingsProperty, null);
		}
		protected internal virtual void RegisterProperty(object property, object initialValue) {
			if (htStyleProperties[property] != null)
				Exceptions.ThrowArgumentException("property", property);
			SetPropertyValue(property, initialValue);
		}
		protected internal virtual void RegisterProperty(object property, SchedulerPrintStylePropertyValidationCallback validationCallback, object initialValue) {
			if (htStyleProperties[property] != null)
				Exceptions.ThrowArgumentException("property", property);
			RigisterValidationCallback(property, validationCallback);
			SetPropertyValue(property, initialValue);
		}
		protected internal virtual void RigisterValidationCallback(object property, SchedulerPrintStylePropertyValidationCallback validationCallback) {
#if DEBUGTEST
			if (this.htStylePropertiesValidationCallbacks.ContainsKey(property))
				Exceptions.ThrowArgumentException("property", property);
#endif
			this.htStylePropertiesValidationCallbacks[property] = validationCallback;
		}
		protected internal virtual object GetPropertyValue(object property) {
			object value = htStyleProperties[property];
			return value != NullObject ? value : null;
		}
		protected internal virtual void SetPropertyValue(object property, object newValue) {
			IDisposable disposable = htStyleProperties[property] as IDisposable;
			if (disposable != null)
				disposable.Dispose();
			if (this.htStylePropertiesValidationCallbacks.ContainsKey(property))
				this.htStylePropertiesValidationCallbacks[property](property, newValue);
			htStyleProperties[property] = newValue != null ? newValue : NullObject;
		}
		public void Reset() {
			ClearProperties();
			RegisterProperties();
		}
		internal void ClearProperties() {
			ICollection keys = htStyleProperties.Keys;
			foreach (object key in keys) {
				IDisposable disposable = htStyleProperties[key] as IDisposable;
				if (disposable != null)
					disposable.Dispose();
			}
			htStyleProperties.Clear();
		}
		protected void Dispose(bool disposing) {
			if (disposing) {
				if (htStyleProperties != null) {
					ClearProperties();
					htStyleProperties.Clear();
					htStyleProperties = null;
				}
				isDisposedProperty = true;
			}
		}
		public void Dispose() {
			Dispose(true);
		}
		public virtual object Clone() {
			SchedulerPrintStyle clone = CreateInstance();
			ICollection keys = htStyleProperties.Keys;
			foreach (object key in keys) {
				object value = htStyleProperties[key];
				ICloneable cloneable = value as ICloneable;
				if (cloneable != null)
					value = cloneable.Clone();
				clone.RegisterProperty(key, value);
			}
			clone.DisplayName = DisplayName;
			clone.MenuCaption = MenuCaption;
			return clone;
		}
		public SchedulerPrintStyle Clone(bool keepBase) {
			SchedulerPrintStyle clone = (SchedulerPrintStyle)Clone();
			clone.SetPropertyValue(baseStyleProperty, false);
			return clone;
		}
		protected internal abstract SchedulerPrintStyle CreateInstance();
		public override bool Equals(object obj) {
			SchedulerPrintStyle style = obj as SchedulerPrintStyle;
			if (style == null)
				return false;
			ICollection keys = htStyleProperties.Keys;
			Hashtable htComparedStyleProperties = style.HtStyleProperties;
			if (keys.Count != htComparedStyleProperties.Keys.Count)
				return false;
			bool checkPageSettingsEquals = false;
			foreach (object key in keys) {
				object value1 = htStyleProperties[key];
				object value2 = htComparedStyleProperties[key];
				if (key != pageSettingsProperty) {
					if (!value1.Equals(value2))
						return false;
				} else
					checkPageSettingsEquals = !((value1 == null && value2 != null) || (value1 == NullObject && value2 == NullObject));
			}
			if (checkPageSettingsEquals && !IsPageSettingsEquals(this.PageSettings, style.PageSettings))
				return false;
			return this.DisplayName == style.DisplayName && this.MenuCaption == style.MenuCaption;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		bool IsPageSettingsEquals(PageSettings pageSettings1, PageSettings pageSettings2) {
			return pageSettings1.Color == pageSettings2.Color && pageSettings1.Landscape == pageSettings2.Landscape &&
				pageSettings1.Margins == pageSettings2.Margins;
		}
		internal static XtraPropertyInfo[] SerializePageSettings(SchedulerPrintStyle style) {
			object pageSettingsValue = style.htStyleProperties[pageSettingsProperty];
			if (pageSettingsValue == null || pageSettingsValue == NullObject)
				return new XtraPropertyInfo[] { };
			List<XtraPropertyInfo> properties = new List<XtraPropertyInfo>();
			PageSettings pageSettings = (PageSettings)pageSettingsValue;
			properties.Add(new XtraPropertyInfo("Landscape", typeof(bool), pageSettings.Landscape));
			try {
				properties.Add(new XtraPropertyInfo("PaperSourceRawKind", typeof(int), pageSettings.PaperSource.RawKind));
			} catch {
			}
			try {
				PaperSize paperSize = pageSettings.PaperSize;
				properties.Add(new XtraPropertyInfo("PaperSizeRawKind", typeof(int), paperSize.RawKind));
				properties.Add(new XtraPropertyInfo("PaperSizeWidth", typeof(int), paperSize.Width));
				properties.Add(new XtraPropertyInfo("PaperSizeHeight", typeof(int), paperSize.Height));
				properties.Add(new XtraPropertyInfo("PaperSizeName", typeof(string), paperSize.PaperName));
			} catch {
			}
			Margins margins = pageSettings.Margins;
			properties.Add(new XtraPropertyInfo("LeftMargin", typeof(int), margins.Left));
			properties.Add(new XtraPropertyInfo("RightMargin", typeof(int), margins.Right));
			properties.Add(new XtraPropertyInfo("TopMargin", typeof(int), margins.Top));
			properties.Add(new XtraPropertyInfo("BottomMargin", typeof(int), margins.Bottom));
			XtraPropertyInfo pageSettingsPropertyInfo = new XtraPropertyInfo("PageSettings", typeof(PageSettings), String.Empty, true);
			pageSettingsPropertyInfo.ChildProperties.AddRange(properties.ToArray());
			return new XtraPropertyInfo[] { pageSettingsPropertyInfo };
		}
		internal static void DeserializePageSettings(IXtraPropertyCollection properties, SchedulerPrintStyle style) {
			BooleanConverter booleanConverter = new BooleanConverter();
			Int32Converter int32Converter = new Int32Converter();
			PaperSize paperSize = null;
			PaperSource paperSource = null;
			foreach (XtraPropertyInfo property in properties) {
				if (property.Name == "Landscape")
					style.PageSettings.Landscape = (bool)booleanConverter.ConvertFrom(property.Value);
				if (property.Name == "PaperSourceRawKind") {
					paperSource = paperSource ?? new PaperSource();
					paperSource.RawKind = (int)int32Converter.ConvertFrom(property.Value);
				}
				if (property.Name == "PaperSizeRawKind" && (paperSize == null || paperSize.Kind == PaperKind.Custom)) {
					paperSize = paperSize ?? new PaperSize();
					paperSize.RawKind = (int)int32Converter.ConvertFrom(property.Value);
					if (paperSize.Kind != PaperKind.Custom)
						paperSize = GetActualPaperSize(style.PageSettings.PrinterSettings.PaperSizes, paperSize);
				}
				if (property.Name == "PaperSizeWidth" && (paperSize == null || paperSize.Kind == PaperKind.Custom)) {
					paperSize = paperSize ?? new PaperSize();
					paperSize.Width = (int)int32Converter.ConvertFrom(property.Value);
				}
				if (property.Name == "PaperSizeHeight" && (paperSize == null || paperSize.Kind == PaperKind.Custom)) {
					paperSize = paperSize ?? new PaperSize();
					paperSize.Height = (int)int32Converter.ConvertFrom(property.Value);
				}
				if (property.Name == "PaperSizeName" && (paperSize == null || paperSize.Kind == PaperKind.Custom)) {
					paperSize = paperSize ?? new PaperSize();
					paperSize.PaperName = (string)property.Value;
				}
				if (property.Name == "LeftMargin")
					style.PageSettings.Margins.Left = (int)int32Converter.ConvertFrom(property.Value);
				if (property.Name == "RightMargin")
					style.PageSettings.Margins.Right = (int)int32Converter.ConvertFrom(property.Value);
				if (property.Name == "TopMargin")
					style.PageSettings.Margins.Top = (int)int32Converter.ConvertFrom(property.Value);
				if (property.Name == "BottomMargin")
					style.PageSettings.Margins.Bottom = (int)int32Converter.ConvertFrom(property.Value);
			}
			style.PageSettings.PaperSize = paperSize;
			style.PageSettings.PaperSource = paperSource;
		}
		static PaperSize GetActualPaperSize(PrinterSettings.PaperSizeCollection paperSizes, PaperSize paperSize) {
			int count = paperSizes.Count;
			for (int i = 0; i < count; i++)
				if (paperSizes[i].Kind == paperSize.Kind)
					return paperSizes[i];
			return paperSize;
		}
		internal static void DeserializeColorConverter(XtraPropertyInfo property, SchedulerPrintStyle style) {
			PrintColorConverterToStringConverter converter = new PrintColorConverterToStringConverter();
			if (converter.CanConvertFrom(property.Value.GetType()))
				style.ColorConverter = (PrintColorConverter)converter.ConvertFrom(property.Value);
		}
	}
}
