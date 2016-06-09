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

using DevExpress.XtraPrinting.Localization;
using DevExpress.XtraPrinting.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace DevExpress.XtraPrinting.Native {
	public static class ExportOptionsLocalizer {
		static Dictionary<Enum, PreviewStringId> localizationAssociations = new Dictionary<Enum, PreviewStringId>();
		static ExportOptionsLocalizer() {
			foreach(ExportOptionKind optionKind in DevExpress.Data.Utils.Helpers.GetEnumValues<ExportOptionKind>()) {
				AddExportOptionKindLocalizationAssociation(optionKind, (PreviewStringId)Enum.Parse(typeof(PreviewStringId), "ExportOption_" + optionKind.ToString(), false));
			}
		}
		internal static string GetLocalizedOption(Enum enumValue) {
			return PreviewLocalizer.GetString(localizationAssociations[enumValue]);
		}
		internal static object GetDelocalizedOption(Type enumType, string option) {
			foreach(Enum enumValue in DevExpress.Data.Utils.Helpers.GetEnumValues(enumType)) {
				if(GetLocalizedOption(enumValue) == option)
					return enumValue;
			}
			return ExceptionHelper.ThrowInvalidOperationException<object>();
		}
		static void AddExportOptionKindLocalizationAssociation(ExportOptionKind optionKind, PreviewStringId previewStringId) {
			AddLocalizationAssociation(optionKind, previewStringId);
			object[] attributes = typeof(ExportOptionKind).GetField(optionKind.ToString()).GetCustomAttributes(typeof(OptionKindPropertyTypeAttribute), false);
			if(attributes.Length == 1 && attributes[0] is OptionKindPropertyTypeAttribute) {
				OptionKindPropertyTypeAttribute optionKindPropertyType = (OptionKindPropertyTypeAttribute)attributes[0];
				if(!optionKindPropertyType.PropertyType.IsSubclassOf(typeof(Enum)))
					return;
				foreach(Enum enumValue in DevExpress.Data.Utils.Helpers.GetEnumValues(optionKindPropertyType.PropertyType)) {
					AddLocalizationAssociation(enumValue, (PreviewStringId)Enum.Parse(typeof(PreviewStringId), "ExportOption_" + optionKind.ToString() + "_" + enumValue.ToString(), false));
				}
			}
		}
		static void AddLocalizationAssociation(Enum enumValue, PreviewStringId previewStringId) {
			localizationAssociations[enumValue] = previewStringId;
		}
	}
}
