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
using DevExpress.Utils;
using System.ComponentModel;
using DevExpress.Office.Forms;
using DevExpress.Compatibility.System.Windows.Forms;
#if !SL
using System.Windows.Forms;
using DevExpress.XtraRichEdit.Forms;
#endif
namespace DevExpress.XtraRichEdit {
#if !SL
	#region ShowFormEventArgs
	public class ShowFormEventArgs : EventArgs {
		#region Fields
		bool handled;
		DialogResult dialogResult = DialogResult.None;
		IWin32Window parent;
		#endregion
		#region Properties
		public DialogResult DialogResult { get { return dialogResult; } set { dialogResult = value; } }
		public IWin32Window Parent { get { return parent; } set { parent = value; } }
		public bool Handled { get { return handled; } set { handled = value; } }
		#endregion
	}
	#endregion
	#region FormShowingEventArgs (abstract class)
	public abstract class FormShowingEventArgs : ShowFormEventArgs {
		protected FormShowingEventArgs(FormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.Parent = controllerParameters.Control;
		}
	}
	#endregion
	#region FontFormShowingEventHandler
	public delegate void FontFormShowingEventHandler(object sender, FontFormShowingEventArgs e);
	#endregion
	#region FontFormShowingEventArgs
	public class FontFormShowingEventArgs : FormShowingEventArgs {
		readonly FontFormControllerParameters controllerParameters;
		public FontFormShowingEventArgs(FontFormControllerParameters controllerParameters)
			: base(controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.controllerParameters = controllerParameters;
		}
		public FontFormControllerParameters ControllerParameters { get { return controllerParameters; } }
	}
	#endregion
	#region FloatingObjectLayoutOptionsFormShowingEventHandler
	public delegate void FloatingInlineObjectLayoutOptionsFormShowingEventHandler(object sender, FloatingInlineObjectLayoutOptionsFormShowingEventArgs e);
	#endregion
	#region FloatingInlineObjectLayoutOptionsFormShowingEventArgs
	public class FloatingInlineObjectLayoutOptionsFormShowingEventArgs : FormShowingEventArgs {
		readonly FloatingInlineObjectLayoutOptionsFormControllerParameters controllerParameters;
		public FloatingInlineObjectLayoutOptionsFormControllerParameters ControllerParameters { get { return controllerParameters; } }
		public FloatingInlineObjectLayoutOptionsFormShowingEventArgs(FloatingInlineObjectLayoutOptionsFormControllerParameters controllerParameters)
			: base(controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.controllerParameters = controllerParameters;
		}
	}
	#endregion
	#region ParagraphFormShowingEventHandler
	public delegate void ParagraphFormShowingEventHandler(object sender, ParagraphFormShowingEventArgs e);
	#endregion
	#region ParagraphFormShowingEventArgs
	public class ParagraphFormShowingEventArgs : FormShowingEventArgs {
		readonly ParagraphFormControllerParameters controllerParameters;
		public ParagraphFormShowingEventArgs(ParagraphFormControllerParameters controllerParameters)
			: base(controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.controllerParameters = controllerParameters;
		}
		public ParagraphFormControllerParameters ControllerParameters { get { return controllerParameters; } }
	}
	#endregion
	#region TabsFormShowingEventHandler
	public delegate void TabsFormShowingEventHandler(object sender, TabsFormShowingEventArgs e);
	#endregion
	#region TabsFormShowingEventArgs
	public class TabsFormShowingEventArgs : FormShowingEventArgs {
		readonly TabsFormControllerParameters controllerParameters;
		public TabsFormShowingEventArgs(TabsFormControllerParameters controllerParameters)
			: base(controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.controllerParameters = controllerParameters;
		}
		public TabsFormControllerParameters ControllerParameters { get { return controllerParameters; } }
	}
	#endregion
	#region BookmarkFormShowingEventHandler
	public delegate void BookmarkFormShowingEventHandler(object sender, BookmarkFormShowingEventArgs e);
	#endregion
	#region BookmarkFormShowingEventArgs
	public class BookmarkFormShowingEventArgs : FormShowingEventArgs {
		readonly BookmarkFormControllerParameters controllerParameters;
		public BookmarkFormShowingEventArgs(BookmarkFormControllerParameters controllerParameters)
			: base(controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.controllerParameters = controllerParameters;
		}
		public BookmarkFormControllerParameters ControllerParameters { get { return controllerParameters; } }
	}
	#endregion
	#region EditStyleFormShowingEventHandler
	public delegate void EditStyleFormShowingEventHandler(object sender, EditStyleFormShowingEventArgs e);
	#endregion
	#region EditStyleFormShowingEventArgs
	public class EditStyleFormShowingEventArgs : FormShowingEventArgs {
		readonly EditStyleFormControllerParameters controllerParameters;
		public EditStyleFormShowingEventArgs(EditStyleFormControllerParameters controllerParameters)
			: base(controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.controllerParameters = controllerParameters;
		}
		public EditStyleFormControllerParameters ControllerParameters { get { return controllerParameters; } }
	}
	#endregion
	#region TableStyleFormShowingEventHandler
	public delegate void TableStyleFormShowingEventHandler(object sender, TableStyleFormShowingEventArgs e);
	#endregion
	#region TableStyleFormShowingEventArgs
	public class TableStyleFormShowingEventArgs : FormShowingEventArgs {
		readonly TableStyleFormControllerParameters controllerParameters;
		public TableStyleFormShowingEventArgs(TableStyleFormControllerParameters controllerParameters)
			: base(controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.controllerParameters = controllerParameters;
		}
		public TableStyleFormControllerParameters ControllerParameters { get { return controllerParameters; } }
	}
	#endregion
	#region HyperlinkFormShowingEventHandler
	public delegate void HyperlinkFormShowingEventHandler(object sender, HyperlinkFormShowingEventArgs e);
	#endregion
	#region HyperlinkFormShowingEventArgs
	public class HyperlinkFormShowingEventArgs : FormShowingEventArgs {
		readonly HyperlinkFormControllerParameters controllerParameters;
		public HyperlinkFormShowingEventArgs(HyperlinkFormControllerParameters controllerParameters)
			: base(controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.controllerParameters = controllerParameters;
		}
		public HyperlinkFormControllerParameters ControllerParameters { get { return controllerParameters; } }
	}
	#endregion
	#region RangeEditingPermissionsFormShowingEventHandler
	public delegate void RangeEditingPermissionsFormShowingEventHandler(object sender, RangeEditingPermissionsFormShowingEventArgs e);
	#endregion
	#region RangeEditingPermissionsFormShowingEventArgs
	public class RangeEditingPermissionsFormShowingEventArgs : FormShowingEventArgs {
		readonly RangeEditingPermissionsFormControllerParameters controllerParameters;
		public RangeEditingPermissionsFormShowingEventArgs(RangeEditingPermissionsFormControllerParameters controllerParameters)
			: base(controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.controllerParameters = controllerParameters;
		}
		public RangeEditingPermissionsFormControllerParameters ControllerParameters { get { return controllerParameters; } }
	}
	#endregion
	#region DocumentProtectionQueryNewPasswordFormShowingEventHandler
	public delegate void DocumentProtectionQueryNewPasswordFormShowingEventHandler(object sender, DocumentProtectionQueryNewPasswordFormShowingEventArgs e);
	#endregion
	#region DocumentProtectionQueryNewPasswordFormShowingEventArgs
	public class DocumentProtectionQueryNewPasswordFormShowingEventArgs : FormShowingEventArgs {
		readonly DocumentProtectionQueryNewPasswordFormControllerParameters controllerParameters;
		public DocumentProtectionQueryNewPasswordFormShowingEventArgs(DocumentProtectionQueryNewPasswordFormControllerParameters controllerParameters)
			: base(controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.controllerParameters = controllerParameters;
		}
		public DocumentProtectionQueryNewPasswordFormControllerParameters ControllerParameters { get { return controllerParameters; } }
	}
	#endregion
	#region DocumentProtectionQueryPasswordFormShowingEventHandler
	public delegate void DocumentProtectionQueryPasswordFormShowingEventHandler(object sender, DocumentProtectionQueryPasswordFormShowingEventArgs e);
	#endregion
	#region DocumentProtectionQueryPasswordFormShowingEventArgs
	public class DocumentProtectionQueryPasswordFormShowingEventArgs : FormShowingEventArgs {
		readonly DocumentProtectionQueryPasswordFormControllerParameters controllerParameters;
		public DocumentProtectionQueryPasswordFormShowingEventArgs(DocumentProtectionQueryPasswordFormControllerParameters controllerParameters)
			: base(controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.controllerParameters = controllerParameters;
		}
		public DocumentProtectionQueryPasswordFormControllerParameters ControllerParameters { get { return controllerParameters; } }
	}
	#endregion
	#region LineNumberingFormShowingEventHandler
	public delegate void LineNumberingFormShowingEventHandler(object sender, LineNumberingFormShowingEventArgs e);
	#endregion
	#region LineNumberingFormShowingEventArgs
	public class LineNumberingFormShowingEventArgs : FormShowingEventArgs {
		readonly LineNumberingFormControllerParameters controllerParameters;
		public LineNumberingFormShowingEventArgs(LineNumberingFormControllerParameters controllerParameters)
			: base(controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.controllerParameters = controllerParameters;
		}
		public LineNumberingFormControllerParameters ControllerParameters { get { return controllerParameters; } }
	}
	#endregion
	#region PageSetupFormShowingEventHandler
	public delegate void PageSetupFormShowingEventHandler(object sender, PageSetupFormShowingEventArgs e);
	#endregion
	#region PageSetupFormShowingEventArgs
	public class PageSetupFormShowingEventArgs : FormShowingEventArgs {
		readonly PageSetupFormControllerParameters controllerParameters;
		public PageSetupFormShowingEventArgs(PageSetupFormControllerParameters controllerParameters)
			: base(controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.controllerParameters = controllerParameters;
		}
		public PageSetupFormControllerParameters ControllerParameters { get { return controllerParameters; } }
	}
	#endregion
	#region ColumnsSetupFormShowingEventHandler
	public delegate void ColumnsSetupFormShowingEventHandler(object sender, ColumnsSetupFormShowingEventArgs e);
	#endregion
	#region ColumnsSetupFormShowingEventArgs
	public class ColumnsSetupFormShowingEventArgs : FormShowingEventArgs {
		readonly ColumnsSetupFormControllerParameters controllerParameters;
		public ColumnsSetupFormShowingEventArgs(ColumnsSetupFormControllerParameters controllerParameters)
			: base(controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.controllerParameters = controllerParameters;
		}
		public ColumnsSetupFormControllerParameters ControllerParameters { get { return controllerParameters; } }
	}
	#endregion
	#region PasteSpecialFormShowingEventHandler
	public delegate void PasteSpecialFormShowingEventHandler(object sender, PasteSpecialFormShowingEventArgs e);
	#endregion
	#region PasteSpecialFormShowingEventArgs
	public class PasteSpecialFormShowingEventArgs : FormShowingEventArgs {
		readonly PasteSpecialFormControllerParameters controllerParameters;
		public PasteSpecialFormShowingEventArgs(PasteSpecialFormControllerParameters controllerParameters)
			: base(controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.controllerParameters = controllerParameters;
		}
		public PasteSpecialFormControllerParameters ControllerParameters { get { return controllerParameters; } }
	}
	#endregion
	#region SymbolFormShowingEventHandler
	public delegate void SymbolFormShowingEventHandler(object sender, SymbolFormShowingEventArgs e);
	#endregion
	#region SymbolFormShowingEventArgs
	public class SymbolFormShowingEventArgs : FormShowingEventArgs {
		readonly RichEditInsertSymbolViewModel viewModel;
		public SymbolFormShowingEventArgs(RichEditInsertSymbolViewModel viewModel)
			: base(viewModel.Parameters) {
			Guard.ArgumentNotNull(viewModel, "viewModel");
			this.viewModel = viewModel;
		}
		public RichEditInsertSymbolViewModel ViewModel { get { return viewModel; } }
	}
	#endregion
	#region NumberingListFormShowingEventHandler
	public delegate void NumberingListFormShowingEventHandler(object sender, NumberingListFormShowingEventArgs e);
	#endregion
	#region NumberingListFormShowingEventArgs
	public class NumberingListFormShowingEventArgs : FormShowingEventArgs {
		readonly NumberingListFormControllerParameters controllerParameters;
		public NumberingListFormShowingEventArgs(NumberingListFormControllerParameters controllerParameters)
			: base(controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.controllerParameters = controllerParameters;
		}
		public NumberingListFormControllerParameters ControllerParameters { get { return controllerParameters; } }
	}
	#endregion
	#region SearchFormShowingEventHandler
	public delegate void SearchFormShowingEventHandler(object sender, SearchFormShowingEventArgs e);
	#endregion
	#region SearchFormShowingEventArgs
	public class SearchFormShowingEventArgs : FormShowingEventArgs {
		readonly SearchFormControllerParameters controllerParameters;
		public SearchFormShowingEventArgs(SearchFormControllerParameters controllerParameters)
			: base(controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.controllerParameters = controllerParameters;
		}
		public SearchFormControllerParameters ControllerParameters { get { return controllerParameters; } }
		public SearchFormActivePage ActivePage { get { return controllerParameters.ActivePage; } }
	}
	#endregion
	#region InsertMergeFieldFormShowingEventHandler
	public delegate void InsertMergeFieldFormShowingEventHandler(object sender, InsertMergeFieldFormShowingEventArgs e);
	#endregion
	#region InsertMergeFieldFormShowingEventArgs
	public class InsertMergeFieldFormShowingEventArgs : FormShowingEventArgs {
		readonly InsertMergeFieldFormControllerParameters controllerParameters;
		public InsertMergeFieldFormShowingEventArgs(InsertMergeFieldFormControllerParameters controllerParameters)
			: base(controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.controllerParameters = controllerParameters;
		}
		public InsertMergeFieldFormControllerParameters ControllerParameters { get { return controllerParameters; } }
	}
	#endregion
	#region InsertTableFormShowingEventHandler
	public delegate void InsertTableFormShowingEventHandler(object sender, InsertTableFormShowingEventArgs e);
	#endregion
	#region InsertTableFormShowingEventArgs
	public class InsertTableFormShowingEventArgs : FormShowingEventArgs {
		readonly InsertTableFormControllerParameters controllerParameters;
		public InsertTableFormShowingEventArgs(InsertTableFormControllerParameters controllerParameters)
			: base(controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.controllerParameters = controllerParameters;
		}
		public InsertTableFormControllerParameters ControllerParameters { get { return controllerParameters; } }
	}
	#endregion
	#region InsertTableCellsFormShowingEventHandler
	public delegate void InsertTableCellsFormShowingEventHandler(object sender, InsertTableCellsFormShowingEventArgs e);
	#endregion
	#region InsertTableCellsFormShowingEventArgs
	public class InsertTableCellsFormShowingEventArgs : FormShowingEventArgs {
		readonly InsertTableCellsFormControllerParameters controllerParameters;
		public InsertTableCellsFormShowingEventArgs(InsertTableCellsFormControllerParameters controllerParameters)
			: base(controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.controllerParameters = controllerParameters;
		}
		public InsertTableCellsFormControllerParameters ControllerParameters { get { return controllerParameters; } }
	}
	#endregion
	#region DeleteTableCellsFormShowingEventHandler
	public delegate void DeleteTableCellsFormShowingEventHandler(object sender, DeleteTableCellsFormShowingEventArgs e);
	#endregion
	#region DeleteTableCellsFormShowingEventArgs
	public class DeleteTableCellsFormShowingEventArgs : FormShowingEventArgs {
		readonly DeleteTableCellsFormControllerParameters controllerParameters;
		public DeleteTableCellsFormShowingEventArgs(DeleteTableCellsFormControllerParameters controllerParameters)
			: base(controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.controllerParameters = controllerParameters;
		}
		public DeleteTableCellsFormControllerParameters ControllerParameters { get { return controllerParameters; } }
	}
	#endregion
	#region SplitTableCellsFormShowingEventHandler
	public delegate void SplitTableCellsFormShowingEventHandler(object sender, SplitTableCellsFormShowingEventArgs e);
	#endregion
	#region SplitTableCellsFormShowingEventArgs
	public class SplitTableCellsFormShowingEventArgs : FormShowingEventArgs {
		readonly SplitTableCellsFormControllerParameters controllerParameters;
		public SplitTableCellsFormShowingEventArgs(SplitTableCellsFormControllerParameters controllerParameters)
			: base(controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.controllerParameters = controllerParameters;
		}
		public SplitTableCellsFormControllerParameters ControllerParameters { get { return controllerParameters; } }
	}
	#endregion
	#region TablePropertiesFormShowingEventHandler
	public delegate void TablePropertiesFormShowingEventHandler(object sender, TablePropertiesFormShowingEventArgs e);
	#endregion
	#region TablePropertiesFormShowingEventArgs
	public class TablePropertiesFormShowingEventArgs : FormShowingEventArgs {
		readonly TablePropertiesFormControllerParameters controllerParameters;
		public TablePropertiesFormShowingEventArgs(TablePropertiesFormControllerParameters controllerParameters)
			: base(controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.controllerParameters = controllerParameters;
		}
		public TablePropertiesFormControllerParameters ControllerParameters { get { return controllerParameters; } }
	}
	#endregion.
	#region TableOptionsFormShowingEventHandler
	public delegate void TableOptionsFormShowingEventHandler(object sender, TableOptionsFormShowingEventArgs e);
	#endregion
	#region TableOptionsFormShowingEventArgs
	public class TableOptionsFormShowingEventArgs : FormShowingEventArgs {
		readonly TableOptionsFormControllerParameters controllerParameters;
		public TableOptionsFormShowingEventArgs(TableOptionsFormControllerParameters controllerParameters)
			: base(controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.controllerParameters = controllerParameters;
		}
		public TableOptionsFormControllerParameters ControllerParameters { get { return controllerParameters; } }
	}
	#endregion
#endif
}
