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
using System.Windows.Forms;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Design {
	public partial class AnnotationShapePositionTypesForm : ItemKindChoosingForm {
		AnnotationShapePosition currentPosition;
		Annotation annotation;
		public AnnotationShapePosition EditValue { 
			get { return currentPosition; } 
			set { 
				currentPosition = value;
				annotation = ((IOwnedElement)value).Owner as Annotation;
				foreach (ListViewItem item in listView.Items)
					item.Selected = item.Text == StringResourcesUtils.GetStringId(currentPosition);
			} 
		}
		public AnnotationShapePositionTypesForm() {
			InitializeComponent();
		}
		void AddItem(AnnotationShapePosition position) {
			imageList.Images.Add(ImageResourcesUtils.GetImageFromResources(position));
			listView.Items.Add(new ListViewItem(StringResourcesUtils.GetStringId(position), imageList.Images.Count - 1));
		}
		protected override void CloseForm() {
			ListView.SelectedListViewItemCollection coll = listView.SelectedItems;
			if (coll.Count > 0) 
				DialogResult = DialogResult.OK;			
		}
		protected override void Initialize() {
			AddItem(new FreePosition());
			AddItem(new RelativePosition());			
		}
		protected override void SelectedIndexChanged() {
			if (listView.SelectedItems.Count > 0) {
				IChartContainer container = CommonUtils.FindChartContainer(annotation);
				AnnotationShapePosition position = AnnotationHelper.CreateFreePosition(annotation, container.Chart);
				if (listView.SelectedItems[0].Text == StringResourcesUtils.GetStringId(position)) {
					currentPosition = position;
					return;
				}
				position = AnnotationHelper.CreateRelativePosition(annotation);
				if (listView.SelectedItems[0].Text == StringResourcesUtils.GetStringId(position)) 
					currentPosition = position;
			}
		}
	}
}
