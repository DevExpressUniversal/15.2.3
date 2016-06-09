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

using System.Windows.Forms;
using System;
using System.IO;
using DevExpress.XtraEditors;
using DevExpress.XtraTreeList.Design;
namespace DevExpress.XtraScheduler.Design {
	public partial class DataBaseCreationScriptForm : XtraForm {
		public DataBaseCreationScriptForm(DBCreationScriptInfo info) {
			InitializeComponent();
			Text = info.Name;
			this.hyperLinkEdit1.EditValue = info.DocHelp;
			this.rtbCreateScript.Text += GetDBCreationScript(info.ResourcePath);
		}
		string GetDBCreationScript(string path) {
			string result = String.Empty;
			using (Stream stream = GetType().Assembly.GetManifestResourceStream(path)) { 
				StreamReader reader = new StreamReader(stream);
				result = reader.ReadToEnd();
			}
			return result;
		}
		void OnSimpleButton1Click(object sender, EventArgs e) {
			System.Windows.Forms.Clipboard.SetText(this.rtbCreateScript.Text);
		}
	}
	public class DBCreationScriptInfo {
		internal const string SRCreationScriptAssemblyPath = "DevExpress.XtraScheduler.Design.Data.DataBaseCreationScript.sql";
		internal const string SRGanttCreationScriptAssemblyPath = "DevExpress.XtraScheduler.Design.Data.GanttDataBaseCreationScript.sql";
		public static DBCreationScriptInfo Sample = new DBCreationScriptInfo() {
			Name = "Create Sample Database",
			DocHelp = "http://documentation.devexpress.com/#WindowsForms/CustomDocument3289",
			ResourcePath = SRCreationScriptAssemblyPath
		};
		public static DBCreationScriptInfo SampleGantt = new DBCreationScriptInfo() {
			Name = "Create Sample Database For Gantt View",
			DocHelp = "https://documentation.devexpress.com/#WindowsForms/CustomDocument10701",
			ResourcePath = SRGanttCreationScriptAssemblyPath
		};
		public string Name { get; protected set; }
		public string DocHelp { get; protected set; }
		public string ResourcePath { get; protected set; }
	}
}
