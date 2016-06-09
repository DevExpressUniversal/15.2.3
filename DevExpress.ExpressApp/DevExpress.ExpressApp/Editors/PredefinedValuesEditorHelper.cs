#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Text.RegularExpressions;
namespace DevExpress.ExpressApp.Editors {
	public static class PredefinedValuesEditorHelper {
		private static string GetEscapedValue(string value) {
			return value.Replace("\\", "\\\\").Replace(";", "\\;");
		}
		public static List<string> CreatePredefinedValuesFromString(string predefinedValues) {
			string backSlashEscape = Encoding.UTF8.GetString(new byte[] { 1 });
			string semicolonEscape = Encoding.UTF8.GetString(new byte[] { 2 });
			string preparedString = predefinedValues.Replace("\\\\", backSlashEscape);
			preparedString = preparedString.Replace("\\;", semicolonEscape);
			List<string> result = new List<string>();
			foreach(string item in preparedString.Split(';')) {
				result.Add(item.Replace(backSlashEscape, "\\").Replace(semicolonEscape, ";"));
			}
			return result;
		}
		public static string GetMergedPredefinedValuesString(string existentValues, string newValue) {
			return GetMergedPredefinedValuesString(CreatePredefinedValuesFromString(existentValues), newValue);
		}
		public static string GetMergedPredefinedValuesString(IEnumerable<string> existentValues, string newValue) {
			List<string> list = new List<string>();
			foreach(string value in existentValues) {
				list.Add(GetEscapedValue(value));
			}
			newValue = GetEscapedValue(newValue);
			if(!list.Contains(newValue) && !string.IsNullOrEmpty(newValue)) {
				list.Add(newValue);
			}
			return string.Join(";", list.ToArray());
		}
	}
}
