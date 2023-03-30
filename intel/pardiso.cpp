// Includes
#include "stdafx.h"

//------------------------------------------------------------------------
ofstream logfile;

//------------------------------------------------------------------------
int Write_Txt_File_Of_Complex(const char* fname, MKL_Complex16* massiv, int n_of_records, int len_of_record)
{
	FILE* fp;

	if ((fp = fopen(fname, "w")) == 0) {
		printf("Error: Cannot open file \"%s\" for writing.\n", fname);
		return 1;
	}

	printf("writing %s... ", fname);

	for (int i = 0; i < n_of_records; i++) {
		for (int j = 0; j < len_of_record; j++)
			fprintf(fp, "%25.13e\t%25.13e", massiv[i * len_of_record + j].real, massiv[i * len_of_record + j].imag);
		fprintf(fp, "\n");
	}
	printf("done\n");

	fclose(fp);
	return 0;
}

//------------------------------------------------------------------------
int Write_Txt_File_Of_Complex(const char* fname, complex<double>* massiv, int n_of_records, int len_of_record)
{
	FILE* fp;

	if ((fp = fopen(fname, "w")) == 0) {
		printf("Error: Cannot open file \"%s\" for writing.\n", fname);
		return 1;
	}

	printf("writing %s... ", fname);

	for (int i = 0; i < n_of_records; i++) {
		for (int j = 0; j < len_of_record; j++)
			fprintf(fp, "%25.13e\t%25.13e", massiv[i * len_of_record + j].real(), massiv[i * len_of_record + j].imag());
		fprintf(fp, "\n");
	}
	printf("done\n");

	fclose(fp);
	return 0;
}

//------------------------------------------------------------------------
int Write_Txt_File_Of_Long(const char* fname, int* massiv, int n_of_records)
{
	FILE* fp;

	if ((fp = fopen(fname, "w")) == 0) {
		printf("Error: Cannot open file \"%s\" for writing.\n", fname);
		return 1;
	}

	printf("writing %s... ", fname);

	for (int i = 0; i < n_of_records; i++)
		fprintf(fp, "%d\n", massiv[i]);
	printf("done\n");

	fclose(fp);
	return 0;
}

//------------------------------------------------------------------------
int Read_Long_From_Txt_File(const char* fname, int* number)
{
	FILE* fp;
	if ((fp = fopen(fname, "r")) == 0) {
		char str[100];
		sprintf(str, "Error: Cannot open file \"%s\" for reading.\n", fname);
		return 1;
	}

	int temp;
	int retcode = fscanf(fp, "%ld", &temp);
	if (retcode != 1) {
		char str[100];
		sprintf(str, "Error reading file \"%s\".\n", fname);
		fclose(fp);
	}
	*number = temp;

	fclose(fp);
	return 0;
}

//------------------------------------------------------------------------
int Read_Bin_File_Of_Complex(const char* fname, MKL_Complex16* massiv, int n_of_records, int len_of_record)
{
	FILE* fp;
	if ((fp = fopen(fname, "r+b")) == 0) {
		char str[100];
		sprintf(str, "Error: Cannot open file \"%s\" for reading.\n", fname);
		return 1;
	}

	double* temp_massiv = new double[2 * n_of_records];
	int temp = fread(temp_massiv, sizeof(double) * len_of_record, 2 * n_of_records, fp);
	if (temp != 2 * n_of_records) {
		char str[100];
		sprintf(str, "Error reading file \"%s\". %ld of %ld records was read.\n", fname, temp, 2 * n_of_records);
		fclose(fp);
		return 1;
	}

	for (int i = 0, id = 0; i < 2 * n_of_records - 1; i += 2, id++)
		massiv[id] = MKL_Complex16{ temp_massiv[i], temp_massiv[i + 1] };

	fclose(fp);
	return 0;
}

//------------------------------------------------------------------------
int Read_Bin_File_Of_Complex(const char* fname, complex<double>* massiv, int n_of_records, int len_of_record)
{
	FILE* fp;
	if ((fp = fopen(fname, "r+b")) == 0) {
		char str[100];
		sprintf(str, "Error: Cannot open file \"%s\" for reading.\n", fname);
		return 1;
	}

	double* temp_massiv = new double[2 * n_of_records];
	int temp = fread(temp_massiv, sizeof(double) * len_of_record, 2 * n_of_records, fp);
	if (temp != 2 * n_of_records) {
		char str[100];
		sprintf(str, "Error reading file \"%s\". %ld of %ld records was read.\n", fname, temp, 2 * n_of_records);
		fclose(fp);
		return 1;
	}

	for (int i = 0, id = 0; i < 2 * n_of_records - 1; i += 2, id++)
		massiv[id] = complex<double>{ temp_massiv[i], temp_massiv[i + 1] };

	fclose(fp);
	return 0;
}

