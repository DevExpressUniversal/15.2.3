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
using System.Globalization;
using System.Linq;
using System.Text;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.XtraRichEdit.Utils {
	class CultureInfoHelper {
		static Dictionary<int, string> dictionary;
		static CultureInfoHelper() { 
			dictionary= new Dictionary<int, string>();
			#region Dictionary
			dictionary.Add(1, "ar");
			dictionary.Add(2, "bg");
			dictionary.Add(3, "ca");
			dictionary.Add(4, "zh-Hans");
			dictionary.Add(5, "cs");
			dictionary.Add(6, "da");
			dictionary.Add(7, "de");
			dictionary.Add(8, "el");
			dictionary.Add(9, "en");
			dictionary.Add(10, "es");
			dictionary.Add(11, "fi");
			dictionary.Add(12, "fr");
			dictionary.Add(13, "he");
			dictionary.Add(14, "hu");
			dictionary.Add(15, "is");
			dictionary.Add(16, "it");
			dictionary.Add(17, "ja");
			dictionary.Add(18, "ko");
			dictionary.Add(19, "nl");
			dictionary.Add(20, "no");
			dictionary.Add(21, "pl");
			dictionary.Add(22, "pt");
			dictionary.Add(23, "rm");
			dictionary.Add(24, "ro");
			dictionary.Add(25, "ru");
			dictionary.Add(26, "hr");
			dictionary.Add(27, "sk");
			dictionary.Add(28, "sq");
			dictionary.Add(29, "sv");
			dictionary.Add(30, "th");
			dictionary.Add(31, "tr");
			dictionary.Add(32, "ur");
			dictionary.Add(33, "id");
			dictionary.Add(34, "uk");
			dictionary.Add(35, "be");
			dictionary.Add(36, "sl");
			dictionary.Add(37, "et");
			dictionary.Add(38, "lv");
			dictionary.Add(39, "lt");
			dictionary.Add(40, "tg");
			dictionary.Add(41, "fa");
			dictionary.Add(42, "vi");
			dictionary.Add(43, "hy");
			dictionary.Add(44, "az");
			dictionary.Add(45, "eu");
			dictionary.Add(46, "hsb");
			dictionary.Add(47, "mk");
			dictionary.Add(50, "tn");
			dictionary.Add(52, "xh");
			dictionary.Add(53, "zu");
			dictionary.Add(54, "af");
			dictionary.Add(55, "ka");
			dictionary.Add(56, "fo");
			dictionary.Add(57, "hi");
			dictionary.Add(58, "mt");
			dictionary.Add(59, "se");
			dictionary.Add(60, "ga");
			dictionary.Add(62, "ms");
			dictionary.Add(63, "kk");
			dictionary.Add(64, "ky");
			dictionary.Add(65, "sw");
			dictionary.Add(66, "tk");
			dictionary.Add(67, "uz");
			dictionary.Add(68, "tt");
			dictionary.Add(69, "bn");
			dictionary.Add(70, "pa");
			dictionary.Add(71, "gu");
			dictionary.Add(72, "or");
			dictionary.Add(73, "ta");
			dictionary.Add(74, "te");
			dictionary.Add(75, "kn");
			dictionary.Add(76, "ml");
			dictionary.Add(77, "as");
			dictionary.Add(78, "mr");
			dictionary.Add(79, "sa");
			dictionary.Add(80, "mn");
			dictionary.Add(81, "bo");
			dictionary.Add(82, "cy");
			dictionary.Add(83, "km");
			dictionary.Add(84, "lo");
			dictionary.Add(86, "gl");
			dictionary.Add(87, "kok");
			dictionary.Add(89, "sd");
			dictionary.Add(90, "syr");
			dictionary.Add(91, "si");
			dictionary.Add(92, "chr");
			dictionary.Add(93, "iu");
			dictionary.Add(94, "am");
			dictionary.Add(95, "tzm");
			dictionary.Add(97, "ne");
			dictionary.Add(98, "fy");
			dictionary.Add(99, "ps");
			dictionary.Add(100, "fil");
			dictionary.Add(101, "dv");
			dictionary.Add(103, "ff");
			dictionary.Add(104, "ha");
			dictionary.Add(106, "yo");
			dictionary.Add(107, "quz");
			dictionary.Add(108, "nso");
			dictionary.Add(109, "ba");
			dictionary.Add(110, "lb");
			dictionary.Add(111, "kl");
			dictionary.Add(112, "ig");
			dictionary.Add(115, "ti");
			dictionary.Add(117, "haw");
			dictionary.Add(120, "ii");
			dictionary.Add(122, "arn");
			dictionary.Add(124, "moh");
			dictionary.Add(126, "br");
			dictionary.Add(127, "");
			dictionary.Add(128, "ug");
			dictionary.Add(129, "mi");
			dictionary.Add(130, "oc");
			dictionary.Add(131, "co");
			dictionary.Add(132, "gsw");
			dictionary.Add(133, "sah");
			dictionary.Add(134, "qut");
			dictionary.Add(135, "rw");
			dictionary.Add(136, "wo");
			dictionary.Add(140, "prs");
			dictionary.Add(145, "gd");
			dictionary.Add(146, "ku");
			dictionary.Add(1025, "ar-SA");
			dictionary.Add(1026, "bg-BG");
			dictionary.Add(1027, "ca-ES");
			dictionary.Add(1028, "zh-TW");
			dictionary.Add(1029, "cs-CZ");
			dictionary.Add(1030, "da-DK");
			dictionary.Add(1031, "de-DE");
			dictionary.Add(1032, "el-GR");
			dictionary.Add(1033, "en-US");
			dictionary.Add(1035, "fi-FI");
			dictionary.Add(1036, "fr-FR");
			dictionary.Add(1037, "he-IL");
			dictionary.Add(1038, "hu-HU");
			dictionary.Add(1039, "is-IS");
			dictionary.Add(1040, "it-IT");
			dictionary.Add(1041, "ja-JP");
			dictionary.Add(1042, "ko-KR");
			dictionary.Add(1043, "nl-NL");
			dictionary.Add(1044, "nb-NO");
			dictionary.Add(1045, "pl-PL");
			dictionary.Add(1046, "pt-BR");
			dictionary.Add(1047, "rm-CH");
			dictionary.Add(1048, "ro-RO");
			dictionary.Add(1049, "ru-RU");
			dictionary.Add(1050, "hr-HR");
			dictionary.Add(1051, "sk-SK");
			dictionary.Add(1052, "sq-AL");
			dictionary.Add(1053, "sv-SE");
			dictionary.Add(1054, "th-TH");
			dictionary.Add(1055, "tr-TR");
			dictionary.Add(1056, "ur-PK");
			dictionary.Add(1057, "id-ID");
			dictionary.Add(1058, "uk-UA");
			dictionary.Add(1059, "be-BY");
			dictionary.Add(1060, "sl-SI");
			dictionary.Add(1061, "et-EE");
			dictionary.Add(1062, "lv-LV");
			dictionary.Add(1063, "lt-LT");
			dictionary.Add(1064, "tg-Cyrl-TJ");
			dictionary.Add(1065, "fa-IR");
			dictionary.Add(1066, "vi-VN");
			dictionary.Add(1067, "hy-AM");
			dictionary.Add(1068, "az-Latn-AZ");
			dictionary.Add(1069, "eu-ES");
			dictionary.Add(1070, "hsb-DE");
			dictionary.Add(1071, "mk-MK");
			dictionary.Add(1074, "tn-ZA");
			dictionary.Add(1076, "xh-ZA");
			dictionary.Add(1077, "zu-ZA");
			dictionary.Add(1078, "af-ZA");
			dictionary.Add(1079, "ka-GE");
			dictionary.Add(1080, "fo-FO");
			dictionary.Add(1081, "hi-IN");
			dictionary.Add(1082, "mt-MT");
			dictionary.Add(1083, "se-NO");
			dictionary.Add(1086, "ms-MY");
			dictionary.Add(1087, "kk-KZ");
			dictionary.Add(1088, "ky-KG");
			dictionary.Add(1089, "sw-KE");
			dictionary.Add(1090, "tk-TM");
			dictionary.Add(1091, "uz-Latn-UZ");
			dictionary.Add(1092, "tt-RU");
			dictionary.Add(1093, "bn-IN");
			dictionary.Add(1094, "pa-IN");
			dictionary.Add(1095, "gu-IN");
			dictionary.Add(1096, "or-IN");
			dictionary.Add(1097, "ta-IN");
			dictionary.Add(1098, "te-IN");
			dictionary.Add(1099, "kn-IN");
			dictionary.Add(1100, "ml-IN");
			dictionary.Add(1101, "as-IN");
			dictionary.Add(1102, "mr-IN");
			dictionary.Add(1103, "sa-IN");
			dictionary.Add(1104, "mn-MN");
			dictionary.Add(1105, "bo-CN");
			dictionary.Add(1106, "cy-GB");
			dictionary.Add(1107, "km-KH");
			dictionary.Add(1108, "lo-LA");
			dictionary.Add(1110, "gl-ES");
			dictionary.Add(1111, "kok-IN");
			dictionary.Add(1114, "syr-SY");
			dictionary.Add(1115, "si-LK");
			dictionary.Add(1116, "chr-Cher-US");
			dictionary.Add(1117, "iu-Cans-CA");
			dictionary.Add(1118, "am-ET");
			dictionary.Add(1121, "ne-NP");
			dictionary.Add(1122, "fy-NL");
			dictionary.Add(1123, "ps-AF");
			dictionary.Add(1124, "fil-PH");
			dictionary.Add(1125, "dv-MV");
			dictionary.Add(1128, "ha-Latn-NG");
			dictionary.Add(1130, "yo-NG");
			dictionary.Add(1131, "quz-BO");
			dictionary.Add(1132, "nso-ZA");
			dictionary.Add(1133, "ba-RU");
			dictionary.Add(1134, "lb-LU");
			dictionary.Add(1135, "kl-GL");
			dictionary.Add(1136, "ig-NG");
			dictionary.Add(1139, "ti-ET");
			dictionary.Add(1141, "haw-US");
			dictionary.Add(1144, "ii-CN");
			dictionary.Add(1146, "arn-CL");
			dictionary.Add(1148, "moh-CA");
			dictionary.Add(1150, "br-FR");
			dictionary.Add(1152, "ug-CN");
			dictionary.Add(1153, "mi-NZ");
			dictionary.Add(1154, "oc-FR");
			dictionary.Add(1155, "co-FR");
			dictionary.Add(1156, "gsw-FR");
			dictionary.Add(1157, "sah-RU");
			dictionary.Add(1158, "qut-GT");
			dictionary.Add(1159, "rw-RW");
			dictionary.Add(1160, "wo-SN");
			dictionary.Add(1164, "prs-AF");
			dictionary.Add(1169, "gd-GB");
			dictionary.Add(1170, "ku-Arab-IQ");
			dictionary.Add(2049, "ar-IQ");
			dictionary.Add(2051, "ca-ES-valencia");
			dictionary.Add(2052, "zh-CN");
			dictionary.Add(2055, "de-CH");
			dictionary.Add(2057, "en-GB");
			dictionary.Add(2058, "es-MX");
			dictionary.Add(2060, "fr-BE");
			dictionary.Add(2064, "it-CH");
			dictionary.Add(2067, "nl-BE");
			dictionary.Add(2068, "nn-NO");
			dictionary.Add(2070, "pt-PT");
			dictionary.Add(2074, "sr-Latn-CS");
			dictionary.Add(2077, "sv-FI");
			dictionary.Add(2092, "az-Cyrl-AZ");
			dictionary.Add(2094, "dsb-DE");
			dictionary.Add(2098, "tn-BW");
			dictionary.Add(2107, "se-SE");
			dictionary.Add(2108, "ga-IE");
			dictionary.Add(2110, "ms-BN");
			dictionary.Add(2115, "uz-Cyrl-UZ");
			dictionary.Add(2117, "bn-BD");
			dictionary.Add(2118, "pa-Arab-PK");
			dictionary.Add(2121, "ta-LK");
			dictionary.Add(2128, "mn-Mong-CN");
			dictionary.Add(2137, "sd-Arab-PK");
			dictionary.Add(2141, "iu-Latn-CA");
			dictionary.Add(2143, "tzm-Latn-DZ");
			dictionary.Add(2151, "ff-Latn-SN");
			dictionary.Add(2155, "quz-EC");
			dictionary.Add(2163, "ti-ER");
			dictionary.Add(3073, "ar-EG");
			dictionary.Add(3076, "zh-HK");
			dictionary.Add(3079, "de-AT");
			dictionary.Add(3081, "en-AU");
			dictionary.Add(3082, "es-ES");
			dictionary.Add(3084, "fr-CA");
			dictionary.Add(3098, "sr-Cyrl-CS");
			dictionary.Add(3131, "se-FI");
			dictionary.Add(3179, "quz-PE");
			dictionary.Add(4097, "ar-LY");
			dictionary.Add(4100, "zh-SG");
			dictionary.Add(4103, "de-LU");
			dictionary.Add(4105, "en-CA");
			dictionary.Add(4106, "es-GT");
			dictionary.Add(4108, "fr-CH");
			dictionary.Add(4122, "hr-BA");
			dictionary.Add(4155, "smj-NO");
			dictionary.Add(4191, "tzm-Tfng-MA");
			dictionary.Add(5121, "ar-DZ");
			dictionary.Add(5124, "zh-MO");
			dictionary.Add(5127, "de-LI");
			dictionary.Add(5129, "en-NZ");
			dictionary.Add(5130, "es-CR");
			dictionary.Add(5132, "fr-LU");
			dictionary.Add(5146, "bs-Latn-BA");
			dictionary.Add(5179, "smj-SE");
			dictionary.Add(6145, "ar-MA");
			dictionary.Add(6153, "en-IE");
			dictionary.Add(6154, "es-PA");
			dictionary.Add(6156, "fr-MC");
			dictionary.Add(6170, "sr-Latn-BA");
			dictionary.Add(6203, "sma-NO");
			dictionary.Add(7169, "ar-TN");
			dictionary.Add(7177, "en-ZA");
			dictionary.Add(7178, "es-DO");
			dictionary.Add(7194, "sr-Cyrl-BA");
			dictionary.Add(7227, "sma-SE");
			dictionary.Add(8193, "ar-OM");
			dictionary.Add(8201, "en-JM");
			dictionary.Add(8202, "es-VE");
			dictionary.Add(8218, "bs-Cyrl-BA");
			dictionary.Add(8251, "sms-FI");
			dictionary.Add(9217, "ar-YE");
			dictionary.Add(9225, "en-029");
			dictionary.Add(9226, "es-CO");
			dictionary.Add(9242, "sr-Latn-RS");
			dictionary.Add(9275, "smn-FI");
			dictionary.Add(10241, "ar-SY");
			dictionary.Add(10249, "en-BZ");
			dictionary.Add(10250, "es-PE");
			dictionary.Add(10266, "sr-Cyrl-RS");
			dictionary.Add(11265, "ar-JO");
			dictionary.Add(11273, "en-TT");
			dictionary.Add(11274, "es-AR");
			dictionary.Add(11290, "sr-Latn-ME");
			dictionary.Add(12289, "ar-LB");
			dictionary.Add(12297, "en-ZW");
			dictionary.Add(12298, "es-EC");
			dictionary.Add(12314, "sr-Cyrl-ME");
			dictionary.Add(13313, "ar-KW");
			dictionary.Add(13321, "en-PH");
			dictionary.Add(13322, "es-CL");
			dictionary.Add(14337, "ar-AE");
			dictionary.Add(14346, "es-UY");
			dictionary.Add(15361, "ar-BH");
			dictionary.Add(15370, "es-PY");
			dictionary.Add(16385, "ar-QA");
			dictionary.Add(16393, "en-IN");
			dictionary.Add(16394, "es-BO");
			dictionary.Add(17417, "en-MY");
			dictionary.Add(17418, "es-SV");
			dictionary.Add(18441, "en-SG");
			dictionary.Add(18442, "es-HN");
			dictionary.Add(19466, "es-NI");
			dictionary.Add(20490, "es-PR");
			dictionary.Add(21514, "es-US");
			dictionary.Add(25626, "bs-Cyrl");
			dictionary.Add(26650, "bs-Latn");
			dictionary.Add(27674, "sr-Cyrl");
			dictionary.Add(28698, "sr-Latn");
			dictionary.Add(28731, "smn");
			dictionary.Add(29740, "az-Cyrl");
			dictionary.Add(29755, "sms");
			dictionary.Add(30724, "zh");
			dictionary.Add(30740, "nn");
			dictionary.Add(30746, "bs");
			dictionary.Add(30764, "az-Latn");
			dictionary.Add(30779, "sma");
			dictionary.Add(30787, "uz-Cyrl");
			dictionary.Add(30800, "mn-Cyrl");
			dictionary.Add(30813, "iu-Cans");
			dictionary.Add(30815, "tzm-Tfng");
			dictionary.Add(31748, "zh-Hant");
			dictionary.Add(31764, "nb");
			dictionary.Add(31770, "sr");
			dictionary.Add(31784, "tg-Cyrl");
			dictionary.Add(31790, "dsb");
			dictionary.Add(31803, "smj");
			dictionary.Add(31811, "uz-Latn");
			dictionary.Add(31814, "pa-Arab");
			dictionary.Add(31824, "mn-Mong");
			dictionary.Add(31833, "sd-Arab");
			dictionary.Add(31836, "chr-Cher");
			dictionary.Add(31837, "iu-Latn");
			dictionary.Add(31839, "tzm-Latn");
			dictionary.Add(31847, "ff-Latn");
			dictionary.Add(31848, "ha-Latn");
			dictionary.Add(31890, "ku-Arab");					   
			#endregion
		}
		public static CultureInfo CreateCultureInfo(int key) {
			string nameLang;
			if (dictionary.TryGetValue(key, out nameLang)) {
				return new CultureInfo(nameLang);
			}
			else return null;
		}
	}
}
