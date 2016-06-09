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
using System.Windows.Forms;
using DevExpress.ExpressApp.Templates.ActionControls;
using DevExpress.ExpressApp.Win.Templates.Bars.ActionControls;
using DevExpress.XtraBars;
namespace DevExpress.ExpressApp.Win.Design.Bars {
	public class SimpleActionControlsEditor : BaseActionControlsEditor {
		protected override bool IsValidBarItem(BarItem barItem) {
			return barItem.GetType().Equals(typeof(BarButtonItem));
		}
		protected override bool IsValidActionUiElement(IActionControl actionControl) {
			return actionControl is BarButtonItemSimpleActionControl;
		}
		protected override BarItem GetBarItem(IDataObject data) {
			return (BarItem)data.GetData(typeof(BarButtonItem));
		}
		protected override IActionControl CreateActionControlCore(BarItem barItem) {
			if(IsValidBarItem(barItem)) {
				return new BarButtonItemSimpleActionControl(barItem.Caption, (BarButtonItem)barItem);
			}
			throw new InvalidOperationException("Not supported bar item.");
		}
		protected override string DescriptionText {
			get { return "You can add / delete Simple and Popup Action controls and customize them. Drag BarButtonItem from the panel on the right to the Action Controls list to create an Action control for it."; }
		}
	}
}
