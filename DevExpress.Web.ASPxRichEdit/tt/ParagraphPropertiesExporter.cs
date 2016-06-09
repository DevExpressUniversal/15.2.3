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
	public class ParagraphFormattingInfoExporter : FormattingExporterBase<DevExpress.XtraRichEdit.Model.ParagraphFormattingInfo> {		
		public override void FillHashtable(Hashtable result, DevExpress.XtraRichEdit.Model.ParagraphFormattingInfo info) {
			result.Add("0", (int)info.Alignment);			
			result.Add("3", info.LeftIndent);			
			result.Add("6", info.RightIndent);			
			result.Add("7", info.SpacingBefore);			
			result.Add("8", info.SpacingAfter);			
			result.Add("5", (int)info.LineSpacingType);			
			result.Add("4", info.LineSpacing);			
			result.Add("2", (int)info.FirstLineIndentType);			
			result.Add("1", info.FirstLineIndent);			
			result.Add("9", info.SuppressHyphenation);			
			result.Add("10", info.SuppressLineNumbers);			
			result.Add("11", info.ContextualSpacing);			
			result.Add("12", info.PageBreakBefore);			
			result.Add("13", info.BeforeAutoSpacing);			
			result.Add("14", info.AfterAutoSpacing);			
			result.Add("15", info.KeepWithNext);			
			result.Add("16", info.KeepLinesTogether);			
			result.Add("17", info.WidowOrphanControl);			
			result.Add("18", info.OutlineLevel);			
			result.Add("19", info.BackColor.ToArgb());			
			result.Add("20", DevExpress.Web.ASPxRichEdit.Export.BorderInfoExporter.ToHashtable(info.LeftBorder));			
			result.Add("21", DevExpress.Web.ASPxRichEdit.Export.BorderInfoExporter.ToHashtable(info.RightBorder));			
			result.Add("22", DevExpress.Web.ASPxRichEdit.Export.BorderInfoExporter.ToHashtable(info.TopBorder));			
			result.Add("23", DevExpress.Web.ASPxRichEdit.Export.BorderInfoExporter.ToHashtable(info.BottomBorder));			
		}
		public override void RestoreInfo(Hashtable source, DevExpress.XtraRichEdit.Model.ParagraphFormattingInfo info) {
			info.Alignment = (DevExpress.XtraRichEdit.Model.ParagraphAlignment)source["0"];
			info.LeftIndent = Convert.ToInt32(source["3"]);
			info.RightIndent = Convert.ToInt32(source["6"]);
			info.SpacingBefore = Convert.ToInt32(source["7"]);
			info.SpacingAfter = Convert.ToInt32(source["8"]);
			info.LineSpacingType = (DevExpress.XtraRichEdit.Model.ParagraphLineSpacing)source["5"];
			info.LineSpacing = Convert.ToSingle(source["4"]);
			info.FirstLineIndentType = (DevExpress.XtraRichEdit.Model.ParagraphFirstLineIndent)source["2"];
			info.FirstLineIndent = Convert.ToInt32(source["1"]);
			info.SuppressHyphenation = Convert.ToBoolean(source["9"]);
			info.SuppressLineNumbers = Convert.ToBoolean(source["10"]);
			info.ContextualSpacing = Convert.ToBoolean(source["11"]);
			info.PageBreakBefore = Convert.ToBoolean(source["12"]);
			info.BeforeAutoSpacing = Convert.ToBoolean(source["13"]);
			info.AfterAutoSpacing = Convert.ToBoolean(source["14"]);
			info.KeepWithNext = Convert.ToBoolean(source["15"]);
			info.KeepLinesTogether = Convert.ToBoolean(source["16"]);
			info.WidowOrphanControl = Convert.ToBoolean(source["17"]);
			info.OutlineLevel = Convert.ToInt32(source["18"]);
			info.BackColor = DevExpress.Web.ASPxRichEdit.Internal.PropertiesHelper.GetColorFromArgb((int)source["19"]);
			info.LeftBorder = DevExpress.Web.ASPxRichEdit.Export.BorderInfoExporter.FromHashtable((Hashtable)source["20"]);
			info.RightBorder = DevExpress.Web.ASPxRichEdit.Export.BorderInfoExporter.FromHashtable((Hashtable)source["21"]);
			info.TopBorder = DevExpress.Web.ASPxRichEdit.Export.BorderInfoExporter.FromHashtable((Hashtable)source["22"]);
			info.BottomBorder = DevExpress.Web.ASPxRichEdit.Export.BorderInfoExporter.FromHashtable((Hashtable)source["23"]);
		}
	}
}
