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

extern alias Platform;
using Platform::System.Windows.Data;
using Platform::DevExpress.Xpf.Ribbon.Design;
using Platform::DevExpress.Xpf.Scheduler;
using Microsoft.Windows.Design.Model;
using Platform::DevExpress.Xpf.Core.Design;
namespace DevExpress.Xpf.Scheduler.Design {
	class SchedulerCommandRibbonCreator : CommandRibbonCreator {
		ModelItem ribbonControl;
		public override System.Type CommandsType { get { return typeof(SchedulerUICommand); } }
		public override System.Type StringIdConverter { get { return typeof(SchedulerControlStringIdConverter); } }
		protected override ModelItem CreateRibbonControl() {
			this.ribbonControl = base.CreateRibbonControl();
			if (ribbonControl != null)
				CreatorHelper.EnsureControlHasName(ribbonControl, Root);
			return ribbonControl;
		}
		protected override void AppendBarItems(Microsoft.Windows.Design.Model.ModelItemCollection target, Microsoft.Windows.Design.Model.ModelItem masterControl, Platform.DevExpress.Xpf.Core.Design.BarInfo[] barInfos) {
			base.AppendBarItems(target, masterControl, barInfos);
			if (this.ribbonControl != null)
				BindRibbonToMasterControl(ribbonControl, masterControl);
		}
		protected internal virtual void BindRibbonToMasterControl(ModelItem barManager, ModelItem masterControl) {
			ModelProperty barManagerProperty = masterControl.Properties["Ribbon"];
			if (!barManagerProperty.IsSet) {
				ModelItem bindingModelItem = ModelFactory.CreateItem(barManager.Context, typeof(Binding));
				bindingModelItem.Properties["ElementName"].SetValue(barManager.Name);
				bindingModelItem.Properties["Mode"].SetValue(BindingMode.OneTime);
				barManagerProperty.SetValue(bindingModelItem);
			}
		}
	}
}
