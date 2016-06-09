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

using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base.General;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
namespace DevExpress.ExpressApp.Notifications {
	[DXToolboxItem(true)]
	[DevExpress.Utils.ToolboxTabName(XafAssemblyInfo.DXTabXafModules)]
	[Description("Provides notifications functionality in XAF applications.")]
	[ToolboxBitmap(typeof(NotificationsModule), "Resources.Toolbox_Module_Notifications.ico")]
	public sealed class NotificationsModule : ModuleBase, INotificationsServiceOwner {
		private TimeSpan defaultNotificationRefreshInterval = TimeSpan.FromMinutes(5);
		private TimeSpan defaultNotificationStartDelay = TimeSpan.FromSeconds(5);
		private DefaultNotificationsProvider defaultNotificationsProvider;
		private void application_SetupComplete(object sender, EventArgs e) {
			Application.SetupComplete -= application_SetupComplete;
			InitializeDefaultNotificationsProvider();
			NotificationsService = new NotificationsService(defaultNotificationsProvider);
		}
		private void InitializeDefaultNotificationsProvider() {
			if(defaultNotificationsProvider == null) {
				defaultNotificationsProvider = new DefaultNotificationsProvider(Application);
			}
		}
		protected override IEnumerable<Type> GetRegularTypes() {
			return null;
		}
		protected override IEnumerable<Type> GetDeclaredExportedTypes() {
			return new Type[] {
				typeof(CommonNotificationItem),
				typeof(Notification),
				typeof(NotificationsObject)
			};
		}
		protected override IEnumerable<Type> GetDeclaredControllerTypes() {
			return new Type[] {
				typeof(UpdateNotificationsSourceListViewController),
				typeof(NotificationsOnCommitController),
				typeof(NotificationsMessageListViewController),
				typeof(NotificationsDialogViewController) 
			};
		}
		public NotificationsModule() {
			NotificationsRefreshInterval = defaultNotificationRefreshInterval;
			NotificationsStartDelay = defaultNotificationStartDelay;
			ShowNotificationsWindow = true;
		}
		public override void Setup(XafApplication application) {
			base.Setup(application);
			application.SetupComplete += application_SetupComplete;
		}
		public override void CustomizeTypesInfo(DC.ITypesInfo typesInfo) {
			base.CustomizeTypesInfo(typesInfo);
			TypeInfo typeInfo = (TypeInfo)XafTypesInfo.Instance.FindTypeInfo(typeof(Notification));
			typeInfo.KeyMember = typeInfo.FindMember("ID");
			typeInfo.FindMember("ID").IsKey = true;
		}
		protected override void Dispose(bool disposing) {
			if(DefaultNotificationsProvider != null) {
				DefaultNotificationsProvider.Dispose();
			}
			if(NotificationsService != null) {
				NotificationsService.Dispose();
			}
			base.Dispose(disposing);
		}
#if !SL
	[DevExpressExpressAppNotificationsLocalizedDescription("NotificationsModuleNotificationsRefreshInterval")]
#endif
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Category("Settings")]
		public TimeSpan NotificationsRefreshInterval {
			get;
			set;
		}
#if !SL
	[DevExpressExpressAppNotificationsLocalizedDescription("NotificationsModuleNotificationsStartDelay")]
#endif
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Category("Settings")]
		public TimeSpan NotificationsStartDelay {
			get;
			set;
		}
#if !SL
	[DevExpressExpressAppNotificationsLocalizedDescription("NotificationsModuleShowNotificationsWindow")]
#endif
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Category("Settings")]
		public bool ShowNotificationsWindow {
			get;
			set;
		}
		[Browsable(false)]
		public NotificationsService NotificationsService { get; private set; }
		INotificationsService INotificationsServiceOwner.NotificationsService {
			get { return NotificationsService; }
		}
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DefaultNotificationsProvider DefaultNotificationsProvider {
			get {
				return defaultNotificationsProvider;
			}
			set {
				defaultNotificationsProvider = value;
				if(NotificationsService != null) {
					NotificationsService.UpdateDefaultNotificationsProvider(defaultNotificationsProvider);
				}
			}
		}
	}
}
