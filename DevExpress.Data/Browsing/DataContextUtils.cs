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
using DevExpress.Data.Browsing;
using System.ComponentModel;
using DevExpress.Utils;
using DevExpress.Data;
using DevExpress.Data.Native;
using DevExpress.XtraReports.Native.Data;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.Data.Browsing {
	public class DataContextUtils {
		DataContext dataContext;
		public DataContextUtils(DataContext dataContext) {
			this.dataContext = dataContext;
		}
		public PropertyDescriptor[] GetDisplayedProperties(object dataSource, string dataMember, Func<PropertyDescriptor, bool> predicate) {
			DataBrowser dataBrowser = dataContext.GetDataBrowser(dataSource, dataMember, true);
			try {
				PropertyDescriptorCollection properties = dataBrowser != null ? dataBrowser.GetItemProperties() : null;
				if(properties == null)
					return null;
				PropertyDescriptor[] filterProperties = FilterProperties(properties, dataSource, dataMember, predicate);
				List<PropertyDescriptor> items = new List<PropertyDescriptor>();
				foreach(PropertyDescriptor property in filterProperties) {
					string displayName = GetPropertyDisplayName(dataSource, dataMember, property);
					if(!string.IsNullOrEmpty(displayName))
						items.Add(new DisplayPropertyDescriptor(property, displayName));
				}
				return items.ToArray();
			} catch {
				return null;
			}
		}
		protected virtual PropertyDescriptor[] FilterProperties(PropertyDescriptorCollection properties, object dataSource, string dataMember, Func<PropertyDescriptor, bool> predicate) {
			List<PropertyDescriptor> result = new List<PropertyDescriptor>();
			PropertyDescriptor[] filterProperties = new PropertyAggregator(dataContext).Aggregate(properties, dataSource, dataMember);
			foreach(PropertyDescriptor property in filterProperties) {
				if(predicate == null || predicate(property))
					result.Add(property);
			}
			return result.ToArray();
		}
		string GetPropertyDisplayName(object dataSource, string dataMember, PropertyDescriptor property) {
			if(dataMember == null)
				dataMember = String.Empty;
			if(dataSource is IDisplayNameProvider)
				return dataContext.GetDataMemberDisplayName(dataSource, dataMember, BindingHelper.JoinStrings(".", dataMember, property.Name));
			return DevExpress.Data.Helpers.MasterDetailHelper.GetDisplayNameCore(property);
		}
	}
}
