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
using System.Collections.Generic;
using System.Text;
using System.Collections;
namespace DevExpress.ExpressApp.Web {
	public class TemporarySelectionContext : ISelectionContext, IDisposable {
		private ArrayList selectedObjects = new ArrayList();
		public TemporarySelectionContext(IEnumerable selectedObjects) {
			foreach(object obj in selectedObjects) {
				this.selectedObjects.Add(obj);
			}
		}
		public TemporarySelectionContext(object selectedObj) {
			selectedObjects.Add(selectedObj);
		}
		#region ISelectionContext Members
		public object CurrentObject {
			get { return null; }
		}
		public IList SelectedObjects {
			get { return selectedObjects; }
		}
		public SelectionType SelectionType {
			get { return SelectionType.TemporarySelection; }
		}
		public string Name {
			get { return "TemporarySelectionContext"; }
		}
		public bool IsRoot {
			get { return false; }
		}
		event EventHandler ISelectionContext.CurrentObjectChanged {
			add { }
			remove { }
		}
		event EventHandler ISelectionContext.SelectionChanged {
			add { }
			remove { }
		}
		event EventHandler ISelectionContext.SelectionTypeChanged {
			add { }
			remove { }
		}
		#endregion
		#region IDisposable Members
		public void Dispose() {
			selectedObjects.Clear();
		}
		#endregion
	}
}
