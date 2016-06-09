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

#if SL
extern alias Platform;
using Platform::DevExpress.Xpf.Editors;
using Platform::DevExpress.Xpf.Editors.Settings;
#else
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Core.Design.Services.PropertyLinesProviders;
#endif
#if DEBUGTEST
using DevExpress.Xpf.Core.Design.Tests;
#endif
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using DevExpress.Xpf.Editors.Design;
namespace DevExpress.Xpf.Core.Design {
	public static class SmartTagPropertyLineRegistrator {
		static bool registered = false;
		public static void RegisterPropertyLines() {
			if(registered) return;
			registered = true;
			RegisterPropertyLineProvider(new ItemsControlPropertyLinesProvider());
			RegisterPropertyLineProvider(new SelectorPropertyLinesProvider());
			RegisterPropertyLineProvider(new TextBoxPropertyLinesProvider());
			RegisterPropertyLineProvider(new TextBlockPropertyLinesProvider());
			RegisterPropertyLineProvider(new ButtonBasePropertyLinesProvider(), false);
			RegisterPropertyLineProvider(new ToggleButtonPropertyLinesProvider(), false);
			RegisterPropertyLineProvider(new ContentControlPropertyLinesProvider(), false);
			RegisterPropertyLineProvider(new ImagePropertyLinesProvider());
			RegisterPropertyLineProvider(new BarManagerPropertyLinesProvider());
			RegisterPropertyLineProvider(new MainMenuControlPropertyLinesProvider());
			RegisterPropertyLineProvider(new ToolBarControlPropertyLinesProvider());
			RegisterPropertyLineProvider(new StatusBarControlPropertyLinesProvider());
			RegisterPropertyLineProvider(new LinksHolderPropertyLinesProvider());
			RegisterPropertyLineProvider(new BarPropertyLinesProvider());
			RegisterPropertyLineProvider(new BarItemPropertyLineProvider());
			RegisterPropertyLineProvider(new BarCheckItemPropertyLineProvider());
			RegisterPropertyLineProvider(new BarEditItemPropetyLineProvider());
			RegisterPropertyLineProvider(new BarItemLinkPropertyLineProvider());
			RegisterPropertyLineProvider(new BaseEditPropertyProvider(typeof(BaseEdit), PropertyTarget.Editor), false);
			RegisterPropertyLineProvider(new LookUpEditPropertyProviderBase(typeof(LookUpEditBase), PropertyTarget.Editor), false);
			RegisterPropertyLineProvider(new RangeBaseEditPropertyProvider(typeof(RangeBaseEdit), PropertyTarget.Editor), false);
			RegisterPropertyLineProvider(new TextEditPropertyProvider(typeof(TextEdit), PropertyTarget.Editor), false);
			RegisterPropertyLineProvider(new ButtonEditPropertyProvider(typeof(ButtonEdit), PropertyTarget.Editor), false);
			RegisterPropertyLineProvider(new SpinEditPropertyProvider(typeof(SpinEdit), PropertyTarget.Editor), false);
			RegisterPropertyLineProvider(new CheckEditPropertyProvider(typeof(CheckEdit), PropertyTarget.Editor), false);
			RegisterPropertyLineProvider(new DateEditPropertyProvider(typeof(DateEdit), PropertyTarget.Editor), false);
			RegisterPropertyLineProvider(new MemoEditPropertyProvider(typeof(MemoEdit), PropertyTarget.Editor), false);
			RegisterPropertyLineProvider(new FontEditPropertyProvider(typeof(FontEdit), PropertyTarget.Editor), false);
			RegisterPropertyLineProvider(new ListBoxEditPropertyProvider(typeof(ListBoxEdit), PropertyTarget.Editor), false);
			RegisterPropertyLineProvider(new ComboBoxEditPropertyProvider(typeof(ComboBoxEdit), PropertyTarget.Editor), false);
			RegisterPropertyLineProvider(new ProgressBarEditPropertyProvider(typeof(ProgressBarEdit), PropertyTarget.Editor), false);
			RegisterPropertyLineProvider(new TrackBarEditPropertyProvider(typeof(TrackBarEdit), PropertyTarget.Editor), false);
			RegisterPropertyLineProvider(new ImageEditPropertyProvider(typeof(ImageEdit), PropertyTarget.Editor), false);
			RegisterPropertyLineProvider(new ImageEditPropertyProvider(typeof(PopupImageEdit), PropertyTarget.Editor), false);
			RegisterPropertyLineProvider(new ColorEditPropertyProvider(typeof(ColorEdit), PropertyTarget.Editor), false);
			RegisterPropertyLineProvider(new ColorEditPropertyProvider(typeof(PopupColorEdit), PropertyTarget.Editor), false);
			RegisterPropertyLineProvider(new CalcEditPropertyProvider(typeof(PopupCalcEdit), PropertyTarget.Editor), false);
			RegisterPropertyLineProvider(new PasswordBoxEditPropertyProvider(typeof(PasswordBoxEdit), PropertyTarget.Editor), false);
			RegisterPropertyLineProvider(new DateNavigatorPropertyLinesProvider());
			RegisterPropertyLineProvider(new DateEditCalendarPropertyLinesProvider(), false);
			RegisterPropertyLineProvider(new CalcEditPropertyProvider(typeof(Calculator), PropertyTarget.Editor), false);
			RegisterPropertyLineProvider(new ButtonInfoPropertyLinesProvider());
			RegisterPropertyLineProvider(new DXTabControlPropertyLinesProvider());
			RegisterPropertyLineProvider(new DXTabItemPropertyLinesProvider());
			RegisterPropertyLineProvider(new DXTabControlScrollViewPropertyLinesViewModel());
			RegisterPropertyLineProvider(new DXTabControlMultiLineViewPropertyLinesViewModel());
			RegisterPropertyLineProvider(new TabControlStretchViewPropertyLinesViewModel());
#if !SL
			RegisterPropertyLineProvider(new WindowPropertyLinesProvider(), false);
			RegisterPropertyLineProvider(new DXWindowPropertyLinesProvider(), false);
			RegisterPropertyLineProvider(new UserControlPropertyLinesProvider());
			RegisterPropertyLineProvider(new PagePropertyLinesProvider());
			RegisterPropertyLineProvider(new RangeControlPropertyLinesProvider());
			RegisterPropertyLineProvider(new DateTimePickerPropertyLinesProvider(), false);
			RegisterPropertyLineProvider(new RangeBasePropertyLinesProvider());
			RegisterPropertyLineProvider(new ProgressBarPropertyLinesProvider());
			RegisterPropertyLineProvider(new CurrentWindowServicePropertyLinesProvider());
			RegisterPropertyLineProvider(new DialogServicePropertyLinesProvider(), false);
			RegisterPropertyLineProvider(new DispatcherServicePropertyLinesProvider());
			RegisterPropertyLineProvider(new DXMessageBoxServicePropertyLinesProvider());
			RegisterPropertyLineProvider(new DXSplashScreenServicePropertyLinesProvider());
			RegisterPropertyLineProvider(new NotificationServicePropertyLinesProvider());
			RegisterPropertyLineProvider(new WindowedDocumentUIServicePropertyLinesProvider());
			RegisterPropertyLineProvider(new ApplicationJumpListServicePropertyLinesProvider());
			RegisterPropertyLineProvider(new TaskbarButtonServicePropertyLinesProvider());
			RegisterPropertyLineProvider(new EnumItemsSourceBehaviorPropertyLinesProvider());
			RegisterPropertyLineProvider(new ConfirmationBehaviorPropertyLinesProvider());
			RegisterPropertyLineProvider(new EventToCommandPropertyLinesProvider(), false);
			RegisterPropertyLineProvider(new KeyToCommandPropertyLinesProvider());
			RegisterPropertyLineProvider(new BarSubItemThemeSelectorPropertyLinesProvider());
			RegisterPropertyLineProvider(new FunctionBindingBehaviorPropertyLinesProvider());
			RegisterPropertyLineProvider(new MethodToCommandBehaviorPropertyLinesProvider());
			RegisterPropertyLineProvider(new CompositeCommandBehaviorPropertyLinesProvider());
			RegisterPropertyLineProvider(new FilteringBehaviorPropertyLinesProvider());
			RegisterPropertyLineProvider(new BarCodeControlPropertyLineProvider());
			RegisterPropertyLineProvider(new CodabarStyleSettingsPropertyLineProvider());
			RegisterPropertyLineProvider(new Code11StyleSettingsPropertyLineProvider());
			RegisterPropertyLineProvider(new Code128StyleSettingsPropertyLineProvider());
			RegisterPropertyLineProvider(new Code39ExtendedStyleSettingsPropertyLineProvider());
			RegisterPropertyLineProvider(new Code39StyleSettingsPropertyLineProvider());
			RegisterPropertyLineProvider(new Code93ExtendedStyleSettingsPropertyLineProvider());
			RegisterPropertyLineProvider(new Code93StyleSettingsPropertyLineProvider());
			RegisterPropertyLineProvider(new CodeMSIStyleSettingsPropertyLineProvider());
			RegisterPropertyLineProvider(new DataBarStyleSettingsPropertyLineProvider());
			RegisterPropertyLineProvider(new DataMatrixGS1StyleSettingsPropertyLineProvider());
			RegisterPropertyLineProvider(new DataMatrixStyleSettingsPropertyLineProvider());
			RegisterPropertyLineProvider(new EAN128StyleSettingsPropertyLineProvider());
			RegisterPropertyLineProvider(new EAN13StyleSettingsPropertyLineProvider());
			RegisterPropertyLineProvider(new EAN8StyleSettingsPropertyLineProvider());
			RegisterPropertyLineProvider(new Industrial2of5StyleSettingsPropertyLineProvider());
			RegisterPropertyLineProvider(new IntelligentMailStyleSettingsPropertyLineProvider());
			RegisterPropertyLineProvider(new Interleaved2of5StyleSettingsPropertyLineProvider());
			RegisterPropertyLineProvider(new ITF14StyleSettingsPropertyLineProvider());
			RegisterPropertyLineProvider(new Matrix2of5StyleSettingsPropertyLineProvider());
			RegisterPropertyLineProvider(new PDF417StyleSettingsPropertyLineProvider());
			RegisterPropertyLineProvider(new PostNetStyleSettingsPropertyLineProvider());
			RegisterPropertyLineProvider(new QRCodeStyleSettingsPropertyLineProvider());
			RegisterPropertyLineProvider(new UPCAStyleSettingsPropertyLineProvider());
			RegisterPropertyLineProvider(new UPCE0StyleSettingsPropertyLineProvider());
			RegisterPropertyLineProvider(new UPCE1StyleSettingsPropertyLineProvider());
			RegisterPropertyLineProvider(new UPCSupplemental2StyleSettingsPropertyLineProvider());
			RegisterPropertyLineProvider(new UPCSupplemental5StyleSettingsPropertyLineProvider());
			RegisterPropertyLineProvider(new Code128StyleSettingsBaseCode128GeneratorPropertyLineProvider());
			RegisterPropertyLineProvider(new Code39StyleSettingsBaseCode39ExtendedGeneratorPropertyLineProvider());
			RegisterPropertyLineProvider(new StyleSettingsCheckSumBaseCode39ExtendedGeneratorPropertyLineProvider());
			RegisterPropertyLineProvider(new Code39StyleSettingsBaseCode39GeneratorPropertyLineProvider());
			RegisterPropertyLineProvider(new StyleSettingsCheckSumBaseCode39GeneratorPropertyLineProvider());
			RegisterPropertyLineProvider(new StyleSettingsCheckSumBaseCode93ExtendedGeneratorPropertyLineProvider());
			RegisterPropertyLineProvider(new StyleSettingsCheckSumBaseCode93GeneratorPropertyLineProvider());
			RegisterPropertyLineProvider(new Code128StyleSettingsBaseEAN128GeneratorPropertyLineProvider());
			RegisterPropertyLineProvider(new Industrial2of5StyleSettingsBaseIndustrial2of5GeneratorPropertyLineProvider());
			RegisterPropertyLineProvider(new StyleSettingsCheckSumBaseIndustrial2of5GeneratorPropertyLineProvider());
			RegisterPropertyLineProvider(new Interleaved2of5StyleSettingsBaseInterleaved2of5GeneratorPropertyLineProvider());
			RegisterPropertyLineProvider(new Interleaved2of5StyleSettingsBaseITF14GeneratorPropertyLineProvider());
			RegisterPropertyLineProvider(new Industrial2of5StyleSettingsBaseMatrix2of5GeneratorPropertyLineProvider());
			RegisterPropertyLineProvider(new StyleSettingsCheckSumBaseMatrix2of5GeneratorPropertyLineProvider());
#else
			RegisterPropertyLineProvider(new DataPagerPropertyLinesProvider(), false);
#endif
			RegisterPropertyLineProvider(new BaseEditPropertyProvider(typeof(BaseEditSettings), PropertyTarget.Grid), false);
			RegisterPropertyLineProvider(new LookUpEditPropertyProviderBase(typeof(LookUpEditSettingsBase), PropertyTarget.Grid), false);
			RegisterPropertyLineProvider(new RangeBaseEditPropertyProvider(typeof(RangeBaseEditSettings), PropertyTarget.Grid), false);
			RegisterPropertyLineProvider(new TextEditPropertyProvider(typeof(TextEditSettings), PropertyTarget.Grid), false);
			RegisterPropertyLineProvider(new SpinEditPropertyProvider(typeof(SpinEditSettings), PropertyTarget.Grid), false);
			RegisterPropertyLineProvider(new CheckEditPropertyProvider(typeof(CheckEditSettings), PropertyTarget.Grid), false);
			RegisterPropertyLineProvider(new ComboBoxEditPropertyProvider(typeof(ComboBoxEditSettings), PropertyTarget.Grid), false);
			RegisterPropertyLineProvider(new DateEditPropertyProvider(typeof(DateEditSettings), PropertyTarget.Grid), false);
			RegisterPropertyLineProvider(new ButtonEditPropertyProvider(typeof(ButtonEditSettings), PropertyTarget.Grid), false);
			RegisterPropertyLineProvider(new MemoEditPropertyProvider(typeof(MemoEditSettings), PropertyTarget.Grid), false);
			RegisterPropertyLineProvider(new ImageEditPropertyProvider(typeof(ImageEditSettings), PropertyTarget.Grid), false);
			RegisterPropertyLineProvider(new ImageEditPropertyProvider(typeof(PopupImageEditSettings), PropertyTarget.Grid), false);
			RegisterPropertyLineProvider(new TrackBarEditPropertyProvider(typeof(TrackBarEditSettings), PropertyTarget.Grid), false);
#if !SILVERLIGHT
			RegisterPropertyLineProvider(new ProgressBarEditPropertyProvider(typeof(ProgressBarEditSettings), PropertyTarget.Grid), false);
#endif
			RegisterPropertyLineProvider(new CalcEditPropertyProvider(typeof(CalcEditSettings), PropertyTarget.Grid), false);
			RegisterPropertyLineProvider(new ColorEditPropertyProvider(typeof(ColorEditSettings), PropertyTarget.Grid), false);
			RegisterPropertyLineProvider(new ColorEditPropertyProvider(typeof(PopupColorEditSettings), PropertyTarget.Grid), false);
			RegisterPropertyLineProvider(new ListBoxEditPropertyProvider(typeof(ListBoxEditSettings), PropertyTarget.Grid), false);
			RegisterPropertyLineProvider(new PasswordBoxEditPropertyProvider(typeof(PasswordBoxEditSettings), PropertyTarget.Grid), false);
#if DEBUGTEST
			RegisterPropertyLineProvider(new TestWindowLinesProvider(), false);
			RegisterPropertyLineProvider(new ModesTestButtonPropertyLinesProvider(), false);
#endif
#if !SL
			RegisterPropertyLineProvider(new SparkLineEditPropertyProvider(typeof(SparklineEdit), PropertyTarget.Editor), false);
			RegisterPropertyLineProvider(new SparkLineEditPropertyProvider(typeof(SparklineEditSettings), PropertyTarget.Grid), false);
#endif
		}
		static void RegisterPropertyLineProvider(PropertyLinesProviderBase provider, bool isSealed = true) {
			FrameworkElementSmartTagPropertiesViewModel.RegisterPropertyLineProvider(provider, isSealed);
		}
	}
}
