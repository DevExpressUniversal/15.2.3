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
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.PropertyGrid.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
namespace DevExpress.Xpf.PropertyGrid {
	public delegate void PropertyGridSortingEventHandler(object sender, PropertyGridSortingEventArgs args);
	public delegate void CellValueChangedEventHandler(object sender, CellValueChangedEventArgs args);
	public delegate void CellValueChangingEventHandler(object sender, CellValueChangingEventArgs args);
	public delegate void ValidateCellEventHandler(object sender, ValidateCellEventArgs args);
	public delegate void InvalidCellExceptionEventHandler(object sender, InvalidCellExceptionEventArgs args);
	public delegate void CustomDisplayTextEventHandler(object sender, CustomDisplayTextEventArgs args);
	public delegate void CustomExpandEventHandler(object sender, CustomExpandEventArgs args);
	public delegate void PropertyGridEditorEventHandler(object sender, PropertyGridEditorEventArgs args);
	public delegate void ShowingPropertyGridEditorEventHandler(object sender, ShowingPropertyGridEditorEventArgs e);
	public class PropertyGridSortingEventArgs : RoutedEventArgs {
		public IEnumerable<RowInfo> SourceCollection { get; private set; }
		public IEnumerable<RowInfo> ResultCollection { get; set; }
		public PropertyGridSortingEventArgs(IEnumerable<RowHandle> handles, DataViewBase viewBase, RowDataGenerator generator) {
			SourceCollection = handles.Select(x => new RowInfo(viewBase, x, generator)).AsEnumerable();
		}
	}
	public class CellValueChangedEventArgs : RoutedEventArgs {
		public object OldValue { get; private set; }
		public object NewValue { get; private set; }
		public RowInfo Row { get; private set; }
		public CellValueChangedEventArgs(RowHandle handle, RowDataGenerator generator, object oldValue, object newValue) {
			this.OldValue = oldValue;
			this.NewValue = newValue;
			this.Row = new RowInfo(generator.DataView, handle, generator);
		}
	}
	public class CellValueChangingEventArgs : CancelRoutedEventArgs {
		public object OldValue { get; private set; }
		public object NewValue { get; private set; }
		public RowInfo Row { get; private set; }
		public CellValueChangingEventArgs(RowHandle handle, RowDataGenerator generator, object oldValue, object newValue) {
			this.OldValue = oldValue;
			this.NewValue = newValue;
			this.Row = new RowInfo(generator.DataView, handle, generator);
		}
	}
	public class ValidateCellEventArgs : RoutedEventArgs {
		public RowInfo Row { get; private set; }
		public object OldValue { get; private set; }
		public object NewValue { get; private set; }
		public Exception ValidationException { get; set; }
		public ValidateCellEventArgs(RowHandle handle, RowDataGenerator generator, object oldValue, object newValue) {
			this.OldValue = oldValue;
			this.NewValue = newValue;
			this.Row = new RowInfo(generator.DataView, handle, generator);
		}
	}
	public class InvalidCellExceptionEventArgs : RoutedEventArgs {
		public RowInfo Row { get; private set; }
		public Exception Exception { get; private set; }
		public string Message { get; set; }
		public InvalidCellExceptionEventArgs(RowHandle handle, RowDataGenerator generator, Exception exception) {
			this.Row = new RowInfo(generator.DataView, handle, generator);
			this.Exception = exception;
			this.Message = exception.Message;
		}
	}
	public class CustomDisplayTextEventArgs : RoutedEventArgs {
		public string DisplayText { get; set; }
		public RowInfo Row { get; private set; }
		public CustomDisplayTextEventArgs(RowHandle handle, RowDataGenerator generator, string displayText) {
			this.DisplayText = displayText;
			this.Row = new RowInfo(generator.DataView, handle, generator);
		}
	}
	public class CustomExpandEventArgs : RoutedEventArgs {
		bool valueAssigned = false;
		bool isExpanded;
		public RowInfo Row { get; private set; }
		protected internal bool ValueAssigned { get { return valueAssigned; } }
		public bool IsExpanded {
			get { return isExpanded; }
			set {
				isExpanded = value;
				valueAssigned = true;
			}
		}
		protected internal CustomExpandEventArgs(RowHandle handle, DataViewBase view, RowDataGenerator generator) {
			isExpanded = view.IsExpanded(handle);
			Row = new RowInfo(view, handle, generator);
		}
	}
	public class ShowingPropertyGridEditorEventArgs : CancelRoutedEventArgs {
		public RowInfo Row { get; private set; }
		public ShowingPropertyGridEditorEventArgs(RowHandle handle, RowDataGenerator generator) {
			Row = new RowInfo(handle, generator);
		}
	}
	public class PropertyGridEditorEventArgs : RoutedEventArgs {
		readonly RowDataGenerator generator;
		readonly RowHandle handle;
		public RowInfo Row { get; private set; }
		public IBaseEdit Editor { get; private set; }
		public object Value { get { return generator.DataView.GetValue(handle); } }
		public PropertyGridEditorEventArgs(RowHandle handle, RowDataGenerator generator, IBaseEdit editor) {
			this.generator = generator;
			this.handle = handle;
			Row = new RowInfo(handle, generator);
		}
	}	
}
