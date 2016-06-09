ASPxClientSpreadsheet.Functions = [
	{
		name: "ABS",
		description: "Mengembalikan nilai mutlak bagi nombor, nombor yang tanpa tandanya.",
		arguments: [
			{
				name: "number",
				description: "ialah nombor nyata yang anda inginkan nilai mutlaknya"
			}
		]
	},
	{
		name: "ACCRINTM",
		description: "Mengembalikan faedah terakru bagi keselamatan yang membayar faedah setelah matang.",
		arguments: [
			{
				name: "issue",
				description: "ialah tarikh pengeluaran keselamatan, disebut sebagai nombor tarikh bersiri"
			},
			{
				name: "settlement",
				description: "ialah tarikh matang keselamatan, disebut sebagai nombor tarikh bersiri"
			},
			{
				name: "rate",
				description: "ialah kadar kupon tahunan keselamatan"
			},
			{
				name: "par",
				description: "ialah nilai tara keselamatan"
			},
			{
				name: "basis",
				description: "ialah jenis berdasarkan bilangan hari untuk digunakan"
			}
		]
	},
	{
		name: "ACOS",
		description: "Mengembalikan lengkok kosinus nombor, dalam radian dalam julat 0 hingga Pi. Lengkok kosinus ialah sudut yang kosinusnya ialah Nombor.",
		arguments: [
			{
				name: "number",
				description: "ialah kosinus sudut yang anda inginkan dan mestilah daripada -1 hingga 1"
			}
		]
	},
	{
		name: "ACOSH",
		description: "Mengembalikan kosinus hiperbola songsang bagi nombor.",
		arguments: [
			{
				name: "number",
				description: "ialah sebarang nombor nyata sama dengan atau lebih besar daripada 1"
			}
		]
	},
	{
		name: "ACOT",
		description: "Mengembalikan kotangen lengkung nombor dalam radian dalam julat 0 hingga Pi.",
		arguments: [
			{
				name: "number",
				description: "ialah kotangen sudut yang anda inginkan"
			}
		]
	},
	{
		name: "ACOTH",
		description: "Mengembalikan fungsi kotangen hiperbola songsang nombor.",
		arguments: [
			{
				name: "number",
				description: "ialah kotangen hiperbola sudut yang anda inginkan"
			}
		]
	},
	{
		name: "ADDRESS",
		description: "Mencipta sel rujukan sebagai teks, diberikan nombor bagi baris dan lajur tertentu.",
		arguments: [
			{
				name: "row_num",
				description: "ialah nombor baris untuk digunakan dalam rujukan sel: nombor_Baris = 1 untuk baris 1"
			},
			{
				name: "column_num",
				description: "ialah nombor lajur untuk digunakan dalam rujukan sel. Contohnya, nombor_Lajur = 4 untuk lajur D"
			},
			{
				name: "abs_num",
				description: "menentukan jenis rujukan: mutlak = 1; baris mutlak/lajur relatif = 2; baris relatif/lajur mutlak = 3; relatif = 4"
			},
			{
				name: "a1",
				description: "ialah nilai logik yang menentukan gaya rujukan: gaya A1 = 1 atau BENAR; gaya R1C1 = 0 atau PALSU"
			},
			{
				name: "sheet_text",
				description: "ialah teks yang menentukan nama lembaran kerja untuk digunakan sebagai rujukan luaran"
			}
		]
	},
	{
		name: "AND",
		description: "Menyemak sama ada semua argumen adalah BENAR, dan mengembalikan BENAR jika semua argumen adalah BENAR.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logical1",
				description: "ialah 1 hingga 255 syarat yang ingin anda uji sama ada BENAR atau PALSU dan mungkin nilai logik, tatasusunan atau rujukan"
			},
			{
				name: "logical2",
				description: "ialah 1 hingga 255 syarat yang ingin anda uji sama ada BENAR atau PALSU dan mungkin nilai logik, tatasusunan atau rujukan"
			}
		]
	},
	{
		name: "ARABIC",
		description: "Menukar angka Roman kepada Arab.",
		arguments: [
			{
				name: "text",
				description: "ialah angka Roman yang anda ingin tukarkan"
			}
		]
	},
	{
		name: "AREAS",
		description: "Mengembalikan bilangan kawasan dalam rujukan. Kawasan ialah julat sel bersebelahan atau sel tunggal.",
		arguments: [
			{
				name: "reference",
				description: "ialah rujukan kepada sel atau julat dan boleh dirujuk kepada kawasan berbilang"
			}
		]
	},
	{
		name: "ASIN",
		description: "Mengembalikan lengkok sinus nombor dalam radian, dalam julat -Pi/2 hingga Pi/2.",
		arguments: [
			{
				name: "number",
				description: "ialah sinus sudut yang anda inginkan dan mestilah daripada -1 hingga 1"
			}
		]
	},
	{
		name: "ASINH",
		description: "Mengembalikan sinus hiperbola songsang bagi nombor.",
		arguments: [
			{
				name: "number",
				description: "ialah sebarang nombor nyata sama dengan atau lebih besar daripada 1"
			}
		]
	},
	{
		name: "ATAN",
		description: "Mengembalikan lengkok tangen bagi nombor dalam radian, dalam julat -Pi/2 ke Pi/2.",
		arguments: [
			{
				name: "number",
				description: "ialah tangen bagi sudut yang anda inginkan"
			}
		]
	},
	{
		name: "ATAN2",
		description: "Mengembalikan lengkok tangen bagi koordinat x- dan y- tertentu, dalam radian antara -Pi dan Pi, tidak termasuk -Pi.",
		arguments: [
			{
				name: "x_num",
				description: "ialah koordinat x bagi titik"
			},
			{
				name: "y_num",
				description: "ialah koordinat y bagi titik"
			}
		]
	},
	{
		name: "ATANH",
		description: "Mengembalikan tangen hiperbola songsang bagi nombor.",
		arguments: [
			{
				name: "number",
				description: "ialah sebarang nombor nyata antara -1 dan 1 tidak termasuk -1 dan 1"
			}
		]
	},
	{
		name: "AVEDEV",
		description: "Mengembalikan purata sisihan mutlak bagi titik data daripada min. Argumen mungkin nombor atau nama, tatasusunan, atau rujukan yang mengandungi nombor.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "ialah 1 hingga 255 argumen yang anda inginkan purata sisihan mutlaknya"
			},
			{
				name: "number2",
				description: "ialah 1 hingga 255 argumen yang anda inginkan purata sisihan mutlaknya"
			}
		]
	},
	{
		name: "AVERAGE",
		description: "Mengembalikan purata (min aritmetik) argumen, mungkin nombor atau nama, tatasusunan, atau rujukan yang mengandungi nombor.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "ialah 1 hingga 255 argumen angka yang anda inginkan puratanya"
			},
			{
				name: "number2",
				description: "ialah 1 hingga 255 argumen angka yang anda inginkan puratanya"
			}
		]
	},
	{
		name: "AVERAGEA",
		description: "Mengembalikan purata (min aritmetik) argumen, menilai teks dan PALSU dalam argumen sebagai 0; BENAR dinilai sebagai 1. Argumen mungkin nombor, nama, tatasusunan atau rujukan.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "ialah 1 hingga 255 argumen yang anda inginkan puratanya"
			},
			{
				name: "value2",
				description: "ialah 1 hingga 255 argumen yang anda inginkan puratanya"
			}
		]
	},
	{
		name: "AVERAGEIF",
		description: "Cari purata (min aritmetik) bagi sel yang ditentukan oleh syarat atau kriteria yang diberikan.",
		arguments: [
			{
				name: "range",
				description: "ialah julat bagi sel yang anda ingin ia dinilai"
			},
			{
				name: "criteria",
				description: "ialah syarat atau kriteria dalam bentuk nombor, ungkapan, atau teks yang mentakrifkan sel mana akan digunakan untuk mencari purata"
			},
			{
				name: "average_range",
				description: "ialah sel sebenar yang akan digunakan untuk mencari purata. Jika diabaikan, sel dalam julat akan digunakan "
			}
		]
	},
	{
		name: "AVERAGEIFS",
		description: "Cari purata (min aritmetik) bagi sel yang ditentukan oleh syarat atau kriteria yang diberikan.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "average_range",
				description: "adalah sel sebenar yang akan digunakan untuk mencari purata."
			},
			{
				name: "criteria_range",
				description: "ialah julat sel yang anda ingin ia dinilai bagi syarat tertentu"
			},
			{
				name: "criteria",
				description: "ialah syarat atau kriteria dalam bentuk nombor, ungkapan, atau teks yang mentakrifkan sel mana yang akan digunakan untuk mencari purata"
			}
		]
	},
	{
		name: "BAHTTEXT",
		description: "Tukarkan nombor kepada teks (baht).",
		arguments: [
			{
				name: "number",
				description: "ialah nombor yang ingin anda tukar"
			}
		]
	},
	{
		name: "BASE",
		description: "Menukar nombor kepada gambaran teks dengan radiks yang diberikan (asas).",
		arguments: [
			{
				name: "number",
				description: "ialah nombor yang anda ingin tukar"
			},
			{
				name: "radix",
				description: "ialah Radiks asas yang anda ingin tukarkan nombor tersebut"
			},
			{
				name: "min_length",
				description: "ialah kepanjangan minimum bagi rentetan yang dikembalikan.  Jika diabaikan, pendahulu sifar tidak ditambah"
			}
		]
	},
	{
		name: "BESSELI",
		description: "Mengembalikan fungsi Bessel In(x) yang diubah suai.",
		arguments: [
			{
				name: "x",
				description: "ialah nilai pada mana untuk menilai fungsi"
			},
			{
				name: "n",
				description: "ialah tertib bagi fungsi Bessel"
			}
		]
	},
	{
		name: "BESSELJ",
		description: "Mengembalikan fungsi Bessel Jn(x).",
		arguments: [
			{
				name: "x",
				description: "ialah nilai pada mana untuk menilai fungsi"
			},
			{
				name: "n",
				description: "ialah tertib bagi fungsi Bessel"
			}
		]
	},
	{
		name: "BESSELK",
		description: "Mengembalikan fungsi Bessel Kn(x) yang diubah suai.",
		arguments: [
			{
				name: "x",
				description: "ialah nilai pada mana untuk menilai fungsi"
			},
			{
				name: "n",
				description: "ialah tertib bagi fungsi"
			}
		]
	},
	{
		name: "BESSELY",
		description: "Mengembalikan fungsi Bessel Yn(x).",
		arguments: [
			{
				name: "x",
				description: "ialah nilai pada mana untuk menilai fungsi"
			},
			{
				name: "n",
				description: "ialah tertib bagi fungsi"
			}
		]
	},
	{
		name: "BETA.DIST",
		description: "Mengembalikan fungsi taburan kebarangkalian beta.",
		arguments: [
			{
				name: "x",
				description: "ialah nilai antara A dan B pada mana untuk menilai fungsi"
			},
			{
				name: "alpha",
				description: "ialah parameter bagi taburan dan mestilah lebih besar daripada 0"
			},
			{
				name: "beta",
				description: "ialah parameter bagi taburan dan mesti lebih besar 0"
			},
			{
				name: "cumulative",
				description: "ialah nilai logik: bagi fungsi taburan kumulatif, gunakan BENAR; BAGI fungsi kepadatan kebarangkalian, gunakan PALSU"
			},
			{
				name: "A",
				description: "ialah batas bawah pilihan pada selang x. Jika diabaikan, A = 0"
			},
			{
				name: "B",
				description: " ialah batas atas pilihan pada selang x. Jika diabaikan, B = 1"
			}
		]
	},
	{
		name: "BETA.INV",
		description: "Mengembalikan songsangan fungsi kepadatan kebarangkalian beta kumulatif (BETA.DIST).",
		arguments: [
			{
				name: "probability",
				description: "ialah kebarangkalian yang dikaitkan dengan taburan beta"
			},
			{
				name: "alpha",
				description: "ialah parameter bagi taburan dan mestilah lebih besar daripada 0"
			},
			{
				name: "beta",
				description: "ialah parameter bagi taburan dan mestilah lebih besar daripada 0"
			},
			{
				name: "A",
				description: "ialah batas bawah pilihan pada selang x. Jika diabaikan, A = 0"
			},
			{
				name: "B",
				description: "ialah batas atas pilihan pada selang x. Jika diabaikan, B = 1"
			}
		]
	},
	{
		name: "BETADIST",
		description: "Mengembalikan fungsi kepadatan kebarangkalian beta kumulatif.",
		arguments: [
			{
				name: "x",
				description: "ialah nilai antara A dan B untuk menilai fungsi"
			},
			{
				name: "alpha",
				description: "ialah parameter bagi pengagihan dan mestilah lebih besar daripada 0"
			},
			{
				name: "beta",
				description: "ialah parameter bagi pengagihan dan mestilah lebih besar daripada 0"
			},
			{
				name: "A",
				description: "ialah pilihan batas bawah pada selang x. Jika diabaikan, A = 0"
			},
			{
				name: "B",
				description: "ialah pilihan batas atas pada selang x. Jika diabaikan, B = 1"
			}
		]
	},
	{
		name: "BETAINV",
		description: "Mengembalikan songsangan fungsi kepadatan kebarangkalian beta kumulatif (BETADIST).",
		arguments: [
			{
				name: "probability",
				description: "ialah kebarangkalian yang dikaitkan dengan taburan beta"
			},
			{
				name: "alpha",
				description: "ialah parameter taburan dan mestilah lebih besar daripada 0"
			},
			{
				name: "beta",
				description: "ialah parameter taburan dan mestilah lebih besar daripada 0"
			},
			{
				name: "A",
				description: "ialah pilihan batas bawah pada selang x. Jika diabaikan, A = 0"
			},
			{
				name: "B",
				description: "ialah pilihan batas atas pada selang x. Jika diabaikan, B = 1"
			}
		]
	},
	{
		name: "BIN2DEC",
		description: "Menukar nombor perduaan kepada perpuluhan.",
		arguments: [
			{
				name: "number",
				description: "ialah nombor perduaan yang ingin anda tukar"
			}
		]
	},
	{
		name: "BIN2HEX",
		description: "Menukar nombor perduaan kepada perenambelasan.",
		arguments: [
			{
				name: "number",
				description: "ialah nombor perduaan yang ingin anda tukar"
			},
			{
				name: "places",
				description: "ialah bilangan aksara untuk digunakan"
			}
		]
	},
	{
		name: "BIN2OCT",
		description: "Menukar nombor perduaan kepada perlapanan.",
		arguments: [
			{
				name: "number",
				description: "ialah nombor perduaan yang ingin anda tukar"
			},
			{
				name: "places",
				description: "ialah bilangan aksara untuk digunakan"
			}
		]
	},
	{
		name: "BINOM.DIST",
		description: "Mengembalikan kebarangkalian taburan binomial tempoh individu.",
		arguments: [
			{
				name: "number_s",
				description: "ialah bilangan kejayaan dalam percubaan"
			},
			{
				name: "trials",
				description: "ialah bilangan percubaan bebas"
			},
			{
				name: "probability_s",
				description: "ialah kebarangkalian kejayaan dalam setiap percubaan"
			},
			{
				name: "cumulative",
				description: "ialah nilai logik: bagi fungsi taburan kumulatif, gunakan BENAR; bagi fungsi massa kebarangkalian, gunakan PALSU"
			}
		]
	},
	{
		name: "BINOM.DIST.RANGE",
		description: "Mengembalikan kebarangkalian hasil percubaan menggunakan taburan binomial.",
		arguments: [
			{
				name: "trials",
				description: "ialah bilangan percubaan bebas"
			},
			{
				name: "probability_s",
				description: "ialah kebarangkalian kejayaan pada setiap percubaan"
			},
			{
				name: "number_s",
				description: "ialah bilangan kejayaan dalam percubaan"
			},
			{
				name: "number_s2",
				description: "jika dibekalkan fungsi ini mengembalikan kebarangkalian yang bilangan percubaan yang berjaya akan terletak di antara nombor_s dan nombor_s2"
			}
		]
	},
	{
		name: "BINOM.INV",
		description: "Mengembalikan nilai terkecil apabila taburan binomial kumulatif lebih besar daripada atau sama dengan nilai kriteria.",
		arguments: [
			{
				name: "trials",
				description: "ialah bilangan percubaan Bernoulli"
			},
			{
				name: "probability_s",
				description: "ialah kebarangkalian kejayaan setiap percubaan, termasuk nombor 0 hingga 1"
			},
			{
				name: "alpha",
				description: "ialah nilai kriteria, termasuk nombor 0 hingga 1"
			}
		]
	},
	{
		name: "BINOMDIST",
		description: "Mengembalikan kebarangkalian taburan binomial tempoh individu.",
		arguments: [
			{
				name: "number_s",
				description: "adalah bilangan kejayaan dalam percubaan"
			},
			{
				name: "trials",
				description: "ialah bilangan percubaan bebas"
			},
			{
				name: "probability_s",
				description: "adalah kebarangkalian kejayaan dalam setiap percubaan"
			},
			{
				name: "cumulative",
				description: "ialah nilai logik: bagi fungsi taburan kumulatif, gunakan TRUE; bagi fungsi jisim kebarangkalian, gunakan FALSE"
			}
		]
	},
	{
		name: "BITAND",
		description: "Mengembalikan bit 'Dan' bagi dua nombor.",
		arguments: [
			{
				name: "number1",
				description: "ialah wakil perpuluhan bagi nombor binari yang anda ingin menilai"
			},
			{
				name: "number2",
				description: "ialah wakil perpuluhan bagi nombor binari yang anda ingin menilai"
			}
		]
	},
	{
		name: "BITLSHIFT",
		description: "Mengembalikan nombor yang dianjak ke kiri oleh bit anjak_amoun.",
		arguments: [
			{
				name: "number",
				description: "ialah wakil perpuluhan nombor binari yang anda ingin menilai"
			},
			{
				name: "shift_amount",
				description: "ialah nombor bagi bit yang anda ingin anjak Nombor ke kiri"
			}
		]
	},
	{
		name: "BITOR",
		description: "Mengembalikan bit 'Atau' bagi dua nombor.",
		arguments: [
			{
				name: "number1",
				description: "ialah wakil perpuluhan bagi nombor binari yang anda ingin menilai"
			},
			{
				name: "number2",
				description: "ialah wakil perpuluhan bagi nombor binari yang anda ingin menilai"
			}
		]
	},
	{
		name: "BITRSHIFT",
		description: "Mengembalikan nombor yang dianjak ke kanan oleh bit shift_amount.",
		arguments: [
			{
				name: "number",
				description: "adalah perwakilan perpuluhan bagi nombor perduaan yang anda ingin untuk menilai"
			},
			{
				name: "shift_amount",
				description: "adalah nombor bit yang anda ingin untuk menganjak Number ke kanan"
			}
		]
	},
	{
		name: "BITXOR",
		description: "Mengembalikan bit 'Eksklusif Atau' bagi dua nombor.",
		arguments: [
			{
				name: "number1",
				description: "ialah wakil perpuluhan bagi nombor binari yang anda ingin menilai"
			},
			{
				name: "number2",
				description: "ialah wakil perpuluhan bagi nombor binari yang anda ingin menilai"
			}
		]
	},
	{
		name: "CEILING",
		description: "Membundarkan nombor ke atas, kepada gandaan bererti yang terdekat.",
		arguments: [
			{
				name: "number",
				description: "ialah nilai yang ingin anda bundarkan"
			},
			{
				name: "significance",
				description: "ialah gandaan yang ingin anda bundarkan"
			}
		]
	},
	{
		name: "CEILING.MATH",
		description: "Membundar naik nombor kepada integer terdekat atau kepada gandaan bererti yang terdekat.",
		arguments: [
			{
				name: "number",
				description: "ialah nilai yang anda ingin bundarkan"
			},
			{
				name: "significance",
				description: "ialah gandaan yang ingin anda bundarkan"
			},
			{
				name: "mode",
				description: "apabila diberi dan bukan sifar fungsi ini akan membundar menjauhi sifar"
			}
		]
	},
	{
		name: "CEILING.PRECISE",
		description: "Membundarkan nombor ke atas, kepada integer atau gandaan bererti yang terdekat.",
		arguments: [
			{
				name: "number",
				description: "ialah nilai yang ingin anda bundarkan"
			},
			{
				name: "significance",
				description: "ialah gandaan yang ingin anda bundarkan"
			}
		]
	},
	{
		name: "CELL",
		description: "Mengembalikan maklumat mengenai pemformatan, lokasi, atau kandungan bagi sel pertama, menurut tertib baca helaian, dalam rujukan.",
		arguments: [
			{
				name: "info_type",
				description: "ialah nilai teks yang menentukan jenis maklumat sel yang anda inginkan."
			},
			{
				name: "reference",
				description: "ialah sel yang anda inginkan maklumatnya"
			}
		]
	},
	{
		name: "CHAR",
		description: "Mengembalikan aksara yang ditentukan oleh nombor kod daripada set aksara komputer anda.",
		arguments: [
			{
				name: "number",
				description: "ialah nombor antara 1 hingga 255 yang menentukan aksara mana yang anda inginkan"
			}
		]
	},
	{
		name: "CHIDIST",
		description: "Mengembalikan kebarangkalian satu ekor bagi taburan chi-kuasa dua.",
		arguments: [
			{
				name: "x",
				description: "ialah nilai yang anda ingin menilai taburan, nombor bukan negatif"
			},
			{
				name: "deg_freedom",
				description: "ialah bilangan darjah kebebasan, nombor antara 1 hingga 10^10, tidak termasuk 10^10"
			}
		]
	},
	{
		name: "CHIINV",
		description: "Mengembalikan songsangan kebarangkalian satu ekor bagi taburan chi-kuasa dua.",
		arguments: [
			{
				name: "probability",
				description: "ialah kebarangkalian yang dikaitkan dengan taburan chi-kuasa dua, termasuk nilai 0 hingga 1"
			},
			{
				name: "deg_freedom",
				description: "ialah bilangan darjah kebebasan, nombor antara 1 dan 10^10, tidak termasuk 10^10"
			}
		]
	},
	{
		name: "CHISQ.DIST",
		description: "Mengembalikan kebarangkalian berekor kiri bagi taburan chi kuasa dua.",
		arguments: [
			{
				name: "x",
				description: "ialah nilai yang anda ingin menilai taburan, nombor bukan negatif"
			},
			{
				name: "deg_freedom",
				description: "ialah bilangan darjah kebebasan, nombor antara 1 hingga 10^10, tidak termasuk 10^10"
			},
			{
				name: "cumulative",
				description: "ialah nilai logik bagi fungsi untuk dikembalikan: fungsi taburan kumulatif = BENAR; fungsi kepadatan kebarangkalian = PALSU"
			}
		]
	},
	{
		name: "CHISQ.DIST.RT",
		description: "Mengembalikan kebarangkalian berekor kanan bagi taburan chi kuasa dua.",
		arguments: [
			{
				name: "x",
				description: "ialah nilai yang anda ingin menilai taburan, nombor bukan negatif"
			},
			{
				name: "deg_freedom",
				description: "ialah bilangan darjah kebebasan, nombor antara 1 hingga 10^10, tidak termasuk 10^10"
			}
		]
	},
	{
		name: "CHISQ.INV",
		description: "Mengembalikan songsangan kebarangkalian berekor kiri bagi taburan chi kuasa dua.",
		arguments: [
			{
				name: "probability",
				description: "ialah kebarangkalian yang dikaitkan dengan taburan chi kuasa dua, termasuk nilai 0 hingga 1"
			},
			{
				name: "deg_freedom",
				description: "ialah bilangan darjah kebebasan, nombor antara 1 dan 10^10, tidak termasuk 10^10"
			}
		]
	},
	{
		name: "CHISQ.INV.RT",
		description: "Mengembalikan songsangan kebarangkalian berekor kanan bagi taburan chi kuasa dua.",
		arguments: [
			{
				name: "probability",
				description: "ialah kebarangkalian yang dikaitkan dengan taburan chi kuasa dua, termasuk nilai 0 hingga 1"
			},
			{
				name: "deg_freedom",
				description: "ialah bilangan darjah kebebasan, nombor antara 1 dan 10^10, tidak termasuk 10^10"
			}
		]
	},
	{
		name: "CHISQ.TEST",
		description: "Mengembalikan ujian bebas: nilai daripada taburan chi-kuasa dua bagi statistik dan darjah kebebasan yang sesuai.",
		arguments: [
			{
				name: "actual_range",
				description: "ialah julat data yang mengandungi pemerhatian untuk ujian terhadap nilai yang dijangkakan"
			},
			{
				name: "expected_range",
				description: "ialah julat data yang mengandungi nisbah produk bagi jumlah baris dan lajur kepada jumlah besar"
			}
		]
	},
	{
		name: "CHITEST",
		description: "Mengembalikan ujian bebas: nilai daripada taburan chi-kuasa dua bagi statistik dan darjah kebebasan yang sesuai.",
		arguments: [
			{
				name: "actual_range",
				description: "ialah julat data yang mengandungi pemerhatian untuk ujian terhadap nilai yang dijangkakan"
			},
			{
				name: "expected_range",
				description: "ialah julat data yang mengandungi nisbah produk bagi jumlah baris dan lajur kepada jumlah besar"
			}
		]
	},
	{
		name: "CHOOSE",
		description: "Memilih nilai atau tindakan untuk dilaksanakan daripada senarai nilai, berdasarkan nombor indeks.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "index_num",
				description: "menentukan nilai argumen yang dipilih. nombor_Indeks mestilah antara 1 hingga 254, atau formula atau rujukan kepada nombor antara 1 hingga 254"
			},
			{
				name: "value1",
				description: "ialah 1 hingga 254 nombor, rujukan sel, nama takrifan, formula, fungsi, atau argumen teks yang dipilih oleh PILIH"
			},
			{
				name: "value2",
				description: "ialah 1 hingga 254 nombor, rujukan sel, nama takrifan, formula, fungsi, atau argumen teks yang dipilih oleh PILIH"
			}
		]
	},
	{
		name: "CLEAN",
		description: "Alih keluar semua aksara tak boleh cetak daripada teks.",
		arguments: [
			{
				name: "text",
				description: "ialah sebarang maklumat lembaran kerja dari mana anda ingin alih keluar aksara tak boleh cetak"
			}
		]
	},
	{
		name: "CODE",
		description: "Mengembalikan kod angka bagi aksara pertama dalam rentetan teks, dalam set aksara yang digunakan oleh komputer anda.",
		arguments: [
			{
				name: "text",
				description: "ialah teks yang anda inginkan kod aksara pertama"
			}
		]
	},
	{
		name: "COLUMN",
		description: "Mengembalikan nombor lajur rujukan.",
		arguments: [
			{
				name: "reference",
				description: "ialah sel atau julat sel bersebelahan yang anda inginkan nombor lajurnya. Jika diabaikan, sel yang mengandungi fungsi LAJUR digunakan"
			}
		]
	},
	{
		name: "COLUMNS",
		description: "Mengembalikan bilangan lajur dalam tatasusunan atau rujukan.",
		arguments: [
			{
				name: "array",
				description: "ialah tatasusunan atau formula tatasusunan, atau rujukan kepada julat sel yang anda inginkan bilangan lajur"
			}
		]
	},
	{
		name: "COMBIN",
		description: "Mengembalikan bilangan penggabungan bagi bilangan item yang diberikan.",
		arguments: [
			{
				name: "number",
				description: "ialah jumlah bilangan item"
			},
			{
				name: "number_chosen",
				description: "ialah bilangan item dalam setiap penggabungan"
			}
		]
	},
	{
		name: "COMBINA",
		description: "Mengembalikan bilangan kombinasi dengan ulangan bagi bilangan item tertentu.",
		arguments: [
			{
				name: "number",
				description: "ialah jumlah item"
			},
			{
				name: "number_chosen",
				description: "ialah bilangan item dalam setiap kombinasi"
			}
		]
	},
	{
		name: "COMPLEX",
		description: "Menukar pekali nyata dan khayalan ke dalam nombor kompleks.",
		arguments: [
			{
				name: "real_num",
				description: "ialah pekali nyata bagi nombor kompleks"
			},
			{
				name: "i_num",
				description: "ialah pekali khayalan bagi nombor kompleks"
			},
			{
				name: "suffix",
				description: "ialah akhiran untuk komponen khayalan bagi nombor kompleks"
			}
		]
	},
	{
		name: "CONCATENATE",
		description: "Mencantumkan beberapa rentetan teks ke dalam satu rentetan teks.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "text1",
				description: "ialah 1 hingga 255 rentetan teks yang akan dicantumkan dalam rentetan teks tunggal dan mungkin rentetan teks, nombor atau rujukan sel tunggal"
			},
			{
				name: "text2",
				description: "ialah 1 hingga 255 rentetan teks yang akan dicantumkan dalam rentetan teks tunggal dan mungkin rentetan teks, nombor atau rujukan sel tunggal"
			}
		]
	},
	{
		name: "CONFIDENCE",
		description: "Mengembalikan selang keyakinan bagi min populasi.",
		arguments: [
			{
				name: "alpha",
				description: "ialah aras keertian yang digunakan untuk mengira aras keyakinan, nombor yang lebih besar daripada 0 dan kurang daripada 1"
			},
			{
				name: "standard_dev",
				description: "ialah sisihan piawai populasi bagi julat data dan dianggap diketahui. Sisihan_piawai mestilah lebih besar daripada 0"
			},
			{
				name: "size",
				description: "ialah saiz sampel"
			}
		]
	},
	{
		name: "CONFIDENCE.NORM",
		description: "Mengembalikan selang keyakinan untuk min populasi, menggunakan taburan normal.",
		arguments: [
			{
				name: "alpha",
				description: "adalah tahap kepentingan yang digunakan untuk mengira tahap keyakinan, nombor lebih besar daripada 0 dan kurang daripada 1"
			},
			{
				name: "standard_dev",
				description: "adalah sisihan piawai populasi untuk julat data dan adalah dipercayai telah diketahui. Sisihan_piawai mesti lebih besar daripada 0"
			},
			{
				name: "size",
				description: "adalah saiz sampel"
			}
		]
	},
	{
		name: "CONFIDENCE.T",
		description: "Mengembalikan selang keyakinan untuk min populasi, menggunakan taburan T Pelajar.",
		arguments: [
			{
				name: "alpha",
				description: "adalah tahap kepentingan yang digunakan untuk mengira tahap keyakinan, nombor lebih besar daripada 0 dan kurang daripada 1"
			},
			{
				name: "standard_dev",
				description: "adalah sisihan piawai populasi untuk julat data dan adalah dipercayaki telah diketahui. Sisihan_piawai mesti lebih besar daripada 0"
			},
			{
				name: "size",
				description: "adalah saiz sampel"
			}
		]
	},
	{
		name: "CONVERT",
		description: "Menukar satu nombor daripada satu sistem pengukuran kepada yang lain.",
		arguments: [
			{
				name: "number",
				description: "ialah nilai dalam daripada_unit untuk ditukar"
			},
			{
				name: "from_unit",
				description: "ialah unit bagi nombor"
			},
			{
				name: "to_unit",
				description: "ialah unit bagi keputusan"
			}
		]
	},
	{
		name: "CORREL",
		description: "Fungsi ini tersedia untuk keserasian dengan Spreadsheet 2007 dan lebih awal.Mengembalikan pekali korelasi antara dua set data.",
		arguments: [
			{
				name: "array1",
				description: "ialah julat sel nilai. Nilai sepatutnya nombor, nama, tatasusunan, atau rujukan yang mengandungi nombor"
			},
			{
				name: "array2",
				description: "ialah julat nilai sel kedua. Nilai sepatutnya nombor, nama, tatasusunan, atau rujukan yang mengandungi nombor"
			}
		]
	},
	{
		name: "COS",
		description: "Mengembalikan kosinus bagi sudut.",
		arguments: [
			{
				name: "number",
				description: "ialah sudut dalam radian yang anda inginkan nilai kosinusnya"
			}
		]
	},
	{
		name: "COSH",
		description: "Mengembalikan kosinus hiperbola bagi nombor.",
		arguments: [
			{
				name: "number",
				description: "ialah sebarang nombor nyata"
			}
		]
	},
	{
		name: "COT",
		description: "Mengembalikan kotangen bagi sudut.",
		arguments: [
			{
				name: "number",
				description: "ialah sudut dalam radian yang anda inginkan bagi kotangen"
			}
		]
	},
	{
		name: "COTH",
		description: "Mengembalikan kotangen hiperbola bagi nombor.",
		arguments: [
			{
				name: "number",
				description: "ialah sudut dalam radian yang anda inginkan bagi kotangen hiperbola"
			}
		]
	},
	{
		name: "COUNT",
		description: "Kira bilangan sel dalam julat yang mengandungi nombor.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "ialah 1 hingga 255 argumen yang boleh mengandungi atau merujuk pelbagai jenis data berbeza, tetapi hanya nombor yang dikira"
			},
			{
				name: "value2",
				description: "ialah 1 hingga 255 argumen yang boleh mengandungi atau merujuk pelbagai jenis data berbeza, tetapi hanya nombor yang dikira"
			}
		]
	},
	{
		name: "COUNTA",
		description: "Mengira bilangan sel dalam julat yang tidak kosong.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "ialah 1 hingga 255 argumen yang mewakili nilai dan sel yang ingin anda bilang. Nilai boleh terdiri daripada sebarang jenis maklumat"
			},
			{
				name: "value2",
				description: "ialah 1 hingga 255 argumen yang mewakili nilai dan sel yang ingin anda bilang. Nilai boleh terdiri daripada sebarang jenis maklumat"
			}
		]
	},
	{
		name: "COUNTBLANK",
		description: "Membilang bilangan sel kosong dalam julat sel yang ditentukan.",
		arguments: [
			{
				name: "range",
				description: "ialah julat yang ingin anda bilang sel kosongnya"
			}
		]
	},
	{
		name: "COUNTIF",
		description: "Membilang bilangan sel dalam julat yang memenuhi syarat yang diberikan.",
		arguments: [
			{
				name: "range",
				description: "ialah julat sel yang ingin anda bilang sel bukan kosong"
			},
			{
				name: "criteria",
				description: "ialah syarat dalam bentuk nombor, ungkapan, atau teks yang mentakrif sel mana yang akan dibilang"
			}
		]
	},
	{
		name: "COUNTIFS",
		description: "Mengira bilangan sel yang ditentukan oleh satu set syarat atau kriteria yang diberikan.",
		arguments: [
			{
				name: "criteria_range",
				description: "ialah julat sel yang anda ingin ia dinilai bagi syarat tertentu"
			},
			{
				name: "criteria",
				description: "ialah syarat dalam bentuk nombor, ungkapan, atau teks yang mentakrifkan sel mana akan dikira"
			}
		]
	},
	{
		name: "COUPDAYBS",
		description: "Mengembalikan bilangan hari sejak permulaan tempoh kupon hingga tarikh penyelesaian.",
		arguments: [
			{
				name: "settlement",
				description: "ialah tarikh penyelesaian keselamatan, disebut sebagai nombor tarikh bersiri"
			},
			{
				name: "maturity",
				description: "ialah tarikh matang keselamatan, disebut sebagai nombor tarikh bersiri"
			},
			{
				name: "frequency",
				description: "ialah bilangan pembayaran kupon setahun"
			},
			{
				name: "basis",
				description: "ialah jenis berdasarkan bilangan hari untuk digunakan"
			}
		]
	},
	{
		name: "COUPNCD",
		description: "Mengembalikan tarikh kupon berikutnya selepas tarikh penyelesaian.",
		arguments: [
			{
				name: "settlement",
				description: "ialah tarikh penyelesaian keselamatan, disebut sebagai nombor tarikh bersiri"
			},
			{
				name: "maturity",
				description: "ialah tarikh matang keselamatan, disebut sebagai nombor tarikh bersiri"
			},
			{
				name: "frequency",
				description: "ialah bilangan pembayaran kupon setahun"
			},
			{
				name: "basis",
				description: "ialah jenis berdasarkan bilangan hari untuk digunakan"
			}
		]
	},
	{
		name: "COUPNUM",
		description: "Mengembalikan bilangan kupon boleh bayar di antara tarikh penyelesaian dan tarikh matang.",
		arguments: [
			{
				name: "settlement",
				description: "ialah tarikh penyelesaian keselamatan, disebut sebagai nombor tarikh bersiri"
			},
			{
				name: "maturity",
				description: "ialah tarikh matang keselamatan, disebut sebagai nombor tarikh bersiri"
			},
			{
				name: "frequency",
				description: "ialah bilangan pembayaran kupon setahun"
			},
			{
				name: "basis",
				description: "ialah jenis berdasarkan bilangan hari untuk digunakan"
			}
		]
	},
	{
		name: "COUPPCD",
		description: "Mengembalikan tarikh kupon sebelum ini sebelum tarikh penyelesaian.",
		arguments: [
			{
				name: "settlement",
				description: "ialah tarikh penyelesaian keselamatan, disebut sebagai nombor tarikh bersiri"
			},
			{
				name: "maturity",
				description: "ialah tarikh matang keselamatan, disebut sebagai nombor tarikh bersiri"
			},
			{
				name: "frequency",
				description: "ialah bilangan pembayaran kupon setahun"
			},
			{
				name: "basis",
				description: "ialah jenis berdasarkan bilangan hari untuk digunakan"
			}
		]
	},
	{
		name: "COVAR",
		description: "Mengembalikan kovarians, purata produk sisihan bagi setiap pasangan titik data dalam dua set data.",
		arguments: [
			{
				name: "array1",
				description: "ialah julat sel pertama integer dan mestilah nombor, tatasusunan atau rujukan yang mengandungi nombor"
			},
			{
				name: "array2",
				description: "ialah julat sel kedua integer dan mestilah nombor, tatasusunan atau rujukan yang mengandungi nombor"
			}
		]
	},
	{
		name: "COVARIANCE.P",
		description: "Mengembalikan kovarians populasi, purata produk sisihan untuk setiap pasangan titik data dalam dua set data.",
		arguments: [
			{
				name: "array1",
				description: "adalah julat sel pertama bagi integer dan mesti nombor, tatasusunan atau rujukan yang mengandungi nombor"
			},
			{
				name: "array2",
				description: "adalah julat sel kedua bagi integer dan mesti nombor, tatasusunan atau rujukan yang mengandungi nombor"
			}
		]
	},
	{
		name: "COVARIANCE.S",
		description: "Mengembalikan kovarians sampel, purata produk sisihan untuk setiap pasangan titik data dalam dua set data.",
		arguments: [
			{
				name: "array1",
				description: "adalah julat sel pertam bagi integer dan mesti nombor, tatasusunan atau rujukan yang mengandungi nombor"
			},
			{
				name: "array2",
				description: "adalah julat sel kedua bagi integer dan mesti nombor, tatasusunan atau rujukan yang mengandungi nombor"
			}
		]
	},
	{
		name: "CRITBINOM",
		description: "Mengembalikan nilai terkecil apabila taburan binomial kumulatif lebih besar daripada atau sama dengan nilai kriteria.",
		arguments: [
			{
				name: "trials",
				description: "ialah bilangan percubaan Bernoulli"
			},
			{
				name: "probability_s",
				description: "ialah kebarangkalian kejayaan setiap percubaan, termasuk nombor 0 hingga 1"
			},
			{
				name: "alpha",
				description: "ialah nilai kriteria, termasuk nombor 0 hingga 1"
			}
		]
	},
	{
		name: "CSC",
		description: "Mengembalikan kosekan sudut.",
		arguments: [
			{
				name: "number",
				description: "ialah sudut dalam radian yang anda inginkan bagi kosekan"
			}
		]
	},
	{
		name: "CSCH",
		description: "Mengembalikan kosekan hiperbola sudut.",
		arguments: [
			{
				name: "number",
				description: "ialah sudut dalam radian yang anda inginkan bagi kosekan hiperbola"
			}
		]
	},
	{
		name: "CUMIPMT",
		description: "mengembalikan faedah kumulatif yang dibayar di antara dua tempoh.",
		arguments: [
			{
				name: "rate",
				description: "ia kadar faedah"
			},
			{
				name: "nper",
				description: "ialah jumlah bilangan tempoh pembayaran"
			},
			{
				name: "pv",
				description: "ialah nilai sekarang"
			},
			{
				name: "start_period",
				description: "ialah tempoh pertama dalam pengiraan"
			},
			{
				name: "end_period",
				description: "ialah tempoh akhir dalam pengiraan"
			},
			{
				name: "type",
				description: "ialah penjadualan pembayaran"
			}
		]
	},
	{
		name: "CUMPRINC",
		description: "Mengembalikan modal kumulatif dibayar melalui pinjaman di antara dua tempoh.",
		arguments: [
			{
				name: "rate",
				description: "ialah kadar faedah"
			},
			{
				name: "nper",
				description: "ialah jumlah bilangan tempoh pembayaran"
			},
			{
				name: "pv",
				description: "ialah nilai sekarang"
			},
			{
				name: "start_period",
				description: "ialah tempoh pertama dalam pengiraan"
			},
			{
				name: "end_period",
				description: "ialah tempoh akhir dalam pengiraan"
			},
			{
				name: "type",
				description: "ialah penjadualan pembayaran"
			}
		]
	},
	{
		name: "DATE",
		description: "Mengembalikan nombor yang mewakili tarikh dalam kod tarikh masa Spreadsheet.",
		arguments: [
			{
				name: "year",
				description: "ialah nombor bermula 1900 hingga 9999 dalam Spreadsheet bagi Windows atau bermula 1904 hingga 9999 dalam Spreadsheet bagi Macintosh"
			},
			{
				name: "month",
				description: "ialah nombor bermula 1 hingga 12 yang mewakili bulan dalam setahun"
			},
			{
				name: "day",
				description: "ialah nombor bermula 1 hingga 31 yang mewakili hari dalam sebulan"
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
		description: "Menukarkan tarikh dalam bentuk teks kepada nombor yang mewakili tarikh dalam kod tarikh masa Spreadsheet.",
		arguments: [
			{
				name: "date_text",
				description: "ialah teks yang mewakili tarikh dalam format tarikh Spreadsheet, antara 1/1/1900 (Windows) atau 1/1/1904 (Macintosh) dan 12/31/9999"
			}
		]
	},
	{
		name: "DAVERAGE",
		description: "Memuratakan nilai dalam lajur dalam senarai atau pangkalan data yang sepadan dengan syarat yang anda tentukan.",
		arguments: [
			{
				name: "database",
				description: "ialah julat sel yang membentuk senarai atau pangkalan data. Pangkalan data ialah senarai data yang berkaitan"
			},
			{
				name: "field",
				description: "adalah sama ada label lajur dalam tanda petikan berganda atau nombor yang mewakili posisi lajur dalam senarai"
			},
			{
				name: "criteria",
				description: "ialah nilai sel yang mengandungi syarat yang anda tentukan. Julat termasuk label lajur dan satu sel di bawah label syarat"
			}
		]
	},
	{
		name: "DAY",
		description: "Mengembalikan hari dalam bulan, nombor bermula 1 hingga 31.",
		arguments: [
			{
				name: "serial_number",
				description: "ialah nombor dalam kod tarikh masa yang digunakan oleh Spreadsheet"
			}
		]
	},
	{
		name: "DAYS",
		description: "Mengembalikan bilangan hari antara dua tarikh.",
		arguments: [
			{
				name: "end_date",
				description: "start_date dan end_date adalah dua tarikh yang anda ingin ketahui bilangan hari di antaranya"
			},
			{
				name: "start_date",
				description: "start_date dan end_date adalah dua tarikh yang anda ingin ketahui bilangan hari di antaranya"
			}
		]
	},
	{
		name: "DAYS360",
		description: "Mengembalikan bilangan hari antara dua tarikh berdasarkan 360 hari setahun (dua belas bulan yang ada 30 hari).",
		arguments: [
			{
				name: "start_date",
				description: "tarikh_mula dan tarikh_akhir adalah di antara dua tarikh yang ingin anda ketahui bilangan harinya"
			},
			{
				name: "end_date",
				description: "tarikh_mula dan tarikh_akhir adalah di antara dua tarikh yang ingin anda ketahui bilangan harinya"
			},
			{
				name: "method",
				description: "ialah nilai logik yang menentukan kaedah pengiraan: A.S. (NASD) = PALSU atau diabaikan; Eropah = BENAR."
			}
		]
	},
	{
		name: "DB",
		description: "Mengembalikan susut nilai aset bagi tempoh tertentu yang menggunakan kaedah imbangan penyusutan tetap.",
		arguments: [
			{
				name: "cost",
				description: "ialah kos permulaan aset"
			},
			{
				name: "salvage",
				description: "ialah nilai sisaan pada akhir hayat aset"
			},
			{
				name: "life",
				description: "ialah bilangan tempoh tamat apabila aset disusut nilai (kadangkala disebut sebagai usia berguna aset)"
			},
			{
				name: "period",
				description: "ialah tempoh yang ingin anda kira susut nilainya. Tempoh mestilah menggunakan unit yang sama seperti Hayat"
			},
			{
				name: "month",
				description: "ialah bilangan bulan dalam tahun pertama. Jika bulan diabaikan, ia dianggap sebagai 12"
			}
		]
	},
	{
		name: "DCOUNT",
		description: "Mengira sel yang mengandungi nombor dalam medan (lajur) rekod dalam pangkalan data yang sepadan dengan syarat yang anda tentukan.",
		arguments: [
			{
				name: "database",
				description: "ialah julat sel yang membentuk senarai atau pangkalan data. Pangkalan data ialah senarai data yang berkaitan"
			},
			{
				name: "field",
				description: "adalah sama ada label lajur dalam tanda petikan berganda atau nombor yang mewakili posisi lajur dalam senarai"
			},
			{
				name: "criteria",
				description: "ialah julat sel yang mengandungi syarat yang anda tentukan. Julat termasuk label lajur dan satu sel di bawah label syarat"
			}
		]
	},
	{
		name: "DCOUNTA",
		description: "Mengira sel bukan kosong dalam medan (lajur) rekod dalam pangkalan data yang memenuhi syarat yang anda tentukan.",
		arguments: [
			{
				name: "database",
				description: "ialah julat sel yang membentuk senarai atau pangkalan data. Pangkalan data ialah senarai data yang berkaitan"
			},
			{
				name: "field",
				description: "adalah sama ada label lajur dalam tanda petikan berganda atau nombor yang mewakili posisi lajur dalam senarai"
			},
			{
				name: "criteria",
				description: "ialah julat sel yang mengandungi syarat yang anda tentukan. Julat termasuk label lajur dan satu sel di bawah label untuk syarat"
			}
		]
	},
	{
		name: "DDB",
		description: "Mengembalikan susut nilai aset untuk tempoh yang ditentukan menggunakan kaedah imbangan penyusutan berganda atau apa-apa kaedah lain yang anda tentukan.",
		arguments: [
			{
				name: "cost",
				description: "ialah kos permulaan aset"
			},
			{
				name: "salvage",
				description: "ialah nilai sisaan di akhir hayat aset"
			},
			{
				name: "life",
				description: "ialah bilangan tempoh tamat apabila aset disusutnilaikan (kadangkala disebut sebagai usia berguna aset)"
			},
			{
				name: "period",
				description: "ialah tempoh yang ingin anda kira susut nilainya. Tempoh mestilah menggunakan unit yang sama dengan Hayat"
			},
			{
				name: "factor",
				description: "ialah kadar apabila baki menyusut. Jika Faktor diabaikan, ia dianggap sebagai 2 (kaedah imbangan penyusutan berganda)"
			}
		]
	},
	{
		name: "DEC2BIN",
		description: "Menukar nombor perpuluhan kepada perduaan.",
		arguments: [
			{
				name: "number",
				description: "ialah integer perpuluhan yang ingin anda tukar"
			},
			{
				name: "places",
				description: "ialah bilangan aksara untuk digunakan"
			}
		]
	},
	{
		name: "DEC2HEX",
		description: "Menukar nombor perpuluhan kepada perenambelasan.",
		arguments: [
			{
				name: "number",
				description: "ialah integer perpuluhan yang ingin anda tukar"
			},
			{
				name: "places",
				description: "ialah bilangan aksara untuk digunakan"
			}
		]
	},
	{
		name: "DEC2OCT",
		description: "Menukar nombor perpuluhan kepada perlapanan.",
		arguments: [
			{
				name: "number",
				description: "ialah integer perpuluhan yang ingin anda tukar"
			},
			{
				name: "places",
				description: "ialah bilangan aksara untuk digunakan"
			}
		]
	},
	{
		name: "DECIMAL",
		description: "Menukar gambaran teks bagi nombor dalam asas tertentu kepada nombor perpuluhan.",
		arguments: [
			{
				name: "number",
				description: "ialah nombor yang anda ingin tukarkan"
			},
			{
				name: "radix",
				description: "ialah Radiks asas bagi nombor yang anda tukarkan"
			}
		]
	},
	{
		name: "DEGREES",
		description: "Menukarkan radian kepada darjah.",
		arguments: [
			{
				name: "angle",
				description: "ialah sudut dalam radian yang ingin anda tukar"
			}
		]
	},
	{
		name: "DELTA",
		description: "Uji sama ada dua nombor adalah sama.",
		arguments: [
			{
				name: "number1",
				description: "ialah nombor pertama"
			},
			{
				name: "number2",
				description: "ialah nombor kedua"
			}
		]
	},
	{
		name: "DEVSQ",
		description: "Mengembalikan hasil tambah kuasa dua sisihan titik data daripada min sampelnya.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "ialah 1 hingga 255 argumen, atau tatasusunan atau rujukan tatasusunan, yang mana anda inginkan DEVSQ kira"
			},
			{
				name: "number2",
				description: "ialah 1 hingga 255 argumen, atau tatasusunan atau rujukan tatasusunan, yang mana anda inginkan DEVSQ kira"
			}
		]
	},
	{
		name: "DGET",
		description: "Mengekstrak daripada pangkalan data rekod tunggal yang memenuhi syarat yang anda tentukan.",
		arguments: [
			{
				name: "database",
				description: "ialah julat sel yang membentuk senarai atau pangkalan data. Pangkalan data ialah senarai data yang berkaitan"
			},
			{
				name: "field",
				description: "adalah sama ada label lajur dalam tanda petikan berganda atau nombor yang mewakili posisi lajur dalam senarai"
			},
			{
				name: "criteria",
				description: "ialah julat sel yang mengandungi syarat yang anda tentukan. Julat termasuk label lajur dan satu sel di bawah label untuk syarat"
			}
		]
	},
	{
		name: "DISC",
		description: "Mengembalikan kadar diskaun keselamatan.",
		arguments: [
			{
				name: "settlement",
				description: "ialah tarikh penyelesaian keselamatan, disebut sebagai nombor tarikh bersiri"
			},
			{
				name: "maturity",
				description: "ialah tarikh kematangan keselamatan, disebut sebagai nombor tarikh bersiri"
			},
			{
				name: "pr",
				description: "ialah harga keselamatan per $100 nilai muka"
			},
			{
				name: "redemption",
				description: "ialah nilai penebusan keselamatan per $100 nilai muka"
			},
			{
				name: "basis",
				description: "ialah jenis berdasarkan bilangan hari untuk digunakan"
			}
		]
	},
	{
		name: "DMAX",
		description: "Mengembalikan nombor terbesar dalam medan (lajur) rekod dalam pangkalan data yang sepadan dengan syarat yang anda tentukan.",
		arguments: [
			{
				name: "database",
				description: "ialah julat sel yang membentuk senarai atau pangkalan data. Pangkalan data ialah senarai data yang berkaitan"
			},
			{
				name: "field",
				description: "adalah sama ada label lajur dalam tanda petikan berganda atau nombor yang mewakili posisi lajur dalam senarai"
			},
			{
				name: "criteria",
				description: "ialah julat sel yang mengandungi syarat yang anda tentukan. Julat termasuk label lajur dan satu sel di bawah label syarat"
			}
		]
	},
	{
		name: "DMIN",
		description: "Mengembalikan nombor terkecil dalam medan (lajur) rekod dalam pangkalan data yang sepadan dengan syarat yang anda tentukan.",
		arguments: [
			{
				name: "database",
				description: "ialah julat sel yang membentuk senarai atau pangkalan data. Pangkalan data ialah senarai data yang berkaitan"
			},
			{
				name: "field",
				description: "adalah sama ada label lajur dalam tanda petikan berganda atau nombor yang mewakili posisi lajur dalam senarai"
			},
			{
				name: "criteria",
				description: "julat sel yang mengandungi syarat yang anda tentukan. Julat termasuk label lajur dan satu sel di bawah label syarat"
			}
		]
	},
	{
		name: "DOLLAR",
		description: "Menukarkan nombor kepada teks, menggunakan format mata wang.",
		arguments: [
			{
				name: "number",
				description: "ialah nombor, rujukan kepada sel yang mengandungi nombor, atau formula yang menilai kepada nombor"
			},
			{
				name: "decimals",
				description: "ialah bilangan digit ke sebelah kanan titik perpuluhan. Nombor dibundarkan jika perlu; jika diabaikan, Perpuluhan = 2"
			}
		]
	},
	{
		name: "DOLLARDE",
		description: "Menukar harga dollar, disebut sebagai pecahan, ke dalam harga dollar, disebut sebagai nombor perpuluhan.",
		arguments: [
			{
				name: "fractional_dollar",
				description: "ialah nombor disebut sebagai pecahan"
			},
			{
				name: "fraction",
				description: "ialah integer untuk digunakan dalam penyebut pecahan"
			}
		]
	},
	{
		name: "DOLLARFR",
		description: "Menukar harga dollar, disebut sebagai nombor perpuluhan, ke dalam harga dollar, disebut sebagai pecahan.",
		arguments: [
			{
				name: "decimal_dollar",
				description: "ialah nombor perpuluhan"
			},
			{
				name: "fraction",
				description: "ialah integer untuk digunakan dalam penyebut pecahan"
			}
		]
	},
	{
		name: "DPRODUCT",
		description: "Menggandakan nilai dalam medan (lajur) rekod dalam pangkalan data yang memenuhi syarat yang anda tentukan.",
		arguments: [
			{
				name: "database",
				description: "ialah julat sel yang membentuk senarai atau pangkalan data. Pangkalan data ialah senarai data yang berkaitan"
			},
			{
				name: "field",
				description: "adalah sama ada label lajur dalam tanda petikan berganda atau nombor yang mewakili posisi lajur dalam senarai"
			},
			{
				name: "criteria",
				description: "ialah julat sel yang mengandungi syarat yang anda tentukan. Julat merangkumi label lajur dan satu sel di bawah label untuk syarat"
			}
		]
	},
	{
		name: "DSTDEV",
		description: "Menganggarkan sisihan piawai berdasarkan sampel daripada entri pangkalan data yang terpilih.",
		arguments: [
			{
				name: "database",
				description: "ialah julat sel yang membentuk senarai atau pangkalan data. Pangkalan data ialah senarai data yang berkaitan"
			},
			{
				name: "field",
				description: "adalah sama ada label lajur dalam tanda petikan berganda atau nombor yang mewakili posisi lajur dalam senarai"
			},
			{
				name: "criteria",
				description: "ialah julat sel yang mengandungi syarat yang anda tentukan. Julat termasuk label lajur dan satu sel di bawah label syarat"
			}
		]
	},
	{
		name: "DSTDEVP",
		description: "Mengira sisihan piawai berdasarkan keseluruhan populasi bagi entri pangkalan data terpilih.",
		arguments: [
			{
				name: "database",
				description: "ialah julat sel yang membentuk senarai atau pangkalan data. Pangkalan data ialah senarai bagi data yang berkaitan"
			},
			{
				name: "field",
				description: "adalah sama ada label lajur dalam tanda petikan berganda atau nombor yang mewakili posisi lajur dalam senarai"
			},
			{
				name: "criteria",
				description: "ialah julat sel yang mengandungi syarat yang anda tentukan. Julat merangkumi label lajur dan satu sel di bawah label untuk syarat"
			}
		]
	},
	{
		name: "DSUM",
		description: "Menambahkan bilangan medan (lajur) rekod dalam pangkalan data yang sepadan dengan syarat yang anda tentukan.",
		arguments: [
			{
				name: "database",
				description: "ialah julat sel yang membentuk senarai atau pangkalan data. Pangkalan data ialah senarai data yang berkaitan"
			},
			{
				name: "field",
				description: "adalah sama ada label lajur dalam tanda petikan berganda atau nombor yang mewakili posisi lajur dalam senarai"
			},
			{
				name: "criteria",
				description: "ialah julat sel yang mengandungi syarat yang anda tentukan. Julat termasuk label lajur dan satu sel di bawah label syarat"
			}
		]
	},
	{
		name: "DVAR",
		description: "Menganggarkan varians berdasarkan sampel daripada entri pangkalan data yang terpilih.",
		arguments: [
			{
				name: "database",
				description: "ialah julat sel yang membentuk senarai atau pangkalan data. Pangkalan data ialah senarai data yang berkaitan"
			},
			{
				name: "field",
				description: "adalah sama ada label lajur dalam tanda petikan berganda atau nombor yang mewakili posisi lajur dalam senarai"
			},
			{
				name: "criteria",
				description: "ialah julat sel yang mengandungi syarat yang anda tentukan. Julat termasuk label lajur dan satu sel di bawah label syarat"
			}
		]
	},
	{
		name: "DVARP",
		description: "Mengira varians berdasarkan keseluruhan populasi bagi entri pangkalan data terpilih.",
		arguments: [
			{
				name: "database",
				description: "ialah julat sel yang membentuk senarai atau pangkalan data. Pangkalan data ialah senarai bagi data yang berkaitan"
			},
			{
				name: "field",
				description: "adalah sama ada label lajur dalam tanda petikan berganda atau nombor yang mewakili posisi lajur dalam senarai"
			},
			{
				name: "criteria",
				description: "ialah julat sel yang mengandungi syarat yang anda tentukan. Julat merangkumi label lajur dan satu sel di bawah label untuk syarat"
			}
		]
	},
	{
		name: "EDATE",
		description: "Mengembalikan nombor siri tarikh yang menunjukkan bilangan bulan sebelum atau selepas tarikh mula.",
		arguments: [
			{
				name: "start_date",
				description: "ialah nombor tarikh bersiri yang mewakili tarikh mula"
			},
			{
				name: "months",
				description: "ialah bilangan bulan sebelum atau selepas tarikh_mula"
			}
		]
	},
	{
		name: "EFFECT",
		description: "Mengembalikan kadar faedah tahunan efektif.",
		arguments: [
			{
				name: "nominal_rate",
				description: "ialah kadar faedah nominal"
			},
			{
				name: "npery",
				description: "ialah bilangan tempoh pengkompaunan setahun"
			}
		]
	},
	{
		name: "ENCODEURL",
		description: "Mengembalikan rentetan dienkodkan URL .",
		arguments: [
			{
				name: "text",
				description: "adalah rentetan untuk dienkodkan URL"
			}
		]
	},
	{
		name: "EOMONTH",
		description: "Mengembalikan nombor siri hari terakhir bulan sebelum atau selepas bilangan bulan yang ditentukan.",
		arguments: [
			{
				name: "start_date",
				description: "ialah nombor tarikh bersiri yang mewakili tarikh mula"
			},
			{
				name: "months",
				description: "ialah bilangan bulan sebelum atau selepas tarikh_mula"
			}
		]
	},
	{
		name: "ERF",
		description: "Mengembalikan fungsi ralat.",
		arguments: [
			{
				name: "lower_limit",
				description: "ialah sempadan bawah untuk pengamir ERF"
			},
			{
				name: "upper_limit",
				description: "ialah sempadan atas bagi pengamir ERF"
			}
		]
	},
	{
		name: "ERF.PRECISE",
		description: "Mengembalikan fungsi ralat.",
		arguments: [
			{
				name: "X",
				description: "ialah batasan bawah untuk mengamirkan ERF.PRECISE"
			}
		]
	},
	{
		name: "ERFC",
		description: "Mengembalikan fungsi ralat pelengkap.",
		arguments: [
			{
				name: "x",
				description: "ialah sempadan bawah bagi pengamir ERF"
			}
		]
	},
	{
		name: "ERFC.PRECISE",
		description: "Mengembalikan fungsi ralat pelengkap.",
		arguments: [
			{
				name: "X",
				description: "ialah sempadan bawah bagi pengamir ERFC.PRECISE"
			}
		]
	},
	{
		name: "ERROR.TYPE",
		description: "Mengembalikan nombor yang sepadan dengan nilai ralat.",
		arguments: [
			{
				name: "error_val",
				description: "ialah nilai ralat yang anda ingin kenal pasti nombornya, dan mungkin nilai ralat sebenar atau rujukan kepada sel yang mengandungi nilai ralat"
			}
		]
	},
	{
		name: "EVEN",
		description: "Membundarkan nombor positif ke atas dan nombor negatif ke bawah kepada integer genap yang terdekat.",
		arguments: [
			{
				name: "number",
				description: "ialah nilai untuk dibundarkan"
			}
		]
	},
	{
		name: "EXACT",
		description: "Menyemak sama ada dua rentetan teks adalah benar-benar sama, dan mengembalikan BENAR atau PALSU. TEPAT adalah sensitif huruf.",
		arguments: [
			{
				name: "text1",
				description: "ialah rentetan teks pertama"
			},
			{
				name: "text2",
				description: "ialah rentetan teks kedua"
			}
		]
	},
	{
		name: "EXP",
		description: "Mengembalikan e dinaikkan kepada kuasa nombor yang diberikan.",
		arguments: [
			{
				name: "number",
				description: "ialah eksponen yang digunakan kepada asas e. Pemalar e bersamaan dengan 2.71828182845904, asas logaritma asli"
			}
		]
	},
	{
		name: "EXPON.DIST",
		description: "Mengembalikan taburan eksponen.",
		arguments: [
			{
				name: "x",
				description: "ialah nilai fungsi, nombor bukan negatif"
			},
			{
				name: "lambda",
				description: "ialah nilai parameter, nombor positif"
			},
			{
				name: "cumulative",
				description: "ialah nilai logik bagi fungsi untuk dikembalikan: fungsi taburan kumulatif = BENAR; fungsi kepadatan kebarangkalian = PALSU"
			}
		]
	},
	{
		name: "EXPONDIST",
		description: "Mengembalikan taburan eksponen.",
		arguments: [
			{
				name: "x",
				description: "ialah nilai fungsi, nombor bukan negatif"
			},
			{
				name: "lambda",
				description: "ialah nilai parameter, nombor positif"
			},
			{
				name: "cumulative",
				description: "ialah nilai logik bagi fungsi untuk dikembalikan: fungsi taburan kumulatif = TRUE; fungsi kepadatan kebarangkalian = FALSE"
			}
		]
	},
	{
		name: "F.DIST",
		description: "Mengembalikan (berekor kiri) taburan kebarangkalian F (darjah kepelbagaian) untuk dua set data.",
		arguments: [
			{
				name: "x",
				description: "ialah nilai untuk menilai fungsi, nombor bukan negatif"
			},
			{
				name: "deg_freedom1",
				description: "ialah pembilang darjah kebebasan, nombor antara 1 dan 10^10, tak termasuk 10^10"
			},
			{
				name: "deg_freedom2",
				description: "ialah darjah kebebasan penyebut, nombor antara 1 dan 10^10, tak termasuk 10^10"
			},
			{
				name: "cumulative",
				description: "ialah nilai logik bagi fungsi untuk dikembalikan: fungsi taburan kumulatif = BENAR; fungsi kepadatan kebarangkalian = PALSU"
			}
		]
	},
	{
		name: "F.DIST.RT",
		description: "Mengembalikan (berekor kanan) taburan kebarangkalian F (darjah kepelbagaian) untuk dua set data.",
		arguments: [
			{
				name: "x",
				description: "ialah nilai untuk menilai fungsi, nombor bukan negatif"
			},
			{
				name: "deg_freedom1",
				description: "ialah pembilang darjah kebebasan, nombor antara 1 dan 10^10, tak termasuk 10^10"
			},
			{
				name: "deg_freedom2",
				description: "ialah darjah kebebasan penyebut, nombor antara 1 dan 10^10, tak termasuk 10^10"
			}
		]
	},
	{
		name: "F.INV",
		description: "Mengembalikan songsangan (berekor kiri) taburan kebarangkalian F: jika p = F.DIST(x,...), maka F.INV(p,...) = x.",
		arguments: [
			{
				name: "probability",
				description: "ialah kebarangkalian yang dikaitkan dengan taburan kumulatif F, termasuk nombor 0 hingga 1"
			},
			{
				name: "deg_freedom1",
				description: "ialah darjah kebebasan pembilang, nombor antara 1 dan 10^10, tak termasuk 10^10"
			},
			{
				name: "deg_freedom2",
				description: "ialah darjah kebebasan penyebut, nombor antara 1 dan 10^10, tak termasuk 10^10"
			}
		]
	},
	{
		name: "F.INV.RT",
		description: "Mengembalikan songsangan (berekor kanan) taburan kebarangkalian F: jika p = F.DIST.RT(x,...), maka F.INV.RT(p,...) = x.",
		arguments: [
			{
				name: "probability",
				description: "ialah kebarangkalian yang dikaitkan dengan taburan kumulatif F, termasuk nombor 0 hingga 1"
			},
			{
				name: "deg_freedom1",
				description: "ialah darjah kebebasan pembilang, nombor antara 1 dan 10^10, tak termasuk 10^10"
			},
			{
				name: "deg_freedom2",
				description: "ialah darjah kebebasan penyebut, nombor antara 1 dan 10^10, tak termasuk 10^10"
			}
		]
	},
	{
		name: "F.TEST",
		description: "Mengembalikan hasil ujian F, kebarangkalian dua ekor bahawa varians dalam Tatasusunan1 dan Tatasusunan2 tidak berbeza secara nyata.",
		arguments: [
			{
				name: "array1",
				description: "ialah tatasusunan atau julat data pertama dan mungkin nombor atau nama, tatasusunan atau rujukan yang mengandungi nombor (ruang kosong diabaikan)"
			},
			{
				name: "array2",
				description: "ialah tatasusunan atau julat data kedua dan mungkin nombor atau nama, tatasusunan atau rujukan yang mengandungi nombor (ruang kosong diabaikan)"
			}
		]
	},
	{
		name: "FACT",
		description: "Mengembalikan faktorial bagi nombor, sama dengan 1*2*3*...* Nombor.",
		arguments: [
			{
				name: "number",
				description: "ialah nombor bukan negatif yang anda inginkan faktorialnya"
			}
		]
	},
	{
		name: "FACTDOUBLE",
		description: "Mengembalikan faktorial berganda bagi nombor.",
		arguments: [
			{
				name: "number",
				description: "ialah nilai pada mana untuk mengembalikan faktorial berganda"
			}
		]
	},
	{
		name: "FALSE",
		description: "Mengembalikan nilai logik PALSU.",
		arguments: [
		]
	},
	{
		name: "FDIST",
		description: "Mengembalikan taburan kebarangkalian F (darjah kepelbagaian) untuk dua set data.",
		arguments: [
			{
				name: "x",
				description: "ialah nilai untuk menilai fungsi, nombor bukan negatif"
			},
			{
				name: "deg_freedom1",
				description: "ialah darjah kebebasan pembilang, nombor antara 1 dan 10^10, tak termasuk 10^10"
			},
			{
				name: "deg_freedom2",
				description: "ialah darjah kebebasan penyebut, nombor antara 1 dan 10^10, tak termasuk 10^10"
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
		description: "Mengembalikan posisi permulaan satu rentetan teks di dalam rentetan teks yang lain. CARI ialah sensitif huruf.",
		arguments: [
			{
				name: "find_text",
				description: "ialah teks yang ingin anda cari. Gunakan tanda petik berganda (teks kosong) untuk memadankan dengan aksara pertama dalam Antara_teks; aksara bebas tidak diizinkan"
			},
			{
				name: "within_text",
				description: "ialah teks yang mengandungi teks yang ingin anda cari"
			},
			{
				name: "start_num",
				description: "menentukan aksara untuk memulakan carian. Aksara pertama dalam Antara_teks ialah aksara nombor 1. Jika diabaikan, nombor_Mula = 1"
			}
		]
	},
	{
		name: "FINV",
		description: "Mengembalikan songsangan taburan kebarangkalian F: jika p = FDIST(x,...), maka FINV(p,...) = x.",
		arguments: [
			{
				name: "probability",
				description: "ialah kebarangkalian yang dikaitkan dengan taburan kumulatif F, termasuk nombor 0 hingga 1"
			},
			{
				name: "deg_freedom1",
				description: "ialah darjah kebebasan pembilang, nombor antara 1 dan 10^10, tak termasuk 10^10"
			},
			{
				name: "deg_freedom2",
				description: "ialah darjah kebebasan penyebut, nombor antara 1 dan 10^10, tak termasuk 10^10"
			}
		]
	},
	{
		name: "FISHER",
		description: "Mengembalikan transformasi Fisher.",
		arguments: [
			{
				name: "x",
				description: "ialah nilai yang anda ingin laksanakan transformasi, nombor antara -1 dan 1, tidak termasuk -1 dan 1"
			}
		]
	},
	{
		name: "FISHERINV",
		description: "Mengembalikan songsangan transformasi Fisher: jika y = FISHER(x), maka FISHERINV(y) = x.",
		arguments: [
			{
				name: "y",
				description: "ialah nilai yang ingin anda laksanakan songsangan transformasi"
			}
		]
	},
	{
		name: "FIXED",
		description: "Membundarkan nombor kepada nombor perpuluhan yang ditentukan dan mengembalikan hasil sebagai teks dengan atau tanpa koma.",
		arguments: [
			{
				name: "number",
				description: "ialah nombor yang ingin anda bundarkan dan tukarkan kepada teks"
			},
			{
				name: "decimals",
				description: "ialah bilangan digit di sebelah kanan titik perpuluhan. Jika diabaikan, Perpuluhan = 2"
			},
			{
				name: "no_commas",
				description: "ialah nilai logik: jangan paparkan koma dalam teks yang dikembalikan = BENAR; paparkan koma pada teks yang dikembalikan = PALSU atau diabaikan"
			}
		]
	},
	{
		name: "FLOOR",
		description: "Membundarkan nombor ke bawah, kepada gandaan bererti yang terdekat.",
		arguments: [
			{
				name: "number",
				description: "ialah nilai angka yang ingin anda bundarkan"
			},
			{
				name: "significance",
				description: "ialah gandaan yang ingin anda bundarkan. Nombor dan Erti mestilah sama ada kedua-duanya positif atau kedua-duanya negatif"
			}
		]
	},
	{
		name: "FLOOR.MATH",
		description: "Membundar turun nombor kepada integer terdekat atau kepada gandaan bererti yang terdekat.",
		arguments: [
			{
				name: "number",
				description: "ialah nilai yang anda ingin bundarkan"
			},
			{
				name: "significance",
				description: "ialah gandaan yang ingin anda bundarkan"
			},
			{
				name: "mode",
				description: "apabila diberi dan bukan sifar fungsi ini akan membundar mendekati sifar"
			}
		]
	},
	{
		name: "FLOOR.PRECISE",
		description: "Membundarkan nombor ke bawah kepada integer terdekat atau gandaan bererti yang terdekat.",
		arguments: [
			{
				name: "number",
				description: "ialah nilai angka yang ingin anda bundarkan"
			},
			{
				name: "significance",
				description: "ialah gandaan yang ingin anda bundarkan. "
			}
		]
	},
	{
		name: "FORECAST",
		description: "Mengira, atau meramalkan, nilai masa depan di sepanjang arah aliran linear menggunakan nilai yang telah wujud.",
		arguments: [
			{
				name: "x",
				description: "ialah titik data yang ingin anda ramalkan satu nilai dan mestilah nilai angka"
			},
			{
				name: "known_y's",
				description: "ialah tatasusunan bersandar atau julat data angka"
			},
			{
				name: "known_x's",
				description: "ialah tatasusunan bebas atau julat data angka.  x_Diketahui mestilah bukan sifar"
			}
		]
	},
	{
		name: "FORMULATEXT",
		description: "Mengembalikan formula sebagai rentetan.",
		arguments: [
			{
				name: "reference",
				description: "ialah rujukan kepada formula"
			}
		]
	},
	{
		name: "FREQUENCY",
		description: "Mengira sekerap mana nilai muncul dalam julat nilai dan kemudian mengembalikan tatasusunan menegak nombor yang mempunyai lebih satu unsur daripada tatasusunan_Bins.",
		arguments: [
			{
				name: "data_array",
				description: "ialah tatasusunan atau rujukan kepada set nilai yang ingin anda bilang kekerapannya (kosong dan teks diabaikan)"
			},
			{
				name: "bins_array",
				description: "ialah tatasusunan atau rujukan kepada selang yang anda ingin kumpulkan nilainya dalam tatasusunan_data"
			}
		]
	},
	{
		name: "FTEST",
		description: "Mengembalikan hasil ujian F, kebarangkalian dua ekor bahawa varians dalam Tatasusunan1 dan Tatasusunan2 tidak berbeza secara nyata.",
		arguments: [
			{
				name: "array1",
				description: "ialah tatasusunan atau julat data pertama dan mungkin nombor atau nama, tatasusunan atau rujukan yang mengandungi nombor (ruang kosong diabaikan)"
			},
			{
				name: "array2",
				description: "ialah tatasusunan atau julat data kedua dan mungkin nombor atau nama, tatasusunan atau rujukan yang mengandungi nombor (ruang kosong diabaikan)"
			}
		]
	},
	{
		name: "FV",
		description: "Mengembalikan nilai masa depan pelaburan berdasarkan tempoh berkala, bayaran malar dan kadar faedah malar.",
		arguments: [
			{
				name: "rate",
				description: "ialah kadar faedah bagi setiap tempoh. Contohnya, gunakan 6%/4 bagi bayaran setiap suku tahun pada 6% APR"
			},
			{
				name: "nper",
				description: "ialah jumlah bilangan tempoh bayaran dalam pelaburan"
			},
			{
				name: "pmt",
				description: "ialah bayaran yang dibuat bagi setiap tempoh; ia tidak boleh diubah sepanjang tempoh pelaburan"
			},
			{
				name: "pv",
				description: "ialah nilai semasa, atau jumlah sekali gus bagi nilai siri bayaran masa depan sekarang. Jika diabaikan, Pv = 0"
			},
			{
				name: "type",
				description: "ialah nilai yang mewakili tempoh bayaran: bayaran pada permulaan tempoh = 1; bayaran pada akhir tempoh = 0 atau diabaikan"
			}
		]
	},
	{
		name: "FVSCHEDULE",
		description: "Mengembalikan nilai masa depan bagi modal permulaan selepas menggunakan satu siri kadar faedah kompaun.",
		arguments: [
			{
				name: "principal",
				description: "ialah nilai sekarang"
			},
			{
				name: "schedule",
				description: "ialah tatasusunan bagi kadar faedah untuk digunakan"
			}
		]
	},
	{
		name: "GAMMA",
		description: "Mengembalikan nilai fungsi Gamma.",
		arguments: [
			{
				name: "x",
				description: "ialah nilai yang anda inginkan untuk mengira Gamma"
			}
		]
	},
	{
		name: "GAMMA.DIST",
		description: "Mengembalikan taburan gamma.",
		arguments: [
			{
				name: "x",
				description: "adalah nilai yang anda ingin menilai taburan, nombor tak negatif"
			},
			{
				name: "alpha",
				description: "adalah parameter kepada taburan, nombor positif"
			},
			{
				name: "beta",
				description: "adalah parameter kepada taburan, nombor positif. Jika beta = 1, GAMMA.DIST mengembalikan taburan gamma standard"
			},
			{
				name: "cumulative",
				description: "adalah nilai logikal: mengembalikan fungsi taburan kumulatif = TRUE; mengembalikan fungsi massa kebarangkalian = FALSE atau dikecualikan"
			}
		]
	},
	{
		name: "GAMMA.INV",
		description: "Mengembalikan taburan kumulatif gamma songsang: jika p = GAMMA.DIST(x,...), kemudian GAMMA.INV(p,...) = x.",
		arguments: [
			{
				name: "probability",
				description: "adalah kebarangkalian yang dikaitkan dengan taburan gamma, nombor antara 0 dan 1, inklusif"
			},
			{
				name: "alpha",
				description: "adalah parameter kepada taburan, nombor positif"
			},
			{
				name: "beta",
				description: "adalah parameter kepada taburan, nombor positif. Jika beta = 1, GAMMA.INV mengembalikan taburan gamma standard songsang"
			}
		]
	},
	{
		name: "GAMMADIST",
		description: "Mengembalikan taburan gamma.",
		arguments: [
			{
				name: "x",
				description: "adalah nilai untuk anda menilai taburan, nombor bukan negatif"
			},
			{
				name: "alpha",
				description: "adalah parameter kepada taburan, nombor positif"
			},
			{
				name: "beta",
				description: "adalah parameter kepada taburan, nombor positif. Jika beta = 1, GAMMADIST mengembalikan taburan gamma standard"
			},
			{
				name: "cumulative",
				description: "adalah nilai logik: mengembalikan fungsi taburan kumulatif = TRUE; mengembalikan fungsi jisim kebarangkalian = FALSE atau diabaikan"
			}
		]
	},
	{
		name: "GAMMAINV",
		description: "Mengembalikan songsangan taburan gama kumulatif: jika p = GAMMADIST(x,...), maka GAMMAINV(p,...) = x.",
		arguments: [
			{
				name: "probability",
				description: "ialah kebarangkalian yang dikaitkan dengan taburan gama, termasuk nombor 0 hingga 1"
			},
			{
				name: "alpha",
				description: "ialah parameter taburan, nombor positif"
			},
			{
				name: "beta",
				description: "ialah parameter taburan, nombor positif. Jika beta=1, GAMMAINV mengembalikan songsangan taburan gama standard"
			}
		]
	},
	{
		name: "GAMMALN",
		description: "Mengembalikan logaritma asli fungsi gama.",
		arguments: [
			{
				name: "x",
				description: "ialah nilai yang anda ingin mengira GAMMALN, nombor positif"
			}
		]
	},
	{
		name: "GAMMALN.PRECISE",
		description: "Mengembalikan logaritma asli fungsi gama.",
		arguments: [
			{
				name: "x",
				description: "ialah nilai yang anda ingin mengira GAMMALN.PRECISE, nombor positif"
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
		description: "Mengembalikan pembahagi biasa terbesar.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "adalah 1 hingga 255 nilai"
			},
			{
				name: "number2",
				description: "adalah 1 hingga 255 nilai"
			}
		]
	},
	{
		name: "GEOMEAN",
		description: "Mengembalikan min geometri tatasusunan atau julat data angka positif.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "ialah 1 hingga 255 nombor atau nama, tatasusunan, atau rujukan yang mengandungi nombor yang anda inginkan minnya"
			},
			{
				name: "number2",
				description: "ialah 1 hingga 255 nombor atau nama, tatasusunan, atau rujukan yang mengandungi nombor yang anda inginkan minnya"
			}
		]
	},
	{
		name: "GESTEP",
		description: "Uji sama ada nombor lebih besar daripada nilai ambang.",
		arguments: [
			{
				name: "number",
				description: "ialah nilai untuk menguji terhadap langkah"
			},
			{
				name: "step",
				description: "ialah nilai ambang"
			}
		]
	},
	{
		name: "GETPIVOTDATA",
		description: "Mengekstrak data yang disimpan dalam Jadual Pangsi.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "data_field",
				description: "ialah nama medan data untuk mengekstrak data"
			},
			{
				name: "pivot_table",
				description: "ialah rujukan kepada sel atau julat sel dalam Jadual Pangsi yang mengandungi data yang ingin anda dapatkan semula"
			},
			{
				name: "field",
				description: "medan untuk dirujuk"
			},
			{
				name: "item",
				description: "item medan untuk dirujuk"
			}
		]
	},
	{
		name: "GROWTH",
		description: "Mengembalikan nombor dalam titik data yang diketahui padanan arah aliran pertumbuhan eksponen.",
		arguments: [
			{
				name: "known_y's",
				description: "ialah set nilai y yang anda telah tahu dalam hubungan y = b*m^x, tatasusunan atau julat nombor positif"
			},
			{
				name: "known_x's",
				description: "ialah set pilihan nilai x yang mungkin anda telah tahu dalam hubungan y = b*m^x, tatasusunan atau julat dengan saiz yang sama seperti y yang diketahui"
			},
			{
				name: "new_x's",
				description: "ialah nilai x baru yang anda ingin PERTUMBUHAN mengembalikan nilai y yang sepadan"
			},
			{
				name: "const",
				description: "ialah nilai logik: pemalar b dikira biasanya jika Const = BENAR; b diset bersamaan dengan 1 jika Const = PALSU atau diabaikan"
			}
		]
	},
	{
		name: "HARMEAN",
		description: "Mengembalikan min harmonik bagi set data nombor positif: salingan bagi min aritmetik salingan.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "ialah 1 hingga 255 nombor atau nama, tatasusunan, atau rujukan yang mengandungi nombor yang anda inginkan min harmoniknya"
			},
			{
				name: "number2",
				description: "ialah 1 hingga 255 nombor atau nama, tatasusunan, atau rujukan yang mengandungi nombor yang anda inginkan min harmoniknya"
			}
		]
	},
	{
		name: "HEX2BIN",
		description: "Menukar nombor Perenambelasan kepada perduaan.",
		arguments: [
			{
				name: "number",
				description: "ialah nombor perenambelasan yang ingin anda tukar"
			},
			{
				name: "places",
				description: "ialah bilangan aksara untuk digunakan"
			}
		]
	},
	{
		name: "HEX2DEC",
		description: "Menukar nombor perenambelasan kepada perpuluhan.",
		arguments: [
			{
				name: "number",
				description: "alah nombor perenambelasan yang ingin anda tukar"
			}
		]
	},
	{
		name: "HEX2OCT",
		description: "Menukar nombor perenambelasan kepada perlapanan.",
		arguments: [
			{
				name: "number",
				description: "ialah nombor perenambelasan yang ingin anda tukar"
			},
			{
				name: "places",
				description: "ialah bilangan aksara untuk digunakan"
			}
		]
	},
	{
		name: "HLOOKUP",
		description: "Mencari nilai dalam baris atas jadual atau tatasusunan nilai dan mengembalikan nilai dalam lajur yang sama daripada baris yang anda tentukan.",
		arguments: [
			{
				name: "lookup_value",
				description: "ialah nilai yang dijumpai dalam baris pertama jadual dan mungkin nilai, rujukan, atau rentetan teks"
			},
			{
				name: "table_array",
				description: "ialah jadual teks, nombor, atau nilai logik di mana data dicari. Tatasusunan_jadual mungkin rujukan kepada julat atau nama julat"
			},
			{
				name: "row_index_num",
				description: "ialah nombor baris dalam tatasusunan_jadual yang mana nilai sepadan hendaklah dikembalikan. Baris pertama nilai dalam jadual ialah baris 1"
			},
			{
				name: "range_lookup",
				description: "ialah nilai logik: untuk mencari padanan terhampir dalam baris teratas (diisih secara menaik) = BENAR atau diabaikan; cari padanan tepat = PALSU"
			}
		]
	},
	{
		name: "HOUR",
		description: "Mengembalikan jam sebagai nombor bermula 0 (12:00 A.M.) hingga 23 (11:00 P.M.).",
		arguments: [
			{
				name: "serial_number",
				description: "ialah nombor dalam kod tarikh masa yang digunakan oleh Spreadsheet, atau teks dalam format masa, seperti 16:48:00 atau 4:48:00 PM"
			}
		]
	},
	{
		name: "HYPERLINK",
		description: "Mencipta pintasan atau lompatan yang membuka dokumen yang tersimpan dalam pemacu keras anda, pelayan rangkaian, atau Internet.",
		arguments: [
			{
				name: "link_location",
				description: "ialah teks yang memberikan laluan dan nama fail kepada dokumen untuk dibuka, lokasi pemacu keras, alamat UNC atau laluan URL"
			},
			{
				name: "friendly_name",
				description: "ialah teks atau nombor yang dipaparkan dalam sel. Jika diabaikan, sel memaparkan teks lokasi_Pautan"
			}
		]
	},
	{
		name: "HYPGEOM.DIST",
		description: "Mengembalikan taburan hipergeometri.",
		arguments: [
			{
				name: "sample_s",
				description: "ialah bilangan kejayaan dalam sampel"
			},
			{
				name: "number_sample",
				description: "ialah saiz sampel"
			},
			{
				name: "population_s",
				description: "ialah bilangan kejayaan dalam populasi"
			},
			{
				name: "number_pop",
				description: "ialah saiz populasi"
			},
			{
				name: "cumulative",
				description: "ialah nilai logik: bagi fungsi taburan kumulatif, gunakan BENAR; fungsi kepadatan kebarangkalian, gunakan PALSU"
			}
		]
	},
	{
		name: "HYPGEOMDIST",
		description: "Mengembalikan taburan hipergeometri.",
		arguments: [
			{
				name: "sample_s",
				description: "ialah bilangan kejayaan dalam sampel"
			},
			{
				name: "number_sample",
				description: "ialah saiz sampel"
			},
			{
				name: "population_s",
				description: "ialah bilangan kejayaan dalam populasi"
			},
			{
				name: "number_pop",
				description: "ialah saiz populasi"
			}
		]
	},
	{
		name: "IF",
		description: "Semak sama ada syarat dipenuhi, dan kembalikan satu nilai jika BENAR, dan satu lagi nilai jika PALSU.",
		arguments: [
			{
				name: "logical_test",
				description: "ialah sebarang nilai atau ungkapan yang boleh dinilai sebagai BENAR atau PALSU"
			},
			{
				name: "value_if_true",
				description: "ialah nilai yang dikembalikan jika ujian_Logik adalah BENAR. Jika diabaikan, BENAR dikembalikan. Anda boleh menyarangkan sehingga tujuh fungsi JKA"
			},
			{
				name: "value_if_false",
				description: "ialah nilai yang dikembalikan jika ujian_Logik ialah PALSU. Jika diabaikan, PALSU dikembalikan"
			}
		]
	},
	{
		name: "IFERROR",
		description: "Mengembalikan value_if_error sekiranya ungkapan adalah ralat dan nilai bagi ungkapan itu sendiri adalah sebaliknya.",
		arguments: [
			{
				name: "value",
				description: "ialah sebarang nilai atau ungkapan atau rujukan"
			},
			{
				name: "value_if_error",
				description: "ialah sebarang nilai atau ungkapan atau rujukan"
			}
		]
	},
	{
		name: "IFNA",
		description: "Mengembalikan nilai yang anda tentukan jika ungkapan menyelesai kepada #N/A, jika tidak mengembalikan hasil bagi ungkapan.",
		arguments: [
			{
				name: "value",
				description: "ialah sebarang nilai atau ungkapan atau rujukan"
			},
			{
				name: "value_if_na",
				description: "ialah sebarang nilai atau ungkapan atau rujukan"
			}
		]
	},
	{
		name: "IMABS",
		description: "Mengembalikan nilai mutlak (modulus) bagi nombor kompleks.",
		arguments: [
			{
				name: "inumber",
				description: "ialah nombor kompleks untuk mana anda inginkan nilai mutlak"
			}
		]
	},
	{
		name: "IMAGINARY",
		description: "Mengembalikan pekali khayalan bagi nombor kompleks.",
		arguments: [
			{
				name: "inumber",
				description: "ialah nombor kompleks untuk mana anda inginkan pekali khayalan"
			}
		]
	},
	{
		name: "IMARGUMENT",
		description: "Mengembalikan argumen q, sudut disebut dalam radian.",
		arguments: [
			{
				name: "inumber",
				description: "ialah nombor kompleks untuk mana anda inginkan argumen"
			}
		]
	},
	{
		name: "IMCONJUGATE",
		description: "Mengembalikan konjugat kompleks bagi nombor kompleks.",
		arguments: [
			{
				name: "inumber",
				description: "ialah nombor kompleks untuk mana anda inginkan konjugat"
			}
		]
	},
	{
		name: "IMCOS",
		description: "Mengembalikan kosinus bagi nombor kompleks.",
		arguments: [
			{
				name: "inumber",
				description: "ialah nombor kompleks untuk mana anda inginkan kosinus"
			}
		]
	},
	{
		name: "IMCOSH",
		description: "Mengembalikan kosinus hiperbola bagi nombor kompleks.",
		arguments: [
			{
				name: "inumber",
				description: "ialah nombor kompleks bagi kosinus hiperbola"
			}
		]
	},
	{
		name: "IMCOT",
		description: "Mengembalikan kotangen bagi nombor kompleks.",
		arguments: [
			{
				name: "inumber",
				description: "ialah nombor kompleks yang anda inginkan bagi kotangen"
			}
		]
	},
	{
		name: "IMCSC",
		description: "Mengembalikan kosekan bagi nombor kompleks.",
		arguments: [
			{
				name: "inumber",
				description: "ialah nombor kompleks yang anda inginkan bagi kosekan"
			}
		]
	},
	{
		name: "IMCSCH",
		description: "Mengembalikan kosekan hiperbola bagi nombor kompleks.",
		arguments: [
			{
				name: "inumber",
				description: "ialah nombor kompleks yang anda inginkan bagi kosekan hiperbola"
			}
		]
	},
	{
		name: "IMDIV",
		description: "Mengembalikan hasil bahagi antara dua nombor kompleks.",
		arguments: [
			{
				name: "inumber1",
				description: "ialah pengangka kompleks atau angka yang dibahagi"
			},
			{
				name: "inumber2",
				description: "ialah penyebut kompleks atau pembahagi"
			}
		]
	},
	{
		name: "IMEXP",
		description: "Mengembalikan eksponen bagi nombor kompleks.",
		arguments: [
			{
				name: "inumber",
				description: "ialah nombor kompleks untuk mana anda inginkan eksponen"
			}
		]
	},
	{
		name: "IMLN",
		description: "Mengembalikan logaritma asli bagi nombor kompleks.",
		arguments: [
			{
				name: "inumber",
				description: "ialah nombor kompleks untuk mana anda inginkan logaritma asli"
			}
		]
	},
	{
		name: "IMLOG10",
		description: "Mengembalikan logaritma dasar-10 bagi nombor kompleks.",
		arguments: [
			{
				name: "inumber",
				description: "ialah nombor kompleks untuk mana anda inginkan logaritma biasa"
			}
		]
	},
	{
		name: "IMLOG2",
		description: "Mengembalikan logaritma dasar-2 bagi nombor kompleks.",
		arguments: [
			{
				name: "inumber",
				description: "ialah nombor kompleks untuk mana anda inginkan logaritma dasar-2"
			}
		]
	},
	{
		name: "IMPOWER",
		description: "Mengembalikan nombor kompleks yang dinaikkan ke kuasa integer.",
		arguments: [
			{
				name: "inumber",
				description: "ialah nombor kompleks yang ingin anda naikkan ke kuasa"
			},
			{
				name: "number",
				description: "ialah kuasa pada mana anda ingin menaikkan nombor kompleks"
			}
		]
	},
	{
		name: "IMPRODUCT",
		description: "Mengembalikan hasil darab bagi 1 ke 255 nombor kompleks.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "inumber1",
				description: "Inumber1, Inumber2,... adalah daripada 1 ke 255 nombor kompleks untuk didarab."
			},
			{
				name: "inumber2",
				description: "Inumber1, Inumber2,... adalah daripada 1 ke 255 nombor kompleks untuk didarab."
			}
		]
	},
	{
		name: "IMREAL",
		description: "Mengembalikan pekali nyata bagi nombor kompleks.",
		arguments: [
			{
				name: "inumber",
				description: "ialah nombor kompleks untuk mana anda inginkan pekali nyata"
			}
		]
	},
	{
		name: "IMSEC",
		description: "Mengembalikan sekan bagi nombor kompleks.",
		arguments: [
			{
				name: "inumber",
				description: "ialah nombor kompleks yang anda inginkan bagi sekan"
			}
		]
	},
	{
		name: "IMSECH",
		description: "Mengembalikan sekan hiperbola bagi nombor kompleks.",
		arguments: [
			{
				name: "inumber",
				description: "ialah nombor kompleks yang anda inginkan bagi sekan hiperbola"
			}
		]
	},
	{
		name: "IMSIN",
		description: "Mengembalikan sinus bagi nombor kompleks.",
		arguments: [
			{
				name: "inumber",
				description: "ialah nombor kompleks untuk mana anda inginkan sinus"
			}
		]
	},
	{
		name: "IMSINH",
		description: "Mengembalikan sin hiperbola bagi nombor kompleks.",
		arguments: [
			{
				name: "inumber",
				description: "ialah nombor kompleks bagi sin hiperbola"
			}
		]
	},
	{
		name: "IMSQRT",
		description: "Mengembalikan punca kuasa dua bagi nombor kompleks.",
		arguments: [
			{
				name: "inumber",
				description: "adalah nombor kompleks untuk mana anda inginkan punca kuasa dua"
			}
		]
	},
	{
		name: "IMSUB",
		description: "Mengembalikan perbezaan antara dua nombor kompleks.",
		arguments: [
			{
				name: "inumber1",
				description: "ialah nombor kompleks dari mana untuk menolak inumber2"
			},
			{
				name: "inumber2",
				description: "ialah nombor kompleks untuk menolak daripada inumber1"
			}
		]
	},
	{
		name: "IMSUM",
		description: "Mengembalikan jumlah nombor kompleks.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "inumber1",
				description: "adalah daripada 1 ke 255 nombor kompleks untuk ditambah"
			},
			{
				name: "inumber2",
				description: "adalah daripada 1 ke 255 nombor kompleks untuk ditambah"
			}
		]
	},
	{
		name: "IMTAN",
		description: "Mengembalikan tangen bagi nombor kompleks.",
		arguments: [
			{
				name: "inumber",
				description: "ialah nombor kompleks yang anda inginkan bagi tangen"
			}
		]
	},
	{
		name: "INDEX",
		description: "Mengembalikan nilai atau rujukan sel di persimpangan baris dan lajur tertentu, dalam julat yang diberikan.",
		arguments: [
			{
				name: "array",
				description: "ialah julat nilai atau pemalar tatasusunan."
			},
			{
				name: "row_num",
				description: "memilih baris dalam Tatasusunan atau Rujukan yang mengembalikan nilai. Jika diabaikan, nombor_Lajur diperlukan"
			},
			{
				name: "column_num",
				description: "pilih lajur dalam Tatasusunan atau Rujukan yang mengembalikan nilai. Jika diabaikan, nombor_Baris diperlukan"
			}
		]
	},
	{
		name: "INDIRECT",
		description: "Mengembalikan rujukan yang ditentukan oleh rentetan teks.",
		arguments: [
			{
				name: "ref_text",
				description: "ialah rujukan kepada sel yang mengandungi rujukan gaya A1- atau R1C1, nama ditakrifkan sebagai rujukan, atau rujukan kepada sel sebagai rentetan sel"
			},
			{
				name: "a1",
				description: "ialah nilai logik yang menentukan jenis rujukan dalam Ruj_teks: gaya R1C1- = PALSU; gaya A1- = BENAR atau diabaikan"
			}
		]
	},
	{
		name: "INFO",
		description: "Mengembalikan maklumat mengenai persekitaran pengendalian semasa.",
		arguments: [
			{
				name: "type_text",
				description: "ialah teks yang menentukan jenis maklumat yang ingin anda kembalikan."
			}
		]
	},
	{
		name: "INT",
		description: "Membundarkan nombor ke bawah kepada integer terdekat.",
		arguments: [
			{
				name: "number",
				description: "ialah nombor nyata yang ingin anda bundarkan ke bawah kepada satu integer"
			}
		]
	},
	{
		name: "INTERCEPT",
		description: "Mengira titik di mana satu garisan akan bersilang dengan paksi y menggunakan garisan regresi paling padan yang diplot melalui nilai x dan y yang diketahui.",
		arguments: [
			{
				name: "known_y's",
				description: "ialah set pemerhatian atau data bersandar dan mungkin nombor atau nama, tatasusunan, atau rujukan yang mengandungi nombor"
			},
			{
				name: "known_x's",
				description: "ialah set bebas pemerhatian atau data dan mungkin nombor atau nama, tatasusunan, atau rujukan yang mengandungi nombor"
			}
		]
	},
	{
		name: "INTRATE",
		description: "Mengembalikan kadar faedah bagi keselamatan pelaburan penuh.",
		arguments: [
			{
				name: "settlement",
				description: "ialah tarikh penyelesaian Keselamatan, disebut sebagai nombor tarikh bersiri"
			},
			{
				name: "maturity",
				description: "ialah tarikh kematangan Keselamatan, disebut sebagai nombor tarikh bersiri"
			},
			{
				name: "investment",
				description: "ialah jumlah yang dilabur dalam keselamatan"
			},
			{
				name: "redemption",
				description: "ialah jumlah yang akan diterima selepas matang"
			},
			{
				name: "basis",
				description: "ialah jenis berdasarkan bilangan hari untuk diguna"
			}
		]
	},
	{
		name: "IPMT",
		description: "Mengembalikan bayaran faedah bagi tempoh yang diberikan bagi pelaburan, berdasarkan tempoh berkala, bayaran malar dan kadar faedah malar.",
		arguments: [
			{
				name: "rate",
				description: "ialah kadar faedah bagi setiap tempoh. Contohnya, gunakan 6%/4 untuk bayaran suku tahun pada 6% APR"
			},
			{
				name: "per",
				description: "ialah tempoh yang anda ingin mencari faedah dan mestilah dalam julat 1 hingga Nper"
			},
			{
				name: "nper",
				description: "ialah jumlah bilangan tempoh bayaran dalam pelaburan"
			},
			{
				name: "pv",
				description: "ialah nilai semasa, atau amaun sekali gus bagi nilai siri bayaran masa depan sekarang"
			},
			{
				name: "fv",
				description: "ialah nilai masa depan, atau baki tunai yang ingin anda peroleh setelah bayaran terakhir dibuat. Jika diabaikan, Fv = 0"
			},
			{
				name: "type",
				description: "ialah nilai logik yang mewakili masa pembayaran: pada akhir tempoh = 0 atau diabaikan, pada permulaan tempoh = 1"
			}
		]
	},
	{
		name: "IRR",
		description: "Mengembalikan kadar pulangan dalaman bagi siri aliran tunai.",
		arguments: [
			{
				name: "values",
				description: "ialah tatasusunan atau rujukan sel yang mengandungi nombor yang ingin anda kirakan kadar pulangan dalaman"
			},
			{
				name: "guess",
				description: "ialah nombor yang anda teka hampir dengan keputusan IRR; 0.1 (10 peratus) jika diabaikan"
			}
		]
	},
	{
		name: "ISBLANK",
		description: "Menyemak sama ada rujukan adalah kepada sel kosong, dan mengembalikan BENAR atau PALSU.",
		arguments: [
			{
				name: "value",
				description: "ialah sel atau nama yang merujuk kepada sel yang ingin anda uji"
			}
		]
	},
	{
		name: "ISERR",
		description: "Menyemak sama ada nilai adalah ralat (#VALUE!, #REF!, #DIV/0!, #NUM!, #NAME?, atau #NULL!) tidak termasuk #N/A, dan mengembalikan BENAR atau PALSU.",
		arguments: [
			{
				name: "value",
				description: "ialah nilai yang ingin anda uji. Nilai boleh merujuk kepada sel, formula, atau nama yang merujuk kepada sel, formula atau nilai"
			}
		]
	},
	{
		name: "ISERROR",
		description: "Semak sama ada nilai adalah ralat (#N/A, #VALUE!, #REF!, #DIV/0!, #NUM!, #NAME?, or #NULL!), dan kembalikan BENAR atau PALSU.",
		arguments: [
			{
				name: "value",
				description: "ialah nilai yang ingin anda uji. Nilai boleh merujuk sel, formula, atau nama yang merujuk sel, formula, atau nilai"
			}
		]
	},
	{
		name: "ISEVEN",
		description: "Mengembalikan BENAR jika nombor adalah genap.",
		arguments: [
			{
				name: "number",
				description: "ialah nilai untuk diuji"
			}
		]
	},
	{
		name: "ISFORMULA",
		description: "Menyemak sama ada rujukan adalah kepada sel yang mengandungi formula dan mengembalikan BENAR atau PALSU.",
		arguments: [
			{
				name: "reference",
				description: "ialah rujukan kepada sel yang anda ingin uji.  Rujukan boleh menjadi rujukan sel, formula atau nama yang merujuk kepada sel"
			}
		]
	},
	{
		name: "ISLOGICAL",
		description: "Menyemak sama ada nilai ialah nilai logik (BENAR atau PALSU), dan mengembalikan BENAR atau PALSU.",
		arguments: [
			{
				name: "value",
				description: "ialah nilai yang ingin anda uji. Nilai boleh merujuk kepada sel, formula, atau nama yang merujuk kepada sel, formula, atau nilai"
			}
		]
	},
	{
		name: "ISNA",
		description: "Semak sama ada nilai adalah #N/A, dan kembalikan BENAR atau PALSU.",
		arguments: [
			{
				name: "value",
				description: "ialah nilai yang ingin anda uji. Nilai boleh merujuk sel, formula, atau nama yang merujuk sel, formula, atau nilai"
			}
		]
	},
	{
		name: "ISNONTEXT",
		description: "Menyemak sama ada nilai adalah bukan teks (sel kosong adalah bukan teks), dan mengembalikan BENAR atau PALSU.",
		arguments: [
			{
				name: "value",
				description: "ialah nilai yang ingin anda uji: sel, formula, atau nama yang merujuk kepada sel, formula, atau nilai"
			}
		]
	},
	{
		name: "ISNUMBER",
		description: "Menyemak sama ada nilai ialah nombor, dan mengembalikan BENAR atau PALSU.",
		arguments: [
			{
				name: "value",
				description: "ialah nilai yang ingin anda uji. Nilai boleh merujuk kepada sel, formula, atau nama yang merujuk kepada sel, formula atau nilai"
			}
		]
	},
	{
		name: "ISO.CEILING",
		description: "Membundarkan nombor, kepada integer terdekat atau kepada bilangan keertian terdekat.",
		arguments: [
			{
				name: "number",
				description: "ialah nilai yang anda ingin bundarkan"
			},
			{
				name: "significance",
				description: "ialah bilangan pilihan yang anda ingin bundarkan"
			}
		]
	},
	{
		name: "ISODD",
		description: "Mengembalikan BENAR jika nombor adalah ganjil.",
		arguments: [
			{
				name: "number",
				description: "ialah nilai untuk diuji"
			}
		]
	},
	{
		name: "ISOWEEKNUM",
		description: "Mengembalikan bilangan nombor minggu ISO bagi tahun untuk tarikh tertentu.",
		arguments: [
			{
				name: "date",
				description: "ialah kod tarikh masa yang digunakan oleh Spreadsheet untuk pengiraan tarikh dan masa"
			}
		]
	},
	{
		name: "ISPMT",
		description: "Mengembalikan faedah yang dibayar semasa tempoh pelaburan yang tertentu.",
		arguments: [
			{
				name: "rate",
				description: "kadar faedah bagi setiap tempoh. Contohnya, gunakan 6%/4 untuk bayaran setiap suku tahun pada 6% APR"
			},
			{
				name: "per",
				description: "tempoh yang anda ingin mendapatkan faedah"
			},
			{
				name: "nper",
				description: "bilangan tempoh bayaran dalam pelaburan"
			},
			{
				name: "pv",
				description: "jumlah sekali gus apabila siri bayaran masa depan adalah betul sekarang"
			}
		]
	},
	{
		name: "ISREF",
		description: "Menyemak sama ada nilai ialah rujukan, dan mengembalikan BENAR atau PALSU.",
		arguments: [
			{
				name: "value",
				description: "ialah nilai yang ingin anda uji. Nilai boleh merujuk sel, formula, atau nama yang merujuk sel, formula atau nilai"
			}
		]
	},
	{
		name: "ISTEXT",
		description: "Menyemak sama ada nilai ialah teks, dan mengembalikan BENAR atau PALSU.",
		arguments: [
			{
				name: "value",
				description: "ialah nilai yang ingin anda uji. Nilai boleh merujuk kepada sel, formula, atau nama yang merujuk kepada sel, formula, atau nilai"
			}
		]
	},
	{
		name: "KURT",
		description: "Mengembalikan kurtosis set data.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "ialah 1 hingga 255 nombor atau nama, tatasusunan, atau rujukan yang mengandungi nombor yang anda inginkan kurtosisnya"
			},
			{
				name: "number2",
				description: "ialah 1 hingga 255 nombor atau nama, tatasusunan, atau rujukan yang mengandungi nombor yang anda inginkan kurtosisnya"
			}
		]
	},
	{
		name: "LARGE",
		description: "Mengembalikan nilai terbesar ke-k dalam set data. Contohnya, nombor terbesar kelima.",
		arguments: [
			{
				name: "array",
				description: "ialah tatasusunan atau julat data yang ingin anda tentukan nilai terbesar ke-k"
			},
			{
				name: "k",
				description: "ialah posisi (daripada yang terbesar) dalam tatasusunan atau julat sel nilai untuk dikembalikan"
			}
		]
	},
	{
		name: "LCM",
		description: "Mengembalikan gandaan biasa terkurang.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "adalah 1 hingga 255 nilai untuk mana anda inginkan gandaan biasa terkurang"
			},
			{
				name: "number2",
				description: "adalah 1 hingga 255 nilai untuk mana anda inginkan gandaan biasa terkurang"
			}
		]
	},
	{
		name: "LEFT",
		description: "Mengembalikan bilangan aksara yang ditentukan dari permulaan rentetan teks.",
		arguments: [
			{
				name: "text",
				description: "ialah rentetan teks yang mengandungi aksara yang ingin anda ekstrak"
			},
			{
				name: "num_chars",
				description: "Menentukan berapa banyak aksara yang anda ingin KIRI untuk mengekstrak; 1 jika diabaikan"
			}
		]
	},
	{
		name: "LEN",
		description: "Mengembalikan bilangan aksara dalam rentetan teks.",
		arguments: [
			{
				name: "text",
				description: "ialah teks yang ingin anda cari panjangnya. Ruang dikira sebagai aksara"
			}
		]
	},
	{
		name: "LINEST",
		description: "Mengembalikan statistik yang memerihalkan titik data yang diketahui padanan arah aliran linear, dengan memuatkan garis lurus menggunakan kaedah kuasa dua terkecil.",
		arguments: [
			{
				name: "known_y's",
				description: "ialah set nilai y yang anda telah tahu dalam hubungan y = mx + b"
			},
			{
				name: "known_x's",
				description: "ialah set pilihan nilai x yang mungkin anda telah tahu dalam hubungan y = mx + b"
			},
			{
				name: "const",
				description: "ialah nilai logik: pemalar b dikira biasanya jika Const = BENAR atau diabaikan; b ialah set yang bersamaan dengan 0 jika Const = PALSU"
			},
			{
				name: "stats",
				description: "ialah nilai logik: kembalikan statistik regresi tambahan = BENAR; kembalikan pekali m dan pemalar b = PALSU atau diabaikan"
			}
		]
	},
	{
		name: "LN",
		description: "Mengembalikan logaritma asli bagi nombor.",
		arguments: [
			{
				name: "number",
				description: "ialah nombor nyata positif yang anda inginkan logaritma aslinya"
			}
		]
	},
	{
		name: "LOG",
		description: "Mengembalikan logaritma nombor kepada asas yang anda tentukan.",
		arguments: [
			{
				name: "number",
				description: "ialah nombor nyata positif yang anda inginkan logaritmanya"
			},
			{
				name: "base",
				description: "ialah asas logaritma; 10 jika diabaikan"
			}
		]
	},
	{
		name: "LOG10",
		description: "Mengembalikan logaritma asas-10 bagi nombor.",
		arguments: [
			{
				name: "number",
				description: "ialah nombor nyata positif yang anda inginkan logaritma asas-10"
			}
		]
	},
	{
		name: "LOGEST",
		description: "Mengembalikan statistik yang memerihalkan titik data yang diketahui padanan lengkok eksponen.",
		arguments: [
			{
				name: "known_y's",
				description: "ialah set nilai y yang telah anda tahu dalam hubungan y = b*m^x"
			},
			{
				name: "known_x's",
				description: "ialah set pilihan nilai x yang mungkin anda telah tahu dalam hubungan y = b*m^x"
			},
			{
				name: "const",
				description: "ialah nilai logik: pemalar b dikira biasanya jika Const = BENAR atau diabaikan; b diset bersamaan dengan 1 jika Const = PALSU"
			},
			{
				name: "stats",
				description: "ialah nilai logik: kembalikan statistik regresi tambahan = BENAR; kembalikan pekali m dan pemalar b = PALSU atau diabaikan"
			}
		]
	},
	{
		name: "LOGINV",
		description: "Mengembalikan fungsi songsang taburan lognormal kumulatif x, di mana ln(x) biasanya tertabur dengan parameter Min dan Sisihan_piawai.",
		arguments: [
			{
				name: "probability",
				description: "ialah kebarangkalian yang dikaitkan dengan taburan lognormal, termasuk nombor 0 hingga 1"
			},
			{
				name: "mean",
				description: "ialah min ln(x)"
			},
			{
				name: "standard_dev",
				description: "ialah sisihan piawai ln(x), nombor positif"
			}
		]
	},
	{
		name: "LOGNORM.DIST",
		description: "Mengembalikan taburan lognormal x, yang mana ln(x) biasanya tertabur dengan parameter Min dan sisihan_Piawai.",
		arguments: [
			{
				name: "x",
				description: "ialah nilai untuk menilai fungsi, nombor positif"
			},
			{
				name: "mean",
				description: "ialah min ln(x)"
			},
			{
				name: "standard_dev",
				description: "ialah sisihan piawai ln(x), nombor positif"
			},
			{
				name: "cumulative",
				description: "ialah nilai logik: bagi fungsi taburan kumulatif, gunakan BENAR; bagi fungsi kepadatan kebarangkalian, gunakan PALSU"
			}
		]
	},
	{
		name: "LOGNORM.INV",
		description: "Mengembalikan fungsi songsang taburan lognormal kumulatif x, di mana ln(x) biasanya tertabur dengan parameter Min dan Sisihan_piawai.",
		arguments: [
			{
				name: "probability",
				description: "ialah kebarangkalian yang dikaitkan dengan taburan lognormal, termasuk nombor 0 hingga 1"
			},
			{
				name: "mean",
				description: "ialah min ln(x)"
			},
			{
				name: "standard_dev",
				description: "ialah sisihan piawai ln(x), nombor positif"
			}
		]
	},
	{
		name: "LOGNORMDIST",
		description: "Mengembalikan taburan lognormal kumulatif x, di mana ln(x) biasanya tertabur dengan parameter Min dan Sisihan_piawai.",
		arguments: [
			{
				name: "x",
				description: "ialah nilai untuk menilai fungsi, nombor positif"
			},
			{
				name: "mean",
				description: "ialah min ln(x)"
			},
			{
				name: "standard_dev",
				description: "ialah sisihan piawai ln(x), nombor positif"
			}
		]
	},
	{
		name: "LOOKUP",
		description: "Mencari nilai daripada julat satu baris atau satu lajur atau daripada satu tatasusunan. Diberikan keserasian ke belakang.",
		arguments: [
			{
				name: "lookup_value",
				description: "ialah nilai yang CARIAN mencari dalam vektor_Carian dan mungkin nombor, teks, nilai logik, atau nama, atau rujukan nilai"
			},
			{
				name: "lookup_vector",
				description: "ialah julat yang mengandungi hanya satu baris atau satu lajur teks, nombor, atau nilai logik, diletakkan dalam tertib menaik"
			},
			{
				name: "result_vector",
				description: "ialah julat yang mengandungi hanya satu baris atau lajur, saiz yang sama dengan vektor_Carian"
			}
		]
	},
	{
		name: "LOWER",
		description: "Menukarkan semua huruf dalam rentetan teks kepada huruf kecil.",
		arguments: [
			{
				name: "text",
				description: "ialah teks yang ingin anda tukarkan kepada huruf kecil. Aksara dalam Teks yang bukannya huruf tidak ditukar"
			}
		]
	},
	{
		name: "MATCH",
		description: "Mengembalikan posisi relatif item dalam tatasusunan yang sepadan dengan nilai yang ditentukan dalam tertib tertentu.",
		arguments: [
			{
				name: "lookup_value",
				description: "ialah nilai yang anda gunakan untuk mencari nilai yang anda inginkan dalam tatasusunan, nombor, teks, atau nilai logik, atau rujukan kepada salah satu daripada ini"
			},
			{
				name: "lookup_array",
				description: "ialah julat sel bersebelahan yang mengandungi nilai carian, tatasusunan nilai, atau rujukan tatasusunan yang mungkin"
			},
			{
				name: "match_type",
				description: "ialah nombor 1, 0, atau -1 menunjukkan nilai mana yang akan dikembalikan."
			}
		]
	},
	{
		name: "MAX",
		description: "Mengembalikan nilai terbesar dalam set nilai. Mengabaikan nilai dan teks logik.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "ialah 1 hingga 255 nombor, sel kosong, nilai logik, atau teks nombor yang anda inginkan maksimumnya"
			},
			{
				name: "number2",
				description: "ialah 1 hingga 255 nombor, sel kosong, nilai logik, atau teks nombor yang anda inginkan maksimumnya"
			}
		]
	},
	{
		name: "MAXA",
		description: "Mengembalikan nilai terbesar dalam set nilai. Tidak mengabaikan nilai logik dan teks.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "ialah 1 hingga 255 nombor, sel kosong, nilai logik, atau nombor teks yang anda inginkan maksimumnya"
			},
			{
				name: "value2",
				description: "ialah 1 hingga 255 nombor, sel kosong, nilai logik, atau nombor teks yang anda inginkan maksimumnya"
			}
		]
	},
	{
		name: "MDETERM",
		description: "Mengembalikan matriks penentu tatasusunan.",
		arguments: [
			{
				name: "array",
				description: "ialah tatasusunan angka dengan bilangan baris dan lajur yang sama, sama ada julat sel atau pemalar tatasusunan"
			}
		]
	},
	{
		name: "MEDIAN",
		description: "Mengembalikan median, atau nombor di tengah set nombor yang diberikan.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "ialah 1 hingga 255 nombor atau nama, tatasusunan, atau rujukan yang mengandungi nombor yang anda inginkan mediannya"
			},
			{
				name: "number2",
				description: "ialah 1 hingga 255 nombor atau nama, tatasusunan, atau rujukan yang mengandungi nombor yang anda inginkan mediannya"
			}
		]
	},
	{
		name: "MID",
		description: "Mengembalikan aksara dari tengah rentetan teks, diberikan posisi mula dan panjang.",
		arguments: [
			{
				name: "text",
				description: "ialah rentetan teks yang ingin anda ekstrak aksaranya"
			},
			{
				name: "start_num",
				description: "ialah posisi aksara pertama yang ingin anda ekstrak. Aksara pertama dalam Teks ialah 1"
			},
			{
				name: "num_chars",
				description: "menentukan berapa banyak aksara dikembalikan daripada Teks"
			}
		]
	},
	{
		name: "MIN",
		description: "Mengembalikan nombor terkecil dalam set nilai. Mengabaikan nilai dan teks logik.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "ialah 1 hingga 255 nombor, sel kosong, nilai logik, atau nombor teks yang anda inginkan minimumnya"
			},
			{
				name: "number2",
				description: "ialah 1 hingga 255 nombor, sel kosong, nilai logik, atau nombor teks yang anda inginkan minimumnya"
			}
		]
	},
	{
		name: "MINA",
		description: "Mengembalikan nilai terkecil dalam set nilai. Tidak mengabaikan nilai logik dan teks.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "ialah 1 hingga 255 nombor, sel kosong, nilai logik, atau nombor teks yang anda inginkan nilai minimumnya"
			},
			{
				name: "value2",
				description: "ialah 1 hingga 255 nombor, sel kosong, nilai logik, atau nombor teks yang anda inginkan nilai minimumnya"
			}
		]
	},
	{
		name: "MINUTE",
		description: "Mengembalikan minit, nombor bermula 0 hingga 59.",
		arguments: [
			{
				name: "serial_number",
				description: "ialah nombor dalam kod tarikh masa yang digunakan oleh Spreadsheet atau teks dalam format masa, seperti 16:48:00 atau 4:48:00 PM"
			}
		]
	},
	{
		name: "MINVERSE",
		description: "Mengembalikan matriks songsang bagi matriks yang disimpan dalam tatasusunan.",
		arguments: [
			{
				name: "array",
				description: "ialah tatasusunan angka dengan bilangan baris dan lajur yang sama, sama ada julat sel atau pemalar tatasusunan"
			}
		]
	},
	{
		name: "MIRR",
		description: "Mengembalikan kadar pulangan dalaman bagi siri aliran tunai berkala, dengan mengambil kira kos pelaburan dan faedah atas pelaburan semula tunai.",
		arguments: [
			{
				name: "values",
				description: "ialah tatasusunan atau rujukan sel yang mengandungi nombor yang mewakili siri bayaran (negatif) dan pendapatan (positif) pada tempoh tetap"
			},
			{
				name: "finance_rate",
				description: "ialah kadar faedah yang anda bayar atas wang digunakan dalam aliran tunai"
			},
			{
				name: "reinvest_rate",
				description: "ialah kadar faedah yang anda terima atas aliran tunai semasa anda melaburkannya semula"
			}
		]
	},
	{
		name: "MMULT",
		description: "Mengembalikan produk matriks dua tatasusunan, tatasusunan dengan bilangan baris yang sama seperti tatasusunan1 dan lajur seperti tatasusunan2.",
		arguments: [
			{
				name: "array1",
				description: "ialah tatasusunan pertama nombor untuk digandakan dan mestilah mempunyai bilangan lajur yang sama seperti bilangan baris dalam Tatasusunan2"
			},
			{
				name: "array2",
				description: "ialah tatasusunan pertama nombor untuk digandakan dan mestilah mempunyai bilangan lajur yang sama seperti bilangan baris dalam Tatasusunan2"
			}
		]
	},
	{
		name: "MOD",
		description: "Mengembalikan baki setelah nombor dibahagikan dengan pembahagi.",
		arguments: [
			{
				name: "number",
				description: "ialah nombor yang ingin anda cari baki setelah pembahagian dilaksanakan"
			},
			{
				name: "divisor",
				description: "ialah nombor dengan mana ingin anda bahagikan Nombor"
			}
		]
	},
	{
		name: "MODE",
		description: "Mengembalikan nilai yang paling kerap muncul, atau berulang, dalam tatasusunan atau julat data.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "ialah 1 hingga 255 nombor, atau nama, tatasusunan, atau rujukan yang mengandungi nombor yang anda inginkan modnya"
			},
			{
				name: "number2",
				description: "ialah 1 hingga 255 nombor, atau nama, tatasusunan, atau rujukan yang mengandungi nombor yang anda inginkan modnya"
			}
		]
	},
	{
		name: "MODE.MULT",
		description: "Mengembalikan tatasusunan menegak bagi yang paling kerap berlaku, atau berulang, nilai dalam tatasusunan atau julat data.  Bagi tatasusunan mendatar, gunakan =TRANSPOSE(MODE.MULT(number1,number2,...)).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "adalah 1 hingga 255 nombor, atau nama, tatasusunan, atau rujukan yang mengandungi nombor yang anda inginkan modnya"
			},
			{
				name: "number2",
				description: "adalah 1 hingga 255 nombor, atau nama, tatasusunan, atau rujukan yang mengandungi nombor yang anda inginkan modnya"
			}
		]
	},
	{
		name: "MODE.SNGL",
		description: "Mengembalikan nilai yang paling kerap muncul, atau berulang, dalam tatasusunan atau julat data.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "ialah 1 hingga 255 nombor, atau nama, tatasusunan, atau rujukan yang mengandungi nombor yang anda inginkan modnya"
			},
			{
				name: "number2",
				description: "ialah 1 hingga 255 nombor, atau nama, tatasusunan, atau rujukan yang mengandungi nombor yang anda inginkan modnya"
			}
		]
	},
	{
		name: "MONTH",
		description: "Mengembalikan bulan, nombor bermula 1 (Januari) hingga 12 (Disember).",
		arguments: [
			{
				name: "serial_number",
				description: "ialah nombor dalam kod tarikh masa yang digunakan oleh Spreadsheet"
			}
		]
	},
	{
		name: "MROUND",
		description: "Mengembalikan nombor yang dibundarkan kepada gandaan yang dikehendaki.",
		arguments: [
			{
				name: "number",
				description: "ialah nilai untuk dibundarkan"
			},
			{
				name: "multiple",
				description: "ialah gandaan kepada mana anda ingin membundarkan nombor"
			}
		]
	},
	{
		name: "MULTINOMIAL",
		description: "Mengembalikan multinomial untuk satu set nombor.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "adalah 1 hingga 255 nilai untuk mana anda ingin multinomial"
			},
			{
				name: "number2",
				description: "adalah 1 hingga 255 nilai untuk mana anda ingin multinomial"
			}
		]
	},
	{
		name: "MUNIT",
		description: "Mengembalikan matriks unit untuk dimensi tertentu.",
		arguments: [
			{
				name: "dimension",
				description: "ialah integer yang menentukan dimensi bagi matriks unit yang anda ingin kembalikan"
			}
		]
	},
	{
		name: "N",
		description: "Menukarkan nilai bukan nombor kepada nombor, tarikh kepada nombor siri, BENAR kepada 1, yang lain kepada 0 (sifar).",
		arguments: [
			{
				name: "value",
				description: "ialah nilai yang ingin anda tukar"
			}
		]
	},
	{
		name: "NA",
		description: "Mengembalikan nilai ralat #N/A (nilai tidak tersedia).",
		arguments: [
		]
	},
	{
		name: "NEGBINOM.DIST",
		description: "Mengembalikan taburan binomial negatif, kebarangkalian adanya kegagalan Nombor_f sebelum kejayaan Nombor_ke-s, dengan Kebarangkalian_s kejayaan.",
		arguments: [
			{
				name: "number_f",
				description: "ialah bilangan kegagalan"
			},
			{
				name: "number_s",
				description: "ialah bilangan ambang kejayaan"
			},
			{
				name: "probability_s",
				description: "ialah kebarangkalian kejayaan; nombor antara 0 dan 1"
			},
			{
				name: "cumulative",
				description: "ialah nilai logik: bagi fungsi taburan kumulatif, gunakan BENAR; bagi fungsi kepadatan kebarangkalian, gunakan PALSU"
			}
		]
	},
	{
		name: "NEGBINOMDIST",
		description: "Mengembalikan taburan binomial negatif, kebarangkalian adanya kegagalan Nombor_f sebelum kejayaan Nombor_ke-s, dengan Kebarangkalian_s kejayaan.",
		arguments: [
			{
				name: "number_f",
				description: "ialah bilangan kegagalan"
			},
			{
				name: "number_s",
				description: "ialah bilangan ambang kejayaan"
			},
			{
				name: "probability_s",
				description: "ialah kebarangkalian kejayaan; nombor antara 0 dan 1"
			}
		]
	},
	{
		name: "NETWORKDAYS",
		description: "Mengembalikan bilangan keseluruhan hari kerja di antara dua tarikh.",
		arguments: [
			{
				name: "start_date",
				description: "ialah nombor tarikh bersiri yang mewakili tarikh mula"
			},
			{
				name: "end_date",
				description: "ialah nombor tarikh bersiri yang mewakili tarikh tamat"
			},
			{
				name: "holidays",
				description: "ialah set pilihan bagi satu nombor tarikh bersiri atau lebih untuk disisihkan daripada kalendar kerja, seperti hari cuti negeri dan persekutuan dan hari cuti terapung"
			}
		]
	},
	{
		name: "NETWORKDAYS.INTL",
		description: "Mengembalikan bilangan keseluruhan hari bekerja antara dua tarikh dengan parameter hujung minggu tersuai.",
		arguments: [
			{
				name: "start_date",
				description: "ialah nombor tarikh bersiri yang mewakili tarikh mula"
			},
			{
				name: "end_date",
				description: "ialah nombor tarikh bersiri yang mewakili tarikh akhir"
			},
			{
				name: "weekend",
				description: "ialah nombor atau rentetan menentukan masa hujung minggu berlaku"
			},
			{
				name: "holidays",
				description: "ialah set pilihan bagi satu atau lebih nombor tarikh bersiri untuk dikecualikan daripada kalendar kerja, seperti cuti negeri dan persekutuan dan cuti apung"
			}
		]
	},
	{
		name: "NOMINAL",
		description: "Mengembalikan kadar faedah nominal tahunan.",
		arguments: [
			{
				name: "effect_rate",
				description: "ialah kadar faedah efektif"
			},
			{
				name: "npery",
				description: "ialah bilangan tempoh pengkompaunan setahun"
			}
		]
	},
	{
		name: "NORM.DIST",
		description: "Mengembalikan taburan normal untuk min yang ditentukan dan sisihan piawai.",
		arguments: [
			{
				name: "x",
				description: "adalah nilai yang anda inginkan taburannya"
			},
			{
				name: "mean",
				description: "adalah min aritmetik taburan"
			},
			{
				name: "standard_dev",
				description: "adalah sisihan piawai taburan, suatu nombor positif"
			},
			{
				name: "cumulative",
				description: "adalah nilai logikal: untuk fungsi taburan kumulatif, gunakan TRUE; untuk fungsi ketumpatan kebarangkalian, gunakan FALSE"
			}
		]
	},
	{
		name: "NORM.INV",
		description: "Mengembalikan songsangan taburan kumulatif normal bagi min dan sisihan piawai yang ditentukan.",
		arguments: [
			{
				name: "probability",
				description: "ialah kebarangkalian yang sepadan dengan taburan normal, termasuk nombor 0 hingga 1"
			},
			{
				name: "mean",
				description: "ialah min aritmetik taburan"
			},
			{
				name: "standard_dev",
				description: "ialah sisihan piawai taburan, nombor positif"
			}
		]
	},
	{
		name: "NORM.S.DIST",
		description: "Mengembalikan taburan normal standard (mempunyai min sifar dan sisihan piawai satu).",
		arguments: [
			{
				name: "z",
				description: "ialah nilai yang anda inginkan taburan"
			},
			{
				name: "cumulative",
				description: "ialah nilai logik bagi fungsi untuk dikembalikan: fungsi taburan kumulatif = BENAR; fungsi kepadatan kebarangkalian = PALSU"
			}
		]
	},
	{
		name: "NORM.S.INV",
		description: "Mengembalikan songsangan taburan kumulatif normal standard (mempunyai min sifar dan sisihan piawai satu).",
		arguments: [
			{
				name: "probability",
				description: "ialah kebarangkalian yang sepadan dengan taburan normal, termasuk nombor 0 hingga 1"
			}
		]
	},
	{
		name: "NORMDIST",
		description: "Mengembalikan taburan kumulatif normal bagi min dan sisihan piawai yang ditentukan.",
		arguments: [
			{
				name: "x",
				description: "adalah nilai yang anda inginkan taburan"
			},
			{
				name: "mean",
				description: "adalah min aritmetik taburan"
			},
			{
				name: "standard_dev",
				description: "ialah sisihan piawai taburan, nombor positif"
			},
			{
				name: "cumulative",
				description: "adalah nilai logik: bagi fungsi taburan kumulatif, gunakan TRUE; bagi fungsi jisim kebarangkalian, gunakan FALSE"
			}
		]
	},
	{
		name: "NORMINV",
		description: "Mengembalikan songsangan taburan kumulatif normal bagi min dan sisihan piawai yang ditentukan.",
		arguments: [
			{
				name: "probability",
				description: "ialah kebarangkalian yang sepadan dengan taburan normal, termasuk nombor 0 hingga 1"
			},
			{
				name: "mean",
				description: "ialah min aritmetik taburan"
			},
			{
				name: "standard_dev",
				description: "ialah sisihan piawai taburan, nombor positif"
			}
		]
	},
	{
		name: "NORMSDIST",
		description: "Mengembalikan taburan kumulatif normal standard (mempunyai min sifar dan sisihan piawai satu).",
		arguments: [
			{
				name: "z",
				description: "ialah nilai yang anda inginkan taburan"
			}
		]
	},
	{
		name: "NORMSINV",
		description: "Mengembalikan songsangan taburan kumulatif normal standard (mempunyai min sifar dan sisihan piawai satu).",
		arguments: [
			{
				name: "probability",
				description: "ialah kebarangkalian yang sepadan dengan taburan normal, termasuk nombor 0 hingga 1"
			}
		]
	},
	{
		name: "NOT",
		description: "Menukarkan PALSU kepada BENAR, atau BENAR kepada PALSU.",
		arguments: [
			{
				name: "logical",
				description: "ialah nilai atau ungkapan yang boleh dinilai sebagai BENAR atau PALSU"
			}
		]
	},
	{
		name: "NOW",
		description: "Mengembalikan tarikh dan masa semasa yang diformatkan sebagai tarikh dan masa.",
		arguments: [
		]
	},
	{
		name: "NPER",
		description: "Mengembalikan bilangan tempoh pelaburan berdasarkan tempoh berkala, bayaran malar dan kadar faedah malar.",
		arguments: [
			{
				name: "rate",
				description: "ialah kadar faedah bagi setiap tempoh. Contohnya, gunakan 6%/4 bagi bayaran setiap suku tahun pada 6% APR"
			},
			{
				name: "pmt",
				description: "ialah bayaran dibuat bagi setiap tempoh; ia tidak boleh diubah sepanjang tempoh pelaburan"
			},
			{
				name: "pv",
				description: "ialah nilai semasa, atau jumlah sekali gus bagi nilai siri bayaran masa depan sekarang"
			},
			{
				name: "fv",
				description: "ialah nilai masa depan, atau baki tunai yang ingin anda peroleh setelah bayaran terakhir dibuat. Jika diabaikan, sifar digunakan"
			},
			{
				name: "type",
				description: "ialah nilai logik: bayaran pada permulaan tempoh = 1; bayaran pada akhir tempoh = 0 atau diabaikan"
			}
		]
	},
	{
		name: "NPV",
		description: "Mengembalikan nilai kini bersih bagi pelaburan berdasarkan kadar diskaun dan siri bayaran (nilai negatif) dan pendapatan (nilai positif) masa depan.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "rate",
				description: "ialah kadar diskaun ke atas satu tempoh masa"
			},
			{
				name: "value1",
				description: "ialah 1 hingga 254 bayaran dan pendapatan, dijarakkan masanya secara sama rata dan berlaku di hujung setiap tempoh"
			},
			{
				name: "value2",
				description: "ialah 1 hingga 254 bayaran dan pendapatan, dijarakkan masanya secara sama rata dan berlaku di hujung setiap tempoh"
			}
		]
	},
	{
		name: "NUMBERVALUE",
		description: "Menukar teks kepada nombor dengan cara bebas tempat.",
		arguments: [
			{
				name: "text",
				description: "ialah rentetan yang mewakili nombor yang anda ingin tukarkan"
			},
			{
				name: "decimal_separator",
				description: "ialah aksara yang digunakan sebagai pemisah perpuluhan dalam rentetan"
			},
			{
				name: "group_separator",
				description: "ialah aksara yang digunakan sebagai pemisah kumpulan dalam rentetan"
			}
		]
	},
	{
		name: "OCT2BIN",
		description: "Menukar nombor perlapanan kepada perduaan.",
		arguments: [
			{
				name: "number",
				description: "ialah nombor perlapanan yang ingin anda tukar"
			},
			{
				name: "places",
				description: "ialah bilangan aksara untuk digunakan"
			}
		]
	},
	{
		name: "OCT2DEC",
		description: "Menukar nombor perlapanan kepada perpuluhan.",
		arguments: [
			{
				name: "number",
				description: "ialah nombor perlapanan yang ingin anda tukar"
			}
		]
	},
	{
		name: "OCT2HEX",
		description: "Menukar nombor perlapanan kepada perenambelasan.",
		arguments: [
			{
				name: "number",
				description: "ialah nombor perlapanan yang ingin anda tukar"
			},
			{
				name: "places",
				description: "ialah bilangan aksara untuk digunakan"
			}
		]
	},
	{
		name: "ODD",
		description: "Membundarkan nombor positif ke atas dan nombor negatif ke bawah kepada integer ganjil yang terdekat.",
		arguments: [
			{
				name: "number",
				description: "ialah nilai untuk dibundarkan"
			}
		]
	},
	{
		name: "OFFSET",
		description: "Mengembalikan rujukan kepada julat, iaitu bilangan baris dan lajur yang diberikan daripada rujukan yang diberikan.",
		arguments: [
			{
				name: "reference",
				description: "ialah rujukan yang anda inginkan asas ofset, rujukan kepada sel atau julat sel bersebelahan"
			},
			{
				name: "rows",
				description: "ialah bilangan baris, atas atau bawah, yang anda inginkan keputusan sel atas kiri dirujuk pada"
			},
			{
				name: "cols",
				description: "ialah bilangan lajur, ke sebelah kiri atau kanan, yang anda inginkan keputusan sel atas-kiri dirujuk pada"
			},
			{
				name: "height",
				description: "ialah tinggi, dalam bilangan baris, yang anda inginkan keputusannya, ketinggian yang sama seperti Rujukan jika diabaikan"
			},
			{
				name: "width",
				description: "ialah lebar, dalam bilangan lajur, yang anda inginkan keputusannya, lebar yang sama seperti Rujukan jika diabaikan"
			}
		]
	},
	{
		name: "OR",
		description: "Menyemak sama ada sebarang argumen adalah BENAR, dan mengembalikan BENAR atau PALSU. Mengembalikan PALSU hanya jika semua argumen adalah PALSU.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logical1",
				description: "ialah 1 hingga 255 syarat yang ingin anda uji sama ada BENAR atau PALSU"
			},
			{
				name: "logical2",
				description: "ialah 1 hingga 255 syarat yang ingin anda uji sama ada BENAR atau PALSU"
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
		description: "Mengembalikan bilangan tempoh yang diperlukan oleh pelaburan untuk mencapai nilai tertentu.",
		arguments: [
			{
				name: "rate",
				description: "ialah kadar faedah per tempoh."
			},
			{
				name: "pv",
				description: "ialah nilai semasa bagi pelaburan"
			},
			{
				name: "fv",
				description: "ialah nilai masa depan yang dikehendaki bagi pelaburan"
			}
		]
	},
	{
		name: "PEARSON",
		description: "Mengembalikan pekali korelasi momen hasil darab Pearson, r.",
		arguments: [
			{
				name: "array1",
				description: "ialah set nilai bebas"
			},
			{
				name: "array2",
				description: "ialah set nilai bersandar"
			}
		]
	},
	{
		name: "PERCENTILE",
		description: "Mengembalikan nilai persentil ke-k dalam julat.",
		arguments: [
			{
				name: "array",
				description: "ialah tatasusunan atau julat data yang mentakrif kedudukan relatif"
			},
			{
				name: "k",
				description: "ialah nilai persentil termasuk 0 hingga 1"
			}
		]
	},
	{
		name: "PERCENTILE.EXC",
		description: "Mengembalikan persentil ke-k nilai dalam julat, yang mana k adalah dalam julat 0..1, eksklusif.",
		arguments: [
			{
				name: "array",
				description: "adalah tatasusunan atau julat data yang menentukan kedudukan relatif"
			},
			{
				name: "k",
				description: "adalah nilai persentil antara 0 hingga 1, inklusif"
			}
		]
	},
	{
		name: "PERCENTILE.INC",
		description: "Mengembalikan persentil ke-k nilai dalam julat, yang mana k adalah dalam julat 0..1, inklusif.",
		arguments: [
			{
				name: "array",
				description: "adalah tatasusunan atau julat data yang menentukan kedudukan relatif"
			},
			{
				name: "k",
				description: "adalah nilai persentil yang berada antara 0 hingga 1, inklusif"
			}
		]
	},
	{
		name: "PERCENTRANK",
		description: "Mengembalikan kedudukan nilai dalam set data sebagai peratus set data.",
		arguments: [
			{
				name: "array",
				description: "ialah tatasusunan atau julat data dengan nilai angka yang mentakrif kedudukan relatif"
			},
			{
				name: "x",
				description: "ialah nilai yang ingin anda tahu kedudukannya"
			},
			{
				name: "significance",
				description: "ialah nilai pilihan yang mengecam nombor daripada digit bererti bagi peratus yang dikembalikan, tiga digit jika diabaikan (0.xxx%)"
			}
		]
	},
	{
		name: "PERCENTRANK.EXC",
		description: "Mengembalikan kedudukan nilai dalam set data sebagai peratus set data sebagai peratus (0..1, eksklusif) bagi set data.",
		arguments: [
			{
				name: "array",
				description: "ialah tatasusunan atau julat data dengan nilai angka yang mentakrif kedudukan relatif"
			},
			{
				name: "x",
				description: "ialah nilai yang ingin anda tahu kedudukannya"
			},
			{
				name: "significance",
				description: "ialah nilai pilihan yang mengecam nombor daripada digit bererti bagi peratus yang dikembalikan, tiga digit jika diabaikan (0.xxx%)"
			}
		]
	},
	{
		name: "PERCENTRANK.INC",
		description: "Mengembalikan kedudukan nilai dalam set data sebagai peratus set data sebagai peratus (0..1, inklusif) bagi set data.",
		arguments: [
			{
				name: "array",
				description: "ialah tatasusunan atau julat data dengan nilai angka yang mentakrif kedudukan relatif"
			},
			{
				name: "x",
				description: "ialah nilai yang ingin anda tahu kedudukannya"
			},
			{
				name: "significance",
				description: "ialah nilai pilihan yang mengenal pasti nombor bagi digit penting untuk peratus yang dikembalikan, tiga digit jika diabaikan (0.xxx%)"
			}
		]
	},
	{
		name: "PERMUT",
		description: "Mengembalikan bilangan pilih atur bagi bilangan objek yang diberikan yang boleh dipilih daripada jumlah objek.",
		arguments: [
			{
				name: "number",
				description: "ialah jumlah bilangan objek"
			},
			{
				name: "number_chosen",
				description: "ialah nombor objek dalam setiap pilih atur"
			}
		]
	},
	{
		name: "PERMUTATIONA",
		description: "Mengembalikan bilangan pilih atur bagi bilangan objek (dengan ulangan) yang boleh dipilih daripada jumlah objek.",
		arguments: [
			{
				name: "number",
				description: "ialah jumlah objek"
			},
			{
				name: "number_chosen",
				description: "ialah bilangan objek dalam setiap pilih atur"
			}
		]
	},
	{
		name: "PHI",
		description: "Mengembalikan nilai fungsi kepadatan untuk taburan normal standard.",
		arguments: [
			{
				name: "x",
				description: "ialah bilangan yang anda inginkan untuk kepadatan bagi taburan normal standard"
			}
		]
	},
	{
		name: "PI",
		description: "Mengembalikan nilai Pi, 3.14159265358979, tepat sehingga 15 digit.",
		arguments: [
		]
	},
	{
		name: "PMT",
		description: "Mengira bayaran pinjaman berdasarkan bayaran malar dan kadar faedah malar.",
		arguments: [
			{
				name: "rate",
				description: "ialah kadar faedah bagi setiap tempoh pinjaman. Contohnya, gunakan 6%/4 bagi bayaran setiap suku tahun pada 6% APR"
			},
			{
				name: "nper",
				description: "ialah jumlah bilangan bayaran bagi pinjaman"
			},
			{
				name: "pv",
				description: "ialah nilai semasa: jumlah keseluruhan bagi nilai siri bayaran masa depan sekarang"
			},
			{
				name: "fv",
				description: "ialah nilai masa depan, atau baki tunai yang ingin anda peroleh setelah bayaran terakhir dibuat, 0 (sifar) jika diabaikan"
			},
			{
				name: "type",
				description: "ialah nilai logik: bayaran pada permulaan tempoh = 1; bayaran pada akhir tempoh = 0 atau diabaikan"
			}
		]
	},
	{
		name: "POISSON",
		description: "Mengembalikan taburan Poisson.",
		arguments: [
			{
				name: "x",
				description: "adalah bilangan peristiwa"
			},
			{
				name: "mean",
				description: "ialah nilai angka yang dijangkakan, nombor positif"
			},
			{
				name: "cumulative",
				description: "adalah nilai logik: bagi kebarangkalian Poisson kumulatif, gunakan TRUE; bagi fungsi jisim kebarangkalian Poisson, gunakan FALSE"
			}
		]
	},
	{
		name: "POISSON.DIST",
		description: "Mengembalikan taburan Poisson.",
		arguments: [
			{
				name: "x",
				description: "ialah bilangan peristiwa"
			},
			{
				name: "mean",
				description: "ialah nilai angka yang dijangkakan, nombor positif"
			},
			{
				name: "cumulative",
				description: "ialah nilai logik: bagi kebarangkalian Poisson kumulatif, gunakan BENAR; bagi fungsi massa kebarangkalian Poisson, gunakan PALSU"
			}
		]
	},
	{
		name: "POWER",
		description: "Mengembalikan hasil satu nombor dinaikkan kepada kuasa.",
		arguments: [
			{
				name: "number",
				description: "ialah nombor asas, sebarang nombor nyata"
			},
			{
				name: "power",
				description: "ialah eksponen, apabila nombor asas dinaikkan"
			}
		]
	},
	{
		name: "PPMT",
		description: "Mengembalikan bayaran pada prinsipal bagi pelaburan yang diberikan berdasarkan tempoh berkala, bayaran malar dan kadar faedah malar.",
		arguments: [
			{
				name: "rate",
				description: "ialah kadar faedah bagi setiap tempoh. Contohnya, gunakan 6%/4 untuk bayaran suku tahun pada 6% APR"
			},
			{
				name: "per",
				description: "Menentukan tempoh dan mestilah dalam julat 1 hingga nper"
			},
			{
				name: "nper",
				description: "ialah jumlah bilangan tempoh bayaran dalam pelaburan"
			},
			{
				name: "pv",
				description: "ialah nilai semasa: jumlah keseluruhan bagi nilai siri bayaran masa depan sekarang"
			},
			{
				name: "fv",
				description: "ialah nilai masa depan, atau baki tunai yang ingin anda peroleh setelah bayaran terakhir dibuat"
			},
			{
				name: "type",
				description: "ialah nilai logik: bayaran pada permulaan tempoh = 1; bayaran pada akhir tempoh = 0 atau diabaikan"
			}
		]
	},
	{
		name: "PRICEDISC",
		description: "Mengembalikan harga per $100 nilai muka bagi keselamatan terdiskaun.",
		arguments: [
			{
				name: "settlement",
				description: "ialah tarikh penyelesaian keselamatan, disebut sebagai nombor tarikh bersiri"
			},
			{
				name: "maturity",
				description: "ialah tarikh kematangan keselamatan, disebut sebagai nombor tarikh bersiri"
			},
			{
				name: "discount",
				description: "ialah kadar diskaun keselamatan"
			},
			{
				name: "redemption",
				description: "ialah nilai penebusan keselamatan per $100 nilai muka"
			},
			{
				name: "basis",
				description: "ialah jenis berdasarkan bilangan hari untuk digunakan"
			}
		]
	},
	{
		name: "PROB",
		description: "Mengembalikan kebarangkalian yang nilai dalam julatnya adalah antara dua had atau bersamaan dengan had lebih rendah.",
		arguments: [
			{
				name: "x_range",
				description: "ialah julat nilai angka x apabila terdapat kebarangkalian yang berkaitan"
			},
			{
				name: "prob_range",
				description: "ialah set kebarangkalian yang dikaitkan dengan nilai dalam julat_X, nilai antara 0 hingga 1 dan tidak termasuk 0"
			},
			{
				name: "lower_limit",
				description: "ialah batas bawah bagi nilai yang anda inginkan kebarangkaliannya"
			},
			{
				name: "upper_limit",
				description: "ialah batas atas pilihan pada nilai. Jika diabaikan, PROB mengembalikan kebarangkalian bahawa nilai julat_X sama dengan had_Bawah"
			}
		]
	},
	{
		name: "PRODUCT",
		description: "Menggandakan semua nombor yang diberikan sebagai argumen.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "ialah 1 hingga 255 nombor, nilai logik, atau perwakilan teks bagi nombor yang ingin anda gandakan"
			},
			{
				name: "number2",
				description: "ialah 1 hingga 255 nombor, nilai logik, atau perwakilan teks bagi nombor yang ingin anda gandakan"
			}
		]
	},
	{
		name: "PROPER",
		description: "Menukarkan rentetan teks kepada huruf sesuai; huruf pertama setiap kata dalam huruf besar dan semua huruf lain dalam huruf kecil.",
		arguments: [
			{
				name: "text",
				description: "ialah teks dalam tanda petikan, formula yang mengembalikan teks, atau rujukan kepada sel yang mengandungi teks untuk dihurufbesarkan sebahagiannya"
			}
		]
	},
	{
		name: "PV",
		description: "Mengembalikan nilai pelaburan sekarang: jumlah keseluruhan bagi nilai siri bayaran masa depan sekarang.",
		arguments: [
			{
				name: "rate",
				description: "ialah kadar faedah bagi setiap tempoh. Contohnya, gunakan 6%/4 bagi bayaran setiap suku tahun pada 6% APR"
			},
			{
				name: "nper",
				description: "ialah jumlah bilangan tempoh bayaran dalam pelaburan"
			},
			{
				name: "pmt",
				description: "ialah bayaran yang dibuat bagi setiap tempoh dan tidak boleh diubah sepanjang tempoh pelaburan"
			},
			{
				name: "fv",
				description: "ialah nilai masa depan, atau baki tunai yang ingin anda peroleh setelah bayaran terakhir dibuat"
			},
			{
				name: "type",
				description: "ialah nilai logik: bayaran pada permulaan tempoh = 1; bayaran pada akhir tempoh = 0 atau diabaikan"
			}
		]
	},
	{
		name: "QUARTILE",
		description: "Mengembalikan sukuan set data.",
		arguments: [
			{
				name: "array",
				description: "ialah tatasusunan atau julat sel nilai angka yang anda inginkan nilai sukuan"
			},
			{
				name: "quart",
				description: "ialah nombor: nilai minimum = 0; sukuan pertama = 1; nilai median = 2; sukuan ketiga = 3; nilai maksimum = 4"
			}
		]
	},
	{
		name: "QUARTILE.EXC",
		description: "Mengembalikan sukuan set data berdasarkan nilai persentil daripada 0.. 1, eksklusif.",
		arguments: [
			{
				name: "array",
				description: "ialah tatasusunan atau julat sel nilai angka yang anda inginkan nilai sukuan"
			},
			{
				name: "quart",
				description: "ialah nombor: nilai minimum = 0; sukuan pertama = 1; nilai median = 2; sukuan ketiga = 3; nilai maksimum = 4"
			}
		]
	},
	{
		name: "QUARTILE.INC",
		description: "Mengembalikan sukuan set data berdasarkan nilai persentil daripada 0.. 1, inklusif.",
		arguments: [
			{
				name: "array",
				description: "ialah tatasusunan atau julat sel nilai angka yang anda inginkan nilai sukuan"
			},
			{
				name: "quart",
				description: "ialah nombor: nilai minimum = 0; sukuan pertama = 1; nilai median = 2; sukuan ketiga = 3; nilai maksimum = 4"
			}
		]
	},
	{
		name: "QUOTIENT",
		description: "Mengembalikan bahagian integer bagi pembahagian.",
		arguments: [
			{
				name: "numerator",
				description: "ialah angka yang dibahagi"
			},
			{
				name: "denominator",
				description: "ialah pembahagi"
			}
		]
	},
	{
		name: "RADIANS",
		description: "Menukarkan darjah kepada radian.",
		arguments: [
			{
				name: "angle",
				description: "ialah sudut dalam darjah yang ingin anda tukar"
			}
		]
	},
	{
		name: "RAND",
		description: "Mengembalikan nombor rawak yang lebih besar daripada atau sama dengan 0 dan kurang daripada 1, diagih secara sekata (perubahan dalam pengiraan semula).",
		arguments: [
		]
	},
	{
		name: "RANDBETWEEN",
		description: "Mengembalikan nombor rawak di antara nombor yang anda tentukan.",
		arguments: [
			{
				name: "bottom",
				description: "ialah integer terkecil RANDBETWEEN akan dikembalikan"
			},
			{
				name: "top",
				description: "ialah integer terbesar RANDBETWEEN akan dikembalikan"
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
		description: "Mengembalikan kedudukan nombor dalam senarai nombor: saiznya relatif kepada nilai lain dalam senarai.",
		arguments: [
			{
				name: "number",
				description: "ialah nombor yang ingin anda cari kedudukannya"
			},
			{
				name: "ref",
				description: "ialah tatasusunan, atau rujukan kepada, senarai nombor. Nilai bukan angka diabaikan"
			},
			{
				name: "order",
				description: "ialah nombor: kedudukan dalam senarai diisih menurun = 0 atau diabaikan; kedudukan dalam senarai diisih menaik = sebarang nilai bukan sifar"
			}
		]
	},
	{
		name: "RANK.AVG",
		description: "Mengembalikan kedudukan nombor dalam senarai nombor: saiznya relatif kepada nilai lain dalam senarai; jika lebih daripada satu nilai mempunyai kedudukan yang sama, kedudukan purata dikembalikan.",
		arguments: [
			{
				name: "number",
				description: "ialah nombor yang ingin anda cari kedudukannya"
			},
			{
				name: "ref",
				description: "ialah tatasusunan, atau rujukan, senarai nombor. Nilai bukan angka diabaikan"
			},
			{
				name: "order",
				description: "ialah nombor: kedudukan dalam senarai diisih menurun = 0 atau diabaikan; kedudukan dalam senarai diisih menaik = sebarang nilai bukan sifar"
			}
		]
	},
	{
		name: "RANK.EQ",
		description: "Mengembalikan kedudukan nombor dalam senarai nombor: saiznya relatif kepada nilai lain dalam senarai; jika lebih daripada satu nilai mempunyai kedudukan yang sama, kedudukan teratas bagi set nilai tersebut dikembalikan.",
		arguments: [
			{
				name: "number",
				description: "ialah nombor yang ingin anda cari kedudukannya"
			},
			{
				name: "ref",
				description: "ialah tatasusunan, atau rujukan, senarai nombor. Nilai bukan angka diabaikan"
			},
			{
				name: "order",
				description: "ialah nombor: kedudukan dalam senarai diisih menurun = 0 atau diabaikan; kedudukan dalam senarai diisih menaik = sebarang nilai bukan sifar"
			}
		]
	},
	{
		name: "RATE",
		description: "Mengembalikan kadar faedah bagi setiap tempoh pinjaman atau pelaburan. Contohnya, gunakan 6%/4 bagi bayaran setiap suku tahun pada 6% APR.",
		arguments: [
			{
				name: "nper",
				description: "ialah jumlah bilangan tempoh bayaran bagi pinjaman atau pelaburan"
			},
			{
				name: "pmt",
				description: "ialah bayaran yang dibuat setiap tempoh dan tidak boleh diubah sepanjang tempoh pinjaman atau pelaburan"
			},
			{
				name: "pv",
				description: "ialah nilai semasa: jumlah keseluruhan bagi nilai siri bayaran masa depan sekarang"
			},
			{
				name: "fv",
				description: "ialah nilai masa depan, atau baki tunai yang ingin anda peroleh setelah bayaran terakhir dibuat. Jika diabaikan, gunakan Fv = 0"
			},
			{
				name: "type",
				description: "ialah nilai logik: bayaran pada permulaan tempoh =1; bayaran pada akhir tempoh = 0 atau diabaikan"
			},
			{
				name: "guess",
				description: "ialah tekaan anda bagi kadarnya; jika diabaikan, Tekaan = 0.1 (10 peratus)"
			}
		]
	},
	{
		name: "RECEIVED",
		description: "Mengembalikan jumlah yang diterima setelah matang bagi keselamatan pelaburan penuh.",
		arguments: [
			{
				name: "settlement",
				description: "ialah tarikh penyelesaian keselamatan, disebut sebagai nombor tarikh bersiri"
			},
			{
				name: "maturity",
				description: "ialah tarikh kematangan keselamatan, disebut sebagai nombor tarikh bersiri"
			},
			{
				name: "investment",
				description: "ialah jumlah yang dilaburkan dalam keselamatan"
			},
			{
				name: "discount",
				description: "ialah kadar diskaun keselamatan"
			},
			{
				name: "basis",
				description: "ialah jenis berdasarkan bilangan hari untuk digunakan"
			}
		]
	},
	{
		name: "REPLACE",
		description: "Menggantikan sebahagian rentetan teks dengan rentetan teks yang lain.",
		arguments: [
			{
				name: "old_text",
				description: "ialah teks yang ingin anda gantikan dengan beberapa aksara"
			},
			{
				name: "start_num",
				description: "ialah posisi aksara dalam teks_Lama yang ingin anda gantikan dengan teks_Baru"
			},
			{
				name: "num_chars",
				description: "ialah bilangan aksara dalam teks_Lama yang ingin anda ganti"
			},
			{
				name: "new_text",
				description: "ialah teks yang akan menggantikan aksara dalam teks_Lama"
			}
		]
	},
	{
		name: "REPT",
		description: "Mengulangi teks ikut bilangan kali yang diberikan. Gunakan ULANG untuk mengisi sel dengan bilangan tika rentetan teks.",
		arguments: [
			{
				name: "text",
				description: "ialah teks yang ingin anda ulangi"
			},
			{
				name: "number_times",
				description: "ialah nombor positif yang menentukan berapa kali untuk mengulangi teks"
			}
		]
	},
	{
		name: "RIGHT",
		description: "Mengembalikan bilangan aksara yang ditentukan dari hujung rentetan teks.",
		arguments: [
			{
				name: "text",
				description: "ialah rentetan teks yang mengandungi aksara yang ingin anda ekstrak"
			},
			{
				name: "num_chars",
				description: "Menentukan berapa banyak aksara yang ingin anda ekstrak, jika diabaikan"
			}
		]
	},
	{
		name: "ROMAN",
		description: "Menukarkan angka Arab kepada Roman, sebagai teks.",
		arguments: [
			{
				name: "number",
				description: "ialah angka Arab yang ingin anda tukar"
			},
			{
				name: "form",
				description: "ialah nombor yang menentukan jenis angka Roman yang anda kehendaki."
			}
		]
	},
	{
		name: "ROUND",
		description: "Membundarkan nombor kepada bilangan digit yang ditentukan.",
		arguments: [
			{
				name: "number",
				description: "ialah nombor yang ingin anda bundarkan"
			},
			{
				name: "num_digits",
				description: "ialah bilangan digit yang ingin anda bundarkan. Negatif dibundarkan ke sebelah kiri titik perpuluhan; sifar kepada integer terdekat"
			}
		]
	},
	{
		name: "ROUNDDOWN",
		description: "Membundarkan nombor ke bawah, ke arah sifar.",
		arguments: [
			{
				name: "number",
				description: "ialah mana-mana nombor nyata yang ingin anda bundarkan ke bawah"
			},
			{
				name: "num_digits",
				description: "ialah bilangan digit yang ingin anda bundarkan. Negatif dibundarkan ke sebelah kiri titik perpuluhan; sifar atau diabaikan, kepada integer yang terdekat"
			}
		]
	},
	{
		name: "ROUNDUP",
		description: "Membundarkan nombor ke atas, jauh daripada sifar.",
		arguments: [
			{
				name: "number",
				description: "ialah mana-mana nombor nyata yang ingin anda bundarkan ke atas"
			},
			{
				name: "num_digits",
				description: "ialah bilangan digit yang ingin anda bundarkan. Negatif dibundarkan ke sebelah kiri titik perpuluhan; sifar atau diabaikan, kepada integer yang terdekat"
			}
		]
	},
	{
		name: "ROW",
		description: "Mengembalikan nombor baris rujukan.",
		arguments: [
			{
				name: "reference",
				description: "ialah sel atau julat tunggal sel yang anda inginkan nombor barisnya, jika diabaikan, kembalikan sel yang mengandungi fungsi BARIS"
			}
		]
	},
	{
		name: "ROWS",
		description: "Mengembalikan bilangan baris dalam rujukan atau tatasusunan.",
		arguments: [
			{
				name: "array",
				description: "ialah tatasusunan, formula tatasusunan, atau rujukan kepada julat sel yang anda inginkan bilangan baris"
			}
		]
	},
	{
		name: "RRI",
		description: "Mengembalikan kadar faedah yang sama bagi pertumbuhan suatu pelaburan.",
		arguments: [
			{
				name: "nper",
				description: "ialah bilangan tempoh untuk pelaburan"
			},
			{
				name: "pv",
				description: "ialah nilai semasa bagi pelaburan"
			},
			{
				name: "fv",
				description: "ialah nilai masa depan bagi pelaburan"
			}
		]
	},
	{
		name: "RSQ",
		description: "Mengembalikan kuasa dua pekali korelasi momen hasil darab Pearson melalui titik data yang diberikan.",
		arguments: [
			{
				name: "known_y's",
				description: "ialah tatasusunan atau julat titik data dan mungkin nombor atau nama, tatasusunan atau rujukan yang mengandungi nombor"
			},
			{
				name: "known_x's",
				description: "ialah tatasusunan atau julat titik data dan mungkin nombor atau nama, tatasusunan, atau rujukan yang mengandungi nombor"
			}
		]
	},
	{
		name: "RTD",
		description: "Mendapatkan semula data masa nyata daripada program yang menyokong automasi COM.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "progID",
				description: "ialah nama ProgID bagi tambahan automasi COM yang berdaftar. Kurungkan nama dalam tanda petikan"
			},
			{
				name: "server",
				description: "ialah nama pelayan di mana tambahan harus dijalankan. Kurungkan nama dalam tanda petikan. Jika tambahan dijalankan secara setempat, gunakan rentetan kosong"
			},
			{
				name: "topic1",
				description: "ialah 1 hingga 38 parameter yang menentukan data"
			},
			{
				name: "topic2",
				description: "ialah 1 hingga 38 parameter yang menentukan data"
			}
		]
	},
	{
		name: "SEARCH",
		description: "Mengembalikan bilangan aksara yang aksara khusus atau rentetan teks mula-mula dijumpai, dibaca dari kiri ke kanan (bukan sensitif huruf).",
		arguments: [
			{
				name: "find_text",
				description: "ialah teks yang ingin anda cari. Anda boleh menggunakan ? dan * aksara bebas; menggunakan ~? dan ~* untuk mencari aksara ? dan *"
			},
			{
				name: "within_text",
				description: "ialah teks yang ingin anda cari Cari_teks"
			},
			{
				name: "start_num",
				description: "ialah aksara nombor dalam Antara_teks, membilang dari kiri, di mana anda ingin memulakan carian. Jika diabaikan, 1 digunakan"
			}
		]
	},
	{
		name: "SEC",
		description: "Mengembalikan sekan bagi sudut.",
		arguments: [
			{
				name: "number",
				description: "ialah sudut dalam radian yang anda inginkan bagi sekan"
			}
		]
	},
	{
		name: "SECH",
		description: "Mengembalikan sekan hiperbola bagi sudut.",
		arguments: [
			{
				name: "number",
				description: "ialah sudut dalam radian yang anda inginkan bagi sekan hiperbola"
			}
		]
	},
	{
		name: "SECOND",
		description: "Mengembalikan saat, nombor bermula 0 hingga 59.",
		arguments: [
			{
				name: "serial_number",
				description: "ialah nombor dalam kod tarikh masa yang digunakan oleh Spreadsheet atau teks dalam format masa, seperti 16:48:23 atau 4:48:47 PM"
			}
		]
	},
	{
		name: "SERIESSUM",
		description: "Mengembalikan jumlah bagi siri kuasa berdasarkan formula.",
		arguments: [
			{
				name: "x",
				description: "ialah nilai input kepada siri kuasa"
			},
			{
				name: "n",
				description: "ialah kuasa permulaan pada mana ingin anda naikkan x"
			},
			{
				name: "m",
				description: "ialah langkah dengan mana untuk menaikkan n untuk setiap terma dalam siri"
			},
			{
				name: "coefficients",
				description: "ialah set pekali dengan mana setiap kuasa berentetan bagi x digandakan"
			}
		]
	},
	{
		name: "SHEET",
		description: "Mengembalikan nombor helaian bagi helaian yang dirujuk.",
		arguments: [
			{
				name: "value",
				description: "ialah nama helaian atau rujukan bagi nombor helaian yang anda inginkan.  Jika diabaikan, nombor helaian yang mengandungi fungsi dikembalikan"
			}
		]
	},
	{
		name: "SHEETS",
		description: "Mengembalikan bilangan helaian dalam rujukan.",
		arguments: [
			{
				name: "reference",
				description: "ialah rujukan yang anda ingin ketahui bilangan helaian yang terkandung di dalamnya.  Jika diabaikan, bilangan helaian dalam buku kerja yang mengandungi fungsi dikembalikan"
			}
		]
	},
	{
		name: "SIGN",
		description: "Mengembalikan tanda nombor: 1 jika nombor positif, sifar jika nombor sifar, atau -1 jika nombor negatif.",
		arguments: [
			{
				name: "number",
				description: "ialah sebarang nombor nyata"
			}
		]
	},
	{
		name: "SIN",
		description: "Mengembalikan sinus bagi sudut.",
		arguments: [
			{
				name: "number",
				description: "ialah sudut dalam radian yang anda inginkan nilai sinusnya. Darjah * PI()/180 = radian"
			}
		]
	},
	{
		name: "SINH",
		description: "Mengembalikan sinus hiperbola bagi nombor.",
		arguments: [
			{
				name: "number",
				description: "ialah sebarang nombor nyata"
			}
		]
	},
	{
		name: "SKEW",
		description: "Mengembalikan pencongan taburan; pencirian darjah asimetri taburan di sekeliling minnya.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "ialah 1 hingga 255 nombor atau nama, tatasusunan, atau rujukan yang mengandungi nombor yang anda inginkan pencongannya"
			},
			{
				name: "number2",
				description: "ialah 1 hingga 255 nombor atau nama, tatasusunan, atau rujukan yang mengandungi nombor yang anda inginkan pencongannya"
			}
		]
	},
	{
		name: "SKEW.P",
		description: "Mengembalikan pencongan taburan berdasarkan populasi: pencirian darjah asimetri taburan di sekeliling minnya.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "ialah 1 hingga 254 nombor atau nama, tatasusunan, atau rujukan yang mengandungi nombor yang anda inginkan pencongan populasinya"
			},
			{
				name: "number2",
				description: "ialah 1 hingga 254 nombor atau nama, tatasusunan, atau rujukan yang mengandungi nombor yang anda inginkan pencongan populasinya"
			}
		]
	},
	{
		name: "SLN",
		description: "Mengembalikan susut nilai garis lurus bagi aset untuk satu tempoh.",
		arguments: [
			{
				name: "cost",
				description: "ialah kos permulaan aset"
			},
			{
				name: "salvage",
				description: "ialah nilai sisaan di hujung jangka hayat aset tersebut"
			},
			{
				name: "life",
				description: "ialah bilangan tempoh tamat apabila aset disusutnilaikan (kadangkala disebut sebagai usia berguna aset)"
			}
		]
	},
	{
		name: "SLOPE",
		description: "Mengembalikan kecerunan garisan regresi linear melalui titik data yang diberikan.",
		arguments: [
			{
				name: "known_y's",
				description: "ialah tatasusunan atau julat sel titik data angka bersandar dan mungkin nombor atau nama, tatasusunan, atau rujukan yang mengandungi nombor"
			},
			{
				name: "known_x's",
				description: "ialah set titik data bebas dan mungkin nombor atau nama, tatasusunan, atau rujukan yang mengandungi nombor"
			}
		]
	},
	{
		name: "SMALL",
		description: "Mengembalikan nilai terkecil ke-k dalam set data. Contohnya, nombor terkecil kelima.",
		arguments: [
			{
				name: "array",
				description: "ialah tatasusunan atau julat data angka yang ingin anda tentukan nilai terkecil ke-k"
			},
			{
				name: "k",
				description: "ialah posisi (daripada yang terkecil) dalam tatasusunan atau julat nilai untuk dikembalikan"
			}
		]
	},
	{
		name: "SQRT",
		description: "Mengembalikan punca kuasa dua bagi nombor.",
		arguments: [
			{
				name: "number",
				description: "ialah nombor yang anda inginkan nilai punca kuasa duanya"
			}
		]
	},
	{
		name: "SQRTPI",
		description: "Mengembalikan punca kuasa dua bagi (nombor * Pi).",
		arguments: [
			{
				name: "number",
				description: "ialah nombor dengan mana p digandakan"
			}
		]
	},
	{
		name: "STANDARDIZE",
		description: "Mengembalikan nilai yang telah dinormalkan daripada taburan yang dicirikan oleh min dan sisihan piawai.",
		arguments: [
			{
				name: "x",
				description: "ialah nilai yang ingin anda normalkan"
			},
			{
				name: "mean",
				description: "ialah min aritmetik taburan"
			},
			{
				name: "standard_dev",
				description: "ialah sisihan piawai taburan, nombor positif"
			}
		]
	},
	{
		name: "STDEV",
		description: "Menganggarkan sisihan piawai berdasarkan satu sampel (mengabaikan nilai dan teks logik dalam sampel).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "ialah 1 hingga 255 nombor yang sepadan dengan sampel populasi dan juga nombor atau rujukan yang mengandungi nombor"
			},
			{
				name: "number2",
				description: "ialah 1 hingga 255 nombor yang sepadan dengan sampel populasi dan juga nombor atau rujukan yang mengandungi nombor"
			}
		]
	},
	{
		name: "STDEV.P",
		description: "Mengira sisihan piawai berdasarkan keseluruhan populasi yang diberikan sebagai argumen (abaikan nilai logik dan teks).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "ialah 1 hingga 255 nombor yang sepadan dengan populasi dan mungkin nombor atau rujukan yang mengandungi nombor"
			},
			{
				name: "number2",
				description: "ialah 1 hingga 255 nombor yang sepadan dengan populasi dan mungkin nombor atau rujukan yang mengandungi nombor"
			}
		]
	},
	{
		name: "STDEV.S",
		description: "Menganggarkan sisihan piawai berdasarkan satu sampel (abaikan nilai dan teks logik dalam sampel).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "ialah 1 hingga 255 nombor yang sepadan dengan sampel populasi dan juga nombor atau rujukan yang mengandungi nombor"
			},
			{
				name: "number2",
				description: "ialah 1 hingga 255 nombor yang sepadan dengan sampel populasi dan juga nombor atau rujukan yang mengandungi nombor"
			}
		]
	},
	{
		name: "STDEVA",
		description: "Menganggarkan sisihan piawai berdasarkan sampel, termasuk nilai logik dan teks. Teks dan nilai logik PALSU mempunyai nilai 0; nilai logik BENAR mempunyai nilai 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "ialah 1 hingga 255 nilai yang sepadan dengan sampel populasi dan mungkin nilai atau nama atau rujukan kepada nilai"
			},
			{
				name: "value2",
				description: "ialah 1 hingga 255 nilai yang sepadan dengan sampel populasi dan mungkin nilai atau nama atau rujukan kepada nilai"
			}
		]
	},
	{
		name: "STDEVP",
		description: "Mengira sisihan piawai berdasarkan keseluruhan populasi yang diberikan sebagai argumen (mengabaikan nilai logik dan teks).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "ialah 1 hingga 255 nombor yang sepadan dengan populasi dan mungkin nombor atau rujukan yang mengandungi nombor"
			},
			{
				name: "number2",
				description: "ialah 1 hingga 255 nombor yang sepadan dengan populasi dan mungkin nombor atau rujukan yang mengandungi nombor"
			}
		]
	},
	{
		name: "STDEVPA",
		description: "Mengira sisihan piawai berdasarkan keseluruhan populasi, termasuk nilai logik dan teks. Teks dan nilai logik PALSU mempunyai nilai 0; nilai logik BENAR mempunyai nilai 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "ialah 1 hingga 255 nilai yang sepadan dengan populasi dan mungkin nilai, nama, tatasusunan, atau rujukan yang mengandungi nilai"
			},
			{
				name: "value2",
				description: "ialah 1 hingga 255 nilai yang sepadan dengan populasi dan mungkin nilai, nama, tatasusunan, atau rujukan yang mengandungi nilai"
			}
		]
	},
	{
		name: "STEYX",
		description: "Mengembalikan ralat standard bagi nilai y yang dijangkakan untuk setiap x dalam regresi.",
		arguments: [
			{
				name: "known_y's",
				description: "ialah tatasusunan atau julat titik data bersandar dan mungkin nombor atau nama, tatasusunan, atau rujukan yang mengandungi nombor"
			},
			{
				name: "known_x's",
				description: "ialah tatasusunan atau julat titik data bebas dan mungkin nombor atau nama, tatasusunan atau rujukan yang mengandungi nombor"
			}
		]
	},
	{
		name: "SUBSTITUTE",
		description: "Menggantikan teks yang telah wujud dengan teks baru dalam rentetan teks.",
		arguments: [
			{
				name: "text",
				description: "ialah teks atau rujukan kepada sel yang mengandungi teks yang ingin anda gantikan aksaranya"
			},
			{
				name: "old_text",
				description: "ialah teks yang telah wujud yang ingin anda gantikan. Jika huruf teks_Lama tidak sepadan dengan huruf teks, PENGGANTI tidak akan menggantikan teks"
			},
			{
				name: "new_text",
				description: "ialah teks yang ingin anda gantikan dengan teks_Lama"
			},
			{
				name: "instance_num",
				description: "menentukan kejadian teks_Lama yang ingin anda gantikan. Jika diabaikan, setiap tika teks_Lama digantikan"
			}
		]
	},
	{
		name: "SUBTOTAL",
		description: "Mengembalikan subjumlah dalam senarai atau pangkalan data.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "function_num",
				description: "ialah nombor 1 hingga 11 yang menentukan fungsi ringkasan untuk subjumlah."
			},
			{
				name: "ref1",
				description: "ialah 1 hingga 254 julat atau rujukan yang ingin anda dapatkan subjumlahnya"
			}
		]
	},
	{
		name: "SUM",
		description: "Tambah semua nombor dalam julat sel.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "ialah 1 hingga 255 nombor untuk dijumlahkan. Nilai logik dan teks diabaikan dalam sel, dimasukkan jika ditaip sebagai argumen"
			},
			{
				name: "number2",
				description: "ialah 1 hingga 255 nombor untuk dijumlahkan. Nilai logik dan teks diabaikan dalam sel, dimasukkan jika ditaip sebagai argumen"
			}
		]
	},
	{
		name: "SUMIF",
		description: "Menambah sel yang ditentukan ikut syarat dan kriteria yang diberikan.",
		arguments: [
			{
				name: "range",
				description: "ialah julat sel yang ingin anda nilaikan"
			},
			{
				name: "criteria",
				description: "ialah syarat atau kriteria dalam bentuk nombor, ungkapan, atau teks yang mentakrif sel mana akan ditambah"
			},
			{
				name: "sum_range",
				description: "ialah sel sebenar untuk dijumlah. Jika diabaikan, sel dalam julat digunakan"
			}
		]
	},
	{
		name: "SUMIFS",
		description: "Tambah sel yang ditentukan oleh set syarat atau kriteria yang diberikan.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "sum_range",
				description: "adalah sel sebenar untuk dijumlah."
			},
			{
				name: "criteria_range",
				description: "ialah julat bagi sel yang anda ingin ia dinilai bagi syarat tertentu"
			},
			{
				name: "criteria",
				description: "ialah syarat atau kriteria dalam bentuk nombor, ungkapan, atau teks yang mentakrifkan sel mana akan ditambah"
			}
		]
	},
	{
		name: "SUMPRODUCT",
		description: "Mengembalikan hasil tambah produk julat atau tatasusunan yang sepadan.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "array1",
				description: "ialah 2 hingga 255 tatasusunan yang ingin anda gandakan dan kemudian menambah komponen. Semua tatasusunan mestilah mempunyai dimensi yang sama"
			},
			{
				name: "array2",
				description: "ialah 2 hingga 255 tatasusunan yang ingin anda gandakan dan kemudian menambah komponen. Semua tatasusunan mestilah mempunyai dimensi yang sama"
			},
			{
				name: "array3",
				description: "ialah 2 hingga 255 tatasusunan yang ingin anda gandakan dan kemudian menambah komponen. Semua tatasusunan mestilah mempunyai dimensi yang sama"
			}
		]
	},
	{
		name: "SUMSQ",
		description: "Mengembalikan hasil tambah kuasa dua argumen. Argumen mungkin nombor, tatasusunan, nama, atau rujukan kepada sel yang mengandungi nombor.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "ialah 1 hingga 255 nombor, tatasusunan, nama, atau rujukan kepada tatasusunan yang anda inginkan hasil tambah bagi kuasa duanya"
			},
			{
				name: "number2",
				description: "ialah 1 hingga 255 nombor, tatasusunan, nama, atau rujukan kepada tatasusunan yang anda inginkan hasil tambah bagi kuasa duanya"
			}
		]
	},
	{
		name: "SUMX2MY2",
		description: "Menjumlahkan perbezaan antara kuasa dua dua julat atau tatasusunan yang sepadan.",
		arguments: [
			{
				name: "array_x",
				description: "ialah julat atau tatasusunan pertama nombor dan mungkin nombor atau nama, tatasusunan, atau rujukan yang mengandungi nombor"
			},
			{
				name: "array_y",
				description: "ialah julat atau tatasusunan kedua nombor dan mungkin nombor atau nama, tatasusunan, atau rujukan yang mengandungi nombor"
			}
		]
	},
	{
		name: "SUMX2PY2",
		description: "Mengembalikan jumlah hasil tambah bagi hasil tambah kuasa dua nombor dalam dua julat atau tatasusunan yang sepadan.",
		arguments: [
			{
				name: "array_x",
				description: "ialah julat atau tatasusunan pertama nombor dan mungkin nombor atau nama, tatasusunan, atau rujukan yang mengandungi nombor"
			},
			{
				name: "array_y",
				description: "ialah julat atau tatasusunan kedua nombor dan mungkin nombor atau nama, tatasusunan, atau rujukan yang mengandungi nombor"
			}
		]
	},
	{
		name: "SUMXMY2",
		description: "Menjumlahkan kuasa dua perbezaan dalam dua julat atau tatasusunan yang sepadan.",
		arguments: [
			{
				name: "array_x",
				description: "ialah julat atau tatasusunan pertama nilai dan mungkin nombor atau nama, tatasusunan, atau rujukan yang mengandungi nombor"
			},
			{
				name: "array_y",
				description: "ialah julat atau tatasusunan kedua nilai dan mungkin nombor atau nama, tatasusunan, atau rujukan yang mengandungi nombor"
			}
		]
	},
	{
		name: "SYD",
		description: "Mengembalikan digit susut nilai hasil tambah tahun bagi aset untuk tempoh ditentukan.",
		arguments: [
			{
				name: "cost",
				description: "ialah kos permulaan aset"
			},
			{
				name: "salvage",
				description: "ialah nilai sisaan di hujung jangka hayat aset tersebut"
			},
			{
				name: "life",
				description: "ialah bilangan tempoh tamat apabila aset disusutnilaikan (kadangkala disebut sebagai usia berguna aset)"
			},
			{
				name: "per",
				description: "ialah tempoh dan mestilah menggunakan unit yang sama dengan Hayat"
			}
		]
	},
	{
		name: "T",
		description: "Menyemak sama ada nilai ialah teks, dan mengembalikan teks jika ia adalah teks, atau mengembalikan tanda petik berganda (teks kosong) jika ia bukan.",
		arguments: [
			{
				name: "value",
				description: "ialah nilai untuk diuji"
			}
		]
	},
	{
		name: "T.DIST",
		description: "Mengembalikan taburan t berekor kiri pelajar.",
		arguments: [
			{
				name: "x",
				description: "ialah nilai angka untuk menilai taburan"
			},
			{
				name: "deg_freedom",
				description: "ialah integer yang menandakan bilangan darjah kebebasan yang mencirikan taburan"
			},
			{
				name: "cumulative",
				description: "ialah nilai logik: bagi fungsi taburan kumulatif, gunakan BENAR; bagi fungsi ketumpatan kebarangkalian, gunakan PALSU"
			}
		]
	},
	{
		name: "T.DIST.2T",
		description: "Mengembalikan taburan t dua ekor Pelajar.",
		arguments: [
			{
				name: "x",
				description: "ialah nilai angka untuk menilai taburan"
			},
			{
				name: "deg_freedom",
				description: "ialah integer yang menandakan bilangan darjah kebebasan yang mencirikan taburan"
			}
		]
	},
	{
		name: "T.DIST.RT",
		description: "Mengembalikan taburan t berekor kanan Pelajar.",
		arguments: [
			{
				name: "x",
				description: "ialah nilai angka untuk menilai taburan"
			},
			{
				name: "deg_freedom",
				description: "ialah integer yang menandakan bilangan darjah kebebasan yang mencirikan taburan"
			}
		]
	},
	{
		name: "T.INV",
		description: "Mengembalikan songsangan ekor kiri taburan t Pelajar.",
		arguments: [
			{
				name: "probability",
				description: "ialah kebarangkalian yang dikaitkan dengan taburan t Pelajar dua ekor, termasuk nombor 0 hingga 1"
			},
			{
				name: "deg_freedom",
				description: "ialah integer positif yang menyatakan bilangan darjah kebebasan untuk mencirikan taburan"
			}
		]
	},
	{
		name: "T.INV.2T",
		description: "Mengembalikan songsangan dua ekor taburan t Pelajar.",
		arguments: [
			{
				name: "probability",
				description: "ialah kebarangkalian yang dikaitkan dengan taburan t Pelajar dua ekor, termasuk nombor 0 hingga 1"
			},
			{
				name: "deg_freedom",
				description: "ialah integer positif yang menyatakan bilangan darjah kebebasan untuk mencirikan taburan"
			}
		]
	},
	{
		name: "T.TEST",
		description: "Mengembalikan kebarangkalian yang dikaitkan dengan Ujian t Pelajar.",
		arguments: [
			{
				name: "array1",
				description: "ialah set data pertama"
			},
			{
				name: "array2",
				description: "ialah set data kedua"
			},
			{
				name: "tails",
				description: "menentukan nombor taburan ekor untuk dikembalikan: taburan satu ekor = 1; taburan dua ekor = 2"
			},
			{
				name: "type",
				description: "ialah jenis ujian t: berpasangan = 1, dua sampel dengan varians yang sama (homoskedastik) = 2, dua sampel dengan varians yang tidak sama = 3"
			}
		]
	},
	{
		name: "TAN",
		description: "Mengembalikan tangen bagi sudut.",
		arguments: [
			{
				name: "number",
				description: "ialah sudut dalam radian yang anda inginkan nilai tangennya. Darjah * PI()/180 = radian"
			}
		]
	},
	{
		name: "TANH",
		description: "Mengembalikan hiperbola tangen bagi nombor.",
		arguments: [
			{
				name: "number",
				description: "ialah sebarang nombor nyata"
			}
		]
	},
	{
		name: "TBILLEQ",
		description: "Mengembalikan hasil setara-bon untuk bil perbendaharaan.",
		arguments: [
			{
				name: "settlement",
				description: "ialah tarikh penyelesaian bil Perbendaharaan, disebut sebagai nombor tarikh bersiri"
			},
			{
				name: "maturity",
				description: "ialah tarikh kematangan bil Perbendaharaan, disebut sebagai nombor tarikh bersiri"
			},
			{
				name: "discount",
				description: "ialah kadar diskaun bil Perbendaharaan"
			}
		]
	},
	{
		name: "TBILLPRICE",
		description: "Mengembalikan harga per $100 nilai muka untuk bil perbendaharaan.",
		arguments: [
			{
				name: "settlement",
				description: "ialah tarikh penyelesaian bil Perbendaharaan, disebut sebagai nombor tarikh bersiri"
			},
			{
				name: "maturity",
				description: "ialah tarikh kematangan bil Perbendaharaan, disebut sebagai nombor tarikh bersiri"
			},
			{
				name: "discount",
				description: "ialah kadar diskaun bil Perbendaharaan"
			}
		]
	},
	{
		name: "TBILLYIELD",
		description: "Mengembalikan hasil untuk bil perbendaharaan.",
		arguments: [
			{
				name: "settlement",
				description: "ialah tarikh penyelesaian bil Perbendaharaan, disebut sebagai nombor tarikh bersiri"
			},
			{
				name: "maturity",
				description: "ialah tarikh kematangan bil Perbendaharaan, disebut sebagai nombor tarikh bersiri"
			},
			{
				name: "pr",
				description: "ialah harga bil Perbendaharaan per $100 nilai muka"
			}
		]
	},
	{
		name: "TDIST",
		description: "Mengembalikan taburan t Pelajar.",
		arguments: [
			{
				name: "x",
				description: "ialah nilai angka untuk menilai taburan"
			},
			{
				name: "deg_freedom",
				description: "ialah integer yang menandakan bilangan darjah kebebasan yang mencirikan taburan"
			},
			{
				name: "tails",
				description: "menentukan bilangan taburan ekor untuk dikembalikan: taburan satu ekor = 1; taburan dua ekor = 2"
			}
		]
	},
	{
		name: "TEXT",
		description: "Menukarkan nilai kepada teks dalam format nombor tertentu.",
		arguments: [
			{
				name: "value",
				description: "ialah nombor, formula yang dinilai kepada nilai angka, atau rujukan kepada sel yang mengandungi nilai angka"
			},
			{
				name: "format_text",
				description: "ialah format nombor dalam bentuk teks daripada kotak Kategori atas tab Nombor dalam kotak dialog Format Sel (bukan Am)"
			}
		]
	},
	{
		name: "TIME",
		description: "Menukarkan jam, minit, dan saat yang diberikan sebagai nombor kepada nombor siri Spreadsheet, diformatkan dengan format masa.",
		arguments: [
			{
				name: "hour",
				description: "ialah nombor bermula 0 hingga 23 yang mewakili jam"
			},
			{
				name: "minute",
				description: "ialah nombor bermula 0 hingga 59 yang mewakili minit"
			},
			{
				name: "second",
				description: "ialah nombor bermula 0 hingga 59 yang mewakili saat"
			}
		]
	},
	{
		name: "TIMEVALUE",
		description: "Menukarkan masa teks kepada nombor siri EXCEL bagi masa, nombor 0 (12:00:00 AM) hingga 0.999988426 (11:59:59 PM). Formatkan nombor dengan format masa setelah memasukkan formula.",
		arguments: [
			{
				name: "time_text",
				description: "ialah rentetan teks yang memberikan masa dalam mana-mana satu format masa Spreadsheet (maklumat tarikh dalam rentetan diabaikan)"
			}
		]
	},
	{
		name: "TINV",
		description: "Mengembalikan songsangan taburan t Pelajar.",
		arguments: [
			{
				name: "probability",
				description: "ialah kebarangkalian yang dikaitkan dengan taburan t Pelajar dua ekor, termasuk nombor 0 hingga 1"
			},
			{
				name: "deg_freedom",
				description: "ialah integer positif yang menyatakan bilangan darjah kebebasan untuk mencirikan pengagihan"
			}
		]
	},
	{
		name: "TODAY",
		description: "Mengembalikan tarikh semasa yang diformatkan sebagai tarikh.",
		arguments: [
		]
	},
	{
		name: "TRANSPOSE",
		description: "Menukar julat sel menegak ke julat mendatar, atau sebaliknya.",
		arguments: [
			{
				name: "array",
				description: "merupakan sel pada lembaran kerja atau tatasusunan nilai yang anda ingin transposisi"
			}
		]
	},
	{
		name: "TREND",
		description: "Mengembalikan nombor dalam titik data yang diketahui padanan arah aliran linear, menggunakan kaedah kuasa dua terkecil.",
		arguments: [
			{
				name: "known_y's",
				description: "ialah julat atau tatasusunan nilai y yang telah anda tahu dalam hubungan y = mx + b"
			},
			{
				name: "known_x's",
				description: "ialah julat pilihan atau tatasusunan nilai x yang anda tahu dalam hubungan y = mx + b, tatasusunan dengan saiz yang sama seperti y yang diketahui"
			},
			{
				name: "new_x's",
				description: "ialah julat atau tatasusunan nilai x baru yang anda inginkan ARAH ALIRAN mengembalikan nilai y yang sepadan"
			},
			{
				name: "const",
				description: "ialah nilai logik: pemalar b dikira biasanya jika Const = BENAR atau diabaikan; b diset sama dengan 0 jika Const = PALSU"
			}
		]
	},
	{
		name: "TRIM",
		description: "Alih keluar semua ruang daripada rentetan teks kecuali ruang tunggal antara setiap kata.",
		arguments: [
			{
				name: "text",
				description: "ialah teks yang ingin anda alih keluar ruangnya"
			}
		]
	},
	{
		name: "TRIMMEAN",
		description: "Mengembalikan min bahagian dalaman set nilai data.",
		arguments: [
			{
				name: "array",
				description: "ialah julat atau tatasusunan nilai untuk trim dan purata"
			},
			{
				name: "percent",
				description: "ialah nombor pecahan titik data untuk disisihkan daripada atas dan bawah set data"
			}
		]
	},
	{
		name: "TRUE",
		description: "Mengembalikan nilai logik BENAR.",
		arguments: [
		]
	},
	{
		name: "TRUNC",
		description: "Memenggal nombor kepada integer dengan mengalih keluar perpuluhan, atau pecahan, sebahagian daripada nombor.",
		arguments: [
			{
				name: "number",
				description: "ialah nombor yang ingin anda penggal"
			},
			{
				name: "num_digits",
				description: "ialah nombor yang ingin anda tentukan kepersisan pemenggalan, 0 (sifar) jika diabaikan"
			}
		]
	},
	{
		name: "TTEST",
		description: "Mengembalikan kebarangkalian yang dikaitkan dengan Ujian t Pelajar.",
		arguments: [
			{
				name: "array1",
				description: "ialah set data pertama"
			},
			{
				name: "array2",
				description: "ialah set data kedua"
			},
			{
				name: "tails",
				description: "menentukan nombor taburan ekor untuk dikembalikan: taburan satu ekor = 1; taburan dua ekor = 2"
			},
			{
				name: "type",
				description: "ialah jenis ujian t: berpasangan = 1, dua sampel dengan varians yang sama (homoskedastik) = 2, dua sampel dengan varians yang tidak sama = 3"
			}
		]
	},
	{
		name: "TYPE",
		description: "Mengembalikan integer mewakili jenis nilai data: nombor = 1; teks = 2; nilai logik = 4; nilai ralat = 16; tatasusunan = 64.",
		arguments: [
			{
				name: "value",
				description: "boleh jadi apa-apa nilai"
			}
		]
	},
	{
		name: "UNICODE",
		description: "Mengembalikan nombor (titik kod) yang sepadan dengan aksara pertama bagi teks.",
		arguments: [
			{
				name: "text",
				description: "ialah aksara yang anda inginkan bagi nilai Unikod"
			}
		]
	},
	{
		name: "UPPER",
		description: "Menukarkan rentetan teks kepada huruf besar semuanya.",
		arguments: [
			{
				name: "text",
				description: "ialah teks yang ingin anda tukarkan kepada huruf besar, rujukan atau rentetan teks"
			}
		]
	},
	{
		name: "VALUE",
		description: "Menukarkan rentetan teks yang mewakili nombor kepada nombor.",
		arguments: [
			{
				name: "text",
				description: "ialah teks yang dikurung dalam tanda petikan atau rujukan kepada sel yang mengandungi teks yang ingin anda tukar"
			}
		]
	},
	{
		name: "VAR",
		description: "Menganggarkan varian berdasarkan sampel (mengabaikan nilai logik dan teks dalam sampel).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "ialah 1 hingga 255 argumen angka yang berkaitan dengan sampel populasi"
			},
			{
				name: "number2",
				description: "ialah 1 hingga 255 argumen angka yang berkaitan dengan sampel populasi"
			}
		]
	},
	{
		name: "VAR.P",
		description: "Mengira varians berdasarkan keseluruhan populasi (abaikan nilai logik dan teks dalam populasi).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "ialah 1 hingga 255 argumen angka yang sepadan dengan populasi"
			},
			{
				name: "number2",
				description: "ialah 1 hingga 255 argumen angka yang sepadan dengan populasi"
			}
		]
	},
	{
		name: "VAR.S",
		description: "Menganggarkan varian berdasarkan sampel (abaikan nilai logik dan teks dalam sampel).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "ialah 1 hingga 255 argumen angka yang berkaitan dengan sampel populasi"
			},
			{
				name: "number2",
				description: "ialah 1 hingga 255 argumen angka yang berkaitan dengan sampel populasi"
			}
		]
	},
	{
		name: "VARA",
		description: "Menganggarkan varians berdasarkan sampel, termasuk nilai logik dan teks. Teks dan nilai logik PALSU mempunyai nilai 0; nilai logik BENAR mempunyai nilai 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "ialah 1 hingga 255 argumen nilai yang sepadan dengan sampel populasi"
			},
			{
				name: "value2",
				description: "ialah 1 hingga 255 argumen nilai yang sepadan dengan sampel populasi"
			}
		]
	},
	{
		name: "VARP",
		description: "Mengira varians berdasarkan keseluruhan populasi (abaikan nilai logik dan teks dalam populasi).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "ialah 1 hingga 255 argumen angka yang sepadan dengan populasi"
			},
			{
				name: "number2",
				description: "ialah 1 hingga 255 argumen angka yang sepadan dengan populasi"
			}
		]
	},
	{
		name: "VARPA",
		description: "Mengira varians berdasarkan keseluruhan populasi, termasuk nilai logik dan teks. Teks dan nilai logik PALSU mempunyai nilai 0; nilai logik BENAR mempunyai nilai 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "ialah 1 hingga 255 nilai argumen yang sepadan dengan populasi"
			},
			{
				name: "value2",
				description: "ialah 1 hingga 255 nilai argumen yang sepadan dengan populasi"
			}
		]
	},
	{
		name: "VDB",
		description: "Mengembalikan susut nilai aset bagi mana-mana tempoh yang anda tentukan, termasuk tempoh separa, gunakan kaedah imbangan penyusutan berganda atau kaedah lain yang anda tentukan.",
		arguments: [
			{
				name: "cost",
				description: "ialah kos permulaan aset"
			},
			{
				name: "salvage",
				description: "ialah nilai sisaan pada akhir hayat aset"
			},
			{
				name: "life",
				description: "ialah bilangan tempoh tamat apabila aset disusutnilaikan (kadangkala disebut sebagai usia berguna aset)"
			},
			{
				name: "start_period",
				description: "ialah tempoh permulaan apabila anda ingin mengira susut nilai, dalam unit yang sama dengan Hayat"
			},
			{
				name: "end_period",
				description: "ialah tempoh tamat apabila anda ingin mengira susut nilai, dalam unit yang sama dengan Hayat"
			},
			{
				name: "factor",
				description: "ialah kadar apabila baki menyusut, 2 (imbangan penyusutan berganda) jika diabaikan"
			},
			{
				name: "no_switch",
				description: "tukarkan kepada susut nilai garis lurus apabila susut nilai lebih besar daripada baki menyusut = PALSU atau diabaikan; jangan tukar = BENAR"
			}
		]
	},
	{
		name: "VLOOKUP",
		description: "Mencari nilai dalam lajur paling kiri pada jadual, kemudian mengembalikan nilai dalam baris yang sama daripada lajur yang anda tentukan. Ikut lalai, jadual mestilah diisih secara menaik.",
		arguments: [
			{
				name: "lookup_value",
				description: "ialah nilai yang dijumpai dalam lajur pertama jadual, dan mungkin nilai, rujukan, atau rentetan teks"
			},
			{
				name: "table_array",
				description: "ialah jadual teks, nombor, atau nilai logik, di mana data didapatkan semula. Tatasusunan_jadual mungkin rujukan kepada julat atau nama julat"
			},
			{
				name: "col_index_num",
				description: "ialah nombor lajur dalam tatasusunan_jadual dari mana nilai sepadan patut dikembalikan. Nilai lajur pertama dalam jadual ialah lajur 1"
			},
			{
				name: "range_lookup",
				description: "ialah nilai logik: untuk mencari padanan terhampir dalam lajur pertama (diisih secara menaik) = BENAR atau diabaikan; cari padanan tepat = PALSU"
			}
		]
	},
	{
		name: "WEEKDAY",
		description: "Mengembalikan nombor bermula 1 hingga 7 yang mengecam hari dalam minggu bagi tarikh.",
		arguments: [
			{
				name: "serial_number",
				description: "ialah nombor yang mewakili tarikh"
			},
			{
				name: "return_type",
				description: "ialah nombor: untuk Ahad=1 hingga Sabtu=7, gunakan 1; untuk Isnin=1 hingga Ahad=7, gunakan 2; untuk Isnin=0 hingga Ahad=6, gunakan 3"
			}
		]
	},
	{
		name: "WEEKNUM",
		description: "Mengembalikan bilangan minggu dalam tahun.",
		arguments: [
			{
				name: "serial_number",
				description: "ialah kod tarikh masa yang digunakan oleh Spreadsheet untuk pengiraan tarikh dan masa"
			},
			{
				name: "return_type",
				description: "ialah satu nombor (1 atau 2) yang menentukan jenis nilai yang dikembalikan"
			}
		]
	},
	{
		name: "WEIBULL",
		description: "Mengembalikan taburan Weibull.",
		arguments: [
			{
				name: "x",
				description: "ialah nilai untuk menilai fungsi, nombor bukan negatif"
			},
			{
				name: "alpha",
				description: "ialah parameter taburan, nombor positif"
			},
			{
				name: "beta",
				description: "ialah parameter taburan, nombor positif"
			},
			{
				name: "cumulative",
				description: "ialah nilai logik: bagi fungsi taburan kumulatif, gunakan TRUE; bagi fungsi massa kebarangkalian, gunakan FALSE"
			}
		]
	},
	{
		name: "WEIBULL.DIST",
		description: "Mengembalikan taburan Weibull.",
		arguments: [
			{
				name: "x",
				description: "ialah nilai untuk menilai fungsi, nombor bukan negatif"
			},
			{
				name: "alpha",
				description: "ialah parameter taburan, nombor positif"
			},
			{
				name: "beta",
				description: "ialah parameter taburan, nombor positif"
			},
			{
				name: "cumulative",
				description: "ialah nilai logik: bagi fungsi taburan kumulatif, gunakan BENAR; bagi fungsi massa kebarangkalian, gunakan PALSU"
			}
		]
	},
	{
		name: "WORKDAY",
		description: "Mengembalikan nombor siri bagi tarikh sebelum atau selepas bilangan hari kerja yang ditentukan.",
		arguments: [
			{
				name: "start_date",
				description: "ialah nombor tarikh bersiri yang mewakili tarikh mula"
			},
			{
				name: "days",
				description: "ialah bilangan hari bukan hujung minggu dan bukan cuti sebelum atau selepas tarikh_mula"
			},
			{
				name: "holidays",
				description: "ialah tatasusunan pilihan untuk satu nombor tarikh bersiri atau lebih untuk disisihkan daripada kalendar kerja, seperti hari cuti negeri dan persekutuan dan hari cuti terapung"
			}
		]
	},
	{
		name: "WORKDAY.INTL",
		description: "Mengembalikan nombor bersiri tarikh sebelum atau selepas bilangan hari kerja yang ditentukan dengan parameter hujung minggu tersuai.",
		arguments: [
			{
				name: "start_date",
				description: "ialah nombor tarikh bersiri yang mewakili tarikh mula"
			},
			{
				name: "days",
				description: "ialah bilangan hari bukan hujung minggu dan bukan cuti atau selepas tarikh_mula"
			},
			{
				name: "weekend",
				description: "ialah nombor atau rentetan yang menentukan masa hujung minggu berlaku"
			},
			{
				name: "holidays",
				description: "ialah tatasusunan pilihan bagi satu atau lebih nombor tarikh bersiri untuk dikecualikan daripada kalendar kerja, seperti cuti negeri dan persekutuan dan cuti apung"
			}
		]
	},
	{
		name: "XIRR",
		description: "Mengembalikan kadar dalaman kembali bagi jadual aliran tunai.",
		arguments: [
			{
				name: "values",
				description: "ialah siri aliran tunai yang berpadanan dengan jadual pembayaran mengikut tarikh"
			},
			{
				name: "dates",
				description: "ialah jadual bagi tarikh pembayaran yang berpadanan dengan pembayaran aliran tunai"
			},
			{
				name: "guess",
				description: "ialah nombor yang anda jangka hampir dengan keputusan XIRR"
			}
		]
	},
	{
		name: "XNPV",
		description: "Mengembalikan nilai kini bersih bagi jadual aliran tunai.",
		arguments: [
			{
				name: "rate",
				description: "ialah kadar diskaun untuk digunakan pada aliran tunai"
			},
			{
				name: "values",
				description: "ialah siri bagi aliran tunai yang berpadanan dengan jadual pembayaran dalam tarikh"
			},
			{
				name: "dates",
				description: "ialah jadual tarikh pembayaran yang berpadanan dengan pembayaran aliran tunai"
			}
		]
	},
	{
		name: "XOR",
		description: "Mengembalikan 'Eksklusif Atau' logik bagi semua argumen.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logical1",
				description: "adalah 1 hingga 254 syarat yang anda ingin uji yang sama ada BENAR atau PALSU dan boleh menjadi nilai, tatasusunan atau rujukan logik"
			},
			{
				name: "logical2",
				description: "adalah 1 hingga 254 syarat yang anda ingin uji yang sama ada BENAR atau PALSU dan boleh menjadi nilai, tatasusunan atau rujukan logik"
			}
		]
	},
	{
		name: "YEAR",
		description: "Mengembalikan tahun bagi tarikh, integer dalam julat 1900 - 9999.",
		arguments: [
			{
				name: "serial_number",
				description: "ialah nombor dalam kod tarikh masa yang digunakan oleh Spreadsheet"
			}
		]
	},
	{
		name: "YEARFRAC",
		description: "Mengembalikan pecahan tahun yang mewakili bilangan hari penuh di antara tarikh_mula dan tarikh_tamat.",
		arguments: [
			{
				name: "start_date",
				description: "ialah nombor tarikh bersiri yang mewakili tarikh mula"
			},
			{
				name: "end_date",
				description: "ialah nombor tarikh bersiri yang mewakili tarikh tamat"
			},
			{
				name: "basis",
				description: "ialah jenis berdasarkan bilangan hari untuk digunakan"
			}
		]
	},
	{
		name: "YIELDDISC",
		description: "Mengembalikan hasil tahunan untuk keselamatan terdiskaun. Contohnya, bil perbendaharaan.",
		arguments: [
			{
				name: "settlement",
				description: "ialah tarikh penyelesaian keselamatan, disebut sebagai nombor tarikh bersiri"
			},
			{
				name: "maturity",
				description: "ialah tarikh kematangan keselamatan, disebut sebagai nombor tarikh bersiri"
			},
			{
				name: "pr",
				description: "ialah harga keselamatan per $100 nilai muka"
			},
			{
				name: "redemption",
				description: "ialah nilai penebusan Keselamatan per $100 nilai muka"
			},
			{
				name: "basis",
				description: "ialah jenis berdasarkan bilangan hari untuk digunakan"
			}
		]
	},
	{
		name: "Z.TEST",
		description: "Mengembalikan nilai P satu ekor bagi ujian z.",
		arguments: [
			{
				name: "array",
				description: "ialah tatasusunan atau julat data yang bertentangan untuk ujian X"
			},
			{
				name: "x",
				description: "ialah nilai untuk diuji"
			},
			{
				name: "sigma",
				description: "ialah sisihan piawai populasi (diketahui). Jika diabaikan, sisihan piawai sampel digunakan"
			}
		]
	},
	{
		name: "ZTEST",
		description: "Mengembalikan nilai P satu ekor bagi ujian z.",
		arguments: [
			{
				name: "array",
				description: "ialah tatasusunan atau julat data yang bertentangan untuk ujian X"
			},
			{
				name: "x",
				description: "ialah nilai untuk diuji"
			},
			{
				name: "sigma",
				description: "ialah sisihan piawai populasi (diketahui). Jika diabaikan, sisihan piawai sampel digunakan"
			}
		]
	}
];