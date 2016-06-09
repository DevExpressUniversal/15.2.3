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
namespace DevExpress.XtraCharts.Design {
	public abstract class SecondaryAxisCollectionEditorForm : DevExpress.XtraCharts.Design.CollectionEditorForm {
		private System.ComponentModel.IContainer components = null;
		SecondaryAxisCollection collection;
		protected SecondaryAxisCollection Collection { get { return collection; } }
		protected override object[] CollectionToArray { get { return collection.ToArray(); } }
		protected override bool SelectableItems { get { return true; } }
		public SecondaryAxisCollectionEditorForm(IServiceProvider serviceProvider, SecondaryAxisCollection collection) {
			InitializeComponent();
			this.collection = collection;
		}
		protected override void Dispose(bool disposing) {
			if (disposing && components != null)
				components.Dispose();
			base.Dispose(disposing);
		}
		#region Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SecondaryAxisCollectionEditorForm));
			((System.ComponentModel.ISupportInitialize)(this.fListBox)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this, "$this");
			this.Name = "SecondaryAxisCollectionEditorForm";
			((System.ComponentModel.ISupportInitialize)(this.fListBox)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		protected abstract void AddItems(Axis2D axis);
		protected override object[] AddItems() {
			Axis2D axis = CreateSecondaryAxis(collection);
			AddItems(axis);
			return new object[] { axis };
		}
		protected override void RemoveItem(object item) {
			collection.Remove((AxisBase)item);
		}
		protected override void Swap(int index1, int index2) {
			collection.Swap(index1, index2);
		}
		protected abstract Axis2D CreateSecondaryAxis(SecondaryAxisCollection collection);
	}
	public class SecondaryAxisXCollectionEditorForm : SecondaryAxisCollectionEditorForm {
		protected new SecondaryAxisXCollection Collection { get { return (SecondaryAxisXCollection)base.Collection; } }
		public SecondaryAxisXCollectionEditorForm(IServiceProvider serviceProvider, SecondaryAxisXCollection collection) : base(serviceProvider, collection) {
		}
		protected override string GetItemDisplayText(int index) {
			return Collection[index].Name;
		}
		protected override Axis2D CreateSecondaryAxis(SecondaryAxisCollection collection) {
			return new SecondaryAxisX(collection.GenerateName());
		}
		protected override void AddItems(Axis2D axis) {
			Collection.Add((SecondaryAxisX)axis);
		}
	}
	public class SecondaryAxisYCollectionEditorForm : SecondaryAxisCollectionEditorForm {
		protected new SecondaryAxisYCollection Collection { get { return (SecondaryAxisYCollection)base.Collection; } }
		public SecondaryAxisYCollectionEditorForm(IServiceProvider serviceProvider, SecondaryAxisYCollection collection) : base(serviceProvider, collection) {
		}
		protected override string GetItemDisplayText(int index) {
			return Collection[index].Name;
		}
		protected override Axis2D CreateSecondaryAxis(SecondaryAxisCollection collection) {
			return new SecondaryAxisY(collection.GenerateName());
		}
		protected override void AddItems(Axis2D axis) {
			Collection.Add((SecondaryAxisY)axis);
		}
	}
	public class SwiftPlotDiagramSecondaryAxisXCollectionEditorForm : SecondaryAxisCollectionEditorForm {
		protected new SwiftPlotDiagramSecondaryAxisXCollection Collection { get { return (SwiftPlotDiagramSecondaryAxisXCollection)base.Collection; } }
		public SwiftPlotDiagramSecondaryAxisXCollectionEditorForm(IServiceProvider serviceProvider, SwiftPlotDiagramSecondaryAxisXCollection collection) : base(serviceProvider, collection) {
		}
		protected override string GetItemDisplayText(int index) {
			return Collection[index].Name;
		}
		protected override Axis2D CreateSecondaryAxis(SecondaryAxisCollection collection) {
			return new SwiftPlotDiagramSecondaryAxisX(collection.GenerateName());
		}
		protected override void AddItems(Axis2D axis) {
			Collection.Add((SwiftPlotDiagramSecondaryAxisX)axis);
		}
	}
	public class SwiftPlotDiagramSecondaryAxisYCollectionEditorForm : SecondaryAxisCollectionEditorForm {
		protected new SwiftPlotDiagramSecondaryAxisYCollection Collection { get { return (SwiftPlotDiagramSecondaryAxisYCollection)base.Collection; } }
		public SwiftPlotDiagramSecondaryAxisYCollectionEditorForm(IServiceProvider serviceProvider, SwiftPlotDiagramSecondaryAxisYCollection collection) : base(serviceProvider, collection) {
		}
		protected override string GetItemDisplayText(int index) {
			return Collection[index].Name;
		}
		protected override Axis2D CreateSecondaryAxis(SecondaryAxisCollection collection) {
			return new SwiftPlotDiagramSecondaryAxisY(collection.GenerateName());
		}
		protected override void AddItems(Axis2D axis) {
			Collection.Add((SwiftPlotDiagramSecondaryAxisY)axis);
		}
	}
}
