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
using DevExpress.XtraCharts;
using DevExpress.Xpf.Diagram;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Reports.UserDesigner.Native.Metadata;
using DevExpress.Xpf.Reports.UserDesigner.ReportModel;
using System.Windows;
using DevExpress.Diagram.Core;
using DevExpress.Xpf.Reports.UserDesigner.XRDiagram;
using System.Reflection;
namespace DevExpress.Xpf.Reports.UserDesigner.Editors {
	public class ChartTitleCollectionUITypeEditor : SingleSelectionCollectionEditor {
		static ChartTitleCollectionUITypeEditor() {
			DependencyPropertyRegistrator<ChartTitleCollectionUITypeEditor>.New()
				.OverrideDefaultStyleKey()
			;
		}
		public override object CreateItem() {
			var provider = (PropertiesProvider<IDiagramItem, IList<DiagramItem>>)EditValue.GetType().GetField("provider", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(EditValue);
			var chartDiagramItem = (XRChartDiagramItem)provider.MainComponent;
			return (XRChartTitleDiagramItem)chartDiagramItem.ItemFactory(new ChartTitle());
		}
		public override Func<IMultiModel, bool> IsEditorItem { get { return item => SelectionModelHelper<IDiagramItem, DiagramItem>.GetUnderlyingItem(item) is XRChartTitleDiagramItem; } }
	}
}
