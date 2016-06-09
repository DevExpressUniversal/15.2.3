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
using System.Web.UI;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.Web;
namespace DevExpress.ExpressApp.Web.Editors.ASPx {
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public abstract class DataItemTemplateBase : ITemplate, IDisposable, IBehaviourTemplate {
		public const string ViewEditModeString = "VEM";
		private WebPropertyEditor propertyEditor;
		private IDataItemTemplateInfoProvider dataItemTemplateInfoProvider;
		public DataItemTemplateBase(WebPropertyEditor propertyEditor, IDataItemTemplateInfoProvider dataItemTemplateInfoProvider) {
			this.propertyEditor = propertyEditor;
			this.dataItemTemplateInfoProvider = dataItemTemplateInfoProvider;
		}
		public WebPropertyEditor PropertyEditor {
			get { return propertyEditor; }
		}
		public void InstantiateIn(Control container) {
			InstantiateInCore(container);
		}
		public bool CancelClickEventPropagation {
			get {
				if(propertyEditor != null) {
					return propertyEditor.CancelClickEventPropagation;
				}
				return false;
			}
		}
		public virtual void Dispose() {
			if(propertyEditor != null) {
				propertyEditor.BreakLinksToControl(false);
			}
			propertyEditor = null;
			dataItemTemplateInfoProvider = null;
			CustomCreateCellControl = null;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected IDataItemTemplateInfoProvider DataItemTemplateInfoProvider {
			get { return dataItemTemplateInfoProvider; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected abstract Control CreateControl(WebColumnBase column, object obj);
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected virtual Control InstantiateInCore(Control container) {
			Control editorControl = null;
			if(dataItemTemplateInfoProvider != null) {
				WebColumnBase column = dataItemTemplateInfoProvider.GetColumn(container);
				object obj = dataItemTemplateInfoProvider.GetObject(container);
				editorControl = CreateControl(column, obj);
				IDataColumnInfo columnInfo = dataItemTemplateInfoProvider.GetColumnInfo(column) as IDataColumnInfo;
				if(columnInfo != null) {
					editorControl.ID = WebIdHelper.GetCorrectedId(columnInfo.Model.PropertyName);
					string controlId = propertyEditor.SetControlId(WebIdHelper.GetCorrectedId(columnInfo.Model.PropertyName));
					if(editorControl != propertyEditor.Control) {
						editorControl.ID = controlId;
					}
				}
				container.Controls.Add(editorControl);
			}
			return editorControl;
		}
		protected virtual void OnCustomCreateCellControl(CustomCreateCellControlEventArgs args) {
			if(CustomCreateCellControl != null) {
				CustomCreateCellControl(this, args);
			}
		}
		public event EventHandler<CustomCreateCellControlEventArgs> CustomCreateCellControl;
	}
}
