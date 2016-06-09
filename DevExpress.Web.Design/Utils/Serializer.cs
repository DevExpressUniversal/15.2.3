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

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.Design;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Design {
	public class Serializer {
		private ASPxWebControlDesignerBase fDesigner = null;
		public Serializer(ASPxWebControlDesignerBase designer) {
			fDesigner = designer;
		}
		public ASPxWebControlDesignerBase Designer {
			get { return fDesigner; }
		}
		public IDesignerHost DesignerHost {
			get { return (Designer != null) ? Designer.DesignerHost : null; }
		}
		public WebFormsRootDesigner DesignerRoot {
			get { return (Designer != null) ? Designer.DesignerRoot : null; }
		}
		public void ClearControl(Control control, bool needClearCannotBeEmptyProperty) {
			ThemesHelper.ClearProperties(control, needClearCannotBeEmptyProperty);
		}
		public void LoadControlFromStream(Control control, Stream stream) {
			ClearControl(control, false);
			using (StreamReader sr = new StreamReader(stream)) {
				string content = sr.ReadToEnd();
				LoadControl(control, content);
			}
		}
		public void SaveControlToStream(Control control, Stream stream) {
			using (StreamWriter sw = new StreamWriter(stream)) {
				string content = SaveControl(control);
				sw.Write(content);
				sw.Flush();
			}
		}
		public void LoadControl(Control control, string content) {
			string directives = "";
			AspxCodeUtils.SeparateContent(ref content, ref directives);
			Control sourceControl = ControlParser.ParseControl(DesignerHost, content, directives);
			ThemesHelper.CopyProperties(sourceControl, control);
		}
		public string SaveControl(Control control) {
			string content = ControlPersister.PersistControl(control, DesignerHost);
			content = AddTagPrefixRegistrations(content);
			content = RemoveDesignerAttributes(content);
			return content;
		}
		protected string AddTagPrefixRegistrations(string content) {
			ICollection directives = DesignerRoot.ReferenceManager.GetRegisterDirectives();
			foreach (string directive in directives) 
				content = directive + "\n" + content;
			return content;
		}
		protected string RemoveDesignerAttributes(string content) {
			Regex regexId = new Regex("\\s+ID=\".+?\"", RegexOptions.IgnoreCase);
			Regex regexDesigner = new Regex("\\s+__designer:[^=]+=\".+?\"", RegexOptions.IgnoreCase);
			content = regexId.Replace(content, "");
			content = regexDesigner.Replace(content, "");
			return content;
		}
	}
}
