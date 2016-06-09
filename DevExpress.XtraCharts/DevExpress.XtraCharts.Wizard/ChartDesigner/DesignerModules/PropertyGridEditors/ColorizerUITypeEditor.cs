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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Charts.Native;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Editors;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraEditors;
namespace DevExpress.XtraCharts.Designer.Native {
	public class ColorizerUITypeEditor : UITypeEditor {
		IWindowsFormsEditorService editorService;
		void listBox_Click(object sender, EventArgs e) {
			this.editorService.CloseDropDown();
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.DropDown;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			this.editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
			var series = context.Instance as DesignerSeriesModelBase;
			if (series == null) {
				ChartDebug.Fail("DesignerSeriesModelBase expected.");
				return value;
			}
			IEnumerable<Type> subclasses = Assembly.GetAssembly(typeof(ChartPaletteColorizerBase)).GetTypes().Where(t => t.IsSubclassOf(typeof(ChartPaletteColorizerBase)) && !t.IsAbstract);
			var listBox = new ListBoxControl();
			listBox.Click += listBox_Click;
			var commandManager = series.CommandManager;
			listBox.Items.Add("None"); 
			foreach (Type t in subclasses) {
				var colorizer = Activator.CreateInstance(t);
				var colorizerModel = ModelHelper.CreateModelInstance(colorizer, commandManager);
				int index = listBox.Items.Add(colorizerModel);
				if (value != null && colorizerModel.GetType() == value.GetType()) {
					listBox.SelectedIndex = index;
				}
			}
			this.editorService.DropDownControl(listBox);
			if (listBox.SelectedIndex != 0)
				if ( value == null || listBox.SelectedItem.GetType() != value.GetType())
					return listBox.SelectedItem;
				else
					return value;
			else
				return null;
		}
	}
	public class KeyCollectionModelTypeEditor : ChartEditorBase {
		CommandManager commandManager;
		protected override Form CreateForm() {
			KeyCollection collection = Value as KeyCollection;
			KeyColorColorizerModel colorizer = Instance as KeyColorColorizerModel;
			if (collection == null || colorizer == null)
				return null;
			commandManager = colorizer.CommandManager;
			commandManager.BeginTransaction();
			Chart chart = ((IOwnedElement)colorizer.Parent.ChartElement).ChartContainer.Chart;
			DesignerCollectionEditorForm form = new KeyCollectionEditorForm(collection, commandManager);
			form.LookAndFeel.ParentLookAndFeel = (UserLookAndFeel)chart.Container.RenderProvider.LookAndFeel;
			form.Initialize(chart);
			return form;
		}
		protected override void AfterShowDialog(Form form, DialogResult dialogResult) {
			base.AfterShowDialog(form, dialogResult);
			commandManager.CommitTransaction();
			commandManager = null;
		}
	}
	public class KeyCollectionEditorForm : DesignerCollectionEditorForm {
		readonly KeyCollection collection;
		readonly AddColorizerKeyCommand addCommand;
		readonly DeleteColorizerKeyCommand deleteCommand;
		protected override bool SelectableItems { get { return false; } }
		protected override object[] CollectionToArray {
			get {
				object[] itemsArray = new object[collection.Count];
				((ICollection)collection).CopyTo(itemsArray, 0);
				return itemsArray;
			}
		}
		public KeyCollectionEditorForm(KeyCollection collection, CommandManager commandManager) : base() {
			this.collection = collection;
			this.addCommand = new AddColorizerKeyCommand(collection, commandManager);
			this.deleteCommand = new DeleteColorizerKeyCommand(collection, commandManager);
		}
		protected override string GetItemDisplayText(int index) {
			return collection[index].ToString();
		}
		protected override object[] AddItems() {
			try {
				ObjectEditor editor = new ObjectEditor("");
				if (editor.ShowDialog() == DialogResult.OK)
					addCommand.Execute(editor.EditValue);
				int index = collection.Count - 1;
				return new object[] { collection[index] };
			}
			catch {
				return null;
			}
		}
		protected override void RemoveItem(object item) {
			deleteCommand.Execute(item);
		}
		protected override void Swap(int index1, int index2) {
		}
	}
}
