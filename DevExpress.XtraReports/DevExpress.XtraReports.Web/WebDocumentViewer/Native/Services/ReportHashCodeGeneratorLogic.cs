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

using System.Collections;
using System.Collections.Generic;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Parameters;
namespace DevExpress.XtraReports.Web.WebDocumentViewer.Native.Services {
	public static class ReportHashCodeGeneratorLogic {
		public static long Generate(IEnumerable<ParameterPath> parameters, Dictionary<string, bool> drillDownKeys) {
			return GenerateHashCode(parameters) ^ GenerateHashCode(drillDownKeys);
		}
		static long GenerateHashCode(IEnumerable<ParameterPath> parameters) {
			long result = 0;
			if(parameters == null) {
				return result;
			}
			foreach(ParameterPath path in parameters) {
				long hash = GenerateKeyHashCode(path.Path);
				long valueHash = GenerateHashCode(path.Parameter);
				hash |= valueHash;
				result ^= hash;
			}
			return result;
		}
		static long GenerateHashCode(Dictionary<string, bool> drillDownKeys) {
			long result = 0;
			if(drillDownKeys == null) {
				return result;
			}
			int i = 0;
			foreach(var pair in drillDownKeys) {
				long hash = GenerateKeyHashCode(pair.Key);
				long valueHash = (long)pair.Value.GetHashCode() << (i % sizeof(int));
				hash |= valueHash;
				result ^= hash;
				i++;
			}
			return result;
		}
		static long GenerateKeyHashCode(string key) {
			return (long)key.GetHashCode() << sizeof(int);
		}
		static int GenerateHashCode(Parameter parameter) {
			if(parameter.Value == null) {
				return 0;
			}
			if(parameter.MultiValue) {
				return GenerateHashCode(parameter.Value as IEnumerable);
			}
			return parameter.Value.GetHashCode();
		}
		static int GenerateHashCode(IEnumerable enumerable) {
			int result = 0;
			foreach(object value in enumerable) {
				if(value != null) {
					result ^= value.GetHashCode();
				}
			}
			return result;
		}
	}
}
