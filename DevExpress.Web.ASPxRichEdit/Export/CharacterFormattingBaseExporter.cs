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

using DevExpress.XtraRichEdit.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.Web.ASPxRichEdit.Export {
	public class CharacterFormattingBaseExporter : FormattingExporterBase<CharacterFormattingBase> {
		public CharacterFormattingBaseExporter(DocumentModel documentModel, WebFontInfoCache fontInfoCache) {
			InfoExporter = new CharacterFormattingInfoExporter(fontInfoCache);
			DocumentModel = documentModel;
		}
		public CharacterFormattingInfoExporter InfoExporter { get; private set; }
		public DocumentModel DocumentModel { get; private set; }
		public override void FillHashtable(System.Collections.Hashtable result, CharacterFormattingBase property) {
			InfoExporter.FillHashtable(result, property.Info);
			result[((int)JSONCharacterFormattingProperty.UseValue).ToString()] = (int)property.UseValue;
		}
		public override void RestoreInfo(System.Collections.Hashtable source, CharacterFormattingBase property) {
			CharacterFormattingInfo characterFormattionInfo = property.GetInfoForModification();
			InfoExporter.RestoreInfo(source, characterFormattionInfo);
			string useValueHastableString = ((int)JSONCharacterFormattingProperty.UseValue).ToString();
			CharacterFormattingOptions characterFormattionOptions = new CharacterFormattingOptions((CharacterFormattingOptions.Mask)source[useValueHastableString]);
			int cacheIndexCharFormattionOptions = DocumentModel.Cache.CharacterFormattingOptionsCache.AddItem(characterFormattionOptions);
			int cacheIndexCharFormattionInfo = DocumentModel.Cache.CharacterFormattingInfoCache.AddItem(characterFormattionInfo);
			property.SetIndexInitial(cacheIndexCharFormattionInfo, cacheIndexCharFormattionOptions);
		}
	}
}
