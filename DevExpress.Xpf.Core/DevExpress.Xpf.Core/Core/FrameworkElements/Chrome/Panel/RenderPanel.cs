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
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Markup;
namespace DevExpress.Xpf.Core.Native {
	[ContentProperty("Children")]
	public class RenderPanel : FrameworkRenderElement {
		LayoutProvider layoutProvider = LayoutProvider.GridInstance;
		FRElementCollection children;
#if DEBUGTEST
		[DevExpress.Xpf.Editors.Tests.IgnoreFREChecker]
#endif
		public LayoutProvider LayoutProvider {
			get { return layoutProvider; }
			set { layoutProvider = value ?? LayoutProvider.GridInstance; }
		}
		public RenderPanel() {
			children = new FRElementCollection(this);
		}
		public FRElementCollection Children { get { return children; } }
		protected override FrameworkRenderElementContext CreateContextInstance() {
			return new RenderPanelContext(this);
		}
		protected override void InitializeContext(FrameworkRenderElementContext context) {
			foreach (var child in Children)
				context.AddChild(child.CreateContext(context.Namescope, context.ElementHost));
			base.InitializeContext(context);
		}
		protected override Size MeasureOverride(Size availableSize, IFrameworkRenderElementContext context) {			
			return (((RenderPanelContext)context).LayoutProvider ?? LayoutProvider).MeasureOverride(availableSize, context);
		}
		protected override Size ArrangeOverride(Size finalSize, IFrameworkRenderElementContext context) {
			return (((RenderPanelContext)context).LayoutProvider ?? LayoutProvider).ArrangeOverride(finalSize, context);
		}
	}
	public class RenderPanelContext : FrameworkRenderElementContext {
		readonly List<FrameworkRenderElementContext> children = new List<FrameworkRenderElementContext>();
		LayoutProvider layoutProvider;
		public LayoutProvider LayoutProvider {
			get { return layoutProvider; }
			set { SetProperty(ref layoutProvider, value); }
		}
		protected override int RenderChildrenCount { get { return children.Count; } }
		public RenderPanelContext(FrameworkRenderElement factory) : base(factory) {
		}
		protected override FrameworkRenderElementContext GetRenderChild(int index) {
			return children[index];
		}
		public override void AddChild(FrameworkRenderElementContext child) {
			base.AddChild(child);
			children.Add(child);
		}
	}
	public class FRElementCollection : ObservableCollection<FrameworkRenderElement> {
		readonly FrameworkRenderElement parent;
		public FRElementCollection(FrameworkRenderElement parent) {
			this.parent = parent;
		}
		protected override void ClearItems() {
			foreach (var item in this)
				item.Parent = parent;
			base.ClearItems();
		}
		protected override void InsertItem(int index, FrameworkRenderElement item) {
			base.InsertItem(index, item);
			item.Parent = parent;
		}
		protected override void RemoveItem(int index) {
			this[index].Parent = null;
			base.RemoveItem(index);
		}
		protected override void SetItem(int index, FrameworkRenderElement item) {
			base.SetItem(index, item);
			item.Parent = parent;
		}
	}
}
