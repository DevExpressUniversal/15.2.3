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
using System.Text;
using DevExpress.XtraPrinting.Native.ExportOptionsControllers;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Localization;
using DevExpress.XtraPrinting.Native.Lines;
using System.ComponentModel;
#if SILVERLIGHT
using DevExpress.Data.Browsing;
#endif
namespace DevExpress.XtraPrinting.Native.ExportOptionsControllers {
	public class XpsExportOptionsController : ExportOptionsControllerBase {
		protected override Type ExportOptionsType {
			get { return typeof(XpsExportOptions); }
		}
		public override PreviewStringId CaptionStringId {
			get { return PreviewStringId.ExportOptionsForm_CaptionXps; }
		}
		public override string[] GetExportedFileNames(PrintingSystemBase ps, ExportOptionsBase options, string fileName) {
			ps.ExportToXps(fileName, (XpsExportOptions)options);
			return new string[] { fileName };
		}
		protected override string[] LocalizerStrings {
			get { return new string[] { PreviewLocalizer.GetString(PreviewStringId.SaveDlg_FilterXps) }; }
		}
		public override string[] FileExtensions {
			get { return new string[] { ".xps" }; }
		}
		protected override void CollectLineControllers(ExportOptionsBase options, List<ExportOptionKind> hiddenOptions, List<BaseLineController> list) {
			AddPageRangeLineControllerToList(hiddenOptions, options, list, ExportOptionKind.XpsPageRange);
			AddControllerToList(hiddenOptions, options, list, ExportOptionsPropertiesNames.Xps.Compression, typeof(PSDropDownLineController), ExportOptionKind.XpsCompression);
			AddSeparatorToList(list);
			XpsDocumentOptions documentOptions = ((XpsExportOptions)options).DocumentOptions;
			PropertyDescriptorCollection documentOptionsProperties = TypeDescriptor.GetProperties(documentOptions);
			AddControllerToList(hiddenOptions, list, documentOptionsProperties, ExportOptionsPropertiesNames.Xps.DocumentOptions.Creator, documentOptions, typeof(PSTextLineController), ExportOptionKind.XpsDocumentCreator);
			AddControllerToList(hiddenOptions, list, documentOptionsProperties, ExportOptionsPropertiesNames.Xps.DocumentOptions.Category, documentOptions, typeof(PSTextLineController), ExportOptionKind.XpsDocumentCategory);
			AddControllerToList(hiddenOptions, list, documentOptionsProperties, ExportOptionsPropertiesNames.Xps.DocumentOptions.Title, documentOptions, typeof(PSTextLineController), ExportOptionKind.XpsDocumentTitle);
			AddControllerToList(hiddenOptions, list, documentOptionsProperties, ExportOptionsPropertiesNames.Xps.DocumentOptions.Subject, documentOptions, typeof(PSTextLineController), ExportOptionKind.XpsDocumentSubject);
			AddControllerToList(hiddenOptions, list, documentOptionsProperties, ExportOptionsPropertiesNames.Xps.DocumentOptions.Keywords, documentOptions, typeof(PSTextLineController), ExportOptionKind.XpsDocumentKeywords);
			AddControllerToList(hiddenOptions, list, documentOptionsProperties, ExportOptionsPropertiesNames.Xps.DocumentOptions.Version, documentOptions, typeof(PSTextLineController), ExportOptionKind.XpsDocumentVersion);
			AddControllerToList(hiddenOptions, list, documentOptionsProperties, ExportOptionsPropertiesNames.Xps.DocumentOptions.Description, documentOptions, typeof(PSTextLineController), ExportOptionKind.XpsDocumentDescription);   
		}
	}
}
