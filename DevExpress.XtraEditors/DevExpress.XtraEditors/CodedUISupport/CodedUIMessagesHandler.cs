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
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Popup;
namespace DevExpress.XtraEditors.CodedUISupport {
	class CodedUIMessagesHandler {
		protected static bool controlsAddedToDictionary;
		protected static DXControlsSupported GetCodedUIEnum(object control) {
			if(!controlsAddedToDictionary) {
				ControlsToCodedUIEnum.Add(typeof(FontEdit), DXControlsSupported.FontEdit);
				ControlsToCodedUIEnum.Add(typeof(MRUEdit), DXControlsSupported.MRUEdit);
				ControlsToCodedUIEnum.Add(typeof(ImageComboBoxEdit), DXControlsSupported.ImageComboBoxEdit);
				ControlsToCodedUIEnum.Add(typeof(ComboBoxEdit), DXControlsSupported.ComboBoxEdit);
				ControlsToCodedUIEnum.Add(typeof(CalcEdit), DXControlsSupported.CalcEdit);
				ControlsToCodedUIEnum.Add(typeof(CheckedComboBoxEdit), DXControlsSupported.CheckedComboBoxEdit);
				ControlsToCodedUIEnum.Add(typeof(DateEdit), DXControlsSupported.DateEdit);
				ControlsToCodedUIEnum.Add(typeof(MemoExEdit), DXControlsSupported.MemoExEdit);
				ControlsToCodedUIEnum.Add(typeof(ColorEdit), DXControlsSupported.ColorEdit);
				ControlsToCodedUIEnum.Add(typeof(LookUpEditBase), DXControlsSupported.LookUpEditBase);
				ControlsToCodedUIEnum.Add(typeof(ImageEdit), DXControlsSupported.ImageEdit);
				ControlsToCodedUIEnum.Add(typeof(PopupBaseEdit), DXControlsSupported.PopupBaseEdit);
				ControlsToCodedUIEnum.Add(typeof(TimeEdit), DXControlsSupported.TimeEdit);
				ControlsToCodedUIEnum.Add(typeof(SpinEdit), DXControlsSupported.SpinEdit);
				ControlsToCodedUIEnum.Add(typeof(HyperLinkEdit), DXControlsSupported.HyperLinkEdit);
				ControlsToCodedUIEnum.Add(typeof(ButtonEdit), DXControlsSupported.ButtonEdit);
				ControlsToCodedUIEnum.Add(typeof(TextEdit), DXControlsSupported.TextEdit);
				ControlsToCodedUIEnum.Add(typeof(CheckEdit), DXControlsSupported.CheckEdit);
				ControlsToCodedUIEnum.Add(typeof(MaskBox), DXControlsSupported.MaskBox);
				ControlsToCodedUIEnum.Add(typeof(SimpleButton), DXControlsSupported.SimpleButton);
				ControlsToCodedUIEnum.Add(typeof(CheckButton), DXControlsSupported.CheckButton);
				ControlsToCodedUIEnum.Add(typeof(DropDownButton), DXControlsSupported.DropDownButton);
				ControlsToCodedUIEnum.Add(typeof(PictureEdit), DXControlsSupported.PictureEdit);
				ControlsToCodedUIEnum.Add(typeof(LabelControl), DXControlsSupported.LabelControl);
				ControlsToCodedUIEnum.Add(typeof(Controls.CalendarControlBase), DXControlsSupported.DateEditCalendarBase);
				ControlsToCodedUIEnum.Add(typeof(ProgressBarControl), DXControlsSupported.ProgressBarControl);
				ControlsToCodedUIEnum.Add(typeof(MarqueeProgressBarControl), DXControlsSupported.MarqueeProgressBarControl);
				ControlsToCodedUIEnum.Add(typeof(SimplePopupBaseForm), DXControlsSupported.PopupBaseForm);
				ControlsToCodedUIEnum.Add(typeof(BaseListBoxControl), DXControlsSupported.BaseListBoxControl);
				ControlsToCodedUIEnum.Add(typeof(CheckedListBoxControl), DXControlsSupported.CheckedListBoxControl);
				ControlsToCodedUIEnum.Add(typeof(TrackBarControl), DXControlsSupported.TrackBarControl);
				ControlsToCodedUIEnum.Add(typeof(RangeTrackBarControl), DXControlsSupported.RangeTrackBarControl);
				ControlsToCodedUIEnum.Add(typeof(ZoomTrackBarControl), DXControlsSupported.ZoomTrackBarControl);
				ControlsToCodedUIEnum.Add(typeof(FilterControl), DXControlsSupported.FilterControl);
				ControlsToCodedUIEnum.Add(typeof(NavigatorBase), DXControlsSupported.NavigatorBase);
				ControlsToCodedUIEnum.Add(typeof(RadioGroup), DXControlsSupported.RadioGroup);
				ControlsToCodedUIEnum.Add(typeof(Popup.ColorCellsControl), DXControlsSupported.ColorCellsControl);
				ControlsToCodedUIEnum.Add(typeof(XtraTab.XtraTabPage), DXControlsSupported.XtraTabPage);
				ControlsToCodedUIEnum.Add(typeof(XtraTab.XtraTabControl), DXControlsSupported.XtraTabControl);
				ControlsToCodedUIEnum.Add(typeof(XtraMessageBoxForm), DXControlsSupported.XtraMessageBoxForm);
				ControlsToCodedUIEnum.Add(typeof(TokenEdit), DXControlsSupported.TokenEdit);
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
}
