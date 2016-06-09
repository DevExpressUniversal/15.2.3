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

using System.Windows.Controls;
using System.Windows;
using DevExpress.Xpf.Utils;
using System.Windows.Markup;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Core.Native {
	[ContentProperty("Children")]
	public class MultiContentControl : Control {
		static readonly DependencyPropertyKey ChildrenPropertyKey;
		public static readonly DependencyProperty ChildrenProperty;
		public static readonly DependencyProperty VisibleChildIndexProperty;
		static MultiContentControl() {
			ChildrenPropertyKey = DependencyPropertyManager.RegisterReadOnly("Children", typeof(ObservableCollection<UIElement>), typeof(MultiContentControl), new FrameworkPropertyMetadata(null));
			ChildrenProperty = ChildrenPropertyKey.DependencyProperty;
			VisibleChildIndexProperty = DependencyPropertyManager.Register("VisibleChildIndex", typeof(object), typeof(MultiContentControl),
				new FrameworkPropertyMetadata(0, (d, e) => ((MultiContentControl)d).PropertyChangedVisibleChildIndex(e.OldValue)));
		}
		public MultiContentControl() {
			this.SetDefaultStyleKey(typeof(MultiContentControl));
			Children = new ObservableCollection<UIElement>();
			Children.CollectionChanged += OnChildrenCollectionChanged;
		}
		public ObservableCollection<UIElement> Children {
			get { return (ObservableCollection<UIElement>)GetValue(ChildrenProperty); }
			protected set { this.SetValue(ChildrenPropertyKey, value); }
		}
		public object VisibleChildIndex {
			get { return GetValue(VisibleChildIndexProperty); }
			set { SetValue(VisibleChildIndexProperty, value); }
		}
		Grid RootElement { get; set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			if (RootElement != null)
				RootElement.Children.Clear();
			RootElement = GetTemplateChild("PART_Root") as Grid;
			UpdateVisibleChild();
		}
		protected virtual void OnChildrenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			UpdateVisibleChild();
		}
		protected virtual void PropertyChangedVisibleChildIndex(object oldValue) {
			UpdateVisibleChild();
		}
		protected virtual void UpdateVisibleChild() {
			if (RootElement == null) return;
			for (int i = RootElement.Children.Count - 1; i >= 0; i--)
				if (!Children.Contains(RootElement.Children[i]))
					RootElement.Children.RemoveAt(i);
			for (int i = 0; i < Children.Count; i++)
				if (!RootElement.Children.Contains(Children[i]))
					RootElement.Children.Add(Children[i]);
			for (int i = 0; i < Children.Count; i++)
				Children[i].SetVisible(i.ToString() == VisibleChildIndex.ToString());
		}
	}
}
