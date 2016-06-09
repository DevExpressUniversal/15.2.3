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
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Data.Filtering;
namespace DevExpress.Web {
	public class FilterControlImages : ImagesBase {		
		public const string 			
			AddButtonName = "fcadd",
			AddButtonHotName = "fcaddhot",
			RemoveButtonName = "fcremove",
			RemoveButtonHotName = "fcremovehot",
			AddConditionName = "fcgroupaddcondition",
			AddGroupName = "fcgroupaddgroup",
			RemoveGroupName = "fcgroupremove",
			GroupTypeAndName = "fcgroupand",
			GroupTypeOrName = "fcgroupor",
			GroupTypeNotAndName = "fcgroupnotand",
			GroupTypeNotOrName = "fcgroupnotor",
			OperationAnyOfName = "fcopany",
			OperationBeginsWithName = "fcopbegin",
			OperationBetweenName = "fcopbetween",
			OperationContainsName = "fcopcontain",
			OperationDoesNotContainName = "fcopnotcontain",
			OperationDoesNotEqualName = "fcopnotequal",
			OperationEndsWithName = "fcopend",
			OperationEqualsName = "fcopequal",
			OperationGreaterName = "fcopgreater",
			OperationGreaterOrEqualName = "fcopgreaterorequal",
			OperationIsNotNullName = "fcopnotblank",
			OperationIsNullName = "fcopblank",
			OperationLessName = "fcopless",
			OperationLessOrEqualName = "fcoplessorequal",
			OperationLikeName = "fcoplike",
			OperationNoneOfName = "fcopnotany",
			OperationNotBetweenName = "fcopnotbetween",
			OperationNotLikeName = "fcopnotlike",
			AggregateExists = "fcopexists",
			AggregateCount = "fcopcount",
			AggregateMax = "fcopmax",
			AggregateMin = "fcopmin",
			AggregateAvg = "fcopavg",
			AggregateSum = "fcopsum",
			AggregateSingle = "fcopsingle",
			OperandTypeIconFieldName = "fcoptypefield",
			OperandTypeIconFieldHotName = "fcoptypefieldhot",
			OperandTypeIconValueName = "fcoptypevalue",
			OperandTypeIconValueHotName = "fcoptypevaluehot";
		internal const string
			LineImageName = "fcLine",
			ElbowImageName = "fcElbow";
		const int IconSize = 13;
		static Dictionary<ClauseType, string> operationsImageInfos = new Dictionary<ClauseType, string>();
		static Dictionary<Aggregate, string> aggregatesImageInfos = new Dictionary<Aggregate,string>();
		static Dictionary<GroupType, string> groupTypesImageInfos = new Dictionary<GroupType, string>();
		static FilterControlImages() {
			SetGroupTypeNames();
			SetOperationNames();
			SetAggregateNames();
		}
		public FilterControlImages(ISkinOwner owner)   : base(owner) {
		}
		protected internal static string GetOperationName(ClauseType clauseType) {
			if(operationsImageInfos.ContainsKey(clauseType))
				return operationsImageInfos[clauseType];
			return string.Empty;
		}
		protected internal static string GetAggregateName(Aggregate aggregate) {
			if(aggregatesImageInfos.ContainsKey(aggregate))
				return aggregatesImageInfos[aggregate];
			return string.Empty;
		}
		protected internal static string GetGroupTypeName(GroupType groupType) {
			return groupTypesImageInfos[groupType];
		}
		protected override void PopulateImageInfoList(List<ImageInfo> list) {
			base.PopulateImageInfoList(list);
			list.Add(new ImageInfo(AddButtonName, ImageFlags.IsPng, IconSize, AddButtonName));
			list.Add(new ImageInfo(AddButtonHotName, ImageFlags.IsPng, IconSize, AddButtonHotName));
			list.Add(new ImageInfo(RemoveButtonName, ImageFlags.IsPng, IconSize, RemoveButtonName));
			list.Add(new ImageInfo(RemoveButtonHotName, ImageFlags.IsPng, IconSize, RemoveButtonHotName));
			list.Add(new ImageInfo(AddConditionName, ImageFlags.IsPng, IconSize, AddConditionName));
			list.Add(new ImageInfo(AddGroupName, ImageFlags.IsPng, IconSize, AddGroupName));
			list.Add(new ImageInfo(RemoveGroupName, ImageFlags.IsPng, IconSize, RemoveGroupName));
			list.Add(new ImageInfo(OperandTypeIconFieldName, ImageFlags.IsPng, IconSize, OperandTypeIconFieldName));
			list.Add(new ImageInfo(OperandTypeIconFieldHotName, ImageFlags.IsPng, IconSize, OperandTypeIconFieldHotName));
			list.Add(new ImageInfo(OperandTypeIconValueName, ImageFlags.IsPng, IconSize, OperandTypeIconValueName));
			list.Add(new ImageInfo(OperandTypeIconValueHotName, ImageFlags.IsPng, IconSize, OperandTypeIconValueHotName));
			foreach(string name in operationsImageInfos.Values)
				list.Add(new ImageInfo(name, ImageFlags.IsPng, IconSize, name));
			foreach(string name in groupTypesImageInfos.Values)
				list.Add(new ImageInfo(name, ImageFlags.IsPng, IconSize, name));
			foreach(string name in aggregatesImageInfos.Values)
				list.Add(new ImageInfo(name, ImageFlags.IsPng, IconSize, name));
		}
		static void SetOperationNames() {
			operationsImageInfos[ClauseType.AnyOf] = OperationAnyOfName;
			operationsImageInfos[ClauseType.BeginsWith] = OperationBeginsWithName;
			operationsImageInfos[ClauseType.Between] = OperationBetweenName;
			operationsImageInfos[ClauseType.Contains] = OperationContainsName;
			operationsImageInfos[ClauseType.DoesNotContain] = OperationDoesNotContainName;
			operationsImageInfos[ClauseType.DoesNotEqual] = OperationDoesNotEqualName;
			operationsImageInfos[ClauseType.EndsWith] = OperationEndsWithName;
			operationsImageInfos[ClauseType.Equals] = OperationEqualsName;
			operationsImageInfos[ClauseType.Greater] = OperationGreaterName;
			operationsImageInfos[ClauseType.GreaterOrEqual] = OperationGreaterOrEqualName;
			operationsImageInfos[ClauseType.IsNotNull] = OperationIsNotNullName;
			operationsImageInfos[ClauseType.IsNull] = OperationIsNullName;
			operationsImageInfos[ClauseType.Less] = OperationLessName;
			operationsImageInfos[ClauseType.LessOrEqual] = OperationLessOrEqualName;
			operationsImageInfos[ClauseType.Like] = OperationLikeName;
			operationsImageInfos[ClauseType.NoneOf] = OperationNoneOfName;
			operationsImageInfos[ClauseType.NotBetween] = OperationNotBetweenName;
			operationsImageInfos[ClauseType.NotLike] = OperationNotLikeName;
		}
		static void SetGroupTypeNames() {
			groupTypesImageInfos[GroupType.And] = GroupTypeAndName;
			groupTypesImageInfos[GroupType.Or] = GroupTypeOrName;
			groupTypesImageInfos[GroupType.NotAnd] = GroupTypeNotAndName;
			groupTypesImageInfos[GroupType.NotOr] = GroupTypeNotOrName;
		}
		static void SetAggregateNames() {
			aggregatesImageInfos[Aggregate.Exists] = AggregateExists;
			aggregatesImageInfos[Aggregate.Count] = AggregateCount;
			aggregatesImageInfos[Aggregate.Max] = AggregateMax;
			aggregatesImageInfos[Aggregate.Min] = AggregateMin;
			aggregatesImageInfos[Aggregate.Avg] = AggregateAvg;
			aggregatesImageInfos[Aggregate.Sum] = AggregateSum;
			aggregatesImageInfos[Aggregate.Single] = AggregateSingle;
		}
		public override string ToString() { return string.Empty; }
		protected override Type GetResourceType() {
			return typeof(ASPxFilterControlBase);
		}
		protected override string GetResourceImagePath() {
			return ASPxEditBase.EditImagesResourcePath;
		}
		protected override string GetDesignTimeResourceSpriteImagePath() {
			return ASPxEditBase.EditImagesResourcePath + GetDesignTimeResourceSpriteImageDefaultName();
		}
		protected override string GetResourceSpriteCssPath() {
			return ASPxEditBase.EditSpriteCssResourceName;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterControlImagesAddButton"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties AddButton { get { return GetImage(AddButtonName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterControlImagesAddButtonHot"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties AddButtonHot { get { return GetImage(AddButtonHotName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterControlImagesRemoveButton"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties RemoveButton { get { return GetImage(RemoveButtonName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterControlImagesRemoveButtonHot"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties RemoveButtonHot { get { return GetImage(RemoveButtonHotName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterControlImagesAddCondition"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties AddCondition { get { return GetImage(AddConditionName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterControlImagesAddGroup"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties AddGroup { get { return GetImage(AddGroupName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterControlImagesRemoveGroup"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties RemoveGroup { get { return GetImage(RemoveGroupName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterControlImagesGroupTypeAnd"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties GroupTypeAnd { get { return GetImage(GroupTypeAndName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterControlImagesGroupTypeOr"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties GroupTypeOr { get { return GetImage(GroupTypeOrName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterControlImagesGroupTypeNotAnd"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties GroupTypeNotAnd { get { return GetImage(GroupTypeNotAndName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterControlImagesGroupTypeNotOr"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties GroupTypeNotOr { get { return GetImage(GroupTypeNotOrName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterControlImagesOperationAnyOf"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties OperationAnyOf { get { return GetImage(OperationAnyOfName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterControlImagesOperationBeginsWith"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties OperationBeginsWith { get { return GetImage(OperationBeginsWithName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterControlImagesOperationBetween"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties OperationBetween { get { return GetImage(OperationBetweenName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterControlImagesOperationContains"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties OperationContains { get { return GetImage(OperationContainsName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterControlImagesOperationDoesNotContain"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties OperationDoesNotContain { get { return GetImage(OperationDoesNotContainName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterControlImagesOperationDoesNotEqual"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties OperationDoesNotEqual { get { return GetImage(OperationDoesNotEqualName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterControlImagesOperationEndsWith"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties OperationEndsWith { get { return GetImage(OperationEndsWithName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterControlImagesOperationEquals"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties OperationEquals { get { return GetImage(OperationEqualsName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterControlImagesOperationGreater"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties OperationGreater { get { return GetImage(OperationGreaterName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterControlImagesOperationGreaterOrEqual"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties OperationGreaterOrEqual { get { return GetImage(OperationGreaterOrEqualName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterControlImagesOperationIsNotNull"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties OperationIsNotNull { get { return GetImage(OperationIsNotNullName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterControlImagesOperationIsNull"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties OperationIsNull { get { return GetImage(OperationIsNullName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterControlImagesOperationLess"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties OperationLess { get { return GetImage(OperationLessName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterControlImagesOperationLessOrEqual"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties OperationLessOrEqual { get { return GetImage(OperationLessOrEqualName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterControlImagesOperationLike"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties OperationLike { get { return GetImage(OperationLikeName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterControlImagesOperationNoneOf"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties OperationNoneOf { get { return GetImage(OperationNoneOfName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterControlImagesOperationNotBetween"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties OperationNotBetween { get { return GetImage(OperationNotBetweenName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterControlImagesOperationNotLike"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties OperationNotLike { get { return GetImage(OperationNotLikeName); } }
		protected override bool KeepDefaultSizes {
			get { return true; }
		}
	}
}
