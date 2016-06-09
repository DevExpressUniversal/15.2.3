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
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Persistent.Base;
using DevExpress.XtraEditors.Repository;
namespace DevExpress.ExpressApp.FileAttachments.Win {
	public class FileDataPropertyEditor : DXPropertyEditor, IComplexViewItem {
		private IObjectSpace objectSpace;
		private XafApplication application;
		protected override object CreateControlCore() {
			return new FileDataEdit();
		}
		protected override void OnCurrentObjectChanged() {
			base.OnCurrentObjectChanged();
			RefreshReadOnly();
		}
		protected override bool IsMemberSetterRequired() {
			bool result = base.IsMemberSetterRequired();
			if(result) {
				return !(PropertyValue is IFileData);
			}
			return result;
		}
		protected virtual void CreateFileDataObject(object sender, CreateCustomFileDataObjectEventArgs e) {
		}
		protected override void SetRepositoryItemReadOnly(RepositoryItem item, bool readOnly) {
			base.SetRepositoryItemReadOnly(item, readOnly);
			((RepositoryItemFileDataEdit)item).FileDataReadOnly = readOnly;
		}
		protected override RepositoryItem CreateRepositoryItem() {
			return new RepositoryItemFileDataEdit();
		}
		private void RepositoryItemFileDataEdit_Disposed(object sender, EventArgs e) {
			((RepositoryItemFileDataEdit)sender).Disposed -= new EventHandler(RepositoryItemFileDataEdit_Disposed);
			((RepositoryItemFileDataEdit)sender).CreateCustomFileDataObject -= new EventHandler<CreateCustomFileDataObjectEventArgs>(CreateFileDataObject);
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(disposing) {
				objectSpace = null;
				application = null;
			}
		}
		protected override void SetupRepositoryItem(RepositoryItem item) {
			base.SetupRepositoryItem(item);
			RepositoryItemFileDataEdit repositoryItemFileDataEdit = item as RepositoryItemFileDataEdit;
			if(repositoryItemFileDataEdit != null) {
				repositoryItemFileDataEdit.CreateCustomFileDataObject += new EventHandler<CreateCustomFileDataObjectEventArgs>(CreateFileDataObject);
				repositoryItemFileDataEdit.Disposed += new EventHandler(RepositoryItemFileDataEdit_Disposed);
				repositoryItemFileDataEdit.ObjectSpace = objectSpace;
				repositoryItemFileDataEdit.MemberInfo = MemberInfo;
				if(Model.ModelMember != null && Model.ModelMember.ModelClass != null) {
					string fileTypesFilter = FileAttachmentsWindowsFormsModule.GetFileTypesFilter(Model.ModelMember.ModelClass, Model.PropertyName);
					repositoryItemFileDataEdit.FileTypesFilter = fileTypesFilter;
				}
				repositoryItemFileDataEdit.FileDataManager = FileAttachmentsWindowsFormsModule.GetFileDataManager(application);
			}
		}
		protected override void InitializeAppearance(RepositoryItem item) {
			base.InitializeAppearance(item);
			RepositoryItemFileDataEdit repositoryItemFileDataEdit = item as RepositoryItemFileDataEdit;
			if(repositoryItemFileDataEdit != null) {
				repositoryItemFileDataEdit.InitializeAppearance();
			}
		}
		protected FileAttachmentsWindowsFormsModule FileAttachmentsWindowsFormsModule {
			get {
				if(application != null) {
					return (FileAttachmentsWindowsFormsModule)application.Modules.FindModule(typeof(FileAttachmentsWindowsFormsModule));
				}
				return null;
			}
		}
		public FileDataPropertyEditor(Type objectType, IModelMemberViewItem model)
			: base(objectType, model) {
		}
		public void Setup(IObjectSpace objectSpace, XafApplication application) {
			this.objectSpace = objectSpace;
			this.application = application;
		}
		public new FileDataEdit Control {
			get { return (FileDataEdit)base.Control; }
		}
	}
}
