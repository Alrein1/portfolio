namespace ConsoleUI;

public static class Helpers
{
    // TODO: Add default value
    public static string? InputString(string prompt, int minLength = 1, int maxLength = 32, bool allowCancel = true)
    {
        do
        {
            Console.Write(prompt);
            if (allowCancel)
            {
                Console.Write(" (empty to cancel)");
            }

            Console.Write(":");


            var res = Console.ReadLine();

            if (allowCancel && res is { Length: 0 }) return null;

            if (string.IsNullOrWhiteSpace(res))
            {
                Console.WriteLine("Input can not be null or whitespace!");
                continue;
            }

            res = res.Trim();

            if (res.Length < minLength)
            {
                Console.WriteLine("Input too short!");
                continue;
            }

            if (res.Length > maxLength)
            {
                Console.WriteLine("Input too long!");
                continue;
            }

            return res;
        } while (true);
    }
 
    public static int? InputInt(string prompt, int? defaultValue = null, int min = int.MinValue, int max = int.MaxValue, bool allowCancel = true)
    {
        do
        {
            Console.Write(prompt);

            switch (min, max)
            {
                case (int.MinValue, int.MaxValue): 
                    break;
                case (int.MinValue, _):
                    Console.Write($"(max: {max})");
                    break;
                case (_, int.MaxValue):
                    Console.Write($"(min: {min})");
                    break;
                case (_, _):
                    Console.Write($"(min: {min}, max: {max})");
                    break;
            }

            if (allowCancel)
            {
                Console.Write(" (- to cancel)");
            }

            if (defaultValue.HasValue)
            {
                Console.Write($"[{defaultValue.Value}]");
            }

            Console.Write(":");

            var res = Console.ReadLine();

            if (allowCancel && res != null && res.Trim().Equals("-")) return null;
            
            if (string.IsNullOrWhiteSpace(res))
            {
                if (defaultValue == null)
                {
                    Console.WriteLine("Input can not be null or whitespace!");
                    continue;
                }
                res = defaultValue.Value.ToString();
            }

            res = res.Trim();
            if (!int.TryParse(res, out var resValue))
            {
                Console.WriteLine("Input is not an number!");
                continue;
            }

            if (resValue < min)
            {
                Console.WriteLine("Input too small!");
                continue;
            }

            if (resValue > max)
            {
                Console.WriteLine("Input too big!");
                continue;
            }

            return resValue;
        } while (true);
    }
}