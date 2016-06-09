ASPxClientSpreadsheet.Functions = [
	{
		name: "ABS",
		description: "Menampilkan nilai absolut dari angka, angka tanpa tanda tersebut.",
		arguments: [
			{
				name: "number",
				description: "adalah angka riil yang nilai absolutnya Anda inginkan"
			}
		]
	},
	{
		name: "ACCRINTM",
		description: "Menampilkan bunga aktual untuk sekuritas yang membayar suku bunga pada jatuh tempo.",
		arguments: [
			{
				name: "issue",
				description: "adalah tanggal pengeluaran sekuritas, yang dinyatakan sebagai angka tanggal seri"
			},
			{
				name: "settlement",
				description: "adalah tanggal jatuh tempo sekuritas, yang dinyatakan sebagai angka tanggal seri"
			},
			{
				name: "rate",
				description: "adalah tingkat kupon tahunan sekuritas"
			},
			{
				name: "par",
				description: "adalah nilai par sekuritas"
			},
			{
				name: "basis",
				description: "adalah jenis basis penghitungan hari untuk digunakan"
			}
		]
	},
	{
		name: "ACOS",
		description: "Menampilkan kurva kosinus dari angka, dalam radian pada rentang 0 sampai Pi. Kurva kosinus adalah sudut yang kosinusnya berupa Angka.",
		arguments: [
			{
				name: "number",
				description: "adalah kosinus dari sudut yang Anda inginkan dan harus dari -1 sampai 1"
			}
		]
	},
	{
		name: "ACOSH",
		description: "Menampilkan kosinus hiperbolik invers dari angka.",
		arguments: [
			{
				name: "number",
				description: "adalah angka riil yang sama dengan atau lebih besar dari 1"
			}
		]
	},
	{
		name: "ACOT",
		description: "Menghasilkan kurva kotangen dari angka dalam radian pada rentang 0 hingga Pi.",
		arguments: [
			{
				name: "number",
				description: "adalah kotangen dari sudut yang Anda inginkan"
			}
		]
	},
	{
		name: "ACOTH",
		description: "Menghasilkan kotangen balikan hiperbolik dari angka.",
		arguments: [
			{
				name: "number",
				description: "adalah kotangen hiperbolik dari sudut yang Anda inginkan"
			}
		]
	},
	{
		name: "ADDRESS",
		description: "Membuat referensi sel sebagai teks, memberikan nomor baris dan kolom tertentu.",
		arguments: [
			{
				name: "row_num",
				description: "adalah nomor baris yang digunakan dalam referensi sel: nomor_Baris = 1 untuk baris 1"
			},
			{
				name: "column_num",
				description: "adalah nomor kolom yang digunakan dalam referensi sel. Contoh, nomor_Kolom, = 4 untuk kolom D"
			},
			{
				name: "abs_num",
				description: "tentukan tipe referensi: absolut = 1; baris absolut/kolom relatif = 2; baris relatif/kolom absolut = 3; relatif = 4"
			},
			{
				name: "a1",
				description: "adalah nilai logis yang menentukan gaya referensi: gaya A1 = 1 atau BENAR; gaya R1C1 = 0 atau SALAH"
			},
			{
				name: "sheet_text",
				description: "adalah teks yang menentukan nama lembar-kerja yang digunakan sebagai referensi eksternal"
			}
		]
	},
	{
		name: "AND",
		description: "Memeriksa apakah semua argumen adalah BENAR, dan mengembalikan BENAR jika semua argumen adalah BENAR.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logical1",
				description: "adalah kondisi 1 sampai 255 yang ingin Anda tes yang dapat bernilai BENAR atau SALAH dan dapat berupa nilai, array, atau referensi logis"
			},
			{
				name: "logical2",
				description: "adalah kondisi 1 sampai 255 yang ingin Anda tes yang dapat bernilai BENAR atau SALAH dan dapat berupa nilai, array, atau referensi logis"
			}
		]
	},
	{
		name: "ARABIC",
		description: "Mengonversi angka Romawi ke angka Arab.",
		arguments: [
			{
				name: "text",
				description: "adalah angka Romawi yang ingin Anda konversi"
			}
		]
	},
	{
		name: "AREAS",
		description: "Menampilkan jumlah area dalam referensi. Area adalah rentang dari sel yang berdekatan atau sel tunggal.",
		arguments: [
			{
				name: "reference",
				description: "adalah referensi ke sel atau rentang sel dan dapat menunjuk ke multi area"
			}
		]
	},
	{
		name: "ASIN",
		description: "Menampilkan kurva sinus angka dalam radian, dalam rentang -Pi/2 sampai Pi/2.",
		arguments: [
			{
				name: "number",
				description: "adalah sinus dari sudut yang Anda inginkan dan harus dari -1 sampai 1"
			}
		]
	},
	{
		name: "ASINH",
		description: "Menampilkan sinus hiperbolik invers dari angka.",
		arguments: [
			{
				name: "number",
				description: "adalah tiap angka riil sama dengan atau lebih besar dari 1"
			}
		]
	},
	{
		name: "ATAN",
		description: "Menampilkan kurva tangen dari angka dalam radian, pada rentang -Pi/2 ke Pi/2.",
		arguments: [
			{
				name: "number",
				description: "adalah tangen dari sudut yang Anda inginkan"
			}
		]
	},
	{
		name: "ATAN2",
		description: "Menampilkan kurva tangen dari koordinat -x dan -y yang ditentukan, dalam radian antara -Pi dan Pi, tidak termasuk -Pi.",
		arguments: [
			{
				name: "x_num",
				description: "adalah koordinat-x dari titik"
			},
			{
				name: "y_num",
				description: "adalah koordinat-y dari titik"
			}
		]
	},
	{
		name: "ATANH",
		description: "Menampilkan invers tangen hiperbolik dari angka.",
		arguments: [
			{
				name: "number",
				description: "adalah angka riil di antara -1 dan 1 mengeluarkan -1 dan 1"
			}
		]
	},
	{
		name: "AVEDEV",
		description: "Menampilkan rata-rata simpangan absolut poin data dari nilai rata-ratanya. Argumen dapat berupa angka atau nama, aray atau referensi yang mengandung angka.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "adalah argumen 1 sampai 255 dimana Anda menginginkan rata-rata dari simpangan absolutnya"
			},
			{
				name: "number2",
				description: "adalah argumen 1 sampai 255 dimana Anda menginginkan rata-rata dari simpangan absolutnya"
			}
		]
	},
	{
		name: "AVERAGE",
		description: "Menampilkan rata-rata (nilai rata-rata aritmatika) dari argumennya, yang dapat berupa angka atau nama, array, atau referensi yang mengandung angka.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "adalah argumen numerik 1 sampai 255 yang Anda inginkan rata-ratanya"
			},
			{
				name: "number2",
				description: "adalah argumen numerik 1 sampai 255 yang Anda inginkan rata-ratanya"
			}
		]
	},
	{
		name: "AVERAGEA",
		description: "Menampilkan rata-rata (nilai rata-rata aritmatika) dari argumennya, mengevaluasi teks dan SALAH dalam argumen sebagai 0; BENAR sebagai 1. Argumen dapat berupa angka atau nama, array, atau referensi.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "adalah argumen 1 sampai 255 yang rata-ratanya Anda inginkan"
			},
			{
				name: "value2",
				description: "adalah argumen 1 sampai 255 yang rata-ratanya Anda inginkan"
			}
		]
	},
	{
		name: "AVERAGEIF",
		description: "Temukan rata-rata(rata-rata aritmatik)untuk sel yang ditentukan oleh kondisi atau kriteria yang diberikan.",
		arguments: [
			{
				name: "range",
				description: "adalah rentang sel yang Anda ingin evaluasi"
			},
			{
				name: "criteria",
				description: "adalah kondisi atau kriteria dalam formulir dari bilangan, ekspresi, atau teks yang menetapkan sel mana yang akan digunakan untuk menemukan rata-rata"
			},
			{
				name: "average_range",
				description: "adalah sel aktual yang akan digunakan untuk menemukan rata-rata. Jika diabaikan, sel dalam rentang sedang digunakan "
			}
		]
	},
	{
		name: "AVERAGEIFS",
		description: "Temukan rata-rata(rata-rata aritmatematika) untuk sel yang ditentukan dengan diberikan sebuah rangkaian kondisi atau kriteria.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "average_range",
				description: "adalah sel aktual yang digunakan untuk menemukan rata-rata."
			},
			{
				name: "criteria_range",
				description: "adalah rentang sel yang Anda ingin evaluasi untuk kondisi tertentu"
			},
			{
				name: "criteria",
				description: "adalah kondisi atau kriteria dalam formulir angka, ekspresi, atau teks yang menetapkan sel mana yang akan digunakan untuk menemukan rata-rata"
			}
		]
	},
	{
		name: "BAHTTEXT",
		description: "Mengonversi angka ke teks (baht).",
		arguments: [
			{
				name: "number",
				description: "adalah angka yang ingin Anda ubah"
			}
		]
	},
	{
		name: "BASE",
		description: "Mengonversi angka menjadi representasi teks dengan radiks tertentu (dasar).",
		arguments: [
			{
				name: "number",
				description: "adalah angka yang ingin Anda konversi"
			},
			{
				name: "radix",
				description: "adalah Radiks dasar yang ingin Anda jadikan hasil konversi angka"
			},
			{
				name: "min_length",
				description: "adalah panjang minimal string yang dihasilkan.  Jika dihilangkan, nol di awal tidak ditambahkan"
			}
		]
	},
	{
		name: "BESSELI",
		description: "Menampilkan fungsi Bessel termodifikasi In(x).",
		arguments: [
			{
				name: "x",
				description: "adalah nilai untuk mengevaluasi fungsi"
			},
			{
				name: "n",
				description: "adalah urutan fungsi Bessel"
			}
		]
	},
	{
		name: "BESSELJ",
		description: "Menampilkan fungsi Bessel Jn(x).",
		arguments: [
			{
				name: "x",
				description: "adalah nilai untuk mengevaluasi fungsi"
			},
			{
				name: "n",
				description: "adalah urutan fungsi Bessel"
			}
		]
	},
	{
		name: "BESSELK",
		description: "Menampilkan fungsi Bessel termodifikasi Kn(x).",
		arguments: [
			{
				name: "x",
				description: "adalah nilai untuk mengevaluasi fungsi"
			},
			{
				name: "n",
				description: "adalah urutan fungsi"
			}
		]
	},
	{
		name: "BESSELY",
		description: "Menampilkan fungsi Bessel Yn(x).",
		arguments: [
			{
				name: "x",
				description: "adalah nilai untuk mengevaluasi fungsi"
			},
			{
				name: "n",
				description: "adalah urutan fungsi"
			}
		]
	},
	{
		name: "BETA.DIST",
		description: "Menghasilkan fungsi distribusi probabilitas beta.",
		arguments: [
			{
				name: "x",
				description: "adalah nilai di antara A dan B untuk mengevaluasi fungsi"
			},
			{
				name: "alpha",
				description: "adalah parameter untuk distribusi dan harus lebih besar daripada 0"
			},
			{
				name: "beta",
				description: "adalah parameter untuk distribusi dan harus lebih besar daripada 0"
			},
			{
				name: "cumulative",
				description: "adalah nilai logis: untuk fungsi distribusi kumulatif, gunakan TRUE; untuk fungsi kerapatan probabilitas, gunakan FALSE"
			},
			{
				name: "A",
				description: "adalah batas bawah opsional ke interval x. Jika dihilangkan, A = 0"
			},
			{
				name: "B",
				description: "adalah batas atas opsional ke interval x. Jika dihilangkan, B = 1"
			}
		]
	},
	{
		name: "BETA.INV",
		description: "Menghasilkan balikan dari fungsi kerapatan probabilitas kumulatif beta (BETA.DIST).",
		arguments: [
			{
				name: "probability",
				description: "adalah probabilitas yang terkait dengan distribusi beta"
			},
			{
				name: "alpha",
				description: "adalah parameter untuk distribusi dan harus lebih besar daripada 0"
			},
			{
				name: "beta",
				description: "adalah parameter untuk distribusi dan harus lebih besar daripada 0"
			},
			{
				name: "A",
				description: "adalah batas bawah opsional ke interval x. Jika dihilangkan, A = 0"
			},
			{
				name: "B",
				description: "adalah batas atas opsional ke interval x. Jika dihilangkan, B = 1"
			}
		]
	},
	{
		name: "BETADIST",
		description: "Menampilkan fungsi kerapatan probabilitas beta kumulatif.",
		arguments: [
			{
				name: "x",
				description: "adalah nilai di antara A dan B untuk mengevaluasi fungsi"
			},
			{
				name: "alpha",
				description: "adalah parameter untuk distribusi dan harus lebih besar daripada 0."
			},
			{
				name: "beta",
				description: "adalah sebuah parameter untuk distribusi dan harus lebih besar daripada 0."
			},
			{
				name: "A",
				description: "adalah batas bawah opsional ke interval x. Jika dihilangkan, A = 0"
			},
			{
				name: "B",
				description: "adalah batas atas opsional ke interval x. Jika dihilangkan, B = 1"
			}
		]
	},
	{
		name: "BETAINV",
		description: "Menampilkan inversi dari fungsi kerapatan probabilitas kumulatif beta (BETADIST).",
		arguments: [
			{
				name: "probability",
				description: "adalah probabilitas yang berhubungan dengan distribusi beta"
			},
			{
				name: "alpha",
				description: "adalah parameter untuk distribusi dan harus lebih besar dari 0"
			},
			{
				name: "beta",
				description: "adalah parameter untuk distribusi dan harus lebih besar dari 0"
			},
			{
				name: "A",
				description: "adalah batas bawah opsional ke interval x. Jika dihilangkan, A = 0"
			},
			{
				name: "B",
				description: "adalah batas atas opsional ke interval x. Jika dihilangkan, B = 1"
			}
		]
	},
	{
		name: "BIN2DEC",
		description: "Mengonversi sebuah bilangan biner ke desimal.",
		arguments: [
			{
				name: "number",
				description: "adalah bilangan biner yang ingin Anda konversi"
			}
		]
	},
	{
		name: "BIN2HEX",
		description: "Mengonversi sebuah bilangan biner ke hexadesimal.",
		arguments: [
			{
				name: "number",
				description: "adalah bilangan biner yang ingin Anda konversi"
			},
			{
				name: "places",
				description: "adalah jumlah karakter untuk digunakan"
			}
		]
	},
	{
		name: "BIN2OCT",
		description: "Mengonversi sebuah bilangan biner ke oktal.",
		arguments: [
			{
				name: "number",
				description: "adalah bilangan biner yang ingin Anda konversi"
			},
			{
				name: "places",
				description: "adalah jumlah karakter untuk digunakan"
			}
		]
	},
	{
		name: "BINOM.DIST",
		description: "Menampilkan probabilitas distribusi binomial term individu.",
		arguments: [
			{
				name: "number_s",
				description: "adalah jumlah kesuksesan dalam percobaan"
			},
			{
				name: "trials",
				description: "adalah jumlah percobaan independen"
			},
			{
				name: "probability_s",
				description: "adalah kemungkinan keberhasilan pada tiap percobaan"
			},
			{
				name: "cumulative",
				description: "adalah nilai logis: untuk fungsi distribusi kumulatif, gunakan BENAR; untuk fungsi massa probabilitas, gunakan SALAH"
			}
		]
	},
	{
		name: "BINOM.DIST.RANGE",
		description: "Menghasilkan probabilitas hasil percobaan dengan distribusi binomial.",
		arguments: [
			{
				name: "trials",
				description: "adalah jumlah percobaan independen"
			},
			{
				name: "probability_s",
				description: "adalah probabilitas keberhasilan di setiap percobaan"
			},
			{
				name: "number_s",
				description: "adalah jumlah keberhasilan dalam percobaan"
			},
			{
				name: "number_s2",
				description: "jika diberikan, fungsi ini menghasilkan probabilitas yang jumlah percobaan berhasilnya harus antara number_s dan number_s2"
			}
		]
	},
	{
		name: "BINOM.INV",
		description: "Menampilkan nilai terkecil di mana distribusi kumulatif binomial lebih besar dari atau sama dengan nilai standar.",
		arguments: [
			{
				name: "trials",
				description: "adalah jumlah percobaan Bernoulli"
			},
			{
				name: "probability_s",
				description: "adalah probabilitas keberhasilan dalam tiap percobaan, yaitu angka di antara 0 sampai dengan 1"
			},
			{
				name: "alpha",
				description: "adalah nilai standar, yaitu angka di antara 0 sampai dengan 1"
			}
		]
	},
	{
		name: "BINOMDIST",
		description: "Menampilkan probabilitas distribusi binomial ketentuan individu.",
		arguments: [
			{
				name: "number_s",
				description: "adalah jumlah keberhasilan dalam percobaan"
			},
			{
				name: "trials",
				description: "adalah jumlah percobaan secara bebas"
			},
			{
				name: "probability_s",
				description: "adalah kemungkinan keberhasilan pada tiap percobaan"
			},
			{
				name: "cumulative",
				description: "adalah nilai logis: untuk fungsi distribusi kumulatif, gunakan BENAR; untuk fungsi massa probabilitas, gunakan SALAH"
			}
		]
	},
	{
		name: "BITAND",
		description: "Menghasilkan bitwise 'Dan' dari dua angka.",
		arguments: [
			{
				name: "number1",
				description: "adalah representasi desimal dari angka biner yang ingin Anda evaluasi"
			},
			{
				name: "number2",
				description: "adalah representasi desimal dari angka biner yang ingin Anda evaluasi"
			}
		]
	},
	{
		name: "BITLSHIFT",
		description: "Menampilkan angka yang dialihkan ke kiri dengan shift_amount bits.",
		arguments: [
			{
				name: "number",
				description: "adalah representasi desimal angka biner yang ingin Anda evaluasi"
			},
			{
				name: "shift_amount",
				description: "adalah angka bit yang Angkanya ingin Anda alihkan ke kiri"
			}
		]
	},
	{
		name: "BITOR",
		description: "Menghasilkan bitwise 'Atau' dari dua angka.",
		arguments: [
			{
				name: "number1",
				description: "adalah representasi desimal dari angka biner yang ingin Anda evaluasi"
			},
			{
				name: "number2",
				description: "adalah representasi desimal dari angka biner yang ingin Anda evaluasi"
			}
		]
	},
	{
		name: "BITRSHIFT",
		description: "Menampilkan angka yang dialihkan ke kanan dengan shift_amount bits.",
		arguments: [
			{
				name: "number",
				description: "adalah representasi desimal angka biner yang ingin Anda evaluasi"
			},
			{
				name: "shift_amount",
				description: "adalah angka bit yang Angkanya ingin Anda alihkan ke kanan"
			}
		]
	},
	{
		name: "BITXOR",
		description: "Menghasilkan bitwise 'Eksklusif Atau' dari dua angka.",
		arguments: [
			{
				name: "number1",
				description: "adalah representasi desimal dari angka biner yang ingin Anda evaluasi"
			},
			{
				name: "number2",
				description: "adalah representasi desimal dari angka biner yang ingin Anda evaluasi"
			}
		]
	},
	{
		name: "CEILING",
		description: "Membulatkan angka ke atas, ke bilangan bulat terdekat atau ke multipel terdekat dari signifikansi.",
		arguments: [
			{
				name: "number",
				description: "adalah nilai yang ingin Anda bulatkan"
			},
			{
				name: "significance",
				description: "adalah multipel yang ingin Anda bulatkan"
			}
		]
	},
	{
		name: "CEILING.MATH",
		description: "Membulatkan angka ke atas, ke bilangan bulat terdekat atau ke kelipatan signifikansi terdekat.",
		arguments: [
			{
				name: "number",
				description: "adalah nilai yang ingin Anda bulatkan"
			},
			{
				name: "significance",
				description: "adalah kelipatan opsional yang ingin Anda bulatkan"
			},
			{
				name: "mode",
				description: "jika diberikan dan selain nol, fungsi ini akan membulatkan jauh dari nol"
			}
		]
	},
	{
		name: "CEILING.PRECISE",
		description: "Membulatkan angka ke atas, ke bilangan bulat terdekat atau ke multipel terdekat dari signifikansi.",
		arguments: [
			{
				name: "number",
				description: "adalah nilai yang ingin Anda bulatkan"
			},
			{
				name: "significance",
				description: "adalah multipel yang ingin Anda bulatkan"
			}
		]
	},
	{
		name: "CELL",
		description: "Menampilkan informasi tentang pemformatan, lokasi atau isi dari sel pertama, berdasarkan pada urutan membaca lembar, dalam referensi.",
		arguments: [
			{
				name: "info_type",
				description: "adalah nilai teks yang menentukan tipe informasi sel apa yang Anda inginkan."
			},
			{
				name: "reference",
				description: "adalah sel yang informasinya Anda inginkan"
			}
		]
	},
	{
		name: "CHAR",
		description: "Menampilkan karakter yang ditentukan oleh nomor kode dari perangkat karakter untuk komputer Anda.",
		arguments: [
			{
				name: "number",
				description: "adalah angka di antara 1 dan 255 yang menentukan karakter yang mana yang Anda inginkan"
			}
		]
	},
	{
		name: "CHIDIST",
		description: "Menampilkan probabilitas satu-lemparan dari distribusi khi-kuadrat.",
		arguments: [
			{
				name: "x",
				description: "adalah nilai di mana Anda ingin gunakan untuk mengevaluasi distribusi, angka bukan negatif"
			},
			{
				name: "deg_freedom",
				description: "adalah jumlah pangkat, yaitu angka di antara 1 dan 10^10, mengecualikan 10^10"
			}
		]
	},
	{
		name: "CHIINV",
		description: "Menampilkan inversi dari probabilitas satu-lemparan dari distribusi khi-kuadrat.",
		arguments: [
			{
				name: "probability",
				description: "adalah probabilitas yang berhubungan dengan distribusi khi-kuadrat, nilai di antara 0 sampai dengan 1"
			},
			{
				name: "deg_freedom",
				description: "adalah jumlah pangkat, yaitu angka di antara 1 dan 10^10, mengecualikan 10^10"
			}
		]
	},
	{
		name: "CHISQ.DIST",
		description: "Menghasilkan probabilitas lemparan-kiri dari distribusi khi-kuadrat.",
		arguments: [
			{
				name: "x",
				description: "adalah nilai yang ingin Anda ingin gunakan untuk mengevaluasi distribusi, bilangan non-negatif"
			},
			{
				name: "deg_freedom",
				description: "adalah jumlah pangkat, yaitu angka di antara 1 dan 10^10, tidak termasuk 10^10"
			},
			{
				name: "cumulative",
				description: "adalah nilai logis untuk fungsi yang muncul: fungsi distribusi kumulatif = TRUE; fungsi kerapatan probabilitas = FALSE"
			}
		]
	},
	{
		name: "CHISQ.DIST.RT",
		description: "Menghasilkan probabilitas lemparan-kanan dari distribusi khi-kuadrat.",
		arguments: [
			{
				name: "x",
				description: "adalah nilai yang ingin Anda gunakan untuk mengevaluasi distribusi, bilangan non-negatif"
			},
			{
				name: "deg_freedom",
				description: "adalah jumlah pangkat, yaitu angka di antara 1 dan 10^10, tidak termasuk 10^10"
			}
		]
	},
	{
		name: "CHISQ.INV",
		description: "Menghasilkan balikan dari probabilitas lemparan-kiri dari distribusi khi-kuadrat.",
		arguments: [
			{
				name: "probability",
				description: "adalah probabilitas yang terkait dengan distribusi khi-kuadrat, termasuk nilai di antara 0 sampai 1"
			},
			{
				name: "deg_freedom",
				description: "adalah jumlah pangkat, yaitu angka di antara 1 dan 10^10, tidak termasuk 10^10"
			}
		]
	},
	{
		name: "CHISQ.INV.RT",
		description: "Menghasilkan balikan dari probabilitas lemparan-kanan dari distribusi khi-kuadrat.",
		arguments: [
			{
				name: "probability",
				description: "adalah probabilitas yang terkait dengan distribusi khi-kuadrat, termasuk nilai di antara 0 sampai 1"
			},
			{
				name: "deg_freedom",
				description: "adalah jumlah pangkat, yaitu angka di antara 1 dan 10^10, tidak termasuk 10^10"
			}
		]
	},
	{
		name: "CHISQ.TEST",
		description: "Menampilkan tes untuk independen: nilai dari distribusi khi-kuadrat untuk statistik dan pangkat yang tepat.",
		arguments: [
			{
				name: "actual_range",
				description: "adalah rentang data yang mengandung observasi untuk dites terhadap nilai yang diharapkan"
			},
			{
				name: "expected_range",
				description: "adalah rentang data yang mengandung rasio produk dari total baris dan kolom ke total seluruhnya"
			}
		]
	},
	{
		name: "CHITEST",
		description: "Menampilkan uji secara bebas: nilai dari distribusi khi-kuadrat untuk statistik dan pangkat yang tepat.",
		arguments: [
			{
				name: "actual_range",
				description: "adalah rentang data yang mengandung observasi untuk diuji terhadap nilai yang diharapkan"
			},
			{
				name: "expected_range",
				description: "adalah rentang data yang mengandung rasio produk dari total baris dan kolom ke total seluruhnya"
			}
		]
	},
	{
		name: "CHOOSE",
		description: "Memilih nilai atau tindakan yang dilakukan dari daftar nilai, berdasarkan nomor indeks.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "index_num",
				description: "menentukan argumen nilai yang mana yang terpilih. Indeks_num harus di antara 1 dan 254, atau rumus atau referensi ke angka di antara 1 dan 254"
			},
			{
				name: "value1",
				description: "adalah angka, referensi sel, nama yang ditentukan, rumus, fungsi, atau argumen teks 1 sampai 254 dari mana CHOOSE memilih"
			},
			{
				name: "value2",
				description: "adalah angka, referensi sel, nama yang ditentukan, rumus, fungsi, atau argumen teks 1 sampai 254 dari mana CHOOSE memilih"
			}
		]
	},
	{
		name: "CLEAN",
		description: "Menghapus semua karakter yang tidak dapat dicetak dari teks.",
		arguments: [
			{
				name: "text",
				description: "adalah informasi lembar kerja mana saja yang karakter tak-tercetaknya ingin Anda hapus"
			}
		]
	},
	{
		name: "CODE",
		description: "Menampilkan kode numerik untuk karakter pertama pada string teks, pada perangkat karakter yang digunakan oleh komputer Anda.",
		arguments: [
			{
				name: "text",
				description: "adalah teks yang Anda inginkan kodenya dari karakter pertama"
			}
		]
	},
	{
		name: "COLUMN",
		description: "Menampilkan nomor kolom dari referensi.",
		arguments: [
			{
				name: "reference",
				description: "adalah sel atau rentang dari sel yang berdekatan yang Anda ingin kolomnya dinomori. Jika diabaikan, sel yang mengandung fungsi KOLOM digunakan"
			}
		]
	},
	{
		name: "COLUMNS",
		description: "Menampilkan jumlah kolom dalam array atau referensi.",
		arguments: [
			{
				name: "array",
				description: "adalah array atau rumus array, atau referensi ke rentang sel yang jumlah kolomnya Anda inginkan"
			}
		]
	},
	{
		name: "COMBIN",
		description: "Menampilkan jumlah kombinasi untuk jumlah item yang ditentukan.",
		arguments: [
			{
				name: "number",
				description: "adalah jumlah total item"
			},
			{
				name: "number_chosen",
				description: "adalah jumlah item dalam tiap kombinasi"
			}
		]
	},
	{
		name: "COMBINA",
		description: "Menghasilkan jumlah kombinasi dengan pengulangan untuk sejumlah objek tertentu.",
		arguments: [
			{
				name: "number",
				description: "adalah total jumlah item"
			},
			{
				name: "number_chosen",
				description: "adalah jumlah item di setiap kombinasi"
			}
		]
	},
	{
		name: "COMPLEX",
		description: "Mengonversi koefisien nyata dan bayangan ke dalam bilangan kompleks.",
		arguments: [
			{
				name: "real_num",
				description: "adalah koefisien nyata dari bilangan kompleks"
			},
			{
				name: "i_num",
				description: "adalah koefisien bayangan dari bilangan kompleks"
			},
			{
				name: "suffix",
				description: "adalah sufiks untuk komponen bayangan dari bilangan kompleks"
			}
		]
	},
	{
		name: "CONCATENATE",
		description: "Gabungkan beberapa string teks ke dalam satu string teks.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "text1",
				description: "adalah string teks 1 sampai 255 untuk digabungkan ke dalam string teks tunggal dan dapat berupa string teks, angka, atau referensi sel-tunggal"
			},
			{
				name: "text2",
				description: "adalah string teks 1 sampai 255 untuk digabungkan ke dalam string teks tunggal dan dapat berupa string teks, angka, atau referensi sel-tunggal"
			}
		]
	},
	{
		name: "CONFIDENCE",
		description: "Menampilkan interval kepercayaan untuk nilai rata-rata populasi, menggunakan distribusi normal.",
		arguments: [
			{
				name: "alpha",
				description: "adalah tingkat signikan yang digunakan untuk menghitung tingkat kepercayaan, angka yang lebih besar dari  0 dan lebih kecil dari 1"
			},
			{
				name: "standard_dev",
				description: "adalah populasi simpangan baku untuk rentang data dan diasumsikan dikenali. Standar_dev harus lebih besar dari 0"
			},
			{
				name: "size",
				description: "adalah ukuran contoh"
			}
		]
	},
	{
		name: "CONFIDENCE.NORM",
		description: "Menghasilkan interval kepercayaan untuk rata-rata populasi, dengan distribusi normal.",
		arguments: [
			{
				name: "alpha",
				description: "adalah tingkat signifikansi yang digunakan untuk menghitung tingkat kepercayaan, angka yang lebih besar daripada 0 dan lebih kecil daripada 1"
			},
			{
				name: "standard_dev",
				description: "adalah simpangan standar populasi untuk rentang data tersebut dan dianggap diketahui. Standard_dev harus lebih besar daripada 0"
			},
			{
				name: "size",
				description: "adalah ukuran sampel"
			}
		]
	},
	{
		name: "CONFIDENCE.T",
		description: "Menghasilkan interval kepercayaan untuk rata-rata populasi, dengan distribusi T Pelajar.",
		arguments: [
			{
				name: "alpha",
				description: "adalah tingkat signifikansi yang digunakan untuk menghitung tingkat kepercayaan, angka yang lebih besar daripada 0 dan lebih kecil daripada 1"
			},
			{
				name: "standard_dev",
				description: "adalah simpangan standar populasi untuk rentang data tersebut dan dianggap diketahui. Standard_dev harus lebih besar daripada 0"
			},
			{
				name: "size",
				description: "adalah ukuran sampel"
			}
		]
	},
	{
		name: "CONVERT",
		description: "Mengubah bilangan dari satu sistem pengukuran ke lainnya.",
		arguments: [
			{
				name: "number",
				description: "adalah nilai dalam dari_unit untuk dikonversi"
			},
			{
				name: "from_unit",
				description: "adalah unit bilangan"
			},
			{
				name: "to_unit",
				description: "adalah unit untuk hasil"
			}
		]
	},
	{
		name: "CORREL",
		description: "Menampilkan koefisien korelasi di antara dua perangkat data.",
		arguments: [
			{
				name: "array1",
				description: "adalah rentang sel nilai. Nilai harus berupa angka, nama, array atau referensi yang mengandung angka"
			},
			{
				name: "array2",
				description: "adalah rentang sel nilai kedua. Nilai harus berupa angka, nama, array, atau referensi yang mengandung angka"
			}
		]
	},
	{
		name: "COS",
		description: "Menampilkan kosinus dari sudut.",
		arguments: [
			{
				name: "number",
				description: "Adalah sudut dalam radian kosinus yang Anda inginkan"
			}
		]
	},
	{
		name: "COSH",
		description: "Menampilkan kosinus hiperbolik dari angka.",
		arguments: [
			{
				name: "number",
				description: "adalah angka real apa saja"
			}
		]
	},
	{
		name: "COT",
		description: "Menghasilkan kotangen sudut.",
		arguments: [
			{
				name: "number",
				description: "adalah sudut dalam radian yang Anda inginkan kotangennya"
			}
		]
	},
	{
		name: "COTH",
		description: "Menghasilkan kotangen hiperbolik angka.",
		arguments: [
			{
				name: "number",
				description: "adalah sudut dalam radian yang Anda inginkan kotangen hiperboliknya"
			}
		]
	},
	{
		name: "COUNT",
		description: "Menghitung jumlah sel dalam rentang yang memuat angka.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: " adalah 1 sampai 255 argumen yang memuat atau mengacu tipe data yang bervariasi, tapi hanya angka yang dihitung"
			},
			{
				name: "value2",
				description: " adalah 1 sampai 255 argumen yang memuat atau mengacu tipe data yang bervariasi, tapi hanya angka yang dihitung"
			}
		]
	},
	{
		name: "COUNTA",
		description: "Menghitung jumlah sel dalam rentang yang tidak kosong.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "adalah argumen 1 sampai 255 argumen yang menunjukkan nilai dan sel yang ingin Anda hitung. Nilai dapat berupa tipe informasi apa saja"
			},
			{
				name: "value2",
				description: "adalah argumen 1 sampai 255 argumen yang menunjukkan nilai dan sel yang ingin Anda hitung. Nilai dapat berupa tipe informasi apa saja"
			}
		]
	},
	{
		name: "COUNTBLANK",
		description: "Menghitung jumlah sel kosong dalam rentang sel tertentu.",
		arguments: [
			{
				name: "range",
				description: "adalah rentang di mana Anda ingin menghitung sel yang kosong"
			}
		]
	},
	{
		name: "COUNTIF",
		description: "Menghitung jumlah sel dalam rentang yang sesuai dengan kondisi yang diberikan.",
		arguments: [
			{
				name: "range",
				description: "adalah rentang sel di mana Anda ingin menghitung sel yang berisi"
			},
			{
				name: "criteria",
				description: "adalah kondisi dalam bentuk angka, ekspresi, atau teks yang menetapkan sel yang ingin dihitung"
			}
		]
	},
	{
		name: "COUNTIFS",
		description: "Hitung jumlah sel yang ditentukan oleh pemberian set dari kondisi atau kriteria.",
		arguments: [
			{
				name: "criteria_range",
				description: "adalah rentang sel yang Anda ingin evaluasi untuk kondisi tertentu"
			},
			{
				name: "criteria",
				description: "adalah kondisi dalam formulir bilangan, ekspresi, atau teks yang menetapkan sel mana yang akan dihitung"
			}
		]
	},
	{
		name: "COUPDAYBS",
		description: "Menampilkan jumlah hari dari awal periode kupon ke tanggal pelunasan.",
		arguments: [
			{
				name: "settlement",
				description: "adalah tanggal pelunasan sekuritas, yang dinyatakan sebagai angka tanggal seri"
			},
			{
				name: "maturity",
				description: "adalah tanggal jatuh tempo sekuritas, yang dinyatakan sebagai angka tanggal seri"
			},
			{
				name: "frequency",
				description: "adalah jumlah pembayaran kupon per tahun"
			},
			{
				name: "basis",
				description: "adalah tipe basis hitungan hari untuk digunakan"
			}
		]
	},
	{
		name: "COUPNCD",
		description: "Menampilkan tanggal kupon selanjutnya setelah tanggal pelunasan.",
		arguments: [
			{
				name: "settlement",
				description: "adalah tanggal pelunasan sekuritas, yang dinyatakan sebagai angka tanggal seri"
			},
			{
				name: "maturity",
				description: "adalah tanggal jatuh tempo sekuritas, yang dinyatakan sebagai angka tanggal seri"
			},
			{
				name: "frequency",
				description: "adalah jumlah kupon pembayaran per tahun"
			},
			{
				name: "basis",
				description: "adalah tipe basis hitungan hari untuk digunakan"
			}
		]
	},
	{
		name: "COUPNUM",
		description: "Menampilkan jumlah kupon yang dapat dibayarkan antara tanggal pelunasan dan tanggal jatuh tempo.",
		arguments: [
			{
				name: "settlement",
				description: "adalah tanggal pelunasan sekuritas, yang dinyatakan sebagai angka tanggal seri"
			},
			{
				name: "maturity",
				description: "adalah tanggal jatuh tempo sekuritas, yang dinyatakan sebagai angka tanggal seri"
			},
			{
				name: "frequency",
				description: "adalah nomor pembayaran kupon per tahun"
			},
			{
				name: "basis",
				description: "adalah tipe basis hari untuk digunakan"
			}
		]
	},
	{
		name: "COUPPCD",
		description: "Menampilkan tanggal kupon sebelumnya sebelum tanggal pelunasan.",
		arguments: [
			{
				name: "settlement",
				description: "adalah tanggal pelunasan sekuritas, yang dinyatakan sebagai angka tanggal seri"
			},
			{
				name: "maturity",
				description: "adalah tanggal jatuh tempo sekuritas, yang dinyatakan sebagai angka tanggal seri"
			},
			{
				name: "frequency",
				description: "adalah jumlah pembayaran kupon per tahun"
			},
			{
				name: "basis",
				description: "adalah basis hitungan hari yang digunakan"
			}
		]
	},
	{
		name: "COVAR",
		description: "Menampilkan kovarian, rata-rata produk dari simpangan untuk tiap pasangan poin data dalam dua perangkat data.",
		arguments: [
			{
				name: "array1",
				description: "adalah rentang sel bilangan bulat pertama dan harus berupa angka, array, atau referensi yang mengandung angka"
			},
			{
				name: "array2",
				description: "adalah rentang sel bilangan bulat kedua dan harus berupa angka, array, atau referensi yang mengandung angka"
			}
		]
	},
	{
		name: "COVARIANCE.P",
		description: "Menghasilkan kovarian populasi, rata-rata produk dari simpangan untuk tiap pasangan poin data dalam dua set data.",
		arguments: [
			{
				name: "array1",
				description: "adalah rentang sel bilangan bulat pertama dan harus berupa angka, array, atau referensi yang berisi angka"
			},
			{
				name: "array2",
				description: "adalah rentang sel bilangan bulat kedua dan harus berupa angka, array, atau referensi yang berisi angka"
			}
		]
	},
	{
		name: "COVARIANCE.S",
		description: "Menghasilkan kovarian sampel, rata-rata produk dari simpangan untuk tiap pasangan poin data dalam dua set data.",
		arguments: [
			{
				name: "array1",
				description: "adalah rentang sel bilangan bulat pertama dan harus berupa angka, array, atau referensi yang berisi angka"
			},
			{
				name: "array2",
				description: "adalah rentang sel bilangan bulat kedua dan harus berupa angka, array, atau referensi yang berisi angka"
			}
		]
	},
	{
		name: "CRITBINOM",
		description: "Menampilkan nilai terkecil di mana distribusi kumulatif binomial lebih besar dari atau sama dengan nilai standar.",
		arguments: [
			{
				name: "trials",
				description: "adalah jumlah percobaan Bernoulli"
			},
			{
				name: "probability_s",
				description: "adalah probabilitas keberhasilan dalam tiap percobaan, yaitu angka di antara 0 sampai dengan 1"
			},
			{
				name: "alpha",
				description: "adalah nilai standar, yaitu angka di antara 0 sampai dengan 1"
			}
		]
	},
	{
		name: "CSC",
		description: "Menghasilkan kosekan sudut.",
		arguments: [
			{
				name: "number",
				description: "adalah sudut dalam radian yang Anda inginkan kosekannya"
			}
		]
	},
	{
		name: "CSCH",
		description: "Menghasilkan kosekan hiperbolik angka.",
		arguments: [
			{
				name: "number",
				description: "adalah sudut dalam radian yang Anda inginkan kosekan hiperboliknya"
			}
		]
	},
	{
		name: "CUMIPMT",
		description: "Menampilkan bunga kumulatif yang dibayarkan antara dua periode.",
		arguments: [
			{
				name: "rate",
				description: "adalah tingkat bunga"
			},
			{
				name: "nper",
				description: "adalah jumlah total periode pembayaran"
			},
			{
				name: "pv",
				description: "adalah nilai saat ini"
			},
			{
				name: "start_period",
				description: "adalah periode pertama dalam perhitungan"
			},
			{
				name: "end_period",
				description: "adalah periode terakhir dalam perhitungan"
			},
			{
				name: "type",
				description: "adalah penentuan waktu pembayaran"
			}
		]
	},
	{
		name: "CUMPRINC",
		description: "Pembayaran utama kumulatif pada pinjaman antara dua periode.",
		arguments: [
			{
				name: "rate",
				description: "adalah tingkat bunga"
			},
			{
				name: "nper",
				description: "adalah jumlah total periode pembayaran"
			},
			{
				name: "pv",
				description: "adalah nilai saat ini"
			},
			{
				name: "start_period",
				description: "adalah periode pertama dalam perhitungan"
			},
			{
				name: "end_period",
				description: "adalah periode terakhir dalam perhitungan"
			},
			{
				name: "type",
				description: "adalah penentuan waktu pembayaran"
			}
		]
	},
	{
		name: "DATE",
		description: "Menampilkan angka yang mewakili tanggal dalam kode tanggal-waktu Spreadsheet.",
		arguments: [
			{
				name: "year",
				description: "adalah angka dari 1900 sampai 9999 dalam Spreadsheet untuk Windows atau dari 1904 sampai 9999 dalam Spreadsheet untuk Macintosh"
			},
			{
				name: "month",
				description: "adalah angka dari 1 sampai 12 yang mewakili bulan dalam satu tahun"
			},
			{
				name: "day",
				description: "adalah angka dari 1 sampai 31 yang mewakili hari dalam satu bulan"
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
		description: "Mengonversi tanggal dalam bentuk teks ke angka yang mewakili tanggal dalam kode tanggal-waktu Spreadsheet.",
		arguments: [
			{
				name: "date_text",
				description: "adalah teks yang mewakili tanggal dalam format tanggal Spreadsheet, di antara 1/1/1900 (Windows) atau 1/1/1904 (Macintosh) dan 12/31/9999"
			}
		]
	},
	{
		name: "DAVERAGE",
		description: "Merata-rata nilai dalam kolom pada daftar atau database yang cocok dengan kondisi yang Anda tentukan.",
		arguments: [
			{
				name: "database",
				description: "adalah rentang sel yang membuat daftar atau database. Database adalah daftar dari data terkait"
			},
			{
				name: "field",
				description: "adalah label kolom dalam tanda kutip ganda atau angka yang menunjukkan posisi kolom dalam daftar"
			},
			{
				name: "criteria",
				description: "adalah rentang sel yang mengandung kondisi yang Anda tentukan. Rentang menyertakan label kolom dan sebuah sel di bawah label sebagai kondisi"
			}
		]
	},
	{
		name: "DAY",
		description: "Menampilkan hari dari satu bulan, angka dari 1 sampai 31.",
		arguments: [
			{
				name: "serial_number",
				description: "adalah angka dalam kode tanggal-waktu yang digunakan oleh Spreadsheet"
			}
		]
	},
	{
		name: "DAYS",
		description: "Menghasilkan jumlah hari antara dua tanggal.",
		arguments: [
			{
				name: "end_date",
				description: "start_date dan end_date adalah dua tanggal yang ingin Anda ketahui jumlah hari di antara keduanya"
			},
			{
				name: "start_date",
				description: "start_date dan end_date adalah dua tanggal yang ingin Anda ketahui jumlah hari di antara keduanya"
			}
		]
	},
	{
		name: "DAYS360",
		description: "Menampilkan jumlah hari di antara dua tanggal berdasarkan pada 360-hari setahun (dua belas bulan per bulan 30-hari).",
		arguments: [
			{
				name: "start_date",
				description: "tanggal_mulai dan tanggal_selesai adalah dua tanggal yang harus Anda ketahui jika ingin menghitung jumlah hari"
			},
			{
				name: "end_date",
				description: "tanggal_mulai dan tanggal_selesai adalah dua tanggal yang harus Anda ketahui jika ingin menghitung jumlah hari"
			},
			{
				name: "method",
				description: "adalah nilai logis yang menentukan metode perhitungan: U.S. (NASD) = SALAH atau dihilangkan; European = BENAR."
			}
		]
	},
	{
		name: "DB",
		description: "Menampilkan depresiasi aset untuk periode tertentu menggunakan metode pengurangan-tetap saldo.",
		arguments: [
			{
				name: "cost",
				description: "adalah nilai awal aset"
			},
			{
				name: "salvage",
				description: "adalah akumulasi penyusutan nilai pada akhir jangka waktu aset"
			},
			{
				name: "life",
				description: "adalah periode pendepresiasian aset (kadang-kadang disebut nilai jual aset)"
			},
			{
				name: "period",
				description: "adalah periode penghitungan depresiasinya. Periode harus menggunakan unit yang sama dengan Life"
			},
			{
				name: "month",
				description: "adalah jumlah bulan pada tahun pertama. Jika bulan dihilangkan, jumlah bulan diasumsikan 12"
			}
		]
	},
	{
		name: "DCOUNT",
		description: "Menghitung sel yang mengandung angka dalam bidang (kolom) dari catatan dalam database yang sesuai dengan kondisi yang Anda tentukan.",
		arguments: [
			{
				name: "database",
				description: "adalah rentang sel yang membuat daftar atau database. Database adalah daftar dari data yang terkait"
			},
			{
				name: "field",
				description: "adalah label kolom dalam tanda kutip ganda atau angka yang menunjukkan posisi kolom dalam daftar"
			},
			{
				name: "criteria",
				description: "adalah rentang sel yang mengandung kondisi yang Anda tentukan. Rentang menyertakan label kolom dan satu sel di bawah label sebagai kondisi"
			}
		]
	},
	{
		name: "DCOUNTA",
		description: "Menghitung sel yang tidak kosong dalam bidang (kolom) dari catatan dalam database yang cocok dengan kondisi yang Anda tentukan.",
		arguments: [
			{
				name: "database",
				description: "adalah rentang sel yang membuat daftar atau database. Database adalah daftar dari data yang terkait"
			},
			{
				name: "field",
				description: "adalah label kolom dalam tanda kutip ganda atau angka yang menunjukkan posisi kolom dalam daftar"
			},
			{
				name: "criteria",
				description: "adalah rentang sel yang mengandung kondisi yang Anda tentukan. Rentang menyertakan label kolom dan satu sel di bawah label sebagai kondisi"
			}
		]
	},
	{
		name: "DDB",
		description: "Menampilkan depresiasi aset untuk periode tertentu menggunakan metode saldo pengurangan-ganda atau beberapa metode lain yang Anda tentukan.",
		arguments: [
			{
				name: "cost",
				description: "adalah nilai awal aset"
			},
			{
				name: "salvage",
				description: "adalah akumulasi penyusutan nilai pada akhir jangka waktu aset"
			},
			{
				name: "life",
				description: "adalah jumlah periode saat nilai aset didepresiasikan (kadang-kadang disebut nilai jual aset)"
			},
			{
				name: "period",
				description: "adalah periode yang ingin Anda hitung depresiasinya. Periode harus menggunakan unit yang sama dengan Life"
			},
			{
				name: "factor",
				description: "adalah laju penurunan saldo. Jika Faktor diabaikan, diasumsikan menjadi 2 (metode saldo pengurangan-ganda)"
			}
		]
	},
	{
		name: "DEC2BIN",
		description: "Mengonversi sebuah bilangan desimal ke biner.",
		arguments: [
			{
				name: "number",
				description: "adalah bilangan bulat desimal yang ingin Anda konversikan"
			},
			{
				name: "places",
				description: "adalah jumlah karakter untuk digunakan"
			}
		]
	},
	{
		name: "DEC2HEX",
		description: "Mengonversi sebuah bilangan desimal ke hexadesimal.",
		arguments: [
			{
				name: "number",
				description: "adalah bilangan bulat desimal yang ingin Anda konversi"
			},
			{
				name: "places",
				description: "adalah jumlah karakter untuk digunakan"
			}
		]
	},
	{
		name: "DEC2OCT",
		description: "Mengonversi sebuah bilangan desimal ke oktal.",
		arguments: [
			{
				name: "number",
				description: "adalah bilangan bulat desimal yang ingin Anda konversi"
			},
			{
				name: "places",
				description: "adalah jumlah karakter untuk digunakan"
			}
		]
	},
	{
		name: "DECIMAL",
		description: "Mengonversi representasi teks suatu angka dalam dasar tertentu ke angka desimal.",
		arguments: [
			{
				name: "number",
				description: "adalah angka yang ingin Anda konversi"
			},
			{
				name: "radix",
				description: "adalah Radiks dasar angka yang Anda konversi"
			}
		]
	},
	{
		name: "DEGREES",
		description: "Mengonversi radian ke derajat.",
		arguments: [
			{
				name: "angle",
				description: "adalah sudut dalam radian yang ingin Anda ubah"
			}
		]
	},
	{
		name: "DELTA",
		description: "Menguji apakah dua bilangan sama.",
		arguments: [
			{
				name: "number1",
				description: "adalah bilangan pertama"
			},
			{
				name: "number2",
				description: "adalah bilangan kedua"
			}
		]
	},
	{
		name: "DEVSQ",
		description: "Menampilkan penjumlahan kuadrat simpangan poin data dari nilai rata-rata contoh poin.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "adalah argumen, atau array atau referensi array 1 sampai 255, di mana Anda ingin DEVSQ menghitungnya"
			},
			{
				name: "number2",
				description: "adalah argumen, atau array atau referensi array 1 sampai 255, di mana Anda ingin DEVSQ menghitungnya"
			}
		]
	},
	{
		name: "DGET",
		description: "Mengekstrak dari database rekaman tunggal yang cocok dengan kondisi yang Anda tentukan.",
		arguments: [
			{
				name: "database",
				description: "adalah rentang sel yang membuat daftar atau database. Database adalah daftar dari data yang terkait"
			},
			{
				name: "field",
				description: "adalah label kolom dalam tanda kutip ganda atau angka yang menunjukkan posisi kolom dalam daftar"
			},
			{
				name: "criteria",
				description: "adalah rentang sel yang mengandung kondisi yang Anda tentukan. Rentang menyertakan label kolom dan satu sel di bawah label sebagai kondisi"
			}
		]
	},
	{
		name: "DISC",
		description: "Menampilkan tingkat potongan untuk sekuritas.",
		arguments: [
			{
				name: "settlement",
				description: "adalah tanggal pelunasan sekuritas, yang dinyatakan sebagai angka tanggal seri"
			},
			{
				name: "maturity",
				description: "adalah tanggal jatuh tempo, yang dinyatakan sebagai angka tanggal seri"
			},
			{
				name: "pr",
				description: "adalah harga sekuritas $100 per nilai awal"
			},
			{
				name: "redemption",
				description: "adalah nilai tebusan sekuritas per $100 per nilai nominal"
			},
			{
				name: "basis",
				description: "adalah jenis basis penghitungan hari untuk digunakan"
			}
		]
	},
	{
		name: "DMAX",
		description: "Menampilkan angka terbesar dalam bidang (kolom) dari catatan dalam database yang sesuai dengan kondisi yang Anda tentukan.",
		arguments: [
			{
				name: "database",
				description: "adalah rentang dari sel yang membuat daftar atau database. Database adalah daftar dari data terkait"
			},
			{
				name: "field",
				description: "adalah label kolom dalam tanda kutip ganda atau angka yang menunjukkan posisi kolom dalam daftar"
			},
			{
				name: "criteria",
				description: "adalah rentang sel yang mengandung kondisi yang Anda tentukan. Rentang menyertakan label kolom dan satu sel di bawah label sebagai kondisi"
			}
		]
	},
	{
		name: "DMIN",
		description: "Menampilkan angka terkecil dalam bidang (kolom) dari catatan dalam database yang sesuai dengan kondisi yang Anda tentukan.",
		arguments: [
			{
				name: "database",
				description: "adalah rentang dari sel yang membuat daftar atau database. Database adalah daftar dari data terkait"
			},
			{
				name: "field",
				description: "adalah label kolom dalam tanda kutip ganda atau angka yang menunjukkan posisi kolom dalam daftar"
			},
			{
				name: "criteria",
				description: "adalah rentang sel yang mengandung kondisi yang Anda tentukan. Rentang menyertakan label kolom dan satu sel di bawah label sebagai kondisi"
			}
		]
	},
	{
		name: "DOLLAR",
		description: "Mengonversi angka ke teks, menggunakan format mata uang.",
		arguments: [
			{
				name: "number",
				description: "adalah angka, referensi ke sel yang mengandung angka, atau rumus yang mengevaluasi ke angka"
			},
			{
				name: "decimals",
				description: "adalah jumlah digit di sebelah kanan titik desimal. Angka dibulatkan jika perlu; jika dihilangkan, Desimal = 2"
			}
		]
	},
	{
		name: "DOLLARDE",
		description: "Mengubah harga dolar, yang dinyatakan sebagai pecahan, ke dalam harga dolar, yang dinyatakan sebagai angka desimal.",
		arguments: [
			{
				name: "fractional_dollar",
				description: "adalah yang dinyatakan sebagai pecahan"
			},
			{
				name: "fraction",
				description: "adalah bilangan bulat untuk digunakan dalam penyebut pecahan"
			}
		]
	},
	{
		name: "DOLLARFR",
		description: "Mengubah harga dolar, yang dinyatakan sebagai angka desimal, ke dalam harga dolar, yang dinyatakan sebagai pecahan.",
		arguments: [
			{
				name: "decimal_dollar",
				description: "adalah angka desimal"
			},
			{
				name: "fraction",
				description: "adalah bilangan bulat untuk digunakan dalam penyebut pecahan"
			}
		]
	},
	{
		name: "DPRODUCT",
		description: "Mengalikan nilai dalam bidang (kolom) dari rekaman dalam database yang cocok dengan kondisi yang Anda tentukan.",
		arguments: [
			{
				name: "database",
				description: "adalah rentang sel yang membuat daftar atau database. Database adalah daftar dari data yang berhubungan"
			},
			{
				name: "field",
				description: "adalah label kolom dalam tanda petik ganda atau angka yang menunjukkan posisi kolom dalam daftar"
			},
			{
				name: "criteria",
				description: "adalah rentang sel yang mengandung kondisi yang Anda tentukan. Rentang menyertakan label kolom dan satu sel di bawah label sebagai kondisi"
			}
		]
	},
	{
		name: "DSTDEV",
		description: "Memperkirakan standar deviasi berdasarkan pada contoh dari entri database terpilih.",
		arguments: [
			{
				name: "database",
				description: "adalah rentang dari sel yang membuat daftar atau database. Database adalah daftar dari data terkait"
			},
			{
				name: "field",
				description: "adalah label kolom dalam tanda kutip ganda atau angka yang menunjukkan posisi kolom dalam daftar"
			},
			{
				name: "criteria",
				description: "adalah rentang sel yang mengandung kondisi yang Anda tentukan. Rentang menyertakan label kolom dan satu sel di bawah label sebagai kondisi"
			}
		]
	},
	{
		name: "DSTDEVP",
		description: "Menghitung simpangan baku berdasarkan seluruh populasi entri database terpilih.",
		arguments: [
			{
				name: "database",
				description: "adalah rentang sel yang membuat daftar atau database. Database adalah daftar dari data yang terkait"
			},
			{
				name: "field",
				description: "adalah label kolom dalam tanda kutip ganda atau angka yang menunjukkan posisi kolom dalam daftar"
			},
			{
				name: "criteria",
				description: "adalah rentang sel yang mengandung kondisi yang Anda tentukan. Rentang menyertakan label kolom dan satu sel di bawah label sebagai kondisi"
			}
		]
	},
	{
		name: "DSUM",
		description: "Menambah angka dalam bidang (kolom) dari rekaman dalam database yang sesuai dengan kondisi yang Anda tentukan.",
		arguments: [
			{
				name: "database",
				description: "adalah rentang sel yang membuat daftar atau database. Database adalah daftar dari data terkait"
			},
			{
				name: "field",
				description: "adalah label kolom dalam tanda kutip ganda atau angka yang menunjukkan posisi kolom dalam daftar"
			},
			{
				name: "criteria",
				description: "adalah rentang sel yang mengandung kondisi yang Anda tentukan. Rentang menyertakan label kolom dan satu sel di bawah label sebagai kondisi"
			}
		]
	},
	{
		name: "DVAR",
		description: "Memperkirakan variansi berdasarkan contoh dari entri database terpilih.",
		arguments: [
			{
				name: "database",
				description: "adalah rentang dari sel yang membuat daftar atau database. Database adalah daftar dari data terkait"
			},
			{
				name: "field",
				description: "adalah label kolom dalam tanda kutip ganda atau angka yang menunjukkan posisi kolom dalam daftar"
			},
			{
				name: "criteria",
				description: "adalah rentang sel yang mengandung kondisi yang Anda tentukan. Rentang menyertakan label kolom dan satu sel di bawah label sebagai kondisi"
			}
		]
	},
	{
		name: "DVARP",
		description: "Menghitung variansi berdasarkan pada seluruh populasi entri database terpilih.",
		arguments: [
			{
				name: "database",
				description: "adalah rentang sel yang membuat daftar atau database. Database adalah daftar dari data terkait"
			},
			{
				name: "field",
				description: "adalah label kolom dalam tanda kutip ganda atau angka yang menunjukkan posisi kolom dalam daftar"
			},
			{
				name: "criteria",
				description: "adalah rentang sel yang mengandung kondisi yang Anda tentukan. Rentang menyertakan label kolom dan satu sel di bawah label sebagai kondisi"
			}
		]
	},
	{
		name: "EDATE",
		description: "Menampilkan angka seri tanggal yang menunjukkan jumlah bulan sebelum atau sesudah tanggal mulai.",
		arguments: [
			{
				name: "start_date",
				description: "adalah angka tanggal seri yang mewakili tanggal mulai"
			},
			{
				name: "months",
				description: "adalah jumlah bulan sebelum atau setelah tanggal_mulai"
			}
		]
	},
	{
		name: "EFFECT",
		description: "Menampilkan suku bunga tahunan yang berlaku.",
		arguments: [
			{
				name: "nominal_rate",
				description: "adalah suku bunga nominal"
			},
			{
				name: "npery",
				description: "adalah jumlah periode pemajemukan per tahun"
			}
		]
	},
	{
		name: "ENCODEURL",
		description: "Menampilkan string berkode-URL.",
		arguments: [
			{
				name: "text",
				description: "adalah string yang dikodekan dengan URL"
			}
		]
	},
	{
		name: "EOMONTH",
		description: "Menampilkan angka seri hari terakhir dari bulan sebelum atau setelah jumlah bulan yang ditentukan.",
		arguments: [
			{
				name: "start_date",
				description: "adalah angka tanggal seri yang mewakili tanggal mulai"
			},
			{
				name: "months",
				description: "adalah jumlah bulan sebelum atau setelah tanggal_mulai"
			}
		]
	},
	{
		name: "ERF",
		description: "Mengembalikan fungsi kesalahan.",
		arguments: [
			{
				name: "lower_limit",
				description: "adalah batas terbawah untuk mengintegrasikan ERF"
			},
			{
				name: "upper_limit",
				description: "adalah batas teratas untuk mengintegrasikan ERF"
			}
		]
	},
	{
		name: "ERF.PRECISE",
		description: "Menghasilkan fungsi kesalahan.",
		arguments: [
			{
				name: "X",
				description: "adalah batas bawah untuk mengintegrasikan ERF.PRECISE"
			}
		]
	},
	{
		name: "ERFC",
		description: "Mengembalikan kesalahan tambahan fungsi.",
		arguments: [
			{
				name: "x",
				description: "adalah batas terbawah untuk mengintegrasikan ERF"
			}
		]
	},
	{
		name: "ERFC.PRECISE",
		description: "Menghasilkan fungsi kesalahan pelengkap.",
		arguments: [
			{
				name: "X",
				description: "adalah batas bawah untuk mengintegrasikan ERFC.PRECISE"
			}
		]
	},
	{
		name: "ERROR.TYPE",
		description: "Mengembalikan nilai yang sesuai dengan nilai kesalahan.",
		arguments: [
			{
				name: "error_val",
				description: "adalah nilai kesalahan dimana Anda ingin mengidentifikasi angka, dan dapat berupa nilai kesalahan aktual atau referensi ke sel yang mengandung nilai kesalahan"
			}
		]
	},
	{
		name: "EVEN",
		description: "Membulatkan angka positif ke atas dan angka negatif ke bawah ke bilangan bulat genap terdekat.",
		arguments: [
			{
				name: "number",
				description: "adalah nilai untuk dibulatkan"
			}
		]
	},
	{
		name: "EXACT",
		description: "Mengecek apakah kedua string teks tersebut tepat sama, dan mengembalikan BENAR atau SALAH. EXACT membedakan huruf besar dan kecil.",
		arguments: [
			{
				name: "text1",
				description: "adalah string teks pertama"
			},
			{
				name: "text2",
				description: "adalah string teks kedua"
			}
		]
	},
	{
		name: "EXP",
		description: "Menampilkan ke e pangkat dari angka yang ditentukan.",
		arguments: [
			{
				name: "number",
				description: "adalah eksponen yang diterapkan ke basis e. Konstanta e sama dengan 2,71828182845904, basis dari logaritma alami"
			}
		]
	},
	{
		name: "EXPON.DIST",
		description: "Menampilkan distribusi eksponensial.",
		arguments: [
			{
				name: "x",
				description: "adalah nilai fungsi, yaitu angka bukan negatif"
			},
			{
				name: "lambda",
				description: "adalah nilai parameter, yaitu angka positif"
			},
			{
				name: "cumulative",
				description: "adalah nilai logika untuk fungsi yang muncul: fungsi distribusi kumulatif = BENAR; fungsi kerapatan probabilitas = SALAH"
			}
		]
	},
	{
		name: "EXPONDIST",
		description: "Menampilkan distribusi eksponensial.",
		arguments: [
			{
				name: "x",
				description: "adalah nilai fungsi, yaitu angka bukan negatif"
			},
			{
				name: "lambda",
				description: "adalah nilai parameter, yaitu angka positif"
			},
			{
				name: "cumulative",
				description: "adalah nilai logis bagi fungsi untuk dikembalikan: fungsi distribusi kumulatif = BENAR; fungsi kerapatan probabilitas = SALAH"
			}
		]
	},
	{
		name: "F.DIST",
		description: "Menghasilkan distribusi probabilitas F (lemparan-kiri) untuk dua set data.",
		arguments: [
			{
				name: "x",
				description: "adalah nilai untuk mengevaluasi fungsi, yaitu bilangan non-negatif"
			},
			{
				name: "deg_freedom1",
				description: "adalah pangkat pembilang, yaitu angka di antara 1 dan 10^10, tidak termasuk 10^10"
			},
			{
				name: "deg_freedom2",
				description: "adalah pangkat angka penyebut, yaitu angka di antara 1 dan 10^10, tidak termasuk 10^10"
			},
			{
				name: "cumulative",
				description: "adalah nilai logis untuk fungsi yang muncul: fungsi distribusi kumulatif = TRUE; fungsi kerapatan probabilitas = FALSE"
			}
		]
	},
	{
		name: "F.DIST.RT",
		description: "Menghasilkan distribusi probabilitas F (lemparan-kanan) untuk dua set data.",
		arguments: [
			{
				name: "x",
				description: "adalah nilai untuk mengevaluasi fungsi, yaitu bilangan non-negatif"
			},
			{
				name: "deg_freedom1",
				description: "adalah pangkat pembilang, yaitu angka di antara 1 dan 10^10, tidak termasuk 10^10"
			},
			{
				name: "deg_freedom2",
				description: "adalah pangkat angka penyebut, yaitu angka di antara 1 dan 10^10, tidak termasuk 10^10"
			}
		]
	},
	{
		name: "F.INV",
		description: "Menghasilkan balikan distribusi probabilitas F (lemparan-kiri): jika p = F.DIST(x,...), maka F.INV(p,...) = x.",
		arguments: [
			{
				name: "probability",
				description: "adalah probabilitas yang terkait dengan distribusi kumulatif F, angka di antara 0 sampai dengan 1"
			},
			{
				name: "deg_freedom1",
				description: "adalah pangkat pembilang, yaitu angka di antara 1 dan 10^10, tidak termasuk 10^10"
			},
			{
				name: "deg_freedom2",
				description: "adalah pangkat angka penyebut, yaitu angka di antara 1 dan 10^10, tidak termasuk 10^10"
			}
		]
	},
	{
		name: "F.INV.RT",
		description: "Menghasilkan balikan distribusi probabilitas F (lemparan-kanan): jika p = F.DIST(x,...), maka F.INV.RT(p,...) = x.",
		arguments: [
			{
				name: "probability",
				description: "adalah probabilitas yang terkait dengan distribusi kumulatif F, angka di antara 0 sampai dengan 1"
			},
			{
				name: "deg_freedom1",
				description: "adalah pangkat pembilang, yaitu angka di antara 1 dan 10^10, tidak termasuk 10^10"
			},
			{
				name: "deg_freedom2",
				description: "adalah pangkat angka penyebut, yaitu angka di antara 1 dan 10^10, tidak termasuk 10^10"
			}
		]
	},
	{
		name: "F.TEST",
		description: "Menampilkan hasil tes-F, probabilitas dua lemparan di mana variansi dalam Array1 dan Array2 tidak memiliki perbedaan yang berarti.",
		arguments: [
			{
				name: "array1",
				description: "adalah array atau rentang data pertama dan dapat berupa angka atau nama, array, atau referensi yang mengandung angka (spasi diabaikan)"
			},
			{
				name: "array2",
				description: "adalah array atau rentang data kedua dan dapat berupa angka atau nama, array, atau referensi yang mengandung angka (spasi diabaikan)"
			}
		]
	},
	{
		name: "FACT",
		description: "Menampilkan faktorial dari angka, sama dengan 1*2*3*...* Angka.",
		arguments: [
			{
				name: "number",
				description: "adalah angka yang bukan negatif yang ingin Anda faktorialkan"
			}
		]
	},
	{
		name: "FACTDOUBLE",
		description: "Menampilkan faktorial ganda dari bilangan.",
		arguments: [
			{
				name: "number",
				description: "adalah nilai untuk mengembalikan faktorial ganda"
			}
		]
	},
	{
		name: "FALSE",
		description: "Menampilkan nilai logika SALAH.",
		arguments: [
		]
	},
	{
		name: "FDIST",
		description: "Menampilkan distribusi probabilitas F (tingkat perbedaan) untuk dua perangkat data.",
		arguments: [
			{
				name: "x",
				description: "adalah nilai untuk mengevaluasi fungsi, yaitu angka bukan negatif"
			},
			{
				name: "deg_freedom1",
				description: "adalah pangkat pembilang, yaitu angka di antara 1 dan 10^10, mengecualikan 10^10"
			},
			{
				name: "deg_freedom2",
				description: "adalah pangkat angka penyebut, yaitu angka di antara 1 dan 10^10, mengecualikan 10^10"
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
		description: "Menampilkan posisi awal satu string teks dalam string teks lain. FIND membedakan huruf besar dan kecil.",
		arguments: [
			{
				name: "find_text",
				description: "adalah teks yang ingin Anda temukan. Gunakan tanda kutip ganda (teks kosong) untuk mencocokkan karakter pertama pada Within_text; karakter bebas tidak diizinkan"
			},
			{
				name: "within_text",
				description: "adalah teks yang mengandung teks yang ingin Anda temukan"
			},
			{
				name: "start_num",
				description: "menentukan karakter yang mana untuk memulai pencarian. Karakter pertama pada Dalam_teks adalah karakter angka 1. Jika dihilangkan, nomor_Mulai = 1"
			}
		]
	},
	{
		name: "FINV",
		description: "Menampilkan inversi distribusi probabilitas F: jika p = FDIST(x,...), maka FINV(p,...) = x.",
		arguments: [
			{
				name: "probability",
				description: "adalah probabilitas yang berhubungan dengan distribusi kumulatif F, angka di antara 0 sampai dengan 1"
			},
			{
				name: "deg_freedom1",
				description: "adalah pangkat pembilang, yaitu angka di antara 1 dan 10^10, mengecualikan 10^10"
			},
			{
				name: "deg_freedom2",
				description: "adalah pangkat angka penyebut, yaitu angka di antara 1 dan 10^10, mengecualikan 10^10"
			}
		]
	},
	{
		name: "FISHER",
		description: "Menampilkan transformasi Fisher.",
		arguments: [
			{
				name: "x",
				description: "adalah angka di mana Anda menginginkan transformasi, yaitu angka di antara -1 dan 1, mengecualikan -1 dan 1"
			}
		]
	},
	{
		name: "FISHERINV",
		description: "Menampilkan invers dari transformasi Fisher: Jika y = FISHER(x), maka FISHERINV(y) = x.",
		arguments: [
			{
				name: "y",
				description: "adalah nilai di mana Anda ingin melakukan invers dari transfomasinya"
			}
		]
	},
	{
		name: "FIXED",
		description: "Membulatkan angka ke angka desimal tertentu dan mengembalikan hasilnya sebagai teks dengan atau tanpa koma.",
		arguments: [
			{
				name: "number",
				description: "adalah angka yang ingin Anda bulatkan dan ubah ke teks"
			},
			{
				name: "decimals",
				description: "adalah jumlah digit di sebelah kanan titik desimal. Jika dihilangkan, Desimal = 2"
			},
			{
				name: "no_commas",
				description: "adalah nilai logis: jangan tampilkan koma dalam teks yang dikembalikan = BENAR; tampilkan koma dalam teks yang dikembalikan = SALAH atau hilangkan"
			}
		]
	},
	{
		name: "FLOOR",
		description: "Membulatkan angka ke bawah, mendekati multipel terdekat dari signifikansi.",
		arguments: [
			{
				name: "number",
				description: "adalah nilai angka yang ingin Anda bulatkan"
			},
			{
				name: "significance",
				description: "adalah multipel yang ingin Anda bulatkan. Angka dan Signifikansi harus keduanya positif atau keduanya negatif"
			}
		]
	},
	{
		name: "FLOOR.MATH",
		description: "Membulatkan angka ke atas, ke bilangan bulat terdekat atau ke kelipatan signifikansi terdekat.",
		arguments: [
			{
				name: "number",
				description: "adalah nilai yang ingin Anda bulatkan"
			},
			{
				name: "significance",
				description: "adalah kelipatan opsional yang ingin Anda bulatkan"
			},
			{
				name: "mode",
				description: "jika diberikan dan selain nol, fungsi ini akan membulatkan ke nol"
			}
		]
	},
	{
		name: "FLOOR.PRECISE",
		description: "Membulatkan angka ke bawah, ke bilangan bulat terdekat atau ke kelipatan signifikansi terdekat.",
		arguments: [
			{
				name: "number",
				description: "adalah nilai yang ingin Anda bulatkan"
			},
			{
				name: "significance",
				description: "adalah kelipatan opsional yang ingin Anda bulatkan. "
			}
		]
	},
	{
		name: "FORECAST",
		description: "Menghitung, atau memprediksi, nilai masa depan mendekati tren linear menggunakan nilai yang ada.",
		arguments: [
			{
				name: "x",
				description: "adalah poin data di mana Anda ingin memprediksi nilai dan harus berupa nilai numerik"
			},
			{
				name: "known_y's",
				description: "adalah array atau rentang data numerik dependen"
			},
			{
				name: "known_x's",
				description: "adalah array atau rentang data numerik independen. Variansi dari x_yang Dikenal tidak boleh nol"
			}
		]
	},
	{
		name: "FORMULATEXT",
		description: "Menghasilkan rumus sebagai string.",
		arguments: [
			{
				name: "reference",
				description: "adalah referensi ke rumus"
			}
		]
	},
	{
		name: "FREQUENCY",
		description: "Menghitung seberapa sering nilai terdapat dalam rentang nilai kemudian kembalikan array vertikal dari angka yang memiliki lebih dari satu elemen dari array_Bins.",
		arguments: [
			{
				name: "data_array",
				description: "adalah array dari atau referensi ke perangkat nilai yang ingin Anda hitung frekuensinya (spasi dan teks diabaikan)"
			},
			{
				name: "bins_array",
				description: "adalah array dari atau referensi ke interval yang menjadi dasar pengelompokan nilai dalam array_data"
			}
		]
	},
	{
		name: "FTEST",
		description: "Menampilkan hasil uji-F, probabilitas dua lemparan di mana varian dalam Barisan1 dan Barisan2 tidak memiliki perbedaan yang berarti.",
		arguments: [
			{
				name: "array1",
				description: "adalah barisan atau rentang data pertama dan dapat berupa angka atau nama, barisan, atau referensi yang mengandung angka (spasi diabaikan)"
			},
			{
				name: "array2",
				description: "adalah barisan atau rentang data kedua dan dapat berupa angka atau nama, array, atau referensi yang mengandung angka (spasi diabaikan)"
			}
		]
	},
	{
		name: "FV",
		description: "Menampilkan nilai masa depan investasi secara periodik, konstanta pembayaran dan konstanta suku bunga.",
		arguments: [
			{
				name: "rate",
				description: "adalah suku bunga tiap periode. Contoh, gunakan 6%/4 untuk pembayaran per kuartal pada APR 6%"
			},
			{
				name: "nper",
				description: "adalah total jumlah periode pembayaran dalam investasi"
			},
			{
				name: "pmt",
				description: "adalah pembayaran yang dilakukan tiap periode dan tidak dapat mengubah jangka waktu investasi"
			},
			{
				name: "pv",
				description: "adalah nilai sekarang, atau jumlah keseluruhan yang dibayar sekaligus di mana seri pembayaran masa depan bermanfaat sekarang. Jika dihilangkan, Pv = 0"
			},
			{
				name: "type",
				description: "adalah nilai yang menunjukkan pemilihan waktu pembayaran: pembayaran di awal periode = 1; pembayaran di akhir periode = 0 atau dihilangkan"
			}
		]
	},
	{
		name: "FVSCHEDULE",
		description: "Menampilkan nilai akan datang dari nilai pokok awal sesudah menerapkan seri suku bunga majemuk.",
		arguments: [
			{
				name: "principal",
				description: "adalah nilai saat ini"
			},
			{
				name: "schedule",
				description: "adalah array dari tingkat bunga untuk diterapkan"
			}
		]
	},
	{
		name: "GAMMA",
		description: "Menghasilkan nilai fungsi Gamma.",
		arguments: [
			{
				name: "x",
				description: "adalah nilai di mana Anda ingin menghitung Gamma"
			}
		]
	},
	{
		name: "GAMMA.DIST",
		description: "Menghasilkan distribusi gamma.",
		arguments: [
			{
				name: "x",
				description: "adalah nilai yang ingin Anda gunakan untuk mengevaluasi distribusi, yaitu bilangan non-negatif"
			},
			{
				name: "alpha",
				description: "adalah parameter untuk distribusi, yaitu bilangan positif"
			},
			{
				name: "beta",
				description: "adalah parameter untuk distribusi, yaitu bilangan positif. Jika beta = 1, GAMMADIST menghasilkan distribusi gamma standar"
			},
			{
				name: "cumulative",
				description: "adalah nilai logis: menghasilkan fungsi distribusi kumulatif = TRUE; menghasilkan fungsi massa probabilitas = FALSE atau dihilangkan"
			}
		]
	},
	{
		name: "GAMMA.INV",
		description: "Menghasilkan balikan distribusi kumulatif gamma: jika p = GAMMADIST(x,...), maka GAMMAINV(p,...) = x.",
		arguments: [
			{
				name: "probability",
				description: "adalah probabilitas yang terkait dengan distribusi gamma, yaitu bilangan di antara 0 dan 1"
			},
			{
				name: "alpha",
				description: "adalah parameter untuk distribusi, yaitu bilangan positif"
			},
			{
				name: "beta",
				description: "adalah parameter untuk distribusi, yaitu bilangan positif. Jika beta = 1, GAMMAINV menghasilkan balikan distribusi gamma standar"
			}
		]
	},
	{
		name: "GAMMADIST",
		description: "Menampilkan distribusi gamma.",
		arguments: [
			{
				name: "x",
				description: "adalah nilai yang ingin Anda gunakan untuk mengevaluasi distribusi, yaitu angka bukan negatif"
			},
			{
				name: "alpha",
				description: "adalah parameter untuk distribusi, yaitu angka positif"
			},
			{
				name: "beta",
				description: "adalah parameter untuk distribusi, yaitu angka positif. Jika beta = 1, GAMMADIST mengembalikan distribusi gamma standar"
			},
			{
				name: "cumulative",
				description: "adalah nilai logis: mengembalikan fungsi distribusi kumulatif = BENAR; mengembalikan fungsi massa probabilitas = SALAH atau hilangkan"
			}
		]
	},
	{
		name: "GAMMAINV",
		description: "Menampilkan inversi distribusi kumulatif gamma: jika p = GAMMADIST(x,...), maka GAMMAINV(p,...) = x.",
		arguments: [
			{
				name: "probability",
				description: "adalah probabilitas yang berhubungan dengan distribusi gamma, yaitu angka di antara 0 sampai dengan 1"
			},
			{
				name: "alpha",
				description: "adalah parameter untuk distribusi, yaitu angka positif"
			},
			{
				name: "beta",
				description: "adalah parameter untuk distribusi, yaitu angka positif. Jika beta = 1, GAMMAINV mengembalikan inversi distribusi gamma standar"
			}
		]
	},
	{
		name: "GAMMALN",
		description: "Menampilkan logaritma alami fungsi gamma.",
		arguments: [
			{
				name: "x",
				description: "adalah nilai di mana Anda ingin menghitung GAMMALN, angka positif"
			}
		]
	},
	{
		name: "GAMMALN.PRECISE",
		description: "Menghasilkan logaritma alami fungsi gamma.",
		arguments: [
			{
				name: "x",
				description: "adalah nilai di mana Anda ingin menghitung GAMMALN.PRECISE, bilangan positif"
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
		description: "Menampilkan pembagi terbesar yang umum.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "adalah 1 sampai 255 nilai"
			},
			{
				name: "number2",
				description: "adalah 1 sampai 255 nilai"
			}
		]
	},
	{
		name: "GEOMEAN",
		description: "Menampilkan nilai rata-rata geometrik array atau rentang data numerik yang positif.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "adalah angka atau nama, array, atau referensi 1 sampai 255 yang mengandung angka di mana Anda inginkan nilai rata-ratanya"
			},
			{
				name: "number2",
				description: "adalah angka atau nama, array, atau referensi 1 sampai 255 yang mengandung angka di mana Anda inginkan nilai rata-ratanya"
			}
		]
	},
	{
		name: "GESTEP",
		description: "Menguji apakah bilangan lebih besar dari nilai ambang.",
		arguments: [
			{
				name: "number",
				description: "adalah nilai untuk menguji langkah"
			},
			{
				name: "step",
				description: "adalah nilai ambang"
			}
		]
	},
	{
		name: "GETPIVOTDATA",
		description: "Mengekstrak data yang tersimpan dalam sebuah PivotTable.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "data_field",
				description: "adalah nama dari bidang data untuk ekstrak data"
			},
			{
				name: "pivot_table",
				description: "adalah referensi ke sebuah sel atau rentang sel dalam PivotTable yang berisikan data yang ingin Anda ambil"
			},
			{
				name: "field",
				description: "bidang acuan"
			},
			{
				name: "item",
				description: "item bidang untuk dirujuk"
			}
		]
	},
	{
		name: "GROWTH",
		description: "Menampilkan angka dalam pencocokan tren pertambahan eksponensial poin data yang dikenal.",
		arguments: [
			{
				name: "known_y's",
				description: "adalah seperangkat nilai-y yang telah Anda ketahui dalam hubungan y = b*m^x, array atau rentang dari angka positif"
			},
			{
				name: "known_x's",
				description: "adalah seperangkat nilai-x opsional yang mungkin telah Anda ketahui dalam hubungan y = b*m^x, array atau rentang berukuran sama seperti y_yang Dikenal"
			},
			{
				name: "new_x's",
				description: "adalah nilai-x baru yang GROWTH-nya Anda inginkan mengembalikan nilai-y yang berhubungan"
			},
			{
				name: "const",
				description: "adalah nilai logika: konstanta b dihitung secara normal jika Const = BENAR; b ditata sama dengan 1 jika Const = SALAH atau dihilangkan"
			}
		]
	},
	{
		name: "HARMEAN",
		description: "Menampilkan nilai rata-rata harmonik perangkat data dari angka positif: resiprokal dari nilai rata-rata aritmatika resiprokal.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "adalah angka atau nama, array, atau referensi 1 sampai 255 yang mengandung angka di mana Anda ingin nilai rata-rata harmoniknya"
			},
			{
				name: "number2",
				description: "adalah angka atau nama, array, atau referensi 1 sampai 255 yang mengandung angka di mana Anda ingin nilai rata-rata harmoniknya"
			}
		]
	},
	{
		name: "HEX2BIN",
		description: "Mengonversi sebuah bilangan Hexadesimal ke biner.",
		arguments: [
			{
				name: "number",
				description: "adalah bilangan hexadesimal yang ingin Anda konversi"
			},
			{
				name: "places",
				description: "adalah jumlah karakter untuk digunakan"
			}
		]
	},
	{
		name: "HEX2DEC",
		description: "Mengonversi sebuah bilangan hexadesimal ke desimal.",
		arguments: [
			{
				name: "number",
				description: "adalah bilangan hexadesimal yang ingin Anda konversi"
			}
		]
	},
	{
		name: "HEX2OCT",
		description: "Mengonversi sebuah bilangan hexadesimal ke oktal.",
		arguments: [
			{
				name: "number",
				description: "adalah bilangan hexadesimal yang ingin Anda konversi"
			},
			{
				name: "places",
				description: "adalah jumlah karakter untuk digunakan"
			}
		]
	},
	{
		name: "HLOOKUP",
		description: "Mencari nilai dalam baris teratas dari tabel atau array nilai dan mengembalikan nilai dalam kolom yang sama dari baris yang Anda tentukan.",
		arguments: [
			{
				name: "lookup_value",
				description: "adalah nilai yang ditemukan dalam baris pertama dalam tabel dan dapat berupa nilai, referensi atau string teks"
			},
			{
				name: "table_array",
				description: "adalah tabel teks, angka, atau nilai logis tempat data dicari. Tabel_array dapat direferensikan ke rentang atau nama rentang"
			},
			{
				name: "row_index_num",
				description: "adalah nomor baris dalam tabel_array yang jika nilainya cocok harus dikembalikan. Baris pertama dari nilai dalam tabel adalah baris 1"
			},
			{
				name: "range_lookup",
				description: "adalah nilai logika: untuk menemukan yang paling mendekati kecocokannya pada baris teratas (urutkan dalam urutan naik) = BENAR atau dihilangkan; temukan yang benar-benar cocok = SALAH"
			}
		]
	},
	{
		name: "HOUR",
		description: "Menampilkan jam sebagai angka dari 0 (12:00 A.M.) sampai 23 (11:00 P.M.).",
		arguments: [
			{
				name: "serial_number",
				description: "adalah angka dalam kode tanggal-waktu yang digunakan oleh Spreadsheet, atau teks dalam format waktu, seperti 16:48:00 atau 4:48:00 PM"
			}
		]
	},
	{
		name: "HYPERLINK",
		description: "Membuat pintasan atau loncatan yang akan membuka dokumen yang disimpan dalam hard drive Anda, server jaringan, atau pada Internet.",
		arguments: [
			{
				name: "link_location",
				description: "adalah teks yang memberikan jalur dan nama file ke dokumen untuk membukanya, lokasi hard drive, alamat UNC, atau jalur URL"
			},
			{
				name: "friendly_name",
				description: "adalah teks atau angka yang ditampilkan dalam sel. Jika dihilangkan, sel tersebut menampilkan teks lokasi_Tautan"
			}
		]
	},
	{
		name: "HYPGEOM.DIST",
		description: "Menghasilkan distribusi hipergeometrik.",
		arguments: [
			{
				name: "sample_s",
				description: "adalah jumlah keberhasilan dalam sampel"
			},
			{
				name: "number_sample",
				description: "adalah ukuran sampel"
			},
			{
				name: "population_s",
				description: "adalah jumlah keberhasilan dalam populasi"
			},
			{
				name: "number_pop",
				description: "adalah ukuran populasi"
			},
			{
				name: "cumulative",
				description: "adalah nilai logis: untuk fungsi distribusi kumulatif, gunakan TRUE; untuk fungsi kerapatan probabilitas, gunakan FALSE"
			}
		]
	},
	{
		name: "HYPGEOMDIST",
		description: "Menampilkan distribusi hipergeometrik.",
		arguments: [
			{
				name: "sample_s",
				description: "adalah jumlah keberhasilan dalam contoh"
			},
			{
				name: "number_sample",
				description: "adalah ukuran contoh"
			},
			{
				name: "population_s",
				description: "adalah jumlah keberhasilan dalam populasi"
			},
			{
				name: "number_pop",
				description: "adalah ukuran populasi"
			}
		]
	},
	{
		name: "IF",
		description: "Memeriksa apakah kondisi dipenuhi, dan mengembalikan satu nilai jika BENAR, dan nilai lain jika SALAH.",
		arguments: [
			{
				name: "logical_test",
				description: "adalah nilai atau ekspresi apa saja yang dapat dievalusi ke BENAR atau SALAH"
			},
			{
				name: "value_if_true",
				description: "adalah nilai yang dikembalikan jika tes_Logika adalah BENAR. Jika dihilangkan, BENAR dikembalikan. Anda dapat menyarangkan hingga tujuh fungsi IF"
			},
			{
				name: "value_if_false",
				description: "adalah nilai yang dikembalikan jika tes_Logis adalah SALAH. Jika dihilangkan, SALAH dikembalikan"
			}
		]
	},
	{
		name: "IFERROR",
		description: "Mengembalikan nilai_jika_kesalahan jika ekspresi merupakan kesalahan dan nilai dari ekspresi itu sendiri jika tidak.",
		arguments: [
			{
				name: "value",
				description: "adalah nilai atau ekspresi atau referensi apasaja"
			},
			{
				name: "value_if_error",
				description: "adalah semua nilai atau ekspresi atau referensi"
			}
		]
	},
	{
		name: "IFNA",
		description: "Menghasilkan nilai yang Anda tentukan jika ekspresi terpisah ke #N/A, jika tidak, hasil ekspresi akan diberikan.",
		arguments: [
			{
				name: "value",
				description: "adalah sembarang nilai atau ekspresi atau referensi"
			},
			{
				name: "value_if_na",
				description: "adalah nilai atau ekspresi atau referensi"
			}
		]
	},
	{
		name: "IMABS",
		description: "Menampilkan bilangan absolut (modul) dari bilangan kompleks.",
		arguments: [
			{
				name: "inumber",
				description: "bilangan kompleks yang bilangan absolutnya Anda inginkan"
			}
		]
	},
	{
		name: "IMAGINARY",
		description: "Menampilkan koefisien bayangan dari bilangan kompleks.",
		arguments: [
			{
				name: "inumber",
				description: "adalah bilangan kompleks yang koefisien bayangannya Anda inginkan"
			}
		]
	},
	{
		name: "IMARGUMENT",
		description: "Menampilkan argumen q, sebuah sudut terekspresi dalam radian.",
		arguments: [
			{
				name: "inumber",
				description: "adalah bilangan kompleks yang argumennya Anda inginkan"
			}
		]
	},
	{
		name: "IMCONJUGATE",
		description: "Menampilkan konjugasi kompleks dari bilangan kompleks.",
		arguments: [
			{
				name: "inumber",
				description: "adalah bilangan kompleks yang konjugasinya Anda inginkan"
			}
		]
	},
	{
		name: "IMCOS",
		description: "Menampilkan kosinus dari bilangan kompleks.",
		arguments: [
			{
				name: "inumber",
				description: "adalah bilangan kompleks yang kosinusnya Anda inginkan"
			}
		]
	},
	{
		name: "IMCOSH",
		description: "Menghasilkan kosinus hiperbolik dari bilangan kompleks.",
		arguments: [
			{
				name: "inumber",
				description: "adalah bilangan kompleks yang Anda inginkan kosinus hiperboliknya"
			}
		]
	},
	{
		name: "IMCOT",
		description: "Menghasilkan kotangen dari bilangan kompleks.",
		arguments: [
			{
				name: "inumber",
				description: "adalah bilangan kompleks yang Anda inginkan kotangennya"
			}
		]
	},
	{
		name: "IMCSC",
		description: "Menghasilkan kosekan dari bilangan kompleks.",
		arguments: [
			{
				name: "inumber",
				description: "adalah bilangan kompleks yang Anda inginkan kosekannya"
			}
		]
	},
	{
		name: "IMCSCH",
		description: "Menghasilkan kosekan hiperbolik dari bilangan kompleks.",
		arguments: [
			{
				name: "inumber",
				description: "adalah bilangan kompleks yang Anda inginkan kosekan hiperboliknya"
			}
		]
	},
	{
		name: "IMDIV",
		description: "Menampilkan hasil bagi dari dua bilangan kompleks.",
		arguments: [
			{
				name: "inumber1",
				description: "adalah penghitung atau dividen kompleks"
			},
			{
				name: "inumber2",
				description: "adalah persamaan atau pembagi kompleks"
			}
		]
	},
	{
		name: "IMEXP",
		description: "Menampilkan eksponensial dari bilangan kompleks.",
		arguments: [
			{
				name: "inumber",
				description: "adalah bilangan kompleks yang eksponensialnya Anda inginkan"
			}
		]
	},
	{
		name: "IMLN",
		description: "Menampilkan logaritma alami dari bilangan kompleks.",
		arguments: [
			{
				name: "inumber",
				description: "adalah bilangan kompleks yang logaritma alaminya Anda inginkan"
			}
		]
	},
	{
		name: "IMLOG10",
		description: "Menampilkan logaritma dasar 10 dari bilangan kompleks.",
		arguments: [
			{
				name: "inumber",
				description: "adalah bilangan kompleks yang logaritma umumnya Anda inginkan"
			}
		]
	},
	{
		name: "IMLOG2",
		description: "Menampilkan logaritma dasar 2 dari bilangan kompleks.",
		arguments: [
			{
				name: "inumber",
				description: "adalah bilangan kompleks yang logaritma dasar 2-nya Anda inginkan"
			}
		]
	},
	{
		name: "IMPOWER",
		description: "Menampilkan bilangan kompleks yang dinaikkan ke kekuatan bilangan bulat.",
		arguments: [
			{
				name: "inumber",
				description: "adalah bilangan kompleks yang ingin Anda naikkan hingga ke kekuatan"
			},
			{
				name: "number",
				description: "adalah kekuatan di mana Anda ingin menaikkan bilangan kompleks"
			}
		]
	},
	{
		name: "IMPRODUCT",
		description: "Menampilkan produk dari 1 sampai 255 bilangan kompleks.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "inumber1",
				description: "Inumber1, Inumber2,... adalah dari 1 sampai 255 bilangan kompleks untuk dikalikan."
			},
			{
				name: "inumber2",
				description: "Inumber1, Inumber2,... adalah dari 1 sampai 255 bilangan kompleks untuk dikalikan."
			}
		]
	},
	{
		name: "IMREAL",
		description: "Menampilkan koefisien nyata dari bilangan kompleks.",
		arguments: [
			{
				name: "inumber",
				description: "adalah bilangan kompleks yang koefisien nyatanya Anda inginkan"
			}
		]
	},
	{
		name: "IMSEC",
		description: "Menghasilkan sekan dari bilangan kompleks.",
		arguments: [
			{
				name: "inumber",
				description: "adalah bilangan kompleks yang Anda inginkan sekannya"
			}
		]
	},
	{
		name: "IMSECH",
		description: "Menghasilkan sekan hiperbolik dari bilangan kompleks.",
		arguments: [
			{
				name: "inumber",
				description: "adalah bilangan kompleks yang Anda inginkan sekan hiperboliknya"
			}
		]
	},
	{
		name: "IMSIN",
		description: "Menampilkan sinus dari bilangan kompleks.",
		arguments: [
			{
				name: "inumber",
				description: "adalah bilangan kompleks yang sinusnya Anda inginkan"
			}
		]
	},
	{
		name: "IMSINH",
		description: "Menghasilkan sinus hiperbolik dari bilangan kompleks.",
		arguments: [
			{
				name: "inumber",
				description: "adalah bilangan kompleks yang Anda inginkan sinus hiperboliknya"
			}
		]
	},
	{
		name: "IMSQRT",
		description: "Menampilkan akar pangkat dua dari bilangan kompleks.",
		arguments: [
			{
				name: "inumber",
				description: "adalah bilangan kompleks yang akar pangkat duanya Anda inginkan"
			}
		]
	},
	{
		name: "IMSUB",
		description: "Menampilkan perbedaan dari dua bilangan kompleks.",
		arguments: [
			{
				name: "inumber1",
				description: "adalah bilangan kompleks untuk mengurangi inumber2"
			},
			{
				name: "inumber2",
				description: "adalah bilangan kompleks untuk mengurangi dari inumber1"
			}
		]
	},
	{
		name: "IMSUM",
		description: "Menampilkan penjumlahan bilangan kompleks.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "inumber1",
				description: "dari 1 sampai 255 bilangan kompleks untuk ditambahkan"
			},
			{
				name: "inumber2",
				description: "dari 1 sampai 255 bilangan kompleks untuk ditambahkan"
			}
		]
	},
	{
		name: "IMTAN",
		description: "Menghasilkan tangen dari bilangan kompleks.",
		arguments: [
			{
				name: "inumber",
				description: "adalah bilangan kompleks yang Anda inginkan tangennya"
			}
		]
	},
	{
		name: "INDEX",
		description: "Menampilkan nilai atau referensi sel pada persimpangan baris atau kolom tertentu, dalam rentang yang ditentukan.",
		arguments: [
			{
				name: "array",
				description: "adalah rentang sel atau konstanta array."
			},
			{
				name: "row_num",
				description: "pilih baris dalam Array atau Referensi yang dari sini sebuah nilai dikembalikan. Jika dihilangkan, nomor_Kolom dibutuhkan"
			},
			{
				name: "column_num",
				description: "pilih kolom dalam Array atau Referensi yang dari sini sebuah nilai dikembalikan. Jika dihilangkan, nomor_Baris dibutuhkan"
			}
		]
	},
	{
		name: "INDIRECT",
		description: "Menampilkan referensi yang ditentukan dengan string teks.",
		arguments: [
			{
				name: "ref_text",
				description: "adalah referensi ke sel yang mengandung gaya referensi- A1 atau -R1C1, nama yang ditentukan sebagai referensi, atau referensi ke sel sebagai string teks"
			},
			{
				name: "a1",
				description: "adalah nilai logis yang menentukan tipe referensi dalam teks_Ref: gaya-R1C1 = SALAH; gaya-A1 = BENAR atau dihilangkan"
			}
		]
	},
	{
		name: "INFO",
		description: "Menampilkan informasi tentang lingkungan operasi sekarang.",
		arguments: [
			{
				name: "type_text",
				description: "adalah teks yang menentukan tipe informasi apa yang ingin Anda kembalikan."
			}
		]
	},
	{
		name: "INT",
		description: "Membulatkan angka ke bawah ke bilangan bulat terdekat.",
		arguments: [
			{
				name: "number",
				description: "adalah angka riil yang ingin Anda bulatkan ke bawah ke bilangan bulat"
			}
		]
	},
	{
		name: "INTERCEPT",
		description: "Menghitung titik di mana garis akan memotong sumbu-y menggunakan garis regresi yang paling cocok yang digambarkan melalui nilai-x dan nilai-y yang diketahui.",
		arguments: [
			{
				name: "known_y's",
				description: "adalah seperangkat observasi atau data dependen dan dapat berupa angka atau nama, array, atau referensi yang mengandung angka"
			},
			{
				name: "known_x's",
				description: "adalah seperangkat observasi atau data independen dan dapat berupa angka atau nama, array, atau referensi yang mengandung angka"
			}
		]
	},
	{
		name: "INTRATE",
		description: "Menampilkan tingkat bunga untuk sekuritas investasi penuh.",
		arguments: [
			{
				name: "settlement",
				description: "adalah tanggal penetapan sekuritas, yang dinyatakan sebagai angka tanggal seri"
			},
			{
				name: "maturity",
				description: "adalah tanggal jatuh tempo sekuritas, yang dinyatakan sebagai angka tanggal seri"
			},
			{
				name: "investment",
				description: "adalah jumlah terinvestasi dalam sekuritas"
			},
			{
				name: "redemption",
				description: "adalah jumlah yang akan diterima saat tanggal jatuh tempo"
			},
			{
				name: "basis",
				description: "adalah jenis basis penghitungan hari untuk digunakan"
			}
		]
	},
	{
		name: "IPMT",
		description: "Menampilkan pembayaran bunga untuk periode investasi tertentu, berdasarkan pada pembayaran periodik, konstan dan suku bunga konstan.",
		arguments: [
			{
				name: "rate",
				description: "adalah suku bunga tiap periode. Contoh, gunakan 6%/4 untuk pembayaran tiap kuartal pada APR 6%"
			},
			{
				name: "per",
				description: "adalah periode yang ingin Anda hitung bunga dan harus dalam rentang 1 sampai Nper"
			},
			{
				name: "nper",
				description: "adalah total jumlah periode pembayaran dalam investasi"
			},
			{
				name: "pv",
				description: "adalah nilai sekarang, atau jumlah keseluruhan yang dibayarkan sekaligus di mana seri pembayaran masa depan bermanfaat sekarang"
			},
			{
				name: "fv",
				description: "adalah nilai masa depan, atau saldo kas yang ingin Anda capai setelah pembayaran terakhir dilakukan. Jika diabaikan, Fv = 0"
			},
			{
				name: "type",
				description: "adalah nilai logis yang menunjukkan pemilihan waktu pembayaran: pembayaran di akhir periode = 0 atau dihilangkan; pembayaran di awal periode = 1"
			}
		]
	},
	{
		name: "IRR",
		description: "Menampilkan angka internal pengembalian untuk seri aliran kas.",
		arguments: [
			{
				name: "values",
				description: "adalah array atau referensi ke sel yang mengandung angka yang pengembalian angka internalnya ingin Anda hitung"
			},
			{
				name: "guess",
				description: "adalah angka yang Anda tebak mendekati hasil dari IRR; 0,1 (10 persen) jika dihilangkan"
			}
		]
	},
	{
		name: "ISBLANK",
		description: "Mengecek apakah referensi ke sel kosong, dan mengembalikan BENAR atau SALAH.",
		arguments: [
			{
				name: "value",
				description: "adalah sel atau nama yang menunjuk ke sel yang ingin Anda tes"
			}
		]
	},
	{
		name: "ISERR",
		description: "Memeriksa apakah nilainya adalah kesalahan (#VALUE!, #REF!, #DIV/0!, #NUM!, #NAME?, atau #NULL!) mengeluarkan #N/A, dan mengembalikan BENAR atau SALAH.",
		arguments: [
			{
				name: "value",
				description: "Adalah nilai yang ingin Anda tes. Nilai dapat menunjuk ke sel, rumus, atau nama yang menunjuk ke sel, rumus, atau nilai"
			}
		]
	},
	{
		name: "ISERROR",
		description: "Memeriksa apakah nilainya adalah kesalahan (#N/A, #VALUE!, #REF!, #DIV/0!, #NUM!, #NAME?, atau #NULL!), dan mengembalikan BENAR atau SALAH.",
		arguments: [
			{
				name: "value",
				description: "adalah nilai yang ingin Anda tes. Nilai dapat menunjuk ke sel, rumus, atau nama yang menunjuk ke sel, rumus, atau nilai"
			}
		]
	},
	{
		name: "ISEVEN",
		description: "Menampilkan BENAR jika bilangan genap.",
		arguments: [
			{
				name: "number",
				description: "adalah nilau untuk diuji"
			}
		]
	},
	{
		name: "ISFORMULA",
		description: "Memeriksa apakah referensi menuju ke sel yang berisi rumus, dan menghasilkan TRUE atau FALSE.",
		arguments: [
			{
				name: "reference",
				description: "adalah referensi ke sel yang ingin Anda uji.  Referensi bisa berupa referensi sel, rumus, atau nama yang mengacu ke sel"
			}
		]
	},
	{
		name: "ISLOGICAL",
		description: "Mengecek apakah nilai adalah nilai logika (BENAR atau SALAH), dan mengembalikan BENAR atau SALAH.",
		arguments: [
			{
				name: "value",
				description: "adalah nilai yang ingin Anda tes. Nilai dapat menunjuk ke sel, rumus, atau nama yang menunjuk ke sel, rumus, atau nilai"
			}
		]
	},
	{
		name: "ISNA",
		description: "Memeriksa apakah nilai adalah #N/A, dan mengembalikan BENAR atau SALAH.",
		arguments: [
			{
				name: "value",
				description: "adalah nilai yang ingin Anda tes. Nilai dapat mengacu pada sel, rumus, atau nama yang mengacu pada sel, rumus atau nilai"
			}
		]
	},
	{
		name: "ISNONTEXT",
		description: "Mengecek apakah nilai bukan teks (sel kosong bukan teks), dan mengembalikan BENAR atau SALAH.",
		arguments: [
			{
				name: "value",
				description: "adalah nilai yang ingin Anda tes: sel; rumus; atau nama yang menunjuk ke sel, rumus, atau nilai"
			}
		]
	},
	{
		name: "ISNUMBER",
		description: "Mengecek apakah nilai adalah sebuah angka, dan mengembalikan BENAR atau SALAH.",
		arguments: [
			{
				name: "value",
				description: "adalah nilai yang ingin Anda tes. Nilai dapat merujuk ke sebuah sel, rumus, atau nama yang merujuk ke sel, rumus atau nilai"
			}
		]
	},
	{
		name: "ISO.CEILING",
		description: "Membulatkan angka ke atas, ke bilangan bulat terdekat atau ke kelipatan signifikansi terdekat.",
		arguments: [
			{
				name: "number",
				description: "adalah nilai yang ingin Anda bulatkan"
			},
			{
				name: "significance",
				description: "adalah kelipatan opsional yang ingin Anda bulatkan"
			}
		]
	},
	{
		name: "ISODD",
		description: "Menampilkan BENAR jika bilangan ganjil.",
		arguments: [
			{
				name: "number",
				description: "adalah nilau untuk diuji"
			}
		]
	},
	{
		name: "ISOWEEKNUM",
		description: "Mengembalikan angka jumlah minggu ISO dari setahun untuk tanggal tertentu.",
		arguments: [
			{
				name: "date",
				description: "adalah kode tanggal-waktu yang digunakan Spreadsheet untuk perhitungan tanggal dan waktu"
			}
		]
	},
	{
		name: "ISPMT",
		description: "Menampilkan bunga dibayar selama periode investasi tertentu.",
		arguments: [
			{
				name: "rate",
				description: "suku bunga tiap periode. Contoh, gunakan 6%/4 untuk pembayaran tiap kuartal pada APR 6%"
			},
			{
				name: "per",
				description: "periode saat Anda ingin mencari bunga"
			},
			{
				name: "nper",
				description: "jumlah periode pembayaran dalam investasi"
			},
			{
				name: "pv",
				description: "jumlah keseluruhan yang dibayar sekaligus agar seri pembayaran masa mendatang dibayar saat ini"
			}
		]
	},
	{
		name: "ISREF",
		description: "Memeriksa apakah nilai adalah referensi, dan mengembalikan BENAR atau SALAH.",
		arguments: [
			{
				name: "value",
				description: "adalah nilai yang ingin Anda tes. Nilai dapat menunjuk ke sel, rumus, atau nama yang menunjuk ke sel, rumus, atau nilai"
			}
		]
	},
	{
		name: "ISTEXT",
		description: "Mengecek apakah nilai adalah teks, dan mengembalikan BENAR atau SALAH.",
		arguments: [
			{
				name: "value",
				description: "adalah nilai yang ingin Anda tes. Nilai dapat merujuk ke sebuah sel, rumus, atau nama yang merujuk ke sel, rumus atau nilai"
			}
		]
	},
	{
		name: "KURT",
		description: "Menampilkan kurtosis dari perangkat data.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "adalah angka atau nama, array, atau referensi  1 sampai 255 yang mengandung angka yang kurtosisnya Anda inginkan"
			},
			{
				name: "number2",
				description: "adalah angka atau nama, array, atau referensi  1 sampai 255 yang mengandung angka yang kurtosisnya Anda inginkan"
			}
		]
	},
	{
		name: "LARGE",
		description: "Menampilkan nilai k-th terbesar dalam perangkat data. Contoh, angka terbesar kelima.",
		arguments: [
			{
				name: "array",
				description: "adalah array atau rentang data di mana Anda ingin menentukan nilai k-th terbesar tersebut"
			},
			{
				name: "k",
				description: "adalah posisi (dari yang terbesar) dalam array atau rentang sel dari nilai untuk dikembalikan"
			}
		]
	},
	{
		name: "LCM",
		description: "Menampilkan multipel umum paling sedikit.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "adalah 1 sampai 255 yang multipel umum paling sedikitnya Anda inginkan"
			},
			{
				name: "number2",
				description: "adalah 1 sampai 255 yang multipel umum paling sedikitnya Anda inginkan"
			}
		]
	},
	{
		name: "LEFT",
		description: "Menampilkan jumlah karakter tertentu dari awal string teks.",
		arguments: [
			{
				name: "text",
				description: "adalah string teks yang mengandung karakter yang ingin Anda ekstrak"
			},
			{
				name: "num_chars",
				description: "menentukan berapa banyak karakter yang Anda ingin LEFT ekstrak; 1 jika dihilangkan"
			}
		]
	},
	{
		name: "LEN",
		description: "Menampilkan jumlah karakter dalam string teks.",
		arguments: [
			{
				name: "text",
				description: "adalah teks yang panjangnya ingin Anda ketahui. Spasi dihitung sebagai karakter"
			}
		]
	},
	{
		name: "LINEST",
		description: "Menampilkan statistik yang menggambarkan pencocokan tren linear sesuai dengan poin data yang dikenal, dengan mencocokkan garis lurus menggunakan metode kuadrat terkecil.",
		arguments: [
			{
				name: "known_y's",
				description: "adalah perangkat dari nilai-y yang telah Anda ketahui dalam hubungan y = mx + b"
			},
			{
				name: "known_x's",
				description: "adalah seperangkat opsional dari nilai-x yang mungkin telah Anda kenal dalam hubungan y = mx + b"
			},
			{
				name: "const",
				description: "adalah nilai logis: konstanta b dihitung secara normal jika Const = BENAR atau dihilangkan; b ditata sama dengan 0 jika Const = SALAH"
			},
			{
				name: "stats",
				description: "adalah nilai logis: mengembalikan statistik regresi tambahan = BENAR; mengembalikan koefisien-m dan konstanta b = SALAH atau hilangkan"
			}
		]
	},
	{
		name: "LN",
		description: "Menampilkan logaritma alami dari angka.",
		arguments: [
			{
				name: "number",
				description: "adalah angka riil positif yang logaritma alaminya Anda inginkan"
			}
		]
	},
	{
		name: "LOG",
		description: "Menampilkan logarima sebuah angka ke basis yang Anda tentukan.",
		arguments: [
			{
				name: "number",
				description: "adalah angka riil positif yang logaritmanya Anda inginkan"
			},
			{
				name: "base",
				description: "adalah basis dari logaritma; 10 jika dihilangkan"
			}
		]
	},
	{
		name: "LOG10",
		description: "Menampilkan logaritma basis-10 dari angka.",
		arguments: [
			{
				name: "number",
				description: "adalah angka riil positif yang logaritma basis-10-nya Anda inginkan"
			}
		]
	},
	{
		name: "LOGEST",
		description: "Menampilkan statistik yang menggambarkan pencocokan kurva eksponensial poin data yang dikenal.",
		arguments: [
			{
				name: "known_y's",
				description: "adalah perangkat nilai-y yang telah Anda ketahui dalam hubungan y = b*m^x"
			},
			{
				name: "known_x's",
				description: "adalah serangkaian nilai-x opsional yang mungkin telah Anda ketahui dalam hubungan y = b*m^x"
			},
			{
				name: "const",
				description: "adalah nilai logis: konstanta b dihitung secara normal jika Const = BENAR atau dihilangkan; b ditata sama dengan 1 jika Const = SALAH"
			},
			{
				name: "stats",
				description: "adalah nilai logika: mengembalikan statistik regresi tambahan = TRUE; mengembalikan koefisien-m dan konstanta b = FALSE atau dihilangkan"
			}
		]
	},
	{
		name: "LOGINV",
		description: "Menampilkan invers fungsi distribusi kumulatif lognormal dari x, di mana ln(x) secara normal terdistribusi dengan parameter Nilai rata-rata dan Standar_dev.",
		arguments: [
			{
				name: "probability",
				description: "adalah probabilitas yang berhubungan dengan distribusi lognormal, angka di antara 0 sampai dengan 1"
			},
			{
				name: "mean",
				description: "adalah nilai rata-rata dari ln(x)"
			},
			{
				name: "standard_dev",
				description: "adalah standar deviasi dari ln(x), angka positif"
			}
		]
	},
	{
		name: "LOGNORM.DIST",
		description: "Menghasilkan distribusi lognormal dari x, di mana ln(x) terdistribusi secara normal dengan parameter Nilai rata-rata dan Standard_dev.",
		arguments: [
			{
				name: "x",
				description: "adalah nilai untuk mengevaluasi fungsi, yaitu bilangan positif"
			},
			{
				name: "mean",
				description: "adalah nilai rata-rata dari ln(x)"
			},
			{
				name: "standard_dev",
				description: "adalah simpangan standar dari ln(x), bilangan positif"
			},
			{
				name: "cumulative",
				description: "adalah nilai logis: untuk fungsi distribusi kumulatif, gunakan TRUE; untuk fungsi kerapatan probabilitas, gunakan FALSE"
			}
		]
	},
	{
		name: "LOGNORM.INV",
		description: "Menampilkan invers fungsi distribusi kumulatif lognormal dari x, di mana ln(x) secara normal terdistribusi dengan parameter Nilai rata-rata dan Standar_dev.",
		arguments: [
			{
				name: "probability",
				description: "adalah probabilitas yang berhubungan dengan distribusi lognormal, angka di antara 0 sampai dengan 1"
			},
			{
				name: "mean",
				description: "adalah nilai rata-rata dari ln(x)"
			},
			{
				name: "standard_dev",
				description: "adalah simpangan baku dari ln(x), angka positif"
			}
		]
	},
	{
		name: "LOGNORMDIST",
		description: "Menampilkan distribusi lognormal kumulatif dari  x, di mana ln(x) terdistribusi secara normal dengan parameter Nilai rata-rata dan Standar_dev.",
		arguments: [
			{
				name: "x",
				description: "adalah nilai untuk mengevaluasi fungsi, yaitu angka positif"
			},
			{
				name: "mean",
				description: "adalah nilai rata-rata dari ln(x)"
			},
			{
				name: "standard_dev",
				description: "adalah standar deviasi dari ln(x), angka positif"
			}
		]
	},
	{
		name: "LOOKUP",
		description: "Mencari nilai dari rentang satu-baris atau satu-kolom atau dari array. Tersedia untuk kompatibilitas terbalik.",
		arguments: [
			{
				name: "lookup_value",
				description: "adalah nilai yang LOOKUP-nya mencari ke dalam Vektor_Lookup dan dapat berupa angka, teks, nilai logis, atau nama atau referensi ke suatu nilai"
			},
			{
				name: "lookup_vector",
				description: "adalah rentang yang mengandung satu baris atau satu kolom teks, angka, atau nilai logis saja, tempatkan dalam urutan naik"
			},
			{
				name: "result_vector",
				description: "adalah rentang yang mengandung satu baris atau kolom saja, berukuran sama seperti vektor_Lookup"
			}
		]
	},
	{
		name: "LOWER",
		description: "Mengonversi semua huruf dalam string teks ke huruf kecil.",
		arguments: [
			{
				name: "text",
				description: "adalah teks yang ingin Anda ubah ke huruf kecil. Karakter dalam Teks yang bukan huruf tidak diubah"
			}
		]
	},
	{
		name: "MATCH",
		description: "Menampilkan posisi relatif item dalam array yang cocok dengan nilai tertentu dalam urutan tertentu.",
		arguments: [
			{
				name: "lookup_value",
				description: "adalah nilai yang Anda gunakan untuk mencari nilai yang Anda inginkan dalam array, angka, teks, atau nilai logika, atau referensi ke salah satunya"
			},
			{
				name: "lookup_array",
				description: "adalah rentang sel berdekatan yang mengandung nilai pencarian, array dari nilai, atau referensi ke array yang mungkin"
			},
			{
				name: "match_type",
				description: "adalah angka 1, 0, atau -1 menunjukkan angka yang mana untuk dikembalikan."
			}
		]
	},
	{
		name: "MAX",
		description: "Menampilkan nilai terbesar dalam seperangkat nilai. Abaikan nilai dan teks logis.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "adalah angka 1 sampai 255, sel kosong, nilai logis, atau nomor teks yang Anda ingin nilainya maksimal"
			},
			{
				name: "number2",
				description: "adalah angka 1 sampai 255, sel kosong, nilai logis, atau nomor teks yang Anda ingin nilainya maksimal"
			}
		]
	},
	{
		name: "MAXA",
		description: "Menampilkan nilai terbesar dalam serangkaian nilai. Tidak mengabaikan nilai logis dan teks.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "adalah angka dari 1 sampai 255, sel kosong, nilai logis, atau nomor teks yang maksimalnya Anda inginkan"
			},
			{
				name: "value2",
				description: "adalah angka dari 1 sampai 255, sel kosong, nilai logis, atau nomor teks yang maksimalnya Anda inginkan"
			}
		]
	},
	{
		name: "MDETERM",
		description: "Menampilkan determinan matrik dari array.",
		arguments: [
			{
				name: "array",
				description: "adalah array numerik dengan jumlah baris dan kolom yang sama, baik rentang sel maupun konstanta array"
			}
		]
	},
	{
		name: "MEDIAN",
		description: "Menampilkan nilai tengah, atau angka di tengah-tengah perangkat angka yang ditentukan.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "adalah angka atau nama, array, atau referensi 1 sampai 255 yang mengandung angka dimana Anda inginkan nilai tengahnya"
			},
			{
				name: "number2",
				description: "adalah angka atau nama, array, atau referensi 1 sampai 255 yang mengandung angka dimana Anda inginkan nilai tengahnya"
			}
		]
	},
	{
		name: "MID",
		description: "Menampilkan karakter dari tengah string teks, memberikan posisi awal dan panjang.",
		arguments: [
			{
				name: "text",
				description: "adalah string teks yang dari sini Anda ingin mengekstrak karakternya"
			},
			{
				name: "start_num",
				description: "adalah posisi dari karakter pertama yang ingin Anda ekstrak. Karakter pertama dalam Teks adalah 1"
			},
			{
				name: "num_chars",
				description: "tentukan berapa banyak karakter untuk dikembalikan dari Teks"
			}
		]
	},
	{
		name: "MIN",
		description: "Menampilkan nilai terkecil dalam seperangkat nilai. Abaikan nilai dan teks logis.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "adalah angka 1 sampai 255, sel kosong, nilai logis, atau angka teks yang Anda ingin nilainya minimal"
			},
			{
				name: "number2",
				description: "adalah angka 1 sampai 255, sel kosong, nilai logis, atau angka teks yang Anda ingin nilainya minimal"
			}
		]
	},
	{
		name: "MINA",
		description: "Menampilkan nilai terkecil dalam perangkat nilai. Tidak mengabaikan nilai logika dan teks.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "adalah angka, sel kosong, nilai logika atau nomor teks 1 sampai 255 yang minimalnya Anda inginkan"
			},
			{
				name: "value2",
				description: "adalah angka, sel kosong, nilai logika atau nomor teks 1 sampai 255 yang minimalnya Anda inginkan"
			}
		]
	},
	{
		name: "MINUTE",
		description: "Menampilkan menit, angka dari 0 sampai 59.",
		arguments: [
			{
				name: "serial_number",
				description: "adalah angka dalam kode tanggal-waktu yang digunakan oleh Spreadsheet atau teks dalam format waktu, seperti 16:48:00 atau 4:48:00 PM"
			}
		]
	},
	{
		name: "MINVERSE",
		description: "Menampilkan matrik invers untuk matrik yang disimpan dalam array.",
		arguments: [
			{
				name: "array",
				description: "adalah array numerik dengan jumlah baris dan kolom yang sama, baik rentang sel maupun konstanta array"
			}
		]
	},
	{
		name: "MIRR",
		description: "Menampilkan angka internal dari pengembalian seri aliran kas secara periodik, mempertimbangkan biaya investasi dan kepentingan pada investasi kembali uang.",
		arguments: [
			{
				name: "values",
				description: "adalah array atau referensi ke sel yang mengandung angka yang menunjukkan seri pembayaran (negatif) dan pemasukan (positif) pada periode reguler"
			},
			{
				name: "finance_rate",
				description: "adalah suku bunga yang Anda bayarkan atas uang yang digunakan dalam aliran kas"
			},
			{
				name: "reinvest_rate",
				description: "adalah suku bunga yang Anda terima pada aliran kas saat Anda menginvestasikan kembali aliran kas"
			}
		]
	},
	{
		name: "MMULT",
		description: "Menampilkan produk matrik dari dua array, array dengan jumlah baris yang sama dengan Array1 dan jumlah kolom yang sama dengan Array2.",
		arguments: [
			{
				name: "array1",
				description: "adalah array pertama dari angka untuk dikalikan dan harus memiliki jumlah kolom yang sama dengan Array2 yang memiliki baris"
			},
			{
				name: "array2",
				description: "adalah array pertama dari angka untuk dikalikan dan harus memiliki jumlah kolom yang sama dengan Array2 yang memiliki baris"
			}
		]
	},
	{
		name: "MOD",
		description: "Menampilkan sisanya setelah angka dibagi dengan pembagi.",
		arguments: [
			{
				name: "number",
				description: "adalah angka yang ingin Anda temukan sisanya setelah pembagian dilakukan"
			},
			{
				name: "divisor",
				description: "adalah angka yang Angkanya ingin Anda bagi"
			}
		]
	},
	{
		name: "MODE",
		description: "Mengembalikan yang paling sering mu ncul, atau berulang, nilai dalam barisan atau rentang data.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "adalah angka, atau nama, barisan, atau referensi 1 sampai 255 yang mengandung angka yang modusnya Anda inginkan"
			},
			{
				name: "number2",
				description: "adalah angka, atau nama, barisan, atau referensi 1 sampai 255 yang mengandung angka yang modusnya Anda inginkan"
			}
		]
	},
	{
		name: "MODE.MULT",
		description: "Menghasilkan array vertikal dari nilai yang paling sering muncul, atau berulang dalam array atau rentang data.  Untuk array horizontal, gunakan =TRANSPOSE(MODE.MULT(angka1,angka2,...)).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "adalah angka 1 sampai 255, atau nama, array, atau referensi yang berisi angka yang Anda inginkan modenya"
			},
			{
				name: "number2",
				description: "adalah angka 1 sampai 255, atau nama, array, atau referensi yang berisi angka yang Anda inginkan modenya"
			}
		]
	},
	{
		name: "MODE.SNGL",
		description: "Menampilkan yang paling sering terjadi, atau berulang, nilai dalam array atau rentang data.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "adalah angka, atau nama, array, atau referensi 1 sampai 255 yang mengandung angka dengan mode yang Anda inginkan"
			},
			{
				name: "number2",
				description: "adalah angka, atau nama, array, atau referensi 1 sampai 255 yang mengandung angka dengan mode yang Anda inginkan"
			}
		]
	},
	{
		name: "MONTH",
		description: "Menampilkan bulan, angka dari 1 (Januari) sampai 12 (Desember).",
		arguments: [
			{
				name: "serial_number",
				description: "adalah angka dalam kode tanggal-waktu yang digunakan oleh Spreadsheet"
			}
		]
	},
	{
		name: "MROUND",
		description: "Menampilkan bilangan yang dibulatkan ke multipel yang diinginkan.",
		arguments: [
			{
				name: "number",
				description: "adalah nilai yang akan dibulatkan"
			},
			{
				name: "multiple",
				description: "adalah multipel untuk membulatkan bilangan"
			}
		]
	},
	{
		name: "MULTINOMIAL",
		description: "Menampilkan multinomial dari set bilangan.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "adalah 1 sampai 255 yang multinomialnya Anda inginkan"
			},
			{
				name: "number2",
				description: "adalah 1 sampai 255 yang multinomialnya Anda inginkan"
			}
		]
	},
	{
		name: "MUNIT",
		description: "Menghasilkan matriks satuan untuk dimensi yang ditentukan.",
		arguments: [
			{
				name: "dimension",
				description: "adalah bilangan bulat yang menentukan dimensi matriks satuan yang ingin Anda hasilkan"
			}
		]
	},
	{
		name: "N",
		description: "Mengonversi nilai non-angka ke angka, tanggal ke nomor seri, BENAR ke 1, yang lain ke 0 (nol).",
		arguments: [
			{
				name: "value",
				description: "adalah nilai yang ingin Anda ubah"
			}
		]
	},
	{
		name: "NA",
		description: "Mengembalikan nilai kesalahan #N/A (nilai tidak tersedia).",
		arguments: [
		]
	},
	{
		name: "NEGBINOM.DIST",
		description: "Menghasilkan distribusi negatif binomial, probabilitas bahwa mungkin ada kegagalan Number_f agar Number_ke-s berhasil, dengan probabilitas keberhasilan Probability_s.",
		arguments: [
			{
				name: "number_f",
				description: "adalah jumlah kegagalan"
			},
			{
				name: "number_s",
				description: "adalah jumlah ambang batas dari keberhasilan"
			},
			{
				name: "probability_s",
				description: "adalah probabilitas keberhasilan; angka di antara 0 dan 1"
			},
			{
				name: "cumulative",
				description: "adalah nilai logis: untuk fungsi distribusi kumulatif, gunakan TRUE; untuk fungsi massa probabilitas, gunakan FALSE"
			}
		]
	},
	{
		name: "NEGBINOMDIST",
		description: "Menampilkan distribusi negatif binomial, probabilitas yang mungkin terdapat Number_f kegagalan sebelum Number_s-th berhasil, dengan Probability_s probabilitas dari keberhasilan.",
		arguments: [
			{
				name: "number_f",
				description: "adalah jumlah kegagalan"
			},
			{
				name: "number_s",
				description: "adalah jumlah ambang batas dari keberhasilan"
			},
			{
				name: "probability_s",
				description: "adalah probabilitas keberhasilan; angka di antara 0 dan 1"
			}
		]
	},
	{
		name: "NETWORKDAYS",
		description: "Menampilkan jumlah dari seluruh hari kerja antara dua tanggal.",
		arguments: [
			{
				name: "start_date",
				description: "adalah angka tanggal seri yang mewakili tanggal mulai"
			},
			{
				name: "end_date",
				description: "adalah angka tanggal seri yang mewakili tanggal selesai"
			},
			{
				name: "holidays",
				description: "adalah set tambahan dari satu atau lebih angka tanggal seri untuk dieksklusifkan dari kalender kerja, seperti libur umum dan nasional dan liburan tertunda"
			}
		]
	},
	{
		name: "NETWORKDAYS.INTL",
		description: "Menghasilkan jumlah seluruh hari kerja antara dua tanggal dengan parameter akhir pekan kustom.",
		arguments: [
			{
				name: "start_date",
				description: "adalah angka tanggal seri yang menunjukkan tanggal mulai"
			},
			{
				name: "end_date",
				description: "adalah angka tanggal seri yang menunjukkan tanggal akhir"
			},
			{
				name: "weekend",
				description: "adalah angka atau string yang menentukan saat akhir pekan berlangsung"
			},
			{
				name: "holidays",
				description: "adalah set opsional dari satu atau beberapa angka tanggal seri untuk dikeluarkan dari kalender kerja, seperti libur umum dan nasional dan liburan tertunda"
			}
		]
	},
	{
		name: "NOMINAL",
		description: "Menampilkan suku bunga tahunan.",
		arguments: [
			{
				name: "effect_rate",
				description: "adalah suku bunga yang berlaku"
			},
			{
				name: "npery",
				description: "adalah jumlah dari periode penyusunan per tahun"
			}
		]
	},
	{
		name: "NORM.DIST",
		description: "Menghasilkan distribusi normal untuk nilai rata-rata dan simpangan standar tertentu.",
		arguments: [
			{
				name: "x",
				description: "adalah nilai yang Anda inginkan distribusinya"
			},
			{
				name: "mean",
				description: "adalah nilai rata-rata aritmatika distribusi"
			},
			{
				name: "standard_dev",
				description: "adalah simpangan standar distribusi, bilangan positif"
			},
			{
				name: "cumulative",
				description: "adalah nilai logis: untuk fungsi distribusi kumulatif, gunakan TRUE; untuk fungsi kerapatan probabilitas, gunakan FALSE"
			}
		]
	},
	{
		name: "NORM.INV",
		description: "Menampilkan invers distribusi kumulatif normal untuk nilai rata-rata dan simpangan baku tertentu.",
		arguments: [
			{
				name: "probability",
				description: "adalah probabilitas yang berhubungan dengan distribusi normal, angka di antara 0 sampai dengan 1"
			},
			{
				name: "mean",
				description: "adalah nilai rata-rata aritmatika distribusi"
			},
			{
				name: "standard_dev",
				description: "adalah simpangan baku distribusi, angka positif"
			}
		]
	},
	{
		name: "NORM.S.DIST",
		description: "Menghasilkan distribusi kumulatif standar normal (memiliki nilai rata-rata nol dan simpangan standar satu).",
		arguments: [
			{
				name: "z",
				description: "adalah nilai yang ingin Anda distribusikan"
			},
			{
				name: "cumulative",
				description: "adalah nilai logis bagi fungsi untuk dikembalikan: fungsi distribusi kumulatif = TRUE; fungsi kerapatan probabilitas = FALSE"
			}
		]
	},
	{
		name: "NORM.S.INV",
		description: "Menampilkan invers distribusi kumulatif standar normal (memiliki nilai rata-rata nol dan simpangan baku satu).",
		arguments: [
			{
				name: "probability",
				description: "adalah probabilitas yang berhubungan dengan distribusi normal, angka di antara 0 sampai dengan 1"
			}
		]
	},
	{
		name: "NORMDIST",
		description: "Menampilkan distribusi kumulatif normal untuk nilai rata-rata dan simpangan baku tertentu.",
		arguments: [
			{
				name: "x",
				description: "adalah nilai yang ingin Anda distribusi"
			},
			{
				name: "mean",
				description: "adalah nilai rata-rata aritmatika distribusi"
			},
			{
				name: "standard_dev",
				description: "adalah simpangan baku dari distribusi, angka positif"
			},
			{
				name: "cumulative",
				description: "adalah nilai logis: untuk fungsi distribusi kumulatif, gunakan BENAR; untuk fungsi kerapatan probabilitas, gunakan SALAH"
			}
		]
	},
	{
		name: "NORMINV",
		description: "Menampilkan invers distribusi kumulatif normal untuk nilai rata-rata dan simpangan baku tertentu.",
		arguments: [
			{
				name: "probability",
				description: "adalah probabilitas yang bersesuaian ke distribusi normal, angka di antara 0 sampai dengan 1"
			},
			{
				name: "mean",
				description: "adalah nilai rata-rata aritmatika distribusi"
			},
			{
				name: "standard_dev",
				description: "adalah simpangan baku distribusi, angka positif"
			}
		]
	},
	{
		name: "NORMSDIST",
		description: "Menampilkan distribusi kumulatif standar normal (memiliki nilai rata-rata nol dan simpangan baku satu).",
		arguments: [
			{
				name: "z",
				description: "adalah nilai yang ingin Anda distribusikan"
			}
		]
	},
	{
		name: "NORMSINV",
		description: "Menampilkan invers distribusi kumulatif standar normal (memiliki nilai rata-rata nol dan simpangan baku satu).",
		arguments: [
			{
				name: "probability",
				description: "adalah probabilitas yang bersesuaian ke distribusi normal, angka di antara 0 sampai dengan 1"
			}
		]
	},
	{
		name: "NOT",
		description: "Mengubah SALAH ke BENAR, atau BENAR ke SALAH.",
		arguments: [
			{
				name: "logical",
				description: "adalah nilai atau eksperesi yang dapat dievaluasi ke BENAR atau SALAH"
			}
		]
	},
	{
		name: "NOW",
		description: "Menampilkan tanggal dan waktu sekarang terformat sebagai tanggal dan waktu.",
		arguments: [
		]
	},
	{
		name: "NPER",
		description: "Menampilkan jumlah periode untuk investasi secara periodik, konstanta pembayaran dan konstanta suku bunga.",
		arguments: [
			{
				name: "rate",
				description: "adalah suku bunga tiap periode. Contoh, gunakan 6%/4 untuk pembayaran per kuartal pada APR 6%"
			},
			{
				name: "pmt",
				description: "adalah pembayaran yang dilakukan tiap periode dan tidak dapat mengubah jangka waktu investasi"
			},
			{
				name: "pv",
				description: "adalah nilai sekarang, atau jumlah keseluruhan yang dibayar sekaligus di mana seri pembayaran masa depan bermanfaat sekarang"
			},
			{
				name: "fv",
				description: "adalah nilai masa depan, atau saldo kas yang ingin Anda capai setelah pembayaran terakhir dilakukan. Jika diabaikan, nol digunakan"
			},
			{
				name: "type",
				description: "adalah nilai logis: pembayaran pada awal periode = 1; pembayaran pada akhir periode = 0 atau dihapus"
			}
		]
	},
	{
		name: "NPV",
		description: "Menampilkan nilai bersih sekarang dari investasi berdasar pada harga diskon dan seri pembayaran di masa mendatang (nilai negatif) dan pemasukan (nilai positif).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "rate",
				description: "adalah harga diskon melebihi panjang satu periode"
			},
			{
				name: "value1",
				description: "adalah pembayaran dan pemasukan 1 sampai 254, yang ditempatkan secara bersamaan dalam waktu dan terjadi pada akhir dari tiap periode"
			},
			{
				name: "value2",
				description: "adalah pembayaran dan pemasukan 1 sampai 254, yang ditempatkan secara bersamaan dalam waktu dan terjadi pada akhir dari tiap periode"
			}
		]
	},
	{
		name: "NUMBERVALUE",
		description: "Mengonversi teks ke angka tanpa tergantung pada lokalnya.",
		arguments: [
			{
				name: "text",
				description: "adalah string yang menunjukkan angka yang ingin Anda konversi"
			},
			{
				name: "decimal_separator",
				description: "adalah karakter yang digunakan sebagai pemisah desimal di string ini"
			},
			{
				name: "group_separator",
				description: "adalah karakter yang digunakan sebagai pemisah grup di string tersebut"
			}
		]
	},
	{
		name: "OCT2BIN",
		description: "Mengonversi sebuah bilangan oktal ke biner.",
		arguments: [
			{
				name: "number",
				description: "adalah bilangan oktal yang ingin Anda konversi"
			},
			{
				name: "places",
				description: "adalah jumlah karakter untuk digunakan"
			}
		]
	},
	{
		name: "OCT2DEC",
		description: "Mengonversi sebuah bilangan oktal ke desimal.",
		arguments: [
			{
				name: "number",
				description: "adalah bilangan oktal yang ingin Anda konversi"
			}
		]
	},
	{
		name: "OCT2HEX",
		description: "Mengonversi bilangan oktal ke hexadesimal.",
		arguments: [
			{
				name: "number",
				description: "adalah bilangan oktal yang ingin Anda konversi"
			},
			{
				name: "places",
				description: "adalah jumlah karakter untuk digunakan"
			}
		]
	},
	{
		name: "ODD",
		description: "Membulatkan angka positif ke atas dan angka negatif ke bawah ke bilangan bulat ganjil terdekat.",
		arguments: [
			{
				name: "number",
				description: "adalah nilai untuk dibulatkan"
			}
		]
	},
	{
		name: "OFFSET",
		description: "Menampilkan referensi ke rentang yang adalah jumlah baris dan kolom yang ditentukan dari referensi yang ditentukan.",
		arguments: [
			{
				name: "reference",
				description: "adalah referensi di mana Anda ingin memusatkan offset, referensi ke sel atau rentang sel yang berdekatan"
			},
			{
				name: "rows",
				description: "adalah jumlah baris, ke atas atau ke bawah, yang Anda inginkan sel kiri-atas hasil merujuk ke"
			},
			{
				name: "cols",
				description: "adalah jumlah kolom, ke kiri atau kanan, yang Anda inginkan sel kiri-atas hasil merujuk ke"
			},
			{
				name: "height",
				description: "adalah tinggi, dalam jumlah baris, di mana Anda ingin hasilnya menjadi, memiliki tinggi yang sama seperti Referensi jika dihilangkan"
			},
			{
				name: "width",
				description: "adalah lebar, dalam jumlah kolom, yang hasilnya Anda inginkan, memiliki lebar yang sama dengan Referensi jika dihilangkan"
			}
		]
	},
	{
		name: "OR",
		description: "Memeriksa apakah tiap argumen adalah BENAR, dan mengembalikan BENAR atau SALAH. Menampilkan SALAH hanya jika semua argumen adalah SALAH.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logical1",
				description: "adalah kondisi 1 sampai 255 yang ingin Anda tes yang dapat bernilai BENAR atau SALAH"
			},
			{
				name: "logical2",
				description: "adalah kondisi 1 sampai 255 yang ingin Anda tes yang dapat bernilai BENAR atau SALAH"
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
		description: "Menghasilkan sejumlah periode yang diperlukan oleh investasi untuk mencapai nilai yang ditentukan.",
		arguments: [
			{
				name: "rate",
				description: "adalah suku bunga per periode."
			},
			{
				name: "pv",
				description: "adalah nilai investasi saat ini"
			},
			{
				name: "fv",
				description: "adalah nilai investasi yang diinginkan di masa mendatang"
			}
		]
	},
	{
		name: "PEARSON",
		description: "Menampilkan koefisien korelasi momen produk Pearson, r.",
		arguments: [
			{
				name: "array1",
				description: "adalah seperangkat nilai independen"
			},
			{
				name: "array2",
				description: "adalah seperangkat nilai dependen"
			}
		]
	},
	{
		name: "PERCENTILE",
		description: "Mengembalikan persentil k-th dari nilai dalam rentang.",
		arguments: [
			{
				name: "array",
				description: "adalah barisan atau rentang data yang menetapkan kedudukan relatif"
			},
			{
				name: "k",
				description: "adalah nilai persentil yang ada di antara 0 sampai dengan 1"
			}
		]
	},
	{
		name: "PERCENTILE.EXC",
		description: "Menghasilkan persentil ke-k dari nilai di suatu rentang, di mana k dalam rentang 0..1, tidak termasuk.",
		arguments: [
			{
				name: "array",
				description: "adalah array atau rentang data yang menentukan posisi relatif"
			},
			{
				name: "k",
				description: "adalah nilai persentil antara 0 hingga 1, termasuk"
			}
		]
	},
	{
		name: "PERCENTILE.INC",
		description: "Menghasilkan persentil ke-k dari nilai di suatu rentang, di mana k dalam rentang 0..1, termasuk.",
		arguments: [
			{
				name: "array",
				description: "adalah array atau rentang data yang menentukan kedudukan relatif"
			},
			{
				name: "k",
				description: "adalah nilai persentil antara 0 hingga 1, termasuk"
			}
		]
	},
	{
		name: "PERCENTRANK",
		description: "Mengembalikan peringkat nilai dalam perangkat data sebagai persentase dari perangkat data.",
		arguments: [
			{
				name: "array",
				description: "adalah barisan atau rentang data dengan nilai angka yang menetapkan kedudukan relatif"
			},
			{
				name: "x",
				description: "adalah nilai yang ingin Anda ketahui peringkatnya"
			},
			{
				name: "significance",
				description: "adalah nilai opsional yang mengenali angka digit yang berarti untuk persentase yang dikembalikan, tiga digit jika diabaikan (0.xxx%)"
			}
		]
	},
	{
		name: "PERCENTRANK.EXC",
		description: "Menghasilkan peringkat nilai dalam set data sebagai persentase dari set data sebagai persentase (0..1, tidak termasuk) dari set data.",
		arguments: [
			{
				name: "array",
				description: "adalah array atau rentang data dengan nilai numerik yang menetapkan kedudukan relatif"
			},
			{
				name: "x",
				description: "adalah nilai yang ingin Anda ketahui peringkatnya"
			},
			{
				name: "significance",
				description: "adalah nilai opsional yang mengenali angka digit yang signifikan untuk persentase yang dikembalikan, tiga digit jika diabaikan (0,xxx%)"
			}
		]
	},
	{
		name: "PERCENTRANK.INC",
		description: "Menghasilkan peringkat nilai dalam set data sebagai persentase dari set data sebagai persentase (0..1, termasuk) dari set data.",
		arguments: [
			{
				name: "array",
				description: "adalah array atau rentang data dengan nilai numerik yang menetapkan kedudukan relatif"
			},
			{
				name: "x",
				description: "adalah nilai yang ingin Anda ketahui peringkatnya"
			},
			{
				name: "significance",
				description: "adalah nilai opsional yang mengenali angka digit yang signifikan untuk persentase yang dikembalikan, tiga digit jika diabaikan (0,xxx%)"
			}
		]
	},
	{
		name: "PERMUT",
		description: "Menampilkan jumlah permutasi untuk jumlah objek yang ditentukan yang dapat dipilih dari objek total.",
		arguments: [
			{
				name: "number",
				description: "adalah jumlah total dari objek"
			},
			{
				name: "number_chosen",
				description: "adalah jumlah objek dalam tiap permutasi"
			}
		]
	},
	{
		name: "PERMUTATIONA",
		description: "Menghasilkan jumlah permutasi untuk sejumlah objek tertentu (dengan pengulangan) yang dapat dipilih dari objek total.",
		arguments: [
			{
				name: "number",
				description: "adalah jumlah objek total"
			},
			{
				name: "number_chosen",
				description: "adalah jumlah objek di setiap permutasi"
			}
		]
	},
	{
		name: "PHI",
		description: "Menghasilkan nilai fungsi kerapatan untuk distribusi normal standar.",
		arguments: [
			{
				name: "x",
				description: "adalah angka yang Anda inginkan kerapatan distribusi normal standarnya"
			}
		]
	},
	{
		name: "PI",
		description: "Menampilkan nilai dari Pi, 3,14159265358979, akurat untuk 15 digit.",
		arguments: [
		]
	},
	{
		name: "PMT",
		description: "Menghitung pembayaran untuk pinjaman berdasarkan konstanta pembayaran dan konstanta suku bunga.",
		arguments: [
			{
				name: "rate",
				description: "adalah suku bunga tiap periode untuk pinjaman. Contoh, gunakan 6%/4 untuk pembayaran tiap kuartal APR 6%"
			},
			{
				name: "nper",
				description: "adalah total jumlah pembayaran untuk pinjaman tersebut"
			},
			{
				name: "pv",
				description: "adalah nilai sekarang: jumlah total di mana seri pembayaran di masa mendatang bermanfaat sekarang"
			},
			{
				name: "fv",
				description: "adalah nilai masa depan, atau saldo kas yang ingin Anda capai setelah pembayaran terakhir dilakukan. 0 (nol) jika dihilangkan"
			},
			{
				name: "type",
				description: "adalah nilai logis: pembayaran pada awal periode = 1; pembayaran pada akhir periode = 0 atau dihapus"
			}
		]
	},
	{
		name: "POISSON",
		description: "Menampilkan distribusi Poisson.",
		arguments: [
			{
				name: "x",
				description: "adalah jumlah kejadian"
			},
			{
				name: "mean",
				description: "adalah nilai angka yang diharapkan, angka positif"
			},
			{
				name: "cumulative",
				description: "adalah nilai logis: untuk probabilitas Poisson kumulatif, gunakan BENAR; untuk fungsi massa probabilitas Poisson, gunakan SALAH"
			}
		]
	},
	{
		name: "POISSON.DIST",
		description: "Menampilkan distribusi Poisson.",
		arguments: [
			{
				name: "x",
				description: "adalah jumlah kejadian"
			},
			{
				name: "mean",
				description: "adalah nilai numerik yang diharapkan, angka positif"
			},
			{
				name: "cumulative",
				description: "adalah nilai logis: untuk probabilitas Poisson kumulatif, gunakan BENAR; untuk fungsi massa probabilitas Poisson, gunakan SALAH"
			}
		]
	},
	{
		name: "POWER",
		description: "Menampilkan hasil dari nilai yang dipangkatkan.",
		arguments: [
			{
				name: "number",
				description: "adalah angka basis, angka riil apa saja"
			},
			{
				name: "power",
				description: "adalah eksponen, tempat angka basis dipangkatkan"
			}
		]
	},
	{
		name: "PPMT",
		description: "Menampilkan pembayaran pada uang pokok untuk investasi tertentu berdasarkan pada pembayaran periodik dan suku bunga konstan.",
		arguments: [
			{
				name: "rate",
				description: "adalah suku bunga tiap periode. Contoh, gunakan 6%/4 untuk pembayaran tiap kuartal pada APR 6%"
			},
			{
				name: "per",
				description: "menentukan periode dan harus ada dalam rentang 1 sampai nper"
			},
			{
				name: "nper",
				description: "adalah total jumlah periode pembayaran dalam investasi"
			},
			{
				name: "pv",
				description: "adalah nilai sekarang: jumlah total di mana seri pembayaran masa depan bermanfaat sekarang"
			},
			{
				name: "fv",
				description: "adalah nilai masa depan, atau saldo kas yang ingin Anda capai setelah pembayaran terakhir dilakukan"
			},
			{
				name: "type",
				description: "adalah nilai logis: pembayaran di awal periode = 1; pembayaran di akhir periode = 0 atau dihilangkan"
			}
		]
	},
	{
		name: "PRICEDISC",
		description: "Menampilkan harga per $100 nilai awal dari pemotongan sekuritas.",
		arguments: [
			{
				name: "settlement",
				description: "adalah tanggal pelunasan sekuritas, pelunasan sekuritas"
			},
			{
				name: "maturity",
				description: "adalah tanggal jatuh tempo sekuritas, yang dinyatakan sebagai angka tanggal seri"
			},
			{
				name: "discount",
				description: "adalah tingkat potongan harga sekuritas"
			},
			{
				name: "redemption",
				description: "adalah nilai tebusan sekuritas per $100 nilai awal"
			},
			{
				name: "basis",
				description: "adalah jenis dari basis penghitungan hari untuk digunakan"
			}
		]
	},
	{
		name: "PROB",
		description: "Menampilkan probabilitas bahwa nilai dalam rentang adalah di antara dua limit atau sama dengan limit terendah.",
		arguments: [
			{
				name: "x_range",
				description: "adalah rentang nilai numerik x di mana terdapat probabilitas terkait"
			},
			{
				name: "prob_range",
				description: "adalah perangkat probabilitas yang berhubungan dengan nilai dalam rentang_ X, nilai di antara 0 dan 1 dan mengeluarkan 0"
			},
			{
				name: "lower_limit",
				description: "adalah batas terendah pada nilai yang probabilitasnya Anda inginkan"
			},
			{
				name: "upper_limit",
				description: "adalah batas atas opsional nilai. Jika dihilangkan, PROB mengembalikan probabilitas bahwa nilai rentang_X adalah sama dengan limit_lebih Rendah"
			}
		]
	},
	{
		name: "PRODUCT",
		description: "Mengalikan semua angka yang ditentukan sebagai argumen.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "adalah angka, nilai logis, atau teks 1 sampai 255 representasi dari angka yang ingin Anda kalikan"
			},
			{
				name: "number2",
				description: "adalah angka, nilai logis, atau teks 1 sampai 255 representasi dari angka yang ingin Anda kalikan"
			}
		]
	},
	{
		name: "PROPER",
		description: "Mengonversi string teks ke huruf yang semestinya; huruf pertama pada tiap kata berhuruf besar, dan semua huruf lain berhuruf kecil.",
		arguments: [
			{
				name: "text",
				description: "adalah teks yang terlampir dalam tanda kutip, rumus yang mengembalikan teks, atau referensi ke sel yang mengandung teks ke huruf besar sebagian"
			}
		]
	},
	{
		name: "PV",
		description: "Menampilkan nilai investasi sekarang: jumlah total di mana seri pembayaran masa depan bermanfaat sekarang.",
		arguments: [
			{
				name: "rate",
				description: "adalah suku bunga tiap periode. Contoh, gunakan 6%/4 untuk pembayaran tiap kuartal pada APR 6%"
			},
			{
				name: "nper",
				description: "adalah total jumlah periode pembayaran dalam investasi"
			},
			{
				name: "pmt",
				description: "adalah pembayaran yang dilakukan tiap periode dan tidak dapat mengubah jangka waktu investasi"
			},
			{
				name: "fv",
				description: "adalah nilai masa depan, atau saldo kas yang ingin Anda capai setelah pembayaran terakhir dilakukan"
			},
			{
				name: "type",
				description: "adalah nilai logis: pembayaran di awal periode = 1; pembayaran di akhir periode = 0 atau dihilangkan"
			}
		]
	},
	{
		name: "QUARTILE",
		description: "Menampilkan kuartil perangkat data.",
		arguments: [
			{
				name: "array",
				description: "adalah barisan atau rentang sel dari nilai angka di mana Anda menginginkan nilai kuartilnya"
			},
			{
				name: "quart",
				description: "adalah angka: nilai minimal = 0; kuartil ke-1 = 1; nilai median = 2; kuartil ke-3 = 3; nilai maksimal = 4"
			}
		]
	},
	{
		name: "QUARTILE.EXC",
		description: "Menghasilkan kuartil set data, berdasarkan nilai persentil dari 0..1, tidak termasuk.",
		arguments: [
			{
				name: "array",
				description: "adalah array atau rentang sel dari nilai numerik yang ingin Anda dapatkan nilai kuartilnya"
			},
			{
				name: "quart",
				description: "adalah angka: nilai minimal = 0; kuartil ke-1 = 1; nilai median = 2; kuartil ke-3 = 3; nilai maksimal = 4"
			}
		]
	},
	{
		name: "QUARTILE.INC",
		description: "Menghasilkan kuartil set data, berdasarkan nilai persentil dari 0..1, termasuk.",
		arguments: [
			{
				name: "array",
				description: "adalah array atau rentang sel dari nilai numerik yang ingin Anda dapatkan nilai kuartilnya"
			},
			{
				name: "quart",
				description: "adalah angka: nilai minimal = 0; kuartil ke-1 = 1; nilai median = 2; kuartil ke-3 = 3; nilai maksimal = 4"
			}
		]
	},
	{
		name: "QUOTIENT",
		description: "Menampilkan porsi bilangan bulat dari pembagian.",
		arguments: [
			{
				name: "numerator",
				description: "adalah yang dibagi"
			},
			{
				name: "denominator",
				description: "adalah pembagi"
			}
		]
	},
	{
		name: "RADIANS",
		description: "Mengonversi derajat ke radian.",
		arguments: [
			{
				name: "angle",
				description: "adalah sudut dalam derajat yang ingin Anda ubah"
			}
		]
	},
	{
		name: "RAND",
		description: "Menampilkan angka acak lebih besar dari 0 dan kurang dari 1, terdistribusi secara merata (perubahan pada perhitungan ulang).",
		arguments: [
		]
	},
	{
		name: "RANDBETWEEN",
		description: "Menampilkan bilangan acak antara bilangan yang anda tentukan.",
		arguments: [
			{
				name: "bottom",
				description: "adalah bilangan bulat terkecil RANDBETWEEN akan muncul"
			},
			{
				name: "top",
				description: "adalah bilangan bulat terbesar RANDBETWEEN akan muncul"
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
		description: "Menampilkan peringkat angka dalam daftar angka: ukurannya relatif kepada nilai lain dalam daftar.",
		arguments: [
			{
				name: "number",
				description: "adalah angka yang ingin Anda temukan peringkatnya"
			},
			{
				name: "ref",
				description: "adalah barisan dari, atau referensi ke, daftar angka. Nilai bukan angka diabaikan"
			},
			{
				name: "order",
				description: "adalah angka: peringkat dalam daftar diurut secara menurun = 0 atau dihilangkan; peringkat dalam daftar diurut secara menaik = nilai bukan nol apa saja"
			}
		]
	},
	{
		name: "RANK.AVG",
		description: "Menghasilkan peringkat angka dalam daftar angka: ukurannya relatif dibandingkan nilai lain dalam daftar; jika lebih dari satu nilai memiliki peringkat yang sama, yang dihasilkan adalah peringkat rata-ratanya.",
		arguments: [
			{
				name: "number",
				description: "adalah angka yang ingin Anda ketahui peringkatnya"
			},
			{
				name: "ref",
				description: "adalah array, atau acuan ke, daftar angka. Nilai non-numerik diabaikan"
			},
			{
				name: "order",
				description: "adalah angka: peringkat dalam daftar disortir secara menurun = 0 atau dihilangkan; peringkat dalam daftar disortir secara naik = sembarang nilai asal bukan nol"
			}
		]
	},
	{
		name: "RANK.EQ",
		description: "Menghasilkan peringkat angka dalam daftar angka: ukurannya relatif dibandingkan nilai lain dalam daftar; jika lebih dari satu nilai memiliki peringkat yang sama, yang dihasilkan adalah peringkat atas dari set nilai tersebut.",
		arguments: [
			{
				name: "number",
				description: "adalah angka yang ingin Anda ketahui peringkatnya"
			},
			{
				name: "ref",
				description: "adalah array, atau acuan ke, daftar angka. Nilai non-numerik diabaikan"
			},
			{
				name: "order",
				description: "adalah angka: peringkat dalam daftar disortir secara menurun = 0 atau dihilangkan; peringkat dalam daftar disortir secara naik = sembarang nilai asal bukan nol"
			}
		]
	},
	{
		name: "RATE",
		description: "Menampilkan suku bunga tiap periode pinjaman atau investasi. Contoh, gunakan 6%/4 untuk pembayaran tiap kuartal pada APR 6%.",
		arguments: [
			{
				name: "nper",
				description: "adalah jumlah total periode pembayaran untuk pinjaman atau investasi"
			},
			{
				name: "pmt",
				description: "adalah pembayaran yang dilakukan tiap periode dan tidak dapat mengubah jangka waktu pinjaman atau investasi"
			},
			{
				name: "pv",
				description: "adalah nilai sekarang: jumlah total di mana seri pembayaran dimasa mendatang bermanfaat sekarang"
			},
			{
				name: "fv",
				description: "adalah nilai masa depan, atau saldo kas yang ingin Anda capai setelah pembayaran terakhir dilakukan. Jika dihilangkan, gunakan Fv = 0"
			},
			{
				name: "type",
				description: "adalah nilai logika: pembayaran di awal periode = 1; pembayaran di akhir periode = 0 atau dihilangkan"
			},
			{
				name: "guess",
				description: "adalah perkiraan Anda untuk harga akan menjadi berapa; jika dihilangkan, Guess = 0,1 (10 persen)"
			}
		]
	},
	{
		name: "RECEIVED",
		description: "Menampilkan jumlah yang diterima saat tanggal jatuh tempo untuk sekuritas investasi penuh.",
		arguments: [
			{
				name: "settlement",
				description: "adalah tanggal penetapan sekuritas, yang dinyatakan sebagai angka tanggal seri"
			},
			{
				name: "maturity",
				description: "adalah tanggal jatuh tempo sekuritas, yang dinyatakan sebagai angka tanggal seri"
			},
			{
				name: "investment",
				description: "adalah jumlah terinvestasi dalam sekuritas"
			},
			{
				name: "discount",
				description: "adalah tingkat potongan sekuritas"
			},
			{
				name: "basis",
				description: "adalah jenis dari basis penghitungan hari untuk digunakan"
			}
		]
	},
	{
		name: "REPLACE",
		description: "Mengganti bagian dari string teks dengan string teks yang lain.",
		arguments: [
			{
				name: "old_text",
				description: "adalah teks yang Anda ingin ganti beberapa karakternya"
			},
			{
				name: "start_num",
				description: "adalah posisi karakter dalam teks_Lama yang ingin Anda ganti dengan teks_Baru"
			},
			{
				name: "num_chars",
				description: "adalah jumlah karakter dalam teks_Lama yang ingin Anda ganti"
			},
			{
				name: "new_text",
				description: "adalah teks yang akan mengganti karakter dalam teks_Lama"
			}
		]
	},
	{
		name: "REPT",
		description: "Mengulangi teks sesuai yang ditentukan. Gunakan REPT untuk mengisi sel dengan sejumlah contoh string teks.",
		arguments: [
			{
				name: "text",
				description: "adalah teks yang ingin Anda ulangi"
			},
			{
				name: "number_times",
				description: "adalah angka positif yang menentukan berapa kali teks diulangi"
			}
		]
	},
	{
		name: "RIGHT",
		description: "Menampilkan jumlah karakter tertentu dari bagian akhir string teks.",
		arguments: [
			{
				name: "text",
				description: "adalah string teks yang mengandung karakter yang ingin Anda ekstrak"
			},
			{
				name: "num_chars",
				description: "menentukan berapa banyak karakter yang ingin Anda ekstrak; 1 jika dihilangkan"
			}
		]
	},
	{
		name: "ROMAN",
		description: "Mengonversi angka Arab ke Romawi, sebagai teks.",
		arguments: [
			{
				name: "number",
				description: "adalah angka Arab yang ingin Anda ubah"
			},
			{
				name: "form",
				description: "adalah angka yang menentukan tipe angka Romawi yang Anda inginkan."
			}
		]
	},
	{
		name: "ROUND",
		description: "Membulatkan angka ke jumlah digit yang ditentukan.",
		arguments: [
			{
				name: "number",
				description: "adalah angka yang ingin Anda bulatkan"
			},
			{
				name: "num_digits",
				description: "adalah jumlah digit yang ingin Anda bulatkan. Negatif dibulatkan ke kiri dari titik desimal; nol ke bilangan bulat terdekat"
			}
		]
	},
	{
		name: "ROUNDDOWN",
		description: "Membulatkan angka ke bawah, terhadap nol.",
		arguments: [
			{
				name: "number",
				description: "adalah angka riil yang ingin Anda bulatkan ke bawah"
			},
			{
				name: "num_digits",
				description: "adalah jumlah digit yang ingin Anda bulatkan. Negatif dibulatkan ke kiri dari titik desimal; nol atau dihilangkan, ke bilangan bulat terdekat"
			}
		]
	},
	{
		name: "ROUNDUP",
		description: "Membulatkan angka ke atas, menjauhi nol.",
		arguments: [
			{
				name: "number",
				description: "adalah angka riil yang ingin Anda bulatkan ke atas"
			},
			{
				name: "num_digits",
				description: "adalah jumlah digit ke mana Anda ingin bulatkan. Negatif dibulatkan ke kiri dari titik desimal; nol atau dihilangkan, ke bilangan bulat terdekat"
			}
		]
	},
	{
		name: "ROW",
		description: "Menampilkan nomor baris dari referensi.",
		arguments: [
			{
				name: "reference",
				description: "adalah sel atau rentang sel tunggal yang Anda ingin barisnya dinomori; jika dihilangkan, kembalikan sel yang mengandung fungsi BARIS"
			}
		]
	},
	{
		name: "ROWS",
		description: "Menampilkan jumlah baris dalam referensi atau array.",
		arguments: [
			{
				name: "array",
				description: "adalah array, rumus array, atau referensi ke rentang sel yang jumlah barisnya Anda inginkan"
			}
		]
	},
	{
		name: "RRI",
		description: "Menghasilkan suku bunga setara untuk pertumbuhan investasi.",
		arguments: [
			{
				name: "nper",
				description: "adalah jumlah periode investasi"
			},
			{
				name: "pv",
				description: "adalah nilai investasi saat ini"
			},
			{
				name: "fv",
				description: "adalah nilai investasi di masa depan"
			}
		]
	},
	{
		name: "RSQ",
		description: "Menampilkan kuadrat koefisien korelasi momen produk Pearson melalui poin data yang ditentukan.",
		arguments: [
			{
				name: "known_y's",
				description: "adalah array atau rentang poin data dan dapat berupa angka atau nama, array, atau referensi yang mengandung angka"
			},
			{
				name: "known_x's",
				description: "adalah array atau rentang poin data dan dapat berupa angka atau nama, array, atau referensi yang mengandung angka"
			}
		]
	},
	{
		name: "RTD",
		description: "Mengambil data waktu-nyata dari program yang mendukung otomatisasi COM.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "progID",
				description: "adalah nama dari ProgID dari COM add-in otomatisasi teregister. Kurung nama dalam tanda kutip"
			},
			{
				name: "server",
				description: "adalah nama server yang harus menjalankan add-in. Kurung nama dalam tanda kutip. Jika add-in berjalan secara lokal, gunakan string kosong"
			},
			{
				name: "topic1",
				description: "adalah parameter 1 sampai 38 yang menentukan sepotong data"
			},
			{
				name: "topic2",
				description: "adalah parameter 1 sampai 38 yang menentukan sepotong data"
			}
		]
	},
	{
		name: "SEARCH",
		description: "Menampilkan jumlah dari karakter saat karakter atau string teks tertentu ditemukan pertama kali, membaca dari kiri ke kanan (huruf besar dan kecil tidak dibedakan).",
		arguments: [
			{
				name: "find_text",
				description: "adalah teks yang ingin Anda temukan. Anda dapat menggunakan karakter bebas ? dan  *; gunakan ~? dan ~* untuk menemukan ? dan karakter *"
			},
			{
				name: "within_text",
				description: "adalah teks yang ingin Anda cari Temukan_teks"
			},
			{
				name: "start_num",
				description: "adalah angka karakter dalam Within_text, dihitung dari kiri, saat Anda ingin memulai pencarian. Jika dihilangkan, 1 digunakan"
			}
		]
	},
	{
		name: "SEC",
		description: "Menghasilkan sekan sudut.",
		arguments: [
			{
				name: "number",
				description: "adalah sudut dalam radian yang Anda inginkan sekannya"
			}
		]
	},
	{
		name: "SECH",
		description: "Menghasilkan sekan hiperbolik angka.",
		arguments: [
			{
				name: "number",
				description: "adalah sudut dalam radian yang Anda inginkan sekan hiperboliknya"
			}
		]
	},
	{
		name: "SECOND",
		description: "Menampilkan detik, angka dari 0 sampai 59.",
		arguments: [
			{
				name: "serial_number",
				description: "adalah angka dalam kode tanggal-waktu yang digunakan oleh Spreadsheet atau teks dalam format waktu, seperti 16:48:23 atau 4:48:47 PM"
			}
		]
	},
	{
		name: "SERIESSUM",
		description: "Menampilkan jumlah dari seri kekuatan berdasarkan pada rumus.",
		arguments: [
			{
				name: "x",
				description: "adalah masukan nilai ke seri kekuatan"
			},
			{
				name: "n",
				description: "adalah kekuatan awal untuk menaikkan x"
			},
			{
				name: "m",
				description: "adalah langkah guna meningkatkan n untuk tiap ketentuan dalam seri"
			},
			{
				name: "coefficients",
				description: "adalah set koefisien yang dikalikan dengan tiap kekuatan berurutan x"
			}
		]
	},
	{
		name: "SHEET",
		description: "Menghasilkan nomor lembar dari lembar yang diacu.",
		arguments: [
			{
				name: "value",
				description: "adalah nama lembar atau referensi yang Anda inginkan nomor lembarnya.  Jika dihilangkan, nomor lembar yang berisi fungsi tersebut dikembalikan"
			}
		]
	},
	{
		name: "SHEETS",
		description: "Menghasilkan jumlah lembar di referensi.",
		arguments: [
			{
				name: "reference",
				description: "adalah referensi yang ingin Anda ketahui jumlah lembar yang dikandungnya.  Jika dihilangkan, jumlah lembar di buku kerja yang berisi fungsi tersebut dikembalikan"
			}
		]
	},
	{
		name: "SIGN",
		description: "Menampilkan tanda dari angka: 1 jika angka adalah positif, nol jika angka adalah nol, atau -1 jika angka adalah negatif.",
		arguments: [
			{
				name: "number",
				description: "adalah angka riil apa saja"
			}
		]
	},
	{
		name: "SIN",
		description: "Menampilkan sinus dari sudut.",
		arguments: [
			{
				name: "number",
				description: "adalah sudut dalam radian sinus yang Anda inginkan. Radian = derajat * PI()/180"
			}
		]
	},
	{
		name: "SINH",
		description: "Menampilkan sinus hiperbolik dari angka.",
		arguments: [
			{
				name: "number",
				description: "adalah angka riil apa saja"
			}
		]
	},
	{
		name: "SKEW",
		description: "Menampilkan kecondongan distribusi: karakterisasi derajat asimetri dari distribusi disekitar nilai rata-ratanya.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "adalah angka atau nama, array, atau referensi 1 sampai 255 yang mengandung angka yang kecondongannya Anda inginkan"
			},
			{
				name: "number2",
				description: "adalah angka atau nama, array, atau referensi 1 sampai 255 yang mengandung angka yang kecondongannya Anda inginkan"
			}
		]
	},
	{
		name: "SKEW.P",
		description: "Menghasilkan kecondongan distribusi berdasarkan populasi: karakterisasi derajat asimetri dari distribusi di sekitar nilai rata-ratanya.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "adalah angka 1 sampai 254 atau nama, array, atau referensi yang berisi angka yang kecondongan populasinya Anda inginkan"
			},
			{
				name: "number2",
				description: "adalah angka 1 sampai 254 atau nama, array, atau referensi yang berisi angka yang kecondongan populasinya Anda inginkan"
			}
		]
	},
	{
		name: "SLN",
		description: "Menampilkan depresiasi garis-lurus modal untuk satu periode.",
		arguments: [
			{
				name: "cost",
				description: "adalah nilai awal aset"
			},
			{
				name: "salvage",
				description: "adalah nilai penyusutan pada nilai akhir aset"
			},
			{
				name: "life",
				description: "adalah jumlah periode saat aset didepresiasikan (kadang-kadang disebut nilai jual aset)"
			}
		]
	},
	{
		name: "SLOPE",
		description: "Menampilkan kemiringan garis regresi linear melalui poin data yang ditentukan.",
		arguments: [
			{
				name: "known_y's",
				description: "adalah array atau rentang sel poin data dependen numerik dan dapat berupa angka atau nama, array, atau referensi yang mengandung angka"
			},
			{
				name: "known_x's",
				description: "adalah perangkat poin data independen dan dapat berupa angka atau nama, array, atau referensi yang mengandung angka"
			}
		]
	},
	{
		name: "SMALL",
		description: "Menampilkan nilai k-th terkecil dalam perangkat data. Contoh, angka terkecil kelima.",
		arguments: [
			{
				name: "array",
				description: "adalah array atau rentang data numerik di mana Anda ingin menentukan nilai k-th terkecil tersebut"
			},
			{
				name: "k",
				description: "adalah posisi (dari yang terkecil) dalam array atau rentang dari nilai untuk dikembalikan"
			}
		]
	},
	{
		name: "SQRT",
		description: "Menampilkan akar pangkat dua dari angka.",
		arguments: [
			{
				name: "number",
				description: "adalah angka akar kuadrat yang Anda inginkan"
			}
		]
	},
	{
		name: "SQRTPI",
		description: "Menampilkan akar pangkat dua dari (bilangan * Pi).",
		arguments: [
			{
				name: "number",
				description: "adalah bilangan yang dikalikan p"
			}
		]
	},
	{
		name: "STANDARDIZE",
		description: "Menampilkan nilai yang dinormalisasi dari distribusi terkarakterisasi oleh nilai rata-rata dan simpangan baku.",
		arguments: [
			{
				name: "x",
				description: "adalah nilai yang ingin Anda normalisasi"
			},
			{
				name: "mean",
				description: "adalah nilai rata-rata aritmatika distribusi"
			},
			{
				name: "standard_dev",
				description: "adalah simpangan baku dari distribusi, angka positif"
			}
		]
	},
	{
		name: "STDEV",
		description: "Memperkirakan simpangan baku berdasarkan pada contoh (abaikan nilai dan teks logis dalam contoh).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "adalah angka 1 sampai 255 yang bersesuaian dengan contoh dari populasi dan dapat berupa angka atau referensi yang mengandung angka"
			},
			{
				name: "number2",
				description: "adalah angka 1 sampai 255 yang bersesuaian dengan contoh dari populasi dan dapat berupa angka atau referensi yang mengandung angka"
			}
		]
	},
	{
		name: "STDEV.P",
		description: "Menghitung simpangan baku berdasarkan pada seluruh populasi yang ditentukan sebagai argumen (abaikan nilai dan teks logis).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "adalah angka 1 sampai 255 yang berhubungan ke populasi dan dapat berupa angka atau referensi yang mengandung angka"
			},
			{
				name: "number2",
				description: "adalah angka 1 sampai 255 yang berhubungan ke populasi dan dapat berupa angka atau referensi yang mengandung angka"
			}
		]
	},
	{
		name: "STDEV.S",
		description: "Memperkirakan simpangan baku berdasarkan pada contoh (mengabaikan nilai dan teks logis dalam contoh).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "adalah angka 1 sampai 255 yang berhubungan dengan contoh dari populasi dan dapat berupa angka atau referensi yang mengandung angka"
			},
			{
				name: "number2",
				description: "adalah angka 1 sampai 255 yang berhubungan dengan contoh dari populasi dan dapat berupa angka atau referensi yang mengandung angka"
			}
		]
	},
	{
		name: "STDEVA",
		description: "Memperkirakan simpangan baku berdasarkan contoh, termasuk nilai dan teks logis. Teks dan nilai logis SALAH memiliki nilai 0; nilai logis BENAR memiliki nilai 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "adalah nilai 1 sampai 255 yang berhubungan dengan contoh dari populasi dan dapat berupa nilai atau nama atau referensi ke nilai"
			},
			{
				name: "value2",
				description: "adalah nilai 1 sampai 255 yang berhubungan dengan contoh dari populasi dan dapat berupa nilai atau nama atau referensi ke nilai"
			}
		]
	},
	{
		name: "STDEVP",
		description: "Menghitung simpangan baku berdasarkan pada seluruh populasi yang ditentukan sebagai argumen (abaikan nilai dan teks logis).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "adalah angka 1 sampai 255 yang bersesuaian dengan populasi dan dapat berupa angka atau referensi yang mengandung angka"
			},
			{
				name: "number2",
				description: "adalah angka 1 sampai 255 yang bersesuaian dengan populasi dan dapat berupa angka atau referensi yang mengandung angka"
			}
		]
	},
	{
		name: "STDEVPA",
		description: "Menghitung simpangan baku berdasarkan seluruh populasi, termasuk nilai dan teks logis. Teks dan nilai logis SALAH bernilai 0; nilai logis BENAR bernilai 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "adalah nilai 1 sampai 255 yang berhubungan ke populasi dan dapat berupa nilai, nama array atau referensi yang mengandung nilai"
			},
			{
				name: "value2",
				description: "adalah nilai 1 sampai 255 yang berhubungan ke populasi dan dapat berupa nilai, nama array atau referensi yang mengandung nilai"
			}
		]
	},
	{
		name: "STEYX",
		description: "Mengembalikan kesalahan stAndar dari nilai-y yang diperkirakan untuk tiap x dalam regresi.",
		arguments: [
			{
				name: "known_y's",
				description: "adalah array atau rentang poin data dependen dan dapat berupa angka atau nama, array, atau referensi yang mengandung angka"
			},
			{
				name: "known_x's",
				description: "adalah array atau rentang poin data independen dan dapat berupa angka atau nama, array, atau referensi yang mengandung angka"
			}
		]
	},
	{
		name: "SUBSTITUTE",
		description: "Mengganti teks yang ada dengan teks baru dalam string teks.",
		arguments: [
			{
				name: "text",
				description: "adalah teks atau referensi ke sel yang mengandung teks yang ingin Anda ganti karakternya"
			},
			{
				name: "old_text",
				description: "adalah teks yang ada yang ingin Anda ganti. Jika huruf teks_Lama tidak cocok dengan huruf teks, SUBSTITUTE tidak akan mengganti teks tersebut"
			},
			{
				name: "new_text",
				description: "adalah teks yang ingin Anda ganti dengan teks_Lama"
			},
			{
				name: "instance_num",
				description: "tentukan kemunculan teks_Lama yang mana yang ingin Anda ganti. Jika dihilangkan, tiap contoh dari teks_Lama akan diganti"
			}
		]
	},
	{
		name: "SUBTOTAL",
		description: "Menampilkan subtotal dalam daftar atau database.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "function_num",
				description: "adalah angka 1 sampai 11 yang menentukan fungsi ringkasan untuk subtotal."
			},
			{
				name: "ref1",
				description: "adalah rentang atau referensi 1 sampai 254 yang subtotalnya Anda inginkan"
			}
		]
	},
	{
		name: "SUM",
		description: "Menambah semua angka dalam rentang sel.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "adalah angka 1 sampai 255 untuk dijumlahkan. Nilai dan teks logis diabaikan dalam sel, termasuk jika diketikkan sebagai argumen"
			},
			{
				name: "number2",
				description: "adalah angka 1 sampai 255 untuk dijumlahkan. Nilai dan teks logis diabaikan dalam sel, termasuk jika diketikkan sebagai argumen"
			}
		]
	},
	{
		name: "SUMIF",
		description: "Menambah sel yang ditentukan oleh kondisi atau kriteria tertentu.",
		arguments: [
			{
				name: "range",
				description: "adalah rentang sel yang ingin Anda evaluasi"
			},
			{
				name: "criteria",
				description: "adalah kondisi atau kriteria dalam bentuk angka, ekspresi, atau teks yang menetapkan sel mana yang akan ditambahkan"
			},
			{
				name: "sum_range",
				description: "adalah sel sebenarnya untuk dijumlahkan. Jika dihilangkan, sel dalam rentang akan digunakan"
			}
		]
	},
	{
		name: "SUMIFS",
		description: "Tambahkan sel yang ditentukan oleh pemberian set aturan atau kriteria.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "sum_range",
				description: "adalah sel sesungguhnya untuk dijumlahkan."
			},
			{
				name: "criteria_range",
				description: "adalah rentang sel yang Anda ingin evaluasi untuk kondisi tertentu"
			},
			{
				name: "criteria",
				description: "adalah kondisi atau kriteria dalam formulir dari bilangan, ekspresi, atau teks yang menetapkan sel mana yang akan ditambah"
			}
		]
	},
	{
		name: "SUMPRODUCT",
		description: "Menampilkan penjumlahan produk dari rentang atau array terkait.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "array1",
				description: "adalah array 2 sampai 255 yang ingin Anda kalikan dan tambahkan komponen. Semua array harus berdimensi sama"
			},
			{
				name: "array2",
				description: "adalah array 2 sampai 255 yang ingin Anda kalikan dan tambahkan komponen. Semua array harus berdimensi sama"
			},
			{
				name: "array3",
				description: "adalah array 2 sampai 255 yang ingin Anda kalikan dan tambahkan komponen. Semua array harus berdimensi sama"
			}
		]
	},
	{
		name: "SUMSQ",
		description: "Menampilkan jumlah kuadrat argumen. Argumen dapat berupa angka, array, nama, atau referensi ke sel yang mengandung angka.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "adalah angka, array, nama, atau referensi ke array 1 sampai 255 yang jumlah kuadratnya Anda inginkan"
			},
			{
				name: "number2",
				description: "adalah angka, array, nama, atau referensi ke array 1 sampai 255 yang jumlah kuadratnya Anda inginkan"
			}
		]
	},
	{
		name: "SUMX2MY2",
		description: "Menjumlahkan perbedaan di antara kuadrat dua rentang array yang berhubungan.",
		arguments: [
			{
				name: "array_x",
				description: "adalah rentang atau array angka pertama dan dapat berupa angka atau nama, array, atau referensi yang mengandung angka"
			},
			{
				name: "array_y",
				description: "adalah rentang atau array angka kedua dan dapat berupa angka atau nama, array, atau referensi yang mengandung angka"
			}
		]
	},
	{
		name: "SUMX2PY2",
		description: "Menampilkan total jumlah penjumlahan kuadrat angka dalam dua rentang array berhubungan.",
		arguments: [
			{
				name: "array_x",
				description: "adalah rentang atau array angka pertama dan dapat berupa angka atau nama, array, atau referensi yang mengandung angka"
			},
			{
				name: "array_y",
				description: "adalah rentang atau array angka kedua dan dapat berupa angka atau nama, array, atau referensi yang mengandung angka"
			}
		]
	},
	{
		name: "SUMXMY2",
		description: "Menjumlahkan kuadrat perbedaan dalam dua rentang array yang berhubungan.",
		arguments: [
			{
				name: "array_x",
				description: "adalah rentang atau array nilai pertama dan dapat berupa angka atau nama, array, atau referensi yang mengandung angka"
			},
			{
				name: "array_y",
				description: "adalah rentang atau array nilai kedua dan dapat berupa angka atau nama, array, atau referensi yang mengandung angka"
			}
		]
	},
	{
		name: "SYD",
		description: "Menampilkan jumlah depresiasi digit tahun dari aset untuk periode tertentu.",
		arguments: [
			{
				name: "cost",
				description: "adalah nilai awal aset"
			},
			{
				name: "salvage",
				description: "adalah nilai penyusutan pada nilai akhir aset"
			},
			{
				name: "life",
				description: "adalah jumlah periode saat nilai aset didepresiasikan (kadang-kadang disebut nilai jual aset)"
			},
			{
				name: "per",
				description: "adalah periode dan harus menggunakan unit yang sama dengan Life"
			}
		]
	},
	{
		name: "T",
		description: "Mengecek apakah nilai adalah teks, dan mengembalikan teksnya jika benar, atau mengembalikan tanda petik ganda (teks kosong) jika tidak benar.",
		arguments: [
			{
				name: "value",
				description: "adalah nilai untuk dites"
			}
		]
	},
	{
		name: "T.DIST",
		description: "Menghasilkan distribusi-t Pelajar lemparan-kiri.",
		arguments: [
			{
				name: "x",
				description: "adalah nilai numerik untuk mengevaluasi distribusi"
			},
			{
				name: "deg_freedom",
				description: "adalah bilangan bulat yang mengindikasikan jumlah pangkat yang mengarakterisasi distribusi"
			},
			{
				name: "cumulative",
				description: "adalah nilai logis: untuk fungsi distribusi kumulatif, gunakan TRUE; untuk fungsi kerapatan probabilitas, gunakan FALSE"
			}
		]
	},
	{
		name: "T.DIST.2T",
		description: "Menghasilkan distribusi-t Pelajar dua-lemparan.",
		arguments: [
			{
				name: "x",
				description: "adalah nilai numerik untuk mengevaluasi distribusi"
			},
			{
				name: "deg_freedom",
				description: "adalah bilangan bulat yang mengindikasikan jumlah pangkat yang mengarakterisasi distribusi"
			}
		]
	},
	{
		name: "T.DIST.RT",
		description: "Menghasilkan distribusi-t Pelajar lemparan-kanan.",
		arguments: [
			{
				name: "x",
				description: "adalah nilai numerik untuk mengevaluasi distribusi"
			},
			{
				name: "deg_freedom",
				description: "adalah bilangan bulat yang mengindikasikan jumlah pangkat yang mengarakterisasi distribusi"
			}
		]
	},
	{
		name: "T.INV",
		description: "Menghasilkan balikan lemparan-kiri dari distribusi-t Pelajar.",
		arguments: [
			{
				name: "probability",
				description: "adalah probabilitas yang terkait dengan distribusi-t Pelajar dua-lemparan, angka di antara 0 sampai dengan 1"
			},
			{
				name: "deg_freedom",
				description: "adalah bilangan bulat positif yang mengindikasikan jumlah pangkat untuk mengarakterisasi distribusi"
			}
		]
	},
	{
		name: "T.INV.2T",
		description: "Menghasilkan balikan dua-lemparan dari distribusi-t Pelajar.",
		arguments: [
			{
				name: "probability",
				description: "adalah probabilitas yang terkait dengan distribusi-t Pelajar dua-lemparan, angka di antara 0 sampai dengan 1"
			},
			{
				name: "deg_freedom",
				description: "adalah bilangan bulat positif yang mengindikasikan jumlah pangkat untuk mengarakterisasi distribusi"
			}
		]
	},
	{
		name: "T.TEST",
		description: "Menampilkan probabilitas yang berhubungan dengan Uji-t Pelajar.",
		arguments: [
			{
				name: "array1",
				description: "adalah perangkat data pertama"
			},
			{
				name: "array2",
				description: "adalah perangkat data kedua"
			},
			{
				name: "tails",
				description: "menentukan jumlah arah distribusi untuk dikembalikan: distribusi satu lemparan = 1; distribusi dua lemparan = 2"
			},
			{
				name: "type",
				description: "adalah jenis uji-t: berpasangan = 1, variansi dua-contoh yang sama (homoscedastic) = 2, variansi dua-contoh yang tidak sama = 3"
			}
		]
	},
	{
		name: "TAN",
		description: "Menampilkan tangen dari sudut.",
		arguments: [
			{
				name: "number",
				description: "Adalah sudut dalam radian tangen yang Anda inginkan. Radian = derajat * PI()/180"
			}
		]
	},
	{
		name: "TANH",
		description: "Menampilkan tangen hiperbolik dari angka.",
		arguments: [
			{
				name: "number",
				description: "adalah angka riil apa saja"
			}
		]
	},
	{
		name: "TBILLEQ",
		description: "Menampilkan hasil setara obligasi untuk surat utang.",
		arguments: [
			{
				name: "settlement",
				description: " adalah tanggal pelunasan surat utang, yang dinyatakan sebagai angka tanggal seri"
			},
			{
				name: "maturity",
				description: "adalah tanggal jatuh tempo Surat utang, yang dinyatakan sebagai angka tanggal seri"
			},
			{
				name: "discount",
				description: "adalah tingkat diskon surat utang"
			}
		]
	},
	{
		name: "TBILLPRICE",
		description: "Menampilkan harga per nilai awal $100 untuk surat utang.",
		arguments: [
			{
				name: "settlement",
				description: "adalah tanggal pelunasan Surat utang, yang dinyatakan sebagai angka tanggal seri"
			},
			{
				name: "maturity",
				description: "adalah tanggal jatuh tempo Surat Utang, yang dinyatakan sebagai angka tanggal seri"
			},
			{
				name: "discount",
				description: "adalah tingkat diskon Surat utang"
			}
		]
	},
	{
		name: "TBILLYIELD",
		description: "Menampilkan hasil untuk Surat utang.",
		arguments: [
			{
				name: "settlement",
				description: "adalah tanggal pelunasan surat utang, yang dinyatakan sebagai angka tanggal seri"
			},
			{
				name: "maturity",
				description: "adalah tanggal jatuh tempo Surat Utang, yang dinyatakan sebagai angka tanggal seri"
			},
			{
				name: "pr",
				description: "adalah harga Surat Utang per nilai awal $100"
			}
		]
	},
	{
		name: "TDIST",
		description: "Menampilkan distribusi-t Pelajar.",
		arguments: [
			{
				name: "x",
				description: "adalah nilai angka untuk mengevaluasi distribusi"
			},
			{
				name: "deg_freedom",
				description: "adalah bilangan bulat yang mengindikasikan jumlah pangkat yang mengkarakterisasi distribusi"
			},
			{
				name: "tails",
				description: "tentukan jumlah arah distribusi untuk dikembalikan: distribusi satu lemparan = 1; distribusi dua lemparan = 2"
			}
		]
	},
	{
		name: "TEXT",
		description: "Mengonversi nilai ke teks dalam format nomor tertentu.",
		arguments: [
			{
				name: "value",
				description: "adalah angka, rumus yang mengevaluasi ke nilai numerik, atau referensi ke sel yang mengandung nilai numerik"
			},
			{
				name: "format_text",
				description: "adalah format nomor dalam formulir teks dari kotak Kategori pada tab Angka dalam kotak dialog Format Sel (bukan Umum)"
			}
		]
	},
	{
		name: "TIME",
		description: "Konversi jam, menit, dan detik yang ditentukan sebagai angka ke nomor seri Spreadsheet, diformat dengan format waktu.",
		arguments: [
			{
				name: "hour",
				description: "adalah angka dari 0 sampai 23 yang menunjukkan jam"
			},
			{
				name: "minute",
				description: "adalah angka dari 0 sampai 59 yang menunjukkan menit"
			},
			{
				name: "second",
				description: "adalah angka dari 0 sampai 59 yang menunjukkan detik"
			}
		]
	},
	{
		name: "TIMEVALUE",
		description: "Mengonversi waktu teks ke nomor seri tanggal Spreadsheet, angka dari 0 (12:00:00 AM) sampai 0.999988426 (11:59:59 PM). Format nomor dengan format waktu setelah memasukkan rumus.",
		arguments: [
			{
				name: "time_text",
				description: "adalah string teks yang menampilkan waktu pada tiap format waktu Spreadsheet (informasi tanggal dalam string diabaikan)"
			}
		]
	},
	{
		name: "TINV",
		description: "Menampilkan invers distribusi-t Pelajar.",
		arguments: [
			{
				name: "probability",
				description: "adalah probabilitas yang berhubungan dengan distribusi-t Pelajar dua-lemparan, angka di antara 0 sampai dengan 1"
			},
			{
				name: "deg_freedom",
				description: "adalah bilangan bulat positif yang mengindikasikan jumlah pangkat untuk mengkarakterisasi distribusi"
			}
		]
	},
	{
		name: "TODAY",
		description: "Menampilkan format tanggal sekarang sebagai tanggal.",
		arguments: [
		]
	},
	{
		name: "TRANSPOSE",
		description: "Mengonversi rentang sel vertikal ke rentang horizontal, atau sebaliknya.",
		arguments: [
			{
				name: "array",
				description: "adalah rentang sel pada lembar kerja atau array nilai yang ingin Anda ubah urutannya"
			}
		]
	},
	{
		name: "TREND",
		description: "Menampilkan statistik yang menggambarkan pencocokan tren linear dengan poin data yang dikenal, menggunakan metode kuadrat terkecil.",
		arguments: [
			{
				name: "known_y's",
				description: "adalah rentang atau array nilai-y yang telah Anda ketahui dalam hubungan y = mx + b"
			},
			{
				name: "known_x's",
				description: "adalah rentang opsional atau array nilai-x yang Anda ketahui dalam hubungan y = mx + b, array berukuran sama seperti y_yang Dikenal"
			},
			{
				name: "new_x's",
				description: "adalah rentang atau array nilai-x yang baru yang Anda ingin TREND mengembalikan nilai-y yang berhubungan"
			},
			{
				name: "const",
				description: "adalah nilai logika: konstanta b dihitung secara normal jika Const = BENAR atau dihilangkan; b ditata sama dengan 0 jika Const = SALAH"
			}
		]
	},
	{
		name: "TRIM",
		description: "Menghapus semua spasi dari string teks kecuali untuk spasi tunggal di antara kata.",
		arguments: [
			{
				name: "text",
				description: "adalah teks yang ingin Anda hapus spasinya"
			}
		]
	},
	{
		name: "TRIMMEAN",
		description: "Menampilkan nilai rata-rata bagian interior dari perangkat nilai data.",
		arguments: [
			{
				name: "array",
				description: "adalah rentang atau array nilai untuk dipotong dan dirata-rata"
			},
			{
				name: "percent",
				description: "adalah sejumlah kecil poin data untuk dikeluarkan dari bagian atas dan bawah perangkat data"
			}
		]
	},
	{
		name: "TRUE",
		description: "Menampilkan nilai logika BENAR.",
		arguments: [
		]
	},
	{
		name: "TRUNC",
		description: "Memotong angka ke bilangan bulat dengan menghapus desimalnya, atau pecahan, bagian dari angka tersebut.",
		arguments: [
			{
				name: "number",
				description: "adalah angka yang ingin Anda potong"
			},
			{
				name: "num_digits",
				description: "adalah angka yang menentukan presisi pemotongan, 0 (nol) jika diabaikan"
			}
		]
	},
	{
		name: "TTEST",
		description: "Menampilkan probabilitas yang berhubungan dengan Uji-t Pelajar.",
		arguments: [
			{
				name: "array1",
				description: "adalah perangkat data pertama"
			},
			{
				name: "array2",
				description: "adalah perangkat data kedua"
			},
			{
				name: "tails",
				description: "tentukan jumlah arah distribusi untuk dikembalikan: distribusi satu lemparan = 1; distribusi dua lemparan = 2"
			},
			{
				name: "type",
				description: "adalah jenis uji-t: berpasangan = 1, varian dua-contoh yang sama (homoscedastic) = 2, variansi dua-contoh yang tidak sama = 3"
			}
		]
	},
	{
		name: "TYPE",
		description: "Menghasilkan bilangan bulat yang menunjukkan tipe data nilai: angka = 1; teks = 2; nilai logis = 4; nilai kesalahan = 16; array = 64.",
		arguments: [
			{
				name: "value",
				description: "boleh berupa sembarang nilai"
			}
		]
	},
	{
		name: "UNICODE",
		description: "Mengembalikan angka (poin kode) yang sesuai dengan karakter pertama teks.",
		arguments: [
			{
				name: "text",
				description: "adalah karakter yang Anda inginkan nilai Unicode-nya"
			}
		]
	},
	{
		name: "UPPER",
		description: "Mengonversi string teks menjadi huruf besar semua.",
		arguments: [
			{
				name: "text",
				description: "adalah teks yang ingin Anda ubah ke huruf besar, referensi atau string teks"
			}
		]
	},
	{
		name: "VALUE",
		description: "Mengonversi string teks yang menunjukkan angka ke angka.",
		arguments: [
			{
				name: "text",
				description: "adalah teks yang dilampirkan dalam tanda kutip atau referensi ke sel yang mengandung teks yang ingin Anda ubah"
			}
		]
	},
	{
		name: "VAR",
		description: "Memperkirakan varian berdasarkan contoh (abaikan nilai dan teks logis dalam contoh).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "adalah argumen angka dari 1 sampai 255 yang bersesuaian dengan contoh dari populasi"
			},
			{
				name: "number2",
				description: "adalah argumen angka dari 1 sampai 255 yang bersesuaian dengan contoh dari populasi"
			}
		]
	},
	{
		name: "VAR.P",
		description: "Menghitung varians berdasarkan seluruh populasi (mengabaikan nilai dan teks logis dalam populasi).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "adalah argumen numerik 1 sampai 255 yang berhubungan dengan populasi"
			},
			{
				name: "number2",
				description: "adalah argumen numerik 1 sampai 255 yang berhubungan dengan populasi"
			}
		]
	},
	{
		name: "VAR.S",
		description: "Memperkirakan variansi berdasarkan contoh (mengabaikan nilai dan teks logis dalam contoh).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "adalah argumen numerik dari 1 sampai 255 yang berhubungan dengan contoh dari populasi"
			},
			{
				name: "number2",
				description: "adalah argumen numerik dari 1 sampai 255 yang berhubungan dengan contoh dari populasi"
			}
		]
	},
	{
		name: "VARA",
		description: "Memperkirakan varians berdasarkan contoh, termasuk nilai dan teks logis. Teks dan nilai logis SALAH memiliki nilai 0; nilai logis BENAR memiliki nilai 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "adalah argumen nilai 1 sampai 255 yang berhubungan dengan contoh dari populasi"
			},
			{
				name: "value2",
				description: "adalah argumen nilai 1 sampai 255 yang berhubungan dengan contoh dari populasi"
			}
		]
	},
	{
		name: "VARP",
		description: "Menghitung variansi berdasarkan seluruh populasi (mengabaikan nilai dan teks logis dalam populasi).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "number1",
				description: "adalah argumen angka 1 sampai 255 yang bersesuaian dengan populasi"
			},
			{
				name: "number2",
				description: "adalah argumen angka 1 sampai 255 yang bersesuaian dengan populasi"
			}
		]
	},
	{
		name: "VARPA",
		description: "Menghitung varians berdasarkan pada seluruh populasi, termasuk nilai dan teks logis. Teks dan nilai logis SALAH bernilai 0; nilai logis BENAR bernilai 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "value1",
				description: "adalah argumen nilai 1 sampai 255 yang berhubungan dengan populasi"
			},
			{
				name: "value2",
				description: "adalah argumen nilai 1 sampai 255 yang berhubungan dengan populasi"
			}
		]
	},
	{
		name: "VDB",
		description: "Menampilkan depresiasi aset untuk periode yang Anda tentukan, termasuk periode parsial, menggunakan metode saldo pengurangan-ganda atau beberapa metode lain yang Anda tentukan.",
		arguments: [
			{
				name: "cost",
				description: "adalah nilai awal aset"
			},
			{
				name: "salvage",
				description: "adalah akumulasi penyusutan nilai pada akhir jangka waktu aset"
			},
			{
				name: "life",
				description: "adalah jumlah periode pendepresiasian aset (kadang-kadang disebut nilai jual aset)"
			},
			{
				name: "start_period",
				description: "adalah periode awal penghitungan depresiasi, dalam unit yang sama dengan Life"
			},
			{
				name: "end_period",
				description: "adalah periode akhir penghitungan depresiasi, dalam unit yang sama dengan Life"
			},
			{
				name: "factor",
				description: "adalah laju pengurangan saldo, 2 (saldo pengurangan-ganda) jika dihilangkan"
			},
			{
				name: "no_switch",
				description: "ganti ke depresiasi garis-lurus ketika depresiasi lebih besar dari saldo pengurangan = SALAH atau dihilangkan; jangan ganti  = BENAR"
			}
		]
	},
	{
		name: "VLOOKUP",
		description: "Mencari nilai dalam kolom terkiri dari tabel, kemudian mengembalikan nilai dalam baris yang sama dengan kolom yang Anda tentukan. Secara default, tabel harus diurutkan dalam urutan naik.",
		arguments: [
			{
				name: "lookup_value",
				description: "adalah nilai yang dapat ditemukan dalam kolom pertama dari tabel, dan dapat berupa nilai, referensi, atau string teks"
			},
			{
				name: "table_array",
				description: "adalah tabel teks, angka, atau nilai logis tempat data diambil. Tabel_array dapat direferensikan ke rentang atau nama rentang"
			},
			{
				name: "col_index_num",
				description: "adalah nomor kolom dalam tabel_array yang jika nilainya cocok harus dikembalikan. Kolom pertama dari nilai dalam tabel adalah kolom 1"
			},
			{
				name: "range_lookup",
				description: "adalah nilai logis: untuk menemukan yang paling mendekati kecocokannya pada kolom pertama (urutkan dalam urutan naik) = BENAR atau dihilangkan; temukan yang benar-benar cocok = SALAH"
			}
		]
	},
	{
		name: "WEEKDAY",
		description: "Menampilkan angka dari 1 sampai 7 untuk menunjukkan hari dari minggu dari tanggal.",
		arguments: [
			{
				name: "serial_number",
				description: "adalah angka yang menunjukkan tanggal"
			},
			{
				name: "return_type",
				description: "adalah angka: untuk Minggu=1 sampai Sabtu=7, gunakan 1; untuk Senin=1 sampai Minggu=7, gunakan 2; untuk Senin=0 sampai Minggu=6, gunakan 3"
			}
		]
	},
	{
		name: "WEEKNUM",
		description: "Menampilkan jumlah minggu dalam tahun.",
		arguments: [
			{
				name: "serial_number",
				description: "adalah kode tanggal-waktu yang digunakan oleh Spreadsheet untuk perhitungan tanggal dan waktu"
			},
			{
				name: "return_type",
				description: "adalah bilangan (1 atau 2) yang menentukan jenis nilai imbal hasil"
			}
		]
	},
	{
		name: "WEIBULL",
		description: "Menampilkan distribusi Weibull.",
		arguments: [
			{
				name: "x",
				description: "adalah nilai yang ingin Anda gunakan untuk mengevaluasi fungsi, angka bukan negatif"
			},
			{
				name: "alpha",
				description: "adalah parameter untuk distribusi, yaitu angka positif"
			},
			{
				name: "beta",
				description: "adalah parameter untuk distribusi, yaitu angka positif"
			},
			{
				name: "cumulative",
				description: "adalah nilai logis: untuk fungsi distribusi kumulatif, gunakan BENAR; untuk fungsi massa probabilitas, gunakan SALAH"
			}
		]
	},
	{
		name: "WEIBULL.DIST",
		description: "Menampilkan distribusi Weibull.",
		arguments: [
			{
				name: "x",
				description: "adalah nilai yang ingin Anda gunakan untuk mengevaluasi fungsi, angka bukan negatif"
			},
			{
				name: "alpha",
				description: "adalah parameter untuk distribusi, yaitu angka positif"
			},
			{
				name: "beta",
				description: "adalah parameter untuk distribusi, yaitu angka positif"
			},
			{
				name: "cumulative",
				description: "adalah nilai logis: untuk fungsi distribusi kumulatif, gunakan BENAR; untuk fungsi massa probabilitas, gunakan SALAH"
			}
		]
	},
	{
		name: "WORKDAY",
		description: "Menampilkan angka seri tanggal sebelum atau sesudah dari bilangan tertentu dari hari kerja.",
		arguments: [
			{
				name: "start_date",
				description: " adalah angka seri tanggal yang mewakili tanggal mulai"
			},
			{
				name: "days",
				description: "adalah angka dari hari bukan akhir pekan dan bukan hari libur sebelum atau setelah start_data"
			},
			{
				name: "holidays",
				description: "merupakan array opsional dari satu atau lebih angka tanggal seri yang dikeluarkan dari kalender kerja, seperti hari libur negara bagian dan federal dan liburan tertunda"
			}
		]
	},
	{
		name: "WORKDAY.INTL",
		description: "Menghasilkan jumlah seluruh hari kerja antara dua tanggal dengan parameter akhir pekan kustom.",
		arguments: [
			{
				name: "start_date",
				description: "adalah angka tanggal seri yang menunjukkan tanggal mulai"
			},
			{
				name: "days",
				description: "adalah jumlah hari non-akhir pekan dan non-liburan sebelum atau setelah tanggal_mulai"
			},
			{
				name: "weekend",
				description: "adalah angka atau string yang menentukan saat akhir pekan berlangsung"
			},
			{
				name: "holidays",
				description: "adalah array opsional dari satu atau beberapa angka tanggal seri untuk dikeluarkan dari kalender kerja, seperti libur umum dan nasional serta liburan tertunda"
			}
		]
	},
	{
		name: "XIRR",
		description: "Menampilkan tingkat internal dari pengembalian untuk jadwal arus kas.",
		arguments: [
			{
				name: "values",
				description: "adalah seri arus kas yang terkait dengan jadwal pembayaran dalam tanggal"
			},
			{
				name: "dates",
				description: "adalah jadwal tanggal pembayaran yang berhubungan dengan arus kas pembayaran"
			},
			{
				name: "guess",
				description: "adalah bilangan yang Anda perkirakan mendekati hasil XIRR"
			}
		]
	},
	{
		name: "XNPV",
		description: "Menampilkan nilai bersih terkini untuk jadwal arus kas.",
		arguments: [
			{
				name: "rate",
				description: "adalah tingkat diskon untuk diterapkan pada arus kas"
			},
			{
				name: "values",
				description: "adalah seri arus kas yang berhubungan dengan jadwal pembayaran dalam tanggal"
			},
			{
				name: "dates",
				description: "adalah jadwal tanggal pembayaran yang berhubungan pada pembayaran arus kas"
			}
		]
	},
	{
		name: "XOR",
		description: "Menghasilkan 'Eksklusif Atau' logis dari semua argumen.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logical1",
				description: "adalah 1 hingga 254 syarat yang ingin Anda uji yang boleh saja TRUE atau FALSE dan bisa saja berupa nilai logis, array, atau referensi"
			},
			{
				name: "logical2",
				description: "adalah 1 hingga 254 syarat yang ingin Anda uji yang boleh saja TRUE atau FALSE dan bisa saja berupa nilai logis, array, atau referensi"
			}
		]
	},
	{
		name: "YEAR",
		description: "Menampilkan tahun dari tanggal, bilangan bulat dalam rentang 1900 - 9999.",
		arguments: [
			{
				name: "serial_number",
				description: "adalah angka dalam kode tanggal-waktu yang digunakan oleh Spreadsheet"
			}
		]
	},
	{
		name: "YEARFRAC",
		description: "Menampilkan pecahan tahun yang mewakili angka seluruh hari antara tanggal_mulai dan tanggal_akhir.",
		arguments: [
			{
				name: "start_date",
				description: "adalah angka tanggal seri yang mewakili tanggal mulai"
			},
			{
				name: "end_date",
				description: "adalah angka tanggal seri yang mewakili tanggal akhir"
			},
			{
				name: "basis",
				description: "adalah tipe basis hitungan hari untuk digunakan"
			}
		]
	},
	{
		name: "YIELDDISC",
		description: "Menampilkan hasil tahunan untuk sekuritas terdiskon. Sebagai contoh, sekuritas terdiskon.",
		arguments: [
			{
				name: "settlement",
				description: "adalah tanggal pelunasan sekuritas, yang dinyatakan sebagai angka tanggal seri"
			},
			{
				name: "maturity",
				description: "adalah tanggal jatuh tempo sekuritas, yang dinyatakan sebagai angka tanggal seri"
			},
			{
				name: "pr",
				description: "adalah harga sekuritas per $100 nilai awal"
			},
			{
				name: "redemption",
				description: "adalah nilai tebusan sekuritas per $100 nilai awal"
			},
			{
				name: "basis",
				description: "adalah jenis basis penghitungan hari yang digunakan"
			}
		]
	},
	{
		name: "Z.TEST",
		description: "Menampilkan nilai-P satu-lemparan dari uji-z.",
		arguments: [
			{
				name: "array",
				description: "adalah array atau rentang data yang berlawanan untuk menguji X"
			},
			{
				name: "x",
				description: "adalah nilai untuk diuji"
			},
			{
				name: "sigma",
				description: "adalah populasi simpangan baku (yang dikenal). Jika dihilangkan, contoh simpangan baku digunakan"
			}
		]
	},
	{
		name: "ZTEST",
		description: "Menampilkan nilai-P satu-lemparan dari uji-z.",
		arguments: [
			{
				name: "array",
				description: "adalah barisan atau rentang data yang berlawanan untuk menguji X"
			},
			{
				name: "x",
				description: "adalah nilai untuk diuji"
			},
			{
				name: "sigma",
				description: "adalah populasi simpangan baku (yang dikenal). Jika dihilangkan, contoh simpangan baku digunakan"
			}
		]
	}
];