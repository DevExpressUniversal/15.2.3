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
using System.Diagnostics;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.XtraEditors;
namespace DevExpress.XtraMap.Design {
	public class EllipsoidEditor : UITypeEditor {
		Ellipsoid selectedEllipsoid = null;
		public override bool IsDropDownResizable {
			get { return true; }
		}
		void InitializeListBox(ListBoxControl lbEllipsoids) {
			lbEllipsoids.BeginUpdate();
			object[] predefinedEllipsoids = Ellipsoid.GetPredefinedEllipsoids();
			lbEllipsoids.Items.AddRange(predefinedEllipsoids);
			lbEllipsoids.Items.Add(DesignSR.CustomEllipsoid);
			lbEllipsoids.SelectedIndex = -1;
			lbEllipsoids.SelectedValueChanged += lbEllipsoids_SelectedValueChanged;
			lbEllipsoids.EndUpdate();
		}
		void lbEllipsoids_SelectedValueChanged(object sender, EventArgs e) {
			var listBox = sender as ListBoxControl;
			if (listBox == null || !(listBox.Tag is IWindowsFormsEditorService)) {
				Debug.Fail("EllipsoidEditor.lbEllipsoids_SelectedValueChanged: Incorrect operation or there is no IWindowsFormsEditorService in the Tag property.");
				return;
			}
			var winFormsEditorService = (IWindowsFormsEditorService)listBox.Tag;
			if (listBox.SelectedItem is Ellipsoid)
				selectedEllipsoid = (Ellipsoid)listBox.SelectedItem;
			else {
				var form = new CustomEllipsoidEditorForm();
				form.FormClosing += form_FormClosing;
				winFormsEditorService.ShowDialog(form);
			}
			winFormsEditorService.CloseDropDown();
		}
		void form_FormClosing(object sender, FormClosingEventArgs e) {
			var form = sender as CustomEllipsoidEditorForm;
			if (form == null)
				return;
			selectedEllipsoid = form.Ellipsoid;
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.DropDown;
		}
		public override bool GetPaintValueSupported(ITypeDescriptorContext context) {
			return false;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if (context == null || context.Instance == null || provider == null)
				return value;
			var winFormsEditorService = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
			if (winFormsEditorService == null)
				return value;
			using (ListBoxControl lbUnits = new ListBoxControl() { Tag = winFormsEditorService }) {
				InitializeListBox(lbUnits);
				winFormsEditorService.DropDownControl(lbUnits);
			}
			return selectedEllipsoid == null ? value : selectedEllipsoid;
		}
	}
}
