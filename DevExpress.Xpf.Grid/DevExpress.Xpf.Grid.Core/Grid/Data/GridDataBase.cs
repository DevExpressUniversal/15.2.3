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

using System.Windows;
using DevExpress.Xpf.Core;
using System.ComponentModel;
using DevExpress.Xpf.Editors;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Grid {
	public abstract class GridDataBase : EditableDataObject {
		object data;
		public object Data {
			get { return data; }
			set {
				if(data != value) {
					data = value;
					OnDataChanged();
					RaisePropertyChanged("Data");
				}
			}
		}
		protected CellEditorBase editor;
		internal CellEditorBase Editor { 
			get { return editor; } 
			set {
				if(editor != value) {
					editor = value;
					OnEditorChanged();
				}
			} 
		}
		protected virtual void OnEditorChanged() {
		}
		bool contentChangedRaised;
		protected internal virtual void UpdateValue() { }
		protected virtual void OnDataChanged() {
			contentChangedRaised = false;
			UpdateValue();
			if(!contentChangedRaised && CanRaiseContentChangedWhenDataChanged())
				RaiseContentChanged();
		}
		protected virtual bool CanRaiseContentChangedWhenDataChanged() {
			return false;
		}
		protected override void RaiseContentChanged() {
			base.RaiseContentChanged();
			contentChangedRaised = true;
		}
		protected internal virtual void ClearBindingValue() { }
	}
}
