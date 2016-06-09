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
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.ComponentModel.Design;
using DevExpress.Data;
using DevExpress.DataAccess.Localization;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.Excel;
using DevExpress.SpreadsheetSource;
using DevExpress.Utils;
using DevExpress.XtraReports;
namespace DevExpress.DataAccess.Excel {
#if !DXPORTABLE
	[Designer("DevExpress.DataAccess.Design.VSExcelDataSourceDesigner," + AssemblyInfo.SRAssemblyDataAccessDesign, typeof(IDesigner))]
	[XRDesigner("DevExpress.DataAccess.UI.Design.XRExcelDataSourceDesigner," + AssemblyInfo.SRAssemblyDataAccessUI, typeof(IDesigner))]
	[ToolboxBitmap(typeof(ResFinder), "Bitmaps256.ExcelDataSource.bmp")]
#endif
	[ToolboxTabName(AssemblyInfo.DXTabData)]
	[Description("A data source used to extract data from Microsoft Excel workbooks or CSV files.")]
	[DXToolboxItem(true)]
	public class ExcelDataSource : DataComponentBase, IListSource, IListAdapterAsync {
		const string xml_ExcelDataSource = "ExcelDataSource";
		const string xml_Name = "Name";
		const string xml_FileName = "FileName";
		const string xml_Options = "Options";
		const string xml_Schema = "Schema";
		const string xml_SourceOptionsType = "Type";
		protected readonly DataView result;
		public event EventHandler<BeforeFillEventArgs> BeforeFill;
		public ExcelDataSource() {
			Schema = new FieldInfoList();
			result = new DataView(this, SelectedDataEx.Empty);
			((IServiceContainer)this).AddService(typeof(IExcelSchemaProvider), new ExcelSchemaProvider());
		}
		[DefaultValue(null)]
		[LocalizableCategory(DataAccessStringId.PropertyGridDataCategoryName)]
#if !DXPORTABLE
		[Editor("DevExpress.DataAccess.UI.Native.Excel.FileNameEditor, " + AssemblyInfo.SRAssemblyDataAccessUI, typeof(UITypeEditor))]
#endif
		public string FileName { get; set; }
		[DefaultValue(null)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[LocalizableCategory(DataAccessStringId.PropertyGridDataCategoryName)]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public ExcelSourceOptionsBase SourceOptions { get; set; }
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FieldInfoList Schema { get; set; }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public Stream Stream { get; set; }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public ExcelDocumentFormat StreamDocumentFormat { get; set; }
		protected override IEnumerable<IParameter> AllParameters {
			get { return new IParameter[0]; }
		}
		public void Fill() {
			Fill(null);
		}
		public override XElement SaveToXml() {
			XElement xElement = new XElement(xml_ExcelDataSource);
			if(!string.IsNullOrEmpty(Name))
				xElement.Add(new XAttribute(xml_Name, Name));
			if(!string.IsNullOrEmpty(FileName))
				xElement.Add(new XAttribute(xml_FileName, FileName));
			if(SourceOptions != null) {
				var sourceOptions = new XElement(xml_Options);
				sourceOptions.Add(new XAttribute(xml_SourceOptionsType, SourceOptions.GetType().FullName));
				SourceOptions.SaveToXml(sourceOptions);
				xElement.Add(sourceOptions);
			}
			var schema = new XElement(xml_Schema);
			Schema.SaveToXml(schema);
			xElement.Add(schema);
			return xElement;
		}
		public override void LoadFromXml(XElement xElement) {
			Reset();
			Name = xElement.GetAttributeValue(xml_Name);
			FileName = xElement.GetAttributeValue(xml_FileName);
			var sourceOptions = xElement.Element(xml_Options);
			if(sourceOptions != null) {
				string settingsType = sourceOptions.GetAttributeValue(xml_SourceOptionsType);
				ExcelSourceOptionsBase options =
					(ExcelSourceOptionsBase)Activator.CreateInstance(Type.GetType(settingsType));
				options.LoadFromXml(sourceOptions);
				SourceOptions = options;
			}
			var schema = xElement.Element(xml_Schema);
			if(schema != null)
				Schema.LoadFromXml(schema);
		}
		protected override string GetDataMember() {
			return string.Empty;
		}
		protected override void Fill(IEnumerable<IParameter> sourceParameters) {
			((IListAdapter)this).FillList(null);
		}
		internal Task<SelectedDataEx> FillAsync(CancellationToken cancellationToken) {
			return Task.Factory.StartNew(() => {
				var task = FillCoreAsync(cancellationToken);
				task.Wait(cancellationToken);
				return task.Result;
			}, cancellationToken);
		}
		bool ShouldSerializeSourceOptions() {
			return !SourceOptions.IsDefault;
		}
		Task<SelectedDataEx> FillCoreAsync(CancellationToken token) {
			return Task.Factory.StartNew(() => {
				ExcelSourceOptionsBase optionsBaseClone = SourceOptions != null ? SourceOptions.Clone() : null;
				BeforeFillEventArgs eventArgs = new BeforeFillEventArgs {
					SourceOptions = optionsBaseClone,
					Stream = Stream,
					StreamDocumentFormat = StreamDocumentFormat,
					FileName = FileName
				};
				RaiseBeforeFill(eventArgs);
				var excelOptionsCustomizationService = (IExcelOptionsCustomizationService)GetService(typeof(IExcelOptionsCustomizationService));
				if(excelOptionsCustomizationService != null)
					excelOptionsCustomizationService.Customize(eventArgs);
				if(!CanFill(eventArgs)) {
					return SelectedDataEx.Empty;
				}
				var schema = GetSchema(optionsBaseClone);
				ValidateSchema(schema);
				using(ISpreadsheetSource source = ExcelDataLoaderHelper.CreateSource(eventArgs.Stream, eventArgs.StreamDocumentFormat, eventArgs.FileName, eventArgs.SourceOptions)) {
					return ExcelDataLoaderHelper.LoadData(source, schema, eventArgs.SourceOptions, false) ?? SelectedDataEx.Empty;
				}
			}, token);
		}
		bool CanFill(BeforeFillEventArgs eventArgs) {
			return (eventArgs.Stream != null || !string.IsNullOrEmpty(eventArgs.FileName)) && (eventArgs.SourceOptions != null);
		}
		void Reset() {
			Name = null;
			FileName = null;
			SourceOptions = null;
			Schema.Clear();
		}
		void ValidateSchema(FieldInfo[] schema) {
			foreach(var fieldInfo in schema) {
				if(string.IsNullOrEmpty(fieldInfo.Name))
					throw new NameIsNullOrEmptyValidationException();
				if(fieldInfo.Type == null)
					throw new TypeIsNullValidationException();
				if(Schema.Any(fi => fi != fieldInfo && fi.Name == fieldInfo.Name))
					throw new DuplicateNameValidationException(fieldInfo.Name);
			}
		}
		FieldInfo[] GetSchema(ExcelSourceOptionsBase excelSourceOptionsBase) {
			if(Schema.Count > 0)
				return Schema.ToArray();
			var excelSchemaProvider = (IExcelSchemaProvider)GetService(typeof(IExcelSchemaProvider));
			if(excelSchemaProvider == null)
				throw new InvalidOperationException("No IExcelSchemaProvider found");
			return excelSchemaProvider.GetSchema(FileName, Stream, StreamDocumentFormat, excelSourceOptionsBase, CancellationToken.None).ToArray();
		}
		void RaiseBeforeFill(BeforeFillEventArgs args) {
			if(BeforeFill != null) {
				BeforeFill(this, args);
			}
		}
		#region Implementation of IListSource
		IList IListSource.GetList() {
			if(result.Count == 0) {
				result.SetColumns(new SelectedDataEx(new IList[Schema.Count(s => s.Selected)], Schema.Where(s => s.Selected).Select(s => new ColumnInfoEx { Name = s.Name, Type = s.Type }).ToArray()));
			}
			return result;
		}
		bool IListSource.ContainsListCollection {
			get { return false; }
		}
		#endregion
		#region Implementation of IListAdapter
		void IListAdapter.FillList(IServiceProvider serviceProvider) {
			var listAdapterAsync = (IListAdapterAsync)this;
			var task = listAdapterAsync.BeginFillList(serviceProvider, CancellationToken.None);
			listAdapterAsync.EndFillList(task);
		}
		bool IListAdapter.IsFilled { get { return result.Count > 0; } }
		#endregion
		#region Implementation of IListAdapterAsync
		readonly Semaphore fillListSemaphore = new Semaphore(1, 1);
		IAsyncResult IListAdapterAsync.BeginFillList(IServiceProvider serviceProvider, CancellationToken token) {
			if(!fillListSemaphore.WaitOne(300))
				throw new InvalidOperationException("Data source is busy");
			return FillAsync(token);
		}
		void IListAdapterAsync.EndFillList(IAsyncResult result) {
			Task<SelectedDataEx> task = (Task<SelectedDataEx>)result;
			try {
				task.Wait();
				this.result.SetColumns(task.Result);
			}
			catch(AggregateException e) {
				if(task.IsFaulted)
					throw ExceptionHelper.Unwrap(e);
			}
			finally {
				fillListSemaphore.Release();
			}
		}
		#endregion
	}
}
