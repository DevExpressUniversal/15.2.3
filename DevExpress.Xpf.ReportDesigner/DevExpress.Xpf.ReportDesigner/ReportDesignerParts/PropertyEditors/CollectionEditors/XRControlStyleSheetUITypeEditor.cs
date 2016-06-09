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
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Xpf.Printing;
using DevExpress.Xpf.Reports.UserDesigner.Editors.Native;
using DevExpress.XtraReports.Serialization;
using DevExpress.XtraReports.UI;
using DevExpress.Xpf.Diagram;
using DevExpress.Xpf.Reports.UserDesigner.XRDiagram;
using DevExpress.Diagram.Core;
using DevExpress.Xpf.Reports.UserDesigner.ReportModel;
using System.Reflection;
namespace DevExpress.Xpf.Reports.UserDesigner.Editors {
	public class XRControlStyleSheetUITypeEditor : SingleSelectionCollectionEditor {
		const string StyleFileExtensionFilter = "Report StyleSheet files (*.repss)|*.repss|All files (*.*)|*.*";
		public static readonly DependencyProperty ReportModelProperty;
		static readonly Action<XRControlStyleSheetUITypeEditor, Action<IOpenFileDialogService>> OpenFileDialogServiceAccessor;
		public static readonly DependencyProperty OpenFileDialogServiceTemplateProperty;
		static readonly Action<XRControlStyleSheetUITypeEditor, Action<ISaveFileDialogService>> SaveFileDialogServiceAccessor;
		public static readonly DependencyProperty SaveFileDialogServiceTemplateProperty;
		static readonly Action<XRControlStyleSheetUITypeEditor, Action<IMessageBoxService>> MessageBoxServiceAccessor;
		public static readonly DependencyProperty MessageBoxServiceTemplateProperty;
		static XRControlStyleSheetUITypeEditor() {
			DependencyPropertyRegistrator<XRControlStyleSheetUITypeEditor>.New()
				.Register(d => d.ReportModel, out ReportModelProperty, null)
				.RegisterServiceTemplateProperty(d => d.OpenFileDialogServiceTemplate, out OpenFileDialogServiceTemplateProperty, out OpenFileDialogServiceAccessor)
				.RegisterServiceTemplateProperty(d => d.SaveFileDialogServiceTemplate, out SaveFileDialogServiceTemplateProperty, out SaveFileDialogServiceAccessor)
				.RegisterServiceTemplateProperty(d => d.MessageBoxServiceTemplate, out MessageBoxServiceTemplateProperty, out MessageBoxServiceAccessor)
				.OverrideDefaultStyleKey()
			;
		}
		public XRControlStyleSheetUITypeEditor() {
			ClearCommand = DelegateCommandFactory.Create(RemoveAllItems);
			ClearUnusedCommand = DelegateCommandFactory.Create(ClearUnused);
			OpenStylesCommand = DelegateCommandFactory.Create(OpenStyles);
			SaveStylesCommand = DelegateCommandFactory.Create(SaveStyles);
		}
		public ICommand ClearCommand { get; private set; }
		public ICommand ClearUnusedCommand { get; private set; }
		public ICommand OpenStylesCommand { get; private set; }
		public ICommand SaveStylesCommand { get; private set; }
		public XtraReportModelBase ReportModel {
			get { return (XtraReportModelBase)GetValue(ReportModelProperty); }
			set { SetValue(ReportModelProperty, value); }
		}
		protected void DoWithOpenFileDialogService(Action<IOpenFileDialogService> action) { OpenFileDialogServiceAccessor(this, action); }
		public DataTemplate OpenFileDialogServiceTemplate {
			get { return (DataTemplate)GetValue(OpenFileDialogServiceTemplateProperty); }
			set { SetValue(OpenFileDialogServiceTemplateProperty, value); }
		}
		protected void DoWithSaveFileDialogService(Action<ISaveFileDialogService> action) { SaveFileDialogServiceAccessor(this, action); }
		public DataTemplate SaveFileDialogServiceTemplate {
			get { return (DataTemplate)GetValue(SaveFileDialogServiceTemplateProperty); }
			set { SetValue(SaveFileDialogServiceTemplateProperty, value); }
		}
		protected void DoWithMessageBoxService(Action<IMessageBoxService> action) { MessageBoxServiceAccessor(this, action); }
		public DataTemplate MessageBoxServiceTemplate {
			get { return (DataTemplate)GetValue(MessageBoxServiceTemplateProperty); }
			set { SetValue(MessageBoxServiceTemplateProperty, value); }
		}
		public override object CreateItem() {
			return CreateStyleDiagramItem(new XRControlStyle());
		}
		public override Func<IMultiModel, bool> IsEditorItem { get { return item => SelectionModelHelper<IDiagramItem, DiagramItem>.GetUnderlyingItem(item) is XRControlStyleDiagramItem; } }
		void ClearUnused() {
			DoWithEditorItems(x => {
				if(!ReportModel.XRObject.IsAttachedStyle(GetXRControl((IMultiModel)Items[x])))
					controller.RemoveAt(x);
			});
		}
		void OpenStyles() {
			DoWithOpenFileDialogService(dialog => {
				dialog.Filter = StyleFileExtensionFilter;
				var newStyles = new XRControlStyleSheet(ReportModel.XRObject);
				if(dialog.ShowDialog(e => {
					try {
						newStyles.LoadFromFile(dialog.GetFullFileName());
					} catch(IOException ex) {
						ShowErrorMessage(e, ex);
					} catch(ArgumentException ex) {
						ShowErrorMessage(e, ex);
					}})) {
					var originalXRControlStyleSheet = new XRControlStyleSheet(ReportModel.XRObject);
					DoWithEditorItems(x => originalXRControlStyleSheet.Add((XRControlStyle)GetXRControl((IMultiModel)Items[x]).Clone()));
					foreach(XRControlStyle newStyle in newStyles) {
						if(!originalXRControlStyleSheet.Contains(newStyle))
							controller.Add(CreateStyleDiagramItem(newStyle));
					}
				}
			});
		}
		void SaveStyles() {
			DoWithSaveFileDialogService(dialog => {
				dialog.Filter = StyleFileExtensionFilter;
				var styleCollection = new XRControlStyleSheet(ReportModel.XRObject);
				DoWithEditorItems(x => styleCollection.Add((XRControlStyle)GetXRControl((IMultiModel)Items[x]).Clone()));
				if(dialog.ShowDialog())
					styleCollection.SaveToFile(dialog.GetFullFileName());
			});
		}
		void ShowErrorMessage(CancelEventArgs e, Exception ex) {
			e.Cancel = true;
			DoWithMessageBoxService(x => x.ShowMessage(ex.Message, "Error", MessageButton.OK, MessageIcon.Error));
		}
		XRControlStyleDiagramItem CreateStyleDiagramItem(XRControlStyle style) {
			return (XRControlStyleDiagramItem)ReportModel.DiagramItem.Diagram.ItemFactory(style);
		}
		XRControlStyle GetXRControl(IMultiModel item) {
			return (XRControlStyle)XRModelBase.GetXRModel(SelectionModelHelper<IDiagramItem, DiagramItem>.GetUnderlyingItem(item)).XRObject;
		}
	}
}
