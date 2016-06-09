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

using DevExpress.Utils.Taskbar;
using DevExpress.XtraBars.Utils;
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
namespace DevExpress.Utils.Design.Taskbar {
	public class ThumbnailButtonCollectionEditor : DXCollectionEditorBase {
		public ThumbnailButtonCollectionEditor(Type type) : base(type) { }
		protected override DXCollectionEditorBase.DXCollectionEditorBaseForm CreateCollectionForm() {
			return new ThumbnailButtonCollectionEditorForm(this);
		}
		class ThumbnailButtonCollectionEditorContentControl : DXCollectionEditorContent {
			public ThumbnailButtonCollectionEditorContentControl()
				: base() {
				if(this.PropertyGrid == null) throw new Exception("PropertyGrid not found...");
				this.PropertyGrid.SelectedObjectsChanged += GridSelectedObjectsChanged;
			}
			void GridSelectedObjectsChanged(object sender, EventArgs e) {
				this.PropertyGrid.PropertyTabs.AddTabType(typeof(DevExpress.XtraEditors.Designer.Utils.DXEventsTabEx), PropertyTabScope.Document);
				this.PropertyGrid.ShowTabEvents(true);
			}
		}
		class ThumbnailButtonCollectionEditorForm : DXCollectionEditorBase.DXCollectionEditorBaseForm {
			public ThumbnailButtonCollectionEditorForm(DXCollectionEditorBase editor) : base(editor) { }
			protected override DXCollectionEditorContent CreateContentControl() {
				return new ThumbnailButtonCollectionEditorContentControl();
			}
		}
	}
	public class JumpListItemChangeFile : UITypeEditor {
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			using(OpenFileDialog dialog = new OpenFileDialog()) {
				dialog.ShowDialog();
				value = dialog.FileName;
			}
			return base.EditValue(context, provider, value);
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.Modal;
		}
	}
	public class JumpListTaskCollectionEditor : ThumbnailButtonCollectionEditor {
		public JumpListTaskCollectionEditor(Type type) : base(type) { }
		protected override Type[] CreateNewItemTypes() {
			return new Type[] { typeof(JumpListItemTask), typeof(JumpListItemSeparator) };
		}
	}
	public class JumpListCategoryCollectionEditor : UITypeEditor {
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			JumpListCategoryCollection oldValue = CreateOldValue(value);
			IDesignerHost host = (IDesignerHost)provider.GetService(typeof(IDesignerHost));
			using(DesignerTransaction transact = host.CreateTransaction("JumpListCustomCategory")) {
				transact.Commit();
				using(JumpListEditorCategoryCollectionForm form = new JumpListEditorCategoryCollectionForm(context, provider, value)) {
					form.ShowDialog();
					if(form.DialogResult == DialogResult.OK)
						return form.Result;
					transact.Cancel();
					return oldValue;
				}
			}
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.Modal;
		}
		JumpListCategoryCollection CreateOldValue(object value) {
			JumpListCategoryCollection result = new JumpListCategoryCollection();
			foreach(JumpListCategory item in (JumpListCategoryCollection)value)
				result.Add(item);
			return result;
		}
	}
	public class ThumbnailClipRegionEditor : UITypeEditor {
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			using(ThumbnailClipRegionEditorForm frm = new ThumbnailClipRegionEditorForm()) {
				using(ThumbnailClipRegionControl control = new ThumbnailClipRegionControl()) {
					control.FormScreen = BarUtilites.RenderToBitmap(((TaskbarAssistant)context.Instance).ParentControl);
					frm.Controls.Add(control);
					control.Dock = DockStyle.Fill;
					control.StartValue = (Rectangle)value;
					frm.Width = control.FormScreen.Width + 80;
					frm.Height = control.FormScreen.Height + 80;
					frm.ShowDialog();
					if(frm.DialogResult == DialogResult.OK)
						value = frm.Result;
				}
			}
			return base.EditValue(context, provider, value);
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.Modal;
		}
	}
}
