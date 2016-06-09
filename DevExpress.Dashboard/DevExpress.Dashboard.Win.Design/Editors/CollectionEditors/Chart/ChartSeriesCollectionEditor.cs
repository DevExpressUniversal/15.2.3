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
using System.Reflection;
using System.Windows.Forms;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.Native;
using DevExpress.Data.Utils;
using DevExpress.LookAndFeel.DesignService;
using DevExpress.Utils;
using DevExpress.Utils.UI;
using DevExpress.XtraTreeList.Nodes;
namespace DevExpress.DashboardWin.Design {
	public abstract class SeriesCollectionEditor : NotifyingCollectionEditor {
		const string ChartSeriesTypePropertyName = "SeriesType";
		protected SeriesCollectionEditor(Type type)
			: base(type) {
		}
		protected override string GetItemName(object item, int index) {
			string name = base.GetItemName(item, index);
			if(!string.IsNullOrEmpty(name))
				return name;
			PropertyInfo pi = item.GetType().GetProperty(ChartSeriesTypePropertyName);
			if(pi != null)
				name = pi.GetValue(item, null).ToString();
			else
				name = item.GetType().Name;
			if(name == null || !name.Contains(ChartSeriesCollectionEditorFormBase.SeriesTitle))
				name = name + ChartSeriesCollectionEditorFormBase.SeriesTitle;
			return name;
		}
	}
	public class RangeFilterSeriesCollectionEditor : SeriesCollectionEditor {
		public RangeFilterSeriesCollectionEditor(Type type)
			: base(type) {
		}
		protected override Utils.UI.CollectionEditorFormBase CreateCollectionForm(IServiceProvider serviceProvider) {
			return new RangeFilterSeriesCollectionEditorForm(serviceProvider, this);
		}
	}
	public class ChartSeriesCollectionEditor : SeriesCollectionEditor {
		public ChartSeriesCollectionEditor(Type type)
			: base(type) {
		}
		protected override Utils.UI.CollectionEditorFormBase CreateCollectionForm(IServiceProvider serviceProvider) {
			return new ChartSeriesCollectionEditorForm(serviceProvider, this);
		}
	}
	public class ChartSeriesCollectionEditorForm : ChartSeriesCollectionEditorFormBase {
		protected override IEnumerable<SeriesViewGroup> SeriesViewGroups {
			get { return ChartDashboardItem.SeriesViewGroups; }
		}
		public ChartSeriesCollectionEditorForm(IServiceProvider provider, CollectionEditor collectionEditor)
			: base(provider, collectionEditor) {
		}
	}
	public class RangeFilterSeriesCollectionEditorForm : ChartSeriesCollectionEditorFormBase {
		protected override IEnumerable<SeriesViewGroup> SeriesViewGroups {
			get { return RangeFilterDashboardItem.SeriesViewGroups; }
		}
		public RangeFilterSeriesCollectionEditorForm(IServiceProvider provider, CollectionEditor collectionEditor)
			: base(provider, collectionEditor) {
		}
	}
	public abstract class ChartSeriesCollectionEditorFormBase : CollectionEditorFormBase {
		class ChartSeriesCollectionContentControl : CollectionEditorContentControl {
			static List<ChartSeriesConverter> imageList;
			static List<ChartSeriesConverter> ImageList {
				get {
					if(imageList == null) {
						imageList = new List<ChartSeriesConverter>();
						foreach(SeriesViewGroup group in ChartDashboardItem.SeriesViewGroups)
							foreach(ChartSeriesConverter converter in group.Converters)
								imageList.Add(converter);
					}
					return imageList;
				}
			}
			ChartSeriesOptionsForm form;
			public ChartSeriesCollectionContentControl(IServiceProvider provider, CollectionEditor collectionEditor, IEnumerable<SeriesViewGroup> seriesViewGroups)
				: base(provider, collectionEditor) {
				form = new ChartSeriesOptionsForm();
				form.InitializeGallery(seriesViewGroups);
				ILookAndFeelService service = provider.GetService<ILookAndFeelService>();
				if(service != null)
					form.InitLookAndFeel(service.LookAndFeel);
				form.StartPosition = FormStartPosition.CenterParent;
				propertyGrid.PropertyGridControl.CellValueChanged += PropertyGridControl_CellValueChanged;
				this.tv.OptionsView.AllowGlyphSkinning = !BitmapStorage.UseColors;
				ImageCollection imageCollection = new ImageCollection();
				foreach(ChartSeriesConverter converter in ImageList)
					imageCollection.AddImage(ImageHelper.GetImage(BitmapStorage.UseColors ? converter.SmallGalleryImagePath : converter.DragAreaSmallImagePath));
				this.tv.StateImageList = imageCollection;
			}
			protected override void buttonAdd_Click(object sender, EventArgs e) {
				if(form.ShowDialog(FindForm()) == System.Windows.Forms.DialogResult.OK) {
					ChartSeries series = CreateInstance(form.ChartSeries.GetType()) as ChartSeries;
					series.Assign(form.ChartSeries);
					AddInstance(series);
				}
			}
			void PropertyGridControl_CellValueChanged(object sender, XtraVerticalGrid.Events.CellValueChangedEventArgs e) {
				if(e.Row.Name != null && e.Row.Name.Contains("SeriesType"))
					foreach(TreeListNode node in tv.Nodes)
						node.StateImageIndex = GetItemImageIndex(node.Tag);
			}
			protected override int GetItemImageIndex(object item) {
				return ImageList.FindIndex((conv) => conv.IsMatch(item as ChartSeries));
			}
		}
		internal const string SeriesTitle = "Series";
		protected abstract IEnumerable<SeriesViewGroup> SeriesViewGroups { get; }
		protected ChartSeriesCollectionEditorFormBase(IServiceProvider provider, CollectionEditor collectionEditor)
			: base(provider, collectionEditor) {
			Text = SeriesTitle;
		}
		protected override CollectionEditorContentControl CreateCollectionEditorContentControl(IServiceProvider provider, CollectionEditor collectionEditor) {
			return new ChartSeriesCollectionContentControl(provider, collectionEditor, SeriesViewGroups);
		}
	}
}
