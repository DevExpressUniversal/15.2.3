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
using Platform::DevExpress.XtraSpreadsheet;
using Platform::DevExpress.Xpf.Core.Design;
using Platform::DevExpress.Xpf.Ribbon.Design;
using Platform::DevExpress.Xpf.Spreadsheet;
using Microsoft.Windows.Design.Model;
namespace DevExpress.Xpf.Spreadsheet.Design {
	public class SpreadsheetCommandRibbonCreator : CommandRibbonCreator {
		ModelItem ribbonControl;
		public override System.Type CommandsType { get { return typeof(SpreadsheetUICommand); } }
		public override System.Type StringIdConverter { get { return typeof(SpreadsheetStringIdConverter); } }
		public override bool GenerateCommandParameter { get { return false; } }
		protected override ModelItem CreateRibbonControl() {
			this.ribbonControl = base.CreateRibbonControl();
			if (ribbonControl != null)
				CreatorHelper.EnsureControlHasName(ribbonControl, Root);
			return ribbonControl;
		}
		protected override void AppendBarItems(ModelItemCollection target, ModelItem masterControl, BarInfo[] barInfos) {
			base.AppendBarItems(target, masterControl, barInfos);
			if (ribbonControl != null)
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
		protected override BarInfoItems GetBarInfoItems(BarInfo barInfo) {
			CompositeBarInfo compositeBarInfo = barInfo as CompositeBarInfo;
			if (compositeBarInfo != null)
				return compositeBarInfo.RibbonItems;
			return base.GetBarInfoItems(barInfo);
		}
	}
}
