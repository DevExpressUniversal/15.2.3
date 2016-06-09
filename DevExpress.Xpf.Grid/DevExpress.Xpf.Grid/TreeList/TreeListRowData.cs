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
using System.Linq;
using System.Text;
using System.Windows;
using DevExpress.Xpf.Utils;
using System.ComponentModel;
using DevExpress.Xpf.Grid.Native;
using System.Windows.Media;
using DevExpress.Xpf.Editors;
#if SILVERLIGHT
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using DevExpress.Data.Browsing;
#endif
namespace DevExpress.Xpf.Grid.TreeList {
	public class TreeListRowData : RowData {
		public static readonly DependencyProperty IsExpandedProperty;
		static readonly DependencyPropertyKey IsExpandedPropertyKey;
		public static readonly DependencyProperty IsButtonVisibleProperty;
		static readonly DependencyPropertyKey IsButtonVisiblePropertyKey;
		public static readonly DependencyProperty IsImageVisibleProperty;
		static readonly DependencyPropertyKey IsImageVisiblePropertyKey;
		public static readonly DependencyProperty IsCheckBoxVisibleProperty;
		static readonly DependencyPropertyKey IsCheckBoxVisiblePropertyKey;
		public static readonly DependencyProperty RowMarginProperty;
		static readonly DependencyPropertyKey RowMarginPropertyKey;
		public static readonly DependencyProperty IndentsProperty;
		static readonly DependencyPropertyKey IndentsPropertyKey;
		public static readonly DependencyProperty RowLevelProperty;
		static readonly DependencyPropertyKey RowLevelPropertyKey;
		static TreeListRowData() {
			Type ownerType = typeof(TreeListRowData);
			IsExpandedPropertyKey = DependencyPropertyManager.RegisterReadOnly("IsExpanded", typeof(bool), ownerType, new PropertyMetadata(false));
			IsExpandedProperty = IsExpandedPropertyKey.DependencyProperty;
			IsButtonVisiblePropertyKey = DependencyPropertyManager.RegisterReadOnly("IsButtonVisible", typeof(bool), ownerType, new PropertyMetadata(false));
			IsButtonVisibleProperty = IsButtonVisiblePropertyKey.DependencyProperty;
			IsImageVisiblePropertyKey = DependencyPropertyManager.RegisterReadOnly("IsImageVisible", typeof(bool), ownerType, new PropertyMetadata(false));
			IsImageVisibleProperty = IsImageVisiblePropertyKey.DependencyProperty;
			IsCheckBoxVisiblePropertyKey = DependencyPropertyManager.RegisterReadOnly("IsCheckBoxVisible", typeof(bool), ownerType, new PropertyMetadata(false));
			IsCheckBoxVisibleProperty = IsCheckBoxVisiblePropertyKey.DependencyProperty;
			IndentsPropertyKey = DependencyPropertyManager.RegisterReadOnly("Indents", typeof(List<TreeListIndentType>), ownerType, new PropertyMetadata(null));
			IndentsProperty = IndentsPropertyKey.DependencyProperty;
			RowMarginPropertyKey = DependencyPropertyManager.RegisterReadOnly("RowMargin", typeof(Thickness), ownerType, new PropertyMetadata(new Thickness()));
			RowMarginProperty = RowMarginPropertyKey.DependencyProperty;
			RowLevelPropertyKey = DependencyPropertyManager.RegisterReadOnly("RowLevel", typeof(int), ownerType, new PropertyMetadata(0));
			RowLevelProperty = RowLevelPropertyKey.DependencyProperty;
		}
		public TreeListRowData(DataTreeBuilder treeBuilder)
			: base(treeBuilder) {
		}
		public bool IsButtonVisible {
			get { return (bool)GetValue(IsButtonVisibleProperty); }
			private set { this.SetValue(IsButtonVisiblePropertyKey, value); }
		}
		public bool IsImageVisible {
			get { return (bool)GetValue(IsImageVisibleProperty); }
			private set { this.SetValue(IsImageVisiblePropertyKey, value); }
		}
		public bool IsCheckBoxVisible {
			get { return (bool)GetValue(IsCheckBoxVisibleProperty); }
			private set { this.SetValue(IsCheckBoxVisiblePropertyKey, value); }
		}
		public bool IsExpanded {
			get { return (bool)GetValue(IsExpandedProperty); }
			private set { this.SetValue(IsExpandedPropertyKey, value); }
		}
		public Thickness RowMargin {
			get { return (Thickness)GetValue(RowMarginProperty); }
			private set { this.SetValue(RowMarginPropertyKey, value); }
		}
		public List<TreeListIndentType> Indents {
			get { return (List<TreeListIndentType>)GetValue(IndentsProperty); }
			private set { this.SetValue(IndentsPropertyKey, value); }
		}
		public int RowLevel {
			get { return (int)GetValue(RowLevelProperty); }
			internal set { this.SetValue(RowLevelPropertyKey, value); }
		}
		public TreeListNode Node { get { return View.TreeListDataProvider.GetNodeByRowHandle(RowHandle.Value); } }
		ImageSource image;
		public ImageSource Image {
			get { return image; }
			set {
				if(Image == value) return;
				image = value;
				NotifyPropertyChanged("Image");
			}
		}
		bool? isChecked;
		public bool? IsChecked {
			get { return isChecked; }
			set {
				Node.UpdateNodeChecked(value);
				if(IsChecked == value) return;
				isChecked = value;
				NotifyPropertyChanged("IsChecked");
			}
		}
		bool isCheckBoxEnabled;
		public bool IsCheckBoxEnabled {
			get { return isCheckBoxEnabled; }
			set {
				if(IsCheckBoxEnabled == value)
					return;
				isCheckBoxEnabled = value;
				NotifyPropertyChanged("IsCheckBoxEnabled");
			}
		}
		protected new TreeListView View { get { return (TreeListView)base.View; } }
		protected internal override void UpdateData() {
			base.UpdateData();
			if(Row != null) {
				IsButtonVisible = CanShowExpandButton();
				UpdateNodeImage();
				IsChecked = Node.IsChecked;
				IsCheckBoxEnabled = Node.IsCheckBoxEnabled;
			}
			UpdateCellDataEditorsDisplayText();
		}
		protected virtual void UpdateNodeImage() {
			Image = GetImageSource();
		}
		protected override void AssignFromCore(NodeContainer nodeContainer, RowNode rowNode, bool forceUpdate) {
			base.AssignFromCore(nodeContainer, rowNode, forceUpdate);
			UpdateRowState();
		}
		protected void UpdateRowState() {
			NextRowLevel = int.MinValue;
			IsExpanded = Node.IsExpanded;
			IsButtonVisible = CanShowExpandButton();
			IsImageVisible = View.ShowNodeImages;
			IsCheckBoxVisible = View.ShowCheckboxes;
			RowLevel = Level;
			RowMargin = new Thickness(Level * View.RowIndent, 0, 0, 0);
			Indents = CalcNodeIndents();
		}
		protected internal override void UpdateIsSelected(bool forceIsSelected) {
			base.UpdateIsSelected(forceIsSelected);
			UpdateNodeImage();
		}
		protected virtual bool CanShowExpandButton() {
			if(!View.ShowRootIndent && Node.ActualLevel == 0) return false;
			return View.ShowExpandButtons && Node.IsTogglable && Node.IsExpandButtonVisible != DevExpress.Utils.DefaultBoolean.False;
		}
		protected internal ImageSource GetImageSource() {
			if(View.NodeImageSelector != null) {
				ImageSource imgSource = View.NodeImageSelector.CanSelect(this) ? View.NodeImageSelector.Select(this) : null;
				if(imgSource != null)
					return imgSource;
			}
			if(!string.IsNullOrEmpty(View.ImageFieldName)) {
				try {
					object value = View.GetNodeValue(Node, View.ImageFieldName);
					if(value == null && Node != null && Node.Content != null)
						value = TryGetValueInternal(Node.Content, View.ImageFieldName);
					if(value != null) {
						ImageSource imageSource = value as ImageSource;
						if(imageSource != null)
							return imageSource;
						byte[] bytes = value as byte[];
						if(bytes != null)
							return new BytesToImageSourceConverter().Convert(bytes, null, null, null) as ImageSource; 
					}
				} catch {
					return null;
				}
			}
			return Node == null ? null : Node.Image;
		}
		object TryGetValueInternal(object obj, string propertyName) {
			PropertyDescriptor prop = TypeDescriptor.GetProperties(obj)[propertyName];
			if(prop == null) return null;
			return prop.GetValue(obj);
		}
		protected internal override bool GetShowBottomLine() {
			return false;
		}
		protected virtual List<TreeListIndentType> CalcNodeIndents() {
			List<TreeListIndentType> list = new List<TreeListIndentType>();
			TreeListUtils.CalcNodeIndents(Node, list, View.ShowRootIndent);
			return list;
		}
		protected override void OnControllerVisibleIndexChanged() {
			if(RowHandle == null) return;
			EvenRow = (treeBuilder.GetRowVisibleIndexByHandleCore(RowHandle.Value) % 2) == 0;
		}
		protected internal override double GetRowIndent(bool isFirstColumn) {
			if(isFirstColumn)
				return GetLeftIndent();
			return base.GetRowIndent(isFirstColumn);
		}
		protected internal override double GetRowLeftMargin(GridColumnData data) {
			if(data.VisibleIndex != data.Column.VisibleIndex && data.VisibleIndex == 0 && View.FixedLeftVisibleColumns.Count == 0 && data.Column.Fixed == FixedStyle.None)
				return GetLeftIndent();
			return 0d;
		}
		double GetLeftIndent() {
			return (View.TreeListDataProvider.MaxVisibleLevel - (Node != null ? Node.ActualLevel : Level)) * View.RowIndent;
		}
	}
	public static class TreeListUtils{
		public static void CalcNodeIndents(TreeListNode node, List<TreeListIndentType> indents, bool showRootIndent) {
			indents.Clear();
			int level = node.ActualLevel;
			int min = showRootIndent ? 0 : 1;
			TreeListIndentType indent;
			TreeListNode parent = node.VisibleParent;
			for (int i = level; i >= min; i--) {
				if (i == level) {
					if (level == 0 && node.IsFirstVisible && node.IsLastVisible)
						indent = TreeListIndentType.Root;
					else if (level == 0 && node.IsFirstVisible)
						indent = TreeListIndentType.First;
					else if (node.IsLastVisible)
						indent = TreeListIndentType.Last;
					else
						indent = TreeListIndentType.Middle;
				} else {
					indent = parent.IsLastVisible ? TreeListIndentType.None : TreeListIndentType.Line;
					parent = parent.VisibleParent;
				}
				indents.Add(indent);
			}
			indents.Reverse();
		}
	}
}
