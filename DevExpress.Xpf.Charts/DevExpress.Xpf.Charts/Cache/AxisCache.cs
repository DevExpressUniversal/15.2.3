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
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using DevExpress.Charts.Native;
namespace DevExpress.Xpf.Charts.Native {
	public class AxisCache {
		static ContentPresenter CreateContentPresenter(DataTemplate template, object content) {
			ContentPresenter presenter = new ContentPresenter();
			presenter.Content = content;
			presenter.ContentTemplate = template;
			presenter.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
			presenter.Arrange(new Rect() { Width = presenter.DesiredSize.Width, Height = presenter.DesiredSize.Height });
			return presenter;
		}
		readonly Dictionary<int, Brush> elementsBrushes = new Dictionary<int, Brush>();
		readonly Dictionary<object, ContentPresenter> axisLabels = new Dictionary<object, ContentPresenter>();
		readonly Dictionary<int, ContentPresenter> constantLineTitles = new Dictionary<int, ContentPresenter>();
		readonly Dictionary<int, Model3D> axisModels = new Dictionary<int, Model3D>();
		ContentPresenter axisTitle;
		public ContentPresenter GetLabelPresenter(Axis axis, AxisTextItem textItem) {
			object content = textItem.Content;
			ContentPresenter contentPresenter;
			if (!axisLabels.TryGetValue(content, out contentPresenter)) {				
				DataTemplate itemTemplate = null;
				AxisLabel label = axis.ActualLabel;
				if (label != null)
					itemTemplate = TemplateHelper.GetAxisLabelItemTemplate(label);
				contentPresenter = CreateContentPresenter(itemTemplate, textItem);
				axisLabels.Add(content, contentPresenter);
			}
			return contentPresenter;
		}
		public ContentPresenter GetTitlePresenter(Axis axis) {
			if (axisTitle == null) {
				AxisTitle title = axis.Title;
				DataTemplate contentTemplate = null;
				if (title != null)
					contentTemplate = TemplateHelper.GetAxisTitleTemplate(title);
				axisTitle = CreateContentPresenter(contentTemplate, title);
			}
			return axisTitle;
		}
		public void Clear() {
			elementsBrushes.Clear();
			axisModels.Clear();
			foreach (ContentPresenter presenter in axisLabels.Values) {
				presenter.ContentTemplate = null;
				presenter.Content = null;
			}
			axisLabels.Clear();
			if (axisTitle != null) {
				axisTitle.ContentTemplate = null;
				axisTitle.Content = null;
				axisTitle = null;
			}
		}
		public Model3D GetModel(ISupportFlowDirection supportFlowDirection, ContentPresenter presenter) {
			int key = presenter.GetHashCode();
			if (!axisModels.ContainsKey(key))
				axisModels.Add(key, Label3DHelper.CreateModel(presenter, supportFlowDirection));
			return axisModels[key];
		}
	}
}
