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
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Design {
	public partial class AnnotationCollectionEditorForm : CollectionEditorForm {
		readonly IAnnotationCollection collection;
		protected override object[] CollectionToArray { get { return collection.ToArray(); } }
		protected override bool SelectableItems { get { return true; } }
		AnnotationCollectionEditorForm() {
			InitializeComponent();			
		}
		public AnnotationCollectionEditorForm(IAnnotationCollection collection) : this() {
			this.collection = collection;
		}
		protected override void Swap(int index1, int index2) {
			collection.Swap(index1, index2);
		}
		protected override object[] AddItems() {
			using (AnnotationTypesForm annotationTypesForm = new AnnotationTypesForm()) {
				if (annotationTypesForm.ShowDialog() == DialogResult.OK && annotationTypesForm.AnnotationKind != null) {
					Annotation annotation = annotationTypesForm.AnnotationKind.CreateInstance();
					if (annotation != null) {
						annotation.Name = collection.GenerateName(((INamedElement)annotation).NamePrefix);
						collection.Add(annotation);
						return new object[] { annotation };
					}
				}
				return null;
			}
		}
		protected override void RemoveItem(object item) {
			collection.Remove((Annotation)item);
		}
		protected override string GetItemDisplayText(int index) {
			return collection[index].Name;
		}
	}
}
