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
using System.Linq;
using System.Text;
using System.Drawing;
namespace DevExpress.ExpressApp.Workflow {
	public class ActivityInformation {
		public const string DefaultActivityCategory = "Common";
		private Type activityType;
		private string displayName;
		private string category;
		private Image image;
		public ActivityInformation(Type activityType) {
			this.activityType = activityType;
		}
		public ActivityInformation(Type activityType, string category)
			: this(activityType) {
			this.category = category;
		}
		public ActivityInformation(Type activityType, string category, string displayName)
			: this(activityType, category) {
			this.displayName = displayName;
		}
		public ActivityInformation(Type activityType, Image image) : this(activityType) {
			this.image = image;
		}
		public ActivityInformation(Type activityType, string category, Image image)
			: this(activityType, category) {
			this.image = image;
		}
		public ActivityInformation(Type activityType, string category, string displayName, Image image)
			: this(activityType, category, displayName) {
			this.image = image;
		}
		public Type ActivityType {
			get { return activityType; }
		}
		public string DisplayName {
			get { 
					if(string.IsNullOrEmpty(displayName)) {
						if(activityType.Name.EndsWith("`1")) {
							return activityType.Name.Replace("`1", "<T>");
						}
						else
							return activityType.Name;
					}
					return displayName;
			}
		}
		public string Category {
			get { return string.IsNullOrEmpty(category) ? DefaultActivityCategory : category; }
		}
		public Image Image {
			get { return image; }
		}
	}
}
