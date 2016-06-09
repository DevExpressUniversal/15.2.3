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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraScheduler.Native;
using DevExpress.Utils.Drawing;
namespace DevExpress.XtraScheduler.Design {
	public class SchedulerFeature {
		string featureName;
		string valueDisplayName;
		Rectangle nameBounds;
		Rectangle valueBounds;
		Color valueColor;
		public string FeatureName { get { return featureName; } set { featureName = value; } }
		public string ValueDisplayName { get { return valueDisplayName; } set { valueDisplayName = value; } }
		public Rectangle NameBounds { get { return nameBounds; } set { nameBounds = value; } }
		public Rectangle ValueBounds { get { return valueBounds; } set { valueBounds = value; } }
		public Color ValueColor { get { return valueColor; } set { valueColor = value; } }
		protected internal virtual Size CalcFeatureNameTextSize(GraphicsCache cache, Font font, StringFormat sf) {
			return Size.Round(cache.CalcTextSize(this.FeatureName, font, sf, 99999999));
		}
		protected internal virtual Size CalcValueDisplayNameTextSize(GraphicsCache cache, Font font, StringFormat sf) {
			return Size.Round(cache.CalcTextSize(this.ValueDisplayName, font, sf, 99999999));
		}
		protected internal virtual void Draw(GraphicsCache cache, Font font, StringFormat sf) {
			cache.DrawString(this.FeatureName, font, SystemBrushes.WindowText, this.NameBounds, sf);
			cache.DrawString(this.ValueDisplayName, font, cache.GetSolidBrush(this.ValueColor), this.ValueBounds, sf);
		}
		protected internal virtual void SetMaxNameAndValueWidth(int maxNameWidth, int maxValueWidth) {
		}
	}
	public class SchedulerFeatureSeparator : SchedulerFeature {
		const int height = 5;
		protected internal override Size CalcFeatureNameTextSize(GraphicsCache cache, Font font, StringFormat sf) {
			return new Size(0, height);
		}
		protected internal override Size CalcValueDisplayNameTextSize(GraphicsCache cache, Font font, StringFormat sf) {
			return new Size(0, height);
		}
		protected internal override void SetMaxNameAndValueWidth(int maxNameWidth, int maxValueWidth) {
			Rectangle bounds = NameBounds;
			bounds.Width = maxNameWidth + maxValueWidth;
			NameBounds = bounds;
		}
		protected internal override void Draw(GraphicsCache cache, Font font, StringFormat sf) {
			Rectangle bounds = NameBounds;
			bounds.Y += height / 2;
			bounds.Height = 1;
			cache.FillRectangle(Brushes.Black, bounds);
		}
	}
	public class SchedulerFeatureCollection : List<SchedulerFeature> {
	}
	public class DesignTimeFeatureInfo : IDisposable {
		const int Transparency = 190;
		SchedulerControl control;
		SchedulerFeatureCollection features;
		Font font;
		StringFormat sf;
		Rectangle bounds;
		public DesignTimeFeatureInfo(SchedulerControl control) {
			this.control = control;
			features = new SchedulerFeatureCollection();
			font = DevExpress.Utils.AppearanceObject.DefaultFont;
			sf = DevExpress.Utils.TextOptions.DefaultStringFormat.Clone() as StringFormat;
		}
		public void Dispose() {
			sf.Dispose();
			sf = null;
		}
		public SchedulerControl Control { get { return control; } }
		public void PrepareInfo() {
			Color colorOk = Color.Green;
			Color colorWarning = Color.Orange;
			Color colorError = Color.Red;
			features.Clear();
			SchedulerFeature feature;
			feature = new SchedulerFeature();
			feature.FeatureName = "Storage Assigned";
			feature.ValueDisplayName = Control.DataStorage != null ? "Yes" : "No";
			feature.ValueColor = Control.DataStorage != null ? colorOk : colorError;
			features.Add(feature);
			feature = new SchedulerFeature();
			feature.FeatureName = "Data Binding Mode";
			feature.ValueDisplayName = Control.UnboundMode ? "Unbound" : "Bound";
			feature.ValueColor = Control.UnboundMode ? colorWarning : colorOk;
			features.Add(feature);
			feature = new SchedulerFeatureSeparator();
			features.Add(feature);
			feature = new SchedulerFeature();
			feature.FeatureName = "Supports Recurrence";
			feature.ValueDisplayName = Control.SupportsRecurrence ? "Yes" : "No";
			feature.ValueColor = Control.SupportsRecurrence ? colorOk : colorWarning;
			features.Add(feature);
			feature = new SchedulerFeature();
			feature.FeatureName = "Supports Reminders";
			feature.ValueDisplayName = Control.SupportsReminders && Control.RemindersEnabled ? "Yes" : "No";
			feature.ValueColor = Control.SupportsReminders && Control.RemindersEnabled ? colorOk : colorWarning;
			features.Add(feature);
			feature = new SchedulerFeature();
			feature.FeatureName = "Supports Resource Sharing";
			feature.ValueDisplayName = Control.ResourceSharing ? "Yes" : "No";
			feature.ValueColor = Control.ResourceSharing ? colorOk : colorWarning;
			features.Add(feature);
			feature = new SchedulerFeatureSeparator();
			features.Add(feature);
			SchedulerViewRepository views = Control.Views;
			int viewCount = views.Count;
			for (int i = 0; i < viewCount; i++) {
				SchedulerViewBase view = views[i];
				feature = new SchedulerFeature();
				feature.FeatureName = view.DisplayName;
				feature.ValueDisplayName = view.Enabled ? "Enabled" : "Disabled";
				feature.ValueColor = view.Enabled ? colorOk : colorWarning;
				features.Add(feature);
			}
			feature = new SchedulerFeatureSeparator();
			features.Add(feature);
			if (Control.DataStorage != null) {
				string[] requiredMappings = Control.DataStorage.Appointments.Mappings.GetRequiredMappingNames();
				int count = requiredMappings.Length;
				MappingCollection mappings = new MappingCollection();
				Control.DataStorage.Appointments.AppendBaseMappings(mappings);
				for (int i = 0; i < count; i++) {
					string mappingName = requiredMappings[i];
					MappingBase mapping = mappings[mappingName];
					feature = new SchedulerFeature();
					feature.FeatureName = String.Format("Required Mapping '{0}'", mappingName);
					feature.ValueDisplayName = mapping != null ? "Assigned" : "Unassigned";
					feature.ValueColor = mapping != null ? colorOk : colorError;
					features.Add(feature);
				}
			}
		}
		public void CalcLayout(GraphicsCache cache) {
			int x = 15;
			int y = 40;
			bounds = new Rectangle(x, y, 0, 0);
			int maxNameWidth = 0;
			int maxValueWidth = 0;
			int count = features.Count;
			for (int i = 0; i < count; i++) {
				SchedulerFeature feature = features[i];
				Size size = feature.CalcFeatureNameTextSize(cache, font, sf);
				maxNameWidth = Math.Max(size.Width, maxNameWidth);
				feature.NameBounds = new Rectangle(x, y, size.Width, size.Height);
				size = feature.CalcValueDisplayNameTextSize(cache, font, sf);
				maxValueWidth = Math.Max(size.Width, maxValueWidth);
				feature.ValueBounds = new Rectangle(x, y, size.Width, size.Height);
				y += size.Height;
			}
			maxNameWidth += 10;
			for (int i = 0; i < count; i++) {
				SchedulerFeature feature = features[i];
				Rectangle r = feature.ValueBounds;
				r.X += maxNameWidth;
				feature.ValueBounds = r;
			}
			bounds.Width = maxNameWidth + maxValueWidth;
			bounds.Height = y - bounds.Y;
			bounds.Inflate(5, 5);
			int offset = (Control.Width - 5 - (Control.DateTimeScrollBar.Visible ? Control.DateTimeScrollBar.Width : 0)) - bounds.Right;
			Offset(offset, maxNameWidth, maxValueWidth);
		}
		void Offset(int offset, int maxNameWidth, int maxValueWidth) {
			bounds.X += offset;
			int count = features.Count;
			for (int i = 0; i < count; i++) {
				SchedulerFeature feature = features[i];
				Rectangle r;
				r = feature.NameBounds;
				r.X += offset;
				feature.NameBounds = r;
				r = feature.ValueBounds;
				r.X += offset;
				feature.ValueBounds = r;
				feature.SetMaxNameAndValueWidth(maxNameWidth, maxValueWidth);
			}
		}
		public void Draw(GraphicsCache cache) {
			Color color = Color.FromArgb(Transparency, SystemColors.Window.R, SystemColors.Window.G, SystemColors.Window.B);
			cache.FillRectangle(cache.GetSolidBrush(color), bounds);
			ControlPaint.DrawBorder3D(cache.Graphics, bounds, Border3DStyle.Raised);
			int count = features.Count;
			for (int i = 0; i < count; i++)
				features[i].Draw(cache, font, sf);
		}
	}
}
