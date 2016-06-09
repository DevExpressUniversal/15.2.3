#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using DevExpress.XtraPrinting;
using System.Windows.Forms;
using DevExpress.Utils.Controls;
using System;
namespace DevExpress.DashboardExport {
	public abstract class DashboardExportPrinter : IDisposable, IPrintable {
		IPrintingSystem psCore;
		protected IPrintingSystem PS {
			get { return psCore; }
		}
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
		}
		protected abstract void CreateDetail(IBrickGraphics graph);
		void SetPS(IPrintingSystem ps) {
			this.psCore = ps;
		}
		void IPrintable.AcceptChanges() { }
		bool IPrintable.CreatesIntersectedBricks { get { return true; } }
		bool IPrintable.HasPropertyEditor() { return false; }
		UserControl IPrintable.PropertyEditorControl { get { return null; } }
		void IPrintable.RejectChanges() { }
		void IPrintable.ShowHelp() { }
		bool IPrintable.SupportsHelp() { return false; }
		void IBasePrintable.CreateArea(string areaName, IBrickGraphics graph) {
			if(PS != null && areaName == SR.Detail)
				CreateDetail(graph);
		}
		void IBasePrintable.Finalize(IPrintingSystem ps, ILink link) {
			SetPS(null);
		}
		void IBasePrintable.Initialize(IPrintingSystem ps, ILink link) {
			SetPS(ps);
		}
	}
}
