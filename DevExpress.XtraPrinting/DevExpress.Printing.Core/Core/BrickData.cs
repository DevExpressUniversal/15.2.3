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
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;
namespace DevExpress.XtraPrinting.Native
{
	public static class BrickPropertyNames {
		public const string DetailPath = "Brick.DetailPath";
		public const string Value = "Brick.Value";
		public const string Url = "Brick.Url";
		public const string Hint = "Brick.Hint";
		public const string Id = "Brick.Id";
		public const string Target = "Brick.Target";
		public const string AnchorName = "Brick.AnchorName";
		public const string NavigationPair = "Brick.NavigationPair";
		public const string BookmarkInfo = "Brick.BookmarkInfo";
		public const string MergeValue = "Brick.MergeValue";
		public const string SummaryInProgress = "Brick.SummaryInProgress";
		public const string XlsxFormatString = "Brick.XlsxFormatString";
		public const string AngleString = "Brick.Angle";
	}
#if !DXPORTABLE
	public static class BrickAttachedProperties {
		public static readonly AttachedProperty<string> DetailPath = AttachedPropertyBase.Register<string>(BrickPropertyNames.DetailPath, typeof(Brick));
		public static readonly AttachedProperty<object> Value = AttachedPropertyBase.Register<object>(BrickPropertyNames.Value, typeof(Brick));
		public static readonly AttachedProperty<string> Url = AttachedPropertyBase.Register<string>(BrickPropertyNames.Url, typeof(Brick));
		public static readonly AttachedProperty<string> Hint = AttachedPropertyBase.Register<string>(BrickPropertyNames.Hint, typeof(Brick));
		public static readonly AttachedProperty<string> Id = AttachedPropertyBase.Register<string>(BrickPropertyNames.Id, typeof(Brick));
		public static readonly AttachedProperty<int> RowIndex = AttachedPropertyBase.Register<int>("RowIndex", typeof(Brick));
		public static readonly AttachedProperty<int> ParentID = AttachedPropertyBase.Register<int>("ParentID", typeof(Brick));
		public static readonly AttachedProperty<string> Target = AttachedPropertyBase.Register<string>(BrickPropertyNames.Target, typeof(Brick));
		public static readonly AttachedProperty<string> AnchorName = AttachedPropertyBase.Register<string>(BrickPropertyNames.AnchorName, typeof(Brick));
		public static readonly AttachedProperty<BrickPagePair> NavigationPair = AttachedPropertyBase.Register<BrickPagePair>(BrickPropertyNames.NavigationPair, typeof(Brick));
		public static readonly AttachedProperty<BookmarkInfo> BookmarkInfo = AttachedPropertyBase.Register<BookmarkInfo>(BrickPropertyNames.BookmarkInfo, typeof(Brick));
		public static readonly AttachedProperty<object> MergeValue = AttachedPropertyBase.Register<object>(BrickPropertyNames.MergeValue, typeof(Brick));
		public static readonly AttachedProperty<bool> SummaryInProgress = AttachedPropertyBase.Register<bool>(BrickPropertyNames.SummaryInProgress, typeof(Brick));
		public static readonly AttachedProperty<float> Angle = AttachedPropertyBase.Register<float>(BrickPropertyNames.AngleString, typeof(Brick));
		public static readonly AttachedProperty<string> XlsxFormatString = AttachedPropertyBase.Register<string>(BrickPropertyNames.XlsxFormatString, typeof(Brick));
	}
#endif
}
