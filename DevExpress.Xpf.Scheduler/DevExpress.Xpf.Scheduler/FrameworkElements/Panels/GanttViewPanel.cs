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
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using DevExpress.XtraScheduler;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Scheduler.Native;
using DevExpress.Xpf.Core;
using DependencyPropertyHelper = DevExpress.Xpf.Core.Native.DependencyPropertyHelper;
using DevExpress.Xpf.Scheduler.Internal;
using System.Diagnostics;
#if SILVERLIGHT
 using DevExpress.Xpf.Editors.Controls;
using DevExpress.Xpf.Core.WPFCompatibility;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using System.Collections.Specialized;
#else
using System.Collections.Specialized;
using System.Windows.Input;
using DevExpress.XtraScheduler.Drawing;
using System.Windows.Controls.Primitives;
using DevExpress.Xpf.Utils;
#endif
using DevExpress.Utils;
using System.Collections.ObjectModel;
using DevExpress.Xpf.Scheduler.Drawing.Components;
using DevExpress.XtraScheduler.Native;
namespace DevExpress.Xpf.Scheduler.Drawing {
	#region GanttViewGroupByDateLayoutPanel
	public class GantViewGroupByDateLayoutPanel : TimelineViewGroupByDateLayoutPanel {
	}
	#endregion    
}
