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
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
using DevExpress.Office.Model;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Model {
	#region DocumentInfo
	public class DocumentInfo : ICloneable<DocumentInfo>, ISupportsCopyFrom<DocumentInfo>, ISupportsSizeOf {
		#region Fields
		public const bool HyphenateDocumentDefaultValue = false;
		int defaultTabWidth;
		bool hyphenateDocument = HyphenateDocumentDefaultValue;
		bool differentOddAndEvenPages;
		Color pageBackColor;
		bool displayBackgroundShape;
		UpdateDocVariablesBeforePrint updateDocVariablesBeforePrint;
#if THEMES_EDIT
		ColorModelInfo colorModelInfo = ColorModelInfo.Create(DXColor.Empty);
#endif
		#endregion
		#region Properties
		public int DefaultTabWidth { get { return defaultTabWidth; } set { defaultTabWidth = value; } }
		public bool HyphenateDocument { get { return hyphenateDocument; } set { hyphenateDocument = value; } }
		public bool DifferentOddAndEvenPages { get { return differentOddAndEvenPages; } set { differentOddAndEvenPages = value; } }
		public Color PageBackColor { get { return pageBackColor; } set { pageBackColor = value; } }
		public bool DisplayBackgroundShape { get { return displayBackgroundShape; } set { displayBackgroundShape = value; } }
		public UpdateDocVariablesBeforePrint UpdateDocVariablesBeforePrint { get { return updateDocVariablesBeforePrint; } set { updateDocVariablesBeforePrint = value; } }
#if THEMES_EDIT
		public ColorModelInfo ColorModelInfo { get { return colorModelInfo; } set { colorModelInfo = value; } }
#endif
		#endregion 
		#region ICloneable<DocumentInfo> Members
		public DocumentInfo Clone() {
			DocumentInfo result = new DocumentInfo();
			result.CopyFrom(this);
			return result;
		}
		#endregion
		public override bool Equals(object obj) {
			DocumentInfo info = (DocumentInfo)obj;
			return
				this.DefaultTabWidth == info.DefaultTabWidth &&
				this.HyphenateDocument == info.HyphenateDocument &&
				this.DifferentOddAndEvenPages == info.DifferentOddAndEvenPages &&
				this.PageBackColor == info.PageBackColor &&
				this.UpdateDocVariablesBeforePrint == info.UpdateDocVariablesBeforePrint &&
#if THEMES_EDIT
				this.ColorModelInfo.Equals(info.ColorModelInfo) &&
#endif
				this.DisplayBackgroundShape == info.DisplayBackgroundShape;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public void CopyFrom(DocumentInfo info) {
			this.DefaultTabWidth = info.DefaultTabWidth;
			this.HyphenateDocument = info.HyphenateDocument;
			this.DifferentOddAndEvenPages = info.DifferentOddAndEvenPages;
			this.PageBackColor = info.PageBackColor;
			this.DisplayBackgroundShape = info.DisplayBackgroundShape;
			this.UpdateDocVariablesBeforePrint = info.UpdateDocVariablesBeforePrint;
#if THEMES_EDIT
			this.ColorModelInfo = info.ColorModelInfo;
#endif
		}
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return ObjectSizeHelper.CalculateApproxObjectSize32(this, true);
		}
		#endregion
	}
	#endregion
	#region DocumentInfoCache
	public class DocumentInfoCache : UniqueItemsCache<DocumentInfo> {
		public DocumentInfoCache(DocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override DocumentInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			DocumentInfo result = new DocumentInfo();
			result.DefaultTabWidth = unitConverter.TwipsToModelUnits(720);
			result.HyphenateDocument = DocumentInfo.HyphenateDocumentDefaultValue;
			return result;
		}
	}
	#endregion
	#region DocumentProperties
	public class DocumentProperties : RichEditIndexBasedObject<DocumentInfo> {
		public static string UpdateDocVariableFieldsBeforePrintDocVarName = "__dx_updateDocVarBeforePrint";
		public DocumentProperties(DocumentModel documentModel)
			: base(GetMainPieceTable(documentModel)) {
		}
		static PieceTable GetMainPieceTable(DocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			return documentModel.MainPieceTable;
		}
		#region Properties
		#region DefaultTabWidth
		public int DefaultTabWidth {
			get { return Info.DefaultTabWidth; }
			set {
				Guard.ArgumentPositive(value, "DefaultTabWidth");
				if (DefaultTabWidth == value)
					return;
				SetPropertyValue(SetDefaultTabWidthCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetDefaultTabWidthCore(DocumentInfo info, int value) {
			info.DefaultTabWidth = value;
			return DocumentPropertiesChangeActionsCalculator.CalculateChangeActions(DocumentPropertiesChangeType.DefaultTabWidth);
		}
		#endregion
		#region HyphenateDocument
		public bool HyphenateDocument {
			get { return Info.HyphenateDocument; }
			set {
				if (HyphenateDocument == value)
					return;
				SetPropertyValue(SetHyphenateDocumentCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetHyphenateDocumentCore(DocumentInfo info, bool value) {
			info.HyphenateDocument = value;
			return DocumentPropertiesChangeActionsCalculator.CalculateChangeActions(DocumentPropertiesChangeType.HyphenateDocument);
		}
		#endregion
		#region DifferentOddAndEvenPages
		public bool DifferentOddAndEvenPages {
			get { return Info.DifferentOddAndEvenPages; }
			set {
				if (DifferentOddAndEvenPages == value)
					return;
				SetPropertyValue(SetDifferentOddAndEvenPagesCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetDifferentOddAndEvenPagesCore(DocumentInfo info, bool value) {
			info.DifferentOddAndEvenPages = value;
			return DocumentPropertiesChangeActionsCalculator.CalculateChangeActions(DocumentPropertiesChangeType.DifferentOddAndEvenPages);
		}
		#endregion
		#region PageBackColor
		public Color PageBackColor {
			get { return Info.PageBackColor; }
			set {
				if (PageBackColor == value)
					return;
				SetPropertyValue(SetPageBackColorCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetPageBackColorCore(DocumentInfo info, Color value) {
			info.PageBackColor = value;
			return DocumentPropertiesChangeActionsCalculator.CalculateChangeActions(DocumentPropertiesChangeType.PageBackColor);
		}
		#endregion
#if THEMES_EDIT            
		#region PageBackColorModelInfo
		public ColorModelInfo PageBackColorModelInfo {
			get { return Info.ColorModelInfo; }
			set {
				if (PageBackColorModelInfo == value)
					return;
				SetPropertyValue(SetColorModelInfoCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetColorModelInfoCore(DocumentInfo info, ColorModelInfo value) {
			info.ColorModelInfo = value;
			return DocumentPropertiesChangeActionsCalculator.CalculateChangeActions(DocumentPropertiesChangeType.PageBackColor);
		}
		#endregion
#endif
		#region DisplayBackgroundShape
		public bool DisplayBackgroundShape {
			get { return Info.DisplayBackgroundShape; }
			set {
				if (DisplayBackgroundShape == value)
					return;
				SetPropertyValue(SetDisplayBackgroundShapeCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetDisplayBackgroundShapeCore(DocumentInfo info, bool value) {
			info.DisplayBackgroundShape = value;
			return DocumentPropertiesChangeActionsCalculator.CalculateChangeActions(DocumentPropertiesChangeType.DisplayBackgroundShape);
		}
		#endregion
		#region UpdateDocVariablesBeforePrint
		public UpdateDocVariablesBeforePrint UpdateDocVariablesBeforePrint {
			get { return Info.UpdateDocVariablesBeforePrint; }
			set {
				if (UpdateDocVariablesBeforePrint == value)
					return;
				SetPropertyValue(SetUpdateDocVariablesBeforePrintCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetUpdateDocVariablesBeforePrintCore(DocumentInfo info, UpdateDocVariablesBeforePrint value) {
			info.UpdateDocVariablesBeforePrint = value;
			return DocumentPropertiesChangeActionsCalculator.CalculateChangeActions(DocumentPropertiesChangeType.UpdateDocVariablesBeforePrint);
		}
		#endregion
		#endregion
		#region Events
		#region PageBackgroundChanged
		EventHandler onPageBackgroundChanged;
		protected internal event EventHandler PageBackgroundChanged { add { onPageBackgroundChanged += value; } remove { onPageBackgroundChanged -= value; } }
		protected internal virtual void RaisePageBackgroundChanged() {
			if (onPageBackgroundChanged != null)
				onPageBackgroundChanged(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		protected override void OnIndexChanged() {
			base.OnIndexChanged();
			RaisePageBackgroundChanged(); 
		}
		protected internal override UniqueItemsCache<DocumentInfo> GetCache(DocumentModel documentModel) {
			return documentModel.Cache.DocumentInfoCache;
		}
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentPropertiesChangeActionsCalculator.CalculateChangeActions(DocumentPropertiesChangeType.BatchUpdate);
		}
		protected internal virtual string GetUpdateFieldsBeforePrintDocVarValue() {
			switch (UpdateDocVariablesBeforePrint) {
				case UpdateDocVariablesBeforePrint.Auto:
					return String.Empty;
				case UpdateDocVariablesBeforePrint.Never:
					return "never";
				case UpdateDocVariablesBeforePrint.Always:
					return "always";
				default:
					return String.Empty;
			}
		}
		protected internal virtual void SetUpdateFieldsBeforePrintFromDocVar(string value) {
			switch(value) {
				case "never":
					UpdateDocVariablesBeforePrint = UpdateDocVariablesBeforePrint.Never;
					break;
				case "always":
					UpdateDocVariablesBeforePrint = UpdateDocVariablesBeforePrint.Always;
					break;
				default:
					UpdateDocVariablesBeforePrint = UpdateDocVariablesBeforePrint.Auto;
					break;
			}
		}
	}
	#endregion
	#region DocumentPropertiesChangeType
	public enum DocumentPropertiesChangeType {
		None = 0,
		DefaultTabWidth,
		HyphenateDocument,
		DifferentOddAndEvenPages,
		PageBackColor,
		DisplayBackgroundShape,
		UpdateDocVariablesBeforePrint,
		BatchUpdate
	}
	#endregion
	#region DocumentPropertiesChangeActionsCalculator
	public static class DocumentPropertiesChangeActionsCalculator {
		internal class DocumentPropertiesChangeActionsTable : Dictionary<DocumentPropertiesChangeType, DocumentModelChangeActions> {
		}
		internal static DocumentPropertiesChangeActionsTable documentPropertiesChangeActionsTable = CreateDocumentPropertiesChangeActionsTable();
		internal static DocumentPropertiesChangeActionsTable CreateDocumentPropertiesChangeActionsTable() {
			DocumentPropertiesChangeActionsTable table = new DocumentPropertiesChangeActionsTable();
			table.Add(DocumentPropertiesChangeType.None, DocumentModelChangeActions.None);
			table.Add(DocumentPropertiesChangeType.DefaultTabWidth, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetAllPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout);
			table.Add(DocumentPropertiesChangeType.HyphenateDocument, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetAllPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout);
			table.Add(DocumentPropertiesChangeType.DifferentOddAndEvenPages, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetAllPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout);
			table.Add(DocumentPropertiesChangeType.PageBackColor, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetAllPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout);
			table.Add(DocumentPropertiesChangeType.DisplayBackgroundShape, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetAllPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout);
			table.Add(DocumentPropertiesChangeType.BatchUpdate, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetAllPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout);
			table.Add(DocumentPropertiesChangeType.UpdateDocVariablesBeforePrint, DocumentModelChangeActions.None);
			return table;
		}
		public static DocumentModelChangeActions CalculateChangeActions(DocumentPropertiesChangeType change) {
			return documentPropertiesChangeActionsTable[change];
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit {
	public enum UpdateDocVariablesBeforePrint {
		Auto,
		Always,
		Never
	}
}
