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
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Globalization;
using DevExpress.Office;
using DevExpress.Office.Services;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Export.Xls;
using DevExpress.XtraSpreadsheet.Model.External;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraExport.Xls;
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region XlsCommandHyperLink
	public class XlsCommandHyperlink : XlsCommandContentBase {
		XlsContentHyperlink content = new XlsContentHyperlink();
		#region Properties
		public CellRangeInfo Range {
			get {
				XlsRef8 range = content.Range;
				return new CellRangeInfo(
					new CellPosition(range.FirstColumnIndex, range.FirstRowIndex), 
					new CellPosition(range.LastColumnIndex, range.LastRowIndex)); 
			}
			set {
				if(value == null)
					value = new CellRangeInfo();
				content.Range = new XlsRef8() { 
					FirstColumnIndex = value.First.Column, 
					FirstRowIndex = value.First.Row, 
					LastColumnIndex = value.Last.Column, 
					LastRowIndex = value.Last.Row 
				};
			}
		}
		public Guid ClassId {
			get { return content.ClassId; }
			set {
				Guard.ArgumentNotNull(value, "ClassId");
				content.ClassId = value;
			}
		}
		public bool HasMoniker { get { return content.HasMoniker; } set { content.HasMoniker = value; } }
		public bool IsAbsolute { get { return content.IsAbsolute; } set { content.IsAbsolute = value; } }
		public bool SiteGaveDisplayName { get { return content.SiteGaveDisplayName; } set { content.SiteGaveDisplayName = value; } }
		public bool HasLocationString { get { return content.HasLocationString; } set { content.HasLocationString = value; } }
		public bool HasDisplayName { get { return content.HasDisplayName; } set { content.HasDisplayName = value; } }
		public bool HasGUID { get { return content.HasGUID; } set { content.HasGUID = value; } }
		public bool HasCreationTime { get { return content.HasCreationTime; } set { content.HasCreationTime = value; } }
		public bool HasFrameName { get { return content.HasFrameName; } set { content.HasFrameName = value; } }
		public bool IsMonkerSavedAsString { get { return content.IsMonkerSavedAsString; } set { content.IsMonkerSavedAsString = value; } }
		public bool IsAbsoluteFromRelative { get { return content.IsAbsoluteFromRelative; } set { content.IsAbsoluteFromRelative = value; } }
		public string DisplayName { get { return content.DisplayName; } set { content.DisplayName = value; } }
		public string FrameName { get { return content.FrameName; } set { content.FrameName = value; } }
		public string Moniker { get { return content.Moniker; } set { content.Moniker = value; } }
		public XlsHyperlinkMonikerBase OleMoniker { get { return content.OleMoniker; } set { content.OleMoniker = value; } }
		public string Location { get { return content.Location; } set { content.Location = value; } }
		public Guid OptionalGUID { get { return content.OptionalGUID; } set { content.OptionalGUID = value; } }
		public Int64 CreationTime { get { return content.CreationTime; } set { content.CreationTime = value; } }
		#endregion
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			if(HasMoniker) {
				if(IsMonkerSavedAsString) {
					Uri uri;
					if(Uri.TryCreate(Moniker, UriKind.RelativeOrAbsolute, out uri))
						CreateHyperlink(contentBuilder, Moniker, true);
				}
				else {
					if(OleMoniker.ClassId == XlsHyperlinkMonikerFactory.CLSID_URLMoniker) {
						XlsHyperlinkURLMoniker urlMoniker = OleMoniker as XlsHyperlinkURLMoniker;
						CreateHyperlink(contentBuilder, urlMoniker.Url, true);
					}
					else if(OleMoniker.ClassId == XlsHyperlinkMonikerFactory.CLSID_FileMoniker) {
						XlsHyperlinkFileMoniker fileMoniker = OleMoniker as XlsHyperlinkFileMoniker;
						CreateHyperlink(contentBuilder, fileMoniker.Path, true);
					}
				}
			}
			else if(HasLocationString) {
				CreateHyperlink(contentBuilder, Location, false);
			}
		}
		void CreateHyperlink(XlsContentBuilder contentBuilder, string targetUri, bool isExternal) {
			if(isExternal) {
				if(!IsValidUri(targetUri)) {
					string msg = string.Format(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_InvalidHyperlinkRemoved), targetUri);
					contentBuilder.LogMessage(LogCategory.Warning, msg);
					return;
				}
				if(HasLocationString)
					targetUri += "#" + Location;
			}
			CellRange range = new CellRange(contentBuilder.CurrentSheet, Range.First, Range.Last);
			ModelHyperlink hyperlink = contentBuilder.CurrentSheet.CreateHyperlinkCoreFromImport(range, targetUri, isExternal);
			contentBuilder.CurrentSheet.Hyperlinks.Add(hyperlink);
			if(HasDisplayName)
				hyperlink.DisplayText = DisplayName;
		}
		bool IsValidUri(string targetUri) {
			Uri result;
			return Uri.TryCreate(targetUri, UriKind.RelativeOrAbsolute, out result);
		}
		public override IXlsCommand GetInstance() {
			this.content = new XlsContentHyperlink();
			return this;
		}
		protected override IXlsContent GetContent() {
			return content;
		}
	}
	#endregion
	#region XlsCommandHyperlinkTooltip
	public class XlsCommandHyperlinkTooltip : XlsCommandContentBase {
		XlsContentHyperlinkTooltip content = new XlsContentHyperlinkTooltip();
		#region Properties
		public CellRangeInfo Range {
			get {
				XlsRef8 range = content.Range;
				return new CellRangeInfo(
					new CellPosition(range.FirstColumnIndex, range.FirstRowIndex),
					new CellPosition(range.LastColumnIndex, range.LastRowIndex));
			}
			set {
				if(value == null)
					value = new CellRangeInfo();
				content.Range = new XlsRef8() {
					FirstColumnIndex = value.First.Column,
					FirstRowIndex = value.First.Row,
					LastColumnIndex = value.Last.Column,
					LastRowIndex = value.Last.Row
				};
			}
		}
		public string Tooltip {
			get { return content.Tooltip; }
			set { content.Tooltip = value; }
		}
		#endregion
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			ModelHyperlink hyperlink = FindHyperLink(contentBuilder.CurrentSheet.Hyperlinks);
			if(hyperlink != null)
				hyperlink.TooltipText = Tooltip;
		}
		ModelHyperlink FindHyperLink(ModelHyperlinkCollection hyperlinks) {
			int count = hyperlinks.Count;
			if(count == 0)
				return null;
			ModelHyperlink item = hyperlinks[count - 1];
			if(IsEqualRange(item.Range))
				return item;
			for(int i = 0; i < count; i++) {
				item = hyperlinks[i];
				if(IsEqualRange(item.Range))
					return item;
			}
			return null;
		}
		bool IsEqualRange(CellRange range) {
			return range.TopLeft.Equals(Range.First) && range.BottomRight.Equals(Range.Last);
		}
		public override IXlsCommand GetInstance() {
			this.content = new XlsContentHyperlinkTooltip();
			return this;
		}
		protected override IXlsContent GetContent() {
			return content;
		}
	}
	#endregion
}
