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
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.Xpo;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.XamlExport;
using DevExpress.XtraReports.Service.Native.DAL;
namespace DevExpress.XtraReports.Service.Native {
	public static class Helper {
		static readonly IEnumerable<string> ExcludedAssemblyPrefixesField = new[] {
			"vshost32",
			"mscorlib,",
			"System.",
			"System,",
			"Microsoft."
		};
		public static IEnumerable<string> ExcludedAssemblyPrefixes {
			get { return ExcludedAssemblyPrefixesField; }
		}
		public static T[] Exclude<T>(this IEnumerable<T> list, T exclude) {
			return list.Where(element => !element.Equals(exclude)).ToArray();
		}
		public static string GetPropertyName<T>(this Expression<Func<T>> property) {
			return GetPropertyNameExtracted((MemberExpression)property.Body);
		}
		public static string GetPropertyName<T>(this Expression<Func<T, object>> property) {
			var body = property.Body;
			var unaryExpression = body as UnaryExpression;
			if(unaryExpression != null) {
				body = unaryExpression.Operand;
			}
			return GetPropertyNameExtracted((MemberExpression)body);
		}
		static string GetPropertyNameExtracted(MemberExpression memberExpression) {
			return memberExpression.Member.Name;
		}
		public static IEnumerable<BoxedExportMode> ToBoxed(this AvailableExportModes exportModes, StoredDocumentRelatedData owner, Session session) {
			return exportModes.Rtf.Select(x => new BoxedExportMode(x, owner, session))
				.Concat(exportModes.Html.Select(x => new BoxedExportMode(x, owner, session)))
				.Concat(exportModes.Image.Select(x => new BoxedExportMode(x, owner, session)))
				.Concat(exportModes.Xls.Select(x => new BoxedExportMode(x, owner, session)))
				.Concat(exportModes.Xlsx.Select(x => new BoxedExportMode(x, owner, session)));
		}
		public static ServiceFault FromStored(this StoredServiceFault storedServiceFault) {
			if(storedServiceFault == null) {
				return null;
			}
			return new ServiceFault {
				FullMessage = storedServiceFault.FullMessage,
				Message = storedServiceFault.Message
			};
		}
		public static StoredServiceFault ToStored(this ServiceFault serviceFault, Session session) {
			if(serviceFault == null) {
				return null;
			}
			return new StoredServiceFault(session) { FullMessage = serviceFault.FullMessage, Message = serviceFault.Message };
		}
		public static XamlCompatibility ToXamlCompatibility(this PageCompatibility compatibility) {
			switch(compatibility) {
				case PageCompatibility.Silverlight:
					return XamlCompatibility.Silverlight;
				case PageCompatibility.WPF:
					return XamlCompatibility.WPF;
				default:
					throw new ArgumentOutOfRangeException("compatibility", compatibility, string.Format("PageCompatibility '{0}' is not supported.", compatibility));
			}
		}
		public static bool TryGetValue(this DbConnectionStringBuilder builder, string key, out string value) {
			object valueObj;
			var result = builder.TryGetValue(key, out valueObj);
			value = result ? valueObj as string : null;
			return result;
		}
		public static void DoubleCheckInitialize<T>(ref T obj, object syncRoot, Func<T> initialize)
			where T : class {
			if(obj != null) {
				return;
			}
			lock (syncRoot) {
				if(obj == null) {
					obj = initialize();
				}
			}
		}
	}
}
