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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Globalization;
using System.Linq;
using DevExpress.Data.Utils;
using DevExpress.XtraReports.Web;
namespace DevExpress.Web.Design.Reports.Converters {
	public class ReportViewerConverter : TypeConverter {
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return sourceType == typeof(string);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			if(value == null)
				return string.Empty;
			if(value is string)
				return (string)value;
			throw base.GetConvertFromException(value);
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			if(context == null) {
				return CreateEmptyStandardValuesCollection();
			}
			IComponent component = context.Instance as IComponent;
			if(component == null && context.Instance is DesignerActionList) {
				component = ((DesignerActionList)context.Instance).Component as IComponent;
			}
			if(component == null) {
				return CreateEmptyStandardValuesCollection();
			}
			ISite site = component.Site;
			if(site == null) {
				return CreateEmptyStandardValuesCollection();
			}
			IDesignerHost host = site.GetService<IDesignerHost>();
			if(host == null) {
				return CreateEmptyStandardValuesCollection();
			}
			var ids = host.Container.Components
				.OfType<ReportViewer>()
				.Select(x => x.ID)
				.ToList();
			return new StandardValuesCollection(ids);
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
			return false;
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return context.Instance is IComponent || context.Instance is DesignerActionList;
		}
		static StandardValuesCollection CreateEmptyStandardValuesCollection() {
			return new StandardValuesCollection(new string[0]);
		}
	}
}
