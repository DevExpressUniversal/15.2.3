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

using DevExpress.Xpf.Core;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;
using DevExpress.XtraPivotGrid.Localization;
#if SL
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using DevExpress.Utils;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.PivotGrid {
	public class DefaultFieldList : ColumnChooserBase {
		public DefaultFieldList(PivotGridControl pivot)
			: base(pivot) {
				Caption = PivotGridLocalizer.GetString(PivotGridStringId.CustomizationFormCaption);
		}
		protected PivotGridControl PivotGrid { get { return (PivotGridControl)Owner; } }
		public Control ContentControl {
			get { return (Control)Container.Content; }
		}
		protected string TemplatePropertyName {
			get {
				if(PivotGrid.FieldListStyle == FieldListStyle.Simple)
					return PivotGridControl.FieldListTemplatePropertyName;
				else
					return PivotGridControl.ExcelFieldListTemplatePropertyName;
			}
		}
		protected override Control CreateContentControl() {
			Control contentControl = base.CreateContentControl();
			PivotGridControl.SetPivotGrid(contentControl, PivotGrid);
			return contentControl;
		}
		protected override void OnContainerHidden(object sender, RoutedEventArgs e) {
			PivotGrid.IsFieldListVisible = false;
#if SL
			PivotGrid.Focus();
#endif
		}
		public override void Show() {
			ContentControl.SetBinding(Control.TemplateProperty,
				new Binding(TemplatePropertyName) { Source = PivotGrid });
			base.Show();
		}
	}
	public sealed class DefaultFieldListFactory : IColumnChooserFactory {
		static readonly DefaultFieldListFactory instance = new DefaultFieldListFactory();
		DefaultFieldListFactory() { }
		#region IColumnChooserFactory Members
		IColumnChooser IColumnChooserFactory.Create(Control owner) {
			return new DefaultFieldList((PivotGridControl)owner);
		}
		#endregion
		public static DefaultFieldListFactory Instance { get { return instance; } }
	}
}
