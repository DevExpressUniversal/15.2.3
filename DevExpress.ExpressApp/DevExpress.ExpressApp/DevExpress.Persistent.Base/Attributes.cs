#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
namespace DevExpress.Persistent.Base {
	[Flags]
	public enum ExpandObjectMembers {
		Never = 0,
		InDetailView = 1,
		InListView = 2,
		Always = (InDetailView | InListView),
	}
	public enum PredefinedCategory {
		Unspecified,
		ObjectsCreation,
		RecordEdit,
		Reports,
		Edit,
		Save,
		UndoRedo,
		Appearance,
		OpenObject,
		View,
		Print,
		RecordsNavigation,
		Search,
		Filters,
		FullTextSearch,
		ViewsHistoryNavigation,
		ViewsNavigation,
		Export,
		Options,
		Tools,
		About,
		PopupActions,
		Menu,
		Windows
	}
	public enum MethodActionSelectionDependencyType { RequireSingleObject, RequireMultipleObjects }
	public class ModelExportedBoolValueAttribute : ModelExportedValueAttribute {
		private bool value;
		public ModelExportedBoolValueAttribute(bool value) {
			this.value = value;
		}
		public override object Value {
			get { return value; }
		}
	}
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
	public class VisibleInLookupListViewAttribute : ModelExportedBoolValueAttribute {
		public VisibleInLookupListViewAttribute(bool value)
			: base(value) {
		}
	}
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
	public class VisibleInListViewAttribute : ModelExportedBoolValueAttribute {
		public VisibleInListViewAttribute(bool value)
			: base(value) {
		}
	}
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
	public class VisibleInDetailViewAttribute : ModelExportedBoolValueAttribute {
		public VisibleInDetailViewAttribute(bool value)
			: base(value) {
		}
	}
	[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
	public class IndexAttribute : ModelExportedValueAttribute {
		private int index;
		public IndexAttribute(int index) {
			this.index = index;
		}
		public int Index {
			get {
				return index;
			}
		}
		public override object Value {
			get { return index; }
		}
	}
	public enum DataSourcePropertyIsNullMode { SelectNothing, SelectAll, CustomCriteria }
	[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	public class DataSourcePropertyAttribute : ModelExportedValuesAttribute {
		private String dataSourceProperty;
		private DataSourcePropertyIsNullMode dataSourcePropertyIsNullMode;
		private String dataSourcePropertyIsNullCriteria;
		public DataSourcePropertyAttribute(String dataSourceProperty)
			: this(dataSourceProperty, DataSourcePropertyIsNullMode.SelectNothing) {
		}
		public DataSourcePropertyAttribute(String dataSourceProperty, DataSourcePropertyIsNullMode mode)
			: this(dataSourceProperty, mode, "") {
		}
		public DataSourcePropertyAttribute(String dataSourceProperty, DataSourcePropertyIsNullMode dataSourcePropertyIsNullMode, String dataSourcePropertyIsNullCriteria) {
			this.dataSourceProperty = dataSourceProperty;
			this.dataSourcePropertyIsNullMode = dataSourcePropertyIsNullMode;
			this.dataSourcePropertyIsNullCriteria = dataSourcePropertyIsNullCriteria;
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("DataSourcePropertyAttributeDataSourceProperty")]
#endif
		public String DataSourceProperty {
			get { return dataSourceProperty; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("DataSourcePropertyAttributeDataSourcePropertyIsNullMode")]
#endif
		public DataSourcePropertyIsNullMode DataSourcePropertyIsNullMode {
			get { return dataSourcePropertyIsNullMode; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("DataSourcePropertyAttributeDataSourcePropertyIsNullCriteria")]
#endif
		public String DataSourcePropertyIsNullCriteria {
			get { return dataSourcePropertyIsNullCriteria; }
		}
		public override void FillValues(Dictionary<String, Object> values) {
			values.Add("DataSourceProperty", dataSourceProperty);
			values.Add("DataSourcePropertyIsNullMode", dataSourcePropertyIsNullMode);
			values.Add("DataSourcePropertyIsNullCriteria", dataSourcePropertyIsNullCriteria);
		}
	}
	[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	public sealed class DataSourceCriteriaAttribute : ModelExportedValueAttribute {
		private String dataSourceCriteria;
		public DataSourceCriteriaAttribute(String dataSourceCriteria) {
			this.dataSourceCriteria = dataSourceCriteria;
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("DataSourceCriteriaAttributeDataSourceCriteria")]
#endif
		public String DataSourceCriteria {
			get { return dataSourceCriteria; }
		}
		public override Object Value {
			get { return dataSourceCriteria; }
		}
	}
	[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	public sealed class DataSourceCriteriaPropertyAttribute : ModelExportedValueAttribute {
		private String dataSourceCriteriaProperty;
		public DataSourceCriteriaPropertyAttribute(String dataSourceCriteriaProperty) {
			this.dataSourceCriteriaProperty = dataSourceCriteriaProperty;
		}
		public String DataSourceCriteriaProperty {
			get { return dataSourceCriteriaProperty; }
		}
		public override Object Value {
			get { return dataSourceCriteriaProperty; }
		}
	}
	public enum LookupEditorMode { Auto, AllItems, Search, AllItemsWithSearch };
	public class LookupEditorModeAttribute : ModelExportedValuesAttribute {
		private LookupEditorMode mode = LookupEditorMode.Auto;
		public LookupEditorModeAttribute(LookupEditorMode mode) {
			this.mode = mode;
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("LookupEditorModeAttributeMode")]
#endif
		public LookupEditorMode Mode {
			get { return mode; }
		}
		public override void FillValues(Dictionary<string, object> values) {
			values.Add("LookupEditorMode", Mode);
		}
	}
	[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	public sealed class ImmediatePostDataAttribute : ModelExportedValueAttribute {
		private bool immediatePostData;
		public ImmediatePostDataAttribute() : this(true) { } 
		public ImmediatePostDataAttribute(bool immediatePostData) {
			this.immediatePostData = immediatePostData;
		}
		public override object Value {
			get { return immediatePostData; }
		}
	}
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = true, AllowMultiple = false)]
	public class ObjectCaptionFormatAttribute : Attribute {
		private string formatString;
		public ObjectCaptionFormatAttribute(string formatString) {
			this.formatString = formatString;
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ObjectCaptionFormatAttributeFormatString")]
#endif
		public virtual string FormatString {
			get { return formatString; }
		}
	}
	[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	public class LookupDropDownWindowPreferredSizeAttribute : ModelExportedValuesAttribute {
		private int width;
		private int height;
		public LookupDropDownWindowPreferredSizeAttribute(int width, int height) {
			this.width = width;
			this.height = height;
		}
		public Size Size {
			get { return new Size(width, height); }
		}
		public override void FillValues(Dictionary<string, object> values) {
			values.Add("LookupDropDownWindowWidth", width);
			values.Add("LookupDropDownWindowHeight", height);
		}
	}
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
	public class ImagesForBoolValuesAttribute : ModelExportedValuesAttribute {
		private string imageForTrue;
		private string imageForFalse;
		public ImagesForBoolValuesAttribute(string imageForTrue, string imageForFalse) {
			this.imageForTrue = imageForTrue;
			this.imageForFalse = imageForFalse;
		}
		public string ImageForTrue {
			get {
				return imageForTrue;
			}
		}
		public string ImageForFalse {
			get {
				return imageForFalse;
			}
		}
		public override void FillValues(Dictionary<string, object> values) {
				values.Add("ImageForTrue", ImageForTrue); 
				values.Add("ImageForFalse", ImageForFalse);
		}
	}
	public abstract class ModelExportedValuesAttribute : Attribute {
		public abstract void FillValues(Dictionary<String, Object> values);
	}
	public abstract class ModelExportedValueAttribute : Attribute {
		public abstract Object Value {
			get;
		}
	}
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
	public class CaptionsForBoolValuesAttribute : ModelExportedValuesAttribute {
		private string captionForTrue;
		private string captionForFalse;
		public CaptionsForBoolValuesAttribute(string captionForTrue, string captionForFalse) {
			this.captionForTrue = captionForTrue;
			this.captionForFalse = captionForFalse;
		}
		public string CaptionForTrue {
			get {
				return captionForTrue;
			}
		}
		public string CaptionForFalse {
			get {
				return captionForFalse;
			}
		}
		public override void FillValues(Dictionary<string, object> values) {
				values.Add("CaptionForTrue", CaptionForTrue); 
				values.Add("CaptionForFalse", CaptionForFalse);
		}
	}
	public enum ImageEditorMode { DropDownPictureEdit, PictureEdit, PopupPictureEdit }
	public enum ImageSizeMode {		 
		Normal = 0,
		StretchImage = 1,
		AutoSize = 2,
		CenterImage = 3,
		Zoom = 4,
	}
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
	public class ImageEditorAttribute : ModelExportedValuesAttribute {
		private ImageEditorMode listViewImageEditorMode = ImageEditorMode.PictureEdit;
		private ImageEditorMode detailViewImageEditorMode = ImageEditorMode.PictureEdit;
		private int listViewImageEditorCustomHeight = 0;
		private int detailViewImageEditorFixedWidth;
		private int detailViewImageEditorFixedHeight;
		private ImageSizeMode imageSizeMode = ImageSizeMode.Zoom;
		public ImageEditorAttribute() { }
		public ImageEditorAttribute(ImageEditorMode listViewImageEditorMode, ImageEditorMode detailViewImageEditorMode, int listViewImageEditorCustomHeight)
			: this(listViewImageEditorMode, detailViewImageEditorMode) {
			this.listViewImageEditorCustomHeight = listViewImageEditorCustomHeight;
		}
		public ImageEditorAttribute(ImageEditorMode listViewImageEditorMode, ImageEditorMode detailViewImageEditorMode) {
			this.listViewImageEditorMode = listViewImageEditorMode;
			this.detailViewImageEditorMode = detailViewImageEditorMode;
		}
		public ImageEditorMode ListViewImageEditorMode {
			get { return listViewImageEditorMode; }
			set { listViewImageEditorMode = value; }
		}
		public ImageEditorMode DetailViewImageEditorMode {
			get { return detailViewImageEditorMode; }
			set { detailViewImageEditorMode = value; }
		}
		public int ListViewImageEditorCustomHeight {
			get { return listViewImageEditorCustomHeight; }
			set { listViewImageEditorCustomHeight = value; }
		}
		public int DetailViewImageEditorFixedWidth {
			get { return detailViewImageEditorFixedWidth; }
			set { detailViewImageEditorFixedWidth = value; }
		}
		public int DetailViewImageEditorFixedHeight {
			get { return detailViewImageEditorFixedHeight; }
			set { detailViewImageEditorFixedHeight = value; }
		}
		public ImageSizeMode ImageSizeMode {
			get { return imageSizeMode; }
			set { imageSizeMode = value; }
		}
		public override void FillValues(Dictionary<string, object> values) {
			values.Add("ListViewImageEditorMode", listViewImageEditorMode);
			values.Add("DetailViewImageEditorMode", detailViewImageEditorMode);
			values.Add("ListViewImageEditorCustomHeight", listViewImageEditorCustomHeight);
			values.Add("DetailViewImageEditorFixedWidth", detailViewImageEditorFixedWidth);
			values.Add("DetailViewImageEditorFixedHeight", detailViewImageEditorFixedHeight);
			values.Add("ImageSizeMode", imageSizeMode);
		}
	}
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
	public class EditorAliasAttribute : Attribute {
		private string alias;
		public EditorAliasAttribute(string alias) {
			this.alias = alias;
		}
		public string Alias { get { return alias; } }
	}
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
	public sealed class CollectionAttribute : Attribute {
		private Type elementType;
		public CollectionAttribute(Type elementType) {
			this.elementType = elementType;
		}
		public Type ElementType {
			get { return elementType; }
		}
	}
	[AttributeUsage(AttributeTargets.Method)]
	public class ActionAttribute : Attribute {
		private string caption;
		private string category;
		private bool autoCommit = true;
		private string confirmationMessage;
		private string targetObjectsCriteria;
		private string toolTip;
		private string imageName;
		private MethodActionSelectionDependencyType selectionDependencyType = MethodActionSelectionDependencyType.RequireMultipleObjects;
		public ActionAttribute() : this(PredefinedCategory.RecordEdit) { }
		public ActionAttribute(PredefinedCategory category) : this(category.ToString()) { }
		public ActionAttribute(string category) {
			if(string.IsNullOrEmpty(category)) {
				throw new ArgumentException("Category should not be empty");
			}
			this.category = category;
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ActionAttributeCaption")]
#endif
		public string Caption {
			get { return caption; }
			set { caption = value; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ActionAttributeCategory")]
#endif
		public string Category {
			get { return category; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ActionAttributeSelectionDependencyType")]
#endif
		public MethodActionSelectionDependencyType SelectionDependencyType {
			get { return selectionDependencyType; }
			set { selectionDependencyType = value; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ActionAttributeConfirmationMessage")]
#endif
		public string ConfirmationMessage {
			get { return confirmationMessage; }
			set { confirmationMessage = value; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ActionAttributeTargetObjectsCriteria")]
#endif
		public string TargetObjectsCriteria {
			get { return targetObjectsCriteria; }
			set { targetObjectsCriteria = value; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ActionAttributeTargetObjectsCriteriaMode")]
#endif
		public TargetObjectsCriteriaMode TargetObjectsCriteriaMode {
			get;
			set;
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ActionAttributeToolTip")]
#endif
		public string ToolTip {
			get { return toolTip; }
			set { toolTip = value; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ActionAttributeImageName")]
#endif
		public string ImageName {
			get { return imageName; }
			set { imageName = value; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ActionAttributeAutoCommit")]
#endif
		public bool AutoCommit {
			get { return autoCommit; }
			set { autoCommit = value; }
		}
	}
	[AttributeUsage(
	AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Interface | AttributeTargets.Property | AttributeTargets.Struct,
	Inherited = true)]
	public class ExpandObjectMembersAttribute : Attribute {
		public static ExpandObjectMembers AggregatedObjectMembersDefaultExpandingMode = ExpandObjectMembers.InDetailView;
		private ExpandObjectMembers expandingMode = ExpandObjectMembers.InDetailView;
		private string memberName = string.Empty;
		public ExpandObjectMembersAttribute(ExpandObjectMembers expandingMode) {
			this.expandingMode = expandingMode;
		}
		public ExpandObjectMembersAttribute(ExpandObjectMembers expandingMode, string memberName) {
			this.expandingMode = expandingMode;
			this.memberName = memberName;
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ExpandObjectMembersAttributeExpandingMode")]
#endif
		public ExpandObjectMembers ExpandingMode {
			get { return expandingMode; }
		}
		public string MemberName {
			get { return memberName; }
		}
	}
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
	public class FriendlyKeyPropertyAttribute : Attribute {
		private string memberName;
		public FriendlyKeyPropertyAttribute(string memberName) {
			this.memberName = memberName;
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("FriendlyKeyPropertyAttributeMemberName")]
#endif
		public string MemberName {
			get { return memberName; }
		}
		public static string FindFriendlyKeyMemberName(ITypeInfo typeInfo, bool recursive) {
			if(typeInfo != null) {
				FriendlyKeyPropertyAttribute friendlyKeyProperty = typeInfo.FindAttribute<FriendlyKeyPropertyAttribute>(recursive);
				if(friendlyKeyProperty != null && typeInfo.FindMember(friendlyKeyProperty.MemberName) != null) {
					return friendlyKeyProperty.MemberName;
				}
			}
			return null;
		}
	}
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = false)]
	public class DefaultClassOptionsAttribute : Attribute {
	}
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = false)]
	public class NavigationItemAttribute : Attribute {
		private static string defaultGroupName = "Default";
		private string groupName = DefaultGroupName;
		private bool isNavigationItem;
		public NavigationItemAttribute() : this(DefaultGroupName) { }
		public NavigationItemAttribute(bool isNavigationItem) {
			this.isNavigationItem = isNavigationItem;
			this.groupName = "";
		}
		public NavigationItemAttribute(string groupName) {
			this.isNavigationItem = true;
			this.groupName = groupName;
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("NavigationItemAttributeIsNavigationItem")]
#endif
		public bool IsNavigationItem {
			get { return isNavigationItem; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("NavigationItemAttributeGroupName")]
#endif
		public string GroupName {
			get { return groupName; }
			set { groupName = value; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("NavigationItemAttributeDefaultGroupName")]
#endif
		public static string DefaultGroupName {
			get { return defaultGroupName; }
			set { defaultGroupName = value; }
		}
	}
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = false)]
	public class CreatableItemAttribute : Attribute {
		private bool isCreatableItem;
		public CreatableItemAttribute() : this(true) { }
		public CreatableItemAttribute(bool isCreatableItem) {
			this.isCreatableItem = isCreatableItem;
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("CreatableItemAttributeIsCreatableItem")]
#endif
		public bool IsCreatableItem {
			get { return isCreatableItem; }
		}
	}
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = false)]
	public class VisibleInReportsAttribute : Attribute {
		private bool isVisible;
		public VisibleInReportsAttribute() : this(true) { }
		public VisibleInReportsAttribute(bool isVisible) {
			this.isVisible = isVisible;
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("VisibleInReportsAttributeIsVisible")]
#endif
		public bool IsVisible {
			get { return isVisible; }
		}
	}
	public class VisibleInReportsIndicator {
		private static object instanceValueManagerLockObj = new object();
		private static IValueManager<VisibleInReportsIndicator> instanceValueManager;
		public static VisibleInReportsIndicator Instance {
			get {
				lock(instanceValueManagerLockObj) {
					if(instanceValueManager == null) {
						instanceValueManager = ValueManager.GetValueManager<VisibleInReportsIndicator>("VisibleInReportsIndicator");
					}
					if(instanceValueManager.Value == null) {
						instanceValueManager.Value = new VisibleInReportsIndicator();
						Guard.ArgumentNotNull(instanceValueManager.Value, "instanceValueManager.Value"); 
					}
				}
				return instanceValueManager.Value;
			}
		}
		public static void SetInstance(VisibleInReportsIndicator indicatorInstance) {
			Guard.ArgumentNotNull(indicatorInstance, "indicatorInstance");
			lock(instanceValueManagerLockObj) {
				if(instanceValueManager == null) {
					instanceValueManager = ValueManager.GetValueManager<VisibleInReportsIndicator>("VisibleInReportsIndicator");
				}
				instanceValueManager.Value = indicatorInstance;
			}
		}
		public virtual bool GetIsVisibleInReports(ITypeInfo classInfo, out bool isVisible) {
			VisibleInReportsAttribute visibleInReportsAttribute =
				classInfo.FindAttribute<VisibleInReportsAttribute>();
			if(visibleInReportsAttribute != null) {
				isVisible = visibleInReportsAttribute.IsVisible;
				return true;
			}
			if(classInfo.FindAttribute<DefaultClassOptionsAttribute>() != null) {
				isVisible = true;
				return true;
			}
			isVisible = false;
			return false;
		}
	}
	public class VisibleInReportsModelIndicator : VisibleInReportsIndicator {
		private IModelApplication model;
		protected IModelApplication Model { get { return model; } }
		public VisibleInReportsModelIndicator(IModelApplication model) {
			this.model = model;
		}
		public override bool GetIsVisibleInReports(ITypeInfo classInfo, out bool isVisible) {
			isVisible = false;
			if(Model == null) { return false; }
			IModelClass classModel = Model.BOModel.GetClass(classInfo.Type);
			IModelClassReportsVisibility classReportsVisibilityModel = classModel as IModelClassReportsVisibility;
			if(classReportsVisibilityModel != null) {
				isVisible = classReportsVisibilityModel.IsVisibleInReports;
				return true;
			}
			return false;
		}
	}
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Interface, Inherited = false)]
	public class ImageNameAttribute : Attribute {
		private string imageName;
		public ImageNameAttribute(string imageName) {
			this.imageName = imageName;
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ImageNameAttributeImageName")]
#endif
		public string ImageName {
			get { return imageName; }
			set { imageName = value; }
		}
	}
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
	public class ToolTipAttribute : ModelExportedValuesAttribute {
		private string toolTip;
		private ToolTipIconType toolTipIconType;
		private string toolTipTitle;
		public ToolTipAttribute(string toolTip, string toolTipTitle, ToolTipIconType toolTipIconType) {
			this.toolTip = toolTip;
			this.toolTipTitle = toolTipTitle;
			this.toolTipIconType = toolTipIconType;
		}
		public ToolTipAttribute(string toolTip, string toolTipTitle)
			: this(toolTip, toolTipTitle, ToolTipIconType.None) {
		}
		public ToolTipAttribute(string toolTip)
			: this(toolTip, null, ToolTipIconType.None) {
		}
		public string ToolTip {
			get {
				return toolTip;
			}
		}
		public string ToolTipTitle {
			get {
				return toolTipTitle;
			}
		}
		public ToolTipIconType ToolTipIconType {
			get {
				return toolTipIconType;
			}
		}
		public override void FillValues(Dictionary<string, object> values) {
			values.Add("ToolTip", ToolTip);
			values.Add("ToolTipTitle", ToolTipTitle);
			values.Add("ToolTipIconType", ToolTipIconType);
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class MediaDataObjectAttribute : Attribute {
		private string mediaDataKeyProperty;
		private string mediaDataBindingProperty;
		private string mediaDataDataViewBindingProperty;
		public MediaDataObjectAttribute(string mediaDataKeyProperty, string mediaDataBindingProperty, string mediaDataDataViewBindingProperty) {
			this.mediaDataKeyProperty = mediaDataKeyProperty;
			this.mediaDataBindingProperty = mediaDataBindingProperty;
			this.mediaDataDataViewBindingProperty = mediaDataDataViewBindingProperty;
		}
		public string MediaDataKeyProperty {
			get { return mediaDataKeyProperty; }
		}
		public string MediaDataBindingProperty {
			get { return mediaDataBindingProperty; }
		}
		public string MediaDataDataViewBindingProperty {
			get { return mediaDataDataViewBindingProperty; }
		}
	}
}
