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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Web.Design;
using DevExpress.Web;
namespace DevExpress.Web.Design {
	public class ColorEditItemCollectionEditor : DevExpress.Web.Design.CollectionEditor {
		public override Form CreateEditorForm(object component, ITypeDescriptorContext context,
			IServiceProvider provider, object propertyValue) {
			return new ColorEditItemCollectionEditorForm(component, context, provider, propertyValue);
		}
	}
	public class ColorEditItemCollectionEditorForm : CollectionEditorForm {
		ToolStripButton retrieveButton = null;
		public ColorEditItemCollectionEditorForm(object component, ITypeDescriptorContext context,
			IServiceProvider provider, object propertyValue)
			: base(component, context, provider, propertyValue) {
		}
		protected override Size FormDefaultSize { get { return new Size(610, 500); } }
		protected override Size FormMinimumSize { get { return new Size(600, 500); } }
		protected override int LeftPanelMinimizeWidth { get { return 300; } }
		protected override int LeftPanelDefaultWidth { get { return 300; } }
		protected new ASPxColorEditDesigner Designer {
			get { return base.Designer as ASPxColorEditDesigner; }
		}
		protected override void AddToolStripButtons(List<ToolStripItem> buttons) {
			base.AddToolStripButtons(buttons);
			buttons.Add(CreateToolStripSeparator());
			this.retrieveButton = new ToolStripButton(string.Format("Create Default {0}", GetItemsName()), null, new EventHandler(OnRetrieveDefaultItems));
			buttons.Add(this.retrieveButton);
		}
		protected void OnRetrieveDefaultItems(object sender, EventArgs e) {
			DialogResult result = Collection.Count == 0
				? DialogResult.No
				: ShowMessage(string.Format("Do you want to delete all existing {0} before retrieving the editor's default item collection?", GetItemsName().ToLower()),
					string.Format("Create default {0} for '{1}'", GetItemsName().ToLower(), (Component as DevExpress.Web.ASPxWebControl).ID), MessageBoxButtons.YesNoCancel);
			if(result != DialogResult.Cancel) {
				CreateDefaultItems(result == DialogResult.Yes);
				AssignControls();
				ComponentChanged(false);
			}
		}
		protected internal DialogResult ShowMessage(string message, string caption, MessageBoxButtons buttons) {
			IServiceProvider serviceProvider = (Component as DevExpress.Web.ASPxWebControl).Site;
			if(serviceProvider != null) {
				IUIService service = (IUIService)serviceProvider.GetService(typeof(IUIService));
				if(service != null)
					return service.ShowMessage(message, caption, buttons);
			}
			return MessageBox.Show(message, caption, buttons);
		}
		protected virtual string GetItemsName() {
			return "Items";
		}
		protected virtual void CreateDefaultItems(bool deleteExistingItems) {
			(Collection as ColorEditItemCollection).CreateDefaultItems(deleteExistingItems);
		}
	}
}
