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

using DevExpress.Utils;
using DevExpress.XtraLayout;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraLayout.Utils;
using System.Text.RegularExpressions;
namespace DevExpress.XtraCharts.Designer.Native {
	public class UIOptionsBuilder : Component, ISupportInitialize {
		class GroupComparer : IComparer<Group> {
			public int Compare(Group x, Group y) {
				if (x.SortingRank == y.SortingRank)
					return 0;
				if (x.SortingRank >= y.SortingRank)
					return 1;
				return -1;
			}
		}
		class RowComparer : IComparer<PropertyRow> {
			public int Compare(PropertyRow x, PropertyRow y) {
				if (x.SortingRank == y.SortingRank)
					return 0;
				if (x.SortingRank >= y.SortingRank)
					return 1;
				return -1;
			}
		}
		class Group {
			const int titleHeigth = 15;
			LayoutControlItem titleLayoutItem;
			TitleElement title;
			string name;
			string prefix;
			int sortingRank;
			List<PropertyRow> rows;
			public int SortingRank { get { return sortingRank; } }
			public Group(string name, string prefix, int sortingRank) {
				rows = new List<PropertyRow>();
				this.sortingRank = sortingRank;
				this.name = name;
				this.prefix = prefix;
				if (!string.IsNullOrEmpty(name)) {
					title = new TitleElement();
					title.Title = name;
					if (!string.IsNullOrEmpty(prefix))
						title.Prefix = prefix;
					titleLayoutItem = new LayoutControlItem();
					titleLayoutItem.Location = new Point(0, 0);
					titleLayoutItem.SizeConstraintsType = SizeConstraintsType.Custom;
				}
			}
			public LabelControl AddProperty(PropertyInfo property, string propertyTitle, Control control, int propertySortingRank) {
				PropertyRow propertyRow = new PropertyRow(propertyTitle, control, propertySortingRank);
				rows.Add(propertyRow);
				return propertyRow.Title;
			}
			public int AddToLayoutControl(IContainer container, LayoutControl layoutControl, LayoutControlGroup root, int offset, Size controlSize, List<TitleElement> titles, bool isTopGroup) {
				LayoutControlGroup box = new LayoutControlGroup();
				box.Location = new Point(0, offset);
				box.Name = name + "Group";
				box.Text = name + "Group";
				box.GroupBordersVisible = false;
				box.ExpandButtonVisible = false;
				root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { box });
				if (title != null) {
					titles.Add(title);
					container.Add(title);
					layoutControl.Controls.Add(title);
					titleLayoutItem.Size = new Size(controlSize.Width - 50, titleHeigth);
					box.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { titleLayoutItem });
					titleLayoutItem.Control = title;
					int topPadding = isTopGroup ? 12 : 24;
					titleLayoutItem.Padding = new XtraLayout.Utils.Padding(12, 2, topPadding, 2);
					titleLayoutItem.MaxSize = new Size(0, titleHeigth + topPadding);
					titleLayoutItem.MinSize = new Size(40, titleHeigth + topPadding);
					titleLayoutItem.TextVisible = false;
					SimpleSeparator separator = new SimpleSeparator();
					separator.Size = new Size(controlSize.Width, 2);
					separator.Location = new Point(0, titleHeigth);
					separator.Spacing = new XtraLayout.Utils.Padding(0, 0, 0, 9);
					box.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { separator });
				}
				int internalOffset = titleHeigth + 2;
				rows.Sort(new RowComparer());
				foreach (PropertyRow property in rows)
					internalOffset = property.AddToLayoutControl(container, layoutControl, box, internalOffset, controlSize);
				return offset + internalOffset;
			}
		}
		class PropertyRow {
			LayoutControlItem titleLayoutItem;
			LayoutControlItem editorLayoutItem;
			int sortingRank;
			Control control;
			LabelControl title;
			public LabelControl Title { get { return title; } }
			public int SortingRank { get { return sortingRank; } }
			public PropertyRow(string titleStr, Control control, int sortingRank) {
				this.control = control;
				this.sortingRank = sortingRank;
				if (!string.IsNullOrEmpty(titleStr)) {
					title = new DevExpress.XtraEditors.LabelControl();
					title.Appearance.TextOptions.HAlignment = HorzAlignment.Near;
					title.Appearance.TextOptions.VAlignment = VertAlignment.Center;
					title.AutoSizeInLayoutControl = false;
					title.AutoSizeMode = LabelAutoSizeMode.None;
					title.Text = titleStr + ":";
					titleLayoutItem = new LayoutControlItem();
					titleLayoutItem.SizeConstraintsType = SizeConstraintsType.Custom;
					titleLayoutItem.FillControlToClientArea = false;
					titleLayoutItem.ControlAlignment = ContentAlignment.MiddleRight;
					titleLayoutItem.TextVisible = false;
				}
				editorLayoutItem = new LayoutControlItem();
				editorLayoutItem.TextVisible = false;
			}
			public int AddToLayoutControl(IContainer container, LayoutControl layoutControl, LayoutControlGroup group, int offset, Size controlSize) {
				container.Add(control);
				if (title != null) {
					container.Add(title);
					layoutControl.Controls.Add(title);
					titleLayoutItem.Location = new Point(0, offset);
					titleLayoutItem.MaxSize = new Size(controlSize.Width / 2, 0);
					titleLayoutItem.MinSize = new Size(controlSize.Width / 2, 0);
					titleLayoutItem.Padding = new XtraLayout.Utils.Padding(12, 2, 3, 3);
					group.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { titleLayoutItem });
					titleLayoutItem.Control = title;
					titleLayoutItem.TextVisible = false;
				}
				layoutControl.Controls.Add(control);
				editorLayoutItem.Location = new Point(controlSize.Width / 2 + 2, offset);
				editorLayoutItem.Padding = new XtraLayout.Utils.Padding(2, 12, 3, 3);
				group.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { editorLayoutItem });
				editorLayoutItem.Control = control;
				editorLayoutItem.TextVisible = false;
				editorLayoutItem.SizeConstraintsType = SizeConstraintsType.Custom;
				editorLayoutItem.ControlMaxSize = new Size(0, 20);
				editorLayoutItem.ControlMinSize = new Size(controlSize.Width / 2, 20);
				editorLayoutItem.MaxSize = new Size(0, 26);
				editorLayoutItem.MinSize = new Size(controlSize.Width / 4, 26);
				return offset + control.Height;
			}
		}
		const int horizontalConst = 30;
		bool rebuild;
		Control _containerControl = null;
		ElementsOptionsControlBase optionsControl;
		Type modelType;
		LayoutControl layoutControl;
		int horizontalOffset = horizontalConst;
		int verticalOffset = 15;
		int horizontalAddition = 10;
		Dictionary<string, Group> groups;
		public static Adapter CreateAdapter(Type type, bool throwException = true) {
			if (type.Equals(typeof(ChartAppearanceModel)))
				return new ChartAppearanceAdapter();
			if (type.Equals(typeof(string[])))
				return new LinesAdapter();
			if (type.Equals(typeof(AxisXBaseModel)))
				return new AxisAdapter();
			if (type.Equals(typeof(AxisYBaseModel)))
				return new AxisAdapter();
			if (type.Equals(typeof(SwiftPlotDiagramAxisXBaseModel)))
				return new SwiftPlotAxisAdapter();
			if (type.Equals(typeof(SwiftPlotDiagramAxisYBaseModel)))
				return new SwiftPlotAxisAdapter();
			if (type.Equals(typeof(Palette)))
				return new PaletteAdapter();
			if (type.Equals(typeof(XYDiagramPaneBaseModel)))
				return new PaneAdapter();
			if (type.Equals(typeof(Color)))
				return new ColorAdapter();
			if (type.Equals(typeof(Size)))
				return new SizeAdapter();
			if (type.Equals(typeof(DefaultBoolean)))
				return new DefaultBooleanAdapter();
			if (type.Equals(typeof(string)))
				return new StringAdapter();
			if (type.Equals(typeof(object)))
				return new StringAdapter();
			if (type.Equals(typeof(Boolean)))
				return new BooleanAdapter();
			if (type.Equals(typeof(Int32)))
				return new IntegerAdapter();
			if (type.Equals(typeof(Byte)))
				return new ByteAdapter();
			if (type.Equals(typeof(double)))
				return new DoubleAdapter();
			if (type.IsEnum)
				return new EnumAdapter();
			if (throwException)
				throw new Exception(string.Format("Unable to create adapter for '{0}' type", type.ToString()));
			return null;
		}
		Control ContainerControl {
			get { return _containerControl; }
			set { _containerControl = value; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool Rebuild {
			get { return rebuild; }
			set {
				Changing();
				if (value)
					Rebuld();
				rebuild = value;
				Changed();
			}
		}
		public override ISite Site {
			get { return base.Site; }
			set {
				base.Site = value;
				if (value == null) {
					return;
				}
				IDesignerHost host = value.GetService(
					typeof(IDesignerHost)) as IDesignerHost;
				if (host != null) {
					IComponent componentHost = host.RootComponent;
					if (componentHost is Control)
						ContainerControl = componentHost as Control;
					optionsControl = host.RootComponent as ElementsOptionsControlBase;
					Assembly assembly = Assembly.GetAssembly(typeof(UIOptionsBuilder));
					Type optionControlType = assembly.GetType("DevExpress.XtraCharts.Designer.Native." + host.RootComponentClassName);
					if (optionControlType != null) {
						object[] attributes = optionControlType.GetCustomAttributes(typeof(ModelBinding), true);
						if (attributes != null && attributes.Length > 0) {
							ModelBinding elementOption = attributes[0] as ModelBinding;
							this.modelType = elementOption.ModelType;
						}
					}
				}
			}
		}
		Group GetGroup(PropertyInfo property, string prefix) {
			PropertyForOptions attribute = ReflectionHelper.GetAttribute<PropertyForOptions>(property);
			if (attribute == null)
				return null;
			return GetGroup(attribute.GroupName, prefix, attribute.GroupSortingRank);
		}
		Group GetGroup(PropertyInfo property) {
			return GetGroup(property, "");
		}
		Group GetGroup(string groupName, string prefix, int sortingRank) {
			if (!groups.ContainsKey(groupName)) {
				Group group = new Group(groupName, prefix, sortingRank);
				groups.Add(groupName, group);
			}
			return groups[groupName];
		}
		string GetEditorTitle(PropertyInfo property, Type modelType) {
			ModelOf attribute = ReflectionHelper.GetAttribute<ModelOf>(modelType);
			string name = property.Name;
			if (ReflectionHelper.HasAttribute<DesignerDisplayName>(property))
				name = ReflectionHelper.GetAttribute<DesignerDisplayName>(property).Name;
			if (attribute == null)
				return name;
			PropertyInfo chartElementProperty = attribute.ChartElementType.GetProperty(property.Name, BindingFlags.DeclaredOnly);
			if (chartElementProperty == null)
				return name;
			DXDisplayNameAttribute nameAttribute = ReflectionHelper.GetAttribute<DXDisplayNameAttribute>(chartElementProperty);
			if (nameAttribute == null)
				return name;
			return nameAttribute.GetLocalizedDisplayName();
		}
		Control CreateControlForProperty(PropertyInfo property) {
			if (!property.CanRead)
				return null;
			if (ReflectionHelper.HasAttribute<UseEditor>(property))
				return GetControlByAttribute(property);
			if (property.PropertyType.Equals(typeof(Size)))
				return GetControlForSize(property);
			if (property.PropertyType.Equals(typeof(string[])))
				return GetControlForLines(property);
			if (property.PropertyType.Equals(typeof(XYDiagramPaneBaseModel)))
				return GetControlForPane(property);
			if (property.PropertyType.Equals(typeof(AxisXBaseModel)))
				return GetControlForAxis(property, true);
			if (property.PropertyType.Equals(typeof(AxisYBaseModel)))
				return GetControlForAxis(property, false);
			if (property.PropertyType.Equals(typeof(SwiftPlotDiagramAxisXBaseModel)))
				return GetControlForSwiftPlotAxis(property, true);
			if (property.PropertyType.Equals(typeof(SwiftPlotDiagramAxisYBaseModel)))
				return GetControlForSwiftPlotAxis(property, false);
			if (property.PropertyType.Equals(typeof(ChartAppearanceModel)))
				return GetControlForAppearance(property);
			if (property.PropertyType.Equals(typeof(Palette)))
				return GetControlForPalette(property);
			if (property.PropertyType.Equals(typeof(Color)))
				return GetControlForColor(property);
			if (property.PropertyType.Equals(typeof(DefaultBoolean)))
				return GetControlForDefaultBoolean(property);
			if (property.PropertyType.Equals(typeof(string)))
				return GetControlForString(property);
			if (property.PropertyType.Equals(typeof(object)))
				return GetControlForString(property);
			if (property.PropertyType.Equals(typeof(Int32)))
				return GetControlForInteger(property);
			if (property.PropertyType.Equals(typeof(Byte)))
				return GetControlForByte(property);
			if (property.PropertyType.Equals(typeof(Double)))
				return GetControlForDouble(property);
			if (property.PropertyType.Equals(typeof(Boolean)))
				return GetControlForBoolean(property);
			if (ReflectionHelper.HasAttribute<ShowAsGallery>(property))
				return GetControlForEnumAsGallery(property);
			if (property.PropertyType.IsEnum)
				return GetControlForEnum(property);
			return null;
		}
		Control GetControlByAttribute(PropertyInfo property) {
			UseEditor useEditor = ReflectionHelper.GetAttribute<UseEditor>(property);
			Type controlType = useEditor.EditorType;
			Type adapterType = useEditor.AdaptorType;
			object tempControl = Activator.CreateInstance(controlType);
			object tempAdapter = Activator.CreateInstance(adapterType);
			Control control = tempControl as Control;			
			if (control == null)
				return null;
			control.Location = new System.Drawing.Point(verticalOffset, horizontalOffset);
			horizontalOffset += horizontalAddition;
			control.Text = string.Empty;
			control.Name = property.Name;
			return control;
		}
		Control GetControlForDefaultBoolean(PropertyInfo property) {
			CheckEdit checkBox = new CheckEdit();
			checkBox.Properties.AllowGrayed = true;
			checkBox.AutoSize = true;
			checkBox.Location = new System.Drawing.Point(verticalOffset, horizontalOffset);
			horizontalOffset += horizontalAddition;
			checkBox.Text = string.Empty;
			checkBox.Name = property.Name;
			return checkBox;
		}
		Control GetControlForEnum(PropertyInfo property) {
			ComboBoxEdit comboBox = new ComboBoxEdit();
			comboBox.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
			comboBox.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			comboBox.Tag = property.Name;
			foreach (var value in Enum.GetValues(property.PropertyType))
				comboBox.Properties.Items.Add(new EnumElementPresentation(EnumLocalizationProvider.GetLocalizationName(value), property.PropertyType, value));
			comboBox.SelectedIndex = 0;
			return comboBox;
		}
		Control GetControlForBoolean(PropertyInfo property) {
			CheckEdit checkBox = new CheckEdit();
			checkBox.AutoSize = true;
			checkBox.Location = new System.Drawing.Point(verticalOffset, horizontalOffset);
			horizontalOffset += horizontalAddition;
			checkBox.Text = string.Empty;
			checkBox.Name = property.Name;
			return checkBox;
		}
		Control GetControlForEnumAsGallery(PropertyInfo property) {
			EnumGalleryControl control = new EnumGalleryControl();
			control.Provider = GetGalleryProvider(property);
			control.Location = new System.Drawing.Point(verticalOffset, horizontalOffset);
			if (control.Provider == null) {
				throw new Exception(string.Format("Incorrect attribute 'ShowAsGallery' on provider '{0}' ", property.Name));
			}
			horizontalOffset += horizontalAddition;
			control.Name = property.Name;
			return control;
		}
		Control GetControlForInteger(PropertyInfo property) {
			SpinEdit control = new SpinEdit();
			control.AutoSize = true;
			control.Properties.IsFloatValue = false;
			control.Location = new System.Drawing.Point(verticalOffset, horizontalOffset);
			horizontalOffset += horizontalAddition;
			control.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "0", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, null, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), null, "", null, null, false)});
			control.Name = property.Name;
			return control;
		}
		Control GetControlForByte(PropertyInfo property) {
			SpinEdit control = new SpinEdit();
			control.AutoSize = true;
			control.Properties.IsFloatValue = false;
			control.Location = new System.Drawing.Point(verticalOffset, horizontalOffset);
			control.Properties.MaxValue = 255;
			control.Properties.MinValue = 0;
			horizontalOffset += horizontalAddition;
			control.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "0", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, null, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), null, "", null, null, false)});
			control.Name = property.Name;
			return control;
		}
		Control GetControlForDouble(PropertyInfo property) {
			SpinEdit control = new SpinEdit();
			control.AutoSize = true;
			control.Properties.IsFloatValue = true;
			control.Properties.Increment = 0.1M;
			control.Location = new System.Drawing.Point(verticalOffset, horizontalOffset);
			control.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
			horizontalOffset += horizontalAddition;
			control.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "0", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, null, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), null, "", null, null, false)});
			control.Name = property.Name;
			return control;
		}
		Control GetControlForString(PropertyInfo property) {
			TextEdit control = new TextEdit();
			control.AutoSize = true;
			control.Location = new System.Drawing.Point(verticalOffset, horizontalOffset);
			horizontalOffset += horizontalAddition;
			control.Name = property.Name;
			return control;
		}
		Control GetControlForColor(PropertyInfo property) {
			ColorPickEdit control = new ColorPickEdit();
			control.Properties.AutomaticColor = Color.Empty;
			control.AutoSize = true;
			control.Location = new System.Drawing.Point(verticalOffset, horizontalOffset);
			horizontalOffset += horizontalAddition;
			control.Name = property.Name;
			return control;
		}
		Control GetControlForSize(PropertyInfo property) {
			SizeControl control = new SizeControl();
			control.Location = new System.Drawing.Point(verticalOffset, horizontalOffset);
			horizontalOffset += horizontalAddition;
			control.Name = property.Name;
			return control;
		}
		Control GetControlForAppearance(PropertyInfo property) {
			AppearanceGalleryControl control = new AppearanceGalleryControl();
			control.Location = new System.Drawing.Point(verticalOffset, horizontalOffset);
			horizontalOffset += horizontalAddition;
			control.Text = property.Name;
			control.Name = property.Name;
			return control;
		}
		Control GetControlForPalette(PropertyInfo property) {
			PaletteGalleryControl control = new PaletteGalleryControl();
			control.Location = new System.Drawing.Point(verticalOffset, horizontalOffset);
			horizontalOffset += horizontalAddition;
			control.Text = property.Name;
			control.Name = property.Name;
			return control;
		}
		Control GetControlForPane(PropertyInfo property) {
			PaneControl control = new PaneControl();
			control.Location = new System.Drawing.Point(verticalOffset, horizontalOffset);
			horizontalOffset += horizontalAddition;
			control.Text = property.Name;
			control.Name = property.Name;
			return control;
		}
		Control GetControlForAxis(PropertyInfo property, bool isArgumentAxis) {
			AxisControl control = new AxisControl();
			control.Location = new System.Drawing.Point(verticalOffset, horizontalOffset);
			horizontalOffset += horizontalAddition;
			control.Text = property.Name;
			control.Name = property.Name;
			control.IsArgumentAxis = isArgumentAxis;
			return control;
		}
		Control GetControlForSwiftPlotAxis(PropertyInfo property, bool isArgumentAxis) {
			SwiftPlotAxisControl control = new SwiftPlotAxisControl();
			control.Location = new System.Drawing.Point(verticalOffset, horizontalOffset);
			horizontalOffset += horizontalAddition;
			control.Text = property.Name;
			control.Name = property.Name;
			control.IsArgumentAxis = isArgumentAxis;
			return control;
		}
		Control GetControlForLines(PropertyInfo property) {
			MemoEdit control = new MemoEdit();
			control.Location = new System.Drawing.Point(verticalOffset, horizontalOffset);
			horizontalOffset += horizontalAddition;
			control.Text = property.Name;
			control.Name = property.Name;
			control.Properties.EditValueChangedDelay = 300;
			control.Properties.ScrollBars = ScrollBars.None;
			control.Properties.EditValueChangedFiringMode = XtraEditors.Controls.EditValueChangedFiringMode.Buffered;
			return control;
		}
		GalleryElementsProvider GetGalleryProvider(PropertyInfo property) {
			object[] attributes = property.GetCustomAttributes(typeof(ShowAsGallery), false);
			if (attributes == null || attributes.Length < 1)
				return null;
			Type providerType = (attributes[0] as ShowAsGallery).ProviderType;
			if (typeof(GalleryElementsProvider).IsAssignableFrom(providerType))
				return Activator.CreateInstance(providerType) as GalleryElementsProvider;
			else
				return null;
		}
		bool IsAssignableFromCollection(Type type) {
			return typeof(ChartElementNamedCollectionModel).IsAssignableFrom(type);
		}
		bool CheckShowInTree(Type type) {
			if (!type.IsClass || type.Equals(typeof(DesignerChartElementModelBase)) || type.Equals(typeof(object)))
				return false;
			if (ReflectionHelper.HasAttribute<HasOptionsControl>(type))
				return true;
			return CheckShowInTree(type.BaseType);
		}
		void Clear() {
			if (optionsControl != null)
				optionsControl.PropertyLinks.Clear();
			ContainerControl.Controls.Clear();
			foreach (IComponent component in ContainerControl.Container.Components) {
				if (component is UIOptionsBuilder || component is ElementsOptionsControlBase)
					continue;
				ContainerControl.Container.Remove(component);
			}
		}
		void AddControl(Control control) {
			if (ContainerControl == null || control == null)
				return;
			ContainerControl.Container.Add(control);
			ContainerControl.Controls.Add(control);
		}
		void Changing() {
			IComponentChangeService cs = (IComponentChangeService)GetService(typeof(IComponentChangeService));
			if (cs != null)
				cs.OnComponentChanging(this, null);
		}
		void Changed() {
			IComponentChangeService cs = (IComponentChangeService)GetService(typeof(IComponentChangeService));
			if (cs != null)
				cs.OnComponentChanged(this, null, null, null);
		}
		void CreateEditorsForModel(PropertyInfo property, string groupPrefix) {
			PropertyInfo[] nestedProperties = property.PropertyType.GetProperties();
			if (ReflectionHelper.HasAttribute<AllocateToGroup>(property)) {
				string groupName = ReflectionHelper.GetAttribute<AllocateToGroup>(property).GroupName;
				int sortingRank = ReflectionHelper.GetAttribute<AllocateToGroup>(property).SortingRank;
				foreach (PropertyInfo nestedProperty in nestedProperties) {
					if (ReflectionHelper.HasAttribute<PropertyForOptions>(nestedProperty) && !IsAssignableFromCollection(nestedProperty.PropertyType)) {
						if (ReflectionHelper.HasAttribute<GenerateHeritableProperties>(nestedProperty.PropertyType)) {
							Type[] descendants = ReflectionHelper.GetTypeDescendants(nestedProperty.PropertyType);
							List<PropertyInfo> properties = new List<PropertyInfo>();
							foreach (Type descendant in descendants)
								properties.AddRange(descendant.GetProperties().Where(info => ReflectionHelper.HasAttribute<PropertyForOptions>(info)));
							foreach (PropertyInfo generatingProperty in properties) {
								Group group = GetGroup(groupName, groupPrefix, sortingRank);
								CreateEditorForProperty(generatingProperty, group, property.Name + "." + generatingProperty.ReflectedType.Name + ".");
							}
						}
						else {
							CreateEditorForProperty(nestedProperty, GetGroup(groupName, groupPrefix, sortingRank), property.Name + ".");
						}
					}
				}
			}
			else {
				foreach (PropertyInfo nestedProperty in nestedProperties)
					if (ReflectionHelper.HasAttribute<PropertyForOptions>(nestedProperty) && !IsAssignableFromCollection(nestedProperty.PropertyType))
						CreateEditorForProperty(nestedProperty, GetGroup(property, groupPrefix), property.Name + ".");
			}
		}
		string SplitByWords(string title) {
			Regex regex = new Regex(@"(?<=\p{Lu})(?=\p{Lu}\P{Lu})|(?<=[\P{Lu}-[\s]])(?=\p{Lu})|(?<=[\w-[\d]])(?=[\W\d-[\s]])");
			return regex.Replace(title, " ");
		}
		void CreateEditorForProperty(PropertyInfo property, Group group, string namePrefix) {
			if (!property.CanRead || group == null || !ReflectionHelper.HasAttribute<PropertyForOptions>(property))
				return;
			Control editor = CreateControlForProperty(property);
			string editorTitle = GetEditorTitle(property, modelType);
			editorTitle = SplitByWords(editorTitle);
			PropertyLink propertyLink = null;
			if (optionsControl != null) {
				Adapter adapter;
				Type adapterType = null;
				if (ReflectionHelper.HasAttribute<ShowAsGallery>(property)) {
					adapter = new EnumGalleryAdapter();
					adapterType = typeof(EnumGalleryAdapter);
				}
				else if (ReflectionHelper.HasAttribute<UseEditor>(property)) {
					adapterType = ReflectionHelper.GetAttribute<UseEditor>(property).AdaptorType;
					object tempAdapter = Activator.CreateInstance(adapterType);
					adapter = tempAdapter as Adapter;
					if (adapter == null)
						return;
				}
				else
					adapter = CreateAdapter(property.PropertyType);
				propertyLink = new PropertyLink(namePrefix + property.Name, property.PropertyType, adapter, editor, adapterType);
				optionsControl.PropertyLinks.Add(propertyLink);
			}
			PropertyForOptions attribute = ReflectionHelper.GetAttribute<PropertyForOptions>(property);
			LabelControl title = group.AddProperty(property, editorTitle, editor, attribute.SortingRank);
			if (propertyLink != null)
				propertyLink.TitleControl = title;
		}
		public virtual void Rebuld() {
			if (ContainerControl == null)
				throw new Exception("This isn't Control to add components");
			if (modelType == null)
				throw new Exception("This isn't ModelType to dig properties");
			if (!typeof(DesignerChartElementModelBase).IsAssignableFrom(modelType))
				throw new Exception("ModelType does not inherit from DesignerChartElementModelBase");
			PropertyInfo[] properties = modelType.GetProperties();
			Clear();
			layoutControl = new LayoutControl();
			layoutControl.AllowCustomization = false;
			layoutControl.Dock = DockStyle.Fill;
			ContainerControl.Container.Add(layoutControl);
			((System.ComponentModel.ISupportInitialize)(layoutControl)).BeginInit();
			layoutControl.SuspendLayout();
			LayoutControlGroup root = new LayoutControlGroup();
			root.Padding = new XtraLayout.Utils.Padding(0);
			root.GroupBordersVisible = false;
			root.TextVisible = false;
			layoutControl.Root = root;
			groups = new Dictionary<string, Group>();
			string groupPrefix = string.Empty;
			if (ReflectionHelper.HasAttribute<GroupPrefix>(modelType))
				groupPrefix = ReflectionHelper.GetAttribute<GroupPrefix>(modelType).Prefix.ToUpper();
			foreach (PropertyInfo property in properties) {
				if ((!ReflectionHelper.HasAttribute<PropertyForOptions>(property) || IsAssignableFromCollection(property.PropertyType) || CheckShowInTree(property.PropertyType)) && !ReflectionHelper.HasAttribute<UseAsSimpleProperty>(property))
					continue;
				if (typeof(DesignerChartElementModelBase).IsAssignableFrom(property.PropertyType) && !ReflectionHelper.HasAttribute<UseAsSimpleProperty>(property)) {
					if (typeof(SeriesViewModelBase).IsAssignableFrom(property.PropertyType)) {
						Group viewGroup = new Group("", "", 20);
						groups.Add("SeriesView", viewGroup);
					}
					else
						CreateEditorsForModel(property, groupPrefix);
				}
				else
					CreateEditorForProperty(property, GetGroup(property, groupPrefix), "");
			}
			List<Group> sortedGroup = new List<Group>();
			foreach (Group group in groups.Values)
				sortedGroup.Add(group);
			bool isSeriesView = typeof(SeriesViewModelBase).IsAssignableFrom(modelType);
			List<TitleElement> titles = new List<TitleElement>();
			int offset = horizontalOffset;
			sortedGroup.Sort(new GroupComparer());
			for (int i = 0; i < sortedGroup.Count; i++) {
				Group group = sortedGroup[i];
				offset = group.AddToLayoutControl(ContainerControl.Container, layoutControl, root, offset, ContainerControl.Size, titles, i == 0 && !isSeriesView);
			}
			optionsControl.Titles.AddRange(titles);
			EmptySpaceItem emptySpace = new DevExpress.XtraLayout.EmptySpaceItem();
			emptySpace.Location = new Point(0, offset);
			root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { emptySpace });
			ContainerControl.Controls.Add(layoutControl);
			((System.ComponentModel.ISupportInitialize)(layoutControl)).EndInit();
			layoutControl.ResumeLayout(false);
			groups = null;
		}
		public virtual void BeginInit() { }
		public virtual void EndInit() { }
	}
	public class EnumElementPresentation {
		string name;
		Type enamType;
		object value;
		public object Value {
			get { return this.value; }
			set {
				this.value = value;
			}
		}
		public string Name {
			get { return this.name; }
			set {
				this.name = value;
			}
		}
		public Type EnamType {
			get { return this.enamType; }
			set {
				this.enamType = value;
			}
		}
		public EnumElementPresentation(string name, Type enamType, object value) {
			this.value = value;
			this.name = name;
			this.enamType = enamType;
		}
		public EnumElementPresentation() {
		}
		public override string ToString() { return this.name; }
	}
	public static class EnumLocalizationProvider {
		public static string GetLocalizationName(object enumValue) {
			return enumValue.ToString();
		}
	}
	public class PropertyLink : INotified {
		string[] propertyName;
		string propertyDisplayName;
		Control editor;
		Type propertyType;
		Adapter adapter;
		IMessageListener listener;
		Type adapterType;
		Control title;
		string joinedName;
		Adapter Adapter {
			get { return adapter; }
			set {
				if (value == null && adapter != null) {
					adapter.Notified = null;
					if (editor != null)
						adapter.UnsubscribeToChangedEvent(editor);
					adapter = null;
				}
				else {
					adapter = value;
					if (this.adapter != null)
						this.adapter.Notified = this;
					if (editor != null)
						adapter.SubscribeToChangedEvent(editor);
				}
			}
		}
		public string JoinedName {
			get {
				if (joinedName == null)
					joinedName = string.Join(".", propertyName);
				return joinedName;
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public string PropertyName {
			get { return propertyName[propertyName.Length - 1]; }
			set { propertyName[propertyName.Length - 1] = value; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public string[] PropertyFullName {
			get { return propertyName; }
			set { propertyName = value; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public Type PropertyType {
			get { return propertyType; }
			set {
				propertyType = value;
				Adapter adapter = UIOptionsBuilder.CreateAdapter(propertyType, false);
				if (Adapter == null && adapter != null)
					Adapter = adapter;
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public Type AdapterType {
			get { return adapterType; }
			set {
				if (value != null && typeof(Adapter).IsAssignableFrom(value)) {
					Adapter = Activator.CreateInstance(value) as Adapter;
					adapterType = value;
				}
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public Control Editor {
			get { return editor; }
			set {
				editor = value;
				if (adapter != null)
					adapter.SubscribeToChangedEvent(editor);
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string DisplayName {
			set {
				propertyDisplayName = value;
				if (title != null)
					title.Text = propertyDisplayName;
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public Control TitleControl {
			get { return title; }
			set {
				title = value;
				if (title != null && !string.IsNullOrEmpty(propertyDisplayName))
					title.Text = propertyDisplayName;
			}
		}
		public PropertyLink(string propertyName, Type type, Adapter adapter, Control control) {
			if (propertyName.Contains("."))
				this.propertyName = propertyName.Split('.');
			else
				this.propertyName = new string[] { propertyName };
			this.propertyType = type;
			this.editor = control;
			this.Adapter = adapter;
		}
		public PropertyLink(string propertyName, Type type, Adapter adapter, Control control, Type adapterType)
			: this(propertyName, type, adapter, control) {
			this.adapterType = adapterType;
		}
		public PropertyLink(string propertyName, Type type, Adapter adapter, Control control, Type adapterType, Control title)
			: this(propertyName, type, adapter, control, adapterType) {
			this.title = title;
		}
		public PropertyLink() {
			this.propertyName = new string[] { "" };
		}
		LayoutControlItem GetLayoutItem(Control control) {
			LayoutControl layoutControl = (LayoutControl)control.Parent;
			return layoutControl.GetItemByControl(control);
		}
		internal void SetEditorActivity(ActivityMessage message) {
			switch (message.EditorActivity) {
				case EditorActivity.Enable: editor.Enabled = message.Activity; break;
				case EditorActivity.Visible: {
					LayoutControlItem controlItem = GetLayoutItem(editor);
					LayoutControlItem labelItem = GetLayoutItem(title);
					controlItem.Visibility = labelItem.Visibility = message.Activity ? LayoutVisibility.Always : LayoutVisibility.Never;
					break;
				}
			}
		}
		internal string GetModelPath() {
			if (propertyName.Length == 1)
				return ".";
			string name = "";
			foreach (string str in propertyName)
				name = name + "." + str;
			return name;
		}
		public virtual void UpdateEditor(object value) {
			if (adapter != null) {
				object oldValue = adapter.GetEditorValue(this.editor);
				adapter.Lock();
				adapter.UpdateEditor(value, editor);
				adapter.Unlock();
			}
		}
		public virtual object GetEditorValue() {
			if (adapter != null)
				return adapter.GetEditorValue(editor);
			return null;
		}
		public void SetListener(IMessageListener listener) {
			this.listener = listener;
		}
		void INotified.Notify() {
			if (listener != null)
				listener.AcceptMessage(new ViewMessage(PropertyFullName, GetEditorValue()));
		}
		bool ShouldSerializeAdapterType() {
			return adapterType != null;
		}
		bool ShouldSerializeTitleControl() {
			return title != null;
		}
		bool ShouldSerializePropertyName() {
			return propertyName.Length == 1;
		}
		bool ShouldSerializePropertyFullName() {
			return propertyName.Length > 1;
		}
	}
	public class ViewMessage {
		readonly string[] name;
		readonly object value;
		string joinedName;
		public string Name {
			get { return name[name.Length - 1]; }
		}
		public string JoinedName {
			get {
				if (joinedName == null)
					joinedName = string.Join(".", FullName);
				return joinedName;
			}
		}
		public string[] FullName {
			get { return name; }
		}
		public object Value {
			get { return value; }
		}
		public bool SimpleName {
			get { return name.Length == 1; }
		}
		public ViewMessage(string name, object value) {
			this.name = new string[] { name };
			this.value = value;
		}
		public ViewMessage(string[] name, object value) {
			this.name = name;
			this.value = value;
		}
	}
	public class ActivityMessage {
		readonly string name;
		string[] path;
		readonly bool activity;
		readonly EditorActivity editorActivity;
		public string Name { get { return name; } }
		public string[] Path { get { return path; } }
		public bool Activity { get { return activity; } }
		public EditorActivity EditorActivity { get { return editorActivity; } }
		public ActivityMessage(string name, EditorActivity editorActivity, bool activity) {
			this.name = name;
			this.editorActivity = editorActivity;
			this.activity = activity;
		}
		public void AddPath(List<string> path) {
			this.path = path.ToArray();
		}
	}
}
