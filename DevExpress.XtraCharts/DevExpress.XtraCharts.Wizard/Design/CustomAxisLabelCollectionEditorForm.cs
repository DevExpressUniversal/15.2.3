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

using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Design {
	public class CustomAxisLabelCollectionEditorForm : CollectionEditorForm {
		readonly CustomAxisLabelCollection collection;
		protected override bool SelectableItems { get { return false; } }
		protected override object[] CollectionToArray { get { return collection.ToArray(); } }
		CustomAxisLabelCollectionEditorForm() {
			InitializeComponent();
		}
		public CustomAxisLabelCollectionEditorForm(CustomAxisLabelCollection collection) : this() {
			this.collection = collection;
		}
		#region Designer generated code
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomAxisLabelCollectionEditorForm));
			((System.ComponentModel.ISupportInitialize)(this.fListBox)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this, "$this");
			this.Name = "CustomAxisLabelCollectionEditorForm";
			((System.ComponentModel.ISupportInitialize)(this.fListBox)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		protected override string GetItemDisplayText(int index) {
			return collection[index].Name;
		}
		protected override object[] AddItems() {
			IAxisData axis = ((IOwnedElement)collection).Owner as IAxisData;
			CustomAxisLabel label = new CustomAxisLabel(collection.GenerateName(), axis.AxisScaleTypeMap.DefaultAxisValue);
			collection.Add(label);
			return new object[] { label };
		}
		protected override void RemoveItem(object item) {
			collection.Remove((CustomAxisLabel)item);
		}
		protected override void Swap(int index1, int index2) {
			collection.Swap(index1, index2);
		}
	}
}
