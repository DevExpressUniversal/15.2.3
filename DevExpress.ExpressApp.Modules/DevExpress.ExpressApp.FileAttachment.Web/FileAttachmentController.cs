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
using System.ComponentModel;
using System.Security;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.FileAttachments.Web {
	public class FileAttachmentController : ObjectViewController {
		private const string HasFileAttachmentAttributeKey = "HasFileAttachmentAttribute";
		private IContainer components;
		private SimpleAction downloadAction;
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.downloadAction = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
			this.downloadAction.Caption = "Download";
			this.downloadAction.ToolTip = "Download file";
			this.downloadAction.ImageName = "Action_Save_To";
			this.downloadAction.Category = PredefinedCategory.Edit.ToString();
			this.downloadAction.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireSingleObject;
			this.downloadAction.Id = "Download";
			this.downloadAction.Execute += new SimpleActionExecuteEventHandler(downloadAction_OnExecute);
		}
		protected override void OnViewChanging(View view) {
			base.OnViewChanging(view);
			bool hasFileAttachmentAttribute = false;
			ObjectView objectView = view as ObjectView;
			if(objectView != null) {
				FileAttachmentAttribute fileAttachmentAttribute = objectView.ObjectTypeInfo.FindAttribute<FileAttachmentAttribute>();
				hasFileAttachmentAttribute = fileAttachmentAttribute != null;
			}
			Active.SetItemValue(HasFileAttachmentAttributeKey, hasFileAttachmentAttribute);
		}
		protected override void OnActivated() {
			base.OnActivated();
			downloadAction.Active.SetItemValue("Has permission", DataManipulationRight.CanRead(View.ObjectTypeInfo.Type, null, null, LinkToListViewController.FindCollectionSource(Frame), View.ObjectSpace));
			downloadAction.Active.SetItemValue("Saved object", !ObjectSpace.IsNewObject(View.CurrentObject));
		}
		public FileAttachmentController() {
			InitializeComponent();
			RegisterActions(components);
		}
		public virtual void Download(object obj) {
			Guard.ArgumentNotNull(obj, "obj");
			if(!DataManipulationRight.CanRead(View.ObjectTypeInfo.Type, null, obj, LinkToListViewController.FindCollectionSource(Frame), View.ObjectSpace)) {
				throw new SecurityException(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.PermissionIsDenied));
			}
			FileAttachmentAttribute fileAttachmentAttribute = View.ObjectTypeInfo.FindAttribute<FileAttachmentAttribute>();
			IFileData attachFile = ReflectionHelper.GetMemberValue(obj, fileAttachmentAttribute.FileDataPropertyName) as IFileData;
			FileDataDownloader.Download(attachFile);
		}
		private void downloadAction_OnExecute(Object sender, SimpleActionExecuteEventArgs args) {
			Download(args.CurrentObject);
		}
		public SimpleAction DownloadAction {
			get { return downloadAction; }
		}
	}
}
