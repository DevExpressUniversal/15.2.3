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
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Drawing.Design;
using System.Collections;
using System.ComponentModel.Design;
using DevExpress.XtraBars;
using DevExpress.XtraPrinting.Preview;
using System.Drawing;
using DevExpress.XtraPrinting.Links;
namespace DevExpress.XtraPrinting.Design {
	public class RichTextEditor : UITypeEditor {
		private IWindowsFormsEditorService edSvc = null;
		Size size = Size.Empty;
		protected virtual string Caption {
			get { return "Rich Text Editor"; }
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object objValue) {
			if(context != null && context.Instance != null && provider != null) {
				edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if(edSvc != null) {
					Forms.RichTextEditorForm form = new Forms.RichTextEditorForm() { Text = Caption };
					form.EditValue = (string)objValue;
					if(!size.IsEmpty)
						form.ClientSize = size;
					if(edSvc.ShowDialog(form) == DialogResult.OK) {
						objValue = form.EditValue;
						context.OnComponentChanged();
					}
					size = form.ClientSize;
					form.Dispose();
				}
			}
			return objValue;
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			if(context != null && context.Instance != null) {
				return UITypeEditorEditStyle.Modal;
			}
			return base.GetEditStyle(context);
		}
	}
	public class ReportHeaderEditor : RichTextEditor {
		protected override string Caption {
			get { return "Report Header Editor"; }
		}
	}
	public class ReportFooterEditor : RichTextEditor {
		protected override string Caption {
			get { return "Report Footer Editor"; }
		}
	}
	public class PageHeaderEditor : RichTextEditor {
		protected override string Caption {
			get { return "Page Header Editor"; }
		}
	}
	public class PageFooterEditor : RichTextEditor {
		protected override string Caption {
			get { return "Page Footer Editor"; }
		}
	}
	public class PageHeaderFooterEditor : UITypeEditor
	{
		private IWindowsFormsEditorService edSvc = null;
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object objValue) {
			if (context != null && context.Instance != null && provider != null) {
				edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (edSvc != null) {
					IList images = new Image[0];
					if(context.Instance is IWinLink)
						images = ((IWinLink)context.Instance).Images;
					Form form = DevExpress.XtraPrinting.InternalAccess.PageHeaderFooterAccessor.GetEditorForm((PageHeaderFooter)objValue, images);
					if(edSvc.ShowDialog(form) == DialogResult.OK) {
						context.OnComponentChanged();
					}
				}
			}
 			return objValue;
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			if (context != null && context.Instance != null) {
				return UITypeEditorEditStyle.Modal;
			}
			return base.GetEditStyle(context);
		}
	}
	public class LinkItemsEditor : UITypeEditor
	{
		private IWindowsFormsEditorService edSvc = null;
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object objValue) {
			if (context != null && context.Instance != null && provider != null) {
				edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));	
				if (edSvc != null) {
					IDesignerHost host = (IDesignerHost)provider.GetService(typeof(IDesignerHost));
					LinkCollection links = (LinkCollection)objValue;
					ArrayList array = new ArrayList(links);
					DesignerTransaction trans = host.CreateTransaction("CollectionEditorUndo");
					bool acceptChanges = true;
					try {
						LinkItemsEditorForm editor = new LinkItemsEditorForm(context);
						editor.EditValue = array;
						if(edSvc.ShowDialog(editor) != DialogResult.OK) {
							acceptChanges = false;
						}
						if(!editor.IsDisposed)
							editor.Dispose();
					}
					catch {
						acceptChanges = false;
					}
					if( acceptChanges ) { 
						trans.Commit();
						links.CopyFrom(array);
					}
					else trans.Cancel();
				}
			}
			return objValue;
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			if (context != null && context.Instance != null) {
				return UITypeEditorEditStyle.Modal;
			}
			return base.GetEditStyle(context);
		}
	}
 	public class PrintableLinkEditor : WindowsFormsComponentEditor 
	{
		public PrintableLinkEditor() : base() {
		}
		public override bool EditComponent(ITypeDescriptorContext context, object component, IWin32Window owner) {
			if(component is PrintableComponentLink) {
				PrintableComponentLink link = (PrintableComponentLink)component;
				ShowEditorForm(link, owner);
			}
			return base.EditComponent(context, component, owner);
		}
		private DialogResult ShowEditorForm(PrintableComponentLink link, IWin32Window owner) {
			try {
				IPrintable printableComponent = link.Component;
				UserControl propertyEditor = printableComponent.PropertyEditorControl;
				if(propertyEditor != null) {
					ComponentEditorForm form = new ComponentEditorForm(link);
					return form.ShowDialog(owner);
				}
			} catch {
				MessageBox.Show("It is impossible to invoke Property Pages Editor",
					"Printable Link Editor", 
					MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			return DialogResult.None;
		}
	}
	public class SelectAreaEditor : UITypeEditor 
	{
		internal IWindowsFormsEditorService edSvc = null;
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if (context != null 
				&& context.Instance != null
				&& provider != null) {
				edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (edSvc != null) {
					SelectAreaEditorForm form = new SelectAreaEditorForm(this, (BrickModifier)value);
					edSvc.DropDownControl(form);
					form.Dispose();
					edSvc = null;
					value = form.EditValue;
				}
			}
			return value;
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			if (context != null && context.Instance != null) {
				return UITypeEditorEditStyle.DropDown;
			}
			return base.GetEditStyle(context);
		}
	}
}
