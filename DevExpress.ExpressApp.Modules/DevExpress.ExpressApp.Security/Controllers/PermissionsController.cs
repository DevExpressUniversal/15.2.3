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
using System.Security;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils.Reflection;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.Security;
namespace DevExpress.ExpressApp.Security {
	public class PermissionsController : ObjectViewController {
		private Type GetCurrentPermissionType() {
			Type result = null;
			if((View != null) && (View.CurrentObject is IPersistentPermission)) {
				IPersistentPermission persistentPermission = (IPersistentPermission)View.CurrentObject;
				if(persistentPermission.Permission != null) {
					result = persistentPermission.Permission.GetType();
				}
			}
			return result;
		}
		private void ForceEventsRaising(NewObjectViewController newObjectViewController) {
			try {
				newObjectViewController.Active.SetItemValue("ForceEventsRaising", false);
			}
			finally {
				newObjectViewController.Active.RemoveItem("ForceEventsRaising");
			}
		}
		private void newObjectViewController_CollectCreatableItemTypes(object sender, CollectTypesEventArgs e) {
			e.Types.Clear();
			Type currentPermissionType = GetCurrentPermissionType();
			ITypeInfo baseInfo = XafTypesInfo.Instance.FindTypeInfo(typeof(PermissionBase));
			XafTypesInfo.Instance.LoadTypes(baseInfo.Type.Assembly);
			foreach(ITypeInfo typeInfo in ReflectionHelper.FindTypeDescendants(baseInfo)) {
				if(((View is DetailView) && (typeInfo.Type != currentPermissionType) && !typeInfo.IsAbstract)) {
					if(DataManipulationRight.HasPermissionTo(typeInfo.Type, ObjectAccess.Create | ObjectAccess.Write, null)) {
						e.Types.Add(typeInfo.Type);
					}
				}
			}
			if(CollectCreatablePermissionTypes != null) {
				CollectCreatablePermissionTypes(this, new CollectTypesEventArgs(e.Types));
			}
		}
		private void newObjectViewController_CollectDescendantTypes(object sender, CollectTypesEventArgs e) {
			e.Types.Clear();
			CollectDescendantTypes(e.Types);
			if(CollectDescendantPermissionTypes != null) {
				CollectDescendantPermissionTypes(this, new CollectTypesEventArgs(e.Types));
			}
		}
		internal void CollectDescendantTypes(ICollection<Type> resultTypes) {
			Type currentPermissionType = GetCurrentPermissionType();
			ITypeInfo baseInfo = XafTypesInfo.Instance.FindTypeInfo(typeof(PermissionBase));
			XafTypesInfo.Instance.LoadTypes(baseInfo.Type.Assembly);
			System.Collections.Generic.List<Type> foundTypes = new System.Collections.Generic.List<Type>();
			foreach(ITypeInfo typeInfo in ReflectionHelper.FindTypeDescendants(baseInfo)) {
				if(!typeInfo.Type.IsAbstract) {
					if((View is ListView) || ((View is DetailView) && (typeInfo.Type == currentPermissionType))) {
						if(DataManipulationRight.HasPermissionTo(typeInfo.Type, ObjectAccess.Create | ObjectAccess.Write, null)) {
						if(typeInfo.Type == typeof(ObjectAccessPermission)) {
							resultTypes.Add(typeInfo.Type);
						}
						else {
							foundTypes.Add(typeInfo.Type);
							}
						}
					}
				}
			}
			foreach(Type foundType in foundTypes) {
				resultTypes.Add(foundType);
			}
		}
		private void newObjectViewController_ObjectCreating(object sender, ObjectCreatingEventArgs e) {
			IPersistentPermission persistentPermission = (IPersistentPermission)e.ObjectSpace.CreateObject(((ObjectView)View).ObjectTypeInfo.Type);
			persistentPermission.Permission = (IPermission)TypeHelper.CreateInstance(e.ObjectType);
			e.NewObject = persistentPermission;
		}
		private void ObjectSpace_ObjectSaving(object sender, ObjectManipulatingEventArgs e) {
			if(e.Object is IPersistentPermission && !((IObjectSpace)sender).IsDeletedObject(e.Object)) {
				ObjectAccessPermission objectAccessPermission = ((IPersistentPermission)e.Object).Permission as ObjectAccessPermission;
				if((objectAccessPermission != null) && objectAccessPermission.IsEmpty) {
					throw new UserFriendlyException(UserVisibleExceptionId.ObjectAccessPermissionCannotBeEmpty);
				}
			}
		}
		private void ObjectSpace_ObjectChanged(object sender, ObjectChangedEventArgs e) {
			if(!ObjectSpace.IsModified && e.Object is IPersistentPermission && e.PropertyName == "Permission") {
				ObjectSpace.SetModified(e.Object);
			}
		}
		private void windowTemplateController_CustomizeWindowCaption(object sender, CustomizeWindowCaptionEventArgs e) {
			DetailView detailView = (DetailView)View;
			DetailPropertyEditor detailPropertyEditor = detailView.FindItem("Permission") as DetailPropertyEditor;
			if((detailPropertyEditor != null) && (detailPropertyEditor.DetailView != null)) {
				e.WindowCaption.FirstPart = detailPropertyEditor.DetailView.GetCurrentObjectCaption();
				e.WindowCaption.SecondPart = detailPropertyEditor.DetailView.Caption;
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			ObjectSpace.ObjectSaving += new EventHandler<ObjectManipulatingEventArgs>(ObjectSpace_ObjectSaving);
			ObjectSpace.ObjectChanged += new EventHandler<ObjectChangedEventArgs>(ObjectSpace_ObjectChanged);
			if(Frame != null) {
				NewObjectViewController newObjectViewController = Frame.GetController<NewObjectViewController>();
				if(newObjectViewController != null) {
					newObjectViewController.ObjectCreating += new EventHandler<ObjectCreatingEventArgs>(newObjectViewController_ObjectCreating);
					newObjectViewController.CollectCreatableItemTypes += new EventHandler<CollectTypesEventArgs>(newObjectViewController_CollectCreatableItemTypes);
					newObjectViewController.CollectDescendantTypes += new EventHandler<CollectTypesEventArgs>(newObjectViewController_CollectDescendantTypes);
					ForceEventsRaising(newObjectViewController);
				}
			}
			if(View is DetailView && Frame.GetController<WindowTemplateController>() != null) {
				Frame.GetController<WindowTemplateController>().CustomizeWindowCaption += new EventHandler<CustomizeWindowCaptionEventArgs>(windowTemplateController_CustomizeWindowCaption);
			}
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			ObjectSpace.ObjectSaving -= new EventHandler<ObjectManipulatingEventArgs>(ObjectSpace_ObjectSaving);
			ObjectSpace.ObjectChanged -= new EventHandler<ObjectChangedEventArgs>(ObjectSpace_ObjectChanged);
			NewObjectViewController newObjectViewController = Frame.GetController<NewObjectViewController>();
			newObjectViewController.ObjectCreating -= new EventHandler<ObjectCreatingEventArgs>(newObjectViewController_ObjectCreating);
			newObjectViewController.CollectCreatableItemTypes -= new EventHandler<CollectTypesEventArgs>(newObjectViewController_CollectCreatableItemTypes);
			newObjectViewController.CollectDescendantTypes -= new EventHandler<CollectTypesEventArgs>(newObjectViewController_CollectDescendantTypes);
			if(View is DetailView) {
				Frame.GetController<WindowTemplateController>().CustomizeWindowCaption -= new EventHandler<CustomizeWindowCaptionEventArgs>(windowTemplateController_CustomizeWindowCaption);
			}
		}
		public PermissionsController() {
			TypeOfView = typeof(ObjectView);
			TargetViewNesting = Nesting.Any;
			TargetObjectType = typeof(IPersistentPermission);
		}
		public event EventHandler<CollectTypesEventArgs> CollectDescendantPermissionTypes;
		public event EventHandler<CollectTypesEventArgs> CollectCreatablePermissionTypes;
	}
}
