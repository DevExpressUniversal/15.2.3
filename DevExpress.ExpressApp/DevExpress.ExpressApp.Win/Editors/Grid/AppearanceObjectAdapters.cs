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
using DevExpress.Utils;
using DevExpress.ExpressApp.Editors;
using System.ComponentModel;
using DevExpress.XtraGrid.Views.Grid;
namespace DevExpress.ExpressApp.Win.Editors {
	public class GridViewCancelEventArgsAppearanceAdapterWithReset : AppearanceObjectAdapter {
		public GridViewCancelEventArgsAppearanceAdapterWithReset(GridView gridView, CancelEventArgs cancelEdit, AppearanceObject appearanceObject)
			: base(appearanceObject) {
			this.Args = cancelEdit;
			this.GridView = gridView;
		}
		public GridView GridView { get; private set; }
		public CancelEventArgs Args { get; private set; }
	}
	public class GridViewCancelEventArgsAppearanceAdapter : IAppearanceEnabled {
		public GridViewCancelEventArgsAppearanceAdapter(GridView gridView, CancelEventArgs cancelEdit) {
			this.Args = cancelEdit;
			this.GridView = gridView;
		}
		public GridView GridView { get; private set; }
		public CancelEventArgs Args { get; private set; }
		#region IAppearanceEnabled Members
		public bool Enabled {
			get { return !Args.Cancel; }
			set { Args.Cancel = !value; }
		}
		public void ResetEnabled() { }
		#endregion
	}
	public class GridViewRowCellStyleEventArgsAppearanceAdapter : AppearanceObjectAdapter {
		public GridViewRowCellStyleEventArgsAppearanceAdapter(GridView gridView, RowCellStyleEventArgs args)
			: base(args.Appearance) {
			this.Args = args;
			this.GridView = gridView;
		}
		public GridView GridView { get; private set; }
		public RowCellStyleEventArgs Args { get; private set; }
	}
}
