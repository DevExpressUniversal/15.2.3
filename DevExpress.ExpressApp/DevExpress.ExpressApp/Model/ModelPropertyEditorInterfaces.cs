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
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.SystemModule;
namespace DevExpress.ExpressApp.Model {
#if !SL
	[DevExpressExpressAppLocalizedDescription("ModelIModelPropertyEditor")]
#endif
	public interface IModelPropertyEditor : IModelViewItem, IModelMemberViewItem {
	}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ModelIModelControlDetailItem")]
#endif
	public interface IModelControlDetailItem : IModelViewItem {
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelControlDetailItemControlTypeName"),
#endif
 Required()]
		[Category("Data")]
		string ControlTypeName { get; set; }
	}
	public interface ISupportControlAlignment {
		[DefaultValue(DevExpress.ExpressApp.Editors.StaticHorizontalAlign.NotSet)]
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("ISupportControlAlignmentHorizontalAlign"),
#endif
 Category("Appearance")]
		DevExpress.ExpressApp.Editors.StaticHorizontalAlign HorizontalAlign { get; set; }
		[DefaultValue(DevExpress.ExpressApp.Editors.StaticVerticalAlign.NotSet)]
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("ISupportControlAlignmentVerticalAlign"),
#endif
 Category("Appearance")]
		DevExpress.ExpressApp.Editors.StaticVerticalAlign VerticalAlign { get; set; }
	}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ModelIModelStaticText")]
#endif
	public interface IModelStaticText : IModelViewItem, ISupportControlAlignment {
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelStaticTextText"),
#endif
 Category("Data"), Localizable(true)]
		[Editor(DevExpress.ExpressApp.Utils.Constants.MultilineStringEditorType, typeof(System.Drawing.Design.UITypeEditor))]
		string Text { get; set; }
	}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ModelIModelStaticImage")]
#endif
	public interface IModelStaticImage : IModelViewItem, ISupportControlAlignment {
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelStaticImageImageName"),
#endif
 Category("Data")]
		[Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.ImageGalleryModelEditorControl, DevExpress.ExpressApp.Win" +XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(System.Drawing.Design.UITypeEditor))]
		string ImageName { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelStaticImageSizeMode"),
#endif
 Category("Behavior")]
		DevExpress.Persistent.Base.ImageSizeMode SizeMode { get; set; }
	}
	public enum ActionContainerOrientation { Horizontal, Vertical };
#if !SL
	[DevExpressExpressAppLocalizedDescription("ModelIModelActionContainerViewItem")]
#endif
	public interface IModelActionContainerViewItem : IModelViewItem {
		[DataSourceProperty("Application.ActionDesign.ActionToContainerMapping")]
#if !SL
	[DevExpressExpressAppLocalizedDescription("IModelActionContainerViewItemActionContainer")]
#endif
		[Category("Data")]
		IModelActionContainer ActionContainer { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelActionContainerViewItemPaintStyle"),
#endif
 DefaultValue(DevExpress.ExpressApp.Templates.ActionItemPaintStyle.Default)]
		[Category("Appearance")]
		DevExpress.ExpressApp.Templates.ActionItemPaintStyle PaintStyle { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelActionContainerViewItemOrientation"),
#endif
 DefaultValue(ActionContainerOrientation.Horizontal)]
		ActionContainerOrientation Orientation { get; set; }
	}
}
