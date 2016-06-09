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
using System.IO;
using System.Xml;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
namespace DevExpress.Xpf.Core.Design.Wizards.Mvvm {
	public static class TemplatingUtils {
		public static string FirstCharToLowerCase(string str) {
			if (string.IsNullOrEmpty(str))
				return str;
			return char.ToLower(str[0]) + str.Substring(1);
		}
		public static string FormatXaml(string xaml) {
			try {
				XmlDocument doc = new XmlDocument();
				doc.LoadXml(xaml);
				StringBuilder sb = new StringBuilder();
				StringWriter stringWriter = new StringWriter(sb);
				XmlTextWriter writer = new XmlTextWriter(stringWriter);
				writer.Formatting = Formatting.Indented;
				writer.Indentation = 4;
				doc.Save(writer);
				const string xmlHeader = "<?xml version=\"1.0\" encoding=\"utf-16\"?>";
				sb.Remove(0, xmlHeader.Length);
				var res = sb.ToString();
				res = new Regex(" xmlns(:|=)").Replace(res, "\r\n        xmlns$1");
				res = new Regex(" mc:Ignorable").Replace(res, "\r\n        mc:Ignorable");
				return res.Trim();
			}
			catch {
				return xaml;
			}
		}
	}
}
