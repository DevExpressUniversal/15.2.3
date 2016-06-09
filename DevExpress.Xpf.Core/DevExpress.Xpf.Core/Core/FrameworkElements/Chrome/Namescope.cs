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
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Editors.Helpers;
namespace DevExpress.Xpf.Core.Native {
	public interface IPropertyChangedListener {
		void SubscribeValueChangedAsync(RenderPropertyChangedListenerContext context);
		void UnsubscribeValueChanged(object target, RenderPropertyChangedListenerContext context);
		void SubscribeValueChanged(object target, RenderPropertyChangedListenerContext context);
		void Flush();
	}
	public interface INamescope {
		FrameworkRenderElementContext GetElement(string name);
		IEnumerable<RenderTriggerContextBase> Triggers { get; set; }
		FrameworkRenderElementContext RootElement { get; }
		void RegisterElement(FrameworkRenderElementContext context);
		void ReleaseElement(FrameworkRenderElementContext context);
		void AddChild(FrameworkRenderElementContext context);
		void RemoveChild(FrameworkRenderElementContext context);
		void GoToState(string state);
	}
	public interface IElementHost {
		int ChildrenCount { get; }
		FrameworkElement GetChild(int index);
		IEnumerator LogicalChildren { get; }
		FrameworkElement TemplatedParent { get; }
		FrameworkElement Parent { get; }
		void InvalidateMeasure();
		void InvalidateArrange();
		void InvalidateVisual();
	}
	public class Namescope : INamescope, IElementHost, IPropertyChangedListener {
		readonly Dictionary<string, FrameworkRenderElementContext> dictionary = new Dictionary<string, FrameworkRenderElementContext>();
		readonly List<RenderControlBaseContext> children = new List<RenderControlBaseContext>();
		readonly ValueChangedStorage valueChangedStorage = new ValueChangedStorage();
		readonly Queue<ChangeValueListenerTask> subscribeQueue = new Queue<ChangeValueListenerTask>();
		readonly IChrome chrome;
		Namescope Parent { get; set; }
		IChrome Chrome { get { return chrome ?? Parent.With(x => x.Chrome); } }
		public IEnumerable<RenderTriggerContextBase> Triggers { get; set; }
		public FrameworkRenderElementContext RootElement { get; set; }
		public ValueChangedStorage ValueChangedStorage { get { return valueChangedStorage; } }
		public FrameworkRenderElementContext GetElement(string name) {
			FrameworkRenderElementContext result;
			dictionary.TryGetValue(name, out result);
			return result;
		}
		public Namescope()
			: this(new Chrome()) {
		}
		public Namescope(Namescope parent) {
			Parent = parent;
		}
		public Namescope(IChrome chrome) {
			this.chrome = chrome;
		}
		public void PropagateDeferredActions() {
			isUsed = false;
			if (subscribeQueue.Count == 0)
				return;
			do {
				var context = subscribeQueue.Dequeue();
				context.Execute();
			}
			while (subscribeQueue.Count > 0);
		}
		bool isUsed = false;
		public void PropagateDeferredActionsAsync() {
			if (isUsed)
				return;
			isUsed = true;
			Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.DataBind, new Action(PropagateDeferredActions));
		}
		public void RegisterElement(FrameworkRenderElementContext context) {
			RegisterElementInternal(context);
			AddChild(context);
		}
		void RegisterElementInternal(FrameworkRenderElementContext context) {
			string name = context.Factory.Name;
			if (dictionary.ContainsKey(name))
				throw new ArgumentException(name);
			dictionary.Add(name, context);
		}
		public void ReleaseElement(FrameworkRenderElementContext element) {
			var context = ReleaseElementInternal(element.Name);
			RemoveChild(context);
		}
		FrameworkRenderElementContext ReleaseElementInternal(string name) {
			var context = dictionary[name];
			dictionary.Remove(name);
			return context;
		}
		public void AddChild(FrameworkRenderElementContext context) {
			if (!context.AttachToRoot || context.IsAttachedToRoot)
				return;
			var renderElement = context as RenderControlBaseContext;
			if(renderElement != null) {
				AddToTree(renderElement);
			}				
			context.AttachToVisualTree((FrameworkElement)Chrome);
			context.IsAttachedToRoot = true;
		}
		void AddToTree(RenderControlBaseContext context) {
			if (Parent == null) {
				Chrome.AddChild(context.Control);
				children.Add(context);
				return;
			}
			Parent.AddToTree(context);
		}
		public void RemoveFromTree(RenderControlBaseContext context) {
			if (Parent == null) {
				Chrome.RemoveChild(context.Control);
				children.Remove(context);
				return;
			}
			Parent.RemoveFromTree(context);
		}
		public void RemoveChild(FrameworkRenderElementContext context) {
			if(!context.IsAttachedToRoot)
				return;
			var renderElement = context as RenderControlBaseContext;
			if (renderElement != null)
				RemoveFromTree(renderElement);
			context.DetachFromVisualTree((FrameworkElement)Chrome);
		}
		int IElementHost.ChildrenCount { get { return children.Count; } }
		FrameworkElement IElementHost.GetChild(int index) { return children[index].Control; }
		IEnumerator IElementHost.LogicalChildren { get { return children.Select(child => child.Control).GetEnumerator(); } }
		FrameworkElement IElementHost.TemplatedParent { get { return (FrameworkElement)((FrameworkElement)Chrome).TemplatedParent; } }
		FrameworkElement IElementHost.Parent { get { return (FrameworkElement)Chrome; } }
		void IElementHost.InvalidateMeasure() {
			Chrome.InvalidateMeasure();
			Chrome.InvalidateVisual();
		}
		void IElementHost.InvalidateArrange() {
			Chrome.InvalidateArrange();
		}
		void IElementHost.InvalidateVisual() {
			Chrome.InvalidateVisual();
		}
		public void SubscribeValueChangedAsync(RenderPropertyChangedListenerContext context) {
			subscribeQueue.Enqueue(new ChangeValueListenerTask(context));
			PropagateDeferredActionsAsync();
		}
		public void UnsubscribeValueChanged(object target, RenderPropertyChangedListenerContext context) {
			foreach (var element in subscribeQueue)
				if (element.Context == context) {
					element.Invalidate();
				}
			valueChangedStorage.UnsubscribeValueChanged(target, context);
		}
		public void SubscribeValueChanged(object target, RenderPropertyChangedListenerContext context) {
			valueChangedStorage.SubscribeValueChanged(target, context);
		}
		public void Flush() {
			PropagateDeferredActions();
		}
		public void GoToState(string stateName) {
			Triggers.Do(coll => coll.OfType<RenderStateGroupContext>().ForEach(x => x.GoToState(stateName)));
		}
	}	
	public class ChangeValueListenerTask {
		bool isValid;
		public ChangeValueListenerTask(RenderPropertyChangedListenerContext context) {
			Context = context;
			isValid = true;
		}
		public RenderPropertyChangedListenerContext Context { get; private set; }
		public void Invalidate() {
			isValid = false;
		}
		public void Execute() {
			if (!isValid)
				return;
			Context.SubscribeValue();
		}
	}
}
