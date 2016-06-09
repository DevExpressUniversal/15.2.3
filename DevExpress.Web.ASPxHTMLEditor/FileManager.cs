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
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using DevExpress.Web.Internal;
using System.Web.UI;
using DevExpress.Web;
namespace DevExpress.Web.ASPxHtmlEditor {
	[ToolboxItem(false)]
	public class HtmlEditorFileManager : ASPxFileManager {
		protected internal new const string ScriptName = ASPxHtmlEditor.HtmlEditorScriptsResourcePath + "FileManager.js";
		internal bool IsHtmlEditorCallback { get; set; }
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(HtmlEditorFileManager), ScriptName);
		}
		protected override StylesBase CreateStyles() {
			return new HtmlEditorFileManagerStyles(this);
		}
		protected override string GetClientObjectClassName() {
			return "ASPx.HtmlEditorFileManager";
		}
		protected internal object GetCallbackResult(string callbackArgs) {
			RaisePostBackEventCore(callbackArgs);
			return GetCallbackResult();
		}
		protected override bool IsNeedResetToInitalFolder() {
			return false;
		}
		protected override bool IsNeedToAddCallbackCommandResult() {
			return base.IsNeedToAddCallbackCommandResult() || IsHtmlEditorCallback;
		}
		protected override string GetRootFolderRelativePath(string rootFolderPath) {
			return CanUseAppRelativeRootFolder(rootFolderPath) ? GetAppRelativeRootFolder() : rootFolderPath;
		}
		protected internal bool CanUseAppRelativeRootFolder(string rootFolderPath) {
			return IsPhysicalFileSystemUsed() && string.IsNullOrEmpty(rootFolderPath);
		}
		protected internal void SetRootFolderRelativePathJSProp(string rootFolderPath) {
			JSProperties["cp_RootFolderRelativePath"] = GetRootFolderRelativePath(rootFolderPath);
		}
		bool IsPhysicalFileSystemUsed() {
			return FileSystemProvider.Provider is PhysicalFileSystemProvider;
		}
	}
	public class HtmlEditorFileManagerStyles : DevExpress.Web.FileManagerStyles {
		const string ControlStyleName = "ControlStyle";
		public HtmlEditorFileManagerStyles(ISkinOwner owner)
			: base(owner) {
		}
		[Browsable(false)]
		public new string CssFilePath {
			get { return base.CssFilePath; }
			set { base.CssFilePath = value; }
		}
		[Browsable(false)]
		public override string CssPostfix {
			get { return base.CssPostfix; }
			set { base.CssPostfix = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorFileManagerStylesControl"),
#endif
NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public AppearanceStyleBase Control
		{
			get { return GetStyle(ControlStyleName); }
		}
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(ControlStyleName, delegate() { return new AppearanceStyleBase(); }));
		}
	}
}
