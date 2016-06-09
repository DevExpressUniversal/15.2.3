ASPxClientSpreadsheet.Functions = [
	{
		name: "ABS",
		description: "החזרת הערך המוחלט של מספר, מספר ללא הסימן שלו.",
		arguments: [
			{
				name: "number",
				description: "המספר הממשי שעבורו אתה רוצה את הערך המוחלט"
			}
		]
	},
	{
		name: "ACCRINTM",
		description: "החזרת הריבית המצטברת עבור נייר ערך הנושא ריבית בעת הפירעון.",
		arguments: [
			{
				name: "issue",
				description: "תאריך ההנפקה של נייר הערך, המובע כמספר תאריך סידורי"
			},
			{
				name: "settlement",
				description: "תאריך הפירעון של נייר הערך, המובע כמספר תאריך סידורי"
			},
			{
				name: "rate",
				description: "שער השובר השנתי של נייר הערך"
			},
			{
				name: "par",
				description: "הערך הנקוב של נייר הערך"
			},
			{
				name: "basis",
				description: "סוג בסיס ספירת הימים שבו יש להשתמש"
			}
		]
	},
	{
		name: "ACOS",
		description: "החזרת הארק-קוסינוס של מספר, ברדיאנים בטווח שבין 0 ל- Pi. הארק-קוסינוס הוא הזווית שהקוסינוס שלה הוא number‏.",
		arguments: [
			{
				name: "number",
				description: "קוסינוס הזווית הרצויה, חייב להיות בין ‎-1 ל- 1‏"
			}
		]
	},
	{
		name: "ACOSH",
		description: "החזרת ארק-קוסינוס היפרבולי, ההופכי של מספר.",
		arguments: [
			{
				name: "number",
				description: "כל מספר ממשי השווה או גדול מ-1"
			}
		]
	},
	{
		name: "ACOT",
		description: "החזרת הקוטנגגנס ההופכי וההפוך של מספר ברדיאנים בטווח של 0 עד פאי.",
		arguments: [
			{
				name: "number",
				description: "הוא הקוטנגנס של הזווית הרצויה"
			}
		]
	},
	{
		name: "ACOTH",
		description: "החזרת הקוטנגנס ההיפרבולי ההפוך של מספר.",
		arguments: [
			{
				name: "number",
				description: "הוא הקוטנגנס ההיפרבולי של הזווית הרצויה"
			}
		]
	},
	{
		name: "ADDRESS",
		description: "יצירת הפניה לתא כטקסט, בהינתן מספרי שורה ועמודה.",
		arguments: [
			{
				name: "row_num",
				description: "מספר השורה לשימוש בהפניה לתא: Row_number = 1 עבור שורה 1"
			},
			{
				name: "column_num",
				description: "מספר העמודה לשימוש בהפניה לתא. לדוגמה, Column_number = 4 עבור עמודה D"
			},
			{
				name: "abs_num",
				description: "ציון סוג ההפניה: מוחלטת = 1; שורה מוחלטת/עמודה יחסית = 2; שורה יחסית/עמודה מוחלטת = 3; יחסית = 4"
			},
			{
				name: "a1",
				description: "ערך לוגי המציין את סגנון ההפניה: סגנון 1 = A1 או TRUE; סגנון R1C1 = 0 או FALSE"
			},
			{
				name: "sheet_text",
				description: "טקסט המציין את שם גליון העבודה שישמש כהפניה החיצונית"
			}
		]
	},
	{
		name: "AND",
		description: "בדיקה אם כל הארגומנטים הם TRUE, והחזרת TRUE אם כל הארגומנטים הם TRUE.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logical1",
				description: "הם 1 עד 255 מצבים שברצונך לבחון ואשר יכולים להיות או TRUE או FALSE ויכולים להיות ערכים לוגים, מערכיים או הפניות"
			},
			{
				name: "logical2",
				description: "הם 1 עד 255 מצבים שברצונך לבחון ואשר יכולים להיות או TRUE או FALSE ויכולים להיות ערכים לוגים, מערכיים או הפניות"
			}
		]
	},
	{
		name: "ARABIC",
		description: "המרת ספרה רומית לערבית.",
		arguments: [
			{
				name: "text",
				description: "הוא הספרה הרומית שברצונך להמיר"
			}
		]
	},
	{
		name: "AREAS",
		description: "החזרת מספר האזורים בהפניה. אזור הוא טווח של תאים רציפים או תא בודד.",
		arguments: [
			{
				name: "reference",
				description: "הפניה לתא או לטווח תאים, וכן למספר אזורים"
			}
		]
	},
	{
		name: "ASIN",
		description: "החזרת הארק-סינוס של מספר ברדיאנים, בטווח שבין ‎-Pi/2 ל- Pi/2‏.",
		arguments: [
			{
				name: "number",
				description: "סינוס הזווית הרצויה, חייב להיות בין 1 ל- ‎-1‏"
			}
		]
	},
	{
		name: "ASINH",
		description: "החזרת ארק-סינוס היפרבולי, ההופכי של מספר.",
		arguments: [
			{
				name: "number",
				description: "כל מספר ממשי השווה או גדול מ-1"
			}
		]
	},
	{
		name: "ATAN",
		description: "החזרת הארק-טנגנס של מספר ברדיאנים, בטווח שבין ‎-Pi/2 ל- Pi/2‏.",
		arguments: [
			{
				name: "number",
				description: "טנגנס הזווית הרצויה"
			}
		]
	},
	{
		name: "ATAN2",
		description: "החזרת הארק-טנגנס של קואורדינטות x ו- y שצוינו, ברדיאנים בטווח שבין ‎-Pi ל- Pi‏, לא כולל ‎-Pi‏.",
		arguments: [
			{
				name: "x_num",
				description: "קואורדינטת x של הנקודה"
			},
			{
				name: "y_num",
				description: "קואורדינטת y של הנקודה"
			}
		]
	},
	{
		name: "ATANH",
		description: "החזרת ארק-טנגנס היפרבולי, ההופכי של מספר.",
		arguments: [
			{
				name: "number",
				description: "כל מספר ממשי בין ‎-1 ל-1, לא כולל ‎-1 ו-1‏"
			}
		]
	},
	{
		name: "AVEDEV",
		description: "החזרת ממוצע הסטיות המוחלטות של נקודות נתונים מהממוצע שלהן. הארגומנטים יכולים להיות מספרים או שמות, מערכים או הפניות המכילים מספרים.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "1 עד 255 ארגומנטים עבורם ברצונך לחשב את ממוצע הסטיות המוחלטות"
			},
			{
				name: "number2",
				description: "1 עד 255 ארגומנטים עבורם ברצונך לחשב את ממוצע הסטיות המוחלטות"
			}
		]
	},
	{
		name: "AVERAGE",
		description: "החזרת הממוצע (ממוצע חשבוני) של הארגומנטים, היכולים להיות מספרים או שמות, הפניות או מערכים המכילים מספרים.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "1 עד 255 ארגומנטים נומריים עבורם ברצונך לחשב את הממוצע"
			},
			{
				name: "number2",
				description: "1 עד 255 ארגומנטים נומריים עבורם ברצונך לחשב את הממוצע"
			}
		]
	},
	{
		name: "AVERAGEA",
		description: "החזרת ממוצע (ממוצע חשבוני) של ארגומנטים. טקסט ו-FALSE בארגומנטים מוערכים כ-0; TRUE מוערך כ-1. הארגומנטים יכולים להיות מספרים, שמות, מערכים או הפניות.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "1 עד 255 ארגומנטים עבורם ברצונך לחשב את הממוצע"
			},
			{
				name: "value2",
				description: "1 עד 255 ארגומנטים עבורם ברצונך לחשב את הממוצע"
			}
		]
	},
	{
		name: "AVERAGEIF",
		description: "מציאת ממוצע (ממוצע אריתמטי) עבור התאים המצוינים על-ידי ערכה נתונה של תנאים או קריטריונים.",
		arguments: [
			{
				name: "range",
				description: "טווח התאים שברצונך שיוערכו"
			},
			{
				name: "criteria",
				description: "התנאי או הקריטריון בצורת מספר, ביטוי, או טקסט המגדיר אילו תאים ישמשו למציאת הממוצע"
			},
			{
				name: "average_range",
				description: "התאים הממשיים שבהם יש להשתמש כדי למצוא את הממוצע. אם מושמט, התאים שבטווח ישמשו לכך "
			}
		]
	},
	{
		name: "AVERAGEIFS",
		description: "מציאת ממוצע (ממוצע אריתמטי) עבור התאים המצוינים על-ידי ערכה נתונה של תנאים או קריטריונים.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "average_range",
				description: "התאים הממשיים שבהם יש להשתמש למציאת הממוצע."
			},
			{
				name: "criteria_range",
				description: "טווח התאים שברצונך שיוערכו עבור התנאי המסויים"
			},
			{
				name: "criteria",
				description: "התנאי או הקריטריון בצורת מספר, ביטוי, או טקסט המגדיר אילו תאים ישמשו למציאת הממוצע"
			}
		]
	},
	{
		name: "BAHTTEXT",
		description: "ממיר מספר לטקסט (בהט).",
		arguments: [
			{
				name: "number",
				description: "הוא מספר שברצונך להמיר"
			}
		]
	},
	{
		name: "BASE",
		description: "המרת מספר לייצוג טקסט עם השורש הנתון (בסיס).",
		arguments: [
			{
				name: "number",
				description: "הוא המספר שברצונך להמיר"
			},
			{
				name: "radix",
				description: "הוא שורש הבסיס שברצונך להמיר את המספר אליו"
			},
			{
				name: "min_length",
				description: "הוא האורך המינימלי של המחרוזת שהוחזרה, אם אפסים מובילים מושמטים אינם מתווספים"
			}
		]
	},
	{
		name: "BESSELI",
		description: "החזרת פונקציית בסל ששונתה In(x)‎.",
		arguments: [
			{
				name: "x",
				description: "הערך שבו יש להעריך את הפונקציה"
			},
			{
				name: "n",
				description: "הסדר של פונקציית בסל"
			}
		]
	},
	{
		name: "BESSELJ",
		description: "החזרת פונקציית בסל Jn(x)‎.",
		arguments: [
			{
				name: "x",
				description: "הערך בו יש להעריך את הפונקציה"
			},
			{
				name: "n",
				description: "הסדר של פונקציית בסל"
			}
		]
	},
	{
		name: "BESSELK",
		description: "החזרת פונקציית בסל ששונתה Kn(x)‎.",
		arguments: [
			{
				name: "x",
				description: "הערך בו יש להעריך את הפונקציה"
			},
			{
				name: "n",
				description: "הסדר של הפונקציה"
			}
		]
	},
	{
		name: "BESSELY",
		description: "החזרת פונקציית בסל Yn(x)‎.",
		arguments: [
			{
				name: "x",
				description: "הערך בו יש להעריך את הפונקציה"
			},
			{
				name: "n",
				description: "הסדר של הפונקציה"
			}
		]
	},
	{
		name: "BETA.DIST",
		description: "הפונקציה מחזירה את פונקציית ההתפלגות של הסתברות ביתא.",
		arguments: [
			{
				name: "x",
				description: "הוא הערך בין A ו- B שלפיו יש להעריך את הפונקציה"
			},
			{
				name: "alpha",
				description: "הוא פרמטר להתפלגות ועליו להיות גדול מ- 0"
			},
			{
				name: "beta",
				description: "הוא פרמטר להתפלגות ועליו להיות גדול מ- 0"
			},
			{
				name: "cumulative",
				description: "הוא ערך לוגי: עבור פונקציית ההתפלגות המצטברת, השתמש ב- TRUE; עבור פונקציית צפיפות ההסתברות, השתמש ב- FALSE"
			},
			{
				name: "A",
				description: "הוא גבול תחתון אופציונלי למרווח של x. אם מושמט, A = 0"
			},
			{
				name: "B",
				description: "הוא גבול עליון אופציונלי למרווח של x. אם מושמט, B = 1"
			}
		]
	},
	{
		name: "BETA.INV",
		description: "הפונקציה מחזירה את ההופכי של פונקציית צפיפות ההסתברות המצטברת של ביתא (BETA.DIST).",
		arguments: [
			{
				name: "probability",
				description: "הוא ההסתברות המשויכת להתפלגות הביתא"
			},
			{
				name: "alpha",
				description: "הוא פרמטר להתפלגות ועליו להיות גדול מ- 0"
			},
			{
				name: "beta",
				description: "הוא פרמטר להתפלגות ועליו להיות גדול מ- 0"
			},
			{
				name: "A",
				description: "הוא גבול תחתון אופציונלי למרווח של x. אם מושמט, A = 0"
			},
			{
				name: "B",
				description: "הוא גבול עליון אופציונלי למרווח של x. אם מושמט, B = 1"
			}
		]
	},
	{
		name: "BETADIST",
		description: "הפונקציה מחזירה את פונקציית צפיפות הסתברות הביתא המצטברת.",
		arguments: [
			{
				name: "x",
				description: "הערך בין A ל- B שבו יש להעריך את הפונקציה"
			},
			{
				name: "alpha",
				description: "פרמטר להתפלגות ועליו להיות גדול מ- 0"
			},
			{
				name: "beta",
				description: "פרמטר להתפלגות ועליו להיות גדול מ- 0"
			},
			{
				name: "A",
				description: "גבול תחתון אופציונלי למרווח של x. אם מושמט, A = 0"
			},
			{
				name: "B",
				description: "גבול עליון אופציונלי למרווח של x. אם מושמט, B = 1"
			}
		]
	},
	{
		name: "BETAINV",
		description: "הפונקציה מחזירה את ההופכי של פונקציית צפיפות הסתברות הביתא המצטברת (BETADIST).",
		arguments: [
			{
				name: "probability",
				description: "הסתברות המשויכת להתפלגות ביתא"
			},
			{
				name: "alpha",
				description: "פרמטר להתפלגות ועליו להיות גדול מ- 0"
			},
			{
				name: "beta",
				description: "פרמטר להתפלגות ועליו להיות גדול מ- 0"
			},
			{
				name: "A",
				description: "גבול תחתון אופציונלי למרווח של x. אם מושמט, A = 0"
			},
			{
				name: "B",
				description: "גבול עליון אופציונלי למרווח של x. אם מושמט, B = 1"
			}
		]
	},
	{
		name: "BIN2DEC",
		description: "המרת מספר בינארי לעשרוני.",
		arguments: [
			{
				name: "number",
				description: "המספר הבינארי שברצונך להמיר"
			}
		]
	},
	{
		name: "BIN2HEX",
		description: "המרת מספר בינארי להקסדצימאלי.",
		arguments: [
			{
				name: "number",
				description: "המספר הבינארי שברצונך להמיר"
			},
			{
				name: "places",
				description: "מספר התווים בהם יש להשתמש"
			}
		]
	},
	{
		name: "BIN2OCT",
		description: "המרת מספר בינארי לאוקטלי.",
		arguments: [
			{
				name: "number",
				description: "המספר הבינארי שברצונך להמיר"
			},
			{
				name: "places",
				description: "מספר התווים בהם יש להשתמש"
			}
		]
	},
	{
		name: "BINOM.DIST",
		description: "הפונקציה מחזירה את הסתברות ההתפלגות הבינומית של איבר בודד.",
		arguments: [
			{
				name: "number_s",
				description: "הוא מספר ההצלחות בניסיונות"
			},
			{
				name: "trials",
				description: "הוא מספר הניסיונות הבלתי תלויים"
			},
			{
				name: "probability_s",
				description: "הוא ההסתברות לההצלחה של כל ניסיון"
			},
			{
				name: "cumulative",
				description: "הוא ערך לוגי: עבור פונקציית ההתפלגות המצטברת, השתמש ב- TRUE; עבור פונקציית מסת ההסתברות, השתמש ב- FALSE"
			}
		]
	},
	{
		name: "BINOM.DIST.RANGE",
		description: "החזרת ההסתברות של תוצאת ניסוי באמצעות התפלגות בינומית.",
		arguments: [
			{
				name: "trials",
				description: "הוא מספר הניסויים העצמאיים"
			},
			{
				name: "probability_s",
				description: "הוא ההסתברות להצלחה בכל ניסוי"
			},
			{
				name: "number_s",
				description: "הוא מספר ההצלחות בניסוי"
			},
			{
				name: "number_s2",
				description: "אם פונקציה זו מחזירה את ההסתברות שמספר הניסויים המוצלחים יהיה בין number_s ו- number_s2"
			}
		]
	},
	{
		name: "BINOM.INV",
		description: "הפונקציה מחזירה את הערך הקטן ביותר שעבורו ההתפלגות הבינומית המצטברת גדולה מערך הקריטריון או שווה לו.",
		arguments: [
			{
				name: "trials",
				description: "הוא מספר ניסיונות ברנולי"
			},
			{
				name: "probability_s",
				description: "הוא ההסתברות להצלחה בכל ניסיון, מספר בין 0 ל- 1, כולל"
			},
			{
				name: "alpha",
				description: "הוא ערך הקריטריון, מספר בין 0 ל- 1, כולל"
			}
		]
	},
	{
		name: "BINOMDIST",
		description: "הפונקציה מחזירה את הסתברות ההתפלגות הבינומית של איבר בודד.",
		arguments: [
			{
				name: "number_s",
				description: "מספר ההצלחות בניסיונות"
			},
			{
				name: "trials",
				description: "מספר הניסיונות הבלתי תלויים"
			},
			{
				name: "probability_s",
				description: "ההסתברות להצלחה בכל ניסוי"
			},
			{
				name: "cumulative",
				description: "ערך לוגי: עבור פונקציית ההתפלגות המצטברת, השתמש ב- TRUE; עבור פונקציית מסת ההסתברות, השתמש ב- FALSE"
			}
		]
	},
	{
		name: "BITAND",
		description: "החזרת 'And' ברמת הסיבית הבודדת של שני מספרים.",
		arguments: [
			{
				name: "number1",
				description: "הוא הייצוג העשרוני של המספר הבינארי שאתה רוצה להעריך"
			},
			{
				name: "number2",
				description: "הוא הייצוג העשרוני של המספר הבינארי שאתה רוצה להעריך"
			}
		]
	},
	{
		name: "BITLSHIFT",
		description: "החזרת מספר שהוזז שמאלה על-ידי סיביות shift_amount.",
		arguments: [
			{
				name: "number",
				description: "הוא הייצוג העשרוני של המספר הבינארי שברצונך להעריך"
			},
			{
				name: "shift_amount",
				description: "הוא מספר הסיביות להזזת המספר שמאלה"
			}
		]
	},
	{
		name: "BITOR",
		description: "החזרת 'Or' ברמת הסיבית הבודדת של שני מספרים.",
		arguments: [
			{
				name: "number1",
				description: "הוא הייצוג העשרוני של המספר הבינארי שאתה רוצה להעריך"
			},
			{
				name: "number2",
				description: "הוא הייצוג העשרוני של המספר הבינארי שאתה רוצה להעריך"
			}
		]
	},
	{
		name: "BITRSHIFT",
		description: "החזרת מספר שהוזז ימינה על-ידי סיביות shift_amount.",
		arguments: [
			{
				name: "number",
				description: "הוא הייצוג העשרוני של המספר הבינארי שברצונך להעריך"
			},
			{
				name: "shift_amount",
				description: "הוא מספר הסיביות להזזת המספר ימינה"
			}
		]
	},
	{
		name: "BITXOR",
		description: "החזרת 'Exclusive Or' ברמת הסיבית הבודדת של שני מספרים.",
		arguments: [
			{
				name: "number1",
				description: "הוא הייצוג העשרוני של המספר הבינארי שאתה רוצה להעריך"
			},
			{
				name: "number2",
				description: "הוא הייצוג העשרוני של המספר הבינארי שאתה רוצה להעריך"
			}
		]
	},
	{
		name: "CEILING",
		description: "עיגול מספר כלפי מעלה לכפולה הקרובה ביותר של significance.",
		arguments: [
			{
				name: "number",
				description: "הערך שברצונך לעגל"
			},
			{
				name: "significance",
				description: "הכפולה אליה ברצונך לעגל"
			}
		]
	},
	{
		name: "CEILING.MATH",
		description: "עיגול מספר כלפי מעלה, למספר השלם הקרוב ביותר או לכפולה הקרובה ביותר שיש לה חשיבות.",
		arguments: [
			{
				name: "number",
				description: "הוא הערך שברצונך לעגל"
			},
			{
				name: "significance",
				description: "הוא הכפולה שאליה ברצונך לעגל"
			},
			{
				name: "mode",
				description: "כאשר פונקציה זו נתונה ואינה אפס, היא תעוגל הרחק מאפס"
			}
		]
	},
	{
		name: "CEILING.PRECISE",
		description: "הפונקציה מעגלת מספר כלפי מעלה, למספר השלם הקרוב ביותר או לכפולה הקרובה ביותר של significance.",
		arguments: [
			{
				name: "number",
				description: "הוא הערך שברצונך לעגל"
			},
			{
				name: "significance",
				description: "היא הכפולה שאליה ברצונך לעגל"
			}
		]
	},
	{
		name: "CELL",
		description: "החזרת מידע אודות העיצוב, המיקום או התוכן של התא הראשון, בהתאם לסדר הקריאה של הגיליון, בהפניה.",
		arguments: [
			{
				name: "info_type",
				description: "הוא ערך טקסט המציין את סוג המידע הרצוי אודות התא."
			},
			{
				name: "reference",
				description: " הוא התא שאודותיו ברצונך לקבל מידע"
			}
		]
	},
	{
		name: "CHAR",
		description: "החזרת התו שצוין על-ידי מספר הקוד מתוך ערכת התווים של המחשב שלך.",
		arguments: [
			{
				name: "number",
				description: "מספר בין 1 ל-255 המציין את התו הרצוי"
			}
		]
	},
	{
		name: "CHIDIST",
		description: "הפונקציה מחזירה את ההסתברות עם זנב ימני של התפלגות חי בריבוע.",
		arguments: [
			{
				name: "x",
				description: "הערך שבו ברצונך להעריך את ההתפלגות, מספר שאינו שלילי"
			},
			{
				name: "deg_freedom",
				description: "מספר דרגות החופש, מספר בין 1 ל- 10^10, לא כולל 10^10"
			}
		]
	},
	{
		name: "CHIINV",
		description: "הפונקציה מחזירה את ההופכי של ההסתברות עם זנב ימני של התפלגות חי בריבוע.",
		arguments: [
			{
				name: "probability",
				description: "הסתברות המשויכת להתפלגות חי בריבוע, ערך בין 0 ל- 1, כולל"
			},
			{
				name: "deg_freedom",
				description: "מספר דרגות החופש, מספר בין 1 ל- 10^10, לא כולל 10^10"
			}
		]
	},
	{
		name: "CHISQ.DIST",
		description: "הפונקציה מחזירה את הסתברות הזנב השמאלי של התפלגות חי בריבוע.",
		arguments: [
			{
				name: "x",
				description: "הוא הערך שלפיו ברצונך להעריך את ההתפלגות, מספר שאינו שלילי"
			},
			{
				name: "deg_freedom",
				description: "הוא מספר דרגות החופש, מספר בין 1 ל- 10^10, לא כולל 10^10"
			},
			{
				name: "cumulative",
				description: "הוא ערך לוגי להחזרה עבור הפונקציה: פונקציית ההתפלגות המצטברת = TRUE; פונקציית צפיפות ההסתברות = FALSE"
			}
		]
	},
	{
		name: "CHISQ.DIST.RT",
		description: "הפונקציה מחזירה את הסתברות הזנב הימני של התפלגות חי בריבוע.",
		arguments: [
			{
				name: "x",
				description: "הוא הערך שלפיו ברצונך להעריך את ההתפלגות, מספר שאינו שלילי"
			},
			{
				name: "deg_freedom",
				description: "הוא מספר דרגות החופש, מספר בין 1 ל- 10^10, לא כולל 10^10"
			}
		]
	},
	{
		name: "CHISQ.INV",
		description: "הפונקציה מחזירה את ההופכי של הסתברות הזנב השמאלי של התפלגות חי בריבוע.",
		arguments: [
			{
				name: "probability",
				description: "הוא ההסתברות המשויכת להתפלגות חי בריבוע, ערך בין 0 ל- 1, כולל"
			},
			{
				name: "deg_freedom",
				description: "הוא מספר דרגות החופש, מספר בין 1 ל- 10^10, לא כולל 10^10"
			}
		]
	},
	{
		name: "CHISQ.INV.RT",
		description: "הפונקציה מחזירה את ההופכי של הסתברות הזנב הימני של התפלגות חי בריבוע.",
		arguments: [
			{
				name: "probability",
				description: "הוא ההסתברות המשויכת להתפלגות חי בריבוע, ערך בין 0 ל- 1, כולל"
			},
			{
				name: "deg_freedom",
				description: "הוא מספר דרגות החופש, מספר בין 1 ל- 10^10, לא כולל 10^10"
			}
		]
	},
	{
		name: "CHISQ.TEST",
		description: "הפונקציה מחזירה מבחן אי-תלות: הערך מהתפלגות חי בריבוע עבור הנתון הסטטיסטי ודרגות החופש המתאימות.",
		arguments: [
			{
				name: "actual_range",
				description: "הוא טווח הנתונים שמכיל תצפיות לבדירה מול ערכים צפויים"
			},
			{
				name: "expected_range",
				description: "הוא טווח הנתונים שמכיל את היחס בין מכפלת סכומי השורות וסכומי העמודות לבין הסכום הכולל"
			}
		]
	},
	{
		name: "CHITEST",
		description: "הפונקציה מחזירה את מבחן האי-תלות: הערך מהתפלגות חי בריבוע עבור הסטטיסט ודרגת החופש המתאימה.",
		arguments: [
			{
				name: "actual_range",
				description: "טווח הנתונים המכיל תצפיות שיש לבדוק כנגד ערכים צפויים"
			},
			{
				name: "expected_range",
				description: "טווח הנתונים המכיל את היחס בין מכפלת סכומי השורות וסכומי העמודות לבין הסכום הכולל"
			}
		]
	},
	{
		name: "CHOOSE",
		description: "בחירת ערך או פעולה לביצוע מתוך רשימת ערכים, על-פי מספר אינדקס.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "index_num",
				description: "מציין איזה ערך ארגומנט נבחר. Index_num חייב להיות בין 1 ל-254, או נוסחה או הפניה למספר בין 1 ל- 254"
			},
			{
				name: "value1",
				description: "1 עד 254 מספרים, הפניות לתאים, שמות מוגדרים, נוסחאות, פונקציות או ארגומנטים של טקסט, שמתוכם CHOOSE בוחרת"
			},
			{
				name: "value2",
				description: "1 עד 254 מספרים, הפניות לתאים, שמות מוגדרים, נוסחאות, פונקציות או ארגומנטים של טקסט, שמתוכם CHOOSE בוחרת"
			}
		]
	},
	{
		name: "CLEAN",
		description: "הסרת כל התווים שאינם ניתנים להדפסה מטקסט.",
		arguments: [
			{
				name: "text",
				description: "כל מידע גליון עבודה ממנו ברצונך להסיר תווים שאינם ניתנים להדפסה"
			}
		]
	},
	{
		name: "CODE",
		description: "החזרת קוד נומרי עבור התו הראשון במחרוזת טקסט, בערכת התווים שבשימוש המחשב שלך.",
		arguments: [
			{
				name: "text",
				description: "הטקסט עבורו ברצונך לקבל את הקוד של התו הראשון"
			}
		]
	},
	{
		name: "COLUMN",
		description: "החזרת מספר העמודה של הפניה.",
		arguments: [
			{
				name: "reference",
				description: "התא או טווח התאים הרציפים עבורם ברצונך לקבל את מספר העמודה. אם reference יושמט, התא המכיל את פונקצית COLUMN ישמש לשם כך"
			}
		]
	},
	{
		name: "COLUMNS",
		description: "החזרת מספר העמודות במערך או בהפניה.",
		arguments: [
			{
				name: "array",
				description: "מערך או נוסחת מערך או הפניה לטווח תאים עבורם ברצונך לקבל את מספר העמודות"
			}
		]
	},
	{
		name: "COMBIN",
		description: "החזרת מספר הצירופים עבור מספר נתון של איברים.",
		arguments: [
			{
				name: "number",
				description: "מספרם הכולל של האיברים"
			},
			{
				name: "number_chosen",
				description: "מספר האיברים בכל צירוף"
			}
		]
	},
	{
		name: "COMBINA",
		description: "החזרת מספר השילובים עם חזרות עבור מספר נתון של פריטים.",
		arguments: [
			{
				name: "number",
				description: "הוא מספר הפריטים הכולל"
			},
			{
				name: "number_chosen",
				description: "הוא מספר הפריטים בכל שילוב"
			}
		]
	},
	{
		name: "COMPLEX",
		description: "המרת המקדם הממשי והמקדם המדומה למספר מרוכב.",
		arguments: [
			{
				name: "real_num",
				description: "המקדם הממשי של המספר המרוכב"
			},
			{
				name: "i_num",
				description: "המקדם המדומה של המספר המרוכב"
			},
			{
				name: "suffix",
				description: "הסיומת עבור המרכיב המדומה של המספר המרוכב"
			}
		]
	},
	{
		name: "CONCATENATE",
		description: "חיבור מספר מחרוזות טקסט למחרוזת טקסט אחת.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "text1",
				description: "1 עד 255 מחרוזות טקסט אותן יש לחבר למחרוזת טקסט אחת, אשר יכולות להיות מחרוזות טקסט, מספרים או הפניות לתאים בודדים"
			},
			{
				name: "text2",
				description: "1 עד 255 מחרוזות טקסט אותן יש לחבר למחרוזת טקסט אחת, אשר יכולות להיות מחרוזות טקסט, מספרים או הפניות לתאים בודדים"
			}
		]
	},
	{
		name: "CONFIDENCE",
		description: "הפונקציה מחזירה את מרווח הביטחון עבור ממוצע אוכלוסייה באמצעות התפלגות נורמלית.",
		arguments: [
			{
				name: "alpha",
				description: "רמת המובהקות המשמשת לחישוב רמת הביטחון, מספר גדול מ- 0 וקטן מ- 1"
			},
			{
				name: "standard_dev",
				description: "סטיית התקן של האוכלוסייה עבור טווח הנתונים ומניחים כי היא ידועה. Standard_dev חייב להיות גדול מ- 0"
			},
			{
				name: "size",
				description: "גודל המדגם"
			}
		]
	},
	{
		name: "CONFIDENCE.NORM",
		description: "הפונקציה מחזירה רווח בר-סמך עבור ממוצע אוכלוסיה באמצעות התפלגות נורמלית.",
		arguments: [
			{
				name: "alpha",
				description: "הוא רמת המובהקות המשמשת לחישוב רמת הרווח בר-סמך, מספר גדול מ- 0 וקטן מ- 1"
			},
			{
				name: "standard_dev",
				description: "הוא סטיית התקן של האוכלוסיה עבור טווח הנתונים ומניחים שהיא ידועה. Standard_dev חייב להיות גדול מ- 0"
			},
			{
				name: "size",
				description: "הוא גודל המדגם"
			}
		]
	},
	{
		name: "CONFIDENCE.T",
		description: "הפונקציה מחזירה רווח בר-סמך עבור ממוצע אוכלוסיה באמצעות התפלגות t של סטודנט.",
		arguments: [
			{
				name: "alpha",
				description: "הוא רמת המובהקות המשמשת לחישוב רמת הרווח בר-סמך, מספר גדול מ- 0 וקטן מ- 1"
			},
			{
				name: "standard_dev",
				description: "הוא סטיית התקן של האוכלוסיה עבור טווח הנתונים ומניחים שהיא ידועה. Standard_dev חייב להיות גדול מ- 0"
			},
			{
				name: "size",
				description: "הוא גודל המדגם"
			}
		]
	},
	{
		name: "CONVERT",
		description: "המרת מספר משיטת מדידה אחת לאחרת.",
		arguments: [
			{
				name: "number",
				description: "הערך ב- from_units שיש להמיר"
			},
			{
				name: "from_unit",
				description: "היחידות עבור number"
			},
			{
				name: "to_unit",
				description: "היחידות עבור התוצאה"
			}
		]
	},
	{
		name: "CORREL",
		description: "הפונקציה מחזירה את מקדם המתאם בין שתי ערכות נתונים.",
		arguments: [
			{
				name: "array1",
				description: "הוא טווח תאים של ערכים. הערכים אמורים להיות מספרים, שמות, מערכים או הפניות המכילות מספרים"
			},
			{
				name: "array2",
				description: "הוא טווח תאים שני של ערכים. הערכים אמורים להיות מספרים, שמות, מערכים או הפניות המכילות מספרים"
			}
		]
	},
	{
		name: "COS",
		description: "החזרת הקוסינוס של זווית.",
		arguments: [
			{
				name: "number",
				description: "הזווית ברדיאנים עבורה ברצונך לקבל את הקוסינוס"
			}
		]
	},
	{
		name: "COSH",
		description: "החזרת הקוסינוס ההיפרבולי של מספר.",
		arguments: [
			{
				name: "number",
				description: "כל מספר ממשי"
			}
		]
	},
	{
		name: "COT",
		description: "החזרת הקוטנגנס של זווית.",
		arguments: [
			{
				name: "number",
				description: "הוא הזווית ברדיאנים שעבורה אתה רוצה את הקוטנגנס"
			}
		]
	},
	{
		name: "COTH",
		description: "החזרת הקוטנגנס ההיפרבולי של מספר.",
		arguments: [
			{
				name: "number",
				description: "הוא הזווית ברדיאנים שעבורה אתה רוצה את הקוטנגנס ההיפרבולי"
			}
		]
	},
	{
		name: "COUNT",
		description: "מניית מספר התאים בטווח המכילים מספרים.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "1 עד 255 ארגומנטים שבאפשרותם להכיל או להפנות למגוון של סוגים שונים של נתונים, אך רק מספרים נמנים"
			},
			{
				name: "value2",
				description: "1 עד 255 ארגומנטים שבאפשרותם להכיל או להפנות למגוון של סוגים שונים של נתונים, אך רק מספרים נמנים"
			}
		]
	},
	{
		name: "COUNTA",
		description: "מניית מספר התאים בטווח שאינם ריקים.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "1 עד 255 ארגומנטים המייצגים את הערכים והתאים שברצונך למנות. הערכים יכולים להיות כל סוג מידע"
			},
			{
				name: "value2",
				description: "1 עד 255 ארגומנטים המייצגים את הערכים והתאים שברצונך למנות. הערכים יכולים להיות כל סוג מידע"
			}
		]
	},
	{
		name: "COUNTBLANK",
		description: "מונה את מספר התאים הריקים בטווח תאים שצוין.",
		arguments: [
			{
				name: "range",
				description: "הטווח שבו ברצונך למנות את התאים הריקים"
			}
		]
	},
	{
		name: "COUNTIF",
		description: "מניית מספר התאים בתוך טווח המקיימים את התנאי הנתון.",
		arguments: [
			{
				name: "range",
				description: "טווח התאים בו ברצונך למנות תאים שאינם ריקים"
			},
			{
				name: "criteria",
				description: "התנאי בצורת מספר, ביטוי או טקסט המגדיר אילו תאים יש למנות"
			}
		]
	},
	{
		name: "COUNTIFS",
		description: "מניית מספר התאים המצוין על-ידי ערכה נתונה של תנאים או קריטריונים.",
		arguments: [
			{
				name: "criteria_range",
				description: "טווח התאים שברצונך שיוערכו עבור התנאי המסוים"
			},
			{
				name: "criteria",
				description: "התנאי או הקריטריון בצורת מספר, ביטוי, או טקסט המגדיר אילו תאים ייספרו"
			}
		]
	},
	{
		name: "COUPDAYBS",
		description: "החזרת מספר הימים מאז תחילת תקופת השובר ועד לתאריך הסדרת החשבון.",
		arguments: [
			{
				name: "settlement",
				description: "תאריך הסדרת החשבון של נייר הערך, המובע כמספר תאריך סידורי"
			},
			{
				name: "maturity",
				description: "תאריך הפירעון של נייר הערך, המובע כמספר תאריך סידורי"
			},
			{
				name: "frequency",
				description: "מספר תשלומי השובר בשנה"
			},
			{
				name: "basis",
				description: "סוג בסיס ספירת הימים שבו יש להשתמש"
			}
		]
	},
	{
		name: "COUPNCD",
		description: "החזרת תאריך השובר הבא לאחר תאריך הסדרת החשבון.",
		arguments: [
			{
				name: "settlement",
				description: "תאריך הסדרת החשבון של נייר הערך, המובע כמספר תאריך סידורי"
			},
			{
				name: "maturity",
				description: "תאריך הפירעון של נייר הערך, המובע כמספר תאריך סידורי"
			},
			{
				name: "frequency",
				description: "מספר תשלומי שובר בשנה"
			},
			{
				name: "basis",
				description: "סוג בסיס ספירת הימים שבו יש להשתמש"
			}
		]
	},
	{
		name: "COUPNUM",
		description: "החזרת מספר השוברים לתשלום בין תאריך הסדרת החשבון לבין תאריך הפירעון.",
		arguments: [
			{
				name: "settlement",
				description: "תאריך הסדרת החשבון של נייר הערך, המובע כמספר תאריך סידורי"
			},
			{
				name: "maturity",
				description: "תאריך הפירעון של נייר הערך, המובע כמספר תאריך סידורי"
			},
			{
				name: "frequency",
				description: "מספר תשלומי שובר בשנה"
			},
			{
				name: "basis",
				description: "סוג בסיס ספירת הימים שבו יש להשתמש"
			}
		]
	},
	{
		name: "COUPPCD",
		description: "החזרת תאריך השובר הקודם לפני תאריך הסדרת החשבון.",
		arguments: [
			{
				name: "settlement",
				description: "תאריך הסדרת החשבון של נייר הערך, המובע כמספר תאריך סידורי"
			},
			{
				name: "maturity",
				description: "תאריך הפירעון של נייר הערך, המובע כמספר תאריך סידורי"
			},
			{
				name: "frequency",
				description: "מספר תשלומי שובר בשנה"
			},
			{
				name: "basis",
				description: "סוג בסיס ספירת הימים שבו יש להשתמש"
			}
		]
	},
	{
		name: "COVAR",
		description: "הפונקציה מחזירה שונות משותפת, ממוצע המכפלות של סטיות עבור כל זוג של נקודת נתונים בשתי סדרות נתונים.",
		arguments: [
			{
				name: "array1",
				description: "טווח התאים הראשון של מספרים שלמים, חייבים להיות מספרים, מערכים או הפניות המכילים מספרים"
			},
			{
				name: "array2",
				description: "טווח התאים השני של מספרים שלמים, חייבים להיות מספרים, מערכים או הפניות המכילים מספרים"
			}
		]
	},
	{
		name: "COVARIANCE.P",
		description: "הפונקציה מחזירה שונות משותפת של האוכלוסיה, ממוצע המכפלות של הסטיות עבור כל זוג נקודות נתונים בשתי ערכות נתונים.",
		arguments: [
			{
				name: "array1",
				description: "הוא טווח התאים הראשון של מספרים שלמים ועליו להיות מספרים, מערכים או הפניות המכילות מספרים"
			},
			{
				name: "array2",
				description: "הוא טווח התאים השני של מספרים שלמים ועליו להיות מספרים, מערכים או הפניות המכילות מספרים"
			}
		]
	},
	{
		name: "COVARIANCE.S",
		description: "הפונקציה מחזירה שונות משותפת של מדגם, ממוצע המכפלות של הסטיות עבור כל זוג נקודות נתונים בשתי ערכות נתונים.",
		arguments: [
			{
				name: "array1",
				description: "הוא טווח התאים הראשון של מספרים שלמים ועליו להיות מספרים, מערכים או הפניות המכילות מספרים"
			},
			{
				name: "array2",
				description: "הוא טווח התאים השני של מספרים שלמים ועליו להיות מספרים, מערכים או הפניות המכילות מספרים"
			}
		]
	},
	{
		name: "CRITBINOM",
		description: "הפונקציה  מחזירה את הערך הקטן ביותר שעבורו ההתפלגות הבינומית המצטברת גדולה מערך קריטריון או שווה לו.",
		arguments: [
			{
				name: "trials",
				description: "המספר של ניסויי ברנולי"
			},
			{
				name: "probability_s",
				description: "ההסתברות להצלחה בכל ניסוי, מספר בין 0 ל- 1, ועד בכלל"
			},
			{
				name: "alpha",
				description: "ערך הקריטריון, מספר בין 0 ל- 1, ועד בכלל"
			}
		]
	},
	{
		name: "CSC",
		description: "החזרת הקוסקנט של זווית.",
		arguments: [
			{
				name: "number",
				description: "הוא הזווית ברדיאנים שעבורה אתה רוצה את הקוסקנט"
			}
		]
	},
	{
		name: "CSCH",
		description: "החזרת הקוסקנט ההיפרבולי של זווית.",
		arguments: [
			{
				name: "number",
				description: "הוא הזווית ברדיאנים שעבורה אתה רוצה את הקוסקנט ההיפרבולי"
			}
		]
	},
	{
		name: "CUMIPMT",
		description: "החזרת הריבית המצטברת המשולמת בין שתי תקופות.",
		arguments: [
			{
				name: "rate",
				description: "שיעור הריבית"
			},
			{
				name: "nper",
				description: "המספר הכולל של תקופות התשלום"
			},
			{
				name: "pv",
				description: "הערך הנוכחי"
			},
			{
				name: "start_period",
				description: "התקופה הראשונה בחישוב"
			},
			{
				name: "end_period",
				description: "התקופה האחרונה בחישוב"
			},
			{
				name: "type",
				description: "תזמון התשלום"
			}
		]
	},
	{
		name: "CUMPRINC",
		description: "החזרת החזר הקרן המצטבר המשולם בגין הלוואה בין שתי תקופות.",
		arguments: [
			{
				name: "rate",
				description: "שיעור הריבית"
			},
			{
				name: "nper",
				description: "המספר הכולל של תקופות תשלום"
			},
			{
				name: "pv",
				description: "הערך הנוכחי"
			},
			{
				name: "start_period",
				description: "התקופה הראשונה בחישוב"
			},
			{
				name: "end_period",
				description: "התקופה האחרונה בחישוב"
			},
			{
				name: "type",
				description: "תזמון התשלום"
			}
		]
	},
	{
		name: "DATE",
		description: "החזרת המספר המייצג את התאריך בקוד תאריך-שעה של Spreadsheet.",
		arguments: [
			{
				name: "year",
				description: "מספר בין 1900 ל- 9999 ב- Spreadsheet for Windows או בין 1904 ל- 9999 ב- Spreadsheet for the Macintosh"
			},
			{
				name: "month",
				description: "מספר בין 1 ל- 12 המייצג את החודש בשנה"
			},
			{
				name: "day",
				description: "מספר בין 1 ל- 31 המייצג את היום בחודש"
			}
		]
	},
	{
		name: "DATEDIF",
		description: "",
		arguments: [
		]
	},
	{
		name: "DATEVALUE",
		description: "המרת תאריך בצורת טקסט למספר המייצג את התאריך בקוד תאריך-שעה של Spreadsheet.",
		arguments: [
			{
				name: "date_text",
				description: "טקסט המייצג תאריך בתבנית התאריך של Spreadsheet, בין 1/1/1900 (Windows) או 1/1/1904 (Macintosh) ל-31/12/9999"
			}
		]
	},
	{
		name: "DAVERAGE",
		description: "חישוב ממוצע הערכים בעמודה בתוך רשימה או מסד נתונים, המקיימים את התנאים שציינת.",
		arguments: [
			{
				name: "database",
				description: "טווח התאים היוצר את הרשימה או מסד הנתונים. מסד נתונים הוא רשימה של נתונים הקשורים זה לזה"
			},
			{
				name: "field",
				description: "תווית העמודה במרכאות כפולות או מספר המייצג את מיקום העמודה ברשימה"
			},
			{
				name: "criteria",
				description: "טווח התאים המכיל את התנאים שציינת. הטווח כולל תווית עמודה ותא אחד מתחת לתווית עבור התנאי"
			}
		]
	},
	{
		name: "DAY",
		description: "החזרת היום בחודש, מספר מ-1 עד 31.",
		arguments: [
			{
				name: "serial_number",
				description: "הוא מספר בקוד תאריך-שעה המשמש ב- Spreadsheet"
			}
		]
	},
	{
		name: "DAYS",
		description: "החזרת מספר הימים בין שני התאריכים.",
		arguments: [
			{
				name: "end_date",
				description: "start_date ו- end_date הם שני התאריכים שברצונך לדעת את מספר הימים ביניהם"
			},
			{
				name: "start_date",
				description: "start_date ו- end_date הם שני התאריכים שברצונך לדעת את מספר הימים ביניהם"
			}
		]
	},
	{
		name: "DAYS360",
		description: "החזרת מספר הימים בין שני תאריכים, בהתבסס על שנה בת 360 יום (שנים-עשר חודשים בני 30 יום).",
		arguments: [
			{
				name: "start_date",
				description: "start_date ו- end_date הם שני תאריכים שברצונך לדעת את מספר הימים ביניהם"
			},
			{
				name: "end_date",
				description: "start_date ו- end_date הם שני תאריכים שברצונך לדעת את מספר הימים ביניהם"
			},
			{
				name: "method",
				description: "הוא ערך לוגי המציין את שיטת החישוב: ארה''ב (FALSE = (NASD או מושמט; אירופאי = TRUE."
			}
		]
	},
	{
		name: "DB",
		description: "החזרת הפחת של נכס עבור תקופה נתונה באמצעות שיטת היתרה הפוחתת הקבועה.",
		arguments: [
			{
				name: "cost",
				description: "עלותו ההתחלתית של הנכס"
			},
			{
				name: "salvage",
				description: "ערך הניצולת בסוף חיי הנכס"
			},
			{
				name: "life",
				description: "מספר התקופות שבמהלכן פוחת ערכו של הנכס (נקרא לעיתים אורך החיים האפקטיבי של הנכס)"
			},
			{
				name: "period",
				description: "התקופה עבורה ברצונך לחשב את הפחת. period חייב להינתן באותן היחידות כמו life"
			},
			{
				name: "month",
				description: "מספר החודשים בשנה הראשונה. אם month מושמט, מניחים שהוא 12"
			}
		]
	},
	{
		name: "DCOUNT",
		description: "מניית התאים המכילים מספרים בשדה (עמודה) של רשומות במסד הנתונים המתאימים לתנאים שציינת.",
		arguments: [
			{
				name: "database",
				description: "טווח התאים היוצר את הרשימה או מסד הנתונים. מסד נתונים הוא רשימה של נתונים הקשורים זה לזה"
			},
			{
				name: "field",
				description: "תווית העמודה במרכאות כפולות או מספר המייצג את מיקום העמודה ברשימה"
			},
			{
				name: "criteria",
				description: "טווח התאים המכיל את התנאים שציינת. הטווח כולל תווית עמודה ותא אחד מתחת לתווית עבור התנאי"
			}
		]
	},
	{
		name: "DCOUNTA",
		description: "מניית תאים שאינם ריקים בשדה (עמודה) של רשומות במסד הנתונים המתאימים לתנאים שציינת.",
		arguments: [
			{
				name: "database",
				description: "טווח התאים היוצר את הרשימה או מסד הנתונים. מסד נתונים הוא רשימה של נתונים הקשורים זה לזה"
			},
			{
				name: "field",
				description: "תווית העמודה במרכאות כפולות או מספר המייצג את מיקום העמודה ברשימה"
			},
			{
				name: "criteria",
				description: "טווח התאים המכיל את התנאים שציינת. הטווח כולל תווית עמודה ותא אחד מתחת לתווית עבור התנאי"
			}
		]
	},
	{
		name: "DDB",
		description: "החזרת הפחת של נכס עבור תקופה נתונה באמצעות שיטת היתרה הפוחתת הכפולה או שיטה אחרת שציינת.",
		arguments: [
			{
				name: "cost",
				description: "עלותו ההתחלתית של הנכס"
			},
			{
				name: "salvage",
				description: "ערך הניצולת בסוף חיי הנכס"
			},
			{
				name: "life",
				description: "מספר התקופות שבמהלכן פוחת ערכו של הנכס (נקרא לעיתים אורך החיים האפקטיבי של הנכס)"
			},
			{
				name: "period",
				description: "התקופה עבורה ברצונך לחשב את הפחת. period חייב להינתן באותן היחידות כמו life"
			},
			{
				name: "factor",
				description: "השיעור בו פוחתת היתרה. אם factor מושמט, מניחים שהוא 2 (שיטת היתרה הפוחתת הכפולה)"
			}
		]
	},
	{
		name: "DEC2BIN",
		description: "המרת מספר עשרוני לבינארי.",
		arguments: [
			{
				name: "number",
				description: "המספר העשרוני השלם שברצונך להמיר"
			},
			{
				name: "places",
				description: "מספר התווים בהם יש להשתמש"
			}
		]
	},
	{
		name: "DEC2HEX",
		description: "המרת מספר עשרוני להקסדצימאלי.",
		arguments: [
			{
				name: "number",
				description: "המספר העשרוני השלם שברצונך להמיר"
			},
			{
				name: "places",
				description: "מספר התווים בהם יש להשתמש"
			}
		]
	},
	{
		name: "DEC2OCT",
		description: "המרת מספר עשרוני לאוקטלי.",
		arguments: [
			{
				name: "number",
				description: "המספר העשרוני השלם שברצונך להמיר"
			},
			{
				name: "places",
				description: "מספר התווים בהם יש להשתמש"
			}
		]
	},
	{
		name: "DECIMAL",
		description: "המרת ייצוג טקסט של מספר בבסיס נתון למספר עשרוני.",
		arguments: [
			{
				name: "number",
				description: "הוא המספר שברצונך להמיר"
			},
			{
				name: "radix",
				description: "הוא שורש הבסיס של המספר שאתה ממיר"
			}
		]
	},
	{
		name: "DEGREES",
		description: "המרת רדיאנים למעלות.",
		arguments: [
			{
				name: "angle",
				description: "הזווית ברדיאנים שברצונך להמיר"
			}
		]
	},
	{
		name: "DELTA",
		description: "בדיקה אם שני מספרים שווים.",
		arguments: [
			{
				name: "number1",
				description: "המספר הראשון"
			},
			{
				name: "number2",
				description: "המספר השני"
			}
		]
	},
	{
		name: "DEVSQ",
		description: "החזרת סכום ריבועי הסטיות של נקודות נתונים מתוך ממוצע המדגם שלהן.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "1 עד 255 ארגומנטים, מערך או הפניה למערך, בהם ברצונך ש- DEVSQ תשתמש לצורך החישוב"
			},
			{
				name: "number2",
				description: "1 עד 255 ארגומנטים, מערך או הפניה למערך, בהם ברצונך ש- DEVSQ תשתמש לצורך החישוב"
			}
		]
	},
	{
		name: "DGET",
		description: "חילוץ רשומה בודדת מתוך מסד נתונים המקיימת את התנאים שציינת.",
		arguments: [
			{
				name: "database",
				description: "טווח התאים היוצר את הרשימה או מסד הנתונים. מסד נתונים הוא רשימה של נתונים הקשורים זה לזה"
			},
			{
				name: "field",
				description: "תווית העמודה במרכאות כפולות או מספר המייצג את מיקום העמודה ברשימה"
			},
			{
				name: "criteria",
				description: "טווח התאים המכיל את התנאים שציינת. הטווח כולל תווית עמודה ותא אחד מתחת לתווית עבור התנאי"
			}
		]
	},
	{
		name: "DISC",
		description: "החזרת שער הנכיון עבור נייר ערך.",
		arguments: [
			{
				name: "settlement",
				description: "תאריך הסדרת החשבון של נייר הערך, המובע כמספר תאריך סידורי"
			},
			{
				name: "maturity",
				description: "תאריך הפירעון של נייר הערך, המובע כמספר תאריך סידורי"
			},
			{
				name: "pr",
				description: "מחיר נייר הערך לכל $100 ערך נקוב"
			},
			{
				name: "redemption",
				description: "ערך הפדיון של נייר הערך לכל $100 ערך נקוב"
			},
			{
				name: "basis",
				description: "סוג בסיס ספירת הימים שבו יש להשתמש"
			}
		]
	},
	{
		name: "DMAX",
		description: "החזרת המספר הגדול ביותר בשדה (עמודה) של רשומות במסד הנתונים המקיימות את התנאים שציינת.",
		arguments: [
			{
				name: "database",
				description: "טווח התאים היוצר את הרשימה או מסד הנתונים. מסד נתונים הוא רשימה של נתונים הקשורים זה לזה"
			},
			{
				name: "field",
				description: "תווית העמודה במרכאות כפולות או מספר המייצג את מיקום העמודה ברשימה"
			},
			{
				name: "criteria",
				description: "טווח התאים המכיל את התנאים שציינת. הטווח כולל תווית עמודה ותא אחד מתחת לתווית עבור התנאי"
			}
		]
	},
	{
		name: "DMIN",
		description: "החזרת המספר הקטן ביותר בשדה (עמודה) של רשומות במסד הנתונים המקיימות את התנאים שציינת.",
		arguments: [
			{
				name: "database",
				description: "טווח התאים היוצר את הרשימה או מסד הנתונים. מסד נתונים הוא רשימה של נתונים הקשורים זה לזה"
			},
			{
				name: "field",
				description: "תווית העמודה במרכאות כפולות או מספר המייצג את מיקום העמודה ברשימה"
			},
			{
				name: "criteria",
				description: "טווח התאים המכיל את התנאים שציינת. הטווח כולל תווית עמודה ותא אחד מתחת לתווית עבור התנאי"
			}
		]
	},
	{
		name: "DOLLAR",
		description: "המרת מספר לטקסט, באמצעות תבנית מטבע.",
		arguments: [
			{
				name: "number",
				description: "מספר, הפניה לתא המכיל מספר או נוסחה המוערכת למספר"
			},
			{
				name: "decimals",
				description: "מספר הספרות מימין לנקודה העשרונית. המספר מעוגל לפי הצורך. אם יושמט, Decimals = 2"
			}
		]
	},
	{
		name: "DOLLARDE",
		description: "המרת מחיר דולרי המובע כשבר למחיר דולרי המובע כמספר עשרוני.",
		arguments: [
			{
				name: "fractional_dollar",
				description: "מספר המובע כשבר"
			},
			{
				name: "fraction",
				description: "המספר השלם שבו יש להשתמש במכנה של השבר"
			}
		]
	},
	{
		name: "DOLLARFR",
		description: "המרת מחיר דולרי המובע כמספר עשרוני למחיר דולרי המובע כשבר.",
		arguments: [
			{
				name: "decimal_dollar",
				description: "מספר עשרוני"
			},
			{
				name: "fraction",
				description: "המספר השלם שבו יש להשתמש במכנה של השבר"
			}
		]
	},
	{
		name: "DPRODUCT",
		description: "הכפלת הערכים בשדה (עמודה) של רשומות במסד הנתונים המקיימות את התנאים שציינת.",
		arguments: [
			{
				name: "database",
				description: "טווח התאים היוצר את הרשימה או מסד הנתונים. מסד נתונים הוא רשימה של נתונים הקשורים זה לזה"
			},
			{
				name: "field",
				description: "תווית העמודה במרכאות כפולות או מספר המייצג את מיקום העמודה ברשימה"
			},
			{
				name: "criteria",
				description: "טווח התאים המכיל את התנאים שציינת. הטווח כולל תווית עמודה ותא אחד מתחת לתווית עבור התנאי"
			}
		]
	},
	{
		name: "DSTDEV",
		description: "הערכת סטיית התקן בהתבסס על מדגם מתוך ערכים נבחרים במסד נתונים.",
		arguments: [
			{
				name: "database",
				description: "טווח התאים היוצר את הרשימה או מסד הנתונים. מסד נתונים הוא רשימה של נתונים הקשורים זה לזה"
			},
			{
				name: "field",
				description: "תווית העמודה במרכאות כפולות או מספר המייצג את מיקום העמודה ברשימה"
			},
			{
				name: "criteria",
				description: "טווח התאים המכיל את התנאים שציינת. הטווח כולל תווית עמודה ותא אחד מתחת לתווית עבור התנאי"
			}
		]
	},
	{
		name: "DSTDEVP",
		description: "חישוב סטיית התקן בהתבסס על כלל האוכלוסייה של ערכים נבחרים במסד נתונים.",
		arguments: [
			{
				name: "database",
				description: "טווח התאים היוצר את הרשימה או מסד הנתונים. מסד נתונים הוא רשימה של נתונים הקשורים זה לזה"
			},
			{
				name: "field",
				description: "תווית העמודה במרכאות כפולות או מספר המייצג את מיקום העמודה ברשימה"
			},
			{
				name: "criteria",
				description: "טווח התאים המכיל את התנאים שציינת. הטווח כולל תווית עמודה ותא אחד מתחת לתווית עבור התנאי"
			}
		]
	},
	{
		name: "DSUM",
		description: "חיבור המספרים בשדה (עמודה) של רשומות במסד הנתונים המקיימות את התנאים שציינת.",
		arguments: [
			{
				name: "database",
				description: "טווח התאים היוצר את הרשימה או מסד הנתונים. מסד נתונים הוא רשימה של נתונים הקשורים זה לזה"
			},
			{
				name: "field",
				description: "תווית העמודה במרכאות כפולות או מספר המייצג את מיקום העמודה ברשימה"
			},
			{
				name: "criteria",
				description: "טווח התאים המכיל את התנאים שציינת. הטווח כולל תווית עמודה ותא אחד מתחת לתווית עבור התנאי"
			}
		]
	},
	{
		name: "DVAR",
		description: "הערכת שונות בהתבסס על מדגם מתוך ערכים נבחרים במסד נתונים.",
		arguments: [
			{
				name: "database",
				description: "טווח התאים היוצר את הרשימה או מסד הנתונים. מסד נתונים הוא רשימה של נתונים הקשורים זה לזה"
			},
			{
				name: "field",
				description: "תווית העמודה במרכאות כפולות או מספר המייצג את מיקום העמודה ברשימה"
			},
			{
				name: "criteria",
				description: "טווח התאים המכיל את התנאים שציינת. הטווח כולל תווית עמודה ותא אחד מתחת לתווית עבור התנאי"
			}
		]
	},
	{
		name: "DVARP",
		description: "הערכת שונות בהתבסס על כלל האוכלוסייה של ערכים נבחרים במסד נתונים.",
		arguments: [
			{
				name: "database",
				description: "טווח התאים היוצר את הרשימה או מסד הנתונים. מסד נתונים הוא רשימה של נתונים הקשורים זה לזה"
			},
			{
				name: "field",
				description: "תווית העמודה במרכאות כפולות או מספר המייצג את מיקום העמודה ברשימה"
			},
			{
				name: "criteria",
				description: "טווח התאים המכיל את התנאים שציינת. הטווח כולל תווית עמודה ותא אחד מתחת לתווית עבור התנאי"
			}
		]
	},
	{
		name: "EDATE",
		description: "החזרת המספר הסידורי של התאריך שהוא מספר החודשים המצוין לפני או אחרי תאריך ההתחלה.",
		arguments: [
			{
				name: "start_date",
				description: "מספר תאריך סידורי המייצג את תאריך ההתחלה"
			},
			{
				name: "months",
				description: "מספר החודשים לפני או אחרי start_date"
			}
		]
	},
	{
		name: "EFFECT",
		description: "החזרת שיעור הריבית האפקטיבית השנתית.",
		arguments: [
			{
				name: "nominal_rate",
				description: "שיעור הריבית הנומינלית"
			},
			{
				name: "npery",
				description: "מספר תקופות צבירה בשנה"
			}
		]
	},
	{
		name: "ENCODEURL",
		description: "מחזיר מחרוזת המקודדת באמצעות כתובת URL.",
		arguments: [
			{
				name: "טקסט",
				description: "הוא מחרוזת לקידוד באמצעות כתובת URL"
			}
		]
	},
	{
		name: "EOMONTH",
		description: "החזרת המספר הסידורי של היום האחרון בחודש לפני או אחרי מספר חודשים שצוין.",
		arguments: [
			{
				name: "start_date",
				description: "מספר תאריך סידורי המייצג את תאריך ההתחלה"
			},
			{
				name: "months",
				description: "מספר החודשים לפני או אחרי start_date"
			}
		]
	},
	{
		name: "ERF",
		description: "החזרת פונקציית השגיאה.",
		arguments: [
			{
				name: "lower_limit",
				description: "הגבול התחתון לשילוב ERF"
			},
			{
				name: "upper_limit",
				description: "הגבול העליון לשילוב ERF"
			}
		]
	},
	{
		name: "ERF.PRECISE",
		description: "הפונקציה מחזירה את פונקציית השגיאה.",
		arguments: [
			{
				name: "X",
				description: "הוא הגבול התחתון לשילוב ERF.PRECISE"
			}
		]
	},
	{
		name: "ERFC",
		description: "החזרת פונקציית השגיאה המשלימה.",
		arguments: [
			{
				name: "x",
				description: "הגבול התחתון לשילוב ERF"
			}
		]
	},
	{
		name: "ERFC.PRECISE",
		description: "הפונקציה מחזירה את פונקציית השגיאה המשלימה.",
		arguments: [
			{
				name: "X",
				description: "הוא הגבול התחתון לשילוב ERFC.PRECISE"
			}
		]
	},
	{
		name: "ERROR.TYPE",
		description: "החזרת מספר המתאים לערך שגיאה.",
		arguments: [
			{
				name: "error_val",
				description: "הוא ערך השגיאה שעבורו ברצונך לקבל את המספר המזהה, והוא יכול להיות ערך שגיאה ממשי או הפניה לתא המכיל ערך שגיאה"
			}
		]
	},
	{
		name: "EVEN",
		description: "עיגול מספר חיובי כלפי מעלה ומספר שלילי כלפי מטה לשלם הזוגי הקרוב ביותר.",
		arguments: [
			{
				name: "number",
				description: "הערך שיש לעגל"
			}
		]
	},
	{
		name: "EXACT",
		description: "בודק האם שתי מחרוזות טקסט זהות, ומחזיר TRUE או FALSE. מדויק הוא תלוי רישיות.",
		arguments: [
			{
				name: "text1",
				description: "הוא מחרוזת הטקסט הראשונה"
			},
			{
				name: "text2",
				description: "הוא מחרוזת הטקסט השניה"
			}
		]
	},
	{
		name: "EXP",
		description: "החזרת e מועלה בחזקה של מספר נתון.",
		arguments: [
			{
				name: "number",
				description: "המעריך המוחל על הבסיס e. הקבוע e שווה ל- 2.71828182845904, הבסיס של הלוגריתם הטבעי"
			}
		]
	},
	{
		name: "EXPON.DIST",
		description: "הפונקציה מחזירה את ההתפלגות המעריכית.",
		arguments: [
			{
				name: "x",
				description: "הוא ערך הפונקציה, מספר שאינו שלילי"
			},
			{
				name: "lambda",
				description: "הוא ערך הפרמטר, מספר חיובי"
			},
			{
				name: "cumulative",
				description: "הוא ערך לוגי להחזרה עבור הפונקציה: פונקציית ההתפלגות המצטברת = TRUE; פונקציית צפיפות ההסתברות = FALSE"
			}
		]
	},
	{
		name: "EXPONDIST",
		description: "הפונקציה מחזירה את ההתפלגות המעריכית.",
		arguments: [
			{
				name: "x",
				description: "ערך הפונקציה, מספר שאינו שלילי"
			},
			{
				name: "lambda",
				description: "ערך הפרמטר, מספר חיובי"
			},
			{
				name: "cumulative",
				description: "ערך לוגי עבור הפונקציה להחזרה: פונקציית ההתפלגות המצטברת = TRUE; פונקציית צפיפות ההסתברות = FALSE"
			}
		]
	},
	{
		name: "F.DIST",
		description: "הפונקציה מחזירה התפלגות הסתברות F (זנב שמאלי) (דרגת הגיוון) עבור שתי ערכות נתונים.",
		arguments: [
			{
				name: "x",
				description: "הוא הערך שלפיו יש להעריך את הפונקציה, מספר שאינו שלילי"
			},
			{
				name: "deg_freedom1",
				description: "הוא דרגות החופש של המונה, מספר בין 1 ל- 10^10, לא כולל 10^10"
			},
			{
				name: "deg_freedom2",
				description: "הוא דרגות החופש של המכנה, מספר בין 1 ל- 10^10, לא כולל 10^10"
			},
			{
				name: "cumulative",
				description: "הוא ערך לוגי להחזרה עבור הפונקציה: פונקציית ההתפלגות המצטברת = TRUE; פונקציית צפיפות ההסתברות = FALSE"
			}
		]
	},
	{
		name: "F.DIST.RT",
		description: "הפונקציה מחזירה התפלגות הסתברות F (זנב ימני) (דרגת הגיוון) עבור שתי ערכות נתונים.",
		arguments: [
			{
				name: "x",
				description: "הוא הערך שלפיו יש להעריך את הפונקציה, מספר שאינו שלילי"
			},
			{
				name: "deg_freedom1",
				description: "הוא דרגות החופש של המונה, מספר בין 1 ל- 10^10, לא כולל 10^10"
			},
			{
				name: "deg_freedom2",
				description: "הוא דרגות החופש של המכנה, מספר בין 1 ל- 10^10, לא כולל 10^10"
			}
		]
	},
	{
		name: "F.INV",
		description: "הפונקציה מחזירה את ההופכי של התפלגות הסתברות F (זנב שמאלי): אם p = F.DIST(x,...)‎, אזי F.INV(p,...) = x.",
		arguments: [
			{
				name: "probability",
				description: "הוא ההסתברות המשויכת להתפלגות F המצטברת, מספר בין 0 ל- 1, כולל"
			},
			{
				name: "deg_freedom1",
				description: "הוא דרגות החופש של המונה, מספר בין 1 ל- 10^10, לא כולל 10^10"
			},
			{
				name: "deg_freedom2",
				description: "הוא דרגות החופש של המכנה, מספר בין 1 ל- 10^10, לא כולל 10^10"
			}
		]
	},
	{
		name: "F.INV.RT",
		description: "הפונקציה מחזירה את ההופכי של התפלגות הסתברות F (זנב ימני): אם p = F.DIST.RT(x,...)‎, אזי F.INV.RT(p,...) = x.",
		arguments: [
			{
				name: "probability",
				description: "הוא ההסתברות המשויכת להתפלגות F המצטברת, מספר בין 0 ל- 1, כולל"
			},
			{
				name: "deg_freedom1",
				description: "הוא דרגות החופש של המונה, מספר בין 1 ל- 10^10, לא כולל 10^10"
			},
			{
				name: "deg_freedom2",
				description: "הוא דרגות החופש של המכנה, מספר בין 1 ל- 10^10, לא כולל 10^10"
			}
		]
	},
	{
		name: "F.TEST",
		description: "הפונקציה מחזירה תוצאה של מבחן F, הסתברות דו-זנבית שהשונות ב- Array1 וב- Array2 אינה שונה באופן מובהק.",
		arguments: [
			{
				name: "array1",
				description: "הוא מערך הנתונים או טווח הנתונים הראשון שיכול להיות מספרים או שמות, מערכים או הפניות המכילות מספרים (המערכת מתעלמת מתאים ריקים"
			},
			{
				name: "array2",
				description: "הוא מערך הנתונים או טווח הנתונים השני שיכול להיות מספרים או שמות, מערכים או הפניות המכילות מספרים (המערכת מתעלמת מתאים ריקים)"
			}
		]
	},
	{
		name: "FACT",
		description: "מחזיר את ערך עצרת של מספר, השווה ל-1*2*3*...*מספר.",
		arguments: [
			{
				name: "number",
				description: "המספר הלא שלילי שברצונך לחשב את העצרת שלו"
			}
		]
	},
	{
		name: "FACTDOUBLE",
		description: "החזרת העצרת הכפולה של מספר.",
		arguments: [
			{
				name: "number",
				description: "הערך שעבורו יש להחזיר את העצרת הכפולה"
			}
		]
	},
	{
		name: "FALSE",
		description: "החזרת הערך הלוגי FALSE.",
		arguments: [
		]
	},
	{
		name: "FDIST",
		description: "הפונקציה מחזירה את התפלגות ההסתברות F (עם זנב ימני) (דרגת הגיוון) עבור שתי סדרות נתונים.",
		arguments: [
			{
				name: "x",
				description: "הערך שבו יש להעריך את הפונקציה, מספר שאינו שלילי"
			},
			{
				name: "deg_freedom1",
				description: "דרגות החופש של המונה, מספר בין 1 ל- 10^10, לא כולל 10^10"
			},
			{
				name: "deg_freedom2",
				description: "דרגות החופש של המכנה, מספר בין 1 ל- 10^10, לא כולל 10^10"
			}
		]
	},
	{
		name: "FIELD",
		description: "",
		arguments: [
			{
				name: "",
				description: ""
			}
		]
	},
	{
		name: "FIELDPICTURE",
		description: "",
		arguments: [
			{
				name: "",
				description: ""
			},
			{
				name: "",
				description: ""
			},
			{
				name: "",
				description: ""
			},
			{
				name: "",
				description: ""
			},
			{
				name: "",
				description: ""
			},
			{
				name: "",
				description: ""
			},
			{
				name: "",
				description: ""
			},
			{
				name: "",
				description: ""
			}
		]
	},
	{
		name: "FIND",
		description: "החזרת המצב ההתחלתי של מחרוזת טקסט אחת בתוך מחרוזת טקסט אחרת. FIND הוא תלוי רישיות.",
		arguments: [
			{
				name: "find_text",
				description: "הטקסט שברצונך לחפש. השתמש במרכאות כפולות (טקסט ריק) כדי למצוא את התו הראשון ב- Within_text. אין להשתמש בתווים כלליים"
			},
			{
				name: "within_text",
				description: "הטקסט המכיל את הטקסט שברצונך לחפש"
			},
			{
				name: "start_num",
				description: "ציון התו ממנו ברצונך להתחיל את החיפוש. התו הראשון ב- Within_text הוא תו מספר 1. אם יושמט, Start_num = 1"
			}
		]
	},
	{
		name: "FINV",
		description: "הפונקציה מחזירה את ההופכי של התפלגות ההסתברות F (עם זנב ימני): אם p ‎= FDIST(x,...)‎ אזי FINV(p,....) = x.",
		arguments: [
			{
				name: "probability",
				description: "הסתברות המשויכת להתפלגות המצטברת F, מספר בין 0 ל- 1, כולל"
			},
			{
				name: "deg_freedom1",
				description: "דרגות החופש של המונה, מספר בין 1 ל- 10^10, לא כולל 10^10"
			},
			{
				name: "deg_freedom2",
				description: "דרגות החופש של המכנה, מספר בין 1 ל- 10^10, לא כולל 10^10"
			}
		]
	},
	{
		name: "FISHER",
		description: "החזרת טרנספורמציית פישר.",
		arguments: [
			{
				name: "x",
				description: "הערך עבורו ברצונך לחשב את הטרנספורמציה, מספר בין ‎-1 ל- 1, ללא ‎-1 ו- 1‏"
			}
		]
	},
	{
		name: "FISHERINV",
		description: "החזרת ההופכי של טרנספורמציית פישר: אם (y = FISHER(x אזי FISHERINV(y) = x.",
		arguments: [
			{
				name: "y",
				description: "הערך עבורו ברצונך לחשב את ההופכי של הטרנספורמציה"
			}
		]
	},
	{
		name: "FIXED",
		description: "עיגול מספר למספר המקומות העשרוניים שצוין והחזרת התוצאה כטקסט עם או בלי פסיקים.",
		arguments: [
			{
				name: "number",
				description: "המספר שברצונך לעגל ולהמיר לטקסט"
			},
			{
				name: "decimals",
				description: "מספר הספרות מימין לנקודה העשרונית. אם יושמט, Decimals = 2"
			},
			{
				name: "no_commas",
				description: "ערך לוגי: ללא הצגת פסיקים בערך המוחזר = TRUE; עם הצגת פסיקים בערך המוחזר = FALSE או מושמט"
			}
		]
	},
	{
		name: "FLOOR",
		description: "עיגול מספר כלפי מטה לכפולה הקרובה ביותר של significance.",
		arguments: [
			{
				name: "number",
				description: "הערך המספרי שברצונך לעגל"
			},
			{
				name: "significance",
				description: "הכפולה אליה ברצונך לעגל. Number ו- Significance חייבים להיות שניהם חיוביים או שניהם שליליים"
			}
		]
	},
	{
		name: "FLOOR.MATH",
		description: "עיגול מספר כלפי מטה, למספר השלם הקרוב ביותר או לכפולה הקרובה ביותר שיש לה חשיבות.",
		arguments: [
			{
				name: "number",
				description: "הוא הערך שברצונך לעגל"
			},
			{
				name: "significance",
				description: "הוא הכפולה שאליה ברצונך לעגל"
			},
			{
				name: "mode",
				description: "כאשר פונקציה זו נתונה ואינה אפס, היא תעוגל כלפי אפס"
			}
		]
	},
	{
		name: "FLOOR.PRECISE",
		description: "הפונקציה מעגלת מספר כלפי מטה למספר השלם הקרוב ביותר או לכפולה הקרובה ביותר של significance.",
		arguments: [
			{
				name: "number",
				description: "הוא הערך המספרי שברצונך לעגל"
			},
			{
				name: "significance",
				description: "היא הכפולה שאליה ברצונך לעגל. "
			}
		]
	},
	{
		name: "FORECAST",
		description: "חישוב או ניבוי ערך עתידי לאורך מגמה ליניארית באמצעות ערכים קיימים.",
		arguments: [
			{
				name: "x",
				description: "נקודת הנתונים עבורה ברצונך לנבא ערך, חייב להיות ערך מספרי"
			},
			{
				name: "known_y's",
				description: "המערך או טווח הנתונים המספריים התלוי"
			},
			{
				name: "known_x's",
				description: "המערך או טווח הנתונים המספריים הבלתי-תלוי. השונות של Known_x's אינה יכולה להיות אפס"
			}
		]
	},
	{
		name: "FORMULATEXT",
		description: "החזרת נוסחה כמחרוזת.",
		arguments: [
			{
				name: "reference",
				description: "הוא הפניה לנוסחה"
			}
		]
	},
	{
		name: "FREQUENCY",
		description: "חישוב שכיחות המופע של ערכים בטווח ערכים והחזרת מערך אנכי של מספרים, הכולל פריט נוסף למספר הפריטים ב- Bins_array.",
		arguments: [
			{
				name: "data_array",
				description: "מערך או הפניה לסדרת ערכים עבורם ברצונך לספור תדירויות (אין התייחסות לתאים ריקים וטקסט)"
			},
			{
				name: "bins_array",
				description: "מערך של מרווחים או הפניה למרווחים שלתוכם ברצונך לקבץ את הערכים ב- data_array"
			}
		]
	},
	{
		name: "FTEST",
		description: "הפונקציה מחזירה את התוצאה של מבחן F, ההסתברות הדו-זנבית שהשונות ב- Array1 וב- Array2 אינה שונה באופן מובהק.",
		arguments: [
			{
				name: "array1",
				description: "מערך הנתונים או טווח הנתונים הראשון שיכול להיות מספרים או שמות, הפניות או מערכים המכילים מספרים (תאים ריקים אינם נלקחים בחשבון)"
			},
			{
				name: "array2",
				description: "מערך הנתונים או טווח הנתונים השני שיכול להיות מספרים או שמות, הפניות או מערכים המכילים מספרים (תאים ריקים אינם נלקחים בחשבון)"
			}
		]
	},
	{
		name: "FV",
		description: "החזרת הערך העתידי של השקעה בהתבסס על תשלומים תקופתיים קבועים ושיעור ריבית קבוע.",
		arguments: [
			{
				name: "rate",
				description: "שיעור הריבית לתקופה. לדוגמה, השתמש ב- 6%/4 לתשלומים רבעוניים ב-6% APR"
			},
			{
				name: "nper",
				description: "המספר הכולל של תקופות תשלום בהשקעה"
			},
			{
				name: "pmt",
				description: "התשלום המבוצע בכל תקופה, אינו יכול להשתנות לאורך חיי ההשקעה"
			},
			{
				name: "pv",
				description: "הערך הנוכחי, או הסכום הכולל שסדרת תשלומים עתידיים שווה ברגע זה. אם מושמט, Pv = 0"
			},
			{
				name: "type",
				description: "ערך המייצג את תזמון התשלום: תשלום בתחילת התקופה = 1; תשלום בסוף התקופה = 0 או מושמט"
			}
		]
	},
	{
		name: "FVSCHEDULE",
		description: "החזרת הערך העתידי של קרן התחלתית לאחר החלת סידרה של שיעורי ריבית מורכבים.",
		arguments: [
			{
				name: "principal",
				description: "הערך הנוכחי"
			},
			{
				name: "schedule",
				description: "מערך שיעורי ריבית שיש להחיל"
			}
		]
	},
	{
		name: "GAMMA",
		description: "החזרת ערך פונקציית גאמה.",
		arguments: [
			{
				name: "x",
				description: "הוא הערך שעבורו ברצונך לחשב גאמה"
			}
		]
	},
	{
		name: "GAMMA.DIST",
		description: "הפונקציה מחזירה את התפלגות הגאמה.",
		arguments: [
			{
				name: "x",
				description: "הוא הערך שלפיו ברצונך להעריך את ההתפלגות, מספר שאינו שלילי"
			},
			{
				name: "alpha",
				description: "הוא פרמטר להתפלגות, מספר חיובי"
			},
			{
				name: "beta",
				description: "הוא פרמטר להתפלגות, מספר חיובי. אם ביתא = 1, ‏GAMMA.DIST מחזירה את התפלגות הגאמה הסטנדרטית"
			},
			{
				name: "cumulative",
				description: "הוא ערך לוגי: החזרת פונקציית ההתפלגות המצטברת = TRUE; החזרת פונקציית מסת ההסתברות = FALSE או מושמט"
			}
		]
	},
	{
		name: "GAMMA.INV",
		description: "הפונקציה מחזירה את ההופכי של התפלגות הגאמה המצטברת: אם p = GAMMA.DIST(x,...)‎, אזי GAMMA.INV(p,...) = x.",
		arguments: [
			{
				name: "probability",
				description: "הוא ההסתברות המשויכת להתפלגות הגאמה, מספר בין 0 ל- 1, כולל"
			},
			{
				name: "alpha",
				description: "הוא פרמטר להתפלגות, מספר חיובי"
			},
			{
				name: "beta",
				description: "הוא פרמטר להתפלגות, מספר חיובי. אם ביתא = 1, ‏GAMMA.INV מחזירה את ההופכי של התפלגות הגאמה הסטנדרטית"
			}
		]
	},
	{
		name: "GAMMADIST",
		description: "הפונקציה מחזירה את התפלגות גאמא.",
		arguments: [
			{
				name: "x",
				description: "הערך שבו ברצונך להעריך את ההתפלגות, מספר שאינו שלילי"
			},
			{
				name: "alpha",
				description: "פרמטר להתפלגות, מספר חיובי"
			},
			{
				name: "beta",
				description: "פרמטר להתפלגות, מספר חיובי. אם beta = 1, מחזירה ‏GAMMADIST את התפלגות גאמא הסטנדרטית"
			},
			{
				name: "cumulative",
				description: "ערך לוגי: החזרת פונקציית ההתפלגות המצטברת = TRUE; החזרת פונקציית מסת ההסתברות = FALSE או מושמט"
			}
		]
	},
	{
		name: "GAMMAINV",
		description: "הפונקציה מחזירה את ההופכי של התפלגות גאמא המצטברת: אם p = GAMMADIST(x,...)‎, אזי GAMMAINV(p,...)‎ = x.",
		arguments: [
			{
				name: "probability",
				description: "ההסתברות המשויכת להתפלגות גאמא, מספר בין 0 ל- 1, כולל"
			},
			{
				name: "alpha",
				description: "פרמטר להתפלגות, מספר חיובי"
			},
			{
				name: "beta",
				description: "פרמטר להתפלגות, מספר חיובי. אם beta = 1, ‏GAMMAINV מחזירה את ההופכי של התפלגות גאמא הסטנדרטית"
			}
		]
	},
	{
		name: "GAMMALN",
		description: "החזרת הלוגריתם הטבעי של פונקצית גאמא.",
		arguments: [
			{
				name: "x",
				description: "הערך עבורו ברצונך לחשב את GAMMALN, מספר חיובי"
			}
		]
	},
	{
		name: "GAMMALN.PRECISE",
		description: "הפונקציה מחזירה את הלוגריתם הטבעי של פונקציית גאמה.",
		arguments: [
			{
				name: "x",
				description: "הוא הערך שעבורו ברצונך לחשב את GAMMALN.PRECISE, מספר חיובי"
			}
		]
	},
	{
		name: "GAUSS",
		description: "",
		arguments: [
		]
	},
	{
		name: "GCD",
		description: "החזרת המכנה המשותף הגדול ביותר.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "1 עד 255 ערכים"
			},
			{
				name: "number2",
				description: "1 עד 255 ערכים"
			}
		]
	},
	{
		name: "GEOMEAN",
		description: "החזרת הממוצע הגיאומטרי של מערך או טווח של נתונים מספריים חיוביים.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "1 עד 255 מספרים או שמות, מערכים או הפניות המכילים מספרים שעבורם ברצונך לחשב את הממוצע"
			},
			{
				name: "number2",
				description: "1 עד 255 מספרים או שמות, מערכים או הפניות המכילים מספרים שעבורם ברצונך לחשב את הממוצע"
			}
		]
	},
	{
		name: "GESTEP",
		description: "בדיקה אם מספר מסוים גדול מערך סף.",
		arguments: [
			{
				name: "number",
				description: "הערך לבדיקה מול step"
			},
			{
				name: "step",
				description: "ערך הסף"
			}
		]
	},
	{
		name: "GETPIVOTDATA",
		description: "חילוץ נתונים המאוחסנים ב- PivotTable.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "data_field",
				description: "שם של שדה הנתונים שממנו יש לחלץ את הנתונים"
			},
			{
				name: "pivot_table",
				description: "הפניה לתא או לטווח תאים ב- PivotTable המכילים את הנתונים שברצונך לאחזר"
			},
			{
				name: "field",
				description: "שדה שאליו יש להפנות"
			},
			{
				name: "item",
				description: "פריט שדה שאליו יש להפנות"
			}
		]
	},
	{
		name: "GROWTH",
		description: "מחזירה מספרים במגמת גידול מעריכי צפוי המתאימה לנקודות נתונים ידועות.",
		arguments: [
			{
				name: "known_y's",
				description: "סדרת ערכי-y הידועים לך בקשר המתואר על-ידי y = b*m^x, מערך או טווח של מספרים חיוביים"
			},
			{
				name: "known_x's",
				description: "סדרת ערכי-x אופציונלית שיתכן וידועים לך בקשר y = b*m^x, מערך או טווח בגודל ערכי Known_y"
			},
			{
				name: "new_x's",
				description: "ערכי-x חדשים עבורם ברצונך כי GROWTH יחזיר ערכי-y מתאימים"
			},
			{
				name: "const",
				description: "ערך לוגי: הקבוע b מחושב באופן רגיל אם Const = TRUE;  נקבע לערך 1 אם Const = FALSE או מושמט"
			}
		]
	},
	{
		name: "HARMEAN",
		description: "החזרת הממוצע ההרמוני של ערכת נתונים של מספרים חיוביים: ההופכי של הממוצע החשבוני של ההופכיים.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "1 עד 255 מספרים או שמות, מערכים או הפניות המכילים מספרים עבורם ברצונך לחשב את הממוצע ההרמוני"
			},
			{
				name: "number2",
				description: "1 עד 255 מספרים או שמות, מערכים או הפניות המכילים מספרים עבורם ברצונך לחשב את הממוצע ההרמוני"
			}
		]
	},
	{
		name: "HEX2BIN",
		description: "המרת מספר הקסדצימאלי לבינארי.",
		arguments: [
			{
				name: "number",
				description: "המספר ההקסדצימאלי שברצונך להמיר"
			},
			{
				name: "places",
				description: "מספר התווים בהם יש להשתמש"
			}
		]
	},
	{
		name: "HEX2DEC",
		description: "המרת מספר הקסדצימאלי לעשרוני.",
		arguments: [
			{
				name: "number",
				description: "המספר ההקסדצימאלי שברצונך להמיר"
			}
		]
	},
	{
		name: "HEX2OCT",
		description: "המרת מספר הקסדצימאלי לאוקטלי.",
		arguments: [
			{
				name: "number",
				description: "המספר ההקסדצימאלי שברצונך להמיר"
			},
			{
				name: "places",
				description: "מספר התווים בהם יש להשתמש"
			}
		]
	},
	{
		name: "HLOOKUP",
		description: "חיפוש ערך בשורה העליונה של טבלה או מערך של ערכים והחזרת הערך באותה עמודה מתוך שורה שציינת.",
		arguments: [
			{
				name: "lookup_value",
				description: "הערך שיש למצוא בשורה הראשונה של הטבלה. יכול להיות ערך, הפניה או מחרוזת טקסט"
			},
			{
				name: "table_array",
				description: "טבלה של טקסט, מספרים או ערכים לוגיים בה יש לחפש נתונים. Table_array יכול להיות הפניה לטווח או שם טווח"
			},
			{
				name: "row_index_num",
				description: "מספר השורה ב- Table_array ממנה יש להחזיר את הערך המתאים. השורה הראשונה של ערכים בטבלה היא שורה מספר 1"
			},
			{
				name: "range_lookup",
				description: "ערך לוגי: למציאת ההתאמה הקרובה ביותר בשורה העליונה (ממוין בסדר עולה) = TRUE או מושמט; מציאת התאמה מדויקת = FALSE"
			}
		]
	},
	{
		name: "HOUR",
		description: "החזרת השעה כמספר בין 0 ‎(‎12:00‎ ‎A.M‎.‎)‎ ל-23 ‎(‎11:00‎ ‎P.M‎.‎)‎‏.",
		arguments: [
			{
				name: "serial_number",
				description: "הוא מספר בקוד תאריך-שעה שבו משתמש Spreadsheet, או טקסט בתבנית שעה, כגון 16:48:00 או ‎4:48:00‎ ‎PM‎‏"
			}
		]
	},
	{
		name: "HYPERLINK",
		description: "יצירת קיצור דרך או קפיצה הפותחים מסמך המאוחסן בדיסק הקשיח שלך, בשרת רשת או באינטרנט.",
		arguments: [
			{
				name: "link_location",
				description: "הטקסט המציין את הנתיב ושם הקובץ של המסמך שיש לפתוח, מיקום בדיסק קשיח, כתובת UNC או נתיב URL"
			},
			{
				name: "friendly_name",
				description: "טקסט או מספר המוצג בתא. אם Friendly_name מושמט, מוצג בתא הטקסט של Link_location"
			}
		]
	},
	{
		name: "HYPGEOM.DIST",
		description: "הפונקציה מחזירה את ההתפלגות ההיפר-גיאומטרית.",
		arguments: [
			{
				name: "sample_s",
				description: "הוא מספר ההצלחות במדגם"
			},
			{
				name: "number_sample",
				description: "הוא גודל המדגם"
			},
			{
				name: "population_s",
				description: "הוא מספר ההצלחות באוכלוסיה"
			},
			{
				name: "number_pop",
				description: "הוא גודל האוכלוסיה"
			},
			{
				name: "cumulative",
				description: "הוא ערך לוגי: עבור פונקציית ההתפלגות המצטברת, השתמש ב- TRUE; עבור פונקציית צפיפות ההסתברות, השתמש ב- FALSE"
			}
		]
	},
	{
		name: "HYPGEOMDIST",
		description: "הפונקציה מחזירה את ההתפלגות ההיפר-גיאומטרית.",
		arguments: [
			{
				name: "sample_s",
				description: "מספר ההצלחות במדגם"
			},
			{
				name: "number_sample",
				description: "גודל המדגם"
			},
			{
				name: "population_s",
				description: "מספר ההצלחות באוכלוסייה"
			},
			{
				name: "number_pop",
				description: "גודל האוכלוסייה"
			}
		]
	},
	{
		name: "IF",
		description: " בודק האם התנאים נפגשים ומחזיר ערך אחד אם הוא TRUE וערך אחר אם הוא FALSE.",
		arguments: [
			{
				name: "logical_test",
				description: "כל ערך או ביטוי שניתן להעריכו כ- TRUE או FALSE"
			},
			{
				name: "value_if_true",
				description: "הערך המוחזר אם Logical_test הוא TRUE. אם Value_if_true מושמט, מוחזר TRUE. ניתן לקנן עד שבע פונקציות IF"
			},
			{
				name: "value_if_false",
				description: "הערך המוחזר אם Logical_test הוא FALSE. אם Value_if_false מושמט, מוחזר FALSE"
			}
		]
	},
	{
		name: "IFERROR",
		description: "אחזור ערך_אם_שגיאה אם הביטוי הוא שגיאה וערך הביטוי עצמו אם לא.",
		arguments: [
			{
				name: "value",
				description: "ערך או ביטוי או הפניה כלשהם"
			},
			{
				name: "value_if_error",
				description: "ערך או ביטוי או הפניה כלשהם"
			}
		]
	},
	{
		name: "IFNA",
		description: "החזרת הערך שאתה מציין אם תוצאת הביטוי היא ‎#N/A, אחרת זו החזרת התוצאה של הביטוי.",
		arguments: [
			{
				name: "value",
				description: "הוא כל ערך או ביטוי או הפניה"
			},
			{
				name: "value_if_na",
				description: "הוא כל ערך או ביטוי או הפניה"
			}
		]
	},
	{
		name: "IMABS",
		description: "החזרת הערך המוחלט (מודולוס) של מספר מרוכב.",
		arguments: [
			{
				name: "inumber",
				description: "מספר מרוכב שעבורו ברצונך לחשב את הערך המוחלט"
			}
		]
	},
	{
		name: "IMAGINARY",
		description: "החזרת המקדם המדומה של מספר מרוכב.",
		arguments: [
			{
				name: "inumber",
				description: "מספר מרוכב שעבורו ברצונך לחשב את המקדם המדומה"
			}
		]
	},
	{
		name: "IMARGUMENT",
		description: "החזרת הארגומנט q, זווית המבוטאת ברדיאנים.",
		arguments: [
			{
				name: "inumber",
				description: "מספר מרוכב שעבורו ברצונך לחשב את הארגומנט"
			}
		]
	},
	{
		name: "IMCONJUGATE",
		description: "החזרת הקונג'וגאט המרוכב של מספר מרוכב.",
		arguments: [
			{
				name: "inumber",
				description: "מספר מרוכב שעבורו ברצונך לחשב את הקונג'וגאט"
			}
		]
	},
	{
		name: "IMCOS",
		description: "החזרת הקוסינוס של מספר מרוכב.",
		arguments: [
			{
				name: "inumber",
				description: "מספר מרוכב שעבורו ברצונך לחשב את הקוסינוס"
			}
		]
	},
	{
		name: "IMCOSH",
		description: "החזרת הקוסינוס ההיפרבולי של מספר מרוכב.",
		arguments: [
			{
				name: "inumber",
				description: "הוא מספר מרוכב שעבורו ברצונך לקבל את הקוסינוס ההיפרבולי"
			}
		]
	},
	{
		name: "IMCOT",
		description: "החזרת הקוטנגנס של מספר מרוכב.",
		arguments: [
			{
				name: "inumber",
				description: "הוא מספר מרוכב שעבורו אתה רוצה את הקוטנגנס"
			}
		]
	},
	{
		name: "IMCSC",
		description: "החזרת הקוסקנט של מספר מרוכב.",
		arguments: [
			{
				name: "inumber",
				description: "הוא מספר מרוכב שעבורו אתה רוצה את הקוסקנט"
			}
		]
	},
	{
		name: "IMCSCH",
		description: "החזרת הקוסקנט ההיפרבולי של מספר מרוכב.",
		arguments: [
			{
				name: "inumber",
				description: "הוא מספר מרוכב שעבורו אתה רוצה את הקוסקנט ההיפרבולי"
			}
		]
	},
	{
		name: "IMDIV",
		description: "החזרת המנה של שני מספרים מרוכבים.",
		arguments: [
			{
				name: "inumber1",
				description: "המונה או המחולק המרוכב"
			},
			{
				name: "inumber2",
				description: "המכנה או המחלק המרוכב"
			}
		]
	},
	{
		name: "IMEXP",
		description: "החזרת המעריך של מספר מרוכב.",
		arguments: [
			{
				name: "inumber",
				description: "מספר מרוכב שעבורו ברצונך לחשב את המעריך"
			}
		]
	},
	{
		name: "IMLN",
		description: "החזרת הלוגריתם הטבעי של מספר מרוכב.",
		arguments: [
			{
				name: "inumber",
				description: "מספר מרוכב שעבורו ברצונך לחשב את הלוגריתם הטבעי"
			}
		]
	},
	{
		name: "IMLOG10",
		description: "החזרת הלוגריתם לפי בסיס 10 של מספר מרוכב.",
		arguments: [
			{
				name: "inumber",
				description: "מספר מרוכב שעבורו ברצונך לחשב את הלוגריתם המשותף"
			}
		]
	},
	{
		name: "IMLOG2",
		description: "החזרת הלוגריתם לפי בסיס 2 של מספר מרוכב.",
		arguments: [
			{
				name: "inumber",
				description: "מספר מרוכב שעבורו ברצונך לחשב את הלוגריתם לפי בסיס 2"
			}
		]
	},
	{
		name: "IMPOWER",
		description: "החזרת מספר מרוכב המועלה בחזקה של מספר שלם.",
		arguments: [
			{
				name: "inumber",
				description: "מספר מרוכב שברצונך להעלות בחזקה"
			},
			{
				name: "number",
				description: "החזקה בה ברצונך להעלות את המספר המרוכב"
			}
		]
	},
	{
		name: "IMPRODUCT",
		description: "החזרת המכפלה של 1 עד 255 מספרים מרוכבים.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "inumber1",
				description: "Inumber1,‏ Inumber2,... הם 1 עד 255 מספרים מרוכבים שיש להכפיל."
			},
			{
				name: "inumber2",
				description: "Inumber1,‏ Inumber2,... הם 1 עד 255 מספרים מרוכבים שיש להכפיל."
			}
		]
	},
	{
		name: "IMREAL",
		description: "החזרת המקדם הממשי של מספר מרוכב.",
		arguments: [
			{
				name: "inumber",
				description: "מספר מרוכב שעבורו ברצונך לחשב את המקדם הממשי"
			}
		]
	},
	{
		name: "IMSEC",
		description: "החזרת הסקאנס של מספר מרוכב.",
		arguments: [
			{
				name: "inumber",
				description: "הוא מספר מרוכב שעבורו אתה רוצה את הסקאנס"
			}
		]
	},
	{
		name: "IMSECH",
		description: "החזרת הסקאנס ההיפרבולי של מספר מרוכב.",
		arguments: [
			{
				name: "inumber",
				description: "הוא מספר מרוכב שעבורו אתה רוצה את הסקאנס ההיפרבולי"
			}
		]
	},
	{
		name: "IMSIN",
		description: "החזרת הסינוס של מספר מרוכב.",
		arguments: [
			{
				name: "inumber",
				description: "מספר מרוכב שעבורו ברצונך לחשב את הסינוס"
			}
		]
	},
	{
		name: "IMSINH",
		description: "החזרת הסינוס ההיפרבולי של מספר מרוכב.",
		arguments: [
			{
				name: "inumber",
				description: "הוא מספר מרוכב שעבורו ברצונך לקבל את הסינוס ההיפרבולי"
			}
		]
	},
	{
		name: "IMSQRT",
		description: "החזרת השורש הריבועי של מספר מרוכב.",
		arguments: [
			{
				name: "inumber",
				description: "מספר מרוכב שעבורו ברצונך לחשב את השורש הריבועי"
			}
		]
	},
	{
		name: "IMSUB",
		description: "החזרת ההפרש בין שני מספרים מרוכבים.",
		arguments: [
			{
				name: "inumber1",
				description: "המספר המרוכב ממנו יש להפחית את inumber2"
			},
			{
				name: "inumber2",
				description: "המספר המרוכב שאותו יש להפחית מ- inumber1"
			}
		]
	},
	{
		name: "IMSUM",
		description: "החזרת הסכום של מספרים מרוכבים.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "inumber1",
				description: "1 עד 255 מספרים מרוכבים שיש לסכם"
			},
			{
				name: "inumber2",
				description: "1 עד 255 מספרים מרוכבים שיש לסכם"
			}
		]
	},
	{
		name: "IMTAN",
		description: "החזרת הטנגנס של מספר מרוכב.",
		arguments: [
			{
				name: "inumber",
				description: "הוא מספר מרוכב שעבורו אתה רוצה את הטנגנס"
			}
		]
	},
	{
		name: "INDEX",
		description: "החזרת ערך או הפניה של התא בהצטלבות של שורה ועמודה מסוימות בטווח נתון.",
		arguments: [
			{
				name: "array",
				description: "הוא טווח של תאים או קבוע מערך."
			},
			{
				name: "row_num",
				description: "בוחר בשורה ב- Array או ב- Reference שמתוכה יש להחזיר ערך. אם משמיטים אותו, Column_num נדרש"
			},
			{
				name: "column_num",
				description: "בוחר את העמודה ב- Array  או ב-Reference שמתוכה יש להחזיר ערך. אם משמיטים אותו, Row_num נדרש"
			}
		]
	},
	{
		name: "INDIRECT",
		description: "החזרת ההפניה שצוינה על-ידי מחרוזת טקסט.",
		arguments: [
			{
				name: "ref_text",
				description: "הפניה לתא המכיל הפניה בסגנון A1 או סגנון R1C1, שם המוגדר כהפניה או הפניה לתא כמחרוזת טקסט"
			},
			{
				name: "a1",
				description: "ערך לוגי המציין את סוג ההפניה ב- Ref_text: הפניה מסוג FALSE = R1C1; הפניה מסוג TRUE = A1‎ או מושמט"
			}
		]
	},
	{
		name: "INFO",
		description: "החזרת מידע אודות סביבת ההפעלה הנוכחית.",
		arguments: [
			{
				name: "type_text",
				description: "הוא טקסט המציין את סוג המידע שאותו ברצונך להחזיר."
			}
		]
	},
	{
		name: "INT",
		description: "עיגול מספר מטה למספר השלם הקרוב ביותר.",
		arguments: [
			{
				name: "number",
				description: "המספר הממשי שברצונך לעגל מטה למספר שלם"
			}
		]
	},
	{
		name: "INTERCEPT",
		description: "חישוב הנקודה שבה יחתוך קו את ציר y בעזרת קו רגרסיית התאמה מיטבית המותווה דרך ערכי x וערכי y ידועים.",
		arguments: [
			{
				name: "known_y's",
				description: "הקבוצה התלויה של תצפיות או נתונים, יכולים להיות מספרים או שמות, הפניות או מערכים המכילים מספרים"
			},
			{
				name: "known_x's",
				description: "הקבוצה הבלתי-תלויה של תצפיות או נתונים, יכולים להיות מספרים או שמות, הפניות או מערכים המכילים מספרים"
			}
		]
	},
	{
		name: "INTRATE",
		description: "החזרת שיעור הריבית עבור נייר ערך שהושקע במלואו.",
		arguments: [
			{
				name: "settlement",
				description: "תאריך הסדרת החשבון של נייר הערך, המובע כמספר תאריך סידורי"
			},
			{
				name: "maturity",
				description: "תאריך הפירעון של נייר הערך, המובע כמספר תאריך סידורי"
			},
			{
				name: "investment",
				description: "הסכום שהושקע בנייר הערך"
			},
			{
				name: "redemption",
				description: "הסכום שאמור להתקבל עם הפירעון"
			},
			{
				name: "basis",
				description: "סוג בסיס ספירת הימים שבו יש להשתמש"
			}
		]
	},
	{
		name: "IPMT",
		description: "החזרת תשלום הריבית לתקופה נתונה עבור השקעה, בהתבסס על תשלומים תקופתיים קבועים ושיעור ריבית קבוע.",
		arguments: [
			{
				name: "rate",
				description: "שיעור הריבית לתקופה. לדוגמה, השתמש ב- 6%/4 לתשלומים רבעוניים ב-6% APR"
			},
			{
				name: "per",
				description: "התקופה שעבורה ברצונך למצוא את הריבית, חייב להיות בטווח שבין 1 ל- Nper"
			},
			{
				name: "nper",
				description: "המספר הכולל של תקופות תשלום בהשקעה"
			},
			{
				name: "pv",
				description: "הערך הנוכחי, או הסכום הכולל, שהוא שוויה העכשווי של סדרת תשלומים עתידיים"
			},
			{
				name: "fv",
				description: "הערך העתידי, או יתרת מזומנים שברצונך להשיג לאחר ביצוע התשלום האחרון. אם יושמט, Fv = 0"
			},
			{
				name: "type",
				description: "ערך לוגי המציין את תזמון התשלום: בסיום התקופה = 0 או מושמט, בתחילת התקופה = 1"
			}
		]
	},
	{
		name: "IRR",
		description: "החזרת שיעור התשואה הפנימי עבור סידרה של תזרימי מזומנים.",
		arguments: [
			{
				name: "values",
				description: "מערך או הפניה לתאים המכילים מספרים עבורם ברצונך לחשב את שיעור התשואה הפנימי"
			},
			{
				name: "guess",
				description: "מספר שלהערכתך קרוב לתוצאה של ‎0.1; IRR (עשרה אחוז) אם מושמט"
			}
		]
	},
	{
		name: "ISBLANK",
		description: "בודק האם ההפניה היא לתא ריק, ומחזיר  TRUE או FALSE.",
		arguments: [
			{
				name: "value",
				description: "iהוא התא או השם המפנים לתא שברצונך לבחון"
			}
		]
	},
	{
		name: "ISERR",
		description: "בודק אם ערך הוא שגיאה (‎#VALUE!, ‏‎#REF!, ‏‎#DIV/0!, ‏‎#NUM!, ‏‎#NAME? או ‎#NULL!) מונע #N/A, ומחזיר TRUE או FALSE.",
		arguments: [
			{
				name: "value",
				description: "הוא הערך שברצונך לבדוק. הערך יכול להפנות לתא, לנוסחה או לשם המפנה לתא, לנוסחה או לערך"
			}
		]
	},
	{
		name: "ISERROR",
		description: "בדיקה אם הערך הוא ערך שגיאה (?N/A, #VALUE!, #REF!, #DIV/0!, #NUM!, #NAME# או #NULL!) והחזרת TRUE או FALSE.",
		arguments: [
			{
				name: "value",
				description: " הערך שברצונך לבדוק. הערך יכול להפנות לתא, לנוסחה או לשם המפנה לתא, נוסחה או ערך"
			}
		]
	},
	{
		name: "ISEVEN",
		description: "החזרת ערך True אם המספר הוא זוגי.",
		arguments: [
			{
				name: "number",
				description: "הערך לבדיקה"
			}
		]
	},
	{
		name: "ISFORMULA",
		description: "בדיקה אם הפניה מתבצעת לתא המכיל נוסחה והחזרת TRUE או FALSE.",
		arguments: [
			{
				name: "reference",
				description: "הוא הפניה לתא שברצונך לבדוק. ההפניה יכולה להיות הפניה לתא, נוסחה או שם המפנה לתא"
			}
		]
	},
	{
		name: "ISLOGICAL",
		description: "בודק האם הערך הוא ערך לוגי (TRUE או FALSE), ומחזיר TRUE או FALSE.",
		arguments: [
			{
				name: "value",
				description: "הערך שברצונך לבדוק. Value יכול להפנות לתא, נוסחה או לשם המפנה לתא, לנוסחה או לערך"
			}
		]
	},
	{
		name: "ISNA",
		description: "בודק האם ערך הוא ‎#N/A, ומחזיר TRUE או FALSE.",
		arguments: [
			{
				name: "value",
				description: "הוא ערך שברצונך לבחון. באפשרות הערך להפנות לתא, לנוסחה או לשם המפנה לתא, לנוסחה או לערך"
			}
		]
	},
	{
		name: "ISNONTEXT",
		description: "בודק האם הערך אינו טקסט (תאים ריקים אינם טקסט), ומחזיר TRUE או FALSE.",
		arguments: [
			{
				name: "value",
				description: "הערך שברצונך לבדוק: תא, נוסחה או שם המפנה לתא, נוסחה או ערך"
			}
		]
	},
	{
		name: "ISNUMBER",
		description: "בודק האם הערך הוא מספר, ומחזיר  TRUE או FALSE.",
		arguments: [
			{
				name: "value",
				description: "הוא הערך שברצונך לבחון. באפשרות הערך להפנות לתא, לנוסחה או לשם המפנה לתא, לנוסחה או לערך"
			}
		]
	},
	{
		name: "ISO.CEILING",
		description: "עיגול מספר כלפי מעלה, למספר השלם הקרוב ביותר או לכפולה הקרובה ביותר של significance.",
		arguments: [
			{
				name: "number",
				description: "הערך שברצונך לעגל"
			},
			{
				name: "significance",
				description: "הכפולה אליה ברצונך לעגל"
			}
		]
	},
	{
		name: "ISODD",
		description: "החזרת ערך True אם המספר הוא אי-זוגי.",
		arguments: [
			{
				name: "number",
				description: "הערך לבדיקה"
			}
		]
	},
	{
		name: "ISOWEEKNUM",
		description: "החזרת המספר של מספר שבוע ISO של השנה עבור תאריך נתון.",
		arguments: [
			{
				name: "date",
				description: "הוא קוד התאריך-שעה המשמש את Spreadsheet לחישוב תאריך ושעה"
			}
		]
	},
	{
		name: "ISPMT",
		description: "מחזיר את הריבית המשולמת במהלך תקופה מסוימת של השקעה.",
		arguments: [
			{
				name: "rate",
				description: " ריבית השקעה לתקופה. לדוגמה, השתמש ב- 6%/4 לתשלומים רבעוניים ב- 6% APR"
			},
			{
				name: "per",
				description: "תקופה שבה תרצה למצוא את הריבית"
			},
			{
				name: "nper",
				description: "מספר התשלומים הוא ההשקעה"
			},
			{
				name: "pv",
				description: "הסכום הכולל שסדרת תשלומים עתידיים שווה ברגע זה"
			}
		]
	},
	{
		name: "ISREF",
		description: "בודק אם ערך הוא הפניה, ומחזיר TRUE או FALSE.",
		arguments: [
			{
				name: "value",
				description: "הערך שברצונך לבדוק. Value יכול להפנות לתא, לנוסחה או לשם המפנה לתא, נוסחה או ערך"
			}
		]
	},
	{
		name: "ISTEXT",
		description: "בודק האם הערך הוא טקסט, ומחזיר  TRUE או FALSE.",
		arguments: [
			{
				name: "value",
				description: "הוא הערך שברצונך לבחון. באפשרות הערך להפנות לתא, לנוסחה או לשם המפנה לתא, לנוסחה או לערך"
			}
		]
	},
	{
		name: "KURT",
		description: "החזרת ה- kurtosis של קבוצת נתונים .",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "1 עד 255 מספרים או שמות, הפניות או מערכים המכילים מספרים, עבורם ברצונך לחשב את ה- kurtosis"
			},
			{
				name: "number2",
				description: "1 עד 255 מספרים או שמות, הפניות או מערכים המכילים מספרים, עבורם ברצונך לחשב את ה- kurtosis"
			}
		]
	},
	{
		name: "LARGE",
		description: "החזרת הערך ה- k הגדול ביותר בקבוצת נתונים. לדוגמה, המספר החמישי הגדול ביותר.",
		arguments: [
			{
				name: "array",
				description: "מערך הנתונים או טווח הנתונים בו ברצונך לקבוע מהו הערך ה-k בגודלו"
			},
			{
				name: "k",
				description: "המיקום (החל מהגדול ביותר) של הערך שיש להחזיר במערך או בטווח התאים"
			}
		]
	},
	{
		name: "LCM",
		description: "החזרת הכפולה הקטנה ביותר.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "1 עד 255 ערכים שעבורם ברצונך לחשב את הכפולה הקטנה ביותר"
			},
			{
				name: "number2",
				description: "1 עד 255 ערכים שעבורם ברצונך לחשב את הכפולה הקטנה ביותר"
			}
		]
	},
	{
		name: "LEFT",
		description: "החזרת מספר התווים המצוין מתחילת מחרוזת הטקסט.",
		arguments: [
			{
				name: "text",
				description: "מחרוזת הטקסט המכילה את התווים שברצונך לחלץ"
			},
			{
				name: "num_chars",
				description: "מספר התווים שברצונך שהפונקציה LEFT תחלץ, 1 אם מושמט"
			}
		]
	},
	{
		name: "LEN",
		description: "החזרת מספר התווים במחרוזת טקסט.",
		arguments: [
			{
				name: "text",
				description: "הטקסט שברצונך למצוא את אורכו. רווחים נמנים כתווים"
			}
		]
	},
	{
		name: "LINEST",
		description: "החזרת סטטיסטיקה המתארת מגמה ליניארית המתאימה לנקודות נתונים ידועות, על-ידי התאמת קו ישר המחושב באמצעות שיטת הריבועים הפחותים.",
		arguments: [
			{
				name: "known_y's",
				description: "סדרת ערכי y הידועים לך בקשר המתואר על-ידי y = mx + b"
			},
			{
				name: "known_x's",
				description: "סידרה אופציונלית של ערכי x שיתכן כי ידועים לך בקשר המתואר על-ידי y = mx + b"
			},
			{
				name: "const",
				description: "ערך לוגי: הקבוע b מחושב באופן רגיל אם Const = TRUE או מושמט; b מוגדר כשווה ל-0 אם Const = FALSE"
			},
			{
				name: "stats",
				description: "ערך לוגי: החזרת סטטיסטיקת רגרסיה נוספת = TRUE; החזרת מקדמי m והקבוע b = FALSE או מושמט"
			}
		]
	},
	{
		name: "LN",
		description: "החזרת הלוגריתם הטבעי של מספר.",
		arguments: [
			{
				name: "number",
				description: "המספר הממשי החיובי עבורו ברצונך לחשב את הלוגריתם הטבעי"
			}
		]
	},
	{
		name: "LOG",
		description: "החזרת הלוגריתם של מספר לפי הבסיס שציינת.",
		arguments: [
			{
				name: "number",
				description: "המספר הממשי החיובי עבורו ברצונך לחשב את הלוגריתם"
			},
			{
				name: "base",
				description: "בסיס הלוגריתם, 10 אם מושמט"
			}
		]
	},
	{
		name: "LOG10",
		description: "החזרת הלוגריתם של מספר לפי בסיס 10.",
		arguments: [
			{
				name: "number",
				description: "המספר הממשי החיובי עבורו ברצונך לחשב את הלוגריתם לפי בסיס 10"
			}
		]
	},
	{
		name: "LOGEST",
		description: "החזרת סטטיסטיקה המתארת עקומה מעריכית המתאימה לנקודות נתונים ידועות.",
		arguments: [
			{
				name: "known_y's",
				description: "סדרת ערכי y הידועים לך בקשר המתואר על-ידי y = b*m^x"
			},
			{
				name: "known_x's",
				description: "סידרה אופציונלית של ערכי x שיתכן כי ידועים לך בקשר המתואר על-ידי y = b*m^x"
			},
			{
				name: "const",
				description: "ערך לוגי: הקבוע b מחושב באופן רגיל אם Const = TRUE או מושמט; b מוגדר שווה 1 אם Const = FALSE"
			},
			{
				name: "stats",
				description: "ערך לוגי: החזרת סטטיסטיקת רגרסיה נוספת = TRUE; החזרת מקדמי m והקבוע b = FALSE או מושמט"
			}
		]
	},
	{
		name: "LOGINV",
		description: "הפונקציה מחזירה את ההופכי של פונקציית ההתפלגות הלוג-נורמלית המצטברת של x, כאשר‎ ln(x)‎ מתפלג באופן נורמלי עם הפרמטרים Mean ו-Standard_dev.",
		arguments: [
			{
				name: "probability",
				description: "הסתברות המשויכת להתפלגות הלוג-נורמלית, מספר בין 0 ל- 1, כולל"
			},
			{
				name: "mean",
				description: "הממוצע של ln(x)‎"
			},
			{
				name: "standard_dev",
				description: "סטיית התקן של ln(x)‎, מספר חיובי"
			}
		]
	},
	{
		name: "LOGNORM.DIST",
		description: "הפונקציה מחזירה את ההתפלגות הלוג-נורמלית של x, כאשר ln(x)‎ מתפלג באופן נורמלי עם הפרמטרים Mean ו- Standard_dev.",
		arguments: [
			{
				name: "x",
				description: "הוא הערך שלפיו יש להעריך את הפונקציה, מספר חיובי"
			},
			{
				name: "mean",
				description: "הוא הממוצע של ln(x)‎"
			},
			{
				name: "standard_dev",
				description: "הוא סטיית התקן של ln(x)‎, מספר חיובי"
			},
			{
				name: "cumulative",
				description: "הוא ערך לוגי: עבור פונקציית ההתפלגות המצטברת, השתמש ב- TRUE; עבור פונקציית צפיפות ההסתברות, השתמש ב- FALSE"
			}
		]
	},
	{
		name: "LOGNORM.INV",
		description: "הפונקציה מחזירה את ההופכי של פונקציית התפלגות מצטברת לוג-נורמלית של x, כאשר ln(x)‎ מתפלג באופן נורמלי עם הפרמטרים Mean ו- Standard_dev.",
		arguments: [
			{
				name: "probability",
				description: "הוא ההסתברות המשויכת להתפלגות הלוג-נורמלית, מספר בין 0 ל- 1, כולל"
			},
			{
				name: "mean",
				description: "הוא הממוצע של ln(x)‎"
			},
			{
				name: "standard_dev",
				description: "הוא סטיית התקן של ln(x)‎, מספר חיובי"
			}
		]
	},
	{
		name: "LOGNORMDIST",
		description: "הפונקציה מחזירה את ההתפלגות הלוג-נורמלית המצטברת של x, כאשר ln(x)‎ מתפלג באופן נורמלי עם הפרמטרים Mean ו- Standard_dev.",
		arguments: [
			{
				name: "x",
				description: "הערך שבו יש להעריך את הפונקציה, מספר חיובי"
			},
			{
				name: "mean",
				description: "הממוצע של ln(x)‎"
			},
			{
				name: "standard_dev",
				description: "סטיית התקן של ln(x)‎, מספר חיובי"
			}
		]
	},
	{
		name: "LOOKUP",
		description: "חיפוש הערך מטווח הכולל שורה בודדת או עמודה בודדת או ממערך. מסופק מתאימות לאחור.",
		arguments: [
			{
				name: "lookup_value",
				description: "ערך שהפונקציה LOOKUP מחפשת ב- Lookup_vector. יכול להיות מספר, טקסט, ערך לוגי או שם או הפניה לערך"
			},
			{
				name: "lookup_vector",
				description: "טווח המכיל רק שורה אחת או עמודה אחת של טקסט, מספרים או ערכים לוגיים, הממוקמים בסדר עולה"
			},
			{
				name: "result_vector",
				description: "טווח המכיל רק שורה אחת או עמודה אחת, בגודל זהה לזה של Lookup_vector"
			}
		]
	},
	{
		name: "LOWER",
		description: "המרת כל האותיות במחרוזת טקסט לאותיות קטנות.",
		arguments: [
			{
				name: "text",
				description: "הטקסט שברצונך להמיר לאותיות קטנות. תווים ב- Text שאינם אותיות לא ישתנו"
			}
		]
	},
	{
		name: "MATCH",
		description: "החזרת המיקום היחסי של פריט במערך המתאים לערך מסוים בסדר מסוים.",
		arguments: [
			{
				name: "lookup_value",
				description: "הערך בו יש להשתמש כדי למצוא את הערך הרצוי במערך. מספר, טקסט או ערך לוגי, או הפניה לאחד מאלה"
			},
			{
				name: "lookup_array",
				description: "הוא טווח רציף של תאים המכילים ערכים אפשריים לחיפוש, מערך של ערכים, או הפניה למערך"
			},
			{
				name: "match_type",
				description: "הוא המספר 1, 0 או ‎-1 המציין איזה ערך להחזיר."
			}
		]
	},
	{
		name: "MAX",
		description: "החזרת הערך הגדול ביותר בקבוצת ערכים. הפונקציה מתעלמת מערכים לוגיים וטקסט.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "1 עד 255 מספרים, תאים ריקים, ערכים לוגיים, או מספרי טקסט עבורם ברצונך לחשב את המקסימום"
			},
			{
				name: "number2",
				description: "1 עד 255 מספרים, תאים ריקים, ערכים לוגיים, או מספרי טקסט עבורם ברצונך לחשב את המקסימום"
			}
		]
	},
	{
		name: "MAXA",
		description: "החזרת הערך הגדול ביותר בקבוצת ערכים. הפונקציה אינה מתעלמת מערכים לוגיים וטקסט.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "1 עד 255 מספרים, תאים ריקים, ערכים לוגיים, או מספרי טקסט עבורם ברצונך למצוא את המקסימום"
			},
			{
				name: "value2",
				description: "1 עד 255 מספרים, תאים ריקים, ערכים לוגיים, או מספרי טקסט עבורם ברצונך למצוא את המקסימום"
			}
		]
	},
	{
		name: "MDETERM",
		description: "החזרת דטרמיננטת מטריצה של מערך.",
		arguments: [
			{
				name: "array",
				description: "מערך מספרי בעל מספר שווה של שורות ועמודות: טווח תאים או קבוע מערך"
			}
		]
	},
	{
		name: "MEDIAN",
		description: "החזרת החציון, או המספר באמצע קבוצת מספרים נתונים.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "1 עד 255 מספרים או שמות, מערכים או הפניות המכילים מספרים עבורם ברצונך לחשב את החציון"
			},
			{
				name: "number2",
				description: "1 עד 255 מספרים או שמות, מערכים או הפניות המכילים מספרים עבורם ברצונך לחשב את החציון"
			}
		]
	},
	{
		name: "MID",
		description: "החזרת התווים מאמצע מחרוזת טקסט, כאשר ניתנים נקודת ההתחלה והאורך.",
		arguments: [
			{
				name: "text",
				description: "מחרוזת הטקסט ממנה ברצונך לחלץ את התווים"
			},
			{
				name: "start_num",
				description: "מיקומו של התו הראשון שברצונך לחלץ. התו הראשון ב- Text הוא 1"
			},
			{
				name: "num_chars",
				description: "מספר התווים שיש להחזיר מ- Text"
			}
		]
	},
	{
		name: "MIN",
		description: "החזרת הערך הקטן ביותר בקבוצת ערכים. הפונקציה מתעלמת מערכים וטקסט.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "1 עד 255 מספרים, תאים ריקים, ערכים לוגיים, או מספרי טקסט עבורם ברצונך לחשב את המינימום"
			},
			{
				name: "number2",
				description: "1 עד 255 מספרים, תאים ריקים, ערכים לוגיים, או מספרי טקסט עבורם ברצונך לחשב את המינימום"
			}
		]
	},
	{
		name: "MINA",
		description: "החזרת הערך הקטן ביותר בקבוצת ערכים. הפונקציה אינה מתעלמת מערכים לוגיים וטקסט.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "1 עד 255 מספרים, תאים ריקים, ערכים לוגיים, או מספרי טקסט עבורם ברצונך למצוא את המינימום"
			},
			{
				name: "value2",
				description: "1 עד 255 מספרים, תאים ריקים, ערכים לוגיים, או מספרי טקסט עבורם ברצונך למצוא את המינימום"
			}
		]
	},
	{
		name: "MINUTE",
		description: "החזרת הדקה, מספר בין 0 ל- 59.",
		arguments: [
			{
				name: "serial_number",
				description: "הוא מספר בקוד תאריך-שעה המשמש ב- Spreadsheet או טקסט בתבנית שעה, כגון 16:48:00 או ‎4:48:00 PM"
			}
		]
	},
	{
		name: "MINVERSE",
		description: "החזרת המטריצה ההופכית של המטריצה המאוחסנת במערך.",
		arguments: [
			{
				name: "array",
				description: "מערך מספרי בעל מספר שווה של שורות ועמודות: טווח תאים או קבוע מערך"
			}
		]
	},
	{
		name: "MIRR",
		description: "החזרת שיעור התשואה הפנימי עבור סידרה של תזרימי מזומנים תקופתיים. הפונקציה לוקחת בחשבון הן את עלות ההשקעה והן את הריבית על השקעה חוזרת של מזומנים.",
		arguments: [
			{
				name: "values",
				description: "מערך או הפניה לתאים המכילים מספרים המייצגים סדרת תשלומים (שליליים) והכנסות (חיוביים) בתקופות קבועות"
			},
			{
				name: "finance_rate",
				description: "שיעור הריבית שתשלם על הכסף המשמש בתזרימי המזומנים"
			},
			{
				name: "reinvest_rate",
				description: "שיעור הריבית שתקבל עבור תזרימי המזומנים בעת השקעתם מחדש"
			}
		]
	},
	{
		name: "MMULT",
		description: "החזרת מכפלת המטריצות של שני מערכים, מערך בעל אותו מספר שורות שב-Array1 ומספר העמודות שב-Array2‏.",
		arguments: [
			{
				name: "array1",
				description: "המערך הראשון של מספרים שיש לכפול, מספר העמודות שלו חייב להיות זהה למספר השורות שב- Array2"
			},
			{
				name: "array2",
				description: "המערך הראשון של מספרים שיש לכפול, מספר העמודות שלו חייב להיות זהה למספר השורות שב- Array2"
			}
		]
	},
	{
		name: "MOD",
		description: "החזרת השארית לאחר חילוק של מספר במחלק.",
		arguments: [
			{
				name: "number",
				description: "המספר עבורו ברצונך לחשב את השארית לאחר ביצוע החילוק"
			},
			{
				name: "divisor",
				description: "המספר בו ברצונך לחלק את Number"
			}
		]
	},
	{
		name: "MODE",
		description: "הפונקציה מחזירה את הערך השכיח ביותר, או החוזר מספר רב של פעמים, במערך נתונים או טווח נתונים.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "1 עד 255 מספרים או שמות, הפניות או מערכים המכילים מספרים, שעבורם ברצונך למצוא את הערך השכיח ביותר"
			},
			{
				name: "number2",
				description: "1 עד 255 מספרים או שמות, הפניות או מערכים המכילים מספרים, שעבורם ברצונך למצוא את הערך השכיח ביותר"
			}
		]
	},
	{
		name: "MODE.MULT",
		description: "הפונקציה מחזירה מערך אנכי של הערכים השכיחים או החוזרים ביותר במערך או בטווח של נתונים. לקבלת מערך אופקי, השתמש ב- ‎=TRANSPOSE(MODE.MULT(number1,number2,...))‎.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "הם 1 עד 255 מספרים או שמות, מערכים או הפניות המכילות מספרים שעבורם ברצונך לקבל את הערך השכיח"
			},
			{
				name: "number2",
				description: "הם 1 עד 255 מספרים או שמות, מערכים או הפניות המכילות מספרים שעבורם ברצונך לקבל את הערך השכיח"
			}
		]
	},
	{
		name: "MODE.SNGL",
		description: "הפונקציה מחזירה את הערך השכיח או החוזר ביותר במערך או טווח של נתונים.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "הם 1 עד 255 מספרים או שמות, מערכים או הפניות המכילות מספרים שעבורם ברצונך לקבל את הערך השכיח"
			},
			{
				name: "number2",
				description: "הם 1 עד 255 מספרים או שמות, מערכים או הפניות המכילות מספרים שעבורם ברצונך לקבל את הערך השכיח"
			}
		]
	},
	{
		name: "MONTH",
		description: "החזרת החודש, מספר מ-1 (ינואר) עד 12 (דצמבר).",
		arguments: [
			{
				name: "serial_number",
				description: "הוא מספר בקוד תאריך-שעה המשמש ב- Spreadsheet"
			}
		]
	},
	{
		name: "MROUND",
		description: "החזרת מספר מעוגל לכפולה הרצויה.",
		arguments: [
			{
				name: "number",
				description: "הערך לעיגול"
			},
			{
				name: "multiple",
				description: "הכפולה שאליה ברצונך לעגל את המספר"
			}
		]
	},
	{
		name: "MULTINOMIAL",
		description: "החזרת איבר מתוך ערכת מספרים.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "1 עד 255 ערכים עבורם הינך רוצה את האיבר"
			},
			{
				name: "number2",
				description: "1 עד 255 ערכים עבורם הינך רוצה את האיבר"
			}
		]
	},
	{
		name: "MUNIT",
		description: "החזרת מטריצת היחידה עבור הממד שצוין.",
		arguments: [
			{
				name: "dimension",
				description: "הוא מספר שלם המציין את הממד של מטריצת היחידה שברצונך להחזיר"
			}
		]
	},
	{
		name: "N",
		description: "ממיר ערך לא מספרי למספר, תאריכים או מספרים סידוריים,  TRUE ל-1, כל דבר אחר ל-0 (אפס).",
		arguments: [
			{
				name: "value",
				description: "הוא הערך שברצונך להמיר"
			}
		]
	},
	{
		name: "NA",
		description: "החזרת ערך השגיאה ‎#N/A‎ (ערך לא זמין).",
		arguments: [
		]
	},
	{
		name: "NEGBINOM.DIST",
		description: "הפונקציה מחזירה את ההתפלגות הבינומית השלילית, ההסתברות שיהיו Number_f כשלונות לפני ההצלחה ה- Number_s, עם הסתברות Probability_s להצלחה.",
		arguments: [
			{
				name: "number_f",
				description: "הוא מספר הכשלונות"
			},
			{
				name: "number_s",
				description: "הוא מספר הסף של ההצלחות"
			},
			{
				name: "probability_s",
				description: "הוא ההסתברות להצלחה; מספר בין 0 ל- 1"
			},
			{
				name: "cumulative",
				description: "הוא ערך לוגי: עבור פונקציית ההתפלגות המצטברת, השתמש ב- TRUE; עבור פונקציית מסת ההסתברות, השתמש ב- FALSE"
			}
		]
	},
	{
		name: "NEGBINOMDIST",
		description: "הפונקציה מחזירה את ההתפלגות הבינומית השלילית, ההסתברות שיהיו Number_f כישלונות לפני Number-s הצלחות, עם הסתברות Probability_s להצלחה.",
		arguments: [
			{
				name: "number_f",
				description: "מספר הכישלונות"
			},
			{
				name: "number_s",
				description: "מספר הסף של ההצלחות"
			},
			{
				name: "probability_s",
				description: "ההסתברות להצלחה; מספר בין 0 ל- 1"
			}
		]
	},
	{
		name: "NETWORKDAYS",
		description: "החזרת מספר ימי העבודה המלאים בין שני תאריכים.",
		arguments: [
			{
				name: "start_date",
				description: "מספר תאריך סידורי המייצג את תאריך ההתחלה"
			},
			{
				name: "end_date",
				description: "מספר תאריך סידורי המייצג את תאריך הסיום"
			},
			{
				name: "holidays",
				description: "מערך אופציונלי של מספר תאריך סידורי אחד או יותר שאינו נכלל בלוח השנה של העבודה, כגון ימי חג לאומיים וימי חופש לא קבועים"
			}
		]
	},
	{
		name: "NETWORKDAYS.INTL",
		description: "הפונקציה מחזירה את מספר ימי העבודה המלאים בין שני תאריכים עם פרמטרים מותאמים אישית של סוף שבוע.",
		arguments: [
			{
				name: "start_date",
				description: "הוא מספר תאריך סידורי המייצג את תאריך ההתחלה"
			},
			{
				name: "end_date",
				description: "הוא מספר תאריך סידורי המייצג את תאריך הסיום"
			},
			{
				name: "weekend",
				description: "הוא מספר או מחרוזת המציינים מתי חלים סופי שבוע"
			},
			{
				name: "holidays",
				description: "הוא ערכה אופציונלית של מספר תאריך סידורי אחד או יותר שאין לכלול מלוח השנה של העבודה, כגון חגים ממלכתיים וימי חופש לא קבועים"
			}
		]
	},
	{
		name: "NOMINAL",
		description: "החזרת שיעור הריבית הנומינלית השנתית.",
		arguments: [
			{
				name: "effect_rate",
				description: "שיעור הריבית האפקטיבית"
			},
			{
				name: "npery",
				description: "מספר תקופות צבירה בשנה"
			}
		]
	},
	{
		name: "NORM.DIST",
		description: "הפונקציה מחזירה את ההתפלגות הנורמלית של ממוצע וסטיית תקן שצוינו.",
		arguments: [
			{
				name: "x",
				description: "הוא הערך שעבורו ברצונך לקבל את ההתפלגות"
			},
			{
				name: "mean",
				description: "הוא הממוצע החשבוני של ההתפלגות"
			},
			{
				name: "standard_dev",
				description: "הוא סטיית התקן של ההתפלגות, מספר חיובי"
			},
			{
				name: "cumulative",
				description: "הוא ערך לוגי: עבור פונקציית ההתפלגות המצטברת, השתמש ב- TRUE; עבור פונקציית צפיפות ההסתברות, השתמש ב- FALSE"
			}
		]
	},
	{
		name: "NORM.INV",
		description: "הפונקציה מחזירה את ההופכי של ההתפלגות המצטברת הנורמלית עבור הממוצע וסטיית התקן שצוינו.",
		arguments: [
			{
				name: "probability",
				description: "הוא ההסתברות התואמת להתפלגות הנורמלית, מספר בין 0 ל- 1, כולל"
			},
			{
				name: "mean",
				description: "הוא הממוצע החשבוני של ההתפלגות"
			},
			{
				name: "standard_dev",
				description: "הוא סטיית התקן של ההתפלגות, מספר חיובי"
			}
		]
	},
	{
		name: "NORM.S.DIST",
		description: "הפונקציה מחזירה את ההתפלגות הנורמלית הסטנדרטית (בעלת ממוצע אפס וסטיית תקן אחת).",
		arguments: [
			{
				name: "z",
				description: "הוא הערך שעבורו ברצונך לקבל את ההתפלגות"
			},
			{
				name: "cumulative",
				description: "הוא ערך לוגי להחזרה עבור הפונקציה: פונקציית ההתפלגות המצטברת = TRUE; פונקציית צפיפות ההסתברות = FALSE"
			}
		]
	},
	{
		name: "NORM.S.INV",
		description: "הפונקציה מחזירה את ההופכי של ההתפלגות המצטברת הנורמלית הסטנדרטית (בעלת ממוצע אפס וסטיית תקן אחת).",
		arguments: [
			{
				name: "probability",
				description: "הוא ההסתברות התואמת להתפלגות הנורמלית, מספר בין 0 ל- 1, כולל"
			}
		]
	},
	{
		name: "NORMDIST",
		description: "הפונקציה מחזירה את ההתפלגות המצטברת הנורמלית עבור הממוצע וסטיית התקן שצוינו.",
		arguments: [
			{
				name: "x",
				description: "הערך שעבורו ברצונך לחשב התפלגות"
			},
			{
				name: "mean",
				description: "הממוצע החשבוני של ההתפלגות"
			},
			{
				name: "standard_dev",
				description: "סטיית התקן של ההתפלגות, מספר חיובי"
			},
			{
				name: "cumulative",
				description: "ערך לוגי: עבור פונקציית ההתפלגות המצטברת, השתמש ב- TRUE; עבור פונקציית צפיפות ההסתברות, השתמש ב- FALSE"
			}
		]
	},
	{
		name: "NORMINV",
		description: "הפונקציה מחזירה את ההופכי של ההתפלגות המצטברת הנורמלית עבור הממוצע וסטיית התקן שצוינו.",
		arguments: [
			{
				name: "probability",
				description: "הסתברות התואמת להתפלגות הנורמלית, מספר בין 0 ל- 1, כולל"
			},
			{
				name: "mean",
				description: "הממוצע החשבוני של ההתפלגות"
			},
			{
				name: "standard_dev",
				description: "סטיית התקן של ההתפלגות, מספר חיובי"
			}
		]
	},
	{
		name: "NORMSDIST",
		description: "הפונקציה מחזירה את ההתפלגות המצטברת הנורמלית הסטנדרטית (בעלת ממוצע 0 וסטיית תקן 1).",
		arguments: [
			{
				name: "z",
				description: "הערך שעבורו ברצונך לחשב את ההתפלגות"
			}
		]
	},
	{
		name: "NORMSINV",
		description: "הפונקציה מחזירה את ההופכי של ההתפלגות המצטברת הנורמלית הסטנדרטית (בעלת ממוצע 0 וסטיית תקן 1).",
		arguments: [
			{
				name: "probability",
				description: "הסתברות התואמת להתפלגות הנורמלית, מספר בין 0 ל- 1, כולל"
			}
		]
	},
	{
		name: "NOT",
		description: "משנה FALSE ל-TRUE, או TRUE ל-FALSE.",
		arguments: [
			{
				name: "logical",
				description: "הוא ערך או ביטוי שאפשר להעריך אותו כ-TRUE או FALSE"
			}
		]
	},
	{
		name: "NOW",
		description: "החזרת התאריך והזמן הנוכחיים המעוצבים כתאריך ושעה.",
		arguments: [
		]
	},
	{
		name: "NPER",
		description: "החזרת מספר התקופות עבור השקעה בהתבסס על תשלומים תקופתיים קבועים ושיעור ריבית קבוע.",
		arguments: [
			{
				name: "rate",
				description: "שיעור הריבית לתקופה"
			},
			{
				name: "pmt",
				description: "התשלום המבוצע בכל תקופה, אינו יכול להשתנות לאורך חיי הקצבה השנתית"
			},
			{
				name: "pv",
				description: "הערך הנוכחי, או הסכום הכולל, שהוא שוויה העכשווי של סדרת תשלומים עתידיים"
			},
			{
				name: "fv",
				description: "הערך העתידי, או יתרת מזומנים שברצונך להשיג לאחר ביצוע התשלום האחרון. אם יושמט, מניחים כי הוא 0"
			},
			{
				name: "type",
				description: "ערך לוגי: תשלום בתחילת התקופה = 1; תשלום בסיום התקופה = 0 או מושמט"
			}
		]
	},
	{
		name: "NPV",
		description: "החזרת ערך הנטו הנוכחי של השקעה המבוסס על שיעור הניכיון וסדרת תשלומים עתידיים (ערכים שליליים) והכנסות (ערכים חיוביים).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "rate",
				description: "שיעור הניכיון במשך תקופה אחת"
			},
			{
				name: "value1",
				description: "1 עד 254 תשלומים והכנסות, במרווחי זמן שווים והמופיעים בסיום כל תקופה"
			},
			{
				name: "value2",
				description: "1 עד 254 תשלומים והכנסות, במרווחי זמן שווים והמופיעים בסיום כל תקופה"
			}
		]
	},
	{
		name: "NUMBERVALUE",
		description: "המרת טקסט למספר בצורה שאינה תלויה באזור.",
		arguments: [
			{
				name: "text",
				description: "הוא המחרוזת המייצגת את המספר שברצונך להמיר"
			},
			{
				name: "decimal_separator",
				description: "הוא התו המשמש כמפריד העשרוני במחרוזת"
			},
			{
				name: "group_separator",
				description: "הוא התו המשמש כמפריד הקבוצה במחרוזת"
			}
		]
	},
	{
		name: "OCT2BIN",
		description: "המרת מספר אוקטלי לבינארי.",
		arguments: [
			{
				name: "number",
				description: "המספר האוקטלי שברצונך להמיר"
			},
			{
				name: "places",
				description: "מספר התווים בהם יש להשתמש"
			}
		]
	},
	{
		name: "OCT2DEC",
		description: "המרת מספר אוקטלי לעשרוני.",
		arguments: [
			{
				name: "number",
				description: "המספר האוקטלי שברצונך להמיר"
			}
		]
	},
	{
		name: "OCT2HEX",
		description: "המרת מספר אוקטלי להקסדצימאלי.",
		arguments: [
			{
				name: "number",
				description: "המספר האוקטלי שברצונך להמיר"
			},
			{
				name: "places",
				description: "מספר התווים בהם יש להשתמש"
			}
		]
	},
	{
		name: "ODD",
		description: "עיגול מספר חיובי כלפי מעלה ומספר שלילי כלפי מטה, למספר השלם האי-זוגי הקרוב ביותר.",
		arguments: [
			{
				name: "number",
				description: "הערך שיש לעגל"
			}
		]
	},
	{
		name: "OFFSET",
		description: "החזרת הפניה לטווח שהוא מספר נתון של שורות ועמודות, מהפניה נתונה.",
		arguments: [
			{
				name: "reference",
				description: "הפניה ממנה ברצונך לבסס את ההיסט, הפניה לתא או טווח של תאים סמוכים"
			},
			{
				name: "rows",
				description: "מספר השורות, מעלה או מטה, שברצונך שהתא הימני-העליון של התוצאה יפנה אליו"
			},
			{
				name: "cols",
				description: "מספר העמודות, לימין או לשמאל, שברצונך שהתא הימני-העליון של התוצאה יפנה אליו"
			},
			{
				name: "height",
				description: "הגובה, במספר שורות, שבו ברצונך שהתוצאה תהיה, גובה זהה לגובה של Reference אם מושמט"
			},
			{
				name: "width",
				description: "הרוחב, במספר עמודות, שבו ברצונך שהתוצאה תהיה, רוחב זהה לרוחב של Reference אם מושמט"
			}
		]
	},
	{
		name: "OR",
		description: "בדיקה אם ארגומנטים הם TRUE, והחזרת TRUE או FALSE. מחזיר FALSE רק אם כל הארגומנטים הם FALSE.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logical1",
				description: "הם 1 עד 255 מצבים שברצונך לבחון ואשר יכולים להיות או TRUE או FALSE"
			},
			{
				name: "logical2",
				description: "הם 1 עד 255 מצבים שברצונך לבחון ואשר יכולים להיות או TRUE או FALSE"
			}
		]
	},
	{
		name: "PARAMETER",
		description: "",
		arguments: [
			{
				name: "",
				description: ""
			}
		]
	},
	{
		name: "PDURATION",
		description: "החזרת מספר התקופות הדרושות להשקעה כדי להגיע לערך שצוין.",
		arguments: [
			{
				name: "rate",
				description: "הוא שער הריבית לכל תקופה."
			},
			{
				name: "pv",
				description: "הוא הערך הנוכחי של ההשקעה"
			},
			{
				name: "fv",
				description: "הוא הערך העתידי הרצוי של ההשקעה"
			}
		]
	},
	{
		name: "PEARSON",
		description: "החזרת מקדם המתאם של מומנט המכפלה לפי פירסון, r.",
		arguments: [
			{
				name: "array1",
				description: "קבוצת ערכים בלתי-תלויים"
			},
			{
				name: "array2",
				description: "קבוצת ערכים תלויים"
			}
		]
	},
	{
		name: "PERCENTILE",
		description: "הפונקציה מחזירה את אחוזון ה- k של ערכים בטווח.",
		arguments: [
			{
				name: "array",
				description: "מערך הנתונים או טווח הנתונים המגדיר מעמד יחסי"
			},
			{
				name: "k",
				description: "ערך האחוזון בין 0 ל- 1 ועד בכלל"
			}
		]
	},
	{
		name: "PERCENTILE.EXC",
		description: "הפונקציה מחזירה את האחוזון ה- k של ערכים בטווח, כאשר k נמצא בטווח 0..1, לא כולל.",
		arguments: [
			{
				name: "array",
				description: "הוא המערך או הטווח של הנתונים המגדיר מעמד יחסי"
			},
			{
				name: "k",
				description: "הוא ערך האחוזון בין 0 עד 1, כולל"
			}
		]
	},
	{
		name: "PERCENTILE.INC",
		description: "הפונקציה מחזירה את אחוזון ה- k של ערכים בטווח, כאשר k נמצא בטווח 0..1, כולל.",
		arguments: [
			{
				name: "array",
				description: "הוא המערך או הטווח של הנתונים שמגדיר מעמד יחסי"
			},
			{
				name: "k",
				description: "הוא ערך האחוזון בין 0 עד 1, כולל"
			}
		]
	},
	{
		name: "PERCENTRANK",
		description: "הפונקציה מחזירה את הדרגה של ערך בקבוצת נתונים כאחוז של קבוצת הנתונים.",
		arguments: [
			{
				name: "array",
				description: "מערך הנתונים או טווח הנתונים המכיל ערכים מספריים המגדיר מעמד יחסי"
			},
			{
				name: "x",
				description: "הערך שברצונך לדעת את הדירוג שלו"
			},
			{
				name: "significance",
				description: "ערך אופציונלי המזהה את מספר הספרות המובהקות עבור האחוז המוחזר, אם מושמט הפונקציה משתמשת בשלוש ספרות ‎(‎0.xxx%‎)"
			}
		]
	},
	{
		name: "PERCENTRANK.EXC",
		description: "הפונקציה מחזירה את הדרגה של ערך בערכת נתונים בתור אחוז מערכת הנתונים (0..1, לא כולל).",
		arguments: [
			{
				name: "array",
				description: "הוא המערך או הטווח של הנתונים עם ערכים מספריים המגדיר מעמד יחסי"
			},
			{
				name: "x",
				description: "הוא הערך שעבורו ברצונך לדעת את הדרגה"
			},
			{
				name: "significance",
				description: "הוא ערך אופציונלי המזהה את מספר הספרות המשמעותיות של האחוז המוחזר, הפונקציה משתמשת בשלוש ספרות אם מושמט (0.xxx%)"
			}
		]
	},
	{
		name: "PERCENTRANK.INC",
		description: "הפונקציה מחזירה את הדרגה של ערך בערכת נתונים בתור אחוז מערכת הנתונים (0..1, כולל).",
		arguments: [
			{
				name: "array",
				description: "הוא המערך או הטווח של הנתונים עם ערכים מספריים המגדיר מעמד יחסי"
			},
			{
				name: "x",
				description: "הוא הערך שעבורו ברצונך לדעת את הדרגה"
			},
			{
				name: "significance",
				description: "הוא ערך אופציונלי המזהה את מספר הספרות המשמעותיות של האחוז המוחזר, הפונקציה משתמשת בשלוש ספרות אם מושמט (0.xxx%)"
			}
		]
	},
	{
		name: "PERMUT",
		description: "החזרת מספר התמורות עבור מספר נתון של אובייקטים אותם ניתן לבחור מתוך כלל האובייקטים.",
		arguments: [
			{
				name: "number",
				description: "מספרם הכולל של האובייקטים"
			},
			{
				name: "number_chosen",
				description: "מספר האובייקטים בכל תמורה"
			}
		]
	},
	{
		name: "PERMUTATIONA",
		description: "החזרת מספר התמורות עבור מספר נתון של אובייקטים (עם חזרות) שניתן לבחור מתוך כלל האובייקטים.",
		arguments: [
			{
				name: "number",
				description: "הוא מספר האובייקטים הכולל"
			},
			{
				name: "number_chosen",
				description: "הוא מספר האובייקטים בכל תמורה"
			}
		]
	},
	{
		name: "PHI",
		description: "החזרת הערך של פונקציית צפיפות עבור התפלגות רגילה.",
		arguments: [
			{
				name: "x",
				description: "הוא המספר שעבורו אתה רוצה את הצפיפות של ההתפלגות הרגילה"
			}
		]
	},
	{
		name: "PI",
		description: "החזרת הערך של Pi‏, 3.14159265358979‏, בדיוק של 15 ספרות.",
		arguments: [
		]
	},
	{
		name: "PMT",
		description: "חישוב התשלום עבור הלוואה בהתבסס על תשלומים קבועים ושיעור ריבית קבוע.",
		arguments: [
			{
				name: "rate",
				description: "שיעור הריבית לתקופה עבור ההלוואה. לדוגמה, השתמש ב- 6%/4 לתשלומים רבעוניים ב-6% APR"
			},
			{
				name: "nper",
				description: "מספר התשלומים עבור ההלוואה"
			},
			{
				name: "pv",
				description: "הערך הנוכחי: הסכום הכולל שהוא שוויה העכשווי של סדרת תשלומים עתידיים"
			},
			{
				name: "fv",
				description: "הערך העתידי, או יתרת מזומנים שברצונך להשיג לאחר ביצוע התשלום האחרון. אם יושמט, מניחים כי הוא 0 (אפס)"
			},
			{
				name: "type",
				description: "ערך לוגי: תשלום בתחילת התקופה = 1; תשלום בסיום התקופה = 0 או מושמט"
			}
		]
	},
	{
		name: "POISSON",
		description: "הפונקציה מחזירה התפלגות פואסונית.",
		arguments: [
			{
				name: "x",
				description: "מספר האירועים"
			},
			{
				name: "mean",
				description: "הערך המספרי הצפוי, מספר חיובי"
			},
			{
				name: "cumulative",
				description: "ערך לוגי: עבור הסתברות פואסונית מצטברת, השתמש ב- TRUE; עבור פונקציית מסת ההסתברות הפואסונית, השתמש ב- FALSE"
			}
		]
	},
	{
		name: "POISSON.DIST",
		description: "הפונקציה מחזירה את התפלגות פואסון.",
		arguments: [
			{
				name: "x",
				description: "הוא מספר האירועים"
			},
			{
				name: "mean",
				description: "הוא הערך המספרי הצפוי, מספר חיובי"
			},
			{
				name: "cumulative",
				description: "הוא ערך לוגי: עבור הסתברות פואסון המצטברת, השתמש ב- TRUE; עבור פונקציית המסה של הסתברות פואסון, השתמש ב- FALSE"
			}
		]
	},
	{
		name: "POWER",
		description: "החזרת התוצאה של מספר המועלה בחזקה.",
		arguments: [
			{
				name: "number",
				description: "מספר הבסיס, כל מספר ממשי"
			},
			{
				name: "power",
				description: "המעריך, בו מועלה בחזקה מספר הבסיס"
			}
		]
	},
	{
		name: "PPMT",
		description: "החזרת התשלום על הקרן עבור השקעה נתונה בהתבסס על תשלומים תקופתיים קבועים ושיעור ריבית קבוע.",
		arguments: [
			{
				name: "rate",
				description: "שיעור הריבית לתקופה. לדוגמה, השתמש ב- 6%/4 לתשלומים רבעוניים ב-6% APR"
			},
			{
				name: "per",
				description: "ציון התקופה, חייב להיות בין 1 ל- nper"
			},
			{
				name: "nper",
				description: "המספר הכולל של תקופות תשלום בהשקעה"
			},
			{
				name: "pv",
				description: "הערך הנוכחי: הסכום הכולל שהוא שוויה העכשווי של סדרת תשלומים עתידיים"
			},
			{
				name: "fv",
				description: "הערך העתידי, או יתרת מזומנים שברצונך להשיג לאחר ביצוע התשלום האחרון"
			},
			{
				name: "type",
				description: "ערך לוגי: תשלום בתחילת התקופה = 1; תשלום בסיום התקופה = 0 או מושמט"
			}
		]
	},
	{
		name: "PRICEDISC",
		description: "החזרת המחיר לכל $100 ערך נקוב של נייר ערך שנוכה.",
		arguments: [
			{
				name: "settlement",
				description: "תאריך הסדרת החשבון של נייר הערך, המובע כמספר תאריך סידורי"
			},
			{
				name: "maturity",
				description: "תאריך הפירעון של נייר הערך, המובע כמספר תאריך סידורי"
			},
			{
				name: "discount",
				description: "שער הנכיון של נייר הערך"
			},
			{
				name: "redemption",
				description: "ערך הפדיון של נייר הערך לכל $100 ערך נקוב"
			},
			{
				name: "basis",
				description: "סוג בסיס ספירת הימים שבו יש להשתמש"
			}
		]
	},
	{
		name: "PROB",
		description: "החזרת ההסתברות שערכים בטווח נמצאים בין שני גבולות או שווים לגבול תחתון.",
		arguments: [
			{
				name: "x_range",
				description: "טווח הערכים המספריים של x להם משויכות הסתברויות"
			},
			{
				name: "prob_range",
				description: "קבוצת ההסתברויות המשויכות לערכים ב-X_range, ערכים בין 0 ל-1 לא כולל 0"
			},
			{
				name: "lower_limit",
				description: "הגבול התחתון של הערך, עבורו ברצונך לחשב את ההסתברות"
			},
			{
				name: "upper_limit",
				description: "הגבול העליון האופציונלי של הערך. אם יושמט, PROB תחזיר את ההסתברות שערכי X_range שווים ל- Lower_limit"
			}
		]
	},
	{
		name: "PRODUCT",
		description: "הכפלת כל המספרים הנתונים כארגומנטים .",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "1 עד 255 מספרים, ערכים לוגיים או מספרים המיוצגים כטקסט אותם ברצונך להכפיל"
			},
			{
				name: "number2",
				description: "1 עד 255 מספרים, ערכים לוגיים או מספרים המיוצגים כטקסט אותם ברצונך להכפיל"
			}
		]
	},
	{
		name: "PROPER",
		description: "המר מחרוזת טקסט לסוג האות המתאים; האות הראשונה בכל מילה ברישית,  וכל האותיות האחרות לאותיות קטנות.",
		arguments: [
			{
				name: "text",
				description: "טקסט בין מרכאות, נוסחה המחזירה טקסט או הפניה לתא המכיל טקסט בו יש להפוך חלק מהאותיות לרישיות"
			}
		]
	},
	{
		name: "PV",
		description: "החזרת הערך הנוכחי של השקעה: הסכום הכולל שהוא שוויה העכשווי של סדרת תשלומים עתידיים.",
		arguments: [
			{
				name: "rate",
				description: "שיעור הריבית לתקופה. לדוגמה, השתמש ב- 6%/4 לתשלומים רבעוניים ב- 6% APR "
			},
			{
				name: "nper",
				description: "המספר הכולל של תקופות תשלום בהשקעה"
			},
			{
				name: "pmt",
				description: "התשלום המבוצע בכל תקופה, אינו יכול להשתנות לאורך חיי ההשקעה"
			},
			{
				name: "fv",
				description: "הערך העתידי, או יתרת מזומנים שברצונך להשיג לאחר ביצוע התשלום האחרון"
			},
			{
				name: "type",
				description: "ערך לוגי: תשלום בתחילת התקופה = 1; תשלום בסיום התקופה = 0 או מושמט"
			}
		]
	},
	{
		name: "QUARTILE",
		description: "הפונקציה מחזירה את הרביעון של קבוצת נתונים.",
		arguments: [
			{
				name: "array",
				description: "המערך או טווח התאים של ערכים מספריים שעבורם ברצונך לחשב את ערך הרביעון"
			},
			{
				name: "quart",
				description: "מספר: ערך מזערי = 0; רביעון ראשון = 1; ערך חציון = 2; רביעון שלישי = 3; ערך מרבי = 4"
			}
		]
	},
	{
		name: "QUARTILE.EXC",
		description: "הפונקציה מחזירה את הרביעון של ערכת נתונים, בהתבסס על ערכי אחוזון מ- 0..1, לא כולל.",
		arguments: [
			{
				name: "array",
				description: "הוא המערך או טווח התאים של ערכים מספריים שעבורוברצונך לקבל את ערך הרביעון"
			},
			{
				name: "quart",
				description: "הוא מספר: ערך מינימום = 0; רביעון ראשון = 1; ערך חציון = 2; רביעון שלישי = 3; ערך מקסימום = 4"
			}
		]
	},
	{
		name: "QUARTILE.INC",
		description: "הפונקציה מחזירה את הרביעון של ערכת נתונים, בהתבסס על ערכי אחוזון מ- 0..1, כולל.",
		arguments: [
			{
				name: "array",
				description: "הוא המערך או טווח התאים של ערכים מספריים שעבורוברצונך לקבל את ערך הרביעון"
			},
			{
				name: "quart",
				description: "הוא מספר: ערך מינימום = 0; רביעון ראשון = 1; ערך חציון = 2; רביעון שלישי = 3; ערך מקסימום = 4"
			}
		]
	},
	{
		name: "QUOTIENT",
		description: "החזרת החלק השלם של פעולת חילוק.",
		arguments: [
			{
				name: "numerator",
				description: "המחולק"
			},
			{
				name: "denominator",
				description: "המחלק"
			}
		]
	},
	{
		name: "RADIANS",
		description: "המרת מעלות לרדיאנים.",
		arguments: [
			{
				name: "angle",
				description: "הזווית במעלות שברצונך להמיר"
			}
		]
	},
	{
		name: "RAND",
		description: "החזרת מספר אקראי הגדול או שווה ל-0 וקטן מ-1 , התפלגות אחידה (משתנה בעת חישוב מחדש).",
		arguments: [
		]
	},
	{
		name: "RANDBETWEEN",
		description: "החזרת מספר אקראי בין המספרים שציינת.",
		arguments: [
			{
				name: "bottom",
				description: "המספר השלם הקטן ביותר ש- RANDBETWEEN יחזיר"
			},
			{
				name: "top",
				description: "המספר השלם הגדול ביותר ש- RANDBETWEEN יחזיר"
			}
		]
	},
	{
		name: "RANGE",
		description: "",
		arguments: [
			{
				name: "",
				description: ""
			}
		]
	},
	{
		name: "RANK",
		description: "הפונקציה מחזירה דירוג של מספר ברשימת מספרים: גודלו ביחס לערכים אחרים ברשימה.",
		arguments: [
			{
				name: "number",
				description: "המספר שעבורו ברצונך לחפש את הדירוג"
			},
			{
				name: "ref",
				description: "מערך של רשימת מספרים או הפניה לרשימת מספרים. הפונקציה מתעלמת מערכים שאינם מספרים"
			},
			{
				name: "order",
				description: "מספר: דירוג ברשימה הממוינת בסדר יורד = 0 או מושמט; דירוג ברשימה הממוינת בסדר עולה = כל ערך שאינו אפס"
			}
		]
	},
	{
		name: "RANK.AVG",
		description: "הפונקציה מחזירה דרגת מספר ברשימת מספרים: גודלו ביחס לערכים אחרים ברשימה; אם יותר מערך אחד נמצא באותה דרגה, מוחזרת הדרגה הממוצעת.",
		arguments: [
			{
				name: "number",
				description: "הוא המספר שעבורו ברצונך למצוא דרגה"
			},
			{
				name: "ref",
				description: "הוא מערך של רשימת מספרים או הפניה אליה. המערכת מתעלמת מערכים לא מספריים"
			},
			{
				name: "order",
				description: "הוא מספר: דרגה ברשימה הממוינת בסדר יורד = 0 או מושמט; דרגה ברשימה הממוינת בסדר עולה = כל ערך שאינו אפס"
			}
		]
	},
	{
		name: "RANK.EQ",
		description: "הפונקציה מחזירה דרגת מספר ברשימת מספרים: גודלו ביחס לערכים אחרים ברשימה; אם יותר מערך אחד נמצא באותה דרגה, מוחזרת הדרגה העליונה של אותה ערכת ערכים.",
		arguments: [
			{
				name: "number",
				description: "הוא המספר שעבורו ברצונך למצוא דרגה"
			},
			{
				name: "ref",
				description: "הוא מערך של רשימת מספרים או הפניה אליה. המערכת מתעלמת מערכים לא מספריים"
			},
			{
				name: "order",
				description: "הוא מספר: דרגה ברשימה הממוינת בסדר יורד = 0 או מושמט; דרגה ברשימה הממוינת בסדר עולה = כל ערך שאינו אפס"
			}
		]
	},
	{
		name: "RATE",
		description: "החזרת שיעור הריבית לתקופה של הלוואה או קצבה שנתית.",
		arguments: [
			{
				name: "nper",
				description: "מספר תקופות התשלום עבור הלוואה או קיצבה שנתית"
			},
			{
				name: "pmt",
				description: "התשלום הניתן בכל תקופה ואשר לא ניתן לשנותו במהלך תקופת ההלוואה או הקיצבה"
			},
			{
				name: "pv",
				description: "הערך הנוכחי: הסכום הכולל שהוא שוויה העכשווי של סדרת תשלומים עתידיים"
			},
			{
				name: "fv",
				description: "הערך העתידי, או יתרת מזומנים שברצונך להשיג לאחר ביצוע התשלום האחרון. אם מושמט, fv=0"
			},
			{
				name: "type",
				description: "ערך לוגי: תשלום בתחילת התקופה = 1; תשלום בסוף התקופה = 0 או מושמט"
			},
			{
				name: "guess",
				description: "הערכתך מה יהיה שיעור הריבית; אם מושמט,  Guess=0.1  ‎ (עשרה אחוזים)"
			}
		]
	},
	{
		name: "RECEIVED",
		description: "החזרת הסכום המתקבל בעת הפירעון עבור נייר ערך שהושקע במלואו.",
		arguments: [
			{
				name: "settlement",
				description: "תאריך הסדרת החשבון של נייר הערך, המובע כמספר תאריך סידורי"
			},
			{
				name: "maturity",
				description: "תאריך הפירעון של נייר הערך, המובע כמספר תאריך סידורי"
			},
			{
				name: "investment",
				description: "הסכום שהושקע בנייר הערך"
			},
			{
				name: "discount",
				description: "שער הנכיון של נייר הערך"
			},
			{
				name: "basis",
				description: "סוג בסיס ספירת הימים שבו יש להשתמש"
			}
		]
	},
	{
		name: "REPLACE",
		description: "החלפת חלק של מחרוזת טקסט במחרוזת טקסט אחרת.",
		arguments: [
			{
				name: "old_text",
				description: "הטקסט שבו ברצונך להחליף מספר תווים"
			},
			{
				name: "start_num",
				description: "מיקום התו ב- Old_text שברצונך להחליף ב- New_text"
			},
			{
				name: "num_chars",
				description: "מספר התווים ב- Old_text שברצונך להחליף"
			},
			{
				name: "new_text",
				description: "הטקסט שיחליף תווים ב- Old_text"
			}
		]
	},
	{
		name: "REPT",
		description: "חזרה על טקסט מספר נתון של פעמים. השתמש ב- REPT כדי למלא תא במספר עותקים של מחרוזת טקסט.",
		arguments: [
			{
				name: "text",
				description: "הטקסט עליו ברצונך לחזור"
			},
			{
				name: "number_times",
				description: "מספר חיובי המציין את מספר הפעמים שיש לחזור על Text"
			}
		]
	},
	{
		name: "RIGHT",
		description: "החזרת מספר התווים המצוין מסוף מחרוזת הטקסט.",
		arguments: [
			{
				name: "text",
				description: "מחרוזת הטקסט המכילה את התווים שברצונך לחלץ"
			},
			{
				name: "num_chars",
				description: "מספר התווים שברצונך לחלץ, 1 אם מושמט"
			}
		]
	},
	{
		name: "ROMAN",
		description: "המרת מספר בספרות רגילות לספרות רומיות, כטקסט.",
		arguments: [
			{
				name: "number",
				description: "הוא המספר בספרות רגילות שאותו ברצונך להמיר"
			},
			{
				name: "form",
				description: "הוא המספר המציין את סוג הספרות הרומיות הרצויות."
			}
		]
	},
	{
		name: "ROUND",
		description: "עיגול מספר למספר ספרות שצוין.",
		arguments: [
			{
				name: "number",
				description: "המספר שברצונך לעגל"
			},
			{
				name: "num_digits",
				description: "מספר הספרות אליו ברצונך לעגל. מספר שלילי מעגל משמאל לנקודה העשרונית, אפס מעגל למספר השלם הקרוב"
			}
		]
	},
	{
		name: "ROUNDDOWN",
		description: "עיגול מספר מטה, כלפי אפס.",
		arguments: [
			{
				name: "number",
				description: "כל מספר ממשי שברצונך לעגל מטה"
			},
			{
				name: "num_digits",
				description: "מספר הספרות אליו ברצונך לעגל. מספר שלילי מעגל משמאל לנקודה העשרונית, אפס או השמטה מעגלים למספר השלם הקרוב"
			}
		]
	},
	{
		name: "ROUNDUP",
		description: "עיגול מספר מעלה, שלא כלפי אפס.",
		arguments: [
			{
				name: "number",
				description: "כל מספר ממשי שברצונך לעגל מעלה"
			},
			{
				name: "num_digits",
				description: "מספר הספרות אליו ברצונך לעגל. מספר שלילי מעגל משמאל לנקודה העשרונית, אפס או השמטה מעגלים למספר השלם הקרוב"
			}
		]
	},
	{
		name: "ROW",
		description: "החזרת מספר השורה של הפניה.",
		arguments: [
			{
				name: "reference",
				description: "התא או טווח יחיד של תאים עבורם ברצונך לקבל את מספר השורה. אם Reference מושמט, מוחזר התא המכיל את הפונקציה ROW"
			}
		]
	},
	{
		name: "ROWS",
		description: "החזרת מספר השורות בהפניה או מערך.",
		arguments: [
			{
				name: "array",
				description: "מערך, נוסחת מערך או הפניה לטווח תאים עבורם ברצונך לקבל את מספר השורות"
			}
		]
	},
	{
		name: "RRI",
		description: "החזרת שער ריבית שווה ערך עבור הגידול בהשקעה.",
		arguments: [
			{
				name: "nper",
				description: "הוא מספר התקופות של ההשקעה"
			},
			{
				name: "pv",
				description: "הוא הערך הנוכחי של ההשקעה"
			},
			{
				name: "fv",
				description: "הוא הערך העתידי של ההשקעה"
			}
		]
	},
	{
		name: "RSQ",
		description: "החזרת ריבוע מקדם המתאם של מומנט המכפלה לפי פירסון באמצעות נקודות הנתונים הנתונות.",
		arguments: [
			{
				name: "known_y's",
				description: "מערך או טווח של נקודות נתונים, יכולים להיות מספרים או שמות, מערכים או הפניות המכילות מספרים"
			},
			{
				name: "known_x's",
				description: "מערך או טווח של נקודות נתונים, יכולים להיות מספרים או שמות, מערכים או הפניות המכילות מספרים"
			}
		]
	},
	{
		name: "RTD",
		description: "החזרת נתוני זמן-אמת מתוכנית שתומכת באוטומציה של COM.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "progID",
				description: "הוא שם ה- ProgID של תוספת האוטומציה של COM. הכנסת השם למרכאות"
			},
			{
				name: "server",
				description: "הוא השם של השרת שבו אמורה לפעול התוספת. הכנסת השם למרכאות. אם התוספת מופעלת מקומית, השתמש במחרוזת ריקה"
			},
			{
				name: "topic1",
				description: "הם 1 עד 28 פרמטרים המגדירים חלק מהנתונים"
			},
			{
				name: "topic2",
				description: "הם 1 עד 28 פרמטרים המגדירים חלק מהנתונים"
			}
		]
	},
	{
		name: "SEARCH",
		description: "החזרת מיקומו של התו בו מופיעים לראשונה תו מסוים או מחרוזת טקסט מסוימת, בקריאה מימין לשמאל (הפונקציה אינה תלויית רישיות).",
		arguments: [
			{
				name: "find_text",
				description: "הטקסט שברצונך לחפש. באפשרותך להשתמש בתווים הכלליים ? ו- *; השתמש ב- ~? וב- ~* לחיפוש התווים ? ו- *"
			},
			{
				name: "within_text",
				description: "הטקסט בו ברצונך לחפש את Find_text"
			},
			{
				name: "start_num",
				description: "מיקום התו ב- Within_text, בספירה מימין, מהמקום בו ברצונך להתחיל את החיפוש. אם Strat_num מושמט, מניחים כי הוא 1"
			}
		]
	},
	{
		name: "SEC",
		description: "החזרת הסקאנס של זווית.",
		arguments: [
			{
				name: "number",
				description: "הוא הזווית ברדיאנים שעבורה אתה רוצה את הסקאנס"
			}
		]
	},
	{
		name: "SECH",
		description: "החזרת הסקאנס ההיפרבולי של זווית.",
		arguments: [
			{
				name: "number",
				description: "הוא הזווית ברדיאנים שעבורה אתה רוצה את הסקאנס ההיפרבולי"
			}
		]
	},
	{
		name: "SECOND",
		description: "החזרת השנייה, מספר בין 0 ל- 59.",
		arguments: [
			{
				name: "serial_number",
				description: "הוא מספר בקוד תאריך-שעה המשמש ב- Spreadsheet או טקסט בתבנית שעה, כגון 16:48:23 או ‎4:48:47 PM"
			}
		]
	},
	{
		name: "SERIESSUM",
		description: "החזרת הסכום של סידרת חזקות בהתבסס על הנוסחה.",
		arguments: [
			{
				name: "x",
				description: "ערך הקלט לסידרת החזקות"
			},
			{
				name: "n",
				description: "החזקה ההתחלתית שבה ברצונך להעלות את x"
			},
			{
				name: "m",
				description: "המידה שבה יש להגדיל את n עבור כל איבר בסידרה"
			},
			{
				name: "coefficients",
				description: "ערכת מקדמים בה מוכפלת כל חזקה של x"
			}
		]
	},
	{
		name: "SHEET",
		description: "החזרת מספר הגיליון של הגיליון שיש הפניה אליו.",
		arguments: [
			{
				name: "value",
				description: "הוא שם גיליון או הפניה שאתה רוצה את מספר הגיליון שלהם. אם הוא מושמט, מוחזר מספר הגיליון המכיל את הפונקציה"
			}
		]
	},
	{
		name: "SHEETS",
		description: "החזרת מספר הגליונות בהפניה.",
		arguments: [
			{
				name: "reference",
				description: "הוא הפניה שעבורה ברצונך לדעת את מספר הגליונות שהיא מכילה. אם הוא מושמט, מוחזר מספר הגליונות בחוברת העבודה המכילה את הפונקציה"
			}
		]
	},
	{
		name: "SIGN",
		description: "החזרת הסימן של מספר: 1 אם המספר חיובי, 0 אם המספר הוא אפס, או ‎-1 אם המספר שלילי.",
		arguments: [
			{
				name: "number",
				description: "כל מספר ממשי"
			}
		]
	},
	{
		name: "SIN",
		description: "החזרת הסינוס של זווית.",
		arguments: [
			{
				name: "number",
				description: "הזווית ברדיאנים עבורה ברצונך לחשב את הסינוס. מעלות * PI()/180 = רדיאנים"
			}
		]
	},
	{
		name: "SINH",
		description: "החזרת הסינוס ההיפרבולי של מספר.",
		arguments: [
			{
				name: "number",
				description: "כל מספר ממשי"
			}
		]
	},
	{
		name: "SKEW",
		description: "החזרת מידת ההטיה של התפלגות: אפיון של דרגת האסימטריה של התפלגות סביב הממוצע שלה.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "1 עד 255 מספרים או שמות, הפניות או מערכים המכילים מספרים עבורם ברצונך לחשב את מידת ההטיה"
			},
			{
				name: "number2",
				description: "1 עד 255 מספרים או שמות, הפניות או מערכים המכילים מספרים עבורם ברצונך לחשב את מידת ההטיה"
			}
		]
	},
	{
		name: "SKEW.P",
		description: "החזרת מידת ההטיה של התפלגות בהתבסס על אוכלוסיה: אפיון של דרגת האסימטריה של התפלגות סביב הממוצע שלה.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "הם 1 עד 254 מספרים או שמות, הפניות או מערכים המכילים מספרים שעבורם אתה רוצה את מידת ההטיה של האוכלוסיה"
			},
			{
				name: "number2",
				description: "הם 1 עד 254 מספרים או שמות, הפניות או מערכים המכילים מספרים שעבורם אתה רוצה את מידת ההטיה של האוכלוסיה"
			}
		]
	},
	{
		name: "SLN",
		description: "החזרת פחת קו-ישר של נכס עבור תקופה אחת.",
		arguments: [
			{
				name: "cost",
				description: "עלותו ההתחלתית של הנכס"
			},
			{
				name: "salvage",
				description: "ערך הניצולת בסוף חיי הנכס"
			},
			{
				name: "life",
				description: "מספר התקופות שבמהלכן פוחת ערכו של הנכס (נקרא לעיתים אורך החיים האפקטיבי של הנכס)"
			}
		]
	},
	{
		name: "SLOPE",
		description: "החזרת השיפוע של קו הרגרסיה הליניארית באמצעות נקודות הנתונים הנתונות.",
		arguments: [
			{
				name: "known_y's",
				description: "מערך או טווח של נקודות תלויות של נתונים מספריים, יכולים להיות מספרים או שמות, הפניות או מערכים המכילים מספרים"
			},
			{
				name: "known_x's",
				description: "מערך או טווח של נקודות בלתי-תלויות של נתונים מספריים, יכולים להיות מספרים או שמות, הפניות או מערכים המכילים מספרים"
			}
		]
	},
	{
		name: "SMALL",
		description: "החזרת ערך ה-k הקטן ביותר בקבוצת נתונים. לדוגמה, המספר החמישי הקטן ביותר.",
		arguments: [
			{
				name: "array",
				description: "מערך או טווח של נתונים מספריים עבורם ברצונך לקבוע מהו הערך ה-k הקטן ביותר"
			},
			{
				name: "k",
				description: "המיקום (החל מהקטן ביותר) של הערך שיש להחזיר במערך או בטווח התאים"
			}
		]
	},
	{
		name: "SQRT",
		description: "החזרת שורש ריבועי של מספר.",
		arguments: [
			{
				name: "number",
				description: "המספר עבורו ברצונך לחשב את השורש הריבועי"
			}
		]
	},
	{
		name: "SQRTPI",
		description: "החזרת השורש הריבועי של (‎number * pi).",
		arguments: [
			{
				name: "number",
				description: "המספר בו מוכפל p"
			}
		]
	},
	{
		name: "STANDARDIZE",
		description: "החזרת ערך מנורמל מתוך התפלגות המאופיינת על-ידי ממוצע וסטיית תקן.",
		arguments: [
			{
				name: "x",
				description: "הערך שברצונך לנרמל"
			},
			{
				name: "mean",
				description: "הממוצע החשבוני של ההתפלגות"
			},
			{
				name: "standard_dev",
				description: "סטיית התקן של ההתפלגות, מספר חיובי"
			}
		]
	},
	{
		name: "STDEV",
		description: "הפונקציה מעריכה סטיית תקן בהתבסס על מדגם (היא מתעלמת מערכים לוגיים וטקסט במדגם).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "ישנם 1 עד 255 מספרים התואמים למדגם אוכלוסין וייתכן שישנם מספרים או הפניות המכילות מספרים"
			},
			{
				name: "number2",
				description: "ישנם 1 עד 255 מספרים התואמים למדגם אוכלוסין וייתכן שישנם מספרים או הפניות המכילות מספרים"
			}
		]
	},
	{
		name: "STDEV.P",
		description: "הפונקציה מחשבת סטיית תקן בהתבסס על כל האוכלוסיה בתור ארגומנטים (המערכת מתעלמת מערכים לוגיים וטקסט).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "הם 1 עד 255 מספרים התואמים לאוכלוסיה והם יכולים להיות מספרים או הפניות המכילות מספרים"
			},
			{
				name: "number2",
				description: "הם 1 עד 255 מספרים התואמים לאוכלוסיה והם יכולים להיות מספרים או הפניות המכילות מספרים"
			}
		]
	},
	{
		name: "STDEV.S",
		description: "הפונקציה משערת סטיית תקן בהתבסס על מדגם (המערכת מתעלמת מערכים לוגיים וטקסט במדגם).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "הם 1 עד 255 מספרים התואמים למדגם אוכלוסיה והם יכולים להיות מספרים או הפניות המכילות מספרים"
			},
			{
				name: "number2",
				description: "הם 1 עד 255 מספרים התואמים למדגם אוכלוסיה והם יכולים להיות מספרים או הפניות המכילות מספרים"
			}
		]
	},
	{
		name: "STDEVA",
		description: "הערכת סטיית תקן בהתבסס על מדגם, כולל ערכים לוגיים וטקסט. ערכם של טקסט והערך הלוגי FALSE הוא 0; ערכו של הערך הלוגי TRUE הוא 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "1 עד 255 ערכים המתאימים למדגם של אוכלוסייה. יכולים להיות ערכים או שמות או הפניות לערכים"
			},
			{
				name: "value2",
				description: "1 עד 255 ערכים המתאימים למדגם של אוכלוסייה. יכולים להיות ערכים או שמות או הפניות לערכים"
			}
		]
	},
	{
		name: "STDEVP",
		description: "הפונקציה מחשבת סטיית תקן בהתבסס על האוכלוסייה כולה בתור ארגומנטים (היא מתעלמת מערכים לוגיים וטקסט).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "1 עד 255 מספרים התואמים לאוכלוסייה ועשויים להיות מספרים או הפניות המכילות מספרים"
			},
			{
				name: "number2",
				description: "1 עד 255 מספרים התואמים לאוכלוסייה ועשויים להיות מספרים או הפניות המכילות מספרים"
			}
		]
	},
	{
		name: "STDEVPA",
		description: "חישוב סטיית תקן בהתבסס על כלל האוכלוסייה, כולל ערכים לוגיים וטקסט. ערכם של טקסט והערך הלוגי FALSE הוא 0; ערכו של הערך הלוגי TRUE הוא 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "1 עד 255 ערכים המתאימים לאוכלוסייה. יכולים להיות ערכים, שמות, הפניות או מערכים המכילים מספרים"
			},
			{
				name: "value2",
				description: "1 עד 255 ערכים המתאימים לאוכלוסייה. יכולים להיות ערכים, שמות, הפניות או מערכים המכילים מספרים"
			}
		]
	},
	{
		name: "STEYX",
		description: "החזרת השגיאה הסטנדרטית של ערך y החזוי עבור כל x ברגרסיה.",
		arguments: [
			{
				name: "known_y's",
				description: "מערך או טווח של נקודות נתונים תלויות, יכולים להיות מספרים או שמות, הפניות או מערכים המכילים מספרים"
			},
			{
				name: "known_x's",
				description: "מערך או טווח של נקודות נתונים בלתי-תלויות, יכולים להיות מספרים או שמות, הפניות או מערכים המכילים מספרים"
			}
		]
	},
	{
		name: "SUBSTITUTE",
		description: "החלפת טקסט קיים בטקסט חדש בתוך מחרוזת טקסט.",
		arguments: [
			{
				name: "text",
				description: "הטקסט או הפניה לתא המכיל טקסט בו ברצונך להחליף תווים"
			},
			{
				name: "old_text",
				description: "הטקסט הקיים אותו ברצונך להחליף. אם הרישיות של Old_text אינן תואמות את הרישיות של SUBSTITUTE, text לא תחליף את הטקסט"
			},
			{
				name: "new_text",
				description: "הטקסט בו ברצונך להחליף את Old_text"
			},
			{
				name: "instance_num",
				description: "מציין איזה מופע של Old_text ברצונך להחליף. אם מושמט, מוחלף כל מופע של Old_text"
			}
		]
	},
	{
		name: "SUBTOTAL",
		description: "החזרת סכום ביניים ברשימה או במסד נתונים.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "function_num",
				description: "הוא מספר בין 1 ל-11 המציין את פונקציית הסיכום עבור סכום הביניים."
			},
			{
				name: "ref1",
				description: "הם 1 עד 254 טווחים או הפניות שעבורם ברצונך לחשב את סכום הביניים"
			}
		]
	},
	{
		name: "SUM",
		description: "סיכום כל המספרים בטווח תאים.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: " הם 1 עד 255 מספרים שיש לסכם. הפונקציה מתעלמת מערכים לוגיים וטקסט בתאים, וכוללת אותם אם הוקלדו כארגומנטים"
			},
			{
				name: "number2",
				description: " הם 1 עד 255 מספרים שיש לסכם. הפונקציה מתעלמת מערכים לוגיים וטקסט בתאים, וכוללת אותם אם הוקלדו כארגומנטים"
			}
		]
	},
	{
		name: "SUMIF",
		description: "סיכום התאים שצוינו לפי תנאי או קריטריונים נתונים.",
		arguments: [
			{
				name: "range",
				description: "טווח התאים אותו ברצונך להעריך"
			},
			{
				name: "criteria",
				description: "התנאי או הקריטריונים בצורת מספר, ביטוי או טקסט המגדירים אילו תאים יש להוסיף לסכום"
			},
			{
				name: "sum_range",
				description: "התאים שיש לסכם. אם Sum_range יושמט, התאים ב-range יסוכמו"
			}
		]
	},
	{
		name: "SUMIFS",
		description: "הוספת התאים המצוינים על-ידי ערכה נתונה של תנאים או קריטריונים.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "sum_range",
				description: "התאים הממשיים שיש לסכם."
			},
			{
				name: "criteria_range",
				description: "טווח התאים שברצונך שיוערכו עבור התנאי המסויים"
			},
			{
				name: "criteria",
				description: "התנאי או הקריטריון בצורת מספר, ביטוי, או טקסט המגדיר אילו תאים יתווספו"
			}
		]
	},
	{
		name: "SUMPRODUCT",
		description: "החזרת סכום המכפלות של הטווחים או המערכים המתאימים.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "array1",
				description: "הם 2 עד 255 מערכים שעבורם ברצונך להכפיל ולאחר מכן להוסיף רכיבים. כל המערכים צריכים להיות בעלי אותם ממדים"
			},
			{
				name: "array2",
				description: "הם 2 עד 255 מערכים שעבורם ברצונך להכפיל ולאחר מכן להוסיף רכיבים. כל המערכים צריכים להיות בעלי אותם ממדים"
			},
			{
				name: "array3",
				description: "הם 2 עד 255 מערכים שעבורם ברצונך להכפיל ולאחר מכן להוסיף רכיבים. כל המערכים צריכים להיות בעלי אותם ממדים"
			}
		]
	},
	{
		name: "SUMSQ",
		description: "החזרת סכום הריבועים של הארגומנטים. הארגומנטים יכולים להיות מספרים, מערכים, שמות או הפניות לתאים המכילים מספרים.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "1 עד 255 מספרים, מערכים, שמות או הפניות למערכים עבורם ברצונך לחשב את סכום הריבועים"
			},
			{
				name: "number2",
				description: "1 עד 255 מספרים, מערכים, שמות או הפניות למערכים עבורם ברצונך לחשב את סכום הריבועים"
			}
		]
	},
	{
		name: "SUMX2MY2",
		description: "סיכום השינויים בין הריבועים של שני טווחים או מעריכים מקבילים.",
		arguments: [
			{
				name: "array_x",
				description: "הטווח או המערך הראשון של מספרים, יכול להיות מספר או שם, מערך או הפניה המכילה מספרים"
			},
			{
				name: "array_y",
				description: "הטווח או המערך השני של מספרים, יכול להיות מספר או שם, מערך או הפניה המכילה מספרים"
			}
		]
	},
	{
		name: "SUMX2PY2",
		description: "החזרת הסכום הכולל של סכום המספרים הריבועים בשני טווחים או מעריכים מקבילים.",
		arguments: [
			{
				name: "array_x",
				description: "הטווח או המערך הראשון של מספרים, יכול להיות מספר או שם, מערך או הפניה המכילה מספרים"
			},
			{
				name: "array_y",
				description: "הטווח או המערך השני של מספרים, יכול להיות מספר או שם, מערך או הפניה המכילה מספרים"
			}
		]
	},
	{
		name: "SUMXMY2",
		description: "מסכם את הריבועים של השינויים בשני טווחים מקבילים או מערכים.",
		arguments: [
			{
				name: "array_x",
				description: "הוא הטווח הראשון או מערך הערכים ויכול להיות מספר או שם, מערך או הפניה המכילה מספרים"
			},
			{
				name: "array_y",
				description: "הוא הטווח השני של מערך ערכים ויכול להיות מספר או שם, מערך או הפניה המכילה מספרים"
			}
		]
	},
	{
		name: "SYD",
		description: "החזרת פחת הספרות של סכום השנים עבור נכס לתקופה שצוינה.",
		arguments: [
			{
				name: "cost",
				description: "עלותו ההתחלתית של הנכס"
			},
			{
				name: "salvage",
				description: "ערך הניצולת בסוף חיי הנכס"
			},
			{
				name: "life",
				description: "מספר התקופות שבמהלכן פוחת ערכו של הנכס (נקרא לעיתים אורך החיים האפקטיבי של הנכס)"
			},
			{
				name: "per",
				description: "התקופה, חייבת להשתמש באותן היחידות כמו Life"
			}
		]
	},
	{
		name: "T",
		description: "בודק האם הערך הוא טקסט, ומחזיר אותו אם הוא טקסט, או מחזיר מרכאות כפולות (טקסט ריק) אם הוא לא טקסט.",
		arguments: [
			{
				name: "value",
				description: "הוא ערך לטקסט"
			}
		]
	},
	{
		name: "T.DIST",
		description: "הפונקציה מחזירה את התפלגות t של סטודנט של זנב שמאלי.",
		arguments: [
			{
				name: "x",
				description: "הוא הערך המספרי שלפיו יש להעריך את ההתפלגות"
			},
			{
				name: "deg_freedom",
				description: "הוא מספר שלם המציין את מספר דרגות החופש המאפיינות את ההתפלגות"
			},
			{
				name: "cumulative",
				description: "הוא ערך לוגי: עבור פונקציית ההתפלגות המצטברת, השתמש ב- TRUE; עבור פונקציית צפיפות ההסתברות, השתמש ב- FALSE"
			}
		]
	},
	{
		name: "T.DIST.2T",
		description: "הפונקציה מחזירה את התפלגות t הדו-זנבית של הסטודנט.",
		arguments: [
			{
				name: "x",
				description: "הוא הערך המספרי שלפיו יש להעריך את ההתפלגות"
			},
			{
				name: "deg_freedom",
				description: "הוא מספר שלם המציין את מספר דרגות החופש המאפיינות את ההתפלגות"
			}
		]
	},
	{
		name: "T.DIST.RT",
		description: "הפונקציה מחזירה את התפלגות t של סטודנט של זנב ימני.",
		arguments: [
			{
				name: "x",
				description: "הוא הערך המספרי שלפיו יש להעריך את ההתפלגות"
			},
			{
				name: "deg_freedom",
				description: "הוא מספר שלם המציין את מספר דרגות החופש המאפיינות את ההתפלגות"
			}
		]
	},
	{
		name: "T.INV",
		description: "הפונקציה מחזירה את ההופכי של הזנב השמאלי של התפלגות t של סטודנט.",
		arguments: [
			{
				name: "probability",
				description: "הוא ההסתברות המשויכת להתפלגות t דו-זנבית של סטודנט, מספר בין 0 ל- 1, כולל"
			},
			{
				name: "deg_freedom",
				description: "הוא מספר שלם חיובי המציין את מספר דרגות החופש לאפיון ההתפלגות"
			}
		]
	},
	{
		name: "T.INV.2T",
		description: "הפונקציה מחזירה את ההופכי הדו-זנבי של התפלגות t של סטודנט.",
		arguments: [
			{
				name: "probability",
				description: "הוא ההסתברות המשויכת להתפלגות t דו-זנבית של סטודנט, מספר בין 0 ל- 1, כולל"
			},
			{
				name: "deg_freedom",
				description: "הוא מספר שלם חיובי המציין את מספר דרגות החופש לאפיון ההתפלגות"
			}
		]
	},
	{
		name: "T.TEST",
		description: "הפונקציה מחזירה את ההסתברות המשויכת למבחן t של סטודנט.",
		arguments: [
			{
				name: "array1",
				description: "הוא ערכת הנתונים הראשונה"
			},
			{
				name: "array2",
				description: "הוא ערכת הנתונים השניה"
			},
			{
				name: "tails",
				description: "מציין את מספר זנבות ההתפלגות להחזרה: התפלגות חד-זנבית = 1; התפלגות דו-זנבית = 2"
			},
			{
				name: "type",
				description: "הוא סוג מבחן t: מותאם = 1, שונות שווה של שני מדגמים (homoscedastic) = 2, שונות לא שווה של שני מדגמים = 3"
			}
		]
	},
	{
		name: "TAN",
		description: "החזרת הטנגנס של זווית.",
		arguments: [
			{
				name: "number",
				description: "הזווית ברדיאנים עבורה ברצונך לחשב את הטנגנס. מעלות * PI()/180 = רדיאנים"
			}
		]
	},
	{
		name: "TANH",
		description: "החזרת הטנגנס ההיפרבולי של מספר.",
		arguments: [
			{
				name: "number",
				description: "כל מספר ממשי"
			}
		]
	},
	{
		name: "TBILLEQ",
		description: "החזרת תשואה שוות-ערך לאגרת חוב עבור אגרת חוב ממשלתית.",
		arguments: [
			{
				name: "settlement",
				description: "תאריך הסדרת החשבון של אגרת החוב הממשלתית, המובע כמספר תאריך סידורי"
			},
			{
				name: "maturity",
				description: "תאריך הפירעון של אגרת החוב הממשלתית, המובע כמספר תאריך סידורי"
			},
			{
				name: "discount",
				description: "שער הנכיון של אגרת החוב הממשלתית"
			}
		]
	},
	{
		name: "TBILLPRICE",
		description: "החזרת המחיר לכל $100 ערך נקוב עבור אגרת חוב ממשלתית.",
		arguments: [
			{
				name: "settlement",
				description: "תאריך הסדרת החשבון של אגרת החוב הממשלתית, המובע כמספר תאריך סידורי"
			},
			{
				name: "maturity",
				description: "תאריך הפירעון של אגרת החוב הממשלתית, המובע כמספר תאריך סידורי"
			},
			{
				name: "discount",
				description: "שער הנכיון של אגרת החוב הממשלתית"
			}
		]
	},
	{
		name: "TBILLYIELD",
		description: "החזרת התשואה עבור אגרת חוב ממשלתית.",
		arguments: [
			{
				name: "settlement",
				description: "תאריך הסדרת החשבון של אגרת החוב הממשלתית, המובע כמספר תאריך סידורי"
			},
			{
				name: "maturity",
				description: "תאריך הפירעון של אגרת החוב הממשלתית, המובע כמספר תאריך סידורי"
			},
			{
				name: "pr",
				description: "מחיר אגרת החוב הממשלתית לכל $100 ערך נקוב"
			}
		]
	},
	{
		name: "TDIST",
		description: "הפונקציה מחזירה התפלגות t של סטודנט.",
		arguments: [
			{
				name: "x",
				description: "הערך המספרי שבו יש להעריך את ההתפלגות"
			},
			{
				name: "deg_freedom",
				description: "מספר שלם המציין את מספר דרגות החופש המאפיינות את ההתפלגות"
			},
			{
				name: "tails",
				description: "מציין את מספר זנבות ההתפלגות שיש להחזיר: התפלגות חד-זנבית = 1; התפלגות דו-זנבית = 2"
			}
		]
	},
	{
		name: "TEXT",
		description: "המרת ערך לטקסט בתבנית מספר מסוימת.",
		arguments: [
			{
				name: "value",
				description: "מספר, נוסחה המוערכת לערך מספרי או הפניה לתא המכיל ערך מספרי"
			},
			{
				name: "format_text",
				description: "תבנית מספר בצורת טקסט מתוך התיבה קטגוריה בכרטיסיה מספר שבתיבת הדו-שיח עיצוב תאים (לא כללי)"
			}
		]
	},
	{
		name: "TIME",
		description: "ממיר שעות, דקות ושניות הניתן כמספר למספר סידורי של Spreadsheet, מעוצב בתבנית זמן.",
		arguments: [
			{
				name: "hour",
				description: "הוא מספר מ-0 עד 23 המייצג את השעה"
			},
			{
				name: "minute",
				description: "הוא מספר מ-0 עד 59 המייצג את הדקה"
			},
			{
				name: "second",
				description: "הוא מספר מ-0 עד 59 המייצג את השניה"
			}
		]
	},
	{
		name: "TIMEVALUE",
		description: "ממיר טקסט של זמן למספר סידורי של Spreadsheet לזמן, מספר מ-0 (12:00:00 AM) עד 0.999988426 (11:59:59 PM). עצב את המספר בתבנית זמן לאחר הזנת הנוסחה.",
		arguments: [
			{
				name: "time_text",
				description: " הוא מחרוזת טקסט המחזירה זמן בכל אחת מתבניות הזמן של Spreadsheet התעלמות ממידע על תאריך במחרוזת)"
			}
		]
	},
	{
		name: "TINV",
		description: "הפונקציה מחזירה את ההופכי הדו-זנבי של התפלגות t של סטודנט.",
		arguments: [
			{
				name: "probability",
				description: "ההסתברות המשויכת להתפלגות t הדו-זנבית של סטודנט, מספר בין 0 ל- 1, כולל"
			},
			{
				name: "deg_freedom",
				description: "מספר שלם חיובי המציין את מספר דרגות החופש לאפיון ההתפלגות"
			}
		]
	},
	{
		name: "TODAY",
		description: "החזרת התאריך הנוכחי בתבנית של תאריך.",
		arguments: [
		]
	},
	{
		name: "TRANSPOSE",
		description: "המרת טווח אנכי של תאים לטווח אופקי ולהפך.",
		arguments: [
			{
				name: "array",
				description: "טווח תאים בגליון עבודה או מערך של ערכים שבו ברצונך לבצע חילוף"
			}
		]
	},
	{
		name: "TREND",
		description: "החזרת מספרים במגמה ליניארית המתאימה לנקודות נתונים ידועות, באמצעות שיטת הריבועים הפחותים.",
		arguments: [
			{
				name: "known_y's",
				description: "טווח או מערך של ערכי y הידועים לך בקשר המתואר על-ידי y = mx + b"
			},
			{
				name: "known_x's",
				description: "טווח או מערך אופציונליים של ערכי x הידועים לך בקשר המתואר על-ידי y = mx + b, מערך בגודל זהה ל- Known_y's"
			},
			{
				name: "new_x's",
				description: "טווח או מערך של ערכי x חדשים עבורם ברצונך ש- TREND תחזיר ערכי y מתאימים"
			},
			{
				name: "const",
				description: "ערך לוגי: הקבוע b מחושב באופן רגיל אם Const = TRUE או מושמט; b מוגדר להיות שווה 0 אם Const = FALSE"
			}
		]
	},
	{
		name: "TRIM",
		description: "הסרת כל הרווחים ממחרוזת טקסט למעט רווחים בודדים בין מילים.",
		arguments: [
			{
				name: "text",
				description: "הטקסט ממנו ברצונך להסיר רווחים"
			}
		]
	},
	{
		name: "TRIMMEAN",
		description: "החזרת הממוצע של חלקה הפנימי של קבוצת ערכי נתונים.",
		arguments: [
			{
				name: "array",
				description: "הטווח או המערך של נתונים אותם יש לקצץ ולחשב ממוצע"
			},
			{
				name: "percent",
				description: "שבר המציין כמה נקודות נתונים יש להוציא מהקצה העליון ומהקצה התחתון של קבוצת הנתונים"
			}
		]
	},
	{
		name: "TRUE",
		description: "החזרת הערך הלוגי TRUE.",
		arguments: [
		]
	},
	{
		name: "TRUNC",
		description: "עיגול מספר למספר שלם על-ידי הסרת החלק העשרוני או השבר של המספר.",
		arguments: [
			{
				name: "number",
				description: "המספר שברצונך לעגל"
			},
			{
				name: "num_digits",
				description: "מספר המציין את הדיוק של העיגול, 0 (אפס) אם מושמט"
			}
		]
	},
	{
		name: "TTEST",
		description: "הפונקציה מחזירה את ההסתברות המשויכת למבחן t של סטודנט.",
		arguments: [
			{
				name: "array1",
				description: "קבוצת הנתונים הראשונה"
			},
			{
				name: "array2",
				description: "קבוצת הנתונים השניה"
			},
			{
				name: "tails",
				description: "מספר זנבות ההתפלגות שיש להחזיר: התפלגות חד-זנבית = 1; התפלגות דו-זנבית = 2"
			},
			{
				name: "type",
				description: "הסוג של מבחן t: מזווג = 1, שני מדגמים שונות שווה (homoscedastic)‏ = 2, שני מדגמים שונות שונה = 3"
			}
		]
	},
	{
		name: "TYPE",
		description: "החזרת מספר שלם המתאר את סוג הנתונים של ערך: מספר= 1; טקסט= 2; ערך לוגי = 4; ערך שגיאה = 16; מערך = 64.",
		arguments: [
			{
				name: "value",
				description: "ערך כלשהו"
			}
		]
	},
	{
		name: "UNICODE",
		description: "החזרת המספר (נקודת קוד) המתאים לתו הראשון של הטקסט.",
		arguments: [
			{
				name: "text",
				description: "הוא התו שאתה רוצה את ערך ה- Unicode שלו"
			}
		]
	},
	{
		name: "UPPER",
		description: "המרת מחרוזת טקסט לכל האותיות הרישיות.",
		arguments: [
			{
				name: "text",
				description: "הטקסט שברצונך להמיר לאותיות רישיות, הפניה או מחרוזת טקסט"
			}
		]
	},
	{
		name: "VALUE",
		description: "המרת מחרוזת טקסט המייצגת מספר למספר.",
		arguments: [
			{
				name: "text",
				description: "הטקסט בין מרכאות או הפניה לתא המכיל את הטקסט שברצונך להמיר"
			}
		]
	},
	{
		name: "VAR",
		description: "הפונקציה מעריכה שונות בהתבסס על מדגם (היא מתעלמת מערכים לוגיים ומטקסט במדגם).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "1 עד 255 ארגומנטים מספריים המתאימים למדגם אוכלוסין"
			},
			{
				name: "number2",
				description: "1 עד 255 ארגומנטים מספריים המתאימים למדגם אוכלוסין"
			}
		]
	},
	{
		name: "VAR.P",
		description: "הפונקציה מחשבת שונות בהתבסס על כל האוכלוסיה (המערכת מתעלמת מערכים לוגיים וטקסט באוכלוסיה).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "הם 1 עד 255 ארגומנטים מספריים התואמים לאוכלוסיה"
			},
			{
				name: "number2",
				description: "הם 1 עד 255 ארגומנטים מספריים התואמים לאוכלוסיה"
			}
		]
	},
	{
		name: "VAR.S",
		description: "הפונקציה משערת שונות בהתבסס על מדגם (המערכת מתעלמת מערכים לוגיים וטקסט במדגם).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "הם 1 עד 255 ארגומנטים מספריים התואמים למדגם אוכלוסיה"
			},
			{
				name: "number2",
				description: "הם 1 עד 255 ארגומנטים מספריים התואמים למדגם אוכלוסיה"
			}
		]
	},
	{
		name: "VARA",
		description: "הערכת שונות בהתבסס על מדגם, כולל ערכים לוגיים וטקסט. ערכם של טקסט והערך הלוגי FALSE הוא 0; ערכו של הערך הלוגי TRUE הוא 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "1 עד 255 ארגומנטי ערכים המתאימים למדגם של אוכלוסייה"
			},
			{
				name: "value2",
				description: "1 עד 255 ארגומנטי ערכים המתאימים למדגם של אוכלוסייה"
			}
		]
	},
	{
		name: "VARP",
		description: "הפונקציה מחשבת שונות בהתבסס על האוכלוסייה כולה (היא מתעלמת מערכים לוגיים וטקסט באוכלוסייה).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "1 עד 255 ארגומנטים מספריים התואמים לאוכלוסייה"
			},
			{
				name: "number2",
				description: "1 עד 255 ארגומנטים מספריים התואמים לאוכלוסייה"
			}
		]
	},
	{
		name: "VARPA",
		description: "חישוב שונות בהתבסס על כלל האוכלוסייה, כולל ערכים לוגיים וטקסט. ערכם של טקסט והערך הלוגי FALSE הוא 0; ערכו של הערך הלוגי TRUE הוא 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "1 עד 255 ארגומנטי ערכים המתאימים לאוכלוסייה"
			},
			{
				name: "value2",
				description: "1 עד 255 ארגומנטי ערכים המתאימים לאוכלוסייה"
			}
		]
	},
	{
		name: "VDB",
		description: "החזרת הפחת של נכס עבור כל תקופה שתציין, כולל תקופות חלקיות, באמצעות שיטת היתרה הפוחתת הכפולה או שיטה אחרת שתציין.",
		arguments: [
			{
				name: "cost",
				description: "עלותו ההתחלתית של הנכס"
			},
			{
				name: "salvage",
				description: "ערך הניצולת בסוף חיי הנכס"
			},
			{
				name: "life",
				description: "מספר התקופות שבמהלכן פוחת ערכו של הנכס (נקרא לעיתים אורך החיים האפקטיבי של הנכס)"
			},
			{
				name: "start_period",
				description: "התקופה ההתחלתית עבורה ברצונך לחשב את הפחת, באותן היחידות כמו Life"
			},
			{
				name: "end_period",
				description: "התקופה הסופית עבורה ברצונך לחשב את הפחת, באותן היחידות כמו Life"
			},
			{
				name: "factor",
				description: "השיעור בו פוחתת היתרה, אם Factor מושמט, מניחים כי הוא 2 (יתרה פוחתת כפולה)"
			},
			{
				name: "no_switch",
				description: "מעבר לפחת קו-ישר כאשר הפחת גדול  מחישוב היתרה הפוחתת = FALSE או מושמט; ללא ביצוע מעבר = TRUE"
			}
		]
	},
	{
		name: "VLOOKUP",
		description: "חיפוש ערך בעמודה הראשונה של טבלה, ולאחר מכן החזרת ערך באותה השורה מעמודה שציינת. כברירת מחדל, הטבלה חייבת להיות מאוחסנת בסדר עולה.",
		arguments: [
			{
				name: "lookup_value",
				description: "הערך שיש למצוא בעמודה הראשונה של הטבלה, יכול להיות ערך, הפניה או מחרוזת טקסט"
			},
			{
				name: "table_array",
				description: "טבלה של טקסט, מספרים או ערכים לוגיים, ממנה מאוחזרים נתונים. Table_array יכול להיות הפניה לטווח או שם טווח"
			},
			{
				name: "col_index_num",
				description: "מספר העמודה ב- Table_array ממנה יש להחזיר את הערך המתאים. העמודה הראשונה של ערכים בטבלה היא עמודה 1"
			},
			{
				name: "range_lookup",
				description: "ערך לוגי: למציאת ההתאמה הקרובה ביותר בעמודה הראשונה (ממוינת בסדר עולה) = TRUE או מושמט; מציאת התאמה מדויקת = FALSE"
			}
		]
	},
	{
		name: "WEEKDAY",
		description: "החזרת מספר בין 1 ל- 7 המזהה את היום בשבוע של תאריך.",
		arguments: [
			{
				name: "serial_number",
				description: "הוא מספר המייצג תאריך"
			},
			{
				name: "return_type",
				description: "הוא מספר: עבור יום ראשון=1 עד שבת=7, השתמש ב- 1; עבור יום שני=1 עד יום ראשון=7, השתמש ב- 2; עבור יום שני=0 עד יום ראשון=6, השתמש ב- 3"
			}
		]
	},
	{
		name: "WEEKNUM",
		description: "החזרת מספר השבוע בשנה.",
		arguments: [
			{
				name: "serial_number",
				description: "קוד תאריך-שעה בו משתמש Spreadsheet לחישוב התאריך והשעה"
			},
			{
				name: "return_type",
				description: "מספר (1 או 2) הקובע את סוג הערך המוחזר"
			}
		]
	},
	{
		name: "WEIBULL",
		description: "הפונקציה מחזירה את התפלגות Weibull.",
		arguments: [
			{
				name: "x",
				description: "הערך שבו יש להעריך את הפונקציה, מספר שאינו שלילי"
			},
			{
				name: "alpha",
				description: "פרמטר להתפלגות, מספר חיובי"
			},
			{
				name: "beta",
				description: "פרמטר להתפלגות, מספר חיובי"
			},
			{
				name: "cumulative",
				description: "ערך לוגי: עבור פונקציית ההתפלגות המצטברת, השתמש ב- TRUE; עבור פונקציית מסת ההסתברות, השתמש ב- FALSE"
			}
		]
	},
	{
		name: "WEIBULL.DIST",
		description: "הפונקציה מחזירה את התפלגות Weibull.",
		arguments: [
			{
				name: "x",
				description: "הוא הערך שלפיו יש להעריך את הפונקציה, מספר שאינו שלילי"
			},
			{
				name: "alpha",
				description: "הוא פרמטר להתפלגות, מספר חיובי"
			},
			{
				name: "beta",
				description: "הוא פרמטר להתפלגות, מספר חיובי"
			},
			{
				name: "cumulative",
				description: "הוא ערך לוגי: עבור פונקציית ההתפלגות המצטברת, השתמש ב- TRUE; עבור פונקציית מסת ההסתברות, השתמש ב- FALSE"
			}
		]
	},
	{
		name: "WORKDAY",
		description: "החזרת המספר הסידורי של התאריך לפני או אחרי מספר ימי עבודה שצוין.",
		arguments: [
			{
				name: "start_date",
				description: "מספר תאריך סידורי המייצג את תאריך ההתחלה"
			},
			{
				name: "days",
				description: "מספר הימים שאינם ימי סוף שבוע ואינם ימי חג לפני או אחרי start_date"
			},
			{
				name: "holidays",
				description: "מערך אופציונלי של מספר תאריך סידורי אחד או יותר שאינו נכלל בלוח השנה של העבודה, כגון ימי חג לאומיים וימי חופש לא קבועים"
			}
		]
	},
	{
		name: "WORKDAY.INTL",
		description: "הפונקציה מחזירה את המספר הסידורי של התאריך לפני או אחרי מספר ימי עבודה שצוין עם פרמטרים מותאמים אישית של סוף שבוע.",
		arguments: [
			{
				name: "start_date",
				description: "הוא מספר תאריך סידורי המייצג את תאריך ההתחלה"
			},
			{
				name: "days",
				description: "הוא מספר הימים שאינם חלים בסוף שבוע או בחגים לפני או אחרי start_date"
			},
			{
				name: "weekend",
				description: "הוא מספר או מחרוזת המציינים מתי חלים סופי שבוע"
			},
			{
				name: "holidays",
				description: "הוא מערך אופציונלי של מספר תאריך סידורי אחד או יותר שאין לכלול בלוח השנה של העבודה, כגון חגים ממלכתיים וימי חופש לא קבועים"
			}
		]
	},
	{
		name: "XIRR",
		description: "החזרת שיעור התשואה הפנימי עבור לוח זמנים של תזרימי מזומנים.",
		arguments: [
			{
				name: "values",
				description: "סידרת תזרימי מזומנים התואמת ללוח זמנים של תשלומים לפי תאריכים"
			},
			{
				name: "dates",
				description: "לוח זמנים של תאריכי תשלום התואם לתשלומי תזרים המזומנים"
			},
			{
				name: "guess",
				description: "מספר שאתה משער כי הוא קרוב לתוצאת XIRR"
			}
		]
	},
	{
		name: "XNPV",
		description: "החזרת הערך הנקי הנוכחי עבור לוח זמנים של תזרימי מזומנים.",
		arguments: [
			{
				name: "rate",
				description: "שער הנכיון שיש להחיל על תזרימי המזומנים"
			},
			{
				name: "values",
				description: "סידרת תזרימי המזומנים התואמים ללוח זמנים של תשלומים לפי תאריכים"
			},
			{
				name: "dates",
				description: "לוח זמנים של תאריכי תשלומים התואם לתשלומי תזרים המזומנים"
			}
		]
	},
	{
		name: "XOR",
		description: "החזרת 'Exclusive Or' לוגי של כל הארגומנטים.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logical1",
				description: "הם 1 עד 254 תנאים שיש לבדוק שיכולים להיות TRUE או FALSE ויכולים להיות ערכים לוגיים, מערכים או הפניות"
			},
			{
				name: "logical2",
				description: "הם 1 עד 254 תנאים שיש לבדוק שיכולים להיות TRUE או FALSE ויכולים להיות ערכים לוגיים, מערכים או הפניות"
			}
		]
	},
	{
		name: "YEAR",
		description: "החזרת השנה של תאריך, מספר שלם בטווח 1900-9999.",
		arguments: [
			{
				name: "serial_number",
				description: "הוא מספר בקוד תאריך-שעה המשמש ב- Spreadsheet"
			}
		]
	},
	{
		name: "YEARFRAC",
		description: "החזרת חלק השנה המייצג את מספר הימים השלמים שבין start_date ו- end_date.",
		arguments: [
			{
				name: "start_date",
				description: "מספר תאריך סידורי המייצג את תאריך ההתחלה"
			},
			{
				name: "end_date",
				description: "מספר תאריך סידורי המייצג את תאריך הסיום"
			},
			{
				name: "basis",
				description: "סוג בסיס ספירת הימים שבו יש להשתמש"
			}
		]
	},
	{
		name: "YIELDDISC",
		description: "החזרת התשואה השנתית עבור נייר ערך שנוכה. לדוגמה, אגרת חוב ממשלתית.",
		arguments: [
			{
				name: "settlement",
				description: "תאריך הסדרת החשבון של נייר הערך, המובע כמספר תאריך סידורי"
			},
			{
				name: "maturity",
				description: "תאריך הפירעון של נייר הערך, המובע כמספר תאריך סידורי"
			},
			{
				name: "pr",
				description: "מחיר נייר הערך לכל $100 ערך נקוב"
			},
			{
				name: "redemption",
				description: "ערך הפדיון של נייר הערך לכל $100 ערך נקוב"
			},
			{
				name: "basis",
				description: "סוג בסיס ספירת הימים שבו יש להשתמש"
			}
		]
	},
	{
		name: "Z.TEST",
		description: "הפונקציה מחזירה ערך P חד-זנבי של מבחן z.",
		arguments: [
			{
				name: "array",
				description: "הוא המערך או הטווח של הנתונים שבו יש לבדוק את X"
			},
			{
				name: "x",
				description: "הוא הערך לבדיקה"
			},
			{
				name: "sigma",
				description: "הוא סטיית התקן של האוכלוסיה (ידועה). אם מושמט, נעשה שימוש בסטיית התקן של המדגם"
			}
		]
	},
	{
		name: "ZTEST",
		description: "הפונקציה מחזירה ערך P חד-זנבי של מבחן Z.",
		arguments: [
			{
				name: "array",
				description: "מערך הנתונים או טווח הנתונים שמולם יש לבחון את X"
			},
			{
				name: "x",
				description: "הערך לבדיקה"
			},
			{
				name: "sigma",
				description: "סטיית התקן (הידועה) של האוכלוסיה. אם מושמט, ייעשה שימוש בסטיית התקן של המדגם"
			}
		]
	}
];