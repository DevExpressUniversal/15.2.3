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
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Utils;
namespace DevExpress.Web.Internal {
	[ToolboxItem(false)]
	public class MediaFileSelector : ASPxWebControl, IRequiresLoadPostDataControl {
		protected internal const string MediaFileSelectorScriptResourceName = WebScriptsResourcePath + "MediaFileSelector.js";
		static readonly object UploadControlFileUploadEventKey = new object();
		MediaFileSelectorMainControl mainControl;
		protected MediaFileSelectorMainControl MainControl {
			get {
				if(this.mainControl == null)
					this.mainControl = new MediaFileSelectorMainControl(this);
				return this.mainControl;
			}
		}
		public MediaFileSelector()
			: base() {
			Settings = new MediaFileSelectorSettings(this);
			StylesFileManager = new FileManagerStyles(this);
			ImagesFileManager = new FileManagerImages(this);
			StylesFileManagerControl = new Style();
		} 
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			if(mainControl == null)
				Controls.Add(MainControl);
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			mainControl = null;
		}
		protected override string GetClientObjectClassName() {
			return "ASPx.MediaFileSelector";
		}
		protected override bool HasFunctionalityScripts() {
			return true;
		}
		protected override bool HasClientInitialization() {
			return true;
		}
		public override bool IsClientSideAPIEnabled() {
			return true;
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterImageControlUtilsScript();
			RegisterIncludeScript(typeof(MediaFileSelector), MediaFileSelectorScriptResourceName);
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRoundPanelRightToLeft"),
#endif
		Category("Layout"), DefaultValue(DefaultBoolean.Default), AutoFormatDisable]
		public DefaultBoolean RightToLeft {
			get { return RightToLeftInternal; }
			set { RightToLeftInternal = value; }
		}
		public string ClientInstanceName {
			get { return base.ClientInstanceNameInternal; }
			set { base.ClientInstanceNameInternal = value; }
		}
		public MediaFileSelectorSettings Settings { get; private set; }
		public FileManagerStyles StylesFileManager { get; private set; }
		public FileManagerImages ImagesFileManager { get; private set; }
		public Style StylesFileManagerControl { get; private set; }
		public static void SaveUploadedFile(ASPxUploadControl uploadControl, FileUploadCompleteEventArgs args, string uploadFolder, string fileName, string uploadFolderUrlPath) {
			if(args.IsValid) {
				try {
					UploadedFile file = args.UploadedFile;
					if(file != null && file.IsValid && !(string.IsNullOrEmpty(file.FileName) && string.IsNullOrEmpty(fileName))) {
						if(string.IsNullOrEmpty(fileName))
							fileName = file.FileName;
						string filePath = GetUniqueFileName(UrlUtils.ResolvePhysicalPath(uploadFolder), fileName.Replace("+", "[plus]").Replace("&", "[amp]"));
						file.SaveAs(filePath);
						fileName = HttpUtils.EncodeFileName(Path.GetFileName(filePath));
						if(string.IsNullOrEmpty(uploadFolderUrlPath))
							args.CallbackData = uploadControl.ResolveClientUrl(Path.Combine(uploadFolder, fileName));
						else
							args.CallbackData = Path.Combine(uploadFolderUrlPath + fileName);
					}
				} catch(Exception e) {
					args.IsValid = false;
					args.ErrorText = e.Message;
				}
			}
		}
		static string GetUniqueFileName(string directory, string fileName, int counter = 0) {
			string uniqueFilePath = Path.Combine(directory, string.Format("{0}{1:(#);;#}{2}",
				Path.GetFileNameWithoutExtension(fileName), counter, Path.GetExtension(fileName)));
			if(File.Exists(uniqueFilePath))
				return GetUniqueFileName(directory, fileName, ++counter);
			return uniqueFilePath;
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			stb.AppendFormat("{0}.appDomainPath = '{1}';\n", localVarName, UrlUtils.AppDomainAppVirtualPathString);
			if(Settings.PreviewType != MediaFileSelectorPreviewType.NotSpecified)
				stb.AppendFormat("{0}.previewType = {1};\n", localVarName, (int)Settings.PreviewType);
		}
		public event FileSavingEventHandler UploadControlFileUploadCompleting {
			add { Events.AddHandler(UploadControlFileUploadEventKey, value); }
			remove { Events.RemoveHandler(UploadControlFileUploadEventKey, value); }
		}
		protected internal void OnUploadControlFileUploadComplete(object sender, FileSavingEventArgs args) {
			FileSavingEventHandler handler = (FileSavingEventHandler)Events[UploadControlFileUploadEventKey];
			if(handler != null)
				handler(this, args);
		}
		protected virtual internal ASPxUploadControl CreateUploadControl() {
			return new ASPxUploadControl();
		}
		protected virtual internal ASPxFileManager CreateFileManager() {
			return new ASPxFileManager();
		}
		public override bool EnableViewState {
			get { return false; }
			set { }
		}
	}
}
