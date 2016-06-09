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
using System.Text;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.ComponentModel.Design.Serialization;
using DevExpress.XtraReports.Serialization;
using System.Globalization;
using DevExpress.XtraReports.Native.Parameters;
using DevExpress.XtraReports.Parameters;
using System.Reflection;
using DevExpress.Data.Browsing.Design;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraPrinting.Localization;
namespace DevExpress.XtraReports.Design {
	public class XRBindingConverter : TypeConverter {
		#region static
		internal static object ValidateDataSource(object dataSource, ISite site) {
			return SiteContainsComponent(site, dataSource) ? dataSource : null;
		}
		static bool SiteContainsComponent(ISite site, object component) {
			if(site != null && site.Container != null && site.Container.Components != null) {
				foreach(IComponent item in site.Container.Components)
					if(item == component)
						return true;
			}
			return false;
		}
		#endregion
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if(destinationType == typeof(InstanceDescriptor)) {
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType == typeof(string)) {
				return "";
			}
			XRBinding xrBinding = value as XRBinding;
			if(destinationType == typeof(InstanceDescriptor) && xrBinding.Parameter != null) {
				System.Reflection.ConstructorInfo ci = value.GetType().GetConstructor(new Type[] { typeof(Parameter), typeof(string), typeof(string) });
				return new InstanceDescriptor(ci, new object[] { xrBinding.Parameter, xrBinding.PropertyName, xrBinding.FormatString });
			}
			if(destinationType == typeof(InstanceDescriptor)) {
				ISite site = xrBinding.Control != null ? xrBinding.Control.Site : null;
				object dataSource = ValidateDataSource(xrBinding.SerializeDataSource, site);
				if(string.IsNullOrEmpty(xrBinding.FormatString)) {
					ConstructorInfo ci = value.GetType().GetConstructor(new Type[] { typeof(string), typeof(object), typeof(string) });
					return new InstanceDescriptor(ci, new object[] { xrBinding.PropertyName, dataSource, xrBinding.DataMember });
				} else {
					ConstructorInfo ci = value.GetType().GetConstructor(new Type[] { typeof(string), typeof(object), typeof(string), typeof(string) });
				return new InstanceDescriptor(ci, new object[] { xrBinding.PropertyName, dataSource, xrBinding.DataMember, xrBinding.FormatString });
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
