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
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.XtraReports.Native.Data {
	using DevExpress.Data.Browsing.Design;
	using DevExpress.Data.Browsing;
	using DevExpress.XtraReports.Design;
	using System.ComponentModel;
	using System.Collections;
	using DevExpress.XtraReports.UI;
	using DevExpress.XtraReports.Native.CalculatedFields;
	public static class LSPropertiesFilter {
		public static PropertyDescriptor[] FilterProperties(PropertyDescriptor[] properties, object dataSource, string dataMember, DataContext dataContext) {
			List<PropertyDescriptor> result = new List<PropertyDescriptor>();
			foreach(PropertyDescriptor property in properties) {
				if(property.PropertyType.FullName == "Microsoft.LightSwitch.IEntityObject")
					continue;
				if(property.Name == "Details" && property.PropertyType.GetInterface("Microsoft.LightSwitch.Details.IDetails") != null)
					continue;
				if(DataContext.IsComplexType(property.PropertyType) && !ThereAreProperties(dataSource, PropertiesProvider.GetFullName(dataMember, property.Name), dataContext))
					continue;
				result.Add(property);
			}
			return result.ToArray();
		}
		static bool ThereAreProperties(object dataSource, string dataMember, DataContext dataContext) {
			return dataContext.GetListItemProperties(dataSource, dataMember).Count > 0;
		}
	}
	public class LSDataContextService : XRDataContextServiceBase {
		public LSDataContextService(XtraReport report) : base(report) {
		}
		protected override PropertyDescriptor[] FilterProperties(PropertyDescriptor[] properties, object dataSource, string dataMember, DataContext dataContext) {
			PropertyDescriptor[] filteredProperties = base.FilterProperties(properties, dataSource, dataMember, dataContext);
			return LSPropertiesFilter.FilterProperties(filteredProperties, dataSource, dataMember, dataContext);
		}
	} 
	public class XRDataContextServiceBase : DataContextServiceBase {
		XtraReport report;
		public XRDataContextServiceBase(XtraReport report): base() {
			this.report = report;
		}
		protected override IEnumerable<ICalculatedField> CalculatedFields {
			get {
				return report != null ? report.CalculatedFields : null;
			}
		}
		protected override DataContext CreateDataContextInternal(DataContextOptions options) {
			return new XRDataContext(options.UseCalculatedFields ? CalculatedFields : null, SuppressListFilling);
		}
	}
}
