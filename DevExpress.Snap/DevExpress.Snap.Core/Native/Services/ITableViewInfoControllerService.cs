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
using System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Layout.TableLayout;
using DevExpress.XtraRichEdit.Native;
using DevExpress.XtraRichEdit;
namespace DevExpress.Snap.Core.Native.Services {
	public interface ITableViewInfoControllerService {
		void Add<TMouseHandlerState, TTableViewInfoController>() where TMouseHandlerState : MouseHandlerState where TTableViewInfoController : TableViewInfoController;
		void AddIfNotPresent<TMouseHandlerState, TTableViewInfoController>() where TMouseHandlerState : MouseHandlerState where TTableViewInfoController : TableViewInfoController;
		void Remove<TMouseHandlerState>() where TMouseHandlerState : MouseHandlerState;
		void Clear();
		TableViewInfoController Get(MouseHandlerState mouseHandlerState, RichEditView view, Point logicalPoint);
	}
	public class TableViewInfoControllerService : ITableViewInfoControllerService {
		readonly TableViewInfoControllers tableViewInfoControllers = new TableViewInfoControllers();
		public void Add<TMouseHandlerState, TTableViewInfoController>()
			where TMouseHandlerState : MouseHandlerState
			where TTableViewInfoController : TableViewInfoController {
				tableViewInfoControllers.Add<TMouseHandlerState, TTableViewInfoController>();
		}
		public void AddIfNotPresent<TMouseHandlerState, TTableViewInfoController>()
			where TMouseHandlerState : MouseHandlerState
			where TTableViewInfoController : TableViewInfoController {
				if (!tableViewInfoControllers.Contains<TMouseHandlerState>())
					tableViewInfoControllers.Add<TMouseHandlerState, TTableViewInfoController>();
		}
		public void Remove<TMouseHandlerState>()
			where TMouseHandlerState : MouseHandlerState {
				tableViewInfoControllers.Remove<TMouseHandlerState>();
		}
		public void Clear() {
			tableViewInfoControllers.Clear();
		}
		public TableViewInfoController Get(MouseHandlerState mouseHandlerState, RichEditView view, Point logicalPoint) {
			return tableViewInfoControllers.Get(mouseHandlerState, view, logicalPoint);
		}
	}
	public class TableViewInfoControllers : ObjectFactoryBase<MouseHandlerState, TableViewInfoController> {
		protected override System.Type[] GetConstructorParameters<TKey, T>() {
			return new Type[] { typeof(RichEditView), typeof(Point) };
		}
		public TableViewInfoController Get(MouseHandlerState state, RichEditView view, Point mousePosition) {
			return base.Get(state.GetType(), view, mousePosition);
		}
	}
}
