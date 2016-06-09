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
using System.Linq;
using System.Collections.Generic;
using System.Text;
using DevExpress.EasyTest.Framework;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing;
using mshtml;
using System.Threading;
using DevExpress.EasyTest.Framework.Utils;
using DevExpress.EasyTest.Framework.Commands;
using DevExpress.ExpressApp.Web.Templates.ActionContainers;
namespace DevExpress.ExpressApp.EasyTest.WebAdapter.TestControls {
	public class JSNavigationTestControl : JSStandartTestControl, IControlAct, IGridBase, IControlActionItems {
		public JSNavigationTestControl(IControlDescription controlDescription) : base(controlDescription) { }
		public void Act(string value) {
			ActNavigation(value);
		}
		private void ExpandPath(string value) {
			string entriesJSONString = (string)ExecuteFunction("GetEntriesAsJSONString", null);
			if(!string.IsNullOrEmpty(entriesJSONString)) {
				NavigationNodePath[] paths = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<NavigationNodePath[]>(entriesJSONString);
				NavigationNodePath targetPath = paths.Where(
					o => { return (value == o.Entries.Last().Caption) || (value == o.Entries.Aggregate(o.NavGroupCaption, (result, o1) => result + "." + o1.Caption).Trim('.')); }
					).FirstOrDefault();
				if(targetPath != null) {
					List<NavigationNodePathEntry> entries = new List<NavigationNodePathEntry>();
					NavigationNodePathEntry firstEntry = targetPath.Entries.First();
					NavigationNodePathEntry lastEntry = targetPath.Entries.Last();
					foreach(NavigationNodePathEntry entry in targetPath.Entries) {
						if(entry != lastEntry) {
							ExecuteFunction("ExpandTreeNodeByNodeName", new object[] { targetPath.TreeId, entry.Id });
						}
					}
				}
			}
		}
		private void ActNavigation(string value) {
			ExpandPath(value);
			ExecuteFunction("Act", new string[] { value });
		}
		protected override string ScriptText {
			get {
				return ReadScriptFormResource("DevExpress.ExpressApp.EasyTest.WebAdapter.TestControls.JSNavigationTestControl.js");
			}
		}
		protected override string JSControlName {
			get { return "NavigationTestControl_JS"; }
		}
		protected override string RegisterControlType {
			get { return "DevExpress.ExpressApp.Web.Templates.ActionContainers.NavigationActionContainer"; }
		}
		#region IGridBase Members
		public IEnumerable<IGridColumn> Columns {
			get { return new IGridColumn[] { new TestGridColumn("Name", 0) }; }
		}
		public string GetCellValue(int row, IGridColumn column) {
			return (string)ExecuteFunction("GetCellValue", new object[] { row });
		}
		public int GetRowCount() {
			return (int)ExecuteFunction("GetRowCount", null);
		}
		#endregion
		#region IControlActionItems Members
		public bool IsEnabled(string item) {
			ExpandPath(item);
			return (bool)ExecuteFunction("IsActionItemEnabled", new object[] { item });
		}
		public bool IsVisible(string item) {
			ExpandPath(item);
			return (bool)ExecuteFunction("IsActionItemVisible", new object[] { item });
		}
		#endregion
	}
}
