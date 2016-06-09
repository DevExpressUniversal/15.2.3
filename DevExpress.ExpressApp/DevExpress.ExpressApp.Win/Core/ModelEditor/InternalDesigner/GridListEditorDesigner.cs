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
using System.Windows.Forms;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraGrid.Views.Grid;
namespace DevExpress.ExpressApp.Win.Core.ModelEditor.InternalDesigner {
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	internal class GridListEditorDesigner : IDisposable, ITestable {
		private IModelListView listViewModel;
		private IModelNode currentNode;
		private GridListEditor editor;
		private DesignerDataSourceProvider dataSourceProvider;
		private EditorModificationObserver viewObserver;
		private GridListEditorColumnChooserExtender columnChooserExtender;
		private bool lockEvents = false;
		public GridListEditorDesigner(IModelListView listViewModel, IModelNode currentNode, System.IServiceProvider serviceProvider) {
			this.listViewModel = listViewModel;
			this.currentNode = currentNode;
			dataSourceProvider = new DesignerDataSourceProvider(serviceProvider);
			Initialize();
		}
		public Control DesignerControl {
			get {
				if(editor != null) {
					return editor.Grid;
				}
				return null;
			}
		}
		public void SaveModel() {
			if(editor != null) {
				editor.SaveModel();
			}
		}
		public static bool CanShowDesigner(IModelNode modelNode) {
			return modelNode != null && modelNode is IModelColumns && modelNode.Parent != null && modelNode.Parent is IModelListView && !((IModelListView)modelNode.Parent).BandsLayout.Enable;
		}
		public void Dispose() {
			DetachObserver();
			if(editor != null) {
				editor.Dispose();
				editor = null;
			}
			if(columnChooserExtender != null) {
				columnChooserExtender.BeginUpdate -= columnChooser_BeginUpdate;
				columnChooserExtender.EndUpdate -= columnChooser_EndUpdate;
				columnChooserExtender.Dispose();
				columnChooserExtender = null;
			}
		}
		private void Initialize() {
			try {
				editor = CreateEditor();
				SetupEditor(editor);
				AttachObserver(editor);
			}
			catch(Exception e) {
				ModelEditorControllerBase.ShowError(e);
			}
		}
		private GridListEditor CreateEditor() {
			GridListEditor result = null;
			if(currentNode is IModelColumns) {
				result = new GridListEditor(listViewModel);
			}
			return result;
		}
		private void SetupEditor(GridListEditor editor) {
			if(editor != null) {
				editor.CreateControls();
				columnChooserExtender = new GridListEditorColumnChooserExtender(listViewModel, editor, ((IGridViewOptions)editor).ObjectTypeInfo, editor.GridView);
				columnChooserExtender.BeginUpdate += columnChooser_BeginUpdate;
				columnChooserExtender.EndUpdate += columnChooser_EndUpdate;
				if(editor.ColumnView is GridView) {
					((GridView)editor.ColumnView).OptionsFind.AllowFindPanel = false;
					((GridView)editor.ColumnView).OptionsView.ColumnAutoWidth = false;
				}
				editor.DataSource = dataSourceProvider.CreateDataSource(listViewModel, editor.ColumnView);
				if(editor is ISupportNewItemRowPosition) {
					((ISupportNewItemRowPosition)editor).NewItemRowPosition = CalculateNewItemRowPosition();
				}
			}
		}
		private void EndUpdate() {
			lockEvents = false;
			OnDesignerChanged();
		}
		private void BeginUpdate() {
			lockEvents = true;
		}
		private void columnChooser_EndUpdate(object sender, EventArgs e) {
			EndUpdate();
		}
		private void columnChooser_BeginUpdate(object sender, EventArgs e) {
			BeginUpdate();
		}
		private NewItemRowPosition CalculateNewItemRowPosition() {
			NewItemRowPosition result = NewItemRowPosition.None;
			if(listViewModel.AllowEdit && listViewModel.AllowNew && listViewModel is IModelListViewNewItemRow) {
				result = ((IModelListViewNewItemRow)listViewModel).NewItemRowPosition;
			}
			return result;
		}
		private void AttachObserver(WinColumnsListEditor editor) {
			viewObserver = new EditorModificationObserver(editor);
			viewObserver.Changed += viewObserver_Changed;
			viewObserver.Attach();
		}
		private void DetachObserver() {
			if(viewObserver != null) {
				viewObserver.Changed -= viewObserver_Changed;
				viewObserver.Detach();
				viewObserver = null;
			}
		}
		private void viewObserver_Changed(object sender, EventArgs e) {
			OnDesignerChanged();
		}
		private void OnDesignerChanged() {
			if(!lockEvents && DesignerChanged != null) {
				DesignerChanged(this, EventArgs.Empty);
			}
		}
		public event EventHandler DesignerChanged;
		#region ITestable Members
		WinColumnsListEditor ITestable.Editor {
			get {
				return editor;
			}
		}
		NewItemRowPosition ITestable.CalculateNewItemRowPosition() {
			return CalculateNewItemRowPosition();
		}
		#endregion
	}
	internal interface ITestable {
		WinColumnsListEditor Editor { get; }
		NewItemRowPosition CalculateNewItemRowPosition();
	}
	public class TypesInfoServiceProvider : System.IServiceProvider {
		object System.IServiceProvider.GetService(Type serviceType) {
			if(serviceType == typeof(ITypesInfo)) {
				return XafTypesInfo.Instance;
			}
			return null;
		}
	}
}
