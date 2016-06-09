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
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.Office.History;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
using DevExpress.Office.Model;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model {
	#region SheetPropertiesBase (abstract class)
	public abstract class SheetPropertiesBase {
		#region Fields
		SheetFormatProperties formatProperties;
		#endregion
		protected SheetPropertiesBase(IDocumentModelPartWithApplyChanges sheet) {
			Guard.ArgumentNotNull(sheet, "sheet");
			this.formatProperties = new SheetFormatProperties(sheet);
		}
		#region Properties
		public SheetFormatProperties FormatProperties { get { return this.formatProperties; } }
		public abstract WorksheetProtectionOptions Protection { get; }
		#endregion
	}
	#endregion
	public class WorksheetProperties : SheetPropertiesBase, ICloneable<WorksheetProperties>, ISupportsCopyFrom<WorksheetProperties> {
		#region Fields
		readonly Worksheet sheet;
		bool isDialog;
		GroupAndOutlineProperties groupAndOutlineProperties;
		Margins margins;
		PrintSetup printSetup;
		HeaderFooterOptions headerFooter;
		TransitionOptions transitionOptions;
		WorksheetProtectionOptions protectionOptions;
		int tabColorIndex;
		string codeName;
		#endregion
		public WorksheetProperties(Worksheet sheet)
			: base(sheet) {
			this.sheet = sheet;
			this.groupAndOutlineProperties = new GroupAndOutlineProperties(sheet);
			this.margins = new Margins(sheet);
			this.printSetup = new PrintSetup(sheet);
			this.headerFooter = new HeaderFooterOptions(sheet);
			this.protectionOptions = new WorksheetProtectionOptions(sheet);
			this.transitionOptions = new TransitionOptions();
			this.codeName = string.Empty;
		}
		#region Properties
		public bool IsDialog { get { return this.isDialog; } set { this.isDialog = value; } }
		public GroupAndOutlineProperties GroupAndOutlineProperties { get { return this.groupAndOutlineProperties; } }
		public Margins Margins { get { return this.margins; } }
		public PrintSetup PrintSetup { get { return this.printSetup; } }
		public HeaderFooterOptions HeaderFooter { get { return headerFooter; } }
		public override WorksheetProtectionOptions Protection { get { return this.protectionOptions; } }
		public TransitionOptions TransitionOptions { get { return transitionOptions; } }
		public int TabColorIndex { get { return tabColorIndex; } set { tabColorIndex = value; } }
		public string CodeName {
			get { return codeName; }
			set {
				if (string.IsNullOrEmpty(value))
					codeName = string.Empty;
				else
					codeName = value;
			}
		}
		#endregion
		public WorksheetProperties Clone() {
			WorksheetProperties result = new WorksheetProperties(sheet);
			result.CopyFrom(this);
			return result;
		}
		public void CopyFrom(WorksheetProperties value) {
			using (HistoryTransaction transaction = new HistoryTransaction(this.Margins.DocumentModel.History)) {
				try {
					this.Margins.DocumentModel.BeginUpdate();
					base.FormatProperties.CopyFrom(value.FormatProperties);
					this.isDialog = value.isDialog;
					this.groupAndOutlineProperties.CopyFrom(value.GroupAndOutlineProperties);
					this.headerFooter.CopyFrom(value.headerFooter);
					this.margins.CopyFrom(value.margins);
					this.printSetup.CopyFrom(value.printSetup);
					this.protectionOptions.CopyFrom(value.protectionOptions);
					this.transitionOptions.CopyFrom(value.transitionOptions);
					this.codeName = value.codeName;
					SetTabColor(value.GetTabColor());
				}
				finally {
					this.Margins.DocumentModel.EndUpdate();
				}
			}
		}
		protected internal Color GetTabColor() {
			DocumentModel workbook = sheet.Workbook as DocumentModel;
			if (workbook != null)
				return workbook.Cache.ColorModelInfoCache[tabColorIndex].ToRgb(workbook.StyleSheet.Palette, workbook.OfficeTheme.Colors);
			return DXColor.Empty; 
		}
		protected internal void SetTabColor(Color color) {
			DocumentModel workbook = sheet.Workbook as DocumentModel;
			if (workbook == null)
				return;
			ColorModelInfo colorModel = new ColorModelInfo();
			colorModel.Rgb = color;
			this.tabColorIndex = workbook.Cache.ColorModelInfoCache.AddItem(colorModel);
		}
	}
}
