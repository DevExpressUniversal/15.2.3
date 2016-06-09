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
using System.Diagnostics.CodeAnalysis;
using DevExpress.ExpressApp.Editors;
using DevExpress.Web.Rendering;
namespace DevExpress.ExpressApp.Web.Editors.ASPx {
	public class ASPxGridInplaceEditAppearanceAdapter : TableCellAppearanceAdapter, IAppearanceEnabled {
		public ASPxGridInplaceEditAppearanceAdapter(GridViewTableEditorCellBase editCell)
			: base(editCell) {
			SetCell(null);
			this.EditCell = editCell;
		}
		public bool Enabled {
			get { return EditCell.Enabled; }
			set { EditCell.Enabled = value; }
		}
		void IAppearanceEnabled.ResetEnabled() { }
		public GridViewTableEditorCellBase EditCell { get; private set; }
		#region Obsolete 15.2
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters")]
		public ASPxGridInplaceEditAppearanceAdapter(GridViewTableEditorCellBase editCell, object data) : this(editCell) { }
		#endregion
	}
}
