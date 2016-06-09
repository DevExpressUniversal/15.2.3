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

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
namespace DevExpress.ExpressApp.Utils {
	public class EasyTestTagHelper {
		public const string TestAction = "testaction";
		public const string TestField = "testfield";
		public const string TestTable = "testtable";
		public const string TestMessage = "testdialog";
		public const string TestWaitForNewWindow = "testnewwindow";
		public const string TestControlClassName = "testControlClassName";
		public const string TestContainer = "testContainer";
		public static string FormatTestAction(string actionName) {
			return TestAction + "=" + actionName;
		}
		public static string FormatTestField(string fieldName) {
			return TestField + "=" + fieldName;
		}
		public static string FormatTestTable(string tableName) {
			return TestTable + "=" + tableName;
		}
		public static string FormatTestMessage() {
			return TestMessage + "=";
		}
		public static string FormatTestControlClassName(string className) {
			return TestControlClassName + "=" + className;
		}
		public static string FormatTestContainer(string containerName) {
			return TestContainer + "=" + containerName;
		}
		public static string GetTestValue(string testTagType, object tagValue) {
			if(tagValue is string) {
				string[] tags = ((string)tagValue).Split(';');
				foreach(string testTag in tags) {
					if(testTag.StartsWith(testTagType) && testTag.Length > testTagType.Length + 1 && testTag[testTagType.Length] == '=') {
						return testTag.Substring(testTagType.Length + 1);
					}
				}
			}
			return null;
		}
		public static string FormatDictionary(IDictionary dictionary) {
			List<string> list = new List<string>();
			foreach(DictionaryEntry entry in dictionary) {
				if(entry.Value is IEnumerable<string>) {
					string entryValue = string.Join(";", (IEnumerable<string>)entry.Value);
					list.Add(string.Format("{0}={1}", entry.Key, entryValue));
				}
				else {
					list.Add(string.Format("{0}={1}", entry.Key, entry.Value));
				}
			}
			return string.Join(";", list.ToArray());
		}
	}
}
namespace DevExpress.ExpressApp.EasyTest.Utils {
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface ITestableControl { 
		void SetTestName(string testName);
	}
}
