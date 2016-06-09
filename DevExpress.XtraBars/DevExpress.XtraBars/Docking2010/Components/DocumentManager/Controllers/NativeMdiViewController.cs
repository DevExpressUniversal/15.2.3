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

using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.XtraBars.Docking2010.Customization;
using System;
using System.Drawing;
namespace DevExpress.XtraBars.Docking2010.Views.NativeMdi {
	class NativeMdiViewController : BaseViewController, INativeMdiViewController {
		public NativeMdiViewController(NativeMdiView view)
			: base(view) {
		}
		public new NativeMdiView View {
			get { return base.View as NativeMdiView; }
		}
		protected override bool DockCore(BaseDocument baseDocument) {
			Document document = baseDocument as Document;
			if(document == null) return false;
			if(AddDocumentCore(document)) {
				Manager.InvokePatchActiveChildren();
				return true;
			}
			return false;
		}
		protected override bool AddDocumentCore(BaseDocument document) {
			return true;
		}
		protected override bool RemoveDocumentCore(BaseDocument document) {
			return true;
		}
		protected override void PatchControlBeforeAdd(Control control) {
		}
		protected override void PatchControlAfterRemove(Control control) {
		}
		public bool Cascade() {
			MdiClient mdiClient = DevExpress.Utils.Mdi.MdiClientSubclasser.GetMdiClient(Manager.MdiParent);
			Rectangle[] rects;
			ArrangeIcons();
			if(mdiClient != null && Manager.IsMdiStrategyInUse) {
				mdiClient.LayoutMdi(MdiLayout.Cascade);
				IList<Form> normalStateMdiChildren = GetNormalStateMdiChildren(mdiClient);
				int topArrageMdiChild = GetTopArrageMdiChild(mdiClient);
				Rectangle bounds = View.Bounds;
				bounds.Height = topArrageMdiChild;
				rects = MdiLayoutHelper.CascadeLayout(normalStateMdiChildren.Count, bounds);
				for(int i = 0; i < rects.Length; i++) {
					normalStateMdiChildren[i].Bounds = rects[i];
				}
				return true;
			}
			else {
				List<BaseDocument> visibleDocuments = GetVisibleDocuments();
				visibleDocuments.Reverse();
				rects = MdiLayoutHelper.CascadeLayout(visibleDocuments.Count, View.Bounds);
				for(int i = 0; i < rects.Length; i++) {
					DocumentContainer container = visibleDocuments[i].ContainerControl;
					if(container != null) {
						container.Bounds = rects[i];
						container.BringToFront();
					}
				}
				return true;
			}
		}
		public bool TileVertical() {
			ArrangeIcons();
			MdiClient mdiClient = DevExpress.Utils.Mdi.MdiClientSubclasser.GetMdiClient(Manager.MdiParent);
			if(mdiClient != null && Manager.IsMdiStrategyInUse)
				return ArrangeMdiChildren(mdiClient, false);
			else
				return ArrangeVisibleDocuments(false);
		}
		public bool TileHorizontal() {
			ArrangeIcons();
			MdiClient mdiClient = DevExpress.Utils.Mdi.MdiClientSubclasser.GetMdiClient(Manager.MdiParent);
			if(mdiClient != null && Manager.IsMdiStrategyInUse)
				return ArrangeMdiChildren(mdiClient, true);
			else
				return ArrangeVisibleDocuments(true);
		}
		protected bool ArrangeVisibleDocuments(bool isHorizontal) {
			Rectangle[] rects;
			int columnCount = 0;
			List<BaseDocument> visibleDocuments = GetVisibleDocuments();
			if(visibleDocuments.Count == 0) return false;
			columnCount = GetColumnCount(visibleDocuments.Count, isHorizontal);
			rects = MdiLayoutHelper.TileLayout(visibleDocuments.Count, columnCount, View.Bounds);
			for(int i = 0; i < rects.Length; i++) {
				visibleDocuments[i].Control.Parent.Bounds = rects[i];
			}
			return true;
		}
		protected bool ArrangeMdiChildren(MdiClient mdiClient, bool isHorizontal) {
			Rectangle[] rects;
			int columnCount = 0;
			IList<Form> normalStateMdiChildren = GetNormalStateMdiChildren(mdiClient);
			int topArrageMdiChild = GetTopArrageMdiChild(mdiClient);
			int childrenCount = normalStateMdiChildren.Count;
			if(childrenCount == 0) return false;
			columnCount = GetColumnCount(childrenCount, isHorizontal);
			Rectangle bounds = View.Bounds;
			bounds.Height = topArrageMdiChild;
			rects = MdiLayoutHelper.TileLayout(childrenCount, columnCount, bounds);
			for(int i = 0; i < rects.Length; i++) {
				normalStateMdiChildren[i].Bounds = rects[i];
			}
			return true;
		}
		protected int GetColumnCount(int childrenCount, bool isHorizontal) {
			if(isHorizontal)
				return (int)Math.Sqrt(childrenCount);
			return childrenCount / (int)Math.Sqrt(childrenCount);
		}
		int GetTopArrageMdiChild(MdiClient mdiClient) {
			int result = View.Bounds.Height;
			if(mdiClient == null) return result;
			for(int i = 0; i < mdiClient.MdiChildren.Length; i++) {
				if(mdiClient.MdiChildren[i].WindowState == FormWindowState.Minimized)
					result = Math.Min(result, mdiClient.MdiChildren[i].Top);
			}
			return result;
		}
		IList<Form> GetNormalStateMdiChildren(MdiClient mdiClient) {
			IList<Form> result = new List<Form>();
			if(mdiClient == null) return result;
			for(int i = 0; i < mdiClient.MdiChildren.Length; i++) {
				if(mdiClient.MdiChildren[i].WindowState != FormWindowState.Minimized)
					result.Add(mdiClient.MdiChildren[i]);
			}
			return result;
		}
		List<BaseDocument> GetVisibleDocuments() {
			List<BaseDocument> result = new List<BaseDocument>();
			foreach(BaseDocument document in Manager.ActivationInfo.DocumentActivationList) {
				if(document.IsVisible && document.Control != null && document.Control.Parent != null)
					result.Add(document);
			}
			return result;
		}
		public bool ArrangeIcons() {
			MdiClient mdiClient = DevExpress.Utils.Mdi.MdiClientSubclasser.GetMdiClient(Manager.MdiParent);
			if(mdiClient != null && mdiClient.IsHandleCreated) {
				mdiClient.LayoutMdi(MdiLayout.ArrangeIcons);
				return true;
			}
			return false;
		}
		public bool MinimizeAll() {
			MdiClient mdiClient = DevExpress.Utils.Mdi.MdiClientSubclasser.GetMdiClient(Manager.MdiParent);
			if(mdiClient != null) {
				Form[] children = mdiClient.MdiChildren;
				for(int i = 0; i < children.Length; i++)
					children[i].WindowState = FormWindowState.Minimized;
			}
			return false;
		}
		public bool RestoreAll() {
			MdiClient mdiClient = DevExpress.Utils.Mdi.MdiClientSubclasser.GetMdiClient(Manager.MdiParent);
			if(mdiClient != null) {
				Form[] children = mdiClient.MdiChildren;
				for(int i = 0; i < children.Length; i++)
					children[i].WindowState = FormWindowState.Normal;
			}
			return false;
		}
		protected override BaseViewControllerMenu CreateContextMenu() {
			return new NativeMdiViewControllerMenu(this);
		}
		protected override Control CalculatePlacementTarget(BaseDocument document) {
			return Manager.GetChild(document);
		}
		protected override void ResetLayoutCore() {
			Cascade();
		}
		protected override void GetCommandsCore(BaseDocument document, IList<BaseViewControllerCommand> commands) {
			commands.Add(BaseViewControllerCommand.Close);
			if(View.Documents.Count + View.FloatDocuments.Count > 1)
				commands.Add(BaseViewControllerCommand.CloseAllButThis);
		}
		protected override void GetCommandsCore(IList<BaseViewControllerCommand> commands) {
			base.GetCommandsCore(commands);
			if(!View.AllChildrenAreIcons()) {
				commands.Add(NativeMdiViewControllerCommand.Cascade);
				commands.Add(NativeMdiViewControllerCommand.TileHorizontal);
				commands.Add(NativeMdiViewControllerCommand.TileVertical);
				if(Manager.IsMdiStrategyInUse)
					commands.Add(NativeMdiViewControllerCommand.MinimizeAll);
			}
			else commands.Add(NativeMdiViewControllerCommand.RestoreAll);
			if(View.HasIconicChildren())
				commands.Add(NativeMdiViewControllerCommand.ArrangeIcons);
		}
	}
}
