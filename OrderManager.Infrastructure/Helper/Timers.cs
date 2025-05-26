namespace OrderManager.Infrastructure.Helper;

public static class Timers
{
    public static int GetMsByMinutes(int minutes) => minutes * 60 * 1000;
}