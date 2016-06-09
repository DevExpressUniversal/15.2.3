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

using DevExpress.XtraPrinting.Localization;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Native.Lines;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Collections;
using DevExpress.XtraPrinting.Native.ExportOtions;
#if SILVERLIGHT
using DevExpress.Data.Browsing;
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
#endif
namespace DevExpress.XtraPrinting.Native.ExportOptionsControllers {
	public abstract class ExportOptionsControllerBase {
		#region static
#if DEBUGTEST
		public static ExportOptionsControllerBase[] ControllersTEST {
			get { return controllers; }
			set { controllers = value; }
		}
#endif
		static ExportOptionsControllerBase[] controllers;
		static ExportOptionsControllerBase() {
			controllers = new ExportOptionsControllerBase[] {
				new PdfExportOptionsController(),
				new XlsExportOptionsController(),
				new XlsxExportOptionsController(),
				new TextExportOptionsController(),
				new CsvExportOptionsController(),
				new ImageExportOptionsController(),
				new HtmlExportOptionsController(),
				new MhtExportOptionsController(),
				new RtfExportOptionsController(),
				new NativeFormatOptionsController(),
				new XpsExportOptionsController()
			};
		}
		public static ExportOptionsControllerBase GetControllerByOptions(ExportOptionsBase options) {
			Type exportOptionsType = options.GetType();
			foreach(ExportOptionsControllerBase controller in controllers)
				if(controller.ExportOptionsType == exportOptionsType)
					return controller;
			return ExceptionHelper.ThrowInvalidOperationException<ExportOptionsControllerBase>();
		}
#if DEBUGTEST
		internal static BaseLineController[] GetLineControllers(List<ExportOptionKind> hiddenOptions, ExportOptionsBase options) {
			return GetControllerByOptions(options).GetLineControllers(options, hiddenOptions);
		}
#endif
		protected static void AddSeparatorToList(List<BaseLineController> list) {
			if(list.Count == 0 || list[list.Count - 1] is SeparatorLineController)
				return;
			list.Add(new SeparatorLineController());
		}
		protected static void AddControllerToList(List<ExportOptionKind> hiddenOptions, ExportOptionsBase options, List<BaseLineController> list, string propertyName, Type lineType, ExportOptionKind optionKind) {
			AddControllerToList(hiddenOptions, list, TypeDescriptor.GetProperties(options), propertyName, options, lineType, optionKind);
		}
		protected static void AddControllerToList(List<ExportOptionKind> hiddenOptions, List<BaseLineController> list, PropertyDescriptorCollection properties, string propertyName, object instance, Type lineType, ExportOptionKind optionKind) {
			PropertyDescriptor property = properties[propertyName];
			if(property == null || (hiddenOptions != null && hiddenOptions.Contains(optionKind)))
				return;
			PSPropertyLineController controller = (PSPropertyLineController)Activator.CreateInstance(lineType, property, instance, ExportOptionsLocalizer.GetLocalizedOption(optionKind));
			list.Add(controller);
		}
		protected static void AddEmptySpaceToList(List<BaseLineController> list) {
			list.Add(new EmptyLineController());
		}
		static protected void AddPageRangeLineControllerToList(List<ExportOptionKind> hiddenOptions, ExportOptionsBase options, List<BaseLineController> list, ExportOptionKind optionKind) {
			AddControllerToList(hiddenOptions, options, list, ExportOptionsPropertiesNames.PageByPage.PageRange, typeof(PSTextLineController), optionKind);
		}
		#endregion
		protected abstract Type ExportOptionsType { get; }
		public abstract PreviewStringId CaptionStringId { get; }
		public string Filter {
			get {
				StringBuilder stringBuilder = new StringBuilder();
				for(int i = 0; i < FileExtensions.Length; i++) {
					stringBuilder.AppendFormat("{0} (*{1})|*{1}|", LocalizerStrings[i], FileExtensions[i]);
				}
				return stringBuilder.ToString(0, stringBuilder.Length - 1);
			}
		}
		protected ExportOptionsControllerBase() {
		}
		public abstract string[] GetExportedFileNames(PrintingSystemBase ps, ExportOptionsBase options, string fileName);
		public virtual bool ValidateInputFileName(ExportOptionsBase options) {
			return true;
		}
		protected abstract string[] LocalizerStrings {
			get;
		}
		public abstract string[] FileExtensions {
			get;
		}
		public virtual string GetFileExtension(ExportOptionsBase options){
			return FileExtensions[0];
		}
		public virtual int GetFilterIndex(ExportOptionsBase options) {
			return 1;
		}
		internal BaseLineController[] GetLineControllers(ExportOptionsBase options, List<ExportOptionKind> hiddenOptions) {
			List<BaseLineController> result = new List<BaseLineController>();
			CollectLineControllers(options, hiddenOptions, result);
			AddSeparatorToList(result);
			return result.ToArray();
		}
		protected abstract void CollectLineControllers(ExportOptionsBase options, List<ExportOptionKind> hiddenOptions, List<BaseLineController> list);
		public virtual ILine[] GetExportLines(ExportOptionsBase options, LineFactoryBase lineFactory, AvailableExportModes availableExportModes, List<ExportOptionKind> hiddenOptions) {
			Type exportModeType = GetExportModeType();			
			if(exportModeType != null && availableExportModes != null) {
				object[] modes = availableExportModes.GetExportModesByType(exportModeType);
				ExportModeTypeProvider provider = new ExportModeTypeProvider(modes, exportModeType);
				TypeDescriptor.AddProvider(provider, exportModeType);
				PropertyDescriptor exportModePropertyDescriptor = GetExportModePropertyDescriptor(options);
				exportModePropertyDescriptor.SetValue(options, provider.Validate(exportModePropertyDescriptor.GetValue(options)));
				try {
					return BaseLineController.GetLines(GetLineControllers(options, hiddenOptions), lineFactory);
				} finally {
					TypeDescriptor.RemoveProvider(provider, exportModeType);
				}
			} else {
				return BaseLineController.GetLines(GetLineControllers(options, hiddenOptions), lineFactory);
			}
		}
		protected virtual Type GetExportModeType() {
			return null;
		}
		protected PropertyDescriptor GetExportModePropertyDescriptor(ExportOptionsBase options) {
			return TypeDescriptor.GetProperties(options)[ExportModePropertyName];
		}
		protected virtual string ExportModePropertyName {
			get {
				return string.Empty;
			}
		}
	}
}
