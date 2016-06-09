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
using System.Collections.Generic;
using System.Globalization;
namespace DevExpress.XtraSpreadsheet.Model.CopyOperation {
	public static class PresentDataCultureTable {
		static readonly Dictionary<string, PresetCulture> cultureTable = CreateCultureTable();
		static Dictionary<string, PresetCulture> CreateCultureTable() {
			Dictionary<string, PresetCulture> result = new Dictionary<string, PresetCulture>();
			result.Add("English", PresetCulture.English);
			result.Add("French", PresetCulture.French);
			result.Add("German", PresetCulture.German);
			result.Add("Italian", PresetCulture.Italian);
			result.Add("Russian", PresetCulture.Russian);
			result.Add("Spanish", PresetCulture.Spanish);
			return result;
		}
		public static PresetData Create(string value, CultureInfo culture) {
			string cultureName = culture.Parent.EnglishName;
			PresetCulture presetCulture;
			if (!cultureTable.TryGetValue(cultureName, out presetCulture))
				presetCulture = PresetCulture.English;
			PresetDataProvider presetDataProvider = CreatePresetDataProvider(value, presetCulture);
			if (presetDataProvider != null)
				return new PresetData(presetDataProvider);
			return null;
		}
		static PresetDataProvider CreatePresetDataProvider(string sourceCellTextValue, PresetCulture culture) {
			PresetDataProvider presetDataProvider;
			switch (culture) {
				default:
				case PresetCulture.English:
					presetDataProvider = new PresetDataProviderEnglish(sourceCellTextValue);
					break;
				case PresetCulture.French:
					presetDataProvider = new PresetDataProviderFrench(sourceCellTextValue);
					break;
				case PresetCulture.German:
					presetDataProvider = new PresetDataProviderGerman(sourceCellTextValue);
					break;
				case PresetCulture.Italian:
					presetDataProvider = new PresetDataProviderItalian(sourceCellTextValue);
					break;
				case PresetCulture.Spanish:
					presetDataProvider = new PresetDataProviderSpanish(sourceCellTextValue);
					break;
				case PresetCulture.Russian:
					presetDataProvider = new PresetDataProviderRussian(sourceCellTextValue);
					break;
			}
			if (presetDataProvider.Index >= 0)
				return presetDataProvider;
			return null;
		}
	}
}
