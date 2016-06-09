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
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.FileAttachments.Win {
	public class FileAttachmentListViewController : FileAttachmentControllerBase {
		private DevExpress.ExpressApp.Actions.SimpleAction addFromFileAction;
		private System.ComponentModel.IContainer components;
		public DevExpress.ExpressApp.Actions.SimpleAction AddFromFileAction {
			get { return addFromFileAction; }
		}
		private void grid_DragDrop(object sender, DragEventArgs e) {
			if(e.Effect == DragDropEffects.Copy && e.Data.GetDataPresent(DataFormats.FileDrop)) {
				AddFiles((string[])e.Data.GetData(DataFormats.FileDrop));
			}
		}
		private void grid_DragOver(object sender, DragEventArgs e) {
			if(addFromFileAction.Active && e.Data.GetDataPresent(DataFormats.FileDrop)) {
				e.Effect = DragDropEffects.Copy;
			}
			else {
				e.Effect = DragDropEffects.None;
			}
		}
		protected string[] FilterFileNames(string[] fileNames) {
			string fileTypesFilter = GetFileTypesFilter();
			if(string.IsNullOrEmpty(fileTypesFilter) || Regex.IsMatch(fileTypesFilter, @"\*\.\*")) {
				return fileNames;
			}
			MatchCollection fileFilters = Regex.Matches(fileTypesFilter.ToLower(), @"((\*\..[^\),\|].)|(\*\.\*))");
			List<string> result = new List<string>();
			foreach(string fileName in fileNames) {
				string extension = Path.GetExtension(fileName).ToLower();
				foreach(Match fileFilter in fileFilters) {
					if(fileFilter.Value.EndsWith(extension)) {
						result.Add(fileName);
						break;
					}
				}
			}
			return result.ToArray();
		}
		protected void AddFiles(string[] fileNames) {
			foreach(string fileName in FilterFileNames(fileNames)) {
				using(FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
					FileAttachmentAttribute fileAttachmentAttribute = ((ListView)View).ObjectTypeInfo.FindAttribute<FileAttachmentAttribute>();
					IMemberInfo memberDescriptor = ((ListView)View).ObjectTypeInfo.FindMember(fileAttachmentAttribute.FileDataPropertyName);
					object fileAttachment = ObjectSpace.CreateObject(((ListView)View).ObjectTypeInfo.Type);
					IFileData fileData = memberDescriptor.GetValue(fileAttachment)  as IFileData;
					if(fileData == null) {
						fileData = FileAttachmentsWindowsFormsModule.CreateFileData(ObjectSpace, memberDescriptor);
						memberDescriptor.SetValue(fileAttachment, fileData);
					}
					FileDataHelper.LoadFromStream(fileData, Path.GetFileName(fileName), stream, fileName);
					if(View.IsRoot) {
						ObjectSpace.CommitChanges();
					}
					((ListView)View).CollectionSource.Add(fileAttachment);
					if(!View.IsRoot) {
						ObjectSpace.SetModified(fileAttachment);
					}
				}
			}
		}
		private void View_ControlsCreated(object sender, EventArgs e) {
			Control control = ((ListEditor)((ListView)View).Editor).Control as Control;
			if(control != null) {
				control.DragDrop += new DragEventHandler(grid_DragDrop);
				control.DragOver += new DragEventHandler(grid_DragOver);
			}
		}
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.addFromFileAction = new SimpleAction(this.components);
			this.addFromFileAction.Caption = "Add From File...";
			this.addFromFileAction.Category = "ObjectsCreation";
			this.addFromFileAction.ToolTip = "Create a new record with the selected file attached";
			this.addFromFileAction.Id = "AddFromFile";
			this.addFromFileAction.ImageName = "MenuBar_AttachmentObject";
			this.addFromFileAction.Shortcut = "CtrlN";
			this.addFromFileAction.Execute += new SimpleActionExecuteEventHandler(this.addFromFileAction_OnExecute);
		}
		private void addFromFileAction_OnExecute(Object sender, SimpleActionExecuteEventArgs args) {
			AddFromFile();
		}
		private void UpdateActionState() {
			String diagnosticInfo;
			addFromFileAction.Enabled.SetItemValue("Security", DataManipulationRight.CanCreate(View, ((ListView)View).ObjectTypeInfo.Type, LinkToListViewController.FindCollectionSource(Frame), out diagnosticInfo));
			if(View != null && View.IsRoot) {
				addFromFileAction.Enabled.SetItemValue(FileAttachmentControllerBase.ActiveKeyObjectSpaceNotModified, !ObjectSpace.IsModified);
			}
			else {
				addFromFileAction.Enabled.RemoveItem(FileAttachmentControllerBase.ActiveKeyObjectSpaceNotModified);
			}
		}
		private void View_AllowNewChanged(object sender, EventArgs e) {
			UpdateActionState();
		}
		private void ObjectSpace_ModifiedChanged(object sender, EventArgs e) {
			UpdateActionState();
		}
		private void CollectionSource_CollectionLoaded(object sender, EventArgs e) {
			UpdateActionState();
		}
		private string GetFileTypesFilterValue(string className, string propertyName) {
			IModelClass modelClass = View.Model.Application.BOModel[className];
			return FileAttachmentsWindowsFormsModule.GetFileTypesFilter(modelClass, propertyName);
		}
		protected virtual string GetFileTypesFilter() {
			ListView view = View as ListView;
			string fileTypesFilter = GetFileTypesFilterValue(view.ObjectTypeInfo.FullName, GetFileAttachmentAttribute(view).FileDataPropertyName);
			if(!view.IsRoot && view.CollectionSource is PropertyCollectionSource) {
				string propertyFileTypesFilter = GetFileTypesFilterValue(((PropertyCollectionSource)(view.CollectionSource)).MasterObjectType.FullName, ((PropertyCollectionSource)(view.CollectionSource)).MemberInfo.Name);
				if(!string.IsNullOrEmpty(propertyFileTypesFilter)) {
					fileTypesFilter = propertyFileTypesFilter;
				}
			}
			return fileTypesFilter;
		}
		protected virtual void AddFromFile() {
			using(OpenFileDialog dialog = new OpenFileDialog()) {
				dialog.CheckFileExists = true;
				dialog.CheckPathExists = true;
				dialog.DereferenceLinks = true;
				dialog.Multiselect = true;
				dialog.Filter = GetFileTypesFilter();
				if(dialog.ShowDialog(Form.ActiveForm) == DialogResult.OK) {
					AddFiles(dialog.FileNames);
				}
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			if(Active) {
				UpdateActionState();
				((ListView)View).CollectionSource.CollectionChanged += new EventHandler(CollectionSource_CollectionLoaded);
				View.ControlsCreated += new EventHandler(View_ControlsCreated);
				View.AllowNewChanged += new EventHandler(View_AllowNewChanged);
				ObjectSpace.ModifiedChanged += new EventHandler(ObjectSpace_ModifiedChanged);
			}
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			DevExpress.ExpressApp.ListView view = (ListView)View;
			view.CollectionSource.CollectionChanged -= new EventHandler(CollectionSource_CollectionLoaded);
			view.ControlsCreated -= new EventHandler(View_ControlsCreated);
			View.AllowNewChanged -= new EventHandler(View_AllowNewChanged);
			ObjectSpace.ModifiedChanged -= new EventHandler(ObjectSpace_ModifiedChanged);
			if(view.Editor != null) {
				Control control = ((ListEditor)((ListView)View).Editor).Control as Control;
				if(control != null) {
					control.DragDrop -= new DragEventHandler(grid_DragDrop);
					control.DragOver -= new DragEventHandler(grid_DragOver);
				}
			}
		}
		public FileAttachmentListViewController()
			: base() {
			InitializeComponent();
			RegisterActions(components);
			this.TargetViewType = ViewType.ListView;
		}
	}
}
