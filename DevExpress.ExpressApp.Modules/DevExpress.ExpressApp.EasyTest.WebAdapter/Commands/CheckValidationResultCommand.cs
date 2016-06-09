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
using System.Diagnostics;
using System.Runtime.InteropServices;
using DevExpress.EasyTest.Framework;
using System.Windows.Forms;
using System.Threading;
using mshtml;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Drawing;
using DevExpress.EasyTest.Framework.Utils;
using System.Drawing.Imaging;
using System.ComponentModel;
using DevExpress.EasyTest.Framework.Commands;
namespace DevExpress.ExpressApp.EasyTest.WebAdapter.Commands {
	public class WebCheckValidationResultCommand : CheckValidationResultCommand {
		private const string descriptionPrefix = "- ";
		private void CheckMessageField(string message, string errorInfoValue) {
			MultiLineComparisionHelper helper = new MultiLineComparisionHelper();
			string compareResult = helper.Compare("CheckValidationResult", new Parameter(" ErrorInfo[0] = " + (string.IsNullOrEmpty(message) ? "''" : message), StartPosition), errorInfoValue, "message value");
			if(!string.IsNullOrEmpty(compareResult)) {
				throw new TestException(compareResult, StartPosition);
			}
		}
		private static List<KeyValuePair<string, string>> CalculateActualInfos(string errorInfoValue) {
			string[] errorInfo = MultiLineComparisionHelper.GetLines(errorInfoValue);
			List<KeyValuePair<string, string>> actualInfos = new List<KeyValuePair<string, string>>();
			string currentTarget = string.Empty;
			for(int iLine = 1; iLine < errorInfo.Length; iLine++) {
				string errorLine = errorInfo[iLine].Trim(' ');
				if(errorLine.StartsWith(descriptionPrefix)) {
					string currentDescription = errorLine.Substring(2);
					actualInfos.Add(new KeyValuePair<string, string>(currentTarget, currentDescription));
				}
				else {
					currentTarget = errorLine;
				}
			}
			return actualInfos;
		}
		private void CheckInfo(string[] columns, string[,] info, List<KeyValuePair<string, string>> actualInfos, bool skipUnexpectedErrors) {
			for(int iLine = 0; iLine < info.GetLength(0); iLine++) {
				bool isFound = false;
				foreach(KeyValuePair<string, string> actualInfo in actualInfos) {
					string actualDescription = actualInfo.Value;
					string actualTarget = actualInfo.Key;
					if((columns.Length == 1 && MultiLineComparisionHelper.CompareString(actualDescription, info[iLine, 0])) ||
					  (columns.Length > 1 && MultiLineComparisionHelper.CompareString(actualTarget, info[iLine, 0]) && MultiLineComparisionHelper.CompareString(actualInfo.Value, info[iLine, 1]))) {
						isFound = true;
						break;
					}
				}
				if(!isFound) {
					throw new TestException(string.Format("The specified identity row was not found in the table:\n{0}", info.GetLength(1) == 1 ? info[iLine, 0] : info[iLine, 0] + ", " + info[iLine, 1]), StartPosition);
				}
			}
			if(actualInfos.Count > info.GetLength(0) && !skipUnexpectedErrors) {
				throw new CommandException(string.Format("{0} unexpected validation error{1} occur", actualInfos.Count - info.GetLength(0), actualInfos.Count - info.GetLength(0) == 1 ? "" : "s"), this.StartPosition);
			}
		}
		public static string GetFieldValue(ICommandAdapter adapter, string fieldName) {
			if(adapter.IsControlExist(TestControlType.Field, fieldName)) {
				ITestControl testControl = adapter.CreateTestControl(TestControlType.Field, fieldName);
				string fieldValue = testControl.GetInterface<IControlReadOnlyText>().Text;
				return fieldValue;
			}
			return "";
		}
		protected override void InternalCheckValidationResult(ICommandAdapter adapter, string message, string[] columns, string[,] info, bool skipUnexpectedErrors) {
			try {
				string errorInfoValue = GetFieldValue(adapter, "ErrorInfo") + Environment.NewLine + GetFieldValue(adapter, "ValidationError") + Environment.NewLine + 
					GetFieldValue(adapter, "ValidationWarning");
				if(message != null) {
					CheckMessageField(message, errorInfoValue);
				}
				List<KeyValuePair<string, string>> actualInfos = CalculateActualInfos(errorInfoValue);
				if(info != null) {
					CheckInfo(columns, info, actualInfos, skipUnexpectedErrors);
				}
			}
			catch(EasyTestException ex) {
				ex.PositionInScript = StartPosition;
				throw;
			}
		}
	}
}