//------------------------------------------------------------------------
int Read_Bin_File_Of_Long(const char* fname, int* massiv, int n_of_records, int len_of_record)
{
	FILE* fp;

	if ((fp = fopen(fname, "r+b")) == 0) {
		char str[100];
		sprintf(str, "Cannot open file %s.\n", fname);
		return 1;
	}

	int temp = fread(massiv, sizeof(int) * len_of_record, n_of_records, fp);
	if (temp != n_of_records) {
		char str[100];
		sprintf(str, "Error reading file \"%s\". %ld of %ld records was read.\n", fname, temp, n_of_records);
		fclose(fp);
		return 1;
	}

	fclose(fp);
	return 0;
}

//------------------------------------------------------------------------
void FromRSFToCSR_Real_1_Sym(int nb, int* ig, int* sz_ia, int* sz_ja)
{
	*sz_ia = nb + 1;
	*sz_ja = ig[nb] + nb;
}

//------------------------------------------------------------------------
void FromRSFToCSR_Real_2_Sym(int nb, int* ig, int* jg, complex<double>* di, complex<double>* gg,
	MKL_INT* ia, MKL_INT* ja, MKL_Complex16* a)
{
	vector<MKL_INT> adr;

	// подсчитываем число элементов в каждой строчке
	adr.resize(nb, 0);

	for (int i = 0; i < nb; i++) {
		adr[i] += 1; // диагональ
		// верхний треугольник
		for (int j = ig[i]; j <= ig[i + 1] - 1; j++)
			adr[jg[j]]++;
	}

	// ia
	ia[0] = 0;
	for (int i = 0; i < nb; i++)
		ia[i + 1] = ia[i] + adr[i];

	// ja,  a
	for (int i = 0; i < ig[nb] + nb; i++)
		a[i] = MKL_Complex16{ 0, 0 };

	for (int i = 0; i < nb; i++)
		adr[i] = ia[i]; // в какую позицию заносить значение

	// диагональ
	for (int i = 0; i < nb; i++) {
		ja[adr[i]] = i;
		a[adr[i]] = MKL_Complex16{ di[i].real(), di[i].imag() };
		adr[i]++;
	}

	// верхний треугольник
	for (int i = 0; i < nb; i++) {
		for (int j = ig[i]; j <= ig[i + 1] - 1; j++) {
			int k = jg[j];
			ja[adr[k]] = i;
			a[adr[k]] = MKL_Complex16{ gg[j].real(), gg[j].imag() };
			adr[k]++;
		}
	}
}

