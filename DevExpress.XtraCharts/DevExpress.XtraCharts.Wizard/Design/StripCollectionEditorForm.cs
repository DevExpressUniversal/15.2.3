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

using DevExpress.XtraCharts.Localization;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Design {
	public class StripCollectionEditorForm : CollectionEditorForm {
		readonly StripCollection collection;
		protected override object[] CollectionToArray { get { return collection.ToArray(); } }
		protected override bool SelectableItems { get { return false; } }
		StripCollectionEditorForm() {
			InitializeComponent();
		}
		public StripCollectionEditorForm(StripCollection collection) : this() {
			this.collection = collection;
		}
		#region Designer generated code
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StripCollectionEditorForm));
			((System.ComponentModel.ISupportInitialize)(this.fListBox)).BeginInit();
			this.SuspendLayout();
			this.Appearance.ForeColor = ((System.Drawing.Color)(resources.GetObject("StripCollectionEditorForm.Appearance.ForeColor")));
			this.Appearance.Options.UseForeColor = true;
			resources.ApplyResources(this, "$this");
			this.Name = "StripCollectionEditorForm";
			((System.ComponentModel.ISupportInitialize)(this.fListBox)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		protected override string GetItemDisplayText(int index) {
			return collection[index].Name;
		}
		protected override object[] AddItems() {
			IAxisData axis = ((IOwnedElement)collection).Owner as IAxisData;
			object minLimit = axis.AxisScaleTypeMap.DefaultAxisValue;
			object maxLimit = axis.AxisScaleTypeMap.InternalToNative(1);
			if (minLimit == null)
				minLimit = ChartLocalizer.GetString(ChartStringId.DefaultMinValue);
			if (maxLimit == null)
				maxLimit = ChartLocalizer.GetString(ChartStringId.DefaultMaxValue);
			Strip strip = new Strip(collection.GenerateName(), minLimit, maxLimit);
			collection.Add(strip);
			return new object[] { strip };
		}
		protected override void RemoveItem(object item) {
			collection.Remove((Strip)item);
		}
		protected override void Swap(int index1, int index2) {
			collection.Swap(index1, index2);
		}
	}
}
