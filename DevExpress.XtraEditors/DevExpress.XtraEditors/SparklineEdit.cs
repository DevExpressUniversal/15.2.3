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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Data.Helpers;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Sparkline;
using DevExpress.Sparkline.Core;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Native;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ToolboxIcons;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraPrinting;
namespace DevExpress.XtraEditors.Repository {
	[Designer("DevExpress.XtraEditors.Design.SparklineRepositoryItemDesigner, " + AssemblyInfo.SRAssemblyEditorsDesign)]
	public class RepositoryItemSparklineEdit : RepositoryItem, ISparklineSettings {
		const ColumnSortOrder defaultSortOrder = ColumnSortOrder.Ascending;
		internal const string SparklineEditName = "SparklineEdit";
		readonly SparklineRange valueRange;
		object dataSource;
		string editValueMember;
		string pointSortingMember;
		string pointValueMember;
		Padding padding = Padding.Empty;
		ColumnSortOrder pointSortOrder = defaultSortOrder;
		ListSourceDataController dataController;
		SparklineViewBase view;
		[DefaultValue(false)]
		internal bool UseBinding { get; private set; }
		public override bool AllowInplaceAutoFilter { get { return false; } }
		public override bool IsNonSortableEditor { get { return true; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool AutoHeight { get { return false; } }
		[Browsable(false)]
		public override string EditorTypeName { get { return SparklineEditName; } }
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		DXCategory(CategoryName.Appearance),
		Editor("DevExpress.XtraEditors.Design.SparklineViewEditor, " + AssemblyInfo.SRAssemblyEditors, typeof(UITypeEditor)),
		RefreshProperties(RefreshProperties.All),
		SmartTagProperty("View", "", SmartTagActionType.RefreshAfterExecute)
		]
		public SparklineViewBase View {
			get { return view; }
			set {
				if (value == null)
					throw new ArgumentNullException();
				if (view != value) {
					if (view != null)
						view.PropertiesChanged -= new EventHandler(ViewPropertiesChanged);
					if (!IsLoading && IsDesignMode)
						value.Assign(view);
					SetView(value);
					if (!IsLoading)
						OnPropertiesChanged();
				}
			}
		}
		[
		DefaultValue(null),
		DXCategory(CategoryName.Data),
		AttributeProvider(typeof(IListSource))
		]
		public object DataSource {
			get { return dataSource; }
			set {
				if (dataSource != value) {
					dataSource = value;
					if (!IsLoading) {
						UpdateDataController();
						OnPropertiesChanged();
					}
				}
			}
		}
		[
		DefaultValue(null),
		DXCategory(CategoryName.Data),
		TypeConverter("System.Windows.Forms.Design.DataMemberFieldConverter, System.Design"),
		Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design", typeof(UITypeEditor))
		]
		public string EditValueMember {
			get { return editValueMember; }
			set {
				if (editValueMember != value) {
					editValueMember = value;
					if (!IsLoading) {
						UpdateDataControllerSorting();
						OnPropertiesChanged();
					}
				}
			}
		}
		[
		DefaultValue(null),
		DXCategory(CategoryName.Data),
		TypeConverter("System.Windows.Forms.Design.DataMemberFieldConverter, System.Design"),
		Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design", typeof(UITypeEditor))
		]
		public string PointSortingMember {
			get { return pointSortingMember; }
			set {
				if (pointSortingMember != value) {
					pointSortingMember = value;
					if (!IsLoading) {
						UpdateDataControllerSorting();
						OnPropertiesChanged();
					}
				}
			}
		}
		[
		DefaultValue(defaultSortOrder),
		DXCategory(CategoryName.Data)
		]
		public ColumnSortOrder PointSortOrder {
			get { return pointSortOrder; }
			set {
				if (pointSortOrder != value) {
					pointSortOrder = value;
					UpdateDataController();
					OnPropertiesChanged();
				}
			}
		}
		[
		DefaultValue(null),
		DXCategory(CategoryName.Data),
		TypeConverter("System.Windows.Forms.Design.DataMemberFieldConverter, System.Design"),
		Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design", typeof(UITypeEditor))
		]
		public string PointValueMember {
			get { return pointValueMember; }
			set {
				if (pointValueMember != value) {
					pointValueMember = value;
					if (!IsLoading) {
						UpdateDataControllerSorting();
						OnPropertiesChanged();
					}
				}
			}
		}
		[DXCategory(CategoryName.Appearance)]
		public Padding Padding {
			get { return padding; }
			set {
				if (padding != value) {
					padding = value;
					if (!IsLoading)
						OnPropertiesChanged();
				}
			}
		}
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		DXCategory(CategoryName.Data),		
		RefreshProperties(RefreshProperties.All),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public SparklineRange ValueRange {
			get { return valueRange; }
		}
		public RepositoryItemSparklineEdit() {
			valueRange = new SparklineRange();
			valueRange.PropertiesChanged += new EventHandler(ValueRangePropertiesChanged);
			SetView(new LineSparklineView());
		}
		#region ISparklineSettings Members
		Padding ISparklineSettings.Padding { get { return Padding; } }
		SparklineViewBase ISparklineSettings.View { get { return View; } }
		#endregion
		#region ShouldSerialize
		bool ShouldSerializePadding() {
			return padding != Padding.Empty;
		}
		void ResetPadding() {
			padding = Padding.Empty;
		}
		#endregion
		void UpdateDataController() {
			if (dataController == null)
				dataController = new ListSourceDataController();
			if (DataSource != null) {
				UseBinding = true;
				dataController.SetDataSource(DataSource);
				UpdateDataControllerSorting();
			}
			else
				UseBinding = false;
		}
		void UpdateDataControllerSorting() {
			if (dataController == null)
				return;
			dataController.BeginUpdate();
			try {
				DataColumnInfo editValueColumn = EditValueMember != null ? dataController.Columns[EditValueMember] : null;
				DataColumnInfo pointSortingColumn = PointSortingMember != null ? dataController.Columns[PointSortingMember] : null;
				DataColumnInfo pointValueColumn = PointValueMember != null ? dataController.Columns[PointValueMember] : null;
				if (editValueColumn != null && pointValueColumn != null) {
					List<DataColumnSortInfo> sortInfoList = new List<DataColumnSortInfo>();
					sortInfoList.Add(new DataColumnSortInfo(editValueColumn, ColumnSortOrder.Ascending));
					if (pointSortingColumn != null)
						sortInfoList.Add(new DataColumnSortInfo(pointSortingColumn, PointSortOrder));
					dataController.SortInfo.ClearAndAddRange(sortInfoList.ToArray(), 1);
				}
				else
					dataController.SortInfo.Clear();
			}
			finally {
				dataController.EndUpdate();
			}
		}
		void SetView(SparklineViewBase view) {
			this.view = view;
			UpdateSparklineViewAppearance();
			this.view.PropertiesChanged += new EventHandler(ViewPropertiesChanged);
		}
		void ViewPropertiesChanged(object sender, EventArgs e) {
			OnPropertiesChanged(e);
		}
		void UpdateSparklineViewAppearance() {
			SparklineAppearanceHelper.SetSparklineAppearanceProvider(View, new SparklineEditAppearanceProvider(LookAndFeel.ActiveLookAndFeel));
		}
		void ValueRangePropertiesChanged(object sender, EventArgs e) {
			OnPropertiesChanged();
		}
		internal List<double> GetBoundValues(object editValue) {
			List<double> values = new List<double>();
			DataColumnInfo editValueColumn = EditValueMember != null ? dataController.Columns[EditValueMember] : null;
			DataColumnInfo pointValueColumn = PointValueMember != null ? dataController.Columns[PointValueMember] : null;
			if (editValueColumn != null && pointValueColumn != null && editValue != null) {
				int row = dataController.FindRowByValue(EditValueMember, editValue);
				GroupRowInfo group = dataController.GroupInfo.GetGroupRowInfoByControllerRowHandle(row);
				if (group != null) {
					for (int i = 0; i < group.ChildControllerRowCount; i++) {
						double value = Convert.ToDouble(dataController.GetRowValue(group.ChildControllerRow + i, pointValueColumn));
						values.Add(value);
					}
				}
			}
			return values;
		}
		protected override void OnLoaded() {
			base.OnLoaded();
			UpdateDataController();
			UpdateSparklineViewAppearance();
		}
		protected override void OnLookAndFeelChanged(object sender, EventArgs e) {
			base.OnLookAndFeelChanged(sender, e);
			UpdateSparklineViewAppearance();
		}
		protected override void OnStyleChanged(object sender, EventArgs e) {
			base.OnStyleChanged(sender, e);
			UpdateSparklineViewAppearance();
		}
		public override void Assign(RepositoryItem item) {
			BeginUpdate();
			try {
				base.Assign(item);
				RepositoryItemSparklineEdit source = item as RepositoryItemSparklineEdit;
				if (source != null) {
					view = SparklineViewBase.CreateView(source.view.Type);
					view.Assign(source.view);
					SparklineAppearanceHelper.SetSparklineAppearanceProvider(view, new SparklineEditAppearanceProvider(LookAndFeel.ActiveLookAndFeel));
					dataSource = source.dataSource;
					editValueMember = source.editValueMember;
					pointSortingMember = source.pointSortingMember;
					pointValueMember = source.pointValueMember;
					pointSortOrder = source.pointSortOrder;
					padding = new Padding(source.Padding.Left, source.Padding.Top, source.Padding.Right, source.Padding.Bottom);
					UpdateDataController();
				}
			}
			finally {
				EndUpdate();
			}
		}
		public override IVisualBrick GetBrick(PrintCellHelperInfo info) {
			IImageBrick brick = CreateImageBrick(info, CreateBrickStyle(info, "image"));
			MultiKey key = new MultiKey(new object[] { info.EditValue, View, EditorTypeName });
			SparklineEditViewInfo viewInfo = (SparklineEditViewInfo)PreparePrintViewInfo(info, true);
			viewInfo.EditValue = info.EditValue;
			Image image = GetCachedPrintImage(key, info.PS);
			if (image == null) {
				using (Graphics metafileGraphics = Graphics.FromHwnd(IntPtr.Zero)) {
					IntPtr hdc = metafileGraphics.GetHdc();
					try {
						Metafile metafile = new Metafile(hdc, viewInfo.Bounds, MetafileFrameUnit.Pixel, EmfType.EmfPlusOnly);
						using (Graphics graphics = Graphics.FromImage(metafile)) {
							viewInfo.CalcViewInfo(graphics);
							SparklineEditPainter painter = new SparklineEditPainter();
							ControlGraphicsInfoArgs args = new ControlGraphicsInfoArgs(viewInfo, new GraphicsCache(graphics), new Rectangle(Point.Empty, viewInfo.Bounds.Size));
							painter.Draw(args);
							image = AddImageToPrintCache(key, metafile, info.PS);
						}
					}
					finally {
						metafileGraphics.ReleaseHdc(hdc);
					}
				}
			}
			brick.Image = image;
			return brick;
		}
	}
}
namespace DevExpress.XtraEditors {
	class BindingListListener {
		public delegate void VoidDel();
		VoidDel onBindingListChanged;
		IBindingList bindableEditValue;
		public BindingListListener(VoidDel onBindingListChanged) {
			this.onBindingListChanged = onBindingListChanged;
		}
		void SubscribeBindingListChanged(IBindingList bindableEditValue) {
			if (bindableEditValue != null)
				bindableEditValue.ListChanged += new ListChangedEventHandler(bindableEditValue_ListChanged);
		}
		void UnsubscribeBindingListChanged(IBindingList bindableEditValue) {
			if (bindableEditValue != null)
				bindableEditValue.ListChanged -= new ListChangedEventHandler(bindableEditValue_ListChanged);
		}
		void bindableEditValue_ListChanged(object sender, ListChangedEventArgs e) {
			if (onBindingListChanged != null)
				onBindingListChanged();
		}
		public void SourceChanged(object list) {
			UnsubscribeBindingListChanged(bindableEditValue);
			bindableEditValue = list as IBindingList;
			SubscribeBindingListChanged(bindableEditValue);
		}
	}
	[
	DXToolboxItem(DXToolboxItemKind.Free),
	Description("Visualizes your data as a simple line, area, bar or win-loss chart."),
	Designer("DevExpress.XtraEditors.Design.SparklineEditDesigner, " + AssemblyInfo.SRAssemblyEditorsDesign),
	ToolboxTabName(AssemblyInfo.DXTabData), SmartTagAction(typeof(SparklineActions), "Data", "Data", SmartTagActionType.CloseAfterExecute),
	ToolboxBitmap(typeof(ToolboxIconsRootNS), "SparklineEdit")
	]
	public class SparklineEdit : BaseEdit {
		static Size defaultMinSize = new Size(40, 20);
		readonly BindingListListener listener;
		[Browsable(false)]
		public override string EditorTypeName { get { return RepositoryItemSparklineEdit.SparklineEditName; } }
		[
		DXCategory(CategoryName.Properties),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), SmartTagSearchNestedProperties
		]
		public new RepositoryItemSparklineEdit Properties { get { return base.Properties as RepositoryItemSparklineEdit; } }
		[
		DefaultValue(null),
		DXCategory(CategoryName.Data),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public double[] Data {
			get { return EditValue as double[]; }
			set { EditValue = value; }
		}
		public SparklineEdit() {
			listener = new BindingListListener(Refresh);
		}
		protected override Size CalcSizeableMinSize() {
			return defaultMinSize;
		}
		protected override Size CalcSizeableMaxSize() {
			return Size.Empty;
		}
		protected override void OnEditValueChanged() {
			base.OnEditValueChanged();
			listener.SourceChanged(EditValue);
		}
	}
}
namespace DevExpress.XtraEditors.ViewInfo {
	public class SparklineEditViewInfo : BaseEditViewInfo, ISparklineData {
		ISparklineSettings settings;
		IList<double> values;
		IList<double> ISparklineData.Values { get { return values; } }
		internal RepositoryItemSparklineEdit SparklineItem { get { return (RepositoryItemSparklineEdit)Item; } }
		protected virtual int CustomWidth { get { return 150; } }
		public SparklineEditViewInfo(RepositoryItem item) : base(item) {
			RepositoryItemSparklineEdit sparklineRepositoryItem = item as RepositoryItemSparklineEdit;
			if (sparklineRepositoryItem == null)
				throw new ArgumentException();
			settings = sparklineRepositoryItem;
			UpdateValues();
		}
		void UpdateValues() {
			values = ConvertValuesToDouble(EditValue);
			if (values == null) {
				if (Item.IsDesignMode)
					values = GenerateRandomValues();
				else if (SparklineItem.UseBinding)
					values = SparklineItem.GetBoundValues(EditValue);
			}
		}
		IList<double> ConvertValuesToDouble(object EditValue) {
			double[] convertedValues = null;
			if (EditValue is IList<double>)
				return EditValue as IList<double>;
			else if (EditValue is IList<sbyte> ||
				EditValue is IList<byte> ||
				EditValue is IList<short> ||
				EditValue is IList<ushort> ||
				EditValue is IList<int> ||
				EditValue is IList<uint> ||
				EditValue is IList<long> ||
				EditValue is IList<ulong> ||
				EditValue is IList<decimal> ||
				EditValue is IList<float>) {
				IList valuesList = EditValue as IList;
				convertedValues = new double[valuesList.Count];
				for (int i = 0; i < valuesList.Count; i++)
					convertedValues[i] = Convert.ToDouble(valuesList[i]);
			}
			return convertedValues;
		}
		List<double> GenerateRandomValues() {
			Random random = new Random(0);
			List<double> values = new List<double>();
			for (int i = 0; i < 10; i++)
				values.Add(random.NextDouble() - 0.5);
			return values;
		}
		protected override void OnEditValueChanged() {
			UpdateValues();
		}
		protected override Size CalcContentSize(Graphics g) {
			Size size = base.CalcContentSize(g);
			size.Width = Math.Max(size.Width, CustomWidth);
			return size;
		}
	}
}
namespace DevExpress.XtraEditors.Drawing {
	public class SparklineEditPainter : BaseEditPainter {
		SparklinePaintersCache paintersCache = new SparklinePaintersCache();
		protected override void DrawContent(ControlGraphicsInfoArgs info) {
			base.DrawContent(info);
			SparklineEditViewInfo sparklineViewInfo = info.ViewInfo as SparklineEditViewInfo;
			if (sparklineViewInfo != null) {
				UserLookAndFeel lookAndFeel = sparklineViewInfo.LookAndFeel.ActiveLookAndFeel;
				SparklineViewBase view = sparklineViewInfo.SparklineItem.View;
				SparklineEditAppearanceProvider appearanceProvider = SparklineAppearanceHelper.GetSparklineAppearanceProvider(view) as SparklineEditAppearanceProvider;
				if (appearanceProvider == null || appearanceProvider.SkinName != lookAndFeel.SkinName)
					SparklineAppearanceHelper.SetSparklineAppearanceProvider(view, new SparklineEditAppearanceProvider(lookAndFeel));
				BaseSparklinePainter painter = paintersCache.GetPainter(sparklineViewInfo.SparklineItem.View);
				painter.Initialize(sparklineViewInfo, sparklineViewInfo.SparklineItem, info.ViewInfo.ClientRect);
				painter.Draw(info.Graphics, info.Cache);
			}
		}
	}
}
namespace DevExpress.XtraEditors.Native {
	public class SparklineEditAppearanceProvider : ISparklineAppearanceProvider {
		public Color Color { get; private set; }
		public Color EndPointColor { get; private set; }
		public Color MarkerColor { get; private set; }
		public Color MaxPointColor { get; private set; }
		public Color MinPointColor { get; private set; }
		public Color NegativePointColor { get; private set; }
		public Color StartPointColor { get; private set; }
		public string SkinName { get; private set; }
		public SparklineEditAppearanceProvider(ISkinProvider skinProvider) {
			if (skinProvider != null) {
				Skin skin = SparklineSkins.GetSkin(skinProvider);
				if (skin == null)
					return;
				SkinName = skinProvider.SkinName;
				Color = skin.Colors.GetColor(SparklineSkins.Color);
				EndPointColor = skin.Colors.GetColor(SparklineSkins.ColorEndPoint);
				MarkerColor = skin.Colors.GetColor(SparklineSkins.ColorMarker);
				MaxPointColor = skin.Colors.GetColor(SparklineSkins.ColorMaxPoint);
				MinPointColor = skin.Colors.GetColor(SparklineSkins.ColorMinPoint);
				NegativePointColor = skin.Colors.GetColor(SparklineSkins.ColorNegativePoint);
				StartPointColor = skin.Colors.GetColor(SparklineSkins.ColorStartPoint);
			}
		}
	}
}
