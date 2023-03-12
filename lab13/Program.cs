using System.Diagnostics;

int[] arr = { 1, 14, 47, 96, 45, 6, 18, 3, 25, 30, 12, 10, 36, 48, 5, 17 };
int p = 8;
Stopwatch stopwatch = new();
stopwatch.Start();
int[] sortedArr = ShellSortParallel(arr, p);
stopwatch.Stop();

Console.WriteLine("Sorted array:");
for (int i = 0; i < sortedArr.Length; i++)
{
    Console.Write(sortedArr[i] + " ");
}
Console.WriteLine("");
Console.WriteLine("Execution time of the ShellSortParallel alghorithm: {0}", stopwatch.Elapsed.TotalMilliseconds);


int[] ShellSortParallel(int[] arr, int p)
{
    int n = arr.Length;

    // Перший етап: взаємодія процесорів
    int index = n / 2;
    while(index > 0)
    {
        int currentGap = index;
        int mask = currentGap - 1;

        Parallel.For(0, p / 2, j =>
        {
            int proc1 = j ^ mask;
            int proc2 = j;

            while (proc1 < p && proc2 < n)
            {
                if (arr[proc1] < arr[proc2])
                {
                    Swap(arr, proc1, proc2);
                }
                proc1 += currentGap;
                proc2 += currentGap;
            }
        });
        index /= 2;
    }
    for (int l = p; l >= 1; l /= 2)
    {
        Task[] tasks = new Task[l];
        for (int i = 0; i < l; i++)
        {
            int startIndex = i * 2;
            int endIndex = i == l - 1 ? arr.Length : (i + 1) * 2;
            tasks[i] = Task.Factory.StartNew(() => SortSubArray(arr, startIndex, endIndex));
        }

        // Чекаємо на завершення всіх тасків
        Task.WaitAll(tasks);
    }
    // Другий етап: парно-непарна перестановка
    /*bool isSorted = false;
    int start = 0;

    while (!isSorted)
    {
        isSorted = true;

        for (int i = start % 2; i < n - 1; i += 2)
        {
            if (arr[i] > arr[i + 1])
            {
                Swap(arr, i, i + 1);
                isSorted = false;
            }
        }
        start++;
    }*/
    return arr;
}

void SortSubArray(int[] arr, int startIndex, int endIndex)
{
    bool swapped = true;
    while (swapped)
    {
        // Непарна ітерація
        swapped = false;
        for (int i = startIndex + 1; i < endIndex - 1; i += 2)
        {
            if (arr[i] > arr[i + 1])
            {
                Swap(arr, i, i + 1);
                swapped = true;
            }
        }

        // Парна ітерація
        for (int i = startIndex; i < endIndex - 1; i += 2)
        {
            if (arr[i] > arr[i + 1])
            {
                Swap(arr, i, i + 1);
                swapped = true;
            }
        }
    }
}

void Swap(int[] arr, int i, int j)
{
    int temp = arr[i];
    arr[i] = arr[j];
    arr[j] = temp;
}