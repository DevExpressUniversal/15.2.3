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
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.Core;
using DevExpress.XtraPivotGrid.Customization;
using System.Collections.Specialized;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
#else
using DevExpress.Xpf.Utils;
#endif
namespace DevExpress.Xpf.PivotGrid.Internal {
	public class TreeViewFieldHeader : FieldHeaderBase {
		bool loaded, inChekedSet, subscribed;
		#region static stuff
		public static readonly DependencyProperty DisplayTextProperty;
		public static readonly DependencyProperty ImageSourceProperty;
		public static readonly DependencyProperty NodeProperty;
		public static readonly DependencyProperty ShowCheckBoxProperty;
		public static readonly DependencyProperty CheckedProperty;
		static TreeViewFieldHeader() {
			Type ownerType = typeof(TreeViewFieldHeader);
			DisplayTextProperty = DependencyPropertyManager.Register("DisplayText", typeof(string), ownerType, new PropertyMetadata(null));
			ImageSourceProperty = DependencyPropertyManager.Register("ImageSource", typeof(ImageSource), ownerType, new PropertyMetadata(null));
			NodeProperty = DependencyPropertyManager.Register("Node", typeof(IVisualCustomizationTreeItem), ownerType, new PropertyMetadata((d, e) => ((TreeViewFieldHeader)d).NodePropertyChanged((IVisualCustomizationTreeItem)e.NewValue)));
			ShowCheckBoxProperty = DependencyPropertyManager.Register("ShowCheckBox", typeof(bool), ownerType, new PropertyMetadata(false));
			CheckedProperty = DependencyPropertyManager.Register("Checked", typeof(bool), ownerType, new PropertyMetadata(false, (d, e) => ((TreeViewFieldHeader)d).WriteChecked()));
		}
		#endregion
		public TreeViewFieldHeader() {
			this.SetDefaultStyleKey(typeof(TreeViewFieldHeader));
		}
		public TreeViewFieldHeader(IVisualCustomizationTreeItem node)
			: this() {
				FieldHeadersBase.SetFieldListArea(this, FieldListArea.All);
				Node = node;
		}
#if !SL
		bool mouseCaptured;
		protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e) {
			base.OnMouseLeftButtonDown(e);
			DependencyObject dobj = e.OriginalSource as DependencyObject;
			if(dobj == null || DevExpress.Xpf.Core.Native.LayoutHelper.FindParentObject<DevExpress.Xpf.Editors.CheckEdit>(dobj) == null)
				mouseCaptured = CaptureMouse();
		}
		protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e) {
			base.OnMouseLeftButtonUp(e);
			if(mouseCaptured)
				ReleaseMouseCapture();
			mouseCaptured = false;
		}
#endif
		public string DisplayText {
			get { return (string)GetValue(DisplayTextProperty); }
			set { SetValue(DisplayTextProperty, value); }
		}
		public ImageSource ImageSource {
			get { return (ImageSource)GetValue(ImageSourceProperty); }
			set { SetValue(ImageSourceProperty, value); }
		}
		public IVisualCustomizationTreeItem Node {
			get { return (IVisualCustomizationTreeItem)GetValue(NodeProperty); }
			set { SetValue(NodeProperty, value); }
		}
		public bool ShowCheckBox {
			get { return (bool)GetValue(ShowCheckBoxProperty); }
			set { SetValue(ShowCheckBoxProperty, value); }
		}
		public bool Checked {
			get { return (bool)GetValue(CheckedProperty); }
			set { SetValue(CheckedProperty, value); }
		}
		void NodePropertyChanged(IVisualCustomizationTreeItem node) {
			if(node == null) return;
			DisplayText = node.ToString();
			ImageSource = GetImageSource(node);
			if(node.Field != null)
				SetField(this, ((PivotFieldItem)node.Field).Wrapper);
			node.ExpandedChanged += (s, e) => ImageSource = GetImageSource(node);
		}
		protected virtual ImageSource GetImageSource(IVisualCustomizationTreeItem node) {
			return ImageHelper.GetImage(node.ImageName);
		}
		protected override IDragElement CreateDragElementCore(Point offset) {
			return new TreeViewFieldHeaderDragElement(this, offset);
		}
		protected internal override void OnFieldChanged(PivotGridField oldField) {
			if(Field == null)
				return;
			ReadChecked();
			EnsureSubscribeEvents(oldField);
		}
		void ReadChecked() {
			if(Field == null)
				return;
			inChekedSet = true;
			Checked = !Data.FieldListFields.HiddenFields.Contains(Field);
			inChekedSet = false;
		}
		void WriteChecked() {
			if(inChekedSet || Field == null)
				return;
			if(!Checked)
				Data.FieldListFields.HideField(Field.FieldItem);
			else
				Data.FieldListFields.MoveField(Field.FieldItem, Field.Area.ToPivotArea(), Math.Min(Data.FieldListFields[Field.Area].Count, Field.AreaIndex));
		}
		protected override void OnUnloaded(object sender, RoutedEventArgs e) {
			base.OnUnloaded(sender, e);
			loaded = false;
			EnsureSubscribeEvents();
		}
		protected override void OnLoaded(object sender, RoutedEventArgs e) {
			base.OnLoaded(sender, e);
			loaded = true;
			ReadChecked();
			EnsureSubscribeEvents();
		}
		INotifyCollectionChanged GetFieldsCollection(PivotGridField field) {
			return Field != null ? Field.Data.FieldListFields.HiddenFields : null;
		}
		protected virtual void EnsureSubscribeEvents(PivotGridField field = null) {
			if(field == null)
				field = Field;
			bool subscribe = ShowCheckBox && Field != null && loaded;
			if(subscribe) {
				if(subscribed) {
					return;
				} else {
					INotifyCollectionChanged fieldsCollection = GetFieldsCollection(field);
					if(fieldsCollection != null) {
						fieldsCollection.CollectionChanged += HiddenFieldsChanged;
						subscribed = true;
					}
				}
			} else {
				if(subscribed) {
					INotifyCollectionChanged fieldsCollection = GetFieldsCollection(field);
					if(fieldsCollection != null) {
						fieldsCollection.CollectionChanged -= HiddenFieldsChanged;
						subscribed = false;
					}
				}
			}
		}
		void HiddenFieldsChanged(object sender, NotifyCollectionChangedEventArgs e) {
			ReadChecked();
		}
	}
	public class TreeViewFieldHeaderDragElement : FieldHeaderDragElement {
		public TreeViewFieldHeaderDragElement(FieldHeaderBase header, Point offset)
			: base(header, header, offset) { }
		protected override FrameworkElement HeaderButton { get { return FieldHeader; } }
	}
	public class DragTreeViewFieldHeader : TreeViewFieldHeader {
		public DragTreeViewFieldHeader() { }
		protected override void EnsureChangeFieldSortCommand(bool can) { }
		protected override void CreateDragDropElementHelper() { }
	}
}
