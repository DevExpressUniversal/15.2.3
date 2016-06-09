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
using System.Drawing;
using System.Linq;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using DevExpress.Data;
using DevExpress.Utils.Menu;
using DevExpress.Data.Utils;
using DevExpress.XtraLayout;
using DevExpress.XtraEditors;
using DevExpress.Utils.Design;
using DevExpress.Utils.Drawing;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Repository;
using System.Reflection;
namespace DevExpress.XtraDataLayout {
	[StructLayout(LayoutKind.Sequential)]
	public struct ScrollInfoStruct {
		public int cbSize;
		public int fMask;
		public int nMin;
		public int nMax;
		public int nPage;
		public int nPos;
		public int nTrackPos;
	}
	public delegate void ScrollEventHandler(object sender, ScrollEventArgs e);
	public class ScrollEventArgs :EventArgs {
		private ScrollInfoStruct scrollInfoCore;
		public ScrollInfoStruct ScrollInfo {
			get {
				return scrollInfoCore;
			}
			set {
				scrollInfoCore = value;
			}
		}
	}
	[System.Security.SecuritySafeCritical]
	public class ScrollNotificationTreeViewNative {
		[DllImport("user32.dll", SetLastError = true)]
		internal static extern int GetScrollInfo(IntPtr hWnd, int n,
			ref ScrollInfoStruct lpScrollInfo);
		[DllImport("user32.dll")]
		internal static extern int SetScrollInfo(int hWnd, int n,
			[MarshalAs(UnmanagedType.Struct)] ref ScrollInfoStruct lpcScrollInfo,
			bool b);
	}
	public class ScrollNotificationTreeView :DXTreeView {
		protected const int SBM_SETSCROLLINFO = 0x00E9;
		protected const int WM_VSCROLL = 0x115;
		protected const int WM_MOUSEWHEEL = 0x20A;
		protected const int SB_VERT = 1;
		protected const int SB_THUMBTRACK = 5;
		protected const int SIF_TRACKPOS = 0x10;
		protected const int SIF_RANGE = 0x1;
		protected const int SIF_POS = 0x4;
		protected const int SIF_PAGE = 0x2;
		protected const int SIF_ALL = SIF_RANGE | SIF_PAGE | SIF_POS | SIF_TRACKPOS;
		[System.Security.SecuritySafeCritical]
		void RaiseScrollEvent(IntPtr handle) {
			ScrollInfoStruct si = new ScrollInfoStruct();
			si.fMask = SIF_ALL;
			si.cbSize = Marshal.SizeOf(si);
			ScrollNotificationTreeViewNative.GetScrollInfo(handle, SB_VERT, ref si);
			ScrollEventArgs e = new ScrollEventArgs();
			e.ScrollInfo = si;
			OnScroll(e);
		}
		protected virtual void OnScroll(ScrollEventArgs e) { }
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			RaiseScrollEvent(this.Handle);
		}
		protected override void WndProc(ref Message m) {
			base.WndProc(ref m);
			if(m.Msg == WM_VSCROLL ||
				 m.Msg == SBM_SETSCROLLINFO ||
				 m.Msg == WM_MOUSEWHEEL) {
				RaiseScrollEvent(m.HWnd);
			}
		}
	}
	public class DataSourceStructureView :ScrollNotificationTreeView {
		RepositoryItemComboBox repositoryItem;
		ButtonEditViewInfo viewInfo;
		BaseEditPainter painter;
		LayoutElementsBindingInfo info;
		BindingMenuManager bindingManager;
		public DataSourceStructureView() {
			DrawMode = TreeViewDrawMode.OwnerDrawText;
			ComboBoxEdit cbe = new ComboBoxEdit();
			ItemHeight = cbe.CalcMinHeight();
			cbe.Dispose();
			repositoryItem = new RepositoryItemComboBox();
			viewInfo = (ButtonEditViewInfo)repositoryItem.CreateViewInfo();
			painter = repositoryItem.CreatePainter();
		}
		public DataSourceStructureView(LayoutElementsBindingInfo info)
			: this() {
			this.info = info;
		}
		public void EnsureBindingManager(BindingMenuManager menuManager) {
			bindingManager = menuManager;
		}
		public void EnsureInfo(LayoutElementsBindingInfo info) {
			this.info = info;
		}
		public void UpdateTreeNode(DataSourceNode node) {
			List<LayoutElementBindingInfo> bindings = info.GetAllBindings();
			BeginUpdate();
			UpdateTreeNodeCore(node, bindings);
			EndUpdate();
		}
		private void UpdateTreeNodeCore(DataSourceNode node, List<LayoutElementBindingInfo> bindings) {
			foreach(LayoutElementBindingInfo bi in bindings) {
				if(bi.InnerLayoutElementsBindingInfo != null) {
					UpdateTreeNodeCore(node, bi.InnerLayoutElementsBindingInfo.GetAllBindings());
					continue;
				}
				if(node.BindingInfo.DataMember == bi.DataMember) {
					node.BindingInfo = bi;
					node.Text = GetNodeText(bi);
				}
			}
		}
		protected virtual string GetNodeText(LayoutElementBindingInfo bi) {
			if(bi.DataInfo.Caption != bi.DataMember)
				return bi.DataInfo.Caption + "(" + bi.DataMember + ")";
			else
				return bi.DataInfo.Caption;
		}
		public void UpdateView() {
			List<LayoutElementBindingInfo> bindings = info.GetAllBindings();
			BeginUpdate();
			Nodes.Clear();
			UpdateViewCore(bindings);
			EndUpdate();
		}
		private void UpdateViewCore(List<LayoutElementBindingInfo> bindings) {
			foreach(LayoutElementBindingInfo bi in bindings) {
				if(bi.InnerLayoutElementsBindingInfo != null) {
					UpdateViewCore(bi.InnerLayoutElementsBindingInfo.GetAllBindings());
					continue;
				}
				DataSourceNode node = new DataSourceNode();
				node.Text = GetNodeText(bi);
				node.BindingInfo = bi;
				Nodes.Add(node);
			}
		}
		void ShowAddControlMenu(DataSourceNode node) {
			DXPopupMenu controlsMenu = CreateMenu(node);
			Point p = Point.Empty;
			p.Y = node.Bounds.Bottom;
			p.X = node.ComboButtonRect.X;
			MenuManagerHelper.ShowMenu(controlsMenu, ((DevExpress.LookAndFeel.ISupportLookAndFeel)Parent).LookAndFeel.ActiveLookAndFeel, null, this, p);
		}
		protected int GetCenter(int largeWigth, int largeX, int smallWidth) {
			return largeX + Math.Abs((largeWigth - smallWidth) / 2);
		}
		Point DrawString(DrawTreeNodeEventArgs e, Point rightTop, string text) {
			Rectangle rect = Rectangle.Empty;
			using(GraphicsCache cache = new GraphicsCache(e.Graphics)) {
				SizeF size = viewInfo.Appearance.CalcTextSize(cache, text, 1000);
				rect = new Rectangle(rightTop.X, GetCenter(viewInfo.Appearance.CalcDefaultTextSize(cache.Graphics).Height, rightTop.Y, (int)size.Height), (int)size.Width, (int)size.Height);
				viewInfo.Appearance.DrawString(cache, text, rect);
				((DataSourceNode)e.Node).ComboButtonRect = Rectangle.Empty;
			}
			return new Point(rect.Right, rightTop.Y);
		}
		Point DrawComboBox(DrawTreeNodeEventArgs e, Point rightTop, string text) {
			SizeF size;
			using(GraphicsCache cache = new GraphicsCache(e.Graphics)) {
				size = viewInfo.Appearance.CalcTextSize(cache, text, 1000);
			}
			Rectangle rect = new Rectangle(
				rightTop.X,
				GetCenter(e.Bounds.Height, rightTop.Y, e.Bounds.Height),
				(int)size.Width + 25,
				e.Bounds.Height
				);
			Rectangle comboButtonRect = Rectangle.Empty;
			viewInfo.Bounds = rect;
			viewInfo.EditValue = text;
			viewInfo.CalcViewInfo(e.Graphics);
			viewInfo.TextUseFullBounds = true;
			comboButtonRect = viewInfo.RightButtons[0].Bounds;
			ControlGraphicsInfoArgs info = new ControlGraphicsInfoArgs(viewInfo, new GraphicsCache(e.Graphics), rect);
			painter.Draw(info);
			((DataSourceNode)e.Node).ComboButtonRect = comboButtonRect;
			return new Point(rect.Right, rightTop.Y);
		}
		void ClearDrawRect(Rectangle rect, DrawTreeNodeEventArgs e) {
			rect.Width += 10;
			e.Graphics.FillRectangle(Brushes.White, rect);
		}
		Point DrawVisibleCheck(Point rightTop, DrawTreeNodeEventArgs e, bool visible) {
			Rectangle rect = new Rectangle(rightTop.X, GetCenter(e.Bounds.Height, rightTop.Y, 16), 16, 16);
			using(GraphicsCache cache = new GraphicsCache(e.Graphics)) {
				ButtonState bState = visible ? ButtonState.Checked : ButtonState.Normal;
				cache.Paint.DrawCheckBox(e.Graphics, rect, bState);
				((DataSourceNode)e.Node).CheckBoxRect = rect;
			}
			return new Point(rect.Right, rightTop.Y);
		}
		Point DrawFieldName(Point rightTop, DrawTreeNodeEventArgs e, string text) {
			Rectangle stringRect = Rectangle.Empty;
			using(GraphicsCache cache = new GraphicsCache(e.Graphics)) {
				SizeF size = viewInfo.Appearance.CalcTextSize(cache, text, 1000);
				stringRect = new Rectangle(rightTop.X, GetCenter(e.Bounds.Height, rightTop.Y, (int)size.Height), (int)size.Width, (int)size.Height);
				viewInfo.Appearance.DrawString(cache, text, stringRect);
				((DataSourceNode)e.Node).FieldRect = stringRect;
			}
			return new Point(stringRect.Right, rightTop.Y);
		}
		Point DrawIcon(Point rightTop, DrawTreeNodeEventArgs e) {
			Rectangle iconRect = new Rectangle(rightTop.X, GetCenter(e.Bounds.Height, rightTop.Y, 16), 16, 16);
			((DataSourceNode)e.Node).IconRect = iconRect;
			e.Graphics.DrawImage(IconsHlper.BindingIcon, iconRect.Location);
			return new Point(iconRect.Right, rightTop.Y);
		}
		Point DrawEditorProperty(Point rightTop, DrawTreeNodeEventArgs e, string text, bool selected) {
			if(selected)
				return DrawComboBox(e, rightTop, text);
			else
				return DrawString(e, rightTop, text);
		}
		protected bool GetVisibleStatus(LayoutElementBindingInfo bi) {
			return bi.Visible;
		}
		void DrawNodeCore(DrawTreeNodeEventArgs e, bool selected) {
			DataSourceNode node = (DataSourceNode)e.Node;
			Point rightTop = e.Bounds.Location;
			ClearDrawRect(e.Bounds, e);
			rightTop = DrawVisibleCheck(rightTop, e, GetVisibleStatus(node.BindingInfo));
			rightTop = DrawFieldName(rightTop, e, node.Text);
			rightTop = DrawIcon(rightTop, e);
			rightTop = DrawEditorProperty(rightTop, e, node.BindingInfo.EditorType.Name + "." + node.BindingInfo.BoundPropertyName, selected);
		}
		void DrawSelectedNode(DrawTreeNodeEventArgs e) {
			DrawNodeCore(e, true);
		}
		void DrawNodeNormal(DrawTreeNodeEventArgs e) {
			DrawNodeCore(e, false);
		}
		protected override void OnDrawNode(DrawTreeNodeEventArgs e) {
			base.OnDrawNode(e);
			if(e.Node == SelectedNode)
				DrawSelectedNode(e);
			else
				DrawNodeNormal(e);
		}
		protected override void OnScroll(ScrollEventArgs e) {
			if(SelectedNode == null) return;
			Invalidate(SelectedNode.Bounds);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			if(((e.Button == MouseButtons.Left) || (e.Button == MouseButtons.Right))) {
				DataSourceNode nodeAt = GetNodeAt(e.Location) as DataSourceNode;
				Point p = e.Location;
				if(nodeAt != null && SelectedNode == nodeAt && nodeAt.IsInComboButtonArea(p)) {
					ShowAddControlMenu(nodeAt);
				}
				if(nodeAt != null && SelectedNode == nodeAt && nodeAt.IsInCheckArea(p)) {
					nodeAt.BindingInfo.Visible = !nodeAt.BindingInfo.Visible;
					Invalidate(nodeAt.Bounds);
				}
			}
		}
		protected DXPopupMenu CreateMenu(DataSourceNode node) {
			return bindingManager.CreateMenu(node, info);
		}
	}
	public class DataSourceNode :TreeNode {
		Rectangle checkBoxRect, comboButtonRect, fieldRect, editorPropertyRect, iconRect;
		LayoutElementBindingInfo bindingInfo;
		public LayoutElementBindingInfo BindingInfo {
			get { return bindingInfo; }
			set { bindingInfo = value; }
		}
		public Rectangle ComboButtonRect {
			get { return comboButtonRect; }
			set { comboButtonRect = value; }
		}
		public Rectangle CheckBoxRect {
			get { return checkBoxRect; }
			set { checkBoxRect = value; }
		}
		public Rectangle FieldRect {
			get { return fieldRect; }
			set { fieldRect = value; }
		}
		public Rectangle EditorPropertyRect {
			get { return editorPropertyRect; }
			set { editorPropertyRect = value; }
		}
		public Rectangle IconRect {
			get { return iconRect; }
			set { iconRect = value; }
		}
		public bool IsInComboButtonArea(Point p) {
			return comboButtonRect.Contains(p);
		}
		public bool IsInCheckArea(Point p) {
			return CheckBoxRect.Contains(p);
		}
	}
	public class LayoutCreator {
		protected DataLayoutControl dataLayoutControl = null;
		public LayoutCreator(DataLayoutControl dataLayoutControl) {
			this.dataLayoutControl = dataLayoutControl;
		}
		public virtual void CreateLayout(LayoutElementsBindingInfo bi, bool shouldUseFlow = false, RetrieveFieldsParameters parameters = null) {
			List<LayoutElementBindingInfo> list = bi.GetAllBindings();
			if(list.Any(e => e.InnerLayoutElementsBindingInfo != null)) shouldUseFlow = false;
			if(dataLayoutControl.Root != null) dataLayoutControl.Root.StartChange();
			dataLayoutControl.BeginUpdate();
			EnsureExistingPlacementConfiguration();
			for(int i = 0; i < list.Count; i++) {
				LayoutElementBindingInfo bindingInfo = list[i];
				if(parameters != null && !ShouldGenerateField(parameters, bindingInfo)) continue;
				if(bindingInfo.InnerLayoutElementsBindingInfo != null) {
					CreateInnerLayoutGroup(parameters, bindingInfo);
					continue;
				} else EnsurePlacementRoot(shouldUseFlow, bi);
				if(parameters != null) {
					bindingInfo.DataSourceUpdateMode = parameters.DataSourceUpdateMode;
					bindingInfo.DataSourceNullValue = parameters.DataSourceNullValue;
					bindingInfo.CustomControl = TryGetCustomControl(parameters, bindingInfo);
					bindingInfo.GroupName = TryGetCustomGroupName(parameters, bindingInfo);
				}
				bindingInfo = dataLayoutControl.RaiseFieldRetrieving(list[i]);
				Binding binding = null;
				LayoutControlItem lci = GetLayoutItemByBindingInfo(bindingInfo, out binding);
				if(lci != null) {
					Control control = lci.Control;
					if(bindingInfo.EditorType != control.GetType()) {
						Control newControl = CreateBindableControl(bindingInfo);
						lci.BeginInit();
						lci.Control = newControl;
						lci.EndInit();
						try {
							control.Dispose();
						} catch {
						} finally {
						}
						CheckItemVisibility(lci, bindingInfo);
						continue;
					}
					if(bindingInfo.BoundPropertyName != binding.PropertyName) {
						control.DataBindings.Clear();
						Binding bindingToSet = GetBinding(control, bindingInfo);
						control.DataBindings.Add(bindingToSet);
						CheckItemVisibility(lci, bindingInfo);
						continue;
					}
					CheckItemVisibility(lci, bindingInfo);
				} else {
					if(((ILayoutControl)dataLayoutControl).DesignMode && !bindingInfo.Visible) {
					} else
						lci = CreateLayoutElement(bindingInfo, bi.ColumnCount, shouldUseFlow);
				}
				if(lci != null) dataLayoutControl.RaiseFieldRetrieved(lci.Control, lci, bindingInfo.DataMember, lci.Control is BaseEdit ? (lci.Control as BaseEdit).Properties : null);
			}
			if(placementRoot != null && shouldUseFlow) {
				SetGroupCell(bi);
			}
			dataLayoutControl.EndUpdate();
			if(dataLayoutControl.Root != null) dataLayoutControl.Root.EndChange();
		}
		private string TryGetCustomGroupName(RetrieveFieldsParameters parameters, LayoutElementBindingInfo bindingInfo) {
			foreach(var item in parameters.CustomListParameters) {
				if(item.FieldName == bindingInfo.DataMember &&
					string.IsNullOrEmpty(bindingInfo.AnnotationAttributes.GroupName) &&
					item.CreateTabGroupForItem) {
					string groupName = AnnotationAttributes.GetColumnCaption(bindingInfo.AnnotationAttributes);
					if(string.IsNullOrEmpty(groupName)) groupName = item.FieldName;
					return string.Format("{{Tabs}}/{0}", groupName);
				}
			}
			return bindingInfo.AnnotationAttributes.GroupName;
		}
		bool ShouldGenerateField(RetrieveFieldsParameters parameters, LayoutElementBindingInfo bindingInfo) {
			foreach(var item in parameters.CustomListParameters) {
				if(item.FieldName == bindingInfo.DataMember) return item.GenerateField;
			}
			return true;
		}
		Control TryGetCustomControl(RetrieveFieldsParameters parameters, LayoutElementBindingInfo bindingInfo) {
			foreach(var item in parameters.CustomListParameters) {
				if(item.FieldName == bindingInfo.DataMember) return item.ControlForField;
			}
			return null;
		}
		void CreateInnerLayoutGroup(RetrieveFieldsParameters parameters, LayoutElementBindingInfo bindingInfo) {
			if(bindingInfo.InnerLayoutElementsBindingInfo.bindingsInfo == null || bindingInfo.InnerLayoutElementsBindingInfo.bindingsInfo.Count == 0) return;
			EnsurePlacementRoot(false,null);
			LayoutGroup parentGroup = placementRoot;
			string parentName = placementRoot.Name;
			placementRoot.Name = "parent" + placementRoot.Name;
			if(!String.IsNullOrEmpty(bindingInfo.GroupName)) {
				AnnotationAttributesGroupName groupNameHelper = new AnnotationAttributesGroupName(bindingInfo.GroupName);
				LayoutGroup group = groupNameHelper.GenerateGroupIfNeeded(placementRoot);
				placementRoot = group;
				string tooltip = AnnotationAttributes.GetColumnDescription(bindingInfo.AnnotationAttributes);
				if(!string.IsNullOrEmpty(tooltip)) {
					group.OptionsToolTip.ToolTip = tooltip;
				}
				string displayName = AnnotationAttributes.GetColumnCaption(bindingInfo.AnnotationAttributes);
				if(!string.IsNullOrEmpty(displayName)) {
					group.Text = displayName;
				}
			} else {
				placementRoot = placementRoot.AddGroup();
				placementRoot.Name = "innerAutoGeneratedGroup" + groupCounter;
				placementRoot.Text = bindingInfo.DataInfo.Name;
				groupCounter++;
			}
			CreateLayout(bindingInfo.InnerLayoutElementsBindingInfo, bindingInfo.InnerLayoutElementsBindingInfo.ColumnCount > 1, parameters);
			placementRoot = parentGroup;
			placementRoot.Name = parentName;
		}
		void SetGroupCell(LayoutElementsBindingInfo bi) {
			int width = placementRoot.CellSize.Width * 3;
			placementRoot.Width = placementRoot.CellSize.Width * bi.ColumnCount * 3 + 50;
			for(int i = 0; i < placementRoot.ItemCount; i++) {
				if(placementRoot[i].StartNewLine)
					placementRoot[i].Size = new Size(placementRoot.Width, placementRoot[i].Height);
				else
					placementRoot[i].Size = new Size(width, placementRoot[i].Height);
			}
			placementRoot.UpdateFlowLayoutItems(true);
			placementRoot.LayoutMode = LayoutMode.Regular;
		}
		protected void CheckItemVisibility(LayoutControlItem lci, LayoutElementBindingInfo elementBi) {
			if(lci.IsHidden && elementBi.Visible) lci.RestoreFromCustomization();
			if((!lci.IsHidden) && (!elementBi.Visible)) lci.HideToCustomization();
		}
		protected virtual LayoutControlItem GetLayoutItemByBindingInfo(LayoutElementBindingInfo info, out Binding outBinding) {
			foreach(BaseLayoutItem item in dataLayoutControl.Items) {
				LayoutControlItem lci = item as LayoutControlItem;
				if(lci != null && lci.Control != null) {
					Control control = lci.Control;
					foreach(Binding binding in control.DataBindings) {
						if(binding.DataSource == dataLayoutControl.DataSource) {
							if(info.DataMember == binding.BindingMemberInfo.BindingMember) {
								outBinding = binding;
								return lci;
							}
						}
					}
				}
			}
			outBinding = null;
			return null;
		}
		protected virtual Control CreateBindableControlRunTime(LayoutElementBindingInfo elementBi) {
			if(elementBi.EditorType.IsSubclassOf(typeof(BaseEdit))) {
				RepositoryItem repository = dataLayoutControl.GetRepositoryItem(elementBi);
				BaseEdit baseEdit = Activator.CreateInstance(elementBi.EditorType) as BaseEdit;
				baseEdit.Properties.Assign(repository);
				SetControlProperties(baseEdit, elementBi);
				return baseEdit;
			} else {
				Control customControl = Activator.CreateInstance(elementBi.EditorType) as Control;
				SetControlProperties(customControl, elementBi);
				return customControl;
			}
		}
		protected string GetControlName(Control control, LayoutElementBindingInfo elementBi) {
			Type type = control.GetType();
			string typeName = type.ToString();
			string typeSName = typeName.Substring(typeName.LastIndexOf(".") + 1);
			return elementBi.DataMember + typeSName;
		}
		protected void SetControlProperties(Control control, LayoutElementBindingInfo elementBi) {
			if(control == null) throw new LayoutControlInternalException("Internal error:cant create instance of" + elementBi.EditorType);
			Binding binding = GetBinding(control, elementBi);
			control.DataBindings.Add(binding);
			control.Name = GetControlName(control, elementBi);
			if(control.Site != null) {
				try {
					control.Site.Name = control.Name;
				} catch { } finally { }
			}
			control.Bounds = Rectangle.Empty;
			control.Visible = false;
			dataLayoutControl.ControlsManager.PerformTypeSpecificActions(control, elementBi);
		}
		private Binding GetBinding(Control control, LayoutElementBindingInfo elementBi) {
			string datamember = MakeDataMemberFullName(dataLayoutControl.DataMember, elementBi.DataMember);
			Binding binding = new Binding(elementBi.BoundPropertyName, dataLayoutControl.DataSource, datamember, true, elementBi.DataSourceUpdateMode);
			binding.DataSourceNullValue = elementBi.DataSourceNullValue;
			if(!elementBi.DataInfo.IsDataViewDescriptor) {
				var propType = elementBi.DataInfo.PropertyDescriptor.PropertyType;
				if(IsNullable(propType)) {
					TextEdit editor = control as TextEdit;
					if(editor != null) {
						binding.DataSourceNullValue = null;
						editor.Properties.AllowNullInput = Utils.DefaultBoolean.True;
					}
				}
			}
			return binding;
		}
		static bool IsNullable(Type type) {
			return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
		}
		static string MakeDataMemberFullName(string dataMember, string name) {
			return dataMember + (String.IsNullOrEmpty(dataMember) ? String.Empty : ".") + name;
		}
		protected virtual Control CreateBindableControl(LayoutElementBindingInfo elementBi) {
			if(dataLayoutControl == null) return null;
			if(elementBi.CustomControl != null) {
				if(dataLayoutControl.Site != null) {
					AddControlToDataLayoutControlInDesignMode(elementBi.CustomControl);   
				}
				return elementBi.CustomControl;
			}
			if(dataLayoutControl.Site != null)
				return CreateBindableControlDesignTime(elementBi);
			else
				return CreateBindableControlRunTime(elementBi);
		}
		public static IDesignerHost GetIDesignerHost(IContainer container) {
			if(container is IDesignerHost) return container as IDesignerHost;
			if(container is NestedContainer) return ((NestedContainer)container).Owner.Site.Container as IDesignerHost;
			return null;
		}
		protected virtual Control CreateBindableControlDesignTime(LayoutElementBindingInfo elementBi) {
			if(elementBi.EditorType == null) throw new LayoutControlInternalException("Internal error:wrong binding info");
			if(elementBi.EditorType.IsSubclassOf(typeof(BaseEdit))) {
				RepositoryItem repositoryItem = dataLayoutControl.GetRepositoryItem(elementBi);
				BaseEdit baseEdit = GetIDesignerHost(dataLayoutControl.Site.Container).CreateComponent(elementBi.EditorType) as BaseEdit;
				baseEdit.Properties.Assign(repositoryItem);
				AddControlToDataLayoutControlInDesignMode(baseEdit);
				SetControlProperties(baseEdit, elementBi);
				return baseEdit;
			} else {
				Control customControl = GetIDesignerHost(dataLayoutControl.Site.Container).CreateComponent(elementBi.EditorType) as Control;
				AddControlToDataLayoutControlInDesignMode(customControl);
				SetControlProperties(customControl, elementBi);
				return customControl;
			}
		}
		private void AddControlToDataLayoutControlInDesignMode(Control customControl) {
			dataLayoutControl.Site.Container.Add(customControl);
			IComponentInitializer initializer = LayoutCreator.GetIDesignerHost(dataLayoutControl.Site.Container).GetDesigner(customControl) as IComponentInitializer;
			if(initializer != null) {
				bool flag = true;
				try {
					initializer.InitializeNewComponent(new Hashtable());
					flag = false;
				} finally {
					if(flag) {
						LayoutCreator.GetIDesignerHost(dataLayoutControl.Site.Container).DestroyComponent(customControl);
					}
				}
			}
		}
		protected virtual LayoutControlItem CreateLayoutItem(LayoutElementBindingInfo elementBi, Control control, DataLayoutControl dataLayout) {
			LayoutControlItem item = dataLayout.CreateLayoutItem(null) as LayoutControlItem;
			item.Owner = dataLayout;
			dataLayout.BeginInit();
			item.Control = control;
			item.Owner = null;
			item.Text = elementBi.DataInfo.Caption;
			item.Name = "ItemFor" + elementBi.DataMember;
			DevExpress.XtraLayout.Helpers.DesignTimeHelper.Default.AddElementToDTSurface(item, dataLayout.Site);
			try {
				if(item.Site != null) item.Site.Name = item.Name;
			} catch { }
			dataLayout.EndInit();
			return item;
		}
		protected virtual void PlaceItemIntoLayout(LayoutControlItem item, LayoutElementBindingInfo bindingInfo, bool shouldUseFlow) {
			if(placementRoot == null) EnsurePlacementRoot(shouldUseFlow,null);
			PlaceItemIntoLayoutByAnnotationAttributes(item, bindingInfo);
			if(item.MaxSize == Size.Empty) item.StartNewLine = true;
		}
		private void PlaceItemIntoLayoutByAnnotationAttributes(LayoutControlItem item, LayoutElementBindingInfo bindingInfo) {
			string caption = AnnotationAttributes.GetColumnCaption(bindingInfo.AnnotationAttributes);
			item.itemAnnotationAttributes = bindingInfo.AnnotationAttributes;
			if(caption != null) {
				if(caption == string.Empty) item.TextVisible = false;
				else item.Text = "";
			}
			if(bindingInfo.GroupName != bindingInfo.AnnotationAttributes.GroupName) item.TextVisible = false;
			string tooltip = AnnotationAttributes.GetColumnDescription(bindingInfo.AnnotationAttributes);
			if(!string.IsNullOrEmpty(tooltip)) {
				item.OptionsToolTip.ToolTip = tooltip;
			}
			if((bindingInfo.AnnotationAttributes == null && bindingInfo.GroupName == null) || placementRoot.LayoutMode == LayoutMode.Flow) {
				placementRoot.AddItem(item);
				return;
			}
			if(!String.IsNullOrEmpty(bindingInfo.GroupName)) {
				AnnotationAttributesGroupName groupNameHelper = new AnnotationAttributesGroupName(bindingInfo.GroupName);
				LayoutGroup group = groupNameHelper.GenerateGroupIfNeeded(placementRoot);
				if(group == null) placementRoot.AddItem(item);
				else group.AddItem(item);
				return;
			}
			placementRoot.AddItem(item);
		}
		int currentGroupCapacity = 0;
		int groupCounter;
		LayoutGroup placementRoot = null;
		protected virtual int GetCapacityThreshold() { return 200; }
		protected virtual LayoutGroup GetPlacementRoot() {
			return placementRoot;
		}
		protected void EnsurePlacementRoot(bool shouldUseFlow, LayoutElementsBindingInfo bi) {
			if(++currentGroupCapacity > GetCapacityThreshold()) {
				if(placementRoot != null && shouldUseFlow && placementRoot.LayoutMode == LayoutMode.Flow && bi != null) {
					SetGroupCell(bi);
				}
				currentGroupCapacity = 0;
				placementRoot = null;
			}
			if(placementRoot == null) {
				placementRoot = dataLayoutControl.Root.AddGroup();
				placementRoot.Name = "autoGeneratedGroup" + groupCounter;
				if(shouldUseFlow) placementRoot.LayoutMode = LayoutMode.Flow;
				placementRoot.GroupBordersVisible = false;
				placementRoot.AllowDrawBackground = false;
				EnsureCellSize(placementRoot);
				groupCounter++;
			}
		}
		protected void EnsureCellSize(LayoutGroup group) {
			if(group.CellSize.Width == 0 || group.CellSize.Height == 0) {
				placementRoot.CellSize = new Size(100, 20);
			}
		}
		protected void EnsureExistingPlacementConfiguration() {
			foreach(BaseLayoutItem item in dataLayoutControl.Items) {
				LayoutControlItem cItem = item as LayoutControlItem;
				if(placementRoot != null && cItem != null)
					currentGroupCapacity++;
				LayoutGroup group = item as LayoutGroup;
				if(group != null && group.Name.StartsWith("autoGeneratedGroup")) {
					groupCounter++;
					placementRoot = group;
					EnsureCellSize(placementRoot);
					currentGroupCapacity = 0;
				}
			}
		}
		protected virtual void AddItemToHiddenList(LayoutControlItem lci) {
			dataLayoutControl.AddToHiddenItems(lci);
		}
		protected virtual LayoutControlItem CreateLayoutElement(LayoutElementBindingInfo bindingInfo, int CellCount, bool shouldUseFlow) {
			Control control = CreateBindableControl(bindingInfo);
			LayoutControlItem layoutItem = CreateLayoutItem(bindingInfo, control, dataLayoutControl);
			if(bindingInfo.Visible)
				PlaceItemIntoLayout(layoutItem, bindingInfo, shouldUseFlow);
			else
				AddItemToHiddenList(layoutItem);
			return layoutItem;
		}
	}
	public class Walker {
		public void Walkthrough(object parent) {
			ICollection children = GetChilden(parent);
			foreach(object child in children) {
				DoAction(parent, child);
				Walkthrough(child);
			}
		}
		protected virtual ICollection GetChilden(object parent) { return null; }
		protected virtual void DoAction(object parent, object target) { }
	}
	public class MenuItemBindingInfo {
		Type controlType;
		String controlProperty;
		DataSourceNode dataSourceNode;
		public MenuItemBindingInfo(Type controlType, String controlProperty, DataSourceNode dataSourceNode) {
			this.controlType = controlType;
			this.controlProperty = controlProperty;
			this.dataSourceNode = dataSourceNode;
		}
		public Type EditorType { get { return controlType; } }
		public String EditorProperty { get { return controlProperty; } }
		public DataSourceNode DataSourceNode { get { return dataSourceNode; } }
	}
	public class LayoutElementBindingInfo {
		DataColumnInfo dataColumanInfo;
		Type editorType;
		string boundPropertyName;
		bool visible;
		public LayoutElementBindingInfo(DataColumnInfo dataColumanInfo) : this(dataColumanInfo, typeof(TextEdit)) { }
		public LayoutElementBindingInfo(DataColumnInfo dataColumanInfo, Type editorType) : this(dataColumanInfo, editorType, "", true) { }
		public LayoutElementBindingInfo(DataColumnInfo dataColumanInfo, Type editorType, string boundPropertyName, bool visible) : this(dataColumanInfo, editorType, boundPropertyName, visible, DataSourceUpdateMode.OnValidation) { }
		public LayoutElementBindingInfo(DataColumnInfo dataColumanInfo, Type editorType, string boundPropertyName, bool visible, DataSourceUpdateMode dataSourceUpdateMode) {
			this.dataColumanInfo = dataColumanInfo;
			this.editorType = editorType;
			this.boundPropertyName = boundPropertyName;
			this.visible = visible;
			if(dataColumanInfo != null) {
				if(dataColumanInfo.PropertyDescriptor != null) {
					ColumnOptions = AnnotationAttributes.GetColumnOptions(dataColumanInfo.PropertyDescriptor, 0, false);
					AnnotationAttributes = ColumnOptions.Attributes;
					GroupName = AnnotationAttributes.GroupName;
					this.visible = ColumnOptions.ColumnIndex != -1;
				}
				Type dataType = dataColumanInfo.GetDataType();
			}
			DataSourceUpdateMode = dataSourceUpdateMode;
			DataMember = dataColumanInfo.Name;
		}
		internal AnnotationAttributes AnnotationAttributes { get; set; }
		internal AnnotationAttributes.ColumnOptions ColumnOptions { get; set; }
		public bool Visible { get { return visible; } set { visible = value; } }
		public DataColumnInfo DataInfo { get { return dataColumanInfo; } }
		public Type EditorType { get { return editorType; } set { editorType = value; } }
		public string BoundPropertyName { get { return boundPropertyName; } set { boundPropertyName = value; } }
		public DataSourceUpdateMode DataSourceUpdateMode { get; set; }
		public object DataSourceNullValue { get; set; }
		public string DataMember { get; internal set; }
		public LayoutElementsBindingInfo InnerLayoutElementsBindingInfo { get; internal set; }
		#region RetrieveFieldsParameters
		internal Control CustomControl { get; set; }
		internal string GroupName { get; set; }
		#endregion
	}
	public class LayoutElementsBindingInfo {
		internal List<LayoutElementBindingInfo> bindingsInfo;
		public LayoutElementsBindingInfo(List<LayoutElementBindingInfo> bindingsInfo) {
			this.bindingsInfo = bindingsInfo;
			bindingsInfo.Sort(new AnnotationAttributesOrderComparer(bindingsInfo));
		}
		public List<LayoutElementBindingInfo> GetDataFieldBindings(DataColumnInfo dataColumanInfo) {
			List<LayoutElementBindingInfo> result = new List<LayoutElementBindingInfo>();
			GetDataFieldBindingsCore(dataColumanInfo, result);
			return result;
		}
		private void GetDataFieldBindingsCore(DataColumnInfo dataColumanInfo, List<LayoutElementBindingInfo> result) {
			foreach(LayoutElementBindingInfo info in bindingsInfo) {
				if(info.InnerLayoutElementsBindingInfo != null) {
					info.InnerLayoutElementsBindingInfo.GetDataFieldBindingsCore(dataColumanInfo, result);
					continue;
				}
				if(info.DataInfo == dataColumanInfo)
					result.Add(info);
			}
		}
		public List<LayoutElementBindingInfo> GetAllBindings() {
			return bindingsInfo;
		}
		int columnCountCore = 1;
		public int ColumnCount { get { return columnCountCore; } set { if(value > 0) columnCountCore = value; } }
	}
	public class AnnotationAttributesOrderComparer :IComparer<LayoutElementBindingInfo> {
		LayoutElementBindingInfo[] bindingInfo;
		public AnnotationAttributesOrderComparer(List<LayoutElementBindingInfo> listLayoutElementBindingInfo) {
			bindingInfo = new LayoutElementBindingInfo[listLayoutElementBindingInfo.Count];
			listLayoutElementBindingInfo.CopyTo(bindingInfo);
		}
		public int Compare(LayoutElementBindingInfo x, LayoutElementBindingInfo y) {
			if(x != null && y != null && x.ColumnOptions != null && y.ColumnOptions != null) {
				if(x.AnnotationAttributes.Order.HasValue && y.AnnotationAttributes.Order.HasValue) return x.AnnotationAttributes.Order.GetValueOrDefault(int.MaxValue) - y.AnnotationAttributes.Order.GetValueOrDefault(int.MaxValue);
				if(!x.AnnotationAttributes.Order.HasValue && y.AnnotationAttributes.Order.HasValue) return 1;
				if(x.AnnotationAttributes.Order.HasValue && !y.AnnotationAttributes.Order.HasValue) return -1;
			}
			return Array.IndexOf(bindingInfo, x) - Array.IndexOf(bindingInfo, y);
		}
	}
	public class AnnotationAttributesGroupName {
		const char StartTabbedGroupChar = '{';
		const char EndTabbedGroupChar = '}';
		const char StartGroupBorderVisibleFalse = '<';
		const char EndGroupBorderVisibleFalse = '>';
		const char StartGroupBorderVisibleTrue = '[';
		const char EndGroupBorderVisibleTrue = ']';
		const char HorizontalGroupMark = '-';
		const char VerticalGroupMark = '|';
		const char GroupPathSeparator = '/';
		const string BeginNameForGroup = "autoGroupFor";
		public AnnotationAttributesGroupName(string GroupName) {
			int counterOfTabbedGroupChars = 0;
			int counterOfGroupBorderVisibleFalse = 0;
			int counterOfGroupBorderVisibleTrue = 0;
			for(int i = 0; i < GroupName.Length; i++) {
				switch(GroupName[i]) {
					case AnnotationAttributesGroupName.GroupPathSeparator:
						childGroupCore = new AnnotationAttributesGroupName(GroupName.Remove(0, i + 1));
						return;
					case AnnotationAttributesGroupName.StartGroupBorderVisibleFalse:
						groupBordersVisible = false;
						counterOfGroupBorderVisibleFalse++;
						if(counterOfGroupBorderVisibleFalse > 1) groupNameCore += GroupName[i];
						break;
					case AnnotationAttributesGroupName.EndGroupBorderVisibleFalse:
						counterOfGroupBorderVisibleFalse--;
						if(counterOfGroupBorderVisibleFalse > 1) groupNameCore += GroupName[i];
						break;
					case AnnotationAttributesGroupName.StartGroupBorderVisibleTrue:
						groupBordersVisible = true;
						counterOfGroupBorderVisibleTrue++;
						if(counterOfGroupBorderVisibleTrue > 1) groupNameCore += GroupName[i];
						break;
					case AnnotationAttributesGroupName.EndGroupBorderVisibleTrue:
						counterOfGroupBorderVisibleTrue--;
						if(counterOfGroupBorderVisibleTrue > 1) groupNameCore += GroupName[i];
						break;
					case AnnotationAttributesGroupName.StartTabbedGroupChar:
						isTab = true;
						counterOfTabbedGroupChars++;
						if(counterOfTabbedGroupChars > 1) groupNameCore += GroupName[i];
						break;
					case AnnotationAttributesGroupName.EndTabbedGroupChar:
						counterOfTabbedGroupChars--;
						if(counterOfTabbedGroupChars > 1) groupNameCore += GroupName[i];
						break;
					case AnnotationAttributesGroupName.HorizontalGroupMark:
						layoutType = LayoutType.Horizontal;
						break;
					case AnnotationAttributesGroupName.VerticalGroupMark:
						layoutType = LayoutType.Vertical;
						break;
					default:
						groupNameCore += GroupName[i];
						break;
				}
			}
		}
		string groupNameCore = string.Empty;
		public string GroupName { get { return groupNameCore; } }
		AnnotationAttributesGroupName childGroupCore;
		public AnnotationAttributesGroupName ChildGroup { get { return childGroupCore; } }
		LayoutType layoutType = LayoutType.Vertical;
		public LayoutType LayoutType { get { return layoutType; } }
		bool isTab = false;
		public bool IsTab { get { return isTab; } }
		bool groupBordersVisible = true;
		public bool GroupBordersVisible { get { return groupBordersVisible; } }
		internal LayoutGroup GenerateGroupIfNeeded(LayoutItemContainer placementRoot) {
			TabbedGroup rootTgroup = placementRoot as TabbedGroup;
			LayoutGroup rootLgroup = placementRoot as LayoutGroup;
			if(rootTgroup != null) {
				return GenerateGroupIfNeededCore(rootTgroup);
			}
			return GenerateGroupIfNeededCore(rootLgroup);
		}
		private LayoutGroup GenerateGroupIfNeededCore(LayoutGroup rootLgroup) {
			BaseLayoutItem item = rootLgroup.Items.FindByName(BeginNameForGroup + GroupName);
			if(item == null) {
				if(!IsTab) {
					LayoutGroup group = rootLgroup.AddGroup();
					group.Name = BeginNameForGroup + GroupName;
					group.Text = GroupName;
					group.DefaultLayoutType = LayoutType;
					group.GroupBordersVisible = GroupBordersVisible;
					if(ChildGroup != null) return ChildGroup.GenerateGroupIfNeededCore(group);
					return group;
				} else {
					TabbedGroup group = rootLgroup.AddTabbedGroup();
					group.Name = BeginNameForGroup + GroupName;
					group.Text = GroupName;
					if(ChildGroup != null) return ChildGroup.GenerateGroupIfNeeded(group);
					if(group.TabPages.Count == 0) group.AddTabPage();
					group.SelectedTabPageIndex = 0;
					return group.TabPages[0];
				}
			} else {
				TabbedGroup tGroup = item as TabbedGroup;
				LayoutGroup lGroup = item as LayoutGroup;
				if(ChildGroup == null) {
					if(tGroup != null) {
						if(tGroup.TabPages.Count == 0) tGroup.AddTabPage();
						return tGroup.TabPages[0];
					}
					if(lGroup != null) return lGroup;
				} else {
					if(tGroup != null) return ChildGroup.GenerateGroupIfNeeded(tGroup);
					if(lGroup != null) return ChildGroup.GenerateGroupIfNeeded(lGroup);
				}
				return null;
			}
		}
		private LayoutGroup GenerateGroupIfNeededCore(TabbedGroup rootTgroup) {
			BaseLayoutItem item = rootTgroup.TabPages.FindByName(BeginNameForGroup + GroupName);
			if(item == null) {
				if(!IsTab) {
					LayoutGroup group = rootTgroup.AddTabPage();
					group.Name = BeginNameForGroup + GroupName;
					group.Text = GroupName;
					group.DefaultLayoutType = LayoutType;
					group.GroupBordersVisible = GroupBordersVisible;
					rootTgroup.SelectedTabPageIndex = 0;
					if(ChildGroup != null) return ChildGroup.GenerateGroupIfNeededCore(group);
					return group;
				} else {
					TabbedGroup group = rootTgroup.TabPages[0].AddTabbedGroup();
					group.Name = BeginNameForGroup + GroupName;
					group.Text = GroupName;
					if(ChildGroup != null) return ChildGroup.GenerateGroupIfNeeded(group);
					group.SelectedTabPageIndex = 0;
					return group.TabPages[0];
				}
			} else {
				TabbedGroup tGroup = item as TabbedGroup;
				LayoutGroup lGroup = item as LayoutGroup;
				if(ChildGroup == null) {
					if(tGroup != null) return tGroup.TabPages[0];
					if(lGroup != null) return lGroup;
				} else {
					if(tGroup != null) return ChildGroup.GenerateGroupIfNeeded(tGroup);
					if(lGroup != null) return ChildGroup.GenerateGroupIfNeeded(lGroup);
				}
				return null;
			}
		}
	}
	public class LayoutElementsBindingInfoHelper {
		DataLayoutControl targetLayout;
		public LayoutElementsBindingInfoHelper(DataLayoutControl dataLayout) {
			this.targetLayout = dataLayout;
		}
		public void FillWithSuggestedValues(LayoutElementsBindingInfo info) {
			foreach(LayoutElementBindingInfo bi in info.GetAllBindings()) {
				if(bi.InnerLayoutElementsBindingInfo != null) {
					FillWithSuggestedValues(bi.InnerLayoutElementsBindingInfo);
					continue;
				}
				if(bi.BoundPropertyName == "") {
					RepositoryItem repository = this.targetLayout.GetRepositoryItem(bi);
					if(repository is RepositoryItemMemoExEdit) bi.EditorType = typeof(MemoEdit);
					else bi.EditorType = repository.GetEditorType();
					bi.BoundPropertyName = targetLayout.ControlsManager.GetSuggestedBindingProperty(bi.EditorType);
				}
			}
		}
		public LayoutElementsBindingInfo CreateDataLayoutElementsBindingInfo() {
			ICollection dataColumnInfoCollection = ((DataLayoutControlDesignerMethods)targetLayout).GetFieldsList();
			List<LayoutElementBindingInfo> result = CreateListElementBindingInfoFromFieldList(dataColumnInfoCollection, string.Empty, null,null);
			LayoutElementsBindingInfo resultInfo = new LayoutElementsBindingInfo(result);
			return resultInfo;
		}
		List<LayoutElementBindingInfo> CreateListElementBindingInfoFromFieldList(ICollection dataColumnInfoCollection, string dataMember, Hashtable dataTypeHashtable, DataColumnInfo parentInfo) {
			List<LayoutElementBindingInfo> result = new List<LayoutElementBindingInfo>();
			foreach(DataColumnInfo info in dataColumnInfoCollection) {
				dataTypeHashtable = InitializeHashtableIfNeed(dataTypeHashtable, info);
				if(info.Browsable && CheckInfo(info)) {
					Type dataType = info.GetDataType();
					LayoutElementBindingInfo newBi = new LayoutElementBindingInfo(info);
					newBi.DataMember = GetDataMemberName(dataMember, info);
					result.Add(newBi);
					if(targetLayout.AllowGeneratingNestedGroups != Utils.DefaultBoolean.True || IsSimpleType(dataTypeHashtable, dataType)) continue;
					if(parentInfo != null && !parentInfo.GetDataType().GetProperties().Any(e => e.Name == info.Name)) continue;
					else {
						Hashtable hashTable = new Hashtable();
						if(dataTypeHashtable != null) hashTable = dataTypeHashtable.Clone() as Hashtable;
						hashTable.Add(dataType, null);
						DataColumnInfoCollection childDataColumnInfoCollection = GetDataColumnInfoCollectionFromType(dataType);
						if(CheckDataType(dataType, childDataColumnInfoCollection))
							newBi.InnerLayoutElementsBindingInfo = new LayoutElementsBindingInfo(CreateListElementBindingInfoFromFieldList(childDataColumnInfoCollection, GetDataMemberName(dataMember, info), hashTable, info));
						}
					}
				}
			return result;
			}
		static DataColumnInfoCollection GetDataColumnInfoCollectionFromType(Type type) {
			PropertyDescriptorCollection propertyDescriptorCollection = TypeDescriptor.GetProperties(type);
			DataColumnInfoCollection collection = new DataColumnInfoCollection();
			foreach(PropertyDescriptor propDescriptor in propertyDescriptorCollection) {
				collection.Add(propDescriptor);
			}
			return collection;
		}
		bool CheckDataType(Type dataType, DataColumnInfoCollection columns) {
			if(columns == null || columns.Count <= 0) return false;
			PropertyInfo[] propertyInfo = dataType.GetProperties();
			foreach(DataColumnInfo item in columns) {
				if(!propertyInfo.Any(e => e.Name == item.Name)) return false;
			}
			return true;
		}
		protected virtual Hashtable InitializeHashtableIfNeed(Hashtable table, DataColumnInfo info) {
			if(table == null && info != null && info.PropertyDescriptor != null && info.PropertyDescriptor.ComponentType != null) {
				table = new Hashtable();
				table.Add(info.PropertyDescriptor.ComponentType, null);
			}
			return table;
		}
		protected virtual bool IsSimpleType(Hashtable dataTypeHashtable, Type checkDataType, bool checkProperties = true) {
			try {
				return IsSimpleTypeCore(dataTypeHashtable, checkDataType);
			} catch {
				return true;
			}
		}
		bool IsSimpleTypeCore(Hashtable dataTypeHashtable, Type checkDataType) {
			if(checkDataType == null) return true;
			if(!checkDataType.IsClass) return true;
			if(checkDataType.IsEnum) return true;
			if(checkDataType.IsPrimitive) return true;
			if(checkDataType == typeof(string)) return true;
			if(checkDataType == typeof(DateTime)) return true;
			if(checkDataType == typeof(Byte[])) return true;
			if(checkDataType == typeof(Image)) return true;
			if(checkDataType.IsSubclassOf(typeof(Image))) return true;
			if(dataTypeHashtable != null && dataTypeHashtable.ContainsKey(checkDataType)) return true;
			return false;
		}
		private static string GetDataMemberName(string dataMember, DataColumnInfo info) {
			return string.IsNullOrEmpty(dataMember) ? info.Name : dataMember + "." + info.Name;
		}
		private bool CheckInfo(DataColumnInfo info) {
			if(info.PropertyDescriptor == null) return true;
			AnnotationAttributes attributes = new AnnotationAttributes(info.PropertyDescriptor);
			return attributes.AutoGenerateField.GetValueOrDefault(true);
		}
		static internal LayoutElementBindingInfo GetDataColumnInfoByLayoutElementBindingInfo(BindingMemberInfo bMemberInfo, LayoutElementsBindingInfo info) {
			foreach(LayoutElementBindingInfo bi in info.GetAllBindings()) {
				if(bi.InnerLayoutElementsBindingInfo != null) {
					LayoutElementBindingInfo result = GetDataColumnInfoByLayoutElementBindingInfo(bMemberInfo, bi.InnerLayoutElementsBindingInfo);
					if(result != null)
						return result;
					continue;
				}
				if(bi.DataMember == bMemberInfo.BindingMember) {
					return bi;
				}
			}
			return null;
		}
		public void CorrectLayoutElementsBindingInfo(LayoutElementsBindingInfo info) {
			SetVisibleProperty(info);
			foreach(BaseLayoutItem item in targetLayout.Items) {
				LayoutControlItem lci = item as LayoutControlItem;
				if(lci != null && lci.Control != null) {
					Control control = lci.Control;
					foreach(Binding binding in control.DataBindings) {
						if(binding.DataSource == targetLayout.DataSource) {
							LayoutElementBindingInfo binfo = GetDataColumnInfoByLayoutElementBindingInfo(binding.BindingMemberInfo, info);
							if(binfo != null) {
								binfo.EditorType = control.GetType();
								binfo.BoundPropertyName = binding.PropertyName;
								binfo.Visible = !lci.IsHidden;
							}
						}
					}
				}
			}
		}
		private static void SetVisibleProperty(LayoutElementsBindingInfo info) {
			foreach(var item in info.GetAllBindings()) {
				if(item.InnerLayoutElementsBindingInfo != null) {
					SetVisibleProperty(item.InnerLayoutElementsBindingInfo);
					continue;
				}
				item.Visible = false;
			}
		}
	}
	public class BindingMenuManager {
		ControlsManager manager;
		public BindingMenuManager(ControlsManager cmanager) { manager = cmanager; }
		protected void FillMenuWithSuggestedControlsCore(DXPopupMenu controlsMenu, Type[] suggestedControlTypes, List<LayoutElementBindingInfo> bsettings, DataSourceNode node) {
			bool isFirtst = true;
			for(int i = 0; i < suggestedControlTypes.Length; i++) {
				DXSubMenuItem controlItem = new DXSubMenuItem();
				controlItem.Caption = GetCaptionTypeName(suggestedControlTypes[i]);
				controlItem.Tag = suggestedControlTypes[i];
				if(GetEditorCheckedStatus(suggestedControlTypes[i], bsettings))
					controlItem.Image = IconsHlper.CheckIcon;
				CreateControlPropertiesSubMenu(controlItem, node, suggestedControlTypes[i], bsettings);
				controlsMenu.Items.Add(controlItem);
				if(isFirtst) {
					controlItem.BeginGroup = true;
					isFirtst = false;
				}
			}
		}
		public virtual DXPopupMenu CreateMenu(DataSourceNode node, LayoutElementsBindingInfo bindingInfo) {
			List<LayoutElementBindingInfo> bsettings = bindingInfo.GetDataFieldBindings(node.BindingInfo.DataInfo);
			DXPopupMenu controlsMenu = new DXPopupMenu();
			Type[] suggestedControlTypes = manager.GetSuggestedControls(node.BindingInfo.DataInfo.Type);
			Type[] otherKnownControlTypes = manager.GetOtherControls(suggestedControlTypes);
			DXMenuItem descriptionMenuItem = new DXMenuItem("Choose editor");
			controlsMenu.Items.Add(descriptionMenuItem);
			descriptionMenuItem.Enabled = false;
			FillMenuWithSuggestedControlsCore(controlsMenu, suggestedControlTypes, bsettings, node);
			FillMenuWithSuggestedControlsCore(controlsMenu, otherKnownControlTypes, bsettings, node);
			return controlsMenu;
		}
		protected string GetCaptionTypeName(Type type) {
			String typeName = type.ToString();
			typeName = typeName.Substring(typeName.LastIndexOf(".") + 1);
			return typeName;
		}
		protected virtual void CreateControlPropertiesSubMenu(DXSubMenuItem controlMenuItem, DataSourceNode node, Type editorType, List<LayoutElementBindingInfo> bsettings) {
			try {
				DXSubMenuItem common = new DXSubMenuItem("Common");
				DXSubMenuItem all = new DXSubMenuItem("All");
				string defaultBindingAttribute = null;
				string defaultProperty = null;
				foreach(Attribute attribute in TypeDescriptor.GetAttributes(editorType)) {
					if(attribute is DefaultBindingPropertyAttribute) {
						defaultBindingAttribute = ((DefaultBindingPropertyAttribute)attribute).Name;
						break;
					}
					if(attribute is DefaultPropertyAttribute) {
						defaultProperty = ((DefaultPropertyAttribute)attribute).Name;
					}
				}
				PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(editorType);
				for(int i = 0; i < properties.Count; i++) {
					if(!properties[i].IsReadOnly) {
						BindableAttribute bindable = (BindableAttribute)properties[i].Attributes[typeof(BindableAttribute)];
						BrowsableAttribute browsable = (BrowsableAttribute)properties[i].Attributes[typeof(BrowsableAttribute)];
						if(((browsable == null) || browsable.Browsable) || ((bindable != null) && bindable.Bindable)) {
							DXMenuItem menuItem = new DXMenuItem(properties[i].Name);
							if(GetPropertyCheckedStatus(properties[i].Name, editorType, bsettings))
								menuItem.Image = IconsHlper.CheckIcon;
							menuItem.Click += new EventHandler(menuItem_Click);
							menuItem.Tag = new MenuItemBindingInfo((Type)controlMenuItem.Tag, properties[i].Name, node);
							if((bindable != null) && bindable.Bindable) {
								common.Items.Add(menuItem);
							} else {
								all.Items.Add(menuItem);
							}
						}
					}
				}
				if(all.Items.Count > 0) all.Items[0].BeginGroup = true;
				if(common.Items.Count > 0) common.Items[0].BeginGroup = true;
				DXMenuItem descriptionMenuItem = new DXMenuItem("Choose Bindable Property");
				controlMenuItem.Items.Add(descriptionMenuItem);
				descriptionMenuItem.Enabled = false;
				foreach(DXMenuItem item in common.Items)
					controlMenuItem.Items.Add(item);
				foreach(DXMenuItem item in all.Items)
					controlMenuItem.Items.Add(item);
			} finally {
			}
		}
		protected bool GetEditorCheckedStatus(Type type, List<LayoutElementBindingInfo> bsettings) {
			foreach(LayoutElementBindingInfo bi in bsettings) {
				if(bi.EditorType == type) return true;
			}
			return false;
		}
		protected bool GetPropertyCheckedStatus(String propertyName, Type type, List<LayoutElementBindingInfo> bsettings) {
			foreach(LayoutElementBindingInfo bi in bsettings) {
				if(bi.EditorType == type && bi.BoundPropertyName == propertyName) return true;
			}
			return false;
		}
		void menuItem_Click(object sender, EventArgs e) {
			DXMenuItem menuItem = sender as DXMenuItem;
			if(menuItem == null) return;
			MenuItemBindingInfo mbi = menuItem.Tag as MenuItemBindingInfo;
			if(mbi == null) return;
			mbi.DataSourceNode.BindingInfo.EditorType = mbi.EditorType;
			mbi.DataSourceNode.BindingInfo.BoundPropertyName = mbi.EditorProperty;
			DataSourceStructureView view = mbi.DataSourceNode.TreeView as DataSourceStructureView;
			if(view != null) view.UpdateTreeNode(mbi.DataSourceNode);
		}
	}
	public class ControlsManager {
		List<Type> allSuggestedControls = new List<Type>();
		List<Type> dateAndTimeControls = new List<Type>();
		List<Type> numberEditControls = new List<Type>();
		List<Type> imageControls = new List<Type>();
		List<Type> enumControls = new List<Type>();
		List<Type> boolControls = new List<Type>();
		List<Type> textControls = new List<Type>();
		public ControlsManager() {
			Init();
		}
		public void Init() {
			enumControls.Add(typeof(ImageComboBoxEdit));
			enumControls.Add(typeof(CheckedComboBoxEdit));
			boolControls.Add(typeof(CheckEdit));
			boolControls.Add(typeof(ToggleSwitch));
			imageControls.Add(typeof(PictureEdit));
			imageControls.Add(typeof(ImageEdit));
			numberEditControls.Add(typeof(SpinEdit));
			numberEditControls.Add(typeof(ProgressBarControl));
			numberEditControls.Add(typeof(CalcEdit));
			numberEditControls.Add(typeof(TrackBarControl));
			numberEditControls.Add(typeof(RangeTrackBarControl));
			textControls.Add(typeof(TextEdit));
			textControls.Add(typeof(ButtonEdit));
			textControls.Add(typeof(HyperLinkEdit));
			textControls.Add(typeof(MemoExEdit));
			textControls.Add(typeof(ComboBoxEdit));
			textControls.Add(typeof(LookUpEdit));
			textControls.Add(typeof(MemoEdit));
			dateAndTimeControls.Add(typeof(DateEdit));
			dateAndTimeControls.Add(typeof(TimeEdit));
			allSuggestedControls.AddRange(textControls);
			allSuggestedControls.AddRange(boolControls);
			allSuggestedControls.AddRange(imageControls);
			allSuggestedControls.AddRange(numberEditControls);
			allSuggestedControls.AddRange(dateAndTimeControls);
			allSuggestedControls.Add(typeof(MarqueeProgressBarControl));
			allSuggestedControls.Add(typeof(RadioGroup));
			allSuggestedControls.Add(typeof(ColorEdit));
			allSuggestedControls.Add(typeof(PopupContainerEdit));
		}
		public virtual Type GetSuggestedNavigator() {
			return typeof(DataNavigator);
		}
		public virtual Type[] GetSuggestedControls(Type dataType) {
			if(dataType.IsEnum)
				return enumControls.ToArray();
			if(dataType == typeof(int) ||
				dataType == typeof(Int16) ||
				dataType == typeof(Int32) ||
				dataType == typeof(Int64) ||
				dataType == typeof(float) ||
				dataType == typeof(double) ||
				dataType == typeof(Double) ||
				dataType == typeof(Decimal) ||
				dataType == typeof(long))
				return numberEditControls.ToArray();
			if(dataType == typeof(DateTime) || dataType == typeof(DateTime?))
				return dateAndTimeControls.ToArray();
			if(dataType == typeof(Bitmap) ||
				dataType == typeof(Image) ||
				dataType == typeof(Icon) ||
				dataType == typeof(Byte[]))
				return imageControls.ToArray();
			if(dataType == typeof(bool) || dataType == typeof(Boolean) || dataType == typeof(bool?))
				return boolControls.ToArray();
			if(dataType == typeof(string) || dataType == typeof(String))
				return textControls.ToArray();
			return allSuggestedControls.ToArray();
		}
		public virtual Type[] GetOtherControls(Type[] inControls) {
			List<Type> result = new List<Type>(allSuggestedControls);
			for(int i = 0; i < inControls.Length; i++)
				result.Remove(inControls[i]);
			return result.ToArray();
		}
		public virtual string GetSuggestedBindingProperty(Type editorType) {
			return "EditValue";
		}
		public virtual Type GetSuggestedControl(Type dataType) {
			Type[] suggestedControls = GetSuggestedControls(dataType);
			if(suggestedControls.Length > 0) {
				return suggestedControls[0];
			} else return typeof(TextEdit);
		}
		public virtual void PerformTypeSpecificActions(Control control, LayoutElementBindingInfo elementBi) {
			if(control is CheckEdit) {
				control.Text = elementBi.DataInfo.Caption;
			}
		}
	}
}
