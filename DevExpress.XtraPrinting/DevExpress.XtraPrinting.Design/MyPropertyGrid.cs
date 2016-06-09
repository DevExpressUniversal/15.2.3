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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Reflection;
namespace DevExpress.XtraPrinting.Design
{
	internal class ComponentSite : ISite 
	{
		IServiceProvider sp;
		IComponent comp;
		public ComponentSite(IServiceProvider sp, IComponent comp) {
			this.sp = sp;
			this.comp = comp;
		}
		IComponent ISite.Component {
			get { return comp;}
		}
		IContainer ISite.Container {
			get { return null; }
		}
		bool ISite.DesignMode {
			get { return false; }
		}
		string ISite.Name {
			get { return null; }
			set { }
		}
		object IServiceProvider.GetService(Type t) {
			if(t.Equals(typeof(System.ComponentModel.Design.IDesignerHost)))
				return sp.GetService(t);
			return null;
		}
	}
	internal class MyPropertyGrid : PropertyGrid
	{
		private System.ComponentModel.Container components = null;
		public MyPropertyGrid() {
			InitializeComponent();
		}
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#region Component Designer generated code
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion
		public void ShowEvents(bool show) {
			ShowEventsButton(show);
		}
		public bool DrawFlat { 
			get { return DrawFlatToolbar; }
			set { DrawFlatToolbar = value; }
		}
		public object GetSelectedObject() {
			if(SelectedObjects != null && SelectedObjects.Length > 0) 
				return SelectedObjects[0];
			return SelectedObject;
		}
		public void UpdateSite(IServiceProvider provider) {
			Site = null;
			if(provider != null) {
				Site = new ComponentSite(provider, this as IComponent);
				PropertyTabs.AddTabType(typeof(System.Windows.Forms.Design.EventsTab));
			}
		}
	}
}