//------------------------------------------------------------------------
int _tmain(int argc, _TCHAR* argv[])
{
	// Путь к СЛАУ
	string path = "test/";

	logfile.open(path + "pardiso64.log");
	if (!logfile) {
		cerr << "Cannot open pardiso64.log" << endl;
		return 1;
	}

	int ig_n_1 = 0;
	int sz_ia = 0;
	int sz_ja = 0;

	MKL_Complex16* pr = NULL;
	MKL_Complex16* x = NULL;

	// Исходная матрица в разреженном строчно-столбцовом формате
	int nb;
	int* ig = NULL;
	int* jg = NULL;
	complex<double>* di = NULL;
	complex<double>* gg = NULL;

	// Матрица, сконвертированная в разреженный строчный формат
	MKL_INT* ia = NULL;
	MKL_INT* ja = NULL;
	MKL_Complex16* a = NULL;

	// Для PARDISO
	clock_t begin = clock();
	MKL_INT n = 0;
	MKL_INT mtype = 6; // комплексная симметричная
	MKL_INT nrhs = 1;
	void* pt[64];
	MKL_INT maxfct = 1;
	MKL_INT	mnum = 1;
	MKL_INT msglvl = 1;
	MKL_INT phase = 13;
	MKL_INT* perm = NULL;
	MKL_INT iparm[64];
	MKL_INT info = -100;

	for (int i = 0; i < 64; i++)
		pt[i] = 0;

	iparm[0] = 0; //iparm(2) - iparm(64) заполняются значениями по умолчанию.
	//iparm[0] = 1; // Нужно указать все значения в компонентах iparm(2) - iparm(64).
	for (int i = 1; i < 64; i++)
		iparm[i] = 0;

	// Чтение размерности nb
	Read_Long_From_Txt_File((path + "kuslau").c_str(), &nb);

	// ig
	ig = new int[nb + 1];
	Read_Bin_File_Of_Long((path + "ig").c_str(), ig, nb + 1, 1);
	Write_Txt_File_Of_Long((path + "ig.txt").c_str(), ig, nb + 1);
	ig_n_1 = ig[nb];

	// jg
	jg = new int[ig_n_1];
	Read_Bin_File_Of_Long((path + "jg").c_str(), jg, ig_n_1, 1);
	Write_Txt_File_Of_Long((path + "jg.txt").c_str(), jg, ig_n_1);

	// di
	di = new complex<double>[nb];
	Read_Bin_File_Of_Complex((path + "di").c_str(), di, nb, 1);
	Write_Txt_File_Of_Complex((path + "di.txt").c_str(), di, nb, 1);

	// gg
	gg = new complex<double>[ig_n_1];
	Read_Bin_File_Of_Complex((path + "gg").c_str(), gg, ig_n_1, 1);
	Write_Txt_File_Of_Complex((path + "gg.txt").c_str(), gg, ig_n_1, 1);

	// pr
	pr = new MKL_Complex16[nb];
	Read_Bin_File_Of_Complex((path + "pr").c_str(), pr, nb, 1);
	Write_Txt_File_Of_Complex((path + "pr.txt").c_str(), pr, nb, 1);

	// Конвертирование в CSR
	FromRSFToCSR_Real_1_Sym(nb, ig, &sz_ia, &sz_ja);
	ia = new MKL_INT[sz_ia];
	ja = new MKL_INT[sz_ja];
	a = new MKL_Complex16[sz_ja];
	FromRSFToCSR_Real_2_Sym(nb, ig, jg, di, gg, ia, ja, a);
	Write_Txt_File_Of_Complex((path + "a.txt").c_str(), a, sz_ja, 1);
	Write_Txt_File_Of_Long((path + "ia.txt").c_str(), ia, sz_ia);
	Write_Txt_File_Of_Long((path + "ja.txt").c_str(), ja, sz_ja);

	// Вот это он решает
	//nb = 5;
	//ia = new MKL_INT[nb + 1]{ 1, 4, 5, 8, 9, 10 };
	//ja = new MKL_INT[ia[nb]]{ 1, 2, 4, 2, 3, 4, 5, 4, 5 };
	//a = new MKL_Complex16[ia[nb]]{ {1, 0}, {-1, 0}, {-3, 0}, {5, 0}, {4, 0}, {6, 0}, {4, 0}, {7, 0}, {-5, 1} };
	//pr = new MKL_Complex16[nb]{ {-13, 0}, {9, 0}, {56, 0}, {43, 0}, {-13, 5}};

	for (int i = 0; i < sz_ia; i++)
		ia[i]++;

	for (int i = 0; i < sz_ja; i++)
		ja[i]++;

	if (ig) { delete[] ig; ig = NULL; }
	if (jg) { delete[] jg; jg = NULL; }
	if (di) { delete[] di; di = NULL; }
	if (gg) { delete[] gg; gg = NULL; }

	// Запуск PARDISO
	n = nb;
	x = new MKL_Complex16[nb];
	perm = new MKL_INT[nb];

	cout << "pardiso start.." << endl << flush;

	PARDISO(pt, &maxfct, &mnum, &mtype, &phase, &n, a, ia, ja, perm, &nrhs, iparm, &msglvl, pr, x, &info);

	clock_t time = (clock() - begin) / CLOCKS_PER_SEC;

	ofstream fout;
	fout.open((path + "kit").c_str(), ios_base::app);
	fout << "n=" << nb << " pardiso time=" << time << " s" << endl;
	fout.close();

	// Запись решения
	Write_Txt_File_Of_Complex((path + "x.txt").c_str(), x, nb, 1);

	// Запись и вывод info
	cout << "info=" << info << endl;
	logfile << "info=" << info << endl;

	if (a) { delete[] a; a = NULL; }
	if (x) { delete[] x; x = NULL; }
	if (pr) { delete[] pr; pr = NULL; }
	if (ja) { delete[] ja; ja = NULL; }
	if (ia) { delete[] ia; ia = NULL; }
	if (perm) { delete[] perm; perm = NULL; }

	logfile.close();
	logfile.clear();
	return 0;
}