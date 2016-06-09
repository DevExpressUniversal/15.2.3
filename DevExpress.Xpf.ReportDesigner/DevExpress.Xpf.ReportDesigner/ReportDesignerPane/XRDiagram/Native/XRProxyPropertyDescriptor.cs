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
using System.Linq;
using System.Windows;
using DevExpress.Xpf.Core.Native;
using DevExpress.Diagram.Core;
using DevExpress.Xpf.Diagram.Native;
using DevExpress.Xpf.Reports.UserDesigner.Native;
using DevExpress.Xpf.Reports.UserDesigner.Native.PropertyConverters;
using DevExpress.Xpf.Reports.UserDesigner.Native.ReportExtensions;
using DevExpress.XtraReports.UI;
using DevExpress.Mvvm.Native;
using DevExpress.Data;
using DevExpress.XtraReports.Parameters;
namespace DevExpress.Xpf.Reports.UserDesigner.Native {
	public static class XRProxyPropertyDescriptor {
		readonly static TypeTree<IXRPropertyConverter> converters = new TypeTree<IXRPropertyConverter>();
		readonly static Dictionary<PropertyDescriptor, IXRPropertyConverter> propertyConverters = new Dictionary<PropertyDescriptor, IXRPropertyConverter>(PropertyDescriptorHelper.Comparer);
		static XRProxyPropertyDescriptor() {
			foreach(var converter in new IXRPropertyConverter[] { 
				new LookUpSettingsPropertyConverter(),
				new XRChartTitleCollectionPropertyConverter(), 
				new XRControlStyleCollectionPropertyConverter(),
				new XRDataFilterCollectionPropertyConverter(),
				new XRPivotGridFieldCollectionPropertyConverter(),
				new XRPivotGridCustomTotalCollectionPropertyConverter(),
				new SubBandCollectionPropertyConverter(),
				new FormattingSheetCollectionPropertyConverter(),
				new CalculatedFieldCollectionPropertyConverter(),
				new ReportParameterCollectionPropertyConverter(),
				new XRColorPropertyConverter(), 
				new XRImagePropertyConverter(), 
				new XRParameterPropertyConverter(),
				new XRSeriesViewBasePropertyConverter() })
				converters.Add(converter.PropertyType, converter);
			RegisterPropertyCovnerter(typeof(XtraReportBase), "DataMember", new DataMemberPropertyConverter());
			RegisterPropertyCovnerter(typeof(XRSubreport), "ReportSource", new ReportSourcePropertyConverter());
			RegisterPropertyCovnerter(typeof(DynamicListLookUpSettings), "DataMember", new DataMemberPropertyConverter());
			RegisterPropertyCovnerter(typeof(DynamicListLookUpSettings), "DisplayMember", new FieldMemberPropertyConverter());
			RegisterPropertyCovnerter(typeof(DynamicListLookUpSettings), "ValueMember", new FieldMemberPropertyConverter());
		}
		static void RegisterPropertyCovnerter(Type type, string property, IXRPropertyConverter propertyConverter) {
			var descriptor = TypeDescriptor.GetProperties(type)[property];
			propertyConverters[descriptor] = propertyConverter;
		}
		static IXRPropertyConverter GetConverter(Type propertyType) {
			return converters.Find(propertyType).Where(x => x.IsNearest).Select(x => x.Value).FirstOrDefault();
		}
		static IXRPropertyConverter GetPropertyConverter(PropertyDescriptor descriptor) {
			return propertyConverters.ContainsKey(descriptor) ? propertyConverters[descriptor] : null;
		}
		public static PropertyDescriptor Create<T>(PropertyDescriptor property, Func<T, object> getRealComponent) {
			IXRPropertyConverter converter = GetPropertyConverter(property) ?? GetConverter(property.PropertyType);
			return converter == null
				? ProxyPropertyDescriptor.Create(property, getRealComponent)
				: new XRProxyPropertyDescriptorCore<T>(property, getRealComponent, converter);
		}
		public static IEnumerable<PropertyDescriptor> GetXRProxyDescriptors<T>(UIElement host, T component, Func<T, object> getRealComponent, ITypeDescriptorContext realComponentContext = null, TypeConverter realComponentOwnerPropertyConverter = null, Attribute[] attributes = null) {
			return ProxyPropertyDescriptor.GetProxyDescriptorsCore(component, getRealComponent, x => new TrackablePropertyDescriptor(x), Create, realComponentContext, realComponentOwnerPropertyConverter, attributes);
		}
		class XRProxyPropertyDescriptorCore<T> : ProxyPropertyDescriptor.ProxyPropertyDescriptorCore<T> {
			readonly IXRPropertyConverter converter;
			public XRProxyPropertyDescriptorCore(PropertyDescriptor property, Func<T, object> getRealComponent, IXRPropertyConverter converter)
				: base(property, getRealComponent) {
				this.converter = converter;
			}
			public override TypeConverter Converter { get { return (TypeConverter)converter; } }
			public override Type PropertyType { get { return converter.VirtualPropertyType; } }
			public override void SetValue(object component, object value) {
				base.SetValue(component, converter.ConvertBack(value));
			}
			public override object GetValue(object component) {
				return converter.Convert(baseDescriptor.GetValue(GetRealComponent(component)), component);
			}
		}
	}
}
