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
using System.Linq;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Core;
#if !SILVERLIGHT
using DevExpress.Mvvm.UI.Interactivity;
using Grid = DevExpress.Xpf.Printing.Native.Lines.LinesGrid;
using DevExpress.Xpf.Utils;
using System.Collections.Generic;
#endif
namespace DevExpress.Xpf.Printing.Native.Lines {
	public class LinesContainer : DXContentPresenter {
		const int headerColumnIndex = 0;
		const int editorColumnIndex = 2;
		const int separatorColumnWidth = 4;
		const int editorColumnMinWidth = 60;
		const int rowSpacerHeight = 4;
		public static readonly DependencyProperty LinesProperty;
		public IEnumerable<LineBase> Lines {
			get { return (IEnumerable<LineBase>)GetValue(LinesProperty); }
			set { SetValue(LinesProperty, value); }
		}
		static LinesContainer() {
			LinesProperty = DependencyPropertyManager.Register("Lines", typeof(IEnumerable<LineBase>), typeof(LinesContainer),
				new FrameworkPropertyMetadata(null, (d, e) => ((LinesContainer)d).OnLinesChanged((LineBase[])e.NewValue)));
		}
		public LinesContainer() {
		}
#if !SILVERLIGHT
		void SetLines(LineBase[] lines, Window owner) {
#else
		public void SetLines(LineBase[] lines) {
#endif
			if(lines == null)
				throw new ArgumentNullException("lines");
			Grid grid = new Grid();
#if !SILVERLIGHT
			Interaction.GetBehaviors(grid).Add(new AutoWidthColumnBehavior() { AutoWidthColumnIndex = 0 });
			grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0) });
#else
			grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
#endif
			grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(separatorColumnWidth) });
			grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star), MinWidth = editorColumnMinWidth });
			foreach(LineBase line in lines) {
				AddRow(grid, line, grid.RowDefinitions.Count);
				AddRowSpacer(grid);
				line.RefreshContent();
				PropertyLine propertyLine = line as PropertyLine;
				if(propertyLine != null) {
					propertyLine.ValueChanged += OnLineValueChanged;
				}
#if !SILVERLIGHT
				ButtonEditPropertyLine buttonLine = line as ButtonEditPropertyLine;
				if(buttonLine != null) {
					buttonLine.OwnerWindow = owner;
				}
#endif
			}
			Content = grid;
		}
		void OnLinesChanged(LineBase[] lines) {
			var owner = Window.GetWindow(this);
			SetLines(lines, owner);
		}
		void OnLineValueChanged(object sender, EventArgs e) {
			PropertyLine changedLine = (PropertyLine)sender;
			RefreshPropertiesAttribute attr = (RefreshPropertiesAttribute)changedLine.Property.Attributes[typeof(RefreshPropertiesAttribute)];
			if(attr != null && attr.RefreshProperties == RefreshProperties.All) {
				foreach(LineBase line in Lines) {
					line.RefreshContent();
				}
			}
		}
		private static void AddRow(Grid grid, LineBase line, int row) {
			grid.RowDefinitions.Add(new RowDefinition());
			if(line.Header != null) {
				line.Header.SetValue(Grid.RowProperty, row);
				line.Header.SetValue(Grid.ColumnProperty, headerColumnIndex);
				grid.Children.Add(line.Header);
				line.Content.SetValue(Grid.ColumnProperty, editorColumnIndex);
			} else {
				line.Content.SetValue(Grid.ColumnProperty, headerColumnIndex);
				line.Content.SetValue(Grid.ColumnSpanProperty, grid.ColumnDefinitions.Count);
			}
			line.Content.SetValue(Grid.RowProperty, row);
			grid.Children.Add(line.Content);
			AddInvisibleBorder(grid, line, row);
		}
		private static void AddInvisibleBorder(Grid grid, LineBase line, int row) {
			ComboBoxPropertyLine editorLine = line as ComboBoxPropertyLine;
			if(editorLine != null && !editorLine.IsDropDownMode) {
				Border invisibleBorder = new Border();
				invisibleBorder.SizeChanged += (o, e) => SyncWidth(editorLine.Editor, e.NewSize.Width);
				invisibleBorder.SetValue(Grid.RowProperty, row);
				invisibleBorder.SetValue(Grid.ColumnProperty, editorColumnIndex);
				grid.Children.Add(invisibleBorder);
			}
		}
		static void SyncWidth(FrameworkElement element, double width) {
			element.Width = width;
		}
		void AddRowSpacer(Grid grid) {
			grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(rowSpacerHeight) });
		}
	}
}
