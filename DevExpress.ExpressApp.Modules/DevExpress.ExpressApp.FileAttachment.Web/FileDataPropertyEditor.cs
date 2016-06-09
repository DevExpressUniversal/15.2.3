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
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.Editors;
using DevExpress.ExpressApp.Web.TestScripts;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.FileAttachments.Web {
	public class FileDataPropertyEditor : WebPropertyEditor, ITestableEx, IComplexViewItem {
		private IFileData FileData {
			get { return MemberInfo.GetValue(CurrentObject) as IFileData; }
		}
		protected override System.Web.UI.WebControls.WebControl CreateEditModeControlCore() {
			FileDataEdit fileEditControl = new FileDataEdit(ViewEditMode, FileData, !AllowEdit, ImmediatePostData);
			fileEditControl.CreateCustomFileDataObject += new EventHandler<CreateCustomFileDataObjectEventArgs>(CreateFileDataObject);
			return fileEditControl;
		}
		protected virtual void CreateFileDataObject(object sender, CreateCustomFileDataObjectEventArgs e) {
			e.FileData = FileAttachmentsAspNetModule.CreateFileData(objectSpace, MemberInfo);
			MemberInfo.SetValue(CurrentObject, e.FileData);
		}
		protected override object GetControlValueCore() {
			return null;
		}
		protected override WebControl CreateViewModeControlCore() {
			return CreateEditModeControlCore();
		}
		protected override IJScriptTestControl GetInplaceViewModeEditorTestControlImpl() {
			return null;
		}
		protected override IJScriptTestControl GetEditorTestControlImpl() {
			return null;
		}
		protected override void ReadViewModeValueCore() {
			((FileDataEdit)InplaceViewModeEditor).FileData = FileData;
		}
		protected override void ReadEditModeValueCore() {
			Editor.FileData = FileData;
		}
		public override bool SupportInlineEdit {
			get {
				return false;
			}
		}
		public FileDataPropertyEditor(Type objectType, IModelMemberViewItem info)
			: base(objectType, info) {
			skipEditModeDataBind = true;
		}
		public new FileDataEdit Editor {
			get {
				return (FileDataEdit)base.Editor;
			}
		}
		#region IComplexViewItem Members
		private IObjectSpace objectSpace;
		public void Setup(IObjectSpace objectSpace, XafApplication application) {
			this.objectSpace = objectSpace;
		}
		#endregion
		#region ITestableEx Members
		public Type RegisterControlType {
			get {
				if(Editor != null) {
					return Editor.GetType();
				}
				else {
					if(InplaceViewModeEditor != null) {
						return InplaceViewModeEditor.GetType();
					}
				}
				return Control.GetType();
			}
		}
		#endregion
	}
}
