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

using System;
using DevExpress.Snap;
using DevExpress.Snap.Core.Native;
using DevExpress.XtraBars;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Native.Lines;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.Office.Utils;
using DevExpress.Snap.Extensions.Native.ActionLists;
using DevExpress.Snap.Core.Fields;
using DevExpress.Snap.Extensions.UI;
using DevExpress.Snap.Core.Commands;
namespace DevExpress.Snap.Extensions.Native {
	public class SNSmartTagService : SmartTagServiceBase, IServiceProvider {
		readonly ActionListPopupControl actionListPopupControl;
		public SnapControl Control { get; set; }
		public SNSmartTagService() {
			actionListPopupControl = new ActionListPopupControl(this) {Dock = System.Windows.Forms.DockStyle.Fill};
		}
		internal void Init(SNPopupControlContainer container) {			
			container.Controls.Add(actionListPopupControl);
		}
		public void UpdatePopup() {
			System.ComponentModel.DXDisplayNameAttribute.UseResourceManager = true;
			BaseLineController[] controllers = GetActionListLineControllers();
			actionListPopupControl.SetLineControllers(controllers);			
		}
		BaseLineController[] GetActionListLineControllers() {			
			ListFieldSelectionController controller = new ListFieldSelectionController(Control.InnerControl.DocumentModel);
			SnapFieldInfo fieldInfo = controller.FindDataField();
			if (fieldInfo == null)
				return new BaseLineController[0];
			return LineControllerCreator.CreateMergeFieldLineControllers(fieldInfo, fieldInfo.ParsedInfo, this);
		}
		#region IServiceProvider Members
		public object GetService(Type serviceType) {
			if (serviceType == typeof(SNSmartTagService))
				return this;
			return Control != null ? Control.GetService(serviceType) : null;
		}
		#endregion
	}
}
