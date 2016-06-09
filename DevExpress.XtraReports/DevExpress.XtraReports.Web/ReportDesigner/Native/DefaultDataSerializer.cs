#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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
using System.Data;
using DevExpress.Data.Native;
using DevExpress.XtraReports.Native;
namespace DevExpress.XtraReports.Web.ReportDesigner.Native {
	public class DefaultDataSerializer : IDataSerializer {
		public const string Name = "DevExpress.XtraReports.Web.ReportDesigner.DefaultDataSerializer";
		const string ActivatorPrefix = "Activator:";
#if DEBUGTEST
		internal const string ActivatorPrefix_TEST = ActivatorPrefix;
#endif
		static DefaultDataSerializer() {
			SerializationService.RegisterSerializer(Name, new DefaultDataSerializer());
		}
		public static void SafeInitialize() {
		}
		#region IDataSerializer
		public bool CanSerialize(object data, object extensionProvider) {
			return IsDataSetOrDataTable(data) || IsDataAdapter(data);
		}
		public string Serialize(object data, object extensionProvider) {
			return ActivatorPrefix + data.GetType().AssemblyQualifiedName;
		}
		public bool CanDeserialize(string value, string typeName, object extensionProvider) {
			return value != null && value.StartsWith(ActivatorPrefix, StringComparison.Ordinal);
		}
		public object Deserialize(string value, string typeName, object extensionProvider) {
			var dataSourceTypeName = value.Split(new[] { ActivatorPrefix }, 2, StringSplitOptions.RemoveEmptyEntries)[0];
			if(string.IsNullOrEmpty(dataSourceTypeName)) {
				return null;
			}
			var type = Type.GetType(dataSourceTypeName, false, false);
			if(type == null) {
				return null;
			}
			return Activator.CreateInstance(type);
		}
		#endregion
		static bool IsDataSetOrDataTable(object data) {
			return (data is DataSet && data.GetType() != typeof(DataSet))
				|| (data is DataTable && data.GetType() != typeof(DataTable));
		}
		static bool IsDataAdapter(object data) {
			return BindingHelper.ConvertToIDataAdapter(data) != null;
		}
	}
}
