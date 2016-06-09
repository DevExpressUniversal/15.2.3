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
using System.ComponentModel;
using System.Drawing.Text;
using System.Drawing.Imaging;
using System.Windows.Forms;
using DevExpress.XtraReports.UI;
using System.Collections;
using System.Windows.Forms.Design;
using System.ComponentModel.Design;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Design.Commands;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraBars;
using DevExpress.XtraReports.UserDesigner; 
using DevExpress.XtraPrinting.Preview;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraReports.Design 
{
	[ToolboxItem(false)]
	public abstract class XtraTabControl : ContainerControl {
		#region inner classes
		[ToolboxItem(false)]
		public class TabPage : IDisposable {
			private Control control;
			private bool selected;
			bool visible;
			string text;
			Image image;
			private ReportCommand command;
			public event EventHandler PageActivate;
			public virtual bool Selected { get { return selected; } 
				set { 
					if(selected != value) {
						selected = value;
						if(control != null) {
							control.Visible = value;
							if(control.Visible)
								control.BringToFront();
						}
					}
				}
			}
			public virtual bool Visible { 
				get { return visible; } 
				set { visible = value; }
			}
			public ReportCommand Command { get { return command; }
			}
			public string Text { get { return text; }
			}
			public Control Control {
				get { return control; }
				set { control = value; }
			}
			public virtual Image Image { 
				get { return image; } 
				set { image = value; }
			}
			public TabPage(Control control, string text, ReportCommand command) {
				this.control = control;
				this.command = command;
				this.text = text;
			}
			public virtual void Dispose() {
			}
			protected virtual void OnPageActivate(EventArgs e) {
				PageActivate(this, e);
			}
		}
		public class TabPages : IDisposable {
			ArrayList tabPages = new ArrayList();
			public TabPage this[int index] { 	
				get { return tabPages[LastControlIndex - index] as TabPage; }
			}
			public int Count { get { return tabPages.Count; }
			}
			public int LastControlIndex { get { return tabPages.Count - 1; }
			}
			public TabPages() {
			}
			public void Remove(TabPage page) {
				tabPages.Remove(page);
			}
			public void Add(TabPage page) {
				tabPages.Insert(0, page);
			}
			public int IndexOf(TabPage page) {
				return LastControlIndex - tabPages.IndexOf(page);
			}
			public void Dispose() {
				foreach(TabPage tabPage in tabPages)
					tabPage.Dispose();
			}
		}
		#endregion
		public event EventHandler SelectedTabIndexChanged;
		protected TabPages fTabPages;
		protected TabPage selectedPage;
		public Control SelectedPageControl { get { return selectedPage != null ? selectedPage.Control : null; }
		}
		public ReportCommand SelectedPageCommand {
			get { return selectedPage != null ? selectedPage.Command : ReportCommand.None; }
		}
		public int SelectedIndex { 
			get { 
				return fTabPages.IndexOf(selectedPage); 
			}
			set {
				value = Math.Max(0, Math.Min(value, fTabPages.LastControlIndex));
				SelectTabPage( fTabPages[value] );
			}
		}
		public TabPages Pages { get { return fTabPages; }
		}
		protected XtraTabControl() {
			this.fTabPages = new TabPages();
		}
		public void AddControl(Control control) {
			Controls.Add(control);
			control.Dock = DockStyle.Fill;
			control.BringToFront();
			control.Visible = false;		
		}
		public void AddPage(TabPage page) {
			fTabPages.Add(page);
			page.PageActivate += new EventHandler(OnPageActivate);
		}
		public void AddPage(Control control, TabPage page) {
			AddControl(control);
			AddPage(page);			
		}
		protected abstract void OnPageActivate(object sender, EventArgs e);
		protected virtual void SelectTabPage(TabPage tabPage) {
			if(Comparer.Equals(selectedPage, tabPage))
				return;
			if(selectedPage != null) {
				selectedPage.Selected = false;
			}
			selectedPage = tabPage;
			selectedPage.Selected = true;
			OnSelectedPageChanged(EventArgs.Empty);
		}
		protected virtual void OnSelectedPageChanged(EventArgs e) {
			if(SelectedTabIndexChanged != null) SelectedTabIndexChanged(this, EventArgs.Empty);
		}
		protected override bool ProcessDialogKey(Keys keyData) {
			return false;
		}
		protected override void Dispose(bool disposing) {
			for(int i = 0; i < fTabPages.Count; i++ )
				fTabPages[i].PageActivate -= new EventHandler(OnPageActivate);
			fTabPages.Dispose();
			base.Dispose(disposing);
		}
	}
}
