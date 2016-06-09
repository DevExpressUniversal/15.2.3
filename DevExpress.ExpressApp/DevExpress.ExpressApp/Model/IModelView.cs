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
using System.ComponentModel;
namespace DevExpress.ExpressApp.Model {
	[DisplayProperty("Id")]
	[ModelAbstractClass]
	public interface IModelView : IModelNode {
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelViewId"),
#endif
 Required()]
		String Id { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelViewCaption"),
#endif
 Localizable(true)]
		String Caption { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelViewAllowEdit"),
#endif
 Category("Behavior")]
		Boolean AllowEdit { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelViewAllowDelete"),
#endif
 Category("Behavior")]
		[DefaultValue(true)]
		Boolean AllowDelete { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelViewAllowNew"),
#endif
 DefaultValue(true)]
		[Category("Behavior")]
		Boolean AllowNew { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelViewImageName"),
#endif
 Category("Appearance")]
		[Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.ImageGalleryModelEditorControl, DevExpress.ExpressApp.Win" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(System.Drawing.Design.UITypeEditor))]
		String ImageName { get; set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		IModelObjectView AsObjectView { get; }
	}
}
