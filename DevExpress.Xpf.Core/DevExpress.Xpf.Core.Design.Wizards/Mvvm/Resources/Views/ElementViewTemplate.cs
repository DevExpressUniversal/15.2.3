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

namespace DevExpress.Xpf.Core.Design.Wizards.Mvvm.Resources.Views
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Linq;
	using DevExpress.Design.Mvvm;
	using DevExpress.Xpf.Core.Design.Wizards.Mvvm.EntityFramework;
	using DevExpress.Xpf.Core.Design.Wizards.Mvvm;
	using DevExpress.Xpf.Core.Design.Wizards.Mvvm.ViewModelData;
	using DevExpress.Xpf.Core.Native;
	using DevExpress.Mvvm.UI.Native.ViewGenerator;
	using DevExpress.Mvvm.Native;
	using DevExpress.Xpf.Internal.EntityFrameworkWrappers;
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public partial class ElementViewTemplate : ElementViewTemplateBase
	{
		public virtual string TransformText()
		{
	T4TemplateInfo templateInfo = this.GetTemplateInfo();
	EntityViewModelData viewModelData = templateInfo.Properties["IViewModelInfo"] as EntityViewModelData;
	UIType uiType = (UIType)templateInfo.Properties["UIType"];
	string viewName = templateInfo.Properties["ViewName"].ToString();	
	string localNamespace = templateInfo.Properties["Namespace"].ToString();
	string viewFullName = localNamespace + "." + viewName;
	var editorInfos = (PropertyEditorGroupInfo)templateInfo.Properties["EditorInfos"];
	var allInfos = editorInfos.Groups.Flatten(g => g.Groups).Concat(new[] { editorInfos }).SelectMany(g => g.Items);
	bool hasHiddenEditors = allInfos.Any(i => i.Property.Attributes.Hidden());
	XamlNamespaces xamlNamespaces = templateInfo.Properties["XamlNamespaces"] as XamlNamespaces;
	string viewModelPrefix = templateInfo.Properties["viewModelPrefix"].ToString();
	var context = (TemplateGenerationContext)templateInfo.Properties["TemplateGenerationContext"];
	string defaultNamespace = (string)templateInfo.Properties["DefaultNamespacePrefix"];
			this.Write("<UserControl x:Class=\"");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewFullName));
			this.Write("\"\r\n    ");
PasteXamlNamespaces(xamlNamespaces);
			this.Write("    xmlns:view=\"clr-namespace:");
			this.Write(this.ToStringHelper.ToStringWithCulture(defaultNamespace));
			this.Write(this.ToStringHelper.ToStringWithCulture(localNamespace));
			this.Write("\"\r\n");
this.ExecuteEntityViewHook(TemplatesCodeGen.STR_EntityViewHook_GenerateAdditionalXmlNamespaces);
			this.Write("    mc:Ignorable=\"");
			this.Write(this.ToStringHelper.ToStringWithCulture(xamlNamespaces.GetPrefix(XamlNamespaces.xmlns_blend)));
			this.Write("\"\r\n    d:DesignHeight=\"400\" d:DesignWidth=\"600\"\r\n");
if(viewModelData.UseProxyFactory) {
			this.Write("    DataContext=\"{dxmvvm:ViewModelSource ");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelPrefix));
			this.Write(":");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelData.Name));
			this.Write("}\"");
}
			this.Write(">\r\n");
