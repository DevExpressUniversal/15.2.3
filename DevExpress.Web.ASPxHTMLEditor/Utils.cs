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

using System.Collections.Generic;
using DevExpress.Web.ASPxHtmlEditor.Internal;
namespace DevExpress.Web.ASPxHtmlEditor {
	public class TagUtils {
		private static string[] tagNames;
		private static object syncRoot = new object();
		public static string[] TagNames {
			get {
				if(tagNames == null) {
					lock(syncRoot) {
						if(tagNames == null) {
							List<string> tagNamesList = new List<string>();
							foreach(DtdElementDeclaration decl in DtdXhtml10Trans.ElementDeclarations) {
								if(!decl.IsEmpty && decl.ValidationBehavior.ElementType != ElementType.Forbidden && decl.Name != "#document" && decl.Name != "html") {
									tagNamesList.Add(decl.Name);
								}
							}
							tagNamesList.Sort();
							tagNames = tagNamesList.ToArray();
						}
					}
				}
				return tagNames;
			}
		}
	}
}
namespace DevExpress.Web.ASPxHtmlEditor.Internal {
	using DevExpress.Web.Internal;
	public static class UrlProcessor {
		public static string PrepareUrl(ASPxHtmlEditor htmlEditor, string url) {
			return PrepareUrl(htmlEditor, url, false);
		}
		public static string PrepareUrl(ASPxHtmlEditor htmlEditor, string url, bool isDialogUrl) {
			ResourcePathMode mode = htmlEditor.SettingsHtmlEditing.ResourcePathMode;
			if(isDialogUrl && mode == ResourcePathMode.NotSet)
				mode = ResourcePathMode.RootRelative;
			return PrepareUrl(mode, url);
		}
		public static string PrepareUrl(ResourcePathMode mode, string url) {
			if(!string.IsNullOrEmpty(url)) {
				switch(mode) {
					case ResourcePathMode.Absolute:
						return UrlUtils.GetAbsoluteUrl(url);
					case ResourcePathMode.Relative:
						return UrlUtils.GetPathRelativeUrl(url);
					case ResourcePathMode.RootRelative:
						return UrlUtils.GetRootRelativeUrl(url);
				}
			}
			return url;
		}
	}
}
