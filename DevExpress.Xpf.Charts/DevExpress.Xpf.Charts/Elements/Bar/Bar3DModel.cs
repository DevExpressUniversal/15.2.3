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
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Markup;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Charts.Native;
using DevExpress.Charts.NotificationCenter;
namespace DevExpress.Xpf.Charts {
	public abstract class Bar3DModel : DependencyObject, IChartElement {
		#region IChartElement implementation
		IChartElement owner;
		IChartElement IOwnedElement.Owner { get { return owner; } set { SetOwner(value); } }
		void IChartElement.AddChild(object child) { }
		void IChartElement.RemoveChild(object child) { }
		bool IChartElement.Changed(ChartUpdate args) {
			Changed(args);
			return true;
		}
		ViewController INotificationOwner.Controller { get { return owner == null ? null : owner.Controller; } }
		protected virtual void Changed(ChartUpdate args) {
			if ((args.Change & ChartElementChange.CollectionModified) > 0)
				sectionsData = null;
			if (owner != null)
				owner.Changed(args);
		}
		internal void SetOwner(IChartElement owner) {
			this.owner = owner;
		}
		#endregion
		public static IEnumerable<Bar3DKind> GetPredefinedKinds() {
			return Bar3DKind.List;
		}
		internal List<Bar3DSectionData> sectionsData = null;
		protected internal abstract bool ActualLoadFromResources { get; }
		internal List<Bar3DSectionData> SectionsData {
			get {
				if (sectionsData == null)
					sectionsData = CreateSectionsData();
				return sectionsData;
			}
		}
#if !SL
	[DevExpressXpfChartsLocalizedDescription("Bar3DModelModelName")]
#endif
		public abstract string ModelName { get; }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public string TypeNameSerializable { get { return this.GetType().Name; } }
		protected internal abstract List<Bar3DSectionData> CreateSectionsData();
		protected abstract Bar3DModel CreateObjectForClone();
		protected virtual void Assign(Bar3DModel model) { }
		protected internal Bar3DModel CloneModel() {
			Bar3DModel model = CreateObjectForClone();
			model.Assign(this);
			return model;
		}
	}
	public abstract class PredefinedBar3DModel : Bar3DModel {
		List<PredefinedBar3DSection> sections = new List<PredefinedBar3DSection>();
		protected internal override bool ActualLoadFromResources { get { return true; } }
		protected void AddSection(string source, bool fixedHeight, bool alignByX, bool alignByY, bool alignByZ, bool useViewColor) {
			sections.Add(new PredefinedBar3DSection(source, fixedHeight, alignByX, alignByY, alignByZ, useViewColor));
		}
		protected internal override List<Bar3DSectionData> CreateSectionsData() {
			List<Bar3DSectionData> sectionsData = new List<Bar3DSectionData>(sections.Count);
			foreach (Bar3DSection section in sections) {
				Bar3DSectionData sectionData = Bar3DSectionData.CreateInstance(section, ActualLoadFromResources);
				if (sectionData != null)
					sectionsData.Add(sectionData);
			}
			return sectionsData;
		}
	}
	public class BoxBar3DModel : PredefinedBar3DModel {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("BoxBar3DModelModelName")]
#endif
		public override string ModelName { get { return "Box"; } }
		public BoxBar3DModel() {
			AddSection(@"bar3d\box\bottom.xaml", true, false, true, false, true);
			AddSection(@"bar3d\box\center.xaml", false, false, true, false, true);
			AddSection(@"bar3d\box\top.xaml", true, false, true, false, true);
		}
		protected override Bar3DModel CreateObjectForClone() {
			return new BoxBar3DModel();
		}
	}
	public class ConeBar3DModel : PredefinedBar3DModel {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("ConeBar3DModelModelName")]
#endif
		public override string ModelName { get { return "Cone"; } }
		public ConeBar3DModel() {
			AddSection(@"bar3d\cone\bottom.xaml", true, false, true, false, true);
			AddSection(@"bar3d\cone\top.xaml", false, false, true, false, true);
		}
		protected override Bar3DModel CreateObjectForClone() {
			return new ConeBar3DModel();
		}
	}
	public class CylinderBar3DModel : PredefinedBar3DModel {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("CylinderBar3DModelModelName")]
