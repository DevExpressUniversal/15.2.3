#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Reflection;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Commands;
using DevExpress.Services.Internal;
using DevExpress.Utils.Commands;
using DevExpress.Utils.KeyboardHandler;
using DevExpress.DashboardWin.ServiceModel;
namespace DevExpress.DashboardWin.Native {
	public class DashboardUserCommandControl : DashboardUserControl, ICommandAwareControl<DashboardCommandId>, IServiceProvider {
		readonly Dictionary<DashboardCommandId, ConstructorInfo> commands = new Dictionary<DashboardCommandId, ConstructorInfo>();
		CommandBasedKeyboardHandler<DashboardCommandId> ICommandAwareControl<DashboardCommandId>.KeyboardHandler { get { return null; } }
		protected internal virtual IServiceProvider ServiceProvider { get { return null; } }
		event EventHandler ICommandAwareControl<DashboardCommandId>.BeforeDispose { add { } remove { } }
		event EventHandler ICommandAwareControl<DashboardCommandId>.UpdateUI { add { UpdateUI += value; } remove { UpdateUI -= value; } }
		EventHandler UpdateUI;
		internal void OnUpdateUI() {
			bool allowUpdate = true;
			if(ServiceProvider != null) {
				IDesignerUpdateService designerUpdateService = ServiceProvider.RequestService<IDesignerUpdateService>();
				if(designerUpdateService != null)
					allowUpdate = !designerUpdateService.Suspended;
			}
			if(UpdateUI != null && allowUpdate)
				UpdateUI(this, EventArgs.Empty);
		}
		protected void AddCommand(DashboardCommandId commandId, Type commandType) {
			ConstructorInfo ci = commandType.GetConstructor(new Type[] { GetType() });
			if(ci != null)
				commands.Add(commandId, ci);
			else
				throw new Exception(String.Format("Cannot find a constructor with a {0} type parameter in the {1} class", GetType().Name, commandType));			
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
		}
		protected virtual void FocusCommandAwareControl() {
		}
		void ICommandAwareControl<DashboardCommandId>.CommitImeContent() {
		}
		Command ICommandAwareControl<DashboardCommandId>.CreateCommand(DashboardCommandId id) {
			ConstructorInfo ci;
			return (commands.TryGetValue(id, out ci) && ci != null) ? ci.Invoke(new object[] { this }) as Command : null;
		}
		void ICommandAwareControl<DashboardCommandId>.Focus() {
			FocusCommandAwareControl();
		}
		bool ICommandAwareControl<DashboardCommandId>.HandleException(Exception e) {
			return false;
		}
		object IServiceProvider.GetService(Type serviceType) {
			if(ServiceProvider != null)
				return ServiceProvider.GetService(serviceType);
			return null;
		}
	}
}
