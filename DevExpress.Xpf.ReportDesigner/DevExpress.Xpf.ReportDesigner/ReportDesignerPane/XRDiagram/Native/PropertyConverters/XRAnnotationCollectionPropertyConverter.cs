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

using DevExpress.Mvvm.Native;
using DevExpress.Xpo.Helpers;
using DevExpress.XtraCharts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using DevExpress.Utils.Design;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UI.PivotGrid;
using DevExpress.Xpf.Reports.UserDesigner.XRDiagram;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.Design;
using DevExpress.Xpf.Diagram;
namespace DevExpress.Xpf.Reports.UserDesigner.Native.PropertyConverters {
	public class XRAnnotationCollectionPropertyConverter : TypeConverter, IXRPropertyConverter {
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
			if(destinationType == typeof(string))
				return "(Collection)";
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public Type PropertyType { get { return typeof(AnnotationCollection); } }
		public Type VirtualPropertyType { get { return typeof(IList<DiagramItem>); } }
		public object Convert(object value, object owner) {
			var chartDiagramItem = (XRChartDiagramItem)owner;
			return chartDiagramItem.Items;
		}
		public object ConvertBack(object value) {
			throw new NotSupportedException();
		}
	}
	public class XRDataFilterCollectionPropertyConverter : TypeConverter, IXRPropertyConverter {
		public Type PropertyType { get { return typeof(DataFilterCollection); } }
		public Type VirtualPropertyType { get { return typeof(IList<DataFilter>); } }
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
			if(destinationType == typeof(string))
				return "(Collection)"; 
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public object Convert(object value, object owner) {
			return ListAdapter<DataFilter>.FromObjectList((DataFilterCollection)value);
		}
		public object ConvertBack(object value) {
			throw new NotSupportedException();
		}
	}
	public class XRChartTitleCollectionPropertyConverter : TypeConverter, IXRPropertyConverter {
		public Type PropertyType { get { return typeof(ChartTitleCollection); } }
		public Type VirtualPropertyType { get { return typeof(IList<DiagramItem>); } }
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
			if(destinationType == typeof(string))
				return "(Collection)"; 
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public object Convert(object value, object owner) {
			var chartDiagramItem = (XRChartDiagramItem)owner;
			return chartDiagramItem.Items;
		}
		public object ConvertBack(object value) {
			return null;
		}
	}
	public class XRPivotGridFieldCollectionPropertyConverter : TypeConverter, IXRPropertyConverter {
		public Type PropertyType { get { return typeof(XRPivotGridFieldCollection); } }
		public Type VirtualPropertyType { get { return typeof(IList<XRPivotGridField>); } }
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
			if(destinationType == typeof(string))
				return "(Collection)"; 
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public object Convert(object value, object owner) {
			return ListAdapter<XRPivotGridField>.FromObjectList((XRPivotGridFieldCollection)value);
		}
		public object ConvertBack(object value) {
			throw new NotSupportedException();
		}
	}
	public class XRPivotGridCustomTotalCollectionPropertyConverter : TypeConverter, IXRPropertyConverter {
		public Type PropertyType { get { return typeof(XRPivotGridCustomTotalCollection); } }
		public Type VirtualPropertyType { get { return typeof(IList<XRPivotGridCustomTotal>); } }
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
			if(destinationType == typeof(string))
				return "(Collection)"; 
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public object Convert(object value, object owner) {
			return ListAdapter<XRPivotGridCustomTotal>.FromObjectList((XRPivotGridCustomTotalCollection)value);
		}
		public object ConvertBack(object value) {
			throw new NotSupportedException();
		}
	}
	public class ReportParameterCollectionPropertyConverter : TypeConverter, IXRPropertyConverter {
		public Type PropertyType { get { return typeof(ParameterCollection); } }
		public Type VirtualPropertyType { get { return typeof(IList<DiagramItem>); } }
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
			if(destinationType == typeof(string))
				return "(Collection)"; 
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public object Convert(object value, object owner) {
			return ((XRDiagramRoot)owner).Items;
		}
		public object ConvertBack(object value) {
			throw new NotSupportedException();
		}
	}
	public class CalculatedFieldCollectionPropertyConverter : TypeConverter, IXRPropertyConverter {
		public Type PropertyType { get { return typeof(CalculatedFieldCollection); } }
		public Type VirtualPropertyType { get { return typeof(IList<DevExpress.Xpf.Diagram.DiagramItem>); } }
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
			if(destinationType == typeof(string))
				return "(Collection)"; 
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public object Convert(object value, object owner) {
			return ((XRDiagramRoot)owner).Items;
		}
		public object ConvertBack(object value) {
			throw new NotSupportedException();
		}
	}
	public class FormattingSheetCollectionPropertyConverter : TypeConverter, IXRPropertyConverter {
		public Type PropertyType { get { return typeof(FormattingRuleSheet); } }
		public Type VirtualPropertyType { get { return typeof(IList<DevExpress.Xpf.Diagram.DiagramItem>); } }
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
			if(destinationType == typeof(string))
				return "(Collection)"; 
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public object Convert(object value, object owner) {
			return ((XRDiagramRoot)owner).Items;
		}
		public object ConvertBack(object value) {
			throw new NotSupportedException();
		}
	}
	public class SubBandCollectionPropertyConverter : TypeConverter, IXRPropertyConverter {
		public Type PropertyType { get { return typeof(SubBandCollection); } }
		public Type VirtualPropertyType { get { return typeof(IList<DevExpress.Xpf.Diagram.DiagramItem>); } }
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
			if(destinationType == typeof(string))
				return "(Collection)"; 
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public object Convert(object value, object owner) {
			return ((BandDiagramItem)owner).Items;
		}
		public object ConvertBack(object value) {
			throw new NotSupportedException();
		}
	}
	public class XRControlStyleCollectionPropertyConverter : TypeConverter, IXRPropertyConverter {
		public Type PropertyType { get { return typeof(XRControlStyleSheet); } }
		public Type VirtualPropertyType { get { return typeof(IList<DevExpress.Xpf.Diagram.DiagramItem>); } }
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
			if(destinationType == typeof(string))
				return "(Collection)"; 
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public object Convert(object value, object owner) {
			return ((XRDiagramRoot)owner).Items;
		}
		public object ConvertBack(object value) {
			throw new NotSupportedException();
		}
	}
	class RootDescriptorContext : ITypeDescriptorContext {
		readonly object instance;
		public RootDescriptorContext(object instance) {
			this.instance = instance;
		}
		IContainer ITypeDescriptorContext.Container { get { return null; } }
		object ITypeDescriptorContext.Instance { get { return instance; } }
		PropertyDescriptor ITypeDescriptorContext.PropertyDescriptor { get { return null; } }
		object IServiceProvider.GetService(Type serviceType) { return null; }
		void ITypeDescriptorContext.OnComponentChanged() { }
		bool ITypeDescriptorContext.OnComponentChanging() { return false; }
	}
}
