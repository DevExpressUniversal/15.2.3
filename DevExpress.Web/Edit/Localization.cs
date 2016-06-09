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
using DevExpress.Utils.Localization;
using DevExpress.Web.Localization;
using DevExpress.Utils.Localization.Internal;
using DevExpress.Web.Internal;
using System.Collections.Generic;
namespace DevExpress.Web.Localization {
	public enum ASPxEditorsStringId {
		DefaultErrorText, RegExValidationErrorText, RequiredFieldErrorText,
		BinaryImage_Cancel, BinaryImage_Clear, BinaryImage_DropZone, BinaryImage_Empty, BinaryImage_Uploading, BinaryImage_UploadFile,
		Calendar_Today, Calendar_Clear, Calendar_OK, Calendar_Cancel,
		ColorEdit_CustomColor, ColorEdit_AutomaticColor, ColorEdit_Cancel, ColorEdit_OK,
		TokenBox_TokenRemoveButtonToolTip,
		FilterControl_OK, FilterControl_Cancel, 
		FilterControl_GroupType, FilterControl_ClauseType, FilterControl_AggregateType, FilterControl_BetweenAnd, FilterControl_Not,
		FilterControl_AddConditionHint, FilterControl_RemoveConditionHint, FilterControl_AddValueHint, 
		FilterControl_ShowOperandTypeButtonHint,
		FilterControl_ClearFilter, FilterControl_ShowFilterControl,
		FilterControl_AddGroup, FilterControl_AddCondition, FilterControl_Remove,
		FilterControl_Empty, FilterControl_EmptyEnter,
		FilterControl_PopupHeaderText,
		FilterControl_VisualFilterTabCaption, FilterControl_TextFilterTabCaption,
		FilterControl_DateTimeOperatorMenuCaption,
		Captcha_DefaultImageAlternateText,
		Captcha_RefreshText,
		Captcha_DefaultErrorText,
		Captcha_DefaultTextBoxLabelText,
		CheckBox_Checked, CheckBox_Unchecked, CheckBox_Undefined,
		TrackBar_Increment, TrackBar_Decrement, TrackBar_Drag,
		InvalidSpinEditRange, InvalidSpinEditMaxValue, InvalidSpinEditMinValue,
		InvalidDateEditRange, InvalidDateEditMaxDate, InvalidDateEditMinDate,
		DateRangeMinErrorText, DateRangeErrorText
	}
	public class ASPxEditorsResLocalizer : ASPxResLocalizerBase<ASPxEditorsStringId> {
		class ASPxEditorsStringIdComparer : IEqualityComparer<ASPxEditorsStringId> {
			public bool Equals(ASPxEditorsStringId x, ASPxEditorsStringId y) {
				return x == y;
			}
			public int GetHashCode(ASPxEditorsStringId obj) {
				return (int)obj;
			}
		}
		public ASPxEditorsResLocalizer() 
			: base(new ASPxEditorsLocalizer()) {
		}
		protected override string GlobalResourceAssemblyName { get { return AssemblyInfo.SRAssemblyEditorsWeb; } }
		protected override string ResxName { get { return "DevExpress.Web.Edit.LocalizationRes"; } }
		protected override IEqualityComparer<ASPxEditorsStringId> CreateComparer() {
			return new ASPxEditorsStringIdComparer();
		}
	}
	public class ASPxEditorsLocalizer : XtraLocalizer<ASPxEditorsStringId> {
		static ASPxEditorsLocalizer() {
			SetActiveLocalizerProvider(new ASPxActiveLocalizerProvider<ASPxEditorsStringId>(CreateResLocalizerInstance));
		}
		static XtraLocalizer<ASPxEditorsStringId> CreateResLocalizerInstance() {
			return new ASPxEditorsResLocalizer();
		}
		public static XtraLocalizer<ASPxEditorsStringId> CreateDefaultLocalizer() {
			return new ASPxEditorsResLocalizer();
		}
		public static string GetString(ASPxEditorsStringId id) {
			return Active.GetLocalizedString(id);
		}
		public override XtraLocalizer<ASPxEditorsStringId> CreateResXLocalizer() {
			return CreateResLocalizerInstance();
		}
		protected override void PopulateStringTable() {
			AddString(ASPxEditorsStringId.DefaultErrorText, StringResources.ASPxEdit_DefaultErrorText);
			AddString(ASPxEditorsStringId.RequiredFieldErrorText, StringResources.ASPxEdit_RequiredFieldDefaultErrorText);
			AddString(ASPxEditorsStringId.RegExValidationErrorText, StringResources.ASPxEdit_RegExValidationDefaultErrorText);
			AddString(ASPxEditorsStringId.BinaryImage_Cancel, StringResources.BinaryImage_Cancel);
			AddString(ASPxEditorsStringId.BinaryImage_Clear, StringResources.BinaryImage_Clear);			
			AddString(ASPxEditorsStringId.BinaryImage_DropZone, StringResources.BinaryImage_DropZone);
			AddString(ASPxEditorsStringId.BinaryImage_Empty, StringResources.BinaryImage_Empty);
			AddString(ASPxEditorsStringId.BinaryImage_Uploading, StringResources.BinaryImage_Uploading);
			AddString(ASPxEditorsStringId.BinaryImage_UploadFile, StringResources.BinaryImage_UploadFile);
			AddString(ASPxEditorsStringId.Calendar_Today, StringResources.Calendar_Today);
			AddString(ASPxEditorsStringId.Calendar_Clear, StringResources.Calendar_Clear);
			AddString(ASPxEditorsStringId.Calendar_OK, StringResources.Calendar_Ok);
			AddString(ASPxEditorsStringId.Calendar_Cancel, StringResources.Calendar_Cancel);
			AddString(ASPxEditorsStringId.ColorEdit_CustomColor, StringResources.ColorEdit_CustomColor);
			AddString(ASPxEditorsStringId.ColorEdit_AutomaticColor, StringResources.ColorEdit_AutomaticColor);
			AddString(ASPxEditorsStringId.ColorEdit_Cancel, StringResources.ColorEdit_Cancel);
			AddString(ASPxEditorsStringId.ColorEdit_OK, StringResources.ColorEdit_OK);
			AddString(ASPxEditorsStringId.TokenBox_TokenRemoveButtonToolTip, StringResources.TokenBox_TokenRemoveButtonToolTip);
			AddString(ASPxEditorsStringId.FilterControl_OK, StringResources.FilterControl_OK);
			AddString(ASPxEditorsStringId.FilterControl_Cancel, StringResources.FilterControl_Cancel);
			AddString(ASPxEditorsStringId.FilterControl_GroupType, StringResources.FilterControl_GroupType);
			AddString(ASPxEditorsStringId.FilterControl_ClauseType, StringResources.FilterControl_ClauseType);
			AddString(ASPxEditorsStringId.FilterControl_AggregateType, StringResources.FilterControl_AggregateType);
			AddString(ASPxEditorsStringId.FilterControl_BetweenAnd, StringResources.FilterControl_BetweenAnd);
			AddString(ASPxEditorsStringId.FilterControl_Not, StringResources.FilterControl_Not);
			AddString(ASPxEditorsStringId.FilterControl_AddConditionHint, StringResources.FilterControl_AddConditionHint);
			AddString(ASPxEditorsStringId.FilterControl_RemoveConditionHint, StringResources.FilterControl_RemoveConditionHint);
			AddString(ASPxEditorsStringId.FilterControl_ShowOperandTypeButtonHint, StringResources.FilterControl_ShowOperandTypeButtonHint);
			AddString(ASPxEditorsStringId.FilterControl_AddValueHint, StringResources.FilterControl_AddValueHint);
			AddString(ASPxEditorsStringId.FilterControl_ClearFilter, StringResources.FilterControl_ClearFilter);
			AddString(ASPxEditorsStringId.FilterControl_ShowFilterControl, StringResources.FilterControl_ShowFilterControl);
			AddString(ASPxEditorsStringId.FilterControl_AddGroup, StringResources.FilterControl_AddGroup);
			AddString(ASPxEditorsStringId.FilterControl_AddCondition, StringResources.FilterControl_AddCondition);
			AddString(ASPxEditorsStringId.FilterControl_Remove, StringResources.FilterControl_Remove);
			AddString(ASPxEditorsStringId.FilterControl_Empty, StringResources.FilterControl_Empty);
			AddString(ASPxEditorsStringId.FilterControl_EmptyEnter, StringResources.FilterControl_EmptyEnter);
			AddString(ASPxEditorsStringId.FilterControl_PopupHeaderText, StringResources.FilterControl_PopupHeaderText);
			AddString(ASPxEditorsStringId.FilterControl_VisualFilterTabCaption, StringResources.FilterControl_VisualFilterTabCaption);
			AddString(ASPxEditorsStringId.FilterControl_TextFilterTabCaption, StringResources.FilterControl_TextFilterTabCaption);
			AddString(ASPxEditorsStringId.FilterControl_DateTimeOperatorMenuCaption, StringResources.FilterControl_DateTimeOperatorMenuCaption);
			AddString(ASPxEditorsStringId.Captcha_DefaultErrorText, StringResources.Captcha_DefaultErrorText);
			AddString(ASPxEditorsStringId.Captcha_DefaultImageAlternateText, StringResources.Captcha_DefaultImageAlternateText);
			AddString(ASPxEditorsStringId.Captcha_RefreshText, StringResources.Captcha_RefreshText);
			AddString(ASPxEditorsStringId.Captcha_DefaultTextBoxLabelText, StringResources.Captcha_DefaultTextBoxLabelText);
			AddString(ASPxEditorsStringId.CheckBox_Checked, StringResources.CheckBox_Checked);
			AddString(ASPxEditorsStringId.CheckBox_Unchecked, StringResources.CheckBox_Unchecked);
			AddString(ASPxEditorsStringId.CheckBox_Undefined, StringResources.CheckBox_Undefined);
			AddString(ASPxEditorsStringId.TrackBar_Decrement, StringResources.TrackBar_Decrement);
			AddString(ASPxEditorsStringId.TrackBar_Increment, StringResources.TrackBar_Increment);
			AddString(ASPxEditorsStringId.TrackBar_Drag, StringResources.TrackBar_Drag);
			AddString(ASPxEditorsStringId.InvalidSpinEditRange, StringResources.InvalidSpinEditRange);
			AddString(ASPxEditorsStringId.InvalidSpinEditMaxValue, StringResources.InvalidSpinEditMaxValue);
			AddString(ASPxEditorsStringId.InvalidSpinEditMinValue, StringResources.InvalidSpinEditMinValue);
			AddString(ASPxEditorsStringId.InvalidDateEditRange, StringResources.InvalidDateEditRange);
			AddString(ASPxEditorsStringId.InvalidDateEditMaxDate, StringResources.InvalidDateEditMaxValue);
			AddString(ASPxEditorsStringId.InvalidDateEditMinDate, StringResources.InvalidDateEditMinValue);
			AddString(ASPxEditorsStringId.DateRangeMinErrorText, StringResources.ASPxDateEdit_DateRange_MinValueErrorText);
			AddString(ASPxEditorsStringId.DateRangeErrorText, StringResources.ASPxDateEdit_DateRange_RangeErrorText);
		}
	}
}
