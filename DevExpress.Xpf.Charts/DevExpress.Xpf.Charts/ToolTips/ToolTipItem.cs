﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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

using System.Windows;
namespace DevExpress.Xpf.Charts.Native {
	public class ToolTipItem : AnnotationItem  {
		readonly ToolTipPresentationData presentationData = new ToolTipPresentationData();
		Thickness contentMargin;
		Point position;
		ToolTipPosition toolTipPosition;
		DataTemplate contentTemplate;
		public Thickness ContentMargin {
			get { return contentMargin; }
			set {
				contentMargin = value;
				OnPropertyChanged("ContentMargin");
			}
		}
		public Point Position {
			get { return position; }
			set { 
				position = value;
				OnPropertyChanged("Position");
			}
		}
		public ToolTipPresentationData PresentationData { get { return presentationData; } }
		public bool ShowShadow {
			get { return Shadow != null; }
			set {
				Shadow = value ? CreateDefaultShadow() : null;
				OnPropertyChanged("ShowShadow");
			}
		}
		public ToolTipPosition ToolTipPosition {
			get { return toolTipPosition; }
			set { 
				toolTipPosition = value;
				OnPropertyChanged("ToolTipPosition");
			}
		}
		public DataTemplate ContentTemplate {
			get { return contentTemplate; }
			set {
				contentTemplate = value;
				OnPropertyChanged("ContentTemplate");
			}
		}
	}
}
