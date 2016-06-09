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
using System.Text;
using System.Web;
using DevExpress.Web.Internal;
using DevExpress.XtraReports.Web.Native.ClientControls.DataContracts;
namespace DevExpress.XtraReports.Web.Native.ClientControls {
	class JsAssignmentGenerator {
		readonly StringBuilder stb;
		readonly string localVarName;
		public JsAssignmentGenerator(StringBuilder stb, string localVarName) {
			this.stb = stb;
			this.localVarName = localVarName;
		}
		public JsAssignmentGenerator AppendRawArray(string fieldName, IEnumerable<string> rawValues) {
			stb.AppendFormat("{0}.{1} = [", localVarName, fieldName);
			bool isFirst = true;
			if(rawValues != null) {
				foreach(var rawValue in rawValues) {
					if(!isFirst) {
						stb.Append(',');
					}
					stb.Append(rawValue);
					isFirst = false;
				}
			}
			stb.AppendLine("];");
			return this;
		}
		public JsAssignmentGenerator AppendContract<T>(string fieldName, T value) {
			return AppendRaw(fieldName, JsonSerializer.Stringify(value, value as IKnownTypes));
		}
		public JsAssignmentGenerator AppendDictionary(string fieldName, IDictionary value) {
			stb.AppendFormat("{0}.{1} = ", localVarName, fieldName);
			HtmlConvertor.ToJSON(stb, value, true, false, true);
			stb.AppendLine(";");
			return this;
		}
		public JsAssignmentGenerator AppendAsString(string fieldName, string value) {
			return AppendRaw(fieldName, HttpUtility.JavaScriptStringEncode(value, true));
		}
		public JsAssignmentGenerator AppendRaw(string fieldName, string value) {
			stb.AppendLine(string.Format("{0}.{1} = {2};", localVarName, fieldName, value ?? "null"));
			return this;
		}
	}
}
