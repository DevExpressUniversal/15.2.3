ASPxClientSpreadsheet.Functions = [
	{
		name: "ACOS",
		description: "Zwraca arcus cosinus liczby w radianach w zakresie od 0 do Pi. Arcus cosinus jest kątem, którego cosinus daje liczbę.",
		arguments: [
			{
				name: "liczba",
				description: "- cosinus szukanego kąta; musi się zawierać w przedziale od -1 do 1"
			}
		]
	},
	{
		name: "ACOSH",
		description: "Zwraca arcus cosinus hiperboliczny liczby.",
		arguments: [
			{
				name: "liczba",
				description: "- dowolna liczba rzeczywista równa lub większa od 1"
			}
		]
	},
	{
		name: "ACOT",
		description: "Zwraca arcus cotangens liczby w radianach w zakresie od 0 do Pi.",
		arguments: [
			{
				name: "liczba",
				description: "- cotangens żądanego kąta."
			}
		]
	},
	{
		name: "ACOTH",
		description: "Zwraca arcus cotangens hiperboliczny liczby.",
		arguments: [
			{
				name: "liczba",
				description: "- cotangens hiperboliczny żądanego kąta."
			}
		]
	},
	{
		name: "ADR.POŚR",
		description: "Zwraca adres wskazany przez wartość tekstową.",
		arguments: [
			{
				name: "adres_tekst",
				description: "- adres komórki zawierającej adres w trybie adresowania A1 lub W1K1 lub nazwa zdefiniowana jako adres lub odwołanie do komórki jako ciągu tekstowego"
			},
			{
				name: "a1",
				description: "- wartość logiczna, która określa typ odwołania w argumencie tekst_odw: styl W1K1 = FAŁSZ; styl A1 = PRAWDA lub pominięta"
			}
		]
	},
	{
		name: "ADRES",
		description: "Tworzy tekst odwołania do komórki z podanego numeru wiersza i numeru komórki.",
		arguments: [
			{
				name: "nr_wiersza",
				description: "- numer wiersza używany w odwołaniu do komórki: Numer_wiersza = 1 dla wiersza 1"
			},
			{
				name: "nr_kolumny",
				description: "- numer kolumny używany w odwołaniu do komórki. Na przykład Numer_kolumny =4 dla kolumny D"
			},
			{
				name: "typ_adresu",
				description: "określa typ odwołania: bezwzględne = 1; bezwzględny wiersz/względna kolumna = 2; względny wiersz/bezwzględna kolumna = 3; względne = 4"
			},
			{
				name: "a1",
				description: "- wartość logiczna określająca styl odwołań: styl A1 = 1 lub PRAWDA; STYL W1K1 = 0 lub FAŁSZ"
			},
			{
				name: "tekst_arkusz",
				description: "- tekst określający nazwę arkusza używanego w odwołaniach zewnętrznych"
			}
		]
	},
	{
		name: "ARABSKIE",
		description: "Konwertuje liczbę rzymską na arabską.",
		arguments: [
			{
				name: "tekst",
				description: "- liczba rzymska, która ma zostać przekonwertowana."
			}
		]
	},
	{
		name: "ARG.LICZBY.ZESP",
		description: "Zwraca wartość argumentu liczby zespolonej, kąta wyrażonego w radianach.",
		arguments: [
			{
				name: "liczba_zesp",
				description: "– liczba zespolona poddana działaniu"
			}
		]
	},
	{
		name: "ARKUSZ",
		description: "Zwraca numer arkusza, którego dotyczy odwołanie.",
		arguments: [
			{
				name: "wartość",
				description: "- nazwa arkusza lub odwołanie, dla którego ma zostać określony numer arkusza. Jeśli pominięto, zwracany jest numer arkusza zawierającego funkcję."
			}
		]
	},
	{
		name: "ARKUSZE",
		description: "Zwraca liczbę arkuszy w odwołaniu.",
		arguments: [
			{
				name: "odwołanie",
				description: "- odwołanie zawierające arkusze, dla którego ma zostać określona ich liczba. Jeśli pominięto, zwracana jest liczba arkuszy skoroszytu zawierającego funkcję."
			}
		]
	},
	{
		name: "ASIN",
		description: "Zwraca arcus sinus liczby w radianach w zakresie od -Pi/2 do Pi/2.",
		arguments: [
			{
				name: "liczba",
				description: "- sinus szukanego kąta; musi zawierać się w przedziale od -1 do 1"
			}
		]
	},
	{
		name: "ASINH",
		description: "Zwraca arcus sinus hiperboliczny liczby.",
		arguments: [
			{
				name: "liczba",
				description: "- dowolna liczba rzeczywista równa lub większa od 1"
			}
		]
	},
	{
		name: "ATAN",
		description: "Zwraca arcus tangens liczby w radianach w zakresie od -Pi/2 do Pi/2.",
		arguments: [
			{
				name: "liczba",
				description: "- tangens żądanego kąta"
			}
		]
	},
	{
		name: "ATAN2",
		description: "Zwraca na podstawie współrzędnych x i y arcus tangens wyrażony w radianach w zakresie od -Pi do Pi z wyłączeniem -Pi.",
		arguments: [
			{
				name: "x_liczba",
				description: "- współrzędna x danego punktu"
			},
			{
				name: "y_liczba",
				description: "- współrzędna y danego punktu"
			}
		]
	},
	{
		name: "ATANH",
		description: "Zwraca arcus tangens hiperboliczny liczby.",
		arguments: [
			{
				name: "liczba",
				description: "- liczba rzeczywista z przedziału od -1 do 1, bez -1 i 1 "
			}
		]
	},
	{
		name: "BAT.TEKST",
		description: "Konwertuje liczbę na tekst (bat tajski).",
		arguments: [
			{
				name: "liczba",
				description: "- liczba, którą chcesz przekonwertować"
			}
		]
	},
	{
		name: "BD.ILE.REKORDÓW",
		description: "Zlicza komórki zawierające liczby we wskazanym polu (kolumnie) rekordów bazy danych, które spełniają podane warunki.",
		arguments: [
			{
				name: "baza",
				description: "- zakres komórek składających się na listę lub bazę danych. Baza danych jest listą powiązanych danych."
			},
			{
				name: "pole",
				description: "- jest albo etykietą kolumny umieszczoną w podwójnym cudzysłowie, albo numerem reprezentującym położenie kolumny na liście"
			},
			{
				name: "kryteria",
				description: "- zakres komórek zawierających podane warunki. Zakres zawiera etykietę kolumny i komórkę poniżej etykiety warunku"
			}
		]
	},
	{
		name: "BD.ILE.REKORDÓW.A",
		description: "Zwraca liczbę niepustych komórek w polu (kolumnie) rekordów bazy danych spełniających podane kryteria.",
		arguments: [
			{
				name: "baza",
				description: "- zakres komórek składających się na listę lub bazę danych. Baza danych jest listą powiązanych danych"
			},
			{
				name: "pole",
				description: "- jest albo etykietą kolumny umieszczoną w podwójnym cudzysłowie, albo numerem reprezentującym położenie kolumny na liście"
			},
			{
				name: "kryteria",
				description: "- zakres komórek zawierających podane warunki. Zakres zawiera etykietę kolumny i komórkę poniżej etykiety warunku"
			}
		]
	},
	{
		name: "BD.ILOCZYN",
		description: "Mnoży wartości umieszczone w danym polu (kolumnie) tych rekordów w bazie danych, które spełniają podane kryteria.",
		arguments: [
			{
				name: "baza",
				description: "- zakres komórek składających się na listę lub bazę danych. Baza danych jest listą powiązanych danych."
			},
			{
				name: "pole",
				description: "- jest albo etykietą kolumny umieszczoną w podwójnym cudzysłowie, albo numerem reprezentującym położenie kolumny na liście"
			},
			{
				name: "kryteria",
				description: "- zakres komórek zawierających podane warunki. Zakres zawiera etykietę kolumny i komórkę poniżej etykiety warunku"
			}
		]
	},
	{
		name: "BD.MAX",
		description: "Zwraca największą liczbę w polu (kolumnie) rekordów bazy danych, które spełniają określone warunki.",
		arguments: [
			{
				name: "baza_danych",
				description: "- zakres komórek składający się na listę lub bazę danych. Baza danych to lista powiązanych danych"
			},
			{
				name: "pole",
				description: "- albo etykieta kolumny w podwójnym cudzysłowie, albo liczba reprezentująca pozycję kolumny na liście"
			},
			{
				name: "kryteria",
				description: "- zakres komórek zawierający określone warunki. Zakres zawiera etykietę kolumny i jedną komórkę poniżej etykiety warunku"
			}
		]
	},
	{
		name: "BD.MIN",
		description: "Zwraca minimalną wartość z pola (kolumny) rekordów bazy danych, które spełniają podane kryteria.",
		arguments: [
			{
				name: "baza",
				description: "- zakres komórek składających się na listę lub bazę danych. Baza danych jest listą powiązanych danych."
			},
			{
				name: "pole",
				description: "- jest albo etykietą kolumny umieszczoną w podwójnym cudzysłowie, albo numerem reprezentującym położenie kolumny na liście"
			},
			{
				name: "kryteria",
				description: "- zakres komórek zawierających podane warunki. Zakres zawiera etykietę kolumny i komórkę poniżej etykiety warunku"
			}
		]
	},
	{
		name: "BD.ODCH.STANDARD",
		description: "Oblicza odchylenie standardowe próbki składającej się z zaznaczonych pozycji bazy danych.",
		arguments: [
			{
				name: "baza",
				description: "- zakres komórek składających się na listę lub bazę danych. Baza danych jest listą powiązanych danych."
			},
			{
				name: "pole",
				description: "- jest albo etykietą kolumny umieszczoną w podwójnym cudzysłowie, albo numerem reprezentującym położenie kolumny na liście"
			},
			{
				name: "kryteria",
				description: "- zakres komórek zawierających podane warunki. Zakres zawiera etykietę kolumny i komórkę poniżej etykiety warunku"
			}
		]
	},
	{
		name: "BD.ODCH.STANDARD.POPUL",
		description: "Oblicza odchylenie standardowe całej populacji składającej się z zaznaczonych pozycji bazy danych.",
		arguments: [
			{
				name: "baza",
				description: "- zakres komórek składających się na listę lub bazę danych. Baza danych jest listą powiązanych danych."
			},
			{
				name: "pole",
				description: "- jest albo etykietą kolumny umieszczoną w podwójnym cudzysłowie, albo numerem reprezentującym położenie kolumny na liście"
			},
			{
				name: "kryteria",
				description: "- zakres komórek zawierających podane warunki. Zakres zawiera etykietę kolumny i komórkę poniżej etykiety warunku"
			}
		]
	},
	{
		name: "BD.POLE",
		description: "Wydziela z bazy danych pojedynczy rekord, spełniający podane kryteria.",
		arguments: [
			{
				name: "baza",
				description: "- zakres komórek składających się na listę lub bazę danych. Baza danych jest listą powiązanych danych."
			},
			{
				name: "pole",
				description: "- jest albo etykietą kolumny umieszczoną w podwójnym cudzysłowie, albo numerem reprezentującym położenie kolumny na liście"
			},
			{
				name: "kryteria",
				description: "- zakres komórek zawierających podane warunki. Zakres zawiera etykietę kolumny i komórkę poniżej etykiety warunku"
			}
		]
	},
	{
		name: "BD.ŚREDNIA",
		description: "Oblicza wartość średnią w kolumnie listy lub bazy danych, która spełnia określone kryteria.",
		arguments: [
			{
				name: "baza_danych",
				description: "- zakres komórek składający się na listę lub bazę danych. Baza danych to lista powiązanych danych"
			},
			{
				name: "pole",
				description: "- albo etykieta kolumny w podwójnym cudzysłowie, albo liczba reprezentująca pozycję kolumny na liście"
			},
			{
				name: "kryteria",
				description: "- zakres komórek zawierający określone warunki. Zakres zawiera etykietę kolumny i jedną komórkę poniżej etykiety warunku"
			}
		]
	},
	{
		name: "BD.SUMA",
		description: "Dodaje liczby umieszczone w polach (kolumnie) tych rekordów bazy danych, które spełniają podane kryteria.",
		arguments: [
			{
				name: "baza",
				description: "- zakres komórek składających się na listę lub bazę danych. Baza danych jest listą powiązanych danych."
			},
			{
				name: "pole",
				description: "- jest albo etykietą kolumny umieszczoną w podwójnym cudzysłowie, albo numerem reprezentującym położenie kolumny na liście"
			},
			{
				name: "kryteria",
				description: "- zakres komórek zawierających podane warunki. Zakres zawiera etykietę kolumny i komórkę poniżej etykiety warunku"
			}
		]
	},
	{
		name: "BD.WARIANCJA",
		description: "Oblicza wariancję próbki składającej się z zaznaczonych pozycji bazy danych.",
		arguments: [
			{
				name: "baza",
				description: "- zakres komórek składających się na listę lub bazę danych. Baza danych jest listą powiązanych danych."
			},
			{
				name: "pole",
				description: "- jest albo etykietą kolumny umieszczoną w podwójnym cudzysłowie, albo numerem reprezentującym położenie kolumny na liście"
			},
			{
				name: "kryteria",
				description: "- zakres komórek zawierających podane warunki. Zakres zawiera etykietę kolumny i komórkę poniżej etykiety warunku"
			}
		]
	},
	{
		name: "BD.WARIANCJA.POPUL",
		description: "Oblicza wariancję całej populacji składającej się z zaznaczonych pozycji bazy danych.",
		arguments: [
			{
				name: "baza",
				description: "- zakres komórek składających się na listę lub bazę danych. Baza danych jest listą powiązanych danych."
			},
			{
				name: "pole",
				description: "- jest albo etykietą kolumny umieszczoną w podwójnym cudzysłowie, albo numerem reprezentującym położenie kolumny na liście"
			},
			{
				name: "kryteria",
				description: "- zakres komórek zawierających podane warunki. Zakres zawiera etykietę kolumny i komórkę poniżej etykiety warunku"
			}
		]
	},
	{
		name: "BESSEL.I",
		description: "Zwraca wartość zmodyfikowanej funkcji Bessela In(x).",
		arguments: [
			{
				name: "x",
				description: "– wartość, przy której należy wyliczyć funkcję"
			},
			{
				name: "n",
				description: "– rząd funkcji Bessela"
			}
		]
	},
	{
		name: "BESSEL.J",
		description: "Zwraca wartość funkcji Bessela Jn(x).",
		arguments: [
			{
				name: "x",
				description: "– wartość, przy której należy wyliczyć funkcję"
			},
			{
				name: "n",
				description: "– rząd funkcji Bessela"
			}
		]
	},
	{
		name: "BESSEL.K",
		description: "Zwraca wartość zmodyfikowanej funkcji Kn(x).",
		arguments: [
			{
				name: "x",
				description: "– wartość, przy której należy wyliczyć funkcję"
			},
			{
				name: "n",
				description: "– rząd funkcji Bessela"
			}
		]
	},
	{
		name: "BESSEL.Y",
		description: "Zwraca wartość funkcji Bessela Yn(x).",
		arguments: [
			{
				name: "x",
				description: "– wartość, przy której należy wyliczyć funkcję"
			},
			{
				name: "n",
				description: "– rząd funkcji Bessela"
			}
		]
	},
	{
		name: "BIT.PRZESUNIĘCIE.W.LEWO",
		description: "Zwraca liczbę przesuniętą w lewo o liczbę bitów liczba_przesunięć.",
		arguments: [
			{
				name: "liczba",
				description: "— reprezentacja dziesiętna liczby dwójkowej, która ma zostać wyznaczona."
			},
			{
				name: "liczba_przesunięć",
				description: "— liczba bitów, o jaką liczba ma zostać przesunięta w lewo."
			}
		]
	},
	{
		name: "BIT.PRZESUNIĘCIE.W.PRAWO",
		description: "Zwraca liczbę przesuniętą w prawo o liczbę bitów liczba_przesunięć.",
		arguments: [
			{
				name: "liczba",
				description: "— reprezentacja dziesiętna liczby dwójkowej, która ma zostać wyznaczona."
			},
			{
				name: "liczba_przesunięć",
				description: "— liczba bitów, o jaką liczba ma zostać przesunięta w prawo."
			}
		]
	},
	{
		name: "BITAND",
		description: "Zwraca wynik operacji bitowej AND (ORAZ) dwóch liczb.",
		arguments: [
			{
				name: "liczba1",
				description: "— reprezentacja dziesiętna liczby dwójkowej, która ma zostać wyznaczona."
			},
			{
				name: "liczba2",
				description: "— reprezentacja dziesiętna liczby dwójkowej, która ma zostać wyznaczona."
			}
		]
	},
	{
		name: "BITOR",
		description: "Zwraca wynik operacji bitowej OR (LUB) dwóch liczb.",
		arguments: [
			{
				name: "liczba1",
				description: "— reprezentacja dziesiętna liczby dwójkowej, która ma zostać wyznaczona."
			},
			{
				name: "liczba2",
				description: "— reprezentacja dziesiętna liczby dwójkowej, która ma zostać wyznaczona."
			}
		]
	},
	{
		name: "BITXOR",
		description: "Zwraca wynik operacji bitowej XOR (wyłączne LUB) dwóch liczb.",
		arguments: [
			{
				name: "liczba1",
				description: "— reprezentacja dziesiętna liczby dwójkowej, która ma zostać wyznaczona."
			},
			{
				name: "liczba2",
				description: "— reprezentacja dziesiętna liczby dwójkowej, która ma zostać wyznaczona."
			}
		]
	},
	{
		name: "BRAK",
		description: "Zwraca wartość błędu #N/D (wartość niedostępna).",
		arguments: [
		]
	},
	{
		name: "CENA.BS",
		description: "Zwraca cenę za 100 jednostek wartości nominalnej bonu skarbowego.",
		arguments: [
			{
				name: "rozliczenie",
				description: "– data rozliczenia papieru wartościowego, podana jako liczba seryjna"
			},
			{
				name: "data_spłaty",
				description: "– data spłaty papieru wartościowego, podana jako liczba seryjna"
			},
			{
				name: "dyskonto",
				description: "– stopa dyskontowa bonu skarbowego"
			}
		]
	},
	{
		name: "CENA.DYSK",
		description: "Zwraca cenę za 100 jednostek wartości nominalnej papieru wartościowego zdyskontowanego.",
		arguments: [
			{
				name: "rozliczenie",
				description: "– data rozliczenia papieru wartościowego, podana jako liczba seryjna"
			},
			{
				name: "data_spłaty",
				description: "– data spłaty papieru wartościowego, podana jako liczba seryjna"
			},
			{
				name: "dyskonto",
				description: "– stopa dyskontowa papieru wartościowego"
			},
			{
				name: "wykup",
				description: "– wartość wykupu papieru wartościowego przypadająca na 100 jednostek wartości nominalnej"
			},
			{
				name: "podstawa",
				description: "– rodzaj podstawy wyliczania dni"
			}
		]
	},
	{
		name: "CENA.DZIES",
		description: "Zamienia cenę w postaci ułamkowej na cenę w postaci dziesiętnej.",
		arguments: [
			{
				name: "wartość_ułamkowa",
				description: "– liczba wyrażona jako ułamek"
			},
			{
				name: "ułamek",
				description: "– liczba całkowita używana jako mianownik ułamka"
			}
		]
	},
	{
		name: "CENA.UŁAM",
		description: "Zamienia cenę w postaci dziesiętnej na cenę w postaci ułamkowej.",
		arguments: [
			{
				name: "wartość_dziesiętna",
				description: "– liczba dziesiętna"
			},
			{
				name: "ułamek",
				description: "– liczba całkowita używana jako mianownik ułamka"
			}
		]
	},
	{
		name: "CHI.TEST",
		description: "Zwraca test na niezależność: wartość z rozkładu chi-kwadrat dla statystyki i odpowiednich stopni swobody.",
		arguments: [
			{
				name: "zakres_bieżący",
				description: "- zakres danych zawierający wartości zaobserwowane, które mają zostać porównane z wartościami oczekiwanymi"
			},
			{
				name: "zakres_przewidywany",
				description: "- zakres danych zawierający stosunek iloczynu sum wierszy i sum kolumn do sumy końcowej"
			}
		]
	},
	{
		name: "COS",
		description: "Zwraca cosinus kąta.",
		arguments: [
			{
				name: "liczba",
				description: "- kąt podany w radianach, dla którego zostanie wyznaczony cosinus"
			}
		]
	},
	{
		name: "COS.LICZBY.ZESP",
		description: "Zwraca wartość cosinusa liczby zespolonej.",
		arguments: [
			{
				name: "liczba_zesp",
				description: "– liczba zespolona poddana działaniu"
			}
		]
	},
	{
		name: "COSH",
		description: "Zwraca cosinus hiperboliczny liczby.",
		arguments: [
			{
				name: "liczba",
				description: "- dowolna liczba rzeczywista"
			}
		]
	},
	{
		name: "COSH.LICZBY.ZESP",
		description: "Zwraca cosinus hiperboliczny liczby zespolonej.",
		arguments: [
			{
				name: "liczba_zespolona",
				description: "– liczba zespolona, dla której ma zostać obliczony cosinus hiperboliczny."
			}
		]
	},
	{
		name: "COT",
		description: "Zwraca cotangens kąta.",
		arguments: [
			{
				name: "liczba",
				description: "- kąt podany w radianach, dla którego ma zostać obliczony cotangens."
			}
		]
	},
	{
		name: "COT.LICZBY.ZESP",
		description: "Zwraca cotangens liczby zespolonej.",
		arguments: [
			{
				name: "liczba_zespolona",
				description: "- liczba zespolona, dla której ma zostać obliczony cotangens."
			}
		]
	},
	{
		name: "COTH",
		description: "Zwraca cotangens hiperboliczny liczby.",
		arguments: [
			{
				name: "liczba",
				description: "- kąt podany w radianach, dla którego ma zostać obliczony cotangens hiperboliczny."
			}
		]
	},
	{
		name: "CSC",
		description: "Zwraca cosecans kąta.",
		arguments: [
			{
				name: "liczba",
				description: "- kąt podany w radianach, dla którego ma zostać obliczony cosecans."
			}
		]
	},
	{
		name: "CSC.LICZBY.ZESP",
		description: "Zwraca cosecans liczby zespolonej.",
		arguments: [
			{
				name: "liczba_zespolona",
				description: "- liczba zespolona, dla której ma zostać obliczony cosecans."
			}
		]
	},
	{
		name: "CSCH",
		description: "Zwraca cosecans hiperboliczny kąta.",
		arguments: [
			{
				name: "liczba",
				description: "- kąt podany w radianach, dla którego ma zostać obliczony cosecans hiperboliczny."
			}
		]
	},
	{
		name: "CSCH.LICZBY.ZESP",
		description: "Zwraca cosecans hiperboliczny liczby zespolonej.",
		arguments: [
			{
				name: "liczba_zespolona",
				description: "- liczba zespolona, dla której ma zostać obliczony cosecans hiperboliczny."
			}
		]
	},
	{
		name: "CZ.CAŁK.DZIELENIA",
		description: "Zwraca część całkowitą z dzielenia.",
		arguments: [
			{
				name: "dzielna",
				description: "– wartość dzielnej"
			},
			{
				name: "dzielnik",
				description: "– wartość dzielnika"
			}
		]
	},
	{
		name: "CZ.RZECZ.LICZBY.ZESP",
		description: "Zwraca część rzeczywistą liczby zespolonej.",
		arguments: [
			{
				name: "liczba_zesp",
				description: "– liczba zespolona poddana działaniu"
			}
		]
	},
	{
		name: "CZ.UROJ.LICZBY.ZESP",
		description: "Zwraca część urojoną liczby zespolonej.",
		arguments: [
			{
				name: "liczba_zesp",
				description: "– liczba zespolona poddana działaniu"
			}
		]
	},
	{
		name: "CZAS",
		description: "Konwertuje godziny, minuty i sekundy dane jako liczby na liczby kolejne programu Spreadsheet, sformatowane za pomocą formatu czasu.",
		arguments: [
			{
				name: "godzina",
				description: "- liczba od 0 do 23 reprezentująca godzinę"
			},
			{
				name: "minuta",
				description: "- liczba od 0 do 59 reprezentująca minutę"
			},
			{
				name: "sekunda",
				description: "- liczba od 0 do 59 reprezentująca sekundę"
			}
		]
	},
	{
		name: "CZAS.WARTOŚĆ",
		description: "Konwertuje czas w formacie tekstowym na kolejną liczbę czasu programu Spreadsheet: liczbę od 0 (00:00:00) do 0,999988426 (23:59:59). Liczbę należy formatować za pomocą formatu czasu po wprowadzeniu formuły.",
		arguments: [
			{
				name: "godzina_tekst",
				description: "- ciąg tekstowy podający czas w dowolnym formacie czasu programu Spreadsheet (informacje o dacie w ciągu są ignorowane)"
			}
		]
	},
	{
		name: "CZĘŚĆ.ROKU",
		description: "Podaje, jaką część roku stanowi pełna liczba dni pomiędzy datą początkową i końcową.",
		arguments: [
			{
				name: "data_pocz",
				description: "– liczba seryjna reprezentująca datę początkową"
			},
			{
				name: "data_końc",
				description: "– liczba seryjna reprezentująca datę końcową"
			},
			{
				name: "podstawa",
				description: "– typ przyjętego systemu liczenia dni"
			}
		]
	},
	{
		name: "CZĘSTOŚĆ",
		description: "Oblicza rozkład częstości występowania wartości w zakresie wartości i zwraca w postaci pionowej tablicy liczby, które mają o jeden element więcej niż tablica_bin.",
		arguments: [
			{
				name: "tablica_dane",
				description: "- tablica lub odwołanie do zbioru wartości, dla których będą obliczane częstości (komórki puste i tekst są ignorowane)"
			},
			{
				name: "tablica_przedziały",
				description: "- tablica lub odwołanie do zbioru przedziałów, w których mają być grupowane wartości ze zbioru tablica_dane"
			}
		]
	},
	{
		name: "CZY.ADR",
		description: "Sprawdza, czy wartość jest odwołaniem i zwraca wartość PRAWDA albo FAŁSZ.",
		arguments: [
			{
				name: "wartość",
				description: "- wartość, która ma być testowana. Wartość może się odwoływać do komórki, formuły lub nazwy, która odwołuje się do komórki, formuły lub wartości"
			}
		]
	},
	{
		name: "CZY.BŁ",
		description: "Sprawdza, czy wartość jest błędem (#ARG!, #ADR!, #ARG!0, #LICZBA!, #NAZWA? lub #ZERO!) z wyjątkiem wartości błędu #N/D i zwraca wartość PRAWDA albo FAŁSZ.",
		arguments: [
			{
				name: "wartość",
				description: "- wartość, która ma być testowana. Wartość może odwoływać się do komórki, formuły lub nazwy, która odwołuje się do komórki, formuły lub wartości"
			}
		]
	},
	{
		name: "CZY.BŁĄD",
		description: "Sprawdza, czy wartość jest błędem (#N/D, #ARG!, #ADR!, #DZIEL/0!, #LICZBA!, #NAZWA? lub #ZERO!) i zwraca wartość PRAWDA albo FAŁSZ.",
		arguments: [
			{
				name: "wartość",
				description: "- wartość, którą chcesz testować. Wartość może się odwoływać do komórki, formuły lub nazwy, która odwołuje się do komórki, formuły lub wartości"
			}
		]
	},
	{
		name: "CZY.BRAK",
		description: "Sprawdza, czy wartość to #N/D i zwraca wartość PRAWDA albo FAŁSZ.",
		arguments: [
			{
				name: "wartość",
				description: "- wartość, którą chcesz testować. Wartość może odwoływać się do komórki, formuły lub nazwy, która odwołuje się do komórki, formuły lub wartości"
			}
		]
	},
	{
		name: "CZY.FORMUŁA",
		description: "Sprawdza, czy odwołanie jest odwołaniem do komórki zawierającej formułę, i zwraca wartość PRAWDA albo FAŁSZ.",
		arguments: [
			{
				name: "odwołanie",
				description: "- odwołanie do komórki, która ma być testowana. Odwołanie może być odwołaniem do komórki, formułą lub nazwą, która odwołuje się do komórki."
			}
		]
	},
	{
		name: "CZY.LICZBA",
		description: "Sprawdza, czy wartość to liczba i zwraca wartość PRAWDA albo FAŁSZ.",
		arguments: [
			{
				name: "wartość",
				description: "- wartość, która ma być testowana. Wartość może odwoływać się do komórki, formuły lub nazwy, która odwołuje się do komórki, formuły lub wartości"
			}
		]
	},
	{
		name: "CZY.LOGICZNA",
		description: "Sprawdza, czy wartość jest wartością logiczną (PRAWDA albo FAŁSZ) i zwraca wartość PRAWDA albo FAŁSZ.",
		arguments: [
			{
				name: "wartość",
				description: "- wartość, która ma być testowana. Wartość może się odwoływać do komórki, formuły lub nazwy, która odwołuje się do komórki, formuły lub wartości"
			}
		]
	},
	{
		name: "CZY.NIE.TEKST",
		description: "Sprawdza, czy wartość nie jest tekstem (puste komórki nie są tekstem) i zwraca wartość PRAWDA albo FAŁSZ.",
		arguments: [
			{
				name: "wartość",
				description: "- wartość, która ma być testowana: komórka, formuła, lub nazwa odwołująca się do komórki, formuły lub wartości"
			}
		]
	},
	{
		name: "CZY.NIEPARZYSTE",
		description: "Zwraca wartość PRAWDA, jeśli liczba jest nieparzysta.",
		arguments: [
			{
				name: "liczba",
				description: "– testowana wartość"
			}
		]
	},
	{
		name: "CZY.PARZYSTE",
		description: "Zwraca wartość PRAWDA, jeśli liczba jest parzysta.",
		arguments: [
			{
				name: "liczba",
				description: "– testowana wartość"
			}
		]
	},
	{
		name: "CZY.PUSTA",
		description: "Sprawdza, czy odwołanie następuje do pustej komórki i zwraca wartość PRAWDA albo FAŁSZ.",
		arguments: [
			{
				name: "wartość",
				description: "- komórka lub nazwa odwołująca się do komórki, którą chcesz testować"
			}
		]
	},
	{
		name: "CZY.RÓWNE",
		description: "Sprawdza, czy dwie liczby są równe.",
		arguments: [
			{
				name: "liczba1",
				description: "– pierwsza liczba"
			},
			{
				name: "liczba2",
				description: "– druga liczba"
			}
		]
	},
	{
		name: "CZY.TEKST",
		description: "Sprawdza, czy wartość to tekst i zwraca wartość PRAWDA albo FAŁSZ.",
		arguments: [
			{
				name: "wartość",
				description: "- wartość, która ma być testowana. Wartość może odwoływać się do komórki, formuły lub nazwy, która odwołuje się do komórki, formuły lub wartości"
			}
		]
	},
	{
		name: "DANE.CZASU.RZECZ",
		description: "Pobiera dane czasu rzeczywistego z programu obsługującego automatyzację COM.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "progID",
				description: "- nazwa identyfikatora ProgID zarejestrowanego dodatku automatyzacji COM. Nazwa musi być umieszczona w cudzysłowie"
			},
			{
				name: "serwer",
				description: "- nazwa serwera, na którym powinien być uruchomiony dodatek. Nazwa musi być umieszczona w cudzysłowie. Jeżeli dodatek jest uruchamiany lokalnie, użyj pustego ciągu"
			},
			{
				name: "temat1",
				description: "- od 1 do 38 parametrów określających dane"
			},
			{
				name: "temat2",
				description: "- od 1 do 38 parametrów określających dane"
			}
		]
	},
	{
		name: "DATA",
		description: "Zwraca liczbę reprezentującą datę w kodzie data-godzina programu Spreadsheet.",
		arguments: [
			{
				name: "rok",
				description: "- liczba między 1900 a 9999 w programie Spreadsheet dla systemu Windows lub 1904 a 9999 w programie Spreadsheet dla komputerów Macintosh"
			},
			{
				name: "miesiąc",
				description: "- liczba od 1 do 12 reprezentująca miesiąc roku"
			},
			{
				name: "dzień",
				description: "- liczba od 1 do 31 reprezentująca dzień miesiąca"
			}
		]
	},
	{
		name: "DATA.RÓŻNICA",
		description: "",
		arguments: [
		]
	},
	{
		name: "DATA.WARTOŚĆ",
		description: "Konwertuje datę w postaci tekstu na liczbę reprezentującą datę w kodzie data-godzina programu Spreadsheet.",
		arguments: [
			{
				name: "data_tekst",
				description: "- tekst, który reprezentuje datę w formacie programu Spreadsheet z przedziału od 1.1.1900 (Windows) lub 1.1.1904 (Macintosh) do 31.12.9999"
			}
		]
	},
	{
		name: "DB",
		description: "Zwraca amortyzację środka trwałego za podany okres metodą równomiernie malejącego salda.",
		arguments: [
			{
				name: "koszt",
				description: "- wartość początkowa (koszt) środka trwałego"
			},
			{
				name: "odzysk",
				description: "- wartość środka trwałego po całkowitym czasie amortyzacji"
			},
			{
				name: "czas_życia",
				description: "- liczba okresów stanowiących całkowity czas amortyzacji środka trwałego (nazywana również czasem życia zasobu)"
			},
			{
				name: "okres",
				description: "- okres, dla którego obliczana jest amortyzacja. Okres musi być wyrażony w tych samych jednostkach co czas_życia"
			},
			{
				name: "miesiąc",
				description: "- liczba miesięcy w pierwszym roku. Przy pominięciu przyjmuje się wartość 12."
			}
		]
	},
	{
		name: "DDB",
		description: "Zwraca amortyzację środka trwałego za podany okres obliczoną metodą podwójnego spadku lub inną metodą określoną przez użytkownika.",
		arguments: [
			{
				name: "koszt",
				description: "- wartość początkowa (koszt) środka trwałego"
			},
			{
				name: "odzysk",
				description: "- wartość środka trwałego po całkowitym czasie amortyzacji"
			},
			{
				name: "czas_życia",
				description: "- liczba okresów stanowiących całkowity czas amortyzacji środka trwałego (nazywana również czasem życia środka trwałego)"
			},
			{
				name: "okres",
				description: "- okres, dla którego obliczana jest amortyzacja. Okres musi być wyrażony w tych samych jednostkach co czas_życia"
			},
			{
				name: "współczynnik",
				description: "- wartość sterująca szybkością, z jaką ma maleć saldo. Przy pominięciu przyjmuje się wartość 2 (metoda podwójnego spadku)"
			}
		]
	},
	{
		name: "DŁ",
		description: "Zwraca liczbę znaków w ciągu znaków.",
		arguments: [
			{
				name: "tekst",
				description: "- tekst, którego długość ma zostać obliczona. Spacje liczą się za znaki"
			}
		]
	},
	{
		name: "DNI",
		description: "Zwraca liczbę dni zawartych między dwiema datami.",
		arguments: [
			{
				name: "data_końcowa",
				description: "— data_początkowa i data_końcowa to dwie daty, dla których ma zostać określona liczba dni zawartych między nimi."
			},
			{
				name: "data_początkowa",
				description: "— data_początkowa i data_końcowa to dwie daty, dla których ma zostać określona liczba dni zawartych między nimi."
			}
		]
	},
	{
		name: "DNI.360",
		description: "Oblicza liczbę dni zawartych między dwiema datami przyjmując rok liczący 360 dni (dwanaście 30-dniowych miesięcy).",
		arguments: [
			{
				name: "data_początkowa",
				description: "- data_początkowa i data_końcowa są dwiema datami, między którymi wyznaczana jest liczba dni"
			},
			{
				name: "data_końcowa",
				description: "- data_początkowa i data_końcowa są dwiema datami, między którymi wyznaczana jest liczba dni"
			},
			{
				name: "metoda",
				description: "- określa metodę przeliczenia. Jeśli nie podano wartości albo jest nią FAŁSZ, użyta zostanie metoda USA (NASD); jeśli wartością jest PRAWDA, użyta zostanie metoda europejska."
			}
		]
	},
	{
		name: "DNI.ROBOCZE",
		description: "Zwraca liczbę dni roboczych pomiędzy dwiema datami.",
		arguments: [
			{
				name: "data_pocz",
				description: "– liczba seryjna reprezentująca datę początkową"
			},
			{
				name: "data_końc",
				description: "– liczba seryjna reprezentująca datę końcową"
			},
			{
				name: "święta",
				description: "– opcjonalna tablica jednej lub więcej liczb seryjnych daty do wyłączenia z kalendarza dni roboczych, jak np. święta państwowe lub kościelne"
			}
		]
	},
	{
		name: "DNI.ROBOCZE.NIESTAND",
		description: "Zwraca liczbę dni roboczych między dwiema datami z niestandardowymi parametrami dotyczącymi weekendów.",
		arguments: [
			{
				name: "data_pocz",
				description: "- liczba seryjna reprezentująca datę początkową"
			},
			{
				name: "data_końc",
				description: "- liczba seryjna reprezentująca datę końcową"
			},
			{
				name: "weekend",
				description: "- liczba lub ciąg określający, kiedy występują weekendy"
			},
			{
				name: "święta",
				description: "- opcjonalny zestaw zawierający jedną lub więcej liczb seryjnych dat do wykluczenia z kalendarza dni roboczych, na przykład święta państwowe lub święta o zmiennej dacie występowania"
			}
		]
	},
	{
		name: "DWÓJK.NA.DZIES",
		description: "Przekształca liczbę dwójkową na dziesiętną.",
		arguments: [
			{
				name: "liczba",
				description: "– liczba dwójkowa poddana przekształceniu"
			}
		]
	},
	{
		name: "DWÓJK.NA.ÓSM",
		description: "Zamienia liczbę dwójkową na liczbę w kodzie ósemkowym.",
		arguments: [
			{
				name: "liczba",
				description: "– liczba dwójkowa poddana przekształceniu"
			},
			{
				name: "miejsca",
				description: "– liczba znaków dla wartości wynikowej"
			}
		]
	},
	{
		name: "DWÓJK.NA.SZESN",
		description: "Zamienia liczbę dwójkową na liczbę w kodzie szesnastkowym.",
		arguments: [
			{
				name: "liczba",
				description: "– liczba dwójkowa poddana przekształceniu"
			},
			{
				name: "miejsca",
				description: "– liczba znaków dla wartości wynikowej"
			}
		]
	},
	{
		name: "DZIEŃ",
		description: "Zwraca dzień miesiąca, liczbę od 1 do 31.",
		arguments: [
			{
				name: "kolejna_liczba",
				description: "- liczba w kodzie data-godzina używanym w programie Spreadsheet"
			}
		]
	},
	{
		name: "DZIEŃ.ROBOCZY",
		description: "Zwraca wartość liczby seryjnej daty przed lub po podanej liczbie dni roboczych.",
		arguments: [
			{
				name: "data_pocz",
				description: "– liczba seryjna reprezentująca datę początkową"
			},
			{
				name: "dni",
				description: "– liczba dni roboczych (nie koniec tyg. i nie święta) przed lub po dacie początkowej"
			},
			{
				name: "święta",
				description: "– opcjonalna tablica jednej lub więcej liczb seryjnych daty do wyłączenia z kalendarza dni roboczych, jak np. święta państwowe lub kościelne"
			}
		]
	},
	{
		name: "DZIEŃ.ROBOCZY.NIESTAND",
		description: "Zwraca liczbę seryjną daty przypadającej przed lub po określonej liczbie dni roboczych z niestandardowymi parametrami weekendów.",
		arguments: [
			{
				name: "data_pocz",
				description: "- liczba seryjna reprezentująca datę początkową"
			},
			{
				name: "dni",
				description: "- liczba dni roboczych (nieprzypadających w weekendy i święta) przed datą data_pocz lub po tej dacie"
			},
			{
				name: "weekend",
				description: "- liczba lub ciąg określający, kiedy występują weekendy"
			},
			{
				name: "święta",
				description: "- opcjonalna tablica zawierająca jedną lub więcej liczb seryjnych dat do wykluczenia z kalendarza dni roboczych, na przykład święta państwowe lub święta o zmiennej dacie występowania"
			}
		]
	},
	{
		name: "DZIEŃ.TYG",
		description: "Zwraca liczbę od 1 do 7, określającą numer dnia tygodnia na podstawie daty.",
		arguments: [
			{
				name: "liczba_kolejna",
				description: "- liczba reprezentująca datę"
			},
			{
				name: "zwracany_typ",
				description: "- liczba: dla numeracji od niedzieli=1 do soboty=7 użyj 1; od poniedziałku=1 do niedzieli=7 użyj 2; od poniedziałku=0 do niedzieli=6 użyj 3"
			}
		]
	},
	{
		name: "DZIES.NA.DWÓJK",
		description: "Zamienia liczbę dziesiętną na liczbę w kodzie dwójkowym.",
		arguments: [
			{
				name: "liczba",
				description: "– liczba dziesiętna poddana przekształceniu"
			},
			{
				name: "miejsca",
				description: "– liczba znaków dla wartości wynikowej"
			}
		]
	},
	{
		name: "DZIES.NA.ÓSM",
		description: "Zamienia liczbę dziesiętną na liczbę w kodzie ósemkowym.",
		arguments: [
			{
				name: "liczba",
				description: "– liczba dziesiętna poddana przekształceniu"
			},
			{
				name: "miejsca",
				description: "– liczba znaków dla wartości wynikowej"
			}
		]
	},
	{
		name: "DZIES.NA.SZESN",
		description: "Zamienia liczbę dziesiętną na liczbę w kodzie szesnastkowym.",
		arguments: [
			{
				name: "liczba",
				description: "– liczba dziesiętna poddana przekształceniu"
			},
			{
				name: "miejsca",
				description: "– liczba znaków dla wartości wynikowej"
			}
		]
	},
	{
		name: "DZIESIĘTNA",
		description: "Konwertuje reprezentację tekstową liczby o podanej podstawie na liczbę dziesiętną.",
		arguments: [
			{
				name: "liczba",
				description: "— liczba, która ma zostać przekonwertowana."
			},
			{
				name: "podstawa",
				description: "— podstawa konwertowanej liczby."
			}
		]
	},
	{
		name: "DZIŚ",
		description: "Zwraca datę bieżącą sformatowaną jako datę.",
		arguments: [
		]
	},
	{
		name: "EFEKTYWNA",
		description: "Zwraca wartość efektywnej rocznej stopy oprocentowania.",
		arguments: [
			{
				name: "stopa_nominalna",
				description: "– nominalna stopa procentowa"
			},
			{
				name: "okresy",
				description: "– liczba okresów składanych w roku"
			}
		]
	},
	{
		name: "ENCODEURL",
		description: "Zwraca ciąg zakodowany w postaci adresu URL.",
		arguments: [
			{
				name: "tekst",
				description: "- ciąg, który ma zostać zakodowany w postaci adresu URL"
			}
		]
	},
	{
		name: "EXP",
		description: "Oblicza wartość liczby e podniesionej do potęgi określonej przez podaną liczbę.",
		arguments: [
			{
				name: "liczba",
				description: "- jest wykładnikiem, do którego podnoszona jest liczba e. Stała e jest równa 2,71828182845904 i jest podstawą logarytmów naturalnych"
			}
		]
	},
	{
		name: "EXP.LICZBY.ZESP",
		description: "Zwraca wartość wykładniczą liczby zespolonej.",
		arguments: [
			{
				name: "liczba_zesp",
				description: "– liczba zespolona poddana działaniu"
			}
		]
	},
	{
		name: "F.TEST",
		description: "Zwraca wynik testu F, dwustronnego prawdopodobieństwa, że wariancje w Tablicy1 i Tablicy2 nie są istotnie różne.",
		arguments: [
			{
				name: "tablica1",
				description: "- pierwsza tablica lub zakres danych, może zawierać liczby lub nazwy, tablice albo odwołania zawierające liczby (puste są ignorowane)"
			},
			{
				name: "tablica2",
				description: "- druga tablica lub zakres danych, może zawierać liczby lub nazwy, tablice albo odwołania zawierające liczby (puste są ignorowane)"
			}
		]
	},
	{
		name: "FAŁSZ",
		description: "Zwraca wartość logiczną FAŁSZ.",
		arguments: [
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
		name: "FORMUŁA.TEKST",
		description: "Zwraca formułę jako ciąg.",
		arguments: [
			{
				name: "odwołanie",
				description: "- odwołanie do formuły."
			}
		]
	},
	{
		name: "FRAGMENT.TEKSTU",
		description: "Zwraca znaki ze środka ciągu tekstowego przy danej pozycji początkowej i długości.",
		arguments: [
			{
				name: "tekst",
				description: "- ciąg znaków, z którego mają być wyodrębnione znaki"
			},
			{
				name: "liczba_początkowa",
				description: "- pozycja pierwszego znaku, który ma być wyodrębniony. Pierwszy znak w tekście to 1"
			},
			{
				name: "liczba_znaków",
				description: "- określa, ile znaków ma zostać zwróconych z tekstu"
			}
		]
	},
	{
		name: "FUNKCJA.BŁ",
		description: "Zwraca funkcję błędu.",
		arguments: [
			{
				name: "dolna_granica",
				description: "– dolne ograniczenie przy wyznaczaniu ERF"
			},
			{
				name: "górna_granica",
				description: "– górne ograniczenie przy wyznaczaniu ERF"
			}
		]
	},
	{
		name: "FUNKCJA.BŁ.DOKŁ",
		description: "Zwraca funkcję błędu.",
		arguments: [
			{
				name: "X",
				description: "- dolne ograniczenie przy wyznaczaniu wartości FUNKCJA.BŁ.DOKŁ"
			}
		]
	},
	{
		name: "FV",
		description: "Zwraca przyszłą wartość inwestycji na podstawie okresowych, stałych płatności i stałej stopy procentowej.",
		arguments: [
			{
				name: "stopa",
				description: "- stopa oprocentowania dla okresu. Na przykład użyj stopy 6%/4 dla płatności kwartalnych w przypadku stopy 6% w stosunku rocznym"
			},
			{
				name: "liczba_rat",
				description: "- liczba wszystkich okresów płatności w czasie lokaty"
			},
			{
				name: "rata",
				description: "- płatność okresowa, niezmienna przez cały czas trwania lokaty"
			},
			{
				name: "wa",
				description: "- wartość bieżąca lokaty, teraźniejsza łączna wartość serii przyszłych płatności. Jeśli pominięta, wa = 0"
			},
			{
				name: "typ",
				description: "- wartość reprezentująca czas dokonywania płatności: płatność na początek okresu = 1; płatność na koniec okresu = 0 albo pominięta"
			}
		]
	},
	{
		name: "GAMMA",
		description: "Zwraca wartość funkcji Gamma.",
		arguments: [
			{
				name: "x",
				description: "- wartość, dla której ma zostać obliczona funkcja Gamma."
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
		name: "GODZINA",
		description: "Zwraca godzinę jako liczbę od 0 (0:00) do 23 (23:00).",
		arguments: [
			{
				name: "kolejna_liczba",
				description: "- liczba w kodzie data-godzina używanym w programie Spreadsheet albo tekst w formacie godziny, taki jak 16:48:00 albo 4:48:00 PM"
			}
		]
	},
	{
		name: "HIPERŁĄCZE",
		description: "Tworzy skrót lub skok, który otwiera dokument przechowywany na dysku twardym, serwerze sieciowym lub w Internecie.",
		arguments: [
			{
				name: "łącze_lokalizacja",
				description: "- tekst określający ścieżkę i nazwę pliku dokumentu, który ma być otwarty, nazwę dysku twardego, adres UNC, lub ścieżkę URL"
			},
			{
				name: "przyjazna_nazwa",
				description: "- jest tekstem lub liczbą wyświetlaną w komórce. Jeżeli zostanie pominięte, to w komórce będzie wyświetlany tekst Łącze_lokalizacja"
			}
		]
	},
	{
		name: "ILE.LICZB",
		description: "Oblicza, ile komórek w zakresie zawiera liczby.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "wartość1",
				description: "- od 1 do 255 argumentów, które mogą zawierać lub odwoływać się do różnych typów danych, przy czym zliczane będą tylko liczby"
			},
			{
				name: "wartość2",
				description: "- od 1 do 255 argumentów, które mogą zawierać lub odwoływać się do różnych typów danych, przy czym zliczane będą tylko liczby"
			}
		]
	},
	{
		name: "ILE.NIEPUSTYCH",
		description: "Oblicza, ile niepustych komórek w zakresie .",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "wartość1",
				description: "- od 1 do 255 argumentów reprezentujących zliczane wartości. Wartości mogą być dowolnego typu"
			},
			{
				name: "wartość2",
				description: "- od 1 do 255 argumentów reprezentujących zliczane wartości. Wartości mogą być dowolnego typu"
			}
		]
	},
	{
		name: "ILE.WIERSZY",
		description: "Zwraca liczbę wierszy odpowiadających podanemu odwołaniu lub tablicy.",
		arguments: [
			{
				name: "tablica",
				description: "- tablica, formuła zwracająca tablicę lub odwołanie do zakresu komórek, dla których zostanie wyznaczona liczba wierszy"
			}
		]
	},
	{
		name: "ILOCZYN",
		description: "Mnoży wszystkie liczby dane jako argumenty.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "liczba1",
				description: "- od 1 do 255 liczb, wartości logicznych lub tekstowej reprezentacji liczb, które mają być mnożone"
			},
			{
				name: "liczba2",
				description: "- od 1 do 255 liczb, wartości logicznych lub tekstowej reprezentacji liczb, które mają być mnożone"
			}
		]
	},
	{
		name: "ILOCZYN.LICZB.ZESP",
		description: "Zwraca iloczyn od 1 do 255 liczb zespolonych.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "liczba_zespolona1",
				description: "liczba_zespolona1, liczba_zespolona2,... jest sekwencją od 1 do 255 liczb zespolonych, dla których należy obliczyć iloczyn."
			},
			{
				name: "liczba_zespolona2",
				description: "liczba_zespolona1, liczba_zespolona2,... jest sekwencją od 1 do 255 liczb zespolonych, dla których należy obliczyć iloczyn."
			}
		]
	},
	{
		name: "ILORAZ.LICZB.ZESP",
		description: "Zwraca iloraz dwóch liczb zespolonych.",
		arguments: [
			{
				name: "liczba_zesp1",
				description: "– liczba zespolona stanowiąca dzielną"
			},
			{
				name: "liczba_zesp2",
				description: "– liczba zespolona stanowiąca dzielnik"
			}
		]
	},
	{
		name: "INDEKS",
		description: "Zwraca wartość lub odwołanie do komórki na przecięciu określonego wiersza i kolumny w danym zakresie.",
		arguments: [
			{
				name: "tablica",
				description: "- zakres komórek lub stała tablicowa."
			},
			{
				name: "nr_wiersza",
				description: "- zaznacza wiersz w tablicy lub odwołaniu, z którego ma zostać zwrócona wartość. W przypadku pominięcia wymagany jest argument nr_kolumny"
			},
			{
				name: "nr_kolumny",
				description: "- zaznacza kolumnę w tablicy lub odwołaniu, z której ma zostać zwrócona wartość. W przypadku pominięcia wymagany jest argument nr_wiersza"
			}
		]
	},
	{
		name: "INFO",
		description: "Zwraca informacje na temat środowiska, w którym działa program.",
		arguments: [
			{
				name: "typ_tekst",
				description: "- tekst określający, jakiego rodzaju informacje zostaną zwrócone."
			}
		]
	},
	{
		name: "IPMT",
		description: "Zwraca wartość płatności odsetek od lokaty w danym okresie przy założeniu okresowych, stałych płatności i stałej stopy procentowej.",
		arguments: [
			{
				name: "stopa",
				description: "- stopa oprocentowania dla okresu. Na przykład użyj stopy 6%/4 dla kwartalnych płatności w przypadku stopy 6% w stosunku rocznym"
			},
			{
				name: "okres",
				description: "- okres, dla którego mają być naliczone odsetki; musi zawierać się między 1 a liczba_rat"
			},
			{
				name: "liczba_rat",
				description: "- liczba wszystkich okresów płatności w całym okresie lokaty"
			},
			{
				name: "wa",
				description: "- wartość bieżąca lokaty, teraźniejsza łączna wartość serii przyszłych rat"
			},
			{
				name: "wp",
				description: "- wartość przyszła lub saldo gotówkowe do uzyskania po dokonaniu ostatniej płatności. Jeśli pominięta, pw = 0"
			},
			{
				name: "typ",
				description: "- wartość logiczna reprezentująca sposób dokonywania płatności: na koniec okresu = 0 lub pominięta; na początku okresu = 1"
			}
		]
	},
	{
		name: "IRR",
		description: "Oblicza wewnętrzną stopę zwrotu dla przepływów gotówkowych.",
		arguments: [
			{
				name: "wartości",
				description: "- tablica lub odwołanie do komórek zawierających wartości do obliczenia wewnętrznej stopy zwrotu"
			},
			{
				name: "wynik",
				description: "- jest liczbą, która jak podejrzewasz, jest bliska wynikowi funkcji IRR; 0,1 (10 procent, jeśli pominięto)"
			}
		]
	},
	{
		name: "ISO.NUM.TYG",
		description: "Zwraca dla danej daty numer tygodnia roku w formacie ISO.",
		arguments: [
			{
				name: "data",
				description: "- kod data-godzina używany przez program Spreadsheet do obliczania daty i godziny."
			}
		]
	},
	{
		name: "ISO.ZAOKR.W.GÓRĘ",
		description: "Zaokrągla liczbę w górę do najbliższej wartości całkowitej lub wielokrotności podanej istotności.",
		arguments: [
			{
				name: "liczba",
				description: "- wartość, która ma zostać zaokrąglona"
			},
			{
				name: "istotność",
				description: "- opcjonalna wielokrotność, do której należy zaokrąglić wartość"
			}
		]
	},
	{
		name: "ISPMT",
		description: "Zwraca wartość odsetek zapłaconych w trakcie określonego okresu lokaty.",
		arguments: [
			{
				name: "stopa",
				description: "- stopa oprocentowania dla okresu. Na przykład użyj stopy 6%/4 dla kwartalnych płatności w przypadku stopy 6% w stosunku rocznym"
			},
			{
				name: "okres",
				description: "- okres, dla którego mają być obliczone odsetki"
			},
			{
				name: "liczba_rat",
				description: "- liczba okresów płatności w czasie lokaty"
			},
			{
				name: "wa",
				description: "- wartość bieżąca lokaty, teraźniejsza łączna wartość serii przyszłych płatności"
			}
		]
	},
	{
		name: "JEŻELI",
		description: "Sprawdza, czy warunek jest spełniony, i zwraca jedną wartość, jeśli PRAWDA, a drugą wartość, jeśli FAŁSZ.",
		arguments: [
			{
				name: "test_logiczny",
				description: "- dowolna wartość lub wyrażenie, które można oszacować jako wartości PRAWDA albo FAŁSZ"
			},
			{
				name: "wartość_jeżeli_prawda",
				description: "- wartość zwracana, gdy test_logiczny ma wartość PRAWDA. W przypadku pominięcia zwracana jest wartość PRAWDA. Można zagnieździć do siedmiu funkcji JEŻELI"
			},
			{
				name: "wartość_jeżeli_fałsz",
				description: "- wartość zwracana, gdy test_logiczny ma wartość FAŁSZ. W przypadku pominięcia zwracana jest wartość FAŁSZ"
			}
		]
	},
	{
		name: "JEŻELI.BŁĄD",
		description: "Zwraca wartość wartość_jeśli_błąd, jeżeli wyrażenie jest błędne, lub wartość wyrażenia w przeciwnym razie.",
		arguments: [
			{
				name: "wartość",
				description: "- dowolna wartość, wyrażenie lub odwołanie"
			},
			{
				name: "wartość_jeśli_błąd",
				description: "- dowolna wartość, wyrażenie lub odwołanie"
			}
		]
	},
	{
		name: "JEŻELI.ND",
		description: "Zwraca określoną wartość, jeśli rozpoznawanie wyrażenia zakończy się błędem #N/D. W przeciwnym razie zostanie zwrócony wynik wyrażenia.",
		arguments: [
			{
				name: "wartość",
				description: "— dowolna wartość, wyrażenie lub odwołanie."
			},
			{
				name: "wartość_jeżeli_nd",
				description: "— dowolna wartość, wyrażenie lub odwołanie."
			}
		]
	},
	{
		name: "KOD",
		description: "Zwraca kod liczbowy pierwszego znaku w tekście odpowiadający zestawowi znaków używanemu w komputerze.",
		arguments: [
			{
				name: "tekst",
				description: "- tekst, z którego zostanie pobrany kod pierwszego znaku"
			}
		]
	},
	{
		name: "KOMBINACJE",
		description: "Zwraca liczbę kombinacji dla danej liczby elementów.",
		arguments: [
			{
				name: "liczba",
				description: "- łączna liczba elementów"
			},
			{
				name: "liczba_wybrana",
				description: "- liczba elementów w każdej kombinacji"
			}
		]
	},
	{
		name: "KOMBINACJE.A",
		description: "Zwraca liczbę kombinacji z powtórzeniami dla podanej liczby elementów.",
		arguments: [
			{
				name: "liczba",
				description: "— całkowita liczba elementów."
			},
			{
				name: "liczba_wybrana",
				description: "— liczba elementów w każdej kombinacji."
			}
		]
	},
	{
		name: "KOMÓRKA",
		description: "Zwraca informacje o formatowaniu, lokalizacji lub zawartości pierwszej komórki w odwołaniu zgodnie z kolejnością odczytu arkusza.",
		arguments: [
			{
				name: "info_typ",
				description: "- wartość tekstowa określająca, jakiego typu informacje o komórce są potrzebne."
			},
			{
				name: "odwołanie",
				description: "- komórka, o której chcesz uzyskać informacje"
			}
		]
	},
	{
		name: "KOMP.FUNKCJA.BŁ",
		description: "Zwraca komplementarną funkcję błędu.",
		arguments: [
			{
				name: "x",
				description: "– dolne ograniczenie przy wyznaczaniu ERF"
			}
		]
	},
	{
		name: "KOMP.FUNKCJA.BŁ.DOKŁ",
		description: "Zwraca komplementarną funkcję błędu.",
		arguments: [
			{
				name: "X",
				description: "- dolne ograniczenie przy wyznaczaniu wartości KOMP.FUNKCJA.BŁ.DOKŁ"
			}
		]
	},
	{
		name: "KONWERTUJ",
		description: "Zamienia liczbę z jednego systemu miar na inny.",
		arguments: [
			{
				name: "liczba",
				description: "– wartość w jednostkach wejściowych poddawana przekształceniu"
			},
			{
				name: "jednostka_we",
				description: "– jednostka dla przekształcanej liczby"
			},
			{
				name: "jednostka_wy",
				description: "– jednostka dla wyniku"
			}
		]
	},
	{
		name: "KOWARIANCJA",
		description: "Zwraca kowariancję, średnią z iloczynów odchyleń dla każdej pary punktów w dwóch zbiorach.",
		arguments: [
			{
				name: "tablica1",
				description: "- pierwszy zakres komórek zawierających liczby całkowite, muszą być to liczby, tablice lub odwołania zawierające liczby"
			},
			{
				name: "tablica2",
				description: "- drugi zakres komórek zawierających liczby całkowite, muszą być to liczby, tablice lub odwołania zawierające liczby"
			}
		]
	},
	{
		name: "KOWARIANCJA.POPUL",
		description: "Zwraca kowariancję populacji, czyli średnią iloczynów odchyleń dla każdej pary punktów danych w dwóch zbiorach danych.",
		arguments: [
			{
				name: "tablica1",
				description: "- pierwszy zakres komórek zawierających liczby całkowite (muszą to być liczby, tablice lub odwołania zawierające liczby)"
			},
			{
				name: "tablica2",
				description: "- drugi zakres komórek zawierających liczby całkowite (muszą to być liczby, tablice lub odwołania zawierające liczby)"
			}
		]
	},
	{
		name: "KOWARIANCJA.PRÓBKI",
		description: "Zwraca kowariancję próbki, czyli średnią iloczynów odchyleń dla każdej pary punktów danych w dwóch zbiorach danych.",
		arguments: [
			{
				name: "tablica1",
				description: "- pierwszy zakres komórek zawierających liczby całkowite (muszą to być liczby, tablice lub odwołania zawierające liczby)"
			},
			{
				name: "tablica2",
				description: "- drugi zakres komórek zawierających liczby całkowite (muszą to być liczby, tablice lub odwołania zawierające liczby)"
			}
		]
	},
	{
		name: "KURTOZA",
		description: "Zwraca kurtozę zbioru danych.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "liczba1",
				description: "- od 1 do 255 liczb lub nazw, tablic albo odwołań zawierających liczby, dla których ma być obliczona kurtoza"
			},
			{
				name: "liczba2",
				description: "- od 1 do 255 liczb lub nazw, tablic albo odwołań zawierających liczby, dla których ma być obliczona kurtoza"
			}
		]
	},
	{
		name: "KWARTYL",
		description: "Wyznacza kwartyl zbioru danych.",
		arguments: [
			{
				name: "tablica",
				description: "- tablica lub zakres komórek zawierający wartości liczbowe, dla których ma być znaleziony kwartyl"
			},
			{
				name: "kwarty",
				description: "- liczba: wartość minimalna = 0; 1. kwartyl = 1; wartość mediany = 2; 3. kwartyl =3; wartość maksymalna = 4"
			}
		]
	},
	{
		name: "KWARTYL.PRZEDZ.OTW",
		description: "Zwraca kwartyl zbioru danych na podstawie wartości percentylu z zakresu od 0 do 1 (bez wartości granicznych).",
		arguments: [
			{
				name: "tablica",
				description: "- tablica lub zakres komórek zawierających wartości liczbowe, dla których ma zostać znaleziona wartość kwartylu"
			},
			{
				name: "kwartyl",
				description: "- liczba: wartość minimalna = 0; 1. kwartyl = 1; wartość mediany = 2; 3. kwartyl = 3; wartość maksymalna = 4"
			}
		]
	},
	{
		name: "KWARTYL.PRZEDZ.ZAMK",
		description: "Zwraca kwartyl zbioru danych na podstawie wartości percentylu z zakresu od 0 do 1 włącznie.",
		arguments: [
			{
				name: "tablica",
				description: "- tablica lub zakres komórek zawierających wartości liczbowe, dla których ma zostać znaleziona wartość kwartylu"
			},
			{
				name: "kwartyl",
				description: "- liczba: wartość minimalna = 0; 1. kwartyl = 1; wartość mediany = 2; 3. kwartyl = 3; wartość maksymalna = 4"
			}
		]
	},
	{
		name: "KWOTA",
		description: "Konwertuje liczbę na tekst, korzystając z formatu walutowego.",
		arguments: [
			{
				name: "liczba",
				description: "- liczba, odwołanie do komórki zawierającej liczbę lub formuła dająca w wyniku liczbę"
			},
			{
				name: "miejsca_dziesiętne",
				description: "- liczba cyfr po przecinku. Liczba jest zaokrąglana zgodnie z wymaganiami; gdy jest pominięta, wynosi 2."
			}
		]
	},
	{
		name: "KWOTA.WYKUP",
		description: "Zwraca wartość kapitału otrzymanego przy wykupie papieru wartościowego całkowicie ulokowanego.",
		arguments: [
			{
				name: "rozliczenie",
				description: "– data rozliczenia papieru wartościowego, podana jako liczba seryjna"
			},
			{
				name: "data_spłaty",
				description: "– data spłaty papieru wartościowego, podana jako liczba seryjna"
			},
			{
				name: "lokata",
				description: "– kwota zainwestowana w papier wartościowy"
			},
			{
				name: "dyskonto",
				description: "– stopa dyskontowa papieru wartościowego"
			},
			{
				name: "podstawa",
				description: "– rodzaj podstawy wyliczania dni"
			}
		]
	},
	{
		name: "LEWY",
		description: "Zwraca określoną liczbę znaków od początku ciągu tekstowego.",
		arguments: [
			{
				name: "tekst",
				description: "- ciąg tekstowy zawierający znaki do wyodrębnienia"
			},
			{
				name: "liczba_znaków",
				description: "- określa, ile znaków ma być wyodrębnionych przez funkcję LEWY; jeśli pominięto, 1"
			}
		]
	},
	{
		name: "LICZ.JEŻELI",
		description: "Oblicza liczbę komórek we wskazanym zakresie spełniających podane kryteria.",
		arguments: [
			{
				name: "zakres",
				description: "- zakres komórek, w którym będą zliczane niepuste komórki"
			},
			{
				name: "kryteria",
				description: "- kryteria podane w formie liczby, wyrażenia lub tekstu, określające, które komórki będą uwzględniane przy zliczaniu"
			}
		]
	},
	{
		name: "LICZ.PUSTE",
		description: "Zlicza liczbę pustych komórek w określonym zakresie komórek.",
		arguments: [
			{
				name: "zakres",
				description: "- zakres, w którym zostaną zliczone puste komórki"
			}
		]
	},
	{
		name: "LICZ.WARUNKI",
		description: "Oblicza liczbę komórek spełniających podany zestaw warunków lub kryteriów.",
		arguments: [
			{
				name: "kryteria_zakres",
				description: "- zakres komórek, dla których należy sprawdzić określony warunek"
			},
			{
				name: "kryteria",
				description: "- warunek określający zliczane komórki, podany w postaci liczby, wyrażenia lub tekstu"
			}
		]
	},
	{
		name: "LICZBA.CAŁK",
		description: "Obcina liczbę do liczby całkowitej, usuwając część dziesiętną lub ułamkową.",
		arguments: [
			{
				name: "liczba",
				description: "- liczba, która ma zostać obcięta"
			},
			{
				name: "liczba_cyfr",
				description: "- liczba określająca dokładność obcięcia, 0 (zero) jeśli pominięta"
			}
		]
	},
	{
		name: "LICZBA.KOLUMN",
		description: "Zwraca liczbę kolumn w tablicy lub odwołaniu.",
		arguments: [
			{
				name: "tablica",
				description: "- tablica, formuła zwracająca tablicę lub odwołanie do zakresu komórek, dla których zostanie wyznaczona liczba kolumn"
			}
		]
	},
	{
		name: "LICZBA.ZESP",
		description: "Przekształca współczynniki rzeczywisty i urojony w liczbę zespoloną.",
		arguments: [
			{
				name: "część_rzecz",
				description: "– część rzeczywista liczby zespolonej"
			},
			{
				name: "część_uroj",
				description: "– część urojona liczby zespolonej"
			},
			{
				name: "jednostka_uroj",
				description: "– symbol jednostki urojonej wykorzystywanej w zapisie liczb zespolonych (i lub j)"
			}
		]
	},
	{
		name: "LITERY.MAŁE",
		description: "Konwertuje wszystkie litery w ciągu tekstowym na małe litery.",
		arguments: [
			{
				name: "tekst",
				description: "- tekst, który ma być konwertowany na małe litery. Znaki w tekście, które nie są literami, nie są zmieniane"
			}
		]
	},
	{
		name: "LITERY.WIELKIE",
		description: "Konwertuje ciąg tekstowy na wielkie litery.",
		arguments: [
			{
				name: "tekst",
				description: "- tekst, który ma zostać konwertowany na wielkie litery, odwołanie lub ciąg tekstowy"
			}
		]
	},
	{
		name: "LN",
		description: "Zwraca logarytm naturalny podanej liczby.",
		arguments: [
			{
				name: "liczba",
				description: "- dodatnia liczba rzeczywista, której logarytm naturalny ma zostać obliczony"
			}
		]
	},
	{
		name: "LN.LICZBY.ZESP",
		description: "Zwraca wartość logarytmu naturalnego liczby zespolonej.",
		arguments: [
			{
				name: "liczba_zesp",
				description: "– liczba zespolona poddana działaniu"
			}
		]
	},
	{
		name: "LOG",
		description: "Zwraca logarytm liczby przy podanej podstawie.",
		arguments: [
			{
				name: "liczba",
				description: "- dodatnia liczba rzeczywista, której logarytm ma zostać obliczony"
			},
			{
				name: "podstawa",
				description: "- podstawa logarytmu; jeśli pominięto, 10"
			}
		]
	},
	{
		name: "LOG10",
		description: "Oblicza logarytm dziesiętny podanej liczby.",
		arguments: [
			{
				name: "liczba",
				description: "- dodatnia liczba rzeczywista, której logarytm dziesiętny ma zostać obliczony"
			}
		]
	},
	{
		name: "LOG10.LICZBY.ZESP",
		description: "Zwraca wartość logarytmu dziesiętnego liczby zespolonej.",
		arguments: [
			{
				name: "liczba_zesp",
				description: "– liczba zespolona poddana działaniu"
			}
		]
	},
	{
		name: "LOG2.LICZBY.ZESP",
		description: "Zwraca wartość logarytmu przy podstawie 2 z liczby zespolonej.",
		arguments: [
			{
				name: "liczba_zesp",
				description: "– liczba zespolona poddana działaniu"
			}
		]
	},
	{
		name: "LOS",
		description: "Zwraca liczbę losową o równomiernym rozkładzie, która jest większa lub równa 0 i mniejsza niż 1 (zmienia się przy ponownym obliczaniu).",
		arguments: [
		]
	},
	{
		name: "LOS.ZAKR",
		description: "Zwraca liczbę losową z przedziału pomiędzy podanymi wartościami.",
		arguments: [
			{
				name: "dół",
				description: "– najmniejsza liczba całkowita, jak może zostać podana przez funkcję"
			},
			{
				name: "góra",
				description: "– największa liczba całkowita, jak może zostać podana przez funkcję"
			}
		]
	},
	{
		name: "LUB",
		description: "Sprawdza, czy którykolwiek z argumentów ma wartość PRAWDA i zwraca wartość PRAWDA albo FAŁSZ. Zwraca wartość FAŁSZ tylko wówczas, gdy wszystkie argumenty mają wartość FAŁSZ.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logiczna1",
				description: "- od 1 do 255 testowanych warunków logicznych mogących mieć wartość albo PRAWDA, albo FAŁSZ"
			},
			{
				name: "logiczna2",
				description: "- od 1 do 255 testowanych warunków logicznych mogących mieć wartość albo PRAWDA, albo FAŁSZ"
			}
		]
	},
	{
		name: "MACIERZ.ILOCZYN",
		description: "Zwraca iloczyn dwóch tablic, tablica o tej samej liczbie wierszy, co tablica1 i tej samej liczbie kolumn, co tablica2.",
		arguments: [
			{
				name: "tablica1",
				description: "- pierwsza tablica liczb do przemnożenia, musi mieć tę samą liczbę kolumn, co liczba wierszy Tablicy2"
			},
			{
				name: "tablica2",
				description: "- pierwsza tablica liczb do przemnożenia, musi mieć tę samą liczbę kolumn, co liczba wierszy Tablicy2"
			}
		]
	},
	{
		name: "MACIERZ.JEDNOSTKOWA",
		description: "Zwraca macierz jednostkową dla określonego wymiaru.",
		arguments: [
			{
				name: "wymiar",
				description: "- liczba całkowita określająca wymiar macierzy jednostkowej, która ma zostać zwrócona."
			}
		]
	},
	{
		name: "MACIERZ.ODW",
		description: "Zwraca macierz odwrotną do macierzy przechowywanej w tablicy.",
		arguments: [
			{
				name: "tablica",
				description: "- tablica liczbowa o równej liczbie wierszy i kolumn, albo zakres komórek lub stała tablicowa"
			}
		]
	},
	{
		name: "MAX",
		description: "Zwraca największą wartość ze zbioru wartości. Ignoruje wartości logiczne i tekst.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "liczba1",
				description: "- od 1 do 255 liczb, pustych komórek, wartości logicznych lub liczb w postaci tekstowej, których maksimum chcesz znaleźć"
			},
			{
				name: "liczba2",
				description: "- od 1 do 255 liczb, pustych komórek, wartości logicznych lub liczb w postaci tekstowej, których maksimum chcesz znaleźć"
			}
		]
	},
	{
		name: "MAX.A",
		description: "Zwraca największą wartość ze zbioru wartości. Nie pomija wartości logicznych i tekstu.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "wartość1",
				description: "- od 1 do 255 liczb, pustych komórek, wartości logicznych lub liczb w postaci tekstowej, których maksimum chcesz znaleźć"
			},
			{
				name: "wartość2",
				description: "- od 1 do 255 liczb, pustych komórek, wartości logicznych lub liczb w postaci tekstowej, których maksimum chcesz znaleźć"
			}
		]
	},
	{
		name: "MAX.K",
		description: "Zwraca k-tą największą wartość w zbiorze danych, na przykład piątą największą wartość.",
		arguments: [
			{
				name: "tablica",
				description: "- tablica lub zakres danych, których k-ta największa wartość ma być określona"
			},
			{
				name: "k",
				description: "- pozycja (licząc od największej wartości) w tablicy lub zakresie komórek wartości, która ma być zwrócona"
			}
		]
	},
	{
		name: "MEDIANA",
		description: "Zwraca medianę lub liczbę w środku zbioru podanych liczb.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "liczba1",
				description: "- od 1 do 255 liczb lub nazw, tablic albo odwołań zawierających liczby, dla których ma zostać wyznaczona mediana"
			},
			{
				name: "liczba2",
				description: "- od 1 do 255 liczb lub nazw, tablic albo odwołań zawierających liczby, dla których ma zostać wyznaczona mediana"
			}
		]
	},
	{
		name: "MIESIĄC",
		description: "Zwraca miesiąc, liczbę od 1 (styczeń) do 12 (grudzień).",
		arguments: [
			{
				name: "kolejna_liczba",
				description: "- liczba w kodzie data-godzina używanym w programie Spreadsheet"
			}
		]
	},
	{
		name: "MIN",
		description: "Zwraca najmniejszą wartość ze zbioru wartości. Ignoruje wartości logiczne i tekst.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "liczba1",
				description: "- od 1 do 255 liczb, pustych komórek, wartości logicznych lub liczb w postaci tekstowej, wśród których nastąpi wyszukanie liczby najmniejszej"
			},
			{
				name: "liczba2",
				description: "- od 1 do 255 liczb, pustych komórek, wartości logicznych lub liczb w postaci tekstowej, wśród których nastąpi wyszukanie liczby najmniejszej"
			}
		]
	},
	{
		name: "MIN.A",
		description: "Zwraca najmniejszą wartość ze zbioru wartości. Nie pomija wartości logicznych i tekstu.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "wartość1",
				description: "- od 1 do 255 liczb, pustych komórek, wartości logicznych lub liczb w postaci tekstowej, wśród których nastąpi wyszukanie liczby najmniejszej"
			},
			{
				name: "wartość2",
				description: "- od 1 do 255 liczb, pustych komórek, wartości logicznych lub liczb w postaci tekstowej, wśród których nastąpi wyszukanie liczby najmniejszej"
			}
		]
	},
	{
		name: "MIN.K",
		description: "Zwraca k-tą najmniejszą wartość w zbiorze danych, na przykład piątą najmniejszą liczbę.",
		arguments: [
			{
				name: "tablica",
				description: "- tablica lub zakres danych liczbowych, których k-ta najmniejsza wartość ma być określona"
			},
			{
				name: "k",
				description: "- pozycja (licząc od najmniejszej wartości) w tablicy lub zakresie komórek wartości, która ma być zwrócona"
			}
		]
	},
	{
		name: "MINUTA",
		description: "Zwraca minutę, liczbę od 0 do 59.",
		arguments: [
			{
				name: "kolejna_liczba",
				description: "- liczba w kodzie data-godzina używanym w programie Spreadsheet lub tekst w formacie godziny, na przykład 16:48:00 albo 4:48:00 PM"
			}
		]
	},
	{
		name: "MIRR",
		description: "Zwraca wewnętrzną stopę zwrotu dla serii okresowych przepływów gotówkowych przy uwzględnieniu kosztu inwestycji i stopy procentowej reinwestycji gotówki.",
		arguments: [
			{
				name: "wartości",
				description: "jest tablicą lub odwołaniem do komórek zawierających liczby, które reprezentują ciąg płatności (ujemne) i wpływów (dodatnie) w regularnych okresach"
			},
			{
				name: "stopa_finansowa",
				description: "jest oprocentowaniem jakie płacisz za pieniądze używane do przepływów gotówkowych"
			},
			{
				name: "stopa_reinwestycji",
				description: "jest oprocentowaniem jakie uzyskujesz przy przepływach gotówkowych, gdy je reinwestujesz"
			}
		]
	},
	{
		name: "MOD",
		description: "Zwraca resztę z dzielenia.",
		arguments: [
			{
				name: "liczba",
				description: "- liczba, dla której chcesz znaleźć resztę z dzielenia"
			},
			{
				name: "dzielnik",
				description: "- liczba, przez którą chcesz podzielić Liczbę"
			}
		]
	},
	{
		name: "MODUŁ.LICZBY",
		description: "Zwraca wartość bezwzględną liczby, wartość bez znaku.",
		arguments: [
			{
				name: "liczba",
				description: "- liczba rzeczywista, której wartość bezwzględna zostanie obliczona"
			}
		]
	},
	{
		name: "MODUŁ.LICZBY.ZESP",
		description: "Zwraca wartość bezwzględną (moduł) liczby zespolonej.",
		arguments: [
			{
				name: "liczba_zesp",
				description: "– liczba zespolona poddana działaniu"
			}
		]
	},
	{
		name: "N",
		description: "Konwertuje wartości nieliczbowe na liczby, daty na liczby kolejne, wartość PRAWDA na 1, wszystko inne na 0 (zero).",
		arguments: [
			{
				name: "wartość",
				description: "- wartość, która ma być konwertowana"
			}
		]
	},
	{
		name: "NACHYLENIE",
		description: "Zwraca nachylenie wykresu regresji liniowej przez zadane punkty danych.",
		arguments: [
			{
				name: "znane_y",
				description: "- tablica lub zakres komórek zależnych punktów danych liczbowych, może zawierać liczby lub nazwy, tablice albo odwołania zawierające liczby"
			},
			{
				name: "znane_x",
				description: "- tablica lub zakres komórek niezależnych punktów danych liczbowych, może zawierać liczby lub nazwy, tablice albo odwołania zawierające liczby"
			}
		]
	},
	{
		name: "NAJMN.WSP.WIEL",
		description: "Zwraca najmniejszą wspólną wielokrotność.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "liczba1",
				description: "- sekwencja od 1 do 255 liczb, dla których należy obliczyć najmniejszą wspólną wielokrotność"
			},
			{
				name: "liczba2",
				description: "- sekwencja od 1 do 255 liczb, dla których należy obliczyć najmniejszą wspólną wielokrotność"
			}
		]
	},
	{
		name: "NAJW.WSP.DZIEL",
		description: "Zwraca największy wspólny dzielnik.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "liczba1",
				description: "- sekwencja od 1 do 255 wartości"
			},
			{
				name: "liczba2",
				description: "- sekwencja od 1 do 255 wartości"
			}
		]
	},
	{
		name: "NAL.ODS.WYKUP",
		description: "Zwraca wartość procentu składanego dla papieru wartościowego oprocentowanego przy wykupie.",
		arguments: [
			{
				name: "emisja",
				description: "– data emisji papieru wartościowego, podana jako liczba seryjna"
			},
			{
				name: "rozliczenie",
				description: "– data rozliczenia papieru wartościowego, podana jako liczba seryjna"
			},
			{
				name: "stopa",
				description: "– roczne oprocentowanie kuponu papieru wartościowego"
			},
			{
				name: "cena_nom",
				description: "– wartość nominalna papieru wartościowego"
			},
			{
				name: "podstawa",
				description: "– rodzaj podstawy wyliczania dni"
			}
		]
	},
	{
		name: "NIE",
		description: "Zmienia wartość FAŁSZ na PRAWDA albo wartość PRAWDA na FAŁSZ.",
		arguments: [
			{
				name: "logiczna",
				description: "- wartość lub wyrażenie, które można oszacować jako PRAWDA albo FAŁSZ"
			}
		]
	},
	{
		name: "NOMINALNA",
		description: "Zwraca wartość minimalnej rocznej stopy oprocentowania.",
		arguments: [
			{
				name: "stopa_efektywna",
				description: "– efektywna stopa procentowa"
			},
			{
				name: "okresy",
				description: "– liczba okresów składanych w roku"
			}
		]
	},
	{
		name: "NORMALIZUJ",
		description: "Zwraca wartość znormalizowaną z rozkładu scharakteryzowanego przez średnią i odchylenie standardowe.",
		arguments: [
			{
				name: "x",
				description: "- wartość, którą chcesz znormalizować"
			},
			{
				name: "średnia",
				description: "- średnia arytmetyczna danego rozkładu"
			},
			{
				name: "odchylenie_std",
				description: "- odchylenie standardowe danego rozkładu, liczba dodatnia"
			}
		]
	},
	{
		name: "NPER",
		description: "Zwraca liczbę okresów dla lokaty opartej na okresowych, stałych wpłatach przy stałym oprocentowaniu.",
		arguments: [
			{
				name: "stopa",
				description: "- stopa oprocentowania dla okresu. Na przykład użyj stopy 6%/4 dla płatności kwartalnych w przypadku stopy 6% w stosunku rocznym"
			},
			{
				name: "rata",
				description: "- płatność okresowa, niezmienna przez cały czas trwania lokaty"
			},
			{
				name: "wa",
				description: "- wartość bieżąca lokaty, teraźniejsza łączna wartość serii przyszłych płatności"
			},
			{
				name: "wp",
				description: "- wartość przyszła lub saldo gotówkowe, jakie chcesz uzyskać po dokonaniu ostatniej płatności. Jeśli pominięta, używane jest zero"
			},
			{
				name: "typ",
				description: "- wartość logiczna: płatność na początku okresu = 1; płatność na końcu okresu = 0 lub pominięta"
			}
		]
	},
	{
		name: "NPV",
		description: "Oblicza wartość bieżącą netto inwestycji w oparciu o okresowe przepływy środków pieniężnych przy określonej stopie dyskontowej i serii przyszłych płatności (wartości ujemne) i wpływów (wartości dodatnie).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "stopa",
				description: "- stopa dyskontowa dla podstawowego okresu płatności"
			},
			{
				name: "wartość1",
				description: "- od 1 do 254 argumentów reprezentujących płatności i wpływy równo rozłożone w czasie i występujące na końcu każdego okresu"
			},
			{
				name: "wartość2",
				description: "- od 1 do 254 argumentów reprezentujących płatności i wpływy równo rozłożone w czasie i występujące na końcu każdego okresu"
			}
		]
	},
	{
		name: "NR.BŁĘDU",
		description: "Zwraca numer odpowiadający jednej z wartości błędu.",
		arguments: [
			{
				name: "błąd_wartość",
				description: "- wartość błędu, którego numer identyfikacyjny chcesz uzyskać; może to być rzeczywista wartość błędu lub odwołanie do komórki zawierającej wartość błędu"
			}
		]
	},
	{
		name: "NR.KOLUMNY",
		description: "Zwraca numer kolumny odpowiadający podanemu odwołaniu.",
		arguments: [
			{
				name: "odwołanie",
				description: "- komórka lub zakres komórek, których numer kolumny zostanie wyznaczony. Jeśli pominięty, używana jest komórka zawierająca funkcję KOLUMNA"
			}
		]
	},
	{
		name: "NR.SER.DATY",
		description: "Zwraca wartość liczby seryjnej daty, przypadającej podaną liczbę miesięcy przed lub po dacie początkowej.",
		arguments: [
			{
				name: "data_pocz",
				description: "– liczba seryjna reprezentująca datę początkową"
			},
			{
				name: "miesiące",
				description: "– liczba miesięcy przed lub po dacie początkowej"
			}
		]
	},
	{
		name: "NR.SER.OST.DN.MIES",
		description: "Zwraca wartość liczby seryjnej daty ostatniego dnia miesiąca przed lub po podanej liczbie miesięcy.",
		arguments: [
			{
				name: "data_pocz",
				description: "– liczba seryjna reprezentująca datę początkową"
			},
			{
				name: "miesiące",
				description: "– liczba miesięcy przed lub po dacie początkowej"
			}
		]
	},
	{
		name: "NUM.TYG",
		description: "Zwraca numer tygodnia w roku.",
		arguments: [
			{
				name: "liczba_seryjna",
				description: "– liczba seryjna daty i czasy, używana przez program Spreadsheet przy przeliczaniu daty i czasu"
			},
			{
				name: "typ_wyniku",
				description: "– liczba (1 lub 2) określająca typ zwracanej wartości"
			}
		]
	},
	{
		name: "O.CZAS.TRWANIA",
		description: "Zwraca liczbę okresów wymaganych przez inwestycję do osiągnięcia określonej wartości.",
		arguments: [
			{
				name: "stopa",
				description: "— stopa procentowa dla okresu."
			},
			{
				name: "wb",
				description: "— wartość bieżąca inwestycji."
			},
			{
				name: "wp",
				description: "— żądana przyszła wartość inwestycji."
			}
		]
	},
	{
		name: "OBSZARY",
		description: "Zwraca liczbę obszarów wskazywanych w odwołaniu. Obszar jest ciągłym zakresem komórek lub pojedynczą komórką.",
		arguments: [
			{
				name: "odwołanie",
				description: "- odwołanie do komórki lub zakresu komórek; może odnosić się do wielu obszarów"
			}
		]
	},
	{
		name: "OCZYŚĆ",
		description: "Usuwa z tekstu wszystkie znaki, które nie mogą być drukowane.",
		arguments: [
			{
				name: "tekst",
				description: "- dowolne informacje z arkusza, z których mają być usunięte niedrukowalne znaki"
			}
		]
	},
	{
		name: "ODCH.KWADRATOWE",
		description: "Zwraca sumę kwadratów odchyleń punktów danych od średniej arytmetycznej z próbki.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "liczba1",
				description: "- od 1 do 255 argumentów, lub tablica albo odwołanie do tablicy, dla której ma zostać obliczona suma kwadratów odchyleń"
			},
			{
				name: "liczba2",
				description: "- od 1 do 255 argumentów, lub tablica albo odwołanie do tablicy, dla której ma zostać obliczona suma kwadratów odchyleń"
			}
		]
	},
	{
		name: "ODCH.ŚREDNIE",
		description: "Zwraca odchylenie średnie (średnia z odchyleń bezwzględnych) punktów danych od ich wartości średniej. Argumentami mogą być liczby lub nazwy, tablice albo odwołania zawierające liczby.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "liczba1",
				description: "- od 1 do 255 argumentów, dla których ma zostać obliczona średnia z odchyleń bezwzględnych"
			},
			{
				name: "liczba2",
				description: "- od 1 do 255 argumentów, dla których ma zostać obliczona średnia z odchyleń bezwzględnych"
			}
		]
	},
	{
		name: "ODCH.STAND.POPUL",
		description: "Oblicza odchylenie standardowe w oparciu o całą populację zadaną jako argument (pomija wartości logiczne i tekstowe).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "liczba1",
				description: "- od 1 do 255 liczb odpowiadających populacji, mogą być to liczby lub odwołania zawierające liczby"
			},
			{
				name: "liczba2",
				description: "- od 1 do 255 liczb odpowiadających populacji, mogą być to liczby lub odwołania zawierające liczby"
			}
		]
	},
	{
		name: "ODCH.STANDARD.POPUL",
		description: "Oblicza odchylenie standardowe na podstawie całej populacji zadanej jako argument (pomija wartości logiczne i tekstowe).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "liczba1",
				description: "- od 1 do 255 liczb odpowiadających populacji, mogą być to liczby lub odwołania zawierające liczby"
			},
			{
				name: "liczba2",
				description: "- od 1 do 255 liczb odpowiadających populacji, mogą być to liczby lub odwołania zawierające liczby"
			}
		]
	},
	{
		name: "ODCH.STANDARD.POPUL.A",
		description: "Oblicza odchylenie standardowe w oparciu o całą populację, włącznie z wartościami logicznymi i tekstem. Teksty i wartości logiczne FAŁSZ są traktowane jako 0; logiczna wartość PRAWDA jest traktowana jako 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "wartość1",
				description: "od 1 do 255 wartości odpowiadających populacji; mogą być wartościami, nazwami i odwołaniami do adresów zawierających wartości"
			},
			{
				name: "wartość2",
				description: "od 1 do 255 wartości odpowiadających populacji; mogą być wartościami, nazwami i odwołaniami do adresów zawierających wartości"
			}
		]
	},
	{
		name: "ODCH.STANDARD.PRÓBKI",
		description: "Dokonuje oszacowania odchylenia standardowego dla podanej próbki (pomija wartości logiczne i tekstowe w próbce).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "liczba1",
				description: "- od 1 do 255 liczb odpowiadających próbce populacji, mogą być to liczby lub odwołania zawierające liczby"
			},
			{
				name: "liczba2",
				description: "- od 1 do 255 liczb odpowiadających próbce populacji, mogą być to liczby lub odwołania zawierające liczby"
			}
		]
	},
	{
		name: "ODCH.STANDARDOWE",
		description: "Dokonuje oszacowania odchylenia standardowego dla podanej próbki (pomija wartości logiczne i tekstowe w próbce).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "liczba1",
				description: "- od 1 do 255 liczb odpowiadających próbce populacji, mogą być to liczby lub odwołania zawierające liczby"
			},
			{
				name: "liczba2",
				description: "- od 1 do 255 liczb odpowiadających próbce populacji, mogą być to liczby lub odwołania zawierające liczby"
			}
		]
	},
	{
		name: "ODCH.STANDARDOWE.A",
		description: "Szacuje odchylenie standardowe na podstawie próbki uwzględniając wartości logiczne oraz tekst. Wartość logiczna FAŁSZ i wartości tekstowe są traktowane jako 0, a wartość logiczna PRAWDA jako 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "wartość1",
				description: "- od 1 do 255 wartości odpowiadających próbce populacji; mogą to być wartościami, nazwami i odwołaniami do adresów zawierających wartości"
			},
			{
				name: "wartość2",
				description: "- od 1 do 255 wartości odpowiadających próbce populacji; mogą to być wartościami, nazwami i odwołaniami do adresów zawierających wartości"
			}
		]
	},
	{
		name: "ODCIĘTA",
		description: "Oblicza miejsce przecięcia się linii z osią y, używając linii najlepszego dopasowania przechodzącej przez znane wartości x i y.",
		arguments: [
			{
				name: "znane_y",
				description: "- zbiór danych lub obserwacji zależnych, może zawierać liczby lub nazwy, tablice albo odwołania zawierające liczby"
			},
			{
				name: "znane_x",
				description: "- zbiór danych lub obserwacji niezależnych, może zawierać liczby lub nazwy, tablice albo odwołania zawierające liczby"
			}
		]
	},
	{
		name: "ORAZ",
		description: "Sprawdza, czy wszystkie argumenty mają wartość PRAWDA, i zwraca wartość PRAWDA, jeśli wszystkie argumenty mają wartość PRAWDA.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logiczna1",
				description: "- od 1 do 255 testowanych warunków, które mogą mieć wartość PRAWDA albo FAŁSZ i są wartościami logicznymi, tablicami lub odwołaniami"
			},
			{
				name: "logiczna2",
				description: "- od 1 do 255 testowanych warunków, które mogą mieć wartość PRAWDA albo FAŁSZ i są wartościami logicznymi, tablicami lub odwołaniami"
			}
		]
	},
	{
		name: "ÓSM.NA.DWÓJK",
		description: "Zamienia liczbę ósemkową na liczbę w kodzie dwójkowym.",
		arguments: [
			{
				name: "liczba",
				description: "– liczba ósemkowa poddana przekształceniu"
			},
			{
				name: "miejsca",
				description: "– liczba znaków dla wartości wynikowej"
			}
		]
	},
	{
		name: "ÓSM.NA.DZIES",
		description: "Przekształca liczbę ósemkową na dziesiętną.",
		arguments: [
			{
				name: "liczba",
				description: "– liczba ósemkowa poddana przekształceniu"
			}
		]
	},
	{
		name: "ÓSM.NA.SZESN",
		description: "Zamienia liczbę ósemkową na liczbę w kodzie szesnastkowym.",
		arguments: [
			{
				name: "liczba",
				description: "– liczba ósemkowa poddana przekształceniu"
			},
			{
				name: "miejsca",
				description: "– liczba znaków dla wartości wynikowej"
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
		name: "PEARSON",
		description: "Zwraca współczynnik korelacji momentów iloczynu Pearsona, r.",
		arguments: [
			{
				name: "tablica1",
				description: "- zbiór wartości niezależnych"
			},
			{
				name: "tablica2",
				description: "- zbiór wartości zależnych"
			}
		]
	},
	{
		name: "PERCENTYL",
		description: "Wyznacza k-ty percentyl wartości w zakresie.",
		arguments: [
			{
				name: "tablica",
				description: "- tablica lub zakres danych definiujących pozycyjność względną"
			},
			{
				name: "k",
				description: "- wartość percentylu z przedziału od 0 do 1 włącznie"
			}
		]
	},
	{
		name: "PERCENTYL.PRZEDZ.OTW",
		description: "Zwraca k-ty percentyl wartości w zakresie, gdzie k należy do zakresu od 0 do 1 (bez wartości granicznych).",
		arguments: [
			{
				name: "tablica",
				description: "- tablica lub zakres danych definiujących pozycyjność względną"
			},
			{
				name: "k",
				description: "- wartość percentylu, należąca do zakresu od 0 do 1 (bez wartości granicznych)"
			}
		]
	},
	{
		name: "PERCENTYL.PRZEDZ.ZAMK",
		description: "Zwraca k-ty percentyl wartości w zakresie, gdzie k należy do zakresu od 0 do 1 włącznie.",
		arguments: [
			{
				name: "tablica",
				description: "- tablica lub zakres danych definiujących pozycyjność względną"
			},
			{
				name: "k",
				description: "- wartość percentylu, należąca do zakresu od 0 do 1 włącznie"
			}
		]
	},
	{
		name: "PERMUTACJE",
		description: "Zwraca liczbę permutacji dla podanej liczby obiektów, które można wybrać ze wszystkich obiektów.",
		arguments: [
			{
				name: "liczba",
				description: "- całkowita liczba obiektów"
			},
			{
				name: "wybór_liczba",
				description: "- wartość całkowita określająca liczbę elementów w każdej permutacji"
			}
		]
	},
	{
		name: "PERMUTACJE.A",
		description: "Zwraca liczbę permutacji dla podanej liczby obiektów (z powtórzeniami), które można wybrać ze wszystkich obiektów.",
		arguments: [
			{
				name: "liczba",
				description: "— całkowita liczba obiektów."
			},
			{
				name: "liczba_wybrana",
				description: "— liczba obiektów w każdej permutacji."
			}
		]
	},
	{
		name: "PHI",
		description: "Zwraca wartość funkcji gęstości dla standardowego rozkładu normalnego.",
		arguments: [
			{
				name: "x",
				description: "- liczba, dla której ma zostać obliczona gęstość standardowego rozkładu normalnego."
			}
		]
	},
	{
		name: "PI",
		description: "Zwraca wartość liczby Pi, 3,14159265358979 z dokładnością do 15 cyfr po przecinku.",
		arguments: [
		]
	},
	{
		name: "PIERW.PI",
		description: "Zwraca pierwiastek kwadratowy z wartości (liczba * pi).",
		arguments: [
			{
				name: "liczba",
				description: "– liczba mnożona przez pi"
			}
		]
	},
	{
		name: "PIERWIASTEK",
		description: "Zwraca pierwiastek kwadratowy liczby.",
		arguments: [
			{
				name: "liczba",
				description: "- liczba, dla której chcesz uzyskać pierwiastek kwadratowy"
			}
		]
	},
	{
		name: "PIERWIASTEK.LICZBY.ZESP",
		description: "Zwraca wartość pierwiastka kwadratowego liczby zespolonej.",
		arguments: [
			{
				name: "liczba_zesp",
				description: "– liczba zespolona poddana działaniu"
			}
		]
	},
	{
		name: "PMT",
		description: "Oblicza ratę spłaty pożyczki opartej na stałych ratach i stałym oprocentowaniu.",
		arguments: [
			{
				name: "stopa",
				description: "- stopa oprocentowania dla okresu pożyczki. Na przykład użyj stopy 6%/4 dla płatności kwartalnych w przypadku stopy 6% w stosunku rocznym"
			},
			{
				name: "liczba_rat",
				description: "- liczba wszystkich płatności pożyczki"
			},
			{
				name: "wa",
				description: "- wartość bieżąca lokaty, teraźniejsza łączna wartość serii przyszłych płatności"
			},
			{
				name: "wp",
				description: "- wartość przyszła lub saldo gotówkowe, jakie chcesz uzyskać po dokonaniu ostatniej płatności, 0 (zero), jeśli pominięta"
			},
			{
				name: "typ",
				description: "- wartość logiczna: płatność na początku okresu = 1; płatność na końcu okresu = 0 lub pominięta"
			}
		]
	},
	{
		name: "PODAJ.POZYCJĘ",
		description: "Zwraca względną pozycję elementu w tablicy, odpowiadającą określonej wartości przy podanej kolejności.",
		arguments: [
			{
				name: "szukana_wartość",
				description: "- wartość używana do znalezienia żądanej wartości w tablicy, liczba, tekst lub wartość logiczna albo odwołanie do jednej z takich wartości"
			},
			{
				name: "przeszukiwana_tab",
				description: "- ciągły zakres komórek zawierający możliwe wartości wyszukiwania, tablica wartości lub odwołanie do tablicy"
			},
			{
				name: "typ_porównania",
				description: "- liczba 1, 0 albo -1 wskazująca, która wartość ma być zwrócona."
			}
		]
	},
	{
		name: "PODSTAW",
		description: "Zamienia istniejący tekst w ciągu nowym tekstem.",
		arguments: [
			{
				name: "tekst",
				description: "- tekst lub odwołanie do komórki zawierającej tekst, w którym nastąpi podstawienie znaków"
			},
			{
				name: "stary_tekst",
				description: "- tekst, który ma zostać zamieniony. Jeśli wielkość liter starego_tekstu nie odpowiada wielkości liter danego tekstu, tekst nie zostanie zamieniony"
			},
			{
				name: "nowy_tekst",
				description: "- tekst, który ma zastąpić stary_tekst"
			},
			{
				name: "wystapienie_liczba",
				description: "- określa, które wystąpienie starego_tekstu ma zostać zamienione przez nowy_tekst. Jeśli pominięto, zamienione zostanie każde wystąpienie"
			}
		]
	},
	{
		name: "PODSTAWA",
		description: "Konwertuje liczbę na reprezentację tekstową o podanej podstawie.",
		arguments: [
			{
				name: "liczba",
				description: "— liczba, która ma zostać przekonwertowana."
			},
			{
				name: "podstawa",
				description: "— podstawa, do której liczba ma zostać przekonwertowana."
			},
			{
				name: "długość_min",
				description: "— minimalna długość zwracanego ciągu. Jeśli pominięto, wiodące zera nie zostaną dodane."
			}
		]
	},
	{
		name: "PORÓWNAJ",
		description: "Sprawdza, czy dwa ciągi tekstowe są identyczne, i zwraca wartość PRAWDA albo FAŁSZ. Funkcja PORÓWNAJ uwzględnia wielkość znaków.",
		arguments: [
			{
				name: "tekst1",
				description: "- pierwszy ciąg znaków"
			},
			{
				name: "tekst2",
				description: "- drugi ciąg znaków"
			}
		]
	},
	{
		name: "POTĘGA",
		description: "Zwraca liczbę podniesioną do potęgi.",
		arguments: [
			{
				name: "liczba",
				description: "- podstawa potęgi, dowolna liczba rzeczywista"
			},
			{
				name: "potęga",
				description: "- wykładnik, do którego zostanie podniesiona podstawa"
			}
		]
	},
	{
		name: "POTĘGA.LICZBY.ZESP",
		description: "Zwraca wartość liczby zespolonej podniesionej do potęgi całkowitej.",
		arguments: [
			{
				name: "liczba_zesp",
				description: "– liczba zespolona poddana działaniu"
			},
			{
				name: "liczba",
				description: "– wykładnik potęgi, do której zostanie podniesiona liczba zespolona"
			}
		]
	},
	{
		name: "POWT",
		description: "Powtarza tekst podaną liczbę razy. Używaj funkcji POWT do wypełnienia komórki podaną liczbą wystąpień ciągu tekstowego.",
		arguments: [
			{
				name: "tekst",
				description: "- tekst, który ma być powtarzany"
			},
			{
				name: "ile_razy",
				description: "- liczba dodatnia, określająca ile razy należy powtórzyć dany tekst"
			}
		]
	},
	{
		name: "POZYCJA",
		description: "Zwraca pozycję liczby na liście liczb: jej rozmiar względem innych wartości na liście.",
		arguments: [
			{
				name: "liczba",
				description: "- liczba, której pozycję chcesz znaleźć"
			},
			{
				name: "lista",
				description: "- tablica lub adres odnoszący się do listy z liczbami. Wartości nieliczbowe są ignorowane"
			},
			{
				name: "lp",
				description: "- liczba: pozycja na liście sortowanej malejąco = 0 lub pominięcie; pozycja na liście sortowanej rosnąco = dowolna wartość niezerowa"
			}
		]
	},
	{
		name: "POZYCJA.NAJW",
		description: "Zwraca pozycję liczby na liście liczb: jej wielkość względem innych wartości na liście; jeśli więcej niż jedna wartość ma taką samą pozycję, jest zwracana najwyższa pozycja zbioru wartości.",
		arguments: [
			{
				name: "liczba",
				description: "- liczba, której pozycję należy znaleźć"
			},
			{
				name: "odwołanie",
				description: "- tablica lub odwołanie do listy liczb. Wartości nieliczbowe są ignorowane"
			},
			{
				name: "lp",
				description: "- liczba: pozycja na liście sortowanej malejąco = 0 lub pominięcie; pozycja na liście sortowanej rosnąco = dowolna wartość niezerowa"
			}
		]
	},
	{
		name: "POZYCJA.ŚR",
		description: "Zwraca pozycję liczby na liście liczb: jej wielkość względem innych wartości na liście; jeśli więcej niż jedna wartość ma taką samą pozycję, jest zwracana średnia pozycja.",
		arguments: [
			{
				name: "liczba",
				description: "- liczba, której pozycję należy znaleźć"
			},
			{
				name: "odwołanie",
				description: "- tablica lub odwołanie do listy liczb. Wartości nieliczbowe są ignorowane"
			},
			{
				name: "lp",
				description: "- liczba: pozycja na liście sortowanej malejąco = 0 lub pominięcie; pozycja na liście sortowanej rosnąco = dowolna wartość niezerowa"
			}
		]
	},
	{
		name: "PPMT",
		description: "Zwraca wartość spłaty kapitału dla danej lokaty przy założeniu okresowych, stałych rat i stałego oprocentowania.",
		arguments: [
			{
				name: "stopa",
				description: "- stopa oprocentowania dla okresu. Na przykład użyj stopy 6%/4 dla kwartalnych płatności w przypadku stopy 6% w stosunku rocznym"
			},
			{
				name: "okres",
				description: "- określa okres i musi zawierać się w zakresie między 1 a liczba_rat"
			},
			{
				name: "liczba_rat",
				description: "- liczba wszystkich okresów płatności w całym czasie lokaty"
			},
			{
				name: "wa",
				description: "- wartość bieżąca: teraźniejsza łączna wartość serii przyszłych rat"
			},
			{
				name: "wp",
				description: "- wartość przyszła lub saldo gotówkowe, jakie ma być uzyskane po płatności ostatniej raty"
			},
			{
				name: "typ",
				description: "- wartość logiczna: płatność na początku okresu = 1; płatność na końcu okresu = 0 lub pominięta"
			}
		]
	},
	{
		name: "PRAWDA",
		description: "Zwraca wartość logiczną PRAWDA.",
		arguments: [
		]
	},
	{
		name: "PRAWDPD",
		description: "Zwraca prawdopodobieństwo, że wartości w zakresie znajdują się między dwoma granicami lub są równe granicy dolnej.",
		arguments: [
			{
				name: "zakres_x",
				description: "- zakres wartości liczbowych zmiennej x, z którymi powiązane są odpowiednie prawdopodobieństwa"
			},
			{
				name: "zakres_prawdop",
				description: "- zbiór prawdopodobieństw powiązanych z wartościami w ciągu zakres_x, wartości między 0 i 1 z wyłączeniem 0"
			},
			{
				name: "dolna_granica",
				description: "- dolna granica przedziału wartości, dla której ma zostać wyznaczone prawdopodobieństwo"
			},
			{
				name: "górna_granica",
				description: "- opcjonalna górna granica wartości. Jeśli pominięta, PROB zwraca prawdopodobieństwo, że wartości x_zakres są równe dolnej_granicy"
			}
		]
	},
	{
		name: "PRAWY",
		description: "Zwraca określoną liczbę znaków od końca ciągu tekstowego.",
		arguments: [
			{
				name: "tekst",
				description: "- tekst zawierający znaki do wyodrębnienia"
			},
			{
				name: "liczba_znaków",
				description: "określa, ile znaków ma być wyodrębnionych; jeśli pominięto, 1"
			}
		]
	},
	{
		name: "PROC.POZ.PRZEDZ.OTW",
		description: "Zwraca pozycję procentową wartości w zbiorze danych, należącą do zakresu od 0 do 1 (bez wartości granicznych).",
		arguments: [
			{
				name: "tablica",
				description: "- tablica lub zakres danych o wartościach liczbowych definiujących pozycyjność względną"
			},
			{
				name: "x",
				description: "- wartość, której pozycja ma zostać znaleziona"
			},
			{
				name: "istotność",
				description: "- opcjonalna wartość określająca liczbę cyfr znaczących zwracanej wartości procentowej; trzy cyfry, jeśli pominięto (0,xxx%)"
			}
		]
	},
	{
		name: "PROC.POZ.PRZEDZ.ZAMK",
		description: "Zwraca pozycję procentową wartości w zbiorze danych, należącą do zakresu od 0 do 1 włącznie.",
		arguments: [
			{
				name: "tablica",
				description: "- tablica lub zakres danych o wartościach liczbowych definiujących pozycyjność względną"
			},
			{
				name: "x",
				description: "- wartość, której pozycja ma zostać znaleziona"
			},
			{
				name: "istotność",
				description: "- opcjonalna wartość określająca liczbę cyfr znaczących zwracanej wartości procentowej; trzy cyfry, jeśli pominięto (0,xxx%)"
			}
		]
	},
	{
		name: "PROCENT.POZYCJA",
		description: "Wyznacza pozycję procentową wartości w zbiorze danych.",
		arguments: [
			{
				name: "tablica",
				description: "- tablica lub zakres danych o wartościach liczbowych definiujących pozycyjność względną"
			},
			{
				name: "x",
				description: "- wartość, której pozycja ma być znaleziona"
			},
			{
				name: "istotność",
				description: "- opcjonalna wartość określająca liczbę cyfr znaczących otrzymanej wartości procentowej, trzy cyfry jeśli pominięto (0,xxx%)"
			}
		]
	},
	{
		name: "PRÓG.ROZKŁAD.DWUM",
		description: "Zwraca najmniejszą wartość, dla której skumulowany rozkład dwumianowy jest większy lub równy podanej wartości progowej.",
		arguments: [
			{
				name: "próby",
				description: "- liczba prób Bernoulliego"
			},
			{
				name: "prawdopodob_s",
				description: "- prawdopodobieństwo sukcesu w pojedynczej próbie, liczba z przedziału od 0 do 1 włącznie"
			},
			{
				name: "alfa",
				description: "- wartość progowa, liczba z przedziału od 0 do 1 włącznie"
			}
		]
	},
	{
		name: "PRZESUNIĘCIE",
		description: "Zwraca odwołanie do zakresu, który jest daną liczbą wierszy lub kolumn z danego odwołania.",
		arguments: [
			{
				name: "odwołanie",
				description: "- odwołanie, od którego określane jest przesunięcie, odwołanie do komórki lub zakresu sąsiadujących komórek"
			},
			{
				name: "wiersze",
				description: "- liczba wierszy w dół lub w górę, do których ma się odwoływać lewa górna komórka wyniku"
			},
			{
				name: "kolumny",
				description: "- liczba kolumn w lewo lub w prawo, do których ma się odwoływać lewa górna komórka wyniku"
			},
			{
				name: "wysokość",
				description: "- wysokość mierzona liczbą wierszy, którą ma mieć wynik, taka sama jak odwołanie, jeśli została pominięta"
			},
			{
				name: "szerokość",
				description: "- szerokość mierzona liczbą kolumn, którą ma mieć wynik, taka sama jak odwołanie, jeśli została pominięta"
			}
		]
	},
	{
		name: "PV",
		description: "Zwraca wartość bieżącą inwestycji: całkowita obecna wartość serii przyszłych płatności.",
		arguments: [
			{
				name: "stopa",
				description: "- stopa oprocentowania dla okresu. Na przykład użyj stopy 6%/4 w przypadku płatności kwartalnych w przypadku stopy 6% w stosunku rocznym"
			},
			{
				name: "liczba_rat",
				description: "- liczba wszystkich okresów płatności lokaty"
			},
			{
				name: "rata",
				description: "- płatność okresowa, niezmienna przez cały czas trwania lokaty"
			},
			{
				name: "wp",
				description: "- wartość przyszła lub saldo gotówkowe, jakie chcesz uzyskać po dokonaniu ostatniej płatności"
			},
			{
				name: "typ",
				description: "- wartość logiczna: płatność na początku okresu = 1; płatność na końcu okresu = 0 lub pominięta"
			}
		]
	},
	{
		name: "R.KWADRAT",
		description: "Zwraca kwadrat współczynnika Pearsona korelacji iloczynu momentów dla zadanych punktów danych.",
		arguments: [
			{
				name: "znane_y",
				description: "- tablica lub zakres punktów danych, może zawierać liczby lub nazwy, tablice albo odwołania zawierające liczby"
			},
			{
				name: "znane_x",
				description: "- tablica lub zakres punktów danych, może zawierać liczby lub nazwy, tablice albo odwołania zawierające liczby"
			}
		]
	},
	{
		name: "RADIANY",
		description: "Konwertuje stopnie na radiany.",
		arguments: [
			{
				name: "kąt",
				description: "- kąt podany w stopniach, który ma być poddany konwersji"
			}
		]
	},
	{
		name: "RATE",
		description: "Zwraca stopę procentową okresu pożyczki lub lokaty. Na przykład użyj stopy 6%/4 dla płatności kwartalnych w przypadku stopy 6% w stosunku rocznym.",
		arguments: [
			{
				name: "liczba_rat",
				description: "- liczba wszystkich okresów płatności pożyczki lub lokaty"
			},
			{
				name: "rata",
				description: "- płatność okresowa, niezmienna przez cały czas pożyczki lub lokaty"
			},
			{
				name: "wa",
				description: "- wartość bieżąca lokaty, teraźniejsza wartość łączna serii przyszłych płatności"
			},
			{
				name: "wp",
				description: "- wartość przyszła lub saldo gotówkowe, jakie chcesz uzyskać po dokonaniu ostatniej płatności. Jeśli pominięta, używana jest wartość wp = 0"
			},
			{
				name: "typ",
				description: "- wartość logiczna: płatność na początku okresu = 1; płatność na końcu okresu = 0 lub pominięta"
			},
			{
				name: "wynik",
				description: "- przypuszczalna stopa procentowa; jeśli pominięta, wynik = 0,1 (10 procent)"
			}
		]
	},
	{
		name: "REGBŁSTD",
		description: "Zwraca błąd standardowy przewidywanej wartości y dla każdej wartości x w regresji.",
		arguments: [
			{
				name: "znane_y",
				description: "- tablica lub zakres zależnych punktów danych, może zawierać liczby lub nazwy, tablice albo odwołania zawierające liczby"
			},
			{
				name: "znane_x",
				description: "- tablica lub zakres niezależnych punktów danych, może zawierać liczby lub nazwy, tablice albo odwołania zawierające liczby"
			}
		]
	},
	{
		name: "REGEXPP",
		description: "Zwraca statystykę, która opisuje krzywą wykładniczą dopasowaną do znanych punktów danych.",
		arguments: [
			{
				name: "znane_y",
				description: "- zbiór już znanych wartości y w relacji y = b*m^x"
			},
			{
				name: "znane_x",
				description: "- opcjonalny zbiór już znanych wartości x w relacji y = b*m^x"
			},
			{
				name: "stała",
				description: "- wartość logiczna: stała b jest obliczana normalnie, jeśli stała = PRAWDA lub jest pominięta; stała b jest ustawiana jako równa 1, jeśli stała = FAŁSZ"
			},
			{
				name: "statystyka",
				description: "- wartość logiczna: zwraca dodatkowe statystki regresji = PRAWDA; zwraca współczynniki m i stałą b = 0 lub pominięta"
			}
		]
	},
	{
		name: "REGEXPW",
		description: "Zwraca liczby wykładniczego trendu wzrostu, dopasowane do znanych punktów danych.",
		arguments: [
			{
				name: "znane_y",
				description: "- zbiór już znanych wartości y w relacji y = b*m^x, tablica lub zakres liczb dodatnich"
			},
			{
				name: "znane_x",
				description: "- opcjonalny zbiór wartości x, już znanych w relacji y = b*m^x, tablica lub zakres o tym samym rozmiarze, co znane_y"
			},
			{
				name: "nowe_x",
				description: "- nowe wartości x, dla których mają zostać zwrócone przez funkcję REGEXPW odpowiadające im wartości y"
			},
			{
				name: "stała",
				description: "- wartość logiczna: stała b jest obliczana normalnie, jeśli stała = PRAWDA; stała b jest ustawiana jako równa 1, jeśli stała = FAŁSZ lub jest pominięta"
			}
		]
	},
	{
		name: "REGLINP",
		description: "Zwraca statystykę opisującą trend liniowy, dopasowany do znanych punktów danych, dopasowując linię prostą przy użyciu metody najmniejszych kwadratów.",
		arguments: [
			{
				name: "znane_y",
				description: "- zbiór już znanych wartości y w relacji y = mx + b"
			},
			{
				name: "znane_x",
				description: "- opcjonalny zbiór już znanych wartości x w relacji y = mx + b"
			},
			{
				name: "stała",
				description: "- wartość logiczna: stała b jest obliczana normalnie, jeśli stała = PRAWDA lub jest pominięta; stała b jest ustawiana jako równa 0, jeśli stała = FAŁSZ"
			},
			{
				name: "statystyka",
				description: "- wartość logiczna: zwraca dodatkowe statystki regresji = PRAWDA; zwraca współczynniki m i stałą b = FAŁSZ lub pominięta"
			}
		]
	},
	{
		name: "REGLINW",
		description: "Zwraca liczby trendu liniowego dopasowane do znanych punktów danych przy użyciu metody najmniejszych kwadratów.",
		arguments: [
			{
				name: "znane_y",
				description: "- zakres lub tablica już znanych wartości y w relacji y = mx + b"
			},
			{
				name: "znane_x",
				description: "- opcjonalny zakres lub tablica już znanych wartości x w relacji y = mx + b, tablica o tym samym rozmiarze, co znane_y"
			},
			{
				name: "nowe_x",
				description: "- zakres lub tablica nowych wartości x, dla których mają być zwrócone odpowiadające im wartości y"
			},
			{
				name: "stała",
				description: "- wartość logiczna: stała b jest obliczana normalnie, jeśli stała = PRAWDA lub jest pominięta; stała b jest ustawiana jako równa 0, jeśli stała = FAŁSZ"
			}
		]
	},
	{
		name: "REGLINX",
		description: "Oblicza, lub przewiduje, wartość przyszłą przy założeniu trendu liniowego i przy użyciu istniejących wartości.",
		arguments: [
			{
				name: "x",
				description: "- punkt danych, dla którego ma być otrzymana wartość prognozy, musi być wartością liczbową"
			},
			{
				name: "znane_y",
				description: "- dane zależne w postaci tablicy lub zakresu danych liczbowych"
			},
			{
				name: "znane_x",
				description: "- dane niezależne w postaci tablicy lub zakresu danych liczbowych. Wariancja zbioru Znane_x musi być niezerowa"
			}
		]
	},
	{
		name: "RENT.BS",
		description: "Zwraca rentowność bonu skarbowego.",
		arguments: [
			{
				name: "rozliczenie",
				description: "– data rozliczenia papieru wartościowego, podana jako liczba seryjna"
			},
			{
				name: "data_spłaty",
				description: "– data spłaty papieru wartościowego, podana jako liczba seryjna"
			},
			{
				name: "kwota",
				description: "– cena za 100 jednostek wartości nominalnej bonu skarbowego"
			}
		]
	},
	{
		name: "RENT.DYSK",
		description: "Zwraca roczną rentowność zdyskontowanego papieru wartościowego, np. bonu skarbowego.",
		arguments: [
			{
				name: "rozliczenie",
				description: "– data rozliczenia papieru wartościowego, podana jako liczba seryjna"
			},
			{
				name: "data_spłaty",
				description: "– data spłaty papieru wartościowego, podana jako liczba seryjna"
			},
			{
				name: "kwota",
				description: "– cena papieru wartościowego za 100 jednostek wartości nominalnej"
			},
			{
				name: "wykup",
				description: "– wartość wykupu papieru wartościowego przypadająca na 100 jednostek wartości nominalnej"
			},
			{
				name: "podstawa",
				description: "– rodzaj podstawy wyliczania dni"
			}
		]
	},
	{
		name: "RENT.EKW.BS",
		description: "Zwraca rentowność ekwiwalentu obligacji dla bonu skarbowego.",
		arguments: [
			{
				name: "rozliczenie",
				description: "– data rozliczenia papieru wartościowego, podana jako liczba seryjna"
			},
			{
				name: "data_spłaty",
				description: "– data spłaty papieru wartościowego, podana jako liczba seryjna"
			},
			{
				name: "dyskonto",
				description: "– stopa dyskontowa bonu skarbowego"
			}
		]
	},
	{
		name: "ROK",
		description: "Zwraca rok z daty, liczbę całkowitą z zakresu 1900-9999.",
		arguments: [
			{
				name: "kolejna_liczba",
				description: "- liczba w kodzie data-godzina używanym w programie Spreadsheet"
			}
		]
	},
	{
		name: "RÓWNOW.STOPA.PROC",
		description: "Zwraca równoważną stopę procentową dla wzrostu inwestycji.",
		arguments: [
			{
				name: "liczba_rat",
				description: "— liczba okresów dla inwestycji."
			},
			{
				name: "wb",
				description: "— bieżąca wartość inwestycji."
			},
			{
				name: "wp",
				description: "— przyszła wartość inwestycji."
			}
		]
	},
	{
		name: "ROZKŁ.BETA",
		description: "Zwraca funkcję rozkładu prawdopodobieństwa beta.",
		arguments: [
			{
				name: "x",
				description: "- wartość między A i B, dla której ma zostać obliczona wartość funkcji"
			},
			{
				name: "alfa",
				description: "- parametr rozkładu (musi być większy niż 0)"
			},
			{
				name: "beta",
				description: "- parametr rozkładu (musi być większy niż 0)"
			},
			{
				name: "skumulowany",
				description: "- wartość logiczna: dla funkcji rozkładu skumulowanego należy użyć wartości PRAWDA; dla funkcji gęstości prawdopodobieństwa należy użyć wartości FAŁSZ"
			},
			{
				name: "A",
				description: "- opcjonalne dolne ograniczenie przedziału wartości x. Jeśli pominięte, to A = 0"
			},
			{
				name: "B",
				description: "- opcjonalne górne ograniczenie przedziału wartości x. Jeśli pominięte, to B = 1"
			}
		]
	},
	{
		name: "ROZKŁ.BETA.ODWR",
		description: "Zwraca odwrotność funkcji gęstości prawdopodobieństwa skumulowanego rozkładu beta (ROZKŁ.BETA).",
		arguments: [
			{
				name: "prawdopodobieństwo",
				description: "- prawdopodobieństwo skojarzone z rozkładem beta"
			},
			{
				name: "alfa",
				description: "- parametr rozkładu (musi być większy niż 0)"
			},
			{
				name: "beta",
				description: "- parametr rozkładu (musi być większy niż 0)"
			},
			{
				name: "A",
				description: "- opcjonalne dolne ograniczenie przedziału wartości x. Jeśli pominięte, to A = 0"
			},
			{
				name: "B",
				description: "- opcjonalne górne ograniczenie przedziału wartości x. Jeśli pominięte, to B = 1"
			}
		]
	},
	{
		name: "ROZKŁ.CHI",
		description: "Zwraca lewostronne prawdopodobieństwo rozkładu chi-kwadrat.",
		arguments: [
			{
				name: "x",
				description: "- wartość, dla której ma zostać oszacowany rozkład (liczba nieujemna)"
			},
			{
				name: "stopnie_swobody",
				description: "- liczba stopni swobody z zakresu od 1 do 10^10 (z wyłączeniem wartości 10^10)"
			},
			{
				name: "skumulowany",
				description: "- wartość logiczna określająca zwracaną funkcję: funkcja rozkładu skumulowanego = PRAWDA; funkcja gęstości prawdopodobieństwa = FAŁSZ"
			}
		]
	},
	{
		name: "ROZKŁ.CHI.ODWR",
		description: "Zwraca odwrotność lewostronnego prawdopodobieństwa rozkładu chi-kwadrat.",
		arguments: [
			{
				name: "prawdopodobieństwo",
				description: "- prawdopodobieństwo skojarzone z rozkładem chi-kwadrat (wartość z zakresu od 0 do 1 włącznie)"
			},
			{
				name: "stopnie_swobody",
				description: "- liczba stopni swobody (liczba z zakresu od 1 do 10^10, z wyłączeniem wartości 10^10)"
			}
		]
	},
	{
		name: "ROZKŁ.CHI.ODWR.PS",
		description: "Zwraca odwrotność prawostronnego prawdopodobieństwa rozkładu chi-kwadrat.",
		arguments: [
			{
				name: "prawdopodobieństwo",
				description: "- prawdopodobieństwo skojarzone z rozkładem chi-kwadrat (wartość z zakresu od 0 do 1 włącznie)"
			},
			{
				name: "stopnie_swobody",
				description: "- liczba stopni swobody (liczba z zakresu od 1 do 10^10, z wyłączeniem wartości 10^10)"
			}
		]
	},
	{
		name: "ROZKŁ.CHI.PS",
		description: "Zwraca prawostronne prawdopodobieństwo rozkładu chi-kwadrat.",
		arguments: [
			{
				name: "x",
				description: "- wartość, dla której ma zostać oszacowany rozkład (liczba nieujemna)"
			},
			{
				name: "stopnie_swobody",
				description: "- liczba stopni swobody z zakresu od 1 do 10^10 (z wyłączeniem wartości 10^10)"
			}
		]
	},
	{
		name: "ROZKŁ.DWUM",
		description: "Zwraca pojedynczy składnik dwumianowego rozkładu prawdopodobieństwa.",
		arguments: [
			{
				name: "liczba_s",
				description: "- liczba sukcesów w próbach"
			},
			{
				name: "próby",
				description: "- liczba niezależnych prób"
			},
			{
				name: "prawdopodob_s",
				description: "- prawdopodobieństwo sukcesu w pojedynczej próbie"
			},
			{
				name: "skumulowany",
				description: "- wartość logiczna: dla funkcji rozkładu skumulowanego, użyj wartości PRAWDA; dla funkcji gęstości prawdopodobieństwa użyj wartości FAŁSZ"
			}
		]
	},
	{
		name: "ROZKŁ.DWUM.ODWR",
		description: "Zwraca najmniejszą wartość, dla której skumulowany rozkład dwumianowy jest większy lub równy podanej wartości progowej.",
		arguments: [
			{
				name: "próby",
				description: "- liczba prób Bernoulliego"
			},
			{
				name: "prawdopodob_s",
				description: "- prawdopodobieństwo sukcesu w pojedynczej próbie, liczba większa od 0 i mniejsza od 1"
			},
			{
				name: "alfa",
				description: "- wartość progowa, liczba większa od 0 i mniejsza lub równa 1"
			}
		]
	},
	{
		name: "ROZKŁ.DWUM.PRZEC",
		description: "Zwraca ujemny rozkład dwumianowy (prawdopodobieństwo, że wystąpi Liczba_p porażek przed sukcesem o numerze Liczba_s, z prawdopodobieństwem sukcesu równym Prawdopodobieństwo_s).",
		arguments: [
			{
				name: "liczba_p",
				description: "- liczba porażek"
			},
			{
				name: "liczba_s",
				description: "- wartość progowa liczby sukcesów"
			},
			{
				name: "prawdopodobieństwo_s",
				description: "- prawdopodobieństwo sukcesu (liczba z zakresu od 0 do 1)"
			},
			{
				name: "skumulowany",
				description: "- wartość logiczna: w przypadku funkcji rozkładu skumulowanego należy użyć wartości PRAWDA; w przypadku funkcji masy prawdopodobieństwa należy użyć wartości FAŁSZ"
			}
		]
	},
	{
		name: "ROZKŁ.DWUM.ZAKRES",
		description: "Zwraca prawdopodobieństwo wyniku próby przy użyciu rozkładu dwumianowego.",
		arguments: [
			{
				name: "próby",
				description: "— liczba niezależnych prób."
			},
			{
				name: "prawdopodob_s",
				description: "— prawdopodobieństwo sukcesu w pojedynczej próbie."
			},
			{
				name: "liczba_s",
				description: "— liczba sukcesów w próbach."
			},
			{
				name: "liczba_s2",
				description: "W przypadku podania ta funkcja zwraca prawdopodobieństwo tego, że liczba pomyślnych prób będzie zawierać się między liczbami liczba_s i liczba_s2."
			}
		]
	},
	{
		name: "ROZKŁ.EXP",
		description: "Zwraca rozkład wykładniczy.",
		arguments: [
			{
				name: "x",
				description: "- wartość funkcji, liczba nieujemna"
			},
			{
				name: "lambda",
				description: "- wartość parametru, liczba dodatnia"
			},
			{
				name: "skumulowany",
				description: "- wartość logiczna, którą funkcja ma zwrócić: funkcja rozkładu skumulowanego = PRAWDA; funkcja gęstości prawdopodobieństwa = FAŁSZ"
			}
		]
	},
	{
		name: "ROZKŁ.F",
		description: "Zwraca lewostronny rozkład F prawdopodobieństwa (stopień zróżnicowania) dla dwóch zbiorów danych.",
		arguments: [
			{
				name: "x",
				description: "- wartość, dla której ma zostać oszacowana funkcja (liczba nieujemna)"
			},
			{
				name: "stopnie_swobody1",
				description: "- stopnie swobody licznika (liczba z zakresu od 1 do 10^10, z wyłączeniem wartości 10^10)"
			},
			{
				name: "stopnie_swobody2",
				description: "- stopnie swobody mianownika (liczba z zakresu od 1 do 10^10, z wyłączeniem wartości 10^10)"
			},
			{
				name: "skumulowany",
				description: "- wartość logiczna określająca zwracaną funkcję: funkcja rozkładu skumulowanego = PRAWDA; funkcja gęstości prawdopodobieństwa = FAŁSZ"
			}
		]
	},
	{
		name: "ROZKŁ.F.ODWR",
		description: "Zwraca odwrotność lewostronnego rozkładu F prawdopodobieństwa: jeśli p = ROZKŁ.F(x,...), wówczas ROZKŁ.F.ODW(p,...) = x.",
		arguments: [
			{
				name: "prawdopodobieństwo",
				description: "- prawdopodobieństwo skojarzone ze skumulowanym rozkładem F (liczba z zakresu od 0 do 1 włącznie)"
			},
			{
				name: "stopnie_swobody1",
				description: "- stopnie swobody licznika (liczba z zakresu od 1 do 10^10, z wyłączeniem wartości 10^10)"
			},
			{
				name: "stopnie_swobody2",
				description: "- stopnie swobody mianownika (liczba z zakresu od 1 do 10^10, z wyłączeniem wartości 10^10)"
			}
		]
	},
	{
		name: "ROZKŁ.F.ODWR.PS",
		description: "Zwraca odwrotność prawostronnego rozkładu F prawdopodobieństwa: jeśli p = ROZKŁ.F.PS(x,...), wówczas ROZKŁ.F.ODWR.PS(p,...) = x.",
		arguments: [
			{
				name: "prawdopodobieństwo",
				description: "- prawdopodobieństwo skojarzone ze skumulowanym rozkładem F (liczba z zakresu od 0 do 1 włącznie)"
			},
			{
				name: "stopnie_swobody1",
				description: "- stopnie swobody licznika (liczba z zakresu od 1 do 10^10, z wyłączeniem wartości 10^10)"
			},
			{
				name: "stopnie_swobody2",
				description: "- stopnie swobody mianownika (liczba z zakresu od 1 do 10^10, z wyłączeniem wartości 10^10)"
			}
		]
	},
	{
		name: "ROZKŁ.F.PS",
		description: "Zwraca prawostronny rozkład F prawdopodobieństwa (stopień zróżnicowania) dla dwóch zbiorów danych.",
		arguments: [
			{
				name: "x",
				description: "- wartość, dla której ma zostać oszacowana funkcja (liczba nieujemna)"
			},
			{
				name: "stopnie_swobody1",
				description: "- stopnie swobody licznika (liczba z zakresu od 1 do 10^10, z wyłączeniem wartości 10^10)"
			},
			{
				name: "stopnie_swobody2",
				description: "- stopnie swobody mianownika (liczba z zakresu od 1 do 10^10, z wyłączeniem wartości 10^10)"
			}
		]
	},
	{
		name: "ROZKŁ.GAMMA",
		description: "Zwraca rozkład gamma.",
		arguments: [
			{
				name: "x",
				description: "- wartość, dla której ma zostać oszacowany rozkład (liczba nieujemna)"
			},
			{
				name: "alfa",
				description: "- parametr rozkładu (liczba dodatnia)"
			},
			{
				name: "beta",
				description: "- parametr rozkładu (liczba dodatnia). Jeśli parametr beta = 1, funkcja ROZKŁ.GAMMA zwraca standardowy rozkład gamma"
			},
			{
				name: "skumulowany",
				description: "- wartość logiczna: zwrócenie funkcji rozkładu skumulowanego = PRAWDA; zwrócenie funkcji gęstości prawdopodobieństwa = FAŁSZ lub pominięta"
			}
		]
	},
	{
		name: "ROZKŁ.GAMMA.ODWR",
		description: "Zwraca odwrotność skumulowanego rozkładu gamma: jeśli p = ROZKŁ.GAMMA(x,...), to ROZKŁ.GAMMA.ODW(p,...) = x.",
		arguments: [
			{
				name: "prawdopodobieństwo",
				description: "- prawdopodobieństwo skojarzone z rozkładem gamma (liczba z zakresu od 0 do 1 włącznie)"
			},
			{
				name: "alfa",
				description: "- parametr rozkładu (liczba dodatnia)"
			},
			{
				name: "beta",
				description: "- parametr rozkładu (liczba dodatnia). Jeśli parametr beta = 1, funkcja ROZKŁ.GAMMA.ODW zwraca odwrotność standardowego rozkładu gamma"
			}
		]
	},
	{
		name: "ROZKŁ.HIPERGEOM",
		description: "Zwraca rozkład hipergeometryczny.",
		arguments: [
			{
				name: "próbka_s",
				description: "- liczba sukcesów w próbce"
			},
			{
				name: "wielk_próbki",
				description: "- wielkość próbki"
			},
			{
				name: "populacja_s",
				description: "- liczba sukcesów w populacji"
			},
			{
				name: "wielk_populacji",
				description: "- rozmiar populacji"
			},
			{
				name: "skumulowany",
				description: "- wartość logiczna: dla funkcji rozkładu skumulowanego należy użyć wartości PRAWDA; dla funkcji gęstości prawdopodobieństwa należy użyć wartości FAŁSZ"
			}
		]
	},
	{
		name: "ROZKŁ.LOG",
		description: "Zwraca rozkład logarytmiczno-normalny dla wartości x, gdzie ln(x) ma rozkład normalny o parametrach Średnia i Odchylenie_std.",
		arguments: [
			{
				name: "x",
				description: "- wartość, dla której ma zostać oszacowana funkcja (liczba dodatnia)"
			},
			{
				name: "średnia",
				description: "- wartość średnia dla ln(x)"
			},
			{
				name: "odchylenie_std",
				description: "- odchylenie standardowe dla ln(x) (liczba dodatnia)"
			},
			{
				name: "skumulowany",
				description: "- wartość logiczna: dla funkcji rozkładu skumulowanego należy użyć wartości PRAWDA; dla funkcji gęstości prawdopodobieństwa należy użyć wartości FAŁSZ"
			}
		]
	},
	{
		name: "ROZKŁ.LOG.ODWR",
		description: "Zwraca odwrotność skumulowanego rozkładu logarytmiczno-normalnego x, gdzie ln(x) ma rozkład normalny o parametrach Średnia i Odch_stand.",
		arguments: [
			{
				name: "prawdopodobieństwo",
				description: "- prawdopodobieństwo skojarzone z danym rozkładem logarytmiczno-normalnym, liczba między 0 i 1 włącznie"
			},
			{
				name: "średnia",
				description: "- wartość średnia dla ln(x)"
			},
			{
				name: "odchylenie_std",
				description: "- odchylenie standardowe dla ln(x), liczba dodatnia"
			}
		]
	},
	{
		name: "ROZKŁ.NORMALNY",
		description: "Zwraca rozkład normalny dla podanej średniej i odchylenia standardowego.",
		arguments: [
			{
				name: "x",
				description: "- wartość, dla której ma zostać obliczony rozkład"
			},
			{
				name: "średnia",
				description: "- średnia arytmetyczna rozkładu"
			},
			{
				name: "odchylenie_std",
				description: "- odchylenie standardowe rozkładu (liczba dodatnia)"
			},
			{
				name: "skumulowany",
				description: "- wartość logiczna: dla funkcji rozkładu skumulowanego należy użyć wartości PRAWDA; dla funkcji gęstości prawdopodobieństwa należy użyć wartości FAŁSZ"
			}
		]
	},
	{
		name: "ROZKŁ.NORMALNY.ODWR",
		description: "Zwraca odwrotność skumulowanego rozkładu normalnego dla podanej średniej i odchylenia standardowego.",
		arguments: [
			{
				name: "prawdopodobieństwo",
				description: "- prawdopodobieństwo dotyczące danego rozkładu normalnego, liczba z przedziału 0 i 1, włącznie"
			},
			{
				name: "średnia",
				description: "- średnia arytmetyczna danego rozkładu"
			},
			{
				name: "odchylenie_std",
				description: "- odchylenie standardowe danego rozkładu, liczba dodatnia"
			}
		]
	},
	{
		name: "ROZKŁ.NORMALNY.S",
		description: "Zwraca standardowy rozkład normalny (o średniej zero i odchyleniu standardowym jeden).",
		arguments: [
			{
				name: "z",
				description: "- wartość, dla której ma zostać wyznaczony rozkład"
			},
			{
				name: "skumulowany",
				description: "- wartość logiczna określająca zwracaną funkcję: funkcja rozkładu skumulowanego = PRAWDA; funkcja gęstości prawdopodobieństwa = FAŁSZ"
			}
		]
	},
	{
		name: "ROZKŁ.NORMALNY.S.ODWR",
		description: "Zwraca odwrotność standardowego skumulowanego rozkładu normalnego (o średniej zero i odchyleniu standardowym jeden).",
		arguments: [
			{
				name: "prawdopodobieństwo",
				description: "- prawdopodobieństwo dotyczące danego rozkładu normalnego, liczba z przedziału 0 i 1, włącznie"
			}
		]
	},
	{
		name: "ROZKŁ.POISSON",
		description: "Zwraca rozkład Poissona.",
		arguments: [
			{
				name: "x",
				description: "- liczba zdarzeń"
			},
			{
				name: "średnia",
				description: "- oczekiwana wartość liczbowa, liczba dodatnia"
			},
			{
				name: "skumulowany",
				description: "- wartość logiczna: dla skumulowanego rozkładu Poissona, użyj wartości PRAWDA; dla funkcji rozkładu Poissona użyj wartości FAŁSZ"
			}
		]
	},
	{
		name: "ROZKŁ.T",
		description: "Zwraca lewostronny rozkład t-Studenta.",
		arguments: [
			{
				name: "x",
				description: "- wartość liczbowa, dla której ma zostać oszacowany rozkład"
			},
			{
				name: "stopnie_swobody",
				description: "- liczba całkowita wskazująca liczbę stopni swobody charakteryzujących rozkład"
			},
			{
				name: "skumulowany",
				description: "- wartość logiczna: dla funkcji rozkładu skumulowanego należy użyć wartości PRAWDA; dla funkcji gęstości prawdopodobieństwa należy użyć wartości FAŁSZ"
			}
		]
	},
	{
		name: "ROZKŁ.T.DS",
		description: "Zwraca dwustronny rozkład t-Studenta.",
		arguments: [
			{
				name: "x",
				description: "- wartość liczbowa, dla której ma zostać oszacowany rozkład"
			},
			{
				name: "stopnie_swobody",
				description: "- liczba całkowita wskazująca liczbę stopni swobody charakteryzujących rozkład"
			}
		]
	},
	{
		name: "ROZKŁ.T.ODWR",
		description: "Zwraca lewostronną odwrotność rozkładu t-Studenta.",
		arguments: [
			{
				name: "prawdopodobieństwo",
				description: "- prawdopodobieństwo skojarzone z dwustronnym rozkładem t-Studenta (liczba z zakresu od 0 do 1 włącznie)"
			},
			{
				name: "stopnie_swobody",
				description: "- dodatnia liczba całkowita wskazująca liczbę stopni swobody charakteryzujących rozkład"
			}
		]
	},
	{
		name: "ROZKŁ.T.ODWR.DS",
		description: "Zwraca dwustronną odwrotność rozkładu t-Studenta.",
		arguments: [
			{
				name: "prawdopodobieństwo",
				description: "- prawdopodobieństwo skojarzone z dwustronnym rozkładem t-Studenta (liczba z zakresu od 0 do 1 włącznie)"
			},
			{
				name: "stopnie_swobody",
				description: "- dodatnia liczba całkowita wskazująca liczbę stopni swobody charakteryzujących rozkład"
			}
		]
	},
	{
		name: "ROZKŁ.T.PS",
		description: "Zwraca prawostronny rozkład t-Studenta.",
		arguments: [
			{
				name: "x",
				description: "- wartość liczbowa, dla której ma zostać oszacowany rozkład"
			},
			{
				name: "stopnie_swobody",
				description: "- liczba całkowita wskazująca liczbę stopni swobody charakteryzujących rozkład"
			}
		]
	},
	{
		name: "ROZKŁ.WEIBULL",
		description: "Zwraca rozkład Weibulla.",
		arguments: [
			{
				name: "x",
				description: "- wartość, dla której ma być oszacowana funkcja, liczba nieujemna"
			},
			{
				name: "alfa",
				description: "- parametr rozkładu, liczba dodatnia"
			},
			{
				name: "beta",
				description: "- parametr rozkładu, liczba dodatnia"
			},
			{
				name: "skumulowany",
				description: "- wartość logiczna: w przypadku funkcji rozkładu skumulowanego użyj wartości PRAWDA; w przypadku funkcji gęstości prawdopodobieństwa użyj wartości FAŁSZ"
			}
		]
	},
	{
		name: "ROZKŁAD.BETA",
		description: "Zwraca funkcję gęstości skumulowanego rozkładu beta.",
		arguments: [
			{
				name: "x",
				description: "- punkt pomiędzy A i B, dla którego ma zostać wyznaczona wartość funkcji"
			},
			{
				name: "alfa",
				description: "- parametr rozkładu, musi być większy od 0"
			},
			{
				name: "beta",
				description: "- parametr rozkładu, musi być większy od 0"
			},
			{
				name: "A",
				description: "- opcjonalne dolne ograniczenie przedziału wartości x. Jeśli pominięto, A = 0"
			},
			{
				name: "B",
				description: "- opcjonalne górne ograniczenie przedziału wartości x. Jeśli pominięto, B = 1"
			}
		]
	},
	{
		name: "ROZKŁAD.BETA.ODW",
		description: "Zwraca odwrotność funkcji gęstości skumulowanego rozkładu beta (ROZKŁAD.BETA).",
		arguments: [
			{
				name: "prawdopodobieństwo",
				description: "- prawdopodobieństwo skojarzone z danym rozkładem beta"
			},
			{
				name: "alfa",
				description: "- parametr rozkładu, musi być większy od 0"
			},
			{
				name: "beta",
				description: "- parametr rozkładu, musi być większy od 0"
			},
			{
				name: "A",
				description: "- opcjonalne dolne ograniczenie przedziału wartości x. Jeśli pominięto, A = 0"
			},
			{
				name: "B",
				description: "- opcjonalne górne ograniczenie przedziału wartości x. Jeśli pominięto, B = 1"
			}
		]
	},
	{
		name: "ROZKŁAD.CHI",
		description: "Zwraca prawostronne prawdopodobieństwo rozkładu chi-kwadrat.",
		arguments: [
			{
				name: "x",
				description: "- wartość, dla której ma zostać oszacowany rozkład (liczba nieujemna)"
			},
			{
				name: "stopnie_swobody",
				description: "- liczba stopni swobody z przedziału od 1 do 10^10 (z wyłączeniem wartości 10^10)"
			}
		]
	},
	{
		name: "ROZKŁAD.CHI.ODW",
		description: "Zwraca odwrotność prawostronnego prawdopodobieństwa rozkładu chi-kwadrat.",
		arguments: [
			{
				name: "prawdopodobieństwo",
				description: "- prawdopodobieństwo skojarzone z rozkładem chi-kwadrat (wartość z przedziału od 0 do 1 włącznie)"
			},
			{
				name: "stopnie_swobody",
				description: "- liczba stopni swobody (liczba z przedziału od 1 do 10^10 z wyłączeniem wartości 10^10)"
			}
		]
	},
	{
		name: "ROZKŁAD.DWUM",
		description: "Zwraca pojedynczy składnik dwumianowego rozkładu prawdopodobieństwa.",
		arguments: [
			{
				name: "liczba_s",
				description: "- liczba sukcesów w próbach"
			},
			{
				name: "próby",
				description: "- liczba niezależnych prób"
			},
			{
				name: "prawdopodob_s",
				description: "- prawdopodobieństwo sukcesu w pojedynczej próbie"
			},
			{
				name: "skumulowany",
				description: "- wartość logiczna: dla funkcji rozkładu skumulowanego użyj wartości PRAWDA; dla funkcji masy prawdopodobieństwa użyj wartości FAŁSZ"
			}
		]
	},
	{
		name: "ROZKŁAD.DWUM.PRZEC",
		description: "Zwraca rozkład dwumianowy ujemny, prawdopodobieństwo, że wystąpi Liczba_p porażek przed sukcesem nr Liczba_s z prawdopodobieństwem sukcesu Prawdopodob_s.",
		arguments: [
			{
				name: "liczba_p",
				description: "- liczba porażek"
			},
			{
				name: "liczba_s",
				description: "- wartość progowa liczby sukcesów"
			},
			{
				name: "prawdopodob_s",
				description: "- prawdopodobieństwo sukcesu; liczba między 0 a 1"
			}
		]
	},
	{
		name: "ROZKŁAD.EXP",
		description: "Zwraca rozkład wykładniczy.",
		arguments: [
			{
				name: "x",
				description: "- wartość funkcji, liczba nieujemna"
			},
			{
				name: "lambda",
				description: "- wartość parametru, liczba dodatnia"
			},
			{
				name: "skumulowany",
				description: "- wartość logiczna, którą funkcja ma zwrócić: funkcja rozkładu skumulowanego = PRAWDA; funkcja gęstości prawdopodobieństwa = FAŁSZ"
			}
		]
	},
	{
		name: "ROZKŁAD.F",
		description: "Zwraca (prawostronny) rozkład F prawdopodobieństwa (stopień zróżnicowania) dla dwóch zbiorów danych.",
		arguments: [
			{
				name: "x",
				description: "- wartość, dla której ma zostać wyznaczona funkcja, liczba nieujemna"
			},
			{
				name: "stopnie_swobody1",
				description: "- wartość stopni swobody w liczniku, liczba z przedziału od 1 do 10^10 z wyłączeniem 10^10"
			},
			{
				name: "stopnie_swobody2",
				description: "- wartość stopni swobody w mianowniku, liczba z przedziału od 1 do 10^10 z wyłączeniem 10^10"
			}
		]
	},
	{
		name: "ROZKŁAD.F.ODW",
		description: "Zwraca odwrotność (prawostronnego) rozkładu F prawdopodobieństwa: jeśli p = ROZKŁAD.F(x,...), wówczas ROZKŁAD.F.ODW(p,...) = x.",
		arguments: [
			{
				name: "prawdopodobieństwo",
				description: "- prawdopodobieństwo skojarzone ze skumulowanym rozkładem F, liczba z przedziału od 0 do 1 włącznie"
			},
			{
				name: "stopnie_swobody1",
				description: "- stopnie swobody licznika, liczba między 1 a 10^10 z wyłączeniem 10^10"
			},
			{
				name: "stopnie_swobody2",
				description: "- wartość stopni swobody mianownika, liczba między 1 a 10^10 z wyłączeniem 10^10"
			}
		]
	},
	{
		name: "ROZKŁAD.FISHER",
		description: "Zwraca transformatę Fishera.",
		arguments: [
			{
				name: "x",
				description: "- wartość, dla której ma zostać obliczona transformata, liczba między -1 a 1 z wyłączeniem -1 i 1"
			}
		]
	},
	{
		name: "ROZKŁAD.FISHER.ODW",
		description: "Zwraca odwrotną transformatę Fishera: jeśli y = ROZKŁAD.FISHER(x), wówczas ROZKŁAD.FISHER.ODW(y) = x.",
		arguments: [
			{
				name: "y",
				description: "- wartość, dla której ma zostać wykonana odwrotna transformata"
			}
		]
	},
	{
		name: "ROZKŁAD.GAMMA",
		description: "Zwraca rozkład gamma.",
		arguments: [
			{
				name: "x",
				description: "- wartość, dla której ma zostać oszacowany rozkład, liczba nieujemna"
			},
			{
				name: "alfa",
				description: "- parametr rozkładu, liczba dodatnia"
			},
			{
				name: "beta",
				description: "- parametr rozkładu, liczba dodatnia. Jeśli beta = 1, funkcja ROZKŁAD.GAMMA zwraca standardowy rozkład gamma"
			},
			{
				name: "skumulowany",
				description: "- wartość logiczna: zwraca funkcję rozkładu skumulowanego = PRAWDA; zwraca funkcję masy prawdopodobieństwa = FAŁSZ lub pominięta"
			}
		]
	},
	{
		name: "ROZKŁAD.GAMMA.ODW",
		description: "Zwraca odwrotność skumulowanego rozkładu gamma: jeśli p = ROZKŁAD.GAMMA(x,...), wówczas ROZKŁAD.GAMMA.ODW(p,...) = x.",
		arguments: [
			{
				name: "prawdopodobieństwo",
				description: "- prawdopodobieństwo skojarzone z danym rozkładem gamma, liczba z przedziału od 0 do 1 włącznie"
			},
			{
				name: "alfa",
				description: "- parametr rozkładu, liczba dodatnia"
			},
			{
				name: "beta",
				description: "- parametr rozkładu, liczba dodatnia. Jeśli beta = 1, funkcja ROZKŁAD.GAMMA.ODW zwraca odwrotność standardowego rozkładu gamma"
			}
		]
	},
	{
		name: "ROZKŁAD.HIPERGEOM",
		description: "Wyznacza rozkład hipergeometryczny.",
		arguments: [
			{
				name: "próbka_s",
				description: "- liczba sukcesów w próbce"
			},
			{
				name: "wielk_próbki",
				description: "- wielkość próbki"
			},
			{
				name: "populacja_s",
				description: "- liczba sukcesów w populacji"
			},
			{
				name: "wielk_populacji",
				description: "- rozmiar populacji"
			}
		]
	},
	{
		name: "ROZKŁAD.LIN.GAMMA",
		description: "Zwraca logarytm naturalny funkcji gamma.",
		arguments: [
			{
				name: "x",
				description: "- wartość, dla której będzie obliczana funkcja ROZKŁAD.LIN.GAMMA, liczba dodatnia"
			}
		]
	},
	{
		name: "ROZKŁAD.LIN.GAMMA.DOKŁ",
		description: "Zwraca logarytm naturalny funkcji gamma.",
		arguments: [
			{
				name: "x",
				description: "- wartość, dla której będzie obliczana funkcja ROZKŁAD.LIN.GAMMA.DOKŁ, liczba dodatnia"
			}
		]
	},
	{
		name: "ROZKŁAD.LOG",
		description: "Zwraca skumulowany rozkład logarytmiczno-normalny x, gdzie ln(x) ma rozkład normalny o parametrach Średnia i Odchylenie_std.",
		arguments: [
			{
				name: "x",
				description: "- punkt, w którym ma zostać wyznaczona wartość funkcji, liczba dodatnia"
			},
			{
				name: "średnia",
				description: "- wartość średnia dla ln(x)"
			},
			{
				name: "odchylenie_std",
				description: "- odchylenie standardowe dla ln(x), liczba dodatnia"
			}
		]
	},
	{
		name: "ROZKŁAD.LOG.ODW",
		description: "Zwraca odwrotność skumulowanego rozkładu logarytmiczno-normalnego x, gdzie ln(x) ma rozkład normalny o parametrach Średnia i Odchylenie_std.",
		arguments: [
			{
				name: "prawdopodobieństwo",
				description: "- prawdopodobieństwo skojarzone z danym rozkładem logarytmiczno-normalnym, liczba z przedziału od 0 do 1 włącznie"
			},
			{
				name: "średnia",
				description: "- wartość średnia dla ln(x)"
			},
			{
				name: "odchylenie_std",
				description: "- odchylenie standardowe dla ln(x), liczba dodatnia"
			}
		]
	},
	{
		name: "ROZKŁAD.NORMALNY",
		description: "Zwraca skumulowany rozkład normalny dla podanej średniej i odchylenia standardowego.",
		arguments: [
			{
				name: "x",
				description: "- wartość, dla której ma zostać obliczony rozkład"
			},
			{
				name: "średnia",
				description: "- średnia arytmetyczna danego rozkładu"
			},
			{
				name: "odchylenie_std",
				description: "- odchylenie standardowe danego rozkładu, liczba dodatnia"
			},
			{
				name: "skumulowany",
				description: "- wartość logiczna: dla funkcji rozkładu skumulowanego użyj wartości PRAWDA; dla funkcji gęstości prawdopodobieństwa użyj wartości FAŁSZ"
			}
		]
	},
	{
		name: "ROZKŁAD.NORMALNY.ODW",
		description: "Zwraca odwrotność skumulowanego rozkładu normalnego dla podanej średniej i odchylenia standardowego.",
		arguments: [
			{
				name: "prawdopodobieństwo",
				description: "- prawdopodobieństwo dotyczące danego rozkładu normalnego, liczba z przedziału od 0 do 1 włącznie"
			},
			{
				name: "średnia",
				description: "- średnia arytmetyczna danego rozkładu"
			},
			{
				name: "odchylenie_std",
				description: "- odchylenie standardowe danego rozkładu, liczba dodatnia"
			}
		]
	},
	{
		name: "ROZKŁAD.NORMALNY.S",
		description: "Zwraca standardowy skumulowany rozkład normalny (o średniej zero i odchyleniu standardowym jeden).",
		arguments: [
			{
				name: "z",
				description: "- wartość, dla której ma zostać obliczony rozkład"
			}
		]
	},
	{
		name: "ROZKŁAD.NORMALNY.S.ODW",
		description: "Zwraca odwrotność standardowego skumulowanego rozkładu normalnego (o średniej zero i odchyleniu standardowym jeden).",
		arguments: [
			{
				name: "prawdopodobieństwo",
				description: "- prawdopodobieństwo dotyczące danego rozkładu normalnego, liczba z przedziału od 0 do 1 włącznie"
			}
		]
	},
	{
		name: "ROZKŁAD.POISSON",
		description: "Zwraca rozkład Poissona.",
		arguments: [
			{
				name: "x",
				description: "- liczba zdarzeń"
			},
			{
				name: "średnia",
				description: "- oczekiwana wartość liczbowa, liczba dodatnia"
			},
			{
				name: "skumulowany",
				description: "- wartość logiczna: dla skumulowanego prawdopodobieństwa Poissona użyj wartości PRAWDA; dla funkcji masy prawdopodobieństwa Poissona użyj wartości FAŁSZ"
			}
		]
	},
	{
		name: "ROZKŁAD.T",
		description: "Zwraca rozkład t-Studenta.",
		arguments: [
			{
				name: "x",
				description: "- wartość liczbowa, dla której ma zostać wyznaczony rozkład"
			},
			{
				name: "stopnie_swobody",
				description: "- liczba całkowita reprezentująca liczbę stopni swobody charakteryzujących rozkład"
			},
			{
				name: "strony",
				description: "- określa liczbę stron rozkładu: rozkład jednostronny = 1; rozkład dwustronny = 2"
			}
		]
	},
	{
		name: "ROZKŁAD.T.ODW",
		description: "Zwraca dwustronną odwrotność rozkładu t-Studenta.",
		arguments: [
			{
				name: "prawdopodobieństwo",
				description: "- prawdopodobieństwo skojarzone z dwustronnym rozkładem t-Studenta, liczba z przedziału od 0 do 1 włącznie"
			},
			{
				name: "stopnie_swobody",
				description: "- dodatnia liczba całkowita wskazująca liczbę stopni swobody charakteryzujących rozkład"
			}
		]
	},
	{
		name: "ROZKŁAD.WEIBULL",
		description: "Zwraca rozkład Weibulla.",
		arguments: [
			{
				name: "x",
				description: "- wartość, dla której ma być oszacowana funkcja, liczba nieujemna"
			},
			{
				name: "alfa",
				description: "- parametr rozkładu, liczba dodatnia"
			},
			{
				name: "beta",
				description: "- parametr rozkładu, liczba dodatnia"
			},
			{
				name: "skumulowany",
				description: "- wartość logiczna: w przypadku funkcji rozkładu skumulowanego użyj wartości PRAWDA; w przypadku funkcji masy prawdopodobieństwa użyj wartości FAŁSZ"
			}
		]
	},
	{
		name: "RÓŻN.LICZB.ZESP",
		description: "Zwraca różnicę dwóch liczb zespolonych.",
		arguments: [
			{
				name: "liczba_zesp1",
				description: "– liczba zespolona, od której zostanie odjęta liczba_zesp2"
			},
			{
				name: "liczba_zesp2",
				description: "– liczba zespolona, która zostanie odjęta od liczby liczba_zesp1"
			}
		]
	},
	{
		name: "RZYMSKIE",
		description: "Konwertuje liczbę arabską na rzymską jako tekst.",
		arguments: [
			{
				name: "liczba",
				description: "- liczba arabska, która ma być konwertowana"
			},
			{
				name: "forma",
				description: "- liczba określająca żądany typ zapisu rzymskiego."
			}
		]
	},
	{
		name: "SEC",
		description: "Zwraca secans kąta.",
		arguments: [
			{
				name: "liczba",
				description: "- kąt podany w radianach, dla którego ma zostać obliczony secans."
			}
		]
	},
	{
		name: "SEC.LICZBY.ZESP",
		description: "Zwraca secans liczby zespolonej.",
		arguments: [
			{
				name: "liczba_zespolona",
				description: "- liczba zespolona, dla której ma zostać obliczony secans."
			}
		]
	},
	{
		name: "SECH",
		description: "Zwraca secans hiperboliczny kąta.",
		arguments: [
			{
				name: "liczba",
				description: "- kąt podany w radianach, dla którego ma zostać obliczony secans hiperboliczny."
			}
		]
	},
	{
		name: "SECH.LICZBY.ZESP",
		description: "Zwraca secans hiperboliczny liczby zespolonej.",
		arguments: [
			{
				name: "liczba_zespolona",
				description: "- liczba zespolona, dla której ma zostać obliczony secans hiperboliczny."
			}
		]
	},
	{
		name: "SEKUNDA",
		description: "Zwraca sekundę, liczbę od 0 do 59.",
		arguments: [
			{
				name: "kolejna_liczba",
				description: "- liczba w kodzie data-godzina używanym w programie Spreadsheet lub tekst w formacie godziny, na przykład 16:48:23 albo 4:48:47 PM"
			}
		]
	},
	{
		name: "SILNIA",
		description: "Oblicza silnię podanej liczby równą 1*2*3...* Liczba.",
		arguments: [
			{
				name: "liczba",
				description: "- liczba nieujemna, której silnia ma zostać obliczona"
			}
		]
	},
	{
		name: "SILNIA.DWUKR",
		description: "Zwraca dwukrotną silnię liczby.",
		arguments: [
			{
				name: "liczba",
				description: "– wartość, dla której ma zostać obliczona dwukrotna silnia"
			}
		]
	},
	{
		name: "SIN",
		description: "Zwraca sinus kąta.",
		arguments: [
			{
				name: "liczba",
				description: "- kąt podany w radianach, którego sinus ma zostać obliczony. Stopnie * PI()/180 = radiany"
			}
		]
	},
	{
		name: "SIN.LICZBY.ZESP",
		description: "Zwraca wartość sinusa liczby zespolonej.",
		arguments: [
			{
				name: "liczba_zesp",
				description: "– liczba zespolona poddana działaniu"
			}
		]
	},
	{
		name: "SINH",
		description: "Oblicza sinus hiperboliczny liczby.",
		arguments: [
			{
				name: "liczba",
				description: "- dowolna liczba rzeczywista"
			}
		]
	},
	{
		name: "SINH.LICZBY.ZESP",
		description: "Zwraca sinus hiperboliczny liczby zespolonej.",
		arguments: [
			{
				name: "liczba_zespolona",
				description: "– liczba zespolona, dla której ma zostać obliczony sinus hiperboliczny."
			}
		]
	},
	{
		name: "SKOŚNOŚĆ",
		description: "Zwraca skośność rozkładu prawdopodobieństwa: charakteryzującą stopień asymetrii rozkładu wokół średniej.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "liczba1",
				description: "- od 1 do 255 liczb lub nazw, tablic albo odwołań, dla których ma być obliczona skośność"
			},
			{
				name: "liczba2",
				description: "- od 1 do 255 liczb lub nazw, tablic albo odwołań, dla których ma być obliczona skośność"
			}
		]
	},
	{
		name: "SKOŚNOŚĆ.P",
		description: "Zwraca skośność rozkładu prawdopodobieństwa na podstawie populacji: charakteryzującą stopień asymetrii rozkładu wokół średniej.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "liczba1",
				description: "- od 1 do 254 liczb lub nazw, tablic albo odwołań zawierających liczby, dla których ma zostać obliczona skośność populacji."
			},
			{
				name: "liczba2",
				description: "- od 1 do 254 liczb lub nazw, tablic albo odwołań zawierających liczby, dla których ma zostać obliczona skośność populacji."
			}
		]
	},
	{
		name: "SLN",
		description: "Zwraca amortyzację środka trwałego za pojedynczy okres metodą liniową.",
		arguments: [
			{
				name: "koszt",
				description: "- wartość początkowa (koszt) środka trwałego"
			},
			{
				name: "odzysk",
				description: "- wartość środka trwałego po całkowitym czasie amortyzacji"
			},
			{
				name: "czas_życia",
				description: "- liczba okresów stanowiących całkowity czas amortyzacji środka trwałego (nazywana również czasem życia środka trwałego)"
			}
		]
	},
	{
		name: "SPŁAC.KAPIT",
		description: "Zwraca wartość łącznego kapitału spłaconej pożyczki pomiędzy dwoma okresami.",
		arguments: [
			{
				name: "stopa",
				description: "– stopa procentowa"
			},
			{
				name: "okresy",
				description: "– całkowita liczba okresów płatności"
			},
			{
				name: "wa",
				description: "– wartość bieżąca"
			},
			{
				name: "okres_pocz",
				description: "– pierwszy okres w wyliczeniu"
			},
			{
				name: "okres_końc",
				description: "– ostatni okres w wyliczeniu"
			},
			{
				name: "rodzaj",
				description: "– określenie typu płatności"
			}
		]
	},
	{
		name: "SPŁAC.ODS",
		description: "Zwraca wartość procentu składanego płatnego pomiędzy dwoma okresami.",
		arguments: [
			{
				name: "stopa",
				description: "– stopa procentowa"
			},
			{
				name: "okresy",
				description: "– całkowita liczba okresów płatności"
			},
			{
				name: "wa",
				description: "– wartość bieżąca"
			},
			{
				name: "okres_pocz",
				description: "– pierwszy okres w wyliczeniu"
			},
			{
				name: "okres_końc",
				description: "– ostatni okres w wyliczeniu"
			},
			{
				name: "rodzaj",
				description: "– określenie typu płatności"
			}
		]
	},
	{
		name: "SPRAWDŹ.PRÓG",
		description: "Sprawdza, czy liczba jest większa niż podana wartość progowa.",
		arguments: [
			{
				name: "liczba",
				description: "– sprawdzana wartość"
			},
			{
				name: "próg",
				description: "– wartość progowa"
			}
		]
	},
	{
		name: "SPRZĘŻ.LICZBY.ZESP",
		description: "Zwraca wartość sprzężoną liczby zespolonej.",
		arguments: [
			{
				name: "liczba_zesp",
				description: "– liczba zespolona poddana działaniu"
			}
		]
	},
	{
		name: "ŚREDNIA",
		description: "Zwraca wartość średnią (średnią arytmetyczną) podanych argumentów, które mogą być liczbami lub nazwami, tablicami albo odwołaniami zawierającymi liczby.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "liczba1",
				description: "- od 1 do 255 argumentów, dla których zostanie wyznaczona wartość średnia"
			},
			{
				name: "liczba2",
				description: "- od 1 do 255 argumentów, dla których zostanie wyznaczona wartość średnia"
			}
		]
	},
	{
		name: "ŚREDNIA.A",
		description: "Zwraca wartość średniej arytmetycznej argumentów. Tekst i wartości logiczne FAŁSZ są przyjmowane jako 0; wartości logiczne PRAWDA są przyjmowane jako 1. Argumenty mogą być liczbami, nazwami, tablicami lub odwołaniami.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "wartość1",
				description: "od 1 do 255 argumentów, dla których ma być obliczona średnia"
			},
			{
				name: "wartość2",
				description: "od 1 do 255 argumentów, dla których ma być obliczona średnia"
			}
		]
	},
	{
		name: "ŚREDNIA.GEOMETRYCZNA",
		description: "Zwraca wartość średniej geometrycznej dla tablicy lub zakresu dodatnich danych liczbowych.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "liczba1",
				description: "- od 1 do 255 liczb lub nazw, tablic albo odwołań, które zawierają uśredniane liczby"
			},
			{
				name: "liczba2",
				description: "- od 1 do 255 liczb lub nazw, tablic albo odwołań, które zawierają uśredniane liczby"
			}
		]
	},
	{
		name: "ŚREDNIA.HARMONICZNA",
		description: "Zwraca wartość średnią harmoniczną zbioru danych liczb dodatnich: odwrotność średniej arytmetycznej odwrotności.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "liczba1",
				description: "- od 1 do 255 liczb lub nazw, tablic albo odwołań, które zawierają uśredniane liczby"
			},
			{
				name: "liczba2",
				description: "- od 1 do 255 liczb lub nazw, tablic albo odwołań, które zawierają uśredniane liczby"
			}
		]
	},
	{
		name: "ŚREDNIA.JEŻELI",
		description: "Oblicza średnią arytmetyczną dla komórek spełniających podany warunek lub kryteria.",
		arguments: [
			{
				name: "zakres",
				description: "- zakres komórek, dla których należy wykonać obliczenia"
			},
			{
				name: "kryteria",
				description: "- warunek lub kryteria określające komórki używane do obliczenia średniej, podane w postaci liczby, wyrażenia lub tekstu"
			},
			{
				name: "średnia_zakres",
				description: "- komórki faktycznie używane do obliczenia średniej. Jeśli pominięte, używane są komórki w zakresie "
			}
		]
	},
	{
		name: "ŚREDNIA.WARUNKÓW",
		description: "Znajduje średnią arytmetyczną dla komórek spełniających podany zestaw warunków lub kryteriów.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "średnia_zakres",
				description: "- komórki faktycznie używane do obliczenia średniej."
			},
			{
				name: "kryteria_zakres",
				description: "- zakres komórek, dla których należy sprawdzić określony warunek"
			},
			{
				name: "kryteria",
				description: "- warunek lub kryteria określające komórki używane do obliczenia średniej podane w postaci liczby, wyrażenia lub tekstu"
			}
		]
	},
	{
		name: "ŚREDNIA.WEWN",
		description: "Zwraca wartość średnią z wewnętrznej części zbioru wartości danych.",
		arguments: [
			{
				name: "tablica",
				description: "- tablica lub zakres wartości do zawężenia i obliczenia średniej"
			},
			{
				name: "procent",
				description: "- liczba ułamkowa, określająca, jaka część punktów danych od góry i od dołu nie zostanie wykluczona podczas obliczeń"
			}
		]
	},
	{
		name: "STOPA.DYSK",
		description: "Zwraca wartość stopy dyskontowej papieru wartościowego.",
		arguments: [
			{
				name: "rozliczenie",
				description: "– data rozliczenia papieru wartościowego, podana jako liczba seryjna"
			},
			{
				name: "data_spłaty",
				description: "– data spłaty papieru wartościowego, podana jako liczba seryjna"
			},
			{
				name: "kwota",
				description: "– cena papieru wartościowego za 100 jednostek wartości nominalnej"
			},
			{
				name: "wykup",
				description: "– wartość wykupu papieru wartościowego przypadająca na 100 jednostek wartości nominalnej"
			},
			{
				name: "podstawa",
				description: "– rodzaj podstawy wyliczania dni"
			}
		]
	},
	{
		name: "STOPA.PROC",
		description: "Zwraca wartość stopy procentowej papieru wartościowego całkowicie ulokowanego.",
		arguments: [
			{
				name: "rozliczenie",
				description: "– data rozliczenia papieru wartościowego, podana jako liczba seryjna"
			},
			{
				name: "data_spłaty",
				description: "– data spłaty papieru wartościowego, podana jako liczba seryjna"
			},
			{
				name: "lokata",
				description: "– kwota zainwestowana w papier wartościowy"
			},
			{
				name: "wykup",
				description: "– kwota, którą otrzymamy w dniu płatności"
			},
			{
				name: "podstawa",
				description: "– rodzaj podstawy wyliczania dni"
			}
		]
	},
	{
		name: "STOPNIE",
		description: "Konwertuje radiany na stopnie.",
		arguments: [
			{
				name: "kąt",
				description: "- kąt podany w radianach, który ma być poddany konwersji"
			}
		]
	},
	{
		name: "SUMA",
		description: "Dodaje wszystkie liczby w zakresie komórek.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "liczba1",
				description: "- od 1 do 255 argumentów, które zostaną zsumowane. Wartości logiczne i tekst w komórkach są ignorowane, a uwzględniane, jeśli są wpisane jako argumenty"
			},
			{
				name: "liczba2",
				description: "- od 1 do 255 argumentów, które zostaną zsumowane. Wartości logiczne i tekst w komórkach są ignorowane, a uwzględniane, jeśli są wpisane jako argumenty"
			}
		]
	},
	{
		name: "SUMA.ILOCZYNÓW",
		description: "Zwraca sumę iloczynów odpowiadających sobie zakresów lub tablic.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tablica1",
				description: "- od 2 do 255 tablic, których składniki mają zostać pomnożone, a następnie dodane. Wszystkie tablice muszą mieć te same wymiary"
			},
			{
				name: "tablica2",
				description: "- od 2 do 255 tablic, których składniki mają zostać pomnożone, a następnie dodane. Wszystkie tablice muszą mieć te same wymiary"
			},
			{
				name: "tablica3",
				description: "- od 2 do 255 tablic, których składniki mają zostać pomnożone, a następnie dodane. Wszystkie tablice muszą mieć te same wymiary"
			}
		]
	},
	{
		name: "SUMA.JEŻELI",
		description: "Dodaje komórki spełniające podane warunki lub kryteria.",
		arguments: [
			{
				name: "zakres",
				description: "- zakres komórek, które mają zostać obliczone"
			},
			{
				name: "kryteria",
				description: "- warunek lub kryteria określające, które komórki zostaną dodane, podane w postaci liczby, wyrażenia lub tekstu"
			},
			{
				name: "suma_zakres",
				description: "- faktycznie sumowane komórki. Jeśli pominięte, używane są komórki w zakresie"
			}
		]
	},
	{
		name: "SUMA.KWADRATÓW",
		description: "Zwraca sumę kwadratów argumentów. Argumenty mogą być liczbami, tablicami, nazwami lub odwołaniami do komórek zawierających liczby.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "liczba1",
				description: "- od 1 do 255 liczb lub nazw, tablic albo odwołań, dla których ma być obliczona suma kwadratów "
			},
			{
				name: "liczba2",
				description: "- od 1 do 255 liczb lub nazw, tablic albo odwołań, dla których ma być obliczona suma kwadratów "
			}
		]
	},
	{
		name: "SUMA.LICZB.ZESP",
		description: "Zwraca sumę liczb zespolonych.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "liczba_zespolona1",
				description: "- sekwencja zawierająca od 1 do 255 liczb zespolonych, dla których należy obliczyć sumę"
			},
			{
				name: "liczba_zespolona2",
				description: "- sekwencja zawierająca od 1 do 255 liczb zespolonych, dla których należy obliczyć sumę"
			}
		]
	},
	{
		name: "SUMA.SZER.POT",
		description: "Oblicza sumę szeregu potęgowego wg odpowiedniego wzoru.",
		arguments: [
			{
				name: "x",
				description: "– wartość początkowa dla szeregu potęgowego"
			},
			{
				name: "n",
				description: "– potęga początkowa, do której ma być podniesiona wartość x"
			},
			{
				name: "m",
				description: "– krok, o który wzrasta n dla każdego składnika w szeregu"
			},
			{
				name: "współczynniki",
				description: "– zbiór współczynników, przez które mnoży się każdą kolejną potęgę x"
			}
		]
	},
	{
		name: "SUMA.WARUNKÓW",
		description: "Oblicza sumę komórek spełniających dany zestaw warunków lub kryteriów.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "suma_zakres",
				description: "- faktycznie sumowane komórki."
			},
			{
				name: "kryteria_zakres",
				description: "- zakres komórek, dla których należy sprawdzić określony warunek"
			},
			{
				name: "kryteria",
				description: "- warunek lub kryteria określające sumowane komórki, podane w postaci liczby, wyrażenia lub tekstu"
			}
		]
	},
	{
		name: "SUMA.X2.M.Y2",
		description: "Sumuje różnice między kwadratami dwóch odpowiadających sobie zakresów lub tablic.",
		arguments: [
			{
				name: "tablica_x",
				description: "- pierwsza tablica lub zakres wartości, może to być liczba lub nazwa, tablica lub odwołanie zawierające liczby"
			},
			{
				name: "tablica_y",
				description: "- drugi zakres lub tablica liczb, może to być liczba lub nazwa, tablica lub odwołanie zawierające liczby"
			}
		]
	},
	{
		name: "SUMA.X2.P.Y2",
		description: "Zwraca sumę końcową sum kwadratów liczb w dwóch odpowiadających sobie zakresach lub tablicach.",
		arguments: [
			{
				name: "tablica_x",
				description: "- pierwsza tablica lub zakres liczb, może to być liczba lub nazwa, tablica lub odwołanie zawierające liczby"
			},
			{
				name: "tablica_y",
				description: "- drugi zakres lub tablica liczb, może to być liczba lub nazwa, tablica lub odwołanie zawierające liczby"
			}
		]
	},
	{
		name: "SUMA.XMY.2",
		description: "Sumuje kwadraty różnic w dwóch odpowiadających sobie zakresach lub tablicach.",
		arguments: [
			{
				name: "tablica_x",
				description: "- pierwsza tablica lub zakres wartości, może to być liczba lub nazwa, tablica lub odwołanie zawierające liczby"
			},
			{
				name: "tablica_y",
				description: "- drugi zakres lub tablica wartości, może to być liczba lub nazwa, tablica lub odwołanie zawierające liczby"
			}
		]
	},
	{
		name: "SUMY.CZĘŚCIOWE",
		description: "Oblicza sumę częściową listy lub bazy danych.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "funkcja_nr",
				description: "- liczba z zakresu od 1 do 11 określająca, która funkcja zostanie użyta do obliczenia sum częściowych."
			},
			{
				name: "adres1",
				description: "- od 1 do 254 zakresów lub odwołań, dla których ma być obliczona suma częściowa"
			}
		]
	},
	{
		name: "SYD",
		description: "Oblicza amortyzację środka trwałego za podany okres metodą sumy cyfr wszystkich lat amortyzacji.",
		arguments: [
			{
				name: "koszt",
				description: "- wartość początkowa (koszt) środka trwałego"
			},
			{
				name: "odzysk",
				description: "- wartość środka trwałego po całkowitym czasie amortyzacji"
			},
			{
				name: "czas_życia",
				description: "- liczba okresów stanowiących całkowity czas amortyzacji środka trwałego (nazywana również czasem życia środka trwałego)"
			},
			{
				name: "okres",
				description: "- okres, dla którego obliczana jest amortyzacja. Okres musi być wyrażony w tych samych jednostkach co czas_życia"
			}
		]
	},
	{
		name: "SZESN.NA.DWÓJK",
		description: "Zamienia liczbę szesnastkową na liczbę w kodzie dwójkowym.",
		arguments: [
			{
				name: "liczba",
				description: "– liczba szesnastkowa poddana przekształceniu"
			},
			{
				name: "miejsca",
				description: "– liczba znaków dla wartości wynikowej"
			}
		]
	},
	{
		name: "SZESN.NA.DZIES",
		description: "Przekształca liczbę szesnastkową na dziesiętną.",
		arguments: [
			{
				name: "liczba",
				description: "– liczba szesnastkowa poddana przekształceniu"
			}
		]
	},
	{
		name: "SZESN.NA.ÓSM",
		description: "Zamienia liczbę szesnastkową na liczbę w kodzie ósemkowym.",
		arguments: [
			{
				name: "liczba",
				description: "– liczba szesnastkowa poddana przekształceniu"
			},
			{
				name: "miejsca",
				description: "– liczba znaków dla wartości wynikowej"
			}
		]
	},
	{
		name: "SZUKAJ.TEKST",
		description: "Zwraca numer znaku, w którym jeden ciąg znaków został znaleziony po raz pierwszy w drugim, począwszy od lewej strony (nie rozróżniając liter małych i dużych).",
		arguments: [
			{
				name: "szukany_tekst",
				description: "jest tekstem, który chcesz znaleźć. Możesz użyć symboli wieloznacznych ? i *; użyj kombinacji ~? i ~*, aby znaleźć znaki ? i *"
			},
			{
				name: "obejmujący_tekst",
				description: "- tekst, w którym ma nastąpić szukanie ciągu szukany_tekst"
			},
			{
				name: "liczba_początkowa",
				description: "- numer znaku liczony od lewej strony w obejmujący_tekst, ustalający punkt, od którego rozpocznie się poszukiwanie. Jeśli pominięto używany jest numer 1"
			}
		]
	},
	{
		name: "T",
		description: "Sprawdza, czy wartość to tekst i zwraca ten tekst, gdy wartość jest tekstem, albo podwójny cudzysłów (pusty tekst), jeśli wartość nie jest tekstem.",
		arguments: [
			{
				name: "wartość",
				description: "- wartość do testowania"
			}
		]
	},
	{
		name: "T.TEST",
		description: "Zwraca prawdopodobieństwo związane z testem t-Studenta.",
		arguments: [
			{
				name: "tablica1",
				description: "- pierwszy zbiór danych"
			},
			{
				name: "tablica2",
				description: "- drugi zbiór danych"
			},
			{
				name: "strony",
				description: "- określa liczbę stron rozkładu: rozkład jednostronny = 1; rozkład dwustronny = 2"
			},
			{
				name: "typ",
				description: "- określa rodzaj testu t: sparowany = 1, z dwiema próbkami o równej wariancji = 2, z dwiema próbkami o nierównej wariancji = 3"
			}
		]
	},
	{
		name: "TAN",
		description: "Zwraca tangens kąta.",
		arguments: [
			{
				name: "liczba",
				description: "- kąt podany w radianach, którego tangens ma być obliczony. Stopnie*PI()/180 = radiany"
			}
		]
	},
	{
		name: "TAN.LICZBY.ZESP",
		description: "Zwraca tangens liczby zespolonej.",
		arguments: [
			{
				name: "liczba_zespolona",
				description: "- liczba zespolona, dla której ma zostać obliczony tangens."
			}
		]
	},
	{
		name: "TANH",
		description: "Zwraca tangens hiperboliczny liczby.",
		arguments: [
			{
				name: "liczba",
				description: "- dowolna liczba rzeczywista"
			}
		]
	},
	{
		name: "TEKST",
		description: "Konwertuje wartość na tekst w podanym formacie liczbowym.",
		arguments: [
			{
				name: "wartość",
				description: "- wartość liczbowa, formuła zwracająca wartość liczbową lub odwołanie do komórki zawierającej wartość liczbową"
			},
			{
				name: "format_tekst",
				description: "- format liczbowy w postaci tekstowej z pola Kategoria na karcie Liczby okna dialogowego Formatuj komórki (nie Ogólne)"
			}
		]
	},
	{
		name: "TERAZ",
		description: "Zwraca bieżącą datę i godzinę sformatowane jako data i godzina.",
		arguments: [
		]
	},
	{
		name: "TEST.CHI",
		description: "Zwraca test na niezależność: wartość z rozkładu chi-kwadrat dla statystyki i odpowiednich stopni swobody.",
		arguments: [
			{
				name: "zakres_bieżący",
				description: "- zakres danych zawierający wartości zaobserwowane, które mają zostać porównane z wartościami oczekiwanymi"
			},
			{
				name: "zakres_przewidywany",
				description: "- zakres danych zawierający stosunek iloczynu sum wierszy i sum kolumn do sumy końcowej"
			}
		]
	},
	{
		name: "TEST.F",
		description: "Zwraca wynik testu F, dwustronnego prawdopodobieństwa, że wariancje w Tablicy1 i Tablicy2 nie są istotnie różne.",
		arguments: [
			{
				name: "tablica1",
				description: "- pierwsza tablica lub zakres danych, może zawierać liczby lub nazwy, tablice albo odwołania zawierające liczby (puste są ignorowane)"
			},
			{
				name: "tablica2",
				description: "- druga tablica lub zakres danych, może zawierać liczby lub nazwy, tablice albo odwołania zawierające liczby (puste są ignorowane)"
			}
		]
	},
	{
		name: "TEST.T",
		description: "Zwraca prawdopodobieństwo związane z testem t-Studenta.",
		arguments: [
			{
				name: "tablica1",
				description: "- pierwszy zbiór danych"
			},
			{
				name: "tablica2",
				description: "- drugi zbiór danych"
			},
			{
				name: "strony",
				description: "- określa liczbę stron rozkładu: rozkład jednostronny = 1; rozkład dwustronny = 2"
			},
			{
				name: "typ",
				description: "- określa rodzaj testu t: sparowany = 1, z dwiema próbkami o równej wariancji = 2, z dwiema próbkami o nierównej wariancji = 3"
			}
		]
	},
	{
		name: "TEST.Z",
		description: "Zwraca wartość P o jednej stronie oraz test z.",
		arguments: [
			{
				name: "tablica",
				description: "- tablica lub zakres danych, na podstawie których będzie testowana wartość x"
			},
			{
				name: "x",
				description: "- wartość, którą chcesz poddać testowi"
			},
			{
				name: "sigma",
				description: "- wartość odchylenia standardowego w populacji (znanej). Jeśli pominięto, używane jest odchylenie standardowe próbki"
			}
		]
	},
	{
		name: "TRANSPONUJ",
		description: "Konwertuje pionowy zakres komórek do zakresu poziomego lub na odwrót.",
		arguments: [
			{
				name: "tablica",
				description: "- zakres komórek w arkuszu lub tablica wartości, która ma być transponowana"
			}
		]
	},
	{
		name: "TYP",
		description: "Zwraca liczbę całkowitą reprezentującą typ danych wartości: liczba = 1; tekst = 2; wartość logiczna = 4; wartość błędu = 16; tablica = 64.",
		arguments: [
			{
				name: "wartość",
				description: "może mieć dowolną wartość"
			}
		]
	},
	{
		name: "UFNOŚĆ",
		description: "Zwraca przedział ufności dla średniej populacji, używając rozkładu normalnego.",
		arguments: [
			{
				name: "alfa",
				description: "- poziom istotności używany do obliczenia poziomu ufności (liczba większa niż 0 i mniejsza niż 1)"
			},
			{
				name: "odchylenie_std",
				description: "- odchylenie standardowe populacji dla zakresu danych (zakłada się, że jest znane). Parametr odchylenie_std musi być większy niż 0"
			},
			{
				name: "rozmiar",
				description: "- wielkość próbki"
			}
		]
	},
	{
		name: "UFNOŚĆ.NORM",
		description: "Zwraca przedział ufności dla średniej populacji, używając rozkładu normalnego.",
		arguments: [
			{
				name: "alfa",
				description: "- poziom istotności używany do obliczenia poziomu ufności (liczba większa niż 0 i mniejsza niż 1)"
			},
			{
				name: "odchylenie_std",
				description: "- odchylenie standardowe populacji dla zakresu danych (zakłada się, że jest znane). Parametr odchylenie_std musi być większy niż 0"
			},
			{
				name: "rozmiar",
				description: "- wielkość próbki"
			}
		]
	},
	{
		name: "UFNOŚĆ.T",
		description: "Zwraca przedział ufności dla średniej populacji, używając rozkładu t-Studenta.",
		arguments: [
			{
				name: "alfa",
				description: "- poziom istotności używany do obliczenia poziomu ufności (liczba większa niż 0 i mniejsza niż 1)"
			},
			{
				name: "odchylenie_std",
				description: "- odchylenie standardowe populacji dla zakresu danych (zakłada się, że jest znane). Parametr odchylenie_std musi być większy niż 0"
			},
			{
				name: "rozmiar",
				description: "- wielkość próbki"
			}
		]
	},
	{
		name: "UNICODE",
		description: "Zwraca liczbę (punkt kodowy) odpowiadającą pierwszemu znakowi tekstu.",
		arguments: [
			{
				name: "tekst",
				description: "- znak, dla którego ma zostać określona wartość Unicode."
			}
		]
	},
	{
		name: "USUŃ.ZBĘDNE.ODSTĘPY",
		description: "Usuwa wszystkie spacje z podanego tekstu poza pojedynczymi spacjami rozdzielającymi słowa.",
		arguments: [
			{
				name: "tekst",
				description: "- tekst, z którego mają zostać usunięte odstępy"
			}
		]
	},
	{
		name: "VDB",
		description: "Zwraca amortyzację środka trwałego za podany okres lub jego część obliczoną metodą podwójnie malejącego salda lub inną podaną metodą.",
		arguments: [
			{
				name: "koszt",
				description: "- wartość początkowa (koszt) środka trwałego"
			},
			{
				name: "odzysk",
				description: "- wartość środka trwałego po całkowitym czasie amortyzacji"
			},
			{
				name: "czas_życia",
				description: "- liczba okresów stanowiących całkowity czas amortyzacji środka trwałego (nazywana również czasem życia środka trwałego)"
			},
			{
				name: "początek",
				description: "- okres początkowy, w którym ma być obliczona wartość amortyzacji, w tych samych jednostkach co czas_życia"
			},
			{
				name: "koniec",
				description: "- okres końcowy, w którym ma być obliczona wartość amortyzacji, w tych samych jednostkach co czas_życia"
			},
			{
				name: "współczynnik",
				description: "- wartość sterująca szybkością, z jaką ma maleć saldo, jeśli pominięta, 2 (metoda podwójnie malejącego salda)"
			},
			{
				name: "bez_przełączenia",
				description: "- wartość logiczna określająca, czy przejść do metody liniowej, jeśli amortyzacja okaże się większa, niż obliczona metodą malejącego salda = FAŁSZ lub pominięta; czy nie przechodzić = PRAWDA"
			}
		]
	},
	{
		name: "WARIANCJA",
		description: "Wyznacza wariancję na podstawie próbki (pomija wartości logiczne i tekstowe w próbce).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "liczba1",
				description: "- od 1 do 255 wartości liczbowych odpowiadających próbce populacji"
			},
			{
				name: "liczba2",
				description: "- od 1 do 255 wartości liczbowych odpowiadających próbce populacji"
			}
		]
	},
	{
		name: "WARIANCJA.A",
		description: "Szacuje wariancję na podstawie próbki uwzględniając wartości logiczne oraz tekst. Wartość logiczna FAŁSZ i wartości tekstowe są traktowane jako 0, a wartość logiczna PRAWDA jako 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "wartość1",
				description: "od 1 do 255 argumentów odpowiadających próbce populacji"
			},
			{
				name: "wartość2",
				description: "od 1 do 255 argumentów odpowiadających próbce populacji"
			}
		]
	},
	{
		name: "WARIANCJA.POP",
		description: "Oblicza wariancję na podstawie całej populacji (pomija wartości logiczne i tekstowe w próbce).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "liczba1",
				description: "- od 1 do 255 wartości liczbowych odpowiadających populacji"
			},
			{
				name: "liczba2",
				description: "- od 1 do 255 wartości liczbowych odpowiadających populacji"
			}
		]
	},
	{
		name: "WARIANCJA.POPUL",
		description: "Oblicza wariancję na podstawie całej populacji (pomija wartości logiczne i tekstowe w próbce).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "liczba1",
				description: "- od 1 do 255 wartości liczbowych odpowiadających populacji"
			},
			{
				name: "liczba2",
				description: "- od 1 do 255 wartości liczbowych odpowiadających populacji"
			}
		]
	},
	{
		name: "WARIANCJA.POPUL.A",
		description: "Oblicza wariancję w oparciu o całą populację, włącznie z wartościami logicznymi i tekstem. Teksty i wartości logiczne FAŁSZ są traktowane jako 0; logiczna wartość PRAWDA jest traktowana jako 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "wartość1",
				description: "- od 1 do 255 argumentów odpowiadających populacji"
			},
			{
				name: "wartość2",
				description: "- od 1 do 255 argumentów odpowiadających populacji"
			}
		]
	},
	{
		name: "WARIANCJA.PRÓBKI",
		description: "Wyznacza wariancję na podstawie próbki (pomija wartości logiczne i tekstowe w próbce).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "liczba1",
				description: "- od 1 do 255 wartości liczbowych odpowiadających próbce populacji"
			},
			{
				name: "liczba2",
				description: "- od 1 do 255 wartości liczbowych odpowiadających próbce populacji"
			}
		]
	},
	{
		name: "WART.PRZYSZŁ.KAP",
		description: "Zwraca wartość przyszłą kapitału początkowego wraz z szeregiem rat procentu składanego.",
		arguments: [
			{
				name: "kapitał",
				description: "– wartość obecna kapitału"
			},
			{
				name: "stopy",
				description: "– tablica stóp procentowych"
			}
		]
	},
	{
		name: "WARTOŚĆ",
		description: "Konwertuje ciąg tekstowy reprezentujący liczbę na liczbę.",
		arguments: [
			{
				name: "tekst",
				description: "- tekst zawarty w cudzysłowie, lub odwołanie do komórki zawierającej tekst, który ma zostać poddany konwersji"
			}
		]
	},
	{
		name: "WARTOŚĆ.LICZBOWA",
		description: "Konwertuje tekst na liczbę w sposób niezależny od ustawień regionalnych.",
		arguments: [
			{
				name: "tekst",
				description: "— ciąg reprezentujący liczbę, która ma zostać przekonwertowana."
			},
			{
				name: "separator_dziesiętny",
				description: "— znak używany jako separator dziesiętny w ciągu."
			},
			{
				name: "separator_grup",
				description: "— znak używany jako separator grup w ciągu."
			}
		]
	},
	{
		name: "WEŹDANETABELI",
		description: "Wyodrębnia dane przechowywane w tabeli przestawnej.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "pole_danych",
				description: "- nazwa pola danych, z którego mają być wyodrębnione dane"
			},
			{
				name: "tabela_przestawna",
				description: "- odwołanie do komórki lub zakresu komórek w tabeli przestawnej zawierające dane, które mają być pobrane"
			},
			{
				name: "pole",
				description: "pole, do którego następuje odwołanie"
			},
			{
				name: "element",
				description: "element pola, do którego następuje odwołanie"
			}
		]
	},
	{
		name: "WIELOMIAN",
		description: "Zwraca wielomian dla zestawu liczb.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "liczba1",
				description: "- sekwencja od 1 do 255 wartości, dla których należy utworzyć wielomian"
			},
			{
				name: "liczba2",
				description: "- sekwencja od 1 do 255 wartości, dla których należy utworzyć wielomian"
			}
		]
	},
	{
		name: "WIERSZ",
		description: "Zwraca numer wiersza odpowiadający podanemu odwołaniu.",
		arguments: [
			{
				name: "odwołanie",
				description: "- komórka lub pojedynczy zakres komórek, których numer wiersza zostanie wyznaczony; jeśli pominięto, zwraca komórkę zawierającą funkcję WIERSZ"
			}
		]
	},
	{
		name: "WSP.KORELACJI",
		description: "Oblicza współczynnik korelacji pomiędzy dwoma zbiorami danych.",
		arguments: [
			{
				name: "tablica1",
				description: "- zakres komórek wartości. Wartości powinny być liczbami, nazwami, tablicami lub odwołaniami zawierającymi liczby"
			},
			{
				name: "tablica2",
				description: "- drugi zakres komórek wartości. Wartości powinny być liczbami, nazwami, tablicami lub odwołaniami zawierającymi liczby"
			}
		]
	},
	{
		name: "WYBIERZ",
		description: "Wybiera z listy wartość lub czynność do wykonania na podstawie numeru wskaźnika.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "nr_arg",
				description: "- określa, który argument zostanie wybrany. Num_indeksu musi się zawierać w przedziale między 1 i 254 lub być formułą albo odwołaniem do liczby między 1 i 254"
			},
			{
				name: "wartość1",
				description: "- od 1 do 254 liczb, odwołań do komórek, zdefiniowanych nazw, formuł lub tekstów, z których funkcja WYBIERZ może wybierać"
			},
			{
				name: "wartość2",
				description: "- od 1 do 254 liczb, odwołań do komórek, zdefiniowanych nazw, formuł lub tekstów, z których funkcja WYBIERZ może wybierać"
			}
		]
	},
	{
		name: "WYPŁ.DATA.NAST",
		description: "Zwraca datę następnej dywidendy po dacie rozliczenia.",
		arguments: [
			{
				name: "rozliczenie",
				description: "– data rozliczenia papieru wartościowego, podana jako liczba seryjna"
			},
			{
				name: "data_spłaty",
				description: "– data spłaty papieru wartościowego, podana jako liczba seryjna"
			},
			{
				name: "częstość",
				description: "– liczba realizacji kuponów w roku"
			},
			{
				name: "podstawa",
				description: "– określenie sposobu płatności"
			}
		]
	},
	{
		name: "WYPŁ.DATA.POPRZ",
		description: "Zwraca datę dywidendy poprzedzającej datę zakupu.",
		arguments: [
			{
				name: "rozliczenie",
				description: "– data rozliczenia papieru wartościowego, podana jako liczba seryjna"
			},
			{
				name: "data_spłaty",
				description: "– data spłaty papieru wartościowego, podana jako liczba seryjna"
			},
			{
				name: "częstość",
				description: "– liczba realizacji kuponów w roku"
			},
			{
				name: "podstawa",
				description: "– określenie typu płatności"
			}
		]
	},
	{
		name: "WYPŁ.DNI.OD.POCZ",
		description: "Zwraca liczbę dni od początku okresu dywidendy do daty wykupu.",
		arguments: [
			{
				name: "rozliczenie",
				description: "– data rozliczenia papieru wartościowego, podana jako liczba seryjna"
			},
			{
				name: "data_spłaty",
				description: "– data spłaty papieru wartościowego, podana jako liczba seryjna"
			},
			{
				name: "częstość",
				description: "– liczba realizacji kuponów w roku"
			},
			{
				name: "podstawa",
				description: "– określenie typu płatności"
			}
		]
	},
	{
		name: "WYPŁ.LICZBA",
		description: " Zwraca liczbę wypłacanych dywidend między datą rozliczenia i datą spłaty.",
		arguments: [
			{
				name: "rozliczenie",
				description: "– data rozliczenia papieru wartościowego, podana jako liczba seryjna"
			},
			{
				name: "data_spłaty",
				description: "– data spłaty papieru wartościowego, podana jako liczba seryjna"
			},
			{
				name: "częstość",
				description: "– liczba realizacji kuponów w roku"
			},
			{
				name: "podstawa",
				description: "– określenie typu płatności"
			}
		]
	},
	{
		name: "WYST.NAJCZĘŚCIEJ",
		description: "Zwraca najczęściej występującą lub powtarzającą się wartość w tablicy albo zakresie danych.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "liczba1",
				description: "- od 1 do 255 liczb, nazw, tablic albo odwołań zawierających liczby, dla których ma być obliczona funkcja WYST.NAJCZĘŚCIEJ"
			},
			{
				name: "liczba2",
				description: "- od 1 do 255 liczb, nazw, tablic albo odwołań zawierających liczby, dla których ma być obliczona funkcja WYST.NAJCZĘŚCIEJ"
			}
		]
	},
	{
		name: "WYST.NAJCZĘŚCIEJ.TABL",
		description: "Zwraca pionową tablicę zawierającą najczęściej występujące lub powtarzające się wartości w tablicy lub zakresie danych. W przypadku tablicy poziomej należy użyć funkcji =TRANSPONUJ(WYST.NAJCZĘŚCIEJ.TABL(liczba1;liczba2;...)).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "liczba1",
				description: "- od 1 do 255 liczb lub nazw albo odwołań zawierających liczby, dla których ma być obliczona funkcja WYST.NAJCZĘŚCIEJ.TABL"
			},
			{
				name: "liczba2",
				description: "- od 1 do 255 liczb lub nazw albo odwołań zawierających liczby, dla których ma być obliczona funkcja WYST.NAJCZĘŚCIEJ.TABL"
			}
		]
	},
	{
		name: "WYST.NAJCZĘŚCIEJ.WART",
		description: "Zwraca najczęściej występującą lub powtarzającą się wartość w tablicy albo zakresie danych.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "liczba1",
				description: "- od 1 do 255 liczb lub nazw albo odwołań zawierających liczby, dla których ma być obliczona funkcja WYST.NAJCZĘŚCIEJ"
			},
			{
				name: "liczba2",
				description: "- od 1 do 255 liczb lub nazw albo odwołań zawierających liczby, dla których ma być obliczona funkcja WYST.NAJCZĘŚCIEJ"
			}
		]
	},
	{
		name: "WYSZUKAJ",
		description: "Wyszukuje wartość z zakresu jednowierszowego lub jednokolumnowego albo z tablicy. Zapewnia zgodność z poprzednimi wersjami.",
		arguments: [
			{
				name: "szukana_wartość",
				description: "- wartość wyszukiwana przez funkcję WYSZUKAJ w argumencie przeszukiwany_wektor, może to być liczba, tekst, wartość logiczna albo nazwa lub odwołanie do wartości"
			},
			{
				name: "przeszukiwany_wektor",
				description: "- zakres zawierający tylko jeden wiersz lub jedną kolumnę tekstu, liczb lub wartości logicznych, umieszczonych w kolejności rosnącej"
			},
			{
				name: "wektor_wynikowy",
				description: "- zakres zawierający tylko jeden wiersz lub kolumnę, o tym samym rozmiarze, co przeszukiwany_wektor"
			}
		]
	},
	{
		name: "WYSZUKAJ.PIONOWO",
		description: "Wyszukuje wartość w pierwszej od lewej kolumnie tabeli i zwraca wartość z tego samego wiersza w kolumnie określonej przez użytkownika. Domyślnie tabela musi być sortowana w kolejności rosnącej.",
		arguments: [
			{
				name: "szukana_wartość",
				description: "- wartość do znalezienia w pierwszej kolumnie tabeli, może być wartością, odwołaniem lub ciągiem tekstowym"
			},
			{
				name: "tabela_tablica",
				description: "- tabela tekstowa, liczbowa lub wartości logicznych, z której są pobierane dane. Argument tabela_tablica może być odwołaniem do zakresu lub nazwą zakresu"
			},
			{
				name: "nr_indeksu_kolumny",
				description: "- numer kolumny w argumencie tabela_tablica, z której ma zostać pobrana zwracana pasująca wartość. Pierwsza kolumna wartości w tabeli to kolumna 1"
			},
			{
				name: "przeszukiwany_zakres",
				description: "- wartość logiczna: aby znaleźć najlepsze dopasowanie w pierwszej kolumnie (sortowanej w kolejności rosnącej) = PRAWDA lub pominięta; aby znaleźć dokładny odpowiednik = FAŁSZ"
			}
		]
	},
	{
		name: "WYSZUKAJ.POZIOMO",
		description: "Wyszukuje wartość w górnym wierszu tabeli lub tablicy wartości i zwraca wartość z tej samej kolumny ze wskazanego wiersza.",
		arguments: [
			{
				name: "odniesienie",
				description: "- wartość do znalezienia w  pierwszym wierszu tabeli, może być wartością, odwołaniem lub ciągiem znaków"
			},
			{
				name: "tablica",
				description: "- tabela tekstowa albo tabela wartości liczbowych lub logicznych, w której będą wyszukiwane dane. Tabela_tablica może być odwołaniem do zakresu lub nazwą zakresu"
			},
			{
				name: "nr_wiersza",
				description: "- numer wiersza w tabeli_tablicy, z którego ma zostać zwrócona pasująca wartość. Pierwszym wierszem wartości w tabeli jest wiersz 1"
			},
			{
				name: "wiersz",
				description: "- wartość logiczna: aby znaleźć najlepsze dopasowanie w górnym wierszu (posortowanym w kolejności rosnącej) = PRAWDA lub pominięte; aby znaleźć dokładne dopasowanie = FAŁSZ"
			}
		]
	},
	{
		name: "WYZNACZNIK.MACIERZY",
		description: "Zwraca wyznacznik podanej tablicy.",
		arguments: [
			{
				name: "tablica",
				description: "- tablica liczbowa o równej liczbie wierszy i kolumn, albo zakres komórek lub stała tablicowa"
			}
		]
	},
	{
		name: "XIRR",
		description: "Zwraca wartość wewnętrznej stopy zwrotu dla serii przepływów gotówkowych niekoniecznie okresowych.",
		arguments: [
			{
				name: "wartości",
				description: "– seria przepływów gotówkowych odpowiadająca zestawieniu płatności wg dat"
			},
			{
				name: "daty",
				description: "– zestawienie dat płatności odpowiadających przepływom gotówkowym"
			},
			{
				name: "wynik",
				description: "– przewidywany wynik funkcji XIRR"
			}
		]
	},
	{
		name: "XNPV",
		description: "Zwraca wartość bieżącą netto serii przepływów gotówkowych niekoniecznie okresowych.",
		arguments: [
			{
				name: "stopa",
				description: "– stopa dyskontowa stosowana dla przepływów gotówkowych"
			},
			{
				name: "wartości",
				description: "– seria przepływów gotówkowych odpowiadająca zestawieniu płatności wg dat"
			},
			{
				name: "daty",
				description: "– zestawienie dat płatności odpowiadających przepływom gotówkowym"
			}
		]
	},
	{
		name: "XOR",
		description: "Zwraca wartość logiczną XOR (wyłączne LUB) wszystkich argumentów.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logiczna1",
				description: "- od 1 do 254 testowanych warunków, które mogą mieć wartość PRAWDA albo FAŁSZ, mogą być wartościami logicznymi, tablicami lub odwołaniami."
			},
			{
				name: "logiczna2",
				description: "- od 1 do 254 testowanych warunków, które mogą mieć wartość PRAWDA albo FAŁSZ, mogą być wartościami logicznymi, tablicami lub odwołaniami."
			}
		]
	},
	{
		name: "Z.TEST",
		description: "Zwraca wartość P o jednej stronie oraz test z.",
		arguments: [
			{
				name: "tablica",
				description: "- tablica lub zakres danych, w oparciu o które będzie testowana wartość x"
			},
			{
				name: "x",
				description: "- wartość, którą chcesz poddać testowi"
			},
			{
				name: "sigma",
				description: "- wartość odchylenia standardowego w populacji (znanej). Jeśli pominięto, używane jest odchylenie standardowe próbki"
			}
		]
	},
	{
		name: "Z.WIELKIEJ.LITERY",
		description: "Konwertuje ciąg tekstowy na litery właściwej wielkości; pierwszą literę w każdym słowie na wielką literę, a wszystkie inne litery na małe litery.",
		arguments: [
			{
				name: "tekst",
				description: "- tekst ujęty w znaki cudzysłowu, formuła zwracająca tekst albo odwołanie do komórki zawierającej tekst, którego litery mają być częściowo zamienione na wielkie litery"
			}
		]
	},
	{
		name: "Zakres",
		description: "",
		arguments: [
			{
				name: "",
				description: ""
			}
		]
	},
	{
		name: "ZAOKR",
		description: "Zaokrągla liczbę do określonej liczby cyfr.",
		arguments: [
			{
				name: "liczba",
				description: "- liczba, która ma być zaokrąglona"
			},
			{
				name: "liczba_cyfr",
				description: "- określa liczbę cyfr, do której ma zostać zaokrąglona dana liczba. Ujemne liczby oznaczają zaokrąglenia do miejsc po lewej stronie przecinka; zero najbliższą liczbę całkowitą"
			}
		]
	},
	{
		name: "ZAOKR.DO.CAŁK",
		description: "Zaokrągla liczbę w dół do najbliższej liczby całkowitej.",
		arguments: [
			{
				name: "liczba",
				description: "- liczba rzeczywista, która ma zostać zaokrąglona w dół do liczby całkowitej"
			}
		]
	},
	{
		name: "ZAOKR.DO.NPARZ",
		description: "Zaokrągla liczbę dodatnią w górę, a liczbę ujemną w dół do najbliższej liczby nieparzystej całkowitej.",
		arguments: [
			{
				name: "liczba",
				description: "- wartość, która ma być zaokrąglona"
			}
		]
	},
	{
		name: "ZAOKR.DO.PARZ",
		description: "Zaokrągla liczbę dodatnią w górę, a liczbę ujemną w dół do najbliższej parzystej liczby całkowitej.",
		arguments: [
			{
				name: "liczba",
				description: "- wartość, która ma być zaokrąglona"
			}
		]
	},
	{
		name: "ZAOKR.DO.TEKST",
		description: "Zaokrągla liczbę do określonej liczby miejsc po przecinku i zwraca wynik jako tekst ze spacjami lub bez.",
		arguments: [
			{
				name: "liczba",
				description: "- liczba, która ma być zaokrąglona i konwertowana na tekst"
			},
			{
				name: "miejsca_dziesiętne",
				description: "- liczba cyfr po prawej stronie przecinka dziesiętnego. Jeśli jest pominięta, równa się 2."
			},
			{
				name: "bez_przecinka",
				description: "- wartość logiczna określająca, czy w liczbie nie mają być wyświetlane separatory tysięcy = PRAWDA; lub czy mają być wyświetlane = FAŁSZ lub pominięte"
			}
		]
	},
	{
		name: "ZAOKR.DO.WIELOKR",
		description: "Zwraca wartość liczby zaokrąglonej do podanej wielokrotności.",
		arguments: [
			{
				name: "liczba",
				description: "– wartość, która będzie zaokrąglana"
			},
			{
				name: "wielokrotność",
				description: "– wielokrotność, do której należy zaokrąglić liczbę"
			}
		]
	},
	{
		name: "ZAOKR.DÓŁ",
		description: "Zaokrągla liczbę w dół (w kierunku: do zera).",
		arguments: [
			{
				name: "liczba",
				description: "- dowolna liczba rzeczywista, która ma zostać zaokrąglona w dół"
			},
			{
				name: "liczba_cyfr",
				description: "- liczba cyfr, do której ma zostać zaokrąglona liczba. Wartość ujemna powoduje zaokrąglanie do miejsc po lewej stronie przecinka dziesiętnego; zero lub wartość pominięta oznacza zaokrąglenie do najbliższej liczby całkowitej"
			}
		]
	},
	{
		name: "ZAOKR.GÓRA",
		description: "Zaokrągla liczbę w górę (w kierunku: od zera).",
		arguments: [
			{
				name: "liczba",
				description: "- dowolna liczba rzeczywista, która ma być zaokrąglona w górę"
			},
			{
				name: "liczba_cyfr",
				description: "- liczba cyfr, do której ma zostać zaokrąglona liczba. Wartość ujemna powoduje zaokrąglanie do miejsc po lewej stronie przecinka dziesiętnego; zero lub wartość pominięta oznacza zaokrąglenie do najbliższej liczby całkowitej"
			}
		]
	},
	{
		name: "ZAOKR.W.DÓŁ",
		description: "Zaokrągla liczbę w dół do najbliższej wielokrotności podanej istotności.",
		arguments: [
			{
				name: "liczba",
				description: "- wartość liczbowa, która ma zostać zaokrąglona"
			},
			{
				name: "istotność",
				description: "- określa wielokrotność, do jakiej nastąpi zaokrąglanie. Liczba i Istotność muszą być albo dodatnie, albo obie ujemne"
			}
		]
	},
	{
		name: "ZAOKR.W.DÓŁ.DOKŁ",
		description: "Zaokrągla liczbę w dół do najbliższej liczby całkowitej lub wielokrotności podanej istotności.",
		arguments: [
			{
				name: "liczba",
				description: "- wartość liczbowa, która ma zostać zaokrąglona"
			},
			{
				name: "istotność",
				description: "- wielokrotność, do której należy zaokrąglić wartość"
			}
		]
	},
	{
		name: "ZAOKR.W.DÓŁ.MATEMATYCZNE",
		description: "Zaokrągla liczbę w dół do najbliższej liczby całkowitej lub najbliższej wielokrotności istotności.",
		arguments: [
			{
				name: "liczba",
				description: "— wartość, która ma zostać zaokrąglona."
			},
			{
				name: "istotność",
				description: "— wielokrotność, do jakiej nastąpi zaokrąglanie."
			},
			{
				name: "tryb",
				description: "Gdy podano wartość niezerową, ta funkcja dokona zaokrąglenia w kierunku zera."
			}
		]
	},
	{
		name: "ZAOKR.W.GÓRĘ",
		description: "Zaokrągla liczbę w górę do najbliższej wielokrotności podanej istotności.",
		arguments: [
			{
				name: "liczba",
				description: "- wartość, która ma być zaokrąglona"
			},
			{
				name: "istotność",
				description: "- parametr określający wielokrotność zaokrąglenia"
			}
		]
	},
	{
		name: "ZAOKR.W.GÓRĘ.DOKŁ",
		description: "Zaokrągla liczbę w górę do najbliższej wartości całkowitej lub wielokrotności podanej istotności.",
		arguments: [
			{
				name: "liczba",
				description: "- wartość, która ma być zaokrąglona"
			},
			{
				name: "istotność",
				description: "- wielokrotność, do której należy zaokrąglić wartość"
			}
		]
	},
	{
		name: "ZAOKR.W.GÓRĘ.MATEMATYCZNE",
		description: "Zaokrągla liczbę w górę do najbliższej liczby całkowitej lub najbliższej wielokrotności istotności.",
		arguments: [
			{
				name: "liczba",
				description: "— wartość, która ma zostać zaokrąglona."
			},
			{
				name: "istotność",
				description: "— wielokrotność, do jakiej nastąpi zaokrąglanie."
			},
			{
				name: "tryb",
				description: "Gdy podano wartość niezerową, ta funkcja dokona zaokrąglenia w kierunku od zera."
			}
		]
	},
	{
		name: "ZASTĄP",
		description: "Zamienia część ciągu znaków innym ciągiem znaków.",
		arguments: [
			{
				name: "stary_tekst",
				description: "- tekst, w którym chcesz zamienić niektóre znaki"
			},
			{
				name: "liczba_początkowa",
				description: "- pozycja znaku w starym_tekście, który ma być zamieniony nowym_tekstem"
			},
			{
				name: "liczba_znaków",
				description: "- liczba znaków w starym_tekście, które mają być zamienione"
			},
			{
				name: "nowy_tekst",
				description: "- tekst, który zamieni znaki w starym_tekście"
			}
		]
	},
	{
		name: "ZŁĄCZ.TEKSTY",
		description: "Łączy kilka ciągów tekstowych w jeden ciąg.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tekst1",
				description: "- od 1 do 255 ciągów tekstowych, które mają zostać połączone w jeden ciąg tekstowy; mogą to być ciągi tekstowe, liczby lub odwołania do pojedynczych komórek"
			},
			{
				name: "tekst2",
				description: "- od 1 do 255 ciągów tekstowych, które mają zostać połączone w jeden ciąg tekstowy; mogą to być ciągi tekstowe, liczby lub odwołania do pojedynczych komórek"
			}
		]
	},
	{
		name: "ZNAJDŹ",
		description: "Zwraca pozycję początkową jednego ciągu tekstowego w drugim ciągu tekstowym. Funkcja ZNAJDŹ uwzględnia wielkość liter.",
		arguments: [
			{
				name: "szukany_tekst",
				description: "- tekst do znalezienia. Użyj podwójnego cudzysłowu (pusty tekst), aby dopasować pierwszy znak w argumencie wewnątrz_tekst; symbole wieloznaczne są niedozwolone"
			},
			{
				name: "w_tekście",
				description: "- tekst zawierający szukany tekst"
			},
			{
				name: "liczba_początkowa",
				description: "- określa znak, od którego ma się rozpocząć wyszukiwanie. Pierwszy znak w argumencie wewnątrz_tekst to znak o numerze 1. Jeśli pominięto, liczba_początkowa =1"
			}
		]
	},
	{
		name: "ZNAK",
		description: "Zwraca znak określony przez numer w kodzie zestawu znaków używanego w komputerze.",
		arguments: [
			{
				name: "liczba",
				description: "- liczba z zakresu od 1 do 255 określająca, który znak zostanie zwrócony"
			}
		]
	},
	{
		name: "ZNAK.LICZBY",
		description: "Zwraca znak podanej liczby: 1, jeśli liczba jest dodatnia, zero, jeśli jest równa zero lub -1, jeśli jest ujemna.",
		arguments: [
			{
				name: "liczba",
				description: "- dowolna liczba rzeczywista"
			}
		]
	}
];