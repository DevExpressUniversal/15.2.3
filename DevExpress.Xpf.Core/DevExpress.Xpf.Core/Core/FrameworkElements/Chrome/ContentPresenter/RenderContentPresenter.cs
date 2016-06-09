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

using DevExpress.Xpf.Core.Internal;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.Editors;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Core.Native {
	public class RenderRealContentPresenter : RenderControlBase {
		bool recognizesAccessKey;
		public bool RecognizesAccessKey {
			get { return recognizesAccessKey; }
			set { SetProperty(ref recognizesAccessKey, value); }
		}
		protected override FrameworkRenderElementContext CreateContextInstance() {
			return new RenderRealContentPresenterContext(this);
		}
		protected override FrameworkElement CreateFrameworkElement(FrameworkRenderElementContext context) {
			return new RenderContentPresenterElement() { RecognizesAccessKey = RecognizesAccessKey};
		}
	}
	public class ErrorRenderControl : RenderControlBase {
		protected override FrameworkElement CreateFrameworkElement(FrameworkRenderElementContext context) {
			return new ContentPresenter();
		}
		protected override FrameworkRenderElementContext CreateContextInstance() {
			return new ErrorRenderControlContext(this);
		}
	}
	public class ErrorRenderControlContext : RenderControlBaseContext {
		public ErrorRenderControlContext(ErrorRenderControl factory) : base(factory) { }
		ContentPresenter ErrorPresenter { get { return Control as ContentPresenter; } }
		protected internal override void AttachToVisualTree(FrameworkElement root) {
			base.AttachToVisualTree(root);
			var inplaceEdit = LayoutHelper.FindLayoutOrVisualParentObject<InplaceBaseEdit>(root) as IBaseEdit;
			if (inplaceEdit != null) {
				ErrorPresenter.Content = inplaceEdit.ValidationError;
				ErrorPresenter.ContentTemplate = inplaceEdit.ValidationErrorTemplate;
			}
		}
	}
	public interface IVisualTransformOwner {
		Transform VisualTransform { get; set; }
	}
	class RenderContentPresenterElement : ContentPresenter, IVisualTransformOwner {
		Transform IVisualTransformOwner.VisualTransform {
			get { return VisualTransform; }
			set { VisualTransform = value; }
		}
	}
	public class DefaultTemplateSelector : DataTemplateSelector {		
		static DataTemplateSelector defaultSelector = null;
		static Func<DependencyObject, object, Type, object> findTemplateResourceFunc = null;
		static Lazy<DefaultTemplateSelector> instance = new Lazy<DefaultTemplateSelector>();
		public static DefaultTemplateSelector Instance {
			get { return instance.Value; }
		}
		static Func<DependencyObject, object, Type, object> FindTemplateResourceFunc {
			get { return findTemplateResourceFunc ?? (findTemplateResourceFunc = ReflectionHelper.CreateInstanceMethodHandler<FrameworkElement, Func<DependencyObject, object, Type, object>>(null, "FindTemplateResourceInternal", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)); }
		}
		static DataTemplateSelector DefaultSelector {
			get { return defaultSelector ?? (defaultSelector = typeof(ContentPresenter).Assembly.GetType(String.Format("{0}+{1}", typeof(ContentPresenter).FullName, "DefaultSelector")).GetConstructor(Type.EmptyTypes).Invoke(null) as DataTemplateSelector); }
		}
		public override DataTemplate SelectTemplate(object item, DependencyObject container) {
			if (container is ContentPresenter)
				return DefaultSelector.SelectTemplate(item, container);
			else
				return (DataTemplate)FindTemplateResourceFunc(container, item, typeof(DataTemplate));
		}
	}
}
