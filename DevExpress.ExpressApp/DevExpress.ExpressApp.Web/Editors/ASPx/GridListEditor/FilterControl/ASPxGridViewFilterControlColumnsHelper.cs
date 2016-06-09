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

using DevExpress.ExpressApp.Utils;
using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
namespace DevExpress.ExpressApp.Web.Editors.ASPx.GridListEditor.FilterControl {
	internal class CustomizeFilterControlColumnEventArgs : EventArgs {
		public CustomizeFilterControlColumnEventArgs(string propertyName, string fullPropertyName, string displayName) {
			this.PropertyName = propertyName;
			this.FullPropertyName = fullPropertyName;
			this.DisplayName = displayName;
			this.Visible = true;
		}
		public string PropertyName { get; private set; }
		public string FullPropertyName { get; private set; }
		public string DisplayName { get; set; }
		[DefaultValue(true)]
		public bool Visible { get; set; }
	}
	internal interface IASPxGridViewFilterControl {
		event EventHandler<CustomizeFilterControlColumnEventArgs> CustomizeColumn;
	}
	internal class ASPxGridViewFilterControlColumnsHelper : IASPxGridViewFilterControl, IDisposable {
		public interface IFilterControlColumnsOwner {
			event FilterControlColumnsCreatedEventHandler FilterControlColumnsCreated;
		}
		public class ASPxGridViewFilterControlColumnsOwnerAdapter : IFilterControlColumnsOwner {
			private void Grid_FilterControlColumnsCreated(object source, FilterControlColumnsCreatedEventArgs e) {
				if(FilterControlColumnsCreated != null) {
					FilterControlColumnsCreated(this, e);
				}
			}
			public ASPxGridViewFilterControlColumnsOwnerAdapter(ASPxGridView gridView) {
				Guard.ArgumentNotNull(gridView, "gridView");
				gridView.FilterControlColumnsCreated += Grid_FilterControlColumnsCreated;
			}
			public event FilterControlColumnsCreatedEventHandler FilterControlColumnsCreated;
		}
		private IFilterControlColumnsOwner columnsOwner;
		private int maxHierarchyDepth;
		private void ProcessCreatedColumnsRecursive(string parentPropertyName, FilterControlColumnCollection columns, int recursionLevel) {
			if(recursionLevel >= maxHierarchyDepth) {
				return;
			}
			for(int i = columns.Count - 1; i >= 0; i--) {
				FilterControlColumn column = columns[i];
				if(!string.IsNullOrEmpty(column.PropertyName)) {
					string fullPropertyName = string.IsNullOrEmpty(parentPropertyName) ? column.PropertyName : (parentPropertyName + "." + column.PropertyName);
					CustomizeFilterControlColumnEventArgs args = new CustomizeFilterControlColumnEventArgs(column.PropertyName, fullPropertyName, column.DisplayName);
					if(CustomizeColumn != null) {
						CustomizeColumn(this, args);
						if(!args.Visible) {
							columns.Remove(column);
						}
						else {
							column.DisplayName = args.DisplayName;
							DevExpress.Web.FilterControlComplexTypeColumn objColumn = column as DevExpress.Web.FilterControlComplexTypeColumn;
							if(objColumn != null) {
								ProcessCreatedColumnsRecursive(fullPropertyName, objColumn.Columns, recursionLevel + 1);
							}
						}
					}
				}
			}
		}
		private void Grid_FilterControlColumnsCreated(object source, FilterControlColumnsCreatedEventArgs e) {
			ProcessCreatedColumnsRecursive("", e.Columns, 0);
		}
		public ASPxGridViewFilterControlColumnsHelper(ASPxGridView gridView) 
			: this(new ASPxGridViewFilterControlColumnsOwnerAdapter(gridView), gridView.SettingsFilterControl.MaxHierarchyDepth) {
		}
		public ASPxGridViewFilterControlColumnsHelper(IFilterControlColumnsOwner columnsOwner, int hierarchyMaxDepthLevel = 4) {
			Guard.ArgumentNotNull(columnsOwner, "columnsOwner");
			this.columnsOwner = columnsOwner;
			this.maxHierarchyDepth = hierarchyMaxDepthLevel;
			this.columnsOwner.FilterControlColumnsCreated += Grid_FilterControlColumnsCreated;
		}
		public void Dispose() {
			if(columnsOwner != null) {
				columnsOwner.FilterControlColumnsCreated -= Grid_FilterControlColumnsCreated;
				columnsOwner = null;
			}
		}
		public event EventHandler<CustomizeFilterControlColumnEventArgs> CustomizeColumn;
	}
}
