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
using System.Collections;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.Web.ASPxRichEdit.Export {
	public class CharacterFormattingBaseExporter2 : FormattingExporterBase<DevExpress.XtraRichEdit.Model.CharacterFormattingBase> {		
	WebFontInfoCache fontInfoCache;
		public CharacterFormattingBaseExporter2(WebFontInfoCache fontInfoCache) {
			this.fontInfoCache = fontInfoCache;
		}
		public override void FillHashtable(Hashtable result, DevExpress.XtraRichEdit.Model.CharacterFormattingBase info) {
			result.Add("6", info.AllCaps);			
			result.Add("10", info.BackColor.ToArgb());			
			result.Add("2", info.FontBold);			
			result.Add("3", info.FontItalic);			
			result.Add("0", this.fontInfoCache.AddItem(info.FontName));			
			result.Add("1", info.DoubleFontSize / 2.0f);			
			result.Add("4", (int)info.FontStrikeoutType);			
			result.Add("5", (int)info.FontUnderlineType);			
			result.Add("9", info.ForeColor.ToArgb());			
			result.Add("13", (int)info.Script);			
			result.Add("12", info.StrikeoutColor.ToArgb());			
			result.Add("7", info.StrikeoutWordsOnly);			
			result.Add("11", info.UnderlineColor.ToArgb());			
			result.Add("8", info.UnderlineWordsOnly);			
			result.Add("14", info.Hidden);			
			result.Add("16", info.NoProof);			
			result.Add("17", (int)info.UseValue);			
		}
		public override void RestoreInfo(Hashtable source, DevExpress.XtraRichEdit.Model.CharacterFormattingBase info) {
			info.AllCaps = Convert.ToBoolean(source["6"]);
			info.BackColor = System.Drawing.Color.FromArgb((int)source["10"]);
			info.FontBold = Convert.ToBoolean(source["2"]);
			info.FontItalic = Convert.ToBoolean(source["3"]);
			info.FontName = this.fontInfoCache.GetItem((int)source[((int)JSONCharacterFormattingProperty.FontInfoIndex).ToString()]).Name;
			info.DoubleFontSize = (int)(Convert.ToDouble(source[((int)JSONCharacterFormattingProperty.FontSize).ToString()]) * 2);
			info.FontStrikeoutType = (DevExpress.XtraRichEdit.Model.StrikeoutType)source["4"];
			info.FontUnderlineType = (DevExpress.XtraRichEdit.Model.UnderlineType)source["5"];
			info.ForeColor = System.Drawing.Color.FromArgb((int)source["9"]);
			info.Script = (DevExpress.Office.CharacterFormattingScript)source["13"];
			info.StrikeoutColor = System.Drawing.Color.FromArgb((int)source["12"]);
			info.StrikeoutWordsOnly = Convert.ToBoolean(source["7"]);
			info.UnderlineColor = System.Drawing.Color.FromArgb((int)source["11"]);
			info.UnderlineWordsOnly = Convert.ToBoolean(source["8"]);
			info.Hidden = Convert.ToBoolean(source["14"]);
			info.NoProof = Convert.ToBoolean(source["16"]);
			info.UseValue = (DevExpress.XtraRichEdit.Model.CharacterFormattingOptions.Mask)source["17"];
		}
	}
}
