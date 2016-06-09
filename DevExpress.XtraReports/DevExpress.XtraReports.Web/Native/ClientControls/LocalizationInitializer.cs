#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Resources;
namespace DevExpress.XtraReports.Web.Native.ClientControls {
	public static class LocalizationInitializer {
		public static Dictionary<string, string> GetLocalization(ILocalizationInfoProvider localizationInfoProvider) {
			Type[] resFinderTypes = localizationInfoProvider.GetPropertyResFinderSuite();
			Type[] localizationTypes = localizationInfoProvider.GetLocalizationResFinderSuite();
			var result = new Dictionary<string, string>();
			if(resFinderTypes != null && resFinderTypes.Length > 0)
				FillLocalizationForProperties(
					result,
					resFinderTypes[0],
					resFinderTypes.Skip(1),
					localizationTypes);
			return result;
		}
		static void FillLocalizationForProperties(Dictionary<string, string> result, Type dataType, IEnumerable<Type> typesProperties, Type[] typesLocalizations) {
			var culture = CultureInfo.CurrentUICulture;
			AddLocalizationSet(result, dataType, culture, DXDisplayNameAttribute.DefaultResourceFile, IsReportingTypeName);
			foreach(Type type in typesProperties) {
				AddLocalizationSet(result, type, culture);
			}
			foreach(Type type in typesLocalizations) {
				AddLocalizationSet(result, type, culture, "LocalizationRes");
			}
		}
		static bool IsReportingTypeName(string typeName) {
			return typeName.StartsWith("DevExpress.XtraReports", StringComparison.Ordinal)
				|| typeName.StartsWith("DevExpress.XtraPrinting", StringComparison.Ordinal);
		}
		static void AddLocalizationSet(Dictionary<string, string> result, Type type, CultureInfo culture, string resourceFileName = DXDisplayNameAttribute.DefaultResourceFile, Predicate<string> addCondition = null) {
			var resourceManager = new ResourceManager(type.Namespace + "." + resourceFileName, type.Assembly);
			var resourceSet = resourceManager.GetResourceSet(culture, true, true);
			foreach(DictionaryEntry entry in resourceSet) {
				var value = (string)entry.Value;
				if(value.Contains('&')) {
					value = value.Remove(value.IndexOf('&'), 1);
				}
				if((addCondition == null || addCondition((string)entry.Key))) {
					result[(string)entry.Key] = value;
				}
			}
		}
	}
}
