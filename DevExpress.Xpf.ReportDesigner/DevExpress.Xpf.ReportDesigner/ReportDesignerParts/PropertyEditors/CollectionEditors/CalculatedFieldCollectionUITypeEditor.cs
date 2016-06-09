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
using DevExpress.Mvvm.UI.Native;
using DevExpress.Xpf.Reports.UserDesigner.Editors.Native;
using DevExpress.XtraReports.UI;
using DevExpress.Mvvm;
using DevExpress.Xpf.Diagram;
using System.Collections;
using System.Linq;
using DevExpress.Diagram.Core;
using System.Linq.Expressions;
using System.ComponentModel;
using System.Windows;
using DevExpress.Xpf.Reports.UserDesigner.ReportModel;
using DevExpress.Xpf.Reports.UserDesigner.XRDiagram;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Reports.UserDesigner.Editors {
	public class CalculatedFieldCollectionUITypeEditor : SingleSelectionCollectionEditor {
		public static readonly DependencyProperty ReportModelProperty;
		public XtraReportModelBase ReportModel {
			get { return (XtraReportModelBase)GetValue(ReportModelProperty); }
			set { SetValue(ReportModelProperty, value); }
		}
		static CalculatedFieldCollectionUITypeEditor() {
			DependencyPropertyRegistrator<CalculatedFieldCollectionUITypeEditor>.New()
				.Register(owner => owner.ReportModel, out ReportModelProperty, null)
				.OverrideDefaultStyleKey()
			;
		}
		public CalculatedFieldCollectionUITypeEditor() {
			this.addNewScriptCommand = DelegateCommandFactory.Create<string>(AddNewScript);
		}
		readonly ICommand<string> addNewScriptCommand;
		public ICommand<string> AddNewScriptCommand { get { return addNewScriptCommand; } }
		public override object CreateItem() {
			var designerHost = ReportModel.DesignerHost as UserDesigner.Native.ReportExtensions.DesignSite.DesignerHost;
			var field = new CalculatedField();
			var name = designerHost.CoerceName(field, null, true);
			designerHost.RemoveName(name);
			return (CalculatedFieldDiagramItem)ReportModel.DiagramItem.Diagram.ItemFactory(new CalculatedField() { Name = name });
		}
		public override Func<IMultiModel, bool> IsEditorItem { get { return item => SelectionModelHelper<IDiagramItem, DiagramItem>.GetUnderlyingItem(item) is CalculatedFieldDiagramItem; } }
		void AddNewScript(string eventName) {
			var calculatedField = (CalculatedField)XRModelBase.GetXRModel(SelectionModelHelper<IDiagramItem, DiagramItem>.GetUnderlyingItem((IMultiModel)SelectedItem)).XRObject;
			var scripts = calculatedField.Scripts;
			if(scripts == null)
				return;
			string procName = scripts.GetProcName(eventName);
			if(string.IsNullOrEmpty(procName))
				procName = scripts.GetDefaultPropertyValue(eventName);
			string baseName = scripts.GetDefaultPropertyValue(eventName);
			procName = baseName;
			int index = 1;
			while(ReportModel.Scripts.Contains(procName))
				procName = string.Format("{0}_{1}", baseName, index++);
			scripts.SetPropertyValue(eventName, procName);
			ReportModel.Scripts.Add(procName);
			string script = scripts.GenerateDefaultEventScript(eventName, procName);
			ReportModel.ScriptsSource += "\r\n" + script;
		}
	}
}
