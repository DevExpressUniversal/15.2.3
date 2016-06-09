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
using System.Text;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.DesignService;
using DevExpress.XtraEditors;
using DevExpress.XtraCharts.Native;
using DevExpress.Data.Browsing.Design;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Native.WinControls;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Native;
using DevExpress.Data.Browsing;
using DevExpress.XtraReports.Native.Data;
using System.Collections;
using DevExpress.XtraEditors.Design;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Design;
using DevExpress.XtraPrinting.Drawing;
using System.Collections.Generic;
using DevExpress.XtraPrinting.BarCode;
using DevExpress.XtraReports.Parameters;
using System.Linq;
using DevExpress.Data;
using DevExpress.XtraReports.Localization;
namespace DevExpress.XtraReports.Design.PivotGrid {
	using DevExpress.XtraPivotGrid.Data;
	using DevExpress.XtraReports.UI.PivotGrid;
	using DevExpress.Utils.Design;
	using DevExpress.XtraPivotGrid;
	public class XRPivotFieldNameEditor : FieldNameEditorBase {
		static PivotGridData GetViewInfoData(ITypeDescriptorContext context) {
			object instanceValue = DXObjectWrapper.GetInstance(context);
			if(instanceValue is IPivotGridDataContainer)
				return ((IPivotGridDataContainer)instanceValue).Data;
			if(instanceValue is PivotGridFieldSortBySummaryInfo) {
				IPivotGridDataContainer owner = ((PivotGridFieldSortBySummaryInfo)instanceValue).Owner as IPivotGridDataContainer;
				if(owner != null) return owner.Data;
			}
			return null;
		}
		protected override Pair<object, string> GetDataInfo(IServiceProvider provider, ITypeDescriptorContext context) {
			XRPivotGridData data = GetViewInfoData(context) as XRPivotGridData;
			if(data == null)
				return null;
			if(!String.IsNullOrEmpty(data.OLAPConnectionString))
				return new Pair<object,string>(data.GetDesignOLAPDataSourceObject(), string.Empty);
			return new Pair<object,string>(data.XRPivotGrid.GetEffectiveDataSource(), data.XRPivotGrid.DataMember);
		}
	}
}
namespace DevExpress.XtraReports.Design {
	abstract public class TypePickEditor : UITypeEditor {
		private TypePickerBase typePicker;
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if(provider != null) {
				try {
					if(typePicker == null || typePicker.IsDisposed)
						typePicker = CreateTypePicker(provider);
					string typeName = (value != null) ? value.GetType().FullName : string.Empty;
					IWindowsFormsEditorService editServ = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
					typePicker.Start(context, editServ, typeName);
					editServ.DropDownControl(typePicker);
					if(typeName != typePicker.TypeName) {
						try {
							value = CreateValue(provider, typePicker.TypeName);
						} catch {
							value = null;
						}
					}
					typePicker.End();
				} catch { }
			}
			return value;
		}
		protected abstract TypePickerBase CreateTypePicker(IServiceProvider provider);
		protected virtual object CreateValue(IServiceProvider provider, string typeName) {
			IDesignerHost host = provider.GetService(typeof(IDesignerHost)) as IDesignerHost;
			Type type = host.GetType(typeName);
			if(type == null && DesignToolHelper.IsEndUserDesigner(host)) {
				string fullAssemblyName = System.Reflection.Assembly.GetEntryAssembly().FullName;
				string fullTypeName = string.Format("{0},{1}", typeName, fullAssemblyName.Substring(0, fullAssemblyName.IndexOf(",")));
				type = host.GetType(fullTypeName);
			}
			return type != null ? Activator.CreateInstance(type) as IComponent : null;
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			if(context != null && context.Instance != null) {
				return UITypeEditorEditStyle.DropDown;
			}
			return base.GetEditStyle(context);
		}
	}
	public class ReportSourceEditor : TypePickEditor {
		static void DisposeObject(IDisposable obj) {
			if(obj != null) obj.Dispose();
		}
		static void AddToContainer(IServiceProvider provider, IComponent component) {
			if(component != null) {
				IDesignerHost host = provider.GetService(typeof(IDesignerHost)) as IDesignerHost;
				DesignToolHelper.AddToContainer(host, component);
			}
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			object oldValue = value;
			value = base.EditValue(context, provider, value);
			if(oldValue != value) {
				AddToContainer(provider, value as IComponent);
				DisposeObject(oldValue as IDisposable);
			}
			return value;
		}
		protected override TypePickerBase CreateTypePicker(IServiceProvider provider) {
			if(provider != null) {
				IDesignerHost host = provider.GetService(typeof(IDesignerHost)) as IDesignerHost;
				if(host != null) {
					ReportDesigner designer = host.GetDesigner(host.RootComponent) as ReportDesigner;
					if(designer != null)
						return designer.CreateReportSourcePicker();
				}
			}
			return new ReportSourcePicker();
		}
	}
	public abstract class FieldNameEditorBase : UITypeEditor {
		PopupFieldNamePicker picker;
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if(provider != null) {
				try {
					IWindowsFormsEditorService editServ = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
					if(picker == null || picker.IsDisposed)
						picker = new PopupFieldNamePicker();
					Pair<object, string> dataPair = GetDataInfo(provider, context);
					picker.Start(provider, dataPair.First, dataPair.Second, value, null);
					editServ.DropDownControl(picker);
					value = picker.EndFieldNamePicker();
				} catch { }
			}
			return value;
		}
		protected abstract Pair<object, string> GetDataInfo(IServiceProvider provider, ITypeDescriptorContext context);
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			if(context != null && context.Instance != null) {
				return UITypeEditorEditStyle.DropDown;
			}
			return base.GetEditStyle(context);
		}
	}
	public class FieldNameEditor : FieldNameEditorBase {
		#region static
		static XtraReportBase GetReport(ITypeDescriptorContext context) {
			if(context == null) return null;
			if(context.Instance is GroupField) {
				GroupField groupField = (GroupField)context.Instance;
				Band band = groupField.Band;
				if(band != null)
					return (band is XtraReportBase) ? (XtraReportBase)band : band.Report;
			} else if(context.Instance is XRGroupSortingSummary) {
				XRGroupSortingSummary summary = (XRGroupSortingSummary)context.Instance;
				GroupHeaderBand band = summary.Band;
				if(band != null)
					return band.Report;
			}
			return null;
		}
		#endregion
		protected override Pair<object, string> GetDataInfo(IServiceProvider provider, ITypeDescriptorContext context) {
			XtraReportBase report = GetReport(context);
			return new Pair<object, string>(ReportHelper.GetEffectiveDataSource(report), report.DataMember);
		}
	}
	public class DataContainerFieldNameEditor : FieldNameEditorBase {
		protected override Pair<object, string> GetDataInfo(IServiceProvider provider, ITypeDescriptorContext context) {
			IDataContainer dataContainer = (IDataContainer)context.Instance;
			return new Pair<object, string>() { First = dataContainer.GetEffectiveDataSource(), Second = dataContainer.DataMember };
		}
	}
	public class ReportDataMemberEditor : DataMemberListEditor {
		protected override object GetDataSource(object instance, IServiceProvider provider) {
			if(instance is XtraReportBase) {
				return ReportHelper.GetEffectiveDataSource((XtraReportBase)instance);
			}
			return null;
		}
	}
	public class DataContainerDataMemberEditor : DataMemberListEditor {
		protected override object GetDataSource(object instance, IServiceProvider provider) {
			if(instance is IDataContainer)
				return ((IDataContainer)instance).GetEffectiveDataSource();
			return null;
		}
	}
	public class BandCollectionEditor : UITypeEditor {
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object objValue) {
			if(provider != null) {
				try {
					BandCollectionEditorForm editorForm = new BandCollectionEditorForm(provider);
					try {
						editorForm.EditValue = objValue as BandCollection;
						DialogRunner.ShowDialog(editorForm, provider);
					} finally {
						editorForm.Dispose();
					}
				} catch { }
			}
			return objValue;
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			if(context != null && context.Instance != null) {
				return UITypeEditorEditStyle.Modal;
			}
			return base.GetEditStyle(context);
		}
	}
	public class FormattingRulesEditor : UITypeEditor {
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object objValue) {
			object[] controls = context.Instance as object[];
			if(controls == null && context.Instance is XRControl)
				controls = new object[] { context.Instance as XRControl };
			if(provider != null && controls.Length > 0) {
				try {
					FormattingRulesEditorForm editorForm = new FormattingRulesEditorForm(provider, controls);
					try {
						DialogRunner.ShowDialog(editorForm, provider.GetService<IDesignerHost>());
						return editorForm.EditValue;
					} finally {
						editorForm.Dispose();
					}
				} catch { }
			}
			return objValue;
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.Modal;
		}
	}
	public class FileNameEditor : UITypeEditor {
		protected OpenFileDialog openFileDialog;
		public FileNameEditor() {
			openFileDialog = new OpenFileDialog();
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider,
			object value) {
			DialogResult dialogResult = DevExpress.XtraPrinting.Native.DialogRunner.ShowDialog(openFileDialog);
			return dialogResult == DialogResult.OK ? openFileDialog.FileName : value;
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext typeDescriptorContext) {
			return UITypeEditorEditStyle.Modal;
		}
	}
	public class XmlFileNameEditor : FileNameEditor {
		public XmlFileNameEditor()
			: base() {
			openFileDialog.Filter = "XML files (*.xml)|*.xml";
		}
	}
	public class StyleSheetFileNameEditor : FileNameEditor {
		public StyleSheetFileNameEditor()
			: base() {
			openFileDialog.Filter = "StyleSheet files (*.repss)|*.repss";
		}
	}
	public class ImageFileNameEditor : System.Windows.Forms.Design.FileNameEditor {
		protected override void InitializeDialog(OpenFileDialog openFileDialog) {
			ImageCodecInfo[] decoders = ImageCodecInfo.GetImageDecoders();
			StringBuilder filterBuilder = new StringBuilder();
			StringBuilder extensionsBuilder = new StringBuilder();
			foreach(ImageCodecInfo info in decoders) {
				filterBuilder.AppendFormat(String.Format("{0} ({1})|{1}|", info.FormatDescription, info.FilenameExtension));
				if(extensionsBuilder.Length > 0)
					extensionsBuilder.Append(";");
				extensionsBuilder.Append(info.FilenameExtension);
			}
			filterBuilder.Append(String.Format("All Image files ({0})|{0}|", extensionsBuilder.ToString()));
			filterBuilder.Append("All files (*.*)|*.*");
			openFileDialog.Filter = filterBuilder.ToString();
			openFileDialog.FilterIndex = decoders.Length + 1;
		}
	}
	public class ImageEditor : ImageFileNameEditor {
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			try {
				string name = (string)base.EditValue(context, provider, String.Empty);
				return String.IsNullOrEmpty(name) ? value : Image.FromFile(name);
			} catch {
				return null;
			}
		}
	}
	public class GroupFieldCollectionEditor : DevExpress.Utils.UI.CollectionEditor {
		public GroupFieldCollectionEditor(Type type)
			: base(type) {
		}
		protected override Utils.UI.CollectionEditorFormBase CreateCollectionForm(IServiceProvider serviceProvider) {
			return new GroupFieldCollectionEditorForm(serviceProvider, this);
		}
	}
	public class SubBandCollectionEditor : DevExpress.Utils.UI.CollectionEditor {
		public SubBandCollectionEditor(Type type)
			: base(type) {
		}
		protected override Utils.UI.CollectionEditorFormBase CreateCollectionForm(IServiceProvider serviceProvider) {
			return new SubBandCollectionEditorForm(serviceProvider, this);
		}
	}
	public class CalculatedFieldCollectionEditor : DevExpress.Utils.UI.CollectionEditor {
		public CalculatedFieldCollectionEditor(Type type)
			: base(type) {
			SelectPrimarySelection = true;
		}
		protected override Utils.UI.CollectionEditorFormBase CreateCollectionForm(IServiceProvider serviceProvider) {
			return new CalculatedFieldCollectionEditorForm(serviceProvider, this);
		}
	}
	public class ParameterCollectionEditor : DevExpress.Utils.UI.CollectionEditor {
		public ParameterCollectionEditor()
			: base(typeof(ParameterCollection)) {
			SelectPrimarySelection = true;
		}
		protected override Utils.UI.CollectionEditorFormBase CreateCollectionForm(IServiceProvider serviceProvider) {
			return new ParameterCollectionEditorForm(serviceProvider, this);
		}
	}
	public class LookUpSettingsFilterStringEditor : UITypeEditor {
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object objValue) {
			if(provider != null) {
				var dataContainer = context.Instance as IDataContainer;
				var lookUpSettings = (context.Instance as LookUpSettings);
				var report = provider.GetService<IDesignerHost>().RootComponent as XtraReport;
				var editingParameter = lookUpSettings != null ? lookUpSettings.Parameter : null;
				var parentParameters = report != null
					? report.Parameters.Cast<IParameter>().TakeWhile(x => x != editingParameter)
					: Enumerable.Empty<IParameter>();
				FilterStringEditorForm editor = new FilterStringEditorForm(provider, dataContainer.GetEffectiveDataSource(), dataContainer.DataMember, parentParameters, provider.GetService<IExtensionsProvider>());
				editor.FilterString = (string)objValue;
				if(DialogRunner.ShowDialog(editor, provider) != DialogResult.Cancel)
					objValue = editor.FilterString;
			}
			return objValue;
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			if(context != null && context.Instance != null) {
				return UITypeEditorEditStyle.Modal;
			}
			return base.GetEditStyle(context);
		}
	}
	public class FilterStringEditor : UITypeEditor {
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object objValue) {
			if(provider != null) {
				XtraReportBase report = (XtraReportBase)context.Instance;
				FilterStringEditorForm editor = new FilterStringEditorForm(provider, ReportHelper.GetEffectiveDataSource(report), report.DataMember, report.RootReport.Parameters, report.RootReport);
				editor.FilterString = (string)objValue;
				if(DialogRunner.ShowDialog(editor, provider) != DialogResult.Cancel)
					objValue = editor.FilterString;
			}
			return objValue;
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			if(context != null && context.Instance != null) {
				return UITypeEditorEditStyle.Modal;
			}
			return base.GetEditStyle(context);
		}
	}
	public class ScriptEditor : UITypeEditor {
		static bool IsNewOrCurrent(string value) {
			return value == DesignSR.DataGridNewString || value == DesignSR.PropertyGridCurrentString;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if(provider != null) {
				try {
					XRDesignPanel panel = provider.GetService(typeof(XRDesignPanel)) as XRDesignPanel;
					ReportTabControl tabControl = provider.GetService(typeof(ReportTabControl)) as ReportTabControl;
					MethodPicker methodPicker = new MethodPicker();
					methodPicker.SelectionMode = SelectionMode.One;
					string itemName = (value != null) ? (string)value : string.Empty;
					IWindowsFormsEditorService editServ = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
					methodPicker.Start(context, editServ, context.PropertyDescriptor.DisplayName);
					methodPicker.SetSelected(Math.Max(methodPicker.Items.IndexOf(value), 0), true);
					editServ.DropDownControl(methodPicker);
					if(itemName != methodPicker.ItemName && methodPicker.Items.Contains(methodPicker.ItemName)) {
						if(panel != null)
							panel.ExecCommand(ReportCommand.ShowScriptsTab);
						else if(tabControl != null)
							tabControl.SelectedIndex = TabIndices.Scripts;
						try {
							if(IsNewOrCurrent(methodPicker.ItemName))
								value = tabControl.SelectScript(context, context.PropertyDescriptor.DisplayName, methodPicker.ItemName == DesignSR.DataGridNewString);
							else
								value = methodPicker.ItemName;
						} catch {
							value = null;
						}
					}
					methodPicker.End();
				} catch { }
			}
			return value;
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			if(context != null && context.Instance != null) {
				return UITypeEditorEditStyle.DropDown;
			}
			return base.GetEditStyle(context);
		}
	}
	public class FormatStringEditor : UITypeEditor {
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object objValue) {
			if(provider != null) {
				try {
					IWindowsFormsEditorService edSvc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
					if(edSvc != null) {
						XRFormatStringEditorForm form = new XRFormatStringEditorForm((string)objValue, provider);
						if(edSvc.ShowDialog(form) == DialogResult.OK) {
							objValue = form.EditValue;
						}
					}
				} catch { }
			}
			return objValue;
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.Modal;
		}
	}
	public class DataBindingFormatStringEditor : FormatStringEditor {
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			if(context != null && DataBinding.IsThereBinding(context.Instance))
				return UITypeEditorEditStyle.Modal;
			return UITypeEditorEditStyle.None;
		}
	}
	public class XRWatermarkEditor : UITypeEditor {
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			XtraReport report = context.Instance as XtraReport;
			IWindowsFormsEditorService edSvc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
			if(report != null && edSvc != null) {
				WatermarkEditorForm form = new WatermarkEditorForm();
				LookAndFeelProviderHelper.SetParentLookAndFeel(form, provider);
				((IReport)report).ApplyPageSettings(form.PageSettings);
				Watermark watermark = (Watermark)value;
				form.Assign(watermark);
				if(edSvc.ShowDialog(form) == DialogResult.OK && !watermark.Equals(form.Watermark)) {
					IDesignerHost host = (IDesignerHost)provider.GetService(typeof(IDesignerHost));
					ReportDesigner designer = host.GetDesigner(report) as ReportDesigner;
					designer.CopyWatermark(form.Watermark, watermark);
				}
				form.Dispose();
			}
			return value;
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.Modal;
		}
	}
	public class XRControlStylesEditor : UITypeEditor {
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.Modal;
		}
		internal static XtraReport GetReport(ITypeDescriptorContext context) {
			XtraReport report = context.Instance as XtraReport;
			if(report != null)
				return report;
			XRControl control = context.Instance as XRControl;
			if(control != null)
				return control.RootReport;
			Array components = context.Instance as Array;
			foreach(object obj in components) {
				control = obj as XRControl;
				if(control != null)
					return control.RootReport;
			}
			return null;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			XtraReport report = GetReport(context);
			if(report != null) {
				try {
					IWindowsFormsEditorService edSvc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
					if(edSvc != null) {
						using(StyleSheetEditorForm form = new StyleSheetEditorForm(report, provider)) {
							edSvc.ShowDialog(form);
						}
					}
				} catch { }
			}
			return base.EditValue(context, provider, value);
		}
	}
	public class FormattingRuleSheetEditor : UITypeEditor {
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.Modal;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			XtraReport report = XRControlStylesEditor.GetReport(context);
			if(report != null) {
				try {
					IWindowsFormsEditorService edSvc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
					if(edSvc != null) {
						using(FormattingRuleSheetEditorForm form = new FormattingRuleSheetEditorForm(report, provider)) {
							edSvc.ShowDialog(form);
						}
					}
				} catch { }
			}
			return base.EditValue(context, provider, value);
		}
	}
	public class ParameterBindingCollectionEditor : DevExpress.Utils.UI.CollectionEditor {
		public ParameterBindingCollectionEditor()
			: base(typeof(ParameterBindingCollection)) {
		}
		protected override Utils.UI.CollectionEditorFormBase CreateCollectionForm(IServiceProvider serviceProvider) {
			return new ParameterBindingCollectionEditorForm(serviceProvider, this);
		}
	}
	public class XRSummaryUIEditor : UITypeEditor {
		public static void ModifySummary(XRLabel label, XRSummaryEditorForm form) {
			XRSummary summary = label.Summary;
			summary.Func = form.Func;
			summary.Running = form.Running;
			summary.FormatString = form.FormatString;
			summary.IgnoreNullValues = form.IgnoreNullValues;
			if(form.DesignBinding.DataMember != null && form.DesignBinding.DataMember.Length > 0) {
				label.DataBindings.Add(new XRBinding("Text", form.DesignBinding.DataSource, form.DesignBinding.DataMember));
			} else if(form.Binding != null) {
				label.DataBindings.Remove(form.Binding);
			}
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if(context == null || context.Instance == null || provider == null)
				return null;
			XRLabel label = context.Instance as XRLabel;
			if(label == null)
				return null;
			IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
			if(edSvc != null) {
				IDesignerHost designerHost = provider.GetService(typeof(IDesignerHost)) as IDesignerHost;
				XRSummaryEditorForm form = new XRSummaryEditorForm(label, designerHost);
				if(edSvc.ShowDialog(form) == DialogResult.OK) {
					DesignerTransaction transaction = designerHost.CreateTransaction(DesignSR.TransFmt_Summary);
					try {
						IComponentChangeService serv = provider.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
						PropertyDescriptor summaryProperty = DevExpress.XtraReports.Native.XRAccessor.GetPropertyDescriptor(label, "Summary");
						PropertyDescriptor bindingsProperty = DevExpress.XtraReports.Native.XRAccessor.GetPropertyDescriptor(label, "DataBindings");
						serv.OnComponentChanging(label, summaryProperty);
						serv.OnComponentChanging(label, bindingsProperty);
						ModifySummary(label, form);
						serv.OnComponentChanged(label, summaryProperty, null, null);
						serv.OnComponentChanged(label, bindingsProperty, null, null);
						transaction.Commit();
					} catch {
						transaction.Cancel();
					}
				}
			}
			return base.EditValue(context, provider, value);
		}
		public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return System.Drawing.Design.UITypeEditorEditStyle.Modal;
		}
	}
	public class XRGroupSortingSummaryUIEditor : UITypeEditor {
		public static void ModifySummary(XRGroupSortingSummary summary, GroupSortingSummaryEditorForm form) {
			summary.Function = form.Function;
			summary.IgnoreNullValues = form.IgnoreNullValues;
			summary.SortOrder = form.SortOrder;
			summary.FieldName = form.FieldName;
			summary.Enabled = form.SortingEnabled;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if(context == null || context.Instance == null || provider == null)
				return null;
			GroupHeaderBand band = context.Instance as GroupHeaderBand;
			if(band == null)
				return null;
			IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
			if(edSvc != null) {
				IDesignerHost designerHost = provider.GetService(typeof(IDesignerHost)) as IDesignerHost;
				GroupSortingSummaryEditorForm form = new GroupSortingSummaryEditorForm(band, designerHost);
				if(edSvc.ShowDialog(form) == DialogResult.OK) {
					DesignerTransaction transaction = designerHost.CreateTransaction(DesignSR.TransFmt_Summary);
					try {
						IComponentChangeService serv = provider.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
						PropertyDescriptor summaryProperty = DevExpress.XtraReports.Native.XRAccessor.GetPropertyDescriptor(band, "SortingSummary");
						serv.OnComponentChanging(band, summaryProperty);
						ModifySummary(band.SortingSummary, form);
						serv.OnComponentChanged(band, summaryProperty, null, null);
						transaction.Commit();
					} catch {
						transaction.Cancel();
					}
				}
			}
			return base.EditValue(context, provider, value);
		}
		public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return System.Drawing.Design.UITypeEditorEditStyle.Modal;
		}
	}
	public class XRFontEditor : UITypeEditor {
		FontDialog fontDialog;
		object value;
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			IWindowsFormsEditorService editorService;
			this.value = value;
			if(provider != null) {
				editorService = ((IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService)));
				if(editorService != null) {
					if(fontDialog == null) {
						fontDialog = new FontDialog();
						fontDialog.ShowApply = false;
						fontDialog.ShowColor = false;
						fontDialog.AllowVerticalFonts = false;
					}
					if((value is System.Drawing.Font))
						fontDialog.Font = ((System.Drawing.Font)value);
					if(DevExpress.XtraPrinting.Native.DialogRunner.ShowDialog(fontDialog) == DialogResult.OK)
						this.value = fontDialog.Font;
				}
			}
			value = this.value;
			this.value = null;
			return value;
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.Modal;
		}
	}
	class ValuePicker : System.Windows.Forms.ListBox {
		IWindowsFormsEditorService editServ;
		object editValue;
		public object EditValue { get { return editValue; } }
		public ValuePicker() {
			this.BorderStyle = System.Windows.Forms.BorderStyle.None;
		}
		public void Start(IWindowsFormsEditorService editServ, object editValue, object[] values) {
			this.editServ = editServ;
			this.editValue = editValue;
			Items.Clear();
			foreach(object value in values) {
				Items.Add(value);
				if(Object.Equals(value, editValue))
					SelectedItem = editValue;
			}
			editServ.DropDownControl(this);
		}
		protected override bool IsInputKey(Keys key) {
			if(key == Keys.Enter)
				CloseDropDown(SelectedItem);
			return base.IsInputKey(key);
		}
		protected override void OnDoubleClick(EventArgs e) {
			base.OnDoubleClick(e);
			CloseDropDown();
		}
		protected override void OnClick(EventArgs e) {
			base.OnClick(e);
			CloseDropDown();
		}
		private void CloseDropDown() {
			CloseDropDown(FindItem(PointToClient(MousePosition)));
		}
		private void CloseDropDown(object item) {
			if(item != null)
				editValue = item;
			editServ.CloseDropDown();
		}
		protected object FindItem(Point pt) {
			for(int i = 0; i < Items.Count; i++) {
				if(GetItemRectangle(i).Contains(pt))
					return Items[i];
			}
			return null;
		}
	}
	public class PaperNameEditor : UITypeEditor {
		#region inner classes
		protected class PaperSizeItem {
			System.Drawing.Printing.PaperSize paperSize;
			public int PaperWidth { get { return paperSize.Width; } }
			public int PaperHeight { get { return paperSize.Height; } }
			public PaperSizeItem(System.Drawing.Printing.PaperSize paperSize) {
				this.paperSize = paperSize;
			}
			public override string ToString() {
				return paperSize.PaperName;
			}
		}
		class PaperNamePicker : ValuePicker {
			ToolTip toolTip;
			string paperName;
			public string PaperName {
				get {
					return EditValue != null ? EditValue.ToString() : paperName;
				}
			}
			public PaperNamePicker() {
				toolTip = new ToolTip();
			}
			protected override void Dispose(bool disposing) {
				if(disposing) {
					if(toolTip != null) {
						toolTip.Dispose();
						toolTip = null;
					}
				}
				base.Dispose(disposing);
			}
			public void Start(IWindowsFormsEditorService editServ, string printerName, string paperName) {
				toolTip.RemoveAll();
				this.paperName = paperName;
				List<PaperSizeItem> items = new List<PaperSizeItem>();
				System.Drawing.Printing.PrinterSettings sets = new System.Drawing.Printing.PrinterSettings();
				sets.PrinterName = printerName;
				PaperSizeItem item = null;
				foreach(System.Drawing.Printing.PaperSize paperSize in sets.PaperSizes) {
					if(paperSize.Kind == System.Drawing.Printing.PaperKind.Custom) {
						items.Add(new PaperSizeItem(paperSize));
						if(Object.Equals(paperSize.PaperName, paperName))
							item = (PaperSizeItem)items[items.Count - 1];
					}
				}
				Start(editServ, item, items.ToArray());
			}
			public void Finish() {
				toolTip.RemoveAll();
			}
			protected override void OnMouseMove(MouseEventArgs e) {
				base.OnMouseMove(e);
				PaperSizeItem item = FindItem(PointToClient(MousePosition)) as PaperSizeItem;
				if(item != null)
					toolTip.SetToolTip(this, String.Format("Width={0:f2}in Height={1:f2}in", item.PaperWidth / 100f, item.PaperHeight / 100f));
				else
					toolTip.RemoveAll();
			}
		}
		#endregion
		PaperNamePicker paperNamePicker;
		public override bool IsDropDownResizable {
			get { return true; }
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if(provider != null) {
				try {
					IWindowsFormsEditorService editServ = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
					if(paperNamePicker == null || paperNamePicker.IsDisposed)
						paperNamePicker = new PaperNamePicker();
					XtraReport report = (XtraReport)context.Instance;
					paperNamePicker.Start(editServ, report.PrinterName, (string)value);
					value = paperNamePicker.PaperName;
				} catch { }
				if(paperNamePicker != null)
					paperNamePicker.Finish();
			}
			return value;
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			if(context != null && context.Instance != null) {
				if(context.PropertyDescriptor != null && context.PropertyDescriptor.Converter != null && context.PropertyDescriptor.Converter.GetStandardValuesSupported(context))
					return UITypeEditorEditStyle.DropDown;
			}
			return base.GetEditStyle(context);
		}
	}
	public class XrControlStyleColorEditor : UITypeEditor {
		UITypeEditor colorEditor;
		public XrControlStyleColorEditor() {
			colorEditor = new System.Drawing.Design.ColorEditor();
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			return colorEditor.EditValue(context, provider, value);
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return colorEditor.GetEditStyle(context);
		}
		public override bool GetPaintValueSupported(ITypeDescriptorContext context) {
			return colorEditor.GetPaintValueSupported(context);
		}
		public override void PaintValue(PaintValueEventArgs e) {
			if(XRControlStyleConverterHelper.NotSet(e.Context, XRControlStyleConverterHelper.ToStyleProperty(e.Context.PropertyDescriptor)))
				colorEditor.PaintValue(new PaintValueEventArgs(e.Context, Color.Empty, e.Graphics, e.Bounds));
			else
				colorEditor.PaintValue(e);
		}
	}
	public class CalculatedFieldExpressionEditor : DevExpress.XtraEditors.Design.ExpressionEditorBase {
		ITypeDescriptorContext context;
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			IDataContextService serv = provider.GetService<IDataContextService>();
			if(serv != null)
				serv.PrefilterProperties += new EventHandler<DataContextFilterPropertiesEventArgs>(serv_PrefilterProperties);
			this.context = context;
			try {
				return base.EditValue(context, provider, value);
			} finally {
				if(serv != null)
					serv.PrefilterProperties -= new EventHandler<DataContextFilterPropertiesEventArgs>(serv_PrefilterProperties);
				this.context = null;
			}
		}
		void serv_PrefilterProperties(object sender, DataContextFilterPropertiesEventArgs e) {
			if(context == null) return;
			CalculatedField calcField = context.Instance as CalculatedField;
			if(calcField == null) return;
			for(int i = e.Properties.Count - 1; i >= 0; i--) {
				if(e.Properties[i] is CalculatedPropertyDescriptorBase && e.Properties[i].Name == calcField.Name) {
					e.Properties.RemoveAt(i);
					return;
				}
			}
		}
		protected override ExpressionEditorForm CreateForm(object instance, IDesignerHost designerHost, object value) {
			return new CalculatedFieldExpressionEditorForm(instance, designerHost);
		}
	}
	public class FormattingRuleExpressionEditor : DevExpress.XtraEditors.Design.ExpressionEditorBase {
		protected override ExpressionEditorForm CreateForm(object instance, IDesignerHost designerHost, object value) {
			return new FormattingRuleConditionEditorForm(instance, designerHost);
		}
	}
	public class BarCodeDataEditor : ArrayEditor {
		public BarCodeDataEditor(Type type) : base(type) { }
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			XRBarCode barCode = context.Instance as XRBarCode;
			if(barCode != null && barCode.Symbology is BarCode2DGenerator)
				return base.GetEditStyle(context);
			return UITypeEditorEditStyle.None;
		}
	}
	public class PdfPasswordSecurityOptionsEditor : UITypeEditor {
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			PdfPasswordSecurityOptions options = value as PdfPasswordSecurityOptions;
			IWindowsFormsEditorService edSvc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
			if(edSvc != null && options != null) {
				PdfPasswordSecurityEditorForm form = new PdfPasswordSecurityEditorForm();
				form.Init(options);
				DesignLookAndFeelHelper.SetParentLookAndFeel(form, provider);
				edSvc.ShowDialog(form);
				form.Dispose();
			}
			return value;
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.Modal;
		}
	}
	public class BorderDashStyleEditor : UITypeEditor {
		#region inner classes
		class PopupListBox : DevExpress.XtraEditors.ListBoxControl {
			public int CalculatedItemHeight {
				get { return ViewInfo.ItemHeight; }
			}
		}
		#endregion
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			IWindowsFormsEditorService editServ = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
			using(PopupListBox listBox = CreateDropDownList(context, editServ)) {
				editServ.DropDownControl(listBox);
				value = ((Pair<object, string>)listBox.SelectedItem).First;
			}
			return value;
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.DropDown;
		}
		PopupListBox CreateDropDownList(ITypeDescriptorContext context, IWindowsFormsEditorService editServ) {
			PopupListBox listBox = new PopupListBox();
			TypeConverter converter = GetConverter(context);
			ArrayList data = new ArrayList();
			foreach(object value in converter.GetStandardValues(context)) {
				data.Add(new Pair<object, string>(value, (string)converter.ConvertTo(new StubTypeDescriptorContext(), System.Globalization.CultureInfo.CurrentCulture, value, typeof(string))));
			}
			listBox.DataSource = data;
			int displayCount = Math.Min(listBox.ItemCount, 10);
			listBox.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			listBox.Size = new Size(listBox.ClientSize.Width, displayCount * listBox.CalculatedItemHeight);
			listBox.ValueMember = "First";
			listBox.DisplayMember = "Second";
			listBox.Click += new EventHandler(delegate(object sender, EventArgs e) { editServ.CloseDropDown(); });
			return listBox;
		}
		public static TypeConverter GetConverter(ITypeDescriptorContext context) {
			TypeConverter converter = context.PropertyDescriptor.Converter;
			if(converter == null) {
				object value = GetValue(context.PropertyDescriptor, context.Instance);
				converter = (value == null) ? TypeDescriptor.GetConverter(context.PropertyDescriptor.PropertyType) : TypeDescriptor.GetConverter(value);
			}
			return converter;
		}
		public static object GetValue(PropertyDescriptor pd, object component) {
			object value = null;
			if(component == null || pd == null) return value;
			try {
				value = pd.GetValue(component);
			} catch(Exception e) {
				value = (e.InnerException != null) ? e.InnerException.Message : value = e.Message;
			}
			return value;
		}
	}
	public class LookUpValuesEditor : UITypeEditor {
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if(context != null && context.Instance != null) {
				StaticListLookUpSettings settings = (StaticListLookUpSettings)context.Instance;
				IDesignerHost designerHost = (IDesignerHost)provider.GetService(typeof(IDesignerHost));
				if(designerHost != null)
					ExecuteInTransaction(designerHost, () => EditValue(provider, settings));
				else
					EditValue(provider, settings);
				return settings.LookUpValues;
			}
			return base.EditValue(context, provider, value);
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			if(context != null && context.Instance != null) {
				return UITypeEditorEditStyle.Modal;
			}
			return base.GetEditStyle(context);
		}
		bool EditValue(IServiceProvider provider, StaticListLookUpSettings lookUpSettings) {
			try {
				IComponentChangeService componentChangeService = (IComponentChangeService)provider.GetService(typeof(IComponentChangeService));
				if(componentChangeService != null)
					componentChangeService.OnComponentChanging(lookUpSettings.Parameter, null);
				LookUpValueCollection lookUpValues = lookUpSettings.LookUpValues;
				LookUpValuesEditorForm form = new LookUpValuesEditorForm(lookUpValues, lookUpSettings.Parameter.Type, provider);
				if(form.ShowDialog() == DialogResult.OK) {
					if(componentChangeService != null)
						componentChangeService.OnComponentChanged(lookUpSettings.Parameter, null, null, null);
					return true;
				}
			} catch { }
			return false;
		}
		void ExecuteInTransaction(IDesignerHost designerHost, Func<bool> editAction) {
			var transaction = designerHost.CreateTransaction("Edit LookUpValues");
			if(editAction())
				transaction.Commit();
			else
				transaction.Cancel();
		}
	}
}
