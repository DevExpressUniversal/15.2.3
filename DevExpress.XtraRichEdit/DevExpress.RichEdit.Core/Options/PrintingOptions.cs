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
using System.Runtime.InteropServices;
using System.ComponentModel;
using DevExpress.Utils.Serializing;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraRichEdit {
	#region PrintingOptions
	[ComVisible(true)]
	public class PrintingOptions : RichEditNotificationOptions {
		#region Fields
		const bool enablePageBackgroundOnPrintDefault = false;
		const bool enableCommentBackgroundOnPrintDefault = true;
		const bool enableCommentFillOnPrintDefault = true;
		const bool updateDocVariablesBeforePrintDefaultValue = true;
		bool enablePageBackgroundOnPrint;
		bool enableCommentBackgroundOnPrint = true;
		bool enableCommentFillOnPrint = true;
		bool updateDocVariablesBeforePrint;
#if!SL
		bool drawLayoutFromSilverlightRendering;
#endif
		const PrintPreviewFormKind printPreviewFormKindDefaultValue = PrintPreviewFormKind.Ribbon;
		PrintPreviewFormKind printPreviewFormKind;
		#endregion
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[Obsolete("Use EnablePageBackgroundOnPrint instead.")]
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("PrintingOptionsPrintPageBackground")]
#endif
		[NotifyParentProperty(true), DefaultValue(enablePageBackgroundOnPrintDefault), XtraSerializableProperty()]
		public bool PrintPageBackground {
			get { return EnablePageBackgroundOnPrint; }
			set { EnablePageBackgroundOnPrint = value; }
		}
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("PrintingOptionsEnablePageBackgroundOnPrint"),
#endif
		NotifyParentProperty(true), DefaultValue(enablePageBackgroundOnPrintDefault), XtraSerializableProperty()]
		public bool EnablePageBackgroundOnPrint
		{
			get { return enablePageBackgroundOnPrint; }
			set
			{
				if (enablePageBackgroundOnPrint == value)
					return;
				bool oldValue = enablePageBackgroundOnPrint;
				enablePageBackgroundOnPrint = value;
				OnChanged("EnablePageBackgroundOnPrint", oldValue, value);
			}
		}
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("PrintingOptionsEnableCommentBackgroundOnPrint"),
#endif
		NotifyParentProperty(true), DefaultValue(enableCommentBackgroundOnPrintDefault), XtraSerializableProperty()]
		public bool EnableCommentBackgroundOnPrint {
			get { return enableCommentBackgroundOnPrint; }
			set {
				if (enableCommentBackgroundOnPrint == value)
					return;
				bool oldValue = enableCommentBackgroundOnPrint;
				enableCommentBackgroundOnPrint = value;
				OnChanged("EnableCommentBackgroundOnPrint", oldValue, value);
			}
		}
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("PrintingOptionsEnableCommentFillOnPrint"),
#endif
		NotifyParentProperty(true), DefaultValue(enableCommentFillOnPrintDefault), XtraSerializableProperty()]
		public bool EnableCommentFillOnPrint {
			get { return enableCommentFillOnPrint; }
			set {
				if (enableCommentFillOnPrint == value)
					return;
				bool oldValue = enableCommentFillOnPrint;
				enableCommentFillOnPrint = value;
				OnChanged("EnableCommentFillOnPrint", oldValue, value);
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[Obsolete("Use UpdateDocVariablesBeforePrint instead.")]
		[NotifyParentProperty(true), DefaultValue(updateDocVariablesBeforePrintDefaultValue), XtraSerializableProperty()]
		public bool DocVariableFieldsUpdateBeforePrint {
			get { return UpdateDocVariablesBeforePrint; }
			set { UpdateDocVariablesBeforePrint = value; }
		}
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("PrintingOptionsUpdateDocVariablesBeforePrint"),
#endif
		NotifyParentProperty(true), DefaultValue(updateDocVariablesBeforePrintDefaultValue), XtraSerializableProperty()]
		public bool UpdateDocVariablesBeforePrint
		{
			get { return updateDocVariablesBeforePrint; }
			set
			{
				if (updateDocVariablesBeforePrint == value)
					return;
				bool oldValue = updateDocVariablesBeforePrint;
				updateDocVariablesBeforePrint = value;
				OnChanged("UpdateDocVariablesBeforePrint", oldValue, value);
			}
		}
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("PrintingOptionsPrintPreviewFormKind"),
#endif
		NotifyParentProperty(true), DefaultValue(printPreviewFormKindDefaultValue), XtraSerializableProperty()]
		public PrintPreviewFormKind PrintPreviewFormKind
		{
			get { return printPreviewFormKind; }
			set
			{
				if (printPreviewFormKind == value)
					return;
				PrintPreviewFormKind oldValue = printPreviewFormKind;
				printPreviewFormKind = value;
				OnChanged("PrintPreviewFormKind", oldValue, value);
			}
		}
#if!SL
		[Browsable(false)]
		[NotifyParentProperty(true), DefaultValue(false), XtraSerializableProperty()]
		public bool DrawLayoutFromSilverlightRendering {
			get { return drawLayoutFromSilverlightRendering; }
			set {
				if (drawLayoutFromSilverlightRendering == value)
					return;
				bool oldValue = drawLayoutFromSilverlightRendering;
				drawLayoutFromSilverlightRendering = value;
				OnChanged("DrawLayoutFromSilverlightRendering", oldValue, value);
			}
		}
#endif
		protected internal override void ResetCore() {
			EnablePageBackgroundOnPrint = enablePageBackgroundOnPrintDefault;
			UpdateDocVariablesBeforePrint = updateDocVariablesBeforePrintDefaultValue;
			EnableCommentBackgroundOnPrint = enableCommentBackgroundOnPrintDefault;
			EnableCommentFillOnPrint = enableCommentFillOnPrintDefault;
#if!SL
			DrawLayoutFromSilverlightRendering = false;
#endif
			PrintPreviewFormKind = printPreviewFormKindDefaultValue;
		}		
	}
	#endregion
	#region PrintPreviewFormKind
	public enum PrintPreviewFormKind {
		Bars,
		Ribbon
	}
	#endregion
}
