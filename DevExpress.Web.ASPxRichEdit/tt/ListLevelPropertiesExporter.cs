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
	public class ListLevelPropertiesExporter : FormattingExporterBase<DevExpress.XtraRichEdit.Model.ListLevelProperties> {		
		public override void FillHashtable(Hashtable result, DevExpress.XtraRichEdit.Model.ListLevelProperties info) {
			result.Add("0", info.Start);			
			result.Add("1", (int)info.Format);			
			result.Add("5", (int)info.Alignment);			
			result.Add("2", info.ConvertPreviousLevelNumberingToDecimal);			
			result.Add("8", info.Separator);			
			result.Add("4", info.SuppressRestart);			
			result.Add("3", info.SuppressBulletResize);			
			result.Add("6", info.DisplayFormatString);			
			result.Add("7", info.RelativeRestartLevel);			
			result.Add("9", info.TemplateCode);			
			result.Add("11", info.Legacy);			
			result.Add("12", info.LegacySpace);			
			result.Add("13", info.LegacyIndent);			
			result.Add("10", info.OriginalLeftIndent);			
		}
		public override void RestoreInfo(Hashtable source, DevExpress.XtraRichEdit.Model.ListLevelProperties info) {
			info.Start = Convert.ToInt32(source["0"]);
			info.Format = (DevExpress.Office.NumberConverters.NumberingFormat)source["1"];
			info.Alignment = (DevExpress.XtraRichEdit.Model.ListNumberAlignment)source["5"];
			info.ConvertPreviousLevelNumberingToDecimal = Convert.ToBoolean(source["2"]);
			info.Separator = Convert.ToChar(source["8"]);
			info.SuppressRestart = Convert.ToBoolean(source["4"]);
			info.SuppressBulletResize = Convert.ToBoolean(source["3"]);
			info.DisplayFormatString = (System.String)source["6"];
			info.RelativeRestartLevel = Convert.ToInt32(source["7"]);
			info.TemplateCode = Convert.ToInt32(source["9"]);
			info.Legacy = Convert.ToBoolean(source["11"]);
			info.LegacySpace = Convert.ToInt32(source["12"]);
			info.LegacyIndent = Convert.ToInt32(source["13"]);
			info.OriginalLeftIndent = Convert.ToInt32(source["10"]);
		}
	}
}
