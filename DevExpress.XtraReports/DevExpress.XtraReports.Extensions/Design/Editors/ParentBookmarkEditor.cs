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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Utils;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Design.Tools;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.UI;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Native;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList.Nodes.Operations;
namespace DevExpress.XtraReports.Design {
	class ParentBookmarkEditor : UITypeEditor {
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return (context != null && context.Instance != null) ? UITypeEditorEditStyle.DropDown : base.GetEditStyle(context);
		}
		public override bool IsDropDownResizable {
			get { return true; }
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			IWindowsFormsEditorService editServ = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
			XRControl control = context.Instance as XRControl;
			if(provider != null && editServ != null && control != null) {
				using(ParentBookmarkController treeViewController = new ParentBookmarkController(provider)) {
					CancelEventHandler eventHandler = (s, e) => {
						if(treeViewController.EditValue != null && !treeViewController.EditValue.IsNavigateTarget)
							e.Cancel = true;
					};
					treeViewController.EditValueChanging += eventHandler;
					XRControlTreeView treeView = new XRControlTreeView();
					try {
						treeViewController.CaptureTreeList(treeView);
						treeViewController.Start(context, editServ, control.BookmarkParent);
						editServ.DropDownControl(treeView);
						return treeViewController.EditValue;
					} finally {
						treeViewController.EditValueChanging -= eventHandler;
						treeView.Dispose();
					}
				}
			}
			return value;
		}
	}
	class ParentBookmarkController : XRControlTreeViewController {
		public ParentBookmarkController(IServiceProvider servProvider)
			: base(servProvider) {
		}
		protected override void FilterComponents(ArrayList items) {
			base.FilterComponents(items);
			for(int i = items.Count - 1; i >= 0; i--) {
				XRControl control = (XRControl)items[i];
				if(ReferenceEquals(control, context.Instance) && !ControlsContainNavigateTarget(control.Controls)) {
					items.RemoveAt(i);
					continue;
				}
				if(ReferenceEquals(control, context.Instance) && ControlsContainNavigateTarget(control.Controls)) {
					items.AddRange(control.Controls);
					items.RemoveAt(i);
					continue;
				}
				if(!control.HasChildren && !control.IsNavigateTarget) {
					items.RemoveAt(i);
					continue;
				}
				if(control.HasChildren && !control.IsNavigateTarget && !ControlsContainNavigateTarget(control.Controls)) {
					items.RemoveAt(i);
					continue;
				}
			}
		}
		static bool ControlsContainNavigateTarget(IList controls) {
			foreach(XRControl control in controls) {
				if(control.IsNavigateTarget || ControlsContainNavigateTarget(control.Controls))
					return true;
			}
			return false;
		}
	}
	abstract class DrillDownControlEditorBase : UITypeEditor {
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return (context != null && context.Instance != null) ? UITypeEditorEditStyle.DropDown : base.GetEditStyle(context);
		}
		public override bool IsDropDownResizable {
			get { return true; }
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			IWindowsFormsEditorService editServ = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
			XRControl control = GetRootControl(context.Instance as XRControl);
			if(provider != null && editServ != null && control != null) {
				using(XRControlTreeViewController treeViewController = new XRControlTreeViewController(provider, control)) {
					CancelEventHandler eventHandler = (s, e) => {
						if(ReferenceEquals(control, treeViewController.EditValue))
							e.Cancel = true;
					};
					treeViewController.EditValueChanging += eventHandler;
					XRControlTreeView treeView = new XRControlTreeView();
					try {
						treeViewController.CaptureTreeList(treeView);
						treeViewController.Start(context, editServ, value as XRControl);
						editServ.DropDownControl(treeView);
						return treeViewController.EditValue;
					} finally {
						treeViewController.EditValueChanging -= eventHandler;
						treeView.Dispose();
					}
				}
			}
			return value;
		}
		protected abstract XRControl GetRootControl(XRControl control);
	}
	class DrillDownBandEditor : DrillDownControlEditorBase {
		protected override XRControl GetRootControl(XRControl control) {
			return control is Band ? ((Band)control).GetPreviousBand() as GroupHeaderBand : null;
		}
	}
	class DrillDownReportEditor : DrillDownControlEditorBase {
		protected override XRControl GetRootControl(XRControl control) {
			return control.Report.Bands[BandKind.Detail];
		}
	}
	class XRControlTreeViewController : ReportExplorerControllerBase {
		#region inner classes
		class NameValueComparer : IComparer {
			NaturalStringComparer stringComparer;
			public NameValueComparer() {
				stringComparer = new NaturalStringComparer();
			}
			public int Compare(object x, object y) {
				return stringComparer.Compare(((XRControl)x).Name, ((XRControl)y).Name);
			}
		}
		#endregion
		protected IWindowsFormsEditorService editServ;
		protected ITypeDescriptorContext context;
		XRControl rootControl;
		public XRControl EditValue {
			get;
			private set;
		}
		public event CancelEventHandler EditValueChanging;
		void RaiseEditValueChanging(CancelEventArgs args) {
			if(EditValueChanging != null) EditValueChanging(this, args);
		}
		public XRControlTreeViewController(IServiceProvider servProvider)
			: base(servProvider) {
		}
		public XRControlTreeViewController(IServiceProvider servProvider, XRControl rootControl)
			: base(servProvider) {
			this.rootControl = rootControl;
		}
		public void Start(ITypeDescriptorContext context, IWindowsFormsEditorService editServ, XRControl editValue) {
			this.context = context;
			this.editServ = editServ;
			UpdateView();
			EditValue = editValue;
			if(editValue != null)
				SetSelectedNode(editValue);
		}
		protected override XRControl RootControl {
			get {
				return rootControl ?? base.RootControl;
			}
		}
		public override void SubscribeTreeListEvents(TreeList treeList) {
			base.SubscribeTreeListEvents(treeList);
			if(treeList != null) {
				treeList.MouseDoubleClick += new MouseEventHandler(OnDoubleClick);
				treeList.KeyDown += new KeyEventHandler(KeyDown);
			}
		}
		public override void UnsubscribeTreeListEvents(TreeList treeList) {
			base.UnsubscribeTreeListEvents(treeList);
			if(treeList != null) {
				treeList.MouseDoubleClick -= new MouseEventHandler(OnDoubleClick);
				treeList.KeyDown -= new KeyEventHandler(KeyDown);
			}
		}
		XRControl SelectedControl {
			get {
				return ((XRControlNode)ReportTreeView.SelectedNode).Component as XRControl;
			}	
		}
		void OnDoubleClick(object sender, MouseEventArgs e) {
			if(TrySetEditValue(SelectedControl))
				editServ.CloseDropDown();
		}
		void KeyDown(object sender, KeyEventArgs e) {
			if(e.KeyCode == Keys.Enter && TrySetEditValue(SelectedControl))
				editServ.CloseDropDown();
		}
		protected bool TrySetEditValue(XRControl value) {
			var saveValue = EditValue;
			EditValue = value;
			var args = new CancelEventArgs(false);
			RaiseEditValueChanging(args);
			if(args.Cancel) {
				EditValue = saveValue;
				return false;
			}
			return true;
		}
		protected override void UpdateView() {
			base.UpdateView();
			XRControlNode noneNode = new XRControlNode(null, ReportTreeView.Nodes, GetImageIndexCore) { Text = ReportStringId.UD_Title_ReportExplorer_NullControl.GetString() };
			((IList)ReportTreeView.Nodes).Add(noneNode);
			((XtraListNode)ReportTreeView.Nodes[0]).Expand();
			foreach(XtraListNode node in ReportTreeView.Nodes[0].Nodes)
				node.Expand();
		}
		protected override IComparer GetComparer() {
			return new NameValueComparer();
		}
	}
	public class XRControlTreeView : XtraTreeView, ISupportController {
		public XRControlTreeView() {
			this.StateImageList = ReportExplorerController.ImageCollection;
			Size = new System.Drawing.Size(Math.Max(Width, 10), Math.Max(Height, 10));
			this.OptionsSelection.EnableAppearanceFocusedCell = false;
			this.OptionsDragAndDrop.DragNodesMode = XtraTreeList.DragNodesMode.Single;
			this.OptionsBehavior.AllowIncrementalSearch = true;
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				ToolTipController.DefaultController.RemoveClientControl(this);
				lock(this) {
					if(this.StateImageList != null)
						this.StateImageList = null;
				}
			}
			base.Dispose(disposing);
		}
		#region ISupportController Members
		TreeListController ISupportController.ActiveController {
			get; 
			set;
		}
		TreeListController ISupportController.CreateController(IServiceProvider serviceProvider) {
			throw new NotImplementedException();
		}
		#endregion
	}
}
