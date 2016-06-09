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
using System.Windows.Forms;
using DevExpress.Utils.CodedUISupport;
namespace DevExpress.XtraVerticalGrid.CodedUISupport
{
	class CodedUIMessagesHandler
	{
		protected static bool controlsAddedToDictionary;
		protected static DXControlsSupported GetCodedUIEnum(object control) {
			if(!controlsAddedToDictionary) {
				ControlsToCodedUIEnum.Add(typeof(PropertyGridControl), DXControlsSupported.PropertyGrid);
				ControlsToCodedUIEnum.Add(typeof(VGridControl), DXControlsSupported.VerticalGrid);
				ControlsToCodedUIEnum.Add(typeof(DevExpress.XtraVerticalGrid.Rows.VGridCustomizationForm), DXControlsSupported.VerticalGridCustomizationForm);
				ControlsToCodedUIEnum.Add(typeof(DevExpress.XtraVerticalGrid.Rows.VGridCustomizationForm.RowsListBox), DXControlsSupported.VerticalGridCustomizationListBox);
				ControlsToCodedUIEnum.Add(typeof(PropertyDescriptionControl), DXControlsSupported.PropertyDescriptionControl);
				controlsAddedToDictionary = true;
			}
			return ControlsToCodedUIEnum.GetCodedUIEnum(control);
		}
		public static void ProcessCodedUIMessage(ref Message message, object sender) {
			if(CodedUIUtils.IsIdentifyControlMessage(ref message, sender))
				message.Result = new IntPtr((int)GetCodedUIEnum(sender));
		}
	}
}
