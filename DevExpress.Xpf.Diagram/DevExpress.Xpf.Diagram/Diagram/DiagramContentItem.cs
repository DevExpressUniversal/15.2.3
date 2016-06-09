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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Diagram.Core;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Utils.Serializing;
using System.Windows.Media;
namespace DevExpress.Xpf.Diagram {
	interface IDiagramContentItem : IDiagramItem {
		[XtraSerializableProperty]
		CornerRadius CornerRadius { get; set; }
		[XtraSerializableProperty]
		Thickness BorderThickness { get; set; }
		[XtraSerializableProperty]
		Brush BorderBrush { get; set; }
	}
	public class DiagramContentItem : DiagramItem, IDiagramContentItem {
		public object Content {
			get { return (object)GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}
		public static readonly DependencyProperty ContentProperty;
		public DataTemplate ContentTemplate {
			get { return (DataTemplate)GetValue(ContentTemplateProperty); }
			set { SetValue(ContentTemplateProperty, value); }
		}
		public static readonly DependencyProperty ContentTemplateProperty;
		public DataTemplateSelector ContentTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ContentTemplateSelectorProperty); }
			set { SetValue(ContentTemplateSelectorProperty, value); }
		}
		public static readonly DependencyProperty ContentTemplateSelectorProperty;
		public CornerRadius CornerRadius {
			get { return (CornerRadius)GetValue(CornerRadiusProperty); }
			set { SetValue(CornerRadiusProperty, value); }
		}
		public static readonly DependencyProperty CornerRadiusProperty;
		static DiagramContentItem() {
			DependencyPropertyRegistrator<DiagramContentItem>.New()
				.OverrideDefaultStyleKey()
				.AddOwner(x => x.CornerRadius, out CornerRadiusProperty, Border.CornerRadiusProperty)
				.Register(x => x.Content, out ContentProperty, null)
				.Register(x => x.ContentTemplate, out ContentTemplateProperty, null)
				.Register(x => x.ContentTemplateSelector, out ContentTemplateSelectorProperty, null)
			;
		}
		protected sealed override DiagramItemController CreateItemController() {
			return CreateContentItemController();
		}
		protected virtual DiagramContentItemController CreateContentItemController() {
			return new DiagramContentItemController(this);
		}
		protected override void OnActualSizeChanged() {
			base.OnActualSizeChanged();
			UpdateRotateTransform();
		}
	}
	public class DiagramContentItemController : DiagramItemController {
		readonly IFontTraits fontTraits;
		public DiagramContentItemController(DiagramContentItem item) : base(item) {
			fontTraits = new FontTraits(item);
		}
		public override DiagramItemStyleId GetDefaultStyleId() {
			return DefaultDiagramStyleId.Variant1;
		}
		public override IFontTraits GetFontTraits() {
			return fontTraits;
		}
		public override IEnumerable<Point> GetConnectionPoints() {
			return BasicShapes.Rectangle.GetConnectionPoints(Item.ActualSize, Enumerable.Empty<double>());
		}
		public override ReadOnlyCollection<DiagramItemStyleId> GetDiagramItemStylesId() {
			return DiagramShapeStyleId.Styles;
		}
	}
}
