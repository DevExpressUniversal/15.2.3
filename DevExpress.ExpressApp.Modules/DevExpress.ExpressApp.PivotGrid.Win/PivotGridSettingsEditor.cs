#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Text;
using DevExpress.LookAndFeel;
using DevExpress.XtraPivotGrid;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model;
using System.Collections.Generic;
using DevExpress.ExpressApp.Utils;
using System.Collections;
namespace DevExpress.ExpressApp.PivotGrid.Win {
	public class PivotGridSettingsEditor : UITypeEditor {
		public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value) {
			using(XafPivotGridDesignerForm editor = new XafPivotGridDesignerForm(UserLookAndFeel.Default)) {
				XafPivotGridControl pivotGridControl = new XafPivotGridControl();
				pivotGridControl.OptionsLayout.StoreAllOptions = true;
				pivotGridControl.OptionsLayout.StoreAppearance = true;
				pivotGridControl.OptionsLayout.Columns.RemoveOldColumns = false;
				pivotGridControl.OptionsLayout.Columns.StoreAllOptions = true;
				pivotGridControl.OptionsLayout.Columns.StoreAppearance = true;
				if(context != null && context.Instance != null) {
					IModelListView modelListView = ((ModelNode)context.Instance).Parent as IModelListView;
					if(modelListView != null && modelListView.ModelClass != null) {
						Type targetType = modelListView.ModelClass.TypeInfo.Type;
						PivotGridListEditor pivotGridListEditor = new PivotGridListEditor(modelListView);
						pivotGridListEditor.CreateControls();
						pivotGridControl = (XafPivotGridControl)pivotGridListEditor.PivotGridControl;
						pivotGridControl.DataSource = DevExpress.ExpressApp.Editors.CriteriaPropertyEditorHelper.CreateFilterControlDataSource(targetType, null);
						((XafPivotGridViewInfoData)(((IPivotGridViewInfoDataOwner)pivotGridControl).DataViewInfo)).DataObjectType = targetType;
					}
				}
				if(!string.IsNullOrEmpty((string)value)) {
					MemoryStream loadStream = new MemoryStream(Encoding.UTF8.GetBytes((string)value));
					pivotGridControl.RestoreLayoutFromStream(loadStream);
				}
				editor.InitEditingObject(pivotGridControl);
				editor.ShowDialog();
				MemoryStream saveStream = new MemoryStream();
				pivotGridControl.SaveLayoutToStream(saveStream);
				return Encoding.UTF8.GetString(saveStream.ToArray());
			}
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.Modal;
		}
	}
}
