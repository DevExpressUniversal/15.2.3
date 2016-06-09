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
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.XtraCharts.Design {
	internal class StylesContainerControl : XtraScrollableControl {
		class AppearanceColors {
			ChartAppearance appearance;
			Palette palette;
			StyleEdit[] editors;
			public ChartAppearance Appearance { get { return appearance; } }
			public int ColorsCount { get { return palette.Count + 1; } }
			public StyleEdit this[int index] {
				get { return editors[index]; }
				set { editors[index] = value; }
			}
			public AppearanceColors(ChartAppearance appearance, Palette palette) {
				this.appearance = appearance;
				this.palette = palette;
				editors = new StyleEdit[ColorsCount];
			}
		}
		PaletteRepository paletteRepository;
		List<AppearanceColors> matrix;
		int lastRegisteredAppearance;
		Size elementSize;
		Point current;
		bool lockChangeCurrent = false;
		public ChartAppearance CurrentAppearance { get { return matrix[current.Y].Appearance; } }
		public int CurrentPaletteIndex { get { return current.X; } }
		public event EventHandler OnEditValueChanged;
		public event EventHandler OnNeedClose;
		public void SetPaletteRepository(PaletteRepository paletteRepository) {
			this.paletteRepository = paletteRepository;
		}
		public void SetAppearancesCount(int count) {
			matrix = new List<AppearanceColors>(count);
			lastRegisteredAppearance = 0;
		}
		public void SelectStyle(ChartAppearance appearance, int paletteIndex) {
			for (int i = 0; i < matrix.Count; i++) {
				AppearanceColors row = matrix[i];
				if (row.Appearance == appearance && paletteIndex < row.ColorsCount) {
					current = new Point(paletteIndex, i);
					lockChangeCurrent = true;
					return;
				}
			}
			current = Point.Empty;
		}
		public void RegisterAppearance(ChartAppearance appearance, ViewType viewType, Palette palette) {
			AppearanceColors row = new AppearanceColors(appearance, palette);
			matrix.Add(row);
			for (int i = 0; i <= palette.Count; i++)
				row[i] = AddStyleEditor(AppearanceImageHelper.CreateImage(viewType, appearance, palette, i), i);
			lastRegisteredAppearance++;
		}
		StyleEdit AddStyleEditor(Image image, int column) {
			elementSize = image.Size;
			StyleEdit styleEdit = new StyleEdit();
			styleEdit.Location = new Point(elementSize.Width * column, elementSize.Height * lastRegisteredAppearance);
			styleEdit.Size = elementSize;
			styleEdit.Image = image;
			styleEdit.BorderStyle = BorderStyles.NoBorder;
			styleEdit.Properties.ReadOnly = true;
			Controls.Add(styleEdit);
			return styleEdit;
		}
		void SelectCurrentChild() {
			lockChangeCurrent = true;
			try {
				matrix[current.Y][current.X].Focus();
			}
			finally {
				lockChangeCurrent = false;
			}
		}
		void CheckCurrentX() {
			int colorsCount = matrix[current.Y].ColorsCount;
			if (current.X >= colorsCount)
				current.X = colorsCount - 1;
		}
		void RaiseEditValueChanged() {
			if (OnEditValueChanged != null)
				OnEditValueChanged(this, EventArgs.Empty);
		}
		internal void RaiseNeedClose() {
			if (OnNeedClose != null)
				OnNeedClose(this, EventArgs.Empty);
		}
		internal void OnFocusChanged() {
			if (lockChangeCurrent)
				return;
			for (int y = 0; y < matrix.Count; y++) {
				AppearanceColors row = matrix[y];
				for (int x = 0; x < row.ColorsCount; x++) 
					if (row[x].Focused) {
						current.X = x;
						current.Y = y;
						RaiseEditValueChanged();
						return;
					}
			}
		}
		protected override void OnGotFocus(EventArgs e) {
			SelectCurrentChild();
		}
		protected override bool ProcessDialogKey(Keys keyData) {
			switch (keyData) {
				case Keys.Left:
					if (--current.X < 0)
						current.X = matrix[current.Y].ColorsCount - 1;
					break;
				case Keys.Right:
					if (++current.X >= matrix[current.Y].ColorsCount)
						current.X = 0;
					break;
				case Keys.Up:
					if (--current.Y < 0)
						current.Y = matrix.Count - 1;
					CheckCurrentX();
					break;
				case Keys.Down:
					if (++current.Y >= matrix.Count)
						current.Y = 0;
					CheckCurrentX();
					break;
				case Keys.Home:
					current.X = 0;
					current.Y = 0;
					break;
				case Keys.End:
					current.Y = matrix.Count - 1;
					current.X = matrix[current.Y].ColorsCount - 1;
					break;
				case Keys.PageUp: {
					int step = ClientSize.Height / elementSize.Height;
					if (step == 0)
						step++;
					current.Y -= step;
					if (current.Y < 0)
						current.Y = 0;
					current.X = 0;
					break;
				}
				case Keys.PageDown: {
					int step = ClientSize.Height / elementSize.Height;
					if (step == 0)
						step++;
					current.Y += step;
					if (current.Y >= matrix.Count)
						current.Y = matrix.Count - 1;
					current.X = int.MaxValue;
					CheckCurrentX();
					break;
				}
				default:
					return base.ProcessDialogKey(keyData);
			}
			SelectCurrentChild();
			RaiseEditValueChanged();
			return true;
		}
		private void InitializeComponent() {
			this.SuspendLayout();
			this.ResumeLayout(false);
		}
	}
}
