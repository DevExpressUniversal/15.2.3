ASPxClientSpreadsheet.Functions = [
	{
		name: "ABS",
		description: "إرجاع القيمة المطلقة لرقم، رقم بدون إشارة.",
		arguments: [
			{
				name: "number",
				description: "هو الرقم الحقيقي الذي تريد القيمة المطلقة له"
			}
		]
	},
	{
		name: "ACCRINTM",
		description: "إرجاع الفائدة المستحقة عن ورقة مالية يستحق عنها فائدة عند تاريخ الاستحقاق.",
		arguments: [
			{
				name: "issue",
				description: "هو تاريخ إصدار الورقة المالية، يتم التعبير عنه كرقم متسلسل للتاريخ"
			},
			{
				name: "settlement",
				description: "هو تاريخ استحقاق الورقة المالية، يتم التعبير عنه كرقم متسلسل للتاريخ"
			},
			{
				name: "rate",
				description: "هو سعر الفائدة السنوي للكوبون الخاص بالورقة المالية"
			},
			{
				name: "par",
				description: "هو سعر التعادل للورقة المالية"
			},
			{
				name: "basis",
				description: "هو نوع الأساس المستخدم لحساب الأيام"
			}
		]
	},
	{
		name: "ACOS",
		description: "إرجاع قوس جيب تمام زاوية بالتقدير الدائري في النطاق من 0 إلى Pi. قوس جيب التمام هذا هو القوس الذي جيب تمامه رقم Number.",
		arguments: [
			{
				name: "number",
				description: "يجب أن يكون جيب تمام الزاوية التي تريدها من -1 إلى 1"
			}
		]
	},
	{
		name: "ACOSH",
		description: "إرجاع جيب التمام العكسي للقطع الزائد لأحد الأرقام.",
		arguments: [
			{
				name: "number",
				description: "أي رقم حقيقي يساوي 1 أو أكبر"
			}
		]
	},
	{
		name: "ACOT",
		description: "إرجاع قوس ظل التمام لرقم، بتقدير دائري في نطاق 0 إلى Pi.",
		arguments: [
			{
				name: "number",
				description: "ظل التمام للزاوية التي تريدها"
			}
		]
	},
	{
		name: "ACOTH",
		description: "إرجاع ظل التمام الزائدي العكسي لرقم.",
		arguments: [
			{
				name: "number",
				description: "ظل التمام الزائدي للزاوية التي تريدها"
			}
		]
	},
	{
		name: "ADDRESS",
		description: "إنشاء مرجع خلية كنص، بناءً على رقم صف ورقم عمود محددين.",
		arguments: [
			{
				name: "row_num",
				description: "رقم الصف الذي سيستخدم في مرجع الخلية: Row_number = 1 من أجل الصف 1"
			},
			{
				name: "column_num",
				description: "رقم العمود الذي سيستخدم في مرجع الخلية. مثال، Column_number = 4 من أجل العمود D"
			},
			{
				name: "abs_num",
				description: "تحديد نوع المرجع: مطلق = 1؛ صف مطلق/عمود نسبي = 2؛ صف نسبي/عمود مطلق = 3؛ نسبي = 4"
			},
			{
				name: "a1",
				description: "قيمة منطقية تحدد نمط المرجع: النمط A1 = 1 أو TRUE؛ النمط R1C1 = 0 أو FALSE"
			},
			{
				name: "sheet_text",
				description: "النص الذي يحدد اسم ورقة العمل التي سيستخدم كمرجع خارجي"
			}
		]
	},
	{
		name: "AND",
		description: "التحقق من أن قيم كافة الوسيطات لها القيمة TRUE، ويتم إرجاع القيمة TRUE إذا كانت قيم كافة الوسيطات هي TRUE.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logical1",
				description: "من 1 إلى 255 شرطاً  ترغب في اختبارها لمعرفة ما إذا كانت قيمة كل منها إما TRUE أو FALSE وقد تكون كذلك قيم منطقية أو صفائف أو مراجع"
			},
			{
				name: "logical2",
				description: "من 1 إلى 255 شرطاً  ترغب في اختبارها لمعرفة ما إذا كانت قيمة كل منها إما TRUE أو FALSE وقد تكون كذلك قيم منطقية أو صفائف أو مراجع"
			}
		]
	},
	{
		name: "ARABIC",
		description: "تحويل رقم روماني إلى رقم عربي.",
		arguments: [
			{
				name: "text",
				description: "الرقم الروماني الذي تريد تحويله"
			}
		]
	},
	{
		name: "AREAS",
		description: "إرجاع عدد المناطق في مرجع. والمنطقة هي مجموعة من الخلايا المتقاربة أو هي خلية واحدة فقط.",
		arguments: [
			{
				name: "reference",
				description: "مرجع خلية أو نطاق خلايا ويمكن أن يشير إلى عدة مناطق"
			}
		]
	},
	{
		name: "ASIN",
		description: "إرجاع قوس جيب زاوية بالتقدير الدائري، في النطاق من -Pi/2 إلى Pi/2‏.",
		arguments: [
			{
				name: "number",
				description: "يجب أن يكون جيب الزاوية من -1 إلى 1"
			}
		]
	},
	{
		name: "ASINH",
		description: "إرجاع جيب الزاوية العكسي لقطع زائد لأحد الأرقام.",
		arguments: [
			{
				name: "number",
				description: "أي رقم حقيقي يساوي 1 أو أكبر"
			}
		]
	},
	{
		name: "ATAN",
		description: "إرجاع مقابل ظل الزاوية بالتقدير الدائري، في النطاق من -Pi/2 إلى Pi/2‏.",
		arguments: [
			{
				name: "number",
				description: "ظل الزاوية التي تريدها"
			}
		]
	},
	{
		name: "ATAN2",
		description: "إرجاع قوس ظل زاوية الإحداثيات المحددة س و ص، بالتقدير الدائري بين القطع الناقص والزائد -Pi و +Pi، باستثناء القطع الناقص -Pi.",
		arguments: [
			{
				name: "x_num",
				description: "الإحداثي س للنقطة"
			},
			{
				name: "y_num",
				description: "الإحداثي ص للنقطة"
			}
		]
	},
	{
		name: "ATANH",
		description: "إرجاع الظل العكسي لقطع زائد لأحد الأرقام.",
		arguments: [
			{
				name: "number",
				description: "أي رقم حقيقي بين -1 و 1 باستثناء -1 و 1"
			}
		]
	},
	{
		name: "AVEDEV",
		description: "إرجاع متوسط الانحرافات المطلقة لنقاط البيانات عن الوسط الخاص بها. والوسيطات يمكن أن تكون أرقاماً أو أسماء أو صفائف أو مراجع تحتوي على أرقام.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: " من 1 إلى 255 وسيطة التي تريد متوسط الانحرافات المطلقة لها"
			},
			{
				name: "number2",
				description: " من 1 إلى 255 وسيطة التي تريد متوسط الانحرافات المطلقة لها"
			}
		]
	},
	{
		name: "AVERAGE",
		description: "إرجاع المتوسط (الوسط الحسابي) الخاص بالوسيطات والذي يمكن أن يكون أرقاماً أو أسماء أو صفائف أو مراجع تحتوي على أرقام.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: " من 1 إلى 255 وسيطة رقمية التي تريد الحصول على المتوسط الخاص بها"
			},
			{
				name: "number2",
				description: " من 1 إلى 255 وسيطة رقمية التي تريد الحصول على المتوسط الخاص بها"
			}
		]
	},
	{
		name: "AVERAGEA",
		description: "إرجاع المتوسط (الوسط الحسابي) الخاص بالوسيطات.  يتم تقيّيم النصوص و FALSE في الوسيطات كـ 0؛ ويتم تقييم TRUE كـ 1. الوسيطات يمكن أن تكون أرقاماً أو أسماء أو صفائف أو مراجع.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "‎من 1 إلى 255 وسيطة تريد الحصول على المتوسط الخاص بها"
			},
			{
				name: "value2",
				description: "‎من 1 إلى 255 وسيطة تريد الحصول على المتوسط الخاص بها"
			}
		]
	},
	{
		name: "AVERAGEIF",
		description: "إيجاد المتوسط (الوسط الحسابي) للخلايا المحددة من قبل شروط أو معايير معينة.",
		arguments: [
			{
				name: "range",
				description: "هو نطاق الخلايا التي تريد تقييمها"
			},
			{
				name: "criteria",
				description: "هو الشرط أو المعيار الموجود على شكل رقم أو تعبير أو نص يحدد الخلايا التي سيتم استخدامها لإيجاد المتوسط"
			},
			{
				name: "average_range",
				description: "هي الخلايا الفعلية التي سيتم استخدامها لإيجاد المتوسط. في حالة حذفها، يتم استخدام الخلايا الموجودة في النطاق "
			}
		]
	},
	{
		name: "AVERAGEIFS",
		description: "إيجاد على المتوسط (الوسط الحسابي) للخلايا المحددة من قبل شروط أو معايير معينة.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "average_range",
				description: "هي الخلايا الفعلية التي سيتم استخدامها للعثور على المتوسط."
			},
			{
				name: "criteria_range",
				description: "هو نطاق الخلايا التي تريد تقييمها لشرط معين"
			},
			{
				name: "criteria",
				description: "هو الشرط أو المعيار الموجود على شكل رقم أو تعبير أو نص يحدد الخلايا التي سيتم استخدامها لإيجاد المتوسط"
			}
		]
	},
	{
		name: "BAHTTEXT",
		description: "تحويل رقم إلى نص (باهت).",
		arguments: [
			{
				name: "number",
				description: "هو الرقم الذي تريد تحويله"
			}
		]
	},
	{
		name: "BASE",
		description: "تحويل رقم إلى تمثيل نصي بـ radix (الأساس) المعطى.",
		arguments: [
			{
				name: "number",
				description: "الرقم الذي تريد تحويله"
			},
			{
				name: "radix",
				description: "اRadix الأساس الذي تريد تحويل الرقم إليه"
			},
			{
				name: "min_length",
				description: "الحد الأدنى لطول السلسلة التي يتم إرجاعها. إذا تم إهمال ذلك، فلن تتم إضافة الأصفار البادئة"
			}
		]
	},
	{
		name: "BESSELI",
		description: "إرجاع دالة Bessel المعدلة In(x).",
		arguments: [
			{
				name: "x",
				description: "هي القيمة التي سيتم عندها تقييم الدالة"
			},
			{
				name: "n",
				description: "هو ترتيب دالة Bessel"
			}
		]
	},
	{
		name: "BESSELJ",
		description: "إرجاع دالة Bessel Jn(x).",
		arguments: [
			{
				name: "x",
				description: "هي القيمة التي سيتم عندها تقييم الدالة"
			},
			{
				name: "n",
				description: "هو الترتيب الخاص بدالة Bessel"
			}
		]
	},
	{
		name: "BESSELK",
		description: "إرجاع دالة Bessel المعدلة Kn(x).",
		arguments: [
			{
				name: "x",
				description: "هي القيمة التي سيتم عندها تقييم الدالة"
			},
			{
				name: "n",
				description: "هو ترتيب الدالة"
			}
		]
	},
	{
		name: "BESSELY",
		description: "إرجاع الدالة Bessel Yn(x).",
		arguments: [
			{
				name: "x",
				description: "هي القيمة التي سيتم عندها تقييم الدالة"
			},
			{
				name: "n",
				description: "هو ترتيب الدالة"
			}
		]
	},
	{
		name: "BETA.DIST",
		description: "إرجاع دالة توزيع الاحتمال beta.",
		arguments: [
			{
				name: "x",
				description: "القيمة بين A وB المراد تقييم الدالة عندها"
			},
			{
				name: "alpha",
				description: "معلمة للتوزيع ويجب أن تكون أكبر من الصفر"
			},
			{
				name: "beta",
				description: "معلمة للتوزيع ويجب أن تكون أكبر من الصفر"
			},
			{
				name: "cumulative",
				description: "قيمة منطقية: من أجل دالة التوزيع التراكمي، استخدم TRUE؛ من أجل دالة كثافة الاحتمال، استخدم FALSE"
			},
			{
				name: "A",
				description: "حدًا اختياريًا منخفضًا للفاصل x. إذا تم حذفها، A = 0‎"
			},
			{
				name: "B",
				description: "حدًا اختياريًا مرتفعًا للفاصل x. إذا تم حذفها، B = 1‎"
			}
		]
	},
	{
		name: "BETA.INV",
		description: "إرجاع عكس دالة كثافة احتمال beta التراكمية (BETADIST).",
		arguments: [
			{
				name: "probability",
				description: "احتمال مقترن بتوزيع beta"
			},
			{
				name: "alpha",
				description: "معلمة للتوزيع ويجب أن تكون أكبر من الصفر"
			},
			{
				name: "beta",
				description: "معلمة للتوزيع ويجب أن تكون أكبر من الصفر"
			},
			{
				name: "A",
				description: "حدًا اختياريًا منخفضًا للفاصل x. إذا تم حذفها، A = 0‎"
			},
			{
				name: "B",
				description: "حدًا اختياريًا مرتفعًا للفاصل x. إذا تم حذفها، B = 1‎"
			}
		]
	},
	{
		name: "BETADIST",
		description: "إرجاع دالة كثافة احتمالات beta التراكمية.",
		arguments: [
			{
				name: "x",
				description: "عبارة عن القيمة بين A و B التي تريد تقييم الدالة عندها"
			},
			{
				name: "alpha",
				description: "معلمة للتوزيع ويجب أن تكون أكبر من الصفر"
			},
			{
				name: "beta",
				description: "معلمة للتوزيع ويجب أن تكون أكبر من الصفر"
			},
			{
				name: "A",
				description: "انضمام أدنى اختياري لفاصل x. إذا كان محذوفاً، A = 0‎"
			},
			{
				name: "B",
				description: "انضمام أعلى اختياري لفاصل x. إذا كان محذوفاً، B = 1‎"
			}
		]
	},
	{
		name: "BETAINV",
		description: "إرجاع عكس دالة كثافة احتمالات beta التراكمية (BETADIST).",
		arguments: [
			{
				name: "probability",
				description: "الاحتمال المقترن مع توزيع beta"
			},
			{
				name: "alpha",
				description: "معلمة للتوزيع ويجب أن تكون أكبر من الصفر"
			},
			{
				name: "beta",
				description: "معلمة للتوزيع ويجب أن تكون أكبر من الصفر"
			},
			{
				name: "A",
				description: "انضمام أدنى اختياري لفاصل x. إذا كان محذوفاً، A = 0‎"
			},
			{
				name: "B",
				description: "انضمام أعلى اختياري لفاصل x. إذا كان محذوفاً، B = 1‎"
			}
		]
	},
	{
		name: "BIN2DEC",
		description: "تحويل رقم ثنائي إلى رقم عشري.",
		arguments: [
			{
				name: "number",
				description: "هو الرقم الثنائي الذي تريد تحويله"
			}
		]
	},
	{
		name: "BIN2HEX",
		description: "تحويل رقم ثنائي إلى رقم سداسي عشري.",
		arguments: [
			{
				name: "number",
				description: "هو الرقم الثنائي الذي تريد تحويله"
			},
			{
				name: "places",
				description: "هو عدد الأحرف التي سيتم استخدامها"
			}
		]
	},
	{
		name: "BIN2OCT",
		description: "تحويل رقم ثنائي إلى رقم ثماني.",
		arguments: [
			{
				name: "number",
				description: "هو الرقم الثنائي الذي تريد تحويله"
			},
			{
				name: "places",
				description: "هو عدد الأحرف التي سيتم استخدامها"
			}
		]
	},
	{
		name: "BINOM.DIST",
		description: "إرجاع المصطلح الفردي لاحتمال التوزيع ذي الحدين.",
		arguments: [
			{
				name: "number_s",
				description: "عدد محاولات النجاح في التجارب"
			},
			{
				name: "trials",
				description: "عدد التجارب المنفصلة"
			},
			{
				name: "probability_s",
				description: "احتمال نجاح كل تجربة"
			},
			{
				name: "cumulative",
				description: "قيمة منطقية: من أجل دالة التوزيع التراكمي، استخدم TRUE؛ من أجل دالة الاحتمالات غير التراكمية، استخدم FALSE"
			}
		]
	},
	{
		name: "BINOM.DIST.RANGE",
		description: "إرجاع احتمال نتيجة تجربة باستخدام توزيع ذي حدين.",
		arguments: [
			{
				name: "trials",
				description: "عدد التجارب المنفصلة"
			},
			{
				name: "probability_s",
				description: "احتمال نجاح كل تجربة"
			},
			{
				name: "number_s",
				description: "عدد محاولات النجاح في التجارب"
			},
			{
				name: "number_s2",
				description: "في حالة توفير ذلك، تقوم هذه الدالة بإرجاع احتمال أن عدد التجارب الناجحة يجب أن يقع بين number_s وnumber_s2"
			}
		]
	},
	{
		name: "BINOM.INV",
		description: "إرجاع القيمة الصغرى التي من أجلها يكون التوزيع التراكمي ذو الحدين أكبر من أو يساوي قيمة المعيار.",
		arguments: [
			{
				name: "trials",
				description: "عدد تجارب Bernoulli"
			},
			{
				name: "probability_s",
				description: "احتمال نجاح كل تجربة، رقم بين 0 و1 ضمنًا"
			},
			{
				name: "alpha",
				description: "قيمة المعيار، رقم بين 0 و1 ضمنًا"
			}
		]
	},
	{
		name: "BINOMDIST",
		description: "إرجاع المصطلح الفردي لاحتمال التوزيع ذي الحدين.",
		arguments: [
			{
				name: "number_s",
				description: "عدد محاولات النجاح في التجارب"
			},
			{
				name: "trials",
				description: "عدد التجارب المنفصلة"
			},
			{
				name: "probability_s",
				description: "احتمال نجاح كل تجربة"
			},
			{
				name: "cumulative",
				description: "قيمة منطقية: من أجل دالة التوزيع التراكمي، استخدم TRUE؛ من أجل  دالة الاحتمالات غير التراكمية، استخدم FALSE"
			}
		]
	},
	{
		name: "BITAND",
		description: "إرجاع البت 'And' لرقمين.",
		arguments: [
			{
				name: "number1",
				description: "التمثيل العشري للرقم الثنائي الذي تريد تقييمه"
			},
			{
				name: "number2",
				description: "التمثيل العشري للرقم الثنائي الذي تريد تقييمه"
			}
		]
	},
	{
		name: "BITLSHIFT",
		description: "إرجاع رقم تمت إزاحته إلى اليسار بمقدار shift_amount بت.",
		arguments: [
			{
				name: "number",
				description: "التمثيل العشري للرقم الثنائي الذي تريد تقييمه"
			},
			{
				name: "shift_amount",
				description: "هو عدد وحدات البت التي تريد إزاحتها بمقدار Number إلى اليسار"
			}
		]
	},
	{
		name: "BITOR",
		description: "إرجاع البت 'Or' لرقمين.",
		arguments: [
			{
				name: "number1",
				description: "التمثيل العشري للرقم الثنائي الذي تريد تقييمه"
			},
			{
				name: "number2",
				description: "التمثيل العشري للرقم الثنائي الذي تريد تقييمه"
			}
		]
	},
	{
		name: "BITRSHIFT",
		description: "إرجاع رقم تمت إزاحته إلى اليمين بمقدار shift_amount بت.",
		arguments: [
			{
				name: "number",
				description: "التمثيل العشري للرقم الثنائي الذي تريد تقييمه"
			},
			{
				name: "shift_amount",
				description: "هو عدد وحدات البت التي تريد إزاحتها بمقدار Number إلى اليمين"
			}
		]
	},
	{
		name: "BITXOR",
		description: "إرجاع البت 'Exclusive Or' لرقمين.",
		arguments: [
			{
				name: "number1",
				description: "التمثيل العشري للرقم الثنائي الذي تريد تقييمه"
			},
			{
				name: "number2",
				description: "التمثيل العشري للرقم الثنائي الذي تريد تقييمه"
			}
		]
	},
	{
		name: "CEILING",
		description: "تقريب رقم إلى الأعلى، إلى أقرب مضاعف من مضاعفات significance.",
		arguments: [
			{
				name: "number",
				description: "القيمة التي تريد تقريبها"
			},
			{
				name: "significance",
				description: "المضاعف الذي تريد التقريب إليه"
			}
		]
	},
	{
		name: "CEILING.MATH",
		description: "تقريب رقم للأعلى إلى أقرب عدد صحيح أو إلى أقرب مضاعف من مضاعفات significance.",
		arguments: [
			{
				name: "number",
				description: "القيمة التي تريد تقريبها"
			},
			{
				name: "significance",
				description: "المضاعف الذي تريد التقريب إليه"
			},
			{
				name: "mode",
				description: "عند تحديد غير الصفر، تقوم الدالة بالتقريب بعيداً عن الصفر"
			}
		]
	},
	{
		name: "CEILING.PRECISE",
		description: "تقريب رقم إلى الأعلى، إلى أقرب رقم صحيح أو إلى أقرب مضاعف من مضاعفات significance.",
		arguments: [
			{
				name: "number",
				description: "القيمة التي تريد تقريبها"
			},
			{
				name: "significance",
				description: "المضاعف الذي تريد التقريب له"
			}
		]
	},
	{
		name: "CELL",
		description: "إرجاع معلومات حول تنسيق أو موقع أو محتوى الخلية الأولى وفق ترتيب قراءة الورقة، في مرجع تعد.",
		arguments: [
			{
				name: "info_type",
				description: "قيمة نصية تحدد نوع معلومات الخلية الذي تريده.تعد"
			},
			{
				name: "reference",
				description: "هي الخلية التي تريد معلومات حولها"
			}
		]
	},
	{
		name: "CHAR",
		description: "إرجاع الحرف المحدد برمز رقمي من مجموعة الأحرف في الكمبيوتر.",
		arguments: [
			{
				name: "number",
				description: "رقم بين 1 و 255 يحدد الحرف الذي تريد"
			}
		]
	},
	{
		name: "CHIDIST",
		description: "إرجاع الاحتمال ذي الطرف الأيمن لتوزيع كاي تربيع.",
		arguments: [
			{
				name: "x",
				description: "القيمة التي تريد تقييم التوزيع عندها، رقم غير سالب"
			},
			{
				name: "deg_freedom",
				description: "عدد درجات الحرية، رقم بين 1 و10^10، باستثناء 10^10"
			}
		]
	},
	{
		name: "CHIINV",
		description: "إرجاع عكس الاحتمال ذي الطرف الأيمن لتوزيع كاي تربيع.",
		arguments: [
			{
				name: "probability",
				description: "احتمال مقترن مع توزيع كاي تربيع، قيمة بين 0 و 1 ضمناً"
			},
			{
				name: "deg_freedom",
				description: "عدد درجات الحرية، رقم بين 1 و10^10، باستثناء 10^10"
			}
		]
	},
	{
		name: "CHISQ.DIST",
		description: "إرجاع الاحتمال ذي الطرف الأيسر لتوزيع كاي تربيع.",
		arguments: [
			{
				name: "x",
				description: "القيمة التي تريد تقييم التوزيع عندها، رقم غير سالب"
			},
			{
				name: "deg_freedom",
				description: "عدد درجات الحرية، رقم بين 1 و10^10، باستثناء 10^10"
			},
			{
				name: "cumulative",
				description: "قيمة منطقية للدالة التي سيتم إرجاعها: دالة التوزيع التراكمي = TRUE؛ دالة كثافة الاحتمال = FALSE"
			}
		]
	},
	{
		name: "CHISQ.DIST.RT",
		description: "إرجاع الاحتمال ذي الطرف الأيمن لتوزيع كاي تربيع.",
		arguments: [
			{
				name: "x",
				description: "القيمة التي تريد تقييم التوزيع عندها، رقم غير سالب"
			},
			{
				name: "deg_freedom",
				description: "عدد درجات الحرية، رقم بين 1 و10^10، باستثناء 10^10"
			}
		]
	},
	{
		name: "CHISQ.INV",
		description: "إرجاع عكس الاحتمال ذي الطرف الأيسر لتوزيع كاي تربيع.",
		arguments: [
			{
				name: "probability",
				description: "احتمال مقترن بتوزيع كاي تربيع، قيمة بين 0 و1 ضمنًا"
			},
			{
				name: "deg_freedom",
				description: "عدد درجات الحرية، رقم بين 1 و10^10، باستثناء 10^10"
			}
		]
	},
	{
		name: "CHISQ.INV.RT",
		description: "إرجاع عكس الاحتمال ذي الطرف الأيمن لتوزيع كاي تربيع.",
		arguments: [
			{
				name: "probability",
				description: "احتمال مقترن بتوزيع كاي تربيع، قيمة بين 0 و1 ضمنًا"
			},
			{
				name: "deg_freedom",
				description: "عدد درجات الحرية، رقم بين 1 و10^10، باستثناء 10^10"
			}
		]
	},
	{
		name: "CHISQ.TEST",
		description: "إرجاع اختبار الاستقلال: القيمة من توزيع كاي تربيع للإحصائية وعدد درجات الحرية المناسبة.",
		arguments: [
			{
				name: "actual_range",
				description: "نطاق البيانات الذي يحتوي على المشاهدات المراد اختبارها بالمقابلة مع قيم متوقعة"
			},
			{
				name: "expected_range",
				description: "نطاق البيانات الذي يحتوي على نسبة ضرب إجماليات الصفوف وإجماليات الأعمدة إلى الإجمالي الكلي"
			}
		]
	},
	{
		name: "CHITEST",
		description: "إرجاع اختبار الاستقلال: القيمة من توزيع كاي تربيع للإحصائية وعدد درجات الحرية المناسبة.",
		arguments: [
			{
				name: "actual_range",
				description: "نطاق البيانات الذي يحتوي على المشاهدات المراد اختبارها بالمقابلة مع قيم متوقعة"
			},
			{
				name: "expected_range",
				description: "نطاق البيانات الذي يحتوي على نسبة ضرب إجماليات الصفوف وإجماليات الأعمدة إلى الإجمالي الكلي"
			}
		]
	},
	{
		name: "CHOOSE",
		description: "اختيار قيمة أو إجراء لتنفيذه من قائمة من القيم، استناداً إلى رقم فهرس.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "index_num",
				description: "تعيين وسيطة القيمة المحددة. يجب أن يكون Index_num بين 1 و 254 أو صيغة أو مرجع لرقم بين 1 و 254"
			},
			{
				name: "value1",
				description: "‎ ‎من 1 إلى 254 رقماً أو مراجع خلايا أو أسماء معرفة أو صيغ أو دالات أو وسيطات نصية تقوم CHOOSE بالتحديد منها"
			},
			{
				name: "value2",
				description: "‎ ‎من 1 إلى 254 رقماً أو مراجع خلايا أو أسماء معرفة أو صيغ أو دالات أو وسيطات نصية تقوم CHOOSE بالتحديد منها"
			}
		]
	},
	{
		name: "CLEAN",
		description: "إزالة كافة الأحرف غير القابلة للطباعة من النص.",
		arguments: [
			{
				name: "text",
				description: "أي معلومات ورقة عمل تريد إزالة الأحرف غير القابلة للطباعة منها"
			}
		]
	},
	{
		name: "CODE",
		description: "إرجاع الرمز الرقمي لمجموعة الأحرف الأولى بإحدى السلاسل النصية المستخدمة في الكمبيوتر.",
		arguments: [
			{
				name: "text",
				description: "النص الذي تريد رمز الحرف الأول الخاص به"
			}
		]
	},
	{
		name: "COLUMN",
		description: "إرجاع رقم عمود مرجع.",
		arguments: [
			{
				name: "reference",
				description: "الخلية أو النطاق من الخلايا المتقاربة تريد رقم العمود له. إذا تم إهمال ذلك، يتم استخدام الخلية التي تحتوي على دالة COLUMN"
			}
		]
	},
	{
		name: "COLUMNS",
		description: "إرجاع عدد الأعمدة في صفيف أو مرجع.",
		arguments: [
			{
				name: "array",
				description: "صفيف أو صيغة صفيف، أو مرجع نطاق من الخلايا التي تريد معرفة عدد الأعمدة الخاص بها"
			}
		]
	},
	{
		name: "COMBIN",
		description: "إرجاع عدد التوافقيات لعدد معطى من العناصر.",
		arguments: [
			{
				name: "number",
				description: "العدد الإجمالي للعناصر"
			},
			{
				name: "number_chosen",
				description: "عدد العناصر في كل واحد من التوافيق"
			}
		]
	},
	{
		name: "COMBINA",
		description: "إرجاع عدد التركيبات مع التكرارات لعدد معطى من العناصر.",
		arguments: [
			{
				name: "number",
				description: "إجمالي عدد العناصر"
			},
			{
				name: "number_chosen",
				description: "عدد العناصر في كل واحد من التركيبات"
			}
		]
	},
	{
		name: "COMPLEX",
		description: "تحويل المعاملات الحقيقية والتخيلية إلى عدد مركب.",
		arguments: [
			{
				name: "real_num",
				description: "هو المعامل الحقيقي للعدد المركب"
			},
			{
				name: "i_num",
				description: "هو المعامل التخيلي للعدد المركب"
			},
			{
				name: "suffix",
				description: "هو الملحق الخاص بالمكون التخيلي للعدد المركب"
			}
		]
	},
	{
		name: "CONCATENATE",
		description: "وصل عدة سلاسل نصية في سلسلة نصية واحدة.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "text1",
				description: "‎من 1 إلى 255 سلسلة نصية لوصلها في سلسلة نصية واحدة ويمكن أن تكون سلاسل نصية أو أرقام أو مراجع خلايا مفردة"
			},
			{
				name: "text2",
				description: "‎من 1 إلى 255 سلسلة نصية لوصلها في سلسلة نصية واحدة ويمكن أن تكون سلاسل نصية أو أرقام أو مراجع خلايا مفردة"
			}
		]
	},
	{
		name: "CONFIDENCE",
		description: "إرجاع فاصل الثقة لوسط محتوى باستخدام التوزيع العادي.",
		arguments: [
			{
				name: "alpha",
				description: "مستوى الأهمية المستخدم لحساب مستوى الثقة، رقم أكبر من 0 وأصغر من 1"
			},
			{
				name: "standard_dev",
				description: "الانحراف المعياري للمحتوى من أجل نطاق البيانات ومن المفترض أن يكون معروفاً. يجب أن يكون Standard_dev أكبر من 0"
			},
			{
				name: "size",
				description: "حجم النموذج"
			}
		]
	},
	{
		name: "CONFIDENCE.NORM",
		description: "إرجاع فاصل الثقة لوسط محتوى، باستخدام التوزيع العادي.",
		arguments: [
			{
				name: "alpha",
				description: "مستوى الأهمية المستخدم لحساب مستوى الثقة، رقم أكبر من 0 وأصغر من 1"
			},
			{
				name: "standard_dev",
				description: "الانحراف المعياري للمحتوى من أجل نطاق البيانات ومن المفترض أن يكون معروفًا. يجب أن يكون Standard_dev أكبر من 0"
			},
			{
				name: "size",
				description: "حجم النموذج"
			}
		]
	},
	{
		name: "CONFIDENCE.T",
		description: "إرجاع فاصل الثقة لوسط محتوى، باستخدام توزيع T للطالب.",
		arguments: [
			{
				name: "alpha",
				description: "مستوى الأهمية المستخدم لحساب مستوى الثقة، رقم أكبر من 0 وأصغر من 1"
			},
			{
				name: "standard_dev",
				description: "الانحراف المعياري للمحتوى الخاص بنطاق البيانات ومن المفترض أن يكون معروفًا. يجب أن يكون Standard_dev أكبر من 0"
			},
			{
				name: "size",
				description: "حجم النموذج"
			}
		]
	},
	{
		name: "CONVERT",
		description: "تحويل رقم من نظام قياس واحد إلى آخر.",
		arguments: [
			{
				name: "number",
				description: "هي قيمة من_الوحدات التي سيتم تحويلها"
			},
			{
				name: "from_unit",
				description: "هي الوحدات الخاصة بالأرقام"
			},
			{
				name: "to_unit",
				description: "هي الوحدات الخاصة بالنتيجة"
			}
		]
	},
	{
		name: "CORREL",
		description: "إرجاع معامل الارتباط بين مجموعتين من البيانات.",
		arguments: [
			{
				name: "array1",
				description: "نطاق خلايا من القيم. يجب أن تكون القيم أرقاماً، أو أسماء، أو صفائف، أو مراجع تحتوي على أرقام"
			},
			{
				name: "array2",
				description: "نطاق ثان من الخلايا فيه قيم. يجب أن تكون القيم أرقاماً، أو أسماء، أو صفائف، أو مراجع تحتوي على أرقام"
			}
		]
	},
	{
		name: "COS",
		description: "إرجاع جيب تمام الزاوية.",
		arguments: [
			{
				name: "number",
				description: "الزاوية بالتقدير الدائري التي تريد حساب جيب تمامها"
			}
		]
	},
	{
		name: "COSH",
		description: "إرجاع جيب التمام للقطع الزائد لأحد الأرقام.",
		arguments: [
			{
				name: "number",
				description: "أي رقم حقيقي"
			}
		]
	},
	{
		name: "COT",
		description: "إرجاع ظل تمام الزاوية.",
		arguments: [
			{
				name: "number",
				description: "الزاوية بالتقدير الدائري التي تريد إيجاد ظل التمام الخاص بها"
			}
		]
	},
	{
		name: "COTH",
		description: "إرجاع ظل التمام الزائدي لرقم.",
		arguments: [
			{
				name: "number",
				description: "الزاوية بالتقدير الدائري التي تريد إيجاد ظل التمام الزائدي الخاص بها"
			}
		]
	},
	{
		name: "COUNT",
		description: "حساب عدد الخلايا الموجودة في نطاق يحتوي على أرقام.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "‎ من 1 إلى 255 وسيطة يمكن أن تحتوي على أنواع متنوعة من البيانات أو تشير إليها، لكن الأرقام هي التي يتم حسابها فقط"
			},
			{
				name: "value2",
				description: "‎ من 1 إلى 255 وسيطة يمكن أن تحتوي على أنواع متنوعة من البيانات أو تشير إليها، لكن الأرقام هي التي يتم حسابها فقط"
			}
		]
	},
	{
		name: "COUNTA",
		description: "حساب عدد الخلايا الموجودة في نطاق غير فارغ.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "‎ من 1 إلى 255 وسيطة تمثل القيم والخلايا التي تريد حسابها. يمكن أن تكون القيم أي نوع من المعلومات"
			},
			{
				name: "value2",
				description: "‎ من 1 إلى 255 وسيطة تمثل القيم والخلايا التي تريد حسابها. يمكن أن تكون القيم أي نوع من المعلومات"
			}
		]
	},
	{
		name: "COUNTBLANK",
		description: "حساب عدد الخلايا الفارغة في نطاق محدد من الخلايا.",
		arguments: [
			{
				name: "range",
				description: "النطاق من الخلايا الذي تريد عد الخلايا الفارغة فيه"
			}
		]
	},
	{
		name: "COUNTIF",
		description: "حساب عدد الخلايا في نطاق والتي تحقق الشرط المعطى.",
		arguments: [
			{
				name: "range",
				description: "النطاق من الخلايا الذي تريد عد الخلايا غير الفارغة فيه"
			},
			{
				name: "criteria",
				description: "الشرط بشكل رقم، أو تعبير، أو نص والذي يعرّف الخلايا التي ستعد"
			}
		]
	},
	{
		name: "COUNTIFS",
		description: "حساب عدد الخلايا المحددة طبقاً لمجموعة من الشروط أو المعايير.",
		arguments: [
			{
				name: "criteria_range",
				description: "هو نطاق الخلايا التي ترغب في تقييمها لشرط معين"
			},
			{
				name: "criteria",
				description: "هو شرط على شكل رقم أو تعبير أو نص يحدد الخلايا التي سيتم حسابها"
			}
		]
	},
	{
		name: "COUPDAYBS",
		description: "إرجاع عدد الأيام من بداية فترة القسيمة إلى تاريخ التسوية.",
		arguments: [
			{
				name: "settlement",
				description: "هو تاريخ تسوية الأوراق المالية، يتم التعبير عنه كرقم متسلسل للتاريخ"
			},
			{
				name: "maturity",
				description: "هو تاريخ استحقاق الأوراق المالية، يتم التعبير عنه كرقم متسلسل للتاريخ"
			},
			{
				name: "frequency",
				description: "هو عدد دفعات القسائم كل سنة"
			},
			{
				name: "basis",
				description: "هو نوع الأساس المستخدم لحساب الأيام"
			}
		]
	},
	{
		name: "COUPNCD",
		description: "إرجاع التاريخ التالي للقسيمة بعد تاريخ التسوية.",
		arguments: [
			{
				name: "settlement",
				description: "هو تاريخ تسوية الأوراق المالية، يتم التعبير عنه كرقم متسلسل للتاريخ"
			},
			{
				name: "maturity",
				description: "هو تاريخ استحقاق الأوراق المالية، يتم التعبير عنه كرقم متسلسل للتاريخ"
			},
			{
				name: "frequency",
				description: "هو عدد دفعات القسائم كل سنة"
			},
			{
				name: "basis",
				description: "هو نوع الأساس المستخدم لحساب الأيام"
			}
		]
	},
	{
		name: "COUPNUM",
		description: "إرجاع عدد القسائم المستحقة الدفع بين تاريخ التسوية وتاريخ الاستحقاق.",
		arguments: [
			{
				name: "settlement",
				description: "هو تاريخ تسوية الأوراق المالية، يتم التعبير عنه كرقم متسلسل للتاريخ"
			},
			{
				name: "maturity",
				description: "هو تاريخ استحقاق الأوراق المالية، يتم التعبير عنه كرقم متسلسل للتاريخ"
			},
			{
				name: "frequency",
				description: "هو عدد دفعات القسائم كل سنة"
			},
			{
				name: "basis",
				description: "هو نوع الأساس المستخدم لحساب الأيام"
			}
		]
	},
	{
		name: "COUPPCD",
		description: "إرجاع تاريخ القسيمة السابق قبل تاريخ التسوية.",
		arguments: [
			{
				name: "settlement",
				description: "هو تاريخ تسوية الأوراق المالية، يتم التعبير عنه كرقم متسلسل للتاريخ"
			},
			{
				name: "maturity",
				description: "هو تاريخ استحقاق الأوراق المالية، يتم التعبير عنه كرقم متسلسل للتاريخ"
			},
			{
				name: "frequency",
				description: "هو عدد دفعات القسائم كل سنة"
			},
			{
				name: "basis",
				description: "هو نوع الأساس المستخدم لحساب الأيام"
			}
		]
	},
	{
		name: "COVAR",
		description: "إرجاع التباين المشترك، معدل ضرب الانحرافات لكل زوج من نقاط البيانات في مجموعتين من البيانات.",
		arguments: [
			{
				name: "array1",
				description: "النطاق الأول من خلايا ذات أرقام صحيحة ويجب أن تكون أرقامًا أو صفائف أو مراجع تحتوي على أرقام"
			},
			{
				name: "array2",
				description: "النطاق الثاني من خلايا ذات أرقام صحيحة ويجب أن تكون أرقامًا أو صفائف أو مراجع تحتوي على أرقام"
			}
		]
	},
	{
		name: "COVARIANCE.P",
		description: "إرجاع التباين المشترك للمحتوى، معدل ضرب الانحرافات لكل زوج من نقاط البيانات في مجموعتين من البيانات.",
		arguments: [
			{
				name: "array1",
				description: "النطاق الأول من خلايا ذات أرقام صحيحة ويجب أن تكون أرقامًا أو صفائف أو مراجع تحتوي على أرقام"
			},
			{
				name: "array2",
				description: "النطاق الثاني من خلايا ذات أرقام صحيحة ويجب أن تكون أرقامًا أو صفائف أو مراجع تحتوي على أرقام"
			}
		]
	},
	{
		name: "COVARIANCE.S",
		description: "إرجاع التباين المشترك للنموذج، معدل ضرب الانحرافات لكل زوج من نقاط البيانات في مجموعتين من البيانات.",
		arguments: [
			{
				name: "array1",
				description: "النطاق الأول من خلايا ذات أرقام صحيحة ويجب أن تكون أرقامًا أو صفائف أو مراجع تحتوي على أرقام"
			},
			{
				name: "array2",
				description: "النطاق الثاني من خلايا ذات أرقام صحيحة ويجب أن تكون أرقامًا أو صفائف أو مراجع تحتوي على أرقام"
			}
		]
	},
	{
		name: "CRITBINOM",
		description: "إرجاع القيمة الصغرى التي من أجلها يكون التوزيع التراكمي ذو الحدين أكبر من أو يساوي قيمة المعيار.",
		arguments: [
			{
				name: "trials",
				description: "عدد تجارب Bernoulli"
			},
			{
				name: "probability_s",
				description: "احتمال نجاح كل تجربة، رقم بين 0 و 1 ضمناً"
			},
			{
				name: "alpha",
				description: "قيمة المعيار، رقم بين 0 و1 ضمناً"
			}
		]
	},
	{
		name: "CSC",
		description: "إرجاع قاطع تمام زاوية.",
		arguments: [
			{
				name: "number",
				description: "الزاوية بالتقدير الدائري التي تريد إيجاد قاطع التمام الخاص بها"
			}
		]
	},
	{
		name: "CSCH",
		description: "إرجاع قاطع التمام الزائدي لزاوية.",
		arguments: [
			{
				name: "number",
				description: "الزاوية بالتقدير الدائري التي تريد إيجاد قاطع التمام الزائدي الخاص بها"
			}
		]
	},
	{
		name: "CUMIPMT",
		description: "إرجاع الفائدة المتراكمة المدفوعة بين فترتين.",
		arguments: [
			{
				name: "rate",
				description: "هي نسبة الفائدة"
			},
			{
				name: "nper",
				description: "هو العدد الإجمالي لفترات الدفع"
			},
			{
				name: "pv",
				description: "هي القيمة الحالية"
			},
			{
				name: "start_period",
				description: "هي الفترة الأولى في الحساب"
			},
			{
				name: "end_period",
				description: "هي الفترة الأخيرة في الحساب"
			},
			{
				name: "type",
				description: "هي الفترة الزمنية المخصصة للدفع"
			}
		]
	},
	{
		name: "CUMPRINC",
		description: "إرجاع رأس المال التراكمي المدفوع على قرض بين فترتين.",
		arguments: [
			{
				name: "rate",
				description: "هي نسبة الفائدة"
			},
			{
				name: "nper",
				description: "هو العدد الإجمالي لفترات الدفع"
			},
			{
				name: "pv",
				description: "هي القيمة الحالية"
			},
			{
				name: "start_period",
				description: "هي الفترة الأولى في الحساب"
			},
			{
				name: "end_period",
				description: "هي الفترة الأخيرة في العملية الحساب"
			},
			{
				name: "type",
				description: "هي الفترة الزمنية المخصصة للدفع"
			}
		]
	},
	{
		name: "DATE",
		description: "إرجاع رقم يمثل التاريخ في التعليمات البرمجية الخاصة بالتاريخ والوقت في Spreadsheet.",
		arguments: [
			{
				name: "year",
				description: "رقم من 1900 إلى 9999 في Spreadsheet لماكنتوش"
			},
			{
				name: "month",
				description: "رقم من 1 إلى 12 يمثل أحد أشهر السنة"
			},
			{
				name: "day",
				description: "رقم من 1 إلى 31 يمثل أحد أيام الشهر"
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
		description: "تحويل التاريخ من شكله النصي إلى رقم يمثل التاريخ في التعليمات البرمجية الخاصة بالتاريخ والوقت في Spreadsheet.",
		arguments: [
			{
				name: "date_text",
				description: "نص يمثل تاريخاً في تنسيق التاريخ لـ Spreadsheet، بين 1/1/1900 (Windows) أو 1/1/1904 (ماكنتوش) و 12/31/9999"
			}
		]
	},
	{
		name: "DAVERAGE",
		description: "الحصول على متوسط القيم في عمود بإحدى القوائم أو في قاعدة بيانات تطابق الشروط المعينة.",
		arguments: [
			{
				name: "database",
				description: "النطاق من الخلايا الذي ينشئ القائمة أو قاعدة البيانات. قاعدة البيانات هي قائمة من البيانات العلائقية"
			},
			{
				name: "field",
				description: "إما تسمية لعمود داخل علامات اقتباس زوجية أو رقم يمثل موضع العمود في القائمة"
			},
			{
				name: "criteria",
				description: "نطاق من الخلايا الذي يحتوي على شروط حددتها أنت. هذا النطاق يتضمن تسمية عمود وخلية واحدة أسفل التسمية من أجل أحد الشروط"
			}
		]
	},
	{
		name: "DAY",
		description: "إرجاع يوم من الشهر، وهو عبارة عن رقم من 1 إلى 31.",
		arguments: [
			{
				name: "serial_number",
				description: "رقم في التعليمات البرمجية الخاصة بالتاريخ والوقت المستخدم من قبل Spreadsheet"
			}
		]
	},
	{
		name: "DAYS",
		description: "إرجاع عدد الأيام بين التاريخين.",
		arguments: [
			{
				name: "end_date",
				description: "start_date وend_date هما التاريخان اللذان تريد معرفة عدد الأيام بينهما"
			},
			{
				name: "start_date",
				description: "start_date وend_date هما التاريخان اللذان تريد معرفة عدد الأيام بينهما"
			}
		]
	},
	{
		name: "DAYS360",
		description: "إرجاع عدد الأيام بين تاريخين استناداً إلى سنة فيها 360 يوماً (12 شهراً وكل شهر 30 يوماً).",
		arguments: [
			{
				name: "start_date",
				description: "start_date و end_date هما التاريخان اللذان تريد معرفة عدد الأيام بينهما"
			},
			{
				name: "end_date",
				description: "start_date و end_date هما التاريخان اللذان تريد معرفة عدد الأيام بينهما"
			},
			{
				name: "method",
				description: "قيمة منطقية تحدد أسلوب الحساب: U.S. (NASD) ‎=‎ FALSE أو مهملة؛ European = TRUE."
			}
		]
	},
	{
		name: "DB",
		description: "إرجاع إهلاك للموجودات باستخدام طريقة الإهلاك المتناقص الثابت.",
		arguments: [
			{
				name: "cost",
				description: "التكلفة الأولية للموجودات"
			},
			{
				name: "salvage",
				description: "قيمة الخردة عند نهاية عمر الموجودات"
			},
			{
				name: "life",
				description: "عدد الفترات التي سيتم فيها إهلاك الموجودات (تسمى في بعض الأحيان العمر الإنتاجي للموجودات)"
			},
			{
				name: "period",
				description: "الفترة التي تريد حساب الإهلاك من أجلها. Period يجب أن تستخدم نفس وحدات Life"
			},
			{
				name: "month",
				description: "عدد الأشهر في السنة الأولى. إذا تم إهمال month، سيفترض أنه 12"
			}
		]
	},
	{
		name: "DCOUNT",
		description: "حساب عدد الخلايا التي تحتوي على أرقام تحقق الشرط المحدد في حقل (عمود) السجلات في قاعدة البيانات.",
		arguments: [
			{
				name: "database",
				description: "النطاق من الخلايا الذي ينشئ القائمة أو قاعدة البيانات. قاعدة البيانات هي قائمة من البيانات المرتبطة"
			},
			{
				name: "field",
				description: "إما تسمية لعمود داخل علامات اقتباس زوجية أو رقم يمثل موضع العمود في القائمة"
			},
			{
				name: "criteria",
				description: "النطاق من الخلايا الذي يحتوي على شروط حددتها أنت. هذا النطاق يتضمن تسمية عمود وخلية واحدة أسفل التسمية من أجل شرط"
			}
		]
	},
	{
		name: "DCOUNTA",
		description: "حساب عدد الخلايا غير الفارغة في حقل (عمود) سجلات في قاعدة البيانات والتي تحقق الشرط المحدد.",
		arguments: [
			{
				name: "database",
				description: "النطاق من الخلايا الذي ينشئ القائمة أو قاعدة البيانات. قاعدة البيانات هي قائمة من البيانات المرتبطة"
			},
			{
				name: "field",
				description: "إما تسمية لعمود داخل علامات اقتباس زوجية أو رقم يمثل موضع العمود في القائمة"
			},
			{
				name: "criteria",
				description: "النطاق من الخلايا الذي يحتوي على شروط حددتها أنت. هذا النطاق يتضمن تسمية عمود وخلية واحدة أسفل التسمية من أجل شرط"
			}
		]
	},
	{
		name: "DDB",
		description: "إرجاع إهلاك موجودات ما لفترة محددة باستخدام أسلوب الاستهلاك المتناقص المزدوج أو باستخدام طرق أخرى أنت تحددها.",
		arguments: [
			{
				name: "cost",
				description: "التكلفة الأولية للموجودات"
			},
			{
				name: "salvage",
				description: "قيمة الخردة عند نهاية عمر الموجودات"
			},
			{
				name: "life",
				description: "عدد الفترات التي سيتم فيها استهلاك الموجودات (تسمى في بعض الأحيان العمر الإنتاجي للموجودات)"
			},
			{
				name: "period",
				description: "الفترة التي تريد حساب الإهلاك من أجلها. Period يجب أن تستخدم نفس وحدات العمر بالنسبة للفترة"
			},
			{
				name: "factor",
				description: "المعدل الذي تتناقص الميزانية عنده. إذا أهمل Factor، سيفترض أنه 2 (أسلوب الاستهلاك المتناقص المزدوج)"
			}
		]
	},
	{
		name: "DEC2BIN",
		description: "تحويل رقم عشري إلى رقم ثنائي.",
		arguments: [
			{
				name: "number",
				description: "هو العدد العشري الصحيح الذي تريد تحويله"
			},
			{
				name: "places",
				description: "هو عدد الأحرف التي سيتم استخدامها"
			}
		]
	},
	{
		name: "DEC2HEX",
		description: "تحويل رقم عشري إلى رقم سداسي عشري.",
		arguments: [
			{
				name: "number",
				description: "هو العدد العشري الصحيح الذي تريد تحويله"
			},
			{
				name: "places",
				description: "هو عدد الأحرف التي سيتم استخدامها"
			}
		]
	},
	{
		name: "DEC2OCT",
		description: "تحويل رقم عشري إلى رقم ثماني.",
		arguments: [
			{
				name: "number",
				description: "هو العدد العشري الصحيح الذي تريد تحويله"
			},
			{
				name: "places",
				description: "هو عدد الأحرف التي سيتم استخدامها"
			}
		]
	},
	{
		name: "DECIMAL",
		description: "تحويل تمثيل نصي لرقم بأساس محدد إلى رقم عشري.",
		arguments: [
			{
				name: "number",
				description: "الرقم الذي تريد تحويله"
			},
			{
				name: "radix",
				description: "Radix الأساس للرقم الذي تقوم بتحويله"
			}
		]
	},
	{
		name: "DEGREES",
		description: "تحويل من  تقدير دائري إلى درجات.",
		arguments: [
			{
				name: "angle",
				description: "الزاوية بالتقدير الدائري التي تريد تحويلها"
			}
		]
	},
	{
		name: "DELTA",
		description: "اختبار ما إذا كان رقمان متساويين.",
		arguments: [
			{
				name: "number1",
				description: "هو الرقم الأول"
			},
			{
				name: "number2",
				description: "هو الرقم الثاني"
			}
		]
	},
	{
		name: "DEVSQ",
		description: "إرجاع مجموع مربعات انحرافات نقاط البيانات عن وسطها النموذجي.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "من 1 إلى 255 وسيطة أو صفيف أو مرجع إلى صفيف، تريد أن يقوم DEVSQ بحسابها"
			},
			{
				name: "number2",
				description: "من 1 إلى 255 وسيطة أو صفيف أو مرجع إلى صفيف، تريد أن يقوم DEVSQ بحسابها"
			}
		]
	},
	{
		name: "DGET",
		description: "استخراج سجل واحد يحقق الشروط التي حددتها من قاعدة بيانات.",
		arguments: [
			{
				name: "database",
				description: "النطاق من الخلايا الذي ينشئ القائمة أو قاعدة البيانات. قاعدة البيانات هي قائمة من البيانات المرتبطة"
			},
			{
				name: "field",
				description: "إما تسمية لعمود داخل علامات اقتباس زوجية أو رقم يمثل موضع العمود في القائمة"
			},
			{
				name: "criteria",
				description: "النطاق من الخلايا الذي يحتوي على شروط حددتها أنت. هذا النطاق يتضمن تسمية عمود وخلية واحدة أسفل التسمية من أجل شرط"
			}
		]
	},
	{
		name: "DISC",
		description: "إرجاع نسبة الخصم الخاصة بالورقة المالية.",
		arguments: [
			{
				name: "settlement",
				description: "هو تاريخ تسوية الأوراق المالية، يتم التعبير عنه كرقم متسلسل للتاريخ"
			},
			{
				name: "maturity",
				description: "هو تاريخ استحقاق الأوراق المالية، يتم التعبير عنه كرقم متسلسل للتاريخ"
			},
			{
				name: "pr",
				description: "هو سعر الورقة المالية لكل قيمة اسمية لـ 100 ر.س."
			},
			{
				name: "redemption",
				description: "هي قيمة الاسترداد الخاصة بالورقة المالية لكل قيمة اسمية لـ 100 ر.س."
			},
			{
				name: "basis",
				description: "هو نوع الأساس المستخدم لحساب الأيام"
			}
		]
	},
	{
		name: "DMAX",
		description: "إرجاع أكبر رقم في حقل (عمود) من سجلات قاعدة البيانات والتي تحقق الشروط التي تحددها.",
		arguments: [
			{
				name: "database",
				description: "النطاق من الخلايا الذي ينشئ القائمة أو قاعدة البيانات. قاعدة البيانات هي قائمة من البيانات العلائقية"
			},
			{
				name: "field",
				description: "إما تسمية لعمود داخل علامات اقتباس زوجية أو رقم يمثل موضع العمود في القائمة"
			},
			{
				name: "criteria",
				description: "نطاق من الخلايا الذي يحتوي على شروط حددتها أنت. هذا النطاق يتضمن تسمية عمود وخلية واحدة أسفل التسمية من أجل أحد الشروط"
			}
		]
	},
	{
		name: "DMIN",
		description: "إرجاع أصغر رقم في حقل (عمود) من سجلات قاعدة البيانات والتي تحقق الشروط التي تحددها.",
		arguments: [
			{
				name: "database",
				description: "النطاق من الخلايا الذي ينشئ القائمة أو قاعدة البيانات. قاعدة البيانات هي قائمة من البيانات العلائقية"
			},
			{
				name: "field",
				description: "إما تسمية لعمود داخل علامات اقتباس زوجية أو رقم يمثل موضع العمود في القائمة"
			},
			{
				name: "criteria",
				description: "نطاق من الخلايا الذي يحتوي على شروط حددتها أنت. هذا النطاق يتضمن تسمية عمود وخلية واحدة أسفل التسمية من أجل أحد الشروط"
			}
		]
	},
	{
		name: "DOLLAR",
		description: "تحويل رقم إلى نص، باستخدام تنسيق العملة.",
		arguments: [
			{
				name: "number",
				description: "رقم، أو مرجع إلى خلية تحتوي على رقم، أو صيغة تقيّم إلى رقم"
			},
			{
				name: "decimals",
				description: "عدد الأرقام على يمين الفاصلة العشرية. يقرب الرقم حسب الحاجة؛ إذا أهمل، فإن المنازل العشرية = 2"
			}
		]
	},
	{
		name: "DOLLARDE",
		description: "تحويل سعر الدولار، يتم التعبير عنه ككسر، إلى سعر دولار، يتم التعبير عنه كرقم عشري.",
		arguments: [
			{
				name: "fractional_dollar",
				description: "هو الرقم الذي يتم التعبير عنه ككسر"
			},
			{
				name: "fraction",
				description: "هو العدد الصحيح المستخدم في مقام الكسر"
			}
		]
	},
	{
		name: "DOLLARFR",
		description: "تحويل سعر الدولار، يتم التعبير عنه كرقم عشري، إلى سعر دولار، يتم التعبير عنه ككسر.",
		arguments: [
			{
				name: "decimal_dollar",
				description: "هو رقم عشري"
			},
			{
				name: "fraction",
				description: "هو العدد الصحيح المستخدم في مقام الكسر"
			}
		]
	},
	{
		name: "DPRODUCT",
		description: "ضرب القيم في حقل (عمود) السجلات في قاعدة البيانات والتي تحقق الشروط التي حددتها.",
		arguments: [
			{
				name: "database",
				description: "النطاق من الخلايا الذي ينشئ القائمة أو قاعدة البيانات. قاعدة البيانات هي قائمة من البيانات المرتبطة"
			},
			{
				name: "field",
				description: "إما تسمية لعمود داخل علامات اقتباس زوجية أو رقم يمثل موضع العمود في القائمة"
			},
			{
				name: "criteria",
				description: "النطاق من الخلايا الذي يحتوي على شروط حددتها أنت. هذا النطاق يتضمن تسمية عمود وخلية واحدة أسفل التسمية من أجل شرط"
			}
		]
	},
	{
		name: "DSTDEV",
		description: "تقدير الانحراف المعياري استناداً إلى نموذج من مدخلات محددة في قاعدة بيانات.",
		arguments: [
			{
				name: "database",
				description: "النطاق من الخلايا الذي ينشئ القائمة أو قاعدة البيانات. قاعدة البيانات هي قائمة من البيانات المرتبطة"
			},
			{
				name: "field",
				description: "إما تسمية لعمود داخل علامات اقتباس زوجية أو رقم يمثل موضع العمود في القائمة"
			},
			{
				name: "criteria",
				description: "النطاق من الخلايا الذي يحتوي على شروط حددتها أنت. هذا النطاق يتضمن تسمية عمود وخلية واحدة أسفل التسمية من أجل شرط"
			}
		]
	},
	{
		name: "DSTDEVP",
		description: "حساب الانحراف المعياري استناداً إلى المحتوى بأكمله للإدخالات المحددة في قاعدة البيانات.",
		arguments: [
			{
				name: "database",
				description: "النطاق من الخلايا الذي ينشئ القائمة أو قاعدة البيانات. قاعدة البيانات هي قائمة من البيانات المرتبطة"
			},
			{
				name: "field",
				description: "إما تسمية لعمود داخل علامات اقتباس زوجية أو رقم يمثل موضع العمود في القائمة"
			},
			{
				name: "criteria",
				description: "النطاق من الخلايا الذي يحتوي على شروط حددتها أنت. هذا النطاق يتضمن عنوان عمود وخلية واحدة أسفل العنوان من أجل شرط"
			}
		]
	},
	{
		name: "DSUM",
		description: "جمع الأرقام في حقل (عمود) السجلات في قاعدة البيانات والتي تحقق الشروط التي حددتها.",
		arguments: [
			{
				name: "database",
				description: "النطاق من الخلايا الذي ينشئ القائمة أو قاعدة البيانات. قاعدة البيانات هي قائمة من البيانات المرتبطة"
			},
			{
				name: "field",
				description: "إما تسمية لعمود داخل علامات اقتباس زوجية أو رقم يمثل موضع العمود في القائمة"
			},
			{
				name: "criteria",
				description: "النطاق من الخلايا الذي يحتوي على شروط حددتها أنت. هذا النطاق يتضمن تسمية عمود وخلية واحدة أسفل التسمية من أجل شرط"
			}
		]
	},
	{
		name: "DVAR",
		description: "تقدير التباين استناداً إلى نموذج من الإدخالات المحددة في قاعدة البيانات.",
		arguments: [
			{
				name: "database",
				description: "النطاق من الخلايا الذي ينشئ القائمة أو قاعدة البيانات. قاعدة البيانات هي قائمة من البيانات المرتبطة"
			},
			{
				name: "field",
				description: "إما تسمية لعمود داخل علامات اقتباس زوجية أو رقم يمثل موضع العمود في القائمة"
			},
			{
				name: "criteria",
				description: "النطاق من الخلايا الذي يحتوي على شروط حددتها أنت. هذا النطاق يتضمن تسمية عمود وخلية واحدة أسفل التسمية من أجل شرط"
			}
		]
	},
	{
		name: "DVARP",
		description: "حساب التباين استناداً إلى المحتوى بأكمله للإدخالات المحددة في قاعدة البيانات.",
		arguments: [
			{
				name: "database",
				description: "النطاق من الخلايا الذي ينشئ القائمة أو قاعدة البيانات. قاعدة البيانات هي قائمة من البيانات المرتبطة"
			},
			{
				name: "field",
				description: "إما تسمية لعمود داخل علامات اقتباس زوجية أو رقم يمثل موضع العمود في القائمة"
			},
			{
				name: "criteria",
				description: "النطاق من الخلايا الذي يحتوي على شروط حددتها أنت. هذا النطاق يتضمن عنوان عمود وخلية واحدة أسفل العنوان من أجل شرط"
			}
		]
	},
	{
		name: "EDATE",
		description: "إرجاع الرقم المتسلسل للتاريخ الذي يشير إلى عدد الأشهر التي تسبق أو تلي تاريخ البدء.",
		arguments: [
			{
				name: "start_date",
				description: "هو رقم تاريخ متسلسل يمثل تاريخ البدء"
			},
			{
				name: "months",
				description: "هو عدد الأشهر التي تسبق أو تلي تاريخ_البدء"
			}
		]
	},
	{
		name: "EFFECT",
		description: "إرجاع النسبة الفعلية السنوية للفائدة.",
		arguments: [
			{
				name: "nominal_rate",
				description: "هو معدل الفائدة الاسمي"
			},
			{
				name: "npery",
				description: "هو عدد الفترات المتراكبة كل سنة"
			}
		]
	},
	{
		name: "ENCODEURL",
		description: "يرجع سلسلة مرمّزة بعنوان URL.",
		arguments: [
			{
				name: "النص",
				description: " سلسلة لتكون مرمّزة بعنوان URL"
			}
		]
	},
	{
		name: "EOMONTH",
		description: "إرجاع الرقم المتسلسل الخاص باليوم الأخير من الشهر الذي يسبق أو يلي عدد محدد من الأشهر.",
		arguments: [
			{
				name: "start_date",
				description: "هو رقم تاريخ متسلسل يمثل تاريخ البدء"
			},
			{
				name: "months",
				description: "هو عدد الأشهر التي تسبق أو تلي تاريخ_البدء"
			}
		]
	},
	{
		name: "ERF",
		description: "إرجاع دالة الخطأ.",
		arguments: [
			{
				name: "lower_limit",
				description: "هو الرابط السفلي لتكامل ERF"
			},
			{
				name: "upper_limit",
				description: "هو الرابط العلوي لتكامل ERF"
			}
		]
	},
	{
		name: "ERF.PRECISE",
		description: "إرجاع دالة الخطأ.",
		arguments: [
			{
				name: "X",
				description: "هو الرابط السفلي لتكامل ERF.PRECISE"
			}
		]
	},
	{
		name: "ERFC",
		description: "إرجاع دالة الخطأ التكميلية.",
		arguments: [
			{
				name: "x",
				description: "هو الرابط السفلي لتكامل ERF"
			}
		]
	},
	{
		name: "ERFC.PRECISE",
		description: "إرجاع دالة الخطأ التكميلية.",
		arguments: [
			{
				name: "X",
				description: "هو الرابط السفلي لتكامل ERFC.PRECISE"
			}
		]
	},
	{
		name: "ERROR.TYPE",
		description: "إرجاع رقم مطابق لإحدى قيم الأخطاء.",
		arguments: [
			{
				name: "error_val",
				description: "قيمة الخطأ التي تريد الرقم المعرِّف لها، ويمكن أن تكون قيمة خطأ فعلية أو أن تكون مرجعاً لخلية تحتوي على قيمة خطأ"
			}
		]
	},
	{
		name: "EVEN",
		description: "تقريب رقم موجب إلى الأعلى ورقم سالب إلى أدنى أقرب رقم صحيح زوجي.",
		arguments: [
			{
				name: "number",
				description: "القيمة التي ستقرَب"
			}
		]
	},
	{
		name: "EXACT",
		description: "التحقق من التشابه التام بين سلسلتين نصيتين وإرجاع TRUE أو FALSE. يتم تحسس حالة أحرف EXACT.",
		arguments: [
			{
				name: "text1",
				description: "السلسلة النصية الأولى"
			},
			{
				name: "text2",
				description: "السلسلة النصية الثانية"
			}
		]
	},
	{
		name: "EXP",
		description: "إرجاع e المرفوعة إلى أس الرقم المعطى.",
		arguments: [
			{
				name: "number",
				description: "هو الأس المطبق على الأساس e. العدد الثابت e يساوي 2.71828182845904، وهو أساس اللوغاريتم الطبيعي"
			}
		]
	},
	{
		name: "EXPON.DIST",
		description: "إرجاع التوزيع الأسي.",
		arguments: [
			{
				name: "x",
				description: "قيمة الدالة، رقم غير سالب"
			},
			{
				name: "lambda",
				description: "قيمة المعلمة، رقم موجب"
			},
			{
				name: "cumulative",
				description: "قيمة منطقية للدالة التي سيتم إرجاعها: دالة التوزيع التراكمي = TRUE؛ دالة كثافة الاحتمال = FALSE"
			}
		]
	},
	{
		name: "EXPONDIST",
		description: "إرجاع التوزيع الأسي.",
		arguments: [
			{
				name: "x",
				description: "قيمةالدالة، رقم غير سالب"
			},
			{
				name: "lambda",
				description: "قيمة المعلمة، رقم موجب"
			},
			{
				name: "cumulative",
				description: "قيمة منطقية للدالة التي سيتم إرجاعها: دالة التوزيع التراكمي = TRUE؛ دالة كثافة الاحتمال = FALSE"
			}
		]
	},
	{
		name: "F.DIST",
		description: "إرجاع توزيع الاحتمال F (ذي الطرف الأيسر) (درجة الاختلاف) لمجموعتين من البيانات.",
		arguments: [
			{
				name: "x",
				description: "القيمة التي تريد تقييم الدالة عندها، رقم غير سالب"
			},
			{
				name: "deg_freedom1",
				description: "بسط درجات الحرية، رقم بين 1 و10^10، باستثناء 10^10"
			},
			{
				name: "deg_freedom2",
				description: "مقام درجات الحرية، رقم بين 1 و10^10، باستثناء 10^10"
			},
			{
				name: "cumulative",
				description: "قيمة منطقية للدالة التي سيتم إرجاعها: دالة التوزيع التراكمي = TRUE؛ دالة كثافة الاحتمال = FALSE"
			}
		]
	},
	{
		name: "F.DIST.RT",
		description: "إرجاع توزيع الاحتمال F (ذي الطرف الأيمن) (درجة الاختلاف) لمجموعتين من البيانات.",
		arguments: [
			{
				name: "x",
				description: "القيمة التي تريد تقييم الدالة عندها، رقم غير سالب"
			},
			{
				name: "deg_freedom1",
				description: "بسط درجات الحرية، رقم بين 1 و10^10، باستثناء 10^10"
			},
			{
				name: "deg_freedom2",
				description: "مقام درجات الحرية، رقم بين 1 و10^10، باستثناء 10^10"
			}
		]
	},
	{
		name: "F.INV",
		description: "إرجاع عكس توزيع احتمال F (ذي الطرف الأيسر): إذا كان p = F.DIST(x,...‎)‎، فإن F.INV(p,....‎)‎ = x.",
		arguments: [
			{
				name: "probability",
				description: "احتمال مقترن بتوزيع F التراكمي، رقم بين 0 و1 ضمنًا"
			},
			{
				name: "deg_freedom1",
				description: "بسط درجات الحرية، رقم بين 1 و10^10، باستثناء 10^10"
			},
			{
				name: "deg_freedom2",
				description: "مقام درجات الحرية، رقم بين 1 و10^10، باستثناء 10^10"
			}
		]
	},
	{
		name: "F.INV.RT",
		description: "إرجاع عكس توزيع احتمال F (ذي الطرف الأيمن): إذا كان p = F.DIST.RT(x,...)‎، فإن F.INV.RT(p,...) = x.",
		arguments: [
			{
				name: "probability",
				description: "احتمال مقترن بتوزيع F التراكمي، رقم بين 0 و1 ضمنًا"
			},
			{
				name: "deg_freedom1",
				description: "بسط درجات الحرية، رقم بين 1 و10^10، باستثناء 10^10"
			},
			{
				name: "deg_freedom2",
				description: "مقام درجات الحرية، رقم بين 1 و10^10، باستثناء 10^10"
			}
		]
	},
	{
		name: "F.TEST",
		description: "إرجاع نتيجة اختبار F، وهو الاحتمال ثنائي الطرف أن تكون التباينات في Array1 وArray2 غير مختلفة إلى حدٍ كبير.",
		arguments: [
			{
				name: "array1",
				description: " الصفيف أو النطاق الأول من البيانات ويمكن أن تكون أرقامًا أو أسماء أو صفائف أو مراجع تحتوي على أرقام (يتم تجاهل الفراغات)"
			},
			{
				name: "array2",
				description: "الصفيف أو النطاق الثاني من البيانات ويمكن أن تكون أرقامًا أو أسماء أو صفائف أو مراجع تحتوي على أرقام (يتم تجاهل الفراغات)"
			}
		]
	},
	{
		name: "FACT",
		description: "إرجاع مضروب رقم، وهو مساو إلى 1*2*3*...*رقم.",
		arguments: [
			{
				name: "number",
				description: " الرقم غير السالب الذي تريد معرفة مضروبه"
			}
		]
	},
	{
		name: "FACTDOUBLE",
		description: "إرجاع العامل المزدوج للرقم.",
		arguments: [
			{
				name: "number",
				description: "هي القيمة التي سيتم إرجاع العامل المزدوج لها"
			}
		]
	},
	{
		name: "FALSE",
		description: "إرجاع القيمة المنطقية FALSE.",
		arguments: [
		]
	},
	{
		name: "FDIST",
		description: "إرجاع توزيع الاحتمال F (ذي الطرف الأيمن) (درجة الاختلاف) لمجموعتين من البيانات.",
		arguments: [
			{
				name: "x",
				description: "القيمة التي تريد تقييم الدالة عندها، رقم غير سالب"
			},
			{
				name: "deg_freedom1",
				description: "بسط درجات الحرية، رقم بين 1 و10^10، باستثناء 10^10"
			},
			{
				name: "deg_freedom2",
				description: "مقام درجات الحرية، رقم بين 1 و10^10، باستثناء 10^10"
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
		description: "إرجاع موضع البداية الخاص بسلسلة نصية في سلسلة نصية أخرى. يتم تحسس حالة أحرف FIND.",
		arguments: [
			{
				name: "find_text",
				description: "النص الذي تريد العثور عليه. استخدم علامات الاقتباس الزوجية (نص فارغ) لمطابقة الحرف الأول في Within_text؛ غير مسموح بالأحرف البدل"
			},
			{
				name: "within_text",
				description: "النص الذي يحتوي على النص الذي تريد البحث عنه"
			},
			{
				name: "start_num",
				description: "تحديد الحرف الذي عنده سيبدأ البحث. الحرف الأول في Within_text هو الحرف رقم 1. إذا أهمل، سيكون Start_num = 1‎"
			}
		]
	},
	{
		name: "FINV",
		description: "إرجاع عكس توزيع احتمال F (ذي الطرف الأيمن): إذا كان p = F.DIST.RT(x,...)‎، فإن F.INV.RT(p,...) = x.",
		arguments: [
			{
				name: "probability",
				description: "احتمال مقترن بتوزيع F التراكمي، رقم بين 0 و1 ضمنًا"
			},
			{
				name: "deg_freedom1",
				description: "بسط درجات الحرية، رقم بين 1 و10^10، باستثناء 10^10"
			},
			{
				name: "deg_freedom2",
				description: "مقام درجات الحرية، رقم بين 1 و10^10، باستثناء 10^10"
			}
		]
	},
	{
		name: "FISHER",
		description: "إرجاع تحويل Fisher.",
		arguments: [
			{
				name: "x",
				description: "القيمة التي تريد التحويل لها، رقم بين -1 و 1، باستثناء -1 و 1"
			}
		]
	},
	{
		name: "FISHERINV",
		description: "إرجاع عكس تحويل Fisher: إذا كانت y = FISHER(x)‎، فإن FISHERINV(y)‎ = x.",
		arguments: [
			{
				name: "y",
				description: "القيمة التي تريد إجراء التحويل العكسي لها"
			}
		]
	},
	{
		name: "FIXED",
		description: "تقريب رقم إلى عدد محدد من الأرقام العشرية وإرجاع النتيجة كنص بدون فواصل.",
		arguments: [
			{
				name: "number",
				description: "الرقم الذي تريد تقريبه وتحويله إلى نص"
			},
			{
				name: "decimals",
				description: "عدد الأرقام إلى يمين الفاصلة العشرية. إذا أهمل، المنازل العشرية = 2"
			},
			{
				name: "no_commas",
				description: "قيمة منطقية: عدم إظهار الفواصل في النص المرجع = TRUE؛ إظهار الفواصل في النص المرجع = FALSE أو مهمل"
			}
		]
	},
	{
		name: "FLOOR",
		description: "تقريب رقم إلى الأدنى، إلى أقرب رقم من مضاعفات significance.",
		arguments: [
			{
				name: "number",
				description: "القيمة الرقمية التي تريد تقريبها"
			},
			{
				name: "significance",
				description: "المضاعف الذي تريد التقريب إليه. يجب أن يكون Number وSignificance كلاهما موجباً أو كلاهما سالباً"
			}
		]
	},
	{
		name: "FLOOR.MATH",
		description: "تقريب رقم للأسفل إلى أقرب عدد صحيح أو إلى أقرب مضاعف من مضاعفات significance.",
		arguments: [
			{
				name: "number",
				description: "القيمة التي تريد تقريبها"
			},
			{
				name: "significance",
				description: "المضاعف الذي تريد التقريب إليه"
			},
			{
				name: "mode",
				description: "عند تحديد غير الصفر، تقوم الدالة بالتقريب في اتجاه الصفر"
			}
		]
	},
	{
		name: "FLOOR.PRECISE",
		description: "تقريب رقم إلى الأدنى، إلى أقرب عدد صحيح أو إلى أقرب رقم من مضاعفات significance.",
		arguments: [
			{
				name: "number",
				description: "القيمة الرقمية التي تريد تقريبها"
			},
			{
				name: "significance",
				description: "المضاعف الذي تريد التقريب إليه. "
			}
		]
	},
	{
		name: "FORECAST",
		description: "حساب، أو توقع، القيمة المستقبلية باتجاه خطي وذلك باستخدام قيم موجودة.",
		arguments: [
			{
				name: "x",
				description: "نقطة البيانات التي تريد توقع قيمة لها ويجب أن تكون قيمة رقمية"
			},
			{
				name: "known_y's",
				description: "صفيف تابع أو نطاق من البيانات الرقمية"
			},
			{
				name: "known_x's",
				description: "صفيف مستقل أو نطاق من البيانات الرقمية. يجب ألا يكون تباين Known_x مساوياً للصفر"
			}
		]
	},
	{
		name: "FORMULATEXT",
		description: "إرجاع صيغة كسلسلة.",
		arguments: [
			{
				name: "reference",
				description: "مرجع لصيغة"
			}
		]
	},
	{
		name: "FREQUENCY",
		description: "حساب عدد مرات ظهور القيم في نطاق من القيم ثم إرجاع صفيف عمودي من الأرقام عدد عناصره تزيد واحداً عن Bins_array.",
		arguments: [
			{
				name: "data_array",
				description: "صفيف من القيم أو مرجع إلى القيم التي تريد عدد ترددها (يتم تجاهل الفراغات والنصوص)"
			},
			{
				name: "bins_array",
				description: "صفيف من الفواصل أو مرجع إلى فواصل حيث تريد تجميع القيم الموجودة في data_array"
			}
		]
	},
	{
		name: "FTEST",
		description: "إرجاع نتيجة اختبار F، وهو الاحتمال ثنائي الطرف أن تكون التباينات في Array1 وArray2 غير مختلفة إلى حدٍ كبير.",
		arguments: [
			{
				name: "array1",
				description: " الصفيف أو النطاق الأول من البيانات ويمكن أن تكون أرقامًا أو أسماء أو صفائف أو مراجع تحتوي على أرقام (يتم تجاهل الفراغات)"
			},
			{
				name: "array2",
				description: "الصفيف أو النطاق الثاني من البيانات ويمكن أن تكون أرقامًا أو أسماء أو صفائف أو مراجع تحتوي على أرقام (يتم تجاهل الفراغات)"
			}
		]
	},
	{
		name: "FV",
		description: "إرجاع القيمة المستقبلية للاستثمار بالاستناد إلى دفعات دورية ثابتة، وإلى نسبة فائدة ثابتة.",
		arguments: [
			{
				name: "rate",
				description: "نسبة الفائدة لكل فترة. على سبيل المثال، استخدام 6%/4 للدفعات الموسمية عند 6% APR"
			},
			{
				name: "nper",
				description: "العدد الإجمالي لفترات الدفع في الاستثمار"
			},
			{
				name: "pmt",
				description: "الدفعة خلال كل فترة؛ ولا يمكن أن يتغير خلال مدة الاستثمار"
			},
			{
				name: "pv",
				description: "القيمة الحالية، أو مقدار المبلغ الإجمالي الذي تساويه الدفعات المستقبلية الآن. إذا أهمل، Pv = 0‎"
			},
			{
				name: "type",
				description: "قيمة تمثل توقيت الدفعات: الدفعة في بداية الفترة = 1؛ الدفعة في نهاية الفترة = 0 أو مهملة"
			}
		]
	},
	{
		name: "FVSCHEDULE",
		description: "إرجاع القيمة المستقبلية لرأس مال أولي بعد تطبيق سلسلة من معدلات الفائدة المركبة.",
		arguments: [
			{
				name: "principal",
				description: "هي القيمة الحالية"
			},
			{
				name: "schedule",
				description: "هي صفيف من معدلات الفائدة التي سيتم تطبيقها"
			}
		]
	},
	{
		name: "GAMMA",
		description: "إرجاع قيمة دالة غاما.",
		arguments: [
			{
				name: "x",
				description: "القيمة التي تريد حساب غاما لها"
			}
		]
	},
	{
		name: "GAMMA.DIST",
		description: "إرجاع توزيع غاما.",
		arguments: [
			{
				name: "x",
				description: "القيمة التي عندها تريد تقييم التوزيع، رقم غير سالب"
			},
			{
				name: "alpha",
				description: "معلمة للتوزيع، عدد موجب"
			},
			{
				name: "beta",
				description: "معلمة للتوزيع، رقم موجب. إذا كان beta = 1، فإن GAMMADIST يرجع توزيع غاما القياسي"
			},
			{
				name: "cumulative",
				description: "قيمة منطقية: إرجاع دالة التوزيع التراكمي = TRUE؛ إرجاع دالة الاحتمالات غير التراكمية = FALSE أو حذفه"
			}
		]
	},
	{
		name: "GAMMA.INV",
		description: "إرجاع عكس توزيع غاما التراكمي: إذا كان p = GAMMADIST(x,...‎)‎، فإن GAMMAINV(p,...‎)‎ = x.",
		arguments: [
			{
				name: "probability",
				description: "الاحتمال المقترن بتوزيع غاما، رقم بين 0 و1، ضمنًا"
			},
			{
				name: "alpha",
				description: "معلمة للتوزيع، عدد موجب"
			},
			{
				name: "beta",
				description: "معلمة للتوزيع، رقم موجب. إذا كان beta = 1، فإن GAMMAINV يرجع عكس توزيع غاما القياسي"
			}
		]
	},
	{
		name: "GAMMADIST",
		description: "إرجاع توزيع غاما.",
		arguments: [
			{
				name: "x",
				description: "القيمة التي عندها تريد تقييم التوزيع، رقم غير سالب"
			},
			{
				name: "alpha",
				description: "معلمة للتوزيع، عدد موجب"
			},
			{
				name: "beta",
				description: "معلمة للتوزيع، رقم موجب. إذا كان beta = 1، فإن GAMMADIST يرجع توزيع غاما القياسي"
			},
			{
				name: "cumulative",
				description: "قيمة منطقية: إرجاع دالة التوزيع التراكمي = TRUE؛ إرجاع دالة الاحتمالات غير التراكمية = FALSE أو حذفه"
			}
		]
	},
	{
		name: "GAMMAINV",
		description: "إرجاع عكس توزيع غاما التراكمي: إذا كان p = GAMMADIST(x,...‎)‎، فإن GAMMAINV(p,...‎)‎ = x.",
		arguments: [
			{
				name: "probability",
				description: "الاحتمال المقترن بتوزيع غاما، رقم بين 0 و1، ضمنًا"
			},
			{
				name: "alpha",
				description: "معلمة للتوزيع، عدد موجب"
			},
			{
				name: "beta",
				description: "معلمة للتوزيع، رقم موجب. إذا كان beta = 1، فإن GAMMAINV يرجع عكس توزيع غاما القياسي"
			}
		]
	},
	{
		name: "GAMMALN",
		description: "إرجاع اللوغاريتم الطبيعي لدالة غاما.",
		arguments: [
			{
				name: "x",
				description: "القيمة التي تريد حساب GAMMALN لها، وهي رقم موجب"
			}
		]
	},
	{
		name: "GAMMALN.PRECISE",
		description: "إرجاع اللوغاريتم الطبيعي لدالة غاما.",
		arguments: [
			{
				name: "x",
				description: "القيمة التي تريد حساب GAMMALN.PRECISE لها، وهي رقم موجب"
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
		description: "إرجاع أكبر عامل قسمة مشترك.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "هي القيم من 1 إلى 255"
			},
			{
				name: "number2",
				description: "هي القيم من 1 إلى 255"
			}
		]
	},
	{
		name: "GEOMEAN",
		description: "إرجاع الوسط الهندسي لصفيف أو لنطاق من بيانات رقمية موجبة.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "من 1 إلى 255 رقماً أو  اسماً أو صفيفاً أو مرجعاُ يحتوي على أرقام تريد الحصول على الوسط الخاص بها"
			},
			{
				name: "number2",
				description: "من 1 إلى 255 رقماً أو  اسماً أو صفيفاً أو مرجعاُ يحتوي على أرقام تريد الحصول على الوسط الخاص بها"
			}
		]
	},
	{
		name: "GESTEP",
		description: "اختبار ما إذا كان الرقم أكبر من قيمة العتبة.",
		arguments: [
			{
				name: "number",
				description: "هي القيمة التي سيتم اختبارها في مقابل الخطوة"
			},
			{
				name: "step",
				description: "هي قيمة العتبة"
			}
		]
	},
	{
		name: "GETPIVOTDATA",
		description: "استخراج البيانات المخزنة في PivotTable.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "data_field",
				description: " هو اسم حقل البيانات الذي سيتم استخراج البيانات منه"
			},
			{
				name: "pivot_table",
				description: "هو مرجع إلى خلية أو نطاق من الخلايا في PivotTable الذي يحتوي على البيانات التي تريد استردادها"
			},
			{
				name: "field",
				description: "حقول للرجوع إليها"
			},
			{
				name: "item",
				description: "وعناصر الحقول للرجوع إليها"
			}
		]
	},
	{
		name: "GROWTH",
		description: "إرجاع الأرقام الموجودة باتجاه التزايد الأسي المطابق لنقاط البيانات المعروفة.",
		arguments: [
			{
				name: "known_y's",
				description: "مجموعة من قيم ص التي تعرفها مسبقاً من العلاقة y = b*m^x، صفيف أو نطاق من الأرقام الموجبة"
			},
			{
				name: "known_x's",
				description: "مجموعة اختيارية من قيم س والتي قد تعرفها مسبقاً في العلاقة y = b*m^x، صفيف أو نطاق له نفس حجم Known_y's"
			},
			{
				name: "new_x's",
				description: "قيم س الجديدة والمطابقة لقيم ص التي تريد من GROWTH إرجاعها"
			},
			{
				name: "const",
				description: "قيمة منطقية: سيحسب العدد الثابت b بشكل اعتيادي إذا كان Const = TRUE؛ سيعيّن b مساوياَ للواحد إذا كان Const = FALSE أو مهملاً"
			}
		]
	},
	{
		name: "HARMEAN",
		description: "إرجاع الوسط التوافقي لمجموعة بيانات ذات أرقام موجبة:  وهو معكوس الوسط الحسابي لمقلوب الأرقام.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "من 1 إلى 255 رقماً أو  اسماً أو صفيفاً أو مرجعاُ يحتوي على أرقام تريد الحصول على الوسط  التوافقي الخاص بها"
			},
			{
				name: "number2",
				description: "من 1 إلى 255 رقماً أو  اسماً أو صفيفاً أو مرجعاُ يحتوي على أرقام تريد الحصول على الوسط  التوافقي الخاص بها"
			}
		]
	},
	{
		name: "HEX2BIN",
		description: "تحويل رقم سداسي عشري إلى رقم ثنائي.",
		arguments: [
			{
				name: "number",
				description: "هو الرقم السداسي العشري الذي تريد تحويله"
			},
			{
				name: "places",
				description: "هو عدد الأحرف التي سيتم استخدامها"
			}
		]
	},
	{
		name: "HEX2DEC",
		description: "تحويل رقم سداسي عشري إلى رقم عشري.",
		arguments: [
			{
				name: "number",
				description: "هو الرقم السداسي العشري الذي تريد تحويله"
			}
		]
	},
	{
		name: "HEX2OCT",
		description: "تحويل رقم سداسي عشري إلى رقم ثماني.",
		arguments: [
			{
				name: "number",
				description: "هو الرقم السداسي العشري الذي تريد تحويله"
			},
			{
				name: "places",
				description: "هو عدد الأحرف التي سيتم استخدامها"
			}
		]
	},
	{
		name: "HLOOKUP",
		description: "بحث عن قيمة في الصف العلوي لجدول أو في الصف العلوي لصفيف من القيم وإرجاع القيمة في نفس العمود من صف تحدده أنت.",
		arguments: [
			{
				name: "lookup_value",
				description: "القيمة التي سيعثر عليها في الصف الأول من الجدول ويمكن أن تكون قيمة، أو مرجعاً، أو سلسلة نصية"
			},
			{
				name: "table_array",
				description: "جدول من نصوص، أو أرقام، أو قيم منطقية يجري البحث عن البيانات فيها. يمكن لـ Table_array أن يكون مرجعاً إلى نطاق أو أن يكون اسم نطاق"
			},
			{
				name: "row_index_num",
				description: "رقم الصف في table_array الذي يجب إرجاع القيمة المطابقة منه. الصف الأول من القيم في الجدول هو الصف 1"
			},
			{
				name: "range_lookup",
				description: "قيمة منطقية: للبحث عن التطابق الأقرب في الصف الأعلى (مفروز بترتيب تصاعدي) = TRUE أو مهمل؛ للبحث عن التطابق الكامل = FALSE"
			}
		]
	},
	{
		name: "HOUR",
		description: "إرجاع الساعة كرقم من 0 (12:00 ص) إلى 23 (11:00 م).",
		arguments: [
			{
				name: "serial_number",
				description: "رقم في التعليمات البرمجية الخاصة بالتاريخ والوقت المستخدم من قبل Spreadsheet، أو نص بتنسيق وقتي، مثل 16:48:00 أو 4:48:00 م"
			}
		]
	},
	{
		name: "HYPERLINK",
		description: "إنشاء اختصار أو إنشاء انتقال يفتح مستنداً مخزناً في القرص الثابت الخاص بك، أو في خادم شبكة، أو على الإنترنت.",
		arguments: [
			{
				name: "link_location",
				description: "النص الذي يعطي مسار واسم الملف للمستند الذي سيفتح، أو موقع على القرص الثابت، أو عنوان UNC، أو مسار URL"
			},
			{
				name: "friendly_name",
				description: "نص أو رقم ظاهر في الخلية. إذا أهمل، فإن الخلية ستظهر النص Link_Location"
			}
		]
	},
	{
		name: "HYPGEOM.DIST",
		description: "إرجاع توزيع الهندسة الفوقية.",
		arguments: [
			{
				name: "sample_s",
				description: "عدد محاولات النجاح في النموذج"
			},
			{
				name: "number_sample",
				description: "حجم النموذج"
			},
			{
				name: "population_s",
				description: "عدد محاولات النجاح في المحتوى"
			},
			{
				name: "number_pop",
				description: "حجم المحتوى"
			},
			{
				name: "cumulative",
				description: "قيمة منطقية: من أجل دالة التوزيع التراكمي، استخدم TRUE؛ من أجل دالة كثافة الاحتمال، استخدم FALSE"
			}
		]
	},
	{
		name: "HYPGEOMDIST",
		description: "إرجاع توزيع الهندسة الفوقية.",
		arguments: [
			{
				name: "sample_s",
				description: "عدد محاولات النجاح في النموذج"
			},
			{
				name: "number_sample",
				description: "حجم النموذج"
			},
			{
				name: "population_s",
				description: "عدد محاولات النجاح في المحتوى"
			},
			{
				name: "number_pop",
				description: "حجم المحتوى"
			}
		]
	},
	{
		name: "IF",
		description: "التأكد من تحقق الشرط وإرجاع قيمة معينة عند TRUEوأخرى عند FALSE.",
		arguments: [
			{
				name: "logical_test",
				description: "هي أية قيمة أو تعبير يمكن تعيينه إلى TRUE أو FALSE"
			},
			{
				name: "value_if_true",
				description: "هي القيمة التي يتم إرجاعها إذا كانت Logical_test هي TRUE. إذا تم الحذف، يتم إرجاع القيمة TRUE. يمكنك تضمين حتى سبع دوال IF"
			},
			{
				name: "value_if_false",
				description: " كقيمة يتم إرجاعها إذا كانت Logical_test هي القيمة FALSE. إذا تم الحذف، يتم إرجاع القيمة FALSE"
			}
		]
	},
	{
		name: "IFERROR",
		description: "إرجاع value_if_error إذا كان التعبير خطأ وقيمة التعبير ذاته غير ذلك.",
		arguments: [
			{
				name: "value",
				description: "هي أي قيمة أو تعبير أو مرجع"
			},
			{
				name: "value_if_error",
				description: "هي أي قيمة أو تعبير أو مرجع"
			}
		]
	},
	{
		name: "IFNA",
		description: "إرجاع القيمة التي تحددها إذا تحوّل التعبير إلى #N/A، وبخلاف ذلك، يتم إرجاع نتيجة التعبير.",
		arguments: [
			{
				name: "value",
				description: "أي قيمة أو تعبير أو مرجع"
			},
			{
				name: "value_if_na",
				description: "أي قيمة أو تعبير أو مرجع"
			}
		]
	},
	{
		name: "IMABS",
		description: "إرجاع القيمة المطلقة (المعامل) الخاصة بعدد مركب.",
		arguments: [
			{
				name: "inumber",
				description: "هو العدد المركب الذي ترغب في إيجاد القيمة المطلقة الخاصة به"
			}
		]
	},
	{
		name: "IMAGINARY",
		description: "إرجاع المعامل التخيلي لعدد مركب.",
		arguments: [
			{
				name: "inumber",
				description: "هو العدد المركب الذي ترغب في إيجاد المعامل التخيلي الخاص به"
			}
		]
	},
	{
		name: "IMARGUMENT",
		description: "إرجاع الوسيطة q، زاوية يتم التعبير عنها في زاوية نصف قطرية.",
		arguments: [
			{
				name: "inumber",
				description: "هو عدد مركب ترغب في إيجاد الوسيطة الخاصة به"
			}
		]
	},
	{
		name: "IMCONJUGATE",
		description: "إرجاع المرافق المركب لعدد مركب.",
		arguments: [
			{
				name: "inumber",
				description: "هو العدد المركب الذي ترغب في إيجاد المرافق الخاص به"
			}
		]
	},
	{
		name: "IMCOS",
		description: "إرجاع جيب التمام الخاص بعدد مركب.",
		arguments: [
			{
				name: "inumber",
				description: "هو العدد المركب الذي تريد إيجاد جيب التمام الخاص به"
			}
		]
	},
	{
		name: "IMCOSH",
		description: "إرجاع جيب التمام الزائدي لعدد مركب.",
		arguments: [
			{
				name: "inumber",
				description: "العدد المركب الذي تريد إيجاد جيب التمام الزائدي له"
			}
		]
	},
	{
		name: "IMCOT",
		description: "إرجاع ظل التمام لعدد مركب.",
		arguments: [
			{
				name: "inumber",
				description: "العدد المركب الذي تريد إيجاد ظل التمام الخاص به"
			}
		]
	},
	{
		name: "IMCSC",
		description: "إرجاع قاطع التمام لعدد مركب.",
		arguments: [
			{
				name: "inumber",
				description: "العدد المركب الذي تريد إيجاد قاطع التمام الخاص به"
			}
		]
	},
	{
		name: "IMCSCH",
		description: "إرجاع قاطع التمام الزائدي لعدد مركب.",
		arguments: [
			{
				name: "inumber",
				description: "العدد المركب الذي تريد إيجاد قاطع التمام الزائدي الخاص به"
			}
		]
	},
	{
		name: "IMDIV",
		description: "إرجاع حاصل قسمة عددين مركبين.",
		arguments: [
			{
				name: "inumber1",
				description: "هو الكسر المركب أو المقسوم المركب"
			},
			{
				name: "inumber2",
				description: "هو المقام المركب أو عامل القسمة المركب"
			}
		]
	},
	{
		name: "IMEXP",
		description: "إرجاع الأس الخاص بعدد مركب.",
		arguments: [
			{
				name: "inumber",
				description: "هو العدد المركب الذي ترغب في إيجاد الأس الخاص به"
			}
		]
	},
	{
		name: "IMLN",
		description: "إرجاع اللوغاريتم الطبيعي لعدد مركب.",
		arguments: [
			{
				name: "inumber",
				description: "هو العدد المركب الذي ترغب في إيجاد اللوغاريتم الطبيعي الخاص به"
			}
		]
	},
	{
		name: "IMLOG10",
		description: "إرجاع لوغاريتم ذي أساس 10 لعدد مركب.",
		arguments: [
			{
				name: "inumber",
				description: "هو العدد المركب الذي تريد إيجاد اللوغاريتم المشترك الخاص به"
			}
		]
	},
	{
		name: "IMLOG2",
		description: "إرجاع لوغاريتم ذي أساس 2 لعدد مركب.",
		arguments: [
			{
				name: "inumber",
				description: " هو العدد المركب الذي تريد إيجاد اللوغاريتم ذي الأساس 2 الخاص به"
			}
		]
	},
	{
		name: "IMPOWER",
		description: "إرجاع عدد مركب تم رفعة لأس عدد صحيح.",
		arguments: [
			{
				name: "inumber",
				description: "هو عدد مركب ترغب في رفعه لأس"
			},
			{
				name: "number",
				description: "هو الأس الذي ترغب في رفع العدد المركب إليه"
			}
		]
	},
	{
		name: "IMPRODUCT",
		description: "إرجاع ناتج من 1 إلى 255 عدد مركب.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "inumber1",
				description: "Inumber1, Inumber2,... من 1 إلى 255 عدد مركب سيتم ضربها."
			},
			{
				name: "inumber2",
				description: "Inumber1, Inumber2,... من 1 إلى 255 عدد مركب سيتم ضربها."
			}
		]
	},
	{
		name: "IMREAL",
		description: "إرجاع المعامل الحقيقي لعدد مركب.",
		arguments: [
			{
				name: "inumber",
				description: "هو العدد المركب الذي ترغب في إيجاد المعامل الحقيقي الخاص به"
			}
		]
	},
	{
		name: "IMSEC",
		description: "إرجاع قاطع المنحنى لعدد مركب.",
		arguments: [
			{
				name: "inumber",
				description: "العدد المركب الذي تريد إيجاد قاطع المنحنى الخاص به"
			}
		]
	},
	{
		name: "IMSECH",
		description: "إرجاع قاطع المنحنى الزائدي لعدد مركب.",
		arguments: [
			{
				name: "inumber",
				description: "العدد المركب الذي تريد إيجاد قاطع المنحنى الزائدي الخاص به"
			}
		]
	},
	{
		name: "IMSIN",
		description: "إرجاع الجيب الخاص بعدد مركب.",
		arguments: [
			{
				name: "inumber",
				description: "هو العدد المركب الذي ترغب في إيجاد الجيب الخاص به"
			}
		]
	},
	{
		name: "IMSINH",
		description: "إرجاع جيب الزاوية الزائدي لعدد مركب.",
		arguments: [
			{
				name: "inumber",
				description: "العدد المركب الذي تريد إيجاد جيب الزاوية الزائدي له"
			}
		]
	},
	{
		name: "IMSQRT",
		description: "إرجاع الجذر التربيعي لعدد مركب.",
		arguments: [
			{
				name: "inumber",
				description: "هو عدد مركب ترغب في إيجاد الجذر التربيعي الخاص به"
			}
		]
	},
	{
		name: "IMSUB",
		description: "إرجاع الفرق بين عددين مركبين.",
		arguments: [
			{
				name: "inumber1",
				description: "هو العدد المركب الذي سيتم طرح inumber2 منه"
			},
			{
				name: "inumber2",
				description: "هو العدد المركب الذي سيتم طرح inumber1 منه"
			}
		]
	},
	{
		name: "IMSUM",
		description: "إرجاع مجموع الأعداد المركبة.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "inumber1",
				description: "تبدأ من 1 إلى 255 عدد مركب يتم إضافتها"
			},
			{
				name: "inumber2",
				description: "تبدأ من 1 إلى 255 عدد مركب يتم إضافتها"
			}
		]
	},
	{
		name: "IMTAN",
		description: "إرجاع ظل الزاوية لعدد مركب.",
		arguments: [
			{
				name: "inumber",
				description: "العدد المركب الذي تريد إيجاد ظل الزاوية الخاص به"
			}
		]
	},
	{
		name: "INDEX",
		description: "إرجاع قيمة الخلية أو مرجعها عند نقطة تقاطع صف مع عمود معين بزاوية محددة.",
		arguments: [
			{
				name: "array",
				description: "نطاق من الخلايا أو أية قيمة ثابتة لصفيف."
			},
			{
				name: "row_num",
				description: "حدد الصف في صفيف Array أو مرجع Reference الذي ستعاد قيمة منه. إذا أهمل، سيكون Column_num مطلوباً"
			},
			{
				name: "column_num",
				description: "تحديد العمود في Array أو Reference الذي ستعاد قيمة منه. إذا أهمل، سيكون Row_num مطلوباً"
			}
		]
	},
	{
		name: "INDIRECT",
		description: "إرجاع المرجع المحدد بسلسلة نصية.",
		arguments: [
			{
				name: "ref_text",
				description: "مرجع لخلية يحتوي على نمط المرجع A1 أو نمط المرجع R1C1، أو يحتوي على اسم معرف كمرجع، أو يحتوي على سلسلة نصية كمرجع لخلية"
			},
			{
				name: "a1",
				description: "قيمة منطقية تحدد نوع المرجع في Ref_text: R1C1-style = FALSE؛ A1-style = TRUE أو مهمل"
			}
		]
	},
	{
		name: "INFO",
		description: "إرجاع معلومات حول بيئة التشغيل الحالية.",
		arguments: [
			{
				name: "type_text",
				description: "نص يحدد نوع البيانات التي تريد إرجاعها."
			}
		]
	},
	{
		name: "INT",
		description: "تقريب رقم للأدنى إلى أقرب عدد صحيح.",
		arguments: [
			{
				name: "number",
				description: "الرقم الحقيقي الذي تريد تقريبه إلى الرقم الصحيح الأدنى"
			}
		]
	},
	{
		name: "INTERCEPT",
		description: "حساب النقطة التي يتقاطع خط مع محور ص فيها باستخدام أقرب خط انحدار مرسوم بواسطة قيم س وقيم ص المعروفة.",
		arguments: [
			{
				name: "known_y's",
				description: "المجموعة التابعة من المشاهدات أو البيانات ويمكن أن تكون أرقاماً أو أسماء، أو صفائف، أو مراجع تحتوي على أرقام"
			},
			{
				name: "known_x's",
				description: "المجموعة المستقلة من المشاهدات أو البيانات ويمكن أن تكون أرقاماً أو أسماء، أو صفائف، أو مراجع تحتوي على أرقام"
			}
		]
	},
	{
		name: "INTRATE",
		description: "إرجاع نسبة الفائدة لورقة مالية تم استثمارها بشكل كامل.",
		arguments: [
			{
				name: "settlement",
				description: "هو تاريخ تسوية الأوراق المالية، يتم التعبير عنه كرقم متسلسل للتاريخ"
			},
			{
				name: "maturity",
				description: "هو تاريخ استحقاق الأوراق المالية، يتم التعبير عنه كرقم متسلسل للتاريخ"
			},
			{
				name: "investment",
				description: "هو مقدار المبلغ الذي تم استثماره في الأوراق المالية"
			},
			{
				name: "redemption",
				description: "هو مقدار المبلغ الذي سيتم تلقيه عند تاريخ الاستحقاق"
			},
			{
				name: "basis",
				description: "هو نوع الأساس المستخدم لحساب الأيام"
			}
		]
	},
	{
		name: "IPMT",
		description: "إرجاع دفعات الفائدة لفترة استثمار محددة، بالاستناد إلى دفعات دورية وثابتة وإلى نسبة فائدة ثابتة.",
		arguments: [
			{
				name: "rate",
				description: "نسبة الفائدة لكل فترة. على سبيل المثال، استخدم 6%/4 للدفعات الربعية في 6% APR"
			},
			{
				name: "per",
				description: "الفترة التي تريد خلالها معرفة الفائدة ويجب أن تكون ضمن النطاق من 1 إلى Nper"
			},
			{
				name: "nper",
				description: "العدد الإجمالي لفترات الدفع في الاستثمار"
			},
			{
				name: "pv",
				description: "القيمة الحالية أو مقدار المبلغ الإجمالي الذي تساويه سلسلة الدفعات المستقبلية الآن"
			},
			{
				name: "fv",
				description: "القيمة المستقبلية أو الرصيد النقدي الذي تريد تحققه بعد إتمام الدفعة الأخيرة. إذا تم الحذف، Fv = 0‎"
			},
			{
				name: "type",
				description: "قيمة منطقية تمثل توقيت الدفعات: في نهاية الفترة = 0 أو محذوفة، في بداية الفترة = 1"
			}
		]
	},
	{
		name: "IRR",
		description: "إرجاع نسبة الإرجاع الداخلية للدفعات النقدية.",
		arguments: [
			{
				name: "values",
				description: "صفيف أو مرجع إلى الخلايا يحتوي على أرقام  تريد حساب نسبة الإرجاع الداخلية لها"
			},
			{
				name: "guess",
				description: "الرقم الذي تتوقعه قريباً من نتيجة IRR؛ 0.1 (10 بالمائة) إذا أهمل"
			}
		]
	},
	{
		name: "ISBLANK",
		description: "التحقق من أن المرجع يشير إلى خلية فارغة، وإرجاع TRUE أو FALSE.",
		arguments: [
			{
				name: "value",
				description: "الخلية أو اسم يشير إلى الخلية التي تريد اختبارها"
			}
		]
	},
	{
		name: "ISERR",
		description: "التحقق أن القيمة هي أي قيمة خطأ (#VALUE! أو #REF! أو #DIV/0! أو #NUM! أو #NAME؟ أو #NULL!) باستثناء #N/A، وإرجاع TRUE أو FALSE.",
		arguments: [
			{
				name: "value",
				description: "هي القيمة التي تريد اختبارها. يمكن أن تشير القيمة إلى خلية أو صيغة أو اسم يشير إلى خلية أو صيغة أو قيمة"
			}
		]
	},
	{
		name: "ISERROR",
		description: "التحقق ما إذا كانت القيمة خطأ (?N/A, #VALUE!, #REF!, #DIV/0!, #NUM!, #NAME# أو #NULL!)، وإرجاع القيمة TRUE أو FALSE.",
		arguments: [
			{
				name: "value",
				description: "هي القيمة التي تريد اختبارها. يمكن أن تشير القيمة إلى خلية أو صيغة أو اسم يشير إلى خلية أو صيغة أو قيمة"
			}
		]
	},
	{
		name: "ISEVEN",
		description: "إرجاع القيمة TRUE إذا كان الرقم زوجياً.",
		arguments: [
			{
				name: "number",
				description: " هي القيمة التي سيتم اختبارها"
			}
		]
	},
	{
		name: "ISFORMULA",
		description: "التحقق مما إذا كان المرجع لخلية تحتوي على صيغة ويتم إرجاع TRUE أو FALSE.",
		arguments: [
			{
				name: "reference",
				description: "مرجع للخلية التي تريد اختبارها. يمكن أن يكون Reference مرجع خلية أو صيغة أو اسماً يشير إلى خلية"
			}
		]
	},
	{
		name: "ISLOGICAL",
		description: "التحقق من أن القيمة قيمة منطقية (TRUE أو FALSE) وإرجاع TRUE أو FALSE.",
		arguments: [
			{
				name: "value",
				description: "هي القيمة التي تريد اختبارها. يمكن أن تشير القيمة إلى خلية، أو إلى صيغة، أو إلى اسم يشير إلى خلية، أو إلى صيغة، أو إلى قيمة"
			}
		]
	},
	{
		name: "ISNA",
		description: "التأكد من أن القيمة هي ‎#N/A ومن إرجاع القيمة TRUE أو FALSE.",
		arguments: [
			{
				name: "value",
				description: " كقيمة تريد اختبارها. يمكن أن تشير 'قيمة' إلى خلية، أو إلى صيغة، أو إلى اسم يشير إلى خلية، أو إلى صيغة، أو إلى قيمة"
			}
		]
	},
	{
		name: "ISNONTEXT",
		description: "التحقق من أن القيمة ليست نصاً ( الخلايا الفارغة ليست نصاً)، وإرجاع TRUE أو FALSE.",
		arguments: [
			{
				name: "value",
				description: " هي القيمة التي تريد اختبارها: هي خلية؛ أو صيغة؛ أو اسم يشير إلى خلية ،أو إلى صيغة، أو إلى قيمة"
			}
		]
	},
	{
		name: "ISNUMBER",
		description: "التحقق من أن القيمة تعد رقماً وإرجاع TRUE أو FALSE.",
		arguments: [
			{
				name: "value",
				description: "هي القيمة التي تريد اختبارها. يمكن أن تشير القيمة إلى خلية، أو إلى صيغة، أو إلى اسم يشير إلى خلية، أو إلى صيغة، أو إلى قيمة"
			}
		]
	},
	{
		name: "ISO.CEILING",
		description: "تقريب رقم إلى الأعلى أو إلى أقرب عدد صحيح أو إلى أقرب مضاعف من مضاعفات significance.",
		arguments: [
			{
				name: "number",
				description: "القيمة التي تريد تقريبها"
			},
			{
				name: "significance",
				description: "المضاعف الاختياري الذي تريد التقريب له"
			}
		]
	},
	{
		name: "ISODD",
		description: "إرجاع القيمة TRUE إذا كان الرقم فردياً.",
		arguments: [
			{
				name: "number",
				description: " هي القيمة التي سيتم اختبارها"
			}
		]
	},
	{
		name: "ISOWEEKNUM",
		description: "إرجاع رقم أسبوع ISO من العام لتاريخ محدد.",
		arguments: [
			{
				name: "date",
				description: "التعليمات البرمجية الخاصة بالتاريخ والوقت المستخدمة بواسطة Spreadsheet لحساب التاريخ والوقت"
			}
		]
	},
	{
		name: "ISPMT",
		description: "إرجاع دفعات الفائدة المدفوعة أثناء فترة استثمار معينة.",
		arguments: [
			{
				name: "rate",
				description: "نسبة الفائدة حسب الفترة. على سبيل المثال، استخدام 6%/4 للدفعات الموسمية في 6% APR"
			},
			{
				name: "per",
				description: "الفترة التي تريد خلالها معرفة الفائدة"
			},
			{
				name: "nper",
				description: "فترات الدفع في الاستثمار"
			},
			{
				name: "pv",
				description: "إجمالي المبلغ الذي تساويه سلسلة الدفعات المستقبلية الآن"
			}
		]
	},
	{
		name: "ISREF",
		description: "التحقق من مرجعية القيمة وإرجاع TRUE أو FALSE.",
		arguments: [
			{
				name: "value",
				description: "القيمة التي تريد اختبارها. يمكن أن تشير القيمة Value إلى خلية، أو إلى صيغة، أو إلى اسم يشير إلى خلية، أو إلى صيغة، أو إلى قيمة"
			}
		]
	},
	{
		name: "ISTEXT",
		description: "التحقق من أن القيمة تعد نصاً وإرجاع TRUE أو FALSE.",
		arguments: [
			{
				name: "value",
				description: "هي القيمة التي تريد اختبارها. يمكن أن تشير القيمة إلى خلية، أو إلى صيغة، أو إلى اسم يشير إلى خلية، أو إلى صيغة، أو إلى قيمة"
			}
		]
	},
	{
		name: "KURT",
		description: "إرجاع تفرطح مجموعة البيانات.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "من 1 إلى 255 رقماً أو اسماً أو صفيفاً أو مرجعاً يحتوي على أرقام  والتي تريد تفرطحها"
			},
			{
				name: "number2",
				description: "من 1 إلى 255 رقماً أو اسماً أو صفيفاً أو مرجعاً يحتوي على أرقام  والتي تريد تفرطحها"
			}
		]
	},
	{
		name: "LARGE",
		description: "إرجاع ترتيب القيم الكبرى في مجموعة البيانات. على سبيل المثال، خامس أكبر رقم.",
		arguments: [
			{
				name: "array",
				description: "الصفيف أو نطاق البيانات الذي تريد تحديد ترتيب القيم الكبرى فيه"
			},
			{
				name: "k",
				description: "موضع القيمة التي سترجع  (اعتباراً من الأكبر) في الصفيف أو في نطاق الخلايا"
			}
		]
	},
	{
		name: "LCM",
		description: "إرجاع أقل مضاعف مشترك.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "هي القيم من 1 إلى 255 التي تريد الحصول على أقل مضاعف مشترك لها"
			},
			{
				name: "number2",
				description: "هي القيم من 1 إلى 255 التي تريد الحصول على أقل مضاعف مشترك لها"
			}
		]
	},
	{
		name: "LEFT",
		description: "إرجاع عدد الأحرف المحدد في بداية سلسلة نصية.",
		arguments: [
			{
				name: "text",
				description: "هي السلسلة النصية التي تحتوي على الأحرف المراد استخراجها"
			},
			{
				name: "num_chars",
				description: "تحديد عدد الأحرف التي ستستخرجها LEFT؛ 1 إذا أهمل"
			}
		]
	},
	{
		name: "LEN",
		description: "إرجاع عدد الأحرف في السلسلة النصية.",
		arguments: [
			{
				name: "text",
				description: "النص الذي تريد معرفة طوله. تعد الفراغات كأحرف"
			}
		]
	},
	{
		name: "LINEST",
		description: "إرجاع الإحصاءات التي تعبر عن الاتجاه الخطي المطابق لنقاط البيانات المعروفة، وذلك بمطابقة الخط المستقيم باستخدام طريقة المربعات الصغرى.",
		arguments: [
			{
				name: "known_y's",
				description: "مجموعة قيم ص والتي تعرفها مسبقاً من العلاقة y = mx +b"
			},
			{
				name: "known_x's",
				description: "مجموعة اختيارية لقيم س والتي تعرفها مسبقاً من العلاقة y = mx + b"
			},
			{
				name: "const",
				description: "قيمة منطقية: العدد الثابت b يحسب بشكل عادي إذا كان Const = TRUE أو مهملاً؛ ويعين b مساوياً للصفر إذا كان Const = FALSE"
			},
			{
				name: "stats",
				description: "قيمة منطقية: إرجاع إحصاءات انحدار إضافية = TRUE؛ إرجاع معاملات m والعدد الثابت b = FALSE أو مهمل"
			}
		]
	},
	{
		name: "LN",
		description: "إرجاع اللوغاريتم الطبيعي لرقم.",
		arguments: [
			{
				name: "number",
				description: "هو الرقم الحقيقي الموجب الذي تريد معرفة اللوغاريتم الطبيعي له"
			}
		]
	},
	{
		name: "LOG",
		description: "إرجاع لوغاريتم رقم قم بتحديد أساسه.",
		arguments: [
			{
				name: "number",
				description: "الرقم الحقيقي الموجب الذي تريد معرفة لوغاريتمه"
			},
			{
				name: "base",
				description: "أساس اللوغاريتم؛ 10 إذا أهمل"
			}
		]
	},
	{
		name: "LOG10",
		description: "إرجاع اللوغاريتم العشري لرقم.",
		arguments: [
			{
				name: "number",
				description: "هو الرقم الحقيقي الموجب الذي تريد معرفة لوغاريتمه العشري"
			}
		]
	},
	{
		name: "LOGEST",
		description: "إرجاع الإحصاءات التي تعبر عن المنحنى الأسي المطابق لنقاط البيانات المعروفة.",
		arguments: [
			{
				name: "known_y's",
				description: "مجموعة قيم ص والتي تعرفها مسبقاً من العلاقة y = b*m^x"
			},
			{
				name: "known_x's",
				description: "مجموعة اختيارية لقيم س والتي قد تعرفها مسبقاً من العلاقة y = b*m^x"
			},
			{
				name: "const",
				description: "قيمة منطقية: يحسب العدد الثابت b بشكل اعتيادي إذا كان Const = TRUE أو مهملاً؛ سيعين b مساوياً لـ 1 إذا كان Const = FALSE"
			},
			{
				name: "stats",
				description: "قيمة منطقية: إرجاع إحصاءات انحدار إضافية = TRUE؛ إرجاع معاملات m والعدد الثابت b = FALSE أو مهمل"
			}
		]
	},
	{
		name: "LOGINV",
		description: "إرجاع عكس دالة التوزيع اللوغاريتمي الطبيعي التراكمي لـ x، حيث يتم توزيع ln(x)‎ بشكل طبيعي باستخدام المعلمتين Mean وStandard_dev.",
		arguments: [
			{
				name: "probability",
				description: "احتمال مقترن بالتوزيع اللوغاريتمي الطبيعي، رقم بين 0 و1، ضمنًا"
			},
			{
				name: "mean",
				description: "وسط ln(x)‎"
			},
			{
				name: "standard_dev",
				description: "الانحراف المعياري لـ ln(x)، رقم موجب"
			}
		]
	},
	{
		name: "LOGNORM.DIST",
		description: "إرجاع التوزيع اللوغاريتمي الطبيعي لـ x، حيث يتم توزيع ln(x)‎ بشكل طبيعي باستخدام المعلمتين Mean وStandard_dev.",
		arguments: [
			{
				name: "x",
				description: "القيمة التي تريد تقييم الدالة عندها، رقم موجب"
			},
			{
				name: "mean",
				description: "وسط ln(x)‎"
			},
			{
				name: "standard_dev",
				description: "الانحراف المعياري لـ ln(x)‎، رقم موجب"
			},
			{
				name: "cumulative",
				description: "قيمة منطقية: من أجل دالة التوزيع التراكمي، استخدم TRUE؛ من أجل دالة كثافة الاحتمال، استخدم FALSE"
			}
		]
	},
	{
		name: "LOGNORM.INV",
		description: "إرجاع عكس دالة التوزيع اللوغاريتمي الطبيعي التراكمي لـ x، حيث يتم توزيع ln(x)‎ بشكل طبيعي باستخدام المعلمتين Mean وStandard_dev.",
		arguments: [
			{
				name: "probability",
				description: "احتمال مقترن بالتوزيع اللوغاريتمي الطبيعي، رقم بين 0 و1، ضمنًا"
			},
			{
				name: "mean",
				description: "وسط ln(x)‎"
			},
			{
				name: "standard_dev",
				description: "الانحراف المعياري لـ ln(x)، رقم موجب"
			}
		]
	},
	{
		name: "LOGNORMDIST",
		description: "إرجاع التوزيع اللوغاريتمي الطبيعي التراكمي لـ x، وذلك عند التوزيع الطبيعي لـ  ln(x)‎ بالمعلمتين Mean و Standard_dev.",
		arguments: [
			{
				name: "x",
				description: "القيمة التي تريد تقييم الدالة عندها، رقم موجب"
			},
			{
				name: "mean",
				description: "وسط ln(x)‎"
			},
			{
				name: "standard_dev",
				description: "الانحراف المعياري لـ ln(x)‎، رقم موجب"
			}
		]
	},
	{
		name: "LOOKUP",
		description: "البحث عن قيمة إما من نطاق صف واحد أو من نطاق عمود واحد أو من صفيف. يتم توفيره للتوافق العكسي.",
		arguments: [
			{
				name: "lookup_value",
				description: "القيمة التي يبحث عنها LOOKUP في Look_vector والتي يمكن أن تكون رقماً أو نصاً أو قيمة منطقية أو اسم أو مرجع لقيمة"
			},
			{
				name: "lookup_vector",
				description: "نطاق يحتوي على صف واحد أو عمود واحد لنص أو أرقام أو قيم منطقية مرتبة تصاعدياً"
			},
			{
				name: "result_vector",
				description: "نطاق يحتوي على صف واحد أو عمود واحد فقط وله نفس حجم Look_vector"
			}
		]
	},
	{
		name: "LOWER",
		description: "تحويل كافة أحرف سلسلة نصية إلى الأحرف الصغيرة.",
		arguments: [
			{
				name: "text",
				description: "النص الذي تريد تحويله إلى أحرف صغيرة. لن يتم تغيير الرموز التي ليست أحرفاً في النص"
			}
		]
	},
	{
		name: "MATCH",
		description: "إرجاع الموضع النسبي لعنصر في الصفيف يطابق قيمة محددة بترتيب محدد.",
		arguments: [
			{
				name: "lookup_value",
				description: "القيمة التي تستخدمها للعثور على قيمة تريدها في الصفيف، قد تكون رقماً، أو نصاً، أو قيمة منطقية، أو مرجعاً لأحدها"
			},
			{
				name: "lookup_array",
				description: "نطاق من الخلايا المتقاربة، أو صفيف من القيم، أو مرجع إلى صفيف يحتوي على قيم بحث محتملة"
			},
			{
				name: "match_type",
				description: "رقم 1 أو 0 أو -1 يشير إلى القيمة التي سترجع."
			}
		]
	},
	{
		name: "MAX",
		description: "إرجاع أكبر قيمة موجودة في مجموعة من القيم. يتم تجاهل القيم والنصوص المنطقية.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "‎ من 1 إلى 255 رقماً أو خلية فارغة أو قيمة منطقية أو رقم نصي تريد الحصول على الحد الأقصى لها"
			},
			{
				name: "number2",
				description: "‎ من 1 إلى 255 رقماً أو خلية فارغة أو قيمة منطقية أو رقم نصي تريد الحصول على الحد الأقصى لها"
			}
		]
	},
	{
		name: "MAXA",
		description: "إرجاع القيمة العظمى في مجموعة من القيم. لا يتم تجاهل القيم المنطقية و النصوص.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "‎ من 1 إلى 255 رقماً أو خلية فارغة أو قيمة منطقية أو رقم نصي تريد الحصول على الحد الأقصى لها"
			},
			{
				name: "value2",
				description: "‎ من 1 إلى 255 رقماً أو خلية فارغة أو قيمة منطقية أو رقم نصي تريد الحصول على الحد الأقصى لها"
			}
		]
	},
	{
		name: "MDETERM",
		description: "إرجاع محدد التنظيمة لصفيف.",
		arguments: [
			{
				name: "array",
				description: "صفيف رقمي له عدد متساوٍ من الصفوف والأعمدة، إما نطاق من الخلايا أو ثابت صفيف"
			}
		]
	},
	{
		name: "MEDIAN",
		description: "إرجاع الوسيط أو الرقم الموجود في منتصف مجموعة من الأرقام المحددة.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "‎ من 1 إلى 255 رقماً أو اسماً أو صفيفاً أو مرجع يحتوي على الأرقام تريد الحصول على الوسيط الخاص بها"
			},
			{
				name: "number2",
				description: "‎ من 1 إلى 255 رقماً أو اسماً أو صفيفاً أو مرجع يحتوي على الأرقام تريد الحصول على الوسيط الخاص بها"
			}
		]
	},
	{
		name: "MID",
		description: "إرجاع أحرف من وسط سلسلة نصية، بداية من موضع وطول محددين.",
		arguments: [
			{
				name: "text",
				description: "السلسلة النصية التي تريد استخراج الحروف منها"
			},
			{
				name: "start_num",
				description: "موضع الحرف الأول الذي تريد استخراجه. الحرف الأول في النص (Text) هو 1"
			},
			{
				name: "num_chars",
				description: "تعيين عدد الأحرف لإرجاعها من Text"
			}
		]
	},
	{
		name: "MIN",
		description: "إرجاع أصغر قيمة موجودة في مجموعة من القيم. يتم تجاهل القيم والنصوص المنطقية.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "‎ من 1 إلى 255 رقماً أو خلية فارغة أو قيمة منطقية أو رقم نصي تريد الحصول على الحد الأدنى لها"
			},
			{
				name: "number2",
				description: "‎ من 1 إلى 255 رقماً أو خلية فارغة أو قيمة منطقية أو رقم نصي تريد الحصول على الحد الأدنى لها"
			}
		]
	},
	{
		name: "MINA",
		description: "إرجاع القيمة الدنيا في مجموعة من القيم. لا يتم تجاهل القيم المنطقية و النصوص.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "‎ من 1 إلى 255 رقماً أو خلية فارغة أو قيمة منطقية أو رقم نصي تريد الحصول على الحد الأدنى لها"
			},
			{
				name: "value2",
				description: "‎ من 1 إلى 255 رقماً أو خلية فارغة أو قيمة منطقية أو رقم نصي تريد الحصول على الحد الأدنى لها"
			}
		]
	},
	{
		name: "MINUTE",
		description: "إرجاع الدقيقة، وهي عبارة عن رقم من 0 إلى 59.",
		arguments: [
			{
				name: "serial_number",
				description: "رقم في التعليمات البرمجية الخاصة بالتاريخ والوقت المستخدم من قبل Spreadsheet أو نص بتنسيق وقتي، مثال 16:48:00 أو 4:48:00 م"
			}
		]
	},
	{
		name: "MINVERSE",
		description: "إرجاع مقلوب التنظيم غير المنفردة للتنظيمة المخزنة في صفيف.",
		arguments: [
			{
				name: "array",
				description: "صفيف رقمي له عدد متساوٍ من الصفوف والأعمدة، إما نطاق من الخلايا أو ثابت صفيف"
			}
		]
	},
	{
		name: "MIRR",
		description: "إرجاع نسبة الإرجاع الداخلية للدفعات النقدية الدورية، مع الأخذ في الاعتبار تكلفة الاستثمار والفائدة عند إعادة استثمار النقد.",
		arguments: [
			{
				name: "values",
				description: "صفيف أو مرجع إلى خلايا تحتوي أرقام تمثل الدفعات (سالب) و الإيرادات (موجب) في فترات منتظمة"
			},
			{
				name: "finance_rate",
				description: "نسبة الفائدة التي تدفعها على الأموال المستخدمة في الدفعات النقدية"
			},
			{
				name: "reinvest_rate",
				description: "نسبة الفائدة التي تستلمها على الدفعات النقدية عند إعادة استثمارها"
			}
		]
	},
	{
		name: "MMULT",
		description: "إرجاع المصفوفة الناتجة عن ضرب صفيفين، صفيف عدد صفوفه مساو لعدد صفوف array1 وعدد أعمدته مساو لعدد أعمدة array2‏.",
		arguments: [
			{
				name: "array1",
				description: "الصفيف الأول من الأرقام المراد ضربها ويجب أن يكون عدد أعمدتها مساوياً لعدد صفوف Array2‎"
			},
			{
				name: "array2",
				description: "الصفيف الأول من الأرقام المراد ضربها ويجب أن يكون عدد أعمدتها مساوياً لعدد صفوف Array2‎"
			}
		]
	},
	{
		name: "MOD",
		description: "إرجاع باقي قسمة رقم على المقسوم عليه.",
		arguments: [
			{
				name: "number",
				description: "هو الرقم الذي تريد معرفة الباقي منه بعد إجراء عملية القسمة"
			},
			{
				name: "divisor",
				description: "هو الرقم الذي تريد قسمة 'رقم' عليه"
			}
		]
	},
	{
		name: "MODE",
		description: "إرجاع القيمة الأكثر تكرارًا أو الأكثر ظهورًا في صفيف أو في نطاق من البيانات.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "‎من 1 إلى 255 رقمًا أو اسمًا أو صفيفًا أو مرجعًا يحتوي على أرقام تريد الحصول على المنوال الخاص بها"
			},
			{
				name: "number2",
				description: "‎من 1 إلى 255 رقمًا أو اسمًا أو صفيفًا أو مرجعًا يحتوي على أرقام تريد الحصول على المنوال الخاص بها"
			}
		]
	},
	{
		name: "MODE.MULT",
		description: "إرجاع صفيف عمودي بالقيم الأكثر تكراراً أو الأكثر ظهوراً في صفيف أو نطاق بيانات. من أجل صفيف أفقي، استخدم =TRANSPOSE(MODE.MULT(number1,number2,...)).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "من 1 إلى 255 رقماً أو اسماً أو صفيفاً أو مرجعاً يحتوي على أرقام تريد الحصول على المنوال الخاص بها"
			},
			{
				name: "number2",
				description: "من 1 إلى 255 رقماً أو اسماً أو صفيفاً أو مرجعاً يحتوي على أرقام تريد الحصول على المنوال الخاص بها"
			}
		]
	},
	{
		name: "MODE.SNGL",
		description: "إرجاع القيمة الأكثر تكراراً أو الأكثر ظهوراً في صفيف أو في نطاق من البيانات.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "‎من 1 إلى 255 رقمًا أو اسماً أو صفيفاً أو مرجعاً يحتوي على أرقام تريد الحصول على المنوال الخاص بها"
			},
			{
				name: "number2",
				description: "‎من 1 إلى 255 رقمًا أو اسماً أو صفيفاً أو مرجعاً يحتوي على أرقام تريد الحصول على المنوال الخاص بها"
			}
		]
	},
	{
		name: "MONTH",
		description: "إرجاع الشهر، وهو عبارة عن رقم من 1 (يناير) إلى 12 (ديسمبر).",
		arguments: [
			{
				name: "serial_number",
				description: "رقم في التعليمات البرمجية الخاصة بالتاريخ والوقت المستخدم من قبل Spreadsheet"
			}
		]
	},
	{
		name: "MROUND",
		description: "إرجاع رقم مقرب إلى المضاعف المطلوب.",
		arguments: [
			{
				name: "number",
				description: "هي القيمة التي سيتم تقريبها"
			},
			{
				name: "multiple",
				description: "هو المضاعف الذي ترغب في تقريب الرقم إليه"
			}
		]
	},
	{
		name: "MULTINOMIAL",
		description: "إرجاع التسمية المتعددة لمجموعة من الأرقام.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "هي القيم من 1 إلى 255 التي تريد الحصول على التسمية المتعددة لها"
			},
			{
				name: "number2",
				description: "هي القيم من 1 إلى 255 التي تريد الحصول على التسمية المتعددة لها"
			}
		]
	},
	{
		name: "MUNIT",
		description: "إرجاع مصفوفة الوحدة للبعد المحدد.",
		arguments: [
			{
				name: "dimension",
				description: "عدد صحيح يحدد بُعد مصفوفة الوحدة التي تريد إرجاعها"
			}
		]
	},
	{
		name: "N",
		description: "تحويل القيم غير الرقمية إلى أرقام والتواريخ إلى أرقام تسلسلية و TRUE إلى 1، وغير ذلك إلى 0 (صفر).",
		arguments: [
			{
				name: "value",
				description: "القيمة التي تريد تحويلها"
			}
		]
	},
	{
		name: "NA",
		description: "إرجاع قيمة الخطأ ‎#N/A (القيمة غير متوفرة).",
		arguments: [
		]
	},
	{
		name: "NEGBINOM.DIST",
		description: "إرجاع التوزيع السالب ذي الحدين، احتمال وجود مرات فشل عددها Number_f قبل النجاح رقم Number_s، مع احتمال Probability_s للنجاح.",
		arguments: [
			{
				name: "number_f",
				description: "عدد مرات الفشل"
			},
			{
				name: "number_s",
				description: "رقم يشير إلى عتبة محاولات النجاح"
			},
			{
				name: "probability_s",
				description: "احتمال النجاح؛ رقم بين 0 و1"
			},
			{
				name: "cumulative",
				description: "قيمة منطقية: من أجل دالة التوزيع التراكمي، استخدم TRUE؛ من أجل دالة الاحتمالات غير التراكمية، استخدم FALSE"
			}
		]
	},
	{
		name: "NEGBINOMDIST",
		description: "إرجاع التوزيع السالب ذي الحدين، احتمال وجود مرات فشل عددها Number_f قبل النجاح رقم Number_s، مع احتمال Probability_s للنجاح.",
		arguments: [
			{
				name: "number_f",
				description: "عدد مرات الفشل"
			},
			{
				name: "number_s",
				description: "رقم يشير إلى عتبة محاولات النجاح"
			},
			{
				name: "probability_s",
				description: "احتمال النجاح؛ رقم بين 0 و1"
			}
		]
	},
	{
		name: "NETWORKDAYS",
		description: "إرجاع عدد أيام العمل الكاملة بين تاريخين.",
		arguments: [
			{
				name: "start_date",
				description: "هو رقم متسلسل للتاريخ يمثل تاريخ البدء"
			},
			{
				name: "end_date",
				description: "هو رقم متسلسل للتاريخ يمثل تاريخ الانتهاء"
			},
			{
				name: "holidays",
				description: "هي مجموعة اختيارية تتكون من رقم متسلسل للتاريخ واحد فقط أو أكثر يتم استبعادها من تقويم العمل، مثل العطلات الرسمية والأعياد التي يتغير توقيتها كل سنة"
			}
		]
	},
	{
		name: "NETWORKDAYS.INTL",
		description: "إرجاع عدد أيام العمل الكاملة بين تاريخين باستخدام معلمات مخصصة للعطلة الأسبوعية.",
		arguments: [
			{
				name: "start_date",
				description: "رقم تسلسلي للتاريخ يمثل تاريخ البدء"
			},
			{
				name: "end_date",
				description: "رقم تسلسلي للتاريخ يمثل تاريخ الانتهاء"
			},
			{
				name: "weekend",
				description: "عدد أو سلسلة تحدد وقت حدوث العطلات الأسبوعية"
			},
			{
				name: "holidays",
				description: "مجموعة اختيارية تتكون من رقم تسلسلي واحد أو أكثر للتاريخ يتم استبعادها من تقويم العمل، مثل العطلات الرسمية والأعياد التي يتغير توقيتها كل سنة"
			}
		]
	},
	{
		name: "NOMINAL",
		description: "إرجاع النسبة الاسمية السنوية للفائدة.",
		arguments: [
			{
				name: "effect_rate",
				description: "هي نسبة الفائدة الفعلية"
			},
			{
				name: "npery",
				description: "هو عدد الفترات المتراكبة كل سنة"
			}
		]
	},
	{
		name: "NORM.DIST",
		description: "إرجاع التوزيع العادي للوسط المحدد وإرجاع الانحراف المعياري.",
		arguments: [
			{
				name: "x",
				description: "القيمة التي تريد التوزيع لها"
			},
			{
				name: "mean",
				description: "الوسط الحسابي للتوزيع"
			},
			{
				name: "standard_dev",
				description: "الانحراف المعياري للتوزيع، رقم موجب"
			},
			{
				name: "cumulative",
				description: "قيمة منطقية: من أجل دالة التوزيع التراكمي، استخدم TRUE؛ من أجل دالة كثافة الاحتمال، استخدم FALSE"
			}
		]
	},
	{
		name: "NORM.INV",
		description: "إرجاع عكس التوزيع التراكمي العادي للوسط المحدد وإرجاع الانحراف المعياري.",
		arguments: [
			{
				name: "probability",
				description: "احتمال متطابق مع التوزيع العادي، رقم بين 0 و1 ضمنًا"
			},
			{
				name: "mean",
				description: "الوسط الحسابي للتوزيع"
			},
			{
				name: "standard_dev",
				description: "الانحراف المعياري للتوزيع، رقم موجب"
			}
		]
	},
	{
		name: "NORM.S.DIST",
		description: "إرجاع التوزيع القياسي العادي (له وسط بقيمة صفر وانحراف معياري بقيمة واحد).",
		arguments: [
			{
				name: "z",
				description: "القيمة التي تريد التوزيع لها"
			},
			{
				name: "cumulative",
				description: "قيمة منطقية للدالة التي سيتم إرجاعها: دالة التوزيع التراكمي = TRUE؛ دالة كثافة الاحتمال = FALSE"
			}
		]
	},
	{
		name: "NORM.S.INV",
		description: "إرجاع عكس التوزيع التراكمي العادي القياسي (له وسط بقيمة صفر وانحراف معياري بقيمة واحد).",
		arguments: [
			{
				name: "probability",
				description: "احتمال متطابق مع التوزيع العادي، رقم بين 0 و1 ضمنًا"
			}
		]
	},
	{
		name: "NORMDIST",
		description: "إرجاع التوزيع العادي للوسط المحدد وإرجاع الانحراف المعياري.",
		arguments: [
			{
				name: "x",
				description: "القيمة التي تريد التوزيع لها"
			},
			{
				name: "mean",
				description: "الوسط الحسابي للتوزيع"
			},
			{
				name: "standard_dev",
				description: "الانحراف المعياري للتوزيع، رقم موجب"
			},
			{
				name: "cumulative",
				description: "قيمة منطقية: من أجل دالة التوزيع التراكمي، استخدم TRUE؛ من أجل دالة كثافة الاحتمال، استخدم FALSE"
			}
		]
	},
	{
		name: "NORMINV",
		description: "إرجاع عكس التوزيع التراكمي العادي للوسط المحدد وإرجاع الانحراف المعياري.",
		arguments: [
			{
				name: "probability",
				description: "احتمال متطابق مع التوزيع العادي، رقم بين 0 و1 ضمنًا"
			},
			{
				name: "mean",
				description: "الوسط الحسابي للتوزيع"
			},
			{
				name: "standard_dev",
				description: "الانحراف المعياري للتوزيع، رقم موجب"
			}
		]
	},
	{
		name: "NORMSDIST",
		description: "إرجاع التوزيع القياسي العادي (له وسط بقيمة صفر وانحراف معياري بقيمة واحد).",
		arguments: [
			{
				name: "z",
				description: "القيمة التي تريد التوزيع لها"
			}
		]
	},
	{
		name: "NORMSINV",
		description: "إرجاع عكس التوزيع التراكمي العادي القياسي (له وسط بقيمة الصفر وانحراف معياري بقيمة واحد).",
		arguments: [
			{
				name: "probability",
				description: "احتمال متطابق مع التوزيع العادي، رقم بين 0 و1 ضمنًا"
			}
		]
	},
	{
		name: "NOT",
		description: "تغيير FALSE إلى TRUE أو TRUE إلى FALSE.",
		arguments: [
			{
				name: "logical",
				description: "قيمة أو تعبير يمكن تقييمها إلى TRUE أو إلى FALSE"
			}
		]
	},
	{
		name: "NOW",
		description: "إرجاع التاريخ والوقت الحاليين بالتنسيق الوقت والتاريخ.",
		arguments: [
		]
	},
	{
		name: "NPER",
		description: "إرجاع عدد الفترات للاستثمار استنادا إلى دفعات ثابتة دورية وإلى نسبة فائدة ثابتة.",
		arguments: [
			{
				name: "rate",
				description: "نسبة الفائدة لكل فترة. على سبيل المثال، استخدام 6%/4 للدفعات الموسمية عند 6% APR"
			},
			{
				name: "pmt",
				description: "الدفعة خلال كل فترة؛ ولا يمكن أن يتغير خلال مدة الاستثمار"
			},
			{
				name: "pv",
				description: "القيمة الحالية، أو مقدار المبلغ الإجمالي الذي تساويه الدفعات المستقبلية المستحقة الآن"
			},
			{
				name: "fv",
				description: "القيمة المستقبلية، أو ميزانية نقدية تريد إحرازها بعد إتمام الدفعة الأخيرة. إذا أهملت، سيستخدم الصفر"
			},
			{
				name: "type",
				description: "قيمة منطقية: الدفع في بداية الفترة = 1؛ الدفع في نهاية الفترة = 0 أو مهمل"
			}
		]
	},
	{
		name: "NPV",
		description: "إرجاع القيمة الصافية الحالية لاستثمار بالاستناد إلى معدل الخصم وإلى الدفعات المستقبلية (قيم سالبة) وإلى الإيرادات (قيم موجبة).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "rate",
				description: "معدل الخصم على طول فترة واحدة‎"
			},
			{
				name: "value1",
				description: " من 1 إلى 254 دفعة وإيراد متساويين في الفاصل الزمني ويتم التحصيل في نهاية كل فترة"
			},
			{
				name: "value2",
				description: " من 1 إلى 254 دفعة وإيراد متساويين في الفاصل الزمني ويتم التحصيل في نهاية كل فترة"
			}
		]
	},
	{
		name: "NUMBERVALUE",
		description: "تحويل نص إلى رقم بطريقة لا تعتمد على الإعدادات المحلية.",
		arguments: [
			{
				name: "text",
				description: "السلسلة التي تمثل الرقم الذي تريد تحويله"
			},
			{
				name: "decimal_separator",
				description: "الحرف المستخدم كفاصل عشري في السلسلة"
			},
			{
				name: "group_separator",
				description: "الحرف المستخدم كفاصل مجموعة في السلسلة"
			}
		]
	},
	{
		name: "OCT2BIN",
		description: "تحويل رقم ثماني إلى رقم ثنائي.",
		arguments: [
			{
				name: "number",
				description: "هو الرقم الثماني الذي تريد تحويله"
			},
			{
				name: "places",
				description: "هو عدد الأحرف التي سيتم استخدامها"
			}
		]
	},
	{
		name: "OCT2DEC",
		description: "تحويل رقم ثماني إلى رقم عشري.",
		arguments: [
			{
				name: "number",
				description: "هو الرقم الثماني الذي تريد تحويله"
			}
		]
	},
	{
		name: "OCT2HEX",
		description: "تحويل رقم ثماني إلى رقم سداسي عشري.",
		arguments: [
			{
				name: "number",
				description: "هو الرقم الثماني الذي تريد تحويله"
			},
			{
				name: "places",
				description: "هو عدد الأحرف التي سيتم استخدامها"
			}
		]
	},
	{
		name: "ODD",
		description: "تقريب رقم موجب إلى الأعلى ورقم سالب إلى أدنى أقرب رقم صحيح فردي.",
		arguments: [
			{
				name: "number",
				description: "القيمة التي ستقرَب"
			}
		]
	},
	{
		name: "OFFSET",
		description: "إرجاع مرجع نطاق عبارة عن عدد صفوف وأعمدة محدد من مرجع محدد.",
		arguments: [
			{
				name: "reference",
				description: "المرجع الذي تريد أن تبدأ منه الإزاحة، مرجع لخلية أو مرجع لنطاق من الخلايا المتجاورة"
			},
			{
				name: "rows",
				description: "عدد الصفوف العلوية أو السفلية، التي تريد أن تشير الخلية العلوية اليسرى للنتيجة إليها"
			},
			{
				name: "cols",
				description: "عدد الأعمدة التي على اليسار أو على اليمين، التي تريد أن تشير الخلية العلوية اليسرى للنتيجة إليها"
			},
			{
				name: "height",
				description: "الارتفاع، بعدد الأسطر الذي تريده للنتيجة، إذا أهمل سيستخدم نفس الارتفاع كمرجع"
			},
			{
				name: "width",
				description: "العرض، بعدد الأعمدة، الذي تريده للنتيجة، إذا أهمل سيستخدم نفس العرض كمرجع"
			}
		]
	},
	{
		name: "OR",
		description: "التحقق من أن أية قيمة من قيم الوسيطات هي TRUE، ويتم إرجاع TRUE أو FALSE. إرجاع FALSE إذا كانت كافة الوسيطاتFALSE.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logical1",
				description: "من 1 إلى 255 شرطاً ترغب في اختبارها لمعرفة ما إذا كانت قيمة كل منها إما TRUE أو FALSE"
			},
			{
				name: "logical2",
				description: "من 1 إلى 255 شرطاً ترغب في اختبارها لمعرفة ما إذا كانت قيمة كل منها إما TRUE أو FALSE"
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
		description: "إرجاع عدد الفترات المطلوب بواسطة استثمار للوصول إلى قيمة محددة.",
		arguments: [
			{
				name: "rate",
				description: "نسبة الفائدة لكل فترة."
			},
			{
				name: "pv",
				description: "القيمة الحالية للاستثمار"
			},
			{
				name: "fv",
				description: "القيمة المستقبلية المطلوبة للاستثمار"
			}
		]
	},
	{
		name: "PEARSON",
		description: "إرجاع معامل الارتباط العز ومي لحواصل الضرب، r.",
		arguments: [
			{
				name: "array1",
				description: "مجموعة قيم مستقلة"
			},
			{
				name: "array2",
				description: "مجموعة قيم تابعة"
			}
		]
	},
	{
		name: "PERCENTILE",
		description: "إرجاع النسب المئوية للقيم في النطاق.",
		arguments: [
			{
				name: "array",
				description: "صفيف أو نطاق بيانات يعرّف حالات النسبية"
			},
			{
				name: "k",
				description: "القيمة المئوية بين 0 و1، ضمنًا"
			}
		]
	},
	{
		name: "PERCENTILE.EXC",
		description: "إرجاع النسب المئوية للقيم في نطاق، حيث يكون k في النطاق من 0 إلى 1، غير متضمن الواحد.",
		arguments: [
			{
				name: "array",
				description: "صفيف أو نطاق بيانات يعرّف حالات النسبية"
			},
			{
				name: "k",
				description: "القيمة المئوية بين 0 و1، ضمنًا"
			}
		]
	},
	{
		name: "PERCENTILE.INC",
		description: "إرجاع النسب المئوية للقيم في نطاق، حيث يكون k في النطاق من 0 إلى 1، ضمنًا.",
		arguments: [
			{
				name: "array",
				description: "صفيف أو نطاق بيانات يعرّف حالات النسبية"
			},
			{
				name: "k",
				description: "القيمة المئوية بين 0 و1، ضمنًا"
			}
		]
	},
	{
		name: "PERCENTRANK",
		description: "إرجاع رتبة القيمة في مجموعة البيانات كنسبة مئوية لمجموعة البيانات.",
		arguments: [
			{
				name: "array",
				description: "الصفيف أو نطاق البيانات ذات القيم الرقمية التي تعرّف حالات النسبية"
			},
			{
				name: "x",
				description: "القيمة التي تريد معرفة الرتبة الخاصة بها"
			},
			{
				name: "significance",
				description: "قيمة اختيارية تحدد عدد الأرقام ذات الأهمية للنسبة المئوية المرجعة، ثلاثة أرقام إذا حُذفت (0.xxx%)"
			}
		]
	},
	{
		name: "PERCENTRANK.EXC",
		description: "إرجاع رتبة القيمة في مجموعة البيانات كنسبة مئوية لمجموعة البيانات (0 إلى 1، غير متضمن الواحد).",
		arguments: [
			{
				name: "array",
				description: "الصفيف أو نطاق البيانات ذات القيم الرقمية التي تعرّف حالات النسبية"
			},
			{
				name: "x",
				description: "القيمة التي تريد معرفة الرتبة الخاصة بها"
			},
			{
				name: "significance",
				description: "قيمة اختيارية تحدد عدد الأرقام ذات الأهمية للنسبة المئوية المرجعة، ثلاثة أرقام إذا حُذفت (0.xxx%)"
			}
		]
	},
	{
		name: "PERCENTRANK.INC",
		description: "إرجاع رتبة القيمة في مجموعة البيانات كنسبة مئوية لمجموعة البيانات (0 إلى 1، ضمنًا).",
		arguments: [
			{
				name: "array",
				description: "الصفيف أو نطاق البيانات ذات القيم الرقمية التي تعرّف حالات النسبية"
			},
			{
				name: "x",
				description: "القيمة التي تريد معرفة الرتبة الخاصة بها"
			},
			{
				name: "significance",
				description: "قيمة اختيارية تحدد عدد الأرقام ذات الأهمية للنسبة المئوية المرجعة، ثلاثة أرقام إذا حُذفت (0.xxx%)"
			}
		]
	},
	{
		name: "PERMUT",
		description: "إرجاع عدد التباديل لعدد محدد من الكائنات التي يمكن تحديدها من إجمالي الكائنات.",
		arguments: [
			{
				name: "number",
				description: "العدد الإجمالي للكائنات"
			},
			{
				name: "number_chosen",
				description: "الكائنات في كل تبادل"
			}
		]
	},
	{
		name: "PERMUTATIONA",
		description: "إرجاع عدد التبادلات لعدد محدد من الكائنات (مع التكرارات) التي يمكن تحديدها من إجمالي الكائنات.",
		arguments: [
			{
				name: "number",
				description: "العدد الإجمالي للكائنات"
			},
			{
				name: "number_chosen",
				description: "عدد الكائنات في كل تبادل"
			}
		]
	},
	{
		name: "PHI",
		description: "إرجاع قيمة دالة الكثافة لتوزيع عادي قياسي.",
		arguments: [
			{
				name: "x",
				description: "الرقم الذي تريد كثافة التوزيع العادي القياسي له"
			}
		]
	},
	{
		name: "PI",
		description: "إرجاع قيمة الدائري، 3.14159265358979، بالتحديد 15 رقماً.",
		arguments: [
		]
	},
	{
		name: "PMT",
		description: "حساب دفعة القرض استناداً إلى دفعات ثابتة ونسبة فائدة ثابتة.",
		arguments: [
			{
				name: "rate",
				description: "نسبة الفائدة خلال فترة للقرض. على سبيل المثال، استخدام 6%/4 للدفعات الموسمية عند 6% APR"
			},
			{
				name: "nper",
				description: "العدد الإجمالي لدفعات القرض"
			},
			{
				name: "pv",
				description: "القيمة الحالية: المبلغ الإجمالي الذي تساويه الدفعات المستقبلية المستحقة الآن"
			},
			{
				name: "fv",
				description: "القيمة المستقبلية، أو ميزانية نقدية تريد إحرازها بعد إتمام الدفعة الأخيرة. إذا أهملت، Fv = 0‎"
			},
			{
				name: "type",
				description: "قيمة منطقية: الدفع في بداية الفترة = 1؛ الدفع في نهاية الفترة = 0 أو مهمل"
			}
		]
	},
	{
		name: "POISSON",
		description: "إرجاع توزيع Poisson.",
		arguments: [
			{
				name: "x",
				description: "عدد الأحداث"
			},
			{
				name: "mean",
				description: "القيمة الرقمية المتوقعة، رقم موجب"
			},
			{
				name: "cumulative",
				description: "قيمة منطقية: من أجل احتمال Poisson التراكمي، استخدم TRUE؛ من أجل دالة Poisson للاحتمالات غير التراكمية، استخدم FALSE"
			}
		]
	},
	{
		name: "POISSON.DIST",
		description: "إرجاع توزيع Poisson.",
		arguments: [
			{
				name: "x",
				description: "عدد الأحداث"
			},
			{
				name: "mean",
				description: "القيمة الرقمية المتوقعة، رقم موجب"
			},
			{
				name: "cumulative",
				description: "قيمة منطقية: من أجل احتمال Poisson التراكمي، استخدم TRUE؛ من أجل دالة Poisson للاحتمالات غير التراكمية، استخدم FALSE"
			}
		]
	},
	{
		name: "POWER",
		description: "إرجاع نتيجة عدد مرفوع إلى أس.",
		arguments: [
			{
				name: "number",
				description: "الرقم الأساسي، أي رقم حقيقي"
			},
			{
				name: "power",
				description: "الأس الذي سيرفع إليه الأساس"
			}
		]
	},
	{
		name: "PPMT",
		description: "إرجاع الدفعة لرأس المال لاستثمار ما استناداً إلى دفعات دورية ثابتة، ونسبة فائدة ثابتة.",
		arguments: [
			{
				name: "rate",
				description: "نسبة الفائدة لكل فترة. على سبيل المثال، استخدم 6%/4 للدفعات الموسمية في 6% APR"
			},
			{
				name: "per",
				description: "تعيين الفترة ويجب أن تكون ضمن النطاق من 1 إلى nper"
			},
			{
				name: "nper",
				description: "العدد الإجمالي لفترات الدفع في الاستثمار"
			},
			{
				name: "pv",
				description: "القيمة الحالية: المبلغ الإجمالي الذي تساويه سلسلة الدفعات المستقبلية الآن"
			},
			{
				name: "fv",
				description: "القيمة المستقبلية، أو ميزانية نقدية تريد إحرازها بعد إتمام الدفعة الأخيرة"
			},
			{
				name: "type",
				description: "قيمة منطقية: الدفع في بداية الفترة = 1؛ الدفع في نهاية الفترة = 0 أو مهمل"
			}
		]
	},
	{
		name: "PRICEDISC",
		description: "إرجاع القيمة الاسمية لسعر كل 100 ر.س. لورقة مالية ذات خصم.",
		arguments: [
			{
				name: "settlement",
				description: "هو تاريخ تسوية الأوراق المالية، يتم التعبير عنه كرقم متسلسل للتاريخ"
			},
			{
				name: "maturity",
				description: "هو تاريخ استحقاق الأوراق المالية، يتم التعبير عنه كرقم متسلسل للتاريخ"
			},
			{
				name: "discount",
				description: "هي نسبة الخصم الخاصة بالورقة المالية"
			},
			{
				name: "redemption",
				description: "هي قيمة الاسترداد الخاصة بالورقة المالية لكل قيمة اسمية لـ 100 ر.س."
			},
			{
				name: "basis",
				description: "هو نوع الأساس المستخدم لحساب الأيام"
			}
		]
	},
	{
		name: "PROB",
		description: "إرجاع احتمال وقوع قيم النطاق بين حدين أو مساوية لحد أدنى.",
		arguments: [
			{
				name: "x_range",
				description: "النطاق الذي يحتوي قيم س رقمية والتي يوجد احتمالات مقترنة معها"
			},
			{
				name: "prob_range",
				description: "مجموعة الاحتمالات المقترنة مع القيم في X_range، القيم بين 0 و 1 بدون 0"
			},
			{
				name: "lower_limit",
				description: "الحد الأدنى للقيمة التي تريد الاحتمال لها"
			},
			{
				name: "upper_limit",
				description: "الحد الأعلى الاختياري للقيمة. إذا أهمل، سيرجع PROB الاحتمال لكون قيم X_range مساوية لـ Lower_limit"
			}
		]
	},
	{
		name: "PRODUCT",
		description: "ضرب كافة الأرقام المحددة كوسيطات.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "من 1 إلى 255 رقماً أو قيم منطقية أو تمثيل نصي للأرقام التي تريد ضربها"
			},
			{
				name: "number2",
				description: "من 1 إلى 255 رقماً أو قيم منطقية أو تمثيل نصي للأرقام التي تريد ضربها"
			}
		]
	},
	{
		name: "PROPER",
		description: "تحويل سلسلة نصية إلى الأحرف العادية، يكتب الحرف الأول في بداية كل كلمة بالأحرف الكبيرة وكافة الأحرف الأخرى بالأحرف الصغيرة.",
		arguments: [
			{
				name: "text",
				description: "نص ضمن علامات اقتباس، أو صيغة ترجع نصاً، أو مرجع خلية تتضمن نصاً مراد كتابته بالأحرف الكبيرة"
			}
		]
	},
	{
		name: "PV",
		description: "إرجاع القيمة الحالية للاستثمار: المقدار الإجمالي للدفعات المستقبلية المستحقة الآن.",
		arguments: [
			{
				name: "rate",
				description: "نسبة الفائدة لكل فترة. على سبيل المثال، استخدام 6%/4 للدفعات الموسمية عند 6% APR"
			},
			{
				name: "nper",
				description: "العدد الإجمالي لفترات الدفع في الاستثمار"
			},
			{
				name: "pmt",
				description: "الدفعة خلال كل فترة ولا يمكن أن يتغير خلال مدة الاستثمار"
			},
			{
				name: "fv",
				description: "القيمة المستقبلية، أو ميزانية نقدية تريد إحرازها بعد إتمام الدفعة الأخيرة"
			},
			{
				name: "type",
				description: "قيمة منطقية: الدفع في بداية الفترة = 1; الدفع في نهاية الفترة = 0 أو مهمل"
			}
		]
	},
	{
		name: "QUARTILE",
		description: "إرجاع ربع مجموعة البيانات.",
		arguments: [
			{
				name: "array",
				description: "صفيف أو نطاق خلايا من القيم الرقمية التي تريد قيمة الربع لها"
			},
			{
				name: "quart",
				description: "رقم: لإرجاع القيمة الدنيا = 0؛ لإرجاع الربع الأول = 1؛ لإرجاع قيمة الوسيط = 2؛ لإرجاع الربع الثالث = 3؛ لإرجاع القيمة القصوى = 4"
			}
		]
	},
	{
		name: "QUARTILE.EXC",
		description: "إرجاع ربع مجموعة البيانات، استنادًا إلى القيم المئوية من 0 إلى 1، غير متضمن الواحد.",
		arguments: [
			{
				name: "array",
				description: "صفيف أو نطاق خلايا من القيم الرقمية التي تريد قيمة الربع لها"
			},
			{
				name: "quart",
				description: "رقم: لإرجاع القيمة الدنيا = 0؛ لإرجاع الربع الأول = 1؛ لإرجاع قيمة الوسيط = 2؛ لإرجاع الربع الثالث = 3؛ لإرجاع القيمة القصوى = 4"
			}
		]
	},
	{
		name: "QUARTILE.INC",
		description: "إرجاع ربع مجموعة البيانات، استنادًا إلى القيم المئوية من 0 إلى 1، ضمنًا.",
		arguments: [
			{
				name: "array",
				description: "صفيف أو نطاق خلايا من القيم الرقمية التي تريد قيمة الربع لها"
			},
			{
				name: "quart",
				description: "رقم: لإرجاع القيمة الدنيا = 0؛ لإرجاع الربع الأول = 1؛ لإرجاع قيمة الوسيط = 2؛ لإرجاع الربع الثالث = 3؛ لإرجاع القيمة القصوى = 4"
			}
		]
	},
	{
		name: "QUOTIENT",
		description: "إرجاع جزء العدد الصحيح الخاص بالقسمة.",
		arguments: [
			{
				name: "numerator",
				description: "هو المقسوم"
			},
			{
				name: "denominator",
				description: "هو عامل القسمة"
			}
		]
	},
	{
		name: "RADIANS",
		description: "تحويل الدرجات إلى التقدير الدائري.",
		arguments: [
			{
				name: "angle",
				description: "الزاوية بالدرجات والتي تريد تحويلها"
			}
		]
	},
	{
		name: "RAND",
		description: "إرجاع رقم عشوائي أكبر من أو يساوي 0 وأصغر من 1، تم توزيعه بالتساوي (يتغير عند إعادة الحساب).",
		arguments: [
		]
	},
	{
		name: "RANDBETWEEN",
		description: "إرجاع رقم عشوائي يقع بين الأرقام التي قمت بتحديدها.",
		arguments: [
			{
				name: "bottom",
				description: "هو أصغر عدد صحيح RANDBETWEEN سيتم إرجاعه"
			},
			{
				name: "top",
				description: "هو أكبر عدد صحيح RANDBETWEEN سيتم إرجاعه"
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
		description: "إرجاع رتبة رقم في قائمة الأرقام: حجمه مقاس نسبة إلى قيم أخرى في القائمة.",
		arguments: [
			{
				name: "number",
				description: "الرقم الذي تريد معرفة الرتبة الخاصة به"
			},
			{
				name: "ref",
				description: "صفيف من قائمة أرقام، أو مرجع لقائمة أرقام. يتم تجاهل القيم غير الرقمية"
			},
			{
				name: "order",
				description: "رقم: مرتّبة في قائمة مفروزة تنازلياً = 0 أو محذوفة؛ مرتّبة في قائمة مفروزة تصاعدياً = قيمة غير الصفر"
			}
		]
	},
	{
		name: "RANK.AVG",
		description: "إرجاع رتبة رقم في قائمة الأرقام: حجمه بالنسبة إلى القيم الأخرى في القائمة، إذا كان هناك أكثر من قيمة واحدة لها نفس الرتبة، يتم إرجاع متوسط الرتبة.",
		arguments: [
			{
				name: "number",
				description: "الرقم الذي تريد العثور على الرتبة الخاصة به"
			},
			{
				name: "ref",
				description: "صفيف من قائمة أرقام، أو مرجع لقائمة أرقام. يتم تجاهل القيم غير الرقمية"
			},
			{
				name: "order",
				description: "رقم: رتبة في قائمة مفروزة تنازليًا = 0 أو حذفه؛ رتبة في قائمة مفروزة تصاعديًا = أي قيمة غير الصفر"
			}
		]
	},
	{
		name: "RANK.EQ",
		description: "إرجاع رتبة رقم في قائمة الأرقام: حجمه بالنسبة إلى القيم الأخرى في القائمة، إذا كان هناك أكثر من قيمة واحدة لها نفس الرتبة، يتم إرجاع أعلى رتبة لمجموعة القيم هذه.",
		arguments: [
			{
				name: "number",
				description: "الرقم الذي تريد العثور على الرتبة الخاصة به"
			},
			{
				name: "ref",
				description: "صفيف من قائمة أرقام، أو مرجع لقائمة أرقام. يتم تجاهل القيم غير الرقمية"
			},
			{
				name: "order",
				description: "رقم: رتبة في قائمة مفروزة تنازليًا = 0 أو حذفه؛ رتبة في قائمة مفروزة تصاعديًا = أي قيمة غير الصفر"
			}
		]
	},
	{
		name: "RATE",
		description: "إرجاع نسبة الفائدة بالفترة لقرض أو لاستثمار. على سبيل المثال، استخدام 6%/4 للدفعات الموسمية عند 6% APR.",
		arguments: [
			{
				name: "nper",
				description: "العدد الإجمالي لفترات قرض أو استثمار"
			},
			{
				name: "pmt",
				description: "الدفع الذي يحدث كل فترة ولا يمكن تغييره خلال مدة القرض أو مدة الاستثمار"
			},
			{
				name: "pv",
				description: "القيمة الحالية: المبلغ الإجمالي الذي تساويه الدفعات المستقبلية المستحقة الآن"
			},
			{
				name: "fv",
				description: "القيمة المستقبلية، أو ميزانية نقدية تريد إحرازها بعد إتمام الدفعة الأخيرة. إذا أهملت، Fv = 0‎"
			},
			{
				name: "type",
				description: "القيمة المنطقية: الدفع في بداية الفترة = 1؛ الدفع في نهاية الفترة = 0 أو مهمل"
			},
			{
				name: "guess",
				description: "تخمينك للنسبة المئوية؛ إذا أهمل، Guess = 0.1 (10 بالمائة)"
			}
		]
	},
	{
		name: "RECEIVED",
		description: "إرجاع مقدار المبلغ التي سيتم تلقيه عند تاريخ الاستحقاق لورقة مالية تم استثمارها بشكل كامل.",
		arguments: [
			{
				name: "settlement",
				description: "هو تاريخ تسوية الأوراق المالية، يتم التعبير عنه كرقم متسلسل للتاريخ"
			},
			{
				name: "maturity",
				description: "هو تاريخ استحقاق الأوراق المالية، يتم التعبير عنه كرقم متسلسل للتاريخ"
			},
			{
				name: "investment",
				description: "هو مقدار المبلغ الذي تم استثماره في الورقة المالية"
			},
			{
				name: "discount",
				description: "هي نسبة الخصم الخاص بالورقة المالية"
			},
			{
				name: "basis",
				description: "هو نوع الأساس المستخدم لحساب الأيام"
			}
		]
	},
	{
		name: "REPLACE",
		description: "تبديل جزء من نص بسلسلة نصية مختلفة.",
		arguments: [
			{
				name: "old_text",
				description: "نص ترغب بتبديل بعض الأحرف فيه"
			},
			{
				name: "start_num",
				description: "موقع الحرف في Old_text الذي تريد تبديله بـ New_text"
			},
			{
				name: "num_chars",
				description: "عدد الأحرف في Old_text التي تريد استبدالها"
			},
			{
				name: "new_text",
				description: "النص الذي سيبدل الأحرف في Old_text"
			}
		]
	},
	{
		name: "REPT",
		description: "تكرار نص بعدد مرات معطى. استخدم REPT لتعبئة خلية بعدد مرات تماثل سلسلة نصية.",
		arguments: [
			{
				name: "text",
				description: "النص الذي تريد تكراره"
			},
			{
				name: "number_times",
				description: "رقم موجب يحدد عدد مرات تكرار النص"
			}
		]
	},
	{
		name: "RIGHT",
		description: "إرجاع عدد الأحرف المحدد في نهاية سلسلة نصية.",
		arguments: [
			{
				name: "text",
				description: "هي السلسلة النصية التي تحتوي على الأحرف المراد استخراجها"
			},
			{
				name: "num_chars",
				description: "تحديد عدد الأحرف التي تريد استخراجها، 1 إذا أهمل"
			}
		]
	},
	{
		name: "ROMAN",
		description: "تحويل الأرقام العربية إلى رومانية على شكل نص.",
		arguments: [
			{
				name: "number",
				description: "الرقم العربي الذي تريد تحويله"
			},
			{
				name: "form",
				description: "رقم يحدد نوع الرقم الروماني الذي تريده."
			}
		]
	},
	{
		name: "ROUND",
		description: "تقريب رقم إلى عدد خانات رقمية معين.",
		arguments: [
			{
				name: "number",
				description: "الرقم الذي تريد تقريبه"
			},
			{
				name: "num_digits",
				description: "عدد المنازل التي سيتم التقريب إليها. الأرقام السالبة تقرب إلى يسار الفاصلة العشرية؛ الصفر إلى أقرب رقم صحيح"
			}
		]
	},
	{
		name: "ROUNDDOWN",
		description: "تقريب رقم للأسفل، باتجاه الصفر.",
		arguments: [
			{
				name: "number",
				description: "أي رقم حقيقي تريد تقريبه إلى الأدنى"
			},
			{
				name: "num_digits",
				description: "عدد الخانات التي تريد التقريب لها. الأرقام السالبة تقرًب إلى يسار الفاصلة العشرية؛ الصفر أو المهمل، تقرًب إلى أقرب رقم صحيح"
			}
		]
	},
	{
		name: "ROUNDUP",
		description: "تقريب رقم للأعلى، بعيداً عن الصفر.",
		arguments: [
			{
				name: "number",
				description: "أي رقم حقيقي تريد تقريبه للأعلى"
			},
			{
				name: "num_digits",
				description: "عدد الخانات التي تريد التقريب لها. الأرقام السالبة تقرًب إلى يسار الفاصلة العشرية؛ الصفر أو المهمل، تقرًب إلى أقرب رقم صحيح"
			}
		]
	},
	{
		name: "ROW",
		description: "إرجاع رقم صف مرجع.",
		arguments: [
			{
				name: "reference",
				description: "الخلية أو نطاق واحد من الخلايا تريد معرفة رقم الصف الخاص به؛ إذا تم إهمال ذلك، يتم إرجاع الخلية التي تحتوي على الدالة ROW"
			}
		]
	},
	{
		name: "ROWS",
		description: "إرجاع عدد الصفوف في مرجع أو في صفيف.",
		arguments: [
			{
				name: "array",
				description: "صفيف، أو صيغة صفيف، أو مرجع نطاق من الخلايا تريد عدد صفوفه"
			}
		]
	},
	{
		name: "RRI",
		description: "إرجاع نسبة الفائدة المساوية لنمو استثمار.",
		arguments: [
			{
				name: "nper",
				description: "عدد فترات الاستثمار"
			},
			{
				name: "pv",
				description: "القيمة الحالية للاستثمار"
			},
			{
				name: "fv",
				description: "القيمة المستقبلية للاستثمار"
			}
		]
	},
	{
		name: "RSQ",
		description: "إرجاع مربع معامل الارتباط العز ومي لحواصل الضرب من خلال نقاط البيانات المعطاة.",
		arguments: [
			{
				name: "known_y's",
				description: "صفيف أو نطاق من نقاط بيانات والتي يمكن أن تكون أرقاماً أو أسماء، صفائف، أو مراجع تحتوي على أرقام"
			},
			{
				name: "known_x's",
				description: "صفيف أو نطاق من نقاط بيانات والتي يمكن أن تكون أرقاماً أو أسماء، صفائف، أو مراجع تحتوي على أرقام"
			}
		]
	},
	{
		name: "RTD",
		description: "استرداد بيانات الوقت الحقيقي من برنامج يعتمد التنفيذ التلقائي لـ COM.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "progID",
				description: "اسم ProgID الخاص بالوظيفة الإضافية للتنفيذ التلقائي لـ COM المسجلة. وضع الاسم بين علامتي اقتباس"
			},
			{
				name: "server",
				description: " اسم الخادم الذي يجب تشغيل الوظيفة الإضافية علي. وضع الاسم بين علامتي اقتباس. إذا تم تشغيل الوظيفة الإضافية محلياً، استخدم سلسلة فارغة"
			},
			{
				name: "topic1",
				description: " من 1 إلى 38 معلمة تحدد جزء من البيانات"
			},
			{
				name: "topic2",
				description: " من 1 إلى 38 معلمة تحدد جزء من البيانات"
			}
		]
	},
	{
		name: "SEARCH",
		description: "إرجاع رقم الحرف الذي تم عنده العثور على أول حرف محدد أو أول سلسلة نصية محددة، باتجاه القراءة من اليسار لليمين (غير متحسس لحالة الأحرف).",
		arguments: [
			{
				name: "find_text",
				description: "النص الذي تريد البحث عنه. استخدم الأحرف البدل ؟ و *؛ استخدم ~؟ و ~* للبحث عن الأحرف ؟ و *"
			},
			{
				name: "within_text",
				description: "النص الذي تريد البحث عن find_text فيه"
			},
			{
				name: "start_num",
				description: "رقم الحرف في Within_text، والعد من اليسار، الذي تريد أن يبدأ البحث عنده. إذا أهمل، يتم استخدام 1"
			}
		]
	},
	{
		name: "SEC",
		description: "إرجاع قاطع المنحنى لزاوية.",
		arguments: [
			{
				name: "number",
				description: "الزاوية بالتقدير الدائري التي تريد إيجاد قاطع المنحنى الخاص بها"
			}
		]
	},
	{
		name: "SECH",
		description: "إرجاع قاطع المنحنى الزائدي لزاوية.",
		arguments: [
			{
				name: "number",
				description: "الزاوية بالتقدير الدائري التي تريد إيجاد قاطع المنحنى الزائدي الخاص بها"
			}
		]
	},
	{
		name: "SECOND",
		description: "إرجاع الثانية، وهي عبارة عن رقم من 0 إلى 59.",
		arguments: [
			{
				name: "serial_number",
				description: "رقم في التعليمات البرمجية الخاصة بالتاريخ والوقت المستخدم من قبل Spreadsheet أو نص بتنسيق وقتي، مثل 16:48:23 أو 4:48:47 م"
			}
		]
	},
	{
		name: "SERIESSUM",
		description: "إرجاع مجموع سلسلة الأس استناداً إلى الصيغة.",
		arguments: [
			{
				name: "x",
				description: "هو قيمة الإدخال الخاصة بسلسلة الأس"
			},
			{
				name: "n",
				description: "هو الأس الأولي الذي ترغب في رفع س إليه"
			},
			{
				name: "m",
				description: "هي الخطوة التي يتم اتخاذها لزيادة العدد لكل شرط من شروط السلسلة"
			},
			{
				name: "coefficients",
				description: "هو مجموعة المعاملات التي يتم ضرب كل أس لاحق لـ س فيها"
			}
		]
	},
	{
		name: "SHEET",
		description: "إرجاع رقم الورقة المشار إليها.",
		arguments: [
			{
				name: "value",
				description: "اسم الورقة أو المرجع الذي تريد رقم ورقته. في حالة الإهمال، يتم إرجاع رقم الورقة التي تحتوي على الدالة"
			}
		]
	},
	{
		name: "SHEETS",
		description: "إرجاع عدد الورق في مرجع.",
		arguments: [
			{
				name: "reference",
				description: "المرجع الذي تريد معرفة عدد الورق التي يتضمنها. في حالة الإهمال، يتم إرجاع عدد ورقات المصنف الذي يحتوي على الدالة"
			}
		]
	},
	{
		name: "SIGN",
		description: "إرجاع إشارة رقم: 1 إذا كان الرقم موجباً، أو صفر إذا كان الرقم صفراً، أو -1 إذا كان الرقم سالباً.",
		arguments: [
			{
				name: "number",
				description: "أي رقم حقيقي"
			}
		]
	},
	{
		name: "SIN",
		description: "إرجاع جيب الزاوية.",
		arguments: [
			{
				name: "number",
				description: "الزاوية بالتقدير الدائري التي تريد معرفة جيبها. درجات * PI()/180 = تقدير دائري"
			}
		]
	},
	{
		name: "SINH",
		description: "إرجاع جيب الزاوية للقطع الزائد لأحد الأرقام.",
		arguments: [
			{
				name: "number",
				description: "أي رقم حقيقي"
			}
		]
	},
	{
		name: "SKEW",
		description: "إرجاع تخالف التوزيع: وصف لدرجة اللا تماثل لتوزيع حول وسطه.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "من 1 إلى 255 رقماً أو اسماً أو صفيفاً أو مرجعاً يحتوي على أرقام  تريد تخالفها"
			},
			{
				name: "number2",
				description: "من 1 إلى 255 رقماً أو اسماً أو صفيفاً أو مرجعاً يحتوي على أرقام  تريد تخالفها"
			}
		]
	},
	{
		name: "SKEW.P",
		description: "إرجاع تخالف التوزيع استناداً إلى محتوى: وصف لدرجة اللا تماثل لتوزيع حول وسطه.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "من 1 إلى 254 رقماً أو اسماً أو صفيفاً أو مرجعاً يحتوي على أرقام تريد تخالف محتواها"
			},
			{
				name: "number2",
				description: "من 1 إلى 254 رقماً أو اسماً أو صفيفاً أو مرجعاً يحتوي على أرقام تريد تخالف محتواها"
			}
		]
	},
	{
		name: "SLN",
		description: "إرجاع الإهلاك الثابت لموجودات في فترة واحدة.",
		arguments: [
			{
				name: "cost",
				description: "التكلفة الأولية للموجودات"
			},
			{
				name: "salvage",
				description: "قيمة الخردة عند نهاية عمر الموجودات"
			},
			{
				name: "life",
				description: "عدد الفترات التي سيتم فيها إهلاك الموجودات (تسمى في بعض الأحيان العمر الإنتاجي للموجودات)"
			}
		]
	},
	{
		name: "SLOPE",
		description: "إرجاع ميل خط الانحدار الخطي المار خلال نقاط البيانات المعطاة.",
		arguments: [
			{
				name: "known_y's",
				description: "صفيف أو نطاق من خلايا يحتوي على نقاط بيانات رقمية تابعة والتي يمكن أن تكون أرقاماً أو أسماء، صفائف، أو مراجع تحتوي على أرقام"
			},
			{
				name: "known_x's",
				description: "مجموعة من نقاط بيانات مستقلة والتي يمكن أن تكون أرقام أو أسماء، صفائف، أو مراجع تحتوي على أرقام"
			}
		]
	},
	{
		name: "SMALL",
		description: "إرجاع ترتيب القيم الصغرى في مجموعة البيانات. على سبيل المثال، خامس أصغر قيمة.",
		arguments: [
			{
				name: "array",
				description: "الصفيف أو نطاق البيانات الذي تريد تحديد ترتيب القيم الصغرى فيه"
			},
			{
				name: "k",
				description: "موضع القيمة التي سترجع  (اعتباراً من الأصغر) في الصفيف أو في نطاق الخلايا"
			}
		]
	},
	{
		name: "SQRT",
		description: "إرجاع الجذر التربيعي لرقم.",
		arguments: [
			{
				name: "number",
				description: "الرقم الذي تريد جذره التربيعي"
			}
		]
	},
	{
		name: "SQRTPI",
		description: "إرجاع الجذر التربيعي لـ (number * Pi).",
		arguments: [
			{
				name: "number",
				description: " هو الرقم الذي سيتم ضرب p فيه"
			}
		]
	},
	{
		name: "STANDARDIZE",
		description: "إرجاع قيمة مسوّاة من توزيع مميز بوسط وبانحراف معياري.",
		arguments: [
			{
				name: "x",
				description: "القيمة التي تريد أن تجعلها مسوّاة"
			},
			{
				name: "mean",
				description: "الوسط الحسابي للتوزيع"
			},
			{
				name: "standard_dev",
				description: "الانحراف المعياري لـلتوزيع، وهو رقم موجب"
			}
		]
	},
	{
		name: "STDEV",
		description: "تقدير الانحراف المعياري استناداً إلى نموذج (تجاهل القيم المنطقية والنصوص في النموذج).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: " من 1 إلى 255 رقماً يطابق نموذجاً للمحتوى ويمكن أن تكون أرقاماً أو مراجع تحتوي على أرقام"
			},
			{
				name: "number2",
				description: " من 1 إلى 255 رقماً يطابق نموذجاً للمحتوى ويمكن أن تكون أرقاماً أو مراجع تحتوي على أرقام"
			}
		]
	},
	{
		name: "STDEV.P",
		description: "حساب الانحراف المعياري استنادًا إلى المحتوى بأكمله والمحدد كوسيطات (تجاهل القيم المنطقية والنصوص).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "من 1 إلى 255 رقمًا يطابق المحتوى ويمكن أن تكون أرقامًا أو مراجع تحتوي على أرقام"
			},
			{
				name: "number2",
				description: "من 1 إلى 255 رقمًا يطابق المحتوى ويمكن أن تكون أرقامًا أو مراجع تحتوي على أرقام"
			}
		]
	},
	{
		name: "STDEV.S",
		description: "تقدير الانحراف المعياري استنادًا إلى نموذج (تجاهل القيم المنطقية والنصوص في النموذج).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "من 1 إلى 255 رقمًا يطابق نموذج للمحتوى ويمكن أن تكون أرقامًا أو مراجع تحتوي على أرقام"
			},
			{
				name: "number2",
				description: "من 1 إلى 255 رقمًا يطابق نموذج للمحتوى ويمكن أن تكون أرقامًا أو مراجع تحتوي على أرقام"
			}
		]
	},
	{
		name: "STDEVA",
		description: "تقدير الانحراف المعياري استناداً إلى نموذج، متضمناً القيم المنطقية والنصوص. تأخذ النصوص والقيمة المنطقية FALSE القيمة 0؛ تأخذ القيمة المنطقية TRUE القيمة 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "‎ من 1 إلى 255 قيمة مطابقة لنموذج للمحتوى ويمكن أن تكون قيماً أو أسماء أو صفائف أو مراجع تحتوي على قيم"
			},
			{
				name: "value2",
				description: "‎ من 1 إلى 255 قيمة مطابقة لنموذج للمحتوى ويمكن أن تكون قيماً أو أسماء أو صفائف أو مراجع تحتوي على قيم"
			}
		]
	},
	{
		name: "STDEVP",
		description: "حساب الانحراف المعياري استناداً إلى المحتوى بأكمله والمحدد كوسيطات (تجاهل القيم المنطقية والنصوص).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "من 1 إلى 255 رقماً يطابق المحتوى ويمكن أن تكون أرقاماً أو مراجع تحتوي على أرقام"
			},
			{
				name: "number2",
				description: "من 1 إلى 255 رقماً يطابق المحتوى ويمكن أن تكون أرقاماً أو مراجع تحتوي على أرقام"
			}
		]
	},
	{
		name: "STDEVPA",
		description: "حساب الانحراف المعياري استناداً إلى المحتوى بأكمله، متضمناً القيم المنطقية و النصوص. تأخذ النصوص والقيمة المنطقية FALSE القيمة 0؛ تأخذ القيمة المنطقية TRUE القيمة 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "‎ من 1 إلى 255 قيمة مطابقة للمحتوى ويمكن أن تكون قيماً أو أسماء أو صفائف أو مراجع تحتوي على قيم"
			},
			{
				name: "value2",
				description: "‎ من 1 إلى 255 قيمة مطابقة للمحتوى ويمكن أن تكون قيماً أو أسماء أو صفائف أو مراجع تحتوي على قيم"
			}
		]
	},
	{
		name: "STEYX",
		description: "إرجاع الخطأ القياسي لقيم ص المتوقعة وذلك من أجل كل س في انحدار.",
		arguments: [
			{
				name: "known_y's",
				description: "صفيف أو نطاق من نقاط بيانات تابعة والتي يمكن أن تكون أرقاماً أو أسماء، صفائف، أو مراجع تحتوي على أرقام"
			},
			{
				name: "known_x's",
				description: "صفيف أو نطاق من نقاط بيانات مستقلة والتي يمكن أن تكون أرقاماً أو أسماء، صفائف، أو مراجع تحتوي على أرقام"
			}
		]
	},
	{
		name: "SUBSTITUTE",
		description: "تبديل النص الموجود بنص جديد في سلسلة نصية.",
		arguments: [
			{
				name: "text",
				description: "النص أو المرجع إلى خلية تحتوي على نص تريد استبدال أحرف فيها"
			},
			{
				name: "old_text",
				description: "النص الموجود والذي تريد تبديله. إذا كانت حالة Old_text لا تطابق حالة text، فإن SUBSTITUTE لن تقوم بتبديل النص"
			},
			{
				name: "new_text",
				description: "النص الذي تريد تبديل Old_text به"
			},
			{
				name: "instance_num",
				description: "تحديد الظهور لـ Old_text الذي تريد تبديله. إذا أهمل، سيتم تبديل كل تواجد لـ Old_text"
			}
		]
	},
	{
		name: "SUBTOTAL",
		description: "إرجاع الإجمالي الفرعي لقائمة أو قاعدة بيانات.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "function_num",
				description: "الأرقام من 1 إلى 11 تحدد دالة التلخيص للإجمالي الفرعي."
			},
			{
				name: "ref1",
				description: "‎ من 1 إلى 254 نطاق أو مرجع والتي تريد الإجمالي الفرعي لها"
			}
		]
	},
	{
		name: "SUM",
		description: "إضافة كافة الأرقام الموجودة في نطاق من الخلايا.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "‎ من 1 إلى 255 رقم ليتم جمعها. يتم تجاهل القيم المنطقية والنصوص الموجودة في الخلايا، ويتم تضمّينها إذا كتبت كوسيطات"
			},
			{
				name: "number2",
				description: "‎ من 1 إلى 255 رقم ليتم جمعها. يتم تجاهل القيم المنطقية والنصوص الموجودة في الخلايا، ويتم تضمّينها إذا كتبت كوسيطات"
			}
		]
	},
	{
		name: "SUMIF",
		description: "جمع الخلايا المحددة بشرط معطى أو معيار معطى.",
		arguments: [
			{
				name: "range",
				description: "النطاق من الخلايا الذي تريد تقييمه"
			},
			{
				name: "criteria",
				description: "الشرط أو المعيار بشكل رقم، أو تعبير، أو نص يعرف الخلايا التي ستجمع"
			},
			{
				name: "sum_range",
				description: "الخلايا الفعلية التي ستجمع. إذا أهمل، ستستخدم الخلايا في نطاق"
			}
		]
	},
	{
		name: "SUMIFS",
		description: "إضافة الخلايا المحدد طبقاً لمجموعة من الشروط أو المعايير.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "sum_range",
				description: "هي الخلايا التي سيتم جمعها بالفعل."
			},
			{
				name: "criteria_range",
				description: "هو نطاق الخلايا التي ترغب في تقييمها لشرط معين"
			},
			{
				name: "criteria",
				description: "هو الشرط أو المعيار الموجود على شكل رقم أو تعبير أو نص يحدد الخلايا التي سيتم جمعها"
			}
		]
	},
	{
		name: "SUMPRODUCT",
		description: "إرجاع مجموع المنتجات الخاص بالنطاقات أو الصفائف المتطابقة.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "array1",
				description: "من 2 إلى 255صفيفاً تريد ضرب وجمع مكونات لها. يجب أن يكون لكافة الصفائف نفس الأبعاد"
			},
			{
				name: "array2",
				description: "من 2 إلى 255صفيفاً تريد ضرب وجمع مكونات لها. يجب أن يكون لكافة الصفائف نفس الأبعاد"
			},
			{
				name: "array3",
				description: "من 2 إلى 255صفيفاً تريد ضرب وجمع مكونات لها. يجب أن يكون لكافة الصفائف نفس الأبعاد"
			}
		]
	},
	{
		name: "SUMSQ",
		description: "إرجاع مجموع مربعات الوسيطات. ويمكن أن تكون الوسيطات أرقاماً أو صفائف أو أسماء أو مراجع إلى خلايا تحتوي على أرقام.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "‎ من 1 إلى 255 رقماً أو صفيفأ أو مرجع إلى صفيف تريد مجموع مربعاتها"
			},
			{
				name: "number2",
				description: "‎ من 1 إلى 255 رقماً أو صفيفأ أو مرجع إلى صفيف تريد مجموع مربعاتها"
			}
		]
	},
	{
		name: "SUMX2MY2",
		description: "جمع الفرق بين مربعات نطاقين أو صفيفين متطابقين.",
		arguments: [
			{
				name: "array_x",
				description: "النطاق أو الصفيف الأول من الأرقام ويمكن أن يكون رقماً أو اسماً، أو صفيفاً، أو مرجعاً يحتوي على أرقام"
			},
			{
				name: "array_y",
				description: "النطاق أو الصفيف الثاني من الأرقام ويمكن أن يكون رقماً أو اسماً، أو صفيفاً، أو مرجعاً يحتوي على أرقام"
			}
		]
	},
	{
		name: "SUMX2PY2",
		description: "إرجاع إجمالي مجموع مجموعات مربعات الأرقام في نطاقين أو في صفيفين متطابقين.",
		arguments: [
			{
				name: "array_x",
				description: "النطاق أو الصفيف الأول من الأرقام ويمكن أن يكون رقماً أو اسماً، أو صفيفاً، أو مرجعاً يحتوي على أرقام"
			},
			{
				name: "array_y",
				description: "النطاق أو الصفيف الثاني من الأرقام ويمكن أن يكون رقماً أو اسماً، أو صفيفاً، أو مرجعاً يحتوي على أرقام"
			}
		]
	},
	{
		name: "SUMXMY2",
		description: "جمع مربعات الفرق بين القيم في نطاقين أو في صفيفين متطابقين.",
		arguments: [
			{
				name: "array_x",
				description: "النطاق أو الصفيف الأول من القيم ويمكن أن يكون رقم أو اسم، أو صفيف، أو مرجع يحتوي على أرقام"
			},
			{
				name: "array_y",
				description: "النطاق أو الصفيف الثاني من القيم ويمكن أن يكون رقماً أو اسماً، أو صفيفاً، أو مرجعاً يحتوي على أرقام"
			}
		]
	},
	{
		name: "SYD",
		description: "إرجاع عدد سنوات إهلاك الموجودات لفترة محددة.",
		arguments: [
			{
				name: "cost",
				description: "التكلفة الأولية للموجودات"
			},
			{
				name: "salvage",
				description: "قيمة الخردة عند نهاية عمر الموجودات"
			},
			{
				name: "life",
				description: "عدد الفترات التي سيتم فيها إهلاك الموجودات (تسمى في بعض الأحيان العمر الإنتاجي للموجودات)"
			},
			{
				name: "per",
				description: "يجب أن تستخدم نفس وحدات العمر بالنسبة للفترة"
			}
		]
	},
	{
		name: "T",
		description: "التحقق من أن القيمة نصاً وإرجاع النص إن كانت القيمة كذلك أو إرجاع علامتي اقتباس مزدوجة (نص فارغ) إذا لم تكن نصاً.",
		arguments: [
			{
				name: "value",
				description: "هي القيمة التي تريد اختبارها"
			}
		]
	},
	{
		name: "T.DIST",
		description: "إرجاع توزيع t للطالب ذي الطرف الأيمن.",
		arguments: [
			{
				name: "x",
				description: "القيمة الرقمية المراد تقييم التوزيع عندها"
			},
			{
				name: "deg_freedom",
				description: "عدد صحيح يشير إلى عدد درجات الحرية التي تميز التوزيع"
			},
			{
				name: "cumulative",
				description: "قيمة منطقية: من أجل دالة التوزيع التراكمي، استخدم TRUE؛ من أجل دالة كثافة الاحتمال، استخدم FALSE"
			}
		]
	},
	{
		name: "T.DIST.2T",
		description: "إرجاع توزيع t للطالب ثنائي الطرف.",
		arguments: [
			{
				name: "x",
				description: "القيمة الرقمية المراد تقييم التوزيع عندها"
			},
			{
				name: "deg_freedom",
				description: "عدد صحيح يشير إلى عدد درجات الحرية التي تميز التوزيع"
			}
		]
	},
	{
		name: "T.DIST.RT",
		description: "إرجاع توزيع t للطالب ذي الطرف الأيمن.",
		arguments: [
			{
				name: "x",
				description: "القيمة الرقمية المراد تقييم التوزيع عندها"
			},
			{
				name: "deg_freedom",
				description: "عدد صحيح يشير إلى عدد درجات الحرية التي تميز التوزيع"
			}
		]
	},
	{
		name: "T.INV",
		description: "إرجاع عكس توزيع t للطالب ذي الطرف الأيسر.",
		arguments: [
			{
				name: "probability",
				description: "الاحتمال المقترن بتوزيع t للطالب ثنائي الطرف، رقم بين 0 و1 ضمنًا"
			},
			{
				name: "deg_freedom",
				description: "عدد صحيح موجب يشير إلى عدد درجات الحرية التي تميز التوزيع"
			}
		]
	},
	{
		name: "T.INV.2T",
		description: "إرجاع عكس توزيع t للطالب ثنائي الطرف.",
		arguments: [
			{
				name: "probability",
				description: "الاحتمال المقترن بتوزيع t للطالب ثنائي الطرف، رقم بين 0 و1 ضمنًا"
			},
			{
				name: "deg_freedom",
				description: "عدد صحيح موجب يشير إلى عدد درجات الحرية التي تميز التوزيع"
			}
		]
	},
	{
		name: "T.TEST",
		description: "إرجاع الاحتمال المقترن باختبارات الطالب t-Test.",
		arguments: [
			{
				name: "array1",
				description: "مجموعة البيانات الأولى"
			},
			{
				name: "array2",
				description: "مجموعة البيانات الثانية"
			},
			{
				name: "tails",
				description: "تحديد عدد أطراف التوزيع التي سيتم إرجاعها: توزيع وحيد الطرف = 1؛ توزيع ثنائي الطرف = 2"
			},
			{
				name: "type",
				description: "نوع t-test: زوجي = 1، نموذجان متساويان بالتباين = 2، نموذجين غير متساويين بالتباين = 3"
			}
		]
	},
	{
		name: "TAN",
		description: "إرجاع ظل الزاوية.",
		arguments: [
			{
				name: "number",
				description: "الزاوية بالتقدير الدائري التي تريد ظلها. درجات * PI()/180 = تقدير دائري"
			}
		]
	},
	{
		name: "TANH",
		description: "إرجاع ظل الزاوية للقطع الزائد لأحد الأرقام.",
		arguments: [
			{
				name: "number",
				description: "رقم حقيقي"
			}
		]
	},
	{
		name: "TBILLEQ",
		description: "إرجاع العائد المساوي للسند لإذن الخزانة.",
		arguments: [
			{
				name: "settlement",
				description: "هو تاريخ تسوية إذن الخزانة، يتم التعبير عنه كرقم متسلسل للتاريخ"
			},
			{
				name: "maturity",
				description: "هو تاريخ استحقاق إذن الخزانة، يتم التعبير عنه كرقم متسلسل للتاريخ"
			},
			{
				name: "discount",
				description: "هي نسبة الخصم الخاصة بإذن الخزانة"
			}
		]
	},
	{
		name: "TBILLPRICE",
		description: "إرجاع السعر لكل قيمة اسمية لـ 100 ر.س. لإذن خزانة.",
		arguments: [
			{
				name: "settlement",
				description: "هو تاريخ تسوية إذن الخزانة، يتم التعبير عنه كرقم متسلسل للتاريخ"
			},
			{
				name: "maturity",
				description: "هو تاريخ استحقاق إذن الخزانة، يتم التعبير عنه كرقم متسلسل للتاريخ"
			},
			{
				name: "discount",
				description: "هي نسبة الخصم الخاصة بإذن الخزانة"
			}
		]
	},
	{
		name: "TBILLYIELD",
		description: "إرجاع العائد الخاص بإذن الخزانة.",
		arguments: [
			{
				name: "settlement",
				description: "هو تاريخ تسوية إذن الخزانة، يتم التعبير عنه كرقم متسلسل للتاريخ"
			},
			{
				name: "maturity",
				description: "هو تاريخ استحقاق إذن الخزانة، يتم التعبير عنه كرقم متسلسل للتاريخ"
			},
			{
				name: "pr",
				description: "هو سعر إذن الخزانة لكل قيمة اسمية لـ 100 ر.س."
			}
		]
	},
	{
		name: "TDIST",
		description: "إرجاع توزيع t للطالب.",
		arguments: [
			{
				name: "x",
				description: "القيمة الرقمية المراد تقييم التوزيع عندها"
			},
			{
				name: "deg_freedom",
				description: "عدد صحيح يشير إلى عدد درجات الحرية التي تميز التوزيع"
			},
			{
				name: "tails",
				description: "تحديد عدد أطراف التوزيع للإرجاع: توزيع وحيد الطرف = 1؛ توزيع ثنائي الطرف = 2"
			}
		]
	},
	{
		name: "TEXT",
		description: "تحويل قيمة إلى نص بتنسيق رقم محدد.",
		arguments: [
			{
				name: "value",
				description: "رقم، أو صيغة تقيّم إلى قيمة رقمية، أو مرجع لخلية تحتوي على قيمة رقمية"
			},
			{
				name: "format_text",
				description: "تنسيق رقمي بشكل نصي من مربع 'الفئة' على علامة التبويب 'رقم' في مربع الحوار 'تنسيق الخلايا' (عدا 'عام')"
			}
		]
	},
	{
		name: "TIME",
		description: "تحويل الساعات والدقائق والثواني التي تكتب بالأرقام إلى أرقام Spreadsheet التسلسلية منسقة بتنسيق وقتي.",
		arguments: [
			{
				name: "hour",
				description: "رقم من 0 إلى 23 يمثل الساعة"
			},
			{
				name: "minute",
				description: "رقم من 0 إلى 59 يمثل الدقيقة"
			},
			{
				name: "second",
				description: "رقم من 0 إلى 59 يمثل الثانية"
			}
		]
	},
	{
		name: "TIMEVALUE",
		description: "تحويل الوقت النصي إلى أرقام Spreadsheet التسلسلية، رقم من 0 (12:00:00 ص) إلى 0.999988426 (11:59:29 م). تنسيق الأرقام بالتنسيق الوقتي بعد إدخال الصيغة.",
		arguments: [
			{
				name: "time_text",
				description: "سلسلة نصية تعطي الوقت بإحدى تنسيقات الوقت في Spreadsheet (يتم تجاهل معلومات التاريخ في السلسلة)"
			}
		]
	},
	{
		name: "TINV",
		description: "إرجاع عكس توزيع t للطالب ثنائي الطرف.",
		arguments: [
			{
				name: "probability",
				description: "الاحتمال المقترن بتوزيع t للطالب ثنائي الطرف، رقم بين 0 و1 ضمنًا"
			},
			{
				name: "deg_freedom",
				description: "عدد صحيح موجب يشير إلى عدد درجات الحرية التي تميز التوزيع"
			}
		]
	},
	{
		name: "TODAY",
		description: "إرجاع التاريخ الحالي الذي تم تنسيقه كتاريخ.",
		arguments: [
		]
	},
	{
		name: "TRANSPOSE",
		description: "تحويل نطاق عمودي من الخلايا إلى نطاق أفقي، أو العكس.",
		arguments: [
			{
				name: "array",
				description: "نطاق من الخلايا على ورقة عمل أو صفيف من القيم التي تريد تبديل موضعه"
			}
		]
	},
	{
		name: "TREND",
		description: "إرجاع الأرقام الموجودة بالاتجاه الخطي المطابق لنقاط البيانات المعروفة، باستخدام طريقة المربعات الصغرى.",
		arguments: [
			{
				name: "known_y's",
				description: "نطاق أو صفيف من قيم ص التي تعرفها مسبقاً من العلاقة y = mx + b"
			},
			{
				name: "known_x's",
				description: "نطاق اختياري أو صفيف اختياري من قيم س التي تعرفها من العلاقة y = mx + b، صفيف بنفس حجم قيم ص المعروفة"
			},
			{
				name: "new_x's",
				description: "نطاق أو صفيف من قيم س جديدة والتي تريد من TREND إرجاع قيم ص المطابقة لها"
			},
			{
				name: "const",
				description: "قيمة منطقية: العدد الثابت b يحسب بشكل عادي إذا كان Const = TRUE أو مهملاً؛ ويعين b مساوياً للصفر إذا كان Const = FALSE"
			}
		]
	},
	{
		name: "TRIM",
		description: "إزالة كافة الفراغات من سلسلة نصية باستثناء الفراغات المفردة بين الكلمات.",
		arguments: [
			{
				name: "text",
				description: "النص الذي تريد إزالة الفراغات منه"
			}
		]
	},
	{
		name: "TRIMMEAN",
		description: "إرجاع وسط الجزء الداخلي من مجموعة من قيم بيانات.",
		arguments: [
			{
				name: "array",
				description: "النطاق أو الصفيف من القيم المراد اقتطاعه ومعرفة معدله"
			},
			{
				name: "percent",
				description: "العدد الكسري لنقاط البيانات لاستثنائه من أعلى ومن أسفل مجموعة البيانات"
			}
		]
	},
	{
		name: "TRUE",
		description: "إرجاع القيمة المنطقية TRUE.",
		arguments: [
		]
	},
	{
		name: "TRUNC",
		description: "اقتطاع رقم إلى عدد صحيح بإزالة الجزء العشري منه، أو بإزالة الجزء الكسري منه.",
		arguments: [
			{
				name: "number",
				description: "الرقم الذي تريد اقتطاعه"
			},
			{
				name: "num_digits",
				description: "رقم يحدد دقة الاقتطاع، 0 (صفر) إذا أهمل"
			}
		]
	},
	{
		name: "TTEST",
		description: "إرجاع الاحتمال المقترن باختبارات الطالب t-Test.",
		arguments: [
			{
				name: "array1",
				description: "مجموعة البيانات الأولى"
			},
			{
				name: "array2",
				description: "مجموعة البيانات الثانية"
			},
			{
				name: "tails",
				description: "تحديد عدد أطراف التوزيع التي سيتم إرجاعها: توزيع وحيد الطرف = 1؛ توزيع ثنائي الطرف = 2"
			},
			{
				name: "type",
				description: "نوع t-test: زوجي = 1، نموذجان متساويان بالتباين = 2، نموذجين غير متساويين بالتباين = 3"
			}
		]
	},
	{
		name: "TYPE",
		description: "إرجاع رقم صحيح يشير إلى نوع بيانات قيمة ما: رقم = 1؛ نص = 2؛ قيمة منطقية = 4؛ قيمة خطأ = 16؛ صفيف = 64.",
		arguments: [
			{
				name: "value",
				description: "يمكن أن تكون أي قيمة"
			}
		]
	},
	{
		name: "UNICODE",
		description: "إرجاع الرقم (نقطة الرمز) المقابل للحرف الأول من النص.",
		arguments: [
			{
				name: "text",
				description: "الحرف الذي تريد قيمة Unicode له"
			}
		]
	},
	{
		name: "UPPER",
		description: "تحويل كافة أحرف سلسلة نصية إلى أحرف كبيرة.",
		arguments: [
			{
				name: "text",
				description: "النص الذي تريد تحويله إلى أحرف كبيرة،مرجع أو سلسلة نصية"
			}
		]
	},
	{
		name: "VALUE",
		description: "تحويل السلسلة النصية التي تمثل رقماً إلى رقم.",
		arguments: [
			{
				name: "text",
				description: "النص ضمن علامات الاقتباس أو مرجع إلى خلية تحتوي نصاً تريد تحويله"
			}
		]
	},
	{
		name: "VAR",
		description: "تقدير التباين استناداً إلى نموذج (تجاهل القيم المنطقية والنصوص في النموذج).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "من1 إلى 255 وسيطة رقمية مطابقة لنموذج من المحتوى"
			},
			{
				name: "number2",
				description: "من1 إلى 255 وسيطة رقمية مطابقة لنموذج من المحتوى"
			}
		]
	},
	{
		name: "VAR.P",
		description: "حساب التباين استنادًا إلى المحتوى بأكمله (تجاهل القيم المنطقية والنصوص في المحتوى).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "‎من 1 إلى 255 وسيطة رقمية مطابقة للمحتوى"
			},
			{
				name: "number2",
				description: "‎من 1 إلى 255 وسيطة رقمية مطابقة للمحتوى"
			}
		]
	},
	{
		name: "VAR.S",
		description: "تقدير التباين استنادًا إلى نموذج (تجاهل القيم المنطقية والنصوص في النموذج).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "من 1 إلى 255 وسيطة رقمية مطابقة لنموذج محتوى"
			},
			{
				name: "number2",
				description: "من 1 إلى 255 وسيطة رقمية مطابقة لنموذج محتوى"
			}
		]
	},
	{
		name: "VARA",
		description: "تقدير التباين استناداً إلى نموذج، متضمناً القيم المنطقية والنصوص. تأخذ النصوص والقيمة المنطقية FALSE القيمة 0؛ تأخذ القيمة المنطقية TRUE القيمة 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "‎ ‎ من 1 إلى 255 قيمة وسيطات مطابقة لنموذج لمحتوى"
			},
			{
				name: "value2",
				description: "‎ ‎ من 1 إلى 255 قيمة وسيطات مطابقة لنموذج لمحتوى"
			}
		]
	},
	{
		name: "VARP",
		description: "حساب التباين استناداً إلى المحتوى بأكمله (تجاهل القيم المنطقية والنصوص في المحتوى).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "‎ من 1 إلى 255 وسيطة رقمية مطابقة للمحتوى"
			},
			{
				name: "number2",
				description: "‎ من 1 إلى 255 وسيطة رقمية مطابقة للمحتوى"
			}
		]
	},
	{
		name: "VARPA",
		description: "حساب التباين استناداً إلى المحتوى بأكمله، متضمناً القيم المنطقية والنصوص. تأخذ النصوص والقيمة المنطقية FALSE القيمة 0؛ تأخذ القيمة المنطقية TRUE القيمة 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "‎ من 1 إلى 255 قيمة وسيطات مطابقة لمحتوى"
			},
			{
				name: "value2",
				description: "‎ من 1 إلى 255 قيمة وسيطات مطابقة لمحتوى"
			}
		]
	},
	{
		name: "VDB",
		description: "إرجاع إهلاك الموجودات لفترة محددة، مع تضمين فترات جزئية، باستخدام طريقة الإهلاك المتناقص المزدوج أو باستخدام طرق أخرى أنت تحددها.",
		arguments: [
			{
				name: "cost",
				description: "التكلفة الأولية للموجودات"
			},
			{
				name: "salvage",
				description: "قيمة الخردة عند نهاية عمر الموجودات"
			},
			{
				name: "life",
				description: "عدد الفترات التي سيتم فيها إهلاك الموجودات (تسمى في بعض الأحيان العمر الإنتاجي للموجودات)"
			},
			{
				name: "start_period",
				description: "الفترة الأولى التي تريد حساب الإهلاك الخاص بها، بنفس وحدات العمر الخاص بالفترة"
			},
			{
				name: "end_period",
				description: "الفترة الأخيرة التي تريد حساب الإهلاك الخاص بها، بنفس وحدات العمر بالنسبة للفترة"
			},
			{
				name: "factor",
				description: "النسبة التي تتناقص الميزانية عندها، 2 (المتناقص المزدوج) إذا أهمل"
			},
			{
				name: "no_switch",
				description: "تبديل إلى إهلاك ثابت عندما يكون الإهلاك أكبر من تناقص الميزانية = FALSE أو مهملاً؛ لا تبديل = TRUE"
			}
		]
	},
	{
		name: "VLOOKUP",
		description: "البحث عن قيمة في العمود في أقصى اليسار من جدول، ثم إرجاع قيمة في نفس الصف من عمود تحدده أنت. يجب أن يتم فرز الجدول بشكل افتراضي بترتيب تصاعدي.",
		arguments: [
			{
				name: "lookup_value",
				description: "القيمة المراد العثور عليها في العمود الأول من الجدول، ويمكن أن تكون قيمة، أو مرجعاً، أو سلسلة نصية"
			},
			{
				name: "table_array",
				description: "جدول من نصوص، أو من أرقام، أو من قيم منطقية، يتم استرداد البيانات فيه. يمكن لـ Table_array أن يكون إما مرجعاً لنطاق أو لاسم نطاق"
			},
			{
				name: "col_index_num",
				description: "رقم العمود في table_array الذي يجب إرجاع القيمة المطابقة منه. العمود الأول من القيم في الجدول هو العمود 1"
			},
			{
				name: "range_lookup",
				description: "قيمة منطقية: العثور على الأكثر تطابقاً في العمود الأول (تم فرزه بترتيب تصاعدي) = TRUE أو مهمل؛ العثور على التطابق التام = FALSE"
			}
		]
	},
	{
		name: "WEEKDAY",
		description: "إرجاع رقم من 1 إلى 7 يحدد أحد أيام الأسبوع.",
		arguments: [
			{
				name: "serial_number",
				description: "رقم يمثل التاريخ"
			},
			{
				name: "return_type",
				description: "رقم: الأحد=1 إلى السبت=7، استخدم 1؛ حيث الاثنين=1 إلى الأحد=7، استخدم 2؛ حيث الاثنين=0 إلى الأحد=6، استخدم 3"
			}
		]
	},
	{
		name: "WEEKNUM",
		description: "إرجاع رقم الأسبوع في السنة.",
		arguments: [
			{
				name: "serial_number",
				description: "هو رمز التاريخ-الوقت المستخدم من قبل Spreadsheet لحساب التاريخ والوقت"
			},
			{
				name: "return_type",
				description: "هو رقم (1 أو 2) يحدد نوع القيمة المرجعة"
			}
		]
	},
	{
		name: "WEIBULL",
		description: "إرجاع توزيع Weibull.",
		arguments: [
			{
				name: "x",
				description: "القيمة المراد تقييم الدالة عندها، رقم غير سالب"
			},
			{
				name: "alpha",
				description: "معلمة للتوزيع، عدد موجب"
			},
			{
				name: "beta",
				description: "معلمة للتوزيع، عدد موجب"
			},
			{
				name: "cumulative",
				description: "قيمة منطقية: من أجل دالة التوزيع التراكمي، استخدم TRUE؛ من أجل دالة الاحتمالات غير التراكمية، استخدم FALSE"
			}
		]
	},
	{
		name: "WEIBULL.DIST",
		description: "إرجاع توزيع Weibull.",
		arguments: [
			{
				name: "x",
				description: "القيمة المراد تقييم الدالة عندها، رقم غير سالب"
			},
			{
				name: "alpha",
				description: "معلمة للتوزيع، عدد موجب"
			},
			{
				name: "beta",
				description: "معلمة للتوزيع، عدد موجب"
			},
			{
				name: "cumulative",
				description: "قيمة منطقية: من أجل دالة التوزيع التراكمي، استخدم TRUE؛ من أجل دالة الاحتمالات غير التراكمية، استخدم FALSE"
			}
		]
	},
	{
		name: "WORKDAY",
		description: "إرجاع الرقم المتسلسل لتاريخ يأتي قبل أو بعد عدد معين من أيام العمل.",
		arguments: [
			{
				name: "start_date",
				description: "هو رقم متسلسل للتاريخ يمثل تاريخ البدء"
			},
			{
				name: "days",
				description: "هو عدد الأيام التي ليست عطلة نهاية الأسبوع أو ليست عطلات رسمية أخرى والتي تأتي قبل أو بعد تاريخ_البدء"
			},
			{
				name: "holidays",
				description: "هو صفيف اختياري يتكون من رقم متسلسل للتاريخ واحد أو أكثر لاستبعاده من تقويم العمل، مثل العطلات الرسمية والأعياد التي يتغير تقويتها كل سنة"
			}
		]
	},
	{
		name: "WORKDAY.INTL",
		description: "إرجاع الرقم التسلسلي للتاريخ الذي يأتي قبل أو بعد عدد معين من أيام العمل باستخدام معلمات مخصصة للعطلة الأسبوعية.",
		arguments: [
			{
				name: "start_date",
				description: "رقم تسلسلي للتاريخ يمثل تاريخ البدء"
			},
			{
				name: "days",
				description: "عدد الأيام التي ليست عطلة أسبوعية أو ليست عطلات رسمية والتي تأتي قبل أو بعد تاريخ_البدء"
			},
			{
				name: "weekend",
				description: "عدد أو سلسلة تحدد وقت حدوث العطلات الأسبوعية"
			},
			{
				name: "holidays",
				description: "صفيف اختياري يتكون من رقم تسلسلي واحد أو أكثر للتاريخ لاستبعاده من تقويم العمل، مثل العطلات الرسمية والأعياد التي يتغير توقيتها كل سنة"
			}
		]
	},
	{
		name: "XIRR",
		description: "إرجاع المعدل الداخلي لجدول الدفعات النقدية.",
		arguments: [
			{
				name: "values",
				description: "هي سلسلة من الدفعات النقدية التي تقابل جدول تواريخ المدفوعات"
			},
			{
				name: "dates",
				description: "هو جدول تواريخ المدفوعات الذي يقابل مدفوعات الدفعات النقدية"
			},
			{
				name: "guess",
				description: "هو الرقم الذي تتوقع أن يكون قريباُ من نتيجة XIRR"
			}
		]
	},
	{
		name: "XNPV",
		description: "إرجاع القيمة الحالية الصافية لجدول الدفعات النقدية.",
		arguments: [
			{
				name: "rate",
				description: "هو معدل الخصم الذي سيتم تطبيقه على الدفعات النقدية"
			},
			{
				name: "values",
				description: "هي سلسلة الدفعات النقدية التي تقابل جدول المدفوعات بالتواريخ"
			},
			{
				name: "dates",
				description: "هو جدول تواريخ المدفوعات الذي يقابل مدفوعات الدفعات النقدية"
			}
		]
	},
	{
		name: "XOR",
		description: "إرجاع 'Exclusive Or' منطقية لكل الوسيطات.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logical1",
				description: "من 1 إلى 254 شرطاً تريد اختبارها ويمكن أن تكون قيمتها TRUE أو FALSE ويمكن أن تكون قيماً منطقية أو صفائف أو مراجع"
			},
			{
				name: "logical2",
				description: "من 1 إلى 254 شرطاً تريد اختبارها ويمكن أن تكون قيمتها TRUE أو FALSE ويمكن أن تكون قيماً منطقية أو صفائف أو مراجع"
			}
		]
	},
	{
		name: "YEAR",
		description: "إرجاع السنة، وهي عبارة عن رقم صحيح ضمن النطاق 1900 - 9999 .",
		arguments: [
			{
				name: "serial_number",
				description: "رقم في التعليمات البرمجية الخاصة بالتاريخ والوقت المستخدم من قبل Spreadsheet"
			}
		]
	},
	{
		name: "YEARFRAC",
		description: "إرجاع كسر السنة الذي يمثل عدد الأيام الكاملة بين تاريخ_البدء وتاريخ_الانتهاء.",
		arguments: [
			{
				name: "start_date",
				description: "هو رقم تاريخ متسلسل يمثل تاريخ البدء"
			},
			{
				name: "end_date",
				description: "هو رقم التاريخ المتسلسل الذي يمثل تاريخ الانتهاء"
			},
			{
				name: "basis",
				description: "هو نوع الأساس المستخدم لحساب الأيام"
			}
		]
	},
	{
		name: "YIELDDISC",
		description: "إرجاع العائد السنوي لورقة مالية ذات خصم. على سبيل المثال، إذون الخزانة.",
		arguments: [
			{
				name: "settlement",
				description: "هو تاريخ تسوية الأوراق المالية، يتم التعبير عنه كرقم متسلسل للتاريخ"
			},
			{
				name: "maturity",
				description: "هو تاريخ استحقاق الأوراق المالية، يتم التعبير عنه كرقم متسلسل للتاريخ"
			},
			{
				name: "pr",
				description: "هو سعر الورقة المالية لكل قيمة اسمية لـ 100 ر.س."
			},
			{
				name: "redemption",
				description: "هو قيمة الاسترداد الخاصة بالورقة المالية لكل قيمة اسمية لـ 100 ر.س."
			},
			{
				name: "basis",
				description: "هو نوع الأساس المستخدم لحساب الأيام"
			}
		]
	},
	{
		name: "Z.TEST",
		description: "إرجاع قيمة P أحادية الطرف لـ z-test.",
		arguments: [
			{
				name: "array",
				description: "صفيف أو نطاق البيانات التي سيتم اختبار X مقارنة بها"
			},
			{
				name: "x",
				description: "القيمة التي سيتم اختبارها"
			},
			{
				name: "sigma",
				description: "الانحراف المعياري للمحتوى (المعروف). في حالة حذفها، يتم استخدام نموذج الانحراف المعياري"
			}
		]
	},
	{
		name: "ZTEST",
		description: "إرجاع قيمة P أحادية الطرف لـ z-test.",
		arguments: [
			{
				name: "array",
				description: "صفيف أو نطاق البيانات التي سيتم اختبار X مقارنة بها"
			},
			{
				name: "x",
				description: "القيمة التي سيتم اختبارها"
			},
			{
				name: "sigma",
				description: "الانحراف المعياري للمحتوى (المعروف). في حالة حذفها، يتم استخدام نموذج الانحراف المعياري"
			}
		]
	}
];