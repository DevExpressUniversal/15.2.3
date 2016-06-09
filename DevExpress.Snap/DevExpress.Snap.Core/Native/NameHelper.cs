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
using DevExpress.XtraReports.Native.Data;
using System.ComponentModel;
using DevExpress.XtraReports.Native;
using DevExpress.Data.Browsing;
namespace DevExpress.Snap.Core.Native {
	public static class NameHelper {
		const string calculatedFieldName = "calculatedField";
		const string parameterName = "parameter";
		public static string GetCalculatedFieldName(object dataSource, IEnumerable<ICalculatedField> calculatedFields, string dataMember) {
			using (XRDataContextBase dataContext = new XRDataContextBase(calculatedFields)) {
				DataBrowser dataBrowser = dataContext[dataSource, dataMember];
				if (dataBrowser != null) {
					PropertyDescriptorCollection properties = dataBrowser.GetItemProperties();
					List<string> propertyNames = new List<string>();
					foreach (PropertyDescriptor item in properties) {
						propertyNames.Add(item.Name);
					}
					return GetName(propertyNames, calculatedFieldName);
				}
			}
			throw new NotImplementedException();
		}
		public static string GetParameterName(IEnumerable<DevExpress.Data.IParameter> parameters) {
			List<string> parameterNames = new List<string>();
			foreach (DevExpress.Snap.Core.API.Parameter item in parameters) {
				parameterNames.Add(item.Name);
			}
			return GetName(parameterNames, parameterName);
		}
		public static string GetName(List<string> names, string baseName) {
			int number = 1;
			while (names.Contains(baseName + number))
				number++;
			return baseName + number;
		}
	}
}
