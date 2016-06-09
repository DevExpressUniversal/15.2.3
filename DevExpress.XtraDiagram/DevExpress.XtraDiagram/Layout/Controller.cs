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
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using DevExpress.Utils;
using DevExpress.XtraDiagram.Utils;
namespace DevExpress.XtraDiagram.Layout {
	public class DiagramSerializationController : IDisposable {
		DiagramControl diagram;
		DesignerTransaction transaction;
		public DiagramSerializationController(DiagramControl diagram) {
			this.diagram = diagram;
			this.transaction = null;
		}
		public void OnStartSerializing() { }
		public void OnEndSerializing() { }
		bool deserializing = false;
		public void OnStartDeserializing(LayoutAllowEventArgs e) {
			this.deserializing = true;
			IDesignerHost designerHost = GetDesignerHost();
			if(designerHost != null) {
				this.transaction = designerHost.CreateTransaction("DiagramDeserializing");
			}
			Diagram.BeginUpdate();
			diagram.Items.DestroyItems();
		}
		public void OnEndDeserializing(string restoredVersion) {
			this.deserializing = false;
			try {
				if(this.transaction != null) {
					this.transaction.Commit();
					this.transaction = null;
				}
				OnLoaded();
			}
			finally {
				Diagram.EndUpdate();
			}
			Diagram.DoFireChanged();
		}
		protected bool Deserializing { get { return deserializing; } }
		public bool OnAllowSerializationProperty(OptionsLayoutBase options, string propertyName, int id) {
			return true;
		}
		public void OnResetSerializationProperties(OptionsLayoutBase options) {
		}
		protected virtual void OnLoaded() {
		}
		protected virtual IDesignerHost GetDesignerHost() {
			return Diagram.Site.Select(x => x.GetService(typeof(IDesignerHost))) as IDesignerHost;
		}
		#region IDisposable
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			this.diagram = null;
		}
		#endregion
		public DiagramControl Diagram { get { return diagram; } }
	}
}
