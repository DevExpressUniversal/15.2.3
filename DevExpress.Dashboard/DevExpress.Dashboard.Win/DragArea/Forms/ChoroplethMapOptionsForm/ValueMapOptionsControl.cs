#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.Localization;
using DevExpress.DataAccess.Native;
using DevExpress.XtraEditors;
using DevExpress.XtraMap;
using DevExpress.XtraMap.Native;
namespace DevExpress.DashboardWin.Native {
	public partial class ValueMapOptionsControl : DashboardUserControl {		
		readonly Locker locker = new Locker();
		readonly ColorCollection autoColors;
		readonly ColorizerScale absoluteScale = null;
		readonly ColorizerScale percentScale = null;
		MapPaletteEqualityComparer paletteComparer = new MapPaletteEqualityComparer();
		MapScaleEqualityComparer scaleComparer = new MapScaleEqualityComparer();
		Color userColor1 = Color.Empty;
		Color userColor2 = Color.Empty;
		bool percents = true;
		bool allowChanges = false;
		bool userColors = false;
		int percentMarksCount = 10;
		int absoluteMarksCount = 10;
		MapScale initialScale;
		MapPalette initialPalette;
		public bool Changed { get { return !(scaleComparer.Equals(initialScale, GetScale()) && paletteComparer.Equals(initialPalette, GetPalette())); } }
		public event EventHandler OnChanged;
		public ValueMapOptionsControl() {
			InitializeComponent();
			autoColors = ColorizerPaletteHelper.GetGradientColors(LookAndFeel);
			userColor1 = autoColors[0];
			userColor2 = autoColors[1];
			absoluteScale = new ColorizerScale(userColor1,userColor2);
			percentScale = new ColorizerScale(userColor1, userColor2);
			absoluteScale.ListChanged += ScaleListChanged;
			percentScale.ListChanged += ScaleListChanged;
			RefreshAbsoluteScale();
			RefreshPercentScale();
			gridControl.DataSource = percentScale;
			UpdateUI();			
		}
		void RaiseChanged() {
			if(OnChanged != null)
				OnChanged(this, EventArgs.Empty);
		}
		void RefreshAbsoluteScale() {
			var marks = GetAbsoluteRangeStops(absoluteMarksCount);
			absoluteScale.UpdateScale(marks);
		}
		List<double> GetAbsoluteRangeStops(int marksCount) {
			var marks = new List<double>(marksCount);
			for(int i = 0; i < marksCount; i++)
				marks.Add(100 * i);
			return marks;
		}
		void RefreshPercentScale() {
			List<double> marks = ValueMapScaleHelper.GetPercentRangeStops(percentMarksCount);
			percentScale.UpdateScale(marks);
		}
		void UpdateUI() {			
			locker.Lock();
			try {
				colorEdit1.Color = userColor1;
				colorEdit2.Color = userColor2;
				percentLevelsEdit.Value = Convert.ToDecimal(percentMarksCount);
				absoluteLevelsEdit.Value = Convert.ToDecimal(absoluteMarksCount);
				colorEdit1.Enabled = userColors;
				colorEdit2.Enabled = userColors;
				customColorsCheckEdit.Checked = userColors;
				absoluteLevelsEdit.Enabled = !percents;
				absoluteScaleCheckEdit.Checked  = !percents;
				percentLevelsEdit.Enabled = percents;
				percentsCheckEdit.Checked = percents;
				allowEditCheckEdit.Checked = allowChanges;
				gridView.OptionsBehavior.Editable = allowChanges;
				gridView.OptionsBehavior.ReadOnly = !allowChanges;
				absoluteLevelsEdit.Enabled = !(percents || allowChanges);
				percentLevelsEdit.Enabled = percents && !allowChanges;
				gridControl.DataSource = percents ? percentScale : absoluteScale;
				gridControl.RefreshDataSource();
			}
			finally {
				locker.Unlock();
			}
		}
		void ScaleListChanged(object sender, ListChangedEventArgs e) {
			RaiseChanged();
		}
		void levelsEdit_EditValueChanged(object sender, EventArgs e) {
			if (locker.IsLocked)
				return;
			percentMarksCount = Convert.ToInt32(percentLevelsEdit.Value);
			RefreshPercentScale();
			UpdateUI();
			RaiseChanged();
		}		
		void customColorsCheckEdit_CheckedChanged(object sender, EventArgs e) {
			if (locker.IsLocked)
				return;
			userColors = customColorsCheckEdit.Checked;
			if (userColors)
				UpdateColors(userColor1, userColor2);
			else
				UpdateColors(autoColors[0], autoColors[1]);
			UpdateUI();
			RaiseChanged();
		}
		void UpdateColors(Color color1, Color color2) {
			percentScale.Color1 = color1;
			absoluteScale.Color1 = color1;
			percentScale.Color2 = color2;
			absoluteScale.Color2 = color2;
		}
		void colorEdit1_EditValueChanged(object sender, EventArgs e) {
			if (locker.IsLocked)
				return;
			userColor1 = colorEdit1.Color;
			UpdateColors(userColor1, userColor2);
			gridControl.RefreshDataSource();
			RaiseChanged();
		}
		void colorEdit2_EditValueChanged(object sender, EventArgs e) {
			if (locker.IsLocked)
				return;
			userColor2 = colorEdit2.Color;
			UpdateColors(userColor1, userColor2);
			gridControl.RefreshDataSource();
			RaiseChanged();
		}
		void percentsCheckEdit_CheckedChanged(object sender, EventArgs e) {
			if (locker.IsLocked)
				return;
			percents = percentsCheckEdit.Checked;
			RaiseChanged();
			UpdateUI();
		}
		void allowEditCheckEdit_CheckedChanged(object sender, EventArgs e) {
			if (locker.IsLocked)
				return;
			allowChanges = allowEditCheckEdit.Checked;
			if(!allowChanges && ((percents && percentScale.HasChanges) || (!percents && absoluteScale.HasChanges))) {
				if(XtraMessageBox.Show(LookAndFeel, this, DashboardWinLocalizer.GetString(DashboardWinStringId.MapLayerOptionsResetCustomScale),
					DashboardWinLocalizer.GetString(DashboardWinStringId.MessageBoxWarningTitle), MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No) {
					allowChanges = true;
					locker.Lock();
					try {
						allowEditCheckEdit.Checked = allowChanges;
					}
					finally {
						locker.Unlock();
					}
				}
				else {
					if(percents)
						RefreshPercentScale();
					else
						RefreshAbsoluteScale();
					RaiseChanged();
				}
			}
			UpdateUI();
		}
		void absoluteLevelsSpinEdit_EditValueChanged(object sender, EventArgs e) {
			if (locker.IsLocked)
				return;
			absoluteMarksCount = Convert.ToInt32(absoluteLevelsEdit.Value);			
			RefreshAbsoluteScale();
			RaiseChanged();
			UpdateUI();
		}
		internal MapPalette GetPalette() {
			ColorizerScale currentScale = percents ? percentScale : absoluteScale;
			if(userColors || allowChanges) {
				if(currentScale.Any(sc => sc.ColorChanged)) {
					CustomPalette customPalette = new CustomPalette();
					customPalette.Colors.AddRange(currentScale.Select(cs => cs.Color).ToList());
					return customPalette;
				}
				else if(userColors) {
					GradientPalette gradientPalette = new GradientPalette();
					gradientPalette.StartColor = userColor1;
					gradientPalette.EndColor = userColor2;
					return gradientPalette;
				}
			}
			return null;
		}
		internal MapScale GetScale() {
			ColorizerScale currentScale = percents ? percentScale : absoluteScale;
			if((allowChanges && currentScale.HasChanges) || !percents) {
				CustomScale scale = new CustomScale();
				scale.IsPercent = percents;
				scale.RangeStops.AddRange(currentScale.Select(cs => cs.Range).OrderBy(rangeStop => rangeStop).ToList());
				return scale;
			}
			else {
				UniformScale scale = new UniformScale();
				scale.LevelsCount = percentMarksCount;
				return scale;
			}
		}
		internal void InitializeFrom(MapPalette palette, MapScale scale) {
			initialPalette = palette;
			initialScale = scale;
			List<double> marks = null;
			if(scale != null) {
				CustomScale customScale = scale as CustomScale;
				if(customScale != null) {
					percents = customScale.IsPercent;
					marks = new List<double>(customScale.RangeStops);
					absoluteMarksCount = percentMarksCount = marks.Count;
					allowChanges = true;
				}
				else {
					UniformScale uniformScale = scale as UniformScale;
					if(uniformScale != null) {
						percents = true;
						percentMarksCount = uniformScale.LevelsCount;
					}
				}
			}
			if(marks == null) {
				percentScale.UpdateScale(ValueMapScaleHelper.GetPercentRangeStops(percentMarksCount));
				absoluteScale.UpdateScale(GetAbsoluteRangeStops(absoluteMarksCount));
			}
			else {
				if(percents)
					percentScale.UpdateScale(marks);
				else
					absoluteScale.UpdateScale(marks);
			}
			if(palette != null) {
				GradientPalette gradientPalette = palette as GradientPalette;
				if(gradientPalette != null) {
					userColors = true;
					userColor1 = gradientPalette.StartColor;
					userColor2 = gradientPalette.EndColor;
					UpdateColors(userColor1, userColor2);
				}
				else {
					CustomPalette customPalette = palette as CustomPalette;
					if(customPalette != null) {
						for(int i = 0; i < customPalette.Colors.Count; i++) {
							if(i < percentScale.Count)
								percentScale[i].Color = customPalette.Colors[i];
							if(i < absoluteScale.Count)
								absoluteScale[i].Color = customPalette.Colors[i];
						}
					}
					else
						userColors = false;
				}
			}
			UpdateUI();	
		}
	}
	public class ColorizerScale : BindingList<ColorizerScaleMark> {
		Color color2;
		Color color1;
		bool hasChanges = false;
		public bool HasChanges { get { return hasChanges; } }
		public Color Color1 {
			get { return color1; }
			set {
				if (color1 != value) {
					color1 = value;
					UpdateColors();
				}
			}
		}
		public Color Color2 {
			get { return color2; }
			set {
				if (color2 != value) {
					color2 = value;
					UpdateColors();
				}
			}
		}
		public ColorizerScale(Color color1, Color color2) {
			ListChanged += ColorizerScale_ListChanged;
			this.color1 = color1;
			this.color2 = color2;
			this.AllowNew = true;		  
		}
		public void UpdateScale(IList<double> marks) {
			Clear();
			for(int i = 0; i < marks.Count; i++)
				Add(new ColorizerScaleMark(marks[i], ValueMapScaleHelper.GetGradientColor(color1, color2, i, marks.Count)));
			hasChanges = false;
		}
		protected override object AddNewCore() {
			ColorizerScaleMark scaleMark = new ColorizerScaleMark(0, Color.Empty);
			Add(scaleMark);
			return scaleMark;
		}
		void ColorizerScale_ListChanged(object sender, ListChangedEventArgs e) {
			if((e.ListChangedType == ListChangedType.ItemChanged && e.PropertyDescriptor.Name == "Range") ||
				e.ListChangedType == ListChangedType.ItemAdded || e.ListChangedType == ListChangedType.ItemDeleted || e.ListChangedType == ListChangedType.ItemMoved) {
				UpdateColors();
			}
			hasChanges = true;
		}
		void UpdateColors() {
			for(int i = 0; i < Count; i++) {
				ColorizerScaleMark mark = this[i];
				if(!mark.ColorChanged)
					mark.SetColor(ValueMapScaleHelper.GetGradientColor(color1, color2, i, Count));
			}
		}
	}
	public class ColorizerScaleMark : INotifyPropertyChanged {
		private bool colorChanged = false;
		private Color color;
		private double range;
		public bool ColorChanged { get { return colorChanged; } }
		public double Range {
			get { return range; }
			set {
				if (range != value) {
					range = value;
					RaisePropertyChanged("Range");
				}
			}
		}
		public Color Color {
			get { return color; }
			set {
				if (color != value) {
					SetColor(value);
					colorChanged = true;
					RaisePropertyChanged("Color");
				}
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		public ColorizerScaleMark(double range, Color color) {
			this.range = range;
			this.color = color;
		}
		public void SetColor(Color color) {
			this.color = color;
		}
		void RaisePropertyChanged(string propertyName) {
			PropertyChangedEventArgs args = new PropertyChangedEventArgs(propertyName);
			if (PropertyChanged != null)
				PropertyChanged(this, args);
		}
	}
}
