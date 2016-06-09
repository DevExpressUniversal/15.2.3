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
using System.Globalization;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using DevExpress.Utils;
using DevExpress.Data.Browsing;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.BarCode;
using DevExpress.XtraPrinting.BarCode.Native;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Native.Data;
using DevExpress.XtraReports.Parameters;
using System.Drawing.Design;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Localization;
using DevExpress.Data.Browsing.Design;
using DevExpress.XtraGauges.Core.Customization;
namespace DevExpress.XtraReports.Design {
	public interface IXRPictureBoxDesignerActionList3 {
		ImageSizeMode Sizing { get; }
	}
	public abstract class StringValuesConverter : TypeConverter {
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if(sourceType == typeof(string))
				return true;
			return base.CanConvertFrom(context, sourceType);
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if(destinationType == typeof(string))
				return true;
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType == typeof(string))
				return value as string;
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			if(value is string)
				return value as string;
			return base.ConvertFrom(context, culture, value);
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
			return false;
		}
	}
	public class NavigateUrlConverter : StringValuesConverter {
		#region static
		public static bool IsNavigateTarget(IComponent c) {
			XRControl conrtol = c as XRControl;
			return conrtol != null && conrtol.IsNavigateTarget;
		}
		static string[] GetValues(ITypeDescriptorContext context) {
			XRControl ctl = context.Instance as XRControl;
			if(ctl == null || ctl.Site == null || !Comparer.Equals(ctl.Target, "_self"))
				return new string[] { };
			ArrayList values = new ArrayList(new object[] { "" });
			foreach(IComponent c in ctl.Site.Container.Components) {
				if(!IsNavigateTarget(c) || Comparer.Equals(c.Site.Name, ctl.Site.Name))
					continue;
				values.Add(c.Site.Name);
			}
			return (string[])values.ToArray(typeof(string));
		}
		#endregion
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			return new StandardValuesCollection(GetValues(context));
		}
	}
	public class XtraReportConverter : ComponentConverter {
		public XtraReportConverter(Type type) : base (type) {
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if(destinationType == typeof(InstanceDescriptor)) {
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(value != null) {
				if(destinationType == typeof(string)) {
					return value.GetType().FullName;
				} else if(destinationType == typeof(InstanceDescriptor)) {
					System.Reflection.ConstructorInfo ci = value.GetType().GetConstructor(new Type[] { });
					return new InstanceDescriptor(ci, new object[] { });
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
			IDesignerHost host = context.GetService(typeof(IDesignerHost)) as IDesignerHost;
			if(host != null && host.RootComponent.Equals(context.Instance) == false) {
				return false;
			}
			return true;
		}
	}
	public class DataMemberTypeConverter : StringConverter {
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			IDataContainer dataContainer = context.Instance as IDataContainer;
			IDataContextService dataContextService = context.GetService(typeof(IDataContextService)) as IDataContextService;
			if(dataContainer == null || dataContextService == null)
				return base.ConvertTo(context, culture, value, destinationType);
			using(DataContext dataContext = dataContextService.CreateDataContext(new DataContextOptions(true, true)))
				return dataContext.GetDataMemberDisplayName(dataContainer.GetEffectiveDataSource(), String.Empty, (string)value);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			IDataContainer dataContainer = context.Instance as IDataContainer;
			IDataContextService dataContextService = context.GetService(typeof(IDataContextService)) as IDataContextService;
			if(dataContainer == null || !(value is string) || dataContextService == null)
				return base.ConvertFrom(context, culture, value);
			string dataMember = (string)value;
			if(String.IsNullOrEmpty(dataMember))
				return dataMember;
			using(XRDataContextBase dataContext = (XRDataContextBase)dataContextService.CreateDataContext(new DataContextOptions(true, true))) {
				string actualDataMember = dataContext.GetActualDataMember(dataContainer.GetEffectiveDataSource(), dataMember);
				return String.IsNullOrEmpty(actualDataMember) ? dataMember : actualDataMember;
			}
		}
	}
	public static class ContextHelper {
		public static bool UseDefaultPaperKind(ITypeDescriptorContext context) {
			if(context != null) {
				XtraReport report = context.Instance as XtraReport;
				return report != null && report.DefaultPrinterSettingsUsing.UsePaperKind;
			}
			return false;
		}
		public static bool UseDefaultLandscape(ITypeDescriptorContext context) {
			if(context != null) {
				XtraReport report = context.Instance as XtraReport;
				return report != null && report.DefaultPrinterSettingsUsing.UseLandscape;
			}
			return false;
		}
	}
	public class ReportPaperKindConverter : DevExpress.Utils.Design.PaperKindConverter {
		#region static
		static PaperKind[] GetValues(string printerName) {
			PrinterSettings sets = new PrinterSettings();
			sets.PrinterName = printerName;
			ArrayList values = new ArrayList();
			foreach(PaperSize paperSize in sets.PaperSizes) {
				if(!values.Contains(paperSize.Kind))
					values.Add(paperSize.Kind);
			}
			return (PaperKind[])values.ToArray(typeof(PaperKind));
		}
		#endregion
		public ReportPaperKindConverter(Type type) : base(type) {
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return sourceType != typeof(string) || !ContextHelper.UseDefaultPaperKind(context) ?
				base.CanConvertFrom(context, sourceType) : false;
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			return destinationType != typeof(string) || !ContextHelper.UseDefaultPaperKind(context) ?
				base.ConvertTo(context, culture, value, destinationType) :
				DesignSR.DefaultValueString;
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return !ContextHelper.UseDefaultPaperKind(context) ? base.GetStandardValuesSupported(context) : false;
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			XtraReport report = context.Instance as XtraReport;
			if(report != null && report.PrinterName.Length > 0) {
				InitializeInternal(context.PropertyDescriptor.PropertyType);
				PaperKind[] paperKinds = GetValues(report.PrinterName);
				if(paperKinds.Length > 0) {
					if(Comparer != null)
						Array.Sort(paperKinds, 0, paperKinds.Length, Comparer);
					return new StandardValuesCollection(paperKinds);
				}
			}
			return base.GetStandardValues(context);
		}
	}
	public class CanGrowCanShrinkConverter : DevExpress.Utils.Design.BooleanTypeConverter {
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			XRControl control = context.Instance as XRControl;
			if(control != null
			   && control.AnchorVertical != VerticalAnchorStyles.None
			   && control.AnchorVertical != VerticalAnchorStyles.Top) {
				return false;
			}
			return sourceType != typeof(bool) ? base.CanConvertFrom(context, sourceType) : false;
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			return destinationType != typeof(bool) ? base.ConvertTo(context, culture, value, destinationType) : value.ToString();
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			XRControl control = context.Instance as XRControl;
			if(control != null
				&& control.AnchorVertical != VerticalAnchorStyles.None
				&& control.AnchorVertical != VerticalAnchorStyles.Top) {
				return false;
			}
			return base.GetStandardValuesSupported(context);
		}
	}
	public class CellCanGrowCanShrinkConverter : CanGrowCanShrinkConverter {
		static bool NotChangableValue(ITypeDescriptorContext context) {
			XRTableCell cell = context.Instance as XRTableCell;
			if(context.Instance is object[])
				cell = ((object[])context.Instance)[0] as XRTableCell;
			if(cell == null)
				return false;
			XRTable table = cell.Row.Table;
			if(table != null
			   && table.AnchorVertical != VerticalAnchorStyles.None
			   && table.AnchorVertical != VerticalAnchorStyles.Top)
				return true;
			return false;
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if(NotChangableValue(context))
				return false;
			return sourceType != typeof(bool) ? base.CanConvertFrom(context, sourceType) : false;
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return NotChangableValue(context) ? false : base.GetStandardValuesSupported(context);
		}
	}
	public class PageHeightConverter : Int32Converter {
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			XtraReport report = context.Instance as XtraReport;
			return sourceType != typeof(string) || !ContextHelper.UseDefaultPaperKind(context)
				&& (report != null && report.PaperKind == PaperKind.Custom && report.RollPaper == false) ?
				base.CanConvertFrom(context, sourceType) : false;
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			return destinationType != typeof(string) || !ContextHelper.UseDefaultPaperKind(context) ?
				base.ConvertTo(context, culture, value, destinationType) :
				DesignSR.DefaultValueString;
		}
	}
	public class PageWidthConverter : Int32Converter {
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			XtraReport report = context.Instance as XtraReport;
			return sourceType != typeof(string) || !ContextHelper.UseDefaultPaperKind(context)
				&& (report != null && report.PaperKind == PaperKind.Custom) ?
				base.CanConvertFrom(context, sourceType) : false;
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			return destinationType != typeof(string) || !ContextHelper.UseDefaultPaperKind(context) ?
				base.ConvertTo(context, culture, value, destinationType) :
				DesignSR.DefaultValueString;
		}
	}
	public class LandscapeConverter : DevExpress.Utils.Design.BooleanTypeConverter {
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			return destinationType != typeof(string) || !ContextHelper.UseDefaultLandscape(context) ?
				base.ConvertTo(context, culture, value, destinationType) :
				DesignSR.DefaultValueString;
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return !ContextHelper.UseDefaultLandscape(context) ? base.GetStandardValuesSupported(context) : false;
		}
	}
	public class PaperNameConverter : StringValuesConverter {
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			XtraReport report = context.Instance as XtraReport;
			if(report != null && report.PaperKind != PaperKind.Custom)
				return false;
			return sourceType != typeof(string) || !ContextHelper.UseDefaultPaperKind(context) ?
				base.CanConvertFrom(context, sourceType) : false;
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			return destinationType != typeof(string) || !ContextHelper.UseDefaultPaperKind(context) ?
				base.ConvertTo(context, culture, value, destinationType) :
				DesignSR.DefaultValueString;
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			XtraReport report = context.Instance as XtraReport;
			if(report != null && report.PaperKind != PaperKind.Custom)
				return false;
			return !ContextHelper.UseDefaultPaperKind(context) ? base.GetStandardValuesSupported(context) : false;
		}
	}
	public class PrinterNameConverter : StringValuesConverter {
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			return new StandardValuesCollection(PrinterSettings.InstalledPrinters);
		}
	}
	public class StartBandConverter : ReferenceConverter {
		public StartBandConverter()
			: base(typeof(Band)) {
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			StandardValuesCollection standardValues = base.GetStandardValues(context);
			ArrayList values = new ArrayList();
			XRCrossBandControl cbControl = (XRCrossBandControl)context.Instance;
			foreach(Band band in standardValues) {
				if(band is XtraReportBase || band == null || !cbControl.AreValidBandIndexes(band, cbControl.EndBand))
					continue;
				values.Add(band);
			}
			return new StandardValuesCollection(values);
		}
	}
	public class EndBandConverter : ReferenceConverter {
		public EndBandConverter()
			: base(typeof(Band)) {
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			StandardValuesCollection standardValues = base.GetStandardValues(context);
			ArrayList values = new ArrayList();
			XRCrossBandControl cbControl = (XRCrossBandControl)context.Instance;
			foreach(Band band in standardValues) {
				if(band is XtraReportBase || band == null || !cbControl.AreValidBandIndexes(cbControl.StartBand, band))
					continue;
				values.Add(band);
			}
			return new StandardValuesCollection(values);
		}
	}
	public class TargetConverter : StringConverter {
		static string[] targetValues = new string[] { "_blank", "_parent", "_search", "_self", "_top" };
		TypeConverter.StandardValuesCollection values;
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			if(this.values == null) {
				this.values = new TypeConverter.StandardValuesCollection(targetValues);
			}
			return this.values;
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
			return false;
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
	}
	public class XRControlReferenceConverter : ReferenceConverter {
		public XRControlReferenceConverter() : base(typeof(XRControl)) {
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return false;
		}
	}
	public class DataAdapterConverter : ReferenceConverter {
		public DataAdapterConverter() : base(typeof(IComponent)) {
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			StandardValuesCollection objects = base.GetStandardValues(context);
			ArrayList adapters = new ArrayList();
			foreach(object obj in objects) {
				if(DevExpress.Data.Native.BindingHelper.IsDataAdapter(obj))
					adapters.Add(obj);
			}
			return new StandardValuesCollection(adapters);
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
			return true;
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType == typeof(string) && value == null)
				return PreviewLocalizer.GetString(PreviewStringId.NoneString);
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			string str = value as string;
			if(str != null && str == PreviewLocalizer.GetString(PreviewStringId.NoneString))
				return null;
			return base.ConvertFrom(context, culture, value);
		}
	}
	public abstract class XRSummaryConverterBase : LocalizableObjectConverter {
		protected abstract string SummaryToString(object value);
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if(destinationType == typeof(InstanceDescriptor) || destinationType == typeof(string))
				return true;
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType == typeof(string)) {
				string result = SummaryToString(value);
				if(!string.IsNullOrEmpty(result))
					return result;
			}
			else if (destinationType == typeof(InstanceDescriptor)) {
				System.Reflection.ConstructorInfo ci = value.GetType().GetConstructor(new Type[] { });
				return new InstanceDescriptor(ci, new object[] { }, false);
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
	public class XRSummaryConverter : XRSummaryConverterBase {
		protected override string SummaryToString(object value) {
			XRSummary summary = value as XRSummary;
			if(summary == null)
				return null;
			string format =
				summary.Running != SummaryRunning.None && !string.IsNullOrEmpty(summary.FormatString) ? "{0}, {1}, {2}" :
				summary.Running != SummaryRunning.None ? "{0}, {1}" :
				"{0}";
			return string.Format(format, ValueToString(summary.Running), ValueToString(summary.Func), summary.FormatString);
		}
		static string ValueToString(object value) {
			TypeConverter converter = TypeDescriptor.GetConverter(value);
			return converter != null ? converter.ConvertToString(new XRStubTypeDescriptorContext(), value) :
				value != null ? value.ToString() :
				string.Empty;
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(XRSummary), attributes);
			return props.Sort(new string[] { "Running", "Func" });
		}
	}
	public class XRGroupSortingSummaryConverter : XRSummaryConverterBase {
		protected override string SummaryToString(object value) {
			XRGroupSortingSummary summary = value as XRGroupSortingSummary;
			if(summary == null)
				return null;
			string s = string.Empty;
			if(summary.Enabled) {
				s = summary.Function.ToString();
				if(!string.IsNullOrEmpty(summary.FieldName))
					s += String.Format(", {0}", summary.FieldName);
				s += String.Format(", {0}", summary.SortOrder);
			}
			return s;
		}
	}
	public class XRMarginsConverter : MarginsConverter {
		#region static
		static protected bool CanProceedBaseMethod(ITypeDescriptorContext context) {
			if(context == null)
				return true;
			XtraReport report = context.Instance as XtraReport;
			return report == null || !report.DefaultPrinterSettingsUsing.UseMargins;
		}
		#endregion
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(Margins), attributes);
			List<PropertyDescriptor> props = new List<PropertyDescriptor>();
			foreach(PropertyDescriptor propertyDescriptor in properties) {
				props.Add(DevExpress.Utils.Design.TypeDescriptorHelper.CreateProperty(typeof(Margins), propertyDescriptor, DevExpress.Utils.Design.AttributeHelper.GetPropertyAttributes(typeof(ResFinder), propertyDescriptor)));
			}
			PropertyDescriptorCollection result = new PropertyDescriptorCollection(props.ToArray());
			return result;
		}
		public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
			return CanProceedBaseMethod(context) ? base.GetPropertiesSupported(context) : false;
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return sourceType != typeof(string) || CanProceedBaseMethod(context) ?
				base.CanConvertFrom(context, sourceType) : false;
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			if(value is string) {
				string text = ((string)value).Trim();
				if(text.Length == 0)
					return null;
				if(culture == null)
					culture = CultureInfo.CurrentCulture;
				char separator = culture.TextInfo.ListSeparator[0];
				string[] tokens = text.Split(new char[] { separator });
				int[] values = new int[tokens.Length];
				TypeConverter intConv = TypeDescriptor.GetConverter(typeof(int));
				for(int i = 0; i < values.Length; i++) {
					values[i] = (int)intConv.ConvertFromString(context, culture, tokens[i]);
				}
				if(values.Length != 4) {
					string s = String.Format("'{0}'{1}'{2}'", value.ToString(), DesignSR.InvalidArgument, "bottom, left, right, top");
					throw new ArgumentException(s);
				}
				return new Margins(values[1], values[2], values[3], values[0]);
			}
			return base.ConvertFrom(context, culture, value);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType == null) {
				throw new ArgumentNullException("destinationType");
			}
			Margins margins = value as Margins;
			if(destinationType == typeof(string) && margins != null) {
				if(!CanProceedBaseMethod(context))
					return DesignSR.DefaultValueString;
				if(culture == null)
					culture = CultureInfo.CurrentCulture;
				string separator = culture.TextInfo.ListSeparator + " ";
				TypeConverter intConv = TypeDescriptor.GetConverter(typeof(int));
				string[] args = new string[4];
				int nArg = 0;
				args[nArg++] = intConv.ConvertToString(context, culture, margins.Bottom);
				args[nArg++] = intConv.ConvertToString(context, culture, margins.Left);
				args[nArg++] = intConv.ConvertToString(context, culture, margins.Right);
				args[nArg++] = intConv.ConvertToString(context, culture, margins.Top);
				return string.Join(separator, args);
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
	public class XRControlStylesConverter : ExpandableObjectConverter {
		public static XRControl.XRControlStyles[] GetStyles(ITypeDescriptorContext context) {
			XRControl.XRControlStyles styles = context.Instance as XRControl.XRControlStyles;
			if(styles != null) {
				return new XRControl.XRControlStyles[] { styles };
			}
			Array stylesArray = context.Instance as Array;
			if(stylesArray != null) {
				return GetStylesFromArray(stylesArray);
			}
			return null;
		}
		static XRControl.XRControlStyles[] GetStylesFromArray(Array stylesArray) {
			int count = stylesArray.GetLength(0);
			XRControl.XRControlStyles[] styles = new XRControl.XRControlStyles[count];
			int i = 0;
			foreach(object obj in stylesArray) {
				styles[i] = (XRControl.XRControlStyles)obj;
				i++;
			}
			return styles;
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if(destinationType == typeof(string))
				return true;
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType == typeof(string) && context != null && context.Instance != null)
				return DevExpress.Utils.Design.CollectionTypeConverter.DisplayName;
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
	public class XRControlStyleConverter : ExpandableObjectConverter {
		#region inner class
		class Wrapper : PropertyDescriptorWrapper {
			public override bool IsReadOnly { get { return true; } }
			public Wrapper(PropertyDescriptor oldPropertyDescriptor) : base(oldPropertyDescriptor) { }
			public override bool CanResetValue(object component) { return false; }
		}
		#endregion
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if(destinationType == typeof(string))
				return true;
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
			if(destinationType == typeof(string) && context != null && context.Instance != null)
				return value != null ? ((XRControlStyle)value).Name : PreviewLocalizer.GetString(PreviewStringId.NoneString);
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection collection = base.GetProperties(context, value, attributes);
			XRControlStyle style = value as XRControlStyle;
			if(style != null && style.Site == null) {
				List<PropertyDescriptor> list = new List<PropertyDescriptor>();
				foreach(PropertyDescriptor property in collection)
					list.Add(new Wrapper(property));
				collection = new PropertyDescriptorCollection(list.ToArray());
			}
			return collection;
		}
	}
	public class BarCodeModuleConverter : SingleConverter {
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return sourceType == typeof(string) && GetAutoModule(context) ? false : base.CanConvertFrom(context, sourceType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			return destinationType == typeof(string) && GetAutoModule(context) ? DesignSR.AutoValueString : base.ConvertTo(context, culture, value, destinationType);
		}
		protected virtual bool GetAutoModule(ITypeDescriptorContext context) {
			XRBarCode barcode = context.Instance as XRBarCode;
			return barcode != null && barcode.AutoModule;
		}
	}
	public class BarCodeDataConverter : ArrayConverter {
		public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
			XRBarCode barCode = context.Instance as XRBarCode;
			if(barCode != null)
				return barCode.Symbology is BarCode2DGenerator;
			return base.GetPropertiesSupported(context);
		}
	}
	public class WinControlPrintModeConverter : DevExpress.Utils.Design.EnumTypeConverter {
		public WinControlPrintModeConverter(Type type)
			: base(type) {
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			WinControlContainer container = context.Instance as WinControlContainer;
			if(container != null && !container.HasLink)
				return false;
			return base.GetStandardValuesSupported(context);
		}
	}
	#region XRControlStyle converters
	static class XRControlStyleConverterHelper {
		internal delegate object ConvertToFunc(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType);
		internal static bool IsSet(ITypeDescriptorContext context, StyleProperty property) {
			if(context == null || context.Instance == null)
				return true;
			if(context.Instance is object[])
				foreach(XRControlStyle style in (object[])context.Instance)
					if(BrickStyle.PropertyIsSet(style, property))
						return true;
			return BrickStyle.PropertyIsSet((XRControlStyle)context.Instance, property);
		}
		internal static bool NotSet(ITypeDescriptorContext context, StyleProperty property) {
			bool result = context != null && context.Instance != null;
			if(context.Instance is object[])
				foreach(XRControlStyle style in (object[])context.Instance)
					result = result && !BrickStyle.PropertyIsSet(style, property);
			else result = result && !BrickStyle.PropertyIsSet((XRControlStyle)context.Instance, property);
			return result;
		}
		internal static object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType, ConvertToFunc baseConvertTo, StyleProperty property) {
			return destinationType != typeof(string) || IsSet(context, property) ?
				baseConvertTo(context, culture, value, destinationType) :
				DesignSR.PropertyGridNotSet;
		}
		internal static StyleProperty ToStyleProperty(PropertyDescriptor propertyDescriptor) {
			string propertyName = propertyDescriptor.Name;
			return (propertyName == XRComponentPropertyNames.BackColor) ?
				StyleProperty.BackColor :
				(propertyName == XRComponentPropertyNames.BorderColor) ?
					StyleProperty.BorderColor :
					StyleProperty.ForeColor;
		}
	}
	public class XRControlStylePaddingConverter : DevExpress.XtraPrinting.Design.PaddingInfoTypeConverter {
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			return XRControlStyleConverterHelper.ConvertTo(context, culture, value, destinationType, base.ConvertTo, StyleProperty.Padding);
		}
	}
	public class XRControlStyleBorderWidthConverter : SingleConverter {
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			return XRControlStyleConverterHelper.ConvertTo(context, culture, value, destinationType, base.ConvertTo, StyleProperty.BorderWidth);
		}
	}
	public class XRControlStyleFontConverter : DevExpress.Utils.Design.FontTypeConverter {
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			return XRControlStyleConverterHelper.ConvertTo(context, culture, value, destinationType, base.ConvertTo, StyleProperty.Font);
		}
		public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
			return XRControlStyleConverterHelper.NotSet(context, StyleProperty.Font) ?
				false : base.GetPropertiesSupported(context);
		}
	}
	public class XRControlStyleColorConverter : ColorConverter {
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			return XRControlStyleConverterHelper.ConvertTo(context, culture, value, destinationType, base.ConvertTo, XRControlStyleConverterHelper.ToStyleProperty(context.PropertyDescriptor));
		}
	}
	public class XRControlStyleBordersConverter : DevExpress.Utils.Design.EnumTypeConverter {
		public XRControlStyleBordersConverter(Type type) : base(type) { }
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			return XRControlStyleConverterHelper.ConvertTo(context, culture, value, destinationType, base.ConvertTo, StyleProperty.Borders);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			if(context != null && context.Instance != null && value.Equals(DesignSR.PropertyGridNotSet)) {
				((XRControlStyle)context.Instance).ResetBorders();
				return null;
			}
			return base.ConvertFrom(context, culture, value);
		}
	}
	public class XRControlStyleTextAlignmentConverter : DevExpress.Utils.Design.EnumTypeConverter {
		public XRControlStyleTextAlignmentConverter(Type type) : base(type) { }
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			return XRControlStyleConverterHelper.ConvertTo(context, culture, value, destinationType, base.ConvertTo, StyleProperty.TextAlignment);
		}
	}
	public class XRControlStyleBorderDashStyleConverter : DevExpress.Utils.Design.EnumTypeConverter {
		public XRControlStyleBorderDashStyleConverter(Type type) : base(type) { }
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			return XRControlStyleConverterHelper.ConvertTo(context, culture, value, destinationType, base.ConvertTo, StyleProperty.BorderDashStyle);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			if(context != null && context.Instance != null && value.Equals(DesignSR.PropertyGridNotSet)) {
				((XRControlStyle)context.Instance).ResetBorderDashStyle();
				return null;
			}
			return base.ConvertFrom(context, culture, value);
		}
	}
	public class XRFormattingVisibleConverter : DevExpress.Utils.Design.DefaultBooleanConverter {
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			return destinationType != typeof(string) || (DefaultBoolean)value != DefaultBoolean.Default ?
				base.ConvertTo(context, culture, value, destinationType) : DesignSR.PropertyGridNotSet;
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			if(value is string && ((string)value) == DesignSR.PropertyGridNotSet)
				return base.ConvertFrom(context, culture, DefaultBoolean.Default.ToString());
			return base.ConvertFrom(context, culture, value);
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			StandardValuesCollection values = base.GetStandardValues(context);
			List<DefaultBoolean> resultValues = new List<DefaultBoolean>();
			foreach(DefaultBoolean value in values)
				if(value != DefaultBoolean.Default)
					resultValues.Add(value);
			return new StandardValuesCollection(resultValues);
		}
	}
	public class XRControlNameDependedConverter : StringConverter {
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			string name = context.Instance is XRControl ? (context.Instance as XRControl).Name : null;
			return (destinationType != typeof(string)
				|| !String.IsNullOrEmpty((string)value)
				|| String.IsNullOrEmpty(name)) ?
				base.ConvertTo(context, culture, value, destinationType) : name;
		}
	}
	#endregion
	public class XtraReportConditionalFormattingTypeConverter : ExpandableObjectConverter {
		string name;
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			name = "";
			if(context.Instance is FormattingRule) {
				Formatting formatting = (context.Instance as FormattingRule).Formatting;
				if(formatting.IsSetBackColor)
					AddName(TypeDescriptor.GetProperties(typeof(Formatting))["BackColor"].DisplayName + " = " + formatting.BackColor.Name);
				if(formatting.IsSetBorderColor)
					AddName(TypeDescriptor.GetProperties(typeof(Formatting))["BorderColor"].DisplayName + " = " + formatting.BorderColor.Name);
				if(formatting.IsSetBorders)
					AddName(TypeDescriptor.GetProperties(typeof(Formatting))["Borders"].DisplayName + " = " + formatting.Borders.ToString());
				if(formatting.IsSetBorderDashStyle)
					AddName(TypeDescriptor.GetProperties(typeof(Formatting))["BorderDashStyle"].DisplayName + " = " + formatting.BorderDashStyle.ToString());
				if(formatting.IsSetBorderWidth)
					AddName(TypeDescriptor.GetProperties(typeof(Formatting))["BorderWidth"].DisplayName + " = " + formatting.BorderWidth.ToString());
				if(formatting.IsSetFont)
					AddName(TypeDescriptor.GetProperties(typeof(Formatting))["Font"].DisplayName + " = " + formatting.Font.ToString());
				if(formatting.IsSetForeColor)
					AddName(TypeDescriptor.GetProperties(typeof(Formatting))["ForeColor"].DisplayName + " = " + formatting.ForeColor.Name);
				if(formatting.IsSetPadding)
					AddName(TypeDescriptor.GetProperties(typeof(Formatting))["Padding"].DisplayName + " = (" + TypeDescriptor.GetProperties(typeof(Formatting))["Padding"].Converter.ConvertTo(formatting.Padding, typeof(string)) + ")");
				if(formatting.IsSetTextAlignment)
					AddName(TypeDescriptor.GetProperties(typeof(Formatting))["TextAlignment"].DisplayName + " = " + formatting.TextAlignment.ToString());
				if(formatting.Visible != DefaultBoolean.Default)
					AddName(TypeDescriptor.GetProperties(typeof(Formatting))["Visible"].DisplayName + " = " + formatting.Visible.ToString());
			}
			return name;
		}
		void AddName(string addedString) {
			name += (string.IsNullOrEmpty(name) ? string.Empty : ", ");
			name += addedString;
		}
	}
	public class RunningBandTypeConverter : ReferenceConverter {
		static bool IsValidRunningBand(Band band) {
			return band == null || band is GroupHeaderBand || band is DetailReportBand;
		}
		public RunningBandTypeConverter()
			: base(typeof(Band)) {
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			StandardValuesCollection standardValues = base.GetStandardValues(context);
			ArrayList values = new ArrayList();
			foreach(Band band in standardValues) {
				if(IsValidRunningBand(band))
					values.Add(band);
			}
			return new StandardValuesCollection(values);
		}
	}
	public class XRCrossBandBoxBorderDashStyleConverter : DevExpress.Utils.Design.EnumTypeConverter {
		public XRCrossBandBoxBorderDashStyleConverter()
			: base(typeof(BorderDashStyle)) {
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			List<BorderDashStyle> newValues = new List<BorderDashStyle>();
			foreach(BorderDashStyle value in base.GetStandardValues(context)) {
				if(value != BorderDashStyle.Double)
					newValues.Add(value);
			}
			return new StandardValuesCollection(newValues);
		}
	}
	public class ProcessNullValuesTypeConverter : DevExpress.Utils.Design.EnumTypeConverter {
		public ProcessNullValuesTypeConverter() : base(typeof(ValueSuppressType)) { }
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			List<ValueSuppressType> newValues = new List<ValueSuppressType>() { 
				ValueSuppressType.Leave,
				ValueSuppressType.Suppress,
				ValueSuppressType.SuppressAndShrink
			};
			return new StandardValuesCollection(newValues);
		}
	}
	public class GlyphAlignmentConverter : DevExpress.Utils.Design.EnumTypeConverter {
		public GlyphAlignmentConverter() : base(typeof(HorzAlignment)) { }
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			List<HorzAlignment> newValues = new List<HorzAlignment>() { 
				HorzAlignment.Near,
				HorzAlignment.Center,
				HorzAlignment.Far
			};
			return new StandardValuesCollection(newValues);
		}
	}
	public class DashboardGaugeStyleConverter : DevExpress.Utils.Design.EnumTypeConverter {
		public DashboardGaugeStyleConverter() : base(typeof(DashboardGaugeStyle)) { }
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			if(context != null && context.Instance is XRGauge) {
				XRGauge gauge = (XRGauge)context.Instance;
				return gauge.ViewType == DashboardGaugeType.Linear ?
					new StandardValuesCollection(DashboardGaugeStyleExtension.LinearStyles) :
					new StandardValuesCollection(DashboardGaugeStyleExtension.CircularStyles);
			}
			return base.GetStandardValues(context);
		}
	}
	public class ImageSizingTypeConverter : DevExpress.Utils.Design.EnumTypeConverter {
		public ImageSizingTypeConverter() : base(typeof(ImageSizeMode)) { }
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			List<ImageSizeMode> newValues = new List<ImageSizeMode>() { 
				ImageSizeMode.Normal,
				ImageSizeMode.StretchImage,
				ImageSizeMode.AutoSize,
				ImageSizeMode.ZoomImage,
				ImageSizeMode.Squeeze,
				ImageSizeMode.Tile	  
			};
			return new StandardValuesCollection(newValues);
		}
	}
	public class DrawWatermarkConverter : DevExpress.Utils.Design.BooleanTypeConverter {
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			XtraReport report = context.Instance as XtraReport;
			return (report != null && report.Watermark != null && (report.Watermark.Image != null || !string.IsNullOrEmpty(report.Watermark.Text))) ?
				base.GetStandardValuesSupported(context) : false;
		}
	}
	public class ImageAlignmentTypeConverter : DevExpress.Utils.Design.EnumTypeConverter {
		public ImageAlignmentTypeConverter() : base(typeof(ImageAlignment)) { }
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if(context == null)
				return false;
			var actList = context.Instance as IXRPictureBoxDesignerActionList3;
			if(actList != null)
				return actList.Sizing == ImageSizeMode.Normal || actList.Sizing == ImageSizeMode.Squeeze || actList.Sizing == ImageSizeMode.ZoomImage;
			XRPictureBox pictureBox = context.Instance as XRPictureBox;
			return (pictureBox != null && (pictureBox.Sizing != ImageSizeMode.Normal && pictureBox.Sizing != ImageSizeMode.Squeeze && pictureBox.Sizing != ImageSizeMode.ZoomImage)) ?
			  false : base.CanConvertFrom(context, sourceType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			XRPictureBox pictureBox = context.Instance as XRPictureBox;
			if(pictureBox != null) {
				if(pictureBox.Sizing != ImageSizeMode.Normal && pictureBox.Sizing != ImageSizeMode.Squeeze && pictureBox.Sizing != ImageSizeMode.ZoomImage)
					return DesignSR.NoneValueString;
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			var actList = context.Instance as IXRPictureBoxDesignerActionList3;
			if(actList != null)
				return actList.Sizing == ImageSizeMode.Normal || actList.Sizing == ImageSizeMode.Squeeze || actList.Sizing == ImageSizeMode.ZoomImage;
			XRPictureBox pictureBox = context.Instance as XRPictureBox;
			return (pictureBox != null && (pictureBox.Sizing != ImageSizeMode.Normal && pictureBox.Sizing != ImageSizeMode.Squeeze && pictureBox.Sizing != ImageSizeMode.ZoomImage)) ?
				false : base.GetStandardValuesSupported(context);
		}
	}
	public class XRTableOfContentsLevelBaseConverter : LocalizableObjectConverter {
		#region inner class
		class StylePropertyWrapper : PropertyDescriptorWrapper {
			Func<object> getValue;
			public StylePropertyWrapper(PropertyDescriptor oldPropertyDescriptor, Func<object> getValue)
				: base(oldPropertyDescriptor) {
					this.getValue = getValue;
			}
			public override object GetValue(object component) {
				return getValue();
			}
		}
		#endregion
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection properties = base.GetProperties(context, value, attributes);
			XRTableOfContentsLevelBase levelBase = value as XRTableOfContentsLevelBase;
			if(levelBase == null || !(levelBase.Parent != null && levelBase.Parent is XRTableOfContents))
				return properties;
			XRTableOfContents TOC = levelBase.Parent as XRTableOfContents;
			XRControlStyle levelStyle = levelBase.Style;
			List<PropertyDescriptor> props = new List<PropertyDescriptor>();
			foreach(PropertyDescriptor propertyDescriptor in properties) {
				switch(propertyDescriptor.Name) {
					case "Font":
						if(levelStyle.IsSetFont)
							props.Add(propertyDescriptor);
						else
							props.Add(new StylePropertyWrapper(propertyDescriptor, () => TOC.GetEffectiveFont()));
						break;
					case "ForeColor":
						if(levelStyle.IsSetForeColor)
							props.Add(propertyDescriptor);
						else
							props.Add(new StylePropertyWrapper(propertyDescriptor, () => TOC.GetEffectiveForeColor()));
						break;
					case "BackColor":
						if(levelStyle.IsSetBackColor)
							props.Add(propertyDescriptor);
						else
							props.Add(new StylePropertyWrapper(propertyDescriptor, () => TOC.GetEffectiveBackColor()));
						break;
					case "Padding":
						if(levelStyle.IsSetPadding)
							props.Add(propertyDescriptor);
						else
							props.Add(new StylePropertyWrapper(propertyDescriptor, () => TOC.GetEffectivePadding()));
						break;
					case "TextAlignment":
						if(levelStyle.IsSetTextAlignment)
							props.Add(propertyDescriptor);
						else
							props.Add(new StylePropertyWrapper(propertyDescriptor, () => TOC.GetEffectiveTextAlignment()));
						break;
					default : 
						props.Add(propertyDescriptor);
						break;
				} 
			}
			PropertyDescriptorCollection result = new PropertyDescriptorCollection(props.ToArray());
			return result;
		}
	}
	public class ProcessDuplicatesModeTypeConverter : DevExpress.Utils.Design.EnumTypeConverter {
		public ProcessDuplicatesModeTypeConverter() : base(typeof(ProcessDuplicatesMode)) { }
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			XRControl control = context.Instance as XRControl;
			return (control != null && control.HasChildren) ? 
				false : base.CanConvertFrom(context, sourceType);
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			XRControl control = context.Instance as XRControl;
			return (control != null && control.HasChildren) ?
				false : base.GetStandardValuesSupported(context);
		}
	}
}
