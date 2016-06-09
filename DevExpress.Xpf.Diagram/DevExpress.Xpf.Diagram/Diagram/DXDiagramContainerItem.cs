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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DevExpress.Xpf.Diagram.Native;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
using System.ComponentModel;
using System;
using DevExpress.Diagram.Core;
using IInputElement = DevExpress.Diagram.Core.IInputElement;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Native;
namespace DevExpress.Xpf.Diagram {
	[ContentProperty("Items")]
	[TemplatePart(Name = ItemsPanelName, Type = typeof(Canvas))]
	public abstract partial class DiagramContainerBase : DiagramItem {
		protected const string ItemsPanelName = "ItemsPanel";
		static DiagramContainerBase() {
			DependencyPropertyRegistrator<DiagramContainerBase>.New()
				.FixPropertyValue(CanRotateProperty, false)
				.FixPropertyValue(AngleProperty, 0d)
			   .OverrideMetadata(ClipToBoundsProperty, true)
			   ;
		}
		public DiagramItemCollection Items { get; private set; }
		protected internal override IList<DiagramItem> NestedItems { get { return Items; } }
		public DiagramContainerBase() {
			Items = CreateDiagramItemCollection();
		}
		Canvas panel;
#if DEBUGTEST
		internal Canvas PanelForTests { get { return panel; } }
#endif
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			Items.SetHost(panel = (Canvas)GetTemplateChild(ItemsPanelName));
		}
		protected sealed override DiagramItemController CreateItemController() {
			return CreateContainerControllerBase();
		}
		protected abstract DiagramContainerControllerBase CreateContainerControllerBase();
	}
	public partial class DiagramContainer : DiagramContainerBase, IDiagramContainer {
		static DiagramContainer() {
			DependencyPropertyRegistrator<DiagramContainer>.New()
			   .OverrideDefaultStyleKey();
		}
		protected sealed override DiagramContainerControllerBase CreateContainerControllerBase() {
			return CreateContainerController();
		}
		protected virtual DiagramContainerController CreateContainerController() {
			return new DiagramContainerController(this);
		}
#if DEBUGTEST
		internal Border BorderForTests { get { return (Border)GetTemplateChild("Border"); } }
#endif
		protected override IEnumerable<PropertyDescriptor> GetEditablePropertiesCore() {
			return base.GetEditablePropertiesCore().Concat(new[] {
				DependencyPropertyDescriptor.FromProperty(IsSnapScopeProperty, typeof(DiagramContainer)),
				DependencyPropertyDescriptor.FromProperty(AdjustBoundsBehaviorProperty, typeof(DiagramContainer)),
			});
		}
	}
}