#endif
		public override string ModelName { get { return "Cylinder"; } }
		public CylinderBar3DModel() {
			AddSection(@"bar3d\cylinder\bottom.xaml", true, false, true, false, true);
			AddSection(@"bar3d\cylinder\center.xaml", false, false, true, false, true);
			AddSection(@"bar3d\cylinder\top.xaml", true, false, true, false, true);
		}
		protected override Bar3DModel CreateObjectForClone() {
			return new CylinderBar3DModel();
		}
	}
	public class HexagonBar3DModel : PredefinedBar3DModel {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("HexagonBar3DModelModelName")]
#endif
		public override string ModelName { get { return "Hexagon"; } }
		public HexagonBar3DModel() {
			AddSection(@"bar3d\hexagon\bottom.xaml", true, false, true, false, true);
			AddSection(@"bar3d\hexagon\center.xaml", false, false, true, false, true);
			AddSection(@"bar3d\hexagon\top.xaml", true, false, true, false, true);
		}
		protected override Bar3DModel CreateObjectForClone() {
			return new HexagonBar3DModel();
		}
	}
	public class PyramidBar3DModel : PredefinedBar3DModel {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("PyramidBar3DModelModelName")]
#endif
		public override string ModelName { get { return "Pyramid"; } }
		public PyramidBar3DModel() {
			AddSection(@"bar3d\pyramid.xaml", false, false, true, false, true);
		}
		protected override Bar3DModel CreateObjectForClone() {
			return new PyramidBar3DModel();
		}
	}
	[ContentProperty("Sections")]
	public class CustomBar3DModel : Bar3DModel {
		internal static readonly DependencyPropertyKey SectionsPropertyKey;
		public static readonly DependencyProperty LoadFromResourcesProperty;
		public static readonly DependencyProperty SectionsProperty;
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("CustomBar3DModelLoadFromResources"),
#endif
		Category(Categories.Common)
		]
		public bool LoadFromResources {
			get { return (bool)GetValue(LoadFromResourcesProperty); }
			set { SetValue(LoadFromResourcesProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("CustomBar3DModelSections"),
#endif
		Category(Categories.Elements)
		]
		public CustomBar3DSectionCollection Sections {
			get { return (CustomBar3DSectionCollection)GetValue(SectionsProperty); }
		}
		static CustomBar3DModel() {
			Type ownerType = typeof(CustomBar3DModel);
			LoadFromResourcesProperty = DependencyProperty.Register("LoadFromResources", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, LoadFromResourcesPropertyChanged));
			SectionsPropertyKey = DependencyProperty.RegisterReadOnly("Sections", typeof(CustomBar3DSectionCollection), ownerType,
				new PropertyMetadata());
			SectionsProperty = SectionsPropertyKey.DependencyProperty;
		}
		static void LoadFromResourcesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			CustomBar3DModel model = d as CustomBar3DModel;
			if (model != null)
				foreach (Bar3DSection section in model.Sections)
					section.ClearCache();
			ChartElementHelper.UpdateWithClearDiagramCache(d, e);
		}
		protected internal override bool ActualLoadFromResources { get { return LoadFromResources; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("CustomBar3DModelModelName")]
#endif
		public override string ModelName { get { return "Custom"; } }
		protected internal override List<Bar3DSectionData> CreateSectionsData() {
			List<Bar3DSectionData> sectionsData = new List<Bar3DSectionData>(Sections.Count);
			foreach (Bar3DSection section in Sections) {
				Bar3DSectionData sectionData = Bar3DSectionData.CreateInstance(section, ActualLoadFromResources);
				if (sectionData != null)
					sectionsData.Add(sectionData);
			}
			return sectionsData;
		}
		public CustomBar3DModel() {
			SetValue(SectionsPropertyKey, ChartElementHelper.CreateInstance<CustomBar3DSectionCollection>(this));
		}
		protected override Bar3DModel CreateObjectForClone() {
			return new CustomBar3DModel();
		}
		protected override void Assign(Bar3DModel model) {
			base.Assign(model);
			CustomBar3DModel customBar3DModel = model as CustomBar3DModel;
			if (customBar3DModel != null) {
				CopyPropertyValueHelper.CopyPropertyValue(this, customBar3DModel, LoadFromResourcesProperty);
				if (customBar3DModel.Sections != null) {
					Sections.Clear();
					foreach (CustomBar3DSection section in customBar3DModel.Sections) {
						CustomBar3DSection newSection = new CustomBar3DSection();
						newSection.Assign(section);
						Sections.Add(newSection);
					}
				}
			}
		}
	}
}
