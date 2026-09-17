namespace Task1;

public static class HistogramPrinter
{
    public static void PrintHistogram(
        this HistogramBin[]? bins,
        string? title = null)
    {
        if (bins == null || bins.Length == 0)
        {
            Console.WriteLine("Гістограма пуста.");
            return;
        }

        if (!string.IsNullOrWhiteSpace(title))
        {
            Console.WriteLine($"\n=== {title} ===");
        }

        int totalCount = bins.Sum(b => b.Count);
        int maxCount = bins.Max(b => b.Count);

        int countWidth = Math.Max("Кількість".Length, maxCount.ToString().Length);
        
        int leftWidth = bins.Max(b => b.Left.ToString("F4").Length);
        int rightWidth = bins.Max(b => b.Right.ToString("F4").Length);
        int intervalWidth = Math.Max("Інтервал".Length, leftWidth + rightWidth + 4); // "[", ", ", "]"

        string header = $"{"Інтервал".PadRight(intervalWidth)} | {"Кількість".PadLeft(countWidth)} | {"Відсоток",8}";
        string separator = new string('-', header.Length);

        Console.WriteLine(separator);
        Console.WriteLine(header);
        Console.WriteLine(separator);

        for (int i = 0; i < bins.Length; i++)
        {
            var bin = bins[i];
            char closingBracket = (i == bins.Length - 1) ? ']' : ')';
            string interval = $"[{bin.Left.ToString("F4").PadLeft(leftWidth)}, {bin.Right.ToString("F4").PadLeft(rightWidth)}{closingBracket}";

            double percent = totalCount > 0 ? (double)bin.Count / totalCount * 100.0 : 0.0;

            Console.WriteLine($"{interval.PadRight(intervalWidth)} | {bin.Count.ToString().PadLeft(countWidth)} | {percent,7:F2}%");
        }

        Console.WriteLine(separator);
        Console.WriteLine($"Загальна кількість: {totalCount} | Діапазон: {bins.Length}");
    }

    /// <summary>
    /// Overload that accepts an IEnumerable of HistogramBin.
    /// </summary>
    public static void PrintHistogram(
        this IEnumerable<HistogramBin>? bins,
        string? title = null)
    {
        PrintHistogram(bins as HistogramBin[] ?? bins?.ToArray(), title);
    }
}