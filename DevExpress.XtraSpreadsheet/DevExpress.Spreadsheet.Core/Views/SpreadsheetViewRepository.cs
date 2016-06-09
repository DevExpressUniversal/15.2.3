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
using System.Collections.Generic;
using DevExpress.Utils;
using System.ComponentModel;
using System.Diagnostics;
using DevExpress.Utils.Serializing;
using System.Runtime.InteropServices;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraSpreadsheet {
	#region SpreadsheetViewRepository (abstract class)
#if !SL
	[TypeConverter(typeof(ExpandableObjectConverter))]
#endif
	[ComVisible(true)]
	public abstract class SpreadsheetViewRepository : IDisposable {
		#region Fields
		bool isDisposed;
		readonly ISpreadsheetControl control;
		readonly Dictionary<SpreadsheetViewType, SpreadsheetView> views;
		#endregion
		protected SpreadsheetViewRepository(ISpreadsheetControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
			this.views = new Dictionary<SpreadsheetViewType, SpreadsheetView>();
			CreateViews();
		}
		#region Properties
		protected internal bool IsDisposed { get { return isDisposed; } }
		protected internal Dictionary<SpreadsheetViewType, SpreadsheetView> Views { get { return views; } }
		protected internal ISpreadsheetControl Control { get { return control; } }
		[
DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public NormalView NormalView { get { return (NormalView)Views[SpreadsheetViewType.Normal]; } }
		#endregion
		protected internal virtual void CreateViews() {
			RegisterView(CreateNormalView());
		}
		protected internal abstract NormalView CreateNormalView();
		protected internal virtual void RegisterView(SpreadsheetView view) {
			Guard.ArgumentNotNull(view, "view");
#if DEBUG
			SpreadsheetView result;
			Debug.Assert(Views.TryGetValue(view.Type, out result) == false);
#endif
			Views.Add(view.Type, view);
		}
		protected internal virtual SpreadsheetView GetViewByType(SpreadsheetViewType type) {
			return Views[type];
		}
		protected internal virtual void DisposeViews() {
			foreach (SpreadsheetViewType type in Views.Keys)
				Views[type].Dispose();
			Views.Clear();
		}
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing)
				DisposeViews();
			isDisposed = true;
		}
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		public override string ToString() {
			return String.Empty;
		}
		public void RecreateScrollControllers() {
			foreach (SpreadsheetViewType type in Views.Keys)
				Views[type].RecreateScrollControllers();
		}
	}
	#endregion
}
