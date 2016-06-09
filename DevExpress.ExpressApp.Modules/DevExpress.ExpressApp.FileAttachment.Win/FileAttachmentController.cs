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
using System.ComponentModel;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.FileAttachments.Win {
	public class FileAttachmentController : FileAttachmentControllerBase {
		private ISelectionContext selectionContext;
		private PropertyObserverBase fileAttachmentPropertyObserver;
		private System.ComponentModel.IContainer components;
		private DevExpress.ExpressApp.Actions.SimpleAction openAction;
		private DevExpress.ExpressApp.Actions.SimpleAction saveToAction;
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.openAction = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
			this.saveToAction = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
			this.openAction.Caption = "Open...";
			this.openAction.Category = "OpenObject";
			this.openAction.ConfirmationMessage = "";
			this.openAction.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireSingleObject;
			this.openAction.ToolTip = "Open the attached file with an external application";
			this.openAction.Id = "Open";
			this.openAction.ImageName = "MenuBar_Open";
			this.openAction.Shortcut = "CtrlO";
			this.openAction.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.openAction_OnExecute);
			this.saveToAction.Caption = "Save As...";
			this.saveToAction.Category = "Save";
			this.saveToAction.ConfirmationMessage = "";
			this.saveToAction.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireMultipleObjects;
			this.saveToAction.ToolTip = "Save file(s) attached to the selected record(s)";
			this.saveToAction.Id = "SaveTo";
			this.saveToAction.ImageName = "MenuBar_SaveTo";
			this.saveToAction.Shortcut = "CtrlShiftS";
			this.saveToAction.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.saveToAction_OnExecute);
		}
		private void selectionContext_SelectionChanged(object sender, EventArgs e) {
			UpdateActionState();
		}
		private void fileAttachmentPropertyObserver_PropertyChanged(object sender, PropertyChangedEventArgs e) {
			UpdateActionState();
		}
		private void ObjectSpace_ModifiedChanged(object sender, EventArgs e) {
			UpdateActionState();
		}
		private List<IFileData> GetSelectedObjectsFileData() {
			List<IFileData> fileDataList = new List<IFileData>();
			if(fileAttachmentPropertyObserver != null) {
				foreach(object obj in selectionContext.SelectedObjects) {
					IFileData fileData = (IFileData)fileAttachmentPropertyObserver.GetPropertyValue(obj);
					if(!FileDataHelper.IsFileDataEmpty(fileData)) {
						fileDataList.Add(fileData);
					}
				}
			}
			return fileDataList;
		}
		protected virtual void UpdateActionState() {
			saveToAction.Active.SetItemValue("Has permission", DataManipulationRight.CanRead(((ObjectView)View).ObjectTypeInfo.Type, null, null, LinkToListViewController.FindCollectionSource(Frame), View.ObjectSpace));
			openAction.Active.SetItemValue("Has permission", DataManipulationRight.CanRead(((ObjectView)View).ObjectTypeInfo.Type, null, null, LinkToListViewController.FindCollectionSource(Frame), View.ObjectSpace));
			openAction.Enabled.SetItemValue(ActiveKeyAttachmentAssigned, !FileDataHelper.IsFileDataEmpty((IFileData)fileAttachmentPropertyObserver.PropertyValue));
			saveToAction.Enabled.SetItemValue(ActiveKeyAttachmentAssigned, (GetSelectedObjectsFileData().Count > 0));
			ListView listView = View as ListView;
			if(listView != null && listView.IsRoot) {
				openAction.Enabled.SetItemValue(FileAttachmentControllerBase.ActiveKeyObjectSpaceNotModified, !ObjectSpace.IsModified);
				saveToAction.Enabled.SetItemValue(FileAttachmentControllerBase.ActiveKeyObjectSpaceNotModified, !ObjectSpace.IsModified);
			}
			else {
				openAction.Enabled.RemoveItem(FileAttachmentControllerBase.ActiveKeyObjectSpaceNotModified);
				saveToAction.Enabled.RemoveItem(FileAttachmentControllerBase.ActiveKeyObjectSpaceNotModified);
			}
		}
		private void openAction_OnExecute(Object sender, SimpleActionExecuteEventArgs args) {
			Open(args);
		}
		private void saveToAction_OnExecute(Object sender, SimpleActionExecuteEventArgs args) {
			SaveTo(args);
		}
		protected virtual ISelectionContext GetSelectionContext() {
			return View as ISelectionContext;
		}
		protected virtual void Open(SimpleActionExecuteEventArgs args) {
			if(fileAttachmentPropertyObserver.PropertyValue is IFileData) {
				FileDataManager.Open(fileAttachmentPropertyObserver.PropertyValue as IFileData);
			}
		}
		protected virtual void SaveTo(SimpleActionExecuteEventArgs args) {
			FileDataManager.SaveFiles(GetSelectedObjectsFileData());
		}
		protected override void OnActivated() {
			base.OnActivated();
			FileAttachmentAttribute attribute = GetFileAttachmentAttribute(View);
			if(attribute != null) {
				selectionContext = GetSelectionContext();
				fileAttachmentPropertyObserver = new PropertyObserverBase(selectionContext, attribute.FileDataPropertyName);
				fileAttachmentPropertyObserver.PropertyChanged += new PropertyChangedEventHandler(fileAttachmentPropertyObserver_PropertyChanged);
				selectionContext.SelectionChanged += new EventHandler(selectionContext_SelectionChanged);
				ObjectSpace.ModifiedChanged += new EventHandler(ObjectSpace_ModifiedChanged);
				UpdateActionState();
			}
		}
		protected override void OnDeactivated() {
			if(selectionContext != null) {
				selectionContext.SelectionChanged -= new EventHandler(selectionContext_SelectionChanged);
				selectionContext = null;
			}
			if(fileAttachmentPropertyObserver != null) {
				fileAttachmentPropertyObserver.PropertyChanged -= new PropertyChangedEventHandler(fileAttachmentPropertyObserver_PropertyChanged);
				fileAttachmentPropertyObserver.Dispose();
				fileAttachmentPropertyObserver = null;
			}
			ObjectSpace.ModifiedChanged -= new EventHandler(ObjectSpace_ModifiedChanged);
			base.OnDeactivated();
		}
		public FileAttachmentController()
			: base() {
			InitializeComponent();
			RegisterActions(components);
		}
		public DevExpress.ExpressApp.Actions.SimpleAction OpenAction {
			get { return openAction; }
		}
		public DevExpress.ExpressApp.Actions.SimpleAction SaveToAction {
			get { return saveToAction; }
		}
	}
}