if(!viewModelData.UseProxyFactory) {
			this.Write("    <UserControl.DataContext>\r\n        <");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelPrefix));
			this.Write(":");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelData.Name));
			this.Write("/>\r\n    </UserControl.DataContext>\r\n");
}
if(viewModelData.SupportServices) {
			this.Write("    <dxmvvm:Interaction.Behaviors>\r\n        <dx:DXMessageBoxService/>\r\n        <d" +
					"xmvvm:EventToCommand Event=\"Loaded\" Command=\"{Binding OnLoadedCommand}\" />\r\n    " +
					"</dxmvvm:Interaction.Behaviors>\r\n");
}
			this.Write(@"    <Grid>
        <DockPanel>
            <dxr:RibbonControl RibbonStyle=""Office2010"" DockPanel.Dock=""Top"" AllowCustomization=""False"">
                <dxr:RibbonDefaultPageCategory Caption=""defaultCategory"">
                    <dxr:RibbonPage Caption=""");
if(uiType == UIType.OutlookInspired) {
			this.Write("HOME");
} else {
			this.Write("Home");
}
			this.Write("\">\r\n                        <dxr:RibbonPageGroup Caption=\"");
			this.Write(this.ToStringHelper.ToStringWithCulture(TemplatesCodeGen.GetCaption(viewModelData.EntityTypeName)));
			this.Write(" Tasks\">\r\n");
foreach(CommandInfo command in viewModelData.NonLayoutCommands) {
			this.Write("                            <dxb:BarButtonItem Content=\"");
			this.Write(this.ToStringHelper.ToStringWithCulture(command.Caption));
			this.Write("\" Command=\"{Binding ");
			this.Write(this.ToStringHelper.ToStringWithCulture(command.CommandPropertyName));
			this.Write("}\" ");
if(command.HasGlyphs()){
			this.Write("LargeGlyph=\"");
			this.Write(this.ToStringHelper.ToStringWithCulture(command.LargeGlyph));
			this.Write("\" Glyph=\"");
			this.Write(this.ToStringHelper.ToStringWithCulture(command.SmallGlyph));
			this.Write("\"");
}
			this.Write("/>\r\n");
}
			this.Write(@"                        </dxr:RibbonPageGroup>
                        <dxr:RibbonPageGroup Caption=""Layout"">
                            <dxb:BarCheckItem Content=""Customize"" IsChecked=""{Binding IsCustomization, ElementName=layoutControl}"" LargeGlyph=""{dx:DXImage Image=PageSetup_32x32.png}"" Glyph=""{dx:DXImage Image=PageSetup_16x16.png}"" />
");
foreach(CommandInfo command in viewModelData.LayoutCommands) {
			this.Write("                            <dxb:BarButtonItem Content=\"");
			this.Write(this.ToStringHelper.ToStringWithCulture(command.Caption));
			this.Write("\" Command=\"{Binding ");
			this.Write(this.ToStringHelper.ToStringWithCulture(command.CommandPropertyName));
			this.Write("}\" ");
if(command.HasGlyphs()){
			this.Write("LargeGlyph=\"");
			this.Write(this.ToStringHelper.ToStringWithCulture(command.LargeGlyph));
			this.Write("\" Glyph=\"");
			this.Write(this.ToStringHelper.ToStringWithCulture(command.SmallGlyph));
			this.Write("\"");
}
			this.Write("/>\r\n");
}
			this.Write(@"                        </dxr:RibbonPageGroup>
                    </dxr:RibbonPage>
                </dxr:RibbonDefaultPageCategory>
            </dxr:RibbonControl>
            <dxr:RibbonStatusBarControl DockPanel.Dock=""Bottom""/>
            <Grid>
                <dxmvvm:Interaction.Behaviors>
                    <dxmvvm:LayoutSerializationService />
                </dxmvvm:Interaction.Behaviors>
                <dxlc:DataLayoutControl AutoGenerateItems=""False"" CurrentItem=""{Binding ");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelData.EntityPropertyName));
			this.Write(@"}"" x:Name=""layoutControl"" Orientation=""Vertical"" IsEnabled=""{Binding Entity, Converter={dxmvvm:ObjectToBooleanConverter}}"">
                    <dxmvvm:Interaction.Behaviors>
                        <dxmvvm:EventToCommand Event=""{x:Static Binding.SourceUpdatedEvent}"" Command=""{Binding UpdateCommand}"" />
                    </dxmvvm:Interaction.Behaviors>
");
if(!this.ExecuteEntityViewHook(TemplatesCodeGen.STR_EntityViewHook_GenerateCustomLayoutItems)) {
					PushIndent("                    ");
					WriteEditors(editorInfos, viewModelData.EntityPropertyName, viewModelData.EntityTypeName, context, true);
					PopIndent();
this.ExecuteEntityViewHook(TemplatesCodeGen.STR_EntityViewHook_GenerateAdditionalLayoutItems);
AddLookUpTables(context, viewModelData, templateInfo);
if(hasHiddenEditors) {
			this.Write("                    <dxlc:LayoutControl.AvailableItems>\r\n");
					PushIndent("                    ");
					WriteEditors(editorInfos, viewModelData.EntityPropertyName, viewModelData.EntityTypeName, context, false);
					PopIndent();
			this.Write("                    </dxlc:LayoutControl.AvailableItems>\r\n");
}
}
			this.Write("                </dxlc:DataLayoutControl>\r\n            </Grid>\r\n        </DockPan" +
					"el>\r\n    </Grid>\r\n</UserControl>\r\n");
			return this.GenerationEnvironment.ToString();
		}
public void WriteEditors(PropertyEditorGroupInfo rootGroup, string entityPropertyName, string entityName, TemplateGenerationContext context, bool visible) {
	foreach(var info in rootGroup.Items) {
		WriteEditor(info, entityPropertyName, entityName, context, visible);
	}
	int n = 0;
	foreach(var group in rootGroup.Groups) {
		WriteGroup(group, visible, entityPropertyName, entityName, context, ref n);
	}
}
void WriteGroup(PropertyEditorGroupInfo group, bool visible, string entityPropertyName, string entityName, TemplateGenerationContext context, ref int n) {
	n++;
	var items = group.Items.Where(i => i.Property.Attributes.Hidden() == !visible);
	if (items.Any()) 
this.Write("<dxlc:LayoutGroup x:Name=\"layoutGroup");
this.Write(this.ToStringHelper.ToStringWithCulture(n));
this.Write("\" View=\"");
this.Write(this.ToStringHelper.ToStringWithCulture(group.View));
this.Write("\" Orientation=\"");
this.Write(this.ToStringHelper.ToStringWithCulture(group.Orientation));
this.Write("\" Header=\"");
this.Write(this.ToStringHelper.ToStringWithCulture(group.Name));
this.Write("\" >\r\n");
		PushIndent("    ");
		foreach(var info in items) {
			WriteEditor(info, entityPropertyName, entityName, context, visible);
		}
		foreach(var subgroup in group.Groups) {
			WriteGroup(subgroup, visible, entityPropertyName, entityName, context, ref n);
		}
		PopIndent();		
this.Write("</dxlc:LayoutGroup>\r\n");
}
public void WriteEditor(PropertyEditorInfo info, string entityPropertyName, string entityName, TemplateGenerationContext context, bool visible) {
	if(info.Property.Attributes.Hidden() == visible)
		return;
	if(info.IsLookup) {
		if (info.IsReadonly) { 
this.Write("<dxlc:DataLayoutItem Label=\"");
this.Write(this.ToStringHelper.ToStringWithCulture(info.Label));
this.Write("\" Name=\"layoutItem");
this.Write(this.ToStringHelper.ToStringWithCulture(info.Property.Name));
this.Write("\" Binding=\"{Binding ");
this.Write(this.ToStringHelper.ToStringWithCulture(info.Lookup.BindingPath));
this.Write(", UpdateSourceTrigger=PropertyChanged}\" IsReadOnly=\"True\" />\r\n");
	 } else {
			var lookupProp = info.Lookup.ItemsSource;
			var suffix = ".Entities";
			var lookupPropSansEntities = lookupProp.EndsWith(suffix) ? lookupProp.Substring(0, lookupProp.Length - suffix.Length) : lookupProp;
			bool isCompositeKey = false;
			var editValue = entityPropertyName + "." + info.Lookup.ForeignKeyInfo.ForeignKeyPropertyName;
			if (context != null && context.MetadataWorkspace != null) {
				var entityWrapper = TemplatesCodeGen.FindEntityType(context.MetadataWorkspace, entityName);
				var navigationProperty = entityWrapper.NavigationProperties.First(x => x.Name == info.Property.Name);
				var lookupEntityType = navigationProperty.ToEndMember.GetEntityType();
				isCompositeKey = lookupEntityType.KeyMembers.Count() > 1;
				if (isCompositeKey) {
					editValue = lookupPropSansEntities + "Entity";
				}
			}
this.Write("<dxlc:LayoutItem Label=\"");
this.Write(this.ToStringHelper.ToStringWithCulture(info.Label));
this.Write("\" Name=\"layoutItem");
this.Write(this.ToStringHelper.ToStringWithCulture(info.Property.Name));
this.Write("\">\r\n    <dxg:LookUpEdit ItemsSource=\"{Binding ");
this.Write(this.ToStringHelper.ToStringWithCulture(info.Lookup.ItemsSource));
this.Write("}\"\r\n                    EditValue=\"{Binding ");
this.Write(this.ToStringHelper.ToStringWithCulture(editValue));
this.Write(", UpdateSourceTrigger=PropertyChanged, ValidatesOnDataErrors=True, NotifyOnSource" +
		"Updated=True}\"\r\n");
if (!isCompositeKey) {
this.Write("                    ValueMember=\"");
this.Write(this.ToStringHelper.ToStringWithCulture(info.Lookup.ForeignKeyInfo.PrimaryKeyPropertyName));
this.Write("\"\r\n");
}
this.Write("                    DisplayMember=\"");
this.Write(this.ToStringHelper.ToStringWithCulture(info.Lookup.DisplayMemberPropertyName));
this.Write("\"\r\n                    IsTextEditable=\"False\"\r\n                    AllowUpdateTwo" +
		"WayBoundPropertiesOnSynchronization=\"False\">\r\n");
 if(lookupProp.EndsWith(suffix)) {
		lookupProp = lookupPropSansEntities;
this.Write("        <dxg:LookUpEdit.PopupContentTemplate>\r\n            <ControlTemplate Targe" +
		"tType=\"ContentControl\">\r\n                <dxg:GridControl x:Name=\"PART_GridContr" +
		"ol\" ShowBorder=\"False\" ShowLoadingPanel=\"{Binding ");
this.Write(this.ToStringHelper.ToStringWithCulture(lookupProp));
this.Write(".IsLoading}\" />\r\n            </ControlTemplate>\r\n        </dxg:LookUpEdit.PopupCo" +
		"ntentTemplate>\r\n");
 } 
this.Write("    </dxg:LookUpEdit>\r\n</dxlc:LayoutItem>\r\n");
		}
	} else if (info.IsImage) { 
this.Write("<dxlc:LayoutItem Label=\"");
this.Write(this.ToStringHelper.ToStringWithCulture(info.Label));
this.Write("\" Name=\"layoutItem");
this.Write(this.ToStringHelper.ToStringWithCulture(info.Property.Name));
this.Write("\">\r\n    <dxe:ImageEdit EditValue=\"{Binding ");
this.Write(this.ToStringHelper.ToStringWithCulture(entityPropertyName));
this.Write(".");
this.Write(this.ToStringHelper.ToStringWithCulture(info.Property.Name));
this.Write(", UpdateSourceTrigger=PropertyChanged, ValidatesOnDataErrors=True, NotifyOnSource" +
		"Updated=True}\" MaxHeight=\"200\" />\r\n</dxlc:LayoutItem>\r\n");
 } else { 
		string readOnly = null;
		if(info.IsReadonly) {
			readOnly = "True";
		} else if (context != null && context.MetadataWorkspace != null) {
			var entity = TemplatesCodeGen.FindEntityType(context.MetadataWorkspace, entityName);
			var keyMember = entity.KeyMembers.FirstOrDefault(m => m.Name == info.Property.Name);
			if (keyMember != null) {
				readOnly = "{Binding DataContext.IsPrimaryKeyReadOnly, RelativeSource={RelativeSource AncestorType={x:Type dxlc:DataLayoutControl}}}";
			}
		}
this.Write("<dxlc:DataLayoutItem Label=\"");
this.Write(this.ToStringHelper.ToStringWithCulture(info.Label));
this.Write("\" Name=\"layoutItem");
this.Write(this.ToStringHelper.ToStringWithCulture(info.Property.Name));
this.Write("\" Binding=\"{Binding ");
this.Write(this.ToStringHelper.ToStringWithCulture(info.Property.Name));
this.Write(", UpdateSourceTrigger=PropertyChanged}\"");
if(readOnly != null){
this.Write(" IsReadOnly=\"");
this.Write(this.ToStringHelper.ToStringWithCulture(readOnly));
this.Write("\"");
}
this.Write(" />\r\n");
 }
}
void AddLookUpTables(TemplateGenerationContext context, EntityViewModelData viewModelData, T4TemplateInfo templateInfo) {
	var m2ms = new List<Tuple<ManyToManyInfo, NavigationPropertyRuntimeWrapper>>();
	if (context.MetadataWorkspace != null) { 
		var entityTypeWrapper = TemplatesCodeGen.FindEntityType(context.MetadataWorkspace, viewModelData.EntityTypeName);
		foreach(var navigation in entityTypeWrapper.NavigationProperties) {
			var manyToMany = TemplatesCodeGen.FindManyToManyAssociation(context.MetadataWorkspace, entityTypeWrapper.Name, navigation.ToEndMember.GetEntityType().Name);
			if (manyToMany != null) {
				m2ms.Add(Tuple.Create(manyToMany, navigation));
			}
		}
	}
	if (!viewModelData.LookUpTables.Any() && !m2ms.Any())
		return;
this.Write("    <dxlc:LayoutGroup x:Name=\"Tabs\" View=\"Tabs\" MinHeight=\"250\">\r\n");
	foreach(var t in m2ms) {
		var navigation = t.Item2;
		var m2m = t.Item1;
this.Write("\t<dxb:BarManager MDIMergeStyle=\"Never\" dxlc:LayoutControl.TabHeader=\"");
this.Write(this.ToStringHelper.ToStringWithCulture(m2m.OtherEntity.Name));
this.Write("\" dxb:BarNameScope.IsScopeOwner=\"True\" DataContext=\"{Binding ");
this.Write(this.ToStringHelper.ToStringWithCulture(navigation.Name));
this.Write("DetailEntities}\">\r\n        <dxmvvm:Interaction.Behaviors>\r\n            <dx:Dialog" +
		"Service DialogWindowStartupLocation=\"CenterOwner\">\r\n                <dx:DialogSe" +
		"rvice.DialogStyle>\r\n                    <Style TargetType=\"Window\">\r\n           " +
		"             <Setter Property=\"Width\" Value=\"600\" />\r\n                        <S" +
		"etter Property=\"Height\" Value=\"400\" />\r\n                        <Setter Property" +
		"=\"ResizeMode\" Value=\"NoResize\" />\r\n                        <Setter Property=\"Sho" +
		"wInTaskbar\" Value=\"False\" />\r\n                        <Setter Property=\"WindowSt" +
		"yle\" Value=\"ToolWindow\" />\r\n                    </Style>\r\n                </dx:D" +
		"ialogService.DialogStyle>\r\n                <dx:DialogService.ViewTemplate>\r\n    " +
		"                <DataTemplate>\r\n                        <dxg:GridControl ItemsSo" +
		"urce=\"{Binding AvailableEntities}\" SelectedItems=\"{Binding SelectedEntities}\" Se" +
		"lectionMode=\"Row\" ShowBorder=\"False\" AutoGenerateColumns=\"RemoveOld\" EnableSmart" +
		"ColumnsGeneration=\"True\">\r\n                            <dxg:GridControl.TotalSum" +
		"mary>\r\n                                <dxg:GridSummaryItem SummaryType=\"Count\" " +
		"Alignment=\"Right\" />\r\n                            </dxg:GridControl.TotalSummary" +
		">\r\n                            <dxg:GridControl.View>\r\n                         " +
		"       <dxg:TableView AllowEditing=\"False\" ShowFixedTotalSummary=\"True\" AllowPer" +
		"PixelScrolling=\"True\" ShowGroupPanel=\"False\" AutoWidth=\"True\" />\r\n              " +
		"              </dxg:GridControl.View>\r\n                        </dxg:GridControl" +
		">\r\n                    </DataTemplate>\r\n                </dx:DialogService.ViewT" +
		"emplate>\r\n            </dx:DialogService>\r\n        </dxmvvm:Interaction.Behavior" +
		"s>\r\n        <dxb:BarManager.Bars>\r\n            <dxb:Bar>\r\n                <dxb:B" +
		"arButtonItem IsVisible=\"{Binding IsCreateDetailButtonVisible}\" ToolTip=\"Create D" +
		"etail Entity\" Glyph=\"{dx:DXImage Image=New_16x16.png}\" Command=\"{Binding CreateD" +
		"etailEntityCommand}\" />\r\n                <dxb:BarButtonItem ToolTip=\"Edit Detail" +
		" Entity\" Glyph=\"{dx:DXImage Image=Edit_16x16.png}\" Command=\"{Binding EditDetailE" +
		"ntityCommand}\" />\r\n                <dxb:BarButtonItem ToolTip=\"Add Associations\"" +
		" Glyph=\"{dx:DXImage Image=Add_16x16.png}\" Command=\"{Binding AddDetailEntitiesCom" +
		"mand}\" />\r\n                <dxb:BarButtonItem ToolTip=\"Remove Associations\" Glyp" +
		"h=\"{dx:DXImage Image=Remove_16x16.png}\" Command=\"{Binding RemoveDetailEntitiesCo" +
		"mmand}\" />\r\n            </dxb:Bar>\r\n        </dxb:BarManager.Bars>\r\n        <dxg" +
		":GridControl ItemsSource=\"{Binding DetailEntities}\" SelectedItems=\"{Binding Sele" +
		"ctedEntities}\" ShowBorder=\"False\" SelectionMode=\"Row\" AutoGenerateColumns=\"Remov" +
		"eOld\" EnableSmartColumnsGeneration=\"True\">\r\n            <dxg:GridControl.TotalSu" +
		"mmary>\r\n                <dxg:GridSummaryItem SummaryType=\"Count\" Alignment=\"Righ" +
		"t\" />\r\n            </dxg:GridControl.TotalSummary>\r\n            <dxg:GridControl" +
		".View>\r\n                <dxg:TableView AllowEditing=\"False\" ShowFixedTotalSummar" +
		"y=\"True\" AllowPerPixelScrolling=\"True\" ShowGroupPanel=\"False\" AutoWidth=\"True\">\r" +
		"\n                </dxg:TableView>\r\n            </dxg:GridControl.View>\r\n        " +
		"</dxg:GridControl>\r\n    </dxb:BarManager>\r\n");
	}
	foreach(LookUpCollectionViewModelData item in viewModelData.LookUpTables) {
		if (TemplatesCodeGen.IsAlreadyHandled(item, m2ms.Select(t => t.Item2)))
			continue;
		AddLookUpTable(context, viewModelData, item, templateInfo);
	}
this.Write("    </dxlc:LayoutGroup>\r\n");
}
void AddLookUpTable(TemplateGenerationContext context, EntityViewModelData parentViewModelData, LookUpCollectionViewModelData viewModelData, T4TemplateInfo templateInfo) {
	string generatedFieldsBase64 = (string)templateInfo.Properties["GeneratedFieldsBase64."+viewModelData.Name];
	string generatedBandsBase64 = (string)templateInfo.Properties["GeneratedBandsBase64."+viewModelData.Name];
	var manyToMany = TemplatesCodeGen.FindManyToManyAssociation(context.MetadataWorkspace, parentViewModelData.EntityTypeName, viewModelData.EntityTypeName);
	string dataContext = GetLookUpPropertyName(parentViewModelData, viewModelData);
this.Write("    <Grid dxb:MergingProperties.AllowMerging=\"False\" x:Name=\"");
this.Write(this.ToStringHelper.ToStringWithCulture(dataContext));
this.Write("Panel\" dxb:BarNameScope.IsScopeOwner=\"True\" DataContext=\"{Binding ");
this.Write(this.ToStringHelper.ToStringWithCulture(dataContext));
this.Write("}\" dxlc:LayoutControl.TabHeader=\"");
this.Write(this.ToStringHelper.ToStringWithCulture(viewModelData.LookUpCollectionPropertyAssociationName));
this.Write("\">\r\n        <Grid.RowDefinitions>\r\n            <RowDefinition Height=\"Auto\"/>\r\n  " +
		"          <RowDefinition Height=\"*\"/>\r\n        </Grid.RowDefinitions>\r\n        <" +
		"dxb:ToolBarControl>\r\n");
foreach(CommandInfo command in viewModelData.Commands) {
this.Write("            <dxb:BarButtonItem BarItemDisplayMode=\"ContentAndGlyph\" Content=\"");
this.Write(this.ToStringHelper.ToStringWithCulture(command.Caption));
this.Write("\" Command=\"{Binding ");
this.Write(this.ToStringHelper.ToStringWithCulture(command.CommandPropertyName));
this.Write("}\" ");
if(command.HasParameter()){
this.Write("CommandParameter=\"{Binding ");
this.Write(this.ToStringHelper.ToStringWithCulture(command.ParameterPropertyName));
this.Write("}\"");
}
this.Write(" ");
if(command.HasGlyphs()){
this.Write("Glyph=\"");
this.Write(this.ToStringHelper.ToStringWithCulture(command.SmallGlyph));
this.Write("\"");
}
this.Write("/>\r\n");
}
this.Write("        </dxb:ToolBarControl>\r\n        <dxg:GridControl dx:DXSerializer.Serializa" +
		"tionID=\"");
this.Write(this.ToStringHelper.ToStringWithCulture(dataContext));
this.Write("Grid\" ItemsSource=\"{Binding ");
this.Write(this.ToStringHelper.ToStringWithCulture(viewModelData.CollectionPropertyName));
this.Write("}\" ");
if(viewModelData.HasEntityPropertyName()){
this.Write("CurrentItem=\"{Binding ");
this.Write(this.ToStringHelper.ToStringWithCulture(viewModelData.EntityPropertyName));
this.Write("}\"");
}
this.Write(" MaxHeight=\"2000\" ShowLoadingPanel=\"{Binding IsLoading}\" Grid.Row=\"1\">\r\n         " +
		"   ");
if(viewModelData.HasEntityEditProperty()) {
this.Write(@"            <dxmvvm:Interaction.Behaviors>
                <dxmvvm:EventToCommand PassEventArgsToCommand=""True"" Command=""{Binding EditCommand}"" EventName=""MouseDoubleClick"">
                    <dxmvvm:EventToCommand.EventArgsConverter>
                        <dx:EventArgsToDataRowConverter/>
                    </dxmvvm:EventToCommand.EventArgsConverter>
                </dxmvvm:EventToCommand>
            </dxmvvm:Interaction.Behaviors>
");
}
if(!string.IsNullOrEmpty(generatedFieldsBase64)) {
this.Write("            <dxg:GridControl.Columns>\r\n                ");
this.Write(this.ToStringHelper.ToStringWithCulture(generatedFieldsBase64));
this.Write("\r\n            </dxg:GridControl.Columns>\r\n");
}
if(!string.IsNullOrEmpty(generatedBandsBase64)) {
this.Write("            <dxg:GridControl.Bands>\r\n                ");
this.Write(this.ToStringHelper.ToStringWithCulture(generatedBandsBase64));
this.Write("\r\n            </dxg:GridControl.Bands>\r\n");
}
this.Write(@"            <dxg:GridControl.TotalSummary>
                <dxg:GridSummaryItem SummaryType=""Count"" Alignment=""Right""/>
            </dxg:GridControl.TotalSummary>
            <dxg:GridControl.GroupSummary>
                <dxg:GridSummaryItem SummaryType=""Count""/>
            </dxg:GridControl.GroupSummary>
            <dxg:GridControl.View>
                <dxg:TableView AllowEditing=""False"" ShowFixedTotalSummary=""True"" AllowPerPixelScrolling=""True"" ShowGroupPanel=""False"">
                    <dxg:TableView.RowCellMenuCustomizations>
");
foreach(CommandInfo command in viewModelData.Commands) {
this.Write("                    <dxb:BarButtonItem Content=\"");
this.Write(this.ToStringHelper.ToStringWithCulture(command.Caption));
this.Write("\" Command=\"{Binding View.DataContext.");
this.Write(this.ToStringHelper.ToStringWithCulture(command.CommandPropertyName));
this.Write("}\" ");
if(command.HasParameter()){
this.Write("CommandParameter=\"{Binding Row.Row}\"");
}
this.Write(" ");
if(command.HasGlyphs()){
this.Write("Glyph=\"");
this.Write(this.ToStringHelper.ToStringWithCulture(command.SmallGlyph));
this.Write("\"");
}
this.Write("/>\r\n");
}
this.Write("                    </dxg:TableView.RowCellMenuCustomizations>\r\n                <" +
		"/dxg:TableView>\r\n            </dxg:GridControl.View>\r\n        </dxg:GridControl>" +
		"    \r\n    </Grid>\r\n");
}
void AddBarButtonItemsForLookUpTables(EntityViewModelData viewModelData, T4TemplateInfo templateInfo){
	foreach(LookUpCollectionViewModelData item in viewModelData.LookUpTables) 
		AddBarButtonItemsForLookUpTable(viewModelData, item, templateInfo);
}
void AddBarButtonItemsForLookUpTable(EntityViewModelData parentViewModelData, LookUpCollectionViewModelData viewModelData, T4TemplateInfo templateInfo){
	string lookUpTablePropertyName = GetLookUpPropertyName(parentViewModelData, viewModelData);
	foreach(CommandInfo command in viewModelData.Commands) {
		string commandName = GetCommandName(command, viewModelData.EntityTypeName);
this.Write("        <dxb:BarButtonItem Content=\"");
this.Write(this.ToStringHelper.ToStringWithCulture(command.Caption));
this.Write("\" Command=\"{Binding ");
this.Write(this.ToStringHelper.ToStringWithCulture(lookUpTablePropertyName));
this.Write(".");
this.Write(this.ToStringHelper.ToStringWithCulture(command.CommandPropertyName));
this.Write("}\" ");
if(command.HasParameter()){
this.Write("CommandParameter=\"{Binding ");
this.Write(this.ToStringHelper.ToStringWithCulture(lookUpTablePropertyName));
this.Write(".");
this.Write(this.ToStringHelper.ToStringWithCulture(command.ParameterPropertyName));
this.Write("}\"");
}
this.Write(" ");
if(command.HasGlyphs()){
this.Write("LargeGlyph=\"");
this.Write(this.ToStringHelper.ToStringWithCulture(command.LargeGlyph));
this.Write("\" Glyph=\"");
this.Write(this.ToStringHelper.ToStringWithCulture(command.SmallGlyph));
this.Write("\"");
}
this.Write("/>\r\n");
}
}
string GetCommandName(CommandInfo info, string entityTypeName){
	string str_command = "Command";
	string infoCommandPropertyName = info.CommandPropertyName;
	string shortName = infoCommandPropertyName.EndsWith(str_command) ? infoCommandPropertyName.Remove(infoCommandPropertyName.Length - str_command.Length) : null;
	if(string.IsNullOrEmpty(shortName))
		return infoCommandPropertyName+entityTypeName;
	return shortName+entityTypeName+str_command;
}
string GetLookUpGridName(LookUpCollectionViewModelData viewModelData){
	return viewModelData.LookUpCollectionPropertyAssociationName+"Grid";
}
string GetLookUpPropertyName(EntityViewModelData parentViewModelData, LookUpCollectionViewModelData viewModelData){
	return parentViewModelData.EntityTypeName+viewModelData.LookUpCollectionPropertyAssociationName + "Details";	
}
void PasteXamlNamespaces(XamlNamespaces xamlNamespaces) {
	string[] toAdd = new string[]{
		XmlNamespaceConstants.RibbonNamespaceDefinition,
		XmlNamespaceConstants.EditorsNamespaceDefinition,
		XmlNamespaceConstants.LayoutControlNamespaceDefinition,
		XmlNamespaceConstants.BarsNamespaceDefinition,
		XmlNamespaceConstants.UtilsNamespaceDefinition,
		XmlNamespaceConstants.GridNamespaceDefinition,
		XmlNamespaceConstants.MvvmNamespaceDefinition
	};
	xamlNamespaces.AddDevExpressXamlNamespaces(toAdd);
	this.WriteLine(xamlNamespaces.GetXaml());
}
	}
	#region Base class
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public class ElementViewTemplateBase
	{
		#region Fields
		private global::System.Text.StringBuilder generationEnvironmentField;
		private global::System.CodeDom.Compiler.CompilerErrorCollection errorsField;
		private global::System.Collections.Generic.List<int> indentLengthsField;
		private string currentIndentField = "";
		private bool endsWithNewline;
		private global::System.Collections.Generic.IDictionary<string, object> sessionField;
		#endregion
		#region Properties
		protected System.Text.StringBuilder GenerationEnvironment
		{
			get
			{
				if ((this.generationEnvironmentField == null))
				{
					this.generationEnvironmentField = new global::System.Text.StringBuilder();
				}
				return this.generationEnvironmentField;
			}
			set
			{
				this.generationEnvironmentField = value;
			}
		}
		public System.CodeDom.Compiler.CompilerErrorCollection Errors
		{
			get
			{
				if ((this.errorsField == null))
				{
					this.errorsField = new global::System.CodeDom.Compiler.CompilerErrorCollection();
				}
				return this.errorsField;
			}
		}
		private System.Collections.Generic.List<int> indentLengths
		{
			get
			{
				if ((this.indentLengthsField == null))
				{
					this.indentLengthsField = new global::System.Collections.Generic.List<int>();
				}
				return this.indentLengthsField;
			}
		}
		public string CurrentIndent
		{
			get
			{
				return this.currentIndentField;
			}
		}
		public virtual global::System.Collections.Generic.IDictionary<string, object> Session
		{
			get
			{
				return this.sessionField;
			}
			set
			{
				this.sessionField = value;
			}
		}
		#endregion
		#region Transform-time helpers
		public void Write(string textToAppend)
		{
			if (string.IsNullOrEmpty(textToAppend))
			{
				return;
			}
			if (((this.GenerationEnvironment.Length == 0) 
						|| this.endsWithNewline))
			{
				this.GenerationEnvironment.Append(this.currentIndentField);
				this.endsWithNewline = false;
			}
			if (textToAppend.EndsWith(global::System.Environment.NewLine, global::System.StringComparison.CurrentCulture))
			{
				this.endsWithNewline = true;
			}
			if ((this.currentIndentField.Length == 0))
			{
				this.GenerationEnvironment.Append(textToAppend);
				return;
			}
			textToAppend = textToAppend.Replace(global::System.Environment.NewLine, (global::System.Environment.NewLine + this.currentIndentField));
			if (this.endsWithNewline)
			{
				this.GenerationEnvironment.Append(textToAppend, 0, (textToAppend.Length - this.currentIndentField.Length));
			}
			else
			{
				this.GenerationEnvironment.Append(textToAppend);
			}
		}
		public void WriteLine(string textToAppend)
		{
			this.Write(textToAppend);
			this.GenerationEnvironment.AppendLine();
			this.endsWithNewline = true;
		}
		public void Write(string format, params object[] args)
		{
			this.Write(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
		}
		public void WriteLine(string format, params object[] args)
		{
			this.WriteLine(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
		}
		public void Error(string message)
		{
			System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
			error.ErrorText = message;
			this.Errors.Add(error);
		}
		public void Warning(string message)
		{
			System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
			error.ErrorText = message;
			error.IsWarning = true;
			this.Errors.Add(error);
		}
		public void PushIndent(string indent)
		{
			if ((indent == null))
			{
				throw new global::System.ArgumentNullException("indent");
			}
			this.currentIndentField = (this.currentIndentField + indent);
			this.indentLengths.Add(indent.Length);
		}
		public string PopIndent()
		{
			string returnValue = "";
			if ((this.indentLengths.Count > 0))
			{
				int indentLength = this.indentLengths[(this.indentLengths.Count - 1)];
				this.indentLengths.RemoveAt((this.indentLengths.Count - 1));
				if ((indentLength > 0))
				{
					returnValue = this.currentIndentField.Substring((this.currentIndentField.Length - indentLength));
					this.currentIndentField = this.currentIndentField.Remove((this.currentIndentField.Length - indentLength));
				}
			}
			return returnValue;
		}
		public void ClearIndent()
		{
			this.indentLengths.Clear();
			this.currentIndentField = "";
		}
		#endregion
		#region ToString Helpers
		public class ToStringInstanceHelper
		{
			private System.IFormatProvider formatProviderField  = global::System.Globalization.CultureInfo.InvariantCulture;
			public System.IFormatProvider FormatProvider
			{
				get
				{
					return this.formatProviderField ;
				}
				set
				{
					if ((value != null))
					{
						this.formatProviderField  = value;
					}
				}
			}
			public string ToStringWithCulture(object objectToConvert)
			{
				if ((objectToConvert == null))
				{
					throw new global::System.ArgumentNullException("objectToConvert");
				}
				System.Type t = objectToConvert.GetType();
				System.Reflection.MethodInfo method = t.GetMethod("ToString", new System.Type[] {
							typeof(System.IFormatProvider)});
				if ((method == null))
				{
					return objectToConvert.ToString();
				}
				else
				{
					return ((string)(method.Invoke(objectToConvert, new object[] {
								this.formatProviderField })));
				}
			}
		}
		private ToStringInstanceHelper toStringHelperField = new ToStringInstanceHelper();
		public ToStringInstanceHelper ToStringHelper
		{
			get
			{
				return this.toStringHelperField;
			}
		}
		#endregion
	}
	#endregion
}
