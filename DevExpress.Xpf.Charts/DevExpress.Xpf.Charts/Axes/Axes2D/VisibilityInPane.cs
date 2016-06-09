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

using System.ComponentModel;
using System.Windows;
using DevExpress.Charts.Native;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
using DevExpress.Utils.Serializing;
namespace DevExpress.Xpf.Charts {
	public class VisibilityInPane : ChartNonVisualElement {
		public static readonly DependencyProperty PaneProperty = DependencyPropertyManager.Register("Pane", 
			typeof(Pane), typeof(VisibilityInPane), new PropertyMetadata(PropertyChanged));
		public static readonly DependencyProperty VisibleProperty = DependencyPropertyManager.Register("Visible",
			typeof(bool), typeof(VisibilityInPane), new PropertyMetadata(true, PropertyChanged));
		static void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ChartElementHelper.Update(d, e, ChartElementChange.UpdatePanesItems);
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("VisibilityInPanePane"),
#endif
		Category(Categories.Behavior)
		]
		public Pane Pane {
			get { return (Pane)GetValue(PaneProperty); }
			set { SetValue(PaneProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("VisibilityInPaneVisible"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool Visible {
			get { return (bool)GetValue(VisibleProperty); }
			set { SetValue(VisibleProperty, value); }
		}
		int paneIndex = -1;
		XYDiagram2D diagram = null;
		XYDiagram2D Diagram {
			get {
				if (diagram == null) {
					Axis2D axis = ((IChartElement)this).Owner as Axis2D;
					if (axis != null)
						diagram = axis.Diagram2D;
				}
				return diagram;
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty
		]
		public int PaneIndex {
			get {
				if (Pane != null && Diagram != null)
					return Diagram.Panes.IndexOf(Pane);
				return -1;
			}
			set { paneIndex = value; }
		}
		internal void CompleteDeserializing() {
			if (Diagram != null) {
				if (paneIndex == -1)
					Pane = Diagram.DefaultPane;
				else if (paneIndex >= 0 && paneIndex < Diagram.Panes.Count)
					Pane = Diagram.Panes[paneIndex];
			}
		}
	}
	public class VisibilityInPaneCollection : ChartElementCollection<VisibilityInPane> {
		Axis2D Axis { get { return (Axis2D)Owner; } }
		protected override ChartElementChange Change { get { return base.Change | ChartElementChange.UpdatePanesItems; } }
	}
}
namespace DevExpress.Xpf.Charts.Native {
	public class ActualAxisVisibilityInPanes : AxisVisibilityInPanes {
		public ActualAxisVisibilityInPanes(Axis2D axis) : base(axis) {
		}
		public override bool SetVisibilityInPane(IPane pane, bool visible) {
			if (!base.SetVisibilityInPane(pane, visible))
				Visibility.Add(pane, visible);
			return true;
		}
		public void UpdateVisibilityInPanes(VisibilityInPaneCollection collection) {
			Reset(true);
			foreach (VisibilityInPane visibility in collection) {
				IPane pane = visibility.Pane;
				if (pane != null)
					SetVisibilityInPane(pane, visibility.Visible);
			}
		}
	}
}
