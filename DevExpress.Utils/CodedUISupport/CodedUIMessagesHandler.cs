﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.XtraEditors;
namespace DevExpress.Utils.CodedUISupport {
	class CodedUIMessagesHandler {
		protected static bool controlsAddedToDictionary;
		protected static DXControlsSupported GetCodedUIEnum(object control) {
			if(!controlsAddedToDictionary) {
				ControlsToCodedUIEnum.Add(typeof(SplitContainerControl), DXControlsSupported.SplitContainerControl);
				ControlsToCodedUIEnum.Add(typeof(PanelControl), DXControlsSupported.PanelControl);
				ControlsToCodedUIEnum.Add(typeof(XtraScrollableControl), DXControlsSupported.XtraScrollableControl);
				ControlsToCodedUIEnum.Add(typeof(ScrollBarBase), DXControlsSupported.ScrollBarBase);
				ControlsToCodedUIEnum.Add(typeof(XtraForm), DXControlsSupported.XtraForm);
				ControlsToCodedUIEnum.Add(typeof(SplitterControl), DXControlsSupported.SplitterControl);
				ControlsToCodedUIEnum.Add(typeof(DevExpress.DocumentView.Controls.ViewControl), DXControlsSupported.PrintViewControl);
				ControlsToCodedUIEnum.Add(typeof(DevExpress.XtraEditors.Internal.FloatingScrollbarBase), DXControlsSupported.FloatingScrollBar);
				controlsAddedToDictionary = true;
			}
			return ControlsToCodedUIEnum.GetCodedUIEnum(control);
		}
		public static void ProcessCodedUIMessage(ref Message message, object sender) {
			if(CodedUIUtils.IsIdentifyControlMessage(ref message, sender)) {
				message.Result = new IntPtr((int)GetCodedUIEnum(sender));
			}
		}
	}
	public class ControlsToCodedUIEnum {
		protected static Dictionary<Type, DXControlsSupported> controlsToCodedUIEnumDictionary;
		static ControlsToCodedUIEnum() {
			controlsToCodedUIEnumDictionary = new Dictionary<Type, DXControlsSupported>();
		}
		protected static DXControlsSupported GetControlCodedUIEnum(Type controlType) {
			DXControlsSupported result = DXControlsSupported.Default;
			if(controlType != typeof(Control) && controlType != typeof(NativeWindow) && controlType != typeof(object))
				if(!controlsToCodedUIEnumDictionary.TryGetValue(controlType, out result))
					result = GetControlCodedUIEnum(controlType.BaseType);
			return result;
		}
		public static void Add(Type controlType, DXControlsSupported enumValue) {
			if(!controlsToCodedUIEnumDictionary.ContainsKey(controlType))
				controlsToCodedUIEnumDictionary.Add(controlType, enumValue);
		}
		public static DXControlsSupported GetCodedUIEnum(object control) {
			if(control == null)
				return DXControlsSupported.NotSupported;
			else return GetControlCodedUIEnum(control.GetType());
		}
	}
}
