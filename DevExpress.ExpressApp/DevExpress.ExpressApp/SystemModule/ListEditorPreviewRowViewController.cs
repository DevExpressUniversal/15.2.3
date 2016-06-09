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
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.SystemModule {
	public class ListEditorPreviewRowViewController : ViewController, IModelExtender {
		public const string PreviewColumnNameAttributeName = "PreviewColumnName";
		private void View_ControlsCreated(object sender, EventArgs e) {
			if(View.Model is IModelListViewPreviewColumn) {
				IModelColumn previewColumnModel = ((IModelListViewPreviewColumn)View.Model).PreviewColumnName;
				if(previewColumnModel != null && DataManipulationRight.CanRead(((ListView)View).ObjectTypeInfo.Type, previewColumnModel.PropertyName, null, null, View.ObjectSpace)) {
					SetPreviewColumn(previewColumnModel);
				}
			}
		}
		protected virtual void SetPreviewColumn(IModelColumn previewColumnModel) {
		}
		protected override void OnActivated() {
			base.OnActivated();
			this.View.ControlsCreated += new EventHandler(View_ControlsCreated); 
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			this.View.ControlsCreated -= new EventHandler(View_ControlsCreated); 
		}
		public ListEditorPreviewRowViewController() {
			TargetViewType = ViewType.ListView; 
		}
		#region IExtendModel Members
		void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
			extenders.Add<IModelListView, IModelListViewPreviewColumn>();
		}
		#endregion
	}
	public interface IModelListViewPreviewColumn {
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelListViewPreviewColumnPreviewColumnName"),
#endif
 Localizable(false)]
		[DataSourceProperty("Columns")]
		[Category("Data")]
		IModelColumn PreviewColumnName { get; set; }
	}
}
