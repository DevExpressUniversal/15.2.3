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

using System.ComponentModel.Design;
using DevExpress.Utils.Design;
using DevExpress.XtraBars.ToastNotifications;
namespace DevExpress.XtraBars.Design {
	public class ToastNotificationsManagerDesigner : BaseComponentDesigner {
		protected ToastNotificationsManager Manager {
			get { return Component as ToastNotificationsManager; }
		}
		public override void InitializeNewComponent(System.Collections.IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			Manager.ApplicationId = System.Guid.NewGuid().ToString();
			EnvDTE.ProjectItem item = GetService(typeof(EnvDTE.ProjectItem)) as EnvDTE.ProjectItem;
			if(item != null) 
				Manager.ApplicationName = ProjectHelper.GetAssemblyName(item.ContainingProject);
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			if(AllowDesigner)
				list.Add(new ToastNotificationsManagerDesignerActionList(this));
			base.RegisterActionLists(list);
		}
		internal void AddNotification(ToastNotificationTemplate template) {
			Manager.Notifications.Add(CreateDefaultNotification(template));
		}
		internal static ToastNotification CreateDefaultNotification(ToastNotificationTemplate template) {
			return new ToastNotification()
			{
				ID = System.Guid.NewGuid().ToString(),
				Body = "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
				Body2 = "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
				Header = "Pellentesque lacinia tellus eget volutpat",
				Template = template
			};
		}
	}
	public class ToastNotificationsManagerDesignerActionList : DesignerActionList {
		ToastNotificationsManagerDesigner designer;
		public ToastNotificationsManagerDesignerActionList(ToastNotificationsManagerDesigner designer)
			: base(designer.Component) {
			this.designer = designer;
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection res = new DesignerActionItemCollection();
			if(designer != null && designer.Component != null) {
				res.Add(new DesignerActionHeaderItem("Notifications", "Notifications"));
				res.Add(new DesignerActionMethodItem(this, "AddNotification", "Add notification", "Notifications"));
				res.Add(new DesignerActionMethodItem(this, "EditNotifications", "Edit Notifications ... ", "Notifications", true));
				res.Add(new DesignerActionMethodItem(this, "About", "About", null, true));
			}
			return res;
		}
		public void About() {
			ToastNotificationsManager.About();
		}
		public void EditNotifications() {
			EditorContextHelper.EditValue(designer, designer.Component, "Notifications");
		}
		public void AddNotification() {
			designer.AddNotification(ToastNotificationTemplate.Text01);
		}
	}
}
